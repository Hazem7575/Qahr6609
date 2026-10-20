// Pipe v2 security test (docs\MULTI_SERVER.md). Build: build.bat (csc of .NET Framework 4, no dependencies).
//
//   pipetest attack    --port 19931 --secret-file f [--server-id 1] [--control-id 2] [--uid 10212287]
//        talks to a real Realm (IsInterServer=1) and checks that forged / unauthenticated / replayed traffic is refused.
//        Verdicts come from the socket (closed or not) and from the Realm's Security_Logs (pass --realm-logs <dir>).
//   pipetest fakerealm --port 19931 --secret-file f --seconds 60 [--report file]
//        listens as a Realm; a home server (admin pipe "visit <uid>") connects with a relay link; prints every frame the
//        home server sends (setup snapshot, then RelayClient envelopes with the inner client packet id), so it shows
//        which client packets home relays. It answers HeroInfo with 2501 like a Realm.
// The secret is read from a file and never printed.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

static class PipeTest
{
    static string Secret;
    static int Port = 19931;
    static uint ServerId = 1, ControlId = 2, Uid = 10212287;
    static string RealmLogs;
    static int Failures;
    static StringBuilder Report = new StringBuilder();

    static void P(string s) { Console.WriteLine(s); Report.AppendLine(s); }

    static int Main(string[] args)
    {
        string mode = args.Length > 0 ? args[0] : "";
        string report = null; int seconds = 60;
        for (int i = 1; i < args.Length - 1; i++)
        {
            switch (args[i])
            {
                case "--port": Port = int.Parse(args[++i]); break;
                case "--secret-file": Secret = File.ReadAllText(args[++i]).Trim(); break;
                case "--server-id": ServerId = uint.Parse(args[++i]); break;
                case "--control-id": ControlId = uint.Parse(args[++i]); break;
                case "--uid": Uid = uint.Parse(args[++i]); break;
                case "--realm-logs": RealmLogs = args[++i]; break;
                case "--report": report = args[++i]; break;
                case "--seconds": seconds = int.Parse(args[++i]); break;
            }
        }
        if (Secret == null) { Console.WriteLine("usage: pipetest attack|fakerealm --port N --secret-file f ..."); return 2; }
        int rc;
        if (mode == "attack") rc = Attack();
        else if (mode == "fakerealm") rc = FakeRealm(seconds);
        else { Console.WriteLine("unknown mode"); return 2; }
        if (report != null) File.WriteAllText(report, Report.ToString());
        return rc;
    }

    // ------------------------------------------------------------------ wire helpers
    static byte[] Seal = Encoding.ASCII.GetBytes("TQServer");

    static byte[] Packet(ushort id, byte[] body)
    {
        byte[] p = new byte[4 + body.Length + 8];
        int len = 4 + body.Length;
        p[0] = (byte)len; p[1] = (byte)(len >> 8); p[2] = (byte)id; p[3] = (byte)(id >> 8);
        Buffer.BlockCopy(body, 0, p, 4, body.Length);
        Buffer.BlockCopy(Seal, 0, p, 4 + body.Length, 8);
        return p;
    }

    class Link
    {
        public TcpClient Tcp; public NetworkStream S;
        public HMACSHA256 SendMac, RecvMac; public uint SendSeq, RecvSeq;
        public byte[] ServerNonce, ClientNonce;
        public byte[] LastFrame;   // for replay tests
        public bool Listener;

        public void Raw(byte[] p) { S.Write(p, 0, p.Length); }

        public byte[] Frame(byte[] inner, uint seq, bool badMac)
        {
            byte[] macIn = new byte[4 + inner.Length];
            BitConverter.GetBytes(seq).CopyTo(macIn, 0);
            Buffer.BlockCopy(inner, 0, macIn, 4, inner.Length);
            byte[] mac = SendMac.ComputeHash(macIn);
            if (badMac) mac[0] ^= 0xFF;
            int total = inner.Length + 28; int len = total - 8;
            byte[] f = new byte[total];
            f[0] = (byte)len; f[1] = (byte)(len >> 8); f[2] = 131; f[3] = 0;
            BitConverter.GetBytes(seq).CopyTo(f, 4);
            Buffer.BlockCopy(mac, 0, f, 8, 12);
            Buffer.BlockCopy(inner, 0, f, 20, inner.Length);
            Buffer.BlockCopy(Seal, 0, f, total - 8, 8);
            return f;
        }
        public void Send(byte[] inner) { byte[] f = Frame(inner, ++SendSeq, false); LastFrame = f; Raw(f); }

