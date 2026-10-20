using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Monster
    {
        public static uint Execute(ServerSockets.Packet stream, MsgSpellAnimation.SpellObj obj, Client.GameClient client, MsgMonster.MonsterRole monster)
        {
            obj.Damage = Conquer6609.Framework.Modules.ModuleManager.MonsterDamage(client, monster, obj.Damage);

            // Hits arrive from several threads (socket, MapGroupThread auto-attack, auto-hunt): the
            // "kill or subtract" decision is taken under the monster's HpSync from one HP snapshot, so a
            // concurrent kill can no longer make HitPoints wrap around, and only the caller that really
            // killed the monster runs the death (drops/XP) and the team XP share.
            // While another caller runs the death (DeathInProgress), the hit leaves HP alone and kills nothing.
            bool killHit, hpHit;
            lock (monster.HpSync)
            {
                hpHit = !monster.DeathInProgress;
                uint hp = monster.HitPoints;
                killHit = hpHit && hp <= obj.Damage;
                if (hpHit && !killHit)
                    monster.HitPoints = hp - obj.Damage;
            }
            if (killHit)
            {
                if (monster.Boss == 1)
                    monster.FlushBossScore(stream);// last pending harm ranking before the boss dies
                client.Map.SetMonsterOnTile(monster.X, monster.Y, false);
                if (monster.TryDead(stream, client, client.Player.UID, client.Map))
                {
                    if (client.Team != null)
                        client.Team.ShareExperience(stream, client, monster);
                }
            }
            else if (hpHit)
            {
                if (monster.Boss == 1)
                {
                    if (client.Player.Map == 10137 || client.Player.Map == 10166)
                    {
                        monster.SendScore(stream, client, obj.Damage);
                    }
                    // Boss HP bar ([Performance] BossHpBarMs, default every hit): when throttled, a skipped update is sent by the next
                    // hit or by the kernel tick (MonsterRole.FlushBossUpdates).
                    if (monster.TakeBossHpSlot())
                    {
                        Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, monster.UID, 2);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, monster.Family.MaxHealth);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, monster.HitPoints);
                        stream = Upd.GetArray(stream);
                        client.Player.View.SendView(stream, true);
                    }
                }
            }

            #region CalmWind
            if (client.PerfectionStatus.CalmWind > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.CalmWind))
                {
                    foreach (var spell in client.MySpells.ClientSpells.Values)
                        spell.ColdTime = Extensions.Time32.Now;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {

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
           
           
            if (client.PerfectionStatus.KillingFlash > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.KillingFlash))
                {
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
            if (client.PerfectionStatus.DrainingTouch > 0)
            {
                if (AttackHandler.Calculate.Base.Rate(client.PerfectionStatus.DrainingTouch))
                {
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
            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) != MsgMonster.MonsterSettings.Guard)
            {
                if (obj.Damage > monster.Family.MaxHealth)
                    return (uint)AdjustExp(monster.Family.MaxHealth, client.Player.Level, monster.Level);
                else
                    return (uint)AdjustExp((int)obj.Damage, client.Player.Level, monster.Level);
            }
            else
                return 0;
        }
        public static int AdjustExp(int nDamage, int nAtkLev, int nDefLev)
        {

        
            int nExp = nDamage;

            int nNameType = Calculate.Base.GetNameType(nAtkLev, nDefLev);
            int nDeltaLev = nAtkLev - nDefLev;
            if (nNameType == Calculate.Base.StatusConstants.NAME_GREEN)
            {
                if (nDeltaLev >= 3 && nDeltaLev <= 5)
                    nExp = nExp * 70 / 100;
                else if (nDeltaLev > 5 && nDeltaLev <= 10)
                    nExp = nExp * 20 / 100;
                else if (nDeltaLev > 10 && nDeltaLev <= 20)
                    nExp = nExp * 10 / 100;
                else if (nDeltaLev > 20)
                    nExp = nExp * 5 / 100;
            }
            else if (nNameType == Calculate.Base.StatusConstants.NAME_RED)
            {
                nExp = (int)(nExp * 1.3);
            }
            else if (nNameType == Calculate.Base.StatusConstants.NAME_BLACK)
            {
                if (nDeltaLev >= -10 && nDeltaLev < -5)
                    nExp = (int)(nExp * 1.5);
                else if (nDeltaLev >= -20 && nDeltaLev < -10)
                    nExp = (int)(nExp * 1.8);
                else if (nDeltaLev < -20)
                    nExp = (int)(nExp * 2.3);
            }

            return Math.Max(10, nExp);
        }
    }
}
