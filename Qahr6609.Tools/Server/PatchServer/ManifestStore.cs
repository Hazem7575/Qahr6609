using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace PatchServer
{
    /// <summary>
    /// Immutable result of one manifest build. Instances are swapped
    /// atomically, so readers always see a complete manifest.
    /// </summary>
    internal sealed class ManifestSnapshot
    {
        public ManifestSnapshot(byte[] json, int fileCount, long totalBytes, DateTime generatedUtc)
        {
            Json = json;
            FileCount = fileCount;
            TotalBytes = totalBytes;
            GeneratedUtc = generatedUtc;
        }

        /// <summary>UTF-8 (no BOM) bytes of manifest.json.</summary>
        public byte[] Json { get; private set; }

        public int FileCount { get; private set; }

        public long TotalBytes { get; private set; }

        public DateTime GeneratedUtc { get; private set; }
    }

    /// <summary>
    /// Builds Root\manifest.json from every file under Root\files and keeps
    /// the current version in memory. The HTTP server serves manifest.json
    /// from memory, so a request during a rebuild always gets either the old
    /// or the new manifest, never a half-written file.
    /// </summary>
    internal sealed class ManifestStore
    {
        private const int HashBufferSize = 1024 * 1024;
        private const string SkippedRootFile = "README.txt";

        private readonly string filesDir;
        private readonly string filesPrefix;
        private readonly string manifestPath;
        private readonly object rebuildLock = new object();

        // Reference swaps are atomic; volatile guarantees readers see the latest one.
        private volatile ManifestSnapshot current;

        public ManifestStore(string filesDir, string manifestPath)
        {
            this.filesDir = filesDir;
            this.filesPrefix = ServerConfig.EnsureTrailingSeparator(filesDir);
            this.manifestPath = manifestPath;
        }

        /// <summary>The latest successfully built manifest, or null if none yet.</summary>
        public ManifestSnapshot Current
        {
            get { return current; }
        }

        /// <summary>
        /// Rescans Root\files, hashes every file and publishes a new manifest.
        /// Returns false (keeping the previous manifest) on failure.
        /// Only one rebuild runs at a time.
        /// </summary>
        public bool Rebuild()
        {
            lock (rebuildLock)
            {
                Stopwatch watch = Stopwatch.StartNew();
                Log.Info("Building manifest from " + filesDir + " ...");

                try
                {
                    Directory.CreateDirectory(filesDir);

                    List<string> fullPaths = new List<string>();
                    CollectFiles(filesDir, fullPaths);

                    // Sort by relative path for a stable, diff-friendly manifest.
                    List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
                    foreach (string full in fullPaths)
                    {
                        if (!full.StartsWith(filesPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        string relative = full.Substring(filesPrefix.Length).Replace('\\', '/');
                        if (string.Equals(relative, SkippedRootFile, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        items.Add(new KeyValuePair<string, string>(relative, full));
                    }
                    items.Sort(CompareByKey);

                    List<object> entries = new List<object>(items.Count);
                    byte[] buffer = new byte[HashBufferSize];
                    long totalBytes = 0;
                    Stopwatch progress = Stopwatch.StartNew();

                    for (int i = 0; i < items.Count; i++)
                    {
                        string relative = items[i].Key;
                        string full = items[i].Value;

                        if (progress.ElapsedMilliseconds >= 2000)
                        {
                            Log.Info("  hashing " + (i + 1).ToString(CultureInfo.InvariantCulture) + "/"
                                + items.Count.ToString(CultureInfo.InvariantCulture) + ": " + relative);
                            progress.Restart();
                        }

                        long size;
                        string md5;
                        try
                        {
                            md5 = ComputeMd5(full, buffer, out size);
                        }
                        catch (Exception ex)
                        {
                            // A locked/unreadable file is skipped so the rest still gets published.
                            Log.Warn("Skipping unreadable file " + relative + ": " + ex.Message);
                            continue;
                        }

                        Dictionary<string, object> entry = new Dictionary<string, object>();
                        entry["path"] = relative;
                        entry["size"] = size;
                        entry["md5"] = md5;
                        entries.Add(entry);
                        totalBytes += size;
                    }

                    DateTime generated = DateTime.UtcNow;
                    Dictionary<string, object> root = new Dictionary<string, object>();
                    root["generated"] = generated.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
                    root["files"] = entries;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = int.MaxValue;
                    string json = serializer.Serialize(root);
                    byte[] bytes = new UTF8Encoding(false).GetBytes(json);

                    // Publish in memory first: this is what HTTP clients receive.
                    current = new ManifestSnapshot(bytes, entries.Count, totalBytes, generated);

                    // Then persist a copy to disk atomically (temp file + replace).
                    try
                    {
                        WriteAtomically(manifestPath, bytes);
                    }
                    catch (Exception ex)
                    {
                        Log.Warn("Manifest is served from memory but could not be written to disk: " + ex.Message);
                    }

                    Log.Info("Manifest ready: " + entries.Count.ToString(CultureInfo.InvariantCulture) + " files, "
                        + Log.FormatSize(totalBytes) + " (" + totalBytes.ToString(CultureInfo.InvariantCulture)
                        + " bytes) in " + watch.Elapsed.TotalSeconds.ToString("0.0", CultureInfo.InvariantCulture) + "s");
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error("Manifest rebuild failed, keeping previous manifest: " + ex.Message);
                    return false;
                }
            }
        }

        private static int CompareByKey(KeyValuePair<string, string> a, KeyValuePair<string, string> b)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(a.Key, b.Key);
        }

        /// <summary>Iteratively collects all files below rootDir, warning about unreadable folders.</summary>
        private static void CollectFiles(string rootDir, List<string> result)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(rootDir);

            while (pending.Count > 0)
            {
                string dir = pending.Pop();
                try
                {
                    result.AddRange(Directory.GetFiles(dir));
                    foreach (string sub in Directory.GetDirectories(dir))
                    {
                        pending.Push(sub);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Log.Warn("Cannot read folder " + dir + ": " + ex.Message);
                }
                catch (IOException ex)
                {
                    Log.Warn("Cannot read folder " + dir + ": " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Streams the file through MD5 with a large buffer (works for files of
        /// any size) and returns the lowercase hex digest. size receives the
        /// number of bytes actually hashed so size and md5 always match.
        /// </summary>
        private static string ComputeMd5(string path, byte[] buffer, out long size)
        {
            size = 0;
            using (MD5 md5 = MD5.Create())
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.SequentialScan))
            {
                int read;
                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    md5.TransformBlock(buffer, 0, read, null, 0);
                    size += read;
                }
                md5.TransformFinalBlock(buffer, 0, 0);
                return ToHex(md5.Hash);
            }
        }

        private static string ToHex(byte[] hash)
        {
            StringBuilder sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        /// <summary>Writes to "path.tmp" and then replaces the target in one step.</summary>
        private static void WriteAtomically(string path, byte[] bytes)
        {
            string tempPath = path + ".tmp";
            using (FileStream fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush(true);
            }

            if (File.Exists(path))
            {
                try
                {
                    File.Replace(tempPath, path, null, true);
                }
                catch (Exception)
                {
                    // Fallback (e.g. target opened by an editor without share-delete).
                    File.Copy(tempPath, path, true);
                    File.Delete(tempPath);
                }
            }
            else
            {
                File.Move(tempPath, path);
            }
        }
    }
}
