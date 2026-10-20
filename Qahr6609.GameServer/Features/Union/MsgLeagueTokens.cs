using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
   public static class MsgLeagueTokens
    {
        [ProtoContract]
        public class GroupToken
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Count;
            [ProtoMember(2, IsRequired = true)]
            public Token[] Tokens;
        }
        [ProtoContract]
        public class Token
        {
            [ProtoMember(1, IsRequired = true)]
            public uint param1;
            [ProtoMember(2, IsRequired = true)]
            public uint param2;
            [ProtoMember(3, IsRequired = true)]
            public uint param3;

        }
        public static unsafe ServerSockets.Packet CreateLeagueTokens(this ServerSockets.Packet stream, GroupToken obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.LeagueGroupTokens);
            return stream;
        }
        public static unsafe void GetLeagueTokens(this ServerSockets.Packet stream, out GroupToken pQuery)
        {
            pQuery = new GroupToken();
            pQuery = stream.ProtoBufferDeserialize<GroupToken>(pQuery);
        }


    }
}
