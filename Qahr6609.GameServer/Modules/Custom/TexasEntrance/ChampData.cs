// Texas Hold'em championships: definitions read from the official client tables (texas_match_type / _condition /
// _stage / _prize, '@@'-separated). Plain copies live in ServerDataBase\Modules\TexasEntrance\texas_match_*.txt; when a
// copy is missing it is decrypted from the client's ini\texas_match_*.dat ([Texas] ClientIni) and written there.
// Layouts: docs\TEXAS_CHAMPIONSHIP.md (from the client code, see the RE spec).
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer.Poker;

namespace Conquer6609.Modules.TexasEntrance
{
    public sealed class ChampDef
    {
        public uint Id;
        public string Name = "";
        /// <summary>1 = No Limit.</summary>
        public int LimitType = 1;
        /// <summary>Fee currency: 0 Silver, 1 CPs, 3 Joy Beans (not supported here: treated as Silver).</summary>
        public byte Currency;
        public long Fee;
        public int Group;
        /// <summary>1 = weekday bitmask (bit d = weekday d, 0 Sunday; 0 = every day), 2 = one date yyyymmdd.</summary>
        public int ScheduleType;
        public int ScheduleValue;
        public List<int> Times = new List<int>();
        public uint Ticket;
        public int MinPlayers = 2;
        public int MaxPlayers = 9999;
        public long StartChips = 100000;
        public uint RebuyTicket;
        public int RebuyCount;
        public long RebuyPrice;
        public long RebuyChips;
        public uint AddonTicket;
        public int AddonCount;
        public long AddonPrice;
        public long AddonChips;
        public List<TourLevel> Levels = new List<TourLevel>();
        /// <summary>Prize brackets: max participants -> rows in rank order.</summary>
        public SortedDictionary<int, List<TourPrizeRow>> Prizes = new SortedDictionary<int, List<TourPrizeRow>>();
        public bool HasCondition;

        public string CurrencyName { get { return Currency == 1 ? "CPs" : "Silver"; } }

        public List<TourPrizeRow> Bracket(int participants)
        {
            if (Prizes.Count == 0)
                return new List<TourPrizeRow>();
            return Prizes[PokerTournament.BracketKey(Prizes.Keys, participants)];
        }

        public bool RunsOn(DateTime day)
        {
            if (ScheduleType == 1)
                return ScheduleValue == 0 || (ScheduleValue & (1 << (int)day.DayOfWeek)) != 0;
            if (ScheduleType == 2)
                return ScheduleValue == day.Year * 10000 + day.Month * 100 + day.Day;
            return false;
        }
    }

    public static class ChampData
    {
        public static readonly string[] Tables = { "texas_match_type", "texas_match_condition", "texas_match_stage", "texas_match_prize" };

        public static Dictionary<uint, ChampDef> Load(string dataPath, string clientIni, Action<string> log)
        {
            var lines = new Dictionary<string, string[]>();
            foreach (var t in Tables)
            {
                string txt = Path.Combine(dataPath, t + ".txt");
                if (!File.Exists(txt))
                {
                    string dat = Path.Combine(clientIni ?? "", t + ".dat");
                    if (File.Exists(dat))
                    {
                        File.WriteAllBytes(txt, Decrypt(File.ReadAllBytes(dat)));
                        log("decrypted " + dat + " -> " + txt);
                    }
                }
                lines[t] = File.Exists(txt) ? File.ReadAllLines(txt, global::Conquer6609.AnsiText.Value) : new string[0];
                if (lines[t].Length == 0)
                    log("WARNING: " + txt + " missing or empty");
            }
            var defs = new Dictionary<uint, ChampDef>();
            foreach (var l in lines["texas_match_type"])
            {
                var c = Split(l);
                if (c.Length < 10 || c[1] != "1")
                    continue;// only championship rows (type 1)
                uint id;
                if (!uint.TryParse(c[0], out id))
                    continue;
                var d = new ChampDef { Id = id, Name = c[2] };
                d.LimitType = Int(c[3]);
                d.Currency = (byte)Int(c[4]);
                d.Fee = Long(c[7]);
                d.Group = Int(c[9]);
                defs[id] = d;
            }
            foreach (var l in lines["texas_match_condition"])
            {
                var c = Split(l);
                uint id;
                ChampDef d;
                if (c.Length < 21 || !uint.TryParse(c[0], out id) || !defs.TryGetValue(id, out d))
                    continue;
                d.HasCondition = true;
                d.ScheduleType = Int(c[1]);
                d.ScheduleValue = Int(c[2]);
                d.Times = c[3].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Int).Where(t => t >= 0 && t < 2400 && t % 100 < 60).OrderBy(t => t).ToList();
                d.Ticket = (uint)Long(c[5]);
                d.MinPlayers = Math.Max(2, Int(c[6]));
                d.MaxPlayers = Math.Max(d.MinPlayers, Int(c[7]));
                d.StartChips = Long(c[10]);
                d.RebuyTicket = (uint)Long(c[11]);
                d.RebuyCount = Int(c[12]);
                d.RebuyPrice = Long(c[13]);
                d.RebuyChips = Long(c[14]);
                d.AddonTicket = (uint)Long(c[16]);
                d.AddonCount = Int(c[17]);
                d.AddonPrice = Long(c[18]);
                d.AddonChips = Long(c[19]);
            }
            foreach (var l in lines["texas_match_stage"])
            {
                var c = Split(l);
                uint id;
                ChampDef d;
                if (c.Length < 2 || !uint.TryParse(c[0], out id) || !defs.TryGetValue(id, out d))
                    continue;
                d.Levels.Clear();
                for (int i = 1; i < c.Length; i++)
                {
                    TourLevel lv = null;
                    try { lv = PokerTournament.ParseLevel(c[i]); } catch { }
                    if (lv != null)
                        d.Levels.Add(lv);
                }
            }
            foreach (var l in lines["texas_match_prize"])
            {
                var c = Split(l);
                uint id;
                ChampDef d;
                if (c.Length < 5 || !uint.TryParse(c[0], out id) || !defs.TryGetValue(id, out d))
                    continue;
                int key = Int(c[1]);
                List<TourPrizeRow> rows;
                if (!d.Prizes.TryGetValue(key, out rows))
                    d.Prizes[key] = rows = new List<TourPrizeRow>();
                rows.Add(new TourPrizeRow { Money = Long(c[2]), Share = Int(c[3]), Flag = Int(c[4]), Text = c.Length > 5 ? c[5] : "" });
            }
            return defs;
        }

