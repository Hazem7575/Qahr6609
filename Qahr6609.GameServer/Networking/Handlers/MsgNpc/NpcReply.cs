using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgNpc
{
    public unsafe static class NpcReply
    {
        public enum InteractTypes : byte
        {
            Dialog = 1,
            Option = 2,
            Input = 3,
            Avatar = 4,
            MessageBox = 6,
            Finish = 100
        }


        public static unsafe ServerSockets.Packet NpcReplyWebCreate(this ServerSockets.Packet stream, string WebSite)
        {
            stream.InitWriter();
            stream.Write(22563468);//0
            stream.Write(0);
            stream.Write((ushort)0);
            stream.Write((ushort)29183);
            stream.Write((byte)1);
            stream.Write((byte)38);
            stream.Write(WebSite, 40);
            stream.Finalize(GamePackets.NpcServerRequest);
            return stream;
        }
        public static unsafe ServerSockets.Packet NpcReplyCreate(this ServerSockets.Packet stream, InteractTypes interactType
            , string text
            , ushort InputMaxLength
            , byte OptionID
            , bool display = true)
        {
            stream.InitWriter();

            stream.Write(10140165);//0
            stream.Write(0);
            stream.Write(InputMaxLength);
            stream.Write((byte)OptionID);
            stream.Write((byte)interactType);
            if (display)
                stream.Write(text);

            stream.Finalize(GamePackets.NpcServerRequest);
            return stream;
        }
    }
}
