// OfficialExtras module: small official 6609 client messages that nothing answered (layouts read from the client code).
//  * 1067 MsgLoginNotice - the login announcement board (glowing button next to the minimap, dialog 597).
//    Client -> server after login: [4] u32 = the last notice version it has seen (ini\GameSetUp.ini [LoginNotice]).
//    Server -> client: one 1067 per notice newer than that, ascending: [4] u32 version, [8] char[128] title,
//    [0x88] char[256] content (392 bytes). Notices: <DataPath>\notices.txt, lines "version|title|content".
//  * 2621 MsgCountryOpt (6 bytes, action 0) is sent by the quest window (cross-server tab) whenever it opens; the
//    client only stores the 2620 MsgCountryInfo reply ([4] u8, [5] u16, [7] u32, [11] u32) - an all-zero reply is enough.
// GM: @notice reload | @notice list
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.OfficialExtras
{
    public sealed class OfficialExtrasModule : ModuleBase
    {
        public override string Name { get { return "OfficialExtras"; } }

        public const ushort MsgLoginNotice = 1067, MsgCountryInfo = 2620, MsgCountryOpt = 2621;
        sealed class Notice { public uint Version; public string Title, Content; }
        static volatile Notice[] notices = new Notice[0];
        static string file;
        static OfficialExtrasModule instance;

        public override void OnLoad()
        {
            instance = this;
            file = DataPath + "notices.txt";
            Log(Load());
        }

        static string Load()
        {
            try
            {
                if (!File.Exists(file))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                    File.WriteAllText(file,
                        "# Login notices (button next to the minimap). One per line: version|title|content\r\n" +
                        "# Raise the version for a new notice: players whose saved version is lower see the button glow.\r\n" +
                        "# Title max 127 bytes, content max 255 bytes. Then @notice reload in game.\r\n" +
                        "1|Welcome to MeGa|Welcome to MeGa Conquer Online 6609. Have fun and play fair!\r\n");
                }
                var list = new List<Notice>();
                foreach (var raw in File.ReadAllLines(file, global::Conquer6609.AnsiText.Value))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line[0] == '#')
                        continue;
                    var p = line.Split(new[] { '|' }, 3);
                    uint v;
                    if (p.Length < 3 || !uint.TryParse(p[0], out v))
                        continue;
                    list.Add(new Notice { Version = v, Title = p[1], Content = p[2] });
                }
                list.Sort((a, b) => a.Version.CompareTo(b.Version));
                notices = list.ToArray();
                return notices.Length + " login notice(s) loaded";
            }
            catch (Exception e)
            {
                return "notices.txt not loaded: " + e.Message;
            }
        }

        [PacketAttribute(MsgLoginNotice)]
        public static unsafe void OnLoginNotice(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null)
                return;
            stream.Seek(4);
            uint seen = stream.ReadUInt32();
            foreach (var n in notices)
            {
                if (n.Version <= seen)
                    continue;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var p = rec.GetStream();
                    p.InitWriter();
                    p.Write(n.Version);
                    WriteFixed(p, n.Title, 128);
                    WriteFixed(p, n.Content, 256);
                    p.Finalize(MsgLoginNotice);
                    client.Send(p);
                }
            }
        }

        [PacketAttribute(MsgCountryOpt)]
        public static unsafe void OnCountryOpt(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var p = rec.GetStream();
                p.InitWriter();
                p.Write((byte)0);
                p.Write((ushort)0);
                p.Write((uint)0);
                p.Write((uint)0);
                p.Finalize(MsgCountryInfo);
                client.Send(p);
            }
        }

        static void WriteFixed(ServerSockets.Packet p, string text, int size)
        {
            byte[] b = global::Conquer6609.AnsiText.Value.GetBytes(text ?? "");
            int n = Math.Min(b.Length, size - 1);
            for (int i = 0; i < size; i++)
                p.Write(i < n ? b[i] : (byte)0);
        }

        [GmCommand("notice", Role = CommandRole.GM, Help = "login notices: @notice reload | @notice list", Usage = "@notice reload")]
        static void NoticeCommand(Client.GameClient client, string[] args)
        {
            if (args.Length > 1 && args[1].Equals("reload", StringComparison.OrdinalIgnoreCase))
            {
                string info = Load();
                instance.Log(info + " (reload by " + client.Player.Name + ")");
                client.SendSysMesage(info);
                return;
            }
            foreach (var n in notices)
                client.SendSysMesage("v" + n.Version + ": " + n.Title);
            if (notices.Length == 0)
                client.SendSysMesage("No login notices. Edit Modules\\OfficialExtras\\notices.txt, then @notice reload.");
        }
    }
}
