using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role
{
    public unsafe class Statue
    {
        public unsafe static Extensions.Counter CounterUID = new Extensions.Counter(105175);
        public static Statue StaticStatue = null;
        public static SobNpc StaticSobNpc = null;
        public static bool ContainStatue(Role.GameMap map, ushort x, ushort y)
        {
            foreach (var npc in map.View.Roles(MapObjectType.SobNpc, x,y))
                if (npc.X == x && npc.Y == y)
                    return true;
            return false;
        }
        public int Action = 0;
        public ushort Action2;
        public uint UID;
        public int HitPotion = 0;
        public Client.GameClient user;

        public byte[] StatuePacket;
        public bool Static = false;
        public unsafe static void winnerstate(Client.GameClient client, ushort x, ushort y, int action2, bool Static = false)
        {
            try
            {

                Statue stat = new Statue();
                stat.user = client;

                stat.UID = CounterUID.Next;
                stat.HitPotion = client.Player.HitPoints / 100;
                stat.Static = Static;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    SobNpc npc = new SobNpc(stat);
                    npc.ObjType = MapObjectType.SobNpc;
                    npc.UID = stat.UID;
                    npc.X = x;
                    npc.Y = y;
                    npc.Map = client.Player.Map;
                    npc.MaxHitPoints = (int)(client.Status.MaxHitpoints * 10);
                    npc.HitPoints = client.Player.HitPoints * 10;

                    client.Player.View.SendView(npc.GetArray(stream, false), true);

                    client.Map.View.EnterMap<Role.IMapObj>(npc);
                    if (Static)
                        StaticSobNpc = npc;
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public unsafe static void CreateStatue(Client.GameClient client, ushort x, ushort y, int Action, int action2, bool Static = false)
        {
            try
            {
              
                Statue stat = new Statue();
                stat.user = client;

                stat.UID = CounterUID.Next;
                stat.HitPotion = client.Player.HitPoints / 100;
                stat.Action = Action;
                stat.Static = Static;
                if (stat.Static)
                {
                    stat.user.Player.Action = Flags.ConquerAction.Sit;
                    stat.user.Player.Angle = Flags.ConquerAngle.South;
                    StaticStatue = stat;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    SobNpc npc = new SobNpc(stat);
                    npc.ObjType = MapObjectType.SobNpc;
                    npc.UID = stat.UID;
                    npc.X = x;
                    npc.Y = y;
                    npc.Map = client.Player.Map;
                    npc.MaxHitPoints = (int)(client.Status.MaxHitpoints * 10);
                    npc.HitPoints = client.Player.HitPoints * 10;

                    client.Player.View.SendView(npc.GetArray(stream,false), true);

                    client.Map.View.EnterMap<Role.IMapObj>(npc);
                    if (Static)
                        StaticSobNpc = npc;
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public unsafe static void RemoveStatue(ServerSockets.Packet stream, Client.GameClient killer, uint UID, Role.IMapObj obj)
        {
            Role.GameMap map;
            if (killer.Map != null)
                map = killer.Map;
            else
              map =  Database.Server.ServerMaps[1002];

            map.View.LeaveMap<Role.IMapObj>(obj);

            ActionQuery action = new ActionQuery()
            {
                ObjId = UID,
                Type = ActionType.RemoveEntity
            };
            killer.Player.View.SendView(stream.ActionCreate(&action), true);
        }
    
        public static void ElitePkStatue(Client.GameClient user)
        {
            if (StaticStatue == null && StaticSobNpc == null)
            {
                CreateStatue(user, 301, 141, 0, 0, true);
            }
            else
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    RemoveStatue(stream, user, StaticSobNpc.UID, StaticSobNpc);
                    CreateStatue(user, 301, 290, 0, 0, true);
                }
            }
        }
        public static void Save()
        {
            using (Database.DBActions.Write _wr = new Database.DBActions.Write("StaticStatue.txt"))
            {
                if (StaticStatue != null && StaticStatue.StatuePacket != null && StaticSobNpc != null)
                {
                    int Size = StaticStatue.StatuePacket.Length;
                    Database.DBActions.WriteLine line = new Database.DBActions.WriteLine('/');
                 
                   
                    line.Add(Size);
                    for (int x = 0; x < Size; x++)
                        line.Add(StaticStatue.StatuePacket[x]);
                    line.Add(StaticSobNpc.UID).Add(StaticSobNpc.X).Add(StaticSobNpc.Y).Add(StaticSobNpc.Map).Add(StaticSobNpc.MaxHitPoints).Add(StaticSobNpc.HitPoints);
                    _wr.Add(line.Close());
                    _wr.Execute(Database.DBActions.Mode.Open);
                }
            }
        }
        public static void Load()
        {
            using (Database.DBActions.Read r = new Database.DBActions.Read("StaticStatue.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        
                        Database.DBActions.ReadLine readerline = new Database.DBActions.ReadLine(r.ReadString(""), '/');
                        int Size = readerline.Read((int)0);
                        if (Size != 0)
                        {
                            StaticStatue = new Statue();
                            StaticStatue.Static = true;

                            StaticStatue.StatuePacket = new byte[Size];
                            for (int i = 0; i < StaticStatue.StatuePacket.Length; i++)
                                StaticStatue.StatuePacket[i] = readerline.Read((byte)0);

                            StaticSobNpc = new SobNpc(StaticStatue);
                            StaticSobNpc.ObjType = MapObjectType.SobNpc;
                            StaticSobNpc.UID = readerline.Read((uint)0);
                            StaticSobNpc.X = readerline.Read((ushort)0);
                            StaticSobNpc.Y = readerline.Read((ushort)0);
                            StaticSobNpc.Map = readerline.Read((ushort)0);
                            StaticSobNpc.MaxHitPoints = readerline.Read((int)0);
                            StaticSobNpc.HitPoints = readerline.Read((int)0);

                            Database.Server.ServerMaps[StaticSobNpc.Map].View.EnterMap<Role.IMapObj>(StaticSobNpc);
                        }
                    }
                }
            }
        }
    
    }
}
