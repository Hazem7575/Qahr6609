// PokerSim - offline tests of the Texas Hold'em engine (Qahr6609.GameServer\Game\MsgServer\Poker\PokerEngine.cs + PokerHandEvaluator.cs,
// compiled in as source files: they have no server dependencies). Build: build.bat next to this file.
//   PokerSim.exe eval [randomDeals]      hand evaluator vs an independent reference (exhaustive 5-card + random 7-card + edge cases)
//   PokerSim.exe sim  [hands] [seed]     random hands driven like PokerTable does, money conservation + rule invariants
//   PokerSim.exe tour [n] [seed]       championship rules + simulated tournaments (PokerSimTour.cs)
//   PokerSim.exe all                     everything with the default sizes
using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer.Poker;

namespace PokerSimTest
{
    // ======================================================================= reference evaluator (independent)
    static class Ref
    {
        // 5 cards -> comparable key: category * 13^5 + tiebreak ranks (base 13). Royal flush = category 9 (the client's names).
        public static long Key5(PokerCard[] c)
        {
            var ranks = c.Select(x => (int)x.Value).ToArray();
            bool flush = c.All(x => x.Suit == c[0].Suit);
            var distinct = ranks.Distinct().OrderByDescending(r => r).ToArray();
            int straightHigh = -1;
            if (distinct.Length == 5)
            {
                if (distinct[0] - distinct[4] == 4) straightHigh = distinct[0];
                else if (distinct.SequenceEqual(new[] { 12, 3, 2, 1, 0 })) straightHigh = 3;
            }
            // groups ordered by size, then rank
            var groups = ranks.GroupBy(r => r).Select(g => new { R = g.Key, N = g.Count() })
                .OrderByDescending(g => g.N).ThenByDescending(g => g.R).ToList();
            int cat;
            List<int> tb;
            if (straightHigh >= 0 && flush) { cat = straightHigh == 12 ? 9 : 8; tb = new List<int> { straightHigh }; }
            else if (groups[0].N == 4) { cat = 7; tb = groups.Select(g => g.R).ToList(); }
            else if (groups[0].N == 3 && groups[1].N == 2) { cat = 6; tb = groups.Select(g => g.R).ToList(); }
            else if (flush) { cat = 5; tb = distinct.ToList(); }
            else if (straightHigh >= 0) { cat = 4; tb = new List<int> { straightHigh }; }
            else if (groups[0].N == 3) { cat = 3; tb = groups.Select(g => g.R).ToList(); }
            else if (groups[0].N == 2 && groups[1].N == 2) { cat = 2; tb = groups.Select(g => g.R).ToList(); }
            else if (groups[0].N == 2) { cat = 1; tb = groups.Select(g => g.R).ToList(); }
            else { cat = 0; tb = distinct.ToList(); }
            long key = cat;
            for (int i = 0; i < 5; i++)
                key = key * 13 + (i < tb.Count ? tb[i] : 0);
            return key;
        }

        public static int CategoryOf(long key) { return (int)(key / (13L * 13 * 13 * 13 * 13)); }

        /// <summary>Best 5 of n (5..7) cards: key and the 5 cards.</summary>
        public static long Best(IList<PokerCard> cards, out PokerCard[] best)
        {
            int n = cards.Count;
            long bestKey = -1; best = null;
            var pick = new PokerCard[5];
            for (int a = 0; a < n; a++)
                for (int b = a + 1; b < n; b++)
                    for (int c = b + 1; c < n; c++)
                        for (int d = c + 1; d < n; d++)
                            for (int e = d + 1; e < n; e++)
                            {
                                pick[0] = cards[a]; pick[1] = cards[b]; pick[2] = cards[c]; pick[3] = cards[d]; pick[4] = cards[e];
                                long k = Key5(pick);
                                if (k > bestKey) { bestKey = k; best = (PokerCard[])pick.Clone(); }
                            }
            return bestKey;
        }
    }

    partial class Program
    {
        static int failures;
        static void Fail(string s) { failures++; if (failures <= 60) Console.WriteLine("  FAIL " + s); }
        static string Str(IEnumerable<PokerCard> c) { return string.Join(" ", c.Select(x => x.ToString())); }

        static int Main(string[] args)
        {
            string mode = args.Length > 0 ? args[0] : "all";
            if (mode == "eval" || mode == "all")
                EvalTests(args.Length > 1 && mode == "eval" ? int.Parse(args[1]) : 3000000);
            if (mode == "tour" || mode == "all")
                TourTests(args.Length > 1 && mode == "tour" ? int.Parse(args[1]) : 300, args.Length > 2 ? int.Parse(args[2]) : 777);
            if (mode == "sim" || mode == "all")
                SimTests(args.Length > 1 && mode == "sim" ? int.Parse(args[1]) : 200000, args.Length > 2 ? int.Parse(args[2]) : 12345);
            Console.WriteLine(failures == 0 ? "ALL OK" : ("FAILURES: " + failures));
            return failures == 0 ? 0 : 1;
        }

