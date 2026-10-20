using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
   public class SlotMachine
    {
       public enum SlotMachineItems : byte
       {
           Stancher = 0,
           Meteor = 1,
           Sword = 2,
           TwoSwords = 3,
           SwordAndShield = 4,
           ExpBall = 5,
           DragonBall = 6
       }
        public static readonly int[] Rates = new int[8] 
        {
            80, //Stancher
            540, //Meteor
            120, //Sword
            180, //TwoSwords
            210, //SwordAndShield
            180, //ExpBall
            600, //DragonBall
            200, //3s line
        };

        public SlotMachineItems[] Wheels = new SlotMachineItems[3];

        public uint NPCID;
        public uint BetAmount;
        public bool Cps;
        public SlotMachine(uint npcid, uint betamount, bool cps = false)
        {
            NPCID = npcid;
            BetAmount = betamount;
            Cps = cps;
        }

        int GetAmount(SlotMachineItems Item)
        {
            int count = 0;
            foreach (SlotMachineItems item in Wheels)
                if (item == Item)
                    count++;
            return count;
        }

        private int GetSLCount()
        {
            return GetAmount(SlotMachineItems.Sword) + GetAmount(SlotMachineItems.SwordAndShield) + GetAmount(SlotMachineItems.TwoSwords) + GetAmount(SlotMachineItems.DragonBall);
        }
        private bool IsSL(SlotMachineItems item)
        {
            return item == SlotMachineItems.DragonBall || item == SlotMachineItems.Sword || item == SlotMachineItems.SwordAndShield || item == SlotMachineItems.TwoSwords;
        }

        public unsafe uint GetRewardAmount(Client.GameClient client,ServerSockets.Packet stream)
        {
            uint win = 0;
            if (GetAmount(SlotMachineItems.DragonBall) == 3)
            {
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won the jackpot from the 1-Arm Bandit!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
               
                client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, "accession5");
                if (Cps) return BetAmount * 3000;
                else return BetAmount * 1000;
            }
            if (GetAmount(SlotMachineItems.ExpBall) == 3)
                win = BetAmount * 60;
            else if (GetAmount(SlotMachineItems.SwordAndShield) == 3)
                win = BetAmount * 40;
            else if (GetAmount(SlotMachineItems.TwoSwords) == 3)
                win = BetAmount * 20;
            else if (GetAmount(SlotMachineItems.Sword) == 3)
                win = BetAmount * 10;
            else if (GetAmount(SlotMachineItems.Meteor) == 2 || GetAmount(SlotMachineItems.DragonBall) == 2)
                win = BetAmount * 5;
            else if (GetAmount(SlotMachineItems.Meteor) == 1 || GetAmount(SlotMachineItems.DragonBall) == 1)
                win = BetAmount * 2;

            if (Cps)
            {
                if (GetAmount(SlotMachineItems.DragonBall) == 1) 
                    win *= 3;
                else if (GetAmount(SlotMachineItems.DragonBall) == 2) 
                    win *= 9;
            }
            if (win == 0)
                if(Cps)
                client.GainExpBall(BetAmount * 3, true);
            return win;
        }
        public bool Rate(int value, int discriminant)
        {
            return value > Program.GetRandom.Next() % discriminant;
        }
        public void SpinTheWheels()
        {
          
                int wheelPick;
                bool canwin = false;
                if (Core.Rate(30))
                    canwin = true;
                for (int i = 2; i >= 0; i--)
                {
                    while (true)
                    {
                        wheelPick = Program.GetRandom.Next(0, 7);
                        if (canwin)
                        {
                            if (Rate(1, Rates[wheelPick]))
                            {
                                if (i == 0)
                                    if (GetSLCount() == 2 && IsSL((SlotMachineItems)wheelPick) || (GetAmount((SlotMachineItems)wheelPick) == 3 - GetAmount(SlotMachineItems.DragonBall)))
                                        if (!Rate(1, Rates[7]))
                                            continue;
                                if (Cps)
                                {
                                    if ((SlotMachineItems)wheelPick == SlotMachineItems.DragonBall)
                                    {
                                        if (Core.Rate(30))
                                            Wheels[i] = (SlotMachineItems)wheelPick;
                                        else
                                            Wheels[i] = SlotMachineItems.Meteor;
                                    }
                                }
                                else
                                    Wheels[i] = (SlotMachineItems)wheelPick;

                                break;
                            }
                        }
                        else
                        {
                            Wheels[i] = (SlotMachineItems)0;
                            if (Core.Rate(20))
                                Wheels[i] = SlotMachineItems.SwordAndShield;
                            break;
                        }
                    }
                }
             
        }
        public unsafe void SendWheelsToClient(Client.GameClient client,ServerSockets.Packet packet)
        {
            client.Send(packet.MachineResponseCreate(MsgMachine.SlotMachineSubType.StartSpin, (byte)Wheels[0], (byte)Wheels[1], (byte)Wheels[2], NPCID));
        }
    }
}
