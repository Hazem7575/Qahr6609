using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conquer6609
{
    // Unlimited arena rooms (map 700, one dynamic id per room).
    // Thread-safe: rooms are created on the NPC thread, checked from the HitPoints/Stamina getters on every
    // thread, and ended by the bot thread. IsArenaRoom(dynId) is O(1) and allocation-free.
    // A room ends when its bot is removed while the owner is gone, or (sweep, every 30 s) when no player
    // and no bot has been in its dynamic id for RoomGraceSeconds.
    public class UnlimitedArenaRooms
    {
        internal sealed class RoomInfo
        {
            public readonly uint RoomNo;
            public long LastUsedTicks;
            public RoomInfo(uint roomNo) { RoomNo = roomNo; LastUsedTicks = DateTime.UtcNow.Ticks; }
        }

        // Room Number -> dynamic id
        private static readonly ConcurrentDictionary<uint, uint> byRoomNo = new ConcurrentDictionary<uint, uint>();
        // dynamic id -> room (the set used by IsArenaRoom)
        private static readonly ConcurrentDictionary<uint, RoomInfo> byDynamicId = new ConcurrentDictionary<uint, RoomInfo>();
        private static readonly object writeSync = new object();

        private const int RoomGraceSeconds = 60;
        private const int SweepIntervalMs = 30000;
        private static int lastSweep = Environment.TickCount;

        // Dictionary-like facade kept so the NPC code (Maps.ContainsKey / Maps[..] / Maps.Add) keeps working.
        public static readonly RoomTable Maps = new RoomTable();

        public sealed class RoomTable
        {
            internal RoomTable() { }
            public bool ContainsKey(uint roomNo) { return byRoomNo.ContainsKey(roomNo); }
            public uint this[uint roomNo]
            {
                get
                {
                    uint dynId = byRoomNo[roomNo];
                    Touch(dynId);
                    return dynId;
                }
            }
            public void Add(uint roomNo, uint dynamicId) { AddRoom(roomNo, dynamicId); }
            public bool Remove(uint roomNo) { return RemoveRoom(roomNo); }
            /// <summary>O(1): true when dynamicId belongs to an arena room.</summary>
            public bool ContainsValue(uint dynamicId) { return byDynamicId.ContainsKey(dynamicId); }
            public int Count { get { return byDynamicId.Count; } }
        }

        /// <summary>O(1), lock-free: true when the dynamic id belongs to an unlimited arena room.</summary>
        public static bool IsArenaRoom(uint dynamicId)
        {
            return byDynamicId.ContainsKey(dynamicId);
        }

        public static bool TryGetRoom(uint roomNo, out uint dynamicId)
        {
            if (byRoomNo.TryGetValue(roomNo, out dynamicId))
            {
                Touch(dynamicId);
                return true;
            }
            return false;
        }

        public static bool AddRoom(uint roomNo, uint dynamicId)
        {
            lock (writeSync)
            {
                if (byRoomNo.ContainsKey(roomNo))
                    return false;
                byDynamicId[dynamicId] = new RoomInfo(roomNo);
                byRoomNo[roomNo] = dynamicId;
                return true;
            }
        }

        public static bool RemoveRoom(uint roomNo)
        {
            lock (writeSync)
            {
                uint dynId;
                if (!byRoomNo.TryRemove(roomNo, out dynId))
                    return false;
                RoomInfo info;
                if (byDynamicId.TryGetValue(dynId, out info) && info.RoomNo == roomNo)
                    byDynamicId.TryRemove(dynId, out info);
                return true;
            }
        }

        public static bool RemoveByDynamicId(uint dynamicId)
        {
            lock (writeSync)
            {
                RoomInfo info;
                if (!byDynamicId.TryRemove(dynamicId, out info))
                    return false;
                uint cur;
                if (byRoomNo.TryGetValue(info.RoomNo, out cur) && cur == dynamicId)
                    byRoomNo.TryRemove(info.RoomNo, out cur);
                return true;
            }
        }

        private static void Touch(uint dynamicId)
        {
            RoomInfo info;
            if (byDynamicId.TryGetValue(dynamicId, out info))
                info.LastUsedTicks = DateTime.UtcNow.Ticks;
        }

        /// <summary>Ends the room when no online player and no bot is inside it any more.</summary>
        public static void RemoveIfEmpty(uint dynamicId)
        {
            if (!byDynamicId.ContainsKey(dynamicId))
                return;
            if (IsOccupied(dynamicId))
                return;
            RemoveByDynamicId(dynamicId);
        }

        private static bool IsOccupied(uint dynamicId)
        {
            foreach (var client in Database.Server.GamePoll.Values)
            {
                var player = client != null ? client.Player : null;
                if (player != null && player.DynamicID == dynamicId)
                    return true;
            }
            foreach (var bot in BotManager.ConquerAI.Pool.Values)
            {
                var player = bot != null ? bot.Player : null;
                if (player != null && player.DynamicID == dynamicId)
                    return true;
            }
            return false;
        }

        /// <summary>Cheap periodic cleanup (self-throttled to every 30 s). Called from the bot thread.</summary>
        public static void Sweep()
        {
            int now = Environment.TickCount;
            if (unchecked(now - lastSweep) < SweepIntervalMs)
                return;
            lastSweep = now;
            if (byDynamicId.IsEmpty)
                return;
            try
            {
                var used = new HashSet<uint>();
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    var player = client != null ? client.Player : null;
                    if (player != null && byDynamicId.ContainsKey(player.DynamicID))
                        used.Add(player.DynamicID);
                }
                foreach (var bot in BotManager.ConquerAI.Pool.Values)
                {
                    var player = bot != null ? bot.Player : null;
                    if (player != null && byDynamicId.ContainsKey(player.DynamicID))
                        used.Add(player.DynamicID);
                }
                long nowTicks = DateTime.UtcNow.Ticks;
                long grace = TimeSpan.FromSeconds(RoomGraceSeconds).Ticks;
                foreach (var kv in byDynamicId)
                {
                    if (used.Contains(kv.Key))
                    {
                        kv.Value.LastUsedTicks = nowTicks;
                        continue;
                    }
                    if (nowTicks - kv.Value.LastUsedTicks > grace)
                        RemoveByDynamicId(kv.Key);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[UnlimitedArenaRooms] sweep: " + e.Message);
            }
        }
    }
}
