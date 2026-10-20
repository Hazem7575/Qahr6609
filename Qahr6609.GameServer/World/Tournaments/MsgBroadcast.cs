using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgBroadcast
    {
        public const int MaxBroadcasts = 50;

        public static Extensions.Time32 TimerStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.BroadCastStamp);


        public static Extensions.Counter BroadcastCounter = new Extensions.Counter(1);

        public struct BroadcastStr
        {
            public uint ID;
            public uint EntityID;
            public string EntityName;
            public uint SpentCPs;
            public string Message;
            public uint UnionRank;
        }

        public static DateTime LastBroadcast = DateTime.Now;

        public static BroadcastStr CurrentBroadcast = new BroadcastStr() { EntityID = 1 };

        /// <summary>Guards <see cref="Broadcasts"/> and <see cref="CurrentBroadcast"/>: the packet handler (socket thread) and Work (KernelThread) both change them.</summary>
        public static readonly object SyncRoot = new object();

        public static List<BroadcastStr> Broadcasts = new List<BroadcastStr>();

        public static void Create()
        {

        }
        public unsafe static void Work(Extensions.Time32 clock)
        {
            if (clock > TimerStamp)
            {
                DateTime Now = DateTime.Now;

                if (Now > LastBroadcast.AddMinutes(1))
                {
                    bool hasNext;
                    BroadcastStr next = default(BroadcastStr);
                    lock (SyncRoot)
                    {
                        hasNext = Broadcasts.Count > 0;
                        if (hasNext)
                        {
                            next = Broadcasts[0];
                            Broadcasts.RemoveAt(0);
                            CurrentBroadcast = next;
                        }
                        else
                            CurrentBroadcast.EntityID = 1;
                    }
                    if (hasNext)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(next.Message, "ALLUSERS", next.EntityName, MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream, next.UnionRank));
                        }
                    }

                    LastBroadcast = DateTime.Now;
                }
                TimerStamp.Value = clock.Value + KernelThread.BroadCastStamp;
            }

        }
   
    }
}
