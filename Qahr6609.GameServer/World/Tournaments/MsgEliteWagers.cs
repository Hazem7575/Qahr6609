using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    /// <summary>
    /// Elite PK Tournament wagers ("Sponsor" in the client, dlgelitepkfinal + dlgsponsor).
    ///
    /// Client packets (from the 6609 client):
    ///   1064 MsgSponsor (client -> server, 24 bytes):
    ///        [4] ushort type (2 = Elite PK, 1 = Guild PK), [8] uint group (0-3), [12] uint fighter UID,
    ///        [16] uint second id (guild PK only), [20] uint number of wagers (1 wager = 100,000 silver).
    ///   1065 MsgSponsorInfo (server -> client):
    ///        [4] ushort group (0-3; 4 = guild PK), [6] byte 1 = clear the old list first, [8] uint count,
    ///        then count x { uint uid1, uint uid2, uint total on uid1, uint total on uid2 }.
    /// The client opens the wager window on every bracket list it receives (InitialList/UpdateList) for
    /// "TimeLeft" seconds, i.e. until the round's matches begin. Wagers are only taken on the top-8 rounds
    /// (the final bracket dialog), on a match that has not started yet.
    ///
    /// Settlement (pari-mutuel): when a match ends, the silver wagered on the loser is shared among the
    /// winner's backers in proportion to their stake, and each backer also gets the stake back. Backers of
    /// the loser lose their stake. A match that never took place (forfeit before it began) or a tournament
    /// that ends/restarts with open wagers is void: every stake is refunded.
    /// Open wagers are written to EliteWagers.txt; since the tournament itself does not survive a restart,
    /// wagers found there at start-up are refunded by mail.
    /// </summary>
    public static unsafe class MsgEliteWagers
    {
        public const long SilverPerWager = 100000;
        public const uint MaxWagersPerBet = 10000;//1,000,000,000 silver per wager
        public const ushort SponsorEliteType = 2;
        const long PayChunk = 1000000000;//money changes above 10B trip the anti-hack check, so pay in pieces

        class Bet
        {
            public uint BettorUID;
            public string BettorName;
            public int Group;
            public uint MatchID;
            public uint FighterUID;
            public long Amount;
        }

        static readonly object SyncRoot = new object();
        static readonly List<Bet> Bets = new List<Bet>();

        static string FilePath
        {
            get { return Program.ServerConfig.DbLocation + "\\EliteWagers.txt"; }
        }

        [PacketAttribute(GamePackets.ElitePKWager)]
        private static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                ushort type = stream.ReadUInt16();
                stream.ReadUInt16();
                uint group = stream.ReadUInt32();
                uint fighter = stream.ReadUInt32();
                stream.ReadUInt32();
                uint wagers = stream.ReadUInt32();
                if (type != SponsorEliteType)
                {
                    client.SendSysMesage("Guild PK Tournament wagers are not available.");
                    return;
                }
                PlaceWager(client, group, fighter, wagers);
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        /// <summary>The matches of the top-8 round that is currently being played in this group.</summary>
        static MsgEliteGroup.Match[] RoundMatches(MsgEliteGroup group)
        {
            Extensions.SafeDictionary<uint, MsgEliteGroup.Match> source = null;
            switch (group.State)
            {
                case MsgElitePKBrackets.GuiTyp.GUI_Top4Qualifier: source = group.Matches; break;
                case MsgElitePKBrackets.GuiTyp.GUI_Top2Qualifier: source = group.Top4Matches; break;
                case MsgElitePKBrackets.GuiTyp.GUI_Top3: source = group.ThreeQualiferMatch; break;
                case MsgElitePKBrackets.GuiTyp.GUI_Top1: source = group.FinalMatch; break;
            }
            if (source == null)
                return new MsgEliteGroup.Match[0];
            return source.GetValues().Where(m => m != null && m.Count == 2).ToArray();
        }

        static void PlaceWager(Client.GameClient client, uint groupIndex, uint fighterUID, uint wagers)
        {
            if (groupIndex >= (uint)MsgEliteTournament.GroupTyp.Count || MsgEliteTournament.EliteGroups == null)
                return;
            var group = MsgEliteTournament.EliteGroups[groupIndex];
            if (group == null || group.Proces != ProcesType.Alive)
            {
                client.SendSysMesage("You can only wager while the Elite PK Tournament top 8 are fighting.");
                return;
            }
            if (wagers == 0)
                return;
            if (wagers > MaxWagersPerBet)
            {
                client.SendSysMesage("You can place at most " + MaxWagersPerBet + " wagers at a time.");
                return;
            }
            MsgEliteGroup.Match match = RoundMatches(group).Where(m => m.Players.ContainsKey(fighterUID)).FirstOrDefault();
            if (match == null)
            {
                client.SendSysMesage("You can only wager on a contestant of the Elite PK Tournament top 8.");
                return;
            }
            if (match.Flag != MsgEliteGroup.Match.StatusFlag.AcceptingWagers || match.IsFinishd() || group.TimeLeft == 0)
            {
                client.SendSysMesage("You can`t wager on that match, as it has already started.");
                return;
            }
            if (match.Players.ContainsKey(client.Player.UID))
            {
                client.SendSysMesage("You can`t wager on your own match.");
                return;
            }
            long cost = (long)wagers * SilverPerWager;
            lock (SyncRoot)
            {
                if (Bets.Any(b => b.BettorUID == client.Player.UID && b.Group == (int)groupIndex && b.MatchID == match.ID && b.FighterUID != fighterUID))
                {
                    client.SendSysMesage("You have already wagered on the other contestant of this match.");
                    return;
                }
                if (client.Player.Money < cost)
                {
                    client.SendSysMesage("You don`t have enough silver!");
                    return;
                }
                client.Player.Money -= cost;
                Bet bet = Bets.Where(b => b.BettorUID == client.Player.UID && b.Group == (int)groupIndex && b.MatchID == match.ID && b.FighterUID == fighterUID).FirstOrDefault();
                if (bet == null)
                {
                    bet = new Bet();
                    bet.BettorUID = client.Player.UID;
                    bet.BettorName = client.Player.Name;
                    bet.Group = (int)groupIndex;
                    bet.MatchID = match.ID;
                    bet.FighterUID = fighterUID;
                    Bets.Add(bet);
                }
                bet.Amount += cost;
                SaveLocked();
            }
            Client.GameClient fighter;
            string fighterName = match.Players.TryGetValue(fighterUID, out fighter) && fighter.Player != null ? fighter.Player.Name : fighterUID.ToString();
            client.SendSysMesage("You wagered " + cost.ToString("N0") + " silver on " + fighterName + ".");
            SendList(group, null);
        }

        /// <summary>Sends the wager totals of the current round to one player, or to everybody when user is null.</summary>
        public static void SendList(MsgEliteGroup group, Client.GameClient user)
        {
            var matches = RoundMatches(group);
            List<uint[]> rows = new List<uint[]>();
            lock (SyncRoot)
            {
                foreach (var match in matches)
                {
                    var players = match.PlayersArray;
                    if (players.Length != 2)
                        continue;
                    uint uid1 = players[0].Player.UID, uid2 = players[1].Player.UID;
                    rows.Add(new uint[] { uid1, uid2, Total((int)group.GroupTyp, match.ID, uid1), Total((int)group.GroupTyp, match.ID, uid2) });
                }
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.Write((ushort)group.GroupTyp);
                stream.Write((byte)1);//clear the old list
                stream.Write((byte)0);
                stream.Write((uint)rows.Count);
                foreach (var row in rows)
                {
                    stream.Write(row[0]);
                    stream.Write(row[1]);
                    stream.Write(row[2]);
                    stream.Write(row[3]);
                }
                stream.Finalize(GamePackets.ElitePKWagersList);
                if (user != null)
                    user.Send(stream);
                else
                    Program.SendGlobalPackets.Enqueue(stream);
            }
        }

        static uint Total(int group, uint matchId, uint fighterUID)
        {
            long sum = 0;
            foreach (var bet in Bets)
                if (bet.Group == group && bet.MatchID == matchId && bet.FighterUID == fighterUID)
                    sum += bet.Amount;
            return (uint)Math.Min(sum, uint.MaxValue);
        }

        /// <summary>Called when a top-8 match ends. played = false when the match never started (void).</summary>
        public static void SettleMatch(MsgEliteGroup group, MsgEliteGroup.Match match, uint winnerUID, bool played)
        {
            List<Bet> bets;
            lock (SyncRoot)
            {
                bets = Bets.Where(b => b.Group == (int)group.GroupTyp && b.MatchID == match.ID).ToList();
                if (bets.Count == 0)
                    return;
                foreach (var bet in bets)
                    Bets.Remove(bet);
                SaveLocked();
            }
            Client.GameClient winner;
            string winnerName = match.Players.TryGetValue(winnerUID, out winner) && winner.Player != null ? winner.Player.Name : "the winner";
            long winPool = bets.Where(b => b.FighterUID == winnerUID).Sum(b => b.Amount);
            long losePool = bets.Where(b => b.FighterUID != winnerUID).Sum(b => b.Amount);
            foreach (var bet in bets)
            {
                if (!played)
                {
                    Pay(bet.BettorUID, bet.Amount, "Wager refunded", "The Elite PK match was void. Your wager of " + bet.Amount.ToString("N0") + " silver was refunded.");
                    continue;
                }
                if (bet.FighterUID != winnerUID)
                {
                    Client.GameClient online;
                    if (Database.Server.GamePoll.TryGetValue(bet.BettorUID, out online))
                        online.SendSysMesage(winnerName + " won the Elite PK match. You lost your wager of " + bet.Amount.ToString("N0") + " silver.");
                    continue;
                }
                long prize = bet.Amount + (long)((decimal)losePool * bet.Amount / winPool);
                Pay(bet.BettorUID, prize, "Wager won", winnerName + " won! You received " + prize.ToString("N0") + " silver for your wager.");
            }
            SendList(group, null);
        }

        /// <summary>Refunds every open wager of a group (tournament over / interrupted).</summary>
        public static void VoidGroup(MsgEliteGroup group)
        {
            List<Bet> bets;
            lock (SyncRoot)
            {
                bets = Bets.Where(b => b.Group == (int)group.GroupTyp).ToList();
                if (bets.Count == 0)
                    return;
                foreach (var bet in bets)
                    Bets.Remove(bet);
                SaveLocked();
            }
            foreach (var bet in bets)
                Pay(bet.BettorUID, bet.Amount, "Wager refunded", "The Elite PK match was void. Your wager of " + bet.Amount.ToString("N0") + " silver was refunded.");
        }

        static void Pay(uint uid, long amount, string subject, string text)
        {
            if (amount <= 0)
                return;
            Client.GameClient online;
            if (Database.Server.GamePoll.TryGetValue(uid, out online) && online.Player != null && online.FullLoading)
            {
                long left = amount;
                while (left > 0)
                {
                    long part = Math.Min(left, PayChunk);
                    online.Player.Money += part;
                    left -= part;
                }
                online.SendSysMesage(text);
                return;
            }
            Mail(uid, amount, subject, text);
        }

        static bool Mail(uint uid, long amount, string subject, string text)
        {
            string error;
            if (!Database.MailService.Send(uid, "Elite PK", subject, text, (ulong)amount, 0, 0, null, out error))
            {
                MyConsole.WriteLine("[EliteWagers] could not mail " + amount + " silver to " + uid + ": " + error);
                return false;
            }
            return true;
        }

        static void SaveLocked()
        {
            try
            {
                if (Bets.Count == 0)
                {
                    if (File.Exists(FilePath))
                        File.Delete(FilePath);
                    return;
                }
                List<string> lines = new List<string>();
                foreach (var bet in Bets)
                    lines.Add(bet.BettorUID + "," + bet.Group + "," + bet.MatchID + "," + bet.FighterUID + "," + bet.Amount + "," + bet.BettorName);
                File.WriteAllLines(FilePath, lines.ToArray());
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        /// <summary>
        /// Start-up: the tournament does not survive a restart, so every wager still open in EliteWagers.txt
        /// is refunded to the player's mailbox.
        /// </summary>
        public static void RefundAfterRestart()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return;
                List<string> failed = new List<string>();
                int refunded = 0;
                foreach (string line in File.ReadAllLines(FilePath))
                {
                    string[] f = line.Split(new char[] { ',' }, 6);
                    uint uid;
                    long amount;
                    if (f.Length < 5 || !uint.TryParse(f[0], out uid) || !long.TryParse(f[4], out amount) || amount <= 0)
                        continue;
                    if (Mail(uid, amount, "Wager refunded", "The Elite PK Tournament was interrupted. Your wager of " + amount.ToString("N0") + " silver was refunded."))
                        refunded++;
                    else
                        failed.Add(line);
                }
                if (failed.Count == 0)
                    File.Delete(FilePath);
                else
                    File.WriteAllLines(FilePath, failed.ToArray());
                if (refunded > 0)
                    MyConsole.WriteLine("[EliteWagers] refunded " + refunded + " open Elite PK wagers by mail.");
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
    }
}
