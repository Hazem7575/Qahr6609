using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Player
    {
        public static void Execute(MsgSpellAnimation.SpellObj obj, Client.GameClient client, Role.Player attacked)
        {

            

            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.SkillTournament)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgSkillTournament;
                        tournament.KillSystem.Update(client);
                        tournament.KillSystem.CheckDead(attacked.UID);

                        if (attacked.SkillTournamentLifes > 0)
                        {
                            attacked.SkillTournamentLifes -= 1;
                            attacked.Owner.SendSysMesage("You have " + attacked.SkillTournamentLifes + " more life's left.", MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                            return;
                        }
                        if (attacked.SkillTournamentLifes == 0)
                        {
                            client.Player.TournamentKills += 1;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                attacked.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                                attacked.HitPoints = 0;
                                attacked.Dead(client.Player, attacked.X, attacked.Y, client.Player.UID);
                            }
                        }
                      
                    }
                }
            }
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FreezeWar)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        if (client.Player.GarmentId != attacked.GarmentId)
                        {
                            if (attacked.ContainFlag(MsgUpdate.Flags.Freeze) == false)
                                attacked.AddFlag(MsgUpdate.Flags.Freeze, Role.StatusFlagsBigVector32.PermanentFlag, true);
                        }
                        else
                        {
                            if (client.Player.GarmentId == attacked.GarmentId)
                                if (attacked.ContainFlag(MsgUpdate.Flags.Freeze))
                                    attacked.RemoveFlag(MsgUpdate.Flags.Freeze);
                        }
                        return;
                    }
                }
            }
            var currentTournament = MsgTournaments.MsgSchedules.CurrentTournament;
            if (currentTournament.Type == MsgTournaments.TournamentType.FootBall)
            {
                var tournament = (MsgTournaments.MsgFootball)currentTournament;
                if (tournament.InTournament(client))
                {
                    if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                        tournament.PassTheBall(client, attacked.Owner);
                    return;
                }
            }
            #region CalmWind
            if (client.PerfectionStatus.CalmWind > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.CalmWind))
                {
                    foreach (var spell in client.MySpells.ClientSpells.Values)
                        spell.ColdTime = Time32.Now;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.CalmWind,
                            Id = client.Player.UID,
                            dwParam = client.Player.UID
                        }), true);
                    }
                }
            }
            #endregion
            if (attacked.Owner.PerfectionStatus.MirrorOfSin > 0)//if error in perfection stats
            {
                if (AttackHandler.Calculate.Base.Rate(attacked.Owner.PerfectionStatus.MirrorOfSin))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.MirrorOfSin,
                            Id = attacked.UID,
                            dwParam = attacked.UID
                        }), true);
                        if (attacked.OnXPSkill() == MsgUpdate.Flags.Normal && !client.Player.ContainFlag(MsgUpdate.Flags.XPList))
                            attacked.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                    }
                }
            }
            if (attacked.Owner.PerfectionStatus.LightOfStamina > 0)
            {
                if (attacked.Owner.PrestigeLevel >= client.PrestigeLevel)
                {
                    if (AttackHandler.Calculate.Base.Rate(attacked.Owner.PerfectionStatus.LightOfStamina))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            attacked.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.LightOfStamina,
                                Id = attacked.UID,
                                dwParam = attacked.UID
                            }), true);
                            if (attacked.Stamina < 100)
                            {
                                attacked.Stamina = 100;
                                attacked.SendUpdate(stream, attacked.Stamina, MsgUpdate.DataType.Stamina);
                            }
                        }
                    }
                }
            }
          //  if (attacked.Owner.PerfectionStatus.LightOfStamina > 0)
            
            if (attacked.Owner.PerfectionStatus.DivineGuard > 0)
            {
                if (attacked.Owner.PrestigeLevel >= client.PrestigeLevel)
                {
                    if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.DivineGuard))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.DivineGuard,
                                Id = client.Player.UID,
                                dwParam = client.Player.UID
                            }), true);
                            if (!client.Player.ContainFlag(MsgUpdate.Flags.DivineGuard))
                            {
                                
                                client.Player.AddFlag(MsgUpdate.Flags.DivineGuard, 15, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.DivineGuard, 15, 0, 0, MsgUpdate.DataType.AzureShield);

                            }
                        }
                    }
                }
            }
           
            if (client.PerfectionStatus.KillingFlash > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.KillingFlash))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.KillingFlash,
                            Id = client.Player.UID,
                            dwParam = client.Player.UID
                        }), true);
                        if (!client.Player.ContainFlag(MsgUpdate.Flags.XPList) && client.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
                            client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                    }
                }
            }
            if (client.PerfectionStatus.DrainingTouch > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.DrainingTouch))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.DrainingTouch,
                            Id = client.Player.UID,
                            dwParam = client.Player.UID
                        }), true);

                        bool update = false;
                        if (client.Player.HitPoints < client.Player.Owner.Status.MaxHitpoints)
                        {
                            update = true;
                            client.Player.HitPoints = (int)client.Player.Owner.Status.MaxHitpoints;
                        }
                        if (client.Player.Mana < client.Player.Owner.Status.MaxMana)
                        {
                            update = true;
                            client.Player.Mana = (ushort)client.Player.Owner.Status.MaxMana;
                        }
                        if (update)
                        {
                            client.Player.SendUpdateHP();
                            client.Player.SendUpdate(stream, client.Player.Mana, MsgUpdate.DataType.Mana, false);
                        }
                    }
                }
            }
            if (attacked.Owner.PerfectionStatus.BloodSpawn > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(attacked.Owner.PerfectionStatus.BloodSpawn))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        attacked.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.BloodSpawn,
                            Id = attacked.UID,
                            dwParam = attacked.UID
                        }), true);
                        bool update = false;
                        if (attacked.HitPoints < attacked.Owner.Status.MaxHitpoints)
                        {
                            update = true;
                            attacked.HitPoints = (int)attacked.Owner.Status.MaxHitpoints;
                        }
                        if (attacked.Mana < attacked.Owner.Status.MaxMana)
                        {
                            update = true;
                            attacked.Mana = (ushort)attacked.Owner.Status.MaxMana;
                        }
                        if (update)
                        {
                            attacked.SendUpdateHP();
                            attacked.SendUpdate(stream, attacked.Mana, MsgUpdate.DataType.Mana, false);
                        }
                    }
                }
            }
            if (Calculate.Base.Rate(10))
            {
                CheckAttack.CheckItems.RespouseDurability(client);
            }
            ushort X = attacked.X;
            ushort Y = attacked.Y;

            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                       // var tournament = (Game.MsgTournaments.MsgDragonWar)Game.MsgTournaments.MsgSchedules.CurrentTournament;
                        if (attacked.ContainFlag(MsgUpdate.Flags.DragonSwing))
                        {
                            if (attacked.DragonWarHits == 0)
                            {
                              //  tournament.KillSystem.CheckDead(client.Player.UID);
                                attacked.Dead(client.Player, X, Y, 0);

                                client.Player.DragonWarScore += 20;
                                client.Player.AddFlag(MsgUpdate.Flags.DragonSwing, Role.StatusFlagsBigVector32.PermanentFlag, true);
                                client.Player.DragonWarHits = 5;
                                attacked.RemoveFlag(MsgUpdate.Flags.DragonSwing);
                            }
                            else
                            {

                                client.Player.DragonWarScore += 5;
                                attacked.DragonWarHits -= 1;
                            }
                        }
                        else if (client.Player.ContainFlag(MsgUpdate.Flags.DragonSwing))
                        {
                            //tournament.KillSystem.Update(client);
                            client.Player.DragonWarScore += 15;
                            attacked.Dead(client.Player, X, Y, 0);
                        }
                        return;

                    }
                }
            }
            if (Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.InTournament(attacked.Owner) && Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.InTournament(client))
            {
                Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.ResondInteract(client.Player.UID, obj.Damage);
            }
            if (attacked.HitPoints <= obj.Damage)
            {
                if (client.Player.OnTransform)
                {
                    client.Player.TransformInfo.FinishTransform();
                }
                attacked.Dead(client.Player, X, Y, 0);

            }
            else
            {
                CheckAttack.CheckGemEffects.CheckRespouseDamage(attacked.Owner);
                client.UpdateQualifier(client, attacked.Owner, obj.Damage);
                attacked.HitPoints -= (int)obj.Damage;
            }


        }
    }
}
