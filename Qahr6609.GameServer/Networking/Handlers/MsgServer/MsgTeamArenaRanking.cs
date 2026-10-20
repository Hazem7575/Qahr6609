using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetTeamArenaRanking(this ServerSockets.Packet stream, out ushort PageNumber)
        {
            PageNumber = stream.ReadUInt16();
        }

        public static unsafe ServerSockets.Packet TeamArenaRankingCreate(this ServerSockets.Packet stream, ushort PageNumber
            , ushort Count, MsgTournaments.MsgArena.ArenaIDs ArenaID)
        {
            stream.InitWriter();
            stream.Write(PageNumber);
            stream.Write(Count);
            stream.Write((uint)ArenaID);

            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemTeamArenaRanking(this ServerSockets.Packet stream, uint Rank
            , uint Points, uint Class, uint Level, byte Gender, string name)
        {
            stream.Write(Rank);
            stream.Write(2979015);
            stream.Write(Points);
            stream.Write(Class);
            stream.Write(Level);
            stream.Write(Gender);
            stream.Write(name, 16);
            stream.Write((byte)0);
            stream.Write((ushort)0);
            return stream;
        }
        public static unsafe ServerSockets.Packet MsgTeamArenaRankingFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTeamArenaRanking);
            return stream;
        }
    }

    public unsafe struct MsgTeamArenaRanking
    {
        [PacketAttribute(GamePackets.MsgTeamArenaRanking)]
        public static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {

                ushort PageNumber;

                stream.GetTeamArenaRanking(out PageNumber);


                var info = Game.MsgTournaments.MsgTeamArena.Top1000Today.Where(p => p != null).ToArray();

                const int max = 10;
                int offset = (PageNumber - 1) * max;
                int count = Math.Min(max, Math.Max(0, info.Length - offset));

                stream.TeamArenaRankingCreate(PageNumber, (ushort)info.Length, MsgTournaments.MsgArena.ArenaIDs.ShowPlayerRankList);

                for (int x = 0; x < count; x++)
                {
                    if (info.Length > offset + x)
                    {
                        var element = info[offset + x];

                        if (element.Info.TodayRank < 1000)
                        {
                            stream.AddItemTeamArenaRanking((ushort)(offset + x + 1), element.Info.ArenaPoints
                                , element.Class, element.Level, element.GetGender, element.Name);

                        }
                    }
                }
                user.Send(stream.MsgTeamArenaRankingFinalize());
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
