using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609
{
    public unsafe class SendGlobalPacket
    {
        public unsafe void Enqueue(ServerSockets.Packet data)
        {

            // Enumerating the ConcurrentDictionary itself is lock-free and safe while players log in/out;
            // .Values would lock every bucket and copy the whole list on each call.
            foreach (var kv in Database.Server.GamePoll)
            {
                kv.Value.Send(data);
            }
        }
    }
}
