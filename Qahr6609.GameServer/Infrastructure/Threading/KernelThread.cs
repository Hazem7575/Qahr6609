using Project2D.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609
{
    public class KernelThread
    {
        public const int TournamentsStamp = 1000,
            ChatItemsStamp = 180000,
            TeamArena_CreateMatches = 900,
            TeamArena_VerifyMatches = 980,
            TeamArena_CheckGroups = 960,
            Arena_CreateMatches = 1100,
            Arena_VerifyMatches = 1200,
            Arena_CheckGroups = 1150,
            TeamPkStamp = 1000,
            ElitePkStamp = 1000,
            BroadCastStamp = 1000,
            ResetDayStamp = 6000,
            SaveDatabaseStamp = 250000;

        public Extensions.Time32 UpdateServerStatus = Extensions.Time32.Now;

        /// <summary>
        /// Fixed-rate timer check: true when clock reached stamp (>=); the stamp then advances by exactly
        /// `interval`, so a job neither drifts nor skips a run when a pass comes a few ms late. When it is
        /// more than one interval behind, it is re-synchronised instead of firing several times in a row.
        /// </summary>
        public static bool Due(Extensions.Time32 clock, ref Extensions.Time32 stamp, int interval)
        {
            if (clock.Value < stamp.Value)
                return false;
            uint next = stamp.Value + (uint)interval;
            if (clock.Value >= next)
                next = clock.Value + (uint)interval;
            stamp.Value = next;
            return true;
        }
        public Extensions.ThreadGroup.ThreadItem Thread;
        public KernelThread(int interval, string name)
        {
            Thread = new Extensions.ThreadGroup.ThreadItem(interval, name, OnProcess);
        }
        public void Start()
        {
            Thread.Open();
        }
        static int _last = 0;
        public int Online
        {
            get
            {
                int current = Database.Server.GamePoll.Count;
                if (current > _last)
                    _last = current;
                return current;
            }
        }
        public static int MaxOnline
        {
            get { return _last; }
        }
        public void OnProcess()
        {
            Extensions.Time32 clock = Extensions.Time32.Now;
            try { Client.PoolProcesses.DailyTasksTick(); }// nightly backup 23:55 + daily restart 23:59:59 (once per day)
            catch (Exception e) { MyConsole.WriteException(e); }
      ;
            try
            {
                
                if (clock > UpdateServerStatus.AddSeconds(5))
                {
                    if (Program.ServerConfig.IsInterServer)
                        MyConsole.Title = "[" + Database.GroupServerList.MyServerInfo.Name + "] Online " + Database.Server.GamePoll.Count + " !";
                    else
                        Console.Title = ConsoleUi.Title(Program.ServerConfig.ServerName, Online, MaxOnline);
                    UpdateServerStatus = Extensions.Time32.Now.AddSeconds(5);
                }
                if (clock > Program.ResetRandom)
                {

                    Program.GetRandom.SetSeed(Environment.TickCount);

                    Program.ResetRandom = Extensions.Time32.Now.AddMinutes(30);
                }
                Game.MsgTournaments.MsgSchedules.CheckUp(clock);
                Game.MsgTournaments.BossHandler.CheckUp(clock);
                Conquer6609.Framework.Modules.ModuleManager.Tick(clock);
                Program.GlobalItems.Work(clock);

          
              
                Game.MsgTournaments.MsgSchedules.TeamArena.CheckGroups(clock);
                Game.MsgTournaments.MsgSchedules.TeamArena.CreateMatches(clock);
                Game.MsgTournaments.MsgSchedules.TeamArena.VerifyMatches(clock);
               

                Game.MsgTournaments.MsgSchedules.Arena.CheckGroups(clock);
                Game.MsgTournaments.MsgSchedules.Arena.CreateMatches(clock);
                Game.MsgTournaments.MsgSchedules.Arena.VerifyMatches(clock);

                foreach (var elitegroup in Game.MsgTournaments.MsgTeamPkTournament.EliteGroups)
                    elitegroup.timerCallback(clock);
                foreach (var elitegroup in Game.MsgTournaments.MsgSkillTeamPkTournament.EliteGroups)
                    elitegroup.timerCallback(clock);

                foreach (var elitegroup in Game.MsgTournaments.MsgEliteTournament.EliteGroups)
                    elitegroup.timerCallback(clock);

                Game.MsgTournaments.MsgBroadcast.Work(clock);
                Database.Server.Reset(clock);
                Program.SaveDBPayers(clock);


            }

            catch (Exception e) { MyConsole.WriteException(e); }

        }
    }
}
