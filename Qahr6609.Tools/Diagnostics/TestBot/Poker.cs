// TestBot poker mode: plays Texas Hold'em hands against the server (Qahr6609.GameServer\Game\MsgServer\Poker) like the 6609 client.
//   --mode poker     one bot: Instant Play "Start Match-up" (3400 action 1 [field]), answers its turns (2092 -> 2093),
//                    plays --hands N hands, leaves (2096), checks its silver against the hand results.
//   --mode pokerduo  two bots in one process (one AppDomain each): account "1"/"1" (--user1/--pass1) and the account of
//                    testaccount.ini (--user2/--pass2), same field, then the money of both is summed (no rake: must be 0).
// Every poker packet is logged with its size, checked against the PokerPackets.cs layouts.
// C# 5 (framework csc): no string interpolation.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestBot
{
    partial class Program
    {
        static Action<Rx> PacketHook;

        // ------------------------------------------------------------------ state (rx thread + main thread)
        static long PkMoney = -1;            // silver, from 1006 and 10017 Update (type 4)
        static uint PkTableUid, PkTableId = uint.MaxValue;
        static volatile bool PkSeated;
        static int PkHandsStarted, PkHandsDone, PkMyTurns, PkActionsSent, PkRePrompts, PkSizeErrors, PkFlowErrors;
        static long PkNetSum;                // sum of my Net in 2095
        static DateTime PkLastEvent = DateTime.Now;
        static Random PkRnd;
        static int PkActiveSeq, PkSkipSeq = -1;              // increments on every 2092 for me (a stale reply is not sent)
        static readonly Dictionary<ushort, int> PkCounts = new Dictionary<ushort, int>();
        static readonly List<string> PkProblems = new List<string>();
        static string PkLastPrompt;          // last 2092 for me (to detect a re-prompt = refused action)
        static bool PkInHand;
        static readonly HashSet<uint> PkOthers = new HashSet<uint>();   // other players seated at my table
        static int PkAfkSkipped, PkAutoActions;
        static volatile bool PkQuit;

        static void Problem(string s)
        {
            lock (PkProblems) PkProblems.Add(s);
            P("  !! " + s);
        }

        static int ExpectedSize(Rx r)
        {
            byte[] d = r.Data;
            switch (r.Type)
            {
                case 2090: return 29;
                case 2091: return d.Length >= 32 ? 44 + 8 * BitConverter.ToUInt16(d, 30) : -2;
                case 2092: return 28;
                case 2093: return 28;
                case 2094: return d.Length >= 8 ? 8 + 12 * BitConverter.ToUInt16(d, 6) : -2;
                case 2095: return d.Length >= 8 ? 8 + 15 * BitConverter.ToUInt16(d, 6) : -2;
                case 2096: return 16;
                case 2097: return 16;
                case 2098: return d.Length >= 7 ? 45 + 11 * d[6] : -2;
                case 2099: return 12;
                case 2171: return 20;
                case 2172: return d.Length >= 52 ? 52 + 6 * d[51] : -2;
            }
            return -1;
        }

        static string Card(int value, int suit)
        {
            const string ranks = "23456789TJQKA", suits = "hscd";
            return (value >= 0 && value < 13 ? ranks[value].ToString() : "?") + (suit >= 0 && suit < 4 ? suits[suit].ToString() : "?");
        }

        static string Actions(int mask)
        {
            var l = new List<string>();
            string[] n = { "Bet", "Call", "Fold", "Check", "Raise", "AllIn" };
            for (int i = 0; i < 6; i++) if ((mask & (1 << i)) != 0) l.Add(n[i]);
            return l.Count == 0 ? "-" : string.Join("|", l);
        }

        static void OnPokerPacket(Rx r)
        {
            byte[] d = r.Data;
            if (r.Type == 10017 && r.Size >= 16 && BitConverter.ToUInt32(d, 8) == Uid)
            {
                uint count = BitConverter.ToUInt32(d, 12);
                for (int i = 0; i < count && 16 + 32 * i + 12 <= r.Size; i++)
                    if (BitConverter.ToUInt32(d, 16 + 32 * i) == 4)
                        PkMoney = BitConverter.ToInt64(d, 20 + 32 * i);
                return;
            }
            if (r.Type == 10010 && r.Size >= 26 && BitConverter.ToUInt16(d, 24) == 234)
            {
                P(string.Format("  rx 10010/234 pot table {0} = {1:N0}", BitConverter.ToUInt32(d, 8), BitConverter.ToInt64(d, 12)));
                return;
            }
            if (!((r.Type >= 2090 && r.Type <= 2099) || r.Type == 2171 || r.Type == 2172 || r.Type == 3401 || r.Type == 3402))
                return;
            lock (PkCounts) { int c; PkCounts.TryGetValue(r.Type, out c); PkCounts[r.Type] = c + 1; }
            PkLastEvent = DateTime.Now;
            int exp = ExpectedSize(r);
            string sizeNote = "";
            if (exp == -2 || (exp >= 0 && exp != r.Size))
            {
                PkSizeErrors++;
                sizeNote = "  SIZE " + r.Size + " != layout " + exp;
                Problem(r.Type + " size " + r.Size + " does not match the PokerPackets layout (" + exp + ")");
            }
            string hex = Hex(d.Take(r.Size).ToArray());
            switch (r.Type)
            {
                case 2172:
                    {
                        int n = d[51];
                        P(string.Format("  rx 2172 TableInfo [{0}] uid {1} id {2} nl {3} money {4} bb {5:N0} state {6} pot {7:N0} seated {8}{9}", r.Size,
                            BitConverter.ToUInt32(d, 4), BitConverter.ToUInt32(d, 26), BitConverter.ToUInt32(d, 30), BitConverter.ToUInt32(d, 34),
                            BitConverter.ToUInt32(d, 38), d[42], BitConverter.ToInt64(d, 43), n, sizeNote));
                        break;
                    }
                case 2171:
                    {
                        ushort type = BitConverter.ToUInt16(d, 4);
                        uint table = BitConverter.ToUInt32(d, 8), who = BitConverter.ToUInt32(d, 12);
                        P(string.Format("  rx 2171 Interactive [{0}] type {1} table {2} player {3} seat {4}{5}", r.Size, type, table, who, d[16], sizeNote));
                        if (who == Uid && type == 0) { PkTableUid = table; PkSeated = true; }
                        if (who == Uid && type == 1) PkSeated = false;
                        break;
                    }
                case 2090:
                    {
                        ushort join = BitConverter.ToUInt16(d, 5);
                        uint tid = BitConverter.ToUInt32(d, 9), who = BitConverter.ToUInt32(d, 13);
                        P(string.Format("  rx 2090 Enter [{0}] join {1} seat {2} table# {3} player {4} pot {5}{6}", r.Size, join, BitConverter.ToUInt16(d, 7), tid, who, BitConverter.ToInt64(d, 17), sizeNote));
                        if (who == Uid) PkTableId = tid;
                        else lock (PkOthers) { if (join == 2) PkOthers.Remove(who); else PkOthers.Add(who); }
                        break;
                    }
                case 2091:
                    {
                        ushort type = BitConverter.ToUInt16(d, 6), n = BitConverter.ToUInt16(d, 8), m = BitConverter.ToUInt16(d, 30);
                        var cards = new List<string>();
                        for (int i = 0; i < n && i < 5; i++) cards.Add(Card(BitConverter.ToUInt16(d, 10 + 2 * i), BitConverter.ToUInt16(d, 20 + 2 * i)));
                        var who = new List<string>();
                        for (int i = 0; i < m; i++) who.Add(BitConverter.ToUInt32(d, 44 + 8 * i + 4).ToString());
                        P(string.Format("  rx 2091 DealtCard [{0}] {1} cards {2}{3}{4}", r.Size, type == 0 ? "PREFLOP" : type == 1 ? "FLOP" : type == 2 ? "TURN" : type == 3 ? "RIVER" : type.ToString(),
                            string.Join(" ", cards), type == 0 ? string.Format(" players {0} dealer {1} sb {2} bb {3}", string.Join(",", who), BitConverter.ToUInt32(d, 32), BitConverter.ToUInt32(d, 36), BitConverter.ToUInt32(d, 40)) : "", sizeNote));
                        if (type == 0)
                        {
                            if (PkInHand) { PkFlowErrors++; Problem("new PREFLOP deal before the previous hand's 2095 result"); }
                            PkInHand = true;
                            PkHandsStarted++;
                            P("  ---- hand " + PkHandsStarted + " (silver now " + PkMoney.ToString("N0") + ")");
                        }
                        else if (!PkInHand) { PkFlowErrors++; Problem("board cards without a hand"); }
                        break;
                    }
                case 2092:
                    {
                        ushort secs = BitConverter.ToUInt16(d, 4), allowed = BitConverter.ToUInt16(d, 6);
                        long min = BitConverter.ToInt64(d, 8), max = BitConverter.ToInt64(d, 16);
                        uint who = BitConverter.ToUInt32(d, 24);
                        P(string.Format("  rx 2092 ActivePlayer [{0}] player {1} {2}s allowed {3} min {4:N0} max {5:N0}{6}", r.Size, who, secs, Actions(allowed), min, max, sizeNote));
                        if (!PkInHand) { PkFlowErrors++; Problem("2092 turn outside a hand"); }
                        if (allowed == 0) { PkFlowErrors++; Problem("2092 with no allowed action"); }
                        if (min > max) { PkFlowErrors++; Problem("2092 min " + min + " > max " + max); }
                        if (who == Uid)
                        {
                            string key = allowed + "/" + min + "/" + max;
                            if (PkLastPrompt == key) PkRePrompts++;   // same buttons again = the server refused the last action
                            PkLastPrompt = key;
                            PkMyTurns++;
                            int seq = Interlocked.Increment(ref PkActiveSeq);
                            if (O.QuitInHand > 0 && PkHandsStarted >= O.QuitInHand && !PkQuit)
                            {
                                // disconnect in the middle of the hand (no 2096): the server must fold me and go on
                                PkQuit = true;
                                P("    (dropping the connection in the middle of hand " + PkHandsStarted + ", my chips in the pot are forfeited)");
                                ThreadPool.QueueUserWorkItem(_ => Disconnect());
                            }
                            else if (O.AfkPct > 0 && PkRnd.Next(100) < O.AfkPct)
                            {
                                // do not answer: the server must act for me (check, else fold) when the timer runs out
                                PkSkipSeq = seq; PkAfkSkipped++;
                                P("    (not answering this turn: timeout test)");
                            }
                            else
                                ThreadPool.QueueUserWorkItem(_ => { Thread.Sleep(300 + PkRnd.Next(700)); if (seq == PkActiveSeq) Decide(allowed, min, max); });
                        }
                        break;
                    }
                case 2093:
                    {
                        ushort act = BitConverter.ToUInt16(d, 6);
                        P(string.Format("  rx 2093 CallAction [{0}] player {1} {2} added {3:N0} total {4:N0}{5}", r.Size, BitConverter.ToUInt32(d, 24), Actions(act), BitConverter.ToInt64(d, 8), BitConverter.ToInt64(d, 16), sizeNote));
                        if (BitConverter.ToUInt32(d, 24) == Uid)
                        {
                            if (PkSkipSeq == PkActiveSeq) { PkAutoActions++; P("    (server acted for me after the timeout: " + Actions(act) + ")"); }
                            Interlocked.Increment(ref PkActiveSeq); PkLastPrompt = null;
                        }
                        break;
                    }
                case 2094:
                    {
                        int n = BitConverter.ToUInt16(d, 6);
                        var l = new List<string>();
                        for (int i = 0; i < n; i++)
                        {
                            int o = 8 + 12 * i;
                            l.Add(BitConverter.ToUInt32(d, o + 8) + ":" + Card(BitConverter.ToUInt16(d, o), BitConverter.ToUInt16(d, o + 4)) + Card(BitConverter.ToUInt16(d, o + 2), BitConverter.ToUInt16(d, o + 6)));
                        }
                        P(string.Format("  rx 2094 LayCard [{0}] {1}{2}", r.Size, string.Join("  ", l), sizeNote));
                        break;
                    }
                case 2095:
                    {
                        int n = BitConverter.ToUInt16(d, 6);
                        var l = new List<string>();
                        long sum = 0;
                        for (int i = 0; i < n; i++)
                        {
                            int o = 8 + 15 * i;
                            uint who = BitConverter.ToUInt32(d, o + 3);
                            long net = BitConverter.ToInt64(d, o + 7);
                            sum += net;
                            l.Add(who + " net " + net.ToString("N0") + " cat " + BitConverter.ToUInt16(d, o + 1) + (d[o] != 0 ? " KICKED" : ""));
                            if (who == Uid) PkNetSum += net;
                        }
                        P(string.Format("  rx 2095 GameResult [{0}] {1}s  {2}{3}", r.Size, BitConverter.ToUInt16(d, 4), string.Join("; ", l), sizeNote));
                        if (sum != 0) { PkFlowErrors++; Problem("hand result nets do not sum to 0 (" + sum + ")"); }
                        if (!PkInHand) { PkFlowErrors++; Problem("2095 result without a hand"); }
                        PkInHand = false;
                        PkHandsDone++;
                        break;
                    }
                case 2096:
                    {
                        uint who = BitConverter.ToUInt32(d, 12);
                        P(string.Format("  rx 2096 Exit [{0}] table# {1} player {2}{3}", r.Size, BitConverter.ToUInt32(d, 8), who, sizeNote));
                        if (who != Uid) lock (PkOthers) PkOthers.Remove(who);
                        break;
                    }
                default:
                    P(string.Format("  rx {0} {1} [{2}] {3}{4}", r.Type, Names.Of(r.Type), r.Size, hex.Length > 120 ? hex.Substring(0, 120) + ".." : hex, sizeNote));
                    break;
            }
            if (r.Type != 2172)
                Log.AppendLine("      hex " + hex);
        }

        static void Decide(ushort allowed, long min, long max)
        {
            // weights: check/call mostly, some raises, a few folds and all-ins
            var opts = new List<KeyValuePair<ushort, int>>();
            Action<ushort, int> add = (a, w) => { if ((allowed & a) != 0) opts.Add(new KeyValuePair<ushort, int>(a, w)); };
            add(8, 40); add(2, 40); add(1, 15); add(16, 15); add(4, 6); add(32, 1);
            if (opts.Count == 0) return;
            int total = opts.Sum(o => o.Value), pick = PkRnd.Next(total);
            ushort act = opts[0].Key;
            foreach (var o in opts) { if (pick < o.Value) { act = o.Key; break; } pick -= o.Value; }
            long amount = 0;
            if (act == 1 || act == 16)
                amount = min + (long)((max - min) * PkRnd.NextDouble() * 0.02);
            byte[] p = Pkt(2093, 28);
            W16(p, 6, act);
            BitConverter.GetBytes(amount).CopyTo(p, 8);
            W32(p, 24, Uid);
            PkActionsSent++;
            Send(p, "2093 CallAction " + Actions(act) + (amount != 0 ? " to " + amount.ToString("N0") : ""));
        }

        static bool PokerPlay()
        {
            P("");
            P("== POKER (field " + O.Field + ", " + O.Hands + " hands)");
            PkRnd = new Random(O.Seed != 0 ? O.Seed : (int)(Uid ^ Environment.TickCount));
            var hero = Snapshot().First(r => r.Type == 1006);
            PkMoney = BitConverter.ToInt64(hero.Data, 20);
            // replay the updates received so far (money changes after 1006), then follow live
            foreach (var r in Snapshot().Where(r => r.Type == 10017)) OnPokerPacket(r);
            PacketHook = OnPokerPacket;
            Thread.Sleep(500);
            long startMoney = PkMoney;
            P("  silver at start: " + startMoney.ToString("N0"));

            // 3400 MsgTexasExInteractive {1: 0} (Instant Play opened) then {1: 1, 2: field} (Start Match-up)
            byte[] body = PbVarints(1, 0);
            byte[] p = Pkt(3400, 4 + body.Length); Buffer.BlockCopy(body, 0, p, 4, body.Length);
            Exchange(p, "3400 Instant Play opened");
            body = PbVarints(1, 1, 2, O.Field);
            p = Pkt(3400, 4 + body.Length); Buffer.BlockCopy(body, 0, p, 4, body.Length);
            Send(p, "3400 Start Match-up field " + O.Field);
            var until = DateTime.Now.AddSeconds(15);
            while (!PkSeated && DateTime.Now < until && !Closed) Thread.Sleep(100);
            if (!PkSeated)
            {
                Problem("not seated 15 s after Match-up");
                foreach (var r in Since(0).Where(x => x.Type == 1004).Reverse().Take(3)) P("    1004 text: " + Printable(r.Data, 4));
                return Finish(startMoney, false);
            }
            P("  seated at table role " + PkTableUid + " (table# " + PkTableId + ")");

            // play until enough hands are finished (a hand stalls if nothing poker-related arrives for 60 s)
            var hardStop = DateTime.Now.AddSeconds(90 + O.Hands * 60);
            while (PkHandsDone < O.Hands && !Closed && DateTime.Now < hardStop)
            {
                if (!PkSeated) { P("  no longer seated (kicked or the table closed) after " + PkHandsDone + " hands"); break; }
                bool alone; lock (PkOthers) alone = PkOthers.Count == 0;
                if (alone && PkHandsDone > 0 && !PkInHand) { P("  nobody left at the table after " + PkHandsDone + " hands"); break; }
                if ((DateTime.Now - PkLastEvent).TotalSeconds > 40) { Problem("no poker packet for 40 s (stall) after " + PkHandsDone + " hands"); break; }
                Thread.Sleep(200);
            }
            Thread.Sleep(500);
            // leave: 2096 type 1 table# uid
            if (!Closed)
            {
                byte[] exit = Pkt(2096, 16);
                W16(exit, 4, 1); W32(exit, 8, PkTableId); W32(exit, 12, Uid);
                Exchange(exit, "2096 Exit");
                Thread.Sleep(1500);
            }
            return Finish(startMoney, true);
        }

        static bool Finish(long startMoney, bool played)
        {
            PacketHook = null;
            long end = PkMoney;
            P("");
            P("== POKER SUMMARY");
            P("  timeout test: turns not answered " + PkAfkSkipped + ", server auto-actions seen " + PkAutoActions + (PkAfkSkipped == PkAutoActions ? "  OK" : "  MISMATCH"));
            if (PkAfkSkipped != PkAutoActions) Problem("the server did not act for every unanswered turn (" + PkAutoActions + "/" + PkAfkSkipped + ")");
            P("  hands started " + PkHandsStarted + ", finished " + PkHandsDone + ", my turns " + PkMyTurns + ", actions sent " + PkActionsSent + ", re-prompts (refused) " + PkRePrompts);
            P("  silver " + startMoney.ToString("N0") + " -> " + end.ToString("N0") + " (delta " + (end - startMoney).ToString("N0") + "), sum of my nets in 2095 " + PkNetSum.ToString("N0")
                + ((end - startMoney) == PkNetSum ? "  OK" : "  MISMATCH"));
            if (PkQuit) P("  (left in the middle of a hand: the last hand's result was not received, delta includes the forfeited bets)");
            else if ((end - startMoney) != PkNetSum) Problem("silver delta " + (end - startMoney) + " != sum of my hand results " + PkNetSum);
            lock (PkCounts) P("  poker packets: " + string.Join(", ", PkCounts.OrderBy(k => k.Key).Select(k => k.Key + " " + Names.Of(k.Key) + " x" + k.Value)));
            P("  size errors " + PkSizeErrors + ", flow errors " + PkFlowErrors + ", problems " + PkProblems.Count);
            AppDomain.CurrentDomain.SetData("poker.result", string.Format("uid={0};start={1};end={2};hands={3};net={4};problems={5}", Uid, startMoney, end, PkHandsDone, PkNetSum, PkProblems.Count));
            return played && PkProblems.Count == 0 && (PkHandsDone >= O.Hands || PkQuit);
        }

        // ------------------------------------------------------------------ two bots, one process
        static int PokerDuo(string[] args)
        {
            string u1 = "1", p1 = "1", u2 = null, p2 = null, quit2 = null;
            var common = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i], v = i + 1 < args.Length ? args[i + 1] : null;
                switch (a)
                {
                    case "--mode": i++; break;
                    case "--user1": u1 = v; i++; break;
                    case "--pass1": p1 = v; i++; break;
                    case "--user2": u2 = v; i++; break;
                    case "--pass2": p2 = v; i++; break;
                    case "--report": i++; break;
                    case "--create2": break;
                    case "--quit2": quit2 = v; i++; break;
                    default: common.Add(a); break;
                }
            }
            string exe = typeof(Program).Assembly.Location, dir = Path.GetDirectoryName(exe);
            var argsA = new List<string>(common) { "--mode", "poker", "--user", u1, "--pass", p1, "--tag", "[A] ", "--report", Path.Combine(dir, "report_poker_A.txt") };
            var argsB = new List<string>(common) { "--mode", "poker", "--tag", "[B] ", "--report", Path.Combine(dir, "report_poker_B.txt") };
            if (u2 != null) { argsB.Add("--user"); argsB.Add(u2); argsB.Add("--pass"); argsB.Add(p2); }
            if (args.Contains("--create2")) argsB.Add("--create");
            if (quit2 != null) { argsB.Add("--quit-in-hand"); argsB.Add(quit2); }
            var da = AppDomain.CreateDomain("botA");
            var db = AppDomain.CreateDomain("botB");
            int ra = -1, rb = -1;
            var ta = new Thread(() => { ra = da.ExecuteAssembly(exe, argsA.ToArray()); });
            var tb = new Thread(() => { rb = db.ExecuteAssembly(exe, argsB.ToArray()); });
            ta.Start();
            Thread.Sleep(4000);   // one login at a time
            tb.Start();
            ta.Join(); tb.Join();
            string resA = da.GetData("poker.result") as string, resB = db.GetData("poker.result") as string;
            Console.WriteLine();
            Console.WriteLine("== POKERDUO");
            Console.WriteLine("  A: rc " + ra + "  " + resA);
            Console.WriteLine("  B: rc " + rb + "  " + resB);
            int rc = 1;
            if (resA != null && resB != null)
            {
                Func<string, string, long> get = (s, k) => long.Parse(s.Split(';').First(x => x.StartsWith(k + "=")).Substring(k.Length + 1));
                long delta = get(resA, "end") - get(resA, "start") + get(resB, "end") - get(resB, "start");
                Console.WriteLine("  silver of both players together: delta " + delta + (delta == 0 ? "  OK (conserved, no rake)" : "  NOT CONSERVED"));
                if (delta == 0 && ra == 0 && rb == 0) rc = 0;
            }
            File.WriteAllText(Path.Combine(dir, "report_poker_duo.txt"), "A rc " + ra + " " + resA + Environment.NewLine + "B rc " + rb + " " + resB + Environment.NewLine);
            Console.WriteLine(rc == 0 ? "POKERDUO OK" : "POKERDUO FAILED");
            return rc;
        }
    }
}
