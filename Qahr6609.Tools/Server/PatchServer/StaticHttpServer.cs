using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatchServer
{
    /// <summary>
    /// Minimal HTTP/1.1 static file server on top of TcpListener (no
    /// HttpListener, so no admin rights or urlacl registration are needed).
    /// One request per connection ("Connection: close"); every connection is
    /// handled on a thread-pool task. Supports GET, HEAD and single byte ranges.
    /// </summary>
    internal sealed class StaticHttpServer
    {
        private const int MaxHeaderBytes = 16 * 1024;
        private const int HeaderTimeoutMs = 15000;
        private const int SendTimeoutMs = 60000;
        private const int DrainTimeoutMs = 1000;
        private const int BodyBufferSize = 64 * 1024;
        private const string ServerName = "scripthazem-PatchServer";
        private const string RootPageText = "scripthazem PatchServer OK";

        private static readonly Dictionary<string, string> ContentTypes = CreateContentTypes();

        private readonly ServerConfig config;
        private readonly ManifestStore manifest;
        private readonly ServerRegistry registry;
        private readonly NewsStore news;
        private readonly StatusStore status;
        private readonly EventsStore events;
        private readonly CrashStore crashes;

        private TcpListener listener;
        private Thread acceptThread;
        private volatile bool running;
        private int activeConnections;

        // [Security] connection and upload limits (one host must not be able to tie up the server or fill the disk)
        private const int MaxConnections = 256, MaxConnectionsPerIp = 16;
        private const int CrashPerIpPerHour = 6;
        private const long CrashBytesPerDay = 100L * 1024 * 1024;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> connectionsByIp = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        private readonly object crashLimitLock = new object();
        private readonly System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<DateTime>> crashTimes = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<DateTime>>();
        private DateTime crashDay = DateTime.MinValue;
        private long crashBytesToday;

        private static string IpOf(TcpClient c)
        {
            try { IPEndPoint ep = c.Client.RemoteEndPoint as IPEndPoint; return ep != null ? ep.Address.ToString() : "?"; }
            catch (Exception) { return "?"; }
        }

        /// <summary>true = this upload is within the per-IP hourly count and the daily byte budget (and is counted).</summary>
        private bool CrashAllowed(string ip, long bytes)
        {
            lock (crashLimitLock)
            {
                DateTime now = DateTime.Now;
                if (now.Date != crashDay) { crashDay = now.Date; crashBytesToday = 0; crashTimes.Clear(); }
                if (crashBytesToday + bytes > CrashBytesPerDay) return false;
                System.Collections.Generic.List<DateTime> list;
                if (!crashTimes.TryGetValue(ip, out list)) crashTimes[ip] = list = new System.Collections.Generic.List<DateTime>();
                list.RemoveAll(t => (now - t).TotalHours >= 1);
                if (list.Count >= CrashPerIpPerHour) return false;
                list.Add(now);
                crashBytesToday += bytes;
                return true;
            }
        }
        private long totalRequests;

        private enum RangeResult
        {
            None,
            Valid,
            Unsatisfiable
        }

        public StaticHttpServer(ServerConfig config, ManifestStore manifest, ServerRegistry registry,
            NewsStore news, StatusStore status, EventsStore events, CrashStore crashes)
        {
            this.config = config;
            this.manifest = manifest;
            this.registry = registry;
            this.news = news;
            this.status = status;
            this.events = events;
            this.crashes = crashes;
        }

        /// <summary>Number of connections currently being served.</summary>
        public int ActiveConnections
        {
            get { return Volatile.Read(ref activeConnections); }
        }

        /// <summary>Total number of parsed requests since start.</summary>
        public long TotalRequests
        {
            get { return Interlocked.Read(ref totalRequests); }
        }

        /// <summary>Binds to IPAddress.Any:Port and starts accepting. Throws SocketException if the port is busy.</summary>
        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, config.Port);
            listener.Start(200);
            running = true;

            acceptThread = new Thread(AcceptLoop);
            acceptThread.IsBackground = true;
            acceptThread.Name = "PatchServer accept";
            acceptThread.Start();
        }

        /// <summary>Stops accepting new connections.</summary>
        public void Stop()
        {
            running = false;
            try
            {
                if (listener != null)
                {
                    listener.Stop();
                }
            }
            catch (Exception)
            {
            }
        }

        private void AcceptLoop()
        {
            while (running)
            {
                TcpClient client;
                try
                {
                    client = listener.AcceptTcpClient();
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch (SocketException ex)
                {
                    if (!running)
                    {
                        break;
                    }
                    Log.Warn("Accept failed: " + ex.Message);
                    Thread.Sleep(100);
                    continue;
                }

                TcpClient accepted = client;
                string ip = IpOf(accepted);
                int perIp = connectionsByIp.AddOrUpdate(ip, 1, (k, v) => v + 1);
                if (perIp > MaxConnectionsPerIp || Volatile.Read(ref activeConnections) >= MaxConnections)
                {
                    connectionsByIp.AddOrUpdate(ip, 0, (k, v) => v - 1);
                    try { accepted.Close(); } catch (Exception) { }// over the limit: dropped right after accept
                    continue;
                }
                Task.Run(() =>
                {
                    try { HandleClient(accepted); }
                    finally
                    {
                        int left = connectionsByIp.AddOrUpdate(ip, 0, (k, v) => v - 1);
                        if (left <= 0) { int z; connectionsByIp.TryRemove(ip, out z); }
                    }
                });
            }
        }

        /// <summary>Handles one connection. Never throws: all errors are logged.</summary>
        private void HandleClient(TcpClient client)
        {
            string remote = "?";
            Interlocked.Increment(ref activeConnections);
            try
            {
                using (client)
                {
                    try
                    {
                        IPEndPoint ep = client.Client.RemoteEndPoint as IPEndPoint;
                        if (ep != null)
                        {
                            remote = ep.Address.ToString();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    client.NoDelay = true;
                    client.SendTimeout = SendTimeoutMs;
                    NetworkStream net = client.GetStream();

                    ProcessRequest(net, remote);
                    GracefulClose(client.Client);
                }
            }
            catch (IOException ex)
            {
                Log.Info(remote + " connection error: " + ShortMessage(ex));
            }
            catch (SocketException ex)
            {
                Log.Info(remote + " socket error: " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                // Connection closed underneath us; nothing to report.
            }
            catch (Exception ex)
            {
                Log.Error(remote + " unexpected error: " + ex);
            }
            finally
            {
                Interlocked.Decrement(ref activeConnections);
            }
        }

        /// <summary>
        /// Half-closes the socket and briefly drains unread input so the
        /// response is not cut off by a TCP reset.
        /// </summary>
        private static void GracefulClose(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Send);
                socket.ReceiveTimeout = DrainTimeoutMs;
                byte[] scratch = new byte[4096];
                int drained = 0;
                while (drained < 64 * 1024)
                {
                    int n = socket.Receive(scratch);
                    if (n <= 0)
                    {
                        break;
                    }
                    drained += n;
                }
            }
            catch (Exception)
            {
                // Timeout or reset while draining is fine; we are closing anyway.
            }
        }

        private void ProcessRequest(NetworkStream net, string remote)
        {
            bool tooLarge;
            byte[] prebuf;
            int bodyStart;
            int bodyCount;
            string headerText = ReadHeaderBlock(net, out tooLarge, out prebuf, out bodyStart, out bodyCount);
            if (headerText == null)
            {
                if (tooLarge)
                {
                    SendText(net, remote, null, 431, "Request headers too large.", null);
                }
                // Otherwise: client closed or timed out before sending a request.
                return;
            }

            Interlocked.Increment(ref totalRequests);

            HttpRequest req = HttpRequest.Parse(headerText);
            if (req == null)
            {
                SendText(net, remote, null, 400, "Bad request.", null);
                return;
            }

            // POST /crash: the launcher uploading a crash report. Everything else is GET/HEAD.
            if (req.Method == "POST")
            {
                if (crashes != null && config.AcceptCrashReports
                    && string.Equals(req.Path, "/crash", StringComparison.OrdinalIgnoreCase))
                {
                    HandleCrashPost(net, remote, req, prebuf, bodyStart, bodyCount);
                }
                else
                {
                    SendText(net, remote, req, 405, "Only GET and HEAD are supported.", "Allow: GET, HEAD\r\n");
                }
                return;
            }

            if (req.Method != "GET" && req.Method != "HEAD")
            {
                SendText(net, remote, req, 405, "Only GET and HEAD are supported.", "Allow: GET, HEAD\r\n");
                return;
            }

            if (req.Path == "/")
            {
                SendText(net, remote, req, 200, RootPageText, null);
                return;
            }

            if (registry != null
                && (string.Equals(req.Path, "/servers", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(req.Path, "/servers.json", StringComparison.OrdinalIgnoreCase)))
            {
                ServeServers(net, remote, req);
                return;
            }

            if (news != null && string.Equals(req.Path, "/news.json", StringComparison.OrdinalIgnoreCase))
            {
                ServeNews(net, remote, req);
                return;
            }

            if (status != null && string.Equals(req.Path, "/status.json", StringComparison.OrdinalIgnoreCase))
            {
                ServeStatus(net, remote, req);
                return;
            }

            if (events != null && string.Equals(req.Path, "/events.json", StringComparison.OrdinalIgnoreCase))
            {
                ServeEvents(net, remote, req);
                return;
            }

            string fullPath;
            int resolveStatus = ResolvePath(req.Path, out fullPath);
            if (resolveStatus != 0)
            {
                SendText(net, remote, req, resolveStatus, ReasonPhrase(resolveStatus) + ".", null);
                return;
            }

            if (string.Equals(fullPath, config.ManifestPath, StringComparison.OrdinalIgnoreCase))
            {
                ServeManifest(net, remote, req);
                return;
            }

            ServeFile(net, remote, req, fullPath);
        }

        /// <summary>
        /// Maps a decoded URL path to a full file path under Root.
        /// Returns 0 on success, otherwise the HTTP status to send.
        /// </summary>
        private int ResolvePath(string urlPath, out string fullPath)
        {
            fullPath = null;
            if (urlPath.IndexOf('\0') >= 0)
            {
                return 400;
            }

            string relative = urlPath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            if (relative.Length == 0)
            {
                return 404;
            }

            string candidate;

            // /files/<path> comes from FilesDir ([Server] Files=, may be outside Root), with its own traversal guard.
            string filesUrl = "files" + Path.DirectorySeparatorChar;
            if (relative.StartsWith(filesUrl, StringComparison.OrdinalIgnoreCase))
            {
                string sub = relative.Substring(filesUrl.Length);
                if (sub.Length == 0)
                {
                    return 404;
                }
                try
                {
                    candidate = Path.GetFullPath(Path.Combine(config.FilesDir, sub));
                }
                catch (Exception)
                {
                    return 403;
                }
                if (!candidate.StartsWith(config.FilesPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return 403;
                }
                fullPath = candidate;
                return 0;
            }

            try
            {
                candidate = Path.GetFullPath(Path.Combine(config.RootDir, relative));
            }
            catch (Exception)
            {
                // Invalid characters, "file:stream" syntax, too long, etc.
                return 403;
            }

            // Path traversal guard: the resolved path must be strictly inside Root.
            if (!candidate.StartsWith(config.RootPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return 403;
            }

            // [Security] private folders under Root are never served (older crash reports, generated Guard.txt)
            string rel = candidate.Substring(config.RootPrefix.Length);
            if (rel.StartsWith("data" + Path.DirectorySeparatorChar + "crashes", StringComparison.OrdinalIgnoreCase)
                || rel.StartsWith("generated" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                || rel.Equals("generated", StringComparison.OrdinalIgnoreCase))
            {
                return 404;
            }

            fullPath = candidate;
            return 0;
        }

        /// <summary>
        /// POST /crash: reads the (length-delimited) report body, caps it at config.MaxCrashBytes and
        /// hands it to the CrashStore. Optional X-Report-Kind / X-Launcher-Version / X-Player-Note
        /// headers label the saved file. Always answers so the launcher is never left hanging.
        /// </summary>
        private void HandleCrashPost(NetworkStream net, string remote, HttpRequest req,
            byte[] prebuf, int bodyStart, int bodyCount)
        {
            string lenText;
            long contentLength;
            if (!req.Headers.TryGetValue("Content-Length", out lenText)
                || !long.TryParse((lenText ?? "").Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out contentLength)
                || contentLength < 0)
            {
                SendText(net, remote, req, 411, "Length Required.", null);
                return;
            }
            if (contentLength == 0)
            {
                SendText(net, remote, req, 400, "Empty report.", null);
                return;
            }
            if (contentLength > config.MaxCrashBytes)
            {
                SendText(net, remote, req, 413, "Report too large.", null);
                return;
            }

            if (!CrashAllowed(remote, contentLength))
            {
                SendText(net, remote, req, 429, "Too many reports.", null);
                return;
            }

            byte[] body;
            try
            {
                body = ReadBody(net, prebuf, bodyStart, bodyCount, (int)contentLength);
            }
            catch (IOException)
            {
                return; // timed out or the connection dropped mid-upload; nothing to answer to.
            }
            if (body == null)
            {
                SendText(net, remote, req, 400, "Incomplete report.", null);
                return;
            }

            string kind, version, note;
            req.Headers.TryGetValue("X-Report-Kind", out kind);
            req.Headers.TryGetValue("X-Launcher-Version", out version);
            req.Headers.TryGetValue("X-Player-Note", out note);

            string saved = crashes.Save(remote, body, kind, version, note);
            if (saved != null)
            {
                SendText(net, remote, req, 200, "OK", null);// [Security] the stored name is not told to the uploader
            }
            else
            {
                SendText(net, remote, req, 500, "Could not store the report.", null);
            }
        }

        /// <summary>
        /// Reads exactly length bytes of request body, starting with the bytes already pulled into
        /// prebuf by the header read. Returns null if the peer closes before length bytes arrive.
        /// Throws IOException on timeout.
        /// </summary>
        private static byte[] ReadBody(NetworkStream net, byte[] prebuf, int bodyStart, int bodyCount, int length)
        {
            byte[] body = new byte[length];
            int have = 0;

            int fromBuf = Math.Min(bodyCount, length);
            if (fromBuf > 0)
            {
                Buffer.BlockCopy(prebuf, bodyStart, body, 0, fromBuf);
                have = fromBuf;
            }

            Stopwatch watch = Stopwatch.StartNew();
            while (have < length)
            {
                long remainingMs = HeaderTimeoutMs - watch.ElapsedMilliseconds;
                if (remainingMs <= 0) throw new IOException("body read timed out");
                net.ReadTimeout = (int)remainingMs;
                int read = net.Read(body, have, length - have);
                if (read <= 0) return null; // peer closed early
                have += read;
            }
            return body;
        }

        private void ServeManifest(NetworkStream net, string remote, HttpRequest req)
        {
            ManifestSnapshot snapshot = manifest.Current;
            if (snapshot == null)
            {
                SendText(net, remote, req, 503, "Manifest not available yet.", null);
                return;
            }

            using (MemoryStream body = new MemoryStream(snapshot.Json, false))
            {
                SendBody(net, remote, req, body, snapshot.Json.Length, "application/json; charset=utf-8", true);
            }
        }

        /// <summary>GET /servers: published game servers with live status (see ServerRegistry).</summary>
        private void ServeServers(NetworkStream net, string remote, HttpRequest req)
        {
            byte[] json;
            try
            {
                json = registry.BuildJson();
            }
            catch (Exception ex)
            {
                Log.Warn("/servers failed: " + ex.Message);
                SendText(net, remote, req, 500, "Server list not available.", null);
                return;
            }

            using (MemoryStream body = new MemoryStream(json, false))
            {
                SendBody(net, remote, req, body, json.Length, "application/json; charset=utf-8", true);
            }
        }

        /// <summary>GET /news.json: the launcher news feed, built from www\data\news.json (newest first).</summary>
        private void ServeNews(NetworkStream net, string remote, HttpRequest req)
        {
            byte[] json;
            try
            {
                json = news.Current;
            }
            catch (Exception ex)
            {
                Log.Warn("/news.json failed: " + ex.Message);
                SendText(net, remote, req, 500, "News feed not available.", null);
                return;
            }

            using (MemoryStream body = new MemoryStream(json, false))
            {
                SendBody(net, remote, req, body, json.Length, "application/json; charset=utf-8", true);
            }
        }

        /// <summary>GET /status.json: maintenance banner (www\data\status.json) + live game-server status.</summary>
        private void ServeStatus(NetworkStream net, string remote, HttpRequest req)
        {
            byte[] json;
            try
            {
                json = status.Build();
            }
            catch (Exception ex)
            {
                Log.Warn("/status.json failed: " + ex.Message);
                SendText(net, remote, req, 500, "Status not available.", null);
                return;
            }

            using (MemoryStream body = new MemoryStream(json, false))
            {
                SendBody(net, remote, req, body, json.Length, "application/json; charset=utf-8", true);
            }
        }

        /// <summary>GET /events.json: upcoming events for the launcher countdown (www\data\events.json).</summary>
        private void ServeEvents(NetworkStream net, string remote, HttpRequest req)
        {
            byte[] json;
            try
            {
                json = events.Current;
            }
            catch (Exception ex)
            {
                Log.Warn("/events.json failed: " + ex.Message);
                SendText(net, remote, req, 500, "Events not available.", null);
                return;
            }

            using (MemoryStream body = new MemoryStream(json, false))
            {
                SendBody(net, remote, req, body, json.Length, "application/json; charset=utf-8", true);
            }
        }

        private void ServeFile(NetworkStream net, string remote, HttpRequest req, string fullPath)
        {
            if (Directory.Exists(fullPath) || !File.Exists(fullPath))
            {
                SendText(net, remote, req, 404, "Not found.", null);
                return;
            }

            FileStream fs;
            try
            {
                fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.SequentialScan);
            }
            catch (FileNotFoundException)
            {
                SendText(net, remote, req, 404, "Not found.", null);
                return;
            }
            catch (DirectoryNotFoundException)
            {
                SendText(net, remote, req, 404, "Not found.", null);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                SendText(net, remote, req, 403, "Forbidden.", null);
                return;
            }
            catch (IOException)
            {
                SendText(net, remote, req, 503, "File is busy, try again.", null);
                return;
            }

            using (fs)
            {
                string extension = (Path.GetExtension(fullPath) ?? string.Empty).ToLowerInvariant();
                bool noCache = extension == ".json";
                SendBody(net, remote, req, fs, fs.Length, GetContentType(extension), noCache);
            }
        }

        /// <summary>
        /// Sends a 200 (or 206 for a valid Range) response streaming the body
        /// with a 64 KB buffer. HEAD requests get headers only.
        /// </summary>
        private void SendBody(NetworkStream net, string remote, HttpRequest req, Stream body, long length,
            string contentType, bool noCache)
        {
            int status = 200;
            long start = 0;
            long end = length - 1;

            string rangeHeader;
            if (req.Headers.TryGetValue("Range", out rangeHeader))
            {
                long rangeStart;
                long rangeEnd;
                RangeResult result = ParseRange(rangeHeader, length, out rangeStart, out rangeEnd);
                if (result == RangeResult.Unsatisfiable)
                {
                    SendText(net, remote, req, 416, "Requested range not satisfiable.",
                        "Content-Range: bytes */" + length.ToString(CultureInfo.InvariantCulture) + "\r\n");
                    return;
                }
                if (result == RangeResult.Valid)
                {
                    status = 206;
                    start = rangeStart;
                    end = rangeEnd;
                }
                // RangeResult.None (malformed / unsupported): serve the full body.
            }

            long count = length > 0 ? end - start + 1 : 0;

            StringBuilder head = BeginHeaders(status);
            head.Append("Content-Type: ").Append(contentType).Append("\r\n");
            head.Append("X-Content-Type-Options: nosniff\r\n");
            head.Append("Content-Length: ").Append(count.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            head.Append("Accept-Ranges: bytes\r\n");
            if (status == 206)
            {
                head.Append("Content-Range: bytes ")
                    .Append(start.ToString(CultureInfo.InvariantCulture)).Append('-')
                    .Append(end.ToString(CultureInfo.InvariantCulture)).Append('/')
                    .Append(length.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            }
            if (noCache)
            {
                head.Append("Cache-Control: no-cache\r\n");
            }
            head.Append("Connection: close\r\n\r\n");

            byte[] headBytes = Encoding.ASCII.GetBytes(head.ToString());
            net.Write(headBytes, 0, headBytes.Length);

            if (req.IsHead || count == 0)
            {
                Log.Request(remote, req.Method, req.Path, status, 0, null);
                return;
            }

            long sent = 0;
            try
            {
                if (start > 0)
                {
                    body.Seek(start, SeekOrigin.Begin);
                }

                byte[] buffer = new byte[BodyBufferSize];
                long remaining = count;
                while (remaining > 0)
                {
                    int want = remaining > buffer.Length ? buffer.Length : (int)remaining;
                    int read = body.Read(buffer, 0, want);
                    if (read <= 0)
                    {
                        break; // File was truncated while we were sending it.
                    }
                    net.Write(buffer, 0, read);
                    sent += read;
                    remaining -= read;
                }
            }
            catch (IOException ex)
            {
                Log.Request(remote, req.Method, req.Path, status, sent, "aborted: " + ShortMessage(ex));
                return;
            }
            catch (ObjectDisposedException)
            {
                Log.Request(remote, req.Method, req.Path, status, sent, "aborted");
                return;
            }

            Log.Request(remote, req.Method, req.Path, status, sent, sent < count ? "file shrank during send" : null);
        }

        /// <summary>Sends a short text/plain response (also used for errors).</summary>
        private static void SendText(NetworkStream net, string remote, HttpRequest req, int status, string text,
            string extraHeaders)
        {
            byte[] body = Encoding.UTF8.GetBytes(text + "\n");
            bool isHead = req != null && req.IsHead;

            StringBuilder head = BeginHeaders(status);
            head.Append("Content-Type: text/plain; charset=utf-8\r\n");
            head.Append("Content-Length: ").Append(body.Length.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            head.Append("Cache-Control: no-cache\r\n");
            if (!string.IsNullOrEmpty(extraHeaders))
            {
                head.Append(extraHeaders);
            }
            head.Append("Connection: close\r\n\r\n");

            byte[] headBytes = Encoding.ASCII.GetBytes(head.ToString());
            net.Write(headBytes, 0, headBytes.Length);
            if (!isHead)
            {
                net.Write(body, 0, body.Length);
            }

            string method = req != null ? req.Method : "-";
            string path = req != null ? req.Path : "-";
            Log.Request(remote, method, path, status, isHead ? 0 : body.Length, null);
        }

        private static StringBuilder BeginHeaders(int status)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("HTTP/1.1 ").Append(status.ToString(CultureInfo.InvariantCulture)).Append(' ')
                .Append(ReasonPhrase(status)).Append("\r\n");
            sb.Append("Server: ").Append(ServerName).Append("\r\n");
            sb.Append("Date: ").Append(DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture)).Append("\r\n");
            return sb;
        }

        /// <summary>
        /// Parses "bytes=start-" or "bytes=start-end". Multiple ranges, suffix
        /// ranges ("bytes=-500") and garbage return None (serve full 200).
        /// A start beyond the end of the file returns Unsatisfiable (416).
        /// </summary>
        private static RangeResult ParseRange(string header, long length, out long start, out long end)
        {
            start = 0;
            end = 0;

            string value = header.Trim();
            if (!value.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase))
            {
                return RangeResult.None;
            }
            value = value.Substring(6).Trim();
            if (value.IndexOf(',') >= 0)
            {
                return RangeResult.None;
            }

            int dash = value.IndexOf('-');
            if (dash <= 0)
            {
                return RangeResult.None;
            }

            string startText = value.Substring(0, dash).Trim();
            string endText = value.Substring(dash + 1).Trim();

            long first;
            if (!long.TryParse(startText, NumberStyles.None, CultureInfo.InvariantCulture, out first))
            {
                return RangeResult.None;
            }

            long last;
            if (endText.Length == 0)
            {
                last = length - 1;
            }
            else
            {
                if (!long.TryParse(endText, NumberStyles.None, CultureInfo.InvariantCulture, out last))
                {
                    return RangeResult.None;
                }
                if (last < first)
                {
                    return RangeResult.None;
                }
                if (last > length - 1)
                {
                    last = length - 1;
                }
            }

            if (first >= length)
            {
                return RangeResult.Unsatisfiable;
            }

            start = first;
            end = last;
            return RangeResult.Valid;
        }

        /// <summary>
        /// Reads bytes until the blank line ending the header block
        /// (CR LF CR LF). Gives up after 16 KB (tooLarge = true) or 15 seconds.
        /// Returns null if no complete header block was received.
        /// </summary>
        private static string ReadHeaderBlock(NetworkStream stream, out bool tooLarge,
            out byte[] buffer, out int bodyStart, out int bodyCount)
        {
            tooLarge = false;
            buffer = new byte[MaxHeaderBytes];
            bodyStart = 0;
            bodyCount = 0;
            int total = 0;
            Stopwatch watch = Stopwatch.StartNew();

            while (true)
            {
                long remainingMs = HeaderTimeoutMs - watch.ElapsedMilliseconds;
                if (remainingMs <= 0)
                {
                    return null;
                }
                if (total >= buffer.Length)
                {
                    tooLarge = true;
                    return null;
                }

                stream.ReadTimeout = (int)remainingMs;
                int read;
                try
                {
                    read = stream.Read(buffer, total, buffer.Length - total);
                }
                catch (IOException)
                {
                    return null; // Timeout or connection reset.
                }
                if (read <= 0)
                {
                    return null;
                }

                int searchFrom = total >= 3 ? total - 3 : 0;
                total += read;

                int endIndex = FindHeaderEnd(buffer, searchFrom, total);
                if (endIndex >= 0)
                {
                    // Any bytes past the CR LF CR LF belong to the body (POST); hand them back.
                    bodyStart = endIndex + 4;
                    bodyCount = total - bodyStart;
                    // UTF-8 so that unencoded non-ASCII paths still decode sensibly.
                    return Encoding.UTF8.GetString(buffer, 0, endIndex);
                }
            }
        }

        private static int FindHeaderEnd(byte[] buffer, int from, int count)
        {
            for (int i = from; i + 3 < count; i++)
            {
                if (buffer[i] == 13 && buffer[i + 1] == 10 && buffer[i + 2] == 13 && buffer[i + 3] == 10)
                {
                    return i;
                }
            }
            return -1;
        }

        private static string GetContentType(string extension)
        {
            string type;
            if (ContentTypes.TryGetValue(extension, out type))
            {
                return type;
            }
            return "application/octet-stream";
        }

        private static Dictionary<string, string> CreateContentTypes()
        {
            Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            map[".json"] = "application/json; charset=utf-8";
            map[".png"] = "image/png";
            map[".jpg"] = "image/jpeg";
            map[".jpeg"] = "image/jpeg";
            map[".gif"] = "image/gif";
            map[".txt"] = "text/plain; charset=utf-8";
            map[".html"] = "text/html; charset=utf-8";
            map[".htm"] = "text/html; charset=utf-8";
            return map;
        }

        private static string ReasonPhrase(int status)
        {
            switch (status)
            {
                case 200: return "OK";
                case 206: return "Partial Content";
                case 400: return "Bad Request";
                case 403: return "Forbidden";
                case 404: return "Not Found";
                case 405: return "Method Not Allowed";
                case 411: return "Length Required";
                case 413: return "Payload Too Large";
                case 416: return "Range Not Satisfiable";
                case 431: return "Request Header Fields Too Large";
                case 500: return "Internal Server Error";
                case 503: return "Service Unavailable";
                default: return "Unknown";
            }
        }

        private static string ShortMessage(Exception ex)
        {
            Exception inner = ex.InnerException;
            return inner != null ? inner.Message : ex.Message;
        }
    }
}
