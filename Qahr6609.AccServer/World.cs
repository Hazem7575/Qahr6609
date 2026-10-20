using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Generic;
using AccServer.Network.Sockets;

namespace AccServer
{
    public unsafe class World
    {
        public static StaticPool ReceivePool, SendPool;
        public TimerRule<ClientWrapper> ConnectionReceive, ConnectionReview, ConnectionSend;

        public void Init()
        {
            ConnectionReview = new TimerRule<Network.Sockets.ClientWrapper>(connectionReview, 1000, ThreadPriority.Lowest);
            // Receive/send are polled every 10 ms (was 1 ms, which Windows rounds to ~15 ms anyway): a login
            // is a few round trips, so this adds at most a few tens of ms to it and saves a lot of CPU.
            ConnectionReceive = new TimerRule<Network.Sockets.ClientWrapper>(connectionReceive, 10, ThreadPriority.Highest);
            ConnectionSend = new TimerRule<Network.Sockets.ClientWrapper>(connectionSend, 10, ThreadPriority.Highest);
        }
        public World()
        {
            DisableQuickEdit();
            // Workers now block until the pool has work (see StaticPool), so a few threads are enough:
            // receive handlers do the login database work, sends are short.
            ReceivePool = new StaticPool(8).Run();
            SendPool = new StaticPool(4).Run();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// <summary>Turns off console QuickEdit (a click in the window would pause the server until a key
        /// is pressed). Ignored when there is no console.</summary>
        public static void DisableQuickEdit()
        {
            try
            {
                IntPtr h = GetStdHandle(-10); // STD_INPUT_HANDLE
                uint mode;
                if (h == IntPtr.Zero || h == new IntPtr(-1) || !GetConsoleMode(h, out mode))
                    return;
                mode &= ~0x0040u; // ENABLE_QUICK_EDIT_MODE
                mode |= 0x0080u;  // ENABLE_EXTENDED_FLAGS
                SetConsoleMode(h, mode);
            }
            catch { }
        }

        private void connectionReview(Network.Sockets.ClientWrapper wrapper, int time)
        {
            Network.Sockets.ClientWrapper.TryReview(wrapper);
        }
        private void connectionReceive(Network.Sockets.ClientWrapper wrapper, int time)
        {
            Network.Sockets.ClientWrapper.TryReceive(wrapper);
        }
        private void connectionSend(Network.Sockets.ClientWrapper wrapper, int time)
        {
            Network.Sockets.ClientWrapper.TrySend(wrapper);
        }

        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param, StaticPool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param, StandalonePool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
       
    }
}