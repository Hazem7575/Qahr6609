using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.MsgInterServer.Packets;

namespace Conquer6609.MsgInterServer
{
    public class PipeServer
    {
        public class User
        {
            public ServerSockets.SecuritySocket Socket;
            public Client.GameClient Owner;
            public bool Alive { get; private set; }
            // Set once the peer presented the shared inter-server secret.
            public bool Authenticated = false;
            public User(ServerSockets.SecuritySocket _socket)
            {
                Socket = _socket;
                Alive = true;
            }
        

            public void Send(ServerSockets.Packet msg)
            {
                /*if (!msg.EncryptPipeServer())
                    return;*/
                if (Alive)
                {
                    if (IsServer)
                        Socket.Send(msg);
                    else
                        Owner.Send(msg);
                }
            }
            public void Disconnect()
            {
                if (Alive)
                {
                    Alive = false;
                    Socket.Disconnect();
                }
            }


            //for standard connecxion
            public Database.GroupServerList.Server ServerInfo;
            public bool IsServer
            {
                get { return ServerInfo != null; }
            }

        }
        public static Extensions.SafeDictionary<uint, User> PollServers = new Extensions.SafeDictionary<uint, User>();
                                                                            //3999900405
        private static Extensions.Counter UIDCounter = new Extensions.Counter(3999900001);
        private static Extensions.Counter UIDOnElitePKCounter = new Extensions.Counter(1000000);
        public static ServerSockets.ServerSocket Server;
        /// <summary>Inter-server link settings from shell.ini [InterServer]:
        /// PipeEnabled=1 (open the pipe listener on a non inter-server), Secret=..., AllowedIPs=ip1,ip2.</summary>
        public static class Security
        {
            public static bool PipeEnabled = false;
            public static string Secret = "";
            public static HashSet<string> AllowedIPs = new HashSet<string>();
            // Upper bounds for balances received from a peer.
            public const long MaxPeerConquerPoints = int.MaxValue;
            public const long MaxPeerMoney = long.MaxValue / 4;
            public const int MaxPeerLevel = 140;
            static int loaded;

            public static void Load()
            {
                if (System.Threading.Interlocked.Exchange(ref loaded, 1) != 0)
                    return;
                try
                {
                    string path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "shell.ini");
                    if (!System.IO.File.Exists(path))
                        return;
                    bool inSec = false;
                    foreach (var raw in System.IO.File.ReadAllLines(path))
                    {
                        var line = raw.Trim();
                        if (line.StartsWith("[")) { inSec = line.Equals("[InterServer]", StringComparison.OrdinalIgnoreCase); continue; }
                        if (!inSec) continue;
                        int eq = line.IndexOf('=');
                        if (eq <= 0) continue;
                        string k = line.Substring(0, eq).Trim().ToLowerInvariant(), v = line.Substring(eq + 1).Trim();
                        if (k == "secret") Secret = v;
                        else if (k == "pipeenabled") PipeEnabled = v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase);
                        else if (k == "allowedips")
                        {
                            foreach (var ip in v.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                                AllowedIPs.Add(ip.Trim());
                        }
                    }
                }
                catch (Exception e) { Conquer6609.Security.Guard.Audit("interserver-config", 0, "", "load failed: " + e.Message); }
            }

            public static bool ListenerEnabled
            {
                get { Load(); return Program.ServerConfig.IsInterServer || PipeEnabled; }
            }

            /// <summary>Peers must be listed in AllowedIPs; without that list only loopback and the configured group servers are accepted.</summary>
            public static bool IsPeerAllowed(string ip)
            {
                Load();
                if (string.IsNullOrEmpty(ip) || ip == "NONE")
                    return false;
                if (AllowedIPs.Count > 0)
                    return AllowedIPs.Contains(ip);
                if (ip == "127.0.0.1")
                    return true;
                try
                {
                    foreach (var server in Database.GroupServerList.GroupServers.Values)
                        if (server.IPAddress == ip)
                            return true;
                    if (Database.GroupServerList.InterServer != null && Database.GroupServerList.InterServer.IPAddress == ip)
                        return true;
                }
                catch { }
                return false;
            }

            public static uint ClampConquerPoints(ulong value)
            {
                return (uint)Math.Min(value, (ulong)MaxPeerConquerPoints);
            }
            public static int ClampBoundConquerPoints(ulong value)
            {
                return (int)Math.Min(value, (ulong)int.MaxValue);
            }
            public static long ClampMoney(ulong value)
            {
                return (long)Math.Min(value, (ulong)MaxPeerMoney);
            }
        }

        /// <summary>Control links by ServerID (one per game server, pipe v2).</summary>
        public static System.Collections.Concurrent.ConcurrentDictionary<uint, User> ControlLinks = new System.Collections.Concurrent.ConcurrentDictionary<uint, User>();

        public static void Initialize()
        {
            // The pipe listener only runs on the inter-server (or when explicitly enabled) and only with a shared secret
            // of at least PipeConfig.MinSecretLength characters (pipe v2 handshake, MsgInterServer\PipeSecurity.cs).
            if (!Security.ListenerEnabled)
                return;
            if (!PipeConfig.SecretUsable)
            {
                MyConsole.WriteLine("[Pipe Server] not started: [InterServer] Secret= must be set and at least " + PipeConfig.MinSecretLength + " characters long.");
                Conquer6609.Security.Guard.Audit("interserver", 0, "", "pipe listener disabled: no usable secret configured");
                return;
            }
            if (Database.GroupServerList.MyServerInfo == null)
                return;
            string bind = PipeConfig.Bind;
            Server = new ServerSockets.ServerSocket(Accept, Receive, Disconnect);
            Server.Initilize(Program.ServerConfig.Port_SendSize, Program.ServerConfig.Port_ReceiveSize, 1000, 1000);
            Server.Open(Database.GroupServerList.MyServerInfo.IPAddress, Database.GroupServerList.MyServerInfo.Port, Program.ServerConfig.Port_BackLog, bind);
            MyConsole.WriteLine("[Pipe Server] v2 listening on " + bind + ":" + Database.GroupServerList.MyServerInfo.Port + " (HMAC handshake, framed links).");
            if (bind == "0.0.0.0")
                MyConsole.WriteLine("[Pipe Server] WARNING: [InterServer] Bind=0.0.0.0 listens on every interface; use the LAN/loopback address.", ConsoleColor.Yellow);
        }

