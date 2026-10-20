using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet TeamArenaRank10Create(this ServerSockets.Packet stream, Game.MsgTournaments.MsgArena.ArenaIDs ArenaMode)
        {
            stream.InitWriter();
            stream.Write((uint)ArenaMode);
            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemTeamArenaRank10(this ServerSockets.Packet stream, Game.MsgTournaments.MsgTeamArena.User user)
        {
            stream.Write(user.Name, 16);
            stream.Write(user.LastSeasonRank);
            stream.Write(user.Mesh);
            stream.Write((uint)user.Class);
            stream.Write((uint)user.Level);
            stream.Write(user.LastSeasonArenaPoints);
            stream.Write(user.LastSeasonWin);
            stream.Write(user.LastSeasonLose);

            return stream;
        }
        public static unsafe ServerSockets.Packet TeamArenaRank10Finalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTeamArenaRank10);
            return stream;
        }
    }
    public unsafe struct MsgTeamArenaRank10
    {

        [PacketAttribute(GamePackets.MsgTeamArenaRank10)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                var info = Game.MsgTournaments.MsgTeamArena.Top10.Where(p => p != null).ToArray();
                int count = info.Length;

                stream.TeamArenaRank10Create(MsgTournaments.MsgArena.ArenaIDs.ShowPlayerRankList);
                for (int x = 0; x < count; x++)
                {
                    var element = info[x];
                    stream.AddItemTeamArenaRank10(element);
                }
                stream.TeamArenaRank10Finalize();
                user.Send(stream);
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
