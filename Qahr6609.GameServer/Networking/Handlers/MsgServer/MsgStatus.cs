using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetStatus(this ServerSockets.Packet stream, out uint dwParam)
        {
            uint timer = stream.ReadUInt32();
            dwParam = stream.ReadUInt32();
        }
        public static unsafe void GetJiangStatus(this ServerSockets.Packet stream, out uint type,out uint UID, out MsgStatus status)
        {
            type = stream.ReadUInt32();
            UID = stream.ReadUInt32();
            status = new MsgStatus();
            status.MaxHitpoints = stream.ReadUInt32();
            status.MaxMana = stream.ReadUInt32();
            status.MaxAttack = stream.ReadUInt32();
            status.MinAttack = stream.ReadUInt32();
            status.Defence = stream.ReadUInt32();
            status.MagicAttack = stream.ReadUInt32();
            status.MagicDefence = stream.ReadUInt32();
            status.Dodge = stream.ReadUInt32();
            status.AgilityAtack = stream.ReadUInt32();
            status.Accuracy = stream.ReadUInt32();
            status.PhysicalPercent = stream.ReadUInt32();
            status.MagicPercent = stream.ReadUInt32();
            status.MDefence = stream.ReadUInt32();
            status.Damage = stream.ReadUInt32();
            status.ItemBless = stream.ReadUInt32();
            status.CriticalStrike = stream.ReadUInt32();
            status.SkillCStrike = stream.ReadUInt32();
            status.Immunity = stream.ReadUInt32();
            status.Penetration = stream.ReadUInt32();
            status.Block = stream.ReadUInt32();
            status.Breakthrough = stream.ReadUInt32();
            status.Counteraction = stream.ReadUInt32();
            status.Detoxication = stream.ReadUInt32();
            status.PhysicalDamageIncrease = stream.ReadUInt32();
            status.MagicDamageIncrease = stream.ReadUInt32();
            status.PhysicalDamageDecrease = stream.ReadUInt32();
            status.MagicDamageDecrease = stream.ReadUInt32();
            status.MetalResistance = stream.ReadUInt32();
            status.WoodResistance = stream.ReadUInt32();
            status.WaterResistance = stream.ReadUInt32();
            status.FireResistance = stream.ReadUInt32();
            status.EarthResistance = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet StatusJiangHuCreate(this ServerSockets.Packet stream, MsgStatus status)
        {
            stream.InitWriter();

            stream.Write((uint)1);//timer -> we abusing this for inter server
            stream.Write(status.UID);//8
            stream.Write(status.MaxHitpoints);
            stream.Write(status.MaxMana);
            stream.Write(status.MaxAttack);
            stream.Write(status.MinAttack);
            stream.Write(status.Defence);
            stream.Write(status.MagicAttack);
            stream.Write(status.MagicDefence);
            stream.Write(status.Dodge);
            stream.Write(status.AgilityAtack);
            stream.Write(status.Accuracy);
            stream.Write(status.PhysicalPercent);
            stream.Write(status.MagicPercent);
            stream.Write(status.MDefence);
            stream.Write(status.Damage);
            stream.Write(status.ItemBless);

            stream.Write(status.CriticalStrike);
            stream.Write(status.SkillCStrike);
            stream.Write(status.Immunity);
            stream.Write(status.Penetration);
            stream.Write(status.Block);
            stream.Write(status.Breakthrough);
            stream.Write(status.Counteraction);
            stream.Write(status.Detoxication);
            stream.Write(status.PhysicalDamageIncrease);
            stream.Write(status.MagicDamageIncrease);
            stream.Write(status.PhysicalDamageDecrease);
            stream.Write(status.MagicDamageDecrease);
            stream.Write(status.MetalResistance);
            stream.Write(status.WoodResistance);
            stream.Write(status.WaterResistance);
            stream.Write(status.FireResistance);
            stream.Write(status.EarthResistance);

            stream.Write(status.PrestigeLevel);

            stream.Finalize(GamePackets.MsgStauts);
            return stream;
        }
        public static unsafe ServerSockets.Packet StatusCreate(this ServerSockets.Packet stream, MsgStatus status)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);//4
            stream.Write(status.UID);//8
            stream.Write(status.MaxHitpoints);
            stream.Write(status.MaxMana);
            stream.Write(status.MaxAttack);
            stream.Write(status.MinAttack);
            stream.Write(status.Defence);
            stream.Write(status.MagicAttack);
            stream.Write(status.MagicDefence);
            stream.Write(status.Dodge);
            stream.Write(status.AgilityAtack);
            stream.Write(status.Accuracy);
            stream.Write(status.PhysicalPercent);
            stream.Write(status.MagicPercent);
            stream.Write(status.MDefence);
            stream.Write(status.Damage);
            stream.Write(status.ItemBless);

            stream.Write(status.CriticalStrike);
            stream.Write(status.SkillCStrike);
            stream.Write(status.Immunity);
            stream.Write(status.Penetration);
            stream.Write(status.Block);
            stream.Write(status.Breakthrough);
            stream.Write(status.Counteraction);
            stream.Write(status.Detoxication);
            stream.Write(status.PhysicalDamageIncrease); 
            stream.Write(status.MagicDamageIncrease);
            stream.Write(status.PhysicalDamageDecrease);
            stream.Write(status.MagicDamageDecrease);
            stream.Write(status.MetalResistance);
            stream.Write(status.WoodResistance);
            stream.Write(status.WaterResistance);
            stream.Write(status.FireResistance);
            stream.Write(status.EarthResistance);

            stream.Write(status.PrestigeLevel);

            stream.Finalize(GamePackets.MsgStauts);
            return stream;
        }
    }
    public class MsgStatus
    {
        public int Stamp;
        public uint UID;
        public uint MaxHitpoints;
        public uint MaxMana;
        public uint MaxAttack;
        public uint MinAttack;
        public uint Defence;
        public uint MagicAttack;
        public uint MagicDefence;
        public uint Dodge;
        public uint AgilityAtack;
        public uint Accuracy;
        public uint PhysicalPercent;//52
        public uint MagicPercent;
        public uint Damage;
        public uint ItemBless;

        public uint CriticalStrike;//100 = 1;
        public uint SkillCStrike;//100 = 1;
        public uint Immunity;//100 = 1;
        public uint Penetration;//100 = 1;
        public uint Block;//100 = 1;
        public uint Breakthrough;
        public uint Counteraction;//10 = 1;
        public uint Detoxication;//1 =1

        public uint PhysicalDamageIncrease;
        public uint MagicDamageIncrease;
        public uint PhysicalDamageDecrease;
        public uint MagicDamageDecrease;

        public uint MetalResistance;
        public uint WoodResistance;
        public uint WaterResistance;
        public uint FireResistance;
        public uint EarthResistance;

        public uint MDefence;
        public ushort MaxVigor;
        public uint PrestigeLevel;
        [PacketAttribute(GamePackets.MsgStauts)]
        private unsafe static void MsgStautssHandler(Client.GameClient user, ServerSockets.Packet stream)
        {

            uint UID;
            stream.GetStatus(out UID);

            if (user.Player.UID == UID)
                user.Send(stream.StatusCreate(user.Status));
            else
            {
                Client.GameClient client;
                if (Database.Server.GamePoll.TryGetValue(UID, out client))
                {
                   user.Send(stream.StatusCreate(client.Status));
                }
            }
        }
    }
}
