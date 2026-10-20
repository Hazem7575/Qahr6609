using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
    public class AttachStatus
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.RevengeTail:
                        {
                            user.Send(stream.InteractionCreate(&Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.RevengeTail))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.RevengeTail);
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.RevengeTail, 10, true, 20);


                                user.Player.RevengeTailChange = 5;

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ChillingSnow:
                        {
                            user.Send(stream.InteractionCreate(&Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.ChillingSnow))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.ChillingSnow);
                                break;
                            }
                            user.Player.RemoveFlag(MsgUpdate.Flags.HealingSnow);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.ChillingSnow, Role.StatusFlagsBigVector32.PermanentFlag, true, 5);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 500, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.HealingSnow:
                        {
                            user.Send(stream.InteractionCreate(&Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.HealingSnow))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.HealingSnow);
                                break;
                            }
                            user.Player.RemoveFlag(MsgUpdate.Flags.ChillingSnow);

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.HealingSnow, Role.StatusFlagsBigVector32.PermanentFlag, true, 5);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FreezingPelter:
                        {

                            user.Send(stream.InteractionCreate(&Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.FreezingPelter))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.FreezingPelter);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.FreezingPelter, Role.StatusFlagsBigVector32.PermanentFlag, true, 5);
                                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SpiritFocus:
                    case (ushort)Role.Flags.SpellID.Intensify:
                        {
                            Attack.SpellID = ClientSpell.ID;
                            Attack.SpellLevel = ClientSpell.Level;
                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);
                            user.Player.IntensifyStamp = Extensions.Time32.Now;
                            user.Player.InUseIntensify = true;
                            user.Player.IntensifyDamage = (int)DBSpell.Damage;

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Backfire:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);



                            user.Player.AddFlag(MsgUpdate.Flags.Backfire, 10, false);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.PathOfShadow:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (!user.Player.RemoveFlag(MsgUpdate.Flags.PathOfShadow))
                                user.Player.AddFlag(MsgUpdate.Flags.PathOfShadow, Role.StatusFlagsBigVector32.PermanentFlag, false);
                            else
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.KineticSpark);
                            }
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }

                    case (ushort)Role.Flags.SpellID.DefensiveStance:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (!user.Player.RemoveFlag(MsgUpdate.Flags.DefensiveStance))
                            {
                                user.Player.AddFlag(MsgUpdate.Flags.DefensiveStance, (int)DBSpell.Duration, false);
                                user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.DefensiveStance, (uint)DBSpell.Duration
                                  , (uint)DBSpell.Damage, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.DefensiveStance, true);
                            }
                            else
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.DefensiveStance);
                            }


                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.PoisonStar:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                Role.Player attacked = target as Role.Player;
                                if (!CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    return;
                                int rate = attacked.BattlePower - user.Player.BattlePower;
                                if ((attacked.BattlePower - user.Player.BattlePower) >= 10)
                                {
                                    user.SendSysMesage("Your BattlePower is lower than your opponent get a better BattlePower to be able to use this skill on him .", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    break;
                                }
                                switch (rate)
                                {
                                    case 0: rate = 100; break;
                                    case 1: rate = 80; break;
                                    case 2: rate = 70; break;
                                    case 3: rate = 60; break;
                                    case 4: rate = 50; break;
                                    case 5: rate = 40; break;
                                    case 6: rate = 30; break;
                                    case 7: rate = 20; break;
                                    case 8: rate = 10; break;
                                    case 9: rate = 5; break;

                                }
                                if (user.Player.BattlePower >= attacked.BattlePower) rate = 100;
                                if (Calculate.Base.Rate(rate))
                                {
                                    attacked.AddSpellFlag(MsgUpdate.Flags.PoisonStar, 30, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 30, MsgAttackPacket.AttackEffect.None));
                                }
                                else
                                {

                                    var clientobj = new MsgSpellAnimation.SpellObj(attacked.UID, MsgSpell.SpellID, MsgAttackPacket.AttackEffect.None);
                                    clientobj.Hit = 0;
                                    MsgSpell.Targets.Enqueue(clientobj);
                                }
                            }
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 250, DBSpells);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DragonSwing:
                        {
                            Attack.TimeStamp = 0;
                            user.Send(stream.InteractionCreate(&Attack));
                            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                            {
                                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar)
                                {
                                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(user))
                                        break;
                                }
                            }

                            if (user.Player.ContainFlag(MsgUpdate.Flags.DragonSwing))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.DragonSwing);
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.DragonSwing, Role.StatusFlagsBigVector32.PermanentFlag, true);
                                user.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.DragonSwing, DBSpell.Duration
       , (uint)33, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.DragonSwing, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);



                            MsgSpell.Send(user);




                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DragonFlow:
                        {
                            Attack.TimeStamp = 0;
                            user.Send(stream.InteractionCreate(&Attack));

                            if (user.Player.ContainFlag(MsgUpdate.Flags.DragonFlow))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.DragonFlow);
                                break;
                            }
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.DragonFlow, Role.StatusFlagsBigVector32.PermanentFlag, true, 8);

                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);



                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Stigma:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.MagicShield:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                if (!user.Player.ContainFlag(MsgUpdate.Flags.Shield))
                                {
                                    user.Player.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    if (!attacked.ContainFlag(MsgUpdate.Flags.Shield))
                                    {
                                        attacked.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    if (!attacked.ContainFlag(MsgUpdate.Flags.Shield))
                                    {
                                        attacked.AddSpellFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SoulShackle:
                        {
                            if (user.Player.Map == 10166)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                Role.Player attacked = target as Role.Player;
                                if (!CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    return;
                                if (attacked.Owner.PerfectionStatus.FreeSoul > 0)
                                {
                                    if (Calculate.Base.Rate(attacked.Owner.PerfectionStatus.FreeSoul))
                                    {
                                        attacked.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                                        {
                                            Effect = MsgRefineEffect.RefineEffects.FreeSoul,
                                            Id = attacked.UID,
                                            dwParam = attacked.UID
                                        }), true);
                                        break;
                                    }
                                }
                                if (!attacked.ContainFlag(MsgUpdate.Flags.SoulShackle))
                                {
                                    int rate = attacked.BattlePower - user.Player.BattlePower;
                                    if ((attacked.BattlePower - user.Player.BattlePower) >= 10)
                                    {
                                        user.SendSysMesage("Your BattlePower is lower than your opponent get a better BattlePower to be able to use this skill on him .", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                        break;
                                    }
                                    switch (rate)
                                    {
                                        case 0: rate = 100; break;
                                        case 1: rate = 80; break;
                                        case 2: rate = 70; break;
                                        case 3: rate = 60; break;
                                        case 4: rate = 50; break;
                                        case 5: rate = 40; break;
                                        case 6: rate = 30; break;
                                        case 7: rate = 20; break;
                                        case 8: rate = 10; break;
                                        case 9: rate = 5; break;

                                    }
                                    if (user.Player.BattlePower >= attacked.BattlePower) rate = 100;
                                    if (Role.Core.Rate(rate))
                                    {
                                        attacked.SendUpdate(stream, MsgUpdate.Flags.SoulShackle, DBSpell.Duration, 0, ClientSpell.Level, MsgUpdate.DataType.SoulShackle, false);

                                        attacked.AddSpellFlag(MsgUpdate.Flags.SoulShackle, (int)DBSpell.Duration, true);
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                    }
                                }
                            }


                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.StarofAccuracy:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Invisibility:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    attacked.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.AzureShield:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            int Time = 30;
                            Time = Time * ClientSpell.Level;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                user.Player.AzureShieldLevel = (byte)ClientSpell.Level;
                                user.Player.AzureShieldDefence = (ushort)(3000 * ClientSpell.Level);
                                user.Player.AddSpellFlag(MsgUpdate.Flags.AzureShield, Time, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    Role.Player attacked = target as Role.Player;
                                    attacked.AzureShieldLevel = (byte)ClientSpell.Level;
                                    attacked.AzureShieldDefence = (ushort)(3000 * ClientSpell.Level);
                                    attacked.AddSpellFlag(MsgUpdate.Flags.AzureShield, Time, true);
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                                }
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);//Damage2 this reads in postion of duration in seconds offest 36
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Shield:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.AddFlag(MsgUpdate.Flags.Shield, (int)DBSpell.Duration, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Accuracy:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.AddFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.XpFly:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.OnTransform || user.Player.ContainFlag(MsgUpdate.Flags.Ride) || user.Player.ContainFlag(MsgUpdate.Flags.PathOfShadow))
                            {
                                user.SendSysMesage("You can't use this skill right now!");
                                break;
                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Fly))
                                user.Player.UpdateFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true, 0);
                            else
                                user.Player.AddFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);


                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Fly:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (user.Player.OnTransform || user.Player.ContainFlag(MsgUpdate.Flags.Ride) || user.Player.ContainFlag(MsgUpdate.Flags.PathOfShadow))
                            {
                                user.SendSysMesage("You can't use this skill right now!");
                                break;
                            }

                            if (user.Player.ContainFlag(MsgUpdate.Flags.Fly))
                                user.Player.UpdateFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true, 0);
                            else
                                user.Player.AddFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Bless:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);


                            user.Player.AddFlag(MsgUpdate.Flags.CastPray, Role.StatusFlagsBigVector32.PermanentFlag, true);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FatalStrike:
                        {
                            if (user.Player.Map == 1038 || user.Player.Map == 3868)
                            {
                                user.SendSysMesage("You can't use this skill right now!");
                                break;

                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.FatalStrike, 60);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SuperCyclone:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.SuperCyclone, 20);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Cyclone:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                             , 0, Attack.X, Attack.Y, ClientSpell.ID
                             , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.Cyclone, 20);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Superman:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.XPList) == false)
                                break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                          , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.Superman, 20);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                }
            }
        }
    }
}
