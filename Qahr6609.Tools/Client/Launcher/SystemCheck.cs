using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace ScriptHazemLauncher
{
    /// <summary>Result of one check row (shown in the panel and written to a crash report).</summary>
    public enum CheckStatus { Ok, Warn, Fail }

    public class CheckItem
    {
        public CheckStatus Status;
        public string Title;
        public string Detail;
        public string FixText;   // short "how to fix" (null = none)
        public string FixUrl;    // download link from launcher.ini (null = no button)
    }

    public class SystemCheckResult
    {
        public readonly List<CheckItem> Items = new List<CheckItem>();
        public string OsInfo = "";
        public string DxInfo = "";
        public string VcInfo = "";
        public string DiskInfo = "";
        public bool Admin;

        public bool HasFail
        {
            get { foreach (CheckItem i in Items) if (i.Status == CheckStatus.Fail) return true; return false; }
        }
        public bool HasWarn
        {
            get { foreach (CheckItem i in Items) if (i.Status == CheckStatus.Warn) return true; return false; }
        }

        public void Add(CheckStatus s, string title, string detail, string fixText = null, string fixUrl = null)
        {
            Items.Add(new CheckItem { Status = s, Title = title, Detail = detail, FixText = fixText, FixUrl = fixUrl });
        }
    }

    /// <summary>
    /// Pre-launch dependency check. Nothing is hard-coded to one client: the working dir, the DirectX
    /// mode and every threshold/URL come from launcher.ini so both the 6609 and 7961 clients work.
    /// </summary>
    public static class SystemCheck
    {
        /// <summary>Runs every enabled check. Safe to call on a background thread (no UI).</summary>
        public static SystemCheckResult Run(LauncherConfig cfg)
        {
            SystemCheckResult r = new SystemCheckResult();
            string workDir = SafeResolve(cfg, cfg.WorkingDirFor());

            // ---- OS / platform (informational, always listed) ----
            try
            {
                r.OsInfo = RuntimeInformation.OSDescription +
                    (Environment.Is64BitOperatingSystem ? " (64-bit)" : " (32-bit)") +
                    " | launcher: " + (Environment.Is64BitProcess ? "x64" : "x86");
            }
            catch { r.OsInfo = Environment.OSVersion.ToString(); }
            r.Add(CheckStatus.Ok, Lang.T("SysOs"), r.OsInfo);

            // ---- DirectX ----
            if (cfg.CheckDirectX)
                CheckDirectX(cfg, workDir, r);

            // ---- Visual C++ runtime (client ships Microsoft.VC90.CRT) ----
            if (cfg.CheckVCRuntime)
                CheckVcRuntime(cfg, workDir, r);

            // ---- free disk space ----
            CheckDisk(cfg, workDir, r);

            // ---- required files (ini-driven; used for testing a missing dependency too) ----
            CheckRequiredFiles(cfg, workDir, r);

            // ---- admin rights (note only, never a hard fail) ----
            try
            {
                r.Admin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch { r.Admin = false; }
            r.Add(CheckStatus.Ok, Lang.T("SysAdmin"),
                r.Admin ? Lang.T("SysAdminYes") : Lang.T("SysAdminNo"));

            return r;
        }

        private static void CheckDirectX(LauncherConfig cfg, string workDir, SystemCheckResult r)
        {
            // The game is a 32-bit process; Environment.SystemDirectory is redirected to SysWOW64 for us,
            // so File.Exists finds the 32-bit system DLL the client actually loads. The client's
            // Play(DX9).bat deletes the app-local d3d9.dll on purpose so the SYSTEM d3d9.dll is used,
            // so the system DLL (not an app-local copy) is what we must verify.
            string sys = Environment.SystemDirectory;
            if (cfg.DirectX == 8)
            {
                bool d3d8 = Exists(sys, "d3d8.dll");
                string d3dx8 = FindFirst(workDir, "D3DX8_56.dll", "d3dx8_43.dll", "d3dx8d.dll");
                r.DxInfo = "DX8 d3d8.dll=" + (d3d8 ? "ok" : "MISSING") + " d3dx8=" + (d3dx8 ?? "MISSING");
                r.Add(d3d8 ? CheckStatus.Ok : CheckStatus.Warn, Lang.T("SysDx") + " 8",
                    d3d8 ? Lang.T("SysDxOk") : Lang.T("SysDxMissing"),
                    d3d8 ? null : Lang.T("SysDxFix"), d3d8 ? null : NullIfEmpty(cfg.DirectXUrl));
                if (d3dx8 == null)
                    r.Add(CheckStatus.Warn, "D3DX8", Lang.T("SysDxxMissing"), Lang.T("SysDxFix"), NullIfEmpty(cfg.DirectXUrl));
            }
            else
            {
                bool d3d9 = Exists(sys, "d3d9.dll");
                string d3dx9 = FindFirst(workDir, "D3DX9_43.dll", "d3dx9_43.dll", "d3dx9_42.dll", "d3dx9_41.dll");
                r.DxInfo = "DX9 d3d9.dll=" + (d3d9 ? "ok" : "MISSING") + " d3dx9=" + (d3dx9 ?? "MISSING");
                r.Add(d3d9 ? CheckStatus.Ok : CheckStatus.Warn, Lang.T("SysDx") + " 9",
                    d3d9 ? Lang.T("SysDxOk") : Lang.T("SysDxMissing"),
                    d3d9 ? null : Lang.T("SysDxFix"), d3d9 ? null : NullIfEmpty(cfg.DirectXUrl));
                if (d3dx9 == null)
                    r.Add(CheckStatus.Warn, "D3DX9", Lang.T("SysDxxMissing"), Lang.T("SysDxFix"), NullIfEmpty(cfg.DirectXUrl));
            }
        }

        private static void CheckVcRuntime(LauncherConfig cfg, string workDir, SystemCheckResult r)
        {
            // 1) client ships the runtime side-by-side (Microsoft.VC90.CRT\msvcr90.dll or app-local msvcr90.dll)
            string bundled = FindVc90(workDir);
            if (bundled != null)
            {
                r.VcInfo = "VC90 bundled: " + bundled;
                r.Add(CheckStatus.Ok, Lang.T("SysVc"), Lang.T("SysVcBundled"));
                return;
            }
            // 2) installed VC++ 2008 (x86) runtime, or the DLL in the system folder
            bool installed = IsVc2008Installed() || Exists(Environment.SystemDirectory, "msvcr90.dll");
            r.VcInfo = "VC90 installed=" + installed;
            r.Add(installed ? CheckStatus.Ok : CheckStatus.Warn, Lang.T("SysVc"),
                installed ? Lang.T("SysVcInstalled") : Lang.T("SysVcMissing"),
                installed ? null : Lang.T("SysVcFix"), installed ? null : NullIfEmpty(cfg.VCRedistUrl));
        }

        private static void CheckDisk(LauncherConfig cfg, string workDir, SystemCheckResult r)
        {
            try
            {
                string root = Path.GetPathRoot(Path.GetFullPath(string.IsNullOrEmpty(workDir) ? cfg.BaseDir : workDir));
                DriveInfo di = new DriveInfo(root);
                long freeMB = di.AvailableFreeSpace / (1024 * 1024);
                r.DiskInfo = root + " " + freeMB + " MB free (min " + cfg.MinFreeDiskMB + " MB)";
                bool ok = freeMB >= cfg.MinFreeDiskMB;
                r.Add(ok ? CheckStatus.Ok : CheckStatus.Warn, Lang.T("SysDisk"),
                    string.Format(ok ? Lang.T("SysDiskOk") : Lang.T("SysDiskLow"), freeMB, root.TrimEnd('\\')),
                    ok ? null : Lang.T("SysDiskFix"));
            }
            catch (Exception ex) { r.DiskInfo = "disk check failed: " + ex.Message; }
        }

        private static void CheckRequiredFiles(LauncherConfig cfg, string workDir, SystemCheckResult r)
        {
            if (cfg.RequiredFiles == null) return;
            foreach (string rel in cfg.RequiredFiles)
            {
                if (string.IsNullOrWhiteSpace(rel)) continue;
                string full = Path.IsPathRooted(rel) ? rel : Path.Combine(workDir, rel);
                bool ok = File.Exists(full);
                r.Add(ok ? CheckStatus.Ok : CheckStatus.Fail, Lang.T("SysRequired") + ": " + rel,
                    ok ? Lang.T("SysReqOk") : Lang.T("SysReqMissing"),
                    ok ? null : Lang.T("SysReqFix"));
            }
        }

        // ---- helpers ----

        private static string SafeResolve(LauncherConfig cfg, string rel)
        {
            try { return cfg.Resolve(rel); } catch { return cfg.BaseDir; }
        }

        private static bool Exists(string dir, string file)
        {
            try { return !string.IsNullOrEmpty(dir) && File.Exists(Path.Combine(dir, file)); }
            catch { return false; }
        }

        private static string FindFirst(string dir, params string[] names)
        {
            foreach (string n in names) if (Exists(dir, n)) return n;
            return null;
        }

        private static string FindVc90(string workDir)
        {
            string[] probes =
            {
                Path.Combine(workDir ?? "", "Microsoft.VC90.CRT", "msvcr90.dll"),
                Path.Combine(workDir ?? "", "msvcr90.dll"),
            };
            foreach (string p in probes)
            {
                try { if (File.Exists(p)) return p; } catch { }
            }
            return null;
        }

        private static bool IsVc2008Installed()
        {
            // 32-bit process -> the registry view is already Wow6432Node, where the x86 runtime key lives.
            try
            {
                using (Microsoft.Win32.RegistryKey k = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\VisualStudio\9.0\VC\Runtimes\x86"))
                {
                    if (k != null)
                    {
                        object v = k.GetValue("Installed");
                        if (v != null && Convert.ToInt32(v) == 1) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private static string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        /// <summary>Short multi-line summary of the problems (for the themed warning dialog).</summary>
        public static string Summary(SystemCheckResult r, bool problemsOnly)
        {
            StringBuilder sb = new StringBuilder();
            foreach (CheckItem i in r.Items)
            {
                if (problemsOnly && i.Status == CheckStatus.Ok) continue;
                sb.Append(i.Status == CheckStatus.Fail ? "✖ " : i.Status == CheckStatus.Warn ? "⚠ " : "✔ ");
                sb.Append(i.Title);
                if (!string.IsNullOrEmpty(i.Detail)) { sb.Append(" — "); sb.Append(i.Detail); }
                if (!string.IsNullOrEmpty(i.FixText)) { sb.Append("\n   "); sb.Append(i.FixText); }
                sb.Append('\n');
            }
            return sb.ToString().TrimEnd();
        }
    }
}
