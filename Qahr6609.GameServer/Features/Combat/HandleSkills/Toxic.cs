using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
  public  class Toxic
    {
      public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
      {
          Database.MagicType.Magic DBSpell;
          MsgSpell ClientSpell;
          if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
          {
              // Target point must be within view range (max(DBSpell.Range, 18)).
              if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, Attack.X, Attack.Y) > Math.Max((int)DBSpell.Range, 18))
                  return;
              switch (ClientSpell.ID)
              {
                  case (ushort)Role.Flags.SpellID.ToxicFog:
                      {
                          MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);
                          uint Damage = 0;
                          foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                          {
                              MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                              if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) < 5)
                              {
                                  if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                  {
                                      if (attacked.Boss == 0)
                                      {
                                          attacked.PoisonLevel = (byte)(ClientSpell.Level + ClientSpell.UseSpellSoul);
                                          Damage = 1000;
                                          attacked.AddSpellFlag(MsgUpdate.Flags.Poisoned, (int)DBSpell.Duration, true, 1);
                                      }
                                  }
                              }
                          }
                          foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                          {
                              var attacked = targer as Role.Player;
                              if (attacked.RealBattlePower > user.Player.RealBattlePower)
                              {
                                  int rate = attacked.BattlePower - user.Player.BattlePower;
                                  if ((attacked.BattlePower - user.Player.BattlePower) >= 10)
                                  {
                                      user.SendSysMesage("Your BattlePower is lower than your opponent get a better BattlePower to be able to use this skill on him .", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                      continue;
                                  }
                                  if ((rate <= 0 || Role.Core.Rate(100 - (Math.Min(rate, 10) * 10))) == false)
                                  {
                                      continue;
                                  }
                              }
                              if (Calculate.Base.GetDistance(Attack.X, Attack.Y, attacked.X, attacked.Y) < 5)
                              {
                                  if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                  {
                                      attacked.PoisonLevel = (byte)(ClientSpell.Level + ClientSpell.UseSpellSoul);
                                      Damage = 1000;
                                      attacked.AddSpellFlag(MsgUpdate.Flags.Poisoned, (int)DBSpell.Duration, true, 2);
                                  }
                              }
                          }

                          Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Damage, DBSpells);
                          MsgSpell.SetStream(stream);
                          MsgSpell.Send(user);

                          break;
                      }

              }
          }
      }
    }
}
