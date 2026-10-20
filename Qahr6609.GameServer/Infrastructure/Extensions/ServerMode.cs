// Server run mode, as in Qahr: Development vs Production. Developer / test tools (PacketStudio's live packet stream and
// packet injection through libs\PacketBridge.dll, and anything else gated on ServerMode.IsDevelopment) run only in
// Development, and so does the live reload of runtime modules (Modules\ModuleWatcher.cs). Production is the SAFE DEFAULT: a missing or unreadable mode file means Production.
//
//   ServerDataBase\servermode.txt   one line:  mode=development  (or dev)   |   mode=production  (or prod)
//   (a bare word works too; '#' starts a comment). Re-read every 5 seconds: switch it while the server runs.
//   Server console: "mode" shows it, "mode dev" / "mode prod" writes the file.
using System;
using System.IO;
using System.Threading;

namespace Conquer6609
{
    public static class ServerMode
    {
        static volatile bool isDev;
        static Timer timer;
        static int started;

        /// <summary>True only when servermode.txt selects development (cached, refreshed every 5 s).</summary>
        public static bool IsDevelopment { get { return isDev; } }

        public static string Current { get { return isDev ? "development" : "production"; } }

        static string FilePath { get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "servermode.txt"); } }

        /// <summary>Reads the mode, applies it (dev tools on/off) and keeps watching the file.</summary>
        public static void Start()
        {
            if (Interlocked.Exchange(ref started, 1) == 1) return;
            isDev = Read();
            Apply(isDev, true);
            timer = new Timer(_ => Refresh(), null, 5000, 5000);
        }

        static void Refresh()
        {
            bool dev = Read();
            if (dev == isDev) return;
            isDev = dev;
            Apply(dev, false);
        }

        static bool Read()
        {
            try
            {
                if (!File.Exists(FilePath)) return false;
                foreach (string raw in File.ReadAllLines(FilePath))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("#")) continue;
                    int eq = line.IndexOf('=');
                    string val = (eq >= 0 ? line.Substring(eq + 1) : line).Trim().ToLowerInvariant();
                    return val == "development" || val == "dev";// first meaningful line wins
                }
            }
            catch { }
            return false;
        }

        /// <summary>Writes servermode.txt (console "mode dev|prod"); the change applies within 5 s (now, here).</summary>
        public static void Set(bool dev)
        {
            File.WriteAllText(FilePath, "# development = test tools on (PacketStudio stream + injection); production = off (default)\r\nmode=" + (dev ? "development" : "production") + "\r\n");
            Refresh();
        }

        // turns the development-only tools on or off
        static void Apply(bool dev, bool boot)
        {
            try
            {
                if (dev)
                {
                    Program.StartPacketBridge();
                    MyConsole.WriteLine("[Mode] DEVELOPMENT: test tools on (PacketStudio live packets / injection if packetbridge.ini allows).", ConsoleColor.Yellow);
                }
                else
                {
                    if (!boot) PacketStudio.Bridge.PacketBridge.Stop();
                    MyConsole.WriteLine("[Mode] PRODUCTION: test tools off" + (boot ? "" : " (PacketBridge stopped)") + ".", ConsoleColor.Green);
                }
            }
            catch (Exception e) { MyConsole.WriteLine("[Mode] " + e.Message, ConsoleColor.Red); }
            try { Framework.Modules.ModuleWatcher.Sync(); } catch { }// live reload of runtime modules (development only)
        }
    }
}
