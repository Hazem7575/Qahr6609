using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    /// <summary>Quality of a plundered Gold Brick (shown with the client's gold brick status icons 161-165).</summary>
    public enum GoldBrickQuality : byte
    {
        Normal = 1,
        Refined = 2,
        Unique = 3,
        Elite = 4,
        Super = 5
    }

    /// <summary>
    /// Official Kingdom War (CO 3.0 "Kingdom War", patch 6609) on a single server:
    ///  * Every Union owns Gold Bricks (500 when founded). At 22:00 on settlement days the Union with the most
    ///    Gold Bricks is upgraded to the Kingdom (its leader becomes Emperor); the defeated Kingdom degrades to Union.
    ///    If the server has no Kingdom yet, the Guild War winner's union is crowned when the Guild War ends
    ///    (a union is founded for the winning guild when the server has no union at all).
    ///  * Plunder War: on Monday/Friday a Union Leader issues a Loot Token (00:00 - 19:59:59, costs Union Fund).
    ///    21:00 - 21:29:59 the plundering unions grab Gold Bricks from the Kingdom's Treasury in Twin City
    ///    (stay next to it for a few seconds); the brick is carried (status icon) and handed in to the
    ///    Kingdom Mission Envoy, which moves Gold Bricks from the Kingdom to the plunderer's Union and gives
    ///    Potency Points, Union Fund and Kingdom Deeds. The Emperor and Marshals can raise a Dragon Shield
    ///    (3 minutes without plundering).
    /// All entry points are defensive: every public method catches its own exceptions.
    /// </summary>
    public static class MsgKingdomWar
    {
        public const uint TreasuryNpcUID = 17450;
        public const ushort TreasuryMesh = 35690; // npc.ini [NpcType3569] Name=Treasury (Kingdom treasury, effect gz_gkgx_big)

        public static readonly object SyncRoot = new object();

        public class Registration
        {
            public uint UnionUID;
            public uint KingdomUID;
            public string UnionName = "";
        }
        public class CarriedBrick
        {
            public GoldBrickQuality Quality;
            public uint KingdomUID;
            public bool AddedFlag;
        }
        class Plunder
        {
            public uint UID;
            public DateTime Start;
            public uint Map;
            public ushort X, Y;
        }

        static bool initialized;
        internal static readonly Dictionary<uint, Registration> Registrations = new Dictionary<uint, Registration>();
        internal static int RegistrationDay;
        internal static readonly Dictionary<uint, CarriedBrick> Carried = new Dictionary<uint, CarriedBrick>();
        internal static readonly HashSet<uint> KnownUnions = new HashSet<uint>();
        internal static int LastSettleDay;
        static readonly Dictionary<uint, Plunder> plundering = new Dictionary<uint, Plunder>();

        static ProcesType warState = ProcesType.Dead;
        static DateTime warEnd;
        static uint warKingdomUID;
        static int lastWarDay, lastCloseNoticeDay;
        static DateTime shieldUntil = DateTime.MinValue;
        static int shieldsUsed;
        static ProcesType lastGuildWarState = ProcesType.Dead;
        static uint pendingGuildWarGuild;
        static DateTime nextFlagCheck = DateTime.MinValue;
        static Game.MsgNpc.Npc treasury;

        /// <summary>Alive while the Plunder War (21:00 - 21:30) runs.</summary>
        public static ProcesType WarState { get { return warState; } }
        public static bool ShieldActive { get { return DateTime.Now < shieldUntil; } }
        public static int ShieldSecondsLeft { get { return ShieldActive ? (int)Math.Ceiling((shieldUntil - DateTime.Now).TotalSeconds) : 0; } }

        public static uint ServerID
        {
            get
            {
                var info = Database.GroupServerList.MyServerInfo;
                return info != null ? info.ID : 0;
            }
        }

        public static Role.Instance.Union GetKingdom()
        {
            foreach (var union in Role.Instance.Union.UnionPoll.Values)
                if (union != null && union.IsKingdom == 1)
                    return union;
            return null;
        }
        public static Role.Instance.Union GetUnion(uint uid)
        {
            Role.Instance.Union union;
            if (uid != 0 && Role.Instance.Union.UnionPoll.TryGetValue(uid, out union))
                return union;
            return null;
        }
        static int DayKey(DateTime d) { return d.Year * 10000 + d.Month * 100 + d.Day; }

        public static bool IsRegistered(uint unionUID)
        {
            lock (SyncRoot)
                return RegistrationDay == DayKey(DateTime.Now) && Registrations.ContainsKey(unionUID);
        }
        public static bool IsAttacker(Client.GameClient client)
        {
            if (client == null || client.Player == null || !client.Player.InUnion)
                return false;
            lock (SyncRoot)
                return warState == ProcesType.Alive && Registrations.ContainsKey(client.Player.MyUnion.UID);
        }
        public static bool IsDefender(Client.GameClient client)
        {
            if (client == null || client.Player == null || !client.Player.InUnion)
                return false;
            lock (SyncRoot)
                return warState == ProcesType.Alive && client.Player.MyUnion.UID == warKingdomUID;
        }
        public static CarriedBrick GetCarried(uint uid)
        {
            lock (SyncRoot)
            {
                CarriedBrick b;
                return Carried.TryGetValue(uid, out b) ? b : null;
            }
        }

        #region Kernel tick
        /// <summary>Called every second from MsgSchedules.CheckUp (kernel thread).</summary>
        public static void CheckUp(DateTime now)
        {
            if (!KingdomWarConfig.Enabled)
                return;
            try
            {
                lock (SyncRoot)
                {
                    if (!initialized)
                        Initialize(now);

                    int today = DayKey(now);
                    TimeSpan tod = now.TimeOfDay;
                    if (warState != ProcesType.Alive && RegistrationDay != today && Registrations.Count > 0)
                    {
                        // Loot Tokens are only valid for the day they were issued.
                        Registrations.Clear();
                        RegistrationDay = today;
                        KingdomWarStore.Save();
                    }

                    bool plunderDay = KingdomWarConfig.IsPlunderDay(now.DayOfWeek);
                    TimeSpan warStart = KingdomWarConfig.WarStart;
                    TimeSpan warStop = warStart + TimeSpan.FromMinutes(KingdomWarConfig.WarMinutes);

                    if (plunderDay && lastCloseNoticeDay != today && tod >= KingdomWarConfig.LootTokenClose && tod < warStart)
                    {
                        lastCloseNoticeDay = today;
                        if (Registrations.Count > 0)
                            Broadcast("The Loot Token is now closed. The Plunder War will start at " + warStart.Hours.ToString("00") + ":" + warStart.Minutes.ToString("00") + ". Get prepared!",
                                MsgMessage.ChatMode.Center);
                    }

                    if (warState == ProcesType.Dead && plunderDay && lastWarDay != today && tod >= warStart && tod < warStop)
                    {
                        lastWarDay = today;
                        StartWar(now.Date + warStop);
                    }
                    if (warState == ProcesType.Alive)
                    {
                        if (now >= warEnd)
                            EndWar();
                        else
                            UpdatePlunders(now);
                    }

                    if (KingdomWarConfig.IsSettleDay(now.DayOfWeek) && LastSettleDay != today
                        && tod >= KingdomWarConfig.SettleTime && tod < KingdomWarConfig.SettleTime + TimeSpan.FromMinutes(10))
                    {
                        LastSettleDay = today;
                        Settle();
                        KingdomWarStore.Save();
                    }

                    CheckGuildWar();

                    if (now >= nextFlagCheck)
                    {
                        nextFlagCheck = now.AddSeconds(5);
                        TrackUnions();
                        RefreshBrickFlags();
                        if (treasury == null)
                            SpawnTreasury();
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }

        static void Initialize(DateTime now)
        {
            initialized = true;
            lastGuildWarState = MsgSchedules.GuildWar != null ? MsgSchedules.GuildWar.Proces : ProcesType.Dead;
            bool firstRun = !KingdomWarStore.Load();

            // Only one Kingdom per server: keep the one with the most Gold Bricks.
            var kingdoms = Role.Instance.Union.UnionPoll.Values.Where(p => p != null && p.IsKingdom == 1).OrderByDescending(p => p.GoldBrick).ToArray();
            for (int i = 1; i < kingdoms.Length; i++)
            {
                kingdoms[i].IsKingdom = 0;
                try
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                        kingdoms[i].UpdateToUnion(rec.GetStream());
                }
                catch (Exception e) { MyConsole.SaveException(e); }
            }

            foreach (var union in Role.Instance.Union.UnionPoll.Values)
            {
                if (union == null)
                    continue;
                if (firstRun && union.GoldBrick == 0)
                    union.GoldBrick = KingdomWarConfig.StartingBricks; // official: a union starts with 500 Gold Bricks
                KnownUnions.Add(union.UID);
            }
            // A server restarted during the war keeps fighting until 21:30.
            lastWarDay = 0;
            lastCloseNoticeDay = DayKey(now);
            SpawnTreasury();
            KingdomWarStore.Save();
        }

        /// <summary>Newly founded unions start with the official 500 Gold Bricks; removed unions drop their Loot Token.</summary>
        static void TrackUnions()
        {
            bool changed = false;
            {
                foreach (var union in Role.Instance.Union.UnionPoll.Values)
                {
                    if (union != null && KnownUnions.Add(union.UID))
                    {
                        if (union.GoldBrick == 0)
                            union.GoldBrick = KingdomWarConfig.StartingBricks;
                        changed = true;
                    }
                }
                foreach (var uid in KnownUnions.ToArray())
                {
                    if (!Role.Instance.Union.UnionPoll.ContainsKey(uid))
                    {
                        KnownUnions.Remove(uid);
                        Registrations.Remove(uid);
                        changed = true;
                    }
                }
            }
            if (changed)
                KingdomWarStore.Save();
        }
        #endregion

        #region Settlement (22:00) and crowning
        static void Settle()
        {
            var current = GetKingdom();
            Role.Instance.Union best = null;
            foreach (var union in Role.Instance.Union.UnionPoll.Values)
            {
                if (union == null || union.GoldBrick == 0)
                    continue;
                if (best == null || union.GoldBrick > best.GoldBrick)
                {
                    best = union;
                    continue;
                }
                if (union.GoldBrick < best.GoldBrick)
                    continue;
                // Tie: the reigning Kingdom keeps the throne, otherwise the older union (lower id).
                bool bestReigns = current != null && best.UID == current.UID;
                bool unionReigns = current != null && union.UID == current.UID;
                if (unionReigns || (!bestReigns && union.UID < best.UID))
                    best = union;
            }
            // Carried Gold Bricks expire with the settlement.
            ClearCarriedBricks();
            if (best == null)
                return;
            if (current != null && current.UID == best.UID)
            {
                Broadcast("Kingdom War settlement: " + best.Name + " keeps the throne with " + best.GoldBrick + " Gold Bricks. Long live Emperor " + best.Emperor + "!",
                    MsgMessage.ChatMode.Center);
                return;
            }
            Crown(best, "has collected the most Gold Bricks (" + best.GoldBrick + ")");
        }

        /// <summary>Upgrades the union to the Kingdom and degrades the previous Kingdom to Union.</summary>
        static void Crown(Role.Instance.Union union, string reason)
        {
            var old = GetKingdom();
            if (old != null && old.UID == union.UID)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (old != null)
                {
                    old.IsKingdom = 0;
                    try { old.UpdateToUnion(stream); }
                    catch (Exception e) { MyConsole.SaveException(e); }
                }
                union.IsKingdom = 1;
                try { union.UpdateToKingdom(stream); }
                catch (Exception e) { MyConsole.SaveException(e); }
            }
            if (old != null)
                Broadcast("The Kingdom " + old.Name + " has been defeated and degraded to Union.", MsgMessage.ChatMode.System);
            Broadcast("Congratulations! " + union.Name + " " + reason + " and has upgraded to the Kingdom! " + union.Emperor + " is crowned as Emperor!",
                MsgMessage.ChatMode.Center);
            Broadcast("Congratulations! " + union.Name + " " + reason + " and has upgraded to the Kingdom! " + union.Emperor + " is crowned as Emperor!",
                MsgMessage.ChatMode.System);
            SendToUnion(union, "Your Union has upgraded to Kingdom!", MsgMessage.ChatMode.System);
            MsgSchedules.UnionWar?.SyncWinner(union);
            try { Database.LeagueTable.Save(); }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        /// <summary>When the server has no Kingdom, the Guild War winner's union is crowned (official rule for new servers).</summary>
        static void CheckGuildWar()
        {
            var gw = MsgSchedules.GuildWar;
            if (gw == null)
                return;
            var state = gw.Proces;
            if (state == ProcesType.Dead && lastGuildWarState != ProcesType.Dead && gw.Winner != null && gw.Winner.GuildID != 0)
                pendingGuildWarGuild = gw.Winner.GuildID;
            lastGuildWarState = state;
            if (pendingGuildWarGuild == 0)
                return;
            if (GetKingdom() != null)
            {
                pendingGuildWarGuild = 0;
                return;
            }
            Role.Instance.Guild guild;
            if (!Role.Instance.Guild.GuildPoll.TryGetValue(pendingGuildWarGuild, out guild) || guild == null)
            {
                pendingGuildWarGuild = 0;
                return;
            }
            var union = guild.GetUnion;
            if (union != null)
            {
                pendingGuildWarGuild = 0;
                Crown(union, "won the Guild War");
                return;
            }
            if (Role.Instance.Union.UnionPoll.Count > 0)
            {
                // Unions exist: the Kingdom is decided by Gold Bricks at the next settlement.
                pendingGuildWarGuild = 0;
                return;
            }
            // No union on the server: the winning guild founds one as soon as its leader is online.
            var leader = guild.GetGuildLeader;
            Client.GameClient owner;
            if (leader == null || !Database.Server.GamePoll.TryGetValue(leader.UID, out owner) || owner == null || owner.Player == null
                || owner.Player.MyGuild == null || owner.Player.MyGuildMember == null || owner.Player.MyUnion != null)
                return;
            string name = guild.GuildName;
            if (!Role.Instance.Union.CheckName(name))
                return;
            pendingGuildWarGuild = 0;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var created = Role.Instance.Union.Create(stream, owner, name);
                created.AddGuild(stream, owner.Player.MyGuild);
                created.GoldBrick = KingdomWarConfig.StartingBricks;
                KnownUnions.Add(created.UID);
                Crown(created, "won the Guild War");
            }
            KingdomWarStore.Save();
        }
        #endregion

        #region Loot Token
        public enum IssueResult { Ok, NeedCps, Failed }

        /// <summary>A Union Leader issues a Loot Token against the Kingdom. On NeedCps, cpsNeeded is the Union Fund shortfall.</summary>
        public static IssueResult IssueLootToken(Client.GameClient client, bool payShortfallWithCps, out uint cpsNeeded, out string message)
        {
            cpsNeeded = 0;
            message = null;
            try
            {
                if (!KingdomWarConfig.Enabled)
                {
                    message = "The Kingdom War is not available.";
                    return IssueResult.Failed;
                }
                var player = client.Player;
                if (!player.OnMyOwnServer)
                {
                    message = "You can only issue this token in your own server.";
                    return IssueResult.Failed;
                }
                if (!player.IsUnionEmperror)
                {
                    message = "Only the Union Leader can issue a Loot Token.";
                    return IssueResult.Failed;
                }
                DateTime now = DateTime.Now;
                if (!KingdomWarConfig.IsPlunderDay(now.DayOfWeek) || now.TimeOfDay >= KingdomWarConfig.LootTokenClose)
                {
                    message = "The Loot Token can only be issued at 00:00 - " + (KingdomWarConfig.LootTokenClose - TimeSpan.FromSeconds(1)).ToString(@"hh\:mm\:ss") + " on Monday or Friday.";
                    return IssueResult.Failed;
                }
                lock (SyncRoot)
                {
                    var union = player.MyUnion;
                    var kingdom = GetKingdom();
                    if (kingdom == null)
                    {
                        message = "There is no Kingdom to plunder on this server.";
                        return IssueResult.Failed;
                    }
                    if (union.IsKingdom == 1 || union.UID == kingdom.UID)
                    {
                        message = "The Kingdom cannot plunder its own Treasury.";
                        return IssueResult.Failed;
                    }
                    int today = DayKey(now);
                    if (RegistrationDay != today)
                    {
                        Registrations.Clear();
                        RegistrationDay = today;
                    }
                    if (Registrations.ContainsKey(union.UID))
                    {
                        message = "Your Union has already issued a Loot Token today.";
                        return IssueResult.Failed;
                    }
                    ulong cost = KingdomWarConfig.LootTokenFund;
                    if (union.Treasury < cost)
                    {
                        ulong shortfall = cost - union.Treasury;
                        if (!payShortfallWithCps)
                        {
                            cpsNeeded = (uint)Math.Min(shortfall, uint.MaxValue);
                            message = "Failed to issue the token. Insufficient Union Fund. Pay " + cpsNeeded + " CPs to make up the expense?";
                            return IssueResult.NeedCps;
                        }
                        if (player.ConquerPoints < shortfall)
                        {
                            message = "Not enough CPs.";
                            return IssueResult.Failed;
                        }
                        player.ConquerPoints -= (uint)shortfall;
                        union.Treasury = 0;
                    }
                    else
                        union.Treasury -= cost;

                    Registrations[union.UID] = new Registration() { UnionUID = union.UID, KingdomUID = kingdom.UID, UnionName = union.Name };
                    union.PlunderTarget = kingdom.Name;
                    if (string.IsNullOrEmpty(kingdom.InvadingUnion) || kingdom.InvadingUnion == "None")
                        kingdom.InvadingUnion = union.Name;
                    KingdomWarStore.Save();

                    TimeSpan start = KingdomWarConfig.WarStart;
                    Broadcast("Union " + union.Name + " has announced a Plunder War against the Kingdom " + kingdom.Name + ", and will start at "
                        + start.Hours.ToString("00") + ":" + start.Minutes.ToString("00") + " today.", MsgMessage.ChatMode.System);
                    message = "You issued a Loot Token against the Kingdom " + kingdom.Name + ". The Plunder War starts at "
                        + start.Hours.ToString("00") + ":" + start.Minutes.ToString("00") + " today.";
                    return IssueResult.Ok;
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                message = "Failed to issue the token.";
                return IssueResult.Failed;
            }
        }
        #endregion

        #region Plunder War
        static void StartWar(DateTime end)
        {
            var kingdom = GetKingdom();
            var attackers = new List<Role.Instance.Union>();
            if (kingdom != null)
            {
                foreach (var reg in Registrations.Values)
                {
                    var union = GetUnion(reg.UnionUID);
                    if (union != null && union.IsKingdom == 0 && reg.KingdomUID == kingdom.UID)
                        attackers.Add(union);
                }
            }
            if (attackers.Count == 0)
            {
                if (Registrations.Count > 0)
                {
                    Registrations.Clear();
                    KingdomWarStore.Save();
                }
                return;
            }
            warState = ProcesType.Alive;
            warEnd = end;
            warKingdomUID = kingdom.UID;
            shieldsUsed = 0;
            shieldUntil = DateTime.MinValue;
            plundering.Clear();
            if (treasury == null)
                SpawnTreasury();

            foreach (var union in attackers)
                union.PlunderTarget = kingdom.Name;
            kingdom.InvadingUnion = attackers[0].Name;

            string names = string.Join(", ", attackers.Select(p => p.Name).Take(5).ToArray()) + (attackers.Count > 5 ? "..." : "");
            Broadcast("The Plunder War has started! " + names + " are looting the Treasury of the Kingdom " + kingdom.Name + " in Twin City until "
                + end.ToString("HH:mm") + ".", MsgMessage.ChatMode.Center);
            Broadcast("The Plunder War has started! " + names + " are looting the Treasury of the Kingdom " + kingdom.Name + " in Twin City until "
                + end.ToString("HH:mm") + ".", MsgMessage.ChatMode.System);

            uint serverId = ServerID;
            var attackerIds = new HashSet<uint>(attackers.Select(p => p.UID));
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (client == null || client.Player == null || !client.Player.InUnion)
                        continue;
                    uint uid = client.Player.MyUnion.UID;
                    if (attackerIds.Contains(uid))
                    {
                        // "The Plunder War against [%s] %s has started! Go attack the Kingdom`s Treasury for Gold Bricks!" (accept -> LeagueOpt 64)
                        client.Send(stream.LeagueOptCreate((MsgLeagueOpt.ActionID)63, 0, serverId, 0, Safe16(kingdom.Name)));
                        SendBeRob(client, stream, 0, new[] { kingdom.UID });
                    }
                    else if (uid == kingdom.UID)
                    {
                        // Plunder War started + "defend our Treasury" box (accept -> LeagueOpt 71).
                        client.Send(stream.LeagueOptCreate(MsgLeagueOpt.ActionID.PlunderWar, 0, serverId, 0, Safe16(attackers[0].Name)));
                        SendBeRob(client, stream, 1, attackerIds.ToArray());// after LeagueOpt 69, or the defend box is not shown
                        client.SendSysMesage("Pay attention! " + names + " are looting the Treasury of your Kingdom! Heroes, hurry and defend our Treasury!",
                            MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                    }
                }
            }
        }

        /// <summary>2641: the unions the client must treat as enemies (type 0 attacker side, 1 defending Kingdom) for the
        /// Loot PK mode, plus the "Plunder War will end in" banner. Keys with and without the server id (the client uses
        /// 0 when it cannot find its own server in the 2500 list).</summary>
        static void SendBeRob(Client.GameClient client, ServerSockets.Packet stream, uint type, uint[] unionUids)
        {
            var msg = new MsgLeagueRobOpt.BeRob() { Type = type, EndTime = (uint)new DateTimeOffset(warEnd).ToUnixTimeSeconds() };
            foreach (var uid in unionUids)
            {
                msg.Unions.Add(new MsgLeagueRobOpt.BeRobUnion() { Key = ServerID * 1000000u + uid });
                if (ServerID != 0)
                    msg.Unions.Add(new MsgLeagueRobOpt.BeRobUnion() { Key = uid });
                if (msg.Unions.Count >= 120)
                    break;// 1024-byte client buffer
            }
            client.Send(stream.CreateLeagueBeRob(msg));
        }

        /// <summary>Re-sends the war lists to a player who logs in or joins while the Plunder War is running.</summary>
        public static void SendWarState(Client.GameClient client)
        {
            try
            {
                if (client == null || client.Player == null || !client.Player.InUnion || warState != ProcesType.Alive)
                    return;
                uint uid = client.Player.MyUnion.UID;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (Registrations.ContainsKey(uid))
                        SendBeRob(client, stream, 0, new[] { warKingdomUID });
                    else if (uid == warKingdomUID)
                        SendBeRob(client, stream, 1, Registrations.Keys.ToArray());
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        static void EndWar()
        {
            warState = ProcesType.Dead;
            var kingdom = GetUnion(warKingdomUID);
            var attackers = Registrations.Values.Select(r => GetUnion(r.UnionUID)).Where(u => u != null).ToList();
            uint serverId = ServerID;
            var attackerIds = new HashSet<uint>(attackers.Select(p => p.UID));
            string kingdomName = kingdom != null ? kingdom.Name : "";
            string firstAttacker = attackers.Count > 0 ? attackers[0].Name : "";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (client == null || client.Player == null || !client.Player.InUnion)
                        continue;
                    uint uid = client.Player.MyUnion.UID;
                    if (attackerIds.Contains(uid))
                    {
                        // "The Plunder War raised by your Union against [%s] %s has ended."
                        client.Send(stream.LeagueOptCreate((MsgLeagueOpt.ActionID)67, 0, serverId, 0, Safe16(kingdomName)));
                        client.Send(stream.LeagueOptCreate((MsgLeagueOpt.ActionID)70, 0, serverId, 0, ""));
                    }
                    else if (uid == warKingdomUID)
                    {
                        // "The Plunder War raised by [%s] %s against your Kingdom has ended."
                        client.Send(stream.LeagueOptCreate((MsgLeagueOpt.ActionID)68, 0, serverId, 0, Safe16(firstAttacker)));
                        client.Send(stream.LeagueOptCreate((MsgLeagueOpt.ActionID)70, 0, serverId, 0, ""));
                    }
                }
            }
            foreach (var union in attackers)
                union.PlunderTarget = "None";
            if (kingdom != null)
                kingdom.InvadingUnion = "None";

            foreach (var p in plundering.Values.ToArray())
                SendTo(p.UID, "The Plunder War has ended. You failed to grab a Gold Brick.");
            plundering.Clear();
            Registrations.Clear();
            shieldUntil = DateTime.MinValue;
            KingdomWarStore.Save();
            Broadcast("The Plunder War against the Kingdom " + kingdomName + " has ended. Hand in your Gold Bricks to the Kingdom Mission Envoy before the 22:00 settlement!",
                MsgMessage.ChatMode.Center);
            Broadcast("The Plunder War against the Kingdom " + kingdomName + " has ended. Hand in your Gold Bricks to the Kingdom Mission Envoy before the 22:00 settlement!",
                MsgMessage.ChatMode.System);
        }

        /// <summary>Treasury dialog: start grabbing a Gold Brick.</summary>
        public static bool StartPlunder(Client.GameClient client, out string message)
        {
            message = null;
            try
            {
                var player = client.Player;
                lock (SyncRoot)
                {
                    if (warState != ProcesType.Alive)
                    {
                        message = "You can only plunder the Treasury during the Plunder War.";
                        return false;
                    }
                    if (!player.InUnion || !Registrations.ContainsKey(player.MyUnion.UID))
                    {
                        message = "Only heroes from a Union that issued a Loot Token can plunder this Treasury.";
                        return false;
                    }
                    if (player.Level < KingdomWarConfig.PlunderMinLevel || player.Reborn < KingdomWarConfig.PlunderMinReborn)
                    {
                        message = "You must reach Level " + KingdomWarConfig.PlunderMinLevel + (KingdomWarConfig.PlunderMinReborn > 0 ? " of the " + (KingdomWarConfig.PlunderMinReborn == 1 ? "1st" : "2nd") + " rebirth" : "") + " to plunder Gold Bricks.";
                        return false;
                    }
                    if (!player.Alive)
                    {
                        message = "You are dead.";
                        return false;
                    }
                    if (ShieldActive)
                    {
                        message = "Dragon Shield is taking effect. Remaining Time: " + ShieldSecondsLeft + " seconds.";
                        return false;
                    }
                    var kingdom = GetUnion(warKingdomUID);
                    if (kingdom == null || kingdom.GoldBrick == 0)
                    {
                        message = "The Treasury is empty.";
                        return false;
                    }
                    if (treasury == null || player.Map != treasury.Map || Role.Core.GetDistance(player.X, player.Y, treasury.X, treasury.Y) > KingdomWarConfig.PlunderRange)
                    {
                        message = "Get closer to the Treasury.";
                        return false;
                    }
                    if (plundering.ContainsKey(player.UID))
                    {
                        message = "You are already plundering the Treasury.";
                        return false;
                    }
                    plundering[player.UID] = new Plunder() { UID = player.UID, Start = DateTime.Now, Map = player.Map, X = player.X, Y = player.Y };
                    SendCountDown(client, (uint)KingdomWarConfig.PlunderSeconds);
                    message = "You started plundering the Treasury. Stay here for " + KingdomWarConfig.PlunderSeconds + " seconds to grab a Gold Brick. If you move or get killed, you will get nothing.";
                    return true;
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                message = "Failed to plunder.";
                return false;
            }
        }

        static void UpdatePlunders(DateTime now)
        {
            if (plundering.Count == 0)
                return;
            int seconds = KingdomWarConfig.PlunderSeconds;
            foreach (var p in plundering.Values.ToArray())
            {
                Client.GameClient client;
                if (!Database.Server.GamePoll.TryGetValue(p.UID, out client) || client == null || client.Player == null)
                {
                    plundering.Remove(p.UID);
                    continue;
                }
                var player = client.Player;
                if (!player.Alive || player.Map != p.Map || Role.Core.GetDistance(player.X, player.Y, p.X, p.Y) > 1
                    || !player.InUnion || !Registrations.ContainsKey(player.MyUnion.UID))
                {
                    plundering.Remove(p.UID);
                    SendCountDown(client, 0);
                    client.SendSysMesage("Plundering interrupted. You did not get a Gold Brick.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                    continue;
                }
                if (ShieldActive)
                {
                    plundering.Remove(p.UID);
                    client.SendSysMesage("Dragon Shield is taking effect. The Treasury cannot be plundered now.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                    continue;
                }
                if ((now - p.Start).TotalSeconds < seconds)
                    continue;
                plundering.Remove(p.UID);
                var kingdom = GetUnion(warKingdomUID);
                if (kingdom == null || kingdom.GoldBrick == 0)
                {
                    client.SendSysMesage("The Treasury is empty.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                    continue;
                }
                AwardBrick(client, RollQuality(), kingdom.UID);
            }
        }

        static GoldBrickQuality RollQuality()
        {
            int r = Program.GetRandom.Next(0, 100);
            if (r < 3) return GoldBrickQuality.Super;
            if (r < 13) return GoldBrickQuality.Elite;
            if (r < 30) return GoldBrickQuality.Unique;
            if (r < 60) return GoldBrickQuality.Refined;
            return GoldBrickQuality.Normal;
        }

        public static MsgUpdate.Flags BrickFlag(GoldBrickQuality q)
        {
            switch (q)
            {
                case GoldBrickQuality.Refined: return MsgUpdate.Flags.GoldBrickRefined;
                case GoldBrickQuality.Unique: return MsgUpdate.Flags.GoldBrickUnique;
                case GoldBrickQuality.Elite: return MsgUpdate.Flags.GoldBrickElite;
                case GoldBrickQuality.Super: return MsgUpdate.Flags.Goldbrick;
                default: return MsgUpdate.Flags.GoldBrickNormal;
            }
        }
        public static string BrickName(GoldBrickQuality q)
        {
            switch (q)
            {
                case GoldBrickQuality.Refined: return "Refined Gold Brick";
                case GoldBrickQuality.Unique: return "Unique Gold Brick";
                case GoldBrickQuality.Elite: return "Elite Gold Brick";
                case GoldBrickQuality.Super: return "Super Gold Brick";
                default: return "Gold Brick";
            }
        }
        /// <summary>Gold Bricks moved from the Kingdom to the plunderer's Union when the brick is handed in.</summary>
        public static uint BrickAmount(GoldBrickQuality q)
        {
            switch (q)
            {
                case GoldBrickQuality.Refined: return 20;
                case GoldBrickQuality.Unique: return 30;
                case GoldBrickQuality.Elite: return 50;
                case GoldBrickQuality.Super: return 100;
                default: return 10;
            }
        }
        /// <summary>Official hand-in rewards: Potency Points and Union Fund (Kingdom Mission "Gold Brick Plunder") and Kingdom Deeds.</summary>
        static void BrickReward(GoldBrickQuality q, out int potency, out uint fund, out uint deeds)
        {
            int n = (int)q; // 1..5
            potency = 1000 * n;
            fund = (uint)(100 * n);
            switch (q)
            {
                case GoldBrickQuality.Refined: deeds = 70; break;
                case GoldBrickQuality.Unique: deeds = 100; break;
                case GoldBrickQuality.Elite: deeds = 150; break;
                case GoldBrickQuality.Super: deeds = 200; break;
                default: deeds = 50; break;
            }
        }

        static void AwardBrick(Client.GameClient client, GoldBrickQuality quality, uint kingdomUID)
        {
            var player = client.Player;
            CarriedBrick old;
            if (Carried.TryGetValue(player.UID, out old) && old.AddedFlag)
                player.RemoveFlag(BrickFlag(old.Quality));
            var brick = new CarriedBrick() { Quality = quality, KingdomUID = kingdomUID };
            brick.AddedFlag = player.AddFlag(BrickFlag(quality), Role.StatusFlagsBigVector32.PermanentFlag, false);
            Carried[player.UID] = brick;
            KingdomWarStore.Save();
            client.SendSysMesage("You grabbed a " + BrickName(quality) + "! Hand it in to the Kingdom Mission Envoy. You can plunder again for a better one.",
                MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
            client.SendSysMesage("You grabbed a " + BrickName(quality) + "!", MsgMessage.ChatMode.Center, MsgMessage.MsgColor.yellow);
        }

        static void RefreshBrickFlags()
        {
            foreach (var kv in Carried)
            {
                Client.GameClient client;
                if (Database.Server.GamePoll.TryGetValue(kv.Key, out client) && client != null && client.Player != null && client.FullLoading)
                {
                    var flag = BrickFlag(kv.Value.Quality);
                    if (!client.Player.ContainFlag(flag))
                        kv.Value.AddedFlag = client.Player.AddFlag(flag, Role.StatusFlagsBigVector32.PermanentFlag, false) || kv.Value.AddedFlag;
                }
            }
        }

        static void ClearCarriedBricks()
        {
            foreach (var kv in Carried)
            {
                Client.GameClient client;
                if (kv.Value.AddedFlag && Database.Server.GamePoll.TryGetValue(kv.Key, out client) && client != null && client.Player != null)
                    client.Player.RemoveFlag(BrickFlag(kv.Value.Quality));
            }
            Carried.Clear();
        }

        /// <summary>MsgAction SubmitGoldBrick (436): the client's Treasury dialog (Kingdom Mission Envoy) submit button.</summary>
        public static bool HasCarriedBrick(uint uid)
        {
            lock (SyncRoot)
                return Carried.ContainsKey(uid);
        }
        public static void SubmitGoldBrick(Client.GameClient client)
        {
            try
            {
                if (client == null || client.Player == null)
                    return;
                var player = client.Player;
                lock (SyncRoot)
                {
                    CarriedBrick brick;
                    if (!Carried.TryGetValue(player.UID, out brick))
                    {
                        client.SendSysMesage("You have no Gold Brick to submit.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        return;
                    }
                    if (!player.InUnion)
                    {
                        client.SendSysMesage("You should join a Union to submit Gold Bricks.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        return;
                    }
                    var union = player.MyUnion;
                    if (union.UID == brick.KingdomUID)
                    {
                        client.SendSysMesage("You can't submit Gold Bricks plundered from your own Kingdom.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        return;
                    }
                    Carried.Remove(player.UID);
                    if (brick.AddedFlag)
                        player.RemoveFlag(BrickFlag(brick.Quality));
                    KingdomWarStore.Save();

                    var kingdom = GetUnion(brick.KingdomUID);
                    if (kingdom == null || kingdom.GoldBrick == 0)
                    {
                        client.SendSysMesage("Failed to submit. The Treasure has been looted.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        return;
                    }
                    uint amount = Math.Min(BrickAmount(brick.Quality), kingdom.GoldBrick);
                    kingdom.GoldBrick -= amount;
                    union.GoldBrick = (uint)Math.Min((ulong)union.GoldBrick + amount, uint.MaxValue);

                    int potency; uint fund, deeds;
                    BrickReward(brick.Quality, out potency, out fund, out deeds);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (player.InnerPower != null)
                            player.InnerPower.AddPotency(stream, client, potency);
                    }
                    player.UpdateKingdomTreasury(fund);
                    uint room = player.KingDomDeeds >= 300 ? 0 : 300 - player.KingDomDeeds;
                    player.KingDomDeeds += Math.Min(deeds, room);

                    client.SendSysMesage("You submitted a " + BrickName(brick.Quality) + ": " + amount + " Gold Bricks moved from the Kingdom " + kingdom.Name + " to your Union. You received "
                        + potency + " Potency Points, " + fund + " points to Union Fund and " + Math.Min(deeds, room) + " Kingdom Deeds.",
                        MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                    SendToUnion(union, player.Name + " submitted a " + BrickName(brick.Quality) + ". Our Union`s Gold Bricks: " + union.GoldBrick + ".", MsgMessage.ChatMode.System);
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }

        /// <summary>The Emperor and Left/Right Marshal can raise the Dragon Shield during the Plunder War (3 minutes).</summary>
        public static bool ActivateDragonShield(Client.GameClient client, out string message)
        {
            message = null;
            try
            {
                var player = client.Player;
                lock (SyncRoot)
                {
                    if (warState != ProcesType.Alive || !player.InUnion || player.MyUnion.UID != warKingdomUID)
                    {
                        message = "The Dragon Shield can only be activated by the Kingdom during the Plunder War.";
                        return false;
                    }
                    var rank = player.UnionMemeber.Rank;
                    if (rank != Role.Instance.Union.Member.MilitaryRanks.Emperor && rank != Role.Instance.Union.Member.MilitaryRanks.LeftMar
                        && rank != Role.Instance.Union.Member.MilitaryRanks.RightMar)
                    {
                        message = "Only the Emperor and the Left/Right Marshal can activate the Dragon Shield.";
                        return false;
                    }
                    if (ShieldActive)
                    {
                        message = "Dragon Shield is taking effect. Remaining Time: " + ShieldSecondsLeft + " seconds.";
                        return false;
                    }
                    if (shieldsUsed >= KingdomWarConfig.DragonShieldsPerWar)
                    {
                        message = "The Dragon Shield has already been used in this Plunder War.";
                        return false;
                    }
                    var kingdom = player.MyUnion;
                    ulong cost = KingdomWarConfig.DragonShieldFund;
                    if (kingdom.Treasury < cost)
                    {
                        message = "Insufficient Kingdom Fund (" + cost + " needed).";
                        return false;
                    }
                    kingdom.Treasury -= cost;
                    shieldsUsed++;
                    shieldUntil = DateTime.Now.AddSeconds(KingdomWarConfig.DragonShieldSeconds);
                    foreach (var p in plundering.Keys.ToArray())
                        SendTo(p, "Dragon Shield is taking effect. The Treasury cannot be plundered now.");
                    plundering.Clear();
                    Broadcast(player.Name + " activated the Dragon Shield of the Kingdom " + kingdom.Name + "! The Treasury cannot be plundered for "
                        + (KingdomWarConfig.DragonShieldSeconds / 60) + " minutes.", MsgMessage.ChatMode.Center);
                    message = "The Dragon Shield is up for " + KingdomWarConfig.DragonShieldSeconds + " seconds.";
                    return true;
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                message = "Failed to activate the Dragon Shield.";
                return false;
            }
        }

        /// <summary>Accepting the Plunder War invitation (LeagueOpt 64 / 71 / 69) or the Treasury dialog: go to the Treasury.</summary>
        public static void JoinWar(Client.GameClient client, bool? attacker)
        {
            try
            {
                if (client == null || client.Player == null)
                    return;
                var player = client.Player;
                ushort x, y; uint map;
                lock (SyncRoot)
                {
                    if (warState != ProcesType.Alive || treasury == null)
                        return;
                    bool isAttacker = player.InUnion && Registrations.ContainsKey(player.MyUnion.UID);
                    bool isDefender = player.InUnion && player.MyUnion.UID == warKingdomUID;
                    if (attacker == true && !isAttacker) return;
                    if (attacker == false && !isDefender) return;
                    if (!isAttacker && !isDefender) return;
                    map = treasury.Map;
                    x = treasury.X;
                    y = (ushort)(treasury.Y + 3);
                }
                if (!player.Alive || player.DynamicID != 0 || Program.BlockTeleportMap.Contains(player.Map) || !player.OnMyOwnServer)
                {
                    client.SendSysMesage("You can't go to the Treasury from here.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                    return;
                }
                Role.GameMap gmap;
                if (Database.Server.ServerMaps.TryGetValue(map, out gmap))
                {
                    ushort fx = x, fy = y;
                    FindFreeTile(gmap, x, y, false, out fx, out fy);
                    x = fx; y = fy;
                }
                client.Teleport(x, y, map);
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        #endregion

        #region Treasury NPC
        static void SpawnTreasury()
        {
            try
            {
                Role.GameMap map;
                if (!Database.Server.ServerMaps.TryGetValue(KingdomWarConfig.TreasuryMap, out map) || map == null)
                    return;
                // Twin City's square (307-317, 273-283) is a non-walkable block in the official map: it is the base
                // the Treasury stands on. Use the configured cell as is; a free-tile search pushed it off the square.
                ushort x = KingdomWarConfig.TreasuryX, y = KingdomWarConfig.TreasuryY;
                if (x >= map.bounds.Width || y >= map.bounds.Height)
                {
                    if (!FindFreeTile(map, KingdomWarConfig.TreasuryX, KingdomWarConfig.TreasuryY, true, out x, out y))
                        return;
                }
                var npc = Game.MsgNpc.Npc.Create();
                npc.UID = TreasuryNpcUID;
                npc.X = x;
                npc.Y = y;
                npc.Map = map.ID;
                npc.Mesh = TreasuryMesh;
                npc.NpcType = Role.Flags.NpcType.Talker;
                npc.ObjType = Role.MapObjectType.Npc;
                map.AddNpc(npc);
                treasury = npc;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }

        static bool FindFreeTile(Role.GameMap map, ushort cx, ushort cy, bool area, out ushort fx, out ushort fy)
        {
            for (int r = 0; r <= 8; r++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (Math.Max(Math.Abs(dx), Math.Abs(dy)) != r)
                            continue;
                        int x = cx + dx, y = cy + dy;
                        if (x <= 0 || y <= 0 || x >= ushort.MaxValue || y >= ushort.MaxValue)
                            continue;
                        if (area ? AreaValid(map, x, y) : map.ValidLocation((ushort)x, (ushort)y))
                        {
                            fx = (ushort)x;
                            fy = (ushort)y;
                            return true;
                        }
                    }
                }
            }
            fx = cx;
            fy = cy;
            return false;
        }
        /// <summary>The 3x3 block an NPC blocks must be free walkable ground (no other NPC, no wall).</summary>
        static bool AreaValid(Role.GameMap map, int x, int y)
        {
            for (int ax = x - 1; ax <= x + 1; ax++)
                for (int ay = y - 1; ay <= y + 1; ay++)
                    if (ax <= 0 || ay <= 0 || !map.ValidLocation((ushort)ax, (ushort)ay))
                        return false;
            return true;
        }
        #endregion

        #region Messages
        static unsafe void SendCountDown(Client.GameClient client, uint seconds)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery action = new ActionQuery();
                action.ObjId = client.Player.UID;
                action.Type = ActionType.CountDown;
                action.dwParam = seconds;
                client.Send(stream.ActionCreate(&action));
            }
        }
        internal static string Safe16(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            return s.Length > 15 ? s.Substring(0, 15) : s;
        }
        static void Broadcast(string text, MsgMessage.ChatMode mode)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage(text, MsgMessage.MsgColor.red, mode).GetArray(stream));
            }
        }
        static void SendToUnion(Role.Instance.Union union, string text, MsgMessage.ChatMode mode)
        {
            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client != null && client.Player != null && client.Player.MyUnion != null && client.Player.MyUnion.UID == union.UID)
                    client.SendSysMesage(text, mode, MsgMessage.MsgColor.yellow);
            }
        }
        static void SendTo(uint uid, string text)
        {
            Client.GameClient client;
            if (Database.Server.GamePoll.TryGetValue(uid, out client) && client != null)
                client.SendSysMesage(text, MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
        }

        /// <summary>Text for the Treasury dialog.</summary>
        public static string DescribeTreasury()
        {
            lock (SyncRoot)
            {
                var kingdom = GetKingdom();
                if (kingdom == null)
                    return "This is the Kingdom Treasury. There is no Kingdom on this server yet: at " + KingdomWarConfig.SettleTime.ToString(@"hh\:mm")
                        + " the Union with the most Gold Bricks will upgrade to Kingdom.";
                string text = "This is the Treasury of the Kingdom " + kingdom.Name + " (Emperor: " + kingdom.Emperor + "). Gold Bricks in Treasury: " + kingdom.GoldBrick + ".";
                if (warState == ProcesType.Alive)
                {
                    text += " The Plunder War is on until " + warEnd.ToString("HH:mm") + "!";
                    if (ShieldActive)
                        text += " Dragon Shield is taking effect. Remaining Time: " + ShieldSecondsLeft + " seconds.";
                }
                else
                    text += " Union Leaders can issue a Loot Token at 00:00 - 19:59 on Monday or Friday to plunder it at 21:00 - 21:30.";
                return text;
            }
        }
        #endregion
    }
}
