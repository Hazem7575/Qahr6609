using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
   public static class MsgLeagueToken
    {
       [ProtoContract]
       public class LeagueToken
       {
           [ProtoMember(1, IsRequired = true)]
           public uint Type;
           [ProtoMember(2, IsRequired = true)]
           public uint dwpram2;
           [ProtoMember(3, IsRequired = true)]
           public uint dwpram3;
           [ProtoMember(4, IsRequired = true)]
           public uint Time;
           [ProtoMember(5, IsRequired = true)]
           public string text;
       }
       public static unsafe ServerSockets.Packet CreateLeagueToken(this ServerSockets.Packet stream, LeagueToken obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.LeagueToken);
           return stream;
       }
       public static unsafe void GetLeagueToken(this ServerSockets.Packet stream, out LeagueToken pQuery)
       {
           pQuery = new LeagueToken();
           pQuery = stream.ProtoBufferDeserialize<LeagueToken>(pQuery);
       }


       // Client requests (CMsgLeagueTokenPB): Type 0 = issue a token (dwpram2 = own UID, dwpram3 = 102 Summon / 204 Shift),
       // Type 2 = answer a Summon / Shift call (dwpram2 = UID of the issuer, dwpram3 = token).
       // Server notice to the called heroes: Type 1, dwpram2 = issuer UID, dwpram3 = token, Time = seconds to answer, text = issuer name.
       [PacketAttribute(GamePackets.LeagueToken)]
       private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
       {
           LeagueToken pQuery;
           try
           {
               stream.GetLeagueToken(out pQuery);
           }
           catch
           {
               return;// malformed protobuf
           }
           if (pQuery == null || !user.Player.InUnion)
               return;
           if (pQuery.dwpram3 != Role.Instance.Union.SummonTokenID && pQuery.dwpram3 != Role.Instance.Union.ShiftTokenID)
               return;
           switch (pQuery.Type)
           {
               case 0:
                   if (pQuery.dwpram2 != user.Player.UID)
                       return;
                   user.Player.MyUnion.IssueSummon(user, stream, pQuery.dwpram3);
                   break;
               case 2:
                   Role.Instance.Union.AnswerSummon(user, pQuery.dwpram2, pQuery.dwpram3);
                   break;
           }
       }
    }
}
