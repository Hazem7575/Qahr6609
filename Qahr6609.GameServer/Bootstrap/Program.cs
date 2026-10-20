using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game;
using Conquer6609.Cryptography;
using Conquer.Shared.Cryptography;
using ProtoBuf;
using Conquer6609.Database;
using System.Net;


namespace Conquer6609
{
    using PacketInvoker = CachedAttributeInvocation<Action<Client.GameClient, ServerSockets.Packet>, PacketAttribute, ushort>;
    using System.Windows.Forms;
    using System.Net.Sockets;
    using Project2D.Database;

    class Program
    {
        public static Extensions.Time32 CurrentTime
        {
            get
            {
                return new Extensions.Time32();
            }
        }
        public static List<byte[]> LoadPackets = new List<byte[]>();
        public static ServerSockets.SocketPoll SocketsGroup;
        public static List<uint> ProtectMapSpells = new List<uint>() { 1038, Game.MsgTournaments.MsgSuperGuildWar.MapID };
        public static List<uint> MapCounterHits = new List<uint>() { 1005, 6000,5051 };
        public static bool OnMainternance = false;
        public static Conquer.Shared.Cryptography.TransferCipher transferCipher;
        public static Extensions.Time32 SaveDBStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.SaveDatabaseStamp);
        public static List<uint> RankableFamilyIds = new List<uint>() { 20300, 20160, 20070, 29370, 29360, 29300, 29363 };
        public static List<uint> NoDropItems = new List<uint>() { 1764, 700, 3954, 3820 };

        public static Game.MsgServer.AttackHandler.CheckAttack.MapIdSet FreePkMap = new Game.MsgServer.AttackHandler.CheckAttack.MapIdSet() { 20061,9800,8800,8843,10137, 10166, 3998,3071,5051, 6000, 6001,1505, 1005, 1038, 700,1508, Game.MsgTournaments.MsgSuperGuildWar.MapID, Game.MsgTournaments.MsgCaptureTheFlag.MapID,1806
        , Game.MsgTournaments.MsgTeamDeathMatch.MapID };
        public static Game.MsgServer.AttackHandler.CheckAttack.MapIdSet BlockAttackMap = new Game.MsgServer.AttackHandler.CheckAttack.MapIdSet() { 7023,3825,3830, 3831, 3832,3834,3826,3827,3828,3829,3833, 9995,1068, 4020, 4000, 4003, 4006, 4008, 4009 , 1860 ,1858, 1801, 1780, 1779/*Ghost Map*/, 9972, 1002, 3954, 3081, 1036, 1004, 1008, 601, 1006, 1511, 1039, 700, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, (uint)Game.MsgTournaments.MsgSteedRace.Maps.DungeonRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.IceRace
        ,(uint)Game.MsgTournaments.MsgSteedRace.Maps.IslandRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.LavaRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.MarketRace};
        public static List<uint> BlockTeleportMap = new List<uint>() {  601, 6000, 6001, 1005, 700, 5051 , 1858, 1860, 3852, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, 1768 };
        public static Role.Instance.Nobility.NobilityRanking NobilityRanking = new Role.Instance.Nobility.NobilityRanking();
        public static Role.Instance.ChiRank ChiRanking = new Role.Instance.ChiRank();

        public static Role.Instance.Flowers.FlowersRankingToday FlowersRankToday = new Role.Instance.Flowers.FlowersRankingToday();
        public static Role.Instance.Flowers.FlowerRanking GirlsFlowersRanking = new Role.Instance.Flowers.FlowerRanking();
        public static Role.Instance.Flowers.FlowerRanking BoysFlowersRanking = new Role.Instance.Flowers.FlowerRanking(false);

