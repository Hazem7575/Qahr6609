
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.ServerSockets
{
    public unsafe class SecuritySocket
    {

        public ReceiveBuffer ReceiveBuff;
        public bool IsGameServer
        {
            get
            {
                return Crypto != null;
            }
        }

        public bool SetDHKey = false;

        public object SendRoot;
        private Action<SecuritySocket> OnDisconnect;
        private Action<SecuritySocket, Packet> OnReceiveHandler;
        public Socket Connection;
        public object Client;

        private Queue<byte[]> OnSend;
        private IDisposable[] TimerSubscriptions = null;
        public bool Alive = false;
        public Cryptography.TQCast5 Crypto;
        public bool OnInterServer = false;
        /// <summary>Inter-server pipe v2 link state (MsgInterServer\PipeSecurity.cs). When set, every packet is sent and
        /// received as an authenticated frame once the handshake finished; before that only handshake packets pass.</summary>
        public Conquer6609.MsgInterServer.PipeCodec Pipe;
        // Remote address read once when the socket is created (RemoteEndPoint allocates on every call
        // and throws once the socket is closed).
        private string remoteAddress;
        public string RemoteIp
        {
            get
            {
                try
                {
                    if (Connection == null)
                        return "NONE";
                    if (Alive && remoteAddress != null)
                        return remoteAddress;
                    return (Connection.RemoteEndPoint as IPEndPoint).Address.ToString();
                }
                catch
                {
                    return "NONE";
                }
            }
        }
        /// <summary>Remote IP captured at creation; still valid after the socket was closed.</summary>
        public string RemoteAddress
        {
            get { return remoteAddress ?? "NONE"; }
        }
        // Environment.TickCount when the connection was created (handshake timeout).
        private int CreatedTick;
        private bool HandshakeDone;
        // 1 while this socket waits in SocketPoll's pending-send list.
        internal int SendNotified;
        public Extensions.Time32 LastReceive;
        public Client.GameClient Game;
        public ServerSocket Server;

        public bool ConnectFull = false;

        public SecuritySocket(ServerSocket serversocket, Action<SecuritySocket> _OnDisconnect, Action<SecuritySocket, Packet> _OnReceiveHandler)
        {
            Server = serversocket;
            OnReceiveHandler = _OnReceiveHandler;
            OnDisconnect = _OnDisconnect;


        }
        public SecuritySocket(Action<SecuritySocket> _OnDisconnect, Action<SecuritySocket, Packet> _OnReceiveHandler)
        {
            OnReceiveHandler = _OnReceiveHandler;
            OnDisconnect = _OnDisconnect;
        }
        public bool Connect(string IPAddres, ushort port, out Socket _socket)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bool connected = false;
            try
            {
                // wait up to 1 s for the connect (same budget as before), then complete it: EndConnect throws when
                // the connection was refused/failed, so a dead socket is never reported as connected
                IAsyncResult asyncResult = _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(IPAddres), port), null, null);
                if (asyncResult.AsyncWaitHandle.WaitOne(1000))
                {
                    _socket.EndConnect(asyncResult);
                    _socket.Blocking = false;
                    _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                    _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                    _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                    _socket.ReceiveBufferSize = _socket.SendBufferSize = ServerSockets.ReceiveBuffer.RECV_BUFFER_SIZE;
                    connected = true;
                }
            }
            catch (Exception)
            {
                connected = false;
            }
            if (!connected)
            {
                // timed out or failed: release the socket (it used to stay open and pending)
                try { _socket.Close(); }
                catch { }
                _socket = null;
                return false;
            }
            // Create(_socket) adds this connection to SocketPoll.ConnectionPoll (only once)
            return true;
        }
        public void Create(Socket _socket)
        {
            try
            {
                ReceiveBuff = new ReceiveBuffer(Program.ServerConfig.Port_ReceiveSize);
                Connection = _socket;
                remoteAddress = null;
                try { remoteAddress = (_socket.RemoteEndPoint as IPEndPoint).Address.ToString(); }
                catch { }
                CreatedTick = Environment.TickCount;
                HandshakeDone = false;
                SetDHKey = false;
                // send queue exists before the socket is marked alive (other threads may Send at once)
                SendRoot = new object();
                OnSend = new Queue<byte[]>();
                Alive = true;
                LastReceive = Extensions.Time32.Now;
                Client = Crypto = null;
                TimerSubscriptions = new IDisposable[]
      {
       
      };
                SocketPoll.ConnectionPoll.Add(this);
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        public void SetCrypto(Cryptography.TQCast5 Crypt)
        {
            if (!Program.ServerConfig.IsInterServer)
            {
                Crypto = Crypt;
            }
        }
        
        // [Performance] HandshakeTimeoutSec: a game connection that has not finished the key exchange and sent
        // its login (or reached character creation) within this time is closed. 0 = off. Logged-in players
        // are never timed out here.
        public static readonly int HandshakeTimeoutMs = Math.Max(0, Conquer6609.Framework.Settings.Int("Performance", "HandshakeTimeoutSec", 60)) * 1000;
        private const Conquer6609.Client.ServerFlag HandshakeCompleteFlags = Conquer6609.Client.ServerFlag.AcceptLogin | Conquer6609.Client.ServerFlag.CreateCharacter
            | Conquer6609.Client.ServerFlag.CreateCharacterSucces | Conquer6609.Client.ServerFlag.LoginFull | Conquer6609.Client.ServerFlag.OnLoggion;

        public void CheckUp()
        {
            if (HandshakeDone || !Alive || HandshakeTimeoutMs <= 0 || !IsGameServer)
                return;
            var game = Game;
            if (SetDHKey && game != null && (game.ClientFlag & HandshakeCompleteFlags) != Conquer6609.Client.ServerFlag.None)
            {
                HandshakeDone = true;
                return;
            }
            if (unchecked(Environment.TickCount - CreatedTick) > HandshakeTimeoutMs)
                Disconnect();
        }


        public int tstgg = 0;
     
        public unsafe void Send(Packet msg)
        {
            try
            {
                
                if (Alive)
                {
                    int size = msg.Size;
                    if (size <= 0)
                        return;
                    if (PacketStudio.Bridge.PacketBridge.IsActive)// PacketStudio live stream (plaintext, before Encrypt)
                    {
                        var g = Game;
                        PacketStudio.Bridge.PacketBridge.OnServerToClient(g != null && g.Player != null ? g.Player.UID : 0, g != null && g.Player != null ? g.Player.Name : null, msg.Memory, size);
                    }
                    var pipe = Pipe;
                    if (pipe != null)
                    {
                        SendPipe(pipe, msg, size);
                        return;
                    }
                    byte[] _buffer = new byte[size];
                    lock (SendRoot)
                    {
                        // Encrypt writes all `size` bytes of _buffer, so no plaintext copy is needed first.
                        // The cipher is a stream cipher: encrypting and queueing stay under one lock to keep order.
                        if (Crypto != null)
                            Crypto.Encrypt(msg.Memory, 0, _buffer, 0, size);
                        else
                            fixed (byte* ptr = _buffer)
                                msg.memcpy(ptr, msg.Memory, size);

                        OnSend.Enqueue(_buffer);
                    }
                    // Wake the socket thread so the packet goes out now instead of on its next pass.
                    if (System.Threading.Interlocked.Exchange(ref SendNotified, 1) == 0)
                        SocketPoll.NotifySend(this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        // Pipe v2: frames the packet (seq + MAC) once the handshake finished; before that only handshake packets go out raw.
        private unsafe void SendPipe(Conquer6609.MsgInterServer.PipeCodec pipe, Packet msg, int size)
        {
            ushort id = size >= 4 ? *(ushort*)(msg.Memory + 2) : (ushort)0;
            byte[] data;
            lock (SendRoot)
            {
                if (pipe.Ready)
                {
                    data = pipe.Wrap(msg.Memory, size);
                    if (data == null)
                    {
                        Conquer6609.Security.Guard.Audit("interserver", 0, "", "pipe packet " + id + " (" + size + " bytes) too big for a frame, dropped");
                        return;
                    }
                }
                else
                {
                    if (!Conquer6609.MsgInterServer.PipeCodec.IsHandshakeId(id))
                        return;// nothing but the handshake before the keys exist
                    data = new byte[size];
                    fixed (byte* ptr = data)
                        msg.memcpy(ptr, msg.Memory, size);
                }
                OnSend.Enqueue(data);
            }
            if (System.Threading.Interlocked.Exchange(ref SendNotified, 1) == 0)
                SocketPoll.NotifySend(this);
        }

        public ReceiveBuffer DHKeyBuffer = new ReceiveBuffer(1024, true);
        public ReceiveBuffer EncryptedDHKeyBuffer = new ReceiveBuffer(1024, true);
        public bool IsCompleteDHKey(out int type)
        {

            type = 0;
            try
            {
                if (DHKeyBuffer.Length() < 8)
                    return false;

                byte[] buffer = new byte[Packet.SealSize];
                for (int x = 0; x < buffer.Length; x++)
                    buffer[x] = DHKeyBuffer.buffer[x + (DHKeyBuffer.Length() - Packet.SealSize)];

                string Text = System.Text.ASCIIEncoding.ASCII.GetString(DHKeyBuffer.buffer);

                bool accept = Text.Contains("TQClient");
                if (!Text.EndsWith("TQClient"))
                    type = 1;
                return accept;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                Disconnect();
                return false;
            }
        }
        public unsafe bool ReceiveDHKey()
        {
            try
            {


                if (Alive)//&& WindowsAPI.ws2_32.CanRead(this))
                {
                    int rec_type = 0;
                    if (!SetDHKey && Alive)
                    {
                        SocketError Socket_Error = SocketError.IsConnected;
                        int length = DHKeyBuffer.MaxLength() - DHKeyBuffer.Length();
                        if (length <= 0)
                        {
                            Disconnect();
                            return false;
                        }
                        int ret = Connection.Receive(DHKeyBuffer.buffer, DHKeyBuffer.Length(), length, SocketFlags.None, out Socket_Error);
                        if (ret > 0)
                        {
                            System.Buffer.BlockCopy(DHKeyBuffer.buffer, DHKeyBuffer.Length(), EncryptedDHKeyBuffer.buffer, EncryptedDHKeyBuffer.Length(), ret);
                            EncryptedDHKeyBuffer.AddLength(ret);
                            if (Crypto != null)
                            {
                                fixed (byte* ptr = DHKeyBuffer.buffer)
                                    Crypto.Decrypt(DHKeyBuffer.buffer, DHKeyBuffer.Length(), ptr, DHKeyBuffer.Length(), ret);
                            }
                            DHKeyBuffer.AddLength(ret);
                            if (IsCompleteDHKey(out rec_type))
                            {

                                using (var rec = new RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    stream.Seek(0);
                                    fixed (byte* ptr = DHKeyBuffer.buffer)
                                        stream.memcpy(stream.Memory, ptr, DHKeyBuffer.Length());

                                    stream.Size = DHKeyBuffer.Length();

                                    if (OnReceiveHandler != null)
                                        OnReceiveHandler.Invoke(this, stream);
                                }
                            }
                        }
                        else if (ret == 0)//<= 0 || Socket_Error == SocketError.ConnectionAborted)
                        {
                            if (Socket_Error != SocketError.WouldBlock)
                                Disconnect();
                            return false;
                        }
                        else//ret < 0
                        {
                            if (Socket_Error != SocketError.WouldBlock)
                                Disconnect();
                        }


                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Disconnect();
            }
            return false;
        }
        public bool ReceiveBuffer()
        {
            if (Alive)
            {

                {
                    try
                    {
                        if (!ConnectFull)
                            return false;
                        if (!SetDHKey && Crypto != null)
                        {
                            ReceiveDHKey();
                        }
                        else //if (WindowsAPI.ws2_32.CanRead(this))
                        {
                            try
                            {
                                if (!Alive)
                                    return false;
                                //  int available = Connection.Available;
                                //  if (available == 0)
                                ///  {
                                //      Disconnect();
                                //      return false;
                                //  }
                                if (!Receive())
                                {
                                    return false;
                                }

                            }
                            catch (Exception e)
                            {
                                MyConsole.SaveException(e);
                                Disconnect();
                            }
                            return true;
                        }


                    }
                    catch (Exception e) { Disconnect(); Console.WriteLine(e.ToString()); }
                }
            }
            return false;
        }

        // Maximum packets a game client may send per second before it is disconnected (0 = unlimited).
        public static int MaxPacketsPerSecond = 400;
        private int RateWindowStart = 0;
        private int RateWindowCount = 0;

        // Counts one received packet; returns false when the per-second limit is exceeded.
        private bool CheckPacketRate()
        {
            if (MaxPacketsPerSecond <= 0 || !IsGameServer)
                return true;
            int now = Environment.TickCount & int.MaxValue;
            if (RateWindowCount == 0 || Math.Abs(now - RateWindowStart) >= 1000)
            {
                RateWindowStart = now;
                RateWindowCount = 0;
            }
            RateWindowCount++;
            return RateWindowCount <= MaxPacketsPerSecond;
        }

        // ---------------------------------------------------------------- per packet type limits
        private PacketTypeLimiter typeLimiter;
        private int dropAuditTick;
        private int droppedPackets;

        /// <summary>
        /// Per packet type token buckets ([Performance] PacketRate*). Returns false when the packet must be
        /// dropped (the connection stays open). Socket thread only.
        /// </summary>
        public unsafe bool AllowPacketType(ushort packetId, Packet stream)
        {
            if (!PacketTypeLimiter.Enabled || !IsGameServer)
                return true;
            int category = PacketTypeLimiter.Classify(packetId, stream);
            if (category < 0)
                return true;
            if (typeLimiter == null)
                typeLimiter = new PacketTypeLimiter();
            if (typeLimiter.Allow(category))
                return true;

            droppedPackets++;
            int now = Environment.TickCount;
            if (dropAuditTick == 0 || unchecked(now - dropAuditTick) >= 10000)
            {
                uint uid = 0;
                string pname = "";
                try
                {
                    var gc = Game;
                    if (gc != null && gc.Player != null) { uid = gc.Player.UID; pname = gc.Player.Name; }
                }
                catch { }
                Conquer6609.Security.Guard.Audit("packet-rate", uid, pname, "ip=" + RemoteAddress + " dropped " + droppedPackets
                    + " packet(s), last type " + packetId + " (" + PacketTypeLimiter.Names[category] + " limit " + PacketTypeLimiter.Rates[category] + "/s)");
                dropAuditTick = now == 0 ? 1 : now;
                droppedPackets = 0;
            }
            return false;
        }

        // ---------------------------------------------------------------- handler exceptions
        // [Performance] MaxHandlerExceptions: more handler exceptions than this within 10 s disconnect the client.
        // 0 (default) = never disconnect, only write a throttled audit line (at most one per connection per 60 s).
        public static readonly int MaxHandlerExceptions = Math.Max(0, Conquer6609.Framework.Settings.Int("Performance", "MaxHandlerExceptions", 0));
        private int handlerExWindowStart;
        private int handlerExCount;
        private int handlerExTotal;
        private int handlerExAuditTick;

        public void CountHandlerException()
        {
            if (!Alive)
                return;
            int now = Environment.TickCount;
            if (handlerExCount == 0 || unchecked(now - handlerExWindowStart) >= 10000)
            {
                handlerExWindowStart = now;
                handlerExCount = 0;
            }
            handlerExCount++;
            handlerExTotal++;
            bool disconnect = MaxHandlerExceptions > 0 && handlerExCount > MaxHandlerExceptions;
            bool audit = disconnect || handlerExAuditTick == 0 || unchecked(now - handlerExAuditTick) >= 60000;
            if (!audit)
                return;
            uint uid = 0;
            string pname = "";
            try
            {
                var gc = Game;
                if (gc != null && gc.Player != null) { uid = gc.Player.UID; pname = gc.Player.Name; }
            }
            catch { }
            handlerExAuditTick = now == 0 ? 1 : now;
            if (disconnect)
            {
                Conquer6609.Security.Guard.Audit("handler-exceptions", uid, pname, "ip=" + RemoteAddress + " more than " + MaxHandlerExceptions + " handler exceptions in 10 s, disconnected");
                handlerExCount = 0;
                handlerExTotal = 0;
                Disconnect();
            }
            else
            {
                Conquer6609.Security.Guard.Audit("handler-exceptions", uid, pname, "ip=" + RemoteAddress + " " + handlerExTotal + " handler exception(s) since last report (audit only)");
                handlerExTotal = 0;
            }
        }

        public unsafe bool HandlerBuffer()
        {
            // At most 30 packets are handled per pass for one connection.
            int counts = 30;
            while (true && counts > 0)
            {
                counts--;
                if (!Alive)
                    return false;
                bool consumed = false;
                try
                {

                    if (!ConnectFull)
                        return false;
                    if (ReceiveBuff.Length() == 0)
                        return false;
                    // The 2-byte length header must be complete before it is read or validated;
                    // otherwise a partial header (pipe/account sockets) would be judged on stale bytes.
                    if (ReceiveBuff.Length() < 2)
                        return false;
                    int Length = (int)(ReceiveBuff.ReadHead() + (IsGameServer ? 8 : 0));
                    if (Program.ServerConfig.IsInterServer || OnInterServer)
                        Length += 8;

                    // A packet shorter than its own header can never be consumed: drop the connection.
                    if (Length < 4)
                    {
                        Disconnect();
                        return false;
                    }
                    if (Length > ServerSockets.ReceiveBuffer.HeadSize)
                    {
                        Disconnect();
                        return false;
                    }
                    if (Length > ReceiveBuff.Length())
                        return false;

                    if (!CheckPacketRate())
                    {
                        uint uid = 0;
                        string pname = "";
                        try
                        {
                            var gc = Client as Conquer6609.Client.GameClient;
                            if (gc != null && gc.Player != null) { uid = gc.Player.UID; pname = gc.Player.Name; }
                        }
                        catch { }
                        Conquer6609.Security.Guard.Audit("packet-flood", uid, pname, "ip=" + RemoteIp + " more than " + MaxPacketsPerSecond + " packets/s");
                        Disconnect();
                        return false;
                    }

                    LastReceive = Extensions.Time32.Now;
                    Packet Stream = PacketRecycle.Take();

                    Stream.Seek(0);

                    fixed (byte* ptr = ReceiveBuff.buffer)
                    {
                        Stream.memcpy(Stream.stream, ptr, Length);
                        if (Length < ReceiveBuff.Length())
                        {
                            fixed (void* next_buffer = &ReceiveBuff.buffer[Length])
                            {
                                Stream.memcpy(ptr, next_buffer, ReceiveBuff.Length() - Length);
                            }
                        }
                        Stream.Size = Length;

                        ReceiveBuff.DelLength(Length);
                        consumed = true;
                    }

                    Stream.SeekForward(2);

                    var pipe = Pipe;
                    if (pipe != null)
                    {
                        Packet inner = PipeReceive(pipe, Stream);
                        PacketRecycle.Reuse(Stream);
                        if (inner == null)
                            return false;
                        Stream = inner;
                    }

                    if (OnReceiveHandler != null)
                        OnReceiveHandler.Invoke(this, Stream);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    // A failure before the packet left the buffer would repeat forever: drop the connection.
                    if (!consumed)
                    {
                        Disconnect();
                        return false;
                    }
                }
            }
            return false;
        }
        // Pipe v2 receive side: rate limit, then the raw handshake packet (before the keys) or the checked inner packet.
        // Returns null after closing the link.
        private unsafe Packet PipeReceive(Conquer6609.MsgInterServer.PipeCodec pipe, Packet frame)
        {
            string error = null;
            ushort id = frame.Size >= 4 ? *(ushort*)(frame.Memory + 2) : (ushort)0;
            Packet result = null;
            if (!pipe.AllowFrame())
                error = "more than " + Conquer6609.MsgInterServer.PipeConfig.LinkPacketRate + " frames/s";
            else if (!pipe.Ready)
            {
                if (Conquer6609.MsgInterServer.PipeCodec.IsHandshakeId(id))
                {
                    result = PacketRecycle.Take();
                    result.Seek(0);
                    result.memcpy(result.Memory, frame.Memory, frame.Size);
                    result.Size = frame.Size;
                    result.Seek(2);
                }
                else
                    error = "packet " + id + " before the pipe handshake";
            }
            else
            {
                result = PacketRecycle.Take();
                if (!pipe.Unwrap(frame.Memory, frame.Size, result, out error))
                {
                    PacketRecycle.Reuse(result);
                    result = null;
                }
            }
            if (result == null)
            {
                Conquer6609.Security.Guard.Audit("interserver", 0, "", "pipe link " + RemoteIp + " closed: " + error);
                Conquer6609.MsgInterServer.PipeBackoff.Fail(RemoteIp);
                Disconnect();
            }
            return result;
        }

        static byte[] Keys = { 18, 09, 17, 19 };// key length = 16
        public static void Decrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(Keys[i % 4] ^ data[i]);
            }
        }
        public bool Receive()
        {
            if (Alive)
            {
                SocketError Socket_Error = SocketError.IsConnected;
                try
                {
                    int length = ReceiveBuff.MaxLength() - ReceiveBuff.Length();

                    /*int receive_size = 0;
                    if (available > length)
                        receive_size = length;
                    else
                        receive_size = available;
                    if (receive_size == 0)
                    {
                        return false;
                    }*/
                    int ret = Connection.Receive(ReceiveBuff.buffer, ReceiveBuff.Length(), length, SocketFlags.None, out Socket_Error);
                    if (ret > 0)
                    {

                        if (Crypto != null)
                        {
                            fixed (byte* ptr = ReceiveBuff.buffer)
                                Crypto.Decrypt(ReceiveBuff.buffer, ReceiveBuff.Length(), ptr, ReceiveBuff.Length(), ret);
                        }
                        ReceiveBuff.AddLength(ret);

                        return true;
                    }
                    else if (ret == 0)//المنيكة
                    {
                        if (Socket_Error != SocketError.WouldBlock)
                            Disconnect();
                        return false;
                    }
                    else//ret < 0
                    {
                        if (Socket_Error != SocketError.WouldBlock)
                            Disconnect();
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(Socket_Error.ToString());

                }
            }
            return false;
        }


        public bool CanDequeue(out byte[] data)
        {
            data = null;
            lock (SendRoot)
            {
                if (OnSend.Count > 0)
                    data = OnSend.Dequeue();
            }
            return data != null;
        }

        public int nPutBytes = 0;
        public byte[] packet = null;
        // Bytes of `packet` that belong to the current send (packet may be the reused batch buffer).
        private int packetLength = 0;
        private byte[] batchBuffer;
        private const int SendBatchSize = 8192;

        // Takes the next data to send from the queue: several small queued packets are copied into one
        // buffer (same bytes, same order) so they leave in one send call. Socket thread only.
        private bool NextSendBuffer()
        {
            lock (SendRoot)
            {
                while (OnSend.Count > 0 && OnSend.Peek().Length == 0)
                    OnSend.Dequeue();
                if (OnSend.Count == 0)
                    return false;
                byte[] first = OnSend.Peek();
                if (OnSend.Count == 1 || first.Length >= SendBatchSize)
                {
                    packet = OnSend.Dequeue();
                    packetLength = packet.Length;
                    nPutBytes = 0;
                    return true;
                }
                if (batchBuffer == null)
                    batchBuffer = new byte[SendBatchSize];
                int length = 0;
                while (OnSend.Count > 0)
                {
                    byte[] next = OnSend.Peek();
                    if (length + next.Length > SendBatchSize)
                        break;
                    OnSend.Dequeue();
                    System.Buffer.BlockCopy(next, 0, batchBuffer, length, next.Length);
                    length += next.Length;
                }
                packet = batchBuffer;
                packetLength = length;
                nPutBytes = 0;
                return true;
            }
        }

        /// <summary>
        /// Sends queued data of one socket (socket thread only). Returns true when a whole buffer went out and
        /// more may be waiting; false when the queue is empty or the socket would block (the rest is kept
        /// and sent on a later call, in order).
        /// </summary>
        public static bool TrySend(SecuritySocket _socket)
        {

            if (_socket.Alive)
            {
                if (!_socket.ConnectFull)
                    return false;
                int queued;
                lock (_socket.SendRoot)
                    queued = _socket.OnSend.Count;
                if (queued > 2500)
                {
                    Console.WriteLine("the sync is " + queued + "");
                    _socket.Disconnect();
                    return false;
                }
                if (_socket.packet == null && !_socket.NextSendBuffer())
                    return false;
                try
                {
                    SocketError sError = SocketError.Success;
                    int ret = _socket.Connection.Send(_socket.packet, _socket.nPutBytes, _socket.packetLength - _socket.nPutBytes, SocketFlags.None, out sError);
                    if (ret > 0)
                    {
                        _socket.nPutBytes += ret;//Connection are NonBlocking = true
                        if (_socket.nPutBytes >= _socket.packetLength)
                        {
                            _socket.packet = null;
                            _socket.nPutBytes = 0;
                            _socket.packetLength = 0;
                            return true;
                        }
                        // Partial send: the socket buffer is full; the rest goes out on a later call.
                        if (sError != SocketError.Success && sError != SocketError.WouldBlock)
                            _socket.Disconnect();
                        return false;
                    }
                    if (sError != SocketError.WouldBlock)
                        _socket.Disconnect();
                    return false;
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                    _socket.Disconnect();
                    return false;
                }
            }

            return false;
        }

        public object SynRoot = new object();
        public void Disconnect()
        {

            lock (SynRoot)
            {
                if (Alive)
                {
                    SocketPoll.ConnectionPoll.Remove(this);
                    lock (SendRoot)
                        OnSend.Clear();
                    if (Server != null)
                        Server.Clients.Remove(this);

                    Alive = false;
                    if (TimerSubscriptions != null)
                    {
                        for (int i = 0; i < TimerSubscriptions.Length; i++)
                            if (TimerSubscriptions[i] != null)
                                TimerSubscriptions[i].Dispose();
                    }
                    try
                    {

                        WindowsAPI.ws2_32.shutdown(Connection.Handle, WindowsAPI.ws2_32.ShutDownFlags.SD_BOTH);
                        WindowsAPI.ws2_32.closesocket(Connection.Handle);
                        Connection.Dispose();
                        GC.SuppressFinalize(Connection);
                    }
                    catch (Exception e)
                    {
                        MyConsole.SaveException(e);
                    }
                    finally
                    {
                        if (OnDisconnect != null)
                            OnDisconnect.Invoke(this);

                    }
                }
            }
        }

        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
    }

    /// <summary>
    /// Token buckets per packet category for one connection. Capacity = one second of packets, refilled
    /// continuously. Defaults are far above what normal play sends.
    /// </summary>
    public sealed class PacketTypeLimiter
    {
        public const int Walk = 0, Jump = 1, Attack = 2, Chat = 3, ItemUse = 4, Npc = 5, Action = 6, Count = 7;
        public static readonly string[] Names = { "walk", "jump", "attack", "chat", "item", "npc", "action" };
        public static readonly int[] Rates = new int[Count];
        public static readonly bool Enabled;
        private const ushort JumpActionType = 137; // MsgDataPacket ActionType.Jump

        static PacketTypeLimiter()
        {
            Rates[Walk] = Rate("PacketRateWalk", 40);
            Rates[Jump] = Rate("PacketRateJump", 15);
            Rates[Attack] = Rate("PacketRateAttack", 30);
            Rates[Chat] = Rate("PacketRateChat", 10);
            Rates[ItemUse] = Rate("PacketRateItem", 40);
            Rates[Npc] = Rate("PacketRateNpc", 20);
            Rates[Action] = Rate("PacketRateAction", 60);
            // Off by default (keeps the original behaviour); the per type values above apply only when enabled.
            Enabled = Conquer6609.Framework.Settings.Bool("Performance", "PacketRateLimits", false);
        }

        private static int Rate(string key, int def)
        {
            return Math.Max(0, Conquer6609.Framework.Settings.Int("Performance", key, def));
        }

        /// <summary>Category of a client packet, or -1 when it is not limited per type.</summary>
        public static unsafe int Classify(ushort packetId, Packet stream)
        {
            switch (packetId)
            {
                case Game.GamePackets.Movement: return Walk;
                case Game.GamePackets.Attack: return Attack;
                case Game.GamePackets.Chat: return Chat;
                case Game.GamePackets.Usage: return ItemUse;
                case Game.GamePackets.NpcServerRequest:
                case Game.GamePackets.NpcServerReplay: return Npc;
                case Game.GamePackets.DataMap:
                    // ActionQuery.Type sits at offset 24 of the packet
                    if (stream != null && stream.Size >= 26 && *(ushort*)(stream.Memory + 24) == JumpActionType)
                        return Jump;
                    return Action;
            }
            return -1;
        }

        private readonly int[] tokens = new int[Count];   // thousandths of a packet
        private readonly int[] lastTick = new int[Count];
        private readonly bool[] started = new bool[Count];

        public bool Allow(int category)
        {
            int rate = Rates[category];
            if (rate <= 0)
                return true;
            int capacity = rate * 1000;
            int now = Environment.TickCount;
            if (!started[category])
            {
                started[category] = true;
                tokens[category] = capacity;
            }
            else
            {
                int elapsed = unchecked(now - lastTick[category]);
                if (elapsed < 0 || elapsed > 1000)
                    elapsed = 1000;
                long refill = (long)tokens[category] + (long)elapsed * rate;
                tokens[category] = refill > capacity ? capacity : (int)refill;
            }
            lastTick[category] = now;
            if (tokens[category] >= 1000)
            {
                tokens[category] -= 1000;
                return true;
            }
            return false;
        }
    }
}
