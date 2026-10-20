using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
   public class DetachStatus
    {
       public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
       {
           Database.MagicType.Magic DBSpell;
           MsgSpell ClientSpell;
           if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
           {
               switch (ClientSpell.ID)
               {
                   case (ushort)Role.Flags.SpellID.ArcherBane:
                       {
                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);


                           Role.IMapObj target;
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                           {
                               Role.Player attacked = target as Role.Player;
                               if (attacked.ContainFlag(MsgUpdate.Flags.Fly))
                               {
                                   if (Calculate.Base.Rate(40))
                                   {
                                       attacked.RemoveFlag(MsgUpdate.Flags.Fly);
                                       MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 30, MsgAttackPacket.AttackEffect.None));
                                   }
                                   else
                                   {
                                       var clientobj = new MsgSpellAnimation.SpellObj(attacked.UID, MsgSpell.SpellID, MsgAttackPacket.AttackEffect.None);
                                       clientobj.Hit = 0;
                                       MsgSpell.Targets.Enqueue(clientobj);
                                   }
                               }
                           }
                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);

                           Updates.UpdateSpell.CheckUpdate(stream,user,Attack, 250, DBSpells);

                           break;
                       }
                   case (ushort)Role.Flags.SpellID.Revive:
                       {
                           if (user.IsWatching())
                           {
                               user.SendSysMesage("This spell not work on this map..");
                               
                               break;
                           }

                           // Staff accounts (by server flag, not by name) cannot use the Revive skill.
                           if (user.ProjectManager || user.GameMaster)
                               break;

                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);

                           user.Player.RemoveFlag(MsgUpdate.Flags.XPList);

                           Role.IMapObj target;
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                           {
                               Role.Player attacked = target as Role.Player;
                               MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                               attacked.Revive(stream);
                           }


                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);

                           Updates.UpdateSpell.CheckUpdate(stream,user,Attack, DBSpell.Duration, DBSpells);


                           if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                           {

                               MsgGameItem BackSword;
                               if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                               {
                                   if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                                   {
                                       if (user.Player.TaoistPower < 10)
                                       {
                                           user.Player.TaoistPower += 1;
                                           user.Player.UpdateTaoPower(stream);
                                       }

                                       byte change = 10;

                                       MsgGameItem hossus;
                                       if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                                       {
                                           if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                                           {
                                               var dbItem = Database.Server.ItemsBase[hossus.ITEM_ID];
                                               change += (byte)(dbItem.Level / 3);
                                           }
                                       }

                                       if (AttackHandler.Calculate.Base.Rate(change))
                                       {
                                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                           {
                                               Role.Player attacked = target as Role.Player;

                                               attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)80, true);
                                               attacked.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)120, true);
                                               attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)60, true);


                                               Attack.SpellID = (ushort)Role.Flags.SpellID.BlessingTouch;
                                               var Spell = Database.Server.Magic[Attack.SpellID];

                                               Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)(change * 100), Spell);

                                           }
                                       }
                                   }
                               }
                           }

                           break;
                       }
                   case (ushort)Role.Flags.SpellID.Pray:
                       {
                           if (user.IsWatching())
                           {
                               user.SendSysMesage("This spell not work on this map..");
                              
                               break;
                           }
                        

                           MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);

                          

                           Role.IMapObj target;
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                           {
                               Role.Player attacked = target as Role.Player;
                               MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                               attacked.Revive(stream);
                           }

                           MsgSpell.SetStream(stream);
                           MsgSpell.Send(user);

                           Updates.UpdateSpell.CheckUpdate(stream,user,Attack, DBSpell.Duration, DBSpells);

                           

                           if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlessingTouch))
                           {

                               MsgGameItem BackSword;
                               if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                               {
                                   if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                                   {
                                       if (user.Player.TaoistPower < 10)
                                       {
                                           user.Player.TaoistPower += 1;
                                           user.Player.UpdateTaoPower(stream);
                                       }

                                       byte change = 10;

                                       MsgGameItem hossus;
                                       if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                                       {
                                           if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                                           {
                                               var dbItem = Database.Server.ItemsBase[hossus.ITEM_ID];
                                               change += (byte)(dbItem.Level / 3);
                                           }
                                       }

                                       if (AttackHandler.Calculate.Base.Rate(change))
                                       {
                                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                           {
                                               Role.Player attacked = target as Role.Player;

                                               attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)80, true);
                                               attacked.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)120, true);
                                               attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)60, true);


                                               Attack.SpellID = (ushort)Role.Flags.SpellID.BlessingTouch;
                                               var Spell = Database.Server.Magic[Attack.SpellID];

                                               Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)(change * 100), Spell);

                                           }
                                       }
                                   }
                               }
                           }

                           break;
                       }
               }
           }
       }
    }
}
