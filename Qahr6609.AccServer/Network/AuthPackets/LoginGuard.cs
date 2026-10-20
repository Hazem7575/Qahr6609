using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AccServer.Network.AuthPackets
{
    /// <summary>Failed-login limiter for the account server.
    /// Failures are counted per source IP and per (username + source IP) inside a sliding window;
    /// reaching the limit locks that IP, or that username for that IP only, for LockMinutes.
    /// The username counter is deliberately bound to the source IP: a global per-username lock would let
    /// anyone lock any account (including the owner's) just by typing wrong passwords for it.</summary>
    public static class LoginGuard
    {
        public static int MaxFailuresPerIp = 5;
        /// <summary>Failures allowed for one username from one IP (shell/config key MaxFailuresPerUser).</summary>
        public static int MaxFailuresPerUser = 3;
        public static int WindowMinutes = 15;
        public static int LockMinutes = 5;
        /// <summary>Maximum authentication packets accepted on a single connection.</summary>
        public static int MaxAttemptsPerConnection = 3;

        class Entry
        {
            public int Failures;
            public DateTime WindowStart;
            public DateTime LockedUntil;
        }

        static readonly ConcurrentDictionary<string, Entry> entries = new ConcurrentDictionary<string, Entry>(StringComparer.Ordinal);
        static Timer cleanup;

        public static void Load(Database.IniFile file)
        {
            try
            {
                MaxFailuresPerIp = Math.Max(1, file.ReadInt32("Security", "MaxFailuresPerIp", MaxFailuresPerIp));
                MaxFailuresPerUser = Math.Max(1, file.ReadInt32("Security", "MaxFailuresPerUser", MaxFailuresPerUser));
                WindowMinutes = Math.Max(1, file.ReadInt32("Security", "FailureWindowMinutes", WindowMinutes));
                LockMinutes = Math.Max(1, file.ReadInt32("Security", "LockoutMinutes", LockMinutes));
                MaxAttemptsPerConnection = Math.Max(1, file.ReadInt32("Security", "MaxAttemptsPerConnection", MaxAttemptsPerConnection));
            }
            catch (Exception e)
            {
                Console.WriteLine("[LoginGuard] config: " + e.Message);
            }
            if (cleanup == null)
                cleanup = new Timer(Cleanup, null, 60000, 60000);
        }

        static string IpKey(string ip) { return "ip:" + (ip ?? ""); }
        static string UserKey(string user, string ip) { return "user:" + (user ?? "").Trim().ToLowerInvariant() + "|" + (ip ?? ""); }

        static bool Locked(string key, DateTime now)
        {
            Entry e;
            if (!entries.TryGetValue(key, out e)) return false;
            lock (e) return e.LockedUntil > now;
        }

        /// <summary>True when either the IP or the username (for this IP) is currently locked out.</summary>
        public static bool IsLocked(string ip, string username)
        {
            return LockReason(ip, username) != null;
        }

        /// <summary>null when not locked, otherwise "ip-lockout" or "user-ip-lockout" (for the audit log).</summary>
        public static string LockReason(string ip, string username)
        {
            DateTime now = DateTime.UtcNow;
            if (Locked(IpKey(ip), now)) return "ip-lockout";
            if (!string.IsNullOrEmpty(username) && Locked(UserKey(username, ip), now)) return "user-ip-lockout";
            return null;
        }

        static void Fail(string key, int max, DateTime now)
        {
            var e = entries.GetOrAdd(key, k => new Entry { WindowStart = now });
            lock (e)
            {
                if (now - e.WindowStart > TimeSpan.FromMinutes(WindowMinutes))
                {
                    e.WindowStart = now;
                    e.Failures = 0;
                }
                e.Failures++;
                if (e.Failures >= max)
                {
                    e.LockedUntil = now.AddMinutes(LockMinutes);
                    e.Failures = 0;
                    e.WindowStart = now;
                }
            }
        }

        public static void RegisterFailure(string ip, string username)
        {
            DateTime now = DateTime.UtcNow;
            Fail(IpKey(ip), MaxFailuresPerIp, now);
            if (!string.IsNullOrEmpty(username))
                Fail(UserKey(username, ip), MaxFailuresPerUser, now);
        }

        /// <summary>A successful login clears the username counter for this IP (the IP counter only decays).</summary>
        public static void RegisterSuccess(string ip, string username)
        {
            Entry e;
            if (!string.IsNullOrEmpty(username))
                entries.TryRemove(UserKey(username, ip), out e);
        }

        // ------------------------------------------------------------ audit log (same format as the game server)
        static readonly object auditLock = new object();

        /// <summary>Appends one line to Security_Logs\yyyyMMdd.log. Never pass passwords.</summary>
        public static void Audit(string category, string username, string details)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "\t" +
                              category + "\t0\t" + Clean(username) + "\t" + Clean(details) + "\r\n";
                lock (auditLock)
                {
                    System.IO.Directory.CreateDirectory("Security_Logs");
                    System.IO.File.AppendAllText(System.IO.Path.Combine("Security_Logs",
                        DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) + ".log"), line);
                }
            }
            catch { }
        }

        static string Clean(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                if (chars[i] < 0x20) chars[i] = ' ';
            return chars.Length > 256 ? new string(chars, 0, 256) : new string(chars);
        }

        static void Cleanup(object state)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                foreach (var kv in entries)
                {
                    bool stale;
                    lock (kv.Value)
                        stale = kv.Value.LockedUntil <= now && now - kv.Value.WindowStart > TimeSpan.FromMinutes(WindowMinutes);
                    if (stale)
                    {
                        Entry removed;
                        entries.TryRemove(kv.Key, out removed);
                    }
                }
            }
            catch { }
        }
    }
}
