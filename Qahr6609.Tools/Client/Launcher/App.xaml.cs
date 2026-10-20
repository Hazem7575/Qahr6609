using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ScriptHazemLauncher
{
    public partial class App : Application
    {
        public static LauncherConfig Config { get; private set; }
        public static Theme CurrentTheme { get; private set; }

        // ---- hidden debug switches (screenshots / tests only, never needed by players) ----
        /// <summary>--demo-state=update|updating|ready|maintenance : fake the play state + progress, the updater does not run.</summary>
        public static string DemoState;
        /// <summary>--demo-servers : fake a /servers answer with two servers (shows the picker).</summary>
        public static bool DemoServers;
        /// <summary>--lang=ar|en : language for this run only (launcher.ini is not changed).</summary>
        public static string LangOverride;
        /// <summary>--fit=WxH : pretend the screen work area is WxH (DIPs) to test the down-scaling.</summary>
        public static Size? FitOverride;
        /// <summary>--snapshot=file.png : render the window to a PNG after 2.5 s and exit.</summary>
        public static string SnapshotPath;
        /// <summary>--embers-always : keep the embers running when the window is not focused (CPU worst-case measurement).</summary>
        public static bool EmbersAlways;
        /// <summary>--startup-log : append "first frame after N ms" to startup_time.log next to the exe.</summary>
        public static bool StartupLog;
        /// <summary>--demo-open=settings|dialog|servers : open that panel (screenshots).</summary>
        public static string DemoOpen;
        /// <summary>--demo-status=online|offline|maintenance : fake the status pill (screenshots).</summary>
        public static string DemoStatus;

        /// <summary>
        /// Folder of the real exe. (AppContext.BaseDirectory can point to the single-file
        /// extraction folder in %TEMP%, so it must not be used for game/theme paths.)
        /// </summary>
        public static readonly string ExeDir = GetExeDir();

        private static string GetExeDir()
        {
            string p = Environment.ProcessPath;
            string d = string.IsNullOrEmpty(p) ? AppContext.BaseDirectory : Path.GetDirectoryName(p);
            return d.EndsWith(Path.DirectorySeparatorChar.ToString()) ? d : d + Path.DirectorySeparatorChar;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Always work relative to the launcher folder (shortcuts may start us elsewhere).
            Directory.SetCurrentDirectory(ExeDir);
            // ANSI code pages (game Settings.ini may be saved in the Windows code page)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            DispatcherUnhandledException += OnUnhandled;
            // background-thread / terminating crashes: write a report file even when no UI can be shown
            AppDomain.CurrentDomain.UnhandledException += delegate (object s, UnhandledExceptionEventArgs ue)
            {
                try { CrashReport.WriteLauncherCrash(ue.ExceptionObject as Exception); } catch { }
            };
            // every animation (entrance, hover zoom) runs at 30 fps at most - keeps the layered window cheap
            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline), new FrameworkPropertyMetadata(30));
            ParseArgs(e.Args);
            base.OnStartup(e);

            ExtractDefaults(ExeDir);
            Config = LauncherConfig.Load();
            Lang.Set(LangOverride ?? Config.Language);
            ReloadTheme();
            MainWindow w = new MainWindow();
            MainWindow = w;
            w.Show();
        }

        private static void ParseArgs(string[] args)
        {
            foreach (string a in args ?? new string[0])
            {
                int eq = a.IndexOf('=');
                string k = (eq > 0 ? a.Substring(0, eq) : a).ToLowerInvariant();
                string v = eq > 0 ? a.Substring(eq + 1).Trim('"') : "";
                if (k == "--demo-state") DemoState = v.ToLowerInvariant();
                else if (k == "--demo-servers") DemoServers = true;
                else if (k == "--embers-always") EmbersAlways = true;
                else if (k == "--startup-log") StartupLog = true;
                else if (k == "--demo-open") DemoOpen = v.ToLowerInvariant();
                else if (k == "--demo-status") DemoStatus = v.ToLowerInvariant();
                else if (k == "--lang") LangOverride = v;
                else if (k == "--snapshot") SnapshotPath = v;
                else if (k == "--fit")
                {
                    string[] p = v.ToLowerInvariant().Split('x');
                    double w, h;
                    if (p.Length == 2 && double.TryParse(p[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out w)
                        && double.TryParse(p[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out h))
                        FitOverride = new Size(w, h);
                }
            }
        }

        /// <summary>Reads theme.ini again and pushes colours/fonts into the app resources.</summary>
        public static Theme ReloadTheme()
        {
            CurrentTheme = Theme.Load(Config.BaseDir, Config.ThemeName);
            CurrentTheme.ApplyResources(Current.Resources);
            return CurrentTheme;
        }

        /// <summary>
        /// The exe carries a copy of launcher.ini and the default theme. When they are missing
        /// (player copied only the exe) they are written next to the exe. Existing files are never overwritten.
        /// </summary>
        private static void ExtractDefaults(string baseDir)
        {
            Assembly asm = typeof(App).Assembly;
            foreach (string name in asm.GetManifestResourceNames())
            {
                string rel;
                if (name == "launcher.ini") rel = "launcher.ini";
                else if (name.StartsWith("theme/", StringComparison.Ordinal)) rel = Path.Combine("themes", name.Substring(6));
                else continue;
                rel = rel.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                string target = Path.Combine(baseDir, rel);
                if (File.Exists(target)) continue;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    using (Stream src = asm.GetManifestResourceStream(name))
                    using (FileStream dst = File.Create(target))
                        src.CopyTo(dst);
                }
                catch { /* read-only folder: the launcher still works with built-in defaults */ }
            }
        }

        private void OnUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                File.AppendAllText(Path.Combine(ExeDir, "launcher_error.log"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + e.Exception + Environment.NewLine);
            }
            catch { /* logging must never crash the launcher */ }
            string report = null;
            try { report = CrashReport.WriteLauncherCrash(e.Exception); } catch { }
            e.Handled = true;
            // themed in-app dialog (no Windows MessageBox, no silent close); before the window exists the error is only logged
            ScriptHazemLauncher.MainWindow w = Current != null ? Current.MainWindow as ScriptHazemLauncher.MainWindow : null;
            if (w != null && w.IsLoaded)
            {
                try
                {
                    string msg = Lang.T("UnexpectedError") + e.Exception.Message;
                    if (report != null) msg += "\n\n" + Lang.T("LauncherCrashBody") + "\n" + report;
                    System.Threading.Tasks.Task<bool> t = w.ShowDialogAsync(Lang.T("ErrorTitle"), msg,
                        ScriptHazemLauncher.MainWindow.DialogKind.Error,
                        report != null ? Lang.T("OpenReport") : Lang.T("Ok"), report != null ? Lang.T("Cancel") : null);
                    string captured = report;
                    t.ContinueWith(delegate (System.Threading.Tasks.Task<bool> done)
                    {
                        if (done.Result && captured != null) CrashReport.OpenFolder(captured);
                    }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch { }
            }
        }
    }
}
