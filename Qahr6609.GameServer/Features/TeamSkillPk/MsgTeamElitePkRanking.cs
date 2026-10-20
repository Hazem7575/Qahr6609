using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet TeamElitePkRankingCreate(this ServerSockets.Packet stream, MsgTeamElitePkRanking.RankType rank
            , uint Group, MsgTeamElitePKBrackets.GuiTyp GroupStatus, uint Count, uint UID)
        {
            stream.InitWriter();

            stream.Write((uint)rank);//4
            stream.Write(Group);//8
            stream.Write((uint)GroupStatus);//12
            stream.Write(Count);//16
            stream.Write((uint)0);//skipping the offests to offest 28
           //stream.Write((ulong)0);//skipping the offests to offest 28

            return stream;
        }
        public static unsafe ServerSockets.Packet SkillElitePkRankingCreate(this ServerSockets.Packet stream, MsgTeamElitePkRanking.RankType rank
            , uint Group, MsgTeamElitePKBrackets.GuiTyp GroupStatus, uint Count, uint UID)
        {
            stream.InitWriter();

            stream.Write((uint)rank);//4
            stream.Write(Group);//8
            stream.Write((uint)GroupStatus);//12
            stream.Write(Count);//16
            return stream;
        }
        public static unsafe ServerSockets.Packet AdditemTeamElitePkRanking(this ServerSockets.Packet stream, MsgTournaments.MsgTeamEliteGroup.FighterStats status, uint Rank)
        {
            stream.Write(status.LeaderUID);
            stream.Write(Rank);
            stream.Write(status.Name, 32);
            stream.Write(status.LeaderMesh);
            stream.Write(0);
            return stream;
        }
        public static unsafe ServerSockets.Packet AdditemSkillElitePkRanking(this ServerSockets.Packet stream, MsgTournaments.MsgTeamEliteGroup.FighterStats status, uint Rank)
        {
            stream.Write(status.LeaderUID);
            stream.Write(Rank);
            stream.Write(status.Name, 32);
            stream.Write(status.LeaderMesh);
            return stream;
        }
        public static unsafe ServerSockets.Packet TeamElitePkRankingFinalize(this ServerSockets.Packet stream, ushort ID)
        {
            stream.ZeroFill(420 - stream.Position);
            stream.Finalize(ID);
            return stream;
        }
        public static unsafe void GetTeamElitePkRanking(this ServerSockets.Packet stream, out uint Group)
        {
            uint first = stream.ReadUInt32();
            Group = stream.ReadUInt32();
        }
    }
    public class MsgTeamElitePkRanking
    {
        public enum RankType : uint
        {
            Top8 = 0,
            Top3 = 2
        }
        [PacketAttribute(GamePackets.TeamElitePkTop)]
        private static void PorocesTeamPkRanking(Client.GameClient user, ServerSockets.Packet stream)
        {

            try
            {
                uint Group;

                stream.GetTeamElitePkRanking(out Group);

                var tournament = MsgTournaments.MsgTeamPkTournament.EliteGroups[Math.Min(3, (int)Group)];

                if (tournament.Top8 == null)
                    return;
                if (tournament.Top8.Length == 0)
                    return;



                if (tournament.State >= MsgTeamElitePKBrackets.GuiTyp.GUI_Top1)
                {
                    if (tournament.State == MsgTeamElitePKBrackets.GuiTyp.GUI_Top1)
                    {
                        if (tournament.Top8[2] != null)
                        {

                            stream.TeamElitePkRankingCreate(RankType.Top3, Group, tournament.State, 1, user.Player.UID);

                            stream.AdditemTeamElitePkRanking(tournament.Top8[2], 3);
                            user.Send(stream.TeamElitePkRankingFinalize(GamePackets.TeamElitePkTop));

                        }
                    }
                    else
                    {
                        stream.TeamElitePkRankingCreate(RankType.Top3, Group, tournament.State, 3, user.Player.UID);

                        for (int i = 0; i < 3; i++)
                            stream.AdditemTeamElitePkRanking(tournament.Top8[i], (uint)(i + 1));

                        user.Send(stream.TeamElitePkRankingFinalize(GamePackets.TeamElitePkTop));

                    }
                }
                else
                {

                    stream.TeamElitePkRankingCreate(RankType.Top8, Group, tournament.State, 8, user.Player.UID);

                    for (int i = 0; i < 8; i++)
                    {
                        if (tournament.Top8[i] == null)
                            break;
                        stream.AdditemTeamElitePkRanking(tournament.Top8[i], (uint)(i + 1));
                    }
                    user.Send(stream.TeamElitePkRankingFinalize(GamePackets.TeamElitePkTop));
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        [PacketAttribute(GamePackets.SkillElitePkTop)]
        private static void PorocesSkillTeamPkRanking(Client.GameClient user, ServerSockets.Packet stream)
        {
            //return;
            try
            {
                uint Group;

                stream.GetTeamElitePkRanking(out Group);

                var tournament = MsgTournaments.MsgSkillTeamPkTournament.EliteGroups[Math.Min(3, (int)Group)];

                if (tournament.Top8 == null)
                    return;
                if (tournament.Top8.Length == 0)
                    return;

                //if (tournament.Top8[0] == null)
                //    return;

                if (tournament.State >= MsgTeamElitePKBrackets.GuiTyp.GUI_Top1)
                {
                    if (tournament.State == MsgTeamElitePKBrackets.GuiTyp.GUI_Top1)
                    {
                        if (tournament.Top8[2] != null)
                        {

                            stream.SkillElitePkRankingCreate(RankType.Top3, Group, tournament.State, 1, user.Player.UID);

                            stream.AdditemSkillElitePkRanking(tournament.Top8[2], 3);
                            user.Send(stream.TeamElitePkRankingFinalize(GamePackets.SkillElitePkTop));

                        }
                    }
                    else
                    {
                        stream.SkillElitePkRankingCreate(RankType.Top3, Group, tournament.State, 3, user.Player.UID);

                        for (int i = 0; i < 3; i++)
                            stream.AdditemSkillElitePkRanking(tournament.Top8[i], (uint)(i + 1));

                        user.Send(stream.TeamElitePkRankingFinalize(GamePackets.SkillElitePkTop));

                    }
                }
                else
                {

                    stream.SkillElitePkRankingCreate(RankType.Top8, Group, tournament.State, 8, user.Player.UID);

                    for (int i = 0; i < 8; i++)
                    {
                        if (tournament.Top8[i] == null)
                            break;
                        stream.AdditemSkillElitePkRanking(tournament.Top8[i], (uint)(i + 1));
                    }
                    user.Send(stream.TeamElitePkRankingFinalize(GamePackets.SkillElitePkTop));
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
