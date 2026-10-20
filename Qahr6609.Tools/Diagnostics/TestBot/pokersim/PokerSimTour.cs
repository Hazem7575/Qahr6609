// PokerSim tournament tests (championships): PokerTournament rules (levels, table sizes, moves between tables, ranks of
// busted players, prize pool) + whole simulated tournaments driven like Modules\TexasEntrance\Championship.cs does
// (tournament chips per UID, antes, blind levels, rebuys/add-ons, players leaving, tables broken / balanced between hands).
//   PokerSim.exe tour [tournaments] [seed]
using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer.Poker;

namespace PokerSimTest
{
    partial class Program
    {
        const string Stage2998 = "120 1000 0 1 1@@120 2000 250 1 1@@120 4000 400 1 1@@120 8000 800 1 1@@120 14000 1500 1 1@@120 24000 3000 1 1@@120 40000 4000 1 1@@120 80000 8000 1 1@@120 140000 15000 1 1@@120 200000 25000 1 1@@120 300000 35000 1 0@@120 500000 60000 1 0@@120 1000000 100000 1 0@@120 1600000 200000 1 0@@120 3000000 350000 1 0@@120 10000000 1000000 1 0@@120 20000000 2500000 1 0@@120 60000000 7500000 1 0@@120 160000000 20000000 1 0@@120 500000000 125000000 0 0";

        static void TourTests(int tournaments, int seed)
        {
            int f0 = failures;
            Console.WriteLine("== tournament rules");
            TourRules();
            Console.WriteLine("== antes");
            AnteScenarios();
            Console.WriteLine("== " + tournaments + " simulated tournaments, seed " + seed);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var rnd = new Random(seed);
            long hands = 0, moves = 0, breaks = 0, rebuys = 0, leaves = 0, maxTables = 0;
            for (int t = 0; t < tournaments; t++)
            {
                try
                {
                    var r = OneTournament(rnd, t);
                    hands += r[0]; moves += r[1]; breaks += r[2]; rebuys += r[3]; leaves += r[4]; maxTables = Math.Max(maxTables, r[5]);
                }
                catch (Exception e) { Fail("tournament " + t + " exception " + e.GetType().Name + " " + e.Message + " @ " + e.StackTrace.Split('\n')[0]); }
                if (failures - f0 > 50) break;
            }
            Console.WriteLine("  hands " + hands + ", player moves " + moves + ", tables broken " + breaks + ", rebuys/add-ons " + rebuys + ", players who left " + leaves + ", most tables " + maxTables);
            Console.WriteLine("  tournament failures " + (failures - f0) + "  (" + sw.Elapsed.TotalSeconds.ToString("F0") + " s)");
        }

