using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
    public class Sering
    {

        public static void Proces(Client.GameClient user, Role.Player attacked, ServerSockets.Packet stream)
        {
            if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SearingTouch))
            {

                MsgGameItem BackSword;
                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                {
                    if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                    {
                       

                        byte change = 10;
                        uint Experience = 0;
                        MsgGameItem hossus;
                        if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                        {
                            if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                            {
                                var dbItem = Database.Server.ItemsBase[hossus.ITEM_ID];
                                change += (byte)(dbItem.Level / 4);
                            }
                        }

                        if (AttackHandler.Calculate.Base.Rate(Math.Min(100, change + 40)))
                        {
                          
                            if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                            {
                                if (user.Player.TaoistPower < 10)
                                {
                                    if (Database.ItemType.IsTaoistEpicWeapon(user.Equipment.RightWeapon))
                                    {
                                        user.Player.TaoistPower += 1;
                                        user.Player.UpdateTaoPower(stream);
                                    }
                                }

                                InteractQuery Attack = new InteractQuery();
                           
                                Attack.SpellID = (ushort)Role.Flags.SpellID.SearingTouch;
                                var DBSpells = Database.Server.Magic[(ushort)Role.Flags.SpellID.SearingTouch];
                                MsgServer.MsgSpell ClientSpell = user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SearingTouch];
                                var DBSpell = DBSpells[ClientSpell.Level];
                           
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(attacked.UID
           , 0, attacked.X , attacked.Y, ClientSpell.ID
           , ClientSpell.Level, ClientSpell.UseSpellSoul, 1);


                                foreach (Role.IMapObj atarget in attacked.View.Roles(Role.MapObjectType.Monster))
                                {

                                    MsgMonster.MonsterRole aattacked = atarget as MsgMonster.MonsterRole;
                                    if (CheckAttack.CanAttackMonster.Verified(user, aattacked, DBSpell))
                                    {
                                        if (aattacked.UID != user.Player.UID)
                                        {
                                            if (Calculate.Base.GetDistance(attacked.X, attacked.Y, aattacked.X, aattacked.Y) < 6)
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                Calculate.Magic.OnMonster(user.Player, aattacked, DBSpell, out AnimationObj);
                                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                                if (AnimationObj.Damage > aattacked.HitPoints)
                                                    AnimationObj.Damage = (uint)(aattacked.HitPoints - 1);
                                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, aattacked);
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                            }
                                        }
                                    }
                                }
                                foreach (Role.IMapObj atarger in attacked.View.Roles(Role.MapObjectType.Player))
                                {
                                    var aattacked = atarger as Role.Player; 
                                    if (aattacked.UID != user.Player.UID)
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, aattacked, DBSpell))
                                        {
                                            if (Calculate.Base.GetDistance(attacked.X, attacked.Y, aattacked.X, aattacked.Y) < 6)
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                Calculate.Magic.OnPlayer(user.Player, aattacked, DBSpell, out AnimationObj);
                                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                                if (AnimationObj.Damage > aattacked.HitPoints)
                                                    AnimationObj.Damage = (uint)(aattacked.HitPoints - 1);
                                                ReceiveAttack.Player.Execute(AnimationObj, user, aattacked);
                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                            }
                                        }
                                    }
                                }

                                Updates.IncreaseExperience.Up(stream, user, Experience);
                                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)(change * 100), DBSpells);
                                MsgSpell.SetStream(stream);
                                MsgSpell.Send(user);


                                attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran04);
                            }
                            // Stage change: the old stage flag is dropped without its own status broadcast; the Add of the
                            // next stage sends one status update with the final flags (same client result, one packet less).
                            else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran03))
                            {
                                attacked.BitVector.TryRemove((int)MsgUpdate.Flags.lianhuaran03);// status sent once by the Add below
                                attacked.AddFlag(MsgUpdate.Flags.lianhuaran04, 20, true);
                            }
                            else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran02))
                            {
                                attacked.BitVector.TryRemove((int)MsgUpdate.Flags.lianhuaran02);// status sent once by the Add below
                                attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran03, 20, true);
                            }
                            else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran01))
                            {
                                attacked.BitVector.TryRemove((int)MsgUpdate.Flags.lianhuaran01);// status sent once by the Add below
                                attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran02, 20, true);
                            }
                            else
                                attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran01, 20, true);
                        }
                    }

                }
            }
        }
    }
}
