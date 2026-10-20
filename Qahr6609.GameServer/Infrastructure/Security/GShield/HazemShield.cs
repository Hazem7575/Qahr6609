namespace Conquer6609.Game.MsgLoader
{
    using Conquer6609;
    using Conquer6609.Client;
    using Conquer6609.Database;
    using Conquer6609.ServerSockets;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using GShield;

    public static class HazemShield
    {
        public const int MaxLoginClient = 3;
        private const ushort RequiredDllVersion = 7941;

        private static readonly uint[] keys =
        {
            0x3F2A9D1Cu, 0x5C7E8A4Bu, 0x2B1F6D39u, 0x7A4C3E91u,
            0x6D3B1F72u, 0x4E2C8A5Fu, 0x1B9D3C7Au, 0x3A7F9E2Du,
            0x5E1C6B3Au, 0x6F2D7C1Bu, 0x7B4A2E5Cu, 0x2D8C3F91u,
            0x4F9E1B7Au, 0x3C7D2A6Eu, 0x1A5B4E93u, 0x6E3F7D2Cu
        };

        public static uint Decrypt(uint value)
        {
            for (int i = 15; i >= 0; --i)
            {
                value -= keys[(i + 3) % 16];
                value ^= (keys[i] >> 16);
                value = (value >> 5) | (value << 27);
                value = (value - keys[15 - i]) ^ keys[i];
            }
            return value;
        }

        // ==============================================================
        // Login Packet
        // ==============================================================
        [Packet((UInt16)Types.PacketTypes.CMsgRequestLogin)]
        public static void RequestLogin(Client.GameClient client, Packet packet)
        {
            uint encrypted = packet.ReadUInt32();
            client.Key =/* Decrypt*/(encrypted);
            client.TqSerial = packet.ReadCString(12).Replace("\0", "");
            client.Version = packet.ReadUInt16();
            client.DllVersion = packet.ReadUInt16();

            if (client.DllVersion != RequiredDllVersion)
            {
                Kick(client, "Wrong HazemShield version (v" + client.DllVersion + ")");
                return;
            }

            client.ClientFlag |= Client.ServerFlag.OnLoggion;
            SendPacket(client, Types.SubType.PCINFOSend);
            ServerDatabase.LoginQueue.TryEnqueue(client);
        }

        // ==============================================================
        // PC Info Packet
        // ==============================================================
        [Packet((ushort)Types.PacketTypes.MsgSendPCInfo)]
        public static void Process(Client.GameClient client, Packet stream)
        {
            stream.Seek(0);
            byte[] buffer = stream.ReadBytes(stream.Size);

            using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
            {
                br.BaseStream.Seek(4, SeekOrigin.Begin);
                client.PCName = global::Conquer6609.AnsiText.Value.GetString(br.ReadBytes(32)).TrimEnd('\0');
                client.HDDSerial = global::Conquer6609.AnsiText.Value.GetString(br.ReadBytes(16)).TrimEnd('\0');
                client.HWID = global::Conquer6609.AnsiText.Value.GetString(br.ReadBytes(64)).TrimEnd('\0');
            }

            if (!IsValidLoginData(client))
                Kick(client, "Invalid HWID/PC info");
        }

        private static bool IsValidLoginData(Client.GameClient c)
        {
            return c.Key != 0 &&
                   !string.IsNullOrEmpty(c.TqSerial) &&
                   c.Version != 0 &&
                   c.DllVersion == RequiredDllVersion &&
                   !string.IsNullOrEmpty(c.PCName) &&
                   !string.IsNullOrEmpty(c.HDDSerial) &&
                   !string.IsNullOrEmpty(c.HWID);
        }

        // ==============================================================
        // Main Shield Packet
        // ==============================================================
        [Packet((ushort)Types.PacketTypes.CMsgShield)]
        public static void LoaderHandler(Client.GameClient client, Packet stream)
        {
            // already kicked: ignore the rest of its reports (no parsing, no logging on the socket thread)
            if (client.IsBeingDisconnected) return;
            stream.Seek(0);
            var buffer = stream.ReadBytes(stream.Size);
            BinaryReader br = new BinaryReader(new MemoryStream(buffer));

            try
            {
                br.BaseStream.Seek(4, SeekOrigin.Current);
                Types.SubType type = (Types.SubType)br.ReadInt16();

                if (type == Types.SubType.HackDetected)
                {
                    Types.CheatFlags flag = (Types.CheatFlags)br.ReadInt16();
                    int len = br.ReadInt32();
                    // client-supplied length: ReadBytes allocates it up front (up to 2 GB in an x86 process)
                    if (len < 0 || len > 256 || len > br.BaseStream.Length - br.BaseStream.Position)
                        return;
                    string reason = global::Conquer6609.AnsiText.Value.GetString(br.ReadBytes(len)).TrimEnd('\0').Replace('\r', ' ').Replace('\n', ' ');

                    string msg = "";
                    switch (flag)
                    {
                        case Types.CheatFlags.Program: msg = "Banned Program: " + reason; break;
                        case Types.CheatFlags.Debugger: msg = "Debugger Detected"; break;
                        case Types.CheatFlags.SpeedCClient: msg = "Speed Hack Detected"; break;
                        case Types.CheatFlags.CloseingThread: msg = "Thread Killer Detected"; break;
                        case Types.CheatFlags.Cheatengine: msg = "Cheat Engine Detected"; break;
                        case Types.CheatFlags.AutoClicker:
                            if (ClickerAllowed(client, flag, reason)) return;
                            msg = "AutoClicker Detected" + (reason.Length > 0 ? " (" + reason + ")" : ""); break;
                        case Types.CheatFlags.AutoKeyboard:
                            if (ClickerAllowed(client, flag, reason)) return;
                            msg = "AutoKeyboard / Macro Detected" + (reason.Length > 0 ? " (" + reason + ")" : ""); break;
                        default: msg = "Unknown Hack"; break;
                    }

                    Detected(client, msg);
                }
                else if (type == Types.SubType.ConquerFileHash)
                {
                    byte[] hashes = br.ReadBytes(231);
                    if (hashes.Length < 231) return;

                    string[] files = new string[7];
                    for (int i = 0; i < 7; i++)
                        files[i] = Encoding.ASCII.GetString(hashes, i * 33, 32).TrimEnd('\0');

                    string invalid;
                    if (!_GShield.Validated(files[0], files[1], files[2], files[3], files[4], files[5], files[6], out invalid))
                        Detected(client, "Modified Client Files: " + invalid);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("[HazemShield] Error: " + ex.Message);
            }
            finally
            {
                br.Close();
            }
        }

        // ==============================================================
        // AutoClicker / AutoKeyboard allow-list
        //   1) devices in ServerDataBase\Allow_Clicker.txt (TqSerial, HazemPanel "Allow Clicker")
        //   2) IPs in shell.ini [HazemShield] AllowClickerIPs=ip1,ip2  (default: 127.0.0.1 + the server's own IP,
        //      i.e. the owner's PC that hosts the server). Set AllowClickerIPs=none to disable.
        // Every detection is logged with the client's reason, serial and IP so false positives can be traced.
        // ==============================================================
        private static HashSet<string> allowedClickerIps;
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime> clickerLogStamp = new System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime>();
        private static readonly bool clickerForgetHook = global::Conquer6609.Security.Guard.RegisterForgetHook(Forget);

        private static HashSet<string> AllowedClickerIps()
        {
            if (allowedClickerIps != null) return allowedClickerIps;
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                // IniFile(string) prepends DbLocation to the path, so set FileName directly (same as SaveDatabase does)
                var ini = new WindowsAPI.IniFile();
                ini.FileName = System.IO.Directory.GetCurrentDirectory() + "\\shell.ini";
                string list = ini.ReadString("HazemShield", "AllowClickerIPs", "default", 512).Trim();
                if (list.Equals("default", StringComparison.OrdinalIgnoreCase))
                    list = "127.0.0.1," + (Program.ServerConfig.IPAddres ?? "");
                if (!list.Equals("none", StringComparison.OrdinalIgnoreCase))
                    foreach (var ip in list.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        set.Add(ip.Trim());
            }
            catch (Exception e) { System.Console.WriteLine("[HazemShield] AllowClickerIPs: " + e.Message); }
            allowedClickerIps = set;
            System.Console.WriteLine("[HazemShield] AutoClicker allowed IPs: " + (set.Count == 0 ? "none" : string.Join(", ", set)));
            return set;
        }

        private static bool ClickerAllowed(GameClient client, Types.CheatFlags flag, string reason)
        {
            string ip = "";
            try { if (client.Socket != null) ip = client.Socket.RemoteIp ?? ""; } catch { }
            bool allowed = Database.Allow_Clicker.IsAllowed(client) || AllowedClickerIps().Contains(ip.Trim());
            uint uid = client.Player != null ? client.Player.UID : 0;
            DateTime last;
            // log throttle per client: allowed devices every 10 min, others every 10 s (the kick itself is not throttled)
            if (!clickerLogStamp.TryGetValue(uid, out last) || (DateTime.Now - last).TotalSeconds >= (allowed ? 600 : 10))
            {
                clickerLogStamp[uid] = DateTime.Now;
                string name = client.Player != null ? client.Player.Name : "?";
                LogCheat(name, uid, flag + (allowed ? " IGNORED (allowed device)" : "") + " | reason='" + reason + "' serial=" + client.TqSerial + " ip=" + ip);
                if (allowed)
                {
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    System.Console.WriteLine("[HazemShield] {0} on allowed device ignored -> {1} ({2}) reason='{3}'", flag, name, ip, reason);
                    System.Console.ResetColor();
                }
            }
            return allowed;
        }

        /// <summary>Drops per-player state (called on logout via Guard.ForgetPlayer).</summary>
        public static void Forget(uint uid)
        {
            DateTime last;
            clickerLogStamp.TryRemove(uid, out last);
        }

        // ==============================================================
        // Memory Check Packet
        // ==============================================================
        [Packet((UInt16)Types.PacketTypes.CMsgMemoryCheck)]
        public static void MemoryShield(Client.GameClient client, Packet stream)
        {
            if (client.IsBeingDisconnected) return;
            stream.Seek(4);
            var buffer = stream.ReadBytes(stream.Size);

            using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
            {
                string mem1 = global::Conquer6609.AnsiText.Value.GetString(br.ReadBytes(17)).Replace("\0", "");
                string mem2 = global::Conquer6609.AnsiText.Value.GetString(br.ReadBytes(17)).Replace("\0", "");

                if (!string.IsNullOrEmpty(mem1) && !string.IsNullOrEmpty(mem2) && mem1 != mem2)
                    Detected(client, "Memory Manipulation Detected");
            }
        }

        // ==============================================================
        // Kick & Detection
        // ==============================================================
        public static void Detected(GameClient client, string reason)
        {
            if (client.IsBeingDisconnected) return;
           // client.IsBeingDisconnected = true;

            LogCheat(client.Player.Name, client.Player.UID, reason);

            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("[HazemShield] KICKED → {0} (UID: {1}) | {2}", client.Player.Name, client.Player.UID, reason);
            System.Console.ResetColor();
            HazemShield.SendForceClose(client, "HazemShield", "You have been disconnected.\nReason: " + reason + "");

           // client.Player.MessageBox("You have been disconnected.\nReason: " + reason, null, null);
            //client.Socket.Disconnect();
        }

        private static void LogCheat(string name, uint uid, string reason)
        {
            string line = string.Format("[{0:HH:mm:ss}] {1} (UID: {2}) → {3}", DateTime.Now, name, uid, reason);
            // written by a background thread to Loader_Logs\HazemShield\yyyy-MM-dd.txt (one open writer per day)
            ShieldLogWriter.AppendDaily(Path.Combine("Loader_Logs", "HazemShield"), line);
        }

        private static void Kick(GameClient client, string reason)
        {
            using (var Recycled = new Conquer6609.ServerSockets.RecycledPacket())
            {
                var cPacket = Recycled.GetStream();
               System.Console.WriteLine("[HazemShield] Login rejected → {0} | UID: {1}", reason, client.Key);
                HazemShield.SendForceClose(client, "HazemShield", "Please restart the game.");
            }
        }

        // ==============================================================
        // Notification
        // ==============================================================
        public static Packet SendNotification(Packet stream, string title, string message, byte type = 0, short duration = 3000)
        {
            stream.InitWriter();
            stream.Write((int)Types.SubType.MsgCustomNotification);
            stream.Write((short)title.Length);
            stream.Write(title, title.Length);
            stream.Write((short)message.Length);
            stream.Write(message, message.Length);
            stream.Write(type);
            stream.Write(duration);
            stream.Finalize((ushort)Types.PacketTypes.CMsgShield);
            return stream;
        }

        public static void SendNotification(Client.GameClient pClient, string title, string message, byte type = 0, short duration = 3000)
        {
            using (var rec = new RecycledPacket())
            {
                var cPacket = rec.GetStream();
                pClient.Send(SendNotification(cPacket, title, message, type, duration));
            }
        }

        // ==============================================================
        // Login & Periodic Checks
        // ==============================================================
        public static void OnLogin(GameClient client)
        {
            SendDiscordStatus(client);
            SendPacket(client, Types.SubType.DoLogin);
            SendPacket(client, Types.SubType.ThreadCheck);
            SendPacket(client, Types.SubType.ConquerFileHash);
            SendPacket(client, Types.SubType.ProcessesCheck);
            SendDiscordOnline(client);

            client.IsBeingDisconnected = false;
        }

        public static void OnThread(GameClient client)
        {
            if (client.IsBeingDisconnected) return;

            if (DateTime.Now > client.MemoryCheckStamp.AddSeconds(18))
            {
                client.MemoryCheckStamp = DateTime.Now;
                SendPacket(client, Types.SubType.MemoryCheck);
            }

            if (DateTime.Now > client.ThreadCheckStamp.AddSeconds(70))
            {
                client.ThreadCheckStamp = DateTime.Now;
                SendPacket(client, Types.SubType.ThreadCheck);
            }
            if (DateTime.Now > client.ThreadSendDiscordOnline.AddMinutes(2))
            {
                client.ThreadSendDiscordOnline = DateTime.Now;
                SendDiscordOnline(client);
            }
        }

        // ==============================================================
        // DiscordOnline
        // ==============================================================
        public static void SendDiscordOnline(Client.GameClient pClient)
        {
            // Never wait on the network here: send the cached value and refresh it in the background.
            DiscordOnline.RequestRefresh();

            int countToSend = DiscordOnline.OnlineCount;

            using (var rec = new RecycledPacket())
            {
                var packet = rec.GetStream();

                packet.InitWriter();
                packet.Write((int)Types.SubType.MsgDiscordOnline);
                packet.Write(countToSend);
                packet.Finalize((ushort)Types.PacketTypes.CMsgShield);

                pClient.Send(packet);

              //  Console.WriteLine($"[DiscordOnline] Sent to {pClient.Player?.Name ?? "Unknown"} | Count = {countToSend}");
            }
        }

        // ==============================================================
        // CloseClient
        // ==============================================================
        public static Packet SendForceClose(Packet stream, string title, string message)
        {
            stream.InitWriter();

            stream.Write((int)Types.SubType.MsgForceClose);

            stream.Write((short)title.Length);
            stream.Write(title, title.Length);

            stream.Write((short)message.Length);
            stream.Write(message, message.Length);

            stream.Finalize((ushort)Types.PacketTypes.CMsgShield);
            return stream;
        }

        public static void SendForceClose(Client.GameClient pClient, string title, string message)
        {
            using (var rec = new RecycledPacket())
            {
                var cPacket = rec.GetStream();
                pClient.Send(SendForceClose(cPacket, title, message));
            }
            pClient.IsBeingDisconnected = true;

            // Disconnect after a delay (so the close message is delivered) without blocking the calling thread.
            var socket = pClient.Socket;
            if (socket == null)
                return;
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
            {
                try { socket.Disconnect(); }
                catch (Exception e) { MyConsole.SaveException(e); }
            });

        }



        // ==============================================================
        // Unchanged packet helpers (exactly your original code)
        // ==============================================================

        public static Conquer6609.ServerSockets.Packet SendProPacket(Conquer6609.ServerSockets.Packet Stream, Types.SubType sData)
        {
            Stream.InitWriter();
            Stream.Write(Convert.ToInt32(sData));
            Stream.Finalize(Convert.ToUInt16(Types.PacketTypes.CMsgShield));
            return Stream;
        }

        public static void SendPacket(Conquer6609.Client.GameClient client, Types.SubType In)
        {
            using (var Recycled = new Conquer6609.ServerSockets.RecycledPacket())
            {
                var cPacket = Recycled.GetStream();
                client.Send(SendProPacket(cPacket, In));
            }
        }

        public static Conquer6609.ServerSockets.Packet DiscordTitle1Create(this Conquer6609.ServerSockets.Packet Stream, String Title)
        {
            Stream.InitWriter();
            Stream.Write(Convert.ToInt32(Types.SubType.DiscordTitle1));
            Stream.Write((Int16)Title.Length);
            Stream.Write(Title, Title.Length);
            Stream.Finalize(Convert.ToUInt16(Types.PacketTypes.CMsgShield));
            return Stream;
        }

        public static Conquer6609.ServerSockets.Packet DiscordTitle2Create(this Conquer6609.ServerSockets.Packet Stream, UInt16 FaceID, String Title)
        {
            Stream.InitWriter();
            Stream.Write(Convert.ToInt32(Types.SubType.DiscordTitle2));
            Stream.Write(Convert.ToInt32(FaceID).ToString(), 3);
            Stream.Write((Int16)Title.Length);
            Stream.Write(Title, Title.Length);
            Stream.Finalize(Convert.ToUInt16(Types.PacketTypes.CMsgShield));
            return Stream;
        }

        public static void SendDiscordStatus(Client.GameClient client)
        {
            using (var rec = new RecycledPacket())
            {
                var cPacket = rec.GetStream();
                client.Send(DiscordTitle1Create(cPacket, string.Format("{0}({1})", client.Player.Name, client.Player.NobilityRank.ToString())));
                client.Send(DiscordTitle2Create(cPacket, client.Player.Face, string.Format("{0}", Helper.GetJobName(client.Player.Class))));
            }
        }
    }

    // Background writer for HazemShield logs: socket-thread callers only enqueue; one StreamWriter per folder/day,
    // flushed after every batch. Bounded queue (lines beyond MaxPending are dropped, never block the caller).
    internal static class ShieldLogWriter
    {
        private sealed class Entry
        {
            public string Dir;      // daily log folder (AppendDaily)
            public string Day;
            public string Text;
            public string File;     // whole-file write (WriteFile)
            public string[] Lines;
        }
        private const int MaxPending = 20000;
        private static readonly System.Collections.Concurrent.ConcurrentQueue<Entry> queue = new System.Collections.Concurrent.ConcurrentQueue<Entry>();
        private static readonly AutoResetEvent signal = new AutoResetEvent(false);
        private static readonly object drainSync = new object();
        private static readonly Dictionary<string, KeyValuePair<string, StreamWriter>> daily = new Dictionary<string, KeyValuePair<string, StreamWriter>>(StringComparer.OrdinalIgnoreCase);
        private static int pending;
        private static int started;

        public static void AppendDaily(string dir, string line)
        {
            Enqueue(new Entry { Dir = dir, Day = DateTime.Now.ToString("yyyy-MM-dd"), Text = line });
        }

        public static void WriteFile(string path, IList<string> lines)
        {
            var copy = new string[lines.Count];
            lines.CopyTo(copy, 0);
            Enqueue(new Entry { File = path, Lines = copy });
        }

        private static void Enqueue(Entry e)
        {
            if (Interlocked.Increment(ref pending) > MaxPending)
            {
                Interlocked.Decrement(ref pending);
                return;
            }
            queue.Enqueue(e);
            EnsureStarted();
            signal.Set();
        }

        private static void EnsureStarted()
        {
            if (Interlocked.CompareExchange(ref started, 1, 0) != 0)
                return;
            var t = new Thread(Run) { IsBackground = true, Name = "HazemShieldLog", Priority = ThreadPriority.BelowNormal };
            t.Start();
            AppDomain.CurrentDomain.ProcessExit += (s, a) => Drain();
        }

        private static void Run()
        {
            while (true)
            {
                signal.WaitOne(1000);
                Drain();
            }
        }

        private static void Drain()
        {
            lock (drainSync)
            {
                Entry e;
                bool wrote = false;
                while (queue.TryDequeue(out e))
                {
                    Interlocked.Decrement(ref pending);
                    try
                    {
                        if (e.Lines != null)
                        {
                            string dir = Path.GetDirectoryName(e.File);
                            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                            File.WriteAllLines(e.File, e.Lines, Encoding.UTF8);
                        }
                        else
                        {
                            GetDaily(e.Dir, e.Day).WriteLine(e.Text);
                            wrote = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("[HazemShield] log write failed: " + ex.Message);
                    }
                }
                if (wrote)
                {
                    foreach (var kv in daily.Values)
                    {
                        try { kv.Value.Flush(); } catch { }
                    }
                }
            }
        }

        private static StreamWriter GetDaily(string dir, string day)
        {
            KeyValuePair<string, StreamWriter> cur;
            if (daily.TryGetValue(dir, out cur))
            {
                if (cur.Key == day)
                    return cur.Value;
                try { cur.Value.Dispose(); } catch { }
                daily.Remove(dir);
            }
            Directory.CreateDirectory(dir);
            var fs = new FileStream(Path.Combine(dir, day + ".txt"), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            var w = new StreamWriter(fs, new UTF8Encoding(false));
            daily[dir] = new KeyValuePair<string, StreamWriter>(day, w);
            return w;
        }
    }

    public static class Helper
    {
        private static readonly Dictionary<byte, string> jobNames = new Dictionary<byte, string>
        {
            { 10, "InternTrojan" }, { 11, "Trojan" }, { 12, "VeteranTrojan" }, { 13, "TigerTrojan" }, { 14, "DragonTrojan" }, { 15, "TrojanMaster" },
            { 20, "InternWarrior" }, { 21, "Warrior" }, { 22, "BrassWarrior" }, { 23, "SilverWarrior" }, { 24, "GoldWarrior" }, { 25, "WarriorMaster" },
            { 40, "InternArcher" }, { 41, "Archer" }, { 42, "EagleArcher" }, { 43, "TigerArcher" }, { 44, "DragonArcher" }, { 45, "ArcherMaster" },
            { 50, "InternNinja" }, { 51, "Ninja" }, { 52, "MiddleNinja" }, { 53, "DarkNinja" }, { 54, "MysticNinja" }, { 55, "NinjaMaster" },
            { 60, "InternMonk" }, { 61, "Monk" }, { 62, "DhyanaMonk" }, { 63, "DharmaMonk" }, { 64, "PrajnaMonk" }, { 65, "NirvanaMonk" },
            { 70, "InternMalePirate" }, { 71, "MalePirate" }, { 72, "PirateGunner" }, { 73, "Quartermaster" }, { 74, "PirateCaptain" }, { 75, "PirateLord" },
            { 80, "NoviceLeeLong" }, { 81, "LeeLong" }, { 82, "Expert~LeeLong" }, { 83, "Elite~LeeLong" }, { 84, "Master~LeeLong" }, { 85, "King~LeeLong" },
            { 100, "InternTaoist" }, { 101, "Taoist" },
            { 112, "MetalTaoist" }, { 113, "MetalWizard" }, { 114, "MetalMaster" }, { 115, "MetalSaint" },
            { 122, "WoodTaoist" }, { 123, "WoodWizard" }, { 124, "WoodMaster" }, { 125, "WoodSaint" },
            { 132, "WaterTaoist" }, { 133, "WaterWizard" }, { 134, "WaterMaster" }, { 135, "WaterSaint" },
            { 142, "FireTaoist" }, { 143, "FireWizard" }, { 144, "FireMaster" }, { 145, "FireSaint" },
            { 152, "EarthTaoist" }, { 153, "EarthWizard" }, { 154, "EarthMaster" }, { 155, "EarthSaint" },
            { 160, "Windwalker" }, { 161, "WindGuard" }, { 162, "WindOfficer" }, { 163, "WindSupervisor" }, { 164, "WindManager" }, { 165, "WindLord" }
        };

        public static string GetJobName(byte job)
        {
            string name;
            jobNames.TryGetValue(job, out name);
            return name ?? "Unknown";
        }
    }
}