using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgSchedules
    {
        // Events that are not part of official Conquer Online (their Twin City NPCs were removed).
        // Set to true to bring back the hourly tournaments, Elite/Super/Score guild wars, pole wars, Couples PK and Mr/Ms Conquer.
        public static bool CustomEvents = false;
        // [Performance] ScoreboardThrottle: 0 (default, original) = event scoreboards (class/nobility pole wars, guild
        // war, treasure thief, clan war) are rebuilt and broadcast on every scoring hit / every second;
        // 1 = at most once per second (clan war: every 8 s).
        public static readonly bool ScoreboardThrottle = Conquer6609.Framework.Settings.Int("Performance", "ScoreboardThrottle", 0) != 0;
        public static Extensions.Time32 Stamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.TournamentsStamp);
        public static Dictionary<TournamentType, ITournament> Tournaments = new Dictionary<TournamentType, ITournament>();
        public static ITournament CurrentTournament;
        internal static MsgGuildWar GuildWar;
        internal static MsgSuperGuildWar SuperGuildWar;
        internal static MsgArena Arena;
        internal static MsgTeamArena TeamArena;
        internal static MsgClassPKWar ClassPkWar;
        internal static MsgEliteTournament ElitePkTournament;
        internal static MsgTeamPkTournament TeamPkTournament;
        internal static MsgSkillTeamPkTournament SkillTeamPkTournament;
        internal static MsgCaptureTheFlag CaptureTheFlag;
        internal static MsgClanWar ClanWar;
        internal static MsgTowerOfMystery TowerOfMystery;
        internal static MsgPkWar PkWar;
        internal static MsgDisCity DisCity;
        internal static MsgPowerArena PowerArena;
        internal static MsgChristmasAnimation ChristmasAnimation;
        internal static MsgSquidwardOctopus SquidwardOctopus;
        internal static MsgDragonIsland DragonIsland;
        internal static MsgCpsArenaSingle ArenaCpsBattle;
        internal static MsgSteedRace SteedRace;
        internal static GuildWarScore GuildWarScore;
        internal static MsgCouples CouplesPKWar;
        internal static MsgEliteGuildWar EliteGuildWar;
        internal static MsgUnionWar UnionWar;
        internal static MsgQuizShow QuizShow;
        internal static void Create()
        {
            Tournaments.Add(TournamentType.None, new MsgNone(TournamentType.None));
            Tournaments.Add(TournamentType.BattleField, new MsgBattleField(TournamentType.BattleField));
            Tournaments.Add(TournamentType.LastManStand, new MsgLastManStand(TournamentType.LastManStand));
            Tournaments.Add(TournamentType.ExtremePk, new MsgExtremePk(TournamentType.ExtremePk));
            Tournaments.Add(TournamentType.KillerOfElite, new MsgTheKillerOfElite(TournamentType.KillerOfElite));
            Tournaments.Add(TournamentType.TreasureThief, new MsgTreasureThief(TournamentType.TreasureThief)); 
            Tournaments.Add(TournamentType.FreezeWar, new MsgFreezeWar(TournamentType.FreezeWar));
            Tournaments.Add(TournamentType.SkillTournament, new MsgSkillTournament(TournamentType.SkillTournament));
            Tournaments.Add(TournamentType.KingOfTheHill, new MsgKingOfTheHill(TournamentType.KingOfTheHill));
            // Quiz Show runs on its own schedule (not in the CurrentTournament slot, which the hourly custom events reuse).
            QuizShow = new MsgQuizShow(TournamentType.QuizShow);
            Tournaments.Add(TournamentType.QuizShow, QuizShow);


            ChristmasAnimation = new MsgChristmasAnimation();
            CurrentTournament = Tournaments[TournamentType.None];
            EliteGuildWar = new MsgEliteGuildWar();
            PowerArena = new MsgPowerArena();
            SquidwardOctopus = new MsgSquidwardOctopus();
            SuperGuildWar = new MsgSuperGuildWar();
            GuildWar = new MsgGuildWar();
            Arena = new MsgArena();
            TeamArena = new MsgTeamArena();
            ClassPkWar = new MsgClassPKWar(ProcesType.Dead);
            ElitePkTournament = new MsgEliteTournament();
            CaptureTheFlag = new MsgCaptureTheFlag();
            PowerArena = new MsgPowerArena();
            PkWar = new MsgPkWar();
            DisCity = new MsgDisCity();
            DragonIsland = new MsgDragonIsland(ProcesType.Dead);
            ArenaCpsBattle = new MsgCpsArenaSingle(ProcesType.Dead);
            UnionWar = new MsgUnionWar();
            TowerOfMystery = new MsgTowerOfMystery();
            CouplesPKWar = new MsgCouples();
            GuildWarScore = new GuildWarScore();
            SteedRace = new MsgSteedRace();
            TeamPkTournament = new MsgTeamPkTournament();
            SkillTeamPkTournament = new MsgSkillTeamPkTournament();
            MsgBroadcast.Create();
        }
        internal static void SendInvitation(string Name, string Prize, ushort X, ushort Y, ushort map, ushort DinamicID, int Secounds, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {

            string Message = " " + Name + " is about to begin! Will you join it? Prize[" + Prize + "]";

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                var packet = new Game.MsgServer.MsgMessage(Message, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer())
                        continue;
                    if (client.Player.Map == 1860 || client.Player.Map == 1858 || client.Player.Map == 1038)
                        continue;
                    client.Send(packet);
                    client.Player.MessageBox(Message, new Action<Client.GameClient>(user => user.Teleport(X, Y, map, DinamicID)), null, Secounds, messaj);
                }
            }
        }
        internal unsafe static void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red, bool SendScren = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var packet = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                    client.Send(packet);
            }
        }
        // Exact-second events fire once inside a short window after their second, so a kernel pass that
        // skips that second (pass took longer, timer jitter) can no longer make an event silently not start.
        // The start stays in the same minute (at most SecondWindow seconds late).
        private const int SecondWindow = 10;
        private static readonly long[] LastFiredMinute = new long[32];
        private static bool AtSecond(DateTime now, int id, int second)
        {
            if (now.Second < second || now.Second > second + SecondWindow)
                return false;
            long minute = now.Ticks / TimeSpan.TicksPerMinute;
            if (LastFiredMinute[id] == minute)
                return false;
            LastFiredMinute[id] = minute;
            return true;
        }

        internal static void CheckUp(Extensions.Time32 clock)
        {


            if (KernelThread.Due(clock, ref Stamp, KernelThread.TournamentsStamp))
            {
                DateTime Now64 = DateTime.Now;
                
                if (!Database.Server.FullLoading)
                    return;
                if (Arena.Proces == ProcesType.Dead)
                {
                    Arena.Proces = ProcesType.Alive;
                }
                if (TeamArena.Proces == ProcesType.Dead)
                {
                    TeamArena.Proces = ProcesType.Alive;
                }

                try
                {
                   
                    #region Hourly
                    CurrentTournament.CheckUp();
                    if (CustomEvents && Now64.Minute == 00 && AtSecond(Now64, 0, 4))
                    {
                        CurrentTournament = Tournaments[TournamentType.SkillTournament];
                        CurrentTournament.Open();
                        MyConsole.WriteLine(CurrentTournament.Type + ": Working Now" + "");
                    }
                    if (CustomEvents && Now64.Minute == 8 && AtSecond(Now64, 1, 4))
                    {
                        CurrentTournament = Tournaments[TournamentType.ExtremePk];
                        CurrentTournament.Open();
                        MyConsole.WriteLine(CurrentTournament.Type + ": Working Now" + "");
                    }
                    if (CustomEvents && Now64.Minute == 14 && AtSecond(Now64, 2, 4))
                    {

                        CurrentTournament = Tournaments[TournamentType.TreasureThief];

                        CurrentTournament.Open();
                        MyConsole.WriteLine(CurrentTournament.Type + ": Working Now" + "");
                    }
                    if (CustomEvents && Now64.Minute == 25 && AtSecond(Now64, 3, 4))
                    {

                        CurrentTournament = Tournaments[TournamentType.KingOfTheHill];

                        CurrentTournament.Open();
                        MyConsole.WriteLine(CurrentTournament.Type + ": Working Now" + "");
                    }
                    if (CustomEvents && Now64.Minute == 32 && AtSecond(Now64, 4, 3))
                    {

                        CurrentTournament = Tournaments[TournamentType.FreezeWar];
                        CurrentTournament.Open();
                        MyConsole.WriteLine(CurrentTournament.Type + ": Working Now" + "");
                    }
                    if (CustomEvents && Now64.Minute == 45 && AtSecond(Now64, 5, 3))
                    {

                        CurrentTournament = Tournaments[TournamentType.LastManStand];
                        CurrentTournament.Open();
                        MyConsole.WriteLine(CurrentTournament.Type + ": Working Now" + "");
                    }
                    if (CustomEvents && Now64.Minute == 52 && AtSecond(Now64, 6, 3))
                    {

                        CurrentTournament = Tournaments[TournamentType.KillerOfElite];
                        CurrentTournament.Open();
                        MyConsole.WriteLine(CurrentTournament.Type + ": Working Now" + "");
                    }




                    #region ChristmasAnimation
                    // custom (not official): reindeer flying over Twin City every hour; only with CustomEvents
                    if (CustomEvents && Now64.Minute == 56)
                        ChristmasAnimation.Start();
                    #endregion
                    #endregion
                    #region Days
                    // Official times = the client's Activity.ini (Bulletin / "Today's Activities"), read with the
                    // server clock (the client's bulletin also uses the server time). Where the client lists a US and
                    // an EU slot, the EU slot is used (same as Capture the Flag, Saturday 20:00).
                    #region Treasure in the Blue (Squidward Octopus) Mon-Sat 12:30-13:29 and 20:30-21:29
                    SquidwardOctopus.CheckUp();
                    if (Now64.DayOfWeek != DayOfWeek.Sunday && (Now64.Hour == 12 || Now64.Hour == 20) && Now64.Minute == 30)
                        SquidwardOctopus.Start();
                    #endregion
                    #region  PowerArena daily 13:00-14:00 and 20:00-21:00 (5-minute warning before)
                    PowerArena.CheckUp();
                        if ((Now64.Hour == 12 || Now64.Hour == 19) && Now64.Minute == 55)
                            PowerArena.Start();
                      
                    #endregion
                    #region Weekly PK War Sunday 20:00 (EU) / Monthly PK War the 1st of each month 20:00
                        if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.Second < 10)
                        {
                            if (Now64.Day == 1)
                                PkWar.Open(true);
                            else if (Now64.DayOfWeek == DayOfWeek.Sunday)
                                PkWar.Open(false);
                        }
                        PkWar.CheckUp();
                        #endregion
                    MsgKingdomWar.CheckUp(Now64); // Kingdom War: Loot Tokens Mon/Fri 00:00-19:59, Plunder War 21:00-21:30, Gold Brick settlement 22:00
                    #region Quiz Show Mon-Fri 03:00, 09:00, 15:00, 21:00 (Questinfo.ini quest 11; shell.ini [Events] QuizShowTimes/QuizShowDays)
                        QuizShow.CheckUp();
                        if (QuizShow.IsStartTime(Now64) && AtSecond(Now64, 9, 0))
                            QuizShow.Open();
                        #endregion
                  
                    #region CouplesPKWar 16:00
                        CouplesPKWar.CheckUp();
                        if (CustomEvents && Now64.Hour == 16 && Now64.Minute == 00)
                        {
                            CouplesPKWar.Open();
                        }
                        #endregion
                    #region DisCity Monday, Wednesday, Friday 19:00 (sign-up 19:00-19:05, ends 20:30)
                        // Days = Activity.ini id 8 (Mon/Wed/Fri). This server runs the timed Dis City; its official
                        // time is client DailyActivity.ini "19:00-20:30 Dis City" (sign-up 19:00-19:05).
                        DisCity.CheckUp();
                        if (Now64.Hour == 19 && Now64.Minute == 00
                            && (Now64.DayOfWeek == DayOfWeek.Monday || Now64.DayOfWeek == DayOfWeek.Wednesday || Now64.DayOfWeek == DayOfWeek.Friday))
                            DisCity.Open();
                        #endregion
                    #region EliteGuildWar 8:00  - 18
                        if (CustomEvents && ( Now64.Hour == 18) && Now64.Minute < 30)
                        {
                            if (EliteGuildWar.Proces == ProcesType.Dead)
                                EliteGuildWar.Start();

                            if (EliteGuildWar.Proces != ProcesType.Dead)
                            {
                                if (DateTime.Now > EliteGuildWar.StampShuffleScore)
                                {
                                    EliteGuildWar.ShuffleGuildScores();
                                }
                            }

                            if (EliteGuildWar.SendInvitation == false && ( Now64.Hour == 18) && Now64.Minute == 00 && AtSecond(Now64, 7, 30))
                            {
                                SendInvitation("EliteGuildWar Has Start", "ConquerPoints", 266, 143, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                                EliteGuildWar.SendInvitation = true;
                            }
                        }
                        else
                        {
                            if (EliteGuildWar.Proces == ProcesType.Alive)
                                EliteGuildWar.CompleteEndGuildWar();
                        }
                        #endregion
                    #region MrConquer 17
                        if (CustomEvents && (Now64.Hour == 17) && Now64.Minute == 00 && AtSecond(Now64, 8, 4))
                        {
                            
                                SendInvitation("MrConquer/MissConquer Has Start", "ConquerPoints", 301, 242, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                        }
                       
                        #endregion
                    #region Class pole 9:00
                        if (CustomEvents) MsgArcherClass.CheckUP();
                       
                        #endregion
                    #region Nobilty Pole War 10 - 21
                        if (CustomEvents) MsgNobilityPole.CheckUP();
                      
                        #endregion
                    #region Guild Score War 18 
                        if (CustomEvents) GuildWarScore.CheckUP();
                        #endregion
                    #region Horse Racing Monday-Friday: invitations 14:55, race 15:00-15:30
                        SteedRace.work(0);
                        #endregion
                    #region Capture the Flag (started on Saturday 20:00, runs 59 minutes)
                        if (CaptureTheFlag.Proces == ProcesType.Alive)
                        {
                            CaptureTheFlag.UpdateMapScore();
                            CaptureTheFlag.CheckUpX2();
                            CaptureTheFlag.SpawnFlags();
                            CaptureTheFlag.CheckFinish();
                        }
                        #endregion
                    #region Guild War (client: Guild PK) Saturday 20:00-22:59, Flame Lit 21:00-22:59
                        // Activity.ini [t22] "Guild PK" Date=6 Enter1=20:00~22:59 (EU) and [t21] "Flame Lit" 21:00~22:59, which the Flame
                        // Taoist calls "the last two hours of the Guild War" -> Saturday 20:00-23:00. Not restarted the same day after
                        // an early (GM) end; a GM-started war (@gw start) is not ended by the clock.
                        bool gwWindow = Now64.DayOfWeek == DayOfWeek.Saturday && Now64.Hour >= 20 && Now64.Hour < 23;
                        if (gwWindow)
                        {
                            if (GuildWar.Proces == ProcesType.Dead && GuildWar.EndedOn != Now64.Date)
                                GuildWar.Start();// sends the "Guild War is about to begin" invitation (StrRes 10515)
                        }
                        else if ((GuildWar.Proces == ProcesType.Alive || GuildWar.Proces == ProcesType.Idle) && !GuildWar.GmStarted)
                            GuildWar.CompleteEndGuildWar();
                        if (GuildWar.Proces != ProcesType.Dead)
                        {
                            if (DateTime.Now > GuildWar.StampShuffleScore)
                                GuildWar.ShuffleGuildScores();
                            if ((Now64.Hour >= 21 || GuildWar.GmStarted) && GuildWar.FlamesQuest.ActiveFlame10 == false)
                            {
                                SendSysMesage("The 10th Flame Stone is active now. Light up the Flame Stone (62,59) near the Stone Pole in the Guild Area.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                                GuildWar.FlamesQuest.ActiveFlame10 = true;
                            }
                        }
                        #endregion
                    #region Clan War daily 20:30 (enter 20:30-20:35), ends 21:00
                        // Client Cn_Res.ini STR_FAMILY_OCCUPY_TEXT1: "sign up from 21:00 to 20:00 (the next day) everyday.
                        // The Clan War will start at 20:30 and you can enter the battlefield during 20:30 - 20:35."
                        if (Now64.Hour == 20 && Now64.Minute == 30)
                            ClanWar.Open();
                        ClanWar.CheckUp(Now64);
                        #endregion
                 
                    #endregion
                    ///////////////////////////////
                    #region Weekly
                        // the PK tournaments' groups finish on their own: mark the tournament over so next week's Start() runs
                        ElitePkTournament.ResetIfFinished();
                        TeamPkTournament.ResetIfFinished();
                        SkillTeamPkTournament.ResetIfFinished();
                        switch (Now64.DayOfWeek)
                        {
                            case DayOfWeek.Saturday:
                                {
                                    #region TeamPkTournament: sign-up 18:45, 130+ starts 19:00 (120-129 19:02, 100-119 19:04, 1-99 19:06)
                                    if (Now64.Hour == 18 && Now64.Minute == 45)
                                    {
                                        TeamPkTournament.Start();
                                    }
                                    #endregion
                                    #region CaptureTheFlag 20:00-20:59 (EU slot)
                                    if (Now64.Hour == 20 && Now64.Minute == 0 && Now64.Second > 0)
                                    {
                                        CaptureTheFlag.Start();
                                    }
                                    #endregion

                                    break;
                                }
                            case DayOfWeek.Monday:
                                {

                                    #region Class Pk: sign-up 19:15-19:29, war 19:30-20:30
                                    if (Now64.Hour == 19 && Now64.Minute == 15 && AtSecond(Now64, 10, 0))
                                    {
                                        // Start() only works on a stopped war; last week's wars are never stopped otherwise.
                                        ClassPkWar.Stop();
                                        ClassPkWar.Start();
                                    }
                                    else if (Now64.Hour == 20 && Now64.Minute == 30 && AtSecond(Now64, 11, 0))
                                    {
                                        ClassPkWar.Stop();
                                    }
                                    #endregion
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {

                                    #region SuperGuildWar
                                    if (CustomEvents && Now64.Hour == 20)
                                    {
                                        if (SuperGuildWar.Proces == ProcesType.Dead)
                                            SuperGuildWar.Start();

                                        if (SuperGuildWar.Proces != ProcesType.Dead)
                                        {
                                            if (DateTime.Now > SuperGuildWar.StampShuffleScore)
                                            {
                                                SuperGuildWar.ShuffleGuildScores();
                                            }
                                            SuperGuildWar.CheckFlags();
                                        }

                                        else if (SuperGuildWar.SendInvitation == false && Now64.Hour == 20)
                                        {
                                            SendInvitation("SuperGuildWar", "ConquerPoints", 200, 254, 1038, 0, 60, MsgServer.MsgStaticMessage.Messages.SuperGuildWar);
                                            SuperGuildWar.SendInvitation = true;
                                        }

                                    }
                                    else if (Now64.Hour == 21)
                                    {
                                        if (SuperGuildWar.Proces == ProcesType.Alive || SuperGuildWar.Proces == ProcesType.Idle)
                                            SuperGuildWar.CompleteEndGuildWar();
                                    }
                                    #endregion
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    #region SkillTeamTournament: sign-up 19:45, 130+ starts 20:00 (120-129 20:02, 100-119 20:04, 1-99 20:06)
                                    if (Now64.Hour == 19 && Now64.Minute == 45)
                                    {
                                        SkillTeamPkTournament.Start();
                                    }
                                    #endregion
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    #region ElitePkTournament: sign-up 19:55, 130+ starts 20:00 (120-129 20:02, 100-119 20:04, 1-99 20:06)

                                    if (Now64.Hour == 19 && Now64.Minute == 55 && Now64.Second > 5)
                                    {
                                        ElitePkTournament.Start();
                                    }
                                    #endregion
                                    break;
                                }
                        }
                    #endregion

                    //DragonIsland.CheckUp();
                 
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }
                
           
            }
        }
    }
}
