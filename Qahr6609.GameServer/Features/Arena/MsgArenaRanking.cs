using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaRankingCreate(this ServerSockets.Packet stream, MsgArenaRanking.RankTyp rank, ushort page
            , uint Count, MsgTournaments.MsgArena.ArenaIDs ArenaID)
        {
            stream.InitWriter();

            stream.Write((ushort)rank);
            stream.Write(page);
            stream.Write(Count);
            stream.Write((uint)ArenaID);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemArenaRanking(this ServerSockets.Packet stream, ushort Rank, string Name, ushort UnKnow, uint Points,
            uint Class, uint level, uint unknow2)
        {
            stream.Write(Rank);
            stream.Write(Name, 16);
            stream.Write(UnKnow);
            stream.Write(Points);
            stream.Write(Class);
            stream.Write(level);
            stream.Write(unknow2);
            return stream;
        }
        public static unsafe ServerSockets.Packet ArenaRankingFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgArenaRanking);
            return stream;
        }
        public static unsafe void GetArenaRanking(this ServerSockets.Packet stream, out MsgArenaRanking.RankTyp rank, out ushort PageNumber)
        {
            rank = (MsgArenaRanking.RankTyp) stream.ReadUInt16();
            PageNumber = stream.ReadUInt16();
        }
    }

    public unsafe struct MsgArenaRanking
    {
        public enum RankTyp : ushort
        {
            Today = 0,
            History = 1
        }
        [PacketAttribute(GamePackets.MsgArenaRanking)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgArenaRanking.RankTyp SubType; ushort PageNumber;

            stream.GetArenaRanking(out SubType, out PageNumber);


            switch (SubType)
            {
                case RankTyp.Today:
                    {
                        var info = Game.MsgTournaments.MsgArena.Top1000Today.Where(p => p != null).ToArray();

                        const int max = 10;
                        int offset = (PageNumber - 1) * max;
                        int count = Math.Min(max, Math.Max(0, info.Length - offset));


                        stream.ArenaRankingCreate(RankTyp.Today, PageNumber, (uint)info.Length, MsgTournaments.MsgArena.ArenaIDs.ShowPlayerRankList);

                        for (int x = 0; x < count; x++)
                        {
                            if (info.Length > offset + x)
                            {
                                var element = info[offset + x];

                                if (element.Info.TodayRank < 1000)
                                {
                                    stream.AddItemArenaRanking((ushort)(offset + x + 1), element.Name, 0, element.Info.ArenaPoints
                                        , element.Class, element.Level, 1);
                                }
                            }
                        }
                        user.Send(stream.ArenaRankingFinalize());

                        break;
                    }
                case RankTyp.History:
                    {
                        var info = Game.MsgTournaments.MsgArena.Top1000.Where(p => p != null).ToArray();

                        const int max = 10;
                        int offset = (PageNumber - 1) * max;
                        int count = Math.Min(max, Math.Max(0, info.Length - offset));

                        stream.ArenaRankingCreate(RankTyp.History, PageNumber, (uint)info.Length, MsgTournaments.MsgArena.ArenaIDs.ShowPlayerRankList);

                        for (int x = 0; x < count; x++)
                        {
                            if (info.Length > offset + x)
                            {
                                var element = info[offset + x];

                                if (element.Info.TodayRank < 1000)
                                {
                                    stream.AddItemArenaRanking((ushort)(offset + x + 1), element.Name, 6004, element.Info.HistoryHonor
                                  , element.Class, element.Level, 33942209);
                                }
                            }
                        }
                        user.Send(stream.ArenaRankingFinalize());
                       
                        break;
                    }

            }
        }
    }
}
