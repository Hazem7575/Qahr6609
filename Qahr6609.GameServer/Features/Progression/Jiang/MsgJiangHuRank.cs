using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static void GetJiangHuRank(this ServerSockets.Packet msg, out byte Page)
        {
            Page = msg.ReadUInt8();
        }
        public static unsafe ServerSockets.Packet JiangHuRankCreate(this ServerSockets.Packet stream, byte Page, byte MyRank, byte PageRegistred
            , byte Registred)
        {
            stream.InitWriter();

            stream.Write(Page);
            stream.Write(MyRank);
            stream.Write(PageRegistred);
            stream.Write(Registred);


            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemJiangHuRank(this ServerSockets.Packet stream, Role.Instance.JiangHu jiang)
        {

            stream.Write(jiang.Rank);
            stream.Write(jiang.Inner_Strength);
            stream.Write((uint)jiang.Level);
            stream.Write(jiang.Name, 16);
            stream.Write(jiang.CustomizedName, 16);

            return stream;
        }
        public static unsafe ServerSockets.Packet JiangHuRankFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.JiangHuRank);
            return stream;
        }

    }
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MsgJiangHuRank
    {
        

        [PacketAttribute(GamePackets.JiangHuRank)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            // a player without JiangHu (not unlocked yet) can still open the ranking window
            if (user.Player.MyJiangHu != null)
                Role.Instance.JiangHu.JiangHuRanking.UpdateRank(user.Player.MyJiangHu);
            byte Page;

            stream.GetJiangHuRank(out Page);

            Page -= 1;

            var top = Role.Instance.JiangHu.JiangHuRanking.TopRank100;
            var info = top != null ? top.ToArray() : new Role.Instance.JiangHu[0];
            if (info != null)
            {
                const int max = 10;
                int offset = Page * max;
                int count = Math.Min(max, Math.Max(0, info.Length - offset));


                byte myrank =0;
                if (user.Player.MyJiangHu != null)
                {
                    if (user.Player.MyJiangHu.Rank < 100)
                       myrank = user.Player.MyJiangHu.Rank;
                }

                stream.JiangHuRankCreate((byte)(Page + 1), myrank, (byte)count, (byte)Math.Min(100, info.Length));

                for (int x = 0; x < count; x++)
                {
                    if (info.Length > offset + x)
                    {
                        var element = info[offset + x];
                        if (element.Rank < 100)
                        {
                            stream.AddItemJiangHuRank(element);
                        }
                    }
                }
                user.Send(stream.JiangHuRankFinalize());
            }
        }
    }
}