        /// <summary>Next whole packet, or null when the peer closed / nothing within ms.</summary>
        public byte[] Read(int ms)
        {
            Tcp.ReceiveTimeout = ms;
            try
            {
                byte[] head = ReadExact(2); if (head == null) return null;
                int len = head[0] | (head[1] << 8);
                byte[] rest = ReadExact(len + 8 - 2); if (rest == null) return null;
                byte[] p = new byte[len + 8]; head.CopyTo(p, 0); rest.CopyTo(p, 2);
                return p;
            }
            catch (IOException) { return Timeout; }
        }
        public static readonly byte[] Timeout = new byte[0];
        byte[] ReadExact(int n)
        {
            byte[] b = new byte[n]; int got = 0;
            while (got < n) { int r = S.Read(b, got, n - got); if (r <= 0) return null; got += r; }
            return b;
        }
        /// <summary>Unwraps a received frame (checks seq + MAC like the server does).</summary>
        public byte[] Unframe(byte[] f)
        {
            if (f == null || f.Length < 32 || (f[2] | (f[3] << 8)) != 131) return null;
            uint seq = BitConverter.ToUInt32(f, 4);
            if (seq != RecvSeq + 1) throw new Exception("peer sequence " + seq + " expected " + (RecvSeq + 1));
            byte[] inner = new byte[f.Length - 28];
            Buffer.BlockCopy(f, 20, inner, 0, inner.Length);
            byte[] macIn = new byte[4 + inner.Length]; Buffer.BlockCopy(f, 4, macIn, 0, 4); inner.CopyTo(macIn, 4);
            byte[] mac = RecvMac.ComputeHash(macIn);
            for (int i = 0; i < 12; i++) if (mac[i] != f[8 + i]) throw new Exception("peer MAC wrong");
            RecvSeq = seq;
            return inner;
        }
        /// <summary>True when the peer closed the connection within ms (any data received in between is skipped).</summary>
        public bool ClosedWithin(int ms)
        {
            DateTime end = DateTime.Now.AddMilliseconds(ms);
            while (DateTime.Now < end)
            {
                byte[] p = Read(Math.Max(50, (int)(end - DateTime.Now).TotalMilliseconds));
                if (p == null) return true;
                if (p == Timeout) return false;
            }
            return false;
        }
        public void Close() { try { Tcp.Close(); } catch { } }
    }

    static byte[] Hmac(byte[] key, string data) { using (var h = new HMACSHA256(key)) return h.ComputeHash(Encoding.ASCII.GetBytes(data)); }
    static string Hex(byte[] b) { return string.Concat(b.Select(x => x.ToString("x2"))); }
    static byte[] Nonce() { var b = new byte[16]; using (var r = RandomNumberGenerator.Create()) r.GetBytes(b); return b; }
    static void SetKeys(Link l, string secret)
    {
        byte[] session = Hmac(Encoding.ASCII.GetBytes(secret), "key|" + Hex(l.ServerNonce) + "|" + Hex(l.ClientNonce));
        byte[] c2s, s2c;
        using (var h = new HMACSHA256(session)) { c2s = h.ComputeHash(Encoding.ASCII.GetBytes("c2s")); s2c = h.ComputeHash(Encoding.ASCII.GetBytes("s2c")); }
        l.SendMac = new HMACSHA256(l.Listener ? s2c : c2s);
        l.RecvMac = new HMACSHA256(l.Listener ? c2s : s2c);
    }
    static byte[] AuthBody(byte link, uint serverId, byte[] cn, byte[] proof)
    {
        var b = new List<byte> { 2, link }; b.AddRange(BitConverter.GetBytes(serverId)); b.AddRange(cn); b.AddRange(proof); return b.ToArray();
    }

