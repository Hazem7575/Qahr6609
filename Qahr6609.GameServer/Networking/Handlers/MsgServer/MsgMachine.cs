using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetMachine(this ServerSockets.Packet stream, out MsgMachine.SlotMachineSubType Mode, out byte BetMultiplier, out uint NpcID)
        {
            Mode = (MsgMachine.SlotMachineSubType)stream.ReadUInt8();
            BetMultiplier = stream.ReadUInt8();
            stream.ReadUInt16();
            NpcID = stream.ReadUInt32();
        }

    }
    public struct MsgMachine
    {
        public enum SlotMachineSubType : byte
        {
            StartSpin = 0,
            StopSpin = 1,
            ClientFinishSpin = 2
        }

        [PacketAttribute(Game.GamePackets.MsgMachine)]
        public unsafe static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgMachine.SlotMachineSubType Mode;
            byte BetMultiplier;
            uint NpcID;
           
            stream.GetMachine(out Mode, out BetMultiplier, out NpcID);
          
            switch (Mode)
            {
                case SlotMachineSubType.StartSpin:
                    {
                        MsgNpc.Npc npc;
                        if (user.Map.SearchNpcInScreen(NpcID, user.Player.X, user.Player.Y, out npc))
                        {

                                if (npc.Mesh / 10 >= 1977 && npc.Mesh / 10 <= 1980)
                                {
                                    int id = npc.Mesh / 10 - 1977;
                                    uint cost = 10000;
                                    bool cps = id != 0;
                                    if (id == 1) cost = 3;
                                    if (id == 2) cost = 10;
                                    if (id == 3) cost = 100;
                                    cost *= BetMultiplier;
                                    if ((cps && user.Player.ConquerPoints >= cost) || (!cps && user.Player.Money >= cost))
                                    {
                                        if (cps)
                                        {
                                            user.Player.ConquerPoints -= cost;
                                       
                                        }
                                        else
                                        {
                                            user.Player.Money -= cost;
                                            user.Player.SendUpdate(stream,user.Player.Money, MsgUpdate.DataType.Money);
                                        }
                                        user.SlotMachine = new Role.Instance.SlotMachine(NpcID, (uint)cost, cps);
                                        user.SlotMachine.SpinTheWheels();
                                        user.SlotMachine.SendWheelsToClient(user,stream);
                                    }

                                }
                                else
                                {
                                    if (npc.Mesh / 10 >= 2313 && npc.Mesh / 10 <= 2316)
                                    {
                                        int id = npc.Mesh / 10 - 2313;
                                        uint cost = 10000;
                                        bool cps = id != 0;
                                        if (id == 1) cost = 3;
                                        if (id == 2) cost = 10;
                                        if (id == 3) cost = 100;
                                        cost *= BetMultiplier;
                                        if ((cps && user.Player.ConquerPoints >= cost) || (!cps && user.Player.Money >= cost))
                                        {
                                            if (cps)
                                            {
                                                user.Player.ConquerPoints -= cost;
                                              
                                            }
                                            else
                                            {
                                                user.Player.Money -= cost;
                                                user.Player.SendUpdate(stream,user.Player.Money, MsgUpdate.DataType.Money);
                                            }
                                            user.SlotMachine = new Role.Instance.SlotMachine(NpcID, (uint)cost, cps);
                                            user.SlotMachine.SpinTheWheels();
                                            user.SlotMachine.SendWheelsToClient(user, stream);
                                        }

                                    }
                                }
                        }
                        break;
                    }
                case SlotMachineSubType.ClientFinishSpin:
                    {
                        if (user.SlotMachine != null)
                        {
                            uint reward = user.SlotMachine.GetRewardAmount(user,stream);
                            if (user.SlotMachine.Cps)
                            {
                                user.Player.ConquerPoints += reward;
                      
                            }
                            else
                            {
                                user.Player.Money += reward;
                                user.Player.SendUpdate(stream,user.Player.Money, MsgUpdate.DataType.Money);
                            }

                            user.Send(stream.MachineResponseCreate(SlotMachineSubType.StopSpin, (byte)user.SlotMachine.Wheels[0], (byte)user.SlotMachine.Wheels[1], (byte)user.SlotMachine.Wheels[2], NpcID));
                            user.SlotMachine = null;
                        }
                        break;
                    }
            }
        }
    }
}
