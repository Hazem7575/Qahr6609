using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
     
    public unsafe class MsgGuildMinDonations
    {
        public MsgGuildMinDonations(ServerSockets.Packet stream, ushort counts = 0)//31
        {
            stream.InitWriter();

            stream.Write((ushort)0);
            stream.Write(counts);
        }
        public void Append(ServerSockets.Packet stream, Role.Flags.GuildMemberRank Rank, uint amount)
        {

            stream.Write((uint)Rank);
            stream.Write(amount);
        }
        public ServerSockets.Packet ToArray(ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.GuildMinDonations);
            return stream;
        }
        public void AprendGuild(ServerSockets.Packet stream, Role.Instance.Guild guild)
        {
            if (guild.RankArsenalDonations.Length >= 5)
            {
                var obj = guild.RankArsenalDonations[4];
                Append(stream, Role.Flags.GuildMemberRank.Manager, obj.ArsenalDonation);
            }
            else
                Append(stream, Role.Flags.GuildMemberRank.Manager, 0);

            if (guild.RankArsenalDonations.Length >= 7)
            {
                var obj = guild.RankArsenalDonations[6];
                Append(stream,Role.Flags.GuildMemberRank.HonoraryManager, obj.ArsenalDonation);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.HonoraryManager, 0);


            if (guild.RankArsenalDonations.Length >= 8)
            {
                var obj = guild.RankArsenalDonations[7];
                Append(stream,Role.Flags.GuildMemberRank.Supervisor, obj.ArsenalDonation);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.Supervisor, 0);

            if (guild.RankArsenalDonations.Length >= 13)
            {
                var obj = guild.RankArsenalDonations[12];
                Append(stream,Role.Flags.GuildMemberRank.Steward, obj.ArsenalDonation);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.Steward, 0);
            if (guild.RankArsenalDonations.Length >= 15)
            {
                var obj = guild.RankArsenalDonations[14];
                Append(stream,Role.Flags.GuildMemberRank.ArsFollower, obj.ArsenalDonation);
            }
            else Append(stream,Role.Flags.GuildMemberRank.ArsFollower, 0);



            if (guild.RankCPDonations.Length >= 3)
            {
                var obj = guild.RankCPDonations[2];
                Append(stream,Role.Flags.GuildMemberRank.CPSupervisor, (uint)obj.CpsDonate);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.CPSupervisor, 0);
            if (guild.RankCPDonations.Length >= 5)
            {
                var obj = guild.RankCPDonations[4];
                Append(stream,Role.Flags.GuildMemberRank.CPAgent, (uint)obj.CpsDonate);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.CPAgent, 0);

            if (guild.RankCPDonations.Length >= 7)
            {
                var obj = guild.RankCPDonations[6];
                Append(stream,Role.Flags.GuildMemberRank.CPFollower, (uint)obj.CpsDonate);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.CPFollower, 0);


            if (guild.RankPkDonations.Length >= 3)
            {
                var obj = guild.RankPkDonations[2];
                Append(stream,Role.Flags.GuildMemberRank.PKSupervisor, obj.PkDonation);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.PKSupervisor, 0);

            if (guild.RankPkDonations.Length >= 5)
            {
                var obj = guild.RankPkDonations[4];
                Append(stream,Role.Flags.GuildMemberRank.PKAgent, obj.PkDonation);
            }
            else Append(stream,Role.Flags.GuildMemberRank.PKAgent, 0);

            if (guild.RankPkDonations.Length >= 7)
            {
                var obj = guild.RankPkDonations[6];
                Append(stream,Role.Flags.GuildMemberRank.PKFollower, obj.PkDonation);
            }
            else Append(stream,Role.Flags.GuildMemberRank.PKFollower, 0);




            if (guild.RankRosseDonations.Length >= 3)
            {
                var obj = guild.RankRosseDonations[2];
                Append(stream,Role.Flags.GuildMemberRank.RoseSupervisor, obj.Rouses);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.RoseSupervisor, 0);

            if (guild.RankRosseDonations.Length >= 5)
            {
                var obj = guild.RankRosseDonations[4];
                Append(stream,Role.Flags.GuildMemberRank.RoseAgent, obj.Rouses);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.RoseAgent, 0);

            if (guild.RankRosseDonations.Length >= 7)
            {
                var obj = guild.RankRosseDonations[6];
                Append(stream,Role.Flags.GuildMemberRank.RoseFollower, obj.Rouses);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.RoseFollower, 0);





            if (guild.RankLiliesDonations.Length >= 3)
            {
                var obj = guild.RankLiliesDonations[2];
                Append(stream,Role.Flags.GuildMemberRank.LilySupervisor, obj.Lilies);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.LilySupervisor, 0);
            if (guild.RankLiliesDonations.Length >= 5)
            {
                var obj = guild.RankLiliesDonations[4];
                Append(stream,Role.Flags.GuildMemberRank.LilyAgent, obj.Lilies);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.LilyAgent, 0);
            if (guild.RankLiliesDonations.Length >= 7)
            {
                var obj = guild.RankLiliesDonations[6];
                Append(stream,Role.Flags.GuildMemberRank.LilyFollower, obj.Lilies);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.LilyFollower, 0);





            if (guild.RankTulipsDonations.Length >= 3)
            {
                var obj = guild.RankTulipsDonations[2];
                Append(stream,Role.Flags.GuildMemberRank.TSupervisor, obj.Tulips);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.TSupervisor, 0);

            if (guild.RankTulipsDonations.Length >= 5)
            {
                var obj = guild.RankTulipsDonations[4];
                Append(stream,Role.Flags.GuildMemberRank.TulipAgent, obj.Tulips);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.TulipAgent, 0);
            if (guild.RankTulipsDonations.Length >= 7)
            {
                var obj = guild.RankTulipsDonations[6];
                Append(stream,Role.Flags.GuildMemberRank.TulipFollower, obj.Tulips);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.TulipFollower, 0);



            if (guild.RankOrchidsDonations.Length >= 3)
            {
                var obj = guild.RankOrchidsDonations[2];
                Append(stream,Role.Flags.GuildMemberRank.OSupervisor, obj.Orchids);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.OSupervisor, 0);

            if (guild.RankOrchidsDonations.Length >= 5)
            {
                var obj = guild.RankOrchidsDonations[4];
                Append(stream,Role.Flags.GuildMemberRank.OrchidAgent, obj.Orchids);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.OrchidAgent, 0);
            if (guild.RankOrchidsDonations.Length >= 7)
            {
                var obj = guild.RankOrchidsDonations[6];
                Append(stream,Role.Flags.GuildMemberRank.OrchidFollower, obj.Orchids);

            }
            else
                Append(stream,Role.Flags.GuildMemberRank.OrchidFollower, 0);



            if (guild.RankTotalDonations.Length >= 2)
            {
                var obj = guild.RankTotalDonations[1];
                Append(stream,Role.Flags.GuildMemberRank.HDeputyLeader, obj.TotalDonation);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.HDeputyLeader, 0);
            if (guild.RankTotalDonations.Length >= 4)
            {
                var obj = guild.RankTotalDonations[3];
                Append(stream,Role.Flags.GuildMemberRank.HonorarySteward, obj.TotalDonation);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.HonorarySteward, 0);





            if (guild.RankSilversDonations.Length >= 4)
            {
                var obj = guild.RankSilversDonations[3];
                Append(stream,Role.Flags.GuildMemberRank.SSupervisor, (uint)obj.MoneyDonate);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.SSupervisor, 0);
            if (guild.RankSilversDonations.Length >= 6)
            {
                var obj = guild.RankSilversDonations[5];
                Append(stream,Role.Flags.GuildMemberRank.SilverAgent, (uint)obj.MoneyDonate);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.SilverAgent, 0);

            if (guild.RankSilversDonations.Length >= 8)
            {
                var obj = guild.RankSilversDonations[7];
                Append(stream,Role.Flags.GuildMemberRank.SilverFollower, (uint)obj.MoneyDonate);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.SilverFollower, 0);


            if (guild.RankGuideDonations.Length >= 3)
            {
                var obj = guild.RankGuideDonations[2];
                Append(stream,Role.Flags.GuildMemberRank.GSupervisor, obj.VirtutePointes);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.GSupervisor, 0);

            if (guild.RankGuideDonations.Length >= 5)
            {
                var obj = guild.RankGuideDonations[4];
                Append(stream,Role.Flags.GuildMemberRank.GuideAgent, obj.VirtutePointes);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.GuideAgent, 0);

            if (guild.RankGuideDonations.Length >= 7)
            {
                var obj = guild.RankGuideDonations[6];
                Append(stream,Role.Flags.GuildMemberRank.GuideFollower, obj.VirtutePointes);
            }
            else
                Append(stream,Role.Flags.GuildMemberRank.GuideFollower, 0);
        }
    }
}
