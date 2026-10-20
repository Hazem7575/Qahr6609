using System;
using System.Collections.Generic;
using System.Linq;

namespace Conquer6609.Game.MsgServer.Poker
{
    /// <summary>Action codes of MsgShowHandCallAction / the ActivePlayer allowed-actions mask (client CMsgShowHandCallAction: 1,2,4,8,16,32).</summary>
    [Flags]
    public enum PokerAction : ushort
    {
        None = 0,
        Bet = 1,
        Call = 2,
        Fold = 4,
        Check = 8,
        Raise = 16,
        AllIn = 32
    }

    /// <summary>Table state shown in the table tooltip (client STR_TEXAS_GAME_INFO_STR3_1..6, NpcInfo byte +0x2A).</summary>
    public enum PokerStage : byte
    {
        Unopened = 0,
        PreFlop = 1,
        Flop = 2,
        Turn = 3,
        River = 4,
        Showdown = 5
    }

    /// <summary>One player of the hand in progress (pure state, no network objects).</summary>
    public class PokerHandPlayer
    {
        public uint UID;
        public int Seat;
        /// <summary>Chips still behind (not yet put in the pot).</summary>
        public long Stack;
        /// <summary>Chips put in during the current betting round.</summary>
        public long RoundBet;
        /// <summary>Chips put in during the whole hand.</summary>
        public long TotalBet;
        public bool Folded;
        public bool AllIn;
        /// <summary>Has acted since the last full raise (or the start of the round).</summary>
        public bool Acted;
        /// <summary>False after a short all-in raise for players that had already acted (they may only call/fold).</summary>
        public bool CanRaise = true;
        public PokerCard Card1, Card2;
        /// <summary>Filled by Settle(): chips received from the pot(s), including an uncalled bet returned.</summary>
        public long Won;
        /// <summary>Filled by Settle() at a showdown: hand score (PokerHandEvaluator), -1 otherwise.</summary>
        public int Score = -1;

        public bool CanAct { get { return !Folded && !AllIn; } }
        public long Net { get { return Won - TotalBet; } }
    }

    /// <summary>What one call of PokerHand.Act really did (for the CallAction broadcast and the money debit).</summary>
    public struct PokerActResult
    {
        public bool Valid;
        public PokerAction Action;
        /// <summary>Chips added by this action (debit this amount from the player).</summary>
        public long Added;
        public uint UID;
    }

    /// <summary>
    /// One Texas Hold'em hand: blinds, dealing, the four betting rounds (fixed limit or no limit), side pots,
    /// showdown and pot distribution. Pure logic: the table (PokerTable) moves money and sends packets.
    /// Amounts for Bet/Raise are "raise to" values (the player's total bet in the current round), the way the
    /// client's raise slider and MsgShowHandLostInfo (min raise = 2 x current bet) work.
    /// </summary>
    public class PokerHand
    {
        public readonly bool NoLimit;
        public readonly long BigBlind;
        public readonly long SmallBlind;
        /// <summary>Ante every player posts before the blinds (championships; 0 = none). Antes are dead money: they count in
        /// TotalBet (pots, side pots) but not in RoundBet (the bet to call).</summary>
        public readonly long Ante;
        /// <summary>Fixed limit: at most this many bets per round (bet + 3 raises).</summary>
        public const int LimitBetCap = 4;

        public readonly List<PokerHandPlayer> Players;
        public readonly List<PokerCard> Board = new List<PokerCard>(5);
        public readonly PokerDeck Deck;

        public int DealerIndex { get; private set; }
        public int SmallBlindIndex { get; private set; }
        public int BigBlindIndex { get; private set; }
        /// <summary>Index in Players of the player to act, -1 when nobody has to act.</summary>
        public int ActiveIndex { get; private set; }
        public PokerStage Stage { get; private set; }
        public long CurrentBet { get; private set; }
        public long MinRaise { get; private set; }
        public int BetsThisRound { get; private set; }
        public bool Settled { get; private set; }

