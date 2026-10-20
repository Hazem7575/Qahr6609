// ============================================================================
//  HotReload - apply edits of the server data files to the RUNNING server, without a restart and without
//  players relogging (same design as Qahr's HotReload).
//
//  What can be reloaded (each one is a console command and an in-game @command for project managers):
//      reloadnpcs      npcs.txt          -> NpcHotReload (diff + despawn/spawn for the players in view)
//      reloadportals   portals.ini       -> every map's portal list is replaced
//      reloadshops     shops\*           -> Shop.dat, emoneyshop.ini + emoneyshopV2.ini, Honor/RacePoint/GoldenLeague
//      reloaditems     itemtype.txt      -> values of EXISTING item types updated in place (never players' items)
//      reloadmonsters  Monsters\*.ini    -> monster type values updated in place (+ spawned monsters' stats)
//      reloadmapcells  map\map\*.dmap    -> server-side walkability of ONE map (arg: map id), map\walk included
//      reloadquests    QuestEngine data  -> the QuestEngine module re-reads its quests (same as @quest reload)
//      reloadall       npcs + portals + shops + items + monsters
//  Optional auto-reload: ServerDataBase\hotreload.txt (watch_npcs=1, watch_portals=1, ... debounce_ms=1000).
//
//  Safety rules:
//  - One reload at a time (lock Sync). Every reload is wrapped: an error aborts it and is logged; it never
//    throws into the caller (console / packet thread / file watcher).
//  - Files are read with ReadStable(): size + md5 identical on two reads a moment apart (a tool that is still
//    writing the file makes the reload retry, then give up). A file that does not parse aborts the WHOLE reload
//    before anything is changed.
//  - Shared tables are replaced by publishing a fully built new object (reference swap) or by updating fields of
//    existing objects in place; no Dictionary that packet threads read is mutated while they may read it.
//  - Player data is never touched.
//  Every reload is appended to ServerDataBase\hotreload_log.txt.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Conquer6609.HotReload
{
    /// <summary>An expected failure (bad file, file still being written...). Its message is shown to the user.</summary>
    public sealed class HotReloadError : Exception
    {
        public HotReloadError(string message) : base(message) { }
    }

    /// <summary>The outcome of one reload: a one-line summary plus counts and notes.</summary>
    public sealed class ReloadResult
    {
        public string Op;
        public bool Ok = true;
        public string Message = "";
        public string Error;
        public long ElapsedMs;
        public readonly Dictionary<string, int> Counts = new Dictionary<string, int>();
        public readonly List<string> Notes = new List<string>();

        public ReloadResult(string op) { Op = op; }

        public void Add(string key, int n = 1)
        {
            int v;
            Counts.TryGetValue(key, out v);
            Counts[key] = v + n;
        }
        public int Get(string key)
        {
            int v;
            return Counts.TryGetValue(key, out v) ? v : 0;
        }
        public void Note(string text)
        {
            if (Notes.Count < 60)
                Notes.Add(text);
            else if (Notes.Count == 60)
                Notes.Add("... (more notes are cut)");
        }
    }

    public static class HotReloadManager
    {
        /// <summary>Only one reload runs at a time (console, GM command and the file watcher share it).</summary>
        public static readonly object Sync = new object();
        private static readonly object _logLock = new object();

        public static readonly string[] Ops = { "reloadnpcs", "reloadportals", "reloadshops", "reloaditems", "reloadmonsters", "reloadmapcells", "reloadquests", "reloadall" };

        public static bool IsReloadOp(string op)
        {
            op = (op ?? "").Trim().ToLowerInvariant();
            if (op.StartsWith("@")) op = op.Substring(1);
            return Array.IndexOf(Ops, op) >= 0;
        }

        internal static string DbFile(string name)
        {
            string db = Program.ServerConfig.DbLocation ?? "";
            if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
            return db + name;
        }

        // ------------------------------------------------------------------ run
        /// <summary>Runs one reload op ("reloadnpcs", "npcs", ...). Never throws. arg = map id for reloadmapcells,
        /// force = allow an NPC reload that removes more than half of the NPCs that came from npcs.txt.</summary>
        public static ReloadResult Run(string op, string arg = null, bool force = false)
        {
            op = (op ?? "").Trim().ToLowerInvariant();
            if (op.StartsWith("@")) op = op.Substring(1);
            if (!op.StartsWith("reload")) op = "reload" + op;
            if (op == "reloadall")
                return RunAll(force);

            var result = new ReloadResult(op);
            var clock = System.Diagnostics.Stopwatch.StartNew();
            bool taken = false;
            try
            {
                Monitor.TryEnter(Sync, 60000, ref taken);
                if (!taken)
                    throw new HotReloadError("another reload is still running; try again in a moment.");
                switch (op)
                {
                    case "reloadnpcs": NpcHotReload.Reload(result, force); break;
                    case "reloadportals": DataHotReload.ReloadPortals(result); break;
                    case "reloadshops": DataHotReload.ReloadShops(result); break;
                    case "reloaditems": DataHotReload.ReloadItems(result); break;
                    case "reloadmonsters": DataHotReload.ReloadMonsters(result); break;
                    case "reloadquests":
                        Modules.QuestEngine.QuestEngine.Init();
                        result.Message = "QuestEngine re-read its quest files (see its console line).";
                        break;
                    case "reloadmapcells":
                        {
                            uint map;
                            if (!uint.TryParse((arg ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out map) || map == 0)
                                throw new HotReloadError("usage: reloadmapcells <map id>");
                            DataHotReload.ReloadMapCells(result, map);
                            break;
                        }
                    default:
                        throw new HotReloadError("unknown reload '" + op + "'. Use: " + string.Join(", ", Ops));
                }
            }
            catch (HotReloadError e)
            {
                result.Ok = false;
                result.Error = e.Message;
            }
            catch (Exception e)
            {
                result.Ok = false;
                result.Error = "server error: " + e.Message;
                try { MyConsole.SaveException(e); } catch { }
            }
            finally
            {
                if (taken) Monitor.Exit(Sync);
            }
            result.ElapsedMs = clock.ElapsedMilliseconds;
            Report(result);
            return result;
        }

        private static ReloadResult RunAll(bool force)
        {
            var all = new ReloadResult("reloadall");
            var clock = System.Diagnostics.Stopwatch.StartNew();
            var parts = new List<string>();
            foreach (string op in new[] { "reloadnpcs", "reloadportals", "reloadshops", "reloaditems", "reloadmonsters" })
            {
                ReloadResult r = Run(op, null, force);
                foreach (var kv in r.Counts) all.Add(op.Substring(6) + "." + kv.Key, kv.Value);
                if (r.Ok) parts.Add(op.Substring(6) + ": " + r.Message);
                else
                {
                    all.Ok = false;
                    parts.Add(op.Substring(6) + ": FAILED - " + r.Error);
                }
                foreach (string n in r.Notes) all.Note(op.Substring(6) + ": " + n);
            }
            all.Message = string.Join(" | ", parts);
            if (!all.Ok) all.Error = all.Message;
            all.ElapsedMs = clock.ElapsedMilliseconds;
            return all;
        }

        // ------------------------------------------------------------------ console / log
        private static void Report(ReloadResult r)
        {
            try
            {
                string head = "[HotReload] " + r.Op + ": " + (r.Ok ? r.Message : "FAILED - " + r.Error) + " (" + r.ElapsedMs + " ms)";
                MyConsole.WriteLine(head, r.Ok ? ConsoleColor.Green : ConsoleColor.Yellow);
                for (int i = 0; i < r.Notes.Count && i < 8; i++)
                    MyConsole.WriteLine("    " + r.Notes[i], ConsoleColor.DarkGray);
                if (r.Notes.Count > 8)
                    MyConsole.WriteLine("    ... " + (r.Notes.Count - 8) + " more in hotreload_log.txt", ConsoleColor.DarkGray);
            }
            catch { }
            try
            {
                var sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
                  .Append(" | ").Append(r.Ok ? "OK  " : "FAIL").Append(" | ").Append(r.Op).Append(" | ")
                  .Append(r.Ok ? r.Message : r.Error).Append(" | ").Append(r.ElapsedMs).Append(" ms").Append(Environment.NewLine);
                foreach (string n in r.Notes)
                    sb.Append("        ").Append(n.Replace("\r", " ").Replace("\n", " ")).Append(Environment.NewLine);
                lock (_logLock)
                    File.AppendAllText(DbFile("hotreload_log.txt"), sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }

        // ------------------------------------------------------------------ file helpers
        /// <summary>Reads a whole file only when it is stable: the same length and md5 on two reads 150 ms apart.
        /// Retries while a tool is still writing it; throws HotReloadError when it never settles or is missing.</summary>
        public static byte[] ReadStable(string path, bool allowEmpty = false)
        {
            if (!File.Exists(path))
                throw new HotReloadError(Path.GetFileName(path) + " not found (" + path + ").");
            string lastError = null;
            for (int attempt = 0; attempt < 6; attempt++)
            {
                try
                {
                    byte[] a = ReadShared(path);
                    Thread.Sleep(150);
                    byte[] b = ReadShared(path);
                    if (a.Length == b.Length && Md5(a) == Md5(b))
                    {
                        if (a.Length == 0 && !allowEmpty)
                            throw new HotReloadError(Path.GetFileName(path) + " is empty - not applied (a tool may be in the middle of saving it).");
                        return a;
                    }
                    lastError = "the file changed while it was being read";
                }
                catch (HotReloadError) { throw; }
                catch (IOException e) { lastError = e.Message; }
                catch (UnauthorizedAccessException e) { lastError = e.Message; }
                Thread.Sleep(300);
            }
            throw new HotReloadError(Path.GetFileName(path) + " is still being written or is locked (" + lastError + "); nothing was changed. Try again.");
        }

        private static byte[] ReadShared(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var ms = new MemoryStream(fs.CanSeek ? (int)Math.Min(fs.Length, int.MaxValue) : 4096))
            {
                fs.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static string Md5(byte[] data)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
                return BitConverter.ToString(md5.ComputeHash(data));
        }

        /// <summary>Text reader over file bytes, decoded like File.OpenText / File.ReadAllLines (UTF-8, BOM aware) -
        /// the way the boot loaders of these files read them.</summary>
        public static StreamReader TextOf(byte[] data)
        {
            return new StreamReader(new MemoryStream(data, false), Encoding.UTF8, true);
        }

        /// <summary>Waits until the file is stable (see ReadStable) without keeping its bytes.</summary>
        public static void WaitStable(string path, bool allowEmpty = true)
        {
            ReadStable(path, allowEmpty);
        }

        /// <summary>Ids of the maps the server has now (all maps load at start-up; a few are added later).</summary>
        public static List<uint> LoadedMapIds()
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    return Database.Server.ServerMaps.Keys.ToList();
                }
                catch (InvalidOperationException) { Thread.Sleep(20); }// a map was added meanwhile
            }
            return new List<uint>();
        }

        public static Role.GameMap LoadedMap(uint id)
        {
            Role.GameMap map;
            return Database.Server.ServerMaps.TryGetValue(id, out map) ? map : null;
        }

        // ------------------------------------------------------------------ watcher (optional)
        private static readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
        private static readonly Dictionary<string, Timer> _debounce = new Dictionary<string, Timer>();
        private static int _debounceMs = 1000;

        /// <summary>Reads ServerDataBase\hotreload.txt (optional). Everything is OFF by default:
        ///   watch_npcs=1  watch_portals=1  watch_shops=1  watch_items=1  watch_monsters=1  debounce_ms=1000
        /// When a watched file changes, the matching reload runs ~debounce_ms after the last change.</summary>
        public static void Start()
        {
            try
            {
                string cfg = DbFile("hotreload.txt");
                if (!File.Exists(cfg))
                {
                    MyConsole.WriteLine("[HotReload] ready (console / @: " + string.Join(", ", Ops) + "). Auto-watch off.", ConsoleColor.DarkGray);
                    return;
                }
                var on = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string raw in File.ReadAllLines(cfg))
                {
                    string line = raw.Trim();
                    int eq = line.IndexOf('=');
                    if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";") || eq < 0) continue;
                    string k = line.Substring(0, eq).Trim().ToLowerInvariant();
                    string v = line.Substring(eq + 1).Trim().ToLowerInvariant();
                    bool yes = v == "1" || v == "true" || v == "yes" || v == "on";
                    if (k == "debounce_ms") { int ms; if (int.TryParse(v, out ms)) _debounceMs = Math.Max(300, Math.Min(10000, ms)); }
                    else if (k.StartsWith("watch_") && yes) on.Add(k.Substring(6));
                }
                string db = DbFile("");
                if (on.Contains("npcs")) Watch(db, "npcs.txt", "reloadnpcs");
                if (on.Contains("portals")) Watch(db, "portals.ini", "reloadportals");
                if (on.Contains("shops")) Watch(db + "shops", "*.*", "reloadshops");
                if (on.Contains("items")) Watch(db, "itemtype.txt", "reloaditems");
                if (on.Contains("monsters")) Watch(db + "Monsters", "*.ini", "reloadmonsters");
                MyConsole.WriteLine("[HotReload] auto-watch: " + (on.Count == 0 ? "off" : string.Join(", ", on)) + " (debounce " + _debounceMs + " ms).", ConsoleColor.Cyan);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[HotReload] watcher not started: " + e.Message, ConsoleColor.Yellow);
            }
        }

        private static void Watch(string folder, string filter, string op)
        {
            if (!Directory.Exists(folder)) return;
            var w = new FileSystemWatcher(folder, filter);
            w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
            FileSystemEventHandler h = (s, e) =>
            {
                // tools write backups next to the file (npcs.txt.bak_...) - only the data file counts
                string name = e.Name ?? "";
                if (name.IndexOf(".bak", StringComparison.OrdinalIgnoreCase) >= 0 || name.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase))
                    return;
                Schedule(op);
            };
            w.Changed += h;
            w.Created += h;
            w.Renamed += (s, e) => h(s, e);
            w.EnableRaisingEvents = true;
            _watchers.Add(w);
        }

        private static void Schedule(string op)
        {
            lock (_debounce)
            {
                Timer t;
                if (_debounce.TryGetValue(op, out t))
                {
                    t.Change(_debounceMs, Timeout.Infinite);
                    return;
                }
                t = new Timer(_ =>
                {
                    try { Run(op); }
                    catch { }
                }, null, _debounceMs, Timeout.Infinite);
                _debounce[op] = t;
            }
        }

        // ------------------------------------------------------------------ in-game command
        /// <summary>For the @reload... chat command: runs the reload off the player's socket thread and sends the
        /// summary back to that player.</summary>
        public static void RunForPlayer(Client.GameClient client, string op, string arg, bool force)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                ReloadResult r = Run(op, arg, force);
                try
                {
                    if (client != null && client.Socket != null && client.Socket.Alive)
                    {
                        client.SendSysMesage("[HotReload] " + r.Op + ": " + (r.Ok ? r.Message : "FAILED - " + r.Error));
                        for (int i = 0; i < r.Notes.Count && i < 4; i++)
                            client.SendSysMesage("  " + r.Notes[i]);
                    }
                }
                catch { }
            });
        }
    }
}