        /// <summary>Relay links of one home server that are open now.</summary>
        static int CountRelayLinks(uint serverId)
        {
            int n = 0;
            if (Server == null)
                return 0;
            foreach (var s in Server.Clients.GetValues())
            {
                var c = s.Pipe;
                if (s.Alive && c != null && c.Ready && c.Link == PipeLinkType.Relay && c.PeerServerID == serverId)
                    n++;
            }
            return n;
        }

        /// <summary>Listener side of the handshake: checks the connector's HMAC proof, answers with ours, sets the keys.</summary>
        static void HandleAuth(User user, ServerSockets.SecuritySocket obj, ServerSockets.Packet stream, ushort PacketID)
        {
            var codec = obj.Pipe;
            string fail = null;
            bool countsForBackoff = true;// a wrong proof / protocol counts; "already connected" / "too many links" do not
            PipeLinkType link = PipeLinkType.None;
            uint serverId = 0;
            byte[] clientNonce = null, proof = null;
            if (PacketID != PacketTypes.InterServer_Auth || !PipeHandshake.ReadAuth(stream, out link, out serverId, out clientNonce, out proof))
                fail = "expected a v2 auth packet, got " + PacketID;
            else if (!PipeConfig.SecretUsable)
                fail = "no usable secret";
            else if (!PipeCrypto.FixedEquals(proof, PipeCrypto.Proof(PipeConfig.Secret, link, serverId, codec.ServerNonce, clientNonce)))
                fail = "wrong proof (secret) for server " + serverId;
            else if (Database.GroupServerList.GetServer(serverId) == null || (Database.GroupServerList.MyServerInfo != null && serverId == Database.GroupServerList.MyServerInfo.ID))
                fail = "server id " + serverId + " is not a peer in client_config.ini";
            else if (link == PipeLinkType.Control)
            {
                User old;
                if (ControlLinks.TryGetValue(serverId, out old) && old.Alive)
                {
                    fail = "control link of server " + serverId + " is already connected";
                    countsForBackoff = false;
                }
            }
            else if (CountRelayLinks(serverId) >= PipeConfig.MaxRelayLinksPerServer)
            {
                fail = "too many relay links from server " + serverId;
                countsForBackoff = false;
            }
            if (fail != null)
            {
                Conquer6609.Security.Guard.Audit("interserver", 0, "", "pipe peer " + obj.RemoteIp + " failed authentication: " + fail);
                if (countsForBackoff)
                    PipeBackoff.Fail(obj.RemoteIp);
                user.Disconnect();
                return;
            }
            codec.ClientNonce = clientNonce;
            using (var rec = new ServerSockets.RecycledPacket())
                obj.Send(PipeHandshake.AuthOk(rec.GetStream(), PipeCrypto.Ack(PipeConfig.Secret, codec.ServerNonce, clientNonce)));
            codec.Link = link;
            codec.PeerServerID = serverId;
            codec.SetKeys(PipeConfig.Secret);
            user.Authenticated = true;
            PipeBackoff.Success(obj.RemoteIp);
            if (link == PipeLinkType.Control)
                ControlLinks[serverId] = user;
            MyConsole.WriteLine("[Pipe Server] " + link + " link of server " + serverId + " authenticated (" + obj.RemoteIp + ").");
        }

        /// <summary>A client packet relayed by the home server: only ever dispatched through the client handlers.</summary>
        static unsafe void HandleEnvelope(User user, PipeCodec codec, ServerSockets.Packet stream)
        {
            uint uid = user.Owner != null && user.Owner.Player != null ? user.Owner.Player.RealUID : 0;
            if (!codec.SetupComplete || user.Owner == null || user.Owner.Player == null || !user.Owner.FullLoading)
            {
                PipeFilter.AuditDrop(uid, "", "client packet before the relay setup finished, server " + codec.PeerServerID);
                return;
            }
            int payload = stream.Size - 4 - 8;
            if (payload < 4 + 8 || payload > ServerSockets.Packet.MAX_SIZE)
            {
                PipeFilter.AuditDrop(uid, user.Owner.Player.Name, "bad envelope size " + payload);
                return;
            }
            var inner = ServerSockets.PacketRecycle.Take();
            try
            {
                inner.Seek(0);
                inner.memcpy(inner.Memory, stream.Memory + 4, payload);
                inner.Size = payload;
                ushort len = *(ushort*)inner.Memory;
                if (len + 8 != payload)
                {
                    PipeFilter.AuditDrop(uid, user.Owner.Player.Name, "envelope length mismatch " + len + "/" + payload);
                    return;
                }
                inner.Seek(2);
                ushort id = inner.ReadUInt16();
                if (!PipeFilter.IsClientRelayAllowed(id))
                {
                    PipeFilter.AuditDrop(uid, user.Owner.Player.Name, "client packet " + id + " inside an envelope dropped (server " + codec.PeerServerID + ")");
                    return;
                }
                Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                if (Program.MsgInvoker.TryGetInvoker(id, out hinvoker))
                {
                    try { hinvoker(user.Owner, inner); }
                    catch (Exception e) { MyConsole.WriteException(e); }
                }
            }
            finally
            {
                ServerSockets.PacketRecycle.Reuse(inner);
            }
        }

