using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
namespace Conquer6609.Game.MsgMonster
{
    public class MonsterView
    {
        public const int ViewThreshold = 18;

     

        private MonsterRole role;
        private readonly Predicate<Role.IMapObj> canSeePlayer;
        public MonsterView(MonsterRole _role)
        {
            role = _role;
            canSeePlayer = p => CanSee(p) && p.ObjType == Role.MapObjectType.Player;
        }
     
        public IEnumerable<Role.IMapObj> Roles(Role.GameMap map, Role.MapObjectType typ)
        {
            return map.View.Roles(typ, role.X, role.Y, p => CanSee(p));
        }
        public Role.IMapObj GetTarget(Role.GameMap map, Role.MapObjectType typ)
        {
            // Single allocation-free pass; same pick as OrderByDescending(IndexInScreen).FirstOrDefault().
            return map.View.FindNewest(typ, role.X, role.Y, canSeePlayer);
        }
        public unsafe void SendScreen(ServerSockets.Packet msg, Role.GameMap map)
        {
            foreach (var obj in Roles(map, Role.MapObjectType.Player))
                obj.Send(msg);
        }
       
      
        public bool CanSee(Role.IMapObj obj)
        {
            if (obj.Map != role.Map)
                return false;
            if (obj.DynamicID != role.DynamicID)
                return false;
            if (obj.UID == role.UID)
                return false;
            return GetDistance(obj.X, obj.Y, role.X, role.Y) <= ViewThreshold;
        }

        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            short x = 0;
            short y = 0;
            if (X >= X2) x = (short)(X - X2);
            else if (X2 >= X) x = (short)(X2 - X);
            if (Y >= Y2) y = (short)(Y - Y2);
            else if (Y2 >= Y) y = (short)(Y2 - Y);
            if (x > y) return x;
            else return y;
        }

      

        internal void CheckScren()
        {
            
        }
    }
}
