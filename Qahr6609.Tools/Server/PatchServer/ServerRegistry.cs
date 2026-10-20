using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PatchServer
{
    /// <summary>One [ServerN] entry of HazemConfig.ini (or servers.ini).</summary>
    internal sealed class GameServerEntry
    {
        public string Section;
        public int Id;
        public string Name;
        public string LoginName;   // what the client sends to the login server; empty = Name
        public string Host;
        public int GamePort;
        public bool Published;
        public bool Enabled;
        public string LoginHost;   // optional per server, else the shared login server
        public int LoginPort;
        public string Icon;        // FlashIcon in the client's server list
        public string Hint;        // FlashHint in the client's server list (optional)
        public string Notice;      // optional per-server notice (Arabic / default)
        public string NoticeEn;    // optional English notice
        public int Order;
    }

    /// <summary>
    /// Reads the game-server registry (HazemConfig.ini, older name servers.ini) and answers
    /// GET /servers with the PUBLISHED entries only: id, name, host, port, login server and a
    /// live status (TCP connect to host:port, cached a few seconds). Folder/Exe and unpublished
    /// entries are never sent. The file is re-read when its time stamp or size changes.
    ///
    /// servers.ini:
    ///   [Login] or [Auth]  optional, the shared account server (Host=, Port=; all game servers are listed under it)
    ///   Host=26.31.69.47
    ///   Port=9961
    ///   [Server1]
    ///   Name=Hazem         shown to players (launcher + FlashName in the client list)
    ///   LoginName=MeGa     optional: name the client sends to the login server (ServerName in Guard.dat),
    ///                      must match AuthConfig.ini [ServerInfo] ServerN (max 15 chars); empty = Name
    ///   ServerID=1
    ///   Folder=...  Exe=... (never published)
    ///   Host=26.31.69.47
    ///   GamePort=5817
    ///   Published=1        ; shown to players
    ///   Enabled=1          ; 0 = listed as offline (maintenance) without probing
    ///   Notice=...         ; optional text shown in the launcher for this server (Notice_en for English)
    ///   Icon=Group3_server12 ; optional client icon name
    ///   Hint=...           ; optional FlashHint for the client list (default: keep the one in the client's file)
    /// </summary>
    internal sealed class ServerRegistry
    {
        /// <summary>Registry names searched in order (HazemConfig.ini = the one settings file; servers.ini = older name).</summary>
        public static readonly string[] DefaultFileNames = { "HazemConfig.ini", "servers.ini" };
        public const int DefaultLoginPort = 9961;
        public const string DefaultIcon = "Group3_server12";

        private static readonly string[] ReservedSections = { "Login", "Auth", "Realm", "Cluster", "Global", "General", "Settings", "Launcher" };

        private readonly ServerConfig config;
        private readonly object sync = new object();
        private readonly ConcurrentDictionary<string, ProbeEntry> probes =
            new ConcurrentDictionary<string, ProbeEntry>(StringComparer.OrdinalIgnoreCase);

        private string loadedPath;
        private DateTime loadedStamp;
        private long loadedSize = -1;
        private DateTime lastCheckUtc;
        private List<GameServerEntry> servers = new List<GameServerEntry>();
        private string loginHost;
        private int loginPort = DefaultLoginPort;

        private sealed class ProbeEntry
        {
            public DateTime CheckedUtc;
            public Task<bool> Pending;
            public bool Online;
        }

        public ServerRegistry(ServerConfig config)
        {
            this.config = config;
        }

        /// <summary>Full path of the registry file in use (null when none was found).</summary>
        public string FilePath
        {
            get { return ResolvePath(); }
        }

        /// <summary>Snapshot of the current entries (re-reads the file when it changed).</summary>
        public List<GameServerEntry> GetAll(out string effectiveLoginHost, out int effectiveLoginPort)
        {
            lock (sync)
            {
                ReloadIfChanged();
                effectiveLoginHost = loginHost;
                effectiveLoginPort = loginPort;
                return new List<GameServerEntry>(servers);
            }
        }

        /// <summary>JSON body for GET /servers.</summary>
        public byte[] BuildJson()
        {
            string lHost;
            int lPort;
            List<GameServerEntry> all = GetAll(out lHost, out lPort);
            List<GameServerEntry> published = all.FindAll(s => s.Published);

            // probe everything in parallel; each probe is cached for CacheSeconds
            Task<bool> loginTask = string.IsNullOrEmpty(lHost) ? Task.FromResult(false) : Probe(lHost, lPort);
            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (GameServerEntry s in published)
            {
                tasks.Add(s.Enabled ? Probe(s.Host, s.GamePort) : Task.FromResult(false));
            }
            List<Task<bool>> waitAll = new List<Task<bool>>(tasks);
            waitAll.Add(loginTask);
            try
            {
                Task.WaitAll(waitAll.ToArray(), config.ProbeTimeoutMs + 1500);
            }
            catch (AggregateException)
            {
            }

            List<object> list = new List<object>();
            for (int i = 0; i < published.Count; i++)
            {
                GameServerEntry s = published[i];
                bool online = Completed(tasks[i]);
                Dictionary<string, object> o = new Dictionary<string, object>();
                o["id"] = s.Id;
                o["name"] = s.Name;
                o["loginName"] = string.IsNullOrEmpty(s.LoginName) ? s.Name : s.LoginName;
                o["host"] = s.Host;
                o["port"] = s.GamePort;
                o["loginHost"] = string.IsNullOrEmpty(s.LoginHost) ? lHost : s.LoginHost;
                o["loginPort"] = s.LoginPort > 0 ? s.LoginPort : lPort;
                o["online"] = online;
                o["maintenance"] = !s.Enabled;
                o["players"] = -1; // not known to the patch server
                o["icon"] = string.IsNullOrEmpty(s.Icon) ? DefaultIcon : s.Icon;
                o["hint"] = s.Hint ?? string.Empty;
                o["notice"] = s.Notice ?? string.Empty;
                o["notice_en"] = s.NoticeEn ?? string.Empty;
                list.Add(o);
            }

            Dictionary<string, object> login = new Dictionary<string, object>();
            login["host"] = lHost ?? string.Empty;
            login["port"] = lPort;
            login["online"] = Completed(loginTask);

            Dictionary<string, object> root = new Dictionary<string, object>();
            root["generated"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            root["login"] = login;
            root["servers"] = list;

            JavaScriptSerializer js = new JavaScriptSerializer();
            return Encoding.UTF8.GetBytes(js.Serialize(root));
        }

        /// <summary>
        /// Published servers as {id, name, online, players, maintenance} for GET /status.json.
        /// Uses the same cached TCP probe as /servers; players is -1 (not known to the patch server).
        /// </summary>
        public List<Dictionary<string, object>> BuildStatusServers()
        {
            string lHost;
            int lPort;
            List<GameServerEntry> all = GetAll(out lHost, out lPort);
            List<GameServerEntry> published = all.FindAll(s => s.Published);

            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (GameServerEntry s in published)
            {
                tasks.Add(s.Enabled ? Probe(s.Host, s.GamePort) : Task.FromResult(false));
            }
            try
            {
                Task.WaitAll(tasks.ToArray(), config.ProbeTimeoutMs + 1500);
            }
            catch (AggregateException)
            {
            }

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            for (int i = 0; i < published.Count; i++)
            {
                GameServerEntry s = published[i];
                Dictionary<string, object> o = new Dictionary<string, object>();
                o["id"] = s.Id;
                o["name"] = s.Name;
                o["online"] = Completed(tasks[i]);
                o["players"] = -1; // not known to the patch server
                o["maintenance"] = !s.Enabled;
                list.Add(o);
            }
            return list;
        }

        /// <summary>Client list (Guard.dat) entries for the published servers, in /servers order.</summary>
        public List<ScriptHazem.ClientFiles.GuardFile.Entry> BuildGuardEntries()
        {
            string lHost;
            int lPort;
            List<ScriptHazem.ClientFiles.GuardFile.Entry> result = new List<ScriptHazem.ClientFiles.GuardFile.Entry>();
            foreach (GameServerEntry s in GetAll(out lHost, out lPort))
            {
                if (!s.Published) continue;
                ScriptHazem.ClientFiles.GuardFile.Entry e = new ScriptHazem.ClientFiles.GuardFile.Entry();
                e.LoginName = string.IsNullOrEmpty(s.LoginName) ? s.Name : s.LoginName;
                e.DisplayName = s.Name;
                e.LoginHost = string.IsNullOrEmpty(s.LoginHost) ? lHost : s.LoginHost;
                e.LoginPort = s.LoginPort > 0 ? s.LoginPort : lPort;
                e.Icon = s.Icon;
                e.Hint = s.Hint;
                result.Add(e);
            }
            return result;
        }

        private static bool Completed(Task<bool> t)
        {
            return t.Status == TaskStatus.RanToCompletion && t.Result;
        }

        /// <summary>Port-open check with a short timeout; one probe per host:port at a time, cached.</summary>
        private Task<bool> Probe(string host, int port)
        {
            if (string.IsNullOrEmpty(host) || port <= 0 || port > 65535)
            {
                return Task.FromResult(false);
            }

            string key = host + ":" + port.ToString(CultureInfo.InvariantCulture);
            ProbeEntry entry = probes.GetOrAdd(key, k => new ProbeEntry());
            lock (entry)
            {
                if (entry.Pending != null)
                {
                    return entry.Pending;
                }
                if ((DateTime.UtcNow - entry.CheckedUtc).TotalSeconds < config.ProbeCacheSeconds)
                {
                    return Task.FromResult(entry.Online);
                }
                entry.Pending = Task.Run(() =>
                {
                    bool ok = Connect(host, port, config.ProbeTimeoutMs);
                    lock (entry)
                    {
                        entry.Online = ok;
                        entry.CheckedUtc = DateTime.UtcNow;
                        entry.Pending = null;
                    }
                    return ok;
                });
                return entry.Pending;
            }
        }

        internal static bool Connect(string host, int port, int timeoutMs)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Task connect = client.ConnectAsync(host, port);
                    if (!connect.Wait(timeoutMs))
                    {
                        connect.ContinueWith(t => { var ignored = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                        return false;
                    }
                    return client.Connected;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ------------------------------------------------------------ loading

        private string ResolvePath()
        {
            string configured = config.ServersFile;
            if (!string.IsNullOrWhiteSpace(configured))
            {
                configured = configured.Trim().Trim('"');
                return Path.GetFullPath(Path.IsPathRooted(configured) ? configured : Path.Combine(config.ExeDir, configured));
            }

            // default: HazemConfig.ini, then servers.ini, in the exe folder or the nearest parent folder (repo root for bin\Release)
            string dir = config.ExeDir;
            for (int i = 0; i < 6 && !string.IsNullOrEmpty(dir); i++)
            {
                foreach (string fileName in DefaultFileNames)
                {
                    string candidate = Path.Combine(dir, fileName);
                    if (File.Exists(candidate))
                    {
                        return Path.GetFullPath(candidate);
                    }
                    candidate = Path.Combine(dir, "Qahr6609.Shared", fileName);// the shared settings folder of the repo root
                    if (File.Exists(candidate))
                    {
                        return Path.GetFullPath(candidate);
                    }
                }
                DirectoryInfo parent = Directory.GetParent(dir.TrimEnd('\\', '/'));
                dir = parent != null ? parent.FullName : null;
            }
            return null;
        }

        private void ReloadIfChanged()
        {
            // at most one disk check per second
            if (loadedPath != null && (DateTime.UtcNow - lastCheckUtc).TotalSeconds < 1)
            {
                return;
            }
            lastCheckUtc = DateTime.UtcNow;

            string path = ResolvePath();
            DateTime stamp = DateTime.MinValue;
            long size = -1;
            if (path != null && File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);
                stamp = fi.LastWriteTimeUtc;
                size = fi.Length;
            }

            if (string.Equals(path, loadedPath, StringComparison.OrdinalIgnoreCase) && stamp == loadedStamp && size == loadedSize)
            {
                return;
            }

            if (size < 0)
            {
                if (loadedSize >= 0 || loadedPath == null)
                {
                    Log.Warn("HazemConfig.ini / servers.ini not found" + (path != null ? " at " + path : "") + " - /servers lists the [Servers] fallback only");
                }
                servers = new List<GameServerEntry>();
                ApplyFallbacks(null);
                loadedPath = path ?? string.Empty;
                loadedStamp = stamp;
                loadedSize = size;
                return;
            }

            try
            {
                string text = ReadShared(path);
                Dictionary<string, Dictionary<string, string>> sections;
                List<string> order;
                ParseIni(text, out sections, out order);
                List<GameServerEntry> parsed = ParseServers(sections, order);

                servers = parsed;
                Dictionary<string, string> loginSection;
                if (!sections.TryGetValue("Login", out loginSection))
                {
                    sections.TryGetValue("Auth", out loginSection);
                }
                Dictionary<string, string> global;
                if (sections.TryGetValue("Global", out global))
                {
                    // [Global] AuthHost / AuthPort (or LoginHost / LoginPort) override the section values
                    Dictionary<string, string> merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    if (loginSection != null) foreach (KeyValuePair<string, string> kv in loginSection) merged[kv.Key] = kv.Value;
                    string gh = Value(global, "AuthHost", null) ?? Value(global, "LoginHost", null);
                    string gp = Value(global, "AuthPort", null) ?? Value(global, "LoginPort", null);
                    if (gh != null && Value(merged, "Host", null) == null) merged["Host"] = gh;
                    if (gp != null && Value(merged, "Port", null) == null) merged["Port"] = gp;
                    loginSection = merged;
                }
                ApplyFallbacks(loginSection);

                loadedPath = path;
                loadedStamp = stamp;
                loadedSize = size;

                int published = parsed.FindAll(s => s.Published).Count;
                Log.Info(Path.GetFileName(path) + " loaded: " + parsed.Count.ToString(CultureInfo.InvariantCulture) + " servers, "
                    + published.ToString(CultureInfo.InvariantCulture) + " published, login " + loginHost + ":"
                    + loginPort.ToString(CultureInfo.InvariantCulture) + " (" + path + ")");
                foreach (GameServerEntry s in parsed)
                {
                    string login = string.IsNullOrEmpty(s.LoginName) ? s.Name : s.LoginName;
                    if (s.Published && Encoding.Default.GetByteCount(login) > 15)
                    {
                        Log.Warn("[" + s.Section + "] login name '" + login + "' is longer than 15 characters: the client sends only 16 bytes to the login server");
                    }
                }
            }
            catch (Exception ex)
            {
                // keep the previous list; try again on the next request
                Log.Warn("server registry could not be read (" + ex.Message + "), keeping the previous list");
                lastCheckUtc = DateTime.UtcNow.AddSeconds(2);
            }
        }

        private void ApplyFallbacks(Dictionary<string, string> loginSection)
        {
            string host = null;
            int port = 0;
            if (loginSection != null)
            {
                host = Value(loginSection, "Host", null);
                port = Int(loginSection, "Port", 0);
            }
            if (string.IsNullOrEmpty(host)) host = config.LoginHost;
            if (port <= 0) port = config.LoginPort;
            if (string.IsNullOrEmpty(host))
            {
                GameServerEntry first = servers.Find(s => s.Published) ?? (servers.Count > 0 ? servers[0] : null);
                host = first != null ? first.Host : null;
            }
            if (port <= 0) port = DefaultLoginPort;
            loginHost = host;
            loginPort = port;
        }

        private static string ReadShared(string path)
        {
            // the server tools may be writing the file right now
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    using (StreamReader reader = new StreamReader(fs, Encoding.UTF8, true))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (IOException)
                {
                    if (attempt >= 3) throw;
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        internal static List<GameServerEntry> ParseServers(Dictionary<string, Dictionary<string, string>> sections, List<string> order)
        {
            List<GameServerEntry> list = new List<GameServerEntry>();
            int index = 0;
            foreach (string name in order)
            {
                index++;
                // game servers are [Server1], [Server2], ... ([Auth], [Realm], [Cluster] etc. are not)
                if (!name.StartsWith("Server", StringComparison.OrdinalIgnoreCase)
                    || Array.Exists(ReservedSections, r => string.Equals(r, name, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                Dictionary<string, string> keys = sections[name];
                string serverName = Value(keys, "Name", null);
                if (string.IsNullOrEmpty(serverName))
                {
                    continue;
                }

                GameServerEntry e = new GameServerEntry();
                e.Section = name;
                e.Order = index;
                e.Name = serverName;
                e.LoginName = Value(keys, "LoginName", null);
                e.Id = Int(keys, "ServerID", Int(keys, "Id", index));
                e.Host = Value(keys, "Host", null) ?? Value(keys, "IP", null) ?? string.Empty;
                e.GamePort = Int(keys, "GamePort", Int(keys, "Port", 0));
                e.Published = Int(keys, "Published", 0) == 1;   // never shown unless explicitly published
                e.Enabled = Int(keys, "Enabled", 1) != 0;
                e.LoginHost = Value(keys, "LoginHost", null);
                e.LoginPort = Int(keys, "LoginPort", 0);
                e.Icon = Value(keys, "Icon", null);
                e.Hint = Value(keys, "Hint", null);
                e.Notice = Value(keys, "Notice", null) ?? Value(keys, "Notice_ar", null);
                e.NoticeEn = Value(keys, "Notice_en", null);
                list.Add(e);
            }
            list.Sort((a, b) => a.Id != b.Id ? a.Id.CompareTo(b.Id) : a.Order.CompareTo(b.Order));
            return list;
        }

        /// <summary>INI parser that also strips inline "; comment" / "# comment" (after whitespace).</summary>
        internal static void ParseIni(string text, out Dictionary<string, Dictionary<string, string>> sections, out List<string> order)
        {
            sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            order = new List<string>();
            Dictionary<string, string> current = null;

            foreach (string raw in text.Replace("\r\n", "\n").Split('\n'))
            {
                string line = raw.Trim().TrimStart('﻿');
                if (line.Length == 0 || line[0] == ';' || line[0] == '#')
                {
                    continue;
                }
                if (line[0] == '[')
                {
                    int close = line.IndexOf(']');
                    if (close > 1)
                    {
                        string name = line.Substring(1, close - 1).Trim();
                        if (!sections.TryGetValue(name, out current))
                        {
                            current = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            sections[name] = current;
                            order.Add(name);
                        }
                    }
                    continue;
                }
                if (current == null)
                {
                    continue;
                }
                int eq = line.IndexOf('=');
                if (eq <= 0)
                {
                    continue;
                }
                string key = line.Substring(0, eq).Trim();
                string value = StripInlineComment(line.Substring(eq + 1)).Trim();
                if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
                {
                    value = value.Substring(1, value.Length - 2);
                }
                current[key] = value;
            }
        }

        private static string StripInlineComment(string value)
        {
            if (value.TrimStart().StartsWith("\"", StringComparison.Ordinal))
            {
                return value; // quoted values are kept as they are
            }
            for (int i = 1; i < value.Length; i++)
            {
                if ((value[i] == ';' || value[i] == '#') && char.IsWhiteSpace(value[i - 1]))
                {
                    return value.Substring(0, i);
                }
            }
            return value;
        }

        private static string Value(Dictionary<string, string> keys, string key, string def)
        {
            string v;
            if (keys.TryGetValue(key, out v) && v.Length > 0)
            {
                return v;
            }
            return def;
        }

        private static int Int(Dictionary<string, string> keys, string key, int def)
        {
            string v = Value(keys, key, null);
            int n;
            if (v != null && int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n))
            {
                return n;
            }
            return def;
        }
    }
}
