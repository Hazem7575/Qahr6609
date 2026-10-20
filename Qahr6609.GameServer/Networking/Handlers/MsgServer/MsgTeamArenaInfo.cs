using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet TeamArenaInfoCreate(this ServerSockets.Packet stream, MsgTeamArenaInfo info)
        {
            stream.InitWriter();

            stream.Write(info.TodayRank);
            stream.Write((uint)info.Status);
            stream.Write(info.TotalWin);
            stream.Write(info.TotalLose);
            stream.Write(info.TodayWin);
            stream.Write(info.TodayBattles);
            stream.Write(info.HistoryHonor);
            stream.Write(info.CurrentHonor);
            stream.Write(info.ArenaPoints);
            stream.Write(info.UnKnow2);
            stream.Write(info.UnKnow3);


            stream.Finalize(GamePackets.MsgTeamArenaInfo);
            return stream;
        }
    }
    public unsafe class MsgTeamArenaInfo
    {
        public enum Action : uint
        {
            NotSignedUp = 0,
            WaitingForOpponent = 1,
            WaitingInactive = 2
        }
        public uint TodayRank;//4
        public Action Status;//8
        public uint TotalWin;//12
        public uint TotalLose;//16
        public uint TodayWin;//20
        public uint TodayBattles;//24
        public uint HistoryHonor;//28
        public uint CurrentHonor;//32
        public uint ArenaPoints;//36
        public uint UnKnow2;//40
        public uint UnKnow3;//44

        public void Send(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                client.Send(stream.TeamArenaInfoCreate(this));
            }

        }

        public static MsgTeamArenaInfo Create()
        {
            MsgTeamArenaInfo packet = new MsgTeamArenaInfo();
            return packet;
        }
        [PacketAttribute(GamePackets.MsgTeamArenaInfo)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            user.TeamArenaStatistic.Info.Send(user);
        }
    }
}
