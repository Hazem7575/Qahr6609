using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.Calculate
{
 public   class BackDmg
    {
     public unsafe static bool Calculate(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, uint Damage , out MsgSpellAnimation.SpellObj SpellObj)
     {
        
         if (player.Alive == false)
         {
             SpellObj = default(MsgSpellAnimation.SpellObj);
             return false;
         }
         //if (Base.Rate(55))
         {
             if (target.ContainFlag(MsgUpdate.Flags.RevengeTail))
             {
                 if (target.RevengeTailChange > 0)
                 {
                     MsgSpell ClientSpell;
                     if (target.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.RevengeTail, out ClientSpell))
                     {
                         Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                         if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.RevengeTail, out DBSpells))
                         {
                             if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                             {
                                 if (Damage < (uint)DBSpell.Damage)
                                 {
                                     MsgSpellAnimation MsgSpell = new MsgSpellAnimation(target.UID
                                  , 0, target.X, target.Y, ClientSpell.ID
                                  , ClientSpell.Level, ClientSpell.UseSpellSoul);

                                     MsgSpell.bomb = 1;
                                     if (CheckAttack.CanAttackPlayer.Verified(player.Owner, target, DBSpell))
                                     {
                                         MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj()
                                         {
                                             UID = player.UID,
                                             Hit = 1
                                         };
                                         AnimationObj.Damage = (uint)DBSpell.Damage;
                                         ReceiveAttack.Player.Execute(AnimationObj, target.Owner, player);
                                         MsgSpell.Targets.Enqueue(AnimationObj);
                                     }
                                     using (var rec = new ServerSockets.RecycledPacket())
                                     {
                                         var stream = rec.GetStream();


                                         MsgSpell.SetStream(stream);
                                         MsgSpell.Send(target.Owner);


                                     }

                                     target.RevengeTailChange -= 1;

                                     if (target.RevengeTailChange == 0)
                                         target.RemoveFlag(MsgUpdate.Flags.RevengeTail);
                                 }
                             }

                         }
                     }
                 }
             }

             if (target.ContainFlag(MsgUpdate.Flags.Backfire))
             {
              
                 using (var rec = new ServerSockets.RecycledPacket())
                 {
                     var stream = rec.GetStream();
                     MsgSpell ClientSpell;
                     if (target.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Backfire, out ClientSpell))
                     {
                         Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                         if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.Backfire, out DBSpells))
                         {
                             if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                             {
                                 SpellObj = new MsgSpellAnimation.SpellObj();
                                 SpellObj.Damage = Damage;
                                 if (target.HitPoints > Damage)//if the target will be alive
                                 {
                                     target.RemoveFlag(MsgUpdate.Flags.Backfire);

                                     MsgSpellAnimation.SpellObj DmgObj = new MsgSpellAnimation.SpellObj();
                                     DmgObj.Damage = Math.Min(50000, (uint)Base.MulDiv((int)(target.HitPoints - Damage), (int)DBSpell.Damage, 100));

                                     //update spell
                                     if (ClientSpell.Level < DBSpells.Count - 1)
                                     {
                                         ClientSpell.Experience += (int)(DmgObj.Damage * Program.ServerConfig.ExpRateSpell);
                                         if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                         {
                                             ClientSpell.Level++;
                                             ClientSpell.Experience = 0;
                                         }
                                         target.Send(stream.SpellCreate(ClientSpell));
                                         target.Owner.MySpells.ClientSpells[ClientSpell.ID] = ClientSpell;
                                     }


                                     InteractQuery action = new InteractQuery()
                                     {
                                         Damage = (int)DmgObj.Damage,
                                         X = target.X,
                                         Y = target.Y,
                                         OpponentUID = player.UID,
                                         UID = target.UID,
                                         Effect = DmgObj.Effect,
                                         AtkType = MsgAttackPacket.AttackID.BackFire

                                     };

                                     target.View.SendView(stream.InteractionCreate(&action), true);
                                     action = new InteractQuery()
                                    {
                                        Damage = (int)Damage,
                                        X = player.X,
                                        Y = player.Y,
                                        UID = player.UID,
                                        OpponentUID = target.UID,
                                        Effect = DmgObj.Effect,
                                        AtkType = MsgAttackPacket.AttackID.Physical

                                    };

                                     player.View.SendView(stream.InteractionCreate(&action), true);

                                     ReceiveAttack.Player.Execute(DmgObj, target.Owner, player);


                                     byte MaxStamina = (byte)(target.HeavenBlessing > 0 ? 150 : 100);
                                     target.Stamina = (ushort)Math.Min((int)target.Stamina + 30, MaxStamina);

                                     target.SendUpdate(stream, target.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);


                                     return true;
                                 }
                             }
                         }
                     }
                 }
             }
         }
         if (Base.Rate(15))
         {

            
             if (target.ActivateCounterKill)
             {
                 using (var rec = new ServerSockets.RecycledPacket())
                 {
                     var stream = rec.GetStream();
                     MsgSpell ClientSpell;
                     if (target.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.CounterKill, out ClientSpell))
                     {
                         Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                         if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.CounterKill, out DBSpells))
                         {
                             if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                             {
                                 SpellObj = new MsgSpellAnimation.SpellObj();
                                 SpellObj.Damage = 0;

                                 MsgSpellAnimation.SpellObj DmgObj = new MsgSpellAnimation.SpellObj();
                                 Physical.OnPlayer(target, player, DBSpell, out DmgObj, true);
                                 DmgObj.Damage /= 2;

                                 //update spell
                                 if (ClientSpell.Level < DBSpells.Count - 1)
                                 {
                                     ClientSpell.Experience += (int)(DmgObj.Damage * Program.ServerConfig.ExpRateSpell);
                                     if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                     {
                                         ClientSpell.Level++;
                                         ClientSpell.Experience = 0;
                                     }
                                     target.Send(stream.SpellCreate(ClientSpell));
                                     target.Owner.MySpells.ClientSpells[ClientSpell.ID] = ClientSpell;
                                 }


                                 InteractQuery action = new InteractQuery()
                                 {
                                     ResponseDamage = DmgObj.Damage,
                                     X = player.X,
                                     Y = player.Y,
                                     OpponentUID = player.UID,
                                     UID = target.UID,
                                     Effect = DmgObj.Effect,
                                     AtkType = MsgAttackPacket.AttackID.Scapegoat
                                 };

                                 target.View.SendView(stream.InteractionCreate(&action), true);



                                 ReceiveAttack.Player.Execute(DmgObj, target.Owner, player);

                                 return true;
                             }
                         }
                     }
                 }
             }
             if (target.ContainReflect)
             {
                 using (var rec = new ServerSockets.RecycledPacket())
                 {
                     var stream = rec.GetStream();
                     SpellObj = new MsgSpellAnimation.SpellObj();
                     SpellObj.Damage = 0;
                    SpellObj.UID = target.UID;

                     MsgSpellAnimation.SpellObj DmgObj = new MsgSpellAnimation.SpellObj();
                     //Physical.OnPlayer(player, target, DBSpell, out DmgObj, true);
                     DmgObj.Damage = Damage / 10;
                     DmgObj.UID = player.UID;


                     InteractQuery action = new InteractQuery()
                     {
                         ResponseDamage = DmgObj.Damage,
                         Damage = (int)DmgObj.Damage,
                         AtkType = MsgAttackPacket.AttackID.Reflect,
                         X = player.X,
                         Y = player.Y,
                         OpponentUID = player.UID,
                         UID = target.UID,
                         Effect = DmgObj.Effect
                     };

                     target.View.SendView(stream.InteractionCreate(&action), true);




                     ReceiveAttack.Player.Execute(DmgObj, target.Owner, player);
                 }
                 return true;
             }
         }
         SpellObj = default(MsgSpellAnimation.SpellObj);
         return false;
     }

    }
}
