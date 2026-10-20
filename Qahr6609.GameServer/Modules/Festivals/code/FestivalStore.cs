// What the festivals remember, in ServerDataBase\Modules\Festivals\:
//   counters.txt  per player counters: festival|name|uid|period|value. The period makes a counter start again from 0:
//                 daily = yyyyMMdd, festival = "E:" + the festival's Edition, ever = "*". Written by OnSave (and on reload).
//   state.ini     GM overrides: <key>=on | off (no line = the dates decide)
// Cooldowns (seconds between two uses) are kept in memory only.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Modules.Festivals
{
    public static class FestivalStore
    {
        sealed class Counter { public string Period; public long Value; }

        static readonly Dictionary<string, Counter> counters = new Dictionary<string, Counter>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, DateTime> cooldowns = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, string> overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        static readonly object sync = new object();
        static bool dirty;
        static string folder;

        static string Period(FestivalDef f, string scope)
        {
            switch (scope)
            {
                case "daily": return DateTime.Now.ToString("yyyyMMdd");
                case "festival": return "E:" + f.Edition;
                default: return "*";
            }
        }

        static string K(FestivalDef f, string name, uint uid) { return f.Key + "|" + name + "|" + uid; }

        public static long Get(FestivalDef f, string name, uint uid, string scope)
        {
            lock (sync)
            {
                Counter c;
                return counters.TryGetValue(K(f, name, uid), out c) && c.Period == Period(f, scope) ? c.Value : 0;
            }
        }

        public static long Add(FestivalDef f, string name, uint uid, string scope, long delta)
        {
            lock (sync)
            {
                string k = K(f, name, uid), p = Period(f, scope);
                Counter c;
                if (!counters.TryGetValue(k, out c) || c.Period != p) counters[k] = c = new Counter { Period = p };
                c.Value += delta;
                dirty = true;
                return c.Value;
            }
        }

        public static void Set(FestivalDef f, string name, uint uid, string scope, long value)
        {
            lock (sync)
            {
                counters[K(f, name, uid)] = new Counter { Period = Period(f, scope), Value = value };
                dirty = true;
            }
        }

        /// <summary>A limit is used up (counter name = what it limits, e.g. "x:Mine" or "i:3008710").</summary>
        public static bool Reached(FestivalDef f, string name, uint uid, Limit l)
        {
            return !l.None && Get(f, name, uid, l.Scope) >= l.Max;
        }

        public static void Count(FestivalDef f, string name, uint uid, Limit l)
        {
            if (!l.None) Add(f, name, uid, l.Scope, 1);
        }

        /// <summary>Seconds still to wait (0 = go).</summary>
        public static int CooldownLeft(FestivalDef f, string name, uint uid)
        {
            lock (sync)
            {
                DateTime t;
                if (!cooldowns.TryGetValue(K(f, name, uid), out t)) return 0;
                double s = (t - DateTime.Now).TotalSeconds;
                return s > 0 ? (int)Math.Ceiling(s) : 0;
            }
        }

        public static void StartCooldown(FestivalDef f, string name, uint uid, int seconds)
        {
            if (seconds <= 0) return;
            lock (sync) cooldowns[K(f, name, uid)] = DateTime.Now.AddSeconds(seconds);
        }

        /// <summary>The counters of one player (for @festival status).</summary>
        public static List<string> Describe(FestivalDef f, uint uid)
        {
            lock (sync)
                return counters.Where(kv => kv.Key.StartsWith(f.Key + "|") && kv.Key.EndsWith("|" + uid))
                    .Select(kv => kv.Key.Split('|')[1] + " = " + kv.Value.Value + " (" + kv.Value.Period + ")").OrderBy(s => s).ToList();
        }

        // ------------------------------------------------------------------ overrides
        public static string Override(string key)
        {
            lock (sync) { string v; return overrides.TryGetValue(key, out v) ? v : null; }
        }

        public static void SetOverride(string key, string value)
        {
            lock (sync)
            {
                if (value == null) overrides.Remove(key); else overrides[key] = value;
                try
                {
                    var sb = new StringBuilder("; GM overrides (@festival on/off/auto): <festival>=on | off. No line = its dates decide.\r\n");
                    foreach (var kv in overrides) sb.Append(kv.Key).Append('=').Append(kv.Value).Append("\r\n");
                    File.WriteAllText(Path.Combine(folder, "state.ini"), sb.ToString());
                }
                catch (Exception e) { MyConsole.WriteLine("[Festivals] state.ini not saved: " + e.Message); }
            }
        }

        // ------------------------------------------------------------------ files
        public static void Load(string dataFolder)
        {
            lock (sync)
            {
                folder = dataFolder;
                counters.Clear();
                overrides.Clear();
                string file = Path.Combine(folder, "counters.txt");
                if (File.Exists(file))
                    foreach (string line in File.ReadAllLines(file))
                    {
                        if (line.StartsWith(";")) continue;
                        string[] p = line.Split('|');
                        long v;
                        if (p.Length == 5 && long.TryParse(p[4], out v))
                            counters[p[0] + "|" + p[1] + "|" + p[2]] = new Counter { Period = p[3], Value = v };
                    }
                string st = Path.Combine(folder, "state.ini");
                if (File.Exists(st))
                    foreach (string line in File.ReadAllLines(st))
                    {
                        int eq = line.IndexOf('=');
                        if (line.StartsWith(";") || eq <= 0) continue;
                        string v = line.Substring(eq + 1).Trim().ToLowerInvariant();
                        if (v == "on" || v == "off") overrides[line.Substring(0, eq).Trim()] = v;
                    }
                dirty = false;
            }
        }

        public static void Save()
        {
            string text;
            lock (sync)
            {
                if (!dirty || folder == null) return;
                string today = DateTime.Now.ToString("yyyyMMdd");
                var sb = new StringBuilder("; festival|counter|uid|period|value (written by the server; daily periods older than today are dropped)\r\n");
                foreach (var kv in counters)
                {
                    string p = kv.Value.Period;
                    if (p.Length == 8 && char.IsDigit(p[0]) && p != today) continue;
                    sb.Append(kv.Key).Append('|').Append(p).Append('|').Append(kv.Value.Value).Append("\r\n");
                }
                text = sb.ToString();
                dirty = false;
            }
            string file = Path.Combine(folder, "counters.txt"), tmp = file + ".tmp";
            File.WriteAllText(tmp, text);
            if (File.Exists(file)) File.Replace(tmp, file, null); else File.Move(tmp, file);
        }
    }
}