        // ======================================================================= evaluator
        static void EvalTests(int randomDeals)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("== evaluator: exhaustive 5-card hands");
            var deck = Enumerable.Range(0, 52).Select(PokerCard.FromId).ToArray();
            var map = new Dictionary<long, int>();
            long count = 0;
            var h = new PokerCard[5];
            int f0 = failures;
            for (int a = 0; a < 52; a++)
                for (int b = a + 1; b < 52; b++)
                    for (int c = b + 1; c < 52; c++)
                        for (int d = c + 1; d < 52; d++)
                            for (int e = d + 1; e < 52; e++)
                            {
                                h[0] = deck[a]; h[1] = deck[b]; h[2] = deck[c]; h[3] = deck[d]; h[4] = deck[e];
                                long rk = Ref.Key5(h);
                                int ev = PokerHandEvaluator.Evaluate(h);
                                count++;
                                if ((int)PokerHandEvaluator.Category(ev) != Ref.CategoryOf(rk))
                                    Fail("5-card category " + Str(h) + " eval " + PokerHandEvaluator.Category(ev) + " ref " + Ref.CategoryOf(rk));
                                int prev;
                                if (map.TryGetValue(rk, out prev)) { if (prev != ev) Fail("5-card same ref class, different score " + Str(h)); }
                                else map[rk] = ev;
                            }
            var keys = map.Keys.OrderBy(k => k).ToList();
            for (int i = 1; i < keys.Count; i++)
                if (map[keys[i]] <= map[keys[i - 1]])
                    Fail("5-card ordering: ref class " + keys[i] + " not above " + keys[i - 1] + " (scores " + map[keys[i]] + " <= " + map[keys[i - 1]] + ")");
            Console.WriteLine("  " + count + " hands, " + keys.Count + " equivalence classes (expected 2598960 / 7462), failures " + (failures - f0));
            if (count != 2598960 || keys.Count != 7462) Fail("5-card counts");

            Console.WriteLine("== evaluator: " + randomDeals + " random 7-card deals (2 players + board each)");
            f0 = failures;
            var rnd = new Random(777);
            var ids = Enumerable.Range(0, 52).ToArray();
            int ties = 0, catCount = 0;
            var catHist = new int[10];
            for (int n = 0; n < randomDeals; n++)
            {
                for (int i = 0; i < 9; i++) { int j = i + rnd.Next(52 - i); int t = ids[i]; ids[i] = ids[j]; ids[j] = t; }
                var board = ids.Take(5).Select(PokerCard.FromId).ToList();
                var p1 = new[] { PokerCard.FromId(ids[5]), PokerCard.FromId(ids[6]) };
                var p2 = new[] { PokerCard.FromId(ids[7]), PokerCard.FromId(ids[8]) };
                int e1 = PokerHandEvaluator.Evaluate(p1[0], p1[1], board), e2 = PokerHandEvaluator.Evaluate(p2[0], p2[1], board);
                PokerCard[] b1, b2;
                long r1 = Ref.Best(p1.Concat(board).ToList(), out b1), r2 = Ref.Best(p2.Concat(board).ToList(), out b2);
                if ((int)PokerHandEvaluator.Category(e1) != Ref.CategoryOf(r1)) Fail("7-card category " + Str(p1) + " | " + Str(board));
                // the 7-card score must equal the score of the best 5 cards
                if (PokerHandEvaluator.Evaluate(b1) != e1) Fail("7-card score != best-5 score " + Str(p1) + " | " + Str(board) + " best " + Str(b1));
                if (Math.Sign(e1.CompareTo(e2)) != Math.Sign(r1.CompareTo(r2)))
                    Fail("7-card compare " + Str(p1) + " vs " + Str(p2) + " | " + Str(board) + " eval " + e1 + "/" + e2 + " ref " + r1 + "/" + r2);
                if (r1 == r2) ties++;
                catHist[Ref.CategoryOf(r1)]++; catCount++;
                // also 6-card (turn) and 5-card+2 subsets: evaluator accepts 5..7 cards
                if ((n & 7) == 0)
                {
                    var six = p1.Concat(board.Take(4)).ToList();
                    PokerCard[] b6;
                    long r6 = Ref.Best(six, out b6);
                    int e6 = PokerHandEvaluator.Evaluate(six);
                    if ((int)PokerHandEvaluator.Category(e6) != Ref.CategoryOf(r6) || PokerHandEvaluator.Evaluate(b6) != e6) Fail("6-card " + Str(six));
                }
            }
            Console.WriteLine("  ties (split pots) " + ties + ", category histogram " + string.Join(",", catHist) + ", failures " + (failures - f0));

