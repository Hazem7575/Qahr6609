// ============================================================================
//  MirrorQueue - background queue for the MySQL copy of player data (PlayerStorage / PlayerBlobStorage),
//  so the UPSERTs never run on a game thread. Same design as Qahr's MirrorQueue.
//
//  shell.ini [Storage]:
//    AsyncMirror=1             1 = this queue (default) | 0 = run the DB work right away on the caller
//    AsyncMirrorWorkers=2      worker threads (a key always goes to the same worker -> its order is kept)
//    AsyncMirrorMaxQueue=20000 max pending operations (beyond that a save for a NEW key is dropped with a warning)
//    AsyncMirrorFlushSec=8     how long shutdown waits for the queue to empty
//
//  * Submit(key, tag, supersede, snapshot, what): snapshot() runs on the caller's thread (reads the file the save
//    just wrote, same file access as before) and returns the DB work, which runs on a worker. snapshot + enqueue
//    happen under a lock striped by key, so the queue order of a key = the order its file was read.
//  * Coalescing per key (players:UID, blob:Store:UID): a pending op with the same tag is replaced by the newer
//    one. Different ops of a key run in order on the same worker; supersede (delete) drops what is pending.
//  * A failed op only prints a warning (the files are saved) and marks the key stale: IsDirty() then tells the
//    DB readers to use the file until a later save of that key succeeds.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Threading;

namespace Conquer6609.Database.Storage
{
    public static class MirrorQueue
    {
        public static bool Async { get; private set; }
        private static int _maxPending = 20000;
        private static int _flushSec = 8;
        public static long Done, Failed;

        private sealed class Op
        {
            public string Tag;
            public Action Work;
            public string What;
        }

        private sealed class Shard
        {
            public readonly object Sync = new object();
            public readonly Queue<string> Order = new Queue<string>();
            public readonly Dictionary<string, List<Op>> Pending = new Dictionary<string, List<Op>>(StringComparer.Ordinal);
            public bool Busy;
            public string RunningKey;
        }

        // keys whose DB copy is not guaranteed to be the latest (failed / dropped / snapshot error)
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, byte> _stale =
            new System.Collections.Concurrent.ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
        private static void MarkStale(string key) { if (key != null) _stale[key] = 1; }
        private static void MarkFresh(string key) { byte b; if (key != null) _stale.TryRemove(key, out b); }

        /// <summary>True when the DB may be behind the file for this key (a save is queued or running, or the last
        /// one failed). A reader that would read from the DB must read the file instead.</summary>
        public static bool IsDirty(string key)
        {
            if (_stale.ContainsKey(key)) return true;
            Shard[] shards = _shards;
            if (!Async || shards == null) return false;
            Shard sh = shards[Hash(key) % shards.Length];
            lock (sh.Sync)
                return sh.Pending.ContainsKey(key) || string.Equals(sh.RunningKey, key, StringComparison.Ordinal);
        }

        private static Shard[] _shards;
        private static int _pendingOps;
        private static long _dropped;
        private static long _lastWarnTicks;
        private static readonly object[] _stripes = CreateStripes(64);
        private static readonly object _initLock = new object();

        private static object[] CreateStripes(int n)
        {
            var a = new object[n];
            for (int i = 0; i < n; i++) a[i] = new object();
            return a;
        }

        /// <summary>Reads [Storage] AsyncMirror* and starts the workers. Called by PlayerStorage.Init.</summary>
        public static void Init()
        {
            lock (_initLock)
            {
                if (_shards != null) return;
                bool on = Framework.Settings.Bool("Storage", "AsyncMirror", true);
                int workers = Framework.Settings.Int("Storage", "AsyncMirrorWorkers", 2);
                _maxPending = Framework.Settings.Int("Storage", "AsyncMirrorMaxQueue", 20000);
                _flushSec = Framework.Settings.Int("Storage", "AsyncMirrorFlushSec", 8);
                if (!on) { Async = false; return; }

                if (workers < 1) workers = 1;
                if (workers > 8) workers = 8;
                if (_maxPending < 100) _maxPending = 100;
                if (_flushSec < 0) _flushSec = 0;

                var shards = new Shard[workers];
                for (int i = 0; i < workers; i++)
                {
                    var sh = new Shard();
                    shards[i] = sh;
                    var t = new Thread(() => WorkerLoop(sh));
                    t.IsBackground = true;
                    t.Name = "StorageMirror-" + i;
                    t.Start();
                }
                _shards = shards;
                Async = true;
            }
        }

        public static int PendingCount { get { return Volatile.Read(ref _pendingOps); } }

        private static int Hash(string key)
        {
            unchecked
            {
                int h = 17;
                for (int i = 0; i < key.Length; i++) h = h * 31 + key[i];
                return h & 0x7fffffff;
            }
        }

