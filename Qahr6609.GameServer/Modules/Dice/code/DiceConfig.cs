using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conquer6609.Modules.Dice
{
    /// <summary>dice.ini (ServerDataBase\Modules\Dice\dice.ini; the default is the module's data\dice.ini).</summary>
    public sealed class DiceConfig
    {
        public bool Enabled = true;
        public int BetSeconds = 40, CutoffSeconds = 1, ResultSeconds = 12;
        public byte MinLevel = 1;
        public long[] Chips = { 100000, 1000000, 3000000, 10000000 };
        public long MaxBetPerCell = 50000000, MaxBetPerRound = 200000000;
        public int MaxBetsPerRound = 80, MinBetIntervalMs = 120;
        public bool TripleLosesSmallBig = true;
        public int SmallBig = 2, Triple = 150, Sum4And17 = 50, Sum5And16 = 25, Sum6And15 = 15, Sum7And14 = 12, Sum8And13 = 8, Sum9To12 = 6;
        public int HistoryCount = 10;
        public long AnnounceWinAbove = 500000000;
        public bool DevCommands;

        public static DiceConfig Load(string file, Action<string> warn)
        {
            var ini = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            string section = "";
            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;
                if (line[0] == '[')
                {
                    section = line.Trim('[', ']').Trim();
                    if (!ini.ContainsKey(section)) ini[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
            Func<string, string, long, long, long, long> N = (sec, k, def, min, max) =>
            {
                long v;
                if (!long.TryParse(S(sec, k), out v)) return def;
                if (v < min || v > max) { warn(sec + "." + k + "=" + v + " out of range " + min + ".." + max); v = Math.Max(min, Math.Min(max, v)); }
                return v;
            };

            var c = new DiceConfig();
            c.Enabled = N("Dice", "Enabled", 1, 0, 1) != 0;
            c.BetSeconds = (int)N("Dice", "BetSeconds", c.BetSeconds, 10, 50);
            c.CutoffSeconds = (int)N("Dice", "CutoffSeconds", c.CutoffSeconds, 0, 5);
            c.ResultSeconds = (int)N("Dice", "ResultSeconds", c.ResultSeconds, 9, 120);
            c.MinLevel = (byte)N("Dice", "MinLevel", c.MinLevel, 1, 255);

            string chips = S("Bets", "Chips");
            if (chips != null)
            {
                var list = chips.Split(',').Select(p => { long v; return long.TryParse(p.Trim(), out v) ? v : 0; }).Where(v => v > 0 && v <= 1000000000).Distinct().ToArray();
                if (list.Length > 0) c.Chips = list; else warn("Bets.Chips has no valid amount, using the defaults");
            }
            c.MaxBetPerCell = N("Bets", "MaxBetPerCell", c.MaxBetPerCell, 1, 60000000);// x150 must stay far below the 10,000,000,000 money guard
            c.MaxBetPerRound = N("Bets", "MaxBetPerRound", c.MaxBetPerRound, 1, 2000000000);
            c.MaxBetsPerRound = (int)N("Bets", "MaxBetsPerRound", c.MaxBetsPerRound, 1, 1000);
            c.MinBetIntervalMs = (int)N("Bets", "MinBetIntervalMs", c.MinBetIntervalMs, 0, 5000);
            c.TripleLosesSmallBig = N("Bets", "TripleLosesSmallBig", 1, 0, 1) != 0;

            c.SmallBig = (int)N("Payout", "SmallBig", c.SmallBig, 1, 1000);
            c.Triple = (int)N("Payout", "Triple", c.Triple, 1, 1000);
            c.Sum4And17 = (int)N("Payout", "Sum4And17", c.Sum4And17, 1, 1000);
            c.Sum5And16 = (int)N("Payout", "Sum5And16", c.Sum5And16, 1, 1000);
            c.Sum6And15 = (int)N("Payout", "Sum6And15", c.Sum6And15, 1, 1000);
            c.Sum7And14 = (int)N("Payout", "Sum7And14", c.Sum7And14, 1, 1000);
            c.Sum8And13 = (int)N("Payout", "Sum8And13", c.Sum8And13, 1, 1000);
            c.Sum9To12 = (int)N("Payout", "Sum9To12", c.Sum9To12, 1, 1000);

            c.HistoryCount = (int)N("Misc", "HistoryCount", c.HistoryCount, 1, 50);
            c.AnnounceWinAbove = N("Misc", "AnnounceWinAbove", c.AnnounceWinAbove, 0, long.MaxValue);
            c.DevCommands = N("Misc", "DevCommands", 0, 0, 1) != 0;
            if (c.MaxBetPerRound < c.MaxBetPerCell) warn("Bets.MaxBetPerRound is below MaxBetPerCell");
            return c;
        }

        /// <summary>Multiplier printed on a cell of the table (cell ids as the client sends them).</summary>
        public int Multiplier(int cell)
        {
            if (cell <= 1) return SmallBig;
            if (cell <= 7) return Triple;
            int sum = cell - 4;
            switch (sum)
            {
                case 4: case 17: return Sum4And17;
                case 5: case 16: return Sum5And16;
                case 6: case 15: return Sum6And15;
                case 7: case 14: return Sum7And14;
                case 8: case 13: return Sum8And13;
                default: return Sum9To12;
            }
        }
    }
}
