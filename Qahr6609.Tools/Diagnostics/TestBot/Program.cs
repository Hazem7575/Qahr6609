// TestBot - headless protocol test client for the Qahr6609 server (official client 6609).
// Read-only tool: it never touches Qahr6609.GameServer\ or the databases, it only talks to the running servers
// like a client would. The server code is the spec; every layout below cites the handler it mirrors.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TestBot.Crypto;

namespace TestBot
{
    class Options
    {
        public string User, Pass;
        public string Server = "MeGa";                 // AuthConfig.ini ServerN name
        public string AuthHost = "127.0.0.1";
        public int AuthPort = 9961;
        public string GameHost = "127.0.0.1";          // overrides the IP of the 1055 reply (it holds the public IP)
        public int GamePort = 0;                       // 0 = use the port of the 1055 reply
        public string LoginKey = "";                   // Program.ServerConfig.LogginKey (first 16 bytes used); empty = --loginkey, testaccount.ini loginkey= or HazemConfig.ini [Security] LoginKey
        public ushort DllVersion = 7941;               // MeGaShield.RequiredDllVersion
        public ushort ClientVersion = 6609;
        public string TqSerial = "TESTBOT00001";
        public string Mac = "TESTBOT";
        public string Mode = "probe";                  // auth | dh | probe | login
        public int ReplyWaitMs = 2000;
        public string Report;
        public string ScenarioFile;
        public uint NpcId;                             // 0 = first NPC seen (2030) during login
        public bool SkipNpc;
        public bool Create;                            // allow 1001 character creation when the server answers NEW_ROLE
        public string CreateName;                      // null = "TestBot" + 3 random digits
        public byte CreateClass = 2;                   // MsgNewRole class INDEX: 0/1 Taoist 100, 2/3 Trojan 10, 4/5 Archer 40, 6/7 Warrior 20, 8/9 Ninja 50, 10/11 Monk 60, 12/13 Pirate 70, 14/15 DragonWarrior 80, 16-19 WindWalker 160
        public ushort CreateBody = 1003;               // MsgNewRole.ExitBody: 1003/1004 male, 2001/2002 female
        public string AccountFile;                     // testaccount.ini (user=/pass=) when --user/--pass are not given
        public int Hands = 20;                         // poker: hands to play
        public uint Field = 1171;                      // poker: texas_match_type.dat quick-match id (1171 = Silver No Limit, big blind 10K)
        public int Seed;                               // poker: random seed (0 = from the clock)
        public string Tag = "";
        public int AfkPct;
        public uint DirectUid;                         // champ tests: skip the account server, log in as this EntityID (sandbox with VerifyLoginTokens=0)
        public uint Session;                           // champ: session id to sign up for (0 = first open row of the Today list)
        public int ChampRebuy;                         // champ: % chance to ask for a rebuy/add-on after each hand
        public int ChampLeave;                         // champ: leave the tournament (act 23) after this many hands (0 = never)
        public int QuitInHand;                         // poker: drop the connection at my first turn of this hand (mid-hand disconnect test)                             // poker: % of turns left unanswered (the server must time them out)                        // line prefix (pokerduo runs two bots in one process)
    }

    class Rx
    {
        public DateTime At;
        public ushort Type;
        public int Size;
        public byte[] Data;
    }

    static class Names
    {
        static readonly Dictionary<ushort, string> map = new Dictionary<ushort, string>();
        public static void Load(string gamePacketsCs)
        {
            try
            {
                if (!File.Exists(gamePacketsCs)) return;
                foreach (Match m in Regex.Matches(File.ReadAllText(gamePacketsCs), @"(\w+)\s*=\s*(\d+)\s*,?"))
                {
                    ushort id;
                    if (!ushort.TryParse(m.Groups[2].Value, out id)) continue;
                    string n;
                    map[id] = map.TryGetValue(id, out n) ? (n.Split('/').Contains(m.Groups[1].Value) ? n : n + "/" + m.Groups[1].Value) : m.Groups[1].Value;
                }
                map[10050] = "CMsgShield"; map[10055] = "CMsgRequestLogin"; map[10057] = "MsgSendPCInfo";
            }
            catch { }
        }
        public static string Of(ushort id) { string n; return map.TryGetValue(id, out n) ? n : "?"; }
    }

    partial class Program
    {
        static Options O = new Options();
        static StringBuilder Log = new StringBuilder();
        static void P(string s) { lock (Log) { Console.WriteLine(O.Tag + s); Log.AppendLine(s); } }

