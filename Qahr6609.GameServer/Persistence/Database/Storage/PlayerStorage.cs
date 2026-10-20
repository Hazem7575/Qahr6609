// ============================================================================
//  PlayerStorage - the character data of Users\<uid>.ini in MySQL (table `players`, one clear column per key).
//  Same design as Qahr's PlayerStorage.
//
//  players_columns.csv (next to the exe, or found by searching up to the source folder) is the only source of
//  the columns. Each line: OldKey|CleanColumn|Type (OldKey = the ini key the server writes). From it come the
//  CREATE TABLE, the key -> column map and the type of each column. Columns added to the CSV later are added to
//  the table on the next boot (ALTER TABLE ADD COLUMN).
//
//  The save code is not changed: SaveClient writes the ini as before, then the file is read back, every key is
//  mapped to its column and the row is UPSERTed (on a MirrorQueue worker).
//
//  shell.ini [Storage]:
//    PlayerBackend    File | Dual (default)  - Dual = files + MySQL copy; File = MySQL not touched
//    PlayerReadSource File (default) | Verify | Db - where LoadCharacter reads the character keys from:
//        Verify = the file, and every value is compared with the DB (differences -> storage_verify.txt)
//        Db     = the players row (keys without a column come from the file). When the player has a save still
//                 queued / failed, has no row, or MySQL is down, the file is read: never older data.
//    Backfill         1 (default) = after the boot, copy the players / blobs missing in MySQL from the files
//
//  Hooks: ServerDatabase.SaveClient (OnSaved), InvalidatePlayerIni (OnIniChanged), LoadCharacter
//  (BeforeLoad/AfterLoad), character deletion (OnDeleted), Server.Initialize (Init).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Project2D.Database;

namespace Conquer6609.Database.Storage
{
    public static class PlayerStorage
    {
        public enum Mode { File, Dual }
        public static Mode Backend = Mode.File;

        public enum ReadMode { File, Verify, Db }
        public static ReadMode ReadSource = ReadMode.File;
        private static bool _ready;
        public static bool Ready { get { return _ready && Backend != Mode.File; } }

        private const int KIND_TEXT = 0, KIND_INT = 1, KIND_REAL = 2;

        private static Dictionary<string, string> _rename;   // OldKey -> CleanColumn
        private static Dictionary<string, int> _cols;         // CleanColumn -> kind
        private static Dictionary<string, string> _types;     // CleanColumn -> MySQL type
        private static List<string> _colOrder;
        private static string _createSql;

