using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Database;

namespace Conquer6609.Game.MsgMonster
{
    public unsafe partial class MonsterRole
    {
        #region Boss garment / mount drop pools
        // The boss garment/mount drop used to retry `pick a random entry of ItemType.Garments (ToArray() every try)
        // until it is in CoatStorage with Stars < N`: rejection sampling, i.e. a uniform pick among the eligible
        // entries. Same distribution, computed once (rebuilt if the source tables change size) and picked once;
        // no eligible entry = no drop instead of an endless loop.
        sealed class DropPool
        {
            public int SourceCount = -1, StorageCount = -1;
            public Database.ItemType.DBItem[] Items = new Database.ItemType.DBItem[0];
        }
        static readonly object DropPoolSync = new object();
        static readonly Dictionary<int, DropPool> GarmentPools = new Dictionary<int, DropPool>();
        static readonly Dictionary<int, DropPool> MountPools = new Dictionary<int, DropPool>();

        static Database.ItemType.DBItem PickPool(Dictionary<int, DropPool> pools, Dictionary<uint, Database.ItemType.DBItem> source
            , Dictionary<uint, Database.CoatStorage.StorageItem> storage, int maxStarsExclusive)
        {
            Database.ItemType.DBItem[] items;
            lock (DropPoolSync)
            {
                DropPool pool;
                if (!pools.TryGetValue(maxStarsExclusive, out pool))
                {
                    pool = new DropPool();
                    pools[maxStarsExclusive] = pool;
                }
                if (pool.SourceCount != source.Count || pool.StorageCount != storage.Count)
                {
                    var list = new List<Database.ItemType.DBItem>();
                    foreach (var item in source.Values)
                    {
                        Database.CoatStorage.StorageItem stored;
                        if (item != null && storage.TryGetValue(item.ID, out stored) && stored.Stars < maxStarsExclusive)
                            list.Add(item);
                    }
                    pool.Items = list.ToArray();
                    pool.SourceCount = source.Count;
                    pool.StorageCount = storage.Count;
                }
                items = pool.Items;
            }
            if (items.Length == 0)
                return null;
            return items[Program.GetRandom.Next(0, items.Length)];
        }
        /// <summary>Uniform random garment of ItemType.Garments whose CoatStorage stars are below maxStarsExclusive (null if none).</summary>
        public static Database.ItemType.DBItem PickDropGarment(int maxStarsExclusive)
        {
            return PickPool(GarmentPools, Database.ItemType.Garments, Database.CoatStorage.Garments, maxStarsExclusive);
        }
        /// <summary>Uniform random mount of ItemType.SteedMounts whose CoatStorage stars are below maxStarsExclusive (null if none).</summary>
        public static Database.ItemType.DBItem PickDropMount(int maxStarsExclusive)
        {
            return PickPool(MountPools, Database.ItemType.SteedMounts, Database.CoatStorage.Mounts, maxStarsExclusive);
        }
        #endregion

        public void DropItem(ServerSockets.Packet stream, uint OwnerItem, Role.GameMap map, uint ItemID, ushort XX, ushort YY, MsgFloorItem.MsgItem.ItemType typ
            , uint amount, bool special, byte ID_Quality, Client.GameClient user = null, Database.ItemType.DBItem DBItem = null)
        {
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();

            DataItem.ITEM_ID = ItemID;
            if (DataItem.Durability > 100)
            {
                DataItem.Durability = (ushort)Program.GetRandom.Next(100, DataItem.Durability / 10);
                DataItem.MaximDurability = DataItem.Durability;
            }

            else
            {
                DataItem.Durability = (ushort)Program.GetRandom.Next(1, 10);
                DataItem.MaximDurability = 10;
            }

            DataItem.Color = Role.Flags.Color.Red;
            if (typ == MsgFloorItem.MsgItem.ItemType.Item)
            {
                byte sockets;
                bool lucky = false;
                if (DataItem.IsEquip)
                {
                    if (!special)
                    {

                        lucky = (ID_Quality > 7); // q>unique
                        if (!lucky)
                            lucky = (DataItem.Plus = Family.ItemGenerator.GeneratePurity()) != 0;
                        if (!lucky)
                            lucky = (DataItem.Bless = Family.ItemGenerator.GenerateBless()) != 0;
                        if (!lucky)
                        {
                            if (DataItem.IsWeapon)
                            {
                                sockets = Family.ItemGenerator.GenerateSocketCount(DataItem.ITEM_ID);

                                if (sockets >= 1)
                                    DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                else if (sockets == 2)
                                    DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                            }
                        }
                    }
                    if (DBItem != null)
                    {
                        DataItem.Durability = (ushort)Program.GetRandom.Next(1, DBItem.Durability / 10 + 10);
                        DataItem.MaximDurability = (ushort)Program.GetRandom.Next(DataItem.Durability, DBItem.Durability);
                    }
                }
                else
                {
                    if (DBItem != null)
                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                }
            }
            if (user != null)
            {
                if (user.Player.VipLevel == 6)
                {
                    /* if (DataItem.IsEquip)
                     {
                         if (DataItem.Plus > 0)
                         {
                             user.Inventory.Update(DataItem, Role.Instance.AddMode.ADD, stream);
                             return;
                         }
                     }
                     else*/
                    if (DataItem.ITEM_ID == 1088000)
                    {
                        user.Inventory.Update(DataItem, Role.Instance.AddMode.ADD, stream);
                        return;
                    }
                }
            }

            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, XX, YY, typ, amount, DynamicID, Map, OwnerItem, true, map);

            if (map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
            }
        }

        public void DropItemID(Client.GameClient killer, uint itemid, ServerSockets.Packet stream, byte range = 3, int mins = 0)
        {
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
            DataItem.ITEM_ID = itemid;
            Database.ItemType.DBItem DBItem;
            if (Database.Server.ItemsBase.TryGetValue(itemid, out DBItem))
            {
                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
            }
            if (mins > 0)
            {
                DataItem.EndDate = DateTime.Now.AddMinutes(mins);
            }
            DataItem.Color = Role.Flags.Color.Red;
            ushort xx = X;
            ushort yy = Y;
            if (killer.Map.AddGroundItem(ref xx, ref yy, range))
            {
                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                if (killer.Map.EnqueueItem(DropItem))
                {
                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                }
            }
        }
    }
}
