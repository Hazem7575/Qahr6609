using System;
using System.Collections.Generic;
using System.Linq;

namespace Conquer6609.Game.MsgServer.Poker
{
    /// <summary>MsgShowHandEnter join types (client CShowHandPlayer::SHP_JT_*).</summary>
    public enum PokerJoinType : ushort
    {
        None = 0,
        Playing = 1,
        Watch = 2,
        /// <summary>Seated while a hand is running: joins the next hand (client shows STR_SH_PRE_JOIN_TIP).</summary>
        PreJoin = 3
    }

    public class PokerSeatInfo
    {
        public Client.GameClient Client;
        public uint UID;
        public byte Seat;
        public PokerJoinType Join;
        public bool Trustee;
        /// <summary>Must leave after the current result (not enough money).</summary>
        public bool Kicked;
        /// <summary>Board table: auto play was switched on by the server because the player went offline.</summary>
        public bool OfflineTrustee;
    }

    /// <summary>
    /// Where the chips of a table come from. World tables use the player's own silver/CPs (no bank); championship
    /// tables use tournament chips kept per player UID (the player may be offline or re-logged with a new client object).
    /// </summary>
    public interface IPokerChipBank
    {
        long Balance(uint uid);
        /// <summary>Takes chips; false (nothing taken) when the balance is too small.</summary>
        bool Debit(uint uid, long amount);
        void Credit(uint uid, long amount);
    }

    /// <summary>
    /// Owner of a board table (championship). Called with the table's SyncRoot held: implementations must only queue
    /// work (never take a lock that is held while a table lock is requested).
    /// </summary>
    public interface IPokerTableOwner
    {
        /// <summary>A hand result is over; the table is on Hold until the owner calls Release().</summary>
        void OnHandFinished(PokerTable table);
    }

    /// <summary>
    /// A Texas Hold'em table: a map object (sent as MsgTexasNpcInfo through the StaticRole view list), up to 9 seats,
    /// watchers, and the hand in progress. Players bet their own silver (or CPs on CP tables) directly, like official:
    /// every chip they put in is taken from their money at once, the pot is paid out at the end of the hand, and
    /// leaving during a hand forfeits what was already bet (STR_SH_LEAVE_WARNING).
    /// All methods must be called with SyncRoot held (PokerManager does it).
    /// </summary>
    public class PokerTable : Role.IMapObj
    {
        public const int MaxSeatsLimit = 9;// client CTexasPoker::CHAIRS_NUM

        // ---------------------------------------------------------------- configuration (PokerTables.txt)
        public uint UID { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public uint MapID;
        public uint Mesh = 9007;
        public ushort RoleType = 110;
        /// <summary>Table number shown by the client - 1.</summary>
        public uint TableId;
        public bool NoLimit;
        /// <summary>0 = silver, 1 = CPs.</summary>
        public byte MoneyType;
        public long BigBlind;
        public int MaxSeats = MaxSeatsLimit;
        /// <summary>Most chips a player brings into one hand (0 = his whole silver/CPs, official).</summary>
        public long MaxStake;
        /// <summary>Money needed to sit down (client checks the same: 5 x big blind).</summary>
        public long MinMoney { get { return BigBlind * 5; } }// official: texas_match_type.dat min starting chips = 5 x big blind (the client checks the same)

        // ---------------------------------------------------------------- championship board tables
        /// <summary>Tournament chips (null = the players' own silver/CPs).</summary>
        public IPokerChipBank Bank;
        /// <summary>Set for a championship table shown in the client's CDlgTexasBoard (no map object, no world packets,
        /// offline players stay seated on auto play, no new hand until the owner releases the table).</summary>
        public IPokerTableOwner Owner;
        public bool IsBoard { get { return Owner != null; } }
        /// <summary>Ante of the next hands (board tables only).</summary>
        public long Ante;
        /// <summary>Board table: waits for the owner (between hands: eliminations, moves, blind level).</summary>
        public bool Hold;
        /// <summary>Board table: room id the client echoes in 3400 actions 5/14/23.</summary>
        public uint RoomId;
        /// <summary>Board table: each player's chips when the last hand started (ranking of players busted in the same hand).</summary>
        public readonly Dictionary<uint, long> LastHandStartStacks = new Dictionary<uint, long>();

        public readonly object SyncRoot = new object();

        // ---------------------------------------------------------------- IMapObj
        public bool AllowDynamic { get; set; }
        public uint Map { get { return MapID; } }
        public uint DynamicID { get { return 0; } }
        public bool Alive { get { return true; } }
        public bool IsTrap() { return false; }
        public uint IndexInScreen { get; set; }
        /// <summary>Spawned through RoleView's StaticRole pass (CanAdd sends GetArray).</summary>
        public Role.MapObjectType ObjType { get { return Role.MapObjectType.StaticRole; } }
        public unsafe void Send(ServerSockets.Packet msg) { }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool View)
        {
            lock (SyncRoot)
                return stream.TableInfoCreate(this, SeatedList(), StageByte, Hand != null ? Hand.Pot : 0);
        }

        // ---------------------------------------------------------------- state
        public readonly PokerSeatInfo[] Seats = new PokerSeatInfo[MaxSeatsLimit];
        public readonly Dictionary<uint, PokerSeatInfo> Watchers = new Dictionary<uint, PokerSeatInfo>();
        public PokerHand Hand;
        private readonly Dictionary<uint, Client.GameClient> handClients = new Dictionary<uint, Client.GameClient>();
        private int dealerSeat = -1;

