using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.MsgInterServer.Packets
{
   public static class JiangHuInfo
    {
        public static unsafe ServerSockets.Packet InterServerJiangHuCreate(this ServerSockets.Packet stream, string text)
        {
            stream.InitWriter();
            stream.Write(text.Length);
            stream.Write(text, text.Length);

            stream.Finalize(PacketTypes.InterServer_JiangHu);
            return stream;
        }
        public static unsafe void GetInterServerJiangHu(this ServerSockets.Packet stream, out string text)
        {
            int size = stream.ReadInt32();
            text = stream.ReadCString(size);
        }
    }
}
