// GM accounts without renaming characters or typing a password in chat.
// shell.ini:
//   [GameMasters]
//   UIDs=10212285,10212286          ; character UIDs (= accounts.EntityID)
//   AllowedIPs=default              ; GM only from these IPs. default = 127.0.0.1 + the server's own IP
//                                   ; (the PC that hosts the server). "any" = from everywhere.
using System;
using System.Collections.Generic;

namespace Conquer6609.Database
{
    public static class GameMasters
    {
        static HashSet<uint> uids;
        static HashSet<string> ips;
        static bool anyIp;

        static void Load()
        {
            if (uids != null) return;
            var u = new HashSet<uint>();
            var i = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                // IniFile(string) prepends DbLocation to the path, so set FileName directly (same as SaveDatabase does)
                var ini = new WindowsAPI.IniFile();
                ini.FileName = System.IO.Directory.GetCurrentDirectory() + "\\shell.ini";
                foreach (var s in ini.ReadString("GameMasters", "UIDs", "", 1024).Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    uint id;
                    if (uint.TryParse(s.Trim(), out id)) u.Add(id);
                }
                string list = ini.ReadString("GameMasters", "AllowedIPs", "default", 512).Trim();
                if (list.Equals("any", StringComparison.OrdinalIgnoreCase)) anyIp = true;
                else
                {
                    if (list.Equals("default", StringComparison.OrdinalIgnoreCase)) list = "127.0.0.1," + (Program.ServerConfig.IPAddres ?? "");
                    foreach (var s in list.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)) i.Add(s.Trim());
                }
            }
            catch (Exception e) { MyConsole.WriteLine("[GM] shell.ini [GameMasters]: " + e.Message); }
            ips = i;
            uids = u;
            MyConsole.WriteLine("[GM] " + u.Count + " GM account(s), allowed from: " + (anyIp ? "any IP" : string.Join(", ", i)), ConsoleColor.Yellow);
        }

        /// <summary>Called once at CompleteLogin.</summary>
        public static void Apply(Client.GameClient client)
        {
            try
            {
                Load();
                if (client == null || client.Player == null || !uids.Contains(client.Player.UID)) return;
                string ip = client.Socket != null ? (client.Socket.RemoteIp ?? "").Trim() : "";
                if (!anyIp && !ips.Contains(ip))
                {
                    MyConsole.WriteLine("[GM] " + client.Player.Name + " is a GM account but logged in from " + ip + " - GM not granted.", ConsoleColor.Red);
                    return;
                }
                client.GameMaster = true;
                client.ProjectManager = true;
                MyConsole.WriteLine("[GM] " + client.Player.Name + " (UID " + client.Player.UID + ") is GM.", ConsoleColor.Yellow);
                client.SendSysMesage("[GM] GM mode is ON for " + client.Player.Name + ". Try: @online", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
    }
}