        /// <param name="players">Players in seat order (clockwise). Stacks must be &gt; 0.</param>
        /// <param name="dealerIndex">Index in players of the button.</param>
        public PokerHand(IList<PokerHandPlayer> players, int dealerIndex, long bigBlind, bool noLimit, PokerDeck deck = null, long ante = 0)
        {
            if (ante < 0)
                throw new ArgumentException("ante must not be negative");
            if (players == null || players.Count < 2)
                throw new ArgumentException("a hand needs at least 2 players");
            if (bigBlind < 2)
                throw new ArgumentException("big blind must be at least 2");
            Players = new List<PokerHandPlayer>(players);
            DealerIndex = ((dealerIndex % Players.Count) + Players.Count) % Players.Count;
            BigBlind = bigBlind;
            SmallBlind = bigBlind / 2;
            NoLimit = noLimit;
            Ante = ante;
            Deck = deck ?? new PokerDeck();
            ActiveIndex = -1;
            Stage = PokerStage.Unopened;
        }

        public PokerHandPlayer Get(uint uid)
        {
            for (int i = 0; i < Players.Count; i++)
                if (Players[i].UID == uid)
                    return Players[i];
            return null;
        }

        public PokerHandPlayer Active
        {
            get { return ActiveIndex >= 0 && ActiveIndex < Players.Count ? Players[ActiveIndex] : null; }
        }

        public int NotFoldedCount
        {
            get { int n = 0; foreach (var p in Players) if (!p.Folded) n++; return n; }
        }

        public long Pot
        {
            get { long n = 0; foreach (var p in Players) n += p.TotalBet; return n; }
        }

        private int Next(int index)
        {
            return (index + 1) % Players.Count;
        }

        /// <summary>Next player after <paramref name="from"/> (exclusive) that can still act, -1 if none.</summary>
        private int NextCanAct(int from)
        {
            int i = from;
            for (int n = 0; n < Players.Count; n++)
            {
                i = Next(i);
                if (Players[i].CanAct)
                    return i;
            }
            return -1;
        }

        private long Put(PokerHandPlayer p, long amount)
        {
            if (amount > p.Stack)
                amount = p.Stack;
            if (amount < 0)
                amount = 0;
            p.Stack -= amount;
            p.RoundBet += amount;
            p.TotalBet += amount;
            if (p.Stack == 0)
                p.AllIn = true;
            return amount;
        }

        /// <summary>Posts the blinds and deals two cards to everybody. Returns the blind amounts really posted.</summary>
        public void Start(out long smallBlindPosted, out long bigBlindPosted)
        {
            if (Stage != PokerStage.Unopened)
                throw new InvalidOperationException("hand already started");
            foreach (var p in Players)
            {
                if (p.Stack <= 0)
                    throw new InvalidOperationException("player without chips in the hand");
            }
            if (Players.Count == 2)
            {
                // heads-up: the button posts the small blind and acts first before the flop
                SmallBlindIndex = DealerIndex;
                BigBlindIndex = Next(DealerIndex);
            }
            else
            {
                SmallBlindIndex = Next(DealerIndex);
                BigBlindIndex = Next(SmallBlindIndex);
            }
            Stage = PokerStage.PreFlop;
            if (Ante > 0)
            {
                // antes first (dead money, not part of the bet to call); a short stack is all-in with what he has
                foreach (var p in Players)
                {
                    long a = Math.Min(Ante, p.Stack);
                    p.Stack -= a;
                    p.TotalBet += a;
                    if (p.Stack == 0)
                        p.AllIn = true;
                }
            }
            smallBlindPosted = Put(Players[SmallBlindIndex], SmallBlind);
            bigBlindPosted = Put(Players[BigBlindIndex], BigBlind);
            CurrentBet = BigBlind;
            MinRaise = BigBlind;
            BetsThisRound = 1;

            // two cards each, one at a time starting left of the button
            int start = Next(DealerIndex);
            for (int round = 0; round < 2; round++)
            {
                int i = start;
                for (int n = 0; n < Players.Count; n++)
                {
                    if (round == 0) Players[i].Card1 = Deck.Deal();
                    else Players[i].Card2 = Deck.Deal();
                    i = Next(i);
                }
            }
            ActiveIndex = NextCanAct(BigBlindIndex);
            if (IsRoundComplete())
                ActiveIndex = -1;
        }

