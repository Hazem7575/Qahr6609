using System;
using System.Collections.Generic;

namespace Conquer6609.Game.MsgServer.Poker
{
    /// <summary>
    /// Texas Hold'em packets of the 6609 client (layouts read from Conquer.exe, CMsgShowHand* / CMsgTexas* classes).
    /// Offsets are from the start of the packet (0 = size, 2 = type). Server -> client builders.
    ///
    /// Two different table ids are used by the client:
    ///  - the table ROLE uid (MsgTexasNpcInfo +4, MsgTexasInteractive +8, player spawn PokerTableID): the map object;
    ///  - the table ID (MsgTexasNpcInfo +0x1A, MsgShowHandEnter +9, MsgShowHandExit +8, OnlineStatus +8) = shown table number - 1;
    ///    it is also the row (Data0) the client reads from its ini/ShowHandTable.dat (Data1 1 = no limit, Data2 1 = CPs,
    ///    Data3 big blind, MaxPlayer default 9), confirmed by the client data (rows 0..279, TexasTableIndex1..280).
    /// </summary>
    public static class PokerPackets
    {
        /// <summary>MsgTexasNpcInfo 2172: table spawn/refresh (re-sending replaces the table role on the client).</summary>
        public static ServerSockets.Packet TableInfoCreate(this ServerSockets.Packet stream, PokerTable table, IList<PokerSeatInfo> seated, byte state, long pot)
        {
            stream.InitWriter();
            stream.Write(table.UID);                 // 4
            stream.Write((uint)0);                   // 8
            stream.Write((uint)0);                   // 12
            stream.Write(table.X);                   // 16
            stream.Write(table.Y);                   // 18
            stream.Write((ushort)table.Mesh);        // 20 look (NpcType = mesh / 10, facing = mesh % 10)
            stream.Write(table.RoleType);            // 22 110 = Texas Hold'em table
            stream.Write((ushort)0);                 // 24
            stream.Write(table.TableId);             // 26 table number - 1 (TexasHoldEm.ini TexasTableIndex = TableId + 1)
            stream.Write((uint)(table.NoLimit ? 1 : 0));   // 30 0 = Fixed Limit, 1 = No Limit
            stream.Write((uint)table.MoneyType);     // 34 0 = Silver, 1 = CPs
            stream.Write((uint)Math.Min(table.BigBlind, int.MaxValue)); // 38 big blind / min bet
            stream.Write(state);                     // 42 0 Unopened,1 Pocket,2 Flop,3 Turn,4 River,5 Showdown
            stream.Write(pot);                       // 43 pot
            int count = Math.Min(seated.Count, 9);
            stream.Write((byte)count);               // 51
            for (int i = 0; i < count; i++)          // 52: uid, seat, state(1 = sitting)
            {
                stream.Write(seated[i].UID);
                stream.Write(seated[i].Seat);
                stream.Write((byte)1);
            }
            stream.Finalize(GamePackets.PokerTable);
            return stream;
        }

        public enum InteractiveType : ushort
        {
            /// <summary>Player sits on a chair of the table (hero: becomes seated).</summary>
            Sit = 0,
            /// <summary>Player leaves the chair (hero: stands up).</summary>
            Stand = 1,
            /// <summary>Chair reserved for a player (no role yet).</summary>
            Reserve = 2,
            /// <summary>Sit (role already on the chair).</summary>
            SitAgain = 3,
            /// <summary>Client request: take the selected chair (sent after the "Take the seat?" confirmation).</summary>
            RequestSeat = 4
        }

        /// <summary>MsgTexasInteractive 2171 (20 bytes): chair of a table in the world.</summary>
        public static ServerSockets.Packet TableInteractiveCreate(this ServerSockets.Packet stream, InteractiveType type, uint tableUID, uint playerUID, byte seat)
        {
            stream.InitWriter();
            stream.Write((ushort)type);   // 4
            stream.Write((ushort)0);      // 6
            stream.Write(tableUID);       // 8
            stream.Write(playerUID);      // 12
            stream.Write(seat);           // 16 server seat (client chair = seat + 2)
            stream.ZeroFill(3);           // 17..19
            stream.Finalize(GamePackets.PokerUpdateTableLocation);
            return stream;
        }

