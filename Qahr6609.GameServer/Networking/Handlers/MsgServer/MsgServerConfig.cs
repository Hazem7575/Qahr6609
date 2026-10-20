using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
   public static class MsgServerConfig
    {
       public static unsafe ServerSockets.Packet ServerConfig(this ServerSockets.Packet stream)
       {
           /*Packet Nr 1083. Server -> Client, Length : 21, PacketType: 1049
0D 00 19 04 00 00 00 00 01 0F 00 00 00 54 51 53      ;
        TQS
65 72 76 65 72                                       ;erver*/
           stream.InitWriter();

           stream.Write(0);
           stream.Write((byte)0);
           stream.Write(15);
           stream.Finalize(GamePackets.CMsgPCServerConfig);
           return stream;
       }
    }
}
