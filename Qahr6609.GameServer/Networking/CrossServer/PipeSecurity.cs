// Inter-server pipe v2 security (docs\MULTI_SERVER.md, design: mutual HMAC handshake, per-frame seq + MAC,
// envelope for relayed client packets, allowlists, bind, size/rate limits).
//
// Wire format (every pipe packet uses the normal [u16 len][u16 id] ... ["TQServer"] framing):
//   95 PipeHello  listener -> connector  {u8 ver=2, u8[16] serverNonce}                                   (raw)
//   98 PipeAuth   connector -> listener  {u8 ver, u8 linkType, u32 serverId, u8[16] clientNonce, u8[32] proof} (raw)
//        proof = HMAC-SHA256(secret, "pipe2|" + linkType + "|" + serverId + "|" + hex(serverNonce) + "|" + hex(clientNonce))
//   96 PipeAuthOk listener -> connector  {u8[32] HMAC-SHA256(secret, "pipe2-ack|" + hex(clientNonce) + "|" + hex(serverNonce))} (raw)
//   after that every packet in both directions is a frame:
//  131 PipeFrame  {u32 seq, u8[12] mac, inner packet bytes}   mac = HMAC-SHA256(dirKey, seq || inner)[0..12]
//        sessionKey = HMAC(secret, "key|" + hex(serverNonce) + "|" + hex(clientNonce)); dirKey = HMAC(sessionKey, "c2s" | "s2c")
//        seq starts at 1 in each direction and must grow by exactly 1 (replay / reorder / drop -> link closed)
//  130 RelayClient (inside a frame, home -> Realm only) {raw client packet}: client bytes are only ever dispatched
//        through the client packet handlers (MsgInvoker), never through the server-data switch.
//  133 PipeHeartbeat (inside a frame) {}
// The secret never travels; a captured session cannot be replayed (fresh nonces) or extended (seq + MAC).
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Conquer6609.MsgInterServer
{
    public enum PipeLinkType : byte
    {
        None = 0,
        /// <summary>One per game server, server data only (server info, ranks, cross chat, schedules).</summary>
        Control = 1,
        /// <summary>One per visiting player: setup snapshot, then enveloped client packets.</summary>
        Relay = 2
    }

    /// <summary>shell.ini [InterServer] settings of the pipe (all optional; without a Secret the pipe is off).</summary>
    public static class PipeConfig
    {
        public const int ProtocolVersion = 2;
        public const int MinSecretLength = 32;
        /// <summary>A frame (and so an inner packet) never exceeds the socket buffer size.</summary>
        public const int MaxFrameSize = ServerSockets.ReceiveBuffer.HeadSize;

        /// <summary>[InterServer] Secret= (same on every server of the group, at least 32 characters).</summary>
        public static string Secret { get { return PipeServer.Security.Secret ?? ""; } }
        public static bool SecretUsable { get { PipeServer.Security.Load(); return Secret.Length >= MinSecretLength; } }

        /// <summary>[InterServer] Bind= interface of the pipe listener (default 127.0.0.1; never 0.0.0.0 on a public host).</summary>
        public static string Bind { get { return Framework.Settings.String("InterServer", "Bind", "127.0.0.1"); } }
        /// <summary>[InterServer] LinkPacketRate= frames per second one pipe link may send (default 5000).</summary>
        public static int LinkPacketRate { get { return Math.Max(100, Framework.Settings.Int("InterServer", "LinkPacketRate", 5000)); } }
        /// <summary>[InterServer] MaxRelayLinksPerServer= visiting players per home server (default 1000).</summary>
        public static int MaxRelayLinksPerServer { get { return Math.Max(1, Framework.Settings.Int("InterServer", "MaxRelayLinksPerServer", 1000)); } }
        /// <summary>[InterServer] HandshakeSeconds= time to finish the handshake (default 10).</summary>
        public static int HandshakeSeconds { get { return Math.Max(2, Framework.Settings.Int("InterServer", "HandshakeSeconds", 10)); } }
        /// <summary>[InterServer] AllowLegacyTransfer=1 re-enables the old paid permanent transfer (UNSAFE: the source character is
        /// deleted before the target saved it; warehouse/mail/house are lost). Default 0 = disabled.</summary>
        public static bool AllowLegacyTransfer { get { return Framework.Settings.Bool("InterServer", "AllowLegacyTransfer", false); } }
        /// <summary>[InterServer] RealmName= name of the Realm row in client_config.ini (default Realm1).</summary>
        public static string RealmName { get { return Framework.Settings.String("InterServer", "RealmName", "Realm1"); } }
    }

    /// <summary>Per-socket state of a v2 pipe link (SecuritySocket.Pipe).</summary>
    public sealed class PipeCodec
    {
        public readonly bool ListenerSide;
        public PipeLinkType Link;
        /// <summary>Peer's ServerID (listener side, after the handshake).</summary>
        public uint PeerServerID;
        public byte[] ServerNonce = new byte[16];
        public byte[] ClientNonce = new byte[16];
        public readonly int CreatedTick = Environment.TickCount;
        volatile bool ready;
        HMACSHA256 sendMac, recvMac;
        uint sendSeq, recvSeq;
        int rateStart, rateCount;
        /// <summary>Relay link on the Realm: the home server finished the setup snapshot (CompleteLogin).</summary>
        public bool SetupComplete;

        public PipeCodec(bool listenerSide, PipeLinkType link)
        {
            ListenerSide = listenerSide;
            Link = link;
        }

        public bool Ready { get { return ready; } }

        public void SetKeys(string secret)
        {
            byte[] session = PipeCrypto.Hmac(Encoding.ASCII.GetBytes(secret),
                Encoding.ASCII.GetBytes("key|" + PipeCrypto.Hex(ServerNonce) + "|" + PipeCrypto.Hex(ClientNonce)));
            byte[] c2s = PipeCrypto.Hmac(session, Encoding.ASCII.GetBytes("c2s"));
            byte[] s2c = PipeCrypto.Hmac(session, Encoding.ASCII.GetBytes("s2c"));
            sendMac = new HMACSHA256(ListenerSide ? s2c : c2s);
            recvMac = new HMACSHA256(ListenerSide ? c2s : s2c);
            sendSeq = 0;
            recvSeq = 0;
            ready = true;
        }

        /// <summary>Handshake ids that may travel before the keys are set.</summary>
        public static bool IsHandshakeId(ushort id)
        {
            return id == PacketTypes.PipeHello || id == PacketTypes.PipeAuthOk || id == PacketTypes.InterServer_Auth;
        }

        /// <summary>Frames per second check (per link, receive side).</summary>
        public bool AllowFrame()
        {
            int now = Environment.TickCount & int.MaxValue;
            if (rateCount == 0 || Math.Abs(now - rateStart) >= 1000)
            {
                rateStart = now;
                rateCount = 0;
            }
            return ++rateCount <= PipeConfig.LinkPacketRate;
        }

        /// <summary>Builds the frame for one inner packet. Caller holds the socket's send lock (sequence order).</summary>
        public unsafe byte[] Wrap(byte* inner, int innerSize)
        {
            if (innerSize < 4 || innerSize + PacketTypes.PipeFrameOverhead > PipeConfig.MaxFrameSize)
                return null;
            int total = innerSize + PacketTypes.PipeFrameOverhead;
            byte[] frame = new byte[total];
            uint seq = ++sendSeq;
            byte[] macInput = new byte[4 + innerSize];
            macInput[0] = (byte)seq; macInput[1] = (byte)(seq >> 8); macInput[2] = (byte)(seq >> 16); macInput[3] = (byte)(seq >> 24);
            System.Runtime.InteropServices.Marshal.Copy((IntPtr)inner, macInput, 4, innerSize);
            byte[] mac = sendMac.ComputeHash(macInput);
            int len = total - 8;
            frame[0] = (byte)len; frame[1] = (byte)(len >> 8);
            frame[2] = (byte)PacketTypes.PipeFrame; frame[3] = (byte)(PacketTypes.PipeFrame >> 8);
            Buffer.BlockCopy(macInput, 0, frame, 4, 4);
            Buffer.BlockCopy(mac, 0, frame, 8, PacketTypes.PipeMacSize);
            Buffer.BlockCopy(macInput, 4, frame, 20, innerSize);
            byte[] seal = Encoding.ASCII.GetBytes(ServerSockets.Packet.SealString ?? "TQServer");
            Buffer.BlockCopy(seal, 0, frame, total - 8, Math.Min(8, seal.Length));
            return frame;
        }

        /// <summary>Checks one received frame (whole packet incl. seal) and copies the inner packet into `inner`
        /// (positioned at offset 2, like a freshly received packet). Socket thread only.</summary>
        public unsafe bool Unwrap(byte* frame, int frameSize, ServerSockets.Packet inner, out string error)
        {
            error = null;
            if (frameSize < PacketTypes.PipeFrameOverhead + 4)
            {
                error = "frame too short (" + frameSize + ")";
                return false;
            }
            ushort id = *(ushort*)(frame + 2);
            if (id != PacketTypes.PipeFrame)
            {
                error = "unframed packet " + id + " after the handshake";
                return false;
            }
            uint seq = *(uint*)(frame + 4);
            if (seq != recvSeq + 1)
            {
                error = "bad sequence " + seq + " (expected " + (recvSeq + 1) + ")";
                return false;
            }
            int innerSize = frameSize - PacketTypes.PipeFrameOverhead;
            byte[] macInput = new byte[4 + innerSize];
            System.Runtime.InteropServices.Marshal.Copy((IntPtr)(frame + 4), macInput, 0, 4);
            System.Runtime.InteropServices.Marshal.Copy((IntPtr)(frame + 20), macInput, 4, innerSize);
            byte[] mac = recvMac.ComputeHash(macInput);
            int diff = 0;
            for (int i = 0; i < PacketTypes.PipeMacSize; i++)
                diff |= mac[i] ^ frame[8 + i];
            if (diff != 0)
            {
                error = "bad MAC (seq " + seq + ")";
                return false;
            }
            ushort innerLen = *(ushort*)(frame + 20);
            if (innerLen + 8 != innerSize)
            {
                error = "inner length " + innerLen + " does not match frame (" + innerSize + ")";
                return false;
            }
            recvSeq = seq;
            inner.Seek(0);
            System.Runtime.InteropServices.Marshal.Copy(macInput, 4, (IntPtr)inner.Memory, innerSize);
            inner.Size = innerSize;
            inner.Seek(2);
            return true;
        }
    }

    public static class PipeCrypto
    {
        static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public static byte[] Nonce()
        {
            byte[] b = new byte[16];
            lock (rng) rng.GetBytes(b);
            return b;
        }
        public static byte[] Hmac(byte[] key, byte[] data)
        {
            using (var h = new HMACSHA256(key))
                return h.ComputeHash(data);
        }
        public static string Hex(byte[] b)
        {
            var sb = new StringBuilder(b.Length * 2);
            foreach (var x in b) sb.Append(x.ToString("x2"));
            return sb.ToString();
        }
        public static bool FixedEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
        public static byte[] Proof(string secret, PipeLinkType link, uint serverId, byte[] serverNonce, byte[] clientNonce)
        {
            return Hmac(Encoding.ASCII.GetBytes(secret), Encoding.ASCII.GetBytes("pipe2|" + (byte)link + "|" + serverId + "|" + Hex(serverNonce) + "|" + Hex(clientNonce)));
        }
        public static byte[] Ack(string secret, byte[] serverNonce, byte[] clientNonce)
        {
            return Hmac(Encoding.ASCII.GetBytes(secret), Encoding.ASCII.GetBytes("pipe2-ack|" + Hex(clientNonce) + "|" + Hex(serverNonce)));
        }
    }

    /// <summary>Building / reading the raw handshake packets.</summary>
    public static class PipeHandshake
    {
        public static ServerSockets.Packet Hello(ServerSockets.Packet stream, byte[] serverNonce)
        {
            stream.InitWriter();
            stream.Write((byte)PipeConfig.ProtocolVersion);
            for (int i = 0; i < 16; i++) stream.Write(serverNonce[i]);
            stream.Finalize(PacketTypes.PipeHello);
            return stream;
        }
        public static bool ReadHello(ServerSockets.Packet stream, out byte[] serverNonce)
        {
            serverNonce = null;
            if (stream.Size < 4 + 1 + 16 + 8) return false;
            if (stream.ReadUInt8() != PipeConfig.ProtocolVersion) return false;
            serverNonce = stream.ReadBytes(16);
            return true;
        }
        public static ServerSockets.Packet Auth(ServerSockets.Packet stream, PipeLinkType link, uint serverId, byte[] clientNonce, byte[] proof)
        {
            stream.InitWriter();
            stream.Write((byte)PipeConfig.ProtocolVersion);
            stream.Write((byte)link);
            stream.Write(serverId);
            for (int i = 0; i < 16; i++) stream.Write(clientNonce[i]);
            for (int i = 0; i < 32; i++) stream.Write(proof[i]);
            stream.Finalize(PacketTypes.InterServer_Auth);
            return stream;
        }
        public static bool ReadAuth(ServerSockets.Packet stream, out PipeLinkType link, out uint serverId, out byte[] clientNonce, out byte[] proof)
        {
            link = PipeLinkType.None; serverId = 0; clientNonce = null; proof = null;
            if (stream.Size < 4 + 1 + 1 + 4 + 16 + 32 + 8) return false;
            if (stream.ReadUInt8() != PipeConfig.ProtocolVersion) return false;
            link = (PipeLinkType)stream.ReadUInt8();
            serverId = stream.ReadUInt32();
            clientNonce = stream.ReadBytes(16);
            proof = stream.ReadBytes(32);
            return link == PipeLinkType.Control || link == PipeLinkType.Relay;
        }
        public static ServerSockets.Packet AuthOk(ServerSockets.Packet stream, byte[] ack)
        {
            stream.InitWriter();
            for (int i = 0; i < 32; i++) stream.Write(ack[i]);
            stream.Finalize(PacketTypes.PipeAuthOk);
            return stream;
        }
        public static bool ReadAuthOk(ServerSockets.Packet stream, out byte[] ack)
        {
            ack = null;
            if (stream.Size < 4 + 32 + 8) return false;
            ack = stream.ReadBytes(32);
            return true;
        }

        /// <summary>Connector side: answers a PipeHello with PipeAuth. Returns false (caller closes) on a bad hello.</summary>
        public static bool AnswerHello(ServerSockets.SecuritySocket socket, ServerSockets.Packet hello, uint myServerId)
        {
            var codec = socket.Pipe;
            byte[] serverNonce;
            if (codec == null || codec.Ready || !ReadHello(hello, out serverNonce))
                return false;
            codec.ServerNonce = serverNonce;
            codec.ClientNonce = PipeCrypto.Nonce();
            byte[] proof = PipeCrypto.Proof(PipeConfig.Secret, codec.Link, myServerId, codec.ServerNonce, codec.ClientNonce);
            using (var rec = new ServerSockets.RecycledPacket())
                socket.Send(Auth(rec.GetStream(), codec.Link, myServerId, codec.ClientNonce, proof));
            return true;
        }

        /// <summary>Connector side: checks the listener's proof and sets the session keys.</summary>
        public static bool AcceptAuthOk(ServerSockets.SecuritySocket socket, ServerSockets.Packet packet)
        {
            var codec = socket.Pipe;
            byte[] ack;
            if (codec == null || codec.Ready || codec.ClientNonce == null || !ReadAuthOk(packet, out ack))
                return false;
            if (!PipeCrypto.FixedEquals(ack, PipeCrypto.Ack(PipeConfig.Secret, codec.ServerNonce, codec.ClientNonce)))
                return false;
            codec.SetKeys(PipeConfig.Secret);
            return true;
        }

        /// <summary>Closes the link when the handshake did not finish in time.</summary>
        public static void ArmTimeout(ServerSockets.SecuritySocket socket, string side)
        {
            Timer t = null;
            t = new Timer(_ =>
            {
                try
                {
                    var codec = socket.Pipe;
                    if (socket.Alive && codec != null && !codec.Ready)
                    {
                        Security.Guard.Audit("interserver", 0, "", side + ": pipe handshake with " + socket.RemoteIp + " timed out");
                        socket.Disconnect();
                    }
                }
                catch { }
                finally { try { t.Dispose(); } catch { } }
            }, null, PipeConfig.HandshakeSeconds * 1000, Timeout.Infinite);
        }
    }

    /// <summary>Which packet ids may cross the pipe where.</summary>
    public static class PipeFilter
    {
        /// <summary>Server-to-client or server-data ids: never accepted from a client (inside an envelope or forwarded by home).</summary>
        static readonly HashSet<ushort> serverOnly = new HashSet<ushort>()
        {
            Game.GamePackets.HeroInfo,      // 1006
            Game.GamePackets.Item,          // 1008
            Game.GamePackets.ExtraItem,     // 2077
            Game.GamePackets.Update,        // 10017
            Game.GamePackets.MsgInterServerIdentifier, // 2501
            Game.GamePackets.ChiInfo,       // 2534 (InterServerStatus snapshot)
            Game.GamePackets.SubClass,      // 2320 (snapshot)
            Game.GamePackets.MsgStauts,     // 1040 (jianghu status snapshot)
            Game.GamePackets.Spell,         // 1103 (snapshot)
            Game.GamePackets.Proficiency,   // 1025 (snapshot)
        };

        /// <summary>Ids 95-140 are the inter-server range (auth, frame, envelope, server info, ranks, snapshot extras).</summary>
        public static bool IsInterServerId(ushort id)
        {
            return id >= 95 && id <= 140;
        }

        /// <summary>Server-data ids (never from a client).</summary>
        public static bool IsServerDataId(ushort id)
        {
            return IsInterServerId(id) || serverOnly.Contains(id);
        }

        /// <summary>Ids that may only travel on a control link.</summary>
        public static bool IsControlOnlyId(ushort id)
        {
            return id == PacketTypes.InterServer_ServerInfo || id == PacketTypes.InterServer_UnionRanks || id == PacketTypes.InterServer_EliteRank
                || id == PacketTypes.ProgramChat || id == PacketTypes.ProgramOnlineInfo || id == PacketTypes.InterServer_NobilityRank;
        }

        /// <summary>Allowlist of client packets that home relays to the Realm (and the Realm accepts inside an envelope):
        /// a real client message (it has a client handler) and not a server-data id.</summary>
        public static bool IsClientRelayAllowed(ushort id)
        {
            if (IsServerDataId(id))
                return false;
            Action<Client.GameClient, ServerSockets.Packet> handler;
            return Program.MsgInvoker != null && Program.MsgInvoker.TryGetInvoker(id, out handler);
        }

        /// <summary>Raw (not enveloped) packets the home server may send on a relay link before CompleteLogin.</summary>
        public static bool IsSetupId(ushort id)
        {
            switch (id)
            {
                case Game.GamePackets.HeroInfo:
                case Game.GamePackets.Item:
                case Game.GamePackets.ExtraItem:
                case Game.GamePackets.SubClass:
                case Game.GamePackets.ChiInfo:
                case Game.GamePackets.Spell:
                case Game.GamePackets.Proficiency:
                case Game.GamePackets.MsgStauts:
                case Game.GamePackets.Update:
                case Game.GamePackets.QuestList:
                case Game.GamePackets.QuestData:
                case Game.GamePackets.DataMap:          // only the CompleteLogin action (ends the setup)
                case PacketTypes.InterServer_RoleInfo:
                case PacketTypes.InterServer_SpecialTitles:
                case PacketTypes.InterServer_CheckTransfer:
                case PacketTypes.InterServer_Achievement:
                case PacketTypes.InterServer_Chi:
                case PacketTypes.InterServer_JiangHu:
                case PacketTypes.InterServer_InnerPower:
                case PacketTypes.InterServer_UnionInfo:
                case PacketTypes.InterServer_GuildInfo:
                case PacketTypes.PipeHeartbeat:
                    return true;
            }
            return false;
        }

        // Audit lines for dropped packets are rate limited (a client spamming forged ids must not flood the log).
        static int auditWindowStart, auditCount;
        public static void AuditDrop(uint uid, string name, string details)
        {
            int now = Environment.TickCount & int.MaxValue;
            lock (serverOnly)
            {
                if (auditCount == 0 || Math.Abs(now - auditWindowStart) >= 10000)
                {
                    auditWindowStart = now;
                    auditCount = 0;
                }
                if (++auditCount > 30)
                    return;
            }
            Security.Guard.Audit("interserver-drop", uid, name ?? "", details);
        }
    }

    /// <summary>Failed handshakes per IP: from the third failure on the IP is refused for 30 s, doubling up to 10 minutes.</summary>
    public static class PipeBackoff
    {
        sealed class Entry { public int Failures; public DateTime Until; }
        static readonly ConcurrentDictionary<string, Entry> entries = new ConcurrentDictionary<string, Entry>();

        public static bool IsBlocked(string ip)
        {
            Entry e;
            return ip != null && entries.TryGetValue(ip, out e) && DateTime.UtcNow < e.Until;
        }
        public static void Fail(string ip)
        {
            if (ip == null) return;
            var e = entries.GetOrAdd(ip, _ => new Entry());
            lock (e)
            {
                // two mistakes are free (restart with a changed secret), from the third on: 30 s, 60 s, ... 10 min
                e.Failures = Math.Min(e.Failures + 1, 12);
                if (e.Failures >= 3)
                    e.Until = DateTime.UtcNow.AddSeconds(Math.Min(600, 30 * (1 << (e.Failures - 3))));
            }
        }
        public static void Success(string ip)
        {
            Entry e;
            if (ip != null) entries.TryRemove(ip, out e);
        }
    }
}
