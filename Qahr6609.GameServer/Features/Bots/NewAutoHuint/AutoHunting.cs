
using Conquer6609.Game.MsgMonster;
using Conquer6609.Game.MsgServer;
using Conquer6609.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conquer6609
{
    public class AutoHunting
    {

        public bool Enable = false;
        public bool DBalls = false;
        public bool Meteors = false;
        public bool PlusItems = false;
        public bool QualityItems = false;
        public bool ExpBallEventItems = false;
        public bool SocketedItems = false;
        public bool BlessedItems = false;
        public bool MaterialItems = false;
        public bool SoulItems = false;
        public bool LootMoney = false;
        public byte Mytitle = 0;
        public ushort DirectionChange;
        public ushort X;
        public ushort Y;
        public DateTime AttackStamp = DateTime.Now;
        public Role.Flags.ConquerAngle Angle;
        // Environment.TickCount of the last whole-map monster scan (Catching.CatchMob fallback), 0 = never.
        public int LastFullMapScan;
        public AutoHunting()
        {
            Mytitle = 0;
            Enable = false;
            DBalls = false;
            Meteors = false;
            PlusItems = false;
            QualityItems = false;
            ExpBallEventItems = false;
            SocketedItems = false;
            BlessedItems = false;
            MaterialItems = false;
            SoulItems = false;
            LootMoney = false;
            DirectionChange = 0;
            X = 0;
            Y = 0;
            AttackStamp = DateTime.Now;
            Angle = (Role.Flags.ConquerAngle)Program.GetRandom.Next(0, 7);
        }
    }
}