        static string[] Split(string line)
        {
            var p = line.Split(new[] { "@@" }, StringSplitOptions.None);
            // rows end with "@@": drop the empty tail
            if (p.Length > 0 && p[p.Length - 1].Trim().Length == 0)
                Array.Resize(ref p, p.Length - 1);
            for (int i = 0; i < p.Length; i++)
                p[i] = p[i].Trim();
            return p;
        }

        static int Int(string s)
        {
            long v;
            return long.TryParse(s, out v) ? (int)Math.Max(int.MinValue, Math.Min(int.MaxValue, v)) : 0;
        }

        static long Long(string s)
        {
            long v;
            return long.TryParse(s, out v) ? v : 0;
        }

        /// <summary>Client ini\*.dat cipher (same LCG key stream for every file).</summary>
        public static byte[] Decrypt(byte[] b)
        {
            var key = new byte[128];
            uint s = 0x2537;
            for (int i = 0; i < 128; i++)
            {
                s = unchecked(s * 214013 + 2531011);
                key[i] = (byte)(((s >> 16) & 0x7fff) % 256);
            }
            var o = new byte[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                int n = b[i] ^ key[i & 127];
                int r = i & 7;
                o[i] = (byte)(((n << (8 - r)) | (n >> r)) & 0xff);
            }
            return o;
        }

        // ------------------------------------------------------------------ reward text -> items
        /// <summary>
        /// "[item 40%Bonus,15PokerTokens]" -> (3304719, 15); "[money 1000]" / "[emoney 50]" -> silver / CPs.
        /// Names are mapped with prize_items.txt (name=itemId); "x%Bonus" labels are display only.
        /// </summary>
        public static void ParseReward(string text, Dictionary<string, uint> names, List<KeyValuePair<uint, int>> items, out long silver, out long cps, List<string> unknown)
        {
            silver = 0;
            cps = 0;
            if (string.IsNullOrEmpty(text))
                return;
            int pos = 0;
            while (true)
            {
                int a = text.IndexOf('[', pos);
                if (a < 0)
                    break;
                int b = text.IndexOf(']', a);
                if (b < 0)
                    break;
                pos = b + 1;
                string inner = text.Substring(a + 1, b - a - 1).Trim();
                int sp = inner.IndexOf(' ');
                string kind = sp < 0 ? inner : inner.Substring(0, sp);
                string rest = sp < 0 ? "" : inner.Substring(sp + 1);
                foreach (var raw in rest.Split(','))
                {
                    string part = raw.Trim();
                    if (part.Length == 0 || part.IndexOf('%') >= 0)
                        continue;
                    int i = 0;
                    while (i < part.Length && char.IsDigit(part[i]))
                        i++;
                    long count = i > 0 ? long.Parse(part.Substring(0, i)) : 1;
                    string name = part.Substring(i).Trim();
                    switch (kind.ToLowerInvariant())
                    {
                        case "money":
                            if (name.Length == 0) silver += count;
                            break;
                        case "emoney":
                            if (name.Length == 0) cps += count;
                            break;
                        case "item":
                            {
                                uint id;
                                if (name.Length > 0 && names.TryGetValue(name, out id))
                                    items.Add(new KeyValuePair<uint, int>(id, (int)Math.Min(count, 1000)));
                                else if (name.Length > 0 && unknown != null)
                                    unknown.Add(name);
                                break;
                            }
                    }
                }
            }
        }

        public static Dictionary<string, uint> LoadItemNames(string file)
        {
            var map = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(file))
            {
                File.WriteAllLines(file, new[]
                {
                    "; reward names of texas_match_prize.dat -> item id (count = the number in front of the name)",
                    "; e.g. [item 40%Bonus,15PokerTokens] gives 15 x PokerTokens; x%Bonus is only a label",
                    "PokerTokens=3304719",
                    "PokerToken=3304719",
                    "PokerTokenFragmen=3305484",
                    "PokerTokenFragment=3305484",
                });
            }
            foreach (var raw in File.ReadAllLines(file))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line[0] == ';' || line[0] == '#')
                    continue;
                int eq = line.IndexOf('=');
                uint id;
                if (eq > 0 && uint.TryParse(line.Substring(eq + 1).Trim(), out id))
                    map[line.Substring(0, eq).Trim()] = id;
            }
            return map;
        }
    }
}
