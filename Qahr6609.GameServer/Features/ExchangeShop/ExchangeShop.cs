using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
    public class ExchangeShop
    {
        public class Item
        {
            public int Index;
            public uint ItemID;
            public ushort BoughtItems;

        }

        public Dictionary<uint, Item> Items = new Dictionary<uint, Item>();
        public bool ShowItems = true;
        private Client.GameClient user;
        public ExchangeShop(Client.GameClient _user)
        {
            user = _user;
        }
        public void Reset()
        {
            Items.Clear();
        }
        public bool CanBuyItem(Database.ExchangeShop.Item _item, ushort Count)
        {
            // Limits are per shop entry (Index): the same item sold in two shops / for two currencies has separate limits.
            if (_item.CountItems == 0)
                return true;
            Item myitem;
            if (Items.TryGetValue((uint)_item.Index, out myitem))
            {
                return myitem.BoughtItems <= _item.CountItems && _item.CountItems - myitem.BoughtItems >= Count;
            }
            return _item.CountItems >= Count;
        }
        public void AddItem(Database.ExchangeShop.Item _item, ushort Count)
        {
            Item myitem;
            if (Items.TryGetValue((uint)_item.Index, out myitem))
            {
                myitem.BoughtItems += Count;
            }
            else
            {
                myitem = new Item()
                {
                    ItemID = _item.ItemID,
                    BoughtItems = Count,
                    Index = _item.Index
                };
                Items.Add((uint)myitem.Index, myitem);
            }
        }
        public void ShowBoughtItems(ServerSockets.Packet stream)
        {
            if (ShowItems)
            {
                ShowItems = false;

                foreach (var item in Items.Values)
                {
                    MsgExchangeShopBuy.ExchangeShopBuy _obj = new MsgExchangeShopBuy.ExchangeShopBuy();
                    _obj.DwParam = 19424;
                    _obj.Index = (uint)item.Index;
                    _obj.CountItems = item.BoughtItems;
                    _obj.Type = 1;
                    user.Send(stream.CreateBuyExchangeShop(_obj));
                }
            }
        }
        public override string ToString()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(Items.Count);
            foreach (var item in Items.Values)
                writer.Add(item.Index).Add(item.ItemID).Add(item.BoughtItems);
            return writer.Close();
        }
        public void Load(string Line)
        {
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(Line, '/');
            int count = reader.Read((int)0);
            for (int x = 0; x < count; x++)
            {
                var item = new Item()
                {
                    Index = reader.Read((int)0),
                    ItemID = reader.Read((uint)0),
                    BoughtItems =reader.Read((ushort)0)

                };
                if (!Items.ContainsKey((uint)item.Index))
                    Items.Add((uint)item.Index, item);
            }
        }

    }
}
