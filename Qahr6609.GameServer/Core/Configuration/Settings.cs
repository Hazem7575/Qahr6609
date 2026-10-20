// Shared runtime settings reader for shell.ini (game server working directory).
// Usage:  int v = Conquer6609.Framework.Settings.Int("Performance", "MonsterAiMode", 0);
// Values are read once (lazily) and cached; call Settings.Reload() to re-read.
// Use your OWN section/keys; document every key you add in docs/PERFORMANCE.md or docs/MODULES.md.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Conquer6609.Framework
{
    public static class Settings
    {
        static Dictionary<string, Dictionary<string, string>> data;
        static readonly object sync = new object();
        public static string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "shell.ini");

        static Dictionary<string, Dictionary<string, string>> Data
        {
            get
            {
                var d = data;
                if (d != null) return d;
                lock (sync)
                {
                    if (data == null) data = Parse(FilePath);
                    return data;
                }
            }
        }

        public static void Reload()
        {
            lock (sync) data = Parse(FilePath);
        }

        static Dictionary<string, Dictionary<string, string>> Parse(string path)
        {
            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (!File.Exists(path)) return result;
                Dictionary<string, string> cur = null;
                foreach (var raw in File.ReadAllLines(path))
                {
                    var line = raw.Trim();
                    if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;
                    if (line[0] == '[')
                    {
                        int end = line.IndexOf(']');
                        string name = end > 1 ? line.Substring(1, end - 1).Trim() : "";
                        if (!result.TryGetValue(name, out cur))
                        {
                            cur = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            result[name] = cur;
                        }
                        continue;
                    }
                    if (cur == null) continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = line.Substring(0, eq).Trim();
                    string v = line.Substring(eq + 1).Trim();
                    int sc = v.IndexOf(';');
                    if (sc >= 0) v = v.Substring(0, sc).Trim();
                    if (!cur.ContainsKey(k)) cur[k] = v; // first wins (same as GetPrivateProfileString)
                }
            }
            catch { }
            return result;
        }

        public static string String(string section, string key, string def)
        {
            Dictionary<string, string> s;
            string v;
            if (Data.TryGetValue(section, out s) && s.TryGetValue(key, out v) && v.Length > 0) return v;
            return def;
        }

        public static long Long(string section, string key, long def)
        {
            long n;
            return long.TryParse(String(section, key, null), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) ? n : def;
        }

        public static int Int(string section, string key, int def)
        {
            long n = Long(section, key, def);
            return n > int.MaxValue ? int.MaxValue : n < int.MinValue ? int.MinValue : (int)n;
        }

        public static bool Bool(string section, string key, bool def)
        {
            string v = String(section, key, null);
            if (v == null) return def;
            return v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("on", StringComparison.OrdinalIgnoreCase);
        }
    }
}
