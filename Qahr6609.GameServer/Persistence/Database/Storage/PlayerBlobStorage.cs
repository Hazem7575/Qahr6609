// ============================================================================
//  PlayerBlobStorage - the player .bin files (items, mail, profs, spells, quests, house) in MySQL table
//  `player_blobs`, byte for byte (Qahr's final design: the exact bytes round-trip every struct field and the
//  deposit chains, no per-field tables to keep in step with the structs).
//
//  Saving: BinaryFile commits every atomic write in one place (Commit / WriteAllBytesAtomic) and calls
//  BinaryFile.Committed; for DbLocation\<store>\<uid>.bin of a player store the bytes are queued for an UPSERT.
//  So no save function needed a hook.
//  Loading: BinaryFile.Open(..., FileMode.Open) asks BinaryFile.BlobResolver first. With PlayerReadSource=Db the
//  blob is read when it is up to date (no queued/failed save); Verify reads the file and logs a different blob.
//  Store list: shell.ini [Storage] BlobStores (default PlayersItems,PlayersMail,PlayersProfs,PlayersSpells,Quests,Houses).
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using Project2D.Database;

namespace Conquer6609.Database.Storage
{
    public static class PlayerBlobStorage
    {
        private static bool _ready;
        private static HashSet<string> _stores;
        private static string _root;// full path of DbLocation, with a trailing '\'
        public static long BlobLoads, BlobMismatches;

