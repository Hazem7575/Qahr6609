using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Database;

namespace Conquer6609.Game.MsgMonster
{
    public unsafe partial class MonsterRole
    {
        /// <summary>Same flags as the boss block of Dead() (only these families have one).</summary>
        static void SetBossRespawnFlag(uint familyId)
        {
            switch (familyId)
            {
                case 20300: BossHandler.Nemesis = true; break;
                case 20160: BossHandler.ThrillingSpook = true; break;
                case 20070: BossHandler.SnowBanshe = true; break;
                case 20060: BossHandler.TeratoDragon = true; break;
                case 29370: BossHandler.BloodyBanshee = true; break;
                case 29360: BossHandler.ChillingSpook = true; break;
                case 29300: BossHandler.NetherTyrant = true; break;
                case 29363: BossHandler.DragonWraith = true; break;
                case 20170: BossHandler.SnowBansheeSoul = true; break;
                case 20061: BossHandler.QueenOfEvil = true; break;
                case 20063: BossHandler.Delhab = true; break;
                case 20064: BossHandler.DarkWizard = true; break;
            }
        }
        private void DeadCore(ServerSockets.Packet stream, Client.GameClient killer, uint aUID, Role.GameMap GameMap)
        {
            if (Alive)
            {
              
                if (IsFloor)
                {

                    FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;
                    if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.Thundercloud)
                    {
                        ActionQuery _action;

                        _action = new ActionQuery()
                        {
                            ObjId = this.FloorPacket.m_UID,
                            Type = ActionType.RemoveEntity
                        };

                        this.View.SendScreen(stream.ActionCreate(&_action), this.GMap);


                        GMap.View.LeaveMap<Role.IMapObj>(this);
                        lock (HpSync) HitPoints = 0;
                        GameMap.SetMonsterOnTile(X, Y, false);
                    }
                    else if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.AuroraLotus)
                    {
                        // Same revive rules as before (a revive budget per monster in the killer's view, players in
                        // the same order), but each low-HP floor is removed once, every visited player gets the
                        // lotus packet once (was once per monster x player), a non-floor monster with < 25 HP no
                        // longer crashes on its null FloorPacket, and the lotus leaves the map once.
                        var users = new List<Role.IMapObj>(View.Roles(GameMap, Role.MapObjectType.Player));
                        int visited = 0;
                        if (killer != null && killer.Player != null && killer.Map != null)
                        {
                            foreach (var map_mob in killer.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                var Mob = (map_mob as MonsterRole);
                                if (Mob == null)
                                    continue;
                                byte revivers = SpellLevel >= 6 ? (byte)2 : (byte)1;
                                if (users.Count > 0 && Mob.HitPoints < 25 && Mob.FloorPacket != null)
                                {
                                    ActionQuery action1;

                                    action1 = new ActionQuery()
                                    {
                                        ObjId = Mob.FloorPacket.m_UID,
                                        Type = ActionType.RemoveEntity
                                    };

                                    Mob.View.SendScreen(stream.ActionCreate(&action1), Mob.GMap);
                                    Mob.HitPoints = 0;
                                    Mob.FloorStampTimer = DateTime.Now;
                                    killer.Map.SetMonsterOnTile(Mob.X, Mob.Y, false);
                                }
                                int i = 0;
                                for (; i < users.Count; i++)
                                {
                                    if (revivers == 0)
                                        break;
                                    var user = users[i];
                                    if (user.Alive == false)
                                    {
                                        if (Role.Core.GetDistance(user.X, user.Y, X, Y) < 5)
                                        {
                                            revivers--;
                                            var player = user as Role.Player;
                                            if (player.ContainFlag(MsgUpdate.Flags.SoulShackle) == false)
                                                player.Revive(stream);

                                        }
                                    }
                                }
                                if (i > visited)
                                    visited = i;
                            }
                        }
                        for (int i = 0; i < visited; i++)
                            users[i].Send(GetArray(stream, false));
                        GMap.View.LeaveMap<Role.IMapObj>(this);
                    }
                    else if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.FlameLotus)
                    {
                        FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;

                        foreach (var user in View.Roles(GameMap, Role.MapObjectType.Player))
                        {
                            if (user.UID != OwnerFloor.Player.UID && Role.Core.GetDistance(user.X, user.Y, this.X, this.Y) < 5)
                            {
                                var player = user as Role.Player;

                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                Game.MsgServer.AttackHandler.Calculate.Magic.OnPlayer(this.OwnerFloor.Player, player, this.DBSpell, out AnimationObj);
                                Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, this.OwnerFloor, player);
                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);

                                InteractQuery Attack = new InteractQuery();
                                Attack.UID = this.UID;
                                Attack.OpponentUID = player.UID;
                                Attack.Damage = (int)AnimationObj.Damage;
                                Attack.Effect = AnimationObj.Effect;
                                Attack.X = player.X;
                                Attack.Y = player.Y;
                                Attack.AtkType = MsgAttackPacket.AttackID.Physical;

                                stream.InteractionCreate(&Attack);

                                player.View.SendView(stream, true);

                            }
                            user.Send(this.GetArray(stream, false));
                        }
                        foreach (var obj in View.Roles(GameMap, Role.MapObjectType.Monster))
                        {
                            if (obj.UID != this.UID && Role.Core.GetDistance(obj.X, obj.Y, this.X, this.Y) < 5)
                            {
                                var monster = obj as Game.MsgMonster.MonsterRole;

                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                Game.MsgServer.AttackHandler.Calculate.Magic.OnMonster(this.OwnerFloor.Player, monster, this.DBSpell, out AnimationObj);
                                Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, monster.OwnerFloor, monster);
                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);

                                InteractQuery Attack = new InteractQuery();
                                Attack.UID = this.UID;
                                Attack.OpponentUID = this.UID;
                                Attack.Damage = (int)AnimationObj.Damage;
                                Attack.Effect = AnimationObj.Effect;
                                Attack.X = this.X;
                                Attack.Y = this.Y;
                                Attack.AtkType = MsgAttackPacket.AttackID.Physical;

                                stream.InteractionCreate(&Attack);


