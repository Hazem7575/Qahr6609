using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public static class MiningTable
    {
        private static MineRule[] MineRules = new MineRule[0];
        private static ThreadSafeRandom Random = new ThreadSafeRandom();
        public static void Load()
        {
            if (!File.Exists(Program.ServerConfig.DbLocation + "MiningRules.ini"))
            {
                CreateRule();
                SaveToFile();
                return;
            }
            WindowsAPI.IniFile Reader = new WindowsAPI.IniFile("MiningRules.ini");
            int Count = Reader.ReadInt32("MinesAmount", "Amount", 0);
            MineRules = new MineRule[Count];
            for (int x = 0; x < Count; x++)
            {
                MineRule Rule = new MineRule()
                {
                    ID = Reader.ReadUInt32("MineRules", "id" + x.ToString(), 0),
                    RuleType = (MineRuleType)Reader.ReadUInt32("MineRules", "RuleType" + x.ToString(), 0),
                    RuleChance = Reader.ReadDouble("MineRules", "RuleChance" + x.ToString(), 0),
                    RuleAmount = Reader.ReadInt32("MineRules", "RuleAmount" + x.ToString(), 0),
                    RuleValue = Reader.ReadUInt32("MineRules", "RuleValue" + x.ToString(), 0)
                };
                MineRules[x] = Rule;
            }
        }
        private static void CreateRule()
        {
            MineRules = new MineRule[54];
            uint Indexer = 0;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 8, RuleValue = 1072010 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.9, RuleValue = 1072011 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.8, RuleValue = 1072012 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.7, RuleValue = 1072013 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.6, RuleValue = 1072014 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.5, RuleValue = 1072015 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.4, RuleValue = 1072016 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.3, RuleValue = 1072017 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.2, RuleValue = 1072018 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 2.1, RuleValue = 1072019 });
            Indexer++;
            ///
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 1.0, RuleValue = 1072020 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.9, RuleValue = 1072021 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.8, RuleValue = 1072022 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.7, RuleValue = 1072023 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.6, RuleValue = 1072024 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.5, RuleValue = 1072025 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.4, RuleValue = 1072026 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.3, RuleValue = 1072027 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.2, RuleValue = 1072028 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.1, RuleValue = 1072029 });
            Indexer++;
            ///
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 1.0, RuleValue = 1072040 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.9, RuleValue = 1072041 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.8, RuleValue = 1072042 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.7, RuleValue = 1072043 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.6, RuleValue = 1072044 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.5, RuleValue = 1072045 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.4, RuleValue = 1072046 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.3, RuleValue = 1072047 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.2, RuleValue = 1072048 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.1, RuleValue = 1072049 });
            Indexer++;
            ///
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 1.0, RuleValue = 1072050 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.9, RuleValue = 1072051 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.8, RuleValue = 1072052 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.7, RuleValue = 1072053 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.6, RuleValue = 1072054 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.5, RuleValue = 1072055 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.4, RuleValue = 1072056 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.3, RuleValue = 1072057 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.2, RuleValue = 1072058 });
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.1, RuleValue = 1072059 });
            Indexer++;
            /// Special Ores
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.9, RuleValue = 1080001 });//Emerlad
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineOre, RuleAmount = 1, RuleChance = 0.8, RuleValue = 1072031 });//EuxeniteOre
            Indexer++;
            /// Special Gems
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineGem, RuleAmount = 1, RuleChance = 1.3, RuleValue = 700001 });//PhoenixGem
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineGem, RuleAmount = 1, RuleChance = 1.3, RuleValue = 700011 });//DragonGem
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineGem, RuleAmount = 1, RuleChance = 1.3, RuleValue = 700021 });//FuryGem
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineGem, RuleAmount = 1, RuleChance = 1.3, RuleValue = 700031 });//RainbowGem
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineGem, RuleAmount = 1, RuleChance = 1.3, RuleValue = 700041 });//KylinGem
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineGem, RuleAmount = 1, RuleChance = 1.3, RuleValue = 700051 });//VioletGem
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineGem, RuleAmount = 1, RuleChance = 1.3, RuleValue = 700061 });//MoonGem
            Indexer++;
            ///Misc
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineMisc, RuleAmount = 1, RuleChance = 0.3, RuleValue = 1088002 });//MeteorTear
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineMisc, RuleAmount = 1, RuleChance = 0.4, RuleValue = 1088001 });//Meteor
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineMisc, RuleAmount = 1, RuleChance = 0.1, RuleValue = 1088000 });//DragonBall
            Indexer++;
            ///Stones
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineStone, RuleAmount = 1, RuleChance = 0.2, RuleValue = 730001 });//Stone+1
            Indexer++;
            MineRules[Indexer] = (new MineRule() { ID = Indexer, RuleType = MineRuleType.MineStone, RuleAmount = 1, RuleChance = 0.1, RuleValue = 730002 });//Stone+2
        }
        private static void SaveToFile()
        {
            if (!File.Exists(Program.ServerConfig.DbLocation + "MiningRules.ini"))
            {
                File.Create(Program.ServerConfig.DbLocation + "MiningRules.ini").Dispose();
                using (var writer = new StreamWriter(Program.ServerConfig.DbLocation + "MiningRules.ini", true))
                {
                    writer.WriteLine("[MinesAmount] \n");
                    writer.WriteLine("Amount=0 \n");
                    writer.WriteLine("[MineRules] \n");
                    writer.Flush();
                    writer.Close();
                }
            }
            WindowsAPI.IniFile Writer = new WindowsAPI.IniFile("MiningRules.ini");
            Writer.Write<int>("MinesAmount", "Amount", MineRules.Length);
            for (int x = 0; x < MineRules.Length; x++)
            {
                MineRule currentrole = MineRules[x];
                Writer.Write<uint>("MineRules", "id" + x.ToString(), currentrole.ID);
                Writer.Write<uint>("MineRules", "RuleType" + x.ToString(), (uint)currentrole.RuleType);
                Writer.Write<double>("MineRules", "RuleChance" + x.ToString(), currentrole.RuleChance);
                Writer.Write<int>("MineRules", "RuleAmount" + x.ToString(), currentrole.RuleAmount);
                Writer.Write<uint>("MineRules", "RuleValue" + x.ToString(), currentrole.RuleValue);
            }
        }
        public static bool GetRandomRole(out MineRule Role)
        {
            Role = null;
            foreach (MineRule role in MineRules)
            {
                if (PercentSuccess(role.RuleChance))
                {
                    Role = role;
                    break;
                }
            }
            if(Role == null)
            {
                Role = MineRules[Program.GetRandom.Next(0, 10)];
            }
            return Role != null;
        }
        public static bool PercentSuccess(double _chance)
        {
            return Random.NextDouble() * 100 < _chance;
        }
    }
    public class MineRule
    {
        public uint ID { get; set; }
        public MineRuleType RuleType { get; set; }
        public double RuleChance { get; set; }
        public int RuleAmount { get; set; }
        public uint RuleValue { get; set; }
    }
    [Flags]
    public enum MineRuleType : uint
    {
        MineOre = 1,
        MineGem = 2,
        MineMisc = 3,
        MineStone = 4,
    }
}
