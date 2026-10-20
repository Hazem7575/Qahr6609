using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class Allow_Clicker
    {
        public static System.Collections.Concurrent.ConcurrentDictionary<string, Client> AllowClickerPoll = new System.Collections.Concurrent.ConcurrentDictionary<string, Client>();
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

        public static bool AddAllowClicker(Conquer6609.Client.GameClient client)
        {
            if (client == null || string.IsNullOrEmpty(client.TqSerial))
                return false;

            string serial = client.TqSerial.Trim();
            if (!AllowClickerPoll.ContainsKey(serial))
            {
                var msg = new Client
                {
                    TqSerial = serial,
                    PlayerName = client.Player?.Name ?? "Unknown",
                    UID = client.Player?.UID ?? 0
                };

                if (!AllowClickerPoll.TryAdd(serial, msg))
                    return false;
                Save();

                Console.WriteLine($"[ALLOW_CLICKER] Added new device: {msg.PlayerName} | TQSerial={msg.TqSerial}");
                return true;
            }
            return false;
        }

        public static bool RemoveAllowClicker(string playerName)
        {
            var client = AllowClickerPoll.Values.FirstOrDefault(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            if (client == null)
                return false;

            Client removed;
            if (!AllowClickerPoll.TryRemove(client.TqSerial, out removed))
                return false;
            Save();

            Console.WriteLine($"[ALLOW_CLICKER-REMOVED] Removed device for {playerName}");
            return true;
        }

        public static bool IsAllowed(Conquer6609.Client.GameClient client)
        {
            string serial = client.TqSerial?.Trim();
            bool allowed = !string.IsNullOrEmpty(serial) && AllowClickerPoll.ContainsKey(serial);

            if (allowed)
            {
              // message = "This device is allowed for clicker!";
                return true;
            }

            //message = "";
            return false;
        }

        public static void Load()
        {
            using (Database.DBActions.Read reader = new Database.DBActions.Read("Allow_Clicker.txt"))
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

                        if (!AllowClickerPoll.ContainsKey(msg.TqSerial))
                            AllowClickerPoll.TryAdd(msg.TqSerial, msg);
                    }
                }
            }
        }

        public static void Save()
        {
            lock (saveSync)
            using (Database.DBActions.Write writer = new Database.DBActions.Write("Allow_Clicker.txt"))
            {
                foreach (var client in AllowClickerPoll.Values)
                    writer.Add(client.ToString());

                writer.Execute(Database.DBActions.Mode.Open);
            }
        }
    }
}
