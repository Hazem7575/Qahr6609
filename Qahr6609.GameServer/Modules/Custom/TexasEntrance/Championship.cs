// Texas Hold'em CHAMPIONSHIPS ("Tournaments" button of the 6609 poker window), single-server version of the official
// cross-server tournaments. Protocol from the client code (RE spec, docs\TEXAS_CHAMPIONSHIP.md):
//   3402 Today list (3 rows / page), 3400 actions 2 list, 3 sign up, 4/22 enter, 5 rebuy/add-on, 9 -> 3404 ranking,
//   14 -> 3405 live rank, 23 leave, 12 login; server -> client 3400 acts 3,4,8,12,15,16,18,19,22, 3406 chips.
//   The table itself is the client's CDlgTexasBoard: 2501 CMsgCrossSwitch mode 0 makes the client route the ShowHand
//   messages 2090-2099 to the board, mode 1 closes it. The hands are played by PokerTable in board mode
//   (tournament chips = this session's bank, offline players on auto play, held between hands for this manager).
// Threads: one manager lock (Sync). Table callbacks only queue (lock order: Sync -> table.SyncRoot, never reverse).
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.Poker;

namespace Conquer6609.Modules.TexasEntrance
{
    public enum ChampState { Open, Running, Finished, Cancelled }

    public sealed class ChampEntry
    {
        public uint Uid;
        public string Name = "";
        /// <summary>0 = fee in the championship currency, 1 = entry ticket, 2 = free (test).</summary>
        public int PaidWith;
        public long PaidAmount;
        public uint PaidItem;
        /// <summary>Rebuy / add-on payments: money in the championship currency and tickets.</summary>
        public long ExtraMoney;
        public List<uint> ExtraItems = new List<uint>();
        public int Rebuys, Addons;
        public bool Alive = true;
        public int Rank;
        public bool Ready;
        /// <summary>The client is in board mode for this championship (2501 mode 0 sent).</summary>
        public bool InBoard;
        public uint Room;
        public long FinalChips;
    }

    public sealed class ChampSession : IPokerTableOwner, IPokerChipBank
    {
        public uint SessionId;
        public ChampDef Def;
        public DateTime StartAt;
        public int HHMM;
        public bool Test;
        public int MinOverride;
        public int LevelSecondsOverride;
        public long FeeOverride = -1;
        public ChampState State = ChampState.Open;
        public readonly Dictionary<uint, ChampEntry> Entries = new Dictionary<uint, ChampEntry>();
        public readonly List<PokerTable> Tables = new List<PokerTable>();
        public int Participants;
        /// <summary>Rebuy / add-on money added to the prize pool.</summary>
        public long ExtraPool;
        public DateTime PlayStart;
        public int Level = -1;
        public bool Pushed, ReadySent, Released;
        public long ChipsIssued, ChipsForfeited;
        public readonly ConcurrentQueue<PokerTable> HandsDone = new ConcurrentQueue<PokerTable>();
        readonly Dictionary<uint, long> chips = new Dictionary<uint, long>();
        readonly object chipLock = new object();

        public long Fee { get { return FeeOverride >= 0 ? FeeOverride : Def.Fee; } }
        public int MinPlayers { get { return MinOverride > 0 ? MinOverride : Def.MinPlayers; } }
        public int AliveCount { get { return Entries.Values.Count(e => e.Alive); } }

        // ---- IPokerChipBank
        public long Balance(uint uid) { lock (chipLock) { long v; return chips.TryGetValue(uid, out v) ? v : 0; } }
        public bool Debit(uint uid, long amount)
        {
            lock (chipLock)
            {
                long v;
                if (amount < 0 || !chips.TryGetValue(uid, out v) || v < amount)
                    return false;
                chips[uid] = v - amount;
                return true;
            }
        }
        public void Credit(uint uid, long amount)
        {
            if (amount <= 0)
                return;
            lock (chipLock)
            {
                long v;
                chips.TryGetValue(uid, out v);
                chips[uid] = v + amount;
            }
        }
        public long Take(uint uid)
        {
            lock (chipLock)
            {
                long v;
                if (!chips.TryGetValue(uid, out v))
                    return 0;
                chips.Remove(uid);
                return v;
            }
        }
        public long TotalChips { get { lock (chipLock) return chips.Values.Sum(); } }

        // ---- IPokerTableOwner (called under the table lock: only queue)
        public void OnHandFinished(PokerTable table) { HandsDone.Enqueue(table); }

        public PokerTable TableOf(uint uid)
        {
            foreach (var t in Tables)
                lock (t.SyncRoot)
                    if (t.SeatOf(uid) != null)
                        return t;
            return null;
        }

        public TourLevel CurrentLevel
        {
            get
            {
                if (Def.Levels.Count == 0)
                    return new TourLevel { Seconds = 120, BigBlind = 1000 };
                return Def.Levels[Math.Max(0, Math.Min(Def.Levels.Count - 1, Level))];
            }
        }

        /// <summary>Alive players by chips (desc), then the eliminated ones by rank.</summary>
        public List<ChampEntry> Standings()
        {
            var alive = Entries.Values.Where(e => e.Alive).OrderByDescending(e => Balance(e.Uid)).ThenBy(e => e.Uid).ToList();
            alive.AddRange(Entries.Values.Where(e => !e.Alive).OrderBy(e => e.Rank == 0 ? int.MaxValue : e.Rank));
            return alive;
        }
    }

    public static class Championship
    {
        public static readonly object Sync = new object();
        static Dictionary<uint, ChampDef> defs = new Dictionary<uint, ChampDef>();
        static Dictionary<string, uint> itemNames = new Dictionary<string, uint>();
        static readonly Dictionary<uint, ChampSession> sessions = new Dictionary<uint, ChampSession>();
        static DateTime builtFor = DateTime.MinValue;
        static System.Threading.Timer timer;
        static int busy;
        static uint nextRoom = 1;
        static int testIndex = 90;
        static readonly ConcurrentQueue<KeyValuePair<uint, DateTime>> relogins = new ConcurrentQueue<KeyValuePair<uint, DateTime>>();
        static TexasEntranceModule module;
        static string dataPath = "";

        // ------------------------------------------------------------------ settings ([Texas] in shell.ini)
        public static bool Enabled { get { return Framework.Settings.Bool("Texas", "Championships", false); } }
        static string IdList { get { return Framework.Settings.String("Texas", "Ids", "2998,2999"); } }
        static int StartWaitSeconds { get { return Math.Max(3, Framework.Settings.Int("Texas", "StartWaitSeconds", 10)); } }
        static int PushSeconds { get { return Framework.Settings.Int("Texas", "PushSeconds", 300); } }
        static int ReadySeconds { get { return Framework.Settings.Int("Texas", "ReadySeconds", 180); } }
        static bool AutoEnterOnline { get { return Framework.Settings.Bool("Texas", "AutoEnterOnline", true); } }
        static bool EarlyBoard { get { return Framework.Settings.Bool("Texas", "EarlyBoard", false); } }
        static bool PoolIncludesRebuys { get { return Framework.Settings.Bool("Texas", "PoolIncludesRebuys", true); } }
        /// <summary>How a moved player is shown his new table: 19 = act 19 [room] + the new table, 4 = act 4 again.</summary>
        static int MoveMode { get { return Framework.Settings.Int("Texas", "MoveMode", 19); } }
        static int LevelSecondsAll { get { return Framework.Settings.Int("Texas", "LevelSeconds", 0); } }
        static string ServerName { get { return Program.ServerConfig.ServerName ?? ""; } }

        static void Log(string s)
        {
            if (module != null)
                module.Log("[champ] " + s);
        }

