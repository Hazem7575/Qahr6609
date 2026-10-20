using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace PatchServer
{
    /// <summary>
    /// Settings loaded from PatchServer.ini next to the executable, plus the
    /// absolute paths derived from them.
    /// </summary>
    internal sealed class ServerConfig
    {
        public const int DefaultPort = 8800;
        public const string DefaultRoot = "www";
        public const string IniFileName = "PatchServer.ini";

        /// <summary>TCP port to listen on.</summary>
        public int Port { get; private set; }

        /// <summary>Full path of the INI file that was read.</summary>
        public string IniPath { get; private set; }

        /// <summary>Full path of the web root (no trailing separator unless it is a drive root).</summary>
        public string RootDir { get; private set; }

        /// <summary>RootDir with exactly one trailing separator; every served path must start with it.</summary>
        public string RootPrefix { get; private set; }

        /// <summary>Full path of the distributed client files: [Server] Files, default Root\files.</summary>
        public string FilesDir { get; private set; }

        /// <summary>FilesDir with exactly one trailing separator; every served /files/ path must start with it.</summary>
        public string FilesPrefix { get; private set; }

        /// <summary>Full path of Root\manifest.json.</summary>
        public string ManifestPath { get; private set; }

        /// <summary>Full path of Root\news.json (legacy static file; the launcher v2 feed uses NewsSourcePath).</summary>
        public string NewsPath { get; private set; }

        /// <summary>Full path of Root\data (editable sources for the launcher feeds).</summary>
        public string DataDir { get; private set; }

        /// <summary>Full path of Root\data\news.json (source for GET /news.json).</summary>
        public string NewsSourcePath { get; private set; }

        /// <summary>Full path of Root\data\status.json (maintenance banner source for GET /status.json).</summary>
        public string StatusSourcePath { get; private set; }

        /// <summary>Full path of Root\data\events.json (upcoming events source for GET /events.json).</summary>
        public string EventsSourcePath { get; private set; }

        /// <summary>Full path of Root\data\crashes (uploaded launcher crash reports; one folder per day).</summary>
        public string CrashDir { get; private set; }

        /// <summary>[Crash] Enabled: accept POST /crash uploads from the launcher (default true).</summary>
        public bool AcceptCrashReports { get; private set; }

        /// <summary>[Crash] MaxBytes: largest crash report body accepted (default 512 KB).</summary>
        public int MaxCrashBytes { get; private set; }

        /// <summary>Folder of the exe (a relative [Servers] File is resolved against it).</summary>
        public string ExeDir { get; private set; }

        /// <summary>[Servers] File: path of HazemConfig.ini/servers.ini; empty = search the exe folder and its parents.</summary>
        public string ServersFile { get; private set; }

        /// <summary>[Servers] LoginHost/LoginPort: shared account server when the registry has no [Login]/[Auth]/[Global] values.</summary>
        public string LoginHost { get; private set; }
        public int LoginPort { get; private set; }

        /// <summary>[Servers] ProbeTimeoutMs / CacheSeconds for the Online/Offline check.</summary>
        public int ProbeTimeoutMs { get; private set; }
        public int ProbeCacheSeconds { get; private set; }

        /// <summary>
        /// Reads PatchServer.ini from exeDir (creating it with defaults if it
        /// does not exist) and resolves all paths.
        /// </summary>
        public static ServerConfig Load(string exeDir)
        {
            string iniPath = Path.Combine(exeDir, IniFileName);
            if (!File.Exists(iniPath))
            {
                WriteDefaultIni(iniPath);
            }

            IniFile ini = IniFile.Load(iniPath);

            int port = DefaultPort;
            string portText = ini.Get("Server", "Port", null);
            if (portText != null)
            {
                int parsed;
                if (int.TryParse(portText.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed)
                    && parsed > 0 && parsed <= 65535)
                {
                    port = parsed;
                }
                else
                {
                    Log.Warn("Invalid Port '" + portText + "' in " + IniFileName + ", using " + DefaultPort.ToString(CultureInfo.InvariantCulture));
                }
            }

            string root = ini.Get("Server", "Root", DefaultRoot);
            if (string.IsNullOrWhiteSpace(root))
            {
                root = DefaultRoot;
            }
            root = root.Trim().Trim('"');

            // Relative roots are resolved against the exe folder, not the
            // current working directory (which differs when started from a shortcut).
            string combined = Path.IsPathRooted(root) ? root : Path.Combine(exeDir, root);
            string rootDir = TrimTrailingSeparators(Path.GetFullPath(combined));

            ServerConfig cfg = new ServerConfig();
            cfg.Port = port;
            cfg.IniPath = iniPath;
            cfg.RootDir = rootDir;
            cfg.RootPrefix = EnsureTrailingSeparator(rootDir);
            // [Server] Files= : folder distributed to the clients (served as /files/...); empty = Root\files.
            // The repo points it at ClientPatch\files (the single source of the client patch).
            string files = (ini.Get("Server", "Files", string.Empty) ?? string.Empty).Trim().Trim('"');
            cfg.FilesDir = files.Length == 0
                ? Path.Combine(rootDir, "files")
                : TrimTrailingSeparators(Path.GetFullPath(Path.IsPathRooted(files) ? files : Path.Combine(exeDir, files)));
            cfg.FilesPrefix = EnsureTrailingSeparator(cfg.FilesDir);
            cfg.ManifestPath = Path.Combine(rootDir, "manifest.json");
            cfg.NewsPath = Path.Combine(rootDir, "news.json");
            cfg.DataDir = Path.Combine(rootDir, "data");
            cfg.NewsSourcePath = Path.Combine(cfg.DataDir, "news.json");
            cfg.StatusSourcePath = Path.Combine(cfg.DataDir, "status.json");
            cfg.EventsSourcePath = Path.Combine(cfg.DataDir, "events.json");
            // [Security] outside Root: everything under Root is served over HTTP, crash reports must not be (was Root\data\crashes)
            cfg.CrashDir = Path.Combine(exeDir, "crashes");
            cfg.AcceptCrashReports = (ini.Get("Crash", "Enabled", "1") ?? "1").Trim() != "0";
            cfg.MaxCrashBytes = ReadInt(ini, "Crash", "MaxBytes", 64 * 1024, 1024, 1024 * 1024);
            cfg.ExeDir = exeDir;
            cfg.ServersFile = ini.Get("Servers", "File", string.Empty);
            cfg.LoginHost = (ini.Get("Servers", "LoginHost", string.Empty) ?? string.Empty).Trim();
            cfg.LoginPort = ReadInt(ini, "Servers", "LoginPort", 0, 0, 65535);
            cfg.ProbeTimeoutMs = ReadInt(ini, "Servers", "ProbeTimeoutMs", 800, 100, 10000);
            cfg.ProbeCacheSeconds = ReadInt(ini, "Servers", "CacheSeconds", 5, 0, 600);
            return cfg;
        }

        private static int ReadInt(IniFile ini, string section, string key, int def, int min, int max)
        {
            string text = ini.Get(section, key, null);
            int value;
            if (string.IsNullOrWhiteSpace(text)
                || !int.TryParse(text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                || value < min || value > max)
            {
                return def;
            }
            return value;
        }

        /// <summary>Creates Root and Root\files if they are missing.</summary>
        public void EnsureDirectories()
        {
            Directory.CreateDirectory(RootDir);
            Directory.CreateDirectory(FilesDir);
            Directory.CreateDirectory(DataDir);
            Directory.CreateDirectory(Path.Combine(RootDir, "news", "img"));
            if (AcceptCrashReports) Directory.CreateDirectory(CrashDir);
        }

        private static void WriteDefaultIni(string iniPath)
        {
            try
            {
                string text = "[Server]\r\nPort=" + DefaultPort.ToString(CultureInfo.InvariantCulture)
                    + "\r\nRoot=" + DefaultRoot + "\r\n";
                File.WriteAllText(iniPath, text, new UTF8Encoding(false));
                Log.Info("Created default " + iniPath);
            }
            catch (Exception ex)
            {
                Log.Warn("Could not create " + iniPath + ": " + ex.Message + " (using defaults)");
            }
        }

        /// <summary>Removes trailing separators, but keeps a drive root such as "C:\" intact.</summary>
        internal static string TrimTrailingSeparators(string fullPath)
        {
            string pathRoot = Path.GetPathRoot(fullPath) ?? string.Empty;
            if (fullPath.Length <= pathRoot.Length)
            {
                return fullPath;
            }
            string trimmed = fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return trimmed.Length < pathRoot.Length ? pathRoot : trimmed;
        }

        /// <summary>Returns the path with exactly one trailing directory separator.</summary>
        internal static string EnsureTrailingSeparator(string fullPath)
        {
            if (fullPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                || fullPath.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                return fullPath;
            }
            return fullPath + Path.DirectorySeparatorChar;
        }
    }
}
