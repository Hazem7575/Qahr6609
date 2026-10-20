using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateGlobalLottery(this ServerSockets.Packet stream, MsgGlobalLottery obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgGlobalLottery);

            return stream;
        }
        public static unsafe void GetGlobalLottery(this ServerSockets.Packet stream, out MsgGlobalLottery pQuery)
        {
            pQuery = new MsgGlobalLottery();
            pQuery = stream.ProtoBufferDeserialize<MsgGlobalLottery>(pQuery);
        }
    }
    /// <summary>
    /// 3282 as the 6609 client reads it (Conquer.exe CMsgGlobalLotteryPB):
    ///   1 Type, 2 condition id (Status) / picker UID (Pick), 3 GUI 0..3, 4 seconds (Status: to bloom / to the end,
    ///   CoolDown: own cooldown) / pool prize id (Pick) / flag (client -> server), 5 picker name,
    ///   6 repeated {1,2}: Status/Pick = [{picks left, total picks}], PrizeList/Ended = one {prize id, count left} per prize.
    /// The client sends only 1 and 4: {1:5} cooldown query, {1:1, 4:flag} pick (flag 1 = pay CPs to skip the cooldown),
    /// {1:3} remaining prize list.
    /// </summary>
    [ProtoContract]
    public class MsgGlobalLottery
    {
        [ProtoMember(1, IsRequired = true)]
        public BlossomAction Type;
        [ProtoMember(2)]
        public uint PrizesType;
        [ProtoMember(3)]
        public uint GUI;
        [ProtoMember(4)]
        public uint Time;
        [ProtoMember(5)]
        public string Name;
        [ProtoMember(6)]
        public List<LotteryCount> Counts = new List<LotteryCount>();

        [PacketAttribute(GamePackets.MsgGlobalLottery)]
        private static void ExcuteMsgGlobalLottery(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null)
                return;
            MsgGlobalLottery Lottery;
            try
            {
                stream.GetGlobalLottery(out Lottery);
            }
            catch
            {
                return;
            }
            if (Lottery == null || !Database.GlobalLotteryTable.Loaded || Database.GlobalLotteryTable.TodayCondition == null)
                return;
            switch (Lottery.Type)
            {
                case BlossomAction.CoolDown:
                    MsgGoldenTree.SendCoolDown(client, stream);
                    break;
                case BlossomAction.PickUp:
                    MsgGoldenTree.Pick(client, stream, Lottery.Time == 1);
                    break;
                case BlossomAction.PrizeList:
                    MsgGoldenTree.SendPrizeList(client, stream);
                    break;
            }
        }
    }
    [ProtoContract]
    public class LotteryCount
    {
        [ProtoMember(1, IsRequired = true)]
        public uint Key;
        [ProtoMember(2, IsRequired = true)]
        public uint Value;
    }
    public enum GuiType : uint
    {
        NotOpened,
        Ready,
        Opened,
        Ranking
    }
    public enum BlossomAction : uint
    {
        Status,
        PickUp,
        EmptyPick,
        PrizeList,
        Ended,
        CoolDown
    }
}
