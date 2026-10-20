using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.DragonIsland
{
    /// <summary>The beasts: each comes at its minutes of every hour (not doubled while alive), is announced to the players
    /// who may enter, has a damage ranking, and when it dies Commander Shan mails the rewards.</summary>
    public static class IslandBeasts
    {
        static readonly Dictionary<string, DateTime> lastSlot = new Dictionary<string, DateTime>();
        static readonly List<uint> rankAdded = new List<uint>();
        static readonly Random rand = new Random();

        public static Role.GameMap Map(IslandConfig c)
        {
            Role.GameMap map;
            return Database.Server.ServerMaps.TryGetValue(c.Map, out map) ? map : null;
        }

        public static bool Alive(IslandConfig c, Beast b)
        {
            var map = Map(c);
            return map != null && map.ContainMobID(b.Id, 0);
        }

        /// <summary>Damage ranking (packet 1044) for every beast of the island.</summary>
        public static void EnableRanking(IslandConfig c)
        {
            lock (Program.RankableFamilyIds)
                foreach (var b in c.Beasts)
                    if (!Program.RankableFamilyIds.Contains(b.Id)) { Program.RankableFamilyIds.Add(b.Id); rankAdded.Add(b.Id); }
        }

        public static void DisableRanking()
        {
            lock (Program.RankableFamilyIds)
            {
                foreach (uint id in rankAdded) Program.RankableFamilyIds.Remove(id);
                rankAdded.Clear();
            }
        }

        /// <summary>About once a second: a beast whose minute has come appears (3 s into the minute).</summary>
        public static void Tick(IslandConfig c, Action<string> log)
        {
            DateTime now = DateTime.Now;
            if (now.Second < 3) return;
            DateTime slot = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            foreach (var b in c.Beasts)
            {
                if (!b.Enabled || Array.IndexOf(b.Minutes, now.Minute) < 0) continue;
                DateTime last;
                lock (lastSlot)
                {
                    if (lastSlot.TryGetValue(b.Key, out last) && last == slot) continue;
                    lastSlot[b.Key] = slot;
                }
                if (Alive(c, b))
                {
                    log(b.Name + " is still alive at " + slot.ToString("HH:mm") + ": not doubled");
                    continue;
                }
                Spawn(c, b, log);
            }
        }

        public static void Spawn(IslandConfig c, Beast b, Action<string> log)
        {
            var map = Map(c);
            if (map == null) { log("map " + c.Map + " is not loaded"); return; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Database.Server.AddMapMonster(stream, map, b.Id, b.X, b.Y, b.Range, b.Range, 1, 0, true);
                string text = "[" + b.Name + "] has appeared around (" + b.X + "," + b.Y + ") on the Dragon Island! Go and kill it now.";
                Program.SendGlobalPackets.Enqueue(new MsgMessage(text, MsgMessage.MsgColor.orange, MsgMessage.ChatMode.SlideCrosTheServer).GetArray(stream));
                if (c.MessageBox)
                    foreach (var user in Database.Server.GamePoll.Values.ToArray())
                    {
                        if (user == null || user.Player == null || !user.FullLoading || !MayEnter(c, user) || user.Player.Map == c.Map) continue;
                        ushort x = b.X, y = b.Y;
                        user.Player.MessageBox(text + " Go there?", p => { if (MayEnter(c, p)) p.Teleport((ushort)(x + rand.Next(-3, 4)), (ushort)(y + rand.Next(-3, 4)), c.Map); }, null, c.MessageBoxSeconds);
                    }
            }
            log(b.Name + " appeared at " + b.X + "," + b.Y);
        }

        public static bool MayEnter(IslandConfig c, Client.GameClient client)
        {
            if (client == null || client.Player == null) return false;
            if (client.GameMaster || client.ProjectManager) return true;// staff test the island with any character
            return client.Player.Level >= c.MinLevel && client.Player.Reborn >= c.MinReborn;
        }

        /// <summary>A beast of the island died: the line to the server, then Commander Shan's mails (Exploit to the best 3
        /// by damage, Participation to everyone with enough damage). Runs before the server clears the ranking.</summary>
        public static void Killed(IslandConfig c, Client.GameClient killer, Game.MsgMonster.MonsterRole monster, Action<string> log)
        {
            if (monster == null || monster.Map != c.Map || monster.Family == null) return;
            var b = c.Beasts.FirstOrDefault(x => x.Id == monster.Family.ID);
            if (b == null) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (killer != null && killer.Player != null && !string.IsNullOrEmpty(c.KillMessage))
                    Program.SendGlobalPackets.Enqueue(new MsgMessage(string.Format(c.KillMessage, killer.Player.Name, b.Name), MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
            }
            var hunters = monster.Hunters != null ? monster.Hunters.Values.OrderByDescending(h => h.HunterScore).ToList() : new List<MsgBossHarmRankingEntry>();
            if (hunters.Count == 0 && killer != null && killer.Player != null)
                hunters.Add(new MsgBossHarmRankingEntry { HunterUID = killer.Player.UID, HunterName = killer.Player.Name, HunterScore = 1 });
            double need = monster.Family.MaxHealth * (c.ParticipationMinPercent / 100.0);
            int sent = 0;
            for (int i = 0; i < hunters.Count; i++)
            {
                var h = hunters[i];
                string error;
                if (i < 3 && (c.ExploitCPs[i] > 0 || c.ExploitItem != 0))
                {
                    if (Database.MailService.Send(h.HunterUID, c.Sender, "Exploit Reward", "Rank " + (i + 1) + " against the " + b.Name + ". Dragon Island thanks you!",
                        0, c.ExploitCPs[i], c.ExploitItem, null, out error)) sent++;
                    else log("Exploit mail to " + h.HunterName + " failed: " + error);
                }
                if (h.HunterScore >= need || i == 0)
                {
                    for (uint n = 0; n < Math.Min(3u, c.ParticipationCount); n++)
                    {
                        if (Database.MailService.Send(h.HunterUID, c.Sender, "Participation Reward", "You fought the " + b.Name + " on the Dragon Island.",
                            0, n == 0 ? c.ParticipationCPs : 0, c.ParticipationItem, null, out error)) sent++;
                        else { log("Participation mail to " + h.HunterName + " failed: " + error); break; }
                    }
                }
            }
            log(b.Name + " killed by " + (killer != null && killer.Player != null ? killer.Player.Name : "?") + ", " + hunters.Count + " hunter(s), " + sent + " mail(s)");
        }
    }
}
