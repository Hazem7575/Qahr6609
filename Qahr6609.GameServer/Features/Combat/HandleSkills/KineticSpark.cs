using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgFloorItem;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
   public class KineticSpark
    {
       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
           {
               switch (ClientSpell.ID)
               {
                   case (ushort)Role.Flags.SpellID.KineticSpark:
                       {
                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                          , 0, Attack.X, Attack.Y, ClientSpell.ID
                                          , ClientSpell.Level, ClientSpell.UseSpellSoul);

                           if (user.Player.ContainFlag(MsgUpdate.Flags.KineticSpark))
                               user.Player.RemoveFlag(MsgUpdate.Flags.KineticSpark);
                           else
                               user.Player.AddSpellFlag(MsgUpdate.Flags.KineticSpark, Role.StatusFlagsBigVector32.PermanentFlag, true);

                           MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                           //Updates.UpdateSpell.CheckUpdate(stream,user,Attack, 1000, DBSpells);

                           MsgSpell.SetStream(stream); MsgSpell.Send(user);

                           break;
                       }
                   case (ushort)Role.Flags.SpellID.ShadowofChaser:
                       {
                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                          , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                          , ClientSpell.Level, ClientSpell.UseSpellSoul);

                           if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                               user.Player.RemoveFlag(MsgUpdate.Flags.ShadowofChaser);
                           else
                               user.Player.AddSpellFlag(MsgUpdate.Flags.ShadowofChaser, Role.StatusFlagsBigVector32.PermanentFlag, true);

                           MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                           Updates.UpdateSpell.CheckUpdate(stream,user,Attack, 1000, DBSpells);

                           MsgSpell.SetStream(stream); MsgSpell.Send(user);

                           break;
                       }
               }
           }
       }
       public unsafe static void AttackSpell(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
           {
               switch (ClientSpell.ID)
               {
                   case (ushort)Role.Flags.SpellID.ShadowofChaser:
                       {
                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);



                           Role.IMapObj _target;
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                               || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                               || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                           {
                               if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                   user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                               var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.ShadowofChaser, (ushort)_target.X, (ushort)_target.Y, 14, DBSpell, 500);
                               user.Player.FloorSpells[ClientSpell.ID].AddItem(FloorItem);

                               MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 1, MoveX = user.Player.X, Hit = 1, MoveY = user.Player.Y, UID = FloorItem.FloorPacket.m_UID });
                               user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);

                           }

                           Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 10000, DBSpells);
                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);
                           break;
                       }
                   case (ushort)Role.Flags.SpellID.KineticSpark:
                       {

                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);


                          
                           uint Experience = 0;
                           Role.IMapObj target;
                           uint count = 0;
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                           {
                               MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                               MsgSpell.X = attacked.X;
                               MsgSpell.Y = attacked.Y;
                               if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                               {
                                   MsgSpellAnimation.SpellObj AnimationObj;
                                   Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                   AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                           
                                   Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                   MsgSpell.Targets.Enqueue(AnimationObj);

                               }

                               foreach (var attobj in user.Player.View.Roles(Role.MapObjectType.Monster))
                               {
                                   if (attobj.UID == Attack.OpponentUID)
                                       continue;
                                   attacked = attobj as MsgMonster.MonsterRole;
                                   if (Calculate.Base.GetDistance(MsgSpell.X, MsgSpell.Y, attacked.X, attacked.Y) < DBSpell.Range / 2)
                                   {
                                       if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                       {
                                           count++;
                                           if (count == 5)
                                               return;
                                           MsgSpellAnimation.SpellObj AnimationObj;
                                           Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                           AnimationObj.Damage = (uint)Calculate.Base.MulDiv((int)AnimationObj.Damage, (int)(100 - (int)(count * 20)), 100);
                                           ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                           MsgSpell.Targets.Enqueue(AnimationObj); ;
                                       }
                                   }
                               }

                           }
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                           {
                               var attacked = target as Role.Player;
                               MsgSpell.X = attacked.X;
                               MsgSpell.Y = attacked.Y;
                               if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                               {
                                   count++;
                                   if (count == 5)
                                       return;
                                   MsgSpellAnimation.SpellObj AnimationObj;
                                   Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                   AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                   ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                   MsgSpell.Targets.Enqueue(AnimationObj);
                               }

                               foreach (var attobj in user.Player.View.Roles(Role.MapObjectType.Player))
                               {
                                   if (attobj.UID == Attack.OpponentUID)
                                       continue;
                                   attacked = attobj as Role.Player;
                                   if (Calculate.Base.GetDistance(MsgSpell.X, MsgSpell.Y, attacked.X, attacked.Y) < DBSpell.Range / 2)
                                   {
                                       if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                       {
                                           count++;
                                           if (count == 5)
                                               return;
                                           MsgSpellAnimation.SpellObj AnimationObj;
                                           Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul); 
                                           AnimationObj.Damage = (uint)Calculate.Base.MulDiv((int)AnimationObj.Damage, (int)(100 - (int)(count * 20)), 100);
                                           ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                           MsgSpell.Targets.Enqueue(AnimationObj);
                                       }
                                   }
                               }
                           }
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                           {
                               var attacked = target as Role.SobNpc;
                               MsgSpell.X = attacked.X;
                               MsgSpell.Y = attacked.Y;
                               if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                               {
                                   count++;
                                   if (count == 5)
                                       return;
                                   MsgSpellAnimation.SpellObj AnimationObj;
                                   Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                   AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                   Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);

                                   MsgSpell.Targets.Enqueue(AnimationObj);
                               }
                                   foreach (var attobj in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                                   {
                                       if (attobj.UID == Attack.OpponentUID)
                                           continue;
                                       attacked = attobj as Role.SobNpc;
                                       if (Calculate.Base.GetDistance(MsgSpell.X, MsgSpell.Y, attacked.X, attacked.Y) < DBSpell.Range / 2)
                                       {
                                           if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                           {
                                               count++;
                                               if (count == 5)
                                                   return;
                                               MsgSpellAnimation.SpellObj AnimationObj;
                                               Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                               AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                               AnimationObj.Damage = (uint)Calculate.Base.MulDiv((int)AnimationObj.Damage, (int)(100 - (int)(count * 20)), 100);
                                               Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);

                                               MsgSpell.Targets.Enqueue(AnimationObj);
                                           }
                                       }
                                   
                               }
                           }
                           Updates.IncreaseExperience.Up(stream, user, Experience);
                           Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                           if (MsgSpell.Targets.Count != 0)
                           {
                               MsgSpell.SetStream(stream);
                               MsgSpell.Send(user);
                           }

                           break;
                       }
               }
           }
       
       }
    }
}