    // ------------------------------------------------------------------ attack mode (against a real Realm)
    static Link Connect()
    {
        var t = new TcpClient(); t.Connect("127.0.0.1", Port);
        return new Link { Tcp = t, S = t.GetStream() };
    }
    static byte[] ReadHello(Link l)
    {
        byte[] h = l.Read(3000);
        if (h == null || h == Link.Timeout || (h[2] | (h[3] << 8)) != 95) return null;
        byte[] sn = new byte[16]; Buffer.BlockCopy(h, 5, sn, 0, 16); return sn;
    }
    /// <summary>Full v2 handshake with the right secret. Returns null when the Realm refused.</summary>
    static Link Handshake(byte linkType, uint serverId, out byte[] authPacket)
    {
        authPacket = null;
        var l = Connect();
        l.ServerNonce = ReadHello(l);
        if (l.ServerNonce == null) { l.Close(); return null; }
        l.ClientNonce = Nonce();
        byte[] proof = Hmac(Encoding.ASCII.GetBytes(Secret), "pipe2|" + linkType + "|" + serverId + "|" + Hex(l.ServerNonce) + "|" + Hex(l.ClientNonce));
        authPacket = Packet(98, AuthBody(linkType, serverId, l.ClientNonce, proof));
        l.Raw(authPacket);
        byte[] ok = l.Read(3000);
        if (ok == null || ok == Link.Timeout || (ok[2] | (ok[3] << 8)) != 96) { l.Close(); return null; }
        byte[] ack = new byte[32]; Buffer.BlockCopy(ok, 4, ack, 0, 32);
        byte[] expect = Hmac(Encoding.ASCII.GetBytes(Secret), "pipe2-ack|" + Hex(l.ClientNonce) + "|" + Hex(l.ServerNonce));
        if (!ack.SequenceEqual(expect)) { P("  !! Realm's AuthOk proof is wrong"); l.Close(); return null; }
        SetKeys(l, Secret);
        return l;
    }

    static void Check(string name, bool pass, string detail)
    {
        if (!pass) Failures++;
        P((pass ? "PASS " : "FAIL ") + name + (detail != null ? "  (" + detail + ")" : ""));
    }

    static long logMark;
    static void MarkLogs() { logMark = LogText().Length; }
    static string NewLog() { string t = LogText(); return logMark <= t.Length ? t.Substring((int)logMark) : t; }
    static string LogText()
    {
        if (RealmLogs == null || !Directory.Exists(RealmLogs)) return "";
        var sb = new StringBuilder();
        foreach (var f in Directory.GetFiles(RealmLogs).OrderBy(x => x))
            try { using (var fs = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) using (var r = new StreamReader(fs)) sb.Append(r.ReadToEnd()); } catch { }
        return sb.ToString();
    }
    static bool LogHas(string text, int waitMs = 2500)
    {
        if (RealmLogs == null) return true;
        DateTime end = DateTime.Now.AddMilliseconds(waitMs);
        do { if (NewLog().Contains(text)) return true; Thread.Sleep(200); } while (DateTime.Now < end);
        return false;
    }

    static byte[] HeroInfoBody(uint uid, string name, uint cps, byte level)
    {
        var b = new List<byte>();
        Action<uint> u32 = v => b.AddRange(BitConverter.GetBytes(v));
        Action<ushort> u16 = v => b.AddRange(BitConverter.GetBytes(v));
        Action<byte> u8 = v => b.Add(v);
        u32(12345);            // not 1 = visit, not a transfer
        u32(uid); u16(0); u32(1003); u16(410);
        b.AddRange(BitConverter.GetBytes(1000L)); u32(cps); b.AddRange(BitConverter.GetBytes(0UL));
        u16((ushort)ServerId); u16(0); u32(0); u32(0); u32(0); u32(0);
        u16(10); u16(10); u16(10); u16(0); u16(0); u32(500); u16(0); u16(0);
        u8(level); u8(15); u8(0); u8(0); u8(0); u8(0); u8(0); u32(0); u32(0);
        u16(0); u16(0); u32(0); u32(0); u16(0); u32(0);
        u8(0); u32(0); u32(0); u32(0); u16(0); u32(0); u32(0); u32(0);
        u8(3); u8((byte)name.Length); b.AddRange(Encoding.ASCII.GetBytes(name)); u8(0); u8(0);
        return b.ToArray();
    }
    static byte[] CompleteLoginBody(uint uid)
    {
        byte[] b = new byte[38];
        BitConverter.GetBytes(uid).CopyTo(b, 4);
        BitConverter.GetBytes((ushort)132).CopyTo(b, 20);   // packet offset 24 = Type
        return b;
    }
    static byte[] Envelope(ushort clientId, byte[] clientBody)
    {
        byte[] client = new byte[4 + clientBody.Length + 8];
        int len = 4 + clientBody.Length;
        client[0] = (byte)len; client[1] = (byte)(len >> 8); client[2] = (byte)clientId; client[3] = (byte)(clientId >> 8);
        Buffer.BlockCopy(clientBody, 0, client, 4, clientBody.Length);
        Buffer.BlockCopy(Encoding.ASCII.GetBytes("TQClient"), 0, client, len, 8);
        return Packet(130, client);
    }
    static byte[] ServerInfoBody(uint type, uint id, string name)
    {
        var b = new List<byte>();
        b.AddRange(BitConverter.GetBytes(type)); b.AddRange(BitConverter.GetBytes(id));
        byte[] n = new byte[16]; Encoding.ASCII.GetBytes(name).CopyTo(n, 0); b.AddRange(n);
        b.AddRange(BitConverter.GetBytes(3935u)); b.AddRange(BitConverter.GetBytes(100u)); b.AddRange(BitConverter.GetBytes(100u)); b.AddRange(BitConverter.GetBytes(0u));
        return b.ToArray();
    }

