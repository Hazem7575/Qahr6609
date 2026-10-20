using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetChiHandler(this ServerSockets.Packet stream, out MsgChiInfo.Action Typ
            , out uint CriticalStrike, out uint SkillCriticalStrike,out uint Immunity, out uint Breakthrough
            ,out uint Counteraction,out uint MaxLife, out uint AddAttack,out uint AddMagicAttack
            , out uint AddMagicDefense,out uint FinalAttack,out uint FinalMagicAttack
        , out uint FinalDefense,out uint FinalMagicDefense)
        {
            Typ = (MsgChiInfo.Action)stream.ReadUInt16();
            CriticalStrike = stream.ReadUInt32();
            SkillCriticalStrike = stream.ReadUInt32();
            Immunity = stream.ReadUInt32();
            Breakthrough = stream.ReadUInt32();
            Counteraction = stream.ReadUInt32();
            MaxLife = stream.ReadUInt32();
            AddAttack = stream.ReadUInt32();
            AddMagicAttack = stream.ReadUInt32();
            AddMagicDefense = stream.ReadUInt32();
            FinalAttack = stream.ReadUInt32();
            FinalMagicAttack = stream.ReadUInt32();
            FinalDefense = stream.ReadUInt32();
            FinalMagicDefense = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet ChiInfoCreate(this ServerSockets.Packet stream, MsgChiInfo.Action Typ, Role.Instance.Chi chi)
        {
            stream.InitWriter();
            stream.Write((ushort)Typ);
            stream.Write(chi.CriticalStrike);
            stream.Write(chi.SkillCriticalStrike);
            stream.Write(chi.Immunity);
            stream.Write(chi.Breakthrough);
            stream.Write(chi.Counteraction);
            stream.Write(chi.MaxLife);
            stream.Write(chi.AddAttack);
            stream.Write(chi.AddMagicAttack);
            stream.Write(chi.AddMagicDefense);
            stream.Write(chi.FinalAttack);
            stream.Write(chi.FinalMagicAttack);
            stream.Write(chi.FinalDefense);
            stream.Write(chi.FinalMagicDefense);
            stream.Finalize(GamePackets.ChiInfo);
            return stream;
        }

        public static unsafe void GetChiHandler(this ServerSockets.Packet stream,out uint UID, out MsgChiInfo.MsgHandleChi.ActionHandle Action, out MsgChiInfo.ChiPowerType chipower, out MsgChiInfo.LockedFlags lokedflags)
        {
            UID = stream.ReadUInt32();//4
            Action = (MsgChiInfo.MsgHandleChi.ActionHandle)stream.ReadUInt16();//8
            
            chipower = (MsgChiInfo.ChiPowerType)stream.ReadUInt8();//10
            lokedflags = (MsgChiInfo.LockedFlags)stream.ReadUInt8();//11
        }

        public static unsafe ServerSockets.Packet ChiInfoCreate(this ServerSockets.Packet stream, MsgChiInfo.Action Typ, uint UID, int Points, uint Locks, uint Count)
        {
            stream.InitWriter();
            stream.Write((ushort)Typ);//4
            stream.Write(UID);//6
            stream.Write(Points);//10
            stream.Write(Locks);//14
            stream.Write(Count);//18

            return stream;
        }

        public static unsafe ServerSockets.Packet AddChiItem(this ServerSockets.Packet stream, MsgChiInfo.ChiPowerType Type, int[] Powers)
        {
            stream.Write((byte)Type);//22
            if (Powers != null)
            {
                for (int x = 0; x < Powers.Length; x++)
                {
                    stream.Write(Powers[x]);
                }
            }
            else
                stream.ZeroFill(4 + sizeof(int));


            return stream;
        }

        public static unsafe ServerSockets.Packet ChiFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.ChiInfo);
            return stream;
        }

        public static unsafe ServerSockets.Packet ChiMessageCreate(this ServerSockets.Packet stream, MsgChiInfo.ChiPowerType Typ, int UID,string Name)
        {
            stream.InitWriter();
            stream.Write((byte)Typ);
            stream.Write(UID);
            stream.Write(Name, 16);
            stream.Finalize(GamePackets.ChiMessage);
            return stream;
        }
    }
    public unsafe struct MsgChiInfo
    {
       
        public enum LockedFlags : byte
        {

            None = 0,
            First = 1 << 0,
            Second = 1 << 1,
            Third = 1 << 2,
            Fourth = 1 << 3,
            All = None | First | Second | Third | Fourth
        }
        public enum ChiPowerType : byte
        {
            None = 0,
            Dragon = 1,
            Phoenix = 2,
            Tiger = 3,
            Turtle = 4
        }
        public enum Action : ushort
        {
            Send = 0, Upgrade = 1, InterServerStatus = 2
        }


        public unsafe struct MsgHandleChi
        {
            public enum ActionHandle : ushort
            {
                Unlock = 0,
                Open = 1,
                Study = 2,
                BuyPoints = 3,
                Buy200 = 6,
            }

            public static ExecuteHandler LoginQueue = new ExecuteHandler();

            public class ChiClient
            {
                public Client.GameClient user;
                public uint UID;
                public ActionHandle Action;
                public ChiPowerType UnLocked;
                public LockedFlags LockedFlag;
            }
            public class ExecuteHandler : ConcurrentSmartThreadQueue<ChiClient>
            {

                public ExecuteHandler()
                    : base(5)
                {
                    Start(10);
                }
                public unsafe void TryEnqueue(ChiClient obj)
                {
                    LoginQueue.Enqueue(obj);
                }
                protected unsafe override void OnDequeue(ChiClient obj, int time)
                {
                    Client.GameClient user = obj.user;

                    switch (obj.Action)
                    {
                        case ActionHandle.Open:
                            {
                                if (obj.UID == user.Player.UID)
                                    SendInfo(user, MsgChiInfo.Action.Send);
                                else
                                {
                                    Client.GameClient AttackedView;
                                    if (Database.Server.GamePoll.TryGetValue(obj.UID, out AttackedView))
                                    {
                                        SendInfo(AttackedView, MsgChiInfo.Action.Send, user, user.Player.MyChi.TotalLock);
                                    }
                                }
                                break;
                            }
                        case ActionHandle.Unlock:
                            {
                                var ChiPower = user.Player.MyChi.SingleOrDefault(p => p.Type == obj.UnLocked);
                                if (ChiPower != null && !ChiPower.UnLocked && user.Player.Level >= 110)
                                {
                                    switch (obj.UnLocked)
                                    {
                                        case ChiPowerType.Dragon: ChiPower.UnLocked = (user.Player.Reborn >= 1); break;
                                        case ChiPowerType.Phoenix: ChiPower.UnLocked = (user.Player.Reborn >= 1) && (user.Player.MyChi.Dragon.Score >= 300); break;
                                        case ChiPowerType.Tiger: ChiPower.UnLocked = (user.Player.Reborn >= 2) && (user.Player.MyChi.Phoenix.Score >= 300); break;
                                        case ChiPowerType.Turtle: ChiPower.UnLocked = (user.Player.Reborn >= 2) && (user.Player.MyChi.Tiger.Score >= 300); break;
                                    }
                                    if (ChiPower.UnLocked)
                                    {
                                        if (!Role.Instance.Chi.ChiPool.ContainsKey(user.Player.UID))
                                            Role.Instance.Chi.ChiPool.TryAdd(user.Player.UID, user.Player.MyChi);

                                        ChiPower.UID = user.Player.UID;
                                        ChiPower.Name = user.Player.Name;

                                        if (user.Player.ChargeChi == 1)
                                        {

                                            ChiPower.Reroll(obj.LockedFlag, true);
                                        }
                                        else
                                            ChiPower.Reroll(obj.LockedFlag);
                                        if (ChiPower.Type == ChiPowerType.Dragon)
                                            Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, ChiPower);
                                        else if (ChiPower.Type == ChiPowerType.Phoenix)
                                            Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, ChiPower);
                                        else if (ChiPower.Type == ChiPowerType.Tiger)
                                            Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, ChiPower);
                                        else if (ChiPower.Type == ChiPowerType.Turtle)
                                            Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, ChiPower);

                                        Role.Instance.Chi.ComputeStatus(user.Player.MyChi);
                                        user.Equipment.QueryEquipment(user.Equipment.Alternante, false);

                                        SendInfo(user, MsgChiInfo.Action.Send);

                                    }
                                }
                                break;
                            }
                        case ActionHandle.Study:
                            {
                                var ChiPower = user.Player.MyChi.SingleOrDefault(p => p.Type == obj.UnLocked);
                                if (ChiPower != null && ChiPower.UnLocked && (user.Player.Level >= 110 || user.Player.Reborn > 0))
                                {
                                    int NeedPonints = UsePoints(obj.LockedFlag);
                                    if (user.Player.MyChi.ChiPoints >= NeedPonints)
                                    {
                                        user.Activeness.IncreaseTask(37);

                                        { user.Player.MyChi.ChiPoints -= NeedPonints; Conquer6609.Modules.OperatingActs.OperatingActs.AddPoints(user, Conquer6609.Modules.OperatingActs.OperatingActs.ChiSpent, (uint)NeedPonints); }
                                        if (user.Player.ChargeChi == 1)
                                        {

                                            ChiPower.Reroll(obj.LockedFlag, true);
                                        }
                                        else
                                            ChiPower.Reroll(obj.LockedFlag);
                                        if (ChiPower.Type == ChiPowerType.Dragon)
                                            Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, ChiPower);
                                        else if (ChiPower.Type == ChiPowerType.Phoenix)
                                            Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, ChiPower);
                                        else if (ChiPower.Type == ChiPowerType.Tiger)
                                            Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, ChiPower);
                                        else if (ChiPower.Type == ChiPowerType.Turtle)
                                            Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, ChiPower);

                                        Role.Instance.Chi.ComputeStatus(user.Player.MyChi);
                                        user.Equipment.QueryEquipment(user.Equipment.Alternante, false);

                                        SendInfo(user, MsgChiInfo.Action.Send);
                                        SendInfo(user, MsgChiInfo.Action.Upgrade);
                                    }
                                }
                                break;
                            }
                        case ActionHandle.BuyPoints:
                            {
                                user.Activeness.IncreaseTask(37);
                                int needed = 4000 - user.Player.MyChi.ChiPoints;
                                if (needed == 0) break;
                                uint cost = CostPoints(needed);
                                if (user.Player.ConquerPoints >= cost)
                                {
                                    user.Player.ConquerPoints -= cost;
                                    user.Player.MyChi.ChiPoints += needed;
                                    SendInfo(user, MsgChiInfo.Action.Upgrade);

                                }
                                else
                                {
                                    user.SendSysMesage("Sorry, you need " + cost + " ConquerPoints");
                                    
                                }
                                break;
                            }
                        case ActionHandle.Buy200:
                            {
                                if (user.Player.MyChi.ChiPoints < 4000)
                                {
                                    if (user.Player.ConquerPoints >= 100)//100 for oficial conquer calculation
                                    {
                                        user.Activeness.IncreaseTask(37);

                                        user.Player.MyChi.ChiPoints = user.Player.MyChi.ChiPoints + 200;
                                        user.Player.ConquerPoints -= 100;//100 for oficial conquer calculation

                                        SendInfo(user, MsgChiInfo.Action.Upgrade);
                                    }
                                    else
                                    {
                                        user.SendSysMesage("Sorry, you need " + CostPoints(200) + " ConquerPoints");
                                       
                                    }
                                }
                                break;
                            }

                    }
                }
            }
            [PacketAttribute(GamePackets.HandleChi)]
            private static void HandleChi(Client.GameClient _user, ServerSockets.Packet _stream)
            {
                
                var obj = new ChiClient() { user = _user };
                
                _stream.GetChiHandler(out obj.UID, out obj.Action, out obj.UnLocked, out obj.LockedFlag);
               
                LoginQueue.TryEnqueue(obj);
            }

            public static unsafe void SendInfo(Client.GameClient client, Game.MsgServer.MsgChiInfo.Action Action, Client.GameClient Sender = null, uint Locked = 0)
            {
                var powerArray = client.Player.MyChi.Where(p => p.UnLocked == true).ToArray();

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (client.IsConnectedInterServer())
                        stream.ChiInfoCreate(Action, client.PipeClient.NewUserID, client.Player.MyChi.ChiPoints, Locked, (ushort)powerArray.Count<Role.Instance.Chi.ChiPower>());
                    else
                        stream.ChiInfoCreate(Action, client.Player.UID, client.Player.MyChi.ChiPoints, Locked, (ushort)powerArray.Count<Role.Instance.Chi.ChiPower>());

                    for (int x = 0; x < powerArray.Length; x++)
                    {
                        stream.AddChiItem(powerArray[x].Type, powerArray[x].GetFieldsArray());
                    }

                    if (Sender == null)
                        client.Send(stream.ChiFinalize());
                    else
                        Sender.Send(stream.ChiFinalize());
                }
            }
            public static uint CostPoints(int Amount)
            {
                //return (uint)(Amount * 1.25);
                return (uint)Math.Ceiling(((uint)(Amount / 50) * 12.5)); // oficial conquer calculation
            }
            private static int UsePoints(LockedFlags locked)
            {
                int points = 0;
                if ((locked & LockedFlags.First) == LockedFlags.First)
                    points++;
                if ((locked & LockedFlags.Second) == LockedFlags.Second)
                    points++;
                if ((locked & LockedFlags.Third) == LockedFlags.Third)
                    points++;
                if ((locked & LockedFlags.Fourth) == LockedFlags.Fourth)
                    points++;
                return points * 50 + 50;
            }

        }
    }
}
