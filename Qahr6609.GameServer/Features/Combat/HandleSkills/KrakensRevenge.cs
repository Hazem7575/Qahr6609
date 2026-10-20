using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
    public class AddBlackSpot
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
            {
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);

                uint Experience = 0;
                uint count = 0;

                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                    {
                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                        {

                            if (count == 5)
                                break;


                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                   
                          Experience+=  ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
      

                            MsgSpell.Targets.Enqueue(AnimationObj);
             

                            if (attacked.Alive)
                            {
                                count++;
                                attacked.BlackSpot = true;
                                attacked.Stamp_BlackSpot = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);

                                user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);

                            }
                        }
                    }
                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                {
                    var attacked = targer as Role.Player;
                    if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) <= DBSpell.Range)
                    {
                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                        {
                            if (count == 5)
                                break;
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                         
                            MsgSpell.Targets.Enqueue(AnimationObj);

                            if (attacked.Alive)
                            {
                                count++;
                                attacked.BlackSpot = true;
                                attacked.Stamp_BlackSpot = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);

                                user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
                            }
                        }
                    }

                }
                Updates.IncreaseExperience.Up(stream,user, Experience);
                Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);
            }
        }
    }
}
