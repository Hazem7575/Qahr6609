using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Conquer6609.Modules.DragonIsland
{
    /// <summary>One beast of the island (island.ini [BeastN]).</summary>
    public sealed class Beast
    {
        public string Key, Name;
        public uint Id;
        public ushort X, Y, Range;
        public int[] Minutes = new int[0];
        public bool Official, Enabled;

        /// <summary>The next time it comes, from now (MinValue = never).</summary>
        public DateTime Next(DateTime now)
        {
            if (!Enabled || Minutes.Length == 0) return DateTime.MinValue;
            DateTime hour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            for (int h = 0; h < 2; h++)
                foreach (int m in Minutes.OrderBy(v => v))
                {
                    DateTime t = hour.AddHours(h).AddMinutes(m);
                    if (t > now) return t;
                }
            return DateTime.MinValue;
        }
    }

    /// <summary>island.ini (ServerDataBase\Modules\DragonIsland\island.ini; the default is the module's data\island.ini).</summary>
    public sealed class IslandConfig
    {
        public uint Map = 10137;
        public byte MinLevel = 120, MinReborn = 1;
        public ushort EntryX = 95, EntryY = 412;
        public uint IronTongMap = 1002;
        public ushort IronTongX = 547, IronTongY = 370, IronTongLook = 5022;
        public bool MessageBox = true;
        public int MessageBoxSeconds = 60;
        public string Sender = "Commander Shan";
        public int ParticipationMinPercent = 1;
        public uint ParticipationItem = 3004181, ParticipationCount = 1, ParticipationCPs;
        public uint[] ExploitCPs = { 3000, 1500, 1000 };
        public uint ExploitItem = 723694;
        public string KillMessage = "{0} defeated the {1} and found its grand treasure!";
        public List<Beast> Beasts = new List<Beast>();

        public static IslandConfig Load(string file, Action<string> warn)
        {
            var ini = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var order = new List<string>();
            string section = "";
            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;
                if (line[0] == '[')
                {
                    section = line.Trim('[', ']').Trim();
                    if (!ini.ContainsKey(section)) { ini[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); order.Add(section); }
                    continue;
                }
                int eq = line.IndexOf('=');
                if (eq > 0 && ini.ContainsKey(section)) ini[section][line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
            Func<string, string, string> S = (sec, k) =>
            {
                Dictionary<string, string> d; string v;
                return ini.TryGetValue(sec, out d) && d.TryGetValue(k, out v) ? v : null;
            };
            Func<string, string, long, long> N = (sec, k, def) => { long v; return long.TryParse(S(sec, k), out v) ? v : def; };

            var c = new IslandConfig();
            c.Map = (uint)N("Island", "Map", c.Map);
            c.MinLevel = (byte)Math.Min(255, N("Island", "MinLevel", c.MinLevel));
            c.MinReborn = (byte)Math.Min(2, N("Island", "MinReborn", c.MinReborn));
            c.EntryX = (ushort)N("Island", "EntryX", c.EntryX);
            c.EntryY = (ushort)N("Island", "EntryY", c.EntryY);
            c.IronTongMap = (uint)N("Island", "IronTongMap", c.IronTongMap);
            c.IronTongX = (ushort)N("Island", "IronTongX", c.IronTongX);
            c.IronTongY = (ushort)N("Island", "IronTongY", c.IronTongY);
            c.IronTongLook = (ushort)N("Island", "IronTongLook", c.IronTongLook);
            c.MessageBox = N("Announce", "MessageBox", 1) != 0;
            c.MessageBoxSeconds = (int)Math.Max(5, Math.Min(300, N("Announce", "MessageBoxSeconds", 60)));
            c.Sender = (S("Rewards", "Sender") ?? c.Sender);
            if (c.Sender.Length > 31) c.Sender = c.Sender.Substring(0, 31);
            c.ParticipationMinPercent = (int)Math.Max(0, Math.Min(100, N("Rewards", "ParticipationMinPercent", 1)));
            c.ParticipationItem = (uint)N("Rewards", "ParticipationItem", c.ParticipationItem);
            c.ParticipationCount = (uint)Math.Max(1, Math.Min(10, N("Rewards", "ParticipationCount", 1)));
            c.ParticipationCPs = (uint)Math.Max(0, Math.Min(1000000, N("Rewards", "ParticipationCPs", 0)));
            for (int i = 0; i < 3; i++)
                c.ExploitCPs[i] = (uint)Math.Max(0, Math.Min(1000000, N("Rewards", "Exploit" + (i + 1) + "CPs", c.ExploitCPs[i])));
            c.ExploitItem = (uint)N("Rewards", "ExploitItem", c.ExploitItem);
            c.KillMessage = S("Rewards", "KillMessage") ?? c.KillMessage;

            foreach (string sec in order.Where(s => s.StartsWith("Beast", StringComparison.OrdinalIgnoreCase)))
            {
                var b = new Beast
                {
                    Key = sec,
                    Name = S(sec, "Name") ?? sec,
                    Id = (uint)N(sec, "Id", 0),
                    X = (ushort)N(sec, "X", 0),
                    Y = (ushort)N(sec, "Y", 0),
                    Range = (ushort)Math.Max(1, Math.Min(30, N(sec, "Range", 3))),
                    Official = N(sec, "Official", 0) != 0,
                    Enabled = N(sec, "Enabled", 1) != 0,
                };
                b.Minutes = (S(sec, "Minutes") ?? "").Split(',').Select(p => { int m; return int.TryParse(p.Trim(), out m) ? m : -1; })
                    .Where(m => m >= 0 && m < 60).Distinct().ToArray();
                if (b.Id == 0 || b.X == 0 || b.Y == 0 || b.Minutes.Length == 0)
                {
                    warn(sec + " skipped: Id, X, Y and Minutes are needed");
                    continue;
                }
                if (!Database.Server.MonsterFamilies.ContainsKey(b.Id))
                {
                    warn(sec + " (" + b.Name + ") skipped: monster type " + b.Id + " does not exist");
                    continue;
                }
                c.Beasts.Add(b);
            }
            return c;
        }
    }
}
