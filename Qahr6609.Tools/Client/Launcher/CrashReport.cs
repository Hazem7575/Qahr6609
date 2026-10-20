using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ScriptHazemLauncher
{
    /// <summary>
    /// Writes a privacy-safe crash report (no passwords; the launcher never handles game login)
    /// to &lt;launcher&gt;\reports\crash_&lt;timestamp&gt;.txt. Nothing is uploaded anywhere.
    /// </summary>
    public static class CrashReport
    {
        public static string ReportsDir
        {
            get { return Path.Combine(App.ExeDir, "reports"); }
        }

        private static string Version()
        {
            try { return "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3); }
            catch { return "v?"; }
        }

        /// <summary>Game exited abnormally (non-zero exit code, or crashed within the watch window).</summary>
        public static string WriteGameCrash(LauncherConfig cfg, SystemCheckResult check, int exitCode, double ranSeconds)
        {
            StringBuilder sb = Header("game crash");
            string workDir = Safe(() => cfg.Resolve(cfg.WorkingDirFor()), cfg != null ? cfg.BaseDir : "");
            string exe = Safe(() => cfg.Resolve(cfg.GameExeFor()), "");

            sb.AppendLine("=== Game ===");
            sb.AppendLine("Server       : " + (string.IsNullOrWhiteSpace(cfg.SelectedServer) ? (cfg.ServerIP + ":" + cfg.LoginPort) : cfg.SelectedServer));
            sb.AppendLine("Game exe     : " + exe);
            sb.AppendLine("Arguments    : " + (cfg.GameArgs ?? ""));
            sb.AppendLine("Working dir  : " + workDir);
            sb.AppendLine("DirectX mode : " + cfg.DirectX);
            sb.AppendLine("Exit code    : " + exitCode + " (0x" + unchecked((uint)exitCode).ToString("X8") + ")");
            sb.AppendLine("Ran for      : " + ranSeconds.ToString("0.0", CultureInfo.InvariantCulture) + " s");
            sb.AppendLine("Classified   : " + (exitCode != 0 ? "non-zero exit" : "early exit (within watch window)"));
            sb.AppendLine();

            // if the pre-launch check was skipped/disabled, gather the system info now for the report
            if (check == null && cfg != null) check = Safe(() => SystemCheck.Run(cfg), null);
            AppendSystem(sb, check);
            AppendClientLogs(sb, workDir);

            string text = sb.ToString();
            string path = Save(sb);
            TryUpload(text, "game-crash", cfg);   // also send it to the team (best-effort; local copy always kept)
            return path;
        }

        /// <summary>The launcher itself threw (DispatcherUnhandledException / AppDomain).</summary>
        public static string WriteLauncherCrash(Exception ex)
        {
            StringBuilder sb = Header("launcher crash");
            sb.AppendLine("=== Exception ===");
            sb.AppendLine(ex == null ? "(null)" : ex.ToString());
            sb.AppendLine();
            LauncherConfig cfg = null;
            try
            {
                cfg = App.Config;
                SystemCheckResult check = cfg != null ? Safe(() => SystemCheck.Run(cfg), null) : null;
                AppendSystem(sb, check);
            }
            catch { }

            string text = sb.ToString();
            string path = Save(sb);
            TryUpload(text, "launcher-crash", cfg);
            return path;
        }

        /// <summary>
        /// Uploads the report to the patch server (POST, see LauncherConfig.CrashUrl) on a background
        /// thread so the UI never waits. Never throws: if the server is down or there is no network,
        /// the locally-saved report is still there. No passwords are ever in the report.
        /// </summary>
        public static void TryUpload(string body, string kind, LauncherConfig cfg)
        {
            try
            {
                if (cfg == null || string.IsNullOrEmpty(body)) return;
                string url = cfg.CrashUrl();
                if (string.IsNullOrEmpty(url)) return;
                byte[] data = new UTF8Encoding(false).GetBytes(body);
                Thread t = new Thread(delegate () { Post(url, data, kind); });
                t.IsBackground = true;
                t.Start();
            }
            catch { }
        }

        private static void Post(string url, byte[] data, string kind)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "text/plain; charset=utf-8";
                req.ContentLength = data.Length;
                req.Timeout = 8000;
                req.ReadWriteTimeout = 8000;
                req.KeepAlive = false;
                req.UserAgent = "HazemLauncher/" + Version();
                try { req.Headers["X-Report-Kind"] = kind ?? ""; } catch { }
                try { req.Headers["X-Launcher-Version"] = Version(); } catch { }
                using (Stream s = req.GetRequestStream()) s.Write(data, 0, data.Length);
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                using (Stream r = resp.GetResponseStream())
                {
                    if (r != null) { byte[] scratch = new byte[256]; while (r.Read(scratch, 0, scratch.Length) > 0) { } }
                }
            }
            catch { /* server unreachable / offline: the local report is enough */ }
        }

        private static StringBuilder Header(string kind)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Hazem Launcher - crash report (" + kind + ")");
            sb.AppendLine("Generated : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("Launcher  : " + Version());
            sb.AppendLine("Language  : " + Lang.Current);
            sb.AppendLine(new string('-', 60));
            sb.AppendLine();
            return sb;
        }

        private static void AppendSystem(StringBuilder sb, SystemCheckResult check)
        {
            sb.AppendLine("=== System ===");
            if (check != null)
            {
                sb.AppendLine("OS        : " + check.OsInfo);
                sb.AppendLine("DirectX   : " + check.DxInfo);
                sb.AppendLine("VC runtime: " + check.VcInfo);
                sb.AppendLine("Disk      : " + check.DiskInfo);
                sb.AppendLine("Admin     : " + check.Admin);
                sb.AppendLine();
                sb.AppendLine("=== System check ===");
                foreach (CheckItem i in check.Items)
                {
                    string tag = i.Status == CheckStatus.Fail ? "[FAIL]" : i.Status == CheckStatus.Warn ? "[WARN]" : "[ OK ]";
                    sb.AppendLine(tag + " " + i.Title + (string.IsNullOrEmpty(i.Detail) ? "" : " - " + i.Detail));
                }
            }
            else
            {
                try { sb.AppendLine("OS        : " + System.Runtime.InteropServices.RuntimeInformation.OSDescription); }
                catch { sb.AppendLine("OS        : " + Environment.OSVersion); }
            }
            sb.AppendLine();
        }

        /// <summary>
        /// Finds the newest *.log / *.dmp / error dumps inside the client folder (both the 6609 client's
        /// debug\ logs and the 7957 client's [Logs] + NdSafeLogin.dmp) and appends the last lines.
        /// </summary>
        private static void AppendClientLogs(StringBuilder sb, string workDir)
        {
            sb.AppendLine("=== Client logs / dumps ===");
            try
            {
                List<string> roots = new List<string>();
                if (!string.IsNullOrEmpty(workDir) && Directory.Exists(workDir)) roots.Add(workDir);
                try
                {
                    string parent = Directory.GetParent(workDir)?.FullName;
                    if (!string.IsNullOrEmpty(parent) && Directory.Exists(parent) && !roots.Contains(parent)) roots.Add(parent);
                }
                catch { }

                List<FileInfo> found = new List<FileInfo>();
                string[] patterns = { "*.log", "*.dmp", "*.err", "error*.txt" };
                foreach (string root in roots)
                {
                    foreach (string pat in patterns)
                    {
                        try
                        {
                            foreach (string f in Directory.EnumerateFiles(root, pat, SearchOption.AllDirectories))
                            {
                                try { found.Add(new FileInfo(f)); } catch { }
                            }
                        }
                        catch { }
                    }
                }
                found.Sort((a, b) => b.LastWriteTimeUtc.CompareTo(a.LastWriteTimeUtc));

                int shown = 0;
                HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (FileInfo fi in found)
                {
                    if (shown >= 3) break;                                   // the 3 most recent logs is enough
                    if (!seen.Add(fi.FullName)) continue;
                    if (fi.LastWriteTimeUtc < DateTime.UtcNow.AddDays(-7)) continue; // ignore stale logs
                    sb.AppendLine("--- " + fi.FullName + "  (" + fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") + ", " + fi.Length + " bytes) ---");
                    if (string.Equals(fi.Extension, ".dmp", StringComparison.OrdinalIgnoreCase))
                        sb.AppendLine("(binary crash dump present - not inlined)");
                    else
                        sb.AppendLine(Tail(fi.FullName, 40));
                    sb.AppendLine();
                    shown++;
                }
                if (shown == 0) sb.AppendLine("(no recent client log or dump found)");
            }
            catch (Exception ex) { sb.AppendLine("(could not read client logs: " + ex.Message + ")"); }
        }

        private static string Tail(string path, int lines)
        {
            try
            {
                byte[] raw;
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long len = fs.Length;
                    long take = Math.Min(len, 64 * 1024);   // last 64 KB at most
                    fs.Seek(len - take, SeekOrigin.Begin);
                    raw = new byte[take];
                    int read = fs.Read(raw, 0, (int)take);
                    if (read < take) Array.Resize(ref raw, read);
                }
                string text;
                try { text = new UTF8Encoding(false, true).GetString(raw); }
                catch { text = Encoding.GetEncoding(1252).GetString(raw); }
                string[] all = text.Replace("\r\n", "\n").Split('\n');
                int start = Math.Max(0, all.Length - lines);
                StringBuilder sb = new StringBuilder();
                for (int i = start; i < all.Length; i++) sb.AppendLine(all[i].TrimEnd());
                return sb.ToString().TrimEnd();
            }
            catch (Exception ex) { return "(could not read: " + ex.Message + ")"; }
        }

        private static string Save(StringBuilder sb)
        {
            try
            {
                Directory.CreateDirectory(ReportsDir);
                string path = Path.Combine(ReportsDir, "crash_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
                int n = 1;
                while (File.Exists(path))
                    path = Path.Combine(ReportsDir, "crash_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + (n++) + ".txt");
                File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
                return path;
            }
            catch
            {
                // reports folder not writable: fall back next to the exe so the info is never lost
                try
                {
                    string path = Path.Combine(App.ExeDir, "crash_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
                    File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
                    return path;
                }
                catch { return null; }
            }
        }

        /// <summary>Opens Explorer with the report selected (or the reports folder).</summary>
        public static void OpenFolder(string reportPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(reportPath) && File.Exists(reportPath))
                    Process.Start(new ProcessStartInfo("explorer.exe", "/select,\"" + reportPath + "\"") { UseShellExecute = true });
                else
                    Process.Start(new ProcessStartInfo(ReportsDir) { UseShellExecute = true });
            }
            catch { }
        }

        private static T Safe<T>(Func<T> f, T fallback)
        {
            try { return f(); } catch { return fallback; }
        }
    }
}
