using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static class MsgPopupInfo
    {
        public static unsafe ServerSockets.Packet PopupInfoCreate(this ServerSockets.Packet stream, uint UID, uint TargerUID, uint Level, int BattlePower)
        {
            stream.InitWriter();
            stream.Write(UID);
            stream.Write(TargerUID);
            stream.Write(Level);
            stream.Write(BattlePower);
            stream.ZeroFill(12);
            stream.Finalize(GamePackets.PopupInfo);

            return stream;
        }
    }
}