        /// <summary>MsgShowHandEnter 2090 (29 bytes): a player joins the table game (hero: opens the poker interface).</summary>
        public static ServerSockets.Packet EnterCreate(this ServerSockets.Packet stream, bool offline, PokerJoinType joinType, ushort seat, uint tableId, uint uid, long pot)
        {
            stream.InitWriter();
            stream.Write((byte)(offline ? 1 : 0));  // 4 player has no role on screen (offline)
            stream.Write((ushort)joinType);         // 5 0 none,1 playing,2 watching,3 joins next hand
            stream.Write(seat);                     // 7
            stream.Write(tableId);                  // 9
            stream.Write(uid);                      // 13
            stream.Write(pot);                      // 17 pot / chips (championship)
            stream.Write((uint)1);                  // 25 game mode: 1 = Texas Hold'em, 2 = ShowHand
            stream.Finalize(GamePackets.PokerPlayerInfo);
            return stream;
        }

        /// <summary>MsgShowHandExit 2096 (16 bytes): a player leaves the table game (hero: closes the poker interface).</summary>
        public static ServerSockets.Packet ExitCreate(this ServerSockets.Packet stream, uint tableId, uint uid)
        {
            stream.InitWriter();
            stream.Write((ushort)1);      // 4 type 1 = leave
            stream.Write((ushort)0);      // 6
            stream.Write(tableId);        // 8
            stream.Write(uid);            // 12
            stream.Finalize(GamePackets.PokerLeaveTable);
            return stream;
        }

        public enum DealType : ushort
        {
            PreFlop = 0,
            Flop = 1,
            Turn = 2,
            River = 3,
            Dealer = 4
        }

        public struct DealtEntry
        {
            public PokerCard Card;
            public uint UID;
        }

        /// <summary>
        /// MsgShowHandDealtCard 2091 (44 + 8n bytes). PreFlop: cards = the receiver's two hole cards (zeros for a
        /// watcher), entries = everybody dealt in (their card fields unused); dealer / small blind / big blind uids
        /// (the client posts the blinds itself from its table data). Flop/Turn/River: cards = new board cards.
        /// </summary>
        public static ServerSockets.Packet DealtCardCreate(this ServerSockets.Packet stream, ushort seconds, DealType type, IList<PokerCard> cards,
            IList<DealtEntry> entries, uint dealer, uint smallBlind, uint bigBlind)
        {
            stream.InitWriter();
            stream.Write(seconds);                      // 4
            stream.Write((ushort)type);                 // 6
            int n = cards == null ? 0 : Math.Min(cards.Count, 5);
            stream.Write((ushort)n);                    // 8
            for (int i = 0; i < 5; i++)                 // 10 values
                stream.Write((ushort)(i < n ? cards[i].Value : 0));
            for (int i = 0; i < 5; i++)                 // 20 suits
                stream.Write((ushort)(i < n ? cards[i].Suit : 0));
            int m = entries == null ? 0 : Math.Min(entries.Count, 9);
            stream.Write((ushort)m);                    // 30
            stream.Write(dealer);                       // 32
            stream.Write(smallBlind);                   // 36
            stream.Write(bigBlind);                     // 40
            for (int i = 0; i < m; i++)                 // 44: value, suit, uid
            {
                stream.Write((ushort)entries[i].Card.Value);
                stream.Write((ushort)entries[i].Card.Suit);
                stream.Write(entries[i].UID);
            }
            stream.Finalize(GamePackets.PokerDrawCards);
            return stream;
        }

        /// <summary>MsgShowHandActivePlayer 2092 (28 bytes): whose turn, allowed actions, raise range ("raise to" amounts), seconds left.</summary>
        public static ServerSockets.Packet ActivePlayerCreate(this ServerSockets.Packet stream, ushort seconds, PokerAction allowed, long min, long max, uint uid)
        {
            stream.InitWriter();
            stream.Write(seconds);          // 4
            stream.Write((ushort)allowed);  // 6
            stream.Write(min);              // 8  call amount (fixed limit) / smallest raise-to (no limit)
            stream.Write(max);              // 16 all-in raise-to
            stream.Write(uid);              // 24
            stream.Finalize(GamePackets.PokerPlayerTurn);
            return stream;
        }