        private enum Phase { Idle, Starting, Betting, Street, Result }
        private Phase phase = Phase.Idle;
        private DateTime deadline;
        private DateTime actionDeadline;

        public static int ActionSeconds { get { return Math.Max(5, Framework.Settings.Int("Poker", "ActionSeconds", 20)); } }
        public static int StartDelaySeconds { get { return Math.Max(1, Framework.Settings.Int("Poker", "StartDelaySeconds", 5)); } }
        public static int ResultSeconds { get { return Math.Max(2, Framework.Settings.Int("Poker", "ResultSeconds", 6)); } }
        public static int StreetDelayMs { get { return Math.Max(300, Framework.Settings.Int("Poker", "StreetDelayMs", 1500)); } }
        public static int TrusteeDelayMs { get { return Math.Max(500, Framework.Settings.Int("Poker", "TrusteeDelayMs", 1500)); } }

        public byte StageByte
        {
            get
            {
                if (Hand == null)
                    return (byte)PokerStage.Unopened;
                if (Hand.Settled)
                    return (byte)PokerStage.Showdown;// result on screen (also a hand won by a fold before the river)
                return (byte)Hand.Stage;
            }
        }

        public List<PokerSeatInfo> SeatedList()
        {
            var list = new List<PokerSeatInfo>();
            for (int i = 0; i < Seats.Length; i++)
                if (Seats[i] != null)
                    list.Add(Seats[i]);
            return list;
        }

        public int SeatedCount
        {
            get { int n = 0; foreach (var s in Seats) if (s != null) n++; return n; }
        }

        public PokerSeatInfo FindMember(uint uid)
        {
            foreach (var s in Seats)
                if (s != null && s.UID == uid)
                    return s;
            PokerSeatInfo w;
            if (Watchers.TryGetValue(uid, out w))
                return w;
            return null;
        }

        public bool HandRunning { get { return Hand != null && !Hand.Settled; } }

        public string CurrencyName { get { return MoneyType == 1 ? "CPs" : "Silver"; } }

        // ---------------------------------------------------------------- money
        public long Balance(Client.GameClient client)
        {
            if (client == null || client.Player == null)
                return 0;
            if (Bank != null)
                return Bank.Balance(client.Player.UID);
            return MoneyType == 1 ? (long)client.Player.ConquerPoints : client.Player.Money;
        }

        private long ChunkSize { get { return MoneyType == 1 ? 100000000L : 1000000000L; } }

        /// <summary>Chips of a player: tournament bank by UID (the client may be null = offline), else his silver/CPs.</summary>
        private long ChipsOf(uint uid, Client.GameClient client)
        {
            return Bank != null ? Bank.Balance(uid) : Balance(client);
        }

        private bool TakeChips(uint uid, Client.GameClient client, long amount)
        {
            if (Bank != null)
                return amount <= 0 || Bank.Debit(uid, amount);
            return Debit(client, amount);
        }

        private void GiveChips(uint uid, Client.GameClient client, long amount)
        {
            if (Bank != null)
            {
                if (amount > 0)
                    Bank.Credit(uid, amount);
                return;
            }
            Credit(client, amount);
        }

        /// <summary>Takes chips from the player's silver/CPs (in chunks under the anti-cheat single-change limits).</summary>
        private bool Debit(Client.GameClient client, long amount)
        {
            if (amount <= 0)
                return true;
            if (Bank != null)
                return client != null && client.Player != null && Bank.Debit(client.Player.UID, amount);// board tables use TakeChips
            if (Balance(client) < amount)
                return false;
            long left = amount;
            while (left > 0)
            {
                long part = Math.Min(left, ChunkSize);
                if (MoneyType == 1)
                    client.Player.ConquerPoints -= (uint)part;
                else
                    client.Player.Money -= part;
                left -= part;
            }
            return true;
        }

        private void Credit(Client.GameClient client, long amount)
        {
            if (amount <= 0 || client == null || client.Player == null)
                return;
            if (Bank != null)
            {
                Bank.Credit(client.Player.UID, amount);
                return;
            }
            if (MoneyType == 1)
            {
                long room = (long)int.MaxValue - client.Player.ConquerPoints;
                if (amount > room)
                {
                    Security.Guard.Audit("poker-win", client.Player.UID, client.Player.Name, "CP winnings " + amount + " clamped to " + Math.Max(0, room));
                    amount = Math.Max(0, room);
                }
            }
            long left = amount;
            while (left > 0)
            {
                long part = Math.Min(left, ChunkSize);
                if (MoneyType == 1)
                    client.Player.ConquerPoints += (uint)part;
                else
                    client.Player.Money += part;
                left -= part;
            }
        }

        // ---------------------------------------------------------------- sending
        private IEnumerable<Client.GameClient> Members()
        {
            foreach (var s in Seats)
                if (s != null && s.Client != null && (!IsBoard || Online(s.Client)))
                    yield return s.Client;
            foreach (var w in Watchers.Values)
                if (w.Client != null)
                    yield return w.Client;
        }

        private void SendMembers(ServerSockets.Packet packet)
        {
            foreach (var c in Members().ToArray())
                c.Send(packet);
        }

