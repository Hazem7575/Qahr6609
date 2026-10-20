using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet MapStatusCreate(this ServerSockets.Packet stream, uint MapID, uint BaseID, ulong Status)
        {
            stream.InitWriter();

            stream.Write(MapID);//4
            stream.Write(BaseID);//8

            //uint update = 1U << 31;
            //Console.WriteLine(update);
            stream.Write(Status);

            stream.Finalize(GamePackets.MapStaus);
            return stream;
        }
    }
}
