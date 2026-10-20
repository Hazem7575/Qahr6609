using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
    public class MoveLine
    {//سحبه القرصان
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);
                user.Player.AttackStamp = Extensions.Time32.Now.AddMilliseconds(500);
                List<Algoritms.InLineAlgorithm.coords> coord = Algoritms.MoveCoords.CheckBladeTeampsCoords(user.Player.X, user.Player.Y, Attack.X, Attack.Y, user.Map);
                if (coord == null || coord.Count == 0) return;
                MsgSpell.X = (ushort)coord[coord.Count - 1].X;
                MsgSpell.Y = (ushort)coord[coord.Count - 1].Y;

                if (!CheckAttack.CheckFloors.CheckGuildWar(user, coord[coord.Count - 1].X, coord[coord.Count - 1].Y))
                {
                    return;
                }
                user.Map.View.MoveTo<Role.IMapObj>(user.Player, MsgSpell.X, MsgSpell.Y);
                user.Player.X = MsgSpell.X;
                user.Player.Y = MsgSpell.Y;

                uint Experience = 0;
                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (attacked.Name.Contains("Guard1"))
                    {
                        if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 2)
                        {
                            if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                            {
                                if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 1, coord))
                                    continue;
                                MsgSpellAnimation.SpellObj AnimationObj;
                                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);

                                if (target.Alive)
                                {
                                    if (Role.Core.Rate(35))
                                    {
                                        attacked.BlackSpot = true;
                                        attacked.Stamp_BlackSpot = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);

                                        user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
                                    }
                                }
                                if (attacked.BlackSpot)
                                {//ادرالين راش
                                    if (Game.MsgServer.AttackHandler.Calculate.Base.Rate(35))
                                    {
                                        MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID, 0, user.Player.X, user.Player.Y, 11130, 4, 0);
                                        RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                        RemoveCloudDown.SetStream(stream);
                                        RemoveCloudDown.JustMe(user);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 15)
                        {
                            if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                            {
                                if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 1, coord))
                                    continue;
                                MsgSpellAnimation.SpellObj AnimationObj;
                                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);

                                if (target.Alive)
                                {
                                    if (Role.Core.Rate(100))
                                    {
                                        attacked.BlackSpot = true;
                                        attacked.Stamp_BlackSpot = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);

                                        user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
                                    }
                                }
                                if (attacked.BlackSpot)
                                {//ادرالين راش
                                    if (Game.MsgServer.AttackHandler.Calculate.Base.Rate(100))
                                    {
                                        MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID, 0, user.Player.X, user.Player.Y, 11130, 4, 0);
                                        RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                        RemoveCloudDown.SetStream(stream);
                                        RemoveCloudDown.JustMe(user);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                {
                    var attacked = targer as Role.Player;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                    {
                        if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 1, coord))
                            continue;
                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                        {

                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);

                            if (attacked.Alive)
                            {
                                if (Role.Core.Rate(35))
                                {
                                    attacked.BlackSpot = true;
                                    attacked.Stamp_BlackSpot = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);

                                    user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
                                }
                            }
                            if (attacked.BlackSpot)
                            {//ادرالين راش
                                if (Game.MsgServer.AttackHandler.Calculate.Base.Rate(35))
                                {
                                    MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID, 0, user.Player.X, user.Player.Y, 11130, 4, 0);
                                    RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                    RemoveCloudDown.SetStream(stream);
                                    RemoveCloudDown.JustMe(user);
                                }
                            }
                        }
                    }

                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                {
                    var attacked = targer as Role.SobNpc;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= 6)
                    {
                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                        {
                            if (!Algoritms.MoveCoords.InRange(attacked.X, attacked.Y, 1, coord))
                                continue;
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                        }
                    }
                }
                Updates.IncreaseExperience.Up(stream, user, Experience);
                Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                MsgSpell.SetStream(stream);
                MsgSpell.SendRole(user);
                MsgSpell.Send(user);

            }
        }
    }
}
