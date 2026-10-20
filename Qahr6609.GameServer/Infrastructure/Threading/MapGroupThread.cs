using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609
{
    public class MapGroupThread
    {
        public const int AI_Buffers = 500,
            AI_Guards = 700,
            AI_Monsters = 400,
            User_Buffers = 500,
            User_Stamina = 500,
            User_StampXPCount = 3000,
            User_AutoAttack = 10,
            User_CheckSecounds = 1000,
            User_Mining = 3000,
            User_FloorSpell = 100,
            User_ItemTIme = 1000,
            User_CheckItems = 1000;

        // Incremented once per OnProcess pass; used by MonsterAiMode >= 1 to handle a monster once per pass.
        public static int AiTick;

        public Extensions.ThreadGroup.ThreadItem Thread;
        public Extensions.ThreadGroup.ThreadItem ThreadBots;
        private readonly int Interval;
        private int overruns;
        public MapGroupThread(int interval, string name)
        {
            Interval = interval;
            ThreadBots = new Extensions.ThreadGroup.ThreadItem(300, name, BotManager.ConquerAIHandle.Action);
            Thread = new Extensions.ThreadGroup.ThreadItem(interval, name, OnProcess);
        }
        public void Start()
        {
            Thread.Open();
            ThreadBots.Open();
        }
        Role.GameMap _desert;
        public Role.GameMap Desert
        {
            get
            {
                if (_desert == null)
                    _desert = Database.Server.ServerMaps[1000];
                return _desert;
            }
        }
        Role.GameMap _bird;
        public Role.GameMap Bird
        {
            get
            {
                if (_bird == null)
                    _bird = Database.Server.ServerMaps[1000];
                return _bird;
            }
        }

        Role.GameMap _towerofmystery;
        public Role.GameMap TowerofMystery
        {
            get
            {
                if (_towerofmystery == null)
                    _towerofmystery = Database.Server.ServerMaps[3998];
                return _towerofmystery;
            }
        }
        public void OnProcess()
        {
            Extensions.Time32 clock = Extensions.Time32.Now;
            var passTimer = System.Diagnostics.Stopwatch.StartNew();
            int players = 0;
            AiTick++;
            try
            {
          
    
                Desert.CheckUpSoldierReamins(clock);
                Bird.CheckUpSoldierReamins(clock);
          
                 TowerofMystery.GenerateSectorTraps(50,336, 1417);
                 TowerofMystery.GenerateSectorTraps(59,334, 1417);
                 TowerofMystery.GenerateSectorTraps(32,351, 1417);
                   TowerofMystery.GenerateSectorTraps(30,346, 1417);
                   TowerofMystery.GenerateSectorTraps(12, 355, 1417);
                   TowerofMystery.GenerateSectorTraps(22, 341, 1417);
            }
            catch (Exception e) { MyConsole.WriteException(e); }

            // Floor items in empty areas (1 s) and abandoned quest instances (30 s).
            try { Game.MsgFloorItem.FloorItemExpiry.Process(); }
            catch (Exception e) { MyConsole.WriteException(e); }
            try { Game.MsgMonster.InstanceSweep.Process(); }
            catch (Exception e) { MyConsole.WriteException(e); }

            // Enumerating the ConcurrentDictionary is lock-free; .Values would lock it and copy every pass.
            foreach (var entry in Database.Server.GamePoll)
            {
                var user = entry.Value;
                players++;
                // one player's failure must not skip the remaining players
                try
                {
                    if (user == null || user.Player == null)
                        continue;
                    if (user.Fake)
                        continue;
                    user.Player.View.CheckUpMonsters(clock);

                    if (clock > user.BuffersStamp)
                    {
                        Client.PoolProcesses.BuffersCallback(user);
                        user.BuffersStamp.Value = clock.Value + User_Buffers;
                    }
                    if (clock > user.StaminStamp)
                    {
                        Client.PoolProcesses.StaminaCallback(user);
                        user.StaminStamp.Value = clock.Value + User_Stamina;
                    }
                    if (clock > user.CheckItemTimeStamp)
                    {
                        Client.PoolProcesses.CheckItemsTime(user);
                        user.CheckItemTimeStamp.Value = clock.Value + User_ItemTIme;
                    }
                    if (clock > user.XPCountStamp)
                    {
                        Client.PoolProcesses.StampXPCountCallback(user);
                        user.XPCountStamp.Value = clock.Value + User_StampXPCount;
                    }
                    if (clock > user.AttackStamp)
                    {
                        Client.PoolProcesses.AutoAttackCallback(user);
                        user.AttackStamp.Value = clock.Value + User_AutoAttack;
                    }
                    if (clock > user.CheckSecoundsStamp)
                    {
                        Client.PoolProcesses.CheckSecounds(user);
                        user.CheckSecoundsStamp.Value = clock.Value + User_CheckSecounds;
                    }
                    if(clock > user.Player.NextMine)
                    {
                        Client.PoolProcesses.MiningProcess(user);
                        user.Player.NextMine.Value = clock.Value + User_Mining;
                    }
                    if (clock > user.FloorSpellStamp)
                    {
                        Client.PoolProcesses.FloorSpell(user);
                        user.FloorSpellStamp.Value = clock.Value + User_FloorSpell;
                    }
                    if (clock > user.CheckItemsView)
                    {
                        Client.PoolProcesses.CheckItems(user);
                        user.CheckItemsView.Value = clock.Value + User_CheckItems;
                    }
                }
                catch (Exception e)
                {
                    if (user != null && user.Player != null && Security.Guard.Throttle("mapgroup:" + user.Player.UID, 10000))
                        MyConsole.WriteLine("MapGroupThread: player " + user.Player.UID + ": " + e);
                }
            }

            // A pass longer than its interval delays every player's periodic work: report it (throttled).
            long took = passTimer.ElapsedMilliseconds;
            if (took > Interval)
            {
                overruns++;
                if (Security.Guard.Throttle("mapgroup-overrun", 60000))
                {
                    MyConsole.WriteLine("MapGroupThread: pass took " + took + " ms (interval " + Interval + " ms, " + players + " players, "
                        + overruns + " slow pass(es) since last report)", ConsoleColor.Yellow);
                    overruns = 0;
                }
            }
        }
    }
}
