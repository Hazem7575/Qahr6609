using Conquer6609.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetSuperFlag(this ServerSockets.Packet stream, out MsgSuperFlag.Types Actions, out uint Index, out uint UID)
        {
            Actions = (MsgSuperFlag.Types)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            Index = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet CreateSuperFlag(this ServerSockets.Packet stream, MsgGameItem item, Dictionary<uint, Tuple<uint, uint, uint, string>> Gates)
        {
            stream.InitWriter();
            stream.Write((uint)MsgSuperFlag.Types.Info);
            stream.Write(item.UID);
            stream.Write((uint)Gates.Count);
            stream.Write((uint)Gates.Count);
            stream.Write((uint)0);
            stream.Write((uint)item.Durability);
            stream.Write(Gates.Count);
            if (Gates.Count > 0)
            {
                for (uint x = 0; x < Gates.Count; x++)
                {
                    stream.Write(x);
                    stream.Write(Gates[x].Item1);
                    stream.Write(Gates[x].Item2);
                    stream.Write(Gates[x].Item3);
                    stream.Write(Gates[x].Item4, 32);
                }
            }
            stream.Finalize(GamePackets.MsgSuperFlag);
            return stream;
        }
    }
    public unsafe struct MsgSuperFlag
    {
        [Flags]
        public enum Types : uint
        {
            Info = 0,
            Record = 1,
            Teleport = 3,
            Renew = 4
        }

        [PacketAttribute(GamePackets.MsgSuperFlag)]
        public static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Server.NoAgateMap.Contains(user.Player.Map))
            {
                user.SendSysMesage("You can not use this item here!");
                return;
            }
            Types Act;
            uint Index, ItemUID;
            stream.GetSuperFlag(out Act, out Index, out ItemUID);
            MsgGameItem Item;
            if (user.Inventory.TryGetItem(ItemUID, out Item))
            {
                if (Item.ITEM_ID != Database.ItemType.MemoryAgate)
                    return;
                Dictionary<uint, Dictionary<uint, Tuple<uint, uint, uint, string>>> myAgates;
                if (!Database.Server.ClientAgates.TryGetValue(user.Player.UID, out myAgates) || !myAgates.ContainsKey(ItemUID))
                    return;
                lock (Database.Server.ClientAgates[user.Player.UID][ItemUID])
                {
                    switch (Act)
                    {
                        case Types.Record:
                            {
                                if (Index > Database.Server.ClientAgates[user.Player.UID][ItemUID].Count)
                                {
                                    Database.Server.ClientAgates[user.Player.UID][ItemUID].Add((uint)Database.Server.ClientAgates[user.Player.UID][ItemUID].Count, Tuple.Create(user.Player.Map, (uint)user.Player.X, (uint)user.Player.Y, user.Map.Name));
                                    user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                }
                                if (Database.Server.ClientAgates[user.Player.UID][ItemUID].ContainsKey(Index))
                                {
                                    Database.Server.ClientAgates[user.Player.UID][ItemUID].Remove(Index);
                                    Database.Server.ClientAgates[user.Player.UID][ItemUID].Add(Index, Tuple.Create(user.Player.Map, (uint)user.Player.X, (uint)user.Player.Y, user.Map.Name));
                                    user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                }
                                else
                                {
                                    Database.Server.ClientAgates[user.Player.UID][ItemUID].Add(Index, Tuple.Create(user.Player.Map, (uint)user.Player.X, (uint)user.Player.Y, user.Map.Name));
                                    user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                }
                                break;
                            }
                        case Types.Teleport:
                            {
                                //Index -= 1;
                                // a worn-out agate (durability 0) must be renewed before it teleports again
                                if (Item.Durability == 0)
                                {
                                    user.SendSysMesage("The Memory Agate is worn out. Renew it first.");
                                    break;
                                }
                                if (Database.Server.ClientAgates[user.Player.UID][ItemUID].ContainsKey(Index))
                                {
                                    user.Teleport((ushort)Database.Server.ClientAgates[user.Player.UID][ItemUID][Index].Item2, (ushort)Database.Server.ClientAgates[user.Player.UID][ItemUID][Index].Item3, Database.Server.ClientAgates[user.Player.UID][ItemUID][Index].Item1);
                                    Item.Durability -= 1;
                                    user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                }
                                break;
                            }
                        case Types.Renew:
                            {
                                if (Item.Durability >= Item.MaximDurability)
                                {
                                    user.SendSysMesage("The Memory Agate is already at full durability.");
                                    break;
                                }
                                int cost = (Item.MaximDurability - Item.Durability) / 2;
                                if (cost == 0) cost = 1;
                                bool renewed = false;
                                if (Item.Bound == 1)
                                {
                                    if (user.Player.BoundConquerPoints >= cost)
                                    {
                                        user.Player.BoundConquerPoints -= cost;
                                        renewed = true;
                                    }
                                }
                                else
                                {
                                    if (user.Player.ConquerPoints >= cost)
                                    {
                                        user.Player.ConquerPoints -= (uint)cost;
                                        renewed = true;
                                    }
                                }
                                if (!renewed)
                                {
                                    user.SendSysMesage("You need " + cost + (Item.Bound == 1 ? " Bound CPs" : " CPs") + " to renew the Memory Agate.");
                                    break;
                                }
                                Item.Durability = Item.MaximDurability;
                                user.Send(stream.CreateSuperFlag(Item, Database.Server.ClientAgates[user.Player.UID][ItemUID]));
                                user.SendSysMesage("MemoryAgate dura renewed.");
                                break;
                            }
                    }
                }
            }
        }
    }
}
