using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.Dice
{
    /// <summary>The table: rounds, bets, dice, payouts, the files. Every entry point takes the one lock.
    /// Cells (as the client sends them): 0 Small, 1 Big, 2..7 triple 1..6, 8..21 sum 4..17. Money type 0 = silver
    /// (the only one Dice.lua opens). Silver is taken when a bet is accepted and paid back (bet x multiplier) at the result.</summary>
    public static class DiceGame
    {
        public const int Cells = 22;
        const long PayStep = 1000000000;// Money changes of 10,000,000,000+ at once are treated as a hack by Player.Money

        sealed class Stake
        {
            public uint Uid;
            public string Name;
            public readonly long[] Cells = new long[DiceGame.Cells];
            public long Total;
            public int Count;
            public DateTime Last;
        }

        public enum Phase { Closed, Betting, Result }

        static readonly object sync = new object();
        static DiceConfig cfg;
        static string dir;
        static Action<string> log = s => { };
        static bool enabled, shutDown;
        static Phase phase = Phase.Closed;
        static DateTime phaseEnd;
        static long round;                                       // yyyyMMddHHmmss of the round start
        static readonly Dictionary<uint, Stake> stakes = new Dictionary<uint, Stake>();
        static readonly long[] cellTotals = new long[Cells];
        static readonly HashSet<uint> watchers = new HashSet<uint>();// window shown (join), not closed
        static readonly Dictionary<uint, DateTime> lastRequest = new Dictionary<uint, DateTime>();
        // the round whose own bets the client holds in its own-bet list (stake total + Council betting box). The client
        // empties that list only on its close button and after a result, never on join / Refresh: resending would double it.
        static readonly Dictionary<uint, long> listRound = new Dictionary<uint, long>();
        static readonly Dictionary<uint, KeyValuePair<long, string>> owed = new Dictionary<uint, KeyValuePair<long, string>>();
        static readonly List<KeyValuePair<long, int[]>> history = new List<KeyValuePair<long, int[]>>();// newest first
        static int[] forced;                                     // GM test: the next dice
        static int[] lastDice;
        static long stakedTotal, paidTotal;                      // since the module loaded

        static string BetsFile { get { return Path.Combine(dir, "bets.txt"); } }
        static string OwedFile { get { return Path.Combine(dir, "owed.txt"); } }
        static string HistoryFile { get { return Path.Combine(dir, "history.txt"); } }
        static string StateFile { get { return Path.Combine(dir, "state.txt"); } }
        static string WatchersFile { get { return Path.Combine(dir, "watchers.txt"); } }

        // ------------------------------------------------------------------ life cycle

        public static void Start(DiceConfig config, string dataPath, Action<string> logger)
        {
            lock (sync)
            {
                cfg = config;
                dir = dataPath;
                log = logger ?? (s => { });
                shutDown = false;
                LoadOwed();
                LoadHistory();
                // bets of a round that never ended (crash / kill): back to their owners
                int lost = RecoverBets();
                enabled = cfg.Enabled;
                string st = ReadState();
                if (st == "off") enabled = false;
                else if (st == "on") enabled = true;
                phase = Phase.Closed;
                phaseEnd = DateTime.Now.AddSeconds(2);
                // the watchers of the previous instance (hot reload): they keep getting the rounds
                try
                {
                    if (File.Exists(WatchersFile))
                    {
                        foreach (string l in File.ReadAllLines(WatchersFile)) { uint u; if (uint.TryParse(l.Trim(), out u)) watchers.Add(u); }
                        File.Delete(WatchersFile);
                    }
                }
                catch { }
                if (lost > 0) log("recovered " + lost + " unsettled bet(s) of an old round as refunds");
                PayOwedOnline();
            }
        }

        public static void ApplyConfig(DiceConfig config)
        {
            lock (sync) { cfg = config; }
        }

        /// <summary>Hot reload / server stop: the open round is refunded (owed.txt; online players are paid by the next
        /// OnLoad, the others at their next login), so nothing is lost and nothing is paid twice.</summary>
        public static void Stop()
        {
            lock (sync)
            {
                if (shutDown) return;
                shutDown = true;
                if (stakes.Count > 0)
                {
                    foreach (var s in stakes.Values) AddOwed(s.Uid, s.Name, s.Total);
                    log("round " + round + " stopped by a reload/shutdown: " + stakes.Count + " player(s) refunded through owed.txt");
                    stakes.Clear();
                    Array.Clear(cellTotals, 0, Cells);
                    SaveOwed();
                }
                DeleteBets();
                try { File.WriteAllLines(WatchersFile, watchers.Select(u => u.ToString())); } catch { }
                phase = Phase.Closed;
            }
        }

        // ------------------------------------------------------------------ rounds

        public static void Tick()
        {
            lock (sync)
            {
                if (shutDown || cfg == null) return;
                DateTime now = DateTime.Now;
                if (!enabled)
                {
                    if (phase != Phase.Closed) phase = Phase.Closed;
                    return;
                }
                if (now < phaseEnd) return;
                switch (phase)
                {
                    case Phase.Closed:
                    case Phase.Result:
                        NewRound(now);
                        break;
                    case Phase.Betting:
                        Roll(now);
                        break;
                }
            }
        }

        static void NewRound(DateTime now)
        {
            phase = Phase.Betting;
            phaseEnd = now.AddSeconds(cfg.BetSeconds);
            round = long.Parse(now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));
            stakes.Clear();
            Array.Clear(cellTotals, 0, Cells);
            DeleteBets();
            var pkt = new DiceOpt { Type = 2, Param = 0, Amount = cfg.BetSeconds + 1 };
            SendTo(Online(watchers), pkt);
        }

        static int Die()
        {
            return RandomNumberGenerator.GetInt32(1, 7);
        }

        static void Roll(DateTime now)
        {
            int[] d = forced ?? new[] { Die(), Die(), Die() };
            forced = null;
            lastDice = d;
            int sum = d[0] + d[1] + d[2];
            bool triple = d[0] == d[1] && d[1] == d[2];

            // multiplier of every cell for these dice (0 = lost)
            var mult = new int[Cells];
            bool smallBigLose = triple && cfg.TripleLosesSmallBig;
            if (!smallBigLose && sum >= 4 && sum <= 10) mult[0] = cfg.Multiplier(0);
            if (!smallBigLose && sum >= 11 && sum <= 17) mult[1] = cfg.Multiplier(1);
            if (triple) mult[1 + d[0]] = cfg.Multiplier(1 + d[0]);
            if (sum >= 4 && sum <= 17) mult[sum + 4] = cfg.Multiplier(sum + 4);

            phase = Phase.Result;
            phaseEnd = now.AddSeconds(cfg.ResultSeconds);
            long stamp = round;// the Lottery window shows (v / 10000) % 100 : (v / 100) % 100 = the round's start HH:mm
            history.Insert(0, new KeyValuePair<long, int[]>(stamp, d));
            while (history.Count > cfg.HistoryCount) history.RemoveAt(history.Count - 1);
            SaveHistory();

            // the round is settled from here on: no refund of it after a crash
            var bets = stakes.Values.ToList();
            stakes.Clear();
            Array.Clear(cellTotals, 0, Cells);
            DeleteBets();

            var audience = Online(watchers);
            foreach (var s in bets)
            {
                Client.GameClient c;
                if (TryOnline(s.Uid, out c) && !audience.Contains(c)) audience.Add(c);
            }
            SendTo(audience, new DiceOpt { Type = 2, Param = 1, Value = (uint)(d[0] + 10 * d[1] + 100 * d[2]) });

            long staked = 0, paid = 0;
            bool owedChanged = false;
            foreach (var s in bets)
            {
                var win = new DiceList { Type = 2 };
                long total = 0;
                for (int cell = 0; cell < Cells; cell++)
                    if (s.Cells[cell] > 0 && mult[cell] > 0)
                    {
                        long w = s.Cells[cell] * mult[cell];
                        total += w;
                        win.Items.Add(new DiceBetInfo { Cell = (uint)cell, Value = 0, Amount = w });
                    }
                staked += s.Total;
                paid += total;
                Client.GameClient c;
                bool online = TryOnline(s.Uid, out c);
                if (online)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        c.Send(total > 0 ? DicePackets.Create(stream, win) : DicePackets.Create(stream, new DiceList { Type = 3 }));
                    }
                }
                if (total > 0)
                {
                    if (online) Credit(c, total);
                    else { AddOwed(s.Uid, s.Name, total); owedChanged = true; }
                    if (cfg.AnnounceWinAbove > 0 && total >= cfg.AnnounceWinAbove)
                        World(s.Name + " won " + total.ToString("N0", CultureInfo.InvariantCulture) + " silver at the dice table (" + d[0] + "-" + d[1] + "-" + d[2] + ")!");
                }
                log("round " + round + " " + s.Name + "(" + s.Uid + ") bet " + s.Total + " [" + Describe(s.Cells) + "] won " + total + (online ? "" : " (offline, owed)"));
            }
            if (owedChanged) SaveOwed();
            stakedTotal += staked;
            paidTotal += paid;
            log("round " + round + " dice " + d[0] + "-" + d[1] + "-" + d[2] + " sum " + sum + (triple ? " triple" : "") + ": " + bets.Count + " player(s), staked " + staked + ", paid " + paid);
        }

        // ------------------------------------------------------------------ client packets

        public static void HandleOpt(Client.GameClient client, DiceOpt msg)
        {
            lock (sync)
            {
                if (shutDown || cfg == null) return;
                uint uid = client.Player.UID;
                switch (msg.Type)
                {
                    case 0:// window shown
                        if (Throttled(uid)) return;
                        watchers.Add(uid);
                        log("join " + client.Player.Name + "(" + uid + ")");
                        if (!enabled)
                        {
                            client.SendSysMesage("The dice table is closed right now.");
                            return;
                        }
                        if (phase == Phase.Betting)
                        {
                            int left = (int)Math.Ceiling((phaseEnd - DateTime.Now).TotalSeconds);
                            Send(client, new DiceOpt { Type = 2, Param = 0, Amount = Math.Max(0, left) + 1 });
                            SendTable(client);
                        }
                        break;
                    case 1:
                        PlaceBet(client, (int)Math.Min(msg.Param, 1000), msg.Value, msg.Amount);
                        break;
                    case 3:// closed (the client emptied its own-bet list)
                        if (watchers.Remove(uid)) log("leave " + client.Player.Name + "(" + uid + ")");
                        listRound.Remove(uid);
                        break;
                }
            }
        }

        public static void HandleList(Client.GameClient client, DiceList msg)
        {
            lock (sync)
            {
                if (shutDown || cfg == null) return;
                if (Throttled(client.Player.UID)) return;
                switch (msg.Type)
                {
                    case 0:// Refresh: the client emptied its table
                        watchers.Add(client.Player.UID);
                        if (enabled && phase == Phase.Betting) SendTable(client);
                        break;
                    case 1:// Lottery window
                        var list = new DiceList { Type = 1 };
                        foreach (var h in history)
                            list.Items.Add(new DiceBetInfo { Cell = 0, Value = (uint)(h.Value[0] + 10 * h.Value[1] + 100 * h.Value[2]), Amount = h.Key });
                        Send(client, list);
                        break;
                }
            }
        }

        static bool Throttled(uint uid)
        {
            DateTime now = DateTime.Now, last;
            if (lastRequest.TryGetValue(uid, out last) && (now - last).TotalMilliseconds < 300) return true;
            lastRequest[uid] = now;
            return false;
        }

        /// <summary>The whole table for one client that just emptied its cells (join / Refresh): 3286 type 0 adds to the cells.
        /// When its own-bet list is empty (closed with X in this round, relogged), its own bets go as 3285 type 1 instead
        /// (they add to the cells and refill the list).</summary>
        static void SendTable(Client.GameClient client)
        {
            uint uid = client.Player.UID;
            Stake mine;
            stakes.TryGetValue(uid, out mine);
            long r;
            bool refill = mine != null && (!listRound.TryGetValue(uid, out r) || r != round);
            var cells = new DiceList { Type = 0 };
            for (int cell = 0; cell < Cells; cell++)
            {
                long v = cellTotals[cell] - (refill ? mine.Cells[cell] : 0);
                if (v > 0) cells.Items.Add(new DiceBetInfo { Cell = (uint)cell, Value = 0, Amount = v });
            }
            if (cells.Items.Count > 0) Send(client, cells);
            if (refill)
            {
                for (int cell = 0; cell < Cells; cell++)
                    if (mine.Cells[cell] > 0)
                        Send(client, new DiceOpt { Type = 1, Uid = uid, Param = (uint)cell, Value = 0, Amount = mine.Cells[cell] });
                listRound[uid] = round;
            }
        }

        /// <summary>One chip on one cell. Also used by the GM test command (same checks).</summary>
        public static string PlaceBet(Client.GameClient client, int cell, uint moneyType, long amount)
        {
            lock (sync)
            {
                string why = TryBet(client, cell, moneyType, amount);
                if (why != null) client.SendSysMesage(why);
                return why;
            }
        }

        static string TryBet(Client.GameClient client, int cell, uint moneyType, long amount)
        {
            if (shutDown || cfg == null || !enabled) return "The dice table is closed right now.";
            DateTime now = DateTime.Now;
            if (phase != Phase.Betting || now >= phaseEnd.AddSeconds(-cfg.CutoffSeconds))
                return "Betting is closed for this round. Please wait for the next round.";
            if (cell < 0 || cell >= Cells) return "Invalid bet.";
            if (moneyType != 0) return "Only Silver can be bet at the dice table.";
            if (amount <= 0 || Array.IndexOf(cfg.Chips, amount) < 0) return "Invalid chip.";
            var p = client.Player;
            if (p.Level < cfg.MinLevel) return "You need level " + cfg.MinLevel + " to bet at the dice table.";
            Stake s;
            if (!stakes.TryGetValue(p.UID, out s)) s = null;
            if (s != null)
            {
                if (s.Count >= cfg.MaxBetsPerRound) return "You placed too many bets in this round.";
                if ((now - s.Last).TotalMilliseconds < cfg.MinBetIntervalMs) return "You are betting too fast.";
                if (s.Cells[cell] + amount > cfg.MaxBetPerCell) return "The most you can bet on one spot is " + cfg.MaxBetPerCell.ToString("N0", CultureInfo.InvariantCulture) + " Silver.";
                if (s.Total + amount > cfg.MaxBetPerRound) return "The most you can bet in one round is " + cfg.MaxBetPerRound.ToString("N0", CultureInfo.InvariantCulture) + " Silver.";
            }
            else if (amount > cfg.MaxBetPerCell || amount > cfg.MaxBetPerRound)
                return "The most you can bet on one spot is " + cfg.MaxBetPerCell.ToString("N0", CultureInfo.InvariantCulture) + " Silver.";
            if (p.Money < amount) return "Failed to bet. Please make sure you have " + amount.ToString("N0", CultureInfo.InvariantCulture) + " Silver!";

            p.Money -= amount;
            if (s == null) stakes[p.UID] = s = new Stake { Uid = p.UID, Name = p.Name };
            s.Cells[cell] += amount;
            s.Total += amount;
            s.Count++;
            s.Last = now;
            cellTotals[cell] += amount;
            AppendBet(round, p.UID, cell, amount, p.Name);
            watchers.Add(p.UID);
            listRound[p.UID] = round;

            var echo = new DiceOpt { Type = 1, Uid = p.UID, Param = (uint)cell, Value = 0, Amount = amount };
            var to = Online(watchers);
            if (!to.Contains(client)) to.Add(client);
            SendTo(to, echo);
            return null;
        }

        // ------------------------------------------------------------------ login / GM

        public static void Login(Client.GameClient client)
        {
            lock (sync)
            {
                if (shutDown || cfg == null) return;
                PayOwed(client);
            }
        }

        public static void Logout(Client.GameClient client)
        {
            lock (sync)
            {
                if (client == null || client.Player == null) return;
                watchers.Remove(client.Player.UID);
                lastRequest.Remove(client.Player.UID);
                listRound.Remove(client.Player.UID);
            }
        }

        /// <summary>@dice on/off. Off refunds the open round at once.</summary>
        public static string SetEnabled(bool on)
        {
            lock (sync)
            {
                if (cfg == null) return "not loaded";
                enabled = on;
                WriteState(on ? "on" : "off");
                if (!on)
                {
                    int n = RefundOpenRound("the dice table was closed");
                    phase = Phase.Closed;
                    // the clients' tables are emptied (no countdown)
                    SendTo(Online(watchers), new DiceOpt { Type = 2, Param = 0, Amount = 1 });
                    return "Dice table closed" + (n > 0 ? ", " + n + " player(s) refunded." : ".");
                }
                phaseEnd = DateTime.Now;
                return "Dice table open: a round starts now.";
            }
        }

        static int RefundOpenRound(string reason)
        {
            int n = 0;
            bool owedChanged = false;
            foreach (var s in stakes.Values)
            {
                Client.GameClient c;
                if (TryOnline(s.Uid, out c))
                {
                    Credit(c, s.Total);
                    c.SendSysMesage("Your dice bets (" + s.Total.ToString("N0", CultureInfo.InvariantCulture) + " Silver) were refunded: " + reason + ".");
                }
                else { AddOwed(s.Uid, s.Name, s.Total); owedChanged = true; }
                log("round " + round + " refund " + s.Name + "(" + s.Uid + ") " + s.Total + ": " + reason);
                n++;
            }
            stakes.Clear();
            Array.Clear(cellTotals, 0, Cells);
            DeleteBets();
            if (owedChanged) SaveOwed();
            return n;
        }

        /// <summary>GM test: 3285 {1:3}: the client empties its table and asks 3286 {1:0} (as its Refresh button).</summary>
        public static void Resync(Client.GameClient client)
        {
            Send(client, new DiceOpt { Type = 3 });
        }

        /// <summary>GM test of the crash path: refunds the bets of a bets.txt that is not the open round's (written by hand).</summary>
        public static string RecoverTest(string file)
        {
            lock (sync)
            {
                if (stakes.Count > 0) return "Not while this round has bets.";
                if (!File.Exists(file)) return file + " not found.";
                File.Move(file, BetsFile, true);
                int n = RecoverBets();
                PayOwedOnline();
                return n + " bet(s) recovered.";
            }
        }

        /// <summary>GM test: end the betting now (optionally with these dice).</summary>
        public static string ForceRoll(int[] dice)
        {
            lock (sync)
            {
                if (!enabled || phase != Phase.Betting) return "No round is taking bets now.";
                forced = dice;
                lastDice = null;
                phaseEnd = DateTime.Now;
                Tick();
                forced = null;
                return lastDice == null ? "Not rolled." : "Rolled " + lastDice[0] + "-" + lastDice[1] + "-" + lastDice[2] + ".";
            }
        }

        public static List<string> Status()
        {
            lock (sync)
            {
                var l = new List<string>();
                if (cfg == null) { l.Add("Dice: not loaded."); return l; }
                int left = (int)Math.Max(0, Math.Ceiling((phaseEnd - DateTime.Now).TotalSeconds));
                l.Add("Dice: " + (enabled ? "open" : "closed") + ", " + phase + (phase != Phase.Closed ? " " + left + " s left" : "") + ", round " + round
                    + ", bet " + cfg.BetSeconds + " s / result " + cfg.ResultSeconds + " s.");
                l.Add("This round: " + stakes.Count + " player(s), " + stakes.Values.Sum(s => s.Total).ToString("N0", CultureInfo.InvariantCulture) + " Silver. Watching: " + watchers.Count + ". Owed: " + owed.Count + " player(s).");
                l.Add("Since load: staked " + stakedTotal.ToString("N0", CultureInfo.InvariantCulture) + ", paid " + paidTotal.ToString("N0", CultureInfo.InvariantCulture) + ".");
                if (history.Count > 0)
                    l.Add("Last: " + string.Join("  ", history.Take(5).Select(h => h.Value[0] + "-" + h.Value[1] + "-" + h.Value[2])));
                foreach (var s in stakes.Values.Take(5)) l.Add("  " + s.Name + ": " + Describe(s.Cells));
                return l;
            }
        }

        public static bool DevCommands { get { var c = cfg; return c != null && c.DevCommands; } }

        // ------------------------------------------------------------------ money

        static void Credit(Client.GameClient c, long amount)
        {
            while (amount > 0)
            {
                long step = Math.Min(amount, PayStep);
                c.Player.Money += step;
                amount -= step;
            }
        }

        static void AddOwed(uint uid, string name, long amount)
        {
            if (amount <= 0) return;
            KeyValuePair<long, string> o;
            long before = owed.TryGetValue(uid, out o) ? o.Key : 0;
            owed[uid] = new KeyValuePair<long, string>(before + amount, name ?? o.Value ?? "");
        }

        static void PayOwed(Client.GameClient c)
        {
            KeyValuePair<long, string> o;
            if (c == null || c.Player == null || !owed.TryGetValue(c.Player.UID, out o)) return;
            owed.Remove(c.Player.UID);
            SaveOwed();// first out of the file, then paid: never twice
            Credit(c, o.Key);
            c.SendSysMesage("Dice table: you received " + o.Key.ToString("N0", CultureInfo.InvariantCulture) + " Silver (winnings or refunds of a round you were not online for).");
            log("owed " + o.Key + " paid to " + c.Player.Name + "(" + c.Player.UID + ")");
        }

        static void PayOwedOnline()
        {
            foreach (uint uid in owed.Keys.ToList())
            {
                Client.GameClient c;
                if (TryOnline(uid, out c)) PayOwed(c);
            }
        }

        // ------------------------------------------------------------------ sending

        static bool TryOnline(uint uid, out Client.GameClient c)
        {
            return Database.Server.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && c.FullLoading;
        }

        static List<Client.GameClient> Online(IEnumerable<uint> uids)
        {
            var l = new List<Client.GameClient>();
            foreach (uint u in uids)
            {
                Client.GameClient c;
                if (TryOnline(u, out c)) l.Add(c);
            }
            return l;
        }

        static void Send(Client.GameClient c, DiceOpt msg)
        {
            using (var rec = new ServerSockets.RecycledPacket())
                c.Send(DicePackets.Create(rec.GetStream(), msg));
        }

        static void Send(Client.GameClient c, DiceList msg)
        {
            using (var rec = new ServerSockets.RecycledPacket())
                c.Send(DicePackets.Create(rec.GetStream(), msg));
        }

        static void SendTo(List<Client.GameClient> to, DiceOpt msg)
        {
            if (to.Count == 0) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = DicePackets.Create(rec.GetStream(), msg);
                foreach (var c in to) c.Send(stream);
            }
        }

        static void World(string text)
        {
            using (var rec = new ServerSockets.RecycledPacket())
                Program.SendGlobalPackets.Enqueue(new MsgMessage(text, MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.System).GetArray(rec.GetStream()));
        }

        public static string CellName(int cell)
        {
            if (cell == 0) return "Small";
            if (cell == 1) return "Big";
            if (cell <= 7) return "Triple " + (cell - 1);
            return "Sum " + (cell - 4);
        }

        static string Describe(long[] cells)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < cells.Length; i++)
                if (cells[i] > 0) sb.Append(sb.Length > 0 ? ", " : "").Append(CellName(i)).Append(' ').Append(cells[i]);
            return sb.ToString();
        }

        // ------------------------------------------------------------------ files (small; written at once, atomically)

        static void WriteAtomic(string file, IEnumerable<string> lines)
        {
            string tmp = file + ".tmp";
            File.WriteAllLines(tmp, lines);
            File.Move(tmp, file, true);
        }

        static void AppendBet(long r, uint uid, int cell, long amount, string name)
        {
            try { File.AppendAllText(BetsFile, r + "\t" + uid + "\t" + cell + "\t" + amount + "\t" + (name ?? "").Replace('\t', ' ') + "\r\n"); }
            catch (Exception e) { log("bets.txt not written: " + e.Message); }
        }

        static void DeleteBets()
        {
            try { if (File.Exists(BetsFile)) File.Delete(BetsFile); }
            catch (Exception e) { log("bets.txt not deleted: " + e.Message); }
        }

        /// <summary>bets.txt left by a round that did not end: every bet becomes a refund (owed.txt).</summary>
        static int RecoverBets()
        {
            if (!File.Exists(BetsFile)) return 0;
            int n = 0;
            try
            {
                foreach (string l in File.ReadAllLines(BetsFile))
                {
                    var f = l.Split('\t');
                    uint uid; long amount; int cell;
                    if (f.Length < 4 || !uint.TryParse(f[1], out uid) || !int.TryParse(f[2], out cell) || !long.TryParse(f[3], out amount) || amount <= 0) continue;
                    AddOwed(uid, f.Length > 4 ? f[4] : "", amount);
                    n++;
                }
                SaveOwed();
                File.Move(BetsFile, Path.Combine(dir, "bets_recovered_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt"), true);
            }
            catch (Exception e) { log("bets.txt recovery failed: " + e.Message); }
            return n;
        }

        static void LoadOwed()
        {
            owed.Clear();
            if (!File.Exists(OwedFile)) return;
            foreach (string l in File.ReadAllLines(OwedFile))
            {
                var f = l.Split('\t');
                uint uid; long amount;
                if (f.Length >= 2 && uint.TryParse(f[0], out uid) && long.TryParse(f[1], out amount) && amount > 0)
                    AddOwed(uid, f.Length > 2 ? f[2] : "", amount);
            }
        }

        static void SaveOwed()
        {
            try { WriteAtomic(OwedFile, owed.Select(kv => kv.Key + "\t" + kv.Value.Key + "\t" + kv.Value.Value)); }
            catch (Exception e) { log("owed.txt not written: " + e.Message); }
        }

        static void LoadHistory()
        {
            history.Clear();
            if (!File.Exists(HistoryFile)) return;
            foreach (string l in File.ReadAllLines(HistoryFile))
            {
                var f = l.Split('\t');
                long t;
                if (f.Length < 2 || f[1].Length != 3 || !long.TryParse(f[0], out t)) continue;
                var d = f[1].Select(ch => ch - '0').ToArray();
                if (d.All(v => v >= 1 && v <= 6)) history.Add(new KeyValuePair<long, int[]>(t, d));
            }
        }

        static void SaveHistory()
        {
            try { WriteAtomic(HistoryFile, history.Select(h => h.Key + "\t" + h.Value[0] + h.Value[1] + h.Value[2])); }
            catch (Exception e) { log("history.txt not written: " + e.Message); }
        }

        static string ReadState()
        {
            try { return File.Exists(StateFile) ? File.ReadAllText(StateFile).Trim().ToLowerInvariant() : ""; }
            catch { return ""; }
        }

        static void WriteState(string s)
        {
            try { WriteAtomic(StateFile, new[] { s }); } catch (Exception e) { log("state.txt not written: " + e.Message); }
        }
    }
}
