using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Shared by the launcher and PatchServer (linked into PatchServer.csproj) - keep it C# 7.3 / .NET Framework friendly.
namespace ScriptHazem.ClientFiles
{
    /// <summary>
    /// The client's login-screen server list. The 6609 client (Env_DX9\Conquer.exe + its shield DLLs) reads
    /// Guard.dat in the client root, not TQ's Server.dat (which is still the untouched 2017 official file).
    /// Guard.dat is a MySQL XML dump of table "outenserver" (id, ServerName, ServerIP, ServerPort, FlashName,
    /// FlashIcon, FlashHint, Child, ...) protected by the CServerFile cipher of the tool
    /// "أداة تعديل اسم واي بي السيرفر\اسحب الملف هنا.exe" (same function encrypts and decrypts):
    ///     buf[i] ^= key[(i + 1) % 32]; buf[i] ^= key[i % 32];
    /// Rows: id 0 = root (Child = number of groups), id 1 = group 1 (Child = number of servers), id 101.. = servers.
    /// ServerIP/ServerPort = the LOGIN (account) server. ServerName = what the client sends at login (16 bytes),
    /// it must match AuthConfig.ini [ServerInfo] ServerN. FlashName = the name shown in the list.
    /// </summary>
    public static class GuardFile
    {
        private static readonly byte[] Key =
        {
            0x5d, 0xcf, 0x12, 0xf6, 0x41, 0x0b, 0x9e, 0xe0, 0x27, 0xbe, 0x8f, 0x4c, 0x58, 0x84, 0x15, 0xc7,
            0x68, 0x3b, 0xad, 0x06, 0xfa, 0x79, 0x25, 0x44, 0x63, 0xb9, 0x0e, 0xf0, 0x47, 0xa0, 0x7f, 0x26
        };

        public const string DefaultIcon = "Group3_server12";
        public const string DefaultHint = "Hazem_Shield";

        /// <summary>One server of the client list.</summary>
        public class Entry
        {
            public string LoginName;    // ServerName (sent to the login server)
            public string DisplayName;  // FlashName (shown); empty = LoginName
            public string LoginHost;    // ServerIP
            public int LoginPort;       // ServerPort
            public string Icon;         // FlashIcon; empty = DefaultIcon
            public string Hint;         // FlashHint; empty = DefaultHint
        }

