// Module (plugin) base class. A module is any non-abstract class deriving ModuleBase that lives in
// Qahr6609.GameServer/Modules/<Name>/ (compiled through the csproj wildcard). It is discovered and started by
// ModuleManager.Initialize(); presence of the folder = active. See docs/MODULES.md.
//
// Rules for hook code (all hooks are called from game threads):
//  - keep it fast, never do blocking I/O (files, MySQL, network) inside a hook; use Log() (async) or your own queue;
//  - assume concurrent calls (socket thread, MapGroupThread, KernelThread, save thread): use Concurrent* collections
//    or locks for shared state;
//  - an exception never reaches the game: it is caught, logged and counted; 100 exceptions in 60 s disables the module.
using System;
using System.IO;

namespace Conquer6609.Framework.Modules
{
    public abstract class ModuleBase
    {
        // ------------------------------------------------------------------ identity
        /// <summary>Unique module name (also the data folder and the shell.ini section [Module.&lt;Name&gt;]).</summary>
        public abstract string Name { get; }
        public virtual string Version { get { return "1.0"; } }
        /// <summary>Lower runs first (load order and hook order). Shutdown runs in reverse order.</summary>
        public virtual int Priority { get { return 0; } }

        // ------------------------------------------------------------------ lifecycle
        /// <summary>Called once after the server data is loaded. Throwing here = module not loaded.</summary>
        public virtual void OnLoad() { }
        /// <summary>Called once when the server shuts down (reverse priority order).</summary>
        public virtual void OnShutdown() { }
        /// <summary>Periodic world save. May be called from a background save thread: must be thread safe.
        /// Calls to OnSave are serialized by the manager (never two OnSave at the same time).</summary>
        public virtual void OnSave() { }

        // ------------------------------------------------------------------ ticks
        /// <summary>About once per second (KernelThread).</summary>
        public virtual void OnTick(Extensions.Time32 clock) { }

        // ------------------------------------------------------------------ player
        /// <summary>After the player finished logging in (CompleteLogin).</summary>
        public virtual void OnLogin(Client.GameClient client) { }
        /// <summary>When the player leaves the game (disconnect / logout).</summary>
        public virtual void OnLogout(Client.GameClient client) { }

        // ------------------------------------------------------------------ movement
        /// <summary>Walk, steed walk and jump to (x, y). Return false to BLOCK the step; the module is then
        /// responsible for correcting the client position (e.g. client.Teleport / pullback).</summary>
        public virtual bool OnMove(Client.GameClient client, ushort x, ushort y) { return true; }

        // ------------------------------------------------------------------ combat
        /// <summary>Damage a player deals to a monster. Return the (possibly adjusted) damage.</summary>
        public virtual uint OnMonsterDamage(Client.GameClient attacker, Game.MsgMonster.MonsterRole target, uint damage) { return damage; }
        /// <summary>A player killed a monster.</summary>
        public virtual void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster) { }
        /// <summary>A player died to another player. killer may be null.</summary>
        public virtual void OnPlayerKilled(Role.Player killer, Role.Player victim) { }

        // ------------------------------------------------------------------ items
        /// <summary>Player uses an item. Return true = handled (the default item processing is skipped).</summary>
        public virtual bool OnItemUse(Client.GameClient client, Game.MsgServer.MsgGameItem item) { return false; }

        // ------------------------------------------------------------------ NPC dialogs
        /// <summary>NPC dialog (first click option 0, or an answer). Return 0 = not handled (the NPC script runs as usual),
        /// 1 = handled (the NPC script does not run), 2 = handled, but run the NPC script with option 0 now
        /// ("something else" chosen in a module dialog). Module options use ids 200-249 (+255) and must be added
        /// with Dialog.AddOption (StrictNpcOptions). Called on the NPC worker thread for NPCs found on the player's screen.</summary>
        public virtual int OnNpcTalk(Client.GameClient client, ServerSockets.Packet stream, byte option, uint npcId) { return 0; }

        // ------------------------------------------------------------------ helpers
        private volatile string dataPath;
        private readonly object dataPathLock = new object();

        /// <summary>&lt;DbLocation&gt;\Modules\&lt;Name&gt;\ (with trailing separator). Created on first access.</summary>
        public string DataPath
        {
            get
            {
                var p = dataPath;
                if (p != null) return p;
                lock (dataPathLock)
                {
                    if (dataPath == null)
                    {
                        string root = Program.ServerConfig.DbLocation ?? "";
                        string dir = Path.Combine(Path.Combine(root, "Modules"), SafeName);
                        try { Directory.CreateDirectory(dir); } catch { }
                        if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString()) && !dir.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                            dir += Path.DirectorySeparatorChar;
                        dataPath = dir;
                    }
                    return dataPath;
                }
            }
        }

        /// <summary>Asynchronous log line -> &lt;DataPath&gt;\logs\yyyyMMdd.log. Never blocks the caller
        /// (bounded queue; lines are dropped when the queue is full).</summary>
        public void Log(string line)
        {
            ModuleLog.Enqueue(this, line);
        }

        /// <summary>shell.ini section of this module: "Module.&lt;Name&gt;".</summary>
        public string ConfigSection
        {
            get
            {
                var s = configSection;
                if (s == null) configSection = s = "Module." + Name;
                return s;
            }
        }
        private string configSection;

        public int ConfigInt(string key, int def) { return Settings.Int(ConfigSection, key, def); }
        public long ConfigLong(string key, long def) { return Settings.Long(ConfigSection, key, def); }
        public bool ConfigBool(string key, bool def) { return Settings.Bool(ConfigSection, key, def); }
        public string ConfigString(string key, string def) { return Settings.String(ConfigSection, key, def); }

        // ------------------------------------------------------------------ manager state (internal)
        internal string SafeName
        {
            get
            {
                string n = Name ?? GetType().Name;
                foreach (char c in Path.GetInvalidFileNameChars())
                    n = n.Replace(c, '_');
                return n.Length == 0 ? GetType().Name : n;
            }
        }

        internal volatile bool Faulted;
        internal volatile bool LoadFailed;
        internal int ErrorCount;                 // total exceptions (Interlocked)
        internal volatile string LastError;
        internal long LastConsoleError;          // DateTime.UtcNow.Ticks of the last console line, 0 = never (throttle)
        internal readonly object ErrorLock = new object();
        internal readonly long[] ErrorWindow = new long[ModuleManager.FaultLimit];
        internal int ErrorWindowPos;
        internal int ErrorWindowFill;
        internal string HookList = "";
    }
}