            Console.WriteLine("== evaluator: edge cases");
            f0 = failures;
            EdgeCases();
            Console.WriteLine("  failures " + (failures - f0) + "  (" + sw.Elapsed.TotalSeconds.ToString("F0") + " s)");
        }

        static List<PokerCard> C(string s) { return s.Split(' ').Select(PokerCard.Parse).ToList(); }
        static int E(string s) { return PokerHandEvaluator.Evaluate(C(s)); }

        static void Expect(bool ok, string what) { if (!ok) Fail("edge: " + what); }

        static void EdgeCases()
        {
            // wheel
            Expect(PokerHandEvaluator.Category(E("Ah 2s 3c 4d 5h Kc Qd")) == PokerHandCategory.Straight, "wheel is a straight");
            Expect(E("Ah 2s 3c 4d 5h Kc Qd") < E("2h 3s 4c 5d 6h Kc Qd"), "6-high straight beats the wheel");
            Expect(E("Ah 2s 3c 4d 5h 6c Qd") == E("2h 3s 4c 5d 6h Kc Qd"), "A-6 run plays the six-high straight");
            Expect(PokerHandEvaluator.Category(E("Ah Ks Qc Jd 9h 8c 2d")) == PokerHandCategory.HighCard, "no wrap-around Q-K-A-2-3");
            Expect(PokerHandEvaluator.Category(E("Qh Ks Ac 2d 3h 8c 7d")) == PokerHandCategory.HighCard, "no wrap-around K-A-2-3-4");
            // steel wheel vs flush / straight flush
            Expect(PokerHandEvaluator.Category(E("Ah 2h 3h 4h 5h Kh Qh")) == PokerHandCategory.StraightFlush, "steel wheel beats the A-high flush in the same 7 cards");
            Expect(E("Ah 2h 3h 4h 5h 9c 9d") < E("2h 3h 4h 5h 6h 9c 9d"), "6-high SF beats steel wheel");
            Expect(PokerHandEvaluator.Category(E("Ah Kh Qh Jh Th 9h 8h")) == PokerHandCategory.RoyalFlush, "royal");
            Expect(PokerHandEvaluator.Category(E("9h 8h 7h 6h 5h 4s 3s")) == PokerHandCategory.StraightFlush, "straight flush");
            // straight + flush in different suits is only a flush; SF with a higher plain straight
            Expect(PokerHandEvaluator.Category(E("9h 8h 7h 6h 2h Ts 5s")) == PokerHandCategory.Flush, "straight 6-T + flush (not SF) = flush");
            Expect(E("5h 6h 7h 8h 9h Ts Jc") == E("5h 6h 7h 8h 9h 2s 3c"), "SF 9-high, the T-J off-suit do not count");
            // flush kickers: 5 best of 6 suited
            Expect(E("Ah Kh 9h 7h 5h 3h 2c") == E("Ah Kh 9h 7h 5h 2s 2c"), "flush uses the 5 highest of 6 suited cards");
            Expect(E("Ah Kh 9h 7h 6h 3h 2c") > E("Ah Kh 9h 7h 5h 4h 2c"), "flush 5th card kicker");
            // quads kicker from the board
            Expect(E("9h 9s 9c 9d Ah 2c 3d") > E("9h 9s 9c 9d Kh Qc Jd"), "quads kicker");
            Expect(E("9h 9s 9c 9d Ah Ac 3d") == E("9h 9s 9c 9d Ah 2c 3d"), "quads + pair: kicker is just the ace");
            Expect(E("9h 9s 9c 9d Kh Kc Kd") == E("9h 9s 9c 9d Kh 2c 3d"), "quads + trips: kicker K");
            // full houses
            Expect(E("Kh Ks Kc 2d 2h Ac Qd") < E("Ah As Ac 2d 2h Kc Qd"), "trips rank decides the full house");
            Expect(E("Kh Ks Kc 3d 3h 2c 2d") == E("Kh Ks Kc 3d 3h Ac Qd"), "full house best pair (3s over 2s)");
            Expect(E("Kh Ks Kc Qd Qh Qc 2d") == E("Kh Ks Kc Qd Qh 2c 3d"), "two trips: second trips plays as the pair");
            Expect(E("Kh Ks Kc Qd Qh Ac Ad") == E("Kh Ks Kc Ad Ah 2c 3d"), "full house: the higher of trips2/pair (A pair over Q)");
            // two pair kickers / three pairs
            Expect(E("Ah As Kc Kd Qh Qc 2d") == E("Ah As Kc Kd Qh 2c 3d"), "three pairs: the third pair is the kicker");
            Expect(E("Ah As Kc Kd 2h 2c Jd") == E("Ah As Kc Kd Jh 3c 4d"), "three pairs: kicker J above the 2s");
            Expect(E("Ah As Kc Kd 5h 4c 3d") < E("Ah As Kc Kd 6h 4c 3d"), "two pair kicker");
            Expect(E("Ah As 2c 2d Kh 4c 3d") < E("Ah As 3c 3d 2h 4c 5d"), "two pair: second pair rank");
            // one pair kickers, high card
            Expect(E("Ah As Kc Qd Jh 4c 3d") > E("Ah As Kc Qd Th 4c 3d"), "pair third kicker");
            Expect(E("Ah As Kc Qd Jh 9c 3d") == E("Ah As Kc Qd Jh 8c 2d"), "pair: only 3 kickers count");
            Expect(E("Ah Ks Qc Jd 9h 8c 3d") == E("Ah Ks Qc Jd 9h 7c 2d"), "high card: only 5 cards count");
            // trips kickers
            Expect(E("7h 7s 7c Ad Kh 4c 3d") > E("7h 7s 7c Ad Qh 4c 3d"), "trips kicker");
            // board plays: both players play the board -> split
            var board = C("Ah Ks Qc Jd Th");
            Expect(PokerHandEvaluator.Evaluate(C("2c")[0], C("3d")[0], board) == PokerHandEvaluator.Evaluate(C("4c")[0], C("5d")[0], board), "board straight plays for both (split)");
            var board2 = C("9h 9s 9c 9d Ah");
            Expect(PokerHandEvaluator.Evaluate(C("Kc")[0], C("Kd")[0], board2) == PokerHandEvaluator.Evaluate(C("2c")[0], C("3d")[0], board2), "quads + ace on board: kickers cannot play");
            // suits never break ties
            Expect(E("Ah Kh Qh Jh 9h 2c 3d") == E("As Ks Qs Js 9s 2c 3d"), "flush suits do not matter");
        }