        public static void Init(bool dbReady)
        {
            _ready = false;
            if (!dbReady) return;
            _stores = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string s in (Framework.Settings.String("Storage", "BlobStores", "PlayersItems,PlayersMail,PlayersProfs,PlayersSpells,Quests,Houses") ?? "").Split(','))
                if (s.Trim().Length > 0) _stores.Add(s.Trim());
            try
            {
                _root = Path.GetFullPath(Program.ServerConfig.DbLocation);
                if (!_root.EndsWith("\\")) _root += "\\";
                // player_blobs itself is created by migration 0002; check it is there
                using (var conn = DataHolder.MySqlConnection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'player_blobs';";
                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0) { MirrorQueue.Warn("table player_blobs missing (migration 0002?) - .bin files not copied."); return; }
                    }
                }
                _ready = true;
                WindowsAPI.BinaryFile.Committed = OnCommitted;
                if (PlayerStorage.ReadSource != PlayerStorage.ReadMode.File)
                    WindowsAPI.BinaryFile.BlobResolver = Resolve;
            }
            catch (Exception e) { MirrorQueue.Warn("player_blobs not ready (.bin files only): " + e.Message); }
        }

        // DbLocation\<store>\<uid>.bin of a player store -> (uid, store)
        private static bool ParsePath(string path, out uint uid, out string store)
        {
            uid = 0; store = null;
            if (!_ready || path == null || !path.EndsWith(".bin", StringComparison.OrdinalIgnoreCase)) return false;
            try
            {
                string full = Path.GetFullPath(path);
                if (!full.StartsWith(_root, StringComparison.OrdinalIgnoreCase)) return false;
                string rel = full.Substring(_root.Length);
                int slash = rel.IndexOf('\\');
                if (slash <= 0 || rel.IndexOf('\\', slash + 1) >= 0) return false;// exactly <store>\<file>
                store = rel.Substring(0, slash);
                return _stores.Contains(store) && uint.TryParse(Path.GetFileNameWithoutExtension(rel.Substring(slash + 1)), out uid) && uid != 0;
            }
            catch { return false; }
        }

        private static string Key(uint uid, string store) { return "blob:" + store + ":" + uid; }

        /// <summary>BinaryFile.Committed: a file was written and replaced atomically (data = its exact bytes).</summary>
        private static void OnCommitted(string path, byte[] data, int length)
        {
            uint uid; string store;
            if (!ParsePath(path, out uid, out store)) return;
            byte[] copy = new byte[length];
            Buffer.BlockCopy(data, 0, copy, 0, length);
            long fileTime = File.GetLastWriteTimeUtc(path).Ticks;// the version these bytes are (just committed)
            MirrorQueue.Submit(Key(uid, store), "save", false, () => () => Save(uid, store, copy, fileTime),
                "player_blobs " + store + "/" + uid + " failed (file is saved)");
        }

        private static void Save(uint uid, string store, byte[] data, long fileTime)
        {
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO `player_blobs` (`uid`,`store`,`data`,`file_time`,`updated_at`) VALUES (@u,@s,@d,@t,NOW()) " +
                                      "ON DUPLICATE KEY UPDATE `data`=VALUES(`data`),`file_time`=VALUES(`file_time`),`updated_at`=VALUES(`updated_at`);";
                    cmd.Parameters.AddWithValue("@u", uid);
                    cmd.Parameters.AddWithValue("@s", store);
                    cmd.Parameters.AddWithValue("@d", data);
                    cmd.Parameters.AddWithValue("@t", fileTime);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static byte[] Load(uint uid, string store, out long fileTime)
        {
            fileTime = 0;
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT `data`, `file_time` FROM `player_blobs` WHERE `uid`=@u AND `store`=@s;";
                    cmd.Parameters.AddWithValue("@u", uid);
                    cmd.Parameters.AddWithValue("@s", store);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read() || r.IsDBNull(0)) return null;
                        fileTime = Convert.ToInt64(r.GetValue(1));
                        return (byte[])r.GetValue(0);
                    }
                }
            }
        }

        /// <summary>BinaryFile.BlobResolver: the bytes to read for this file instead of the file, or null = read the file.</summary>
        private static byte[] Resolve(string path)
        {
            uint uid; string store;
            if (!ParsePath(path, out uid, out store)) return null;
            if (MirrorQueue.IsDirty(Key(uid, store))) return null;// a newer save is not in the DB yet
            byte[] blob;
            long fileTime;
            try { blob = Load(uid, store, out fileTime); }
            catch { return null; }
            if (blob == null) return null;
            try
            {
                // only the version that is on disk now (after a crash the DB can be one save behind)
                if (!File.Exists(path) || File.GetLastWriteTimeUtc(path).Ticks != fileTime) return null;
            }
            catch { return null; }
            if (PlayerStorage.ReadSource == PlayerStorage.ReadMode.Db)
            {
                System.Threading.Interlocked.Increment(ref BlobLoads);
                return blob;
            }
            // Verify: read the file, report a different blob
            try
            {
                byte[] file = File.ReadAllBytes(path);
                bool same = file.Length == blob.Length;
                for (int i = 0; same && i < file.Length; i++) same = file[i] == blob[i];
                if (!same)
                {
                    System.Threading.Interlocked.Increment(ref BlobMismatches);
                    PlayerStorage.VerifyLog("uid " + uid + " BLOB MISMATCH " + store + ": file " + file.Length + " bytes, db " + blob.Length + " bytes");
                }
            }
            catch { }
            return null;
        }

        /// <summary>Character deleted: remove the blobs whose file was deleted (the deletion keeps PlayersMail, so does
        /// the DB), after any queued save of them.</summary>
        public static void OnDeleted(uint uid)
        {
            if (!_ready || uid == 0) return;
            foreach (string store in _stores)
            {
                string s = store;
                if (File.Exists(Path.Combine(_root, s, uid + ".bin")))
                    continue;
                MirrorQueue.Submit(Key(uid, s), "delete", true, () => () =>
                {
                    using (var conn = DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "DELETE FROM `player_blobs` WHERE `uid`=@u AND `store`=@s;";
                            cmd.Parameters.AddWithValue("@u", uid);
                            cmd.Parameters.AddWithValue("@s", s);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }, "player_blobs DELETE " + s + "/" + uid + " failed");
            }
        }

        /// <summary>Queues every player .bin that has no blob yet (players that did not save since the copy was turned on).</summary>
        public static int BackfillMissing()
        {
            if (!_ready) return 0;
            var have = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT `store`, `uid` FROM `player_blobs`;";
                    using (var r = cmd.ExecuteReader())
                        while (r.Read()) have.Add(r.GetString(0) + "|" + Convert.ToString(r.GetValue(1), System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            int n = 0;
            foreach (string store in _stores)
            {
                string dir = Path.Combine(_root, store);
                if (!Directory.Exists(dir)) continue;
                foreach (string f in Directory.GetFiles(dir, "*.bin"))
                {
                    uint uid; string st;
                    if (!ParsePath(f, out uid, out st) || have.Contains(st + "|" + uid)) continue;
                    string file = f;
                    MirrorQueue.Submit(Key(uid, st), "save", false, () =>
                    {
                        long fileTime = File.GetLastWriteTimeUtc(file).Ticks;
                        byte[] data = File.ReadAllBytes(file);
                        return () => Save(uid, st, data, fileTime);
                    }, "player_blobs backfill " + st + "/" + uid + " failed");
                    n++;
                }
            }
            return n;
        }
    }
}
