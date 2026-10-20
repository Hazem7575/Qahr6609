using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
  public  class CallPet
    {
      public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
      {
          if (user.Player.MyClones.Count != 0)
          {
              foreach (var clone in user.Player.MyClones.GetValues())
                  clone.RemoveThat(user);

              user.Player.MyClones.Clear();
              return;
          }

          Database.MagicType.Magic DBSpell;
          MsgSpell ClientSpell;
          if (CheckAttack.CanUseSpell.Verified(Attack,user, DBSpells, out ClientSpell, out DBSpell))
          {
              switch (ClientSpell.ID)
              {
                  case (ushort)Role.Flags.SpellID.Thundercloud:
                      {
                          // Summon point must be within view range (max(DBSpell.Range, 18)).
                          if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, Attack.X, Attack.Y) > Math.Max((int)DBSpell.Range, 18))
                              return;
                          MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                           , 0, Attack.X, Attack.Y, ClientSpell.ID
                           , ClientSpell.Level, ClientSpell.UseSpellSoul);


                          
                         Game.MsgMonster.MonsterFamily famil;
                         if (Database.Server.MonsterFamilies.TryGetValue(1, out famil))
                         {
                             Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                             Monster.SpawnX = Attack.X;

                             Monster.SpawnY = Attack.Y;
                             Monster.MaxSpawnX = (ushort)(Attack.X + 1);
                             Monster.MaxSpawnY = (ushort)(Attack.Y + 1);
                             Monster.MapID = user.Player.Map;
                             Monster.SpawnCount = 1;
                             Game.MsgMonster.MonsterRole rolemonster = user.Map.MonstersColletion.Add(Monster, true, user.Player.DynamicID, true);
                             if (rolemonster == null)
                             {
                                 //invalid x ,y
                                 return;
                             }
                             rolemonster.Family.ID = 999996641;
                             rolemonster.IsFloor = true;
                             rolemonster.FloorStampTimer = DateTime.Now.AddSeconds(7);
                             rolemonster.Family.Settings = Game.MsgMonster.MonsterSettings.Lottus;

                             rolemonster.FloorPacket = new MsgFloorItem.MsgItemPacket();
                             rolemonster.FloorPacket.m_UID = rolemonster.UID;
                             rolemonster.FloorPacket.m_ID =MsgFloorItem.MsgItemPacket.Thundercloud;
                             rolemonster.FloorPacket.m_X = Attack.X;
                             rolemonster.FloorPacket.m_Y = Attack.Y;
                             rolemonster.FloorPacket.MaxLife = 25;
                             rolemonster.FloorPacket.Life = 25;
                             rolemonster.FloorPacket.DropType = MsgFloorItem.MsgDropID.Effect;
                             rolemonster.FloorPacket.m_Color = 13;
                             rolemonster.FloorPacket.ItemOwnerUID = user.Player.UID;
                             rolemonster.FloorPacket.GuildID = user.Player.GuildID;
                             rolemonster.FloorPacket.FlowerType = 2;//2;
                             rolemonster.FloorPacket.Timer = Role.Core.TqTimer(rolemonster.FloorStampTimer);
                             rolemonster.PetFlag = 3;
                             if (DBSpell.Level >= 5)
                                 rolemonster.PetFlag = 9;

                             rolemonster.HitPoints = 10;
                             rolemonster.Boss = 1;
                             rolemonster.Name = "Thundercloud";
                             rolemonster.Mesh = 980;
                             rolemonster.StampFloorSecounds = 10000;
                             rolemonster.FloorStampTimer = DateTime.Now.AddSeconds(1);
                             rolemonster.RemoveFloor = DateTime.Now.AddSeconds(19);

                             rolemonster.DBSpell = DBSpell;
                             rolemonster.Family.MaxHealth = 25;
                             rolemonster.HitPoints = 25;
                             rolemonster.OwnerFloor = user;
                             rolemonster.SpellLevel = DBSpell.Level;
                             user.Map.View.EnterMap<Role.IMapObj>(rolemonster);
                             rolemonster.Send(rolemonster.GetArray(stream, false));

                             ActionQuery action = new ActionQuery()
                             {
                                 dwParam = 8,
                                 ObjId = rolemonster.UID,
                                 Type = ActionType.ReviveMonster,
                                 wParam1 = rolemonster.X,
                                 wParam2 = rolemonster.Y
                             };
                             user.Player.View.SendView(stream.ActionCreate(&action), true);
                             
                         }

                       

                         /*44 00 F3 07 80 AE 0A 00 A8 10 00 00 03 00 00 00      ;D ó®
¨     
D4 03 00 00 00 00 00 00 0E 00 B7 00 A7 00 54 68      ;Ô       · § Th
75 6E 64 65 72 63 6C 6F 75 64 00 00 00 00 00 00      ;undercloud      
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 54 51 53 65 72 76 65 72                  ;    TQServer*/
                          Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1000, DBSpells);
                          MsgSpell.SetStream(stream);
                          MsgSpell.Send(user);
                          break;
                      }
                  case (ushort)Role.Flags.SpellID.ShadowClone:
                      {

                          MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);

                          Role.Clone.CreateShadowClone(user.Player,stream);


                          foreach (var clone in user.Player.MyClones.GetValues())
                          {

                              ActionQuery action = new ActionQuery()
                              {
                                  ObjId = clone.UID,
                                  Type = ActionType.ReviveMonster,
                                  wParam1 = user.Player.X,
                                  wParam2 = user.Player.Y
                              };
                              user.Player.View.SendView(stream.ActionCreate(&action),true);
                          }
                         
                          Updates.UpdateSpell.CheckUpdate(stream,user,Attack, 1000, DBSpells);
                          MsgSpell.SetStream(stream);
                          MsgSpell.Send(user);

                          break;
                      }
              }
          }
      }
    }
}
