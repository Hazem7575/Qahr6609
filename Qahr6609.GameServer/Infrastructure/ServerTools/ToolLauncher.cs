// Opens the tools of Qahr6609.Tools\Releases on the server PC, as in Qahr.
// Console: "tool" lists them, "tool <number|name>" opens one. In game: "@tool" (project manager) lists, "@tool <n|name>" opens.
// A tool is <Releases>\<Name>.bat (preferred: it passes this server's folders) or <Releases>\<Name>\<Name>.exe.
// The Releases folder is found by walking up from the server exe, or set in ServerDataBase\tools.ini: "Folder=D:\path".
// A tool that is already running is brought to the front instead of opened twice.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Conquer6609.Framework.Commands;

namespace Conquer6609.ServerTools
{
    public static class ToolLauncher
    {
        public sealed class Tool
        {
            public string Name;
            public string Start;// .bat or .exe
            public string Exe;  // the process (for "already open")
            public string Info;
        }

        // display order + short description (English: the game chat font has no Arabic); missing tools are hidden
        static readonly string[][] Known =
        {
            new[] { "ServerControl", "server start/stop, stats, settings, logs, backups" },
            new[] { "MapStudio", "maps, NPC positions, walkable cells, portals (apply live)" },
            new[] { "ConquerStudio", "client assets and 3D models" },
            new[] { "PacketStudio", "live packets + injection (development mode)" },
            new[] { "DatCrypt", "encrypt/decrypt client .dat (items, skills, monsters)" },
            new[] { "UiExplorer", "client windows (7957 catalog; 6609 windows: ServerControl > UI page)" },
        };

        public static string ToolsFolder()
        {
            try
            {
                string ini = Path.Combine(Program.ServerConfig.DbLocation ?? "", "tools.ini");
                if (File.Exists(ini))
                    foreach (var line in File.ReadAllLines(ini))
                    {
                        int eq = line.IndexOf('=');
                        if (eq > 0 && line.Substring(0, eq).Trim().Equals("Folder", StringComparison.OrdinalIgnoreCase))
                        {
                            string f = line.Substring(eq + 1).Trim().Trim('"');
                            if (Directory.Exists(f)) return f;
                        }
                    }
            }
            catch { }
            // ...\Qahr6609\Qahr6609.GameServer\bin\Debug\  ->  ...\Qahr6609\Qahr6609.Tools\Releases
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 6 && dir != null; i++, dir = dir.Parent)
            {
                string candidate = Path.Combine(dir.FullName, "Qahr6609.Tools", "Releases");
                if (Directory.Exists(candidate)) return candidate;
            }
            return null;
        }

        public static List<Tool> List()
        {
            var list = new List<Tool>();
            string root = ToolsFolder();
            if (root == null) return list;
            var names = Known.Select(k => k[0]).ToList();
            // tools not in the known list (a new .bat dropped in Releases) come after, alphabetically
            foreach (var bat in Directory.GetFiles(root, "*.bat").Select(Path.GetFileNameWithoutExtension).OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
                if (!names.Contains(bat, StringComparer.OrdinalIgnoreCase)) names.Add(bat);
            foreach (string name in names)
            {
                string exe = Path.Combine(root, name, name + ".exe"), bat = Path.Combine(root, name + ".bat");
                string start = File.Exists(bat) ? bat : File.Exists(exe) ? exe : null;
                if (start == null || !File.Exists(exe)) continue;// a .bat without its tool (not unpacked) is hidden
                var known = Known.FirstOrDefault(k => k[0].Equals(name, StringComparison.OrdinalIgnoreCase));
                list.Add(new Tool { Name = name, Start = start, Exe = exe, Info = known != null ? known[1] : "" });
            }
            return list;
        }

        /// <summary>Find by 1-based number or (part of) the name.</summary>
        public static Tool Find(string key)
        {
            var list = List();
            int n;
            if (int.TryParse(key, out n) && n >= 1 && n <= list.Count) return list[n - 1];
            return list.FirstOrDefault(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                ?? list.FirstOrDefault(t => t.Name.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>Opens the tool (or focuses it if already running). Returns a short result message.</summary>
        public static string Open(Tool t)
        {
            if (t == null) return "Tool not found. Type tool to see the list.";
            try
            {
                var running = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(t.Exe))
                    .FirstOrDefault(p => { try { return p.MainWindowHandle != IntPtr.Zero; } catch { return false; } });
                if (running != null)
                {
                    IntPtr h = running.MainWindowHandle;
                    if (IsIconic(h)) ShowWindow(h, 9);// SW_RESTORE
                    SetForegroundWindow(h);
                    return t.Name + " is already open (brought to front).";
                }
                var psi = t.Start.EndsWith(".bat", StringComparison.OrdinalIgnoreCase)
                    ? new ProcessStartInfo("cmd.exe", "/c \"\"" + t.Start + "\"\"") { UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = Path.GetDirectoryName(t.Start) }
                    : new ProcessStartInfo(t.Start) { UseShellExecute = true, WorkingDirectory = Path.GetDirectoryName(t.Start) };
                Process.Start(psi);
                return t.Name + " opened on the server PC.";
            }
            catch (Exception e)
            {
                return "Could not open " + t.Name + ": " + e.Message;
            }
        }

        // ---------------- console: "tool" / "tool <n|name>" ----------------
        public static void Console(string[] args)
        {
            var list = List();
            if (list.Count == 0)
            {
                MyConsole.WriteLine("[tool] No tools found. Unpack them in Qahr6609.Tools\\Releases, or put Folder=<path> in ServerDataBase\\tools.ini", ConsoleColor.Yellow);
                return;
            }
            if (args.Length < 2)
            {
                MyConsole.WriteLine("[tool] ScriptHazem tools  ·  type: tool <number or name>", ConsoleColor.Yellow);
                for (int i = 0; i < list.Count; i++)
                    MyConsole.WriteLine("   " + (i + 1) + ". " + list[i].Name.PadRight(14) + " " + list[i].Info, ConsoleColor.Yellow);
                return;
            }
            MyConsole.WriteLine("[tool] " + Open(Find(args[1])), ConsoleColor.Green);
        }

        // ---------------- in game: "@tool" / "@tool <n|name>" ----------------
        [GmCommand("tool", "tools", Role = CommandRole.ProjectManager, Help = "server tools on the server PC: @tool (list) | @tool <number|name> (open)")]
        static void Command(Client.GameClient client, string[] data)
        {
            if (data.Length > 1)
            {
                client.SendSysMesage(Open(Find(data[1])));
                return;
            }
            var list = List();
            if (list.Count == 0) { client.SendSysMesage("No tools found on the server PC."); return; }
            for (int i = 0; i < list.Count; i++)
                client.SendSysMesage((i + 1) + ". " + list[i].Name + " - " + list[i].Info);
            client.SendSysMesage("@tool <number or name> opens it on the server PC.");
        }

        [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")] static extern bool IsIconic(IntPtr hWnd);
    }
}
