
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Concurrent;

namespace Conquer6609.ServerSockets
{
    public class ServerSocket
    {
        private bool Alive = false;
        private Socket Connection;

        public Socket GetConnection
        {
            get { return Connection; }
        }
        public bool IsAlive
        {
            get { return Alive; }
        }

        private string ServerAddresIP = "";
        public uint SPort;
        public string ServerName = "";
        private Action<SecuritySocket> ProcessConnection, ProcessDisconnect;
        private Action<SecuritySocket, Packet> ProcessReceive;
        private BruteforceProtection Bruteforce;
        public Extensions.MyList<SecuritySocket> Clients = new Extensions.MyList<SecuritySocket>();
        public ServerSocket(Action<SecuritySocket> _processConnection, Action<SecuritySocket, Packet> _procesreceive, Action<SecuritySocket> _processdisconnect)
        {
            ProcessConnection = _processConnection;
            ProcessReceive = _procesreceive;
            ProcessDisconnect = _processdisconnect;
            Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Initilize(ushort MaxBufferSend, ushort MaxBufferReceive, uint MaxAcceptConnections, uint MaxClientsConnections)
        {
            Connection.ReceiveBufferSize = MaxBufferReceive;
            Connection.SendBufferSize = MaxBufferSend;
        }

        public void Connect(string IpAddres, ushort port, string aServerName)
        {
            SPort = port;
            ServerAddresIP = IpAddres;
            ServerName = aServerName;
            TryConnect(aServerName);
        }
     
        private void TryConnect(string servername)
        {
           
            Connection.BeginConnect(ServerAddresIP, (int)SPort, new AsyncCallback(ConnectCallback), null);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Connection.EndConnect(ar);

                Console.WriteLine("Socket connected to " + Connection.RemoteEndPoint.ToString() + " successful !");

                Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Connection.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                Connection.Blocking = false;
                Connection.ReceiveBufferSize = 16384;
                Connection.SendBufferSize = 16384;

                Alive = Connection.Connected;
                if (Alive)
                {
                    var client = new SecuritySocket(this, ProcessDisconnect, ProcessReceive);
                    client.Create(Connection);
                    client.Connection.Blocking = false;
                    if (ProcessConnection != null)
                    {
                        ProcessConnection(client);

                    }
                }
            }
            catch
            {

                TryConnect(ServerName);
            }
        }

        public void Open(string IpAddres, ushort port, int backlog)
        {
            Open(IpAddres, port, backlog, null);
        }
        /// <summary>bindAddress = local interface to listen on (null/empty/"0.0.0.0" = every interface, the old behaviour).</summary>
        public void Open(string IpAddres, ushort port, int backlog, string bindAddress)
        {
            Bruteforce = new BruteforceProtection();
            Bruteforce.Init(10);

            SPort = port;
            ServerAddresIP = IpAddres;
            IPAddress bind = IPAddress.Any;
            if (!string.IsNullOrEmpty(bindAddress) && !IPAddress.TryParse(bindAddress.Trim(), out bind))
                throw new ArgumentException("bind address '" + bindAddress + "' is not an IP address");
            Connection.Bind(new IPEndPoint(bind, port));

            // Listen backlog comes from shell.ini [InternetPort] BackLog.
            Connection.Listen(backlog > 0 ? backlog : 100);
            Connection.Blocking = false;
            Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Connection.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            Console.WriteLine("Listening on port : " + port.ToString());
            Alive = true;
        }
        // At most this many pending connections are accepted per socket pass.
        private const int MaxAcceptsPerPass = 16;
        public void Accept()
        {
            for (int i = 0; i < MaxAcceptsPerPass; i++)
            {
                if (!AcceptOne())
                    break;
            }
        }
        // Accepts one pending connection; returns false when none was waiting (or on error).
        private bool AcceptOne()
        {

            try
            {
                if (Alive)
                {
                    if (Connection.Poll(0, SelectMode.SelectRead))
                    {

                        var socket = Connection.Accept();
                        try { socket.NoDelay = true; }
                        catch { }


                        string RemoteIP = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();



                        if (!Bruteforce.IsBanned(RemoteIP) || Bruteforce.AllowAddress(RemoteIP))
                        {

                            SecuritySocket user = new SecuritySocket(this, ProcessDisconnect, ProcessReceive);
                            user.Create(socket);
                            Clients.Add(user);

                            if (ProcessConnection != null)
                            {

                                ProcessConnection.Invoke(user);
                            }


                            user.ConnectFull = true;
                            Bruteforce.AddWatch(user.RemoteIp);
                        }
                        else
                        {
                            CloseNewSocket(socket);
                        }
                        return true;
                    }
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return false;

        }
        public void CloseNewSocket(Socket socket)
        {
            WindowsAPI.ws2_32.shutdown(socket.Handle, WindowsAPI.ws2_32.ShutDownFlags.SD_BOTH);
            WindowsAPI.ws2_32.closesocket(socket.Handle);
            socket.Dispose();
            GC.SuppressFinalize(socket);
        }



        public void Close()
        {
            if (Alive)
            {

                Alive = false;
                Connection.Close(1);

            }
        }
    }
}
