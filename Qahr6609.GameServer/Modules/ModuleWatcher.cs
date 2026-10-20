// Development: a runtime module (ServerDataBase\Modules\<Name>\) is reloaded by itself when one of its .cs files is
// saved (ModuleManager.ReloadRuntime): write the code, save, test in game - no build, no restart.
// Only in development mode (servermode.txt); production never watches. Online GMs get the result in their chat.
// By hand:  console "module reload <Name>" | in game "@module reload <Name>" | AdminApi {"op":"modreload","name":"<Name>"}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Conquer6609.Framework.Modules
{
    public static class ModuleWatcher
    {
        static readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        static Timer timer;
        static readonly ConcurrentDictionary<string, DateTime> pending = new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        const int QuietMs = 1200;// editors write a file in several steps: wait until it is quiet

        /// <summary>Starts (development) or stops (production) the watchers; called at start-up and when the mode changes.
        /// Watched: ServerDataBase\Modules and the LIVE source roots (RuntimeModules.LiveRoots).</summary>
        public static void Sync()
        {
            bool want = ServerMode.IsDevelopment && ModuleManager.Initialized;
            lock (watchers)
            {
                if (want == (watchers.Count > 0)) return;
                if (!want)
                {
                    foreach (var w in watchers) try { w.EnableRaisingEvents = false; w.Dispose(); } catch { }
                    watchers.Clear();
                    MyConsole.WriteLine("[Modules] live reload off (production mode)", ConsoleColor.Yellow);
                    return;
                }
                var roots = new List<string>();
                if (Directory.Exists(RuntimeModules.Root)) roots.Add(RuntimeModules.Root);
                roots.AddRange(RuntimeModules.LiveRoots);
                foreach (string root in roots)
                {
                    try
                    {
                        string r = root;
                        var w = new FileSystemWatcher(r, "*.cs") { IncludeSubdirectories = true, NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size };
                        FileSystemEventHandler h = (s, e) => OnFile(r, e.FullPath);
                        w.Changed += h;
                        w.Created += h;
                        w.Deleted += h;
                        w.Renamed += (s, e) => OnFile(r, e.FullPath);
                        w.EnableRaisingEvents = true;
                        watchers.Add(w);
                    }
                    catch (Exception e) { MyConsole.WriteLine("[Modules] live reload: " + root + ": " + e.Message, ConsoleColor.Yellow); }
                }
                if (watchers.Count > 0)
                {
                    if (timer == null) timer = new Timer(_ => Flush(), null, 500, 500);
                    MyConsole.WriteLine("[Modules] live reload on: saving a .cs file of a runtime / LIVE module reloads it (" + string.Join(", ", roots) + ")", ConsoleColor.Green);
                }
            }
        }

        static void OnFile(string root, string path)
        {
            string rel = path.Length > root.Length ? path.Substring(root.Length).TrimStart('\\', '/') : "";
            string[] parts = rel.Split('\\', '/');
            if (parts.Length < 2) return;
            string name = parts[0];
            if (name.StartsWith("_") || name.StartsWith(".")) return;
            string sub = parts.Length > 2 ? parts[1].ToLowerInvariant() : "";
            if (sub == "data" || sub == "content" || sub == "logs" || sub == ".build" || sub == "client") return;
            // only a runtime module (ServerDataBase) or a LIVE source module: the other source modules are built into the exe
            if (!string.Equals(Path.GetFullPath(Path.Combine(root, name)), Path.GetFullPath(RuntimeModules.FolderOf(name)), StringComparison.OrdinalIgnoreCase)) return;
            pending[name] = DateTime.UtcNow;
        }

        static int busy;

        static void Flush()
        {
            if (Interlocked.Exchange(ref busy, 1) != 0) return;
            try
            {
                foreach (var kv in pending.ToArray())
                {
                    if ((DateTime.UtcNow - kv.Value).TotalMilliseconds < QuietMs) continue;
                    DateTime stamp;
                    if (!pending.TryRemove(kv.Key, out stamp)) continue;
                    if (stamp != kv.Value) { pending[kv.Key] = stamp; continue; }// changed again meanwhile
                    var lines = ModuleManager.ReloadRuntime(kv.Key);
                    TellGms(lines);
                }
            }
            catch (Exception e) { MyConsole.WriteLine("[Modules] live reload: " + e.Message, ConsoleColor.Red); }
            finally { Volatile.Write(ref busy, 0); }
        }

        /// <summary>The result in the chat of every online GM (max 8 lines).</summary>
        public static void TellGms(List<string> lines)
        {
            try
            {
                foreach (var c in Database.Server.GamePoll.Values.ToArray())
                    if (c != null && c.Player != null && c.FullLoading && Commands.CommandRegistry.CanUse(c, Commands.CommandRole.GM))
                        foreach (var l in lines.Take(8))
                            c.SendSysMesage(l, Game.MsgServer.MsgMessage.ChatMode.System);
            }
            catch { }
        }
    }
}
