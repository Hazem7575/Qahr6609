// TestBot championship mode (Texas Hold'em tournaments, Qahr6609.GameServer\Modules\TexasEntrance\Championship.cs):
//   --mode champ       one bot: Today list (3400 act 2 -> 3402), sign-up (act 3), waits for the board (2501 mode 0 + act 4),
//                      plays every turn (2092 -> 2093), asks the live rank (act 14 -> 3405) and the ranking (act 9 -> 3404),
//                      optional rebuy/add-on (act 5) and leave (act 23), until its result (act 18 + 2501 mode 1).
//                      Checks: packet sizes, board flag byte of MsgPlayer, own chips (act 15 / 3406) never negative,
//                      silver delta = prize - fee - rebuys.
//   --mode champmulti  N bots in one process (AppDomains), direct login (sandbox: [Security] VerifyLoginTokens=0):
//                      --bots N --uid-base U [--create] ; prints every bot's rank and silver delta.
// C# 5 (framework csc).
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
        static volatile bool ChInBoard, ChOut;
        static int ChRank, ChBoardOn, ChBoardOff, ChLooks, ChLookBad, ChAct4, ChAct16, ChAct15, ChChips3406, ChMyRank3405, ChRank3404, ChMoves, ChRebuyOk, ChRebuyFail;
        static long ChChips = -1, ChRebuyPaid;
        static uint ChRoom, ChChamp;
        static readonly List<string> ChLog = new List<string>();

        // ---- protobuf reader: field -> list of varints (wire 0) ; nested messages as raw bytes (wire 2)
        static void Pb(byte[] d, int from, int to, Action<int, ulong, byte[]> f)
        {
            int pos = from;
            while (pos < to)
            {
                ulong key = Var(d, ref pos, to);
                int field = (int)(key >> 3), wire = (int)(key & 7);
                if (wire == 0) f(field, Var(d, ref pos, to), null);
                else if (wire == 2)
                {
                    int len = (int)Var(d, ref pos, to);
                    var sub = new byte[len];
                    Buffer.BlockCopy(d, pos, sub, 0, Math.Min(len, to - pos));
                    pos += len;
                    f(field, 0, sub);
                }
                else if (wire == 1) pos += 8;
                else if (wire == 5) pos += 4;
                else break;
            }
        }

        static ulong Var(byte[] d, ref int pos, int to)
        {
            ulong v = 0;
            for (int s = 0; s < 64 && pos < to; s += 7)
            {
                byte b = d[pos++];
                v |= (ulong)(b & 0x7f) << s;
                if ((b & 0x80) == 0) break;
            }
            return v;
        }

        static void Tx3400(params ulong[] actionThenArgs)
        {
            var fv = new List<ulong> { 1, actionThenArgs[0] };
            for (int i = 1; i < actionThenArgs.Length; i++) { fv.Add(2); fv.Add(actionThenArgs[i]); }
            byte[] body = PbVarints(fv.ToArray());
            byte[] p = Pkt(3400, 4 + body.Length); Buffer.BlockCopy(body, 0, p, 4, body.Length);
            Send(p, "3400 act " + string.Join(",", actionThenArgs));
        }

        static void OnChampPacket(Rx r)
        {
            byte[] d = r.Data;
            if (r.Type == 2501)
            {
                uint mode = BitConverter.ToUInt32(d, 4);
                P("  rx 2501 CrossSwitch mode " + mode + " ids " + BitConverter.ToUInt32(d, 8) + "/" + BitConverter.ToUInt32(d, 12) + " [" + r.Size + "]");
                if (mode == 0) { ChBoardOn++; ChInBoard = true; } else { ChBoardOff++; ChInBoard = false; }
                return;
            }
            if (r.Type == 10014 && ChInBoard && r.Size > 136)
            {
                // MsgPlayer while on the board: the server sets byte 136 = 2 (board role cache); level is the u16 at 134
                uint who = BitConverter.ToUInt32(d, 12);
                if (who != Uid)
                {
                    if (d[136] == 2) ChLooks++;
                    else { ChLookBad++; }
                    P("  rx 10014 look of " + who + " byte136=" + d[136] + " level@134=" + BitConverter.ToUInt16(d, 134) + " [" + r.Size + "]");
                }
                return;
            }
            if (r.Type == 3400 || r.Type == 3402 || r.Type == 3404 || r.Type == 3405 || r.Type == 3406)
            {
                lock (PkCounts) { int c; PkCounts.TryGetValue(r.Type, out c); PkCounts[r.Type] = c + 1; }
                PkLastEvent = DateTime.Now;
                if (r.Type == 3400)
                {
                    ulong act = 0; var args = new List<ulong>(); long v3 = 0;
                    Pb(d, 4, r.Size, (f, v, b) => { if (f == 1) act = v; else if (f == 2) args.Add(v); else if (f == 3) v3 = (long)v; });
                    P("  rx 3400 act " + act + " [" + string.Join(",", args) + "]" + (act == 15 ? " value " + v3 : "") + " [" + r.Size + "]");
                    switch (act)
                    {
                        case 4: ChAct4++; if (args.Count > 0) ChRoom = (uint)args[0]; if (args.Count > 1) ChChamp = (uint)args[1]; break;
                        case 19: ChMoves++; if (args.Count > 0) ChRoom = (uint)args[0]; break;
                        case 16: ChAct16++; break;
                        case 15: ChAct15++; ChChips = v3; if (v3 < 0) Problem("act 15 negative chips " + v3); break;
                        case 18: ChRank = args.Count > 1 ? (int)args[1] : -1; ChOut = true; break;
                        case 5: if (args.Count > 3 && args[3] == 0) ChRebuyOk++; else ChRebuyFail++; break;
                    }
                }
                else if (r.Type == 3406)
                {
                    ChChips3406++;
                    var l = new List<string>();
                    Pb(d, 4, r.Size, (f, v, b) =>
                    {
                        if (f != 1 || b == null) return;
                        ulong uid = 0; long chips = 0;
                        Pb(b, 0, b.Length, (f2, v2, b2) => { if (f2 == 1) uid = v2; else if (f2 == 2) chips = (long)v2; });
                        if (chips < 0) Problem("3406 negative chips");
                        if (uid == Uid) ChChips = chips;
                        l.Add(uid + ":" + chips);
                    });
                    P("  rx 3406 chips " + string.Join(" ", l));
                }
                else if (r.Type == 3405)
                {
                    ChMyRank3405++;
                    ulong my = 0, total = 0; long myChips = 0; var rows = new List<string>();
                    Pb(d, 4, r.Size, (f, v, b) =>
                    {
                        if (f == 1) my = v; else if (f == 2) myChips = (long)v; else if (f == 4) total = v;
                        else if (f == 5 && b != null)
                        {
                            string name = ""; long c = 0;
                            Pb(b, 0, b.Length, (f2, v2, b2) => { if (f2 == 2 && b2 != null) name = Encoding.Default.GetString(b2); else if (f2 == 3) c = (long)v2; });
                            rows.Add(name + " " + c);
                        }
                    });
                    P("  rx 3405 my rank " + my + " chips " + myChips + " total " + total + ": " + string.Join(", ", rows));
                }
                else if (r.Type == 3404)
                {
                    ChRank3404++;
                    var names = new List<string>(); ulong total = 0;
                    Pb(d, 4, r.Size, (f, v, b) =>
                    {
                        if (f == 2) total = v;
                        if (f == 3 && b != null) Pb(b, 0, b.Length, (f2, v2, b2) => { if (f2 == 2 && b2 != null) names.Add(Encoding.Default.GetString(b2)); });
                    });
                    P("  rx 3404 ranking total " + total + ": " + string.Join(", ", names));
                }
                else if (r.Type == 3402)
                    P("  rx 3402 list [" + r.Size + "]");
                return;
            }
            OnPokerPacket(r);
        }

        static List<uint[]> ParseList(byte[] d, int size, out ulong page, out ulong total)
        {
            var rows = new List<uint[]>();
            ulong pg = 0, tt = 0;
            Pb(d, 4, size, (f, v, b) =>
            {
                if (f == 1) pg = v; else if (f == 2) tt = v;
                else if (f == 3 && b != null)
                {
                    var row = new uint[8];
                    Pb(b, 0, b.Length, (f2, v2, b2) => { if (f2 >= 1 && f2 <= 7) row[f2] = (uint)v2; if (f2 == 7) row[7] = 1; });
                    rows.Add(row);
                }
            });
            page = pg; total = tt;
            return rows;
        }

        static bool ChampPlay()
        {
            P("");
            P("== CHAMPIONSHIP");
            PkRnd = new Random(O.Seed != 0 ? O.Seed : (int)(Uid ^ Environment.TickCount));
            var hero = Snapshot().First(r => r.Type == 1006);
            PkMoney = BitConverter.ToInt64(hero.Data, 20);
            foreach (var r in Snapshot().Where(r => r.Type == 10017)) OnPokerPacket(r);
            PacketHook = OnChampPacket;
            Thread.Sleep(500);
            long startMoney = PkMoney;
            P("  silver at start: " + startMoney.ToString("N0"));

            // ---- Today list, all pages
            uint session = O.Session, champ = 0;
            var all = new List<uint[]>();
            for (uint page = 1; page <= 40; page++)
            {
                int m = Mark();
                Tx3400(2, page);
                if (!WaitSince(m, r => r.Type == 3402, 3000)) { Problem("no 3402 for page " + page); break; }
                var rx = Since(m).First(r => r.Type == 3402);
                ulong pg, total;
                var rows = ParseList(rx.Data, rx.Size, out pg, out total);
                if (pg != page) Problem("3402 page " + pg + " != requested " + page);
                all.AddRange(rows);
                if (page * 3 >= total) break;
            }
            P("  Today list: " + all.Count + " rows: " + string.Join(" ", all.Select(x => x[1] + "/" + x[2] + "@" + x[5].ToString("0000") + (x[6] == 2 ? "(closed)" : "") + (x[7] == 1 ? "(over)" : "") + "x" + x[3])));
            var target = all.FirstOrDefault(x => (session == 0 || x[1] == session) && x[6] != 2 && x[7] == 0 && (session != 0 || x[1] % 100 >= 90));
            if (target == null) { Problem("session " + session + " not in the list / not open"); return ChampFinish(startMoney); }
            session = target[1]; champ = target[2];

            // ---- sign up
            {
                int m = Mark();
                Tx3400(3, session, champ);
                WaitSince(m, r => r.Type == 3400, 4000);
                Thread.Sleep(300);
                bool acked = Since(m).Any(r => r.Type == 3400 && r.Data[5] == 3);
                P("  sign-up " + (acked ? "acknowledged (act 3)" : "NOT acknowledged"));
                foreach (var r in Since(m).Where(x => x.Type == 1004)) P("    1004 text: " + Printable(r.Data, 4));
                if (!acked) { Problem("sign-up refused"); return ChampFinish(startMoney); }
                Thread.Sleep(500);
                P("  silver after sign-up: " + PkMoney.ToString("N0") + " (fee " + (startMoney - PkMoney).ToString("N0") + ")");
                // ranking tab before the start (act 9)
                Tx3400(9, session, champ, 1, 10);
            }

            // ---- wait for the board and play until the result
            var until = DateTime.Now.AddMinutes(25);
            int handsAtLastRank = 0;
            bool left = false;
            while (!ChOut && !Closed && DateTime.Now < until)
            {
                Thread.Sleep(250);
                if (ChInBoard && PkHandsDone > handsAtLastRank)
                {
                    handsAtLastRank = PkHandsDone;
                    if (PkHandsDone % 3 == 0) Tx3400(14, ChRoom, champ, 1, 7);
                    if (O.ChampRebuy > 0 && PkRnd.Next(100) < O.ChampRebuy)
                    {
                        long before = PkMoney;
                        Tx3400(5, 0, champ, (ulong)PkRnd.Next(2));
                        Thread.Sleep(600);
                        if (PkMoney < before) ChRebuyPaid += before - PkMoney;
                    }
                    if (O.ChampLeave > 0 && PkHandsDone >= O.ChampLeave && !left)
                    {
                        left = true;
                        P("  (leaving the tournament: act 23)");
                        Tx3400(23, ChRoom, champ);
                    }
                }
            }
            if (ChOut) AppDomain.CurrentDomain.SetData("champ.out", session.ToString());
            Thread.Sleep(3000);   // prizes are paid when the whole tournament is over
            if (ChOut)
            {
                // wait until the tournament is over (my prize arrives then): poll the ranking until nobody is alive
                var end = DateTime.Now.AddMinutes(20);
                long last = PkMoney; DateTime stable = DateTime.Now;
                while (DateTime.Now < end && !Closed)
                {
                    if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "champ_over_" + session + ".flag"))) break;
                    Thread.Sleep(1000);
                }
                Thread.Sleep(2000);
                Tx3400(9, session, champ, 1, 20);
                Thread.Sleep(1000);
            }
            return ChampFinish(startMoney);
        }

        static bool ChampFinish(long startMoney)
        {
            PacketHook = null;
            P("");
            P("== CHAMPIONSHIP SUMMARY");
            P("  rank " + ChRank + ", board on/off " + ChBoardOn + "/" + ChBoardOff + ", act4 " + ChAct4 + ", act16 " + ChAct16 + ", act15 " + ChAct15 + ", 3406 " + ChChips3406 + ", 3405 " + ChMyRank3405 + ", 3404 " + ChRank3404 + ", moves " + ChMoves);
            P("  looks with board flag " + ChLooks + ", without " + ChLookBad + ", rebuys ok/refused " + ChRebuyOk + "/" + ChRebuyFail);
            P("  hands started " + PkHandsStarted + ", finished " + PkHandsDone + ", my turns " + PkMyTurns + ", actions " + PkActionsSent + ", re-prompts " + PkRePrompts);
            P("  silver " + startMoney.ToString("N0") + " -> " + PkMoney.ToString("N0") + " (delta " + (PkMoney - startMoney).ToString("N0") + ")");
            lock (PkCounts) P("  packets: " + string.Join(", ", PkCounts.OrderBy(k => k.Key).Select(k => k.Key + " x" + k.Value)));
            P("  size errors " + PkSizeErrors + ", flow errors " + PkFlowErrors + ", problems " + PkProblems.Count);
            if (ChLookBad > 0) Problem("MsgPlayer during the board without the flag byte: " + ChLookBad);
            if (ChBoardOn > 0 && ChBoardOff == 0) Problem("board never closed (2501 mode 1 missing)");
            AppDomain.CurrentDomain.SetData("champ.result", string.Format("uid={0};start={1};end={2};rank={3};hands={4};problems={5};rebuypaid={6}", Uid, startMoney, PkMoney, ChRank, PkHandsDone, PkProblems.Count, ChRebuyPaid));
            return PkProblems.Count == 0 && ChRank > 0;
        }

        // ------------------------------------------------------------------ N bots in one process
        static int ChampMulti(string[] args)
        {
            int bots = 4; uint uidBase = 1990001; var common = new List<string>(); string leave = null; int leaver = -1;
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i], v = i + 1 < args.Length ? args[i + 1] : null;
                switch (a)
                {
                    case "--mode": i++; break;
                    case "--bots": bots = int.Parse(v); i++; break;
                    case "--uid-base": uidBase = uint.Parse(v); i++; break;
                    case "--report": i++; break;
                    case "--leaver": leaver = int.Parse(v); i++; break;
                    case "--leave-after": leave = v; i++; break;
                    default: common.Add(a); break;
                }
            }
            string exe = typeof(Program).Assembly.Location, dir = Path.GetDirectoryName(exe);
            var domains = new AppDomain[bots];
            var rc = new int[bots];
            var threads = new List<Thread>();
            foreach (var f in Directory.GetFiles(dir, "champ_over_*.flag")) File.Delete(f);
            for (int b = 0; b < bots; b++)
            {
                int idx = b;
                var a = new List<string>(common) { "--mode", "champ", "--direct", (uidBase + (uint)b).ToString(), "--serial", "TB" + (uidBase + (uint)b),
                    "--tag", "[" + (char)('A' + b % 26) + (b >= 26 ? (b / 26).ToString() : "") + "] ", "--report", Path.Combine(dir, "report_champ_" + b + ".txt"), "--name", "TBot" + (uidBase + (uint)b) % 100000 };
                if (b == leaver && leave != null) { a.Add("--champ-leave"); a.Add(leave); }
                domains[b] = AppDomain.CreateDomain("bot" + b);
                var t = new Thread(() => { rc[idx] = domains[idx].ExecuteAssembly(exe, a.ToArray()); });
                threads.Add(t);
                t.Start();
                Thread.Sleep(1500);
            }
            // when every bot has its result the tournament is over: tell them (their prizes are paid at the end)
            while (threads.Any(t => t.IsAlive))
            {
                Thread.Sleep(1000);
                int done = 0;
                string sess = null;
                for (int b = 0; b < bots; b++)
                {
                    var log = Path.Combine(dir, "report_champ_" + b + ".txt");
                    try
                    {
                        // ChOut is in the other AppDomain; look for the result line in its live console log instead
                        var data = domains[b].GetData("champ.out") as string;
                        if (data != null) { done++; sess = data; }
                    }
                    catch { }
                }
                if (done == bots && sess != null) File.WriteAllText(Path.Combine(dir, "champ_over_" + sess + ".flag"), "over");
            }
            Console.WriteLine();
            Console.WriteLine("== CHAMPMULTI (" + bots + " bots)");
            var results = new List<string>();
            for (int b = 0; b < bots; b++)
            {
                string r = domains[b].GetData("champ.result") as string;
                results.Add(r);
                Console.WriteLine("  bot " + b + ": rc " + rc[b] + "  " + r);
            }
            File.WriteAllLines(Path.Combine(dir, "report_champ_multi.txt"), results.Select((r, i) => "bot " + i + " rc " + rc[i] + " " + r));
            bool ok = rc.All(x => x == 0) && results.All(x => x != null);
            Console.WriteLine(ok ? "CHAMPMULTI OK" : "CHAMPMULTI FAILED");
            return ok ? 0 : 1;
        }
    }
}
