using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MYSQLCONNECTION = MySqlConnector.MySqlConnection; // [net10] .NET 10 build

namespace Conquer6609.Database.Migrations
{
    // ============================================================================
    //  SQL migrations for the MySQL copy of the player data (same as Qahr's Migrator).
    //  On a boot with PlayerBackend=Dual and a working MySQL connection:
    //     1) CREATE DATABASE `<name>` if it is missing      (EnsureDatabase)
    //     2) the players table from players_columns.csv     (PlayerStorage.EnsureSchema)
    //     3) every *.sql file of the Migrations folder that has not run yet, in file name order, recorded in
    //        `schema_migrations` (Run). A failed file is not recorded, so it is tried again on the next boot.
    //  A later schema change = a new file 00NN_name.sql in Persistence\Database\Migrations (copied next to the exe).
    // ============================================================================
    public static class Migrator
    {
        /// <summary>Step 1: creates the database of the connection string when it does not exist.</summary>
        public static void EnsureDatabase()
        {
            try
            {
                string cs = Project2D.Database.DataHolder.ConnectionString ?? "";
                string db = ConnValue(cs, "Database");
                if (string.IsNullOrEmpty(db)) return;
                using (var conn = new MYSQLCONNECTION(ConnRemove(cs, "Database")))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "CREATE DATABASE IF NOT EXISTS `" + db.Replace("`", "") + "` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Warn("could not ensure the database (" + e.Message + ") - continuing.");
            }
        }

        /// <summary>Step 3: applies every migration file that has not run yet, in file name order.</summary>
        public static void Run()
        {
            try
            {
                string dir = Storage.PlayerStorage.FindNearExe("Migrations", "Persistence\\Database");
                using (var conn = Project2D.Database.DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE IF NOT EXISTS `schema_migrations` (`name` VARCHAR(190) NOT NULL, `applied_at` DATETIME NOT NULL, PRIMARY KEY (`name`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";
                        cmd.ExecuteNonQuery();
                    }
                    var applied = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT `name` FROM `schema_migrations`;";
                        using (var r = cmd.ExecuteReader())
                            while (r.Read()) applied.Add(r.GetString(0));
                    }
                    string[] files = dir == null ? new string[0] : Directory.GetFiles(dir, "*.sql");
                    Array.Sort(files, StringComparer.OrdinalIgnoreCase);
                    int ran = 0;
                    foreach (string f in files)
                    {
                        string name = Path.GetFileNameWithoutExtension(f);
                        if (applied.Contains(name)) continue;
                        ApplyOne(conn, name, File.ReadAllText(f));
                        ran++;
                        MyConsole.WriteLine("[Migrate] applied " + name);
                    }
                    if (dir == null) Warn("no Migrations folder next to the exe or in the source tree.");
                    else if (ran > 0) MyConsole.WriteLine("[Migrate] " + applied.Count + " already applied, " + ran + " new.");
                }
            }
            catch (Exception e)
            {
                Warn("FAILED: " + e.Message);
            }
        }

        private static void ApplyOne(MYSQLCONNECTION conn, string name, string sql)
        {
            foreach (string stmt in SplitStatements(sql))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = stmt;
                    cmd.ExecuteNonQuery();
                }
            }
            // recorded only after every statement succeeded
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO `schema_migrations` (`name`,`applied_at`) VALUES (@n, NOW());";
                cmd.Parameters.AddWithValue("@n", name);
                cmd.ExecuteNonQuery();
            }
        }

        // statements split on ';', -- and # line comments dropped (DDL/DML only: no ';' inside strings)
        private static IEnumerable<string> SplitStatements(string sql)
        {
            var clean = new StringBuilder();
            foreach (string raw in (sql ?? "").Replace("\r", "").Split('\n'))
            {
                string t = raw.TrimStart();
                if (t.StartsWith("--") || t.StartsWith("#")) continue;
                clean.Append(raw).Append('\n');
            }
            foreach (string part in clean.ToString().Split(';'))
            {
                string s = part.Trim();
                if (s.Length > 0) yield return s;
            }
        }

        private static string ConnValue(string cs, string key)
        {
            foreach (string part in cs.Split(';'))
            {
                int eq = part.IndexOf('=');
                if (eq > 0 && part.Substring(0, eq).Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                    return part.Substring(eq + 1).Trim();
            }
            return null;
        }

        private static string ConnRemove(string cs, string key)
        {
            var sb = new StringBuilder();
            foreach (string part in cs.Split(';'))
            {
                if (part.Trim().Length == 0) continue;
                int eq = part.IndexOf('=');
                if (eq > 0 && part.Substring(0, eq).Trim().Equals(key, StringComparison.OrdinalIgnoreCase)) continue;
                sb.Append(part.Trim()).Append(';');
            }
            return sb.ToString();
        }

        private static void Warn(string m) { try { MyConsole.WriteLine("[Migrate] " + m, ConsoleColor.Yellow); } catch { } }
    }
}
