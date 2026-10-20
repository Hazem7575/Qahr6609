using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateExpireNotify(this ServerSockets.Packet stream, List<Game.MsgServer.MsgChiInfo.ChiPowerType> Ids)
        {
            stream.InitWriter();
            stream.Write((uint)Ids.Count);
            foreach (var gate in Ids)
            {
                stream.Write((byte)gate);
                stream.Write((byte)1);
                stream.Write((byte)0);
            }
            stream.Finalize(GamePackets.MsgTrainingVitalityExpiryNotify);
            return stream;
        }
        public static unsafe void GetChiRetreatHandler(this ServerSockets.Packet stream, out MsgTrainingVitalityProtect.RetreatType Type, out MsgChiInfo.ChiPowerType Mode)
        {
            Type = (MsgTrainingVitalityProtect.RetreatType)stream.ReadUInt16();
            Mode = (MsgChiInfo.ChiPowerType)stream.ReadUInt8();
        }
        public static unsafe ServerSockets.Packet CreateRetreatedChiPacket(this ServerSockets.Packet stream, Game.MsgServer.MsgTrainingVitalityProtect.RetreatType Type, MsgChiInfo.ChiPowerType Mode)
        {
            stream.InitWriter();
            stream.Write((ushort)Type);
            stream.Write((byte)Mode);
            return stream;
        }
        public static unsafe ServerSockets.Packet ChiRetreatFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTrainingVitalityProtect);
            return stream;
        }
    }
    public unsafe struct MsgTrainingVitalityProtect
    {
        [Flags]
        public enum RetreatType
        {
            #region client
            Info = 0,
            RequestRetreat = 1,
            RequestRestore = 3,
            RequestExtend = 5,
            RequestPayoff = 7,
            RequestAbondan = 9,
            RequestUpdate = 11,
            RequestExtend2 = 13,
            #endregion
            #region Server
            Retreat = 2,
            Restore = 4,
            Extend = 6,
            Payoff = 8,
            Abondan = 10,
            Update = 12,
            Extend2 = 14,
            #endregion Server
        }
        // client defaults (ini/TrainingVitality.ini [ProtectLimit]): Lev=4 (VIP), ProtectCost=4000,
        // ProtectCostMax=4000, ProtectTime=168 hours. Overdue time costs ProtectCost per ProtectTime (by the minute).
        public const int ProtectCost = 4000, ProtectCostMax = 4000, ProtectHours = 168;
        public const uint FrozenChiPill = 3005360;

        static int OverdueCost(long time)
        {
            double minutes = (DateTime.Now - DateTime.FromBinary(time)).TotalMinutes;
            if (minutes <= 0)
                return 0;
            return (int)Math.Min(Math.Ceiling(minutes * ProtectCost / (ProtectHours * 60.0)), ProtectCostMax);
        }
        static bool GetLock(Role.Instance.Chi chi, MsgChiInfo.ChiPowerType mode, out long time, out Tuple<Role.Instance.Chi.ChiAttributeType, int>[] powers)
        {
            time = 0;
            powers = null;
            if (chi == null)
                return false;
            switch (mode)
            {
                case MsgChiInfo.ChiPowerType.Dragon: time = chi.DragonTime; powers = chi.DragonPowers; return true;
                case MsgChiInfo.ChiPowerType.Phoenix: time = chi.PhoenixTime; powers = chi.PhoenixPowers; return true;
                case MsgChiInfo.ChiPowerType.Tiger: time = chi.TigerTime; powers = chi.TigerPowers; return true;
                case MsgChiInfo.ChiPowerType.Turtle: time = chi.TurtleTime; powers = chi.TurtlePowers; return true;
            }
            return false;
        }
        static void SetLock(Role.Instance.Chi chi, MsgChiInfo.ChiPowerType mode, long time, Tuple<Role.Instance.Chi.ChiAttributeType, int>[] powers)
        {
            switch (mode)
            {
                case MsgChiInfo.ChiPowerType.Dragon: chi.DragonTime = time; chi.DragonPowers = powers; break;
                case MsgChiInfo.ChiPowerType.Phoenix: chi.PhoenixTime = time; chi.PhoenixPowers = powers; break;
                case MsgChiInfo.ChiPowerType.Tiger: chi.TigerTime = time; chi.TigerPowers = powers; break;
                case MsgChiInfo.ChiPowerType.Turtle: chi.TurtleTime = time; chi.TurtlePowers = powers; break;
            }
        }
        static Tuple<Role.Instance.Chi.ChiAttributeType, int>[] CopyPowers(Tuple<Role.Instance.Chi.ChiAttributeType, int>[] powers)
        {
            return powers == null ? null : (Tuple<Role.Instance.Chi.ChiAttributeType, int>[])powers.Clone();
        }
        static void RefreshChi(Client.GameClient user, MsgChiInfo.ChiPowerType mode)
        {
            var chi = user.Player.MyChi;
            Role.Instance.Chi.ComputeStatus(chi);
            user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(user, MsgChiInfo.Action.Send);
            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(user, MsgChiInfo.Action.Upgrade);
            switch (mode)
            {
                case MsgChiInfo.ChiPowerType.Dragon: Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, chi.Dragon); break;
                case MsgChiInfo.ChiPowerType.Phoenix: Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, chi.Phoenix); break;
                case MsgChiInfo.ChiPowerType.Tiger: Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, chi.Tiger); break;
                case MsgChiInfo.ChiPowerType.Turtle: Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, chi.Turtle); break;
            }
        }
        [PacketAttribute(GamePackets.MsgTrainingVitalityProtect)]
        private static void HandleRetreatChi(Client.GameClient _user, ServerSockets.Packet _stream)
        {
            RetreatType Action;
            MsgChiInfo.ChiPowerType Mode;
            _stream.GetChiRetreatHandler(out Action, out Mode);
                switch (Action)
                {
                    case RetreatType.Info:
                        {
                            MsgTrainingVitalityProtectInfo.SendInfo(_stream, _user);
                            break;
                        }
                    #region RequestRetreat
                    case RetreatType.RequestRetreat:
                        {
                            if (_user.Inventory.Contain(FrozenChiPill, 1) || _user.Inventory.Contain(FrozenChiPill, 1, 1))
                            {
                                _user.Inventory.Remove(FrozenChiPill, 1, _stream);
                                goto Reatreat;
                            }
                            else if (_user.Player.MyChi.ChiPoints >= ProtectCost)
                            {
                                { _user.Player.MyChi.ChiPoints -= ProtectCost; Conquer6609.Modules.OperatingActs.OperatingActs.AddPoints(_user, Conquer6609.Modules.OperatingActs.OperatingActs.ChiSpent, (uint)ProtectCost); }
                                goto Reatreat;
                            }
                            else
                            {
                                break;
                            }
                        Reatreat:
                            {
                                switch (Mode)
                                {
                                    case MsgChiInfo.ChiPowerType.Dragon:
                                        {
                                            if (_user.Player.MyChi.DragonTime != 0)
                                                break;
                                            _user.Player.MyChi.DragonPowers = _user.Player.MyChi.Dragon.Fields;
                                            _user.Player.MyChi.DragonTime = DateTime.Now.AddHours(ProtectHours).ToBinary();
                                            _stream.CreateRetreatedChiPacket(RetreatType.Retreat, Mode);
                                            _user.Send(_stream.ChiRetreatFinalize());
                                            Database.ServerDatabase.QueueUrgentSave(_user);// written on the player-save thread, not in the packet handler
                                            break;
                                        }
                                    case MsgChiInfo.ChiPowerType.Phoenix:
                                        {
                                            if (_user.Player.MyChi.PhoenixTime != 0)
                                                break;
                                            _user.Player.MyChi.PhoenixPowers = _user.Player.MyChi.Phoenix.Fields;
                                            _user.Player.MyChi.PhoenixTime = DateTime.Now.AddHours(ProtectHours).ToBinary();
                                            _stream.CreateRetreatedChiPacket(RetreatType.Retreat, Mode);
                                            _user.Send(_stream.ChiRetreatFinalize());
                                            Database.ServerDatabase.QueueUrgentSave(_user);// written on the player-save thread, not in the packet handler
                                            break;
                                        }
                                    case MsgChiInfo.ChiPowerType.Tiger:
                                        {
                                            if (_user.Player.MyChi.TigerTime != 0)
                                                break;
                                            _user.Player.MyChi.TigerPowers = _user.Player.MyChi.Tiger.Fields;
                                            _user.Player.MyChi.TigerTime = DateTime.Now.AddHours(ProtectHours).ToBinary();
                                            _stream.CreateRetreatedChiPacket(RetreatType.Retreat, Mode);
                                            _user.Send(_stream.ChiRetreatFinalize());
                                            Database.ServerDatabase.QueueUrgentSave(_user);// written on the player-save thread, not in the packet handler
                                            break;
                                        }
                                    case MsgChiInfo.ChiPowerType.Turtle:
                                        {
                                            if (_user.Player.MyChi.TurtleTime != 0)
                                                break;
                                            _user.Player.MyChi.TurtlePowers = _user.Player.MyChi.Turtle.Fields;
                                            _user.Player.MyChi.TurtleTime = DateTime.Now.AddHours(ProtectHours).ToBinary();
                                            _stream.CreateRetreatedChiPacket(RetreatType.Retreat, Mode);
                                            _user.Send(_stream.ChiRetreatFinalize());
                                            Database.ServerDatabase.QueueUrgentSave(_user);// written on the player-save thread, not in the packet handler
                                            break;
                                        }
                                }
                                break;
                            }
                        }
                    #endregion
                    #region RequestRestore
                    case RetreatType.RequestRestore:
                        {
                            switch (Mode)
                            {
                                case MsgChiInfo.ChiPowerType.Dragon:
                                    {
                                        if (_user.Player.MyChi.DragonTime == 0)
                                            break;
                                        if (DateTime.Now > DateTime.FromBinary(_user.Player.MyChi.DragonTime))
                                            break;
                                        _user.Player.MyChi.Dragon.Fields = _user.Player.MyChi.DragonPowers;
                                        Role.Instance.Chi.ComputeStatus(_user.Player.MyChi);
                                        _user.Equipment.QueryEquipment(_user.Equipment.Alternante, false);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(_user, MsgChiInfo.Action.Send);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(_user, MsgChiInfo.Action.Upgrade);
                                        Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, _user.Player.MyChi.Dragon);
                                        _stream.CreateRetreatedChiPacket(RetreatType.Restore, Mode);
                                        _user.Send(_stream.ChiRetreatFinalize());
                                        Database.ServerDatabase.QueueUrgentSave(_user);// written on the player-save thread, not in the packet handler
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Phoenix:
                                    {
                                        if (_user.Player.MyChi.PhoenixTime == 0)
                                            break;
                                        if (DateTime.Now > DateTime.FromBinary(_user.Player.MyChi.PhoenixTime))
                                            break;
                                        _user.Player.MyChi.Phoenix.Fields = _user.Player.MyChi.PhoenixPowers;
                                        Role.Instance.Chi.ComputeStatus(_user.Player.MyChi);
                                        Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, _user.Player.MyChi.Phoenix);
                                        _stream.CreateRetreatedChiPacket(RetreatType.Restore, Mode);
                                        _user.Send(_stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Tiger:
                                    {
                                        if (_user.Player.MyChi.TigerTime == 0)
                                            break;
                                        if (DateTime.Now > DateTime.FromBinary(_user.Player.MyChi.TigerTime))
                                            break;
                                        _user.Player.MyChi.Tiger.Fields = _user.Player.MyChi.TigerPowers;
                                        Role.Instance.Chi.ComputeStatus(_user.Player.MyChi);
                                        Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, _user.Player.MyChi.Tiger);
                                        _stream.CreateRetreatedChiPacket(RetreatType.Restore, Mode);
                                        _user.Send(_stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Turtle:
                                    {
                                        if (_user.Player.MyChi.TurtleTime == 0)
                                            break;
                                        if (DateTime.Now > DateTime.FromBinary(_user.Player.MyChi.TurtleTime))
                                            break;
                                        _user.Player.MyChi.Turtle.Fields = _user.Player.MyChi.TurtlePowers;
                                        Role.Instance.Chi.ComputeStatus(_user.Player.MyChi);
                                        Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, _user.Player.MyChi.Turtle);
                                        _stream.CreateRetreatedChiPacket(RetreatType.Restore, Mode);
                                        _user.Send(_stream.ChiRetreatFinalize());
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    #endregion
                    #region RequestExtend
                    case RetreatType.RequestExtend://renew an expired lock: pay up and lock again
                        {
                            long time;
                            Tuple<Role.Instance.Chi.ChiAttributeType, int>[] powers;
                            if (!GetLock(_user.Player.MyChi, Mode, out time, out powers) || time == 0)
                                break;
                            if (DateTime.Now < DateTime.FromBinary(time))
                                break;
                            int cost = Math.Min(OverdueCost(time) + ProtectCost, ProtectCostMax);
                            if (_user.Player.MyChi.ChiPoints < cost)
                            {
                                _user.SendSysMesage("You don`t have enough Chi Points.");
                                break;
                            }
                            { _user.Player.MyChi.ChiPoints -= cost; Conquer6609.Modules.OperatingActs.OperatingActs.AddPoints(_user, Conquer6609.Modules.OperatingActs.OperatingActs.ChiSpent, (uint)cost); }
                            SetLock(_user.Player.MyChi, Mode, DateTime.Now.AddHours(ProtectHours).ToBinary(), powers);
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(_user, MsgChiInfo.Action.Send);
                            _stream.CreateRetreatedChiPacket(RetreatType.Extend, Mode);
                            _user.Send(_stream.ChiRetreatFinalize());
                            Database.ServerDatabase.QueueUrgentSave(_user);
                            break;
                        }
                    #endregion
                    #region RequestUpdate
                    case RetreatType.RequestUpdate://Update
                        {
                            switch (Mode)
                            {
                                case MsgChiInfo.ChiPowerType.Dragon:
                                    {
                                        if (_user.Player.MyChi.DragonTime == 0)
                                            break;
                                        if (DateTime.Now > DateTime.FromBinary(_user.Player.MyChi.DragonTime))
                                            break;
                                        _user.Player.MyChi.DragonPowers = _user.Player.MyChi.Dragon.Fields;
                                        _stream.CreateRetreatedChiPacket(RetreatType.Update, Mode);
                                        _user.Send(_stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Phoenix:
                                    {
                                        if (_user.Player.MyChi.PhoenixTime == 0)
                                            break;
                                        if (DateTime.Now > DateTime.FromBinary(_user.Player.MyChi.PhoenixTime))
                                            break;
                                        _user.Player.MyChi.PhoenixPowers = _user.Player.MyChi.Phoenix.Fields;
                                        _stream.CreateRetreatedChiPacket(RetreatType.Update, Mode);
                                        _user.Send(_stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Tiger:
                                    {
                                        if (_user.Player.MyChi.TigerTime == 0)
                                            break;
                                        if (DateTime.Now > DateTime.FromBinary(_user.Player.MyChi.TigerTime))
                                            break;
                                        _user.Player.MyChi.TigerPowers = _user.Player.MyChi.Tiger.Fields;
                                        _stream.CreateRetreatedChiPacket(RetreatType.Update, Mode);
                                        _user.Send(_stream.ChiRetreatFinalize());
                                        break;
                                    }
                                case MsgChiInfo.ChiPowerType.Turtle:
                                    {
                                        if (_user.Player.MyChi.TurtleTime == 0)
                                            break;
                                        if (DateTime.Now > DateTime.FromBinary(_user.Player.MyChi.TurtleTime))
                                            break;
                                        _user.Player.MyChi.TurtlePowers = _user.Player.MyChi.Turtle.Fields;
                                        _stream.CreateRetreatedChiPacket(RetreatType.Update, Mode);
                                        _user.Send(_stream.ChiRetreatFinalize());
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    #endregion
                    #region RequestPayoff
                    case RetreatType.RequestPayoff://expired lock: pay up the overdue time and restore the locked attributes
                        {
                            long time;
                            Tuple<Role.Instance.Chi.ChiAttributeType, int>[] powers;
                            if (!GetLock(_user.Player.MyChi, Mode, out time, out powers) || time == 0 || powers == null)
                                break;
                            if (DateTime.Now < DateTime.FromBinary(time))
                                break;//still locked: use Restore
                            var power = _user.Player.MyChi.GetChiPowerByMode(Mode);
                            if (power == null || !power.UnLocked)
                                break;
                            int cost = OverdueCost(time);
                            if (_user.Player.MyChi.ChiPoints < cost)
                            {
                                _user.SendSysMesage("You don`t have enough Chi Points.");
                                break;
                            }
                            { _user.Player.MyChi.ChiPoints -= cost; Conquer6609.Modules.OperatingActs.OperatingActs.AddPoints(_user, Conquer6609.Modules.OperatingActs.OperatingActs.ChiSpent, (uint)cost); }
                            SetLock(_user.Player.MyChi, Mode, DateTime.Now.ToBinary(), powers);//paid up to now
                            power.Fields = CopyPowers(powers);
                            RefreshChi(_user, Mode);
                            _stream.CreateRetreatedChiPacket(RetreatType.Payoff, Mode);
                            _user.Send(_stream.ChiRetreatFinalize());
                            Database.ServerDatabase.QueueUrgentSave(_user);
                            break;
                        }
                    #endregion
                    #region RequestAbondan
                    case RetreatType.RequestAbondan://drop the locked attributes
                        {
                            long time;
                            Tuple<Role.Instance.Chi.ChiAttributeType, int>[] powers;
                            if (!GetLock(_user.Player.MyChi, Mode, out time, out powers) || time == 0)
                                break;
                            SetLock(_user.Player.MyChi, Mode, 0, null);
                            _stream.CreateRetreatedChiPacket(RetreatType.Abondan, Mode);
                            _user.Send(_stream.ChiRetreatFinalize());
                            Database.ServerDatabase.QueueUrgentSave(_user);
                            break;
                        }
                    #endregion
                    #region RequestExtend2
                    case RetreatType.RequestExtend2://extend a running lock by another ProtectTime (Frozen Chi Pill or Chi Points)
                        {
                            long time;
                            Tuple<Role.Instance.Chi.ChiAttributeType, int>[] powers;
                            if (!GetLock(_user.Player.MyChi, Mode, out time, out powers) || time == 0)
                                break;
                            DateTime expire = DateTime.FromBinary(time);
                            if (DateTime.Now > expire)
                                break;//expired: use Extend (renew)
                            if (_user.Inventory.Contain(FrozenChiPill, 1) || _user.Inventory.Contain(FrozenChiPill, 1, 1))
                                _user.Inventory.Remove(FrozenChiPill, 1, _stream);
                            else if (_user.Player.MyChi.ChiPoints >= ProtectCost)
                            {
                                { _user.Player.MyChi.ChiPoints -= ProtectCost; Conquer6609.Modules.OperatingActs.OperatingActs.AddPoints(_user, Conquer6609.Modules.OperatingActs.OperatingActs.ChiSpent, (uint)ProtectCost); }
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(_user, MsgChiInfo.Action.Send);
                            }
                            else
                            {
                                _user.SendSysMesage("You don`t have enough Chi Points.");
                                break;
                            }
                            SetLock(_user.Player.MyChi, Mode, expire.AddHours(ProtectHours).ToBinary(), powers);
                            _stream.CreateRetreatedChiPacket(RetreatType.Extend2, Mode);
                            _user.Send(_stream.ChiRetreatFinalize());
                            Database.ServerDatabase.QueueUrgentSave(_user);
                            break;
                        }
                    #endregion
                    default: MyConsole.WriteLine("Unknown Chi Retreat Action Type [" + Action + "]."); break;
                }
            
        }
        public static void Login(Client.GameClient client)
        {
            if (client.Player.VipLevel < 4)
                return;
            List<Conquer6609.Game.MsgServer.MsgChiInfo.ChiPowerType> Removed = new List<MsgChiInfo.ChiPowerType>(4);
            if (client.Player.MyChi.DragonTime != 0)
            {
                if (DateTime.Now > DateTime.FromBinary(client.Player.MyChi.DragonTime))
                {
                    Removed.Add(Conquer6609.Game.MsgServer.MsgChiInfo.ChiPowerType.Dragon);
                }
            }
            if (client.Player.MyChi.PhoenixTime != 0)
            {
                if (DateTime.Now > DateTime.FromBinary(client.Player.MyChi.PhoenixTime))
                {
                    Removed.Add(Conquer6609.Game.MsgServer.MsgChiInfo.ChiPowerType.Phoenix);
                }
            }
            if (client.Player.MyChi.TigerTime != 0)
            {
                if (DateTime.Now > DateTime.FromBinary(client.Player.MyChi.TigerTime))
                {
                    Removed.Add(Conquer6609.Game.MsgServer.MsgChiInfo.ChiPowerType.Tiger);
                }
            }
            if (client.Player.MyChi.TurtleTime != 0)
            {
                if (DateTime.Now > DateTime.FromBinary(client.Player.MyChi.TurtleTime))
                {
                    Removed.Add(Conquer6609.Game.MsgServer.MsgChiInfo.ChiPowerType.Turtle);
                }
            }
            if(Removed.Count > 0)
            {
                using (var recycle = new ServerSockets.RecycledPacket())
                {
                    var stream = recycle.GetStream();
                    client.Send(stream.CreateExpireNotify(Removed));
                }
            }
        }
    }
}