        public long ToCall(PokerHandPlayer p)
        {
            long c = CurrentBet - p.RoundBet;
            return c < 0 ? 0 : c;
        }

        /// <summary>Fixed limit bet unit: big blind before the turn, double big blind on turn and river.</summary>
        public long LimitUnit
        {
            get { return Stage >= PokerStage.Turn ? BigBlind * 2 : BigBlind; }
        }

        private bool MayRaise(PokerHandPlayer p)
        {
            if (!p.CanRaise)
                return false;
            if (!NoLimit && BetsThisRound >= LimitBetCap)
                return false;
            // somebody else must still be able to respond, with more chips than the current bet
            // (against players that can only call all-in a raise means nothing)
            foreach (var o in Players)
                if (o != p && o.CanAct && o.RoundBet + o.Stack > CurrentBet)
                    return true;
            return false;
        }

        /// <summary>Allowed actions mask for MsgShowHandActivePlayer.</summary>
        public PokerAction AllowedActions(PokerHandPlayer p)
        {
            if (p == null || !p.CanAct)
                return PokerAction.None;
            PokerAction a = PokerAction.Fold;
            long toCall = ToCall(p);
            bool mayRaise = MayRaise(p);
            // fixed limit: the raise goes to one unit above the bet (at most the effective stack); it is an all-in
            // when that is everything the player has
            bool limitRaiseAllIn = !NoLimit && Math.Min(CurrentBet + LimitUnit, MaxRaiseTo(p)) >= p.RoundBet + p.Stack;
            if (toCall == 0)
            {
                a |= PokerAction.Check;
                if (mayRaise)
                {
                    if (limitRaiseAllIn) a |= PokerAction.AllIn;
                    else a |= CurrentBet == 0 ? PokerAction.Bet : PokerAction.Raise;
                }
            }
            else
            {
                if (p.Stack > toCall)
                {
                    a |= PokerAction.Call;
                    if (mayRaise)
                    {
                        if (limitRaiseAllIn) a |= PokerAction.AllIn;
                        else a |= PokerAction.Raise;
                    }
                }
                else
                    a |= PokerAction.AllIn;// can only call all-in (or fold)
            }
            if (NoLimit && p.Stack > 0 && (mayRaise || p.Stack <= toCall))
                a |= PokerAction.AllIn;
            return a;
        }

        /// <summary>Smallest "raise to" for Bet/Raise (no limit), capped at the player's all-in.</summary>
        public long MinRaiseTo(PokerHandPlayer p)
        {
            long min = CurrentBet == 0 ? BigBlind : CurrentBet + MinRaise;
            if (!NoLimit)
                min = CurrentBet + LimitUnit;
            return Math.Min(min, MaxRaiseTo(p));
        }

        /// <summary>
        /// Largest "raise to": the player's all-in (round bet + stack), capped at the effective stack = the most any other
        /// player still in the hand could put in this round. Chips above that could never be called (they would only be
        /// returned at the end); without the cap a rich player's "all-in" moved his whole silver into the pot for the hand.
        /// </summary>
        public long MaxRaiseTo(PokerHandPlayer p)
        {
            long own = p.RoundBet + p.Stack;
            long others = CurrentBet;
            foreach (var o in Players)
                if (o != p && !o.Folded && o.RoundBet + o.Stack > others)
                    others = o.RoundBet + o.Stack;
            return Math.Min(own, others);
        }

