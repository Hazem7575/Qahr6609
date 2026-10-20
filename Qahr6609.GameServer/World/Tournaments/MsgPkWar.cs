using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgPkWar
    {
        public const int RewardConquerPoints = ServerRate.Weeklypk, FinishMinutes = 20;// Activity.ini ids 14/37/54: Enter 20:00~20:19

        private ProcesType Mode;
        private DateTime FinishTimer = new DateTime();


        public uint WinnerUID = 0;
        public uint WinnerMonthlyUID = 0;
        // Official (client Activity.ini): Weekly PK War every Sunday 20:00-20:19 (EU slot), Monthly PK War on the
        // 1st of every month 20:00-20:19. True while the current/last war is the monthly one.
        public bool Monthly = false;
        public MsgPkWar()
        {
            Mode = ProcesType.Dead;
        }

        /// <summary>GM / console start: monthly on the 1st of the month, weekly otherwise.</summary>
        public void Open()
        {
            Open(DateTime.Now.Day == 1);
        }
        public void Open(bool monthly)
        {
            if (Mode == ProcesType.Dead)
            {
                Mode = ProcesType.Idle;
                Monthly = monthly;

                if (monthly)
                    MsgSchedules.SendInvitation("Monthly PK War", "CPs,PowerExpBall", 324, 194, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.MonthlyPK);
                else
                    MsgSchedules.SendInvitation("Weekly PK War", "CPs,PowerExpBall", 324, 194, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.WeeklyPK);

                FinishTimer = DateTime.Now.AddMinutes(FinishMinutes);

            }
        }
        public bool AllowJoin()
        {
            return Mode == ProcesType.Idle;
        }

        public void CheckUp()
        {
            if (Mode == ProcesType.Idle)
            {
                if (DateTime.Now > FinishTimer)
                {
                    Mode = ProcesType.Dead;
                    MsgSchedules.SendSysMesage((Monthly ? "The Monthly PK War" : "The Weekly PK War") + " has started! Sign-up is now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                }
            }
        }
        public bool IsFinished() { return Mode == ProcesType.Dead; }
        public bool TheLastPlayer()
        {
            return Database.Server.GamePoll.Values.Where(p => p.Player.Map == 1508 && p.Player.DynamicID == 0 && p.Player.Alive).Count() == 1;// 1508 copies (DynamicID != 0) = Desert Clan War
        }
        public void GiveReward(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!Monthly)
            {
                WinnerUID = client.Player.UID;
                client.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints and 2 PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                MsgSchedules.SendSysMesage("" + client.Player.Name + " Won  PK War , he received " + RewardConquerPoints.ToString() + " ConquerPoints and 2PowerExpBalls!", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);
                client.Player.ConquerPoints += RewardConquerPoints;
                if (client.Inventory.HaveSpace(1))
                    client.Inventory.Add(stream, 3003235);
                else
                    client.Inventory.AddReturnedItem(stream, 3003235);

                AddTop(client);
                client.Teleport(302, 169, 1002, 0);
            }
            else
            {
                WinnerMonthlyUID = client.Player.UID;
                client.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints and 2 PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                MsgSchedules.SendSysMesage("" + client.Player.Name + " Won  PK War , he received " + RewardConquerPoints.ToString() + " ConquerPoints and 2PowerExpBalls!", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);
                client.Player.ConquerPoints += RewardConquerPoints;
                if (client.Inventory.HaveSpace(1))
                    client.Inventory.Add(stream, 722057);
                else
                    client.Inventory.AddReturnedItem(stream, 722057);

                AddTop(client);
                client.Teleport(302, 169, 1002, 0);
            }
        }
        public void AddTop(Client.GameClient client)
        {
            if (WinnerUID == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.WeeklyPKChampion, Role.StatusFlagsBigVector32.WeeklyFlag, false);
            if (WinnerMonthlyUID == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.MonthlyPKChampion, Role.StatusFlagsBigVector32.WeeklyFlag, false);
        }
    }
}
