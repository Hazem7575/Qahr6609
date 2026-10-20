using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgSignIn
    {

        public static unsafe void GetMsgSignIn(this ServerSockets.Packet stream, out ActionID Action, out uint dwPram)
        {
            // client layout: [4] byte action, [8] uint param (late sign-in: the day to make up)
            Action = (ActionID)stream.ReadUInt8();
            stream.Seek(8);
            dwPram = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet MsgSignInCreate(this ServerSockets.Packet stream, ActionID Action, byte dwParam, ushort dwParam2, ulong Flags)
        {
            stream.InitWriter();

            stream.Write((byte)Action);
            stream.Write(dwParam);
            stream.Write(dwParam2);
            stream.Write(Flags);
            stream.Finalize(GamePackets.MsgSignIn);
            return stream;
        }
        public enum ActionID : byte
        {
            Show = 3,
            SignIn = 0,
            ClaimReward = 2,
            LateSignIn = 1
        }

        // Official data: client ini/signin.ini (signin.lua). Cumulative sign-in packs, given once per month each,
        // in order, when the month's sign-in count reaches needDays. Prize [6] (needDays 1) is the daily pack.
        public const uint DailyPack = 3100011;
        public static readonly uint[] MilestoneItems = { 3100013, 3100014, 3100015, 3100016, 3100017 };
        public static readonly uint[] MilestoneDays = { 2, 7, 14, 21, 28 };

        // client ini/info.ini [SignIn] NeedEmoney: CPs or CPs(B) per late sign-in
        public const uint LateSignInCost = 15;

        // Late sign-in chances per month by VIP level (client Cn_Res STR_SIGNIN_VIP_2..7).
        public static ushort MonthlyLateSignIns(uint vipLevel)
        {
            switch (vipLevel)
            {
                case 0: return 0;
                case 1: return 1;
                case 2: return 2;
                case 3: return 3;
                case 4: return 5;
                case 5: return 8;
                default: return 10;
            }
        }

        // Player.LateSignIn (saved as "LateSignIn") now holds (month << 8) | late sign-ins used that month, so the
        // chances refill monthly. Old saves stored the VIP level there (month bits 0) and simply start at 0 used.
        private static byte LateSignInUsed(Client.GameClient user)
        {
            if ((user.Player.LateSignIn >> 8) != DateTime.Now.Month)
                return 0;
            return (byte)(user.Player.LateSignIn & 0xFF);
        }
        public static ushort LateSignInLeft(Client.GameClient user)
        {
            int left = MonthlyLateSignIns(user.Player.VipLevel) - LateSignInUsed(user);
            return (ushort)Math.Max(0, left);
        }
        private static void UseLateSignIn(Client.GameClient user)
        {
            int used = LateSignInUsed(user) + 1;
            user.Player.LateSignIn = (ushort)((DateTime.Now.Month << 8) | Math.Min(used, 255));
        }

        // Defensive month rollover (normally done by the daily reset).
        private static void CheckMonth(Client.GameClient user)
        {
            if (user.Player.DailyMonth != DateTime.Now.Month)
            {
                user.Player.DailySignUpRewards = 0;
                user.Player.DailySignUpDays = 0;
                user.Player.DailyMonth = (byte)DateTime.Now.Month;
            }
        }

        public static void SendInfo(Client.GameClient user, ServerSockets.Packet stream)
        {
            user.Send(stream.MsgSignInCreate(ActionID.Show, user.Player.DailySignUpRewards, LateSignInLeft(user), user.Player.DailySignUpDays));
        }

        /// <summary>
        /// Gives every cumulative sign-in pack the player has reached this month and not received yet.
        /// The client has no claim button for them (its sign-in dialog only sends Show / SignIn / LateSignIn),
        /// it shows a reached-but-not-given pack highlighted, so a pack that could not be given because the
        /// bag was full is given on the next sign-in or when the window is opened again.
        /// </summary>
        private static void GiveMilestones(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint days = user.Player.DailyDays;
            while (user.Player.DailySignUpRewards < MilestoneItems.Length
                && days >= MilestoneDays[user.Player.DailySignUpRewards])
            {
                if (!user.Inventory.HaveSpace(1))
                {
                    user.SendSysMesage("Please make some room in your inventory, first.");
                    break;
                }
                uint item = MilestoneItems[user.Player.DailySignUpRewards];
                user.Player.DailySignUpRewards++;
                user.Inventory.Add(stream, item, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
            }
        }

        [PacketAttribute(GamePackets.MsgSignIn)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {

            ActionID Action;
            uint dwPram;
            stream.GetMsgSignIn(out Action, out dwPram);
            CheckMonth(user);
            switch (Action)
            {
                case ActionID.LateSignIn:
                    {
                        if (MonthlyLateSignIns(user.Player.VipLevel) == 0)
                        {
                            user.SendSysMesage("Become a VIP to activate late sign-in function.");
                            break;
                        }
                        if (LateSignInLeft(user) == 0)
                        {
                            user.SendSysMesage("You have no late sign-in chance left this month. Increase VIP level to get more chances of late sign-in.");
                            break;
                        }
                        int today = DateTime.Now.Day;
                        int day = 0;
                        // the client sends the first missed day; fall back to it if the value is not valid
                        if (dwPram >= 1 && dwPram < today && (user.Player.DailySignUpDays & (1ul << (int)(dwPram - 1))) == 0)
                            day = (int)dwPram;
                        else
                        {
                            for (int x = 1; x < today; x++)
                            {
                                if ((user.Player.DailySignUpDays & (1ul << (x - 1))) == 0)
                                {
                                    day = x;
                                    break;
                                }
                            }
                        }
                        if (day == 0)
                        {
                            user.SendSysMesage("There is no missed day to sign in this month.");
                            break;
                        }
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.SendSysMesage("Please make some room in your inventory, first.");
                            break;
                        }
                        if (user.Player.ConquerPoints >= LateSignInCost)
                            user.Player.ConquerPoints -= LateSignInCost;
                        else if (user.Player.BoundConquerPoints >= LateSignInCost)
                            user.Player.BoundConquerPoints -= (int)LateSignInCost;
                        else if (user.Player.ConquerPoints + user.Player.BoundConquerPoints >= LateSignInCost)
                        {
                            // the client allows CPs + CPs(B) together
                            int rest = (int)(LateSignInCost - user.Player.ConquerPoints);
                            user.Player.ConquerPoints = 0;
                            user.Player.BoundConquerPoints -= rest;
                        }
                        else
                        {
                            user.SendSysMesage("You don`t have " + LateSignInCost + " CPs/CPs (B) to complete a late sign-in.");
                            break;
                        }
                        user.Player.DailySignUpDays |= (1ul << (day - 1));
                        UseLateSignIn(user);
                        user.Inventory.Add(stream, DailyPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        GiveMilestones(user, stream);
                        SendInfo(user, stream);
                        break;
                    }
                case ActionID.SignIn:
                    {
                        ulong flag = (1ul << (DateTime.Now.Day - 1));
                        if ((user.Player.DailySignUpDays & flag) == flag)
                        {
                            user.SendSysMesage("You`ve already signed in.");
                            SendInfo(user, stream);
                            break;
                        }
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.SendSysMesage("Please make some room in your inventory, first.");
                            break;
                        }
                        user.Player.DailySignUpDays |= flag;
                        user.Inventory.Add(stream, DailyPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        GiveMilestones(user, stream);
                        SendInfo(user, stream);
                        break;
                    }
                case ActionID.Show:
                case ActionID.ClaimReward:
                    {
                        // only packs already reached and not given yet (bag was full at sign-in time)
                        GiveMilestones(user, stream);
                        SendInfo(user, stream);
                        break;
                    }
            }
        }
    }
}
