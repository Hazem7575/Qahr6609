using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {
        public enum ItemUsuageID : uint
        {
            CreateSocketItem = 43,
            BuyItemFromForging = 55,
            GemCompose = 39,
            ToristSuper = 51,
            AddBless = 40,
            GarmentShop = 53,

            BuyItem = 0x01,
            SellItem = 0x02,
            RemoveInventory = 0x03,
            Equip = 0x04,
            SetEquipPosition = 0x05,
            Unequip = 0x06,
            UpgradeEnchant = 0x07,
            ShowWarehouseMoney = 0x09,
            DepositWarehouse = 0x0A,
            WarehouseWithdraw = 11,
            RepairItem = 0x0E,
            VIPRepairItem = 0x0F,
            UpdateDurability = 0x11,
            RemoveEquipment = 0x12,
            UpgradeDragonball = 0x13,
            UpgradeMeteor = 0x14,
            ShowVendingList = 0x15,
            AddVendingItemGold = 0x16,
            RemoveVendingItem = 0x17,
            BuyVendingItem = 0x18,
            UpdateArrowCount = 0x19,
            ParticleEffect = 0x1A,
            Ping = 0x1B,
            UpdateEnchant = 0x1C,
            AddVendingItemConquerPts = 0x1D,
            UpdatePurity = 0x23,
            DropItem = 0x25,
            DropGold = 0x26,
            RedeemGear = 32,
            ClaimGear = 33,
            UnAlternante = 44,
            ActiveItems = 41,
            Alternante = 45,
            SocketTalismanWithItem = 35,
            SocketTalismanWithCPs = 36,
            MergeStackableItems = 48,
            ReturnedItems = 50,
            ShowItem = 52,
            DegradeEquipment = 54,
            OpenInventorySash = 56,
            SplitStack = 49
        }

        public static void GetUsageItem(this ServerSockets.Packet msg, out ItemUsuageID action, out uint id, out ulong dwParam, out uint timestamp, out uint dwParam2, out uint dwParam3, out uint dwparam4, out List<uint> args)
        {
            uint timer = msg.ReadUInt32();//4

            id = msg.ReadUInt32();//8
            dwParam = msg.ReadUInt64();//12
            action = (ItemUsuageID)msg.ReadInt16();//32  //20
            timestamp = msg.ReadUInt32();
            dwParam2 = msg.ReadUInt32();//26
            dwParam3 = msg.ReadUInt32();

            dwparam4 = msg.ReadUInt32();

            msg.SeekForward(12 * sizeof(int));
            msg.SeekForward(sizeof(byte));

            args = new List<uint>();

            if (dwParam2 > 0 && dwParam2 < 50)
            {
                msg.SeekForward(4);
                for (int i = 0; i < dwParam2; i++)
                {
                    args.Add(msg.ReadUInt32());
                }
            }
        }
        public unsafe static ServerSockets.Packet ItemUsageCreate(this ServerSockets.Packet msg, ItemUsuageID action, uint id, ulong dwParam1, uint timestamp, uint dwParam2, uint dwParam3, uint dwparam4, List<uint> args = null)
        {

            msg.InitWriter();

            msg.Write(Extensions.Time32.Now.Value);//4

            msg.Write(id);//8
            msg.Write(dwParam1);//12


            msg.Write((ushort)action);//16
            msg.Write((timestamp != 0) ? timestamp : Extensions.Time32.Now.Value);//20

            msg.Write(dwParam2);
            msg.Write(dwParam3);//28
            msg.Write(dwparam4);//32

            msg.SeekForward(12 * sizeof(int));
            msg.SeekForward(sizeof(int));

            if (args != null)
            {
                foreach (int arg in args)
                    msg.Write(arg);
            }
            msg.Finalize(GamePackets.Usage);


            return msg;
        }

        [PacketAttribute(GamePackets.Usage)]
        public unsafe static void ItemUsuage(Client.GameClient client, ServerSockets.Packet stream)
        {


            ItemUsuageID action;
            uint id;

            ulong dwParam;
            uint timestamp;
            uint dwParam2;
            uint dwParam3;
            uint dwparam4;//unknow
            List<uint> args;


            stream.GetUsageItem(out action, out id, out dwParam, out timestamp, out dwParam2, out dwParam3, out dwparam4, out args);
            if ((client.InTrade || client.IsVendor) && action != ItemUsuageID.Ping && action != ItemUsuageID.AddVendingItemConquerPts && action != ItemUsuageID.AddVendingItemGold && action != ItemUsuageID.RemoveVendingItem)
                return;
            // no booth listing while a trade is open (the same item could be sold twice)
            if (client.InTrade && (action == ItemUsuageID.AddVendingItemConquerPts || action == ItemUsuageID.AddVendingItemGold))
                return;
            switch (action)
            {
                case ItemUsuageID.OpenInventorySash:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        MsgGameItem GameItem;
                        if (client.Inventory.TryGetItem((uint)id, out GameItem))
                        {
                            if (Database.ItemType.IsSash(GameItem.ITEM_ID))
                            {
                                if (GameItem.Deposite.Count > 0)
                                {
                                    client.CreateBoxDialog("Please remove items from your sash.");
                                    break;
                                }

                                ushort open_size = Database.ItemType.GetSashCounts(GameItem.ITEM_ID);
                                if (client.Player.InventorySashCount + open_size < 300)
                                {
                                    client.Player.InventorySashCount += open_size;
                                    client.Player.UpdateInventorySash(stream);
                                    client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.ReturnedItems:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Warehouse.RemoveItem(id, ushort.MaxValue, stream))
                        {

                            client.Send(stream.ItemUsageCreate(ItemUsuageID.ReturnedItems, id, 0, timestamp, 1, 1, 1));
                        }

                        break;
                    }
                case ItemUsuageID.DegradeEquipment:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        MsgGameItem GameItem;
                        if (client.Inventory.TryGetItem((uint)id, out GameItem))
                        {
                            if (GameItem.IsEquip)
                            {
                                if (client.Player.ConquerPoints >= 54)
                                {
                                    client.Player.ConquerPoints -= 54;
                                    GameItem.ITEM_ID = Database.Server.ItemsBase.DowngradeItem(GameItem.ITEM_ID);

                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(GameItem.ITEM_ID, out DBItem))
                                    {
                                        GameItem.Durability = DBItem.Durability;
                                        GameItem.MaximDurability = DBItem.Durability;
                                    }

                                    GameItem.Mode = Role.Flags.ItemMode.Update;
                                    GameItem.Send(client, stream);
                                }
                                else
                                {
                                    client.SendSysMesage("You do not have 54 Conquer Points with you.");
                                }
                            }
                        }
                        break;
                    }


                case ItemUsuageID.ShowItem:
                    {
                        try
                        {
                            ShowChatItems.Item msg_item;
                            if (Program.GlobalItems.Items.TryGetValue((uint)id, out msg_item))
                            {
                                msg_item.aItem.Mode = Role.Flags.ItemMode.ChatItem;

                                msg_item.aItem.Send(client, stream);

                                msg_item.aItem.Mode = Role.Flags.ItemMode.AddItem;
                            }
                        }
                        catch (Exception e)
                        {
                            MyConsole.WriteException(e);
                        }
                        break;
                    }
                case ItemUsuageID.ActiveItems:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        
                        MsgGameItem GameItem;
                        if (client.Inventory.TryGetItem((uint)id, out GameItem))
                        {
                            #if ItemTime
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(GameItem.ITEM_ID, out DBItem))
                            {
                                if (DBItem.Time != 0 && GameItem.Activate == 0)
                                {
                                    GameItem.Activate = 1;
                                    GameItem.EndDate = DateTime.Now.AddMinutes(DBItem.Time);

                                }
                            }
                            #endif
                            switch (GameItem.ITEM_ID)
                            {
                                case 3005061:// BrightActivePack
                                    {
                                        if (client.Inventory.HaveSpace(4))
                                        {
                                            client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);

                                            client.Inventory.Add(stream, 729022, 1);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 1);
                                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream);
                                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 5, stream);
                                            client.Player.ConquerPoints += 7000;

                                        }
                                        else
                                        {
#if Arabic
                                              client.CreateBoxDialog("Please make 4 more spaces in your inventory.");
#else
                                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");
#endif

                                        }
                                        break;
                                    }
                                case 3005062:// FortuneActivePack 
                                    {
                                        if (client.Inventory.HaveSpace(6))
                                        {
                                            client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);

                                            client.Inventory.Add(stream, 729022, 2);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 2);
                                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream);
                                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 5, stream);
                                            //client.Player.ConquerPoints += 10000;

                                        }
                                        else
                                        {
#if Arabic
                                              client.CreateBoxDialog("Please make 6 more spaces in your inventory.");
#else
                                            client.CreateBoxDialog("Please make 6 more spaces in your inventory.");
#endif

                                        }
                                        break;
                                    }
                                case 3005064:// SolarActivePack
                                    {
                                        if (client.Inventory.HaveSpace(11))
                                        {
                                            client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);

                                            client.Inventory.Add(stream, 729023, 4);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 2);
                                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream);
                                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 5, stream);
                                            //client.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2);
                                            //client.Player.ConquerPoints += 20000;

                                        }
                                        else
                                        {
#if Arabic
                                                client.CreateBoxDialog("Please make 11 more spaces in your inventory.");
                                        
#else
                                            client.CreateBoxDialog("Please make 11 more spaces in your inventory.");

#endif
                                        }
                                        break;
                                    }
                                case 3005065:// DivineActivePack
                                    {
                                        if (client.Inventory.HaveSpace(14))
                                        {
                                            client.Inventory.Update(GameItem, Role.Instance.AddMode.REMOVE, stream);

                                            client.Inventory.Add(stream, 729023, 4);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 2);
                                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream);
                                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 5, stream);
                                            //  client.Inventory.Add(stream,Database.ItemType.PowerExpBall, 4);
                                            //client.Player.ConquerPoints += 30000;

                                        }
                                        else
                                        {
#if Arabic
                                                 client.CreateBoxDialog("Please make 14 more spaces in your inventory.");
#else
                                            client.CreateBoxDialog("Please make 14 more spaces in your inventory.");
#endif

                                        }
                                        break;
                                    }
                            }

                        }
                        break;
                    }
                case ItemUsuageID.SplitStack:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        MsgGameItem MainItem = null;
                        MsgGameItem MinorItem = null;
                        if (client.Inventory.TryGetItem((uint)id, out MainItem))
                        {
                            Database.ItemType.DBItem DBItem = null;
                            if (Database.Server.ItemsBase.TryGetValue(MainItem.ITEM_ID, out DBItem))
                            {
                                if (MainItem.StackSize > 1 && MainItem.StackSize <= DBItem.StackSize)
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
#if Arabic
                                          client.SendSysMesage("Please make 1 more space in your inventory.");
#else
                                        client.SendSysMesage("Please make 1 more space in your inventory.");
#endif

                                        break;
                                    }

                                    // Split size is validated on the full value and must leave at least one in the source stack.
                                    if (dwParam > 0 && dwParam <= DBItem.StackSize && dwParam < MainItem.StackSize)
                                    {
                                        ushort Amount = (ushort)dwParam;
                                        MainItem.StackSize -= Amount;
                                        MainItem.Mode = Role.Flags.ItemMode.Update;
                                        MainItem.Send(client, stream);

                                        // The new stack keeps every restriction of the source stack.
                                        MinorItem = new MsgGameItem();
                                        MinorItem.ITEM_ID = MainItem.ITEM_ID;
                                        MinorItem.StackSize += Amount;
                                        MinorItem.Durability = MainItem.Durability;
                                        MinorItem.MaximDurability = MainItem.MaximDurability;
                                        MinorItem.Plus = MainItem.Plus;
                                        MinorItem.Bound = MainItem.Bound;
                                        MinorItem.Locked = MainItem.Locked;
                                        MinorItem.UnLockTimer = MainItem.UnLockTimer;
                                        MinorItem.Inscribed = MainItem.Inscribed;
                                        MinorItem.Activate = MainItem.Activate;
                                        MinorItem.EndDate = MainItem.EndDate;
                                        client.Inventory.Update(MinorItem, Role.Instance.AddMode.ADD, stream);
                                    }
                                    else
                                    {
                                        Conquer6609.Security.Guard.Audit("stack-split", client.Player.UID, client.Player.Name, "rejected split " + dwParam + " of " + MainItem.StackSize);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.MergeStackableItems:
                    {
                        if (client.Player.OnMyOwnServer == false)
                            break;
                        MsgGameItem MainItem = null;
                        MsgGameItem MinorItem = null;
                        if (client.Inventory.TryGetItem((uint)id, out MainItem) && client.Inventory.TryGetItem((uint)dwParam, out MinorItem))
                        {
                            if (MainItem.UID != MinorItem.UID)
                            {
                                // Only stacks with identical bound/activation/lock/expiry state can be merged.
                                if (MainItem.ITEM_ID == MinorItem.ITEM_ID && MainItem.Bound == MinorItem.Bound
                                    && MainItem.Activate == MinorItem.Activate && MainItem.Locked == MinorItem.Locked
                                    && MainItem.EndDate == MinorItem.EndDate)
                                {
                                    Database.ItemType.DBItem DBItem = null;
                                    if (Database.Server.ItemsBase.TryGetValue(MainItem.ITEM_ID, out DBItem))
                                    {
                                        if (MainItem.StackSize < 1)
                                            MainItem.StackSize = 1;
                                        if (MinorItem.StackSize < 1)
                                            MinorItem.StackSize = 1;

                                        if ((MainItem.StackSize + MinorItem.StackSize) <= DBItem.StackSize)
                                        {
                                            MainItem.StackSize += MinorItem.StackSize;
                                            MainItem.Mode = Role.Flags.ItemMode.Update;
                                            MainItem.Send(client, stream).Update(MinorItem, Role.Instance.AddMode.REMOVE, stream, true);
                                        }
                                        /*  else
                                           {
                                               ushort removestak = (ushort)(DBItem.StackSize - MainItem.StackSize);
                                               if (MinorItem.StackSize > removestak)
                                               {
                                                   MainItem.StackSize += removestak;
                                                   MainItem.Mode = Role.Flags.ItemMode.Update;
                                                   MainItem.Send(client, stream);

                                                   MinorItem.StackSize -= removestak;
                                                   MainItem.Mode = Role.Flags.ItemMode.Update;
                                                   MainItem.Send(client, stream);
                                                   if (MinorItem.StackSize == 0)
                                                       client.Inventory.Update(MainItem, Role.Instance.AddMode.REMOVE, stream);
                                               }
                                           }*/
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.RedeemGear:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        Game.MsgServer.MsgDetainedItem DetainedItem;
                        if (client.Confiscator.RedeemContainer.TryGetValue((uint)id, out DetainedItem))
                        {
                            DetainedItem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(DetainedItem.Date).Ticks).Days);
                            if (DetainedItem.DaysLeft > 7)
                            {
#if Arabic
                                  client.SendSysMesage("This item is expired!");
#else
                                client.SendSysMesage("This item is expired!");
#endif

                                break;
                            }
                            if (!client.Inventory.HaveSpace(1))
                            {
#if Arabic
                                                                client.SendSysMesage("Please make 1 space in your container!");
#else
                                client.SendSysMesage("Please make 1 more space in your container!");
#endif

                                break;
                            }
                            if (client.Player.ConquerPoints >= Role.Instance.Confiscator.CalculateCpsCost(DetainedItem))
                            {
                                client.Player.ConquerPoints -= (uint)Role.Instance.Confiscator.CalculateCpsCost(DetainedItem);


                                dwParam = client.Player.UID;
                                dwparam4 = (uint)Role.Instance.Confiscator.CalculateCpsCost(DetainedItem);

                                client.Send(stream.ItemUsageCreate(ItemUsuageID.RedeemGear, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));


                                client.Inventory.Update(MsgDetainedItem.CopyTo(DetainedItem), Role.Instance.AddMode.ADD, stream);

#if Arabic
  string Messajj = "" + client.Player.Name + " redeemed his equipment (" + Database.Server.ItemsBase.GetItemName(DetainedItem.ItemID) + "), he paying " + Role.Instance.Confiscator.CalculateCpsCost(DetainedItem) + " conquer points for " + DetainedItem.GainerName + ".";

#else
                                string Messajj = "" + client.Player.Name + " redeemed his equipment (" + Database.Server.ItemsBase.GetItemName(DetainedItem.ItemID) + "), he paying " + Role.Instance.Confiscator.CalculateCpsCost(DetainedItem) + " conquer points for " + DetainedItem.GainerName + ".";

#endif

                                //string Messaj = "" + client.Player.Name + " redeemed his equipment, hereby obtained a ransom of " + DetainedItem.ConquerPointsCost + " points, all the days Stone Award to seize its equipment to help players " + DetainedItem.GainerName + "";
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(Messajj, MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));

                                if (client.Confiscator.RedeemContainer.TryRemove(DetainedItem.UID, out DetainedItem))
                                {
                                    Role.Instance.Confiscator GainerCointainer;
                                    if (Database.Server.QueueContainer.PollContainers.TryGetValue(DetainedItem.GainerUID, out GainerCointainer))
                                    {
                                        if (GainerCointainer.ClaimContainer.ContainsKey(DetainedItem.UID))
                                        {
                                            DetainedItem.Action = MsgDetainedItem.ContainerType.RewardCps;
                                            DetainedItem.RewardConquerPoints = Role.Instance.Confiscator.CalculateCpsCost(DetainedItem);
                                            GainerCointainer.ClaimContainer[DetainedItem.UID] = DetainedItem;

                                            Client.GameClient Gainer;
                                            if (Database.Server.GamePoll.TryGetValue(DetainedItem.GainerUID, out Gainer))
                                            {
                                                dwParam = Gainer.Player.UID;
                                                action = ItemUsuageID.ClaimGear;

                                                Gainer.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));



                                                GainerCointainer.ClaimContainer[DetainedItem.UID].Send(Gainer, stream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.ClaimGear:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (!client.Inventory.HaveSpace(1))
                        {
#if Arabic
                              client.SendSysMesage("Please make 1 space in your container!");
#else
                            client.SendSysMesage("Please make 1 more space in your container!");
#endif

                            break;
                        }

                        Game.MsgServer.MsgDetainedItem ClaimItem;
                        if (client.Confiscator.ClaimContainer.TryGetValue(id, out ClaimItem))
                        {
                            if (ClaimItem.Bound && ClaimItem.DaysLeft > 7)
                            {

                                dwParam = client.Player.UID;

                                dwparam4 = (uint)ClaimItem.RewardConquerPoints;


                                client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));

                                client.Confiscator.ClaimContainer.TryRemove(ClaimItem.UID, out ClaimItem);
#if Arabic
                                 client.SendSysMesage("Unnclaimable Bound item!");
#else
                                client.SendSysMesage("Unnclaimable Bound item!");
#endif

                                break;
                            }
                            ClaimItem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(ClaimItem.Date).Ticks).Days);
                            if (ClaimItem.DaysLeft < 7 && ClaimItem.Action != MsgDetainedItem.ContainerType.RewardCps)
                            {
#if Arabic
                                 client.SendSysMesage("This item is not expired. You cannot claim it yet!");
#else
                                client.SendSysMesage("This item is not expired. You cannot claim it yet!");
#endif

                                break;
                            }
                            if (ClaimItem.RewardConquerPoints != 0)
                            {
                                client.Player.ConquerPoints += (uint)ClaimItem.RewardConquerPoints;

                                client.Confiscator.ClaimContainer.TryRemove(ClaimItem.UID, out ClaimItem);
                            }
                            else if (ClaimItem.DaysLeft > 7)
                            {
                                client.Inventory.Update(MsgDetainedItem.CopyTo(ClaimItem), Role.Instance.AddMode.ADD, stream);
                                client.Confiscator.ClaimContainer.TryRemove(ClaimItem.UID, out ClaimItem);
                            }
#if Arabic
                              string Messaj = "Congratulation! " + client.Player.Name + " has received " + ClaimItem.RewardConquerPoints + " conquer points for capture the red/black player called " + ClaimItem.OwnerName + "";//"Thank you for arresting red/black name players " + client.Entity.Name + " has recived " + item.ConquerPointsCost + " CPS . Congratulations!";
                           
#else
                            string Messaj = "Congratulation! " + client.Player.Name + " has received " + ClaimItem.RewardConquerPoints + " conquer points for capture the red/black player called " + ClaimItem.OwnerName + "";//"Thank you for arresting red/black name players " + client.Entity.Name + " has recived " + item.ConquerPointsCost + " CPS . Congratulations!";

#endif
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(Messaj, MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));

                            dwParam = client.Player.UID;
                            dwparam4 = (uint)ClaimItem.RewardConquerPoints;
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));

                        }
                        break;
                    }
                case ItemUsuageID.GarmentShop:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (dwParam > uint.MaxValue)
                            break;
                        uint GarmentID = (uint)dwParam;
                        bool RightInfo = true;
                        if (Database.ItemType.ItemPosition(GarmentID) == (ushort)Role.Flags.ConquerItem.Garment)
                        {
                            Database.ItemType.DBItem DBItem;
                            if (!Database.Server.ItemsBase.TryGetValue(GarmentID, out DBItem))
                                break;
                            // Only garments with a CP price can be obtained here.
                            if (DBItem.ConquerPointsWorth == 0)
                                break;
                            // A free slot is only needed without trade-ins: removing a traded-in garment frees one.
                            if ((args == null || args.Count == 0) && !client.Inventory.HaveSpace(1))
                            {
                                client.SendSysMesage("You do not have enough space in your inventory");
                                break;
                            }
                            uint Points = 0;

                            // Each traded-in item (by UID) counts once and must be a garment in the inventory;
                            // trading in a copy of the garment being bought is allowed (the new one gets a new UID).
                            HashSet<uint> SeenUID = new HashSet<uint>();
                            List<Game.MsgServer.MsgGameItem> RemoveItem = new List<MsgGameItem>();
                            if (args == null || args.Count > 40)
                                RightInfo = false;
                            else foreach (var itemUID in args)
                            {
                                Game.MsgServer.MsgGameItem item;
                                // [Security] bound / locked / inscribed garments can not be traded in (the new garment is unbound)
                                if (SeenUID.Add(itemUID) && client.Inventory.TryGetItem(itemUID, out item)
                                    && Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment
                                    && item.Bound == 0 && item.Locked == 0 && item.Inscribed == 0)
                                {
                                    switch (item.ITEM_ID)
                                    {
                                        case 188435://188435
                                        case 181355:
                                        case 182435: Points += 150; break;

                                        case 191305:
                                        case 191405:
                                        case 183305:
                                        case 183375:
                                        case 183315:
                                        case 183325: Points += 300; break;
                                        default:
                                            Points += 50;
                                            break;
                                    }
                                    RemoveItem.Add(item);
                                }
                                else
                                {
                                    RightInfo = false;
                                    break;
                                }
                            }
                            if (RightInfo)
                            {
                                uint price = DBItem.ConquerPointsWorth;
                                // Trade-in points can at most cover the full price.
                                if (Points > price)
                                    Points = price;
                                uint cpCost = price - Points;
                                if (client.Player.ConquerPoints >= cpCost)
                                {
                                    // Traded-in items are removed before the new garment is granted; abort if any removal fails.
                                    bool removed = true;
                                    foreach (var item in RemoveItem)
                                    {
                                        if (!client.Inventory.ClientItems.ContainsKey(item.UID))
                                        {
                                            removed = false;
                                            break;
                                        }
                                    }
                                    if (removed)
                                    {
                                        foreach (var item in RemoveItem)
                                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
                                        foreach (var item in RemoveItem)
                                        {
                                            if (client.Inventory.ClientItems.ContainsKey(item.UID))
                                            {
                                                removed = false;
                                                break;
                                            }
                                        }
                                    }
                                    if (!removed)
                                    {
                                        Conquer6609.Security.Guard.Audit("garment-exchange", client.Player.UID, client.Player.Name, "trade-in removal failed for " + GarmentID);
                                        break;
                                    }

                                    if (cpCost > 0)
                                        client.Player.ConquerPoints -= cpCost;

                                    client.Inventory.Add(stream, GarmentID, 1);
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.AddBless:
                    {
                        if (client.IsConnectedInterServer())
                            break;

                        MsgGameItem DataItem;
                        if (client.TryGetItem(id, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                            if (Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                            {
                                if (Database.ItemType.IsHossu(DataItem.ITEM_ID) && DataItem.Bless == 1)
                                {
                                    return;
                                }
                                Queue<MsgGameItem> LoseItems;
                                if (client.Inventory.VerifiedUpdateItem(args, Database.ItemType.GetGemID(Role.Flags.Gem.SuperTortoiseGem), (byte)dwParam2, out LoseItems))
                                {
                                    if (Position <= 0 || !client.Inventory.DoSvae(LoseItems, client) || !Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position) || DataItem.Bless >= 7)
                                    {
                                        Console.WriteLine(client.Player.Name + "This Player Youse Hack AddBless");
                                        client.Socket.Disconnect();
                                        return;
                                    }
                                    foreach (var item in LoseItems)
                                    {
                                        ushort Position2 = Database.ItemType.ItemPosition(item.ITEM_ID);
                                        if (item.ITEM_ID % 10000 != 73 || Position2 != 0)
                                        {
                                            Console.WriteLine(client.Player.Name + "This Player Youse Hack AddBless");
                                            client.Socket.Disconnect();
                                            return;
                                        }
                                    }

                                    byte OldBless = DataItem.Bless;
                                    if (DataItem.Bless == 0 && LoseItems.Count == 5)
                                        DataItem.Bless = 1;
                                    else if (DataItem.Bless == 1 && LoseItems.Count == 1)
                                        DataItem.Bless = 3;
                                    else if (DataItem.Bless == 3 && LoseItems.Count == 3)
                                        DataItem.Bless = 5;
                                    else if (DataItem.Bless == 5 && LoseItems.Count == 5)
                                        DataItem.Bless = 7;
                                    if (OldBless < DataItem.Bless)
                                    {
                                        DataItem.Mode = Role.Flags.ItemMode.Update;
                                        DataItem.Send(client, stream);
                                        if (DataItem.Position != 0)
                                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                        while (LoseItems.Count > 0)
                                            client.Inventory.Update(LoseItems.Dequeue(), Role.Instance.AddMode.REMOVE, stream);


                                        client.Send(stream.ItemUsageCreate(ItemUsuageID.AddBless, 0, 1, 0, 0, 0, 0));
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.ToristSuper:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (id % 1000 == 0)
                        {
                            Console.WriteLine(client.Player.Name + "This Player Youse Hack items");
                            client.Socket.Disconnect();
                            return;
                        }
                        if (client.Player.Money >= 100000)
                        {
                            client.Player.Money -= 100000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            bool HaveAllGems = false;
                            for (uint x = 0; x < 7; x++)
                            {
                                uint ItemID = 700002 + x * 10;
                                if (client.Inventory.Contain(ItemID, 1))
                                    HaveAllGems = true;
                                else
                                {
                                    HaveAllGems = false;
                                    break;
                                }
                            }
                            if (HaveAllGems)
                            {
                                for (uint x = 0; x < 7; x++)
                                {
                                    uint ItemID = 700002 + x * 10;
                                    client.Inventory.Remove(ItemID, 1, stream);
                                }
                                client.Inventory.Add(stream, 700072, 1);

                                client.Send(stream.ItemUsageCreate(ItemUsuageID.ToristSuper, 0, 1, 0, 0, 0, 0));
                            }
                        }
                        else
                        {
#if Arabic
                             client.SendSysMesage("Sorry you don`t have 100,000 silver!.");
#else
                            client.SendSysMesage("Sorry you don`t have 100,000 silver!.");
#endif

                        }
                        break;
                    }
                case ItemUsuageID.GemCompose:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (id % 1000 == 0)
                        {
                            Console.WriteLine(client.Player.Name + "This Player Youse Hack GemCompose");
                            client.Socket.Disconnect();
                            return;
                        }
                        if (id / 10000 == 70 && Enum.IsDefined(typeof(Role.Flags.Gem), (byte)(id % 1000)))
                        {
                            uint price = 0;
                            if (id % 10 == 1)
                                price = 10000;
                            else if (id % 10 == 2)
                            {
                                price = 800000;
                                if (id % 100 == 72)
                                    price = 1000000;
                            }

                            if (client.Player.Money >= price)
                            {
                                if (client.Inventory.Contain(id, 15))
                                {
                                    List<uint> items = new List<uint>();
                                    // The composed gem is bound when any of the 15 consumed gems is bound.
                                    bool composeBound = false;
                                    foreach (var item in client.Inventory.ClientItems.Values)
                                    {
                                        if (item.ITEM_ID == id)
                                        {
                                            if (items.Count < 15 && item.Bound != 0)
                                                composeBound = true;
                                            if (!items.Contains(item.UID))
                                                items.Add(item.UID);
                                            else
                                            {
                                                Console.WriteLine(client.Player.Name + "This Player Youse Hack GemCompose");
                                                client.Socket.Disconnect();
                                                return;
                                            }
                                        }
                                    }
                                    if (client.Inventory.Remove(id, 15, stream))
                                    {
                                        client.Inventory.Add(stream, (uint)(id + 1), 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, composeBound);

                                        dwParam = 1;
                                        client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4, args));

                                        client.Player.Money -= (uint)price;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    }
                                }
                            }
                            else
                                client.SendSysMesage("Sorry you don`t have 100,000 silver!.");
                        }
                        break;
                    }
                case ItemUsuageID.UpdateEnchant:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        MsgGameItem DataItem;
                        if (client.TryGetItem(id, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                            if (Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                            {
                                MsgGameItem Gem;
                                if (client.Inventory.ClientItems.TryGetValue((uint)dwParam, out Gem))
                                {
                                    if (Enum.IsDefined(typeof(Role.Flags.Gem), (byte)(Gem.ITEM_ID % 1000)))
                                    {
                                        if (Gem.ITEM_ID % 1000 == 0 || Position <= 0)
                                        {
                                            Console.WriteLine(client.Player.Name + "This Player Youse Hack UpdateEnchant");
                                            client.Socket.Disconnect();
                                            return;
                                        }
                                        byte Enchant = 0;
                                        switch (Gem.ITEM_ID % 10)
                                        {
                                            case 1:
                                                {
                                                    Enchant = (byte)Program.GetRandom.Next(1, 59);
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    if (Gem.ITEM_ID == 700012)
                                                        Enchant = (byte)Program.GetRandom.Next(100, 159);
                                                    else if (Gem.ITEM_ID == 700002 || Gem.ITEM_ID == 700052 || Gem.ITEM_ID == 700062)
                                                        Enchant = (byte)Program.GetRandom.Next(60, 109);
                                                    else if (Gem.ITEM_ID == 700032)
                                                        Enchant = (byte)Program.GetRandom.Next(80, 129);
                                                    else
                                                        Enchant = (byte)Program.GetRandom.Next(40, 89);
                                                    break;
                                                }
                                            default:
                                                {
                                                    if (Gem.ITEM_ID == 700013)
                                                        Enchant = (byte)Program.GetRandom.Next(200, 256);
                                                    else if (Gem.ITEM_ID == 700003 || Gem.ITEM_ID == 700073 || Gem.ITEM_ID == 700033)
                                                        Enchant = (byte)Program.GetRandom.Next(170, 229);
                                                    else if (Gem.ITEM_ID == 700063 || Gem.ITEM_ID == 700053)
                                                        Enchant = (byte)Program.GetRandom.Next(140, 199);
                                                    else if (Gem.ITEM_ID == 700023)
                                                        Enchant = (byte)Program.GetRandom.Next(90, 149);
                                                    else
                                                        Enchant = (byte)Program.GetRandom.Next(70, 119);
                                                    break;
                                                }
                                        }
                                        if (Enchant > DataItem.Enchant)
                                        {
                                            DataItem.Enchant = Enchant;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(client, stream).Update(Gem, Role.Instance.AddMode.REMOVE, stream);
                                            if (DataItem.Position != 0)
                                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                        }
                                        else
                                        {
                                            client.Inventory.Update(Gem, Role.Instance.AddMode.REMOVE, stream);
                                        }

                                        dwParam = Enchant;
                                        client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                                    }
                                }
                            }
                        }

                        break;
                    }
                case ItemUsuageID.BuyItemFromForging:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (dwParam > uint.MaxValue)
                            break;
                        uint ItemID = (uint)dwParam;
                        uint ItemsCount = dwParam2;
                        // Count must fit in the inventory before any space check or purchase.
                        if (ItemsCount == 0 || ItemsCount > 40)
                            break;
                        if (ItemID == 1088001 || ItemID == 1088000 || ItemID == 730001 || ItemID == 730003 || ItemID == 730006
                                            || ItemID == 723694 || ItemID == 723695 || ItemID == 700073 || ItemID == 1200005)
                        {
                            if (client.Inventory.HaveSpace((byte)ItemsCount))
                            {
                                Database.ItemType.DBItem DBItem = null;
                                if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                                {
                                    for (int x = 0; x < ItemsCount; x++)
                                    {
                                        if (client.Player.ConquerPoints >= DBItem.ConquerPointsWorth)
                                        {
                                            // Charged only once the item was actually added.
                                            bool added;
                                            if ((ItemID % 730000) <= 9)
                                                added = client.Inventory.Add(DBItem.ID, (byte)(ItemID % 730000), DBItem, stream);
                                            else
                                                added = client.Inventory.Add(DBItem.ID, 0, DBItem, stream);
                                            if (!added)
                                                break;
                                            client.Player.ConquerPoints -= DBItem.ConquerPointsWorth;
                                        }
                                        else break;
                                    }
                                }

                            }
                        }
                        break;
                    }
                case ItemUsuageID.CreateSocketItem:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (args == null || args.Count == 0)// [Security] args[0] below: an empty list threw on every packet
                            break;
                        uint effectuid = 0;
                        uint effectdwparam1 = 0;
                        MsgGameItem DataItemm;
                        if (client.TryGetItem(id, out DataItemm))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItemm.ITEM_ID);
                            if (Position <= 0 || (client.Inventory.HasDuplicates(args)))
                            {
                                Console.WriteLine("fuck player " + client.Player.Name + " try use Socket bug");
                                string log = ("[Hack]" + client.Player.Name + "He Try To Use hack Socket Bug ");
                                Logs.OnDequeue(log);
                                client.Socket.Disconnect();
                                return;
                            }
                        }
                        MsgGameItem Item;
                        if (client.TryGetItem(args[0], out Item))
                        {
                            if (Item.ITEM_ID == 1088000 && args.Count == 1)
                            {
                                dwParam2 = 1;
                            }
                            else if (Item.ITEM_ID == 1088000 && args.Count == 5)
                            {
                                dwParam2 = 5;
                            }
                            else if (Item.ITEM_ID == 1088000 && args.Count == 12)
                            {
                                dwParam2 = 12;
                            }
                            else if (Item.ITEM_ID == Database.ItemType.ToughDrill && args.Count == 1)
                            {
                                dwParam2 = 2;
                            }
                            else if (Item.ITEM_ID == Database.ItemType.StarDrill && args.Count == 7)
                            {
                                dwParam2 = 7;
                            }
                        }


                        switch (dwParam2)
                        {

                            case 1:
                                {
                                    //سلاح سوكت واحد
                                    MsgGameItem DataItem;
                                    if (client.TryGetItem(id, out DataItem))
                                    {
                                        effectuid = DataItem.UID;

                                        ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                                        if (Position != (ushort)Role.Flags.ConquerItem.LeftWeapon && Position != (ushort)Role.Flags.ConquerItem.RightWeapon)
                                        {
                                            client.SendSysMesage("Sorry can't make socket in this item.");
                                            Console.WriteLine("fuck player " + client.Player.Name + " try use CreateSocketItem bug");
                                            string log = ("[Hack]" + client.Player.Name + "He Try To Use hack CreateSocketItem Bug ");
                                            Logs.OnDequeue(log);
                                            client.Socket.Disconnect();
                                            return;
                                        }
                                        if (Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                                        {
                                            if (DataItem.SocketOne == Role.Flags.Gem.NoSocket)
                                            {
                                                MsgGameItem LoseItem;

                                                if (client.Inventory.ClientItems.TryGetValue(args[0], out LoseItem))
                                                {
                                                    if (LoseItem.ITEM_ID == Database.ItemType.DragonBall)
                                                    {
                                                        DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                                        DataItem.Mode = Role.Flags.ItemMode.Update;

                                                        if (DataItem.Position != 0)
                                                            client.Equipment.QueryEquipment(client.Equipment.Alternante);

                                                        effectdwparam1 = 1;
                                                        DataItem.Send(client, stream).Update(LoseItem, Role.Instance.AddMode.REMOVE, stream);
                                                    }
                                                }
                                            }

                                        }
                                        else
                                        {
#if Arabic
                                              client.SendSysMesage("Sorry can't make socket in this item !");
#else
                                            client.SendSysMesage("Sorry can't make socket in this item !");
#endif

                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    //ايتم  سوكت اثنين
                                    MsgGameItem DataItem;
                                    if (client.TryGetItem(id, out DataItem))
                                    {
                                        effectuid = DataItem.UID;


                                        if (DataItem.SocketOne != Role.Flags.Gem.NoSocket && DataItem.SocketTwo == Role.Flags.Gem.NoSocket)// [Security] a 2nd socket needs the 1st
                                        {
                                            MsgGameItem LoseItem;
                                            if (client.Inventory.ClientItems.TryGetValue(args[0], out LoseItem))
                                            {
                                                if (LoseItem.ITEM_ID == Database.ItemType.ToughDrill)
                                                {
                                                    if (Role.Core.Rate(20))
                                                    {
                                                        effectdwparam1 = 1;

                                                        DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                                        DataItem.Mode = Role.Flags.ItemMode.Update;
                                                        DataItem.Send(client, stream).Update(LoseItem, Role.Instance.AddMode.REMOVE, stream);
                                                        if (DataItem.Position != 0)
                                                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                                    }
                                                    else
                                                    {
                                                        bool bound = LoseItem.Bound == 1 ? true : false;
                                                        client.Inventory.Update(LoseItem, Role.Instance.AddMode.REMOVE, stream);
                                                        client.Inventory.Add(stream, Database.ItemType.StarDrill, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        client.SendSysMesage("Sorry can't make socket in this item !");

                                    }

                                    break;
                                }
                            case 5:
                                { // سلاح سوكت 2
                                    MsgGameItem DataItem;
                                    if (client.TryGetItem(id, out DataItem))
                                    {
                                        effectuid = DataItem.UID;

                                        ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                                        if (Position != (ushort)Role.Flags.ConquerItem.LeftWeapon && Position != (ushort)Role.Flags.ConquerItem.RightWeapon)
                                        {
                                            client.SendSysMesage("Sorry can't make socket in this item.");
                                            Console.WriteLine("fuck player " + client.Player.Name + " try use CreateSocketItem bug");
                                            string log = ("[Hack]" + client.Player.Name + "He Try To Use hack CreateSocketItem Bug ");
                                            Logs.OnDequeue(log);
                                            client.Socket.Disconnect();
                                            return;
                                        }
                                        if (Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                                        {
                                            if (DataItem.SocketOne != Role.Flags.Gem.NoSocket && DataItem.SocketTwo == Role.Flags.Gem.NoSocket)// [Security] a 2nd socket needs the 1st
                                            {
                                                Queue<MsgGameItem> LoseItems;
                                                if (client.Inventory.VerifiedUpdateItem(args, Database.ItemType.DragonBall, (byte)dwParam2, out LoseItems))
                                                {
                                                    effectdwparam1 = 1;

                                                    DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream);
                                                    if (DataItem.Position != 0)
                                                        client.Equipment.QueryEquipment(client.Equipment.Alternante);

                                                    while (LoseItems.Count > 0)
                                                        client.Inventory.Update(LoseItems.Dequeue(), Role.Instance.AddMode.REMOVE, stream);
                                                }
                                            }
                                        }
                                        else
                                        {
#if Arabic
                                             client.SendSysMesage("Sorry can't make socket in this item !");
#else
                                            client.SendSysMesage("Sorry can't make socket in this item !");
#endif

                                        }
                                    }
                                    break;
                                }
                            case 7:
                                {
                                    MsgGameItem DataItem;
                                    if (client.TryGetItem(id, out DataItem))
                                    {
                                        effectuid = DataItem.UID;

                                        ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                                        if (Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                                        {
                                            if (DataItem.SocketOne != Role.Flags.Gem.NoSocket && DataItem.SocketTwo == Role.Flags.Gem.NoSocket)// [Security] a 2nd socket needs the 1st
                                            {
                                                Queue<MsgGameItem> LoseItems;
                                                if (client.Inventory.VerifiedUpdateItem(args, Database.ItemType.StarDrill, (byte)dwParam2, out LoseItems))
                                                {
                                                    effectdwparam1 = 1;

                                                    DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream);
                                                    if (DataItem.Position != 0)
                                                        client.Equipment.QueryEquipment(client.Equipment.Alternante);

                                                    while (LoseItems.Count > 0)
                                                        client.Inventory.Update(LoseItems.Dequeue(), Role.Instance.AddMode.REMOVE, stream);
                                                }
                                            }
                                        }
                                        else
                                        {
#if Arabic
                                             client.SendSysMesage("Sorry can't make socket in this item !");
#else
                                            client.SendSysMesage("Sorry can't make socket in this item !");
#endif

                                        }
                                    }
                                    break;
                                }
                            case 12:
                                {
                                    MsgGameItem DataItem;
                                    if (client.TryGetItem(id, out DataItem))
                                    {
                                        effectuid = DataItem.UID;

                                        ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                                        if (Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                                        {
                                            if (DataItem.SocketOne == Role.Flags.Gem.NoSocket)
                                            {
                                                Queue<MsgGameItem> LoseItems;
                                                if (client.Inventory.VerifiedUpdateItem(args, Database.ItemType.DragonBall, (byte)dwParam2, out LoseItems))
                                                {
                                                    effectdwparam1 = 1;

                                                    DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream);
                                                    if (DataItem.Position != 0)
                                                        client.Equipment.QueryEquipment(client.Equipment.Alternante);

                                                    while (LoseItems.Count > 0)
                                                        client.Inventory.Update(LoseItems.Dequeue(), Role.Instance.AddMode.REMOVE, stream);
                                                }
                                            }
                                        }
                                        else
                                        {

                                            client.SendSysMesage("Sorry can't make socket in this item !");

                                        }
                                    }
                                    break;
                                }
                        }
                        client.Send(stream.ItemUsageCreate(ItemUsuageID.CreateSocketItem, effectuid, effectdwparam1, 0, 0, 0, 0));

                        break;
                    }
                case ItemUsuageID.RemoveVendingItem:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (client.IsVendor)
                        {
                            Role.Instance.Vendor.VendorItem VItem = null;
                            if (client.MyVendor.Items.TryRemove(id, out VItem))
                            {
                                client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                            }
                        }
                        break;
                    }
                case ItemUsuageID.BuyVendingItem:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (client.Inventory.HaveSpace(1))
                        {
                            Role.IMapObj Obj;
                            if (client.Player.View.TryGetValue((uint)dwParam, out Obj, Role.MapObjectType.SobNpc))
                            {
                                Role.SobNpc npc = Obj as Role.SobNpc;
                                if (npc.Booth != null)
                                {
                                    if (npc.Booth.ItemList.ContainsKey(id))
                                    {
                                        BoothItem item;
                                        if (npc.Booth.ItemList.TryGetValue(id, out item))
                                        {
                                            if (item.Cost_Type == MsgItemView.ActionMode.CPs)
                                            {
                                                if (client.Player.ConquerPoints >= item.Cost)
                                                {
                                                    client.Player.ConquerPoints -= (uint)item.Cost;
                                                    client.Inventory.Add(stream, item.Item.ITEM_ID, (byte)item.Item.StackSize, item.Item.Plus, item.Item.Bless, item.Item.Enchant, item.Item.SocketOne, item.Item.SocketTwo, item.Item.Bound == 0 ? false : true, item.Item.Effect);
                                                }
                                            }
                                            else if (item.Cost_Type == MsgItemView.ActionMode.Gold)
                                            {
                                                if (client.Player.Money >= item.Cost)
                                                {
                                                    client.Player.Money -= item.Cost;
                                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                                    client.Inventory.Add(stream, item.Item.ITEM_ID, (byte)item.Item.StackSize, item.Item.Plus, item.Item.Bless, item.Item.Enchant, item.Item.SocketOne, item.Item.SocketTwo, item.Item.Bound == 0 ? false : true, item.Item.Effect);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (npc.OwnerVendor.IsVendor)
                                {
                                    if (npc.OwnerVendor.Inventory.ClientItems.ContainsKey(id))
                                    {
                                        Role.Instance.Vendor.VendorItem VItem = null;
                                        if (npc.OwnerVendor.MyVendor.Items.TryGetValue(id, out VItem))
                                        {
                                            if (npc.OwnerVendor.Player.UID == client.Player.UID)
                                            {
                                                MyConsole.WriteLine("Thi Fuck Man" + client.Player.Name + " he use New FuckMan Bug ", ConsoleColor.DarkYellow);
                                                Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 1000, "Shop Bug");
                                                client.Socket.Disconnect();
                                                return;
                                            }

                                            if (client.Player.Map == 1036)
                                            {

                                                if (!(Role.Core.GetDistance(npc.OwnerVendor.Player.X, npc.OwnerVendor.Player.Y, client.Player.X, client.Player.Y) <= 18))
                                                {
                                                    client.SendSysMesage("Sorry but you are far away you should be close to the seller");
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                // a booth outside the Market: refuse the purchase. This used to ban the
                                                // buyer for 1000 h, so anyone could get innocent buyers banned.
                                                break;
                                            }

                                            // Unsellable listings are refused while still on the booth (never removed).
                                            if (VItem.AmountCost <= 0 || VItem.DataItem.Bound != 0)
                                                break;
                                            // The listing is taken off the booth before payment so it can only be sold once;
                                            // it is put back if the buyer cannot pay.
                                            if (!npc.OwnerVendor.MyVendor.Items.TryRemove(id, out VItem))
                                                break;
                                            if (VItem.AmountCost <= 0 || VItem.DataItem.Bound != 0)
                                            {
                                                npc.OwnerVendor.MyVendor.Items.TryAdd(id, VItem);
                                                break;
                                            }
                                            bool RightBuy = false;
                                            if (VItem.CostType == MsgItemView.ActionMode.CPs)
                                            {
                                                if (RightBuy = (client.Player.ConquerPoints >= VItem.AmountCost
                                                    && (long)npc.OwnerVendor.Player.ConquerPoints + VItem.AmountCost <= int.MaxValue))
                                                {
                                                    client.Player.ConquerPoints -= (uint)VItem.AmountCost;
                                                    npc.OwnerVendor.Player.ConquerPoints += (uint)VItem.AmountCost;
                                                }
                                            }
                                            else if (VItem.CostType == MsgItemView.ActionMode.Gold)
                                            {
                                                if (RightBuy = (client.Player.Money >= VItem.AmountCost))
                                                {
                                                    client.Player.Money -= VItem.AmountCost;
                                                    npc.OwnerVendor.Player.Money += VItem.AmountCost;

                                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                                    npc.OwnerVendor.Player.SendUpdate(stream, npc.OwnerVendor.Player.Money, MsgUpdate.DataType.Money);
                                                }
                                            }
                                            if (!RightBuy)
                                                npc.OwnerVendor.MyVendor.Items.TryAdd(id, VItem);
                                            if (RightBuy)
                                            {
                                                {
                                                    client.Inventory.Update(VItem.DataItem, Role.Instance.AddMode.MOVE, stream);



                                                    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));

                                                    action = ItemUsuageID.RemoveVendingItem;
                                                    npc.OwnerVendor.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));


                                                    npc.OwnerVendor.Inventory.Update(VItem.DataItem, Role.Instance.AddMode.REMOVE, stream, true);


                                                    var sellit = Database.Server.ItemsBase[VItem.DataItem.ITEM_ID];
#if Arabic
                                                           string Messaj = "" + npc.OwnerVendor.Player.Name + " just sold " + sellit.Name + " to " + client.Player.Name + " for " + VItem.AmountCost + (VItem.CostType == MsgItemView.ActionMode.CPs ? " ConquerPoints." : " Gold.");
                                              
#else
                                                    string Messaj = "" + npc.OwnerVendor.Player.Name + " just sold " + sellit.Name + " to " + client.Player.Name + " for " + VItem.AmountCost + (VItem.CostType == MsgItemView.ActionMode.CPs ? " ConquerPoints." : " Gold.");

#endif
                                                    client.SendSysMesage(Messaj, MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red, true);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.ShowVendingList:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        // The whole booth list is resent on every request: ignore a repeat request for the
                        // same booth within [Performance] VendingListThrottleMs (0 = off, default).
                        int vendWindow = Conquer6609.Framework.Settings.Int("Performance", "VendingListThrottleMs", 0);
                        if (vendWindow > 0)
                        {
                            if (client.VendingListUid != id)
                            {
                                client.VendingListUid = id;
                                client.VendingListStamp = 0;
                            }
                            if (Client.GameClient.RequestTooSoon(ref client.VendingListStamp, vendWindow))
                                break;
                        }
                        Role.IMapObj Obj;
                        if (client.Player.View.TryGetValue(id, out Obj, Role.MapObjectType.SobNpc))
                        {
                            Role.SobNpc npc = Obj as Role.SobNpc;
                            if (npc.Booth != null)
                            {
                                Game.MsgServer.MsgItemExtra itemExtra = new Game.MsgServer.MsgItemExtra();
                                foreach (Game.BoothItem item in npc.Booth.ItemList.Values)
                                {
                                    client.Send(stream.ItemViewCreate(id, item.Cost, item.Item, item.Cost_Type));
                                    if (item.Item.Refinary.InLife)
                                    {
                                        item.Item.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.RefinaryAdding;
                                        if (item.Item.Refinary.EffectDuration == 0)
                                            item.Item.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.PermanentRefinery;
                                        itemExtra.Refinerys.Add(item.Item.Refinary);
                                    }
                                    if (item.Item.Purification.InLife)
                                    {
                                        item.Item.Purification.Typ = Game.MsgServer.MsgItemExtra.Typing.PurificationAdding;
                                        itemExtra.Purifications.Add(item.Item.Purification);
                                    }
                                }
                                if (itemExtra.Refinerys.Count != 0 || itemExtra.Purifications.Count != 0)
                                    client.Send(itemExtra.CreateArray(stream));
                            }
                            else if (npc.OwnerVendor.IsVendor)
                            {
                                Game.MsgServer.MsgItemExtra itemExtra = new Game.MsgServer.MsgItemExtra();
                                foreach (var item in npc.OwnerVendor.MyVendor.Items.Values)
                                {
                                    client.Send(stream.ItemViewCreate(npc.UID, item.AmountCost, item.DataItem, item.CostType));

                                    if (item.DataItem.Refinary.InLife)
                                    {
                                        item.DataItem.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.RefinaryAdding;
                                        if (item.DataItem.Refinary.EffectDuration == 0)
                                            item.DataItem.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.PermanentRefinery;
                                        itemExtra.Refinerys.Add(item.DataItem.Refinary);
                                    }
                                    if (item.DataItem.Purification.InLife)
                                    {
                                        item.DataItem.Purification.Typ = Game.MsgServer.MsgItemExtra.Typing.PurificationAdding;
                                        itemExtra.Purifications.Add(item.DataItem.Purification);
                                    }
                                }
                                if (itemExtra.Refinerys.Count != 0 || itemExtra.Purifications.Count != 0)
                                    client.Send(itemExtra.CreateArray(stream));
                            }
                        }
                        break;
                    }
                case ItemUsuageID.AddVendingItemConquerPts:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (client.IsVendor)
                        {
                            if (!client.Player.IsCheckedPass)
                                return;
                            MsgGameItem item;
                            if (client.Inventory.TryGetItem(id, out item))
                            {
                                //Database.ItemType.DBItem DBItem = null;
                                //if (Database.Server.ItemsBase.TryGetValue(item.ITEM_ID, out DBItem))
                                //{
                                //    if (DBItem.Type != Database.ItemType.DBItem.ItemType.Dropable)
                                //        return;
                                //}
                                if (Database.ItemType.unabletradeitem.Contains(item.ITEM_ID))
                                {
                                    break;
                                }
                                if (client.MyVendor.AddItem(item, MsgItemView.ActionMode.CPs, dwParam))
                                {
                                    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4, args));
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.AddVendingItemGold:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (client.IsVendor)
                        {
                            if (!client.Player.IsCheckedPass)
                                return;
                            MsgGameItem item;
                            if (client.Inventory.TryGetItem(id, out item))
                            {
                                if (Database.ItemType.unabletradeitem.Contains(item.ITEM_ID))
                                    break;
                                //Database.ItemType.DBItem DBItem = null;
                                //if (Database.Server.ItemsBase.TryGetValue(item.ITEM_ID, out DBItem))
                                //{
                                //    if (DBItem.Type != Database.ItemType.DBItem.ItemType.Dropable)
                                //        return;
                                //}
                                if (client.MyVendor.AddItem(item, MsgItemView.ActionMode.Gold, dwParam))
                                {
                                    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4, args));
                                }
                            }
                        }
                        break;
                    }
              
                case ItemUsuageID.DropItem:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (client.InTrade || client.IsVendor || !client.Player.IsCheckedPass)
                            return;


                        MsgGameItem item = null;
                        //Database.ItemType.DBItem DBItem = null;
                        if (client.Inventory.TryGetItem(id, out item))
                        {

                            if (item.Locked > 0 || item.Inscribed > 0)
                                break;
                            if (Database.ItemType.unabletradeitem.Contains(item.ITEM_ID) || item.Bound >= 1)
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
                                break;
                            }
                            //if (Database.Server.ItemsBase.TryGetValue(item.ITEM_ID, out DBItem))
                            //{
                            //    if (DBItem.Type != Database.ItemType.DBItem.ItemType.Dropable)
                            //        break;
                            //}
                            ushort x = client.Player.X;
                            ushort y = client.Player.Y;
                            if (client.Map.AddGroundItem(ref x, ref y))
                            {

                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, x, y, MsgFloorItem.MsgItem.ItemType.Item, 0, client.Player.DynamicID, client.Player.Map
                                    , client.Player.UID, false, client.Map);

                                if (client.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.DropGold:
                    {
                        // "Drop Silver" from the inventory: the amount is put on the floor as a money item
                        // anyone can pick up (the client asks for at least 100 silver).
                        if (client.IsConnectedInterServer())
                            break;
                        if (client.InTrade || client.IsVendor || !client.Player.IsCheckedPass)
                            return;
                        ulong amount = id != 0 ? id : dwParam;
                        if (amount < 100 || amount > uint.MaxValue || client.Player.Money < (long)amount)
                            break;
                        ushort x = client.Player.X;
                        ushort y = client.Player.Y;
                        if (!client.Map.AddGroundItem(ref x, ref y))
                            break;
                        MsgGameItem DataItem = new MsgGameItem();
                        DataItem.ITEM_ID = Database.ItemType.MoneyItemID((uint)amount);
                        MsgFloorItem.MsgItem DropMoney = new MsgFloorItem.MsgItem(DataItem, x, y, MsgFloorItem.MsgItem.ItemType.Money, (uint)amount
                            , client.Player.DynamicID, client.Player.Map, client.Player.UID, false, client.Map);
                        if (client.Map.EnqueueItem(DropMoney))
                        {
                            client.Player.Money -= (long)amount;
                            DropMoney.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                        }
                        break;
                    }
                case ItemUsuageID.VIPRepairItem:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        foreach (var item in client.Equipment.ClientItems.Values)
                        {
                            if (!Database.ItemType.Equipable(item, client))
                                continue;
                            if (item.Suspicious > 0)
                                continue;
                            //if (client.TryGetItem(id, out item))
                            {
                                if (item.Durability > 0 && item.Durability < item.MaximDurability)
                                {
                                    uint Price = Database.Server.ItemsBase[item.ITEM_ID].GoldWorth;
                                    byte Quality = (byte)(item.ITEM_ID % 10);
                                    double QualityMultipier = 0;

                                    switch (Quality)
                                    {
                                        case 9: QualityMultipier = 1.125; break;
                                        case 8: QualityMultipier = 0.975; break;
                                        case 7: QualityMultipier = 0.9; break;
                                        case 6: QualityMultipier = 0.825; break;
                                        default: QualityMultipier = 0.75; break;
                                    }

                                    int nRepairCost = 0;
                                    if (Price > 0)
                                        nRepairCost = (int)Math.Ceiling((Price * (item.MaximDurability - item.Durability) / item.MaximDurability) * QualityMultipier);

                                    nRepairCost = Math.Max(1, nRepairCost);
                                    if (client.Player.Money >= nRepairCost)
                                    {
                                        client.Player.Money -= nRepairCost;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                        item.Durability = item.MaximDurability;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                    }
                                }
                                /*else if (item.Durability == 0)
                                {
                                    if (client.Inventory.Remove(1088001, 5, stream))
                                    {
                                        item.Durability = item.MaximDurability;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                    }
                                }*/
                            }
                        }
                        break;
                    }
                case ItemUsuageID.RepairItem:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                      
                        MsgGameItem item = null;
                        if (client.TryGetItem(id, out item))
                        {
                            if (item.Durability > 0 && item.Durability < item.MaximDurability)
                            {
                                uint Price = Database.Server.ItemsBase[item.ITEM_ID].GoldWorth;
                                byte Quality = (byte)(item.ITEM_ID % 10);
                                double QualityMultipier = 0;

                                switch (Quality)
                                {
                                    case 9: QualityMultipier = 1.125; break;
                                    case 8: QualityMultipier = 0.975; break;
                                    case 7: QualityMultipier = 0.9; break;
                                    case 6: QualityMultipier = 0.825; break;
                                    default: QualityMultipier = 0.75; break;
                                }

                                int nRepairCost = 0;
                                if (Price > 0)
                                    nRepairCost = (int)Math.Ceiling((Price * (item.MaximDurability - item.Durability) / item.MaximDurability) * QualityMultipier);

                                nRepairCost = Math.Max(1, nRepairCost);
                                if (client.Player.Money >= nRepairCost)
                                {
                                    client.Player.Money -= nRepairCost;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                    item.Durability = item.MaximDurability;
                                    item.Mode = Role.Flags.ItemMode.Update;
                                    item.Send(client, stream);
                                }
                            }
                            else if (item.Durability == 0)
                            {
                                if (client.Inventory.Remove(1088001, 5, stream))
                                {
                                    item.Durability = item.MaximDurability;
                                    item.Mode = Role.Flags.ItemMode.Update;
                                    item.Send(client, stream);
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.ShowWarehouseMoney:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                   
                        dwParam = (ulong)client.Player.WHMoney;

                        client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        break;
                    }
                case ItemUsuageID.DepositWarehouse:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        // Amount is validated on the full 64-bit value; warehouse balance is capped.
                        long Amount;
                        if (!Conquer6609.Security.Guard.ValidAmount(dwParam, Conquer6609.Security.Guard.MaxMoneyAmount, out Amount))
                        {
                            Conquer6609.Security.Guard.Audit("warehouse-deposit", client.Player.UID, client.Player.Name, "rejected amount " + dwParam);
                            break;
                        }
                        // Deposits fill the warehouse up to MaxWarehouseMoney (the Player.Money overflow may push the
                        // balance past it, in which case nothing more can be deposited); the player is always told.
                        long room = client.Player.WHMoney < 0 ? 0 : Conquer6609.Security.Guard.MaxWarehouseMoney - client.Player.WHMoney;
                        if (room <= 0)
                        {
                            client.SendSysMesage("Your warehouse is full, it can't hold more than " + Conquer6609.Security.Guard.MaxWarehouseMoney + " gold.");
                            break;
                        }
                        if (Amount > room)
                        {
                            Amount = room;
                            client.SendSysMesage("Your warehouse can hold at most " + Conquer6609.Security.Guard.MaxWarehouseMoney + " gold, only " + Amount + " gold was deposited.");
                        }
                        if (client.Player.Money >= Amount)
                        {

                            client.Player.WHMoney += Amount;
                            client.Player.Money -= Amount;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendUpdate(stream, client.Player.WHMoney, MsgUpdate.DataType.WHMoney);
                            dwParam = (ulong)Amount;
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        }
                        break;
                    }
                case ItemUsuageID.WarehouseWithdraw:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                        if (client.Player.WHMoney <= 0)
                        {
                            break;
                        }
                        // Amount is validated on the full 64-bit value and must be covered by the warehouse balance.
                        long Amount;
                        if (!Conquer6609.Security.Guard.ValidAmount(dwParam, Conquer6609.Security.Guard.MaxMoneyAmount, out Amount))
                        {
                            Conquer6609.Security.Guard.Audit("warehouse-withdraw", client.Player.UID, client.Player.Name, "rejected amount " + dwParam);
                            break;
                        }
                        if (client.Player.WHMoney >= Amount)
                        {
                            client.Player.WHMoney -= Amount;
                            client.Player.Money += Amount;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendUpdate(stream, client.Player.WHMoney, MsgUpdate.DataType.WHMoney);
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        }
                        break;
                    }
                case ItemUsuageID.SocketTalismanWithCPs:
                    {
                       
                        if (client.IsConnectedInterServer())
                            break;
                        MsgGameItem Talisman = null;
                        if (client.TryGetItem(id, out Talisman))
                        {
                            uint price = 0;
                            if (Talisman.SocketOne == Role.Flags.Gem.NoSocket)
                            {
                                double procent = 100 - (Talisman.SocketProgress * 25600 / 2048000);
                                if (100 - procent < 25)
                                    return;
                                price = (uint)(procent * 55);
                            }
                            else if (Talisman.SocketTwo == Role.Flags.Gem.NoSocket)
                            {
                                double procent = 100 - (Talisman.SocketProgress * 25600 / 5120000);
                                if (100 - procent < 25)
                                    return;
                                price = (uint)(procent * 110);
                            }
                            else
                                return;
                            if (client.Player.ConquerPoints >= price)
                            {
                                client.Player.ConquerPoints -= price;

                                if (Talisman.SocketOne == Role.Flags.Gem.NoSocket)
                                    Talisman.SocketOne = Role.Flags.Gem.EmptySocket;
                                else if (Talisman.SocketTwo == Role.Flags.Gem.NoSocket)
                                    Talisman.SocketTwo = Role.Flags.Gem.EmptySocket;
                                Talisman.SocketProgress = 0;
                                Talisman.Mode = Role.Flags.ItemMode.Update;
                                Talisman.Send(client, stream);
                            }
                        }
                        break;
                    }
                case ItemUsuageID.SocketTalismanWithItem:
                    {
                        if (client.IsConnectedInterServer())
                            break;
                     
                        MsgGameItem Talisman = null;
                        if (client.TryGetItem(id, out Talisman))
                        {
                            if (!Database.ItemType.IsTalisman(Talisman.ITEM_ID))
                                return;
                            foreach (var itemUID in args)
                            {
                                MsgGameItem Item = null;
                                if (client.Inventory.TryGetItem(itemUID, out Item))
                                {
                                    if (Item.ITEM_ID / 1000 == Talisman.ITEM_ID / 1000  || Talisman.SocketTwo != Role.Flags.Gem.NoSocket)
                                        return;
                                    uint Points = 0;
                                    switch (Item.ITEM_ID % 10)
                                    {
                                        case 6: Points += 5; break;
                                        case 7: Points += 10; break;
                                        case 8: Points += 40; break;
                                        case 9: Points += 1000; break;
                                    }
                                    Points += Database.ItemType.TalismanExtra[Math.Min(Item.Plus, (byte)12)];

                                    int position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                                    switch (position)
                                    {
                                        case 0: return;
                                        case 4:
                                        case 5:
                                            if (Item.ITEM_ID % 10 >= 8)
                                            {
                                                if (Item.SocketOne != Role.Flags.Gem.NoSocket)
                                                    Points += 160;
                                                if (Item.SocketTwo != Role.Flags.Gem.NoSocket)
                                                    Points += 800;
                                            }
                                            break;
                                        default:
                                            if (Item.ITEM_ID % 10 >= 8)
                                            {
                                                if (Item.SocketOne != Role.Flags.Gem.NoSocket)
                                                    Points += 2000;
                                                if (Item.SocketTwo != Role.Flags.Gem.NoSocket)
                                                    Points += 6000;
                                            }
                                            break;
                                    }
                                    Talisman.SocketProgress += Points;
                                    if (Talisman.SocketOne == Role.Flags.Gem.NoSocket)
                                    {
                                        if (Talisman.SocketProgress >= 8000)
                                        {
                                            Talisman.SocketProgress -= 8000;
                                            Talisman.SocketOne = Role.Flags.Gem.EmptySocket;
                                        }
                                    }
                                    if (Talisman.SocketOne != Role.Flags.Gem.NoSocket)
                                    {
                                        if (Talisman.SocketProgress >= 20000)
                                        {
                                            Talisman.SocketProgress = 0;
                                            Talisman.SocketTwo = Role.Flags.Gem.EmptySocket;
                                        }
                                    }
                                    Talisman.Mode = Role.Flags.ItemMode.Update;
                                    Talisman.Send(client, stream).Update(Item, Role.Instance.AddMode.REMOVE, stream);
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.SellItem:
                    {
                      
                        if (client.IsConnectedInterServer())
                            break;
                        if (!client.Player.IsCheckedPass)
                            return;
                        uint ShopUID = id;
                        uint ItemUID = (uint)dwParam;
                        Game.MsgNpc.Npc obj;
                        if (client.Map.SearchNpcInScreen(ShopUID, client.Player.X, client.Player.Y, out obj))
                        {
                            Game.MsgServer.MsgGameItem Item;
                            if (client.Inventory.TryGetItem(ItemUID, out Item))
                            {
                                if (Item.Locked > 0 || Item.Inscribed > 0)
                                {
#if Arabic
                                      client.SendSysMesage("This item is Inscribed in your guild, or this items is locked.");
#else
                                    client.SendSysMesage("This item is Inscribed in your guild, or this items is locked.");
#endif

                                    break;
                                }
                                Database.ItemType.DBItem DBItem = null;
                                if (Database.Server.ItemsBase.TryGetValue(Item.ITEM_ID, out DBItem))
                                {
                                    long prince = DBItem.GoldWorth / 3;
                                    if (Item.Durability > 0 && Item.Durability < Item.MaximDurability)
                                        prince = (prince * Item.Durability) / Item.MaximDurability;

                                    if (Item.Durability > 0 && Item.Durability <= Item.MaximDurability)
                                    {
                                        client.Inventory.Update(Item, Role.Instance.AddMode.REMOVE, stream);
                                        client.Player.Money += prince;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    }
                                    else
                                    {
                                        client.Inventory.Update(Item, Role.Instance.AddMode.REMOVE, stream);
                                    }
                                }
                            }

                        }
                        break;
                    }
                case ItemUsuageID.BuyItem:
                    {
                      
                        if (client.IsConnectedInterServer())
                            break;
                        uint ShopUID = id;
                        if (dwParam > uint.MaxValue)
                            break;
                        uint ItemID = (uint)dwParam;
                        // Purchase count is validated on the full value (0 means 1); space is checked per shop below.
                        if (dwParam2 > byte.MaxValue)
                            break;
                        byte Counts = (byte)(dwParam2 == 0 ? 1 : dwParam2);
                        if (ShopUID == 0)
                            return;
                        Game.MsgNpc.Npc obj;

                        if (ShopUID == Database.Shops.ChampionShop.UID)
                        {
                            var shop = Database.Shops.ChampionShop.Shop;

                            Database.Shops.ChampionShop.ChampionItem item;
                            if (shop.Items.TryGetValue(ItemID, out item))
                            {
                                if (ItemID == 3005360)
                                    return;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(item.ID, out DBItem))
                                {
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        if (client.Player.ChampionPoints > item.Price)
                                        {
                                            client.Player.ChampionPoints -= item.Price;
                                            byte plus = 0;
                                            if (DBItem.ID >= 730001 && DBItem.ID <= 730008)
                                                plus = (byte)(DBItem.ID % 10);
                                            client.Inventory.Add(stream, ItemID, 1, plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                            client.Send(stream.CreateGoldLeaguePoint(new MsgGoldLeaguePoint.GoldLeaguePoint() { Points = client.Player.ChampionPoints, HistoryPoints = 0 }));
                                            client.SendSysMesage("You bought 1 " + DBItem.Name + "!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                        }
                                        else
                                        {
                                            client.SendSysMesage("You do not have enough Champion Points.", MsgMessage.ChatMode.System);
                                        }
                                    }
                                    else
                                    {
                                        client.SendSysMesage("You do not have enough space in your inventory.", MsgMessage.ChatMode.System);
                                    }
                                }
                            }
                        }
                        else if (ShopUID == Database.Shops.HonorShop.UID)
                        {
                            var shop = Database.Shops.HonorShop.Shop;
                            uint cost;
                            if (shop.Items.TryGetValue(ItemID, out cost))
                            {
                                long totalCost;
                                if (!Conquer6609.Security.Guard.SafeMultiply(cost, Counts, out totalCost) || totalCost > uint.MaxValue)
                                    break;
                                if (client.Inventory.HaveSpace(Counts))
                                {
                                    if (client.HonorPoints >= totalCost)
                                    {
                                        // Honor points are charged only after the items were added.
                                        if (client.Inventory.Add(stream, ItemID, Counts))
                                        {
                                            client.HonorPoints -= (uint)totalCost;
                                            client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
                                        }
                                    }
                                    else
                                    {
                                        client.SendSysMesage("You do not have enough Honor Points");
                                    }
                                }
                                else
                                {
                                    client.SendSysMesage("You do not have enough space in your inventory");
                                }
                            }
                        }
                        else if (ShopUID == Database.Shops.RacePointShop.UID)
                        {
                            Database.Shops.RacePointShop.RacePointItem item;
                            if (Database.Shops.RacePointShop.Shop.Items.TryGetValue(ItemID, out item))
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    if (client.Player.RacePoints >= item.Price)
                                    {
                                        client.Player.RacePoints -= item.Price;
                                        client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
                                        client.Inventory.Add(stream, item.ID, 1);
                                    }
                                    else
                                    {
#if Arabic
                                         client.SendSysMesage("You do not have enough Race Points");
#else
                                        client.SendSysMesage("You do not have enough Race Points");
#endif

                                    }
                                }
                                else
                                {
#if Arabic
                                      client.SendSysMesage("You do not have enough space in your inventory");
#else
                                    client.SendSysMesage("You do not have enough space in your inventory");
#endif

                                }
                            }
                        }
                        else if (client.Map.SearchNpcInScreen(ShopUID, client.Player.X, client.Player.Y, out obj) || ShopUID == 2888)
                        {
                            Database.Shops.ShopFile.Shop shop = new Database.Shops.ShopFile.Shop();
                            if (!Database.Shops.ShopFile.Shops.TryGetValue(ShopUID, out shop))
                                if (!Database.Shops.EShopFile.Shops.TryGetValue(ShopUID, out shop))
                                    shop = null;
                            if (shop != null && shop.UID != 0)
                            {
                                if (dwparam4 == 2)
                                {
                                    if (!shop.BoundItems.Contains(ItemID))
                                        return;
                                    Database.ItemType.DBItem DBItem = null;
                                    if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                                    {
                                        byte Amount = Counts;

                                        while (Amount > 0)
                                        {
                                            switch (shop.MoneyType)
                                            {
                                                case Database.Shops.ShopFile.MoneyType.ConquerPoints:
                                                    {
                                                        if (DBItem.ConquerPointsWorth >= 1)
                                                        {
                                                            if (client.Player.BoundConquerPoints >= DBItem.ConquerPointsWorth)
                                                            {
                                                                byte plus = 0;
                                                                if (DBItem.ID >= 730001 && DBItem.ID <= 730008)
                                                                    plus = (byte)(DBItem.ID % 10);
                                                                // Charged only once the item was actually added.
                                                                if (client.Inventory.Add(DBItem.ID, plus, DBItem, stream, true))
                                                                    client.Player.BoundConquerPoints -= (int)DBItem.ConquerPointsWorth;
                                                                else
                                                                    Amount = 1;

                                                            }
                                                        }
                                                        break;
                                                    }
                                            }
                                            Amount--;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!shop.Items.Contains(ItemID))
                                        return;
                                    Database.ItemType.DBItem DBItem = null;
                                    if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                                    {
                                        byte Amount = Counts;

                                        while (Amount > 0)
                                        {
                                            switch (shop.MoneyType)
                                            {
                                                case Database.Shops.ShopFile.MoneyType.ConquerPoints:
                                                    {
                                                        if (DBItem.ConquerPointsWorth >= 1)
                                                        {
                                                            if (client.Player.ConquerPoints >= DBItem.ConquerPointsWorth)
                                                            {
                                                                byte plus = 0;
                                                                if (DBItem.ID >= 730001 && DBItem.ID <= 730008)
                                                                    plus = (byte)(DBItem.ID % 10);
                                                                // Charged only once the item was actually added.
                                                                if (client.Inventory.Add(DBItem.ID, plus, DBItem, stream))
                                                                {
                                                                    client.Player.ConquerPoints -= DBItem.ConquerPointsWorth;
                                                                    // Welfare Events tab: "CPs spent in the CP Mall"
                                                                    Conquer6609.Modules.OperatingActs.OperatingActs.AddPoints(client, Conquer6609.Modules.OperatingActs.OperatingActs.CpsSpentMall, DBItem.ConquerPointsWorth);
                                                                }
                                                                else
                                                                    Amount = 1;

                                                            }
                                                        }
                                                        break;
                                                    }
                                                case Database.Shops.ShopFile.MoneyType.Gold:
                                                    {
                                                        if (DBItem.GoldWorth >= 1)
                                                        {
                                                            if (client.Player.Money >= DBItem.GoldWorth)
                                                            {
                                                                // Charged only once the item was actually added.
                                                                if (!client.Inventory.Add(DBItem.ID, 0, DBItem, stream))
                                                                {
                                                                    Amount = 1;
                                                                    break;
                                                                }
                                                                if (client.Player.QuestGUI.CheckQuest(1740, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                                {
                                                                    if (DBItem.ID == 1000000)
                                                                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1740, 1);
                                                                    else if (DBItem.ID == 1001000)
                                                                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1740, 0, 1);
                                                                }
                                                                client.Player.Money -= DBItem.GoldWorth;
                                                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                                            }
                                                        }
                                                        break;
                                                    }
                                            }
                                            Amount--;
                                        }
                                    }
                                }

                            }
                        }
                        break;
                    }
                case ItemUsuageID.Ping:
                    {
                       
                        client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                    
                         #if ItemTime

                        //foreach (var item in client.AllMyTimeItems())
                        //{
                        //    if (item.EndDate <= DateTime.Now)
                        //    {
                        //        Database.ItemType.DBItem DbItem;
                        //        if (Database.Server.ItemsBase.TryGetValue(item.ITEM_ID, out DbItem))
                        //        {
                        //            if (client.Inventory.ClientItems.ContainsKey(item.UID))
                        //            {
                        //                client.Inventory.Remove(item.UID, 1, stream);
                        //                client.SendSysMesage(DbItem.Name + " Time Expired.");
                        //            }
                        //            else if (client.Equipment.Contains(item.UID))
                        //            {
                        //                client.Equipment.Delete((Role.Flags.ConquerItem)item.Position, stream);
                        //                client.SendSysMesage(DbItem.Name + " Time Expired.");
                        //            }
                        //            else if (client.MyWardrobe.Contain(item.UID))
                        //            {
                        //                MsgGameItem Delete;
                        //                if (client.MyWardrobe.RemoveItem(item.UID, out Delete))
                        //                {
                        //                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        //                    Game.MsgServer.MsgCoatStorage.CoatStorage astore = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                        //                    astore.ActionID = MsgCoatStorage.Action.Expire;
                        //                    astore.dwparam1 = item.UID;
                        //                    astore.dwpram2 = item.ITEM_ID;
                        //                    client.Send(stream.CreateCoatStorage(astore));
                        //                    client.Send(stream.CreateCoatStorage(astore));
                        //                    client.SendSysMesage(DbItem.Name + " Time Expired.");
                        //                }
                        //            }
                        //            else
                        //            {
                        //                //foreach (var WH in client.Warehouse.ClientItems.Values)
                        //                //{
                        //                //    MsgGameItem Delete;
                        //                //    if (WH.TryRemove(item.UID, out Delete))
                        //                //    {
                        //                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        //                //        client.SendSysMesage(DbItem.Name + " Time Expired.");
                        //                //        Delete.Send(client, stream);
                        //                //        //stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                        //                //        //stream.AddItemWarehouse(item);

                        //                //        //client.Send(stream.FinalizeWarehouse());
                        //                //    }
                        //                //}

                        //                //foreach (var Wh in client.Warehouse.ClientItems)
                        //                //{
                        //                //    foreach (var item2 in Wh.Value.Values)
                        //                //    {
                        //                //        if (item2.UID == item.UID)
                        //                //        {
                        //                //            client.Warehouse.RemoveItem(item2.UID, Wh.Key, stream);
                        //                //            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        //                //        }
                        //                //    }
                        //                //}
                        //            }
                        //        }
                        //    }
                        //}
#endif
                        break;
                    

                    }
                case ItemUsuageID.Unequip:
                    {
                        if (client.Player.ContainFlag(MsgUpdate.Flags.PathOfShadow))
                        {
#if Arabic
                                client.SendSysMesage("Please remove the PathOfShadow.");
#else
                            client.SendSysMesage("Please remove the PathOfShadow.");
#endif

                            break;
                        }
                        uint Position = (uint)dwParam;

                        if (Position == (ushort)Role.Flags.ConquerItem.Garment)
                        {
                            if (client.Player.SpecialGarment != 0)
                            {
#if Arabic
                                client.SendSysMesage("Item can't be unequiped during the event.");
#else
                                client.SendSysMesage("Item can't be unequiped during the event.");
#endif

                                return;
                            }
                        }
                        if (Position == (ushort)Role.Flags.ConquerItem.Garment || Position == (ushort)Role.Flags.ConquerItem.SteedMount)
                            return;
                        if (client.Equipment.Remove((Role.Flags.ConquerItem)Position, stream))
                        {

                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }
                        break;
                    }
                case ItemUsuageID.Equip:
                    {

                        if (dwParam < 20)
                            EquipItem(client, stream, id, (uint)dwParam, timestamp, dwParam2, dwParam3, dwparam4);
                        else
                            AlternanteEquipItem(client, stream, id, (uint)dwParam, timestamp, dwParam2, dwParam3, dwparam4);
                        break;
                    }
                case ItemUsuageID.Alternante:
                case ItemUsuageID.UnAlternante:
                    {
                        if (client.Player.ContainFlag(MsgUpdate.Flags.Fly))
                            break;
                        Extensions.Time32 clock = Extensions.Time32.Now;
                        if (clock < client.Player.Changetamp.AddSeconds(5))
                        {
                            return;
                        }
                        client.Player.Changetamp = Extensions.Time32.Now;
                        if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.OnAutoAttack = false;
                        }
                        if (client.Player.ContainFlag(MsgUpdate.Flags.PathOfShadow))
                        {

#if Arabic
                                 client.SendSysMesage("Please remove the PathOfShadow.");
#else
                            client.SendSysMesage("Please remove the PathOfShadow.");
#endif

                            break;
                        }
                        if (client.Player.ContainFlag(MsgUpdate.Flags.Fly))
                            break;

                        if (client.Player.SpecialGarment != 0)
                        {
#if Arabic
                            
                            client.SendSysMesage("Item can't be unequiped during the event.");
#else

                            client.SendSysMesage("Item can't be unequiped during the event.");
#endif
                            return;
                        }

                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Equipment.Alternante = !client.Equipment.Alternante;
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            client.Player.View.ReSendView(stream);
                        }
                        else
                        {
#if Arabic
                              client.SendSysMesage("Your Inventory Is Full!");
#else
                            client.SendSysMesage("Your Inventory Is Full!");
#endif

                        }
                        break;
                    }
                default:
                    {
                        Conquer6609.Modules.PacketAudit.PacketAuditModule.UnhandledSub(client, "1009", (uint)action, stream);
#if TEST
                        MyConsole.WriteLine(" [" + client.Player.Name + "] MsgItemUsuagePacket  not find -> " + id);
#endif
                        break;
                    }
            }
        }
    }
}
