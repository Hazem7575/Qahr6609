using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
   public static class MsgLeagueAllegianceList
    {
       public static unsafe ServerSockets.Packet LeagueAllegianceListCreate(this ServerSockets.Packet stream, uint Count, ushort Page, ushort PageCount, byte DwParam)
        {
            stream.InitWriter();
            stream.Write(Count);
            stream.Write(Page);
            stream.Write(PageCount);
            stream.Write(DwParam);
            return stream;
        }
     
       public static unsafe ServerSockets.Packet AddItemLeagueAllegianceList(this ServerSockets.Packet stream, ulong Fund,
          uint UID, uint GoldBricks, string Name, string LeaderName, string RecruitDeclaration, byte IsKingdom)
        {

            stream.Write((ulong)Fund);
            stream.Write(UID);
            stream.Write(GoldBricks);
            stream.Write(Name,16);
            stream.Write(LeaderName,16);
            stream.Write(RecruitDeclaration, 256);
            return stream;
        }
       public static unsafe ServerSockets.Packet LeagueAllegianceListFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.LeagueAllegianceList);
            return stream;
        }
    }
}
