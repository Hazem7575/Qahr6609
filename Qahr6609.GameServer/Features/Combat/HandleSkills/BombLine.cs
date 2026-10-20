using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
   public class BombLine
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
               uint Experience = 0;
               foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
               {
                   MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                   if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                   {
                       if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                       {
                          /* Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckCoords(user.Player.X, user.Player.Y
                           , attacked.X, attacked.Y, 4, user.Map);

                           attacked.X = (ushort)coord.X;
                           attacked.Y = (ushort)coord.Y;*/

                           MsgSpellAnimation.SpellObj AnimationObj;
                           Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                           Experience+=    ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);


                           /*AnimationObj.MoveX = (uint)coord.X;
                           AnimationObj.MoveY = (uint)coord.Y;*/

                           MsgSpell.Targets.Enqueue(AnimationObj);
                           
                       }
                   }
               }
               foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
               {
                   var attacked = targer as Role.Player;
                   if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                   {
                       if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                       {
                           Algoritms.InLineAlgorithm.coords coord = Algoritms.MoveCoords.CheckBombCoords(user.Player.X, user.Player.Y
                      , attacked.X, attacked.Y, 4, user.Map);
                           if (coord.X == 0) break;

                           if (!CheckAttack.CheckFloors.CheckGuildWar(user, coord.X, coord.Y))
                           {
                               continue;
                           }
                           
                               user.Map.View.MoveTo<Role.IMapObj>(attacked, coord.X, coord.Y);
                               attacked.X = (ushort)coord.X;
                               attacked.Y = (ushort)coord.Y;

                           MsgSpellAnimation.SpellObj AnimationObj;
                           Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                           ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                         
                           AnimationObj.MoveX = (uint)coord.X;
                           AnimationObj.MoveY = (uint)coord.Y;
                           attacked.View.Role(false,null);

                           MsgSpell.Targets.Enqueue(AnimationObj);
                       }
                   }

               }
               foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
               {
                   var attacked = targer as Role.SobNpc;
                   if (targer.UID == MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].UID || targer.UID == MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].UID)
                   {
                       continue;
                   }
                   if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                   {
                       if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                       {
                           /*if (targer.UID != MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].UID && targer.UID != MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].UID)
                           {
                            }*/
                           MsgSpellAnimation.SpellObj AnimationObj;
                           Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                           Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                           MsgSpell.Targets.Enqueue(AnimationObj);
                       }
                   }
               }
               Updates.IncreaseExperience.Up(stream,user, Experience);
               Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
               MsgSpell.SetStream(stream);
               MsgSpell.Send(user);

           }
       }
  
    }
}
