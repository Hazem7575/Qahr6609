using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateGlobalLotteryRank(this ServerSockets.Packet stream, MsgGlobalLotteryRankList obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgGlobalLotteryRankList);

            return stream;
        }
        public static unsafe void GetGlobalLotteryRank(this ServerSockets.Packet stream, out MsgGlobalLotteryRankList pQuery)
        {
            pQuery = new MsgGlobalLotteryRankList();
            pQuery = stream.ProtoBufferDeserialize<MsgGlobalLotteryRankList>(pQuery);
        }
    }
    [ProtoContract]
    public class MsgGlobalLotteryRankList
    {
        [ProtoMember(1, IsRequired = true)]
        public uint Page;
        [ProtoMember(2, IsRequired = true)]
        public uint TotalPages;
        [ProtoMember(3, IsRequired = true)]
        public uint RegistedCount;
        [ProtoMember(4, IsRequired = true)]
        public Picker[] Pickers;
        const int PageSize = 10;
        [PacketAttribute(GamePackets.MsgGlobalLotteryRankList)]
        private static void ExcuteMsgGlobalLotteryRankingList(Client.GameClient client, Conquer6609.ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null || !Database.GlobalLotteryTable.Loaded)
                return;
            var all = MsgGoldenTree.GoldenTreeRank;
            MsgGlobalLotteryRankList Rank;
            try
            {
                stream.GetGlobalLotteryRank(out Rank);
            }
            catch
            {
                return;
            }
            if (Rank == null)
                return;
            uint TotalPages = (uint)Math.Max(1, (all.Count + PageSize - 1) / PageSize);
            uint Page = Rank.Page;
            if (Page >= TotalPages)
                Page = TotalPages - 1;
            // Best prizes (grade 1) first.
            var selected = all.OrderBy(p =>
            {
                var prize = Database.GlobalLotteryTable.FindPrize(p.Item2);
                return prize != null ? prize.PerfectionLevel : byte.MaxValue;
            }).ThenBy(p => p.Item1).Skip((int)(PageSize * Page)).Take(PageSize).ToArray();
            Rank.Page = Page;
            Rank.TotalPages = TotalPages;
            Rank.RegistedCount = (uint)all.Count;
            Rank.Pickers = new Picker[selected.Length];
            for (int index = 0; index < selected.Length; index++)
                Rank.Pickers[index] = new Picker() { Name = selected[index].Item1, PickItem = selected[index].Item2 };
            client.Send(stream.CreateGlobalLotteryRank(Rank));
        }
    }
    [ProtoContract]
    public class Picker
    {
        [ProtoMember(1, IsRequired = true)]
        public uint PickItem;
        [ProtoMember(2, IsRequired = true)]
        public string Name;
    }
}
