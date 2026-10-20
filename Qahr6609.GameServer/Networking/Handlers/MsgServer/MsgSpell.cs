using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static class SpellPacket
    {
        public static unsafe void GetSpell(this ServerSockets.Packet stream, out MsgSpell spell)
        {
            spell = new MsgSpell();

            spell.PacketStamp = stream.ReadInt32();
            spell.Experience = stream.ReadInt32();
            spell.ID = stream.ReadUInt16();
            spell.UnKnow = stream.ReadUInt16();
            spell.Level = stream.ReadUInt16();
            spell.Type = stream.ReadUInt16();
            spell.UseSoul = (MsgSpell.UseSpellSoulTyp)stream.ReadUInt32();
            spell.Soul = stream.ReadUInt8();
            spell.HashLevelSoul = stream.ReadInt32();
        }
        public static unsafe ServerSockets.Packet SpellCreate(this ServerSockets.Packet stream, MsgSpell spell)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);//4
            stream.Write(spell.Experience);//8
            stream.Write(spell.ID);//12
            stream.Write(spell.UnKnow);//14
            stream.Write(spell.Level);//16
            stream.Write(spell.Type);//18
            stream.Write((uint)spell.UseSoul);//20
            stream.Write(spell.Soul);
            stream.Write( spell.HashLevelSoul);

            stream.Finalize(GamePackets.Spell);
            return stream;
        }
    }
    public unsafe class MsgSpell
    {
        public enum UseSpellSoulTyp : uint
        {
            None = 0,
            One = 9,
            Two = 12,
            Three = 16,
            Four = 24
        }
        public Extensions.Time32 ColdTime = new Extensions.Time32();
        public int GetColdTime
        {
            get
            {
                return (ColdTime.AllMilliseconds - Extensions.Time32.Now.AllMilliseconds);
            }
        }
        public bool IsSpellWithColdTime = false;

        public ushort Size;
        public ushort PacketType;
        public int PacketStamp;
        public int Experience;
        public ushort ID;
        public ushort UnKnow;
        public ushort Level;
        public ushort Type;
        public UseSpellSoulTyp UseSoul;
        public byte Soul;
        public int HashLevelSoul;
        public DateTime StampSpell = new DateTime();
        public byte SoulLevel
        {
            get
            {
                return _levelHu;
            }
            set
            {
                _levelHu = value;
                if (value > 0) HashLevelSoul |= (int)(1UL << 1);
                if (value > 1) HashLevelSoul |= (int)(1UL << 4);
                if (value > 2) HashLevelSoul |= (int)(1UL << 8);
                if (value > 3) HashLevelSoul |= (int)(1UL << 16);
            }
        }

        public byte PreviousLevel;

        private byte _levelHu;

     

        public byte UseSpellSoul
        {
            get
            {
                if (UseSoul == UseSpellSoulTyp.One)
                    return 1;
                if (UseSoul == UseSpellSoulTyp.Two)
                    return 2;
                if (UseSoul == UseSpellSoulTyp.Three)
                    return 3;
                if (UseSoul == UseSpellSoulTyp.Four)
                    return 4;
                return 0;
            }
            set
            {
                if (value == 1)
                    UseSoul = UseSpellSoulTyp.One;
                else if (value == 2)
                    UseSoul = UseSpellSoulTyp.Two;
                else if (value == 3)
                    UseSoul = UseSpellSoulTyp.Three;
                else if (value == 4)
                    UseSoul = UseSpellSoulTyp.Four;
                else
                    UseSoul = UseSpellSoulTyp.None;
            }
        }

     
        [PacketAttribute(Game.GamePackets.Spell)]
        public unsafe static void HandlerSpell(Client.GameClient client, ServerSockets.Packet stream)
        {

            MsgSpell spell;

            stream.GetSpell(out spell);

            MsgSpell ClientSpell;
            if (client.MySpells.ClientSpells.TryGetValue(spell.ID, out ClientSpell))
            {
                if (ClientSpell.SoulLevel >= spell.UseSpellSoul)
                {
                    ClientSpell.UseSoul = spell.UseSoul;
                    client.MySpells.ClientSpells[spell.ID] = ClientSpell;

                    spell.Type = 3;

                    client.Send(stream.SpellCreate(spell));
                }
            }
        }
    }
}
