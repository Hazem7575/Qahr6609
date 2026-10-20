using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    /// <summary>
    /// Client entry points of the Kingdom War: the Treasury NPC in Twin City, the League (union) operations the
    /// client uses for the Plunder War (LeagueOpt 60/64/69/71/78) and MsgLeagueRobOpt (2642).
    ///
    /// Client protocol (Conquer.exe 6609, CMsgLeagueOpt / CMsgLeagueRobOpt):
    ///   server -> client LeagueOpt 63 (UID = server id, text = Kingdom): "The Plunder War against [%s] %s has started!" box, accept sends 64.
    ///   server -> client LeagueOpt 69: "The Plunder War has started" + defend box, accept sends 71; 70 clears the war state.
    ///   server -> client LeagueOpt 67 / 68 (text = union): Plunder War raised by your Union / against your Kingdom has ended.
    ///   client -> server LeagueOpt 60 / 78: Loot / Reinforce token dialogs opened.
    ///   RobOpt type 0 = issue Loot Token (servers = target server ids), 1 = same after "pay CPs to make up the expense"
    ///   (server sends type 1 with param = CPs to ask), 2/7 = Loot target list page (param 7000 + page), 3/8 = Reinforce
    ///   list page, 5/6 = issue Reinforce Token (only effective to Kingdoms on other servers).
    /// </summary>
    public static class KingdomWarHandlers
    {
        #region Treasury NPC (Twin City)
        [NpcAttribute((NpcID)MsgKingdomWar.TreasuryNpcUID)]
        public static void KingdomTreasury(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            try
            {
                if (client == null || client.Player == null)
                    return;
                Dialog dialog = new Dialog(client, stream);
                dialog.AddAvatar(6);
                switch (Option)
                {
                    case 0:
                        {
                            dialog.AddText(MsgKingdomWar.DescribeTreasury());
                            var player = client.Player;
                            if (MsgKingdomWar.IsAttacker(client))
                                dialog.AddOption("Plunder~a~Gold~Brick.", 1);
                            if (MsgKingdomWar.IsDefender(client) && player.UnionMemeber != null
                                && (player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor
                                || player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.LeftMar
                                || player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.RightMar))
                                dialog.AddOption("Activate~the~Dragon~Shield.", 2);
                            if (player.IsUnionEmperror && player.MyUnion.IsKingdom == 0 && MsgKingdomWar.WarState != ProcesType.Alive
                                && !MsgKingdomWar.IsRegistered(player.MyUnion.UID) && KingdomWarConfig.IsPlunderDay(DateTime.Now.DayOfWeek)
                                && DateTime.Now.TimeOfDay < KingdomWarConfig.LootTokenClose)
                                dialog.AddOption("Issue~a~Loot~Token.", 3);
                            dialog.AddOption("How~does~the~Kingdom~War~work?", 4);
                            dialog.AddOption("I~see.", 255);
                            dialog.FinalizeDialog();
                            break;
                        }
                    case 1:
                        {
                            string message;
                            MsgKingdomWar.StartPlunder(client, out message);
                            if (message != null)
                                client.SendSysMesage(message, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                            break;
                        }
                    case 2:
                        {
                            string message;
                            MsgKingdomWar.ActivateDragonShield(client, out message);
                            if (message != null)
                                dialog.AddText(message).AddOption("I~see.", 255).FinalizeDialog();
                            break;
                        }
                    case 3:
                    case 5:
                        {
                            uint cps;
                            string message;
                            var result = MsgKingdomWar.IssueLootToken(client, Option == 5, out cps, out message);
                            if (result == MsgKingdomWar.IssueResult.NeedCps)
                            {
                                dialog.AddText(message);
                                dialog.AddOption("Pay~" + cps + "~CPs.", 5);
                                dialog.AddOption("Never~mind.", 255);
                                dialog.FinalizeDialog();
                            }
                            else if (message != null)
                                dialog.AddText(message).AddOption("I~see.", 255).FinalizeDialog();
                            break;
                        }
                    case 4:
                        {
                            dialog.AddText("At " + KingdomWarConfig.SettleTime.ToString(@"hh\:mm") + " the Union with the most Gold Bricks upgrades to Kingdom and its leader is crowned Emperor; the defeated Kingdom degrades to Union.")
                                .AddText(" A Union Leader can issue a Loot Token at 00:00 - 19:59 on Monday or Friday (costs Union Fund). At 21:00 - 21:30 heroes of that Union")
                                .AddText(" stay next to this Treasury to grab a Gold Brick, then hand it in to the Kingdom Mission Envoy: the bricks move from the Kingdom to your Union.")
                                .AddText(" The Emperor and Marshals can raise a Dragon Shield to protect the Treasury for 3 minutes.");
                            dialog.AddOption("I~see.", 255);
                            dialog.FinalizeDialog();
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        #endregion

        #region LeagueOpt (2622) Plunder War actions
        /// <summary>
        /// Called first from MsgLeagueOpt.Process. Returns true when the action is a Plunder War action handled here.
        /// </summary>
        public static bool HandleLeagueOpt(Client.GameClient user, ServerSockets.Packet stream, ushort action, uint id, uint uid, uint index)
        {
            try
            {
                if (user == null || user.Player == null)
                    return false;
                switch (action)
                {
                    case 64: // accepted "Go attack the Kingdom`s Treasury"
                        MsgKingdomWar.JoinWar(user, true);
                        return true;
                    case 71: // accepted "defend our Treasury"
                        MsgKingdomWar.JoinWar(user, false);
                        return true;
                    case (ushort)MsgLeagueOpt.ActionID.PlunderWar: // 69
                        MsgKingdomWar.JoinWar(user, null);
                        return true;
                    case 60: // Loot Token dialog opened -> 2640 type 0 (it used to get RobOpt 2, which feeds another window)
                        SendTargetList(user, stream, 0);
                        return true;
                    case 78: // Reinforce Token dialog opened -> 2640 type 1
                        SendTargetList(user, stream, 1);
                        return true;
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                return true;
            }
            return false;
        }
        #endregion

        #region MsgLeagueRobOpt (2642)
        public static void HandleRobOpt(Client.GameClient user, ServerSockets.Packet stream, MsgLeagueRobOpt.RobOpt msg)
        {
            if (user == null || user.Player == null || msg == null)
                return;
            switch (msg.Type)
            {
                case 0: // issue Loot Token
                case 1: // issue after agreeing to pay the shortfall in CPs
                    {
                        if (!TargetsThisServer(msg))
                        {
                            user.SendSysMesage("You can only plunder the Kingdom of your own server.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            break;
                        }
                        uint cps;
                        string message;
                        var result = MsgKingdomWar.IssueLootToken(user, msg.Type == 1, out cps, out message);
                        if (result == MsgKingdomWar.IssueResult.NeedCps)
                        {
                            var reply = new MsgLeagueRobOpt.RobOpt() { Type = 1, Param = cps };
                            reply.Servers.Add(new MsgLeagueRobOpt.RobServer() { ID = MsgKingdomWar.ServerID });
                            user.Send(stream.CreateLeagueRobOpt(reply));
                        }
                        else if (message != null)
                            user.SendSysMesage(message, MsgMessage.ChatMode.System, result == MsgKingdomWar.IssueResult.Ok ? MsgMessage.MsgColor.yellow : MsgMessage.MsgColor.red);
                        break;
                    }
                case 2:
                case 7:
                    SendRobList(user, stream, msg.Type, msg.Param);
                    break;
                case 3:
                case 8:
                    SendRobList(user, stream, msg.Type, msg.Param);
                    break;
                case 5:
                case 6:
                    user.SendSysMesage("The Reinforce Token is only effective to the Kingdoms on other servers.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                    break;
            }
        }

        static bool TargetsThisServer(MsgLeagueRobOpt.RobOpt msg)
        {
            if (msg.Servers == null || msg.Servers.Count == 0)
                return true;
            uint me = MsgKingdomWar.ServerID;
            foreach (var server in msg.Servers.Take(32))
                if (server != null && (server.ID == me || server.ID == 0))
                    return true;
            return false;
        }

        /// <summary>
        /// Loot (type 2/7) or Reinforce (3/8) target list. A single server has one Kingdom; reinforcing works only
        /// across servers, so that list is empty. The page parameter is echoed (7000 + page).
        /// </summary>
        static void SendRobList(Client.GameClient user, ServerSockets.Packet stream, uint type, uint param)
        {
            // the client sends param = rowsPerPage * 1000 + page and reads param / 1000 of the reply as the PAGE COUNT
            uint page = param % 1000;
            if (page == 0) page = 1;
            var rows = new List<MsgLeagueRobOpt.RobServer>();
            if (type == 2 || type == 7)
            {
                var kingdom = MsgKingdomWar.GetKingdom();
                if (kingdom != null && (!user.Player.InUnion || user.Player.MyUnion.UID != kingdom.UID))
                    rows.Add(new MsgLeagueRobOpt.RobServer() { ID = MsgKingdomWar.ServerID, Name = kingdom.Name, Name2 = kingdom.Emperor });
            }
            const int perPage = 7;
            uint pages = (uint)Math.Max(1, (rows.Count + perPage - 1) / perPage);
            if (page > pages) page = pages;
            var reply = new MsgLeagueRobOpt.RobOpt() { Type = type, Param = pages * 1000 + page };
            foreach (var r in rows.Skip((int)(page - 1) * perPage).Take(perPage))
                reply.Servers.Add(r);
            user.Send(stream.CreateLeagueRobOpt(reply));
        }

        /// <summary>2640 target list of the Loot (type 0) / Reinforce (type 1) token window. One server has one Kingdom;
        /// reinforcing other servers is cross-server only, so that list stays empty (the client ignores an empty list).</summary>
        static void SendTargetList(Client.GameClient user, ServerSockets.Packet stream, uint type)
        {
            var list = new MsgLeagueRobOpt.RobList() { Type = type };
            var kingdom = MsgKingdomWar.GetKingdom();
            if (type == 0 && kingdom != null && (!user.Player.InUnion || user.Player.MyUnion.UID != kingdom.UID))
                list.Items.Add(new MsgLeagueRobOpt.RobItem() { ServerID = MsgKingdomWar.ServerID, Name = TargetName(kingdom.Name) });
            if (list.Items.Count == 0)
            {
                user.SendSysMesage(type == 0 ? "There is no Kingdom you can loot right now." : "There is no Kingdom you can reinforce on this server.");
                return;
            }
            user.Send(stream.CreateLeagueRobList(list));
        }

        static string TargetName(string name)
        {
            return string.IsNullOrEmpty(name) ? "-" : name.Replace(' ', '~').Replace(',', '.');
        }
        #endregion
    }
}
