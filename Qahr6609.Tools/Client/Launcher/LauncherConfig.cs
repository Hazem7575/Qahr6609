using System;
using System.Collections.Generic;
using System.IO;

namespace ScriptHazemLauncher
{
    /// <summary>Values from launcher.ini (next to the launcher exe).</summary>
    public class LauncherConfig
    {
        public string BaseDir;
        public string UpdateUrl;
        /// <summary>Primary UpdateUrl first, then launcher.ini UpdateUrlMirror1..N (the updater falls back down the list).</summary>
        public string[] UpdateUrls;
        /// <summary>launcher.ini DownloadThreads = how many files download at once (default 3, clamped 1..8).</summary>
        public int DownloadThreads;
        /// <summary>launcher.ini DownloadKBps = global download speed cap in KB/s (0 = unlimited).</summary>
        public int DownloadKBps;
        public string ServerIP;
        public int LoginPort;
        /// <summary>Game server port for the Online/Offline check (launcher.ini GamePort=, 0 = only the login port).</summary>
        public int GamePort;
        public string GameExe;
        public string GameArgs;
        public string WorkingDir;
        public string SettingsIni;
        public bool CloseOnPlay;
        public string Facebook;
        public string Discord;
        public string Website;
        public string ThemeName;
        public string Language;
        /// <summary>Client server-list file written from the patch server's /servers (empty = never touch it).</summary>
        public string ServerListFile;
        /// <summary>Game server the player picked last (only used when more than one server is published).</summary>
        public string SelectedServer;
        /// <summary>8 or 9 (launcher.ini DirectX=, optional). Swaps Env_DX9/Env_DX8 in GameExe and WorkingDir.</summary>
        public int DirectX;

        // ---- [SystemCheck] (all optional; defaults keep old launcher.ini working) ----
        /// <summary>Run the dependency check before the game starts (launcher.ini [SystemCheck] Enabled, default 1).</summary>
        public bool SysCheckEnabled;
        /// <summary>Block the launch when a hard requirement (RequiredFiles) fails; otherwise only warn (default 0).</summary>
        public bool SysCheckBlockOnFail;
        /// <summary>Minimum free disk space (MB) on the client drive before a warning (default 500).</summary>
        public long MinFreeDiskMB;
        public bool CheckDirectX;
        public bool CheckVCRuntime;
        /// <summary>Download page for the VC++ runtime (shown in the fix hint; nothing is bundled/auto-downloaded).</summary>
        public string VCRedistUrl;
        /// <summary>Download page for the DirectX runtime (shown in the fix hint).</summary>
        public string DirectXUrl;
        /// <summary>Files that MUST exist (relative to the working dir, or absolute); missing = hard fail. Also used to test a missing dependency.</summary>
        public string[] RequiredFiles;
        /// <summary>Detect an abnormal game exit and offer a crash report (default 1).</summary>
        public bool CrashReporting;
        /// <summary>Also upload the crash report to the patch server so the team gets it automatically (default 1).</summary>
        public bool CrashUpload;
        /// <summary>Where crash reports are POSTed (launcher.ini [SystemCheck] CrashUploadUrl). Blank = UpdateUrl + "crash".</summary>
        public string CrashUploadUrl;
        /// <summary>A game exit within this many seconds counts as an early crash; also how long the launcher watches before honouring CloseOnPlay (default 15).</summary>
        public int CrashWatchSeconds;

        // ---- [Music] user preferences (persisted; theme.ini [Music] holds the defaults/content) ----
        /// <summary>launcher.ini [Music] Volume 0..100 that the player last chose (-1 = not set, use the theme default).</summary>
        public int MusicUserVolume = -1;
        /// <summary>launcher.ini [Music] Muted (1/0) that the player last chose (-1 = not set, use the theme StartMuted).</summary>
        public int MusicUserMuted = -1;

        private IniText _ini;

