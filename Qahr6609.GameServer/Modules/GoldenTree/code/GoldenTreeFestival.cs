using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    /// <summary>
    /// The festival around the Golden Tree (data\festival.ini): festival days, the daily automatic bloom, the Anniversary
    /// Star sources (Chi Token swap, Outstanding Exploit Pack, Bliss Boy's Anniversary Blessing, Dragon Island hunting)
    /// and Bliss Boy. Each player's stars of the day are kept in stars.txt (uid|yyyyMMdd|blessing|island).
    /// </summary>
    public static class GoldenTreeFestival
    {
        public static DateTime StartDay = DateTime.MinValue, EndDay = DateTime.MaxValue;
        public static bool AutoBloom = true;
        public static TimeSpan BloomTime = new TimeSpan(8, 0, 0);
        public static double BloomHours = 3;
        public static uint MaxPicks;
        public static int AnnounceMinutes = 10;
        public static uint StarItem = 3303191;
        public static ushort ChiTokenStars = 3, ExploitPackStars = 3, BlessingStars = 3;
        public static uint IslandMap = 10137;
        public static int IslandChance = 10, IslandMax = 20;
        public static uint NpcMap = 1002;
        public static ushort NpcX = 308, NpcY = 244;
        public static byte NpcDir = 2;

        public const uint BlissBoyUid = 8156001;
        public const ushort BlissBoyLook = 4676;// client ini\npc.ini [NpcType4676] Name=BlissBoy

        /// <summary>Today is a festival day.</summary>
        public static bool IsOn
        {
            get { var d = DateTime.Now.Date; return d >= StartDay && d <= EndDay; }
        }

        /// <summary>Start of today's automatic bloom (MinValue = none today).</summary>
        public static DateTime TodayBloom
        {
            get { return AutoBloom && IsOn && BloomHours > 0 ? DateTime.Now.Date + BloomTime : DateTime.MinValue; }
        }

        /// <summary>Seconds until the next automatic bloom when it is less than Announce minutes away (else 0).</summary>
        public static uint SecondsToBloom
        {
            get
            {
                if (GlobalLotteryTable.IsActive) return 0;
                DateTime b = TodayBloom;
                if (b == DateTime.MinValue) return 0;
                if (DateTime.Now >= b) b = AutoBloom && IsOnDay(DateTime.Now.Date.AddDays(1)) ? b.AddDays(1) : DateTime.MinValue;
                if (b == DateTime.MinValue) return 0;
                double s = (b - DateTime.Now).TotalSeconds;
                return s > 0 && s <= AnnounceMinutes * 60 ? (uint)Math.Ceiling(s) : 0;
            }
        }
        static bool IsOnDay(DateTime d) { return d >= StartDay && d <= EndDay; }

        public static void Load(string folder)
        {
            string file = Path.Combine(folder, "festival.ini");
            if (!File.Exists(file))
            {
                MyConsole.WriteLine("[GoldenTree] festival.ini not found: no automatic bloom, no star sources.");
                AutoBloom = false;
                StartDay = DateTime.MaxValue;
                return;
            }
            var ini = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string section = "";
            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;
                if (line[0] == '[') { section = line.Trim('[', ']').Trim(); continue; }
                int eq = line.IndexOf('=');
                if (eq > 0) ini[section + "." + line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
            Func<string, string> S = k => { string v; return ini.TryGetValue(k, out v) ? v : ""; };
            Func<string, int, int> I = (k, d) => { int v; return int.TryParse(S(k), out v) ? v : d; };
            DateTime day;
            StartDay = DateTime.TryParseExact(S("Festival.Start"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out day) ? day : DateTime.MinValue;
            EndDay = DateTime.TryParseExact(S("Festival.End"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out day) ? day : DateTime.MaxValue.Date;
            AutoBloom = I("Bloom.Auto", 1) != 0;
            TimeSpan t;
            BloomTime = TimeSpan.TryParseExact(S("Bloom.Time"), @"hh\:mm", CultureInfo.InvariantCulture, out t) ? t : new TimeSpan(8, 0, 0);
            double h;
            BloomHours = double.TryParse(S("Bloom.Hours"), NumberStyles.Float, CultureInfo.InvariantCulture, out h) && h > 0 && h < 24 ? h : 3;
            MaxPicks = (uint)Math.Max(0, I("Bloom.MaxPicks", 0));
            AnnounceMinutes = Math.Max(0, Math.Min(120, I("Bloom.Announce", 10)));
            StarItem = (uint)Math.Max(0, I("Stars.ItemID", 3303191));
            ChiTokenStars = (ushort)Math.Max(0, Math.Min(100, I("Stars.ChiTokenSwap", 3)));
            ExploitPackStars = (ushort)Math.Max(0, Math.Min(100, I("Stars.ExploitPack", 3)));
            BlessingStars = (ushort)Math.Max(0, Math.Min(100, I("Stars.Blessing", 3)));
            IslandMap = (uint)Math.Max(0, I("Stars.IslandMap", 10137));
            IslandChance = Math.Max(0, Math.Min(100, I("Stars.IslandChance", 10)));
            IslandMax = Math.Max(0, I("Stars.IslandMax", 20));
            NpcMap = (uint)Math.Max(0, I("BlissBoy.Map", 1002));
            NpcX = (ushort)Math.Max(0, I("BlissBoy.X", 308));
            NpcY = (ushort)Math.Max(0, I("BlissBoy.Y", 244));
            NpcDir = (byte)(Math.Max(0, I("BlissBoy.Dir", 2)) % 8);
            if (!Server.ItemsBase.ContainsKey(StarItem))
                MyConsole.WriteLine("[GoldenTree] festival.ini: star item " + StarItem + " is not in itemtype, star sources off.");
        }

        public static string Describe()
        {
            return (IsOn ? "festival on" : "festival off") + (StartDay != DateTime.MinValue || EndDay != DateTime.MaxValue.Date
                    ? " (" + (StartDay == DateTime.MinValue ? "..." : StartDay.ToString("yyyy-MM-dd")) + " - " + (EndDay == DateTime.MaxValue.Date ? "..." : EndDay.ToString("yyyy-MM-dd")) + ")" : "")
                + (AutoBloom ? ", blooms daily at " + BloomTime.ToString(@"hh\:mm") + " for " + BloomHours + " h" : ", no automatic bloom");
        }

        // ------------------------------------------------------------------ stars of the day per player
        class Daily { public int Day; public bool Blessing; public int Island; }
        static readonly Dictionary<uint, Daily> days = new Dictionary<uint, Daily>();
        static readonly object sync = new object();
        static bool dirty;

        static int Today { get { return int.Parse(DateTime.Now.ToString("yyyyMMdd")); } }

        static Daily Get(uint uid)
        {
            Daily d;
            if (!days.TryGetValue(uid, out d) || d.Day != Today)
                days[uid] = d = new Daily { Day = Today };
            return d;
        }

        public static void LoadPlayers(string folder)
        {
            string file = Path.Combine(folder, "stars.txt");
            lock (sync)
            {
                days.Clear();
                if (!File.Exists(file)) return;
                int today = Today;
                foreach (string line in File.ReadAllLines(file))
                {
                    string[] p = line.Split('|');
                    uint uid; int day, b, isl;
                    if (p.Length >= 4 && uint.TryParse(p[0], out uid) && int.TryParse(p[1], out day) && day == today
                        && int.TryParse(p[2], out b) && int.TryParse(p[3], out isl))
                        days[uid] = new Daily { Day = day, Blessing = b != 0, Island = isl };
                }
            }
        }

        public static void SavePlayers(string folder)
        {
            string text;
            lock (sync)
            {
                if (!dirty) return;
                int today = Today;
                var sb = new StringBuilder("; uid|yyyyMMdd|blessing taken|Dragon Island stars (today only; written by the server)\r\n");
                foreach (var kv in days)
                    if (kv.Value.Day == today)
                        sb.Append(kv.Key).Append('|').Append(kv.Value.Day).Append('|').Append(kv.Value.Blessing ? 1 : 0).Append('|').Append(kv.Value.Island).Append("\r\n");
                text = sb.ToString();
                dirty = false;
            }
            Directory.CreateDirectory(folder);
            string file = Path.Combine(folder, "stars.txt"), tmp = file + ".tmp";
            File.WriteAllText(tmp, text);
            if (File.Exists(file)) File.Replace(tmp, file, null);
            else File.Move(tmp, file);
        }

        /// <summary>Room for the stars: a stack of them that still has room, or a free slot.</summary>
        public static bool HasRoom(Client.GameClient client, ushort count)
        {
            ItemType.DBItem db;
            if (!Server.ItemsBase.TryGetValue(StarItem, out db)) return false;
            foreach (var item in client.Inventory.ClientItems.Values)
                if (item.ITEM_ID == StarItem && item.Bound == 0 && item.StackSize + count <= db.StackSize)
                    return true;
            return client.Inventory.HaveSpace(1);
        }

        public static bool GiveStars(Client.GameClient client, ushort count, ServerSockets.Packet stream, string from)
        {
            if (count == 0 || !Server.ItemsBase.ContainsKey(StarItem) || !HasRoom(client, count))
                return false;
            client.Inventory.AddItemWitchStack(StarItem, 0, count, stream, false);
            client.SendSysMesage("You received " + count + " Anniversary Star" + (count > 1 ? "s" : "") + " (" + from + "). Use them to pick the Golden Tree!",
                Game.MsgServer.MsgMessage.ChatMode.System);
            return true;
        }

        /// <summary>Item hook, before the item's own code. true = stop (no room for the stars: the item is kept).</summary>
        public static bool OnItemUse(Client.GameClient client, Game.MsgServer.MsgGameItem item)
        {
            if (!IsOn || item == null) return false;
            ushort stars = 0;
            string from = null;
            if (item.ITEM_ID == 729304 && ChiTokenStars > 0)// 7 Chi Tokens swapped once a day (UseItem.EventRewards.cs)
            {
                if (client.Player.UseChiToken != 0 || !client.Inventory.Contain(729304, 7)) return false;
                stars = ChiTokenStars; from = "Chi Token";
            }
            else if (item.ITEM_ID == 3007108 && ExploitPackStars > 0)// Outstanding Exploit Pack
            {
                stars = ExploitPackStars; from = "Outstanding Exploit Pack";
            }
            if (stars == 0) return false;
            if (!HasRoom(client, stars))
            {
                client.SendSysMesage("Please make 1 more space in your inventory for the Anniversary Stars.");
                return true;
            }
            using (var rec = new ServerSockets.RecycledPacket())
                GiveStars(client, stars, rec.GetStream(), from);
            return false;
        }

        static readonly Random rand = new Random();

        public static void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            if (killer == null || killer.Player == null || monster == null || !IsOn || IslandChance <= 0 || IslandMax <= 0)
                return;
            if (killer.Player.Map != IslandMap) return;
            int roll;
            lock (rand) roll = rand.Next(100);
            if (roll >= IslandChance) return;
            int got;
            lock (sync)
            {
                var d = Get(killer.Player.UID);
                if (d.Island >= IslandMax) return;
                d.Island++;
                got = d.Island;
                dirty = true;
            }
            using (var rec = new ServerSockets.RecycledPacket())
                if (!GiveStars(killer, 1, rec.GetStream(), "Dragon Island " + got + "/" + IslandMax + " today"))
                    lock (sync) { Get(killer.Player.UID).Island--; }
        }

        // ------------------------------------------------------------------ Bliss Boy
        public static void BlissBoy(Client.GameClient client, ServerSockets.Packet stream, byte option)
        {
            var dlg = new Game.MsgNpc.Dialog(client, stream);
            string when = StartDay != DateTime.MinValue || EndDay != DateTime.MaxValue.Date
                ? "~Time:~" + (StartDay == DateTime.MinValue ? "" : StartDay.ToString("MMM~d", CultureInfo.InvariantCulture)) + "~to~" + (EndDay == DateTime.MaxValue.Date ? "" : EndDay.ToString("MMM~d", CultureInfo.InvariantCulture)) + "." : "";
            switch (option)
            {
                case 1:// how to obtain
                    dlg.AddText("[How~to~Obtain]~~~~~~~~~~~~[Number~of~Stars]\n"
                        + "Chi~Token~(swap~7,~daily~quests)~~~" + ChiTokenStars + "\n"
                        + "Outstanding~Exploit~Pack~~~~~~~~~~~~" + ExploitPackStars + "\n"
                        + "Anniversary~Blessing~(here,~daily)~~" + BlessingStars + "\n"
                        + "Dragon~Island~Hunting~~~~~~~~~~~~~~~up~to~" + IslandMax + "\n"
                        + "It~must~be~easy~for~you~to~collect~the~Anniversary~Stars." + when);
                    dlg.AddOption("Anniversary Blessing.", 2);
                    dlg.AddOption("I see.", 255);
                    break;
                case 2:// Anniversary Blessing, once a day
                    {
                        if (!IsOn) { dlg.AddText("The~festival~is~over.~See~you~next~time!"); dlg.AddOption("I see.", 255); break; }
                        bool taken;
                        lock (sync) taken = Get(client.Player.UID).Blessing;
                        if (taken)
                        {
                            dlg.AddText("You~have~received~today's~Anniversary~Blessing.~Come~back~tomorrow!");
                            dlg.AddOption("I see.", 255);
                            break;
                        }
                        if (!GiveStars(client, BlessingStars, stream, "Anniversary Blessing"))
                        {
                            dlg.AddText("Please~make~1~more~space~in~your~inventory.");
                            dlg.AddOption("I see.", 255);
                            break;
                        }
                        lock (sync) { Get(client.Player.UID).Blessing = true; dirty = true; }
                        dlg.AddText("Happy~anniversary!~Here~are~" + BlessingStars + "~Anniversary~Stars.~Go~harvest~the~Golden~Tree!");
                        dlg.AddOption("Thanks!", 255);
                        break;
                    }
                default:
                    dlg.AddText("My~grandpa~said,~money~doesn't~grow~on~trees.~He~may~be~right.~It's~treasure~chests~that~grow~on~the~Golden~Tree.~"
                        + "If~you~have~an~Anniversary~Star,~go~harvest~the~Golden~Tree~for~blissful~gifts!"
                        + (AutoBloom ? "~It~blooms~every~day~at~" + BloomTime.ToString(@"hh\:mm") + "~(server~time)." : "") + when);
                    dlg.AddOption("How to obtain Anniversary Stars?", 1);
                    dlg.AddOption("Anniversary Blessing.", 2);
                    dlg.AddOption("I see.", 255);
                    break;
            }
            dlg.AddAvatar(0).FinalizeDialog();
        }

        static Game.MsgNpc.Npc npc;
        static readonly object npcSync = new object();

        /// <summary>Bliss Boy stands in town on festival days.</summary>
        public static void SyncNpc()
        {
            lock (npcSync)
            {
                bool want = IsOn;
                if (want == (npc != null)) return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    if (!want)
                    {
                        Role.GameMap old;
                        if (Server.ServerMaps.TryGetValue(npc.Map, out old))
                            try { old.RemoveNpc(npc, rec.GetStream()); } catch (Exception e) { MyConsole.WriteException(e); }
                        npc = null;
                        return;
                    }
                    Role.GameMap map;
                    if (!Server.ServerMaps.TryGetValue(NpcMap, out map) || map.View.MapContain(Role.MapObjectType.Npc, BlissBoyUid))
                        return;
                    var np = Game.MsgNpc.Npc.Create();
                    np.UID = BlissBoyUid;
                    np.NpcType = Role.Flags.NpcType.Talker;
                    np.Mesh = (ushort)(BlissBoyLook * 10 + NpcDir);
                    np.Map = NpcMap;
                    np.X = NpcX;
                    np.Y = NpcY;
                    map.AddNpc(np);
                    npc = np;
                }
            }
        }

        public static void RemoveNpc()
        {
            lock (npcSync)
            {
                if (npc == null) return;
                Role.GameMap map;
                using (var rec = new ServerSockets.RecycledPacket())
                    if (Server.ServerMaps.TryGetValue(npc.Map, out map))
                        try { map.RemoveNpc(npc, rec.GetStream()); } catch (Exception e) { MyConsole.WriteException(e); }
                npc = null;
            }
        }
    }
}

namespace Conquer6609.Game.MsgNpc
{
    public unsafe partial class NpcHandler
    {
        [NpcAttribute((NpcID)Database.GoldenTreeFestival.BlissBoyUid)]
        public static void GoldenTreeBlissBoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Database.GoldenTreeFestival.BlissBoy(client, stream, Option);
        }
    }
}
