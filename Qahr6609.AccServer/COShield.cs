namespace COShield
{
    using System;
    using System.IO;
    using System.Text;

    public class COShield
    {
        public class Encryption
        {
            private static readonly uint[] keys = new uint[16]
{
    0x3F2A9D1Cu, 0x5C7E8A4Bu, 0x2B1F6D39u, 0x7A4C3E91u,
    0x6D3B1F72u, 0x4E2C8A5Fu, 0x1B9D3C7Au, 0x3A7F9E2Du,
    0x5E1C6B3Au, 0x6F2D7C1Bu, 0x7B4A2E5Cu, 0x2D8C3F91u,
    0x4F9E1B7Au, 0x3C7D2A6Eu, 0x1A5B4E93u, 0x6E3F7D2Cu
};

            public static uint Encrypt(uint value)
            {
                for (int i = 0; i < 16; ++i)
                {
                    value = (uint)((value ^ keys[i]) + keys[15 - i]);
                    value = (value << 5) | (value >> 27);
                    value = (uint)(value ^ (keys[i] >> 16));
                    value = (uint)(value + keys[(i + 3) % 16]);
                }
                return value;
            }

            public static uint Decrypt(uint value)
            {
                for (int i = 15; i >= 0; --i)
                {
                    value = (uint)(value - keys[(i + 3) % 16]);
                    value = (uint)(value ^ (keys[i] >> 16));
                    value = (value >> 5) | (value << 27);
                    value = (uint)((value - keys[15 - i]) ^ keys[i]);
                }
                return value;
            }
        }
        public class Authentication
        {
            public String Server;
            public String Username;
            public String Password;
            public String MacAddress;
            public String ProSerial;
            public Authentication(Byte[] Buffer)
            {
                if (BitConverter.ToUInt16(Buffer, 0) == 312)
                {
                    if (BitConverter.ToUInt16(Buffer, 2) == (ushort)1952)
                    {
                        BinaryReader BR = new BinaryReader(new MemoryStream(Buffer));
                        BR.BaseStream.Seek(8, SeekOrigin.Current);
                        Username = Encoding.Default.GetString(BR.ReadBytes(16)).Replace("\0", "");
                        BR.ReadBytes(31);
                        ProSerial = Encoding.Default.GetString(BR.ReadBytes(12)).Replace("\0", "");
                        BR.ReadBytes(5);
                        Byte SizePassword = BR.ReadByte();
                        Password = PasswordDecryption.Decrypt(BR.ReadBytes(32), SizePassword);
                        BR.ReadBytes(31);
                        Server = Encoding.Default.GetString(BR.ReadBytes(16)).Replace("\0", "");
                        MacAddress = Encoding.Default.GetString(BR.ReadBytes(16)).Replace("\0", "");
                        BR.Close();
                    }
                }
            }
        }
        private class PasswordDecryption
        {
            static Byte[] Own_Key = new Byte[32] { 90, 17, 149, 66, 219, 255, 251, 70, 166, 196, 18, 119, 53, 86, 200, 80, 163, 238, 137, 46, 10, 56, 158, 100, 64, 26, 88, 207, 239, 167, 123, 88 };
            static Byte[] Tow_Key = new Byte[32] { 88, 254, 246, 138, 126, 226, 64, 156, 140, 89, 90, 28, 108, 196, 28, 62, 23, 161, 92, 192, 94, 253, 194, 74, 138, 224, 88, 171, 233, 26, 217, 179 };
            public static String Decrypt(Byte[] Data, Byte Length)
            {
                Byte[] BufferOut = new Byte[Math.Min((Int32)Length, 32)];
                for (Int32 x = 0; x < Math.Min((Int32)Length, 32); x++)
                {
                    BufferOut[x] = (Byte)(Own_Key[x * 44 % 32] ^ Data[x]);
                    BufferOut[x] = (Byte)(Tow_Key[x * 99 % 32] ^ BufferOut[x]);
                }
                return Encoding.Default.GetString(BufferOut).Replace("\0", "");
            }
        }
    }
}