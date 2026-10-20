using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
    public class RemoveStamin
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.FrostGazeII:
                    case (ushort)Role.Flags.SpellID.FrostGazeIII:
                    case (ushort)Role.Flags.SpellID.FrostGazeI:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                           
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        if (Role.Core.Rate(100 - 5 * (attacked.BattlePower - user.Player.BattlePower)))
                                        {
                                            uint removeStamin = (uint)DBSpell.Damage;
                                            if (attacked.Stamina >= removeStamin)
                                            {
                                                attacked.Stamina -= (ushort)removeStamin;
                                            }
                                            else
                                                attacked.Stamina = 0;

                                            attacked.SendUpdate(stream, attacked.Stamina, MsgUpdate.DataType.Stamina, false);

                                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = removeStamin, UID = targer.UID, Hit = 1 });
                                        }
                                        else
                                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = 0, UID = targer.UID, Hit = 0 });
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);
                            break;
                        }
                   
                }
            }
        }
    }
}
