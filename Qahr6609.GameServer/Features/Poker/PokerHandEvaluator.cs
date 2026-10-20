using System;
using System.Collections.Generic;

namespace Conquer6609.Game.MsgServer.Poker
{
    /// <summary>
    /// One playing card in the client's encoding (MsgShowHandDealtCard / LayCard / LostInfo):
    /// Suit 0..3 = Hearts, Spades, Clubs, Diamonds (Cn_Res STR_SH_CARD_SUIT_n),
    /// Value 0..12 = 2,3,...,10,J,Q,K,A (STR_SH_CARD_VALUE_n).
    /// Pure data (no server dependencies) so the hand evaluator can be unit tested on its own.
    /// </summary>
    public struct PokerCard
    {
        public byte Suit;
        public byte Value;

        public PokerCard(byte suit, byte value)
        {
            Suit = suit;
            Value = value;
        }

        /// <summary>0..51, unique per card.</summary>
        public int Id { get { return Suit * 13 + Value; } }

        public static PokerCard FromId(int id)
        {
            return new PokerCard((byte)(id / 13), (byte)(id % 13));
        }

        private const string Ranks = "23456789TJQKA";
        private const string Suits = "hscd";

        /// <summary>Parses "As", "Td", "2h" ... (rank then suit h/s/c/d).</summary>
        public static PokerCard Parse(string text)
        {
            if (text == null || text.Length != 2)
                throw new ArgumentException("bad card " + text);
            int v = Ranks.IndexOf(char.ToUpperInvariant(text[0]));
            int s = Suits.IndexOf(char.ToLowerInvariant(text[1]));
            if (v < 0 || s < 0)
                throw new ArgumentException("bad card " + text);
            return new PokerCard((byte)s, (byte)v);
        }

        public override string ToString()
        {
            if (Value > 12 || Suit > 3)
                return "??";
            return Ranks[Value].ToString() + Suits[Suit];
        }
    }

    /// <summary>
    /// Hand categories in the order of the client's STR_TEXAS_CARD_TYPE_0..9
    /// (H.Card, 1-Pair, 2-Pair, 3K, Straight, Flush, F.House, 4K, S.Flush, R.Flush).
    /// </summary>
    public enum PokerHandCategory : byte
    {
        HighCard = 0,
        OnePair = 1,
        TwoPair = 2,
        ThreeOfAKind = 3,
        Straight = 4,
        Flush = 5,
        FullHouse = 6,
        FourOfAKind = 7,
        StraightFlush = 8,
        RoyalFlush = 9,
        /// <summary>STR_TEXAS_CARD_TYPE_10 (blank): folded / no showdown.</summary>
        None = 10
    }

    /// <summary>
    /// Texas Hold'em hand evaluation: best 5 of 5..7 cards. The score is a single int that compares
    /// correctly (higher wins, equal = split): category in bits 20..23, then up to five rank nibbles.
    /// Suits never break ties.
    /// </summary>
    public static class PokerHandEvaluator
    {
        public static PokerHandCategory Category(int score)
        {
            return (PokerHandCategory)((score >> 20) & 0xF);
        }

        private static int Make(PokerHandCategory cat, int r1, int r2, int r3, int r4, int r5)
        {
            return ((int)cat << 20) | (r1 << 16) | (r2 << 12) | (r3 << 8) | (r4 << 4) | r5;
        }

        /// <summary>Highest straight in the rank bit mask (bit v = value v present); -1 if none. A-2-3-4-5 returns 3 (the five).</summary>
        private static int HighestStraight(int mask)
        {
            for (int high = 12; high >= 4; high--)
            {
                int need = 0x1F << (high - 4);
                if ((mask & need) == need)
                    return high;
            }
            // wheel: A,2,3,4,5 = bits 12,0,1,2,3
            if ((mask & 0x100F) == 0x100F)
                return 3;
            return -1;
        }

        public static int Evaluate(IList<PokerCard> cards)
        {
            if (cards == null || cards.Count < 5 || cards.Count > 7)
                throw new ArgumentException("Evaluate needs 5 to 7 cards");

            int[] countByValue = new int[13];
            int[] countBySuit = new int[4];
            int[] suitMask = new int[4];
            int allMask = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                var c = cards[i];
                if (c.Value > 12 || c.Suit > 3)
                    throw new ArgumentException("bad card");
                countByValue[c.Value]++;
                countBySuit[c.Suit]++;
                suitMask[c.Suit] |= 1 << c.Value;
                allMask |= 1 << c.Value;
            }

            // straight flush / flush
            int flushSuit = -1;
            for (int s = 0; s < 4; s++)
                if (countBySuit[s] >= 5)
                    flushSuit = s;
            if (flushSuit >= 0)
            {
                int sf = HighestStraight(suitMask[flushSuit]);
                if (sf >= 0)
                    return Make(sf == 12 ? PokerHandCategory.RoyalFlush : PokerHandCategory.StraightFlush, sf, 0, 0, 0, 0);
            }

