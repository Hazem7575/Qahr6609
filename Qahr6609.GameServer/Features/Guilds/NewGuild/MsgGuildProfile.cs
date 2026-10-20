using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public class MsgGuildProfile
    {
        public ushort Length;
        public ushort PacketID;
        public uint Stamp;//?
        public uint MoneyDonate;
        public uint ConquerPoints;
        public uint AllDonation;
        public uint PkDonation;
        public uint ArsenalDonaion;
        public uint RedRouses;
        public uint Tulips;
        public uint Lilies;
        public uint Orchids;
        public uint AllFlowersDonation;
      

        [PacketAttribute(Game.GamePackets.GuildInfo)]
        public unsafe static void HandlerGuildInfo(Client.GameClient user, ServerSockets.Packet stream)
        {
       

            if (user.Player.MyGuildMember != null)
            {
                stream.InitWriter();
                stream.Write(uint.MaxValue);
                stream.Write((uint)user.Player.MyGuildMember.MoneyDonate);
                stream.Write(user.Player.MyGuildMember.CpsDonate);
                stream.Write(user.Player.MyGuildMember.TotalDonation);
                stream.Write(user.Player.MyGuildMember.PkDonation);
                stream.Write(user.Player.MyGuildMember.ArsenalDonation);
                stream.Write(user.Player.MyGuildMember.Rouses);
                stream.Write(user.Player.MyGuildMember.Tulips);
                stream.Write(user.Player.MyGuildMember.Lilies);
                stream.Write(user.Player.MyGuildMember.Orchids);
                stream.Write(user.Player.MyGuildMember.AllFlowers);
               // stream.ZeroFill(36);

                stream.Finalize(GamePackets.GuildInfo);

                user.Send(stream);
            }
        }
    }
}