        /// <summary>
        /// The active player acts. <paramref name="raiseTo"/> is used for no limit Bet/Raise only.
        /// Invalid requests are corrected the way the table would (check when facing a bet -> ignored,
        /// raise below the minimum -> minimum, raise beyond the stack -> all-in).
        /// </summary>
        public PokerActResult Act(uint uid, PokerAction action, long raiseTo)
        {
            var r = new PokerActResult { UID = uid };
            var p = Active;
            if (p == null || p.UID != uid || Settled)
                return r;
            long toCall = ToCall(p);
            long before = p.RoundBet;
            switch (action)
            {
                case PokerAction.Fold:
                    p.Folded = true;
                    r.Action = PokerAction.Fold;
                    break;
                case PokerAction.Check:
                    if (toCall > 0)
                        return r;
                    r.Action = PokerAction.Check;
                    break;
                case PokerAction.Call:
                    if (toCall == 0)
                    {
                        r.Action = PokerAction.Check;
                        break;
                    }
                    Put(p, toCall);
                    r.Action = p.AllIn ? PokerAction.AllIn : PokerAction.Call;
                    break;
                case PokerAction.Bet:
                case PokerAction.Raise:
                    {
                        if (!MayRaise(p))
                        {
                            // cannot raise any more: treat as a call/check
                            if (toCall == 0) { r.Action = PokerAction.Check; break; }
                            Put(p, toCall);
                            r.Action = p.AllIn ? PokerAction.AllIn : PokerAction.Call;
                            break;
                        }
                        long target = NoLimit ? raiseTo : CurrentBet + LimitUnit;
                        long min = CurrentBet == 0 ? BigBlind : CurrentBet + MinRaise;
                        if (!NoLimit)
                            min = CurrentBet + LimitUnit;
                        if (target < min)
                            target = min;
                        long max = MaxRaiseTo(p);
                        if (target >= max)
                            target = max;
                        if (target <= CurrentBet)
                        {
                            // cannot even call fully -> all-in call
                            Put(p, target - p.RoundBet);
                            r.Action = p.AllIn ? PokerAction.AllIn : PokerAction.Call;
                            break;
                        }
                        Put(p, target - p.RoundBet);
                        r.Action = p.AllIn ? PokerAction.AllIn : (CurrentBet == 0 ? PokerAction.Bet : PokerAction.Raise);
                        break;
                    }
                case PokerAction.AllIn:
                    if (!MayRaise(p))
                    {
                        // betting not reopened for him (short all-in), bet cap reached or nobody left to respond:
                        // all-in may only call (an all-in for less when the stack does not cover the call)
                        if (toCall == 0) { r.Action = PokerAction.Check; break; }
                        Put(p, toCall);
                        r.Action = p.AllIn ? PokerAction.AllIn : PokerAction.Call;
                        break;
                    }
                    if (!NoLimit)
                    {
                        // fixed limit: never more than one bet unit above the call (nor above the effective stack)
                        Put(p, Math.Min(CurrentBet + LimitUnit, MaxRaiseTo(p)) - p.RoundBet);
                        r.Action = p.AllIn ? PokerAction.AllIn : (CurrentBet == 0 ? PokerAction.Bet : PokerAction.Raise);
                        break;
                    }
                    // no limit: everything he has, up to the effective stack (see MaxRaiseTo)
                    Put(p, MaxRaiseTo(p) - p.RoundBet);
                    r.Action = p.AllIn ? PokerAction.AllIn : (CurrentBet == 0 ? PokerAction.Bet : PokerAction.Raise);
                    break;
                default:
                    return r;
            }
            r.Valid = true;
            r.Added = p.RoundBet - before;
            p.Acted = true;
            OnBetChanged(p);
            AdvanceActive();
            return r;
        }

        /// <summary>A player leaves / disconnects: folds out of turn. Returns true if he was in the hand and not folded yet.</summary>
        public bool ForceFold(uint uid)
        {
            var p = Get(uid);
            if (p == null || p.Folded || Settled)
                return false;
            bool wasActive = Active == p;
            p.Folded = true;
            p.Acted = true;
            if (wasActive)
                AdvanceActive();
            else if (IsRoundComplete())
                ActiveIndex = -1;
            return true;
        }

