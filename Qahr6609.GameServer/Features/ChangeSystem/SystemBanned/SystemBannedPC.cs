using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class SystemBannedPC
    {
        public static System.Collections.Concurrent.ConcurrentDictionary<string, Client> BannedPoll = new System.Collections.Concurrent.ConcurrentDictionary<string, Client>();
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

        public static bool AddBan(Conquer6609.Client.GameClient client, string Reason = "")
        {
            if (client == null || string.IsNullOrEmpty(client.TqSerial))
                return false;

            string serial = client.TqSerial.Trim();

            if (!BannedPoll.ContainsKey(serial))
            {
                var msg = new Client
                {
                    TqSerial = serial,
                    PlayerName = client.Player?.Name ?? "Unknown",
                    UID = client.Player?.UID ?? 0
                };

                if (!BannedPoll.TryAdd(serial, msg))
                    return false;
                Save();

                Console.WriteLine($"[BAN] Added new banned device: {msg.PlayerName} | TQSerial={msg.TqSerial}");
                return true;
            }
            return false;
        }

        public static bool RemoveBan(string playerName)
        {
            var banned = BannedPoll.Values.FirstOrDefault(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            if (banned == null)
                return false;

            Client removed;
            if (!BannedPoll.TryRemove(banned.TqSerial, out removed))
                return false;
            Save();

            Console.WriteLine($"[UNBAN] Removed ban for {playerName}");
            return true;
        }

        public static bool IsBanned(Conquer6609.Client.GameClient client, out string message)
        {
            string tqserial = client.TqSerial?.Trim();

            bool banned = !string.IsNullOrEmpty(tqserial) && BannedPoll.ContainsKey(tqserial);

            if (banned)
            {
                message = "You Are Banned For (25) Years";
                Console.WriteLine($"[BAN-DETECTED] Blocked attempt from {client.Player?.Name} | TQSerial={tqserial}");
                return true;
            }

            message = "";
            return false;
        }

        public static void Load()
        {
            using (Database.DBActions.Read reader = new Database.DBActions.Read("BannedPC.txt"))
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

                        if (!BannedPoll.ContainsKey(msg.TqSerial))
                            BannedPoll.TryAdd(msg.TqSerial, msg);

                        Console.WriteLine($"[BANNED] {msg.PlayerName} | TQSerial={msg.TqSerial}");
                    }
                }
            }

        }

        public static void Save()
        {
            lock (saveSync)
            using (Database.DBActions.Write writer = new Database.DBActions.Write("BannedPC.txt"))
            {
                foreach (var ban in BannedPoll.Values)
                    writer.Add(ban.ToString());

                writer.Execute(Database.DBActions.Mode.Open);
            }
        }
    }
}
