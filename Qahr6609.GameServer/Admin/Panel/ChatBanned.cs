using Conquer6609.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conquer6609
{
    class ChatBanned
    {
        public static void AddChatBan(string Name, int Hours, bool Permenant = false)
        {
            DateTime Time = DateTime.Now;
            uint UID = 0;
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;
                string RName = ini.ReadString("Character", "Name", "None");
                if (RName.GetHashCode() == Name.GetHashCode())
                {
                    UID = ini.ReadUInt32("Character", "UID", 0);
                    break;
                }

            }
            try
            {

                Time = DateTime.Now.AddHours(Hours);
            }
            catch
            {
                Permenant = true;
            }
            Client.GameClient clienttoban = null;
            if (Server.GamePoll.TryGetValue(UID, out clienttoban))
            {
                if (!Permenant)
                {
                    clienttoban.Player.BannedChatStamp = Time;
                }
                else
                {
                    clienttoban.Player.PermenantBannedChat = Permenant;
                }
                clienttoban.Player.IsBannedChat = true;
            }
            else
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
                write.Write<bool>("Character", "IsBannedChat", true);
                if (!Permenant)
                {
                    write.Write<long>("Character", "BannedChatStamp", Time.ToBinary());
                }
                else
                {
                    write.Write<bool>("Character", "PermenantBannedChat", Permenant);
                }
            }
        }
        public static void RemoveChatBan(string Name)
        {

            uint UID = 0;
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;

                string RName = ini.ReadString("Character", "Name", "None");
                if (RName.GetHashCode() == Name.GetHashCode())
                {
                    UID = ini.ReadUInt32("Character", "UID", 0);
                    break;
                }

            }
            Client.GameClient clienttoban = null;
            if (Server.GamePoll.TryGetValue(UID, out clienttoban))
            {
                clienttoban.Player.BannedChatStamp = DateTime.Now;
                clienttoban.Player.IsBannedChat = false;
                clienttoban.Player.PermenantBannedChat = false;
                Console.WriteLine("Player " + clienttoban.Player.Name + " UnBanned Chat.", ConsoleColor.DarkRed);
            }
            else
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
                write.Write<bool>("Character", "IsBannedChat", false);
                write.Write<long>("Character", "BannedChatStamp", DateTime.Now.ToBinary());
                write.Write<bool>("Character", "PermenantBannedChat", false);
                Console.WriteLine("Player " + clienttoban.Player.Name + " In Database UnBanned Chat.", ConsoleColor.DarkRed);
            }
        }
        public static void Cheek(Client.GameClient User)
        {
            if (DateTime.Now > User.Player.BannedChatStamp && User.Player.IsBannedChat)
            {
                User.Player.IsBannedChat = false;
            }
        }

    }
}
