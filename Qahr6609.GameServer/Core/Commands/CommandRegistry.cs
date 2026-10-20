// Attribute based chat command registry.
//
// ---------------------------------------------------------------------------------------------- DEFINING A COMMAND
//   using Conquer6609.Framework.Commands;
//
//   [GmCommand("give", "giveitem", Role = CommandRole.GM, Help = "give an item", Usage = "@give itemid [count]")]
//   public static void Give(Client.GameClient client, string[] args)   // any class, any visibility, must be static
//   {
//       // args = chat text after '@' split on ' ' => args[0] = "give", args[1] = first argument ...
//   }
//
//   Role: CommandRole.Player (everyone), CommandRole.GM (client.GameMaster || client.ProjectManager),
//         CommandRole.ProjectManager (client.ProjectManager).
//   Sensitive = true  => only the command word is audited (use for anything with a password / secret).
//   Names are matched exactly first (case-sensitive, like the old `switch (data[0])`), then case-insensitively.
//   Duplicate names: console warning, the first registration wins (types are scanned in FullName order, so the
//   built-ins in Conquer6609.Framework.* win over Conquer6609.Modules.*).
//
// ---------------------------------------------------------------------------------------------- INTEGRATION (MsgMessage)
//   Near the top of MsgMessage.ChatCommands(client, msg) (Qahr6609.GameServer/Game/MsgServer/MsgMessage.cs), BEFORE the
//   "no GM rights" early return (so that CommandRole.Player commands work for everyone):
//
//       bool staff = client.ProjectManager || client.GameMaster;
//       ...
//       if (msg.__Message.StartsWith("@"))
//       {
//           string[] data = msg.__Message.Substring(1).Split(' ');
//           var info = Conquer6609.Framework.Commands.CommandRegistry.Find(data[0]);
//           // accounts without GM rights only reach the commands they may use; a denied command keeps the old
//           // behaviour below (gm-denied audit, text goes on as normal chat)
//           if (info != null && Conquer6609.Commands.CommandSupport.Matches(info, data[0])
//               && (staff || Conquer6609.Framework.Commands.CommandRegistry.CanUse(client, info.Role)))
//           {
//               bool handled;
//               Conquer6609.Framework.Commands.CommandRegistry.TryExecute(client, data, out handled);
//               if (handled)
//                   return true;
//           }
//       }
//       if (!staff) { ... gm-denied audit (throttled 1/s) ...; return false; }
//       if (msg.__Message.StartsWith("@")) { ... unknown command from a GM/PM: gm-command audit; return true; }
//       return false;
//
//   TryExecute does its own auditing:
//       executed by staff or a GM/PM command .. Guard.Audit("gm-command", uid, name, "@" + full text)  (as MsgMessage)
//       Player command run by a normal player .. Guard.Audit("player-command", ...) throttled 1/s per player
//       permission denied ..................... Guard.Audit("gm-denied", uid, name, "@" + command word) throttled 1/s
//   Sensitive commands (or names containing "pass"/"pwd") only log the command word.
//
//   Migrating an old `case "x": { ... break; }` : move the body into
//       [GmCommand("x")] static void X(Client.GameClient client, string[] data) { ... }   (replace `break;` by `return;`)
//   The body can keep using `data[1]`, `data[2]` ... unchanged. Exceptions are caught by the registry.
//
// ---------------------------------------------------------------------------------------------- THREADING
//   Discovery runs once (Initialize(), called by ModuleManager.Initialize(), or lazily on the first TryExecute).
//   After that the tables are immutable, so TryExecute is safe from any thread and does not allocate for unknown
//   commands. Handlers run on the caller's thread (the socket thread for chat).
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Conquer6609.Framework.Commands
{
    public delegate void CommandHandler(Client.GameClient client, string[] args);

    public sealed class CommandInfo
    {
        /// <summary>Primary name (first name of the attribute).</summary>
        public string Name { get; internal set; }
        /// <summary>Other names of the same attribute.</summary>
        public string[] Aliases { get; internal set; }
        public CommandRole Role { get; internal set; }
        public string Help { get; internal set; }
        public string Usage { get; internal set; }
        public bool Sensitive { get; internal set; }
        /// <summary>Declaring type + method, for diagnostics.</summary>
        public string Source { get; internal set; }
        public CommandHandler Handler { get; internal set; }
    }

    public static class CommandRegistry
    {
        static volatile Dictionary<string, CommandInfo> exact;   // Ordinal (case-sensitive)
        static volatile Dictionary<string, CommandInfo> loose;   // OrdinalIgnoreCase
        static volatile CommandInfo[] list = new CommandInfo[0];
        static readonly object initLock = new object();

        // ================================================================== discovery
        /// <summary>Scans the assembly for [GmCommand] methods (once; later calls do nothing).</summary>
        public static void Initialize()
        {
            if (exact != null) return;
            lock (initLock)
            {
                if (exact != null) return;
                Build();
            }
        }

        /// <summary>Scans the server and the runtime modules again (a runtime module was reloaded: its commands
        /// are replaced by the new code's, a removed command disappears).</summary>
        public static void Rescan()
        {
            lock (initLock)
                Build();
        }

        static void Build()
        {
            {
                var ex = new Dictionary<string, CommandInfo>(StringComparer.Ordinal);
                var lo = new Dictionary<string, CommandInfo>(StringComparer.OrdinalIgnoreCase);
                var all = new List<CommandInfo>();

                // the server + every runtime module (ServerDataBase\Modules\<Name>\*.cs, Modules\RuntimeModules.cs)
                var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
                foreach (var pkg in global::Conquer6609.Framework.Modules.RuntimeModules.Packages)
                    if (pkg.Assembly != null) assemblies.Add(pkg.Assembly);
                var typeList = new List<Type>();
                foreach (var asm in assemblies)
                {
                    try { typeList.AddRange(asm.GetTypes()); }
                    catch (ReflectionTypeLoadException e) { typeList.AddRange(e.Types); }
                }
                Type[] types = typeList.ToArray();
                var sorted = new List<Type>();
                foreach (var t in types) if (t != null) sorted.Add(t);
                sorted.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));

                foreach (var t in sorted)
                {
                    MethodInfo[] methods;
                    try { methods = t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly); }
                    catch { continue; }
                    Array.Sort(methods, (a, b) => a.MetadataToken.CompareTo(b.MetadataToken));
                    foreach (var mi in methods)
                    {
                        object[] attrs;
                        try { attrs = mi.GetCustomAttributes(typeof(GmCommandAttribute), false); }
                        catch { continue; }
                        if (attrs.Length == 0) continue;

                        string source = t.FullName + "." + mi.Name;
                        CommandHandler handler = null;
                        var ps = mi.GetParameters();
                        if (!mi.IsStatic || mi.ReturnType != typeof(void) || ps.Length != 2
                            || ps[0].ParameterType != typeof(Client.GameClient) || ps[1].ParameterType != typeof(string[]))
                        {
                            Warn("[Commands] " + source + " ignored: signature must be static void (Client.GameClient client, string[] args)");
                            continue;
                        }
                        try { handler = (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), mi); }
                        catch (Exception e) { Warn("[Commands] " + source + " ignored: " + e.Message); continue; }

                        foreach (GmCommandAttribute a in attrs)
                        {
                            var names = new List<string>();
                            foreach (var raw in a.Names)
                            {
                                string n = (raw ?? "").Trim();
                                if (n.StartsWith("@")) n = n.Substring(1);
                                if (n.Length == 0 || n.IndexOf(' ') >= 0)
                                {
                                    Warn("[Commands] " + source + ": invalid command name '" + raw + "' ignored");
                                    continue;
                                }
                                names.Add(n);
                            }
                            if (names.Count == 0) continue;

                            var info = new CommandInfo
                            {
                                Name = names[0],
                                Aliases = names.GetRange(1, names.Count - 1).ToArray(),
                                Role = a.Role,
                                Help = a.Help ?? "",
                                Usage = a.Usage ?? "",
                                Sensitive = a.Sensitive || LooksSensitive(names),
                                Source = source,
                                Handler = handler
                            };
                            bool any = false;
                            foreach (var n in names)
                            {
                                CommandInfo prev;
                                if (ex.TryGetValue(n, out prev))
                                {
                                    Warn("[Commands] duplicate @" + n + " in " + source + " ignored (already " + prev.Source + ")");
                                    continue;
                                }
                                ex[n] = info;
                                if (!lo.ContainsKey(n)) lo[n] = info;
                                any = true;
                            }
                            if (any) all.Add(info);
                        }
                    }
                }

                all.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
                list = all.ToArray();
                loose = lo;
                exact = ex; // published last: `exact != null` means ready
            }
        }

        static bool LooksSensitive(List<string> names)
        {
            foreach (var n in names)
            {
                var l = n.ToLowerInvariant();
                if (l.Contains("pass") || l.Contains("pwd")) return true;
            }
            return false;
        }

        static void Warn(string line)
        {
            try { MyConsole.WriteLine(line, ConsoleColor.Yellow); }
            catch { try { System.Console.WriteLine(line); } catch { } }
        }

        // ================================================================== lookup
        /// <summary>The command registered under name (exact match first, then case-insensitive), or null.</summary>
        public static CommandInfo Find(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var ex = exact;
            if (ex == null) { Initialize(); ex = exact; }
            CommandInfo info;
            if (ex.TryGetValue(name, out info)) return info;
            if (loose.TryGetValue(name, out info)) return info;
            return null;
        }

        public static bool IsRegistered(string name) { return Find(name) != null; }

        /// <summary>Every registered command (one entry per attribute, aliases inside), sorted by name.</summary>
        public static IReadOnlyList<CommandInfo> Commands
        {
            get
            {
                if (exact == null) Initialize();
                return Array.AsReadOnly(list);
            }
        }

        /// <summary>Same rights check as MsgMessage.ChatCommands.</summary>
        public static bool CanUse(Client.GameClient client, CommandRole role)
        {
            if (client == null) return false;
            switch (role)
            {
                case CommandRole.Player: return true;
                case CommandRole.GM: return client.GameMaster || client.ProjectManager;
                case CommandRole.ProjectManager: return client.ProjectManager;
                default: return false;
            }
        }

        // ================================================================== execution
        /// <summary>
        /// data = chat text after '@' split on ' ' (data[0] = command word).
        /// handled = false: unknown command, nothing done (let the old switch run).
        /// handled = true : the registry owns this command (executed, or permission denied and audited).
        /// Returns true only when the handler ran without throwing.
        /// </summary>
        public static bool TryExecute(Client.GameClient client, string[] data, out bool handled)
        {
            handled = false;
            if (client == null || data == null || data.Length == 0) return false;
            var info = Find(data[0]);
            if (info == null) return false;
            handled = true;

            uint uid = 0;
            string pname = "";
            var player = client.Player;
            if (player != null) { uid = player.UID; pname = player.Name; }

            string word = data[0].Length > 32 ? data[0].Substring(0, 32) : data[0];
            if (!CanUse(client, info.Role))
            {
                if (Conquer6609.Security.Guard.Throttle("gm-denied:" + uid, 1000))
                    Conquer6609.Security.Guard.Audit("gm-denied", uid, pname, "@" + word);
                return false;
            }

            string text = info.Sensitive ? "@" + word + " <redacted>" : "@" + string.Join(" ", data);
            bool staff = client.GameMaster || client.ProjectManager;
            if (staff || info.Role != CommandRole.Player)
                Conquer6609.Security.Guard.Audit("gm-command", uid, pname, text);
            else if (Conquer6609.Security.Guard.Throttle("player-command:" + uid, 1000))
                Conquer6609.Security.Guard.Audit("player-command", uid, pname, text);

            try
            {
                info.Handler(client, data);
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    if (Conquer6609.Security.Guard.Throttle("cmd-error:" + info.Name, 30000))
                        MyConsole.WriteLine("[Commands] @" + info.Name + " (" + info.Source + ") error: " + e.GetType().Name + ": " + e.Message, ConsoleColor.Red);
                    if (staff)
                        client.SendSysMesage("@" + word + " failed: " + e.GetType().Name + ": " + e.Message);
                    else
                        client.SendSysMesage("@" + word + " failed.");
                }
                catch { }
                return false;
            }
        }

        /// <summary>Convenience: handled flag only.</summary>
        public static bool TryExecute(Client.GameClient client, string[] data)
        {
            bool handled;
            TryExecute(client, data, out handled);
            return handled;
        }

        /// <summary>Convenience: message is the raw chat text ("@name arg1 arg2"). Returns handled.</summary>
        public static bool TryExecuteText(Client.GameClient client, string message)
        {
            if (string.IsNullOrEmpty(message) || message[0] != '@') return false;
            return TryExecute(client, message.Substring(1).Split(' '));
        }
    }
}
