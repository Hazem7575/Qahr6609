// Live API for the tools in Qahr6609.Tools\Releases (MapStudio "Apply live", ...): the same wire format as Qahr's AdminApi,
// so the released tools work with this server unchanged.
//   ServerDataBase\adminapi.txt :  port=5899   secret=<at least 16 random characters>   (no file = API off)
//   TCP 127.0.0.1:port, one JSON line in, one JSON line out:
//     -> {"secret":"...","op":"reloadnpcs"}            <- {"ok":true,"message":"..."}  or  {"ok":false,"error":"..."}
//   ops: ping, online, and the live reloads of Infrastructure\HotReload (reloadnpcs, reloadportals, reloadshops,
//   reloaditems, reloadmonsters, reloadmapcells {"map":id}, reloadquests, reloadall {"force":true}),
//   and the PlayerStudio player-admin ops in AdminApi.Players.cs (player, search, setlevel, giveitem, ban, ...).
// Loopback only; a wrong secret is refused after 0.5 s. Every call is written to ServerDataBase\adminapi_log.txt.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609
{
    // partial: the PlayerStudio ops (player admin) are in AdminApi.Players.cs
    public static partial class AdminApi
    {
        public const int DefaultPort = 5899;
        static TcpListener listener;
        static byte[] secret;
        static readonly object logLock = new object();
        // [Security] at most 4 requests at a time; a request line is at most 64 KB and must arrive within 10 s
        static readonly SemaphoreSlim slots = new SemaphoreSlim(4, 4);
        const int MaxLine = 65536;

        static string ConfigPath { get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "adminapi.txt"); } }

        public static void Start()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                    return;// off
                int port = DefaultPort;
                string key = null;
                foreach (string raw in File.ReadAllLines(ConfigPath))
                {
                    string line = raw.Trim();
                    int eq = line.IndexOf('=');
                    if (line.Length == 0 || line[0] == '#' || line[0] == ';' || eq < 0) continue;
                    string k = line.Substring(0, eq).Trim().ToLowerInvariant(), v = line.Substring(eq + 1).Trim();
                    int p;
                    if (k == "port" && int.TryParse(v, out p) && p > 0 && p < 65536) port = p;
                    else if (k == "secret") key = v;
                }
                if (string.IsNullOrEmpty(key) || key.Length < 16 || key.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase))
                {
                    MyConsole.WriteLine("[AdminApi] NOT started: adminapi.txt needs secret=<at least 16 random characters>.", ConsoleColor.Yellow);
                    return;
                }
                secret = Encoding.UTF8.GetBytes(key);
                listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                new Thread(Accept) { IsBackground = true, Name = "AdminApi" }.Start();
                MyConsole.WriteLine("[AdminApi] listening on 127.0.0.1:" + port + " (tools: MapStudio apply live ...)", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[AdminApi] NOT started: " + e.Message, ConsoleColor.Yellow);
            }
        }

        static void Accept()
        {
            while (true)
            {
                TcpClient c;
                try { c = listener.AcceptTcpClient(); }
                catch { return; }
                var ep = c.Client.RemoteEndPoint as IPEndPoint;
                if (ep == null || !IPAddress.IsLoopback(ep.Address)) { c.Close(); continue; }
                if (!slots.Wait(0)) { c.Close(); continue; }// busy: refuse instead of queueing without limit
                ThreadPool.QueueUserWorkItem(_ => { try { Serve(c); } finally { slots.Release(); } });
            }
        }

        static void Serve(TcpClient c)
        {
            using (c)
            {
                try
                {
                    c.ReceiveTimeout = 10000;
                    c.SendTimeout = 10000;
                    var stream = c.GetStream();
                    string line = ReadLineLimited(stream, MaxLine, DateTime.UtcNow.AddSeconds(10));
                    if (line == null) return;
                    JObject reply = Handle(line);
                    byte[] data = Encoding.UTF8.GetBytes(reply.ToString(Formatting.None) + "\n");
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
                catch { }
            }
        }

        // one \n-terminated UTF-8 line, at most max bytes, before the deadline; null otherwise (nothing is buffered beyond max)
        static string ReadLineLimited(NetworkStream s, int max, DateTime deadline)
        {
            var buf = new MemoryStream();
            var one = new byte[1024];
            while (true)
            {
                if (DateTime.UtcNow > deadline) return null;
                int n = s.Read(one, 0, one.Length);
                if (n <= 0) return null;
                int nl = Array.IndexOf(one, (byte)'\n', 0, n);
                buf.Write(one, 0, nl >= 0 ? nl : n);
                if (buf.Length > max) return null;
                if (nl >= 0) return Encoding.UTF8.GetString(buf.ToArray()).TrimEnd('\r');
            }
        }

        static JObject Handle(string line)
        {
            JObject req;
            try { req = JObject.Parse(line); }
            catch { return Fail("bad json"); }
            string given = (string)req["secret"];
            if (given == null || !SecretEquals(given))
            {
                Thread.Sleep(500);
                Log("denied", "wrong or missing secret", false);
                return Fail("unauthorized");// same text as Qahr's AdminApi (PlayerStudio shows "wrong secret" for it)
            }
            string op = ((string)req["op"] ?? "").Trim().ToLowerInvariant();
            try
            {
                switch (op)
                {
                    case "ping":// + server/version/online/maxLevel for PlayerStudio
                        return PlayersPing(Ok("pong " + Program.ServerConfig.ServerName));
                    case "status":
                        {
                            var r = Ok(Program.ServerConfig.ServerName + " " + ServerMode.Current);
                            r["mode"] = ServerMode.Current;
                            r["online"] = Database.Server.GamePoll.Count;
                            r["bridgeConfig"] = PacketStudio.Bridge.PacketBridge.ConfigPath;
                            r["bridgeStream"] = PacketStudio.Bridge.PacketBridge.StreamListening;
                            r["bridgeInject"] = PacketStudio.Bridge.PacketBridge.InjectListening;
                            return r;
                        }
                    case "online":// uid/name/map/x/y/level + the PlayerStudio row fields (AdminApi.Players.cs)
                        return PlayersOnline();
                    case "opendialog":// {"player":"name|uid","id":N[,"type":126]} - opens a client window on that character (ServerControl UI page)
                        return OpenDialog(req);
                    case "command":// {"player":"name|uid","text":"@goldentree start 1 100"} - a GM chat command, run as that online GM
                        return RunCommand(req);
                    case "modreload":// {"name":"DragonIsland"} - hot reload of a runtime module (ServerDataBase\Modules\<name>)
                        {
                            string name = req["name"] != null ? req["name"].ToString().Trim() : "";
                            var lines = Framework.Modules.ModuleManager.ReloadRuntime(name);
                            bool ok = lines.Count > 0 && (lines[0].Contains(" reloaded in ") || lines[0].Contains(" loaded in "));
                            Log("modreload", name + ": " + (lines.Count > 0 ? lines[0] : ""), ok);
                            var r = ok ? Ok(lines[0]) : Fail(lines.Count > 0 ? lines[0] : "failed");
                            r["lines"] = new JArray(lines.Take(40));
                            return r;
                        }
                    default:
                        if (HotReload.HotReloadManager.IsReloadOp(op))
                        {
                            string arg = req["map"] != null ? req["map"].ToString() : null;
                            bool force = req["force"] != null && req["force"].Type == JTokenType.Boolean && (bool)req["force"];
                            var r = HotReload.HotReloadManager.Run(op, arg, force);
                            Log(op, r.Ok ? r.Message : "FAILED: " + r.Error, r.Ok);
                            JObject j = r.Ok ? Ok(r.Message) : Fail(r.Error ?? r.Message);
                            j["op"] = op;
                            j["ms"] = r.ElapsedMs;
                            if (r.Counts.Count > 0) j["counts"] = JObject.FromObject(r.Counts);
                            if (r.Notes.Count > 0) j["notes"] = new JArray(r.Notes.Take(50));
                            return j;
                        }
                        {
                            JObject pr = PlayerOp(op, req);// PlayerStudio ops (AdminApi.Players.cs); null = not one of them
                            if (pr != null) return pr;
                        }
                        return Fail("unknown op: " + op);
                }
            }
            catch (Exception e)
            {
                Log(op, "server error: " + e.Message, false);
                return Fail("server error: " + e.Message);
            }
        }

        // MsgAction 10010, type OpenDialog (126) with the window id in dwParam, as the GM command @addgui does.
        // type 116 (OpenCustom) is accepted too for the client's custom windows.
        static unsafe JObject OpenDialog(JObject req)
        {
            string who = req["player"] != null ? req["player"].ToString().Trim() : "";
            uint id, uid;
            if (req["id"] == null || !uint.TryParse(req["id"].ToString(), out id))
                return Fail("id missing");
            uint type = 126;
            if (req["type"] != null && (!uint.TryParse(req["type"].ToString(), out type) || (type != 126 && type != 116)))
                return Fail("type must be 126 (OpenDialog) or 116 (OpenCustom)");
            Client.GameClient client = null;
            bool byUid = uint.TryParse(who, out uid);
            foreach (var c in Database.Server.GamePoll.Values.ToArray())
                if (c != null && c.Player != null && c.FullLoading && (byUid ? c.Player.UID == uid : string.Equals(c.Player.Name, who, StringComparison.OrdinalIgnoreCase)))
                { client = c; break; }
            if (client == null)
                return Fail("player not online: " + who);
            var action = new Game.MsgServer.ActionQuery
            {
                Type = (Game.MsgServer.ActionType)type,
                ObjId = client.Player.UID,
                dwParam = id,
                dwParam2 = 100,
                dwParam3 = 10000,
                dwParam4 = 1000,
                wParam1 = client.Player.X,
                wParam2 = client.Player.Y
            };
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Send(stream.ActionCreate(&action));
            }
            Log("opendialog", client.Player.Name + " id=" + id + " type=" + type, true);
            return Ok("window " + id + " sent to " + client.Player.Name);
        }

        // the same rights as typing it in game: the character must be online and the command's role must allow it
        static JObject RunCommand(JObject req)
        {
            string who = req["player"] != null ? req["player"].ToString().Trim() : "";
            string text = req["text"] != null ? req["text"].ToString().Trim() : "";
            if (!text.StartsWith("@") || text.Length < 2)
                return Fail("text must be an @command");
            Client.GameClient client = null;
            uint uid;
            bool byUid = uint.TryParse(who, out uid);
            foreach (var c in Database.Server.GamePoll.Values.ToArray())
                if (c != null && c.Player != null && c.FullLoading && (byUid ? c.Player.UID == uid : string.Equals(c.Player.Name, who, StringComparison.OrdinalIgnoreCase)))
                { client = c; break; }
            if (client == null)
                return Fail("player not online: " + who);
            string[] data = text.Substring(1).Split(' ');
            var info = Framework.Commands.CommandRegistry.Find(data[0]);
            if (info == null)
                return Fail("unknown command: " + data[0]);
            if (!Framework.Commands.CommandRegistry.CanUse(client, info.Role))
                return Fail(client.Player.Name + " may not use @" + data[0]);
            bool handled;
            Framework.Commands.CommandRegistry.TryExecute(client, data, out handled);
            Log("command", client.Player.Name + " " + text, handled);
            return handled ? Ok("ran " + text + " as " + client.Player.Name) : Fail("not handled: " + text);
        }

        static bool SecretEquals(string given)
        {
            byte[] g = Encoding.UTF8.GetBytes(given);
            int diff = g.Length ^ secret.Length;
            for (int i = 0; i < secret.Length; i++)
                diff |= secret[i] ^ (i < g.Length ? g[i] : (byte)0);
            return diff == 0;
        }

        static JObject Ok(string message)
        {
            var d = new JObject { ["ok"] = true };
            if (message != null) d["message"] = message;
            return d;
        }

        static JObject Fail(string error)
        {
            return new JObject { ["ok"] = false, ["error"] = error };
        }

        static void Log(string op, string details, bool ok)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + (ok ? "OK  " : "FAIL") + " | " + op + " | " + details + Environment.NewLine;
                lock (logLock)
                    File.AppendAllText(Path.Combine(Program.ServerConfig.DbLocation ?? "", "adminapi_log.txt"), line);
            }
            catch { }
        }
    }
}
