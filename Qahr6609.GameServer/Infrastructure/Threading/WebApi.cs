// Website -> game server API (send CPs, give an item, read the bag). OFF BY DEFAULT.
// The PHP client is Qahr6609.Tools\Web\QahrWebApi (README_AR.md there explains it for the owner).
//
// Config: ServerDataBase\webapi.ini. Missing file or enabled=0 -> nothing listens (the server writes webapi.ini.example).
//   enabled=1  bind=127.0.0.1  port=5900  key=<base64 of 32 random bytes: console "webapi genkey">  allow_ips=127.0.0.1
//   ops=cps,giveitem,inventory  max_cps_per_request  cps_modes=add  cps_offline=0  allowed_items=  max_item_count  max_plus
//   rate_per_minute
//
// Wire (TCP, one JSON line each way):
//   request  {"v":1,"ts":<unix s>,"nonce":b64(16),"iv":b64(12),"ct":b64(AES-256-GCM(inner)),"tag":b64(16),"code":"<32 hex>"}
//   encKey  = HKDF-SHA256(key, salt="", info="qahr-webapi-enc",  32)
//   codeKey = HKDF-SHA256(key, salt="", info="qahr-webapi-code", 32)
//   code    = hex(HMAC-SHA256(codeKey, BE64(floor(ts/30)) || nonce))[0..32]   (rotating, TOTP-style; server accepts window ±1)
//   AAD     = "v1|" + ts + "|" + nonce(b64 text)
//   checks  : |now-ts| <= 30 s, code (constant time), AES-GCM tag, nonce never seen in the last 2 minutes, ip allow-list,
//             rate limit, op whitelist, limits, "ref" never used before (ServerDataBase\webapi_refs.txt).
//   response {"v":1,"ts":<server unix s>,"nonce":<request nonce>,"iv":b64(12 fresh),"ct":..,"tag":..,"code":..}
//             code = hex(HMAC-SHA256(codeKey, "srv|" || BE64(floor(ts/30)) || request nonce))[0..32]   (server proves the key)
//             AAD  = "v1r|" + ts + "|" + request nonce
//   Authentication failures get the plain line {"ok":false,"error":"unauthorized"} after 0.5 s (logged with the IP).
//   error "uncertain" = an unexpected server error in the middle of cps/giveitem: it MAY have been applied, so the ref is
//   kept as used (never delivered twice) and marked UNCERTAIN in webapi_refs.txt - check adminapi_log.txt by hand.
//   Limits: 4 connections at once, one line <= 16 KB that must arrive within 10 s, ct <= 8 KB, rate_per_minute (global,
//   failed attempts count too), nonce cache <= 100000 entries (expire after 2 min), player 1-32 printable chars.
//   inner request: {"op":"cps","player":"name|uid","mode":"add","amount":1000,"ref":"order-123"}
//                  {"op":"giveitem","player":..,"item":id,"count":n,"plus":0,"bound":false,"ref":..}
//                  {"op":"inventory","player":..}
//   The ops run through AdminApi.WebOp (AdminApi.Web.cs) = the same functions as the AdminApi/PlayerStudio ops.
// Console: webapi status | webapi genkey | webapi test (in-process crypto round trip, no socket, no player is touched).
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Conquer6609
{
    /// <summary>Outcome of one website op (filled by AdminApi.WebOp).</summary>
    public sealed class WebApiResult
    {
        public bool Ok;
        public string Code;      // "" when ok, else a short code the website sees (player_offline, no_space, ...)
        public string Detail;    // detailed text, server log only
        public JObject Data;     // ok reply of the underlying op
        public bool Uncertain;   // unexpected exception in the middle of the op: it MAY have been applied -> the ref stays used
    }

    /// <summary>webapi.ini</summary>
    public sealed class WebApiConfig
    {
        public bool Enabled;
        public IPAddress Bind = IPAddress.Loopback;
        public int Port = 5900;
        public byte[] Key;
        public List<IPAddress> AllowIps = new List<IPAddress> { IPAddress.Loopback };
        public HashSet<string> Ops = new HashSet<string>(WebApiCore.KnownOps);
        public long MaxCpsPerRequest = 100000;
        public HashSet<string> CpsModes = new HashSet<string> { "add" };
        public bool CpsOffline;
        public List<KeyValuePair<uint, uint>> AllowedItems = new List<KeyValuePair<uint, uint>>();// empty = any item
        public int MaxItemCount = 10;
        public int MaxPlus = 12;
        public int RatePerMinute = 30;
        public string Error;// why it cannot start (null = fine)

        public const int MinKeyBytes = 32;

        public static WebApiConfig Parse(IEnumerable<string> lines)
        {
            var c = new WebApiConfig();
            string keyText = null;
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                int eq = line.IndexOf('=');
                if (line.Length == 0 || line[0] == '#' || line[0] == ';' || line[0] == '[' || eq < 0) continue;
                string k = line.Substring(0, eq).Trim().ToLowerInvariant(), v = line.Substring(eq + 1).Trim();
                int i; long l; IPAddress ip;
                switch (k)
                {
                    case "enabled": c.Enabled = v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                    case "bind":
                        if (IPAddress.TryParse(v, out ip)) c.Bind = ip; else c.Error = "bind is not an IP address";
                        break;
                    case "port":
                        if (int.TryParse(v, out i) && i > 0 && i < 65536) c.Port = i; else c.Error = "bad port";
                        break;
                    case "key": keyText = v; break;
                    case "allow_ips":
                        c.AllowIps.Clear();
                        foreach (string s in v.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            if (IPAddress.TryParse(s.Trim(), out ip)) c.AllowIps.Add(Norm(ip));
                            else c.Error = "allow_ips: '" + s + "' is not an IP address";
                        break;
                    case "ops":
                        c.Ops.Clear();
                        foreach (string s in v.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            if (WebApiCore.KnownOps.Contains(s.Trim().ToLowerInvariant())) c.Ops.Add(s.Trim().ToLowerInvariant());
                        break;
                    case "max_cps_per_request":
                        if (long.TryParse(v, out l) && l >= 1 && l <= 2000000000L) c.MaxCpsPerRequest = l; else c.Error = "bad max_cps_per_request";
                        break;
                    case "cps_modes":
                        c.CpsModes.Clear();
                        foreach (string s in v.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string m = s.Trim().ToLowerInvariant();
                            if (m == "add" || m == "sub" || m == "set") c.CpsModes.Add(m);
                        }
                        break;
                    case "cps_offline": c.CpsOffline = v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                    case "allowed_items":
                        c.AllowedItems.Clear();
                        foreach (string s in v.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] p = s.Split('-');
                            uint a, b;
                            if (p.Length == 1 && uint.TryParse(p[0], out a)) c.AllowedItems.Add(new KeyValuePair<uint, uint>(a, a));
                            else if (p.Length == 2 && uint.TryParse(p[0], out a) && uint.TryParse(p[1], out b) && a <= b) c.AllowedItems.Add(new KeyValuePair<uint, uint>(a, b));
                            else c.Error = "allowed_items: bad entry '" + s + "'";
                        }
                        break;
                    case "max_item_count":
                        if (int.TryParse(v, out i) && i >= 1 && i <= 65535) c.MaxItemCount = i; else c.Error = "bad max_item_count";
                        break;
                    case "max_plus":
                        if (int.TryParse(v, out i) && i >= 0 && i <= 12) c.MaxPlus = i; else c.Error = "bad max_plus";
                        break;
                    case "rate_per_minute":
                        if (int.TryParse(v, out i) && i >= 1 && i <= 6000) c.RatePerMinute = i; else c.Error = "bad rate_per_minute";
                        break;
                }
            }
            c.Key = ParseKey(keyText);
            if (c.Key == null && c.Error == null)
                c.Error = "key must be the base64 of at least " + MinKeyBytes + " random bytes (console: webapi genkey)";
            if (c.AllowIps.Count == 0 && c.Error == null) c.Error = "allow_ips is empty";
            if (c.Ops.Count == 0 && c.Error == null) c.Error = "ops is empty";
            return c;
        }

        static byte[] ParseKey(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.IndexOf("CHANGE", StringComparison.OrdinalIgnoreCase) >= 0) return null;
            byte[] k;
            try { k = Convert.FromBase64String(text); } catch { return null; }
            if (k.Length < MinKeyBytes) return null;
            if (k.Distinct().Count() < 8) return null;// all zeros / repeated pattern = not random
            bool counting = true;// the PUBLIC test-vector key (bytes 0,1,2,...,31 - selftest.php / webapi test) is never accepted
            for (int i = 1; i < k.Length && counting; i++) counting = k[i] == (byte)(k[i - 1] + 1);
            if (counting) return null;
            return k;
        }

        public static IPAddress Norm(IPAddress ip)
        {
            return ip != null && ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip;
        }

        public bool IpAllowed(IPAddress ip)
        {
            ip = Norm(ip);
            foreach (var a in AllowIps)
                if (a.Equals(ip)) return true;
            return false;
        }

        public bool ItemAllowed(uint id)
        {
            if (AllowedItems.Count == 0) return true;
            foreach (var r in AllowedItems)
                if (id >= r.Key && id <= r.Value) return true;
            return false;
        }

        public const string Example =
@"# Website API (Qahr6609.Tools\Web\QahrWebApi). It is OFF until this file is copied to webapi.ini with enabled=1.
# Read Qahr6609.Tools\Web\QahrWebApi\README_AR.md first.
#
# enabled=1 turns it on (0 or no webapi.ini = off). Restart the server after a change.
enabled=0
# Listen address. Keep 127.0.0.1 (the website on the same machine). Another address only on a trusted private
# network, NEVER open this port to the internet (firewall).
bind=127.0.0.1
port=5900
# Shared secret: base64 of 32 random bytes. Make one with the server console command:  webapi genkey
# (or php gen_key.php). The same key goes into the website's config.php. Never share it, change it if it leaks.
key=
# Only these IPs may connect (comma list). The website server's IP.
allow_ips=127.0.0.1
# Which ops the website may use (cps, giveitem, inventory).
ops=cps,giveitem,inventory
# CPs: biggest amount in one request, allowed modes (add / sub / set), and whether an OFFLINE player can get CPs
# (written to Users\<uid>.ini with a backup; 0 = the player must be online).
max_cps_per_request=100000
cps_modes=add
cps_offline=0
# Items: allowed item ids, single ids or ranges (empty = any item). Example: allowed_items=720027,1088000-1088002
allowed_items=
max_item_count=10
max_plus=12
# Requests per minute (all clients together). More are refused.
rate_per_minute=30
";
    }

    /// <summary>The protocol (crypto, replay, refs, limits). No sockets: the listener and the self-test both feed it lines.</summary>
    public sealed class WebApiCore
    {
        public static readonly string[] KnownOps = { "cps", "giveitem", "inventory" };
        public const int Version = 1, StepSeconds = 30, MaxSkewSeconds = 30, NonceSeconds = 120;
        public const string Unauthorized = "{\"ok\":false,\"error\":\"unauthorized\"}";
        static readonly Regex RefRx = new Regex("^[A-Za-z0-9_.:\\-]{1,64}$", RegexOptions.CultureInvariant);

        public readonly WebApiConfig Config;
        readonly byte[] encKey, codeKey;
        readonly Func<string, JObject, WebApiResult> exec;
        readonly string refsPath, logPath;
        public int FailDelayMs = 500;
        public Func<long> Now = () => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        readonly object stateLock = new object();
        readonly Dictionary<string, long> nonces = new Dictionary<string, long>();// nonce b64 -> expiry
        readonly HashSet<string> refs = new HashSet<string>(StringComparer.Ordinal);
        readonly HashSet<string> pendingRefs = new HashSet<string>(StringComparer.Ordinal);
        readonly Queue<long> recent = new Queue<long>();// request times (ms) for the rate limit
        readonly object logLock = new object();
        long lastRateLog = -100000;

        /// <param name="exec">runs a checked op (op, AdminApi args) - AdminApi.WebOp on the server, a stub in the self-test</param>
        /// <param name="refsPath">webapi_refs.txt (null = in memory only)</param>
        /// <param name="logPath">webapi_log.txt (null = no log)</param>
        public WebApiCore(WebApiConfig config, Func<string, JObject, WebApiResult> exec, string refsPath, string logPath)
        {
            Config = config;
            this.exec = exec;
            this.refsPath = refsPath;
            this.logPath = logPath;
            DeriveKeys(config.Key, out encKey, out codeKey);
            if (refsPath != null && File.Exists(refsPath))
                foreach (string l in File.ReadAllLines(refsPath))
                {
                    int t = l.IndexOf('\t');
                    string r = (t >= 0 ? l.Substring(0, t) : l).Trim();
                    if (r.Length > 0) refs.Add(r);
                }
        }

        // ------------------------------------------------------------------ crypto primitives (the PHP client does the same)
        public static void DeriveKeys(byte[] key, out byte[] enc, out byte[] code)
        {
            enc = HKDF.DeriveKey(HashAlgorithmName.SHA256, key, 32, null, Encoding.ASCII.GetBytes("qahr-webapi-enc"));
            code = HKDF.DeriveKey(HashAlgorithmName.SHA256, key, 32, null, Encoding.ASCII.GetBytes("qahr-webapi-code"));
        }

        public static string Code(byte[] codeKey, long ts, byte[] nonce, bool server)
        {
            return CodeForWindow(codeKey, FloorDiv(ts, StepSeconds), nonce, server);
        }

        static long FloorDiv(long a, long b) { long q = a / b; if ((a % b != 0) && ((a < 0) != (b < 0))) q--; return q; }

        static string CodeForWindow(byte[] codeKey, long window, byte[] nonce, bool server)
        {
            var ms = new MemoryStream();
            if (server) ms.Write(Encoding.ASCII.GetBytes("srv|"), 0, 4);
            var w = new byte[8];
            for (int i = 0; i < 8; i++) w[i] = (byte)((ulong)window >> (56 - 8 * i));
            ms.Write(w, 0, 8);
            ms.Write(nonce, 0, nonce.Length);
            byte[] h;
            using (var hm = new HMACSHA256(codeKey)) h = hm.ComputeHash(ms.ToArray());
            return Convert.ToHexString(h, 0, 16).ToLowerInvariant();
        }

        public static string RequestAad(long ts, string nonceB64) { return "v1|" + ts.ToString(CultureInfo.InvariantCulture) + "|" + nonceB64; }
        public static string ResponseAad(long ts, string nonceB64) { return "v1r|" + ts.ToString(CultureInfo.InvariantCulture) + "|" + nonceB64; }

        static void Seal(byte[] encKey, byte[] iv, byte[] plain, string aad, out byte[] ct, out byte[] tag)
        {
            ct = new byte[plain.Length];
            tag = new byte[16];
            using (var g = new AesGcm(encKey, 16))
                g.Encrypt(iv, plain, ct, tag, Encoding.UTF8.GetBytes(aad));
        }

        static byte[] Open(byte[] encKey, byte[] iv, byte[] ct, byte[] tag, string aad)
        {
            var plain = new byte[ct.Length];
            try
            {
                using (var g = new AesGcm(encKey, 16))
                    g.Decrypt(iv, ct, tag, plain, Encoding.UTF8.GetBytes(aad));
                return plain;
            }
            catch (CryptographicException) { return null; }
        }

        // strict base64: must decode to exactly len bytes and re-encode to the same text (no padding/bit tricks)
        static byte[] B64(JToken t, int len)
        {
            if (t == null || t.Type != JTokenType.String) return null;
            string s = (string)t;
            if (s.Length > 4 * ((len + 2) / 3) + 4 && len > 0) return null;
            byte[] b;
            try { b = Convert.FromBase64String(s); } catch { return null; }
            if (len > 0 && b.Length != len) return null;
            if (Convert.ToBase64String(b) != s) return null;
            return b;
        }

        // ------------------------------------------------------------------ server side
        /// <summary>Set by WebApi.Stop (server shutdown): every request is refused from then on.</summary>
        public volatile bool Stopped;

        /// <summary>Handles one request line from ip; returns the reply line (without "\n"). Never throws: an unexpected
        /// error is logged (message only, no request body) and answered with the plain unauthorized line.</summary>
        public string HandleLine(string line, IPAddress ip)
        {
            try { return HandleLineCore(line, ip); }
            catch (Exception e)
            {
                Log(false, ip != null ? WebApiConfig.Norm(ip).ToString() : "-", "-", "-", "-", "server error: " + e.GetType().Name + ": " + e.Message);
                return Unauthorized;
            }
        }

        // JSON integer -> long without throwing (a 100-digit "ts" is a BigInteger in Newtonsoft and (long) would throw)
        static bool TryLong(JToken t, out long v)
        {
            v = 0;
            if (t == null || t.Type != JTokenType.Integer) return false;
            object o = ((JValue)t).Value;
            if (o is long) { v = (long)o; return true; }
            if (o is int) { v = (int)o; return true; }
            return false;// BigInteger / ulong = out of range
        }

        string HandleLineCore(string line, IPAddress ip)
        {
            string ipText = ip != null ? WebApiConfig.Norm(ip).ToString() : "-";
            long now = Now();
            if (Stopped) return "{\"ok\":false,\"error\":\"shutting_down\"}";
            if (!RateOk())
            {
                long t = Environment.TickCount64;// a flood must not fill the disk: one log line per 10 s
                if (t - Interlocked.Read(ref lastRateLog) > 10000) { Interlocked.Exchange(ref lastRateLog, t); Log(false, ipText, "-", "-", "-", "rate limit (" + Config.RatePerMinute + "/min)"); }
                return "{\"ok\":false,\"error\":\"rate_limited\"}";
            }

            // ---- envelope + authentication
            JObject env;
            try { env = JObject.Parse(line); } catch { env = null; }
            string why = null;
            long ts = 0;
            byte[] nonce = null, iv = null, ct = null, tag = null;
            string nonceB64 = null;
            long v;
            if (env == null) why = "bad json";
            else if (!TryLong(env["v"], out v) || v != Version) why = "bad version";
            else if (!TryLong(env["ts"], out ts)) why = "bad ts";
            else
            {
                nonce = B64(env["nonce"], 16);
                iv = B64(env["iv"], 12);
                tag = B64(env["tag"], 16);
                ct = B64(env["ct"], 0);
                string code = env["code"] != null && env["code"].Type == JTokenType.String ? (string)env["code"] : null;
                // no Math.Abs(now - ts): ts = long.MinValue + now would overflow it (OverflowException)
                if (ts < now - MaxSkewSeconds || ts > now + MaxSkewSeconds) why = "ts outside +-" + MaxSkewSeconds + " s (clock?)";
                else if (nonce == null || iv == null || tag == null || ct == null || ct.Length == 0 || ct.Length > 8192) why = "bad field";
                else if (code == null || code.Length != 32) why = "bad code";
                else
                {
                    nonceB64 = (string)env["nonce"];
                    byte[] given = Encoding.ASCII.GetBytes(code);
                    long w = FloorDiv(ts, StepSeconds);
                    bool match = false;
                    for (long d = -1; d <= 1; d++)// constant time: all three windows are always compared
                        match |= CryptographicOperations.FixedTimeEquals(given, Encoding.ASCII.GetBytes(CodeForWindow(codeKey, w + d, nonce, false)));
                    if (!match) why = "wrong code";
                }
            }
            byte[] plain = null;
            if (why == null)
            {
                plain = Open(encKey, iv, ct, tag, RequestAad(ts, nonceB64));
                if (plain == null) why = "decrypt failed (wrong key or tampered)";
            }
            if (why == null && !NonceFresh(nonceB64, now)) why = "replayed nonce";
            if (why != null)
            {
                if (FailDelayMs > 0) Thread.Sleep(FailDelayMs);
                Log(false, ipText, "auth", "-", "-", "DENIED: " + why);
                return Unauthorized;
            }

            // ---- authenticated: the inner request
            JObject reply;
            try { reply = Stopped ? Err("shutting_down", null) : Run(Encoding.UTF8.GetString(plain), ipText); }
            catch (Exception e)
            {
                Log(false, ipText, "?", "-", "-", "server error: " + e.GetType().Name + ": " + e.Message);
                reply = Err("failed", null);// the website never sees exception texts
            }
            return SealReply(reply, nonceB64);
        }

        string SealReply(JObject reply, string nonceB64)
        {
            long ts = Now();
            byte[] iv = RandomNumberGenerator.GetBytes(12);
            byte[] ct, tag;
            Seal(encKey, iv, Encoding.UTF8.GetBytes(reply.ToString(Formatting.None)), ResponseAad(ts, nonceB64), out ct, out tag);
            var env = new JObject
            {
                ["v"] = Version,
                ["ts"] = ts,
                ["nonce"] = nonceB64,
                ["iv"] = Convert.ToBase64String(iv),
                ["ct"] = Convert.ToBase64String(ct),
                ["tag"] = Convert.ToBase64String(tag),
                ["code"] = Code(codeKey, ts, Convert.FromBase64String(nonceB64), true),
            };
            return env.ToString(Formatting.None);
        }

        bool RateOk()
        {
            long ms = Environment.TickCount64;
            lock (stateLock)
            {
                while (recent.Count > 0 && ms - recent.Peek() > 60000) recent.Dequeue();
                if (recent.Count >= Config.RatePerMinute) return false;
                recent.Enqueue(ms);
                return true;
            }
        }

        bool NonceFresh(string nonceB64, long now)
        {
            lock (stateLock)
            {
                if (nonces.Count > 256)
                    foreach (var k in nonces.Where(kv => kv.Value < now).Select(kv => kv.Key).ToList())
                        nonces.Remove(k);
                long exp;
                if (nonces.TryGetValue(nonceB64, out exp) && exp >= now) return false;
                if (nonces.Count >= 100000) return false;// cannot happen with the rate limit; never forget a nonce early
                nonces[nonceB64] = now + NonceSeconds;
                return true;
            }
        }

        static JObject Err(string code, string reference)
        {
            var j = new JObject { ["ok"] = false, ["error"] = code };
            if (reference != null) j["ref"] = reference;
            return j;
        }

        JObject Run(string innerText, string ip)
        {
            JObject req;
            try { req = JObject.Parse(innerText); } catch { return Err("invalid_request", null); }
            string op = req["op"] != null && req["op"].Type == JTokenType.String ? ((string)req["op"]).Trim().ToLowerInvariant() : "";
            string reference = req["ref"] != null && req["ref"].Type == JTokenType.String ? (string)req["ref"] : null;
            string playerText = req["player"] != null && (req["player"].Type == JTokenType.String || req["player"].Type == JTokenType.Integer) ? req["player"].ToString().Trim() : "";

            if (!KnownOps.Contains(op) || !Config.Ops.Contains(op))
            {
                Log(false, ip, op, reference, playerText, "op not allowed");
                return Err("op_not_allowed", reference);
            }
            // player: 1-32 printable characters. Control, format (RTL override ...), line/paragraph separators, unpaired
            // surrogates and unassigned code points are refused, so the name can never break a log/refs line or an ini.
            if (playerText.Length == 0 || playerText.Length > 32 || playerText.Any(BadChar))
            {
                Log(false, ip, op, reference, playerText, "invalid player text");
                return Err("invalid_request", reference);
            }
            var args = new JObject();
            uint uid;
            if (playerText.All(char.IsDigit) && uint.TryParse(playerText, out uid) && uid > 0) args["uid"] = uid;
            else args["name"] = playerText;

            string amountText = "";
            bool needsRef = op != "inventory";
            if (op == "cps")
            {
                long amount;
                string mode = req["mode"] != null && req["mode"].Type == JTokenType.String ? ((string)req["mode"]).Trim().ToLowerInvariant() : "add";
                if (!Config.CpsModes.Contains(mode)) { Log(false, ip, op, reference, playerText, "mode '" + Clean(mode, 16) + "' not allowed"); return Err("op_not_allowed", reference); }
                if (req["amount"] == null || req["amount"].Type != JTokenType.Integer) return Err("invalid_request", reference);
                try { amount = (long)req["amount"]; } catch { return Err("invalid_request", reference); }
                if (amount < (mode == "set" ? 0 : 1) || amount > Config.MaxCpsPerRequest)
                {
                    Log(false, ip, op, reference, playerText, "amount " + amount + " outside 1-" + Config.MaxCpsPerRequest);
                    return Err("invalid_amount", reference);
                }
                args["mode"] = mode;// always explicit (AdminApi's default is "set")
                args["amount"] = amount;
                amountText = mode + " " + amount;
            }
            else if (op == "giveitem")
            {
                if (req["item"] == null || req["item"].Type != JTokenType.Integer) return Err("invalid_request", reference);
                long item, count = 1, plus = 0;
                try
                {
                    item = (long)req["item"];
                    if (req["count"] != null) { if (req["count"].Type != JTokenType.Integer) return Err("invalid_request", reference); count = (long)req["count"]; }
                    if (req["plus"] != null) { if (req["plus"].Type != JTokenType.Integer) return Err("invalid_request", reference); plus = (long)req["plus"]; }
                }
                catch { return Err("invalid_request", reference); }
                bool bound = req["bound"] != null && req["bound"].Type == JTokenType.Boolean && (bool)req["bound"];
                if (item < 1 || item > uint.MaxValue || !Config.ItemAllowed((uint)item))
                {
                    Log(false, ip, op, reference, playerText, "item " + item + " not allowed");
                    return Err("invalid_item", reference);
                }
                if (count < 1 || count > Config.MaxItemCount) return Err("invalid_amount", reference);
                if (plus < 0 || plus > Config.MaxPlus) return Err("invalid_request", reference);
                args["id"] = item;
                args["amount"] = count;
                args["plus"] = plus;
                args["bound"] = bound;
                amountText = "item " + item + " x" + count + (plus > 0 ? " +" + plus : "") + (bound ? " bound" : "");
            }

            if (needsRef)
            {
                if (reference == null || !RefRx.IsMatch(reference)) return Err("invalid_ref", null);
                lock (stateLock)
                {
                    if (refs.Contains(reference) || pendingRefs.Contains(reference))
                    {
                        Log(false, ip, op, reference, playerText, "duplicate ref");
                        return Err("duplicate_ref", reference);
                    }
                    pendingRefs.Add(reference);
                }
            }

            WebApiResult r;
            try { r = exec(op, args); }
            catch (Exception e) { r = new WebApiResult { Ok = false, Uncertain = true, Code = "uncertain", Detail = "server error: " + e.GetType().Name + ": " + e.Message }; }
            if (r == null) r = new WebApiResult { Ok = false, Uncertain = true, Code = "uncertain", Detail = "no result" };
            if (needsRef)
                lock (stateLock)
                {
                    pendingRefs.Remove(reference);
                    // done, or maybe done: the ref can never run again (no double delivery); "uncertain" = check the logs by hand
                    if (r.Ok || r.Uncertain) { refs.Add(reference); SaveRef(reference, op, playerText, amountText + (r.Ok ? "" : " UNCERTAIN - check adminapi_log.txt")); }
                }
            Log(r.Ok, ip, op, reference, playerText, (amountText.Length > 0 ? amountText + "; " : "") + (r.Ok ? "" : r.Code + ": ") + r.Detail);
            if (!r.Ok) return Err(string.IsNullOrEmpty(r.Code) ? "failed" : r.Code, reference);

            var ok = new JObject { ["ok"] = true, ["op"] = op };
            if (reference != null) ok["ref"] = reference;
            if (r.Data != null)// only these fields go to the website (never ip, backup file names, ...)
                foreach (var p in r.Data.Properties())
                    if (ReplyFields.Contains(p.Name) && ok[p.Name] == null) ok[p.Name] = p.Value;
            return ok;
        }

        static readonly HashSet<string> ReplyFields = new HashSet<string>(StringComparer.Ordinal)
        { "message", "echo", "old", "new", "online", "uid", "name", "level", "cps", "inventory", "inventoryMax", "equipment" };

        static bool BadChar(char ch)
        {
            switch (char.GetUnicodeCategory(ch))
            {
                case UnicodeCategory.Control:
                case UnicodeCategory.Format:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.Surrogate:
                case UnicodeCategory.PrivateUse:
                case UnicodeCategory.OtherNotAssigned:
                    return true;
            }
            return ch == '|' || ch == '=' || ch == '[' || ch == ']'// log/refs separator, ini syntax
                || ch == '�';// replacement char = the JSON had a broken surrogate / invalid UTF-8
        }

        void SaveRef(string reference, string op, string player, string amount)
        {
            if (refsPath == null) return;
            try
            {
                File.AppendAllText(refsPath, reference + "\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "\t" + op + "\t" + Clean(player) + "\t" + amount + Environment.NewLine);
            }
            catch (Exception e) { Log(false, "-", op, reference, player, "could not write webapi_refs.txt: " + e.Message); }
        }

        // one log/refs field: no CR/LF/tab/other control, no Unicode line separators or bidi overrides, no '|', at most max chars
        static string Clean(string s, int max = 300)
        {
            s = s ?? "";
            if (s.Length > max) s = s.Substring(0, max) + "...";
            return new string(s.Select(ch => BadChar(ch) && ch != '=' && ch != '[' && ch != ']' ? '_' : ch).ToArray());
        }

        void Log(bool ok, string ip, string op, string reference, string player, string details)
        {
            string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | " + (ok ? "OK  " : "FAIL") + " | ip " + ip + " | " + Clean(op, 32) +
                          " | ref " + Clean(reference ?? "-", 64) + " | player " + Clean(player ?? "-", 40) + " | " + Clean(details) + Environment.NewLine;
            if (logPath == null) return;
            try { lock (logLock) File.AppendAllText(logPath, line); } catch { }
        }

        // ------------------------------------------------------------------ client side (self-test; QahrApi.php does exactly this)
        public static string ClientRequest(byte[] key, string innerJson, long ts, out string nonceB64)
        {
            byte[] enc, code;
            DeriveKeys(key, out enc, out code);
            byte[] nonce = RandomNumberGenerator.GetBytes(16), iv = RandomNumberGenerator.GetBytes(12);
            return ClientRequestFixed(enc, code, innerJson, ts, nonce, iv, out nonceB64);
        }

        public static string ClientRequestFixed(byte[] enc, byte[] code, string innerJson, long ts, byte[] nonce, byte[] iv, out string nonceB64)
        {
            nonceB64 = Convert.ToBase64String(nonce);
            byte[] ct, tag;
            Seal(enc, iv, Encoding.UTF8.GetBytes(innerJson), RequestAad(ts, nonceB64), out ct, out tag);
            return new JObject
            {
                ["v"] = Version,
                ["ts"] = ts,
                ["nonce"] = nonceB64,
                ["iv"] = Convert.ToBase64String(iv),
                ["ct"] = Convert.ToBase64String(ct),
                ["tag"] = Convert.ToBase64String(tag),
                ["code"] = Code(code, ts, nonce, false),
            }.ToString(Formatting.None);
        }

        /// <summary>Verifies and decrypts a reply; null = not authentic (or a plain unauthenticated error line).</summary>
        public static JObject ClientOpen(byte[] key, string line, string nonceB64, long now)
        {
            byte[] enc, code;
            DeriveKeys(key, out enc, out code);
            JObject env;
            try { env = JObject.Parse(line); } catch { return null; }
            if (env["ct"] == null || env["ts"] == null || env["ts"].Type != JTokenType.Integer) return null;
            long ts = (long)env["ts"];
            if (Math.Abs(now - ts) > 2 * MaxSkewSeconds) return null;
            if ((string)env["nonce"] != nonceB64) return null;
            byte[] iv = B64(env["iv"], 12), tag = B64(env["tag"], 16), ct = B64(env["ct"], 0);
            if (iv == null || tag == null || ct == null) return null;
            string want = Code(code, ts, Convert.FromBase64String(nonceB64), true);
            string got = env["code"] != null ? (string)env["code"] : "";
            if (!CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(want), Encoding.ASCII.GetBytes(got ?? ""))) return null;
            byte[] plain = Open(enc, iv, ct, tag, ResponseAad(ts, nonceB64));
            if (plain == null) return null;
            try { return JObject.Parse(Encoding.UTF8.GetString(plain)); } catch { return null; }
        }

        /// <summary>Known-answer vector (also checked by QahrApi.php's selftest.php): key = bytes 0..31, ts = 1700000000,
        /// nonce = 16 x 0xA5, iv = 12 x 0x11, inner = {"op":"inventory","player":"Test"}.</summary>
        public static string VectorLine()
        {
            var key = new byte[32];
            for (int i = 0; i < 32; i++) key[i] = (byte)i;
            byte[] enc, code;
            DeriveKeys(key, out enc, out code);
            var nonce = Enumerable.Repeat((byte)0xA5, 16).ToArray();
            var iv = Enumerable.Repeat((byte)0x11, 12).ToArray();
            string n;
            return ClientRequestFixed(enc, code, "{\"op\":\"inventory\",\"player\":\"Test\"}", 1700000000, nonce, iv, out n);
        }
    }

    /// <summary>The listener + console commands. Off unless ServerDataBase\webapi.ini has enabled=1 and a valid key.</summary>
    public static class WebApi
    {
        static TcpListener listener;
        static WebApiCore core;
        const int MaxConnections = 4;// connections served at once; more are closed at once (allow-listed IPs only)
        static readonly SemaphoreSlim slots = new SemaphoreSlim(MaxConnections, MaxConnections);
        const int MaxLine = 16384;// bytes of one request line; longer = connection closed without reading on

        static string Dir { get { return Program.ServerConfig.DbLocation ?? ""; } }
        static string ConfigPath { get { return Path.Combine(Dir, "webapi.ini"); } }

        public static bool Listening { get { return listener != null; } }

        public static void Start()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    WriteExample();
                    return;// off (default)
                }
                var cfg = WebApiConfig.Parse(File.ReadAllLines(ConfigPath));
                if (!cfg.Enabled)
                {
                    WriteExample();
                    return;// off
                }
                if (cfg.Error != null)
                {
                    MyConsole.WriteLine("[WebApi] NOT started: webapi.ini: " + cfg.Error, ConsoleColor.Yellow);
                    return;
                }
                core = new WebApiCore(cfg, (op, args) => AdminApi.WebOp(op, args, cfg.CpsOffline),
                    Path.Combine(Dir, "webapi_refs.txt"), Path.Combine(Dir, "webapi_log.txt"));
                listener = new TcpListener(cfg.Bind, cfg.Port);
                listener.Start();
                new Thread(Accept) { IsBackground = true, Name = "WebApi" }.Start();
                MyConsole.WriteLine("[WebApi] listening on " + cfg.Bind + ":" + cfg.Port + " (ops " + string.Join(",", cfg.Ops) + ", allow " + string.Join(",", cfg.AllowIps) + ")",
                    IPAddress.IsLoopback(cfg.Bind) ? ConsoleColor.Green : ConsoleColor.Yellow);
                if (!IPAddress.IsLoopback(cfg.Bind))
                    MyConsole.WriteLine("[WebApi] WARNING: bind is not 127.0.0.1 - keep port " + cfg.Port + " closed to the internet (firewall).", ConsoleColor.Yellow);
            }
            catch (Exception e)
            {
                listener = null;
                MyConsole.WriteLine("[WebApi] NOT started: " + e.Message, ConsoleColor.Yellow);
            }
        }

        public static void Stop()
        {
            var l = listener;
            listener = null;
            var co = core;
            if (co != null) co.Stopped = true;// requests already read are refused from now on
            try { if (l != null) l.Stop(); } catch { }
            // wait (max 5 s) for a request that is already inside an op, so its change is in memory before the shutdown save
            int got = 0;
            var until = DateTime.UtcNow.AddSeconds(5);
            while (got < MaxConnections && DateTime.UtcNow < until)
                if (slots.Wait(200)) got++;
            if (got > 0) slots.Release(got);
        }

        static void WriteExample()
        {
            try
            {
                string f = Path.Combine(Dir, "webapi.ini.example");
                if (!File.Exists(f)) File.WriteAllText(f, WebApiConfig.Example);
            }
            catch { }
        }

        static void Accept()
        {
            while (true)
            {
                var l = listener;
                if (l == null) return;
                TcpClient c;
                try { c = l.AcceptTcpClient(); }
                catch (ObjectDisposedException) { return; }
                catch (InvalidOperationException) { return; }
                catch (SocketException)
                {
                    if (listener == null) return;// stopped
                    Thread.Sleep(100);// a reset before accept: keep listening (the listener must not die silently)
                    continue;
                }
                IPEndPoint ep;
                try { ep = c.Client.RemoteEndPoint as IPEndPoint; } catch { ep = null; }
                if (ep == null || core == null || !core.Config.IpAllowed(ep.Address)) { try { c.Close(); } catch { } continue; }// not on allow_ips: dropped
                if (!slots.Wait(0)) { try { c.Close(); } catch { } continue; }
                ThreadPool.QueueUserWorkItem(_ => { try { Serve(c, ep.Address); } finally { slots.Release(); } });
            }
        }

        static void Serve(TcpClient c, IPAddress ip)
        {
            using (c)
            {
                try
                {
                    c.ReceiveTimeout = 10000;
                    c.SendTimeout = 10000;
                    var stream = c.GetStream();
                    string line = ReadLine(c, stream, DateTime.UtcNow.AddSeconds(10));
                    if (line == null) return;
                    byte[] data = Encoding.UTF8.GetBytes(core.HandleLine(line, ip) + "\n");
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
                catch { }
            }
        }

        // slow-loris safe: the WHOLE line must arrive before the deadline (each read only waits for the time that is left)
        static string ReadLine(TcpClient c, NetworkStream s, DateTime deadline)
        {
            var buf = new MemoryStream();
            var one = new byte[1024];
            while (true)
            {
                int left = (int)(deadline - DateTime.UtcNow).TotalMilliseconds;
                if (left <= 0) return null;
                c.ReceiveTimeout = Math.Max(1, left);
                int n = s.Read(one, 0, one.Length);
                if (n <= 0) return null;
                int nl = Array.IndexOf(one, (byte)'\n', 0, n);
                buf.Write(one, 0, nl >= 0 ? nl : n);
                if (buf.Length > MaxLine) return null;
                if (nl >= 0) return Encoding.UTF8.GetString(buf.ToArray()).TrimEnd('\r');
            }
        }

        // ------------------------------------------------------------------ console: webapi status | genkey | test
        public static void Console(string[] line)
        {
            string sub = line.Length > 1 ? line[1].ToLowerInvariant() : "status";
            switch (sub)
            {
                case "genkey":
                    MyConsole.WriteLine("[WebApi] new key (put it in webapi.ini AND the website's config.php, keep it secret):", ConsoleColor.Cyan);
                    MyConsole.WriteLine("key=" + Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), ConsoleColor.Cyan);
                    break;
                case "test":
                    foreach (string l in SelfTest())
                        MyConsole.WriteLine("[WebApi test] " + l, l.StartsWith("FAIL") ? ConsoleColor.Red : ConsoleColor.Green);
                    break;
                default:
                    if (listener != null)
                        MyConsole.WriteLine("[WebApi] ON " + core.Config.Bind + ":" + core.Config.Port + "  ops " + string.Join(",", core.Config.Ops), ConsoleColor.Green);
                    else
                        MyConsole.WriteLine("[WebApi] OFF (ServerDataBase\\webapi.ini missing, enabled=0 or invalid - see webapi.ini.example)", ConsoleColor.Yellow);
                    MyConsole.WriteLine("[WebApi] webapi status | webapi genkey | webapi test");
                    break;
            }
        }

        /// <summary>In-process round trips (client encrypt -> server handle -> client decrypt) with a stub executor:
        /// no socket, no file, no player is changed. Returns "OK ..." / "FAIL ..." lines.</summary>
        public static List<string> SelfTest()
        {
            var outp = new List<string>();
            int fails = 0;
            Action<bool, string> check = (ok, what) => { outp.Add((ok ? "OK   " : "FAIL ") + what); if (!ok) fails++; };

            byte[] key = RandomNumberGenerator.GetBytes(32);
            string cfgKey = null;
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var real = WebApiConfig.Parse(File.ReadAllLines(ConfigPath));
                    if (real.Key != null) { key = real.Key; cfgKey = "the key of webapi.ini"; }
                }
            }
            catch { }
            outp.Add("using " + (cfgKey ?? "a random temporary key (webapi.ini has no valid key)"));
            var cfg = WebApiConfig.Parse(new[] { "enabled=1", "key=" + Convert.ToBase64String(key), "max_cps_per_request=5000", "rate_per_minute=1000", "allowed_items=720027,1088000-1088002" });
            var calls = new List<string>();
            var t = new WebApiCore(cfg, (op, args) =>
            {
                calls.Add(op + " " + args.ToString(Formatting.None));
                return new WebApiResult { Ok = true, Detail = "stub", Data = new JObject { ["ok"] = true, ["message"] = "stub " + op, ["echo"] = args } };
            }, null, null) { FailDelayMs = 0 };
            var ip = IPAddress.Loopback;
            long now = t.Now();
            string n;

            Func<string, JObject> round = inner =>
            {
                string req = WebApiCore.ClientRequest(key, inner, t.Now(), out n);
                return WebApiCore.ClientOpen(key, t.HandleLine(req, ip), n, t.Now());
            };

            var r = round("{\"op\":\"inventory\",\"player\":\"SomePlayer\"}");
            check(r != null && (bool)r["ok"] && (string)r["echo"]["name"] == "SomePlayer", "inventory round trip (encrypt -> handle -> verify server code -> decrypt)");
            r = round("{\"op\":\"cps\",\"player\":\"1000001\",\"mode\":\"add\",\"amount\":1000,\"ref\":\"order-1\"}");
            check(r != null && (bool)r["ok"] && (long)r["echo"]["uid"] == 1000001 && (string)r["echo"]["mode"] == "add", "cps add by uid with ref order-1");
            r = round("{\"op\":\"cps\",\"player\":\"SomePlayer\",\"mode\":\"add\",\"amount\":10,\"ref\":\"order-1\"}");
            check(r != null && (string)r["error"] == "duplicate_ref", "same ref again -> duplicate_ref");
            r = round("{\"op\":\"cps\",\"player\":\"SomePlayer\",\"mode\":\"add\",\"amount\":999999,\"ref\":\"order-2\"}");
            check(r != null && (string)r["error"] == "invalid_amount", "amount over max_cps_per_request -> invalid_amount");
            r = round("{\"op\":\"cps\",\"player\":\"SomePlayer\",\"mode\":\"set\",\"amount\":5,\"ref\":\"order-3\"}");
            check(r != null && (string)r["error"] == "op_not_allowed", "mode set (not in cps_modes) -> op_not_allowed");
            r = round("{\"op\":\"giveitem\",\"player\":\"SomePlayer\",\"item\":1088001,\"count\":2,\"ref\":\"order-4\"}");
            check(r != null && (bool)r["ok"] && (long)r["echo"]["id"] == 1088001 && (long)r["echo"]["amount"] == 2, "giveitem allowed id");
            r = round("{\"op\":\"giveitem\",\"player\":\"SomePlayer\",\"item\":1,\"count\":1,\"ref\":\"order-5\"}");
            check(r != null && (string)r["error"] == "invalid_item", "giveitem id not in allowed_items -> invalid_item");
            r = round("{\"op\":\"ban\",\"player\":\"SomePlayer\"}");
            check(r != null && (string)r["error"] == "op_not_allowed", "unknown op -> op_not_allowed");

            string req1 = WebApiCore.ClientRequest(key, "{\"op\":\"inventory\",\"player\":\"A\"}", now, out n);
            string rep1 = t.HandleLine(req1, ip);
            check(WebApiCore.ClientOpen(key, rep1, n, now) != null, "fresh request accepted");
            check(t.HandleLine(req1, ip) == WebApiCore.Unauthorized, "replayed request (same nonce) -> unauthorized");

            var env = JObject.Parse(WebApiCore.ClientRequest(key, "{\"op\":\"inventory\",\"player\":\"A\"}", now, out n));
            byte[] ct = Convert.FromBase64String((string)env["ct"]); ct[0] ^= 1; env["ct"] = Convert.ToBase64String(ct);
            check(t.HandleLine(env.ToString(Formatting.None), ip) == WebApiCore.Unauthorized, "tampered ciphertext -> unauthorized");

            env = JObject.Parse(WebApiCore.ClientRequest(key, "{\"op\":\"inventory\",\"player\":\"A\"}", now, out n));
            env["code"] = new string('0', 32);
            check(t.HandleLine(env.ToString(Formatting.None), ip) == WebApiCore.Unauthorized, "wrong rotating code -> unauthorized");

            env = JObject.Parse(WebApiCore.ClientRequest(key, "{\"op\":\"inventory\",\"player\":\"A\"}", now, out n));
            env["ts"] = now + 1;// changes AAD and window: code/tag must fail
            check(t.HandleLine(env.ToString(Formatting.None), ip) == WebApiCore.Unauthorized, "changed ts in envelope -> unauthorized");

            check(t.HandleLine(WebApiCore.ClientRequest(key, "{\"op\":\"inventory\",\"player\":\"A\"}", now - 120, out n), ip) == WebApiCore.Unauthorized, "request 2 minutes old -> unauthorized");
            check(t.HandleLine(WebApiCore.ClientRequest(RandomNumberGenerator.GetBytes(32), "{\"op\":\"inventory\",\"player\":\"A\"}", now, out n), ip) == WebApiCore.Unauthorized, "other key -> unauthorized");

            string rep = t.HandleLine(WebApiCore.ClientRequest(key, "{\"op\":\"inventory\",\"player\":\"A\"}", now, out n), ip);
            var renv = JObject.Parse(rep);
            renv["code"] = new string('f', 32);
            check(WebApiCore.ClientOpen(key, renv.ToString(Formatting.None), n, now) == null, "client rejects a reply with a wrong server code");
            check(WebApiCore.ClientOpen(RandomNumberGenerator.GetBytes(32), rep, n, now) == null, "client with another key cannot read the reply");

            check(calls.Count == 5, "only the 5 accepted requests reached the executor (" + calls.Count + ")");
            var v = JObject.Parse(WebApiCore.VectorLine());
            outp.Add("vector (selftest.php): code=" + v["code"] + " ct=" + v["ct"] + " tag=" + v["tag"]);
            outp.Add(fails == 0 ? "ALL PASSED" : "FAIL: " + fails + " check(s) failed");
            return outp;
        }
    }
}
