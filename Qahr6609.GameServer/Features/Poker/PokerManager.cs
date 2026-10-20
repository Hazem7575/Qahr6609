using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Conquer6609.Game.MsgServer.Poker
{
    /// <summary>
    /// All Texas Hold'em tables: registry, packet entry points (one lock per table) and the table ticker
    /// (own 500 ms timer, like the roulette). Disconnects, teleports and walking away are detected by the ticker,
    /// so no logout hook is needed: such a player is folded and removed from his table.
    /// </summary>
    public static class PokerManager
    {
        public static readonly Dictionary<uint, PokerTable> TablesByUID = new Dictionary<uint, PokerTable>();
        public static readonly Dictionary<uint, PokerTable> TablesById = new Dictionary<uint, PokerTable>();
        private static readonly ConcurrentDictionary<uint, PokerTable> PlayerTable = new ConcurrentDictionary<uint, PokerTable>();

        private static System.Threading.Timer ticker;
        private static int tickerBusy;

        public static bool Enabled { get { return Framework.Settings.Bool("Poker", "Enabled", true); } }

        public static bool Register(PokerTable table)
        {
            if (table == null || table.UID == 0 || TablesByUID.ContainsKey(table.UID) || TablesById.ContainsKey(table.TableId))
                return false;
            Role.GameMap map;
            if (!Database.Server.ServerMaps.TryGetValue(table.MapID, out map) || map == null)
                return false;
            if (!map.View.EnterMap<Role.IMapObj>(table))
                return false;
            TablesByUID[table.UID] = table;
            TablesById[table.TableId] = table;
            return true;
        }

        public static void StartTicker()
        {
            lock (TickerLock)
            {
                if (ticker == null && (TablesByUID.Count > 0 || !BoardTables.IsEmpty))
                    ticker = new System.Threading.Timer(OnTick, null, 1000, 500);
            }
        }

        private static readonly object TickerLock = new object();

        // ---------------------------------------------------------------- championship board tables (no map object)
        /// <summary>Board tables by RoomId (ticked with the world tables).</summary>
        public static readonly ConcurrentDictionary<uint, PokerTable> BoardTables = new ConcurrentDictionary<uint, PokerTable>();

        public static void RegisterBoard(PokerTable table)
        {
            BoardTables[table.RoomId] = table;
            StartTicker();
        }

        public static void UnregisterBoard(PokerTable table)
        {
            PokerTable t;
            BoardTables.TryRemove(table.RoomId, out t);
            foreach (var kv in PlayerTable.ToArray())
                if (kv.Value == table)
                    PlayerTable.TryRemove(kv.Key, out t);
        }

        /// <summary>Routes the player's ShowHand packets (2093/2099/2096) to his board table.</summary>
        public static void SetBoardPlayer(uint uid, PokerTable table)
        {
            if (table == null)
            {
                PokerTable t;
                PlayerTable.TryRemove(uid, out t);
            }
            else
                PlayerTable[uid] = table;
        }

        /// <summary>
        /// Logout hook: a world table folds and removes the player (TexasEntrance.OnLogout); a championship board table
        /// keeps him seated on auto play (his chips stay; the ticker switches auto play on).
        /// </summary>
        public static void OnLogout(Client.GameClient client)
        {
            var table = TableOf(client);
            if (table != null && table.IsBoard)
                return;
            OnExit(client);
        }

        /// <summary>Board players who asked to leave (2096 from the board); the championship manager drains it.</summary>
        public static readonly ConcurrentQueue<uint> BoardLeaveRequests = new ConcurrentQueue<uint>();

        private static void OnTick(object state)
        {
            if (System.Threading.Interlocked.Exchange(ref tickerBusy, 1) == 1)
                return;
            try
            {
                var now = DateTime.Now;
                foreach (var table in TablesByUID.Values.Concat(BoardTables.Values))
                {
                    try
                    {
                        lock (table.SyncRoot)
                            table.Tick(now);
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteException(e);
                    }
                }
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref tickerBusy, 0);
            }
        }

        internal static void ForgetPlayer(uint uid, PokerTable table)
        {
            PokerTable t;
            if (PlayerTable.TryGetValue(uid, out t) && t == table)
                PlayerTable.TryRemove(uid, out t);
        }

        private static PokerTable TableOf(Client.GameClient client)
        {
            PokerTable t;
            if (client == null || client.Player == null)
                return null;
            return PlayerTable.TryGetValue(client.Player.UID, out t) ? t : null;
        }

        private static bool CanPlay(Client.GameClient client)
        {
            if (!Enabled || client.Player == null || !client.Player.Alive)
                return false;
            if (client.InTrade || client.IsWatching())
                return false;
            return true;
        }

        /// <summary>MsgTexasInteractive from the client (sit on a chair of a table in the world).</summary>
        public static void OnInteractive(Client.GameClient client, PokerPackets.InteractiveType type, uint tableUid, byte seat)
        {
            if (type != PokerPackets.InteractiveType.Sit && type != PokerPackets.InteractiveType.RequestSeat)
                return;
            PokerTable table;
            if (!TablesByUID.TryGetValue(tableUid, out table))
                return;
            if (!CanPlay(client))
                return;
            var current = TableOf(client);
            if (current != null && current != table)
            {
                client.SendSysMesage("You are playing at another table. Please leave it first.");
                return;
            }
            lock (table.SyncRoot)
            {
                if (table.Sit(client, seat))
                    PlayerTable[client.Player.UID] = table;
            }
        }

        /// <summary>MsgShowHandEnter from the table interface: 1 = take a seat, 2 = watch instead of waiting for the next hand.</summary>
        public static void OnEnter(Client.GameClient client, PokerJoinType joinType, ushort seat, uint tableId)
        {
            PokerTable table;
            if (!TablesById.TryGetValue(tableId, out table))
                return;
            var current = TableOf(client);
            if (current != null && current != table)
                return;
            lock (table.SyncRoot)
            {
                switch (joinType)
                {
                    case PokerJoinType.Playing:
                        if (!CanPlay(client))
                            return;
                        if (current == null)
                            return;// the interface is only open for members of this table
                        if (table.Sit(client, seat))
                            PlayerTable[client.Player.UID] = table;
                        break;
                    case PokerJoinType.Watch:
                        if (current == table)
                            table.ToWatcher(client);
                        break;
                }
            }
        }

        /// <summary>MsgShowHandExit from the client: leave the table (a hand in progress is folded, its bets are lost).</summary>
        public static void OnExit(Client.GameClient client)
        {
            var table = TableOf(client);
            if (table != null && table.IsBoard)
            {
                // championship: leaving = giving up (STR_TEXAS_CHAMPIONSHIP_LEAVE_WARNING); the manager eliminates him
                BoardLeaveRequests.Enqueue(client.Player.UID);
                return;
            }
            if (table == null)
            {
                // not at a table: still clear a stale sitting state
                if (client.Player.PokerTableID != 0)
                {
                    PokerTable t;
                    if (TablesByUID.TryGetValue(client.Player.PokerTableID, out t))
                    {
                        lock (t.SyncRoot)
                            t.Remove(client.Player.UID, client, null);
                    }
                    client.Player.PokerTableID = 0;
                    client.Player.PokerSeat = 0;
                }
                return;
            }
            lock (table.SyncRoot)
                table.Remove(client.Player.UID, client, null);
        }

        public static void OnCallAction(Client.GameClient client, PokerAction action, long amount)
        {
            var table = TableOf(client);
            if (table == null)
                return;
            lock (table.SyncRoot)
                table.OnAction(client, action, amount);
        }

        public static void OnTrusteeship(Client.GameClient client, bool on)
        {
            var table = TableOf(client);
            if (table == null)
                return;
            lock (table.SyncRoot)
                table.SetTrustee(client, on);
        }

        /// <summary>True while the player sits at (or watches) a poker table.</summary>
        public static bool IsAtTable(Client.GameClient client)
        {
            return TableOf(client) != null;
        }

        /// <summary>
        /// Free spot for a table near (x, y): the 5x5 square around it walkable and no other table closer than 10 cells.
        /// keepAway: minimum distance from (x, y) (keeps the room entrance free). Returns false if nothing was found within 30 cells.
        /// </summary>
        public static bool FindSpot(uint mapId, ushort x, ushort y, out ushort fx, out ushort fy, int keepAway = 0)
        {
            fx = x; fy = y;
            Role.GameMap map;
            if (!Database.Server.ServerMaps.TryGetValue(mapId, out map) || map == null)
                return false;
            for (int r = 0; r <= 30; r++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (Math.Max(Math.Abs(dx), Math.Abs(dy)) != r || r < keepAway)
                            continue;
                        int cx = x + dx, cy = y + dy;
                        if (cx < 3 || cy < 3 || cx > 65000 || cy > 65000)
                            continue;
                        if (!Clear(map, cx, cy))
                            continue;
                        bool near = false;
                        foreach (var t in TablesByUID.Values)
                            if (t.MapID == mapId && Math.Max(Math.Abs(t.X - cx), Math.Abs(t.Y - cy)) < 10)
                                near = true;
                        if (near)
                            continue;
                        fx = (ushort)cx;
                        fy = (ushort)cy;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Walkable cell right next to a table (ring 3..6 around its 5x5 footprint), so a player put there is in view of it
        /// (PokerTable.NearTable). Unlike FindSpot it does not keep away from the table itself. False if none.
        /// </summary>
        public static bool FindStandSpot(PokerTable table, out ushort fx, out ushort fy)
        {
            fx = table.X; fy = table.Y;
            Role.GameMap map;
            if (!Database.Server.ServerMaps.TryGetValue(table.MapID, out map) || map == null)
                return false;
            for (int r = 3; r <= 6; r++)
            {
                // below the table first (where the old Match-up put players), then the rest of the ring
                for (int pass = 0; pass < 2; pass++)
                {
                    for (int dx = -r; dx <= r; dx++)
                    {
                        for (int dy = -r; dy <= r; dy++)
                        {
                            if (Math.Max(Math.Abs(dx), Math.Abs(dy)) != r || (pass == 0) != (dy == r))
                                continue;
                            int cx = table.X + dx, cy = table.Y + dy;
                            if (cx < 1 || cy < 1 || cx > 65000 || cy > 65000 || !map.ValidLocation((ushort)cx, (ushort)cy))
                                continue;
                            fx = (ushort)cx;
                            fy = (ushort)cy;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool Clear(Role.GameMap map, int cx, int cy)
        {
            for (int ax = cx - 2; ax <= cx + 2; ax++)
                for (int ay = cy - 2; ay <= cy + 2; ay++)
                    if (!map.ValidLocation((ushort)ax, (ushort)ay))
                        return false;
            return true;
        }
    }
}
