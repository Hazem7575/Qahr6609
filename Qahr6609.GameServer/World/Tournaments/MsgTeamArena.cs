using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgTeamArena
    {

        public static ConcurrentDictionary<uint, User> ArenaPoll = new ConcurrentDictionary<uint, User>();

        public class User
        {
            public Game.MsgServer.MsgTeamArenaInfo Info;
            public string Name = "None";
            public uint UID;
            public ushort Level;
            public byte Class;
            public uint Mesh;

            public byte GetGender
            {
                get
                {
                    if (Mesh % 10 >= 3)
                        return 0;
                    else
                        return 1;
                }
            }
            public uint LastSeasonArenaPoints;
            public uint LastSeasonWin;
            public uint LastSeasonLose;
            public uint LastSeasonRank;

            public uint Cheers;

            public void Reset()
            {
                Cheers = 0;
            }

            public void ApplayInfo(Role.Player player)
            {
                Name = player.Name;
                UID = player.UID;
                Level = player.Level;
                Class = player.Class;
                Mesh = player.Mesh;
            }

            public User()
            {
                Info = Game.MsgServer.MsgTeamArenaInfo.Create();
            }

            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                writer.Add(UID).Add(Name).Add(Level).Add(Class).Add(Mesh)
                    .Add(Info.ArenaPoints).Add(Info.CurrentHonor).Add(Info.HistoryHonor)
                    .Add(Info.TodayBattles).Add(Info.TodayWin).Add(Info.TotalLose).Add(Info.TotalWin)
                    .Add(LastSeasonArenaPoints).Add(LastSeasonWin)
                    .Add(LastSeasonLose).Add(LastSeasonRank);
                return writer.Close();
            }
            internal void Load(string Line)
            {
                if (Line == null)
                    return;
                Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(Line, '/');
                UID = reader.Read((uint)0);
                Name = reader.Read("None");
                Level = reader.Read((ushort)0);
                Class = reader.Read((byte)0);
                Mesh = reader.Read((uint)0);
                Info.ArenaPoints = reader.Read((uint)0);
                Info.CurrentHonor = reader.Read((uint)0);
                Info.HistoryHonor = reader.Read((uint)0);
                Info.TodayBattles = reader.Read((uint)0);
                Info.TodayWin = reader.Read((uint)0);
                Info.TotalLose = reader.Read((uint)0);
                Info.TodayWin = reader.Read((uint)0);
                LastSeasonArenaPoints = reader.Read((uint)0);
                LastSeasonWin = reader.Read((uint)0);
                LastSeasonLose = reader.Read((uint)0);
                LastSeasonRank = reader.Read((uint)0);
            }
        }

        public static User[] Top10 = new User[10];
        public static User[] Top1000Today = new User[1000];
        public static User[] Top1000 = new User[1000];

        public List<uint> BlockArenaMaps = new List<uint>() { 1858, 1860, 4020, 4000, 4003, 4006, 4008, 4009 ,6000, 6001, 1017,1080, 1081, 2060, 6002, 6003, 601, 700, 1038, MsgClassPKWar.MapID, 1036, 1764, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID
        , MsgSuperGuildWar.MapID};
        public ConcurrentDictionary<uint, Client.GameClient> Registered;
        public Extensions.Counter MatchCounter = new Extensions.Counter(1);

        public ConcurrentDictionary<uint, Match> MatchesRegistered;

        public ProcesType Proces { get; set; }

        public Extensions.Time32 CreateMatchesStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.TeamArena_CreateMatches);
        public Extensions.Time32 VerifyMatchesStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.TeamArena_VerifyMatches);
        public Extensions.Time32 CheckGroupsStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.TeamArena_CheckGroups);

        public MsgTeamArena()
        {
            Proces = ProcesType.Dead;
            Registered = new ConcurrentDictionary<uint, Client.GameClient>();
            MatchesRegistered = new ConcurrentDictionary<uint, Match>();
        }
        // Today's ranking is rebuilt at most every [Performance] ArenaRankRebuildSec seconds (default 15; 0 = after
        // every match as before) from the kernel tick instead of after every single match. Everyone in ArenaPoll
        // stays ranked exactly as before; the new array is published only once it is complete.
        private static readonly object RankSync = new object();
        private static volatile bool RankDirty;
        private static int RankBuiltTick = Environment.TickCount;
        internal static void RequestRankUpdate()
        {
            if (Conquer6609.Framework.Settings.Int("Performance", "ArenaRankRebuildSec", 15) <= 0)
                UpdateRank();
            else
                RankDirty = true;
        }
        /// <summary>Rebuilds now if a rebuild is pending: call before anything reads TodayRank for rewards, honor,
        /// a reset or a broadcast, so those see exactly the ranks the original per-match rebuild produced.</summary>
        internal static void FlushRank()
        {
            if (RankDirty)
                UpdateRank();
        }
        internal static void RankTick()
        {
            if (!RankDirty)
                return;
            int sec = Conquer6609.Framework.Settings.Int("Performance", "ArenaRankRebuildSec", 15);
            if (unchecked(Environment.TickCount - RankBuiltTick) < sec * 1000)
                return;
            UpdateRank();
        }
        public static void UpdateRank()
        {
            lock (RankSync)
            {
                RankDirty = false;
                RankBuiltTick = Environment.TickCount;
                var top = new User[1000];
                var array = ArenaPoll.Values.ToArray();
                foreach (var user in array)
                    user.Info.TodayRank = 0;
                var Rank = array.OrderByDescending(p => p.Info.ArenaPoints).ToArray();
                for (int x = 0; x < Rank.Length; x++)
                {
                    if (x == 1000)
                        break;
                    top[x] = Rank[x];
                    Rank[x].Info.TodayRank = (uint)(x + 1);
                }
                Top1000Today = top;
            }
        }

        public void CreateRankTop10()
        {
            lock (Top10)
            {
                Top10 = new User[10];
                var array = ArenaPoll.Values.ToArray();
                var Rank = array.OrderByDescending(p => p.LastSeasonArenaPoints).ToArray();
                for (int x = 0; x < Rank.Length; x++)
                {
                    if (x == 10)
                        break;
                    var element = Rank[x];
                    Top10[x] = element;
                }
            }
        }
        public void CreateRankTop1000()
        {
            lock (Top1000)
            {
                Top1000 = new User[1000];
                var array = ArenaPoll.Values.ToArray();
                var Rank = array.OrderByDescending(p => p.Info.CurrentHonor).ToArray();
                for (int x = 0; x < Rank.Length; x++)
                {
                    if (x == 1000)
                        break;
                    Top1000[x] = Rank[x];
                }
            }
        }
        public void CheckGroups(Extensions.Time32 clock)
        {
            RankTick();
            if (clock > CheckGroupsStamp)
            {
                if (MatchesRegistered.Count > 0)
                {
                    DateTime Now = DateTime.Now;
                    var ArrayMatches = MatchesRegistered.Values.ToArray();
                    foreach (var group in ArrayMatches)
                    {
                        if (Now > group.StartTimer.AddSeconds(5))
                        {
                            if (!group.Done)
                            {
                                if (Now > group.StartTimer.AddMinutes(3))
                                {
                                    group.End();
                                }
                            }
                            else
                            {
                                if (Now > group.DoneStamp.AddSeconds(4))
                                {
                                    group.DoneStamp = DateTime.Now.AddDays(1);
                                    group.Export();
                                    group.Win(group.Winner(), group.Loser());
                                }
                            }
                        }
                    }
                }
                CheckGroupsStamp.Value = clock.Value + KernelThread.TeamArena_CheckGroups;
            }
        }
        public void CreateMatches(Extensions.Time32 clock)
        {
            if (clock > CreateMatchesStamp)
            {
                DateTime Timer = DateTime.Now;
                if (Registered.Count < 2)
                    return;
                if (Timer.Second % 6 == 0)
                {
                    ConcurrentQueue<Client.GameClient> Remover = new ConcurrentQueue<Client.GameClient>();

                    var array = Registered.Values.ToArray();
                    var Players = array.OrderByDescending(p => p.TeamArenaStatistic.Info.ArenaPoints).ToArray();

                    Client.GameClient user1 = null;
                    Client.GameClient user2 = null;
                    foreach (var user in Players)
                    {
                        if (user.Team == null || user.InQualifier() || user.Socket.Alive == false )
                        {
                            Remover.Enqueue(user);
                            continue;
                        }
                        if (BlockArenaMaps.Contains(user.Player.Map))
                            continue;
                        if (user.Team.ArenaState == Role.Instance.Team.StateType.FindMatch
                            && user.TeamArenaStatistic.Info.Status == MsgServer.MsgTeamArenaInfo.Action.WaitingForOpponent)
                        {
                            if (user1 == null)
                                user1 = user;
                            else if (user2 == null)
                                user2 = user;
                            if (user1 != null && user2 != null)
                                break;
                        }
                    }
                    if (user1 != null && user2 != null)
                    {
                        if (user1.Team == null)
                        {
                            Remover.Enqueue(user1);
                            return;
                        }
                        if (user2.Team == null)
                        {
                            Remover.Enqueue(user2);
                            return;
                        }
                        user1.Team.ArenaState = user2.Team.ArenaState = Role.Instance.Team.StateType.WaitForBox;
                        user1.Team.AcceptBoxShow = user2.Team.AcceptBoxShow = DateTime.Now;
                        user1.TeamArenaStatistic.Info.Status = user2.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.WaitingInactive;
                        user1.TeamArenaStatistic.Info.Send(user1);
                        user2.TeamArenaStatistic.Info.Send(user2);

                        Match match = new Match(user1.Team, user2.Team, MatchCounter.Next);
                        match.SendSignUp(user1);
                        match.SendSignUp(user2);
                        MatchesRegistered.TryAdd(match.MatchUID, match);

                        UnRegistered(user1);
                        UnRegistered(user2);
                    }

                    Client.GameClient remover;
                    while (Remover.TryDequeue(out remover))
                        Registered.TryRemove(remover.Player.UID, out remover);
                }
                CreateMatchesStamp.Value = clock.Value + KernelThread.TeamArena_CreateMatches;
            }
        }
        public void VerifyMatches(Extensions.Time32 clock)
        {
            if (clock > VerifyMatchesStamp)
            {
                if (MatchesRegistered.Count == 0)
                    return;
                var Array = MatchesRegistered.Values.ToArray();

                foreach (var match in Array)
                {
                    if (match.Teams[0].Members.Count == 0)
                    {
                        match.End(match.Teams[0]);
                        return;
                    }
                    if (match.Teams[1].Members.Count == 0)
                    {
                        match.End(match.Teams[1]);
                        return;
                    }
                    if (match.Teams[0] != null && match.Teams[1] != null)
                    {
                        if (match.Teams[0].ArenaState == Role.Instance.Team.StateType.WaitForBox
                            || match.Teams[1].ArenaState == Role.Instance.Team.StateType.WaitForBox)
                        {
                            if (DateTime.Now > match.Teams[0].AcceptBoxShow.AddSeconds(60))
                            {
                                if (match.Teams[0].ArenaState == Role.Instance.Team.StateType.WaitForBox)
                                    match.Win(match.Teams[1], match.Teams[0]);
                                else
                                    match.Win(match.Teams[0], match.Teams[1]);

                                return;
                            }
                        }
                        if (match.Teams[0].ArenaState == Role.Instance.Team.StateType.WaitForOther
                            && !match.Teams[0].AcceptBox)
                        {
                            match.Win(match.Teams[1], match.Teams[0]);
                        }
                        else if (match.Teams[1].ArenaState == Role.Instance.Team.StateType.WaitForOther
                           && !match.Teams[1].AcceptBox)
                        {
                            match.Win(match.Teams[0], match.Teams[1]);
                        }
                        else if (match.Teams[0].ArenaState == Role.Instance.Team.StateType.WaitForOther
                            && match.Teams[1].ArenaState == Role.Instance.Team.StateType.WaitForOther)
                        {
                            if (!match.Teams[0].AcceptBox || !match.Teams[1].AcceptBox)
                            {
                                if (!match.Teams[0].AcceptBox)
                                {
                                    match.Win(match.Teams[1], match.Teams[0]);
                                }
                                else
                                {
                                    match.Win(match.Teams[0], match.Teams[1]);
                                }
                            }
                            else
                            {
                                match.Teams[0].ArenaState = match.Teams[1].ArenaState = Role.Instance.Team.StateType.Fight;
                                match.Import();

                            }

                        }
                    }

                }
                VerifyMatchesStamp.Value = clock.Value + KernelThread.TeamArena_VerifyMatches;
            }
        }
        public unsafe class Match
        {
            public List<uint> Cheerers = new List<uint>();
            public ConcurrentDictionary<uint, Client.GameClient> Watchers = new ConcurrentDictionary<uint, Client.GameClient>();

            public void BeginWatching(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (dinamicID == 0)
                {
                    client.SendSysMesage("The match not started.");

                    return;
                }

                if (!client.Player.Alive)
                {
                    client.SendSysMesage("Please revive your character to watching that match");

                    return;
                }
                if (client.InQualifier() || client.IsWatching())
                {
                    client.SendSysMesage("You're already in a match.");

                    return;
                }

                if (Watchers.TryAdd(client.Player.UID, client))
                {

                    stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.RequestView, 0, 0, 0, Teams[0].Cheers, Teams[1].Cheers);
                    client.Send(stream.ArenaWatchersFinalize());


                    stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Teams[0].Cheers, Teams[1].Cheers);

                    var array = Watchers.Values.ToArray();
                    for (int x = 0; x < Watchers.Count; x++)
                        stream.AddItemArenaWatchers(array[x].TeamArenaStatistic);

                    stream.ArenaWatchersFinalize();

                    foreach (var user in Watchers.Values)
                        user.Send(stream);
                    foreach (var team in Teams)
                    {
                        foreach (var user in team.GetMembers())
                        {
                            if (user.Player.DynamicID == dinamicID)
                            {
                                user.Send(stream);
                            }
                        }
                    }
                    SendScore();

                    client.TeamArenaWatchingGroup = this;
                    client.Teleport((ushort)Program.GetRandom.Next(35, 70), (ushort)Program.GetRandom.Next(35, 70), 700, dinamicID);
                }
            }
            public unsafe void DoLeaveWatching(Client.GameClient client)
            {
                Client.GameClient remover;
                if (client.IsWatching() && Watchers.TryRemove(client.Player.UID, out remover) && client.Player.Map == 700 && client.Player.DynamicID == dinamicID)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, 0, (uint)Watchers.Count, Teams[0].Cheers, Teams[1].Cheers);
                        var array = Watchers.Values.ToArray();
                        for (int x = 0; x < Watchers.Count; x++)
                            stream.AddItemArenaWatchers(array[x].TeamArenaStatistic);

                        client.Send(stream.ArenaWatchersFinalize());


                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Teams[0].Cheers, Teams[1].Cheers);
                        for (int x = 0; x < Watchers.Count; x++)
                            stream.AddItemArenaWatchers(array[x].TeamArenaStatistic);

                        stream.ArenaWatchersFinalize();

                        foreach (var user in Watchers.Values)
                            user.Send(stream);
                        foreach (var team in Teams)
                        {
                            foreach (var user in team.GetMembers())
                            {
                                if (user.Player.DynamicID == dinamicID)
                                {
                                    user.Send(stream);
                                }
                            }
                        }

                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, 0, 0, 0, 0);
                        client.Send(stream.ArenaWatchersFinalize());
                    }

                    SendScore();
                    client.TeamArenaWatchingGroup = null;
                    client.TeleportCallBack();

                }
                client.TeamArenaWatchingGroup = null;
            }
            public unsafe void DoCheer(ServerSockets.Packet stream, Client.GameClient client, uint uid)
            {
                if (client.IsWatching() && !Cheerers.Contains(client.Player.UID))
                {
                    Cheerers.Add(client.Player.UID);

                    if (Teams[0].Members.ContainsKey(uid))
                    {
                        Teams[0].Cheers++;
                    }
                    else if (Teams[1].Members.ContainsKey(uid))
                    {
                        Teams[1].Cheers++;
                    }

                    stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Teams[0].Cheers, Teams[1].Cheers);

                    var array = Watchers.Values.ToArray();
                    for (int x = 0; x < Watchers.Count; x++)
                        stream.AddItemArenaWatchers(array[x].TeamArenaStatistic);

                    stream.ArenaWatchersFinalize();
                    foreach (var user in Watchers.Values)
                        user.Send(stream);
                    foreach (var team in Teams)
                    {
                        foreach (var user in team.GetMembers())
                        {
                            if (user.Player.DynamicID == dinamicID)
                            {
                                user.Send(stream);
                            }
                        }
                    }

                    SendScore();
                }
            }
            public bool Done;

            public uint dinamicID;
            public bool Imported = false;
            private uint UID;
            public DateTime DoneStamp;
            public DateTime StartTimer;
            public uint MatchUID
            {
                get { return UID; }
            }

            public Role.Instance.Team Winner()
            {
                var team = Teams.Where(p => p.Status != Role.Instance.Team.TournamentProces.Loser && p.Status != Role.Instance.Team.TournamentProces.None).SingleOrDefault();
                return team;
            }
            public Role.Instance.Team Loser()
            {
                var team = Teams.Where(p => p.Status == Role.Instance.Team.TournamentProces.Loser).SingleOrDefault();
                return team;
            }
            public Role.Instance.Team[] Teams;

            public Match(Role.Instance.Team team1, Role.Instance.Team team2, uint _uid)
            {
                Teams = new Role.Instance.Team[2];
                Teams[0] = team1;
                Teams[1] = team2;
                UID = _uid;

                DoneStamp = new DateTime();
                team1.TeamArenaMatch = team2.TeamArenaMatch = this;

                foreach (var team in Teams)
                {
                    team.Cheers = 0;
                    team.Damage = 0;
                    team.Status = Role.Instance.Team.TournamentProces.None;
                }
                StartTimer = DateTime.Now;
            }
            public bool TryGetOpponent(uint MyUID, out Role.Instance.Team Opponentteam)
            {
                foreach (var team in Teams)
                {
                    if (team.UID != MyUID)
                    {
                        Opponentteam = team;
                        return true;
                    }
                }
                Opponentteam = null;
                return false;
            }

            public void SendSignUp(Client.GameClient user)
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    user.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.StartCountDown, MsgTeamArenaSignup.DialogButton.SignUp, user));
                }
            }
            public void Export()
            {
                if (Imported == true)
                {
                    Match m_math;
                    MsgSchedules.TeamArena.MatchesRegistered.TryRemove(UID, out m_math);

                    foreach (var user in Watchers.Values)
                        DoLeaveWatching(user);

                    foreach (var team in Teams)
                    {
                        foreach (var user in team.GetMembers())
                        {
                            if (user.Player.Map == 700 && user.Player.DynamicID == dinamicID)
                            {
                                user.TeleportCallBack();
                                user.Player.RestorePkMode();
                            }
                        }
                    }
                }
            }
            public void Win(Role.Instance.Team winner, Role.Instance.Team loser)
            {
                winner.Status = Role.Instance.Team.TournamentProces.Winner;
                loser.Status = Role.Instance.Team.TournamentProces.Loser;

                if (winner.TeamArenaMatch != null && loser.TeamArenaMatch != null)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        winner.TeamArenaMatch = null;
                        loser.TeamArenaMatch = null;

                        int diff = Program.GetRandom.Next(30, 50);

                        foreach (var team in Teams)
                        {
                            foreach (var user in team.GetMembers())
                            {
                                //if (user.Player.DynamicID == dinamicID)
                                {
                                    user.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.NotSignedUp;
                                    user.TeamArenaStatistic.Info.Send(user);

                                }
                            }
                            team.ArenaState = Role.Instance.Team.StateType.FindMatch;
                        }

                        foreach (var user in winner.GetMembers())
                        {
                            //if (user.Player.DynamicID == dinamicID)
                            {
                                user.ArenaPoints += (uint)diff;

                                //user.TeamArenaStatistic.Info.ArenaPoints += (uint)diff;
                                user.TeamArenaStatistic.Info.TodayWin++;
                                user.TeamArenaStatistic.Info.TotalWin++;
                                user.TeamArenaStatistic.Info.TodayBattles++;

                                if (user.TeamArenaStatistic.Info.TodayWin == 9)
                                {
                                    if (user.Inventory.HaveSpace(1))
                                        user.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                                    else
                                        user.Inventory.AddReturnedItem(stream, 723912, 1);
                                }
                                if (user.TeamArenaStatistic.Info.TodayWin == 20)
                                    user.HeroRewards.AddGoal(507);

                                user.Activeness.OnTeamQualifierMatch();// "Team Qualifier" (12/24/36)

                                if (user.TeamArenaStatistic.Info.TodayBattles == 20)
                                {
                                    if (user.Inventory.HaveSpace(1))
                                        user.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                                    else
                                        user.Inventory.AddReturnedItem(stream, 723912, 1);
                                }

                            }
                        }
                        foreach (var user in loser.GetMembers())
                        {
                            // if (user.Player.DynamicID == dinamicID)
                            {
                                if (user.ArenaPoints > diff)
                                    user.ArenaPoints -= (uint)diff;

                                user.TeamArenaStatistic.Info.TodayBattles++;
                                user.TeamArenaStatistic.Info.TotalLose++;
                                user.Activeness.OnTeamQualifierMatch();// "Team Qualifier": the losing team competed too

                                if (user.TeamArenaStatistic.Info.TodayBattles == 20)
                                {
                                    if (user.Inventory.HaveSpace(1))
                                        user.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                                    else
                                        user.Inventory.AddReturnedItem(stream, 723912, 1);
                                }

                            }
                        }

                        // MsgTournaments.MsgSchedules.TeamArena.DoQuit(winner.Leader);
                        // MsgTournaments.MsgSchedules.TeamArena.DoQuit(loser.Leader);

                        RequestRankUpdate();
                        FlushRank();// the broadcast below shows TodayRank
                        StringBuilder builder = new StringBuilder();
                        if (winner.Leader.Player.MyGuild != null)
                        {
                            builder.Append("(");
                            builder.Append(winner.Leader.Player.MyGuild.GuildName.ToString());
                            builder.Append(") ");
                        }
                        builder.Append(winner.Leader.Player.Name);
                        builder.Append(" has defeated ");

                        if (loser.Leader.Player.MyGuild != null)
                        {
                            builder.Append("(");
                            builder.Append(loser.Leader.Player.MyGuild.GuildName.ToString());
                            builder.Append(") ");
                        }
                        builder.Append(loser.Leader.Player.Name);
                        if (winner.Leader.TeamArenaStatistic.Info.TodayRank > 0)
                        {
                            builder.Append(" in the Qualifier, and is currently ranked No. ");
                            builder.Append(winner.Leader.TeamArenaStatistic.Info.TodayRank);
                        }
                        else
                            builder.Append(" in the Qualifier");

                        builder.Append(".");


                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(builder.ToString(), MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Qualifier).GetArray(stream));

                        foreach (var user in loser.GetMembers())
                        {
                            user.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Dialog2, MsgTeamArenaSignup.DialogButton.SignUp, user));

                            user.TeamArenaStatistic.Reset();
                            user.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.NotSignedUp;
                            user.TeamArenaStatistic.Info.Send(user);
                        }

                        foreach (var user in winner.GetMembers())
                        {
                            user.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Dialog2, MsgTeamArenaSignup.DialogButton.Win, user));

                            user.TeamArenaStatistic.Reset();
                            user.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.NotSignedUp;
                            user.TeamArenaStatistic.Info.Send(user);
                        }

                        winner.ResetTeamArena();
                        loser.ResetTeamArena();

                        Match m_math;
                        MsgSchedules.TeamArena.MatchesRegistered.TryRemove(UID, out m_math);
                    }
                }
            }
            public void End()
            {
                End((Teams[0].Damage > Teams[1].Damage) ? Teams[1] : Teams[0]);
            }
            public void End(Role.Instance.Team loser)
            {
                if (Done) return;
                if (Teams[0].UID == loser.UID)
                {
                    Teams[0].Status = Role.Instance.Team.TournamentProces.Loser;
                    Teams[1].Status = Role.Instance.Team.TournamentProces.Winner;
                }
                else
                {
                    Teams[1].Status = Role.Instance.Team.TournamentProces.Loser;
                    Teams[0].Status = Role.Instance.Team.TournamentProces.Winner;
                }

                using (var rec = new ServerSockets.RecycledPacket())
                {

                    var stream = rec.GetStream();


                    foreach (var user in loser.GetMembers())
                    {
                        if (user.Player.DynamicID == dinamicID)
                        {
                            user.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Dialog, MsgTeamArenaSignup.DialogButton.Lose, loser.Leader));
                        }
                    }
                    foreach (var user in Winner().GetMembers())
                    {
                        if (user.Player.DynamicID == dinamicID)
                        {
                            user.HeroRewards.AddGoal(305);


                            user.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Dialog, MsgTeamArenaSignup.DialogButton.Win, Winner().Leader));


                        }
                    }

                    Done = true;
                    DoneStamp = DateTime.Now;
                }
            }
            public void RemoveJiangMode(Client.GameClient user)
            {
                if (user.Player.MyJiangHu != null)
                {
                    if (user.Player.JiangHuActive != 0)
                    {
                        user.Player.PkMode = Role.Flags.PKMode.Capture;
                        user.Player.MyJiangHu.OnJiangMode = false;
                        user.Player.MyJiangHu.DisableJiang(user);
                    }
                }
            }
            public void Import()
            {
                if (!Imported)
                {

                    var map = Database.Server.ServerMaps[700];
                    dinamicID = map.GenerateDynamicID();
                    foreach (var team in Teams)
                    {
                        team.Damage = 0;

                        ushort x = 0;
                        ushort y = 0;
                        map.GetRandCoord(ref x, ref y);

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            foreach (var user in team.GetMembers())
                            {
                                user.Teleport(x, y, 700, dinamicID);
                                user.Player.SetPkMode(Role.Flags.PKMode.Team);
                                user.Player.ProtectJumpAttack(10);
                                RemoveJiangMode(user);
                            }


                            Role.Instance.Team Opponent;
                            if (TryGetOpponent(team.UID, out Opponent))
                            {

                                stream.TeamArenaInfoPlayersCreate(MsgTeamArenaInfoPlayers.KindOfParticipants.Opponents, team.Leader.Player.UID, (uint)team.Members.Count);
                                foreach (var user in team.GetMembers())
                                    stream.AddItemTeamArenaInfoPlayers(user.TeamArenaStatistic);
                                stream.TeamArenaInfoPlayersFinalize();
                                Opponent.SendTeam(stream, 0);

                                Opponent.SendTeam(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.StartTheFight, MsgTeamArenaSignup.DialogButton.SignUp, team.Leader), 0);
                                Opponent.SendTeam(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Match, MsgTeamArenaSignup.DialogButton.MatchOn, team.Leader), 0);
                            }
                        }
                    }
                    Imported = true;
                    SendScore();
                }
            }
            public void SendScore()
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.TeamArenaMatchScoreCreate(Teams[0].Leader.Player.UID
                        , Teams[0].Leader.TeamArenaStatistic.Info.TodayRank
                        , Teams[0].TeamName
                        , Teams[0].Damage
                        , Teams[1].Leader.Player.UID
                        , Teams[1].Leader.TeamArenaStatistic.Info.TodayRank
                        , Teams[1].TeamName
                        , Teams[1].Damage);

                    foreach (var team in Teams)
                        team.SendTeam(stream, 0);


                    foreach (var user in Watchers.Values)
                        user.Send(stream);

                }

            }
        }
        public void DoSignup(Client.GameClient client)
        {
            if (client.Team == null)
            {
                client.SendSysMesage("Sorry, you not have team.", MsgServer.MsgMessage.ChatMode.Agate, MsgServer.MsgMessage.MsgColor.red);

                return;
            }
            if (!client.Team.TeamLider(client))
            {

                client.SendSysMesage("Sorry, you are not the team leader.", MsgServer.MsgMessage.ChatMode.Agate, MsgServer.MsgMessage.MsgColor.red);
                return;
            }
            if (client.TeamArenaStatistic.Info.Status != MsgServer.MsgTeamArenaInfo.Action.NotSignedUp)
            {
                client.SendSysMesage("You already joined a qualifier arena! Quit the other one and sign up again.", MsgServer.MsgMessage.ChatMode.Agate, MsgServer.MsgMessage.MsgColor.red);

                return;
            }
            if (client.InQualifier())
            {
                client.SendSysMesage("You already joined a qualifier arena! Quit the other one and sign up again.", MsgServer.MsgMessage.ChatMode.Agate, MsgServer.MsgMessage.MsgColor.red);


                return;
            }
            if (client.TeamArenaStatistic.Info.ArenaPoints == 0)
            {
                client.SendSysMesage("You don't have enough Arena Points.");

                return;
            }
            if (BlockArenaMaps.Contains(client.Player.Map))
            {

                return;
            }

            /* if (WaitingPlayerList.ContainsKey(client.Entity.UID))
             {
                 if (client.QualifierGroup == null)
                     WaitingPlayerList.Remove(client.Entity.UID);
                 else
                     return;
             }*/
            if (!Game.MsgTournaments.MsgTeamArena.ArenaPoll.ContainsKey(client.Player.UID))
                Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);

            Registered.TryAdd(client.Player.UID, client);
            client.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.WaitingForOpponent;
            client.Team.ArenaState = Role.Instance.Team.StateType.FindMatch;
            client.TeamArenaStatistic.Info.Send(client);
            //RequestGroupList(client, 1);
        }
        public void UnRegistered(Client.GameClient client)
        {
            Client.GameClient remover;
            Registered.TryRemove(client.Player.UID, out remover);
        }
        public void DoQuit(Client.GameClient client, bool InMathat = false)
        {
            if (client.Team == null)
            {
                return;
            }
            if (!client.Team.TeamLider(client))
            {
                return;
            }
            if (client.Team.TeamArenaMatch != null)
                client.Team.TeamArenaMatch.End(client.Team);
            else
            {
                client.Team.ResetTeamArena();
                client.TeamArenaStatistic.Reset();
            }

            UnRegistered(client);

            client.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.NotSignedUp;
            client.TeamArenaStatistic.Info.Send(client);
        }
        public unsafe void DoGiveUp(Client.GameClient client)
        {
            if (client.Team == null)
                return;
            if (!client.Team.TeamLider(client))
                return;
            if (client.Team.ArenaState == Role.Instance.Team.StateType.WaitForBox)
            {
                client.Team.AcceptBox = false;
                client.Team.ArenaState = Role.Instance.Team.StateType.WaitForOther;
            }
            else
            {
                client.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.WaitingInactive;
                client.TeamArenaStatistic.Info.Send(client);


                if (client.Team != null & client.Team.TeamArenaMatch != null)
                {
                    Role.Instance.Team Opponent;
                    if (client.Team.TeamArenaMatch.TryGetOpponent(client.Team.UID, out Opponent))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();


                            Opponent.Leader.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.OpponentGaveUp, MsgTeamArenaSignup.DialogButton.SignUp, Opponent.Leader));
                        }

                        Opponent.Leader.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.NotSignedUp;
                        client.TeamArenaStatistic.Info.Status = MsgServer.MsgTeamArenaInfo.Action.NotSignedUp;
                        client.TeamArenaStatistic.Info.Send(client);
                        Opponent.Leader.TeamArenaStatistic.Info.Send(Opponent.Leader);



                    }
                    /* if (WaitingPlayerList.ContainsKey(client.Entity.UID))
                    {
                        WaitingPlayerList.Remove(client.Entity.UID);
                        WaitingPlayerList.Remove(client.TeamArenaStatistic.PlayWith.Entity.UID);
                    }*/
                    if (!client.Team.TeamArenaMatch.Done)
                    {
                        client.Team.TeamArenaMatch.End(client.Team);
                    }
                    else
                    {
                        if (Opponent != null)
                            client.Team.TeamArenaMatch.Win(Opponent, client.Team);
                        else
                            client.Team.TeamArenaMatch.End(client.Team);
                    }
                }
            }
        }
    }
}
