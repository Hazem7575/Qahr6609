using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database
{
    public class TaskRewards
    {
        public static Dictionary<uint, List<uint>> Rewards = new Dictionary<uint, List<uint>>();
        public static Dictionary<uint, int[]> Weights = new Dictionary<uint, int[]>();

        /// <summary>Index of the reward drawn from a table, by weight (uniform when the table has no weights).</summary>
        public static uint PickIndex(uint table, int count)
        {
            int[] w;
            if (count <= 0)
                return 0;
            if (Weights.TryGetValue(table, out w))
            {
                int total = 0;
                for (int i = 0; i < count && i < w.Length; i++)
                    total += w[i];
                if (total > 0)
                {
                    int roll = Program.GetRandom.Next(0, total);
                    for (int i = 0; i < count && i < w.Length; i++)
                    {
                        if (roll < w[i])
                            return (uint)i;
                        roll -= w[i];
                    }
                }
            }
            return (uint)Program.GetRandom.Next(0, count);
        }
        public static void Load()
        {
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "task_reward_type.ini");
            foreach (var bas_line in baseText)
            {
                string[] line = bas_line.Split(' ');
                uint UID = uint.Parse(line[0]);

                Rewards.Add(UID, new List<uint>());
                for (int x = 0; x < 8; x++)
                    Rewards[UID].Add(uint.Parse(line[14 + x]));
                // columns 22-29: the weight of each reward (official task_reward_type); the draw used to be uniform
                var weights = new int[8];
                for (int x = 0; x < 8 && 22 + x < line.Length; x++)
                {
                    int w;
                    weights[x] = int.TryParse(line[22 + x], out w) && w > 0 ? w : 0;
                }
                Weights[UID] = weights;
            }
        }
    }
}
