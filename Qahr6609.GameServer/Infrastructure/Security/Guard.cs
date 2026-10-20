// Shared security helpers (phase 1 hardening).
// Keep this file dependency-light: it is also compiled by tests/Security.Tests.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Conquer6609.Security
{
    public static class Guard
    {
        // ------------------------------------------------------------ settings ([Security] in shell.ini)
        public static int StrictNpcOptions = 1;          // 0 = off, 1 = audit only (first rollout), 2 = enforce
        public static long MaxMoneyAmount = 10000000000;  // per single money operation (trade, warehouse, vendor price...)
        public static long MaxWarehouseMoney = 50000000000;
        public static long MaxCpsAmount = 1000000000;     // per single CP operation
        public static long MaxDonation = 10000000000;
        public static int MaxJumpDistance = 18;
        public static int MinJumpIntervalMs = 300;
        public static int FastJumpViolationLimit = 3;     // fast jumps (< MinJumpIntervalMs) tolerated ...
        public static int FastJumpWindowMs = 5000;        // ... within this window before they are pulled back
        public static int VipTeleportMinLevel = 1;        // minimum active VIP level (own or shared) for VIP teleports
        public static int VipTeamTeleportSameMap = 1;     // 1 = team teleport invites only reach members on the same map
        public static int NpcDialogTimeoutSec = 300;
        static int loaded;

        public static void Load(string shellIniPath)
        {
            try
            {
                if (!File.Exists(shellIniPath)) return;
                bool inSec = false;
                foreach (var raw in File.ReadAllLines(shellIniPath))
                {
                    var line = raw.Trim();
                    if (line.StartsWith("[")) { inSec = line.Equals("[Security]", StringComparison.OrdinalIgnoreCase); continue; }
                    if (!inSec) continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = line.Substring(0, eq).Trim(), v = line.Substring(eq + 1).Trim();
                    long n;
                    if (!long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) continue;
                    switch (k.ToLowerInvariant())
                    {
                        case "strictnpcoptions": StrictNpcOptions = (int)Math.Max(0, Math.Min(2, n)); break;
                        case "maxmoneyamount": if (n > 0) MaxMoneyAmount = n; break;
                        case "maxwarehousemoney": if (n > 0) MaxWarehouseMoney = n; break;
                        case "maxcpsamount": if (n > 0) MaxCpsAmount = n; break;
                        case "maxdonation": if (n > 0) MaxDonation = n; break;
                        case "maxjumpdistance": if (n > 0) MaxJumpDistance = (int)n; break;
                        case "minjumpintervalms": if (n >= 0) MinJumpIntervalMs = (int)n; break;
                        case "fastjumpviolationlimit": if (n > 0) FastJumpViolationLimit = (int)n; break;
                        case "fastjumpwindowms": if (n > 0) FastJumpWindowMs = (int)n; break;
                        case "vipteleportminlevel": if (n >= 1) VipTeleportMinLevel = (int)Math.Min(n, 255); break;
                        case "vipteamteleportsamemap": VipTeamTeleportSameMap = n != 0 ? 1 : 0; break;
                        case "npcdialogtimeoutsec": if (n > 0) NpcDialogTimeoutSec = (int)n; break;
                    }
                }
            }
            catch (Exception e) { Audit("security-config", 0, "", "load failed: " + e.Message); }
        }

        public static void EnsureLoaded()
        {
            if (Interlocked.Exchange(ref loaded, 1) == 0)
                Load(Path.Combine(Directory.GetCurrentDirectory(), "shell.ini"));
        }

        // ------------------------------------------------------------ amounts
        /// <summary>Validates a client supplied 64-bit amount against the full value (never a truncated cast).
        /// Rejects 0, values above long.MaxValue and values above max.</summary>
        public static bool ValidAmount(ulong value, long max, out long amount)
        {
            amount = 0;
            if (value == 0 || value > (ulong)long.MaxValue || max <= 0 || value > (ulong)max) return false;
            amount = (long)value;
            return true;
        }

        public static bool ValidAmount(long value, long max)
        {
            return value > 0 && max > 0 && value <= max;
        }

        /// <summary>Clamp a client supplied loop count / array size. Negative -> 0.</summary>
        public static int ClampCount(long requested, int max)
        {
            if (requested <= 0) return 0;
            if (requested > max) return max;
            return (int)requested;
        }

        /// <summary>a * b without overflow; returns false on overflow or negative inputs.</summary>
        public static bool SafeMultiply(long a, long b, out long result)
        {
            result = 0;
            if (a < 0 || b < 0) return false;
            try { result = checked(a * b); return true; }
            catch (OverflowException) { return false; }
        }

        // ------------------------------------------------------------ audit log
        static readonly BlockingCollection<string> auditQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), 100000);
        static Thread auditThread;
        static readonly object auditStartLock = new object();
        public static string AuditFolder = "Security_Logs";

        /// <summary>Asynchronous audit log. Never pass passwords or secrets in details.</summary>
        public static void Audit(string category, uint uid, string name, string details)
        {
            try
            {
                StartAuditThread();
                var line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "\t" + category + "\t" + uid + "\t" +
                           Clean(name) + "\t" + Clean(details);
                auditQueue.TryAdd(line);
            }
            catch { }
        }

        static string Clean(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length);
            foreach (var c in s) sb.Append(c < 0x20 ? ' ' : c);
            return sb.Length > 512 ? sb.ToString(0, 512) : sb.ToString();
        }

        static void StartAuditThread()
        {
            if (auditThread != null) return;
            lock (auditStartLock)
            {
                if (auditThread != null) return;
                var t = new Thread(() =>
                {
                    foreach (var line in auditQueue.GetConsumingEnumerable())
                    {
                        try
                        {
                            Directory.CreateDirectory(AuditFolder);
                            File.AppendAllText(Path.Combine(AuditFolder, DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".log"),
                                line + "\r\n", new UTF8Encoding(false));
                        }
                        catch { }
                    }
                }) { IsBackground = true, Name = "SecurityAudit" };
                t.Start();
                auditThread = t;
            }
        }

        // ------------------------------------------------------------ throttling
        static readonly ConcurrentDictionary<string, long> throttle = new ConcurrentDictionary<string, long>();

        // Idle entries (throttle keys, violation windows) are purged every PurgeEveryMs by a timer, so the
        // dictionaries don't grow forever with per-player keys. An entry is idle after PurgeIdleMs, or after
        // the longest interval/window ever used if that is longer (so purging never shortens a throttle).
        public const int PurgeIdleMs = 10 * 60 * 1000;
        public const int PurgeEveryMs = 60 * 1000;
        static int maxIntervalMs;
        static readonly Timer purgeTimer = new Timer(delegate { try { PurgeIdle(); } catch { } }, null, PurgeEveryMs, PurgeEveryMs);

        static void NoteInterval(int ms)
        {
            if (ms > Volatile.Read(ref maxIntervalMs))
            {
                int cur;
                while (ms > (cur = Volatile.Read(ref maxIntervalMs)))
                    if (Interlocked.CompareExchange(ref maxIntervalMs, ms, cur) == cur) break;
            }
        }

        /// <summary>Removes throttle keys and violation windows idle for more than max(PurgeIdleMs, longest
        /// interval used). Runs from a timer every minute; returns the number of removed entries.</summary>
        public static int PurgeIdle()
        {
            return PurgeIdle(Environment.TickCount & int.MaxValue, Math.Max(PurgeIdleMs, Volatile.Read(ref maxIntervalMs)));
        }

        internal static int PurgeIdle(long now, long maxIdleMs)
        {
            int removed = 0;
            foreach (var kv in throttle)
            {
                if (Math.Abs(now - kv.Value) > maxIdleMs)
                {
                    if (((ICollection<KeyValuePair<string, long>>)throttle).Remove(kv)) removed++;// only if not refreshed meanwhile
                }
            }
            foreach (var kv in violations)
            {
                if (Math.Abs(now - kv.Value.Last) > maxIdleMs)
                {
                    ViolationWindow old;
                    if (violations.TryRemove(kv.Key, out old))
                    {
                        Interlocked.Decrement(ref violationCount);
                        removed++;
                    }
                }
            }
            return removed;
        }

        /// <summary>Approximate number of entries (diagnostics/tests).</summary>
        public static int ThrottleEntries { get { return throttle.Count; } }
        public static int ViolationEntries { get { return Math.Max(0, Volatile.Read(ref violationCount)); } }

        /// <summary>Returns true at most once per intervalMs for a given key (e.g. "jump:"+uid).</summary>
        public static bool Throttle(string key, int intervalMs)
        {
            NoteInterval(intervalMs);
            long now = Environment.TickCount & int.MaxValue;
            long last;
            if (throttle.TryGetValue(key, out last) && Math.Abs(now - last) < intervalMs) return false;
            throttle[key] = now;
            return true;
        }

        static readonly object throttlePairLock = new object();

        /// <summary>Like Throttle, but for two keys at once (e.g. per-player and global): returns true and
        /// consumes both slots only when both are free; otherwise neither slot is consumed.</summary>
        public static bool ThrottleBoth(string key1, int intervalMs1, string key2, int intervalMs2)
        {
            NoteInterval(Math.Max(intervalMs1, intervalMs2));
            lock (throttlePairLock)
            {
                long now = Environment.TickCount & int.MaxValue;
                long last;
                if (throttle.TryGetValue(key1, out last) && Math.Abs(now - last) < intervalMs1) return false;
                if (throttle.TryGetValue(key2, out last) && Math.Abs(now - last) < intervalMs2) return false;
                throttle[key1] = now;
                throttle[key2] = now;
                return true;
            }
        }

        class ViolationWindow { public readonly Queue<long> Stamps = new Queue<long>(); public long Last; }
        static readonly ConcurrentDictionary<string, ViolationWindow> violations = new ConcurrentDictionary<string, ViolationWindow>();
        static int violationCount;      // approximate violations.Count (ConcurrentDictionary.Count takes every lock)
        static long lastViolationPurge;

        /// <summary>Records one violation for key and returns true when at least `limit` violations
        /// (including this one) happened within the last windowMs.</summary>
        public static bool RecordViolation(string key, int limit, int windowMs)
        {
            NoteInterval(windowMs);
            long now = Environment.TickCount & int.MaxValue;
            if (Volatile.Read(ref violationCount) > 5000)
            {
                // at most once per second (not on every call while the table stays big)
                long last = Interlocked.Read(ref lastViolationPurge);
                if (Math.Abs(now - last) >= 1000 && Interlocked.CompareExchange(ref lastViolationPurge, now, last) == last)
                {
                    foreach (var kv in violations)
                        if (Math.Abs(now - kv.Value.Last) > windowMs)
                        {
                            ViolationWindow old;
                            if (violations.TryRemove(kv.Key, out old))
                                Interlocked.Decrement(ref violationCount);
                        }
                }
            }
            ViolationWindow w;
            if (!violations.TryGetValue(key, out w))
            {
                var created = new ViolationWindow();
                w = violations.GetOrAdd(key, created);
                if (ReferenceEquals(w, created))
                    Interlocked.Increment(ref violationCount);
            }
            lock (w)
            {
                while (w.Stamps.Count > 0 && Math.Abs(now - w.Stamps.Peek()) > windowMs)
                    w.Stamps.Dequeue();
                w.Stamps.Enqueue(now);
                while (w.Stamps.Count > Math.Max(1, limit))
                    w.Stamps.Dequeue();
                w.Last = now;
                return w.Stamps.Count >= limit;
            }
        }

        // Other per-uid security state (movement step buckets, HazemShield log stamps...) registers here, so this
        // file stays dependency-free for the unit tests.
        static Action<uint>[] forgetHooks = new Action<uint>[0];
        static readonly object forgetHookLock = new object();

        /// <summary>Registers a callback run by ForgetPlayer (returns true so it can initialize a static field).</summary>
        public static bool RegisterForgetHook(Action<uint> hook)
        {
            if (hook == null) return false;
            lock (forgetHookLock)
            {
                var cur = forgetHooks;
                var next = new Action<uint>[cur.Length + 1];
                Array.Copy(cur, next, cur.Length);
                next[cur.Length] = hook;
                forgetHooks = next;
            }
            return true;
        }

        /// <summary>Drops per-player security state on logout.</summary>
        public static void ForgetPlayer(uint uid)
        {
            NpcDialogState.Clear(uid);
            foreach (var hook in Volatile.Read(ref forgetHooks))
            {
                try { hook(uid); }
                catch { }
            }
        }
    }

    /// <summary>Server-side record of which dialog options an NPC actually offered to a player.
    /// The client may only answer with an option from the last dialog of the same NPC.</summary>
    public static class NpcDialogState
    {
        class Entry
        {
            public uint NpcId;
            public readonly HashSet<byte> Options = new HashSet<byte>();
            public DateTime At;
        }

        static readonly ConcurrentDictionary<uint, Entry> states = new ConcurrentDictionary<uint, Entry>();

        /// <summary>Called when a new dialog is built for a player (Dialog constructor).</summary>
        public static void Begin(uint uid, uint npcId)
        {
            var e = new Entry { NpcId = npcId, At = DateTime.Now };
            states[uid] = e;
        }

        public static void RecordOption(uint uid, uint npcId, byte option)
        {
            var e = states.GetOrAdd(uid, u => new Entry { NpcId = npcId, At = DateTime.Now });
            lock (e)
            {
                if (e.NpcId != npcId)
                {
                    e.NpcId = npcId;
                    e.Options.Clear();
                }
                e.Options.Add(option);
                e.At = DateTime.Now;
            }
        }

        /// <summary>Option 0 opens a dialog and 255 closes it; both are always allowed.</summary>
        public static bool IsAllowed(uint uid, uint npcId, byte option)
        {
            if (option == 0 || option == 255) return true;
            Entry e;
            if (!states.TryGetValue(uid, out e)) return false;
            lock (e)
            {
                if (e.NpcId != npcId) return false;
                if ((DateTime.Now - e.At).TotalSeconds > Guard.NpcDialogTimeoutSec) return false;
                return e.Options.Contains(option);
            }
        }

        public static void Clear(uint uid)
        {
            Entry e;
            states.TryRemove(uid, out e);
        }
    }

    /// <summary>Thread-safe replacement for counters that hand out unique IDs.</summary>
    public class SafeCounter
    {
        long value;
        public SafeCounter(long start) { value = start; }
        public uint Next { get { return (uint)Interlocked.Increment(ref value); } }
        public uint Current { get { return (uint)Interlocked.Read(ref value); } }
        /// <summary>Raise the counter to at least v (never lowers it).</summary>
        public void EnsureAtLeast(long v)
        {
            long cur;
            while ((cur = Interlocked.Read(ref value)) < v)
                if (Interlocked.CompareExchange(ref value, v, cur) == cur) break;
        }
    }
}
