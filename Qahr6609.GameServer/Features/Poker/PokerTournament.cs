using System;
using System.Collections.Generic;
using System.Linq;

namespace Conquer6609.Game.MsgServer.Poker
{
    /// <summary>One blind level of a championship (client texas_match_stage.dat: "seconds bigBlind ante addon rebuy").</summary>
    public class TourLevel
    {
        public int Seconds;
        public long BigBlind;
        public long Ante;
        public bool AddonAllowed;
        public bool RebuyAllowed;
    }

    /// <summary>One prize row (client texas_match_prize.dat): fixed money + share of the pool (1/10000) + reward text.</summary>
    public class TourPrizeRow
    {
        public long Money;
        public int Share;
        public int Flag;
        public string Text = "";
    }

    /// <summary>
    /// Championship rules that do not depend on the network (shared by the server and Qahr6609.Tools\Diagnostics\TestBot\pokersim):
    /// blind levels, table sizes, moving players between tables, ranks of busted players, prize pool.
    /// </summary>
    public static class PokerTournament
    {
        /// <summary>Parses "120 1000 0 1 1" (seconds, big blind, ante, add-on allowed, rebuy allowed).</summary>
        public static TourLevel ParseLevel(string s)
        {
            var p = s.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length < 2)
                return null;
            var l = new TourLevel
            {
                Seconds = int.Parse(p[0]),
                BigBlind = long.Parse(p[1]),
                Ante = p.Length > 2 ? long.Parse(p[2]) : 0,
                AddonAllowed = p.Length > 3 && p[3] != "0",
                RebuyAllowed = p.Length > 4 && p[4] != "0"
            };
            if (l.Seconds <= 0 || l.BigBlind < 2 || l.Ante < 0)
                return null;
            return l;
        }

        /// <summary>Level index after <paramref name="seconds"/> of play (the last level lasts forever).
        /// secondsOverride &gt; 0 replaces every level's duration (test championships).</summary>
        public static int LevelAt(IList<TourLevel> levels, double seconds, int secondsOverride = 0)
        {
            if (levels == null || levels.Count == 0)
                return 0;
            double t = 0;
            for (int i = 0; i < levels.Count - 1; i++)
            {
                t += secondsOverride > 0 ? secondsOverride : levels[i].Seconds;
                if (seconds < t)
                    return i;
            }
            return levels.Count - 1;
        }

        /// <summary>Seconds from the start until level <paramref name="index"/> begins.</summary>
        public static double LevelStart(IList<TourLevel> levels, int index, int secondsOverride = 0)
        {
            double t = 0;
            for (int i = 0; i < index && i < levels.Count; i++)
                t += secondsOverride > 0 ? secondsOverride : levels[i].Seconds;
            return t;
        }

        /// <summary>ceil(players / maxSeats) tables whose sizes differ by at most one.</summary>
        public static int[] InitialTableSizes(int players, int maxSeats)
        {
            if (players <= 0)
                return new int[0];
            int tables = (players + maxSeats - 1) / maxSeats;
            var sizes = new int[tables];
            for (int i = 0; i < players; i++)
                sizes[i % tables]++;
            return sizes;
        }

        /// <summary>
        /// Moves for table <paramref name="table"/>, which is between hands. counts = players at every open table
        /// (this one included). If there are more tables than ceil(players/maxSeats) and this is a smallest table, it is
        /// broken (all its players go to the smallest other tables, closeTable = true). Otherwise, when there are not
        /// too many tables, players move from this table to the smallest one while the difference is 2 or more.
        /// Returns the destination table of each player who has to leave (in order).
        /// </summary>
        public static List<int> PlanMoves(IDictionary<int, int> counts, int table, int maxSeats, out bool closeTable)
        {
            closeTable = false;
            var moves = new List<int>();
            if (!counts.ContainsKey(table))
                return moves;
            var c = new Dictionary<int, int>(counts);
            int players = c.Values.Sum();
            int need = Math.Max(1, (players + maxSeats - 1) / maxSeats);
            if (c.Count > need)
            {
                if (c[table] != c.Values.Min())
                    return moves;// another (smaller) table will be broken when it is between hands
                int room = c.Where(kv => kv.Key != table).Sum(kv => maxSeats - kv.Value);
                if (room < c[table])
                    return moves;
                closeTable = true;
                int n = c[table];
                for (int i = 0; i < n; i++)
                {
                    int dest = c.Where(kv => kv.Key != table && kv.Value < maxSeats).OrderBy(kv => kv.Value).ThenBy(kv => kv.Key).First().Key;
                    c[dest]++;
                    c[table]--;
                    moves.Add(dest);
                }
                return moves;
            }
            while (true)
            {
                var others = c.Where(kv => kv.Key != table).OrderBy(kv => kv.Value).ThenBy(kv => kv.Key).ToList();
                if (others.Count == 0)
                    break;
                var min = others[0];
                if (c[table] - min.Value >= 2 && min.Value < maxSeats)
                {
                    c[table]--;
                    c[min.Key]++;
                    moves.Add(min.Key);
                }
                else
                    break;
            }
            return moves;
        }

        /// <summary>
        /// Ranks of players busted in the same hand: aliveBefore = players still in the tournament including them.
        /// The smaller stack at the start of the hand is eliminated first (worse rank); equal stacks by UID.
        /// </summary>
        public static List<KeyValuePair<uint, int>> RankBusted(IEnumerable<KeyValuePair<uint, long>> bustedStartStacks, int aliveBefore)
        {
            var result = new List<KeyValuePair<uint, int>>();
            int rank = aliveBefore;
            foreach (var b in bustedStartStacks.OrderBy(x => x.Value).ThenBy(x => x.Key))
                result.Add(new KeyValuePair<uint, int>(b.Key, rank--));
            return result;
        }

        /// <summary>Prize bracket for this many participants: smallest key &gt;= participants, else the largest key.</summary>
        public static int BracketKey(IEnumerable<int> keys, int participants)
        {
            int best = -1, largest = -1;
            foreach (int k in keys)
            {
                if (k >= participants && (best < 0 || k < best))
                    best = k;
                if (k > largest)
                    largest = k;
            }
            return best >= 0 ? best : largest;
        }

        /// <summary>Prize pool (client 0x833c5f): max(0, fee x participants + extra - fixed money of the bracket).</summary>
        public static long Pool(long fee, int participants, long extra, IList<TourPrizeRow> rows)
        {
            long fixedMoney = rows == null ? 0 : rows.Sum(r => r.Money);
            return Math.Max(0, fee * participants + extra - fixedMoney);
        }

        /// <summary>Money of a rank (1 = champion): fixed money + pool x share / 10000.</summary>
        public static long PrizeMoney(IList<TourPrizeRow> rows, int rank, long pool)
        {
            if (rows == null || rank < 1 || rank > rows.Count)
                return 0;
            var r = rows[rank - 1];
            return r.Money + (long)((decimal)pool * r.Share / 10000m);
        }
    }
}