        static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--selftest") return SelfTest();
            if (args.Contains("pokerduo")) return PokerDuo(args);
            if (args.Contains("champmulti")) return ChampMulti(args);
            return Main2(args);
        }
        static int Main2(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i], v = i + 1 < args.Length ? args[i + 1] : null;
                switch (a)
                {
                    case "--user": O.User = v; i++; break;
                    case "--pass": O.Pass = v; i++; break;
                    case "--server": O.Server = v; i++; break;
                    case "--auth": { var p = v.Split(':'); O.AuthHost = p[0]; if (p.Length > 1) O.AuthPort = int.Parse(p[1]); i++; break; }
                    case "--game-host": O.GameHost = v; i++; break;
                    case "--game-port": O.GamePort = int.Parse(v); i++; break;
                    case "--loginkey": O.LoginKey = v; i++; break;
                    case "--dll": O.DllVersion = ushort.Parse(v); i++; break;
                    case "--serial": O.TqSerial = v; i++; break;
                    case "--mac": O.Mac = v; i++; break;
                    case "--mode": O.Mode = v; i++; break;
                    case "--wait": O.ReplyWaitMs = int.Parse(v); i++; break;
                    case "--report": O.Report = v; i++; break;
                    case "--scenario": O.ScenarioFile = v; i++; break;
                    case "--npc": O.NpcId = uint.Parse(v); i++; break;
                    case "--no-npc": O.SkipNpc = true; break;
                    case "--create": O.Create = true; break;
                    case "--name": O.CreateName = v; O.Create = true; i++; break;
                    case "--class": O.CreateClass = byte.Parse(v); i++; break;
                    case "--body": O.CreateBody = ushort.Parse(v); i++; break;
                    case "--account": O.AccountFile = v; i++; break;
                    case "--hands": O.Hands = int.Parse(v); i++; break;
                    case "--field": O.Field = uint.Parse(v); i++; break;
                    case "--seed": O.Seed = int.Parse(v); i++; break;
                    case "--tag": O.Tag = v; i++; break;
                    case "--afk": O.AfkPct = int.Parse(v); i++; break;
                    case "--quit-in-hand": O.QuitInHand = int.Parse(v); i++; break;
                    case "--direct": O.DirectUid = uint.Parse(v); i++; break;
                    case "--session": O.Session = uint.Parse(v); i++; break;
                    case "--champ-rebuy": O.ChampRebuy = int.Parse(v); i++; break;
                    case "--champ-leave": O.ChampLeave = int.Parse(v); i++; break;
                    default: Console.WriteLine("unknown arg " + a); Usage(); return 2;
                }
            }

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(O.User) || O.Pass == null)
            {
                // default credentials: testaccount.ini next to the exe (user=... / pass=...); the password is never printed
                string f = O.AccountFile ?? Path.Combine(exeDir, "testaccount.ini");
                if (File.Exists(f))
                {
                    foreach (var line in File.ReadAllLines(f))
                    {
                        int eq = line.IndexOf('='); if (eq <= 0) continue;
                        string k = line.Substring(0, eq).Trim().ToLowerInvariant(), val = line.Substring(eq + 1).Trim();
                        if (k == "user" && string.IsNullOrEmpty(O.User)) O.User = val;
                        else if ((k == "pass" || k == "password") && O.Pass == null) O.Pass = val;
                    }
                    Console.WriteLine("credentials from " + f);
                }
            }
            if (O.DirectUid != 0 && string.IsNullOrEmpty(O.User)) { O.User = "direct-" + O.DirectUid; O.Pass = ""; }
            if (string.IsNullOrEmpty(O.User) || O.Pass == null) { Usage(); return 2; }
            if (O.Report == null) O.Report = Path.Combine(exeDir, "report.txt");
            Names.Load(Path.GetFullPath(Path.Combine(exeDir, @"..\..\..\Qahr6609.GameServer\Networking\Protocol\Packets\GamePackets.cs")));

            P("TestBot  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  mode=" + O.Mode + "  user=" + O.User + "  server=" + O.Server);
            int rc = 1;
            try { rc = Run(); }
            catch (Exception e) { P("FATAL: " + e); }
            try { File.WriteAllText(O.Report, Log.ToString(), Encoding.UTF8); Console.WriteLine("report: " + O.Report); } catch (Exception e) { Console.WriteLine("report write failed: " + e.Message); }
            return rc;
        }

        // Offline check of the client crypto against the server algorithms (no network).
        static int SelfTest()
        {
            bool ok = true;
            // auth: server Encrypt/Decrypt formula  b = K ^ swap(b ^ 0xAB)
            byte[] k1 = new byte[256], k2 = new byte[256]; byte a1 = 0x9D, a2 = 0x62;
            for (int i = 0; i < 256; i++) { k1[i] = a1; k2[i] = a2; a1 = (byte)((0x0F + (byte)(a1 * 0xFA)) * a1 + 0x13); a2 = (byte)((0x79 - (byte)(a2 * 0x5C)) * a2 + 0x6D); }
            Func<byte[], byte[]> server = src => { var b = (byte[])src.Clone(); for (int i = 0; i < b.Length; i++) { b[i] ^= 0xAB; b[i] = (byte)(b[i] >> 4 | b[i] << 4); b[i] ^= (byte)(k1[i & 0xFF] ^ k2[(i >> 8) & 0xFF]); } return b; };
            byte[] plain = new byte[700]; new Random(1).NextBytes(plain);
            var c = new ClientAuthCipher(); byte[] x = server(plain); c.Decrypt(x, x.Length);
            bool a = x.SequenceEqual(plain); ok &= a; Console.WriteLine("auth server->client decrypt: " + a);
            var c2 = new ClientAuthCipher(); byte[] y = (byte[])plain.Clone(); c2.Encrypt(y); y = server(y);
            a = y.SequenceEqual(plain); ok &= a; Console.WriteLine("auth client->server encrypt: " + a);
            // password: COShield.PasswordDecryption.Decrypt
            byte[] ep = EncryptPassword("secret"); var back = new byte[6];
            for (int i = 0; i < 6; i++) back[i] = (byte)(TowKey[i * 99 % 32] ^ (byte)(OwnKey[i * 44 % 32] ^ ep[i]));
            a = Encoding.Default.GetString(back) == "secret"; ok &= a; Console.WriteLine("password: " + a);
            // DH: server side (GenerateRequest/HandleResponse) vs client side (GenerateResponse)
            string P_ = "E7A69EBDF105F2A6BBDEAD7E798F76A209AD73FB466431E2E7352ED262F8C558F10BEFEA977DE9E21DCEE9B04D245F300ECCBBA03E72630556D011023F9E857F";
            var srv = new DiffieHellman(P_, "05"); string A = srv.GenerateRequest();
            var cli = new DiffieHellman(P_, "05"); string B = cli.GenerateResponse(A); srv.HandleResponse(B);
            a = srv.ToBytes().SequenceEqual(cli.ToBytes()); ok &= a; Console.WriteLine("DH shared secret: " + a);
            // CAST5 CFB64 streams: client encrypt -> server decrypt
            var e1 = new TQCast5(); e1.GenerateKey(PostProcessDHKey(cli.ToBytes()));
            var d1 = new TQCast5(); d1.GenerateKey(PostProcessDHKey(srv.ToBytes()));
            byte[] enc = new byte[plain.Length], dec = new byte[plain.Length];
            unsafe { fixed (byte* i = plain) e1.Encrypt(i, 0, enc, 0, 100); fixed (byte* i = plain) { byte[] e2 = new byte[600]; e1.Encrypt(i + 100, 0, e2, 0, 600); Buffer.BlockCopy(e2, 0, enc, 100, 600); } fixed (byte* o = dec) d1.Decrypt(enc, 0, o, 0, enc.Length); }
            a = dec.SequenceEqual(plain); ok &= a; Console.WriteLine("CAST5 stream: " + a);
            Console.WriteLine(ok ? "SELFTEST OK" : "SELFTEST FAILED");
            return ok ? 0 : 1;
        }

        static void Usage()
        {
            Console.WriteLine("testbot [--user NAME --pass PASS | --account testaccount.ini] [--mode auth|dh|probe|login|sweep] [--server MeGa] [--auth 127.0.0.1:9961]");
            Console.WriteLine("        [--game-host 127.0.0.1] [--game-port N] [--dll 7941] [--wait 2000] [--scenario file.txt] [--npc UID|--no-npc] [--report path]");
            Console.WriteLine("        [--create [--name TestBot123] [--class 2] [--body 1003]]");
            Console.WriteLine("  (no --user/--pass: credentials are read from testaccount.ini next to the exe)");
            Console.WriteLine("  --create: if the account has no character (server says NEW_ROLE), create one with 1001 and continue the login");
            Console.WriteLine("  auth  : account server only (1059 seed -> 1952 -> 1055)");
            Console.WriteLine("  dh    : + game connection, DH key exchange, then disconnect (no login packet)");
            Console.WriteLine("  probe : + 10055 with a WRONG dll version: server answers with a force-close; the character is NOT loaded");
            Console.WriteLine("  login : full login, enter the world, run the scenario, disconnect (an online session of the same account gets kicked!)");
            Console.WriteLine("  poker : like login, then Instant Play Match-up (--field 1171) and play --hands 20 [--afk %] [--seed N]");
            Console.WriteLine("  pokerduo: two poker bots in one process: --user1/--pass1 (default 1/1) and testaccount.ini (or --user2/--pass2, --create2)");
            Console.WriteLine("  sweep : like login, but sends a broad list of read-only queries instead of the scenario and lists the ones with no reply");
        }

        // =================================================================== auth server
        static uint Identifier; static string FwIp; static int FwPort;

        static int Run()
        {
            if (O.DirectUid != 0)
            {
                // sandbox only: the game server verifies no login token ([Security] VerifyLoginTokens=0), so the
                // 10055 key is just the account server's encryption of the EntityID
                Identifier = COShieldEncryption.Encrypt(O.DirectUid);
                FwIp = O.GameHost; FwPort = O.GamePort;
                P("== DIRECT LOGIN as EntityID " + O.DirectUid + " (no account server)");
            }
            else if (!Auth()) return 1;
            if (O.Mode == "auth") return 0;
            return Game() ? 0 : 1;
        }

        static readonly byte[] OwnKey = { 90, 17, 149, 66, 219, 255, 251, 70, 166, 196, 18, 119, 53, 86, 200, 80, 163, 238, 137, 46, 10, 56, 158, 100, 64, 26, 88, 207, 239, 167, 123, 88 };
        static readonly byte[] TowKey = { 88, 254, 246, 138, 126, 226, 64, 156, 140, 89, 90, 28, 108, 196, 28, 62, 23, 161, 92, 192, 94, 253, 194, 74, 138, 224, 88, 171, 233, 26, 217, 179 };

        // inverse of AccountServer COShield.PasswordDecryption.Decrypt (a plain XOR, so the same operation)
        static byte[] EncryptPassword(string pw)
        {
            byte[] plain = Encoding.Default.GetBytes(pw);
            byte[] o = new byte[32];
            for (int x = 0; x < Math.Min(plain.Length, 32); x++)
                o[x] = (byte)(plain[x] ^ OwnKey[x * 44 % 32] ^ TowKey[x * 99 % 32]);
            return o;
        }

        static bool Auth()
        {
            P("");
            P("== AUTH " + O.AuthHost + ":" + O.AuthPort);

            var cipher = new ClientAuthCipher();
            using (var tcp = new TcpClient())
            {
                tcp.Connect(O.AuthHost, O.AuthPort);
                tcp.ReceiveTimeout = 8000;
                var ns = tcp.GetStream();
                // server -> client: 1059 PasswordCryptographySeed, 8 bytes, auth cipher, no seal
                byte[] seed = ReadExact(ns, 8);
                cipher.Decrypt(seed, seed.Length);
                ushort sLen = BitConverter.ToUInt16(seed, 0), sType = BitConverter.ToUInt16(seed, 2);
                P(string.Format("  rx {0} len {1} seed {2}", sType, sLen, BitConverter.ToInt32(seed, 4)));
                if (sType != 1059) { P("  FAIL: expected 1059"); return false; }

                // client -> server: 1952, exactly 312 bytes (COShield.Authentication)
                byte[] pk = new byte[312];
                W16(pk, 0, 312); W16(pk, 2, 1952);
                WStr(pk, 8, O.User, 16);
                WStr(pk, 55, O.TqSerial, 12);                  // ProSerial
                byte[] pw = Encoding.Default.GetBytes(O.Pass);
                pk[72] = (byte)Math.Min(pw.Length, 32);
                Buffer.BlockCopy(EncryptPassword(O.Pass), 0, pk, 73, 32);
                WStr(pk, 136, O.Server, 16);
                WStr(pk, 152, O.Mac, 16);
                cipher.Encrypt(pk);
                ns.Write(pk, 0, pk.Length);
                P("  tx 1952 len 312 (user, encrypted password, server '" + O.Server + "')");

                // server -> client: 1055 Forward, 52 bytes
                byte[] fw = ReadExact(ns, 52);
                cipher.Decrypt(fw, fw.Length);
                ushort fType = BitConverter.ToUInt16(fw, 2);
                uint type = BitConverter.ToUInt32(fw, 8);
                Identifier = BitConverter.ToUInt32(fw, 4);
                FwPort = BitConverter.ToUInt16(fw, 12);
                FwIp = Encoding.Default.GetString(fw, 20, 16).TrimEnd('\0');
                P(string.Format("  rx {0} len {1} type {2} identifier 0x{3:X8} ip '{4}' port {5}", fType, BitConverter.ToUInt16(fw, 0), type, Identifier, FwIp, FwPort));
                if (fType != 1055 || type != 2)
                {
                    P("  AUTH FAILED (Forward type " + type + ": 1=invalid info/locked, 25=banned, 73=protocol). Not retrying (LoginGuard).");
                    return false;
                }
                P("  AUTH OK -> EntityID " + COShieldEncryption.Decrypt(Identifier) + " (decrypted identifier)");
            }
            return true;
        }

        // =================================================================== game server
        static TcpClient G; static NetworkStream GS;
        static TQCast5 Cast;
        static readonly object CastSend = new object();
        static readonly List<Rx> Inbox = new List<Rx>();
        static volatile bool Closed; static string CloseReason;
        static Thread RxThread;
        static long RawBytes;
        static uint Uid; static uint MapId; static ushort X, Y;
        static readonly List<uint> SeenNpcs = new List<uint>();

        // LoginKey when --loginkey is not given: testaccount.ini (loginkey=) next to the exe, else the server's
        // Qahr6609.Shared\HazemConfig.ini [Security] LoginKey= (never printed). Empty = not found.
        static string FindLoginKey()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] files = { O.AccountFile ?? Path.Combine(exeDir, "testaccount.ini"),
                               Path.GetFullPath(Path.Combine(exeDir, @"..\..\..\Qahr6609.Shared\HazemConfig.ini")) };
            foreach (string f in files)
            {
                try
                {
                    if (!File.Exists(f)) continue;
                    foreach (var line in File.ReadAllLines(f))
                    {
                        int eq = line.IndexOf('='); if (eq <= 0) continue;
                        if (line.Substring(0, eq).Trim().ToLowerInvariant() != "loginkey") continue;
                        string val = line.Substring(eq + 1);
                        int sc = val.IndexOf(';'); if (sc >= 0) val = val.Substring(0, sc);
                        val = val.Trim();
                        if (val.Length > 0) return val;
                    }
                }
                catch { }
            }
            return "";
        }

        static bool Game()
        {
            string host = O.GameHost ?? FwIp;
            int port = O.GamePort != 0 ? O.GamePort : FwPort;
            P("");
            P("== GAME " + host + ":" + port + "  (1055 said " + FwIp + ":" + FwPort + ")");
            Cast = new TQCast5();
            if (string.IsNullOrEmpty(O.LoginKey)) O.LoginKey = FindLoginKey();
            if (string.IsNullOrEmpty(O.LoginKey)) { P("no LoginKey: pass --loginkey, add loginkey= to testaccount.ini or set [Security] LoginKey in Qahr6609.Shared\\HazemConfig.ini"); return false; }
            Cast.GenerateKey(Encoding.ASCII.GetBytes(O.LoginKey));
            G = new TcpClient(); G.NoDelay = true;
            G.Connect(host, port);
            G.ReceiveTimeout = 8000;
            GS = G.GetStream();

            // ---- server handshake (MsgHandShake.Handshake), CAST5 with the login key
            byte[] head = ReadExact(GS, 15);
            DecryptInPlace(head);
            int rest = BitConverter.ToInt32(head, 11);            // total - 11
            if (rest <= 4 || rest > 4096) { P("  FAIL: bad handshake size " + rest); return false; }
            byte[] body = ReadExact(GS, rest - 4);
            DecryptInPlace(body);
            byte[] hs = new byte[11 + rest];
            Buffer.BlockCopy(head, 0, hs, 0, 15); Buffer.BlockCopy(body, 0, hs, 15, body.Length);
            int pos = 15;
            int junk = BitConverter.ToInt32(hs, pos); pos += 4 + junk;
            int civ = BitConverter.ToInt32(hs, pos); pos += 4 + civ;
            int siv = BitConverter.ToInt32(hs, pos); pos += 4 + siv;
            string pHex = LenStr(hs, ref pos), gHex = LenStr(hs, ref pos), pubHex = LenStr(hs, ref pos);
            string seal = Encoding.ASCII.GetString(hs, hs.Length - 8, 8);
            P(string.Format("  rx handshake {0} bytes: P {1} hex chars, G '{2}', server public key {3} hex chars, seal '{4}'", hs.Length, pHex.Length, gHex, pubHex.Length, seal));
            if (seal != "TQServer") { P("  FAIL: handshake seal / login key mismatch"); return false; }

            // ---- client reply (DHKeyExchange.GetHandshakeReplyKey): 7 pad | int len (ignored) | @11 int junkLen | junk | int keyLen | key | "TQClient"
            var dh = new DiffieHellman(pHex, gHex);
            string myPub = dh.GenerateResponse(pubHex);
            var rnd = new Random();
            byte[] junkBytes = new byte[12]; rnd.NextBytes(junkBytes);
            byte[] keyBytes = Encoding.ASCII.GetBytes(myPub);
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            byte[] pad = new byte[7]; rnd.NextBytes(pad); bw.Write(pad);
            int total = 7 + 4 + 4 + junkBytes.Length + 4 + keyBytes.Length + 8;
            bw.Write(total - 7); bw.Write(junkBytes.Length); bw.Write(junkBytes);
            bw.Write(keyBytes.Length); bw.Write(keyBytes); bw.Write(Encoding.ASCII.GetBytes("TQClient"));
            byte[] reply = ms.ToArray();
            SendRaw(reply);
            // server: HandleResponse -> PostProcessDHKey(s.getBytes()) -> GenerateKey -> Reset (both IVs back to 0)
            byte[] newKey = PostProcessDHKey(dh.ToBytes());
            Cast.GenerateKey(newKey);
            Cast.Reset();
            P("  tx DH reply " + reply.Length + " bytes; new CAST5 key '" + Encoding.ASCII.GetString(newKey, 0, 16) + "...' (IVs reset)");

            G.ReceiveTimeout = 0;
            RxThread = new Thread(RxLoop) { IsBackground = true };
            RxThread.Start();
            Thread.Sleep(400);   // the server must switch keys before the next bytes arrive (separate recv)

            if (O.Mode == "dh")
            {
                Thread.Sleep(1000);
                P(Closed ? "  server closed the connection after DH: " + CloseReason : "  DH OK (connection still open), disconnecting");
                Disconnect();
                return !Closed;
            }

            Uid = COShieldEncryption.Decrypt(Identifier);
            // ---- 10055 CMsgRequestLogin (MeGaShield.RequestLogin): key | TqSerial[12] | Version | DllVersion
            ushort dll = O.Mode == "probe" ? (ushort)1 : O.DllVersion;
            byte[] login = Pkt(10055, 24);
            W32(login, 4, Identifier); WStr(login, 8, O.TqSerial, 12); W16(login, 20, O.ClientVersion); W16(login, 22, dll);
            int mark = Mark();
            Send(login, "10055 RequestLogin dll=" + dll);
            if (O.Mode == "probe")
            {
                Thread.Sleep(3500);
                var got = Since(mark);
                P("  probe: " + got.Count + " packet(s) after 10055 with a wrong dll version: " + Summ(got));
                foreach (var r in got.Where(r => r.Type == 10050)) P("    10050 text: " + Printable(r.Data, 4));
                P("  raw bytes received after DH: " + RawBytes);
                P(Closed ? "  connection closed by server (expected after force-close): " + CloseReason : "  connection still open");
                P("  => encrypted channel after DH " + (got.Count > 0 ? "WORKS (server decoded 10055 and answered)" : "UNVERIFIED (no answer)"));
                Disconnect();
                return got.Count > 0;
            }

            // ---- 10057 MsgSendPCInfo (MeGaShield.Process): PCName[32] HDDSerial[16] HWID[64]
            byte[] pc = Pkt(10057, 116);
            WStr(pc, 4, "TESTBOT-PC", 32); WStr(pc, 36, "TESTBOTHDD", 16); WStr(pc, 52, "TESTBOT-HWID-0000", 64);
            Send(pc, "10057 PCInfo");

            // wait for the character: 1006 HeroInfo (MsgLoginHandler.LoadMap runs right after 1044),
            // or 1004 "NEW_ROLE" (MsgLoginClient.LoginHandler: AllowCreate = no Users\<uid>.ini yet -> create flow)
            WaitFor(r => r.Type == 1006 || IsMsg(r, "NEW_ROLE"), 15000);
            if (!Since(mark).Any(r => r.Type == 1006) && Since(mark).Any(r => IsMsg(r, "NEW_ROLE")))
            {
                P("  rx 1004 NEW_ROLE -> this account has no character yet");
                if (!O.Create)
                {
                    P("  character creation not allowed: run again with --create");
                    Disconnect();
                    return false;
                }
                if (!CreateCharacter(login)) { Disconnect(); return false; }
            }
            if (!WaitFor(r => r.Type == 1006, 15000))
            {
                var got = Since(mark);
                P("  no 1006 HeroInfo within 15 s. received: " + Summ(got));
                foreach (var r in got.Where(r => r.Type == 1004 || r.Type == 10050)) P("    " + r.Type + " text: " + Printable(r.Data, 4));
                Disconnect();
                return false;
            }
            P("  rx 1006 HeroInfo -> character loaded (UID " + Uid + (CreatedName != null ? ", created '" + CreatedName + "'" : "") + ")");
            { var hero = Snapshot().First(r => r.Type == 1006); P("    1006 strings: " + Printable(hero.Data, 4)); }
            Thread.Sleep(800);

            // ---- what the client sends after the hero info (MsgDataPacket handlers)
            var steps = new[] {
                new { T = (ushort)74, N = "SetLocation" }, new { T = (ushort)75, N = "Hotkeys" }, new { T = (ushort)76, N = "ConfirmAssociates" },
                new { T = (ushort)77, N = "ConfirmProficiencies" }, new { T = (ushort)78, N = "ConfirmSpells" }, new { T = (ushort)97, N = "ConfirmGuild" },
                new { T = (ushort)251, N = "AllowAnimation" }, new { T = (ushort)132, N = "CompleteLogin" } };
            P("");
            P("== LOGIN SEQUENCE (10010 actions)");
            var loginResults = new List<string>();
            foreach (var s in steps)
            {
                var res = Exchange(Action10010(s.T, Uid, 0, 0, 0, 0), "10010/" + s.T + " " + s.N);
                loginResults.Add(res);
                if (s.T == 74)
                {
                    var loc = Since(LastMark).FirstOrDefault(r => r.Type == 10010 && r.Size >= 32 && BitConverter.ToUInt16(r.Data, 24) == 74);
                    if (loc != null) { MapId = BitConverter.ToUInt32(loc.Data, 12); X = BitConverter.ToUInt16(loc.Data, 28); Y = BitConverter.ToUInt16(loc.Data, 30); P("    -> location map " + MapId + " (" + X + "," + Y + ")"); }
                }
                if (Closed) break;
            }
            Thread.Sleep(1500);
            bool inWorld = !Closed && MapId != 0;
            P("  ENTERED WORLD: " + (inWorld ? "yes (map " + MapId + " " + X + "," + Y + ")" : "NO"));

            // ---- inventory of everything the server sent during login
            P("");
            P("== SERVER -> CLIENT INVENTORY (connect .. CompleteLogin + 1.5 s)");
            Inventory(Snapshot());

            if (inWorld && O.Mode == "champ")
            {
                bool ok = ChampPlay();
                Disconnect();
                P("disconnected" + (CloseReason != null ? " (" + CloseReason + ")" : ""));
                return ok;
            }
            if (inWorld && O.Mode == "poker")
            {
                bool ok = PokerPlay();
                Disconnect();
                P("disconnected" + (CloseReason != null ? " (" + CloseReason + ")" : ""));
                return ok;
            }

            // ---- scenario
            if (inWorld)
            {
                P("");
                P("== SCENARIO (reply window " + O.ReplyWaitMs + " ms; a window can also hold unrelated server pushes)");
                var noReply = new List<string>(); var onlyBg = new List<string>(); var answered = new List<string>();
                bool sweep = O.Mode == "sweep";
                if (sweep) P("   (sweep: " + string.Join(",", Background) + " count as background, not as a reply)");
                foreach (var sc in sweep ? Sweep() : Scenario())
                {
                    if (Closed) { P("  connection closed, scenario stopped"); break; }
                    string r = Exchange(sc.Value, sc.Key);
                    var got = Since(LastMark);
                    if (got.Count == 0) noReply.Add(sc.Key);
                    else if (got.All(x => Background.Contains(x.Type))) onlyBg.Add(sc.Key);
                    else answered.Add(sc.Key + " -> " + Summ(got.Where(x => !Background.Contains(x.Type)).ToList()));
                }
                P("");
                P("== ANSWERED (" + answered.Count + ")");
                foreach (var s in answered) P("  " + s);
                P("== ONLY BACKGROUND PUSHES (" + onlyBg.Count + "): " + (onlyBg.Count == 0 ? "(none)" : string.Join("; ", onlyBg)));
                P("== NO REPLY (" + noReply.Count + "): " + (noReply.Count == 0 ? "(none)" : string.Join("; ", noReply)));
            }

            P("");
            P("== FULL INVENTORY (whole session)");
            Inventory(Snapshot());
            Disconnect();
            P("disconnected cleanly" + (CloseReason != null ? " (" + CloseReason + ")" : ""));
            return inWorld;
        }

        // server pushes that arrive anyway (other players moving/spawning, NPC/sob spawns, map items): not counted as a reply
        static readonly HashSet<ushort> Background = new HashSet<ushort> { 10005, 10014, 2030, 1109, 1101, 1022, 1012, 10050 /*shield heartbeat*/, 10017 /*periodic status update*/ };

        public static string CreatedName;

        // MsgNewRole.CreateCharacter (1001): reader @4 skips 20 -> @24 name[16] | 32 skipped -> @72 body u16 | @74 class index u8.
        // Name: Program.NameStrCheck = ^[A-Za-z0-9 arabic *~.&$]{1,16}$, no "GM"/"PM"/"SYSTEM". On success the server sends
        // a 1004 "ANSWER_OK" popup and sets CreateCharacterSucces; the client then re-sends 10055 on the same connection and
        // LoginHandler saves the character (CreateCharacte + SaveClient) and sends 1052/LoadMap -> 1006.
        static bool CreateCharacter(byte[] login)
        {
            P("");
            P("== CHARACTER CREATION (1001)");
            var rnd = new Random();
            for (int attempt = 0; attempt < 3; attempt++)
            {
                string name = attempt == 0 && O.CreateName != null ? O.CreateName : "TestBot" + rnd.Next(100, 1000);
                byte[] p = Pkt(1001, 120);
                WStr(p, 24, name, 16);
                W16(p, 72, O.CreateBody);
                p[74] = O.CreateClass;
                int m = Mark();
                Send(p, "1001 NewRole name '" + name + "' body " + O.CreateBody + " class index " + O.CreateClass);
                WaitSince(m, r => IsMsg(r, "ANSWER_OK") || IsMsg(r, "in use") || IsMsg(r, "Invalid") || IsMsg(r, "WRONG"), 8000);
                var got = Since(m);
                foreach (var r in got.Where(r => r.Type == 1004)) P("    1004 text: " + Printable(r.Data, 4));
                if (got.Any(r => IsMsg(r, "ANSWER_OK")))
                {
                    CreatedName = name;
                    P("  character '" + name + "' accepted (ANSWER_OK); re-sending 10055 to finish the login");
                    Send(login, "10055 RequestLogin (after creation)");
                    return true;
                }
                if (got.Any(r => IsMsg(r, "in use"))) { P("  name taken, trying another"); continue; }
                P("  creation refused: " + (got.Count == 0 ? "no reply" : Summ(got)));
                return false;
            }
            P("  creation failed after 3 names");
            return false;
        }

        static bool IsMsg(Rx r, string text)
        {
            if (r.Type != 1004) return false;
            return Encoding.ASCII.GetString(r.Data).IndexOf(text, StringComparison.Ordinal) >= 0;
        }

        static IEnumerable<KeyValuePair<string, byte[]>> Scenario()
        {
            var list = new List<KeyValuePair<string, byte[]>>();
            Action<string, byte[]> add = (n, b) => list.Add(new KeyValuePair<string, byte[]>(n, b));
            if (O.ScenarioFile != null)
            {
                // one packet per line: "name | hex bytes of the body after the 4-byte header | type"  e.g.  Ping | 0 | 1009
                foreach (var line in File.ReadAllLines(O.ScenarioFile))
                {
                    var t = line.Trim(); if (t.Length == 0 || t.StartsWith("#")) continue;
                    var parts = t.Split('|').Select(x => x.Trim()).ToArray();
                    ushort type = ushort.Parse(parts[2]);
                    byte[] bodyHex = parts[1].Length == 0 || parts[1] == "0" ? new byte[0] : parts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(h => Convert.ToByte(h, 16)).ToArray();
                    byte[] p = Pkt(type, 4 + bodyHex.Length); Buffer.BlockCopy(bodyHex, 0, p, 4, bodyHex.Length);
                    add(parts[0] + " (" + type + ")", p);
                }
                return list;
            }
            // 1009 MsgItemUsuagePacket.GetUsageItem: @4 timer @8 id @12 dwParam(u64) @20 action(u16) @22 timestamp @26 p2 @30 p3 @34 p4, +48+1
            { byte[] p = Pkt(1009, 88); W32(p, 4, (uint)Environment.TickCount); W16(p, 20, 27); W32(p, 22, (uint)Environment.TickCount); add("1009 Usage/Ping(27)", p); }
            // 10010 Jump(137) in place: dwParam_Lo = X, dwParam_Hi = Y (PlayerJump)
            add("10010/137 Jump in place", Action10010(137, Uid, (uint)(X | (Y << 16)), 0, X, Y));
            add("10010/75 Hotkeys (again)", Action10010(75, Uid, 0, 0, 0, 0));
            add("10010/76 ConfirmAssociates (again)", Action10010(76, Uid, 0, 0, 0, 0));
            add("10010/77 ConfirmProficiencies (again)", Action10010(77, Uid, 0, 0, 0, 0));
            add("10010/78 ConfirmSpells (again)", Action10010(78, Uid, 0, 0, 0, 0));
            add("10010/97 ConfirmGuild (again)", Action10010(97, Uid, 0, 0, 0, 0));
            // 1151 GetGenericRanking: @4 mode @8 rankType @12 registered(u16) @14 page(u16) @16 count
            { byte[] p = Pkt(1151, 20); W32(p, 4, 6); add("1151 GenericRanking mode 6 PrestigeRanks", p); }
            { byte[] p = Pkt(1151, 20); W32(p, 4, 1); W32(p, 8, 30000002); add("1151 GenericRanking mode 1 Ranking RoseFairy page 0", p); }
            { byte[] p = Pkt(1151, 20); W32(p, 4, 2); W32(p, 8, 30000002); add("1151 GenericRanking mode 2 QueryCount RoseFairy", p); }
            // 1312 GetClan: @4 mode @8 uid @12 dwparam @16 string list (count 0)
            { byte[] p = Pkt(1312, 20); W32(p, 4, 1); add("1312 Clan Info(1)", p); }
            // arena: 2209 MsgArenaInfo (no body read), 2208 Rank10, 2207 GetArenaRanking @4 type(u16) @6 page(u16)
            add("2209 ArenaInfo", Pkt(2209, 8));
            add("2208 ArenaRank10", Pkt(2208, 8));
            { byte[] p = Pkt(2207, 8); W16(p, 6, 1); add("2207 ArenaRanking Today page 1", p); }
            // 1134 GetQuestList: @4 mode(u16)=3 Review @6 count(u16)
            { byte[] p = Pkt(1134, 20); W16(p, 4, 3); add("1134 QuestList Review(3)", p); }
            // 3300 coat storage = protobuf; an empty body is ActionID 0 (no case, nothing changes)
            add("3300 CoatStorage empty protobuf (ActionID 0, no-op probe)", Pkt(3300, 4));
            // 2533 GetChiHandler: @4 uid @8 action(u16)=1 Open @10 power @11 flags (0 = Unlock, never sent)
            { byte[] p = Pkt(2533, 12); W32(p, 4, Uid); W16(p, 8, 1); add("2533 HandleChi Open(1) own uid", p); }
            // 1130 GetTitle: @4 uid @8 title(u8) @9 action(u8)=4 List
            { byte[] p = Pkt(1130, 12); W32(p, 4, Uid); p[9] = 4; add("1130 Title List(4)", p); }
            // 2064 GetNobility: @4 mode(i32) @8 uid/page(u64) +88 @104 donationType   (mode 1 = Donate, never sent)
            { byte[] p = Pkt(2064, 112); W32(p, 4, 2); add("2064 Nobility RankListen(2) page 0", p); }
            { byte[] p = Pkt(2064, 112); W32(p, 4, 4); add("2064 Nobility Information(4)", p); }
            // 1058 GuildInfo (no body read)
            add("1058 GuildInfo", Pkt(1058, 8));
            // 2031 NpcServerReplay.NpcDialog: @4 stamp @8 npcid @12 mesh(u16) @14 option @15 type @16 action @17 strlen
            if (!O.SkipNpc)
            {
                uint npc = O.NpcId;
                lock (SeenNpcs) if (npc == 0 && SeenNpcs.Count > 0) npc = SeenNpcs[0];
                if (npc != 0) { byte[] p = Pkt(2031, 20); W32(p, 4, (uint)Environment.TickCount); W32(p, 8, npc); add("2031 NPC click npc " + npc + " option 0", p); }
                else P("  (no NPC seen in view - 2031 skipped)");
            }
            return list;
        }

        // ---- sweep: read-only queries only. Layouts from the server handlers; every state-changing action (buy, donate, unlock,
        // sign-up, claim, delete, teleport) is left out on purpose, and no packet is sent shorter than the fields its handler reads.
        static IEnumerable<KeyValuePair<string, byte[]>> Sweep()
        {
            var list = new List<KeyValuePair<string, byte[]>>();
            Action<string, byte[]> add = (n, b) => list.Add(new KeyValuePair<string, byte[]>(n, b));
            // generic builder: fields as (offset, width, value)
            Func<ushort, int, ulong[], byte[]> mk = (type, len, f) =>
            {
                byte[] p = Pkt(type, len);
                for (int i = 0; i + 2 < f.Length; i += 3)
                {
                    int o = (int)f[i], w = (int)f[i + 1]; ulong v = f[i + 2];
                    for (int k = 0; k < w; k++) p[o + k] = (byte)(v >> (8 * k));
                }
                return p;
            };
            Func<ushort, byte[], byte[]> raw = (type, body) => { byte[] p = Pkt(type, 4 + body.Length); Buffer.BlockCopy(body, 0, p, 4, body.Length); return p; };

            // ---- 1151 GenericRanking (GenericRank.cs): @4 mode @8 rankType @12 registered u16 @14 page u16 @16 count. All modes = reads.
            var flower = new[] { 30000002u, 30000102u, 30000202u, 30000302u, 30000402u, 30000502u, 30000602u, 30000702u };
            var flowerN = new[] { "Rose", "Lily", "Orchid", "Tulip", "Kiss", "Love", "Tins", "Jade" };
            for (int i = 0; i < flower.Length; i++) add("1151 Ranking flower " + flowerN[i], mk(1151, 20, new ulong[] { 4, 4, 1, 8, 4, flower[i] }));
            for (uint c = 1; c <= 4; c++) add("1151 Ranking chi " + (60000000 + c), mk(1151, 20, new ulong[] { 4, 4, 1, 8, 4, 60000000 + c }));
            add("1151 Ranking InnerPower 70000000", mk(1151, 20, new ulong[] { 4, 4, 1, 8, 4, 70000000 }));
            for (uint c = 1; c <= 10; c++) add("1151 Ranking prestige " + (80000000 + c), mk(1151, 20, new ulong[] { 4, 4, 1, 8, 4, 80000000 + c }));
            add("1151 QueryCount own prestige 80000000", mk(1151, 20, new ulong[] { 4, 4, 2, 8, 4, 80000000 }));
            add("1151 QueryCount Kiss (own flower rank)", mk(1151, 20, new ulong[] { 4, 4, 2, 8, 4, 30000402 }));
            add("1151 PrestigeRanks mode 6", mk(1151, 20, new ulong[] { 4, 4, 6 }));

            // ---- arena / team arena (2207 & 2243: page must be >= 1)
            add("2209 ArenaInfo", Pkt(2209, 8));
            add("2208 ArenaRank10", Pkt(2208, 8));
            add("2207 ArenaRanking today p1", mk(2207, 8, new ulong[] { 4, 2, 0, 6, 2, 1 }));
            add("2207 ArenaRanking history p1", mk(2207, 8, new ulong[] { 4, 2, 1, 6, 2, 1 }));
            add("2242 TeamArenaMatches p0", mk(2242, 8, new ulong[] { 4, 4, 0 }));
            add("2243 TeamArenaRanking p1", mk(2243, 8, new ulong[] { 4, 2, 1 }));
            add("2244 TeamArenaRank10", Pkt(2244, 8));
            add("2245 TeamArenaInfo", Pkt(2245, 8));
            add("2247 TeamArenaInfoPlayers own uid", mk(2247, 12, new ulong[] { 4, 4, 0, 8, 4, Uid }));

            // ---- elite PK (brackets 24 bytes: @4 mode u16 @6 page @8 unk @10 matchCount u32 @14 group u16 @16 gui @18 time @20 total)
            for (uint g = 0; g < 4; g++) add("2219 ElitePk brackets RequestInformation(4) group " + g, mk(2219, 24, new ulong[] { 4, 2, 4, 14, 2, g }));
            add("2219 ElitePk brackets InitialList(0) group 0", mk(2219, 24, new ulong[] { 4, 2, 0 }));
            add("2223 EliteRanks type 0 group 0", mk(2223, 12, new ulong[] { 4, 4, 0, 8, 4, 0 }));
            add("2232 TeamElitePk brackets mode 4", mk(2232, 24, new ulong[] { 4, 2, 4 }));
            add("2252 SkillElitePk brackets mode 4", mk(2252, 24, new ulong[] { 4, 2, 4 }));
            add("2233 TeamElitePkTop group 0", mk(2233, 12, new ulong[] { 8, 4, 0 }));
            add("2253 SkillElitePkTop group 0", mk(2253, 12, new ulong[] { 8, 4, 0 }));
            // ---- capture the flag (1063: @4 mode u16 @6 dw @10 page): 0 RewardRanking, 9 ArenaGui (3/4/5 move guild funds - never)
            add("1063 CTF RewardRanking(0) p1", mk(1063, 28, new ulong[] { 4, 2, 0, 10, 4, 1 }));
            add("1063 CTF ArenaGui(9)", mk(1063, 28, new ulong[] { 4, 2, 9, 10, 4, 1 }));

            // ---- guild (most need a guild: no reply expected for a guildless character)
            add("1058 GuildInfo", Pkt(1058, 8));
            add("1107 Guild Show(12)", mk(1107, 28, new ulong[] { 4, 4, 12 }));
            add("1107 Guild InfoName(6) guild 1", mk(1107, 28, new ulong[] { 4, 4, 6, 8, 4, 1 }));
            add("2101 GuildRanks type 0 p0", mk(2101, 12, new ulong[] { 4, 2, 0 }));
            add("2102 GuildMembers mode 0 p0", mk(2102, 12, new ulong[] { 4, 4, 0, 8, 4, 0 }));
            add("2203 Arsenal Show(4)", mk(2203, 20, new ulong[] { 4, 4, 4 }));
            // ---- clan / family occupy
            add("1312 Clan MyClan(29)", mk(1312, 20, new ulong[] { 4, 4, 29, 8, 4, Uid }));
            add("1312 Clan Members(4)", mk(1312, 20, new ulong[] { 4, 4, 4, 8, 4, Uid }));
            add("1313 FamilyOccupy info(6) npc 101905", mk(1313, 16, new ulong[] { 4, 4, 6, 12, 4, 101905 }));
            // ---- union / league (2622: @4 action u16 @6 ID @10 UID @14 index @18 strCount)
            add("2622 League List(4)", mk(2622, 20, new ulong[] { 4, 2, 4 }));
            add("2622 League Info(0)", mk(2622, 20, new ulong[] { 4, 2, 0 }));
            add("2622 League MyUnion(5)", mk(2622, 20, new ulong[] { 4, 2, 5 }));
            add("2622 League KingdomRank(15)", mk(2622, 20, new ulong[] { 4, 2, 15 }));
            add("2622 League ImperialGuards(23) p1", mk(2622, 20, new ulong[] { 4, 2, 23, 14, 4, 1 }));
            add("2622 League Officials(34)", mk(2622, 20, new ulong[] { 4, 2, 34 }));
            add("2622 League Tokens(37)", mk(2622, 20, new ulong[] { 4, 2, 37 }));
            add("2622 League ImperialHarem(40)", mk(2622, 20, new ulong[] { 4, 2, 40 }));
            add("2622 League loot target list(60)", mk(2622, 20, new ulong[] { 4, 2, 60 }));
            add("2642 LeagueRobOpt loot list (pb type 2 param 7001)", raw(2642, new byte[] { 0x08, 0x02, 0x10, 0xD9, 0x36 }));
            add("2642 LeagueRobOpt reinforce list (pb type 3 param 7001)", raw(2642, new byte[] { 0x08, 0x03, 0x10, 0xD9, 0x36 }));
            // ---- nobility (never action 1 = donate)
            for (uint pg = 0; pg < 2; pg++) add("2064 Nobility RankList(2) p" + pg, mk(2064, 108, new ulong[] { 4, 4, 2, 8, 8, pg }));
            add("2064 Nobility Info(4)", mk(2064, 108, new ulong[] { 4, 4, 4 }));
            // ---- achievement view of a player in sight (CheakView), own uid as fallback
            add("1136 Achievement CheakView(1) own uid", mk(1136, 16, new ulong[] { 4, 4, 1, 8, 4, Uid }));
            // ---- quests / titles / chi / jianghu / inner power / retreat / subclass (never short packets: missing fields read as 0)
            add("1134 QuestList Review(3)", mk(1134, 20, new ulong[] { 4, 2, 3 }));
            add("1130 Title List(4)", mk(1130, 12, new ulong[] { 4, 4, Uid, 9, 1, 4 }));
            add("2533 HandleChi Open(1) own uid", mk(2533, 12, new ulong[] { 4, 4, Uid, 8, 2, 1 }));
            {   // 2700 JiangHu ViewJiang(9): @4 mode u8 @5 count u8 @6 strlen u8 @7 ascii uid
                byte[] s = Encoding.ASCII.GetBytes(Uid.ToString());
                byte[] p = Pkt(2700, 7 + s.Length); p[4] = 9; p[5] = 1; p[6] = (byte)s.Length; Buffer.BlockCopy(s, 0, p, 7, s.Length);
                add("2700 JiangHu ViewJiang(9) own uid", p);
            }
            add("2703 JiangHuRank page 1", mk(2703, 8, new ulong[] { 4, 1, 1 }));
            add("2610 InnerPower InfoStage(0) stage 1", mk(2610, 13, new ulong[] { 4, 1, 0, 5, 4, Uid, 9, 4, 1 }));
            add("2610 InnerPower OpenInner(5)", mk(2610, 13, new ulong[] { 4, 1, 5, 5, 4, Uid }));
            add("2536 ChiRetreat Info(0)", mk(2536, 8, new ulong[] { 4, 2, 0, 6, 1, 0 }));
            add("2320 SubClass Open(6)", mk(2320, 12, new ulong[] { 4, 4, (uint)Environment.TickCount, 8, 2, 6 }));
            // ---- activity / events tab / mail
            add("2822 ActivityClaims resync (chest 0 = no claim)", mk(2822, 8, new ulong[] { 4, 1, 0 }));
            {   // 1315 OperatingAct action 0 (query) with the 9 ids 273-281 (= events_scenario.txt EventsQuery; never action 2 = claim)
                byte[] p = Pkt(1315, 14 + 9 * 4); W16(p, 8, 9);
                for (int i = 0; i < 9; i++) W32(p, 14 + i * 4, (uint)(273 + i));
                add("1315 OperatingAct query(0) ids 273-281", p);
            }
            add("1046 MailList page 0", mk(1046, 8, new ulong[] { 4, 4, 0 }));
            // ---- protobuf queries (body right after the 4-byte header)
            add("3253 UserAbilityScore own uid (pb f2)", raw(3253, PbVarints(1, 0, 2, Uid)));
            add("3255 ItemRefineRecord (empty pb)", Pkt(3255, 4));
            for (uint a = 0; a <= 3; a++) add("3256 EquipRefineRank action " + a, raw(3256, PbVarints(1, a, 3, 0, 4, 1)));
            add("3257 BestOfTheWorld Show(0)", Pkt(3257, 4));
            add("3283 GlobalLotteryRankList page 0", raw(3283, PbVarints(1, 0)));
            add("3282 GlobalLottery Show(5)", raw(3282, PbVarints(1, 5)));
            add("2600 GoldLeaguePoint", Pkt(2600, 4));
            // Texas (texas_scenario.txt without InstantPlay: f1=0 teleports to the Poker Room)
            add("3400 TexasEx login(12) no-op", raw(3400, new byte[] { 0x08, 0x0C }));
            add("3400 TexasEx Tournaments(2) page 1", raw(3400, new byte[] { 0x08, 0x02, 0x10, 0x01 }));
            // ---- 10010 pure queries on own uid (dwParam = target)
            add("10010/310 QuerySpawn own uid", Action10010(310, Uid, Uid, 0, 0, 0));
            add("10010/148 ViewFriendInfo own uid", Action10010(148, Uid, Uid, 0, 0, 0));
            add("10010/152 TradePartnerInfo own uid", Action10010(152, Uid, Uid, 0, 0, 0));
            add("1009 Usage/Ping(27)", mk(1009, 88, new ulong[] { 4, 4, (uint)Environment.TickCount, 20, 2, 27, 22, 4, (uint)Environment.TickCount }));
            return list;
        }

        // protobuf body of varint fields: PbVarints(field, value, field, value, ...)
        static byte[] PbVarints(params ulong[] fv)
        {
            var o = new List<byte>();
            for (int i = 0; i + 1 < fv.Length; i += 2)
            {
                o.Add((byte)(fv[i] << 3));
                ulong v = fv[i + 1];
                do { byte b = (byte)(v & 0x7F); v >>= 7; if (v != 0) b |= 0x80; o.Add(b); } while (v != 0);
            }
            return o.ToArray();
        }

        // 10010 ActionQuery (Pack=1): @4 stamp @8 ObjId @12 dwParam @16 dwParam2 @20 Timestamp @24 Type @26 Facing @28 wParam1 @30 wParam2 @32 p3 @36 p4 @40 wParam5
        static byte[] Action10010(ushort type, uint objId, uint dwParam, uint dwParam2, ushort w1, ushort w2)
        {
            byte[] p = Pkt(10010, 42);
            uint now = (uint)Environment.TickCount;
            W32(p, 4, now); W32(p, 8, objId); W32(p, 12, dwParam); W32(p, 16, dwParam2); W32(p, 20, now);
            W16(p, 24, type); W16(p, 28, w1); W16(p, 30, w2);
            return p;
        }

        // =================================================================== transport
        static void DecryptInPlace(byte[] b)
        {
            unsafe { fixed (byte* o = b) Cast.Decrypt(b, 0, o, 0, b.Length); }
        }

        static void SendRaw(byte[] plain)
        {
            byte[] enc = new byte[plain.Length];
            lock (CastSend)
            {
                unsafe { fixed (byte* i = plain) Cast.Encrypt(i, 0, enc, 0, plain.Length); }
                GS.Write(enc, 0, enc.Length);
            }
        }

        // a game packet: header length excludes the 8-byte seal; the server reads ReadHead() + 8 (SecuritySocket.HandlerBuffer)
        static byte[] Pkt(ushort type, int len)
        {
            byte[] p = new byte[len];
            W16(p, 0, (ushort)len); W16(p, 2, type);
            return p;
        }

        static void Send(byte[] p, string what)
        {
            byte[] full = new byte[p.Length + 8];
            Buffer.BlockCopy(p, 0, full, 0, p.Length);
            Encoding.ASCII.GetBytes("TQClient").CopyTo(full, p.Length);
            SendRaw(full);
            P("  tx " + what + " (" + p.Length + "+8 bytes)");
        }

        static int LastMark;
        static string Exchange(byte[] p, string what)
        {
            LastMark = Mark();
            Send(p, what);
            Thread.Sleep(O.ReplyWaitMs);
            var got = Since(LastMark);
            string s = got.Count == 0 ? "no reply" : Summ(got);
            P("     <- " + s);
            return s;
        }

        static void RxLoop()
        {
            var buf = new List<byte>();
            byte[] tmp = new byte[65536];
            try
            {
                while (true)
                {
                    int n;
                    try { n = GS.Read(tmp, 0, tmp.Length); }
                    catch (IOException e) { if (e.InnerException is SocketException && ((SocketException)e.InnerException).SocketErrorCode == SocketError.TimedOut) continue; throw; }
                    if (n <= 0) { if (!Closed) CloseReason = "server closed the connection"; break; }
                    RawBytes += n;
                    byte[] chunk = new byte[n]; Buffer.BlockCopy(tmp, 0, chunk, 0, n);
                    DecryptInPlace(chunk);
                    buf.AddRange(chunk);
                    while (buf.Count >= 4)
                    {
                        int len = buf[0] | (buf[1] << 8);
                        int full = len + 8;
                        if (len < 4) { CloseReason = "bad frame length " + len; return; }
                        if (buf.Count < full) break;
                        byte[] pk = buf.GetRange(0, full).ToArray();
                        buf.RemoveRange(0, full);
                        var r = new Rx { At = DateTime.Now, Type = BitConverter.ToUInt16(pk, 2), Size = len, Data = pk };
                        string sealS = Encoding.ASCII.GetString(pk, len, 8);
                        if (sealS != "TQServer") { CloseReason = "seal mismatch on " + r.Type + ": '" + sealS + "'"; }
                        if (r.Type == 2030 && len >= 12) lock (SeenNpcs) SeenNpcs.Add(BitConverter.ToUInt32(pk, 8));
                        lock (Inbox) Inbox.Add(r);
                        var hook = PacketHook;
                        if (hook != null) { try { hook(r); } catch (Exception e) { P("  hook error on " + r.Type + ": " + e.Message); } }
                    }
                }
            }
            catch (Exception e) { if (!Closed) CloseReason = "rx: " + e.Message; }
            finally { Closed = true; }
        }

        static int Mark() { lock (Inbox) return Inbox.Count; }
        static List<Rx> Since(int mark) { lock (Inbox) return Inbox.Skip(mark).ToList(); }
        static List<Rx> Snapshot() { lock (Inbox) return Inbox.ToList(); }
        static bool WaitFor(Func<Rx, bool> f, int ms)
        {
            var end = DateTime.Now.AddMilliseconds(ms);
            while (DateTime.Now < end) { lock (Inbox) if (Inbox.Any(f)) return true; if (Closed) return false; Thread.Sleep(50); }
            return false;
        }

        static bool WaitSince(int mark, Func<Rx, bool> f, int ms)
        {
            var end = DateTime.Now.AddMilliseconds(ms);
            while (DateTime.Now < end) { lock (Inbox) if (Inbox.Skip(mark).Any(f)) return true; if (Closed) return false; Thread.Sleep(50); }
            return false;
        }

        static string Summ(List<Rx> l)
        {
            return string.Join(", ", l.GroupBy(r => r.Type).Select(g => g.Key + " " + Names.Of(g.Key) + (g.Count() > 1 ? " x" + g.Count() : "") + " [" + string.Join("/", g.Select(r => r.Size).Distinct().Take(6)) + "]"));
        }

        static void Inventory(List<Rx> l)
        {
            P(string.Format("  {0,-6} {1,-26} {2,6} {3,8} {4}", "type", "name", "count", "bytes", "sizes"));
            foreach (var g in l.GroupBy(r => r.Type).OrderBy(g => g.Key))
                P(string.Format("  {0,-6} {1,-26} {2,6} {3,8} {4}", g.Key, Names.Of(g.Key), g.Count(), g.Sum(r => r.Size), string.Join(",", g.Select(r => r.Size).Distinct().OrderBy(x => x).Take(10))));
            P("  total " + l.Count + " packets, " + l.Sum(r => r.Size) + " bytes");
            var shield = l.Where(r => r.Type == 1004).Take(8).ToList();
            foreach (var r in shield) P("    1004 text: " + Printable(r.Data, 4));
        }

        static void Disconnect()
        {
            Closed = true;
            try { G.Client.Shutdown(SocketShutdown.Both); } catch { }
            try { G.Close(); } catch { }
        }

        // =================================================================== helpers
        static byte[] PostProcessDHKey(byte[] key)   // DHKeyExchange.ServerKeyExchange.PostProcessDHKey
        {
            int n = key.Length; for (int i = 0; i < key.Length; i++) if (key[i] == 0) { n = i; break; }
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                string s1 = Hex(md5.ComputeHash(key, 0, n));
                string s2 = Hex(md5.ComputeHash(Encoding.ASCII.GetBytes(s1 + s1)));
                return (s1 + s2).Select(c => (byte)c).ToArray();
            }
        }
        static string Hex(byte[] b) { var sb = new StringBuilder(); foreach (var x in b) sb.Append(x.ToString("x2")); return sb.ToString(); }

        static string LenStr(byte[] b, ref int pos) { int l = BitConverter.ToInt32(b, pos); pos += 4; string s = Encoding.ASCII.GetString(b, pos, l); pos += l; return s; }
        static byte[] ReadExact(NetworkStream s, int n)
        {
            byte[] b = new byte[n]; int got = 0;
            while (got < n) { int r = s.Read(b, got, n - got); if (r <= 0) throw new IOException("connection closed after " + got + "/" + n + " bytes"); got += r; }
            return b;
        }
        static void W16(byte[] b, int o, ushort v) { b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); }
        static void W32(byte[] b, int o, uint v) { for (int i = 0; i < 4; i++) b[o + i] = (byte)(v >> (8 * i)); }
        static void WStr(byte[] b, int o, string s, int max) { var x = Encoding.Default.GetBytes(s ?? ""); Buffer.BlockCopy(x, 0, b, o, Math.Min(x.Length, max)); }
        static string Printable(byte[] d, int from)
        {
            var sb = new StringBuilder();
            for (int i = from; i < d.Length - 8; i++) { char c = (char)d[i]; sb.Append(c >= 32 && c < 127 ? c : '.'); }
            return Regex.Replace(sb.ToString(), @"\.{3,}", "..");
        }
    }

    // AccountServer COShield.Encryption (identifier of the 1055 reply); Decrypt = MsgLoginClient.Decrypt on the game side
    static class COShieldEncryption
    {
        static readonly uint[] keys = { 0x3F2A9D1Cu, 0x5C7E8A4Bu, 0x2B1F6D39u, 0x7A4C3E91u, 0x6D3B1F72u, 0x4E2C8A5Fu, 0x1B9D3C7Au, 0x3A7F9E2Du,
            0x5E1C6B3Au, 0x6F2D7C1Bu, 0x7B4A2E5Cu, 0x2D8C3F91u, 0x4F9E1B7Au, 0x3C7D2A6Eu, 0x1A5B4E93u, 0x6E3F7D2Cu };
        public static uint Encrypt(uint value)
        {
            for (int i = 0; i < 16; i++)
            {
                value = (value ^ keys[i]) + keys[15 - i];
                value = (value << 5) | (value >> 27);
                value = value ^ (keys[i] >> 16);
                value = value + keys[(i + 3) % 16];
            }
            return value;
        }

        public static uint Decrypt(uint value)
        {
            for (int i = 15; i >= 0; --i)
            {
                value = value - keys[(i + 3) % 16];
                value = value ^ (keys[i] >> 16);
                value = (value >> 5) | (value << 27);
                value = (value - keys[15 - i]) ^ keys[i];
            }
            return value;
        }
    }
}
