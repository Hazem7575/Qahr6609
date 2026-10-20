using System;
using System.Collections.Generic;
using System.IO;

namespace Conquer6609.Database
{
    /// <summary>
    /// Contents of the openable packs listed in ServerDataBase\PackContents.ini (see that file's header for the format).
    /// Loaded on first use; a missing or broken file only means "no data-driven packs".
    /// </summary>
    public static class PackContents
    {
        public enum RewardKind : byte
        {
            Item,
            Steed,
            Money,
            ConquerPoints,
            BoundConquerPoints,
            StudyPoints,
            ChiPoints,
            PotencyPoints
        }

        public class Reward
        {
            public RewardKind Kind;
            public uint ItemID;
            public int Count = 1;
            public byte Plus;
            public byte Bless;
            public byte Sockets;
            public bool Bound;
            public int Days;
            public byte Green, Blue, Red;
            public long Amount;
        }

        public class Pack
        {
            public uint ID;
            public List<Reward> Rewards = new List<Reward>();
            /// <summary>CP price of the 1st, 2nd, ... open; the last value repeats. Empty = free.</summary>
            public uint[] Fees = new uint[0];
            /// <summary>How many times the pack can be opened before it is used up; 0 = never used up.</summary>
            public int Opens = 1;
            /// <summary>random=1: an open gives ONE of the rewards, picked by Weights (equal odds when empty).</summary>
            public bool Random;
            public int[] Weights = new int[0];

            /// <summary>The rewards one open gives (all of them, or one random pick for a random pack).</summary>
            public List<Reward> Roll()
            {
                if (!Random || Rewards.Count <= 1)
                    return Rewards;
                int total = 0;
                for (int i = 0; i < Rewards.Count; i++)
                    total += WeightOf(i);
                int roll = Program.GetRandom.Next(Math.Max(1, total));
                for (int i = 0; i < Rewards.Count; i++)
                {
                    roll -= WeightOf(i);
                    if (roll < 0)
                        return new List<Reward>() { Rewards[i] };
                }
                return new List<Reward>() { Rewards[Rewards.Count - 1] };
            }
            private int WeightOf(int index)
            {
                if (Weights.Length == 0)
                    return 1;
                return index < Weights.Length ? Math.Max(0, Weights[index]) : 0;
            }

            public uint FeeFor(int openIndex)
            {
                if (Fees.Length == 0)
                    return 0;
                return Fees[Math.Min(openIndex, Fees.Length - 1)];
            }
        }

        public const string FileName = "PackContents.ini";

        private static readonly object SyncRoot = new object();
        private static Dictionary<uint, Pack> Packs;

        public static bool TryGet(uint id, out Pack pack)
        {
            var packs = Packs;
            if (packs == null)
            {
                lock (SyncRoot)
                {
                    if (Packs == null)
                        Packs = LoadFile();
                    packs = Packs;
                }
            }
            return packs.TryGetValue(id, out pack);
        }

        /// <summary>Re-reads the file; returns the number of packs loaded.</summary>
        public static int Reload()
        {
            var packs = LoadFile();
            lock (SyncRoot)
                Packs = packs;
            return packs.Count;
        }

