using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe struct PrizeInfo
    {
        public string Subject;
        public string Sender;
        public string Message;
        public uint ID;
        public uint Time
        {
            get
            {
                return (uint)((DateTime.Now - SentOn).Seconds);
            }
        }
        public uint cpsprize;
        public uint Attackment;
        public ulong goldprize;
        public ulong JoyBeans;
        public MsgGameItem Item;
        public DateTime SentOn;
    }
}