        /// <summary>Encrypts or decrypts (the cipher is its own inverse). Returns a new array.</summary>
        public static byte[] Crypt(byte[] data)
        {
            byte[] buf = (byte[])data.Clone();
            int n = Key.Length;
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] ^= Key[(i + 1) % n];
                buf[i] ^= Key[i % n];
            }
            return buf;
        }

        /// <summary>Plain XML in the exact layout of the original Guard.txt (MySQL XML dump, CRLF, tabs).</summary>
        public static string BuildXml(IList<Entry> servers)
        {
            const string nl = "\r\n";
            StringBuilder sb = new StringBuilder(4096 + servers.Count * 900);
            sb.Append("<?xml version=\"1.0\"?>").Append(nl);
            sb.Append("<mysqldump xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">").Append(nl);
            sb.Append("<database name=\"serverdat\">").Append(nl);
            sb.Append("\t<table_structure name=\"outenserver\">").Append(nl);
            Field(sb, "id", "int(10) unsigned", "PRI", false);
            Field(sb, "ServerName", "varchar(50)", "", true);
            Field(sb, "ServerIP", "varchar(128)", "", true);
            Field(sb, "ServerPort", "int(10) unsigned", "", true);
            Field(sb, "FlashName", "varchar(50)", "", true);
            Field(sb, "FlashIcon", "varchar(50)", "", true);
            Field(sb, "FlashHint", "varchar(50)", "", true);
            Field(sb, "Child", "int(10) unsigned", "", true);
            Field(sb, "PicServerIP", "varchar(50)", "", true);
            Field(sb, "PicServerPort", "int(10) unsigned", "", true);
            Field(sb, "BindServerIP", "varchar(50)", "", true);
            Field(sb, "BindServerPort", "int(10) unsigned", "", true);
            Field(sb, "Charges", "int(10) unsigned", "", true);
            Field(sb, "MiniDelayx", "int(10) unsigned", "", true);
            Field(sb, "BGPServerIP", "varchar(50)", "", true);
            Field(sb, "BGPServerPort", "int(11)", "", true);
            sb.Append("\t\t<key Table=\"outenserver\" Non_unique=\"0\" Key_name=\"PRIMARY\" Seq_in_index=\"1\" Column_name=\"id\" Collation=\"A\" Cardinality=\"3\" Null=\"\" Index_type=\"BTREE\" Comment=\"\" Index_comment=\"\" />").Append(nl);
            sb.Append("\t\t<options Name=\"outenserver\" Engine=\"MyISAM\" Version=\"10\" Row_format=\"Dynamic\" Rows=\"3\" Avg_row_length=\"46\" Data_length=\"140\" Max_data_length=\"281474976710655\" Index_length=\"2048\" Data_free=\"0\" Create_time=\"2023-10-31 06:44:40\" Update_time=\"2024-01-09 07:54:30\" Collation=\"utf8mb4_bin\" Create_options=\"\" Comment=\"\" />").Append(nl);
            sb.Append("\t</table_structure>").Append(nl);
            sb.Append("\t<table_data name=\"outenserver\">").Append(nl);
            Row(sb, 0, "", "", 0, "", "", "", 1);                 // root: one group
            Row(sb, 1, "", "", 0, "", "", "", servers.Count);     // group 1: N servers
            for (int i = 0; i < servers.Count; i++)
            {
                Entry e = servers[i];
                Row(sb, 101 + i, e.LoginName, e.LoginHost, e.LoginPort, Display(e), Icon(e), Hint(e), 0);
            }
            sb.Append("\t</table_data>").Append(nl);
            sb.Append("</database>").Append(nl);
            sb.Append("</mysqldump>").Append(nl);
            return sb.ToString();
        }

        private static string Display(Entry e) { return string.IsNullOrWhiteSpace(e.DisplayName) ? e.LoginName : e.DisplayName.Trim(); }
        private static string Icon(Entry e) { return string.IsNullOrWhiteSpace(e.Icon) ? DefaultIcon : e.Icon.Trim(); }
        private static string Hint(Entry e) { return string.IsNullOrWhiteSpace(e.Hint) ? DefaultHint : e.Hint.Trim(); }

        private static void Field(StringBuilder sb, string name, string type, string key, bool withDefault)
        {
            sb.Append("\t\t<field Field=\"").Append(name).Append("\" Type=\"").Append(type).Append("\" Null=\"NO\" Key=\"").Append(key).Append('"');
            if (withDefault) sb.Append(" Default=\"0\"");
            sb.Append(" Extra=\"\" />\r\n");
        }

        private static void Row(StringBuilder sb, int id, string name, string ip, int port, string flashName, string icon, string hint, int child)
        {
            sb.Append("\t<row>\r\n");
            Val(sb, "id", id.ToString());
            Val(sb, "ServerName", name);
            Val(sb, "ServerIP", ip);
            Val(sb, "ServerPort", port.ToString());
            Val(sb, "FlashName", flashName);
            Val(sb, "FlashIcon", icon);
            Val(sb, "FlashHint", hint);
            Val(sb, "Child", child.ToString());
            Val(sb, "PicServerIP", "0");
            Val(sb, "PicServerPort", "0");
            Val(sb, "BindServerIP", "0");
            Val(sb, "BindServerPort", "0");
            Val(sb, "Charges", "0");
            Val(sb, "MiniDelayx", "0");
            Val(sb, "BGPServerIP", "0");
            Val(sb, "BGPServerPort", "0");
            sb.Append("\t</row>\r\n");
        }

        private static void Val(StringBuilder sb, string name, string value)
        {
            sb.Append("\t\t<field name=\"").Append(name).Append("\">").Append(Escape(value)).Append("</field>\r\n");
        }

        private static string Escape(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        /// <summary>Encrypted Guard.dat bytes for the entries.</summary>
        public static byte[] BuildEncrypted(IList<Entry> servers)
        {
            return Crypt(Encoding.UTF8.GetBytes(BuildXml(servers)));
        }

        /// <summary>
        /// Writes the encrypted list to path when the server entries differ from what is there.
        /// The first time the original file is kept as path + ".orig". Returns true when the file was changed.
        /// </summary>
        public static bool Write(string path, IList<Entry> servers)
        {
            if (servers == null || servers.Count == 0) return false;
            byte[] encrypted = BuildEncrypted(servers);

            if (File.Exists(path))
            {
                List<Entry> old = ParseRows(Encoding.UTF8.GetString(Crypt(File.ReadAllBytes(path))));
                if (Same(old, servers)) return false;
                string backup = path + ".orig";
                if (!File.Exists(backup)) File.Copy(path, backup);
            }

            string tmp = path + ".tmp";
            File.WriteAllBytes(tmp, encrypted);
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Replace(tmp, path, null);
            }
            else
            {
                File.Move(tmp, path);
            }
            return true;
        }

        /// <summary>Server rows (id >= 100) of an existing Guard.dat; empty when the file is missing.</summary>
        public static List<Entry> Read(string path)
        {
            if (!File.Exists(path)) return new List<Entry>();
            return ParseRows(Encoding.UTF8.GetString(Crypt(File.ReadAllBytes(path))));
        }

        public static List<Entry> ParseRows(string xml)
        {
            List<Entry> list = new List<Entry>();
            int pos = 0;
            while (true)
            {
                int start = xml.IndexOf("<row>", pos, StringComparison.Ordinal);
                if (start < 0) break;
                int end = xml.IndexOf("</row>", start, StringComparison.Ordinal);
                if (end < 0) break;
                string row = xml.Substring(start, end - start);
                pos = end + 6;
                int id;
                if (!int.TryParse(Get(row, "id"), out id) || id < 100) continue;
                int port;
                int.TryParse(Get(row, "ServerPort"), out port);
                Entry e = new Entry();
                e.LoginName = Get(row, "ServerName");
                e.DisplayName = Get(row, "FlashName");
                e.LoginHost = Get(row, "ServerIP");
                e.LoginPort = port;
                e.Icon = Get(row, "FlashIcon");
                e.Hint = Get(row, "FlashHint");
                list.Add(e);
            }
            return list;
        }

        private static string Get(string row, string field)
        {
            string open = "<field name=\"" + field + "\">";
            int a = row.IndexOf(open, StringComparison.Ordinal);
            if (a < 0) return "";
            a += open.Length;
            int b = row.IndexOf("</field>", a, StringComparison.Ordinal);
            if (b < 0) return "";
            return row.Substring(a, b - a).Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
        }

        private static bool Same(List<Entry> old, IList<Entry> servers)
        {
            if (old.Count != servers.Count) return false;
            for (int i = 0; i < old.Count; i++)
            {
                Entry a = old[i];
                Entry b = servers[i];
                if (a.LoginName != b.LoginName || a.DisplayName != Display(b) || a.LoginHost != b.LoginHost
                    || a.LoginPort != b.LoginPort || a.Icon != Icon(b) || a.Hint != Hint(b))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
