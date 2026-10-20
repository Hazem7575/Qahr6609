using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgNpc;

namespace Conquer6609.Role
{
    public class RoleView
    {
        public const int ViewThreshold = 18;

        public Extensions.Time32 Monster_BuffersCallbackStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.AI_Buffers);
        public Extensions.Time32 Monster_GuardsCallbackStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.AI_Guards);
        public Extensions.Time32 Monster_AliveMonstersCallback = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.AI_Monsters);

        public Game.MsgMonster.ActionHandler MobActions = new Game.MsgMonster.ActionHandler();

        // [Performance] StanceCooldownMs: a stance/direction packet that repeats the player's current
        // stance/direction within this window is only echoed to the sender (nobody's view changes). 0 = off (default, original).
        public static readonly int StanceCooldownMs = Math.Max(0, Conquer6609.Framework.Settings.Int("Performance", "StanceCooldownMs", 0));
        // [Performance] PrayStanceLocal: CastPray stance copies go only to the players within 4 tiles
        // (each copy is addressed to one of them) instead of being re-broadcast to the whole screen. 0 = original (default).
        public static readonly bool PrayStanceLocal = Conquer6609.Framework.Settings.Int("Performance", "PrayStanceLocal", 0) != 0;
        public int StanceStamp;
        public int DirectionStamp;

        /// <summary>True when the request repeats the current value inside StanceCooldownMs (skip the broadcast).</summary>
        public bool SuppressRepeat(ref int stamp, bool sameAsCurrent)
        {
            int now = Environment.TickCount;
            if (StanceCooldownMs > 0 && sameAsCurrent && stamp != 0 && unchecked(now - stamp) < StanceCooldownMs)
                return true;
            stamp = now == 0 ? 1 : now;
            return false;
        }


        public Extensions.SafeDictionary<uint, IMapObj>[] src;
        public Client.GameClient Owner;
        public Role.Player GetPlayer()
        {
            return Owner.Player;
        }
        public RoleView(Client.GameClient _client)
        {
            Owner = _client;
            src = new Extensions.SafeDictionary<uint, IMapObj>[(byte)MapObjectType.Count];
            for (byte x = 0; x < (byte)MapObjectType.Count; x++)
                src[x] = new Extensions.SafeDictionary<uint, IMapObj>();
        }

        public void CheckUpMonsters(Extensions.Time32 clock)
        {
            if (Game.MsgMonster.PoolProcesses.MonsterAiMode >= 2)
            {
                // The three AI steps run on their own cadence instead of at most one of them per tick.
                if (clock > Monster_BuffersCallbackStamp)
                {
                    Game.MsgMonster.PoolProcesses.BuffersCallback(Owner, clock);
                    Monster_BuffersCallbackStamp.Value = clock.Value + MapGroupThread.AI_Buffers;
                }
                if (clock > Monster_GuardsCallbackStamp)
                {
                    Game.MsgMonster.PoolProcesses.GuardsCallback(Owner, clock);
                    Monster_GuardsCallbackStamp.Value = clock.Value + MapGroupThread.AI_Guards;
                }
                if (clock > Monster_AliveMonstersCallback)
                {
                    Game.MsgMonster.PoolProcesses.AliveMonstersCallback(Owner, clock);
                    Monster_AliveMonstersCallback.Value = clock.Value + MapGroupThread.AI_Monsters;
                }
                return;
            }
            if (clock > Monster_BuffersCallbackStamp)
            {
                Game.MsgMonster.PoolProcesses.BuffersCallback(Owner, clock);
                Monster_BuffersCallbackStamp.Value = clock.Value + MapGroupThread.AI_Buffers;
            }
            else if (clock > Monster_GuardsCallbackStamp)
            {
                Game.MsgMonster.PoolProcesses.GuardsCallback(Owner, clock);
                Monster_GuardsCallbackStamp.Value = clock.Value + MapGroupThread.AI_Guards;
            }
            else if (clock > Monster_AliveMonstersCallback)
            {
                Game.MsgMonster.PoolProcesses.AliveMonstersCallback(Owner, clock);
                Monster_AliveMonstersCallback.Value = clock.Value + MapGroupThread.AI_Monsters;
            }
        }


        public unsafe void ReSendView(ServerSockets.Packet stream)
        {
            SendView(Owner.Player.GetArray(stream,false), false);
        }
        public bool SameLocation(MapObjectType typ, out Role.IMapObj obj)
        {
            foreach (var client in Roles(typ))
            {
                if (client.X == GetPlayer().X && client.Y == GetPlayer().Y)
                {
                    obj = client;
                    return true;
                }
            }
            obj = null;
            return false;
        }

        public unsafe void SendView(ServerSockets.Packet msg, bool me)
        {

            if (me)
                Owner.Send(msg);
            bool hidden = Owner.Player.Invisible;
            foreach (IMapObj obj in Roles(MapObjectType.Player))
            {
                if (hidden && !SeesHidden((obj as Role.Player).Owner)) continue;// a hidden GM: what he does reaches staff only
                obj.Send(msg);
            }
        }
        public unsafe void SendView(byte[] msg, bool me)
        {

            if (me)
                Owner.Send(msg);
            bool hidden = Owner.Player.Invisible;
            foreach (IMapObj obj in Roles(MapObjectType.Player))
            {
                if (hidden && !SeesHidden((obj as Role.Player).Owner)) continue;
                (obj as Role.Player).Owner.Send(msg);
            }
        }
        /// <summary>Staff (GM / PM) see a hidden GM; players do not.</summary>
        public static bool SeesHidden(Client.GameClient c)
        {
            return c != null && (c.GameMaster || c.ProjectManager);
        }
        public IEnumerable<IMapObj> Roles(MapObjectType typ, Predicate<bool> P = null)
        {
            if (Owner.Map != null)
            {
                if (P != null)
                    return Owner.Map.View.Roles(typ, Owner.Player.X, Owner.Player.Y, p => CanSee(p) && P(p.Alive));
                else
                    return Owner.Map.View.Roles(typ, Owner.Player.X, Owner.Player.Y, p => CanSee(p));
            }
            else
                return new IMapObj[0];
        }
        public IEnumerable<IMapObj> AttackableRoles()
        {
            foreach (var rule in Owner.Map.View.Roles(MapObjectType.Monster, Owner.Player.X, Owner.Player.Y, x => CanSee(x)))
                yield return rule;
            foreach (var rule in Owner.Map.View.Roles(MapObjectType.Player, Owner.Player.X, Owner.Player.Y, x => CanSee(x)))
                yield return rule;
            foreach (var rule in Owner.Map.View.Roles(MapObjectType.SobNpc, Owner.Player.X, Owner.Player.Y, x => CanSee(x)))
                yield return rule;
        }
        public bool TryGetValue(uint UID, out IMapObj obj, MapObjectType typ)
        {
            if (Owner.Map != null)
                return Owner.Map.View.TryGetObject<IMapObj>(UID, typ, Owner.Player.X, Owner.Player.Y, out obj);
            obj = null;
            return false;
        }
        public bool CanSee(IMapObj obj)
        {
            try
            {
                if (obj == null)
                    return false;
                if (obj.Map != Owner.Player.Map)
                    return false;
                if (!obj.AllowDynamic)
                    if (obj.DynamicID != Owner.Player.DynamicID)
                        return false;
                if (obj.UID == Owner.Player.UID)
                    return false;
                if (!Co_Panle.On && obj.UID == 2000000000)
                {//لوحه الشات  اختفاء الجى ام

                    return false;
                }
                return Core.GetDistance(obj.X, obj.Y, Owner.Player.X, Owner.Player.Y) <= ViewThreshold;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                return false;
            }
        }
        public bool Contains(IMapObj obj)
        {
            if (obj.UID == Owner.Player.UID)
                return true;
            if (Owner.Map != null)
                return Owner.Map.View.Contain(obj.UID, Owner.Player.X, Owner.Player.Y);
            return false;
        }
        
        public bool ContainMobInScreen(string name)
        {
            bool contain = false;
            foreach (var obj in Roles(MapObjectType.Monster))
            {
                var monster = obj as Game.MsgMonster.MonsterRole;
                if (monster.Name == name)
                {
                    contain = true;
                    break;
                }
            }
            return contain;
        }
        public unsafe bool CanAdd(IMapObj obj, bool Force, ServerSockets.Packet stream)
        {
            return CanAdd(obj, Force, stream, true);
        }
        /// <summary>
        /// sendBack = false skips the Force-only part that sends my spawn to obj and obj's clones to me. In Role()'s
        /// reverse call (me = the neighbour, obj = the mover) that is the neighbour's spawn to the mover and the
        /// mover's clones to the neighbour: Role()'s forward CanAdd has already sent both, under weaker conditions.
        /// </summary>
        public unsafe bool CanAdd(IMapObj obj, bool Force, ServerSockets.Packet stream, bool sendBack)
        {
            try
            {
                if (!CanSee(obj))
                    return false;
                if (Owner.Player.InView(obj.X, obj.Y, ViewThreshold) || Force)
                {
                    try
                    {

                        if (obj.ObjType == MapObjectType.Monster)
                        {
                            if (!obj.Alive)
                            {
                                var monster = obj as Game.MsgMonster.MonsterRole;
                                if (monster.CanRespawn(Owner.Map))
                                    monster.Respawn(false);
                            }
                            else
                                Owner.Send(obj.GetArray(stream, false));
                        }
                        else if (obj.ObjType == MapObjectType.Player)
                        {

                            if (Owner.Player.Map == 700)
                            {
                                var pClient = (obj as Role.Player).Owner;
                                if (Owner.InQualifier() && pClient.IsWatching())
                                    return true;
                                if (pClient.IsWatching())
                                    return true;
                            }
                            var apClient = (obj as Role.Player).Owner;
                            if (apClient.Player.Invisible && !SeesHidden(Owner))
                                return true;// a hidden GM: only staff get his spawn
                            Owner.Send(obj.GetArray(stream, false));

                            foreach (var clone in Owner.Player.MyClones.GetValues())
                            {
                                clone.Send((obj as Role.Player).Owner, stream);
                            }
                            if (Force && sendBack && Owner.IsWatching() == false && Owner.Player.Invisible == false)
                            {
                                obj.Send(Owner.Player.GetArray(stream, false));
                                foreach (var clone in (obj as Role.Player).MyClones.GetValues())
                                {
                                    clone.Send(Owner, stream);
                                }
                            }
                        }
                        else if (obj.ObjType == MapObjectType.Item)
                        {
                            if (obj.Alive == false)
                            {
                                var item = obj as Game.MsgFloorItem.MsgItem;
                                item.SendAll(stream, Game.MsgFloorItem.MsgDropID.Remove);
                                Owner.Map.View.LeaveMap<IMapObj>(obj);
                            }
                            else
                            {
                                if (obj.Map == 4006)
                                {
                                    var item = obj as Game.MsgFloorItem.MsgItem;
                                    if (item.Typ == Game.MsgFloorItem.MsgItem.ItemType.Effect && item.MsgFloor.m_ID == 1037)
                                    {
                                        if (Owner.Player.JoinTowerOfMysteryLayer == 7)
                                            Owner.Send(obj.GetArray(stream, false));
                                    }
                                    else if (item.MsgFloor.m_ID == 810)
                                        Owner.Send(obj.GetArray(stream, false));
                                }
                                else
                                    Owner.Send(obj.GetArray(stream, false));
                            }
                        }
                        else if (obj.ObjType == MapObjectType.Npc)
                        {
                            if (obj.Map == 1015 && obj.UID == (uint)Game.MsgNpc.NpcID.LittleBen)
                            {
                                if (Owner.Player.QuestGUI.CheckQuest(6129, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    return false;
                            }

                            if (obj.Map == 4009)
                            {
                                if (obj.UID == 19139)
                                {
                                    Owner.Send(stream.NpcCreate(obj as Npc, (ushort)(40350 + Math.Min(8, (int)Owner.Player.JoinTowerOfMysteryLayer) * 10)));
                                    return true;
                                }
                            }
                            if (obj.Map == 4000 || obj.Map == 4003 || obj.Map == 4006 || obj.Map == 4008)
                            {
                                if (Owner.Player.TOM_StartChallenge == false)
                                    Owner.Send(stream.NpcCreate(obj as Npc, (ushort)(40450 + Math.Min(9, (int)Owner.Player.JoinTowerOfMysteryLayer + 1) * 10)));
                                else if (Owner.Player.TOM_FinishChallenge)
                                    Owner.Send(stream.NpcCreate(obj as Npc, 40150));
                            }
                            else
                                Owner.Send(obj.GetArray(stream, false));
                        }
                        else if (obj.ObjType == MapObjectType.SobNpc || obj.ObjType == MapObjectType.StaticRole )
                            Owner.Send(obj.GetArray(stream, false));
                    }
                    catch (Exception e) { MyConsole.WriteException(e); }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                return false;
            }
        }
        public bool InColorScrenn = false;
        public unsafe void CheckScrennColors(ServerSockets.Packet stream)
        {
            if (Owner.Player.Map == 1002)
            {
                if (Owner.Player.Y < 325)
                {
                   
                    if (DateTime.Now.Minute > 10 && DateTime.Now.Minute < 20)
                    {
                        Owner.Send(stream.WeatherCreate(MsgWeather.WeatherType.Snow, 1000, 3, 0, 0));
                    }
                    else if (DateTime.Now.Minute > 30 && DateTime.Now.Minute < 40)
                    {
                        Owner.Send(stream.WeatherCreate(MsgWeather.WeatherType.AutumnLeaves, 1000, 3, 0, 0));
                    }
                    

                }
                if (InColorScrenn)
                {
                    InColorScrenn = false;
                    var action = new ActionQuery()
                    {
                        ObjId = Owner.Player.UID,
                        Type = ActionType.SetMapColor,
                        dwParam = 0,
                        wParam1 = Owner.Player.X,
                        wParam2 = Owner.Player.Y

                    };
                    Owner.Send(stream.ActionCreate(&action));
                }
            }
        }
        // [Performance] HouseFurnitureOnView: 1 = a house's furniture is sent to the mover only when it comes into
        // view (or after a teleport); 0 = old behaviour (every step resends all furniture to everyone nearby).
        public static readonly bool HouseFurnitureOnView = Conquer6609.Framework.Settings.Int("Performance", "HouseFurnitureOnView", 1) != 0;
        private unsafe void SendFurniture(Game.MsgNpc.Npc npc, bool clear, ServerSockets.Packet stream)
        {
            if (npc == null)
                return;
            if (!HouseFurnitureOnView)
            {
                npc.Send(stream);
                return;
            }
            // Same visibility rule as Npc.Send (CanSee: map, dynamic id, SeedDistance), but only for me and only
            // when the furniture was not already in range at my previous position.
            if (!npc.CanSee(Owner.Player))
                return;
            if (clear || Owner.Player.InView(npc.X, npc.Y, Game.MsgNpc.Npc.SeedDistance))
                Owner.Send(npc.GetArray(stream, false));
        }
        public unsafe void Role(bool clear = false, ServerSockets.Packet msg = null)
        {
            try
            {
                if (Owner.Player == null || Owner.Map == null)
                    return;
                if (clear)
                {
                    Owner.Player.Px = 0;
                    Owner.Player.Py = 0;
                }

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    //CheckScrennColors(stream);
                    try
                    {
                        if (Database.HouseTable.InHouse(Owner.Player.Map))
                        {
                            if (Owner.Player.UID == Owner.Player.DynamicID)
                            {
                                if (Owner.MyHouse != null)
                                {
                                    foreach (var entry in Owner.MyHouse.Furnitures)
                                        SendFurniture(entry.Value, clear, stream);
                                }
                            }
                            else
                            {
                                Conquer6609.Role.Instance.House House;
                                if (Conquer6609.Role.Instance.House.HousePoll.TryGetValue(Owner.Player.DynamicID, out House))
                                {
                                    try
                                    {
                                        foreach (var entry in House.Furnitures)
                                        {
                                            try
                                            {
                                                SendFurniture(entry.Value, clear, stream);
                                            }
                                            catch (Exception e)
                                            {
                                                MyConsole.WriteException(e);
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
                       
                        if (Owner.Player.Map == 3852)
                        {
                            foreach (var Roullet in Database.Roulettes.RoulettesPoll.Values)
                            {
                                try
                                {
                                    if (Owner.Player.InView(Roullet.SpawnPacket.X, Roullet.SpawnPacket.Y, 16))
                                    {
                                        Owner.Send(stream.RouletteTableCreate(Roullet.SpawnPacket.UID, Roullet.SpawnPacket.TableNumber
                                            , Roullet.SpawnPacket.MoneyType, Roullet.SpawnPacket.X, Roullet.SpawnPacket.Y, Roullet.SpawnPacket.Mesh, Roullet.SpawnPacket.PlayersCount));
                                    }
                                }
                                catch (Exception e)
                                {
                                    MyConsole.WriteException(e);
                                }

                            }
                        }

                        foreach (var m_client in Owner.Map.View.Roles(MapObjectType.Player, Owner.Player.X, Owner.Player.Y, null))// && (p as Role.Player).View.CanAdd(Owner.Player,clear, stream)))
                        {
                            if (m_client == null)
                                continue;
                            try
                            {
                               
                                // Reverse call: sends my spawn to m_client. Its Force "send back" part would resend
                                // m_client's spawn to me and my clones to m_client, which the forward CanAdd above
                                // has just done.
                                if (CanAdd(m_client, clear, stream) && (m_client as Role.Player).View.CanAdd(Owner.Player, true, stream, false))
                                {

                                     var client = (m_client as Role.Player).Owner;
                                    try
                                    {
                                        if (client.Socket != null)
                                        {
                                            if (client.Socket.Alive == false)
                                            {
                                                Owner.Map.Denquer(client);
                                                ActionQuery action;

                                                action = new ActionQuery()
                                                {
                                                    ObjId = client.Player.UID,
                                                    Type = ActionType.RemoveEntity
                                                };
                                                SendView(stream.ActionCreate(&action), false);
                                                continue;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        MyConsole.SaveException(e);
                                    }
                                    if (msg != null)
                                        client.Send(msg);

                                    client.Player.SendScrennXPSkill(Owner.Player);
                                    Owner.Player.SendScrennXPSkill(client.Player);

                                    client.Player.SendPowerTaoist(Owner, stream);
                                    Owner.Player.SendPowerTaoist(client, stream);

                                    if (client.Player.BlackSpot)
                                    {
                                        Owner.Send(stream.BlackspotCreate(true, client.Player.UID));
                                    }
                                    if (Owner.Player.BlackSpot)
                                    {
                                        client.Send(stream.BlackspotCreate(true, Owner.Player.UID));
                                    }
                                    if (client.Player.OnInteractionEffect)
                                    {
                                        client.Player.InteractionEffect.X = client.Player.X;
                                        client.Player.InteractionEffect.Y = client.Player.Y;


                                        InteractQuery action = InteractQuery.ShallowCopy(client.Player.InteractionEffect);

                                        Owner.Send(stream.InteractionCreate(&action));

                                    }
                                    if (Owner.Player.OnInteractionEffect)
                                    {
                                        Owner.Player.InteractionEffect.X = Owner.Player.X;
                                        Owner.Player.InteractionEffect.Y = Owner.Player.Y;


                                        InteractQuery action = InteractQuery.ShallowCopy(Owner.Player.InteractionEffect);
                                        client.Send(stream.InteractionCreate(&action));

                                    }
                                    if (Owner.IsVendor)
                                    {
                                        if (Owner.MyVendor.HalkMeesaje != null)
                                            client.Send(Owner.MyVendor.HalkMeesaje.GetArray(stream));
                                    }
                                    if (client.IsVendor)
                                    {
                                        if (client.MyVendor.HalkMeesaje != null)
                                            Owner.Send(client.MyVendor.HalkMeesaje.GetArray(stream));
                                    }
                                    if (Owner.Player.OnFairy != false)
                                    {
                                        client.Send(stream.TransformFairyCreate(Owner.Player.FairySpawn.Mode, Owner.Player.FairySpawn.FairyType, Owner.Player.FairySpawn.UID));
                                    }
                                    if (client.Player.OnFairy != false)
                                    {
                                        Owner.Send(stream.TransformFairyCreate(client.Player.FairySpawn.Mode, client.Player.FairySpawn.FairyType, client.Player.FairySpawn.UID));
                                    }
                                 
                                }
                            }
                            catch (Exception e)
                            {
                                MyConsole.WriteException(e);
                            }
                        }
                        foreach (var npc in Owner.Map.View.Roles(MapObjectType.Npc, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {
                            //npc.Send(stream, Owner);
                        }
                        foreach (var npc in Owner.Map.View.Roles(MapObjectType.StaticRole, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {
                            //npc.Send(stream, Owner);
                        }
                        foreach (var mob in Owner.Map.View.Roles(MapObjectType.Monster, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {
                            if (mob == null)
                                continue;
                            var Monster = mob as Game.MsgMonster.MonsterRole;
                            if (Monster.HitPoints > ushort.MaxValue || Monster.Boss == 1)
                            {
                                Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, Monster.UID, 2);
                                stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Monster.Family.MaxHealth);
                                stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, Monster.HitPoints);
                                stream = Upd.GetArray(stream);
                                Owner.Send(stream);
                            }
                            if (Monster.BlackSpot)
                            {
                                Owner.Send(stream.BlackspotCreate(true, Monster.UID));
                            }
                        }


                        foreach (var item in Owner.Map.View.Roles(MapObjectType.Item, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {
                            var PItem = item as Game.MsgFloorItem.MsgItem;
                            PItem.Send(stream);
                        }


                        foreach (var subnpc in Owner.Map.View.Roles(MapObjectType.SobNpc, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {
                            if (subnpc == null)
                                continue;
                            var SobMobNpcs = subnpc as Role.SobNpc;
                            if (SobMobNpcs.BitVector.ArrayFlags.Count != 0)
                            {
                                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, subnpc.UID, 1);
                                stream = upd.Append(stream, MsgUpdate.DataType.StatusFlag, SobMobNpcs.BitVector.bits);
                                stream = upd.GetArray(stream);
                                Owner.Send(stream);
                            }

                        }
                        //foreach (var staticrole in Owner.Map.View.Roles(MapObjectType.StaticRole, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        //{

                        //}
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteException(e);
                    }
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
        public unsafe void Clear(ServerSockets.Packet stream)
        {
            Owner.Player.Px = 0;
            Owner.Player.Py = 0;

            ActionQuery action;

            action = new ActionQuery()
            {
                ObjId = Owner.Player.UID,
                Type = ActionType.RemoveEntity
            };
            SendView(stream.ActionCreate(&action), false);


            foreach (var clone in Owner.Player.MyClones.GetValues())
            {
                clone.RemoveThat(Owner);
            }

            Owner.Player.MyClones.Clear();
         
        }
    }
}
