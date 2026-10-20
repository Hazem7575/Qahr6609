using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ServerTimerCreate(this ServerSockets.Packet stream)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);
            stream.Write(0);
            DateTime dNow = DateTime.Now;
            stream.Write((uint)(dNow.Year - 1900));
            stream.Write((uint)(dNow.Month - 1));
            stream.Write(dNow.DayOfYear);
            stream.Write(dNow.Day);
            stream.Write(dNow.Hour);
            stream.Write(dNow.Minute);
            stream.Write(dNow.Second);

            stream.Finalize(GamePackets.ServerInfo);
            return stream;
        }

    }
}
