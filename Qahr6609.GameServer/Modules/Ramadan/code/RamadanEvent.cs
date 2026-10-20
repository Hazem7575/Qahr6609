// The Legend of the Last Crescent - Ramadan event, runtime layer.
// Everything here is called from one-line hooks in the original source:
//   Database.Server.Initialize   -> RamadanEvent.Init()
//   Database.Server.SaveDatabase -> RamadanEvent.Save()
//   KernelThread.OnProcess       -> RamadanEvent.Tick()
//   MsgDataPacket.CompleteLogin  -> RamadanEvent.OnLogin(client)
//   Program.Game_Disconnect      -> RamadanEvent.OnLogout(client)
//   MonsterRole.Dead             -> RamadanEvent.OnMonsterKilled(killer, monster)
//   ReceiveAttack.Monster        -> RamadanEvent.AdjustMonsterDamage(client, monster, damage)
//   MsgMessage.ChatCommands      -> RamadanEvent.GmCommand(client, data)
// Lock order (never reversed): RunSync / CaravanSync  ->  (released)  ->  RamadanPlayerState.Sync.
// A player lock is never held while taking RunSync or CaravanSync, and two player locks are never nested.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.Ramadan
{
    public static unsafe partial class RamadanEvent
    {
        public static RamadanConfig Config = new RamadanConfig { Enabled = false };
        public static RamadanStore Store;
        public static bool Loaded;
        public static string Root;

        /// <summary>Online players' event state (loaded on login, saved on logout / every save cycle).</summary>
        static readonly ConcurrentDictionary<uint, RamadanPlayerState> Online = new ConcurrentDictionary<uint, RamadanPlayerState>();
        /// <summary>Serialises "load from file / modify offline / save" against login loads.</summary>
        static readonly object StateLoadSync = new object();

        static readonly object RndSync = new object();
        static readonly Random Rnd = new Random();
        static int Roll(int max) { lock (RndSync) return Rnd.Next(max); }
        static int Roll(int min, int max) { lock (RndSync) return Rnd.Next(min, max); }
        // ------------------------------------------------------------------ activity
        /// <summary>Story/daily/caravan/boss systems run only inside the event window.</summary>
        public static bool Active { get { return Loaded && Config.Enabled && Config.InWindow(DateTime.Now); } }
        /// <summary>NPCs (shop + herald + story NPCs) stay up during the window plus the shop grace days.</summary>
        public static bool NpcsUp { get { return Loaded && Config.Enabled && Config.ShopOpen(DateTime.Now); } }

        // ------------------------------------------------------------------ init / config
        public static void Init()
        {
            try
            {
                Root = Path.Combine(Conquer6609.Framework.Modules.RuntimeModules.Root, "Ramadan");// ServerDataBase\Modules\Ramadan (the build copies data\ there)
                Directory.CreateDirectory(Root);
                Store = new RamadanStore(Root);
                LoadConfig();
                BuildBoardIndex();
                Loaded = true;
                FinishCrashedCaravan();
                MyConsole.WriteLine("[Ramadan] The Legend of the Last Crescent loaded. Enabled=" + Config.Enabled
                    + " Window=" + Config.Start.ToString("yyyy-MM-dd HH:mm") + " -> " + Config.End.ToString("yyyy-MM-dd HH:mm")
                    + " Active=" + Active + " Warnings=" + Config.Errors.Count, ConsoleColor.Yellow);
                foreach (var e in Config.Errors.Take(20))
                    MyConsole.WriteLine("[Ramadan] " + e, ConsoleColor.DarkYellow);
            }
            catch (Exception e)
            {
                Loaded = false;
                MyConsole.WriteLine("[Ramadan] init failed, event disabled: " + e.Message, ConsoleColor.Red);
                MyConsole.WriteException(e);
            }
        }

        static string ConfigPath { get { return Path.Combine(Root, "event.ini"); } }

        public static void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                File.WriteAllText(ConfigPath, DefaultIni, new UTF8Encoding(false));
                MyConsole.WriteLine("[Ramadan] created default " + ConfigPath, ConsoleColor.Yellow);
            }
            var cfg = RamadanConfig.Load(File.ReadAllText(ConfigPath, Encoding.UTF8));
            ApplyNpcDefaults(cfg);
            ValidateItems(cfg);
            ValidateMonsters(cfg);
            Config = cfg;
            LoadDecor();
        }

        /// <summary>NPC UIDs are fixed (they must match NpcID); event.ini may move them or change their look.</summary>
        static void ApplyNpcDefaults(RamadanConfig cfg)
        {
            foreach (var d in NpcDefaults)
            {
                NpcSpot spot;
                if (!cfg.Npcs.TryGetValue(d.Key, out spot))
                {
                    cfg.Npcs[d.Key] = new NpcSpot { Key = d.Key, UID = d.UID, LookType = d.LookType, Dir = d.Dir, At = new MapPoint(d.At.Map, d.At.X, d.At.Y) };
                    continue;
                }
                if (spot.UID != d.UID)
                {
                    cfg.Errors.Add("Npcs." + d.Key + ": UID must stay " + d.UID + " (it is bound to NpcID) - ignored " + spot.UID);
                    spot.UID = d.UID;
                }
            }
            foreach (var key in cfg.Npcs.Keys.ToList())
                if (!NpcDefaults.Any(n => string.Equals(n.Key, key, StringComparison.OrdinalIgnoreCase)))
                {
                    cfg.Errors.Add("Npcs." + key + " is not a known Ramadan NPC - ignored");
                    cfg.Npcs.Remove(key);
                }
            foreach (var spot in cfg.Npcs.Values)
            {
                Role.GameMap map;
                if (Database.Server.ServerMaps == null || !Database.Server.ServerMaps.TryGetValue(spot.At.Map, out map))
                    cfg.Errors.Add("Npcs." + spot.Key + ": map " + spot.At.Map + " is not loaded");
                else if (!map.ValidLocation(spot.At.X, spot.At.Y))
                    cfg.Errors.Add("Npcs." + spot.Key + ": " + spot.At + " is not a walkable cell (the NPC is still placed)");
            }
        }

        /// <summary>Every item ID in rewards and the shop must exist in itemtype; unknown IDs are logged and dropped.</summary>
        static void ValidateItems(RamadanConfig cfg)
        {
            if (Database.Server.ItemsBase == null) { cfg.Errors.Add("ItemsBase not loaded - item validation skipped"); return; }
            foreach (var kv in cfg.Rewards)
            {
                var bad = kv.Value.Items.Where(i => !Database.Server.ItemsBase.ContainsKey(i.Key)).ToList();
                foreach (var b in bad)
                {
                    cfg.Errors.Add("Rewards." + kv.Key + ": item " + b.Key + " does not exist in itemtype - removed");
                    kv.Value.Items.Remove(b);
                }
            }
            foreach (var e in cfg.Shop.ToList())
            {
                Database.ItemType.DBItem db;
                if (!Database.Server.ItemsBase.TryGetValue(e.ItemId, out db))
                {
                    cfg.Errors.Add("Shop." + e.Key + ": item " + e.ItemId + " does not exist in itemtype - removed");
                    cfg.Shop.Remove(e);
                }
                else if (e.Label.StartsWith("Item ")) e.Label = db.Name;
            }
        }

        static void ValidateMonsters(RamadanConfig cfg)
        {
            foreach (var id in new[] { cfg.ScoutId, cfg.RaiderId, cfg.WardenId, cfg.TotemId, cfg.SentinelId, cfg.BossId, cfg.ShadeId })
            {
                Game.MsgMonster.MonsterFamily f;
                if (!Database.Server.MonsterFamilies.TryGetValue(id, out f))
                {
                    cfg.Errors.Add("monster " + id + " is missing from ServerDataBase\\Monsters (the matching feature will not spawn)");
                    continue;
                }
                foreach (var word in new[] { "Guard", "King", "Messenger", "Reviver" })
                    if (f.Name.Contains(word)) cfg.Errors.Add("monster " + id + " name '" + f.Name + "' contains '" + word + "' which changes its AI - rename it");
                if (f.Boss != 0) cfg.Errors.Add("monster " + id + " has Boss=" + f.Boss + " - event monsters must use Boss=0 or they never attack");
            }
        }

        /// <summary>@ramadan on/off persists the switch by rewriting only the Enabled= line of [Event].</summary>
        static void PersistEnabled(bool on)
        {
            try
            {
                var lines = File.ReadAllLines(ConfigPath, Encoding.UTF8).ToList();
                bool inEvent = false, done = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    var t = lines[i].Trim();
                    if (t.StartsWith("[")) { if (inEvent && !done) { lines.Insert(i, "Enabled=" + (on ? 1 : 0)); done = true; break; } inEvent = t.Equals("[Event]", StringComparison.OrdinalIgnoreCase); continue; }
                    if (inEvent && t.StartsWith("Enabled", StringComparison.OrdinalIgnoreCase) && t.Contains("="))
                    { lines[i] = "Enabled=" + (on ? 1 : 0); done = true; break; }
                }
                if (!done) { if (!inEvent) lines.Add("[Event]"); lines.Add("Enabled=" + (on ? 1 : 0)); }
                RamadanStore.AtomicWrite(ConfigPath, string.Join("\r\n", lines) + "\r\n");
            }
            catch (Exception e) { MyConsole.WriteLine("[Ramadan] could not persist Enabled: " + e.Message, ConsoleColor.Red); }
        }

        // ------------------------------------------------------------------ player state
        /// <summary>
        /// State of an online player (loaded once, kept in memory). Returns null for bots and for clients that are
        /// already disconnecting, so a late packet can never re-load a stale copy after the logout save.
        /// </summary>
        public static RamadanPlayerState GetState(Client.GameClient client)
        {
            if (client == null || client.Player == null || Store == null || client.Fake) return null;
            uint uid = client.Player.UID;
            RamadanPlayerState s;
            if (Online.TryGetValue(uid, out s)) return s;
            if (client.Socket == null || !client.Socket.Alive) return null;
            lock (StateLoadSync)
            {
                if (Online.TryGetValue(uid, out s)) return s;
                if (!client.Socket.Alive) return null;
                s = Store.LoadPlayer(uid);
                lock (s.Sync)
                {
                    s.Name = client.Player.Name ?? "";
                    RamadanRules.EnsureDaily(Config, s, DateTime.Now);
                }
                Online[uid] = s;
                return s;
            }
        }

        /// <summary>
        /// Saves under the player lock, so two saves of one player can never land on disk in the wrong order.
        /// (lock order: player Sync -> store file lock; the store never takes a player lock.)
        /// Players who never started the event and own nothing are not written at all.
        /// </summary>
        static bool SaveState(RamadanPlayerState s)
        {
            lock (s.Sync)
            {
                if (s.Stage == QuestStage.NotStarted && s.Tokens == 0 && s.Claims.Count == 0 && s.StampMask == 0 && s.CaravanRuns == 0)
                {
                    s.Dirty = false;
                    return true;
                }
                bool ok = Store.SavePlayer(s);
                if (ok) BoardUpdate(s.UID, s.Name, s.Points);
                return ok;
            }
        }

        /// <summary>Runs an action on a player's state whether he is online or not (offline: load, change, save).</summary>
        static void WithState(uint uid, Action<RamadanPlayerState, Client.GameClient> action)
        {
            var client = FindOnline(uid);
            if (client != null)
            {
                var s = GetState(client);
                if (s != null) { lock (s.Sync) action(s, client); return; }
            }
            lock (StateLoadSync)
            {
                RamadanPlayerState s;
                if (Online.TryGetValue(uid, out s)) { lock (s.Sync) action(s, null); return; }
                s = Store.LoadPlayer(uid);
                lock (s.Sync) action(s, null);
                if (!SaveState(s)) Store.Log(uid, s.Name, "offline-save-failed", "");
            }
        }

        public static void OnLogin(Client.GameClient client)
        {
            try
            {
                if (!Active || client == null || client.Fake) return;
                var s = GetState(client);
                if (s == null) return;
                RetryEarnedRewards(client, s);
                if (Config.LoginAnnouncement)
                {
                    client.SendSysMesage("Ramadan Kareem! The Legend of the Last Crescent has begun. Find the Guardian of the Crescent in Twin City.",
                        MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.yellow);
                    bool caravan;
                    lock (CaravanSync) caravan = Caravan != null;
                    if (caravan)
                        client.SendSysMesage("A Caravan of Light is under attack near Twin City! Ask the Caravan Master to take you there.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.orange);
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        public static void OnLogout(Client.GameClient client)
        {
            try
            {
                if (!Loaded || client == null || client.Player == null) return;
                lock (StateLoadSync)
                {
                    RamadanPlayerState s;
                    if (Online.TryRemove(client.Player.UID, out s))
                        SaveState(s);
                }
                RiddleCtx.TryRemove(client.Player.UID, out _riddleDummy);
                LanternHint lh; lanternHints.TryRemove(client.Player.UID, out lh);
                GuideJob gj; guides.TryRemove(client.Player.UID, out gj);
                uint nm; nightMap.TryRemove(client.Player.UID, out nm);
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        static string _riddleDummy;

        /// <summary>Called by Database.Server.SaveDatabase (every 250 s) - writes dirty states.</summary>
        public static void Save()
        {
            try
            {
                if (!Loaded) return;
                foreach (var s in Online.Values.ToList())
                {
                    try
                    {
                        lock (s.Sync) if (s.Dirty) SaveState(s);
                    }
                    catch (Exception e) { MyConsole.WriteException(e); }
                }
                SaveCaravanState();
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        /// <summary>
        /// Rewards are derived from the state, so anything earned but not delivered (full bag, failed save)
        /// is handed out later: on login and on every visit to the Guardian. Claim keys keep this idempotent.
        /// </summary>
        public static void RetryEarnedRewards(Client.GameClient c, RamadanPlayerState s)
        {
            var due = new List<KeyValuePair<string, string>>(); // claim key -> reward key
            int day;
            bool allDaily, stampDue;
            lock (s.Sync)
            {
                day = s.DailyDay;
                if (s.Stage >= QuestStage.Introduction) due.Add(new KeyValuePair<string, string>("intro", "Intro"));
                if (s.Stage >= QuestStage.SealChallenges) due.Add(new KeyValuePair<string, string>("clues", "Clues"));
                if ((s.Seals & SealFlags.Courage) != 0) due.Add(new KeyValuePair<string, string>("seal:courage", "SealCourage"));
                if ((s.Seals & SealFlags.Wisdom) != 0) due.Add(new KeyValuePair<string, string>("seal:wisdom", "SealWisdom"));
                if ((s.Seals & SealFlags.Unity) != 0) due.Add(new KeyValuePair<string, string>("seal:unity", "SealUnity"));
                if ((s.Seals & SealFlags.Trust) != 0) due.Add(new KeyValuePair<string, string>("seal:trust", "SealTrust"));
                if (s.Stage >= QuestStage.FinalGateUnlocked) due.Add(new KeyValuePair<string, string>("gate", "GateUnlocked"));
                if (s.Stage >= QuestStage.BossDefeated) due.Add(new KeyValuePair<string, string>("boss:first", "BossFirstClear"));
                if (s.Stage >= QuestStage.Completed) due.Add(new KeyValuePair<string, string>("story", "StoryComplete"));
                if (s.BossLastClearDay >= 0) due.Add(new KeyValuePair<string, string>("boss:day:" + s.BossLastClearDay, "BossDaily"));
                if (s.MakeupDone) due.Add(new KeyValuePair<string, string>("makeup:" + day, "Makeup"));
                for (int i = 0; i < 3; i++)
                    if ((s.DailyDoneMask & (1 << i)) != 0) due.Add(new KeyValuePair<string, string>("daily:" + day + ":" + i, "DailyTask"));
                due.RemoveAll(d => s.Claims.Contains(d.Key));
                allDaily = RamadanRules.AllDailyDone(s);
                stampDue = allDaily && !s.DailyStampGiven;
            }
            foreach (var d in due)
                Grant(c, s, d.Key, d.Value, "Reward kept for you");
            if (allDaily || stampDue) FinishNight(c, s);
            for (int i = 0; i < 3; i++) CompleteDaily(c, s, i);
            CheckMilestones(c, s);
        }

        // ------------------------------------------------------------------ leaderboard index (no file scans on the NPC thread)
        static readonly Dictionary<uint, KeyValuePair<string, int>> BoardIndex = new Dictionary<uint, KeyValuePair<string, int>>();

        static void BoardUpdate(uint uid, string name, int points)
        {
            lock (BoardIndex) BoardIndex[uid] = new KeyValuePair<string, int>(name ?? "", points);
        }

        /// <summary>Built once at start-up from the player files, then kept current by every save.</summary>
        static void BuildBoardIndex()
        {
            int n = 0;
            foreach (var uid in Store.AllPlayerUids())
            {
                var st = Store.LoadPlayer(uid);
                BoardUpdate(uid, st.Name, st.Points);
                n++;
            }
            MyConsole.WriteLine("[Ramadan] leaderboard index: " + n + " player file(s).", ConsoleColor.Yellow);
        }

        // ------------------------------------------------------------------ rewards
        public enum GrantResult { Ok, Already, NoSpace, SaveFailed, Empty }

        /// <summary>
        /// The single path for every reward: claim key first (idempotent), state is saved BEFORE items are handed out,
        /// a failed save undoes the claim and the currency. Caller must NOT hold any run/caravan lock.
        /// </summary>
        public static GrantResult Grant(Client.GameClient client, RamadanPlayerState s, string claimKey, string rewardKey, string why)
        {
            var spec = Config.Reward(rewardKey);
            lock (s.Sync)
            {
                if (s.Claims.Contains(claimKey)) return GrantResult.Already;
                int slots = spec.SlotsNeeded;
                if (slots > 0 && (client == null || !client.Inventory.HaveSpace((byte)Math.Min(40, slots))))
                {
                    if (client != null) Tell(client, "Make " + slots + " free inventory slot(s) first; your reward (" + why + ") is kept for you.", MsgMessage.MsgColor.red);
                    return GrantResult.NoSpace;
                }
                int oldTokens = s.Tokens, oldPoints = s.Points; long oldEarned = s.TokensEarned;
                RamadanRules.TryClaim(s, claimKey);
                RamadanRules.AddTokens(s, spec.Tokens);
                s.Points += spec.Points;
                if (!SaveState(s))
                {
                    s.Tokens = oldTokens; s.Points = oldPoints; s.TokensEarned = oldEarned;
                    RamadanRules.UndoClaim(s, claimKey);
                    Store.Log(s.UID, s.Name, "grant-save-failed", claimKey);
                    if (client != null) Tell(client, "The crescent flickers... please try again in a moment.", MsgMessage.MsgColor.red);
                    return GrantResult.SaveFailed;
                }
                if (client != null) Deliver(client, s, spec, claimKey);
                Store.Log(s.UID, s.Name, "reward", claimKey + " -> " + spec);
                if (client != null && !spec.IsEmpty)
                    Tell(client, why + ": " + spec + ". Tokens: " + s.Tokens + ".", MsgMessage.MsgColor.yellow);
                return GrantResult.Ok;
            }
        }

        static void Deliver(Client.GameClient client, RamadanPlayerState s, RewardSpec spec, string claimKey)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var item in spec.Items)
                {
                    bool ok = false;
                    try { ok = client.Inventory.Add(stream, item.Key, (byte)item.Value); } catch (Exception e) { MyConsole.WriteException(e); }
                    if (!ok) Store.Log(s.UID, s.Name, "item-delivery-failed", claimKey + " item " + item.Key + "x" + item.Value);
                }
                if (spec.ExpBalls > 0)
                    client.GainExpBall(600 * spec.ExpBalls, true);
                if (spec.Silver > 0)
                    client.Player.Money += spec.Silver;
            }
        }

        /// <summary>Grants a story stamp (idempotent) and checks the stamp milestones.</summary>
        static void Stamp(Client.GameClient client, RamadanPlayerState s, int n)
        {
            lock (s.Sync)
            {
                if (!RamadanRules.GrantStamp(s, n)) return;
                Store.Log(s.UID, s.Name, "stamp", n + " " + Stamps.Name(n));
                if (client != null)
                    Tell(client, "You received the " + Stamps.Name(n) + " (" + s.StampCount + "/" + Stamps.Total + ").", MsgMessage.MsgColor.yellow);
            }
            CheckMilestones(client, s);
        }

        static readonly int[] Milestones = { 7, 15, 30 };
        static void CheckMilestones(Client.GameClient client, RamadanPlayerState s)
        {
            int count;
            lock (s.Sync) count = s.StampCount;
            foreach (var m in Milestones)
                if (count >= m && client != null)
                    Grant(client, s, "milestone:" + m, "Milestone" + m, "Lantern of " + m + " stamps");
        }

        // ------------------------------------------------------------------ messages
        public static void Tell(Client.GameClient client, string text, MsgMessage.MsgColor color = MsgMessage.MsgColor.yellow)
        {
            if (client == null) return;
            try { client.SendSysMesage(text, MsgMessage.ChatMode.TopLeft, color); } catch { }
        }

        public static void Center(Client.GameClient client, string text)
        {
            if (client == null) return;
            try { client.SendSysMesage(text, MsgMessage.ChatMode.Center, MsgMessage.MsgColor.yellow); } catch { }
        }

        public static void Broadcast(string text, MsgMessage.ChatMode mode = MsgMessage.ChatMode.Center, MsgMessage.MsgColor color = MsgMessage.MsgColor.yellow)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new MsgMessage(text, color, mode).GetArray(stream));
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        static IEnumerable<Client.GameClient> OnlinePlayers()
        {
            foreach (var c in Database.Server.GamePoll.Values)
                if (c != null && !c.Fake && c.Player != null && c.Player.CompleteLogin && c.Socket != null && c.Socket.Alive)
                    yield return c;
        }

        static Client.GameClient FindOnline(uint uid)
        {
            Client.GameClient c;
            if (Database.Server.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && c.Player.CompleteLogin) return c;
            return null;
        }

        static long Now() { return RamadanRules.UnixNow(DateTime.Now); }

        static string Wait(long untilUnix)
        {
            long s = Math.Max(0, untilUnix - Now());
            return s >= 60 ? (s / 60) + " min " + (s % 60) + " s" : s + " s";
        }

        // ------------------------------------------------------------------ hooks: kills & damage
        public static void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            try
            {
                if (!Loaded || killer == null || monster == null || killer.Player == null) return;
                EventMob em;
                if (EventMobs.TryGetValue(monster.UID, out em) && ReferenceEquals(em.Mob, monster))
                {
                    // Dead() can run twice for one mob when two attack threads race; handle the kill once.
                    if (System.Threading.Interlocked.Exchange(ref em.Handled, 1) == 1) return;
                    em.DeadAt = DateTime.Now;
                    if (killer.Fake) { if (em.Kind == MobKind.Scout) scoutNextSpawnSet(em.Slot); }
                    else OnEventMobKilled(killer, em);   // instance/caravan kills are finished even after End
                }
                if (Active && !killer.Fake) OnAnyKill(killer, monster);
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        /// <summary>Damage hook: boss shield and phase thresholds. Returns the damage to apply.</summary>
        public static uint AdjustMonsterDamage(Client.GameClient client, Game.MsgMonster.MonsterRole monster, uint damage)
        {
            try
            {
                if (monster == null) return damage;
                EventMob em;
                if (!EventMobs.TryGetValue(monster.UID, out em) || !ReferenceEquals(em.Mob, monster)) return damage;
                if (em.Kind == MobKind.Boss) return BossAdjustDamage(client, em, damage);
                return damage;
            }
            catch (Exception e) { MyConsole.WriteException(e); return damage; }
        }

        /// <summary>Daily task progress for every kill (event mobs included).</summary>
        static void OnAnyKill(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            var s = GetState(killer);
            if (s == null) return;
            var now = DateTime.Now;
            var completed = new List<int>();
            bool makeupReady = false;
            lock (s.Sync)
            {
                if (s.Stage == QuestStage.NotStarted) return; // dailies start after the Guardian's introduction
                RamadanRules.EnsureDaily(Config, s, now);
                completed.AddRange(RamadanRules.AddDailyProgress(Config, s, now, DailyTaskType.Hunt, 1));
                if (monster.Family != null && monster.Family.MaxHealth >= Config.DailyEliteMinHP)
                    completed.AddRange(RamadanRules.AddDailyProgress(Config, s, now, DailyTaskType.Elite, 1));
                if (s.DailyTasks.Contains((int)DailyTaskType.CrescentDust) && Roll(100) < Config.DailyDustChancePct)
                {
                    var before = completed.Count;
                    completed.AddRange(RamadanRules.AddDailyProgress(Config, s, now, DailyTaskType.CrescentDust, 1));
                    int idx = Array.IndexOf(s.DailyTasks, (int)DailyTaskType.CrescentDust);
                    if (idx >= 0 && (s.DailyDoneMask & (1 << idx)) == 0 && completed.Count == before)
                        Tell(killer, "You found Crescent Dust (" + s.DailyProgress[idx] + "/" + Config.DailyDustNeeded + ").", MsgMessage.MsgColor.cyan);
                }
                if (!s.MakeupDone && RamadanRules.MakeupAvailable(Config, s, now) && s.MakeupKills < Config.MakeupKillCount)
                {
                    s.MakeupKills++;
                    s.Dirty = true;
                    if (s.MakeupKills == Config.MakeupKillCount) makeupReady = true;
                }
            }
            foreach (var i in completed.Distinct()) CompleteDaily(killer, s, i);
            if (makeupReady) Tell(killer, "Your Night Watch is complete. Report to the Night Watchman on Bird Island.", MsgMessage.MsgColor.yellow);
        }

        /// <summary>Rewards a finished daily task (claim daily:{day}:{i}); all three -> daily stamp.</summary>
        static void CompleteDaily(Client.GameClient client, RamadanPlayerState s, int i)
        {
            int day; DailyTaskType type;
            lock (s.Sync)
            {
                day = s.DailyDay;
                type = (DailyTaskType)s.DailyTasks[i];
                if ((s.DailyDoneMask & (1 << i)) != 0) return;
                if (s.DailyProgress[i] < RamadanRules.TaskTarget(Config, type)) return;
            }
            var r = Grant(client, s, "daily:" + day + ":" + i, "DailyTask", "Daily task done (" + TaskName(type) + ")");
            if (r != GrantResult.Ok && r != GrantResult.Already) return;
            lock (s.Sync)
            {
                if (s.DailyDay != day) return;
                s.DailyDoneMask |= 1 << i;
                s.Dirty = true;
            }
            FinishNight(client, s);
        }

        /// <summary>All three tasks done -> "dailyall" reward, then tonight's lantern stamp. Safe to call again (retries).</summary>
        static void FinishNight(Client.GameClient client, RamadanPlayerState s)
        {
            int day;
            lock (s.Sync)
            {
                if (!RamadanRules.AllDailyDone(s)) return;
                day = s.DailyDay;
            }
            var r2 = Grant(client, s, "dailyall:" + day, "DailyAllDone", "All of tonight's tasks");
            if (r2 != GrantResult.Ok && r2 != GrantResult.Already) return;
            int stamp = 0;
            lock (s.Sync)
            {
                if (s.DailyDay == day && !s.DailyStampGiven && s.DailyStamps < Stamps.DailyCount)
                {
                    stamp = RamadanRules.NextDailyStamp(s);
                    s.DailyStampGiven = true;
                    s.Dirty = true;
                }
            }
            if (stamp != 0) Stamp(client, s, stamp);
        }

        public static string TaskName(DailyTaskType t)
        {
            switch (t)
            {
                case DailyTaskType.Hunt: return "Hunt " + Config.DailyHuntCount + " monsters";
                case DailyTaskType.CrescentDust: return "Gather " + Config.DailyDustNeeded + " Crescent Dust from monsters";
                case DailyTaskType.Elite: return "Defeat " + Config.DailyEliteCount + " elite monsters (" + Config.DailyEliteMinHP + "+ HP)";
                case DailyTaskType.Caravan: return "Defend a Caravan of Light";
                case DailyTaskType.IftarErrand: return "Carry the iftar basket to the Old Lamplighter";
                case DailyTaskType.Riddle: return "Answer the Riddle of the Night";
                default: return "Unknown task";
            }
        }

        // ------------------------------------------------------------------ tick
        static DateTime lastTick = DateTime.MinValue;
        static int tickBusy;

        /// <summary>Called by KernelThread every second.</summary>
        public static void Tick()
        {
            if (!Loaded) return;
            var now = DateTime.Now;
            if ((now - lastTick).TotalMilliseconds < 900) return;
            if (System.Threading.Interlocked.Exchange(ref tickBusy, 1) == 1) return;
            try
            {
                lastTick = now;
                Step(SyncNpcs);
                Step(() => CleanupDeadMobs(now));
                if (Active)
                {
                    Step(() => TickScouts(now));
                    Step(() => TickCaravan(now));
                }
                else
                {
                    Step(() => { if (Caravan != null) StopCaravan("The Caravan of Light has ended with the event.", true); });
                    Step(() => RemoveAllMobs(m => m.Kind == MobKind.Scout));
                }
                Step(() => TickInstances(now));
                Step(() => TickGuides(now));
                Step(TickNight);
                Step(TickLanternSpots);
            }
            catch (Exception e) { MyConsole.WriteException(e); }
            finally { System.Threading.Interlocked.Exchange(ref tickBusy, 0); }
        }

        /// <summary>Each tick step is isolated: one failing step never stops caravans or instance timeouts.</summary>
        static void Step(Action a)
        {
            try { a(); } catch (Exception e) { MyConsole.WriteException(e); }
        }

        // ------------------------------------------------------------------ event monsters
        public enum MobKind { Scout, Raider, Warden, CourageMinion, Totem, Sentinel, Boss, Shade }

        public class EventMob
        {
            public Game.MsgMonster.MonsterRole Mob;
            public MobKind Kind;
            public object Owner;          // the run that owns it (null for scouts)
            public int Slot = -1;         // scout spawn slot
            public DateTime DeadAt = DateTime.MinValue;
            public int Handled;           // 1 once the kill was processed (Interlocked)
        }

        static readonly ConcurrentDictionary<uint, EventMob> EventMobs = new ConcurrentDictionary<uint, EventMob>();

        /// <summary>Spawns one event monster. Mirrors Database.Server.AddMapMonster but returns the MonsterRole.</summary>
        static EventMob SpawnMob(MobKind kind, uint familyId, uint mapId, ushort x, ushort y, ushort range, uint dynamicId, int hpOverride, object owner)
        {
            Role.GameMap map;
            if (!Database.Server.ServerMaps.TryGetValue(mapId, out map)) { Store.Log(0, "", "spawn-failed", "map " + mapId + " not loaded"); return null; }
            Game.MsgMonster.MonsterFamily famil;
            if (!Database.Server.MonsterFamilies.TryGetValue(familyId, out famil)) { Store.Log(0, "", "spawn-failed", "monster " + familyId + " missing"); return null; }
            if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap()) return null;

            var family = famil.Copy();
            family.SpawnX = (ushort)Math.Max(1, x - range);
            family.SpawnY = (ushort)Math.Max(1, y - range);
            family.MaxSpawnX = (ushort)(x + range + 1);
            family.MaxSpawnY = (ushort)(y + range + 1);
            family.MapID = map.ID;
            family.SpawnCount = 1;
            family.LavaX = x;
            family.LavaY = y;
            var mob = map.MonstersColletion.Add(family, true, dynamicId, true);
            if (mob == null) { Store.Log(0, "", "spawn-failed", "no valid cell near " + mapId + "," + x + "," + y); return null; }

            // Family.Copy() forces AttackRange=12 for 100k..7M HP; event mobs keep their own range.
            mob.Family.AttackRange = famil.AttackRange;
            mob.Family.RespawnTime = 24 * 3600; // never respawn by itself; we remove it after death
            if (hpOverride > 0)
            {
                mob.Family.MaxHealth = hpOverride;
                mob.HitPoints = (uint)hpOverride;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var action = new ActionQuery() { ObjId = mob.UID, Type = ActionType.RemoveEntity };
                mob.Send(stream.ActionCreate(&action));
                mob.Send(mob.GetArray(stream, false));
                if (mob.HitPoints > 65535) SendMobHp(stream, mob);
            }
            var em = new EventMob { Mob = mob, Kind = kind, Owner = owner };
            EventMobs[mob.UID] = em;
            return em;
        }

        static void SendMobHp(ServerSockets.Packet stream, Game.MsgMonster.MonsterRole mob)
        {
            var upd = new MsgUpdate(stream, mob.UID, 2);
            stream = upd.Append(stream, MsgUpdate.DataType.MaxHitpoints, mob.Family.MaxHealth);
            stream = upd.Append(stream, MsgUpdate.DataType.Hitpoints, mob.HitPoints);
            stream = upd.GetArray(stream);
            mob.Send(stream);
        }

        /// <summary>Removes an event monster from the map (alive or dead). Safe to call twice.</summary>
        static void RemoveMob(EventMob em)
        {
            EventMob dummy;
            EventMobs.TryRemove(em.Mob.UID, out dummy);
            if (em.Kind == MobKind.Scout && em.Slot >= 0)
                lock (scoutNextSpawn)
                {
                    DateTime next, min = DateTime.Now.AddSeconds(Config.ScoutRespawnSec);
                    if (!scoutNextSpawn.TryGetValue(em.Slot, out next) || next < min.AddSeconds(-5)) scoutNextSpawn[em.Slot] = min;
                }
            try
            {
                var mob = em.Mob;
                var map = mob.GMap;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    var action = new ActionQuery() { ObjId = mob.UID, Type = ActionType.RemoveEntity };
                    mob.Send(stream.ActionCreate(&action));
                }
                if (map != null)
                {
                    map.View.LeaveMap<Role.IMapObj>(mob);
                    mob.HitPoints = 0;
                    map.SetMonsterOnTile(mob.X, mob.Y, false);
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        /// <summary>Dead() can return early and leave a mob in the map (it would respawn) - we remove our own after ~3 s.</summary>
        static void CleanupDeadMobs(DateTime now)
        {
            foreach (var em in EventMobs.Values.ToList())
            {
                if (em.Mob.Alive) continue;
                if (em.DeadAt == DateTime.MinValue) em.DeadAt = now;
                if ((now - em.DeadAt).TotalSeconds >= 3) RemoveMob(em);
            }
        }

        static void RemoveAllMobs(Func<EventMob, bool> filter)
        {
            foreach (var em in EventMobs.Values.ToList())
                if (filter(em)) RemoveMob(em);
        }

        // ------------------------------------------------------------------ eclipse scouts (clue C)
        static readonly Dictionary<int, DateTime> scoutNextSpawn = new Dictionary<int, DateTime>();

        static void TickScouts(DateTime now)
        {
            var taken = new HashSet<int>(EventMobs.Values.Where(m => m.Kind == MobKind.Scout).Select(m => m.Slot));
            lock (scoutNextSpawn)
            for (int i = 0; i < Config.ScoutSpawns.Count; i++)
            {
                if (taken.Contains(i)) continue;
                DateTime next;
                if (scoutNextSpawn.TryGetValue(i, out next) && now < next) continue;
                var p = Config.ScoutSpawns[i];
                var em = SpawnMob(MobKind.Scout, Config.ScoutId, p.Map, p.X, p.Y, 3, 0, 0, null);
                if (em != null) em.Slot = i;
                scoutNextSpawn[i] = now.AddSeconds(Config.ScoutRespawnSec);
            }
        }

        static void OnEventMobKilled(Client.GameClient killer, EventMob em)
        {
            switch (em.Kind)
            {
                case MobKind.Scout:
                    scoutNextSpawnSet(em.Slot);
                    var s = GetState(killer);
                    if (s == null) return;
                    bool got;
                    lock (s.Sync) got = RamadanRules.GiveClue(s, ClueFlags.ShadowLetter);
                    if (got)
                    {
                        Store.Log(s.UID, s.Name, "clue", "ShadowLetter");
                        Stamp(killer, s, Stamps.ClueC);
                        Center(killer, "The Eclipse Scout drops a Shadow Letter. Bring it to the Guardian of the Crescent!");
                    }
                    break;
                case MobKind.Raider:
                    CaravanKill(killer, em);
                    break;
                default:
                    InstanceKill(killer, em);
                    break;
            }
        }

        static void scoutNextSpawnSet(int slot)
        {
            if (slot < 0) return;
            lock (scoutNextSpawn) scoutNextSpawn[slot] = DateTime.Now.AddSeconds(Config.ScoutRespawnSec);
        }

        // ------------------------------------------------------------------ default event.ini (written when missing)
        public const string DefaultIni = RamadanDefaults.EventIni;
    }
}
