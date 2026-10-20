using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet UpdateProfExperienceCreate(this ServerSockets.Packet stream, uint Experience, uint UID, uint ID)
        {
            stream.InitWriter();

            stream.Write(Experience);//4
            stream.Write(UID);//8
            stream.Write(ID);//12
            stream.ZeroFill(8);//unknow

            stream.Finalize(GamePackets.UpgradeSpellExperience);
            return stream;
        }
    }
}
