using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609.ServerSockets
{
    public class RecycledPacket : IDisposable
    {
        private Packet stream;
        public Packet GetStream() { return stream; }
        public RecycledPacket()
        {
            this.stream = PacketRecycle.Take();
        }
        ~RecycledPacket()
        {
            this.Dispose();
        }
        public void Dispose()
        {
            if (stream == null)
                return;
            PacketRecycle.Reuse(stream);
            stream = null;
            GC.SuppressFinalize(this);
        }
    }
    public static unsafe class PacketRecycle
    {
        public static int Count
        {
            get { return System.Threading.Volatile.Read(ref binCount); }
        }
        private static ConcurrentQueue<Packet> bin = new ConcurrentQueue<Packet>();
        private static int binCount;
        // [Performance] PacketPoolMax: packets (16 KB native memory each) kept for reuse; extra ones are left
        // to the garbage collector instead of growing the pool forever after a burst.
        private static readonly int MaxPooled = Math.Max(64, Conquer6609.Framework.Settings.Int("Performance", "PacketPoolMax", 2048));
        public static Packet Take()
        {
            Packet from;
            while (bin.TryDequeue(out from))
            {
                System.Threading.Interlocked.Decrement(ref binCount);
                if (from != null && (IntPtr)from.Memory != IntPtr.Zero)
                {
                    from.Size = 0;// the size of its previous use must not limit reads
                    from.Seek(4);
                    return from;
                }
            }
            from = new Packet(Packet.MAX_SIZE);
            from.Seek(4);
            return from;
        }
        public static void Reuse(Packet old)
        {
            // A packet whose native memory was already freed (finalized before its RecycledPacket) must
            // never be handed out again.
            if (old == null || (IntPtr)old.Memory == IntPtr.Zero)
                return;
            if (System.Threading.Volatile.Read(ref binCount) >= MaxPooled)
                return;
            old.Seek(0);
            System.Threading.Interlocked.Increment(ref binCount);
            bin.Enqueue(old);
        }
    }
}
