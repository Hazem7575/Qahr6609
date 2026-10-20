using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaRank10Create(this ServerSockets.Packet stream, MsgTournaments.MsgArena.ArenaIDs Typ)
        {
            stream.InitWriter();

            stream.Write((uint)Typ);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemArenaRank10(this ServerSockets.Packet stream, Game.MsgTournaments.MsgArena.User user)
        {
            stream.Write(user.UID);
            stream.Write(user.Name, 16);
            stream.Write(user.Mesh);
            stream.Write((uint)user.Level);
            stream.Write((uint)user.Class);
            stream.Write(user.LastSeasonRank);
            stream.Write(user.LastSeasonRank);
            stream.Write(user.LastSeasonArenaPoints);
            stream.Write(user.LastSeasonWin);
            stream.Write(user.LastSeasonLose);
            return stream;
        }
        public static unsafe ServerSockets.Packet ArenaRank10Finalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgArenaRank10);
            return stream;
        }
    }
    public unsafe struct MsgArenaRank10
    {
        [PacketAttribute(GamePackets.MsgArenaRank10)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                var info = Game.MsgTournaments.MsgArena.Top10.Where(p => p != null).ToArray();

                int count = info.Length;

                stream.ArenaRank10Create(MsgTournaments.MsgArena.ArenaIDs.ShowPlayerRankList);


                for (int x = 0; x < count; x++)
                {
                    var element = info[x];
                    stream.AddItemArenaRank10(element);

                }
                user.Send(stream.ArenaRank10Finalize());
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
