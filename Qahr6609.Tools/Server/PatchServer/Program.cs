using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace PatchServer
{
    /// <summary>
    /// scripthazem PatchServer: serves the launcher's manifest, client files,
    /// news and images over plain HTTP.
    ///
    ///   http://host:port/manifest.json   generated list of files (path/size/md5)
    ///   http://host:port/files/path      the client files themselves
    ///   http://host:port/news.json       news shown by the launcher
    ///   http://host:port/news/1.png      images, relative to the base URL
    /// </summary>
    internal static class Program
    {
        private static ServerConfig config;
        private static ManifestStore manifest;
        private static ServerRegistry registry;
        private static NewsStore news;
        private static StatusStore status;
        private static EventsStore events;
        private static CrashStore crashes;
        private static StaticHttpServer server;
        private static DateTime startedAt;

        private static int Main(string[] args)
        {
            startedAt = DateTime.Now;
            TrySetConsoleEncoding();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // Many players may download large files at once; each transfer
            // blocks a pool thread, so raise the minimum to avoid slow ramp-up.
            int minWorkers;
            int minIo;
            ThreadPool.GetMinThreads(out minWorkers, out minIo);
            ThreadPool.SetMinThreads(Math.Max(minWorkers, 64), Math.Max(minIo, 64));

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                config = ServerConfig.Load(exeDir);
                bool makeGuard = args.Length >= 1 && string.Equals(args[0], "--make-guard", StringComparison.OrdinalIgnoreCase);
                if (!makeGuard) config.EnsureDirectories();
            }
            catch (Exception ex)
            {
                Log.Error("Configuration failed: " + ex.Message);
                PauseBeforeExit();
                return 1;
            }

            TrySetTitle("scripthazem PatchServer :" + config.Port.ToString(CultureInfo.InvariantCulture));

            // PatchServer.exe --make-guard <folder>: writes Guard.dat + Guard.txt for the published servers and exits
            if (args.Length >= 1 && string.Equals(args[0], "--make-guard", StringComparison.OrdinalIgnoreCase))
            {
                registry = new ServerRegistry(config);
                return MakeGuard(args.Length >= 2 ? args[1] : exeDir) ? 0 : 3;
            }

            Log.Info("Config : " + config.IniPath);
            Log.Info("Root   : " + config.RootDir);

            // Build the manifest before listening so the first client never
            // sees a missing or stale manifest.
            manifest = new ManifestStore(config.FilesDir, config.ManifestPath);
            manifest.Rebuild();

            registry = new ServerRegistry(config);
            PrintServers();

            news = new NewsStore(config.NewsSourcePath);
            news.Prime();
            status = new StatusStore(config.StatusSourcePath, registry);
            events = new EventsStore(config.EventsSourcePath);
            events.Prime();
            crashes = config.AcceptCrashReports ? new CrashStore(config.CrashDir) : null;

            server = new StaticHttpServer(config, manifest, registry, news, status, events, crashes);
            try
            {
                server.Start();
            }
            catch (SocketException ex)
            {
                Log.Error("Cannot listen on port " + config.Port.ToString(CultureInfo.InvariantCulture)
                    + ": " + ex.Message + " (is another program using it?)");
                PauseBeforeExit();
                return 2;
            }

            PrintBanner();
            PrintNewsSummary();

            CommandLoop();

            server.Stop();
            Log.Info("Server stopped.");
            return 0;
        }

        /// <summary>Reads console commands until "exit".</summary>
        private static void CommandLoop()
        {
            while (true)
            {
                string line;
                try
                {
                    line = Console.ReadLine();
                }
                catch (IOException)
                {
                    line = null;
                }

                if (line == null)
                {
                    // stdin closed (e.g. started as a background process):
                    // keep serving until the process is killed.
                    Thread.Sleep(Timeout.Infinite);
                    return;
                }

                string command = line.Trim().ToLowerInvariant();
                switch (command)
                {
                    case "":
                        break;

                    case "rebuild":
                        manifest.Rebuild();
                        break;

                    case "status":
                        PrintStatus();
                        break;

                    case "servers":
                        PrintServers();
                        break;

                    case "crashes":
                        PrintCrashes();
                        break;

                    case "guard":
                        MakeGuard(Path.Combine(config.RootDir, "generated"));
                        break;

                    case "exit":
                    case "quit":
                        return;

                    default:
                        Log.Raw("Commands: rebuild | status | servers | crashes | guard | exit");
                        break;
                }
            }
        }

        private static void PrintBanner()
        {
            string port = config.Port.ToString(CultureInfo.InvariantCulture);
            const string Rule = "==============================================================";

            Log.Raw(string.Empty);
            Log.Raw(Rule, ConsoleColor.Cyan);
            Log.Raw("  scripthazem PatchServer", ConsoleColor.Cyan);
            Log.Raw(Rule, ConsoleColor.Cyan);
            Log.Raw("  Listening on TCP port " + port + " (all interfaces)");
            Log.Raw("  Root : " + config.RootDir);
            Log.Raw("  Files: " + config.FilesDir);
            Log.Raw(string.Empty);
            Log.Raw("  Launcher base URL (pick the one players can reach):");

            int shown = 0;
            foreach (KeyValuePair<string, string> address in GetIPv4Addresses())
            {
                string ip = address.Key;
                string tag = ip.StartsWith("26.", StringComparison.Ordinal) ? "Radmin" : "LAN";
                string url = "http://" + ip + ":" + port + "/";
                Log.Raw("    " + url.PadRight(30) + " [" + tag + "] " + address.Value,
                    tag == "Radmin" ? ConsoleColor.Green : ConsoleColor.Gray);
                shown++;
            }
            if (shown == 0)
            {
                Log.Raw("    (no active IPv4 network interfaces found)");
            }
            Log.Raw("    " + ("http://127.0.0.1:" + port + "/").PadRight(30) + " [local] this PC only");
            Log.Raw(string.Empty);
            Log.Raw("  manifest: <base>manifest.json   files : <base>files/<path>");
            Log.Raw("  news    : <base>news.json       source: www\\data\\news.json   images: www\\news\\img\\");
            Log.Raw("  status  : <base>status.json     source: www\\data\\status.json + live server status");
            Log.Raw("  events  : <base>events.json     source: www\\data\\events.json   (launcher countdown)");
            Log.Raw("  servers : <base>servers         published game servers + Online/Offline (HazemConfig.ini)");
            if (crashes != null)
                Log.Raw("  crash   : POST <base>crash      launcher crash reports -> www\\data\\crashes\\<date>\\");
            else
                Log.Raw("  crash   : disabled              ([Crash] Enabled=0 in PatchServer.ini)");
            Log.Raw(string.Empty);
            Log.Raw("  Allow inbound TCP " + port + " in Windows Firewall for other PCs.");
            Log.Raw("  Commands: rebuild | status | servers | crashes | guard | exit");
            Log.Raw(Rule, ConsoleColor.Cyan);
            Log.Raw(string.Empty);
        }

        /// <summary>Lists how many crash reports have been received and where they are stored.</summary>
        private static void PrintCrashes()
        {
            if (crashes == null)
            {
                Log.Raw("  Crash reports are disabled ([Crash] Enabled=0 in PatchServer.ini).");
                return;
            }
            Log.Raw("  Crash reports folder : " + crashes.Dir);
            Log.Raw("  Received this session: " + crashes.TotalSaved.ToString(CultureInfo.InvariantCulture));
            try
            {
                if (Directory.Exists(crashes.Dir))
                {
                    int onDisk = Directory.GetFiles(crashes.Dir, "crash_*.txt", SearchOption.AllDirectories).Length;
                    Log.Raw("  Stored on disk (total): " + onDisk.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    Log.Raw("  Stored on disk (total): 0 (no report received yet)");
                }
            }
            catch (Exception ex)
            {
                Log.Warn("  Could not count stored reports: " + ex.Message);
            }
        }

        private static void PrintStatus()
        {
            ManifestSnapshot snapshot = manifest.Current;
            TimeSpan uptime = DateTime.Now - startedAt;

            Log.Raw("  Port        : " + config.Port.ToString(CultureInfo.InvariantCulture));
            Log.Raw("  Root        : " + config.RootDir);
            if (snapshot != null)
            {
                Log.Raw("  Files       : " + snapshot.FileCount.ToString(CultureInfo.InvariantCulture)
                    + " (" + Log.FormatSize(snapshot.TotalBytes) + ")");
                Log.Raw("  Manifest    : generated " + snapshot.GeneratedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else
            {
                Log.Raw("  Files       : (no manifest built)");
            }
            Log.Raw("  Connections : " + server.ActiveConnections.ToString(CultureInfo.InvariantCulture) + " active, "
                + server.TotalRequests.ToString(CultureInfo.InvariantCulture) + " requests total");
            Log.Raw("  Uptime      : " + ((int)uptime.TotalHours).ToString(CultureInfo.InvariantCulture) + "h "
                + uptime.Minutes.ToString("00", CultureInfo.InvariantCulture) + "m");
            PrintNewsSummary();
        }

        /// <summary>
        /// Writes the client login list (Guard.dat, encrypted) and Guard.txt (plain) for the published servers
        /// into folder. Never touches www\files: copying the file to the players is a separate, manual step.
        /// </summary>
        private static bool MakeGuard(string folder)
        {
            try
            {
                List<ScriptHazem.ClientFiles.GuardFile.Entry> entries = registry.BuildGuardEntries();
                if (entries.Count == 0)
                {
                    Log.Warn("guard: no published server in " + (registry.FilePath ?? "HazemConfig.ini / servers.ini (not found)"));
                    return false;
                }
                Directory.CreateDirectory(folder);
                string xml = ScriptHazem.ClientFiles.GuardFile.BuildXml(entries);
                File.WriteAllText(Path.Combine(folder, "Guard.txt"), xml, new UTF8Encoding(false));
                File.WriteAllBytes(Path.Combine(folder, "Guard.dat"), ScriptHazem.ClientFiles.GuardFile.Crypt(new UTF8Encoding(false).GetBytes(xml)));
                Log.Info("guard: " + entries.Count.ToString(CultureInfo.InvariantCulture) + " servers -> " + Path.Combine(folder, "Guard.dat"));
                foreach (ScriptHazem.ClientFiles.GuardFile.Entry e in entries)
                {
                    Log.Raw("    ServerName=" + e.LoginName + "  FlashName=" + e.DisplayName + "  " + e.LoginHost + ":"
                        + e.LoginPort.ToString(CultureInfo.InvariantCulture));
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("guard: " + ex.Message);
                return false;
            }
        }

        /// <summary>Prints the game-server registry (published entries are what /servers sends).</summary>
        private static void PrintServers()
        {
            string loginHost;
            int loginPort;
            List<GameServerEntry> all = registry.GetAll(out loginHost, out loginPort);
            string path = registry.FilePath;
            Log.Info("registry: " + (path ?? "(not found)") + " - login " + (loginHost ?? "?") + ":"
                + loginPort.ToString(CultureInfo.InvariantCulture));
            foreach (GameServerEntry s in all)
            {
                Log.Raw("    #" + s.Id.ToString(CultureInfo.InvariantCulture) + " " + s.Name.PadRight(16) + " "
                    + (s.Host + ":" + s.GamePort.ToString(CultureInfo.InvariantCulture)).PadRight(22)
                    + (s.Published ? "published" : "hidden   ") + (s.Enabled ? "" : " (disabled)"));
            }
        }

        /// <summary>Summarises the launcher feeds so a typo is noticed before players see a broken launcher.</summary>
        private static void PrintNewsSummary()
        {
            if (news == null)
            {
                return;
            }

            // reading Current forces a rebuild; a broken source is logged by the store itself
            int bytes = news.Current.Length;
            Log.Info("news.json: " + news.Count.ToString(CultureInfo.InvariantCulture) + " items, "
                + bytes.ToString(CultureInfo.InvariantCulture) + " bytes (source " + news.SourcePath + ")");

            if (status != null)
            {
                Log.Info("status.json: " + (File.Exists(status.SourcePath)
                    ? "banner from " + status.SourcePath
                    : "no banner file (default: not in maintenance) - " + status.SourcePath));
            }

            if (events != null)
            {
                int ebytes = events.Current.Length;
                Log.Info("events.json: " + events.Count.ToString(CultureInfo.InvariantCulture) + " entries, "
                    + ebytes.ToString(CultureInfo.InvariantCulture) + " bytes (source " + events.SourcePath + ")");
            }
        }

        /// <summary>Returns (ip, interface name) for every IPv4 unicast address on active, non-loopback NICs.</summary>
        private static List<KeyValuePair<string, string>> GetIPv4Addresses()
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            NetworkInterface[] nics;
            try
            {
                nics = NetworkInterface.GetAllNetworkInterfaces();
            }
            catch (Exception)
            {
                return result;
            }

            foreach (NetworkInterface nic in nics)
            {
                try
                {
                    if (nic.OperationalStatus != OperationalStatus.Up
                        || nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    {
                        continue;
                    }

                    IPInterfaceProperties props = nic.GetIPProperties();
                    foreach (UnicastIPAddressInformation unicast in props.UnicastAddresses)
                    {
                        if (unicast.Address.AddressFamily != AddressFamily.InterNetwork)
                        {
                            continue;
                        }
                        string ip = unicast.Address.ToString();
                        if (ip.StartsWith("169.254.", StringComparison.Ordinal))
                        {
                            continue; // APIPA / link-local: not reachable by players.
                        }
                        result.Add(new KeyValuePair<string, string>(ip, nic.Name));
                    }
                }
                catch (Exception)
                {
                    // Some virtual adapters throw on property access; skip them.
                }
            }

            return result;
        }

        private static void TrySetConsoleEncoding()
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
            }
            catch (Exception)
            {
            }
        }

        private static void TrySetTitle(string title)
        {
            try
            {
                Console.Title = title;
            }
            catch (Exception)
            {
            }
        }

        private static void PauseBeforeExit()
        {
            try
            {
                Log.Raw("Press Enter to exit.");
                Console.ReadLine();
            }
            catch (Exception)
            {
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Unhandled exception: " + e.ExceptionObject);
        }
    }
}
