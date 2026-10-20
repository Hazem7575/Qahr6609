using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class LotteryTable
    {

        public class LotteryItem
        {
            public int Rank, Chance;
            public string Name;
            public uint ID;
            public byte Color;
            public byte Sockets;
            public byte Plus;
            /// <summary>In lottery.ini: the only box this row belongs to ("" = every box). On a drawn prize: the box it came from.</summary>
            public string Box = "";

            public override string ToString()
            {
                return Rank + " " + Chance + " " + Name + " " + ID + " " + Color + " " + Sockets + " " + Plus + " " + Box;
            }
        }

        // ------------------------------------------------------------------ boxes
        // The 5 Lucky Boxes of the Lottery Center (map 700). The client names them by npc type (npc.ini
        // [Npctype925..930]); the server's Npcs.txt gives each box its look (mesh / 10 = npc type).
        public const string BoxAssorted = "Assorted";
        public const string BoxStarStone = "StarStone";
        public const string BoxSteed = "Steed";
        public const string BoxStone = "Stone";
        public const string BoxChi = "Chi";
        public static readonly string[] Boxes = { BoxAssorted, BoxStarStone, BoxSteed, BoxStone, BoxChi };

        /// <summary>Box of a Lucky Box npc. lookType = npc mesh / 10 (npc.ini type); npcUid is the fallback when the
        /// npc could not be found (Npcs.txt: 925 Assorted, 926 Steed, 927 StarStone, 928 Stone, 930 Chi).</summary>
        public static string BoxForNpc(uint lookType, uint npcUid)
        {
            switch (lookType)
            {
                case 925: return BoxAssorted;
                case 926: return BoxStarStone;
                case 927: return BoxSteed;
                case 928: return BoxStone;
                case 930: return BoxChi;
                case 929: return BoxAssorted;// LuckyBox (not spawned)
            }
            switch (npcUid)
            {
                case 926: return BoxSteed;
                case 927: return BoxStarStone;
                case 928: return BoxStone;
                case 930: return BoxChi;
            }
            return BoxAssorted;
        }

        static string NormalizeBox(string box)
        {
            if (string.IsNullOrWhiteSpace(box))
                return "";
            foreach (var b in Boxes)
                if (string.Equals(b, box.Trim(), StringComparison.OrdinalIgnoreCase))
                    return b;
            return null;
        }

        /// <summary>box -> rank -> prizes (general rows are copied into every box).</summary>
        private Dictionary<string, Dictionary<int, List<LotteryItem>>> Pools = new Dictionary<string, Dictionary<int, List<LotteryItem>>>();

        public void LoadLotteryItems()
        {
            var general = new List<LotteryItem>();
            var themed = new List<LotteryItem>();
            string path = Program.ServerConfig.DbLocation + "lottery.ini";
            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("lottery.ini: can't read " + path + " (" + e.Message + ") - the Lottery has no prizes.");
                lines = new string[0];
            }
            foreach (var line0 in lines)
            {
                string raw = line0;
                int comment = raw.IndexOf(';');
                if (comment >= 0)
                    raw = raw.Substring(0, comment);
                raw = raw.Trim();
                if (raw.Length == 0 || raw.StartsWith("Count=", StringComparison.OrdinalIgnoreCase))
                    continue;
                string[] f = raw.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                LotteryItem item = new LotteryItem();
                int rank = 0, chance = 0;
                uint id = 0;
                byte color = 0, sockets = 0, plus = 0;
                if (f.Length < 4 || !int.TryParse(f[0], out rank) || !int.TryParse(f[1], out chance) || !uint.TryParse(f[3], out id))
                {
                    MyConsole.WriteLine("lottery.ini: skipped bad line -> " + line0.Trim());
                    continue;
                }
                if (f.Length > 4) byte.TryParse(f[4], out color);
                if (f.Length > 5) byte.TryParse(f[5], out sockets);
                if (f.Length > 6) byte.TryParse(f[6], out plus);
                string box = NormalizeBox(f.Length > 7 ? f[7] : "");
                item.Rank = rank;
                item.Chance = chance;
                item.Name = f[2];
                item.ID = id;
                item.Color = color;
                item.Sockets = Math.Min(sockets, (byte)2);
                item.Plus = plus;
                // a +N Stone is the same item id with Plus N: a row that forgot the plus gave a useless +0 stone
                if (item.ID >= 730001 && item.ID <= 730009 && item.Plus == 0)
                    item.Plus = (byte)(item.ID - 730000);
                // a prize the server cannot create would leave the draw stuck (Accept threw on ItemsBase[ID])
                if (box == null || item.Rank < 1 || item.Rank > RankCount || item.ID == 0 || !Database.Server.ItemsBase.ContainsKey(item.ID))
                {
                    MyConsole.WriteLine("lottery.ini: skipped invalid prize -> " + line0.Trim());
                    continue;
                }
                if (item.Chance <= 0)
                    item.Chance = 1;
                item.Box = box;
                if (box.Length == 0)
                    general.Add(item);
                else
                    themed.Add(item);
            }

            var pools = new Dictionary<string, Dictionary<int, List<LotteryItem>>>();
            foreach (var b in Boxes)
            {
                var ranks = new Dictionary<int, List<LotteryItem>>();
                foreach (var it in general.Concat(themed.Where(t => t.Box == b)))
                {
                    List<LotteryItem> list;
                    if (!ranks.TryGetValue(it.Rank, out list))
                        ranks.Add(it.Rank, list = new List<LotteryItem>());
                    list.Add(it);
                }
                pools[b] = ranks;
            }
            Pools = pools;
            var summary = new StringBuilder("lottery.ini: " + general.Count + " general prizes");
            foreach (var b in Boxes)
                summary.Append(", ").Append(b).Append(" +").Append(themed.Count(t => t.Box == b));
            MyConsole.WriteLine(summary.ToString());
        }

        Dictionary<int, List<LotteryItem>> Pool(string box)
        {
            Dictionary<int, List<LotteryItem>> ranks;
            if (box != null && Pools.TryGetValue(box, out ranks) && ranks.Count > 0)
                return ranks;
            if (Pools.TryGetValue(BoxAssorted, out ranks))
                return ranks;
            return new Dictionary<int, List<LotteryItem>>();
        }

        /// <summary>Prize ranks 1 (best) .. 6 (common) as in lottery.ini.</summary>
        public const int RankCount = 6;
        /// <summary>
        /// Chance per 1,000,000 draws of ranks 1..5; the rest is rank 6. These are the effective rates of the old
        /// global "every Nth draw" counters (600/500/300/100/40), now rolled independently on every draw, so the
        /// result can no longer be predicted or forced by counting draws/re-draws. The same for every box.
        /// </summary>
        public static readonly int[] RankPpm = { 1667, 1997, 3317, 9917, 24543 };

        public int GenerateRank(Dictionary<int, List<LotteryItem>> pool)
        {
            int roll = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1000000);
            int acc = 0;
            for (int x = 0; x < RankPpm.Length; x++)
            {
                acc += RankPpm[x];
                if (roll < acc && pool.ContainsKey(x + 1))
                    return x + 1;
            }
            return RankCount;
        }

        /// <summary>Official daily draws: 10, plus 10 per VIP level (client STR_VIP_LOTTERY_2..7: VIP1 +10 .. VIP6 +60).</summary>
        public int LotteryEntry(byte vipLevel)
        {
            return 10 + 10 * Math.Min((int)vipLevel, 6);
        }

        /// <summary>
        /// Draws one prize from a box: the rank is rolled independently, then the item inside the rank is weighted by
        /// its Chance column. Returns a copy whose Color is fixed now, so the 1314 window shows the colour the player gets.
        /// </summary>
        public LotteryItem GenerateLotteryItem(Client.GameClient user, string box)
        {
            var pool = Pool(box);
            if (pool.Count == 0)
                return null;
            int Rank = GenerateRank(pool);
            List<LotteryItem> items;
            if (!pool.TryGetValue(Rank, out items) || items.Count == 0)
                items = pool.Values.First(l => l.Count > 0);
            int total = 0;
            foreach (var it in items)
                total += it.Chance;
            int roll = System.Security.Cryptography.RandomNumberGenerator.GetInt32(total);
            LotteryItem pick = items[items.Count - 1];
            foreach (var it in items)
            {
                if (roll < it.Chance) { pick = it; break; }
                roll -= it.Chance;
            }
            return new LotteryItem
            {
                Rank = pick.Rank,
                Chance = pick.Chance,
                Name = pick.Name,
                ID = pick.ID,
                Sockets = pick.Sockets,
                Plus = pick.Plus,
                Box = (box != null && Pools.ContainsKey(box)) ? box : BoxAssorted,
                Color = (byte)System.Security.Cryptography.RandomNumberGenerator.GetInt32(4, 8)
            };
        }

        // ------------------------------------------------------------------ un-accepted prize across logout
        /// <summary>Character save value of a drawn, not yet accepted prize: Box/Rank/ID/Color/Sockets/Plus/ReDraws ("0" = none).</summary>
        public static string SavePrize(Role.Player player)
        {
            var it = player.LotteryItem;
            if (it == null)
                return "0";
            return (it.Box ?? BoxAssorted) + "/" + it.Rank + "/" + it.ID + "/" + it.Color + "/" + it.Sockets + "/" + it.Plus + "/" + player.AddJade;
        }

        /// <summary>Restores a prize saved by SavePrize at login; an unknown/invalid value is dropped (and logged).</summary>
        public static void LoadPrize(Role.Player player, string value)
        {
            player.LotteryItem = null;
            player.AddJade = 0;
            if (string.IsNullOrWhiteSpace(value) || value.Trim() == "0")
                return;
            try
            {
                string[] f = value.Split('/');
                if (f.Length < 7)
                    throw new FormatException("fields");
                string box = NormalizeBox(f[0]);
                var it = new LotteryItem
                {
                    Box = string.IsNullOrEmpty(box) ? BoxAssorted : box,
                    Rank = int.Parse(f[1]),
                    ID = uint.Parse(f[2]),
                    Color = byte.Parse(f[3]),
                    Sockets = Math.Min(byte.Parse(f[4]), (byte)2),
                    Plus = byte.Parse(f[5])
                };
                byte reDraws = byte.Parse(f[6]);
                Database.ItemType.DBItem db;
                if (it.ID == 0 || !Database.Server.ItemsBase.TryGetValue(it.ID, out db) || it.Rank < 1 || it.Rank > RankCount)
                    throw new FormatException("item " + it.ID);
                if (it.Color < 4 || it.Color > 7)
                    it.Color = 4;
                // the name the draw showed (lottery.ini name, e.g. "Stone+8"); the item name when the row is gone
                it.Name = db.Name;
                if (Server.Lottery != null)
                {
                    var row = Server.Lottery.FindRow(it);
                    if (row != null)
                    {
                        it.Name = row.Name;
                        it.Chance = row.Chance;
                    }
                }
                player.LotteryItem = it;
                player.AddJade = Math.Min(reDraws, (byte)2);
            }
            catch (Exception e)
            {
                Log(player.Name + " (" + player.UID + ") saved lottery prize '" + value + "' dropped: " + e.Message);
            }
        }

        LotteryItem FindRow(LotteryItem it)
        {
            List<LotteryItem> list;
            if (!Pool(it.Box).TryGetValue(it.Rank, out list))
                return null;
            return list.FirstOrDefault(r => r.ID == it.ID && r.Sockets == it.Sockets && r.Plus == it.Plus);
        }

        public Game.MsgServer.MsgGameItem CreateGameItem(LotteryItem Item)
        {
            Game.MsgServer.MsgGameItem GameItem = new Game.MsgServer.MsgGameItem();
            GameItem.ITEM_ID = Item.ID;
            GameItem.Color = (Role.Flags.Color)Item.Color;
            GameItem.Plus = Item.Plus;
            if (Item.Sockets > 0)
                GameItem.SocketOne = Role.Flags.Gem.EmptySocket;
            if (Item.Sockets > 1)
                GameItem.SocketTwo = Role.Flags.Gem.EmptySocket;
            GameItem.UID = Database.Server.ITEM_Counter.Next;
            var DBItem = Database.Server.ItemsBase[Item.ID];
            GameItem.Durability = GameItem.MaximDurability = DBItem.Durability;
            if (DBItem.StackSize > 1)
                GameItem.StackSize = 1;
            return GameItem;
        }

        public static void Log(string text)
        {
            Game.Conquer6609_Work.Logs.OnDequeue("[Lottery]" + DateTime.Now.ToString("HH:mm:ss") + " " + text);
        }
    }
}
