using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using System.Diagnostics;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Role.Instance
{

    public class Inventory
    {
        private const byte File_Size = 40;

        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> ClientItems = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();



        public int GetCountItem(uint ItemID)
        {
            int count = 0;
            foreach (var DataItem in ClientItems.Values)
            {
                if (DataItem.ITEM_ID == ItemID)
                {
                    count += DataItem.StackSize > 1 ? DataItem.StackSize : 1;
                }
            }
            return count;
        }

        public bool VerifiedUpdateItem(List<uint> ItemsUIDS, uint ID, byte count, out Queue<Game.MsgServer.MsgGameItem> Items)
        {
            Queue<Game.MsgServer.MsgGameItem> ExistItems = new Queue<Game.MsgServer.MsgGameItem>();
            foreach (var DataItem in ClientItems.Values)
            {
                if (DataItem.ITEM_ID == ID)
                {
                    if (ItemsUIDS.Contains(DataItem.UID))
                    {
                        count--;
                        ItemsUIDS.Remove(DataItem.UID);
                        ExistItems.Enqueue(DataItem);
                    }
                }
            }
            Items = ExistItems;
            return ItemsUIDS.Count == 0 && count == 0;
        }

        private Client.GameClient Owner;
        public Inventory(Client.GameClient _own)
        {
            Owner = _own;
        }

        public void AddDBItem(Game.MsgServer.MsgGameItem item)
        {
            ClientItems.TryAdd(item.UID, item);
        }

        public void AddReturnedItem(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
             , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, ushort StackSize = 0)
        {

            byte x = 0;
            for (; x < count; )
            {
                x++;
                Database.ItemType.DBItem DbItem;
                if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                {

                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.Effect = Effect;
                    ItemDat.StackSize = StackSize;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    ItemDat.Mode = Flags.ItemMode.AddItemReturned;
                    ItemDat.WH_ID = ushort.MaxValue;
                    if (DbItem.Time != 0 && DbItem.StackSize == 1)
                    {
                        ItemDat.Activate = 1;
                        ItemDat.EndDate = DateTime.Now.AddMinutes(DbItem.Time);
                    }
                    Owner.Warehouse.AddItem(ItemDat, ushort.MaxValue);

                    ItemDat.Send(Owner, stream);
                }
            }
        }


        public bool HaveSpace(byte count)
        {
            return (ClientItems.Count + count) <= File_Size;
        }

        public bool TryGetItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            return ClientItems.TryGetValue(UID, out item);
        }
        public bool SearchItemByID(uint ID, out Game.MsgServer.MsgGameItem item)
        {
            foreach (var msg_item in ClientItems.Values)
            {
                if (msg_item.ITEM_ID == ID)
                {
                    item = msg_item;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool SearchItemByID(uint ID, byte count, out List<Game.MsgServer.MsgGameItem> Items)
        {
            byte increase = 0;
            Items = new List<Game.MsgServer.MsgGameItem>();
            foreach (var msg_item in ClientItems.Values)
            {
                if (msg_item.ITEM_ID == ID)
                {
                    Items.Add(msg_item);
                    increase++;
                    if (increase == count)
                    {
                        return true;
                    }
                }
            }
            Items = null;
            return false;
        }
        public bool HasDuplicates(List<uint> list)
        {
            Dictionary<uint, uint> numberCounts = new Dictionary<uint, uint>();
            List<uint> list2 = new List<uint>();
            for (int i = 0; i < list.Count; i++)
            {
                MsgGameItem itemuse;
                if (ClientItems.TryGetValue(list[i], out itemuse))
                {
                    list2.Add(itemuse.UID);
                }
            }
            foreach (uint number in list2)
            {
                if (numberCounts.ContainsKey(number))
                {
                    return true;
                }
                numberCounts[number] = 1;
            }

            return false;
        }

        public bool DoSvae(Queue<MsgGameItem> UseItems, Client.GameClient user)
        {
            int count = 0;
            foreach (var item in UseItems)
            {
                count = 0;
                foreach (var item2 in UseItems)
                {
                    if (item.UID == item2.UID)
                    {
                        count++;
                    }
                    if (count > 1)
                    {
                        Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, 600);
                        user.Socket.Disconnect();
                        Console.WriteLine("This Fuck player He Tray Hack Plus and Gte ban In His Ass" + user.Player.Name);
                        return false;
                    }
                }

            }
            return true;
        }
        public bool Contain(uint ID, uint Amount, byte bound = 0)
        {
            if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == Database.ItemType.Meteor
                        || item.ITEM_ID == Database.ItemType.MeteorTear)
                    {
                        if (item.Bound == bound)
                        {
                            count += item.StackSize;
                            if (count >= Amount)
                                return true;
                        }
                    }
                }
            }
            else if (ID == Database.ItemType.MoonBox || ID == 723087)//execept for bound
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID)
                    {
                        count += item.StackSize;
                        if (count >= Amount)
                            return true;
                    }
                }
            }
            else
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID)
                    {
                        if (item.Bound == bound)
                        {
                            count += item.StackSize;
                            if (count >= Amount)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        public uint GetCount(uint ID)
        {
            uint count = 0;
            foreach (var item in ClientItems.Values)
            {
                if (item.ITEM_ID == ID)
                {
                    count++;
                }
            }
            return count;
        }
        public bool HaveItemsInSash()
        {
            foreach (var item in ClientItems.Values)
            {
                if (Database.ItemType.IsSash(item.ITEM_ID))
                {
                    if (item.Deposite.Count > 0)
                        return true;
                }
            }
            return false;
        }
        public bool Remove2(uint ID, uint Amount, ServerSockets.Packet stream, byte bound = 0)
        {
            ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> RemoverUID = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();
            if (Contain(ID, Amount) || Contain(ID, Amount, 1))
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {

                    if (item.ITEM_ID == ID)
                    {
                        if (item.Bound == bound)
                        {
                            count += item.StackSize;
                            RemoverUID.TryAdd(item.UID, item);

                            if (count >= Amount)
                            {
                                foreach (var T in RemoverUID.Values)
                                {
                                    try
                                    {
                                        Update(T, AddMode.REMOVE, stream);
                                    }
                                    catch (Exception e)
                                    {
                                        MyConsole.SaveException(e);
                                    }

                                }
                                return true;
                            }

                        }
                    }

                }
                if (count >= Amount)
                {
                    return true;
                }
                else
                    return false;

            }
            return false;

        }
        public bool Remove(uint ID, uint count, ServerSockets.Packet stream)
        {
            if (Contain(ID, count) || Contain(ID, count, 1))
            {
                if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
                {
                    byte removed = 0;
                    for (byte x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == Database.ItemType.Meteor
                         || item.ITEM_ID == Database.ItemType.MeteorTear)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                else
                {
                    byte removed = 0;
                    for (byte x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == ID)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                return true;
            }
            return false;
        }
        public bool AddSteed(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, bool bound = false, byte ProgresGreen = 0, byte ProgresBlue = 0, byte ProgresRed = 0)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                for (byte x = 0; x < count; x++)
                {
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;

                        ItemDat.ProgresGreen = ProgresGreen;
                        ItemDat.Enchant = ProgresBlue;
                        ItemDat.Bless = ProgresRed;
                        ItemDat.SocketProgress = (uint)(ProgresGreen | (ProgresBlue << 8) | (ProgresRed << 16));
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }
                        if (x >= count)
                            return true;
                    }
                }
            }
            return false;
        }
        public bool Add(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
             , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
            , string another_text = "", int days = 0, int hours = 0, int mins = 0, bool mine = false)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count; )
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {

                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Effect = Effect;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        if (days == 0 && hours == 0 && mins == 0)
                        {
                            if (DbItem.Time != 0 && DbItem.StackSize == 0)
                            {
                                ItemDat.Activate = 1;
                                ItemDat.EndDate = DateTime.Now.AddMinutes(DbItem.Time);
                            }
                        }
                        else if (days != 0)
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddDays(days);
                        }
                        else if (hours != 0)
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddHours(hours);
                        }
                        else
                        {
                            ItemDat.Activate = 1;
                            ItemDat.EndDate = DateTime.Now.AddMinutes(mins);
                        }
                        // Console.WriteLine(Database.ItemType.GetItemPoints(DbItem, ItemDat));
                        if (SendMessage)
                        {
                            if (mine)
                            {
                                Owner.SendSysMesage("You~received~a~" + DbItem.Name + "" + another_text);
                            }
                            else
                            {
                                Owner.CreateBoxDialog("You~received~a~" + DbItem.Name + "" + another_text);
                            }

                        }

                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            MyConsole.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;
            }

            return false;
        }
        public bool AddRefinaryItem(uint ID, bool Bound, ServerSockets.Packet stream)
        {
            ID = ID + Database.ItemType.GetNextRefineryItem();
            if (ID == 724348 || ID == 724349)
                ID += 150;
            if (ID == 724449)
                ID = 724445;

            return Add(stream, ID, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, Bound);
        }
        public bool AddItemWitchStack(uint ID, byte Plus, ushort amount, ServerSockets.Packet stream, bool bound = false)
        {
            Database.ItemType.DBItem DbItem;
            if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
            {

                if (DbItem.StackSize > 0)
                {
                    byte _bound = 0;
                    if (bound)
                        _bound = 1;
                    foreach (var item in ClientItems.Values)
                    {

                        if (item.ITEM_ID == ID && item.Bound == _bound)
                        {
                            if (item.StackSize + amount <= DbItem.StackSize)
                            {
                                item.Mode = Flags.ItemMode.Update;
                                item.StackSize += amount;
                                if (bound)
                                    item.Bound = 1;
                                item.Send(Owner, stream);

                                return true;
                            }
                        }
                    }

                    if (amount > DbItem.StackSize)
                    {
                        if (HaveSpace((byte)((amount / DbItem.StackSize) + (byte)(Owner.OnInterServer ? 1 : 0))))
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += DbItem.StackSize;
                                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                                amount -= DbItem.StackSize;

                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                                ItemDat.ITEM_ID = ID;
                                ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                                ItemDat.Plus = Plus;
                                ItemDat.StackSize += amount;
                                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                if (bound)
                                    ItemDat.Bound = 1;
                                try
                                {
                                    Update(ItemDat, AddMode.ADD, stream);
                                }
                                catch (Exception e)
                                {
                                    MyConsole.SaveException(e);
                                }
                            }
                            return true;
                        }
                        else
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, DbItem.StackSize);
                                amount -= DbItem.StackSize;
                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, amount);
                            }
                            return true;
                        }
                    }
                    else
                    {
                        if (HaveSpace(1))
                        {
                            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                            ItemDat.UID = Database.Server.ITEM_Counter.Next;
                            ItemDat.ITEM_ID = ID;
                            ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                            ItemDat.Plus = Plus;
                            ItemDat.StackSize = amount;
                            ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                            if (bound)
                                ItemDat.Bound = 1;
                            try
                            {
                                Update(ItemDat, AddMode.ADD, stream);
                            }
                            catch (Exception e)
                            {
                                MyConsole.SaveException(e);
                            }
                            return true;
                        }
                    }
                }
                for (int count = 0; count < amount; count++)
                    Add(ID, Plus, DbItem, stream, bound);
                return true;
            }
            return false;
        }
        public bool AddSoul(uint ID, uint SoulID, uint soullevel, uint souldays, byte plus, byte gem1, byte gem2, byte hp, byte daamge, byte times, ServerSockets.Packet stream, bool bound)
        {
            Database.ItemType.DBItem Soul = null;
            Database.ItemType.DBItem ITEMDB = null;
            if (SoulID != 0)
            {
                if (!Database.Server.ItemsBase.TryGetValue((uint)SoulID, out Soul))
                    return false;

            }
            if (!Database.Server.ItemsBase.TryGetValue((uint)ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                if (hp > 0)
                {
                    ItemDat.Enchant = (byte)(new System.Random().Next(200, 240));
                }
                ItemDat.PerfectionLevel = 54;
                ItemDat.OwnerName = Owner.Player.Name;
                ItemDat.OwnerUID = Owner.Player.UID;
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                ItemDat.RemainingTime2 = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;

                ItemDat.Purification.AddedOn = DateTime.Now;
                ItemDat.Purification.ItemUID = ItemDat.UID;
                ItemDat.Purification.PurificationLevel = 0;
                ItemDat.Purification.PurificationDuration = 0;
                ItemDat.Purification.PurificationItemID = SoulID;
                ItemDat.Purification.Typ = MsgItemExtra.Typing.PurificationEffect;
                MsgItemExtra extra = new MsgItemExtra
                {
                    Purifications = { ItemDat.Purification }
                };
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;

        }

        public bool ContainItemWithStack(uint UID, ushort Count)
        {
            Game.MsgServer.MsgGameItem ItemDat;
            if (ClientItems.TryGetValue(UID, out ItemDat))
            {
                return ItemDat.StackSize >= Count || Count == 1 && ItemDat.StackSize == 0;
            }
            return false;
        }

        public bool RemoveStackItem(uint UID, ushort Count, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgGameItem ItemDat;
            if (ClientItems.TryGetValue(UID, out ItemDat))
            {
                if (ItemDat.StackSize > Count)
                {
                    ItemDat.StackSize -= Count;
                    ItemDat.Mode = Flags.ItemMode.Update;
                    ItemDat.Send(Owner, stream);
                }
                else
                {
                    ItemDat.StackSize = 1;
                    Update(ItemDat, AddMode.REMOVE, stream);
                    return true;
                }
            }
            else
            {

                foreach (var item in ClientItems.Values)
                {
                    if (0 == Count)
                        break;
                    if (item.ITEM_ID == UID)
                    {
                        if (item.StackSize > Count)
                        {
                            item.StackSize -= Count;
                            item.Mode = Flags.ItemMode.Update;
                            item.Send(Owner, stream);
                            Count = 0;
                        }
                        else
                        {
                            Count -= item.StackSize;
                            item.StackSize = 1;
                            Update(item, AddMode.REMOVE, stream);
                        }
                    }
                }
            }
            return false;
        }
        public bool Add(uint ID, byte Plus, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream, bool bound = false)
        {
            if (ITEMDB.StackSize > 0)
            {
                byte _bound = 0;
                if (bound)
                    _bound = 1;
                foreach (var item in ClientItems.Values)
                {

                    if (item.ITEM_ID == ID && item.Bound == _bound)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            if (ITEMDB.Time != 0)
                            {
                                item.Activate = 1;
                                item.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                            }
                            if (bound)
                                item.Bound = 1;
                            item.Send(Owner, stream);

                            return true;
                        }
                    }
                }
            }
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = Plus;
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                if (ITEMDB.Time != 0 && ITEMDB.StackSize == 1)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                }
                if (bound)
                    ItemDat.Bound = 1;
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                return true;
            }
            return false;

        }
        public bool Add(Game.MsgServer.MsgGameItem ItemDat, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream)
        {
            if (ITEMDB.StackSize > 0)
            {
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ItemDat.ITEM_ID)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            item.Send(Owner, stream);
                            return true;
                        }
                    }
                }
            }
            if (HaveSpace(1))
            {
                if (ITEMDB.Time != 0 && ITEMDB.StackSize == 1)
                {
                    ItemDat.Activate = 1;
                    ItemDat.EndDate = DateTime.Now.AddMinutes(ITEMDB.Time);
                }
                Update(ItemDat, AddMode.ADD, stream);
                return true;
            }
            return false;

        }
        public bool AddItemWitchStack(Game.MsgServer.MsgGameItem ItemDat, byte amount, ServerSockets.Packet stream)
        {
            Database.ItemType.DBItem DbItem;
            if (Database.Server.ItemsBase.TryGetValue(ItemDat.ITEM_ID, out DbItem))
            {
                for (int count = 0; count < amount; count++)
                    Add(ItemDat, DbItem, stream);
                return true;
            }
            return false;
        }
        public unsafe bool Update(Game.MsgServer.MsgGameItem ItemDat, AddMode mode, ServerSockets.Packet stream, bool Removefull = false)
        {

            if (HaveSpace(1) || mode == AddMode.REMOVE)
            {
                string logs = "[Item]" + Owner.Player.Name + " He Do [" + mode + "] UID:  [" + ItemDat.UID + "] ID: " + ItemDat.ITEM_ID + " Plus: [" + ItemDat.Plus + "] bless: [" + ItemDat.Bless + "]Socket: [" + ItemDat.SocketTwo + "]";
                Logs.OnDequeue(logs);
                switch (mode)
                {
                    case AddMode.ADD:
                        {
                            CheakUp(ItemDat);
                            if (ItemDat.StackSize == 0)
                                ItemDat.StackSize = 1;
                            ItemDat.Position = 0;
                            ItemDat.Mode = Flags.ItemMode.AddItem;
                            ItemDat.Send(Owner, stream);
                            if (Owner.IsConnectedInterServer())
                            {
                                ItemDat.Send(Owner.PipeClient, stream);
                            }
                            break;
                        }
                    case AddMode.MOVE:
                        {
                            CheakUp(ItemDat);
                            ItemDat.Position = 0;
                            ItemDat.Mode = Flags.ItemMode.AddItem;
                            ItemDat.Send(Owner, stream);
                            break;
                        }
                    case AddMode.REMOVE:
                        {
                            // Only items held in this inventory can be removed; reports false when nothing was removed.
                            if (!ClientItems.ContainsKey(ItemDat.UID))
                                return false;
                            if (ItemDat.StackSize > 1 && ItemDat.Position < 40 && !Removefull)
                            {
                                ItemDat.StackSize -= 1;
                                ItemDat.Mode = Flags.ItemMode.Update;
                                ItemDat.Send(Owner, stream);
                                break;
                            }
                            Game.MsgServer.MsgGameItem item;
                            if (ClientItems.TryRemove(ItemDat.UID, out item))
                            {
                                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveInventory, item.UID, 0, 0, 0, 0, 0));
                            }
                            else
                                return false;
                            break;
                        }
                }
                if (ItemDat.ITEM_ID == 750000)
                {
                    Owner.DemonExterminator.ItemUID = ItemDat.UID;
                    if (mode == AddMode.REMOVE)
                        Owner.DemonExterminator.ItemUID = 0;
                }

                return true;

            }
            return false;
        }
        /// <summary>Removes one unit of the inventory item (one from a stack, or the whole item if not stacked).
        /// Returns true only if something was removed.</summary>
        public bool RemoveOne(uint uid, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgGameItem item;
            if (!ClientItems.TryGetValue(uid, out item) || item == null)
                return false;
            return Update(item, AddMode.REMOVE, stream, false);
        }

        /// <summary>Removes the whole inventory item including its full stack.
        /// Returns true only if the item was removed from the inventory.</summary>
        public bool RemoveWhole(uint uid, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgGameItem item;
            if (!ClientItems.TryGetValue(uid, out item) || item == null)
                return false;
            return Update(item, AddMode.REMOVE, stream, true);
        }

        /// <summary>True while the owner has a trade window or a booth open.</summary>
        public bool TradeOrBoothActive
        {
            get { return Owner.InTrade || Owner.IsVendor; }
        }

        /// <summary>True if the item is offered in the owner's trade window or booth, or one of them is open.
        /// Such items must not be moved to another container, refined, socketed or locked.</summary>
        public bool IsTradeOrBoothItem(uint uid)
        {
            if (TradeOrBoothActive)
                return true;
            var trade = Owner.MyTrade;
            if (trade != null && trade.Items != null && trade.Items.ContainsKey(uid))
                return true;
            var vendor = Owner.MyVendor;
            if (vendor != null && vendor.Items != null && vendor.Items.ContainsKey(uid))
                return true;
            return false;
        }

        private void CheakUp(Game.MsgServer.MsgGameItem ItemDat)
        {
            if (ItemDat.UID == 0)
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
            if (!ClientItems.TryAdd(ItemDat.UID, ItemDat))
            {
                do
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                while
                  (ClientItems.TryAdd(ItemDat.UID, ItemDat) == false);
            }
        }

        public bool CheckMeteors(byte count, bool Removethat, ServerSockets.Packet stream)
        {

            if (Contain(1088001, count))
            {
                if (Removethat)
                    Remove(1088001, count, stream);
                return true;
            }
            else
            {
                byte Counter = 0;
                var RemoveThis = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
                var MyMetscrolls = GetMyMetscrolls();
                var MyMeteors = GetMyMeteors();
                foreach (var GameItem in MyMetscrolls.Values)
                {
                    Counter += 10;
                    RemoveThis.Add(GameItem.UID, GameItem);
                    if (Counter >= count)
                        break;
                }
                if (Counter >= count)
                {
                    byte needSpace = (byte)(Counter - count);
                    if (HaveSpace(needSpace))
                    {
                        // [Security] the change of a MeteorScroll is needSpace Meteors (count), not one Meteor with +needSpace
                        // (was Add(stream, 1088001, 0, needSpace): a free Meteor per compose, and a +9 Meteor accepted as a +9 stone)
                        if (Removethat && needSpace > 0)
                        {
                            Add(stream, 1088001, needSpace);
                        }
                    }
                    else
                    {
                        Counter -= 10;
                        RemoveThis.Remove(RemoveThis.Values.First().UID);
                        byte needmetsss = (byte)(count - Counter);
                        if (needmetsss <= MyMeteors.Count)
                        {
                            foreach (var GameItem in MyMeteors.Values)
                            {
                                Counter += 1;
                                RemoveThis.Add(GameItem.UID, GameItem);
                                if (Counter >= count)
                                    break;
                            }
                            if (Removethat)
                            {
                                foreach (var GameItem in RemoveThis.Values)
                                    Update(GameItem, AddMode.REMOVE, stream);
                            }
                        }
                        else
                            return false;
                    }
                    if (Removethat)
                    {
                        foreach (var GameItem in RemoveThis.Values)
                            Update(GameItem, AddMode.REMOVE, stream);
                    }
                    return true;
                }
                foreach (var GameItem in MyMeteors.Values)
                {
                    Counter += 1;
                    RemoveThis.Add(GameItem.UID, GameItem);
                    if (Counter >= count)
                        break;
                }
                if (Counter >= count)
                {
                    if (Removethat)
                    {
                        foreach (var GameItem in RemoveThis.Values)
                            Update(GameItem, AddMode.REMOVE, stream);
                    }
                    return true;
                }
            }

            return false;
        }
        private Dictionary<uint, Game.MsgServer.MsgGameItem> GetMyMetscrolls()
        {
            var array = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            foreach (var GameItem in ClientItems.Values)
            {
                if (GameItem.ITEM_ID == 720027)
                {
                    if (!array.ContainsKey(GameItem.UID))
                        array.Add(GameItem.UID, GameItem);
                }
            }
            return array;
        }
        private Dictionary<uint, Game.MsgServer.MsgGameItem> GetMyMeteors()
        {
            var array = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            foreach (var GameItem in ClientItems.Values)
            {
                if (GameItem.ITEM_ID == Database.ItemType.Meteor || GameItem.ITEM_ID == Database.ItemType.MeteorTear)
                {
                    if (!array.ContainsKey(GameItem.UID))
                        array.Add(GameItem.UID, GameItem);
                }
            }
            return array;
        }
        public bool CollectedMoonBoxTokens(byte bound = 0)
        {
            return Contain(721010, 1, bound) && Contain(721011, 1, bound) && Contain(721012, 1, bound) && Contain(721013, 1, bound) && Contain(721014, 1, bound) && Contain(721015, 1, bound);
        }
        public bool CollectedAnyMoonBoxTokens()
        {
            return Contain(721010, 1) || Contain(721011, 1) || Contain(721012, 1) || Contain(721013, 1) || Contain(721014, 1) || Contain(721015, 1);
        }
        public bool CollectedTokens(uint ID)
        {
            if (ID == 1044)
            {
                return Contain(721011, 1);
            }
            if (ID == 1046)
            {
                return Contain(721013, 1);
            }
            if (ID == 1048)
            {
                return Contain(721015, 1);
            }
            if (ID == 1045)
            {
                return Contain(721012, 1);
            }
            if (ID == 1043)
            {
                return Contain(721010, 1);
            }
            if (ID == 1047)
            {
                return Contain(721014, 1);
            }
            return false;
        }
        public void ShowALL(ServerSockets.Packet stream)
        {
            foreach (var msg_item in ClientItems.Values)
            {
                msg_item.Mode = Flags.ItemMode.AddItem;
                msg_item.Send(Owner, stream);
            }
        }
        public void Clear(ServerSockets.Packet stream)
        {
            var dictionary = ClientItems.Values.ToArray();
            foreach (var msg_item in dictionary)
                Update(msg_item, AddMode.REMOVE, stream);
        }
    }
}