        /// <summary>MsgShowHandCallAction 2093 (28 bytes): a player acted. amount = chips added now, total = chips in this hand.</summary>
        public static ServerSockets.Packet CallActionCreate(this ServerSockets.Packet stream, PokerAction action, long amount, long total, uint uid)
        {
            stream.InitWriter();
            stream.Write((ushort)0);        // 4 seconds (0 = stop the countdown)
            stream.Write((ushort)action);   // 6
            stream.Write(amount);           // 8
            stream.Write(total);            // 16
            stream.Write(uid);              // 24
            stream.Finalize(GamePackets.PokerHand);
            return stream;
        }

        public struct LayEntry
        {
            public PokerCard Card1, Card2;
            public uint UID;
        }

        /// <summary>MsgShowHandLayCard 2094 (8 + 12n bytes): showdown, hole cards of the players still in.</summary>
        public static ServerSockets.Packet LayCardCreate(this ServerSockets.Packet stream, ushort seconds, IList<LayEntry> entries)
        {
            stream.InitWriter();
            int n = Math.Min(entries.Count, 9);
            stream.Write(seconds);          // 4
            stream.Write((ushort)n);        // 6
            for (int i = 0; i < n; i++)     // 8: values[2], suits[2], uid
            {
                stream.Write((ushort)entries[i].Card1.Value);
                stream.Write((ushort)entries[i].Card2.Value);
                stream.Write((ushort)entries[i].Card1.Suit);
                stream.Write((ushort)entries[i].Card2.Suit);
                stream.Write(entries[i].UID);
            }
            stream.Finalize(GamePackets.PokerShowAllCards);
            return stream;
        }

        public struct ResultEntry
        {
            /// <summary>Player must leave the table (not enough silver/CPs to continue): client shows SH_INSUFFICIENT_CHIP_TIP.</summary>
            public bool Kicked;
            public PokerHandCategory Category;
            public uint UID;
            /// <summary>Net win (+) or loss (-) of this hand.</summary>
            public long Net;
        }

        /// <summary>MsgShowHandGameResult 2095 (8 + 15n bytes): end of the hand.</summary>
        public static ServerSockets.Packet GameResultCreate(this ServerSockets.Packet stream, ushort seconds, IList<ResultEntry> entries)
        {
            stream.InitWriter();
            int n = Math.Min(entries.Count, 9);
            stream.Write(seconds);          // 4
            stream.Write((ushort)n);        // 6
            for (int i = 0; i < n; i++)     // 8
            {
                stream.Write((byte)(entries[i].Kicked ? 1 : 0));
                stream.Write((ushort)entries[i].Category);
                stream.Write(entries[i].UID);
                stream.Write(entries[i].Net);
            }
            stream.Finalize(GamePackets.PokerRoundResult);
            return stream;
        }

        /// <summary>MsgShowHandOnlineStatus 2097 (16 bytes): 1 = player offline, 0 = online again.</summary>
        public static ServerSockets.Packet OnlineStatusCreate(this ServerSockets.Packet stream, bool offline, uint tableId, uint uid)
        {
            stream.InitWriter();
            stream.Write((byte)(offline ? 1 : 0)); // 4
            stream.ZeroFill(3);                    // 5
            stream.Write(tableId);                 // 8
            stream.Write(uid);                     // 12
            stream.Finalize(GamePackets.MsgShowHandOnlineStatus);
            return stream;
        }

        /// <summary>MsgShowHandTrusteeship 2099 (12 bytes): auto play on/off for a player.</summary>
        public static ServerSockets.Packet TrusteeshipCreate(this ServerSockets.Packet stream, bool on, uint uid)
        {
            stream.InitWriter();
            stream.Write((byte)(on ? 1 : 0));     // 4
            stream.ZeroFill(3);                   // 5
            stream.Write(uid);                    // 8
            stream.Finalize(GamePackets.MsgShowHandTrusteeship);
            return stream;
        }

        public struct LostInfoEntry
        {
            public byte CardCount;
            public PokerCard Card1, Card2;
            public bool Shown1, Shown2;
            public uint UID;
        }