        // ======================================================================= engine simulation
        static void SimTests(int hands, int seed)
        {
            Console.WriteLine("== engine: fixed scenarios");
            Scenarios();
            Console.WriteLine("== engine: " + hands + " random hands, seed " + seed);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var rnd = new Random(seed);
            int f0 = failures;
            var stats = new Dictionary<string, long>();
            Action<string> st = k => { long v; stats.TryGetValue(k, out v); stats[k] = v + 1; };
            for (int hn = 0; hn < hands; hn++)
            {
                try { OneHand(rnd, hn, st); }
                catch (Exception e) { Fail("hand " + hn + " exception: " + e.GetType().Name + " " + e.Message + " @ " + e.StackTrace.Split('\n')[0]); }
                if (failures - f0 > 50) { Console.WriteLine("  too many failures, stopping"); break; }
            }
            Console.WriteLine("  " + string.Join(", ", stats.OrderBy(k => k.Key).Select(k => k.Key + "=" + k.Value)));
            Console.WriteLine("  failures " + (failures - f0) + "  (" + sw.Elapsed.TotalSeconds.ToString("F0") + " s)");
        }

        static PokerHand Make(long bb, bool nl, int dealer, params long[] stacks)
        {
            var ps = stacks.Select((x, i) => new PokerHandPlayer { UID = (uint)(1 + i), Seat = i, Stack = x }).ToList();
            var h = new PokerHand(ps, dealer, bb, nl, new PokerDeck(Enumerable.Range(0, 52).Select(PokerCard.FromId).ToList()));
            long a, b; h.Start(out a, out b);
            return h;
        }

