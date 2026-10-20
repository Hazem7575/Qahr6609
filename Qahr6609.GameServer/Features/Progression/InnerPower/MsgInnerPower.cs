using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetInnerPower(this ServerSockets.Packet stream, out MsgInnerPower.ActionID mode, out uint dwparam, out uint dwparam2)
        {
            mode = (MsgInnerPower.ActionID)stream.ReadUInt8();
            dwparam = stream.ReadUInt32();
            dwparam2 = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet InnerPowerOptCreate(this ServerSockets.Packet stream, MsgInnerPower.ActionID mode, uint dwparam, uint dwparam2)
        {
            stream.InitWriter();
            stream.Write((byte)mode);
            stream.Write(dwparam);
            stream.Write(dwparam2);
            stream.Finalize(GamePackets.InnerPowerHandler);
            return stream;
        }
    }
    public class MsgInnerPower
    {
        public enum ActionID : byte
        {
            InfoStage = 0,
            Perfect = 1,//"YuanGong": complete an Inner Power at its last phase, reveals its score
            Nirvana = 2,//reshape a Perfect Inner Power from P0 with a Power Eraser
            UpdateGong = 3,
            UnlockStage = 4,
            OpenInner = 5,
            TransferInnerPowers = 6,//request: swap Inner Power with a trade partner (dwparam = partner)
            TransferAccept = 7,//server -> partner: request from dwparam; partner -> server: accepted
            TransferRefuse = 8,
        }

        // partner UID -> (requester UID, request time)
        static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, KeyValuePair<uint, DateTime>> TransferRequests
            = new System.Collections.Concurrent.ConcurrentDictionary<uint, KeyValuePair<uint, DateTime>>();

        static void SendAll(ServerSockets.Packet stream, Client.GameClient client, Role.Instance.InnerPower.Stage stage)
        {
            client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
            if (stage != null)
            {
                client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
            }
        }

        static bool TransferAllowed(Client.GameClient client, string who)
        {
            if (client.Player.Reborn < 2 || (client.Player.Reborn == 2 && client.Player.Level < 15))
            {
                client.SendSysMesage(who + " must be a 2nd-rebirth hero of level 15 or above to transfer Inner Power.");
                return false;
            }
            int minutes;
            if (!client.Player.InnerPower.CanExchange(out minutes))
            {
                client.SendSysMesage(who + " can transfer Inner Power again in " + (minutes / 60) + "h " + (minutes % 60) + "m.");
                return false;
            }
            return true;
        }

        static bool IsOfficialPartner(Client.GameClient client, uint partner)
        {
            if (client.Player.Associate == null)
                return false;
            System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Associate.Member> partners;
            Role.Instance.Associate.Member member;
            if (!client.Player.Associate.Associat.TryGetValue(Role.Instance.Associate.Partener, out partners) || !partners.TryGetValue(partner, out member))
                return false;
            return member.GetTimerLeft() == 0;//still in the trial period otherwise
        }

        static void RefreshAfterTransfer(ServerSockets.Packet stream, Client.GameClient client)
        {
            var inner = client.Player.InnerPower;
            inner.AddPotency(stream, client, 0);
            Role.Instance.InnerPower.Stage last;
            client.Send(stream.InnerPowerGui(inner.GetNeiGongs()));
            foreach (var stage in inner.Stages)
            {
                if (!stage.UnLocked)
                    continue;
                client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
            }
            if (inner.GetLastStage(out last))
                client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, last));
            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
            Role.Instance.InnerPower.InnerPowerRank.UpdateRank(inner);
        }
        [PacketAttribute(GamePackets.InnerPowerHandler)]
        public unsafe static void InnerPowerHandler(Client.GameClient client, ServerSockets.Packet stream)
        {
            ActionID Action;
            uint dwparam;
            uint dwparam2;

            stream.GetInnerPower(out Action, out dwparam, out dwparam2);
        
            switch (Action)
            {
                case ActionID.TransferInnerPowers:
                    {
                        Client.GameClient partner;
                        if (client.Player.InnerPower == null || dwparam == client.Player.UID)
                            break;
                        if (!Database.Server.GamePoll.TryGetValue(dwparam, out partner) || partner.Player.InnerPower == null)
                        {
                            client.SendSysMesage("Select an online trade partner to transfer Inner Power.");
                            break;
                        }
                        if (!IsOfficialPartner(client, dwparam) || !IsOfficialPartner(partner, client.Player.UID))
                        {
                            client.SendSysMesage("You can only transfer Inner Power to your official trade partners.");
                            break;
                        }
                        if (client.Player.ConquerPoints < Role.Instance.InnerPower.ExchangeCostCPs)
                        {
                            client.SendSysMesage("It requires " + Role.Instance.InnerPower.ExchangeCostCPs + " CPs to transfer Inner Power.");
                            break;
                        }
                        if (!TransferAllowed(client, "You") || !TransferAllowed(partner, partner.Player.Name))
                            break;
                        TransferRequests[partner.Player.UID] = new KeyValuePair<uint, DateTime>(client.Player.UID, DateTime.Now);
                        partner.Send(stream.InnerPowerOptCreate(ActionID.TransferAccept, client.Player.UID, 0));
                        client.SendSysMesage("Your Inner Power transfer request has been sent to " + partner.Player.Name + ".");
                        break;
                    }
                case ActionID.TransferRefuse:
                case ActionID.TransferAccept:
                    {
                        KeyValuePair<uint, DateTime> request;
                        if (!TransferRequests.TryRemove(client.Player.UID, out request) || request.Key != dwparam)
                            break;
                        Client.GameClient requester;
                        if (!Database.Server.GamePoll.TryGetValue(request.Key, out requester) || requester.Player.InnerPower == null || client.Player.InnerPower == null)
                        {
                            client.SendSysMesage("Your trade partner is offline.");
                            break;
                        }
                        if (Action == ActionID.TransferRefuse)
                        {
                            requester.SendSysMesage(client.Player.Name + " declined to transfer Inner Power with you.");
                            break;
                        }
                        if (DateTime.Now > request.Value.AddMinutes(2))
                        {
                            client.SendSysMesage("The Inner Power transfer request has expired.");
                            break;
                        }
                        if (!IsOfficialPartner(requester, client.Player.UID) || !IsOfficialPartner(client, requester.Player.UID)
                            || !TransferAllowed(requester, requester.Player.Name) || !TransferAllowed(client, "You"))
                            break;
                        if (requester.Player.ConquerPoints < Role.Instance.InnerPower.ExchangeCostCPs)
                        {
                            requester.SendSysMesage("It requires " + Role.Instance.InnerPower.ExchangeCostCPs + " CPs to transfer Inner Power.");
                            client.SendSysMesage(requester.Player.Name + " doesn`t have enough CPs to transfer Inner Power.");
                            break;
                        }
                        requester.Player.ConquerPoints -= (uint)Role.Instance.InnerPower.ExchangeCostCPs;
                        requester.Player.InnerPower.ExchangeWith(client.Player.InnerPower);
                        RefreshAfterTransfer(stream, requester);
                        RefreshAfterTransfer(stream, client);
                        requester.SendSysMesage("You transferred Inner Power with " + client.Player.Name + ".");
                        client.SendSysMesage("You transferred Inner Power with " + requester.Player.Name + ".");
                        break;
                    }
                case ActionID.Perfect:
                    {
                        Database.InnerPowerTable.Stage DBStage = null;
                        Database.InnerPowerTable.Stage.NeiGong DBGong = null;
                        Role.Instance.InnerPower.Stage stage = null;
                        Role.Instance.InnerPower.Stage.NeiGong gong = null;
                        if (!Database.InnerPowerTable.GetDBInfo(dwparam, out DBStage, out DBGong)
                            || !client.Player.InnerPower.TryGetStageAndGong((byte)dwparam, out stage, out gong))
                            break;
                        if (!stage.UnLocked || !gong.Unlocked || gong.Complete || gong.level < DBGong.MaxLevel)
                            break;
                        gong.Complete = true;
                        gong.Score = Role.Instance.InnerPower.RollPerfectScore(gong);
                        SendAll(stream, client, stage);
                        client.Player.InnerPower.UpdateStatus();
                        client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                        Role.Instance.InnerPower.InnerPowerRank.UpdateRank(client.Player.InnerPower);
                        client.SendSysMesage("Your Inner Power is Perfect! Score: " + gong.Score + ".");
                        break;
                    }
                case ActionID.Nirvana:
                    {
                        Database.InnerPowerTable.Stage DBStage = null;
                        Database.InnerPowerTable.Stage.NeiGong DBGong = null;
                        Role.Instance.InnerPower.Stage stage = null;
                        Role.Instance.InnerPower.Stage.NeiGong gong = null;
                        if (!Database.InnerPowerTable.GetDBInfo(dwparam, out DBStage, out DBGong)
                            || !client.Player.InnerPower.TryGetStageAndGong((byte)dwparam, out stage, out gong))
                            break;
                        if (!gong.Unlocked || !gong.Complete)
                        {
                            client.SendSysMesage("Only a Perfect Inner Power can be reshaped.");
                            break;
                        }
                        if (gong.Score >= 100)
                        {
                            client.SendSysMesage("Only an Inner Power with a score below 100 can be reshaped.");
                            break;
                        }
                        if (!client.Inventory.Contain(Role.Instance.InnerPower.PowerEraser, 1))
                        {
                            client.SendSysMesage("Please use the Power Eraser to reshape your Inner Power.");
                            break;
                        }
                        client.Inventory.Remove(Role.Instance.InnerPower.PowerEraser, 1, stream);
                        int refund = Role.Instance.InnerPower.SpentPotency(DBGong, gong.level);
                        gong.BestScore = Math.Max(gong.BestScore, gong.Score);
                        gong.level = 0;
                        gong.Score = 0;
                        gong.Complete = false;
                        client.Player.InnerPower.AddPotency(stream, client, refund);
                        client.Player.InnerPower.UpdateStatus();
                        SendAll(stream, client, stage);
                        client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                        Role.Instance.InnerPower.InnerPowerRank.UpdateRank(client.Player.InnerPower);
                        client.SendSysMesage("You abandoned the Inner Power and got " + refund + " Potency Points back. Perfect it again for a higher score!");
                        break;
                    }
                case ActionID.OpenInner:
                    {
                        Client.GameClient user;
                        if (Database.Server.GamePoll.TryGetValue(dwparam, out user) && user.Player.InnerPower != null)
                        {
                            client.Send(stream.InnerPowerGui(user.Player.InnerPower.GetNeiGongs()));
                            // the viewed player's own stages (not the viewer's)
                            var stages = user.Player.InnerPower.Stages.Where(p => p.Complete);
                            if(stages.Count() > 0)
                                client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, user.Player.UID, stages.Last()));
                    }
                        break;
                    }
                case ActionID.InfoStage:
                    {

                        Client.GameClient user;
                        if (Database.Server.GamePoll.TryGetValue(dwparam, out user) && user.Player.InnerPower != null)
                        {
                            var allStages = user.Player.InnerPower.Stages;
                            // stage numbers start at 1; anything else is ignored
                            if (allStages == null || dwparam2 < 1 || dwparam2 > allStages.Length)
                                break;
                            var stage = allStages[dwparam2 - 1];
                            client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, user.Player.UID, stage));
                            client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, user.Player.UID, stage));
                        }
                        break;
                    }
                case ActionID.UnlockStage:
                    {
                        Database.InnerPowerTable.Stage DBStage = null;
                        Database.InnerPowerTable.Stage.NeiGong DBGong = null;
                        if (Database.InnerPowerTable.GetDBInfo(dwparam, out DBStage, out DBGong))
                        {
                            if (DBGong.CheckAccount(client.Player.Reborn, client.Player.Level) && client.Inventory.Contain(DBGong.ItemID, 1)
                                && client.Player.InnerPower.isUnlockedNeiGong((byte)dwparam))
                            {
                                Role.Instance.InnerPower.Stage stage = null;
                                Role.Instance.InnerPower.Stage.NeiGong gong = null;
                                if (client.Player.InnerPower.TryGetStageAndGong((byte)dwparam, out stage, out gong))
                                {
                                    stage.UnLocked = gong.Unlocked = true;

                                    stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs());
                                    client.Send(stream);
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                                    client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));

                                    client.Inventory.Remove(DBGong.ItemID, 1, stream);
                                }
                            }
                        }
                        break;
                    }
                case ActionID.UpdateGong:
                    {
                        Database.InnerPowerTable.Stage DBStage = null;
                        Database.InnerPowerTable.Stage.NeiGong DBGong = null;
                        if (Database.InnerPowerTable.GetDBInfo(dwparam, out DBStage, out DBGong))
                        {
                            Role.Instance.InnerPower.Stage stage = null;
                            Role.Instance.InnerPower.Stage.NeiGong gong = null;
                            if (client.Player.InnerPower.TryGetStageAndGong((byte)dwparam, out stage, out gong))
                            {
                                if (stage.UnLocked && gong.Unlocked && gong.level < DBGong.MaxLevel)
                                {
                                    int potency_cost = (int)DBGong.ProgressNeiGongValue[Math.Min(gong.level, (int)(DBGong.ProgressNeiGongValue.Length - 1))];
                                    if (client.Player.InnerPower.Potency >= potency_cost)
                                    {
                                        client.Player.InnerPower.AddPotency(stream, client, -potency_cost);

                                        gong.level += 1;
                                        //the score stays unknown until the Inner Power is made Perfect (action Perfect)
                                        gong.Score = 0;
                                        gong.Complete = false;

                                        client.Send(stream.InnerPowerGui(client.Player.InnerPower.GetNeiGongs()));
                                        client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage,client.Player.UID, stage));
                                        client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
                                        client.Player.InnerPower.UpdateStatus();
                                        client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                        Role.Instance.InnerPower.InnerPowerRank.UpdateRank(client.Player.InnerPower);
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
