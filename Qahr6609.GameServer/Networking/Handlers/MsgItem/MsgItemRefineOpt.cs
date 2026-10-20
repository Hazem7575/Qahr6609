using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgItemRefineOpt
    {
        [ProtoContract]
        public class ItemRefineOpt
        {
            [ProtoMember(1, IsRequired = true)]
            public ActionID Type;
            [ProtoMember(2, IsRequired = true)]
            public uint ItemUID;
            [ProtoMember(3)]
            public string Signature;
            [ProtoMember(4, IsRequired = true)]
            public uint[] Items;
        }

        [Flags]
        public enum ActionID
        {
            Perfection = 0,
            Ownership = 1,
            Signature = 2,
            CPBoost = 3,
            Exchange = 4,
            Quick = 5
        }

        [PacketAttribute(GamePackets.MsgItemRefineOpt)]
        public static unsafe void Handler(Client.GameClient client, ServerSockets.Packet stream)
        {
            if ((client.InTrade || client.IsVendor))
                return;

            ItemRefineOpt msg = new ItemRefineOpt();
            msg = stream.ProtoBufferDeserialize<ItemRefineOpt>(msg);

            switch (msg.Type)
            {
                case ActionID.Quick:
                    {
                        MsgGameItem Item;
                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);

                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                                   || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                                   || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory || position == (ushort)Role.Flags.ConquerItem.Inventory
                                   || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }
                            if (Item.PerfectionLevel >= 54) return;
                            var persent = Database.ItemRefineUpgrade.ProgresUpdates[Item.PerfectionLevel + 1];
                            var CurrentProgres = Item.PerfectionProgress;
                            var NextLevel = persent - Item.PerfectionProgress;
                            var sucsesRate = (CurrentProgres / (double)persent) * 100;
                            sucsesRate = Math.Round(sucsesRate, 2);

                            if (Item.PerfectionProgress > 0)
                            {
                                if (Role.Core.Rate(sucsesRate))
                                {
                                    Item.PerfectionLevel++;
                                }
                                Item.PerfectionProgress = 0;
                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, stream);
                                client.UpdatePerfectionLevel(stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                            }
                        }
                        break;
                    }
                #region CP Boost
                case ActionID.CPBoost:
                    {
                        MsgGameItem Item;
                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                                   || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                                   || position == (ushort)Role.Flags.ConquerItem.Inventory
                                   || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                   || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }

                            uint oldrank = Database.RankItems.RankPoll[(uint)Item.GetPerfectionPosition].GetItemRank(Item.UID);

                            while (client.Equipment.CanUpdatePerfectionItem(Item) && Item.PerfectionLevel < 54)
                            {
                                var currentProgress = Item.PerfectionProgress;
                                var required = Database.ItemRefineUpgrade.ProgresUpdates[Item.PerfectionLevel + 1];
                                var cost = (required - currentProgress) / 10 * 8;
                                if (client.Player.ConquerPoints >= cost)
                                {
                                    client.Player.ConquerPoints -= (uint)cost;
                                    Item.PerfectionProgress = 0;
                                    Item.PerfectionLevel++;

                                }
                                else
                                    break;
                            }
                            Item.OwnerName = client.Player.Name;
                            Item.OwnerUID = client.Player.UID;
                            Item.Mode = Role.Flags.ItemMode.Update;
                            Item.Send(client, stream);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                            uint rank = Database.RankItems.RankPoll[(uint)Item.GetPerfectionPosition].GetItemRank(Item.UID);
                            if (rank <= 50 && rank < oldrank)
                            {
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(Item.ITEM_ID, out DBItem))
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congrats! " + client.Player.Name + "`s " + DBItem.Name + " has climbed to No." + rank.ToString() + " place on the Perfection Ranking. [Link I want to get on the list###1 345]", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeftSystem).GetArray(stream));
                            }
                        }
                        break;
                    }
                #endregion
                case ActionID.Ownership:
                    {
                        MsgGameItem Item;
                        if (msg.Items == null)
                        {

                            if (client.TryGetItem(msg.ItemUID, out Item))
                            {
                                ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                                if (position == (ushort)Role.Flags.ConquerItem.Garment
                                       || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                                       || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory || position == (ushort)Role.Flags.ConquerItem.Inventory
                                       || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                                {
                                    Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                    client.Socket.Disconnect();
                                    return;
                                }

                                if (client.Player.ConquerPoints > 1500)
                                {
                                    client.Player.ConquerPoints -= 1500;
                                    Item.OwnerName = client.Player.Name;
                                    Item.OwnerUID = client.Player.UID;
                                    Item.Mode = Role.Flags.ItemMode.Update;
                                    Item.Send(client, stream);
                                    client.UpdatePerfectionLevel(stream);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                                }
                            }
                            break;
                        }
                        break;
                    }
                case ActionID.Perfection:
                    {
                        MsgGameItem Item;
                        if (msg.Items == null)
                        {
                            break;
                        }

                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                                   || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount || position == (ushort)Role.Flags.ConquerItem.Inventory
                                   || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                   || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }
                         
                            if (client.Equipment.CanUpdatePerfectionItem(Item))
                            {
                                uint oldrank = Database.RankItems.RankPoll[(uint)Item.GetPerfectionPosition].GetItemRank(Item.UID);

                                // Stones must be distinct inventory items (never equipment, never the target item);
                                // all are validated before any is consumed, and only removed stones add progress.
                                if (msg.Items.Length > 40)
                                    return;
                                List<MsgGameItem> stonesToUse = new List<MsgGameItem>();
                                HashSet<uint> seenStones = new HashSet<uint>();
                                foreach (var _stone in msg.Items)
                                {
                                    if (_stone == Item.UID || !seenStones.Add(_stone))
                                        return;
                                    MsgGameItem Stone;
                                    if (!client.Inventory.TryGetItem(_stone, out Stone) || Stone == null)
                                        return;
                                    if (Item.PerfectionLevel > 54) return;

                                    if (Item.ITEM_ID == 300000)
                                    { // الايتم حصان


                                        if ( Item.ITEM_ID % 1000 != 0) return;
                                    }
                                    else
                                    {// الاتيم مش حصان 

                                        if (Item.ITEM_ID % 1000 == 0) return;
                                        if (Stone.ITEM_ID >= 730001 && Stone.ITEM_ID <= 730008)
                                        {
                                            var type1 = Database.ItemType.ItemPosition(Stone.ITEM_ID);
                                            if (type1 != 0 && Stone.ITEM_ID / 10000 != 73) return;

                                        }
                                        else if (Stone.ITEM_ID >= 3009000 && Stone.ITEM_ID <= 3009003)
                                        {
                                            var type1 = Database.ItemType.ItemPosition(Stone.ITEM_ID);
                                            if (type1 != 0 && Stone.ITEM_ID / 10000 != 300) return;
                                        }
                                        else
                                        { // باقى الايتمات
                                            uint r1 = Item.ITEM_ID / 1000;
                                            uint r2 = Stone.ITEM_ID / 1000;
                                            if (r1 != r2 || Stone.ITEM_ID % 1000 == 0)
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    stonesToUse.Add(Stone);
                                }
                                /////////////////////////////////////////////
                                foreach (var Stone in stonesToUse)
                                {
                                    uint stoneId = Stone.ITEM_ID;
                                    byte stonePlus = Stone.Plus;
                                    if (!client.Inventory.RemoveOne(Stone.UID, stream))
                                        continue;
                                    if (stoneId == 3009001)
                                        Item.PerfectionProgress += 100;
                                    else if (stoneId == 3009002)
                                        Item.PerfectionProgress += 1000;
                                    else if (stoneId == 3009003)
                                        Item.PerfectionProgress += 10000;
                                    else if (stoneId == 3009000)
                                        Item.PerfectionProgress += 10;
                                    else
                                        Item.PerfectionProgress += Database.ItemType.StonePlusPoints(stonePlus);
                                }
                                while (Item.PerfectionLevel < Database.ItemRefineUpgrade.ProgresUpdates.Count && Database.ItemRefineUpgrade.ProgresUpdates.ContainsKey(Item.PerfectionLevel + 1) && Item.PerfectionProgress >= Database.ItemRefineUpgrade.ProgresUpdates[Item.PerfectionLevel + 1])
                                {
                                    Item.PerfectionProgress -= Database.ItemRefineUpgrade.ProgresUpdates[Item.PerfectionLevel + 1];
                                    Item.PerfectionLevel += 1;
                                    if (Item.PerfectionLevel == Database.ItemRefineUpgrade.ProgresUpdates.Count)
                                    {
                                        Item.PerfectionProgress = 0;
                                        break;
                                    }
                                }



                                Item.OwnerName = client.Player.Name;
                                Item.OwnerUID = client.Player.UID;
                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, stream);
                                client.UpdatePerfectionLevel(stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);

                                uint rank = Database.RankItems.RankPoll[(uint)Item.GetPerfectionPosition].GetItemRank(Item.UID);
                                if (rank <= 50 && rank < oldrank)
                                {
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(Item.ITEM_ID, out DBItem))
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congrats! " + client.Player.Name + "`s " + DBItem.Name + " has climbed to No." + rank.ToString() + " place on the Perfection Ranking. [Link I want to get on the list###1 345]", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeftSystem).GetArray(stream));
                                }
                            }
                        }
                        break;
                    }
                case ActionID.Exchange:
                    {
                        if (msg.Items == null)
                            return;
                        MsgGameItem Item;
                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                             || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                             || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory || position == (ushort)Role.Flags.ConquerItem.Inventory
                             || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }
                            if (msg.Items.Length == 1)
                            {
                                MsgGameItem ItemExchange;
                                if (client.TryGetItem(msg.Items[0], out ItemExchange))
                                {
                                    if (ItemExchange.IsEquip)
                                    {
                                        ushort ExchangePosition = Database.ItemType.ItemPosition(ItemExchange.ITEM_ID);
                                        if (position != ExchangePosition)
                                        {
                                            Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                            client.Socket.Disconnect();
                                            break;
                                        }
                                        if (Item.OwnerUID != 0)
                                        {
                                            if (Item.OwnerUID != client.Player.UID)
                                            {
                                                client.SendSysMesage("Can't Exchange another Owner item.");
                                                break;
                                            }
                                        }
                                        //Item.PerfectionLevel يمين
                                        // ItemExchange.PerfectionLevel شمال
                                        if (client.Player.ConquerPoints >= 1000)
                                        {
                                            uint Level = ItemExchange.PerfectionLevel;
                                            uint Progress = ItemExchange.PerfectionProgress;
                                            ItemExchange.PerfectionLevel = Item.PerfectionLevel;
                                            ItemExchange.PerfectionProgress = Item.PerfectionProgress;

                                            if (ItemExchange.PerfectionLevel > 0 || ItemExchange.PerfectionProgress > 0)
                                            {
                                                ItemExchange.OwnerUID = client.Player.UID;
                                                ItemExchange.OwnerName = client.Player.Name;
                                                Item.OwnerName = "";
                                                Item.OwnerUID = 0;
                                            }
                                            else
                                            {
                                                Item.OwnerUID = client.Player.UID;
                                                Item.OwnerName = client.Player.Name;

                                            }
                                            Item.PerfectionLevel = Level;
                                            Item.PerfectionProgress = Progress;
                                            Item.Mode = Role.Flags.ItemMode.Update;
                                            Item.Send(client, stream);
                                            ItemExchange.Mode = Role.Flags.ItemMode.Update;
                                            ItemExchange.Send(client, stream);

                                            client.Player.ConquerPoints -= 1000;
                                        }
                                        break;
                                    }
                                }
                            }
                            client.UpdatePerfectionLevel(stream);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                        }
                        break;
                    }
                case ActionID.Signature:
                    {
                        MsgGameItem Item;
                        if (client.TryGetItem(msg.ItemUID, out Item))
                        {

                            //to check for proxy.
                            ushort position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (position == (ushort)Role.Flags.ConquerItem.Garment
                                || position == (ushort)Role.Flags.ConquerItem.Bottle || position == (ushort)Role.Flags.ConquerItem.SteedMount
                                || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory || position == (ushort)Role.Flags.ConquerItem.Inventory
                                || position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                            {
                                Console.WriteLine("Client " + client.Player.Name + " cheater.");
                                client.Socket.Disconnect();
                                return;
                            }
                            // Validate the text first: an invalid signature must never cost CPs.
                            // Signatures are free text (not names): previous length limit (< 32) and the free text rules.
                            string signature = msg.Signature;
                            if (signature == null || signature.Length >= 32 || !Program.NameStrCheck(signature, false))
                            {
                                Conquer6609.Security.Guard.Audit("item-signature", client.Player.UID, client.Player.Name, "rejected signature for item " + msg.ItemUID);
                                break;
                            }
                            uint Cost = (uint)(string.IsNullOrEmpty(Item.Signature) ? 0 : 270);
                            if (client.Player.ConquerPoints > Cost)
                            {
                                if (Cost != 0)
                                    client.Player.ConquerPoints -= Cost;
                                Item.Signature = signature;
                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, stream);
                            }
                            client.UpdatePerfectionLevel(stream);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                        }

                        break;
                    }
                default:
                    {
                        Console.WriteLine(msg.Type);
                        break;
                    }
            }
        }
    }
}
