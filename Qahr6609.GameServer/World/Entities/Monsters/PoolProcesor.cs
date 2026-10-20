using System;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;

namespace Conquer6609.Game.MsgMonster
{
    public class PoolProcesses
    {
        // [Performance] MonsterAiMode: 0 = today's behaviour (every viewer runs the AI of every monster it sees);
        // 1 = a monster's buffers/AI step runs at most once per MapGroupThread tick, whichever viewer reaches it
        //     first (guards are still run per viewer: they attack the viewer); 2 = mode 1 + the three AI steps
        //     (buffers 500 ms / guards 700 ms / monsters 400 ms) run independently instead of one per tick.
        public static readonly int MonsterAiMode = Conquer6609.Framework.Settings.Int("Performance", "MonsterAiMode", 0);

        public unsafe static void BuffersCallback(Client.GameClient client, Extensions.Time32 clock)
        {
            try
            {
                Extensions.Time32 timer = Extensions.Time32.Now;
                bool dedupe = MonsterAiMode >= 1;
                int tick = MapGroupThread.AiTick;
                var Array = client.Player.View.Roles(Role.MapObjectType.Monster);
                foreach (var map_mob in Array)
                {

                    var Mob = (map_mob as MonsterRole);
                    if (dedupe)
                    {
                        if (Mob.LastBuffTick == tick)
                            continue;
                        Mob.LastBuffTick = tick;
                    }


                    if (Mob.IsFloor)
                    {
                        if (Mob.FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.Thundercloud && DateTime.Now > Mob.FloorStampTimer)
                        {
                            Mob.FloorStampTimer = DateTime.Now.AddSeconds(1);
                            Mob.StampFloorSecounds -= 1000;
                            if(DateTime.Now > Mob.RemoveFloor)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    ActionQuery action;

                                    action = new ActionQuery()
                                    {
                                        ObjId = Mob.FloorPacket.m_UID,
                                        Type = ActionType.RemoveEntity
                                    };

                                    Mob.View.SendScreen(stream.ActionCreate(&action), Mob.GMap);



                                    Mob.GMap.View.LeaveMap<Role.IMapObj>(Mob);

                                    Mob.HitPoints = 0;
                                    client.Map.SetMonsterOnTile(Mob.X, Mob.Y, false);
                                }
                            }
                        }
                        else if (Mob.FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.AuroraLotus && DateTime.Now > Mob.FloorStampTimer)
                        {
                            Mob.FloorStampTimer = DateTime.Now.AddYears(1);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Mob.FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;
                                foreach (var user in Mob.View.Roles(client.Map, Role.MapObjectType.Player))
                                {
                                    if (user.Alive == false)
                                    {
                                        if (Role.Core.GetDistance(user.X, user.Y, Mob.X, Mob.Y) < 5)
                                        {
                                            var player = user as Role.Player;
                                            if (player.ContainFlag(MsgUpdate.Flags.SoulShackle) == false)
                                                player.Revive(stream);
                                        }
                                    }
                                }
                                ActionQuery action;
                                action = new ActionQuery()
                                {
                                    ObjId = Mob.FloorPacket.m_UID,
                                    Type = ActionType.RemoveEntity
                                };
                                Mob.View.SendScreen(stream.ActionCreate(&action), Mob.GMap);
                                Mob.GMap.View.LeaveMap<Role.IMapObj>(Mob);
                                Mob.HitPoints = 0;
                                client.Map.SetMonsterOnTile(Mob.X, Mob.Y, false);
                            }
                        }

                       
                        else if (Mob.FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.FlameLotus && DateTime.Now > Mob.FloorStampTimer)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                Mob.FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;
                                uint experience = 0;
                                foreach (var user in Mob.View.Roles(client.Map, Role.MapObjectType.Player))
                                {
                                    if (user.UID != Mob.OwnerFloor.Player.UID && Role.Core.GetDistance(user.X, user.Y, Mob.X, Mob.Y) < 5)
                                    {
                                        var player = user as Role.Player;

                                        if (MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(Mob.OwnerFloor, player, null))
                                        {
                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnPlayer(Mob.OwnerFloor.Player, player, Mob.DBSpell, out AnimationObj);
                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, Mob.OwnerFloor, player);
                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);


                                            InteractQuery Attack = new InteractQuery();
                                            Attack.UID = Mob.UID;
                                            Attack.OpponentUID = player.UID;
                                            Attack.Damage = (int)AnimationObj.Damage;
                                            Attack.Effect = AnimationObj.Effect;
                                            Attack.X = player.X;
                                            Attack.Y = player.Y;
                                            Attack.AtkType = MsgAttackPacket.AttackID.Physical;

                                            stream.InteractionCreate(&Attack);

                                            player.View.SendView(stream, true);

                                        }
                                    }
                                    user.Send(Mob.GetArray(stream, false));
                                }
                                foreach (var obj in Mob.OwnerFloor.Player.View.Roles(Role.MapObjectType.Monster))
                                {
                                    if (Role.Core.GetDistance(obj.X, obj.Y, Mob.X, Mob.Y) < 5)
                                    {
                                        var monster = obj as Game.MsgMonster.MonsterRole;
                                        if (monster.UID != Mob.UID)
                                        {
                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnMonster(Mob.OwnerFloor.Player, monster, Mob.DBSpell, out AnimationObj);
                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, Mob.OwnerFloor, monster);
                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                                            experience += AnimationObj.Damage;

                                            InteractQuery Attack = new InteractQuery();
                                            Attack.UID = Mob.UID;
                                            Attack.OpponentUID = Mob.UID;
                                            Attack.Damage = (int)AnimationObj.Damage;
                                            Attack.Effect = AnimationObj.Effect;
                                            Attack.X = Mob.X;
                                            Attack.Y = Mob.Y;
                                            Attack.AtkType = MsgAttackPacket.AttackID.Physical;

                                            stream.InteractionCreate(&Attack);


                                            monster.View.SendScreen(stream, client.Map);

                                        }
                                    }

                                }


                                var DBSpells = Database.Server.Magic[(ushort)Role.Flags.SpellID.FlameLotus];
                                MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, Mob.OwnerFloor, experience);
                                MsgServer.AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, Mob.OwnerFloor, new InteractQuery()
                                {
                                    SpellID = (ushort)Role.Flags.SpellID.FlameLotus
                                }, experience, DBSpells);

                                Mob.FloorPacket.DropType = MsgFloorItem.MsgDropID.Remove;
                                Mob.View.SendScreen(stream.ItemPacketCreate(Mob.FloorPacket), Mob.GMap);


                                ActionQuery action;

                                action = new ActionQuery()
                                {
                                    ObjId = Mob.FloorPacket.m_UID,
                                    Type = ActionType.RemoveEntity
                                };
                                unsafe
                                {
                                    Mob.View.SendScreen(stream.ActionCreate(&action), Mob.GMap);

                                }

                                //   Mob.FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;
                                // Mob.View.SendScreen(stream.ItemPacketCreate(Mob.FloorPacket), Mob.GMap);
                                Mob.GMap.View.LeaveMap<Role.IMapObj>(Mob);
                                Mob.HitPoints = 0;
                                //client.Map.MonstersColletion.Roles.Remove(Mob.UID);
                                client.Map.SetMonsterOnTile(Mob.X, Mob.Y, false);
                            }
                        }

                        continue;
                    }

                    if (Mob.BlackSpot)
                    {
                        if (timer > Mob.Stamp_BlackSpot)
                        {
                            Mob.BlackSpot = false;

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                Mob.Send(stream.BlackspotCreate(false, Mob.UID));
                            }

                        }
                    }
                    foreach (var flag in Mob.BitVector.GetFlags())
                    {

                        if (flag.Expire(timer))
                        {
                            Mob.RemoveFlag((MsgServer.MsgUpdate.Flags)flag.Key, client.Map);
                        }
                        else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.ScarofEarth)
                        {

                            if (flag.CheckInvoke(timer))
                            {


                                if (Mob.ScarofEarthl != null && Mob.AttackerScarofEarthl != null)
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        var DBSpell = Mob.ScarofEarthl;
                                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(    Mob.UID, 0, Mob.X, Mob.Y, DBSpell.ID, DBSpell.Level, 0, 1);
                                        MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj()
                                        {
                                            UID = Mob.UID,
                                            Damage = (uint)DBSpell.Damage2,
                                            Hit = 1
                                        };

                                        Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, Mob.AttackerScarofEarthl, Mob);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.Send(client);
                                    }
                                }
                            }
                        }
                        else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Poisoned)
                        {
                            if (flag.CheckInvoke(timer))
                            {
                                uint damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculatePoisonDamage(Mob.HitPoints, Mob.PoisonLevel);

                                if (Mob.HitPoints == 1)
                                {
                                    damage = 0;
                                    goto jump;
                                }

                                Mob.HitPoints = (uint)Math.Max(1, (int)(Mob.HitPoints - damage));

                            jump:

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    InteractQuery action = new InteractQuery()
                                    {
                                        Damage = (int)damage,
                                        AtkType = MsgAttackPacket.AttackID.Physical,
                                        X = Mob.X,
                                        Y = Mob.Y,
                                        OpponentUID = Mob.UID
                                    };

                                    Mob.Send(stream.InteractionCreate(&action));

                                }
                            }
                        }
                    }
                }



            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
        public static void GuardsCallback(Client.GameClient client, Extensions.Time32 clock)
        {
            try
            {
                if (client.Map == null)
                    return;
                Extensions.Time32 timer = Extensions.Time32.Now;

                var Array = client.Player.View.Roles(Role.MapObjectType.Monster);
                foreach (var map_mob in Array)
                {

                    var Guard = (map_mob as MonsterRole);
                    if ((Guard.Family.Settings & MonsterSettings.Guard) == MonsterSettings.Guard)
                    {
                        if (timer > Guard.AttackSpeed.AddMilliseconds(Guard.Family.AttackSpeed))
                        {
                            client.Player.View.MobActions.CheckGuardPosition(client.Player.View.GetPlayer(), Guard);
                            if (client.Player.View.MobActions.GuardAttackPlayer(client.Player.View.GetPlayer(), Guard))
                                Guard.AttackSpeed = timer;

                            if (!Guard.Alive)
                            {
                                Guard.AddFadeAway(timer.AllMilliseconds, client.Map);
                                Guard.RemoveView(timer.AllMilliseconds, client.Map);

                            }
                            foreach (var mob in Array)
                            {
                                var monseter = (mob as MonsterRole);
                                if ((monseter.Family.Settings & MonsterSettings.Guard) != MonsterSettings.Guard
                                    && (monseter.Family.Settings & MonsterSettings.Reviver) != MonsterSettings.Reviver
                                    && !monseter.IsFloor)
                                {
                                    if (client.Player.View.MobActions.GuardAttackMonster(client.Map, monseter, Guard))
                                        break;
                                }
                            }
                        }
                    }
                }



            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
        public static void AliveMonstersCallback(Client.GameClient client, Extensions.Time32 clock)
        {
            try
            {
                if (client.Map == null)
                    return;

                Extensions.Time32 timer = Extensions.Time32.Now;
                bool dedupe = MonsterAiMode >= 1;
                int tick = MapGroupThread.AiTick;
                var Array = client.Player.View.Roles(Role.MapObjectType.Monster);

                foreach (var map_mob in Array)
                {

                    var monster = (map_mob as MonsterRole);
                    if (dedupe)
                    {
                        if (monster.LastAiTick == tick)
                            continue;
                        monster.LastAiTick = tick;
                    }
                    if (!map_mob.Alive)
                    {
                        if (monster.State == Game.MsgMonster.MobStatus.Respawning)
                        {
                            if (MonsterRole.SpecialMonsters.Contains(monster.Family.ID))
                                continue;
                            if (timer > monster.RespawnStamp)
                            {
                                if (!client.Map.MonsterOnTile(monster.RespawnX, monster.RespawnY))
                                {
                                    monster.Respawn();
                                    client.Map.SetMonsterOnTile(monster.X, monster.Y, true);
                                }
                            }
                        }
                    }
                    if ((monster.Family.Settings & MonsterSettings.Guard) != MonsterSettings.Guard
                        && (monster.Family.Settings & MonsterSettings.Reviver) != MonsterSettings.Reviver
                        && (monster.Family.Settings & MonsterSettings.Lottus) != MonsterSettings.Lottus)
                    {
                        var Mob = map_mob as MonsterRole;
                        if (Mob.Family.ID == 20211)
                            continue;
                        client.Player.View.MobActions.ExecuteAction(client.Player.View.GetPlayer(), Mob);
                        // An idle monster that found no target from this viewer may still aggro a later viewer
                        // (CheckTarget is gated on the viewer's distance): let the next viewer try it this tick.
                        if (dedupe && Mob.Alive && Mob.State == MobStatus.Idle && Mob.Target == null)
                            Mob.LastAiTick = tick - 1;
                        if (!Mob.Alive)
                        {
                            var now = Extensions.Time32.Now;
                            Mob.AddFadeAway(now.AllMilliseconds, client.Map);
                            Mob.RemoveView(now.AllMilliseconds, client.Map);

                        }
                    }
                }

            }

            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
        private unsafe static void ReviversCallback(Client.GameClient client)
        {
            try
            {
                if (client.Map == null)
                    return;

                var Array = client.Player.View.Roles(Role.MapObjectType.Monster);
                foreach (var map_mob in Array)
                {
                    var monseter = (map_mob as MonsterRole);
                    if ((monseter.Family.Settings & MonsterSettings.Reviver) == MonsterSettings.Reviver)
                    {
                        if (!monseter.Alive)
                        {
                            var now = Extensions.Time32.Now.AllMilliseconds;
                            monseter.AddFadeAway(now, client.Map);
                            monseter.RemoveView(now, client.Map);
                        }
                        if (Role.Core.GetDistance(map_mob.X, map_mob.Y, client.Player.View.GetPlayer().X, client.Player.View.GetPlayer().Y) < 13)
                        {
                            if (!client.Player.View.GetPlayer().Alive)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.View.GetPlayer().Revive(stream);
                                }
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(map_mob.UID
                                    , 0, map_mob.X, map_mob.Y, (ushort)Role.Flags.SpellID.Pray, 0, 0);
                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(client.Player.View.GetPlayer().UID, 0, MsgServer.MsgAttackPacket.AttackEffect.None));
                                    SpellPacket.SetStream(stream);
                                    SpellPacket.Send(map_mob as Game.MsgMonster.MonsterRole);
                                }
                            }
                        }
                    }
                }



            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
    }
}
