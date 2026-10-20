using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
   public class MortalDrag
    {
       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
           {
               switch (ClientSpell.ID)
               {
                   case (ushort)Role.Flags.SpellID.MortalDrag:
                       {
                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                 , 0, Attack.X, Attack.Y, ClientSpell.ID
                                 , ClientSpell.Level, ClientSpell.UseSpellSoul);

                           uint Experience = 0;
              
                           Role.IMapObj target;
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                           {
                               MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                               if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                               {
                                   if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                   {
                                       if (attacked.UpdateMapCoords(user.Player.X, user.Player.Y, user.Map))
                                       {

                                           user.Map.View.MoveTo<Role.IMapObj>(attacked, user.Player.X, user.Player.Y);
                                           attacked.X = user.Player.X;
                                           attacked.Y = user.Player.Y;
                                           attacked.UpdateMonsterView(user.Player.View,stream);

                                           MsgSpellAnimation.SpellObj AnimationObj;
                                           Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                           Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                           MsgSpell.Targets.Enqueue(AnimationObj);
                                       }
                                   }
                               }
                           }
                           else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                           {
                               var attacked = target as Role.Player;
                               if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                               {

                                   if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                   {

                                       attacked.Owner.Map.View.MoveTo<Role.IMapObj>(attacked, user.Player.X, user.Player.Y);
                                       attacked.X = user.Player.X;
                                       attacked.Y = user.Player.Y;
                                       attacked.View.Role(false,null);

                                       MsgSpellAnimation.SpellObj AnimationObj;
                                       Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                       AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                       ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                       MsgSpell.Targets.Enqueue(AnimationObj);
                                   }
                               }

                           }
                           Updates.IncreaseExperience.Up(stream,user, Experience);
                           Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);
                           break;
                       }
               }
           }
       }
    }
}
