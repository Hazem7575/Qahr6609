using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InterActionWalk
    {
        public MsgInterAction.Action Mode;//4
        public byte DirectionOne;//6
        public byte DirectionTwo;//7
      
        public ushort X
        {
            get { fixed (void* ptr = &DirectionOne) { return *((ushort*)ptr); } }
            set { fixed (void* ptr = &DirectionOne) { *((ushort*)ptr) = value; } }
        }
        
        public ushort Y;
        public ushort UnKnow;
        public uint UnKnow2;//static 2
        public uint UID;
        public uint OponentUID;
    }

   public static class MsgInterAction
    {
       public enum Action : ushort
       {
           Walk = 1,
           Jump = 2
       }


       public static unsafe ServerSockets.Packet InterActionWalk(this ServerSockets.Packet stream, InterActionWalk* pQuery)
       {

           stream.InitWriter();

           pQuery->UnKnow2 = 2;
           stream.WriteUnsafe(pQuery, sizeof(InterActionWalk));

           stream.Finalize(GamePackets.InterAction);

           return stream;
       }

    }
}
