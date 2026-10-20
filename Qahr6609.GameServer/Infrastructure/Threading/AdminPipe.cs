// Local admin channel for the control panel (Qahr6609.Tools\Server\Cluster\cluster.ps1): graceful stop / save / status.
// Named pipe \\.\pipe\hazem-admin-<process id>; only the Windows account that runs the server may connect,
// network logons are denied. One text line per connection, one reply line:
//   ping  -> "ok name=<ServerName> id=<ServerID> role=game|realm online=<n> pid=<pid> ram=<MB>"
//   save  -> saves the database and every online character (same as the console "save"), replies "saved"
//   exit  -> replies "stopping", then the same shutdown as Ctrl+C (players disconnected and saved) and exits
//   reloadnpcs | reloadportals | reloadshops | reloaditems | reloadmonsters | reloadmapcells <map> | reloadquests |
//   reloadall [force] -> live reload of the data file (HotReload), replies "ok <summary>" or "error <reason>"
// shell.ini [Admin] Pipe=0 switches it off (then the panel can only kill the process).
using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Conquer6609
{
    public static class AdminPipe
    {
        public static string PipeName
        {
            get { return "hazem-admin-" + System.Diagnostics.Process.GetCurrentProcess().Id; }
        }
        static int started;
        static int stopping;

        public static void Start()
        {
            if (!Framework.Settings.Bool("Admin", "Pipe", true))
                return;
            if (Interlocked.Exchange(ref started, 1) != 0)
                return;
            var t = new Thread(Loop) { IsBackground = true, Name = "AdminPipe" };
            t.Start();
        }

        static NamedPipeServerStream Create()
        {
            var security = new PipeSecurity();
            var me = WindowsIdentity.GetCurrent().User;
            security.AddAccessRule(new PipeAccessRule(me, PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance | PipeAccessRights.Synchronize, AccessControlType.Allow));
            security.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkSid, null), PipeAccessRights.FullControl, AccessControlType.Deny));
            // [net10] the PipeSecurity constructor moved to NamedPipeServerStreamAcl
            return NamedPipeServerStreamAcl.Create(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 1024, 1024, security);
        }

        static void Loop()
        {
            int failures = 0;
            while (true)
            {
                try
                {
                    using (var pipe = Create())
                    {
                        failures = 0;
                        pipe.WaitForConnection();
                        var reader = new StreamReader(pipe, Encoding.ASCII, false, 256, true);
                        string line = ReadLineLimited(reader, 200);
                        string reply = Execute((line ?? "").Trim().ToLowerInvariant());
                        byte[] data = Encoding.ASCII.GetBytes(reply + "\n");
                        pipe.Write(data, 0, data.Length);
                        pipe.Flush();
                        try { pipe.WaitForPipeDrain(); } catch { }
                        if (reply == "stopping")
                        {
                            pipe.Disconnect();
                            Shutdown();
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (++failures >= 5)
                    {
                        MyConsole.WriteLine("[AdminPipe] stopped: " + e.Message);
                        return;
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        static string ReadLineLimited(StreamReader reader, int max)
        {
            var sb = new StringBuilder();
            while (sb.Length < max)
            {
                int c = reader.Read();
                if (c < 0 || c == '\n')
                    break;
                if (c != '\r')
                    sb.Append((char)c);
            }
            return sb.ToString();
        }

        static string Execute(string cmd)
        {
            switch (cmd)
            {
                case "ping":
                    {
                        int online = 0;
                        try { online = Database.Server.GamePoll != null ? Database.Server.GamePoll.Count : 0; } catch { }
                        long ram = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
                        return "ok name=" + Program.ServerConfig.ServerName + " id=" + Program.ServerConfig.ServerID
                            + " role=" + (Program.ServerConfig.IsInterServer ? "realm" : "game") + " online=" + online
                            + " pid=" + System.Diagnostics.Process.GetCurrentProcess().Id + " ram=" + ram
                            + " loaded=" + (Database.Server.FullLoading ? 1 : 0);
                    }
                case "save":
                    {
                        if (!Database.Server.FullLoading)
                            return "error not loaded";
                        MyConsole.WriteLine("[AdminPipe] save requested.");
                        Database.Server.SaveDatabase();
                        if (!Program.ServerConfig.IsInterServer)
                            Database.ServerDatabase.QueueAllPlayerSaves(false);
                        return Database.ServerDatabase.FinishPlayerSaves(120000) ? "saved" : "error saves still running";
                    }
                case "realms":
                    {
                        var r = Database.GroupServerList.InterServer;
                        return r == null ? "error no realm row" : "ok realm=" + r.Name + " " + r.IPAddress + ":" + r.Port + " secret=" + (MsgInterServer.PipeConfig.SecretUsable ? "set" : "missing");
                    }
                case "exit":
                case "stop":
                    return Interlocked.Exchange(ref stopping, 1) == 0 ? "stopping" : "error already stopping";
                default:
                    {
                        // live reload of the data files: "reloadnpcs", "reloadmapcells 1002", "reloadall force"... (HotReload)
                        string[] parts = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 0 && HotReload.HotReloadManager.IsReloadOp(parts[0]))
                        {
                            if (!Database.Server.FullLoading)
                                return "error not loaded";
                            var r = HotReload.HotReloadManager.Run(parts[0], parts.Length > 1 ? parts[1] : null, Array.IndexOf(parts, "force") > 0);
                            return (r.Ok ? "ok " + r.Message : "error " + r.Error).Replace('\r', ' ').Replace('\n', ' ');
                        }
                    }
                    if (cmd.StartsWith("visit "))
                    {
                        // staff / test tool: send an online player of this server to the Realm (pipe v2 relay link)
                        uint uid;
                        if (!uint.TryParse(cmd.Substring(6).Trim(), out uid))
                            return "error usage: visit <uid>";
                        Client.GameClient client;
                        if (Program.ServerConfig.IsInterServer || Database.Server.GamePoll == null || !Database.Server.GamePoll.TryGetValue(uid, out client) || client.Player == null)
                            return "error player " + uid + " is not online here";
                        if (!MsgInterServer.PipeConfig.SecretUsable || Database.GroupServerList.InterServer == null)
                            return "error pipe not configured (secret / realm row)";
                        if (client.PipeClient != null)
                            return "error player is already visiting";
                        MsgInterServer.PipeClient.Connect(client, Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port);
                        MyConsole.WriteLine("[AdminPipe] visit: " + client.Player.Name + " -> " + Database.GroupServerList.InterServer.Name);
                        return "ok visiting " + Database.GroupServerList.InterServer.Name;
                    }
                    return "error unknown command";
            }
        }

        static void Shutdown()
        {
            MyConsole.WriteLine("[AdminPipe] graceful stop requested by the control panel.");
            // a server that never finished loading has nothing to save (and must not write half-loaded state)
            if (!Database.Server.FullLoading)
                Environment.Exit(0);
            try { Program.ProcessConsoleEvent(0); }
            catch (Exception e) { MyConsole.SaveException(e); }
            Environment.Exit(0);
        }
    }
}