        static void Scenarios()
        {
            int f0 = failures;
            // heads-up: the button posts the small blind and acts first preflop, the big blind acts first after the flop
            var h = Make(10, true, 0, 1000, 1000);
            Expect(h.SmallBlindIndex == 0 && h.BigBlindIndex == 1 && h.ActiveIndex == 0, "HU blinds/first actor");
            h.Act(1, PokerAction.Call, 0);
            Expect(h.ActiveIndex == 1 && !h.IsRoundComplete(), "HU big blind gets the option");
            h.Act(2, PokerAction.Check, 0);
            Expect(h.IsRoundComplete(), "HU preflop over after the BB checks");
            h.NextStreet();
            Expect(h.ActiveIndex == 1, "HU big blind acts first on the flop");
            // heads-up: big blind all-in for less than the small blind -> nothing left to decide preflop
            h = Make(10, true, 0, 1000, 3);
            Expect(h.IsRoundComplete(), "HU BB all-in for 3 < SB 5: the SB has nothing to call");
            // 3 players: min-raise is the last raise size; short all-in does not reopen the betting
            h = Make(10, true, 0, 1000, 1000, 1000, 45);  // dealer 0, SB 1, BB 2, UTG 3 (45)
            Expect(h.ActiveIndex == 3, "UTG acts first");
            h.Act(4, PokerAction.Raise, 20);              // UTG (45 chips) min-raises to 20
            Expect(h.CurrentBet == 20 && h.MinRaise == 10, "raise to 20 (min)");
            h.Act(1, PokerAction.Raise, 35);              // dealer re-raises to 35 (min 30), increment 15
            Expect(h.CurrentBet == 35 && h.MinRaise == 15, "raise to 35, min raise 15");
            h.Act(2, PokerAction.Call, 0);                // SB calls 35
            h.Act(3, PokerAction.Call, 0);                // BB calls 35
            h.Act(4, PokerAction.AllIn, 0);               // UTG all-in 45: +10 < 15 = short all-in
            Expect(h.CurrentBet == 45, "short all-in to 45");
            var dealer = h.Players[0];
            Expect(h.Active == dealer && (h.AllowedActions(dealer) & (PokerAction.Raise | PokerAction.AllIn)) == 0, "short all-in does not reopen the raise for players who already acted");
            var r = h.Act(1, PokerAction.AllIn, 0);
            Expect(r.Action == PokerAction.Call && r.Added == 10, "all-in request after a short all-in is only a call (got " + r.Action + " " + r.Added + ")");
            // fixed limit: cap of 4 bets, raise = one unit
            h = Make(10, false, 0, 10000, 10000, 10000);
            for (int i = 0; i < 6 && !h.IsRoundComplete(); i++)
            {
                var a = h.Active;
                h.Act(a.UID, PokerAction.Raise, 0);
            }
            Expect(h.CurrentBet == 40 && h.BetsThisRound == 4, "FL preflop capped at 4 bets (got " + h.CurrentBet + ")");
            h = Make(10, false, 0, 10000, 10000, 10000);
            r = h.Act(h.Active.UID, PokerAction.AllIn, 0);
            Expect(r.Action == PokerAction.Raise && h.CurrentBet == 20, "FL all-in request with a big stack = one raise (got " + r.Action + " to " + h.CurrentBet + ")");
            // no limit: a raise is capped at the effective stack (the most the others can still call)
            h = Make(10, true, 0, 1000000, 500);          // HU: dealer/SB 1,000,000 vs BB 500
            Expect(h.MaxRaiseTo(h.Players[0]) == 500, "max raise-to = the big blind's 500 (got " + h.MaxRaiseTo(h.Players[0]) + ")");
            r = h.Act(1, PokerAction.AllIn, 0);
            Expect(r.Added == 495 && !h.Players[0].AllIn && h.Players[0].Stack == 1000000 - 500, "rich all-in only puts in what can be called (added " + r.Added + ")");
            r = h.Act(2, PokerAction.Call, 0);
            Expect(r.Action == PokerAction.AllIn && h.IsRoundComplete(), "short stack calls all-in, betting over");
            // nobody left who could call a raise: no raise offered
            h = Make(10, true, 0, 1000, 1000, 15);         // dealer 0, SB 1, BB 2 (15 chips)
            h.Act(1, PokerAction.Raise, 100);
            h.Act(2, PokerAction.Fold, 0);
            Expect(h.Active.UID == 3 && (h.AllowedActions(h.Active) & PokerAction.Raise) == 0, "BB with 15 chips facing 100 cannot raise (only fold / call all-in)");
            h = Make(10, true, 0, 1000, 1000, 1000);
            h.Act(1, PokerAction.Call, 0); h.Act(2, PokerAction.Call, 0); h.Act(3, PokerAction.Check, 0);
            h.NextStreet();
            h.Act(2, PokerAction.Bet, 990);               // SB bets everything (990 = all-in)
            Expect(h.Players[1].AllIn, "SB all-in");
            Expect((h.AllowedActions(h.Active) & PokerAction.Raise) == 0, "equal stacks after an all-in bet: nobody can raise, only call");
            Console.WriteLine("  scenario failures " + (failures - f0));
        }

