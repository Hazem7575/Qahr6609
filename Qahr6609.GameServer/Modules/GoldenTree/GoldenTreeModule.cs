// Golden Tree module (TQ "Golden Tree" in the Global Lottery window, packets 3282/3283). The whole event is this folder.
//
//   Qahr6609.GameServer\Modules\GoldenTree\     in the source: built into GameServer.exe by a normal build
//     GoldenTreeModule.cs   this file: start-up, save, login state, daily bloom, end of the bloom, star hooks, @goldentree
//     code\                 the tree: picking, prize pool, winners list, packets (MsgGoldenTree, GlobalLotteryTable...),
//                           the festival (GoldenTreeFestival.cs: daily bloom, Anniversary Star sources, NPC Bliss Boy)
//     client\               reference only: what the client needs (README_AR.md + the current ini\globallotterycondition.ini)
//     data\                 globallotterycondition.ini (cost, cooldown per condition) + global_lottery_pool.txt (prizes)
//                           + festival.ini (festival days, daily bloom hour, stars per source, Bliss Boy place),
//                           copied by the build to ServerDataBase\Modules\GoldenTree\ when they are not there yet;
//                           the running bloom is saved there in state.ini; client\ini\globallotterycondition.ini is
//                           written next to it at every load: the client's copy of the conditions (ship it in the patch)
//
// In game (GM): @goldentree start <hours> [max picks]  |  @goldentree stop  |  @goldentree status  |  @goldentree reload
// The same folder also works without a build in ServerDataBase\Modules\GoldenTree\ (compiled at start-up).
// Remove the event: delete the folder (or rename it to _GoldenTree).
using System;
using System.IO;
using Conquer6609.Database;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.GoldenTree
{
    public sealed class GoldenTreeModule : ModuleBase
    {
        public override string Name { get { return "GoldenTree"; } }

        static GoldenTreeModule instance;
        static DateTime endAnnounced = DateTime.MinValue;// the bloom whose end was already sent to the world

        // the prize files: the module folder; an older server keeps them in ServerDataBase itself
        string DataFolder
        {
            get
            {
                if (File.Exists(Path.Combine(DataPath, "globallotterycondition.ini")))
                    return DataPath;
                return File.Exists(Path.Combine(Program.ServerConfig.DbLocation, "globallotterycondition.ini")) ? Program.ServerConfig.DbLocation : DataPath;
            }
        }

        public override void OnLoad()
        {
            instance = this;
            GoldenTreeState.Load(DataPath);
            GlobalLotteryTable.LoadDBConditions(DataFolder);
            WriteClientIni();
            GoldenTreeFestival.Load(DataPath);
            GoldenTreeFestival.LoadPlayers(DataPath);
            MyConsole.WriteLine("[GoldenTree] " + GoldenTreeFestival.Describe());
            if (DateTime.Now >= GoldenTreeState.Expiration)
                endAnnounced = GoldenTreeState.Expiration;// ended before this start: nothing to announce
            MyConsole.WriteLine("[GoldenTree] " + (GlobalLotteryTable.Loaded ? GlobalLotteryTable.Conditions.Length + " condition(s) loaded" : "prize files missing, tree disabled")
                + (GlobalLotteryTable.IsActive ? ", blooming until " + GoldenTreeState.Expiration.ToString("yyyy-MM-dd HH:mm") : ", not blooming"));
        }

        public override void OnSave()
        {
            GoldenTreeState.Save(DataPath);
            GoldenTreeFestival.SavePlayers(DataPath);
        }

        public override void OnShutdown()
        {
            GoldenTreeFestival.RemoveNpc();
        }

        // Anniversary Star sources
        public override bool OnItemUse(Client.GameClient client, MsgGameItem item) { return GoldenTreeFestival.OnItemUse(client, item); }
        public override void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster) { GoldenTreeFestival.OnMonsterKilled(killer, monster); }

        /// <summary>Saves the running bloom now (after every pick).</summary>
        public static void SaveState()
        {
            try { if (instance != null) GoldenTreeState.Save(instance.DataPath); }
            catch (Exception e) { MyConsole.WriteLine("[GoldenTree] state.ini not saved: " + e.Message); }
        }

        string ClientIniPath { get { return Path.Combine(DataPath, "client", "ini", "globallotterycondition.ini"); } }

        void WriteClientIni()
        {
            try
            {
                if (GlobalLotteryTable.Conditions.Length > 0)
                    GlobalLotteryTable.WriteClientIni(ClientIniPath);
            }
            catch (Exception e) { MyConsole.WriteLine("[GoldenTree] client ini not written: " + e.Message); }
        }

        public override void OnLogin(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
                MsgGoldenTree.LoginEvaluation(client, rec.GetStream());
        }

        static DateTime announced = DateTime.MinValue;// the bloom whose "Coming Soon" was sent
        static int tickCount;

        public override void OnTick(Extensions.Time32 clock)
        {
            if (++tickCount % 10 == 1)
                GoldenTreeFestival.SyncNpc();
            if (GlobalLotteryTable.Loaded)
            {
                DateTime bloom = GoldenTreeFestival.TodayBloom;
                if (bloom != DateTime.MinValue)
                {
                    DateTime bloomEnd = bloom.AddHours(GoldenTreeFestival.BloomHours);
                    // the daily bloom (once: a restart inside the window finds the same end time in state.ini)
                    if (DateTime.Now >= bloom && DateTime.Now < bloomEnd && GoldenTreeState.Expiration != bloomEnd)
                    {
                        StartBloom(bloomEnd, GoldenTreeFestival.MaxPicks, "daily bloom");
                        announced = bloom;
                    }
                }
                // "Coming Soon" a few minutes before the next daily bloom
                uint toBloom = GoldenTreeFestival.SecondsToBloom;
                if (toBloom > 0)
                {
                    DateTime next = DateTime.Now.AddSeconds(toBloom);
                    next = new DateTime(next.Year, next.Month, next.Day, next.Hour, next.Minute, 0);
                    if (announced != next)
                    {
                        announced = next;
                        ShowToEveryone();
                    }
                }
            }
            // time is up: tell everyone once (the window turns into the winners list)
            DateTime end = GoldenTreeState.Expiration;
            if (end != endAnnounced && DateTime.Now >= end)
            {
                endAnnounced = end;
                MsgGoldenTree.EndEvent();
                GoldenTreeState.Save(DataPath);
            }
        }

        [GmCommand("goldentree", Role = CommandRole.GM, Help = "Golden Tree: @goldentree start <hours> [max picks] | stop | status | reload",
            Usage = "@goldentree start 2 500")]
        static void Command(Client.GameClient client, string[] data)
        {
            var m = instance;
            if (m == null) return;
            string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "status";
            switch (sub)
            {
                case "start":
                    {
                        double hours;
                        uint max = 0;
                        if (data.Length < 3 || !double.TryParse(data[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out hours) || hours <= 0
                            || (data.Length > 3 && !uint.TryParse(data[3], out max)))
                        {
                            client.SendSysMesage("Usage: @goldentree start <hours> [max picks, 0 = whole pool]");
                            return;
                        }
                        if (!GlobalLotteryTable.Loaded)
                        {
                            client.SendSysMesage("Golden Tree prize files are missing (Modules\\GoldenTree). Not started.");
                            return;
                        }
                        StartBloom(DateTime.Now.AddHours(hours), max, client.Player.Name);
                        client.SendSysMesage("Golden Tree started: " + hours + " h, " + GlobalLotteryTable.TotalPicks + " pick(s).");
                        break;
                    }
                case "stop":
                    GoldenTreeState.Expiration = DateTime.Now;
                    GoldenTreeState.Save(m.DataPath);
                    m.Log(client.Player.Name + " stopped the Golden Tree");
                    client.SendSysMesage("Golden Tree stopped.");
                    break;// OnTick sends the end to everyone
                case "reload":
                    GlobalLotteryTable.LoadDBConditions(m.DataFolder);
                    m.WriteClientIni();
                    GoldenTreeFestival.Load(m.DataPath);
                    GoldenTreeFestival.SyncNpc();
                    client.SendSysMesage(GlobalLotteryTable.Loaded ? "Golden Tree files reloaded: " + GlobalLotteryTable.Conditions.Length + " condition(s)." : "Golden Tree files missing or invalid (see console).");
                    break;
                default:
                    client.SendSysMesage(!GlobalLotteryTable.Loaded ? "Golden Tree: prize files missing."
                        : GlobalLotteryTable.IsActive ? "Golden Tree is blooming until " + GoldenTreeState.Expiration.ToString("yyyy-MM-dd HH:mm") + ", picks left " + GlobalLotteryTable.RemainingPicks + " of " + GlobalLotteryTable.TotalPicks + "."
                        : "Golden Tree is not blooming. @goldentree start <hours> [max picks]");
                    client.SendSysMesage("Golden Tree " + GoldenTreeFestival.Describe() + ".");
                    break;
            }
        }

        /// <summary>A new bloom until <paramref name="end"/>: fresh prize pool, empty winners list, everyone told.</summary>
        static void StartBloom(DateTime end, uint max, string by)
        {
            var m = instance;
            if (m == null) return;
            GoldenTreeState.Claimed = 0;
            GoldenTreeState.MaxClaims = max;
            GoldenTreeState.Expiration = end;
            GoldenTreeState.Save(m.DataPath);
            m.Log(by + " started the Golden Tree until " + end.ToString("yyyy-MM-dd HH:mm") + ", max picks " + max);
            MyConsole.WriteLine("[GoldenTree] blooming until " + end.ToString("yyyy-MM-dd HH:mm") + " (" + by + ")");
            ShowToEveryone();
            using (var rec = new ServerSockets.RecycledPacket())
                Program.SendGlobalPackets.Enqueue(new MsgMessage("The Golden Tree is blooming! Open it from the event window and pick your prize.",
                    MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.Center).GetArray(rec.GetStream()));
        }

        // opens the tree for the players already online (new logins get it from OnLogin)
        static void ShowToEveryone()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var c in Database.Server.GamePoll.Values)
                {
                    try { if (c.Player != null) MsgGoldenTree.BlossomEvaluation(c, stream); }
                    catch { }
                }
            }
        }
    }
}
