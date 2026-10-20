using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.Calculate
{
    public class Physical
    {
        public static void OnMonster(Role.Player player, MsgMonster.MonsterRole monster, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, byte MultipleDamage = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (monster.IsFloor)
            {
                SpellObj.Damage = 2;
                return;
            }
            if (DBSpell != null)
            {
                //if (Base.Dodged(player.Owner, monster))
                //{
                //    SpellObj.Damage = 0;
                //    return;
                //}
                if (DBSpell.ID == (ushort)Role.Flags.SpellID.ScarofEarth)
                {
                    if (monster.Family.ID == 2700)
                    {
                        SpellObj.Damage = 1;
                        return;
                    }
                }

            }
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            Damage = (int)player.Owner.AjustMaxAttack((uint)Damage);

            var rawDefense = monster.Family.Defense;

            Damage = Math.Max(0, Damage - rawDefense);

            if (DBSpell != null && DBSpell.Damage < 10)
                DBSpell.Damage = 10;
            if (player.ContainFlag(MsgUpdate.Flags.FatalStrike))
            {
                Damage = Base.MulDiv((int)Damage, 500, 100);
            }
            else if (DBSpell != null && DBSpell.ID == (ushort)Role.Flags.SpellID.Omnipotence)
            {
                Damage = Base.MulDiv((int)Damage, 350, 100);
            }
            else if (MultipleDamage != 0)
            {
                Damage = Damage * MultipleDamage;
            }
            else
            {
                if (DBSpell != null && DBSpell.ID == 12770)
                {
                    if (monster.Boss == 1)
                        Damage = Base.MulDiv((int)Damage, 350, 100);
                    else
                        Damage = Base.MulDiv((int)Damage, 500, 100);
                }
                else
                    Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);
            }
            if (player.ContainFlag(MsgUpdate.Flags.ManiacDance))
            {
                if (monster.Boss == 0)
                    Damage *= 10;
                else
                    Damage *= 3;
            }

            if (player.ContainFlag(MsgUpdate.Flags.Oblivion))
                Damage = Base.MulDiv((int)Damage, 200, 100);

            Damage = Base.AdjustMinDamageUser2Monster(Damage, player.Owner);
            Damage = Base.CalcDamageUser2Monster(Damage, monster.Family.Defense, player.Level, monster.Level, false);
            Damage = (int)Base.BigMulDiv(Damage, monster.Family.Defense2, Client.GameClient.DefaultDefense2);

            if (monster.Family.Defense2 > 0)
                Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.AjustPhysicalDamageIncrease(), 0);


            SpellObj.Damage = (uint)Math.Max(1, Damage);

            //if (monster.Boss == 0)
            {
                if (player.ContainFlag(MsgUpdate.Flags.Superman))
                    SpellObj.Damage *= 5;
            }
            if (Base.Rate(player.Owner.PerfectionStatus.LuckyStrike))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                    {
                        Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
                        Id = player.UID
                    }), true);
                }
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
                SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 200, 100);

            }

            if (Base.GetRefinery())
            {
                if (player.Owner.Status.CriticalStrike > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage += (SpellObj.Damage * (player.Owner.AjustCriticalStrike() / 100)) / 100;
                }
            }

            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                SpellObj.Damage /= 10;
            if (monster.Family.ID == 20211)
                SpellObj.Damage = 1;
        }
        public static void OnPlayer(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, bool StackOver = false, int IncreaseAttack = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex) || target.ContainFlag(MsgUpdate.Flags.ManiacDance))
            {
                SpellObj.Damage = (uint)Calculate.Base.CalculateExtraAttack((uint)SpellObj.Damage, player.Owner.Status.PhysicalDamageIncrease, target.Owner.Status.PhysicalDamageDecrease);
                return;
            }
            bool update = false;
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);


            Damage = (int)player.Owner.AjustAttack((uint)Damage);

            var rawDefense = target.Owner.AjustDefense;
            if (Damage > rawDefense)
                Damage -= (int)rawDefense;
            else
                Damage = 1;

            if (DBSpell != null)
            {
                if (DBSpell.ID == 12080)
                {
                    {
                        Damage = Base.MulDiv((int)Damage, 135, 100);
                        update = true;
                    }
                }
                else if (DBSpell.ID == 12290)
                {
                    Damage = Base.MulDiv((int)Damage, 60, 100);
                    update = true;
                }
                else if (DBSpell.ID == 11050)
                {
                    Damage = Base.MulDiv((int)Damage, 50, 100);
                    update = true;
                }
                else if (DBSpell.ID == (ushort)Role.Flags.SpellID.Omnipotence)
                {
                    Damage = Base.MulDiv((int)Damage, 25, 100);
                    update = true;
                }
            }
            if (SpellObj.Damage > 0)
            {
                if (DBSpell != null)
                {
                    int val = 0;
                    if (Database.AttackCompatetor.CheckDmg(DBSpell.ID, out val))
                    {
                        int dmg = (int)SpellObj.Damage;
                        SpellObj.Damage = (uint)Base.MulDiv((int)dmg, val, 100);
                    }
                }
            }
            if (!update)
            {
                if (DBSpell != null)
                    Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);

            }
         
            update = false;
            Damage = Database.Disdain.UserAttackUser(player, target, Damage);

            var TortoisePercent = target.Owner.GemValues(Role.Flags.Gem.SuperTortoiseGem);
            if (TortoisePercent > 0)
                Damage -= Damage * Math.Min((int)TortoisePercent, 60) / 100;

            if (target.Reborn > 0)
                Damage = (int)Base.BigMulDiv((int)Damage, target.Owner.GetDefense2(), Client.GameClient.DefaultDefense2);

            Damage -= (int)(Damage * target.Owner.Status.ItemBless / 100);
            Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.Status.PhysicalDamageIncrease, target.Owner.Status.PhysicalDamageDecrease);

            SpellObj.Damage = (uint)Math.Max(1, Damage);
            if (player.BlessTime > 0)
            {
                if (Base.Rate(1))
                {
                    using (var rect = new ServerSockets.RecycledPacket())
                    {
                        var stream = rect.GetStream();
                        player.SendString(stream, MsgStringPacket.StringID.Effect, true, "LuckyGuy");
                    }
                    Damage = Base.MulDiv((int)Damage, 200, 100);
                }
            }
            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
            {
                if (SpellObj.Damage > target.AzureShieldDefence)
                {
                    Calculate.AzureShield.CreateDmg(player, target, target.AzureShieldDefence);
                    target.RemoveFlag(MsgUpdate.Flags.AzureShield);
                    SpellObj.Damage -= target.AzureShieldDefence;

                }
                else
                {
                    target.AzureShieldDefence -= (ushort)SpellObj.Damage;
                    Calculate.AzureShield.CreateDmg(player, target, SpellObj.Damage);
                    SpellObj.Damage = 1;
                }
            }
            if (!update && player.Owner.Status.Breakthrough > 0)
            {
                if (Base.GetRefinery(player.Owner.Status.Breakthrough / 10, target.Owner.Status.Counteraction / 10) && player.BattlePower < target.BattlePower)
                {

                    SpellObj.Damage = SpellObj.Damage * 150 / 100;
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Break;
                    update = true;

                }
            }
            if (CheckAttack.BlockRefect.CanUseReflect(player.Owner))
            {
                if (!StackOver)
                {
                    MsgSpellAnimation.SpellObj InRedirect;
                    if (BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out InRedirect))
                        SpellObj = InRedirect;
                }
            }
            if (target.Owner.Equipment.ShieldID != 0)
            {
                int Block = (int)(target.Owner.Status.Block / 100);
                Block += (int)((target.ShieldBlockDamage * Block) / 100);
                uint Change = (uint)Math.Min(70, Block / 2);
                if (player.Owner.PerfectionStatus.ShieldBreak > 0)
                    if (Base.Rate(player.Owner.PerfectionStatus.ShieldBreak))
                    {
                        if (!target.ContainFlag(MsgUpdate.Flags.ShieldBreak))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                target.SendUpdate(stream, MsgUpdate.Flags.ShieldBreak, 15, 0, 0, MsgUpdate.DataType.AzureShield);
                            }
                            target.AddFlag(MsgUpdate.Flags.ShieldBreak, 15, true);
                        }
                    }
                if (target.ContainFlag(MsgUpdate.Flags.ShieldBreak))
                {
                    if (Change > 20)
                        Change -= 20;
                    else
                        Change = 0;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.ShiledBreak,
                            Id = player.UID,
                            dwParam = player.UID
                        }), true);
                    }
                }
                if (Base.Rate((byte)Change))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
                    SpellObj.Damage = 0;
                }
            }

            if (target.MyClones.Count != 0)
            {
                foreach (var clone in target.MyClones.GetValues())
                    clone.RemoveThat(target.Owner);

                target.MyClones.Clear();
            }
            if (player.Owner.Status.CriticalStrike > 0)
            {
                uint r = (player.Owner.Status.CriticalStrike / 100 - target.Owner.Status.Immunity / 100);
                if (Base.RateStrick(player.Owner.Status.CriticalStrike / 100, target.Owner.Status.Immunity / 100) && player.BattlePower >= target.BattlePower)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage = SpellObj.Damage * 150 / 100;
                    update = true;
                }
                if (!update && Base.Rate(player.Owner.PerfectionStatus.LuckyStrike))
                {
                    if (Base.Rate(target.Owner.PerfectionStatus.StrikeLock))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.StrikeLockLevel,
                                Id = player.UID
                            }), true);
                        }
                    }
                    else
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                            {
                                Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
                                Id = player.UID
                            }), true);
                        }
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
                        Damage = Base.MulDiv((int)Damage, 200, 100);
                        update = true;
                    }
                }

            }
            if (target.Owner.PerfectionStatus.StrikeLock > 0)
            {
                if (Role.Core.Rate(target.Owner.PerfectionStatus.StrikeLock))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                        {
                            Effect = MsgRefineEffect.RefineEffects.StrikeLockLevel,
                            Id = target.UID,
                            dwParam = target.UID
                        }), true);
                    }
                }
            }
            #region ExtraPanleDamge
            if (DBSpell != null)
            {
                Co_Panle.ExtraPanleDamge(DBSpell, SpellObj);
            }
            #endregion
        }
        public static void OnNpcs(Role.Player player, Role.SobNpc target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);

            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);

            Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);


            SpellObj.Damage = (uint)Math.Max(1, Damage);

            if (Base.GetRefinery())
            {
                if (player.Owner.Status.CriticalStrike > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage = Base.CalculateArtefactsDmg(SpellObj.Damage, player.Owner.Status.CriticalStrike, 0);
                }
            }
            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.PhysicalDamageIncrease, 0);
            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
                SpellObj.Damage = 100;
        }
    }
}
