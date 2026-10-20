using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace PatchServer
{
    /// <summary>
    /// Receives crash reports uploaded by the launcher (POST /crash) and writes each one to
    /// Root\data\crashes\yyyy-MM-dd\crash_HHmmss_&lt;ip&gt;_&lt;kind&gt;.txt. One folder per day keeps the
    /// list easy to browse; nothing is ever read back or served, so there is no index to keep.
    /// The report is privacy-safe plain text built by the launcher (no passwords - the launcher
    /// never handles the game login).
    /// </summary>
    internal sealed class CrashStore
    {
        private readonly string baseDir;
        private long totalSaved;

        public CrashStore(string crashDir)
        {
            baseDir = crashDir;
        }

        /// <summary>Folder crash reports are written under.</summary>
        public string Dir { get { return baseDir; } }

        /// <summary>Reports saved since the server started.</summary>
        public long TotalSaved { get { return Interlocked.Read(ref totalSaved); } }

        /// <summary>
        /// Saves one report. body is the raw uploaded text; remote/kind/version/note come from the
        /// request. Returns the saved file name (not the full path) or null on failure. Never throws.
        /// </summary>
        public string Save(string remote, byte[] body, string kind, string version, string note)
        {
            try
            {
                DateTime now = DateTime.Now;
                string dayDir = Path.Combine(baseDir, now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                Directory.CreateDirectory(dayDir);

                string ip = Sanitize(remote, 39);
                string tag = Sanitize(kind, 24);
                string name = "crash_" + now.ToString("HHmmss", CultureInfo.InvariantCulture)
                    + "_" + ip + (tag.Length > 0 ? "_" + tag : "") + ".txt";

                string path = Path.Combine(dayDir, name);
                int n = 1;
                while (File.Exists(path))
                {
                    name = "crash_" + now.ToString("HHmmss", CultureInfo.InvariantCulture)
                        + "_" + ip + (tag.Length > 0 ? "_" + tag : "") + "_" + (n++).ToString(CultureInfo.InvariantCulture) + ".txt";
                    path = Path.Combine(dayDir, name);
                }

                // A short header of our own first (the uploaded body already carries the launcher's header,
                // but these three lines record what the *server* saw, which the body cannot forge usefully).
                StringBuilder head = new StringBuilder();
                head.Append("# received ").Append(now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
                    .Append(" from ").Append(remote ?? "?");
                if (!string.IsNullOrEmpty(version)) head.Append("  launcher=").Append(Clean(version, 32));
                if (!string.IsNullOrEmpty(note)) head.Append("  note=").Append(Clean(note, 200));
                head.Append("\r\n").Append(new string('-', 60)).Append("\r\n");

                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                {
                    byte[] h = Encoding.UTF8.GetBytes(head.ToString());
                    fs.Write(h, 0, h.Length);
                    if (body != null && body.Length > 0) fs.Write(body, 0, body.Length);
                }

                Interlocked.Increment(ref totalSaved);
                Log.Info("crash report saved: " + Path.Combine(now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), name)
                    + " (" + (body != null ? body.Length : 0).ToString(CultureInfo.InvariantCulture) + " bytes from " + (remote ?? "?") + ")");
                return name;
            }
            catch (Exception ex)
            {
                Log.Warn("crash report not saved: " + ex.Message);
                return null;
            }
        }

        /// <summary>Keeps only characters safe for a file name; trims to max. Empty-safe.</summary>
        private static string Sanitize(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "unknown";
            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (c == '.' || c == '-' || c == '_'
                    || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    sb.Append(c);
                else
                    sb.Append('_');
                if (sb.Length >= max) break;
            }
            string r = sb.ToString().Trim('_');
            return r.Length == 0 ? "unknown" : r;
        }

        /// <summary>Strips control characters from a header value so a crafted header cannot break the log line.</summary>
        private static string Clean(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            StringBuilder sb = new StringBuilder(Math.Min(s.Length, max));
            foreach (char c in s)
            {
                if (c < ' ' || c == '\u007f') sb.Append(' ');
                else sb.Append(c);
                if (sb.Length >= max) break;
            }
            return sb.ToString().Trim();
        }
    }
}
