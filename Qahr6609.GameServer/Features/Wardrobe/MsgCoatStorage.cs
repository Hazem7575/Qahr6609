using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgCoatStorage
    {

        [ProtoContract]
        public class CoatStorage
        {
            [ProtoMember(1, IsRequired = true)]
            public Action ActionID;
            [ProtoMember(2, IsRequired = false)]
            public uint dwparam1;
            [ProtoMember(3, IsRequired = false)]
            public uint dwpram2;
            [ProtoMember(4, IsRequired = true)]
            public uint dwpram3;
            [ProtoMember(5, IsRequired = false)]
            public ItemStorage Item;

            public void AddItem(Game.MsgServer.MsgGameItem item)
            {
                Item = new ItemStorage();
                Item.ItemUID = item.UID;
                Item.ItemID = item.ITEM_ID;
                Item.MaxDurability = item.MaximDurability;
                Item.MinDurability = item.Durability;
                Item.Stack = 1;
                Item.Plus = item.Plus;
                Item.Bless = item.Bless;
                Item.Color = Role.Flags.Color.Orange;
                Item.TimeLeft = item.RemainingTime;
                Item.Bound = item.Bound;
                Item.Lock = item.Locked;
            }
        }
        [ProtoContract]
        public class ItemStorage
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ItemUID;
            [ProtoMember(2, IsRequired = true)]
            public uint ItemID;
            [ProtoMember(3, IsRequired = true)]
            public uint SocketProgress;
            [ProtoMember(4, IsRequired = true)]
            public uint SocketOne;
            [ProtoMember(5, IsRequired = true)]
            public uint SocketTwo;
            [ProtoMember(6, IsRequired = true)]
            public Role.Flags.ItemEffect Effect;
            [ProtoMember(7, IsRequired = true)]
            public uint dwparam7;
            [ProtoMember(8, IsRequired = true)]
            public uint Plus;
            [ProtoMember(9, IsRequired = true)]
            public uint Bless;
            [ProtoMember(10, IsRequired = true)]
            public uint Bound;//??
            [ProtoMember(11, IsRequired = true)]
            public uint HP;
            [ProtoMember(12, IsRequired = true)]
            public uint Equipped;
            [ProtoMember(13, IsRequired = true)]
            public uint Suspicious;
            [ProtoMember(14, IsRequired = true)]
            public uint Lock;
            [ProtoMember(15, IsRequired = true)]
            public Role.Flags.Color Color;
            [ProtoMember(16, IsRequired = true)]
            public uint Type2;
            [ProtoMember(17, IsRequired = true)]
            public uint CraftProgress;
            [ProtoMember(18, IsRequired = true)]
            public uint Inscribed;
            [ProtoMember(19, IsRequired = true)]
            public uint TimeLeft;
            [ProtoMember(20, IsRequired = true)]
            public uint SubTime;
            [ProtoMember(21, IsRequired = true)]
            public uint Stack;
            [ProtoMember(22, IsRequired = true)]
            public uint MinDurability;
            [ProtoMember(23, IsRequired = true)]
            public uint MaxDurability;
        }
        [Flags]
        public enum Action : uint
        {
            Equip = 1,
            TakeOff = 6,
            AddToWardRobe = 5,
            Update = 4,
            Expire = 8,
            Retrive = 2
        }
        public static unsafe ServerSockets.Packet CreateCoatStorage(this ServerSockets.Packet stream, CoatStorage obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCoatStorage);

            return stream;
        }
        public static unsafe void GetCoatStorage(this ServerSockets.Packet stream, out CoatStorage pQuery)
        {
            pQuery = new CoatStorage();
            pQuery = stream.ProtoBufferDeserialize<CoatStorage>(pQuery);
        }
        public static bool CheckGender(Client.GameClient user, uint itemid)
        {
            Database.ItemType.DBItem dbitem;
            if (Database.Server.ItemsBase.TryGetValue(itemid, out dbitem))
            {
                if (dbitem.Gender == 0)
                    return true;
                if (dbitem.Gender == 1)//boy
                {
                    if (Role.Core.IsBoy(user.Player.Body))
                        return true;
                }
                if (dbitem.Gender == 2)//female
                {
                    if (Role.Core.IsGirl(user.Player.Body))
                        return true;
                }
            }
            return false;
        }
        /// <summary>Puts a wardrobe garment / mount on (Equipment slot 9 / 17), replacing the worn one (which stays in the
        /// wardrobe), refreshes the look for the owner and the screen and marks it worn in the CoatStor window.</summary>
        public static bool WearFromWardrobe(Client.GameClient client, Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            if (item == null || !client.MyWardrobe.ContainUID(item.UID))
                return false;
            ushort Position = Database.ItemType.ItemPosition(item.ITEM_ID);
            if (Position != (ushort)Role.Flags.ConquerItem.Garment && Position != (ushort)Role.Flags.ConquerItem.SteedMount)
                return false;
            if (Position == (ushort)Role.Flags.ConquerItem.Garment)
            {
                if (client.Player.SpecialGarment != 0)
                {
                    client.SendSysMesage("Item can't be unequiped during the event.");
                    return false;
                }
                if (!CheckGender(client, item.ITEM_ID))
                {
                    client.SendSysMesage("This garment is not for your gender.");
                    return false;
                }
            }
            // A wardrobe item must not also be held in the inventory.
            if (client.Inventory.ClientItems.ContainsKey(item.UID) && !client.Inventory.RemoveWhole(item.UID, stream))
                return false;
            Game.MsgServer.MsgGameItem worn;
            if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)Position, out worn) && worn != null)
            {
                if (worn.UID == item.UID)
                {
                    SendWorn(client, item, stream);
                    return true;
                }
                if (!client.Equipment.Remove((Role.Flags.ConquerItem)Position, stream))
                    return false;
                if (client.MyWardrobe.ContainUID(worn.UID))
                {
                    Game.MsgServer.MsgCoatStorage.CoatStorage off = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                    off.ActionID = MsgCoatStorage.Action.TakeOff;
                    off.dwparam1 = worn.UID;
                    off.dwpram2 = worn.ITEM_ID;
                    client.Send(stream.CreateCoatStorage(off));
                }
            }
            item.Position = Position;
            client.Equipment.Add(item, stream);
            item.Mode = Role.Flags.ItemMode.Update;
            item.Send(client, stream);
            client.Equipment.QueryEquipment(client.Equipment.Alternante);
            SendWorn(client, item, stream);
            return true;
        }
        private static void SendWorn(Client.GameClient client, Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            // Action 1 (Equip) with uid + item id is what the login list uses for "worn" (Wardrobe.EquipItem).
            Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
            store.ActionID = MsgCoatStorage.Action.Equip;
            store.dwparam1 = item.UID;
            store.dwpram2 = item.ITEM_ID;
            client.Send(stream.CreateCoatStorage(store));
        }
        [PacketAttribute(GamePackets.MsgCoatStorage)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            CoatStorage pQuery;
            stream.GetCoatStorage(out pQuery);
            switch (pQuery.ActionID)
            {
                case Action.Update:
                    {
                        Game.MsgServer.MsgGameItem item, ExistedItem;
                        if (client.Inventory.TryGetItem(pQuery.dwpram2, out item))
                        {
                            if (client.MyWardrobe.TryGetItem(pQuery.dwparam1, out ExistedItem))
                            {
                                if (item != null && ExistedItem != null)
                                {
                                    // Renewal consumes an inventory copy of the same item; it never unbinds the stored item.
                                    if (item.UID == ExistedItem.UID || item.ITEM_ID != ExistedItem.ITEM_ID)
                                        break;
                                    if (client.Inventory.IsTradeOrBoothItem(item.UID))
                                        break;
                                    bool makePermanent = ExistedItem.RemainingTime > 0 && item.RemainingTime == 0;
                                    bool extendTime = !makePermanent && ExistedItem.RemainingTime > 0 && item.RemainingTime > 0 && ExistedItem.Bound == item.Bound;
                                    if (!makePermanent && !extendTime)
                                        break;
                                    uint addSeconds = item.RemainingTime;
                                    byte consumedBound = item.Bound;
                                    if (!client.Inventory.RemoveWhole(item.UID, stream))
                                        break;
                                    if (makePermanent)
                                    {
                                        ExistedItem.Activate = 0;
                                        ExistedItem.EndDate = DateTime.FromBinary(0);
                                        if (consumedBound == 1)
                                            ExistedItem.Bound = 1;
                                    }
                                    else
                                    {
                                        DateTime start = ExistedItem.EndDate > DateTime.Now ? ExistedItem.EndDate : DateTime.Now;
                                        ExistedItem.EndDate = start.AddSeconds(addSeconds);
                                    }
                                    ExistedItem.Mode = Role.Flags.ItemMode.Update;
                                    Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                    store.ActionID = MsgCoatStorage.Action.Update;
                                    store.dwparam1 = ExistedItem.UID;
                                    client.Send(stream.CreateCoatStorage(store));
                                }
                            }
                        }
                        break;
                    }
                case Action.Equip:
                    {
                        // 6609 client: action 1 = "wear". It is sent both for a garment/mount in the bag (official: it moves
                        // into the wardrobe and is put on) and for an item already in the wardrobe (the Wear button of the
                        // CoatStor window, dialog 741). The old handler only accepted a bag item and never put it on, so the
                        // item vanished into the wardrobe and every later Wear click was ignored.
                        Game.MsgServer.MsgGameItem item;
                        if (client.Inventory.TryGetItem(pQuery.dwparam1, out item))
                        {
                            // same garment already stored: the client renews it with action 4 (Update) instead.
                            if (client.MyWardrobe.Contain(item.ITEM_ID) || client.MyWardrobe.ContainUID(item.UID))
                                return;
                            if (client.Inventory.IsTradeOrBoothItem(item.UID))
                                return;
                            ushort Position = Database.ItemType.ItemPosition(item.ITEM_ID);
                            if (Position != (ushort)Role.Flags.ConquerItem.Garment && Position != (ushort)Role.Flags.ConquerItem.SteedMount)
                                return;
                            if (Position == (ushort)Role.Flags.ConquerItem.Garment && client.Player.SpecialGarment != 0)
                            {
                                client.SendSysMesage("Item can't be unequiped during the event.");
                                return;
                            }
                            // The item leaves the inventory completely before it is stored in the wardrobe.
                            if (!client.Inventory.RemoveWhole(item.UID, stream))
                                return;
                            if (!client.MyWardrobe.AddItem(item))
                            {
                                item.WH_ID = 0;
                                item.Position = 0;
                                client.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);
                                return;
                            }
                            client.MyWardrobe.SendItem(stream, item);
                            // stored; put it on (if that fails, e.g. wrong gender, it simply stays in the wardrobe).
                            WearFromWardrobe(client, item, stream);
                        }
                        else if (client.MyWardrobe.TryGetItem(pQuery.dwparam1, out item) && item != null)
                        {
                            WearFromWardrobe(client, item, stream);
                        }
                        break;
                    }
                case Action.AddToWardRobe:
                    {
                        Game.MsgServer.MsgGameItem item;
                        if (client.MyWardrobe.TryGetItem(pQuery.dwparam1, out item) && item != null)
                            WearFromWardrobe(client, item, stream);
                        break;
                    }
                case Action.TakeOff:
                    {
                        Game.MsgServer.MsgGameItem item;
                        // dwparam1 = item uid; older code read the slot from dwpram2 - accept both.
                        if (!client.Equipment.TryGetValue(pQuery.dwparam1, out item) || item == null
                            || (item.Position != (ushort)Role.Flags.ConquerItem.Garment && item.Position != (ushort)Role.Flags.ConquerItem.SteedMount))
                        {
                            item = null;
                            if (pQuery.dwpram2 == (uint)Role.Flags.ConquerItem.Garment || pQuery.dwpram2 == (uint)Role.Flags.ConquerItem.SteedMount)
                                client.Equipment.TryGetEquip((Role.Flags.ConquerItem)pQuery.dwpram2, out item);
                        }
                        if (item != null)
                        {
                            if (item.Position == (ushort)Role.Flags.ConquerItem.Garment)
                            {
                                if (client.Player.SpecialGarment != 0)
                                {
                                    client.SendSysMesage("Item can't be unequiped during the event.");

                                    return;
                                }
                            }
                            client.Equipment.Remove((Role.Flags.ConquerItem)item.Position, stream);

                            Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                            store.ActionID = MsgCoatStorage.Action.TakeOff;
                            store.dwparam1 = item.UID;
                            store.dwpram2 = item.ITEM_ID;
                            client.Send(stream.CreateCoatStorage(store));

                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }
                        break;
                    }
                case Action.Retrive:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please make 1 more space in your inventory.");

                            break;
                        }
                        if (!client.MyWardrobe.ContainUID(pQuery.dwparam1))
                            break;
                        Game.MsgServer.MsgGameItem item2;
                        if (client.Equipment.TryGetValue(pQuery.dwparam1, out item2))
                        {
                            if (item2.UID == pQuery.dwparam1)
                            {
                                // The worn copy is taken off first; abort if that fails.
                                if (!client.Equipment.Remove((Role.Flags.ConquerItem)item2.Position, stream))
                                    break;

                                Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                store.ActionID = MsgCoatStorage.Action.TakeOff;
                                store.dwparam1 = item2.UID;
                                store.dwpram2 = item2.ITEM_ID;
                                client.Send(stream.CreateCoatStorage(store));

                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                        }

                        Game.MsgServer.MsgGameItem item;
                        if (client.MyWardrobe.RemoveItem(pQuery.dwparam1, out item))
                        {
                            if (client.Inventory.ClientItems.ContainsKey(item.UID) || !client.Inventory.Update(item, Role.Instance.AddMode.ADD, stream))
                            {
                                client.MyWardrobe.AddItem(item);
                                break;
                            }
                            Game.MsgServer.MsgCoatStorage.CoatStorage astore = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                            astore.ActionID = MsgCoatStorage.Action.Retrive;
                            astore.dwparam1 = pQuery.dwparam1;
                            astore.dwpram2 = item.ITEM_ID;
                            client.Send(stream.CreateCoatStorage(astore));
                            client.Send(stream.CreateCoatStorage(astore));

                        }
                        break;
                    }
                case Action.Expire:
                    {
                        // The client reports a timed wardrobe item whose time is up; drop it now instead of
                        // waiting for the periodic item-time check (only if it really has expired).
                        Game.MsgServer.MsgGameItem item;
                        if (!client.MyWardrobe.TryGetItem(pQuery.dwparam1, out item) || item == null)
                            break;
                        if (item.Activate != 1 || item.EndDate > DateTime.Now)
                            break;
                        Client.PoolProcesses.CheckItemsTime(client);
                        break;
                    }
                default: MyConsole.WriteLine("Unknown Action Type [" + (uint)pQuery.ActionID + "] MsgCoatStorage."); break;
            }
        }

    }
}
