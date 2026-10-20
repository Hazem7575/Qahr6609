using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgOsShop
    {
        public static unsafe void GetOsShop(this ServerSockets.Packet stream, out byte type,
            out ushort DwParam1, out ushort DwParam2, out ushort DwParam3, out byte Count, out ushort DwParam4)
        {
            type = stream.ReadUInt8();
            DwParam1 = stream.ReadUInt16();
            DwParam2 = stream.ReadUInt16();
            DwParam3 = stream.ReadUInt16();
            Count = stream.ReadUInt8();
            DwParam4 = stream.ReadUInt16();

        }
        public static unsafe ServerSockets.Packet OsShopCreate(this ServerSockets.Packet stream, byte type,
             ushort DwParam1,  ushort DwParam2,  ushort DwParam3,  byte Count,  ushort DwParam4)
        {
            stream.InitWriter();

            stream.Write(type);
            stream.Write(DwParam1);
            stream.Write(DwParam2);
            stream.Write(DwParam3);
            stream.Write(Count);
            stream.Write(DwParam4);

            stream.Finalize(GamePackets.MsgOsShop);
            return stream;
        }

        // Quick-buy (client: the two potion slots of the quick-buy bar). The client sends a protobuf message:
        // field 1 = 0, field 2 = item type, field 3 = pay type (0 = Bound CPs first then CPs, 1 = CPs, 2 = Bound CPs),
        // field 4 = count. Each unit is one mall pouch: the pouch price buys 5 potions of the requested type.
        public const int PotionsPerUnit = 5;
        static readonly Dictionary<uint, uint> QuickBuyPouch = new Dictionary<uint, uint>()
        {
            { 1003010, 725067 },//7 Star Ointment <- 7 Star Pouch
            { 1004010, 725068 },//Serenity Pill  <- Serenity Pouch
        };

        static uint ReadVarint(ServerSockets.Packet stream, ref int budget)
        {
            uint value = 0;
            for (int shift = 0; shift < 35 && budget > 0; shift += 7)
            {
                byte b = stream.ReadUInt8();
                budget--;
                value |= (uint)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    break;
            }
            return value;
        }

        [PacketAttribute(GamePackets.MsgOsShop)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint[] fields = new uint[5];
            int budget = 40;
            while (budget > 0)
            {
                byte tag = stream.ReadUInt8();
                budget--;
                int number = tag >> 3;
                if (tag == 0 || (tag & 7) != 0 || number < 1 || number > 4)
                    break;
                fields[number] = ReadVarint(stream, ref budget);
            }
            uint itemId = fields[2], payType = fields[3], count = fields[4];
            if (count == 0 || count > 40 || payType > 2)
                return;
            uint pouchId;
            if (!QuickBuyPouch.TryGetValue(itemId, out pouchId))
                return;
            Database.ItemType.DBItem pouch;
            if (!Database.Server.ItemsBase.TryGetValue(pouchId, out pouch) || !Database.Server.ItemsBase.ContainsKey(itemId))
                return;
            if (!user.Inventory.HaveSpace((byte)count))
            {
                user.SendSysMesage("Please make " + count + " more spaces in your inventory.");
                return;
            }
            long cost = (long)Math.Max(1u, pouch.ConquerPointsWorth) * count;
            bool payBound;
            if (payType == 2 || (payType == 0 && user.Player.BoundConquerPoints >= cost))
                payBound = true;
            else
                payBound = false;
            if (payBound ? user.Player.BoundConquerPoints < cost : user.Player.ConquerPoints < cost)
            {
                user.SendSysMesage("You do not have enough " + (payBound ? "Bound CPs" : "CPs") + ".");
                return;
            }
            List<uint> Items = new List<uint>();
            foreach (var item in user.Inventory.ClientItems.Values)
                Items.Add(item.UID);
            if (!user.Inventory.AddItemWitchStack(itemId, 0, (ushort)(count * PotionsPerUnit), stream, payBound))
                return;
            // the currency setters send the client update themselves
            if (payBound)
                user.Player.BoundConquerPoints -= (int)cost;
            else
                user.Player.ConquerPoints -= (uint)cost;
            if (user.IsConnectedInterServer())
            {
                foreach (var item in user.Inventory.ClientItems.Values)
                    if (!Items.Contains(item.UID))
                        item.Send(user.PipeClient, stream);
            }
        }

    }
}
