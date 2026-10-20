using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
   public class Oblivion
    {
       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
           {
               MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);

               // Activation consumes the XP list; without it the skill cannot start.
               if (!user.Player.RemoveFlag(MsgUpdate.Flags.XPList))
                   return;
               user.Player.OpenXpSkill(MsgUpdate.Flags.Oblivion, 30);
               user.Player.OblivionCounter = user.Player.KillCounter;
               MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
               MsgSpell.SetStream(stream);
               MsgSpell.Send(user);
           }
       }
    }
}
