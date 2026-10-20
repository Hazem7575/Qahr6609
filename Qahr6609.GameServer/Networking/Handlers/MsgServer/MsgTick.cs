using Conquer6609.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgInfo
    {
        public uint UniqId;
        public int Timestamp;
        public int Junk1;
        public int Junk2;
        public int Junk3;
        public int Junk4;
        public uint Hash;
    };
    public unsafe static class MsgTick
    {
        public static unsafe ServerSockets.Packet TickCreate(this ServerSockets.Packet stream, MsgInfo* pQuery)
        {

            stream.InitWriter();

            stream.WriteUnsafe(pQuery, sizeof(MsgInfo));
            stream.Finalize(GamePackets.MsgTick);

            return stream;
        }
        public static unsafe void GetTick(this ServerSockets.Packet stream, MsgInfo* pQuery)
        {
            stream.ReadUnsafe(pQuery, sizeof(MsgInfo));
        }
        public static unsafe ServerSockets.Packet CreateTickData(this ServerSockets.Packet stream, Player role)
        {
            MsgInfo Info = new MsgInfo()
            {
                UniqId = role.UID,
                Timestamp = 0x00,
                Junk1 =  MyMath.Generate(0x1FFFFFFF, 0x7FFFFFFF),
                Junk2 = MyMath.Generate(0x1FFFFFFF, 0x7FFFFFFF),
                Junk3 = MyMath.Generate(0x1FFFFFFF, 0x7FFFFFFF),
                Junk4 = MyMath.Generate(0x1FFFFFFF, 0x7FFFFFFF),
                Hash = 0x00,
            };
            return stream.TickCreate(&Info);
        }
        [PacketAttribute(GamePackets.MsgTick)]
        public static unsafe void TickHandler(Client.GameClient client, ServerSockets.Packet stream)
        {
        }
        private static uint HashName(string Name)
        {
            if (string.IsNullOrEmpty(Name) || Name.Length < 4)
                return 0x9D4B5703;
            else
            {
                byte[] Buffer = global::Conquer6609.AnsiText.Value.GetBytes(Name);
                fixed (byte* pBuf = Buffer)
                    return ((ushort*)pBuf)[0] ^ 0x9823U;
            }
        }
    }
}
