using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
    public class Activeness
    {
        // Task ids per reborn tier, as defined by the official (client) ini/ActivityTask.ini:
        //   tier 0 (no reborn)  tasks 1..12, tier 1 (1st reborn) 13..24, tier 2 (2nd reborn) 25..38.
        // A character only owns the tier of its own reborn count (OpenLev/CloseLev), so every event
        // increases all three ids and IncreaseTask keeps the one that is open for the player.
        public static readonly uint[] LoginTasks = { 1, 13, 25 };            // Login the Game
        public static readonly uint[] VipTasks = { 2, 14, 26 };              // VIP Login
        public static readonly uint[] OnlineTasks = { 3, 15, 27 };           // 0.5-Hour Online (1 progress = 30 minutes)
        public static readonly uint[] LotteryTasks = { 4, 16, 28 };          // Lottery
        public static readonly uint[] DailyQuestTasks = { 5, 17, 29 };       // Daily Quest (Daily Quest Envoy quests)
        public static readonly uint[] FlowerTasks = { 6, 18, 30 };           // Flowers/Gifts
        public static readonly uint[] QualifierTasks = { 7, 19, 31 };        // Qualifier (1v1 Arena)
        public static readonly uint[] ChampionPointTasks = { 8, 20, 32 };    // Champion Points (receive once)
        public static readonly uint[] HorseRaceTasks = { 9, 21, 33 };        // Horse Racing
        public static readonly uint[] TreasureInTheBlueTasks = { 10, 22, 34 }; // Treasure in the Blue
        public static readonly uint[] EnlightenTasks = { 11, 23, 35 };       // Enlightenment
        public static readonly uint[] TeamQualifierTasks = { 12, 24, 36 };   // Team Qualifier
        public const uint ChiStudyTask = 37;                                 // Chi Study (2nd reborn 120+)
        public const uint JiangHuTask = 38;                                  // JiangHu (2nd reborn 30+)

        public class Task
        {
            public uint ID;
            public byte Progres;
            public byte Completed;
        }
        public Client.GameClient user;
        public byte ClaimRewards = 0;
        public Extensions.SafeDictionary<uint, Task> Tasks;
        public uint ActivityPoints = 0;


        public unsafe void UpdateActivityPoints(ServerSockets.Packet stream)
        {
            // points + number of chests claimed today (the client derives the next claimable chest from it)
            user.Send(stream.ActivityRewardsStateCreate(ActivityPoints, ClaimRewards));
        }

        public Activeness(Client.GameClient client)
        {
            user = client;
            Tasks = new Extensions.SafeDictionary<uint, Task>();
        }
        public unsafe void ResetDaily()
        {
            Tasks.Clear();
            ActivityPoints = 0;
            ClaimRewards = 0;
            using (var rect = new ServerSockets.RecycledPacket())
            {
                var stream = rect.GetStream();
                CheckTasks(stream);
                UpdateActivityPoints(stream);
                UpdateClaimRewards(stream);
            }
        }
        /// <summary>
        /// Daily reset for a player who is online at midnight: new day, and he is logged in (VIP) for it.
        /// </summary>
        public void NewDayOnline()
        {
            ResetDaily();
            IncreaseTasks(LoginTasks);
            if (user.Player.VipLevel > 0)
                IncreaseTasks(VipTasks);
        }
        public void IncreaseTasks(uint[] ids)
        {
            for (int x = 0; x < ids.Length; x++)
                IncreaseTask(ids[x]);
        }
        public void OnQualifierMatch()
        {
            IncreaseTasks(QualifierTasks);
        }
        public void OnTeamQualifierMatch()
        {
            IncreaseTasks(TeamQualifierTasks);
        }
        public void OnChampionPointsReceived()
        {
            IncreaseTasks(ChampionPointTasks);
        }
        // The 8 daily quests listed by the Daily Quest Envoy (Worship Leaders, Everything Has A Price,
        // Release the Souls, Rare Materials, Magnolias All Around, Tower of Mystery, Spirit Beads, Heaven Treasury).
        public static readonly uint[] DailyQuestMissions = { 6329, 6245, 6049, 6366, 6014, 6126, 2375, 6390 };
        /// <summary>
        /// Called when a quest is finished; a finished Daily Quest Envoy quest counts for the "Daily Quest" task.
        /// </summary>
        public void OnQuestFinished(uint MissionId)
        {
            if (Array.IndexOf(DailyQuestMissions, MissionId) >= 0)
                IncreaseTasks(DailyQuestTasks);
        }
        public void IncreaseTask(uint UID)
        {
            Database.ActivityTask.Task DBTask;
            if (Database.Server.ActivityTasks.TryGetValue(UID, out DBTask))
            {
                // only the task tier of the player's own level/reborn (OpenLev <= value < CloseLev)
                if (!DBTask.CheckOpen(user.Player.Level, user.Player.Reborn))
                    return;
                if (!Tasks.ContainsKey(UID))
                    Tasks.Add(UID, new Task() { ID = UID });
                UpdateTask(UID, DBTask);
            }
        }
        private void UpdateTask(uint ID, Database.ActivityTask.Task DBTask)
        {
            var mytask = Tasks[ID];
            if (mytask.Completed == 0)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    // every progress step gives the task's Activity value (official: the client's chest table
                    // 100/210/340 is only reachable this way, e.g. the 2nd-reborn tier adds up to exactly 340)
                    if (mytask.Progres < DBTask.NeedProgress)
                    {
                        mytask.Progres++;
                        ActivityPoints += DBTask.ActiveValue;
                        UpdateActivityPoints(stream);
                    }

                    if (mytask.Progres >= DBTask.NeedProgress)
                        mytask.Completed = 1;
                    UpdateSingleTask(mytask, stream);
                }
            }
        }
        public unsafe void UpdateSingleTask(Task task,ServerSockets.Packet stream)
        {
            user.Send(stream.ActivityTasksCreate(MsgActivityTasks.Action.InitializeList, new Task[1] { task }));
        }
        public void CheckTasks(ServerSockets.Packet stream)
        {
            // drop tasks that are unknown or not of the player's tier any more (e.g. reborn, or old saves that
            // still hold all three tiers); the points already earned today are kept.
            foreach (var task in Tasks.GetValues())
            {
                Database.ActivityTask.Task DBTask;
                if (!Database.Server.ActivityTasks.TryGetValue(task.ID, out DBTask)
                    || !DBTask.CheckOpen(user.Player.Level, user.Player.Reborn))
                    Tasks.Remove(task.ID);
            }
            foreach (var task in Database.Server.ActivityTasks.GetValues())
            {
                if (task.CheckOpen(user.Player.Level, user.Player.Reborn))
                {
                    if (!Tasks.ContainsKey(task.ID))
                        Tasks.Add(task.ID, new Task() { ID= task.ID });
                }
            }
            UpdateTasksList(stream);
        }
        public unsafe void UpdateTasksList(ServerSockets.Packet stream)
        {

            user.Send(stream.ActivityTasksCreate(MsgActivityTasks.Action.InitializeList, Tasks.GetValues()));
        }
        public unsafe void UpdateClaimRewards(ServerSockets.Packet stream)
        {
            user.Send(stream.ActivityClaimsCreate(ClaimRewards));
        }

        public override string ToString()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(ClaimRewards);
            writer.Add(ActivityPoints);
            writer.Add(Tasks.Count);

            var array = Tasks.GetValues();
            for (int x = 0; x < array.Length; x++)
            {
                var task = array[x];
                writer.Add(task.ID).Add(task.Progres).Add(task.Completed);
            }
            return writer.Close();
        }
        public void Load(string Line)
        {
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(Line, '/');
            ClaimRewards = reader.Read((byte)0);
            // saved as plain decimal text; it was read back as a byte, so 256+ points were lost on relog
            ActivityPoints = reader.Read((uint)0);
            int counttasks = reader.Read((int)0);
            for (int x = 0; x < counttasks; x++)
            {
                Task task = new Task();
                task.ID = reader.Read((uint)0);
                task.Progres = reader.Read((byte)0);
                task.Completed = reader.Read((byte)0);

                if (!Tasks.ContainsKey(task.ID))
                    Tasks.Add(task.ID, task);
            }
        }
    }
}