        /// <summary>
        /// MsgShowHandLostInfo 2098 (45 + 11n bytes): full state of the hand in progress for a player that joins/watches
        /// mid-hand: round, board, dealer/blinds, players in the hand, active player and his countdown.
        /// The client shows the active player's raise range as [2 x currentBet, currentBet + stack].
        /// </summary>
        public static ServerSockets.Packet LostInfoCreate(this ServerSockets.Packet stream, byte round, IList<PokerCard> board, ushort seconds,
            PokerAction allowed, long currentBet, long activeStack, uint activeUid, uint dealer, uint smallBlind, uint bigBlind, IList<LostInfoEntry> users)
        {
            stream.InitWriter();
            int nb = board == null ? 0 : Math.Min(board.Count, 5);
            int nu = Math.Min(users.Count, 9);
            stream.Write(round);                                   // 4 0 preflop,1 flop,2 turn,3 river
            stream.Write((byte)nb);                                // 5
            stream.Write((byte)nu);                                // 6
            stream.Write(seconds);                                 // 7
            stream.Write((ushort)allowed);                         // 9
            stream.Write((uint)Math.Min(currentBet, uint.MaxValue)); // 11
            stream.Write((uint)Math.Min(activeStack, uint.MaxValue)); // 15
            stream.Write(activeUid);                               // 19
            stream.Write(dealer);                                  // 23
            stream.Write(smallBlind);                              // 27
            stream.Write(bigBlind);                                // 31
            for (int i = 0; i < 5; i++)                            // 35 board (suit, value)
            {
                stream.Write((byte)(i < nb ? board[i].Suit : 0));
                stream.Write((byte)(i < nb ? board[i].Value : 0));
            }
            for (int i = 0; i < nu; i++)                           // 45
            {
                var u = users[i];
                stream.Write(u.CardCount);
                stream.Write(u.Card1.Suit); stream.Write(u.Card1.Value); stream.Write((byte)(u.Shown1 ? 1 : 0));
                stream.Write(u.Card2.Suit); stream.Write(u.Card2.Value); stream.Write((byte)(u.Shown2 ? 1 : 0));
                stream.Write(u.UID);
            }
            stream.Finalize(GamePackets.MsgShowHandLostInfo);
            return stream;
        }

        /// <summary>
        /// MsgAction 234 (PoketTableBet): pot of a table (ObjId = table role uid, int64 at +12). Updates the table
        /// tooltip and, for players at that table, the pot shown in the poker interface.
        /// </summary>
        public static unsafe ServerSockets.Packet PotCreate(this ServerSockets.Packet stream, uint tableUID, long pot)
        {
            ActionQuery action = new ActionQuery();
            action.ObjId = tableUID;
            action.Type = ActionType.PoketTableBet;
            action.PokerTableBet = pot;
            return stream.ActionCreate(&action);
        }

        // ------------------------------------------------------------------ championship (board) messages, protobuf bodies
        // Framing of 3400-3406: u16 size | u16 type | protobuf body (uint32/int64 varints, not packed), at most 1024 bytes.

        public const ushort MsgTexasExInteractive = 3400;
        public const ushort MsgTexasExChampionshipList = 3402;
        public const ushort MsgTexasExRankList = 3404;
        public const ushort MsgTexasExMyRank = 3405;
        public const ushort MsgTexasExTableChips = 3406;

        /// <summary>Protobuf writer helpers (wire types 0 and 2 only).</summary>
        public static class Proto
        {
            public static void Varint(List<byte> b, ulong v)
            {
                while (v >= 0x80)
                {
                    b.Add((byte)(v | 0x80));
                    v >>= 7;
                }
                b.Add((byte)v);
            }

            public static void Field(List<byte> b, int field, ulong value)
            {
                Varint(b, (ulong)(field << 3));
                Varint(b, value);
            }

            /// <summary>int64 field (two's complement varint, like protobuf int64).</summary>
            public static void Int64(List<byte> b, int field, long value)
            {
                Field(b, field, unchecked((ulong)value));
            }

            public static void Bytes(List<byte> b, int field, byte[] data)
            {
                Varint(b, (ulong)(field << 3 | 2));
                Varint(b, (ulong)data.Length);
                b.AddRange(data);
            }

            public static void String(List<byte> b, int field, string s)
            {
                Bytes(b, field, global::Conquer6609.AnsiText.Value.GetBytes(s ?? ""));
            }

            public static void Message(List<byte> b, int field, List<byte> sub)
            {
                Varint(b, (ulong)(field << 3 | 2));
                Varint(b, (ulong)sub.Count);
                b.AddRange(sub);
            }
        }

        /// <summary>Most bytes of a protobuf body (the client buffer is 1024 bytes with the header).</summary>
        public const int MaxProtoBody = 1000;