        /// <summary>Re-evaluates who has to act (after a stack was cut down outside the hand, e.g. to 0 = all-in).</summary>
        public void Refresh()
        {
            if (Settled)
                return;
            var a = Active;
            if (a == null || a.CanAct)
                return;
            AdvanceActive();
        }

        private void OnBetChanged(PokerHandPlayer p)
        {
            if (p.RoundBet <= CurrentBet)
                return;
            long increment = p.RoundBet - CurrentBet;
            bool fullRaise = NoLimit ? increment >= MinRaise : increment >= LimitUnit;
            CurrentBet = p.RoundBet;
            foreach (var o in Players)
            {
                if (o == p || !o.CanAct)
                    continue;
                if (fullRaise)
                    o.CanRaise = true;
                else if (o.Acted)
                    o.CanRaise = false;// short all-in does not reopen the betting for him
                o.Acted = false;
            }
            if (fullRaise)
            {
                if (NoLimit)
                    MinRaise = increment;
                BetsThisRound++;
            }
        }

        private void AdvanceActive()
        {
            if (IsRoundComplete())
            {
                ActiveIndex = -1;
                return;
            }
            int from = ActiveIndex < 0 ? DealerIndex : ActiveIndex;
            int i = from;
            for (int n = 0; n < Players.Count; n++)
            {
                i = Next(i);
                var o = Players[i];
                if (o.CanAct && (!o.Acted || o.RoundBet < CurrentBet))
                {
                    ActiveIndex = i;
                    return;
                }
            }
            ActiveIndex = -1;
        }

        /// <summary>The current betting round is over (or the whole hand: only one player left).</summary>
        public bool IsRoundComplete()
        {
            if (NotFoldedCount <= 1)
                return true;
            int canAct = 0;
            foreach (var p in Players)
            {
                if (!p.CanAct)
                    continue;
                canAct++;
                if (!p.Acted || p.RoundBet < CurrentBet)
                {
                    // a lone player who already matches what the others (all-in) put in has nobody to play against
                    // (also when a big blind all-in for less leaves the current bet above anything really posted)
                    if (OthersAllInOrFolded(p) && p.RoundBet >= MaxOtherRoundBet(p))
                        continue;
                    return false;
                }
            }
            return true;
        }

        private long MaxOtherRoundBet(PokerHandPlayer p)
        {
            long max = 0;
            foreach (var o in Players)
                if (o != p && !o.Folded && o.RoundBet > max)
                    max = o.RoundBet;
            return max;
        }

        private bool OthersAllInOrFolded(PokerHandPlayer p)
        {
            foreach (var o in Players)
                if (o != p && o.CanAct)
                    return false;
            return true;
        }

        /// <summary>The hand is decided without more betting (one player left, or showdown reached).</summary>
        public bool IsHandOver
        {
            get { return NotFoldedCount <= 1 || (Stage == PokerStage.River && IsRoundComplete()) || Stage == PokerStage.Showdown; }
        }

        /// <summary>No more betting is possible in later rounds (everybody but at most one player is all-in): run the board out.</summary>
        public bool IsRunOut
        {
            get
            {
                int canAct = 0;
                foreach (var p in Players)
                    if (p.CanAct) canAct++;
                return NotFoldedCount > 1 && canAct <= 1;
            }
        }

        /// <summary>Deals the next street (flop 3 / turn 1 / river 1) after a completed round. Returns the new cards.</summary>
        public List<PokerCard> NextStreet()
        {
            if (!IsRoundComplete() || NotFoldedCount <= 1)
                throw new InvalidOperationException("betting round not finished");
            var dealt = new List<PokerCard>();
            switch (Stage)
            {
                case PokerStage.PreFlop:
                    Deck.Deal();// burn
                    for (int i = 0; i < 3; i++) dealt.Add(Deck.Deal());
                    Stage = PokerStage.Flop;
                    break;
                case PokerStage.Flop:
                    Deck.Deal();
                    dealt.Add(Deck.Deal());
                    Stage = PokerStage.Turn;
                    break;
                case PokerStage.Turn:
                    Deck.Deal();
                    dealt.Add(Deck.Deal());
                    Stage = PokerStage.River;
                    break;
                default:
                    throw new InvalidOperationException("no street after " + Stage);
            }
            Board.AddRange(dealt);
            CurrentBet = 0;
            MinRaise = BigBlind;
            BetsThisRound = 0;
            foreach (var p in Players)
            {
                p.RoundBet = 0;
                p.Acted = false;
                p.CanRaise = true;
            }
            ActiveIndex = -1;
            if (!IsRoundComplete())
                ActiveIndex = NextCanAct(DealerIndex);
            return dealt;
        }