        static void OneHand(Random rnd, int hn, Action<string> st)
        {
            int n = rnd.Next(2, 10);
            bool noLimit = rnd.Next(3) != 0;
            long[] bbs = { 2, 3, 5, 10, 100, 10000, 5000000 };
            long bb = bbs[rnd.Next(bbs.Length)];
            int dealer = rnd.Next(n);
            var wallets = new long[n];
            var players = new List<PokerHandPlayer>();
            long maxStake = rnd.Next(5) == 0 ? bb * rnd.Next(1, 60) : 0;
            for (int i = 0; i < n; i++)
            {
                int kind = rnd.Next(4);
                long w = kind == 0 ? bb + rnd.Next((int)Math.Min(bb * 3, int.MaxValue))           // short: 1..4 BB
                    : kind == 1 ? bb * rnd.Next(1, 30) + rnd.Next(100)
                    : kind == 2 ? bb * rnd.Next(30, 300) + rnd.Next(1000)
                    : (long)bb * rnd.Next(1, 8);
                if (w < bb) w = bb;
                wallets[i] = w;
                long stack = maxStake > 0 && w > maxStake ? maxStake : w;
                if (stack <= 0) stack = w;
                players.Add(new PokerHandPlayer { UID = (uint)(1000 + i), Seat = i, Stack = stack });
            }
            long startSum = wallets.Sum();
            var startStacks = players.Select(p => p.Stack).ToArray();
            long spentOutside = 0;

            // seeded deck
            var ids = Enumerable.Range(0, 52).ToList();
            for (int i = 51; i > 0; i--) { int j = rnd.Next(i + 1); int t = ids[i]; ids[i] = ids[j]; ids[j] = t; }
            long ante = rnd.Next(3) == 0 ? Math.Max(1, bb / (rnd.Next(1, 8))) * (rnd.Next(4) == 0 ? 3 : 1) : 0;   // championship antes
            if (ante > 0) st("ante hands");
            var hand = new PokerHand(players, dealer, bb, noLimit, new PokerDeck(ids.Select(PokerCard.FromId).ToList()), ante);
            long sbp, bbp;
            hand.Start(out sbp, out bbp);
            for (int i = 0; i < n; i++) wallets[i] -= players[i].TotalBet;   // PokerTable debits the blinds
            st(noLimit ? "hands NL" : "hands FL");

            // first to act preflop
            CheckFirstActor(hand, true, hn);

            var shrunk = new long[n];
            int steps = 0;
            PokerStage lastStage = hand.Stage;
            while (true)
            {
                if (++steps > 2000) { Fail("hand " + hn + " does not end"); return; }
                CheckMoney(hand, wallets, startSum - spentOutside, startStacks, shrunk, hn);
                if (hand.IsRoundComplete())
                {
                    CheckRoundComplete(hand, hn);
                    if (hand.NotFoldedCount <= 1 || hand.Stage == PokerStage.River)
                        break;
                    hand.NextStreet();
                    CheckFirstActor(hand, false, hn);
                    continue;
                }
                var a = hand.Active;
                if (a == null) { Fail("hand " + hn + " round not complete but nobody active (stage " + hand.Stage + ")"); return; }
                if (!a.CanAct) { Fail("hand " + hn + " active player cannot act"); return; }
                int ai = players.IndexOf(a);

                // events outside the action: a player leaves (fold out of turn), silver spent elsewhere
                int ev = rnd.Next(100);
                if (ev < 2)
                {
                    var live = players.Where(p => !p.Folded).ToList();
                    var victim = live[rnd.Next(live.Count)];
                    hand.ForceFold(victim.UID);
                    st("leave");
                    continue;
                }
                if (ev < 4)
                {
                    // PokerTable.SyncStack on the active player before he acts
                    long cut = rnd.Next(2) == 0 ? a.Stack : (long)(a.Stack * rnd.NextDouble());
                    long spend = Math.Min(cut, wallets[ai]);
                    wallets[ai] -= spend; spentOutside += spend;
                    if (wallets[ai] < a.Stack)
                    {
                        shrunk[ai] += a.Stack - Math.Max(0, wallets[ai]);
                        a.Stack = Math.Max(0, wallets[ai]);
                        if (a.Stack == 0) { if (a.TotalBet == 0) hand.ForceFold(a.UID); else a.AllIn = true; }
                    }
                    st("spent-elsewhere");
                    if (!a.CanAct) { hand.Refresh(); continue; }
                }

                var allowed = hand.AllowedActions(a);
                long toCall = hand.ToCall(a);
                long curBet = hand.CurrentBet, minRaise = hand.MinRaise, minTo = hand.MinRaiseTo(a), maxTo = hand.MaxRaiseTo(a);
                int betsBefore = hand.BetsThisRound;
                bool canRaiseFlag = a.CanRaise;
                long roundBefore = a.RoundBet;

                // pick an action: mostly a legal one, sometimes anything (clients can send anything)
                PokerAction act;
                PokerAction[] all = { PokerAction.Fold, PokerAction.Check, PokerAction.Call, PokerAction.Bet, PokerAction.Raise, PokerAction.AllIn };
                if (rnd.Next(5) == 0) act = all[rnd.Next(all.Length)];
                else
                {
                    var legal = all.Where(x => (allowed & x) != 0).ToList();
                    // fewer folds so hands reach showdowns
                    if (legal.Count > 1 && rnd.Next(3) != 0) legal.Remove(PokerAction.Fold);
                    act = legal[rnd.Next(legal.Count)];
                }
                long amount;
                int am = rnd.Next(6);
                amount = am == 0 ? 0 : am == 1 ? minTo : am == 2 ? maxTo : am == 3 ? maxTo + 1000 : am == 4 ? minTo + (long)((maxTo - minTo) * rnd.NextDouble()) : curBet + 1;

                var r = hand.Act(a.UID, act, amount);
                if (!r.Valid)
                {
                    st("refused");
                    if (!(act == PokerAction.Check && toCall > 0))
                        Fail("hand " + hn + " action " + act + " refused (toCall " + toCall + ")");
                    // the table shows the buttons again; the player then folds or calls
                    r = hand.Act(a.UID, rnd.Next(2) == 0 ? PokerAction.Fold : PokerAction.Call, 0);
                    if (!r.Valid) { Fail("hand " + hn + " fold/call refused"); return; }
                    act = r.Action;
                    allowed = PokerAction.Fold | PokerAction.Call | PokerAction.AllIn | PokerAction.Check;
                }
                st("act " + r.Action);
                if ((allowed & r.Action) == 0)
                    Fail("hand " + hn + " " + (noLimit ? "NL" : "FL") + " sent " + act + " -> did " + r.Action + " which was not in the allowed mask " + allowed
                        + " (toCall " + toCall + " stack " + (a.Stack + r.Added) + " canRaise " + canRaiseFlag + ")");
                if (r.Added != a.RoundBet - roundBefore) Fail("hand " + hn + " Added mismatch");
                if (r.Added > wallets[ai]) Fail("hand " + hn + " debit more than the wallet");
                wallets[ai] -= r.Added;   // PokerTable.DoAction debits r.Added

                long increment = hand.CurrentBet - curBet;
                if (increment > 0)
                {
                    if (!canRaiseFlag)
                        Fail("hand " + hn + " raised although the betting was not reopened for him (" + r.Action + ")");
                    if (!a.AllIn && a.RoundBet != maxTo)   // a raise to the effective stack (all the others can call) may be short
                    {
                        if (noLimit && increment < minRaise) Fail("hand " + hn + " NL raise increment " + increment + " < min raise " + minRaise);
                        if (!noLimit && increment != hand.LimitUnit) Fail("hand " + hn + " FL raise increment " + increment + " != unit " + hand.LimitUnit);
                        if (!noLimit && hand.BetsThisRound > PokerHand.LimitBetCap) Fail("hand " + hn + " FL bet cap exceeded");
                    }
                    else if (!noLimit && increment > hand.LimitUnit)
                        Fail("hand " + hn + " FL all-in raised by " + increment + " > unit " + hand.LimitUnit + " (fixed limit allows one unit)");
                    st(a.AllIn ? "raise all-in" : "raise");
                }
            }

            // ---- settle (PokerTable.FinishHand)
            bool showdown = hand.NotFoldedCount > 1;
            if (showdown && hand.Board.Count < 5)
            {
                while (hand.Board.Count < 5) hand.NextStreet();   // the table runs the board out street by street
            }
            var expected = ReferenceSettle(hand);
            hand.Settle();
            long won = players.Sum(p => p.Won), bet = players.Sum(p => p.TotalBet);
            if (won != bet) Fail("hand " + hn + " pot not conserved: won " + won + " bet " + bet);
            for (int i = 0; i < n; i++)
            {
                if (players[i].Won != expected[i])
                    Fail("hand " + hn + " seat " + i + " won " + players[i].Won + " expected " + expected[i] + " (bets " + string.Join(",", players.Select(p => p.TotalBet + (p.Folded ? "f" : ""))) + ")");
                if (players[i].Folded && players[i].Won > 0) Fail("hand " + hn + " folded player won chips");
                if (players[i].Won < 0) Fail("hand " + hn + " negative win");
                wallets[i] += players[i].Won;
            }
            if (wallets.Sum() != startSum - spentOutside) Fail("hand " + hn + " money not conserved: " + wallets.Sum() + " vs " + (startSum - spentOutside));
            if (wallets.Any(w => w < 0)) Fail("hand " + hn + " negative wallet");
            if (showdown) st("showdown"); else st("fold-win");
            if (showdown)
            {
                var live = players.Where(p => !p.Folded).ToList();
                int best = live.Max(p => p.Score);
                if (live.Count(p => p.Score == best) > 1) st("split");
                if (players.Where(p => !p.Folded).Select(p => p.TotalBet).Distinct().Count() > 1) st("side-pots");
            }
        }

