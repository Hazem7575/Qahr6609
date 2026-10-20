using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetTeamArenaSignup(this ServerSockets.Packet stream, out MsgTeamArenaSignup.DialogType DType
            , out MsgTeamArenaSignup.DialogButton DButton)
        {
            DType = (MsgTeamArenaSignup.DialogType)stream.ReadUInt32();
            DButton = (MsgTeamArenaSignup.DialogButton)stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet TeamArenaSignupCreate(this ServerSockets.Packet stream, MsgTeamArenaSignup.DialogType DType
            , MsgTeamArenaSignup.DialogButton DButton, Client.GameClient client)
        {
            stream.InitWriter();
            stream.Write((uint)DType);
            stream.Write((uint)DButton);
            stream.Write(client.Player.UID);
            stream.Write(client.Player.Name, 16);
            stream.Write(client.TeamArenaStatistic.Info.TodayRank);
            stream.Write((uint)client.Player.Class);
            stream.Write(0);
            stream.Write(client.TeamArenaStatistic.Info.ArenaPoints);
            stream.Write((uint)client.Player.Level);

            stream.Finalize(GamePackets.MsgTeamArenaSignup);

            return stream;
        }


    }

    [StructLayout(LayoutKind.Explicit, Size = 60)]
    public unsafe struct MsgTeamArenaSignup
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

        [PacketAttribute(GamePackets.MsgTeamArenaSignup)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgTeamArenaSignup.DialogType DialogID;
            MsgTeamArenaSignup.DialogButton OptionID;

            stream.GetTeamArenaSignup(out DialogID, out OptionID);

            switch (DialogID)
            {
                case DialogType.OpponentGaveUp:
                    {
                        switch (OptionID)
                        {
                            case DialogButton.SignUp:
                                {
                                    Game.MsgTournaments.MsgSchedules.TeamArena.DoQuit(user);
                                    break;
                                }
                        }
                        break;
                    }
                case DialogType.ArenaIconOn:
                    {
                        user.Send(stream.TeamArenaSignupCreate(DialogID, OptionID, user));
                        Game.MsgTournaments.MsgSchedules.TeamArena.DoSignup(user);

                        break;
                    }
                case DialogType.ArenaIconOff:
                    {
                        user.Send(stream.TeamArenaSignupCreate(DialogID, OptionID, user));

                        Game.MsgTournaments.MsgSchedules.TeamArena.DoQuit(user);
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
                            case DialogButton.DoGiveUp: Game.MsgTournaments.MsgSchedules.TeamArena.DoGiveUp(user); break;
                            case DialogButton.Accept:
                                {
                                    if (user.Team != null)
                                    {
                                        if (user.Team.ArenaState == Role.Instance.Team.StateType.WaitForBox)
                                        {
                                            user.Team.AcceptBox = true;
                                            user.Team.ArenaState = Role.Instance.Team.StateType.WaitForOther;
                                        }
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
                                    Game.MsgTournaments.MsgSchedules.TeamArena.DoSignup(user);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
