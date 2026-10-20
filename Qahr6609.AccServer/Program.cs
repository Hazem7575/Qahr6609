using System;
using COShield;
using AccServer.Network;
using AccServer.Database;
using System.Windows.Forms;
using AccServer.Network.Sockets;
using AccServer.Network.AuthPackets;
using System.Threading.Tasks;
using System.Text;
using static COShield.COShield;

namespace AccServer
{
    public unsafe class Program
    {
        public static Counter EntityUID;
        public static FastRandom Random = new FastRandom();
        public static ServerSocket AuthServer;
        public static ServerSocket WebServer;
        private static ClientWrapper _WebServer = null;
        public static World World;
        public static ushort LoginPort = 9961;
        private static object SyncLogin;
        private static System.Collections.Concurrent.ConcurrentDictionary<uint, int> LoginProtection;
        private const int TimeLimit = 10000;
        private static void WorkConsole()
        {
            while (true)
            {
                try
                {
                    CommandsAI(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=======================================================");
            Console.WriteLine("");
            Console.Title = "Account Server.";

            World = new World();
            World.Init();
            EntityUID = new Counter(0);
            LoginProtection = new System.Collections.Concurrent.ConcurrentDictionary<uint, int>();
            SyncLogin = new object();
            Database.Server.Load();
            LoginTokens.CheckTable();
            PasswordHasher.MigrateAll();
            Network.Cryptography.AuthCryptography.PrepareAuthCryptography();
            AuthServer = new ServerSocket();
            AuthServer.OnClientConnect += AuthServer_OnClientConnect;
            AuthServer.OnClientReceive += AuthServer_OnClientReceive;
            AuthServer.OnClientDisconnect += AuthServer_OnClientDisconnect;
            AuthServer.Enable(LoginPort, "0.0.0.0");


            Console.WriteLine("");
            Console.WriteLine("Listening to port [ " + LoginPort + " ] .");
            Console.WriteLine("=======================================================");
            WorkConsole();
            CommandsAI(Console.ReadLine());
        }

        public static void CommandsAI(string command)
        {
            if (command == null) return;
            string[] data = command.Split(' ');
            switch (data[0])
            {
                case "@memory":
                    {
                        var proc = System.Diagnostics.Process.GetCurrentProcess();
                        Console.WriteLine("Thread count: " + proc.Threads.Count);
                        Console.WriteLine("Memory set(MB): " + ((double)((double)proc.WorkingSet64 / 1024)) / 1024);
                        proc.Close();
                        break;
                    }
                case "@clear":
                    {
                        Console.Clear();
                        break;
                    }

                case "@restart":
                    {
                        AuthServer.Disable();
                        Application.Restart();
                        Environment.Exit(0);
                        break;
                    }

            }
        }


        private static void AuthServer_OnClientReceive(byte[] buffer, int length, ClientWrapper arg3)
        {

            var player = arg3.Connector as Client.AuthClient;
            if (player == null || player.Closing) return;
            player.Cryptographer.Decrypt(buffer, length);
            player.Queue.Enqueue(buffer, length);
            while (player.Queue.CanDequeue())
            {
                if (player.Closing) return;
                byte[] packet = player.Queue.Dequeue();
                ushort _pID = BitConverter.ToUInt16(packet, 2);
                if (_pID == (ushort)1952)
                {
                    Forward Fw = new Forward();
                    // Only a limited number of authentication requests are accepted per connection.
                    if (++player.Attempts > LoginGuard.MaxAttemptsPerConnection)
                    {
                        Fw.Type = Forward.ForwardType.InvalidInfo;
                        player.Send(Fw);
                        player.DisconnectLater(1000);
                        return;
                    }

                    player.Info = new COShield.COShield.Authentication(packet);
                    if (player.Info == null || String.IsNullOrEmpty(player.Info.Server) || String.IsNullOrEmpty(player.Info.Username))
                    {
                        Fw.Type = Forward.ForwardType.InvalidAuthenticationProtocol;
                        player.Send(Fw);
                        player.DisconnectLater(1000);
                        return;
                    }

                    // Locked IPs / usernames (per source IP) are refused before any credential check.
                    string lockReason = LoginGuard.LockReason(player.IP, player.Info.Username);
                    if (lockReason != null)
                    {
                        Console.WriteLine("Login refused ({2}): UserName [{0}] IP [{1}]", player.Info.Username, player.IP, lockReason);
                        LoginGuard.Audit(lockReason, player.Info.Username, "login refused, ip " + player.IP);
                        Fw.Type = Forward.ForwardType.InvalidInfo;
                        player.Send(Fw);
                        player.DisconnectLater(1000);
                        return;
                    }

                    Database.ServerInfo Server = null;
                    if (!Database.Server.Servers.TryGetValue(player.Info.Server, out Server))
                    {
                        Fw.Type = Forward.ForwardType.InvalidInfo;
                        player.Send(Fw);
                        player.DisconnectLater(1000);
                        return;
                    }

                    player.Account = new AccountTable(player.Info.Username);

                    // accounts.Password holds a Laravel-style bcrypt hash; a row still in plain text is
                    // accepted once and rehashed (Database\PasswordHasher.cs).
                    bool needsRehash = false;
                    bool passwordOk = player.Account.exists
                        && PasswordHasher.Verify(player.Info.Password, player.Account.Password, out needsRehash);
                    if (passwordOk && needsRehash)
                        PasswordHasher.RehashOnLogin(player.Info.Username, player.Info.Password);
                    if (!passwordOk)
                    {
                        LoginGuard.RegisterFailure(player.IP, player.Info.Username);
                        Console.WriteLine("Login failed: UserName [{0}] IP [{1}]", player.Info.Username, player.IP);
                        LoginGuard.Audit("login-failed", player.Info.Username, (player.Account.exists ? "bad password" : "unknown account") + ", ip " + player.IP);
                        Fw.Type = Forward.ForwardType.InvalidInfo;
                        player.Send(Fw);
                        player.DisconnectLater(1000);
                        return;
                    }
                    LoginGuard.RegisterSuccess(player.IP, player.Info.Username);

                    // Banned and not-yet-activated accounts never receive a login handoff.
                    if (player.Account.State == AccountTable.AccountState.Banned || player.Account.State == AccountTable.AccountState.NotActivated)
                    {
                        Console.WriteLine("Login refused (account state {2}): UserName [{0}] IP [{1}]", player.Info.Username, player.IP, (int)player.Account.State);
                        Fw.Type = Forward.ForwardType.Banned;
                        player.Send(Fw);
                        player.DisconnectLater(1000);
                        return;
                    }

                    lock (SyncLogin)
                    {
                        Fw.Type = Forward.ForwardType.Ready;
                        if (player.Account.EntityID == 0)
                        {
                            using (MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT))
                            {
                                cmd.Select("configuration");
                                using (MySqlReader r = new MySqlReader(cmd))
                                {
                                    if (r.Read())
                                    {
                                        EntityUID = new Counter(r.ReadUInt32("EntityID"));
                                        player.Account.EntityID = EntityUID.Next;
                                        using (MySqlCommand cmd2 = new MySqlCommand(MySqlCommandType.UPDATE).Update("configuration")
                                        .Set("EntityID", player.Account.EntityID)) cmd2.Execute();

                                    }
                                }
                            }
                        }

                        player.Account.UpdatePCInfo(player.Info.MacAddress, "");
                        // One-time handoff record verified by the game server (same EntityID, same IP, short lifetime).
                        // Without it the game server refuses the character, so never hand off when it was not stored.
                        uint loginToken;
                        if (!LoginTokens.Issue(player.Account.EntityID, player.IP, Server.ServerID, out loginToken))
                        {
                            Console.WriteLine("Login aborted (login token not stored, see docs/security/login_tokens.sql): UserName [{0}] IP [{1}]", player.Info.Username, player.IP);
                            LoginGuard.Audit("login-token-error", player.Info.Username, "token insert failed for EntityID " + player.Account.EntityID + ", ip " + player.IP);
                            Fw.Type = Forward.ForwardType.InvalidInfo;
                            player.Send(Fw);
                            player.DisconnectLater(1000);
                            return;
                        }
                        // [Security] the identifier is the random token of login_tokens (the game server finds the account by it);
                        // it was Encrypt(EntityID): fixed and reversible, so a known UID was enough to claim the handoff
                        Fw.Identifier = loginToken;
                        Fw.IP = Server.IP;
                        Fw.Port = Server.Port;
                        Console.ForegroundColor = ConsoleColor.Green;
                        string log = string.Format("UserName [{0}] IP [{2}] Logged Into ==> {1}", player.Info.Username, Server.Name + (Server.ServerID != 0 ? " (id " + Server.ServerID + ")" : ""), player.IP);
                        Console.WriteLine(log);
                        player.Send(Fw);
                    }
                }
                else
                {
                    Forward Fw = new Forward();
                    Fw.Type = Forward.ForwardType.InvalidInfo;
                    player.Send(Fw);
                }
            }
        }
        private static void AuthServer_OnClientDisconnect(ClientWrapper obj)
        {
            obj.Disconnect();
        }
        private static void AuthServer_OnClientConnect(ClientWrapper obj)
        {

            Client.AuthClient authState;
            obj.Connector = (authState = new Client.AuthClient(obj));
            authState.Cryptographer = new Network.Cryptography.AuthCryptography();
            Network.AuthPackets.PasswordCryptographySeed pcs = new PasswordCryptographySeed();
            pcs.Seed = Program.Random.Next();
            authState.PasswordSeed = pcs.Seed;
            authState.Send(pcs);
        }
    }
} 