        static void CheckFirstActor(PokerHand hand, bool preflop, int hn)
        {
            var ps = hand.Players;
            if (hand.ActiveIndex < 0)
                return;
            int n = ps.Count;
            int from = preflop ? hand.BigBlindIndex : hand.DealerIndex;
            int expect = -1;
            for (int k = 1; k <= n; k++)
            {
                int i = (from + k) % n;
                if (ps[i].CanAct) { expect = i; break; }
            }
            if (preflop && n == 2 && hand.SmallBlindIndex != hand.DealerIndex) Fail("hand " + hn + " heads-up: the button must post the small blind");
            if (preflop && n > 2 && hand.SmallBlindIndex != (hand.DealerIndex + 1) % n) Fail("hand " + hn + " small blind not left of the button");
            if (hand.ActiveIndex != expect) Fail("hand " + hn + " first to act " + hand.ActiveIndex + " expected " + expect + (preflop ? " preflop" : " postflop"));
        }

        static void CheckRoundComplete(PokerHand hand, int hn)
        {
            if (hand.NotFoldedCount <= 1) return;
            var can = hand.Players.Where(p => p.CanAct).ToList();
            long maxOther = 0;
            foreach (var p in can)
            {
                maxOther = hand.Players.Where(o => o != p && !o.Folded).Select(o => o.RoundBet).DefaultIfEmpty(0).Max();
                if (p.RoundBet < maxOther) Fail("hand " + hn + " round over while a player has not matched (" + p.RoundBet + " < " + maxOther + ")");
                if (can.Count >= 2 && !p.Acted) Fail("hand " + hn + " round over while a player has not acted (stage " + hand.Stage + ")");
            }
        }

