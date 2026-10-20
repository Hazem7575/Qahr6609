using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet GuildRanksCreate(this ServerSockets.Packet stream, MsgGuildRanks.RankTyp rank, ushort count, ushort Page)
        {
            stream.InitWriter();


            stream.Write((ushort)rank);
            stream.Write(count);
            stream.Write((ushort)20);//register count;
            stream.Write(Page);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemGuildRanks(this ServerSockets.Packet stream, MsgGuildRanks.RankTyp rank, Role.Instance.Guild.Member member, long Donation)
        {
            stream.Write(member.UID);
            stream.Write((uint)member.Rank);
            stream.Write((uint)0);
            stream.ZeroFill((ushort)(4 * (ushort)rank));
            stream.Write((uint)Donation);
            stream.ZeroFill(36 - (ushort)(4 * (ushort)rank));
            stream.Write(member.Name, 16);

            return stream;
        }
        public static unsafe ServerSockets.Packet GuildRanksFinalize(this ServerSockets.Packet stream)
        {

            stream.Finalize(GamePackets.GuildRanks);
            return stream;
        }
    }
    public unsafe struct MsgGuildRanks
    {
        public enum RankTyp : ushort
        {
            SilverRank = 0,
            CpRank = 1,
            GuideDonation = 2,
            PkRank = 3,
            ArsenalRank = 4,
            RosesRank = 5,
            OrchidRank = 6,
            LilyRank = 7,
            TulipRank = 8,
            TotalDonaion = 9,
            MaxCounts = 10
        }
        [PacketAttribute(GamePackets.GuildRanks)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.MyGuild == null)
                return;
            RankTyp Rank = (RankTyp)stream.ReadUInt16();
            uint unknow = stream.ReadUInt32();
            ushort Page = stream.ReadUInt16();
            Page = (ushort)Math.Min(2, (int)Page);
            switch (Rank)
            {
                case RankTyp.SilverRank:
                    {
                        var array = user.Player.MyGuild.RankSilversDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));


                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.MoneyDonate);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.CpRank:
                    {
                        var array = user.Player.MyGuild.RankCPDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));


                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.CpsDonate);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.GuideDonation:
                    {
                        var array = user.Player.MyGuild.RankGuideDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));


                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.VirtutePointes);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.PkRank:
                    {
                        var array = user.Player.MyGuild.RankPkDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));


                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.PkDonation);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.ArsenalRank:
                    {
                        var array = user.Player.MyGuild.RankArsenalDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));


                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.ArsenalDonation);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.RosesRank:
                    {
                        var array = user.Player.MyGuild.RankRosseDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));

                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.Rouses);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.LilyRank:
                    {
                        var array = user.Player.MyGuild.RankLiliesDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));

                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.Lilies);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.OrchidRank:
                    {
                        var array = user.Player.MyGuild.RankOrchidsDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));


                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.Orchids);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.TulipRank:
                    {
                        var array = user.Player.MyGuild.RankTulipsDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));


                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.Tulips);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.TotalDonaion:
                    {
                        var array = user.Player.MyGuild.RankTotalDonations;

                        const int max = 10;
                        int offset = Page * max;
                        int count = Math.Min(max, Math.Max(0, array.Length - offset));


                        stream.GuildRanksCreate(Rank, (ushort)count, Page);

                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                var element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.TotalDonation);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
            }

        }

    }
}