    static int Attack()
    {
        P("== pipe v2 attack test against 127.0.0.1:" + Port + "  " + DateTime.Now);
        byte[] dummy;

        // 1. no handshake: forged HeroInfo / Item / Update straight away
        MarkLogs();
        var l = Connect(); ReadHello(l);
        l.Raw(Packet(1006, HeroInfoBody(Uid, "Forged", 999999999, 140)));
        Check("unauthenticated 1006 HeroInfo -> link closed", l.ClosedWithin(3000), null);
        Check("   audited", LogHas("before the pipe handshake"), null);
        l.Close();

        // 2. wrong secret
        MarkLogs();
        l = Connect(); byte[] sn = ReadHello(l); byte[] cn = Nonce();
        byte[] badProof = Hmac(Encoding.ASCII.GetBytes("not-the-secret-not-the-secret-not-the-secret"), "pipe2|2|" + ServerId + "|" + Hex(sn) + "|" + Hex(cn));
        l.Raw(Packet(98, AuthBody(2, ServerId, cn, badProof)));
        Check("wrong HMAC proof -> link closed", l.ClosedWithin(3000), null);
        Check("   audited", LogHas("wrong proof"), null);
        l.Close();

        // 3. replay of a valid auth packet captured from another session
        var good = Handshake(2, ServerId, out dummy);
        Check("valid relay handshake (control for the replay test)", good != null, null);
        if (good != null) good.Close();
        MarkLogs();
        l = Connect(); ReadHello(l);
        l.Raw(dummy);
        Check("replayed auth packet (old nonce) -> link closed", l.ClosedWithin(3000), null);
        Check("   audited", LogHas("wrong proof"), null);
        l.Close();

        // 4. three failed handshakes in a row from this IP -> back-off (the valid handshake of step 3 reset the count)
        for (int k = 0; k < 2; k++)
        {
            l = Connect(); sn = ReadHello(l); cn = Nonce();
            if (sn != null) l.Raw(Packet(98, AuthBody(2, ServerId, cn, Hmac(Encoding.ASCII.GetBytes("wrong-wrong-wrong-wrong-wrong-wrong!"), "pipe2|2|" + ServerId + "|" + Hex(sn) + "|" + Hex(cn)))));
            l.ClosedWithin(3000); l.Close();
        }
        l = Connect();
        bool refused = ReadHello(l) == null;
        Check("after 3 failures the IP is refused (back-off)", refused, null);
        l.Close();
        P("   waiting 32 s for the back-off to expire...");
        Thread.Sleep(32000);

        // 5. bad MAC on a frame
        MarkLogs();
        l = Handshake(2, ServerId, out dummy);
        Check("handshake after the back-off", l != null, null);
        if (l == null) return Finish();
        l.Raw(l.Frame(Packet(133, new byte[0]), ++l.SendSeq, true));
        Check("frame with a bad MAC -> link closed", l.ClosedWithin(3000), null);
        Check("   audited", LogHas("bad MAC"), null);
        l.Close();

        // 6. replayed frame (same seq + MAC)
        MarkLogs();
        l = Handshake(2, ServerId, out dummy);
        l.Send(Packet(133, new byte[0]));
        l.Raw(l.LastFrame);
        Check("replayed frame (same sequence) -> link closed", l.ClosedWithin(3000), null);
        Check("   audited", LogHas("bad sequence"), null);
        l.Close();

        // 7. unframed packet after the handshake
        MarkLogs();
        l = Handshake(2, ServerId, out dummy);
        l.Raw(Packet(10017, new byte[40]));
        Check("raw (unframed) 10017 after the handshake -> link closed", l.ClosedWithin(3000), null);
        Check("   audited", LogHas("unframed packet 10017"), null);
        l.Close();

        // 8. relay link: control-only ServerInfo registration -> closed, not registered
        MarkLogs();
        l = Handshake(2, ServerId, out dummy);
        l.Send(Packet(99, ServerInfoBody(1, ServerId, "Fake")));
        Check("ServerInfo (99) on a relay link -> link closed", l.ClosedWithin(3000), null);
        Check("   audited", LogHas("control-only packet 99"), null);
        l.Close();

        // 9. legacy permanent transfer request -> refused
        MarkLogs();
        l = Handshake(2, ServerId, out dummy);
        var ct = new List<byte>(); ct.AddRange(BitConverter.GetBytes(0u)); ct.AddRange(BitConverter.GetBytes(Uid));
        l.Send(Packet(111, ct.ToArray()));
        Check("legacy transfer 111 -> link closed (AllowLegacyTransfer=0)", l.ClosedWithin(3000), null);
        Check("   audited", LogHas("legacy transfer request"), null);
        l.Close();

        // 10. relay link with a real setup, then forged packets
        MarkLogs();
        l = Handshake(2, ServerId, out dummy);
        l.Send(Packet(113, new byte[64]));                               // RoleInfo before HeroInfo
        Check("113 RoleInfo before HeroInfo dropped", LogHas("setup packet 113 before HeroInfo"), null);
        l.Send(Packet(1006, HeroInfoBody(Uid, "PipeBot", 1000, 100)));
        uint visitor = 0;
        DateTime until = DateTime.Now.AddSeconds(5);
        while (DateTime.Now < until && visitor == 0)
        {
            byte[] f = l.Read(1000);
            if (f == null) break; if (f == Link.Timeout) continue;
            byte[] inner = l.Unframe(f);
            if (inner != null && (inner[2] | (inner[3] << 8)) == 2501) visitor = BitConverter.ToUInt32(inner, 12);
        }
        Check("HeroInfo accepted, Realm answered 2501 with a visitor UID", visitor >= 3999900001u, "visitor " + visitor);
        l.Send(Packet(10010, CompleteLoginBody(Uid)));                   // ends the setup
        Thread.Sleep(1500);
        // forged client packets inside envelopes
        foreach (var id in new ushort[] { 1006, 1008, 10017, 99, 113, 2077, 2501 })
        {
            MarkLogs();
            byte[] body = id == 1006 ? HeroInfoBody(Uid, "Hacked", 999999999, 140) : new byte[60];
            l.Send(Envelope(id, body));
            Check("enveloped client packet " + id + " dropped", LogHas("client packet " + id + " inside an envelope dropped"), null);
        }
        // raw server data after the setup
        foreach (var id in new ushort[] { 10017, 113, 1008, 1006 })
        {
            MarkLogs();
            l.Send(Packet(id, id == 1006 ? HeroInfoBody(Uid, "Hacked", 999999999, 140) : new byte[60]));
            Check("raw " + id + " after the setup dropped", LogHas("raw packet " + id + " on a relay link") || LogHas("second HeroInfo"), null);
        }
        Check("relay link still open after the dropped packets (drops do not kill the visit)", !l.ClosedWithin(1500), null);
        l.Close();

        // 11. control link registration rules
        MarkLogs();
        var c = Handshake(1, ControlId, out dummy);
        Check("control link handshake (server " + ControlId + ")", c != null, null);
        if (c != null)
        {
            c.Send(Packet(99, ServerInfoBody(1, ServerId, "Spoof")));
            Check("control link of " + ControlId + " registering as server " + ServerId + " refused", LogHas("ServerInfo registration for " + ServerId + " refused"), null);
            c.Send(Packet(1006, new byte[40]));
            Check("1006 on a control link dropped", LogHas("not allowed on the control link"), null);
            byte[] d2;
            var c2 = Handshake(1, ControlId, out d2);
            Check("second control link for the same server refused", c2 == null, null);
            if (c2 != null) c2.Close();
            c.Close();
        }
        return Finish();
    }

