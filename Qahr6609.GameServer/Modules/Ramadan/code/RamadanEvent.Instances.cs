// The Legend of the Last Crescent - instanced content: Trial of Courage, Trial of Unity, Eclipse Sovereign.
// An instance is a DynamicID on an existing map (same mechanism as Tower of Mystery).
// All run bookkeeping happens under RunSync; rewards and player-state changes are collected and applied
// AFTER RunSync is released (see the lock order note in RamadanEvent.cs).
using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.Ramadan
{
    public static unsafe partial class RamadanEvent
    {
        static readonly object RunSync = new object();
        static readonly List<Run> Runs = new List<Run>();

        public enum RunKind { Courage, Unity, Boss }

        public class Run
        {
            public RunKind Kind;
            public uint Map, Dyn;
            public ushort X, Y;
            public uint Leader;
            public HashSet<uint> Participants = new HashSet<uint>();
            public HashSet<uint> Joined = new HashSet<uint>();      // everyone who ever entered (rewards by damage)
            public DateTime Started, Deadline;
            public DateTime EndAt = DateTime.MaxValue;   // set when finished: players are sent out at this time
            public bool Finished, Success;
            public List<EventMob> Mobs = new List<EventMob>();
            public Dictionary<uint, int> Kills = new Dictionary<uint, int>();
            // boss
            public EventMob Boss;
            public int Phase = 1;
            public bool Shield;
            public long ShieldHp;                                    // HP held while the shield is up
            public bool SentinelsPending;
            public int BaseMinAtk, BaseMaxAtk;
            public Dictionary<uint, long> Damage = new Dictionary<uint, long>();
            public DateTime NextBurst = DateTime.MaxValue;
            public List<Burst> Bursts = new List<Burst>();
            public HashSet<uint> PresentAtEnd = new HashSet<uint>();
            public bool RewardsDone;
            public DateTime NextHpUpdate = DateTime.MinValue;
            public NpcSpot Exit;
        }

        public class Burst { public ushort X, Y; public DateTime At; public uint Caster; }

        static Run RunOf(uint uid)
        {
            foreach (var r in Runs) if (!r.Finished && r.Participants.Contains(uid)) return r;
            return null;
        }

        static bool InRun(Client.GameClient c, Run r)
        {
            return c != null && c.Player != null && c.Player.Map == r.Map && c.Player.DynamicID == r.Dyn;
        }

        static bool MapLoaded(uint map)
        {
            return Database.Server.ServerMaps != null && Database.Server.ServerMaps.ContainsKey(map);
        }

        static void SendIn(Client.GameClient c, Run r)
        {
            ushort x = (ushort)(r.X + Roll(-2, 3)), y = (ushort)(r.Y + Roll(-2, 3));
            c.Teleport(x, y, r.Map, r.Dyn);
        }

        // ------------------------------------------------------------------ Trial of Courage (solo)
        static string StartCourage(Client.GameClient c)
        {
            var at = Config.CourageArena;
            if (!MapLoaded(at.Map)) return "The old battlefield cannot be reached (map " + at.Map + " is not loaded on this server).";
            if (!Database.Server.MonsterFamilies.ContainsKey(Config.WardenId)) return "The Warden has not answered the call (monster " + Config.WardenId + " missing).";
            Run r;
            lock (RunSync)
            {
                if (RunOf(c.Player.UID) != null) return "You are already inside a trial.";
                r = new Run { Kind = RunKind.Courage, Map = at.Map, X = at.X, Y = at.Y, Leader = c.Player.UID, Started = DateTime.Now, Deadline = DateTime.Now.AddSeconds(Config.CourageTimeLimitSec), Exit = Spot("Rashid") };
                r.Dyn = Database.Server.ServerMaps[at.Map].GenerateDynamicID();
                r.Participants.Add(c.Player.UID);
                r.Joined.Add(c.Player.UID);
                var w = SpawnMob(MobKind.Warden, Config.WardenId, r.Map, r.X, r.Y, 2, r.Dyn, 0, r);
                if (w == null) return "The Warden has not answered the call. Tell a GM.";
                r.Mobs.Add(w);
                for (int i = 0; i < Config.CourageMinions; i++)
                {
                    var m = SpawnMob(MobKind.CourageMinion, Config.ShadeId, r.Map, r.X, r.Y, 6, r.Dyn, 0, r);
                    if (m != null) r.Mobs.Add(m);
                }
                Runs.Add(r);
            }
            Store.Log(c.Player.UID, c.Player.Name, "courage-start", "dyn " + r.Dyn);
            SendIn(c, r);
            Center(c, "Trial of Courage: defeat the Eclipse Warden within " + (Config.CourageTimeLimitSec / 60) + " minutes!");
            return null;
        }

        // ------------------------------------------------------------------ Trial of Unity (team)
        static string StartUnity(Client.GameClient c)
        {
            var at = Config.UnityArena;
            if (!MapLoaded(at.Map)) return "The totem grounds cannot be reached (map " + at.Map + " is not loaded).";
            if (!Database.Server.MonsterFamilies.ContainsKey(Config.TotemId)) return "The totems are silent tonight (monster " + Config.TotemId + " missing).";
            var team = c.Team;
            if (team == null) return "Unity needs a team of at least " + Config.UnityMinTeam + ".";
            if (!team.TeamLider(c)) return "Only the team leader can lead the team into the trial.";
            var members = TeamNearby(c);
            if (members.Count < Config.UnityMinTeam) return "Gather at least " + Config.UnityMinTeam + " team members around me (you have " + members.Count + ").";
            Run r;
            lock (RunSync)
            {
                foreach (var m in members) if (RunOf(m.Player.UID) != null) return m.Player.Name + " is already inside a trial.";
                r = new Run { Kind = RunKind.Unity, Map = at.Map, X = at.X, Y = at.Y, Leader = c.Player.UID, Started = DateTime.Now, Deadline = DateTime.Now.AddSeconds(Config.UnityTimeLimitSec), Exit = Spot("Kareem") };
                r.Dyn = Database.Server.ServerMaps[at.Map].GenerateDynamicID();
                foreach (var m in members) { r.Participants.Add(m.Player.UID); r.Joined.Add(m.Player.UID); }
                for (int i = 0; i < Config.UnityTotems; i++)
                {
                    var t = SpawnMob(MobKind.Totem, Config.TotemId, r.Map, r.X, r.Y, 7, r.Dyn, 0, r);
                    if (t != null) r.Mobs.Add(t);
                }
                if (r.Mobs.Count == 0) return "The totems did not rise. Tell a GM.";
                Runs.Add(r);
            }
            Store.Log(c.Player.UID, c.Player.Name, "unity-start", "dyn " + r.Dyn + " members " + string.Join(",", members.Select(m => m.Player.Name)));
            foreach (var m in members)
            {
                SendIn(m, r);
                Center(m, "Trial of Unity: destroy the Shadow Totems together! Everyone who destroys one earns the seal.");
            }
            return null;
        }

        /// <summary>The leader plus team members on the same map within 18 cells, outside any instance.</summary>
        static List<Client.GameClient> TeamNearby(Client.GameClient c)
        {
            var list = new List<Client.GameClient> { c };
            if (c.Team == null) return list;
            foreach (var mi in c.Team.Temates)
            {
                var m = mi.client;
                if (m == null || m.Fake || m.Player == null || m.Socket == null || m.Player.UID == c.Player.UID || !m.Socket.Alive) continue;
                if (m.Player.Map != c.Player.Map || m.Player.DynamicID != 0) continue;
                if (Role.Core.GetDistance(m.Player.X, m.Player.Y, c.Player.X, c.Player.Y) > 18) continue;
                list.Add(m);
            }
            return list;
        }

        // ------------------------------------------------------------------ Eclipse Sovereign
        static string StartBoss(Client.GameClient c, bool withTeam)
        {
            var at = Config.BossArena;
            if (!MapLoaded(at.Map)) return "The Forgotten Gate leads nowhere (map " + at.Map + " is not loaded).";
            if (!Database.Server.MonsterFamilies.ContainsKey(Config.BossId)) return "The Sovereign is not here (monster " + Config.BossId + " missing).";
            var group = new List<Client.GameClient>();
            var skipped = new List<string>();
            if (withTeam)
            {
                if (c.Team == null) return "You have no team. Choose to go alone, or form a team first.";
                if (!c.Team.TeamLider(c)) return "Only the team leader can open the gate for a team.";
                foreach (var m in TeamNearby(c))
                {
                    var ms = GetState(m);
                    bool ok = false;
                    if (ms != null) lock (ms.Sync) ok = ms.Stage >= QuestStage.FinalGateUnlocked && ms.BossLockUntil <= Now();
                    if (ok && group.Count < Config.BossMaxTeam) group.Add(m); else if (m != c) skipped.Add(m.Player.Name);
                }
                if (!group.Contains(c)) return "You are not ready to enter yourself.";
            }
            else group.Add(c);

            int n = group.Count;
            long hp = (long)(Config.BossBaseHP * (1 + (n - 1) * Config.BossHPPerExtraMember));
            hp = Math.Min(hp, int.MaxValue - 1);
            Run r;
            lock (RunSync)
            {
                foreach (var m in group) if (RunOf(m.Player.UID) != null) return m.Player.Name + " is already inside a trial.";
                r = new Run { Kind = RunKind.Boss, Map = at.Map, X = at.X, Y = at.Y, Leader = c.Player.UID, Started = DateTime.Now, Deadline = DateTime.Now.AddSeconds(Config.BossTimeLimitSec), Exit = Spot("GateKeeper") };
                r.Dyn = Database.Server.ServerMaps[at.Map].GenerateDynamicID();
                foreach (var m in group) { r.Participants.Add(m.Player.UID); r.Joined.Add(m.Player.UID); }
                var b = SpawnMob(MobKind.Boss, Config.BossId, r.Map, r.X, r.Y, 1, r.Dyn, (int)hp, r);
                if (b == null) return "The Sovereign did not appear. Tell a GM.";
                r.Boss = b;
                r.BaseMinAtk = b.Mob.Family.MinAttack;
                r.BaseMaxAtk = b.Mob.Family.MaxAttack;
                r.Mobs.Add(b);
                Runs.Add(r);
            }
            foreach (var m in group)
            {
                var ms = GetState(m);
                if (ms != null) lock (ms.Sync) { ms.BossLockUntil = Now() + Config.BossEntryCooldownSec; ms.Dirty = true; }
            }
            Store.Log(c.Player.UID, c.Player.Name, "boss-start", "dyn " + r.Dyn + " hp " + hp + " party " + string.Join(",", group.Select(m => m.Player.Name)));
            foreach (var m in group)
            {
                SendIn(m, r);
                Center(m, "The Eclipse Sovereign awakens! (" + n + " hero" + (n > 1 ? "es" : "") + ", " + (Config.BossTimeLimitSec / 60) + " minutes)");
            }
            if (skipped.Count > 0) Tell(c, "Not ready (stage or cooldown) and left behind: " + string.Join(", ", skipped), MsgMessage.MsgColor.orange);
            return null;
        }

        /// <summary>Shield (phase 2) and threshold clamping, so a phase can never be skipped with one hit.</summary>
        static uint BossAdjustDamage(Client.GameClient client, EventMob em, uint damage)
        {
            var r = em.Owner as Run;
            if (r == null) return damage;
            lock (RunSync)
            {
                var mob = em.Mob;
                if (r.Finished || !mob.Alive) return damage;
                if (client == null || client.Player == null || !r.Participants.Contains(client.Player.UID)) return 0;
                if (r.Shield) return 0;
                long max = mob.Family.MaxHealth, hp = mob.HitPoints;
                long p2 = max * Config.BossPhase2Pct / 100, p3 = max * Config.BossPhase3Pct / 100;
                long dmg = damage;
                if (r.Phase == 1 && hp - dmg < p2)
                {
                    dmg = Math.Max(0, hp - p2);
                    r.Phase = 2;
                    r.Shield = Config.BossSentinels > 0;
                    r.ShieldHp = hp - dmg;
                    r.SentinelsPending = true;
                }
                else if (r.Phase == 2 && hp - dmg < p3)
                {
                    dmg = Math.Max(0, hp - p3);
                    r.Phase = 3;
                    r.NextBurst = DateTime.Now.AddSeconds(2);
                }
                long d;
                r.Damage.TryGetValue(client.Player.UID, out d);
                r.Damage[client.Player.UID] = d + Math.Min(dmg, hp);
                return (uint)dmg;
            }
        }

        // ------------------------------------------------------------------ kills inside instances
        static void InstanceKill(Client.GameClient killer, EventMob em)
        {
            var r = em.Owner as Run;
            if (r == null) return;
            var sealFor = new List<uint>();
            SealFlags seal = SealFlags.None;
            lock (RunSync)
            {
                if (r.Finished) return;
                int k; r.Kills.TryGetValue(killer.Player.UID, out k); r.Kills[killer.Player.UID] = k + 1;
                switch (em.Kind)
                {
                    case MobKind.Warden:
                        if (r.Kind == RunKind.Courage && r.Participants.Contains(killer.Player.UID))
                        {
                            r.Finished = true; r.Success = true; r.EndAt = DateTime.Now.AddSeconds(8);
                            seal = SealFlags.Courage; sealFor.Add(killer.Player.UID);
                        }
                        break;
                    case MobKind.Totem:
                        if (r.Mobs.Where(m => m.Kind == MobKind.Totem).All(m => !m.Mob.Alive))
                        {
                            r.Finished = true; r.Success = true; r.EndAt = DateTime.Now.AddSeconds(8);
                            seal = SealFlags.Unity;
                            foreach (var uid in r.Participants)
                            {
                                int kills; r.Kills.TryGetValue(uid, out kills);
                                if (kills > 0 && InRun(FindOnline(uid), r)) sealFor.Add(uid);
                            }
                        }
                        else
                        {
                            int left = r.Mobs.Count(m => m.Kind == MobKind.Totem && m.Mob.Alive);
                            foreach (var uid in r.Participants) Tell(FindOnline(uid), "A Shadow Totem shatters! " + left + " left.", MsgMessage.MsgColor.cyan);
                        }
                        break;
                    case MobKind.Sentinel:
                        if (r.Kind == RunKind.Boss && r.Shield && r.Mobs.Where(m => m.Kind == MobKind.Sentinel).All(m => !m.Mob.Alive))
                        {
                            r.Shield = false;
                            foreach (var uid in r.Participants) Center(FindOnline(uid), "The Sentinels fall - the Sovereign's shield shatters!");
                        }
                        break;
                    case MobKind.Boss:
                        if (r.Kind == RunKind.Boss)
                        {
                            r.Finished = true; r.Success = true; r.EndAt = DateTime.Now.AddSeconds(15);
                            r.Bursts.Clear();
                            foreach (var uid in r.Participants) if (InRun(FindOnline(uid), r)) r.PresentAtEnd.Add(uid);
                            var any = FindOnline(r.PresentAtEnd.FirstOrDefault());
                            if (any != null)
                                using (var rec = new ServerSockets.RecycledPacket())
                                    any.Player.AddMapEffect(rec.GetStream(), em.Mob.X, em.Mob.Y, "moon");
                        }
                        break;
                }
            }
            if (seal != SealFlags.None) GiveSealTo(sealFor, seal);
        }

        static void GiveSealTo(List<uint> uids, SealFlags seal)
        {
            foreach (var uid in uids)
            {
                try { GiveSealOne(uid, seal); } catch (Exception e) { MyConsole.WriteException(e); }
            }
        }

        static void GiveSealOne(uint uid, SealFlags seal)
        {
            {
                var c = FindOnline(uid);
                if (c == null) return;
                var s = GetState(c);
                if (s == null) return;
                bool ok;
                lock (s.Sync) ok = RamadanRules.GiveSeal(s, seal);
                if (seal == SealFlags.Courage)
                {
                    if (ok) { Stamp(c, s, Stamps.Courage); Grant(c, s, "seal:courage", "SealCourage", "Seal of Courage"); }
                    Center(c, ok ? "The Warden falls. Captain Rashid will be proud: the Seal of Courage is yours!" : "The Warden falls!");
                }
                else if (seal == SealFlags.Unity)
                {
                    if (ok) { Stamp(c, s, Stamps.Unity); Grant(c, s, "seal:unity", "SealUnity", "Seal of Unity"); }
                    Center(c, ok ? "The totems are gone. Together you earned the Seal of Unity!" : "The totems are gone. Well fought, friend!");
                }
                Store.Log(uid, c.Player.Name, "seal", seal + (ok ? " granted" : " (not eligible)"));
            }
        }

        static void BossRewards(Run r)
        {
            long max = r.Boss != null ? r.Boss.Mob.Family.MaxHealth : 1;
            int day = RamadanRules.EventDay(Config, DateTime.Now);
            foreach (var uid in r.Joined.ToList())
            {
                try
                {
                    long dmg; r.Damage.TryGetValue(uid, out dmg);
                    bool qualifies = r.PresentAtEnd.Contains(uid) || (Config.BossMinDamagePctForReward > 0 && dmg * 100 >= max * Config.BossMinDamagePctForReward);
                    var c = FindOnline(uid);
                    if (!qualifies) { Tell(c, "You were not part of the final battle - no reward this time.", MsgMessage.MsgColor.red); continue; }
                    if (c == null) { Store.Log(uid, "", "boss-reward-skipped", "offline at the end"); continue; }
                    var s = GetState(c);
                    if (s == null) continue;
                    bool first; bool daily;
                    lock (s.Sync)
                    {
                        first = RamadanRules.Advance(s, QuestStage.BossDefeated);
                        daily = s.BossLastClearDay != day;
                        s.BossClears++;
                        if (daily) s.BossLastClearDay = day;
                        s.Dirty = true;
                    }
                    Store.Log(uid, c.Player.Name, "boss-clear", "dmg " + dmg + "/" + max + (first ? " first" : ""));
                    if (first) { Stamp(c, s, Stamps.BossDefeated); Grant(c, s, "boss:first", "BossFirstClear", "The Eclipse is broken"); }
                    if (daily) Grant(c, s, "boss:day:" + day, "BossDaily", "Tonight's victory over the Sovereign");
                    Center(c, "The Eclipse Sovereign is defeated! The stolen light returns to the sky." + (first ? " Return to the Guardian of the Crescent." : ""));
                }
                catch (Exception e) { MyConsole.WriteException(e); }
            }
        }

        // ------------------------------------------------------------------ tick
        static void TickInstances(DateTime now)
        {
            List<Run> list;
            lock (RunSync) list = Runs.ToList();
            foreach (var r in list) TickRun(r, now);
        }

        static void TickRun(Run r, DateTime now)
        {
            var lockouts = new List<uint>();
            var exits = new List<uint>();
            bool rewardBoss = false, cleanup = false;
            string failMsg = null;
            var burstHits = new List<KeyValuePair<Client.GameClient, int>>();
            lock (RunSync)
            {
                // participants who left (scroll, death-revive in town, logout) are no longer part of the run
                foreach (var uid in r.Participants.ToList())
                    if (!InRun(FindOnline(uid), r) && now > r.Started.AddSeconds(5) && !r.Finished)
                    {
                        r.Participants.Remove(uid);
                        if (r.Kind == RunKind.Courage) lockouts.Add(uid);
                    }
                if (!r.Finished && r.Participants.Count == 0)
                {
                    r.Finished = true; r.EndAt = now;
                    Store.Log(r.Leader, "", r.Kind + "-abandoned", "dyn " + r.Dyn);
                }
                if (!r.Finished && now > r.Deadline)
                {
                    r.Finished = true; r.EndAt = now.AddSeconds(3);
                    failMsg = r.Kind == RunKind.Boss ? "The eclipse deepens... time is up. The Sovereign casts you out." : "Time is up! The shadows push you out of the trial.";
                    if (r.Kind == RunKind.Courage) lockouts.AddRange(r.Participants);
                    Store.Log(r.Leader, "", r.Kind + "-timeout", "dyn " + r.Dyn);
                }
                if (r.Kind == RunKind.Boss && !r.Finished) BossMechanics(r, now, burstHits);
                if (r.Kind == RunKind.Boss && r.Finished && r.Success && !r.RewardsDone) { r.RewardsDone = true; rewardBoss = true; }
                if (r.Finished && now >= r.EndAt)
                {
                    foreach (var uid in r.Participants.Union(r.PresentAtEnd)) exits.Add(uid);
                    Runs.Remove(r);
                    cleanup = true;
                }
            }
            if (failMsg != null) foreach (var uid in r.Participants) Center(FindOnline(uid), failMsg);
            foreach (var hit in burstHits) BurstDamage(hit.Key, hit.Value);
            if (rewardBoss) BossRewards(r);
            foreach (var uid in lockouts.Distinct())
            {
                var c = FindOnline(uid);
                var s = c != null ? GetState(c) : null;
                if (s != null) lock (s.Sync) { s.CourageLockUntil = Now() + Config.CourageCooldownSec; s.Dirty = true; }
            }
            if (cleanup)
            {
                foreach (var uid in exits)
                {
                    var c = FindOnline(uid);
                    if (InRun(c, r)) c.Teleport((ushort)(r.Exit.At.X + 2), (ushort)(r.Exit.At.Y + 2), r.Exit.At.Map, 0);
                }
                foreach (var m in r.Mobs) RemoveMob(m);
            }
        }

        /// <summary>Phase 2 sentinels, phase 3 Eclipse Burst, live HP bar. Runs under RunSync; player damage is returned to the caller.</summary>
        static void BossMechanics(Run r, DateTime now, List<KeyValuePair<Client.GameClient, int>> hits)
        {
            var boss = r.Boss.Mob;
            // Toxic Fog / poison lower HP without passing through AdjustMonsterDamage: strip it and hold the phase floor.
            if (boss.Alive)
            {
                if (boss.ContainFlag(MsgUpdate.Flags.Poisoned)) boss.RemoveFlag(MsgUpdate.Flags.Poisoned, boss.GMap);
                long max = boss.Family.MaxHealth;
                long floor = r.Phase == 1 ? max * Config.BossPhase2Pct / 100 : r.Phase == 2 ? (r.Shield ? r.ShieldHp : max * Config.BossPhase3Pct / 100) : 0;
                if (floor > 0 && boss.HitPoints < floor) boss.HitPoints = (uint)floor;
            }
            if (r.SentinelsPending)
            {
                r.SentinelsPending = false;
                boss.Family.MinAttack = (int)(r.BaseMinAtk * 1.25);
                boss.Family.MaxAttack = (int)(r.BaseMaxAtk * 1.25);
                for (int i = 0; i < Config.BossSentinels; i++)
                {
                    var sMob = SpawnMob(MobKind.Sentinel, Config.SentinelId, r.Map, boss.X, boss.Y, 6, r.Dyn, 0, r);
                    if (sMob != null) r.Mobs.Add(sMob);
                }
                if (r.Shield && !r.Mobs.Any(m => m.Kind == MobKind.Sentinel && m.Mob.Alive)) r.Shield = false; // nothing spawned: never lock the fight
                foreach (var uid in r.Participants)
                    Center(FindOnline(uid), r.Shield ? "The Sovereign raises an eclipse shield! Destroy the Eclipse Sentinels!" : "The Sovereign grows furious!");
            }
            if (r.Phase == 3 && now >= r.NextBurst)
            {
                r.NextBurst = now.AddSeconds(Config.BossBurstIntervalSec);
                var targets = r.Participants.Select(FindOnline).Where(c => InRun(c, r) && c.Player.Alive).OrderBy(c => Roll(1000)).Take(Config.BossBurstTargets).ToList();
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var t in targets)
                    {
                        r.Bursts.Add(new Burst { X = t.Player.X, Y = t.Player.Y, At = now.AddSeconds(Config.BossBurstWarnSec), Caster = t.Player.UID });
                        t.Player.AddMapEffect(stream, t.Player.X, t.Player.Y, "ring");
                    }
                }
                foreach (var uid in r.Participants) Tell(FindOnline(uid), "Eclipse Burst! Move away from the glowing rings!", MsgMessage.MsgColor.red);
            }
            foreach (var b in r.Bursts.Where(b => now >= b.At).ToList())
            {
                r.Bursts.Remove(b);
                var players = r.Participants.Select(FindOnline).Where(c => InRun(c, r)).ToList();
                var any = players.FirstOrDefault();
                if (any != null)
                    using (var rec = new ServerSockets.RecycledPacket())
                        any.Player.AddMapEffect(rec.GetStream(), b.X, b.Y, "bomb");
                foreach (var p in players)
                    if (p.Player.Alive && Role.Core.GetDistance(p.Player.X, p.Player.Y, b.X, b.Y) <= Config.BossBurstRadius)
                        hits.Add(new KeyValuePair<Client.GameClient, int>(p, (int)(p.Status.MaxHitpoints * (uint)Config.BossBurstDamagePct / 100)));
            }
            if (now >= r.NextHpUpdate && boss.Alive)
            {
                r.NextHpUpdate = now.AddSeconds(1);
                using (var rec = new ServerSockets.RecycledPacket()) SendMobHp(rec.GetStream(), boss);
            }
        }

        /// <summary>Non-lethal mechanic damage (never below 1 HP).</summary>
        static void BurstDamage(Client.GameClient c, int dmg)
        {
            try
            {
                if (c == null || !c.Player.Alive || dmg <= 0) return;
                c.Player.HitPoints = Math.Max(1, c.Player.HitPoints - dmg);
                using (var rec = new ServerSockets.RecycledPacket())
                    c.Player.SendUpdate(rec.GetStream(), c.Player.HitPoints, MsgUpdate.DataType.Hitpoints, true);
                Tell(c, "The Eclipse Burst hits you for " + dmg + "!", MsgMessage.MsgColor.red);
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        /// <summary>Ends every run (used by @ramadan off/reload): players are sent back, monsters removed, no rewards.</summary>
        static void EndAllRuns(string reason)
        {
            List<Run> list;
            lock (RunSync)
            {
                list = Runs.ToList();
                foreach (var r in list) { if (!r.Finished) { r.Finished = true; } r.EndAt = DateTime.MinValue; }
            }
            foreach (var r in list)
            {
                foreach (var uid in r.Participants) Center(FindOnline(uid), reason);
                TickRun(r, DateTime.Now);
            }
        }

        static string RunsText()
        {
            lock (RunSync)
                return Runs.Count == 0 ? "no instances" : string.Join("; ", Runs.Select(r => r.Kind + " dyn " + r.Dyn + " players " + r.Participants.Count + (r.Kind == RunKind.Boss && r.Boss != null ? " hp " + r.Boss.Mob.HitPoints + "/" + r.Boss.Mob.Family.MaxHealth + " phase " + r.Phase + (r.Shield ? " shield" : "") : "") + (r.Finished ? " finished" : "")));
        }
    }
}
