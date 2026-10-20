using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{

    public class Rectangle
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.Rectangle:
                        {


                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, DBSpell.Range, 0, ClientSpell.ID);

                            byte LineRange = 2;

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {

                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                                    continue;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {


                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(&_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                        {
                                            if (Program.MapCounterHits.Contains(user.Player.Map))
                                                user.Player.HitShoot += 1;
                                            user.Player.HitSBShoot += 1;
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(&_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < DBSpell.Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            if (user.OnAutoAttack == false)
                                            {
                                                unsafe
                                                {
                                                    InteractQuery _attack = new InteractQuery();
                                                    _attack.Damage = (int)AnimationObj.Damage;
                                                    _attack.X = attacked.X;
                                                    _attack.Y = attacked.Y;
                                                    _attack.OpponentUID = attacked.UID;
                                                    _attack.AtkType = MsgAttackPacket.AttackID.Physical;
                                                    _attack.UID = user.Player.UID;
                                                    _attack.Effect = AnimationObj.Effect;
                                                    user.Send(stream.InteractionCreate(&_attack));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user, user.OnAutoAttack ? true : false);
                            break;
                        }
                }
            }
        }
    }
}

