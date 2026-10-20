using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgGoldenTree
    {
        // every pick of this bloom (name, pool prize id), in pick order: the "Lucky Picks" list of the ended tree
        private static readonly List<Tuple<string, uint>> Picks = new List<Tuple<string, uint>>();
        private static readonly object RankSync = new object();

        /// <summary>Snapshot of the winners list (name, prize id).</summary>
        public static List<Tuple<string, uint>> GoldenTreeRank
        {
            get { lock (RankSync) return new List<Tuple<string, uint>>(Picks); }
        }
        public static int RankCount
        {
            get { lock (RankSync) return Picks.Count; }
        }
        /// <summary>A new bloom starts with an empty winners list.</summary>
        public static void ClearRank()
        {
            lock (RankSync)
                Picks.Clear();
        }
        public static void Update(string Name, uint PrizeID)
        {
            if (Name == null)
                return;
            lock (RankSync)
                Picks.Add(Tuple.Create(Name, PrizeID));
        }

        public static void SendWorldMessage(ServerSockets.Packet stream)
        {
            foreach (var kv in Database.Server.GamePoll)
                kv.Value.Send(stream);
        }

        static List<LotteryCount> Progress()
        {
            return new List<LotteryCount> { new LotteryCount { Key = Database.GlobalLotteryTable.RemainingPicks, Value = Database.GlobalLotteryTable.TotalPicks } };
        }
        static List<LotteryCount> PrizesLeft()
        {
            return Database.GlobalLotteryTable.PrizesLeft().Select(p => new LotteryCount { Key = p.Key, Value = (uint)p.Value }).ToList();
        }
        /// <summary>Type 0 (status): GUI 2 = blooming (Time = seconds to the end), 3 = ended with a winners list, 0 = nothing.
        /// The client never asks for it: it is pushed at login and on every change.</summary>
        static MsgGlobalLottery Status()
        {
            var condition = Database.GlobalLotteryTable.TodayCondition;
            var Lottery = new MsgGlobalLottery { Type = BlossomAction.Status, PrizesType = condition.ID };
            if (Database.GlobalLotteryTable.IsActive)
            {
                Lottery.GUI = (uint)GuiType.Opened;
                Lottery.Time = (uint)Math.Max(0, (Database.GoldenTreeState.Expiration - DateTime.Now).TotalSeconds);
            }
            else if ((Lottery.Time = Database.GoldenTreeFestival.SecondsToBloom) > 0)
                Lottery.GUI = (uint)GuiType.Ready;// "Coming Soon": Time = seconds to the bloom
            else// between two blooms of the festival the tree stays on the screen (icon + "Bloom has ended" + winners), as official
                Lottery.GUI = (uint)(RankCount == 0 && !Database.GoldenTreeFestival.IsOn ? GuiType.NotOpened : GuiType.Ranking);
            Lottery.Counts = Progress();
            return Lottery;
        }
        public static void BlossomEvaluation(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (Database.GlobalLotteryTable.TodayCondition == null)
                return;
            client.Send(stream.CreateGlobalLottery(Status()));
        }
        public static void LoginEvaluation(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!Database.GlobalLotteryTable.Loaded || Database.GlobalLotteryTable.TodayCondition == null)
                return;
            BlossomEvaluation(client, stream);
        }
        /// <summary>Answer to the client's {1:5} (sent when the tree opens while blooming): the player's own cooldown in seconds.</summary>
        public static void SendCoolDown(Client.GameClient client, ServerSockets.Packet stream)
        {
            uint cd = Database.GlobalLotteryTable.IsActive ? Database.GlobalLotteryTable.CooldownLeft(client.Player.UID) : 0;
            client.Send(stream.CreateGlobalLottery(new MsgGlobalLottery { Type = BlossomAction.CoolDown, Time = cd }));
        }
        /// <summary>Answer to the client's {1:3} (sent when the ended tree opens): count left of every prize ("N Left").</summary>
        public static void SendPrizeList(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Send(stream.CreateGlobalLottery(new MsgGlobalLottery { Type = BlossomAction.PrizeList, Counts = PrizesLeft() }));
        }
        /// <summary>The bloom is over (time up, stopped or no picks left): the unclaimed prizes, then the ended status, to everyone.</summary>
        public static void EndEvent()
        {
            if (Database.GlobalLotteryTable.TodayCondition == null)
                return;
            using (var recycle = new ServerSockets.RecycledPacket())
            {
                var stream = recycle.GetStream();
                SendWorldMessage(stream.CreateGlobalLottery(new MsgGlobalLottery { Type = BlossomAction.Ended, Counts = PrizesLeft() }));
                var status = Status();
                if (status.GUI == (uint)GuiType.Opened)// no picks left before the time is up
                    status.GUI = (uint)GuiType.Ranking;
                SendWorldMessage(stream.CreateGlobalLottery(status));
            }
        }

        /// <summary>
        /// One pick from the Golden Tree: bloom running, picks left, own cooldown over (or paid away: payCd = the client's
        /// flag 1, "Auto-reset CD" / "Pay n CPs to reset"), one free inventory slot, the picking cost of the condition paid;
        /// then one prize leaves the shared pool and everyone gets the pick (Type 1).
        /// The cost, as the client shows it: CostType 1 = CPs (Monopoly 0 = CP(B) first then CPs, 1 = CP(B) only, 2 = CPs only),
        /// 2 = Silver, 5 = one item of type CostValue.
        /// </summary>
        public static void Pick(Client.GameClient client, ServerSockets.Packet stream, bool payCd)
        {
            var condition = Database.GlobalLotteryTable.TodayCondition;
            if (condition == null)
                return;
            if (!Database.GlobalLotteryTable.IsActive)
            {
                client.SendSysMesage(RankCount > 0 ? "Bloom has ended." : "The Golden Tree hasn`t bloomed. Please be patient.");
                return;
            }
            if (Database.GlobalLotteryTable.RemainingPicks == 0)
            {
                client.SendSysMesage("Bloom has ended.");
                EndEvent();
                return;
            }
            uint cdCost = 0;
            uint cd = Database.GlobalLotteryTable.CooldownLeft(client.Player.UID);
            if (cd > 0)
            {
                if (!payCd || condition.CD == 0 || condition.CDCost == 0)
                {
                    client.SendSysMesage("You can pick again in " + cd + " sec.");
                    SendCoolDown(client, stream);
                    return;
                }
                cdCost = (uint)Math.Ceiling(Math.Min(cd, condition.CD) * (double)condition.CDCost / condition.CD);
            }
            if (!client.Inventory.HaveSpace(1))
            {
                client.SendSysMesage("Your inventory is full.");
                return;
            }
            uint cost = condition.CostValue;
            // what this pick takes: bound CPs, CPs (cost + cooldown reset), silver or one item
            uint boundCps = 0, cps = cdCost, silver = 0, item = 0;
            switch (condition.CostType)
            {
                case 1:
                    if (condition.Monopoly == 2)
                        cps += cost;
                    else if (client.Player.BoundConquerPoints >= cost)
                        boundCps = cost;
                    else if (condition.Monopoly == 0 && payCd)
                        cps += cost;
                    else
                    {
                        client.SendSysMesage("Unable to pick chests due to insufficient CPs (B).");
                        return;
                    }
                    break;
                case 2:
                    silver = cost;
                    break;
                case 5:
                    item = cost;
                    break;
                default:
                    return;
            }
            if (client.Player.ConquerPoints < cps)
            {
                client.SendSysMesage("Unable to pick chests due to insufficient CPs.");
                return;
            }
            if (client.Player.Money < silver)
            {
                client.SendSysMesage("Unable to pick chests due to insufficient Silver.");
                return;
            }
            if (item != 0 && !client.Inventory.Contain(item, 1) && !client.Inventory.Contain(item, 1, 1))
            {
                Database.ItemType.DBItem costItem;
                client.SendSysMesage("You need 1 " + (Database.Server.ItemsBase.TryGetValue(item, out costItem) ? costItem.Name : "item " + item) + " to pick a chest.");
                return;
            }
            var Prize = Database.GlobalLotteryTable.Draw(client.Player.UID);
            if (Prize == null)
            {
                client.SendSysMesage("Bloom has ended.");
                EndEvent();
                return;
            }
            Database.ItemType.DBItem DBItem;
            if (!Database.Server.ItemsBase.TryGetValue(Prize.PrizeItemID, out DBItem))
            {
                Database.GlobalLotteryTable.Undo(client.Player.UID, Prize);
                return;
            }
            if (item != 0 && !client.Inventory.Remove(item, 1, stream))
            {
                Database.GlobalLotteryTable.Undo(client.Player.UID, Prize);
                return;
            }
            if (boundCps != 0) client.Player.BoundConquerPoints -= (int)boundCps;
            if (cps != 0) client.Player.ConquerPoints -= cps;
            if (silver != 0) client.Player.Money -= silver;

            Database.GoldenTreeState.Claimed++;
            bool bound = Prize.dwparam != 0;
            byte plus = (Prize.PrizeItemID >= 730001 && Prize.PrizeItemID <= 730009) ? (byte)(Prize.PrizeItemID - 730000) : (byte)0;
            int mins = Prize.dwparam2 != 0 ? (int)Math.Min(Prize.dwparam3, 525600u) : 0;
            byte amount = (byte)Math.Min(Math.Max(Prize.dwparam7, 1u), 10u);
            if (DBItem.StackSize > 1 && mins == 0)
                client.Inventory.AddItemWitchStack(Prize.PrizeItemID, plus, amount, stream, bound);
            else
                client.Inventory.Add(stream, Prize.PrizeItemID, amount, plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound, Role.Flags.ItemEffect.None, false, "", 0, 0, mins);

            Update(client.Player.Name, Prize.ID);
            Modules.GoldenTree.GoldenTreeModule.SaveState();
            string paid = (boundCps != 0 ? boundCps + " CPs(B) " : "") + (cps != 0 ? cps + " CPs " : "") + (silver != 0 ? silver + " Silver " : "") + (item != 0 ? "item " + item + " " : "");
            Database.ServerDatabase.LoginQueue.Enqueue("[GoldenTree] " + client.Player.UID + " " + client.Player.Name + " paid " + paid + "picked prize " + Prize.ID + " item " + Prize.PrizeItemID + " x" + amount);

            // everyone: the client plays the chest for its own pick, shows the prize and prints the pool's broadcast line itself
            uint left = Database.GlobalLotteryTable.RemainingPicks;
            SendWorldMessage(stream.CreateGlobalLottery(new MsgGlobalLottery
            {
                Type = BlossomAction.PickUp,
                PrizesType = client.Player.UID,
                GUI = (uint)(left == 0 ? GuiType.Ranking : GuiType.Opened),
                Time = Prize.ID,
                Name = client.Player.Name,
                Counts = Progress()
            }));
            if (left == 0)
                EndEvent();
        }
    }
}