        // ------------------------------------------------------------------ start-up
        public static void Init(TexasEntranceModule m)
        {
            module = m;
            dataPath = m.DataPath;
            try
            {
                Directory.CreateDirectory(Path.Combine(dataPath, "sessions"));
                Directory.CreateDirectory(Path.Combine(dataPath, "results"));
                Reload();
                lock (Sync)
                {
                    BuildToday(DateTime.Now);
                    RestorePersisted();
                    string test = Framework.Settings.String("Texas", "TestSession", "");
                    if (test.Length > 0)
                    {
                        var p = test.Split(',').Select(x => x.Trim()).ToArray();
                        string err;
                        CreateTest(p.Length > 0 ? uint.Parse(p[0]) : 2998, p.Length > 1 ? int.Parse(p[1]) : 2,
                            p.Length > 2 ? int.Parse(p[2]) : 120, p.Length > 3 ? int.Parse(p[3]) : 0, p.Length > 4 ? long.Parse(p[4]) : -1, out err);
                        Log("TestSession " + test + ": " + (err ?? "created"));
                    }
                }
            }
            catch (Exception e)
            {
                Log("init failed: " + e);
            }
            timer = new System.Threading.Timer(OnTimer, null, 2000, 500);
        }

        public static string Reload()
        {
            var d = ChampData.Load(dataPath, Framework.Settings.String("Texas", "ClientIni", @"D:\Conquer Online 3.0\ini"), Log);
            var names = ChampData.LoadItemNames(Path.Combine(dataPath, "prize_items.txt"));
            lock (Sync)
            {
                defs = d;
                itemNames = names;
            }
            string msg = defs.Count + " championships (" + string.Join(",", EnabledIds().Where(defs.ContainsKey)) + " enabled), " + names.Count + " reward names";
            Log("loaded " + msg);
            return msg;
        }

        static IEnumerable<uint> EnabledIds()
        {
            foreach (var s in IdList.Split(','))
            {
                uint id;
                if (uint.TryParse(s.Trim(), out id))
                    yield return id;
            }
        }

        /// <summary>Today's sessions of the enabled championships (kept: sessions still running or with sign-ups).</summary>
        static void BuildToday(DateTime now)
        {
            var day = now.Date;
            builtFor = day;
            foreach (var key in sessions.Keys.ToList())
            {
                var s = sessions[key];
                if (s.StartAt.Date != day && (s.State == ChampState.Finished || s.State == ChampState.Cancelled))
                    sessions.Remove(key);
            }
            if (!Enabled)
                return;
            var list = new List<KeyValuePair<ChampDef, int>>();
            foreach (var id in EnabledIds())
            {
                ChampDef d;
                if (!defs.TryGetValue(id, out d) || !d.HasCondition || !d.RunsOn(day))
                    continue;
                foreach (var t in d.Times)
                    list.Add(new KeyValuePair<ChampDef, int>(d, t));
            }
            int index = 0;
            foreach (var kv in list.OrderBy(x => x.Value).ThenBy(x => x.Key.Id))
            {
                uint sid = (uint)((day.Year % 100) * 10000 + day.Month * 100 + day.Day) * 100 + (uint)index++;
                if (index >= 90)
                    break;
                if (sessions.ContainsKey(sid))
                    continue;
                var s = new ChampSession
                {
                    SessionId = sid,
                    Def = kv.Key,
                    HHMM = kv.Value,
                    StartAt = day.AddHours(kv.Value / 100).AddMinutes(kv.Value % 100)
                };
                if (s.StartAt <= now)
                    s.State = ChampState.Cancelled;// already past (server started later): shown as over
                sessions[sid] = s;
            }
        }

        /// <summary>A test championship starting in <paramref name="startSeconds"/> (GM command / [Texas] TestSession).</summary>
        public static ChampSession CreateTest(uint champId, int minPlayers, int startSeconds, int levelSeconds, long fee, out string error)
        {
            error = null;
            ChampDef d;
            if (!defs.TryGetValue(champId, out d) || !d.HasCondition)
            {
                error = "unknown championship " + champId;
                return null;
            }
            var day = DateTime.Now.Date;
            if (testIndex > 99)
                testIndex = 90;
            uint sid = (uint)((day.Year % 100) * 10000 + day.Month * 100 + day.Day) * 100 + (uint)testIndex++;
            var start = DateTime.Now.AddSeconds(Math.Max(10, startSeconds));
            var s = new ChampSession
            {
                SessionId = sid,
                Def = d,
                StartAt = start,
                HHMM = start.Hour * 100 + start.Minute,
                Test = true,
                MinOverride = Math.Max(2, minPlayers),
                LevelSecondsOverride = levelSeconds,
                FeeOverride = fee
            };
            ChampSession old;
            if (sessions.TryGetValue(sid, out old) && (old.State == ChampState.Running || old.Entries.Count > 0))
            {
                error = "test slot " + sid + " busy";
                return null;
            }
            sessions[sid] = s;
            Persist(s);
            return s;
        }

        // ------------------------------------------------------------------ persistence (a restart refunds)
        static string FileOf(ChampSession s) { return Path.Combine(Path.Combine(dataPath, "sessions"), s.SessionId + ".txt"); }