        public static ShowChatItems GlobalItems;
        public static SendGlobalPacket SendGlobalPackets;
        public static PacketInvoker MsgInvoker;
        public static ServerSockets.ServerSocket GameServer;

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleHandlerDelegate handler, bool add);
        private delegate bool ConsoleHandlerDelegate(int type);
        private static ConsoleHandlerDelegate handlerKeepAlive;

        public  static bool ProcessConsoleEvent(int type)
        {
            try
            {
                if (ServerConfig.IsInterServer)
                {
                    foreach (var client in Database.Server.GamePoll.Values)
                    {
                        try
                        {
                            if (client.Socket != null)
                                client.Socket.Disconnect();
                        }
                        catch (Exception e)
                        {
                            MyConsole.WriteLine(e.ToString());
                        }
                    }
                    return true;
                }
                try { WebApi.Stop(); } catch { }// website API: no more CPs/items once the shutdown save starts
                try
                {
                    if (GameServer != null)
                        GameServer.Close();

                   
                }
              catch (Exception e) { MyConsole.SaveException(e); }
                MyConsole.WriteLine("Saving Database...");
         
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    try
                    {
                        if (client.Socket != null)
                            client.Socket.Disconnect();
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteLine(e.ToString());
                    }
                }
                // Windows gives a closing console (CTRL_CLOSE/LOGOFF/SHUTDOWN) only about 5 seconds, so the waits
                // are short there; a Ctrl+C / Ctrl+Break waits until every queued save has finished.
                // Saves are atomic (tmp + replace), so a save cut off by the OS keeps the previous file.
                bool closing = type == 2 || type == 5 || type == 6;
                int waitMs = closing ? 1500 : 10000;

                // Disconnect handlers queue the player saves; wait until they all did so.
                // Fake clients (bots without a socket) never go through a disconnect, so they are not waited for.
                var waitDisconnects = System.Diagnostics.Stopwatch.StartNew();
                while (waitDisconnects.ElapsedMilliseconds < waitMs &&
                    Database.Server.GamePoll.Values.Any(p => p != null && !p.Fake && p.Socket != null && (p.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull))
                    System.Threading.Thread.Sleep(10);

                // Player data first, it matters most if the process gets killed.
                if (closing)
                    Database.ServerDatabase.FinishPlayerSaves(2500);

                Role.Instance.Clan.ProcessChangeNames();

                Conquer6609.Framework.Modules.ModuleManager.Shutdown();
                // waits for a background SaveDatabase that is still running, then saves again (synchronously)
                Database.Server.SaveDatabase();
                if (Database.ServerDatabase.FinishPlayerSaves(closing ? 1000 : 120000))
                {
                    if (!closing)
                        System.Threading.Thread.Sleep(1000);
                    MyConsole.WriteLine("Database Save Succefull.");
                }
                else
                    MyConsole.WriteLine("Database save may be incomplete (saves still running).");
                // [Storage] then the MySQL copy of those saves ([Storage] AsyncMirrorFlushSec, the files are already saved)
                if (!closing)
                    Database.Storage.MirrorQueue.Flush();
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            return true;
        }

          public static Extensions.Time32 ResetRandom = new Extensions.Time32();

          public static Extensions.SafeRandom GetRandom = new Extensions.SafeRandom();
        public static Extensions.RandomLite LiteRandom = new Extensions.RandomLite();

        public static class ServerConfig
        {
            public static string CO2Folder = "";
            public static string XtremeTopLink = "";
            public static string FBStoreLink = "";
            public static string RedeemLink = "";

            public static uint ServerID = 0;
            public static string LogginKey = "";
            public static string IPAddres = "";
            public static ushort AuthPort = 9961;
            public static ushort GamePort = 5816;
            public static string ServerName = "";

            public static string OfficialWebSite = "";
            //InternetPort
            public static ushort Port_BackLog;
            public static ushort Port_ReceiveSize = 16384;
            public static ushort Port_SendSize = 16384;
            //Database
            public static string DbLocation = "";
            public static uint ExpRateSpell { get { return Rates.Spell; } }// shell.ini [Rates] (Infrastructure\Extensions\Rates.cs)
            public static uint ExpRateProf { get { return Rates.Prof; } }
            public static uint UserExpRate { get { return Rates.Exp; } }// official x [Rates] Exp (was 50000)
            public static uint PhysicalDamage = 100;// + 150%
            //interServer
            public static string InterServerAddress = "";
            public static ushort InterServerPort = 0;
            public static bool IsInterServer = false;
        }

        public static unsafe void Main(string[] args)
        {
            // [net10] legacy ANSI code pages (1252/1256/...) used by AnsiText, IniFile and the quest files only exist
            // on .NET 5+ after this registration. Must run before anything touches them.
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Console first: no QuickEdit freezes, and Console.Write only enqueues from here on.
            try { MyConsole.DisableQuickEdit(); }
            catch { }
            ConsoleUi.Prepare();
            ConsoleQueue.Install();
            string serverName = Conquer6609.Framework.Settings.String("ServerInfo", "ServerName", "Hazem");
            // loading bar + summary like Qahr's console (shell.ini [Console] BootScreen=0 = the old scrolling banner)
            BootScreen.Begin("Conquer Online Server  ·  Version 6609", serverName + "  ·  .NET " + Environment.Version + (IntPtr.Size == 4 ? " x86" : " x64"));
            if (!BootScreen.Active)
                ConsoleUi.Banner(serverName);
            ServerSockets.SocketPoll.ApplyTimerResolution();
            try
            {
                // first CAST5 key of the game connection; it must match the one compiled into the client's shield dll (shell.ini [Security] LoginKey)
                ServerConfig.LogginKey = Conquer6609.Framework.Settings.String("Security", "LoginKey", ServerConfig.LogginKey);
                if (string.IsNullOrEmpty(ServerConfig.LogginKey))
                    MyConsole.WriteLine("shell.ini [Security] LoginKey is missing - clients cannot log in.", ConsoleColor.Red);
                GShield._GShield.LoadFilesAntiCheat();
                    ServerSockets.Packet.SealString = "TQServer";
                    //MyConsole.WriteLine("My IP:" + MyIP);
                    Console.ForegroundColor = ConsoleColor.Green;
                    MsgInvoker = new PacketInvoker(PacketAttribute.Translator);
                    Cryptography.DHKeyExchange.KeyExchange.CreateKeys();
                    Game.MsgTournaments.MsgSchedules.Create();
                    WindowsAPI.IniCache.Begin();// .ini files are read from memory while the database loads
                    // [FastBoot, from Qahr] shell.ini [Performance] FastBoot (default 1): boot-only speed-ups, 0 = the old start-up
                    bool fastBoot = Conquer6609.Framework.Settings.Bool("Performance", "FastBoot", true);
                    WindowsAPI.IniCache.TrustRecent = fastBoot;// no file stat per ini read while loading; End() switches it off
                    try
                    {
                    Database.Server.Initialize();
                    Game.MsgServer.LoginTokenCheck.CheckTableAtStartup();// loud error if login_tokens is missing
                    SendGlobalPackets = new SendGlobalPacket();
                    Cryptography.AuthCryptography.PrepareAuthCryptography();
                    Database.Server.LoadDatabase();
                    }
                    finally
                    {
                        WindowsAPI.IniCache.End();
                    }
                    handlerKeepAlive = ProcessConsoleEvent;
                    SetConsoleCtrlHandler(handlerKeepAlive, true);
                //
                    // [Security] TransferKey / TransferSalt: no built-in default any more; empty = a random value per start
                    // (the transfer cipher has no callers since the login handoff uses a random token).
                    string transferKey = Conquer6609.Framework.Settings.String("Security", "TransferKey", "");
                    string transferSalt = Conquer6609.Framework.Settings.String("Security", "TransferSalt", "");
                    if (string.IsNullOrEmpty(transferKey)) transferKey = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
                    if (string.IsNullOrEmpty(transferSalt)) transferSalt = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
                    TransferCipher.Key = Encoding.ASCII.GetBytes(transferKey);
                    TransferCipher.Salt = Encoding.ASCII.GetBytes(transferSalt);
                    Program.transferCipher = new TransferCipher("127.0.0.1");
                ///
                    if (ServerConfig.IsInterServer == false)
                    {
                        GameServer = new ServerSockets.ServerSocket(
                            new Action<ServerSockets.SecuritySocket>(p => new Client.GameClient(p))
                            , Game_Receive, Game_Disconnect);
                        GameServer.Initilize(ServerConfig.Port_SendSize, ServerConfig.Port_ReceiveSize, 1, 3);
                        GameServer.Open(ServerConfig.IPAddres, ServerConfig.GamePort, ServerConfig.Port_BackLog);


                    }
                    GlobalItems = new ShowChatItems();
                    Database.NpcServer.LoadServerTraps();
                    MsgInterServer.PipeServer.Initialize();
                    ThreadPoll.Create();
                    //SocketsGroup = new ServerSockets.SocketPoll("ConquerServer", GameServer, MsgInterServer.PipeServer.Server);
                    //SocketsGroup.Start();
                  
                    //MsgInterServer.StaticConnexion.Create();
                    // the pipe listener (Realm, or PipeEnabled=1 with a usable secret) is polled with the game port; null = off
                    SocketsGroup = new ServerSockets.SocketPoll("ConquerServer", GameServer, MsgInterServer.PipeServer.Server);
                    new Catching();
                    Game.MsgTournaments.MsgSchedules.ClanWar = new Game.MsgTournaments.MsgClanWar();
                    new KernelThread(1000, "ConquerServer2").Start();
                    new MapGroupThread(300, "ConquerServer3").Start();
                    HotReload.HotReloadManager.Start();// optional auto-reload of edited data files (ServerDataBase\hotreload.txt)
                    long ramMb = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
                    var onlineRows = new[]
                    {
                        ConsoleUi.Row("Game", ServerConfig.IPAddres + ":" + ServerConfig.GamePort),
                        ConsoleUi.Row("Started", (DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime).TotalSeconds.ToString("0.0") + " s"),
                        ConsoleUi.Row("Maps", Database.Server.ServerMaps.Count),
                        ConsoleUi.Row("Items", Database.Server.ItemsBase.Count),
                        ConsoleUi.Row("Monsters", Database.Server.MonsterFamilies.Count + " types"),
                        ConsoleUi.Row("RAM", ramMb + " MB"),
                        ConsoleUi.Row("Name", ServerConfig.ServerName),
                        ConsoleUi.Row("Console", "tool | mode | save | clear | Backup | reloadnpcs ... reloadall")
                    };
                    if (BootScreen.Active)
                        BootScreen.Finish("Server is ONLINE  ·  port " + ServerConfig.GamePort, onlineRows);
                    else
                        ConsoleUi.OnlineBox(onlineRows);

            }
            catch (Exception e)
            {
                BootScreen.Abort();// give the console back and show what was written before the failure
                MyConsole.WriteException(e); 
            }
            AdminPipe.Start();// local graceful stop/save/status for Qahr6609.Tools\Server\Cluster ([Admin] Pipe=0 = off)
            AdminApi.Start();// live API for the released tools (MapStudio apply live ...), ServerDataBase\adminapi.txt (no file = off)
            WebApi.Start();// website API (send CPs / give item / bag), ServerDataBase\webapi.ini, OFF by default (Infrastructure\Threading\WebApi.cs)
            ServerMode.Start();// ServerDataBase\servermode.txt: development = PacketStudio stream/injection on, production (default) = off
            for (; ; )
                ConsoleCMD(MyConsole.ReadLine());
        }

        static int playerSaveStagger = -1;
        /// <summary>KernelThread (every second). Player saves: with [Performance] PlayerSaveStagger=1 (default)
        /// each online character is queued on its own schedule (every PlayerSaveIntervalSec, spread by UID);
        /// with 0 every character is queued at once every 250 s (old behaviour). The global SaveDatabase runs
        /// every 250 s on a background thread, so this thread never waits for the disk.</summary>
        public static void SaveDBPayers(Extensions.Time32 clock)
        {
            if (playerSaveStagger < 0)
                playerSaveStagger = Conquer6609.Framework.Settings.Bool("Performance", "PlayerSaveStagger", true) ? 1 : 0;
            bool active = Database.Server.FullLoading && !Program.ServerConfig.IsInterServer;
            if (active && playerSaveStagger == 1)
                Database.ServerDatabase.QueueDuePlayerSaves();

            if (clock > SaveDBStamp)
            {
                if (active)
                {
                    if (playerSaveStagger == 0)
                        Database.ServerDatabase.QueueAllPlayerSaves(false);
                    Database.Server.SaveDatabaseInBackground();
                }
                SaveDBStamp.Value = clock.Value + KernelThread.SaveDatabaseStamp;
            }

        }
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
               // Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
       
        
        public unsafe static void ConsoleCMD(string cmd)
        {
            try
            {
                string[] line = cmd.Split(' ');

                // live reload of the data files (Infrastructure\HotReload): reloadnpcs, reloadportals, ... reloadall [force]
                if (HotReload.HotReloadManager.IsReloadOp(line[0]))
                {
                    HotReload.HotReloadManager.Run(line[0], line.Length > 1 ? line[1] : null, line.Skip(1).Any(a => a == "force"));
                    return;
                }

                switch (line[0])
                {
                    case "tool":// the tools of Qahr6609.Tools\Releases (Infrastructure\ServerTools\ToolLauncher.cs)
                    case "tools":
                        ServerTools.ToolLauncher.Console(line);
                        break;
                    case "module":// module reload <Name>: hot reload of a runtime module (Modules\ModuleManager.cs ReloadRuntime)
                    case "modules":
                        {
                            if (line.Length > 2 && line[1] == "reload")
                                Framework.Modules.ModuleManager.ReloadRuntime(line[2]);// prints its result
                            else
                            {
                                foreach (var m in Framework.Modules.ModuleManager.Loaded)
                                    MyConsole.WriteLine("  " + m.Name + " v" + m.Version + " [" + m.Status + "]" + (Framework.Modules.RuntimeModules.Find(m.Name) != null ? " runtime" : ""));
                                MyConsole.WriteLine("[Modules] module reload <Name>  (runtime modules in ServerDataBase\\Modules; development mode reloads them by itself when a .cs file is saved)");
                            }
                            break;
                        }
                    case "webapi":// website API: webapi status | genkey | test (Infrastructure\Threading\WebApi.cs)
                        WebApi.Console(line);
                        break;
                    case "mode":// server run mode (Infrastructure\Extensions\ServerMode.cs)
                        {
                            if (line.Length > 1 && (line[1] == "dev" || line[1] == "development")) ServerMode.Set(true);
                            else if (line.Length > 1 && (line[1] == "prod" || line[1] == "production")) ServerMode.Set(false);
                            MyConsole.WriteLine("[Mode] " + ServerMode.Current + "   (mode dev | mode prod)");
                            break;
                        }
                    case "hazem":
                        {
                            AdminTool.HazemPanel cp = new AdminTool.HazemPanel();
                            cp.ShowDialog();
                            break;
                        }
                    case "cp":
                        {
#if HAS_DEVEXPRESS
                            Co_Panle cp = new Co_Panle();
                            cp.ShowDialog();
#else
                            // optional DevExpress GM panel: put the DevExpress v21.2 dlls in libs\ and rebuild (README.md)
                            MyConsole.WriteLine("[cp] panel not installed (DevExpress v21.2 dlls are not in libs\\)");
#endif
                            break;
                        }
                    case "Backup":
                        {
                            try
                            {
                                string create = Program.ServerConfig.DbLocation + "\\AABackUP\\" + DateTime.Now.Year + " - " + DateTime.Now.Month + " - " + DateTime.Now.Day + " ";
                                string createUsers = create + "\\Users";
                                string createspells = create + "\\PlayersSpells";
                                string createprofs = create + "\\PlayersProfs";
                                string createitems = create + "\\PlayersItems";
                                string createquests = create + "\\Quests";
                                string createhouses = create + "\\Houses";
                                string createclans = create + "\\Clans";
                                string createguilds = create + "\\Guilds";
                                string createunions = create + "\\Unions";
                                string all =  createUsers + createspells + createprofs + createitems + createquests + createhouses + createclans + createguilds +createunions;
                                try
                                {
                                    if (!Directory.Exists(create))
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(create);
                                        DirectoryInfo di2 = Directory.CreateDirectory(createUsers);
                                        DirectoryInfo di3 = Directory.CreateDirectory(createspells);
                                        DirectoryInfo di4 = Directory.CreateDirectory(createprofs);
                                        DirectoryInfo di5 = Directory.CreateDirectory(createitems);
                                        DirectoryInfo di6 = Directory.CreateDirectory(createquests);
                                        DirectoryInfo di7 = Directory.CreateDirectory(createhouses);
                                        DirectoryInfo di8 = Directory.CreateDirectory(createclans);
                                        DirectoryInfo di9 = Directory.CreateDirectory(createguilds);
                                        DirectoryInfo di0 = Directory.CreateDirectory(createunions);
                                        File.Copy(Program.ServerConfig.DbLocation + "\\JiangHu.txt", create + "\\JiangHu.txt", true);
                                        File.Copy(Program.ServerConfig.DbLocation + "\\PrestigeRanking.txt", create + "\\PrestigeRanking.txt", true);
                                        File.Copy(Program.ServerConfig.DbLocation + "\\InnerPower.txt", create + "\\InnerPower.txt", true);
                                        Copy(Program.ServerConfig.DbLocation + "\\Users", createUsers);
                                        Copy(Program.ServerConfig.DbLocation + "\\PlayersSpells", createspells);
                                        Copy(Program.ServerConfig.DbLocation + "\\PlayersProfs", createprofs);
                                        Copy(Program.ServerConfig.DbLocation + "\\PlayersItems", createitems);
                                        Copy(Program.ServerConfig.DbLocation + "\\Quests", createquests);
                                        Copy(Program.ServerConfig.DbLocation + "\\Houses", createhouses);
                                        Copy(Program.ServerConfig.DbLocation + "\\Clans", createclans);
                                        Copy(Program.ServerConfig.DbLocation + "\\Guilds", createguilds);
                                        Copy(Program.ServerConfig.DbLocation + "\\Unions", createunions);
                                        MyConsole.WriteLine("Done BackUp Database For today ( " + DateTime.Now.Year + " - " + DateTime.Now.Month + " - " + DateTime.Now.Day + " ) ");
                                        return;
                                       
                                    }
                                    else
                                    {
                                        MyConsole.WriteLine("" + create + " Already BackedUp once !");
                                        return;
                                    
                                    }
                                }
                                catch (IOException ioex)
                                {
                                    Console.WriteLine(ioex.Message);
                                }
                               
                              
                            }
                            catch (Exception e) { Console.WriteLine(e.ToString()); }
                            break;
                        }
               
                
                
                    case "clear":
                        {
                            Console.Clear();
                            break;
                        }
                    case "cpst":
                        {
                            if (line.Length < 2)
                            {
                                MyConsole.WriteLine("Message Can't be Deququed.");
                                break;
                            }
                            uint Bet = Convert.ToUInt32(line[1]);
                            foreach (var user in Server.GamePoll.Values)
                            {
                                user.Player.MessageBox("Cps Tournment War Started Bet " + Bet + " Cps", p =>
                                {
                                    if (Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.SignUp(p, Bet) == false)
                                    {
                                        p.SendSysMesage("don't have requierments.", MsgMessage.ChatMode.Service, MsgMessage.MsgColor.yellow);
                                    }
                                }, null, 15);
                            }
                            break;
                        }
                    case "goldentree":
                        MyConsole.WriteLine("The Golden Tree is a module now: in game @goldentree start <hours> [max picks] | stop | status (Modules\\GoldenTree).");
                        break;
                    case "powerarena":
                        {
                            Game.MsgTournaments.MsgSchedules.PowerArena.Start();
                            break;
                        }
                    case "squidward":
                        {
                            Game.MsgTournaments.MsgSchedules.SquidwardOctopus.Start();
                            break;
                        }
                    case "save":
                        {
                            Database.Server.SaveDatabase();// waits for a running background save first
                            if (Database.Server.FullLoading && !Program.ServerConfig.IsInterServer)
                            {
                                Database.ServerDatabase.QueueAllPlayerSaves(false);
                                MyConsole.WriteLine("Database got saved ! ");
                            }
                            if (Database.ServerDatabase.FinishPlayerSaves(-1))
                            {
                                System.Threading.Thread.Sleep(1000);
                                MyConsole.WriteLine("Database saved successfully.");
                            }
                            break;
                        }
              
                    case "steed":
                        {
                            Game.MsgTournaments.MsgSchedules.SteedRace.Create();
                            break;
                        }
                    case "ctfon":
                        {
                            Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Start();
                            break;
                        }
                    case "teamon":
                        {
                            Game.MsgTournaments.MsgSchedules.TeamPkTournament.Start();
                            break;
                        }
                    case "kick":
                        {

                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                if (user.Player.Name.Contains(line[1]))
                                {
                                    user.EndQualifier();
                                }
                            }
                            break;
                        }

                    case "pk":
                        {
                            Game.MsgTournaments.MsgSchedules.ElitePkTournament.Start();

                            foreach (var clients in Database.Server.GamePoll.Values)
                            {
                                Game.MsgTournaments.MsgSchedules.ElitePkTournament.SignUp(clients);
                            }
                            break;
                        }   
                    case "teampk":
                        {
                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Start();
                            var array = Database.Server.GamePoll.Values.ToArray();


                            for (int x = 0; x < array.Length - 5; x += 5)
                            {
                                if (array[x].Team == null)
                                {
                                    try
                                    {
                                        array[x].Team = new Role.Instance.Team(array[x]);
                                        Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x]);
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            array[x + 1].Team = array[0].Team;
                                            array[x].Team.Add(stream, array[x + 1]);
                                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x + 1]);

                                            array[x + 2].Team = array[0].Team;
                                            array[x].Team.Add(stream, array[x + 2]);
                                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x + 2]);

                                            array[x + 3].Team = array[0].Team;
                                            array[x].Team.Add(stream, array[x + 3]);
                                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x + 3]);

                                            array[x + 4].Team = array[0].Team;
                                            array[x].Team.Add(stream, array[x + 4]);
                                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x + 4]);
                                        }

                                    }
                                    catch { }
                                }
                            }
                            break;
                        }
                    case "search":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                        
                                string Name = ini.ReadString("Character", "Name", "None");
                                if (Name.ToLower() == line[1].ToLower() || Name.Contains(line[1]))
                                {
                                    Console.WriteLine(ini.ReadUInt32("Character", "UID", 0));
                                    break;
                                }
                               
                            }
                            break;
                        }
                    case "resetno"://resetnobility
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong nobility = ini.ReadUInt64("Character", "DonationNobility", 0);
                                nobility = nobility * 50 / 100;
                             
                                ini.Write<ulong>("Character", "DonationNobility", nobility);
                            }

                            break;
                        }
                    case "check":
                        {
                              WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                              foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                              {
                                  ini.FileName = fname;

                                  long nobility = ini.ReadInt64("Character", "Money", 0);
                                  if (nobility < 0)
                                  {
                                      Console.WriteLine("");
                                  }

                              }
                            break;
                        }
                    case "fixedgamemap":
                        {
                            Dictionary<int, string> maps = new Dictionary<int, string>();
                            using (var gamemap = new BinaryReader(new FileStream(Path.Combine(Program.ServerConfig.CO2Folder, "ini/gamemap.dat"), FileMode.Open)))
                            {

                                var amount = gamemap.ReadInt32();
                                for (var i = 0; i < amount; i++)
                                {

                                    var id = gamemap.ReadInt32();
                                    var fileName = Encoding.ASCII.GetString(gamemap.ReadBytes(gamemap.ReadInt32()));
                                    var puzzleSize = gamemap.ReadInt32();
                                    if (id == 1017)
                                    {
                                        Console.WriteLine(puzzleSize);
                                    }
                                    if (!maps.ContainsKey(id))
                                        maps.Add(id, fileName);
                                    else 
                                        maps[id] = fileName;
                                }
                            }
                            break;
                        }
                 

                    case "startgw":
                        {
                            Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Alive;
                            Game.MsgTournaments.MsgSchedules.GuildWar.Start();
                            break;
                        }
                    case "startsgw":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Start();
                            break;
                        }
                    case "finishsgw":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.CompleteEndGuildWar();
                            break;
                        }
                    case "finishgw":
                        {
                            Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.GuildWar.CompleteEndGuildWar();
                            break;
                        }
                  
                    case "exit":
                        {
                            new Thread(new ThreadStart(Maintenance)).Start();
                            break;
                        }
                    case "forceexit":
                        {
                            ProcessConsoleEvent(0);

                            Environment.Exit(0);
                            break;
                        }
                    case "restart":
                        {
                            ProcessConsoleEvent(0);

                            System.Diagnostics.Process hproces = new System.Diagnostics.Process();
                            hproces.StartInfo.FileName = System.Reflection.Assembly.GetEntryAssembly().Location;
                            hproces.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                            hproces.Start();

                            Environment.Exit(0);

                            break;
                        }
                  
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        public static void Maintenance()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                OnMainternance = true;
                MyConsole.WriteLine("The server will be brought down for maintenance in (5 Minutes). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (5 Minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (4 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (4 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (4 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (4 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (3 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (3 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (3 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (3 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (2 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (2 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (2 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (2 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (1 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (1 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (1 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (1 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (0 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (0 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 20);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MsgMessage msg = new MsgMessage("Server maintenance(few minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 10);
            ProcessConsoleEvent(0);

            Environment.Exit(0);
        }

        // PacketStudio connector (libs\PacketBridge.dll, Qahr6609.Tools\Releases\PacketStudio\INTEGRATION_EN.txt):
        // started only in development mode (ServerMode). packetbridge.ini next to the exe (enabled=1, secret=...) streams every plaintext packet to the tool on 127.0.0.1,
        // and with devmode=1 + allow= lets the tool send a test packet to an allowed test character. No file = off.
        public static void StartPacketBridge()
        {
            try
            {
                PacketStudio.Bridge.PacketBridge.SetLogger(line => MyConsole.WriteLine(line));
                PacketStudio.Bridge.PacketBridge.SetPlayerListProvider(() => Database.Server.GamePoll.Values
                    .Where(c => c != null && c.Player != null)
                    .Select(c => new KeyValuePair<uint, string>(c.Player.UID, c.Player.Name)).ToList());
                PacketStudio.Bridge.PacketBridge.SetInjector((uid, packet, sealAppended) =>
                {
                    Client.GameClient client;
                    if (!Database.Server.GamePoll.TryGetValue(uid, out client) || client == null || client.Player == null)
                        return false;
                    client.Send(packet);
                    return true;
                });
                PacketStudio.Bridge.PacketBridge.SetDevModeCheck(() => ServerMode.IsDevelopment);// injection only in development
                PacketStudio.Bridge.PacketBridge.Start("packetbridge.ini");
            }
            catch (Exception e) { MyConsole.WriteLine("[PacketBridge] not started: " + e.Message); }
        }

        // [Security] before the account server's token is accepted (AcceptLogin) or character creation is opened, a connection
        // may only send the login / shield / PC-info packets (every handler was reachable right after the key exchange:
        // e.g. world chat with no sender from unauthenticated connections). shell.ini [Security] PreLoginGate=0 turns it off.
        static readonly bool PreLoginGate = Conquer6609.Framework.Settings.Bool("Security", "PreLoginGate", true);
        const Client.ServerFlag LoggedInFlags = Client.ServerFlag.AcceptLogin | Client.ServerFlag.CreateCharacter
            | Client.ServerFlag.CreateCharacterSucces | Client.ServerFlag.LoginFull;
        static bool PreLoginAllowed(Client.GameClient client, ushort packetId)
        {
            if (!PreLoginGate || (client.ClientFlag & LoggedInFlags) != Client.ServerFlag.None || client.Player.CheckTransfer || client.PipeClient != null)
                return true;
            switch (packetId)
            {
                case 10055:// CMsgRequestLogin (login token)
                case 10050:// CMsgShield
                case 10053:// CMsgMemoryCheck
                case 10057:// MsgSendPCInfo
                case Game.GamePackets.MsgPCNum:
                case Game.GamePackets.LoginGame:
                    return true;
            }
            if (Conquer6609.Security.Guard.Throttle("prelogin-drop-" + packetId, 60000))
                Conquer6609.Security.Guard.Audit("prelogin-drop", 0, "", "packet " + packetId + " before login ip=" + (client.Socket != null ? client.Socket.RemoteIp : ""));
            return false;
        }

        public unsafe static void Game_Receive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)//ServerSockets.Packet data)
        {
            if (!obj.SetDHKey)
                CreateDHKey(obj, stream);
            else
            {
                try
                {
                    if (obj.Game == null)
                        return;
                    ushort PacketID = stream.ReadUInt16();
                    if (PacketStudio.Bridge.PacketBridge.IsActive && obj.Game.Player != null)
                        PacketStudio.Bridge.PacketBridge.OnClientToServer(obj.Game.Player.UID, obj.Game.Player.Name, stream.Memory, stream.Size);

                    // Per packet type flood limits: excess packets are dropped silently (never a disconnect).
                    if (!obj.AllowPacketType(PacketID, stream))
                        return;
                    if (!PreLoginAllowed(obj.Game, PacketID))
                        return;
                    if (obj.Game.Player.CheckTransfer)
                        goto jmp;
                    if (obj.Game.PipeClient != null && PacketID != Game.GamePackets.Achievement)
                    {
                        if (PacketID == (ushort)Game.GamePackets.MsgOsShop
             || PacketID == (ushort)Game.GamePackets.SecondaryPassword
                      || PacketID >= (ushort)Game.GamePackets.LeagueOpt && PacketID <= (ushort)Game.GamePackets.LeagueConcubines
                      || PacketID == (ushort)Game.GamePackets.LeagueRobOpt)
                            goto jmp;

                        // pipe v2: only real client messages are relayed, inside a RelayClient envelope (never raw:
                        // a raw 1006/1008/10017/99/113... would be read as server data by the Realm)
                        if (!MsgInterServer.PipeFilter.IsClientRelayAllowed(PacketID))
                        {
                            MsgInterServer.PipeFilter.AuditDrop(obj.Game.Player.UID, obj.Game.Player.Name, "client packet " + PacketID + " not relayed to the Realm (not an allowed client message)");
                            return;
                        }
                        obj.Game.PipeClient.SendClientPacket(stream);

                        // 1009 runs on the Realm only (it used to run here too: drop/sell/equip twice). Only the
                        // Ping sub-action is also answered at home (keeps the home connection's ping/time check).
                        if (PacketID != Game.GamePackets.Usage || stream.Size < 22 || *(ushort*)(stream.Memory + 20) != (ushort)MsgItemUsuagePacket.ItemUsuageID.Ping)
                            return;
                        stream.Seek(4);
                    }


                jmp:
                  
                        Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                        Conquer6609.Modules.PacketAudit.PacketAuditModule.Incoming(obj.Game, PacketID, stream);
                        if (MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                        {
                            hinvoker(obj.Game, stream);
                        }
                        else
                        {
                            Conquer6609.Modules.PacketAudit.PacketAuditModule.Unhandled(obj.Game, PacketID, stream);
                        }


                }
                catch (Exception e)
                {
                    MyConsole.WriteException(e);
                    // A client whose packets keep crashing handlers is disconnected.
                    obj.CountHandlerException();
                }
                finally
                {
                    ServerSockets.PacketRecycle.Reuse(stream);
                }
            }

        }
        public static void DirectoryCopy(
       string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public unsafe static void CreateDHKey(ServerSockets.SecuritySocket obj, ServerSockets.Packet Stream)
        {
            try
            {
                byte[] buffer = new byte[36];
                bool extra = false;
                string text = System.Text.ASCIIEncoding.ASCII.GetString(obj.DHKeyBuffer.buffer, 0, obj.DHKeyBuffer.Length());
                if (!text.EndsWith("TQClient"))
                {
                    System.Buffer.BlockCopy(obj.EncryptedDHKeyBuffer.buffer, obj.EncryptedDHKeyBuffer.Length() - 36, buffer, 0, 36);
                    extra = true;
                }

                string key;
                if (Stream.GetHandshakeReplyKey(out key))
                {
                    bool keyOk;
                    try { keyOk = obj.Game.DHKey.HandleResponse(key); }
                    catch (Exception) { keyOk = false; }// not a hex number
                    if (!keyOk)
                    {
                        obj.Disconnect();
                        return;
                    }
                    obj.SetDHKey = true;
                    var compute_key = obj.Game.DHKeyExchance.PostProcessDHKey(obj.Game.DHKey.ToBytes());
                    obj.Game.Crypto.GenerateKey(compute_key);
                    obj.Game.Crypto.Reset();
                }
                else
                {
                    obj.Disconnect();
                    return;
                }
                if (extra)
                {

                    Stream.Seek(0);
                    obj.Game.Crypto.Decrypt(buffer, 0, Stream.Memory, 0, 36);
                    Stream.Size = buffer.Length;
                 

                  
               

                    Stream.Size = buffer.Length;
                    Stream.Seek(2);
                    ushort PacketID = Stream.ReadUInt16();
                    Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                    if (MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                    {
                        hinvoker(obj.Game, Stream);
                    }
                    else
                    {
                        obj.Disconnect();

                        MyConsole.WriteLine("DH KEY Not found the packet ----> " + PacketID);

                    }
                }
                
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        public unsafe static void Game_Disconnect(ServerSockets.SecuritySocket obj)
        {
           
            if (obj.Game != null && obj.Game.Player != null)
            {
                try
                {
               
                    Client.GameClient client;
                    if (Database.Server.GamePoll.TryGetValue(obj.Game.Player.UID, out client))
                    {
                        
                        // forget per-uid security state before the inter-server early return too
                        if (client == obj.Game)
                            Security.Guard.ForgetPlayer(client.Player.UID);
                        if (client.OnInterServer)
                            return;
                        if ((client.ClientFlag & Client.ServerFlag.LoginFull) != Client.ServerFlag.LoginFull && client == obj.Game)
                        {
                            // The character never finished logging in (or its load was refused): don't leave it in GamePoll.
                            if (client.FullLoading)
                            {
                                client.ClientFlag |= Client.ServerFlag.Disconnect;
                                client.ClientFlag |= Client.ServerFlag.QueuesSave;
                                Database.ServerDatabase.LoginQueue.TryEnqueue(client);//SaveClient removes it from GamePoll
                            }
                            else
                            {
                                ((ICollection<KeyValuePair<uint, Client.GameClient>>)Database.Server.GamePoll).Remove(new KeyValuePair<uint, Client.GameClient>(client.Player.UID, client));
                                if (client.Map != null)
                                    client.Map.Denquer(client);
                            }
                        }
                        else if ((client.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                        {
                            if (obj.Game.PipeClient != null)
                                obj.Game.PipeClient.Disconnect();
                            //#region Update Accuont
                            //var cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                            //cmd.Update("accounts").Set("Online", 0).Where("EntityID", client.Player.UID).Execute2();
                            //#endregion
                            Console.ForegroundColor = ConsoleColor.White;
                            MyConsole.WriteLine("-------------------------------------------------");
                            MyConsole.WriteLine("Client " + client.Player.Name + " was loggin out.", ConsoleColor.Red);
                            MyConsole.WriteLine("-------------------------------------------------");
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Conquer6609.ServerSockets.LoginController.Disconnect(obj.RemoteAddress);// address captured at connect: the socket is already closed here
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                try
                                {
                                    if (client.Player.InUnion)
                                    {
                                        client.Player.UnionMemeber.Owner = null;
                                    }
                                   
                                    client.EndQualifier();

                                   


                                    try { Conquer6609.Framework.Modules.ModuleManager.Logout(client); } catch (Exception e) { MyConsole.SaveException(e); }
                                    if (client.Team != null)
                                        client.Team.Remove(client, true);
                                    if (client.Player.MyClanMember != null)
                                        client.Player.MyClanMember.Online = false;
                                    if (client.IsVendor)
                                        client.MyVendor.StopVending(stream);
                                    // read the trade once: the partner may close it (and null it) concurrently
                                    var trade = client.MyTrade;
                                    if (trade != null && trade.WindowOpen)
                                        trade.CloseTrade();
                                    if (client.Player.MyGuildMember != null)
                                        client.Player.MyGuildMember.IsOnline = false;

                                    if (client.Player.ObjInteraction != null)
                                    {
                                        client.Player.InteractionEffect.AtkType = Game.MsgServer.MsgAttackPacket.AttackID.InteractionStopEffect;

                                        InteractQuery action = InteractQuery.ShallowCopy(client.Player.InteractionEffect);

                                        client.Send(stream.InteractionCreate(&action));

                                        client.Player.ObjInteraction.Player.OnInteractionEffect = false;
                                        client.Player.ObjInteraction.Player.ObjInteraction = null;
                                    }

                                    
                                    client.Player.View.Clear(stream);
                                 

                                }
                                catch (Exception e)
                                {
                                    MyConsole.WriteException(e);
                                    client.Player.View.Clear(stream);
                                }
                                finally
                                {
                                    client.ClientFlag &= ~Client.ServerFlag.LoginFull;
                                    client.ClientFlag |= Client.ServerFlag.Disconnect;
                                    client.ClientFlag |= Client.ServerFlag.QueuesSave;
                                    client.Player.UpdateSurroundings(stream, true, true);
                                    Database.ServerDatabase.LoginQueue.TryEnqueue(client);
                                }

                                try
                                {
                                    Database.Roulettes.OnDisconnect(client);// leave roulette tables (player + spectator)
                                }
                                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

                                try
                                {
                                    client.Player.Associate.OnDisconnect(stream, client);

                                    //remove mentor and apprentice
                                    if (client.Player.MyMentor != null)
                                    {
                                        Client.GameClient me;
                                        client.Player.MyMentor.OnlineApprentice.TryRemove(client.Player.UID, out me);
                                        client.Player.MyMentor = null;
                                    }
                                    client.Player.Associate.Online = false;
                                    lock (client.Player.Associate.MyClient)
                                        client.Player.Associate.MyClient = null;
                                    foreach (var clien in client.Player.Associate.OnlineApprentice.Values)
                                        clien.Player.SetMentorBattlePowers(0, 0);
                                    client.Player.Associate.OnlineApprentice.Clear();
                                    //done remove
                                }
                                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                            }
                        }
                    }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }
            else if (obj.Game != null)
            {
                if (obj.Game.ConnectionUID != 0)
                {
                   
                    Client.GameClient client;
                    Database.Server.GamePoll.TryRemove(obj.Game.ConnectionUID, out client);
                }
            }
        }


        // Names: the whole string must use the allowed set, 1..16 characters.
        static readonly System.Text.RegularExpressions.Regex ValidNameRegex =
            new System.Text.RegularExpressions.Regex("^[A-Za-z0-9\u0621-\u064A*~.&$]{1,16}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        // Free text (ExceptedSize == false: guild/union bulletins) keeps the old rule on the last character.
        static readonly System.Text.RegularExpressions.Regex LegacyTextRegex =
            new System.Text.RegularExpressions.Regex("[^A-Za-z0-9\u0621-\u064A*~.&.$]$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);

        /// <summary>
        /// ExceptedSize == true (character, guild, clan... names): full-string character set check, max 16
        /// characters. Reserved words: "GM", "PM", "SYSTEM" rejected case-sensitively (as before, so names like
        /// Sigma / Magma / Enigma stay valid); the bracketed tags "[GM]" / "[PM]" are rejected in any letter case.
        /// ExceptedSize == false (bulletins): no length limit, previous rules, control characters rejected.
        /// </summary>
        public static bool NameStrCheck(string name, bool ExceptedSize = true)
        {
            if (name == null)
                return false;
            if (name == "")
                return false;
            for (int x = 0; x < name.Length; x++)
                if (name[x] < 0x20 || name[x] == 0x7F)
                    return false;

            if (name.Contains('/'))
                return false;
            if (name.Contains(@"\"))
                return false;
            if (name.Contains(@"'"))
                return false;
            if (name.Contains("{") || name.Contains("}") || name.Contains("[") || name.Contains("]"))
                return false;

            if (ExceptedSize)
            {
                if (!ValidNameRegex.IsMatch(name))
                    return false;
            }
            else
            {
                if (LegacyTextRegex.IsMatch(name))
                    return false;
            }
            if (ContainsReservedWord(name))
                return false;
            return true;
        }

        static bool ContainsReservedWord(string name)
        {
            if (name.Contains("GM") || name.Contains("PM") || name.Contains("SYSTEM"))
                return true;
            return name.IndexOf("[gm]", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("[pm]", StringComparison.OrdinalIgnoreCase) >= 0;
        }
        public static bool StringCheck(string pszString)
        {
            for (int x = 0; x < pszString.Length; x++)
            {
                if (pszString[x] > ' ' && pszString[x] <= '~')
                    return false;
            }
            return true;
        }
    }
}
