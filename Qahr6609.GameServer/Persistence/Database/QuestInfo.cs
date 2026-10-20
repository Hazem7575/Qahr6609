using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class QuestInfo
    {
        public class DBQuest
        {
            public class NpcInfo
            {
                public uint ID;
                public ushort Map;
                public ushort X;
                public ushort Y;
                public string Name;
                public string MapName;

                public NpcInfo(string line)
                {
                    if (line == "")
                        return;
                    try
                    {
                        string[] data = line.Split(',');
                        ID = uint.Parse(data[0]);
                        Map = ushort.Parse(data[1]);
                        X = ushort.Parse(data[2]);
                        Y = ushort.Parse(data[3]);
                        Name = data[4];
                        MapName = data[5];
                    }
                    catch { 
                    }
                }
            }
            public byte TypeId = 0;
            public byte CompleteFlag = 0;
            public uint ActivityType = 0;
            public ushort MissionId = 0;
            public string Name = "";
            public ushort Lv_min;
            public ushort Lv_max;
            public ushort First;
           // public ushort Prequest;
            public ushort Map;
            public byte[] Profession;
            public ulong Exp = 0;
            public ulong Gold = 0;
            public byte Sex;
            public uint FinishTime;
            public uint ActivityBeginTime;
            public uint ActivityEndTime;
            public NpcInfo BeginNpcId;
            public NpcInfo FinishNpcId;
            public uint Intentions;
            public uint[] Prize;
            public ushort[] Prequest;
            public bool CanAcceptQuest(byte Class)
            {
                if (Profession.Length == 0 || Profession.Length == 1)
                    return true;
                for (int x = 0; x < Profession.Length; x++)
                    if (Class == Profession[x])
                        return true;
                return false;
            }

        }
        public class ListNpcQuests
        {
            public List<DBQuest> Quests = new List<DBQuest>();
        }
        public static Dictionary<uint, DBQuest> AllQuests = new Dictionary<uint, DBQuest>();
        public static Dictionary<uint, ListNpcQuests> NpcQuests = new Dictionary<uint, ListNpcQuests>();

        public static DBQuest GetFinishQuest(uint NPCID, byte _class, uint QuestID = 0, Func<DBQuest, bool> P = null)
        {
            ListNpcQuests array;
            if (!NpcQuests.TryGetValue(NPCID, out array))
                return null;
            for (int x = 0; x < array.Quests.Count; x++)
            {
                var quest = array.Quests[x];

                if (QuestID == 0)
                {
                    if (quest.CanAcceptQuest(_class))
                    {
                        if (P != null)
                        {
                            if (P(quest))
                                return quest;
                        }
                        else
                            return quest;
                    }
                }
                else if (quest.MissionId == QuestID)
                {
                    return quest;
                }
                   
            }
            return null;
        }
        public static List<uint> KingDomMissions = new List<uint>() { 35024, 35007, 35025, 35028, 35034 };
        public static bool IsKingDomMission(uint MissionID)
        {
            return KingDomMissions.Contains(MissionID);
              
        }


        public static void Init()
        {

            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Questinfo.ini");
            int TotalMission = reader.ReadUInt16("TotalMission", "TotalMission", 0);

            for (ushort count = 1; count < TotalMission; count++)
            {
                DBQuest quest = new DBQuest();
                quest.TypeId = reader.ReadByte(count.ToString(), "TypeId", 0);
                quest.CompleteFlag = reader.ReadByte(count.ToString(), "CompleteFlag", 0);
                quest.ActivityType = reader.ReadByte(count.ToString(), "ActivityType", 0);
                quest.MissionId = reader.ReadUInt16(count.ToString(), "MissionId", 0);
                quest.Intentions = reader.ReadUInt16(count.ToString(), "IntentAmount", 0);
                string Prequest = reader.ReadString(count.ToString(), "Prequest", "");
                if (Prequest != "")
                {
                        if (Prequest.Contains('|'))
                        {
                            var preq2 = Prequest.Split('|');
                            quest.Prequest = new ushort[preq2.Length];
                            for (int x = 0; x < preq2.Length; x++)
                                quest.Prequest[x] = ushort.Parse(preq2[x]);
                        }
                        else if (Prequest.Contains(','))
                        {
                            var preq2 = Prequest.Split(',');
                            quest.Prequest = new ushort[preq2.Length];
                            for (int x = 0; x < preq2.Length; x++)
                                quest.Prequest[x] = ushort.Parse(preq2[x]);
                        }
                    else
                            quest.Prequest = new ushort[1] { ushort.Parse(Prequest) };

                }
                else
                    quest.Prequest = new ushort[1] { 0 };

                string profesion = reader.ReadString(count.ToString(), "Profession", "");
                if (profesion != "")
                {
                    if (profesion.Contains(','))
                    {
                        var profesion2 = profesion.Split(',');
                        quest.Profession = new byte[profesion2.Length];
                        for (int x = 0; x < profesion2.Length; x++)
                            quest.Profession[x] = byte.Parse(profesion2[x]);
                    }
                    else
                        quest.Profession = new byte[1] { byte.Parse(profesion) };
                }
                else
                    quest.Profession = new byte[1] { 0 };

                string Prize = reader.ReadString(count.ToString(), "Prize", "");
                if (Prize.Contains('['))
                {
                    
                    string[] items = Prize.Split('[');
                    quest.Prize = new uint[items.Length - 1];
                    try
                    {
                        for (int x = 1; x < items.Length; x++)
                        {
                            var element = items[x];
                            var sp1 = element.Split(' ');
                            var sp2 = sp1[1].Split(',');
                            quest.Prize[x-1] = uint.Parse(sp2[0]);
                        }
                    }
                    catch
                    {

                    }
                }
                quest.BeginNpcId = new DBQuest.NpcInfo(reader.ReadString(count.ToString(), "BeginNpcId", ""));
                quest.FinishNpcId = new DBQuest.NpcInfo(reader.ReadString(count.ToString(), "FinishNpcId", ""));
                /*BeginNpcId=2001,1002,307,338,TCCaptain,TwinCity
FinishNpcId=0004,1002,284,266,Armorer,TwinCity*/
                if (!AllQuests.ContainsKey(quest.MissionId))
                    AllQuests.Add(quest.MissionId, quest);
               // else
               //     Console.WriteLine(quest.MissionId);

                if (NpcQuests.ContainsKey(quest.BeginNpcId.ID))
                {
                    NpcQuests[quest.BeginNpcId.ID].Quests.Add(quest);
                }
                else
                {
                    NpcQuests.Add(quest.BeginNpcId.ID, new ListNpcQuests());
                    NpcQuests[quest.BeginNpcId.ID].Quests.Add(quest);
                }

            }
          
        }
    }
}
