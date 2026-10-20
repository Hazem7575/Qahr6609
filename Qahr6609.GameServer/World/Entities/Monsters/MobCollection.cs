using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgMonster
{
    /// <summary>
    /// Monsters spawned into a quest instance (AddMapMonster with the player's DynamicID) were never removed when
    /// the instance was abandoned: they stayed in the map blocks and every player of that map kept iterating over
    /// them. Registry by (map, dynamic id), filled when a monster is spawned with a generated dynamic id
    /// (>= GameMap.DinamicIDS start, so house ids = owner UID are never touched) while a player is standing in
    /// that instance (event/tournament set-ups spawn before their players arrive and clean up themselves).
    /// Every 30 s, instances without any player for more than 60 s have their monsters removed from the map.
    /// [Performance] InstanceSweep = 1 enables it (default 0 = off, original behaviour).
    /// </summary>
    public static class InstanceSweep
    {
        public static readonly bool Enabled = Conquer6609.Framework.Settings.Int("Performance", "InstanceSweep", 0) != 0;
        public const uint MinGeneratedDynamicId = 10000001; // Role.GameMap.DinamicIDS starts here
        const int SweepEveryMs = 30000;
        const int EmptyForMs = 60000;

        sealed class Entry
        {
            public Role.GameMap Map;
            public uint DynamicId;
            public int LastSeen;
            public readonly List<MonsterRole> Mobs = new List<MonsterRole>();
        }
        static readonly System.Collections.Concurrent.ConcurrentDictionary<ulong, Entry> Entries = new System.Collections.Concurrent.ConcurrentDictionary<ulong, Entry>();
        static readonly HashSet<ulong> occupied = new HashSet<ulong>();
        static int lastRun;
        static bool ran;

        public static int Count { get { return Entries.Count; } }

        static ulong Key(uint map, uint dyn) { return ((ulong)map << 32) | dyn; }

        public static bool ShouldTrack(Role.GameMap map, uint dyn)
        {
            if (!Enabled || map == null || dyn < MinGeneratedDynamicId)
                return false;
            foreach (var entry in Database.Server.GamePoll)
            {
                var c = entry.Value;
                if (c != null && c.Player != null && c.Player.Map == map.ID && c.Player.DynamicID == dyn)
                    return true;
            }
            return false;
        }

        public static void Register(MonsterRole mob, Role.GameMap map, uint dyn)
        {
            if (mob == null || map == null)
                return;
            var e = Entries.GetOrAdd(Key(map.ID, dyn), k => new Entry() { Map = map, DynamicId = dyn, LastSeen = Environment.TickCount });
            lock (e)
                e.Mobs.Add(mob);
        }

        /// <summary>Called from MapGroupThread every pass; does its work at most once per 30 s.</summary>
        public static void Process()
        {
            if (!Enabled || Entries.IsEmpty)
                return;
            int now = Environment.TickCount;
            if (ran && unchecked(now - lastRun) < SweepEveryMs)
                return;
            ran = true;
            lastRun = now;

            occupied.Clear();
            foreach (var entry in Database.Server.GamePoll)
            {
                var c = entry.Value;
                if (c != null && c.Player != null && c.Player.DynamicID >= MinGeneratedDynamicId)
                    occupied.Add(Key(c.Player.Map, c.Player.DynamicID));
            }
            foreach (var kv in Entries)
            {
                var e = kv.Value;
                if (occupied.Contains(kv.Key))
                {
                    e.LastSeen = now;
                    continue;
                }
                if (unchecked(now - e.LastSeen) <= EmptyForMs)
                    continue;
                Entry removed;
                if (!Entries.TryRemove(kv.Key, out removed))
                    continue;
                MonsterRole[] mobs;
                lock (removed)
                {
                    mobs = removed.Mobs.ToArray();
                    removed.Mobs.Clear();
                }
                foreach (var mob in mobs)
                {
                    try
                    {
                        var map = mob.GMap ?? removed.Map;
                        if (mob.Alive)
                            map.SetMonsterOnTile(mob.X, mob.Y, false);
                        mob.HitPoints = 0;
                        map.View.LeaveMap<Role.IMapObj>(mob);
                    }
                    catch (Exception ex)
                    {
                        if (Conquer6609.Security.Guard.Throttle("instance-sweep", 60000))
                            MyConsole.WriteException(ex);
                    }
                }
            }
            occupied.Clear();
        }
    }

   public class MobCollection
    {
       public const byte Multiple = 3;

       public string LocationSpawn = "";

       public object SyncRoot = new object();
       public static Extensions.Counter GenerateUid = new Extensions.Counter(400000);

       public Role.GameMap DMap = null;

       private uint DmapID = 0;
       public MobCollection(uint Map)
       {
           DmapID = Map;
           if (Database.Server.ServerMaps != null)
           {
               if (Database.Server.ServerMaps.TryGetValue(Map, out DMap))
                   DMap.MonstersColletion = this;
           }
       }
       public bool ReadMap()
       {
           if (Database.Server.ServerMaps != null)
           {
               if (Database.Server.ServerMaps.TryGetValue(DmapID, out DMap))
                   DMap.MonstersColletion = this;
           }

           return DMap != null;
       }
       public MonsterRole Add(MonsterFamily Famili, bool RemoveOnDead = false, uint dinamicid = 0, bool justone = false)
       {
           if (DMap == null)
               ReadMap();
         
           MonsterRole monsterr = null;
           int count = (int)((Famili.Boss > 0) ? 1 : (int)(Math.Max(1, (int)Famili.SpawnCount) * Multiple));
           if (justone)
               count = Math.Max(1, (int)Famili.SpawnCount);

         
               if (DmapID == 1011 || DmapID == 3071 || DmapID == 1770 || DmapID == 1771 || DmapID == 1772 || DmapID == 1773 || DmapID == 1774
                   || DmapID == 1775 || DmapID == 1777 || DmapID == 1782
                   || DmapID == 1785 || DmapID == 1786 || DmapID == 1787 || DmapID == 1794)
               {
                   if (count > 1 && Famili.SpawnCount > 1)
                       count = (int)(((Famili.MaxSpawnX - Famili.SpawnX) / Famili.SpawnCount) * ((Famili.MaxSpawnY - Famili.SpawnY) / Famili.SpawnCount));
               }
           if(Famili.Name.Contains("Reviver"))count = 1;
            if (DmapID == 3030) count = count * 3;
           // Instance monsters (generated dynamic id, spawned for a player standing in it) are swept once the
           // instance has been empty for a while, see InstanceSweep.
           bool trackInstance = InstanceSweep.ShouldTrack(DMap, dinamicid);
           for (int x = 0; x < count; x++)
           {

               MonsterRole Mob = new MonsterRole(Famili.Copy(), GenerateUid.Next,LocationSpawn,DMap);
               Mob.RemoveOnDead = RemoveOnDead;
               ushort _x =0, _y = 0;
               TryObtainSpawnXY(Famili, out _x, out _y);
               if (!DMap.ValidLocation(_x, _y))
               {
                   ushort r = (ushort)Role.Core.Random.Next(1, 10);
                   _x += r;
                   _y += r;
               }
                
               if(DMap.MonsterOnTile(_x, _y))
               {
                   ushort r = (ushort)Role.Core.Random.Next(1,20);
                   _x += r;
                   _y += r;
               }
               if (DMap.MonsterOnTile(_x, _y))
               {
                   ushort r = (ushort)Role.Core.Random.Next(1, 30);
                   _x += r;
                   _y += r;
               }
               if (!DMap.ValidLocation(_x, _y))
                   continue;
               DMap.SetMonsterOnTile(_x, _y, true);

               Mob.X = _x;
               Mob.Y = _y;
               Mob.RespawnX = _x;
               Mob.RespawnY = _y;

               Mob.Map = DMap.ID;
               Mob.DynamicID = dinamicid;
              
               monsterr = Mob;

               DMap.View.EnterMap<MonsterRole>(Mob);
               if (trackInstance)
                   InstanceSweep.Register(Mob, DMap, dinamicid);
           }
           return monsterr;
       }

    
       public void TryObtainSpawnXY(MonsterFamily Monster, out ushort X, out ushort Y)
       {

           X = (ushort)Program.GetRandom.Next(Monster.SpawnX, Monster.MaxSpawnX);
           Y = (ushort)Program.GetRandom.Next(Monster.SpawnY, Monster.MaxSpawnY);

           for (byte i = 0; i < 10; i++)
           {
               if (DMap == null)
                   break;
              if (DMap.ValidLocation(X, Y) && !DMap.MonsterOnTile(X, Y))
                   break;

               X = (ushort)Program.GetRandom.Next(Monster.SpawnX, Monster.MaxSpawnX);
               Y = (ushort)Program.GetRandom.Next(Monster.SpawnY, Monster.MaxSpawnY);
           }
       }
    }
}