        /// <summary>Players that can see the table (same map, in view) plus the members.</summary>
        private List<Client.GameClient> RoomViewers()
        {
            var list = new List<Client.GameClient>();
            var seen = new HashSet<uint>();
            Role.GameMap map;
            if (Database.Server.ServerMaps.TryGetValue(MapID, out map) && map != null)
            {
                foreach (var obj in map.View.Roles(Role.MapObjectType.Player, X, Y))
                {
                    var player = obj as Role.Player;
                    if (player == null || player.Owner == null || player.Map != MapID)
                        continue;
                    if (Role.Core.GetDistance(player.X, player.Y, X, Y) > Role.RoleView.ViewThreshold)
                        continue;
                    if (seen.Add(player.UID))
                        list.Add(player.Owner);
                }
            }
            foreach (var c in Members())
                if (c.Player != null && seen.Add(c.Player.UID))
                    list.Add(c);
            return list;
        }

        private void SendRoom(ServerSockets.Packet packet)
        {
            foreach (var c in RoomViewers())
                c.Send(packet);
        }

        /// <summary>
        /// Refreshes the table (seats count, state, pot) for the players around it. Not for the table's own members:
        /// MsgTexasNpcInfo re-creates the table role on the client and would drop their own chair; they follow the
        /// chairs through MsgTexasInteractive and the game through the ShowHand packets.
        /// </summary>
        public void BroadcastTableInfo()
        {
            if (IsBoard)
                return;// no table role in the world
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var packet = stream.TableInfoCreate(this, SeatedList(), StageByte, Hand != null ? Hand.Pot : 0);
                var members = new HashSet<uint>();
                foreach (var c in Members())
                    if (c.Player != null)
                        members.Add(c.Player.UID);
                foreach (var c in RoomViewers())
                    if (c.Player != null && !members.Contains(c.Player.UID))
                        c.Send(packet);
            }
        }

        private long Pot { get { return Hand != null ? Hand.Pot : 0; } }

        /// <summary>Pot update for everybody around the table (MsgAction 234).</summary>
        private void SendPot(ServerSockets.Packet stream)
        {
            if (IsBoard)
                return;// MsgAction 234 needs the world table role
            SendRoom(stream.PotCreate(UID, Pot));
        }

        private ushort SecondsLeft(DateTime until)
        {
            double s = (until - DateTime.Now).TotalSeconds;
            if (s < 0) s = 0;
            return (ushort)Math.Ceiling(s);
        }

        // ---------------------------------------------------------------- seating
        public static bool Online(Client.GameClient client)
        {
            if (client == null || client.Player == null || client.Socket == null || !client.Socket.Alive)
                return false;
            if ((client.ClientFlag & Conquer6609.Client.ServerFlag.Disconnect) != 0)
                return false;
            Client.GameClient current;
            return Database.Server.GamePoll.TryGetValue(client.Player.UID, out current) && current == client;
        }

        /// <summary>
        /// The character is still the live one in GamePoll (not yet saved and dropped after a logout), so money given
        /// now is saved with it. Wider than Online(): a winner whose socket just died is still paid.
        /// </summary>
        private static bool Payable(Client.GameClient client)
        {
            if (client == null || client.Player == null)
                return false;
            Client.GameClient current;
            return Database.Server.GamePoll.TryGetValue(client.Player.UID, out current) && current == client;
        }

        public bool NearTable(Client.GameClient client)
        {
            return client.Player.Map == MapID && client.Player.DynamicID == 0
                && Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.RoleView.ViewThreshold;
        }

