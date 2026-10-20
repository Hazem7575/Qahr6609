using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.Reward
{
    public enum Mode
    {
        None,
        Top_Boss_Slayer,
        Top_Mob_Slayer,
        Top_pvp,
        Top_Wars,
        Top_Killers
    }
    public struct Reward
    {
        public Mode ID;
        public uint UID;
        public uint number;
        public uint NeedPoints;
        public uint prize;
        public uint prizevalue;
    }
}
