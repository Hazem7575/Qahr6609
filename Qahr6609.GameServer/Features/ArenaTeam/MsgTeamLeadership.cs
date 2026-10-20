using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TeamLeadership
    {
        public MsgTeamLeadership.Mode Typ;
        public uint UID;
        public uint LeaderUID;
        public int Count;
        public uint UnKnow;
    }

    public unsafe static class MsgTeamLeadership
    {
        public enum Mode : uint
        {
            Leader = 1,
            Teammate = 2
        }
        public static unsafe void GetTeamLeadership(this ServerSockets.Packet stream, TeamLeadership* pQuery)
        {
            stream.ReadUnsafe(pQuery, sizeof(TeamLeadership));
        }

        public static unsafe ServerSockets.Packet TeamLeadershipCreate(this ServerSockets.Packet stream, TeamLeadership* pQuery)
        {
            stream.InitWriter();

            stream.WriteUnsafe(pQuery, sizeof(TeamLeadership));

            stream.Finalize(GamePackets.LeaderSchip);

            return stream;
        }

        [PacketAttribute(GamePackets.LeaderSchip)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            TeamLeadership action;

            stream.GetTeamLeadership(&action);

            user.Send(stream.TeamLeadershipCreate(&action));

            if (user.Team != null)
                user.Team.AutoInvite = action.UID == 1;
        }
    }
}