        /// <summary>Sits the player down (from the world chair or the table interface). Returns false with a message when refused.</summary>
        public bool Sit(Client.GameClient client, int seat)
        {
            uint uid = client.Player.UID;
            if (seat < 0 || seat >= MaxSeats || seat >= MaxSeatsLimit)
                return false;
            var existing = FindMember(uid);
            if (existing != null && existing.Join != PokerJoinType.Watch)
                return false;// already seated here
            if (Seats[seat] != null)
            {
                client.SendSysMesage("This seat has been taken.");
                return false;
            }
            if (!NearTable(client))
                return false;
            if (Balance(client) < MinMoney)
            {
                client.SendSysMesage("You need a minimum " + MinMoney.ToString("N0") + " " + CurrencyName + " to play the game.");
                return false;
            }
            if (existing != null)
                Watchers.Remove(uid);

            var info = new PokerSeatInfo
            {
                Client = client,
                UID = uid,
                Seat = (byte)seat,
                Join = HandRunning ? PokerJoinType.PreJoin : PokerJoinType.Playing
            };
            Seats[seat] = info;
            client.Player.PokerTableID = UID;
            client.Player.PokerSeat = (byte)seat;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SendRoom(stream.TableInteractiveCreate(PokerPackets.InteractiveType.Sit, UID, uid, (byte)seat));

                // the hero first (opens the interface for this table), then everybody already there
                client.Send(stream.EnterCreate(false, info.Join, info.Seat, TableId, uid, Pot));
                foreach (var s in Seats)
                {
                    if (s == null || s == info)
                        continue;
                    client.Send(stream.EnterCreate(false, s.Join, s.Seat, TableId, s.UID, Pot));
                }
                var others = stream.EnterCreate(false, info.Join, info.Seat, TableId, uid, Pot);
                foreach (var c in Members().ToArray())
                    if (c != client)
                        c.Send(others);
                if (HandRunning)
                    SendLostInfo(client, stream);
            }
            BroadcastTableInfo();
            if (phase == Phase.Idle)
                TryScheduleStart();
            return true;
        }

        /// <summary>A seated player that is not in the current hand becomes a watcher (client "watch" from the pre-join state).</summary>
        public void ToWatcher(Client.GameClient client)
        {
            uint uid = client.Player.UID;
            PokerSeatInfo info = null;
            foreach (var s in Seats)
                if (s != null && s.UID == uid)
                    info = s;
            if (info == null)
                return;
            if (HandRunning && Hand.Get(uid) != null && !Hand.Get(uid).Folded)
                return;// still playing the hand
            Seats[info.Seat] = null;
            info.Join = PokerJoinType.Watch;
            Watchers[uid] = info;
            client.Player.PokerTableID = 0;
            client.Player.PokerSeat = 0;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SendRoom(stream.TableInteractiveCreate(PokerPackets.InteractiveType.Stand, UID, uid, info.Seat));
                SendMembers(stream.EnterCreate(false, PokerJoinType.Watch, info.Seat, TableId, uid, Pot));
            }
            BroadcastTableInfo();
            if (phase == Phase.Starting && EligibleSeats().Count < 2)
                phase = Phase.Idle;
        }

        /// <summary>Player leaves the table (Exit, disconnect, walked away, not enough money). Folds him if he is in the hand.</summary>
        public void Remove(uint uid, Client.GameClient client, string reason)
        {
            PokerSeatInfo info = null;
            foreach (var s in Seats)
                if (s != null && s.UID == uid)
                    info = s;
            bool watcher = false;
            if (info == null)
            {
                if (Watchers.TryGetValue(uid, out info))
                    watcher = true;
                else
                    return;
            }
            if (client == null)
                client = info.Client;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!watcher && HandRunning)
                {
                    var hp = Hand.Get(uid);
                    if (hp != null && !hp.Folded)
                    {
                        int activeBefore = Hand.ActiveIndex;
                        Hand.ForceFold(uid);
                        SendMembers(stream.CallActionCreate(PokerAction.Fold, 0, hp.TotalBet, uid));
                        if (phase == Phase.Betting && (Hand.ActiveIndex != activeBefore || Hand.IsRoundComplete()))
                            AfterAction(stream);
                    }
                }

                var exit = stream.ExitCreate(TableId, uid);
                foreach (var c in Members().ToArray())
                    c.Send(exit);
                if (watcher)
                    Watchers.Remove(uid);
                else
                {
                    Seats[info.Seat] = null;
                    var viewers = RoomViewers();
                    var stand = stream.TableInteractiveCreate(PokerPackets.InteractiveType.Stand, UID, uid, info.Seat);
                    foreach (var c in viewers)
                        c.Send(stand);
                    // the leaving player gets the stand-up even when he is already out of view (only once)
                    if (client != null && client.Player != null && !viewers.Contains(client))
                        client.Send(stand);
                }
            }
            if (client != null && client.Player != null && client.Player.PokerTableID == UID)
            {
                client.Player.PokerTableID = 0;
                client.Player.PokerSeat = 0;
            }
            PokerManager.ForgetPlayer(uid, this);
            if (!string.IsNullOrEmpty(reason) && client != null && Online(client))
                client.SendSysMesage(reason);
            if (!watcher)
            {
                BroadcastTableInfo();
                if (phase == Phase.Starting && EligibleSeats().Count < 2)
                    phase = Phase.Idle;
            }
        }

        public void SetTrustee(Client.GameClient client, bool on)
        {
            var info = FindMember(client.Player.UID);
            if (info == null || info.Join == PokerJoinType.Watch)
                return;
            SetSeatTrustee(info, on, false);
        }

        private void SetSeatTrustee(PokerSeatInfo info, bool on, bool offline)
        {
            info.Trustee = on;
            info.OfflineTrustee = on && offline;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SendMembers(stream.TrusteeshipCreate(on, info.UID));
            }
            if (on && phase == Phase.Betting && Hand != null && Hand.Active != null && Hand.Active.UID == info.UID)
            {
                var soon = DateTime.Now.AddMilliseconds(TrusteeDelayMs);
                if (soon < actionDeadline)
                    actionDeadline = soon;
            }
        }

        // ---------------------------------------------------------------- championship board tables
        public PokerSeatInfo SeatOf(uint uid)
        {
            foreach (var s in Seats)
                if (s != null && s.UID == uid)
                    return s;
            return null;
        }

        public int FreeSeat()
        {
            for (int i = 0; i < MaxSeats && i < MaxSeatsLimit; i++)
                if (Seats[i] == null)
                    return i;
            return -1;
        }

        /// <summary>Chips each seated player has (3406 MsgTexasExTableChips).</summary>
        public List<KeyValuePair<uint, long>> Stacks()
        {
            var list = new List<KeyValuePair<uint, long>>();
            foreach (var s in Seats)
                if (s != null)
                    list.Add(new KeyValuePair<uint, long>(s.UID, Bank != null ? Bank.Balance(s.UID) : Balance(s.Client)));
            return list;
        }

        public void SendChips(ServerSockets.Packet stream)
        {
            var packet = stream.BoardChipsCreate(Stacks());
            foreach (var c in Members().ToArray())
                c.Send(packet);
        }

        /// <summary>
        /// Board table: seats a championship player (client may be offline = stale object, he plays on auto).
        /// Members get his look (MsgPlayer with the board flag) and his 2090; the hero's own view is sent by SendBoardView.
        /// </summary>
        public bool BoardSeat(Client.GameClient client, uint uid, int seat)
        {
            if (!IsBoard || seat < 0 || seat >= MaxSeatsLimit || Seats[seat] != null || SeatOf(uid) != null)
                return false;
            var info = new PokerSeatInfo
            {
                Client = client,
                UID = uid,
                Seat = (byte)seat,
                Join = HandRunning ? PokerJoinType.PreJoin : PokerJoinType.Playing
            };
            Seats[seat] = info;
            if (!Online(client))
            {
                info.Trustee = true;
                info.OfflineTrustee = true;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var c in Members().ToArray())
                {
                    if (c == client)
                        continue;
                    SendBoardLook(c, client, stream);
                    c.Send(stream.EnterCreate(false, info.Join, info.Seat, TableId, uid, ChipsOf(uid, client)));
                    if (info.Trustee)
                        c.Send(stream.TrusteeshipCreate(true, uid));
                }
            }
            if (phase == Phase.Idle)
                TryScheduleStart();
            return true;
        }

        /// <summary>MsgPlayer 10014 of <paramref name="who"/> with byte 136 = 2: fills the board's role cache (names/looks of the seats).</summary>
        private static unsafe void SendBoardLook(Client.GameClient to, Client.GameClient who, ServerSockets.Packet stream)
        {
            if (to == null || who == null || who.Player == null || to == who)
                return;
            try
            {
                var packet = who.Player.GetArray(stream, false);
                if (packet.Size > BoardLookOffset)
                    packet.Memory[BoardLookOffset] = 2;
                to.Send(packet);
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }

        /// <summary>Offset of the "windows view" byte of MsgPlayer 10014 (client reads 2 = board role, spec 3.2).</summary>
        public static int BoardLookOffset { get { return Framework.Settings.Int("Texas", "BoardLookOffset", 136); } }

        /// <summary>
        /// Everything the hero needs to see his board table: his 2090 first (sets "hero on board"), then the look and the
        /// 2090 of every other player, the chips (3406), auto-play marks, and the hand in progress (2098).
        /// </summary>
        public void SendBoardView(Client.GameClient client)
        {
            if (!Online(client))
                return;
            var me = SeatOf(client.Player.UID);
            if (me == null)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Send(stream.EnterCreate(false, me.Join, me.Seat, TableId, me.UID, ChipsOf(me.UID, client)));
                foreach (var s in Seats)
                {
                    if (s == null || s == me)
                        continue;
                    SendBoardLook(client, s.Client, stream);
                    client.Send(stream.EnterCreate(false, s.Join, s.Seat, TableId, s.UID, Bank != null ? Bank.Balance(s.UID) : 0));
                }
                client.Send(stream.BoardChipsCreate(Stacks()));
                foreach (var s in Seats)
                    if (s != null && s.Trustee && s != me)
                        client.Send(stream.TrusteeshipCreate(true, s.UID));
                if (HandRunning)
                    SendLostInfo(client, stream);
            }
        }

        /// <summary>The player logged in again: his seat follows the new client object, auto play off.</summary>
        public void BoardRebind(uint uid, Client.GameClient client)
        {
            var s = SeatOf(uid);
            if (s == null)
                return;
            s.Client = client;
            if (handClients.ContainsKey(uid))
                handClients[uid] = client;
            if (s.Trustee)
                SetSeatTrustee(s, false, false);
        }

        /// <summary>
        /// Board table: takes the player off the table (eliminated, moved to another table, left). A hand in progress is
        /// folded for him (his bets stay in the pot). Everybody at the table, him included, gets the 2096 exit.
        /// Returns the chips he still has in the bank (not touched).
        /// </summary>
        public bool BoardRemove(uint uid)
        {
            var info = SeatOf(uid);
            if (info == null)
                return false;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (HandRunning)
                {
                    var hp = Hand.Get(uid);
                    if (hp != null && !hp.Folded)
                    {
                        int activeBefore = Hand.ActiveIndex;
                        Hand.ForceFold(uid);
                        SendMembers(stream.CallActionCreate(PokerAction.Fold, 0, hp.TotalBet, uid));
                        if (phase == Phase.Betting && (Hand.ActiveIndex != activeBefore || Hand.IsRoundComplete()))
                            AfterAction(stream);
                    }
                }
                var exit = stream.ExitCreate(TableId, uid);
                foreach (var c in Members().ToArray())
                    c.Send(exit);
                Seats[info.Seat] = null;
            }
            PokerManager.ForgetPlayer(uid, this);
            if (phase == Phase.Starting && EligibleSeats().Count < 2)
                phase = Phase.Idle;
            return true;
        }

        /// <summary>Board table: auto play for a player who is not looking at the board (offline / did not enter);
        /// cleared by BoardRebind when he comes back.</summary>
        public void BoardSetAuto(uint uid, bool on)
        {
            var s = SeatOf(uid);
            if (s != null && s.Trustee != on)
                SetSeatTrustee(s, on, true);
        }

        /// <summary>Owner: new hands may start (after eliminations / moves / the blind level were applied).</summary>
        public void Release()
        {
            Hold = false;
            if (phase == Phase.Idle)
                TryScheduleStart();
        }

        public bool IsIdle { get { return phase == Phase.Idle; } }

        // ---------------------------------------------------------------- hand flow
        private List<PokerSeatInfo> EligibleSeats()
        {
            var list = new List<PokerSeatInfo>();
            foreach (var s in Seats)
            {
                if (s == null || s.Join == PokerJoinType.Watch || s.Kicked)
                    continue;
                if (IsBoard)
                {
                    // championship: offline players are dealt in (auto play) and a short stack plays all-in
                    if (ChipsOf(s.UID, s.Client) > 0)
                        list.Add(s);
                }
                else if (Online(s.Client) && Balance(s.Client) >= BigBlind)
                    list.Add(s);
            }
            return list;
        }

        private void TryScheduleStart()
        {
            if (Hold)
                return;
            if (EligibleSeats().Count >= 2)
            {
                phase = Phase.Starting;
                deadline = DateTime.Now.AddSeconds(StartDelaySeconds);
            }
        }

        private void StartHand(ServerSockets.Packet stream)
        {
            // players who cannot pay the big blind leave the table
            foreach (var s in Seats.ToArray())
            {
                if (s == null || s.Join == PokerJoinType.Watch || IsBoard)
                    continue;
                if (Online(s.Client) && Balance(s.Client) < BigBlind)
                    Remove(s.UID, s.Client, MoneyType == 1 ? "You are out of the game for insufficient CPs." : "You don`t have enough silver to continue.");
            }
            var seats = EligibleSeats();
            if (seats.Count < 2)
            {
                phase = Phase.Idle;
                return;
            }
            seats = seats.OrderBy(s => s.Seat).ToList();
            // button moves to the next player clockwise
            int dealerIdx = 0;
            for (int i = 0; i < seats.Count; i++)
            {
                if (seats[i].Seat > dealerSeat)
                {
                    dealerIdx = i;
                    break;
                }
            }
            dealerSeat = seats[dealerIdx].Seat;

            var players = new List<PokerHandPlayer>();
            handClients.Clear();
            foreach (var s in seats)
            {
                long stack = ChipsOf(s.UID, s.Client);
                if (MaxStake > 0 && stack > MaxStake)
                    stack = MaxStake;
                players.Add(new PokerHandPlayer { UID = s.UID, Seat = s.Seat, Stack = stack });
                handClients[s.UID] = s.Client;
            }
            LastHandStartStacks.Clear();
            foreach (var p in players)
                LastHandStartStacks[p.UID] = p.Stack;
            Hand = new PokerHand(players, dealerIdx, BigBlind, NoLimit, null, IsBoard ? Ante : 0);
            long sbPosted, bbPosted;
            Hand.Start(out sbPosted, out bbPosted);
            foreach (var p in Hand.Players)
            {
                if (p.TotalBet > 0 && !TakeChips(p.UID, handClients[p.UID], p.TotalBet))
                {
                    // money vanished between the check and now: his blind is void and he sits this hand out
                    p.Stack += p.TotalBet;
                    p.TotalBet = 0;
                    p.RoundBet = 0;
                    p.AllIn = false;
                    Hand.ForceFold(p.UID);
                }
            }

            // everyone dealt in plays now (pre-joined players become players)
            foreach (var s in seats)
            {
                if (s.Join != PokerJoinType.Playing)
                {
                    s.Join = PokerJoinType.Playing;
                    SendMembers(stream.EnterCreate(false, PokerJoinType.Playing, s.Seat, TableId, s.UID, Pot));
                }
            }

            uint dealer = Hand.Players[Hand.DealerIndex].UID;
            uint sb = Hand.Players[Hand.SmallBlindIndex].UID;
            uint bb = Hand.Players[Hand.BigBlindIndex].UID;
            var entries = Hand.Players.Select(p => new PokerPackets.DealtEntry { UID = p.UID }).ToList();
            var none = new[] { new PokerCard(0, 0), new PokerCard(0, 0) };
            foreach (var c in Members().ToArray())
            {
                var hp = c.Player != null ? Hand.Get(c.Player.UID) : null;
                var cards = hp != null ? new[] { hp.Card1, hp.Card2 } : none;
                c.Send(stream.DealtCardCreate(0, PokerPackets.DealType.PreFlop, cards, entries, dealer, sb, bb));
            }
            if (IsBoard)
                SendChips(stream);// the client posts the blinds itself but not the antes: send the real stacks
            BroadcastTableInfo();
            SendPot(stream);
            AfterAction(stream);
        }

        /// <summary>After any change of the hand: next player to act, next street, or the end of the hand.</summary>
        private void AfterAction(ServerSockets.Packet stream)
        {
            if (Hand == null || Hand.Settled)
                return;
            if (Hand.IsRoundComplete())
            {
                if (Hand.NotFoldedCount <= 1 || Hand.Stage == PokerStage.River)
                {
                    FinishHand(stream);
                    return;
                }
                phase = Phase.Street;
                deadline = DateTime.Now.AddMilliseconds(StreetDelayMs);
                return;
            }
            var active = Hand.Active;
            if (active == null)
            {
                phase = Phase.Street;
                deadline = DateTime.Now.AddMilliseconds(StreetDelayMs);
                return;
            }
            SyncStack(active);
            if (!active.CanAct)
            {
                // his silver/CPs are gone (spent elsewhere): he is all-in with what he already bet
                Hand.Refresh();
                AfterAction(stream);
                return;
            }
            phase = Phase.Betting;
            var info = FindMember(active.UID);
            actionDeadline = DateTime.Now.AddSeconds(ActionSeconds);
            SendActivePlayer(stream, active);
            if (info != null && info.Trustee)
                actionDeadline = DateTime.Now.AddMilliseconds(TrusteeDelayMs);
        }

        /// <summary>The player's real silver/CPs may only shrink his stack (never grow it during a hand).</summary>
        private void SyncStack(PokerHandPlayer p)
        {
            Client.GameClient c;
            if (!handClients.TryGetValue(p.UID, out c) || p.Folded || p.AllIn)
                return;
            long bal = ChipsOf(p.UID, c);
            if (bal < p.Stack)
            {
                p.Stack = Math.Max(0, bal);
                if (p.Stack == 0)
                {
                    if (p.TotalBet == 0)
                        Hand.ForceFold(p.UID);// nothing in the pot and nothing left: out of the hand
                    else
                        p.AllIn = true;
                }
            }
        }

        private void SendActivePlayer(ServerSockets.Packet stream, PokerHandPlayer active)
        {
            var allowed = Hand.AllowedActions(active);
            long min, max;
            if (NoLimit)
            {
                min = Hand.MinRaiseTo(active);
                max = Hand.MaxRaiseTo(active);
            }
            else
            {
                min = Hand.ToCall(active);
                max = Hand.MaxRaiseTo(active);
            }
            SendMembers(stream.ActivePlayerCreate(SecondsLeft(actionDeadline), allowed, min, max, active.UID));
        }

        private void NextStreet(ServerSockets.Packet stream)
        {
            var cards = Hand.NextStreet();
            PokerPackets.DealType type = Hand.Stage == PokerStage.Flop ? PokerPackets.DealType.Flop
                : Hand.Stage == PokerStage.Turn ? PokerPackets.DealType.Turn : PokerPackets.DealType.River;
            SendMembers(stream.DealtCardCreate(0, type, cards, null, 0, 0, 0));
            BroadcastTableInfo();
            AfterAction(stream);
        }

        private void FinishHand(ServerSockets.Packet stream)
        {
            bool showdown = Hand.NotFoldedCount > 1;
            if (showdown)
            {
                while (Hand.Board.Count < 5)
                {
                    // should not happen (the table runs the board out street by street), but never settle without a full board
                    var cards = Hand.NextStreet();
                    PokerPackets.DealType type = Hand.Stage == PokerStage.Flop ? PokerPackets.DealType.Flop
                        : Hand.Stage == PokerStage.Turn ? PokerPackets.DealType.Turn : PokerPackets.DealType.River;
                    SendMembers(stream.DealtCardCreate(0, type, cards, null, 0, 0, 0));
                }
            }
            Hand.Settle();
            ushort seconds = (ushort)ResultSeconds;
            if (showdown)
            {
                var lay = Hand.Players.Where(p => !p.Folded).Select(p => new PokerPackets.LayEntry { Card1 = p.Card1, Card2 = p.Card2, UID = p.UID }).ToList();
                SendMembers(stream.LayCardCreate(seconds, lay));
            }

            // pay the pots
            foreach (var p in Hand.Players)
            {
                if (p.Won <= 0)
                    continue;
                Client.GameClient c;
                if (handClients.TryGetValue(p.UID, out c) && Bank != null)
                    GiveChips(p.UID, c, p.Won);// tournament chips: kept by UID, also for an offline player
                else if (c != null && Payable(c))
                {
                    Credit(c, p.Won);
                    if (p.Net > 0 && p.Net >= BigBlind * 1000)
                        Security.Guard.Audit("poker-win", c.Player.UID, c.Player.Name, "table " + (TableId + 1) + " won " + p.Net + " " + CurrencyName);
                }
                else
                {
                    // character already saved and dropped: should not happen (TexasEntrance.OnLogout folds a leaving player first)
                    Security.Guard.Audit("poker-lost-pay", p.UID, "", "table " + (TableId + 1) + " " + p.Won + " " + CurrencyName + " not paid (player offline)");
                }
            }

            var result = new List<PokerPackets.ResultEntry>();
            foreach (var p in Hand.Players)
            {
                var seat = FindMember(p.UID);
                Client.GameClient c;
                handClients.TryGetValue(p.UID, out c);
                bool kicked = seat != null && seat.Join != PokerJoinType.Watch && (IsBoard ? ChipsOf(p.UID, c) <= 0 : c != null && Balance(c) < BigBlind);
                if (kicked && !IsBoard)
                    seat.Kicked = true;// board tables: the owner eliminates him after the result
                result.Add(new PokerPackets.ResultEntry
                {
                    Kicked = kicked,
                    Category = (showdown && !p.Folded && p.Score >= 0) ? PokerHandEvaluator.Category(p.Score) : PokerHandCategory.None,
                    UID = p.UID,
                    Net = p.Net
                });
            }
            SendMembers(stream.GameResultCreate(seconds, result));
            phase = Phase.Result;
            deadline = DateTime.Now.AddSeconds(ResultSeconds);
            BroadcastTableInfo();
        }

        private void EndResult()
        {
            if (IsBoard)
            {
                Hand = null;
                handClients.Clear();
                phase = Phase.Idle;
                Hold = true;
                using (var rec = new ServerSockets.RecycledPacket())
                    SendChips(rec.GetStream());
                Owner.OnHandFinished(this);
                return;
            }
            foreach (var s in Seats.ToArray())
            {
                if (s != null && s.Kicked)
                {
                    s.Kicked = false;
                    Remove(s.UID, s.Client, MoneyType == 1 ? "You are out of the game for insufficient CPs." : "You don`t have enough silver to continue.");
                }
            }
            Hand = null;
            handClients.Clear();
            phase = Phase.Idle;
            BroadcastTableInfo();
            using (var rec = new ServerSockets.RecycledPacket())
                SendPot(rec.GetStream());
            if (EligibleSeats().Count >= 2)
            {
                phase = Phase.Starting;
                deadline = DateTime.Now.AddSeconds(1);
            }
        }

        /// <summary>Client action (MsgShowHandCallAction).</summary>
        public void OnAction(Client.GameClient client, PokerAction action, long amount)
        {
            if (Hand == null || Hand.Settled || phase != Phase.Betting)
                return;
            var active = Hand.Active;
            if (active == null || active.UID != client.Player.UID)
                return;
            DoAction(active, action, amount, false);
        }

        private void DoAction(PokerHandPlayer active, PokerAction action, long amount, bool auto)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SyncStack(active);
                if (!active.CanAct)
                {
                    Hand.Refresh();
                    AfterAction(stream);
                    return;
                }
                var r = Hand.Act(active.UID, action, amount);
                if (!r.Valid)
                {
                    if (auto)
                        r = Hand.Act(active.UID, PokerAction.Fold, 0);
                    else
                    {
                        // refused (e.g. check while facing a bet): show the buttons again
                        SendActivePlayer(stream, active);
                        return;
                    }
                }
                if (r.Added > 0)
                {
                    Client.GameClient c;
                    if (!handClients.TryGetValue(active.UID, out c) || !TakeChips(active.UID, c, r.Added))
                    {
                        // cannot pay: undo the chips and fold him
                        active.Stack += r.Added;
                        active.RoundBet -= r.Added;
                        active.TotalBet -= r.Added;
                        active.AllIn = false;
                        if (!active.Folded)
                            Hand.ForceFold(active.UID);
                        r.Action = PokerAction.Fold;
                        r.Added = 0;
                    }
                }
                SendMembers(stream.CallActionCreate(r.Action, r.Added, active.TotalBet, active.UID));
                if (r.Added > 0)
                    SendPot(stream);
                AfterAction(stream);
            }
        }

        // ---------------------------------------------------------------- timer
        public void Tick(DateTime now)
        {
            // members that disconnected, left the map or walked away
            foreach (var s in Seats.ToArray())
            {
                if (s == null)
                    continue;
                if (IsBoard)
                {
                    // championship: an offline player keeps his seat and his chips, the server plays for him (check / fold)
                    if (!Online(s.Client) && !s.Trustee)
                        SetSeatTrustee(s, true, true);
                    continue;
                }
                if (!Online(s.Client))
                    Remove(s.UID, s.Client, null);
                else if (!NearTable(s.Client))
                    Remove(s.UID, s.Client, "You left the poker table.");
            }
            foreach (var w in Watchers.Values.ToArray())
            {
                if (!Online(w.Client) || !NearTable(w.Client))
                    Remove(w.UID, w.Client, null);
            }

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                switch (phase)
                {
                    case Phase.Idle:
                        TryScheduleStart();
                        break;
                    case Phase.Starting:
                        if (now >= deadline)
                            StartHand(stream);
                        break;
                    case Phase.Betting:
                        if (Hand != null && Hand.Active != null && now >= actionDeadline)
                        {
                            // time is up (or auto play): check if possible, otherwise fold
                            var a = Hand.Active;
                            DoAction(a, Hand.ToCall(a) == 0 ? PokerAction.Check : PokerAction.Fold, 0, true);
                        }
                        break;
                    case Phase.Street:
                        if (now >= deadline && Hand != null && !Hand.Settled)
                        {
                            if (Hand.NotFoldedCount <= 1 || (Hand.Stage == PokerStage.River && Hand.IsRoundComplete()))
                                FinishHand(stream);
                            else
                                NextStreet(stream);
                        }
                        break;
                    case Phase.Result:
                        if (now >= deadline)
                            EndResult();
                        break;
                }
            }
        }

        /// <summary>Hand state for a player joining while a hand runs (MsgShowHandLostInfo).</summary>
        private void SendLostInfo(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!HandRunning)
                return;
            var active = Hand.Active;
            byte round = (byte)Math.Max(0, (int)Hand.Stage - 1);
            var users = new List<PokerPackets.LostInfoEntry>();
            foreach (var p in Hand.Players)
            {
                if (p.Folded)
                    continue;
                bool mine = client.Player != null && p.UID == client.Player.UID;
                users.Add(new PokerPackets.LostInfoEntry
                {
                    CardCount = 2,
                    Card1 = mine ? p.Card1 : new PokerCard(0, 0),
                    Card2 = mine ? p.Card2 : new PokerCard(0, 0),
                    Shown1 = mine,
                    Shown2 = mine,
                    UID = p.UID
                });
            }
            client.Send(stream.LostInfoCreate(round, Hand.Board, phase == Phase.Betting ? SecondsLeft(actionDeadline) : (ushort)0,
                active != null ? Hand.AllowedActions(active) : PokerAction.None, Hand.CurrentBet, active != null ? active.Stack : 0,
                active != null ? active.UID : 0, Hand.Players[Hand.DealerIndex].UID, Hand.Players[Hand.SmallBlindIndex].UID,
                Hand.Players[Hand.BigBlindIndex].UID, users));
        }
    }
}
