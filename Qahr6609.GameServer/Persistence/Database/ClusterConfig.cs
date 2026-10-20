// Multi-server settings of this game server process (docs\MULTI_SERVER.md).
//
// shell.ini (all keys optional, the defaults keep the single-server behaviour):
//   [ServerInfo] ServerID=1            must equal this server's row in ServerDataBase\client_config.ini
//                                      (0 / missing = take the id of the row whose name is ServerName)
//   [ServerInfo] Registry=path         HazemConfig.ini (or older servers.ini) to check against (default: searched
//                                      in the working directory and up to 4 parent folders)
//   [Database]   ItemUIDMin=1          item UIDs handed out by this server stay inside [Min, Max]
//   [Database]   ItemUIDMax=99999999   (default 1 .. 2,000,000,000 = old behaviour)
//
// Start-up refuses (throws, the game port is never opened) when:
//   - ItemUIDMin/Max are invalid, or the saved ItemUID counter is already past ItemUIDMax;
//   - ServerID does not match the client_config.ini row of ServerName, or ServerName has no row;
//   - servers.ini lists another server (or the Realm) whose item UID range overlaps this one,
//     or lists this ServerID twice.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Conquer6609.Database
{
    public static class ClusterConfig
    {
        public const uint DefaultItemUIDMin = 1;
        public const uint DefaultItemUIDMax = ServerDatabase.MaxPlausibleItemUID;

        /// <summary>[ServerInfo] ServerID from shell.ini (0 = not set).</summary>
        public static uint ConfiguredServerID;
        public static uint ItemUIDMin = DefaultItemUIDMin;
        public static uint ItemUIDMax = DefaultItemUIDMax;
        /// <summary>servers.ini used for the checks (null = none found).</summary>
        public static string RegistryPath;

        static int warned90;

        /// <summary>Server.Initialize, before the item counter is set.</summary>
        public static void LoadShell()
        {
            ConfiguredServerID = (uint)Math.Max(0, Framework.Settings.Long("ServerInfo", "ServerID", 0));
            long min = Framework.Settings.Long("Database", "ItemUIDMin", DefaultItemUIDMin);
            long max = Framework.Settings.Long("Database", "ItemUIDMax", DefaultItemUIDMax);
            if (min < 1 || max > DefaultItemUIDMax || min >= max)
                Fatal("[Database] ItemUIDMin=" + min + " / ItemUIDMax=" + max + " is not a valid range (1 <= Min < Max <= " + DefaultItemUIDMax + ").");
            ItemUIDMin = (uint)min;
            ItemUIDMax = (uint)max;
        }

        /// <summary>The value the item counter starts from: the saved high-water mark, raised to Min-1 when it is
        /// below the range (new server). Refuses to start when the range is already used up.</summary>
        public static uint StartItemCounter(uint saved)
        {
            uint start = saved;
            if (start < ItemUIDMin - 1)
            {
                MyConsole.WriteLine("Item UID counter " + saved + " is below this server's range, starting at " + ItemUIDMin + ".");
                start = ItemUIDMin - 1;
            }
            if (start >= ItemUIDMax)
                Fatal("[Database] ItemUID=" + saved + " is already at/after ItemUIDMax=" + ItemUIDMax + ": the item UID range of this server is used up.");
            return start;
        }

        /// <summary>Range check after the counter was raised by the PlayersItems scan; arms the per-item limit.</summary>
        public static void ArmItemCounter(AtomicCounter counter)
        {
            if (counter.Count >= ItemUIDMax)
                Fatal("Item UID counter " + counter.Count + " is at/after ItemUIDMax=" + ItemUIDMax + ".");
            counter.Limit = ItemUIDMax;
            CheckItemUsage(counter.Count);
            MyConsole.WriteLine("Item UIDs: range " + ItemUIDMin + " - " + ItemUIDMax + ", next " + (counter.Count + 1L) + ".");
        }

        /// <summary>Warns once when 90% of the range is used (called at start-up and on every database save).</summary>
        public static void CheckItemUsage(uint current)
        {
            ulong size = (ulong)ItemUIDMax - ItemUIDMin + 1;
            ulong used = current >= ItemUIDMin ? (ulong)current - ItemUIDMin + 1 : 0;
            if (used * 10 >= size * 9 && System.Threading.Interlocked.Exchange(ref warned90, 1) == 0)
            {
                string msg = "Item UID range " + ItemUIDMin + "-" + ItemUIDMax + " is " + (used * 100 / size) + "% used (counter " + current + ").";
                MyConsole.WriteLine("WARNING: " + msg, ConsoleColor.Yellow);
                Security.Guard.Audit("item-uid-range", 0, "", msg);
            }
        }

        /// <summary>After GroupServerList.Load: ServerID / client_config / servers.ini checks.</summary>
        public static void ValidateGroup()
        {
            var my = GroupServerList.MyServerInfo;
            if (my == null)
                Fatal("[ServerInfo] ServerName=" + Program.ServerConfig.ServerName + " has no row in " + Program.ServerConfig.DbLocation + "client_config.ini.");
            if (ConfiguredServerID != 0 && ConfiguredServerID != my.ID)
                Fatal("[ServerInfo] ServerID=" + ConfiguredServerID + " but client_config.ini gives " + my.Name + " the id " + my.ID + ".");
            Program.ServerConfig.ServerID = my.ID;

            RegistryPath = FindRegistry();
            if (RegistryPath == null)
            {
                MyConsole.WriteLine("Cluster: ServerID " + my.ID + " (" + my.Name + "), no servers.ini found (single server).");
                return;
            }
            List<RegistryEntry> entries;
            try { entries = RegistryEntry.Load(RegistryPath); }
            catch (Exception e)
            {
                MyConsole.WriteLine("Cluster: servers.ini could not be read (" + e.Message + "), checks skipped.", ConsoleColor.Yellow);
                return;
            }
            var mine = entries.Where(e => e.ServerID == my.ID).ToList();
            if (mine.Count > 1)
                Fatal("servers.ini lists ServerID " + my.ID + " " + mine.Count + " times (" + string.Join(", ", mine.Select(e => "[" + e.Section + "]")) + ").");
            if (mine.Count == 0)
                MyConsole.WriteLine("Cluster: ServerID " + my.ID + " is not listed in " + RegistryPath + ".", ConsoleColor.Yellow);
            else
            {
                var e = mine[0];
                if (!string.Equals(e.Name, Program.ServerConfig.ServerName, StringComparison.Ordinal))
                    MyConsole.WriteLine("Cluster: servers.ini [" + e.Section + "] Name=" + e.Name + " but shell.ini ServerName=" + Program.ServerConfig.ServerName + ".", ConsoleColor.Yellow);
                if (e.ItemUIDMin != 0 && (e.ItemUIDMin != ItemUIDMin || e.ItemUIDMax != ItemUIDMax))
                    MyConsole.WriteLine("Cluster: servers.ini [" + e.Section + "] item range " + e.ItemUIDMin + "-" + e.ItemUIDMax + " differs from shell.ini " + ItemUIDMin + "-" + ItemUIDMax + ".", ConsoleColor.Yellow);
                if (!Program.ServerConfig.IsInterServer && e.GamePort != 0 && e.GamePort != Program.ServerConfig.GamePort)
                    MyConsole.WriteLine("Cluster: servers.ini [" + e.Section + "] GamePort=" + e.GamePort + " but shell.ini Game_Port=" + Program.ServerConfig.GamePort + ".", ConsoleColor.Yellow);
            }
            foreach (var other in entries)
            {
                if (other.ServerID == my.ID || other.ItemUIDMin == 0)
                    continue;
                if (other.ItemUIDMin <= ItemUIDMax && ItemUIDMin <= other.ItemUIDMax)
                    Fatal("item UID range " + ItemUIDMin + "-" + ItemUIDMax + " of this server overlaps [" + other.Section + "] " + other.Name +
                        " (" + other.ItemUIDMin + "-" + other.ItemUIDMax + ") in " + RegistryPath + ".");
            }
            MyConsole.WriteLine("Cluster: ServerID " + my.ID + " (" + my.Name + "), registry " + RegistryPath + " (" + entries.Count + " entries) OK.");
        }

        /// <summary>The master settings file (HazemConfig.ini, docs\MULTI_SERVER.md); servers.ini = older name.</summary>
        public static readonly string[] RegistryFileNames = { "HazemConfig.ini", "servers.ini" };

        public static string FindRegistry()
        {
            string configured = Framework.Settings.String("ServerInfo", "Registry", "");
            if (configured.Length > 0)
            {
                string p = Path.IsPathRooted(configured) ? configured : Path.Combine(Directory.GetCurrentDirectory(), configured);
                return File.Exists(p) ? Path.GetFullPath(p) : null;
            }
            try
            {
                var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                for (int i = 0; i < 5 && dir != null; i++, dir = dir.Parent)
                {
                    foreach (var name in RegistryFileNames)
                    {
                        string p = Path.Combine(dir.FullName, name);
                        if (File.Exists(p))
                            return p;
                        p = Path.Combine(dir.FullName, "Qahr6609.Shared", name);// the shared settings folder of the repo root
                        if (File.Exists(p))
                            return p;
                    }
                }
            }
            catch { }
            return null;
        }

        static void Fatal(string message)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("#####################################################################");
            Console.WriteLine("##  CLUSTER CONFIGURATION ERROR - the server does not start:");
            Console.WriteLine("##  " + message);
            Console.WriteLine("##  See docs\\MULTI_SERVER.md");
            Console.WriteLine("#####################################################################");
            Console.ForegroundColor = old;
            try { Security.Guard.Audit("cluster-config", 0, "", message); } catch { }
            throw new InvalidOperationException("Cluster configuration: " + message);
        }

        /// <summary>One game server / Realm section of servers.ini.</summary>
        public class RegistryEntry
        {
            public string Section = "";
            public string Name = "";
            public uint ServerID;
            public ushort GamePort;
            public uint ItemUIDMin, ItemUIDMax;

            /// <summary>[ServerN] and [Realm] sections with a ServerID; ';' starts a comment anywhere on a line.</summary>
            public static List<RegistryEntry> Load(string path)
            {
                var list = new List<RegistryEntry>();
                RegistryEntry cur = null;
                foreach (var raw in File.ReadAllLines(path))
                {
                    string line = raw;
                    int sc = line.IndexOf(';');
                    if (sc >= 0) line = line.Substring(0, sc);
                    line = line.Trim();
                    if (line.Length == 0)
                        continue;
                    if (line[0] == '[')
                    {
                        if (cur != null && cur.ServerID != 0) list.Add(cur);
                        string name = line.Trim('[', ']').Trim();
                        bool server = name.StartsWith("Server", StringComparison.OrdinalIgnoreCase) || name.Equals("Realm", StringComparison.OrdinalIgnoreCase);
                        cur = server ? new RegistryEntry { Section = name } : null;
                        continue;
                    }
                    if (cur == null)
                        continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                        continue;
                    string k = line.Substring(0, eq).Trim(), v = line.Substring(eq + 1).Trim();
                    uint n;
                    uint.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n);
                    switch (k.ToLowerInvariant())
                    {
                        case "name": cur.Name = v; break;
                        case "serverid": cur.ServerID = n; break;
                        case "gameport": cur.GamePort = (ushort)Math.Min(n, 65535); break;
                        case "itemuidmin": cur.ItemUIDMin = n; break;
                        case "itemuidmax": cur.ItemUIDMax = n; break;
                    }
                }
                if (cur != null && cur.ServerID != 0) list.Add(cur);
                return list;
            }
        }
    }
}
