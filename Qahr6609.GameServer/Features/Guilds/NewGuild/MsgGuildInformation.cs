using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet GuildInformationCreate(this ServerSockets.Packet stream, MsgGuildInformation info)
        {
            stream.InitWriter();


            stream.Write(info.GuildID);//4
            stream.Write((uint)0);//8
            stream.Write(info.SilverFund);//12
            stream.Write(info.ConquerPointFund);//20
            stream.Write(info.MembersCount);//24
            stream.Write(info.MyRank);//28
            stream.Write(info.LeaderName, 16);//32
            stream.Write(1);//1?? //48
            stream.Write(0);//52
            stream.Write(0);//56
            stream.Write(info.Level);//60
            stream.ZeroFill(3);//64
            stream.Write(info.CreateTime);//67
            stream.ZeroFill(21);//71

            stream.Finalize(GamePackets.Guild);
            return stream;

        }
    }
    public class MsgGuildInformation
    {
        public uint GuildID;
        public long SilverFund;
        public uint ConquerPointFund;
        public uint MembersCount;
        public uint MyRank;
        public uint Level;//60
        public uint CreateTime;//67
        public string LeaderName;
       
        public static MsgGuildInformation Create()
        {
            MsgGuildInformation packet = new MsgGuildInformation();
            return packet;
        }
    }
}