        static void TourRules()
        {
            var levels = Stage2998.Split(new[] { "@@" }, StringSplitOptions.None).Select(PokerTournament.ParseLevel).ToList();
            Expect(levels.Count == 20 && levels[1].BigBlind == 2000 && levels[1].Ante == 250 && levels[10].RebuyAllowed == false && levels[10].AddonAllowed && !levels[19].AddonAllowed, "2998 stage parse");
            Expect(PokerTournament.LevelAt(levels, 0) == 0 && PokerTournament.LevelAt(levels, 119.9) == 0 && PokerTournament.LevelAt(levels, 120) == 1, "level boundaries");
            Expect(PokerTournament.LevelAt(levels, 1e9) == 19, "last level forever");
            Expect(PokerTournament.LevelAt(levels, 25, 10) == 2, "level seconds override");
            Expect(string.Join(",", PokerTournament.InitialTableSizes(24, 9)) == "8,8,8", "24 players -> 8,8,8");
            Expect(string.Join(",", PokerTournament.InitialTableSizes(19, 9)) == "7,6,6", "19 players -> 7,6,6");
            Expect(string.Join(",", PokerTournament.InitialTableSizes(9, 9)) == "9", "9 players -> one table");
            bool close;
            // 3 tables, 15 players -> need 2: the smallest (4) breaks into the others
            var m = PokerTournament.PlanMoves(new Dictionary<int, int> { { 1, 4 }, { 2, 5 }, { 3, 6 } }, 1, 9, out close);
            Expect(close && m.Count == 4 && Math.Abs((5 + m.Count(x => x == 2)) - (6 + m.Count(x => x == 3))) <= 1, "break smallest table (" + string.Join(",", m) + ")");
            m = PokerTournament.PlanMoves(new Dictionary<int, int> { { 1, 4 }, { 2, 5 }, { 3, 6 } }, 3, 9, out close);
            Expect(!close && m.Count == 0, "not the smallest: wait for it");
            m = PokerTournament.PlanMoves(new Dictionary<int, int> { { 1, 9 }, { 2, 3 } }, 1, 9, out close);
            Expect(!close && m.Count == 3 && m.All(x => x == 2), "balance 9/3 -> 6/6 (" + string.Join(",", m) + ")");
            m = PokerTournament.PlanMoves(new Dictionary<int, int> { { 1, 9 }, { 2, 8 } }, 1, 9, out close);
            Expect(m.Count == 0, "9/8 stays");
            m = PokerTournament.PlanMoves(new Dictionary<int, int> { { 1, 1 }, { 2, 9 } }, 1, 9, out close);
            Expect(m.Count == 0 && !close, "lone player waits for the big table to send players (10 players need 2 tables)");
            m = PokerTournament.PlanMoves(new Dictionary<int, int> { { 1, 9 }, { 2, 1 } }, 1, 9, out close);
            Expect(m.Count == 4, "9/1 -> 5/5");
            m = PokerTournament.PlanMoves(new Dictionary<int, int> { { 1, 2 }, { 2, 2 } }, 2, 9, out close);
            Expect(close && m.Count == 2, "final table merge 2+2");
            var ranks = PokerTournament.RankBusted(new[] { new KeyValuePair<uint, long>(7, 500), new KeyValuePair<uint, long>(8, 100), new KeyValuePair<uint, long>(9, 500) }, 10);
            Expect(ranks.First(x => x.Key == 8).Value == 10 && ranks.First(x => x.Key == 7).Value == 9 && ranks.First(x => x.Key == 9).Value == 8, "busted ranks: smaller start stack worse");
            var rows = new List<TourPrizeRow>();
            foreach (var share in new[] { 4000, 2000, 1000, 300, 200, 100, 100, 100, 100, 100 }) rows.Add(new TourPrizeRow { Share = share });
            for (int i = 0; i < 490; i++) rows.Add(new TourPrizeRow());
            long pool = PokerTournament.Pool(800000, 24, 0, rows);
            Expect(pool == 19200000 && PokerTournament.PrizeMoney(rows, 1, pool) == 7680000 && PokerTournament.PrizeMoney(rows, 11, pool) == 0 && PokerTournament.PrizeMoney(rows, 501, pool) == 0, "pool 24 x 800K, champion 40%");
            long paid = Enumerable.Range(1, 24).Sum(r => PokerTournament.PrizeMoney(rows, r, pool));
            Expect(paid <= pool, "prizes never exceed the pool");
            Expect(PokerTournament.Pool(100, 2, 0, new List<TourPrizeRow> { new TourPrizeRow { Money = 1000 } }) == 0, "pool not negative");
            Expect(PokerTournament.BracketKey(new[] { 1500, 3000 }, 1000) == 1500 && PokerTournament.BracketKey(new[] { 1500, 3000 }, 2000) == 3000 && PokerTournament.BracketKey(new[] { 1500, 3000 }, 5000) == 3000, "bracket choice");
        }

        static PokerHand MakeAnte(long bb, long ante, int dealer, params long[] stacks)
        {
            var ps = stacks.Select((x, i) => new PokerHandPlayer { UID = (uint)(1 + i), Seat = i, Stack = x }).ToList();
            var h = new PokerHand(ps, dealer, bb, true, new PokerDeck(Enumerable.Range(0, 52).Select(PokerCard.FromId).ToList()), ante);
            long a, b; h.Start(out a, out b);
            return h;
        }

        static void AnteScenarios()
        {
            var h = MakeAnte(100, 10, 0, 1000, 1000, 1000);
            Expect(h.Pot == 30 + 50 + 100, "antes + blinds in the pot (" + h.Pot + ")");
            Expect(h.Players[2].RoundBet == 100 && h.Players[2].TotalBet == 110 && h.CurrentBet == 100, "ante not part of the bet to call");
            Expect(h.ToCall(h.Players[0]) == 100, "UTG calls 100");
            h = MakeAnte(100, 10, 0, 1000, 1000, 5);
            Expect(h.Players[2].AllIn && h.Players[2].TotalBet == 5, "short stack all-in with part of the ante");
            h = MakeAnte(100, 50, 0, 50, 50, 50);
            Expect(h.IsRoundComplete() && h.ActiveIndex == -1, "everybody all-in from the antes: no betting");
            while (h.Board.Count < 5) h.NextStreet();
            h.Settle();
            Expect(h.Players.Sum(p => p.Won) == 150, "antes-only pot paid");
            h = MakeAnte(100, 10, 0, 1000, 1000);   // heads-up with antes
            Expect(h.Pot == 20 + 50 + 100 && h.ActiveIndex == h.SmallBlindIndex, "HU antes, SB acts first");
            h.Act(h.Active.UID, PokerAction.Fold, 0);
            h.Settle();
            Expect(h.Players[1].Won == 170 && h.Players.Sum(p => p.Won) == 170, "fold: BB wins antes+blinds (uncalled part back)");
        }

