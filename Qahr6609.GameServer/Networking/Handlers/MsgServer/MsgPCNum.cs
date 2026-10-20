using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe struct MsgPCNum
    {
        public ushort Length;
        public ushort PacketID;
        public uint Identifier;
        public string MacAddress;
        [PacketAttribute(Game.GamePackets.MsgPCNum)]
        public unsafe static void CheckPC(Client.GameClient client, ServerSockets.Packet packet)
        {
          
        }
    }
}
