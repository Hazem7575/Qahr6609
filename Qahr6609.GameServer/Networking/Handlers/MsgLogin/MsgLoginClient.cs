
using Conquer6609.Game.MsgServer;
using Conquer6609.ServerSockets;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySqlNs = MySqlConnector; // [net10] .NET 10 build


namespace Conquer6609.Game.MsgServer
{
    public struct MsgLoginClient
    {
        public ushort Length;
        public ushort PacketID;
        public uint AccountHash;
        public uint PlayerUID;
        public uint Key, HDSerial;
        public string MachineName, HWID, MacAddress, username;
        public bool VMware, VPSMachine;
        private static readonly uint[] keys = new uint[16]
{
    0x3F2A9D1Cu, 0x5C7E8A4Bu, 0x2B1F6D39u, 0x7A4C3E91u,
    0x6D3B1F72u, 0x4E2C8A5Fu, 0x1B9D3C7Au, 0x3A7F9E2Du,
    0x5E1C6B3Au, 0x6F2D7C1Bu, 0x7B4A2E5Cu, 0x2D8C3F91u,
    0x4F9E1B7Au, 0x3C7D2A6Eu, 0x1A5B4E93u, 0x6E3F7D2Cu
};

        public static uint Decrypt(uint value)
        {
            for (int i = 15; i >= 0; --i) 
            {
                value = (uint)(value - keys[(i + 3) % 16]);

                value = (uint)(value ^ (keys[i] >> 16));

                value = (value >> 5) | (value << 27);

                value = (uint)((value - keys[15 - i]) ^ keys[i]);
            }
            return value;
        }

        public unsafe static void LoginHandler(Client.GameClient client, MsgLoginClient packet)
        {
            try
            {
                client.ClientFlag &= ~Client.ServerFlag.OnLoggion;
            }
            catch (Exception e) { MyConsole.WriteException(e); }

            if (client.Socket != null)
            {
                if (client.Socket == null || client.Socket.RemoteIp == "NONE")
                {
                    Console.WriteLine("Break login client: Socket null or RemoteIp NONE");
                    return;
                }
            }
            try
            {
                // [Security] the identifier is the random token the account server stored in login_tokens.Token: the account is
                // looked up by it (was Decrypt(identifier) = the EntityID, fixed and reversible: a known UID could claim the handoff)
                uint byToken;
                // after "character created" the client sends the login again on the same connection: its token is
                // already used, so take the UID this connection was verified for
                if (!LoginTokenCheck.VerifiedUidOf(client, out byToken) && !LoginTokenCheck.ResolveToken(client.Key, out byToken))
                {
                    Conquer6609.Security.Guard.Audit("login-rejected", 0, "", "unknown or expired login token ip=" + client.Socket.RemoteIp);
                    Game.MsgLoader.HazemShield.SendForceClose(client, "Login", "Your login session is invalid or expired. Please log in again.");
                    return;
                }
                client.Key = byToken;
                string BanMessaje;
                if (Database.SystemBanned.IsBanned(client.Socket.RemoteIp, out BanMessaje))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";

                        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                    }
                    return;
                }
                if (client.Key < 1000000 || client.Key > 19000000)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        client.Send(new MsgServer.MsgMessage("You can't login!", "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                    }
                    return;
                }

                // The character is loaded only after the account server's one-time handoff for this
                // UID and IP has been verified; nothing below (including kicking an online session) runs before it.
                if (client.IsBeingDisconnected)
                    return;
                string tokenFailure;
                if (!LoginTokenCheck.Verify(client, client.Key, out tokenFailure))
                {
                    Conquer6609.Security.Guard.Audit("login-rejected", client.Key, "", tokenFailure + " ip=" + client.Socket.RemoteIp);
                    Console.WriteLine("Login rejected for UID " + client.Key + " [" + client.Socket.RemoteIp + "]: " + tokenFailure);
                    Game.MsgLoader.HazemShield.SendForceClose(client, "Login", "Your login session is invalid or expired. Please log in again.");
                    return;
                }
                if (LoginTokenCheck.AccountBanned(client.Key))// [Security] banned after the account server issued the token
                {
                    Conquer6609.Security.Guard.Audit("login-rejected", client.Key, "", "account banned ip=" + client.Socket.RemoteIp);
                    Game.MsgLoader.HazemShield.SendForceClose(client, "Login", "This account is banned.");
                    return;
                }
                LoginTokenCheck.ApplyAccountRole(client, client.Key);

                // Count this machine's sessions without snapshotting the pool; the count is only
                // compared against 3 and 10, so stop as soon as the highest limit is reached.
                int count = 0;
                foreach (var kv in Database.Server.GamePoll)
                {
                    if (kv.Value.TqSerial == client.TqSerial && ++count >= 10)
                        break;
                }

                bool isUnlocked = Database.UnLockLogin.IsUnlocked(client, out BanMessaje);

                if (!isUnlocked && count >= 3)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Send(new MsgServer.MsgMessage(
                            "You can only open 3 accounts. Contact support to unlock up to 10 accounts.",
                            "ALLUSERS",
                            MsgMessage.MsgColor.red,
                            MsgMessage.ChatMode.Dialog).GetArray(stream));
                    }
                    return;
                }

