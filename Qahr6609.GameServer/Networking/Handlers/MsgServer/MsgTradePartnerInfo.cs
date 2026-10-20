using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static unsafe ServerSockets.Packet TradePartnerInfoCreate(this ServerSockets.Packet stream, Client.GameClient user)
        {
            stream.InitWriter();

            stream.Write(user.Player.UID);//4
            stream.Write(user.Player.Mesh);//8
            stream.Write((byte)user.Player.Level);//12
            stream.Write(user.Player.Class);//13
            stream.Write(user.Player.PKPoints);//14
            stream.Write(user.Player.GuildID);//16
            stream.Write((ushort)user.Player.GuildRank);//20
            stream.Write((uint)0);
            stream.Write(user.Player.Spouse, 16);

            stream.Write((uint)0);
            stream.Write((ushort)0);

            stream.Finalize(GamePackets.TratePartnerInfo);

            return stream;
        }
    }
}
