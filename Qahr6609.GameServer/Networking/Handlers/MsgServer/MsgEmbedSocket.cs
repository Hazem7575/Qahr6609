using Conquer6609.Game.Conquer6609_Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler
{
    public static unsafe partial class MsgBuilder
    {

        public static unsafe void GetEmbedSocket(this ServerSockets.Packet stream, out uint ItemUID, out uint GemUID
            , out MsgEmbedSocket.ActionSlot Slot, out MsgEmbedSocket.ActionMode Mode)
        {
            uint unknow1 = stream.ReadUInt32();
            uint unknow2 = stream.ReadUInt32();
            ItemUID = stream.ReadUInt32();
            GemUID = stream.ReadUInt32();
            Slot = (MsgEmbedSocket.ActionSlot)stream.ReadUInt16();
            Mode = (MsgEmbedSocket.ActionMode)stream.ReadUInt16();
        }
    }
    public struct MsgEmbedSocket
    {
        public enum ActionSlot : ushort
        {
            SlotOne = 1,
            SlotTwo = 2
        }
        public enum ActionMode : ushort
        {
            Add = 0,
            Remove = 1,
        }

        [PacketAttribute(GamePackets.EmbedSocket)]
        public unsafe static void EmbedSocket(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint ItemUID;
            uint GemUID;
            ActionSlot Slot;
            ActionMode Mode;

            stream.GetEmbedSocket(out ItemUID, out GemUID, out Slot, out Mode);

            // Items cannot be socketed while a trade window or booth is open.
            if (user.Inventory.IsTradeOrBoothItem(ItemUID))
                return;

            MsgGameItem DataItem;
            if (user.TryGetItem(ItemUID, out DataItem))
            {
                ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                if (Position <= 0)
                {
                    Console.WriteLine("fuck player " + user.Player.Name + " try use UpdateLevel bug");
                    Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, 99999, "Hack  UpdateLevel");
                    string log = ("[Hack]" + user.Player.Name + "He Try To Use hack UpdateLevel Bug ");
                    Logs.OnDequeue(log);
                    user.Socket.Disconnect();
                    return;
                }
                //anti-proxy-----------------------
                if (Position != (ushort)Role.Flags.ConquerItem.Fan && Position != (ushort)Role.Flags.ConquerItem.Tower
                    && Position != (ushort)Role.Flags.ConquerItem.Wing)
                {
                    if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                    {

                        user.SendSysMesage("Sorry can't make socket in this item.");

                        return;
                    }
                }
                //------------------------
                switch (Mode)
                {
                    case ActionMode.Add:
                        {
                            // The gem must be a different item held in the inventory.
                            MsgGameItem Gem;
                            if (GemUID != ItemUID && user.Inventory.TryGetItem(GemUID, out Gem) && Gem != null)
                            {
                                ushort Position2 = Database.ItemType.ItemPosition(Gem.ITEM_ID);
                                if (Position2 != 0)
                                {
                                    return;
                                }
                                if (Gem.ITEM_ID % 1000 == 0)
                                {
                                    Console.WriteLine(user.Player.Name + "This Player Youse Hack items");
                                    user.Socket.Disconnect();
                                    return;
                                }
                                if (Enum.IsDefined(typeof(Role.Flags.Gem), (byte)(Gem.ITEM_ID % 1000)))
                                {
                                    Role.Flags.Gem GemType = (Role.Flags.Gem)(Gem.ITEM_ID % 1000);

                                    //anti proxy------------------
                                    if (Position == (ushort)Role.Flags.ConquerItem.Fan)
                                    {
                                        if (!Database.ItemType.CheckAddGemFan(GemType))
                                            return;
                                    }
                                    if (Position == (ushort)Role.Flags.ConquerItem.Tower)
                                    {
                                        if (!Database.ItemType.CheckAddGemTower(GemType))
                                            return;
                                    }
                                    if (Position == (ushort)Role.Flags.ConquerItem.Wing)
                                    {
                                        if (!Database.ItemType.CheckAddGemWing(GemType, (byte)Slot))
                                            return;
                                    }
                                    if (Position != (ushort)Role.Flags.ConquerItem.Fan && Position != (ushort)Role.Flags.ConquerItem.Tower
                                        && Position != (ushort)Role.Flags.ConquerItem.Wing)
                                    {
                                        if (Database.ItemType.CheckAddGemFan(GemType) || Database.ItemType.CheckAddGemTower(GemType))
                                            return;
                                    }
                                    //------------------------

                                    switch (Slot)
                                    {
                                        case ActionSlot.SlotOne:
                                            {
                                                if (DataItem.SocketOne == Role.Flags.Gem.EmptySocket)
                                                {
                                                    // The gem is consumed before the socket is filled.
                                                    if (!user.Inventory.RemoveOne(Gem.UID, stream))
                                                        return;
                                                    DataItem.SocketOne = GemType;
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Send(user, stream);
                                                    if (DataItem.Position != 0)
                                                        user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                                }
                                                break;
                                            }
                                        case ActionSlot.SlotTwo:
                                            {

                                                if ((byte)DataItem.SocketOne > 0 && (byte)DataItem.SocketOne < 255 || Position == (ushort)Role.Flags.ConquerItem.Wing)
                                                {
                                                    if (DataItem.SocketTwo == Role.Flags.Gem.EmptySocket)
                                                    {
                                                        // The gem is consumed before the socket is filled.
                                                        if (!user.Inventory.RemoveOne(Gem.UID, stream))
                                                            return;
                                                        DataItem.SocketTwo = GemType;
                                                        DataItem.Mode = Role.Flags.ItemMode.Update;
                                                        DataItem.Send(user, stream);
                                                        if (DataItem.Position != 0)
                                                            user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                                    }
                                                }
                                                else
                                                {
                                                    goto case ActionSlot.SlotOne;
                                                }
                                                break;
                                            }

                                    }

                                }
                            }

                            break;
                        }
                    case ActionMode.Remove:
                        {
                            switch (Slot)
                            {
                                case ActionSlot.SlotOne:
                                    {
                                        if (DataItem.SocketOne != Role.Flags.Gem.NoSocket)
                                        {
                                            DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                            if (DataItem.SocketTwo != Role.Flags.Gem.NoSocket && DataItem.SocketTwo != Role.Flags.Gem.EmptySocket
                                                && Position != (ushort)Role.Flags.ConquerItem.Wing)
                                            {
                                                DataItem.SocketOne = DataItem.SocketTwo;
                                                DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                            }
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(user, stream);
                                            if (DataItem.Position != 0)
                                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                        }
                                        break;
                                    }
                                case ActionSlot.SlotTwo:
                                    {
                                        if (DataItem.SocketTwo != Role.Flags.Gem.NoSocket && DataItem.SocketTwo != Role.Flags.Gem.EmptySocket)
                                        {
                                            DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(user, stream);
                                            if (DataItem.Position != 0)
                                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                        }
                                        else if (DataItem.SocketOne != Role.Flags.Gem.NoSocket)
                                        {
                                            DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Send(user, stream);
                                            if (DataItem.Position != 0)
                                                user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }

                }
            }
        }
    }
}
