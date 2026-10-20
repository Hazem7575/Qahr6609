// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AccServer.Network.Sockets
{
    public unsafe class ServerSocket
    {
        public event Action<ClientWrapper> OnClientConnect, OnClientDisconnect;
        public event Action<byte[], int, ClientWrapper> OnClientReceive;
       
        private object SyncRoot;
        private Socket Connection;
        private ushort port;
        private string ipString;
        private bool enabled;
        private System.Threading.Thread thread;
        public ServerSocket()
        {
            this.Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.SyncRoot = new object();
            thread = new System.Threading.Thread(doSyncAccept);
            thread.Start();
        }
        public void Enable(ushort port, string ip)
        {
            this.ipString = ip;
           this.port = port;
            this.Connection.Bind(new IPEndPoint(IPAddress.Parse(ipString), this.port));
            this.Connection.Listen(50);
            this.enabled = true;
            
        }
        public bool PrintoutIPs = true;
        private void doSyncAccept()
        {
            while (true)
            {
                if (this.enabled)
                {
                    try
                    {
                        processSocket(this.Connection.Accept());
                    }
                    catch
                    {
                        // listener closed or failing: don't spin a CPU core on the error
                        System.Threading.Thread.Sleep(10);
                    }
                }
                else
                    System.Threading.Thread.Sleep(10);// not listening yet (or disabled): wait instead of spinning
            }
        }
        private void doAsyncAccept(IAsyncResult res)
        {
            try
            {
                Socket socket = this.Connection.EndAccept(res);
                processSocket(socket);
                this.Connection.BeginAccept(doAsyncAccept, null);
            }
            catch
            {
            }
        }
        private void processSocket(Socket socket)
        {
            try
            {
                string ip = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();

                try { socket.NoDelay = true; }
                catch { }
                ClientWrapper wrapper = new ClientWrapper();
                wrapper.Create(socket, this, OnClientReceive);
                wrapper.Alive = true;
                wrapper.IP = ip;
                if (this.OnClientConnect != null) this.OnClientConnect(wrapper);
            }
            catch
            {
            }
        }
        public void Reset()
        {
            this.Disable();
            this.Enable();
        }
        public void Disable()
        {
            this.enabled = false;
            this.Connection.Close(1);
        }
        public void Enable()
        {
            if (!this.enabled)
            {
                this.Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.Connection.Bind(new IPEndPoint(IPAddress.Parse(ipString), this.port));
                this.Connection.Listen((int)SocketOptionName.MaxConnections);
                this.enabled = true;
            }
        }
        public void InvokeDisconnect(ClientWrapper Client)
        {
            if (this.OnClientDisconnect != null)
                this.OnClientDisconnect(Client);
        }
        public bool Enabled
        {
            get { return this.enabled; }
        }
    }
}