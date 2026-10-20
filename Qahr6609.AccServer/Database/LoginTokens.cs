using System;

namespace AccServer.Database
{
    /// <summary>One-time login handoff record checked by the game server before it loads a character.
    /// A row is valid for TokenLifetimeSeconds, only for the IP that authenticated, and only once.
    /// Table definition: docs/security/login_tokens.sql</summary>
    public static class LoginTokens
    {
        /// <summary>Long enough to survive the game server login queue; still single-use and IP bound.</summary>
        public const int TokenLifetimeSeconds = 300;

        /// <summary>login_tokens has the optional ServerID column (the token is then only accepted by that game server).</summary>
        public static bool HasServerColumn;

        /// <summary>login_tokens.Token: the random number the client gets as its login identifier (the game server finds the
        /// account by it). [Security] was the EntityID under a fixed reversible cipher: anyone who knew a character's UID
        /// could present it and race the real player for the token. Required: without the column no login is handed off.</summary>
        public static bool HasTokenColumn;
        static readonly System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();

        /// <summary>Startup check: the login_tokens table must exist, otherwise every login is refused.
        /// Adds the nullable ServerID column when it is missing (multi-server, docs/MULTI_SERVER.md).</summary>
        public static bool CheckTable()
        {
            try
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT 1 FROM `login_tokens` LIMIT 0", conn))
                        cmd.ExecuteNonQuery();
                    HasServerColumn = EnsureServerColumn(conn);
                    HasTokenColumn = EnsureTokenColumn(conn);
                    if (!HasTokenColumn) throw new Exception("login_tokens.Token column missing and could not be added");
                }
                return true;
            }
            catch (Exception e)
            {
                ConsoleColor old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("#####################################################################");
                Console.WriteLine("##  SECURITY: table `login_tokens` is NOT usable: " + e.Message);
                Console.WriteLine("##  EVERY LOGIN WILL BE REFUSED until you apply");
                Console.WriteLine("##      docs/security/login_tokens.sql");
                Console.WriteLine("##  to the account database (or, on the game server, set");
                Console.WriteLine("##      [Security] VerifyLoginTokens=0  in shell.ini  - NOT recommended).");
                Console.WriteLine("#####################################################################");
                Console.ForegroundColor = old;
                return false;
            }
        }

        static bool EnsureTokenColumn(MySql.Data.MySqlClient.MySqlConnection conn)
        {
            try
            {
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'login_tokens' AND COLUMN_NAME = 'Token'", conn))
                {
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return true;
                }
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "ALTER TABLE `login_tokens` ADD COLUMN `Token` BIGINT NULL DEFAULT NULL, ADD INDEX `ix_token` (`Token`)", conn))
                    cmd.ExecuteNonQuery();
                Console.WriteLine("[LoginTokens] column login_tokens.Token added (random login identifiers).");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[LoginTokens] login_tokens.Token not available: " + e.Message);
                return false;
            }
        }

        /// <summary>A random non-zero 32-bit identifier.</summary>
        static uint NewToken()
        {
            var b = new byte[4];
            uint t;
            do { lock (rng) rng.GetBytes(b); t = BitConverter.ToUInt32(b, 0); } while (t == 0);
            return t;
        }

        static bool EnsureServerColumn(MySql.Data.MySqlClient.MySqlConnection conn)
        {
            try
            {
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'login_tokens' AND COLUMN_NAME = 'ServerID'", conn))
                {
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return true;
                }
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "ALTER TABLE `login_tokens` ADD COLUMN `ServerID` INT NULL DEFAULT NULL", conn))
                    cmd.ExecuteNonQuery();
                Console.WriteLine("[LoginTokens] column login_tokens.ServerID added (tokens are bound to the chosen game server).");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[LoginTokens] login_tokens.ServerID not available (" + e.Message + "): tokens are not bound to a server.");
                return false;
            }
        }

        /// <summary>Insert or replace the token for this character. Returns false on database error.</summary>
        public static bool Issue(uint entityId, string ip)
        {
            uint token;
            return Issue(entityId, ip, 0, out token);
        }

        /// <summary>Same, bound to one game server (serverId 0 = not bound: ServerID stays NULL).</summary>
        public static bool Issue(uint entityId, string ip, uint serverId, out uint token)
        {
            token = NewToken();
            try
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    bool bind = HasServerColumn;
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(bind
                        ? "REPLACE INTO `login_tokens` (`EntityID`, `IP`, `ExpiresAt`, `Used`, `ServerID`, `Token`) " +
                          "VALUES (@id, @ip, DATE_ADD(NOW(), INTERVAL " + TokenLifetimeSeconds + " SECOND), 0, @sid, @tok)"
                        : "REPLACE INTO `login_tokens` (`EntityID`, `IP`, `ExpiresAt`, `Used`, `Token`) " +
                          "VALUES (@id, @ip, DATE_ADD(NOW(), INTERVAL " + TokenLifetimeSeconds + " SECOND), 0, @tok)", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", (long)entityId);
                        cmd.Parameters.AddWithValue("@tok", (long)token);
                        cmd.Parameters.AddWithValue("@ip", ip ?? "");
                        if (bind)
                            cmd.Parameters.AddWithValue("@sid", serverId != 0 ? (object)(long)serverId : DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[LoginTokens] issue failed for EntityID " + entityId + ": " + e.Message);
                return false;
            }
        }
    }
}
