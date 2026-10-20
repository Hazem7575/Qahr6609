using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;
using Conquer6609.Database;

namespace Conquer6609.Role
{
    public unsafe partial class Player
    {
        public uint ChargCps
        {
            get
            {
                return chargec;
            }
            set
            {
                if (value > 1000000)
                    value = 1000000;
                if (value > 0)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        InteractQuery action = new InteractQuery()
                        {
                            AtkType = MsgAttackPacket.AttackID.ClaimCP,
                            ResponseDamage = value,
                        };
                        View.SendView(stream.InteractionCreate(&action), true);
                    }
                }
                chargec = value;
            }
        }
        // First Credit Pack ("Credit any amount, and all these are yours!"): the 9 packs shown in the client window.
        public static readonly uint[] FirstCreditPackItems = new uint[]
        {
            3001035, // ChiPointPack(5000)
            3301830, // FancyMeteorScrollPack
            3300403, // BigEXPBallPack(10)
            3001063, // +3MaroonSteedPack
            3300241, // RollingCashPack
            3301828, // DelicateBrightStarPack
            3300242, // GeniusStudyPack
            3300366, // 1000CPs(B)Pack
            3301829, // IvoryRobePack
        };
        // Called when the player receives credited CPs. Unlocks the First Credit Pack once (no effect after it was claimed).
        public bool MarkFirstCredit(ServerSockets.Packet stream)
        {
            if ((MainFlag & (MainFlagType.CanClaim | MainFlagType.ClaimGift)) != MainFlagType.None)
                return false;
            MainFlag |= MainFlagType.CanClaim;
            SendUpdate(stream, (uint)MainFlag, MsgUpdate.DataType.MainFlag);
            if (Owner != null)
                Owner.SendSysMesage("Your First Credit Pack is ready! Open it from the gift icon and claim your 9 gifts.");
            return true;
        }
        public uint Merchant
        {
            get
            {
                return _merchant;
            }
            set
            {
                _merchant = value;
            }
        }
        public bool ContainRechargeType(RechargeType flag)
        {
            return (RechargeProgress & flag) == flag;
        }
        public bool AddRechargeFlag(RechargeType flag)
        {
            if (!ContainRechargeType(flag))
            {
                RechargeProgress |= flag;

                Database.RechargeShop.UpdateRecharge(Owner);

                return true;
            }
            return false;
        }


        public void UpdateKingdomTreasury(uint points)
        {
            if (InUnion)
            {
                MyUnion.Treasury += points;
                UnionMemeber.MyTreasury += points;
            }
        }
        public uint ChampionPoints
        {

            get
            {
                return _ChampionPoints;
            }
            set
            {

                _ChampionPoints = value;
                if (_ChampionPoints > HistoryChampionPoints)
                {
                    HistoryChampionPoints = value;
                }
            }
        }

        public void AddChampionPoints(uint value, bool settodayvalue = true)
        {
            if (HistoryChampionPoints >= 343700)
            {
                return;
            }
            if (settodayvalue)
            {
                if (TodayChampionPoints > 650)
                {
                    Owner.SendSysMesage("Your Champion Points have reached the maximum amount of 650 points. You can collect more points tomorrow.");
                    return;
                }
            }
            ChampionPoints += value;
            TodayChampionPoints += value;
            if (settodayvalue && value > 0 && Owner.Activeness != null) Owner.Activeness.OnChampionPointsReceived();// "Champion Points": receive once
        }
        public long Money
        {
            get
            {
                return _Money;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
               
                if (value > 50000000000)
                {
                    long dif = value - 50000000000;
                    WHMoney += dif;
                    value -= dif;
                    this.Owner.SendSysMesage(dif + " Gold has transfare to your warehouse you can't hold more than 50,000,000,000 into your inventory.", MsgMessage.ChatMode.Monster, MsgMessage.MsgColor.yellow);
                }
                ///
                if (Owner.FullLoading)
                {
                    long NewValue = 0;
                    if (_Money > value)
                    {
                        NewValue = _Money - value;
                    }
                    else
                    {
                        NewValue = value - _Money;
                    }
                    //banned
                    if (NewValue > 10000000000)
                    {

                        if (!(Owner != null && (Owner.ProjectManager || Owner.GameMaster)))
                        {
                            string logs = "[Money]" + Name + "He Hack Game and Get Get " + NewValue + " he have " + value + "Money";
                            Logs.OnDequeue(logs);
                            value = 0;
                            Database.SystemBannedAccount.AddBan(UID, Name, 999999, "Hack Money");
                            MyConsole.WriteLine(Name + " He Has Get Money From Hack And Get Ban", ConsoleColor.DarkRed);
                            Owner.Socket.Disconnect();
                        }
                    }
                    if (value > _Money)
                    {

                        if (NewValue > 500000)
                        {
                            string logs = "[Money]" + Name + " Get " + NewValue + " he have " + (_Money + NewValue) + " Money";
                           Logs.OnDequeue(logs);
                        }
                    }
                    else
                    {
                        long lost_Money = _Money - value;
                        if (lost_Money > 500000)
                        {
                            string logs = "[Money]" + Name + " lost " + lost_Money + " he have " + (_Money - lost_Money) + " Money";
                            Logs.OnDequeue(logs);
                        }
                    }
                }
                _Money = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, value, MsgUpdate.DataType.Money);
                }
            }
        }
        public void AddDropLayer(byte layerlevel)
        {
            if (layerlevel > byte.MaxValue)
            {
                MyConsole.WriteLine("Invalid Layer Level");
                return;
            }
            ConquerPointDropLimitLayer = layerlevel;
            ConquerPointsDropStamp = DateTime.Now.AddDays(7);
            MessageBox("Congratulation, you charged " + ConquerPointDropLimitLayer.ToString() + " drop layer level will last for 7 days from now.", null, null);
        }
        public uint ConquerPoints
        {
            get { return _cps; }
            set
            {
                if (value > int.MaxValue)
                {
                    if (!(Owner != null && (Owner.ProjectManager || Owner.GameMaster)))
                    {
                        string logs = "[ConquerPoints]" + Name + "He Hack Game and Get Get " + value + "";
                        Logs.OnDequeue(logs);
                        value = 0;
                        Database.SystemBannedAccount.AddBan(UID, Name, 999999, "Hack Cps");
                        MyConsole.WriteLine(Name + " He Has Get Cps From Hack And Get Ban", ConsoleColor.DarkRed);
                        Owner.Socket.Disconnect();
                    }
                }

                if (Owner.FullLoading)
                {
                    uint NewValue = 0;
                    if (_cps > value)
                    {
                        NewValue = _cps - value;
                    }
                    else
                    {
                        NewValue = value - _cps;
                    }
                    if (NewValue > 1000000000)
                    {
                        //banned
                        if (!(Owner != null && (Owner.ProjectManager || Owner.GameMaster)))
                        {
                            string logs = "[ConquerPoints]" + Name + "He Hack Game and Get Get " + NewValue + " he have " + value + "";
                            Logs.OnDequeue(logs);
                            value = 0;
                            Database.SystemBannedAccount.AddBan(UID, Name, 999999, "Hack Cps");
                            MyConsole.WriteLine(Name + " He Has Get Cps From Hack And Get Ban", ConsoleColor.DarkRed);
                            Owner.Socket.Disconnect();
                        }
                    }
                    if (value > _cps)
                    {
                        uint get_cps = value - _cps  ;
                        if (get_cps > 1000)
                        {
                            string logs = "[ConquerPoints]" + Name + " Get " + get_cps + " he have " + (_cps + get_cps) + " cps";
                            Logs.OnDequeue(logs);
                        }
                    }
                    else
                    {
                        uint lost_cps = _cps - value;
                        if (lost_cps > 1000)
                        {
                            string logs = "[ConquerPoints]" + Name + " lost " + lost_cps + " he have " + _cps + "cps ";
                            Logs.OnDequeue(logs);
                        }
                    }
                }
                _cps = value;
                if (Owner.FullLoading)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                        stream = packet.Append(stream, MsgUpdate.DataType.ConquerPoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }
        public int BoundConquerPoints
        {
            get
            { 
                return _bountCps;
            }
            set
            {
                if (Owner.FullLoading)
                {
                    int NewValue = 0;
                    if (_bountCps > value)
                    {
                        NewValue = _bountCps - value;
                    }
                    else
                    {
                        NewValue = value - _bountCps;
                    }
                    if (NewValue > 100000000)
                    {
                        //banned
                        if (!(Owner != null && (Owner.ProjectManager || Owner.GameMaster)))
                        {
                            string logs = "[BoundConquerPoints]" + Name + "He Hack Game and Get Get " + NewValue + " he have " + value + "";
                            Logs.OnDequeue(logs);
                            value = 0;
                            Database.SystemBannedAccount.AddBan(UID, Name, 999999, "Hack BoundCps");
                            MyConsole.WriteLine(Name + " He Has Get BoundCps From Hack And Get Ban", ConsoleColor.DarkRed);
                            Owner.Socket.Disconnect();
                        }
                    }
                }
                _bountCps = value;
                if (Owner.FullLoading)
                {
                    if (value > _cps)
                    {
                        int get_cps = value - _bountCps;
                        if (get_cps > 1000)
                        {
                            string logs = "[BoundConquerPoints]" + Name + " Get " + get_cps + " he have " + (_bountCps + get_cps) + " Bound Cps";
                            Logs.OnDequeue(logs);
                        }
                    }
                    else
                    {
                        int lost_cps = _bountCps - value;
                        if (lost_cps > 1000)
                        {
                            string logs = "[BoundConquerPoints]" + Name + " lost " + lost_cps + " he have " + _bountCps + "Bound Cps";
                            Logs.OnDequeue(logs);
                        }
                    }
                  
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                        stream = packet.Append(stream, MsgUpdate.DataType.BoundConquerPoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }
        public DateTime ExpireVip
        {
            get
            {
                var share = GetActiveSharedVip(UID);
                if (share != null)
                    return share.ShareEnds;
                return _ExpireVip;
            }
            set
            {
                _ExpireVip = value;
            }
        }

        public void AddVipVote(double Minutes)
        {
            if (DateTime.Now > ExpireVip)
                ExpireVip = DateTime.Now.AddMinutes(Minutes);
            else
                ExpireVip = ExpireVip.AddMinutes(Minutes);
            VipLevel = 6;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SendUpdate(stream, VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                UpdateVip(stream);
            }
            Owner.CreateBoxDialog("Thank you for your support. You`ve received vip6 (1 hour).");
        }

        // shared VIP only applies while the share has not expired; expired shares are dropped from the pool
        private static Database.ShareVIP.Client GetActiveSharedVip(uint uid)
        {
            Database.ShareVIP.Client active = null;
            var now = DateTime.Now;
            foreach (var entry in Database.ShareVIP.SharedPoll.GetValues())
            {
                if (entry == null || entry.ShareUID != uid)
                    continue;
                if (entry.ShareEnds > now)
                {
                    if (active == null)
                        active = entry;
                }
                else
                {
                    Database.ShareVIP.SharedPoll.Remove(entry);
                }
            }
            return active;
        }
        public byte VipLevel
        {
            get
            {
                var share = GetActiveSharedVip(UID);
                if (share != null)
                    return share.ShareLevel;
                return _viplevel;
            }
            set
            {

                _viplevel = value;
            }
        }
        public unsafe void UpdateVip(ServerSockets.Packet stream)
        {
            SendUpdate(stream, VipLevel, MsgUpdate.DataType.VIPLevel, false);
            if (VipLevel == 6)
            {

                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.FullVip));
                Owner.Send(stream.AutoHuntCreate(0, 341));
            }
            else if (VipLevel >= 1)
            {
                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.VipLevelOne));
            }
            else
                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.None));
        }
    }

}
