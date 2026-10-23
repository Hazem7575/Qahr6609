using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.Conquer6609_Work;
using Project2D.Database;

namespace Conquer6609.Database
{
    /// <summary>
    /// Thread-safe unique id counter (Interlocked based, built on Security.SafeCounter) with the same members
    /// as the Extensions.Counter it replaces: Next, Count and Set.
    /// </summary>
    public class AtomicCounter
    {
        Security.SafeCounter inner;
        public AtomicCounter(uint start)
        {
            inner = new Security.SafeCounter(start);
        }
        /// <summary>Highest id this counter may hand out (item UID range of this server, ClusterConfig).
        /// Past it, Next throws instead of giving an id that belongs to another server.</summary>
        public uint Limit = uint.MaxValue;
        int limitAudited;
        /// <summary>Returns the next id (atomic increment).</summary>
        public uint Next
        {
            get
            {
                uint value = System.Threading.Volatile.Read(ref inner).Next;
                if (value > Limit)
                {
                    if (System.Threading.Interlocked.Exchange(ref limitAudited, 1) == 0)
                        Security.Guard.Audit("item-uid-range", 0, "", "id range exhausted: " + value + " > " + Limit + ", new ids refused");
                    throw new InvalidOperationException("id range exhausted (" + value + " > " + Limit + ")");
                }
                return value;
            }
        }
        /// <summary>The last id handed out.</summary>
        public uint Count { get { return System.Threading.Volatile.Read(ref inner).Current; } }
        /// <summary>Resets the counter (startup only).</summary>
        public void Set(uint start)
        {
            System.Threading.Interlocked.Exchange(ref inner, new Security.SafeCounter(start));
        }
        /// <summary>Raises the counter to at least value (never lowers it).</summary>
        public void EnsureAtLeast(long value)
        {
            System.Threading.Volatile.Read(ref inner).EnsureAtLeast(value);
        }
    }

