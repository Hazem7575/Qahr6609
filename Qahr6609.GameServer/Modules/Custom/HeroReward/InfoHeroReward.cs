using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class InfoHeroReward : Extensions.SafeDictionary<uint, InfoHeroReward.StageGoal>
    {
        public class StageGoal
        {
            public uint UID = 0;
            public uint OpenLev = 0;
            public uint CountGoals = 0;
            public string Prize = "";

            public Extensions.SafeDictionary<uint, Item> ArrayItem = new Extensions.SafeDictionary<uint, Item>();

            public bool CheckOpen(ushort level, byte reborn)
            {
                // OpenLev = reborn * 1000 + level (e.g. 1015 = 1st reborn level 15); a higher reborn always
                // counts as higher, so a reborn character keeps the stages of the lower tiers open.
                return (uint)reborn * 1000 + level >= OpenLev;
            }
            public uint GetPrize()
            {
                return uint.Parse(Prize.Split(' ')[0]);
            }
            public byte GetCount()
            {
                return byte.Parse(Prize.Split(' ')[1]);
            }
            // StageGoal.txt prize format: "itemtype amount gift" - gift 1 = bound (gift) item, 0 = normal item
            public bool GetBound()
            {
                return PrizeBound(Prize);
            }
            public static bool PrizeBound(string prize)
            {
                var parts = prize.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                    return true;// old/short data: keep the previous behaviour (bound)
                return parts[2].Trim() != "0";
            }

            public class Item
            {
                public uint UID;
                public string Prize = "";
                public byte Progres;

                public uint GetPrize()
                {
                    return uint.Parse(Prize.Split(' ')[0]);
                }
                public byte GetCount()
                {
                    return byte.Parse(Prize.Split(' ')[1]);
                }
                public bool GetBound()
                {
                    return StageGoal.PrizeBound(Prize);
                }
            }
        }

        internal void LoadInformations()
        {
            WindowsAPI.IniFile Reader = new WindowsAPI.IniFile("StageGoal.txt");

            int StageCount = Reader.ReadByte("StageGoal", "StageAmount", 0);

            const ushort FileSize = 100;

            for (uint x = 1; x <= StageCount; x++)
            {
                var Stage = new StageGoal()
                {
                    UID = x,
                    OpenLev = Reader.ReadUInt32(x.ToString(), "OpenLev", 0),
                    CountGoals = Reader.ReadByte(x.ToString(), "GoalNum", 0),
                    Prize = Reader.ReadString(x.ToString(), "Prize1","")
                };

                this.Add(x, Stage);

                for (uint i = 1; i <= Stage.CountGoals; i++)
                {
                    uint Key = x * FileSize + i;
                    string section = x.ToString() + "-" + i.ToString();
                    Stage.ArrayItem.Add(Key, new StageGoal.Item()
                     {
                         UID = Key,
                         Progres = Reader.ReadByte(section, "Progress", 0),
                         Prize = Reader.ReadString(section, "Prize1", "")
                     });
                }
            }
        }
    }
}