            int quad = -1;
            int trip1 = -1, trip2 = -1;
            int pair1 = -1, pair2 = -1, pair3 = -1;
            for (int v = 12; v >= 0; v--)
            {
                switch (countByValue[v])
                {
                    case 4: if (quad < 0) quad = v; break;
                    case 3: if (trip1 < 0) trip1 = v; else if (trip2 < 0) trip2 = v; break;
                    case 2: if (pair1 < 0) pair1 = v; else if (pair2 < 0) pair2 = v; else if (pair3 < 0) pair3 = v; break;
                }
            }

            if (quad >= 0)
            {
                int kicker = HighestExcept(allMask, quad, -1, 1)[0];
                return Make(PokerHandCategory.FourOfAKind, quad, kicker, 0, 0, 0);
            }
            if (trip1 >= 0 && (trip2 >= 0 || pair1 >= 0))
            {
                int pairPart = Math.Max(trip2, pair1);
                return Make(PokerHandCategory.FullHouse, trip1, pairPart, 0, 0, 0);
            }
            if (flushSuit >= 0)
            {
                int[] top = TopBits(suitMask[flushSuit], 5);
                return Make(PokerHandCategory.Flush, top[0], top[1], top[2], top[3], top[4]);
            }
            int straight = HighestStraight(allMask);
            if (straight >= 0)
                return Make(PokerHandCategory.Straight, straight, 0, 0, 0, 0);
            if (trip1 >= 0)
            {
                int[] k = HighestExcept(allMask, trip1, -1, 2);
                return Make(PokerHandCategory.ThreeOfAKind, trip1, k[0], k[1], 0, 0);
            }
            if (pair1 >= 0 && pair2 >= 0)
            {
                // a third pair can only serve as the kicker
                int[] k = HighestExcept(allMask, pair1, pair2, 1);
                return Make(PokerHandCategory.TwoPair, pair1, pair2, k[0], 0, 0);
            }
            if (pair1 >= 0)
            {
                int[] k = HighestExcept(allMask, pair1, -1, 3);
                return Make(PokerHandCategory.OnePair, pair1, k[0], k[1], k[2], 0);
            }
            int[] h = TopBits(allMask, 5);
            return Make(PokerHandCategory.HighCard, h[0], h[1], h[2], h[3], h[4]);
        }

        /// <summary>Evaluates 2 hole cards + the board (3..5 cards).</summary>
        public static int Evaluate(PokerCard hole1, PokerCard hole2, IList<PokerCard> board)
        {
            var all = new List<PokerCard>(7);
            all.Add(hole1);
            all.Add(hole2);
            for (int i = 0; i < board.Count; i++)
                all.Add(board[i]);
            return Evaluate(all);
        }

        private static int[] TopBits(int mask, int count)
        {
            int[] r = new int[count];
            int n = 0;
            for (int v = 12; v >= 0 && n < count; v--)
                if ((mask & (1 << v)) != 0)
                    r[n++] = v;
            return r;
        }

        private static int[] HighestExcept(int mask, int ex1, int ex2, int count)
        {
            if (ex1 >= 0) mask &= ~(1 << ex1);
            if (ex2 >= 0) mask &= ~(1 << ex2);
            return TopBits(mask, count);
        }
    }

    /// <summary>52 card deck shuffled with a cryptographic RNG (Fisher-Yates).</summary>
    public class PokerDeck
    {
        private readonly List<PokerCard> cards = new List<PokerCard>(52);
        private int next;
        private static readonly System.Security.Cryptography.RandomNumberGenerator Rng = System.Security.Cryptography.RandomNumberGenerator.Create();

        public PokerDeck()
        {
            for (int i = 0; i < 52; i++)
                cards.Add(PokerCard.FromId(i));
            Shuffle();
        }

        /// <summary>Deck with a fixed order (tests): cards are dealt from the start of the list.</summary>
        public PokerDeck(IList<PokerCard> fixedOrder)
        {
            cards.AddRange(fixedOrder);
        }

        private static int NextInt(int exclusiveMax)
        {
            byte[] b = new byte[4];
            uint limit = uint.MaxValue - (uint.MaxValue % (uint)exclusiveMax);
            uint v;
            do
            {
                lock (Rng)
                    Rng.GetBytes(b);
                v = BitConverter.ToUInt32(b, 0);
            }
            while (v >= limit);
            return (int)(v % (uint)exclusiveMax);
        }

        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = NextInt(i + 1);
                var t = cards[i];
                cards[i] = cards[j];
                cards[j] = t;
            }
            next = 0;
        }

        public PokerCard Deal()
        {
            if (next >= cards.Count)
                throw new InvalidOperationException("deck is empty");
            return cards[next++];
        }
    }
}
