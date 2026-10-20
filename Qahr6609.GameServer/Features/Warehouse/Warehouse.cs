using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
    public class Warehouse
    {
        public const byte Max_Count = 60;

        public static bool IsWarehouse(Game.MsgNpc.NpcID ID)
        {
            return (ID == Game.MsgNpc.NpcID.WHTwin || ID == Game.MsgNpc.NpcID.wHPheonix
                              || ID == Game.MsgNpc.NpcID.WHMarket || ID == Game.MsgNpc.NpcID.WHBird
                              || ID == Game.MsgNpc.NpcID.WHDesert || ID == Game.MsgNpc.NpcID.WHApe
                              || ID == Game.MsgNpc.NpcID.WHPoker || ID == Game.MsgNpc.NpcID.WHStone
                              || ID == (Game.MsgNpc.NpcID)ushort.MaxValue);
        }


        public byte WHMaxSpace()
        {
            return (byte)((User.Player.VipLevel != 0) ? 80 : Max_Count);
        }

        public void RemoveInscribedItems()
        {
            foreach (var wh in ClientItems.Values)
            {
                foreach (var item in wh.Values)
                {
                    if (item.Inscribed == 1)
                        item.Inscribed = 0;
                }
            }
        }

        public bool HaveItemsInBanks()
        {
        
            foreach (var bank in ClientItems.Values)
            {
                if (bank.Count > 0)
                    return true;
            }
            return false;
        }

        public ConcurrentDictionary<uint, ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>> ClientItems;
        public List<uint> IsShow = new List<uint>();
        public Client.GameClient User;
        public Warehouse(Client.GameClient client)
        {
            ClientItems = new ConcurrentDictionary<uint, ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>>();
            User = client;
        }

        public void SendReturnedItems(ServerSockets.Packet stream)
        {
            ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> wh_items;
            if (ClientItems.TryGetValue(ushort.MaxValue, out wh_items))
            {
                foreach (var item in wh_items.Values)
                {
                    item.Mode = Flags.ItemMode.AddItemReturned;
                    item.Send(User, stream);
                }
            }
        }


        /// <summary>Capacity of a storage: returned-items storage is unlimited, the inventory sash uses
        /// the unlocked sash slots, every other warehouse uses WHMaxSpace().</summary>
        public int GetCapacity(uint NpcID)
        {
            if (NpcID == ushort.MaxValue)
                return int.MaxValue;
            if (User != null && User.Player != null && NpcID == User.Player.UID)
                return User.Player.InventorySashCount;
            return WHMaxSpace();
        }

        public bool AddItem(Game.MsgServer.MsgGameItem DataItem, uint NpcID)
        {
            // 0 means "not stored" and 100 marks wardrobe items; neither is a warehouse.
            if (DataItem == null || NpcID == 0 || NpcID == 100)
                return false;
            if (!ClientItems.ContainsKey(NpcID))
                ClientItems.TryAdd(NpcID, new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>());

            if (ClientItems[NpcID].Count >= GetCapacity(NpcID))
                return false;
            if (ClientItems[NpcID].TryAdd(DataItem.UID, DataItem))
            {
                DataItem.WH_ID = NpcID;
                return true;
            }
            return false;
        }
        public unsafe bool RemoveItem(uint UID, uint NpcID, ServerSockets.Packet stream)
        {
            if (ClientItems.ContainsKey(NpcID))
            {
                if (User.Inventory.HaveSpace(1))
                {
                    Game.MsgServer.MsgGameItem item;
                    if (ClientItems[NpcID].TryRemove(UID, out item))
                    {
                        // The item leaves the warehouse before entering the inventory; restore it if that fails.
                        uint oldWh = item.WH_ID;
                        item.Position = 0;
                        item.WH_ID = 0;
                        if (User.Inventory.ClientItems.ContainsKey(item.UID) || !User.Inventory.Update(item, AddMode.ADD, stream))
                        {
                            item.WH_ID = oldWh;
                            ClientItems[NpcID].TryAdd(item.UID, item);
                            return false;
                        }
                        return true;
                    }
                }
                else
                {
                    User.SendSysMesage("Your Inventory Is Full!");
                   
                }
            }
            return false;
        }
        public unsafe bool DeleteItem(uint UID, uint NpcID, ServerSockets.Packet stream)
        {
            if (ClientItems.ContainsKey(NpcID))
            {
                
                    Game.MsgServer.MsgGameItem item;
                    if (ClientItems[NpcID].TryRemove(UID, out item))
                    {
                        item.Position = 0;
                        item.WH_ID = 0;
                        User.Inventory.Update(item, AddMode.REMOVE, stream);
                        stream.WarehouseCreate(NpcID, MsgWarehouse.DepositActionID.WithdrawItem, UID, 0, 0);
                        User.Send(stream.FinalizeWarehouse());
                        return true;
                    }
              
            }
            return false;
        }
        public unsafe void Show(uint NpcID, Game.MsgServer.MsgWarehouse.DepositActionID Action, ServerSockets.Packet stream)
        {
            if (ClientItems.ContainsKey(NpcID) && !IsShow.Contains(NpcID))
            {
                IsShow.Add(NpcID);

                Dictionary<int, List<Game.MsgServer.MsgGameItem>> Queues = new Dictionary<int, List<Game.MsgServer.MsgGameItem>>();
                Queues.Add(0, new List<Game.MsgServer.MsgGameItem>());

                int count = 0;
                var Array = ClientItems[NpcID].Values.ToArray();
                for (uint x = 0; x < Array.Length; x++)
                {
                    if (x % 8 == 0)
                    {
                        count++;
                        Queues.Add(count, new List<Game.MsgServer.MsgGameItem>());
                    }
                    Queues[count].Add(Array[x]);
                }

                foreach (var aray in Queues.Values)
                {
             
                    Game.MsgServer.MsgItemExtra itemExtra = new Game.MsgServer.MsgItemExtra();

                    stream.WarehouseCreate(NpcID, Action, 0, WHMaxSpace(), aray.Count);

                    foreach (var item in aray)
                    {
                        stream.AddItemWarehouse(item);

                        if (item.Refinary.InLife)
                        {
                            item.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.RefinaryAdding;
                            if (item.Refinary.EffectDuration == 0)
                                item.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.PermanentRefinery;
                            itemExtra.Refinerys.Add(item.Refinary);
                        }
                        if (item.Purification.InLife)
                        {
                            item.Purification.Typ = Game.MsgServer.MsgItemExtra.Typing.PurificationAdding;
                            itemExtra.Purifications.Add(item.Purification);
                        }
                    }
                    User.Send(stream.FinalizeWarehouse());

                    foreach (var item in aray)
                        item.SendItemLocked(User, stream);

                    if (itemExtra.Refinerys.Count != 0 || itemExtra.Purifications.Count != 0)
                        User.Send(itemExtra.CreateArray(stream));
                }
            }
        }
    }
}