        public static LauncherConfig Load()
        {
            string dir = App.ExeDir;
            IniText ini = new IniText(Path.Combine(dir, "launcher.ini"));
            LauncherConfig c = new LauncherConfig();
            c._ini = ini;
            c.BaseDir = dir;
            c.UpdateUrl = ini.Get("Launcher", "UpdateUrl", "");
            if (c.UpdateUrl.Length > 0 && !c.UpdateUrl.EndsWith("/")) c.UpdateUrl += "/";
            c.ServerIP = ini.Get("Launcher", "ServerIP", "");
            c.LoginPort = ini.GetInt("Launcher", "LoginPort", 9961);
            c.GamePort = ini.GetInt("Launcher", "GamePort", 0);
            c.GameExe = ini.Get("Launcher", "GameExe", @"Env_DX9\Conquer.exe");
            c.GameArgs = ini.Get("Launcher", "GameArgs", "blacknull");
            c.WorkingDir = ini.Get("Launcher", "WorkingDir", "Env_DX9");
            c.SettingsIni = ini.Get("Launcher", "SettingsIni", "Settings.ini");
            c.CloseOnPlay = ini.Get("Launcher", "CloseOnPlay", "1") != "0";
            c.Facebook = ini.Get("Launcher", "Facebook", "");
            c.Discord = ini.Get("Launcher", "Discord", "");
            c.Website = ini.Get("Launcher", "Website", "");
            c.ThemeName = ini.Get("Launcher", "Theme", "default");
            c.Language = ini.Get("Launcher", "Language", "ar");
            c.ServerListFile = ini.Get("Launcher", "ServerListFile", "Guard.dat");
            c.SelectedServer = ini.Get("Launcher", "Server", "");
            c.DirectX = ini.GetInt("Launcher", "DirectX", c.GameExe.IndexOf("DX8", StringComparison.OrdinalIgnoreCase) >= 0 ? 8 : 9) == 8 ? 8 : 9;

            // download: mirror list (primary + UpdateUrlMirror1..N) and the optional parallelism / speed cap
            List<string> urls = new List<string>();
            if (c.UpdateUrl.Length > 0) urls.Add(c.UpdateUrl);
            for (int i = 1; i <= 8; i++)
            {
                string m = ini.Get("Launcher", "UpdateUrlMirror" + i, "").Trim();
                if (m.Length == 0) continue;
                if (!m.EndsWith("/")) m += "/";
                if (!urls.Contains(m)) urls.Add(m);
            }
            c.UpdateUrls = urls.ToArray();
            c.DownloadThreads = ini.GetInt("Launcher", "DownloadThreads", 3);
            c.DownloadKBps = ini.GetInt("Launcher", "DownloadKBps", 0);

            // [SystemCheck] - every key optional so an old launcher.ini keeps working
            c.SysCheckEnabled = ini.Get("SystemCheck", "Enabled", "1") != "0";
            c.SysCheckBlockOnFail = ini.Get("SystemCheck", "BlockOnFail", "0") == "1";
            c.MinFreeDiskMB = ini.GetInt("SystemCheck", "MinFreeDiskMB", 500);
            c.CheckDirectX = ini.Get("SystemCheck", "CheckDirectX", "1") != "0";
            c.CheckVCRuntime = ini.Get("SystemCheck", "CheckVCRuntime", "1") != "0";
            c.VCRedistUrl = ini.Get("SystemCheck", "VCRedistUrl", "");
            c.DirectXUrl = ini.Get("SystemCheck", "DirectXUrl", "");
            c.RequiredFiles = SplitList(ini.Get("SystemCheck", "RequiredFiles", ""));
            c.CrashReporting = ini.Get("SystemCheck", "CrashReporting", "1") != "0";
            c.CrashUpload = ini.Get("SystemCheck", "CrashUpload", "1") != "0";
            c.CrashUploadUrl = ini.Get("SystemCheck", "CrashUploadUrl", "").Trim();
            c.CrashWatchSeconds = ini.GetInt("SystemCheck", "CrashWatchSeconds", 15);
            if (c.CrashWatchSeconds < 1) c.CrashWatchSeconds = 1;

            // [Music] user prefs - optional, blank means "use the theme default"
            c.MusicUserVolume = ParseOpt(ini.Get("Music", "Volume", ""), -1);
            if (c.MusicUserVolume > 100) c.MusicUserVolume = 100;
            string mm = ini.Get("Music", "Muted", "").Trim().ToLowerInvariant();
            c.MusicUserMuted = mm.Length == 0 ? -1 : (mm == "1" || mm == "true" || mm == "yes" || mm == "on" ? 1 : 0);
            return c;
        }

