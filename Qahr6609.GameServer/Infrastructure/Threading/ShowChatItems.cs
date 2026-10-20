using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609
{
    public class ShowChatItems
    {
        public static void ShowFakeItem(Client.GameClient user,ServerSockets.Packet stream,  uint ItemID, ushort Durability)
        {
            Game.MsgServer.MsgGameItem GameItem = new Game.MsgServer.MsgGameItem();
            GameItem.UID = 1;
            GameItem.Durability = GameItem.MaximDurability = Durability;
            GameItem.ITEM_ID = ItemID;
            GameItem.Mode = Role.Flags.ItemMode.ChatItem;
            GameItem.Send(user, stream);
        }
        public Extensions.Time32 Stamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.ChatItemsStamp);
        public class Item
        {
            public DateTime Stamp = new DateTime();
            public Game.MsgServer.MsgGameItem aItem = null;
        }
        public Extensions.SafeDictionary<uint, Item> Items;

        public ShowChatItems()
        {
            Items = new Extensions.SafeDictionary<uint, Item>();
        }

        public void Add(Game.MsgServer.MsgGameItem GameItem)
        {
            if (!Items.ContainsKey(GameItem.UID))
            {
                Items.Add(GameItem.UID, new Item()
                {
                    Stamp = DateTime.Now.AddHours(5),
                    aItem = GameItem
                });
            }
        }

        public void Work(Extensions.Time32 clock)
        {

            if (clock > Stamp)
            {
                try
                {
                    List<uint> remover = new List<uint>();

                    foreach (var item in Items.GetValues())
                    {
                        if (DateTime.Now > item.Stamp)
                        {
                            remover.Add(item.aItem.UID);
                        }
                    }

                    foreach (var rem in remover)
                        Items.Remove(rem);
                }

                catch (Exception e)
                {
                    MyConsole.WriteException(e);
                }
                Stamp.Value = clock.Value + KernelThread.ChatItemsStamp;
            }
        }
    
    }
}