        // columns with a special default (not 0/''): position and starting names
        private static readonly Dictionary<string, string> _defaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "BodyType", "1002" }, { "MapID", "1002" }, { "PrevMapID", "1002" },
            { "PosX", "410" }, { "PrevMapX", "410" }, { "PosY", "354" }, { "PrevMapY", "354" },
            { "Name", "'None'" }, { "SpouseName", "'None'" }, { "Description", "'None'" },
        };

        public static void Init()
        {
            string s = (Framework.Settings.String("Storage", "PlayerBackend", "Dual") ?? "Dual").Trim().ToLowerInvariant();
            Backend = s == "file" ? Mode.File : Mode.Dual;
            if (Backend == Mode.File)
            {
                MyConsole.WriteLine("[Storage] Player backend: File (MySQL copy off, shell.ini [Storage] PlayerBackend=Dual turns it on)");
                return;
            }
            Migrations.Migrator.EnsureDatabase();
            EnsureSchema();
            MirrorQueue.Init();
            Migrations.Migrator.Run();
            PlayerBlobStorage.Init(_ready);
            string rs = (Framework.Settings.String("Storage", "PlayerReadSource", "File") ?? "File").Trim().ToLowerInvariant();
            ReadSource = !_ready ? ReadMode.File : rs == "db" ? ReadMode.Db : rs == "verify" ? ReadMode.Verify : ReadMode.File;
            MyConsole.WriteLine("[Storage] Player backend: Dual (files + MySQL) - " + (_ready ? "players table ready, " + _cols.Count + " columns" : "MySQL NOT ready, files only")
                + ", read source " + ReadSource + (MirrorQueue.Async ? ", async mirror" : ""));
            if (_ready)
            {
                _usersDir = Path.GetFullPath(Program.ServerConfig.DbLocation + "\\Users\\");
                WindowsAPI.IniFile.Written = OnIniWritten;
                StartOnlineSync();
                if (Framework.Settings.Bool("Storage", "Backfill", true))
                    StartBackfill();
            }
        }

        // players_columns.csv next to the exe, or searching up (source tree: Persistence\Database\Storage\)
        internal static string FindNearExe(string fileName, string sourceSubFolder)
        {
            // from the exe folder, then from the working folder (the server runs in Qahr6609.GameServer\bin\Debug)
            foreach (string start in new[] { AppDomain.CurrentDomain.BaseDirectory, Directory.GetCurrentDirectory() })
            {
                try
                {
                    string dir = start;
                    for (int i = 0; i < 12 && !string.IsNullOrEmpty(dir); i++)
                    {
                        string p1 = Path.Combine(dir, fileName);
                        if (File.Exists(p1) || Directory.Exists(p1)) return p1;
                        string p2 = Path.Combine(dir, sourceSubFolder, fileName);
                        if (File.Exists(p2) || Directory.Exists(p2)) return p2;
                        DirectoryInfo parent = Directory.GetParent(dir);
                        dir = parent != null ? parent.FullName : null;
                    }
                }
                catch { }
            }
            return null;
        }

        private static bool LoadMap()
        {
            string file = FindNearExe("players_columns.csv", "Persistence\\Database\\Storage");
            if (file == null) { MirrorQueue.Warn("players_columns.csv not found - MySQL copy off."); return false; }

            var rename = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var cols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var types = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var order = new List<string>();
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE IF NOT EXISTS `players` (\n");
            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                string[] p = line.Split('|');
                if (p.Length != 3) continue;
                string oldKey = p[0].Trim(), clean = p[1].Trim(), type = p[2].Trim();
                if (oldKey.Length == 0 || clean.Length == 0 || type.Length == 0) continue;
                rename[oldKey] = clean;
                if (cols.ContainsKey(clean)) continue;
                string up = type.ToUpperInvariant();
                int kind = (up.Contains("DOUBLE") || up.Contains("FLOAT") || up.Contains("DECIMAL")) ? KIND_REAL
                         : (up.Contains("INT") || up.Contains("BIT")) ? KIND_INT
                         : KIND_TEXT;
                cols[clean] = kind;
                string def = ColumnDefinition(clean, type);
                types[clean] = def;
                order.Add(clean);
                sb.Append("  `").Append(clean).Append("` ").Append(def).Append(",\n");
            }
            if (!cols.ContainsKey("Online"))
            {
                // the server keeps players.Online = 1/0 itself (OnlineSync); not an ini key
                cols["Online"] = KIND_INT;
                types["Online"] = "TINYINT UNSIGNED NOT NULL DEFAULT 0";
                order.Add("Online");
                sb.Append("  `Online` TINYINT UNSIGNED NOT NULL DEFAULT 0,\n");
            }
            // write time (UTC ticks) of the ini this row was copied from: Db reads use the row only while it matches the file
            cols["FileTime"] = KIND_INT;
            types["FileTime"] = "BIGINT NOT NULL DEFAULT 0";
            order.Add("FileTime");
            sb.Append("  `FileTime` BIGINT NOT NULL DEFAULT 0,\n");
            sb.Append("  PRIMARY KEY (`UID`),\n");
            sb.Append("  KEY `idx_name` (`Name`(64)),\n");
            sb.Append("  KEY `idx_online` (`Online`)\n");
            sb.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");
            _rename = rename;
            _cols = cols;
            _types = types;
            _colOrder = order;
            _createSql = sb.ToString();
            return _rename.Count > 0 && _cols.Count > 0;
        }

        private static string ColumnDefinition(string clean, string type)
        {
            string up = type.ToUpperInvariant();
            if (clean.Equals("UID", StringComparison.OrdinalIgnoreCase)) return type + " NOT NULL";
            if (up.Contains("TEXT")) return type + " NULL";
            string def;
            if (!_defaults.TryGetValue(clean, out def))
                def = (up.Contains("CHAR") ? "''" : "0");
            return type + " NOT NULL DEFAULT " + def;
        }

        private static void EnsureSchema()
        {
            try
            {
                if (!LoadMap()) { _ready = false; return; }
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = _createSql;
                        cmd.ExecuteNonQuery();
                    }
                    // columns added to the CSV after the table was created
                    var have = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'players';";
                        using (var r = cmd.ExecuteReader())
                            while (r.Read()) have.Add(r.GetString(0));
                    }
                    foreach (string c in _colOrder)
                    {
                        if (have.Contains(c)) continue;
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "ALTER TABLE `players` ADD COLUMN `" + c + "` " + _types[c] + ";";
                            cmd.ExecuteNonQuery();
                        }
                        MyConsole.WriteLine("[Storage] players: column `" + c + "` added.");
                    }
                }
                _ready = true;
            }
            catch (Exception e)
            {
                _ready = false;
                MirrorQueue.Warn("players table not ready (files only): " + e.Message);
            }
        }

        private static string PlayerFile(uint uid)
        {
            return Program.ServerConfig.DbLocation + "\\Users\\" + uid + ".ini";
        }

        // an ini value -> a value for the column's type
        private static string Coerce(string val, int kind)
        {
            if (val == null) val = "";
            val = val.Trim();
            if (kind == KIND_TEXT) return val;
            if (val.Equals("True", StringComparison.OrdinalIgnoreCase)) return "1";
            if (val.Equals("False", StringComparison.OrdinalIgnoreCase)) return "0";
            if (val.Length == 0) return "0";
            if (kind == KIND_REAL)
            {
                if (val.IndexOf(',') >= 0 && val.IndexOf('.') < 0) val = val.Replace(',', '.');
                double d;
                return double.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? val : "0";
            }
            // KIND_INT: kept as text (full BIGINT UNSIGNED precision); empty / invalid -> 0
            double chk;
            return double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out chk) ? val : "0";
        }

        // Reads the ini like the profile API does (no BOM = ANSI code page, FF FE = UTF-16) and maps every key to its column.
        private static Dictionary<string, string> ParseIniToColumns(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            string text = bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE
                ? Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2)
                : AnsiText.Value.GetString(bytes);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string section = "";
            foreach (string raw in text.Split('\n'))
            {
                string t = raw.Trim();
                if (t.Length == 0 || t[0] == ';' || t[0] == '#') continue;
                if (t[0] == '[' && t.EndsWith("]")) { section = t.Substring(1, t.Length - 2).Trim(); continue; }
                int eq = t.IndexOf('=');
                if (eq <= 0) continue;
                string key = t.Substring(0, eq).Trim();
                string val = t.Substring(eq + 1);
                if (key.Length == 0) continue;
                string clean = ColumnFor(section, key);
                if (clean == null || clean.Equals("Online", StringComparison.OrdinalIgnoreCase) || clean.Equals("FileTime", StringComparison.OrdinalIgnoreCase)) continue;
                if (result.ContainsKey(clean)) continue;// the profile API returns the first key of that name
                result[clean] = Coerce(val, _cols[clean]);
            }
            return result;
        }

        /// <summary>End of SaveClient: the ini is written; mirror it into its players row.</summary>
        public static void OnSaved(uint uid, string name)
        {
            if (!Ready || uid == 0) return;
            MirrorQueue.Submit("players:" + uid, "save", false,
                () => PrepareUpsertFromIni(uid, name, false),
                "players UPSERT for " + uid + " failed (file is saved)");
        }

        private static string _usersDir;

        // IniFile.Written: a key of some ini was written through IniFile (SaveClient writes without IniFile)
        private static void OnIniWritten(string fileName)
        {
            if (!fileName.EndsWith(".ini", StringComparison.OrdinalIgnoreCase)) return;
            string full = Path.GetFullPath(fileName);
            if (!full.StartsWith(_usersDir, StringComparison.OrdinalIgnoreCase) || full.IndexOf('\\', _usersDir.Length) >= 0) return;
            uint uid;
            if (uint.TryParse(Path.GetFileNameWithoutExtension(full), out uid))
                OnIniChanged(uid);
        }

        /// <summary>After a direct write to Users\uid.ini outside SaveClient (offline guild/union/clan/spouse updates,
        /// chat ban...). A file without Name (a write that created the file of a deleted uid) is ignored.</summary>
        public static void OnIniChanged(uint uid)
        {
            if (!Ready || uid == 0) return;
            MirrorQueue.Submit("players:" + uid, "save", false,
                () => PrepareUpsertFromIni(uid, null, true),
                "players UPSERT for " + uid + " failed (file is saved)");
        }

        // reads the file now and returns the DB work (null = nothing to do)
        private static Action PrepareUpsertFromIni(uint uid, string fallbackName, bool requireName)
        {
            string path = PlayerFile(uid);
            if (!File.Exists(path)) return null;
            // time first: a write racing the read can only make the row look older than the file (-> file read)
            long fileTime = File.GetLastWriteTimeUtc(path).Ticks;
            var vals = ParseIniToColumns(path);
            if (requireName && !vals.ContainsKey("Name")) return null;
            vals["UID"] = uid.ToString(CultureInfo.InvariantCulture);
            if (!vals.ContainsKey("Name")) vals["Name"] = fallbackName ?? "None";
            vals["FileTime"] = fileTime.ToString(CultureInfo.InvariantCulture);

            var colList = new List<string>(vals.Keys);
            var insCols = new StringBuilder();
            var insVals = new StringBuilder();
            var updSet = new StringBuilder();
            for (int i = 0; i < colList.Count; i++)
            {
                if (i > 0) { insCols.Append(','); insVals.Append(','); }
                insCols.Append('`').Append(colList[i]).Append('`');
                insVals.Append("@p").Append(i);
                if (!colList[i].Equals("UID", StringComparison.OrdinalIgnoreCase))
                {
                    if (updSet.Length > 0) updSet.Append(',');
                    updSet.Append('`').Append(colList[i]).Append("`=VALUES(`").Append(colList[i]).Append("`)");
                }
            }
            string sql = "INSERT INTO `players` (" + insCols + ") VALUES (" + insVals + ") ON DUPLICATE KEY UPDATE " + updSet + ";";
            return () =>
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        for (int i = 0; i < colList.Count; i++)
                            cmd.Parameters.AddWithValue("@p" + i, vals[colList[i]]);
                        cmd.ExecuteNonQuery();
                    }
                }
            };
        }

        /// <summary>Character deleted (its files are removed): remove its row too.</summary>
        public static void OnDeleted(uint uid)
        {
            if (!Ready || uid == 0) return;
            MirrorQueue.Submit("players:" + uid, "delete", true, () => () =>
            {
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM `players` WHERE `UID`=@uid;";
                        cmd.Parameters.AddWithValue("@uid", uid);
                        cmd.ExecuteNonQuery();
                    }
                }
            }, "players DELETE for " + uid + " failed");
            PlayerBlobStorage.OnDeleted(uid);
        }

        // ------------------------------------------------------------------ Online flag (as Qahr)
        // players.Online = 1/0 reconciled with the connected clients every 15 s, so it can be queried from MySQL.
        private static bool _onlineSyncStarted;

        private static void StartOnlineSync()
        {
            if (_onlineSyncStarted) return;
            _onlineSyncStarted = true;
            new System.Threading.Thread(OnlineSyncLoop) { IsBackground = true, Name = "OnlineSync" }.Start();
        }

        private static void OnlineSyncLoop()
        {
            while (true)
            {
                try
                {
                    var ids = new List<uint>();
                    var poll = Server.GamePoll;
                    if (poll != null)
                        foreach (var c in poll.Values)
                            if (c != null && !c.Fake && c.Player != null && c.FullLoading) ids.Add(c.Player.UID);
                    SyncOnline(ids);
                }
                catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
                System.Threading.Thread.Sleep(15000);
            }
        }

        private static void SyncOnline(ICollection<uint> onlineUids)
        {
            if (!Ready) return;
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    if (onlineUids.Count == 0)
                    {
                        cmd.CommandText = "UPDATE `players` SET `Online`=0 WHERE `Online`<>0;";
                        cmd.ExecuteNonQuery();
                        return;
                    }
                    var sb = new StringBuilder();
                    foreach (uint u in onlineUids) { if (sb.Length > 0) sb.Append(','); sb.Append(u.ToString(CultureInfo.InvariantCulture)); }
                    string ids = sb.ToString();
                    cmd.CommandText = "UPDATE `players` SET `Online`=1 WHERE `Online`<>1 AND `UID` IN (" + ids + ");";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE `players` SET `Online`=0 WHERE `Online`<>0 AND `UID` NOT IN (" + ids + ");";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ------------------------------------------------------------------ backfill
        // Players that have not saved since the MySQL copy was turned on have no row / blobs yet: copy them from the
        // files once, in the background, so the DB holds every character. Only missing rows are written.
        private static void StartBackfill()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    System.Threading.Thread.Sleep(5000);// after the boot
                    var have = new HashSet<uint>();
                    using (var conn = DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT `UID` FROM `players`;";
                            using (var r = cmd.ExecuteReader())
                                while (r.Read()) have.Add(Convert.ToUInt32(r.GetValue(0), CultureInfo.InvariantCulture));
                        }
                    }
                    int rows = 0;
                    foreach (string f in Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\", "*.ini"))
                    {
                        uint uid;
                        if (!uint.TryParse(Path.GetFileNameWithoutExtension(f), out uid) || uid == 0 || have.Contains(uid)) continue;
                        OnIniChanged(uid);
                        rows++;
                    }
                    int blobs = PlayerBlobStorage.BackfillMissing();
                    if (rows > 0 || blobs > 0)
                        MyConsole.WriteLine("[Storage] backfill: " + rows + " player row(s), " + blobs + " blob(s) queued from the files.");
                }
                catch (Exception e) { MirrorQueue.Warn("backfill failed: " + e.Message); }
            }) { IsBackground = true, Name = "StorageBackfill", Priority = System.Threading.ThreadPriority.BelowNormal }.Start();
        }

        // ------------------------------------------------------------------ reading from MySQL (overlay)
        public static long DbLoads, FileFallbacks, VerifyLogins, VerifyMismatches;

        /// <summary>Start of LoadCharacter: in Verify/Db mode the players row becomes a read overlay of the ini.</summary>
        public static void BeforeLoad(uint uid)
        {
            if (ReadSource == ReadMode.File || !Ready || uid == 0) return;
            string why;
            try
            {
                if (MirrorQueue.IsDirty("players:" + uid)) why = "DB copy not up to date yet (pending/failed save)";
                else
                {
                    var row = LoadRow(uid);
                    string ft;
                    if (row == null) why = "no players row";
                    else if (!row.TryGetValue("FileTime", out ft) || ft != File.GetLastWriteTimeUtc(PlayerFile(uid)).Ticks.ToString(CultureInfo.InvariantCulture))
                        why = "DB row older than the file (not copied yet, e.g. after a crash)";
                    else
                    {
                        WindowsAPI.IniFile.SetOverlay(PlayerFile(uid), new RowOverlay(uid, row), ReadSource == ReadMode.Verify);
                        if (ReadSource == ReadMode.Db) System.Threading.Interlocked.Increment(ref DbLoads);
                        else System.Threading.Interlocked.Increment(ref VerifyLogins);
                        return;
                    }
                }
            }
            catch (Exception e) { why = "DB error: " + e.Message; }
            System.Threading.Interlocked.Increment(ref FileFallbacks);
            VerifyLog("uid " + uid + ": reading from FILE (" + why + ")");
        }

        /// <summary>End of LoadCharacter (finally): removes the overlay.</summary>
        public static void AfterLoad(uint uid)
        {
            if (ReadSource == ReadMode.File || uid == 0) return;
            WindowsAPI.IniFile.ClearOverlay(PlayerFile(uid));
        }

        private static Dictionary<string, string> LoadRow(uint uid)
        {
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM `players` WHERE `UID`=@uid;";
                    cmd.Parameters.AddWithValue("@uid", uid);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return null;
                        var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 0; i < r.FieldCount; i++)
                        {
                            if (r.IsDBNull(i)) continue;// NULL = no value -> that key is read from the file
                            object v = r.GetValue(i);
                            row[r.GetName(i)] = v is IFormattable ? ((IFormattable)v).ToString(null, CultureInfo.InvariantCulture) : Convert.ToString(v);
                        }
                        return row;
                    }
                }
            }
        }

        // (section, key) -> column; same rule as the mapping of the saved file
        private static string ColumnFor(string section, string key)
        {
            if (section == null || key == null || _rename == null) return null;
            key = key.Trim();
            bool isChar = section.Equals("Character", StringComparison.OrdinalIgnoreCase) || section.Length == 0;
            string oldKey = null;
            if (!isChar && _rename.ContainsKey(section + "_" + key)) oldKey = section + "_" + key;
            else if (isChar && _rename.ContainsKey(key)) oldKey = key;
            if (oldKey == null) return null;
            string clean = _rename[oldKey];
            return _cols.ContainsKey(clean) ? clean : null;
        }

        private sealed class RowOverlay : WindowsAPI.IniFile.IReadOverlay
        {
            private readonly uint uid;
            private readonly Dictionary<string, string> row;
            private readonly HashSet<string> written = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public RowOverlay(uint uid, Dictionary<string, string> row) { this.uid = uid; this.row = row; }
            // LoadCharacter writes some keys while loading: from then on that key is read from the file
            public void OnWrite(string section, string key)
            {
                lock (written) written.Add((section ?? "") + "|" + (key ?? ""));
            }
            public bool TryGet(string section, string key, out string value)
            {
                value = null;
                lock (written) if (written.Count != 0 && written.Contains((section ?? "") + "|" + (key ?? ""))) return false;
                string col = ColumnFor(section, key);
                if (col == null || col.Equals("UID", StringComparison.OrdinalIgnoreCase) || col.Equals("Online", StringComparison.OrdinalIgnoreCase) || col.Equals("FileTime", StringComparison.OrdinalIgnoreCase)) return false;
                return row.TryGetValue(col, out value);
            }
            public void OnMismatch(string section, string key, string fileValue, string overlayValue)
            {
                System.Threading.Interlocked.Increment(ref VerifyMismatches);
                VerifyLog("uid " + uid + " MISMATCH [" + section + "] " + key + " (col " + ColumnFor(section, key) + "): file='" + fileValue + "' db='" + overlayValue + "'");
            }
        }

        private static readonly object verifyLock = new object();
        internal static void VerifyLog(string line)
        {
            try
            {
                lock (verifyLock)
                    File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "storage_verify.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line + Environment.NewLine);
            }
            catch { }
        }
    }
}
