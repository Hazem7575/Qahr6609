using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetAdvertise(this ServerSockets.Packet stream,out int AtCount)
        {
            AtCount = stream.ReadInt32();
        }
        public static unsafe ServerSockets.Packet AdvertiseCreate(this ServerSockets.Packet stream, int AtCount, int count, int AllRegistred, int PacketNo)
        {
            stream.InitWriter();

            stream.Write(AtCount);
            stream.Write(count);
            stream.Write(AllRegistred);
            stream.Write(PacketNo);
            stream.Write(0);//unknow;

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemAdvertise(this ServerSockets.Packet stream, Role.Instance.Guild Guild)
        {
            stream.Write(Guild.Info.GuildID);

            if (Guild.AdvertiseRecruit.Buletin != null)
            {
                stream.Write(Guild.AdvertiseRecruit.Buletin, 255);
            }
            else
            {
                stream.ZeroFill(255);
            }
            if (Guild.GuildName != null)
            {
                stream.Write(Guild.GuildName, 36);
            }
            else
            {
                stream.ZeroFill(36);
            }
            if (Guild.Info.LeaderName != null)
            {
                stream.Write(Guild.Info.LeaderName, 17);
            }
            else
            {
                stream.ZeroFill(17);
            }
            stream.Write((uint)Guild.Info.Level);
            stream.Write((uint)Guild.Info.MembersCount);
            stream.Write((long)Guild.Info.SilverFund);
            stream.Write((ushort)(Guild.AdvertiseRecruit.AutoJoin ? 1 : 0));
            stream.Write((ushort)Guild.AdvertiseRecruit.NotAllowFlag);
            stream.ZeroFill(12);

            return stream;
        }
        public static unsafe ServerSockets.Packet AdvertiseFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.Advertise);

            return stream;
        }
    }
    public unsafe class MsgAdvertise
    {

        [PacketAttribute(GamePackets.Advertise)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            
            uint all_advertise = (ushort)Role.Instance.Guild.Advertise.AdvertiseRanks.Length;

            int Receive_count;
            stream.GetAdvertise(out Receive_count);

            if (Receive_count != 0 && Receive_count % 4 != 0)
                return;
            List<Role.Instance.Guild> AdvGuilds = new List<Role.Instance.Guild>();
            for (ushort x = 0; x < 4; x++)
            {
                ushort getposition = (ushort)(Receive_count + x);
                if (Role.Instance.Guild.Advertise.AdvertiseRanks.Length <= getposition)
                    break;
                AdvGuilds.Add(Role.Instance.Guild.Advertise.AdvertiseRanks[getposition]);
            }


            if (AdvGuilds.Count <= 2)
            {
                stream.AdvertiseCreate(Receive_count, AdvGuilds.Count, Role.Instance.Guild.Advertise.AdvertiseRanks.Length, 1);

                for (ushort x = 0; x < AdvGuilds.Count; x++)
                {
                    var element = AdvGuilds[x];
                    stream.AddItemAdvertise(element);
                }
                user.Send(stream.AdvertiseFinalize());

            }
            else if (AdvGuilds.Count == 3)
            {
                stream.AdvertiseCreate(Receive_count, 2, Role.Instance.Guild.Advertise.AdvertiseRanks.Length, 1);
                // AdvGuilds.Count
                for (ushort x = 0; x < 2; x++)
                {
                    var element = AdvGuilds[x];
                    stream.AddItemAdvertise(element);
                }
                user.Send(stream.AdvertiseFinalize());

                stream.AdvertiseCreate(Receive_count, 1, Role.Instance.Guild.Advertise.AdvertiseRanks.Length, 0);
                stream.AddItemAdvertise(AdvGuilds.Last());
                user.Send(stream.AdvertiseFinalize());
            }
            else if (AdvGuilds.Count == 4)
            {
                stream.AdvertiseCreate(Receive_count, 2, Role.Instance.Guild.Advertise.AdvertiseRanks.Length, 1);
                for (ushort x = 0; x < 2; x++)
                {
                    var element = AdvGuilds[x];
                    stream.AddItemAdvertise(element);
                }
                user.Send(stream.AdvertiseFinalize());

                stream.AdvertiseCreate(Receive_count, 2, Role.Instance.Guild.Advertise.AdvertiseRanks.Length, 0);
                for (ushort x = 2; x < 4; x++)
                {
                    var element = AdvGuilds[x];
                    stream.AddItemAdvertise(element);
                }
                user.Send(stream.AdvertiseFinalize());
            }
        }
    }
}
