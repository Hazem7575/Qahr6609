using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet BroadcastlistCreate(this ServerSockets.Packet stream, uint dwParam, ushort Total, ushort Count)
        {
            stream.InitWriter();
            stream.Write(dwParam);
            stream.Write(Total);
            stream.Write(Count);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemBroadcastlist(this ServerSockets.Packet stream, MsgTournaments.MsgBroadcast.BroadcastStr str, uint index)
        {
            stream.Write(str.ID);
            stream.Write(index);
            stream.Write(str.EntityID);
            stream.Write(str.EntityName, 16);
            stream.Write(str.SpentCPs);
            stream.Write(str.Message, 80);
            stream.ZeroFill(20);//unknow
            stream.Write(str.UnionRank);
            stream.Write(0);

            //stream.ZeroFill(24);
            return stream;
        }
        public static unsafe ServerSockets.Packet FinalizeBroadcastlist(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgBroadcastliest);
            return stream;
        }
    }
}
