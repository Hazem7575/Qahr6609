namespace GShield
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class QueryProcess
    {
        public string ExeName { get; set; }
        public string MD5_Hash { get; set; }
        public uint SectionXxHash { get; set; }
        public uint SectionMemXxHash { get; set; }
        public bool IsHack { get; set; } = true;

        /// <summary>The loaded blacklist. Never changed in place: Init builds a new dictionary and swaps it in,
        /// so the socket threads reading it (ContainOnList/BuildIndex) never see a half-filled list.</summary>
        public static volatile Dictionary<uint, QueryProcess> QueryProcessList = new Dictionary<uint, QueryProcess>();

        internal static void Init()
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "QueryProcess.txt");
            Dictionary<uint, QueryProcess> fresh = null;

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("[QueryProcess] Blacklist file not found: " + filePath);
                    return;
                }

                fresh = new Dictionary<uint, QueryProcess>();
                uint counter = 0;
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                        continue;

                    string[] parts = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 4) continue;

                    try
                    {
                        var process = new QueryProcess
                        {
                            ExeName = parts[0].Trim().ToLowerInvariant(),
                            MD5_Hash = parts[1].Trim(),
                            SectionXxHash = string.IsNullOrWhiteSpace(parts[2]) ? 0 : uint.Parse(parts[2].Trim()),
                            SectionMemXxHash = string.IsNullOrWhiteSpace(parts[3]) ? 0 : uint.Parse(parts[3].Trim())
                        };

                        fresh.Add(++counter, process);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[QueryProcess] Error parsing line '{0}': {1}", line, ex.Message);
                    }
                }

                QueryProcessList = fresh;
                fresh = null;
                BuildIndex();
                Console.WriteLine("[QueryProcess] Successfully loaded {0} banned processes.", QueryProcessList.Count);
            }
            catch (Exception ex)
            {
                // same result as before (the list was cleared before reading): a failed reload leaves it empty
                if (fresh != null)
                    QueryProcessList = new Dictionary<uint, QueryProcess>();
                Console.WriteLine("[QueryProcess] Failed to initialize: " + ex.Message);
            }
        }

        // Lookup index over QueryProcessList: key -> position of the FIRST blacklist entry with that key, so the
        // result (which entry, which reason) is exactly what the old linear scan returned.
        private sealed class BlacklistIndex
        {
            public int Count;
            public QueryProcess[] Entries;
            public Dictionary<string, int> Exe = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, int> Md5 = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            public Dictionary<uint, int> Sec = new Dictionary<uint, int>();
            public Dictionary<uint, int> Mem = new Dictionary<uint, int>();
        }
        private static volatile BlacklistIndex index;

        private static void AddFirst<T>(Dictionary<T, int> map, T key, int i)
        {
            if (!map.ContainsKey(key))
                map[key] = i;
        }

        internal static void BuildIndex()
        {
            var idx = new BlacklistIndex();
            var source = QueryProcessList;
            var list = new List<QueryProcess>(source.Values);
            idx.Entries = list.ToArray();
            idx.Count = source.Count;
            for (int i = 0; i < idx.Entries.Length; i++)
            {
                var b = idx.Entries[i];
                if (!string.IsNullOrEmpty(b.ExeName)) AddFirst(idx.Exe, b.ExeName, i);
                if (!string.IsNullOrEmpty(b.MD5_Hash) && b.MD5_Hash != "0") AddFirst(idx.Md5, b.MD5_Hash, i);
                if (b.SectionXxHash != 0) AddFirst(idx.Sec, b.SectionXxHash, i);
                if (b.SectionMemXxHash != 0) AddFirst(idx.Mem, b.SectionMemXxHash, i);
            }
            index = idx;
        }

        private static void Lower<T>(Dictionary<T, int> map, T key, ref int best)
        {
            int i;
            if (map.TryGetValue(key, out i) && i < best)
                best = i;
        }

        internal static bool ContainOnList(QueryProcess clientProcess, out string detectedExeName)
        {
            detectedExeName = "None";

            if (clientProcess == null)
                return false;

            var idx = index;
            if (idx == null || idx.Count != QueryProcessList.Count)
            {
                BuildIndex();
                idx = index;
            }
            // O(1): first blacklist entry that matches by any rule
            int best = int.MaxValue;
            if (!string.IsNullOrEmpty(clientProcess.ExeName))
                Lower(idx.Exe, clientProcess.ExeName, ref best);
            if (!string.IsNullOrEmpty(clientProcess.MD5_Hash) && clientProcess.MD5_Hash != "0")
                Lower(idx.Md5, clientProcess.MD5_Hash, ref best);
            Lower(idx.Sec, clientProcess.SectionXxHash, ref best);
            Lower(idx.Sec, clientProcess.SectionMemXxHash, ref best);
            Lower(idx.Mem, clientProcess.SectionMemXxHash, ref best);
            Lower(idx.Mem, clientProcess.SectionXxHash, ref best);
            if (best == int.MaxValue)
                return false;

            // re-run the original rule chain on that single entry (gives the same reason text)
            foreach (var banned in new QueryProcess[] { idx.Entries[best] })
            {
                bool matched = false;
                string reason = "";

                // Match by executable name
                if (!string.IsNullOrEmpty(banned.ExeName) &&
                    !string.IsNullOrEmpty(clientProcess.ExeName) &&
                    banned.ExeName.Equals(clientProcess.ExeName, StringComparison.OrdinalIgnoreCase))
                {
                    matched = true;
                    reason = "ProcessName";
                }
                // Match by MD5
                else if (!string.IsNullOrEmpty(banned.MD5_Hash) && banned.MD5_Hash != "0" &&
                         !string.IsNullOrEmpty(clientProcess.MD5_Hash) && clientProcess.MD5_Hash != "0" &&
                         banned.MD5_Hash.Equals(clientProcess.MD5_Hash, StringComparison.OrdinalIgnoreCase))
                {
                    matched = true;
                    reason = "MD5";
                }
                // Match by Section Hash
                else if (banned.SectionXxHash != 0 &&
                        (banned.SectionXxHash == clientProcess.SectionXxHash ||
                         banned.SectionXxHash == clientProcess.SectionMemXxHash))
                {
                    matched = true;
                    reason = "SectionHash";
                }
                // Match by Memory Section Hash
                else if (banned.SectionMemXxHash != 0 &&
                        (banned.SectionMemXxHash == clientProcess.SectionMemXxHash ||
                         banned.SectionMemXxHash == clientProcess.SectionXxHash))
                {
                    matched = true;
                    reason = "MemoryHash";
                }

                if (matched)
                {
                    detectedExeName = banned.ExeName ?? "UnknownProcess";
                    Console.WriteLine("[HazemShield] Banned process detected → {0} ({1})", detectedExeName, reason);
                    return true;
                }
            }

            return false;
        }

        public static bool IsValidProcessData(QueryProcess process)
        {
            if (process == null) return false;

            return !string.IsNullOrEmpty(process.ExeName) ||
                   (!string.IsNullOrEmpty(process.MD5_Hash) && process.MD5_Hash != "0") ||
                   process.SectionXxHash != 0 ||
                   process.SectionMemXxHash != 0;
        }
    }

    public static class _GShield
    {
        public static List<string> BlockAppsList = new List<string>();
        public static string GameCryptographyKey = "fQ9X7vB3sP1kE8tL0mW4aR9uZ2yN6hC";

        public static string exefile;
        public static string dllfile;
        public static string enMTFile;
        public static string enMEFile;
        public static string enSRFile;
        public static string exefile_dx9;
        public static string dllfile_dx9;

        // Accepted hashes per checked file. Index: 0 DX8 Conquer.exe, 1 MagicType.dat, 2 MagicEffect.ini,
        // 3 DX8 HazemShield.dll, 4 StrRes.ini, 5 DX9 Conquer.exe, 6 DX9 HazemShield.dll.
        // Filled from Files\ (the main client) and from every Files\Clients\<name>\ folder (other allowed
        // clients, e.g. a clean TQ client + our engine). Put the client's files there with the same relative
        // paths (ini\MagicType.dat, Env_DX9\Conquer.exe, ...) and restart: that client is accepted too.
        static readonly string[] CheckNames = { "DX8_Conquer.exe", "MagicType.dat", "MagicEffect.ini", "DX8_MeGaShield.dll", "StrRes.ini", "DX9_Conquer.exe", "DX9_MeGaShield.dll" };
        static readonly string[] CheckPaths = { @"Env_DX8\Conquer.exe", @"ini\MagicType.dat", @"ini\MagicEffect.ini", @"Env_DX8\HazemShield.dll", @"ini\StrRes.ini", @"Env_DX9\Conquer.exe", @"Env_DX9\HazemShield.dll" };
        static HashSet<string>[] accepted;

        public static bool Validated(string Ex8, string MT, string ME, string DL8, string SR, string Ex9, string DL9, out string FileChanged)
        {
            FileChanged = string.Empty;
            string[] got = { Ex8, MT, ME, DL8, SR, Ex9, DL9 };
            var sets = accepted;
            for (int i = 0; i < got.Length; i++)
            {
                string h = (got[i] ?? "").Trim().ToLowerInvariant();
                bool ok = sets != null && sets[i].Count > 0 ? sets[i].Contains(h) : h == (Primary(i) ?? "");
                if (!ok) FileChanged += CheckNames[i] + " ";
            }
            return string.IsNullOrEmpty(FileChanged);
        }

        static string Primary(int i)
        {
            switch (i)
            {
                case 0: return exefile;
                case 1: return enMTFile;
                case 2: return enMEFile;
                case 3: return dllfile;
                case 4: return enSRFile;
                case 5: return exefile_dx9;
                default: return dllfile_dx9;
            }
        }

        static void BuildAcceptedSets()
        {
            var sets = new HashSet<string>[CheckPaths.Length];
            for (int i = 0; i < sets.Length; i++)
            {
                sets[i] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                string p = Primary(i);
                if (!string.IsNullOrEmpty(p)) sets[i].Add(p);
            }
            try
            {
                string root = Path.Combine("Files", "Clients");
                if (Directory.Exists(root))
                {
                    foreach (var dir in Directory.GetDirectories(root))
                    {
                        int n = 0;
                        for (int i = 0; i < CheckPaths.Length; i++)
                        {
                            string f = ShieldFile(Path.Combine(dir, CheckPaths[i]));
                            if (!File.Exists(f)) continue;
                            string h = CalculateMD5(Path.GetFileName(dir) + " " + CheckNames[i], f);
                            if (!string.IsNullOrEmpty(h)) { sets[i].Add(h); n++; }
                        }
                        Console.WriteLine("HazemShield: extra client '" + Path.GetFileName(dir) + "' accepted (" + n + " file hashes).");
                    }
                }
            }
            catch (Exception e) { Console.WriteLine("HazemShield: Files\\Clients load failed: " + e.Message); }
            accepted = sets;
        }

        // The client's shield dll is still named MeGaShield.dll (the client and Files\ were not renamed with the code):
        // HazemShield.dll when it exists, else MeGaShield.dll next to it. Without this the dll hash was empty and every
        // real client was disconnected with "Modified Client Files: DX8_MeGaShield.dll DX9_MeGaShield.dll".
        static string ShieldFile(string hazemPath)
        {
            if (File.Exists(hazemPath)) return hazemPath;
            string mega = Path.Combine(Path.GetDirectoryName(hazemPath) ?? "", "MeGaShield.dll");
            return File.Exists(mega) ? mega : hazemPath;
        }

        public static void LoadFilesAntiCheat()
        {
            QueryProcess.Init();

            //Console.WriteLine(" [+] Loading HazemShield File Hashes (Anti-Tamper System)");

            exefile = CalculateMD5("DX8 Conquer.exe", @"Files\Env_DX8\Conquer.exe");
            enMTFile = CalculateMD5("MagicType.dat", @"Files\ini\MagicType.dat");
            enMEFile = CalculateMD5("MagicEffect.ini", @"Files\ini\MagicEffect.ini");
            dllfile = CalculateMD5("DX8 shield dll", ShieldFile(@"Files\Env_DX8\HazemShield.dll"));
            enSRFile = CalculateMD5("StrRes.ini", @"Files\ini\StrRes.ini");
            exefile_dx9 = CalculateMD5("DX9 Conquer.exe", @"Files\Env_DX9\Conquer.exe");
            dllfile_dx9 = CalculateMD5("DX9 shield dll", ShieldFile(@"Files\Env_DX9\HazemShield.dll"));
            BuildAcceptedSets();

            Console.WriteLine("HazemShield Full Loaded & Ready.");
            Console.WriteLine("");
        }

        private static string CalculateMD5(string displayName, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" [X] Missing: {0} → {1}", displayName.PadRight(25), filePath);
                Console.ResetColor();
                return string.Empty;
            }

            try
            {
                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    string result = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                    Console.ForegroundColor = ConsoleColor.Green;
                    //Console.WriteLine(" [OK] {0} → {1}", displayName.PadRight(25), result);
                    Console.ResetColor();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" [!] Failed to hash {0}: {1}", displayName.PadRight(25), ex.Message);
                Console.ResetColor();
                return string.Empty;
            }
        }
    }
    public class Types
    {
        public enum PacketTypes : short
        {
            CMsgShield = 10050,
            CMsgMemoryCheck = 10053,
            CMsgCloseClient = 10054,
            CMsgRequestLogin = 10055,
            CMsgOpenedProcesses = 10056,
            MsgSendPCInfo = 10057,
            CMsgHotKey = 10058,
        }

        public enum CheatFlags : short
        {
            None = 0,
            Program = 2,
            AutoClicker = 4,
            AutoKeyboard = 6,
            SpeedCClient = 8,
            Debugger = 10,
            InjectDll = 12,
            CloseingThread = 14,
            ChangeConquerFile = 16,
            Cheatengine = 18
        }

        public enum SubType : short
        {
            DoLogin = 10,
            ThreadCheck = 20,
            MemoryCheck = 30,
            HackDetected = 40,
            DiscordTitle1 = 50,
            DiscordTitle2 = 60,
            ProcessesCheck = 70,
            ConquerFileHash = 80,
            PCINFOSend = 90,
            MsgForceClose = 122,
            MsgCustomNotification = 124,
            MsgDiscordOnline = 125,
        }

        public enum ProcessesType : short
        {
            Start = 110,
            Insert = 220,
            Finish = 330,
        }
    }

}