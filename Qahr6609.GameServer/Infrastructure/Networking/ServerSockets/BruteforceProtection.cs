using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Conquer6609.ServerSockets
{
    public class BruteForceEntry
    {
        public string IPAddress;
        public int WatchCheck;
        public Extensions.Time32 Unbantime;
        public Extensions.Time32 AddedTimeRemove;
    }

    public class BruteforceProtection
    {
        private Extensions.SafeDictionary<string, BruteForceEntry> collection = new Extensions.SafeDictionary<string, BruteForceEntry>();
        private int BanOnWatch;


        private void _internalInit()
        {
            Extensions.Time32 Now;
            while (true)
            {

                Now = Extensions.Time32.Now;
                try
                {
                    // All access to the collection is serialized on the same lock as AddWatch/IsBanned.
                    lock (collection)
                    {
                        foreach (BruteForceEntry bfe in collection.GetValues().ToArray())
                        {
                            if (bfe.AddedTimeRemove <= Now)
                            {
                                collection.Remove(bfe.IPAddress);
                            }
                            else if (bfe.Unbantime.Value != 0)
                            {
                                if (bfe.Unbantime.Value <= Now.Value)
                                {
                                    collection.Remove(bfe.IPAddress);
                                }
                            }
                        }
                    }
                }
                catch (Exception e) { MyConsole.SaveException(e); }

                Thread.Sleep(2000);
            }
        }

        public void Init(int WatchBeforeBan)
        {
            BanOnWatch = WatchBeforeBan;
            new Thread(new ThreadStart(_internalInit)) { IsBackground = true, Name = "BruteforceProtection" }.Start();
        }

        public void AddWatch(string IPAddress)
        {
            lock (collection)
            {
                BruteForceEntry bfe;
                if (!collection.TryGetValue(IPAddress, out bfe))
                {
                    bfe = new BruteForceEntry();
                    bfe.IPAddress = IPAddress;
                    bfe.WatchCheck = 1;
                    bfe.AddedTimeRemove = Extensions.Time32.Now.AddMinutes(3);
                    bfe.Unbantime = new Extensions.Time32(0);
                    collection.Add(IPAddress, bfe);
                }
                else
                {
                    bfe.WatchCheck++;
                    if (bfe.WatchCheck >= BanOnWatch)
                    {
                        bfe.Unbantime = Extensions.Time32.Now.AddMinutes(3);
                    }
                }
            }
        }
        public bool AllowAddress(string IPAddress)
        {
            foreach (var server in Database.GroupServerList.GroupServers.Values)
                if (server.IPAddress == IPAddress)
                    return true;
            return false;
        }
        public bool IsBanned(string IPAddress)
        {
            bool check = false;
            BruteForceEntry bfe;
            lock (collection)
            {
                if (collection.TryGetValue(IPAddress, out bfe))
                {
                    check = (bfe.Unbantime.Value != 0);
                }
            }
            return check;
        }
    }
}