        private static Dictionary<uint, Pack> LoadFile()
        {
            var result = new Dictionary<uint, Pack>();
            string path;
            try
            {
                path = Path.Combine(Program.ServerConfig.DbLocation ?? "", FileName);
                if (!File.Exists(path))
                {
                    MyConsole.WriteLine("[PackContents] " + path + " not found - data-driven packs disabled.");
                    return result;
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                return result;
            }

            string[] lines;
            try
            {
                lines = File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                return result;
            }
            Pack current = null;
            int lineNo = 0;
            var broken = new HashSet<uint>();
            foreach (var raw in lines)
            {
                lineNo++;
                try
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line[0] == ';' || line[0] == '#')
                        continue;
                    if (line[0] == '[')
                    {
                        current = null;
                        uint id;
                        int end = line.IndexOf(']');
                        if (end > 1 && uint.TryParse(line.Substring(1, end - 1).Trim(), out id))
                        {
                            current = new Pack() { ID = id };
                            result[id] = current;
                        }
                        else
                            MyConsole.WriteLine("[PackContents] bad section at line " + lineNo);
                        continue;
                    }
                    if (current == null)
                        continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                        continue;
                    string key = line.Substring(0, eq).Trim().ToLowerInvariant();
                    string value = line.Substring(eq + 1);
                    int comment = value.IndexOf(';');
                    if (comment >= 0)
                        value = value.Substring(0, comment);
                    string[] f = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < f.Length; i++)
                        f[i] = f[i].Trim();
                    if (f.Length == 0)
                        continue;
                    switch (key)
                    {
                        case "item":
                            {
                                var r = new Reward() { Kind = RewardKind.Item };
                                r.ItemID = uint.Parse(f[0]);
                                if (f.Length > 1) r.Count = int.Parse(f[1]);
                                if (f.Length > 2) r.Plus = byte.Parse(f[2]);
                                if (f.Length > 3) r.Bless = byte.Parse(f[3]);
                                if (f.Length > 4) r.Sockets = byte.Parse(f[4]);
                                if (f.Length > 5) r.Bound = f[5] == "1";
                                if (f.Length > 6) r.Days = int.Parse(f[6]);
                                if (r.Count < 1) r.Count = 1;
                                current.Rewards.Add(r);
                                break;
                            }
                        case "steed":
                            {
                                var r = new Reward() { Kind = RewardKind.Steed, ItemID = 300000 };
                                r.Plus = byte.Parse(f[0]);
                                if (f.Length > 1) r.Green = byte.Parse(f[1]);
                                if (f.Length > 2) r.Blue = byte.Parse(f[2]);
                                if (f.Length > 3) r.Red = byte.Parse(f[3]);
                                if (f.Length > 4) r.Count = int.Parse(f[4]);
                                if (f.Length > 5) r.Bound = f[5] == "1";
                                if (r.Count < 1) r.Count = 1;
                                current.Rewards.Add(r);
                                break;
                            }
                        case "money": current.Rewards.Add(new Reward() { Kind = RewardKind.Money, Amount = long.Parse(f[0]) }); break;
                        case "cps": current.Rewards.Add(new Reward() { Kind = RewardKind.ConquerPoints, Amount = long.Parse(f[0]) }); break;
                        case "bcps": current.Rewards.Add(new Reward() { Kind = RewardKind.BoundConquerPoints, Amount = long.Parse(f[0]) }); break;
                        case "study": current.Rewards.Add(new Reward() { Kind = RewardKind.StudyPoints, Amount = long.Parse(f[0]) }); break;
                        case "chi": current.Rewards.Add(new Reward() { Kind = RewardKind.ChiPoints, Amount = long.Parse(f[0]) }); break;
                        case "potency": current.Rewards.Add(new Reward() { Kind = RewardKind.PotencyPoints, Amount = long.Parse(f[0]) }); break;
                        case "random": current.Random = f[0] == "1"; break;
                        case "weights":
                            {
                                var weights = new int[f.Length];
                                for (int i = 0; i < f.Length; i++)
                                    weights[i] = int.Parse(f[i]);
                                current.Weights = weights;
                                break;
                            }
                        case "fees":
                            {
                                var fees = new uint[f.Length];
                                for (int i = 0; i < f.Length; i++)
                                    fees[i] = uint.Parse(f[i]);
                                current.Fees = fees;
                                break;
                            }
                        case "opens": current.Opens = int.Parse(f[0]); break;
                        default:
                            MyConsole.WriteLine("[PackContents] unknown key '" + key + "' at line " + lineNo);
                            break;
                    }
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[PackContents] bad value at line " + lineNo + " (" + e.Message + ") - pack skipped.");
                    if (current != null)
                        broken.Add(current.ID);
                }
            }

            // drop packs without rewards or with a broken line instead of half-opening them
            var empty = new List<uint>(broken);
            foreach (var p in result.Values)
                if (p.Rewards.Count == 0 && !broken.Contains(p.ID))
                    empty.Add(p.ID);
            foreach (var id in empty)
                result.Remove(id);
            MyConsole.WriteLine("[PackContents] loaded " + result.Count + " packs.");
            return result;
        }
    }
}
