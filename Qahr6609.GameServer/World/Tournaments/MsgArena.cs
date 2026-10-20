using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
   public class MsgArena
    {
       public enum ArenaIDs : uint
       {
           ShowPlayerRankList = 0xA,
           QualifierList = 0x6
       }

       public static ConcurrentDictionary<uint, User> ArenaPoll = new ConcurrentDictionary<uint, User>();

    
       public static User[] Top10 = new User[10];
       public static User[] Top1000Today = new User[1000];
       public static User[] Top1000 = new User[1000];

       public List<uint> BlockArenaMaps = new List<uint>() { 4020, 4000, 4003, 4006, 4008, 4009 ,6000, 6001, 1017,1080,1081,2060, 6002, 6003, 601, 700, 1038,MsgClassPKWar.MapID, 1036, 1764, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID
       , MsgSuperGuildWar.MapID};
       public ConcurrentDictionary<uint, Client.GameClient> Registered;
       public Extensions.Counter MatchCounter = new Extensions.Counter(1);

       public ConcurrentDictionary<uint, Match> MatchesRegistered;

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
       public class User
       {
           public enum StateType : byte
           {
               None = 0,
               FindMatch = 1,
               WaitForBox = 2,
               WaitForOther = 3,
               Fight = 4
           }
           public enum MatchStatus : byte
           {
               None,
               Winner,
               Loser
           }
           public Game.MsgServer.MsgArenaInfo Info;
           public string Name = "None";
           public uint UID;
           public ushort Level;
           public byte Class;
           public uint Mesh;
           public StateType ArenaState = StateType.None;
           public DateTime AcceptBoxShow = new DateTime();
           public bool AcceptBox = false;

           public MatchStatus QualifierStatus = MatchStatus.None;

           public uint Damage;


           public uint LastSeasonArenaPoints;
           public uint LastSeasonWin;
           public uint LastSeasonLose;
           public uint LastSeasonRank;

           public uint Cheers;

           public void Reset()
           {
               ArenaState = StateType.None;
               AcceptBox = false;
               Info.Status = MsgServer.MsgArenaInfo.Action.NotSignedUp;

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
               Info = new MsgArenaInfo();
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
       public ProcesType Proces { get; set; }

       public Extensions.Time32 CreateMatchesStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.Arena_CreateMatches);
       public Extensions.Time32 VerifyMatchesStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.Arena_VerifyMatches);
       public Extensions.Time32 CheckGroupsStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.Arena_CheckGroups);

       public MsgArena()
       {
           Proces = ProcesType.Dead;
           Registered = new ConcurrentDictionary<uint, Client.GameClient>();
           MatchesRegistered = new ConcurrentDictionary<uint, Match>();
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
               CheckGroupsStamp.Value = clock.Value + KernelThread.Arena_CheckGroups;
           }
       }
       private void CreateMatch(Client.GameClient user1, Client.GameClient user2)
       {
           user1.ArenaStatistic.ArenaState = user2.ArenaStatistic.ArenaState = User.StateType.WaitForBox;

           user2.ArenaStatistic.AcceptBoxShow = user1.ArenaStatistic.AcceptBoxShow = DateTime.Now.AddSeconds(60);
           //  user1.ArenaStatistic.AcceptBoxShow = DateTime.Now;

           using (var rec = new ServerSockets.RecycledPacket())
           {
               var stream = rec.GetStream();
               user1.Send(stream.ArenaInfoCreate(user1.ArenaStatistic.Info));
               user2.Send(stream.ArenaInfoCreate(user2.ArenaStatistic.Info));


               Match match = new Match(user1, user2, MatchCounter.Next);
               match.SendSignUp(stream, user1);
               match.SendSignUp(stream, user2);
               MatchesRegistered.TryAdd(match.MatchUID, match);

               UnRegistered(user1);
               UnRegistered(user2);
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
                   var Players = Registered.Values.ToArray();

                   ConcurrentQueue<Client.GameClient> Remover = new ConcurrentQueue<Client.GameClient>();

                   // Same candidate rule as before (the next two waiting players in Registered order), but every
                   // waiting pair is matched in this run instead of only the first pair every 6 seconds
                   // ([Performance] ArenaPairAllPerRun, default false = original: one pair per run).
                   bool pairAll = Conquer6609.Framework.Settings.Bool("Performance", "ArenaPairAllPerRun", false);
                   Client.GameClient user1 = null;
                   foreach (var user in Players)
                   {
                       if (BlockArenaMaps.Contains(user.Player.Map) || user.InQualifier() || user.Socket.Alive == false
                           || Game.MsgTournaments.MsgSchedules.ClanWar.Process == ProcesType.Alive
                           && Game.MsgTournaments.MsgSchedules.ClanWar.InClanWar(user)
                           || user.Player.Map == 10166
                           || user.IsConnectedInterServer())
                       {
                           Remover.Enqueue(user);
                           continue;
                       }
                       if (user.ArenaStatistic.ArenaState == User.StateType.FindMatch
                           && user.ArenaStatistic.Info.Status == MsgServer.MsgArenaInfo.Action.WaitingForOpponent)
                       {
                           if (user1 == null)
                           {
                               user1 = user;
                               continue;
                           }
                           CreateMatch(user1, user);
                           user1 = null;
                           if (!pairAll)
                               break;
                       }
                   }
                   Client.GameClient remover;
                   while (Remover.TryDequeue(out remover))
                       Registered.TryRemove(remover.Player.UID, out remover);
               }
               CreateMatchesStamp.Value = clock.Value + KernelThread.Arena_CreateMatches;
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
                   if (match.Players[0] != null && match.Players[1] != null)
                   {
                       if (match.Players[0].Player.Map != 700)
                           if (BlockArenaMaps.Contains(match.Players[0].Player.Map))
                           {
                               match.Win(match.Players[1], match.Players[0]);
                           }
                       if (match.Players[1].Player.Map != 700)
                           if (BlockArenaMaps.Contains(match.Players[1].Player.Map))
                           {
                               match.Win(match.Players[0], match.Players[1]);
                           }
                       if (match.Players[0].ArenaStatistic.ArenaState == User.StateType.WaitForBox
                           || match.Players[1].ArenaStatistic.ArenaState == User.StateType.WaitForBox)
                       {
                           if (DateTime.Now > match.Players[0].ArenaStatistic.AcceptBoxShow)
                           {
                               if (match.Players[0].ArenaStatistic.ArenaState == User.StateType.WaitForBox)
                                   match.Win(match.Players[1], match.Players[0]);
                               else
                                   match.Win(match.Players[0], match.Players[1]);

                               return;
                           }
                       }
                       if (match.Players[0].ArenaStatistic.ArenaState == User.StateType.WaitForOther
                           && !match.Players[0].ArenaStatistic.AcceptBox)
                       {
                           match.Win(match.Players[1], match.Players[0]);
                       }
                       else if (match.Players[1].ArenaStatistic.ArenaState == User.StateType.WaitForOther
                          && !match.Players[1].ArenaStatistic.AcceptBox)
                       {
                           match.Win(match.Players[0], match.Players[1]);
                       }
                       else if (match.Players[0].ArenaStatistic.ArenaState == User.StateType.WaitForOther
                           && match.Players[1].ArenaStatistic.ArenaState == User.StateType.WaitForOther)
                       {
                           if (!match.Players[0].ArenaStatistic.AcceptBox || !match.Players[1].ArenaStatistic.AcceptBox)
                           {
                               if (!match.Players[0].ArenaStatistic.AcceptBox)
                               {
                                   match.Win(match.Players[1], match.Players[0]);
                               }
                               else
                               {
                                   match.Win(match.Players[0], match.Players[1]);
                               }
                           }
                           else
                           {
                               match.Players[0].ArenaStatistic.ArenaState = match.Players[1].ArenaStatistic.ArenaState = User.StateType.Fight;
                               match.Import();

                           }

                       }
                   }

               }
               VerifyMatchesStamp.Value = clock.Value + KernelThread.Arena_VerifyMatches;
           }
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

               if (!Watchers.ContainsKey(client.Player.UID))
               {
                   client.ArenaWatchingGroup = this;


                   client.Teleport((ushort)Program.GetRandom.Next(35, 70), (ushort)Program.GetRandom.Next(35, 70), 700, dinamicID);

                   if (Watchers.TryAdd(client.Player.UID, client))
                   {

               
                       stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.RequestView, 0, 0, 0, Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);
                       client.Send(stream.ArenaWatchersFinalize());

                       stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count,
                           Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);
                       var array = Watchers.Values.ToArray();
                       for (int x = 0; x < Watchers.Count; x++)
                           stream.AddItemArenaWatchers(array[x].ArenaStatistic);

                       stream.ArenaWatchersFinalize();

                       foreach (var user in Watchers.Values)
                           user.Send(stream);
                       foreach (var user in Players)
                           user.Send(stream);

                       //SendScore(client);
                       SendScore();

                     
                      
                   }
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

                       stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, 0, (uint)Watchers.Count
                           , Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);

                       var array = Watchers.Values.ToArray();
                       for (int x = 0; x < Watchers.Count; x++)
                           stream.AddItemArenaWatchers(array[x].ArenaStatistic);

                       client.Send(stream.ArenaWatchersFinalize());

                       stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count
            , Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);

                       for (int x = 0; x < Watchers.Count; x++)
                           stream.AddItemArenaWatchers(array[x].ArenaStatistic);

                       stream.ArenaWatchersFinalize();
                       foreach (var user in Watchers.Values)
                           user.Send(stream);
                       foreach (var user in Players)
                           user.Send(stream);

                       stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, 0, 0, 0, 0);
                       client.Send(stream.ArenaWatchersFinalize());
                   }

                   SendScore();


                   client.ArenaWatchingGroup = null;

                   client.TeleportCallBack();

               }
               client.ArenaWatchingGroup = null;
           }
           public unsafe void DoCheer(ServerSockets.Packet stream, Client.GameClient client, uint uid)
           {
               if (client.IsWatching() && !Cheerers.Contains(client.Player.UID))
               {
                   Cheerers.Add(client.Player.UID);

                   if (Players[0].Player.UID == uid)
                       Players[0].ArenaStatistic.Cheers++;
                   else if (Players[1].ArenaStatistic.UID == uid)
                       Players[1].ArenaStatistic.Cheers++;

                   stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count,
                     Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);
                   var array = Watchers.Values.ToArray();
                   for (int x = 0; x < Watchers.Count; x++)
                       stream.AddItemArenaWatchers(array[x].ArenaStatistic);

                   stream.ArenaWatchersFinalize();
                   foreach (var user in Watchers.Values)
                       user.Send(stream);
                   foreach (var user in Players)
                       user.Send(stream);

                   SendScore();
               }
           }
           public bool Done;
           public bool Imported = false;
           public uint dinamicID;

           private uint UID;
           public DateTime DoneStamp;
           public DateTime StartTimer;
           public DateTime FightStamp;
           public uint MatchUID
           {
               get { return UID; }
           }

           public Client.GameClient Winner()
           {
               var client = Players.Where(p => p.ArenaStatistic.QualifierStatus != User.MatchStatus.Loser && p.ArenaStatistic.QualifierStatus != User.MatchStatus.None).SingleOrDefault();
               if (client == null)
                   return Players[0];
               return client;
           }
           public Client.GameClient Loser()
           {
               var client = Players.Where(p => p.ArenaStatistic.QualifierStatus == User.MatchStatus.Loser).SingleOrDefault();
               if (client == null)
                   return Players[0];
               return client;
           }
           public Client.GameClient[] Players;

           public Match(Client.GameClient user1, Client.GameClient user2, uint _uid)
           {
               Players = new Client.GameClient[2];
               Players[0] = user1;
               Players[1] = user2;
               UID = _uid;

               DoneStamp = new DateTime();
               user1.ArenaMatch = user2.ArenaMatch = this;
               user1.ArenaStatistic.QualifierStatus = user2.ArenaStatistic.QualifierStatus = User.MatchStatus.None;
               StartTimer = DateTime.Now;
           }
           public bool TryGetOpponent(uint MyUID, out Client.GameClient client)
           {
               foreach (var user in Players)
               {
                   if (MyUID != user.Player.UID)
                   {
                       client = user;
                       return true;
                   }
               }
               client = null;
               return false;
           }

           public void SendSignUp(ServerSockets.Packet stream, Client.GameClient user)
           {
               user.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.StartCountDown, MsgArenaSignup.DialogButton.SignUp, user));
           }
           public void Export()
           {
               if (Imported)
               {
                   Match m_math;
                   MsgSchedules.Arena.MatchesRegistered.TryRemove(UID, out m_math);

                   foreach (var user in Watchers.Values)
                       DoLeaveWatching(user);

                   foreach (var user in Players)
                   {
                       user.TeleportCallBack();
                       user.Player.RestorePkMode();
                   }
               }
           }
           public void Win(Client.GameClient winner, Client.GameClient loser)
           {
               winner.ArenaStatistic.QualifierStatus = User.MatchStatus.Winner;
               loser.ArenaStatistic.QualifierStatus = User.MatchStatus.Loser;

               if (winner.ArenaMatch != null && loser.ArenaMatch != null)
               {
                   using (var rec = new ServerSockets.RecycledPacket())
                   {
                       var stream = rec.GetStream();

                       winner.ArenaMatch = null;
                       loser.ArenaMatch = null;

                       int diff = Program.GetRandom.Next(30, 50);

                       winner.ArenaStatistic.Info.Status = MsgServer.MsgArenaInfo.Action.NotSignedUp;

                       winner.Send(stream.ArenaInfoCreate(winner.ArenaStatistic.Info));

                    
                       winner.ArenaStatistic.ArenaState = User.StateType.FindMatch;

                       loser.ArenaStatistic.Info.Status = MsgServer.MsgArenaInfo.Action.NotSignedUp;

                       loser.Send(stream.ArenaInfoCreate(loser.ArenaStatistic.Info));

                       loser.ArenaStatistic.ArenaState = User.StateType.FindMatch;


                       winner.ArenaPoints += (uint)diff;
                       if (loser.ArenaPoints > diff)
                           loser.ArenaPoints -= (uint)diff;

                       if (winner.ArenaStatistic.Info.TodayWin == 5)
                       {

                           winner.Player.AddChampionPoints(200);
                           winner.SendSysMesage("You received 200 ChampionPoints.", MsgMessage.ChatMode.System);
                           if (winner.Inventory.HaveSpace(1))
                               winner.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                           else
                               winner.Inventory.AddReturnedItem(stream, 723912);
                       }
                       if (winner.ArenaStatistic.Info.TodayWin == 10)
                       {

                           winner.Player.AddChampionPoints(200);
                           winner.SendSysMesage("You received 200 ChampionPoints.", MsgMessage.ChatMode.System);
                           if (winner.Inventory.HaveSpace(1))
                               winner.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                           else
                               winner.Inventory.AddReturnedItem(stream, 723912);
                       }
                       if (winner.ArenaStatistic.Info.TodayWin == 15)
                       {

                           winner.Player.AddChampionPoints(200);
                           winner.SendSysMesage("You received 200 ChampionPoints.", MsgMessage.ChatMode.System);
                           if (winner.Inventory.HaveSpace(1))
                               winner.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                           else
                               winner.Inventory.AddReturnedItem(stream, 723912);
                       }
                       if (winner.ArenaStatistic.Info.TodayWin == 20)
                       {
                           winner.HeroRewards.AddGoal(506);
                       }

                       winner.ArenaStatistic.Info.TodayWin++;
                       winner.ArenaStatistic.Info.TotalWin++;

                       loser.ArenaStatistic.Info.TodayBattles++;
                       loser.ArenaStatistic.Info.TotalLose++;


                       //winner.HeroRewards.AddGoal(305);

                       winner.Activeness.OnQualifierMatch();// "Qualifier" (7/19/31): both fighters competed
                       loser.Activeness.OnQualifierMatch();

                       if (winner.ArenaStatistic.Info.TodayBattles == 20)
                       {
                           winner.Player.AddChampionPoints(200);
                           winner.SendSysMesage("You received 200 ChampionPoints.", MsgMessage.ChatMode.System);
                           if (winner.Inventory.HaveSpace(1))
                               winner.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                           else
                               winner.Inventory.AddReturnedItem(stream, 723912);
                       }
                       if (loser.ArenaStatistic.Info.TodayBattles == 20)
                       {
                           loser.Player.AddChampionPoints(200);
                           loser.SendSysMesage("You received 200 ChampionPoints.", MsgMessage.ChatMode.System);
                           if (loser.Inventory.HaveSpace(1))
                               loser.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                           else
                               loser.Inventory.AddReturnedItem(stream, 723912);
                       }

                       RequestRankUpdate();
                       FlushRank();// the broadcast and the signup dialogs below show TodayRank

                       StringBuilder builder = new StringBuilder();
                       if (winner.Player.MyGuild != null)
                       {
                           builder.Append("(");
                           builder.Append(winner.Player.MyGuild.GuildName.ToString());
                           builder.Append(") ");
                       }
                       builder.Append(winner.Player.Name);
                       builder.Append(" has defeated ");

                       if (loser.Player.MyGuild != null)
                       {
                           builder.Append("(");
                           builder.Append(loser.Player.MyGuild.GuildName.ToString());
                           builder.Append(") ");
                       }
                       builder.Append(loser.Player.Name);
                       if (winner.ArenaStatistic.Info.TodayRank > 0)
                       {
                           builder.Append(" in the Qualifier, and is currently ranked No. ");
                           builder.Append(winner.ArenaStatistic.Info.TodayRank);
                       }
                       else
                           builder.Append(" in the Qualifier");

                       builder.Append(".");


                       Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(builder.ToString(), MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Qualifier).GetArray(stream));

                       loser.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Dialog2, MsgArenaSignup.DialogButton.SignUp, loser));
                       winner.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Dialog2, MsgArenaSignup.DialogButton.Win, winner));

                       winner.ArenaStatistic.Reset();
                       loser.ArenaStatistic.Reset();


                       winner.Send(stream.ArenaInfoCreate(winner.ArenaStatistic.Info));
                       loser.Send(stream.ArenaInfoCreate(loser.ArenaStatistic.Info));

                       Match m_math;
                       MsgSchedules.Arena.MatchesRegistered.TryRemove(UID, out m_math);
                   }
               }
           }
           public void End()
           {
               End((Players[0].ArenaStatistic.Damage > Players[1].ArenaStatistic.Damage) ? Players[1] : Players[0]);
           }
           public void End(Client.GameClient loser)
           {
               if (Done) 
                   return;
               Done = true;
               Players[0].Player.ProtectAttack(5 * 1000);
               Players[1].Player.ProtectAttack(5 * 1000);
               if (Players[0].Player.UID == loser.Player.UID)
               {
                   Players[0].ArenaStatistic.QualifierStatus = User.MatchStatus.Loser;
                   Players[1].ArenaStatistic.QualifierStatus = User.MatchStatus.Winner;
               }
               else
               {
                   Players[1].ArenaStatistic.QualifierStatus = User.MatchStatus.Loser;
                   Players[0].ArenaStatistic.QualifierStatus = User.MatchStatus.Winner;
               }

               using (var rec = new ServerSockets.RecycledPacket())
               {
                   var stream = rec.GetStream();
                   loser.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Dialog, MsgArenaSignup.DialogButton.Lose, loser));
                   Winner().Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Dialog, MsgArenaSignup.DialogButton.Win, Winner()));

               }

               Winner().HeroRewards.AddGoal(204);
               
            
               DoneStamp = DateTime.Now;
           }
           public void Import()
           {
               if (!Imported)
               {
                   var map = Database.Server.ServerMaps[700];
                   dinamicID = map.GenerateDynamicID();
                   foreach (var user in Players)
                   {
                       user.ArenaStatistic.Damage = 0;
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           user.Player.Revive(stream);
                       }
                       ushort x = 0;
                       ushort y = 0;
                       map.GetRandCoord(ref x, ref y);
                       user.Teleport(x, y, 700, dinamicID);
                       user.Player.ProtectJumpAttack(10);

                       if (user.Player.MyJiangHu != null)
                       {
                           if (user.Player.JiangHuActive != 0)
                           {
                               user.Player.PkMode = Role.Flags.PKMode.Capture;
                               user.Player.MyJiangHu.OnJiangMode = false;
                               user.Player.MyJiangHu.DisableJiang(user);
                           }
                       }
                       Client.GameClient Opponent;
                       if (TryGetOpponent(user.Player.UID, out Opponent))
                       {
                           using (var rec = new ServerSockets.RecycledPacket())
                           {
                               var stream = rec.GetStream();
                               Opponent.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.StartTheFight, MsgArenaSignup.DialogButton.SignUp, user));
                               Opponent.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Match, MsgArenaSignup.DialogButton.MatchOn, user));
                           }
                       }

                       user.Player.SetPkMode(Role.Flags.PKMode.PK);
                   }
                   Imported = true;
                   FightStamp = DateTime.Now;
                   SendScore();
               }
           }
           public void SendScore()
           {
               using (var rec = new ServerSockets.RecycledPacket())
               {

                   var stream = rec.GetStream();

                 stream =  stream.ArenaMatchScoreCreate(Players[0].Player.UID, Players[0].ArenaStatistic.Name, Players[0].ArenaStatistic.Damage
                       , Players[1].Player.UID, Players[1].ArenaStatistic.Name, Players[1].ArenaStatistic.Damage);

                   foreach (var user in Players)
                       user.Send(stream);

                   foreach (var user in Watchers.Values)
                       user.Send(stream);
               }
           }
         
       }
       public void DoSignup(ServerSockets.Packet stream, Client.GameClient client)
       {
           if (client.ArenaStatistic.Info.Status != MsgServer.MsgArenaInfo.Action.NotSignedUp)
           {
               client.SendSysMesage("You already joined a qualifier arena! Quit the other one and sign up again.", MsgServer.MsgMessage.ChatMode.Agate, MsgServer.MsgMessage.MsgColor.red);
              
              return;
           }
           if (client.InQualifier())
           {
               return;
           }
           if(client.Player.Map == 10166)
           {
               return;
           }
           if (client.ArenaStatistic.Info.ArenaPoints == 0)
               return;
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
           if (!Game.MsgTournaments.MsgArena.ArenaPoll.ContainsKey(client.Player.UID))
               Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);

           Registered.TryAdd(client.Player.UID, client);
           client.ArenaStatistic.Info.Status = MsgServer.MsgArenaInfo.Action.WaitingForOpponent;
           client.ArenaStatistic.ArenaState = User.StateType.FindMatch;

           client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
           //RequestGroupList(client, 1);
       }
       public void UnRegistered(Client.GameClient client)
       {
           Client.GameClient remover;
           Registered.TryRemove(client.Player.UID, out remover);
       }
       public void DoQuit(ServerSockets.Packet stream, Client.GameClient client, bool InMathat = false)
       {
           if (client.ArenaMatch != null)
               client.ArenaMatch.End(client);
           else
               client.ArenaStatistic.Reset();

          

           UnRegistered(client);

           client.ArenaStatistic.Info.Status = MsgServer.MsgArenaInfo.Action.NotSignedUp;

           client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
       }
       public unsafe void DoGiveUp(ServerSockets.Packet stream, Client.GameClient client)
       {
           if (client.ArenaStatistic.ArenaState == User.StateType.WaitForBox)
           {
               client.ArenaStatistic.AcceptBox = false;
               client.ArenaStatistic.ArenaState = User.StateType.WaitForOther;
           }
           else
           {
               client.ArenaStatistic.Info.Status = MsgServer.MsgArenaInfo.Action.WaitingInactive;
               client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));

               if (client.ArenaMatch != null)
               {
                   Client.GameClient Opponent;
                   if (client.ArenaMatch.TryGetOpponent(client.Player.UID, out Opponent))
                   {
                       Opponent.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.OpponentGaveUp, MsgArenaSignup.DialogButton.SignUp, client));

                       Opponent.ArenaStatistic.Info.Status = MsgServer.MsgArenaInfo.Action.NotSignedUp;
                       client.ArenaStatistic.Info.Status = MsgServer.MsgArenaInfo.Action.NotSignedUp;

                       Opponent.Send(stream.ArenaInfoCreate(Opponent.ArenaStatistic.Info));
                       client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
                   }
                   if (!client.ArenaMatch.Done)
                   {
                       client.ArenaMatch.End(client);
                   }
                   else
                   {
                       if (Opponent != null)
                           client.ArenaMatch.Win(Opponent, client);
                       else
                           client.ArenaMatch.End(client);
                   }
               }
           }
       }
    }
}
