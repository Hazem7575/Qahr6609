using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Generators;

namespace AccServer.Database
{
    /// <summary>Account passwords stored the way Laravel stores them: bcrypt "$2y$10$" + 53 chars
    /// (Hash::make / password_hash(PASSWORD_BCRYPT)), so the website can check them with Hash::check
    /// and the account server can check what the website writes.
    /// Rows still holding a plain-text password (made before the change, or by an old tool) are
    /// accepted once and rehashed: on login (RehashOnLogin) and at start-up (MigrateAll).</summary>
    public static class PasswordHasher
    {
        /// <summary>Laravel default (config/hashing.php bcrypt.rounds).</summary>
        public const int Cost = 10;

        /// <summary>True once accounts.Password is wide enough for a hash (set by MigrateAll).
        /// A plain-text row is only rewritten after that, so a varchar(16) column never truncates a hash.</summary>
        public static volatile bool ColumnReady;

        static readonly System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();

        public static bool IsHashed(string stored)
        {
            return stored != null && stored.Length == 60 && stored.StartsWith("$2", StringComparison.Ordinal) && stored[3] == '$';
        }

        public static string Hash(string password)
        {
            byte[] salt = new byte[16];
            lock (rng)
                rng.GetBytes(salt);
            return OpenBsdBCrypt.Generate("2y", password.ToCharArray(), salt, Cost);
        }

        /// <summary>needsRehash = the row holds a plain-text password that matched.</summary>
        public static bool Verify(string password, string stored, out bool needsRehash)
        {
            needsRehash = false;
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored))
                return false;
            if (IsHashed(stored))
            {
                try
                {
                    return OpenBsdBCrypt.CheckPassword(stored, password.ToCharArray());
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Passwords] unreadable hash: " + e.Message);
                    return false;
                }
            }
            if (FixedTimeEquals(stored, password))
            {
                needsRehash = true;
                return true;
            }
            return false;
        }

        static bool FixedTimeEquals(string a, string b)
        {
            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }

        /// <summary>Replaces a matched plain-text password with its hash (only if the row still holds that plain text).</summary>
        public static void RehashOnLogin(string username, string plain)
        {
            if (!ColumnReady)
                return;
            try
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                        "UPDATE `accounts` SET `Password` = @hash WHERE `Username` = @user AND `Password` = @plain", conn))
                    {
                        cmd.Parameters.AddWithValue("@hash", Hash(plain));
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@plain", plain);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Passwords] rehash failed for " + username + ": " + e.Message);
            }
        }

        /// <summary>Start-up: widens accounts.Password to varchar(255) (it was varchar(16), too short for a
        /// 60-char hash) and hashes every plain-text password. Returns false when the column could not be
        /// widened; nothing is rehashed then.</summary>
        public static bool MigrateAll()
        {
            try
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    long width = 0;
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT CHARACTER_MAXIMUM_LENGTH FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'accounts' AND COLUMN_NAME = 'Password'", conn))
                    {
                        object v = cmd.ExecuteScalar();
                        if (v != null && v != DBNull.Value)
                            width = Convert.ToInt64(v);
                    }
                    if (width < 60)
                    {
                        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("ALTER TABLE `accounts` MODIFY `Password` varchar(255) DEFAULT ''", conn))
                        {
                            cmd.CommandTimeout = 120;
                            cmd.ExecuteNonQuery();
                        }
                        Console.WriteLine("[Passwords] accounts.Password widened from varchar(" + width + ") to varchar(255).");
                    }
                    ColumnReady = true;

                    var plain = new List<KeyValuePair<string, string>>();
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT `Username`, `Password` FROM `accounts`", conn))
                    {
                        cmd.CommandTimeout = 120;
                        using (var r = cmd.ExecuteReader())
                            while (r.Read())
                            {
                                string user = r.IsDBNull(0) ? null : r.GetString(0);
                                string pass = r.IsDBNull(1) ? null : r.GetString(1);
                                if (user != null && !string.IsNullOrEmpty(pass) && !IsHashed(pass))
                                    plain.Add(new KeyValuePair<string, string>(user, pass));
                            }
                    }
                    int done = 0;
                    foreach (var row in plain)
                    {
                        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                            "UPDATE `accounts` SET `Password` = @hash WHERE `Username` = @user AND `Password` = @plain", conn))
                        {
                            cmd.Parameters.AddWithValue("@hash", Hash(row.Value));
                            cmd.Parameters.AddWithValue("@user", row.Key);
                            cmd.Parameters.AddWithValue("@plain", row.Value);
                            done += cmd.ExecuteNonQuery();
                        }
                    }
                    if (plain.Count > 0)
                        Console.WriteLine("[Passwords] " + done + " plain-text password(s) replaced by bcrypt hashes.");
                }
                return true;
            }
            catch (Exception e)
            {
                ConsoleColor old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Passwords] migration failed, plain-text rows are still accepted and rehashed on login: " + e.Message);
                Console.ForegroundColor = old;
                return false;
            }
        }
    }
}
