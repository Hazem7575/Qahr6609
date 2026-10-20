using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Database;

namespace Conquer6609.Game.MsgMonster
{
    public unsafe partial class MonsterRole
    {
        // ---- Single death per life -------------------------------------------------------------------
        // Dead() can be reached concurrently (socket thread, MapGroupThread auto-attack, auto-hunt threads).
        // Only one caller at a time may run the death logic; a later caller sees !Alive and does nothing.
        // No lock is held while the death logic runs (it takes other locks), only this flag.
        private int deathRunning;
        // Serialises the "HP <= damage ? kill : HP -= damage" decision of ReceiveAttack.Monster.
        public readonly object HpSync = new object();
        /// <summary>True while Dead()'s death logic runs (a hit landing now must not touch HP or kill again).</summary>
        public bool DeathInProgress { get { return System.Threading.Volatile.Read(ref deathRunning) != 0; } }

        public void Dead(ServerSockets.Packet stream, Client.GameClient killer, uint aUID, Role.GameMap GameMap)
        {
            TryDead(stream, killer, aUID, GameMap);
        }
        /// <summary>Runs the death logic once; true when this call is the one that killed the monster.</summary>
        public bool TryDead(ServerSockets.Packet stream, Client.GameClient killer, uint aUID, Role.GameMap GameMap)
        {
            if (!Alive)
                return false;
            if (System.Threading.Interlocked.CompareExchange(ref deathRunning, 1, 0) != 0)
                return false;
            try
            {
                if (!Alive)
                    return false;
                DeadCore(stream, killer, aUID, GameMap);
                return true;
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref deathRunning, 0);
            }
        }

        // ---- Boss HP bar / harm ranking throttling -------------------------------------------------
        // Bosses with pending (throttled) updates; flushed from the 1 s kernel tick (BossHandler.CheckUp).
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, MonsterRole> PendingBossUpdates
            = new System.Collections.Concurrent.ConcurrentDictionary<uint, MonsterRole>();
        // [Performance] BossHpBarMs: 0 (default, original) = the boss HP bar is sent on every hit; e.g. 250 = at most
        // once per 250 ms per boss, a skipped update is sent by the next hit or the 1 s kernel tick.
        public static readonly int BossHpUpdateMs = Math.Max(0, Conquer6609.Framework.Settings.Int("Performance", "BossHpBarMs", 0));
        // [Performance] BossScoreThrottle: 0 (default, original) = the boss harm ranking is sent on every scored hit;
        // 1 = at most once per second (kernel tick) and once more right before the boss dies.
        public static readonly bool BossScoreThrottle = Conquer6609.Framework.Settings.Int("Performance", "BossScoreThrottle", 0) != 0;
        private int bossHpSentTick;
        private bool bossHpSentOnce;
        private volatile bool bossHpDirty;
        private volatile bool bossScoreDirty;

