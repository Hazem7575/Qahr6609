using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
   public class ShieldBlock
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

               user.Player.ShieldBlockDamage = (uint)DBSpell.Damage;
               user.Player.AddSpellFlag(MsgUpdate.Flags.ShieldBlock, (int)DBSpell.Duration, true);
               user.Player.SendUpdate(stream,Game.MsgServer.MsgUpdate.Flags.ShieldBlock, DBSpell.Duration
       , (uint)DBSpell.Damage, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);

               MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
               MsgSpell.SetStream(stream);
               MsgSpell.Send(user);
           }
       }
    }
}
