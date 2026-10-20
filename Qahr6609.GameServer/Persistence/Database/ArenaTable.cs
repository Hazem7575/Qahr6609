using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
   public class ArenaTable
    {

       internal void Save()
       {
           using (Database.DBActions.Write writer = new DBActions.Write("Arena.ini"))
           {
               foreach (var user in Game.MsgTournaments.MsgArena.ArenaPoll.Values)
               {
                   writer.Add(user.ToString());
               }
               writer.Execute(DBActions.Mode.Open);
           }
       }
       internal void Load()
       {
           using (Database.DBActions.Read reader = new DBActions.Read("Arena.ini"))
           {
               if (reader.Reader())
               {
                   for (int i = 0; i < reader.Count; i++)
                   {
                       Game.MsgTournaments.MsgArena.User user = new Game.MsgTournaments.MsgArena.User();
                       user.Load(reader.ReadString(""));
                       Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(user.UID, user);
                   }
               }
           }

           Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop10();
           Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop1000();
           Game.MsgTournaments.MsgArena.UpdateRank();
       }

       internal void ResetArena()
       {
           Game.MsgTournaments.MsgArena.FlushRank();// rewards/honor below read TodayRank

           foreach (var user in Game.MsgTournaments.MsgArena.ArenaPoll.Values)
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

           Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop10();
           Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop1000();
           Game.MsgTournaments.MsgArena.UpdateRank();
       }
    }
}
