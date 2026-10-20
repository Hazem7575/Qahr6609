using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ElitePKWatchCreate(this ServerSockets.Packet stream, MsgArenaWatchers.WatcherTyp typ, uint dwParam
            , uint ID, uint WatcherCount, uint dwCheers1, uint dwCheers2)
        {
            stream.InitWriter();

            stream.Write((ushort)typ);
            stream.Write(dwParam);
            stream.Write(ID);
            stream.Write(WatcherCount);
            stream.Write(dwCheers1);
            stream.Write(dwCheers2);
          //  stream.Write((uint)0);
          //  stream.Write((uint)0);
            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemElitePKWatch(this ServerSockets.Packet stream, uint mesh, string name)
        {
            stream.Write(mesh);
            
            stream.Write(name, 32);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemElitePKWatch(this ServerSockets.Packet stream, uint mesh,uint ServerID, string name)
        {
            stream.Write(mesh);

            stream.Write(name, 32);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemElitePKWatch(this ServerSockets.Packet stream, string name, uint ServerID)
        {
            stream.Write((uint)0);
            stream.Write(name, 32);
            return stream;
        }
        public static unsafe ServerSockets.Packet ElitePKWatchFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgElitePkWatch);
            return stream;
        }
    }
}
