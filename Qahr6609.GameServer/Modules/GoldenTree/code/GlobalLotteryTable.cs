using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    /// <summary>
    /// Golden Tree (Global Lottery, packets 3282/3283).
    /// globallotterycondition.ini : one section per condition ("1".."Amount"): id, Type, CostType (1 CP, 2 Silver,
    ///                              5 one item of type CostValue), CostValue (per pick), Monopoly (how CPs are paid: 0 CP(B)
    ///                              first, 1 CP(B) only, 2 CPs only), CD (picking cooldown, seconds), CDCost (CPs to skip a whole
    ///                              cooldown), StartTime, Desc. Optional [ConditionAmount] Active=id.
    ///                              The client reads the same keys from its ini/globallotterycondition.ini (sections [0]..):
    ///                              WriteClientIni writes that file from this one.
    /// global_lottery_pool.txt    : the client's ini/global_lottery_pool.dat decrypted, one prize per line, "@@" separated:
    ///                              ID@@Type@@Rank@@Name@@ItemID@@Monopoly@@Timed@@Minutes@@?@@Announce@@Amount@@ItemCount@@
    ///                              Amount is how many of that prize the tree holds; every pick takes one prize out of the pool.
    /// The files are in the module folder (ServerDataBase\Modules\GoldenTree\). The event is started in game:
    /// Missing files disable the Golden Tree (logged) instead of stopping the server.
    /// </summary>
    public class GlobalLotteryTable
    {
        public static bool Loaded = false;
        public static uint Count = 0;
        public static DateTime NextRewardUpdateSpan;
        public static uint TickCount
        {
            get
            {
                TimeSpan Date = DateTime.Now - NextRewardUpdateSpan;
                return (uint)Math.Max(0, Date.TotalSeconds);
            }
        }
        public static GlobalLotteryCondition[] Conditions = new GlobalLotteryCondition[0];
        public static GlobalLotteryCondition TodayCondition = null;
        public static uint TodayPrizeID = 0;
        private static uint ForcedConditionID = 0;
        private static readonly object Sync = new object();
        private static readonly Random Rand = new Random();

        public static bool TryGetCondition(byte Type, out GlobalLotteryCondition Condition)
        {
            Condition = null;
            foreach (var condition in Conditions)
                if (condition.Type == Type)
                {
                    Condition = condition;
                    return true;
                }
            return false;
        }
        private static void SetTodayCondition()
        {
            var usable = Conditions.Where(p => p.Prizes.Count > 0).ToArray();
            if (usable.Length == 0)
            {
                TodayCondition = null;
                return;
            }
            if (ForcedConditionID != 0)
            {
                var forced = usable.FirstOrDefault(p => p.ID == ForcedConditionID);
                if (forced != null)
                {
                    TodayCondition = forced;
                    return;
                }
            }
            lock (Rand)
                TodayCondition = usable[Rand.Next(0, usable.Length)];
        }
        /// <summary>Kept for old callers: a random prize id of the active condition.</summary>
        public static uint GetRandomPrizeToday()
        {
            if (TodayCondition == null)
                SetTodayCondition();
            if (TodayCondition == null || TodayCondition.Prizes.Count == 0)
                return 0;
            lock (Rand)
                TodayPrizeID = TodayCondition.Prizes[Rand.Next(0, TodayCondition.Prizes.Count)].ID;
            return TodayPrizeID;
        }
        public class GlobalLotteryCondition
        {
            public uint ID;
            public uint Type;
            public byte CostType;
            public uint CostValue;
            public byte Monopoly;
            public uint CD;
            public uint CDCost;
            public DateTime StartTime;
            public string Desc;
            public List<GlobalLotteryConditionPrizes> Prizes = new List<GlobalLotteryConditionPrizes>();
            public GlobalLotteryConditionPrizes PrizeItemID(uint UID)
            {
                foreach (var prize in Prizes)
                {
                    if (prize.ID == UID)
                        return prize;
                }
                return null;
            }
        }
        public class GlobalLotteryConditionPrizes
        {
            public uint ID;
            /// <summary>Condition (pool) type the prize belongs to.</summary>
            public uint Type;
            /// <summary>Prize grade, 1 = best.</summary>
            public byte PerfectionLevel;
            public string PrizeName;
            public uint PrizeItemID;
            /// <summary>Monopoly of the handed out item (non-zero = bound).</summary>
            public byte dwparam;
            /// <summary>1 = the item is time limited to dwparam3 minutes.</summary>
            public byte dwparam2;
            public uint dwparam3;
            public byte dwparam4;
            /// <summary>1 = rare prize, announced to the whole server.</summary>
            public byte dwparam5;
            /// <summary>How many of this prize the tree holds.</summary>
            public ushort Count;
            /// <summary>Item count handed out per pick.</summary>
            public uint dwparam7;

            public static GlobalLotteryConditionPrizes TryParse(string Line)
            {
                if (string.IsNullOrWhiteSpace(Line))
                    return null;
                string[] data = Line.Trim().Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length < 12)
                    return null;
                var Prize = new GlobalLotteryConditionPrizes();
                if (!uint.TryParse(data[0], out Prize.ID)
                    || !uint.TryParse(data[1], out Prize.Type)
                    || !byte.TryParse(data[2], out Prize.PerfectionLevel)
                    || !uint.TryParse(data[4], out Prize.PrizeItemID)
                    || !byte.TryParse(data[5], out Prize.dwparam)
                    || !byte.TryParse(data[6], out Prize.dwparam2)
                    || !uint.TryParse(data[7], out Prize.dwparam3)
                    || !byte.TryParse(data[8], out Prize.dwparam4)
                    || !byte.TryParse(data[9], out Prize.dwparam5)
                    || !ushort.TryParse(data[10], out Prize.Count)
                    || !uint.TryParse(data[11], out Prize.dwparam7))
                    return null;
                Prize.PrizeName = data[3].Replace('~', ' ');
                if (Prize.dwparam7 == 0)
                    Prize.dwparam7 = 1;
                return Prize;
            }
            public static implicit operator GlobalLotteryConditionPrizes(string Line)
            {
                return TryParse(Line);
            }
        }

        public static GlobalLotteryConditionPrizes FindPrize(uint ID)
        {
            foreach (var condition in Conditions)
            {
                var prize = condition.PrizeItemID(ID);
                if (prize != null)
                    return prize;
            }
            return null;
        }

        private static int AddPrize(GlobalLotteryConditionPrizes Prize)
        {
            int added = 0;
            foreach (var condition in Conditions)
                if (condition.Type == Prize.Type)
                {
                    condition.Prizes.Add(Prize);
                    added++;
                }
            return added;
        }
        public static void LoadDBConditions(string folder)
        {
            Loaded = false;
            Conditions = new GlobalLotteryCondition[0];
            TodayCondition = null;
            string condPath = Path.Combine(folder, "globallotterycondition.ini");
            string poolPath = Path.Combine(folder, "global_lottery_pool.txt");
            if (!File.Exists(condPath) || !File.Exists(poolPath))
            {
                MyConsole.WriteLine("[GoldenTree] globallotterycondition.ini / global_lottery_pool.txt not found. Golden Tree disabled.");
                return;
            }
            try
            {
                WindowsAPI.IniFile Reader = new WindowsAPI.IniFile { FileName = condPath };
                Count = Math.Min(Reader.ReadUInt32("ConditionAmount", "Amount", 0), 100u);
                ForcedConditionID = Reader.ReadUInt32("ConditionAmount", "Active", 0);
                var list = new List<GlobalLotteryCondition>();
                for (int x = 1; x <= Count; x++)
                {
                    string section = x.ToString();
                    GlobalLotteryCondition Condition = new GlobalLotteryCondition()
                    {
                        ID = Reader.ReadUInt32(section, "id", 0),
                        Type = Reader.ReadUInt32(section, "Type", 0),
                        CostType = Reader.ReadByte(section, "CostType", 0),
                        CostValue = Reader.ReadUInt32(section, "CostValue", 0),
                        Monopoly = Reader.ReadByte(section, "Monopoly", 0),
                        CD = Reader.ReadUInt32(section, "CD", 0),
                        CDCost = Reader.ReadUInt32(section, "CDCost", 0),
                        Desc = Reader.ReadString(section, "Desc", ""),
                    };
                    if (Condition.ID == 0 || Condition.Type == 0)
                    {
                        MyConsole.WriteLine("[GoldenTree] condition section [" + section + "] skipped: id/Type missing.");
                        continue;
                    }
                    long start = Reader.ReadInt64(section, "StartTime", 0);
                    DateTime when;
                    if (start > 0 && DateTime.TryParseExact(start.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out when))
                        Condition.StartTime = when;
                    if ((Condition.CostType != 1 && Condition.CostType != 2 && Condition.CostType != 5) || Condition.CostValue == 0
                        || (Condition.CostType == 5 && !Server.ItemsBase.ContainsKey(Condition.CostValue)))
                    {
                        MyConsole.WriteLine("[GoldenTree] condition " + Condition.ID + " skipped: CostType must be 1 (CPs), 2 (Silver) or 5 (item id in CostValue) with CostValue > 0.");
                        continue;
                    }
                    list.Add(Condition);
                }
                Conditions = list.ToArray();

                int lineNo = 0, prizes = 0;
                foreach (string line in File.ReadAllLines(poolPath))
                {
                    lineNo++;
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                        continue;
                    var Prize = GlobalLotteryConditionPrizes.TryParse(line);
                    if (Prize == null)
                    {
                        MyConsole.WriteLine("[GoldenTree] global_lottery_pool.txt line " + lineNo + " skipped: bad format.");
                        continue;
                    }
                    if (Prize.Count == 0)
                        continue;
                    if (!Server.ItemsBase.ContainsKey(Prize.PrizeItemID))
                    {
                        MyConsole.WriteLine("[GoldenTree] prize " + Prize.ID + " skipped: item " + Prize.PrizeItemID + " is not in itemtype.");
                        continue;
                    }
                    prizes += AddPrize(Prize) > 0 ? 1 : 0;
                }
                SetTodayCondition();
                Loaded = TodayCondition != null;
                MyConsole.WriteLine("Loading: " + Conditions.Length + " Golden Tree conditions, " + prizes + " prizes" + (Loaded ? "" : " (Golden Tree disabled: no condition with prizes)"));
            }
            catch (Exception e)
            {
                Loaded = false;
                MyConsole.WriteLine("[GoldenTree] load failed, Golden Tree disabled.");
                MyConsole.WriteException(e);
            }
        }

        #region Running event (one pool per "goldentree" console start)
        private static DateTime PoolEventEnd = DateTime.MinValue;
        private static readonly Dictionary<uint, int> PoolLeft = new Dictionary<uint, int>();
        private static readonly Dictionary<uint, DateTime> NextPick = new Dictionary<uint, DateTime>();

        /// <summary>The tree is blooming: files loaded, event started and not expired.</summary>
        public static bool IsActive
        {
            get { return Loaded && TodayCondition != null && DateTime.Now < GoldenTreeState.Expiration; }
        }
        // A new console start (new expiration date) refills the pool and picks the condition of this bloom.
        private static void EnsurePool()
        {
            if (PoolEventEnd == GoldenTreeState.Expiration)
                return;
            PoolEventEnd = GoldenTreeState.Expiration;
            NextPick.Clear();
            PoolLeft.Clear();
            // the bloom saved in state.ini (server restart): same condition, the prizes still in the tree
            if (GoldenTreeState.PoolExpiration == PoolEventEnd && GoldenTreeState.ConditionID != 0)
            {
                var saved = Conditions.FirstOrDefault(p => p.ID == GoldenTreeState.ConditionID && p.Prizes.Count > 0);
                if (saved != null)
                {
                    TodayCondition = saved;
                    foreach (var kv in GoldenTreeState.PoolLeft)
                        if (saved.PrizeItemID(kv.Key) != null)
                            PoolLeft[kv.Key] = kv.Value;
                    return;
                }
            }
            SetTodayCondition();
            if (DateTime.Now < PoolEventEnd)
                Game.MsgServer.MsgGoldenTree.ClearRank();
            if (TodayCondition == null)
                return;
            foreach (var prize in TodayCondition.Prizes)
            {
                int left;
                PoolLeft.TryGetValue(prize.ID, out left);
                PoolLeft[prize.ID] = left + prize.Count;
            }
        }
        /// <summary>Total picks of this bloom: the pool size, or the console limit if smaller.</summary>
        public static uint TotalPicks
        {
            get
            {
                lock (Sync)
                {
                    EnsurePool();
                    uint pool = 0;
                    if (TodayCondition != null)
                        foreach (var prize in TodayCondition.Prizes)
                            pool += prize.Count;
                    uint max = GoldenTreeState.MaxClaims;
                    return max == 0 ? pool : Math.Min(pool, max);
                }
            }
        }
        public static uint RemainingPicks
        {
            get
            {
                uint total = TotalPicks;
                lock (Sync)
                {
                    uint left = (uint)PoolLeft.Values.Sum();
                    uint byClaims = total > GoldenTreeState.Claimed ? total - GoldenTreeState.Claimed : 0;
                    return Math.Min(left, byClaims);
                }
            }
        }
        /// <summary>Seconds until the player may pick again (0 = now).</summary>
        public static uint CooldownLeft(uint uid)
        {
            lock (Sync)
            {
                EnsurePool();
                DateTime next;
                if (NextPick.TryGetValue(uid, out next) && next > DateTime.Now)
                    return (uint)Math.Ceiling((next - DateTime.Now).TotalSeconds);
                return 0;
            }
        }
        /// <summary>Takes one prize out of the pool (weighted by what is left) and starts the player's cooldown.</summary>
        public static GlobalLotteryConditionPrizes Draw(uint uid)
        {
            lock (Sync)
            {
                EnsurePool();
                if (TodayCondition == null)
                    return null;
                int total = PoolLeft.Values.Sum();
                if (total <= 0)
                    return null;
                int roll;
                lock (Rand)
                    roll = Rand.Next(0, total);
                foreach (var prize in TodayCondition.Prizes)
                {
                    int left;
                    if (!PoolLeft.TryGetValue(prize.ID, out left) || left <= 0)
                        continue;
                    if (roll < left)
                    {
                        PoolLeft[prize.ID] = left - 1;
                        if (TodayCondition.CD > 0)
                            NextPick[uid] = DateTime.Now.AddSeconds(TodayCondition.CD);
                        return prize;
                    }
                    roll -= left;
                }
                return null;
            }
        }
        /// <summary>Prize id -> count still in the tree (every prize of the bloom, 0 included).</summary>
        public static List<KeyValuePair<uint, int>> PrizesLeft()
        {
            lock (Sync)
            {
                EnsurePool();
                var list = new List<KeyValuePair<uint, int>>();
                if (TodayCondition != null)
                    foreach (var prize in TodayCondition.Prizes)
                    {
                        int left;
                        PoolLeft.TryGetValue(prize.ID, out left);
                        if (!list.Any(p => p.Key == prize.ID))
                            list.Add(new KeyValuePair<uint, int>(prize.ID, Math.Max(0, left)));
                    }
                return list;
            }
        }
        /// <summary>What state.ini keeps of the running bloom: its condition and the prizes left.</summary>
        public static void Snapshot(out uint conditionId, out DateTime expiration, out Dictionary<uint, int> left)
        {
            lock (Sync)
            {
                EnsurePool();
                conditionId = TodayCondition != null ? TodayCondition.ID : 0;
                expiration = PoolEventEnd;
                left = new Dictionary<uint, int>(PoolLeft);
            }
        }
        /// <summary>
        /// The client's ini/globallotterycondition.ini made from the server's file (sections [0]..[Amount-1], the keys the
        /// client reads). The client loads it once at start-up: without it the tree shows no prizes, no cost and refuses
        /// every pick. Ship the written file in the client patch (ini\globallotterycondition.ini).
        /// </summary>
        public static void WriteClientIni(string file)
        {
            var sb = new StringBuilder();
            sb.Append("[ConditionAmount]\r\nAmount=").Append(Conditions.Length).Append("\r\n");
            for (int i = 0; i < Conditions.Length; i++)
            {
                var c = Conditions[i];
                sb.Append("[").Append(i).Append("]\r\n")
                  .Append("Id=").Append(c.ID).Append("\r\n")
                  .Append("Type=").Append(c.Type).Append("\r\n")
                  .Append("CostType=").Append(c.CostType).Append("\r\n")
                  .Append("CostValue=").Append(c.CostValue).Append("\r\n")
                  .Append("Monopoly=").Append(c.Monopoly).Append("\r\n")
                  .Append("CD=").Append(c.CD).Append("\r\n")
                  .Append("CDCost=").Append(c.CDCost).Append("\r\n")
                  .Append("StartTime=").Append(c.StartTime == DateTime.MinValue ? "0" : c.StartTime.ToString("yyyyMMddHHmmss")).Append("\r\n")
                  .Append("Desc=").Append(c.Desc).Append("\r\n");
            }
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.WriteAllText(file, sb.ToString(), Encoding.ASCII);
        }
        /// <summary>Puts a drawn prize back (the pick failed after the draw).</summary>
        public static void Undo(uint uid, GlobalLotteryConditionPrizes prize)
        {
            if (prize == null)
                return;
            lock (Sync)
            {
                int left;
                PoolLeft.TryGetValue(prize.ID, out left);
                PoolLeft[prize.ID] = left + 1;
                NextPick.Remove(uid);
            }
        }
        #endregion
    }
}
