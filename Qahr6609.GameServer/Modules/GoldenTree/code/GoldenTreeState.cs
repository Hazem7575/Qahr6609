using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Conquer6609.Database
{
    /// <summary>The running bloom of the Golden Tree: end time, picks taken, pick limit, and (so a restart keeps the tree as
    /// it was) its condition, the prizes still in it and the winners list. Kept in ServerDataBase\Modules\GoldenTree\state.ini
    /// (was shell.ini [GoldenTreeEvent], read once when state.ini is missing).</summary>
    public static class GoldenTreeState
    {
        public static DateTime Expiration = DateTime.MinValue;
        public static uint Claimed;
        /// <summary>Most picks of this bloom (0 = the whole prize pool).</summary>
        public static uint MaxClaims;
        // read from state.ini, used once by GlobalLotteryTable when the pool of this bloom is rebuilt
        public static uint ConditionID;
        public static DateTime PoolExpiration = DateTime.MinValue;
        public static Dictionary<uint, int> PoolLeft = new Dictionary<uint, int>();

        public static void Load(string folder)
        {
            string file = Path.Combine(folder, "state.ini");
            if (File.Exists(file))
            {
                Game.MsgServer.MsgGoldenTree.ClearRank();
                foreach (string raw in File.ReadAllLines(file))
                {
                    string line = raw.Trim();
                    int eq = line.IndexOf('=');
                    if (eq <= 0 || line[0] == ';' || line[0] == '[') continue;
                    string k = line.Substring(0, eq).Trim(), v = line.Substring(eq + 1).Trim();
                    DateTime end;
                    uint u;
                    switch (k)
                    {
                        case "Expiration":
                            Expiration = DateTime.TryParseExact(v, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out end) ? end : DateTime.MinValue;
                            break;
                        case "Claimed": uint.TryParse(v, out Claimed); break;
                        case "MaxClaims": uint.TryParse(v, out MaxClaims); break;
                        case "Condition": uint.TryParse(v, out ConditionID); break;
                        case "PoolOf":
                            PoolExpiration = DateTime.TryParseExact(v, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out end) ? end : DateTime.MinValue;
                            break;
                        case "Pool":// id:count,id:count
                            foreach (string part in v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string[] p = part.Split(':');
                                int n;
                                if (p.Length == 2 && uint.TryParse(p[0], out u) && int.TryParse(p[1], out n))
                                    PoolLeft[u] = n;
                            }
                            break;
                        default:
                            if (k.StartsWith("Pick", StringComparison.Ordinal))// PickN=prizeId|name
                            {
                                int bar = v.IndexOf('|');
                                if (bar > 0 && uint.TryParse(v.Substring(0, bar), out u))
                                    Game.MsgServer.MsgGoldenTree.Update(v.Substring(bar + 1), u);
                            }
                            break;
                    }
                }
                return;
            }
            // first start as a module: take over what the server kept in shell.ini
            var ini = new WindowsAPI.IniFile();
            ini.FileName = Path.Combine(Directory.GetCurrentDirectory(), "shell.ini");
            long binary = ini.ReadInt64("GoldenTreeEvent", "ExpirationDate", 0);
            Expiration = binary == 0 ? DateTime.MinValue : DateTime.FromBinary(binary);
            Claimed = ini.ReadUInt32("GoldenTreeEvent", "ClaimedCount", 0);
            MaxClaims = ini.ReadUInt32("GoldenTreeEvent", "MaxAvaliableGoldenTreeClaim", 0);
        }

        static readonly object saveLock = new object();

        public static void Save(string folder)
        {
            uint condition;
            DateTime poolOf;
            Dictionary<uint, int> left;
            GlobalLotteryTable.Snapshot(out condition, out poolOf, out left);
            var sb = new StringBuilder();
            sb.Append("; Golden Tree bloom (written by the server; start / stop it in game with @goldentree)\r\n[State]\r\n")
              .Append("Expiration=").Append(Expiration == DateTime.MinValue ? "" : Expiration.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).Append("\r\n")
              .Append("Claimed=").Append(Claimed).Append("\r\n")
              .Append("MaxClaims=").Append(MaxClaims).Append("\r\n")
              .Append("Condition=").Append(condition).Append("\r\n")
              .Append("PoolOf=").Append(poolOf == DateTime.MinValue ? "" : poolOf.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).Append("\r\n")
              .Append("Pool=");
            bool first = true;
            foreach (var kv in left)
            {
                if (!first) sb.Append(',');
                sb.Append(kv.Key).Append(':').Append(kv.Value);
                first = false;
            }
            sb.Append("\r\n[Picks]\r\n");
            int i = 0;
            foreach (var pick in Game.MsgServer.MsgGoldenTree.GoldenTreeRank)
                sb.Append("Pick").Append(++i).Append('=').Append(pick.Item2).Append('|').Append(pick.Item1.Replace("\r", "").Replace("\n", "")).Append("\r\n");
            lock (saveLock)
            {
                Directory.CreateDirectory(folder);
                string file = Path.Combine(folder, "state.ini"), tmp = file + ".tmp";
                File.WriteAllText(tmp, sb.ToString(), Encoding.UTF8);
                if (File.Exists(file)) File.Replace(tmp, file, null);
                else File.Move(tmp, file);
                WindowsAPI.IniCache.Invalidate(file);
            }
        }
    }
}
