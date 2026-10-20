// TexasEntrance module: the 6609 "CROSS-SERVER TOURNAMENTS" poker window (client CDlgCrossTexasEntrance,
// CDlgTexasMatchField = "INSTANT PLAY"). Protocol read from the client code:
//   3400 MsgTexasExInteractive (protobuf {1: action, 2: repeated uint32 params, 3: int64}), client -> server:
//     12 = login query (no answer)   0 = Instant Play opened / page changed -> answer 3401
//     1 [matchFieldId] = "Start Match-up" (client already checked min chips = 5 x big blind)
//     2 [page] = Tournaments page -> answer 3402 with field 1 = the same page (otherwise dropped)
//   3401 MsgTexasExMatchFieldList {1: repeated {1: matchFieldId, 2: players}}; ids from ini\texas_match_type.dat
//     (Silver No Limit quick-match fields, one id per big blind).
// On a normal (not cross) server the client plays at the in-world tables through the old messages
// (2090-2099, 2171/2172) that Game\MsgServer\Poker already implements, so Match-up walks the player to a table of
// that big blind with a free chair and seats him there (3400 action 4 would open the cross-server board instead).
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Framework.Modules;
using Conquer6609.Game.MsgServer.Poker;

namespace Conquer6609.Modules.TexasEntrance
{
    public sealed class TexasEntranceModule : ModuleBase
    {
        public override string Name { get { return "TexasEntrance"; } }

        static TexasEntranceModule instance;
        public const ushort MsgTexasExMatchFieldList = 3401;

        /// <summary>client texas_match_type.dat quick-match ids (Silver, No Limit) by big blind.</summary>
        static readonly KeyValuePair<uint, long>[] Fields =
        {
            new KeyValuePair<uint, long>(1171, 10000),
            new KeyValuePair<uint, long>(1051, 100000),
            new KeyValuePair<uint, long>(1008, 1000000),
            new KeyValuePair<uint, long>(1015, 5000000),
            new KeyValuePair<uint, long>(1001, 10000000),
        };

        sealed class PendingSeat { public Client.GameClient Client; public uint TableUid; public byte Seat; public DateTime Due; }
        static readonly ConcurrentQueue<PendingSeat> pending = new ConcurrentQueue<PendingSeat>();

        public override void OnLoad()
        {
            instance = this;
            Championship.Init(this);
        }

        public override void OnLogin(Client.GameClient client)
        {
            try { Championship.OnLogin(client); }
            catch (Exception e) { WriteLog("login: " + e.Message); }
        }

        /// <summary>
        /// A player who logs out at a poker table leaves it right away (his hand is folded, the pot goes on), BEFORE his
        /// character is saved: the poker ticker only notices the dead socket later, and a pot won by a player whose
        /// character was already saved would be lost.
        /// </summary>
        public override void OnLogout(Client.GameClient client)
        {
            try
            {
                if (client != null && client.Player != null && (PokerManager.IsAtTable(client) || client.Player.PokerTableID != 0))
                    PokerManager.OnLogout(client);
                Championship.OnLogout(client);
            }
            catch (Exception e) { WriteLog("logout: " + e.Message); }
        }

        public override void OnTick(Extensions.Time32 clock)
        {
            // seat players walked to a table on the previous tick (the client must see the table spawn first)
            int n = pending.Count;
            var now = DateTime.Now;
            while (n-- > 0)
            {
                PendingSeat p;
                if (!pending.TryDequeue(out p))
                    break;
                if (p.Due > now)
                {
                    pending.Enqueue(p);
                    continue;
                }
                try
                {
                    if (p.Client != null && p.Client.Player != null && p.Client.FullLoading && !PokerManager.IsAtTable(p.Client))
                    {
                        // the chair picked at Match-up may have been taken meanwhile: take another free one
                        byte seat = p.Seat;
                        PokerTable table;
                        if (PokerManager.TablesByUID.TryGetValue(p.TableUid, out table))
                        {
                            lock (table.SyncRoot)
                            {
                                if (table.Seats[seat] != null)
                                {
                                    for (int i = 0; i < table.MaxSeats && i < PokerTable.MaxSeatsLimit; i++)
                                        if (table.Seats[i] == null) { seat = (byte)i; break; }
                                }
                            }
                        }
                        PokerManager.OnInteractive(p.Client, PokerPackets.InteractiveType.Sit, p.TableUid, seat);
                    }
                }
                catch (Exception e) { WriteLog("seat: " + e.Message); }
            }
        }

        static IEnumerable<PokerTable> TablesFor(long bigBlind)
        {
            return PokerManager.TablesByUID.Values.Where(t => t.MoneyType == 0 && t.NoLimit && t.BigBlind == bigBlind);
        }

        static int Seated(PokerTable t)
        {
            lock (t.SyncRoot)
                return t.Seats.Count(s => s != null);
        }

        [PacketAttribute(Game.GamePackets.MsgTexasExInteractive)]
        public static unsafe void Interactive(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null || !client.FullLoading)
                return;
            ulong action;
            List<ulong> args;
            Read(stream, out action, out args);
            if (Championship.OnAction(client, action, args))
                return;// championship actions 2, 3, 4, 5, 9, 12, 14, 22, 23
            switch (action)
            {
                case 0:// Instant Play opened or page changed
                    SendMatchFields(client);
                    break;
                case 1:// Start Match-up [field id]
                    if (args.Count > 0)
                        MatchUp(client, (uint)args[0]);
                    break;
                default:
                    WriteLog("3400 action " + action + " args " + string.Join(",", args) + " from " + client.Player.Name);
                    break;
            }
        }

