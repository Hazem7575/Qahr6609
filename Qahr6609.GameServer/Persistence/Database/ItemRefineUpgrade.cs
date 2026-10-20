using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database
{
    public class ItemRefineUpgrade
    {

        public static Dictionary<uint, uint> ProgresUpdates = new Dictionary<uint, uint>();
        public static void Load()
        {

            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "item_refine_upgrade.txt");
            foreach (var bas_line in baseText)
            {
                Database.DBActions.ReadLine line = new DBActions.ReadLine(bas_line, ' ');
                line.Read((uint)0);
                uint Level = line.Read((uint)0);
                uint Progres = line.Read((uint)0);
                ProgresUpdates.Add(Level, Progres);
            }
        }

    }
}