        sealed class SimTable { public int Id; public List<uint> Seats = new List<uint>(); public int Dealer; }

        /// <returns>hands, moves, breaks, rebuys, leaves, max tables</returns>
        static long[] OneTournament(Random rnd, int tn)
        {
            var levels = Stage2998.Split(new[] { "@@" }, StringSplitOptions.None).Select(PokerTournament.ParseLevel).ToList();
            int n = rnd.Next(4) == 0 ? rnd.Next(2, 10) : rnd.Next(10, 80);
            long start = 100000;
            int levelSec = rnd.Next(2) == 0 ? 120 : 20;
            var bank = new Dictionary<uint, long>();
            var rank = new Dictionary<uint, int>();
            var rebuysOf = new Dictionary<uint, int>();
            long issued = 0, forfeited = 0;
            long hands = 0, moves = 0, breaks = 0, rebuys = 0, leaves = 0, maxTables = 0;
            var uids = Enumerable.Range(0, n).Select(i => (uint)(5000 + i)).OrderBy(x => rnd.Next()).ToList();
            foreach (var u in uids) { bank[u] = start; issued += start; rebuysOf[u] = 0; }
            var tables = new List<SimTable>();
            int k = 0, id = 1;
            foreach (var size in PokerTournament.InitialTableSizes(n, 9))
            {
                var t = new SimTable { Id = id++ };
                for (int i = 0; i < size; i++) t.Seats.Add(uids[k++]);
                tables.Add(t);
            }
            maxTables = tables.Count;
            double clock = 0;
            Func<int> alive = () => bank.Count;
            Action<SimTable> plan = null;
            plan = t =>
            {
                if (!tables.Contains(t)) return;
                if (t.Seats.Count == 0) { tables.Remove(t); return; }
                var counts = tables.ToDictionary(x => x.Id, x => x.Seats.Count);
                bool close;
                var m = PokerTournament.PlanMoves(counts, t.Id, 9, out close);
                var movers = close ? t.Seats.ToList() : t.Seats.OrderBy(x => rnd.Next()).Take(m.Count).ToList();
                for (int i = 0; i < m.Count; i++)
                {
                    var dest = tables.First(x => x.Id == m[i]);
                    t.Seats.Remove(movers[i]);
                    dest.Seats.Insert(rnd.Next(dest.Seats.Count + 1), movers[i]);
                    moves++;
                    if (dest.Seats.Count > 9) Fail("tour " + tn + " table " + dest.Id + " has " + dest.Seats.Count + " players");
                }
                if (close) { if (t.Seats.Count != 0) Fail("tour " + tn + " closed table not empty"); tables.Remove(t); breaks++; }
            };
            int guard = 0;
            while (alive() > 1)
            {
                if (++guard > 200000) { Fail("tour " + tn + " does not end (" + alive() + " alive, tables " + string.Join("/", tables.Select(x => x.Seats.Count)) + ")"); break; }
                // a table that cannot play (one player) is idle: it is planned like the manager's idle check
                foreach (var t in tables.Where(x => x.Seats.Count < 2).ToList()) plan(t);
                var playable = tables.Where(x => x.Seats.Count >= 2).ToList();
                if (playable.Count == 0) { Fail("tour " + tn + " stalled: no table with 2 players, alive " + alive() + " tables " + string.Join("/", tables.Select(x => x.Seats.Count))); break; }
                var table = playable[rnd.Next(playable.Count)];
                var lv = levels[PokerTournament.LevelAt(levels, clock, levelSec)];
                // rebuys / add-ons between hands (only while allowed; rebuy needs chips <= start)
                foreach (var u in table.Seats)
                {
                    if (rnd.Next(100) < 3 && ((lv.RebuyAllowed && bank[u] <= start && rebuysOf[u] < 1) || (lv.AddonAllowed && rebuysOf[u] < 7)))
                    {
                        bank[u] += 100000; issued += 100000; rebuysOf[u]++; rebuys++;
                    }
                }
                // a player may leave (forfeits his chips, ranked now)
                if (rnd.Next(400) == 0)
                {
                    var u = table.Seats[rnd.Next(table.Seats.Count)];
                    forfeited += bank[u];
                    rank[u] = alive();
                    bank.Remove(u);
                    table.Seats.Remove(u);
                    leaves++;
                    plan(table);
                    continue;
                }
                // ---- one hand (PokerTable.StartHand / DoAction / FinishHand with the tournament bank)
                table.Dealer = (table.Dealer + 1) % table.Seats.Count;
                var players = table.Seats.Select((u, i) => new PokerHandPlayer { UID = u, Seat = i, Stack = bank[u] }).ToList();
                var startStacks = players.ToDictionary(p => p.UID, p => p.Stack);
                var ids = Enumerable.Range(0, 52).OrderBy(x => rnd.Next()).ToList();
                var hand = new PokerHand(players, table.Dealer, lv.BigBlind, true, new PokerDeck(ids.Select(PokerCard.FromId).ToList()), lv.Ante);
                long a1, a2;
                hand.Start(out a1, out a2);
                foreach (var p in players) bank[p.UID] -= p.TotalBet;
                int steps = 0;
                while (true)
                {
                    if (++steps > 3000) { Fail("tour " + tn + " hand does not end"); return new long[6]; }
                    if (hand.IsRoundComplete())
                    {
                        if (hand.NotFoldedCount <= 1 || hand.Stage == PokerStage.River) break;
                        hand.NextStreet();
                        continue;
                    }
                    var act = hand.Active;
                    if (act == null) { Fail("tour " + tn + " nobody active"); return new long[6]; }
                    var allowed = hand.AllowedActions(act);
                    bool auto = rnd.Next(10) == 0;   // offline player on auto play: check, else fold
                    PokerAction pick;
                    long amount = 0;
                    if (auto) pick = hand.ToCall(act) == 0 ? PokerAction.Check : PokerAction.Fold;
                    else
                    {
                        var legal = new[] { PokerAction.Check, PokerAction.Call, PokerAction.Bet, PokerAction.Raise, PokerAction.AllIn, PokerAction.Fold }.Where(x => (allowed & x) != 0).ToList();
                        if (legal.Count > 1 && rnd.Next(3) != 0) legal.Remove(PokerAction.Fold);
                        if (legal.Contains(PokerAction.AllIn) && legal.Count > 1 && rnd.Next(4) != 0) legal.Remove(PokerAction.AllIn);
                        pick = legal[rnd.Next(legal.Count)];
                        long min = hand.MinRaiseTo(act), max = hand.MaxRaiseTo(act);
                        amount = min + (long)((max - min) * rnd.NextDouble() * 0.3);
                    }
                    long before = act.Stack;
                    var r = hand.Act(act.UID, pick, amount);
                    if (!r.Valid) { Fail("tour " + tn + " " + pick + " refused"); r = hand.Act(act.UID, PokerAction.Fold, 0); }
                    if (r.Added > 0)
                    {
                        if (bank[act.UID] < r.Added) Fail("tour " + tn + " debit beyond the bank");
                        bank[act.UID] -= r.Added;
                    }
                    if (bank[act.UID] != act.Stack) Fail("tour " + tn + " bank " + bank[act.UID] + " != stack behind " + act.Stack);
                }
                while (hand.NotFoldedCount > 1 && hand.Board.Count < 5) hand.NextStreet();
                hand.Settle();
                foreach (var p in players) bank[p.UID] += p.Won;
                hands++;
                clock += rnd.Next(10, 60);
                long total = bank.Values.Sum();
                if (total + forfeited != issued) Fail("tour " + tn + " chips not conserved: " + total + " + forfeited " + forfeited + " != issued " + issued);
                // busted players: ranked by their stack at the start of the hand
                var busted = players.Where(p => bank[p.UID] == 0).Select(p => new KeyValuePair<uint, long>(p.UID, startStacks[p.UID])).ToList();
                foreach (var b in PokerTournament.RankBusted(busted, alive()))
                {
                    rank[b.Key] = b.Value;
                    bank.Remove(b.Key);
                    table.Seats.Remove(b.Key);
                }
                if (bank.Values.Any(v => v < 0)) Fail("tour " + tn + " negative chips");
                plan(table);
                maxTables = Math.Max(maxTables, tables.Count);
                int need = (alive() + 8) / 9;
                if (tables.Count > need + tables.Count(x => x.Seats.Count >= 2 && x.Seats.Count <= 9) && tables.Count > n) Fail("tour " + tn + " too many tables");
            }
            if (alive() == 1)
            {
                var w = bank.Keys.First();
                if (bank[w] + forfeited != issued) Fail("tour " + tn + " winner chips " + bank[w] + " + forfeited " + forfeited + " != issued " + issued);
                rank[w] = 1;
            }
            var all = rank.Values.OrderBy(x => x).ToList();
            if (all.Count != n || !all.SequenceEqual(Enumerable.Range(1, n))) Fail("tour " + tn + " ranks are not 1.." + n + ": " + string.Join(",", all.Take(20)));
            return new[] { hands, moves, breaks, rebuys, leaves, maxTables };
        }
    }
}