        private static int ParseOpt(string s, int def)
        {
            int v;
            return int.TryParse(s.Trim(), out v) ? v : def;
        }

        public void SaveMusicVolume(int volume0to100)
        {
            MusicUserVolume = volume0to100 < 0 ? 0 : (volume0to100 > 100 ? 100 : volume0to100);
            try { _ini.Set("Music", "Volume", MusicUserVolume.ToString()); _ini.Save(); }
            catch { /* read-only folder: keep the value for this session only */ }
        }

        public void SaveMusicMuted(bool muted)
        {
            MusicUserMuted = muted ? 1 : 0;
            try { _ini.Set("Music", "Muted", muted ? "1" : "0"); _ini.Save(); }
            catch { }
        }

        private static string[] SplitList(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            string[] parts = s.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>();
            foreach (string p in parts) { string t = p.Trim(); if (t.Length > 0) list.Add(t); }
            return list.Count == 0 ? null : list.ToArray();
        }

        /// <summary>GameExe / WorkingDir for the chosen DirectX version (Env_DX9 &lt;-&gt; Env_DX8).</summary>
        public string GameExeFor() { return SwapDx(GameExe); }
        public string WorkingDirFor() { return SwapDx(WorkingDir); }

        private string SwapDx(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            string from = DirectX == 8 ? "Env_DX9" : "Env_DX8";
            string to = DirectX == 8 ? "Env_DX8" : "Env_DX9";
            int i = path.IndexOf(from, StringComparison.OrdinalIgnoreCase);
            return i < 0 ? path : path.Substring(0, i) + to + path.Substring(i + from.Length);
        }

        public void SaveDirectX(int dx)
        {
            DirectX = dx == 8 ? 8 : 9;
            try
            {
                _ini.Set("Launcher", "DirectX", DirectX.ToString());
                _ini.Save();
            }
            catch { }
        }

        public void SaveCloseOnPlay(bool value)
        {
            CloseOnPlay = value;
            try
            {
                _ini.Set("Launcher", "CloseOnPlay", value ? "1" : "0");
                _ini.Save();
            }
            catch { /* read-only folder: keep the value for this session only */ }
        }

        public void SaveLanguage(string code)
        {
            Language = code;
            try
            {
                _ini.Set("Launcher", "Language", code);
                _ini.Save();
            }
            catch { }
        }

        public void SaveSysCheckEnabled(bool value)
        {
            SysCheckEnabled = value;
            try
            {
                _ini.Set("SystemCheck", "Enabled", value ? "1" : "0");
                _ini.Save();
            }
            catch { /* read-only folder: keep the value for this session only */ }
        }

        public void SaveSelectedServer(string name)
        {
            SelectedServer = name;
            try
            {
                _ini.Set("Launcher", "Server", name ?? "");
                _ini.Save();
            }
            catch { }
        }

        /// <summary>
        /// Full URL crash reports are POSTed to: the explicit CrashUploadUrl, or UpdateUrl + "crash".
        /// Returns "" when uploading is off or no URL can be formed (then reports stay local only).
        /// </summary>
        public string CrashUrl()
        {
            if (!CrashUpload) return "";
            if (!string.IsNullOrEmpty(CrashUploadUrl)) return CrashUploadUrl;
            if (!string.IsNullOrEmpty(UpdateUrl)) return UpdateUrl + "crash"; // UpdateUrl already ends with '/'
            return "";
        }

        public string Resolve(string relative)
        {
            if (string.IsNullOrEmpty(relative)) return BaseDir;
            return Path.IsPathRooted(relative) ? relative : Path.GetFullPath(Path.Combine(BaseDir, relative));
        }
    }
}
