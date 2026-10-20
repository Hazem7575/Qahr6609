// Module discovery + hook dispatch.
//
// Wiring (done by the core hook owner, not by modules):
//   startup, after Database.Server.LoadDatabase() ...... ModuleManager.Initialize();
//   KernelThread (1 s) .................................. ModuleManager.Tick(clock);
//   periodic world save (any thread) .................... ModuleManager.Save();
//   server shutdown ..................................... ModuleManager.Shutdown();
//   after CompleteLogin ................................. ModuleManager.Login(client);
//   on disconnect/logout ................................ ModuleManager.Logout(client);
//   walk / steed walk / jump, before applying ........... if (!ModuleManager.AllowMove(client, x, y)) return;
//   player -> monster damage ............................ damage = ModuleManager.MonsterDamage(client, monster, damage);
//   monster died, killer is a player .................... ModuleManager.MonsterKilled(client, monster);
//   start of every Player.Dead (killer may be null) ..... ModuleManager.PlayerKilled(killer, victim);
//   item use, before the default handling ............... if (ModuleManager.ItemUse(client, item)) return;
//   NPC click/answer, before the NPC script ............. int mh = ModuleManager.NpcTalk(client, stream, option, npcId); (Procesor.ExecuteNpcInvoker)
//
// Every dispatch method is allocation free and returns immediately when no module implements that hook
// (one array length check). Exceptions thrown by a module are caught, logged to the module log, printed
// at most once per 30 s per module, and after FaultLimit exceptions within FaultWindowMs the module is
// marked faulted: its gameplay hooks (tick, login, move, damage, kills, item use) are skipped from then on,
// but OnSave / OnLogout / OnShutdown still run so its data is not lost. Its commands, NPCs and packet
// handlers stay registered.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Conquer6609.Framework.Modules
{
    public sealed class ModuleInfo
    {
        public string Name { get; internal set; }
        public string Version { get; internal set; }
        public int Priority { get; internal set; }
        public string TypeName { get; internal set; }
        /// <summary>"ok", "faulted" or "load-failed".</summary>
        public string Status { get; internal set; }
        public bool Faulted { get; internal set; }
        public int ErrorCount { get; internal set; }
        public string LastError { get; internal set; }
        /// <summary>Comma separated list of the hooks this module overrides.</summary>
        public string Hooks { get; internal set; }
    }

    public static class ModuleManager
    {
        public const int FaultLimit = 100;         // exceptions ...
        public const int FaultWindowMs = 60000;    // ... within this window => module faulted
        const int ConsoleErrorIntervalMs = 30000;  // max one console line per module per 30 s

        static readonly ModuleBase[] Empty = new ModuleBase[0];

        // per-hook arrays (copy-on-write; replaced when a module faults)
        static volatile ModuleBase[] hTick = Empty, hLogin = Empty, hLogout = Empty, hSave = Empty, hShutdown = Empty,
            hMove = Empty, hMonsterDamage = Empty, hMonsterKilled = Empty, hPlayerKilled = Empty, hItemUse = Empty, hNpcTalk = Empty;
        static volatile ModuleBase[] all = Empty;      // every module that loaded successfully, priority order
        static volatile ModuleBase[] failed = Empty;   // modules whose ctor/OnLoad failed (reporting only)

        static readonly object initLock = new object();
        static readonly object rebuildLock = new object();
        static readonly object saveLock = new object();
        static bool initialized;
        static int shutdownDone;

        /// <summary>True after Initialize() ran.</summary>
        public static bool Initialized { get { return initialized; } }
        /// <summary>Number of modules that loaded successfully (faulted ones included).</summary>
        public static int Count { get { return all.Length; } }

        // ================================================================== init
        /// <summary>Discovers, instantiates and loads every module (once; later calls do nothing).
        /// Call after the server data is loaded.</summary>
        public static void Initialize()
        {
            lock (initLock)
            {
                if (initialized) return;
                initialized = true;
                // runtime modules (ServerDataBase\Modules\<Name>\*.cs) are compiled / loaded first, so their commands,
                // packet handlers, NPC scripts and module classes are collected together with the server's
                try { RuntimeModules.LoadAll(); }
                catch (Exception e) { Print("[Modules] runtime modules failed: " + e.Message, ConsoleColor.Red); }
                RegisterRuntimeHandlers();
                // the game data the modules bring in their content\ folders (items, maps, NPCs, monsters, spawns, portals)
                try { ModuleContent.LoadAll(); }
                catch (Exception e) { Print("[Modules] module content failed: " + e.Message, ConsoleColor.Red); }
                try { Commands.CommandRegistry.Initialize(); }
                catch (Exception e) { Print("[Modules] command registry failed: " + e.Message, ConsoleColor.Red); }
                ReportRuntimeCommands();

                var created = new List<ModuleBase>();
                var bad = new List<ModuleBase>();
                foreach (var t in DiscoverTypes())
                {
                    try
                    {
                        var ctor = t.GetConstructor(Type.EmptyTypes);
                        if (ctor == null)
                        {
                            Print("[Modules] FAILED " + t.FullName + ": no public parameterless constructor", ConsoleColor.Red);
                            continue;
                        }
                        var m = (ModuleBase)ctor.Invoke(null);
                        if (string.IsNullOrEmpty(m.Name))
                        {
                            Print("[Modules] FAILED " + t.FullName + ": Name is empty", ConsoleColor.Red);
                            continue;
                        }
                        created.Add(m);
                    }
                    catch (Exception e)
                    {
                        var inner = e is TargetInvocationException && e.InnerException != null ? e.InnerException : e;
                        Print("[Modules] FAILED " + t.FullName + ": " + inner.GetType().Name + ": " + inner.Message, ConsoleColor.Red);
                    }
                }

                created.Sort(CompareModules);

                var loaded = new List<ModuleBase>();
                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var m in created)
                {
                    if (!names.Add(m.Name))
                    {
                        Print("[Modules] FAILED " + m.Name + " (" + m.GetType().FullName + "): duplicate module name, skipped", ConsoleColor.Red);
                        continue;
                    }
                    m.HookList = DescribeHooks(m.GetType());
                    try
                    {
                        m.OnLoad();
                        loaded.Add(m);
                    }
                    catch (Exception e)
                    {
                        m.LoadFailed = true;
                        m.LastError = "OnLoad: " + e.GetType().Name + ": " + e.Message;
                        Interlocked.Increment(ref m.ErrorCount);
                        m.Log("[ERROR] OnLoad failed: " + e);
                        bad.Add(m);
                        Print("[Modules] FAILED " + m.Name + " v" + SafeVersion(m) + ": OnLoad: " + e.GetType().Name + ": " + e.Message, ConsoleColor.Red);
                    }
                }

                all = loaded.ToArray();
                failed = bad.ToArray();
                Rebuild();

                var sb = new System.Text.StringBuilder("[Modules] Loaded " + loaded.Count);
                for (int i = 0; i < loaded.Count; i++)
                    sb.Append(i == 0 ? ": " : ", ").Append(loaded[i].Name).Append(" v").Append(SafeVersion(loaded[i]));
                Print(sb.ToString(), ConsoleColor.Green);
            }
            try { ModuleWatcher.Sync(); } catch { }
        }

        /// <summary>Adds the [PacketAttribute] / [NpcAttribute] / [DataAttribute] handlers of every runtime module.
        /// A packet / NPC / action the server or another module already handles is kept and reported.</summary>
        static void RegisterRuntimeHandlers()
        {
            foreach (var pkg in RuntimeModules.Packages)
            {
                if (pkg.Assembly == null) continue;
                var conflicts = new List<string>();
                try
                {
                    if (Program.MsgInvoker != null) conflicts.AddRange(Program.MsgInvoker.AddAssembly(pkg.Assembly, pkg.Name));
                    conflicts.AddRange(Game.MsgNpc.Procesor.invoker.AddAssembly(pkg.Assembly, pkg.Name));
                    conflicts.AddRange(Game.MsgServer.MsgDataPacket.invoker.AddAssembly(pkg.Assembly, pkg.Name));
                }
                catch (Exception e) { conflicts.Add("handlers not registered: " + e.Message); }
                foreach (string c in conflicts)
                    Print("[Modules] " + pkg.Name + ": " + c, ConsoleColor.Yellow);
                int packets = Program.MsgInvoker != null ? Program.MsgInvoker.CountOwner(pkg.Name) : 0;
                Print("[Modules] runtime module " + pkg.Name + (pkg.FromCache ? " loaded from cache" : " compiled") + " in " + pkg.CompileMs + " ms (packets " + packets
                    + ", NPC scripts " + Game.MsgNpc.Procesor.invoker.CountOwner(pkg.Name) + ", actions " + Game.MsgServer.MsgDataPacket.invoker.CountOwner(pkg.Name) + ")", ConsoleColor.Cyan);
            }
        }

        static void ReportRuntimeCommands()
        {
            foreach (var pkg in RuntimeModules.Packages)
            {
                if (pkg.Assembly == null) continue;
                int n = 0;
                try
                {
                    foreach (var t in pkg.Assembly.GetTypes())
                        foreach (var m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                            foreach (Commands.GmCommandAttribute a in m.GetCustomAttributes(typeof(Commands.GmCommandAttribute), false))
                            {
                                var info = a.Names.Length > 0 ? Commands.CommandRegistry.Find(a.Names[0]) : null;
                                if (info != null && info.Handler != null && info.Handler.Method.Module.Assembly == pkg.Assembly) n++;
                            }
                }
                catch { }
                if (n > 0) Print("[Modules] " + pkg.Name + ": " + n + " GM command(s) registered", ConsoleColor.Cyan);
            }
        }

        static IEnumerable<Type> DiscoverTypes()
        {
            var typeList = new List<Type>();
            try { typeList.AddRange(Assembly.GetExecutingAssembly().GetTypes()); }
            catch (ReflectionTypeLoadException e) { typeList.AddRange(e.Types); }
            foreach (var pkg in RuntimeModules.Packages)
            {
                if (pkg.Assembly == null) continue;
                try { typeList.AddRange(pkg.Assembly.GetTypes()); }
                catch (ReflectionTypeLoadException e) { typeList.AddRange(e.Types); }
            }
            var types = typeList.ToArray();
            var result = new List<Type>();
            foreach (var t in types)
            {
                try
                {
                    if (t == null || !t.IsClass || t.IsAbstract || t.ContainsGenericParameters) continue;
                    if (!t.IsSubclassOf(typeof(ModuleBase))) continue;
                    result.Add(t);
                }
                catch { } // a type whose dependencies can't load is never a module
            }
            return result;
        }

        static int CompareModules(ModuleBase a, ModuleBase b)
        {
            int pa = SafePriority(a), pb = SafePriority(b);
            if (pa != pb) return pa.CompareTo(pb);
            int c = string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
            if (c != 0) return c;
            return string.CompareOrdinal(a.GetType().FullName, b.GetType().FullName);
        }

        static int SafePriority(ModuleBase m) { try { return m.Priority; } catch { return 0; } }
        static string SafeVersion(ModuleBase m) { try { return m.Version ?? "?"; } catch { return "?"; } }

        static readonly string[] HookNames =
        {
            "OnTick", "OnLogin", "OnLogout", "OnSave", "OnShutdown", "OnMove",
            "OnMonsterDamage", "OnMonsterKilled", "OnPlayerKilled", "OnItemUse", "OnNpcTalk"
        };

        static bool Overrides(Type t, string method)
        {
            var baseMethod = typeof(ModuleBase).GetMethod(method, BindingFlags.Public | BindingFlags.Instance);
            if (baseMethod == null) return false;
            var ps = baseMethod.GetParameters();
            var types = new Type[ps.Length];
            for (int i = 0; i < ps.Length; i++) types[i] = ps[i].ParameterType;
            var m = t.GetMethod(method, BindingFlags.Public | BindingFlags.Instance, null, types, null);
            return m != null && m.DeclaringType != typeof(ModuleBase) && m.GetBaseDefinition().DeclaringType == typeof(ModuleBase);
        }

        static string DescribeHooks(Type t)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var h in HookNames)
                if (Overrides(t, h))
                    sb.Append(sb.Length == 0 ? "" : ",").Append(h.Substring(2));
            return sb.ToString();
        }

        /// <summary>Recomputes the per-hook arrays from `all`, skipping faulted modules.</summary>
        static void Rebuild()
        {
            lock (rebuildLock)
            {
                var src = all;
                // gameplay hooks skip faulted modules; logout/save/shutdown keep them (persist their data)
                hTick = Filter(src, "OnTick", true);
                hLogin = Filter(src, "OnLogin", true);
                hLogout = Filter(src, "OnLogout", false);
                hSave = Filter(src, "OnSave", false);
                hMove = Filter(src, "OnMove", true);
                hMonsterDamage = Filter(src, "OnMonsterDamage", true);
                hMonsterKilled = Filter(src, "OnMonsterKilled", true);
                hPlayerKilled = Filter(src, "OnPlayerKilled", true);
                hItemUse = Filter(src, "OnItemUse", true);
                hNpcTalk = Filter(src, "OnNpcTalk", true);
                // shutdown: every loaded module (OnShutdown is cheap to call even if empty), reverse priority order
                var sd = new List<ModuleBase>();
                for (int i = src.Length - 1; i >= 0; i--)
                    if (Overrides(src[i].GetType(), "OnShutdown")) sd.Add(src[i]);
                hShutdown = sd.ToArray();
            }
        }

        static ModuleBase[] Filter(ModuleBase[] src, string hook, bool skipFaulted)
        {
            var list = new List<ModuleBase>();
            foreach (var m in src)
                if ((!skipFaulted || !m.Faulted) && Overrides(m.GetType(), hook)) list.Add(m);
            return list.Count == 0 ? Empty : list.ToArray();
        }

        // ================================================================== error handling
        static void Fail(ModuleBase m, string hook, Exception e)
        {
            try
            {
                Interlocked.Increment(ref m.ErrorCount);
                m.LastError = hook + ": " + e.GetType().Name + ": " + e.Message;
                m.Log("[ERROR] " + hook + ": " + e);

                int now = Environment.TickCount;
                long nowTicks = DateTime.UtcNow.Ticks;
                long last = Interlocked.Read(ref m.LastConsoleError);
                if (last == 0 || nowTicks - last >= ConsoleErrorIntervalMs * TimeSpan.TicksPerMillisecond || nowTicks < last)
                {
                    if (Interlocked.CompareExchange(ref m.LastConsoleError, nowTicks, last) == last)
                        Print("[Modules] " + m.Name + "." + hook + " error: " + e.GetType().Name + ": " + e.Message
                                + " (total " + m.ErrorCount + ", see module log)", ConsoleColor.Red);
                }

                bool fault = false;
                lock (m.ErrorLock)
                {
                    if (m.Faulted) return;
                    var w = m.ErrorWindow;
                    w[m.ErrorWindowPos] = now;
                    m.ErrorWindowPos = (m.ErrorWindowPos + 1) % w.Length;
                    if (m.ErrorWindowFill < w.Length) m.ErrorWindowFill++;
                    // after the write, ErrorWindowPos points at the oldest of the last FaultLimit exceptions
                    if (m.ErrorWindowFill >= w.Length && unchecked(now - (int)w[m.ErrorWindowPos]) <= FaultWindowMs)
                    {
                        m.Faulted = true;
                        fault = true;
                    }
                }
                if (fault)
                {
                    Rebuild();
                    string text = FaultLimit + " exceptions within " + (FaultWindowMs / 1000) + " s, module disabled until restart. Last: " + m.LastError;
                    Print("[Modules] " + m.Name + " FAULTED: " + text, ConsoleColor.Red);
                    m.Log("[FAULTED] " + text);
                    Conquer6609.Security.Guard.Audit("module-faulted", 0, m.Name, text);
                }
            }
            catch { }
        }

        static void Print(string line, ConsoleColor color)
        {
            try { MyConsole.WriteLine(line, color); }
            catch { try { System.Console.WriteLine(line); } catch { } }
        }

        // ================================================================== hook dispatch
        public static void Tick(Extensions.Time32 clock)
        {
            var a = hTick;
            if (a.Length == 0 || Volatile.Read(ref shutdownDone) != 0) return;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try { m.OnTick(clock); }
                catch (Exception e) { Fail(m, "OnTick", e); }
            }
        }

        public static void Login(Client.GameClient client)
        {
            var a = hLogin;
            if (a.Length == 0) return;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try { m.OnLogin(client); }
                catch (Exception e) { Fail(m, "OnLogin", e); }
            }
        }

        public static void Logout(Client.GameClient client)
        {
            var a = hLogout;
            if (a.Length == 0) return;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try { m.OnLogout(client); }
                catch (Exception e) { Fail(m, "OnLogout", e); }
            }
        }

        /// <summary>Calls OnSave of every module (faulted ones too). Thread safe; concurrent Save() calls are
        /// serialized, and Save() does nothing once Shutdown() has started.</summary>
        public static void Save()
        {
            if (hSave.Length == 0 || Volatile.Read(ref shutdownDone) != 0) return;
            lock (saveLock)
            {
                if (Volatile.Read(ref shutdownDone) != 0) return;
                var a = hSave;
                for (int i = 0; i < a.Length; i++)
                {
                    var m = a[i];
                    try { m.OnSave(); }
                    catch (Exception e) { Fail(m, "OnSave", e); }
                }
            }
        }

        /// <summary>Calls OnShutdown (reverse priority order) once, then flushes the module logs (max 3 s).
        /// Runs under the same lock as Save(), so it never overlaps a module save; Save() and Tick() do
        /// nothing afterwards.</summary>
        public static void Shutdown()
        {
            if (Interlocked.Exchange(ref shutdownDone, 1) != 0) return;
            lock (saveLock)
            {
                var a = hShutdown;
                for (int i = 0; i < a.Length; i++)
                {
                    var m = a[i];
                    try { m.OnShutdown(); }
                    catch (Exception e) { Fail(m, "OnShutdown", e); }
                }
            }
            if (all.Length != 0 || failed.Length != 0)
                ModuleLog.Flush(3000);
        }

        /// <summary>Returns false when a module blocked the step (the module corrects the client position).
        /// A module that throws does not block.</summary>
        public static bool AllowMove(Client.GameClient client, ushort x, ushort y)
        {
            var a = hMove;
            if (a.Length == 0) return true;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try
                {
                    if (!m.OnMove(client, x, y)) return false;
                }
                catch (Exception e) { Fail(m, "OnMove", e); }
            }
            return true;
        }

        /// <summary>Passes the damage through every module (in priority order) and returns the result.</summary>
        public static uint MonsterDamage(Client.GameClient attacker, Game.MsgMonster.MonsterRole target, uint damage)
        {
            var a = hMonsterDamage;
            if (a.Length == 0) return damage;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try { damage = m.OnMonsterDamage(attacker, target, damage); }
                catch (Exception e) { Fail(m, "OnMonsterDamage", e); }
            }
            return damage;
        }

        public static void MonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            var a = hMonsterKilled;
            if (a.Length == 0) return;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try { m.OnMonsterKilled(killer, monster); }
                catch (Exception e) { Fail(m, "OnMonsterKilled", e); }
            }
        }

        /// <summary>killer may be null.</summary>
        public static void PlayerKilled(Role.Player killer, Role.Player victim)
        {
            var a = hPlayerKilled;
            if (a.Length == 0) return;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try { m.OnPlayerKilled(killer, victim); }
                catch (Exception e) { Fail(m, "OnPlayerKilled", e); }
            }
        }

        /// <summary>Returns true when a module handled the item use (skip the default processing).</summary>
        public static bool ItemUse(Client.GameClient client, Game.MsgServer.MsgGameItem item)
        {
            var a = hItemUse;
            if (a.Length == 0) return false;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try
                {
                    if (m.OnItemUse(client, item)) return true;
                }
                catch (Exception e) { Fail(m, "OnItemUse", e); }
            }
            return false;
        }

        /// <summary>NPC dialog hook (R1): the first module (priority order) returning non-zero wins.
        /// 0 = run the NPC script as usual, 1 = handled (skip the script), 2 = run the NPC script with option 0.
        /// A module that throws counts as 0.</summary>
        public static int NpcTalk(Client.GameClient client, ServerSockets.Packet stream, byte option, uint npcId)
        {
            var a = hNpcTalk;
            if (a.Length == 0) return 0;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i];
                try
                {
                    int r = m.OnNpcTalk(client, stream, option, npcId);
                    if (r == 1 || r == 2) return r;
                }
                catch (Exception e) { Fail(m, "OnNpcTalk", e); }
            }
            return 0;
        }

        // ================================================================== hot reload
        static readonly object reloadLock = new object();

        /// <summary>
        /// Hot reload of a runtime module (ServerDataBase\Modules\&lt;name&gt;\): compiles its code again and, only when it
        /// compiles, swaps it in while the server runs: OnSave + OnShutdown of the old instance, its packet / NPC /
        /// action handlers and GM commands replaced by the new code's, new instance created and OnLoad. A module that
        /// was not loaded yet (new folder) is loaded the same way. Modules compiled into GameServer.exe can not be
        /// reloaded (their code is the server's). The module must clean up in OnShutdown what it put in the world
        /// (NPCs, monsters) and load its state again in OnLoad; static fields of the old code are not carried over.
        /// Returns the lines to show.
        /// </summary>
        public static List<string> ReloadRuntime(string name)
        {
            var lines = new List<string>();
            lock (reloadLock)
            {
                var builtIn = Find(name);
                if (builtIn != null && builtIn.GetType().Assembly == Assembly.GetExecutingAssembly())
                {
                    lines.Add(builtIn.Name + " is compiled into GameServer.exe: only modules in ServerDataBase\\Modules\\" + builtIn.Name + " (runtime) can be reloaded.");
                    return lines;
                }
                var clock = System.Diagnostics.Stopwatch.StartNew();
                var loadedPkg = RuntimeModules.Find(name);
                if (loadedPkg != null && !RuntimeModules.HasSource(name))
                {
                    lines.AddRange(UnloadRuntime(loadedPkg));// folder deleted (or renamed to _Name): the module leaves the server
                    foreach (var l in lines) Print(l, ConsoleColor.Cyan);
                    return lines;
                }
                string error;
                var now = RuntimeModules.Compile(name, out error);
                if (now == null)
                {
                    lines.Add("[Modules] " + name + " NOT reloaded, the running code stays:");
                    foreach (var l in (error ?? "").Split('\n'))
                        if (l.Trim().Length > 0) lines.Add(l.TrimEnd());
                    foreach (var l in lines) Print(l, ConsoleColor.Red);
                    return lines;
                }
                var old = RuntimeModules.Find(now.Name);

                // 1. the old instances: save, shut down, out of every hook
                var keep = new List<ModuleBase>();
                var gone = new List<ModuleBase>();
                foreach (var m in all)
                    (old != null && old.Assembly != null && m.GetType().Assembly == old.Assembly ? gone : keep).Add(m);
                foreach (var m in gone)
                {
                    try { m.OnSave(); } catch (Exception e) { lines.Add(m.Name + ".OnSave: " + e.Message); }
                    try { m.OnShutdown(); } catch (Exception e) { lines.Add(m.Name + ".OnShutdown: " + e.Message); }
                }
                all = keep.ToArray();
                var badKeep = new List<ModuleBase>();
                foreach (var m in failed)
                    if (old == null || old.Assembly == null || m.GetType().Assembly != old.Assembly) badKeep.Add(m);
                failed = badKeep.ToArray();
                Rebuild();

                // 2. handlers and commands of the new code
                string owner = now.Name;
                try
                {
                    if (Program.MsgInvoker != null) { Program.MsgInvoker.RemoveOwner(owner); foreach (var c in Program.MsgInvoker.AddAssembly(now.Assembly, owner)) lines.Add(c); }
                    Game.MsgNpc.Procesor.invoker.RemoveOwner(owner);
                    foreach (var c in Game.MsgNpc.Procesor.invoker.AddAssembly(now.Assembly, owner)) lines.Add(c);
                    Game.MsgServer.MsgDataPacket.invoker.RemoveOwner(owner);
                    foreach (var c in Game.MsgServer.MsgDataPacket.invoker.AddAssembly(now.Assembly, owner)) lines.Add(c);
                }
                catch (Exception e) { lines.Add("handlers: " + e.Message); }
                RuntimeModules.Replace(old, now);
                try { Commands.CommandRegistry.Rescan(); }
                catch (Exception e) { lines.Add("commands: " + e.Message); }

                // 3. the new instances
                var types = new List<Type>();
                try { types.AddRange(now.Assembly.GetTypes()); }
                catch (ReflectionTypeLoadException e) { foreach (var t in e.Types) if (t != null) types.Add(t); }
                var added = new List<ModuleBase>(all);
                var bad = new List<ModuleBase>(failed);
                int count = 0;
                foreach (var t in types)
                {
                    if (!t.IsClass || t.IsAbstract || t.ContainsGenericParameters || !t.IsSubclassOf(typeof(ModuleBase))) continue;
                    ModuleBase m;
                    try { m = (ModuleBase)Activator.CreateInstance(t); }
                    catch (Exception e) { lines.Add("FAILED " + t.FullName + ": " + (e.InnerException ?? e).Message); continue; }
                    if (string.IsNullOrEmpty(m.Name) || Find(m.Name) != null)
                    {
                        lines.Add("FAILED " + t.FullName + ": empty or duplicate module name " + m.Name);
                        continue;
                    }
                    m.HookList = DescribeHooks(t);
                    try
                    {
                        m.OnLoad();
                        added.Add(m);
                        count++;
                    }
                    catch (Exception e)
                    {
                        m.LoadFailed = true;
                        m.LastError = "OnLoad: " + e.GetType().Name + ": " + e.Message;
                        m.Log("[ERROR] OnLoad failed: " + e);
                        bad.Add(m);
                        lines.Add("FAILED " + m.Name + ": " + m.LastError);
                    }
                }
                added.Sort(CompareModules);
                all = added.ToArray();
                failed = bad.ToArray();
                Rebuild();
                lines.Insert(0, "[Modules] " + now.Name + (old != null && old.Assembly != null ? " reloaded" : " loaded") + " in " + clock.ElapsedMilliseconds + " ms: "
                    + count + " module class(es), NPC scripts " + Game.MsgNpc.Procesor.invoker.CountOwner(owner)
                    + ", packets " + (Program.MsgInvoker != null ? Program.MsgInvoker.CountOwner(owner) : 0) + ", actions " + Game.MsgServer.MsgDataPacket.invoker.CountOwner(owner) + ".");
                foreach (var l in lines) Print(l, ConsoleColor.Cyan);
            }
            return lines;
        }

        // a runtime module whose folder is gone: shut down, handlers and commands removed, code unloaded
        static List<string> UnloadRuntime(RuntimeModules.Package pkg)
        {
            var lines = new List<string>();
            var keep = new List<ModuleBase>();
            foreach (var m in all)
            {
                if (pkg.Assembly == null || m.GetType().Assembly != pkg.Assembly) { keep.Add(m); continue; }
                try { m.OnSave(); } catch (Exception e) { lines.Add(m.Name + ".OnSave: " + e.Message); }
                try { m.OnShutdown(); } catch (Exception e) { lines.Add(m.Name + ".OnShutdown: " + e.Message); }
            }
            all = keep.ToArray();
            var badKeep = new List<ModuleBase>();
            foreach (var m in failed)
                if (pkg.Assembly == null || m.GetType().Assembly != pkg.Assembly) badKeep.Add(m);
            failed = badKeep.ToArray();
            Rebuild();
            if (Program.MsgInvoker != null) Program.MsgInvoker.RemoveOwner(pkg.Name);
            Game.MsgNpc.Procesor.invoker.RemoveOwner(pkg.Name);
            Game.MsgServer.MsgDataPacket.invoker.RemoveOwner(pkg.Name);
            RuntimeModules.Remove(pkg);
            try { Commands.CommandRegistry.Rescan(); } catch (Exception e) { lines.Add("commands: " + e.Message); }
            lines.Insert(0, "[Modules] " + pkg.Name + " removed: its folder is gone.");
            return lines;
        }

        // ================================================================== queries
        /// <summary>Snapshot of every discovered module (loaded, faulted and load-failed) for GM commands.</summary>
        public static IReadOnlyList<ModuleInfo> Loaded
        {
            get
            {
                var a = all;
                var f = failed;
                var list = new List<ModuleInfo>(a.Length + f.Length);
                foreach (var m in a) list.Add(Info(m));
                foreach (var m in f) list.Add(Info(m));
                return list.AsReadOnly();
            }
        }

        static ModuleInfo Info(ModuleBase m)
        {
            return new ModuleInfo
            {
                Name = m.Name,
                Version = SafeVersion(m),
                Priority = SafePriority(m),
                TypeName = m.GetType().FullName,
                Status = m.LoadFailed ? "load-failed" : m.Faulted ? "faulted" : "ok",
                Faulted = m.Faulted || m.LoadFailed,
                ErrorCount = Volatile.Read(ref m.ErrorCount),
                LastError = m.LastError,
                Hooks = m.HookList
            };
        }

        /// <summary>The loaded instance of module type T (null if not loaded). Lets modules talk to each other.</summary>
        public static T Get<T>() where T : ModuleBase
        {
            var a = all;
            for (int i = 0; i < a.Length; i++)
            {
                var m = a[i] as T;
                if (m != null) return m;
            }
            return null;
        }

        /// <summary>The loaded module with this name (case-insensitive), or null.</summary>
        public static ModuleBase Find(string name)
        {
            var a = all;
            for (int i = 0; i < a.Length; i++)
                if (string.Equals(a[i].Name, name, StringComparison.OrdinalIgnoreCase)) return a[i];
            return null;
        }
    }
}
