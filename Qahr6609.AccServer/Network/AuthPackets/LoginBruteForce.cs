



using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using AccServer;

namespace MoDY.Network.Sockets
{
    public unsafe class LoginBruteForce
    {
        public class ClientLogin
        {
            public string IPAdres;
            public Time32 Timer;
            public uint Trying;
        }
        public static object SyncRoot;
        public static void CreatePoll()
        {
            SyncRoot = new object();
            World.Subscribe(new Action<int>(Work), 500, ThreadPriority.Normal);
        }
        public static void Work(int Time)
        {
            ConcurrentQueue<int> concurrentQueue = new ConcurrentQueue<int>();
            Time32 now = Time32.Now;
            foreach (KeyValuePair<int, ClientLogin> current in Registered)
            {
                if (now > current.Value.Timer.AddSeconds(30))
                {
                    if (current.Value.Trying > 0)
                    {
                        current.Value.Timer = Time32.Now;
                        current.Value.Trying -= 1;
                    }
                    else
                    {
                        concurrentQueue.Enqueue(current.Key);
                    }
                }
            }
            int key = 0;
            while (concurrentQueue.TryDequeue(out key))
            {
                ClientLogin clientLogin;
                Registered.TryRemove(key, out clientLogin);
            }
        }
        public static bool AcceptJoin(string Adres)
        {
            int hashCode = Adres.GetHashCode();
            ClientLogin clientLogin;
            return !Registered.TryGetValue(hashCode, out clientLogin) || clientLogin.Trying < 5;
        }
        public static void ClientRegistred(string Adrres)
        {
            lock (SyncRoot)
            {
                int hashCode = Adrres.GetHashCode();
                ClientLogin clientLogin;
                if (Registered.TryGetValue(hashCode, out clientLogin))
                {
                    clientLogin.Trying += 1;
                    clientLogin.Timer = Time32.Now;
                }
                else
                {
                    clientLogin = new ClientLogin();
                    clientLogin.IPAdres = Adrres;
                    clientLogin.Timer = Time32.Now;
                    clientLogin.Trying = 1;
                    Registered.TryAdd(hashCode, clientLogin);
                }
            }
        }
        public static ConcurrentDictionary<int, ClientLogin> Registered = new ConcurrentDictionary<int, ClientLogin>();
    }
}