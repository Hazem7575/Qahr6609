using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    /// <summary>
    /// Persists the Kingdom War state in ServerDataBase\KingdomWar.ini (Loot Tokens of the day, carried Gold Bricks,
    /// known unions, last settlement). Union Gold Bricks / Kingdom status are saved with the unions (Unions\*.ini).
    /// Callers hold MsgKingdomWar.SyncRoot.
    /// </summary>
    internal static class KingdomWarStore
    {
        const string FileName = "KingdomWar.ini";

        static string FullPath { get { return Path.Combine(Program.ServerConfig.DbLocation, FileName); } }

        /// <summary>Returns false when there was no saved state yet (first run).</summary>
        public static bool Load()
        {
            try
            {
                string path = FullPath;
                if (!File.Exists(path))
                    return false;
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var raw in File.ReadAllLines(path))
                {
                    var line = raw.Trim();
                    if (line.Length == 0 || line[0] == ';' || line[0] == '[')
                        continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                        continue;
                    values[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
                }
                MsgKingdomWar.LastSettleDay = Int(values, "LastSettleDay");
                MsgKingdomWar.RegistrationDay = Int(values, "RegistrationDay");

                MsgKingdomWar.KnownUnions.Clear();
                foreach (var uid in UIntList(Get(values, "KnownUnions")))
                    MsgKingdomWar.KnownUnions.Add(uid);

                MsgKingdomWar.Registrations.Clear();
                foreach (var entry in Entries(Get(values, "Registrations")))
                {
                    // unionUID:kingdomUID
                    if (entry.Length < 2) continue;
                    uint union, kingdom;
                    if (!uint.TryParse(entry[0], out union) || !uint.TryParse(entry[1], out kingdom) || union == 0)
                        continue;
                    var u = MsgKingdomWar.GetUnion(union);
                    if (u == null) continue;
                    MsgKingdomWar.Registrations[union] = new MsgKingdomWar.Registration() { UnionUID = union, KingdomUID = kingdom, UnionName = u.Name };
                }

                MsgKingdomWar.Carried.Clear();
                foreach (var entry in Entries(Get(values, "CarriedBricks")))
                {
                    // playerUID:quality:kingdomUID
                    if (entry.Length < 3) continue;
                    uint player, quality, kingdom;
                    if (!uint.TryParse(entry[0], out player) || !uint.TryParse(entry[1], out quality) || !uint.TryParse(entry[2], out kingdom))
                        continue;
                    if (quality < 1 || quality > 5 || player == 0)
                        continue;
                    MsgKingdomWar.Carried[player] = new MsgKingdomWar.CarriedBrick() { Quality = (GoldBrickQuality)quality, KingdomUID = kingdom };
                }
                return Get(values, "Initialized") == "1";
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                return true; // never re-grant starting bricks because of a read error
            }
        }

        public static void Save()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("; Kingdom War state (written by the server)\r\n[KingdomWar]\r\n");
                sb.Append("Initialized=1\r\n");
                sb.Append("LastSettleDay=").Append(MsgKingdomWar.LastSettleDay.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
                sb.Append("RegistrationDay=").Append(MsgKingdomWar.RegistrationDay.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
                sb.Append("KnownUnions=").Append(string.Join(",", MsgKingdomWar.KnownUnions.Select(p => p.ToString(CultureInfo.InvariantCulture)).ToArray())).Append("\r\n");
                sb.Append("Registrations=").Append(string.Join(";", MsgKingdomWar.Registrations.Values
                    .Select(r => r.UnionUID + ":" + r.KingdomUID).ToArray())).Append("\r\n");
                sb.Append("CarriedBricks=").Append(string.Join(";", MsgKingdomWar.Carried
                    .Select(kv => kv.Key + ":" + (byte)kv.Value.Quality + ":" + kv.Value.KingdomUID).ToArray())).Append("\r\n");
                var kingdom = MsgKingdomWar.GetKingdom();
                sb.Append("Kingdom=").Append(kingdom != null ? kingdom.UID.ToString(CultureInfo.InvariantCulture) : "0").Append("\r\n");

                string path = FullPath;
                string tmp = path + ".tmp";
                File.WriteAllText(tmp, sb.ToString(), Encoding.ASCII);
                if (File.Exists(path))
                    File.Replace(tmp, path, null);
                else
                    File.Move(tmp, path);
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }

        static string Get(Dictionary<string, string> values, string key)
        {
            string v;
            return values.TryGetValue(key, out v) ? v : "";
        }
        static int Int(Dictionary<string, string> values, string key)
        {
            int v;
            return int.TryParse(Get(values, key), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0;
        }
        static IEnumerable<uint> UIntList(string s)
        {
            foreach (var part in s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                uint v;
                if (uint.TryParse(part.Trim(), out v) && v != 0)
                    yield return v;
            }
        }
        static IEnumerable<string[]> Entries(string s)
        {
            foreach (var part in s.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                yield return part.Trim().Split(':');
        }
    }
}