        /// <summary>Any protobuf message: header + body (body is cut to MaxProtoBody by the callers, see MaxProtoBody).</summary>
        public static ServerSockets.Packet ProtoCreate(this ServerSockets.Packet stream, ushort type, List<byte> body)
        {
            stream.InitWriter();
            int n = Math.Min(body.Count, MaxProtoBody);
            for (int i = 0; i < n; i++)
                stream.Write(body[i]);
            stream.Finalize(type);
            return stream;
        }

        /// <summary>3400 MsgTexasExInteractive server -> client: {1: action, 2: repeated uint32 params}.</summary>
        public static ServerSockets.Packet TexasExActionCreate(this ServerSockets.Packet stream, uint action, params ulong[] args)
        {
            var b = new List<byte>();
            Proto.Field(b, 1, action);
            if (args != null)
                foreach (var a in args)
                    Proto.Field(b, 2, a);
            return stream.ProtoCreate(MsgTexasExInteractive, b);
        }

        /// <summary>3400 with the int64 field 3 (action 15 = hero chips on the board).</summary>
        public static ServerSockets.Packet TexasExValueCreate(this ServerSockets.Packet stream, uint action, long value)
        {
            var b = new List<byte>();
            Proto.Field(b, 1, action);
            Proto.Int64(b, 3, value);
            return stream.ProtoCreate(MsgTexasExInteractive, b);
        }

        /// <summary>3406 MsgTexasExTableChips: {1: repeated {1: uid, 2: int64 chips}} = stack of every seated player.</summary>
        public static ServerSockets.Packet BoardChipsCreate(this ServerSockets.Packet stream, IList<KeyValuePair<uint, long>> stacks)
        {
            var b = new List<byte>();
            foreach (var s in stacks)
            {
                var e = new List<byte>();
                Proto.Field(e, 1, s.Key);
                Proto.Int64(e, 2, s.Value);
                Proto.Message(b, 1, e);
            }
            return stream.ProtoCreate(MsgTexasExTableChips, b);
        }

        // ------------------------------------------------------------------ client -> server

        /// <summary>MsgTexasInteractive 2171 from the client: type 0 = sit on the chair reached, 4 = take the selected chair.</summary>
        [PacketAttribute(GamePackets.PokerUpdateTableLocation)]
        private static void HandleInteractive(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null)
                return;
            stream.Seek(4);
            ushort type = stream.ReadUInt16();
            stream.Seek(8);
            uint tableUid = stream.ReadUInt32();
            stream.Seek(16);
            byte seat = stream.ReadUInt8();
            PokerManager.OnInteractive(client, (InteractiveType)type, tableUid, seat);
        }

        /// <summary>MsgShowHandEnter 2090 from the client: join type 1 = sit at seat (from the table interface), 2 = watch.</summary>
        [PacketAttribute(GamePackets.PokerPlayerInfo)]
        private static void HandleEnter(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null)
                return;
            stream.Seek(5);
            ushort joinType = stream.ReadUInt16();
            ushort seat = stream.ReadUInt16();
            uint tableId = stream.ReadUInt32();
            PokerManager.OnEnter(client, (PokerJoinType)joinType, seat, tableId);
        }

        /// <summary>MsgShowHandExit 2096 from the client: leave the table.</summary>
        [PacketAttribute(GamePackets.PokerLeaveTable)]
        private static void HandleExit(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null)
                return;
            PokerManager.OnExit(client);
        }

        /// <summary>MsgShowHandCallAction 2093 from the client: action (+6) and, for no limit bet/raise, the raise-to amount (+8).</summary>
        [PacketAttribute(GamePackets.PokerHand)]
        private static void HandleCallAction(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null)
                return;
            stream.Seek(6);
            ushort action = stream.ReadUInt16();
            long amount = stream.ReadInt64();
            PokerManager.OnCallAction(client, (PokerAction)action, amount);
        }

        /// <summary>MsgShowHandTrusteeship 2099 from the client: auto play on (1) / off (0).</summary>
        [PacketAttribute(GamePackets.MsgShowHandTrusteeship)]
        private static void HandleTrusteeship(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null)
                return;
            stream.Seek(4);
            byte on = stream.ReadUInt8();
            PokerManager.OnTrusteeship(client, on != 0);
        }
    }
}
