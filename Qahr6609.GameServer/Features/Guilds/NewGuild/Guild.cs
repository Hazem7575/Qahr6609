using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgTournaments;

namespace Conquer6609.Role.Instance
{
    public class Guild
    {
        public class UpdateDB
        {

            public uint UID;
            public uint GuildID;
            public Flags.GuildMemberRank Rank;
        }
        public class Member
        {

            public Union.Member UnionMem;

            public uint UID;
            public string Name;
            public Flags.GuildMemberRank Rank;
            public uint Level;
            public byte Class;
            public uint NobilityRank;
            public byte Graden
            {
                get
                {
                    byte val = 2;
                    if (Mesh % 10 >= 2)
                        val =1;

                    return val;
                }
            }
            public long MoneyDonate;//save
            public uint CpsDonate;//save
            public uint VirtutePointes;
            public uint Mesh;
            public uint PrestigePoints = 0;

            public uint Lilies;
            public uint Rouses;
            public uint Orchids;
            public uint Tulips;

            public uint ArsenalDonation;
            public uint PkDonation;//save
          

            public bool IsOnline = false;
            public long LastLogin = 0;//save

            public uint AllFlowers { get { return (uint)(Lilies + Orchids + Tulips + Rouses); } }
            public uint TotalDonation { get { return (uint)(Lilies + Orchids + Tulips + Rouses + CpsDonate + VirtutePointes + (uint)MoneyDonate + ArsenalDonation + PkDonation); } }
            public bool Accepter
            {
                get { return (ushort)Rank > 870; }
            }

