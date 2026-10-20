using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.ServerSockets
{
    public unsafe class Packet : IDisposable
    {

        public const int MAX_SIZE = 16384;
        private const int TQ_SEALSIZE = 8;

        private static string seal;

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void* memset(void* dst, int val, int count);


        public static string SealString
        {
            get { return seal; }
            set
            {

                if (value.Length != TQ_SEALSIZE)
                    throw new ArgumentOutOfRangeException("value", "Packet seal must be " + TQ_SEALSIZE + " chars long.");

                seal = value;
            }
        }

        public static int SealSize
        {
            get { return seal != null ? TQ_SEALSIZE : 0; }
        }

        public int Size { get; set; }
        public byte* Memory { get; private set; }
        private bool IsDisposed = false;
        public byte* stream;

        public int Position { get { return (int)(stream - Memory); } }
        public Packet(byte[] buffer)
        {
            // The allocation always covers the copied buffer.
            Memory = (byte*)Marshal.AllocHGlobal(Math.Max(1024, buffer.Length));
            stream = Memory;
            Marshal.Copy(buffer, 0, (IntPtr)this.stream, buffer.Length);
            this.Size = buffer.Length;
        }
        public Packet(int size)
        {
            Memory = (byte*)Marshal.AllocHGlobal(size);
            stream = Memory;
        }

        ~Packet()
        {
            this.Dispose();
        }
        public void Dispose()
        {
            // lock (this)
            {
                if (this.IsDisposed) return;
                IsDisposed = true;
                if ((IntPtr)this.Memory == IntPtr.Zero)
                    return;
                Marshal.FreeHGlobal((IntPtr)this.Memory);
                this.Memory = null;
                GC.SuppressFinalize(this);
            }
        }

        public void InitWriter()
        {
            Size = 0;// a new packet is being written: no read limit until Finalize sets the size again
            Seek(4);
        }
        public void Seek(int offset)
        {
            // The cursor always stays inside the packet buffer.
            if (offset < 0) offset = 0;
            if (offset > MAX_SIZE) offset = MAX_SIZE;
            stream = &Memory[offset];
        }
        // Upper bound for reads: the received/finalized size when known, otherwise the buffer size.
        private int ReadLimit
        {
            get { return (Size > 0 && Size <= MAX_SIZE) ? Size : MAX_SIZE; }
        }
        public void SeekForward(int amount)
        {
            Seek(Position + amount);
        }
        public void SeekBackwards(int amount)
        {
            Seek(Position - amount);
        }
        public void Write(byte value)
        {
            if (Position + 1 >= MAX_SIZE)
                return;
            *stream = value;
            stream++;
        }
        public void Write(ushort value)
        {
            if (Position + 2 >= MAX_SIZE)
                return;
            *((ushort*)stream) = value;
            stream += sizeof(ushort);
        }
        public void Write(uint value)
        {
            if (Position + 4 >= MAX_SIZE)
                return;
            *((uint*)stream) = value;
            stream += sizeof(uint);
        }
        public void Write(ulong value)
        {
            if (Position + 8 >= MAX_SIZE)
                return;
            *((ulong*)stream) = value;
            stream += sizeof(ulong);
        }
        public void Write(sbyte value) { Write((byte)value); }
        public void Write(short value) { Write((ushort)value); }
        public void Write(int value) { Write((uint)value); }
        public void Write(long value) { Write((ulong)value); }
        public void Write(string value, int length)
        {
            int min = Math.Min(value.Length, length);
            var buf = global::Conquer6609.AnsiText.Value.GetBytes(value);
            for (int i = 0; i < min; i++)
                Write(buf[i]);
            ZeroFill(length - min);
        }
        public void Write(params string[] value)
        {
            Write((byte)value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                var str = value[i];
                if (string.IsNullOrEmpty(str))
                {
                    Write((byte)0);
                    continue;
                }

                Write((byte)str.Length);
                Write(str, str.Length);
            }
        }
        public void WriteUnsafe(void* buf, int length)
        {
            if (Position + length >= MAX_SIZE)
                return;
            memcpy(stream, buf, length);
            stream += length;
        }
        public void ZeroFill(int amount)
        {
            if (Position + amount >= MAX_SIZE)
                return;
            memset(stream, 0, amount);
            stream += amount;
        }
        public string[] ReadStringList()
        {
            int count = ReadUInt8();
            // Each entry needs at least its length byte, so the count is bounded by the remaining bytes.
            int remaining = ReadLimit - Position;
            if (remaining < 0) remaining = 0;
            if (count > remaining) count = remaining;
            var result = new string[count];
            for (int i = 0; i < result.Length; i++)
                result[i] = ReadCString(ReadUInt8());
            return result;
        }
        public void ReadUnsafe(void* buf, int length)
        {
            if (length <= 0 || Position + length >= MAX_SIZE)
                return;
            // Only bytes that were actually received are copied; the rest of the target is zeroed.
            int available = ReadLimit - Position;
            if (available < 0) available = 0;
            int copy = Math.Min(length, available);
            if (copy > 0)
                memcpy(buf, stream, copy);
            for (int i = copy; i < length; i++)
                ((byte*)buf)[i] = 0;
            stream += length;
        }
        public byte[] ReadBytes(int size)
        {
            if (size < 0) size = 0;
            if (size > MAX_SIZE) size = MAX_SIZE;
            byte[] res = new byte[size];
            for (int i = 0; i < res.Length; i++)
                res[i] = ReadUInt8();
            return res;
        }
        // Reads stop at ReadLimit (the received size): the 16 KB buffers are pooled and never cleared, so reading
        // past a short packet used to return bytes of other players' earlier packets (and some handlers echo them).
        public string ReadCString(int size)
        {
            if (size <= 0)
                return "";
            int available = ReadLimit - Position;
            if (available <= 0)
                return "";
            if (size > available)
                size = available;
            string result = new string((sbyte*)this.stream, 0, size);
            stream += size;
            int idx = result.IndexOf('\0');
            return (idx > -1) ? result.Substring(0, idx) : result;
        }
        public byte ReadUInt8()
        {
            if (Position + 1 > ReadLimit)
                return 0;
            var result = *stream;
            stream++;
            return result;
        }
        public ushort ReadUInt16()
        {
            if (Position + 2 > ReadLimit)
                return 0;
            var result = *((ushort*)stream);
            stream += sizeof(ushort);
            return result;
        }
        public uint ReadUInt32()
        {
            if (Position + 4 > ReadLimit)
                return 0;
            var result = *((uint*)stream);
            stream += sizeof(uint);
            return result;
        }
        public ulong ReadUInt64()
        {
            if (Position + 8 > ReadLimit)
                return 0;
            var result = *((ulong*)stream);
            stream += sizeof(ulong);
            return result;
        }
        public sbyte ReadInt8() { return (sbyte)ReadUInt8(); }
        public short ReadInt16() { return (short)ReadUInt16(); }
        public int ReadInt32() { return (int)ReadUInt32(); }
        public long ReadInt64() { return (long)ReadUInt64(); }

        public void Finalize(ushort type)
        {
            if (SealSize > 0)
            {
                WriteSeal();
            }

            this.Size = this.Position;
            Seek(0);
            Write((ushort)(this.Size - SealSize));
            Write((ushort)type);
        }
        public void ProtoBufferSerialize(object obj)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                byte[] array = ms.ToArray();

                fixed (byte* proto_ptr = array)
                    memcpy(stream, proto_ptr, array.Length);

                stream += array.Length;
            }
        }
        public T ProtoBufferDeserialize<T>(T obj)
        {
            Seek(0);
            ushort packet_length = ReadUInt16();
            ReadUInt16();
            byte[] array = new byte[packet_length - 4];
            array = ReadBytes(packet_length - 4);

            using (var ms = new System.IO.MemoryStream(array))
            {
                obj = ProtoBuf.Serializer.Deserialize<T>(ms);
            }
            return obj;
        }
        public T ProtoBufferDeserialize<T>(T obj, byte[] packet)
        {

            using (var ms = new System.IO.MemoryStream(packet))
            {
                obj = ProtoBuf.Serializer.Deserialize<T>(ms);
            }
            return obj;
        }

        public void WriteSeal()
        {
            Write(SealString, TQ_SEALSIZE); // NOTE: must be 8 chars
        }
        private static string CreatePacketStringWithNumbers(byte* b, int len)
        {
           
            StringBuilder hex = new StringBuilder(len);
            for (int z = 0; z < len; z++)
            {
                hex.AppendFormat("{0:x2} ", b[z]);
            }
            return hex.ToString();
        }
        public static string Dump(byte[] b)
        {
            fixed (byte* ptr = b)
                return CreatePacketStringWithNumbers(ptr, b.Length);
        }
        public string Dump(string header)
        {
            int offset = Position; Seek(2);
            int type = ReadUInt16(); Seek(offset);

            string str = "Packet (" + header + ") - Type: " + type + " Size - " + Size + "\r\n";
            str += CreatePacketStringWithNumbers(this.Memory, this.Size);
            return str;
        }

        public unsafe void memcpy(void* dest, void* src, Int32 size)
        {
            Int32 count = size / sizeof(long);
            for (Int32 i = 0; i < count; i++)
                *(((long*)dest) + i) = *(((long*)src) + i);

            Int32 pos = size - (size % sizeof(long));
            for (Int32 i = 0; i < size % sizeof(long); i++)
                *(((Byte*)dest) + pos + i) = *(((Byte*)src) + pos + i);
        }
        public unsafe bool EncryptPipeServer()
        {
            try
            {
                Write((ulong)19562354);
                return true;
            }
            catch
            {
                MyConsole.WriteLine("Error Encrypting Web Server Packet.");
                return false;
            }
        }
        public unsafe bool DecryptPipeServer()
        {
            try
            {
                Seek(Size - 8);
                if (ReadUInt64() == 19562354)
                {
                    Seek(0);
                    return true;
                }
                Seek(0);
                return false;
            }
            catch
            {
                MyConsole.WriteLine("Error Decrypting Web Server Packet.");
            }
            return false;
        }
    }
}
