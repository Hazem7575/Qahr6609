using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
    public class Wardrobe
    {
        public enum ItemsType : byte
        {
            Garment = 0,
            Mount = 1,
            Count = 2
        }
        private List<uint> MonkeysValues = new List<uint>()
        {
            200553,
            200554,
            200555,
            200556,
            200557,
            200558,
            200559,
            200560     //testcode
            //200596
        };
        public ItemsType GetItemType(uint ID)
        {
            if (Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.SteedMount)
                return ItemsType.Mount;
            if (Database.ItemType.ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Garment)
                return ItemsType.Garment;
            return ItemsType.Count;
        }
        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>[] Items;

        public bool HaveItems()
        {
            foreach (var item in Items)
            {
                if (item.Count > 0)
                    return true;
            }
            return false;
        }

        public Client.GameClient Owner;

        public Wardrobe(Client.GameClient _owner)
        {
            Owner = _owner;
            Items = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>[(int)ItemsType.Count];
            for (int x = 0; x < (int)ItemsType.Count; x++)
                Items[x] = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();
        }
        public bool CollectedRandomMonkey()
        {
            bool Yes = false;
            for (int x = 0; x < this.MonkeysValues.Count; x++)
            {
                if(Contain(this.MonkeysValues[x]))
                {
                    Yes = true;
                    break;
                }
            }
            return Yes;
        }
        public bool CollectedLunarMonkey()
        {
            return Contain(200560);
        }
        public bool CollectedSolarMonkey()
        {
            return Contain(200559);
        }
       /* public bool CollectedCelestialFox()//200596 will test!?
        {
            return Contain(200596);
        }*/
        public bool CollectedAllMonkeyTypes()
        {
            bool Yes = false;
            for (int x = 0; x < this.MonkeysValues.Count; x++)
            {
                Yes &= Contain(this.MonkeysValues[x]);
            }
            return Yes;
        }
        public bool Contain(uint ID)
        {
            foreach (var objects in Items)
            {
                var dictionary = objects;
                foreach (var item in dictionary.Values)
                    if (item.ITEM_ID == ID)
                        return true;
            }
            return false;
        }
        public bool ContainUID(uint UID)
        {
            foreach (var objects in Items)
            {
                var dictionary = objects;
                foreach (var item in dictionary.Values)
                    if (item.UID == UID)
                        return true;
            }
            return false;
        }
        public bool TryGetItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            Items[(byte)ItemsType.Garment].TryGetValue(UID, out item);
            if (item != null)
                return true;
            Items[(byte)ItemsType.Mount].TryGetValue(UID, out item);
            if (item != null)
                return true;
            item = null;
            return false;

        }
        public bool TryGetItemByID(uint ID, out Game.MsgServer.MsgGameItem itemout)
        {
            foreach (var objects in Items)
            {
                var dictionary = objects;
                foreach (var item in dictionary.Values)
                    if (item.ITEM_ID == ID)
                    {
                        itemout = item;
                        return true;
                    }
            }
            itemout = null;
            return false;

        }
        public bool AddItem(Game.MsgServer.MsgGameItem item)
        {
            ItemsType type = GetItemType(item.ITEM_ID);
            if (type == ItemsType.Count)
                return false;//invalid item.

            var dictinary = Items[(int)type];
            if (!dictinary.ContainsKey(item.UID))
            {
                item.WH_ID = 100;
                dictinary.TryAdd(item.UID, item);
                return true;
            }
            return false;
        }
        public bool RemoveItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            item = null;
            foreach (var objects in Items)
            {
                var dictionary = objects;
                bool accrepter = dictionary.TryRemove(UID, out item);
                if (accrepter)
                    break;
            }

            if (item != null)
            {
                item.WH_ID = 0;
                item.Position = 0;
            }
            return item != null;
        }
        public void SendToClient(ServerSockets.Packet stream)
        {
            bool haveitems = false;

            Game.MsgServer.MsgGameItem Garment;
            Game.MsgServer.MsgGameItem Mount;
            Owner.Equipment.TryGetEquip(Flags.ConquerItem.Garment, out Garment);
            Owner.Equipment.TryGetEquip(Flags.ConquerItem.SteedMount, out Mount);


            foreach (var objects in Items)
            {
                foreach (var item in objects.Values)
                {
                    haveitems = true;
                    if (Garment != null)
                    {
                        if (Garment.UID == item.UID)
                        {
                           EquipItem(stream, Garment);
                            continue;
                        }
                    }
                    if (Mount != null)// was "else if": with a garment worn the worn mount was never marked
                    {
                        if (Mount.UID == item.UID)
                        {
                            EquipItem(stream, Mount);
                            continue;
                        }
                    }


                    Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                    store.AddItem(item);
                    Owner.Send(stream.CreateCoatStorage(store));
                }
            }
            if (haveitems == false)
            {
                Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
            //    store.Item = new MsgCoatStorage.ItemStorage[0];
                Owner.Send(stream.CreateCoatStorage(store));
            }
        }
        public void EquipItem(ServerSockets.Packet stream, Game.MsgServer.MsgGameItem item)
        {
          
            SendItem(stream, item);
            Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
            store.ActionID = MsgCoatStorage.Action.Equip;
            store.dwparam1 = item.UID; store.dwpram2 = item.ITEM_ID;
            Owner.Send(stream.CreateCoatStorage(store));
        }
        public void SendItem(ServerSockets.Packet stream, Game.MsgServer.MsgGameItem item)
        {
            Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
            store.AddItem(item);
            Owner.Send(stream.CreateCoatStorage(store));
        }
        public bool Equipped(uint UID, out Role.Flags.ConquerItem position, out Game.MsgServer.MsgGameItem item)
        {
            item = null;
            position = Flags.ConquerItem.Inventory;
            Game.MsgServer.MsgGameItem Garment;
            Game.MsgServer.MsgGameItem Mount;
            Owner.Equipment.TryGetEquip(Flags.ConquerItem.Garment, out Garment);
            Owner.Equipment.TryGetEquip(Flags.ConquerItem.SteedMount, out Mount);
            if (Garment != null && Garment.UID == UID)
            {
                item = Garment;
                position = Flags.ConquerItem.Garment;
            }
            else if (Mount != null && Mount.UID == UID)
            {
                item = Mount;
                position = Flags.ConquerItem.SteedMount;
            }
            return item != null;
        }
        public IEnumerable<Game.MsgServer.MsgGameItem> GetAllItems()
        {
            foreach (var objects in Items)
            {
                foreach (var item in objects.Values)
                {
                    yield return item;
                }
            }
        }
        public int GetCountItems()
        {
            int count = 0;
            foreach (var objects in Items)
                count += objects.Count;
            return count;
        }

    }
}