            public uint CTF_Exploits = 0;
            public uint RewardConquerPoints = 0;
            public uint RewardMoney = 0;
            public byte CTF_Claimed = 0;


        }
        public static bool Change(Client.GameClient client, string Name)
        {
            foreach (Guild guil in GuildPoll.Values)
                if (client.Player.GuildID == guil.Info.GuildID)
                    guil.GuildName = Name;

            return true;
        }
        public void CreateMembersRank()
        {
            lock (this)
            {
                //remove all ranks
                foreach (Member memb in Members.Values)
                {
                    if ((ushort)memb.Rank < 920)
                    {
                        if (RanksCounts[(ushort)memb.Rank] > 0)
                            RanksCounts[(ushort)memb.Rank]--;
                        memb.Rank = Role.Flags.GuildMemberRank.Member;
                        RanksCounts[(ushort)memb.Rank]++;
                    }
                    else // create old rank
                    {
                        //for deputi leaders and others.
                        RanksCounts[(ushort)memb.Rank]++;
                    }
                }
                Member[] Poll = null;

                //calculate manager`s
                const byte MaxMannager = 5;//0,1,2,3,4
                const byte MaxHonorManager = 2;//5,6,
                const byte MaxSupervisor = 2;//7,8,
                const byte MaxSteward = 4;//9,10,11,12
                const byte MaxArsFollower = 2;//13,14
                byte amount = 0;//8
                Poll = (from memb in Members.Values orderby memb.ArsenalDonation descending select memb).ToArray();
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.Manager)
                        continue;
                    if (amount < MaxMannager)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.Manager;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxMannager + MaxHonorManager)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.HonoraryManager;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxHonorManager + MaxMannager + MaxSupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.Supervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxHonorManager + MaxMannager + MaxSupervisor + MaxSteward)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.Steward)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.Steward;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxHonorManager + MaxMannager + MaxSupervisor + MaxSteward + MaxArsFollower)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.ArsFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.ArsFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankArsenalDonations = Poll.ToArray();

                //calculate rank cps
                const byte MaxCPSupervisor = 3;//0,1,2
                const byte MaxCpAgent = 2;//3,4
                const byte MaxCpFollower = 2;//5,6
                amount = 0;//3
                Poll = (from memb in Members.Values orderby memb.CpsDonate descending select memb).ToArray();
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.CPSupervisor)
                        continue;
                    if (amount < MaxCPSupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.CPSupervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxCPSupervisor + MaxCpAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.CPAgent)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.CPAgent;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxCPSupervisor + MaxCpAgent + MaxCpFollower)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.CPFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.CPFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankCPDonations = Poll.ToArray();

                //calculate pk ranks
                const byte MaxPkSupervisor = 3;//0,1,2
                const byte MaxPkAgent = 2;//3,4,
                const byte MaxPkFollower = 2;//5,6
                amount = 0;//3
                Poll = (from memb in Members.Values orderby memb.PkDonation descending select memb).ToArray();
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.PKSupervisor)
                        continue;
                    if (amount < MaxPkSupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.PKSupervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxPkSupervisor + MaxPkAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.PKAgent)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.PKAgent;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxPkSupervisor + MaxPkAgent + MaxPkFollower)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.PKFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.PKFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankPkDonations = Poll.ToArray();

                //calculate RoseSupervisor
                const byte MaxRoseSupervisor = 3;//0,1,2
                const byte MaxRoseAgent = 2;//3,4
                const byte MaxRoseFollower = 2;//5,6
                amount = 0;//3
                Poll = (from memb in Members.Values orderby memb.Rouses descending select memb).ToArray();
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.RoseSupervisor)
                        continue;
                    if (amount < MaxRoseSupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.RoseSupervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxRoseSupervisor + MaxRoseAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.RoseAgent)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.RoseAgent;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxRoseSupervisor + MaxRoseAgent + MaxRoseFollower)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.RoseFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.RoseFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankRosseDonations = Poll.ToArray();

                //calculate LilySupervisor
                const byte MaxLilySupervisor = 3;
                const byte MaxLilyAgent = 2;
                const byte MaxLilyFollower = 2;
                amount = 0;//3
                Poll = (from memb in Members.Values orderby memb.Lilies descending select memb).ToArray();
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.LilySupervisor)
                        continue;
                    if (amount < MaxLilySupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.LilySupervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxLilySupervisor + MaxLilyAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.LilyAgent)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.LilyAgent;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxLilySupervisor + MaxLilyAgent + MaxLilyFollower)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.LilyFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.LilyFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankLiliesDonations = Poll.ToArray();

                //calculate TulipAgent
                const byte MaxTSupervisor = 3;
                const byte MaxTulipAgent = 2;
                const byte MaxTulupFollower = 2;
                amount = 0;//3
                Poll = (from memb in Members.Values orderby memb.Tulips descending select memb).ToArray();
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.TSupervisor)
                        continue;
                    if (amount < MaxTSupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.TSupervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxTSupervisor + MaxTulipAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.TulipAgent)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.TulipAgent;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxTSupervisor + MaxTulipAgent + MaxTulupFollower)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.TulipFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.TulipFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankTulipsDonations = Poll.ToArray();

                // calculate OrchidAgent
                const byte MaxOSupervisor = 3;
                const byte MaxOrchidAgent = 2;
                const byte MaxOrchidFollower = 2;
                amount = 0;//3
                Poll = (from memb in Members.Values
                        orderby memb.Tulips descending
                        select memb).ToArray();
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.OSupervisor)
                        continue;
                    if (amount < MaxOSupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.OSupervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxOSupervisor + MaxOrchidAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.OrchidAgent)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.OrchidAgent;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < MaxOSupervisor + MaxOrchidFollower + MaxOrchidAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.OrchidFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.OrchidFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankOrchidsDonations = Poll.ToArray();



                Poll = (from memb in Members.Values
                        orderby memb.TotalDonation descending
                        select memb).ToArray();

                const byte HDeputyLeader = 2;//0,1
                const byte MaxHonorarySteward = 2;//2,3
                amount = 0;//20
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.HDeputyLeader)
                        continue;
                    if (amount < HDeputyLeader)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.HDeputyLeader;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < HDeputyLeader + MaxHonorarySteward)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.HonorarySteward)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.HonorarySteward;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }

                RankTotalDonations = Poll.ToArray();


                const byte SSupervisor = 5;//0,1,2,3
                const byte MaxSilverAgent = 2;//4,5
                const byte MaxSilverFollowr = 2;//6,7
                amount = 0;//20
                Poll = (from memb in Members.Values
                        orderby memb.MoneyDonate descending
                        select memb).ToArray();
                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.SSupervisor)
                        continue;
                    if (amount < SSupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.SSupervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < SSupervisor + MaxSilverAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.SilverAgent)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.SilverAgent;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < SSupervisor + MaxSilverAgent + MaxSilverFollowr)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.SilverFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.SilverFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankSilversDonations = Poll.ToArray();

                const byte GSupervisor = 3;//0,1,2
                const byte MaxGAgent = 2;//3,4
                const byte MaxGFollower = 2;//5,6
                amount = 0;//20
                Poll = (from memb in Members.Values
                        orderby memb.VirtutePointes descending
                        select memb).ToArray();

                for (byte x = 0; x < Poll.Length; x++)
                {
                    Member membru = Poll[x];
                    if (membru.Rank > Role.Flags.GuildMemberRank.GSupervisor)
                        continue;
                    if (amount < GSupervisor)
                    {
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.GSupervisor;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < GSupervisor + MaxGAgent)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.GuideAgent)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.GuideAgent;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else if (amount < GSupervisor + MaxGAgent + MaxGFollower)
                    {
                        if (membru.Rank > Role.Flags.GuildMemberRank.GuideFollower)
                            continue;
                        if (RanksCounts[(ushort)membru.Rank] > 0)
                            RanksCounts[(ushort)membru.Rank]--;
                        membru.Rank = Role.Flags.GuildMemberRank.GuideFollower;
                        RanksCounts[(ushort)membru.Rank]++;
                        amount++;
                    }
                    else
                        break;
                }
                RankGuideDonations = Poll.ToArray();
            }
        }
        public class Advertise
        {
            public static System.Collections.Concurrent.ConcurrentDictionary<uint, Guild> AGuilds = new System.Collections.Concurrent.ConcurrentDictionary<uint, Guild>();

            public static Guild[] AdvertiseRanks = new Guild[0];
            public static void Add(Guild obj)
            {
                obj.UseAdvertise = true;
                if (!AGuilds.ContainsKey(obj.Info.GuildID))
                    AGuilds.TryAdd(obj.Info.GuildID, obj);
                CalculateRanks();
            }

            public static void CalculateRanks()
            {
                lock (AdvertiseRanks)
                {
                    Guild[] array = AGuilds.Values.ToArray();
                    array = (from guil in array orderby guil.AdvertiseRecruit.Donations descending select guil).ToArray();
                    List<Guild> listarray = new List<Guild>();
                    for (ushort x = 0; x < array.Length; x++)
                    {
                        listarray.Add(array[x]);
                        if (x == 40)
                            break;
                    }
                    AdvertiseRanks = listarray.ToArray();
                }
            }
        }
        public class Recruitment
        {
            public Recruitment()
            {
                NotAllowFlag = 0;
            }
            public enum Mode
            {
                Requirements, Recruit
            }
            public class Flags
            {
                public const int
                    NoneBlock = 0,
                    Trojan = 1,
                    Warrior = 2,
                    Taoist = 4,
                    Archas = 8,
                    Ninja = 16,
                    Monk = 32,
                    Pirate = 64;
            }

            // Auto-join is only enabled when the guild leader turns it on while advertising.
            public bool AutoJoin = false;
            public string Buletin = "Nothing";
            public int NotAllowFlag;
            public byte Level = 0;
            public byte Reborn = 0;
            public byte Grade = 0;
            public long Donations;

            public bool ContainFlag(int val)
            {
                bool vaal = ((NotAllowFlag & val) == val);
                return vaal;
            }
            public void AddFlag(int val)
            {
                if (!ContainFlag(val))
                    NotAllowFlag |= val;
            }
            public void Remove(int val)
            {
                if (ContainFlag(val))
                    NotAllowFlag &= ~val;
            }
            public void SetFlag(int m_flag, Mode mod)
            {
                switch (mod)
                {
                    case Mode.Requirements:
                        {
                            NotAllowFlag = m_flag;
                            //break;
                            if (m_flag == 0) NotAllowFlag = Flags.NoneBlock;
                            if (m_flag >= 127)
                                AddFlag(Flags.Trojan | Flags.Warrior | Flags.Taoist | Flags.Archas | Flags.Ninja | Flags.Monk | Flags.Pirate);

                            int n_flag = 127 - m_flag;
                            AddFlag(n_flag);
                            break;
                        }
                    case Mode.Recruit:
                        {
                            if (m_flag == 0) NotAllowFlag = Flags.NoneBlock;
                            AddFlag(m_flag);
                            break;
                        }
                }
            }
            public bool Compare(Role.Player player, Mode mod)
            {
                if (player.Level < Level)
                    return false;
                if (player.Reborn < Reborn && Reborn != 0)
                    return false;
                if (Database.AtributesStatus.IsArcher(player.Class) && ContainFlag(Flags.Archas))
                    return false;
                if (Database.AtributesStatus.IsTaoist(player.Class) && ContainFlag(Flags.Taoist))
                    return false;
                if (Database.AtributesStatus.IsWarrior(player.Class) && ContainFlag(Flags.Warrior))
                    return false;
                if (Database.AtributesStatus.IsTrojan(player.Class) && ContainFlag(Flags.Trojan))
                    return false;
                if (Database.AtributesStatus.IsPirate(player.Class) && ContainFlag(Flags.Pirate))
                    return false;
                if (Database.AtributesStatus.IsMonk(player.Class) && ContainFlag(Flags.Monk))
                    return false;
                if (Database.AtributesStatus.IsNinja(player.Class) && ContainFlag(Flags.Ninja))
                    return false;
                if (mod == Mode.Recruit)
                {
                    if (Grade == 0) return true;
                    if (player.Mesh != Grade)
                        return false;
                }

                return true;
            }
            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                return writer.Add(NotAllowFlag).Add(Level).Add(Reborn).Add(Grade).Add(Donations).Add((byte)(AutoJoin ? 1 : 0))
                      .Add(Buletin).Add(0).Add(0).Add(0).Close();
            }
            public void Load(string line)
            {
                Database.DBActions.ReadLine Reader = new Database.DBActions.ReadLine(line, '/');
                NotAllowFlag = Reader.Read(0);
                Level = Reader.Read((byte)0);
                Reborn = Reader.Read((byte)0);
                Grade = Reader.Read((byte)0);
                Donations = Reader.Read((long)0);
                AutoJoin = Reader.Read((byte)0) == 1;
                Buletin = Reader.Read("");
            }

        }
        public Member GetGuildLeader
        {

            get
            {
                return Members.Values.Where(p => p.Rank == Flags.GuildMemberRank.GuildLeader).FirstOrDefault();
            }
        }

        public Member[] RankSilversDonations = new Member[0];
        public Member[] RankArsenalDonations = new Member[0];
        public Member[] RankCPDonations = new Member[0];
        public Member[] RankPkDonations = new Member[0];
        public Member[] RankLiliesDonations = new Member[0];
        public Member[] RankOrchidsDonations = new Member[0];
        public Member[] RankRosseDonations = new Member[0];
        public Member[] RankTulipsDonations = new Member[0];
        public Member[] RankGuideDonations = new Member[0];
        public Member[] RankTotalDonations = new Member[0];

        public static Extensions.Counter Counter = new Extensions.Counter(1000);
        public static ConcurrentDictionary<uint, Guild> GuildPoll = new ConcurrentDictionary<uint, Guild>();

        public Game.MsgServer.MsgGuildInformation Info;
        public uint ClaimCtfReward = 0;

        public uint CTF_Exploits = 0;
        public uint CTF_Next_ConquerPoints = 0;
        public uint CTF_Next_Money = 0;
        public uint CTF_Rank = 0;

        public uint[] GetLeaderReward()
        {
            const uint ConquerPoints = ServerRate.CTF
                , Money = 120000000;

            uint[] wReward = new uint[2];
            if (CTF_Rank != 0)
            {
                wReward[0] = ConquerPoints / CTF_Rank;
                wReward[1] = Money / CTF_Rank;
            }
            return wReward;
        }

        public uint UnionID;
        public Union GetUnion
        {
            get
            {
                Union _union;
                if (Union.UnionPoll.TryGetValue(UnionID, out _union))
                    return _union;
                return null;
            }
        }

        public Member[] GetOnlineMembers
        {
            get { return Members.Values.OrderByDescending(p => p.IsOnline).ToArray();  }
        }

        public ConcurrentDictionary<uint, Member> Members;
        public ConcurrentDictionary<uint, Guild> Ally;
        public ConcurrentDictionary<uint, Guild> Enemy;
        private System.Collections.Concurrent.ConcurrentDictionary<byte, Arsenal.Arsenals> Pool_Arsenals = new System.Collections.Concurrent.ConcurrentDictionary<byte, Arsenal.Arsenals>();
        public Arsenal MyArsenal;
        public string Bulletin = "None";
        public string GuildName = "";
        public ushort[] RanksCounts = new ushort[(ushort)Flags.GuildMemberRank.GuildLeader + 1];
        public Recruitment Recruit;
        public Recruitment AdvertiseRecruit;

        public bool CanSave = true;
        private bool Addable(Role.Player player)
        {
            if (!this.Recruit.Compare(player, Recruitment.Mode.Requirements))
                return false;
            return true;
        }
        public Guild(Client.GameClient client, string Name, ServerSockets.Packet stream)
        {
            AdvertiseRecruit = new Recruitment();
            Recruit = new Recruitment();
            MyArsenal = new Arsenal();
            Info = Game.MsgServer.MsgGuildInformation.Create();
            Members = new ConcurrentDictionary<uint, Member>();
            Ally = new ConcurrentDictionary<uint, Guild>();
            Enemy = new ConcurrentDictionary<uint, Guild>();
            GuildName = Name;
            if (client != null)
            {
                DateTime Now = DateTime.Now;
                Info.LeaderName = client.Player.Name;
                Info.GuildID = Counter.Next;
                Info.SilverFund = 500000;
                Info.CreateTime = (uint)GetTime(Now.Year, Now.Month, Now.Day);
                Info.Level = MyArsenal.GetGuildLevel;

                AddPlayer(client.Player, stream);

                GuildPoll.TryAdd(Info.GuildID, this);

               
            }
        }

        public unsafe void AddPlayer(Role.Player player, ServerSockets.Packet stream)
        {
            if (player.MyGuild == null)
            {
                /*if (this.Addable(player) == false)
                    return;*/
                Member memb = new Member();
                if (player.MyUnion != null)
                {
                    if (player.MyUnion.UID == this.UnionID)
                    {
                        Role.Instance.Union.Member member;
                        player.MyUnion.Members.TryRemove(player.UID, out member);
                    }
                    else
                    {
                        player.MyUnion.Quit(stream, player.UnionMemeber);
                        //if (GetUnion != null)
                        //{
                        //    memb.UnionMem = Role.Instance.Union.Member.CreateMember(player.Owner, Union.Member.MilitaryRanks.Member);
                        //    memb.UnionMem.Owner = player.Owner;
                        //    player.MyUnion = GetUnion;
                        //    player.UnionMemeber = memb.UnionMem;
                        //}
                    }
                }
                else
                {
                    //if (GetUnion != null)
                    //{
                    //    memb.UnionMem = Role.Instance.Union.Member.CreateMember(player.Owner, Union.Member.MilitaryRanks.Member);
                    //    memb.UnionMem.Owner = player.Owner;
                    //    player.MyUnion = GetUnion;
                    //    player.UnionMemeber = memb.UnionMem;
                    //}
                }

                memb.IsOnline = true;
                memb.Name = player.Name;
                if (Members.Count == 0)
                    memb.Rank = Flags.GuildMemberRank.GuildLeader;
                else
                    memb.Rank = Flags.GuildMemberRank.Member;
                memb.UID = player.UID;
                memb.Level = player.Level;
                memb.NobilityRank = (byte)player.NobilityRank;


                if (GetGuildLeader != null && player.Spouse == GetGuildLeader.Name)
                    memb.Rank = Flags.GuildMemberRank.LeaderSpouse;
                if (Members.Count == 0)
                    Info.SilverFund = memb.MoneyDonate = 500000;

                memb.Class = player.Class;
                // memb.Graden = player.GetGender;
                memb.Mesh = player.Mesh;
                if (player.Flowers != null)
                {
                    memb.Lilies = player.Flowers.Lilies;
                    memb.Rouses = player.Flowers.RedRoses;
                    memb.Orchids = player.Flowers.Orchids;
                    memb.Tulips = player.Flowers.Tulips;
                }

                Members.TryAdd(memb.UID, memb);
                Info.MembersCount = (ushort)Members.Count;


                player.GuildID = Info.GuildID;
                player.GuildRank = memb.Rank;
                player.MyGuild = this;
                player.MyGuildMember = memb;

                Info.MyRank = (ushort)memb.Rank;
                if (Program.ServerConfig.IsInterServer == false && player.Owner.OnInterServer == false)
                {
                    SendThat(player);
                    player.View.SendView(player.GetArray(stream, false), false);

                    player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildName, Info.GuildID, true, new string[1] { GuildName + " " + Info.LeaderName + " " + Info.Level + " " + Info.MembersCount });
                }

                RanksCounts[(ushort)memb.Rank]++;

                player.GuildBattlePower = ShareMemberPotency(memb.Rank);

                if (GetUnion != null)
                {
                    memb.UnionMem = Union.Member.CreateMember(memb);
                    GetUnion.SendMyInfo(stream, player.Owner);
                }

                if (Program.ServerConfig.IsInterServer == false && player.Owner.OnInterServer == false)
                {
                    Game.MsgServer.MsgGuildMinDonations MinimDonation = new MsgGuildMinDonations(stream, 31);
                    MinimDonation.AprendGuild(stream, this);
                    player.Owner.Send(MinimDonation.ToArray(stream));
                }
            }
        }
        public uint GetFullPotencyArsenal()
        {
            return MyArsenal.GetFullBp();
        }
        public uint ShareMemberPotency(Role.Flags.GuildMemberRank RankMember)
        {
           // uint GetArsenalPotency = GetFullPotencyArsenal();
           //// return GetArsenalPotency;

            return (uint)15;
        }
        /// <summary>A rank is assignable only if it is a defined rank (not None) inside the RanksCounts table.</summary>
        public bool IsValidRank(uint rank)
        {
            if (rank == 0 || rank >= (uint)RanksCounts.Length)
                return false;
            return Enum.IsDefined(typeof(Role.Flags.GuildMemberRank), (ushort)rank);
        }
        public unsafe bool Promote(uint rank, Role.Player owner, string Name, ServerSockets.Packet stream)
        {
            if (owner == null || owner.MyGuildMember == null || !IsValidRank(rank))
                return false;

            Member membru = GetMember(Name);

            if (membru != null)
            {
                // The guild leader keeps the rank until leadership is transferred to another member.
                if (membru.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                    return false;
                // Leadership can only be handed over by the current leader to another member.
                if (rank == (ushort)Role.Flags.GuildMemberRank.GuildLeader
                    && (membru.UID == owner.UID || owner.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader))
                    return false;
                if (rank == (ushort)Role.Flags.GuildMemberRank.DeputyLeader && membru.Rank != Role.Flags.GuildMemberRank.DeputyLeader
                    && RanksCounts[(ushort)Role.Flags.GuildMemberRank.DeputyLeader] >= 8)
                    return false;

                SendMessajGuild("" + owner.Name + " has appointed " + Name + " as " + ((Role.Flags.GuildMemberRank)rank).ToString() + ".");

                if (RanksCounts[(ushort)membru.Rank] > 0)
                    RanksCounts[(ushort)membru.Rank]--;
                switch (rank)
                {
                    case (ushort)Role.Flags.GuildMemberRank.GuildLeader:
                        {
                            membru.Rank = Role.Flags.GuildMemberRank.GuildLeader;
                            Info.LeaderName = membru.Name;
                            bool Online = false;
                            Client.GameClient client;
                            if (Database.Server.GamePoll.TryGetValue(membru.UID, out client))
                            {
                                Online = true;
                                client.Player.GuildRank = Role.Flags.GuildMemberRank.GuildLeader;
                                client.Player.View.SendView(client.Player.GetArray(stream,false), false);
                                Info.MyRank = (ushort)membru.Rank;
                                client.Player.GuildBattlePower = ShareMemberPotency(membru.Rank);
                                SendThat(client.Player);
                            }
                            var union = GetUnion;
                            if (union != null)
                            {
                                if (union.EmperrorUID == owner.UID)
                                {
                                    union.EmperrorUID = membru.UID;
                                    union.Emperor = membru.Name;
                                    if (membru.UnionMem != null)
                                    {
                                        union.RemoveEmperor();
                                        union.AddEmperror(membru.UnionMem);
                                    }
                                }
                            }
                            // The leader slot moves from owner to membru; owner becomes a member.
                            owner.MyGuildMember.Rank = Role.Flags.GuildMemberRank.Member;
                            RanksCounts[(ushort)Role.Flags.GuildMemberRank.Member]++;
                            owner.GuildRank = Role.Flags.GuildMemberRank.Member;
                            owner.View.SendView(owner.GetArray(stream,false), false);
                            Info.MyRank = (ushort)membru.Rank;
                            SendThat(owner);
                         //   SendMessajGuild("" + membru.Name + " is now " + ((Role.Flags.GuildMemberRank)rank).ToString() + ".");
                            if (!Online)
                            {
                                Database.ServerDatabase.LoginQueue.TryEnqueue(membru);
                            }
                            break;
                        }
                    case (ushort)Role.Flags.GuildMemberRank.DeputyLeader:
                        {
                            bool online = false;
                            RanksCounts[rank]++;
                            membru.Rank = Role.Flags.GuildMemberRank.DeputyLeader;
                            Client.GameClient client;
                            if (Database.Server.GamePoll.TryGetValue(membru.UID, out client))
                            {
                                online = true;
                                client.Player.GuildRank = Role.Flags.GuildMemberRank.DeputyLeader;
                                client.Player.View.SendView(client.Player.GetArray(stream,false), false);
                                client.Player.GuildBattlePower = ShareMemberPotency(membru.Rank);
                                Info.MyRank = (ushort)membru.Rank;
                                SendThat(client.Player);
                                
                            }
                            SendMessajGuild("" + membru.Name + " is now " + ((Role.Flags.GuildMemberRank)rank).ToString() + ".");
                            if (!online)
                            {
                                Database.ServerDatabase.LoginQueue.TryEnqueue(membru);
                            }
                            break;
                        }
                    default:
                        {
                            bool online = false;
                            RanksCounts[rank]++;
                            membru.Rank = (Role.Flags.GuildMemberRank)rank;
                            Client.GameClient client;
                            if (Database.Server.GamePoll.TryGetValue(membru.UID, out client))
                            {
                                online = true;
                                client.Player.GuildRank = membru.Rank;
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                client.Player.GuildBattlePower = ShareMemberPotency(membru.Rank);
                                Info.MyRank = (ushort)membru.Rank;
                                SendThat(client.Player);
                              
                            }
                           // SendMessajGuild("" + membru.Name + " is now Rank : [" + ((Role.Flags.GuildMemberRank)rank).ToString() + "].");
                            if (!online)
                            {
                                Database.ServerDatabase.LoginQueue.TryEnqueue(membru);
                            }
                            break;
                        }
                     
                       
                }
                return true;
            }
            return false;
        }
        public bool AllowAddAlly(string Name)
        {
            foreach (Guild all in Ally.Values)
                if (all.GuildName == Name) return false;
            return true;
        }
        public bool AllowAddEnemy(string name)
        {
            foreach (Guild all in Enemy.Values)
                if (all.GuildName == name) return false;
            return true;
        }
        public bool IsEnemy(string Name)
        {
            foreach (Guild gui in Enemy.Values)
                if (gui.GuildName == Name)
                    return true;
            return false;
        }
        public static Member GetLeaderGuild(string guildname)
        {
            foreach (var obj in GuildPoll.Values)
            {
                if (obj.GuildName == guildname)
                {
                    return obj.GetGuildLeader;
                }
            }
            return null;
        }
        public uint AllowAlly = 0;
        public bool AddAlly(ServerSockets.Packet stream, string Name)
        {
            if (Ally.Count >= 15)
                return false;
            if (!AllowAddAlly(Name))
                return false;
            Guild GuildAlly = null;
            foreach (Guild gui in GuildPoll.Values)
                if (gui.GuildName == Name)
                {
                    GuildAlly = gui;
                    break;
                }
            if (GuildAlly != null)
            {
                AllowAlly = GuildAlly.Info.GuildID;
                if (GuildAlly.AllowAlly == Info.GuildID)
                {
                    GuildAlly.Ally.TryAdd(Info.GuildID, this);
                    Ally.TryAdd(GuildAlly.Info.GuildID, GuildAlly);
                    GuildAlly.SendGuildAlly(stream, false, null);
                    SendGuildAlly(stream,false, null);
                    return true;
                }
            }
            return false;
        }
        public void AddEnemy(ServerSockets.Packet stream, string Name)
        {
            if (Enemy.Count >= 15) return;
            if (AllowAddEnemy(Name))
            {
                Guild GuildEnnemy = null;
                foreach (Guild gui in GuildPoll.Values)
                    if (gui.GuildName == Name)
                    {
                        GuildEnnemy = gui;
                        break;
                    }
                if (GuildEnnemy != null)
                {
                    Enemy.TryAdd(GuildEnnemy.Info.GuildID, GuildEnnemy);
                    SendGuilEnnemy(stream,false, null);
                }
            }
        }
        public void UpdateGuildInfo()
        {
            Info.Level = MyArsenal.GetGuildLevel;
            Info.MembersCount = (uint)Members.Count;
        }
        public unsafe void RemoveAlly(string Name,ServerSockets.Packet stream)
        {
            if (AllowAddAlly(Name))
                return;
            Guild GuildAlly = null;
            foreach (Guild gui in Ally.Values)
                if (gui.GuildName == Name)
                {
                    GuildAlly = gui;
                    break;
                }
            if (GuildAlly != null)
            {
                foreach (Client.GameClient aclient in Database.Server.GamePoll.Values)
                {
                    if (aclient.Player.GuildID == Info.GuildID)
                        aclient.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.RemoveAlly, GuildAlly.Info.GuildID, new int[3], ""));
                    if (aclient.Player.GuildID == GuildAlly.Info.GuildID)
                        aclient.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.RemoveAlly, Info.GuildID, new int[3], ""));
                }
                Guild rem;
                GuildAlly.Ally.TryRemove(Info.GuildID, out rem);
                Ally.TryRemove(GuildAlly.Info.GuildID, out rem);
            }
        }
        public void SendGuilEnnemy(ServerSockets.Packet stream, bool JustMe, Client.GameClient client)
        {
            if (JustMe)
            {
                foreach (Guild GuildEnemie in Enemy.Values)
                {
                        client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildEnemies, GuildEnemie.Info.GuildID, false
                           , new string[1] { GuildEnemie.GuildName + " " + GuildEnemie.Info.LeaderName + " " + GuildEnemie.Info.Level + " " + GuildEnemie.Members.Count +""});
                }
            }
            else
            {
                foreach (Client.GameClient GuildMember in Database.Server.GamePoll.Values)
                {
                    if (GuildMember.Player.GuildID == Info.GuildID)
                    {
                        foreach (Guild GuildEnemie in Enemy.Values)
                        {
                            GuildMember.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildEnemies, GuildEnemie.Info.GuildID, false
                                , new string[1] { GuildEnemie.GuildName + " " + GuildEnemie.Info.LeaderName + " " + GuildEnemie.Info.Level + " " + GuildEnemie.Members.Count });
                        
                        }
                    }
                }
            }
        }
        public void SendGuildAlly(ServerSockets.Packet stream, bool JustMe, Client.GameClient client)
        {
            if (JustMe)
            {
                foreach (Guild AllyGuild in Ally.Values)
                {
                    client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildAllies, AllyGuild.Info.GuildID, false
                        , new string[1] { AllyGuild.GuildName + " " + AllyGuild.Info.LeaderName + " " + AllyGuild.Info.Level + " " + AllyGuild.Members.Count });
                }
            }
            else
            {
                foreach (Client.GameClient GuildMember in Database.Server.GamePoll.Values)
                {
                    if (GuildMember.Player.GuildID == Info.GuildID)
                    {
                        foreach (Guild AllyGuild in Ally.Values)
                        {
                            GuildMember.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildAllies, AllyGuild.Info.GuildID, false
                                , new string[1] { AllyGuild.GuildName + " " + AllyGuild.Info.LeaderName + " " + AllyGuild.Info.Level + " " + AllyGuild.Members.Count });
                        }
                    }
                }
            }
        }
        public unsafe void RemoveEnemy(string Name,ServerSockets.Packet stream)
        {
            if (AllowAddEnemy(Name))
                return;
            Guild GuildEnemy = null;
            foreach (Guild gui in Enemy.Values)
                if (gui.GuildName == Name)
                {
                    GuildEnemy = gui;
                    break;
                }
            if (GuildEnemy != null)
            {
                foreach (Client.GameClient aclient in Database.Server.GamePoll.Values)
                {
                    if (aclient.Player.GuildID == Info.GuildID)
                    {

                        aclient.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.RemoveEnemy, GuildEnemy.Info.GuildID, new int[3], ""));
                    }
                }
               
                Guild rem;
                Enemy.TryRemove(GuildEnemy.Info.GuildID, out rem);
            }
        }
        public unsafe void Dismis(Client.GameClient client,ServerSockets.Packet stream)
        {
            try
            {
                if (Members.Count == 1)
                {
                    Guild dismising;
                    if (GuildPoll.TryRemove(Info.GuildID, out dismising))
                    {
                        if (Ally.Count > 0)
                        {
                            foreach (var GuildAlly in Ally.Values)
                                GuildAlly.RemoveAlly(GuildName,stream);
                        }
                        foreach (var Guilds in GuildPoll.Values)
                        {
                            if (Guilds.Info.GuildID != Info.GuildID)
                            {
                                if (Guilds.Enemy.ContainsKey(Info.GuildID))
                                    Guilds.RemoveEnemy(GuildName,stream);
                            }
                        }

                        lock (client.Equipment.ClientItems)
                        {
                            foreach (var GameItem in client.Equipment.ClientItems.Values)
                            {
                                if (GameItem.Inscribed == 1)
                                {
                                    GameItem.Inscribed = 0;
                                    GameItem.Mode = Flags.ItemMode.Update;
                                    GameItem.Send(client,stream);
                                }
                            }
                        }
                        lock (client.Inventory.ClientItems)
                        {
                            foreach (var GameItem in client.Inventory.ClientItems.Values)
                            {
                                if (GameItem.Inscribed == 1)
                                {
                                    GameItem.Inscribed = 0;
                                    GameItem.Mode = Flags.ItemMode.Update;
                                    GameItem.Send(client,stream);
                                }
                            }
                        }
                        client.Warehouse.RemoveInscribedItems();

                        client.Player.GuildID = 0;
                        client.Player.GuildRank = 0;
                        client.Player.MyGuild = null;
                        client.Player.MyGuildMember = null;


                        client.Player.View.SendView(stream.GuildRequestCreate(MsgGuildProces.GuildAction.Disband, Info.GuildID, new int[3], ""),true);

                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);

                        Guild myguild;
                        if (Advertise.AGuilds.TryRemove(Info.GuildID, out myguild))
                        {
                            Advertise.CalculateRanks();
                        }
                    }
                }
                else
                {
                    SendMessajGuild("Please kick all members, and next dismising");
                 
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public unsafe void Discharge(string Name, ServerSockets.Packet stream)
        {
            try
            {
                Member membru = GetMember(Name);
                // The guild leader can never be discharged.
                if (membru != null && membru.Rank != Flags.GuildMemberRank.GuildLeader)
                {
                    bool online = false;
                    if (RanksCounts[(ushort)membru.Rank] > 0)
                        RanksCounts[(ushort)membru.Rank]--;
                    membru.Rank = Flags.GuildMemberRank.Member;
                    RanksCounts[(ushort)membru.Rank]++;
                    Client.GameClient client;
                    if (Database.Server.GamePoll.TryGetValue(membru.UID, out client))
                    {
                        online = true;
                        client.Player.GuildRank = Role.Flags.GuildMemberRank.Member;
                        Info.MyRank = (ushort)membru.Rank;
                        client.Player.GuildBattlePower = ShareMemberPotency(membru.Rank);
                        SendThat(client.Player);
                    }
                    SendMessajGuild("" + membru.Name + " is now Discharged.");
                    if (!online)
                    {
                        Database.ServerDatabase.LoginQueue.TryEnqueue(membru);
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public int BuletinEnrole = 0;

        public void CreateBuletinTime(int Time = 0)
        {
            if (Time == 0)
            {
                var timers = DateTime.Now;
                Time = GetTime(timers.Year, timers.Month, timers.Day);
            }
            BuletinEnrole = Time;
        }
        public int GetTime(int year, int month, int day)
        {
            int Timer = year * 10000 + month * 100 + day;
            return Timer;
        }
   
        public unsafe void SendMessajGuild(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.Guild
            , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.yellow)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                // Same recipients; no pool snapshot and the message is built once (Send copies it).
                ServerSockets.Packet msg = null;
                foreach (var kv in Database.Server.GamePoll)
                {
                    var user = kv.Value;
                    if (user.Player != null && user.Player.GuildID == Info.GuildID)
                    {
                        if (msg == null)
                            msg = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                        user.Send(msg);
                    }
                }
            }
        }
      
        public unsafe void SendPacket(ServerSockets.Packet packet)
        {
            foreach (var kv in Database.Server.GamePoll)
            {
                var user = kv.Value;
                if (user.Player != null && user.Player.GuildID == Info.GuildID)
                    user.Send(packet);
            }
        }
        public unsafe void SendThat(Role.Player player)
        {
            if (player.MyGuildMember == null)
                return;
            if (Bulletin != null && Bulletin != "" && Bulletin != "None")
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    player.Owner.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.Bulletin, (uint)BuletinEnrole, new int[3], Bulletin));
                }
            }
            Info.MembersCount = (uint)Members.Count;
            Info.Level = MyArsenal.GetGuildLevel;
            Info.MyRank = (ushort)player.MyGuildMember.Rank;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                player.Owner.Send(stream.GuildInformationCreate(Info));

            }
        }
        public Member GetMember(string name)
        {
            foreach (Member memb in Members.Values)
                if (memb.Name == name)
                    return memb;
            return null;
        }
        public void Quit(string name, bool ReceiveKick, ServerSockets.Packet stream)
        {
       
            Member member = GetMember(name);
            if (member == null) return;
            // A kick can never remove the guild leader.
            if (ReceiveKick && member.Rank == Flags.GuildMemberRank.GuildLeader)
                return;
            if (ReceiveKick)
            {
                SendMessajGuild(member.Name + " have been expelled from our guild.");

            }
            else
            {
                SendMessajGuild(member.Name + " has quit our guild.");

            }
            if (RanksCounts[(ushort)member.Rank] > 0)
                RanksCounts[(ushort)member.Rank]--;

            // Items inscribed by a member leave the arsenal together with the member.
            foreach (var quiter in MyArsenal.GetCllection())
                quiter.RemoveAllClientItem(member.UID);

            Client.GameClient client;
            if (Database.Server.GamePoll.TryGetValue(member.UID, out client))
            {

                foreach (var GameItem in client.Equipment.ClientItems.Values)
                {
                    if (GameItem.Inscribed == 1)
                    {
                        GameItem.Inscribed = 0;
                        GameItem.Mode = Flags.ItemMode.Update;
                        GameItem.Send(client, stream);
                    }
                }


                foreach (var GameItem in client.Inventory.ClientItems.Values)
                {
                    if (GameItem.Inscribed == 1)
                    {
                        GameItem.Inscribed = 0;
                        GameItem.Mode = Flags.ItemMode.Update;
                        GameItem.Send(client, stream);
                    }
                }

                client.Warehouse.RemoveInscribedItems();

                client.Player.GuildID = 0;
                client.Player.GuildRank = 0;
                client.Player.MyGuild = null;
                client.Player.MyGuildMember = null;

                client.Send(stream.GuildRequestCreate(MsgGuildProces.GuildAction.Disband, Info.GuildID, new int[3], ""));


                client.Player.View.Clear(stream);
                client.Player.View.Role();
                client.Player.GuildBattlePower = 0;

                if (member.UnionMem != null)
                {
                    var union = GetUnion;
                    if (union != null)
                        union.AddOtherMember(stream, client);
                    else
                        if (client.Player.InUnion)
                            client.Player.MyUnion.AddOtherMember(stream, client);
                }
            }
            else
            {
                Database.ServerDatabase.LoginQueue.TryEnqueue(new UpdateDB()
                    {
                        GuildID = 0,
                        Rank = Flags.GuildMemberRank.None,
                        UID = member.UID
                    });



            }
            Member removed;
            Members.TryRemove(member.UID, out removed);
            Info.MembersCount = (uint)Members.Count;


            if (member.UnionMem != null)
            {
                var union = GetUnion;
                if (union != null)
                    union.AddOtherMember(stream, member.UnionMem);
            }

        }
        public static bool AllowToCreate(string Name)
        {
            if (!Program.NameStrCheck(Name))
                return false;
            foreach (Guild guil in GuildPoll.Values)
                if (guil.GuildName == Name)
                    return false;

            return true;
        }
        public bool UseAdvertise = false;
        public override string ToString()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            return writer.Add(Info.GuildID).Add(GuildName).Add(Info.LeaderName).Add(Info.SilverFund).Add(Info.ConquerPointFund)
                  .Add(Info.CreateTime).Add(Bulletin).Add((byte)(UseAdvertise ? 1 : 0)).Add(BuletinEnrole).Add(UnionID).Close();
        }
        public class Arsenal
        {
            public const byte
                None = 0,
                Armor = 1,
                Weapon = 2,
                Ring = 3,
                Boots = 4,
                Necklace = 5,
                Fan = 6,
                Tower = 7,
                Headgear = 8;

            public static byte GetArsenalPosition(uint ID)
            {
                if ((ID >= 111003 && ID <= 118309) || (ID >= 123000 && ID <= 123309) || (ID >= 141003 && ID <= 145309)
                    || ID >= 170000 && ID <= 170309)
                    return Headgear;
                else if (ID >= 120001 && ID <= 121269)
                    return Necklace;
                else if (ID >= 130003 && ID <= 139309 || ID >= 101000 && ID <= 101309)
                    return Armor;
                else if (ID >= 150000 && ID <= 152279)
                    return Ring;
                else if (ID >= 160013 && ID <= 160249)
                    return Boots;
                else if (ID >= 201003 && ID <= 201009)
                    return Fan;
                else if (ID >= 202003 && ID <= 202009)
                    return Tower;
                else if (ID >= 410003 && ID <= 617439 || Database.ItemType.IsNinjaEpicWeapon(ID) || Database.ItemType.IsTrojanEpicWeapon(ID) || Database.ItemType.IsMonkEpicWeapon(ID)
                    || Database.ItemType.IsWarriorEpicWeapons(ID) || Database.ItemType.IsWindWalkerWeapon(ID)) 
                    return Weapon;
                else if ((ID >= 900000 && ID <= 900309) || (ID >= 1050000 && ID <= 1051000) || (ID >= 612000 && ID <= 612439
                    || (ID >= 612000 && ID <= 613429)
                    || (ID >= 611000 && ID <= 611439)))
                    return Weapon;
                return 0;
            }
            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                foreach (var ars in Pool_Arsenals)
                    writer.Add(ars.Key).Add(ars.Value.Enchant);
                return writer.Close();
            }
            public void Load(string line)
            {
                Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
                for (int x = 0; x < reader.GetCount(); x++)
                    Pool_Arsenals.TryAdd(reader.Read((byte)0), new Arsenals() { Enchant = reader.Read((byte)0) });

            }
            private System.Collections.Concurrent.ConcurrentDictionary<byte, Arsenals> Pool_Arsenals = new System.Collections.Concurrent.ConcurrentDictionary<byte, Arsenals>();

            public void CheckLoad()
            {
                foreach (var ars in Pool_Arsenals.Values)
                    ars.CreateArsenalPotency();
            }
            public ICollection<Arsenals> GetCllection()
            {
                return Pool_Arsenals.Values.ToArray();
            }
            public byte GetGuildLevel
            {
                get
                {
                    int val = 1;
                    foreach (var arsen in Pool_Arsenals.Values)
                    {
                        if (arsen.GetPotency == 3)
                            val += 2;
                        else if(arsen.GetPotency == 2)
                            val += 1;
                    }

                    return (byte)Math.Min(val, 9);
                }
            }

            public byte Unlockers() { return (byte)Pool_Arsenals.Count; }
            public void Unlock(byte typ)
            {
                if (IsUnlock(typ))
                    return;
                Pool_Arsenals.TryAdd(typ, new Arsenals());
            }

            public ulong GetTypDonation(byte typ)
            {
                if (!IsUnlock(typ))
                    return 0;
                return Pool_Arsenals[typ].GetDonation;
            }
            public ulong GetTypPotency(byte typ)
            {
                if (!IsUnlock(typ))
                    return 0;
                return Pool_Arsenals[typ].GetPotency;
            }

            public uint GetFullBp()
            {
                uint bp = 0;
                var array = Pool_Arsenals.Values.OrderByDescending(p => p.GetPotency).ToArray();
                for (byte x = 0; x < array.Length; x++)
                {
                    if (x == 5)
                        break;
                    bp += (uint)array[x].GetPotency;
                }// foreach (var arsen in Pool_Arsenals.Values)
               //     bp += (byte)arsen.GetPotency;
                return bp;
            }
            public bool IsUnlock(byte typ)
            {
                return Pool_Arsenals.ContainsKey(typ);
            }
            public bool Add(byte typ, InscribeItem item)
            {
                if (!IsUnlock(typ))
                    return false;
                return Pool_Arsenals[typ].Add(item);
            }
            public bool Remove(byte typ, uint UID)
            {
                if (!IsUnlock(typ))
                    return false;
                return Pool_Arsenals[typ].Remove(UID);
            }
            public bool TryGetArsenal(byte typ, uint UID, out InscribeItem item)
            {
                if (!IsUnlock(typ))
                {
                    item = null;
                    return false;
                }
                return Pool_Arsenals[typ].TryGetValue(UID, out item);
            }
            public Arsenals GetArsenal(byte typ)
            {
                if (!IsUnlock(typ))
                    return null;
                return Pool_Arsenals[typ];
            }

            public class InscribeItem
            {
                public Game.MsgServer.MsgGameItem BaseItem;
                public string Name = "";
                public uint UID;
            }
            public class Arsenals
            {
                public bool IsDone
                {
                    get
                    {
                        return GetPotency >= 2;
                 
                    }
                }
                public ulong GetPotency = 0;
                public ulong GetDonation = 0;
             
               public byte Enchant;

                public InscribeItem[] DescreasedItems = new InscribeItem[0];
                private System.Collections.Concurrent.ConcurrentDictionary<uint, InscribeItem> Items = new System.Collections.Concurrent.ConcurrentDictionary<uint, InscribeItem>();

              
                public void RemoveAllClientItem(uint uid)
                {
                    var dictionar = Items.Values.ToArray();
                    lock (dictionar)
                    {
                        foreach (var client in dictionar)
                        {
                            // Items are keyed by item UID; remove every item owned by this player.
                            if (client.UID == uid && client.BaseItem != null)
                                Remove(client.BaseItem.UID);
                        }
                    }
                }
                public bool Add(InscribeItem item)
                {
                    bool accept = Items.TryAdd(item.BaseItem.UID, item);
                    if (accept)
                    {
                        GetDonation += GetItemDonation(item.BaseItem);
                        CreateArsenalPotency();
                        CalculateRank();
                    }
                    return accept;
                }
                public bool Remove(uint UID)
                {
                    InscribeItem ite = null;
                    if (Items.TryRemove(UID, out ite))
                    {
                        GetDonation -= GetItemDonation(ite.BaseItem);
                        CalculateRank();
                        CreateArsenalPotency();
                    }
                    return ite != null;
                }
                public bool TryGetValue(uint UID, out InscribeItem item)
                {
                    return Items.TryGetValue(UID, out item);
                }
                public void CalculateRank()
                {
                    lock (DescreasedItems)
                    {
                        InscribeItem[] data = Items.Values.ToArray();

                        InscribeItem[] rnk = (from item in data
                                              orderby GetItemDonation(item.BaseItem) descending
                                              select item).ToArray();

                        DescreasedItems = rnk;
                    }
                }
                public void CreateArsenalPotency()
                {
                    if (GetDonation >= (uint)(1362330 * 2))
                        //5582640
                        //5449320
                        GetPotency = 1;
                    if (GetDonation >= (uint)(1362330 * 3))//7 items  +12
                        GetPotency = 2;
                    if (GetDonation >= (uint)(1362330 * 4))//9 item  +12
                        GetPotency = 3;
                    if (GetPotency < 3)
                    {
                        if (Enchant > 0)
                        {
                            if (Enchant > GetPotency)
                                GetPotency = Enchant;
                        }
                    }
                }
                public uint GetItemDonation(Game.MsgServer.MsgGameItem Item)//1395660 on full item
                {
                    uint Return = 0;
                    int id = (int)(Item.ITEM_ID % 10);
                    switch (id)
                    {
                        case 8: Return = 1000; break;
                        case 9: Return = 16660; break;
                    }
                    if (Item.SocketOne > 0 && Item.SocketTwo == 0)
                        Return += 33330;
                    if (Item.SocketOne > 0 && Item.SocketTwo > 0)
                        Return += 133330;

                    switch (Item.Plus)
                    {
                        case 1: Return += 90; break;
                        case 2: Return += 490; break;
                        case 3: Return += 1350; break;
                        case 4: Return += 4070; break;
                        case 5: Return += 12340; break;
                        case 6: Return += 37030; break;
                        case 7: Return += 111110; break;
                        case 8: Return += 333330; break;
                        case 9: Return += 1000000; break;
                        case 10: Return += 1033330; break;
                        case 11: Return += 1101230; break;
                        case 12: Return += 1212340; break;
                        default: break;
                    }

                    return Return;
                }
            }
        }
    }
}
