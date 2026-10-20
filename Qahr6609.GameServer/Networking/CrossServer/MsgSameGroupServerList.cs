using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgSameGroupServerList
    {
        [ProtoContract]
        public class GroupServer
        {
            [ProtoMember(1, IsRequired = true)]
            public Server[] Servers;
        }
        [ProtoContract]
        public class Server
        {
            [ProtoMember(1, IsRequired = false)]
            public uint ServerID;
            [ProtoMember(2, IsRequired = false)]
            public uint dwparam;
            [ProtoMember(3, IsRequired = false)]
            public string Name;
            [ProtoMember(4, IsRequired = false)]
            public uint MapID;
            [ProtoMember(5, IsRequired = false)]
            public uint X;
            [ProtoMember(6, IsRequired = false)]
            public uint Y;
            [ProtoMember(7, IsRequired = false)]
            public uint GroupID;
        }
        public static unsafe ServerSockets.Packet CreateGroupServerList(this ServerSockets.Packet stream, GroupServer obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgSameGroupServerList);
            return stream;
        }
        
        public static unsafe void GetGroupServerList(this ServerSockets.Packet stream, out GroupServer pQuery)
        {
            pQuery = new GroupServer();
            pQuery = stream.ProtoBufferDeserialize<GroupServer>(pQuery);
        }
    }
}
