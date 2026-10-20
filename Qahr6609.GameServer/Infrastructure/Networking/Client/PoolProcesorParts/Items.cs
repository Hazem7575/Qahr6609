using Conquer6609.Database;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgLoader;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.AttackHandler;
using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Client
{
    public partial class PoolProcesses
    {
        public static unsafe void CheckItems(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null)
                    return;

                Extensions.Time32 Now = Extensions.Time32.Now;
                foreach (var item in client.Player.View.Roles(Role.MapObjectType.Item))
                {
                    if (item.Alive == false)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var PItem = item as Game.MsgFloorItem.MsgItem;
                            if (PItem.IsTrap())
                            {
                                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == Game.MsgTournaments.ProcesType.Alive)
                                {
                                    if (PItem.ItemBase.ITEM_ID == Game.MsgFloorItem.MsgItemPacket.DBShowerEffect)
                                    {

                                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.DBShower)
                                        {
                                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgDBShower;
                                            tournament.DropDragonBall(PItem.X, PItem.Y, stream);
                                        }

                                    }
                                }
                                PItem.SendAll(stream, MsgDropID.RemoveEffect);
                            }
                            else
                                PItem.SendAll(stream, MsgDropID.Remove);
                            client.Map.View.LeaveMap<Role.IMapObj>(PItem);
                        }
                    }
                    else if (item.IsTrap())
                    {
                        if (client.Player.Map == 4006 && client.Player.JoinTowerOfMysteryLayer == 7)
                        {
                            if (!(Role.Core.GetDistance(client.Player.X, client.Player.Y, 44, 62) <= 3))
                            {
                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, item.X, item.Y) <= 2)
                                {

                                    if (DateTime.Now > client.Player.TowerOfMysteryFrezeeStamp)
                                    {
                                        client.Player.AddFlag(MsgUpdate.Flags.Freeze, 3, true);
                                        client.Player.TowerOfMysteryFrezeeStamp = DateTime.Now.AddSeconds(5);
                                    }

                                }
                                foreach (var user in client.Player.View.Roles(Role.MapObjectType.Player))
                                {
                                    if (Role.Core.GetDistance(user.X, user.Y, item.X, item.Y) <= 2)
                                    {
                                        var _user = user as Role.Player;
                                        if (DateTime.Now > _user.TowerOfMysteryFrezeeStamp)
                                        {
                                            _user.AddFlag(MsgUpdate.Flags.Freeze, 3, true);
                                            _user.TowerOfMysteryFrezeeStamp = DateTime.Now.AddSeconds(5);
                                        }

                                    }
                                }
                            }
                        }
                        var FloorItem = item as Game.MsgFloorItem.MsgItem;
                        if (FloorItem.ItemBase == null)
                            continue;
                        if (FloorItem.ItemBase.ITEM_ID == Game.MsgFloorItem.MsgItemPacket.SoulTwoDaggerStorm || FloorItem.ItemBase.ITEM_ID == Game.MsgFloorItem.MsgItemPacket.SoulOneDaggerStorm || FloorItem.ItemBase.ITEM_ID == Game.MsgFloorItem.MsgItemPacket.NormalDaggerStorm)
                        
                        {
                            if (Now > FloorItem.AttackStamp.AddMilliseconds(1))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    foreach (var _monster in FloorItem.GMap.View.Roles(Role.MapObjectType.Monster, FloorItem.X, FloorItem.Y
                                        , p => Role.Core.GetDistance(p.X, p.Y, FloorItem.MsgFloor.m_X, FloorItem.MsgFloor.m_Y) <= 3))
                                    {
                                        var monster = _monster as Game.MsgMonster.MonsterRole;
                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(FloorItem.OwnerEffert, monster, FloorItem.DBSkill))
                                        {
                                            InteractQuery action = new InteractQuery()
                                            {
                                                AtkType = MsgAttackPacket.AttackID.Physical,
                                                X = monster.X,
                                                Y = monster.Y,
                                                OpponentUID = monster.UID
                                            };


                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                            Game.MsgServer.AttackHandler.Calculate.Range.OnMonster(FloorItem.OwnerEffert.Player, monster, FloorItem.DBSkill, out AnimationObj);
                                            AnimationObj.Damage = AnimationObj.Damage * 50 / 100;
                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, FloorItem.OwnerEffert, monster);
                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, FloorItem.SpellSoul);

                                            action.Damage = (int)AnimationObj.Damage;
                                            action.Effect = AnimationObj.Effect;
                                            monster.Send(stream.InteractionCreate(&action));

                                        }
                                    }
                                    foreach (var player in FloorItem.GMap.View.Roles(Role.MapObjectType.Player, FloorItem.X, FloorItem.Y
                                        , p => Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(p.X, p.Y, FloorItem.MsgFloor.m_X, FloorItem.MsgFloor.m_Y) <= 3))
                                    {
                                        if (player.UID != FloorItem.OwnerEffert.Player.UID)
                                        {
                                            var atacked = player as Role.Player;
                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(FloorItem.OwnerEffert, atacked, FloorItem.DBSkill))
                                            {

                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(FloorItem.OwnerEffert.Player, atacked, FloorItem.DBSkill, out AnimationObj);
                                                AnimationObj.Damage = AnimationObj.Damage * 50 / 100;
                                                Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, FloorItem.OwnerEffert, atacked);
                                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, FloorItem.SpellSoul);

                                                InteractQuery action = new InteractQuery()
                                                {
                                                    AtkType = MsgAttackPacket.AttackID.Physical,
                                                    X = atacked.X,
                                                    Y = atacked.Y,
                                                    OpponentUID = atacked.UID
                                                };

                                                action.Damage = (int)AnimationObj.Damage;
                                                action.Effect = AnimationObj.Effect;

                                                atacked.View.SendView(stream.InteractionCreate(&action), true);

                                            }
                                        }

                                    }
                                }
                                FloorItem.AttackStamp = Now;
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
        public static void CheckItemsTime(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null)
                    return;
                // Allocation-free scan; expired items are collected first and handled exactly as before.
                List<MsgGameItem> expired = null;
                client.CollectExpiredTimeItems(DateTime.Now, ref expired);
                if (expired == null)
                    return;
                foreach (var item in expired)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (client.Inventory.ClientItems.ContainsKey(item.UID))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            continue;
                        }
                        if (client.Equipment.ClientItems.ContainsKey(item.UID))
                        {
                            Role.Flags.ConquerItem position = (Role.Flags.ConquerItem)Database.ItemType.ItemPosition(item.ITEM_ID);
                            client.Equipment.Remove(position, stream);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }
                        if (client.MyWardrobe.ContainUID(item.UID))
                        {
                            MsgGameItem items2;
                            if (client.MyWardrobe.RemoveItem(item.UID, out items2))
                            {
                                if (item.IsEquip)
                                {
                                    client.Equipment.Remove((Role.Flags.ConquerItem)Database.ItemType.ItemPosition(item.ITEM_ID), stream);
                                    Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                    store.ActionID = MsgCoatStorage.Action.TakeOff;
                                    store.dwparam1 = items2.UID;
                                    store.dwpram2 = items2.ITEM_ID;
                                    client.Send(stream.CreateCoatStorage(store));
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                }
                                client.Inventory.Update(items2, Role.Instance.AddMode.REMOVE, stream);
                                Game.MsgServer.MsgCoatStorage.CoatStorage astore = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                astore.ActionID = MsgCoatStorage.Action.Expire;//official: the wardrobe drops the expired garment
                                astore.dwparam1 = items2.UID;
                                astore.dwpram2 = items2.ITEM_ID;
                                client.Send(stream.CreateCoatStorage(astore));
                            }
                        }
                        foreach (var Wh in client.Warehouse.ClientItems)
                        {
                            foreach (var item2 in Wh.Value.Values)
                            {
                                if (item2.UID == item.UID)
                                {
                                     client.Warehouse.DeleteItem(item2.UID, Wh.Key, stream);
                                   
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyConsole.WriteException(ex);
            }
        }
    }
}