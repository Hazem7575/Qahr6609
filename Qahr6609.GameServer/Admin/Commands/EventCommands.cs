// Event and tournament commands.
// Migrated 1:1 from the old `switch (data[0])` in Game/MsgServer/MsgMessage.cs (ChatCommands).
// Every command keeps its old name (case-sensitive), its `data[]` indices and its body; `break` that left the
// switch became `return`. Role = GM is the old gate (client.GameMaster || client.ProjectManager).
// Auditing (gm-command / gm-denied) is done by CommandRegistry; argument errors give the old messages
// through CommandSupport.ArgumentError.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Game.Conquer6609_Work;
using System.IO;
using Conquer6609.Role;
using Conquer6609.Framework.Commands;

namespace Conquer6609.Commands
{
    // same name lookup as inside namespace Conquer6609.Game.MsgServer / class MsgMessage
    using Conquer6609.Game;
    using Conquer6609.Game.MsgServer;
    using static Conquer6609.Game.MsgServer.MsgMessage;
    using MsgServer = Conquer6609.Game.MsgServer;
    using MsgTournaments = Conquer6609.Game.MsgTournaments;

    internal static unsafe class EventCommands
    {
        [GmCommand("gs", Role = CommandRole.GM)]
        static void Cmd_gs(Client.GameClient client, string[] data)
        {
            try
            {
                //if (Game.MsgTournaments.MsgSchedules.GuildWarScore.Proces == ProcesType.Dead)
                //    Game.MsgTournaments.MsgSchedules.GuildWarScore.Start();
                //else
                //    Game.MsgTournaments.MsgSchedules.GuildWarScore.End();
                Role.Statue.ElitePkStatue(client);
                //Statue.winnerstate(client, 300, 290, 1, true);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        /// <summary>@gw start|end|status|round - Guild War (map 1038) by hand. start: runs until @gw end (the Saturday clock
        /// does not end a GM war); end: normal end with winner + rewards reset; round: knock the pole down now (test).</summary>
        [GmCommand("gw", Role = CommandRole.GM)]
        static void Cmd_gw(Client.GameClient client, string[] data)
        {
            try
            {
                var gw = MsgTournaments.MsgSchedules.GuildWar;
                string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "status";
                switch (sub)
                {
                    case "start":
                        if (gw.Proces != MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("Guild War is already running.");
                            break;
                        }
                        gw.GmStarted = true;
                        gw.Start();
                        client.SendSysMesage("Guild War started (GM).");
                        break;
                    case "end":
                        if (gw.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            client.SendSysMesage("Guild War is not running.");
                            break;
                        }
                        gw.CompleteEndGuildWar();
                        client.SendSysMesage("Guild War ended. Winner: " + gw.Winner.Name);
                        break;
                    case "round":
                        if (gw.Proces != MsgTournaments.ProcesType.Alive || gw.ScoreList.Count == 0)
                        {
                            client.SendSysMesage("No score yet: hit the pole first.");
                            break;
                        }
                        gw.FinishRound();
                        client.SendSysMesage("Pole knocked down. Holder: " + gw.Winner.Name);
                        break;
                    default:
                        client.SendSysMesage(gw.Status());
                        break;
                }
                MsgTournaments.MsgGuildWar.Log("@gw " + sub + " by " + client.Player.Name + ": " + gw.Status());
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("classpoleon", Role = CommandRole.GM)]
        static void Cmd_classpoleon(Client.GameClient client, string[] data)
        {
            try
            {

                Game.MsgTournaments.MsgArcherClass.Start();
                Game.MsgTournaments.MsgFireClass.Start();
                Game.MsgTournaments.MsgMonkClass.Start();
                Game.MsgTournaments.MsgNinjaClass.Start();
                Game.MsgTournaments.MsgTrojanClass.Start();
                Game.MsgTournaments.MsgWarriorClass.Start();
                Game.MsgTournaments.MsgWaterClass.Start();
                Game.MsgTournaments.MsgPariteClass.Start();
                Game.MsgTournaments.MsgArcherClass.CheckUP();
                Game.MsgTournaments.MsgFireClass.CheckUP();
                Game.MsgTournaments.MsgMonkClass.CheckUP();
                Game.MsgTournaments.MsgNinjaClass.CheckUP();
                Game.MsgTournaments.MsgPariteClass.CheckUP();
                Game.MsgTournaments.MsgTrojanClass.CheckUP();
                Game.MsgTournaments.MsgWarriorClass.CheckUP();
                Game.MsgTournaments.MsgWaterClass.CheckUP();

                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("battlefieldon", Role = CommandRole.GM)]
        static void Cmd_battlefieldon(Client.GameClient client, string[] data)
        {
            try
            {
                MsgTournaments.MsgSchedules.Tournaments[MsgTournaments.TournamentType.BattleField].Open();
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("dragonwar", Role = CommandRole.GM)]
        static void Cmd_dragonwar(Client.GameClient client, string[] data)
        {
            try
            {
                Game.MsgTournaments.ITournament dragonWar;
                if (!Game.MsgTournaments.MsgSchedules.Tournaments.TryGetValue(MsgTournaments.TournamentType.DragonWar, out dragonWar))
                {
                    client.SendSysMesage("DragonWar is not registered in MsgSchedules.Tournaments on this server.");
                    return;
                }
                dragonWar.Open();
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("dis", Role = CommandRole.GM)]
        static void Cmd_dis(Client.GameClient client, string[] data)
        {
            try
            {
                Game.MsgTournaments.MsgSchedules.DisCity.Open();
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("pkwar", Role = CommandRole.GM)]
        static void Cmd_pkwar(Client.GameClient client, string[] data)
        {
            try
            {
                MsgTournaments.MsgSchedules.PkWar.Open();
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("chr", Role = CommandRole.GM)]
        static void Cmd_chr(Client.GameClient client, string[] data)
        {
            try
            {
                Game.MsgTournaments.MsgSchedules.ChristmasAnimation.Start();
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("aelite", Role = CommandRole.GM)]
        static void Cmd_aelite(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    stream.ElitePKBracketsCreate((MsgElitePKBrackets.Action)ushort.Parse(data[1]), 0, 0, MsgTournaments.MsgEliteTournament.GroupTyp.EPK_Lvl130Plus, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, 3);
                    stream.ElitePKBracketsFinalize();
                    Program.SendGlobalPackets.Enqueue(stream);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }
    }
}
