// The Legend of the Last Crescent - Caravan of Light (open-world community event).
// Contribution = server-side Shadow Raider kills (capped). Rewards use the claim key caravan:{runId},
// are limited per machine (TqSerial) and are also applied to players who are offline when the run ends.
// The run is written to caravan_run.ini every 15 s; after a crash/restart the open run is closed with partial rewards.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.Ramadan
{
    public static unsafe partial class RamadanEvent
    {
        static readonly object CaravanSync = new object();
        static CaravanRun Caravan;
        static string lastCaravanSlot = "";
        static DateTime lastCaravanSave = DateTime.MinValue;

        public class CaravanRun
        {
            public string RunId;
            public DateTime Started, Ends;
            public int Wave;                       // 1-based current wave
            public DateTime NextWaveAt;
            public List<EventMob> Raiders = new List<EventMob>();
            public Dictionary<uint, int> Points = new Dictionary<uint, int>();
            public Dictionary<uint, string> Names = new Dictionary<uint, string>();
            public Dictionary<uint, string> Machines = new Dictionary<uint, string>();
        }

        static string CaravanFile { get { return Path.Combine(Root, "caravan_run.ini"); } }

        static void TickCaravan(DateTime now)
        {
            CaravanRun run;
            lock (CaravanSync) run = Caravan;
            if (run == null)
            {
                foreach (var t in Config.CaravanTimes)
                {
                    var at = now.Date + t;
                    string slot = at.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);
                    if (now >= at && now < at.AddMinutes(1) && slot != lastCaravanSlot && slot != ReadLastSlot())
                    {
                        lastCaravanSlot = slot;
                        WriteLastSlot(slot);
                        StartCaravan(slot);
                        break;
                    }
                }
                return;
            }
            bool finish = false, success = false;
            lock (CaravanSync)
            {
                if (Caravan == null) return;
                bool waveCleared = run.Raiders.All(r => !r.Mob.Alive);
                if (waveCleared && run.Wave >= Config.CaravanWaves) { finish = true; success = true; }
                else if (now >= run.Ends) finish = true;
                else if (run.Wave < Config.CaravanWaves && (waveCleared || now >= run.NextWaveAt)) SpawnWave(run, now);
            }
            if (finish) { FinishCaravan(success ? "The Caravan of Light reached the city safely!" : "The caravan limps into the city. The raiders took part of its light.", success); return; }
            if ((now - lastCaravanSave).TotalSeconds >= 15) SaveCaravanState();
        }

        /// <summary>Starts a run (schedule or GM). Returns an error text or null.</summary>
        public static string StartCaravan(string runId)
        {
            if (!Database.Server.MonsterFamilies.ContainsKey(Config.RaiderId)) return "Shadow Raider monster " + Config.RaiderId + " is missing.";
            if (!MapLoaded(Config.CaravanAt.Map)) return "map " + Config.CaravanAt.Map + " is not loaded.";
            lock (CaravanSync)
            {
                if (Caravan != null) return "a caravan is already running (" + Caravan.RunId + ").";
                var now = DateTime.Now;
                Caravan = new CaravanRun { RunId = runId, Started = now, Ends = now.AddMinutes(Config.CaravanDurationMin) };
                SpawnWave(Caravan, now);
                if (Caravan.Raiders.Count == 0) { Caravan = null; return "no Shadow Raider could be placed around " + Config.CaravanAt + "."; }
            }
            Store.Log(0, "", "caravan-start", runId);
            Broadcast("A Caravan of Light is under attack near Twin City! The Caravan Master can take you there. Defend it from the Shadow Raiders!");
            SaveCaravanState();
            return null;
        }

        static void SpawnWave(CaravanRun run, DateTime now)
        {
            run.Wave++;
            run.NextWaveAt = now.AddMinutes(Math.Max(1.0, (double)Config.CaravanDurationMin / Config.CaravanWaves));
            var p = Config.CaravanAt;
            for (int i = 0; i < Config.CaravanRaidersPerWave; i++)
            {
                var em = SpawnMob(MobKind.Raider, Config.RaiderId, p.Map, p.X, p.Y, (ushort)Config.CaravanSpawnRange, 0, 0, run);
                if (em != null) run.Raiders.Add(em);
            }
            if (run.Wave > 1) Broadcast("Wave " + run.Wave + "/" + Config.CaravanWaves + " of Shadow Raiders strikes the caravan!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.orange);
        }

        static void CaravanKill(Client.GameClient killer, EventMob em)
        {
            lock (CaravanSync)
            {
                var run = em.Owner as CaravanRun;
                if (run == null || !ReferenceEquals(run, Caravan)) return;
                uint uid = killer.Player.UID;
                int pts; run.Points.TryGetValue(uid, out pts);
                if (pts >= Config.CaravanPointsCap) return;
                run.Points[uid] = pts + 1;
                run.Names[uid] = killer.Player.Name;
                run.Machines[uid] = MachineKey(killer);
                if (pts + 1 == Config.CaravanMinContribution)
                    Tell(killer, "Your help counts! Stay until the caravan arrives to receive your reward.", MsgMessage.MsgColor.cyan);
            }
        }

        static string MachineKey(Client.GameClient c)
        {
            return string.IsNullOrWhiteSpace(c.TqSerial) ? "uid:" + c.Player.UID : c.TqSerial.Trim();
        }

        /// <summary>GM stop / event off: close the current run with partial rewards.</summary>
        public static void StopCaravan(string reason, bool partialRewards)
        {
            CaravanRun run;
            lock (CaravanSync) run = Caravan;
            if (run == null) return;
            if (partialRewards) FinishCaravan(reason, false);
            else
            {
                lock (CaravanSync) { Caravan = null; foreach (var r in run.Raiders) RemoveMob(r); }
                DeleteCaravanFile();
                Broadcast(reason, MsgMessage.ChatMode.TopLeft);
            }
        }

        static void FinishCaravan(string message, bool success)
        {
            CaravanRun run;
            lock (CaravanSync)
            {
                run = Caravan;
                if (run == null) return;
                Caravan = null;
                foreach (var r in run.Raiders) RemoveMob(r);
            }
            Broadcast(message, MsgMessage.ChatMode.Center);
            PayCaravan(run.RunId, run.Points, run.Names, run.Machines, success);
            DeleteCaravanFile();
        }

        static void PayCaravan(string runId, Dictionary<uint, int> points, Dictionary<uint, string> names, Dictionary<uint, string> machines, bool success)
        {
            var perMachine = new Dictionary<string, int>();
            int paid = 0;
            foreach (var kv in points.OrderByDescending(k => k.Value))
            {
                try
                {
                    uint uid = kv.Key;
                    int pts = Math.Min(kv.Value, Config.CaravanPointsCap);
                    if (pts < Config.CaravanMinContribution) continue;          // participation counts even if a config pays 0 tokens
                    int tokens = RamadanRules.CaravanTokens(Config, pts, success);
                    string machine; machines.TryGetValue(uid, out machine); machine = string.IsNullOrEmpty(machine) ? ("uid:" + uid) : machine;
                    int used; perMachine.TryGetValue(machine, out used);
                    if (Config.CaravanMaxRewardsPerMachine > 0 && used >= Config.CaravanMaxRewardsPerMachine)
                    {
                        Store.Log(uid, names.ContainsKey(uid) ? names[uid] : "", "caravan-machine-cap", runId + " " + machine);
                        Tell(FindOnline(uid), "This computer already received the maximum caravan rewards for this run.", MsgMessage.MsgColor.red);
                        continue;
                    }
                    if (PayOne(runId, uid, pts, tokens, names, success))
                    {
                        perMachine[machine] = used + 1;
                        paid++;
                    }
                }
                catch (Exception e) { MyConsole.WriteException(e); }
            }
            Store.Log(0, "", "caravan-end", runId + (success ? " success" : " partial") + " paid " + paid + "/" + points.Count);
        }

        static bool PayOne(string runId, uint uid, int pts, int tokens, Dictionary<uint, string> names, bool success)
        {
            string key = "caravan:" + runId;
            int stamp = 0;
            List<int> doneTasks = null;
            bool granted = false;
            WithState(uid, (s, client) =>
            {
                if (!RamadanRules.TryClaim(s, key)) return;
                granted = true;
                if (string.IsNullOrEmpty(s.Name) && names.ContainsKey(uid)) s.Name = names[uid];
                RamadanRules.AddTokens(s, tokens);
                s.Points += pts * Config.PointsPerCaravanPoint;
                s.CaravanRuns++;
                s.CaravanPointsTotal += pts;
                var now = DateTime.Now;
                s.CaravanDay = RamadanRules.EventDay(Config, now);
                if (!s.HasStamp(Stamps.FirstCaravan))
                {
                    if (client == null) RamadanRules.GrantStamp(s, Stamps.FirstCaravan);   // offline: saved by WithState
                    else stamp = Stamps.FirstCaravan;
                }
                if (s.Stage != QuestStage.NotStarted)
                    doneTasks = RamadanRules.AddDailyProgress(Config, s, now, DailyTaskType.Caravan, 1);
                s.Dirty = true;
                if (client != null && !SaveState(s)) Store.Log(uid, s.Name, "save-error", key);
                Store.Log(uid, s.Name, "reward", key + " -> " + tokens + " tokens, " + pts + " points" + (success ? "" : " (partial)") + (client == null ? " (offline)" : ""));
                if (client != null) Tell(client, "Caravan reward: " + tokens + " Crescent Tokens for " + pts + " point(s)" + (success ? "." : " (partial).") + " Tokens: " + s.Tokens + ".", MsgMessage.MsgColor.yellow);
            });
            if (!granted) return false;
            var online = FindOnline(uid);
            if (online != null)
            {
                var st = GetState(online);
                if (st != null)
                {
                    if (stamp != 0) Stamp(online, st, stamp);
                    if (doneTasks != null) foreach (var i in doneTasks) CompleteDaily(online, st, i);
                }
            }
            return true;
        }

        static string NextCaravanText()
        {
            lock (CaravanSync) if (Caravan != null) return "A caravan is under attack right now near Twin City!";
            if (Config.CaravanTimes.Count == 0) return "No caravans are scheduled.";
            var now = DateTime.Now;
            var next = Config.CaravanTimes.Select(t => now.Date + t).Select(d => d < now ? d.AddDays(1) : d).Min();
            return "The next caravan leaves at " + next.ToString("HH:mm") + " (server time) from the Caravan Master in Twin City.";
        }

        static string CaravanStatusText(uint uid)
        {
            lock (CaravanSync)
            {
                if (Caravan == null) return NextCaravanText() + " Defeat Shadow Raiders (" + Config.CaravanMinContribution + " at least) to earn Crescent Tokens.";
                int mine; Caravan.Points.TryGetValue(uid, out mine);
                int left = Caravan.Raiders.Count(r => r.Mob.Alive);
                return "The caravan is under attack! Wave " + Caravan.Wave + "/" + Config.CaravanWaves + ", " + left + " raiders left, " + Math.Max(0, (int)(Caravan.Ends - DateTime.Now).TotalMinutes) + " min to go. Your points: " + mine + " (minimum " + Config.CaravanMinContribution + ", max " + Config.CaravanPointsCap + ").";
            }
        }

        // ------------------------------------------------------------------ persistence of the running caravan
        static readonly object CaravanFileSync = new object();

        static void SaveCaravanState()
        {
            lock (CaravanFileSync)
            try
            {
                if (Store == null) return;
                string text;
                lock (CaravanSync)
                {
                    if (Caravan == null) return;
                    var sb = new StringBuilder();
                    sb.Append("RunId=" + Caravan.RunId + "\r\n");
                    sb.Append("Started=" + Caravan.Started.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "\r\n");
                    sb.Append("Wave=" + Caravan.Wave + "\r\n");
                    foreach (var kv in Caravan.Points)
                    {
                        string n, m; Caravan.Names.TryGetValue(kv.Key, out n); Caravan.Machines.TryGetValue(kv.Key, out m);
                        sb.Append("C=" + kv.Key + "|" + kv.Value + "|" + (m ?? "").Replace("|", "") + "|" + (n ?? "").Replace("|", "") + "\r\n");
                    }
                    sb.Append("End=OK\r\n");
                    text = sb.ToString();
                }
                RamadanStore.AtomicWrite(CaravanFile, text);
                lastCaravanSave = DateTime.Now;
            }
            catch (Exception e) { MyConsole.WriteLine("[Ramadan] caravan save failed: " + e.Message, ConsoleColor.Red); }
        }

        static void DeleteCaravanFile()
        {
            lock (CaravanFileSync)
            {
                try { if (File.Exists(CaravanFile)) File.Delete(CaravanFile); } catch { }
                try { if (File.Exists(CaravanFile + ".bak")) File.Delete(CaravanFile + ".bak"); } catch { }
            }
        }

        /// <summary>On startup: a caravan file means the server stopped during a run -> close it with partial rewards.</summary>
        static void FinishCrashedCaravan()
        {
            try
            {
                foreach (var path in new[] { CaravanFile, CaravanFile + ".bak" })
                {
                    if (!File.Exists(path)) continue;
                    var lines = File.ReadAllLines(path, Encoding.UTF8);
                    if (!lines.Any(l => l.Trim() == "End=OK")) { MyConsole.WriteLine("[Ramadan] " + path + " is incomplete - trying the backup.", ConsoleColor.DarkYellow); continue; }
                    string runId = null;
                    var pts = new Dictionary<uint, int>(); var names = new Dictionary<uint, string>(); var machines = new Dictionary<uint, string>();
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("RunId=")) runId = line.Substring(6).Trim();
                        else if (line.StartsWith("C="))
                        {
                            var p = line.Substring(2).Split('|');
                            uint uid; int v;
                            if (p.Length >= 2 && uint.TryParse(p[0], out uid) && int.TryParse(p[1], out v))
                            {
                                pts[uid] = v;
                                if (p.Length > 2) machines[uid] = p[2];
                                if (p.Length > 3) names[uid] = p[3];
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(runId))
                    {
                        MyConsole.WriteLine("[Ramadan] closing caravan " + runId + " interrupted by a restart (" + pts.Count + " contributors, partial rewards).", ConsoleColor.Yellow);
                        PayCaravan(runId, pts, names, machines, false);   // claim keys make a second pass harmless
                    }
                    break;
                }
                DeleteCaravanFile();
            }
            catch (Exception e) { MyConsole.WriteLine("[Ramadan] could not recover caravan: " + e.Message, ConsoleColor.Red); }
        }

        static string ReadLastSlot()
        {
            try { var t = Store.ReadGlobal("caravan_last.txt"); return t == null ? "" : t.Trim(); } catch { return ""; }
        }

        static void WriteLastSlot(string slot)
        {
            try { Store.WriteGlobal("caravan_last.txt", slot); } catch { }
        }
    }
}