    static int Finish()
    {
        P("");
        P(Failures == 0 ? "ALL CHECKS PASSED" : Failures + " CHECK(S) FAILED");
        return Failures == 0 ? 0 : 1;
    }

    // ------------------------------------------------------------------ fake Realm (tests what a home server relays)
    static int FakeRealm(int seconds)
    {
        var listener = new TcpListener(IPAddress.Loopback, Port);
        listener.Start();
        P("== fake Realm on 127.0.0.1:" + Port + " for " + seconds + " s");
        DateTime end = DateTime.Now.AddSeconds(seconds);
        var counts = new SortedDictionary<string, int>();
        while (DateTime.Now < end)
        {
            if (!listener.Pending()) { Thread.Sleep(100); continue; }
            var l = new Link { Tcp = listener.AcceptTcpClient(), Listener = true };
            l.S = l.Tcp.GetStream();
            l.ServerNonce = Nonce();
            var hello = new byte[17]; hello[0] = 2; Buffer.BlockCopy(l.ServerNonce, 0, hello, 1, 16);
            l.Raw(Packet(95, hello));
            byte[] auth = l.Read(5000);
            if (auth == null || auth == Link.Timeout || (auth[2] | (auth[3] << 8)) != 98) { P("home did not authenticate"); l.Close(); continue; }
            byte linkType = auth[5]; uint sid = BitConverter.ToUInt32(auth, 6);
            l.ClientNonce = new byte[16]; Buffer.BlockCopy(auth, 10, l.ClientNonce, 0, 16);
            byte[] proof = new byte[32]; Buffer.BlockCopy(auth, 26, proof, 0, 32);
            byte[] expect = Hmac(Encoding.ASCII.GetBytes(Secret), "pipe2|" + linkType + "|" + sid + "|" + Hex(l.ServerNonce) + "|" + Hex(l.ClientNonce));
            P("home server " + sid + " link type " + linkType + " proof " + (proof.SequenceEqual(expect) ? "OK" : "WRONG"));
            l.Raw(Packet(96, Hmac(Encoding.ASCII.GetBytes(Secret), "pipe2-ack|" + Hex(l.ClientNonce) + "|" + Hex(l.ServerNonce))));
            SetKeys(l, Secret);
            while (DateTime.Now < end)
            {
                byte[] f = l.Read(500);
                if (f == null) { P("home closed the link"); break; }
                if (f == Link.Timeout) continue;
                byte[] inner;
                try { inner = l.Unframe(f); }
                catch (Exception e) { P("FRAME ERROR " + e.Message); Failures++; break; }
                if (inner == null) { P("unframed packet " + (f[2] | (f[3] << 8)) + " from home"); Failures++; continue; }
                ushort id = (ushort)(inner[2] | (inner[3] << 8));
                string key;
                if (id == 130)
                {
                    ushort cid = (ushort)(inner[6] | (inner[7] << 8));
                    key = "envelope(" + cid + ")";
                }
                else key = "raw " + id;
                int n; counts.TryGetValue(key, out n); counts[key] = n + 1;
                if (n == 0) P("  first " + key + " (" + inner.Length + " bytes)");
                if (id == 1006)
                {
                    uint realUid = BitConverter.ToUInt32(inner, 8);
                    var b = new List<byte>(); b.AddRange(BitConverter.GetBytes((ushort)0)); b.AddRange(new byte[2]);
                    b.AddRange(BitConverter.GetBytes(realUid)); b.AddRange(BitConverter.GetBytes(3999900001u)); b.AddRange(BitConverter.GetBytes(0u));
                    l.Send(Packet(2501, b.ToArray()));
                    P("  answered 2501 (visitor 3999900001 for " + realUid + ")");
                }
            }
            l.Close();
        }
        listener.Stop();
        P("== frames received from home:");
        foreach (var kv in counts) P("  " + kv.Key + " x" + kv.Value);
        return 0;
    }
}