        /// <summary>
        /// Ends the hand: returns an uncalled bet, builds the main/side pots from the total contributions,
        /// evaluates the hands still in (if more than one) and fills Won/Score. Odd chips go to the first winner
        /// left of the button. Sum of Won always equals the sum of TotalBet.
        /// </summary>
        public void Settle()
        {
            if (Settled)
                return;
            Settled = true;
            ActiveIndex = -1;
            var live = Players.Where(p => !p.Folded).ToList();
            if (live.Count > 1 && Board.Count == 5)
                Stage = PokerStage.Showdown;

            foreach (var p in Players)
            {
                p.Won = 0;
                p.Score = -1;
            }
            if (live.Count == 0)
                return;// cannot happen: the last player can never fold

            // contributions still to distribute
            var left = new Dictionary<PokerHandPlayer, long>();
            foreach (var p in Players)
                left[p] = p.TotalBet;

            // uncalled bet: the top contributor (still in) gets back what nobody matched
            var top = Players.OrderByDescending(p => p.TotalBet).ToList();
            if (top.Count > 1 && !top[0].Folded && top[0].TotalBet > top[1].TotalBet)
            {
                long back = top[0].TotalBet - top[1].TotalBet;
                top[0].Won += back;
                left[top[0]] -= back;
            }

            if (live.Count == 1)
            {
                long all = 0;
                foreach (var p in Players)
                {
                    all += left[p];
                    left[p] = 0;
                }
                live[0].Won += all;
                return;
            }

            if (Board.Count == 5)
                foreach (var p in live)
                    p.Score = PokerHandEvaluator.Evaluate(p.Card1, p.Card2, Board);

            // seat order starting left of the button (odd chips)
            var order = new List<PokerHandPlayer>();
            for (int n = 1; n <= Players.Count; n++)
                order.Add(Players[(DealerIndex + n) % Players.Count]);

            var levels = live.Select(p => left[p]).Where(v => v > 0).Distinct().OrderBy(v => v).ToList();
            long prev = 0;
            PokerHandPlayer[] lastWinners = null;
            foreach (long level in levels)
            {
                long pot = 0;
                foreach (var p in Players)
                {
                    long c = left[p];
                    pot += Math.Max(0, Math.Min(c, level) - prev);
                }
                var eligible = live.Where(p => left[p] >= level).ToList();
                if (pot > 0 && eligible.Count > 0)
                {
                    int best = eligible.Max(p => p.Score);
                    var winners = order.Where(p => eligible.Contains(p) && p.Score == best).ToArray();
                    long share = pot / winners.Length;
                    long odd = pot - share * winners.Length;
                    foreach (var w in winners)
                        w.Won += share;
                    for (int i = 0; i < odd; i++)
                        winners[i % winners.Length].Won += 1;
                    lastWinners = winners;
                }
                prev = level;
            }
            // folded chips above the highest live level (dead money) go to the last pot's winners
            long rest = 0;
            foreach (var p in Players)
                rest += Math.Max(0, left[p] - prev);
            if (rest > 0)
            {
                if (lastWinners == null)
                    lastWinners = new[] { order.First(p => !p.Folded) };
                long share = rest / lastWinners.Length;
                long odd = rest - share * lastWinners.Length;
                foreach (var w in lastWinners)
                    w.Won += share;
                for (int i = 0; i < odd; i++)
                    lastWinners[i % lastWinners.Length].Won += 1;
            }
        }
    }
}