        static void Persist(ChampSession s)
        {
            try
            {
                if (s.State == ChampState.Finished || s.State == ChampState.Cancelled || (s.Entries.Count == 0 && !s.Test))
                {
                    if (File.Exists(FileOf(s)))
                        File.Delete(FileOf(s));
                    return;
                }
                var sb = new StringBuilder();
                sb.AppendLine("session=" + s.SessionId);
                sb.AppendLine("champ=" + s.Def.Id);
                sb.AppendLine("start=" + s.StartAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                sb.AppendLine("state=" + s.State);
                sb.AppendLine("currency=" + s.Def.Currency);
                sb.AppendLine("test=" + (s.Test ? 1 : 0) + "," + s.MinOverride + "," + s.LevelSecondsOverride + "," + s.FeeOverride);
                foreach (var e in s.Entries.Values)
                    sb.AppendLine("entry=" + e.Uid + "|" + e.Name.Replace("|", "") + "|" + e.PaidWith + "|" + e.PaidAmount + "|" + e.PaidItem + "|" + e.ExtraMoney + "|"
                        + string.Join(",", e.ExtraItems) + "|" + e.Rebuys + "|" + e.Addons);
                string tmp = FileOf(s) + ".tmp";
                File.WriteAllText(tmp, sb.ToString(), Encoding.UTF8);
                if (File.Exists(FileOf(s)))
                    File.Delete(FileOf(s));
                File.Move(tmp, FileOf(s));
            }
            catch (Exception e)
            {
                Log("persist " + s.SessionId + " failed: " + e.Message);
            }
        }

        /// <summary>
        /// Sessions saved before a restart: sign-ups of a championship that has not started yet are kept; a championship
        /// that was running (or whose start passed while the server was down) is refunded to every player (fee, ticket,
        /// rebuys, add-ons) by mail, and logged.
        /// </summary>
        static void RestorePersisted()
        {
            foreach (var file in Directory.GetFiles(Path.Combine(dataPath, "sessions"), "*.txt"))
            {
                try
                {
                    var kv = new Dictionary<string, string>();
                    var entries = new List<ChampEntry>();
                    foreach (var line in File.ReadAllLines(file, Encoding.UTF8))
                    {
                        int eq = line.IndexOf('=');
                        if (eq <= 0)
                            continue;
                        string k = line.Substring(0, eq), v = line.Substring(eq + 1);
                        if (k == "entry")
                        {
                            var p = v.Split('|');
                            if (p.Length < 9)
                                continue;
                            var e = new ChampEntry
                            {
                                Uid = uint.Parse(p[0]),
                                Name = p[1],
                                PaidWith = int.Parse(p[2]),
                                PaidAmount = long.Parse(p[3]),
                                PaidItem = uint.Parse(p[4]),
                                ExtraMoney = long.Parse(p[5]),
                                Rebuys = int.Parse(p[7]),
                                Addons = int.Parse(p[8])
                            };
                            foreach (var x in p[6].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                e.ExtraItems.Add(uint.Parse(x));
                            entries.Add(e);
                        }
                        else
                            kv[k] = v;
                    }
                    uint sid = uint.Parse(kv["session"]);
                    uint champ = uint.Parse(kv["champ"]);
                    var start = DateTime.ParseExact(kv["start"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    byte currency = kv.ContainsKey("currency") ? byte.Parse(kv["currency"]) : (byte)0;
                    string state = kv.ContainsKey("state") ? kv["state"] : "Open";
                    ChampSession s;
                    bool keep = state == "Open" && start > DateTime.Now.AddSeconds(30) && sessions.TryGetValue(sid, out s) && s.Def.Id == champ && s.StartAt == start;
                    if (keep)
                    {
                        s = sessions[sid];
                        foreach (var e in entries)
                            s.Entries[e.Uid] = e;
                        Log("restored " + entries.Count + " sign-ups of session " + sid + " (" + s.Def.Name + " " + start.ToString("HH:mm") + ")");
                        continue;
                    }
                    foreach (var e in entries)
                        Refund(e, currency, champ, "Server restart: the poker tournament (" + sid + ") was cancelled, your entry fee is refunded.", true);
                    Log("session " + sid + " (" + state + ", start " + start + ") refunded after a restart: " + entries.Count + " players");
                    Security.Guard.Audit("texas-champ", 0, "", "restart refund session " + sid + " state " + state + " players " + entries.Count);
                    File.Move(file, Path.Combine(Path.Combine(dataPath, "results"), Path.GetFileNameWithoutExtension(file) + "-refunded-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt"));
                }
                catch (Exception e)
                {
                    Log("restore " + file + " failed: " + e.Message);
                }
            }
        }

        // ------------------------------------------------------------------ money / items
        static Client.GameClient OnlineClient(uint uid)
        {
            Client.GameClient c;
            if (Database.Server.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && c.FullLoading && PokerTable.Online(c))
                return c;
            return null;
        }

        static long BalanceOf(Client.GameClient c, byte currency)
        {
            return currency == 1 ? (long)c.Player.ConquerPoints : c.Player.Money;
        }

        static bool Take(Client.GameClient c, byte currency, long amount)
        {
            if (amount <= 0)
                return true;
            if (BalanceOf(c, currency) < amount)
                return false;
            long left = amount, chunk = currency == 1 ? 100000000L : 1000000000L;
            while (left > 0)
            {
                long part = Math.Min(left, chunk);
                if (currency == 1) c.Player.ConquerPoints -= (uint)part;
                else c.Player.Money -= part;
                left -= part;
            }
            return true;
        }

        static void GiveDirect(Client.GameClient c, byte currency, long amount)
        {
            long left = amount, chunk = currency == 1 ? 100000000L : 1000000000L;
            while (left > 0)
            {
                long part = Math.Min(left, chunk);
                if (currency == 1) c.Player.ConquerPoints += (uint)part;
                else c.Player.Money += part;
                left -= part;
            }
        }

        /// <summary>Money + items to a player: straight into his silver/CPs and bag when online (bag full -> mail), by mail when offline.</summary>
        static void Deliver(uint uid, string name, byte currency, long money, IList<KeyValuePair<uint, int>> items, string subject, string text)
        {
            var c = OnlineClient(uid);
            string err;
            if (money > 0)
            {
                if (c != null && (currency == 1 ? (long)c.Player.ConquerPoints + money <= int.MaxValue : true))
                    GiveDirect(c, currency, money);
                else
                {
                    long left = money;
                    while (left > 0)
                    {
                        long part = currency == 1 ? Math.Min(left, int.MaxValue) : left;
                        if (!Database.MailService.Send(uid, "PokerTournament", subject, text, currency == 1 ? 0UL : (ulong)part, currency == 1 ? (uint)part : 0u, 0, null, out err))
                        {
                            Log("MAIL FAILED " + uid + " " + name + " " + part + " " + (currency == 1 ? "CPs" : "silver") + ": " + err);
                            Security.Guard.Audit("texas-champ-lost", uid, name, "could not deliver " + part + " currency " + currency + ": " + err);
                        }
                        left -= part;
                    }
                }
            }
            if (items == null)
                return;
            foreach (var it in items)
            {
                Database.ItemType.DBItem db;
                if (!Database.Server.ItemsBase.TryGetValue(it.Key, out db))
                {
                    Log("unknown reward item " + it.Key);
                    continue;
                }
                int count = it.Value;
                bool stack = db.StackSize > 1;
                if (c != null)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (stack && count <= db.StackSize && c.Inventory.HaveSpace(1) && c.Inventory.AddItemWitchStack(it.Key, 0, (ushort)count, stream))
                            continue;
                        if (!stack && count <= 40 && c.Inventory.HaveSpace((byte)count) && c.Inventory.Add(stream, it.Key, (byte)count))
                            continue;
                    }
                }
                // offline or no room: mail (stackable = one mail with the stack, otherwise one mail per item)
                if (stack)
                {
                    while (count > 0)
                    {
                        int n = Math.Min(count, Math.Max(1, (int)db.StackSize));
                        var item = new Game.MsgServer.MsgGameItem
                        {
                            UID = Database.Server.ITEM_Counter.Next,
                            ITEM_ID = it.Key,
                            Durability = db.Durability,
                            MaximDurability = db.Durability,
                            StackSize = (ushort)n
                        };
                        if (!Database.MailService.Send(uid, "PokerTournament", subject, text, 0, 0, 0, item, out err))
                            Log("MAIL FAILED item " + it.Key + " x" + n + " to " + uid + ": " + err);
                        count -= n;
                    }
                }
                else
                {
                    for (int i = 0; i < Math.Min(count, 20); i++)
                        if (!Database.MailService.Send(uid, "PokerTournament", subject, text, 0, 0, it.Key, null, out err))
                            Log("MAIL FAILED item " + it.Key + " to " + uid + ": " + err);
                }
            }
        }

        static void Refund(ChampEntry e, byte currency, uint champ, string why, bool forceMail)
        {
            long money = e.ExtraMoney + (e.PaidWith == 0 ? e.PaidAmount : 0);
            var items = new List<KeyValuePair<uint, int>>();
            if (e.PaidWith == 1 && e.PaidItem != 0)
                items.Add(new KeyValuePair<uint, int>(e.PaidItem, 1));
            foreach (var x in e.ExtraItems)
                items.Add(new KeyValuePair<uint, int>(x, 1));
            if (money <= 0 && items.Count == 0)
                return;
            if (forceMail)
            {
                // at start-up nobody is online: Deliver mails
            }
            Deliver(e.Uid, e.Name, currency, money, items, "Poker refund", why);
            Log("refund " + e.Uid + " " + e.Name + ": " + money + " " + (currency == 1 ? "CPs" : "silver") + (items.Count > 0 ? " + items " + string.Join(",", items.Select(i => i.Key)) : "") + " (" + why + ")");
            var c = OnlineClient(e.Uid);
            if (c != null)
                c.SendSysMesage(why);
        }

        // ------------------------------------------------------------------ packets
        static void Act(Client.GameClient c, uint act, params ulong[] args)
        {
            if (c == null)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
                c.Send(rec.GetStream().TexasExActionCreate(act, args));
        }

        static void SendRaw(Client.GameClient c, ushort type, List<byte> body)
        {
            if (c == null)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
                c.Send(rec.GetStream().ProtoCreate(type, body));
        }

        /// <summary>3400 act 16 [BB, ante, rebuyOK, addonOK, myRank] + act 15 (own chips).</summary>
        static void SendLevel(ChampSession s, ChampEntry e, Client.GameClient c)
        {
            if (c == null)
                return;
            var lv = s.CurrentLevel;
            long mine = s.Balance(e.Uid);
            bool rebuy = RebuyAllowed(s, e, false), addon = RebuyAllowed(s, e, true);
            Act(c, 16, (ulong)lv.BigBlind, (ulong)lv.Ante, rebuy ? 1UL : 0UL, addon ? 1UL : 0UL, (ulong)LiveRank(s, e));
            using (var rec = new ServerSockets.RecycledPacket())
                c.Send(rec.GetStream().TexasExValueCreate(15, mine));
        }

        static int LiveRank(ChampSession s, ChampEntry e)
        {
            if (!e.Alive)
                return e.Rank;
            var st = s.Standings();
            int i = st.IndexOf(e);
            return i < 0 ? 0 : i + 1;
        }

        static bool RebuyAllowed(ChampSession s, ChampEntry e, bool addon)
        {
            if (s.State != ChampState.Running || !e.Alive || s.Level < 0)
                return false;
            var lv = s.CurrentLevel;
            if (addon)
                return lv.AddonAllowed && s.Def.AddonCount > 0 && e.Addons < s.Def.AddonCount && s.Def.AddonChips > 0;
            return lv.RebuyAllowed && s.Def.RebuyCount > 0 && e.Rebuys < s.Def.RebuyCount && s.Def.RebuyChips > 0 && s.Balance(e.Uid) <= s.Def.StartChips;
        }

        /// <summary>3402 Today list.</summary>
        static void SendList(Client.GameClient c, uint page)
        {
            List<ChampSession> list;
            lock (Sync)
                list = sessions.Values.Where(x => x.StartAt.Date == DateTime.Now.Date || x.State == ChampState.Running).OrderBy(x => x.StartAt).ThenBy(x => x.SessionId).ToList();
            var b = new List<byte>();
            PokerPackets.Proto.Field(b, 1, page);
            PokerPackets.Proto.Field(b, 2, (ulong)list.Count);
            int skip = (int)(Math.Max(1, page) - 1) * 3;
            uint uid = c.Player.UID;
            foreach (var s in list.Skip(skip).Take(3))
            {
                var e = new List<byte>();
                bool signed, full;
                int count;
                lock (Sync)
                {
                    signed = s.Entries.ContainsKey(uid);
                    count = s.State == ChampState.Running || s.State == ChampState.Finished ? Math.Max(s.Participants, s.Entries.Count) : s.Entries.Count;
                    full = s.Entries.Count >= s.Def.MaxPlayers;
                }
                PokerPackets.Proto.Field(e, 1, s.SessionId);
                PokerPackets.Proto.Field(e, 2, s.Def.Id);
                PokerPackets.Proto.Field(e, 3, (ulong)count);
                PokerPackets.Proto.Field(e, 4, signed ? 1UL : 0UL);
                PokerPackets.Proto.Field(e, 5, (ulong)s.HHMM);
                PokerPackets.Proto.Field(e, 6, s.State != ChampState.Open || full ? 2UL : 0UL);
                if (s.State == ChampState.Finished || s.State == ChampState.Cancelled)
                    PokerPackets.Proto.Field(e, 7, 1);// presence = game over
                PokerPackets.Proto.Message(b, 3, e);
            }
            SendRaw(c, PokerPackets.MsgTexasExChampionshipList, b);
        }

        // ------------------------------------------------------------------ client actions (3400)
        /// <summary>Returns true when the action belongs to the championships.</summary>
        public static bool OnAction(Client.GameClient client, ulong action, List<ulong> args)
        {
            switch (action)
            {
                case 2:
                    SendList(client, args.Count > 0 ? (uint)args[0] : 1);
                    return true;
                case 3:
                    lock (Sync)
                        SignUp(client, Arg(args, 0), Arg(args, 1), false);
                    return true;
                case 4:
                case 22:
                    lock (Sync)
                        Enter(client, Arg(args, 0), Arg(args, 1));
                    return true;
                case 5:
                    lock (Sync)
                        Rebuy(client, Arg(args, 1), Arg(args, 2) != 0);
                    return true;
                case 9:
                    lock (Sync)
                        SendRanking(client, Arg(args, 0), Arg(args, 2), Arg(args, 3));
                    return true;
                case 14:
                    lock (Sync)
                        SendMyRank(client, Arg(args, 2));
                    return true;
                case 23:
                    PokerManager.BoardLeaveRequests.Enqueue(client.Player.UID);
                    return true;
                case 12:
                    lock (Sync)
                    {
                        var ids = sessions.Values.Where(s => (s.State == ChampState.Open || s.State == ChampState.Running) && s.Entries.ContainsKey(client.Player.UID)
                            && (s.State == ChampState.Open || s.Entries[client.Player.UID].Alive)).Select(s => (ulong)s.Def.Id).Distinct().ToArray();
                        if (ids.Length > 0)
                            Act(client, 12, ids);
                    }
                    return true;
            }
            return false;
        }

        static uint Arg(List<ulong> a, int i) { return i < a.Count ? (uint)a[i] : 0; }

        static ChampSession Find(uint sessionId, uint champId)
        {
            ChampSession s;
            if (sessions.TryGetValue(sessionId, out s) && (champId == 0 || s.Def.Id == champId))
                return s;
            return null;
        }

        static ChampSession ActiveSessionOf(uint uid)
        {
            foreach (var s in sessions.Values)
            {
                ChampEntry e;
                if (s.Entries.TryGetValue(uid, out e) && (s.State == ChampState.Open || (s.State == ChampState.Running && e.Alive)))
                    return s;
            }
            return null;
        }

        static bool SignUp(Client.GameClient c, uint sessionId, uint champId, bool quiet)
        {
            if (!Enabled)
            {
                c.SendSysMesage("Poker tournaments are not available now.");
                return false;
            }
            var s = Find(sessionId, champId);
            uint uid = c.Player.UID;
            if (s == null || s.State != ChampState.Open || DateTime.Now >= s.StartAt)
            {
                c.SendSysMesage("Sign-up for this tournament is closed.");
                return false;
            }
            if (s.Entries.ContainsKey(uid))
                return true;
            var other = ActiveSessionOf(uid);
            if (other != null)
            {
                c.SendSysMesage("You already signed up for " + other.Def.Name + " at " + other.StartAt.ToString("HH:mm") + ".");
                return false;
            }
            if (s.Entries.Count >= s.Def.MaxPlayers)
            {
                c.SendSysMesage("This tournament is full.");
                return false;
            }
            var e = new ChampEntry { Uid = uid, Name = c.Player.Name ?? "" };
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (s.Fee <= 0)
                    e.PaidWith = 2;
                else if (s.Def.Ticket != 0 && (c.Inventory.Contain(s.Def.Ticket, 1) || c.Inventory.Contain(s.Def.Ticket, 1, 1)) && c.Inventory.Remove(s.Def.Ticket, 1, stream))
                {
                    e.PaidWith = 1;
                    e.PaidItem = s.Def.Ticket;
                }
                else if (Take(c, s.Def.Currency, s.Fee))
                {
                    e.PaidWith = 0;
                    e.PaidAmount = s.Fee;
                }
                else
                {
                    c.SendSysMesage("You need " + s.Fee.ToString("N0") + " " + s.Def.CurrencyName + " or a " + ItemName(s.Def.Ticket) + " to sign up.");
                    return false;
                }
            }
            s.Entries[uid] = e;
            Persist(s);
            Log("sign-up " + uid + " " + e.Name + " session " + s.SessionId + " paid " + (e.PaidWith == 1 ? "ticket " + e.PaidItem : e.PaidAmount + " " + s.Def.CurrencyName));
            Act(c, 3, s.SessionId, s.Def.Id);
            if (!quiet)
                c.SendSysMesage("You`ve successfully signed up for " + s.Def.Name + "!");
            return true;
        }

        static string ItemName(uint id)
        {
            Database.ItemType.DBItem db;
            return id != 0 && Database.Server.ItemsBase.TryGetValue(id, out db) ? db.Name : "ticket";
        }

        static void Enter(Client.GameClient c, uint sessionId, uint champId)
        {
            var s = Find(sessionId, champId);
            uint uid = c.Player.UID;
            if (s == null)
                return;
            ChampEntry e;
            if (s.State == ChampState.Running)
            {
                if (s.Entries.TryGetValue(uid, out e) && e.Alive)
                    ShowBoard(s, e, c, true);
                else
                    c.SendSysMesage("The tournament is underway.");
                return;
            }
            if (s.State != ChampState.Open)
                return;
            if (!s.Entries.TryGetValue(uid, out e))
            {
                // countdown, not signed: sign up and enter
                if (!SignUp(c, sessionId, champId, false))
                    return;
                e = s.Entries[uid];
            }
            e.Ready = true;
            int left = (int)Math.Max(0, (s.StartAt - DateTime.Now).TotalSeconds);
            if (EarlyBoard)
            {
                EnterBoardMode(c);
                Act(c, 4, 0, s.Def.Id, (ulong)(left + StartWaitSeconds));
                e.InBoard = true;
            }
            else
                c.SendSysMesage("The tournament starts in " + left + " sec. You will be seated automatically.");
        }

        static void EnterBoardMode(Client.GameClient c)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                c.Send(stream.MsgInterServerIdentifier(0, c.Player.UID, c.Player.UID));
                c.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.ServerName, false, ServerName);
            }
        }

        static void LeaveBoardMode(Client.GameClient c)
        {
            using (var rec = new ServerSockets.RecycledPacket())
                c.Send(rec.GetStream().MsgInterServerIdentifier(1, c.Player.UID, c.Player.UID));
        }

        /// <summary>Opens the board for a seated player: 2501 mode 0, act 4 [room, champ, wait], act 16/15, then the table (2090s, looks, 3406).</summary>
        static void ShowBoard(ChampSession s, ChampEntry e, Client.GameClient c, bool rebind)
        {
            var t = s.Tables.FirstOrDefault(x => x.RoomId == e.Room);
            if (t == null || c == null)
                return;
            int wait = (int)Math.Max(0, Math.Ceiling((s.PlayStart - DateTime.Now).TotalSeconds));
            EnterBoardMode(c);
            Act(c, 4, t.RoomId, s.Def.Id, (ulong)wait);
            SendLevel(s, e, c);
            lock (t.SyncRoot)
            {
                if (rebind)
                    t.BoardRebind(e.Uid, c);
                t.SendBoardView(c);
            }
            PokerManager.SetBoardPlayer(e.Uid, t);
            e.InBoard = true;
        }

        static void Rebuy(Client.GameClient c, uint champId, bool addon)
        {
            uint uid = c.Player.UID;
            ChampSession s = sessions.Values.FirstOrDefault(x => x.State == ChampState.Running && x.Entries.ContainsKey(uid) && x.Entries[uid].Alive && (champId == 0 || x.Def.Id == champId));
            ChampEntry e = s != null ? s.Entries[uid] : null;
            bool ok = false;
            if (s != null && RebuyAllowed(s, e, addon))
            {
                uint ticket = addon ? s.Def.AddonTicket : s.Def.RebuyTicket;
                long price = addon ? s.Def.AddonPrice : s.Def.RebuyPrice;
                long chips = addon ? s.Def.AddonChips : s.Def.RebuyChips;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (ticket != 0 && (c.Inventory.Contain(ticket, 1) || c.Inventory.Contain(ticket, 1, 1)) && c.Inventory.Remove(ticket, 1, stream))
                    {
                        e.ExtraItems.Add(ticket);
                        ok = true;
                    }
                    else if (Take(c, s.Def.Currency, price))
                    {
                        e.ExtraMoney += price;
                        s.ExtraPool += price;
                        ok = true;
                    }
                }
                if (ok)
                {
                    if (addon) e.Addons++; else e.Rebuys++;
                    s.Credit(uid, chips);
                    s.ChipsIssued += chips;
                    Persist(s);
                    Log((addon ? "add-on " : "rebuy ") + uid + " " + e.Name + " +" + chips + " chips, session " + s.SessionId);
                }
                else
                    c.SendSysMesage("You need " + price.ToString("N0") + " " + s.Def.CurrencyName + " or a " + ItemName(ticket) + ".");
            }
            long money = c.Player.Money;
            Act(c, 5, e != null ? e.Room : 0, 0, 0, ok ? 0UL : 1UL, (ulong)(money & 0xffffffff), (ulong)((money >> 32) & 0xffffffff));
            if (s != null && e != null)
            {
                SendLevel(s, e, c);
                var t = s.Tables.FirstOrDefault(x => x.RoomId == e.Room);
                if (t != null && ok)
                    lock (t.SyncRoot)
                        if (!t.HandRunning)
                            using (var rec = new ServerSockets.RecycledPacket())
                                t.SendChips(rec.GetStream());
            }
        }

        /// <summary>3404 (EventName "Ranking" tab): names in rank order.</summary>
        static void SendRanking(Client.GameClient c, uint sessionId, uint page, uint per)
        {
            var s = Find(sessionId, 0);
            if (per == 0 || per > 20)
                per = 10;
            if (page == 0)
                page = 1;
            var names = s == null ? new List<string>() : (s.State == ChampState.Open ? s.Entries.Values.Select(e => e.Name).ToList() : s.Standings().Select(e => e.Name).ToList());
            var b = new List<byte>();
            PokerPackets.Proto.Field(b, 1, page);
            PokerPackets.Proto.Field(b, 2, (ulong)names.Count);
            foreach (var n in names.Skip((int)((page - 1) * per)).Take((int)per))
            {
                var e = new List<byte>();
                PokerPackets.Proto.String(e, 1, ServerName);
                PokerPackets.Proto.String(e, 2, n);
                PokerPackets.Proto.Message(b, 3, e);
            }
            SendRaw(c, PokerPackets.MsgTexasExRankList, b);
        }

        /// <summary>3405 (ranking inside the board): my rank/chips + 7 rows per page.</summary>
        static void SendMyRank(Client.GameClient c, uint page)
        {
            uint uid = c.Player.UID;
            var s = sessions.Values.FirstOrDefault(x => x.State == ChampState.Running && x.Entries.ContainsKey(uid));
            if (page == 0)
                page = 1;
            var b = new List<byte>();
            var rows = s == null ? new List<ChampEntry>() : s.Standings().Where(e => e.Alive).ToList();
            ChampEntry me = s != null ? s.Entries[uid] : null;
            PokerPackets.Proto.Field(b, 1, me != null ? (ulong)LiveRank(s, me) : 0UL);
            PokerPackets.Proto.Int64(b, 2, me != null ? s.Balance(uid) : 0);
            PokerPackets.Proto.Field(b, 3, page);
            PokerPackets.Proto.Field(b, 4, (ulong)rows.Count);
            foreach (var r in rows.Skip((int)(page - 1) * 7).Take(7))
            {
                var e = new List<byte>();
                PokerPackets.Proto.String(e, 1, ServerName);
                PokerPackets.Proto.String(e, 2, r.Name.Replace(' ', '_'));
                PokerPackets.Proto.Int64(e, 3, s.Balance(r.Uid));
                PokerPackets.Proto.Message(b, 5, e);
            }
            SendRaw(c, PokerPackets.MsgTexasExMyRank, b);
        }

        // ------------------------------------------------------------------ login / logout
        public static void OnLogin(Client.GameClient client)
        {
            if (client != null && client.Player != null)
                relogins.Enqueue(new KeyValuePair<uint, DateTime>(client.Player.UID, DateTime.Now.AddSeconds(3)));
        }

        public static void OnLogout(Client.GameClient client)
        {
            if (client == null || client.Player == null)
                return;
            lock (Sync)
            {
                foreach (var s in sessions.Values)
                {
                    ChampEntry e;
                    if (s.State == ChampState.Running && s.Entries.TryGetValue(client.Player.UID, out e))
                        e.InBoard = false;// he stays seated on auto play; the table ticker switches it on
                }
            }
        }

        // ------------------------------------------------------------------ timer
        static void OnTimer(object state)
        {
            if (System.Threading.Interlocked.Exchange(ref busy, 1) == 1)
                return;
            try
            {
                lock (Sync)
                    Tick(DateTime.Now);
            }
            catch (Exception e)
            {
                Log("tick: " + e);
                MyConsole.WriteException(e);
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref busy, 0);
            }
        }

        static void Tick(DateTime now)
        {
            if (now.Date != builtFor)
                BuildToday(now);
            // leave requests (2096 from the board / 3400 act 23)
            uint leaver;
            while (PokerManager.BoardLeaveRequests.TryDequeue(out leaver))
            {
                foreach (var s in sessions.Values.Where(x => x.State == ChampState.Running).ToList())
                {
                    ChampEntry e;
                    if (s.Entries.TryGetValue(leaver, out e) && e.Alive)
                    {
                        Log("leave " + leaver + " " + e.Name + " session " + s.SessionId);
                        Eliminate(s, e, s.AliveCount, "You left the tournament.");
                        if (s.AliveCount <= 1)
                            Finish(s);
                    }
                }
            }
            // players who logged in again
            int n = relogins.Count;
            while (n-- > 0)
            {
                KeyValuePair<uint, DateTime> r;
                if (!relogins.TryDequeue(out r))
                    break;
                if (r.Value > now)
                {
                    relogins.Enqueue(r);
                    continue;
                }
                var c = OnlineClient(r.Key);
                if (c == null)
                    continue;
                foreach (var s in sessions.Values.Where(x => x.State == ChampState.Running))
                {
                    ChampEntry e;
                    if (s.Entries.TryGetValue(r.Key, out e) && e.Alive)
                    {
                        Log("re-login " + r.Key + " " + e.Name + " -> back to the board, session " + s.SessionId);
                        ShowBoard(s, e, c, true);
                    }
                }
            }
            foreach (var s in sessions.Values.ToList())
            {
                try
                {
                    TickSession(s, now);
                }
                catch (Exception e)
                {
                    Log("session " + s.SessionId + ": " + e);
                }
            }
        }

        static void TickSession(ChampSession s, DateTime now)
        {
            switch (s.State)
            {
                case ChampState.Open:
                    {
                        double left = (s.StartAt - now).TotalSeconds;
                        if (!s.Pushed && left <= PushSeconds && left > 0 && s.Entries.Count > 0)
                        {
                            s.Pushed = true;
                            foreach (var e in s.Entries.Values)
                                Act(OnlineClient(e.Uid), 22, s.SessionId, s.Def.Id, (ulong)left, (ulong)s.Entries.Count, 0);
                        }
                        if (!s.ReadySent && left <= ReadySeconds && left > 0 && s.Entries.Count > 0)
                        {
                            s.ReadySent = true;
                            foreach (var e in s.Entries.Values.Where(x => !x.InBoard))
                                Act(OnlineClient(e.Uid), 8, s.SessionId, s.Def.Id, (ulong)left);
                        }
                        if (left <= 0)
                            Start(s, now);
                        break;
                    }
                case ChampState.Running:
                    TickRunning(s, now);
                    break;
            }
        }

        // ------------------------------------------------------------------ start
        static void Start(ChampSession s, DateTime now)
        {
            if (s.Entries.Count < s.MinPlayers || s.Def.StartChips <= 0)
            {
                s.State = ChampState.Cancelled;
                Log("session " + s.SessionId + " " + s.Def.Name + " cancelled: " + s.Entries.Count + " < " + s.MinPlayers + " players");
                foreach (var e in s.Entries.Values)
                {
                    Refund(e, s.Def.Currency, s.Def.Id, "The tournament has been cancelled due to insufficient number of participants. You will get a full refund of the entry fee.", false);
                    var c = OnlineClient(e.Uid);
                    if (c != null && e.InBoard)
                        LeaveBoardMode(c);
                }
                Persist(s);
                return;
            }
            s.State = ChampState.Running;
            s.Participants = s.Entries.Count;
            s.PlayStart = now.AddSeconds(StartWaitSeconds);
            s.Level = 0;
            var lv = s.CurrentLevel;
            var players = s.Entries.Values.OrderBy(x => Guid.NewGuid()).ToList();
            var sizes = PokerTournament.InitialTableSizes(players.Count, PokerTable.MaxSeatsLimit);
            int k = 0;
            foreach (var size in sizes)
            {
                var t = new PokerTable
                {
                    UID = 0,
                    TableId = s.Def.Id,// 2090 +9 / 2096 +8 = championship id in board mode
                    NoLimit = s.Def.LimitType == 1,
                    MoneyType = s.Def.Currency,
                    BigBlind = lv.BigBlind,
                    Ante = lv.Ante,
                    Bank = s,
                    Owner = s,
                    Hold = true,
                    RoomId = nextRoom++
                };
                s.Tables.Add(t);
                PokerManager.RegisterBoard(t);
                lock (t.SyncRoot)
                {
                    for (int i = 0; i < size; i++)
                    {
                        var e = players[k++];
                        s.Credit(e.Uid, s.Def.StartChips);
                        s.ChipsIssued += s.Def.StartChips;
                        e.Room = t.RoomId;
                        Client.GameClient c = OnlineClient(e.Uid);
                        t.BoardSeat(c ?? StaleClient(e.Uid), e.Uid, i);
                    }
                }
            }
            foreach (var t in s.Tables)
                foreach (var e in s.Entries.Values.Where(x => x.Room == t.RoomId))
                    PokerManager.SetBoardPlayer(e.Uid, t);
            Persist(s);
            Log("session " + s.SessionId + " " + s.Def.Name + " started: " + s.Participants + " players at " + s.Tables.Count + " tables, fee " + s.Fee + " " + s.Def.CurrencyName);
            foreach (var e in s.Entries.Values)
            {
                var c = OnlineClient(e.Uid);
                if (c != null && (AutoEnterOnline || e.Ready || e.InBoard))
                    ShowBoard(s, e, c, false);
                else
                {
                    var t = s.Tables.First(x => x.RoomId == e.Room);
                    lock (t.SyncRoot)
                        t.BoardSetAuto(e.Uid, true);
                }
            }
        }

        /// <summary>A seat needs a client object even for an offline player (his Player gives the name/look): a placeholder without socket.</summary>
        static Client.GameClient StaleClient(uint uid)
        {
            Client.GameClient c;
            if (Database.Server.GamePoll.TryGetValue(uid, out c))
                return c;
            return null;
        }

        // ------------------------------------------------------------------ running
        static void TickRunning(ChampSession s, DateTime now)
        {
            // blind level
            if (now >= s.PlayStart)
            {
                int ls = s.LevelSecondsOverride > 0 ? s.LevelSecondsOverride : LevelSecondsAll;
                int level = PokerTournament.LevelAt(s.Def.Levels, (now - s.PlayStart).TotalSeconds, ls);
                if (level != s.Level)
                {
                    s.Level = level;
                    Log("session " + s.SessionId + " level " + (level + 1) + ": BB " + s.CurrentLevel.BigBlind + " ante " + s.CurrentLevel.Ante);
                    foreach (var e in s.Entries.Values.Where(x => x.Alive && x.InBoard))
                        SendLevel(s, e, OnlineClient(e.Uid));
                }
                if (!s.Released)
                {
                    s.Released = true;
                    foreach (var t in s.Tables)
                        lock (t.SyncRoot)
                            ReleaseTable(s, t);
                }
            }
            // hands that ended
            PokerTable done;
            var handled = new HashSet<PokerTable>();
            while (s.HandsDone.TryDequeue(out done))
            {
                if (!s.Tables.Contains(done) || !handled.Add(done))
                    continue;
                AfterHand(s, done);
                if (s.State != ChampState.Running)
                    return;
            }
            // idle tables (not enough players to start a hand) may have to be broken / filled
            foreach (var t in s.Tables.ToList())
            {
                bool idle;
                lock (t.SyncRoot)
                    idle = !t.Hold && t.IsIdle && !t.HandRunning;
                if (idle && now >= s.PlayStart)
                    MoveBetweenTables(s, t);
            }
            if (s.State == ChampState.Running && s.AliveCount <= 1)
                Finish(s);
        }

        static void ReleaseTable(ChampSession s, PokerTable t)
        {
            var lv = s.CurrentLevel;
            t.BigBlind = Math.Max(2, lv.BigBlind);
            t.Ante = lv.Ante;
            t.Release();
        }

        /// <summary>Between two hands of table t: eliminations, end of the tournament, moves between tables, blinds, next hand.</summary>
        static void AfterHand(ChampSession s, PokerTable t)
        {
            var busted = new List<KeyValuePair<uint, long>>();
            lock (t.SyncRoot)
            {
                foreach (var seat in t.SeatedList())
                {
                    ChampEntry e;
                    if (s.Entries.TryGetValue(seat.UID, out e) && e.Alive && s.Balance(seat.UID) <= 0)
                    {
                        long start;
                        t.LastHandStartStacks.TryGetValue(seat.UID, out start);
                        busted.Add(new KeyValuePair<uint, long>(seat.UID, start));
                    }
                }
            }
            if (busted.Count > 0)
            {
                foreach (var r in PokerTournament.RankBusted(busted, s.AliveCount))
                    Eliminate(s, s.Entries[r.Key], r.Value, null);
            }
            if (s.AliveCount <= 1)
            {
                Finish(s);
                return;
            }
            MoveBetweenTables(s, t);
            if (s.Tables.Contains(t))
            {
                lock (t.SyncRoot)
                    ReleaseTable(s, t);
            }
            // ranks changed: refresh "Current Rank" / own chips of everybody still playing
            foreach (var e in s.Entries.Values.Where(x => x.Alive && x.InBoard && x.Room == t.RoomId))
                SendLevel(s, e, OnlineClient(e.Uid));
        }

        static void MoveBetweenTables(ChampSession s, PokerTable t)
        {
            if (!s.Tables.Contains(t))
                return;
            var counts = new Dictionary<int, int>();
            foreach (var x in s.Tables)
                lock (x.SyncRoot)
                    counts[(int)x.RoomId] = x.SeatedCount;
            if (counts[(int)t.RoomId] == 0)
            {
                CloseTable(s, t);
                return;
            }
            bool close;
            var moves = PokerTournament.PlanMoves(counts, (int)t.RoomId, PokerTable.MaxSeatsLimit, out close);
            if (moves.Count == 0)
                return;
            List<PokerSeatInfo> movers;
            lock (t.SyncRoot)
            {
                // between hands only
                if (t.HandRunning)
                    return;
                var seated = t.SeatedList();
                movers = close ? seated : seated.OrderBy(x => Guid.NewGuid()).Take(moves.Count).ToList();
            }
            for (int i = 0; i < movers.Count && i < moves.Count; i++)
            {
                var dest = s.Tables.First(x => x.RoomId == (uint)moves[i]);
                MovePlayer(s, s.Entries[movers[i].UID], t, dest);
            }
            if (close)
                CloseTable(s, t);
        }

        static void MovePlayer(ChampSession s, ChampEntry e, PokerTable from, PokerTable to)
        {
            Client.GameClient seatClient;
            lock (from.SyncRoot)
            {
                var seat = from.SeatOf(e.Uid);
                seatClient = seat != null ? seat.Client : null;
                from.BoardRemove(e.Uid);
            }
            var c = OnlineClient(e.Uid);
            bool ok;
            lock (to.SyncRoot)
                ok = to.BoardSeat(c ?? seatClient, e.Uid, to.FreeSeat());
            if (!ok)
            {
                Log("MOVE FAILED " + e.Uid + " room " + from.RoomId + " -> " + to.RoomId + ": eliminated with his chips kept for the check");
                return;
            }
            e.Room = to.RoomId;
            PokerManager.SetBoardPlayer(e.Uid, to);
            Log("move " + e.Uid + " " + e.Name + " room " + from.RoomId + " -> " + to.RoomId);
            if (c != null && e.InBoard)
            {
                if (MoveMode == 4)
                {
                    Act(c, 4, to.RoomId, s.Def.Id, 0);
                }
                else
                    Act(c, 19, to.RoomId);
                SendLevel(s, e, c);
                lock (to.SyncRoot)
                    to.SendBoardView(c);
            }
            else if (c == null)
            {
                lock (to.SyncRoot)
                    to.BoardSetAuto(e.Uid, true);
            }
        }

        static void CloseTable(ChampSession s, PokerTable t)
        {
            s.Tables.Remove(t);
            PokerManager.UnregisterBoard(t);
            Log("session " + s.SessionId + " table " + t.RoomId + " closed, " + s.Tables.Count + " left");
        }

        /// <summary>Takes the player out: 2096 to his table, 2501 mode 1 (board closes), act 18 result popup.</summary>
        static void Eliminate(ChampSession s, ChampEntry e, int rank, string message)
        {
            var t = s.Tables.FirstOrDefault(x => x.RoomId == e.Room);
            if (t != null)
                lock (t.SyncRoot)
                    t.BoardRemove(e.Uid);
            PokerManager.SetBoardPlayer(e.Uid, null);
            long left = s.Take(e.Uid);
            if (left > 0)
                s.ChipsForfeited += left;
            e.Alive = false;
            e.Rank = rank;
            e.FinalChips = left;
            Log("out " + e.Uid + " " + e.Name + " rank " + rank + (left > 0 ? " (left with " + left + " chips, forfeited)" : ""));
            var c = OnlineClient(e.Uid);
            if (c != null)
            {
                if (e.InBoard)
                    LeaveBoardMode(c);
                Act(c, 18, (ulong)rank, (ulong)rank, s.Def.Id);
                c.SendSysMesage(message ?? ("You finished No." + rank + " in " + s.Def.Name + "."));
            }
            e.InBoard = false;
        }

        // ------------------------------------------------------------------ end: ranks and prizes
        static void Finish(ChampSession s)
        {
            if (s.State != ChampState.Running)
                return;
            var winner = s.Entries.Values.FirstOrDefault(e => e.Alive);
            long winnerChips = winner != null ? s.Balance(winner.Uid) : 0;
            if (winner != null)
                Eliminate(s, winner, 1, "Congratulations! You won " + s.Def.Name + "!");
            foreach (var t in s.Tables.ToList())
                CloseTable(s, t);
            s.State = ChampState.Finished;
            // chip check: every chip issued is with the winner or was forfeited by a player who left
            long accounted = winnerChips + s.ChipsForfeited;
            bool chipsOk = accounted == s.ChipsIssued;
            Log("session " + s.SessionId + " finished: winner " + (winner != null ? winner.Name : "-") + ", chips issued " + s.ChipsIssued + ", winner " + winnerChips
                + " + forfeited " + s.ChipsForfeited + (chipsOk ? " OK" : " MISMATCH"));
            if (!chipsOk)
                Security.Guard.Audit("texas-champ", 0, "", "chip mismatch session " + s.SessionId + ": issued " + s.ChipsIssued + " accounted " + accounted);
            PayPrizes(s);
            Persist(s);
        }

        static void PayPrizes(ChampSession s)
        {
            var rows = s.Def.Bracket(s.Participants);
            long pool = PokerTournament.Pool(s.Fee, s.Participants, PoolIncludesRebuys ? s.ExtraPool : 0, rows);
            var sb = new StringBuilder();
            sb.AppendLine("session " + s.SessionId + " " + s.Def.Name + " " + s.StartAt.ToString("yyyy-MM-dd HH:mm") + (s.Test ? " TEST" : ""));
            sb.AppendLine("participants " + s.Participants + ", fee " + s.Fee + " " + s.Def.CurrencyName + ", rebuy/add-on money " + s.ExtraPool + ", pool " + pool + ", chips issued " + s.ChipsIssued);
            long paidMoney = 0;
            foreach (var e in s.Entries.Values.OrderBy(x => x.Rank))
            {
                long money = PokerTournament.PrizeMoney(rows, e.Rank, pool);
                var items = new List<KeyValuePair<uint, int>>();
                long silver = 0, cps = 0;
                var unknown = new List<string>();
                if (e.Rank >= 1 && e.Rank <= rows.Count)
                    ChampData.ParseReward(rows[e.Rank - 1].Text, itemNames, items, out silver, out cps, unknown);
                foreach (var u in unknown)
                    Log("reward name '" + u + "' has no item id in prize_items.txt");
                sb.AppendLine("  No." + e.Rank + " " + e.Uid + " " + e.Name + ": " + money + " " + s.Def.CurrencyName + (items.Count > 0 ? ", items " + string.Join(",", items.Select(i => i.Key + "x" + i.Value)) : "")
                    + (silver > 0 ? ", silver " + silver : "") + (cps > 0 ? ", CPs " + cps : ""));
                if (money <= 0 && items.Count == 0 && silver <= 0 && cps <= 0)
                    continue;
                string subject = s.Def.Name + " No." + e.Rank;
                string text = "Poker tournament reward (No." + e.Rank + ").";
                Deliver(e.Uid, e.Name, s.Def.Currency, money, items, subject, text);
                if (silver > 0) Deliver(e.Uid, e.Name, 0, silver, null, subject, text);
                if (cps > 0) Deliver(e.Uid, e.Name, 1, cps, null, subject, text);
                paidMoney += money;
                if (money > 0)
                    Security.Guard.Audit("texas-champ", e.Uid, e.Name, "prize No." + e.Rank + " " + money + " " + s.Def.CurrencyName + " session " + s.SessionId);
            }
            sb.AppendLine("money paid " + paidMoney);
            try { File.WriteAllText(Path.Combine(Path.Combine(dataPath, "results"), s.SessionId + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt"), sb.ToString(), Encoding.UTF8); }
            catch (Exception ex) { Log("result file: " + ex.Message); }
            Log(sb.ToString().Replace(Environment.NewLine, " | "));
        }

        // ------------------------------------------------------------------ GM
        public static string Status()
        {
            lock (Sync)
            {
                var sb = new StringBuilder();
                sb.Append(Enabled ? "on" : "OFF").Append(", ").Append(sessions.Count).Append(" sessions today");
                foreach (var s in sessions.Values.Where(x => x.State == ChampState.Running || x.Entries.Count > 0 || x.Test).OrderBy(x => x.StartAt))
                    sb.Append(" | ").Append(s.SessionId).Append(' ').Append(s.Def.Id).Append(' ').Append(s.StartAt.ToString("HH:mm:ss")).Append(' ').Append(s.State)
                        .Append(" signed ").Append(s.Entries.Count).Append(" alive ").Append(s.AliveCount).Append(" tables ").Append(s.Tables.Count)
                        .Append(" lvl ").Append(s.Level + 1);
                return sb.ToString();
            }
        }

        public static string GmTest(uint champ, int min, int startSec, int levelSec, long fee)
        {
            lock (Sync)
            {
                string err;
                var s = CreateTest(champ, min, startSec, levelSec, fee, out err);
                if (s == null)
                    return err;
                return "test session " + s.SessionId + " (" + s.Def.Name + ") starts at " + s.StartAt.ToString("HH:mm:ss") + ", min " + s.MinPlayers + " players, fee " + s.Fee + ", level " + (levelSec > 0 ? levelSec + " s" : "table");
            }
        }

        public static string GmCancel(uint sessionId)
        {
            lock (Sync)
            {
                ChampSession s;
                if (!sessions.TryGetValue(sessionId, out s))
                    return "no session " + sessionId;
                if (s.State == ChampState.Running)
                {
                    foreach (var e in s.Entries.Values)
                    {
                        if (e.Alive)
                        {
                            var t = s.Tables.FirstOrDefault(x => x.RoomId == e.Room);
                            if (t != null)
                                lock (t.SyncRoot)
                                    t.BoardRemove(e.Uid);
                            PokerManager.SetBoardPlayer(e.Uid, null);
                            var c = OnlineClient(e.Uid);
                            if (c != null && e.InBoard)
                                LeaveBoardMode(c);
                        }
                        Refund(e, s.Def.Currency, s.Def.Id, "The poker tournament was cancelled by a GM, your entry fee is refunded.", false);
                    }
                    foreach (var t in s.Tables.ToList())
                        CloseTable(s, t);
                }
                else if (s.State == ChampState.Open)
                {
                    foreach (var e in s.Entries.Values)
                        Refund(e, s.Def.Currency, s.Def.Id, "The poker tournament was cancelled by a GM, your entry fee is refunded.", false);
                }
                else
                    return "session " + sessionId + " is " + s.State;
                s.State = ChampState.Cancelled;
                Persist(s);
                Log("GM cancel " + sessionId);
                return "session " + sessionId + " cancelled and refunded";
            }
        }

        public static string GmSignUp(Client.GameClient c, uint sessionId)
        {
            lock (Sync)
            {
                ChampSession s;
                if (!sessions.TryGetValue(sessionId, out s))
                    return "no session " + sessionId;
                return SignUp(c, sessionId, s.Def.Id, false) ? "signed up" : "not signed up";
            }
        }
    }
}
