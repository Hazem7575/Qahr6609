using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
    public class Spell
    {
        public ConcurrentDictionary<ushort, Game.MsgServer.MsgSpell> ClientSpells = new ConcurrentDictionary<ushort, Game.MsgServer.MsgSpell>();
       
        private Client.GameClient Owner;

        public Spell(Client.GameClient _own)
        {
            Owner = _own;
        }
        public bool CheckSpell(ushort ID, byte Level)
        {
            Game.MsgServer.MsgSpell spell;
            if (ClientSpells.TryGetValue(ID, out spell))
            {
                return spell.Level >= Level;
            }
            return false;
        }
        public void ClearSpells(List<ushort> StaticSpells,ServerSockets.Packet stream)
        {
            foreach (var spell in ClientSpells.Values)
            {
                if (!StaticSpells.Contains(spell.ID))
                    Remove(spell.ID,stream);
                else
                    RebornSpell(stream, spell.ID);
            }
        }
        public unsafe void Remove(ushort ID , ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgSpell my_spell;
            if (ClientSpells.TryRemove(ID, out my_spell))
            {
                ActionQuery action = new ActionQuery()
                {
                    Type = ActionType.RemoveSpell,
                    ObjId = Owner.Player.UID,
                    dwParam = ID
                };
                Owner.Send(stream.ActionCreate(&action));
            }
        }
        public unsafe void Add(ServerSockets.Packet stream, ushort ID, ushort level = 0,byte levelsoul = 0
            , byte previouslevel = 0, int Experience = 0, bool ClearExp =false)
        {
            if (ID == 1105 || ID >= 4000 && ID <= 4070)
                return;
            Game.MsgServer.MsgSpell my_spell = new MsgSpell();
            my_spell.ID = ID;
            my_spell.Level = level;
            my_spell.Experience = Experience;
            my_spell.SoulLevel = levelsoul;
            my_spell.PreviousLevel = previouslevel;
            my_spell.Experience = Experience;
            if (ClearExp)
                my_spell.Experience = 0;
            if (ClientSpells.ContainsKey(ID))
            {
                ClientSpells[ID] = my_spell;
            }
            ClientSpells.TryAdd(my_spell.ID, my_spell);

            Owner.Send(stream.SpellCreate(my_spell));
        }
        public bool RebornSpell(ServerSockets.Packet stream, ushort ID)
        {
            Game.MsgServer.MsgSpell spell;
            if (ClientSpells.TryRemove(ID, out spell))
            {
                Add(stream,ID, 0, spell.SoulLevel, (byte)spell.Level, 0);
                return true;
            }
            return false;
        }
        public void Update(ServerSockets.Packet stream,Game.MsgServer.MsgSpell spell)
        {
            Add(stream, spell.ID, spell.Level, spell.SoulLevel, spell.PreviousLevel, spell.Experience);
        }

     
        public unsafe void SendAll(ServerSockets.Packet stream)
        {
            foreach (Game.MsgServer.MsgSpell spell in ClientSpells.Values)
            {
                Owner.Send(stream.SpellCreate(spell));
            }
        }
    }
}
