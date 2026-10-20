using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaSignupCreate(this ServerSockets.Packet stream, MsgArenaSignup.DialogType DialogID
            , MsgArenaSignup.DialogButton OptionID, Client.GameClient user)
        {
            stream.InitWriter();

            stream.Write((uint)DialogID);
            stream.Write((uint)OptionID);
           
            stream.Write(user.Player.UID);
            stream.Write((uint)0);//server id
            stream.Write(user.Player.Name, 16);
            stream.Write(user.ArenaStatistic.Info.TodayRank);
            stream.Write((uint)user.Player.Class);
            stream.Write((uint)0);
            stream.Write(user.ArenaStatistic.Info.ArenaPoints);
            stream.Write((uint)user.Player.Level);

            stream.Finalize(GamePackets.MsgArenaSignup);
            return stream;
        }
        public static unsafe void GetArenaSignup(this ServerSockets.Packet stream, out MsgArenaSignup.DialogType DialogID, out MsgArenaSignup.DialogButton OptionID)
        {
            DialogID = (MsgArenaSignup.DialogType)stream.ReadUInt32();
            OptionID = (MsgArenaSignup.DialogButton)stream.ReadUInt32();
        }
    }


    [StructLayout(LayoutKind.Explicit, Size = 60)]
    public unsafe struct MsgArenaSignup
    {
        public enum DialogType : uint
        {
            ArenaIconOn = 0,
            ArenaIconOff = 1,
            ArenaGui = 3,
            StartCountDown = 2,
            OpponentGaveUp = 4,
            BuyPoints = 5,
            Match = 6,
            YouAreKicked = 7,
            StartTheFight = 8,
            Dialog = 9,
            Dialog2 = 10,
            Continue = 11
        }
        public enum DialogButton : uint
        {
            Lose = 3,
            Win = 1,
            DoGiveUp = 2,
            Accept = 1,
            MatchOff = 3,
            MatchOn = 5,
            SignUp = 0
        }
        [PacketAttribute(GamePackets.MsgArenaSignup)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
          
            DialogType DialogID;
            DialogButton OptionID;

            stream.GetArenaSignup(out DialogID, out OptionID);

            switch (DialogID)
            {
                case DialogType.OpponentGaveUp:
                    {
                        switch (OptionID)
                        {
                            case DialogButton.SignUp:
                                {
                                    Game.MsgTournaments.MsgSchedules.Arena.DoQuit(stream, user);
                                    break;
                                }
                        }
                        break;
                    }
                case DialogType.ArenaIconOn:
                    {
                        user.Send(stream.ArenaSignupCreate(DialogID, OptionID, user));
                        Game.MsgTournaments.MsgSchedules.Arena.DoSignup(stream, user);

                        break;
                    }
                case DialogType.ArenaIconOff:
                    {
                        user.Send(stream.ArenaSignupCreate(DialogID, OptionID, user));

                        Game.MsgTournaments.MsgSchedules.Arena.DoQuit(stream, user);
                        break;
                    }
                case DialogType.BuyPoints:
                    {
                        if (user.ArenaPoints <= 1500)
                        {
                            if (user.Player.Money >= 9000000)
                            {
                                user.Player.Money -= 9000000;
                                user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                user.ArenaPoints += 1500;
                                user.Send(stream.ArenaInfoCreate(user.ArenaStatistic.Info));
                            }
                        }
                        break;
                    }
                case DialogType.ArenaGui:
                    {
                        switch (OptionID)
                        {
                            case DialogButton.DoGiveUp: Game.MsgTournaments.MsgSchedules.Arena.DoGiveUp(stream, user); break;
                            case DialogButton.Accept:
                                {
                                    if (user.ArenaStatistic.ArenaState == MsgTournaments.MsgArena.User.StateType.WaitForBox)
                                    {
                                        user.ArenaStatistic.AcceptBox = true;
                                        user.ArenaStatistic.ArenaState = MsgTournaments.MsgArena.User.StateType.WaitForOther;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case DialogType.Continue:
                    {
                        switch (OptionID)
                        {
                            case DialogButton.SignUp:
                                {
                                    Game.MsgTournaments.MsgSchedules.Arena.DoSignup(stream, user);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
