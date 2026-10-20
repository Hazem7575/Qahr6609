// Asynchronous per-module log writer used by ModuleBase.Log().
// One background thread, one bounded queue, one StreamWriter per module per day:
//   <DbLocation>\Modules\<Name>\logs\yyyyMMdd.log
// Enqueue never blocks: when the queue is full the line is dropped (and counted).
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Conquer6609.Framework.Modules
{
    internal static class ModuleLog
    {
        struct Entry
        {
            public ModuleBase Module;
            public DateTime Time;
            public string Text;
        }

        sealed class Writer
        {
            public StreamWriter Stream;
            public string Day;
            public int LastUse;
        }

        const int QueueCapacity = 50000;
        const int IdleCloseMs = 30000;

        static readonly BlockingCollection<Entry> queue = new BlockingCollection<Entry>(new ConcurrentQueue<Entry>(), QueueCapacity);
        static readonly Dictionary<ModuleBase, Writer> writers = new Dictionary<ModuleBase, Writer>(); // writer thread only
        static readonly object startLock = new object();
        static readonly UTF8Encoding utf8 = new UTF8Encoding(false);
        static volatile Thread thread;
        static int pending; // queued + being written (not yet flushed)
        public static long Dropped;

        public static void Enqueue(ModuleBase module, string text)
        {
            try
            {
                if (module == null) return;
                if (thread == null) Start();
                var e = new Entry { Module = module, Time = DateTime.Now, Text = text ?? "" };
                Interlocked.Increment(ref pending);
                if (!queue.TryAdd(e))
                {
                    Interlocked.Decrement(ref pending);
                    Interlocked.Increment(ref Dropped);
                }
            }
            catch { }
        }

        static void Start()
        {
            lock (startLock)
            {
                if (thread != null) return;
                var t = new Thread(Run) { IsBackground = true, Name = "ModuleLog", Priority = ThreadPriority.BelowNormal };
                t.Start();
                thread = t;
            }
        }

        static void Run()
        {
            for (; ; )
            {
                try
                {
                    Entry e;
                    if (queue.TryTake(out e, 2000))
                    {
                        int n = 1;
                        Write(e);
                        while (n < 5000 && queue.TryTake(out e))
                        {
                            Write(e);
                            n++;
                        }
                        FlushAll();
                        Interlocked.Add(ref pending, -n);
                    }
                    else
                        CloseIdle();
                }
                catch { }
            }
        }

        static void Write(Entry e)
        {
            try
            {
                string day = e.Time.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                Writer w;
                if (!writers.TryGetValue(e.Module, out w))
                {
                    w = new Writer();
                    writers[e.Module] = w;
                }
                if (w.Stream == null || w.Day != day)
                {
                    if (w.Stream != null) { try { w.Stream.Dispose(); } catch { } w.Stream = null; }
                    string dir = Path.Combine(e.Module.DataPath, "logs");
                    Directory.CreateDirectory(dir);
                    var fs = new FileStream(Path.Combine(dir, day + ".log"), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    w.Stream = new StreamWriter(fs, utf8);
                    w.Day = day;
                }
                w.LastUse = Environment.TickCount;
                w.Stream.Write(e.Time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture));
                w.Stream.Write(' ');
                w.Stream.Write(e.Text);
                w.Stream.Write("\r\n");
            }
            catch { }
        }

        static void FlushAll()
        {
            foreach (var w in writers.Values)
            {
                if (w.Stream == null) continue;
                try { w.Stream.Flush(); } catch { }
            }
        }

        static void CloseIdle()
        {
            int now = Environment.TickCount;
            foreach (var w in writers.Values)
            {
                if (w.Stream == null) continue;
                if (unchecked(now - w.LastUse) < IdleCloseMs) continue;
                try { w.Stream.Dispose(); } catch { }
                w.Stream = null;
            }
        }

        /// <summary>Waits (up to timeoutMs) until every queued line has been written and flushed.</summary>
        public static void Flush(int timeoutMs)
        {
            if (thread == null) return;
            int start = Environment.TickCount;
            while (Volatile.Read(ref pending) > 0)
            {
                if (unchecked(Environment.TickCount - start) > timeoutMs) break;
                Thread.Sleep(10);
            }
        }
    }
}
