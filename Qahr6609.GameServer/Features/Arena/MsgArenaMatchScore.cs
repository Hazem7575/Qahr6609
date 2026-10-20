using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaMatchScoreCreate(this ServerSockets.Packet stream, uint UidOne, string nameone, uint damageone
            , uint UidTwo, string nametwo, uint damagetwo)
        {
            stream.InitWriter();

            stream.Write((uint)0);//server
            stream.Write(UidOne);
           
            stream.Write(nameone, 16);
            stream.Write(damageone);


            stream.Write((uint)0);//server
            stream.Write(UidTwo);
           
            stream.Write(nametwo, 16);
            stream.Write(damagetwo);

            stream.Write((uint)0);

            stream.Finalize(GamePackets.MsgArenaMatchScore);
            return stream;
        }

    }


}
