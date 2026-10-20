using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace PatchServer
{
    /// <summary>
    /// Serves GET /status.json for the launcher: a maintenance banner plus the
    /// live status of every published game server.
    ///
    /// The launcher consumes exactly:
    ///   {"maintenance":false,"until":"2026-10-12T18:00:00Z",
    ///    "message_ar":"","message_en":"",
    ///    "servers":[{"id":1,"name":"Hazem","online":true,"players":-1,"maintenance":false}]}
    ///
    /// The <c>maintenance</c>/<c>until</c>/<c>message_ar</c>/<c>message_en</c> block is read
    /// from the hand-edited file <c>www\data\status.json</c> (all fields optional). The
    /// <c>servers</c> array is built live from the game-server registry (same TCP probe,
    /// cached, as GET /servers), so it reflects which servers are up right now. A missing
    /// or broken source yields a sane default (not in maintenance, empty messages).
    /// </summary>
    internal sealed class StatusStore
    {
        private readonly string sourcePath;
        private readonly ServerRegistry registry;

        public StatusStore(string sourcePath, ServerRegistry registry)
        {
            this.sourcePath = sourcePath;
            this.registry = registry;
        }

        /// <summary>Full path of the source file (www\data\status.json).</summary>
        public string SourcePath
        {
            get { return sourcePath; }
        }

        /// <summary>Builds the /status.json body fresh (server status is probed live, cached a few seconds).</summary>
        public byte[] Build()
        {
            bool maintenance = false;
            string until = string.Empty;
            string messageAr = string.Empty;
            string messageEn = string.Empty;
            Dictionary<string, int> players = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            ReadBanner(ref maintenance, ref until, ref messageAr, ref messageEn, players);

            List<Dictionary<string, object>> servers = registry != null
                ? registry.BuildStatusServers()
                : new List<Dictionary<string, object>>();

            // the game server does not report a count; let the owner fill one per server in status.json
            if (players.Count > 0)
            {
                foreach (Dictionary<string, object> s in servers)
                {
                    object name;
                    int count;
                    if (s.TryGetValue("name", out name) && name != null
                        && players.TryGetValue(Convert.ToString(name, CultureInfo.InvariantCulture), out count))
                    {
                        s["players"] = count;
                    }
                }
            }

            Dictionary<string, object> root = new Dictionary<string, object>();
            root["maintenance"] = maintenance;
            root["until"] = until;
            root["message_ar"] = messageAr;
            root["message_en"] = messageEn;
            root["servers"] = servers;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return Encoding.UTF8.GetBytes(serializer.Serialize(root));
        }

        private void ReadBanner(ref bool maintenance, ref string until, ref string messageAr, ref string messageEn,
            Dictionary<string, int> players)
        {
            if (!File.Exists(sourcePath))
            {
                return;
            }

            try
            {
                string text = ReadShared(sourcePath);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                Dictionary<string, object> src = serializer.DeserializeObject(text) as Dictionary<string, object>;
                if (src == null)
                {
                    return;
                }

                maintenance = Bool(src, "maintenance");
                until = Str(src, "until");
                messageAr = Str(src, "message_ar", "message");
                messageEn = Str(src, "message_en");

                // optional owner-supplied player counts: "servers":[{"name":"Hazem","players":137}]
                object serversValue;
                if (players != null && src.TryGetValue("servers", out serversValue))
                {
                    object[] rows = serversValue as object[];
                    if (rows != null)
                    {
                        foreach (object raw in rows)
                        {
                            Dictionary<string, object> row = raw as Dictionary<string, object>;
                            if (row == null) continue;
                            string name = Str(row, "name");
                            object pv;
                            if (name.Length == 0 || !row.TryGetValue("players", out pv) || pv == null) continue;
                            int count;
                            if (int.TryParse(Convert.ToString(pv, CultureInfo.InvariantCulture), NumberStyles.Integer,
                                    CultureInfo.InvariantCulture, out count) && count >= 0)
                            {
                                players[name] = count;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("status.json could not be read (" + ex.Message + "), using defaults");
            }
        }

        private static bool Bool(Dictionary<string, object> src, string key)
        {
            object value;
            if (src.TryGetValue(key, out value) && value != null)
            {
                if (value is bool) return (bool)value;
                bool b;
                if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out b))
                {
                    return b;
                }
                string s = Convert.ToString(value, CultureInfo.InvariantCulture).Trim();
                return s == "1";
            }
            return false;
        }

        private static string Str(Dictionary<string, object> src, params string[] keys)
        {
            foreach (string key in keys)
            {
                object value;
                if (src.TryGetValue(key, out value) && value != null)
                {
                    string s = Convert.ToString(value, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(s))
                    {
                        return s;
                    }
                }
            }
            return string.Empty;
        }

        private static string ReadShared(string path)
        {
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read,
                        FileShare.ReadWrite | FileShare.Delete))
                    using (StreamReader reader = new StreamReader(fs, Encoding.UTF8, true))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (IOException)
                {
                    if (attempt >= 3) throw;
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }
}