    /// <summary>ConcurrentDictionary that also offers the Dictionary style Add/Remove used by existing code.</summary>
    public class ConcurrentMap<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException("An item with the same key has already been added.");
        }
        public bool Remove(TKey key)
        {
            TValue value;
            return TryRemove(key, out value);
        }
    }

    public class Server
    {
        /// <summary>
        /// Item UIDs: shell.ini [Database] ItemUID holds a high-water mark. Every database save writes
        /// "current counter + ItemUIDReserve", so after a crash the server restarts above every UID that was
        /// handed out since the last save (as long as fewer than ItemUIDReserve items were created in between).
        /// At startup the counter is additionally raised above the highest UID found in PlayersItems\*.bin.
        /// </summary>
        public const uint ItemUIDReserve = 10000;
        public const uint MailUIDReserve = 1000;

        public static ushort AncientBeadX = 196, AncientBeadY = 252;
        public static bool GatesOpened = false;
        ///
        public static List<uint> NoAgateMap = new List<uint>() { };
        public static List<uint> RedeemActivated = new List<uint>();
        public static int DB = 0, Plus3Stone = 0, Parement = 0, ChiPack = 0, ToughDrill = 0, StarDrill = 0;
      
       
        public static DateTime RetailDuration;
        public static byte LordTokenCount = 3;

        public static Extensions.Time32 ResetStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.ResetDayStamp);
        public static ushort[] PriceUpdatePorf = new ushort[] { 36000, 36000, 36000, 36000, 36000, 36000, 18367, 12328, 7377, 6164, 3688, 3082, 3082, 3082, 3082,  2670, 1825, 1251, 866, 704 };
        public static Dictionary<DBLevExp.Sort, Dictionary<byte, DBLevExp>> LevelInfo = new Dictionary<DBLevExp.Sort, Dictionary<byte, DBLevExp>>();
        public static ConcurrentDictionary<uint, TheCrimeTable> TheCrimePoll = new ConcurrentDictionary<uint, TheCrimeTable>();
        public static Database.ActivityTask ActivityTasks = new ActivityTask();

        public static InfoHeroReward TableHeroRewards = new InfoHeroReward();

        public static ConcurrentMap<uint, Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>>> ClientAgates = new ConcurrentMap<uint, Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>>>();
        public static Dictionary<ushort, ushort> WeaponSpells = new Dictionary<ushort, ushort>();
        public static MagicType Magic = new MagicType();

        public static LotteryTable Lottery = new LotteryTable();
        public static SubProfessionInfo SubClassInfo = new SubProfessionInfo();
        public static Dictionary<uint, Game.MsgMonster.MonsterFamily> MonsterFamilies = new Dictionary<uint, Game.MsgMonster.MonsterFamily>();
        public static AtomicCounter ITEM_Counter = new AtomicCounter(1);
        public static AtomicCounter Inbox_Counter = new AtomicCounter(1);
        public static Rifinery RifineryItems;
        public static RefinaryBoxes DBRerinaryBoxes;
        public static ItemType ItemsBase;
        public static Dictionary<uint, Role.GameMap> ServerMaps;
        public static ConcurrentDictionary<uint, Client.GameClient> GamePoll;
        /// <summary>Hash codes of the character names in use. Always access it under lock (NameUsed).</summary>
        public static List<int> NameUsed;
        public static RebornInfomations RebornInfo;

        public static ArenaTable Arena = new ArenaTable();
        public static TeamArenaTable TeamArena = new TeamArenaTable();

        public static AtomicCounter ClientCounter = new AtomicCounter(1000000);
        public static ConfiscatorTable QueueContainer = new ConfiscatorTable();

        public static bool FullLoading = false;

        public static Dictionary<uint, string> MapName = new Dictionary<uint, string>() { { 1015, "BirdIsland" }, { 1011, "PhoenixCastle" }, { 1000, "DesertCity" }, { 1020, "ApeMountain" }, { 1001, "MysticCastle" } };
        public static string DbName = "";
        public static string DbPass = "";
       public static uint ResetServerDay = 0;
        public static unsafe void Reset(Extensions.Time32 Clock)
        {
            if (Clock > ResetStamp)
            {
                if (DateTime.Now.DayOfYear != ResetServerDay)
                {
                    try
                    {
                     

                        Arena.ResetArena();
                        TeamArena.ResetArena();

                        foreach (var flowerclient in Role.Instance.Flowers.ClientPoll.Values)
                        {
                            foreach (var flower in flowerclient)
                                flower.Amount2day = 0;
                        }
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            foreach (var client in GamePoll.Values)
                            {


                                client.Player.TodayChampionPoints = 0;

                                client.Player.ChangeEpicTrojan = client.Player.ChangeArrayEpicTrojan =
                        client.Player.ChangeMr_MirrorEpicTrojan = client.Player.ChangeGeneralPakEpicTrojan = 0;
                                client.Player.CanChangeEpicMaterial = client.Player.CanChangeArrayEpicMaterial =
                                    client.Player.CanChangeMr_MirrorEpicMaterial = client.Player.CanChangGeneralPakMaterial = 1;

                                client.Player.UseChiToken = 0;


                                client.Player.TowerOfMysterychallenge = 3;
                                client.Player.TOMChallengeToday = 0;
                                client.Player.TowerOfMysteryChallengeFlag = 0;
                                client.Player.TOMSelectChallengeToday = 0;
                                client.Player.ClaimTowerAmulets = 0;
                                client.Player.TOMClaimTeamReward = 0;
                                client.Player.TOMRefreshReward = 0;
                                client.Player.QuestGUI.RemoveQuest(6126);




                                client.Player.OpenHousePack = 0;
                                client.MyExchangeShop.Reset();
                                if (client.Player.DailyMonth == 0)
                                    client.Player.DailyMonth = (byte)DateTime.Now.Month;
                                if (client.Player.DailyMonth != DateTime.Now.Month)
                                {
                                    client.Player.DailySignUpRewards = 0;
                                    client.Player.DailySignUpDays = 0;
                                    client.Player.DailyMonth = (byte)DateTime.Now.Month;
                                }
                                if (client.Player.MyJiangHu != null)
                                {
                                    client.Player.MyJiangHu.FreeCourse = 30000;
                                    client.Player.MyJiangHu.FreeTimeToday = 0; 
                                    client.Player.MyJiangHu.RoundBuyPoints = 0;
                                }
                                client.Player.ArenaKills = client.Player.ArenaDeads = 0;
                                client.Player.HitShoot = client.Player.MisShoot = 0;

                                client.Player.ArenaSBKills = client.Player.ArenaSBDeads = 0;
                                client.Player.HitSBShoot = client.Player.MisSBShoot = 0;
                                client.Player.MRConquer = false;
                                client.Player.MSConquer = false;
                                client.Player.DbTry = false;
                                client.Player.LotteryEntries = 0;
                                client.Player.Day = DateTime.Now.DayOfYear;
                                if (client.Activeness != null) client.Activeness.NewDayOnline();
                                client.Player.BDExp = 0;
                                client.Player.TCCaptainTimes = 0;
                                client.Player.ExpBallUsed = 0;
                                client.DemonExterminator.FinishToday = 0;
                            
                                client.Player.helpladytime = 0;
                          
                                client.Player.BattleFieldPoints = 0;
                              

                                if (client.Player.MyChi != null)
                                {
                                    client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 300;
                                    Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                }
                                client.Player.Flowers.FreeFlowers = 1;
                                foreach (var flower in client.Player.Flowers)
                                    flower.Amount2day = 0;


                                if (client.Player.Flowers.FreeFlowers > 0)
                                {
                                    client.Send(stream.FlowerCreate(Role.Core.IsBoy(client.Player.Body)
                                        ? Game.MsgServer.MsgFlower.FlowerAction.FlowerSender
                                        : Game.MsgServer.MsgFlower.FlowerAction.Flower
                                        , 0, 0, client.Player.Flowers.FreeFlowers));
                                }

                                if (client.Player.Level >= 90)
                                {
                                    client.Player.Enilghten = ServerDatabase.CalculateEnlighten(client.Player);
                                    client.Player.SendUpdate(stream, client.Player.Enilghten, Game.MsgServer.MsgUpdate.DataType.EnlightPoints);
                                }

                                client.Player.BuyKingdomDeeds = 0;
                                client.Player.QuestGUI.RemoveQuest(35024);
                                client.Player.QuestGUI.RemoveQuest(35007);
                                client.Player.QuestGUI.RemoveQuest(35025);
                                client.Player.QuestGUI.RemoveQuest(35028);
                                client.Player.QuestGUI.RemoveQuest(35034);

                                //---- reset Quests
                                client.Player.QuestGUI.RemoveQuest(6390);
                                client.Player.QuestGUI.RemoveQuest(6329);
                                client.Player.QuestGUI.RemoveQuest(6245);
                                client.Player.QuestGUI.RemoveQuest(6049);
                                client.Player.QuestGUI.RemoveQuest(6366);
                                client.Player.QuestGUI.RemoveQuest(6014);
                                client.Player.QuestGUI.RemoveQuest(2375);
                                client.Player.QuestGUI.RemoveQuest(6126);
                                client.Player.DailyHeavenChance = client.Player.DailyMagnoliaChance
                                    = client.Player.DailyMagnoliaItemId
                                    = client.Player.DailyHeavenChance = client.Player.DailySpiritBeadCount = client.Player.DailyRareChance = 0;
                                //
                            }
                        }
                        ResetServerDay = (uint)DateTime.Now.DayOfYear;
                    }
                    catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                }
                ResetStamp.Value = Clock.Value + KernelThread.ResetDayStamp;
            }
        }
        #region Start-up timing
        static readonly List<KeyValuePair<string, long>> startupSteps = new List<KeyValuePair<string, long>>();
        static readonly System.Diagnostics.Stopwatch startupClock = System.Diagnostics.Stopwatch.StartNew();

        /// <summary>Runs one loader of Initialize and records how long it took (exceptions still stop the start-up as before).</summary>
        static void Step(string name, Action load)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            BootScreen.StepStart(name);
            try
            {
                load();
            }
            finally
            {
                startupSteps.Add(new KeyValuePair<string, long>(name, sw.ElapsedMilliseconds));
                BootScreen.StepDone();
            }
        }

        /// <summary>Prints the slowest loaders, so a slow start can be traced to its file.</summary>
        static void ReportStartup()
        {
            long total = 0;
            foreach (var s in startupSteps)
                total += s.Value;
            long upMs = (long)(DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime).TotalMilliseconds;
            MyConsole.WriteLine("Database loaded in " + startupClock.ElapsedMilliseconds + " ms, process up " + upMs + " ms (" + startupSteps.Count + " loaders, ini cache: "
                + WindowsAPI.IniCache.Parsed + " files / " + WindowsAPI.IniCache.Hits + " reads). Slowest:");
            foreach (var s in startupSteps.OrderByDescending(x => x.Value).Take(12))
                MyConsole.WriteLine("    " + s.Value.ToString().PadLeft(6) + " ms  " + s.Key);
            BootScreen.Slowest = string.Join(" · ", startupSteps.OrderByDescending(x => x.Value).Take(3).Select(x => x.Key + " " + x.Value + "ms"));
        }
        #endregion
        public static void Initialize()
        {
            Security.Guard.EnsureLoaded();
            ServerMaps = new Dictionary<uint, Role.GameMap>();
            GamePoll = new ConcurrentDictionary<uint, Client.GameClient>();
            NameUsed = new List<int>();
            WindowsAPI.IniFile IniFile = new WindowsAPI.IniFile(System.IO.Directory.GetCurrentDirectory() + "\\shell.ini");
            ServerDatabase.LenientLegacyLoad = IniFile.ReadInt32("Security", "LenientLegacyLoad", 1) != 0;
            if (!ServerDatabase.LenientLegacyLoad)
                MyConsole.WriteLine("[Security] LenientLegacyLoad=0: players with damaged save files (and no valid .bak) cannot log in.");
            Program.ServerConfig.IPAddres = IniFile.ReadString("ServerInfo", "AddresIP","");
            Program.ServerConfig.AuthPort = IniFile.ReadUInt16("ServerInfo", "Auth_Port",0);
            Program.ServerConfig.GamePort = IniFile.ReadUInt16("ServerInfo", "Game_Port",0);
            Program.ServerConfig.ServerName = IniFile.ReadString("ServerInfo", "ServerName", "");
            Program.ServerConfig.OfficialWebSite = IniFile.ReadString("ServerInfo", "WebSite", "");
            Program.ServerConfig.XtremeTopLink = IniFile.ReadString("ServerInfo", "XtremSite", "");
            Program.ServerConfig.FBStoreLink = IniFile.ReadString("ServerInfo", "FBStoreLink", "");
            Program.ServerConfig.RedeemLink = IniFile.ReadString("ServerInfo", "RedeemLink", "");
            DbName = IniFile.ReadString("ServerInfo", "DbName", "");
            DbPass = IniFile.ReadString("ServerInfo", "DbPass", "");
            string dbHost = IniFile.ReadString("ServerInfo", "DbHost", "localhost");
            string dbUser = IniFile.ReadString("ServerInfo", "DbUser", "root");
            uint dbPort = IniFile.ReadUInt16("ServerInfo", "DbPort", 3306);
            // The database settings live in one place: HazemConfig.ini [Global] MySql* (the same values the AccountServer
            // uses). shell.ini Db* is only the fallback when no HazemConfig.ini is found.
            try
            {
                var g = ClusterConfig.ReadRegistrySection("Global");
                string v;
                if (g != null && g.TryGetValue("mysqldatabase", out v) && v.Length > 0)
                {
                    DbName = v;
                    DbPass = g.TryGetValue("mysqlpassword", out v) ? v : "";
                    dbHost = g.TryGetValue("mysqlhost", out v) && v.Length > 0 ? v : "localhost";
                    dbUser = g.TryGetValue("mysqluser", out v) && v.Length > 0 ? v : "root";
                    ushort port;
                    dbPort = g.TryGetValue("mysqlport", out v) && ushort.TryParse(v, out port) ? port : (uint)3306;
                    MyConsole.WriteLine("MySQL settings from " + ClusterConfig.FindRegistry() + " (" + dbUser + "@" + dbHost + ":" + dbPort + "/" + DbName + ").");
                }
            }
            catch (Exception e) { MyConsole.WriteLine("HazemConfig.ini could not be read, using shell.ini Db*: " + e.Message); }
            DB =IniFile.ReadInt32("CPFestival", "DB", 0);
            Plus3Stone = IniFile.ReadInt32("CPFestival", "Plus3Stone", 0);
            Parement = IniFile.ReadInt32("CPFestival", "Parement", 0);
            ChiPack = IniFile.ReadInt32("CPFestival", "ChiPack", 0);
            ToughDrill = IniFile.ReadInt32("CPFestival", "ToughDrill", 0);
            StarDrill = IniFile.ReadInt32("CPFestival", "StarDrill", 0);
            DataHolder.CreateConnection(DbName, DbPass, dbHost, dbUser, dbPort);
            Program.ServerConfig.Port_BackLog = IniFile.ReadUInt16("InternetPort", "BackLog",100);
            Program.ServerConfig.Port_ReceiveSize = IniFile.ReadUInt16("InternetPort", "ReceiveSize",8194);
            Program.ServerConfig.Port_SendSize = IniFile.ReadUInt16("InternetPort", "SendSize",1024);

            Program.ServerConfig.DbLocation = IniFile.ReadString("Database", "Location","");
            Program.ServerConfig.CO2Folder = IniFile.ReadString("Database", "CO2FOLDER","");
            // player folders: git does not keep empty folders, so a fresh copy of the source has none of them
            foreach (var folder in new[] { "Users", "PlayersItems", "PlayersMail", "PlayersSpells", "PlayersProfs", "Quests", "Halo_Flag",
                "Guilds", "Clans", "Houses", "Agates", "Unions", "Retreate", "BackUp", "AABackUP" })
            {
                try { System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Program.ServerConfig.DbLocation, folder)); }
                catch (Exception e) { MyConsole.WriteLine("Could not create " + folder + ": " + e.Message); }
            }
            // [FastBoot, from Qahr] shell.ini [Performance] FastBoot (default 1): read gamemap.dat + the DMap files on pool
            // threads while the loaders below run; the "Role.GameMap.LoadMaps" step waits for them and builds the maps as before.
            if (Conquer6609.Framework.Settings.Bool("Performance", "FastBoot", true))
            {
                Role.GameMap.PrefetchMaps();
                // parse the big ini folders into the ini cache meanwhile (LoadMonsters, map info); reads are unchanged
                WindowsAPI.IniCache.Prefetch(Program.ServerConfig.DbLocation + "\\Monsters\\", false);
                WindowsAPI.IniCache.Prefetch(Program.ServerConfig.DbLocation + "\\MonsterSpawns\\", true);
                WindowsAPI.IniCache.Prefetch(Program.ServerConfig.DbLocation + "\\maps\\", false);
            }
            // [Storage, from Qahr] player data in MySQL too (players + player_blobs); shell.ini [Storage] PlayerBackend
            Step("Storage.PlayerStorage.Init", () => Storage.PlayerStorage.Init());

            Program.ServerConfig.InterServerAddress = IniFile.ReadString("InterServer", "AddresIP", "");
            Program.ServerConfig.InterServerPort = IniFile.ReadUInt16("InterServer", "Port", 9915);
            Program.ServerConfig.IsInterServer = IniFile.ReadUInt32("InterServer", "IsInterServer", 0) > 0 ? true : false;

            RetailDuration = DateTime.FromBinary(IniFile.ReadInt64("RetailRedeem", "Duration", 0));
            RebornInfo = new RebornInfomations();
            Step("RebornInfo.Load", () => RebornInfo.Load());
            Inbox_Counter.Set(IniFile.ReadUInt32("Database", "MailUID", 0));
            uint nextmail = Inbox_Counter.Next;
            // per-server item UID range ([Database] ItemUIDMin/ItemUIDMax, ClusterConfig; default = whole old range)
            ClusterConfig.LoadShell();
            ITEM_Counter.Set(ClusterConfig.StartItemCounter(IniFile.ReadUInt32("Database", "ItemUID", 0)));
            uint nextitem = ITEM_Counter.Next;
            // ignores (and audits) bogus UIDs far above the saved counter instead of raising it near uint.MaxValue;
            // UIDs outside this server's range (items transferred from another server) are skipped
            uint highestItemUID = ServerDatabase.ScanHighestItemUID(ITEM_Counter.Count, ClusterConfig.ItemUIDMin, ClusterConfig.ItemUIDMax);
            if (highestItemUID >= ITEM_Counter.Count)
            {
                MyConsole.WriteLine("Item UID counter raised from " + ITEM_Counter.Count + " to " + (highestItemUID + 1) + " (highest saved item UID " + highestItemUID + ").");
                ITEM_Counter.EnsureAtLeast((long)highestItemUID + 1);
            }
            ClusterConfig.ArmItemCounter(ITEM_Counter);
            ClientCounter.Set(IniFile.ReadUInt32("Database", "ClientUID", 1000000));
            uint nextclient = ClientCounter.Next;
            ResetServerDay = IniFile.ReadUInt32("Database", "Day", 0);
            Game.MsgTournaments.MsgSchedules.PkWar.WinnerUID = IniFile.ReadUInt32("Tournaments", "PkWarWinner", 0);
    
            Step("ActionHelper.Create", () => ActionHelper.Create());
            ItemsBase = new ItemType();
            RifineryItems = new Rifinery();
            DBRerinaryBoxes = new RefinaryBoxes();
            Step("ItemsBase.Loading", () => ItemsBase.Loading());

            //-------------------------- Load shops -------------------
            Step("Shops.ChampionShop.Load", () => Shops.ChampionShop.Load());
            Step("Shops.EShopFile.Load", () => Shops.EShopFile.Load());
            Step("Shops.EShopV2File.Load", () => Shops.EShopV2File.Load());
            Step("Shops.HonorShop.Load", () => Shops.HonorShop.Load());
            Step("Shops.RacePointShop.Load", () => Shops.RacePointShop.Load());
            Step("Shops.ShopFile.Load", () => Shops.ShopFile.Load());
            //--------------------------
            Step("SystemBanned.Load", () => SystemBanned.Load());
            Step("SystemBannedAccount.Load", () => SystemBannedAccount.Load());
            Step("Database.ShareVIP.Load", () => Database.ShareVIP.Load());
            Step("MiningTable.Load", () => MiningTable.Load());
            Step("LoadExpInfo", () => LoadExpInfo());
            Step("DataCore.AtributeStatus.Load", () => DataCore.AtributeStatus.Load());
            Step("Role.GameMap.LoadMaps", () => Role.GameMap.LoadMaps());
            Step("NpcServer.LoadNpcs", () => NpcServer.LoadNpcs());
            Step("Magic.Load", () => Magic.Load());
            Step("LoadMonsters", () => LoadMonsters());
            Step("Tranformation.Int", () => Tranformation.Int());
            Step("QuestInfo.Init", () => QuestInfo.Init());
            Step("LoadPortals", () => LoadPortals());
            Step("Allow_Clicker.Load", () => Allow_Clicker.Load());
            Step("UnLockLogin.Load", () => UnLockLogin.Load());
            Step("SystemBannedPC.Load", () => SystemBannedPC.Load());
            #region Other
            Step("SubClassInfo.Load", () => SubClassInfo.Load());
            Step("ChiTable.Load", () => ChiTable.Load());
            Step("FlowersTable.Load", () => FlowersTable.Load());
            Step("NobilityTable.Load", () => NobilityTable.Load());
            Step("Role.Instance.Associate.Load", () => Role.Instance.Associate.Load());
            Step("CoatStorage.Load", () => CoatStorage.Load());
            Step("NpcServer.LoadSobNpcs", () => NpcServer.LoadSobNpcs());

            Step("LeagueTable.Load", () => LeagueTable.Load());
            Step("GuildTable.Load", () => GuildTable.Load());
            Step("Database.ClanTable.Load", () => Database.ClanTable.Load());
            Step("JianHuTable.LoadStatus", () => JianHuTable.LoadStatus());
            Step("JianHuTable.LoadJiangHu", () => JianHuTable.LoadJiangHu());
            Step("TaskRewards.Load", () => TaskRewards.Load());
            Step("DBEffects.Load", () => DBEffects.Load());

            Step("InnerPowerTable.LoadDBInformation", () => InnerPowerTable.LoadDBInformation());
            Step("ExchangeShop.LoadDBInfo", () => ExchangeShop.LoadDBInfo());
            Step("Co_Panle.LoadAttack", () => Co_Panle.LoadAttack());
            Step("QuizShow.Load", () => QuizShow.Load());
            //Game.MsgTournaments.MsgSchedules.UnionWar.Create();
            Step("Game.MsgTournaments.MsgSchedules.CouplesPKWar.Load", () => Game.MsgTournaments.MsgSchedules.CouplesPKWar.Load());
            Step("Game.MsgTournaments.MsgSchedules.GuildWarScore.Load", () => Game.MsgTournaments.MsgSchedules.GuildWarScore.Load());
            Step("Game.MsgTournaments.MsgSchedules.ClassPkWar.Load", () => Game.MsgTournaments.MsgSchedules.ClassPkWar.Load());
            Step("Game.MsgTournaments.MsgSchedules.ElitePkTournament.Load", () => Game.MsgTournaments.MsgSchedules.ElitePkTournament.Load());
            Step("Game.MsgTournaments.MsgSchedules.TeamPkTournament.Load", () => Game.MsgTournaments.MsgSchedules.TeamPkTournament.Load());
            Step("Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Load", () => Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Load());
            Step("Database.PrestigeRanking.Load", () => Database.PrestigeRanking.Load());
            Step("RankItems.LoadAllItems", () => RankItems.LoadAllItems());
            Step("TitleStorage.LoadDBInformation", () => TitleStorage.LoadDBInformation());
            Step("ItemRefineUpgrade.Load", () => ItemRefineUpgrade.Load());

            Step("TheCrimeTable.Load", () => TheCrimeTable.Load());
            Step("ActivityTasks.Load", () => ActivityTasks.Load());
            Step("Role.Statue.Load", () => Role.Statue.Load());
            Step("Role.KOBoard.KOBoardRanking.Load", () => Role.KOBoard.KOBoardRanking.Load());
            Step("TableHeroRewards.LoadInformations", () => TableHeroRewards.LoadInformations());
            Step("Database.Disdain.Load", () => Database.Disdain.Load());

            Step("RechargeShop.Load", () => RechargeShop.Load());
            Step("Arena.Load", () => Arena.Load());
            Step("TeamArena.Load", () => TeamArena.Load());
            Step("InnerPowerTable.Load", () => InnerPowerTable.Load());


            Step("MsgInterServer.Instance.CrossElitePKTournament.Load", () => MsgInterServer.Instance.CrossElitePKTournament.Load());

            Step("Database.TutorInfo.Load", () => Database.TutorInfo.Load());
            Step("LoadAgates", () => LoadAgates());
            Step("InfoDemonExterminators.Create", () => InfoDemonExterminators.Create());
            Step("Lottery.LoadLotteryItems", () => Lottery.LoadLotteryItems());
            Step("Roulettes.Load", () => Roulettes.Load());
            Step("PocketInformations.LoadPokerTables", () => PocketInformations.LoadPokerTables());
            Step("QueueContainer.Load", () => QueueContainer.Load());
            Step("LoadMapName", () => LoadMapName());
            Step("GroupServerList.Load", () => GroupServerList.Load());
            ClusterConfig.ValidateGroup();// ServerID / client_config / servers.ini (refuses to start on a conflict)
            Step("VoteSystem.Load", () => VoteSystem.Load());
            Step("Database.RanksTable.Initialize", () => Database.RanksTable.Initialize());
            Step("RewardSystem.Load", () => RewardSystem.Load());
            Step("Booths.Load", () => Booths.Load());
            Step("Conquer6609.Framework.Modules.ModuleManager.Initialize", () => Conquer6609.Framework.Modules.ModuleManager.Initialize());
            FullLoading = true;
            ReportStartup();
            #endregion
        }
        public static void SaveFestival()
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile(System.IO.Directory.GetCurrentDirectory() + "\\shell.ini");
            write.Write<int>("CPFestival", "DB", DB);
            write.Write<int>("CPFestival", "Plus3Stone", Plus3Stone);
            write.Write<int>("CPFestival", "Parement", Parement);
            write.Write<int>("CPFestival", "ChiPack", ChiPack);
            write.Write<int>("CPFestival", "ToughDrill", ToughDrill);
            write.Write<int>("CPFestival", "StarDrill", StarDrill);
        }
     
        public static void LoadMapName()
        {
  
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "GameMapEx.ini"))
            {
                foreach (var map in ServerMaps.Values)
                {
                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("GameMapEx.ini");
                    map.Name = ini.ReadString(map.ID.ToString(), "Name", Program.ServerConfig.ServerName);
                }
            }
        }
        public static void LoadExpInfo()
        {
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "levexp.txt"))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "levexp.txt"))
                {
                    while (true)
                    {
                        string GetLine = read.ReadLine();
                        if (GetLine == null) return;
                        string[] line = GetLine.Split(' ');
                        DBLevExp exp = new DBLevExp();
                        exp.Action = (DBLevExp.Sort)byte.Parse(line[0]);
                        exp.Level = byte.Parse(line[1]);
                        exp.Experience = ulong.Parse(line[2]);
                        exp.UpLevTime = int.Parse(line[3]);
                        exp.MentorUpLevTime = int.Parse(line[4]);

                        if (!LevelInfo.ContainsKey(exp.Action))
                            LevelInfo.Add(exp.Action, new Dictionary<byte, DBLevExp>());

                        LevelInfo[exp.Action].Add(exp.Level, exp);

                    }
                }
            }
            // [FastBoot, from Qahr] was GC.Collect(): a full blocking GC of every heap on each loader; the runtime collects on its own
        }
        public static void LoadAgates()
        {
            try
            {
                if (!System.IO.Directory.Exists(Program.ServerConfig.DbLocation + "\\Agates\\"))
                    System.IO.Directory.CreateDirectory(Program.ServerConfig.DbLocation + "\\Agates\\");
                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Agates\\"))
                {
                    try
                    {
                        ini.FileName = fname;
                        uint UID = ini.ReadUInt32("Character", "UID", 0);
                        var agates = new Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>>();
                        int CountGates = ini.ReadInt32("Character", "Count", 0);
                        for (int y = 0; y < CountGates; y++)
                        {
                            uint ItemUID = ini.ReadUInt32("Item" + y.ToString(), "UID", 0);
                            int Count = ini.ReadInt32("Item" + y.ToString(), "Count", 0);
                            if (Count < 0)
                                Count = 0;
                            var gates = new Dictionary<uint, Tuple<uint, uint, uint, string>>(Count);
                            agates[ItemUID] = gates;
                            for (int x = 0; x < Count; x++)
                            {
                                string[] strs = ini.ReadString("Item" + y.ToString(), "Gate" + x.ToString(), "").Split('~');
                                if (strs.Length < 4)
                                    continue;
                                gates[(uint)x] = Tuple.Create(uint.Parse(strs[0]), uint.Parse(strs[1]), uint.Parse(strs[2]), strs[3]);
                            }
                        }
                        ClientAgates[UID] = agates;
                        agatesWritten[UID] = AgatesSnapshot(UID, agates);// unchanged agates are not rewritten
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteLine("Agates file " + fname + " skipped: " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        // Text of each player's agates as last written (or loaded): SaveAgates skips players whose agates are
        // unchanged instead of rewriting every file with one WritePrivateProfileString per value.
        static readonly ConcurrentDictionary<uint, string> agatesWritten = new ConcurrentDictionary<uint, string>();

        /// <summary>Optional hint for code that changes a player's agates: forces the next SaveAgates to write
        /// that player's file. (Not required: changes are detected by comparing the content.)</summary>
        public static void MarkAgatesDirty(uint uid)
        {
            string ignored;
            agatesWritten.TryRemove(uid, out ignored);
        }

        /// <summary>Canonical text of one player's agates (same values SaveAgates writes), used for change detection.</summary>
        static string AgatesSnapshot(uint uid, Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>> agates)
        {
            KeyValuePair<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>>[] items;
            lock (agates)
                items = agates.ToArray();
            StringBuilder sb = new StringBuilder(64 + items.Length * 128);
            sb.Append(uid).Append('|').Append(items.Length);
            foreach (var gate in items)
            {
                KeyValuePair<uint, Tuple<uint, uint, uint, string>>[] coordinates;
                lock (gate.Value)
                    coordinates = gate.Value.ToArray();
                sb.Append('\n').Append(gate.Key).Append('|').Append(coordinates.Length);
                foreach (var coors in coordinates)
                    sb.Append('|').Append(coors.Key).Append('=').Append(coors.Value.Item1).Append('~').Append(coors.Value.Item2)
                        .Append('~').Append(coors.Value.Item3).Append('~').Append(coors.Value.Item4);
            }
            return sb.ToString();
        }

        public static void SaveAgates()
        {
            try
            {
                foreach (var agate in ClientAgates.ToArray())
                {
                    try
                    {
                        string path = Program.ServerConfig.DbLocation + "\\Agates\\" + agate.Key + ".ini";
                        string snapshot = AgatesSnapshot(agate.Key, agate.Value);
                        string last;
                        if (agatesWritten.TryGetValue(agate.Key, out last) && string.Equals(last, snapshot, StringComparison.Ordinal)
                            && System.IO.File.Exists(path))
                            continue;
                        SaveAgatesFile(agate.Key, agate.Value);
                        agatesWritten[agate.Key] = snapshot;
                    }
                    catch (Exception e)
                    {
                        MarkAgatesDirty(agate.Key);
                        MyConsole.WriteException(e);
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        static void SaveAgatesFile(uint uid, Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>> agates)
        {
            // Snapshot under the same locks the gameplay code uses (MsgSuperFlag locks the inner dictionary).
            KeyValuePair<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>>[] items;
            lock (agates)
                items = agates.ToArray();
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Agates\\" + uid + ".ini");
            write.Write<uint>("Character", "UID", uid);
            write.Write<int>("Character", "Count", items.Length);
            int x = 0;
            foreach (var gate in items)
            {
                KeyValuePair<uint, Tuple<uint, uint, uint, string>>[] coordinates;
                lock (gate.Value)
                    coordinates = gate.Value.ToArray();
                write.Write<uint>("Item" + x.ToString(), "UID", gate.Key);
                write.Write<int>("Item" + x.ToString(), "Count", coordinates.Length);
                foreach (var coors in coordinates)
                {
                    write.Write<string>("Item" + x.ToString(), "Gate" + coors.Key.ToString(), coors.Value.Item1 + "~" + coors.Value.Item2 + "~" + coors.Value.Item3 + "~" + coors.Value.Item4);
                }
                x++;
            }
        }
        public static void LoadMonsters()
        {
            try
            {
               
                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Monsters\\"))
                {
                    Game.MsgMonster.MonsterFamily Family = ParseMonsterFamily(ini, fname);
                    if (Family == null)
                        continue;
                    Family.CreateItemGenerator();
                    Family.CreateMonsterSettings();
                    try
                    {
                        MonsterFamilies.Add(Family.ID, Family);
                    }
                    catch { MyConsole.WriteLine("Error In File " + fname); }
                }
                LoadMonsterSpawns();
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }

        /// <summary>One Monsters\*.ini -> a MonsterFamily (no item generator / settings yet), or null for a corrupt file.
        /// Shared by LoadMonsters and the live reload (HotReload\DataHotReload).</summary>
        public static Game.MsgMonster.MonsterFamily ParseMonsterFamily(WindowsAPI.IniFile ini, string fname)
        {
                    ini.FileName = fname;
                    Game.MsgMonster.MonsterFamily Family = new Game.MsgMonster.MonsterFamily();
                    Family.ID = ini.ReadUInt32("cq_monstertype", "id", 0);
                    Family.Name = ini.ReadString("cq_monstertype", "name", "INVALID_MOB");

                    Family.Level = ini.ReadUInt16("cq_monstertype", "level", 0);
                    Family.MaxAttack = ini.ReadInt32("cq_monstertype", "attack_max", 0);
                    Family.MinAttack = ini.ReadInt32("cq_monstertype", "attack_min", 0);
                    if (Family.Name == "INVALID_MOB" || Family.Level == 0 || Family.ID == 0 || Family.MinAttack > Family.MaxAttack)
                    {
                        MyConsole.WriteLine("MONSTER FILE CORRUPT: \r\n" + fname + "\r\n");
                        return null;
                    }
                    Family.Defense = ini.ReadUInt16("cq_monstertype", "defence", 0);
                    Family.Mesh = ini.ReadUInt16("cq_monstertype", "lookface", 0);
                    Family.MaxHealth = ini.ReadInt32("cq_monstertype", "life", 0);
                    Family.ViewRange = 16;
                    Family.AttackRange = ini.ReadSByte("cq_monstertype", "attack_range", 0);
                    Family.Dodge = ini.ReadByte("cq_monstertype", "dodge", 0);
                    Family.DropBoots = ini.ReadByte("cq_monstertype", "drop_shoes", 0);
                    Family.DropNecklace = ini.ReadByte("cq_monstertype", "drop_necklace", 0);
                    Family.DropRing = ini.ReadByte("cq_monstertype", "drop_ring", 0);
                    Family.DropArmet = ini.ReadByte("cq_monstertype", "drop_armet", 0);
                    Family.DropArmor = ini.ReadByte("cq_monstertype", "drop_armor", 0);
                    Family.DropShield = ini.ReadByte("cq_monstertype", "drop_shield", 0);
                    Family.DropWeapon = ini.ReadByte("cq_monstertype", "drop_weapon", 0);
                    Family.DropMoney = ini.ReadUInt16("cq_monstertype", "drop_money", 0);
                    Family.DropHPItem = ini.ReadUInt32("cq_monstertype", "drop_hp", 0);
                    Family.DropMPItem = ini.ReadUInt32("cq_monstertype", "drop_mp", 0);
                    Family.Boss = ini.ReadByte("cq_monstertype", "Boss", 0);
                    Family.Defense2 = ini.ReadInt32("cq_monstertype", "defence2", 0);
                    if (Family.Boss != 0)
                        Family.AttackRange = 3;
                 
                    Family.MoveSpeed = ini.ReadInt32("cq_monstertype", "move_speed", 0);
                    Family.AttackSpeed = ini.ReadInt32("cq_monstertype", "attack_speed", 0);
                    Family.SpellId = ini.ReadUInt32("cq_monstertype", "magic_type", 0);

                    Family.ExtraCritical = ini.ReadUInt32("cq_monstertype", "critical", 0);
                    Family.ExtraBreack = ini.ReadUInt32("cq_monstertype", "break", 0);

                    Family.extra_battlelev = ini.ReadInt32("cq_monstertype", "extra_battlelev", 0);
                    Family.extra_exp = ini.ReadInt32("cq_monstertype", "extra_exp", 0);
                    Family.extra_damage = ini.ReadInt32("cq_monstertype", "extra_damage", 0);

                    Family.DropSpecials = new Game.MsgMonster.SpecialItemWatcher[ini.ReadInt32("SpecialDrop", "Count", 0)];
                    for (int i = 0; i < Family.DropSpecials.Length; i++)
                    {
                        string[] Data = ini.ReadString("SpecialDrop", i.ToString(), "", 32).Split(',');
                     
                        Family.DropSpecials[i] = new Game.MsgMonster.SpecialItemWatcher(uint.Parse(Data[0]), int.Parse(Data[1]));
                    }
                    return Family;
        }

        /// <summary>MonsterSpawns layout: <spawnsRoot>\<map id>\<type folder>\*.ini (cq_generator). Used for ServerDataBase\MonsterSpawns
        /// (serverMaps = skip the maps that use Monsters<map>.txt) and for the content\MonsterSpawns of a module (Modules\ModuleContent).</summary>
        public static int LoadSpawnFolder(string spawnsRoot, bool serverMaps)
        {
            int added = 0;
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                foreach (string fmap in System.IO.Directory.GetDirectories(spawnsRoot))
                {
                    uint tMapID;
                    if (!uint.TryParse(System.IO.Path.GetFileName(fmap.TrimEnd('\\', '/')), out tMapID))
                        continue;
                    if (serverMaps && (tMapID == 1002 || tMapID == 1020 || tMapID == 1038 || tMapID == 1000 || tMapID == 3935))// || tMapID == 1015) - these maps load their spawns from Monsters<map>.txt
                        continue;
                    // a map that already has monsters (a module adding spawns to a server map) keeps its collection
                    Role.GameMap spawnMap;
                    Game.MsgMonster.MobCollection colletion = ServerMaps != null && ServerMaps.TryGetValue(tMapID, out spawnMap) && spawnMap.MonstersColletion != null
                        ? spawnMap.MonstersColletion : new Game.MsgMonster.MobCollection(tMapID);
                    if (colletion.ReadMap())
                    {
                        foreach (string fmobtype in System.IO.Directory.GetDirectories(fmap))
                        {
                            foreach (string ffile in System.IO.Directory.GetFiles(fmobtype))
                            {
                                ini.FileName = ffile;
                                colletion.LocationSpawn = ffile;

                                uint ID = ini.ReadUInt32("cq_generator", "npctype", 0);

                                Game.MsgMonster.MonsterFamily famil;
                                if (!MonsterFamilies.TryGetValue(ID, out famil))
                                {
                                    continue;
                                }
                                if (Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(famil.ID))
                                    continue;
                                Game.MsgMonster.MonsterFamily Monster = famil.Copy();
                           
                                Monster.SpawnX = ini.ReadUInt16("cq_generator", "bound_x", 0);
                                Monster.SpawnY = ini.ReadUInt16("cq_generator", "bound_y", 0);
                                Monster.MaxSpawnX = (ushort)(Monster.SpawnX + ini.ReadUInt16("cq_generator", "bound_cx", 0));
                                Monster.MaxSpawnY = (ushort)(Monster.SpawnY + ini.ReadUInt16("cq_generator", "bound_cy", 0));
                                Monster.MapID = ini.ReadUInt32("cq_generator", "mapid", 0);
                                Monster.SpawnCount = ini.ReadByte("cq_generator", "max_per_gen", 0);//"maxnpc", 0);//max_per_gen", 0);
                                Monster.rest_secs = ini.ReadInt32("cq_generator", "rest_secs", 0);

                               
                                if (Monster.MapID == 1011 || Monster.MapID == 3071 || Monster.MapID == 1770 || Monster.MapID == 1771 || Monster.MapID == 1772
                                    || Monster.MapID == 1773 || Monster.MapID == 1774 || Monster.MapID == 1775  || Monster.MapID == 1777
                                    || Monster.MapID == 1782 || Monster.MapID == 1785 || Monster.MapID == 1786 || Monster.MapID == 1787
                                    || Monster.MapID == 1794)
                                    Monster.SpawnCount = ini.ReadByte("cq_generator", "maxnpc", 0);
                                colletion.Add(Monster);
                                added++;
                            }
                        }
                    }
                }
            return added;
        }

        static void LoadMonsterSpawns()
        {
            try
            {
                LoadSpawnFolder(Program.ServerConfig.DbLocation + "\\MonsterSpawns\\", true);
                LoadMapMonsters("Monsters1002.txt");
                LoadMapMonsters("Monsters1020.txt");

                LoadMapMonsters("Monsters1011.txt");
                LoadMapMonsters("Monsters1768.txt");
                LoadMapMonsters("Monsters1000.txt");

                LoadMapMonsters("Monsters1779.txt");
                LoadMapMonsters("Monsters1780.txt");
                LoadMapMonsters("Monsters3935.txt");
                LoadMapMonsters("Monsters3998.txt");
                LoadMapMonsters("Monsters10089.txt");
                LoadMapMonsters("Monsters10137.txt");
                LoadMapMonsters("Monsters10166.txt");
                LoadMapMonsters("Monsters1043.txt");
                LoadMapMonsters("Monsters1044.txt");
                LoadMapMonsters("Monsters1045.txt");
                LoadMapMonsters("Monsters1046.txt");
                LoadMapMonsters("Monsters1047.txt");
                LoadMapMonsters("Monsters1048.txt");
                LoadMapMonsters("Monsters8822.txt");
                LoadMapMonsters("Monsters3030.txt");
                // [FastBoot, from Qahr] was GC.Collect(): a full blocking GC of every heap on each loader; the runtime collects on its own
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public static void LoadMapMonsters(string file)
        {
            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "MonsterSpawns\\" + file + ""))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "MonsterSpawns\\" + file + ""))
                {
                    while (true)
                    {
                        
                            string aline = read.ReadLine();
                            if (aline != null && aline != "")
                            {
                                try
                                {
                                string[] line = aline.Split(',');
                                uint body = uint.Parse(line[1]);
                                string name = line[2];
                                if (name.Contains("Titan"))
                                    continue;
                              
                                ushort X = ushort.Parse(line[3]);
                                ushort Y = ushort.Parse(line[4]);
                                uint Map = uint.Parse(line[5]);
                                var GMap = Database.Server.ServerMaps[Map];
                                if (GMap.MonstersColletion == null)
                                {
                                    GMap.MonstersColletion = new Game.MsgMonster.MobCollection(GMap.ID);
                                }
                                else if (GMap.MonstersColletion.DMap == null)
                                    GMap.MonstersColletion.DMap = GMap;
                                foreach (var _monster in MonsterFamilies.Values)
                                {
                                   
                                    if (_monster.Name == name)
                                    {

                                        Game.MsgMonster.MonsterFamily Monster = _monster.Copy();

                                        Monster.SpawnX = X;
                                        Monster.SpawnY = Y;
                                        Monster.MaxSpawnX = (ushort)(X + 1);
                                        Monster.MaxSpawnY = (ushort)(Y + 1);
                                        Monster.MapID = GMap.ID;
                                        Monster.SpawnCount = 1;
                                        Game.MsgMonster.MonsterRole rolemonster = GMap.MonstersColletion.Add(Monster, false, 0, Map != 10166);
                                        break;
                                    }


                                }
                                 }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            break;
                        }
                            }
                            else
                                break;
                       
                    }
                }
            }
        }
        public unsafe static void AddMapMonster(ServerSockets.Packet stream, Role.GameMap map, uint ID, ushort x, ushort y, ushort max_x, ushort max_y, byte count, uint DinamicID = 0, bool RemoveOnDead = true
            , Game.MsgFloorItem.MsgItemPacket.EffectMonsters m_effect = Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None,  string streffect = "")
        {
            if (map.MonstersColletion == null)
            {
                map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            }
            if (map.MonstersColletion.ReadMap())
            {

                Game.MsgMonster.MonsterFamily famil;
                if (MonsterFamilies.TryGetValue(ID, out famil))
                {
                    Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                    Monster.SpawnX = x;
                    Monster.SpawnY = y;
                    Monster.MaxSpawnX = (ushort)(x + max_x);
                    Monster.MaxSpawnY = (ushort)(y + max_y);
                    Monster.MapID = map.ID;
                    Monster.SpawnCount = count;
                    Monster.LavaX = x;
                    Monster.LavaY = y;
                    Game.MsgMonster.MonsterRole rolemonster = map.MonstersColletion.Add(Monster, RemoveOnDead, DinamicID, true);

                    if (rolemonster == null)
                    {
                        Console.WriteLine("Eror monster spawn. Server.");
                        return;
                    }

                    Game.MsgServer.ActionQuery action = new Game.MsgServer.ActionQuery()
                    {
                        ObjId = rolemonster.UID,
                        Type = Game.MsgServer.ActionType.RemoveEntity
                    };
                    rolemonster.Send(stream.ActionCreate(&action));
                    rolemonster.Send(rolemonster.GetArray(stream, false));

                    if (streffect != null)
                    {
                        rolemonster.SendString(stream, MsgStringPacket.StringID.Effect, streffect);
                    }
                        
                        
                   
                    if (m_effect != Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None && rolemonster != null)
                    {
                        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                        effect.m_UID = (uint)m_effect;
                        effect.m_X = rolemonster.X;
                        effect.m_Y = rolemonster.Y;
                        effect.DropType = MsgDropID.Earth;
                        rolemonster.Send(stream.ItemPacketCreate(effect));
                        rolemonster.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, "glebesword");
                    }
                    if (rolemonster.HitPoints > 65535)
                    {
                        Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, rolemonster.UID, 2);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, rolemonster.Family.MaxHealth);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, rolemonster.HitPoints);
                        stream = Upd.GetArray(stream);
                        rolemonster.Send(stream);
                    }
                }
            }
        }
        public unsafe static bool AddFloor(ServerSockets.Packet stream, Role.GameMap map, uint ID, ushort x, ushort y, ushort spelllevel, Database.MagicType.Magic dbspell,Client.GameClient Owner,  uint GuildID, uint OwnerUID, uint DinamicID = 0,string Name = "", bool RemoveOnDead = true)
        {
            try
            {
                if (map.MonstersColletion == null)
                {
                    map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
                }
                if (map.MonstersColletion.ReadMap())
                {

                    Game.MsgMonster.MonsterFamily famil;
                    if (MonsterFamilies.TryGetValue(1, out famil))
                    {
                        Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                        Monster.SpawnX = x;
                        Monster.SpawnY = y;
                        Monster.MaxSpawnX = (ushort)(x + 1);
                        Monster.MaxSpawnY = (ushort)(y + 1);
                        Monster.MapID = map.ID;
                        Monster.SpawnCount = 1;
                        Game.MsgMonster.MonsterRole rolemonster = map.MonstersColletion.Add(Monster, RemoveOnDead, DinamicID, true);
                        if (rolemonster == null)
                        {
                            //invalid x ,y
                            return false;
                        }
                        rolemonster.Family.ID = ID;
                        rolemonster.IsFloor = true;
                        rolemonster.FloorStampTimer = DateTime.Now.AddSeconds(7);
                        rolemonster.Family.Settings = Game.MsgMonster.MonsterSettings.Lottus;

                        rolemonster.FloorPacket = new MsgItemPacket();
                        rolemonster.FloorPacket.m_UID = rolemonster.UID;
                        rolemonster.FloorPacket.m_ID = ID;
                        rolemonster.FloorPacket.m_X = x;
                        rolemonster.FloorPacket.m_Y = y;
                        rolemonster.FloorPacket.MaxLife = 25;
                        rolemonster.FloorPacket.Life = 25;
                        rolemonster.FloorPacket.DropType = MsgDropID.Effect;
                        rolemonster.FloorPacket.m_Color = 13;
                        rolemonster.FloorPacket.ItemOwnerUID = OwnerUID;
                        rolemonster.FloorPacket.GuildID = GuildID;
                        rolemonster.FloorPacket.FlowerType = 2;//2;
                        rolemonster.FloorPacket.Timer = Role.Core.TqTimer(rolemonster.FloorStampTimer);
                        rolemonster.FloorPacket.Name = Name;

                        rolemonster.DBSpell = dbspell;
                        rolemonster.Family.MaxHealth = 25;
                        rolemonster.HitPoints = 25;
                        rolemonster.OwnerFloor = Owner;
                        rolemonster.SpellLevel = spelllevel;
                        

                        if (rolemonster == null)
                        {
                            Console.WriteLine("Eror monster spawn. Server.");
                            return  false;
                        }
                        map.View.EnterMap<Role.IMapObj>(rolemonster);
                        rolemonster.Send(rolemonster.GetArray(stream, false));
                        return true;
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return false;

        }
        public unsafe static void LoadDatabase()
        {
            try
            {
                foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                {
                    WindowsAPI.IniFile IniFile = new WindowsAPI.IniFile(fname);
                    IniFile.FileName = fname;
                    string name = IniFile.ReadString("Character", "Name", "");
                    lock (NameUsed)
                        NameUsed.Add(name.GetHashCode());
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        static readonly object saveDatabaseLock = new object();
        static int backgroundSaveDatabase;

        /// <summary>True while a SaveDatabase started by SaveDatabaseInBackground is running.</summary>
        public static bool SaveDatabaseInProgress
        {
            get { return System.Threading.Volatile.Read(ref backgroundSaveDatabase) != 0; }
        }

        /// <summary>Starts SaveDatabase on a background thread (KernelThread does not wait for the disk).
        /// Does nothing (returns false) while the previous background save is still running. SaveDatabase()
        /// itself never overlaps: the console "save", shutdown and the daily restart wait for a running save
        /// and then save again on their own thread.</summary>
        public static bool SaveDatabaseInBackground()
        {
            if (!FullLoading)
                return false;
            if (System.Threading.Interlocked.CompareExchange(ref backgroundSaveDatabase, 1, 0) != 0)
                return false;
            try
            {
                var thread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        SaveDatabase();
                    }
                    catch (Exception e) { MyConsole.SaveException(e); }
                    finally
                    {
                        System.Threading.Volatile.Write(ref backgroundSaveDatabase, 0);
                    }
                });
                thread.IsBackground = true;
                thread.Name = "SaveDatabase";
                thread.Start();
                return true;
            }
            catch (Exception e)
            {
                System.Threading.Volatile.Write(ref backgroundSaveDatabase, 0);
                MyConsole.SaveException(e);
                return false;
            }
        }

        /// <summary>Saves the global tables synchronously. Concurrent calls are serialized (a caller waits for
        /// a save that is already running, then runs its own).</summary>
        public unsafe static void SaveDatabase()
        {
            if (!FullLoading)
                return;
            lock (saveDatabaseLock)
                SaveDatabaseCore();
        }
        unsafe static void SaveDatabaseCore()
        {
            Conquer6609.Framework.Modules.ModuleManager.Save();
            try
            {
                try
                {
                    Save(new Action(Database.JianHuTable.SaveJiangHu));
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                try
                {
                    Save(new Action(Role.Instance.Associate.Save));
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                try
                {
                    Save(new Action(Database.GuildTable.Save));
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                try
                {
                    Save(new Action(Database.AttackCompatetor.Save));
                }
                catch (Exception e) { MyConsole.SaveException(e); }
                WindowsAPI.IniFile IniFile = new WindowsAPI.IniFile("");
                IniFile.FileName = System.IO.Directory.GetCurrentDirectory() + "\\shell.ini";
                IniFile.Write<uint>("Database", "MailUID", ReserveMark(Inbox_Counter.Count, MailUIDReserve));
                IniFile.Write<uint>("Database", "ItemUID", ReserveMark(ITEM_Counter.Count, ItemUIDReserve));
                ClusterConfig.CheckItemUsage(ITEM_Counter.Count);
                IniFile.Write<uint>("Database", "ClientUID", ClientCounter.Count);
                IniFile.Write<uint>("Database", "Day", ResetServerDay);
                IniFile.Write<uint>("Tournaments", "PkWarWinner", Game.MsgTournaments.MsgSchedules.PkWar.WinnerUID);
                IniFile.Write<uint>("Tournaments", "MonthlyPkWarWinner", Game.MsgTournaments.MsgSchedules.PkWar.WinnerMonthlyUID);
                Save(new Action(Database.ClanTable.Save));
                Save(new Action(QueueContainer.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.GuildWar.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Save));
                Save(new Action(SaveAgates));
                Save(new Action(SaveFestival));
                Save(new Action(TheCrimeTable.Save));
                Save(new Action(Arena.Save));
                Save(new Action(TeamArena.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.ClassPkWar.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.ElitePkTournament.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.TeamPkTournament.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Save));
                Save(new Action((Game.MsgTournaments.MsgSchedules.Tournaments[Game.MsgTournaments.TournamentType.BattleField]
                    as Game.MsgTournaments.MsgBattleField).Save));
                Save(new Action(SystemBanned.Save));
                Save(new Action(SystemBannedPC.Save));
                Save(new Action(Allow_Clicker.Save));
                Save(new Action(UnLockLogin.Save));
                Save(new Action(SystemBannedAccount.Save));
                Save(new Action(ShareVIP.Save));
                Save(new Action(InnerPowerTable.Save));
                Save(new Action(VoteSystem.Save));
                Save(new Action(LeagueTable.Save));

                Save(new Action(RechargeShop.Save));
                Save(new Action(Game.MsgTournaments.MsgSchedules.ClanWar.Save));
          
                Save(new Action (RankItems.SaveRanks));
                Save(new Action(Role.Statue.Save));

                Save(new Action(PrestigeRanking.Save));
                Save(new Action(Role.KOBoard.KOBoardRanking.Save));
                Save(new Action(MsgInterServer.Instance.CrossElitePKTournament.Save));
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        static uint ReserveMark(uint current, uint reserve)
        {
            ulong mark = (ulong)current + reserve;
            return mark > uint.MaxValue - 1 ? current : (uint)mark;
        }
        public static void Save(Action obj)
        {
            try
            {
                obj.Invoke();
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
        /// <summary>Reads portals.ini ([map] / Count= / Portal= / Destination= lines) and hands every portal to found
        /// (section map id, portal). Shared by LoadPortals and the live reload (HotReload\DataHotReload).</summary>
        public static void ParsePortals(System.IO.TextReader read, Action<ushort, Role.Portal> found)
        {
            while (true)
            {
                string lines = read.ReadLine();
                if (lines == null)
                    break;
                ushort Map = ushort.Parse(lines.Split('[')[1].ToString().Split(']')[0]);
                ushort Count = ushort.Parse(read.ReadLine().Split('=')[1]);
                for (ushort x = 0; x < Count; x++)
                {
                    Role.Portal portal = new Role.Portal();
                    string[] line = read.ReadLine().Split('=')[1].Split(' ');
                    portal.MapID = ushort.Parse(line[0]);
                    portal.X = ushort.Parse(line[1]);
                    portal.Y = ushort.Parse(line[2]);

                    string[] dline = read.ReadLine().Split('=')[1].Split(' ');
                    portal.Destiantion_MapID = ushort.Parse(dline[0]);
                    portal.Destiantion_X = ushort.Parse(dline[1]);
                    portal.Destiantion_Y = ushort.Parse(dline[2]);
                    found(Map, portal);
                }
            }
        }

        public static void LoadPortals()
        {

            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "portals.ini"))
            {
                using (System.IO.StreamReader read = System.IO.File.OpenText(Program.ServerConfig.DbLocation + "portals.ini"))
                {
                    ushort count = 0;
                    ParsePortals(read, (section, portal) =>
                    {
                        if (ServerMaps.ContainsKey(portal.MapID))
                            ServerMaps[portal.MapID].Portals.Add(portal);
                        count++;
                    });
                    try
                    {
                        read.Close();
                    }
                    catch (Exception)
                    {
                        MyConsole.WriteLine("Error Closing");
                    }
                    MyConsole.WriteLine("Loading " + count + " portals");
                }
            }
            // [FastBoot, from Qahr] was GC.Collect(): a full blocking GC of every heap on each loader; the runtime collects on its own
        }
    }
}