        public static void Send(ServerSockets.Packet stream)
        {
            /*if (!stream.EncryptPipeServer())
                return;*/
            foreach (var server in PollServers.Values)
                server.Send(stream);
        }
        public static void Accept(ServerSockets.SecuritySocket obj)
        {
            var user = new User(obj);
            obj.Client = user;
            obj.OnInterServer = true;
            bool blocked = PipeBackoff.IsBlocked(obj.RemoteIp);
            if (!Security.IsPeerAllowed(obj.RemoteIp) || blocked)
            {
                Conquer6609.Security.Guard.Audit("interserver", 0, "", "rejected pipe peer " + obj.RemoteIp + (blocked ? " (back-off after failed handshakes)" : ""));
                user.Disconnect();
                return;
            }
            // pipe v2: the listener speaks first with a fresh nonce; the peer must answer with an HMAC proof
            var codec = new PipeCodec(true, PipeLinkType.None);
            codec.ServerNonce = PipeCrypto.Nonce();
            obj.Pipe = codec;
            using (var rec = new ServerSockets.RecycledPacket())
                obj.Send(PipeHandshake.Hello(rec.GetStream(), codec.ServerNonce));
            PipeHandshake.ArmTimeout(obj, "pipe server");
        }

        public static unsafe void Receive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)
        {
            
            /*if (!stream.DecryptPipeServer())
                return;*/
            ushort PacketID = stream.ReadUInt16();
            var user = obj.Client as User;
            try
            {
                if (user == null || !user.Alive)
                    return;
                var codec = obj.Pipe;
                if (codec == null)
                {
                    user.Disconnect();
                    return;
                }
                // Nothing is processed until the peer proved it knows the secret (SecuritySocket only lets handshake ids through).
                if (!codec.Ready || !user.Authenticated)
                {
                    HandleAuth(user, obj, stream, PacketID);
                    return;
                }
                if (PacketID == PacketTypes.PipeHeartbeat || PacketID == PacketTypes.InterServer_Auth)
                    return;
                if (codec.Link == PipeLinkType.Control)
                {
                    // server data only
                    if (PacketID != PacketTypes.InterServer_ServerInfo && PacketID != PacketTypes.InterServer_EliteRank
                        && PacketID != PacketTypes.InterServer_UnionRanks && PacketID != Game.GamePackets.Chat)
                    {
                        PipeFilter.AuditDrop(0, "", "packet " + PacketID + " not allowed on the control link of server " + codec.PeerServerID);
                        return;
                    }
                }
                else
                {
                    if (PipeFilter.IsControlOnlyId(PacketID))
                    {
                        Conquer6609.Security.Guard.Audit("interserver", 0, "", "control-only packet " + PacketID + " on a relay link of server " + codec.PeerServerID + ", link closed");
                        user.Disconnect();
                        return;
                    }
                    if (PacketID == PacketTypes.RelayClient)
                    {
                        HandleEnvelope(user, codec, stream);
                        return;
                    }
                    // raw packets on a relay link: only the setup snapshot, and only until CompleteLogin
                    if (codec.SetupComplete || !PipeFilter.IsSetupId(PacketID))
                    {
                        PipeFilter.AuditDrop(0, "", "raw packet " + PacketID + " on a relay link of server " + codec.PeerServerID + (codec.SetupComplete ? " after the setup" : ""));
                        return;
                    }
                    if (PacketID == PacketTypes.InterServer_CheckTransfer && !PipeConfig.AllowLegacyTransfer)
                    {
                        Conquer6609.Security.Guard.Audit("interserver", 0, "", "legacy transfer request from server " + codec.PeerServerID + " refused ([InterServer] AllowLegacyTransfer=0)");
                        user.Disconnect();
                        return;
                    }
                    if (PacketID != Game.GamePackets.HeroInfo && PacketID != PacketTypes.InterServer_CheckTransfer
                        && (user.Owner == null || user.Owner.Player == null))
                    {
                        PipeFilter.AuditDrop(0, "", "setup packet " + PacketID + " before HeroInfo from server " + codec.PeerServerID);
                        return;
                    }
                    if (PacketID == Game.GamePackets.HeroInfo && user.Owner != null && user.Owner.FullLoading)
                    {
                        PipeFilter.AuditDrop(user.Owner.Player.RealUID, user.Owner.Player.Name, "second HeroInfo on a relay link dropped");
                        return;
                    }
                    if (PacketID == Game.GamePackets.DataMap)
                    {
                        // only the CompleteLogin action, which ends the setup phase
                        ushort action = stream.Size >= 26 + 8 ? *(ushort*)(stream.Memory + 24) : (ushort)0;
                        if (action != (ushort)ActionType.CompleteLogin || user.Owner == null || !user.Owner.FullLoading)
                        {
                            PipeFilter.AuditDrop(0, "", "raw 10010 action " + action + " on a relay link dropped");
                            return;
                        }
                        codec.SetupComplete = true;
                    }
                }
                switch (PacketID)
                {
                    case PacketTypes.InterServer_RoleInfo:
                        {
                            stream.GetInterServerRoleInfo(user.Owner.Player);
                            break;
                        }
                    case PacketTypes.InterServer_SpecialTitles:
                        {
                            string text;
                            stream.GetInterServerSpecialTitles(out text);

                            Database.ServerDatabase.LoadSpecialTitles(user.Owner, text);
                            break;
                        }
                    case PacketTypes.InterServer_CheckTransfer:
                        {
                            user.Owner = new Client.GameClient(obj, true);
                            user.Owner.PipeServer = user;

                            uint type;
                            uint UID;
                            stream.GetInterServerCheckTransfer(out type, out UID);
                            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + UID.ToString() + ".ini"))
                                type = 1;
                            else
                                type = 2;
                            user.Send(stream.InterServerCheckTransferCreate(type, UID));


                            break;
                        }
                    case PacketTypes.InterServer_Achievement:
                        {
                            string text;
                            stream.GetInterServerAchievement(out text);
                            user.Owner.Achievement = new Database.AchievementCollection();
                            user.Owner.Achievement.Load(text);
                            user.Owner.Player.Achievement = new Game.MsgServer.ClientAchievement(user.Owner.Achievement.Value,  user.Owner.Player.UID);
                            break;
                        }
                    case PacketTypes.InterServer_Chi:
                        {
                            uint ChiPoints = 0;
                            string Dragon;
                            string Phoenix;
                            string Turtle;
                            string Tiger;
                            stream.GetInterServerChi(out ChiPoints, out Dragon, out Phoenix, out Turtle, out Tiger);


                            user.Owner.Player.MyChi = new Role.Instance.Chi(user.Owner.Player.UID);
                            user.Owner.Player.MyChi.Name = user.Owner.Player.Name;
                            user.Owner.Player.MyChi.ChiPoints = (int)ChiPoints;
                            user.Owner.Player.MyChi.Dragon.Load(Dragon, user.Owner.Player.UID, user.Owner.Player.Name);
                            user.Owner.Player.MyChi.Phoenix.Load(Phoenix, user.Owner.Player.UID, user.Owner.Player.Name);
                            user.Owner.Player.MyChi.Turtle.Load(Turtle, user.Owner.Player.UID, user.Owner.Player.Name);
                            user.Owner.Player.MyChi.Tiger.Load(Tiger, user.Owner.Player.UID, user.Owner.Player.Name);

                            if (user.Owner.Player.MyChi.Dragon.UnLocked)
                            {
                                Role.Instance.Chi.ChiPool.TryAdd(user.Owner.Player.MyChi.UID, user.Owner.Player.MyChi);
                                Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, user.Owner.Player.MyChi.Dragon);
                            }
                            if (user.Owner.Player.MyChi.Phoenix.UnLocked)
                                Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, user.Owner.Player.MyChi.Phoenix);
                            if (user.Owner.Player.MyChi.Tiger.UnLocked)
                                Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, user.Owner.Player.MyChi.Tiger);
                            if (user.Owner.Player.MyChi.Turtle.UnLocked)
                                Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, user.Owner.Player.MyChi.Turtle);

                            break;
                        }
                    case PacketTypes.InterServer_JiangHu:
                        {
                            string text;
                            stream.GetInterServerJiangHu(out text);

                            Database.DBActions.ReadLine readerline = new Database.DBActions.ReadLine(text, '/');
                            readerline.Read((uint)0);
                            Role.Instance.JiangHu jiang = new Role.Instance.JiangHu(user.Owner.Player.UID); 
                            jiang.Name = readerline.Read("");
                            jiang.CustomizedName = readerline.Read("");
                            jiang.Level = readerline.Read((byte)0);
                            jiang.Talent = readerline.Read((byte)0);
                            jiang.FreeTimeToday = readerline.Read((uint)0);
                            jiang.OnJiangMode = readerline.Read((byte)0) == 1;
                            jiang.FreeCourse = readerline.Read((uint)0);
                            jiang.StartCountDwon = DateTime.Now;
                            jiang.CountDownEnd = DateTime.Now.AddSeconds(readerline.Read((uint)0));
                            jiang.RoundBuyPoints = readerline.Read((uint)0);

                            uint cStage = 0;
                            foreach (var stage in jiang.ArrayStages)
                            {
                                cStage += 1;
                                stage.Activate = readerline.Read((byte)0) == 1;

                                foreach (var star in stage.ArrayStars)
                                {

                                    star.Activate = readerline.Read((byte)0) == 1;
                                    star.UID = readerline.Read((ushort)0);
                                    if (star.Activate)
                                    {
                                        star.Typ = jiang.GetValueType(star.UID);
                                        star.Level = jiang.GetValueLevel(star.UID);
                                        if (cStage == 9)
                                        {
                                            if (star.Typ == Role.Instance.JiangHu.Stage.AtributesType.PAttack)
                                            {
                                                star.Typ = Role.Instance.JiangHu.Stage.AtributesType.MaxLife;
                                                star.UID = jiang.ValueToRoll(star.Typ, star.Level);
                                            }
                                        }
                                    }
                                }
                            }
                            jiang.CreateStatusAtributes(null);
                            break;
                        }
                    case PacketTypes.InterServer_InnerPower:
                        {
                          

                            string text;
                            stream.GetInterServerInnerPower(out text);
                            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(text, '/');
                            Role.Instance.InnerPower item = new Role.Instance.InnerPower(reader.Read(""), user.Owner.Player.UID);
                            reader.Read((uint)0);//uid!
                            item.Potency = reader.Read((int)0);
                            int Stages = reader.Read((int)0);
                            for (int i = 0; i < Stages; i++)
                            {
                                var Stage = item.Stages[i];
                                Stage.ID = reader.Read((ushort)0);
                                Stage.UnLocked = reader.Read((byte)0) == 1;
                                int count_neigongs = reader.Read((int)0);
                                for (int y = 0; y < count_neigongs; y++)
                                {
                                    var neigon = Stage.NeiGongs[y];
                                    neigon.ID = reader.Read((byte)0);
                                    neigon.Score = reader.Read((byte)0);
                                    neigon.Unlocked = reader.Read((byte)0) == 1;
                                    neigon.level = reader.Read((byte)0);
                                    neigon.Complete = reader.Read((byte)0) == 1;
                                }
                            }
                            if (Role.Instance.InnerPower.InnerPowerPolle.ContainsKey(item.UID))
                                Role.Instance.InnerPower.InnerPowerPolle[item.UID] = item;
                            else
                                Role.Instance.InnerPower.InnerPowerPolle.TryAdd(item.UID, item);
                            Role.Instance.InnerPower.InnerPowerRank.UpdateRank(item);
                            break;
                        }
                    case PacketTypes.InterServer_EliteRank:
                        {
                            
                                if (user.IsServer)
                                {
                                    stream.Seek(stream.Size - 8);
                                    stream.Finalize(PacketTypes.InterServer_EliteRank);
                                    foreach (var server in PollServers.Values)
                                        server.Send(stream);
                                }

                          
                            break;
                        }

                    case Game.GamePackets.Chat:
                        {
                           
                            if (Program.ServerConfig.IsInterServer)
                            {
                                var msg = new MsgMessage();
                                msg.Deserialize(stream);
                           
                                if (msg.ChatType == MsgMessage.ChatMode.CrosTheServer)
                                {
                                    stream.Seek(stream.Size -8);
                                    stream.Finalize(1004);
                                    if (user.IsServer)
                                    {

                                        foreach (var server in PollServers.Values)
                                            server.Send(stream);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                                            if (Program.MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                                            {
                                                hinvoker(user.Owner, stream);
                                            }
                                            else
                                            {
                                            }
                                        }
                                        catch (Exception e) { Console.WriteLine(e.ToString()); }
                                        
                                    }
                                }
                                else
                                {
                                    stream.Seek(stream.Size - 8);
                                    stream.Finalize(1004);
                                    goto default;
                                }
                            }
                            else
                                goto default;
                          
                            break;
                        }
                    case Game.GamePackets.Update:
                        {
                            
                                MsgUpdate.DataType Action;
                                ulong Value;
                                stream.GetUpdatePacket(out Action, out Value);

                                switch (Action)
                                {
                                    case MsgUpdate.DataType.ConquerPoints: user.Owner.Player.ConquerPoints = Security.ClampConquerPoints(Value); break;
                                    case MsgUpdate.DataType.BoundConquerPoints: user.Owner.Player.BoundConquerPoints = Security.ClampBoundConquerPoints(Value); break;
                                }
                          
                            break;
                        }
                    case PacketTypes.InterServer_UnionInfo:
                        {
                          
                                uint UID;
                                Role.Instance.Union.Member.MilitaryRanks rank;
                                string Name;
                                string LeaderName;
                                byte IsKingDom;
                                stream.GetUnionInfo(out UID, out rank, out Name, out LeaderName, out IsKingDom);
                                if (string.IsNullOrEmpty(Name) == false && string.IsNullOrEmpty(LeaderName) == false)
                                {
                                    Instance.Union.AddToUnion(stream, user.Owner, UID, rank, Name, LeaderName, IsKingDom);
                                }
                           
                            break;
                        }
                    case PacketTypes.InterServer_GuildInfo:
                        {
                           
                                uint UID;
                                Role.Flags.GuildMemberRank rank;
                                string GuildName;
                                string LeaderName;
                                stream.GetGuildInfo(out UID, out rank, out GuildName, out LeaderName);
                                if (string.IsNullOrEmpty(GuildName) == false && string.IsNullOrEmpty(LeaderName) == false)
                                    Instance.Guilds.AddToGuild(stream, user.Owner, UID, rank, GuildName, LeaderName);
                            
                            break;
                        }
                    case PacketTypes.InterServer_ServerInfo://server info.
                        {
                           
                                uint type;
                                uint ServerID; string ServerName; uint MapID; uint X; uint Y; uint Group;
                                stream.GetServerInfo(out type, out ServerID, out ServerName, out MapID, out X, out Y, out Group);
                                // a server registers only over its own authenticated control link
                                if (type == 1 && (obj.Pipe == null || obj.Pipe.Link != PipeLinkType.Control || ServerID != obj.Pipe.PeerServerID))
                                {
                                    Conquer6609.Security.Guard.Audit("interserver", 0, "", "ServerInfo registration for " + ServerID + " refused (not the control link of that server)");
                                    break;
                                }
                                if (type == 1 && ServerID < 10)
                                {
                                    user.ServerInfo = new Database.GroupServerList.Server()
                                    {
                                        ID = ServerID,
                                        Group = Group,
                                        MapID = MapID,
                                        Name = ServerName,
                                        X = X,
                                        Y = Y
                                    };

                                    if (!PollServers.ContainsKey(user.ServerInfo.ID))
                                        PollServers.Add(user.ServerInfo.ID, user);
                                    else
                                        PollServers[user.ServerInfo.ID] = user;
                                }
                        
                            break;
                        }
                    case PacketTypes.InterServer_UnionRanks://union Info.
                        {
                          
                                if (user.IsServer)
                                {
                                    Game.MsgServer.MsgLeagueRank.ActionID type; ushort count;
                                    ushort Page; byte dwparam; ushort PageCount;
                                    stream.GetLeagueRank(out type, out count, out Page, out dwparam, out PageCount);
                                    if (count == 1)
                                    {
                                        uint ServerID; uint GoldBricks; string Name; string LeaderName;
                                        stream.GetItemLeagueRank(out ServerID, out GoldBricks, out Name, out LeaderName);
                                        Instance.Union.AddUnion(ServerID, GoldBricks, Name, LeaderName);


                                        stream.LeagueRankCreate(MsgLeagueRank.ActionID.ShowAllUnions, (ushort)1, (ushort)0, (byte)0, (ushort)0);
                                        stream.AddItemLeagueRank(Database.GroupServerList.MyServerInfo.ID, GoldBricks, Name, LeaderName);

                                        stream.InterServerLeagueRankFinalize();
                                        foreach (var server in PollServers.Values)
                                            server.Send(stream);
                                    }
                                }
                           
                            break;
                        }

                    case Game.GamePackets.HeroInfo:
                        {
                            Role.Player player;

                            if (user.Owner == null)
                            {
                                user.Owner = new Client.GameClient(obj, true);
                                user.Owner.PipeServer = user;
                            }
                            user.Owner.Equipment = new Role.Instance.Equip(user.Owner);
                            stream.GetHeroInfo(user.Owner, out player);

                            if (player.InitTransfer == 1 && !PipeConfig.AllowLegacyTransfer)
                            {
                                Conquer6609.Security.Guard.Audit("interserver", player.RealUID, player.Name, "legacy transfer HeroInfo refused ([InterServer] AllowLegacyTransfer=0)");
                                user.Owner = null;
                                user.Disconnect();
                                break;
                            }
                            if (player.InitTransfer == 1)
                            {
                                player.UID = player.RealUID;

                                user.Owner.Player = player;
                                player.Owner.Map = Database.Server.ServerMaps[1002];
                                player.Owner.Map.View.EnterMap<Role.IMapObj>(player);
                                player.Map = 1002;
                                player.X = 241;
                                player.Y = 242;

                                player.SubClass = new Role.Instance.SubClass();
                                player.MyChi = new Role.Instance.Chi(player.UID);
                                player.InnerPower = new Role.Instance.InnerPower(player.Name, player.UID);
                                user.Owner.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
                                user.Owner.ArenaStatistic.ApplayInfo(user.Owner.Player);
                                user.Owner.ArenaStatistic.Info.ArenaPoints = 4000;
                                user.Owner.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
                                user.Owner.TeamArenaStatistic.ApplayInfo(user.Owner.Player);
                                user.Owner.TeamArenaStatistic.Info.ArenaPoints = 4000;

                                if (user.Owner.Player.Flowers == null)
                                {
                                    user.Owner.Player.Flowers = new Role.Instance.Flowers(user.Owner.Player.UID, user.Owner.Player.Name);
                                    user.Owner.Player.Flowers.FreeFlowers = 1;
                                }
                                if (user.Owner.Player.Nobility == null)
                                    user.Owner.Player.Nobility = new Role.Instance.Nobility(user.Owner);
                                if (user.Owner.Player.Associate == null)
                                {
                                    user.Owner.Player.Associate = new Role.Instance.Associate.MyAsociats(user.Owner.Player.UID);
                                    user.Owner.Player.Associate.MyClient = user.Owner;
                                    user.Owner.Player.Associate.Online = true;
                                }
                                player.Owner.Inventory = new Role.Instance.Inventory(player.Owner);

                                player.Owner.Warehouse = new Role.Instance.Warehouse(player.Owner);
                                player.Owner.MyProfs = new Role.Instance.Proficiency(player.Owner);
                                player.Owner.MySpells = new Role.Instance.Spell(player.Owner);

                                if (player.Owner.Achievement == null)
                                    player.Owner.Achievement = new Database.AchievementCollection();
                                Database.Server.GamePoll.TryAdd(user.Owner.Player.UID, user.Owner);


                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var msg = rec.GetStream();
                                    obj.Send(msg.MsgInterServerIdentifier(0, player.RealUID, 0));
                                }

                                user.Owner.Player.View.Role();
                                user.Owner.FullLoading = true;

                            }
                            else
                            {
                                // user.Owner = new Client.GameClient(obj, true);
                                // user.Owner.PipeServer = user;

                                uint New_UID = UIDCounter.Next;

                                player.UID = New_UID;

                                user.Owner.Player = player;
                                player.Owner.Map = Database.Server.ServerMaps[1002];
                                player.Owner.Map.View.EnterMap<Role.IMapObj>(player);
                                player.Map = 1002;
                                player.X = 152;
                                player.Y = 219;


                                player.SubClass = new Role.Instance.SubClass();
                                player.MyChi = new Role.Instance.Chi(player.UID);
                                player.InnerPower = new Role.Instance.InnerPower(player.Name, player.UID);
                                user.Owner.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
                                user.Owner.ArenaStatistic.ApplayInfo(user.Owner.Player);
                                user.Owner.ArenaStatistic.Info.ArenaPoints = 4000;
                                user.Owner.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
                                user.Owner.TeamArenaStatistic.ApplayInfo(user.Owner.Player);
                                user.Owner.TeamArenaStatistic.Info.ArenaPoints = 4000;

                                if (user.Owner.Player.Flowers == null)
                                {
                                    user.Owner.Player.Flowers = new Role.Instance.Flowers(user.Owner.Player.UID, user.Owner.Player.Name);
                                    user.Owner.Player.Flowers.FreeFlowers = 1;
                                }
                                if (user.Owner.Player.Nobility == null)
                                    user.Owner.Player.Nobility = new Role.Instance.Nobility(user.Owner);
                                if (user.Owner.Player.Associate == null)
                                {
                                    user.Owner.Player.Associate = new Role.Instance.Associate.MyAsociats(user.Owner.Player.UID);
                                    user.Owner.Player.Associate.MyClient = user.Owner;
                                    user.Owner.Player.Associate.Online = true;
                                }
                                player.Owner.Inventory = new Role.Instance.Inventory(player.Owner);
                                player.Owner.Equipment = new Role.Instance.Equip(player.Owner);
                                player.Owner.Warehouse = new Role.Instance.Warehouse(player.Owner);
                                player.Owner.MyProfs = new Role.Instance.Proficiency(player.Owner);
                                player.Owner.MySpells = new Role.Instance.Spell(player.Owner);

                                if (player.Owner.Achievement == null)
                                    player.Owner.Achievement = new Database.AchievementCollection();
                                Database.Server.GamePoll.TryAdd(user.Owner.Player.UID, user.Owner);


                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var msg = rec.GetStream();
                                    obj.Send(msg.MsgInterServerIdentifier(0, player.RealUID, New_UID));
                                }

                                user.Owner.Player.View.Role();
                                user.Owner.FullLoading = true;
                                SetLocation(user.Owner);

                                if (Program.ServerConfig.IsInterServer)
                                    user.Owner.Player.SetPkMode(Role.Flags.PKMode.Union);
                                else
                                    user.Owner.Player.SetPkMode(Role.Flags.PKMode.Peace);


                                user.Owner.Player.SendString(stream, (MsgStringPacket.StringID)60, false, null);

                                user.Owner.Player.ActiveSpecialTitles(stream);
                                user.Owner.Player.Stamina = 100;
                                user.Owner.Player.SendUpdate(stream, user.Owner.Player.Stamina, MsgUpdate.DataType.Stamina);

                            }
                            break;
                        }
                    case Game.GamePackets.Item:
                        {
                          
                                MsgGameItem item;
                                stream.GetItemPacketPacket(out item);
                                switch (item.Position)
                                {
                                    case 0:
                                        {

                                            user.Owner.Inventory.AddDBItem(item);
                                            if (user.Owner.Inventory.ClientItems.ContainsKey(item.UID))
                                            {
                                                if (user.Owner.Inventory.ClientItems[item.UID].StackSize != item.StackSize)
                                                    user.Owner.Inventory.ClientItems[item.UID] = item;
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            if (item.Position > 0 && item.Position <= (ushort)Role.Flags.ConquerItem.Wing)
                                            {
                                                user.Owner.Equipment.Add(item, stream);//.ClientItems.TryAdd(item.UID, item);
                                            }
                                            else if (item.Position > (ushort)Role.Flags.ConquerItem.Wing && item.Position <= (ushort)Role.Flags.ConquerItem.AleternanteGarment)
                                            {
                                                user.Owner.Equipment.ClientItems.TryAdd(item.UID, item);
                                            }
                                            break;
                                        }
                                }
                          
                            break;
                        }
                    case Game.GamePackets.ExtraItem:
                        {

                            MsgItemExtra.Refinery refinary;
                            MsgItemExtra.Purification purification;
                            stream.GetExtraItem(out purification, out refinary);
                            MsgGameItem Item;
                            if (user.Owner.TryGetItem(refinary.ItemUID, out Item))
                                Item.Refinary = refinary;
                            if (user.Owner.TryGetItem(purification.ItemUID, out Item))
                                Item.Purification = purification;
                            if (Item != null)
                                Item.Send(user.Owner, stream);
                            // user.Owner.Equipment.QueryEquipment(user.Owner.Equipment.Alternante);

                            break;
                        }
                    case Game.GamePackets.SubClass:
                        {
                            
                                MsgSubClass.Action action;
                                MsgSubClass.SubClases[] src;
                                stream.GetSubClass(out action, out src);
                                for (int x = 0; x < src.Length; x++)
                                    user.Owner.Player.SubClass.src.TryAdd(src[x].ID, src[x]);
                          
                            break;
                        }
                    case Game.GamePackets.MsgStauts:
                        {
                            
                                uint Type;
                                uint UID;
                                Game.MsgServer.MsgStatus Jiang_status;
                                stream.GetJiangStatus(out Type, out UID, out Jiang_status);
                                if (Type == 1)
                                {
                                    user.Owner.Player.MyJiangHu = new Role.Instance.JiangHu(user.Owner.Player.UID)
                                    {
                                        Name = user.Owner.Player.Name,
                                        Level = (byte)user.Owner.Player.Level,
                                        statusclient = Jiang_status
                                    };
                                    //       user.Owner.Equipment.QueryEquipment(user.Owner.Equipment.Alternante);
                                }
                           
                            break;
                        }
                    case Game.GamePackets.ChiInfo:
                        {
                           
                                MsgChiInfo.Action type;
                                uint CriticalStrike;
                                uint SkillCriticalStrike;
                                uint Immunity;
                                uint Breakthrough;
                                uint Counteraction;
                                uint MaxLife;
                                uint AddAttack;
                                uint AddMagicAttack;
                                uint AddMagicDefense;
                                uint FinalAttack;
                                uint FinalMagicAttack;
                                uint FinalDefense;
                                uint FinalMagicDefense;
                                stream.GetChiHandler(out type, out CriticalStrike, out SkillCriticalStrike, out Immunity
                                    , out Breakthrough, out Counteraction, out MaxLife, out AddAttack
                                    , out AddMagicAttack, out AddMagicDefense, out FinalAttack, out FinalMagicAttack
                                    , out FinalDefense, out FinalMagicDefense);

                                if (user.Owner.Player.MyChi != null && type == MsgChiInfo.Action.InterServerStatus)
                                {
                                    user.Owner.Player.MyChi.CriticalStrike = CriticalStrike;
                                    user.Owner.Player.MyChi.SkillCriticalStrike = SkillCriticalStrike;
                                    user.Owner.Player.MyChi.Immunity = Immunity;
                                    user.Owner.Player.MyChi.Breakthrough = Breakthrough;
                                    user.Owner.Player.MyChi.Counteraction = Counteraction;
                                    user.Owner.Player.MyChi.MaxLife = MaxLife;
                                    user.Owner.Player.MyChi.AddAttack = AddAttack;
                                    user.Owner.Player.MyChi.AddMagicAttack = AddMagicAttack;
                                    user.Owner.Player.MyChi.AddMagicDefense = AddMagicDefense;
                                    user.Owner.Player.MyChi.FinalAttack = FinalAttack;
                                    user.Owner.Player.MyChi.FinalMagicAttack = FinalMagicAttack;
                                    user.Owner.Player.MyChi.FinalDefense = FinalDefense;
                                    user.Owner.Player.MyChi.FinalMagicDefense = FinalMagicDefense;
                                }
                           
                            //  user.Owner.Equipment.QueryEquipment(user.Owner.Equipment.Alternante);
                            break;
                        }
                    case Game.GamePackets.Spell:
                        {
                          
                                MsgSpell spell;
                                stream.GetSpell(out spell);
                                if (user.Owner.MySpells.ClientSpells.ContainsKey(spell.ID) == false)
                                    user.Owner.MySpells.ClientSpells.TryAdd(spell.ID, spell);
                        
                            break;
                        }
                    case Game.GamePackets.Proficiency:
                        {
                           
                                MsgProficiency prof;
                                stream.GetProficiency(out prof);
                                if (user.Owner.MyProfs.ClientProf.ContainsKey(prof.ID) == false)
                                    user.Owner.MyProfs.ClientProf.TryAdd(prof.ID, prof);
                          
                            break;
                        }
                    default:
                        {
                            try
                            {
                                // Game packets are only routed for a player connection with a loaded character.
                                if (user.IsServer || user.Owner == null || user.Owner.Player == null || !user.Owner.FullLoading)
                                {
                                    Conquer6609.Security.Guard.Audit("interserver", 0, "", "dropped pipe packet " + PacketID + " from " + obj.RemoteIp);
                                    break;
                                }


                                Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                                if (Program.MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                                {
                                    hinvoker(user.Owner, stream);
                                }
                                else
                                {
                                }

                            }
                            catch (Exception e) { Console.WriteLine("hhh "+PacketID); MyConsole.WriteException(e); }
                            
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(PacketID);
                MyConsole.SaveException(e);
            }
            finally
            {
                ServerSockets.PacketRecycle.Reuse(stream);
            }
        }
        public static void SetLocation(Client.GameClient user)
        {
            switch (user.Player.SetLocationType)
            {
                case 1://elite pk
                    {
                        Game.MsgTournaments.MsgSchedules.ElitePkTournament.SignUp(user);
                        break;
                    }
                default:
                    {
                        if (Database.GroupServerList.MyServerInfo.ID == Database.GroupServerList.InterServer.ID)
                        {
                            foreach (var server in Database.GroupServerList.GroupServers.Values)
                            {
                                if (server.ID == user.Player.ServerID)
                                {
                                    user.Teleport((ushort)server.X, (ushort)server.Y, (ushort)server.MapID);
                                }
                            }
                        }
                        else
                        {
                            user.Teleport(432, 390, 1002);
                        }
                        break;
                    }
            }
            return;
            //foreach (var server in PollServers.Values)
            //{
            //    if (server.ServerInfo.ID == user.Player.ServerID)
            //    {
            //        user.Teleport((ushort)server.ServerInfo.X, (ushort)server.ServerInfo.Y, (ushort)server.ServerInfo.MapID);
            //        break;
            //    }
            //}
        }
        public static void Disconnect(ServerSockets.SecuritySocket obj)
        {
            var user = obj.Client as User;
            if (user == null)
                return;
            var codec = obj.Pipe;
            if (codec != null && codec.Link == PipeLinkType.Control)
            {
                User current;
                if (ControlLinks.TryGetValue(codec.PeerServerID, out current) && current == user)
                    ControlLinks.TryRemove(codec.PeerServerID, out current);
                if (user.ServerInfo != null)
                {
                    User registered;
                    if (PollServers.TryGetValue(user.ServerInfo.ID, out registered) && registered == user)
                        PollServers.Remove(user.ServerInfo.ID);
                }
            }
            if (user.IsServer == false && user.Owner != null)
            {
                if (user.Owner.Map != null && user.Owner.Player != null)
                {
                    if (user.Owner.Player.SetLocationType == 1)//elitepk
                    {
                            user.Owner.EndQualifier();
                    }
                    user.Owner.Map.Denquer(user.Owner);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        user.Owner.Player.View.Clear(stream);
                    }
                    Client.GameClient client;
                    if (Database.Server.GamePoll.TryRemove(user.Owner.Player.UID, out client))
                    {
                        if (client.Player.InitTransfer == 2)
                        {
                            client.Player.InitTransfer = 0;
                            client.ClientFlag |= Client.ServerFlag.Disconnect;
                            client.ClientFlag |= Client.ServerFlag.QueuesSave;
                            Database.ServerDatabase.LoginQueue.TryEnqueue(client);
                        }
                    }

                }
            }
            user.Disconnect();
        }
    }
}