        static void SendMatchFields(Client.GameClient client)
        {
            var body = new List<byte>();
            foreach (var f in Fields)
            {
                var tables = TablesFor(f.Value).ToList();
                if (tables.Count == 0)
                    continue;
                var entry = new List<byte>();
                Field(entry, 1, f.Key);
                Field(entry, 2, (ulong)tables.Sum(Seated));
                body.Add(0x0A);
                Varint(body, (ulong)entry.Count);
                body.AddRange(entry);
            }
            Send(client, MsgTexasExMatchFieldList, body);
        }

        static void MatchUp(Client.GameClient client, uint fieldId)
        {
            long bigBlind = Fields.Where(f => f.Key == fieldId).Select(f => f.Value).FirstOrDefault();
            if (bigBlind == 0)
                return;
            if (PokerManager.IsAtTable(client))
            {
                client.SendSysMesage("You are already at a poker table. Please leave it first.");
                return;
            }
            if (!CanLeave(client))
            {
                client.SendSysMesage("You cannot join a poker table from here.");
                return;
            }
            // a table of that blind with a free chair, the fullest first (players want company)
            PokerTable best = null;
            int bestSeat = -1, bestCount = -1;
            foreach (var t in TablesFor(bigBlind))
            {
                lock (t.SyncRoot)
                {
                    int free = -1, count = 0;
                    for (int i = 0; i < t.MaxSeats && i < PokerTable.MaxSeatsLimit; i++)
                    {
                        if (t.Seats[i] != null) count++;
                        else if (free < 0) free = i;
                    }
                    if (free >= 0 && count > bestCount)
                    {
                        best = t;
                        bestSeat = free;
                        bestCount = count;
                    }
                }
            }
            if (best == null)
            {
                client.SendSysMesage("All tables of this field are full. Please try again later.");
                return;
            }
            if (best.Balance(client) < best.MinMoney)
            {
                client.SendSysMesage("You need a minimum " + best.MinMoney.ToString("N0") + " " + best.CurrencyName + " to play the game.");
                return;
            }
            // next to the table (FindSpot keeps 10 cells away from every table, which can be out of the table's view range)
            ushort x, y;
            if (!PokerManager.FindStandSpot(best, out x, out y))
            {
                x = best.X;
                y = (ushort)(best.Y + 3);
            }
            client.Teleport(x, y, best.MapID);
            pending.Enqueue(new PendingSeat { Client = client, TableUid = best.UID, Seat = (byte)bestSeat, Due = DateTime.Now.AddMilliseconds(1200) });
        }

        static bool CanLeave(Client.GameClient client)
        {
            uint map = client.Player.Map;
            if (map == 5000 || map == 6000 || map == 6001 || map == 6003)
                return false;
            if (Program.BlockTeleportMap.Contains(map) && map != 1858 && map != 1860)
                return false;
            return client.Player.Alive && !client.InTrade && !client.IsVendor && client.Player.DynamicID == 0 && !client.IsConnectedInterServer();
        }

        #region protobuf helpers
        static void Varint(List<byte> b, ulong v)
        {
            while (v >= 0x80)
            {
                b.Add((byte)(v | 0x80));
                v >>= 7;
            }
            b.Add((byte)v);
        }

        static void Field(List<byte> b, int field, ulong value)
        {
            Varint(b, (ulong)(field << 3));
            Varint(b, value);
        }

        static void Send(Client.GameClient client, ushort type, List<byte> body)
        {
            if (body.Count > 1000)
                return;// the client buffer is 1024 bytes
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                foreach (var x in body)
                    stream.Write(x);
                stream.Finalize(type);
                client.Send(stream);
            }
        }

        /// <summary>Reads field 1 (action) and every field-2 value (plain or packed) of a 3400 body.</summary>
        static unsafe void Read(ServerSockets.Packet stream, out ulong action, out List<ulong> args)
        {
            action = 0;
            args = new List<ulong>();
            int end = stream.Size - ServerSockets.Packet.SealSize;
            int pos = 4;
            byte* p = stream.Memory;
            while (pos < end)
            {
                ulong key;
                if (!ReadVarint(p, ref pos, end, out key))
                    return;
                int field = (int)(key >> 3), wire = (int)(key & 7);
                if (wire == 0)
                {
                    ulong v;
                    if (!ReadVarint(p, ref pos, end, out v))
                        return;
                    if (field == 1) action = v;
                    else if (field == 2 && args.Count < 16) args.Add(v);
                }
                else if (wire == 2)
                {
                    ulong len;
                    if (!ReadVarint(p, ref pos, end, out len) || len > (ulong)(end - pos))
                        return;
                    int stop = pos + (int)len;
                    if (field == 2)
                    {
                        while (pos < stop && args.Count < 16)
                        {
                            ulong v;
                            if (!ReadVarint(p, ref pos, stop, out v))
                                break;
                            args.Add(v);
                        }
                    }
                    pos = stop;
                }
                else if (wire == 1) pos += 8;
                else if (wire == 5) pos += 4;
                else return;
            }
        }

        static unsafe bool ReadVarint(byte* p, ref int pos, int end, out ulong value)
        {
            value = 0;
            for (int shift = 0; shift < 64 && pos < end; shift += 7)
            {
                byte b = p[pos++];
                value |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    return true;
            }
            return false;
        }
        #endregion

        static void WriteLog(string line)
        {
            if (instance != null)
                instance.Log(line);
        }
    }
}
