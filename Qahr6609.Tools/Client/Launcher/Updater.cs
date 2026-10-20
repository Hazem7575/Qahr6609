using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace ScriptHazemLauncher
{
    // manifest.json served by PatchServer:
    // {"generated":"...","files":[{"path":"Env_DX9/Conquer.exe","size":123,"md5":"..."}]}
    public class Manifest
    {
        public string generated { get; set; }
        public List<ManifestFile> files { get; set; }
    }

    public class ManifestFile
    {
        public string path { get; set; }
        public long size { get; set; }
        public string md5 { get; set; }
    }

    public class UpdateProgress
    {
        public string State;          // human readable status line
        public string FileName;       // current file (manifest path)
        public long FileDone;
        public long FileTotal;
        public long TotalDone;
        public long TotalAll;
        public double BytesPerSecond;
    }

    public class UpdateResult
    {
        public bool ServerReachable;
        public int Downloaded;
        public bool LauncherReplaced;  // launcher exe itself was updated -> restart needed
        public string Error;           // null when everything is fine
    }

    /// <summary>
    /// Compares local files with the server manifest and downloads what changed.
    ///
    /// Features:
    ///  * resumable downloads - each file is written to "file.ext.part" and resumed with an
    ///    HTTP Range request from wherever the last run stopped, then moved to the final name
    ///    once its MD5 matches the manifest;
    ///  * parallel downloads - several files at once (launcher.ini DownloadThreads, default 3);
    ///  * mirrors/fallback - launcher.ini UpdateUrlMirror1..N; a dead mirror or repeated chunk
    ///    failure falls back to the next base URL;
    ///  * retry with backoff on a failed chunk, and an optional global speed cap (DownloadKBps).
    ///
    /// MD5 of unchanged files is cached in launcher_cache.txt (path|size|mtime|md5) so big .wdf
    /// files are not re-hashed on every start.
    /// </summary>
    public class Updater
    {
        private readonly string[] _baseUrls;      // primary first, then mirrors (all end with '/')
        private readonly string _root;
        private readonly string _cachePath;
        private readonly int _parallelism;
        private readonly RateLimiter _rate;
        internal static readonly HttpClient Http = CreateHttp();
        private readonly Dictionary<string, string[]> _cache = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        private readonly object _cacheLock = new object();

        private const int MaxRetriesPerMirror = 3;
        private const int BackoffBaseMs = 400;
        private const int BufferSize = 1 << 16;

        /// <summary>Manifest paths that the launcher writes itself (the client server list when /servers answered).</summary>
        public HashSet<string> SkipPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>The base URL that actually answered the manifest (used for the file downloads).</summary>
        private string _activeBase;

        private static HttpClient CreateHttp()
        {
            SocketsHttpHandler handler = new SocketsHttpHandler
            {
                // a dead mirror must fail fast so the fallback kicks in, not after 100 s
                ConnectTimeout = TimeSpan.FromSeconds(8),
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                MaxConnectionsPerServer = 16,
                AutomaticDecompression = DecompressionMethods.None
            };
            HttpClient c = new HttpClient(handler);
            c.Timeout = TimeSpan.FromMinutes(30);   // whole-file timeout; per-connect handled above
            c.DefaultRequestHeaders.UserAgent.ParseAdd("scripthazemLauncher/2.0");
            return c;
        }

        internal static readonly JsonSerializerOptions Json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /// <summary>Backward-compatible single-url constructor.</summary>
        public Updater(string baseUrl, string rootDir)
            : this(new[] { baseUrl }, rootDir, 3, 0)
        {
        }

        /// <param name="baseUrls">Primary url first, then mirrors. Each should end with '/'.</param>
        /// <param name="parallelism">Number of files downloaded concurrently (clamped 1..8).</param>
        /// <param name="maxKBps">Global download cap in KB/s (0 = unlimited).</param>
        public Updater(IEnumerable<string> baseUrls, string rootDir, int parallelism, int maxKBps)
        {
            List<string> urls = new List<string>();
            if (baseUrls != null)
            {
                foreach (string u in baseUrls)
                {
                    if (string.IsNullOrWhiteSpace(u)) continue;
                    string v = u.Trim();
                    if (!v.EndsWith("/")) v += "/";
                    if (!urls.Contains(v)) urls.Add(v);
                }
            }
            _baseUrls = urls.ToArray();
            _activeBase = _baseUrls.Length > 0 ? _baseUrls[0] : null;
            _root = Path.GetFullPath(rootDir);
            _cachePath = Path.Combine(_root, "launcher_cache.txt");
            _parallelism = Math.Max(1, Math.Min(8, parallelism));
            _rate = new RateLimiter(maxKBps > 0 ? (long)maxKBps * 1024 : 0);
        }

        public UpdateResult Run(IProgress<UpdateProgress> progress, CancellationToken token)
        {
            UpdateResult result = new UpdateResult();
            CleanupOldLauncher();

            if (_baseUrls.Length == 0)
            {
                result.Error = Lang.T("UrlEmpty");
                return result;
            }

            // 1) fetch the manifest, trying each mirror until one answers
            Report(progress, Lang.T("Checking"), null, 0, 0, 0, 0, 0);
            Manifest manifest = null;
            string lastErr = null;
            foreach (string baseUrl in _baseUrls)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    string json = DownloadString(baseUrl + "manifest.json", token);
                    manifest = JsonSerializer.Deserialize<Manifest>(json, Json);
                    _activeBase = baseUrl;
                    result.ServerReachable = true;
                    break;
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    lastErr = ex.Message;
                }
            }
            if (!result.ServerReachable)
            {
                result.Error = Lang.T("UpdateOffline") + (lastErr != null ? " (" + lastErr + ")" : "");
                return result;
            }
            if (manifest == null || manifest.files == null) return result;

            LoadCache();

            // 2) find what needs downloading
            List<ManifestFile> needed = new List<ManifestFile>();
            int checkedCount = 0;
            foreach (ManifestFile f in manifest.files)
            {
                token.ThrowIfCancellationRequested();
                checkedCount++;
                string local = LocalPath(f.path);
                if (local == null) continue; // unsafe path -> ignore
                if (SkipPaths.Contains(f.path.Replace('\\', '/'))) continue;
                Report(progress, Lang.T("CheckingFiles") + " " + checkedCount + "/" + manifest.files.Count, f.path, 0, 0, 0, 0, 0);
                if (!IsUpToDate(f, local)) needed.Add(f);
            }

            long totalAll = 0;
            foreach (ManifestFile f in needed) totalAll += f.size;
            if (needed.Count == 0)
            {
                SaveCache();
                Report(progress, Lang.T("UpToDate"), null, 0, 0, 0, 0, 0);
                return result;
            }

            // 3) download (parallel, resumable, with mirror fallback)
            try
            {
                DownloadAllAsync(needed, totalAll, progress, token, result).GetAwaiter().GetResult();
            }
            catch (AggregateException ae)
            {
                // unwrap to the first meaningful exception
                Exception inner = ae.Flatten().InnerException ?? ae;
                if (inner is OperationCanceledException) throw inner;
                throw inner;
            }

            SaveCache();
            Report(progress, Lang.T("Updated", result.Downloaded), null, 0, 0, totalAll, totalAll, 0);
            return result;
        }

        // ---------------------------------------------------------------- download orchestration

        private async Task DownloadAllAsync(List<ManifestFile> needed, long totalAll,
            IProgress<UpdateProgress> progress, CancellationToken token, UpdateResult result)
        {
            // live, thread-safe progress state
            long completedBytes = 0;                                  // bytes of finished files
            ConcurrentDictionary<string, long[]> active = new ConcurrentDictionary<string, long[]>(); // path -> [done,total]

            // pre-count bytes already sitting in .part files so the total bar starts where we left off
            foreach (ManifestFile f in needed)
            {
                string local = LocalPath(f.path);
                string part = local + ".part";
                try
                {
                    if (File.Exists(part))
                    {
                        long plen = new FileInfo(part).Length;
                        if (plen > f.size) { TryDelete(part); plen = 0; }
                        if (plen > 0) active[f.path] = new long[] { plen, f.size };
                    }
                }
                catch { }
            }

            // background reporter: aggregate current-file + accurate total + overall speed
            bool finished = false;
            Task reporter = Task.Run(async () =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                long lastBytes = SnapshotTotal(ref completedBytes, active);
                long lastMs = 0;
                double emaBps = 0;
                while (!finished)
                {
                    await Task.Delay(150).ConfigureAwait(false);
                    long now = SnapshotTotal(ref completedBytes, active);
                    long ms = sw.ElapsedMilliseconds;
                    double dt = (ms - lastMs) / 1000.0;
                    if (dt > 0.01)
                    {
                        double bps = Math.Max(0, now - lastBytes) / dt;
                        emaBps = emaBps <= 0 ? bps : emaBps * 0.7 + bps * 0.3;
                        lastBytes = now;
                        lastMs = ms;
                    }

                    // aggregate of in-flight files for the "current file" bar + a representative name
                    long fileDone = 0, fileTotal = 0;
                    string name = null; long nameTotal = -1;
                    foreach (KeyValuePair<string, long[]> kv in active)
                    {
                        fileDone += kv.Value[0];
                        fileTotal += kv.Value[1];
                        if (kv.Value[1] > nameTotal) { nameTotal = kv.Value[1]; name = kv.Key; }
                    }
                    Report(progress, Lang.T("DownloadingUpdates"), name, fileDone, fileTotal, now, totalAll, emaBps);
                }
            });

            // worker pool: _parallelism files at once
            SemaphoreSlim gate = new SemaphoreSlim(_parallelism);
            List<Task> tasks = new List<Task>();
            CancellationTokenSource failCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            foreach (ManifestFile file in needed)
            {
                ManifestFile f = file;
                await gate.WaitAsync(failCts.Token).ConfigureAwait(false);
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await DownloadOneAsync(f, active, failCts.Token,
                            () => Interlocked.Add(ref completedBytes, f.size), result).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        failCts.Cancel(); // stop the others promptly on a hard failure
                        throw;
                    }
                    finally
                    {
                        gate.Release();
                    }
                }, failCts.Token));
            }

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            finally
            {
                finished = true;
                try { await reporter.ConfigureAwait(false); } catch { }
                failCts.Dispose();
            }
        }

        private static long SnapshotTotal(ref long completedBytes, ConcurrentDictionary<string, long[]> active)
        {
            long total = Interlocked.Read(ref completedBytes);
            foreach (KeyValuePair<string, long[]> kv in active) total += Interlocked.Read(ref kv.Value[0]);
            return total;
        }

        /// <summary>Downloads one file (resuming its .part), trying each mirror with retries, then finalises it.</summary>
        private async Task DownloadOneAsync(ManifestFile f, ConcurrentDictionary<string, long[]> active,
            CancellationToken token, Action onCompleted, UpdateResult result)
        {
            string local = LocalPath(f.path);
            string part = local + ".part";
            Directory.CreateDirectory(Path.GetDirectoryName(local));

            long[] prog = active.GetOrAdd(f.path, _ => new long[] { ExistingPartLen(part, f.size), f.size });

            // order the mirrors with the active one first
            List<string> mirrors = new List<string>();
            if (_activeBase != null) mirrors.Add(_activeBase);
            foreach (string b in _baseUrls) if (!mirrors.Contains(b)) mirrors.Add(b);

            Exception last = null;
            foreach (string baseUrl in mirrors)
            {
                for (int attempt = 0; attempt <= MaxRetriesPerMirror; attempt++)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        string md5 = await DownloadToPartAsync(baseUrl, f, part, prog, token).ConfigureAwait(false);
                        long len = new FileInfo(part).Length;
                        if (len == f.size && string.Equals(md5, f.md5, StringComparison.OrdinalIgnoreCase))
                        {
                            Finalize(f, local, part, result);
                            onCompleted();
                            active.TryRemove(f.path, out _);
                            return;
                        }
                        // wrong size or hash: the part is corrupt -> start it over
                        TryDelete(part);
                        Interlocked.Exchange(ref prog[0], 0);
                        last = new InvalidDataException(Lang.T("BadDownload", f.path));
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                        throw; // paused / closing: keep the part for a resume
                    }
                    catch (Exception ex)
                    {
                        last = ex;
                        // keep whatever bytes we have for the next attempt (resume)
                        Interlocked.Exchange(ref prog[0], ExistingPartLen(part, f.size));
                    }

                    if (attempt < MaxRetriesPerMirror)
                        await Task.Delay(BackoffBaseMs * (attempt + 1), token).ConfigureAwait(false);
                }
                // this mirror is exhausted -> try the next one
            }
            throw last ?? new IOException(Lang.T("BadDownload", f.path));
        }

        private static long ExistingPartLen(string part, long max)
        {
            try
            {
                if (File.Exists(part))
                {
                    long len = new FileInfo(part).Length;
                    if (len > max) { TryDelete(part); return 0; }
                    return len;
                }
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Downloads (or resumes) one file into its .part and returns the MD5 of the whole part.
        /// Uses a Range request when bytes already exist; if the server ignores the range
        /// (answers 200), the part is restarted from scratch.
        /// </summary>
        private async Task<string> DownloadToPartAsync(string baseUrl, ManifestFile f, string part, long[] prog, CancellationToken token)
        {
            string url = baseUrl + "files/" + EscapePath(f.path);
            long existing = ExistingPartLen(part, f.size);

            // the .part already holds the whole file (finished last time but not yet moved):
            // just hash it, no Range request (asking for bytes=size- would get a 416).
            if (existing == f.size && f.size > 0)
            {
                Interlocked.Exchange(ref prog[0], existing);
                return Md5Of(part);
            }

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url);
            if (existing > 0) req.Headers.Range = new RangeHeaderValue(existing, null);

            using (HttpResponseMessage resp = await Http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
            {
                // a stale/over-long part can still draw a 416: drop it and restart from zero
                if (resp.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                {
                    TryDelete(part);
                    Interlocked.Exchange(ref prog[0], 0);
                    throw new IOException("range not satisfiable for " + f.path + " (restarting)");
                }
                resp.EnsureSuccessStatusCode();
                bool resume = existing > 0 && resp.StatusCode == HttpStatusCode.PartialContent;
                if (!resume) existing = 0; // no part, or the server ignored the Range -> full download

                using (MD5 md5 = MD5.Create())
                {
                    byte[] buffer = new byte[BufferSize];

                    // hash the bytes already on disk (resume only)
                    if (resume)
                    {
                        using (FileStream pre = new FileStream(part, FileMode.Open, FileAccess.Read, FileShare.None, BufferSize))
                        {
                            long left = existing;
                            int r;
                            while (left > 0 && (r = pre.Read(buffer, 0, (int)Math.Min(buffer.Length, left))) > 0)
                            {
                                md5.TransformBlock(buffer, 0, r, null, 0);
                                left -= r;
                            }
                            if (left != 0) { resume = false; existing = 0; } // part was shorter than expected
                        }
                    }

                    Interlocked.Exchange(ref prog[0], existing);

                    FileMode mode = resume ? FileMode.Open : FileMode.Create;
                    using (Stream input = await resp.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                    using (FileStream output = new FileStream(part, mode, FileAccess.Write, FileShare.None, BufferSize))
                    {
                        if (resume) output.Seek(existing, SeekOrigin.Begin);
                        int read;
                        while ((read = await input.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false)) > 0)
                        {
                            await output.WriteAsync(buffer, 0, read, token).ConfigureAwait(false);
                            md5.TransformBlock(buffer, 0, read, null, 0);
                            Interlocked.Add(ref prog[0], read);
                            _rate.Throttle(read, token);
                        }
                        output.Flush();
                    }
                    md5.TransformFinalBlock(buffer, 0, 0);
                    return ToHex(md5.Hash);
                }
            }
        }

        /// <summary>Moves a verified .part to its final name (renaming a running launcher exe to .old first).</summary>
        private void Finalize(ManifestFile f, string local, string part, UpdateResult result)
        {
            string selfPath = Environment.ProcessPath;
            if (selfPath != null && string.Equals(Path.GetFullPath(local), Path.GetFullPath(selfPath), StringComparison.OrdinalIgnoreCase))
            {
                // a running exe can be renamed but not overwritten
                TryDelete(local + ".old");
                File.Move(local, local + ".old");
                lock (_cacheLock) result.LauncherReplaced = true;
            }
            else if (File.Exists(local))
            {
                File.SetAttributes(local, FileAttributes.Normal);
                File.Delete(local);
            }
            File.Move(part, local);

            FileInfo fi = new FileInfo(local);
            lock (_cacheLock)
            {
                _cache[f.path] = new[] { f.path, fi.Length.ToString(), fi.LastWriteTimeUtc.Ticks.ToString(), f.md5.ToLowerInvariant() };
                result.Downloaded++;
            }
        }

        // ---------------------------------------------------------------- helpers

        private string LocalPath(string manifestPath)
        {
            if (string.IsNullOrEmpty(manifestPath)) return null;
            string full = Path.GetFullPath(Path.Combine(_root, manifestPath.Replace('/', Path.DirectorySeparatorChar)));
            string rootWithSep = _root.EndsWith(Path.DirectorySeparatorChar.ToString()) ? _root : _root + Path.DirectorySeparatorChar;
            return full.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase) ? full : null;
        }

        private bool IsUpToDate(ManifestFile f, string local)
        {
            FileInfo fi = new FileInfo(local);
            if (!fi.Exists || fi.Length != f.size) return false;
            string ticks = fi.LastWriteTimeUtc.Ticks.ToString();
            string[] c;
            if (_cache.TryGetValue(f.path, out c) && c[1] == fi.Length.ToString() && c[2] == ticks)
                return string.Equals(c[3], f.md5, StringComparison.OrdinalIgnoreCase);
            string md5 = Md5Of(local);
            _cache[f.path] = new[] { f.path, fi.Length.ToString(), ticks, md5 };
            return string.Equals(md5, f.md5, StringComparison.OrdinalIgnoreCase);
        }

        private static string EscapePath(string path)
        {
            string[] parts = path.Split('/');
            for (int i = 0; i < parts.Length; i++) parts[i] = Uri.EscapeDataString(parts[i]);
            return string.Join("/", parts);
        }

        internal static string DownloadString(string url)
        {
            return DownloadString(url, CancellationToken.None);
        }

        internal static string DownloadString(string url, CancellationToken token)
        {
            byte[] data = Http.GetByteArrayAsync(url + (url.Contains("?") ? "&" : "?") + "t=" + DateTime.UtcNow.Ticks, token).GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(data);
        }

        private static string Md5Of(string file)
        {
            using (MD5 md5 = MD5.Create())
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, BufferSize))
                return ToHex(md5.ComputeHash(fs));
        }

        private static string ToHex(byte[] hash)
        {
            StringBuilder sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private void LoadCache()
        {
            _cache.Clear();
            try
            {
                if (!File.Exists(_cachePath)) return;
                foreach (string line in File.ReadAllLines(_cachePath, Encoding.UTF8))
                {
                    string[] p = line.Split('|');
                    if (p.Length == 4) _cache[p[0]] = p;
                }
            }
            catch { _cache.Clear(); }
        }

        private void SaveCache()
        {
            try
            {
                List<string> lines = new List<string>();
                lock (_cacheLock)
                    foreach (string[] p in _cache.Values) lines.Add(string.Join("|", p));
                File.WriteAllLines(_cachePath, lines.ToArray(), Encoding.UTF8);
            }
            catch { /* cache is only an optimisation */ }
        }

        private static void CleanupOldLauncher()
        {
            try
            {
                string self = Environment.ProcessPath;
                TryDelete(self + ".old");
            }
            catch { }
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) { File.SetAttributes(path, FileAttributes.Normal); File.Delete(path); } }
            catch { }
        }

        private static void Report(IProgress<UpdateProgress> p, string state, string file, long fileDone, long fileTotal, long totalDone, long totalAll, double bps)
        {
            if (p == null) return;
            p.Report(new UpdateProgress
            {
                State = state,
                FileName = file,
                FileDone = fileDone,
                FileTotal = fileTotal,
                TotalDone = totalDone,
                TotalAll = totalAll,
                BytesPerSecond = bps
            });
        }

        /// <summary>Global download speed cap (bytes/s) shared by every parallel worker; 0 = unlimited.</summary>
        private sealed class RateLimiter
        {
            private readonly long _bytesPerSec;
            private readonly object _gate = new object();
            private long _windowStartMs;
            private long _bytesInWindow;
            private readonly Stopwatch _sw = Stopwatch.StartNew();

            public RateLimiter(long bytesPerSec) { _bytesPerSec = bytesPerSec; }

            public void Throttle(int n, CancellationToken token)
            {
                if (_bytesPerSec <= 0) return;
                int sleepMs = 0;
                lock (_gate)
                {
                    long now = _sw.ElapsedMilliseconds;
                    if (now - _windowStartMs >= 1000) { _windowStartMs = now; _bytesInWindow = 0; }
                    _bytesInWindow += n;
                    if (_bytesInWindow > _bytesPerSec)
                    {
                        long wait = 1000 - (now - _windowStartMs);
                        if (wait > 0) sleepMs = (int)wait;
                    }
                }
                if (sleepMs > 0)
                {
                    try { token.WaitHandle.WaitOne(sleepMs); } catch { }
                }
            }
        }
    }
}
