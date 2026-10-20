using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class UnLockLogin
    {
        public static System.Collections.Concurrent.ConcurrentDictionary<string, Client> UnLockLoginPoll = new System.Collections.Concurrent.ConcurrentDictionary<string, Client>();
        private static readonly object saveSync = new object();

        public class Client
        {
            public string TqSerial;
            public string PlayerName;
            public uint UID;

            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                writer.Add(TqSerial)
                      .Add(PlayerName)
                      .Add(UID.ToString());
                return writer.Close();
            }
        }

        public static bool AddLockLog(Conquer6609.Client.GameClient client)
        {
            if (client == null || string.IsNullOrEmpty(client.TqSerial))
                return false;

            string serial = client.TqSerial.Trim();
            if (!UnLockLoginPoll.ContainsKey(serial))
            {
                var msg = new Client
                {
                    TqSerial = serial,
                    PlayerName = client.Player?.Name ?? "Unknown",
                    UID = client.Player?.UID ?? 0
                };

                if (!UnLockLoginPoll.TryAdd(serial, msg))
                    return false;
                Save();

                Console.WriteLine($"[UNLOCK] Added new unlocked device: {msg.PlayerName} | TQSerial={msg.TqSerial}");
                return true;
            }
            return false;
        }

        public static bool RemoveLock(string playerName)
        {
            var client = UnLockLoginPoll.Values.FirstOrDefault(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            if (client == null)
                return false;

            Client removed;
            if (!UnLockLoginPoll.TryRemove(client.TqSerial, out removed))
                return false;
            Save();

            Console.WriteLine($"[UNLOCK-REMOVED] Removed unlock for {playerName}");
            return true;
        }

        public static bool IsUnlocked(Conquer6609.Client.GameClient client, out string message)
        {
            string serial = client.TqSerial?.Trim();
            bool unlocked = !string.IsNullOrEmpty(serial) && UnLockLoginPoll.ContainsKey(serial);

            if (unlocked)
            {
                message = "This device is already unlocked!";
                return true;
            }

            message = "";
            return false;
        }

        public static void Load()
        {
            using (Database.DBActions.Read reader = new Database.DBActions.Read("UnLockLogin.txt"))
            {
                if (reader.Reader())
                {
                    uint count = (uint)reader.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine readline = new Database.DBActions.ReadLine(reader.ReadString(""), '/');
                        Client msg = new Client
                        {
                            TqSerial = readline.Read(""),
                            PlayerName = readline.Read(""),
                            UID = uint.TryParse(readline.Read(""), out uint id) ? id : 0
                        };

                        if (!UnLockLoginPoll.ContainsKey(msg.TqSerial))
                            UnLockLoginPoll.TryAdd(msg.TqSerial, msg);
                    }
                }
            }
        }

        public static void Save()
        {
            lock (saveSync)
            using (Database.DBActions.Write writer = new Database.DBActions.Write("UnLockLogin.txt"))
            {
                foreach (var client in UnLockLoginPoll.Values)
                    writer.Add(client.ToString());

                writer.Execute(Database.DBActions.Mode.Open);
            }
        }
    }
}
