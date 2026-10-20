
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Conquer6609.ServerSockets
{
    public class SocketPoll
    {
        const int FD_SETSIZE = 2048;
        // [Performance] SocketLoopMs: interval of the full receive/handle/send pass (was 20 ms).
        static readonly int SOCKET_PROCESS_INTERVAL = Math.Max(1, Math.Min(100, Conquer6609.Framework.Settings.Int("Performance", "SocketLoopMs", 10)));

        public static Extensions.MyList<SecuritySocket> ConnectionPoll = new Extensions.MyList<SecuritySocket>();

        private static ServerSocket[] Sockets;

        // Sockets whose send queue went from empty to non-empty, and the event that wakes the socket thread
        // for them. Between two full passes the socket thread waits on this event and flushes these sockets
        // immediately, so replies/broadcasts made by other threads do not wait for the next pass.
        private static readonly ConcurrentQueue<SecuritySocket> PendingSend = new ConcurrentQueue<SecuritySocket>();
        private static readonly AutoResetEvent SendWake = new AutoResetEvent(false);
        private static readonly System.Diagnostics.Stopwatch Clock = System.Diagnostics.Stopwatch.StartNew();

        public SocketPoll(string GroupName, params ServerSocket[] _Sockets)
        {
            Sockets = _Sockets;
            var ThreadItem = new Extensions.ThreadGroup.ThreadItem(SOCKET_PROCESS_INTERVAL, GroupName, CheckUp, WaitAndFlush);
            ThreadItem.Open();
        }

        /// <summary>Called by SecuritySocket.Send when the socket's queue goes from empty to non-empty.</summary>
        public static void NotifySend(SecuritySocket socket)
        {
            PendingSend.Enqueue(socket);
            SendWake.Set();
        }

        // ThreadItem's sleep hook: instead of sleeping `ms`, wait on SendWake and flush pending sends
        // until the time of the next full pass.
        private static void WaitAndFlush(int ms)
        {
            long deadline = Clock.ElapsedMilliseconds + ms;
            while (true)
            {
                FlushPendingSends();
                long remaining = deadline - Clock.ElapsedMilliseconds;
                if (remaining <= 0)
                    break;
                if (!SendWake.WaitOne((int)remaining))
                    break;
            }
            FlushPendingSends();
        }

        private static void FlushPendingSends()
        {
            SecuritySocket socket;
            int budget = 4096;
            while (budget-- > 0 && PendingSend.TryDequeue(out socket))
            {
                // cleared before sending: a Send() after this point queues the socket again
                Interlocked.Exchange(ref socket.SendNotified, 0);
                try
                {
                    while (SecuritySocket.TrySend(socket)) ;
                }
                catch (Exception e)
                {
                    MyConsole.WriteException(e);
                }
            }
        }

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint timeBeginPeriod(uint uPeriod);

        /// <summary>[Performance] TimerResolution1ms = 1 asks Windows for a 1 ms timer (more precise sleeps,
        /// slightly more CPU). Default 0 = unchanged.</summary>
        public static void ApplyTimerResolution()
        {
            try
            {
                if (Conquer6609.Framework.Settings.Bool("Performance", "TimerResolution1ms", false))
                    timeBeginPeriod(1);
            }
            catch { }
        }

        public static void CheckUp()
        {
            try
            {
                if (ConnectionPoll.Count > 0 || Sockets.Length > 0)
                {
                    foreach (var socket in ConnectionPoll.GetValues())
                    {
                        socket.CheckUp();

                        if (!(socket.Alive && socket.Connection.Connected))
                            socket.Disconnect();
                    }

                    foreach (var socket in ConnectionPoll.GetValues())
                    {
                        {
                            try
                            {
                                socket.ReceiveBuffer();
                                socket.HandlerBuffer();

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                // A connection whose buffer handling fails is dropped instead of failing on every pass.
                                try { socket.Disconnect(); } catch { }
                                continue;
                            }
                        }
                        try
                        {
                            while (SecuritySocket.TrySend(socket)) ;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }

                    foreach (var socket in Sockets)
                    {
                        if (socket == null)
                            continue;

                        {
                            socket.Accept();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}