        static void CheckMoney(PokerHand hand, long[] wallets, long expectedSum, long[] startStacks, long[] shrunk, int hn)
        {
            long pot = hand.Pot;
            if (wallets.Sum() + pot != expectedSum) Fail("hand " + hn + " balances+pot " + (wallets.Sum() + pot) + " != " + expectedSum);
            for (int i = 0; i < hand.Players.Count; i++)
            {
                var p = hand.Players[i];
                if (p.Stack < 0) Fail("hand " + hn + " negative stack");
                if (p.Stack + p.TotalBet + shrunk[i] != startStacks[i]) Fail("hand " + hn + " stack+bets drifted");
                if (p.AllIn && p.Stack != 0) Fail("hand " + hn + " all-in with chips behind");
            }
        }

        /// <summary>Independent pot distribution: per contribution level (live players), best reference hand, odd chips from the left of the button.</summary>
        static long[] ReferenceSettle(PokerHand hand)
        {
            var ps = hand.Players;
            int n = ps.Count;
            var won = new long[n];
            var contrib = ps.Select(p => p.TotalBet).ToArray();
            var liveIdx = Enumerable.Range(0, n).Where(i => !ps[i].Folded).ToList();
            if (liveIdx.Count == 1)
            {
                won[liveIdx[0]] = contrib.Sum();
                return won;
            }
            var key = new long[n];
            foreach (int i in liveIdx)
            {
                PokerCard[] b;
                key[i] = Ref.Best(new[] { ps[i].Card1, ps[i].Card2 }.Concat(hand.Board).ToList(), out b);
            }
            var order = Enumerable.Range(1, n).Select(k => (hand.DealerIndex + k) % n).ToList();
            var levels = liveIdx.Select(i => contrib[i]).Where(v => v > 0).Distinct().OrderBy(v => v).ToList();
            long prev = 0;
            List<int> lastWinners = null;
            foreach (var level in levels)
            {
                long pot = 0;
                for (int i = 0; i < n; i++) pot += Math.Max(0, Math.Min(contrib[i], level) - prev);
                var elig = liveIdx.Where(i => contrib[i] >= level).ToList();
                long best = elig.Max(i => key[i]);
                var winners = order.Where(i => elig.Contains(i) && key[i] == best).ToList();
                Split(won, winners, pot);
                lastWinners = winners;
                prev = level;
            }
            long dead = 0;
            for (int i = 0; i < n; i++) dead += Math.Max(0, contrib[i] - prev);
            if (dead > 0) Split(won, lastWinners, dead);
            return won;
        }

        static void Split(long[] won, List<int> winners, long pot)
        {
            long share = pot / winners.Count, odd = pot % winners.Count;
            foreach (var w in winners) won[w] += share;
            for (int k = 0; k < odd; k++) won[winners[k]] += 1;
        }
    }
}
