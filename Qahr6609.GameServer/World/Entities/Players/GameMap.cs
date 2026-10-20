using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Client;
using Conquer6609.BotManager;

namespace Conquer6609.Role
{
    public class MapView
    {
        const int CELLS_PER_BLOCK = 18;

        private Extensions.Counter CounterMovement = new Extensions.Counter(1);

        public ViewPtr[,] m_setBlock;

        private int Width, Height;

        private int GetWidthOfBlock() { return (Width - 1) / CELLS_PER_BLOCK + 1; }
        private int GetHeightOfBlock() { return (Height - 1) / CELLS_PER_BLOCK + 1; }

        public MapView(int _Width, int _Height)
        {
            Width = _Width;
            Height = _Height;

            m_setBlock = new ViewPtr[GetWidthOfBlock(), GetHeightOfBlock()];
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y < GetHeightOfBlock(); y++)
                    m_setBlock[x, y] = new ViewPtr();
        }

        private int Block(int nPos)
        {
            return nPos / CELLS_PER_BLOCK;
        }
        private ViewPtr BlockSet(int nPosX, int nPosY) { return m_setBlock[Block(nPosX), Block(nPosY)]; }

        public bool MoveTo<T>(T obj, int nNewPosX, int nNewPosY)
            where T : IMapObj
        {

            int nOldPosX = obj.X;
            int nOldPosY = obj.Y;
            if ((nOldPosX >= 0 && nOldPosX < Width) == false)
                return false;
            if ((nOldPosY >= 0 && nOldPosY < Height) == false)
                return false;
            if ((nNewPosX >= 0 && nNewPosX < Width) == false)
                return false;
            if ((nNewPosY >= 0 && nNewPosY < Height) == false)
                return false;

            if (Block(nOldPosX) == Block(nNewPosX) && Block(nOldPosY) == Block(nNewPosY))
                return false;

            BlockSet(nOldPosX, nOldPosY).RemoveObject<T>(obj);
            BlockSet(nNewPosX, nNewPosY).AddObject<T>(obj);

            if (obj.ObjType == MapObjectType.Player)
                obj.IndexInScreen = CounterMovement.Next;

            return true;
        }

