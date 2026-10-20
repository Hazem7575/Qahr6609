using System;
using System.Collections.Generic;

namespace Conquer6609.Game.MsgTournaments
{
    /// <summary>
    /// Kingdom War settings (shell.ini [KingdomWar]). Defaults follow the official 6609 client texts
    /// (Cn_Res.ini STR_LEAGUE_ORDER_101_DESC / STR_LEAGUE_ORDER_ROB_TIME / STR_LEAGUE_INFO_DESC) and the
    /// official Kingdom War guide (co.99.com, 2015):
    ///  - Loot Token: issued 00:00:00 - 19:59:59 on Monday or Friday by a Union Leader against the Kingdom.
    ///  - Plunder War: 21:00:00 - 21:29:59 the same day; heroes grab Gold Bricks from the Kingdom's Treasury in Twin City.
    ///  - "Collect the most Gold Bricks to upgrade at 22:00": the union with the most Gold Bricks becomes the Kingdom.
    ///  - A new Union starts with 500 Gold Bricks; Dragon Shield protects the Treasury for 3 minutes.
    /// </summary>
    public static class KingdomWarConfig
    {
        const string Section = "KingdomWar";

        public static bool Enabled { get { return Framework.Settings.Bool(Section, "Enabled", true); } }

        /// <summary>Days a Loot Token can be issued and the Plunder War runs (0 = Sunday ... 6 = Saturday).</summary>
        public static bool IsPlunderDay(DayOfWeek day) { return DayList("PlunderDays", "1,5").Contains((int)day); }
        /// <summary>Days the 22:00 Gold Brick settlement crowns the Kingdom.</summary>
        public static bool IsSettleDay(DayOfWeek day) { return DayList("SettleDays", "1,3,4,5").Contains((int)day); }

        /// <summary>Loot Tokens can be issued from 00:00 until this time (exclusive).</summary>
        public static TimeSpan LootTokenClose { get { return Time("LootTokenClose", new TimeSpan(20, 0, 0)); } }
        public static TimeSpan WarStart { get { return Time("WarStart", new TimeSpan(21, 0, 0)); } }
        public static int WarMinutes { get { return Clamp(Framework.Settings.Int(Section, "WarMinutes", 30), 1, 180); } }
        public static TimeSpan SettleTime { get { return Time("SettleTime", new TimeSpan(22, 0, 0)); } }

        public static uint StartingBricks { get { return (uint)Clamp(Framework.Settings.Int(Section, "StartingBricks", 500), 0, 1000000); } }
        /// <summary>Union Fund one Loot Token costs. A shortfall can be paid in CPs (1 CP per point).</summary>
        public static uint LootTokenFund { get { return (uint)Clamp(Framework.Settings.Int(Section, "LootTokenFund", 500), 0, 100000000); } }
        public static uint DragonShieldFund { get { return (uint)Clamp(Framework.Settings.Int(Section, "DragonShieldFund", 500), 0, 100000000); } }
        public static int DragonShieldSeconds { get { return Clamp(Framework.Settings.Int(Section, "DragonShieldSeconds", 180), 10, 3600); } }
        public static int DragonShieldsPerWar { get { return Clamp(Framework.Settings.Int(Section, "DragonShieldsPerWar", 1), 0, 100); } }

        /// <summary>Seconds a hero must stay next to the Treasury (alive) to grab a Gold Brick.</summary>
        public static int PlunderSeconds { get { return Clamp(Framework.Settings.Int(Section, "PlunderSeconds", 15), 1, 600); } }
        public static int PlunderRange { get { return Clamp(Framework.Settings.Int(Section, "PlunderRange", 6), 1, 18); } }
        public static int PlunderMinLevel { get { return Clamp(Framework.Settings.Int(Section, "PlunderMinLevel", 70), 1, 255); } }
        public static int PlunderMinReborn { get { return Clamp(Framework.Settings.Int(Section, "PlunderMinReborn", 1), 0, 2); } }

        public static uint TreasuryMap { get { return (uint)Clamp(Framework.Settings.Int(Section, "TreasuryMap", 1002), 1, 100000); } }
        public static ushort TreasuryX { get { return (ushort)Clamp(Framework.Settings.Int(Section, "TreasuryX", 300), 1, 2000); } }
        public static ushort TreasuryY { get { return (ushort)Clamp(Framework.Settings.Int(Section, "TreasuryY", 280), 1, 2000); } }

        static int Clamp(int v, int min, int max) { return v < min ? min : v > max ? max : v; }

        static TimeSpan Time(string key, TimeSpan def)
        {
            string v = Framework.Settings.String(Section, key, null);
            TimeSpan t;
            if (v != null && TimeSpan.TryParse(v, out t) && t >= TimeSpan.Zero && t < TimeSpan.FromDays(1))
                return t;
            return def;
        }

        static readonly Dictionary<string, HashSet<int>> dayCache = new Dictionary<string, HashSet<int>>();
        static HashSet<int> DayList(string key, string def)
        {
            lock (dayCache)
            {
                HashSet<int> set;
                if (dayCache.TryGetValue(key, out set))
                    return set;
                set = new HashSet<int>();
                foreach (var part in Framework.Settings.String(Section, key, def).Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int d;
                    if (int.TryParse(part, out d))
                    {
                        if (d == 7) d = 0; // allow 7 = Sunday like Activity.ini
                        if (d >= 0 && d <= 6) set.Add(d);
                    }
                }
                dayCache[key] = set;
                return set;
            }
        }
    }
}