                if (isUnlocked && count >= 10)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Send(new MsgServer.MsgMessage(
                            "You have reached the maximum limit of 10 accounts.",
                            "ALLUSERS",
                            MsgMessage.MsgColor.red,
                            MsgMessage.ChatMode.Dialog).GetArray(stream));
                    }
                    return;
                }

                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) == Client.ServerFlag.CreateCharacterSucces)
                {
                    if (Database.ServerDatabase.AllowCreate(client.ConnectionUID))
                    {

                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacterSucces;
                        if (client.Player.MyChi == null)
                        {
                            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);

                        }
                        if (client.Player.SubClass == null)
                            client.Player.SubClass = new Role.Instance.SubClass();
                        if (client.Player.Flowers == null)
                        {
                            client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
                            client.Player.Flowers.FreeFlowers = 1;
                        }
                        if (client.Player.Nobility == null)
                            client.Player.Nobility = new Role.Instance.Nobility(client);
                        if (client.Player.Associate == null)
                        {
                            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                            client.Player.Associate.MyClient = client;
                            client.Player.Associate.Online = true;
                        }
                        if (client.Player.InnerPower == null)
                            client.Player.InnerPower = new Role.Instance.InnerPower(client.Player.Name, client.Player.UID);

                        Database.Server.GamePoll.TryAdd(client.Player.UID, client);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage("ANSWER_OK", "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));


                            Database.ServerDatabase.CreateCharacte(client);
                            Database.ServerDatabase.SaveClient(client);

                            client.ClientFlag |= Client.ServerFlag.AcceptLogin;

                            Console.WriteLine("Client " + client.Player.Name + " was login on [" + client.Socket.RemoteIp + "] [UID : " + client.Player.UID + " ]");


                            client.Send(stream.LoginHandlerCreate(1, client.Player.Map));
                            MsgLoginHandler.LoadMap(client, stream);
                        }
                        return;
                    }
                }
                if ((client.ClientFlag & Client.ServerFlag.AcceptLogin) != Client.ServerFlag.AcceptLogin)
                {
                    uint encryptedIdentifier = client.Key;


                    client.ConnectionUID = client.Key;


                    if (Database.SystemBannedAccount.IsBanned(client.ConnectionUID, out BanMessaje))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            string aMessaj = "Your account is Banned for: " + BanMessaje + " ";

                            client.Send(new MsgServer.MsgMessage(aMessaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                        return;
                    }

                    if (Database.SystemBannedPC.IsBanned(client, out BanMessaje))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage(BanMessaje, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                        return;
                    }
                    string Messaj = "NEW_ROLE";



                    if (Database.ServerDatabase.AllowCreate(client.Key) == false)
                    {

                        Client.GameClient InGame = null;
                        if (Database.Server.GamePoll.TryGetValue((uint)client.Key, out InGame))
                        {
                            if (InGame.Player != null)
                            {
                                Console.WriteLine("Account try join but is in use. " + InGame.Player.Name);

                                if (InGame.Player.UID == 0)
                                {
                                    Database.Server.GamePoll.TryRemove((uint)client.Key, out InGame);
                                    if (InGame != null && InGame.Player != null)
                                    {
                                        if (InGame.Map != null)
                                            InGame.Map.Denquer(InGame);
                                    }
                                }
                            }
                            InGame.Socket.Disconnect();
                            Messaj = "Account Disconnected. Please try again.";

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                            }
                            if (InGame.TRyDisconnect-- == 0)
                            {
                                if (InGame.Player != null && InGame.FullLoading)
                                {
                                    InGame.ClientFlag |= Client.ServerFlag.Disconnect;
                                    //if ((InGame.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                                    Database.ServerDatabase.SaveClient(InGame);
                                }
                                Database.Server.GamePoll.TryRemove((uint)client.Key, out InGame);
                                if (InGame != null && InGame.Player != null)
                                {
                                    if (InGame.Map != null)
                                        InGame.Map.Denquer(InGame);
                                }
                            }
                        }
                        else
                        {
                            Database.Server.GamePoll.TryAdd((uint)client.Key, client);
                            Messaj = "ANSWER_OK";
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) != Client.ServerFlag.CreateCharacterSucces)
                                {
                                    try
                                    {
                                        Database.ServerDatabase.LoadCharacter(client, (uint)client.Key);
                                    }
                                    catch (Exception loadError)
                                    {
                                        // a character that failed to load must never stay in the online pool,
                                        // but only this client's entry is removed (never another session's)
                                        ((ICollection<KeyValuePair<uint, Client.GameClient>>)Database.Server.GamePoll).Remove(
                                            new KeyValuePair<uint, Client.GameClient>((uint)client.Key, client));
                                        Security.Guard.Audit("login-load-failed", (uint)client.Key, "", loadError.Message);
                                        string reason = loadError is InvalidDataException
                                            ? "Your character data failed a safety check and was not loaded, so nothing was lost or overwritten. Please contact the server owner."
                                            : "Your character could not be loaded. Please try again later or contact the server owner.";
                                        try
                                        {
                                            // shows a dialog, then disconnects after a short delay
                                            Game.MsgLoader.HazemShield.SendForceClose(client, "Login", reason);
                                        }
                                        catch
                                        {
                                            if (client.Socket != null)
                                                client.Socket.Disconnect();
                                        }
                                        return;
                                    }

                                    //if (client.Player.Name != "Solly[GM]" )//&& client.Player.Name != "NCnotorouiS" && client.Player.Name != "GrimReaper")
                                    //    client.Socket.Disconnect();
                                }
                                client.ClientFlag |= Client.ServerFlag.AcceptLogin;
                                Console.WriteLine("Client " + client.Player.Name + " was login on [" + client.Socket.RemoteIp + "]");
                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                                client.Send(stream.LoginHandlerCreate(1, client.Player.Map));
                                MsgLoginHandler.LoadMap(client, stream);
                                Conquer6609.ServerSockets.LoginController.LoggedIn(client.Socket.RemoteIp);
                            }
                        }
                    }
                    else//new client
                    {
                        client.ClientFlag |= Client.ServerFlag.CreateCharacter;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                    }

                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
    }

    /// <summary>Game-server side of the login handoff (table `login_tokens`, see docs/security/login_tokens.sql).
    /// A login is accepted only when the account server issued a token for that EntityID, from the same IP,
    /// that is unused and not expired; the token is consumed atomically. Switch: shell.ini [Security] VerifyLoginTokens (default 1).
    /// [Security] VerifyTokenIp (default 1): 0 ignores the IP column; 1 requires the same IP, where loopback and
    /// private LAN addresses all count as one "local" address (owner playing on the host / LAN).</summary>
    internal static class LoginTokenCheck
    {
        sealed class VerifiedUid
        {
            public uint Uid;
        }

        // Connections that already passed verification (the character-creation step re-sends the login on the same connection).
        static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Client.GameClient, VerifiedUid> verified =
            new System.Runtime.CompilerServices.ConditionalWeakTable<Client.GameClient, VerifiedUid>();
        static readonly object verifiedLock = new object();

        /// <summary>The UID this connection was already verified for (its token is used up by then), else false.</summary>
        public static bool VerifiedUidOf(Client.GameClient client, out uint uid)
        {
            uid = 0;
            VerifiedUid known;
            lock (verifiedLock)
            {
                if (!verified.TryGetValue(client, out known))
                    return false;
            }
            uid = known.Uid;
            return true;
        }
        static int enabled = -1;
        static int verifyIp = 1;
        static readonly object configLock = new object();

        static void LoadConfig()
        {
            lock (configLock)
            {
                if (enabled >= 0)
                    return;
                int value = 1, ipValue = 1;
                try
                {
                    // read straight from shell.ini next to the exe (not relative to DbLocation)
                    value = Database.ServerDatabase.ReadShellInt("Security", "VerifyLoginTokens", 1) != 0 ? 1 : 0;
                    ipValue = Database.ServerDatabase.ReadShellInt("Security", "VerifyTokenIp", 1) != 0 ? 1 : 0;
                }
                catch (Exception e)
                {
                    Conquer6609.Security.Guard.Audit("security-config", 0, "", "VerifyLoginTokens/VerifyTokenIp read failed, using 1: " + e.Message);
                }
                verifyIp = ipValue;
                enabled = value;
                if (value == 0)
                    Console.WriteLine("[Security] VerifyLoginTokens=0: login handoff verification is DISABLED.");
                else if (ipValue == 0)
                    Console.WriteLine("[Security] VerifyTokenIp=0: login tokens are not bound to the client IP.");
            }
        }

        public static bool Enabled
        {
            get
            {
                if (enabled < 0)
                    LoadConfig();
                return enabled != 0;
            }
        }

        public static bool VerifyIp
        {
            get
            {
                if (enabled < 0)
                    LoadConfig();
                return verifyIp != 0;
            }
        }

        /// <summary>Startup check (only when VerifyLoginTokens=1): without the table every login is refused.</summary>
        public static void CheckTableAtStartup()
        {
            if (!Enabled)
                return;
            try
            {
                using (var conn = Project2D.Database.DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = new MySqlNs.MySqlCommand("SELECT 1 FROM `login_tokens` LIMIT 0", conn))
                        cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Conquer6609.Security.Guard.Audit("security-config", 0, "", "login_tokens table check failed: " + e.Message);
                ConsoleColor old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("#####################################################################");
                Console.WriteLine("##  SECURITY: table `login_tokens` is NOT usable: " + e.Message);
                Console.WriteLine("##  VerifyLoginTokens=1, so EVERY LOGIN WILL BE REFUSED until you apply");
                Console.WriteLine("##      docs/security/login_tokens.sql");
                Console.WriteLine("##  to the account database, or set  [Security] VerifyLoginTokens=0");
                Console.WriteLine("##  in shell.ini (disables login handoff verification - NOT recommended).");
                Console.WriteLine("#####################################################################");
                Console.ForegroundColor = old;
            }
        }

        /// <summary>Same address, or both loopback / private LAN (when IP binding is on).</summary>
        public static bool IpMatches(string tokenIp, string clientIp)
        {
            if (!VerifyIp)
                return true;
            string a = NormalizeIp(tokenIp), b = NormalizeIp(clientIp);
            if (a.Length == 0 || b.Length == 0)
                return false;
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
                return true;
            return IsLocalOrPrivate(a) && IsLocalOrPrivate(b);
        }

        static string NormalizeIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return "";
            ip = ip.Trim();
            System.Net.IPAddress addr;
            if (!System.Net.IPAddress.TryParse(ip, out addr))
                return ip;
            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && addr.IsIPv4MappedToIPv6)
                addr = addr.MapToIPv4();
            return addr.ToString();
        }

        static bool IsLocalOrPrivate(string ip)
        {
            System.Net.IPAddress addr;
            if (!System.Net.IPAddress.TryParse(ip, out addr))
                return false;
            if (System.Net.IPAddress.IsLoopback(addr))
                return true;
            byte[] b = addr.GetAddressBytes();
            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && b.Length == 4)
            {
                return b[0] == 10                                   // 10.0.0.0/8
                    || (b[0] == 172 && b[1] >= 16 && b[1] <= 31)    // 172.16.0.0/12
                    || (b[0] == 192 && b[1] == 168)                 // 192.168.0.0/16
                    || (b[0] == 169 && b[1] == 254);                // 169.254.0.0/16 link-local
            }
            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && b.Length == 16)
            {
                return (b[0] & 0xFE) == 0xFC                        // fc00::/7 unique local
                    || (b[0] == 0xFE && (b[1] & 0xC0) == 0x80);     // fe80::/10 link-local
            }
            return false;
        }

        /// <summary>The EntityID of a live, unused token (login_tokens.Token). With VerifyLoginTokens=0 (development only) the
        /// old fixed-cipher identifier is still decoded.</summary>
        public static bool ResolveToken(uint token, out uint uid)
        {
            uid = 0;
            if (!Enabled)
            {
                uid = MsgLoginClient.Decrypt(token);
                return uid >= 1000000 && uid <= 19000000;
            }
            if (token == 0)
                return false;
            try
            {
                using (var conn = Project2D.Database.DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = new MySqlNs.MySqlCommand(
                        "SELECT `EntityID` FROM `login_tokens` WHERE `Token` = @t AND `Used` = 0 AND `ExpiresAt` >= NOW() LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@t", (long)token);
                        object r = cmd.ExecuteScalar();
                        if (r == null || r is DBNull) return false;
                        long id = Convert.ToInt64(r);
                        if (id < 1000000 || id > 19000000) return false;
                        uid = (uint)id;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Conquer6609.Security.Guard.Audit("login-token", 0, "", "token lookup failed: " + e.Message);
                return false;
            }
        }

        public static bool Verify(Client.GameClient client, uint uid, out string failure)
        {
            failure = "";
            if (!Enabled)
            {
                lock (verifiedLock)
                {
                    verified.Remove(client);
                    verified.Add(client, new VerifiedUid { Uid = uid });
                }
                return true;
            }

            VerifiedUid known;
            lock (verifiedLock)
            {
                if (verified.TryGetValue(client, out known) && known.Uid == uid)
                    return true;
            }

            string ip = client.Socket != null ? client.Socket.RemoteIp : "NONE";
            if (string.IsNullOrEmpty(ip) || ip == "NONE")
            {
                failure = "no remote ip";
                return false;
            }

            try
            {
                using (var conn = Project2D.Database.DataHolder.MySqlConnection)
                {
                    conn.Open();
                    // Read the token's IP first and compare it here (LAN/loopback equivalence, VerifyTokenIp);
                    // the UPDATE below still consumes it atomically and only if it is unchanged, unused and live.
                    string tokenIp;
                    long tokenServer;// -1 = no ServerID column / NULL (token not bound to a server)
                    ReadLiveToken(conn, uid, out tokenIp, out tokenServer);
                    if (tokenIp != null && tokenServer >= 0 && tokenServer != Program.ServerConfig.ServerID)
                    {
                        // issued by the AccountServer for another game server of the group (shared accounts DB)
                        failure = "token issued for server " + tokenServer + " (this is server " + Program.ServerConfig.ServerID + ")";
                        return false;
                    }
                    int rows = 0;
                    if (tokenIp != null && IpMatches(tokenIp, ip))
                    {
                        using (var cmd = new MySqlNs.MySqlCommand(
                            "UPDATE `login_tokens` SET `Used` = 1 WHERE `EntityID` = @id AND `Used` = 0 AND `IP` = @tokenIp AND `ExpiresAt` >= NOW()", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", (long)uid);
                            cmd.Parameters.AddWithValue("@tokenIp", tokenIp);
                            rows = cmd.ExecuteNonQuery();
                        }
                    }
                    if (rows == 1)
                    {
                        lock (verifiedLock)
                        {
                            verified.Remove(client);
                            verified.Add(client, new VerifiedUid { Uid = uid });
                        }
                        return true;
                    }
                    failure = DescribeFailure(conn, uid, ip);
                    return false;
                }
            }
            catch (Exception e)
            {
                failure = "token check db error: " + e.Message;
                return false;
            }
        }

        // login_tokens.ServerID is optional (added by the AccountServer at start-up, docs/security/login_tokens.sql).
        // While it is missing the old query is used; the column is looked for again every 5 minutes.
        static int serverColumn = 1;
        static long serverColumnRetry;

        static void ReadLiveToken(MySqlNs.MySqlConnection conn, uint uid, out string tokenIp, out long tokenServer)
        {
            tokenIp = null;
            tokenServer = -1;
            bool useColumn = System.Threading.Volatile.Read(ref serverColumn) == 1
                || DateTime.UtcNow.Ticks > System.Threading.Interlocked.Read(ref serverColumnRetry);
            if (useColumn)
            {
                try
                {
                    using (var sel = new MySqlNs.MySqlCommand(
                        "SELECT `IP`, `ServerID` FROM `login_tokens` WHERE `EntityID` = @id AND `Used` = 0 AND `ExpiresAt` >= NOW() LIMIT 1", conn))
                    {
                        sel.Parameters.AddWithValue("@id", (long)uid);
                        using (var r = sel.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                tokenIp = r.IsDBNull(0) ? "" : r.GetValue(0).ToString();
                                if (!r.IsDBNull(1))
                                    tokenServer = Convert.ToInt64(r.GetValue(1));
                            }
                        }
                    }
                    System.Threading.Volatile.Write(ref serverColumn, 1);
                    return;
                }
                catch (MySqlNs.MySqlException e)
                {
                    if (e.Number != 1054)// 1054 = unknown column
                        throw;
                    System.Threading.Volatile.Write(ref serverColumn, 0);
                    System.Threading.Interlocked.Exchange(ref serverColumnRetry, DateTime.UtcNow.AddMinutes(5).Ticks);
                }
            }
            using (var sel = new MySqlNs.MySqlCommand(
                "SELECT `IP` FROM `login_tokens` WHERE `EntityID` = @id AND `Used` = 0 AND `ExpiresAt` >= NOW() LIMIT 1", conn))
            {
                sel.Parameters.AddWithValue("@id", (long)uid);
                object result = sel.ExecuteScalar();
                if (result != null && !(result is DBNull))
                    tokenIp = result.ToString();
            }
        }

        static string DescribeFailure(MySqlNs.MySqlConnection conn, uint uid, string ip)
        {
            try
            {
                using (var cmd = new MySqlNs.MySqlCommand(
                    "SELECT `IP`, `Used`, (`ExpiresAt` >= NOW()) AS `Live` FROM `login_tokens` WHERE `EntityID` = @id LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@id", (long)uid);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                            return "no token";
                        string tokenIp = r.IsDBNull(0) ? "" : r.GetValue(0).ToString();
                        bool used = !r.IsDBNull(1) && Convert.ToInt32(r.GetValue(1)) != 0;
                        bool live = !r.IsDBNull(2) && Convert.ToInt32(r.GetValue(2)) != 0;
                        if (used) return "token already used";
                        if (!live) return "token expired";
                        if (!IpMatches(tokenIp, ip)) return "ip mismatch (token ip " + tokenIp + ")";
                        return "token rejected";
                    }
                }
            }
            catch (Exception e)
            {
                return "token rejected (" + e.Message + ")";
            }
        }

        /// <summary>GM rights come only from accounts.State: 255 = project manager + GM, 3 = GM, anything else (or a DB error) = none.</summary>
        /// <summary>accounts.State 1 or 100 = banned (as the account server reads it). A DB error is not a ban.</summary>
        public static bool AccountBanned(uint uid)
        {
            try
            {
                using (var conn = Project2D.Database.DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = new MySqlNs.MySqlCommand("SELECT `State` FROM `accounts` WHERE `EntityID` = @id LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", (long)uid);
                        object result = cmd.ExecuteScalar();
                        if (result == null || result is DBNull) return false;
                        long state = Convert.ToInt64(result);
                        return state == 1 || state == 100;
                    }
                }
            }
            catch (Exception) { return false; }
        }

        public static void ApplyAccountRole(Client.GameClient client, uint uid)
        {
            client.GameMaster = false;
            client.ProjectManager = false;
            try
            {
                using (var conn = Project2D.Database.DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = new MySqlNs.MySqlCommand("SELECT `State` FROM `accounts` WHERE `EntityID` = @id LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", (long)uid);
                        object result = cmd.ExecuteScalar();
                        if (result == null || result is DBNull)
                            return;
                        long state = Convert.ToInt64(result);
                        if (state == 255)
                        {
                            client.ProjectManager = true;
                            client.GameMaster = true;
                        }
                        else if (state == 3)
                        {
                            client.GameMaster = true;
                        }
                        if (client.GameMaster)
                            Conquer6609.Security.Guard.Audit("gm-login", uid, "", "account state " + state + " ip=" + (client.Socket != null ? client.Socket.RemoteIp : ""));
                    }
                }
            }
            catch (Exception e)
            {
                client.GameMaster = false;
                client.ProjectManager = false;
                Conquer6609.Security.Guard.Audit("login-role", uid, "", "account state lookup failed: " + e.Message);
            }
        }
    }
}