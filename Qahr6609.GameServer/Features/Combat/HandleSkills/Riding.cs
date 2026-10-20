using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
   public class Riding
    {
       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
           {
               if (user.Player.ContainFlag(MsgUpdate.Flags.Fly) || user.Player.OnTransform || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(user.Player.Map))
               {
                   user.SendSysMesage("You can`t use this skill right now !");
                 
                   return;
               }
               if (user.Player.Map == 10166 || user.Player.Map == 1005 || user.Player.Map == 700 || user.Player.Map == 1860 || user.Player.Map == 1858 || user.Player.Map == 1004 || MsgTournaments.MsgSchedules.CurrentTournament.InTournament(user))
               {
                   user.SendSysMesage("You can't use this skill on this map.");
                  
                   return;
               }

               MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

               if (!user.Player.ContainFlag(MsgUpdate.Flags.Ride))
               {
                   user.Player.AddFlag(MsgUpdate.Flags.Ride, Role.StatusFlagsBigVector32.PermanentFlag, true, 1);

                   user.Vigor = user.Status.MaxVigor;

                   user.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, user.Vigor));

               }
               else
                   user.Player.RemoveFlag(MsgUpdate.Flags.Ride);
               MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
               MsgSpell.SetStream(stream);
               MsgSpell.Send(user);

           }
       }
    }
}
