using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaMatchesCreate(this ServerSockets.Packet stream, uint Page, MsgTournaments.MsgArena.ArenaIDs Typ
            , uint MarchesCount, uint PlayersCount, uint PageCount)
        {
            stream.InitWriter();

            stream.Write(Page);
            stream.Write((uint)Typ);
            stream.Write(MarchesCount);
            stream.Write(PlayersCount);
            stream.Write((uint)0);
            stream.Write(PageCount);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemArenaMatches(this ServerSockets.Packet stream, Game.MsgTournaments.MsgArena.User user1, Game.MsgTournaments.MsgArena.User user2)
        {

      
            stream.Write(user1.UID);
            stream.Write(0);//server
            stream.Write(user1.Mesh);
            stream.Write(user1.Name, 16);
            stream.Write((uint)user1.Level);
            stream.Write((uint)user1.Class);
            stream.Write((uint)0);
            stream.Write(user1.Info.TodayRank);
            stream.Write(user1.Info.ArenaPoints);
            stream.Write(user1.Info.TotalLose);
            stream.Write(user1.Info.TodayBattles);
            stream.Write(user1.Info.CurrentHonor);
            stream.Write(user1.Info.HistoryHonor);

            
            stream.Write(user2.UID);
            stream.Write(0);//server
            stream.Write(user2.Mesh);
            stream.Write(user2.Name, 16);
            stream.Write((uint)user2.Level);
            stream.Write((uint)user2.Class);
            stream.Write((uint)0);
            stream.Write(user2.Info.TodayRank);
            stream.Write(user2.Info.ArenaPoints);
            stream.Write(user2.Info.TotalLose);
            stream.Write(user2.Info.TodayBattles);
            stream.Write(user2.Info.CurrentHonor);
            stream.Write(user2.Info.HistoryHonor);
            return stream;
        }
        public static unsafe ServerSockets.Packet ArenaMatchesFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgArenaMatches);
            return stream;
        }


        public static unsafe void GetArenaMatches(this ServerSockets.Packet stream, out uint Page)
        {
            Page = stream.ReadUInt32();
        }
    }
    public unsafe struct MsgArenaMatches
    {
        [PacketAttribute(GamePackets.MsgArenaMatches)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {

                uint aPage;
                stream.GetArenaMatches(out aPage);

                var info = MsgTournaments.MsgSchedules.Arena.MatchesRegistered.Values.Where(p => p.dinamicID != 0).ToArray();
                int Page = (int)(aPage / 6);

                const int max = 6;
                int offset = Page * max;
                int count = Math.Min(max, Math.Max(0, info.Length - offset));

                stream.ArenaMatchesCreate(aPage, MsgTournaments.MsgArena.ArenaIDs.QualifierList, (uint)info.Length, (uint)MsgTournaments.MsgSchedules.Arena.Registered.Count()
                    , (uint)(info.Length - (aPage - 1)));

                for (int x = 0; x < count; x++)
                {
                    if (info.Length > offset + x)
                    {
                        var element = info[offset + x];

                        stream.AddItemArenaMatches(element.Players[0].ArenaStatistic, element.Players[1].ArenaStatistic);
                    }
                }
                user.Send(stream.ArenaMatchesFinalize());

            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
