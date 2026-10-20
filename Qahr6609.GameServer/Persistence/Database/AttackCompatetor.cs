using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public static class AttackCompatetor
    {
        /// <summary>
        /// for saving any values or it's skill id.
        /// </summary>
        public static Dictionary<ushort, byte> Skills = new Dictionary<ushort, byte>();
        /// <summary>
        /// Loading Dictionary With Values From DB and sorting it.
        /// </summary>
        public static void Load()
        {
            if (!File.Exists(Program.ServerConfig.DbLocation + "\\Comatetor.ini"))
            {
                File.Create(Program.ServerConfig.DbLocation + "\\Comatetor.ini").Dispose();
            }
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Comatetor.ini");
            ushort Count = reader.ReadUInt16("System", "Count", 0);
            if (Count > 0)
            {
                for (ushort x = 0; x < Count; x++)
                {
                    ushort ID = reader.ReadUInt16("Skill" + x, "ID", 0);
                    byte Value = reader.ReadByte("Skill" + x, "Value", 0);
                    if (!Skills.ContainsKey(ID))
                    {
                        Skills.Add(ID, Value);
                    }
                }
            }
        }
        /// <summary>
        /// for inserting new percentage value.
        /// </summary>
        /// <param name="ID">ushort.MaxValue must be in care</param>
        /// <param name="Value">Byte.MaxValue must be in care</param>
        public static void Insert(ushort ID, byte Value)
        {
            if (Skills.ContainsKey(ID))
            {
                Skills[ID] = Value;
            }
            else
                Skills.Add(ID, Value);
        }
        /// <summary>
        /// for checking damage and query any changes.
        /// </summary>
        /// <param name="ID">ushort value paramerter is for spell id.</param>
        /// <param name="Damage">int value parameter using ref symbol to enable editing.</param>
        public static bool CheckDmg(ushort ID, out int Damage)
        {
            if (Skills.ContainsKey(ID))
            {
                Damage = Skills[ID];
                return true;
            }
            Damage = 0;
            return false;
        }
        /// <summary>
        /// for calculating the percentage (number * numerator) / denominator
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        private static int Percentage(int number, int numerator, int denominator)
        {
            return (number * numerator) / denominator;
        }
        /// <summary>
        /// saving all Dictionary values into DB file and if needed clearing values.
        /// </summary>
        public static void Save()
        {
            try
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "\\Comatetor.ini"))
                {
                    File.Create(Program.ServerConfig.DbLocation + "\\Comatetor.ini").Dispose();
                }
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Comatetor.ini");
                write.Write<ushort>("System", "Count", (ushort)Skills.Count);
                if (Skills.Count > 0)
                {
                    int z = 0;
                    foreach (var query in Skills)
                    {
                        write.Write<ushort>("Skill" + z, "ID", query.Key);
                        write.Write<byte>("Skill" + z, "Value", query.Value);
                        z += 1;
                    }
                }
            }
            catch
            {
                MyConsole.WriteLine("Error in saving attack compatetor.");
            }
        }
    }
}
