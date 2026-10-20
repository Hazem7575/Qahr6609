using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgExchangeShopBuy
    {
        const int MaxInputEntries = 40;
        const uint MaxBuyCount = 40;
        [ProtoContract]
        public class ExchangeShopBuy
        {
            [ProtoMember(1, IsRequired = true)]
            public uint DwParam;
            [ProtoMember(2, IsRequired = true)]
            public uint Type;//1 and 2 for others ranks?
            [ProtoMember(3, IsRequired = true)]
            public uint CountItems;
            [ProtoMember(4, IsRequired = true)]
            public uint Index;

            [ProtoMember(5, IsRequired = true)]
            public Item[] Items;

            [ProtoContract]
            public class Item
            {
                [ProtoMember(1, IsRequired = true)]
                public uint ItemUID;
                [ProtoMember(2, IsRequired = true)]
                public uint Cost;

            }
        }

        public static unsafe ServerSockets.Packet CreateBuyExchangeShop(this ServerSockets.Packet stream, ExchangeShopBuy obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.BuyFromExchangeShop);//2443

            return stream;
        }
        public static unsafe void GetBuyExchangeShop(this ServerSockets.Packet stream, out ExchangeShopBuy pQuery)
        {
            pQuery = new ExchangeShopBuy();
            pQuery = stream.ProtoBufferDeserialize<ExchangeShopBuy>(pQuery);
        }
        [PacketAttribute(GamePackets.BuyFromExchangeShop)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            ExchangeShopBuy pQuery;
            stream.GetBuyExchangeShop(out pQuery);

            // Purchase count and input list sizes must be within inventory limits.
            if (pQuery == null || pQuery.Items == null || pQuery.Items.Length == 0 || pQuery.Items.Length > MaxInputEntries)
                return;
            if (pQuery.CountItems == 0 || pQuery.CountItems > MaxBuyCount)
                return;

            Database.ExchangeShop.Item Item;
            if (!Database.ExchangeShop.Items.TryGetValue((int)pQuery.Index, out Item))
                return;
            Database.ItemType.DBItem goodDb;
            if (!Database.Server.ItemsBase.TryGetValue(Item.ItemID, out goodDb))
                return;

            // Required amount of every currency of the good, in checked 64-bit arithmetic.
            // A good priced in two currencies (e.g. 88 Fortune Coins + 88 Venus` Blessings) costs both.
            var need = new Dictionary<uint, long>();
            foreach (var cost in Item.Costs)
            {
                long amount;
                if (!Conquer6609.Security.Guard.SafeMultiply(cost.Amount, pQuery.CountItems, out amount) || amount <= 0)
                    return;
                need[cost.ItemID] = amount;
            }
            if (need.Count == 0)
                return;

            // Every input entry is validated before any inventory change: distinct UID, owned, one of the
            // good's currency items, and enough stack for its part of the cost.
            var paid = new Dictionary<uint, long>();
            HashSet<uint> seen = new HashSet<uint>();
            foreach (var _items in pQuery.Items)
            {
                if (_items == null || _items.Cost == 0 || _items.Cost > ushort.MaxValue || !seen.Add(_items.ItemUID))
                    return;
                MsgServer.MsgGameItem iTEM;
                if (!client.Inventory.TryGetItem(_items.ItemUID, out iTEM))
                    return;
                if (!need.ContainsKey(iTEM.ITEM_ID))
                {
                    Conquer6609.Security.Guard.Audit("exchange-shop", client.Player.UID, client.Player.Name, "wrong input item " + iTEM.ITEM_ID + " for index " + pQuery.Index);
                    return;
                }
                if (!client.Inventory.ContainItemWithStack(_items.ItemUID, (ushort)_items.Cost))
                    return;
                long sum;
                paid.TryGetValue(iTEM.ITEM_ID, out sum);
                paid[iTEM.ITEM_ID] = sum + _items.Cost;
            }
            long _allcost = 0;
            foreach (var req in need)
            {
                long got;
                if (!paid.TryGetValue(req.Key, out got) || got != req.Value)
                    return;
                _allcost += got;
            }

            // Daily purchase limit (0 = unlimited); the counters reset at the daily reset.
            if (!Item.Unlimited && !client.MyExchangeShop.CanBuyItem(Item, (ushort)pQuery.CountItems))
            {
                client.SendSysMesage("You have reached today`s purchase limit of this item.");
                return;
            }

            int slots = goodDb.StackSize > 1 ? (int)((pQuery.CountItems + goodDb.StackSize - 1) / goodDb.StackSize) : (int)pQuery.CountItems;
            if (!client.Inventory.HaveSpace((byte)Math.Max(1, slots)))
            {
                client.CreateBoxDialog("Please make " + slots + " more space in your inventory.");
                return;
            }

            // Inputs are consumed once, then the reward is granted exactly once.
            foreach (var _items in pQuery.Items)
                client.Inventory.RemoveStackItem(_items.ItemUID, (ushort)_items.Cost, stream);

            if (!Item.Unlimited)
                client.MyExchangeShop.AddItem(Item, (ushort)pQuery.CountItems);

            var dt = DateTime.Now;
            string logs = "[ExChange]" + dt.Hour + "H:" + dt.Minute + "M pid: " + client.Player.UID + " " + client.Player.Name + " [ Use ]  [" + _allcost + "] Of " + string.Join("+", Item.Costs.Select(c => c.ItemID.ToString()).ToArray()) + " For Buy  [" + pQuery.CountItems + "]    " + Item.ItemID + "";
            Database.ServerDatabase.LoginQueue.Enqueue(logs);
            // +1..+9 stones (730001..730009) carry their plus; the Monopoly column makes the good bound.
            byte plus = (Item.ItemID >= 730001 && Item.ItemID <= 730009) ? (byte)(Item.ItemID - 730000) : (byte)0;
            client.Inventory.AddItemWitchStack(Item.ItemID, plus, (ushort)pQuery.CountItems, stream, Item.Bound);

            client.Send(stream.CreateBuyExchangeShop(pQuery));
        }
    }
}