        /// <summary>snapshot runs now on the caller's thread (reads the file) and returns the DB work (or null = nothing
        /// to do). AsyncMirror=0: snapshot and work both run right away on the caller.</summary>
        public static void Submit(string key, string tag, bool supersede, Func<Action> snapshot, string what)
        {
            Shard[] shards = _shards;
            if (!Async || shards == null)
            {
                try
                {
                    Action w = snapshot();
                    if (w != null) { w(); MarkFresh(key); Interlocked.Increment(ref Done); }
                }
                catch (Exception e)
                {
                    MarkStale(key);
                    Interlocked.Increment(ref Failed);
                    Warn(what + ": " + e.Message);
                }
                return;
            }

            int h = Hash(key);
            lock (_stripes[h % _stripes.Length])
            {
                Action work;
                try { work = snapshot(); }
                catch (Exception e)
                {
                    MarkStale(key);
                    Interlocked.Increment(ref Failed);
                    Warn(what + ": " + e.Message);
                    return;
                }
                if (work == null) return;
                Enqueue(shards[h % shards.Length], key, new Op { Tag = tag, Work = work, What = what }, supersede);
            }
        }

        private static void Enqueue(Shard sh, string key, Op op, bool supersede)
        {
            bool dropped = false;
            lock (sh.Sync)
            {
                List<Op> list;
                if (sh.Pending.TryGetValue(key, out list))
                {
                    if (supersede)
                    {
                        Interlocked.Add(ref _pendingOps, 1 - list.Count);
                        list.Clear();
                        list.Add(op);
                    }
                    else if (list.Count > 0 && list[list.Count - 1].Tag == op.Tag)
                        list[list.Count - 1] = op;// coalesce: the newer snapshot replaces the older one
                    else
                    {
                        list.Add(op);
                        Interlocked.Increment(ref _pendingOps);
                    }
                }
                else if (!supersede && Volatile.Read(ref _pendingOps) >= _maxPending)
                    dropped = true;// deletes are never dropped
                else
                {
                    list = new List<Op>(2);
                    list.Add(op);
                    sh.Pending[key] = list;
                    sh.Order.Enqueue(key);
                    Interlocked.Increment(ref _pendingOps);
                    Monitor.PulseAll(sh.Sync);
                }
            }

            if (dropped)
            {
                MarkStale(key);
                Interlocked.Increment(ref Failed);
                long n = Interlocked.Increment(ref _dropped);
                long now = DateTime.UtcNow.Ticks;
                long last = Interlocked.Read(ref _lastWarnTicks);
                if (now - last > TimeSpan.TicksPerSecond * 30 && Interlocked.CompareExchange(ref _lastWarnTicks, now, last) == last)
                    Warn("DB mirror queue full (" + _maxPending + "), dropped " + n + " update(s) so far - files are saved, the DB catches up on the next save. Is MySQL slow/down?");
            }
        }

        private static void WorkerLoop(Shard sh)
        {
            while (true)
            {
                List<Op> ops;
                string key;
                lock (sh.Sync)
                {
                    while (sh.Order.Count == 0)
                        Monitor.Wait(sh.Sync);
                    key = sh.Order.Dequeue();
                    if (!sh.Pending.TryGetValue(key, out ops)) continue;
                    sh.Pending.Remove(key);// a new request for this key queues again behind this run
                    sh.Busy = true;
                    sh.RunningKey = key;
                }
                try
                {
                    for (int i = 0; i < ops.Count; i++)
                    {
                        Op op = ops[i];
                        try
                        {
                            op.Work();
                            MarkFresh(key);
                            Interlocked.Increment(ref Done);
                        }
                        catch (Exception e)
                        {
                            MarkStale(key);
                            Interlocked.Increment(ref Failed);
                            Warn(op.What + ": " + e.Message);
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _pendingOps);
                        }
                    }
                }
                catch { }
                finally
                {
                    lock (sh.Sync)
                    {
                        sh.Busy = false;
                        sh.RunningKey = null;
                        Monitor.PulseAll(sh.Sync);
                    }
                }
            }
        }

        /// <summary>Shutdown: waits until the queue is empty or [Storage] AsyncMirrorFlushSec is over.</summary>
        public static bool Flush()
        {
            return Flush(_flushSec * 1000);
        }

        public static bool Flush(int timeoutMs)
        {
            Shard[] shards = _shards;
            if (!Async || shards == null) return true;
            int start = Environment.TickCount;
            bool ok = true;
            try
            {
                foreach (var sh in shards)
                {
                    lock (sh.Sync)
                    {
                        while (sh.Order.Count > 0 || sh.Busy)
                        {
                            int left = timeoutMs - unchecked(Environment.TickCount - start);
                            if (left <= 0) { ok = false; break; }
                            Monitor.Wait(sh.Sync, left);
                        }
                    }
                    if (!ok) break;
                }
                if (ok) MyConsole.WriteLine("[Storage] DB mirror queue flushed.");
                else Warn("DB mirror flush timed out, " + PendingCount + " update(s) not mirrored (files are saved).");
            }
            catch { }
            return ok;
        }

        internal static void Warn(string text)
        {
            try { MyConsole.WriteLine("[Storage] " + text, ConsoleColor.Yellow); } catch { }
        }
    }
}
