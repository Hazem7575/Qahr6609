using Conquer6609.Game.Conquer6609_Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        /// <summary>Upper bound on material UIDs accepted in one compose / upgrade request.</summary>
        public const int UpdateItemMaxMaterials = 64;

        public static unsafe void GetUpdateItem(this ServerSockets.Packet stream, out MsgUpdateItem.ActionType Action, out uint ItemUID, out List<uint> items)
        {
            Action =(MsgUpdateItem.ActionType)stream.ReadUInt8();
            byte FullCount = stream.ReadUInt8();
            ushort padding = stream.ReadUInt16();
            ItemUID = stream.ReadUInt32();
            uint Count = stream.ReadUInt32();

            items = new List<uint>();
            // Client supplied counts are clamped and reading stops at the end of the packet.
            if (Action != MsgUpdateItem.ActionType.UpdateLevel && Action != MsgUpdateItem.ActionType.UpdateQuality)
            {

                items.Add(Count);
                int extra = Conquer6609.Security.Guard.ClampCount((long)FullCount - 2, UpdateItemMaxMaterials - 1);
                for (int x = 0; x < extra; x++)
                {
                    if (stream.Position + 4 > stream.Size)
                        break;
                    items.Add(stream.ReadUInt32());
                }
            }
            else
            {
                int total = Conquer6609.Security.Guard.ClampCount(Count, UpdateItemMaxMaterials);
                for (int x = 0; x < total; x++)
                {
                    if (stream.Position + 4 > stream.Size)
                        break;
                    items.Add(stream.ReadUInt32());
                }
            }
        }
    }


    public struct MsgUpdateItem
    {
        public enum ActionType : byte
        {
            Plus = 0,
            CurrentSteed = 2,
            NewSteed = 3,
            ChanceUpgrade = 4,
            ChanceUpgrade2 = 5,
            UpdateLevel = 6,
            UpdateQuality = 7
        }
        [PacketAttribute(GamePackets.Compose)]
        public unsafe static void Compose(Client.GameClient client, ServerSockets.Packet stream)
        {

            MsgUpdateItem.ActionType Action; uint ItemUID;
            List<uint> ItemsUIDS;
            uint dwParam1 = 2;
            stream.GetUpdateItem(out Action, out ItemUID, out ItemsUIDS);

            if (ItemsUIDS.Count == 0)
                return;
            if (client.InTrade || client.IsVendor || !client.Socket.Alive)
                return;
            // The upgraded item can never be one of its own materials.
            if (ItemsUIDS.Contains(ItemUID))
                return;
            MsgGameItem DataItemm;
            if (client.TryGetItem(ItemUID, out DataItemm))
            {
                ushort Position = Database.ItemType.ItemPosition(DataItemm.ITEM_ID);
                if (Position <= 0 || client.Inventory.HasDuplicates(ItemsUIDS))
                {
                    Console.WriteLine("fuck player " + client.Player.Name + " try use Compose bug");
                    Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 99999, "Hack  Compose");
                    string log = ("[Hack]" + client.Player.Name + "He Try To Use hack Compose Bug ");
                    Logs.OnDequeue(log);
                    client.Socket.Disconnect();
                    return;
                }
            }
            switch (Action)
            {
                case ActionType.UpdateLevel:
                    {

                        MsgGameItem DataItem;
                        if (client.TryGetItem(ItemUID, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                            //anti proxy --------------------
                            if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                            {
                                client.SendSysMesage("This item's level cannot be upgraded anymore.");
                                return;
                            }

                            //------------------------
                            MsgGameItem itemuse;
                            if (client.Inventory.ClientItems.TryGetValue(ItemsUIDS[0], out itemuse))
                            {
                                if (itemuse.ITEM_ID == Database.ItemType.DragonBall)
                                {
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                                    {
                                        bool succesed = false;
                                        uint nextItemId = Database.Server.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);
                                        // The DragonBall is consumed before the upgrade is applied.
                                        if (succesed && nextItemId != 0 && nextItemId != DataItem.ITEM_ID && client.Inventory.RemoveOne(itemuse.UID, stream))
                                        {
                                            dwParam1 = 1;
                                            DataItem.ITEM_ID = nextItemId;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(client, stream);
                                        }
                                        else
                                        {
                                            client.SendSysMesage("This item's level cannot be upgraded anymore.");
                                        }
                                    }
                                }
                                else if (itemuse.ITEM_ID == Database.ItemType.Meteor || itemuse.ITEM_ID == Database.ItemType.MeteorScroll)
                                {
                                    if (client.Inventory.CheckMeteors((byte)ItemsUIDS.Count, false, stream))
                                    {
                                        Database.ItemType.DBItem DBItem;
                                        if (Database.Server.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                                        {
                                            bool succesed = false;

                                            uint nextItemId = Database.Server.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);

                                            {
                                                if (Database.ItemType.UpItemMeteors(DataItem.ITEM_ID, (uint)ItemsUIDS.Count))
                                                {
                                                    client.HeroRewards.AddGoal(205);

                                                    dwParam1 = 1;
                                                    DataItem.ITEM_ID = Database.Server.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream);

                                                }
                                                else
                                                {
                                                    if (DataItem.Durability > 200)
                                                    {
                                                        DataItem.Durability -= 200;
                                                        DataItem.Mode = Role.Flags.ItemMode.Update;
                                                        DataItem.Send(client, stream);
                                                    }
                                                }

                                                client.Inventory.CheckMeteors((byte)ItemsUIDS.Count, true, stream);
                                            }
                                        }
                                    }
                                }
                                if (DataItem.Position != 0)
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpgradeMeteor, ItemUID, dwParam1, 0, 0, 0, 0));

                        }
                        break;
                    }
                case ActionType.UpdateQuality:
                    {
                        MsgGameItem DataItem;
                        if (client.TryGetItem(ItemUID, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                            //anti proxy --------------------
                            if (Position != (ushort)Role.Flags.ConquerItem.Fan
                                && Position != (ushort)Role.Flags.ConquerItem.Tower && Position != (ushort)Role.Flags.ConquerItem.RidingCrop
                                && Position != (ushort)Role.Flags.ConquerItem.Wing)
                            {
                                if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                                {
                                    client.SendSysMesage("This item's Quality cannot be upgraded anymore.");


                                    return;
                                }
                            }
                            //------------------------
                            // Every material must be a distinct DragonBall in the inventory; all are checked
                            // before any is consumed, and only DragonBalls actually removed are counted.
                            List<MsgGameItem> UseItems = new List<MsgGameItem>();
                            HashSet<uint> seen = new HashSet<uint>();
                            bool EmbedUpdate = ItemsUIDS.Count > 0;
                            for (int x = 0; x < ItemsUIDS.Count; x++)
                            {
                                MsgGameItem itemuse;
                                if (seen.Add(ItemsUIDS[x]) && client.Inventory.ClientItems.TryGetValue(ItemsUIDS[x], out itemuse)
                                    && itemuse != null && itemuse.ITEM_ID == Database.ItemType.DragonBall)
                                {
                                    UseItems.Add(itemuse);
                                }
                                else { EmbedUpdate = false; break; }
                            }
                            uint nextQualityId = DataItem.ITEM_ID;
                            if (nextQualityId % 10 < 5)
                                nextQualityId += 5 - nextQualityId % 10;
                            nextQualityId++;
                            if (EmbedUpdate && (DataItem.ITEM_ID % 10 >= 9 || !Database.Server.ItemsBase.ContainsKey(nextQualityId)))
                            {
                                client.SendSysMesage("This item's Quality cannot be upgraded anymore.");
                                EmbedUpdate = false;
                            }
                            if (EmbedUpdate && UseItems.Count > 0)
                            {
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                                {
                                    uint consumed = 0;
                                    foreach (var ball in UseItems)
                                    {
                                        if (client.Inventory.RemoveOne(ball.UID, stream))
                                            consumed++;
                                    }
                                    if (consumed > 0)
                                    {
                                        if (Database.ItemType.UpQualityDB(DataItem.ITEM_ID, consumed))
                                        {
                                            dwParam1 = 1;
                                            DataItem.ITEM_ID = nextQualityId;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(client, stream);
                                        }
                                        else
                                        {
                                            if (DataItem.Durability > 200)
                                            {
                                                DataItem.Durability -= 200;
                                                DataItem.Mode = Role.Flags.ItemMode.Update;
                                                DataItem.Send(client, stream);
                                            }
                                        }
                                    }
                                }
                            }
                            if (DataItem.Position != 0)
                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }

                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpgradeDragonball, ItemUID, dwParam1, 0, 0, 0, 0));


                        break;
                    }
                default:
                    {

                        MsgGameItem DataItem;
                        if (client.TryGetItem(ItemUID, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);

                            //anti proxy --------------------
                            if (Position != (ushort)Role.Flags.ConquerItem.Fan && Position != (ushort)Role.Flags.ConquerItem.Steed
                                && Position != (ushort)Role.Flags.ConquerItem.Tower && Position != (ushort)Role.Flags.ConquerItem.RidingCrop
                              
                                && Position != (ushort)Role.Flags.ConquerItem.Wing)
                            {
                                if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                                {

                                    client.SendSysMesage("This item's Plus cannot be upgraded anymore.");

                                    return;
                                }
                            }

                            if (Action == ActionType.ChanceUpgrade || Action == ActionType.ChanceUpgrade2)
                            {
                                if (DataItem.Plus < 12 && DataItem.PlusProgress != 0)
                                {
                                    byte oldplus = DataItem.Plus;
                                    double percent = (double)DataItem.PlusProgress / (double)Database.ItemType.ComposePlusPoints(DataItem.Plus);

                                    if (Role.Core.Rate(percent))
                                    {
                                        DataItem.Plus++;
                                    }
                                    DataItem.PlusProgress = 0;
                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                    DataItem.Send(client, stream);
                                    if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                    {

                                        client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Database.Server.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");

                                    }
                                }
                                else
                                {
                                    DataItem.PlusProgress = 0;
                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                    DataItem.Send(client, stream);
                                }
                                break;
                            }


                            Queue<MsgGameItem> UseItems = new Queue<MsgGameItem>();
                            bool EmbedUpdate = false;
                            for (int x = 0; x < ItemsUIDS.Count; x++)
                            {
                                MsgGameItem itemuse;
                                if (client.Inventory.ClientItems.TryGetValue(ItemsUIDS[x], out itemuse))
                                {
                                    UseItems.Enqueue(itemuse);
                                    EmbedUpdate = true;
                                }
                                else { EmbedUpdate = false; break; }
                            }
                            if (EmbedUpdate && UseItems.Count > 0 && client.Inventory.DoSvae(UseItems, client))
                            {
                                switch (Action)
                                {
                                    case ActionType.Plus:
                                        {
                                            if (DataItem.Plus < 12)
                                            {
                                                while (UseItems.Count > 0)
                                                {

                                                    byte oldplus = DataItem.Plus;
                                                    var Stone = UseItems.Dequeue();
                                                    var type1 = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                                                    var type2 = Database.ItemType.ItemPosition(Stone.ITEM_ID);

                                                    if (type1 <= 0)
                                                    {
                                                        return;
                                                    }
                                                    if (DataItem.ITEM_ID == 300000 || Stone.ITEM_ID == 300000 || type1 == (ushort)Role.Flags.ConquerItem.Steed || type2 == (ushort)Role.Flags.ConquerItem.Steed)
                                                    {
                                                        return;
                                                    }

                                                    if (!(Stone.ITEM_ID >= 730001 && Stone.ITEM_ID <= 730008))
                                                    {
                                                        uint r1 = DataItem.ITEM_ID / 1000;
                                                        uint r2 = Stone.ITEM_ID / 1000;

                                                        if (type2 <= 0)
                                                        {
                                                            return;
                                                        }

                                                        if (r1 != r2)
                                                        {
                                                            if (type1 == 4 || type1 == 5)// سلاح 
                                                            {
                                                                if (Database.ItemType.IsBow(DataItem.ITEM_ID) && !Database.ItemType.IsBow(Stone.ITEM_ID))
                                                                    return;
                                                                else if (Database.ItemType.IsTwoHand(DataItem.ITEM_ID) && !Database.ItemType.IsTwoHand(Stone.ITEM_ID))
                                                                    return;
                                                                else if (Database.ItemType.IsOneHand(DataItem.ITEM_ID) && !Database.ItemType.IsOneHand(Stone.ITEM_ID))
                                                                    return;
                                                                else if (Database.ItemType.IsShield(DataItem.ITEM_ID) && !Database.ItemType.IsShield(Stone.ITEM_ID))
                                                                    return;
                                                                if (Database.ItemType.IsPistol(DataItem.ITEM_ID) && !Database.ItemType.IsPistol(Stone.ITEM_ID))
                                                                    return;
                                                                else if (Database.ItemType.IsBacksword(DataItem.ITEM_ID) && !Database.ItemType.IsBacksword(Stone.ITEM_ID))
                                                                    return;
                                                            }
                                                            else // other staff
                                                            {
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (type2 != 0) return;
                                                    }
                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    DataItem.PlusProgress += Stone.PlusProgress;
                                                    while (DataItem.PlusProgress >= Database.ItemType.ComposePlusPoints(DataItem.Plus) && DataItem.Plus != 12)
                                                    {
                                                        DataItem.PlusProgress -= Database.ItemType.ComposePlusPoints(DataItem.Plus);
                                                        DataItem.Plus++;
                                                        if (DataItem.Plus == 12)
                                                            DataItem.PlusProgress = 0;
                                                    }
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                                    if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                                    {
                                                        client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Database.Server.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");
                                                    }

                                                    if (client.Player.MyMentor != null)
                                                    {
                                                        client.Player.MyMentor.Mentor_Blessing += (uint)(Database.ItemType.StonePlusPoints(Stone.Plus) / 100);
                                                        Role.Instance.Associate.Member mee;
                                                        if (client.Player.MyMentor.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                                                        {
                                                            if (client.Player.MyMentor.Associat[Role.Instance.Associate.Apprentice].TryGetValue(client.Player.UID, out mee))
                                                            {
                                                                mee.Blessing += (uint)(Database.ItemType.ComposePlusPoints(Stone.Plus) / 100);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Wing)
                                            {
                                                while (UseItems.Count > 0)
                                                {
                                                    var Stone = UseItems.Dequeue();

                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    if (DataItem.PlusProgress >= 2000000)
                                                        DataItem.PlusProgress = 2000000;

                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);

                                                }
                                            }
                                            break;
                                        }
                                    case ActionType.CurrentSteed:
                                        {
                                            if (DataItem.Plus < 12)
                                            {
                                                while (UseItems.Count > 0)
                                                {

                                                    byte oldplus = DataItem.Plus;
                                                    var Stone = UseItems.Dequeue();
                                                    var type1 = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                                                    var type2 = Database.ItemType.ItemPosition(Stone.ITEM_ID);

                                                    if (DataItem.ITEM_ID != 300000 || Stone.ITEM_ID != 300000 || type1 != (ushort)Role.Flags.ConquerItem.Steed || type2 != (ushort)Role.Flags.ConquerItem.Steed || type1 <= 0 || type2 <= 0)
                                                    {
                                                        return;
                                                    }
                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    DataItem.PlusProgress += Stone.PlusProgress;
                                                    while (DataItem.PlusProgress >= Database.ItemType.ComposePlusPoints(DataItem.Plus) && DataItem.Plus != 12)
                                                    {
                                                        DataItem.PlusProgress -= Database.ItemType.ComposePlusPoints(DataItem.Plus);
                                                        DataItem.Plus++;
                                                        if (DataItem.Plus == 12)
                                                            DataItem.PlusProgress = 0;
                                                    }
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                                    if (oldplus != DataItem.Plus && DataItem.Plus >= 6)
                                                    {
                                                        client.Map.SendSysMesage("Congratulations, " + client.Player.Name + " has upgraded His " + Database.Server.ItemsBase[DataItem.ITEM_ID].Name + " to + " + DataItem.Plus + " and " + DataItem.PlusProgress + " in Progress!");
                                                    }

                                                    if (client.Player.MyMentor != null)
                                                    {
                                                        client.Player.MyMentor.Mentor_Blessing += (uint)(Database.ItemType.StonePlusPoints(Stone.Plus) / 100);
                                                        Role.Instance.Associate.Member mee;
                                                        if (client.Player.MyMentor.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                                                        {
                                                            if (client.Player.MyMentor.Associat[Role.Instance.Associate.Apprentice].TryGetValue(client.Player.UID, out mee))
                                                            {
                                                                mee.Blessing += (uint)(Database.ItemType.ComposePlusPoints(Stone.Plus) / 100);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Steed)
                                            {
                                                while (UseItems.Count > 0)
                                                {
                                                    var Stone = UseItems.Dequeue();

                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    if (DataItem.PlusProgress >= 2000000)
                                                        DataItem.PlusProgress = 2000000;

                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);

                                                }
                                            }
                                            break;
                                        }
                                    case ActionType.NewSteed:
                                        {
                                            if (DataItem.Plus < 12)
                                            {
                                                var Stone = UseItems.Dequeue();

                                                var type1 = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                                                var type2 = Database.ItemType.ItemPosition(Stone.ITEM_ID);

                                                if (DataItem.ITEM_ID != 300000 || Stone.ITEM_ID != 300000 || type1 != (ushort)Role.Flags.ConquerItem.Steed || type2 != (ushort)Role.Flags.ConquerItem.Steed || type1 <= 0 || type2 <= 0)
                                                {
                                                    return;
                                                }
                                                if (DataItem.Plus < 12)
                                                {
                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    while (DataItem.PlusProgress >= Database.ItemType.ComposePlusPoints(DataItem.Plus) && DataItem.Plus != 12)
                                                    {
                                                        DataItem.PlusProgress -= Database.ItemType.ComposePlusPoints(DataItem.Plus);
                                                        DataItem.Plus++;
                                                        if (DataItem.Plus == 12)
                                                            DataItem.PlusProgress = 0;
                                                    }
                                                }
                                                DataItem = SteedColor(DataItem, Stone.SocketProgress);
                                                DataItem.Mode = Role.Flags.ItemMode.Update;
                                                DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);


                                                while (UseItems.Count > 0)
                                                {
                                                    Stone = UseItems.Dequeue();
                                                    if (DataItem.Plus < 12)
                                                    {
                                                        DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                        while (DataItem.PlusProgress >= Database.ItemType.ComposePlusPoints(DataItem.Plus) && DataItem.Plus != 12)
                                                        {
                                                            DataItem.PlusProgress -= Database.ItemType.ComposePlusPoints(DataItem.Plus);
                                                            DataItem.Plus++;
                                                            if (DataItem.Plus == 12)
                                                                DataItem.PlusProgress = 0;
                                                        }
                                                    }
                                                    DataItem = SteedColor(DataItem, Stone.SocketProgress);
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);
                                                }
                                                DataItem.Send(client, stream);

                                                break;

                                            }
                                            else if (Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Steed)
                                            {
                                                while (UseItems.Count > 0)
                                                {
                                                    var Stone = UseItems.Dequeue();

                                                    DataItem.PlusProgress += Database.ItemType.StonePlusPoints(Stone.Plus);
                                                    if (DataItem.PlusProgress >= 2000000)
                                                        DataItem.PlusProgress = 2000000;
                                                    DataItem = SteedColor(DataItem, Stone.SocketProgress);
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(client, stream).Update(Stone, Role.Instance.AddMode.REMOVE, stream);

                                                }
                                            }
                                            break;
                                        }
                                }
                                if (DataItem.Position != 0)
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }

                        }

                        break;
                    }

            }
        }
        public static MsgGameItem SteedColor(MsgGameItem DataItem, uint SocketProgress)
        {
            int color11 = (int)DataItem.SocketProgress;
            int color22 = (int)SocketProgress;

            int G11 = color11 & 0xFF;
            int G22 = color22 & 0xFF;
            int B11 = (color11 >> 8) & 0xFF;
            int B22 = (color22 >> 8) & 0xFF;
            int R11 = (color11 >> 16) & 0xFF;
            int R22 = (color22 >> 16) & 0xFF;
            DataItem.ProgresGreen = (byte)((int)Math.Floor(0.9 * G11) + (int)Math.Floor(0.1 * G22) + 1);
            DataItem.Enchant = (byte)((int)Math.Floor(0.9 * B11) + (int)Math.Floor(0.1 * B22) + 1);
            DataItem.Bless = (byte)((int)Math.Floor(0.9 * R11) + (int)Math.Floor(0.1 * R22) + 1);
            DataItem.SocketProgress = (uint)(DataItem.ProgresGreen | (DataItem.Enchant << 8) | (DataItem.Bless << 16));
            return DataItem;
        }
    }
}
