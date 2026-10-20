using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ScriptHazem.ClientFiles;

namespace ScriptHazemLauncher
{
    // GET <UpdateUrl>servers (PatchServer, from servers.ini - published servers only):
    // {"generated":"..","login":{"host","port","online"},
    //  "servers":[{"id","name","host","port","loginHost","loginPort","online","maintenance","players","icon","notice","notice_en"}]}
    public class ServerListDto
    {
        public string generated { get; set; }
        public LoginDto login { get; set; }
        public List<GameServerDto> servers { get; set; }
    }

    public class LoginDto
    {
        public string host { get; set; }
        public int port { get; set; }
        public bool online { get; set; }
    }

    public class GameServerDto
    {
        public int id { get; set; }
        public string name { get; set; }
        /// <summary>Name the client sends to the login server (ServerName); empty = name.</summary>
        public string loginName { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public string loginHost { get; set; }
        public int loginPort { get; set; }
        public bool online { get; set; }
        public bool maintenance { get; set; }
        public int players { get; set; }
        public string icon { get; set; }
        public string hint { get; set; }
        public string notice { get; set; }
        public string notice_en { get; set; }

        [JsonIgnore]
        public string Notice { get { return NewsItem.Pick(notice, notice_en); } }
    }

    public static class ServerListClient
    {
        private static readonly HttpClient Http = CreateHttp();

        private static HttpClient CreateHttp()
        {
            HttpClient c = new HttpClient();
            c.Timeout = TimeSpan.FromSeconds(6);
            c.DefaultRequestHeaders.UserAgent.ParseAdd("scripthazemLauncher/2.1");
            return c;
        }

        /// <summary>Downloads and parses /servers. Throws when the patch server is unreachable or the answer is not a list.</summary>
        public static ServerListDto Fetch(string baseUrl)
        {
            byte[] data = Http.GetByteArrayAsync(baseUrl + "servers?t=" + DateTime.UtcNow.Ticks).GetAwaiter().GetResult();
            return Parse(Encoding.UTF8.GetString(data));
        }

        public static ServerListDto Parse(string json)
        {
            ServerListDto list = JsonSerializer.Deserialize<ServerListDto>(json, Updater.Json);
            if (list == null || list.servers == null) throw new InvalidDataException("no server list");
            list.servers.RemoveAll(s => s == null || string.IsNullOrWhiteSpace(s.name));
            return list;
        }

        /// <summary>Client list entries from /servers; the selected server (by display name) is listed first.</summary>
        public static List<GuardFile.Entry> ToGuardEntries(ServerListDto list, string selected, string fallbackHost, int fallbackPort)
        {
            List<GuardFile.Entry> result = new List<GuardFile.Entry>();
            if (list == null || list.servers == null) return result;
            foreach (GameServerDto s in list.servers)
            {
                string host = !string.IsNullOrWhiteSpace(s.loginHost) ? s.loginHost
                    : (list.login != null && !string.IsNullOrWhiteSpace(list.login.host) ? list.login.host : fallbackHost);
                int port = s.loginPort > 0 ? s.loginPort : (list.login != null && list.login.port > 0 ? list.login.port : fallbackPort);
                if (string.IsNullOrWhiteSpace(host) || port <= 0) continue;
                GuardFile.Entry e = new GuardFile.Entry
                {
                    LoginName = (string.IsNullOrWhiteSpace(s.loginName) ? s.name : s.loginName).Trim(),
                    DisplayName = s.name.Trim(),
                    LoginHost = host.Trim(),
                    LoginPort = port,
                    Icon = s.icon,
                    Hint = s.hint
                };
                if (!string.IsNullOrEmpty(selected) && string.Equals(s.name, selected, StringComparison.OrdinalIgnoreCase))
                    result.Insert(0, e);
                else
                    result.Add(e);
            }
            return result;
        }
    }
}

namespace ScriptHazemLauncher
{
    /// <summary>Row of the server list in the status card (only when more than one server is published).</summary>
    public class ServerRow
    {
        public string Name { get; set; }
        public string StateText { get; set; }
        public string Tip { get; set; }
        public System.Windows.Media.Brush DotBrush { get; set; }
        public System.Windows.Media.Brush RowBackground { get; set; }
        public System.Windows.Media.Brush RowBorder { get; set; }
    }
}
