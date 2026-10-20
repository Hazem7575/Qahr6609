using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateBossHarmRankList(this ServerSockets.Packet stream, MsgBossHarmRanking obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(1044);
            return stream;
        }
    }
    [ProtoContract]
    public struct MsgBossHarmRanking
    {
        [Flags]
        public enum RankAction : int
        {
            Show = 0,
            ShowRespondForFirstThree = 1,
            ShowRespondForTheRest = 2,
        }
        [ProtoMember(1, IsRequired = true)]
        public RankAction Type;
        [ProtoMember(2, IsRequired = true)]
        public uint MonsterID;
        [ProtoMember(3, IsRequired = true)]
        public MsgBossHarmRankingEntry[] Hunters;
    }
    [ProtoContract]
    public class MsgBossHarmRankingEntry
    {
        
        [ProtoMember(1, IsRequired = true)]
        public uint Rank;
        [ProtoMember(2, IsRequired = true)]
        public uint ServerID;//Which Sent In 2500 Packet
        [ProtoMember(3, IsRequired = true)]
        public uint HunterUID;
        [ProtoMember(4, IsRequired = true)]
        public string HunterName;
        [ProtoMember(5, IsRequired = true)]
        public uint HunterScore;
    }
}
