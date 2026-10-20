using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609.Database
{
    public static class ShareVIP
    {
        public static Extensions.MyList<Client> SharedPoll = new Extensions.MyList<Client>();
        public class Client
        {
            public uint UID;
            public uint ShareUID;
            public string ShareName;
            public byte ShareLevel;
            public DateTime ShareEnds;
            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new DBActions.WriteLine('/');
                writer.Add(UID).Add(ShareUID).Add(ShareName).Add(ShareLevel).Add(ShareEnds.Ticks);
                return writer.Close();
            }
        }
        /// <summary>A share is active only until ShareEnds.</summary>
        public static bool IsActive(Client x)
        {
            return x != null && x.ShareEnds > DateTime.Now;
        }

        /// <summary>Removes every share whose end date has passed. Returns the number removed.</summary>
        public static int PurgeExpired()
        {
            int removed = 0;
            foreach (var x in SharedPoll.GetValues().ToArray())
            {
                if (!IsActive(x))
                {
                    SharedPoll.Remove(x);
                    removed++;
                }
            }
            return removed;
        }

        /// <summary>Active (not expired) shares only.</summary>
        public static List<Client> GetActive()
        {
            return SharedPoll.GetValues().Where(p => IsActive(p)).ToList();
        }

        /// <summary>The active share received by ShareUID, or null. Expired shares are purged and never returned.</summary>
        public static Client GetActiveShare(uint ShareUID)
        {
            Client found = null;
            foreach (var x in SharedPoll.GetValues().ToArray())
            {
                if (x.ShareUID != ShareUID)
                    continue;
                if (!IsActive(x))
                {
                    SharedPoll.Remove(x);
                    continue;
                }
                if (found == null)
                    found = x;
            }
            return found;
        }

        public static bool CanShare(uint UID, uint ShareUID)
        {
            if (UID == 0 || ShareUID == 0 || UID == ShareUID)
                return false;
            PurgeExpired();
            foreach (var p in SharedPoll.GetValues())
            {
                if (p.UID == UID || p.ShareUID == UID || p.UID == ShareUID || p.ShareUID == ShareUID)
                    return false;
            }
            return true;
        }
        public static bool Add(Client x)
        {
            if (x == null || !IsActive(x))
                return false;
            lock (SharedPoll)
            {
                if (CanShare(x.UID, x.ShareUID))
                {
                    SharedPoll.Add(x);
                    return true;
                }
            }
            return false;
        }
        public static void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("Share.txt"))
            {
                PurgeExpired();
                foreach (var x in SharedPoll.GetValues())
                {
                    writer.Add(x.ToString());
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }
        public static void Load()
        {
            using (Database.DBActions.Read Reader = new DBActions.Read("Share.txt"))
            {
                if (Reader.Reader())
                {
                    uint count = (uint)Reader.Count;
                    for (uint i = 0; i < count; i++)
                    {
                        DBActions.ReadLine readline = new DBActions.ReadLine(Reader.ReadString(""), '/');
                        Client x = new Client();
                        x.UID = readline.Read((uint)0);
                        x.ShareUID = readline.Read((uint)0);
                        x.ShareName = readline.Read("");
                        x.ShareLevel = readline.Read((byte)0);
                        x.ShareEnds = DateTime.FromBinary(readline.Read((long)0));
                        if (IsActive(x) && x.UID != x.ShareUID)
                            SharedPoll.Add(x);
                    }
                }
            }
        }
    }
}
