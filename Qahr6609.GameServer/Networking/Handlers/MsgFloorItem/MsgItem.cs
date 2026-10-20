using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Role;

namespace Conquer6609.Game.MsgFloorItem
{
    public class MsgItem : Role.IMapObj
    {
        public enum ItemType
        {
            Item, Money, Cps, Effect
        }
        public static Extensions.Counter UIDS = new Extensions.Counter(900027);

        public Extensions.Time32 Expire = new Extensions.Time32();

        public uint ItemOwner;
        public bool ToMySelf;

        public bool ExpireMySelf { get { return Extensions.Time32.Now > Expire.AddSeconds(20); } }
        public bool AllowDynamic { get; set; }
        public uint IndexInScreen { get; set; }
        public uint DynamicID { get; set; }
        public uint Map { get; set; }
        public bool Alive { get { return Expire.AddSeconds(SpecialSecounds != 0 ? SpecialSecounds : 40) > Extensions.Time32.Now; } }
        public Extensions.Time32 AttackStamp = Extensions.Time32.Now;


        public MsgServer.MsgGameItem ItemBase;
        public MsgFloorItem.MsgItemPacket MsgFloor;
        public Role.MapObjectType ObjType { get; set; }
        public bool IsTrap() { return MsgFloor.DropType == MsgDropID.Effect; }   
     
        public uint UID { get { return MsgFloor.m_UID; } set { MsgFloor.m_UID = value; } }
        public ushort X { get { return MsgFloor.m_X; } set { MsgFloor.m_X = value; } }
        public ushort Y { get { return MsgFloor.m_Y;  }
            set { MsgFloor.m_Y = value; }
        }

        public Role.GameMap GMap;
        public ItemType Typ;
        public uint Gold;
        public uint ConquerPoints;
        private int SpecialSecounds = 0;

        public Client.GameClient OwnerEffert = null;
        public Database.MagicType.Magic DBSkill = null;
        public byte SpellSoul = 0;
    
        public MsgItem(MsgServer.MsgGameItem item, ushort x, ushort y
            , ItemType Mode, uint Amount, uint dinamicid, uint _mapid
            , uint _ItemOwner, bool _ToMySelf, Role.GameMap _map, int specialSecound = 0)
        {
            AllowDynamic = false;
            SpecialSecounds = specialSecound;
            GMap = _map;
            ItemOwner = _ItemOwner;
            
            ToMySelf = _ToMySelf;
            Map = _mapid;
            DynamicID = dinamicid;
            Typ = Mode;
            MsgFloor = MsgFloorItem.MsgItemPacket.Create();
            if (item != null)
            {
                ItemBase = item;
             
                if (item.Purification.ItemUID != 0 && item.Purification.InLife)
                    MsgFloor.m_ID = item.Purification.PurificationItemID;
                else
                    MsgFloor.m_ID = item.ITEM_ID;
                MsgFloor.m_Color = (byte)item.Color;
                MsgFloor.Plus = item.Plus;
            }
            MsgFloor.ItemOwnerUID = ItemOwner;
            MsgFloor.m_UID = UIDS.Next;
            MsgFloor.m_X = x;
            MsgFloor.m_Y = y;

            ObjType = MapObjectType.Item;
            switch (Mode)
            {
                case ItemType.Cps: ConquerPoints += Amount; break;
                case ItemType.Money: Gold += Amount; break;
            }
            Expire = Extensions.Time32.Now;
            
        }

     
        public bool CanSee(IMapObj obj)
        {
            if (DynamicID != obj.DynamicID) return false;
            if (Map != obj.Map) return false;
            if (Core.GetDistance(MsgFloor.m_X, MsgFloor.m_Y, obj.X, obj.Y) >= 18)
                return false;
            return true;
        }
        public unsafe void Send(ServerSockets.Packet msg)
        {
            foreach (var user in GMap.View.Roles(MapObjectType.Player, X, Y, p => CanSee(p)))
            {
                user.Send(msg);
            }
        }
        public unsafe void SendAll(ServerSockets.Packet stream, MsgDropID Typ)
        {
            if (Alive)
            {
                MsgFloor.DropType = Typ;
                stream = stream.ItemPacketCreate(MsgFloor);
                Send(stream);
            }
            else if(Typ == MsgDropID.Remove || Typ == MsgDropID.RemoveEffect)
            {
                MsgFloor.DropType = Typ;
                stream = stream.ItemPacketCreate(MsgFloor);
                Send(stream);
                if ((GMap.cells[X, Y] & MapFlagType.Item) == MapFlagType.Item)
                    GMap.cells[X, Y] &= ~MapFlagType.Item;
            }
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool View)
        {
            return (stream = stream.ItemPacketCreate(MsgFloor));
        }
    }

    /// <summary>
    /// Floor items used to expire only while a player stood near them (PoolProcesses.CheckItems / RoleView.CanAdd),
    /// so drops in empty areas stayed in the map blocks forever. Items entering a map through GameMap.EnqueueItem
    /// are tracked here and removed once per second when their normal lifetime is over, with the same removal
    /// packets CheckItems sends. Runs on the MapGroupThread (same thread as CheckItems).
    /// [Performance] FloorItemSweep = 1 (0 = off).
    /// </summary>
    public static class FloorItemExpiry
    {
        public static readonly bool Enabled = Conquer6609.Framework.Settings.Int("Performance", "FloorItemSweep", 1) != 0;
        static readonly ConcurrentQueue<MsgItem> Added = new ConcurrentQueue<MsgItem>();
        static readonly List<MsgItem> Tracked = new List<MsgItem>();
        static int lastRun;
        static bool ran;

        public static int Count { get { return Tracked.Count + Added.Count; } }

        public static void Track(MsgItem item)
        {
            if (Enabled && item != null && item.GMap != null)
                Added.Enqueue(item);
        }

        /// <summary>Called from MapGroupThread every pass; does its work at most once per second.</summary>
        public static void Process()
        {
            if (!Enabled)
                return;
            int now = Environment.TickCount;
            if (ran && unchecked(now - lastRun) < 1000)
                return;
            ran = true;
            lastRun = now;

            MsgItem added;
            while (Added.TryDequeue(out added))
                Tracked.Add(added);

            for (int i = Tracked.Count - 1; i >= 0; i--)
            {
                var item = Tracked[i];
                try
                {
                    if (item.Alive)
                        continue;
                    Expire(item);
                }
                catch (Exception e)
                {
                    if (Conquer6609.Security.Guard.Throttle("floor-item-expiry", 60000))
                        MyConsole.WriteException(e);
                }
                int last = Tracked.Count - 1;
                Tracked[i] = Tracked[last];
                Tracked.RemoveAt(last);
            }
        }

        static unsafe void Expire(MsgItem item)
        {
            var map = item.GMap;
            if (map == null)
                return;
            // Already picked up / removed by a nearby player's CheckItems or view update?
            IMapObj current;
            if (!map.View.TryGetObject<IMapObj>(item.UID, MapObjectType.Item, item.X, item.Y, out current) || !ReferenceEquals(current, item))
                return;
            // A DBShower effect drops the tournament dragon ball when CheckItems expires it: leave that to CheckItems.
            if (item.IsTrap() && item.ItemBase != null && item.ItemBase.ITEM_ID == MsgItemPacket.DBShowerEffect)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (item.IsTrap())
                    item.SendAll(stream, MsgDropID.RemoveEffect);
                else
                    item.SendAll(stream, MsgDropID.Remove);
                map.View.LeaveMap<Role.IMapObj>(item);
            }
        }
    }
}
