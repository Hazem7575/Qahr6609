using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class TeamArenaTable
    {
        internal void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("TeamArena.ini"))
            {
                foreach (var user in Game.MsgTournaments.MsgTeamArena.ArenaPoll.Values)
                {
                    writer.Add(user.ToString());
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }
        internal void Load()
        {
            using (Database.DBActions.Read reader = new DBActions.Read("TeamArena.ini"))
            {
                if (reader.Reader())
                {
                    for (int i = 0; i < reader.Count; i++)
                    {
                        Game.MsgTournaments.MsgTeamArena.User user = new Game.MsgTournaments.MsgTeamArena.User();
                        user.Load(reader.ReadString(""));
                        Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryAdd(user.UID, user);
                    }
                }
            }

            Game.MsgTournaments.MsgSchedules.TeamArena.CreateRankTop10();
            Game.MsgTournaments.MsgSchedules.TeamArena.CreateRankTop1000();
            Game.MsgTournaments.MsgTeamArena.UpdateRank();
        }

        internal void ResetArena()
        {
            Game.MsgTournaments.MsgTeamArena.FlushRank();// rewards/honor below read TodayRank

            foreach (var user in Game.MsgTournaments.MsgTeamArena.ArenaPoll.Values)
            {
                user.LastSeasonArenaPoints = user.Info.ArenaPoints;
                user.LastSeasonWin = user.Info.TodayWin;
                user.LastSeasonLose = user.Info.TotalLose;
                user.LastSeasonRank = user.Info.TodayRank;


                //if (user.Info.TodayWin >= 10)
                {
                    if (user.Info.TodayRank != 0 && user.Info.ArenaPoints != 4000)
                    {
                        user.Info.CurrentHonor += user.Info.ArenaPoints * 5;
                    }
                    if (user.Info.HistoryHonor < user.Info.CurrentHonor)
                        user.Info.HistoryHonor = user.Info.CurrentHonor;
                }

                user.Info.TodayWin = 0;
                user.Info.TodayBattles = 0;
                user.Info.TotalLose = 0;

                user.Info.TodayRank = 0;
                user.Info.ArenaPoints = 4000;

            }

            Game.MsgTournaments.MsgSchedules.TeamArena.CreateRankTop10();
            Game.MsgTournaments.MsgSchedules.TeamArena.CreateRankTop1000();
            Game.MsgTournaments.MsgTeamArena.UpdateRank();
        }
    }
}