        public bool EnterMap<T>(T obj)
            where T : IMapObj
        {
            if ((obj.X >= 0 && obj.X < Width) == false)
                return false;
            if ((obj.Y >= 0 && obj.Y < Height) == false)
                return false;

            BlockSet(obj.X, obj.Y).AddObject<T>(obj);

            if (obj.ObjType == MapObjectType.Player)
                obj.IndexInScreen = CounterMovement.Next;

            return true;
        }
        public bool LeaveMap<T>(T obj)
             where T : IMapObj
        {
            if ((obj.X >= 0 && obj.X < Width) == false)
                return false;
            if ((obj.Y >= 0 && obj.Y < Height) == false)
                return false;

                BlockSet(obj.X, obj.Y).RemoveObject<T>(obj);

            return true;
        }
        public IEnumerable<IMapObj> Roles(MapObjectType typ, int X, int Y, Predicate<IMapObj> P = null)
        {
          
                for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                    for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                    {
                        var list = m_setBlock[x, y].GetObjects(typ);
                        if (list != null)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (i >= list.Count)
                                    break;
                                var element = list[i];
                                if (element != null)
                                {
                                    if (P != null)
                                    {
                                        if (P(element))
                                            yield return element;
                                    }
                                    else if (element != null)
                                        yield return element;
                                }
                            }
                        }
                    }
            
           
        }
        /// <summary>
        /// Allocation-free scan of the same 3x3 block area as Roles(): returns the object accepted by P with the
        /// highest IndexInScreen; on ties the first one in scan order wins (same as
        /// Roles(...).OrderByDescending(p => p.IndexInScreen).FirstOrDefault()).
        /// </summary>
        public IMapObj FindNewest(MapObjectType typ, int X, int Y, Predicate<IMapObj> P)
        {
            IMapObj best = null;
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    if (list == null)
                        continue;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i >= list.Count)
                            break;
                        var element = list[i];
                        if (element == null || (P != null && !P(element)))
                            continue;
                        if (best == null || element.IndexInScreen > best.IndexInScreen)
                            best = element;
                    }
                }
            return best;
        }
        /// <summary>
        /// Bounded search around (X, Y): scans block rings 0..maxBlockRadius (ring 0 = the block of (X, Y)) and returns
        /// the first object accepted by P, nearest rings first. Gives up (null) after maxExamined objects.
        /// </summary>
        public IMapObj FindNearby(MapObjectType typ, int X, int Y, int maxBlockRadius, int maxExamined, Predicate<IMapObj> P)
        {
            int bx = Block(X), by = Block(Y);
            int w = GetWidthOfBlock(), h = GetHeightOfBlock();
            int examined = 0;
            for (int r = 0; r <= maxBlockRadius; r++)
            {
                for (int x = bx - r; x <= bx + r; x++)
                {
                    if (x < 0 || x >= w)
                        continue;
                    for (int y = by - r; y <= by + r; y++)
                    {
                        if (y < 0 || y >= h)
                            continue;
                        if (r > 0 && x != bx - r && x != bx + r && y != by - r && y != by + r)
                            continue; // inner blocks were scanned by a smaller ring
                        var list = m_setBlock[x, y].GetObjects(typ);
                        if (list == null)
                            continue;
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (i >= list.Count)
                                break;
                            var element = list[i];
                            if (element == null)
                                continue;
                            if (P == null || P(element))
                                return element;
                            if (++examined >= maxExamined)
                                return null;
                        }
                    }
                }
            }
            return null;
        }
        public int CountRoles(MapObjectType typ, int X, int Y)
        {
            int count = 0;
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    count += list.Count;
                }
            return count;
        }
        public IEnumerable<IMapObj> GetAllMapRoles(MapObjectType typ, Predicate<IMapObj> P = null)
        {
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y  < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y].GetObjects(typ);
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i >= list.Count)
                            break;
                        var element = list[i];
                        if (element != null)
                        {
                            if (P != null)
                            {
                                if (P(element))
                                    yield return element;
                            }
                            else if (element != null)
                                yield return element;
                        }
                    }
                }
        }
        public int GetAllMapRolesCount(MapObjectType typ, Predicate<IMapObj> P = null)
        {
            return GetAllMapRoles(typ, P).Count();
        }
        public T GetMapObject<T>(MapObjectType typ, uint UID, Predicate<IMapObj> P = null)
        {
            foreach (var obj in GetAllMapRoles(typ, P))
                if (obj.UID == UID)
                    return (T)obj;
            return default(T);
        }
        public bool MapContain(MapObjectType typ, uint UID, Predicate<IMapObj> P = null)
        {
            foreach (var obj in GetAllMapRoles(typ, P))
                if (obj.UID == UID)
                    return true;
            return false;
        }
        public void ClearMap(MapObjectType typ)
        {
            for (int x = 0; x < GetWidthOfBlock(); x++)
                for (int y = 0; y < GetHeightOfBlock(); y++)
                {
                    m_setBlock[x, y].Clear(typ);
                }
        }
        public bool TryGetObject<T>(uint UID, MapObjectType typ, int X, int Y, out T obj)
            where T: IMapObj
        {
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y];
                    if (list.TryGetObject<T>(typ, UID, out obj))
                        return true;
                      
                }
            obj = default(T);
            return false;
        }
        public bool Contain(uint UID, int X, int Y)
        {
            for (int x = Math.Max(Block(X) - 1, 0); x <= Block(X) + 1 && x < GetWidthOfBlock(); x++)
                for (int y = Math.Max(Block(Y) - 1, 0); y <= Block(Y) + 1 && y < GetHeightOfBlock(); y++)
                {
                    var list = m_setBlock[x, y];
                    for (int i = 0; i < (int)MapObjectType.Count; i++)
                        if (list.ContainObject((MapObjectType)i, UID))
                            return true;

                }
            return false;
        }
    }
    public class ViewPtr
    {
        private Extensions.MyList<Role.IMapObj>[] Objects;
        public ViewPtr()
        {
            Objects = new  Extensions.MyList<IMapObj>[(int)MapObjectType.Count];
            for (int x = 0; x < (int)MapObjectType.Count; x++)
                Objects[x] = new  Extensions.MyList<IMapObj>();
        }

      
        public void AddObject<T>(T obj)
             where T : IMapObj
        {
         
            Objects[(int)obj.ObjType].Add(obj);
        }

        public void RemoveObject<T>(T obj)
            where T : IMapObj
        {
            Objects[(int)obj.ObjType].Remove(obj);
        }


        public bool ContainObject(MapObjectType obj_t, uint UID)
        {
            for (int x = 0; x < Objects[(int)obj_t].Count; x++)
            {
                var list = Objects[(int)obj_t];
                if (x >= list.Count)
                    break;
                if (list[x].UID == UID)
                    return true;
            }
            return false;
        }

        public bool TryGetObject<T>(MapObjectType obj_t, uint UID, out T obj)
        {
            for (int x = 0; x < Objects[(int)obj_t].Count; x++)
            {
                var list = Objects[(int)obj_t];
                if (x >= list.Count)
                    break;
                if (list[x].UID == UID)
                {
                    obj = (T)list[x];
                    return true;
                }
            }
            obj = default(T);
            return false;
        }
        public Extensions.MyList<IMapObj> GetObjects(MapObjectType typ)
        {
            return Objects[(int)typ];
        }

        public void Clear(MapObjectType typ)
        {
            Objects[(int)typ].Clear();
        }
    }

    public class Portal
    {
        public ushort MapID { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }

        public ushort Destiantion_MapID { get; set; }
        public ushort Destiantion_X { get; set; }
        public ushort Destiantion_Y { get; set; }
    }
    [Flags]
    public enum MapFlagType : byte
    {
        None = 0,
        Valid = 1 << 0,
        Monster = 1 << 1,
        Item = 1 << 2,
        Player = 1 << 3

    }
    [Flags]
    public enum MapTypeFlags
    {
        Normal = 0,
        PkField = 1 << 0,
        ChangeMapDisable = 1 << 1,
        RecordDisable = 1 << 2,
        PkDisable = 1 << 3,
        BoothEnable = 1 << 4,
        TeamDisable = 1 << 5,
        TeleportDisable = 1 << 6,
        GuildMap = 1 << 7,
        PrisonMap = 1 << 8,
        FlyDisable = 1 << 9,
        Family = 1 << 10,
        MineEnable = 1 << 11,
        FreePk = 1 << 12,
        NeverWound = 1 << 13,
        DeadIsland = 1 << 14
    }
    public class GameMap
    {
        public uint RecordSteedRace = 0;

        public static sbyte[] XDir = new sbyte[] 
        { 
            -1, -2, -2, -1, 1, 2, 2, 1,
             0, -2, -2, -2, 0, 2, 2, 2, 
            -1, -2, -2, -1, 1, 2, 2, 1,
             0, -1, -1, -1, 0, 1, 1, 1,
        };
        public static sbyte[] YDir = new sbyte[] 
        {
            2,  1, -1, -2, -2, -1, 1, 2,
            2,  2,  0, -2, -2, -2, 0, 2, 
            2,  1, -1, -2, -2, -1, 1, 2, 
            1,  1,  0, -1, -1, -1, 0, 1
        };

        public static bool IsGate(uint UID)
        {
            return UID == 516076 || UID == 516077 || UID == 516074 || UID == 516075 || UID == 516078 || UID == 516079 || UID == 516080;
        }
        public static bool IsFrozengrotoMaps(uint Map)
        {
            return Map == 1762 || Map == 1927 || Map == 1999 || Map == 2054 || Map == 2055 || Map == 2056;
        }
        public static bool IsMineCave(uint Map)
        {
            return Map == 1028;
        }
        private static List<ushort> UsingMaps = new List<ushort>()
        {
            601,//offlineTG
            700,//arena map, lotery map
            1000,//Desert
            1001,//MysticCastel
            1002,//TwinCity
            1004,//Prommoter
            1005,//Arena
              5051,//SS/FB Arena
            1006,//Steeding TC
            1008,//color you armors/heah
            1010,//bird vilage
            1011,//PhoenixCastle
            1013,//HalkingCave
            1015,//BirdIsland
            1020,//ApeMoutain
            1036,//Market
            //1039,//TrainingGrounds
            1511,//buy mobila
            1038,//GuildWar
            2068,//elitepk map
            6001,//GuildWarJaill
            1098,1099,2080,601,3024,//house id`s
            1351,1352,1353,1354,//lab`s
            1762,//fg1
            1927,//fg2
            1999,//fg3
            2054,//fg4
            2055,//fg5
            2056,//fg6
            1858,//roulette
            3846,//Nemesys Map
            1700,//2nd reborn quest !!!
            3851,//epic ninja quest
            3055,//first map nemesys
            3056,//pestera
            3846,//nemesys map
            1039,//
            6000,//jail
            3825,//trojan epic quest
            2057,
            Game.MsgTournaments.MsgClassPKWar.MapID,
            Game.MsgTournaments.MsgEliteGroup.WaitingAreaID,
            10137,
            10166
        };

        public List<Portal> Portals = new List<Portal>();

        public unsafe void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                var Packet = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                foreach (var client in Users)
                    client.Send(Packet);
            }
        }

        public string Name = "";

        public uint BaseID = 0;
        public MapFlagType[,] cells { get; set; }
        public System.Drawing.Size bounds;
        public Game.MsgMonster.MobCollection MonstersColletion;

        public MapView View;

        public bool AddStaticRole(StaticRole role)
        {
            if (View.EnterMap<StaticRole>(role))
            {
                SetFlagNpc(role.X, role.Y);
                return true;
            }
            return false;
        }
        public bool RemoveStaticRole(Role.IMapObj obj)
        {

            if (View.LeaveMap<Role.IMapObj>(obj))
            {
                RemoveFlagNpc(obj.X, obj.Y);
                return true;
            }
            return false;
        }

        public Game.MsgNpc.Npc Magnolia = null;
        public void AddMagnolia(ServerSockets.Packet stream, uint Quality)
        {
            bool Location = false;
            
            if (Magnolia != null)
            {
                if (Magnolia.X == 99)
                    Location = true;
                RemoveNpc(Magnolia, stream);
            }
             Magnolia = Game.MsgNpc.Npc.Create();
            if (Location)
            {
                Magnolia.UID = 999900;
                Magnolia.X = 106;
                Magnolia.Y = 99;
            }
            else
            {
                Magnolia.UID = 999901;
                Magnolia.X = 99;
                Magnolia.Y = 112;
            }
            Magnolia.ObjType = MapObjectType.Npc;
            Magnolia.NpcType = Flags.NpcType.Talker;
            uint mesh = 0;
            if(Quality% 10 == 7)
                mesh = 10;
            else if(Quality% 10 == 8)
                mesh = 20;
            if(Quality% 10 == 9)
                mesh = 30;
            if(Quality% 10 == 0)
                mesh = 40;
            Magnolia.Mesh = (ushort)(19340 + mesh);
            Magnolia.Map = this.ID;
            AddNpc(Magnolia);
        }


        public void GenerateSectorTraps(ushort x, ushort y, int type)
        {
            if (View.CountRoles(MapObjectType.Item, x, y) < 6)
            {
                ushort newx = (ushort)Program.GetRandom.Next(1, 18);
                ushort newy = (ushort)Program.GetRandom.Next(1, 18);
                newx += x;
                newy += y;
                if (IsFlagPresent(newx, newy, MapFlagType.Item) == false && IsFlagPresent(newx, newy, MapFlagType.Valid))
                {
                    var Item = new Game.MsgFloorItem.MsgItem(null, newx, newy, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, ID, 0, false, this, 60 * 60 * 1000);
                    Item.MsgFloor.m_ID = (uint)type;
                    Item.MsgFloor.m_Color = 2;
                    Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;
                    cells[newx, newy] |= MapFlagType.Item;
                    View.EnterMap<Role.IMapObj>(Item);

                  
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Item.SendAll(stream, MsgDropID.Effect);
                    }
                }
            }
        }
        public void RemoveTrap(ushort x, ushort y, Role.IMapObj item)
        {

            View.LeaveMap<Role.IMapObj>(item);
            cells[item.X, item.Y] &= ~MapFlagType.Item;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var ittem = item as Game.MsgFloorItem.MsgItem;
                ittem.SendAll(stream, MsgDropID.RemoveEffect);
            }

        }
        public ConcurrentDictionary<uint, Game.MsgNpc.Npc> soldierRemains = new ConcurrentDictionary<uint, Game.MsgNpc.Npc>();
        public void CheckUpSoldierReamins(Extensions.Time32 Now)
        {
            List<Game.MsgNpc.Npc> remove = new List<Game.MsgNpc.Npc>();
            foreach (var npc in soldierRemains.Values)
            {
                if (ID == 1000)
                {
                    if (Now > npc.Respawn)
                    {
                        npc.X = (ushort)Program.GetRandom.Next(624 - 32, 624 + 32);
                        npc.Y = (ushort)Program.GetRandom.Next(477 - 32, 477 + 32);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                }
                else if (ID == 1015)
                {
                    if (npc.UID == 8551)
                    {
                        npc.X = (ushort)Program.GetRandom.Next(551 - 32, 551 + 32);
                        npc.Y = (ushort)Program.GetRandom.Next(342 - 32, 342 + 32);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                    else
                    {
                        npc.X = (ushort)Program.GetRandom.Next(454 - 90, 454 + 90);
                        npc.Y = (ushort)Program.GetRandom.Next(574 - 90, 574 + 90);
                        AddNpc(npc);
                        remove.Add(npc);
                    }
                }
            }
            foreach (var npc in remove)
            {
                Game.MsgNpc.Npc rem;
                soldierRemains.TryRemove(npc.UID, out rem);
            }
        }

        public void AddNpc(Game.MsgNpc.Npc npc)
        {
            if (!View.MapContain(MapObjectType.Npc, npc.UID))
            {
                View.EnterMap<Role.IMapObj>(npc);
                SetFlagNpc(npc.X, npc.Y);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    npc.Send(stream);
                }
            }
        }
        public unsafe void RemoveNpc(Game.MsgNpc.Npc npc, ServerSockets.Packet stream)
        {
            if (View.MapContain(MapObjectType.Npc, npc.UID))
            {
                View.LeaveMap<Role.IMapObj>(npc);
                RemoveFlagNpc(npc.X, npc.Y);


                ActionQuery action;

                action = new ActionQuery()
                {
                    ObjId = npc.UID,
                    Type = ActionType.RemoveEntity
                };

                foreach (var client in View.Roles(MapObjectType.Player, npc.X, npc.Y))
                {
                    if (Core.GetDistance(client.X, client.Y, npc.X, npc.Y) <= Game.MsgNpc.Npc.SeedDistance)
                    {
                        client.Send(stream.ActionCreate(&action));
                    }
                }
            }

        }
        // Terrain-only walkability as read from the DMap when the map was loaded. Unlike `cells`
        // it is never modified by NPC / gate / monster / item flags, so movement checks against
        // it do not pull players back next to NPCs.
        public bool[,] TerrainWalkable;

        public bool InMapBounds(int X, int Y)
        {
            return X >= 0 && Y >= 0 && X < bounds.Width && Y < bounds.Height;
        }

        /// <summary>
        /// Out-of-bounds coordinates are always rejected; inside the map only the DMap terrain is
        /// consulted (NPC footprints are ignored). Maps without terrain data only enforce bounds.
        /// </summary>
        public bool ValidTerrain(int X, int Y)
        {
            if (!InMapBounds(X, Y))
                return false;
            var terrain = TerrainWalkable;
            if (terrain == null)
                return true;
            return terrain[X, Y];
        }

        public bool ValidLocation(ushort X, ushort Y)
        {
            if (bounds.Width > X && this.bounds.Height > Y)
            {
           //     Console.WriteLine(cells[X, Y]);
                return (cells[X, Y] & MapFlagType.Valid) == MapFlagType.Valid;
            }
            return false;
        }
        public bool MonsterOnTile(ushort X, ushort Y)
        {
            if (bounds.Width > X && this.bounds.Height > Y)
            {
                return (cells[X, Y] & MapFlagType.Monster) == MapFlagType.Monster;
            }
            return false;
        }
        public void SetMonsterOnTile(ushort X, ushort Y, bool Value)
        {
            try
            {
                if (Value)
                    cells[X, Y] |= MapFlagType.Monster;
                else
                    cells[X, Y] &= ~MapFlagType.Monster;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                MyConsole.WriteLine("Problem monsters on map " + ID.ToString());
            }
        }
        public bool SearchNpcInScreen(uint UID, ushort X, ushort Y, out  Game.MsgNpc.Npc obj)
        {
            if (View.TryGetObject<Game.MsgNpc.Npc>(UID, MapObjectType.Npc, X, Y, out obj))
            {
                return Core.GetDistance(X, Y, obj.X, obj.Y) < Game.MsgNpc.Npc.SeedDistance;
            }
            obj = default(Game.MsgNpc.Npc);
            return false;
        }
       

        public uint ID { get; private set; }
        public GameMap(int width, int height, int m_id)
        {
            Clients = new ConcurrentDictionary<uint, Client.GameClient>();
            this.cells = new MapFlagType[width, height];
            this.bounds = new System.Drawing.Size(width, height);

            this.ID = (uint)m_id;
        }

        public static Extensions.Counter DinamicIDS = new Extensions.Counter(10000001);

        public uint GenerateDynamicID()
        {
            return DinamicIDS.Next;
        }

        //reviver character
        public ushort Reborn_Map = 0;
        public ushort Reborn_X = 0;
        public ushort Reborn_Y = 0;

        public static Dictionary<int, string> MapContents = new Dictionary<int, string>();
        public static bool CheckMap(uint ID)
        {
            if (!Database.Server.ServerMaps.ContainsKey(ID))
            {
                try
                {
                    LoadMap((int)ID, MapContents[(int)ID]);
                    return true;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    MyConsole.WriteLine("MapID = " + ID + " not exist.");
                    return false;
                }
            }
            return true;
        }
        public MapTypeFlags TypeStatus { get; set; }
        /// <summary>gamemap.dat + every DMap file, read off the main thread (see PrefetchMaps).</summary>
        sealed class MapPrefetch
        {
            public Dictionary<int, string> Contents;
            public List<MapJob> Jobs;
            public ConcurrentDictionary<string, MapGrid> Grids;
        }
        static System.Threading.Tasks.Task<MapPrefetch> mapPrefetch;

        /// <summary>[FastBoot, Qahr idea] Starts reading gamemap.dat and all DMap files on pool threads, so the disk work
        /// runs while the loaders before LoadMaps (items, shops...) run. Reads files only; LoadMaps still builds the maps
        /// on the calling thread in the same order. Program.ServerConfig.CO2Folder must be set.</summary>
        public static void PrefetchMaps()
        {
            mapPrefetch = System.Threading.Tasks.Task.Run(() => ReadMapFiles());
        }

        static MapPrefetch ReadMapFiles()
        {
            var r = new MapPrefetch { Contents = new Dictionary<int, string>(MapContents) };
            r.Jobs = ReadJobs(r.Contents);
            r.Grids = ReadGrids(r.Jobs);
            return r;
        }

        public static void LoadMaps()
        {
            var prefetch = mapPrefetch;
            mapPrefetch = null;
            // waits for the prefetch (and rethrows what it threw, as the old in-line read did); none = read now
            MapPrefetch r = prefetch != null ? prefetch.GetAwaiter().GetResult() : ReadMapFiles();
            foreach (var kv in r.Contents)
                MapContents[kv.Key] = kv.Value;
            BuildMaps(r.Jobs, r.Grids);
            MyConsole.WriteLine("Loaded " + Database.Server.ServerMaps.Count + " maps (" + WalkOverrides + " with client walk data from map\\walk)");
            // [FastBoot, from Qahr] was GC.Collect(): a full blocking GC of every heap on each loader; the runtime collects on its own
        }

        /// <summary>gamemap.dat -> contents (map id -> DMap file) and the list of map ids to build (with the copies).</summary>
        static List<MapJob> ReadJobs(Dictionary<int, string> contents)
        {
            var jobs = new List<MapJob>();
            void Queue(int qid, string qfile, uint qbase = 0) { jobs.Add(new MapJob { Id = qid, File = qfile, BaseId = qbase }); }
           
            using (var gamemap = new BinaryReader(new FileStream(Path.Combine(Program.ServerConfig.CO2Folder, "ini/gamemap.dat"), FileMode.Open)))
            {
                var amount = gamemap.ReadInt32();
                for (var i = 0; i < amount; i++)
                {

                    var id = gamemap.ReadInt32();
                    var fileName = Encoding.ASCII.GetString(gamemap.ReadBytes(gamemap.ReadInt32()));
                    var puzzleSize = gamemap.ReadInt32();
                    contents[id] = fileName.Replace(".7z", ".dmap");
                }
                
                foreach (var folded in contents)
                {

                    int id = folded.Key;
                    var mapFile = folded.Value;
                    Queue(id, mapFile);
                    if (folded.Key == 1780)
                    {
                        Queue(3830, mapFile, 1780);
                        Queue(3831, mapFile, 1780);
                        Queue(3832, mapFile, 1780);

                        Queue(3834, mapFile, 1780);
                    }
                    if (folded.Key == 1787)
                    {
                        Queue(9956, mapFile, 1787);
                    }
                    if (folded.Key == 1039)
                    {
                        Queue(10390, mapFile, 1039);
                    }
                    if (folded.Key == 1039)
                    {
                        Queue(10391, mapFile, 1039);
                    }
                    if (folded.Key == 3825)
                    {
                        Queue(3826, mapFile, 3825);
                        Queue(3827, mapFile, 3825);
                        Queue(3828, mapFile, 3825);
                        Queue(3829, mapFile, 3825);
                    }
                    if (folded.Key == 3825)
                        Queue(3833, mapFile, 3825);
                    if (folded.Key == 1765)
                        Queue(1818, mapFile, 1765);
                    if (folded.Key == 1082)
                        Queue(1052, mapFile, 1082);
                    if (folded.Key == 1004)
                    {
                        Queue(6072, mapFile, 1004);
                        Queue(1782, mapFile, 1004);
                        Queue(1783, mapFile, 1004);
                    }
                    if (folded.Key == 601)
                    {
                        Queue(1784, mapFile, 601);
                    }
                    if (folded.Key == 2060)
                    {
                        Queue(20061, mapFile, 2060);
                    }
                    if (folded.Key == 1028)
                    {
                        Queue(1794, mapFile, 1028);
                    }
                    if (folded.Key == 1014)
                        Queue(1792, mapFile, 1014);
                    if (folded.Key == 1765)
                        Queue(1791, mapFile, 1765);
                    if (folded.Key == 10088)
                    {
                        Queue(44455, mapFile, 10088);
                        Queue(44456, mapFile, 10088);
                        Queue(44457, mapFile, 10088);
                    }
                    if (folded.Key == 10090)
                    {
                        Queue(44460, mapFile, 10090);
                        Queue(44461, mapFile, 10090);
                        Queue(44462, mapFile, 10090);
                        Queue(44463, mapFile, 10090);
                    }

                }
                try
                {
                    gamemap.Close();
                }
                catch (Exception)
                {
                    MyConsole.WriteLine("Error Closing");
                }
            }
            return jobs;
        }

        public uint MapColor = 0;


    

        public int[,] FloorType;
        public int[,] Altitude;
        sealed class MapJob
        {
            public int Id;
            public string File;
            public uint BaseId;
        }

        /// <summary>Walk grid of one DMap file (with the map\walk override applied), shared by every
        /// map id that uses the same file (the copies of 1780, 3825, 10088...).</summary>
        internal sealed class MapGrid
        {
            public int Width, Height;
            public MapFlagType[,] Cells;
            public bool[,] Walkable;
            public int[,] FloorType, Altitude;
            public Exception Error;
            public bool WalkOverride;// map\walk data replaced the DMap walkability
            public int Users;// map ids still to be built from this grid; the last one takes the arrays without a copy
        }

        /// <summary>Reads every DMap file once, in parallel (BuildMaps then builds the maps one by one in the
        /// GameMap.dat order, so ServerMaps, duplicate ids and console messages behave as before).</summary>
        static ConcurrentDictionary<string, MapGrid> ReadGrids(List<MapJob> jobs)
        {
            var grids = new ConcurrentDictionary<string, MapGrid>(StringComparer.OrdinalIgnoreCase);
            var files = new List<KeyValuePair<string, MapJob>>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var job in jobs)
                if (seen.Add(job.File))
                    files.Add(new KeyValuePair<string, MapJob>(job.File, job));
            // [FastBoot] ReadGrid only touches its own file and grid, so more readers are safe (was 4)
            var parallel = new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Math.Min(12, Environment.ProcessorCount)) };
            System.Threading.Tasks.Parallel.ForEach(files, parallel, f =>
            {
                bool floor = jobs.Any(j => j.Id == 1038 && string.Equals(j.File, f.Key, StringComparison.OrdinalIgnoreCase));
                grids[f.Key] = ReadGrid(f.Key, f.Value.Id, floor);
            });
            return grids;
        }

        static void BuildMaps(List<MapJob> jobs, ConcurrentDictionary<string, MapGrid> grids)
        {
            foreach (var job in jobs)
                grids[job.File].Users++;
            foreach (var job in jobs)
                LoadMap(job.Id, job.File, job.BaseId, grids[job.File]);
        }

        internal static MapGrid ReadGrid(string mapFile, int id, bool withFloor)
        {
            var grid = new MapGrid();
            try
            {
                byte[] data = File.ReadAllBytes(Path.Combine(Program.ServerConfig.CO2Folder, mapFile));
                if (data.Length < 276)
                    throw new EndOfStreamException("Unable to read beyond the end of the stream.");
                int width = BitConverter.ToInt32(data, 268), height = BitConverter.ToInt32(data, 272);
                long need = 276L + (long)height * ((long)width * 6 + 4);
                if (width < 0 || height < 0 || need > data.Length)
                    throw new EndOfStreamException("Unable to read beyond the end of the stream.");
                grid.Width = width;
                grid.Height = height;
                grid.Cells = new MapFlagType[width, height];
                grid.Walkable = new bool[width, height];
                if (withFloor)
                {
                    grid.FloorType = new int[width, height];
                    grid.Altitude = new int[width, height];
                }
                int pos = 276;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bool ok = BitConverter.ToInt16(data, pos) == 0;
                        grid.Cells[x, y] = ok ? MapFlagType.Valid : MapFlagType.None;
                        grid.Walkable[x, y] = ok;
                        if (withFloor)
                        {
                            grid.FloorType[x, y] = BitConverter.ToInt16(data, pos + 2);
                            grid.Altitude[x, y] = BitConverter.ToInt16(data, pos + 4);
                        }
                        pos += 6;
                    }
                    pos += 4;
                }
                ApplyWalkOverride(grid, mapFile, id);
            }
            catch (Exception e)
            {
                grid.Error = e;
            }
            return grid;
        }

        public static void LoadMap(int id, string mapFile, uint baseid = 0)
        {
            var grid = ReadGrid(mapFile, id, id == 1038);
            grid.Users = 1;
            LoadMap(id, mapFile, baseid, grid);
        }

        static void LoadMap(int id, string mapFile, uint baseid, MapGrid grid)
        {
            bool last = --grid.Users <= 0;
            try
            {
                if (grid.Error != null)
                    throw grid.Error;
                if (grid.WalkOverride)
                    WalkOverrides++;
                GameMap ourInst = new GameMap(grid.Width, grid.Height, id);
                ourInst.MonstersColletion = new Game.MsgMonster.MobCollection((uint)id);
                ourInst.View = new MapView(ourInst.bounds.Width, ourInst.bounds.Height);
                ourInst.MonstersColletion = new Game.MsgMonster.MobCollection((uint)id);
                ourInst.BaseID = baseid;
                ourInst.cells = last ? grid.Cells : (MapFlagType[,])grid.Cells.Clone();
                ourInst.TerrainWalkable = last ? grid.Walkable : (bool[,])grid.Walkable.Clone();
                if (id == 1038 && grid.FloorType != null)
                {
                    ourInst.FloorType = last ? grid.FloorType : (int[,])grid.FloorType.Clone();
                    ourInst.Altitude = last ? grid.Altitude : (int[,])grid.Altitude.Clone();
                }
                else if (id == 1038)
                {
                    ourInst.FloorType = new int[grid.Width, grid.Height];
                    ourInst.Altitude = new int[grid.Width, grid.Height];
                }
                Database.Server.ServerMaps.Add((ushort)id, ourInst);
                int info = baseid != 0 ? (int)baseid : (int)id;

                if (File.Exists(Program.ServerConfig.DbLocation + "maps\\" + info + ".ini"))
                {
                    WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\maps\\" + info + ".ini");
                    ourInst.TypeStatus = (MapTypeFlags)reader.ReadUInt32("info", "type", 0);
                    ourInst.Reborn_X = reader.ReadUInt16("info", "portal0_x", 0);
                    ourInst.Reborn_Y = reader.ReadUInt16("info", "portal0_y", 0);
                    ourInst.Reborn_Map = reader.ReadUInt16("info", "reborn_map", 0);
                    ourInst.RecordSteedRace = reader.ReadUInt32("info", "race_record", 0);// ms, written as uint by UpdateMapRace
                    ourInst.MapColor = reader.ReadUInt32("info", "color", 0);
                }
            }
            catch (FileNotFoundException)
            {
                MyConsole.WriteLine("\tMap not found: " + id + " - " + mapFile + "");
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
            finally
            {
                if (last)
                {
                    grid.Cells = null;
                    grid.Walkable = null;
                    grid.FloorType = grid.Altitude = null;
                }
            }
        }
        public static int WalkOverrides;

        /// <summary>
        /// The client reads the maps listed in GameMap.dat as map\map\*.7z (the newer TQ versions),
        /// but the loose *.DMap files on the server are older copies of several of them (Desert,
        /// Bird Island, Phoenix Castle, Ape City, guild area...). Their walkable cells differ, so
        /// the jump/walk checks pulled players back on ground the client shows as walkable.
        /// map\walk\NAME.walk is the walk grid extracted from the client's NAME.7z (gzip:
        /// int "WALK", int version, int width, int height, int flags, width*height bytes 1=walkable,
        /// and when flags&amp;1 width*height int16 floor types + width*height int16 altitudes).
        /// When the file exists and the size matches, it replaces the DMap walkability.
        /// </summary>
        private static void ApplyWalkOverride(MapGrid map, string mapFile, int id)
        {
            try
            {
                string path = Path.Combine(Program.ServerConfig.CO2Folder, "map", "walk", Path.GetFileNameWithoutExtension(mapFile) + ".walk");
                if (!File.Exists(path))
                    return;
                using (var gz = new System.IO.Compression.GZipStream(File.OpenRead(path), System.IO.Compression.CompressionMode.Decompress))
                using (var rdr = new BinaryReader(new BufferedStream(gz, 1 << 16)))
                {
                    if (rdr.ReadInt32() != 0x4B4C4157)// "WALK"
                        return;
                    rdr.ReadInt32();
                    int width = rdr.ReadInt32(), height = rdr.ReadInt32(), flags = rdr.ReadInt32();
                    if (width != map.Width || height != map.Height)
                    {
                        MyConsole.WriteLine("\tWalk data size mismatch for map " + id + " (" + width + "x" + height + " vs " + map.Width + "x" + map.Height + "), DMap kept");
                        return;
                    }
                    byte[] walk = rdr.ReadBytes(width * height);
                    if (walk.Length != width * height)
                        return;
                    for (int y = 0; y < height; y++)
                        for (int x = 0; x < width; x++)
                        {
                            bool ok = walk[y * width + x] != 0;
                            map.Cells[x, y] = ok ? MapFlagType.Valid : MapFlagType.None;
                            map.Walkable[x, y] = ok;
                        }
                    if ((flags & 1) != 0 && map.FloorType != null && map.Altitude != null)
                    {
                        byte[] extra = rdr.ReadBytes(width * height * 4);
                        if (extra.Length != width * height * 4)
                            throw new EndOfStreamException("Unable to read beyond the end of the stream.");
                        int p = 0;
                        for (int y = 0; y < height; y++)
                            for (int x = 0; x < width; x++, p += 2)
                                map.FloorType[x, y] = BitConverter.ToInt16(extra, p);
                        for (int y = 0; y < height; y++)
                            for (int x = 0; x < width; x++, p += 2)
                                map.Altitude[x, y] = BitConverter.ToInt16(extra, p);
                    }
                    map.WalkOverride = true;
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("\tWalk data error for map " + id + ": " + e.Message + ", DMap kept");
            }
        }

        /// <summary>1 = the player list changed and <see cref="Values"/> must rebuild its snapshot. The flag is
        /// cleared before the snapshot is taken, so a change made while copying is never lost.</summary>
        private int Update = 0;
        private volatile Client.GameClient[] Users = new Client.GameClient[0];
        public Client.GameClient[] Values
        {
            get
            {
                if (System.Threading.Interlocked.Exchange(ref Update, 0) != 0)
                    Users = Clients.Values.ToArray();
                return Users;
            }
            set { }
        }
        private ConcurrentDictionary<uint, Client.GameClient> Clients;
        public void Enquer(Client.GameClient client)
        {
            if (Clients.TryAdd(client.Player.UID, client))
            {
                View.EnterMap<Role.IMapObj>(client.Player);
                client.Map = this;
                System.Threading.Interlocked.Exchange(ref Update, 1);
            }
        }
        public void Denquer(Client.GameClient client)
        {
            Client.GameClient aclient;
            if (Clients.TryRemove(client.Player.UID, out aclient))
            {
                View.LeaveMap<Role.IMapObj>(client.Player);

                System.Threading.Interlocked.Exchange(ref Update, 1);
            }
        }

        public void SetFlagNpc(ushort x, ushort y)
        {
            cells[x, y] = MapFlagType.None;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 1, y + 1);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 1, x + 1);
            ushort xstart = (ushort)Math.Max(x - 1, 0);

            for (ushort ay = (ushort)Math.Max(y - 1, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = MapFlagType.None;
                }
            }
        }
        public void SetGateFlagNpc(ushort x, ushort y)
        {
            cells[x, y] = MapFlagType.None;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 2, y + 2);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 2, x + 2);
            ushort xstart = (ushort)Math.Max(x - 2, 0);

            for (ushort ay = (ushort)Math.Max(y - 2, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = MapFlagType.None;
                }
            }
        }
        public void RemoveFlagNpc(ushort x, ushort y)
        {
            // Only cells whose DMap terrain is walkable become valid again; walls stay walls.
            cells[x, y] = ValidTerrain(x, y) ? MapFlagType.Valid : MapFlagType.None;

            ushort limy = (ushort)Math.Min(this.bounds.Height - 1, y + 1);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 1, x + 1);
            ushort xstart = (ushort)Math.Max(x - 1, 0);

            for (ushort ay = (ushort)Math.Max(y - 1, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    cells[ax, ay] = ValidTerrain(ax, ay) ? MapFlagType.Valid : MapFlagType.None;
                }
            }
        }
        public bool ContainMobID(uint ID, uint Dynamic = 0)
      {
            foreach (var monster in View.GetAllMapRoles(MapObjectType.Monster))
            {
                var mob = monster as Game.MsgMonster.MonsterRole;
                if (mob.Family != null)
                    if (mob.Family.ID == ID)
                    {
                        if (Dynamic == 0)
                            return true;
                        else
                            return Dynamic == monster.DynamicID;
                    }
            }
            return false;
        }
       
        public object SyncRoot = new object();
        public void GetRandCoord(ref ushort x, ref ushort y)
        {
            lock (SyncRoot)
            {
                do
                {
                    x = (ushort)Program.GetRandom.Next(20, (ushort)(bounds.Width - 1));
                    y = (ushort)Program.GetRandom.Next(20, (ushort)(bounds.Height - 1));
                }
                while ((cells[x, y] & MapFlagType.Valid) != MapFlagType.Valid);
            }
        }

        public bool IsFlagPresent(int x, int y, MapFlagType flag)
        {
            if (x > 0 && y > 0 && x < bounds.Width && y < bounds.Height)
                return (cells[x, y] & flag) == flag;
            return false;
        }
        public bool EnqueueItem(Game.MsgFloorItem.MsgItem item)
        {
            if (!View.EnterMap<Role.IMapObj>(item))
                return false;
            Game.MsgFloorItem.FloorItemExpiry.Track(item);
            return true;
        }
        public bool IsValidFlagNpc(ushort x, ushort y)
        {
            ushort limy = (ushort)Math.Min(this.bounds.Height - 1, y + 1);
            ushort limx = (ushort)Math.Min(this.bounds.Width - 1, x + 1);
            ushort xstart = (ushort)Math.Max(x - 1, 0);

            for (ushort ay = (ushort)Math.Max(y - 1, 0); ay <= limy; ay++)
            {
                for (ushort ax = xstart; ax <= limx; ax++)
                {
                    if (!this.IsFlagPresent(x, y, MapFlagType.Valid))
                        return false;
                }
            }
            return true;
        }
        public bool AddGroundItemWithAngle(ref ushort x, ref ushort y, byte Range = 0, Flags.ConquerAngle Angle = Flags.ConquerAngle.East)
        {
            if (this.IsFlagPresent(x, y, MapFlagType.Item) || !this.IsFlagPresent(x, y, MapFlagType.Valid))
            {
                ushort limy = (ushort)Math.Min(this.bounds.Height - (1 + Range), y + (1 + Range));
                ushort limx = (ushort)Math.Min(this.bounds.Width - (1 + Range), x + (1 + Range));
                ushort xstart = (ushort)Math.Max(x - (1 + Range), 0);
                ushort ystart = (ushort)Math.Max(y - (1 + Range), 0);

                for (ushort ay = ystart; ay <= limy; ay++)
                {
                    for (ushort ax = xstart; ax <= limx; ax++)
                    {
                        //if (!this.IsFlagPresent(ax, ay, MapFlagType.Item))
                        {
                            if (this.IsFlagPresent(ax, ay, MapFlagType.Valid))
                            {
                                if (Role.Core.GetAngle(x, y, ax, ay) == Angle)
                                {
                                    x = ax;
                                    y = ay;
                                    cells[ax, ay] |= MapFlagType.Item;
                                    return true;
                                }
                            }
                        }
                    }
                }
                x = 0;
                y = 0;
                return false;
            }

            cells[x, y] |= MapFlagType.Item;
            return true;
        }
        public void RemoveAI(GameClient entity)//bot
        {
            Client.GameClient remov = null;
            if (Clients.TryRemove(entity.Player.UID, out entity))
            {

            }
            if (ConquerAI.Pool.ContainsKey(entity.Player.UID))
            {
                ConquerAI.Pool.TryRemove(entity.Player.UID, out  remov);
                View.LeaveMap<IMapObj>(entity.Player);
                System.Threading.Interlocked.Exchange(ref Update, 1);
            }
        }
        public void AddAI(GameClient entity)//bot
        {
            Role.GameMap map = null;
            if (Clients.TryAdd(entity.Player.UID, entity))
            {
            }

            if (!ConquerAI.Pool.ContainsKey(entity.Player.UID))
            {
                ConquerAI.Pool.TryAdd(entity.Player.UID, entity);
                if (Database.Server.ServerMaps.TryGetValue(entity.Player.Map, out  map))
                {
                    entity.Map = map;
                    View.EnterMap<IMapObj>(entity.Player);
                }

                System.Threading.Interlocked.Exchange(ref Update, 1);
            }
            else
            {
                View.LeaveMap<IMapObj>(entity.Player);
                ConquerAI.Pool.TryAdd(entity.Player.UID, entity);
                View.EnterMap<IMapObj>(entity.Player);
            }
        }
        public bool AddGuildTeleporterItem(ref ushort x, ref ushort y)
        {
            if (IsValidFlagNpc(x, y))
            {
                ushort limy = (ushort)Math.Min(this.bounds.Height - 6, y + 6);
                ushort limx = (ushort)Math.Min(this.bounds.Width - 6, x + 6);
                ushort xstart = (ushort)Math.Max(x - 6, 0);
                ushort ystart = (ushort)Math.Max(y - 6, 0);

                for (ushort ay = ystart; ay <= limy; ay++)
                {
                    for (ushort ax = xstart; ax <= limx; ax++)
                    {
                        if (IsValidFlagNpc(ax, ay))
                        {
                            x = ax;
                            y = ay;
                          
                            cells[ax, ay] |= MapFlagType.Item;

                            return true;
                        }
                    }
                }
                x = 0;
                y = 0;
                return false;
            }
          
            cells[x, y] |= MapFlagType.Item;
            return true;
        }
        public bool AddGroundItem(ref ushort x, ref ushort y, byte Range = 0)
        {
            if (this.IsFlagPresent(x, y, MapFlagType.Item) || !this.IsFlagPresent(x, y, MapFlagType.Valid))
            {
                ushort limy = (ushort)Math.Min(this.bounds.Height - (1 + Range), y + (1 + Range));
                ushort limx = (ushort)Math.Min(this.bounds.Width - (1 + Range), x + (1 + Range));
                ushort xstart = (ushort)Math.Max(x - (1 + Range), 0);
                ushort ystart = (ushort)Math.Max(y - (1 + Range), 0);

                for (ushort ay = ystart; ay <= limy; ay++)
                {
                    for (ushort ax = xstart; ax <= limx; ax++)
                    {
                        if (!this.IsFlagPresent(ax, ay, MapFlagType.Item))
                        {
                            if (this.IsFlagPresent(ax, ay, MapFlagType.Valid))
                            {
                                x = ax;
                                y = ay;
                          
                                cells[ax, ay] |= MapFlagType.Item;

                                return true;
                            }
                        }
                    }
                }
                x = 0;
                y = 0;
                return false;
            }
        
            cells[x, y] |= MapFlagType.Item;
            return true;
        }
    }
}
