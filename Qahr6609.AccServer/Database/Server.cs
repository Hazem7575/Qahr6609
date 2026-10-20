// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace AccServer.Database
{
    public unsafe class ServerInfo
    {
        public string Name;
        public string IP;
        public ushort Port;
        /// <summary>Game server id from servers.ini (0 = unknown: AuthConfig.ini line without an id).
        /// Written into login_tokens.ServerID so the token is only accepted by that server.</summary>
        public uint ServerID;
        /// <summary>Other names clients may send for this server (servers.ini ClientNames=, e.g. the old name
        /// still compiled into the client). They map to this entry; tokens are bound by ServerID, never by name.</summary>
        public List<string> ClientNames = new List<string>();
        public string TransferKey;
        public string TransferSalt;
    }
    public unsafe class Server
    {
        public static Dictionary<string, ServerInfo> Servers = new Dictionary<string, ServerInfo>();
        /// <summary>servers.ini the list came from (null = AuthConfig.ini ServerN= lines).</summary>
        public static string RegistryPath;

        public static void Load()
        {
            IniFile file = new IniFile(Directory.GetCurrentDirectory() + @"\AuthConfig.ini");
            Program.LoginPort = file.ReadUInt16("ServerInfo", "Auth_Port", 9961);
            string mySqlHost = file.ReadString("MySql", "Host", "");
            string mySqlPassword = file.ReadString("MySql", "Password", "");
            string mySqlDatabase = file.ReadString("MySql", "Database", "");
            string mySqlUser = file.ReadString("MySql", "User", "root");
            uint mySqlPort = file.ReadUInt16("MySql", "Port", 3306);
            DataHolder.CreateConnection(mySqlDatabase, mySqlPassword, mySqlHost, mySqlUser, mySqlPort);
            Network.AuthPackets.LoginGuard.Load(file);

            // Server list: servers.ini (repo root, shared with the launcher and the control panel), entries with
            // Published=1. Without a usable servers.ini the old AuthConfig.ini ServerCount/ServerN= lines are used.
            RegistryPath = FindRegistry(file.ReadString("ServerInfo", "Registry", ""));
            if (RegistryPath != null)
            {
                try
                {
                    foreach (var serverinfo in LoadRegistry(RegistryPath))
                        AddServer(serverinfo);
                }
                catch (Exception e)
                {
                    Console.WriteLine("servers.ini " + RegistryPath + " could not be read: " + e.Message);
                    Servers.Clear();
                }
                if (Servers.Count > 0)
                {
                    Console.WriteLine("Server list from " + RegistryPath + " (Published=1 entries).");
                    return;
                }
                Console.WriteLine("servers.ini " + RegistryPath + " has no published server, using AuthConfig.ini.");
            }
            RegistryPath = null;
            int count = file.ReadByte("ServerInfo", "ServerCount", 1);
            for (int i = 0; i < count; i = (int)(i + 1))
            {
                int num3 = (int)(i + 1);
                // Name,Port,IP[,ServerID]
                string[] strArray = file.ReadString("ServerInfo", "Server" + ((int)num3).ToString(), "").Split((char[])new char[] { ',' });
                ServerInfo serverinfo = new ServerInfo
                {
                    Name = strArray[0],
                    Port = ushort.Parse(strArray[1]),
                    IP = strArray[2]
                };
                uint id;
                if (strArray.Length > 3 && uint.TryParse(strArray[3].Trim(), out id))
                    serverinfo.ServerID = id;
                AddServer(serverinfo);
            }
        }

        static void AddServer(ServerInfo serverinfo)
        {
            if (Servers.ContainsKey(serverinfo.Name))
            {
                Console.WriteLine("Server name " + serverinfo.Name + " listed twice, the second entry is ignored.");
                return;
            }
            Servers.Add(serverinfo.Name, serverinfo);
            string format = "({3}) ==> [ {0} ] : [ IP {1} : Port {2} ]{4} .";
            Console.WriteLine(string.Format(format, serverinfo.Name, serverinfo.IP, serverinfo.Port, Servers.Count,
                (serverinfo.ServerID != 0 ? " ServerID " + serverinfo.ServerID : "")
                + (serverinfo.ClientNames.Count > 0 ? " aliases " + string.Join("/", serverinfo.ClientNames) : "")));
            foreach (var alias in serverinfo.ClientNames)
            {
                if (alias == serverinfo.Name)
                    continue;
                if (Servers.ContainsKey(alias))
                    Console.WriteLine("Client name " + alias + " of " + serverinfo.Name + " is already used, ignored.");
                else
                    Servers.Add(alias, serverinfo);// the client's name -> the canonical entry
            }
        }

        /// <summary>AuthConfig [ServerInfo] Registry=path, else HazemConfig.ini (older name servers.ini) in the working
        /// directory or up to 4 parents.</summary>
        static string FindRegistry(string configured)
        {
            if (!string.IsNullOrEmpty(configured))
            {
                string p = Path.IsPathRooted(configured) ? configured : Path.Combine(Directory.GetCurrentDirectory(), configured);
                if (File.Exists(p))
                    return Path.GetFullPath(p);
                Console.WriteLine("[ServerInfo] Registry=" + configured + " not found.");
                return null;
            }
            try
            {
                var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                for (int i = 0; i < 5 && dir != null; i++, dir = dir.Parent)
                {
                    foreach (var name in new[] { "HazemConfig.ini", "servers.ini" })
                    {
                        string p = Path.Combine(dir.FullName, name);
                        if (File.Exists(p))
                            return p;
                        p = Path.Combine(dir.FullName, "Qahr6609.Shared", name);// the shared settings folder of the repo root
                        if (File.Exists(p))
                            return p;
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>[ServerN] sections with Published=1 (Name, ServerID, Host, GamePort). ';' starts a comment anywhere.</summary>
        static List<ServerInfo> LoadRegistry(string path)
        {
            var list = new List<ServerInfo>();
            ServerInfo cur = null;
            bool published = false;
            Action flush = () =>
            {
                if (cur != null && published && !string.IsNullOrEmpty(cur.Name) && cur.Port != 0 && !string.IsNullOrEmpty(cur.IP))
                    list.Add(cur);
            };
            foreach (var raw in File.ReadAllLines(path))
            {
                string line = raw;
                int sc = line.IndexOf(';');
                if (sc >= 0) line = line.Substring(0, sc);
                line = line.Trim();
                if (line.Length == 0)
                    continue;
                if (line[0] == '[')
                {
                    flush();
                    string name = line.Trim('[', ']').Trim();
                    cur = name.StartsWith("Server", StringComparison.OrdinalIgnoreCase) ? new ServerInfo() : null;
                    published = false;
                    continue;
                }
                if (cur == null)
                    continue;
                int eq = line.IndexOf('=');
                if (eq <= 0)
                    continue;
                string k = line.Substring(0, eq).Trim().ToLowerInvariant(), v = line.Substring(eq + 1).Trim();
                uint n;
                uint.TryParse(v, out n);
                switch (k)
                {
                    case "name": cur.Name = v; break;
                    case "serverid": cur.ServerID = n; break;
                    case "host": cur.IP = v; break;
                    case "gameport": cur.Port = (ushort)Math.Min(n, 65535); break;
                    case "published": published = v == "1"; break;
                    case "loginname":// the name written into the client's Guard.dat (PatchServer / configure)
                    case "clientnames":
                        foreach (var a in v.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            if (!cur.ClientNames.Contains(a)) cur.ClientNames.Add(a);
                        break;
                }
            }
            flush();
            return list;
        }
    }
}
