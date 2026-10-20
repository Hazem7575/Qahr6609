// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Text;

namespace AccServer.Network.AuthPackets
{
    public unsafe class PasswordCryptographySeed : Interfaces.IPacket
    {
        private byte[] Buffer;
        public PasswordCryptographySeed()
        {
            Buffer = new byte[8];
            Network.Writer.WriteUInt16(8, 0, Buffer);
            Network.Writer.WriteUInt16(1059, 2, Buffer);
        }
        public int Seed
        {
            get { return BitConverter.ToInt32(Buffer, 4); }
            set { Network.Writer.WriteInt32(value, 4, Buffer); }
        }
        public void Deserialize(byte[] buffer)
        {
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Send(Client.AuthClient client)
        {
            client.Send(Buffer);
        }
    }
}