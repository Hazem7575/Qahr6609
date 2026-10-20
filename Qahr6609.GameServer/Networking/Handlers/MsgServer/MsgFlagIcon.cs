using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{

    public static unsafe partial class MsgBuilder
    {

        public static unsafe void GetFlagIcon(this ServerSockets.Packet stream, out uint Type, out uint UID, out MsgFlagIcon.ShowIcon Icon
            , out uint Level, out uint Mode, out uint Damage)
        {
            uint timerstamp = stream.ReadUInt32();
            Type = stream.ReadUInt32();
            UID = stream.ReadUInt32();
            Icon = (MsgFlagIcon.ShowIcon) stream.ReadUInt32();
            Level = stream.ReadUInt32();
            Mode = stream.ReadUInt32();
            Damage = stream.ReadUInt32();

        }

        public static unsafe ServerSockets.Packet FlagIconCreate(this ServerSockets.Packet stream,  uint UID, MsgFlagIcon.ShowIcon Icon
            ,  uint Level, uint Damage)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);
            stream.Write(3);
            stream.Write(UID);
            stream.Write((uint)Icon);
            stream.Write(Level);
            stream.Write(30);
            stream.Write(Damage);
            
            stream.Finalize(GamePackets.FlagIcon);

            return stream;
        }
    }

    public struct MsgFlagIcon
    {
        public enum ShowIcon : uint
        {
            TyrantAura = 1,
            FeandAura = 2,
            MetalAura = 3,
            WoodAura = 4,
            WaterAura = 5,
            FireAura = 6,
            EarthAura = 7,
            MagicDefender = 8
        }
        [PacketAttribute(GamePackets.FlagIcon)]
        private unsafe static void MsgFlagIconHandler(Client.GameClient user, ServerSockets.Packet stream)
        {
             uint Type;  uint UID;MsgFlagIcon.ShowIcon Icon;
             uint Level; uint Mode; uint Damage;

             stream.GetFlagIcon(out Type, out UID, out Icon, out Level, out Mode, out Damage);
            user.Send(stream.FlagIconCreate(UID, Icon, Level, Damage));
        }
    }
}
