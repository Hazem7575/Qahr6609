using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {


        public static void GetInterServerIdentifier(this ServerSockets.Packet stream, out uint mode,out uint dwparam1,out uint dwparam2)
        {
            mode = stream.ReadUInt32();
            dwparam1 = stream.ReadUInt32();
            dwparam2 = stream.ReadUInt32();
        }


        public static unsafe ServerSockets.Packet MsgInterServerIdentifier(this ServerSockets.Packet stream, uint mode, uint dwparam1, uint dwparam2
            , Game.MsgServer.MsgGameItem[] items = null)
        {
            stream.InitWriter();
            stream.Write(mode);
            stream.Write(dwparam1);
            stream.Write(dwparam2);
          /*  if (items != null)
            {
                stream.Write(items.Length);
                foreach (var item in items)
                {
                    stream.Write(item.UID);
                    stream.Write(item.UID);
                }
            }*/
            stream.ZeroFill(8);
            stream.Finalize(2501);
    
            return stream;
        }
    }
}
