using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet TeamArenaInfoPlayersCreate(this ServerSockets.Packet stream, MsgTeamArenaInfoPlayers.KindOfParticipants Mode, uint LeaderUID, uint Count)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Write(LeaderUID);
            stream.Write(Count);
            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemTeamArenaInfoPlayers(this ServerSockets.Packet stream, MsgTournaments.MsgTeamArena.User user)
        {
            stream.Write(user.UID);
            stream.Write((uint)user.Level);
            stream.Write((uint)user.Class);
            stream.Write(user.Mesh);
            stream.Write(user.Info.TodayRank);
            stream.Write(user.Info.ArenaPoints);
            stream.Write(user.Name, 16);

            return stream;
        }
        public static unsafe ServerSockets.Packet TeamArenaInfoPlayersFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTeamArenaInfoPlayers);
            return stream;
        }
    }

    public unsafe class MsgTeamArenaInfoPlayers
    {
        public enum KindOfParticipants : uint
        {
            Neutral = 0,
            Opponents = 1
        }

        [PacketAttribute(GamePackets.MsgTeamArenaInfoPlayers)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            // The Team Qualifier window asks for the members of a team (uint kind, uint leader UID):
            // answer with that team's member list (level, class, rank, points, name).
            uint kind = stream.ReadUInt32();
            uint leaderUID = stream.ReadUInt32();
            if (kind > (uint)KindOfParticipants.Opponents)
                kind = (uint)KindOfParticipants.Neutral;
            Client.GameClient leader;
            if (!Database.Server.GamePoll.TryGetValue(leaderUID, out leader) || leader.Player == null)
                return;
            List<MsgTournaments.MsgTeamArena.User> members = new List<MsgTournaments.MsgTeamArena.User>();
            if (leader.Team != null && leader.Team.TeamLider(leader))
            {
                foreach (var member in leader.Team.GetMembers())
                    if (member != null && member.TeamArenaStatistic != null)
                        members.Add(member.TeamArenaStatistic);
            }
            else if (leader.TeamArenaStatistic != null)
                members.Add(leader.TeamArenaStatistic);
            if (members.Count == 0)
                return;
            stream.TeamArenaInfoPlayersCreate((KindOfParticipants)kind, leaderUID, (uint)members.Count);
            foreach (var member in members)
                stream.AddItemTeamArenaInfoPlayers(member);
            user.Send(stream.TeamArenaInfoPlayersFinalize());
        }
    }
}