        /// <summary>True when a MaxHP/HP update for this boss may be sent now (at most once per 250 ms);
        /// otherwise the update is left for the next hit or the kernel tick.</summary>
        public bool TakeBossHpSlot()
        {
            if (BossHpUpdateMs <= 0)
                return true;
            lock (HpSync)
            {
                int now = Environment.TickCount;
                if (bossHpSentOnce && unchecked(now - bossHpSentTick) < BossHpUpdateMs)
                {
                    bossHpDirty = true;
                    PendingBossUpdates[UID] = this;
                    return false;
                }
                bossHpSentOnce = true;
                bossHpSentTick = now;
                bossHpDirty = false;
                return true;
            }
        }
        /// <summary>Called from the kernel tick: sends pending boss HP bars / harm rankings.</summary>
        public static void FlushBossUpdates()
        {
            if (PendingBossUpdates.IsEmpty)
                return;
            foreach (var key in PendingBossUpdates.Keys)
            {
                MonsterRole mob;
                if (!PendingBossUpdates.TryRemove(key, out mob) || mob == null)
                    continue;
                try
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        mob.FlushBossScore(stream);
                        if (mob.bossHpDirty)
                        {
                            if (!mob.Alive)
                                mob.bossHpDirty = false;
                            else if (mob.TakeBossHpSlot())
                            {
                                Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, mob.UID, 2);
                                stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, mob.Family.MaxHealth);
                                stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, mob.HitPoints);
                                stream = Upd.GetArray(stream);
                                mob.Send(stream);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyConsole.SaveException(ex);
                }
            }
        }

        /// <summary>Adds the hit to the harm ranking; the ranking itself is sent at most once per second
        /// (kernel tick) and once more right before the boss dies.</summary>
        public void SendScore(ServerSockets.Packet stream, Client.GameClient killer, uint Damage)
        {
            try
            {
                if (!Program.RankableFamilyIds.Contains(Family.ID))
                    return;
                lock (scoreObj)
                {
                    if (this.Hunters == null)
                        this.Hunters = new System.Collections.Concurrent.ConcurrentDictionary<uint, MsgBossHarmRankingEntry>();
                    MsgBossHarmRankingEntry entry;
                    if (this.Hunters.TryGetValue(killer.Player.UID, out entry))
                        entry.HunterScore += Damage;
                    else
                        this.Hunters[killer.Player.UID] = new MsgBossHarmRankingEntry() { HunterUID = killer.Player.UID, HunterScore = Damage, Rank = 0, HunterName = killer.Player.Name, ServerID = killer.Player.ServerID };
                    bossScoreDirty = true;
                }
                if (!BossScoreThrottle)
                    FlushBossScore(stream);
                else
                    PendingBossUpdates[UID] = this;
            }
            catch (Exception ex)
            {
                MyConsole.WriteException(ex);
                MyConsole.SaveException(ex);
            }
        }
        /// <summary>Sends the harm ranking to the boss screen if it changed since the last send.</summary>
        public void FlushBossScore(ServerSockets.Packet stream)
        {
            if (!bossScoreDirty)
                return;
            try
            {
                lock (scoreObj)
                {
                    if (!bossScoreDirty)
                        return;
                    bossScoreDirty = false;
                    if (this.Hunters == null || this.Hunters.Count == 0)
                        return;
                    var array = this.Hunters.Values.OrderByDescending(p => p.HunterScore).ToArray();
                    for (int x = 0; x < array.Length; x++)
                    {
                        array[x].Rank = (uint)(x + 1);
                    }
                    MsgServer.MsgBossHarmRanking Rank = new MsgServer.MsgBossHarmRanking();
                    Rank.MonsterID = this.Family.ID;
                    Rank.Type = MsgBossHarmRanking.RankAction.ShowRespondForTheRest;
                    Rank.Hunters = array;
                    this.Send(stream.CreateBossHarmRankList(Rank));
                }
            }
            catch (Exception ex)
            {
                MyConsole.WriteException(ex);
                MyConsole.SaveException(ex);
            }
        }
        public void SendBossSysMesage(string KillerName, int StudyPoints, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.Center
          , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        { 
            SendSysMesage("The " + Name.ToString() + " has been destroyed by the team " + KillerName.ToString() + "`s ! All team members received " + StudyPoints.ToString() + " Study Points!", ChatType, color);

        }
        public uint GetScorePosition(Client.GameClient killer)
        {
            lock (scoreObj)
            {
                if (this.Hunters == null) return 0;

                var a = Hunters.Values.OrderByDescending(p => p.HunterScore).ToArray();
                LogBossHunters(killer, a);
                for (int x = 0; x < a.Length; x++)
                {
                    if (a[x].HunterUID == killer.Player.UID)
                        return (uint)(x + 1);
                }
                return 0;
            }
        }
        // Same one-time "[Bosses]" log line GetScorePosition always wrote (caller holds scoreObj).
        private void LogBossHunters(Client.GameClient killer, MsgBossHarmRankingEntry[] sorted)
        {
            if (!Alive && !logged)
            {
                string log = "[Bosses] Name: " + Name + " | Killer: " + killer.Player.Name + " | Hunters: ";
                foreach (var hunter in sorted.Take(5))
                    log += hunter.HunterName + "     |      ";
                Database.ServerDatabase.LoginQueue.Enqueue(log);
                logged = true;
            }
        }
        public void GetBossReward(Client.GameClient killer, ServerSockets.Packet stream)
        {//جايزه الوحش الكبيره
            killer.Player.ConquerPoints += ServerRate.BigMonster;
            killer.SendSysMesage("You have received " + 1000+ " Conquer Points for being the last hitter of " + Name + "!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
            if (Map == 10137 )//Deityland
            {
                if (Hunters == null)
                    return;
                // Ranking is computed once (it used to be re-sorted for every hunter). Positions and the
                // credited CPs are unchanged.
                MsgBossHarmRankingEntry[] hunters;
                MsgBossHarmRankingEntry[] sorted;
                lock (scoreObj)
                {
                    hunters = Hunters.Values.ToArray();
                    sorted = hunters.OrderByDescending(p => p.HunterScore).ToArray();
                }
                foreach (var hunter in hunters)
                {
                    Client.GameClient client;
                    if (Server.GamePoll.TryGetValue(hunter.HunterUID, out client) && client != null)
                    {
                        uint position = 0;
                        lock (scoreObj)
                        {
                            LogBossHunters(client, sorted);
                        }
                        for (int x = 0; x < sorted.Length; x++)
                        {
                            if (sorted[x].HunterUID == client.Player.UID)
                            {
                                position = (uint)(x + 1);
                                break;
                            }
                        }
                        switch (position)
                        {
                            case 1:
                                killer.Player.ConquerPoints += ServerRate.BigMonster ;
                                killer.SendSysMesage("You have received " + ServerRate.BigMonster + " Conquer Points you Get Rank 1", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                                break;
                            case 2:
                                killer.Player.ConquerPoints += ServerRate.BigMonster / 2;
                                killer.SendSysMesage("You have received " + ServerRate.BigMonster /2+ " Conquer Points you Get Rank 2", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                                break;
                            case 3:
                                killer.Player.ConquerPoints += ServerRate.BigMonster / 3;
                                killer.SendSysMesage("You have received " + ServerRate.BigMonster/3 + " Conquer Points you Get Rank 3", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                                break;
                          
                        }
                    }
                }
            }
         
            
        }
    }
}
