using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static unsafe void GetAtributeSet(this ServerSockets.Packet stream, out uint Str, out uint Agi, out uint vit, out uint spi)
        {
            uint timerstamp = stream.ReadUInt32();
            uint unknow = stream.ReadUInt32();

            Str = stream.ReadUInt32();
            Agi = stream.ReadUInt32();
            vit = stream.ReadUInt32();
            spi = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet AtributeSetCreate(this ServerSockets.Packet stream, uint Str, uint Agi, uint vit, uint spi)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);
            stream.Write((uint)0);//unknow
            stream.Write(Str);
            stream.Write(Agi);
            stream.Write(vit);
            stream.Write(spi);

            stream.Finalize(GamePackets.AtributeSet);
            return stream;
        }
    }

    public unsafe struct MsgAtributeSet
    {
        [PacketAttribute(GamePackets.AtributeSet)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {

            uint Str;
            uint Agi;
            uint Vit;
            uint Spi;

            stream.GetAtributeSet(out Str, out Agi, out Vit, out Spi);

            if (user.Player.Atributes == 0)
                return;

            // [Security] summed in 64 bits: a uint sum wrapped (Str=0xFFFF9C40 + Agi=25537 = 1 point for +40000 Str / +25537 Agi)
            ulong total = (ulong)Str + Agi + Vit + Spi;
            if (total == 0 || total > user.Player.Atributes)
                return;
            uint TotalStatPoints = (uint)total;

            if (user.Player.Atributes >= TotalStatPoints)
            {
                user.Player.Strength += (ushort)Str;
                user.Player.Vitality += (ushort)Vit;
                user.Player.Spirit += (ushort)Spi;
                user.Player.Agility += (ushort)Agi;
                user.Player.Atributes -= (ushort)TotalStatPoints;

                user.Send(stream.AtributeSetCreate(Str, Agi, Vit, Spi));

                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
            }
        }
    }
}
