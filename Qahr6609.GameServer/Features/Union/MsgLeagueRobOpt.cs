using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
   /// <summary>
   /// MsgLeagueRobOpt (2642), layout from the 6609 client (CMsgLeagueRobOptPB / CMsgLeagueRobOptServerPB):
   ///   message RobOpt  { uint32 type = 1; uint32 param = 2; repeated RobServer servers = 3; }
   ///   message RobServer { uint32 id = 1; string name = 2; string name2 = 3; }
   /// Types: 0 issue Loot Token, 1 issue after paying the Union Fund shortfall in CPs (server -> client: ask, param = CPs),
   /// 2/7 Loot target list (param 7000 + page), 3/8 Reinforce target list, 4 open the plunder relation dialog,
   /// 5 issue Reinforce Token, 6 same after paying CPs. Handled by Game.MsgTournaments.KingdomWarHandlers.
   /// </summary>
   public static class MsgLeagueRobOpt
    {
       [ProtoContract]
       public class RobServer
       {
           [ProtoMember(1, IsRequired = true)]
           public uint ID { get; set; }

           [ProtoMember(2, IsRequired = false)]
           public string Name { get; set; }

           [ProtoMember(3, IsRequired = false)]
           public string Name2 { get; set; }
       }

       [ProtoContract]
       public class RobOpt
       {
           [ProtoMember(1, IsRequired = true)]
           public uint Type { get; set; }

           [ProtoMember(2, IsRequired = true)]
           public uint Param { get; set; }

           [ProtoMember(3, IsRequired = false)]
           public List<RobServer> Servers { get; set; }

           public RobOpt()
           {
               Servers = new List<RobServer>();
           }
       }

       // ---- Plunder War messages read from the 6609 client (CMsgLeagueRobList 2640, CMsgLeagueBeRob 2641,
       // CMsgTokenUpdate 2643). Each packet must stay within the client's 1024-byte buffer.
       [ProtoContract]
       public class RobItem
       {
           [ProtoMember(1, IsRequired = true)] public uint ServerID { get; set; }
           [ProtoMember(2, IsRequired = false)] public string Name { get; set; }
       }

       /// <summary>2640: Type 0 = Loot Token targets (CDlgLeagueOrderRob), 1 = Reinforce targets (CDlgLeagueOrderAid).
       /// The client reads each row with "%u %s": names must not contain spaces or commas. An empty list is ignored.</summary>
       [ProtoContract]
       public class RobList
       {
           [ProtoMember(1, IsRequired = true)] public uint Type { get; set; }
           [ProtoMember(2, IsRequired = false)] public List<RobItem> Items { get; set; }
           public RobList() { Items = new List<RobItem>(); }
       }

       [ProtoContract]
       public class BeRobUnion
       {
           [ProtoMember(1, IsRequired = true)] public uint Key { get; set; }// serverId * 1000000 + union UID
       }

       /// <summary>2641: adds union keys to the hero's enemy (type 0, 1, 4) or ally (type 2, 3) lists, which drive the
       /// Loot PK mode and name colours, and shows the "Plunder War will end in" banner (types 0-3).</summary>
       [ProtoContract]
       public class BeRob
       {
           [ProtoMember(1, IsRequired = true)] public uint Type { get; set; }
           [ProtoMember(2, IsRequired = false)] public uint Param { get; set; }
           [ProtoMember(3, IsRequired = false)] public uint EndTime { get; set; }// unix seconds
           [ProtoMember(4, IsRequired = false)] public string Text { get; set; }
           [ProtoMember(5, IsRequired = false)] public List<BeRobUnion> Unions { get; set; }
           public BeRob() { Unions = new List<BeRobUnion>(); }
       }

       [ProtoContract]
       public class TokenInfo
       {
           [ProtoMember(1, IsRequired = true)] public uint Token { get; set; }// ini\token_type.dat id (101 Loot, 206 Dragon Shield ...)
           [ProtoMember(2, IsRequired = false)] public uint Used { get; set; }
           [ProtoMember(3, IsRequired = false)] public uint Seconds { get; set; }// timer from now, 0 = none
       }

       /// <summary>2643: refreshes the token window (issue chances left = max - Used, countdown = Seconds).</summary>
       [ProtoContract]
       public class TokenUpdate
       {
           [ProtoMember(1, IsRequired = false)] public List<uint> Ids { get; set; }
           [ProtoMember(2, IsRequired = false)] public List<TokenInfo> Infos { get; set; }
           public TokenUpdate() { Ids = new List<uint>(); Infos = new List<TokenInfo>(); }
       }

       public static unsafe ServerSockets.Packet CreateLeagueRobList(this ServerSockets.Packet stream, RobList obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.LeagueRobList);
           return stream;
       }
       public static unsafe ServerSockets.Packet CreateLeagueBeRob(this ServerSockets.Packet stream, BeRob obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.LeagueBeRob);
           return stream;
       }
       public static unsafe ServerSockets.Packet CreateTokenUpdate(this ServerSockets.Packet stream, TokenUpdate obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.TokenUpdate);
           return stream;
       }

       public static unsafe ServerSockets.Packet CreateLeagueRobOpt(this ServerSockets.Packet stream, RobOpt obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.LeagueRobOpt);

           return stream;
       }
       public static unsafe void GetLeagueRobOpt(this ServerSockets.Packet stream, out RobOpt pQuery)
       {
           pQuery = new RobOpt();
           pQuery = stream.ProtoBufferDeserialize<RobOpt>(pQuery);
       }

       [PacketAttribute(GamePackets.LeagueRobOpt)]
       private static void Process(Client.GameClient user, ServerSockets.Packet stream)
       {
           RobOpt msg;
           try
           {
               stream.GetLeagueRobOpt(out msg);
           }
           catch (Exception)
           {
               return; // malformed protobuf from the client
           }
           try
           {
               if (msg == null || msg.Servers == null || msg.Servers.Count > 64)
                   return;
               MsgTournaments.KingdomWarHandlers.HandleRobOpt(user, stream, msg);
           }
           catch (Exception e)
           {
               MyConsole.SaveException(e);
           }
       }
    }
}