                                monster.View.SendScreen(stream, this.OwnerFloor.Map);

                            }

                        }
                        GMap.View.LeaveMap<Role.IMapObj>(this);
                    }
                    lock (HpSync) HitPoints = 0;
                    GameMap.SetMonsterOnTile(X, Y, false);
                    return;
                }


                RespawnStamp = Extensions.Time32.Now.AddSeconds(8 + Family.RespawnTime);

                if (BlackSpot)
                {
                    Send(stream.BlackspotCreate(false, UID));
                    BlackSpot = false;
                }
                ClearFlags(false);
                lock (HpSync) HitPoints = 0;// same lock as ReceiveAttack.Monster's kill-or-subtract
                AddFlag(MsgServer.MsgUpdate.Flags.Dead, Role.StatusFlagsBigVector32.PermanentFlag, true);
                DeadStamp = Extensions.Time32.Now;
                if (killer != null) Conquer6609.Framework.Modules.ModuleManager.MonsterKilled(killer, this);

                InteractQuery action = new InteractQuery()
                {
                    UID = aUID,
                    KilledMonster = true,
                    X = this.X,
                    Y = this.Y,
                    AtkType = MsgAttackPacket.AttackID.Death,
                    OpponentUID = UID
                };



                // DropMob.Checkup dropping something used to `return` here, which also skipped the XP/kill
                // counters, the death packet, the RemoveOnDead cleanup and the boss respawn flag. The drops that
                // follow are still skipped (same drop rates); the common tail now runs.
                bool checkupDropped = false;
                if (killer != null)
                {
                    if (DropMob.Checkup(stream, killer, this))
                    {
                        checkupDropped = true;
                        if (Boss > 0)
                            SetBossRespawnFlag(Family.ID);
                        goto jmp;
                    }
                    #region Boss
                    if (Boss > 0 && (Family.ID == 20070 || Family.ID == 29300 || Family.ID == 29370 || Family.ID == 20300 || Family.ID == 20160 || Family.ID == 20060 || Family.ID == 29360 || Family.ID == 29363 || Family.ID == 20170 || Family.ID == 20061 || Family.ID == 20063 || Family.ID == 20064))//Snow Banshee & Spook & Nemsis
                    {
                        #region Respawin
                        switch (Family.ID)
                        {
                            case 20300: BossHandler.Nemesis = true; break;
                            case 20160: BossHandler.ThrillingSpook = true; break;
                            case 20070: BossHandler.SnowBanshe = true; break;
                            case 20060: BossHandler.TeratoDragon = true; break;
                            case 29370: BossHandler.BloodyBanshee = true; break;
                            case 29360: BossHandler.ChillingSpook = true; break;
                            case 29300: BossHandler.NetherTyrant = true; break;
                            case 29363: BossHandler.DragonWraith = true; break;
                            case 20170: BossHandler.SnowBansheeSoul = true; break;
                            case 20061: BossHandler.QueenOfEvil = true; break;
                            case 20063: BossHandler.Delhab = true; break;
                            case 20064: BossHandler.DarkWizard = true; break;
                        }
                        #endregion
                        #region Rank
                        // a module that runs this map's bosses (BossHandler.ModuleOwnedMaps, e.g. Modules\DragonIsland) pays its own rewards
                        if (!BossHandler.ModuleOwnedMaps.ContainsKey(killer.Player.Map))
                            GetBossReward(killer, stream);//جوائز الرنكات
                        if (killer.Player.Map == 10137 || killer.Player.Map == 10166)
                        {
                            //var array = this.Hunters.Values.OrderBy(p => p.HunterScore).Take(3).ToArray();
                            //for (int x = 0, y = 3; x < array.Length; x++, y--)
                            //{
                            //    if (x > array.Length)
                            //        break;
                            //    Database.Server.GamePoll[array[x].HunterUID].Player.ConquerPoints += (uint)(y * 25000);
                            //}
                            uint Score = this.Hunters.ContainsKey(killer.Player.UID) == true ? this.Hunters[killer.Player.UID].HunterScore : 0;
                            MsgTournaments.MsgSchedules.DragonIsland.SortKiller(killer, Score, MsgTournaments.ConquererMode.BansheeKiller);
                   
                            this.Hunters.Clear();
                        }
                        #endregion
                       
                        const ushort GetStudyPoints = 0;
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(0);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    killer.HeroRewards.AddGoal(709);
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        #region random Grament
                        Database.ItemType.DBItem aitem;
                        aitem = PickDropGarment(4);
                        if (aitem != null)
                        {
                            {

                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = aitem.ID;
                                    DataItem.EndDate = DateTime.Now.AddDays(2);
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(Database.ItemType.DragonBall, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region random Mount
                        Database.ItemType.DBItem Mount_item;
                        Mount_item = PickDropMount(4);
                        if (Mount_item != null)
                        {
                            {

                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = Mount_item.ID;
                                    DataItem.EndDate = DateTime.Now.AddDays(2);
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(Database.ItemType.DragonBall, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        if (killer.Player.DynamicID != killer.Player.UID)
                        {
                            for (int x = 0; x < 7; x++)
                            {
                                uint id = Database.ItemType.DragonBall;
                                if (x == 0)
                                    id = 3004181;
                                if (x == 1)
                                    id = 3004181;
                                if (x == 2)
                                    id = 3004181;
                                if (x == 3)
                                    id = 3004181;
                                if (x == 4)
                                    id = 723694;
                                if (x == 5)
                                    id = 3004181;
                                if (x == 6)
                                    id = 3004181;
                                if (x == 7)
                                    id = 3004181;
                                if (x == 8)
                                    id = 3004181;
                                if (x == 9)
                                    id = 3004181;
                                if (x == 10)
                                    id = 3004181;
                                if (x == 11)
                                    id = 723694;
                                if (x == 12)
                                    id = 3004181;
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        SendBossSysMesage(killer.Player.Name, GetStudyPoints, MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        if (killer.Player.Map == 10137)
                        {
                            SendSysMesage(killer.Player.Name + " has defeated [" + Name + "].", MsgServer.MsgMessage.ChatMode.SlideCrosTheServer, MsgServer.MsgMessage.MsgColor.white);
                        }
                        else if (killer.Player.Map == 10166)
                        {
                            SendSysMesage(killer.Player.Name + " has defeated [" + Name + "].", MsgServer.MsgMessage.ChatMode.SlideCrosTheServer, MsgServer.MsgMessage.MsgColor.white);
                        }
                        return;
                    }
                    #endregion
                    #region Server Drob
                    if (killer.Player.VipLevel > 6)
                    {
                        uint Value = (uint)Program.GetRandom.Next(0, 6);
                        //killer.Player.BoundConquerPoints += (int)Value;
                        killer.Player.ConquerPoints += Value;
                        killer.SendSysMesage("You have picked up a  " + Value + " CPS.");
                    }

                    else
                    {
                        uint Value = (uint)Program.GetRandom.Next(0, 4);
                        //killer.Player.BoundConquerPoints += (int)Value;
                        killer.Player.ConquerPoints += (uint)Value;
                        killer.SendSysMesage("You have picked up a  " + Value + " CPS.");

                    }

                    #endregion
                    if (Boss > 0 && Family.ID == 20055)//Lava Beast
                    {
                        const ushort GetStudyPoints = 0;
                        BossHandler.LavaBeast.Add(new Conquer6609.Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm.coords(Family.LavaX, Family.LavaY) { });
                        BossHandler.NextLavaBeast = DateTime.Now.AddMinutes(3);
                        BossHandler.LavaBeastsCount++;
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(3);
                        if (Role.Core.Rate(30))
                            DropIems.Add(711679);
                        if (Role.Core.Rate(30))
                            DropIems.Add(711188);


                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        SendBossSysMesage(killer.Player.Name, GetStudyPoints, MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);


                        return;
                    }

                    


                    if (killer.Player.Map == 10137)//New Dragon Island Points Collectors
                    {
                        if (Boss != 1)
                        {
                            killer.SendSysMesage("Dragon~Island~Points~Collected.");
                            killer.Player.DragonIslandCollectedPoints++;
                            MsgTournaments.MsgSchedules.DragonIsland.SortKiller(killer, killer.Player.DragonIslandCollectedPoints, MsgTournaments.ConquererMode.MobsKiller);
                        }
                    }
                    //Fiend's lair
                    if (killer.Player.Map == 8822)
                    {
                        if (killer.Player.AukKilled < 30)
                            killer.Player.AukKilled++;
                    }
                    //
                    //MoonBoxQuest
                    if (killer.Player.Map == 1047)
                    {
                        if (killer.Inventory.Contain(721014, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Disturbed~Token!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1044, 12, 63, 600007)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 5;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 4)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721014); //it's item for DOP
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721014);
                                    }
                                    killer.Player.MessageBox("You~received~a~Disturbed~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721014, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1045)
                    {
                        if (killer.Inventory.Contain(721012, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Deserted~Token!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1045, 12, 63, 600009)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 10;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 4)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721012);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721012);
                                    }
                                    killer.Player.MessageBox("You~received~a~Deserted~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721012, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1043)
                    {
                        if (killer.Inventory.Contain(721010, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Peace~Token!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1043, 12, 63, 600005)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 100;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 4)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721010);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721010);
                                    }
                                    killer.Player.MessageBox("You~received~a~Peace~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721010, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1048)
                    {
                        if (killer.Inventory.Contain(721015, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Peace~Calm!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1048, 12, 63, 600015)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 10;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 4)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721015);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721015);
                                    }
                                    killer.Player.MessageBox("You~received~a~Calm~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721015, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1044)
                    {
                        if (killer.Inventory.Contain(721013, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Peace~Chaos!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1044, 12, 63, 600007)), null);
                        }
                        else
                        {
                            double chance = killer.Player.KillCounter / 10;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 4)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721013);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721013);
                                    }
                                    killer.Player.MessageBox("You~received~a~Chaos~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721013, stream, 3, 59);
                                }
                            }
                        }
                    }
                    if (Family.ID == 0015)
                    {
                        double chance = killer.Player.KillCounter / 10;
                        if (MyMath.Success(chance))
                        {
                            DropItemID(killer, 1080001, stream);
                        }
                    }
                    if (killer.Player.Map == 1046)
                    {
                        if (killer.Inventory.Contain(721013, 1))
                        {
                            killer.Player.MessageBox("You~received~a~Peace~Prosperous!~Head~to~see~the~Ghost~for~more~details!", p => p.Send(stream.PathFinding(p.Player.UID, 1046, 12, 63, 600011)), null);
                        }
                        else
                        {

                            double chance = killer.Player.KillCounter / 10;
                            if (MyMath.Success(chance))
                            {
                                if (killer.Player.VipLevel >= 4)
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721013);
                                    }
                                    else
                                    {
                                        killer.Inventory.AddReturnedItem(stream, 721013);
                                    }
                                    killer.Player.MessageBox("You~received~a~Prosperous~Token!", null, null);
                                }
                                else
                                {
                                    DropItemID(killer, 721013, stream, 3, 59);
                                }
                            }
                        }
                    }
                    ///
                    if (killer.Player.Map == 10166)
                    {
                        if (!killer.Player.GiantGhasomKillClaimed && killer.Player.GiantGhasomKill < 300)
                        {
                            killer.Player.GiantGhasomKill++;
                            killer.SendSysMesage("Gained~a~ghasom~point.");
                        }
                    }
                    if (killer.Player.Map == 10166 && killer.Player.HuntingBounsLayer > 1)
                    {
                        var done = Program.GetRandom.Next(1, 26);
                        if (done == 1)
                        {
                            DropItemID(killer, Database.ItemType.Meteor, stream);
                            killer.SendSysMesage("Meteor~dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 5 && killer.Player.HuntingBounsLayer > 2)
                        {
                            DropItemID(killer, Database.ItemType.MeteorScroll, stream);
                            killer.SendSysMesage("Meteor~Scroll~dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 25)
                        {
                            if (killer.Player.VipLevel > 2)
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 3303481, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 0, 59);
                                    killer.SendSysMesage("You~recieved~1~TemporaryPenitenceAmulet.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                }
                            }
                        }
                        if (done == 20 && killer.Player.HuntingBounsLayer > 4)
                        {
                            DropItemID(killer, 3303974, stream, 3, 59);
                            killer.SendSysMesage("EpicChestKey~Dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 29 && killer.Player.HuntingBounsLayer > 3)
                        {
                            DropItemID(killer, 3303973, stream, 3, 59);
                            killer.SendSysMesage("RareChestKey~Dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 7)
                        {
                            DropItemID(killer, 729252, stream, 3, 59);
                            killer.SendSysMesage("Vigor~Pill~dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 8)
                        {
                            //3006536
                            DropItemID(killer, 3006536, stream, 3, 59);
                            killer.SendSysMesage("StudyPack(50pts)~Dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 10)
                        {
                            DropItemID(killer, 3007106, stream, 3, 59);
                            killer.SendSysMesage("WeaponAccessoryFragment~Dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 11)
                        {
                            //3006536
                            DropItemID(killer, 3007106, stream, 3, 59);
                            killer.SendSysMesage("ChiPack(50pts)~Dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 9)
                        {
                            DropItemID(killer, 3303975, stream, 3, 59);
                            killer.SendSysMesage("ChasmDBBag~Dropped.");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        if (done == 3 && killer.Player.HuntingBounsLayer > 4)
                        {
                            if (Family.ID == 23974 || Family.ID == 22973 || Family.ID == 23973)
                            {
                                killer.Inventory.AddItemWitchStack(3303483, 0, 1, stream);
                                killer.SendSysMesage("You~recieved~1~ChaosBead.");
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            }
                        }
                        if (done == 17 && killer.Player.HuntingBounsLayer > 7)
                        {
                            if (Family.ID == 23974 || Family.ID == 22973 || Family.ID == 23973)
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 3303484, 1);
                                    killer.SendSysMesage("You~recieved~1~ChaosBeadPack.");
                                }
                                else
                                {
                                    killer.Inventory.AddReturnedItem(stream, 3303484, 1);
                                    killer.SendSysMesage("You~recieved~1~ChaosBeadPack.");
                                }
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            }
                        }
                    }
                    if (killer.Player.Map == 44461)
                    {
                        if (Family.ID == 452373)
                        {
                            killer.CreateBoxDialog("Duke of Hell has been enraged and revealed his true appearance. Hurry and defeat him to prevent the imperial mausoleum from destruction.");
                            killer.Teleport(killer.Player.X, killer.Player.Y, 44462, Database.Server.ServerMaps[44462].GenerateDynamicID());
                            Database.Server.AddMapMonster(stream, killer.Map, 452374, 336, 193, 2, 2, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        }
                    }
                    else if (killer.Player.Map == 44462)
                    {
                        killer.Player.MessageBox("You`ve finally knocked back the Duke of Hell. However, the fierce fights have also triggered the emergency switch and the imperial mausoleum will soon collapse. Run!"
                            , new Action<Client.GameClient>(p => p.Player.QuestGUI.SendAutoPatcher(10090, 58, 164, 0)), null);

                        killer.Teleport(killer.Player.X, killer.Player.Y, 44463, Database.Server.ServerMaps[44463].GenerateDynamicID());
                    }
                    else if (killer.Player.Map == 44456)
                    {
                        if (Family.ID == 657218)
                        {
                            Database.Server.AddMapMonster(stream, killer.Map, 657219, killer.Player.X, killer.Player.Y, 6, 6, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                        }
                        if (killer.Player.View.Roles(Role.MapObjectType.Monster).Count() <= 1)
                        {
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 3799, 1);
                            killer.Player.QuestGUI.SendAutoPatcher("You`ve successfully driven off the pursuing devils. Hurry and report back to the Elder in the hall.", 10088, 123, 119, 200058);

                        }
                    }
                    else if (killer.Player.Map == 1791)
                    {
                        if (Family.ID == 2172)
                        {
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 7, 7, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 6, 6, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 5, 5, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 4, 4, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            Database.Server.AddMapMonster(stream, killer.Map, 2173, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                            killer.SendSysMesage("Some Lyra Ghosts are coming! Watch out!");
                        }
                    }
                    else if (killer.Player.Map == 1792)
                    {
                        if (Family.ID == 2170)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(407, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 710216, 1);
                                    killer.SendSysMesage("You`ve killed the Crimson Viper and obtained a Purple Eye. Now go back to Maple Forest (788,456) to show Villager Chou.");
                                }
                                else
                                {
                                    killer.SendSysMesage("There`s not enough space in your inventory.");
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1794)
                    {
                        if (Family.ID == 2168)//brigard
                        {
                            if (killer.Player.QuestGUI.CheckQuest(404, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (!killer.Inventory.Contain(721806, 1))
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.Add(stream, 721806, 1);
                                            killer.SendSysMesage("The Brigands dropped a Bee Kettle! Use the kettle to leave here!");
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("There`s not enough space in your inventory for the Bee Kettle!");
                                        }
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2167)//viper
                        {
                            if (killer.Player.QuestGUI.CheckQuest(404, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (!killer.Inventory.Contain(721803, 1))
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.Add(stream, 721803, 1);
                                            killer.SendSysMesage("You obtained a Poison Fang and got poisoned.");
                                            killer.SendSysMesage("Some Brigands are coming! Look out!");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2168, killer.Player.X, killer.Player.Y, 7, 7, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2168, killer.Player.X, killer.Player.Y, 6, 6, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2168, killer.Player.X, killer.Player.Y, 6, 6, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2168, killer.Player.X, killer.Player.Y, 5, 5, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "movego");

                                        }
                                    }
                                }
                            }

                        }
                    }
                    else if (killer.Player.Map == 1787)
                    {
                        if (Family.ID == 2160)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Inventory.Contain(721777, 3))
                                {
                                    if (killer.Inventory.HaveSpace(3))
                                    {
                                        killer.Inventory.Add(stream, 721777, 3);
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 0, 0, 0, 0, 1);
                                        killer.CreateBoxDialog("Nothing~but~these~three~Tower~Splinters~will~do.~Take~them.~Maybe~you`ll~get~something~from~them~at~the~Cauldron~(48,37).");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.~Make~some~room~first,~and~come~back~for~the~Tower~Splinter.");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2163)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Map.ContainMobID(2160))
                                {
                                    Database.Server.AddMapMonster(stream, killer.Map, 2160, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                }
                                killer.CreateBoxDialog("The~ghost~of~Yang~Feng~has~appeared~at~(86,78).~Hurry~up~and~see~what`s~going~on~there!");
                            }
                        }
                    }
                    else if (killer.Player.Map == 1786)
                    {
                        if (Family.ID == 2159)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721789, 1))
                                        {
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 0, 0, 1);
                                            killer.Inventory.Add(stream, 721789, 1);
                                            killer.CreateBoxDialog("You~received~the~Wasp~Spear.");
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full!");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2178)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(721790, 1))
                                    {

                                        killer.Inventory.Add(stream, 721790, 1);
                                        killer.CreateBoxDialog("You~received~the~Infernal~Axe.");
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 0, 0, 0, 1);
                                    }
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }
                        }
                    }
                    if (killer.Player.Map == 1785)
                    {
                        if (Family.ID == 2166)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(721788, 1))
                                    {
                                        killer.Inventory.Add(stream, 721788, 1);
                                        killer.CreateBoxDialog("You~obtained~the~Annatto~Blade.");
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 0, 1);
                                    }
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }
                        }
                        else if (Family.ID == 2161)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(527, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721784, 1))
                                        {
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 527, 1);
                                            killer.Inventory.Add(stream, 721784, 1);
                                            killer.CreateBoxDialog("You~obtained~the~Glitter~Sword.");
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full!");
                                    }
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1783)
                    {
                        if (Family.ID == 2158)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(526, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Player.QuestGUI.CheckObjectives(526, 20))
                                {
                                    killer.CreateBoxDialog("What~a~fight!~Congratulations,~you~can~make~your~escape~through~the~rift~now.");
                                }
                                else
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 526, 1);
                                    if (killer.Player.QuestGUI.CheckObjectives(526, 20))
                                    {
                                        killer.CreateBoxDialog("What~a~fight!~Congratulations,~you~can~make~your~escape~through~the~rift~now.");
                                    }
                                }
                            }

                        }
                    }
                    else if (killer.Player.Map == 1109)
                    {
                        if (Family.ID == 7696)
                        {
                            if (killer.Inventory.HaveSpace(1))
                            {
                                killer.Inventory.Add(stream, 3006392, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e280");
                                killer.CreateBoxDialog("You~successfully~defeated~the~Heaven~Roc,~and~received~Roc`s~Feather!~It`s~amazing!");
                                killer.Teleport(213, 200, 1011);
                            }
                            else
                            {
                                killer.SendSysMesage("Your~inventory~is~full.~Please~make~some~room~first,~and~then~come~back~for~the~challenge.");
                                killer.Teleport(213, 200, 1011);
                            }
                        }
                    }
                    else if (killer.Player.Map == 1351)
                    {
                        if (Family.ID == 3141)//slinger
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1841, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1841, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1841, 20))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1841);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");

                                    killer.CreateBoxDialog("You`ve~finished~the~first~step.~Now~you~may~commence~the~second~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1842);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1843, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1843, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1843, 15, 15))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1843);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~accomplished~the~third~step.~Commence~the~fourth~step~now.~Keep~up~with~the~good~work.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1844);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                        }
                        else if (Family.ID == 3142)//goldghost
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1843, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1843, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1843, 15, 15))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1843);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~accomplished~the~third~step.~Commence~the~fourth~step~now.~Keep~up~with~the~good~work.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1844);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1842, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1842, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1842, 10))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1842);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~killed~10~Gold~Ghosts.~Great!");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1843);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1352)//laibirint2
                    {
                        if (Family.ID == 3144)//blading
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1844, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1844, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1844, 2))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1844);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~finished~the~fourth~step.~Now~commence~the~fifth~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1845);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);

                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1846, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1846, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1846, 10, 10))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1846);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~finished~the~6th~step.~Now~commence~the~7th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1847);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                        }
                        else if (Family.ID == 3145)//agilerat
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1845, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(721926, 1))
                                {
                                    killer.CreateBoxDialog("You`ve~received~an~Agile~Rat~Fang~already.~You~cannot~take~any~more.");
                                }
                                else
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721926, 1);
                                        killer.CreateBoxDialog("You~received~an~Agile~Rat~Fang!");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.");
                                    }
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1846, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1846, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1846, 10, 10))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1846);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");
                                    killer.CreateBoxDialog("You`ve~finished~the~6th~step.~Now~commence~the~7th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1847);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1353)
                    {
                        if (Family.ID == 3147)//bluebird
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1847, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1847, 1);
                                killer.CreateBoxDialog("You`ve~killed~a~Blue~Bird.~Kill~another~one~in~20~seconds.");
                                if (killer.Player.QuestGUI.CheckObjectives(1847, 2))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1847);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");

                                    killer.CreateBoxDialog("You`ve~finished~the~7th~step.~Now~commence~the~8th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1848);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1849, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1849, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1849, 5, 5))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1849);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");

                                    killer.CreateBoxDialog("You`ve~finished~the~9th~step.~Now~commence~the~10th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1850);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(721931, 1))
                                {
                                    killer.CreateBoxDialog("You`ve~received~a~Blue~Bird~Plume~already.~You~cannot~take~any~more.");
                                }
                                else
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721931);
                                        killer.CreateBoxDialog("You~received~a~Blue~Bird~Plume.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 3148)//fiendbat
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1848, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(50))
                                {
                                    if (killer.Inventory.Contain(721928, 1))
                                    {
                                        killer.CreateBoxDialog("You`ve~received~a~Fiend~Bat~Wing.~You~cannot~take~any~more.");
                                    }
                                    else
                                    {
                                        if (killer.Inventory.Contain(721929, 1))
                                        {
                                            killer.CreateBoxDialog("You`ve~received~the~Roast~Fiend~Bat~Wing.~Eat~it~now!");
                                        }
                                        else
                                        {
                                            if (killer.Inventory.HaveSpace(1))
                                            {
                                                killer.Inventory.Add(stream, 721928);
                                                killer.CreateBoxDialog("You~received~a~Fiend~Bat~Wing.");
                                            }
                                            else
                                            {
                                                killer.CreateBoxDialog("Your~inventory~is~full.");
                                            }
                                        }
                                    }
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1849, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1849, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1849, 5, 5))
                                {
                                    killer.Player.QuestGUI.FinishQuest(1849);
                                    killer.Player.Money += 100;
                                    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                                    killer.SendSysMesage("You received 100 Silver!");

                                    killer.CreateBoxDialog("You`ve~finished~the~9th~step.~Now~commence~the~10th~step.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, killer.Player.Class, 1850);
                                    killer.Player.QuestGUI.Accept(ActiveQuest, 0);
                                }
                            }
                            else if (killer.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(721931, 1))
                                {
                                    killer.CreateBoxDialog("You`ve~received~a~Blue~Bird~Plume~already.~You~cannot~take~any~more.");
                                }
                                else
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 721931);
                                        killer.CreateBoxDialog("You~received~a~Blue~Bird~Plume.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.");
                                    }
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1354)
                    {
                        if (Family.ID == 3155)//minotaurlevel120
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.OnBluedBird = true;
                                killer.CreateBoxDialog("Use~the~Blue~Bird~Wing~in~3~seconds~to~ignite~it,~or~it~will~never~be~ignited.");

                                killer.Player.BlueBirdPlumeStamp = DateTime.Now.AddSeconds(3);
                            }
                        }
                    }
                    else if (killer.Player.Map == 1001)
                    {
                        if (Family.ID == 2232)//bullmaster
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1838, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(710260, 1))
                                {
                                    killer.CreateBoxDialog("You~received~the~Bull~heart!");
                                }
                                else
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 710260, 1);
                                        killer.CreateBoxDialog("You~killed~the~Bull~Master~and~received~its~heart.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                                    }
                                }
                            }
                        }
                        if (Family.ID == 2231)//madbull
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1837, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 102, 292) <= 3)
                                {

                                    killer.Player.AddMapEffect(stream, 102, 292, "zf2-e027");
                                    if (killer.Inventory.Contain(710261, 3))
                                    {
                                        killer.SendSysMesage("You`ve collected 3 Bull Ghost.");
                                    }
                                    else
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            if (Role.Core.Rate(1, 3))
                                            {
                                                killer.Inventory.Add(stream, 710261, 1);
                                                killer.SendSysMesage("You obtained a Bull Ghost.");
                                                if (killer.Inventory.Contain(710261, 3))
                                                {
                                                    killer.SendSysMesage("You`ve collected 3 Bull Ghost.");
                                                }
                                            }
                                            else
                                            {
                                                killer.SendSysMesage("Well, pay more attention next time. You failed in retrieving the Bull Ghost.");
                                            }
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("You inventory is full. Make some room and come back for the Bull Ghost.");
                                        }
                                    }
                                }
                                else
                                {
                                    killer.SendSysMesage("Only when you herd the Mad Bull to (102,292) before killing them can you capture its ghost.");
                                }
                            }
                        }
                        if (Family.ID == 57)//bullmonster
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1836, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Player.OnFerentPill)
                                {
                                    if (killer.Inventory.Contain(710259, 10))
                                    {
                                        killer.SendSysMesage("You`ve obtained 10 Burnt Bull Horns and put it in your inventory.");
                                    }
                                    else
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.AddItemWitchStack(710259, 0, 1, stream);
                                            if (killer.Inventory.Contain(710259, 10))
                                            {
                                                killer.SendSysMesage("You`ve obtained 10 Burnt Bull Horns and put it in your inventory.");
                                            }
                                            else
                                            {
                                                killer.SendSysMesage("You`ve obtained a Burnt Bull Horn.");
                                            }
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("Your inventory is full. Make some room and come back for the Burnt Bull Horn.");
                                        }
                                    }
                                }
                                else
                                    killer.SendSysMesage("Bring the Fervent Pill received from Master Mo Mo to go to the Mystic Castle. Use the pill first, and then kill the Bull Monsters to get 10 Burnt Bull Horns for Master Mo Mo.");
                            }
                        }
                        if (Family.ID == 2189)
                        {
                            if (!killer.Inventory.Contain(711518, 1))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 711518);
                                    killer.CreateBoxDialog("You~killed~the~Jinx~Tomb~Bat~and~received~a~Jinx~Tomb~Bat~Heart!");
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full.~Hurry~to~make~1~empty~space.");
                                }
                            }

                        }
                        if (Family.ID == 56)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1832, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721877, 5))
                                        {
                                            killer.Inventory.AddItemWitchStack(721877, 0, 1, stream);
                                            killer.SendSysMesage("You found the BatEssence from the Tomb Bats.");
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1832, 1);
                                        }
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("You~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                            }
                        }
                        if (Family.ID == 20)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1829, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721870, 1))
                                        {
                                            if (killer.Inventory.Contain(721876, 5))
                                            {
                                                killer.CreateBoxDialog("Sorry,~but~you`ve~already~had~5~Keys.");
                                            }
                                            else
                                            {
                                                killer.Inventory.AddItemWitchStack(721876, 0, 1, stream);
                                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1829, 1);
                                                killer.CreateBoxDialog("You~received~a~Key~from~the~Tomb~Bat.");
                                            }
                                        }
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("You~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                            }

                        }
                    }
                    else if (killer.Player.Map == 3833)
                    {
                        if (Family.ID == 7485)
                        {
                            killer.Player.StageEpicTrojanQuest = 51;
                            if (!killer.Map.ContainMobID(7486))
                            {
                                Database.Server.AddMapMonster(stream, killer.Map, 7486, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "ride_screen");
                            }

                            killer.SendSysMesage("The~earth~is~shaking,~and~the~Flame~Devastator~revived!");
                        }
                        else if (Family.ID == 7486)
                        {
                            killer.Player.StageEpicTrojanQuest = 52;
                            killer.Player.AddMapEffect(stream, 45, 45, "xidag_bafi");
                            if (!killer.Map.ContainMobID(7487))
                            {
                                Database.Server.AddMapMonster(stream, killer.Map, 7487, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "xidag_bafi");
                            }
                        }
                        else if (Family.ID == 7487)
                        {

                            killer.CreateBoxDialog("Holding~the~Epic~Weapon~infused~with~the~blade~spirit,~you~beheaded~the~Flame~Devastator~and~burned~it~into~ashes.");
                            killer.Teleport(154, 132, 3832);
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "role-select1");
                            bool finish = false;
                            foreach (var item in killer.Inventory.ClientItems.Values)
                            {
                                if (item.IsWeapon && !Database.ItemType.IsTwoHand(item.ITEM_ID) && !Database.ItemType.IsTrojanEpicWeapon(item.ITEM_ID))
                                {
                                    uint UpdateToEpic = (item.ITEM_ID % 1000) + 614000;
                                    item.ITEM_ID = UpdateToEpic;
                                    item.Mode = Role.Flags.ItemMode.Update;
                                    item.Send(killer, stream);
                                    finish = true;
                                    break;
                                }
                            }
                            if (finish == false)
                            {
                                foreach (var item in killer.Equipment.ClientItems.Values)
                                {
                                    if (item.IsWeapon && !Database.ItemType.IsTwoHand(item.ITEM_ID) && !Database.ItemType.IsTrojanEpicWeapon(item.ITEM_ID))
                                    {
                                        uint UpdateToEpic = (item.ITEM_ID % 1000) + 614000;
                                        item.ITEM_ID = UpdateToEpic;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(killer, stream);
                                        finish = true;
                                        break;
                                    }
                                }
                            }
                            if (finish)
                            {
                                killer.Player.ResetEpicTrojan();


                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + "~successfully~prevented~Twin~City~from~an~olden~massacre,~and~obtained~an~Epic~Weapon!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                                killer.Inventory.Remove(3003340, 1, stream);
                            }
                        }
                    }
                    if (killer.Player.Map == 3831)
                    {
                        if (Family.ID == 7484)
                        {
                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MonkMisery, 0, 3272);
                            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap2, 0, 3273);
                            killer.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            killer.Player.StageEpicTrojanQuest = 40;
                            killer.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            killer.Player.QuestGUI.SendAutoPatcher("The~Evil~Monk~Misery`s~death~laugh~sounds~weird.~Something~may~happen~to~the~Flame~Devastator.~Go~report~to~General~Pak!", 3831, 154, 130, (uint)NpcID.GeneralPakMap2);
                        }
                    }
                    else if (killer.Player.Map == 3834)
                    {
                        if (Family.ID == 7483)
                        {


                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3271);
                            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3277);
                            killer.Player.StageEpicTrojanQuest = 30;
                            killer.Player.EpicTrijanKillGhostReaver = 1;
                            killer.Player.EpicTrojanMrMirrorPrograss += 1;



                            MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                            item.Color = (Role.Flags.Color)2;
                            item.ITEM_ID = 1182;
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 163, 228, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, killer.Player.Map
                                   , 0, false, killer.Map, 4);

                            if (killer.Map.EnqueueItem(DropItem))
                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);

                            if (killer.Player.EpicTrojanMrMirrorPrograss >= 18)
                            {
                                killer.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                killer.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                killer.Player.MessageBox("All~the~18~Ghost~Reavers~in~the~Flame~Devastator`s~army~have~been~eliminated.~Report~back~to~General~Pak!", new Action<Client.GameClient>(p =>
                                {
                                    killer.Teleport(162, 218, 3830);
                                    killer.Player.QuestGUI.SendAutoPatcher(3830, 154, 130, 10581);

                                }), null);
                            }
                            else
                            {
                                killer.Player.MessageBox("A~Ghost~Reaver~fell~down~and~vanished.~So~far,~you~still~have~" + (18 - killer.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reaver(s)~to~deal~with.", new Action<Client.GameClient>(p =>
                                {

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var pstream = rec.GetStream();
                                        killer.ActiveNpc = (uint)Game.MsgNpc.NpcID.MrMirror2;
                                        Game.MsgNpc.NpcHandler.MrMirror2(killer, pstream, 0, "", 0);
                                    }
                                }), null);
                            }
                        }
                    }
                    else if (killer.MyHouse != null && killer.Player.DynamicID == killer.Player.UID)
                    {
                        if (Family.ID == 2435)//HeavenDemonBox
                        {
                            if (Role.Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720679, stream);
                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Frost CP Pack (69000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Frost CP Pack (69000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else
                            {
                                if (Role.Core.Rate(10, 9999))
                                {
                                    DropItemID(killer, 720678, stream);
                                    killer.CreateBoxDialog("You killed a Heaven Demon and found a Life CP Pack (13500CPs)!");
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Life CP Pack (13500CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                                }
                                else
                                {
                                    if (Role.Core.Rate(3700, 9989))
                                    {
                                        DropItemID(killer, 720677, stream);
                                        killer.CreateBoxDialog("You killed a Heaven Demon and found a Blood CP Pack (1000CPs)!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1289, 6289))
                                        {
                                            DropItemID(killer, 720676, stream);
                                            killer.CreateBoxDialog("You killed a Heaven Demon and found a Soul CP Pack (500CPs)!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1000, 5000))
                                            {
                                                DropItemID(killer, 720675, stream);

                                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Ghost CP Pack (250CPs)!");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 720680, stream);
                                                killer.CreateBoxDialog("You killed a Heaven Demon and found a Heaven Pill equal to the EXP of 2 and a half EXP Balls!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2436)//ChaosDemonBox
                        {
                            if (Role.Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720685, stream);

                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Nimbus CP Pack (138000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Nimbus CP Pack (138000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            else
                            {
                                if (Role.Core.Rate(10, 9999))
                                {
                                    DropItemID(killer, 720684, stream);


                                    killer.CreateBoxDialog("You killed a Chaos Demon and found a Butterfly CP Pack (27000CPs)!");
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Butterfly CP Pack (27000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                                }
                                else
                                {
                                    if (Role.Core.Rate(3700, 9989))
                                    {
                                        DropItemID(killer, 720683, stream);
                                        killer.CreateBoxDialog("You killed a Chaos Demon and found a Heart CP Pack (2000CPs)!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1289, 6289))
                                        {
                                            DropItemID(killer, 720682, stream);
                                            killer.CreateBoxDialog("You killed a Chaos Demon and found a Flower CP Pack (1000CPs)!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1000, 5000))
                                            {
                                                DropItemID(killer, 720681, stream);
                                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Deity CP Pack (500CPs)!");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 720686, stream);
                                                killer.CreateBoxDialog("You killed a Chaos Demon and found a Mystery Pill equal to the EXP of 2 and 1/3 EXP Balls!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2437)//sacreddemon
                        {
                            if (Role.Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720691, stream);
                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Kylin CP Pack (276000CPs)");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Kylin CP Pack (276000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else
                            {
                                if (Role.Core.Rate(10, 9999))
                                {
                                    DropItemID(killer, 720690, stream);
                                    killer.CreateBoxDialog("You killed a Sacred Demon and found a Rainbow CP Pack (54000CPs)!");
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Rainbow CP Pack (54000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                                }
                                else
                                {
                                    if (Role.Core.Rate(3700, 9989))
                                    {
                                        DropItemID(killer, 720689, stream);

                                        killer.CreateBoxDialog("You killed a Sacred Demon and found a Shadow CP Pack (4000CPs)!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1289, 6289))
                                        {
                                            DropItemID(killer, 720688, stream);
                                            killer.CreateBoxDialog("You killed a Sacred Demon and found a Jewel CP Pack (2000CPs)!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1000, 5000))
                                            {
                                                DropItemID(killer, 720687, stream);

                                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Cloud CP Pack (1000CPs)!");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 720692, stream);
                                                killer.CreateBoxDialog("You killed a Sacred Demon and found a Wind Pill equal to the EXP of 5 EXP Balls!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2438)//aurorademonbox
                        {
                            if (Role.Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720697, stream);

                                killer.CreateBoxDialog("You killed an Aurora Demon and found a Pilgrim CP Pack (690000CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " got a Pilgrim CP Pack (690000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            else
                            {
                                if (Role.Core.Rate(10, 9999))
                                {
                                    DropItemID(killer, 720696, stream);
                                    killer.CreateBoxDialog("You killed an Aurora Demon and found a Zephyr CP Pack (135000CPs)!");
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Zephyr CP Pack (135000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                                }
                                else
                                {
                                    if (Role.Core.Rate(3700, 9989))
                                    {
                                        DropItemID(killer, 720695, stream);
                                        killer.CreateBoxDialog("You killed an Aurora Demon and found an Earth CP Pack (10000CPs)!");
                                        Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found an Earth CP Pack (10000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1289, 6289))
                                        {
                                            DropItemID(killer, 720694, stream);
                                            killer.CreateBoxDialog("You killed an Aurora Demon and found a Moon CP Pack (5000CPs)!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1000, 5000))
                                            {
                                                DropItemID(killer, 720693, stream);
                                                killer.CreateBoxDialog("You killed an Aurora Demon and found a Fog CP Pack (2500CPs)!");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 720698, stream);
                                                killer.CreateBoxDialog("You killed an Aurora Demon and got a Wind Pill equal to the EXP of 8 and 1/3 EXP Balls!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2420)//demon
                        {
                            if (Role.Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720654, stream);

                                killer.CreateBoxDialog("You killed a Demon and found a Joy CP Pack (1380CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Joy CP Pack (1380CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            else
                            {
                                if (Role.Core.Rate(10, 9999))
                                {
                                    DropItemID(killer, 720653, stream);

                                    killer.CreateBoxDialog("You killed a Demon and found a Dream CP Pack (270CPs)!");
                                }
                                else
                                {
                                    if (Role.Core.Rate(3700, 9989))
                                    {
                                        DropItemID(killer, 720655, stream);
                                        killer.CreateBoxDialog("You killed a Demon and found a Mammon CP Pack (20CPs)!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1289, 6289))
                                        {
                                            DropItemID(killer, 720656, stream);
                                            killer.CreateBoxDialog("You killed a Demon and found a Mascot CP Pack (10CPs)!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1000, 5000))
                                            {
                                                DropItemID(killer, 720657, stream);
                                                killer.CreateBoxDialog("You killed a Demon and found a Hope CP Pack (5CPs)!");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 720668, stream);
                                                killer.CreateBoxDialog("You killed a Demon and found a Magic Ball equal to the EXP of 1/6 of an EXP Ball!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2421)//ancient
                        {
                            if (Role.Core.Rate(1, 10000))
                            {
                                killer.CreateBoxDialog("You killed a Ancient Demon and found a Mystic CP Pack (6900CPs)!");
                                DropItemID(killer, 720662, stream);
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Mystic CP Pack (6900CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            else
                            {
                                if (Role.Core.Rate(10, 9999))
                                {
                                    DropItemID(killer, 720661, stream);

                                    killer.CreateBoxDialog("You killed a Ancient Demon and found a Pure CP Pack (1350CPs)!");
                                }
                                else
                                {
                                    if (Role.Core.Rate(3700, 9989))
                                    {
                                        DropItemID(killer, 720660, stream);
                                        killer.CreateBoxDialog("You killed a Ancient Demon and found a Legend CP Pack (100CPs)!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1289, 6289))
                                        {
                                            DropItemID(killer, 720659, stream);

                                            killer.CreateBoxDialog("You killed a Ancient Demon and found a Sweet CP Pack (50CPs)!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1000, 5000))
                                            {
                                                DropItemID(killer, 720658, stream);

                                                killer.CreateBoxDialog("You killed a Ancient Demon and found a Festival CP Pack (25CPs)!");
                                            }
                                            else
                                            {
                                                DropItemID(killer, 720669, stream);
                                                killer.CreateBoxDialog("You killed the Ancient Demon and found a Super Ball equal to the EXP of 5/6 of an EXP Ball!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 2422)
                        {
                            if (Role.Core.Rate(1, 10000))
                            {
                                DropItemID(killer, 720667, stream);
                                killer.CreateBoxDialog("You killed a Flood Demon and found a Fantasy CP Pack (13800CPs)!");
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Fantasy CP Pack (13800CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            else
                            {
                                if (Role.Core.Rate(10, 9999))
                                {
                                    DropItemID(killer, 720666, stream);

                                    killer.CreateBoxDialog("You killed a Flood Demon and found a Star CP Pack (2700CPs)!");
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage("" + killer.Player.Name + " found a Star CP Pack (2700CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                                }
                                else
                                {
                                    /*if (Role.Core.Rate(3700, 9989))
                                    {
                                        DropItemID(killer, 720665, stream);

                                        killer.CreateBoxDialog("You killed a Flood Demon and found a Cute CP Pack (65CPs)!");
                                    }
                                    else
                                    {*/
                                    if (Role.Core.Rate(1289, 6289))
                                    {
                                        DropItemID(killer, 720664, stream);

                                        killer.CreateBoxDialog("You killed a Flood Demon and found a Flare CP Pack (100CPs)!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1000, 5000))
                                        {
                                            DropItemID(killer, 720663, stream);
                                            killer.CreateBoxDialog("You killed a Flood Demon and found a Violet CP Pack (50CPs)!");
                                        }
                                        else
                                        {
                                            DropItemID(killer, 720670, stream);
                                            killer.CreateBoxDialog("You killed the Flood Demon and found an Ultra Ball equal to EXP worth 1 and 2/3 EXP Balls!");
                                        }
                                    }
                                    //}
                                }
                            }
                        }
                    }
                    if (killer.Player.QuestGUI.CheckQuest(2375, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                    {
                        uint spirites = 0;
                        if (Level < 70)
                            spirites = 1;
                        else if (Level >= 70 && Level <= 99)
                            spirites = 2;
                        else if (Level >= 100 && Level <= 119)
                            spirites = 3;
                        else if (Level >= 120 && Level < 140)
                            spirites = 4;
                        else if (Boss == 1 && Family.MaxHealth >= 1000000)
                            spirites = 1000;

                        killer.Player.DailySpiritBeadCount += spirites;
                        killer.SendSysMesage("You received " + spirites + " spirites.", MsgMessage.ChatMode.System);

                        if (Game.MsgNpc.NpcHandler.GetDailySpiritBeadKills(killer) <= killer.Player.DailySpiritBeadCount)
                        {
                            if (!killer.Player.QuestGUI.CheckObjectives(2375, 1))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 2375, 1, 1);
                                killer.CreateBoxDialog("You`ve~collected~enough~spirits,~and~you~can~use~the~bead~to~claim~a~reward,~now!");

                            }
                        }
                    }
                    if (Family.ID == 20160)
                    {
                        if (killer.Team != null)
                        {
                            foreach (var user in killer.Team.Temates)
                            {
                                if (user.client.Player.Map == killer.Player.Map && user.client.Player.DynamicID == killer.Player.DynamicID)
                                {
                                    if (user.client.Player.QuestGUI.CheckQuest(6126, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                    {
                                        user.client.Player.QuestGUI.IncreaseQuestObjectives(stream, 6126, 1);
                                    }
                                }
                            }
                        }
                        if (killer.Player.QuestGUI.CheckQuest(6126, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 6126, 1);
                        }
                    }
                    if (killer.DemonExterminator != null)
                        killer.DemonExterminator.UppdateJar(killer, Family.ID);

                    if (killer.Player.Map == 1770 || killer.Player.Map == 1771 || killer.Player.Map == 1772 || killer.Player.Map == 1773
                        || killer.Player.Map == 1774 || killer.Player.Map == 1775 || killer.Player.Map == 1777)
                    {
                        Game.MsgTournaments.MsgSchedules.PowerArena.CheckMonstersMap(killer.Player);
                    }
                    else if (killer.Player.Map == 3071)
                    {
                        if (Family.ID == 2700)
                        {
                            DropItemID(killer, 1088001, stream, 6);
                            DropItemID(killer, 1088001, stream, 6);
                            DropItemID(killer, 1088001, stream, 6);
                            DropItemID(killer, 723341, stream, 6);
                            DropItemID(killer, 723341, stream, 6);
                            DropItemID(killer, 723341, stream, 6);

                            if (Role.Core.Rate(5, 100))
                                DropItemID(killer, 710212, stream, 6);
                            else if (Role.Core.Rate(30, 100))
                                DropItemID(killer, 720128, stream, 6);
                            else if (Role.Core.Rate(30, 100))
                                DropItemID(killer, 720128, stream, 6);
                            else if (Role.Core.Rate(20, 100))
                                DropItemID(killer, 728917, stream, 6);
                            else if (Role.Core.Rate(20, 100))
                                DropItemID(killer, 727306, stream, 6);
                            else if (Role.Core.Rate(20, 100))
                                DropItemID(killer, 728918, stream, 6);
                            else if (Role.Core.Rate(10, 100))
                                DropItemID(killer, 710214, stream, 6);
                            else if (Role.Core.Rate(20, 100))
                                DropItemID(killer, Database.ItemType.ExpBall2, stream, 6);
                            else
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 711609);
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                                    killer.CreateBoxDialog("You~received~a~Gold~Coin!");
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }

                        }
                        else if (Family.ID == 2699)
                        {
                            if (killer.Inventory.HaveSpace(1))
                            {
                                killer.Inventory.Add(stream, 711610);
                                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel7");
                                killer.SendSysMesage("You~received~a~Silver~Coin!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                killer.CreateBoxDialog("Your~inventory~is~full!");
                            }
                        }
                        else if (Family.ID == 7022)
                        {
                            if (Role.Core.Rate(1, 30))//to check !!!
                            {
                                if (Role.Core.Rate(1, 200))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711609);
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                                        killer.CreateBoxDialog("You~received~a~Gold~Coin!");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full!");
                                    }
                                }
                                else
                                {
                                    if (Role.Core.Rate(2, 199))
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.Add(stream, 711610);
                                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel7");
                                            killer.SendSysMesage("You~received~a~Silver~Coin!", MsgMessage.ChatMode.System);
                                        }
                                        else
                                        {
                                            killer.CreateBoxDialog("Your~inventory~is~full!");
                                        }
                                    }
                                    else
                                    {

                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.Add(stream, 711611);
                                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel5");
                                            killer.SendSysMesage("You~received~a~Copper~Coin!", MsgMessage.ChatMode.System);
                                        }
                                        else
                                        {
                                            killer.CreateBoxDialog("Your~inventory~is~full!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 3935)
                    {
                        if (Family.ID == 40125)
                        {
                            killer.Send(stream.InterCreateItem(3600031));
                            killer.CreateBoxDialog("You've received a bounty of (Star Stone Pack) for conquering the divine Maniac!");
                        }
                        if (Family.ID == 40121 || Family.ID == 40122 || Family.ID == 40123 || Family.ID == 40124)
                        {
                            killer.Send(stream.InterCreateItem(3600031));
                            killer.CreateBoxDialog("You've received a bounty of (Star Stone Pack) for conquering the divine crystal!");

                        }
                        if (Family.ID == 40120)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35034, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(35034, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                {
                                    if (killer.Player.QuestGUI.CheckKingDomQuest(35034, 1) == false)
                                    {
                                        killer.Player.QuestGUI.FinishQuest(35034);
                                        killer.CreateBoxDialog("You've completed the [Scramble for Justice] mission in the realm, you've received a bounty of (Senior Dragon Soul)");
                                    }
                                }
                            }
                            killer.Send(stream.InterCreateItem(3600027));
                        }

                        if (Family.ID == 40104 || Family.ID == 40109 || Family.ID == 40110 || Family.ID == 40108)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35025, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(35025, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                {
                                    if (killer.Player.QuestGUI.CheckKingDomQuest(35025, 1) == false)
                                    {
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 35025, 1);
                                        killer.CreateBoxDialog("You've defeated the divine beast! Hurry and claim your reward from the Kingdom Mission Envoy!");

                                    }
                                }
                            }
                        }

                        if (killer.Player.QuestGUI.CheckQuest(35025, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35025, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                if (killer.Player.QuestGUI.CheckKingDomQuest(35025, 1) == false)
                                {
                                    if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 367, 459) <= 18)
                                    {
                                        if (Role.Core.Rate(40))
                                        {
                                            if (killer.Player.View.ContainMobInScreen("WhiteTiger") == false)
                                                Database.Server.AddMapMonster(stream, killer.Map, 40104, killer.Player.X, killer.Player.Y, 367, 459, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                        }
                                    }
                                    else if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 471, 371) <= 18)
                                    {
                                        if (Role.Core.Rate(40))
                                        {
                                            if (killer.Player.View.ContainMobInScreen("AzureDragon") == false)
                                                Database.Server.AddMapMonster(stream, killer.Map, 40109, killer.Player.X, killer.Player.Y, 471, 371, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                        }
                                    }
                                    else if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 371, 289) <= 18)
                                    {
                                        if (Role.Core.Rate(40))
                                        {
                                            if (killer.Player.View.ContainMobInScreen("VermilionBird") == false)
                                                Database.Server.AddMapMonster(stream, killer.Map, 40108, killer.Player.X, killer.Player.Y, 371, 289, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                        }
                                    }
                                    else if (Role.Core.GetDistance(killer.Player.X, killer.Player.Y, 295, 337) <= 18)
                                    {
                                        if (Role.Core.Rate(40))
                                        {
                                            if (killer.Player.View.ContainMobInScreen("BlackTurtle") == false)
                                                Database.Server.AddMapMonster(stream, killer.Map, 40110, killer.Player.X, killer.Player.Y, 295, 337, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                        }
                                    }
                                }
                            }
                        }

                        if (killer.Player.QuestGUI.CheckQuest(35028, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35028, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                if (killer.Player.QuestGUI.CheckKingDomQuest(35028, 50) == false)
                                {

                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 35028, 1);

                                    if (killer.Player.QuestGUI.CheckKingDomQuest(35028, 50))
                                        killer.CreateBoxDialog("You've eliminated 50 enemies. Hurry and claim a bounty from Realm Crystal.");

                                }
                            }
                        }
                        if (killer.Player.QuestGUI.CheckQuest(35007, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (killer.Player.QuestGUI.CheckQuest(35007, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                if (killer.Player.QuestGUI.CheckKingDomQuest(35007, 300) == false)
                                {
                                    uint Points = (uint)Program.GetRandom.Next(2, 6);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 35007, Points);
                                    uint[] Intentions;
                                    killer.Player.QuestGUI.GetQuestObjectives(35007, out Intentions);

                                    killer.SendSysMesage("You've received " + Math.Min(300, Intentions[0]).ToString() + " Strike Points by killing enemies fiercely. When you earn 300 Strike Points, you can claim a reward from the Kingdom Mission Envoy.", MsgMessage.ChatMode.System);
                                    if (killer.Player.QuestGUI.CheckKingDomQuest(35007, 300))
                                        killer.CreateBoxDialog("You've completed the [Thunder Strike] mission in the realm. Report back to the Kingdom Mission Envoy to claim your reward.");

                                }
                            }
                        }
                        goto jmp;
                    }
                    else if (killer.Player.Map == 3998)
                    {
                        if (Family.ID == 40903)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3634, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 3634, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(3634, 10))
                                {
                                    killer.CreateBoxDialog("You've eliminated enough number of Anger Rats. Hurry and report back to Chong Yan Elder!");
                                }
                            }
                        }
                        else if (Family.ID == 40900)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3636, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(45))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.BrokenForgeFurnace, killer.Player.Class, 3636);
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (killer.Inventory.Contain(3008752, 8) == false)
                                        {
                                            killer.Inventory.AddItemWitchStack(3008752, 0, 1, stream);
                                        }
                                        if (killer.Inventory.Contain(3008752, 8))
                                            killer.Player.QuestGUI.SendAutoPatcher("You've collected enough number of Rune Fragments. Go and try to complete the runes on the Forge Furnace.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                        }
                        else if (Family.ID == 40902)
                        {

                            if (killer.Player.QuestGUI.CheckQuest(3638, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.BrokenForgeFurnace, killer.Player.Class, 3638);
                                    if (!killer.Inventory.Contain(3008750, 1))
                                        killer.Inventory.Add(stream, 3008750);
                                    killer.Player.QuestGUI.SendAutoPatcher("The Violet Bat King fell down and dropped an ancient-style hammer.Hurry and take this hammer to the Forge Furnace.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                        }
                        else if (Family.ID == 40904)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3641, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 3641, 1);

                                if (killer.Player.QuestGUI.CheckObjectives(3641, 15))
                                    killer.Player.QuestGUI.SendAutoPatcher("You've defeat enough number of Lava Scorpions. Now, you can appease the sacrificed Bright people.", 3998, 220, 294, 0);

                            }

                        }
                        else if (Family.ID == 40908)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3644, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.FlameAltar, killer.Player.Class, 3644);
                                    if (!killer.Inventory.Contain(3008742, 50))
                                        killer.Inventory.AddItemWitchStack(3008742, 0, 1, stream);
                                    else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve collect enough number of Building Stones. Go and try to restore the ruined altar.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                        }
                        else if (Family.ID == 40907)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3648, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.FlameAltar, killer.Player.Class, 3648);
                                    if (!killer.Inventory.Contain(3008748, 100))
                                        killer.Inventory.AddItemWitchStack(3008748, 0, 1, stream);
                                    else
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve collected enough number of Star Ores. Go and try to extract the Essence of Star at the altar.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }

                        }
                        else if (Family.ID == 40905)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3645, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(40))
                                    {//3008744
                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.FlameAltar, killer.Player.Class, 3645);
                                        if (!killer.Inventory.Contain(3008743, 1))
                                            killer.Inventory.AddItemWitchStack(3008743, 0, 1, stream);
                                        else
                                            killer.Player.QuestGUI.SendAutoPatcher("You`ve retrieved the Wheel of Nature from the Clawed Rock Devil. Hurry and take the next action at the altar.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                        }
                        else if (Family.ID == 40906)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(3646, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(40))
                                    {
                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.FlameAltar, killer.Player.Class, 3646);
                                        if (!killer.Inventory.Contain(3008745, 1))
                                        {

                                            if (!killer.Inventory.Contain(3008754, 5))
                                                killer.Inventory.AddItemWitchStack(3008754, 0, 1, stream);
                                            if (killer.Inventory.Contain(3008754, 5))
                                            {
                                                killer.Inventory.Remove(3008754, 5, stream);
                                                killer.Inventory.Add(stream, 3008745, 1);
                                                killer.Player.QuestGUI.SendAutoPatcher("You received the Earth Force! Hurry and transform it into Metal Force through the Wheel of Nature!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                            }
                                        }
                                        else
                                            killer.Player.QuestGUI.SendAutoPatcher("You received the Earth Force! Hurry and transform it into Metal Force through the Wheel of Nature!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("You received the Earth Force! Hurry and transform it into Metal Force through the Wheel of Nature!");
                            }


                        }


                        //3008754
                    }
                    else if (killer.Player.Map == 1002)
                    {
                        if (Family.ID == 1)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1307, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1307, 1);
                                killer.CreateBoxDialog("You~killed~Pheasant~and~scared~the~Apes~away!");

                            }
                            if (killer.Player.QuestGUI.CheckQuest(824, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 824, 1);
                        }
                        else if (Family.ID == 2)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1007, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Inventory.Contain(711236, 1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.TCViceCaptain, killer.Player.Class, 1007);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1007, 1);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~a~Turtledove~Plume.~Please~take~it~to~TC~Vice~Captain.", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                                    killer.Inventory.Add(stream, 711236);
                                }
                            }
                            //1009
                        }
                        else if (Family.ID == 3)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1735, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1735, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1735, 100))
                                {
                                    killer.CreateBoxDialog("You`ve~collected~enough~Feathers.~Now~you~can~go~report~to~Artisan~Luo.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1009, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1009, 1);
                            if (killer.Player.QuestGUI.CheckQuest(2365, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 2365, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(2365, 100))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.XuLiang, killer.Player.Class, 2365);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~100~Robins!~Report~back~to~XuLiang~and~claim~your~reward.", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1010, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1010, 1, 1);
                                if (!killer.Inventory.Contain(711238, 10))
                                    killer.Inventory.AddItemWitchStack(711238, 0, 1, stream);
                                if (killer.Player.QuestGUI.CheckObjectives(1010, 10))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.ArtisanLuo, killer.Player.Class, 1010);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~10~Robin~Feathers.~Please~take~them~to~Artisan~Luo.", 1002, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                }
                            }
                        }
                        else if (Family.ID == 4)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1013, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Inventory.Contain(711241, 1))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.RuHua, killer.Player.Class, 1013);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1013, 1);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~the~Apparition~and~received~a~Handkerchief!", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                                    killer.Inventory.Add(stream, 711241);
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1016, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You've collected spirit of an Apparition and stored it in Sipirit Container.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1016, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1016, 10))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.Fortuneteller, killer.Player.Class, 1016);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~the~spirits~of~10~Apparitions.~Hurry~to~report~back~to~the~Fortuneteller!", 1002, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                                }
                            }
                        }
                    }
                    else if (killer.Player.Map == 1013 && killer.Player.DynamicID != 0)
                    {
                        if (Family.ID == 14333)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1748, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1748, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1748, 5))
                                {
                                    killer.Teleport(299, 244, 1002);
                                    killer.SendSysMesage("You`ve~returned~to~Royal~Doctor~Li.");

                                }
                            }
                        }

                    }
                    else if (killer.Player.Map == 1780)
                    {
                        if (Family.ID == 12314463)
                        {
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = 721714;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(721714, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        else if (Family.ID == 12314462)
                        {
                            if (killer.Player.View.ContainMobInScreen("KidSiezer") == false)
                            {
                                Database.Server.AddMapMonster(stream, killer.Map, 12314463, killer.Player.X, killer.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                effect.m_UID = (uint)MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeUpDown;
                                effect.m_X = killer.Player.X;
                                effect.m_Y = killer.Player.Y;
                                killer.Send(stream.ItemPacketCreate(effect));
                                killer.CreateBoxDialog("The~Kid~Siezer~appeared.~Go~kill~it.");
                            }
                        }
                        else if (Role.Core.Rate(10))
                        {
                            if (killer.Player.View.ContainMobInScreen("KidHunter") == false && !killer.Inventory.Contain(721714, 1))
                            {
                                Database.Server.AddMapMonster(stream, killer.Map, 12314462, killer.Player.X, killer.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                effect.m_UID = (uint)MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeUpDown;
                                effect.m_X = killer.Player.X;
                                effect.m_Y = killer.Player.Y;
                                killer.Send(stream.ItemPacketCreate(effect));
                                killer.CreateBoxDialog("The~Kid~Hunter~appeared.~Go~kill~it.");
                            }
                        }
                    }
                    else if (killer.Player.Map == 1015)
                    {
                        if (Family.ID == 2171)
                        {
                            if (!killer.Inventory.Contain(710217, 1))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 710217, 1);
                                    killer.Player.QuestGUI.SetQuestObjectives(stream, 408, 1);
                                    killer.SendSysMesage("You`ve killed the Jade Courtesan and obtained her Jade Sceptre.");
                                    if (killer.Inventory.Contain(710216, 1))
                                    {
                                        if (killer.Inventory.Contain(710217, 1))
                                        {
                                            if (killer.Inventory.Contain(710218, 1))
                                            {
                                                if (killer.Inventory.Contain(710219, 1))
                                                {
                                                    killer.SendSysMesage("You`ve collected the Purple Eye, Jade Scepter, Jadeite, and Hairpin. Go to open the Teleport Platform on Bird Island (655,693).");
                                                }
                                                else
                                                {
                                                    killer.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                                }
                                            }
                                            else
                                            {
                                                killer.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                            }
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                        }
                                    }
                                    else
                                    {
                                        killer.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                    }
                                }
                                else
                                {
                                    killer.SendSysMesage("Your inventory is full! Make some room, first.");
                                }
                            }

                        }
                        else if (Family.ID == 2157)//bloodyshaw
                        {
                            if (killer.Player.QuestGUI.CheckQuest(523, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(721781, 1))
                                    {
                                        killer.Inventory.Add(stream, 721781, 1);
                                        killer.CreateBoxDialog("Your~power~and~fortitude~overwhelmed~Bloody~Shawn,~and~you~obtained~his~Spear.~Grab~it~along~and~find~Ruan~Brother.");
                                    }
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Your~inventory~is~full!");
                                }
                            }
                        }
                        else if (Family.ID == 2156)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (!killer.Inventory.Contain(721778, 3))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.AddItemWitchStack(721778, 0, 1, stream);
                                        killer.CreateBoxDialog("Trout~Elves~died~and~turned~into~fresh~fish.~Collect~3~fish~and~trade~with~Ye~Sheng~(Bird~Island~324,400)~for~his~Iron~Poise.");

                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~inventory~is~full!");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 85)//senior
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1824, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710566, 1))
                                        {
                                            killer.SendSysMesage("You picked up the Scarlet Token. Now hurry up and give it to Felix!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(710566, 0, 1, stream);
                                        }
                                    }
                                    else
                                        killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }

                            }
                        }
                        if (Family.ID == 2226)//banditdelivery
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1823, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710565, 1))
                                        {
                                            killer.SendSysMesage("You picked up the Pitch Token. Now hurry up and give it to Felix!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(710565, 0, 1, stream);
                                        }
                                    }
                                    else
                                        killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }

                            }
                        }
                        if (Family.ID == 2225)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1822, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(50))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710564, 1))
                                        {
                                            killer.SendSysMesage("You picked up the Black Token. Now hurry up and give it to Felix!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(710564, 0, 1, stream);
                                        }
                                    }
                                    else
                                        killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }

                            }
                        }
                        if (Family.ID == 79)//banditL98
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1821, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710563, 1))
                                        {
                                            killer.SendSysMesage("You~picked~up~the~Indigo~Token.~Now~hurry~up~and~give~it~to~Felix!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(710563, 0, 1, stream);
                                        }
                                    }
                                    else
                                        killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711474, 10))
                                        {
                                            killer.SendSysMesage("You received a Shot Saber!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711474, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1641, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BanditL98.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1641, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BanditL98.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1647, 0, 1);

                            }

                        }
                        if (Family.ID == 84)//banditti
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1651, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711476, 1))
                                        {
                                            killer.SendSysMesage("You received 1 Yarn Ball!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711476, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1652, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711477, 1))
                                        {
                                            killer.SendSysMesage("You received Flower Wine!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711477, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1650, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1650, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1645, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Banditti.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711475, 1))
                                        {
                                            killer.SendSysMesage("You received the Royal Sword!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711475, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                        }
                        if (Family.ID == 2224)
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1820, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(710562, 1))
                                    {
                                        killer.Inventory.Add(stream, 710562);
                                        killer.CreateBoxDialog("You picked up the Red Token. Now hurry up and give it to Felix!");
                                    }
                                }
                                else
                                {
                                    killer.CreateBoxDialog("Sorry, your inventory is full. Make some room first.");
                                }
                            }
                        }
                        if (Family.ID == 55)//banditL97
                        {


                            if (killer.Player.QuestGUI.CheckQuest(1820, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1820, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1820, 50))
                                {
                                    if (!killer.Inventory.Contain(710562, 1))
                                    {
                                        if (!killer.Player.View.ContainMobInScreen("BanditLeaderL97"))
                                        {
                                            killer.CreateBoxDialog("Bandit Leader L97 appeared on Bird Island (262,118). Get rid of him!");
                                            Database.Server.AddMapMonster(stream, killer.Map, 2224, killer.Player.X, killer.Player.Y, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                        }

                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1647, 1);

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1635, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711468, 1))
                                        {
                                            killer.SendSysMesage("You~received~a~box~of~Food~Supply!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711468, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1632, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711467, 5))
                                        {
                                            killer.SendSysMesage("You've received a piece of Bacon!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711467, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1633, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BanditL97.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1633, 1);

                            }
                        }
                        if (Family.ID == 19)//hawkings
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1814, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721910, 5))
                                        {
                                            killer.Inventory.AddItemWitchStack(721910, 0, 1, stream);
                                            killer.CreateBoxDialog("You've got a Hawk Claw!");
                                        }
                                        else
                                            killer.CreateBoxDialog("You've got 5 Hawk Claws! Hurry to deliver them to Doctor Know-it-All!");
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721909, 1) && !killer.Inventory.Contain(721908, 1))
                                        {
                                            killer.Inventory.Add(stream, 721909);
                                            killer.CreateBoxDialog("You've got a Gallbladerr!");
                                        }
                                    }

                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1630, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1630, 0, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1616, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1616, 2, 1);


                            }
                            if (killer.Player.QuestGUI.CheckQuest(1611, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1611, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1618, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711454, 5))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~Feather!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711454);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1610, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKing.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(720987, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~OldBook1!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720987);
                                        }
                                        else if (!killer.Inventory.Contain(720988, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~OldBook2!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720988);
                                        }
                                        else if (!killer.Inventory.Contain(720989, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~OldBook3!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720989);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                        }
                        if (Family.ID == 4431179)//haw leader
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1629, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawkLeader.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1629, 1);
                            }
                        }
                        if (Family.ID == 78)//hawkingsL93
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1814, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (Role.Core.Rate(20))
                                    {
                                        if (!killer.Inventory.Contain(721910, 5))
                                        {
                                            killer.Inventory.AddItemWitchStack(721910, 0, 1, stream);
                                            killer.CreateBoxDialog("You've got a Hawk Claw!");
                                        }
                                        else
                                            killer.CreateBoxDialog("You've got 5 Hawk Claws! Hurry to deliver them to Doctor Know-it-All!");
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (!killer.Inventory.Contain(721909, 1) && !killer.Inventory.Contain(721908, 1))
                                    {
                                        killer.Inventory.Add(stream, 721909);
                                        killer.CreateBoxDialog("You've got a Gallbladerr!");
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1630, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1630, 1);

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1628, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1628, 1);
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1624, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(720993, 1))
                                        {
                                            killer.SendSysMesage("You~received~some~Hawk~Blood!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720993);
                                        }

                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1622, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711456, 1))
                                        {
                                            killer.SendSysMesage("You~received~1~Hawk~Claw!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711456);
                                        }
                                        else if (!killer.Inventory.Contain(711457, 1))
                                        {
                                            killer.SendSysMesage("You~received~1~Hawk~Fang!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711457);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1621, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a HawKingL93.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1621, 1);

                            }
                        }
                        if (Family.ID == 77)//birdmanl88
                        {
                            //721906
                            if (killer.Player.QuestGUI.CheckQuest(1812, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721906, 10))
                                        {
                                            killer.Inventory.AddItemWitchStack(721906, 0, 1, stream);
                                            killer.CreateBoxDialog("You've got some Bird Blood!");
                                        }
                                        else
                                            killer.CreateBoxDialog("You've collected enough Bird Blood!");
                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1599, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711445, 5))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~Bird~Beak!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711445);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1601, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711446, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~a~Sharp~Claw!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711446);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1605, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711449, 5))
                                        {
                                            killer.SendSysMesage("You received a Birdman Feather!", MsgMessage.ChatMode.System);
                                            killer.Inventory.AddItemWitchStack(711449, 0, 1, stream);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1604, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1604, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1598, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a BirdmanL88.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1598, 1);

                            }

                        }
                        if (Family.ID == 18)//birdman
                        {
                            if (Family.ID == 18)//bird man
                            {
                                if (Role.Core.Rate(10))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 729094;//birdclaw
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(729094, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1812, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721906, 10))
                                        {
                                            killer.Inventory.AddItemWitchStack(721906, 0, 1, stream);
                                            killer.CreateBoxDialog("You've got some Bird Blood!");
                                        }
                                        else
                                            killer.CreateBoxDialog("You've collected enough Bird Blood!");
                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1626, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711464, 1))
                                        {
                                            killer.SendSysMesage("You~received~1~Golden~Beak!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711464);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1597, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(720986, 1))
                                        {
                                            killer.SendSysMesage("You`ve~got~some~Birdman~Feathers!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 720986);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1592, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(711438, 5))
                                        {
                                            killer.SendSysMesage("You received a Birdman Arm!", MsgMessage.ChatMode.System);
                                            killer.Inventory.Add(stream, 711438);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }

                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1591, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Birdman.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1591, 1);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1593, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(44))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711439);
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.CreateBoxDialog("You`ve~got~Xu~Fan`s~Tool~Bag!");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                        }
                    }
                    else if (killer.Player.Map == 1000)
                    {
                        if (Family.ID == 2163)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(525, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    if (!killer.Inventory.Contain(721782, 1))
                                    {
                                        killer.Inventory.Remove(721777, 1, stream);
                                        killer.Inventory.Add(stream, 721782, 1);
                                        killer.CreateBoxDialog("Your~gut~feeling~told~you~that~the~Bud~Handkerchief~has~something~to~do~with~Yang~Yun~in~Bird~Island.~Find~him~(732,516)~now.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Your~gut~feeling~told~you~that~the~Bud~Handkerchief~has~something~to~do~with~Yang~Yun~in~Bird~Island.~Find~him~(732,516)~now.");
                                    }
                                }
                                else
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                        }
                        else if (Family.ID == 2179)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(525, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (!killer.Inventory.Contain(721782, 1))
                                    {
                                        killer.CreateBoxDialog("Caprice~Leader~has~appeared~at~(170,230).");
                                        Database.Server.AddMapMonster(stream, killer.Map, 2163, 170, 230, 3, 3, 1, killer.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "ride_screen");
                                    }
                                }
                            }
                        }
                        else if (Family.ID == 55434417)//angry blade ghost
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1498, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a AngryBladeGhost.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1498, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1498, 3))
                                {
                                    killer.CreateBoxDialog("Sculptor~He~has~finished~the~inscription!~Go~visit~him,~right~away.");


                                }
                            }
                        }
                        if (Family.ID == 76)//blade ghost L83
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1803, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(50))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721846, 5))
                                        {
                                            killer.Inventory.Add(stream, 721846);
                                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                            killer.CreateBoxDialog("You~picked~up~a~Blade~Ghost~Fang~and~put~it~in~your~inventory.");
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1801, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(721843, 1))
                                        {
                                            killer.Inventory.Add(stream, 721843);
                                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                            killer.CreateBoxDialog("You~obtained~the~Stone~Spell.");
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1499, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve killed a Level 83 Blade Ghost.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1499, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1499, 30))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~30~Level~83~Blade~Ghosts.~Go~report~to~Spring~General~Xu.");


                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1506, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 720968);
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.CreateBoxDialog("You`ve~killed~the~Level~83~Blade~Ghost~and~obtained~an~Antidote.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1500, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711407);
                                        killer.CreateBoxDialog("You`ve~killed~a~Level~83~Blade~Ghost~and~cut~its~ear.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                        }
                        if (Family.ID == 17)//blade ghost
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1802, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1802, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1802, 200))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 721845);
                                        killer.CreateBoxDialog("You`ve~killed~200~Blade~Ghosts~and~found~a~Blunt~Sword!");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1501, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711413);
                                        killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~cut~its~ear.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1508, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711414);
                                        killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~obtained~a~Straight~Hook.~Deliver~it~to~Elder~Jiang.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1494, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.Contain(711406, 5))
                                {
                                    killer.SendSysMesage("You`ve gathered 5 Sharp Blades. Hurry to deliver them to Spring Vice General Ou!", MsgMessage.ChatMode.System);
                                }
                                if (Role.Core.Rate(25))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~obtained~a~Sharp~Blade.");
                                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711406;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711406, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1497, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711409);
                                        killer.CreateBoxDialog("You`ve~killed~the~Blade~Ghost~and~found~a~Sharp~Axe.~Deliver~it~to~Sculptor~He.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1493, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve~killed~the~Blade~Ghost.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1493, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1493, 50))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~enough~Blade~Ghosts.~Go~report~to~Spring~General~Xu.");


                                }
                            }

                        }
                        if (Family.ID == 75)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1797, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(45))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        byte rand = (byte)Program.GetRandom.Next(0, 3);
                                        switch (rand)
                                        {
                                            case 0:
                                                {
                                                    killer.Inventory.AddItemWitchStack(721952, 0, 1, stream);
                                                    killer.SendSysMesage("You Maple Stone came out from the Rock Monster`s body. Pick it up.", MsgMessage.ChatMode.System);
                                                    break;
                                                }
                                            case 1:
                                                {
                                                    killer.Inventory.AddItemWitchStack(721953, 0, 1, stream);
                                                    killer.SendSysMesage("A Ocean Stone came out from the Rock Monster`s body. Pick it up.", MsgMessage.ChatMode.System);
                                                    break;
                                                }
                                            default:
                                                {
                                                    killer.Inventory.AddItemWitchStack(721951, 0, 1, stream);
                                                    killer.SendSysMesage("A Leaf Stone came out from the Rock Monster`s body. Pick it up.", MsgMessage.ChatMode.System);
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1796, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        if (!killer.Inventory.Contain(710581, 10))
                                        {
                                            killer.Inventory.AddItemWitchStack(710581, 0, 1, stream);
                                            killer.SendSysMesage("A Marble came out from the Rock Monster`s body. Check your inventory.", MsgMessage.ChatMode.System);
                                        }
                                        else
                                            killer.SendSysMesage("You've collected 10 Marbles. Take them to Farmer Lynn!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1485, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711402);
                                        killer.CreateBoxDialog("Deliver~the~Monster~Tail~to~Convoy~Vice~Leader~Ling.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1491, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711405);
                                        killer.CreateBoxDialog("You`ve~killed~the~Level~78~Rock~Monster~and~obtained~a~Thunder~Stone.~Deliver~it~to~Ironsmith~Li.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1484, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1484, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1484, 30))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~30~Level~78~Rock~Monsters.~Go~report~to~Convoy~Leader~Gu.");

                                }
                            }
                        }

                        if (Family.ID == 51)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1479, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve~killed~the~Rock~Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1479, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1479, 50))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~50~Rock~Monsters.~Go~report~to~General~ZhuGe.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1501, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711412);
                                        killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~Rock~Monster`s~Tail.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1480, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~a~Colorful~Stone.");
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711349;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711349, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1481, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~a~Grindstone.");
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711399;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711399, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1478, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(45))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~Rock~Monster~and~obtained~a~slabstone.");
                                    killer.Inventory.AddItemWitchStack(720970, 0, 1, stream);
                                }
                            }
                        }
                        else if (Family.ID == 74)
                        {
                            if (Role.Core.Rate(5))
                            {
                                if (!killer.Player.QuestGUI.CheckQuest(1477, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~Hill~Monster~and~received~a~Secret~Letter.");
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720975;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720975, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1471, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1471, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1471, 30))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~30~Hill~Monsters~L73.~Go~report~to~Han~Cheng.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1476, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1476, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1476, 40, 10))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~40~Hill~Monsters~and~10~Hill~Monsters~L73.~Go~report~to~Han~Cheng.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1473, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("You`ve~killed~a~Level~73~Hill~Monster~and~obtained~an~armor.");
                                        killer.Inventory.Add(stream, 711416);
                                    }
                                    else
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1472, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You`ve~killed~a~Level~73~Hill~Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1472, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1472, 20))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~20~Level~73~Hill~Monsters.~Go~report~to~Ironsmith~Li.");

                                }
                            }

                        }
                        else if (Family.ID == 14)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1458, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711341);
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.CreateBoxDialog("You`ve~killed~the~Sand~Monster~and~received~a~Sandbag.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }


                            if (killer.Player.QuestGUI.CheckQuest(1501, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711410);
                                        killer.CreateBoxDialog("You`ve~killed~the~Sand~Monster~and~received~a~Sand~Pill.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }

                            if (killer.Player.QuestGUI.CheckQuest(1464, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You've~killed~a~Sand~Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1464, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1464, 40))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~40~Sand~Monsters.~After~you~kill~another~10~Level~68~Sand~Monsters,~you~can~go~report~to~Ke~Yulun.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1451, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You've~killed~a~Sand~Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1451, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1451, 50))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~50~Sand~Monsters.~Go~report~to~the~DC~Vice~Captain.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1453, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Inventory.Add(stream, 711338);
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");

                                    }
                                }
                            }
                        }
                        else if (Family.ID == 15)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1501, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(40))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                        killer.Inventory.Add(stream, 711411);
                                        killer.CreateBoxDialog("You`ve~killed~the~Hill~Monster~and~received~a~Hill~Monster`s~Heart.");
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                    }
                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1476, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You've killed a Hill Monster.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1476, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1476, 40, 10))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~40~Hill~Monsters~and~10~Hill~Monsters~L73.~Go~report~to~Han~Cheng.");

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1466, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You've killed a Hill Monster.", MsgMessage.ChatMode.System);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1466, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1466, 50))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~50~Hill~Monsters.~Go~report~to~General~ZhuGe.");

                                }

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1467, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~Hill~Monster~and~received~a~Hammer.");
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711344;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711344, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1474, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~monster~and~received~a~Heavy~Axe.");
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711415;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711415, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1465, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1465, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1465, 20))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~20~Hill~Monsters.~Go~report~to~General~ZhuGe.");

                                }
                            }
                        }
                        else if (Family.ID == 73)
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1464, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1464, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1464, 0, 10))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~40~Sand~Monsters~and~10~Level~68~Sand~Monsters.~Go~report~to~KeYulun.");
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1463, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1463, 1);
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1460, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.SendSysMesage("You've killed a Level 68 Sand Monster.", MsgMessage.ChatMode.System);

                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1460, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1460, 30))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~30~Level~68~Sand~Monsters.~Go~report~to~the~DC~Vice~Captain.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1461, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    killer.CreateBoxDialog("You`ve~killed~the~Level~68~Sand~Monster~and~received~a~Sand~Essence.");

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711343;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711343, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else if (killer.Player.Map == 1020)
                    {
                        if (Family.ID == 14337)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1347, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1347, 1);


                            }
                        }
                        if (Family.ID == 14338)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1347, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1347, 0, 1);


                            }
                        }
                        if (Family.ID == 3050)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1346, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1346, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1346, 0, 40))
                                {
                                    killer.SendSysMesage("You killed 40 Heresy Snakeman.", MsgMessage.ChatMode.System);

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1345, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1345, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1345, 0, 20))
                                {
                                    killer.SendSysMesage("You killed 20 Heresy Snakeman.", MsgMessage.ChatMode.System);

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1359, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1359, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1359, 50))
                                {
                                    killer.SendSysMesage("You killed 50 Heresy Snakeman.");
                                }

                            }
                        }
                        if (Family.ID == 72)//Snakeman lev 63
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1341, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720858;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720858, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1343, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1343, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1343, 50))
                                {
                                    killer.SendSysMesage("You killed 50 Snakemen L63.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1334, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720855;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720855, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }
                        }
                        if (Family.ID == 13)//Snakeman
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1346, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1346, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1346, 40))
                                {
                                    killer.SendSysMesage("You killed 40 Snakeman.", MsgMessage.ChatMode.System);

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1345, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1345, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1345, 20))
                                {
                                    killer.SendSysMesage("You killed 20 Snakeman.", MsgMessage.ChatMode.System);

                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1360, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1360, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1360, 60))
                                {
                                    killer.SendSysMesage("You killed 60 Snakeman.", MsgMessage.ChatMode.System);

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1341, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720857;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720857, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1337, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.CreateBoxDialog("You~received~the~Cloud~Riding~Scroll!");

                                    if (killer.Inventory.Contain(720856, 1))
                                        killer.Inventory.Add(stream, 720856);
                                }
                                else
                                {
                                    killer.SendSysMesage("Please make 1 more space in your inventory.");

                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1335, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1335, 1);
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1333, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720854;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720854, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }
                        }
                        if (Family.ID == 71)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1325, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1325, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1325, 50))
                                {
                                    killer.SendSysMesage("You killed 50 Thunder Apes L58.", MsgMessage.ChatMode.System);

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1328, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 720850;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(720850, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            killer.SendSysMesage("A strange item dropped on the ground! Pick it up to see that is! ", MsgMessage.ChatMode.System);

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }

                            }
                        }
                        if (Family.ID == 12)// thunder
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1327, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(20))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711353;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(711353, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            killer.SendSysMesage("A strange item dropped on the ground! Pick it up to see that is! ", MsgMessage.ChatMode.System);

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1324, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1324, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1324, 60))
                                {
                                    killer.SendSysMesage("You killed 60 Thunder Apes.", MsgMessage.ChatMode.System);

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1329, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(25))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1329, 1);
                                        killer.Inventory.Add(stream, 720851);
                                        killer.CreateBoxDialog("You received the Blaze Pill!");

                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");

                                    }
                                }

                            }
                        }
                        if (Family.ID == 14336)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1356, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1356, 1);

                            }
                        }
                        if (Family.ID == 1)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1307, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1307, 1);
                                killer.CreateBoxDialog("You~killed~Pheasant~and~scared~the~Apes~away!");

                            }
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 824, 1);
                        }
                        if (Family.ID == 678000)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1351, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1351, 1);
                            }
                        }
                        if (Family.ID == 11)//giant ape
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1363, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    if (killer.Inventory.HaveSpace(1))
                                    {


                                        if (killer.Player.QuestGUI.CheckObjectives(1363, 1) == false)
                                        {
                                            killer.SendSysMesage("You received 1 Ape Claw.", MsgMessage.ChatMode.System);

                                            killer.Inventory.Add(stream, 711357, 1);
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1363, 1);
                                        }
                                        else if (killer.Player.QuestGUI.CheckObjectives(1363, 0, 1) == false)
                                        {
                                            killer.SendSysMesage("You received 1 Ape Bone.", MsgMessage.ChatMode.System);

                                            killer.Inventory.Add(stream, 711358, 1);
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1363, 0, 1);
                                        }
                                        else if (killer.Player.QuestGUI.CheckObjectives(1363, 0, 0, 1) == false)
                                        {
                                            killer.SendSysMesage("You received 1 Ape Skin.", MsgMessage.ChatMode.System);

                                            killer.Inventory.Add(stream, 711359, 1);
                                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1363, 0, 0, 1);
                                        }
                                    }
                                    else
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");

                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1316, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1316, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1316, 50))
                                {
                                    killer.SendSysMesage("You killed 50 Giant Apes.");

                                }
                            }
                        }
                        if (Family.ID == 33212)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1756, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Inventory.Add(stream, 721706);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1756, 1);
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.GuardLi, killer.Player.Class, 1756);
                                killer.Player.QuestGUI.SendAutoPatcher("You~killed~the~monster~and~received~a~Paper~Figure!~Take~it~to~General~Zhao!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                            }
                        }
                        if (Family.ID == 70)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1753, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1753, 0, 0, 1);
                                killer.SendSysMesage("You,ve~collected~some~smells.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                if (killer.Player.QuestGUI.CheckObjectives(1753, 0, 0, 10))
                                {
                                    killer.CreateBoxDialog("You,ve collected the smell of Gaint Ape!");

                                }
                            }
                        }
                        if (Family.ID == 13)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1753, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1753, 1);
                                killer.SendSysMesage("You,ve collected the smell of Snake Man!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1783, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(15))
                                {
                                    killer.Inventory.Add(stream, 729978);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1783, 1);
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.GeneralJudd, killer.Player.Class, 1783);
                                    killer.Player.QuestGUI.SendAutoPatcher("You,ve collected the smell of Snake Man!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

                                }
                            }
                        }
                        if (Family.ID == 14334)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1749, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1749, 0, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1749, 0, killer.Player.QuestMultiple * 5) && killer.Player.QuestGUI.CheckObjectives(1749, 1) == false)
                                {
                                    killer.Player.QuestMultiple = 1;
                                    Database.Server.AddMapMonster(stream, killer.Map, 14334, killer.Player.X, killer.Player.Y, 7, 7, 5, 0, true);
                                }
                                if (Role.Core.Rate(20))
                                {
                                    if (killer.Player.QuestGUI.CheckObjectives(1749, 1) == false)
                                    {
                                        killer.CreateBoxDialog("The Ape Leader has appeared! Be extra cautious!");

                                        Database.Server.AddMapMonster(stream, killer.Map, 14335, killer.Player.X, killer.Player.Y, 7, 7, 1, 0, true, MsgItemPacket.EffectMonsters.Night);
                                    }
                                }
                            }
                        }
                        if (Family.ID == 14335)
                        {
                            Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(killer, stream);
                            dialog.AddText("Amazing! You now have the treasure those Venomous Apes were guarding -the Devilish Sword.")
                              .FinalizeDialog();

                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1749, 1);
                            killer.Inventory.Remove(721701, 1, stream);
                            if (!killer.Inventory.Contain(721702, 1))
                                killer.Inventory.Add(stream, 721702);
                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.DoctorLi, killer.Player.Class, 1749);
                            killer.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        }
                        if (Family.ID == 10)//macaque
                        {

                            if (killer.Player.QuestGUI.CheckQuest(1311, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(30))
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = 711352;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(729976, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1303, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1303, 1);
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1314, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1314, 1);
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1746, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (Role.Core.Rate(15))
                                {
                                    killer.Inventory.Add(stream, 721699);
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1746, 1);
                                    killer.CreateBoxDialog("You`ve~found~a~Decayed~Heart!");

                                }
                            }
                        }
                        if (Family.ID == 8305)//monkeyking
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1305, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1305, 1);
                                killer.CreateBoxDialog("You~received~the~Monkey~King~Tail!");

                                killer.Inventory.Add(stream, 720863);
                            }
                        }
                        if (Family.ID == 69)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1304, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1304, 1);
                            }
                        }
                        if (Family.ID == 11223)//vulture
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1306, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (killer.Inventory.HaveSpace(1))
                                {
                                    killer.Inventory.Add(stream, 729976);
                                    killer.CreateBoxDialog("You~received~a~Vulture~Feather.~Climb~up~to~the~City~Wall~at~(554,606)~to~burn~it!");

                                }
                                else
                                {
                                    killer.CreateBoxDialog("Please make 1 more space in your inventory.");

                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1301, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1301, 1);
                            }
                            if (Role.Core.Rate(15))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1736, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1736, 1, 1);
                                    killer.CreateBoxDialog("You`ve~found~a~Vulture~Feather.");

                                }

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = 729976;//birdclaw
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(729976, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }

                            }
                        }
                    }
                    else if (killer.Player.Map == 1011)//phoenix
                    {

                        if (Family.ID == 106)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1753, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1753, 0, 1);
                                killer.SendSysMesage("You,ve~collected~some~smells.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                if (killer.Player.QuestGUI.CheckObjectives(1753, 0, 10))
                                {

                                    killer.CreateBoxDialog("You,ve collected the smell of Rock Monster!");

                                }
                            }
                        }

                        if (Family.ID == 1403)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1725, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                killer.Inventory.Add(stream, 729973);
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1725, 1, 1);
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.VeteranHong, killer.Player.Class, 1725);
                                killer.Player.QuestGUI.SendAutoPatcher("You~killed~Cloud~the~Lustful~and~found~a~Wooden~Box!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                            }
                        }
                        else if (Family.ID == 11007)
                        {
                            if (Role.Core.Rate(4))
                            {
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = 721263;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(729094, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1103, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1103, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1103, 10))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.YuJing, killer.Player.Class, 1103);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~10~Bandits.~Go~tell~Yu~Jing~about~it.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1720, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1720, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1720, 5))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.ThiefChen, killer.Player.Class, 1720);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~5~Bandits.~Now~go~find~Thief~Chen!.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                                }
                            }
                        }
                        else if (Family.ID == 7)//bandit
                        {
                            if (Role.Core.Rate(15))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1724, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1724, 1);
                                    killer.Inventory.AddItemWitchStack(729971, 0, 1, stream);
                                    if (killer.Player.QuestGUI.CheckObjectives(1724, 1))
                                    {
                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.VeteranHong, killer.Player.Class, 1724);
                                        killer.Player.QuestGUI.SendAutoPatcher("Congratulations!~You~found~an~invitation~to~the~Treasure~Appraisal~Meeting.~Open~your~inventory~and~read~it.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                                    }
                                }
                            }

                            if (killer.Player.QuestGUI.CheckQuest(1114, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1114, 1);
                                killer.Inventory.AddItemWitchStack(711322, 0, 1, stream);
                                killer.SendSysMesage("You've killed the Bandit and received a bag of grain!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);


                                if (killer.Player.QuestGUI.CheckObjectives(1114, 10))
                                {
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.ForestGuvernor, killer.Player.Class, 1114);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~collected~the~bags~of~10~Bandits.~Hurry~to~report~back~to~the~ForestGuvernor!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                                }
                            }

                        }
                        else if (Family.ID == 66)//banditL33
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1115, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1115, 1);
                                    killer.Inventory.AddItemWitchStack(720837, 0, 1, stream);
                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)MsgNpc.NpcID.IntelligenceAgent, killer.Player.Class, 1115);
                                    killer.Player.QuestGUI.SendAutoPatcher("You~killed~the~Bandit~and~found~a~Bandit`s~Coat!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

                                }
                            }
                        }
                        else if (Family.ID == 11008)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1118, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1118, 1);
                                killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~the~Bandit~Leader.~Hurry~and~return~to~the~Bandit~Boss.", 1011, 227, 404, 8272);

                            }
                        }
                        else if (Family.ID == 8)//Ratlings
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1142, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1142, 1);
                                    killer.Inventory.AddItemWitchStack(711334, 0, 1, stream);
                                    killer.SendSysMesage("You received a Fury Core!", MsgMessage.ChatMode.System);

                                    if (killer.Player.QuestGUI.CheckObjectives(1142, 5))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.MilitiaLeopard, killer.Player.Class, 1142);
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~5~Fury~Cores!~Hurry~to~deliver~them~to~Militia~Leopard!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1741, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1741, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1741, 20))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.DealerShen, killer.Player.Class, 1741);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~20~Rats!~Now~you~can~go~report~to~Dealer~Shen~(779,477).", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                }
                            }



                            if (Role.Core.Rate(10))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1729, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1729, 0, 1);
                                    killer.Inventory.AddItemWitchStack(721696, 0, 1, stream);
                                    killer.CreateBoxDialog("You`ve~killed~the~Level~38~Fire~Rat~and~found~a~Ratling~Meat!");

                                }
                            }
                            if (Role.Core.Rate(15))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1729, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1729, 1, 0);
                                    killer.Inventory.AddItemWitchStack(721695, 0, 1, stream);
                                    killer.CreateBoxDialog("You~found~a~Ratling~Eye~on~the~Ratling`s~body.");

                                }
                            }
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1121, 1);
                            if (killer.Player.QuestGUI.CheckObjectives(1121, 10))
                            {
                                var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.BoldBoy, killer.Player.Class, 1121);
                                killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~10~Ratlings.~Go~tell~Bold~Boy~about~it", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                            }
                            if (killer.Player.QuestGUI.CheckQuest(1127, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1127, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1127, 10))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.VillageHead, killer.Player.Class, 1127);
                                    killer.Player.QuestGUI.SendAutoPatcher("You~have~killed~10~Ratlings!~Go~report~to~the~Village~Head!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                }
                            }
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1123, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1123, 1);
                                    killer.Inventory.AddItemWitchStack(711325, 0, 1, stream);
                                    if (killer.Player.QuestGUI.CheckObjectives(1123, 10))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.FurDealer, killer.Player.Class, 1123);
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~pieces~Ratling~Fur.~Go~deliver~them~to~the~Fur~Dealer!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                    }
                                }
                            }
                        }
                        else if (Family.ID == 6)
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(401, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    if (killer.Inventory.Contain(710220, 5))
                                    {
                                        killer.SendSysMesage("You`ve obtained 5 pieces of Snake Meat, already.");
                                    }
                                    else
                                    {
                                        if (killer.Inventory.HaveSpace(1))
                                        {
                                            killer.Inventory.AddItemWitchStack(710220, 0, 1, stream);
                                            killer.SendSysMesage("You received a piece of Snake Meat!");
                                        }
                                        else
                                        {
                                            killer.SendSysMesage("Your inventory is full!");
                                        }
                                    }
                                }
                                if (killer.Player.QuestGUI.CheckQuest(1107, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1107, 1);
                                    killer.Inventory.AddItemWitchStack(711321, 0, 1, stream);
                                    killer.SendSysMesage("You've killed the Winged Snake and received a Snake Scale!", MsgMessage.ChatMode.System);

                                    if (killer.Player.QuestGUI.CheckObjectives(1107, 10))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.HunterWong, killer.Player.Class, 1107);
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~Snake~Scales!~Now~go~deliver~them~to~Hunter~Wong!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                    }
                                }
                                if (killer.Player.QuestGUI.CheckQuest(1125, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    if (killer.Player.QuestGUI.CheckObjectives(1125, 1) == false)
                                    {
                                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1125, 1);
                                        killer.Inventory.AddItemWitchStack(711327, 0, 1, stream);
                                        killer.SendSysMesage("You've killed the Winged Snake and received a Snake Meat!", MsgMessage.ChatMode.System);


                                    }
                                }

                            }
                        }
                        else if (Family.ID == 3031)//cateran
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1128, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1128, 1);
                                    if (killer.Player.QuestGUI.CheckObjectives(1128, 1))
                                    {
                                        killer.Inventory.Add(stream, 711329);
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.VillageHead, killer.Player.Class, 1128);

                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~received~a~Frost~Token!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                    }
                                }
                            }
                        }
                        else if (Family.ID == 8303)
                        {
                            if (killer.Player.QuestGUI.CheckQuest(1129, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1129, 1);
                                if (killer.Player.QuestGUI.CheckObjectives(1129, 1))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.VillageHead, killer.Player.Class, 1129);
                                    killer.Player.QuestGUI.SendAutoPatcher("You~defeated~the~Ratling~King!~Report~your~victory~to~the~Village~Head.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                }
                            }
                        }
                        else if (Family.ID == 113)
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1135, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1135, 1, 1);
                                    killer.Inventory.AddItemWitchStack(711332, 0, 1, stream);
                                    killer.SendSysMesage("You received a Fire Spirit Tooth.", MsgMessage.ChatMode.System);

                                    if (killer.Player.QuestGUI.CheckObjectives(1135, 10))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.MilitiaTiger, killer.Player.Class, 1135);
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~10~Spirit~Teeth!~Go~deliver~them~to~Militia~Tiger!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                    }
                                }
                            }
                            if (killer.Player.QuestGUI.CheckQuest(1144, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1144, 1);
                                killer.SendSysMesage("You received a Fire Spirit Tooth.", MsgMessage.ChatMode.System);

                                if (killer.Player.QuestGUI.CheckObjectives(1144, 15))
                                {
                                    var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.WarrantOfArrest, killer.Player.Class, 1144);
                                    killer.Player.QuestGUI.SendAutoPatcher("You`ve~killed~15~Fire~Spirits!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                }
                            }

                            //WarrantOfArrest, client.Player.Class, 1144);
                            //711332
                        }
                        else if (Family.ID == 68)
                        {
                            if (Role.Core.Rate(40))
                            {
                                if (killer.Player.QuestGUI.CheckQuest(1142, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 1142, 1);
                                    killer.Inventory.AddItemWitchStack(711334, 0, 1, stream);

                                    killer.SendSysMesage("You received a Fury Core!", MsgMessage.ChatMode.System);
                                    if (killer.Player.QuestGUI.CheckObjectives(1142, 1))
                                    {
                                        var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.MilitiaLeopard, killer.Player.Class, 1142);
                                        killer.Player.QuestGUI.SendAutoPatcher("You`ve~gathered~5~Fury~Cores!~Hurry~to~deliver~them~to~Militia~Leopard!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                                    }
                                }
                            }
                        }
                    }
                jmp:
                    if (killer.Player.OnXPSkill() == MsgUpdate.Flags.SuperCyclone || killer.Player.OnXPSkill() == MsgUpdate.Flags.Cyclone
                           || killer.Player.OnXPSkill() == MsgUpdate.Flags.Superman)
                    {
                        killer.Player.XPCount++;
                        killer.Player.KillCounter++;

                        if (killer.Player.OnXPSkill() != MsgServer.MsgUpdate.Flags.Normal)
                        {

                            action.KillCounter = killer.Player.KillCounter;
                            killer.Player.UpdateXpSkill();
                        }

                    }
                    else if (killer.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
                    {
                        killer.Player.XPCount++;
                    }
                    else if (killer.Player.OnXPSkill() != MsgUpdate.Flags.BladeFlurry)
                    {
                        if (killer.Player.OnXPSkill() != MsgUpdate.Flags.Omnipotence)
                        {
                            killer.Player.KillCounter++;
                            if (killer.Player.KillCounter % 4 == 0)
                                killer.Player.XPCount++;
                        }
                    }

                }
                Send(stream.InteractionCreate(&action));
                if (RemoveOnDead)
                {
                    AddFlag(MsgUpdate.Flags.FadeAway, 10, false);
                    GMap.View.LeaveMap<Role.IMapObj>(this);
                    if (GMap.IsFlagPresent(X, Y, Role.MapFlagType.Monster))
                        GMap.cells[X, Y] &= ~Role.MapFlagType.Monster;

                    if (killer != null)
                    {
                        if (killer.Player.TOM_StartChallenge)
                        {
                            bool finished = true;
                            foreach (var mob in killer.Player.View.Roles(Role.MapObjectType.Monster))
                                if (mob.Alive)
                                    finished = false;
                            if (finished)
                            {

                                if (killer.Player.TOMChallengeToday == 0)
                                {
                                    if (killer.Player.MyTowerOfMysteryLayer <= killer.Player.JoinTowerOfMysteryLayer)
                                        killer.Player.MyTowerOfMysteryLayer = (byte)Math.Min(killer.Player.MyTowerOfMysteryLayer + 1, 9);
                                }
                                else
                                {

                                    if (killer.Player.MyTowerOfMysteryLayerElite <= killer.Player.JoinTowerOfMysteryLayer)
                                        killer.Player.MyTowerOfMysteryLayerElite = (byte)Math.Min(killer.Player.MyTowerOfMysteryLayerElite + 1, 9);
                                }
                                killer.CreateBoxDialog("You`ve successfully defeated the devil on Tower of Mystery " + (killer.Player.JoinTowerOfMysteryLayer + 1).ToString() + "F. Hurry and go claim the Bright Tribe`s reward for you.");
                                killer.Player.TOM_FinishChallenge = true;
                                foreach (var npc in killer.Player.View.Roles(Role.MapObjectType.Npc))
                                    killer.Player.View.SendView(stream.NpcCreate(npc as Npc, 40150), true);
                            }
                        }
                    }
                }
                if (Map == 3935)
                    return;
                if (checkupDropped)
                    return;
                if (killer != null)
                {
                    if (Map == 2022)//dis city map 2
                    {
                        killer.Player.KillersDisCity += 1;
                    }
                    if (Map == 2024)// dis city map 4
                    {
                        if (Family.ID == 66432)//ultimate pluto
                        {

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = 790001;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(3004181, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                            MsgTournaments.MsgSchedules.DisCity.KillTheUltimatePluto(killer);

                        }
                    }
                    if (Map == 1081)
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.BattleField
                            && Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                            killer.Player.BattleFieldPoints += 10;
                        killer.SendSysMesage("You received 10 BattlePoints.");

                    }
                    if (Map == 1080)
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.BattleField
                            && Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                            killer.Player.BattleFieldPoints += 20;
                        killer.SendSysMesage("You received 20 BattlePoints.");

                    }
                    if (Map == 2060)
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.BattleField
                             && Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                            killer.Player.BattleFieldPoints += 30;
                        killer.SendSysMesage("You received 30 BattlePoints.");

                    }
                  
                    if (Map == 3846)
                    {
                        uint PostionDrop = Family.ItemGenerator.GeneratePotionExtra();
                        if (Role.Core.Rate(40))
                        {
                            if (PostionDrop != 0)
                            {
                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                {
                                    DropItem(stream, killer.Player.UID, killer.Map, PostionDrop, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                }
                            }
                        }
                        if (Family.ID == 20211)//box;
                        {
                            for (int x = 0; x < 2; x++)
                            {
                                PostionDrop = Family.ItemGenerator.GeneratePotionExtra(true);
                                if (PostionDrop != 0)
                                {
                                    ushort xx = X;
                                    ushort yy = Y;
                                    if (killer.Map.AddGroundItem(ref xx, ref yy))
                                    {
                                        DropItem(stream, killer.Player.UID, killer.Map, PostionDrop, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                    }
                                }
                            }
                        }

                    
                        
                    }
                    else if (Map == 1011 || Map == 1020)
                    {
                        if (Family.ID == 3130 || Family.ID == 3134)//titan/ ganoderma
                        {
                            if (Role.Core.Rate(50))
                            {
                                uint[] DropSpecialItems = new uint[] { Database.ItemType.MoonBox, Database.ItemType.PowerExpBall, Database.ItemType.DragonBall, 
                                };

                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 4))
                                {
                                    uint IDDrop = DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)];
                                    DropItem(stream, killer.Player.UID, killer.Map, DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)], xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);

                                    string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                                    SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

                                }

                            }
                        }
                    }
                    if (Boss > 0 && Family.ID == 20100)//NightmareCaptain
                    {
                        uint ItemID = 3004465;
                        if (Role.Core.Rate(20))
                        {
                            ItemID = 3004463;
                            killer.CreateBoxDialog("You've found an YinYangSeal.");

                        }
                        else
                        {
                            killer.CreateBoxDialog("You wasn`t very lucky today .. I hope you will have a better luck next time ^^.");

                        }
                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }
                        return;
                    }
                    if (Boss > 0 && Family.ID == 20101 && Map == 3851)//PurpleBanshee
                    {
                        uint ItemID = 3004465;
                        if (Role.Core.Rate(20))
                        {
                            ItemID = 3004464;
                            killer.CreateBoxDialog("You've found an Life`sEye.");
                            SendSysMesage("The PurpleBanshee has been destroyed by " + killer.Player.Name.ToString() + ", he got the Life`sEye", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                        }
                        else
                        {
                            killer.CreateBoxDialog("You wasn`t very lucky today .. I hope you will have a better luck next time ^^.");

                        }
                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }
                        return;
                    }
                    if (Boss > 0 && Family.ID == 20101 && Map == 3825)//PurpleBanshee
                    {
                        //3003340 SolarBlade
                        uint ItemID = 3003340;
                        killer.CreateBoxDialog("You've found an SolarBlade.");
                        SendSysMesage("The PurpleBanshee has been destroyed by " + killer.Player.Name.ToString() + ", he got the SolarBlade", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 5))
                        {
                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }
                        return;
                    }
                    
                
                    if (killer.Player.Map == 1001 && Family.ID == 40937)//LadyqUEST
                    {
                        //camilasans
                        uint Items = 720957;
                        killer.Inventory.Add(stream, Items);
                        killer.Teleport(276, 360, 1002);
                    }
                    if (Boss > 0 && Family.ID == 213883)//chaos Guard
                    {
                        const ushort GetStudyPoints = 0;

                        //150059 GoldRing
                        //120049 ThreadNecklace
                        uint[] ItemsIDS = new uint[] { 3003235, 3005126 /*chi 500*/
                        , 3001303, 3001304};

                        {
                            uint ItemId = ItemsIDS[Program.GetRandom.Next(0, ItemsIDS.Length)];

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ItemId;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ItemId, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;


                            if (ItemId == 150059)
                                DataItem.Effect = Role.Flags.ItemEffect.Stigma;

                            if (ItemId == 120049)
                                DataItem.Effect = Role.Flags.ItemEffect.Shield;

                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    killer.HeroRewards.AddGoal(709);
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        Database.ItemType.DBItem aitem;
                        aitem = PickDropGarment(3);
                        if (aitem != null)
                        {
                            {
                                {
                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = aitem.ID;
                                    Database.ItemType.DBItem DBItem;
                                    DataItem.EndDate = DateTime.Now.AddDays(2);
                                    if (Database.Server.ItemsBase.TryGetValue(Database.ItemType.DragonBall, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                        }
                        //generate random items.....
                        for (int x = 0; x < 2; x++)
                        {

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = Database.ItemType.DragonBall;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(Database.ItemType.DragonBall, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 5, Y + 5);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        if (killer.Team != null)
                        {
                            foreach (var member in killer.Team.Temates)
                            {
                                if (member != null && member.client != null && member.client.Player != null && member.client.Player.SubClass != null)
                                {
                                    if (member.client.Player.UID != killer.Player.UID)
                                    {
                                        member.client.Player.SubClass.AddStudyPoints(member.client, GetStudyPoints, stream);
                                    }
                                }
                            }
                        }
                        if (killer.Player.SubClass != null)
                            killer.Player.SubClass.AddStudyPoints(killer, GetStudyPoints, stream);
                        SendBossSysMesage(killer.Player.Name, GetStudyPoints, MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);


                        return;
                    }


                  
                    
                    if (Boss > 0 && Family.ID == 6643)
                    {
                    
                        List<uint> DropIems = Family.ItemGenerator.GenerateSoulsItems(3);
                        foreach (var ids in DropIems)
                        {
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ids;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);

                                  
                                }
                            }
                        }
                        return;
                    }
                    if ((Family.Settings & MonsterSettings.DropItemsOnDeath) == MonsterSettings.DropItemsOnDeath)
                    {
                        if (Family.ID == 23973 || Family.ID == 22973 || Family.ID == 23974 || Family.ID == 3969)
                            return;
                        if (Family.MaxHealth > 100000 && Family.MaxHealth < 7000000 || Boss == 1)
                        {
                            List<uint> DropIems = Family.ItemGenerator.GenerateBossFamily();
                            foreach (var ids in DropIems)
                            {
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = ids;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = X;
                                ushort yy = Y;
                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                            return;
                        }
                        ushort rand = (ushort)(killer.Player.MyRandom.Next() % 1000);
                        byte count = 1;//(byte)(rand % 3);
                        if (Map == 1700)
                        {
                            if (rand > 100 && rand < 200)
                            {
                                switch (rand % 4)
                                {

                                    case 0:
                                        {
                                            ushort xx = X;
                                            ushort yy = Y;
                                            for (byte i = 0; i < count; i++)
                                            {
                                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                                {
                                                    uint ItemID = 722723;

                                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                                }
                                            }
                                            break;
                                        }
                                    case 2:
                                        {
                                            ushort xx = X;
                                            ushort yy = Y;
                                            for (byte i = 0; i < count; i++)
                                            {
                                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                                {
                                                    uint ItemID = 722724;

                                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                                }
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            ushort xx = X;
                                            ushort yy = Y;
                                            for (byte i = 0; i < count; i++)
                                            {
                                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                                {
                                                    uint ItemID = 722725;

                                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                                }
                                            }
                                            break;
                                        }
                                }

                            }
                            if (rand < 50)
                            {
                                ushort xx = X;
                                ushort yy = Y;
                                for (byte i = 0; i < count; i++)
                                {
                                    if (killer.Map.AddGroundItem(ref xx, ref yy))
                                    {
                                        uint ItemID = 722732;

                                        DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                    }
                                }
                            }
                            else if (rand > 300 && rand < 350)
                            {
                                ushort xx = X;
                                ushort yy = Y;
                                for (byte i = 0; i < count; i++)
                                {
                                    if (killer.Map.AddGroundItem(ref xx, ref yy))
                                    {
                                        uint ItemID = 722736;

                                        DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                    }
                                }
                            }

                        }
                        Database.ItemType.DBItem CPDBItem = null;
                        uint Value = 0;
                        uint CPPackID = Family.ItemGenerator.GenerateItemCPID(killer.Player.Map, out CPDBItem, out Value);
                        if (CPDBItem != null && Value != 0)
                        {
                            ushort xx = X;
                            ushort yy = Y;
                            if (killer.Player.VipLevel == 1)
                            {
                                killer.Player.ConquerPoints += Value;
                                killer.SendSysMesage("You have picked up a " + Value + " ConquerPoints.");
                                CPDBItem = null;
                                Value = 0;
                            }
                            else
                            {
                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                    DropItem(stream, killer.Player.UID, killer.Map, CPDBItem.ID, xx, yy, killer.Player.VipLevel > 4 ? MsgItem.ItemType.Cps : MsgItem.ItemType.Item, (uint)(killer.Player.VipLevel > 4 ? Value : 0), false, 0);
                            }
                        }
                     
                        else if (rand > 40 && rand < 60)
                        {
                            ushort xx = X;
                            ushort yy = Y;
                            for (byte i = 0; i < count; i++)
                            {
                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                {
                                    uint ItemID = 0;
                                    uint Amount = Family.ItemGenerator.GenerateGold(out ItemID, Map == 1700, Map == 1002);
                                  
                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, Amount, false, 0);
                                }
                            }
                        }
                        else if (rand > 500 && rand < 600)
                        {

                            ushort xx = X;
                            ushort yy = Y;
                            for (byte i = 0; i < count; i++)
                            {
                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                {
                                    uint ItemID = Family.DropHPItem;

                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                }
                            }
                        }
                        else if (rand > 600 && rand < 700)
                        {
                            ushort xx = X;
                            ushort yy = Y;
                            for (byte i = 0; i < count; i++)
                            {
                                if (killer.Map.AddGroundItem(ref xx, ref yy))
                                {
                                    uint ItemID = Family.DropMPItem;

                                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                                }
                            }
                        }
                        else if (rand > 700)//&& rand < 770)
                        {
                            ushort xx = X;
                            ushort yy = Y;
                            for (byte i = 0; i < count; i++)
                            {

                                Database.ItemType.DBItem DbItem = null;
                                byte ID_Quality;
                                bool ID_Special;
                                uint ID = Family.ItemGenerator.GenerateItemId(Map, out ID_Quality, out ID_Special, out DbItem);
                                if (ID != 0)
                                {
                                    if (killer.Map.AddGroundItem(ref xx, ref yy))
                                    {
                                        if (ID == 1088000)
                                        {




                                            ActionQuery action2;

                                            action2 = new ActionQuery()
                                            {
                                                ObjId = killer.Player.UID,
                                                Type = ActionType.DragonBall
                                            };
                                            killer.Send(stream.ActionCreate(&action2));


                                        }
                                        DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, ID_Special, ID_Quality, killer, DbItem);
                                        if (ID_Special)
                                            break;
                                    }
                                }

                            }
                        }

                    }
                }
            }

        }
    }
}
