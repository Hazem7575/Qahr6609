// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

namespace AccServer.Interfaces
{
    public unsafe interface IPacket
    {
        byte[] ToArray();
        void Deserialize(byte[] buffer);
        void Send(Client.AuthClient client);
    }
}