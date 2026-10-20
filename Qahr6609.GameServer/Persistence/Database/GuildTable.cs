using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class GuildTable
    {
        //save ----------------------
        internal static void Save()
        {
            foreach (var obj in Role.Instance.Guild.GuildPoll)
            {
                if (obj.Value.CanSave == false)
                    continue;
                var guild = obj.Value;
                using (DBActions.Write writer = new DBActions.Write("Guilds\\" + obj.Key + ".txt"))
                {
                    writer.SkipIfUnchanged = true;// unchanged guilds are not rewritten (content compared with the last write)
                    writer.Add(guild.ToString()).Add(guild.Recruit.ToString())
                        .Add(guild.AdvertiseRecruit.ToString()).Add(ToStringAlly(guild)).Add(ToStringEnemy(guild))
                        .Add(guild.MyArsenal.ToString())
                        .Add(guild.CTF_Exploits.ToString())
                        .Add(guild.CTF_Next_ConquerPoints.ToString())
                        .Add(guild.CTF_Next_Money.ToString())
                        .Add(guild.CTF_Rank.ToString())
                        .Add(guild.ClaimCtfReward.ToString());
                    writer.Execute(DBActions.Mode.Open);
                }
            }
        }
        public static string ToStringAlly(Role.Instance.Guild guild)
        {
            DBActions.WriteLine writer = new DBActions.WriteLine('/');
            writer.Add(guild.Ally.Count);
            foreach (Role.Instance.Guild ally in guild.Ally.Values)
                writer.Add(ally.Info.GuildID);
            return writer.Close();
        }
        public static string ToStringEnemy(Role.Instance.Guild guild)
        {
            DBActions.WriteLine writer = new DBActions.WriteLine('/');
            writer.Add(guild.Enemy.Count);
            foreach (Role.Instance.Guild enemy in guild.Enemy.Values)
                writer.Add(enemy.Info.GuildID);
            return writer.Close();
        }
        //------------------------


        internal static void Load()
        {
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Guilds\\"))
            {
                // Only live guild files: skip N.txt.bak / N.txt.tmp / N.txt.corrupt-* left by atomic saves.
                if (!string.Equals(System.IO.Path.GetExtension(fname), ".txt", StringComparison.OrdinalIgnoreCase))
                    continue;
                using (DBActions.Read reader = new DBActions.Read(fname, true))
                {
                    if (reader.Reader())
                    {
                        //--------- guild info ------------------
                        DBActions.ReadLine GuildReader = new DBActions.ReadLine(reader.ReadString("0/"), '/');
                        uint ID = GuildReader.Read((uint)0);
                        if (ID > 100000)
                            continue;
                        if (ID > Role.Instance.Guild.Counter.Count)
                            Role.Instance.Guild.Counter.Set(ID);
                        Role.Instance.Guild guild = new Role.Instance.Guild(null, GuildReader.Read("None"),null);
                        guild.Info.GuildID = ID;
                        guild.Info.LeaderName = GuildReader.Read("None");
                        guild.Info.SilverFund = GuildReader.Read((long)0);
                        guild.Info.ConquerPointFund = GuildReader.Read((uint)0);
                        guild.Info.CreateTime = GuildReader.Read((uint)0);
                        guild.Bulletin = GuildReader.Read("None");
                        guild.UseAdvertise = GuildReader.Read((byte)0) == 1;
                        guild.BuletinEnrole = GuildReader.Read((int)0);
                        guild.UnionID = GuildReader.Read((uint)0);

                        //----------------------------------

                        //----------load requit and advertise ----------------
                        guild.Recruit.Load(reader.ReadString("0/"));
                        guild.AdvertiseRecruit.Load(reader.ReadString("0/"));
                        //----------------------------------------------------

                        //---------load ally ---------------------
                        LoadGuildAlly(ID, reader.ReadString("0/"));
                        //-----------------------------------

                        //---------load enemy --------------------
                        LoadGuildEnemy(ID, reader.ReadString("0/"));
                        //----------------------------------------

                        //---------load arsenals ------------------
                        guild.MyArsenal.Load(reader.ReadString("0/"));
                        //-----------------------------------------

                        guild.CTF_Exploits = reader.ReadUInt32(0);
                        guild.CTF_Next_ConquerPoints = reader.ReadUInt32(0);
                        guild.CTF_Next_Money = reader.ReadUInt32(0);
                        guild.CTF_Rank = reader.ReadUInt32(0);
                        guild.ClaimCtfReward = reader.ReadUInt32(0);
                        if (guild.UseAdvertise)
                            Role.Instance.Guild.Advertise.Add(guild);
                        if (!Role.Instance.Guild.GuildPoll.ContainsKey(guild.Info.GuildID))
                            Role.Instance.Guild.GuildPoll.TryAdd(guild.Info.GuildID, guild);
                        guild.MyArsenal.CheckLoad();


                        if (guild.UnionID != 0)
                        {
                            Role.Instance.Union union;
                            if (Role.Instance.Union.UnionPoll.TryGetValue(guild.UnionID, out union))
                            {
                                if (!union.Guilds.ContainsKey(guild.Info.GuildID))
                                    union.Guilds.TryAdd(guild.Info.GuildID, guild);
                            }
                        }
                    }
                }
            }
            ExecuteAllyAndEnemy();
            LoadMembers();
            LoadArsenals();
            foreach (var guilds in Role.Instance.Guild.GuildPoll.Values)
            {
                guilds.CreateMembersRank();
                guilds.UpdateGuildInfo();
            }
            enemy.Clear();
            ally.Clear();



            MyConsole.WriteLine("Loaging " + Role.Instance.Guild.GuildPoll.Count + " Guilds ! ");
            // [FastBoot, from Qahr] was GC.Collect(): a full blocking GC of every heap on each loader; the runtime collects on its own
        }
        private unsafe static void LoadArsenals()
        {
            foreach (var guild in Role.Instance.Guild.GuildPoll.Values)
            {
                foreach (var member in guild.Members.Values)
                {
                    WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                    if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + member.UID + ".bin", System.IO.FileMode.Open))
                    {
                        ClientItems.DBItem Item;
                        int ItemCount;
                        binary.Read(&ItemCount, sizeof(int));
                        for (int x = 0; x < ItemCount; x++)
                        {
                            binary.Read(&Item, sizeof(ClientItems.DBItem));
                            Game.MsgServer.MsgGameItem ClienItem = Item.GetDataItem();
                            if (Item.DepositeCount != 0)
                            {
                                uint DepositeCount = Item.DepositeCount;
                                for (int i = 0; i < DepositeCount; i++)
                                {
                                    binary.Read(&Item, sizeof(ClientItems.DBItem));
                                    Game.MsgServer.MsgGameItem DepositeItem = Item.GetDataItem();
                                    if (DepositeItem.Inscribed == 1)
                                    {
                                        guild.MyArsenal.Add(Role.Instance.Guild.Arsenal.GetArsenalPosition(DepositeItem.ITEM_ID)
                                            , new Role.Instance.Guild.Arsenal.InscribeItem() { BaseItem = DepositeItem, Name = member.Name, UID = member.UID });
                                        member.ArsenalDonation += GetItemDonation(DepositeItem);
                                    }
                                }
                            }
                            else
                            {
                                if (ClienItem.Inscribed == 1)
                                {
                                    guild.MyArsenal.Add(Role.Instance.Guild.Arsenal.GetArsenalPosition(ClienItem.ITEM_ID)
                                        , new Role.Instance.Guild.Arsenal.InscribeItem() { BaseItem = ClienItem, Name = member.Name, UID = member.UID });
                                    member.ArsenalDonation += GetItemDonation(ClienItem);
                                }
                            }

                        }
                        binary.Close();
                    }
                }
            }
        }
        private static uint GetItemDonation(Game.MsgServer.MsgGameItem Item)//1395660 on full item
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
        private static void LoadMembers()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;

                uint UID = ini.ReadUInt32("Character", "UID", 0);
                string Name = ini.ReadString("Character", "Name", "None");
                uint GuildID = ini.ReadUInt32("Character", "GuildID", 0);
                if (GuildID != 0)
                {
                    Role.Instance.Guild Guild;
                    if (Role.Instance.Guild.GuildPoll.TryGetValue(GuildID, out Guild))
                    {
                        ushort Body = ini.ReadUInt16("Character", "Body", 1002);
                        ushort Face = ini.ReadUInt16("Character", "Face", 0);

                        Role.Instance.Guild.Member member = new Role.Instance.Guild.Member();
                        member.UID = UID;
                        member.Mesh = (uint)(Face * 10000 + Body);
                        member.Name = Name;
                        member.Rank = (Role.Flags.GuildMemberRank)ini.ReadUInt32("Character", "GuildRank", 200);
                        member.Class = ini.ReadByte("Character", "Class", 0);
                        member.CpsDonate = ini.ReadUInt32("Character", "CpsDonate", 0);
                        member.MoneyDonate = ini.ReadInt64("Character", "MoneyDonate", 0);
                        member.PkDonation = ini.ReadUInt32("Character", "PkDonation", 0);
                        member.LastLogin = ini.ReadInt64("Character", "LastLogin", 0);
                        member.Level = ini.ReadUInt16("Character", "Level", 0);
                        member.PrestigePoints = ini.ReadUInt16("Character", "PrestigePoints", 0);
                        //------------------------------- CTF--------------
                        member.CTF_Exploits = ini.ReadUInt32("Character", "CTF_Exploits", 0);
                        member.RewardConquerPoints = ini.ReadUInt32("Character", "CTF_RCPS", 0);
                        member.RewardMoney = ini.ReadUInt32("Character", "CTF_RM", 0);
                        member.CTF_Claimed = ini.ReadByte("Character", "CTF_R", 0);
                        //-----------------------------------------------

                       
                        Role.Instance.Flowers flower;
                        if (Role.Instance.Flowers.ClientPoll.TryGetValue(UID, out flower))
                        {
                            member.Lilies = flower.Lilies;
                            member.Orchids = flower.Orchids;
                            member.Rouses = flower.RedRoses;
                            member.Tulips = flower.Tulips;
                        }
                        ulong nobilitydonation = ini.ReadUInt64("Character", "DonationNobility", 0);
                        Role.Instance.Nobility nobility;
                        if (Program.NobilityRanking.TryGetValue(UID, out nobility))
                        {
                            member.NobilityRank = (uint)nobility.Rank;
                        }
                        else
                        {
                            if (nobilitydonation >= 200000000)
                                member.NobilityRank = (uint)Role.Instance.Nobility.NobilityRank.Earl;
                            else if (nobilitydonation >= 100000000)
                                member.NobilityRank = (uint)Role.Instance.Nobility.NobilityRank.Baron;
                            else if (nobilitydonation >= 30000000)
                                member.NobilityRank = (uint)Role.Instance.Nobility.NobilityRank.Knight;
                        }
                        if (!Guild.Members.ContainsKey(member.UID))
                            Guild.Members.TryAdd(member.UID, member);

                        //---------------------union info------------------
                        if (Guild.UnionID != 0)
                        {
                            var union = Guild.GetUnion;
                            if (union != null)
                            {
                                uint UnionRank = ini.ReadUInt32("Character", "UnionRank", 0);
                                member.UnionMem = Role.Instance.Union.Member.CreateMember(member, Role.Instance.Union.LoadedRank(union, UID, UnionRank));
                                member.UnionMem.UnionID = union.UID;
                                member.UnionMem.Exploits = ini.ReadUInt32("Character", "UnionExploits", 0);
                                member.UnionMem.MyTreasury = ini.ReadUInt32("Character", "Treasury", 0);
                                member.UnionMem.AllowanceDay = ini.ReadUInt32("Character", "UnionAllowanceDay", 0);
                                member.UnionMem.StipendClaimed = ini.ReadUInt32("Character", "UnionStipend", 0); member.UnionMem.RememberClaims();
                                union.UpdataDBRank(member.UnionMem, member.UnionMem.Rank);
                            }
                        }
                        //-------------------------------------------------
                    }
                }
                else
                {
                    uint UnionUID = ini.ReadUInt32("Character", "UnionUID", 0);
                    if (UnionUID != 0)
                    {
                        Role.Instance.Union union;
                        if (Role.Instance.Union.UnionPoll.TryGetValue(UnionUID, out union))
                        {
                    
                            Role.Instance.Union.Member UnionMember = new Role.Instance.Union.Member();
                            ushort Body = ini.ReadUInt16("Character", "Body", 1002);
                            ushort Face = ini.ReadUInt16("Character", "Face", 0);

                            UnionMember.UID = UID;
                            UnionMember.Mesh = (uint)(Face * 10000 + Body);
                            UnionMember.Name = Name;
                            UnionMember.Class = ini.ReadByte("Character", "Class", 0);
                            UnionMember.Level = ini.ReadUInt16("Character", "Level", 0);

                            UnionMember.UnionID = union.UID;
                            UnionMember.Rank = Role.Instance.Union.LoadedRank(union, UID, ini.ReadUInt32("Character", "UnionRank", 0));

                            ulong nobilitydonation = ini.ReadUInt64("Character", "DonationNobility", 0);
                            Role.Instance.Nobility nobility;
                            if (Program.NobilityRanking.TryGetValue(UID, out nobility))
                            {
                                UnionMember.NobilityRank = nobility.Rank;
                            }
                            else
                            {
                                if (nobilitydonation >= 200000000)
                                    UnionMember.NobilityRank = Role.Instance.Nobility.NobilityRank.Earl;
                                else if (nobilitydonation >= 100000000)
                                    UnionMember.NobilityRank =Role.Instance.Nobility.NobilityRank.Baron;
                                else if (nobilitydonation >= 30000000)
                                    UnionMember.NobilityRank = Role.Instance.Nobility.NobilityRank.Knight;
                            }

                            UnionMember.Exploits = ini.ReadUInt32("Character", "UnionExploits", 0);
                            UnionMember.MyTreasury = ini.ReadUInt32("Character", "Treasury", 0);
                            UnionMember.AllowanceDay = ini.ReadUInt32("Character", "UnionAllowanceDay", 0);
                            UnionMember.StipendClaimed = ini.ReadUInt32("Character", "UnionStipend", 0); UnionMember.RememberClaims();
                            union.UpdataDBRank(UnionMember, UnionMember.Rank);
                            if (UnionMember.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                            {
                                if (union.EmperrorUID != UnionMember.UID)
                                    UnionMember.Rank = Role.Instance.Union.Member.MilitaryRanks.Member;
                            }
                            if (!union.Members.ContainsKey(UnionMember.UID))
                                union.Members.TryAdd(UnionMember.UID, UnionMember);
                        }
                    }
                }
            }
        }
        public static void ExecuteAllyAndEnemy()
        {
            foreach (var obj in ally)
            {
                foreach (var guild in obj.Value)
                {
                    Role.Instance.Guild alyguild;
                    if (Role.Instance.Guild.GuildPoll.TryGetValue(guild, out alyguild))
                    {
                        if (Role.Instance.Guild.GuildPoll.ContainsKey(obj.Key))
                        {
                            Role.Instance.Guild.GuildPoll[obj.Key].Ally.TryAdd(alyguild.Info.GuildID, alyguild);
                        }
                    }
                }
            }
            foreach (var obj in enemy)
            {
                foreach (var guild in obj.Value)
                {
                    Role.Instance.Guild alyenemy;
                    if (Role.Instance.Guild.GuildPoll.TryGetValue(guild, out alyenemy))
                    {
                        if (Role.Instance.Guild.GuildPoll.ContainsKey(obj.Key))
                        {
                            Role.Instance.Guild.GuildPoll[obj.Key].Enemy.TryAdd(alyenemy.Info.GuildID, alyenemy);
                        }
                    }
                }
            }
        }
        public static Dictionary<uint, List<uint>> ally = new Dictionary<uint, List<uint>>();
        public static Dictionary<uint, List<uint>> enemy = new Dictionary<uint, List<uint>>();
        public static void LoadGuildAlly(uint id, string line)
        {
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, '/');
            int count = reader.Read(0);
            for (int x = 0; x < count; x++)
            {
                if (ally.ContainsKey(id))
                    ally[id].Add(reader.Read((uint)0));
                else
                    ally.Add(id, new List<uint>() { reader.Read((uint)0) });
            }
        }
        public static void LoadGuildEnemy(uint id, string line)
        {
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, '/');
            int count = reader.Read(0);
            for (int x = 0; x < count; x++)
            {
                if (enemy.ContainsKey(id))
                    enemy[id].Add(reader.Read((uint)0));
                else
                    enemy.Add(id, new List<uint>() { reader.Read((uint)0) });
            }
        }
    }
}
