using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
   public static class MsgLeagueMainRank
    {
       [Flags]
       public enum RankType : byte
       {
           None = 0,
           Union =1,
           Kingdom = 4
       }
       [ProtoContract]
       public class MsgUnionRank
       {
           [ProtoMember(1, IsRequired = true)]
           public uint dwparam1;
           [ProtoMember(2, IsRequired = true)]
           public uint UID;
           [ProtoMember(3, IsRequired = true)]
           public RankType Type;
           [ProtoMember(4, IsRequired = true)]
           public uint Leader;
           [ProtoMember(5, IsRequired = true)]
           public uint IsKingdom;
           [ProtoMember(6, IsRequired = true)]
           public uint dwparam6;
           [ProtoMember(7, IsRequired = true)]
           public string Name;
       }

       public static unsafe ServerSockets.Packet CreateLeagueMainRank(this ServerSockets.Packet stream, MsgUnionRank obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.LeagueMainRank);
           return stream;
       }
       public static unsafe void GetLeagueMainRank(this ServerSockets.Packet stream, out MsgUnionRank pQuery)
       {
           pQuery = new MsgUnionRank();
           pQuery = stream.ProtoBufferDeserialize<MsgUnionRank>(pQuery);
       }

    }
}
