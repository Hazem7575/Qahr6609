using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ScriptHazem.ClientFiles;

namespace ScriptHazemLauncher
{
    /// <summary>What the big button does right now.</summary>
    public enum PlayState { Update, Updating, Ready, Maintenance }

    public partial class MainWindow : Window
    {
        private const double DesignW = 1060, DesignH = 625;

        private LauncherConfig _cfg;
        private Theme _theme;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();   // window lifetime
        private CancellationTokenSource _runCts;                                           // one updater run (pause = cancel)
        private bool _updating;
        private PlayState _state = PlayState.Updating;
        private readonly Random _rnd = new Random();
        private DispatcherTimer _statusTimer;
        private DispatcherTimer _themeReloadTimer;
        private FileSystemWatcher _themeWatcher;
        private NewsFeed _feed;
        private bool? _lastOnline;
        private bool _statusChecked;
        private int _currentIndex;
        private ServerListDto _servers;      // last good answer of <UpdateUrl>servers (null = not available)
        private bool _statusFromList;        // status line shows the /servers summary
        private double _speedEma;            // smoothed download speed (bytes/s) for the ETA
        private string _lastFile;
        private StatusDto _status;           // last good /status.json (maintenance banner + player count), null = none
        private EventsFeed _events;          // last good /events.json (countdown), null = none
        private DispatcherTimer _eventsTimer; // 1 s countdown ticker
        private int _pingMs = -1;            // TCP connect time to the chosen server, -1 = unknown
        private DispatcherTimer _newsTimer;  // auto-advance the news every ~7 s
        private bool _newsHover;             // pause auto-advance while the cursor is over the news card

        // background slideshow (cross-fades behind the card; zero CPU between transitions)
        private readonly List<string> _bgPaths = new List<string>();
        private DispatcherTimer _bgTimer;    // fires once per IntervalSeconds, nothing in between
        private int _bgFadeMs = 1500;
        private bool _bgRandom;
        private int _bgIndex = -1;           // index of the image currently fully shown
        private bool _bgShowingA = true;     // which Image layer is on top right now
        private BitmapImage _bgNext;         // the next image, decoded off the UI thread
        private int _bgNextIndex = -1;
        private bool _bgEnabled;
        private const int BgDecodeWidth = 1100; // decode to ~display size (window is <= 1060 wide)

        // background music (audio only, no visual element -> cheap over AllowsTransparency)
        private System.Windows.Media.MediaPlayer _music;
        private bool _musicEnabled;
        private bool _musicMuted;
        private double _musicVolume = 0.3;   // 0..1 effective volume
        private bool _musicPauseOnMin;
        private bool _musicLoading;          // guard the settings slider while loading its value

        public MainWindow()
        {
            InitializeComponent();
            _cfg = App.Config;
            _theme = App.CurrentTheme;
            FitToScreen();
            ApplyTheme();
        }

        // ============================================================ size / DPI

        /// <summary>
        /// The layout is a fixed 1060 x 625 design; on small work areas (e.g. 1366x768 at 125% = 1093 x 582 DIPs)
        /// the whole window is scaled down by the Viewbox so it always fits.
        /// </summary>
        private void FitToScreen()
        {
            Size wa = App.FitOverride ?? new Size(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
            double scale = Math.Min(1.0, Math.Min((wa.Width - 12) / DesignW, (wa.Height - 8) / DesignH));
            if (scale < 0.5) scale = 0.5;
            Width = Math.Round(DesignW * scale);
            Height = Math.Round(DesignH * scale);
        }

        // ============================================================ theme

        /// <summary>Applies every visual setting from theme.ini. Safe to call again (live reload).</summary>
        private void ApplyTheme()
        {
            Theme t = _theme;
            Title = t.Str("Texts", "WindowTitle", "Hazem");

            // frame + card art
            ImageSource frame = t.Image("Images", "Frame", "images/frame_smoke.png");
            FrameImage.Source = frame;
            FrameFallback.Visibility = frame == null ? Visibility.Visible : Visibility.Collapsed;
            if (frame == null)
            {
                BgImage.Source = t.Image("Images", "Background", "images/bg_main.jpg");
                BgImage.Opacity = t.Num("Background", "Opacity", 1.0) * 0.45;
            }
            ImageSource card = t.Image("Images", "Card", "images/card_paper.png");
            CardImage.Source = card;
            Color tint = t.Col("Card", "TintColor", "#FFEDE8E0");
            double tintOpacity = t.Num("Card", "TintOpacity", 0);
            if (card == null)
            {
                CardTint.Fill = new SolidColorBrush(tint);
                CardTint.OpacityMask = null;
                CardTint.Margin = new Thickness(0, 8, 0, 8);
                CardTint.Opacity = 1;
            }
            else
            {
                CardTint.Fill = new SolidColorBrush(tint);
                CardTint.OpacityMask = new ImageBrush(card);
                CardTint.Margin = new Thickness(0);
                CardTint.Opacity = tintOpacity;
            }
            SettingsPaper.Source = card;
            SysCheckPaper.Source = card;
            EventsPaper.Source = card;
            DialogPaper.ImageSource = card;
            ServerPaper.ImageSource = card;
            CardLayer.CacheMode = new BitmapCache { RenderAtScale = 1.25 };
            CardOrnaments.CacheMode = new BitmapCache { RenderAtScale = 1.25 };
            BandOrnaments.CacheMode = new BitmapCache { RenderAtScale = 1.25 };

            // character (Height in design pixels; Right/Bottom = distance from the window's right/bottom edge)
            ImageSource ch = t.Bool("Character", "Show", true) ? t.Image("Images", "Character", "images/char_flame.png") : null;
            CharImage.Source = ch;
            CharImage.Visibility = ch == null ? Visibility.Collapsed : Visibility.Visible;
            double chH = t.Num("Character", "Height", 560);
            double chW = ch != null && ch.Height > 0 ? chH * ch.Width / ch.Height : 0;
            CharImage.Height = chH;
            CharImage.Width = chW;
            double chLeft = DesignW - t.Num("Character", "Right", 0) - chW;
            double chTop = DesignH - t.Num("Character", "Bottom", 7) - chH;
            Canvas.SetLeft(CharImage, chLeft);
            Canvas.SetTop(CharImage, chTop);
            CharImage.Opacity = t.Num("Character", "Opacity", 1.0);
            CharFlip.ScaleX = t.Bool("Character", "Flip", false) ? -1 : 1;
            CharImage.CacheMode = new BitmapCache { RenderAtScale = 1.25 };
            // glow = static radial gradient behind the figure (no blur effect)
            double gs = t.Num("Character", "GlowSize", 40);
            CharGlowStop.Color = t.Col("Character", "GlowColor", "#FFFF7A30");
            CharGlow.Opacity = ch == null ? 0 : t.Num("Character", "GlowOpacity", 0.4);
            CharGlow.Width = chW * 0.7 + gs * 2;
            CharGlow.Height = chH * 0.6 + gs * 2;
            Canvas.SetLeft(CharGlow, chLeft + chW * 0.5 - CharGlow.Width / 2);
            Canvas.SetTop(CharGlow, chTop + chH * 0.45 - CharGlow.Height / 2);

            // logo: [Logo] Image (e.g. the official Conquer logo) with a "HAZEM SERVER" ribbon, else the vector emblem
            ImageSource logoArt = t.Image("Logo", "Image", "");
            Color gl = t.Col("Colors", "GoldLight", "#FFFFE7A8"), gm = t.Col("Colors", "Gold", "#FFE8B64E"), gd = t.Col("Colors", "GoldDark", "#FF8A5512");
            if (logoArt != null)
            {
                Emblem.BuildImage(EmblemHost, logoArt, t.Num("Logo", "Width", 250), t.Num("Logo", "OffsetY", 0),
                    t.Text("Logo", "Ribbon", "HAZEM SERVER", "HAZEM SERVER"),
                    (FontFamily)Application.Current.Resources["LogoFont"], gl, gm, gd, t.Col("Logo", "DiscColor", "#FF5A1610"));
            }
            else
            {
                Emblem.Build(EmblemHost,
                    t.Str("Texts", "LogoText", "HAZEM"),
                    t.Text("Texts", "LogoSub", "CONQUER ONLINE", "CONQUER ONLINE"),
                    t.Image("Images", "Logo", ""),
                    (FontFamily)Application.Current.Resources["LogoFont"],
                    (FontFamily)Application.Current.Resources["LogoFont"],
                    gl, gm, gd, t.Col("Logo", "DiscColor", "#FF5A1610"));
            }

            // texts
            DownloadingLabel.Text = t.Text("Texts", "Downloading", Lang.T("Downloading"), Lang.T("Downloading"));
            TotalLabel.Text = t.Text("Texts", "TotalProgress", Lang.T("TotalProgress"), Lang.T("TotalProgress"));
            FooterText.Text = t.Text("Texts", "Footer", "", "");
            FooterText.Visibility = string.IsNullOrWhiteSpace(FooterText.Text) ? Visibility.Collapsed : Visibility.Visible;
            Gallery.Visibility = t.Bool("Layout", "ShowGallery", true) ? Visibility.Visible : Visibility.Collapsed;
            _showStatus = t.Bool("Layout", "ShowStatus", true);
            UpdateStatusPlacement();
            AvatarImage.Source = t.Image("Images", "Avatar", "images/news_dragon.jpg");

            // play button font: Arabic labels use the body font, Latin labels the logo font
            Application.Current.Resources["PlayFont"] = Lang.IsArabic ? Application.Current.Resources["BodyFont"] : Application.Current.Resources["LogoFont"];
            Application.Current.Resources["PlayFontSize"] = Lang.IsArabic ? 21.0 : 17.0;

            // effects
            _emberTarget = t.Bool("Effects", "Embers", true) ? (int)Math.Min(15, t.Num("Effects", "EmberCount", 12)) : 0;
            _emberColor = t.Col("Effects", "EmberColor", "#FFFFA040");
            BuildEmberBrushes();

            ApplyLanguage();
            if (_feed == null) ShowFallbackNews();
            else ShowFeed(_feed, _currentIndex);
            SetState(_state);
            ConfigureSlideshow();
        }

        /// <summary>Built-in (non-theme) texts, tooltips and the settings panel for the current language.</summary>
        private void ApplyLanguage()
        {
            SettingsBtn.ToolTip = Lang.T("TipSettings");
            MinBtn.ToolTip = Lang.T("TipMinimize");
            CloseBtn.ToolTip = Lang.T("TipClose");
            PostedRun.Text = Lang.T("Posted");
            ByRun.Text = Lang.T("By");
            SettingsPanel.FlowDirection = Lang.Flow;
            NewsCard.FlowDirection = Lang.Flow;
            ProgressCard.FlowDirection = Lang.Flow;
            StatusCard.FlowDirection = Lang.Flow;
            ServerPanel.FlowDirection = Lang.Flow;
            SettingsTitle.Text = Lang.T("SettingsTitle");
            ScreenModeLabel.Text = Lang.T("ScreenMode");
            ModeFull.Content = Lang.T("Fullscreen");
            ModeWindow.Content = Lang.T("Windowed");
            ModeCustom.Content = Lang.T("Custom");
            FpsLabel.Text = Lang.T("MaxFps");
            DirectXLabel.Text = Lang.T("DirectX");
            ResolutionLabel.Text = Lang.T("Resolution");
            CloseOnPlayLabel.Text = Lang.T("AfterStart");
            RepairLabel.Text = Lang.T("GameFiles");
            ServerPanelTitle.Text = Lang.T("PickServerTitle");
            SettingsCloseBtn.ToolTip = Lang.T("TipClose");
            LanguageLabel.Text = Lang.T("Language");
            CloseOnPlayBox.Content = Lang.T("CloseOnPlay");
            RepairBtn.Content = Lang.T("Repair");
            RepairHint.Text = Lang.T("RepairHint");
            SysCheckLabel.Text = Lang.T("SysCheckLabel");
            SysCheckBtn.Content = Lang.T("SysCheckShow");
            PreCheckBox.Content = Lang.T("PreCheck");
            SaveBtn.Content = Lang.T("Save");
            CancelBtn.Content = Lang.T("Cancel");
            if (_statusFromList && _servers != null) ShowServers();
            else if (_statusChecked) SetStatus(_lastOnline);
            UpdateMaintenanceBanner();
            UpdatePingPlayers();
            EventsBtn.ToolTip = Lang.T("EventsTip");
            if (EventsOpen()) BuildEventsTable();
            MusicLabel.Text = Lang.T("MusicLabel");
            MusicMuteBox.Content = Lang.T("MusicMute");
            UpdateMuteButton();
        }

        private void WatchTheme()
        {
            try
            {
                if (_themeWatcher != null) _themeWatcher.Dispose();
                _themeWatcher = new FileSystemWatcher(_theme.Dir);
                _themeWatcher.IncludeSubdirectories = true;
                _themeWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
                FileSystemEventHandler h = delegate { Dispatcher.BeginInvoke(new Action(ScheduleThemeReload)); };
                _themeWatcher.Changed += h;
                _themeWatcher.Created += h;
                _themeWatcher.Renamed += delegate { Dispatcher.BeginInvoke(new Action(ScheduleThemeReload)); };
                _themeWatcher.EnableRaisingEvents = true;
            }
            catch { /* live reload is a convenience only */ }
        }

        private void ScheduleThemeReload()
        {
            if (_themeReloadTimer == null)
            {
                _themeReloadTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                _themeReloadTimer.Tick += delegate
                {
                    _themeReloadTimer.Stop();
                    try
                    {
                        _theme = App.ReloadTheme();
                        ApplyTheme();
                        StartEmbers();
                    }
                    catch { }
                };
            }
            _themeReloadTimer.Stop();
            _themeReloadTimer.Start();
        }

        // ============================================================ startup

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VersionText.Text = "v" + typeof(MainWindow).Assembly.GetName().Version.ToString(3);
            FooterVersion.Text = VersionText.Text;
            StatusText.Text = Lang.T("Checking");
            UpdateStatusPlacement();
            HideEmptyLink(FacebookButton, _cfg.Facebook);
            HideEmptyLink(DiscordButton, _cfg.Discord);
            HideEmptyLink(WebsiteButton, _cfg.Website);
            FileBar.Value = 0;
            TotalBar.Value = 0;

            Activated += delegate { ResumeEmbers(); ResumeSlideshow(); };
            Deactivated += delegate { PauseEmbers(); PauseSlideshow(); };
            StateChanged += delegate
            {
                if (WindowState == WindowState.Minimized) { PauseEmbers(); PauseSlideshow(); if (_musicPauseOnMin) MusicPause(); }
                else { if (IsActive) { ResumeEmbers(); ResumeSlideshow(); } if (_musicPauseOnMin) MusicResumeIfOn(); }
            };
            StartEmbers();
            InitMusic();
            ResumeSlideshow();
            WatchTheme();
            LoadNewsAsync();
            LoadStatusAsync();
            LoadEventsAsync();
            _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _statusTimer.Tick += async delegate { await StatusTick(); };
            _statusTimer.Start();

            if (App.DemoState != null) StartDemo(App.DemoState);
            else RunUpdateAsync(); // asks /servers first, then updates
            if (App.DemoStatus == "online") SetStatus(true);
            else if (App.DemoStatus == "offline") SetStatus(false);
            else if (App.DemoStatus == "maintenance") SetMaintenanceStatus();
            if (App.DemoOpen == "settings") Settings_Click(this, null);
            else if (App.DemoOpen == "events") Events_Click(this, null);
            else if (App.DemoOpen == "syscheck") OpenSysCheck_Click(this, null);
            else if (App.DemoOpen == "crash") { string cb = Lang.T("CrashBody") + "\n" + Lang.T("CrashExit", -1073741819); if (!string.IsNullOrEmpty(_cfg.CrashUrl())) cb += "\n" + Lang.T("CrashSent"); cb += "\n" + System.IO.Path.Combine(App.ExeDir, "reports", "crash_demo.txt"); Task<bool> ignored = ShowDialogAsync(Lang.T("CrashTitle"), cb, DialogKind.Error, Lang.T("OpenReport"), Lang.T("Cancel")); }
            else if (App.DemoOpen == "dialog") ShowDialog(Lang.T("SrvMaintenance"), Lang.T("MaintenanceAsk"), DialogKind.Question);
            else if (App.DemoOpen == "repair") { Task<bool> ignored = ShowDialogAsync(Lang.T("Repair"), Lang.T("RepairAsk"), DialogKind.Question, Lang.T("Repair"), Lang.T("Cancel")); }
            else if (App.DemoOpen == "servers" && _servers != null && _servers.servers.Count > 1) OpenServerPanel();
            else if (App.DemoOpen == "play") { DispatcherTimer tp = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) }; tp.Tick += delegate { tp.Stop(); Play_Click(this, null); }; tp.Start(); }

            if (!string.IsNullOrEmpty(App.SnapshotPath))
            {
                DispatcherTimer snap = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(2500) };
                snap.Tick += delegate { snap.Stop(); SaveSnapshot(App.SnapshotPath); Close(); };
                snap.Start();
            }
        }

        /// <summary>
        /// The window is created fully transparent (Opacity 0) at its final size and position, and only fades in
        /// after WPF has rendered the first complete frame - so nothing half-drawn or wrongly sized is ever shown.
        /// </summary>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DoubleAnimation fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220));
            fade.FillBehavior = FillBehavior.Stop;
            fade.Completed += delegate { Opacity = 1; };
            BeginAnimation(OpacityProperty, fade);
            if (App.StartupLog)
            {
                try
                {
                    double ms = (DateTime.Now - Process.GetCurrentProcess().StartTime).TotalMilliseconds;
                    File.AppendAllText(System.IO.Path.Combine(App.ExeDir, "startup_time.log"),
                        DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + " first frame after " + ms.ToString("0", CultureInfo.InvariantCulture) + " ms" + Environment.NewLine);
                    DispatcherTimer chk = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                    chk.Tick += delegate
                    {
                        chk.Stop();
                        try { File.AppendAllText(System.IO.Path.Combine(App.ExeDir, "startup_time.log"), "  window opacity after 2 s = " + Opacity.ToString(CultureInfo.InvariantCulture) + ", visible=" + IsVisible + ", state=" + WindowState + Environment.NewLine); } catch { }
                    };
                    chk.Start();
                }
                catch { }
            }
        }

        private static void HideEmptyLink(Button b, string url)
        {
            b.Visibility = string.IsNullOrWhiteSpace(url) ? Visibility.Collapsed : Visibility.Visible;
        }

        // ============================================================ play button states

        private void SetState(PlayState s)
        {
            _state = s;
            ResourceDictionary r = Application.Current.Resources;
            switch (s)
            {
                case PlayState.Ready:
                    PlayButton.Content = _theme.Text("Texts", "Play", Lang.T("PlayReady"), Lang.T("PlayReady"));
                    PlayButton.Tag = "";
                    PlayButton.Uid = "";
                    r["PlayFace"] = Face("#FFFFE9AE", "#FFEDBC5C", "#FFC4842C", "#FFE9AE4E", "#FFFFD98A");
                    r["PlayEdge"] = Solid("#FF8A5512");
                    r["PlayInk"] = Solid("#FF3E1C06");
                    break;
                case PlayState.Update:
                    PlayButton.Content = Lang.T("PlayUpdate");
                    PlayButton.Tag = Lang.T("PlayUpdateSub");
                    PlayButton.Uid = ""; // download
                    r["PlayFace"] = Face("#FFFFE9AE", "#FFEDBC5C", "#FFC4842C", "#FFE9AE4E", "#FFFFD98A");
                    r["PlayEdge"] = Solid("#FF8A5512");
                    r["PlayInk"] = Solid("#FF3E1C06");
                    break;
                case PlayState.Updating:
                    PlayButton.Content = Lang.T("PlayPause");
                    PlayButton.Tag = TotalBar.Value > 0 ? TotalBar.Value.ToString("0", CultureInfo.InvariantCulture) + "%" : "";
                    PlayButton.Uid = ""; // pause
                    r["PlayFace"] = Face("#FFE2C58E", "#FFC2914A", "#FF8C5A22", "#FFB27D3C", "#FFD8B072");
                    r["PlayEdge"] = Solid("#FF5E3810");
                    r["PlayInk"] = Solid("#FF2A1204");
                    break;
                case PlayState.Maintenance:
                    PlayButton.Content = Lang.T("PlayMaintenance");
                    PlayButton.Tag = Lang.T("PlayMaintenanceSub");
                    PlayButton.Uid = ""; // repair
                    r["PlayFace"] = Face("#FFE9E6E0", "#FFC2BCB2", "#FF8A8378", "#FFADA699", "#FFD6D1C8");
                    r["PlayEdge"] = Solid("#FF5A544A");
                    r["PlayInk"] = Solid("#FF2C2824");
                    break;
            }
            // While a launch is in progress the button stays locked so the game can't be started twice;
            // a state refresh (status poll, etc.) must not quietly re-enable it.
            PlayButton.IsEnabled = !_launching;
        }

        private static LinearGradientBrush Face(string a, string b, string c, string d, string e)
        {
            LinearGradientBrush g = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            g.GradientStops.Add(new GradientStop(Theme.ParseColor(a, Colors.Gold), 0));
            g.GradientStops.Add(new GradientStop(Theme.ParseColor(b, Colors.Gold), 0.45));
            g.GradientStops.Add(new GradientStop(Theme.ParseColor(c, Colors.Gold), 0.62));
            g.GradientStops.Add(new GradientStop(Theme.ParseColor(d, Colors.Gold), 0.9));
            g.GradientStops.Add(new GradientStop(Theme.ParseColor(e, Colors.Gold), 1));
            g.Freeze();
            return g;
        }

        private static SolidColorBrush Solid(string c)
        {
            SolidColorBrush b = new SolidColorBrush(Theme.ParseColor(c, Colors.Black));
            b.Freeze();
            return b;
        }

        /// <summary>After an update (or a /servers refresh): Ready, or Maintenance when the server says so.</summary>
        private void RefreshReadyState()
        {
            if (_state == PlayState.Update || _state == PlayState.Updating) return;
            SetState(MaintenanceActive() ? PlayState.Maintenance : PlayState.Ready);
            UpdateMaintenanceBanner();
        }

        /// <summary>Maintenance when /status.json says so globally, or the chosen server is flagged in /servers.</summary>
        private bool MaintenanceActive()
        {
            if (_status != null && _status.maintenance) return true;
            GameServerDto sel = SelectedServer();
            return sel != null && sel.maintenance;
        }

        // ============================================================ update

        private async void RunUpdateAsync()
        {
            if (_updating) return;
            _updating = true;
            _speedEma = 0;
            SetState(PlayState.Updating);
            StateText.Foreground = (Brush)FindResource("GoldText");
            _runCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            Progress<UpdateProgress> progress = new Progress<UpdateProgress>(ShowProgress);
            Updater updater = new Updater(_cfg.UpdateUrls, _cfg.BaseDir, _cfg.DownloadThreads, _cfg.DownloadKBps);
            await CheckServerAsync();
            string listPath = ClientListManifestPath();
            if (_servers != null && listPath != null)
            {
                // the launcher writes this file from /servers, so the patch copy must not overwrite it
                updater.SkipPaths.Add(listPath);
            }
            UpdateResult result = null;
            string error = null;
            try
            {
                CancellationToken token = _runCts.Token;
                result = await Task.Run(() => updater.Run(progress, token));
            }
            catch (OperationCanceledException)
            {
                _updating = false;
                if (_cts.IsCancellationRequested) return; // window closing
                await Task.Delay(150);
                StateText.Text = Lang.T("Paused");
                FileInfoText.Text = "";
                SetState(PlayState.Update);
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            _updating = false;

            // let queued progress reports land first, then show the final state
            await Task.Delay(150);
            if (error != null)
            {
                StateText.Text = Lang.T("UpdateError") + error;
                StateText.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x7A, 0x60));
            }
            else if (result != null && result.Error != null)
            {
                StateText.Text = result.Error + " - " + Lang.T("PlayAnyway");
                FileName.Text = "-";
                FileSizeRun.Text = "";
                FileInfoText.Text = "";
                FileBar.Value = 0;
                TotalBar.Value = 100;
                TotalInfo.Text = ""; TotalEta.Text = "";
            }
            else
            {
                FileName.Text = "-";
                FileSizeRun.Text = "";
                FileInfoText.Text = "";
                FileBar.Value = 100;
                TotalBar.Value = 100;
                TotalInfo.Text = "100%"; TotalEta.Text = "";
            }

            WriteClientServerList(false);

            if (result != null && result.LauncherReplaced)
            {
                await ShowDialogAsync(Lang.T("NoticeTitle"), Lang.T("LauncherUpdated"), DialogKind.Info);
                try { Process.Start(Environment.ProcessPath); } catch { }
                Close();
                return;
            }
            _state = PlayState.Ready;
            RefreshReadyState();
            TrimMemorySoon();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, IntPtr min, IntPtr max);

        /// <summary>
        /// The launcher mostly sits idle after the update: free the start-up garbage and give unused pages
        /// back to Windows once (they come back on demand).
        /// </summary>
        private void TrimMemorySoon()
        {
            DispatcherTimer t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            t.Tick += delegate
            {
                t.Stop();
                try
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (IntPtr)(-1), (IntPtr)(-1));
                }
                catch { }
            };
            t.Start();
        }

        private void ShowProgress(UpdateProgress p)
        {
            if (!_updating) return;
            if (p.State != null) StateText.Text = p.State;
            FileName.Text = string.IsNullOrEmpty(p.FileName) ? "-" : ShortPath(p.FileName);
            if (p.FileTotal > 1)
            {
                double fp = 100.0 * p.FileDone / p.FileTotal;
                FileBar.Value = fp;
                FileSizeRun.Text = "[" + Size(p.FileTotal) + "]";
                if (p.FileName != _lastFile) { _lastFile = p.FileName; }
                if (p.BytesPerSecond > 0) _speedEma = _speedEma <= 0 ? p.BytesPerSecond : _speedEma * 0.8 + p.BytesPerSecond * 0.2;
                FileInfoText.Text = fp.ToString("0.0", CultureInfo.InvariantCulture) + "% [" + Size(_speedEma) + "/s]";
            }
            else
            {
                FileSizeRun.Text = "";
                FileInfoText.Text = "";
            }
            if (p.TotalAll > 1)
            {
                double tp = 100.0 * p.TotalDone / p.TotalAll;
                TotalBar.Value = tp;
                TotalInfo.Text = tp.ToString("0.0", CultureInfo.InvariantCulture) + "% [" + Size(p.TotalDone) + " " + Lang.T("Of") + " " + Size(p.TotalAll) + "]";
                TotalEta.Text = _speedEma > 1 && p.TotalDone < p.TotalAll
                    ? (Lang.IsArabic ? Lang.T("Left") + " " + Eta((p.TotalAll - p.TotalDone) / _speedEma) : Eta((p.TotalAll - p.TotalDone) / _speedEma) + " " + Lang.T("Left"))
                    : "";
                if (_state == PlayState.Updating) PlayButton.Tag = tp.ToString("0", CultureInfo.InvariantCulture) + "%";
            }
        }

        private static string ShortPath(string p)
        {
            p = p.Replace('/', '\\');
            return p.Length <= 34 ? p : p.Substring(0, 14) + "..." + p.Substring(p.Length - 17);
        }

        private static string Eta(double seconds)
        {
            if (seconds < 0 || double.IsInfinity(seconds) || double.IsNaN(seconds)) return "--:--";
            TimeSpan t = TimeSpan.FromSeconds(Math.Min(seconds, 359999));
            return t.TotalHours >= 1 ? ((int)t.TotalHours).ToString(CultureInfo.InvariantCulture) + ":" + t.ToString("mm\\:ss", CultureInfo.InvariantCulture)
                                     : t.ToString("mm\\:ss", CultureInfo.InvariantCulture);
        }

        private static string Size(double bytes)
        {
            CultureInfo c = CultureInfo.InvariantCulture;
            if (bytes >= 1024.0 * 1024 * 1024) return (bytes / (1024.0 * 1024 * 1024)).ToString("0.00", c) + " GB";
            if (bytes >= 1024.0 * 1024) return (bytes / (1024.0 * 1024)).ToString("0.00", c) + " MB";
            if (bytes >= 1024.0) return (bytes / 1024.0).ToString("0.0", c) + " KB";
            return bytes.ToString("0", c) + " B";
        }

        // ============================================================ demo (hidden debug switch)

        private void StartDemo(string state)
        {
            if (App.DemoServers)
            {
                _servers = new ServerListDto
                {
                    login = new LoginDto { host = "127.0.0.1", port = 9961, online = true },
                    servers = new List<GameServerDto>
                    {
                        new GameServerDto { name = "Hazem", online = true, players = 142 },
                        new GameServerDto { name = "Hazem-Test", online = true, maintenance = state == "maintenance", players = 3 }
                    }
                };
                if (state == "maintenance") _cfg.SelectedServer = "Hazem-Test";
                ShowServers();
            }
            else
            {
                SetStatus(true);
            }
            _feed = new NewsFeed
            {
                items = new List<NewsItem>
                {
                    new NewsItem { tag = "تحديث", tag_en = "UPDATE", title = "تحديث الخريف: حرب النقابات رجعت بنظام جديد", title_en = "Autumn update: Guild War returns with new rules",
                        date = "2026-10-12 12:00", author = "Hazem",
                        body = "حرب النقابات بقت كل **سبت الساعة 8 بالليل** مع جوايز جديدة لأول 3 نقابات، والبوابات الأربعة بقت تتكسر بالترتيب. [اقرا التفاصيل](https://example.com/news) وجهّز نقابتك من دلوقتي، وفيه خصم 20% في متجر الـCPs طول الأسبوع.",
                        body_en = "Guild War is now every **Saturday at 20:00** with new rewards for the top 3 guilds, and the four gates must be broken in order. [Read the details](https://example.com/news) and get your guild ready - the CP shop also has 20% off all week." },
                    new NewsItem { tag = "إيفنت", tag_en = "EVENT", title = "إيفنت الويك إند", title_en = "Weekend event", date = "2026-10-10", author = "Hazem", body = "خبرة مضاعفة.", body_en = "Double experience." }
                }
            };
            ShowFeed(_feed, 0);

            // demo status (maintenance banner + player count + ping) and a demo countdown
            _status = new StatusDto
            {
                maintenance = state == "maintenance",
                until = DateTime.UtcNow.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                message_ar = "السيرفر تحت الصيانة لتركيب تحديث الخريف. هنرجع تاني خلال ساعتين، شكراً لصبركم.",
                message_en = "The server is under maintenance for the autumn patch. We will be back within two hours - thanks for your patience.",
                servers = new List<StatusServer> { new StatusServer { name = "Hazem", online = true, players = 142 } }
            };
            _events = new EventsFeed
            {
                weekly = new List<WeeklyEvent>
                {
                    new WeeklyEvent { name_ar = "معركة المملكة", name_en = "Kingdom War", day = "daily", time = "20:30" },
                    new WeeklyEvent { name_ar = "حرب النقابات", name_en = "Guild War", day = "Sat", time = "20:00" },
                    new WeeklyEvent { name_ar = "مدينة الشياطين", name_en = "Demon City", day = "Mon,Wed,Fri", time = "19:00" },
                    new WeeklyEvent { name_ar = "سباق الخيول", name_en = "Horse Racing", day = "daily", time = "15:00" }
                },
                events = new List<EventTime>
                {
                    new EventTime { name_ar = "افتتاح موسم الهلال", name_en = "Crescent Season opening", time = DateTime.Now.AddDays(3).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) }
                }
            };
            _pingMs = 23;
            UpdateMaintenanceBanner();
            UpdatePingPlayers();

            _updating = state == "updating";
            ShowProgressDemo(state);
            switch (state)
            {
                case "update": TrimMemorySoon(); SetState(PlayState.Update); StateText.Text = Lang.T("Paused"); break;
                case "updating": SetState(PlayState.Updating); StateText.Text = Lang.T("DownloadingUpdates"); break;
                case "maintenance": SetState(PlayState.Maintenance); StateText.Text = Lang.T("UpToDate"); break;
                default: SetState(PlayState.Ready); StateText.Text = Lang.T("UpToDate"); TrimMemorySoon(); break;
            }
        }

        private void ShowProgressDemo(string state)
        {
            const long file = 76786483, total = 1649756979;
            if (state == "updating" || state == "update")
            {
                _updating = true;
                ShowProgress(new UpdateProgress { FileName = "Env_DX9/data/c3/Libs/GrnEffect.wdf", FileDone = (long)(file * 0.721), FileTotal = file,
                    TotalDone = (long)(total * 0.821), TotalAll = total, BytesPerSecond = 3.71 * 1024 * 1024 });
                _updating = state == "updating";
            }
            else
            {
                FileName.Text = "-";
                FileBar.Value = 100;
                TotalBar.Value = 100;
                TotalInfo.Text = "100%"; TotalEta.Text = "";
            }
        }

        private void SaveSnapshot(string path)
        {
            try
            {
                const double s = 1.5;
                Root.UpdateLayout();
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)(DesignW * s), (int)(DesignH * s), 96 * s, 96 * s, PixelFormats.Pbgra32);
                rtb.Render(Root);
                PngBitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(rtb));
                using (FileStream fs = File.Create(path)) enc.Save(fs);
            }
            catch (Exception ex)
            {
                try { File.WriteAllText(path + ".error.txt", ex.ToString()); } catch { }
            }
        }

        // ============================================================ news

        /// <summary>
        /// News: 1) fetch GET /news.json from the PatchServer and cache the last good copy to disk;
        /// 2) when offline, show the cached copy; 3) when neither is available, keep the theme's built-in news.
        /// </summary>
        private async void LoadNewsAsync()
        {
            NewsFeed feed = null;
            if (!string.IsNullOrEmpty(_cfg.UpdateUrl))
            {
                try
                {
                    string json = await Task.Run(() => Updater.DownloadString(_cfg.UpdateUrl + "news.json"));
                    feed = JsonSerializer.Deserialize<NewsFeed>(json, Updater.Json);
                    if (feed != null && feed.items != null && feed.items.Count > 0)
                        FeedCache.Save("news.json", json); // only cache a good feed
                }
                catch { feed = null; }
            }
            if (feed == null || feed.items == null || feed.items.Count == 0)
            {
                string cached = FeedCache.Load("news.json");
                if (cached != null) { try { feed = JsonSerializer.Deserialize<NewsFeed>(cached, Updater.Json); } catch { feed = null; } }
            }
            if (feed == null || feed.items == null || feed.items.Count == 0) return; // keep the theme fallback
            _feed = feed;
            ShowFeed(feed, 0);
        }

        /// <summary>Loads a news image from a URL (relative to the patch base, or a full http(s) URL). Null = skip.</summary>
        private ImageSource LoadNewsImage(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            try
            {
                Uri uri = url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(url)
                    : new Uri(_cfg.UpdateUrl + url.TrimStart('/'));
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = uri;
                bmp.CacheOption = BitmapCacheOption.OnLoad; // decode now; the dark thumbnail border is the placeholder
                bmp.DecodePixelWidth = 300;
                bmp.EndInit();
                return bmp;
            }
            catch { return null; } // bad url or no base: the thumbnail stays on its placeholder background
        }

        private void ShowFeed(NewsFeed feed, int index)
        {
            if (index < 0 || index >= feed.items.Count) index = 0;
            _currentIndex = index;
            ShowItem(feed.items[index]);

            // pager (round prev/next + dots) - only when there is more than one post
            bool many = feed.items.Count > 1;
            NewsPager.Visibility = many ? Visibility.Visible : Visibility.Collapsed;
            if (many)
            {
                Brush on = (Brush)FindResource("CardTag");
                Brush off = new SolidColorBrush(Color.FromArgb(0x80, 0xA0, 0x96, 0x8C));
                List<NewsDot> dots = new List<NewsDot>();
                for (int i = 0; i < feed.items.Count; i++)
                    dots.Add(new NewsDot { Index = i, Size = i == index ? 9 : 7, Fill = i == index ? on : off });
                NewsDots.ItemsSource = dots;
            }
            StartNewsAutoAdvance();

            List<GalleryItem> gallery = new List<GalleryItem>();
            // PatchServer contract: the 3 thumbnails are the per-item images of the most recent posts (click = jump)
            for (int i = 0; i < feed.items.Count; i++)
            {
                if (gallery.Count == 3) break;
                ImageSource src = LoadNewsImage(feed.items[i].image);
                if (src != null) gallery.Add(new GalleryItem { Image = src, Index = i, Caption = feed.items[i].Title });
            }
            // old shape: an explicit top-level "images" list (no per-post index to jump to)
            if (gallery.Count == 0 && feed.images != null)
            {
                foreach (NewsImage img in feed.images)
                {
                    if (gallery.Count == 3) break;
                    ImageSource src = LoadNewsImage(img.url);
                    if (src != null) gallery.Add(new GalleryItem { Image = src, Link = img.link, Caption = img.Caption });
                }
            }
            if (gallery.Count == 0) gallery = ThemeGallery();
            Gallery.ItemsSource = gallery;
        }

        /// <summary>Jumps the news by delta posts (wraps). Used by the pager buttons and auto-advance.</summary>
        private void ShowRelative(int delta)
        {
            if (_feed == null || _feed.items == null || _feed.items.Count < 2) return;
            int n = _feed.items.Count;
            ShowFeed(_feed, ((_currentIndex + delta) % n + n) % n);
        }

        /// <summary>News cycles on its own every ~7 s so it is obvious there is more than one post; paused on hover.</summary>
        private void StartNewsAutoAdvance()
        {
            if (_newsTimer == null)
            {
                _newsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(7) };
                _newsTimer.Tick += delegate { if (!_newsHover) ShowRelative(1); };
            }
            bool many = _feed != null && _feed.items != null && _feed.items.Count > 1;
            if (many) { _newsTimer.Stop(); _newsTimer.Start(); } else _newsTimer.Stop();
        }

        private NewsItem _shownItem;

        private void ShowItem(NewsItem it)
        {
            _shownItem = it;
            NewsTag.Text = "[" + (it.Tag ?? "NEWS").ToUpperInvariant() + "] ";
            NewsTitle.Text = it.Title ?? "";
            NewsHeader.FlowDirection = FlowFor(it.Title);
            NewsDate.Text = it.date ?? "";
            NewsAuthor.Text = string.IsNullOrEmpty(it.author) ? _theme.Str("Texts", "LogoText", "Hazem") : it.author;
            SetRichText(NewsBody, it.Body ?? "");
            NewsBody.FlowDirection = FlowFor(it.Body);
        }

        private static readonly Regex Markup = new Regex(@"\*\*(.+?)\*\*|\[([^\]]+)\]\((https?://[^\s\)]+)\)", RegexOptions.Compiled);

        /// <summary>News body with **bold** and [text](https://link) support; \n = new line.</summary>
        private void SetRichText(TextBlock tb, string text)
        {
            tb.Inlines.Clear();
            text = text.Replace("\\n", "\n");
            int pos = 0;
            foreach (Match m in Markup.Matches(text))
            {
                if (m.Index > pos) tb.Inlines.Add(new Run(text.Substring(pos, m.Index - pos)));
                if (m.Groups[1].Success)
                {
                    tb.Inlines.Add(new Run(m.Groups[1].Value) { FontWeight = FontWeights.Bold, Foreground = (Brush)FindResource("CardTitle") });
                }
                else
                {
                    Hyperlink h = new Hyperlink(new Run(m.Groups[2].Value)) { Foreground = (Brush)FindResource("CardLink"), Cursor = Cursors.Hand, Tag = m.Groups[3].Value };
                    h.Click += delegate { OpenUrl(h.Tag as string); };
                    tb.Inlines.Add(h);
                }
                pos = m.Index + m.Length;
            }
            if (pos < text.Length) tb.Inlines.Add(new Run(text.Substring(pos)));
        }

        /// <summary>Arabic text is always right-to-left; Latin-only text follows the UI language.</summary>
        private static FlowDirection FlowFor(string text)
        {
            return TextDir.IsArabic(text) ? FlowDirection.RightToLeft : Lang.Flow;
        }

        /// <summary>News shown when the patch server is offline (from theme.ini [OfflineNews]).</summary>
        private void ShowFallbackNews()
        {
            ShowItem(new NewsItem
            {
                tag = _theme.Str("OfflineNews", "Tag", "INFO"),
                title = _theme.Text("OfflineNews", "Title", "أهلاً بيك في حازم", "Welcome to Hazem"),
                date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                author = _theme.Str("Texts", "LogoText", "Hazem"),
                body = _theme.Text("OfflineNews", "Body", "جاري تحميل الأخبار...", "Loading news...")
            });
            NewsPager.Visibility = Visibility.Collapsed;
            Gallery.ItemsSource = ThemeGallery();
        }

        private List<GalleryItem> ThemeGallery()
        {
            List<GalleryItem> list = new List<GalleryItem>();
            for (int i = 1; i <= 3; i++)
            {
                string p = _theme.FilePath("Gallery", "Image" + i, "");
                list.Add(new GalleryItem
                {
                    Image = p == null ? null : Theme.LoadImage(p, 300),
                    Caption = _theme.Text("Gallery", "Caption" + i, "", ""),
                    Link = _theme.Str("Gallery", "Link" + i, "")
                });
            }
            return list;
        }

        /// <summary>Thumbnail click: jump to that news post (old-shape galleries open the image link instead).</summary>
        private void GalleryItem_Click(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                int idx;
                if (fe.Tag is int) idx = (int)fe.Tag;
                else int.TryParse(fe.Tag as string, out idx);
                if (_feed != null && idx >= 0 && idx < _feed.items.Count) ShowFeed(_feed, idx);
                else if (fe.Tag is string s) OpenUrl(s); // old-shape gallery: Tag was a link
            }
            e.Handled = true;
        }

        private void NewsTitle_Click(object sender, MouseButtonEventArgs e)
        {
            if (_shownItem != null && !string.IsNullOrEmpty(_shownItem.link)) OpenUrl(_shownItem.link);
            e.Handled = true;
        }

        private void NewsPrev_Click(object sender, RoutedEventArgs e) { ShowRelative(-1); }

        private void NewsNext_Click(object sender, RoutedEventArgs e) { ShowRelative(1); }

        private void NewsDot_Click(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null && fe.Tag is int && _feed != null) ShowFeed(_feed, (int)fe.Tag);
            e.Handled = true;
        }

        private void NewsCard_MouseEnter(object sender, MouseEventArgs e) { _newsHover = true; }

        private void NewsCard_MouseLeave(object sender, MouseEventArgs e) { _newsHover = false; }

        // ============================================================ server status

        /// <summary>
        /// Asks the patch server for the published game servers (GET /servers). When it does not answer,
        /// falls back to the old check of launcher.ini ServerIP:LoginPort.
        /// </summary>
        private async Task CheckServerAsync()
        {
            if (App.DemoState != null) return;
            if (!string.IsNullOrEmpty(_cfg.UpdateUrl))
            {
                ServerListDto list = null;
                try
                {
                    string baseUrl = _cfg.UpdateUrl;
                    list = await Task.Run(() => ServerListClient.Fetch(baseUrl));
                }
                catch
                {
                    list = null; // old patch server without /servers, or offline
                }
                if (list != null && list.servers.Count > 0)
                {
                    _servers = list;
                    ShowServers();
                    return;
                }
            }

            _statusFromList = false;
            ServerArrow.Visibility = Visibility.Collapsed;
            VersionText.Visibility = Visibility.Visible;
            ServerPanel.Visibility = Visibility.Collapsed;
            if (string.IsNullOrEmpty(_cfg.ServerIP))
            {
                SetStatus(null);
                return;
            }
            // fallback: launcher.ini ServerIP + LoginPort, and GamePort when it is set (both must answer)
            Task<bool> login = ProbeAsync(_cfg.ServerIP, _cfg.LoginPort);
            Task<bool> game = _cfg.GamePort > 0 ? ProbeAsync(_cfg.ServerIP, _cfg.GamePort) : Task.FromResult(true);
            bool loginUp = await login, gameUp = await game;
            if (loginUp && gameUp) SetStatus(true);
            else if (loginUp) SetStatusText(false, Lang.T("GameOffline"));
            else SetStatus(false);
            StatusPill.ToolTip = _cfg.ServerIP + "  login " + _cfg.LoginPort + (loginUp ? " OK" : " --")
                + (_cfg.GamePort > 0 ? "   game " + _cfg.GamePort + (gameUp ? " OK" : " --") : "");
        }

        private static async Task<bool> ProbeAsync(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(host) || port <= 0) return false;
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Task connect = client.ConnectAsync(host, port);
                    Task done = await Task.WhenAny(connect, Task.Delay(2500));
                    return done == connect && !connect.IsFaulted && client.Connected;
                }
            }
            catch { return false; }
        }

        // ============================================================ status.json / events.json / ping (my feature)

        /// <summary>The periodic (30 s) refresh: servers + maintenance banner + events + ping.</summary>
        private async Task StatusTick()
        {
            await CheckServerAsync();
            if (App.DemoState == null) { LoadStatusAsync(); LoadEventsAsync(); }
            await MeasurePingAsync();
            if (!_updating) RefreshReadyState();
        }

        /// <summary>GET /status.json: the maintenance banner + player count. Offline keeps the last good copy.</summary>
        private async void LoadStatusAsync()
        {
            if (App.DemoState != null) return;
            if (string.IsNullOrEmpty(_cfg.UpdateUrl)) return;
            StatusDto st = null;
            try
            {
                string json = await Task.Run(() => Updater.DownloadString(_cfg.UpdateUrl + "status.json"));
                st = JsonSerializer.Deserialize<StatusDto>(json, Updater.Json);
            }
            catch { st = null; } // old patch server without /status.json, or offline: no banner
            if (st == null) return;
            _status = st;
            if (!_updating) RefreshReadyState();
            UpdateMaintenanceBanner();
            UpdatePingPlayers();
        }

        /// <summary>GET /events.json: the upcoming events for the countdown. Hidden when none are configured.</summary>
        private async void LoadEventsAsync()
        {
            if (App.DemoState != null) return;
            if (string.IsNullOrEmpty(_cfg.UpdateUrl)) return;
            EventsFeed ev = null;
            try
            {
                string json = await Task.Run(() => Updater.DownloadString(_cfg.UpdateUrl + "events.json"));
                ev = JsonSerializer.Deserialize<EventsFeed>(json, Updater.Json);
            }
            catch { ev = null; }
            if (ev == null) return;
            _events = ev;
            if (EventsOpen()) BuildEventsTable();
        }

        private List<EventScheduleRow> _eventRows;

        /// <summary>Calendar button: opens the events schedule popup.</summary>
        private void Events_Click(object sender, RoutedEventArgs e)
        {
            BuildEventsTable();
            OpenPanel(EventsOverlay, EventsScale);
            if (_eventsTimer == null)
            {
                _eventsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _eventsTimer.Tick += delegate { TickCountdown(); };
            }
            _eventsTimer.Start(); // the 1 s ticker only runs while the panel is open
        }

        private void EventsClose_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel(EventsOverlay, EventsScale);
            if (_eventsTimer != null) _eventsTimer.Stop(); // cheap when closed
        }

        private bool EventsOpen() { return EventsOverlay != null && EventsOverlay.Visibility == Visibility.Visible; }

        /// <summary>Fills the schedule table from events.json (weekly + upcoming one-off), soonest first.</summary>
        private void BuildEventsTable()
        {
            EventsContent.FlowDirection = Lang.Flow;
            EventsTitleText.Text = Lang.T("EventsScheduleTitle");
            ColEventHdr.Text = Lang.T("ColEvent");
            ColDayHdr.Text = Lang.T("ColDay");
            ColTimeHdr.Text = Lang.T("ColTime");
            ColCountHdr.Text = Lang.T("ColCountdown");
            EventsEmptyText.Text = Lang.T("EventsEmpty");
            EventsCloseBtn2.Content = Lang.T("Ok");
            _eventRows = BuildEventRows();
            EventsTable.ItemsSource = _eventRows;
            EventsEmptyText.Visibility = _eventRows.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            DateTime now = DateTime.Now;
            foreach (EventScheduleRow r in _eventRows) r.Tick(now);
        }

        private List<EventScheduleRow> BuildEventRows()
        {
            List<EventScheduleRow> rows = new List<EventScheduleRow>();
            if (_events != null)
            {
                DateTime now = DateTime.Now;
                CultureInfo c = new CultureInfo(Lang.IsArabic ? "ar" : "en-US");
                if (_events.weekly != null)
                {
                    foreach (WeeklyEvent w in _events.weekly)
                    {
                        DateTime? next = w.NextLocal(now);
                        if (next == null) continue;
                        rows.Add(new EventScheduleRow { Name = w.Name, Day = FormatWeeklyDay(w, c), Time = (w.time ?? "").Trim(), When = next.Value, NameFlow = FlowFor(w.Name) });
                    }
                }
                if (_events.events != null)
                {
                    foreach (EventTime ev in _events.events)
                    {
                        DateTime? at = ev.AtLocal();
                        if (at == null || at.Value <= now) continue;
                        rows.Add(new EventScheduleRow { Name = ev.Name, Day = at.Value.ToString("ddd d MMM", c), Time = at.Value.ToString("HH:mm", CultureInfo.InvariantCulture), When = at.Value, NameFlow = FlowFor(ev.Name) });
                    }
                }
            }
            rows.Sort(delegate (EventScheduleRow a, EventScheduleRow b) { return a.When.CompareTo(b.When); });
            return rows;
        }

        private static string FormatWeeklyDay(WeeklyEvent w, CultureInfo c)
        {
            int[] ds = w.DayIndices();
            if (ds.Length == 0) return "";
            foreach (int d in ds) if (d == -1) return Lang.T("EventDaily");
            List<string> parts = new List<string>();
            // one day -> full name; several -> abbreviated so the column does not overflow
            foreach (int d in ds)
                parts.Add(ds.Length == 1 ? c.DateTimeFormat.GetDayName((DayOfWeek)d) : c.DateTimeFormat.GetAbbreviatedDayName((DayOfWeek)d));
            return string.Join(Lang.IsArabic ? "، " : ", ", parts.ToArray());
        }

        /// <summary>Updates the live countdowns while the panel is open; rebuilds when an event has passed.</summary>
        private void TickCountdown()
        {
            if (!EventsOpen()) { if (_eventsTimer != null) _eventsTimer.Stop(); return; }
            if (_eventRows == null) return;
            DateTime now = DateTime.Now;
            bool stale = false;
            foreach (EventScheduleRow r in _eventRows) if (r.When <= now) { stale = true; break; }
            if (stale)
            {
                _eventRows = BuildEventRows();
                EventsTable.ItemsSource = _eventRows;
                EventsEmptyText.Visibility = _eventRows.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            foreach (EventScheduleRow r in _eventRows) r.Tick(now);
        }

        /// <summary>Shows/hides the amber maintenance banner with the message + optional "until" time.</summary>
        private void UpdateMaintenanceBanner()
        {
            if (MaintBanner == null) return;
            if (!MaintenanceActive()) { MaintBanner.Visibility = Visibility.Collapsed; return; }

            string msg = _status != null ? _status.Message : null;
            GameServerDto sel = SelectedServer();
            if (string.IsNullOrWhiteSpace(msg) && sel != null && !string.IsNullOrWhiteSpace(sel.Notice)) msg = sel.Notice;

            MaintPanel.FlowDirection = Lang.Flow;
            MaintTitle.Text = Lang.T("MaintBannerTitle");
            MaintTitle.FlowDirection = Lang.Flow;
            MaintMessage.Text = string.IsNullOrWhiteSpace(msg) ? Lang.T("MaintBannerDefault") : msg;
            MaintMessage.FlowDirection = FlowFor(MaintMessage.Text);

            string until = _status != null ? _status.until : null;
            DateTime u;
            if (!string.IsNullOrWhiteSpace(until)
                && DateTime.TryParse(until, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out u))
            {
                CultureInfo c = new CultureInfo(Lang.IsArabic ? "ar" : "en-US");
                MaintUntil.Text = Lang.T("MaintUntil") + " " + u.ToLocalTime().ToString("dddd HH:mm", c);
                MaintUntil.FlowDirection = Lang.Flow;
                MaintUntil.Visibility = Visibility.Visible;
            }
            else MaintUntil.Visibility = Visibility.Collapsed;

            MaintBanner.Visibility = Visibility.Visible;
        }

        /// <summary>Shows the player count (when the owner supplies one) and the ping next to the status pill.</summary>
        private void UpdatePingPlayers()
        {
            if (FooterMeta == null) return;
            List<string> parts = new List<string>();

            int players = -1;
            GameServerDto sel = SelectedServer();
            string name = sel != null ? sel.name : null;
            if (_status != null)
            {
                if (name != null) players = _status.PlayersOf(name);
                if (players < 0 && _status.servers != null && _status.servers.Count == 1) players = _status.servers[0].players;
            }
            if (players < 0 && sel != null && sel.players >= 0) players = sel.players;

            if (players >= 0) parts.Add(players + " " + Lang.T("PlayersWord"));
            if (_pingMs >= 0) parts.Add(_pingMs + " " + Lang.T("PingMs"));
            FooterMeta.Text = parts.Count == 0 ? "" : "·  " + string.Join("  ·  ", parts.ToArray());
        }

        /// <summary>Measures the TCP connect time (ms) to the chosen server; -1 when it cannot connect.</summary>
        private async Task MeasurePingAsync()
        {
            if (App.DemoState != null) return;
            string host = null;
            int port = 0;
            GameServerDto sel = SelectedServer();
            if (sel != null && !string.IsNullOrWhiteSpace(sel.host) && sel.port > 0) { host = sel.host; port = sel.port; }
            else if (!string.IsNullOrEmpty(_cfg.ServerIP)) { host = _cfg.ServerIP; port = _cfg.GamePort > 0 ? _cfg.GamePort : _cfg.LoginPort; }
            _pingMs = host == null ? -1 : await ProbePingAsync(host, port);
            UpdatePingPlayers();
        }

        private static async Task<int> ProbePingAsync(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(host) || port <= 0) return -1;
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                using (TcpClient client = new TcpClient())
                {
                    Task connect = client.ConnectAsync(host, port);
                    Task done = await Task.WhenAny(connect, Task.Delay(2500));
                    if (done == connect && !connect.IsFaulted && client.Connected) { sw.Stop(); return (int)sw.ElapsedMilliseconds; }
                }
            }
            catch { }
            return -1;
        }

        /// <summary>Server picked by the player (launcher.ini Server=), else the first published one.</summary>
        private GameServerDto SelectedServer()
        {
            if (_servers == null || _servers.servers.Count == 0) return null;
            foreach (GameServerDto s in _servers.servers)
                if (string.Equals(s.name, _cfg.SelectedServer, StringComparison.OrdinalIgnoreCase)) return s;
            return _servers.servers[0];
        }

        /// <summary>One server: the same single status line as before. More: the line becomes a picker.</summary>
        private void ShowServers()
        {
            _statusFromList = true;
            List<GameServerDto> all = _servers.servers;
            bool loginKnown = _servers.login != null && !string.IsNullOrEmpty(_servers.login.host);
            bool loginOk = !loginKnown || _servers.login.online;
            GameServerDto selected = SelectedServer();

            if (all.Count == 1)
            {
                ServerArrow.Visibility = Visibility.Collapsed;
                VersionText.Visibility = Visibility.Visible;
                StatusBox.Cursor = Cursors.Arrow;
                ServerPanel.Visibility = Visibility.Collapsed;
                if (all[0].maintenance) SetMaintenanceStatus();
                else if (!loginOk && all[0].online) SetStatusText(false, Lang.T("LoginOffline"));
                else SetStatus(all[0].online && loginOk);
            }
            else
            {
                Color green = Color.FromRgb(0x4C, 0xE0, 0x6A), red = Color.FromRgb(0xE0, 0x40, 0x30), amber = Color.FromRgb(0xF0, 0xA0, 0x40);
                List<ServerRow> rows = new List<ServerRow>();
                foreach (GameServerDto s in all)
                {
                    bool up = s.online && loginOk;
                    bool isSel = ReferenceEquals(s, selected);
                    Color c = s.maintenance ? amber : (up ? green : red);
                    string state = s.maintenance ? Lang.T("SrvMaintenance") : (up ? Lang.T("SrvOnline") : Lang.T("SrvOffline"));
                    if (up && !s.maintenance && s.players >= 0) state += " (" + s.players + ")";
                    rows.Add(new ServerRow
                    {
                        Name = s.name,
                        StateText = state,
                        DotBrush = new SolidColorBrush(c),
                        RowBackground = new SolidColorBrush(isSel ? Color.FromArgb(0x40, 0xE8, 0xB6, 0x4E) : Colors.Transparent),
                        RowBorder = new SolidColorBrush(isSel ? Color.FromArgb(0xB0, 0xE8, 0xB6, 0x4E) : Colors.Transparent),
                        Tip = string.IsNullOrEmpty(s.Notice) ? Lang.T("PickServer") : s.Notice
                    });
                }
                ServerList.ItemsSource = rows;
                ServerArrow.Visibility = Visibility.Visible;
                VersionText.Visibility = Visibility.Collapsed;
                StatusBox.Cursor = Cursors.Hand;
                bool selUp = selected != null && selected.online && loginOk;
                Color sc = selected != null && selected.maintenance ? amber : (selUp ? green : red);
                StatusText.Text = selected != null ? selected.name + " - " + (selected.maintenance ? Lang.T("SrvMaintenance") : (selUp ? Lang.T("SrvOnline") : Lang.T("SrvOffline"))) : "";
                StatusDot.Fill = new SolidColorBrush(sc);
                if (!loginOk) SetStatusText(false, Lang.T("LoginOffline"));
                else if (selected != null && selected.maintenance) SetMaintenanceStatus();
                else SetStatusText(selUp, selUp ? Lang.T("Online") : Lang.T("Offline"));
            }

            string notice = selected != null ? selected.Notice : null;
            ServerNotice.Text = notice ?? "";
            ServerNotice.FlowDirection = FlowFor(notice);
            ServerNotice.Visibility = string.IsNullOrWhiteSpace(notice) ? Visibility.Collapsed : Visibility.Visible;
            StatusBox.ToolTip = string.IsNullOrWhiteSpace(notice) ? (all.Count > 1 ? Lang.T("PickServer") : null) : notice;
        }

        private bool _showStatus = true;

        /// <summary>
        /// The status pill under the play button always shows the (chosen) server state; with more than one
        /// server the picker box above the button appears too.
        /// </summary>
        private void UpdateStatusPlacement()
        {
            bool picker = _statusFromList && _servers != null && _servers.servers.Count > 1;
            StatusCard.Visibility = _showStatus && picker ? Visibility.Visible : Visibility.Collapsed;
            StatusFooter.Visibility = _showStatus ? Visibility.Visible : Visibility.Collapsed;
            FooterStatus.Text = StatusText.Text;
            FooterDot.Fill = StatusDot.Fill;
            FooterHalo.Fill = StatusDot.Fill;
            if (picker)
            {
                GameServerDto sel = SelectedServer();
                if (sel != null) StatusText.Text = sel.name;   // the box shows the chosen server, the pill its state
            }
        }

        private void SetMaintenanceStatus()
        {
            StatusText.Text = _theme.Text("Texts", "StatusMaintenance", Lang.T("SrvMaintenance"), Lang.T("SrvMaintenance"));
            StatusDot.Fill = new SolidColorBrush(Color.FromRgb(0xF0, 0xA8, 0x30));
            UpdateStatusPlacement();
        }

        private void StatusBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (_servers == null || _servers.servers.Count < 2) return;
            if (ServerPanel.Visibility == Visibility.Visible) ServerPanel.Visibility = Visibility.Collapsed;
            else OpenServerPanel();
            e.Handled = true;
        }

        /// <summary>The list grows upwards from the server box.</summary>
        private void OpenServerPanel()
        {
            ServerPanel.Visibility = Visibility.Visible;
            ServerPanel.UpdateLayout();
            Canvas.SetTop(ServerPanel, 442 - ServerPanel.ActualHeight);
        }

        private void ServerRow_Click(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            string name = fe != null ? fe.Tag as string : null;
            if (string.IsNullOrEmpty(name) || _servers == null) return;
            _cfg.SaveSelectedServer(name);
            ShowServers();
            ServerPanel.Visibility = Visibility.Collapsed;
            WriteClientServerList(false);
            if (!_updating) RefreshReadyState();
            e.Handled = true;
        }

        /// <summary>Manifest path of the client server list (launcher.ini ServerListFile), null when disabled.</summary>
        private string ClientListManifestPath()
        {
            string f = _cfg.ServerListFile;
            if (string.IsNullOrWhiteSpace(f)) return null;
            return f.Trim().Replace('\\', '/').TrimStart('/');
        }

        /// <summary>
        /// Writes the client's login server list (Guard.dat) from /servers. Nothing happens when the patch
        /// server did not answer: the client then keeps the list it already has.
        /// </summary>
        private void WriteClientServerList(bool showErrors)
        {
            if (App.DemoState != null) return;
            if (_servers == null || string.IsNullOrWhiteSpace(_cfg.ServerListFile)) return;
            try
            {
                GameServerDto sel = _servers.servers.Count > 1 ? SelectedServer() : null;
                List<GuardFile.Entry> entries = ServerListClient.ToGuardEntries(_servers, sel != null ? sel.name : null, _cfg.ServerIP, _cfg.LoginPort);
                GuardFile.Write(_cfg.Resolve(_cfg.ServerListFile.Trim()), entries);
            }
            catch (Exception ex)
            {
                if (showErrors) ShowDialog(Lang.T("NoticeTitle"), Lang.T("ServerListFailed") + ex.Message, DialogKind.Warning);
            }
        }

        /// <summary>Status line with a custom text (green dot when ok).</summary>
        private void SetStatusText(bool ok, string text)
        {
            Color c = ok ? Color.FromRgb(0x4C, 0xE0, 0x6A) : Color.FromRgb(0xE0, 0x40, 0x30);
            StatusText.Text = text;
            StatusDot.Fill = new SolidColorBrush(c);
            UpdateStatusPlacement();
        }

        private void SetStatus(bool? online)
        {
            _lastOnline = online;
            _statusChecked = true;
            Color c;
            if (online == null)
            {
                c = Colors.Gray;
                StatusText.Text = _theme.Text("Texts", "StatusUnknown", Lang.T("Unknown"), Lang.T("Unknown"));
            }
            else if (online.Value)
            {
                c = Color.FromRgb(0x4C, 0xE0, 0x6A);
                StatusText.Text = _theme.Text("Texts", "StatusOnline", Lang.T("Online"), Lang.T("Online"));
            }
            else
            {
                c = Color.FromRgb(0xE0, 0x40, 0x30);
                StatusText.Text = _theme.Text("Texts", "StatusOffline", Lang.T("Offline"), Lang.T("Offline"));
            }
            StatusDot.Fill = new SolidColorBrush(c);
            UpdateStatusPlacement();
        }

        // ============================================================ play

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            switch (_state)
            {
                case PlayState.Updating:
                    if (_runCts != null) _runCts.Cancel();   // pause: the current file is downloaded again on resume
                    return;
                case PlayState.Update:
                    RunUpdateAsync();
                    return;
                case PlayState.Maintenance:
                    if (!await ShowDialogAsync(Lang.T("SrvMaintenance"), Lang.T("MaintenanceAsk"), DialogKind.Question, Lang.T("StartAnyway"), Lang.T("Cancel"))) return;
                    break;
            }
            if (_updating) return;

            // Block a second launch: lock Play the moment we commit to launching (a double-click, or
            // clicking again while the async system check runs). If the launch never actually starts the
            // game - file missing, check cancelled, Process.Start threw - the button is turned back on in
            // the finally below so the player can try again. If the game does start, it stays locked until
            // the game exits (ReEnablePlayWhenGameExits / OnGameExited turn it back on).
            if (_launching) return;
            _launching = true;
            PlayButton.IsEnabled = false;
            bool launched = false;
            try
            {
                // Demo mode has no real game to start. Keep Play locked for a couple of seconds (as if the
                // game is launching) so the lock is visible, then show the crash dialog to prove it unlocks
                // again on an error. The real path below is what runs for players.
                if (App.DemoState != null)
                {
                    await Task.Delay(2500);
                    _launching = false;
                    PlayButton.IsEnabled = true;
                    string cb = Lang.T("CrashBody") + "\n" + Lang.T("CrashExit", -1073741819);
                    if (!string.IsNullOrEmpty(_cfg.CrashUrl())) cb += "\n" + Lang.T("CrashSent");
                    launched = true;   // we already unlocked Play ourselves
                    await ShowDialogAsync(Lang.T("CrashTitle"), cb, DialogKind.Error, Lang.T("OpenReport"), Lang.T("Cancel"));
                    return;
                }

                string exe = _cfg.Resolve(_cfg.GameExeFor());
                if (!File.Exists(exe))
                {
                    ShowDialog(Lang.T("ErrorTitle"), Lang.T("GameNotFound") + exe, DialogKind.Warning);
                    return;
                }
                // pre-launch system check (DirectX / VC++ runtime / disk / required files)
                if (_cfg.SysCheckEnabled)
                {
                    SystemCheckResult chk = await Task.Run(() => SystemCheck.Run(_cfg));
                    _lastCheck = chk;
                    if (chk.HasFail || chk.HasWarn)
                    {
                        string msg = SystemCheck.Summary(chk, true);
                        if (chk.HasFail && _cfg.SysCheckBlockOnFail)
                        {
                            await ShowDialogAsync(Lang.T("SysCheckTitle"), msg + "\n\n" + Lang.T("SysCheckBlocked"), DialogKind.Error);
                            return;
                        }
                        string tail = chk.HasFail && _cfg.SysCheckBlockOnFail ? Lang.T("SysCheckBlocked") : Lang.T("SysCheckContinue");
                        if (!await ShowDialogAsync(Lang.T("SysCheckTitle"), msg + "\n\n" + tail, DialogKind.Warning, Lang.T("StartAnyway"), Lang.T("Cancel")))
                            return;
                    }
                }

                WriteClientServerList(true);
                Process proc = null;
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(exe, _cfg.GameArgs ?? "");
                    psi.WorkingDirectory = _cfg.Resolve(_cfg.WorkingDirFor());
                    psi.UseShellExecute = true;
                    proc = Process.Start(psi);
                }
                catch (Exception ex)
                {
                    ShowDialog(Lang.T("ErrorTitle"), Lang.T("GameFailed") + ex.Message, DialogKind.Error);
                    return;
                }

                launched = true;   // the game started - keep Play locked so it can't be started a second time
                if (_cfg.CrashReporting && proc != null)
                {
                    // Watch the game for an early/abnormal exit. We minimise instead of closing right away so an
                    // early crash can be caught; once the game survives the watch window we honour CloseOnPlay.
                    MonitorGame(proc);
                    WindowState = WindowState.Minimized;
                    DispatcherTimer watch = new DispatcherTimer { Interval = TimeSpan.FromSeconds(_cfg.CrashWatchSeconds) };
                    watch.Tick += delegate
                    {
                        watch.Stop();
                        if (!_crashHandled && _game != null && !GameHasExited(_game) && _cfg.CloseOnPlay) Close();
                    };
                    watch.Start();
                }
                else if (_cfg.CloseOnPlay) Close();
                else
                {
                    WindowState = WindowState.Minimized;
                    ReEnablePlayWhenGameExits(proc);   // crash watch is off, but still unlock Play when the game exits
                }
            }
            finally
            {
                // The launch never started the game: unlock Play so the player can press it again.
                if (!launched) { _launching = false; PlayButton.IsEnabled = true; }
            }
        }

        /// <summary>
        /// When crash monitoring is off (CrashReporting=0) we still want Play to come back once the game
        /// closes, so a player who returns to a minimised launcher can relaunch. Best-effort: some shells
        /// block Exited events, in which case the button is governed by the window state instead.
        /// </summary>
        private void ReEnablePlayWhenGameExits(Process proc)
        {
            if (proc == null) { _launching = false; try { PlayButton.IsEnabled = true; } catch { } return; }
            try
            {
                proc.EnableRaisingEvents = true;
                proc.Exited += delegate
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        _launching = false;
                        if (PlayButton != null) PlayButton.IsEnabled = true;
                    }));
                };
            }
            catch { }
        }

        // ============================================================ game crash monitoring

        private SystemCheckResult _lastCheck;
        private Process _game;
        private DateTime _gameStart;
        private bool _crashHandled;
        /// <summary>True from the moment Play is pressed until the game exits (or the launch fails): blocks a second launch.</summary>
        private bool _launching;

        /// <summary>Hook the game process so we hear about an abnormal exit (non-zero code or a very early exit).</summary>
        private void MonitorGame(Process p)
        {
            _game = p;
            _gameStart = DateTime.Now;
            _crashHandled = false;
            try
            {
                p.EnableRaisingEvents = true;
                p.Exited += GameExited;   // fires on a thread-pool thread
            }
            catch { /* some shell launches do not allow raising events; crash watch is best-effort */ }
        }

        private void GameExited(object sender, EventArgs e)
        {
            Process p = sender as Process;
            int code;
            try { code = p != null ? p.ExitCode : 0; } catch { code = 0; }
            double secs = (DateTime.Now - _gameStart).TotalSeconds;
            bool abnormal = code != 0 || secs < _cfg.CrashWatchSeconds;
            try { Dispatcher.BeginInvoke(new Action(delegate { OnGameExited(code, secs, abnormal); })); }
            catch { }
        }

        private async void OnGameExited(int code, double secs, bool abnormal)
        {
            if (_crashHandled) return;
            _crashHandled = true;
            // Back at the launcher (the game exited): unlock Play so another launch is possible.
            _launching = false;
            try { PlayButton.IsEnabled = true; } catch { }
            if (!abnormal)
            {
                if (_cfg.CloseOnPlay) Close();
                return;
            }
            // bring the launcher back and offer a crash report
            try { if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal; Show(); Activate(); } catch { }
            string path = CrashReport.WriteGameCrash(_cfg, _lastCheck, code, secs);
            string body = Lang.T("CrashBody") + "\n" + Lang.T("CrashExit", code);
            if (!string.IsNullOrEmpty(_cfg.CrashUrl())) body += "\n" + Lang.T("CrashSent");
            if (path != null) body += "\n" + path;
            bool open = await ShowDialogAsync(Lang.T("CrashTitle"), body, DialogKind.Error,
                path != null ? Lang.T("OpenReport") : Lang.T("Ok"), path != null ? Lang.T("Cancel") : null);
            if (open && path != null) CrashReport.OpenFolder(path);
        }

        private static bool GameHasExited(Process p)
        {
            try { return p.HasExited; } catch { return true; }
        }

        // ============================================================ settings overlay

        private static readonly string[] Resolutions = { "1024x768", "1280x720", "1280x800", "1280x1024", "1366x768", "1440x900", "1600x900", "1680x1050", "1920x1080", "1920x1200", "2560x1440" };
        private bool _settingsLoading;

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            _settingsLoading = true;
            IniText ini = new IniText(_cfg.Resolve(_cfg.SettingsIni));
            string mode = ini.Get("Screen", "ResolutionMode", "fullscreen").ToLowerInvariant();
            ModeFull.IsChecked = mode != "windowed" && mode != "custom";
            ModeWindow.IsChecked = mode == "windowed";
            ModeCustom.IsChecked = mode == "custom";
            WidthBox.Text = ini.Get("Screen", "CustomWidth", "1024");
            HeightBox.Text = ini.Get("Screen", "CustomHeight", "768");
            string cur = WidthBox.Text.Trim() + "x" + HeightBox.Text.Trim();
            List<string> res = new List<string>(Resolutions);
            if (!res.Contains(cur)) res.Insert(0, cur);
            ResolutionBox.ItemsSource = res;
            ResolutionBox.SelectedItem = cur;
            FpsBox.Text = ini.Get("GameFPS", "Value", "60");
            int fps;
            if (int.TryParse(FpsBox.Text, out fps)) { FpsSlider.Maximum = Math.Max(240, fps); FpsSlider.Value = Math.Max(30, fps); }
            CloseOnPlayBox.IsChecked = _cfg.CloseOnPlay;
            Dx9Box.IsChecked = _cfg.DirectX != 8;
            Dx8Box.IsChecked = _cfg.DirectX == 8;
            LangArBox.IsChecked = Lang.IsArabic;
            LangEnBox.IsChecked = !Lang.IsArabic;
            RepairBtn.IsEnabled = !_updating;
            RepairHint.Text = Lang.T("RepairHint");
            if (PreCheckBox != null) PreCheckBox.IsChecked = _cfg.SysCheckEnabled;
            _musicLoading = true;
            MusicSlider.Value = Math.Round(_musicVolume * 100);
            MusicValue.Text = ((int)Math.Round(_musicVolume * 100)) + "%";
            MusicMuteBox.IsChecked = _musicMuted;
            _musicLoading = false;
            MusicLabel.Text = Lang.T("MusicLabel");
            MusicMuteBox.Content = Lang.T("MusicMute");
            _settingsLoading = false;
            UpdateResolutionEnabled();
            OpenPanel(SettingsOverlay, SettingsScale);
        }

        private void Mode_Changed(object sender, RoutedEventArgs e) { UpdateResolutionEnabled(); }

        private void UpdateResolutionEnabled()
        {
            if (ResolutionBox == null || WidthBox == null || HeightBox == null) return;
            bool custom = ModeCustom.IsChecked == true;
            ResolutionBox.IsEnabled = custom;
            WidthBox.IsEnabled = custom;
            HeightBox.IsEnabled = custom;
            double o = custom ? 1 : 0.45;
            ResolutionBox.Opacity = o; WidthBox.Opacity = o; HeightBox.Opacity = o;
        }

        private void Resolution_Changed(object sender, SelectionChangedEventArgs e)
        {
            string r = ResolutionBox.SelectedItem as string;
            if (r == null || _settingsLoading) return;
            string[] p = r.Split('x');
            if (p.Length != 2) return;
            WidthBox.Text = p[0];
            HeightBox.Text = p[1];
        }

        private void Fps_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_settingsLoading || FpsBox == null) return;
            FpsBox.Text = ((int)FpsSlider.Value).ToString(CultureInfo.InvariantCulture);
        }

        private void SettingsSave_Click(object sender, RoutedEventArgs e)
        {
            int w, h, fps;
            if (!int.TryParse(WidthBox.Text.Trim(), out w) || !int.TryParse(HeightBox.Text.Trim(), out h) || w < 640 || h < 480)
            {
                ShowDialog(Lang.T("SettingsTitle"), Lang.T("BadSize"), DialogKind.Warning);
                return;
            }
            if (!int.TryParse(FpsBox.Text.Trim(), out fps) || fps < 30 || fps > 1000)
            {
                ShowDialog(Lang.T("SettingsTitle"), Lang.T("BadFps"), DialogKind.Warning);
                return;
            }
            string mode = ModeWindow.IsChecked == true ? "windowed" : ModeCustom.IsChecked == true ? "custom" : "fullscreen";
            try
            {
                IniText ini = new IniText(_cfg.Resolve(_cfg.SettingsIni));
                ini.Set("Screen", "ResolutionMode", mode);
                ini.Set("Screen", "CustomWidth", w.ToString(CultureInfo.InvariantCulture));
                ini.Set("Screen", "CustomHeight", h.ToString(CultureInfo.InvariantCulture));
                ini.Set("GameFPS", "Value", fps.ToString(CultureInfo.InvariantCulture));
                ini.Save();
            }
            catch (Exception ex)
            {
                ShowDialog(Lang.T("ErrorTitle"), Lang.T("SaveFailed") + ex.Message, DialogKind.Error);
                return;
            }
            _cfg.SaveCloseOnPlay(CloseOnPlayBox.IsChecked == true);
            if (PreCheckBox != null) _cfg.SaveSysCheckEnabled(PreCheckBox.IsChecked == true);
            _cfg.SaveMusicVolume((int)Math.Round(MusicSlider.Value));
            int dx = Dx8Box.IsChecked == true ? 8 : 9;
            if (dx != _cfg.DirectX) _cfg.SaveDirectX(dx);
            ClosePanel(SettingsOverlay, SettingsScale);
            string lang = LangEnBox.IsChecked == true ? "en" : "ar";
            if (lang != Lang.Current)
            {
                Lang.Set(lang);
                _cfg.SaveLanguage(lang);
                ApplyTheme();
            }
        }

        private void SettingsCancel_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel(SettingsOverlay, SettingsScale);
        }

        // ============================================================ system check panel

        /// <summary>Opens the themed system-check panel from the settings row (runs the check first).</summary>
        private async void OpenSysCheck_Click(object sender, RoutedEventArgs e)
        {
            SysCheckPanel.FlowDirection = Lang.Flow;
            SysCheckTitleText.Text = Lang.T("SysCheckTitle");
            SysCheckRecheck.Content = Lang.T("Recheck");
            SysCheckClose.Content = Lang.T("Ok");
            SysCheckList.Children.Clear();
            SysCheckStatusText.Text = Lang.T("Checking");
            OpenPanel(SysCheckOverlay, SysCheckScale);
            await RunSysCheckIntoPanel();
        }

        private async void SysCheckRecheck_Click(object sender, RoutedEventArgs e)
        {
            SysCheckStatusText.Text = Lang.T("Checking");
            await RunSysCheckIntoPanel();
        }

        private void SysCheckClose_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel(SysCheckOverlay, SysCheckScale);
        }

        private async Task RunSysCheckIntoPanel()
        {
            SystemCheckResult r = await Task.Run(() => SystemCheck.Run(_cfg));
            _lastCheck = r;
            PopulateSysCheck(r);
        }

        private void PopulateSysCheck(SystemCheckResult r)
        {
            SysCheckList.Children.Clear();
            foreach (CheckItem it in r.Items)
                SysCheckList.Children.Add(BuildCheckRow(it));
            SysCheckStatusText.Text = r.HasFail || r.HasWarn
                ? SysCheckShort(r) : Lang.T("SysCheckAllOk");
            SysCheckStatusText.Foreground = r.HasFail
                ? new SolidColorBrush(Color.FromRgb(0xC0, 0x3A, 0x2A))
                : (Brush)FindResource("PaperDim");
        }

        private static string SysCheckShort(SystemCheckResult r)
        {
            int fails = 0, warns = 0;
            foreach (CheckItem i in r.Items)
            {
                if (i.Status == CheckStatus.Fail) fails++;
                else if (i.Status == CheckStatus.Warn) warns++;
            }
            string s = "";
            if (fails > 0) s += (Lang.IsArabic ? fails + " مشكلة" : fails + " error" + (fails == 1 ? "" : "s"));
            if (warns > 0) s += (s.Length > 0 ? " · " : "") + (Lang.IsArabic ? warns + " تحذير" : warns + " warning" + (warns == 1 ? "" : "s"));
            return s;
        }

        private FrameworkElement BuildCheckRow(CheckItem it)
        {
            Color dot = it.Status == CheckStatus.Fail ? Color.FromRgb(0xD0, 0x3A, 0x2A)
                : it.Status == CheckStatus.Warn ? Color.FromRgb(0xE0, 0x9A, 0x30)
                : Color.FromRgb(0x4C, 0xA0, 0x50);
            Grid g = new Grid { Margin = new Thickness(0, 4, 0, 4) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(16) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Ellipse e = new Ellipse { Width = 9, Height = 9, Fill = new SolidColorBrush(dot), VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 4, 0, 0) };
            Grid.SetColumn(e, 0);
            g.Children.Add(e);

            StackPanel sp = new StackPanel();
            Grid.SetColumn(sp, 1);
            TextBlock title = new TextBlock { Text = it.Title, FontWeight = FontWeights.Bold, Foreground = (Brush)FindResource("PaperInk"), TextWrapping = TextWrapping.Wrap };
            title.FlowDirection = FlowFor(it.Title);
            sp.Children.Add(title);
            if (!string.IsNullOrEmpty(it.Detail))
            {
                TextBlock d = new TextBlock { Text = it.Detail, Foreground = (Brush)FindResource("PaperDim"), FontSize = 11.5, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 1, 0, 0) };
                d.FlowDirection = FlowFor(it.Detail);
                sp.Children.Add(d);
            }
            if (!string.IsNullOrEmpty(it.FixText))
            {
                StackPanel fix = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 0) };
                TextBlock ft = new TextBlock { Text = it.FixText, Foreground = (Brush)FindResource("CardAuthor"), FontSize = 11.5, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center };
                ft.FlowDirection = FlowFor(it.FixText);
                fix.Children.Add(ft);
                if (!string.IsNullOrEmpty(it.FixUrl))
                {
                    Button link = new Button { Style = (Style)FindResource("PaperButton"), Content = "↓", Margin = new Thickness(8, 0, 0, 0), Tag = it.FixUrl, ToolTip = it.FixUrl };
                    link.Click += delegate { OpenUrl(it.FixUrl); };
                    fix.Children.Add(link);
                }
                sp.Children.Add(fix);
            }
            g.Children.Add(sp);
            return g;
        }

        /// <summary>Repair = forget the MD5 cache so the updater hashes every file again and downloads the broken ones.</summary>
        private async void Repair_Click(object sender, RoutedEventArgs e)
        {
            if (_updating)
            {
                RepairHint.Text = Lang.T("RepairBusy");
                return;
            }
            if (!await ShowDialogAsync(Lang.T("Repair"), Lang.T("RepairAsk"), DialogKind.Question, Lang.T("Repair"), Lang.T("Cancel"))) return;
            try
            {
                string cache = System.IO.Path.Combine(_cfg.BaseDir, "launcher_cache.txt");
                if (File.Exists(cache)) File.Delete(cache);
            }
            catch { }
            ClosePanel(SettingsOverlay, SettingsScale);
            if (App.DemoState == null) RunUpdateAsync();
        }

        // ============================================================ panels + themed dialogs (no Windows MessageBox)

        public enum DialogKind { Info, Warning, Error, Question }

        private TaskCompletionSource<bool> _dialogResult;

        /// <summary>Fade + slight zoom in (180 ms, 30 fps cap).</summary>
        private static void OpenPanel(FrameworkElement overlay, ScaleTransform scale)
        {
            overlay.Visibility = Visibility.Visible;
            Duration d = new Duration(TimeSpan.FromMilliseconds(180));
            IEasingFunction ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            overlay.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, d));
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.94, 1, d) { EasingFunction = ease });
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.94, 1, d) { EasingFunction = ease });
        }

        private static void ClosePanel(FrameworkElement overlay, ScaleTransform scale)
        {
            if (overlay.Visibility != Visibility.Visible) return;
            Duration d = new Duration(TimeSpan.FromMilliseconds(140));
            DoubleAnimation fade = new DoubleAnimation(1, 0, d);
            fade.Completed += delegate { if (overlay.Opacity < 0.05) overlay.Visibility = Visibility.Collapsed; };
            overlay.BeginAnimation(OpacityProperty, fade);
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, 0.96, d));
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, 0.96, d));
        }

        /// <summary>Message box in the launcher style (parchment card). Fire and forget.</summary>
        public void ShowDialog(string title, string text, DialogKind kind)
        {
            Task<bool> ignored = ShowDialogAsync(title, text, kind);
        }

        /// <summary>Themed dialog; returns true for OK/Yes. cancel == null shows only one button.</summary>
        public Task<bool> ShowDialogAsync(string title, string text, DialogKind kind, string ok = null, string cancel = null)
        {
            if (_dialogResult != null) _dialogResult.TrySetResult(false);
            _dialogResult = new TaskCompletionSource<bool>();
            DialogPanel.FlowDirection = Lang.Flow;
            DialogTitle.Text = title ?? "Hazem";
            DialogText.Text = text ?? "";
            DialogText.FlowDirection = FlowFor(text);
            DialogOk.Content = ok ?? Lang.T("Ok");
            DialogNo.Content = cancel ?? Lang.T("Cancel");
            DialogNo.Visibility = cancel == null ? Visibility.Collapsed : Visibility.Visible;
            switch (kind)
            {
                case DialogKind.Error: DialogIcon.Text = ""; DialogIconDisc.Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x6A, 0x50)); break;
                case DialogKind.Warning: DialogIcon.Text = ""; DialogIconDisc.Fill = (Brush)FindResource("PlayFace"); break;
                case DialogKind.Question: DialogIcon.Text = ""; DialogIconDisc.Fill = (Brush)FindResource("PlayFace"); break;
                default: DialogIcon.Text = ""; DialogIconDisc.Fill = (Brush)FindResource("PlayFace"); break;
            }
            OpenPanel(DialogOverlay, DialogScale);
            return _dialogResult.Task;
        }

        private void DialogOk_Click(object sender, RoutedEventArgs e) { CloseDialog(true); }

        private void DialogCancel_Click(object sender, RoutedEventArgs e) { CloseDialog(false); }

        private void CloseDialog(bool result)
        {
            ClosePanel(DialogOverlay, DialogScale);
            TaskCompletionSource<bool> t = _dialogResult;
            _dialogResult = null;
            if (t != null) t.TrySetResult(result);
        }

        // ============================================================ window chrome

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed) return;
            if (ServerPanel.Visibility == Visibility.Visible && !ServerPanel.IsMouseOver && !StatusBox.IsMouseOver) ServerPanel.Visibility = Visibility.Collapsed;
            try { DragMove(); } catch (InvalidOperationException) { }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _cts.Cancel();
            PauseEmbers();
            if (_statusTimer != null) _statusTimer.Stop();
            if (_eventsTimer != null) _eventsTimer.Stop();
            if (_newsTimer != null) _newsTimer.Stop();
            StopSlideshowTimer();
            try { if (_music != null) { _music.Stop(); _music.Close(); } } catch { }
            try { BgVideo.Stop(); BgVideo.Close(); } catch { }
            if (_themeWatcher != null) _themeWatcher.Dispose();
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string key = b != null ? b.Tag as string : null;
            if (key == "Facebook") OpenUrl(_cfg.Facebook);
            else if (key == "Discord") OpenUrl(_cfg.Discord);
            else if (key == "Website") OpenUrl(_cfg.Website);
        }

        private static void OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return;
            try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
        }

        // ============================================================ background slideshow
        // Two Image layers; a slow cross-fade (DoubleAnimation on Opacity only) runs just at the
        // transition, capped at 30 fps by the global DesiredFrameRate. Between transitions nothing
        // animates, so CPU is zero. The whole slideshow is paused when the window is unfocused or
        // minimized. Images are decoded off the UI thread to the display size and frozen (cached).

        /// <summary>(Re)reads theme.ini [Background] and starts/stops the slideshow. Safe to call again.</summary>
        private void ConfigureSlideshow()
        {
            Theme t = _theme;
            // mask the scene with the frame PNG so it keeps the launcher silhouette + soft edges
            ImageSource frame = t.Image("Images", "Frame", "images/frame_smoke.png");
            BgSlideMask.ImageSource = frame;

            StopSlideshowTimer();
            _bgPaths.Clear();
            _bgIndex = -1; _bgNext = null; _bgNextIndex = -1; _bgShowingA = true;
            BgSlideA.Source = null; BgSlideB.Source = null; BgSlideA.Opacity = 1; BgSlideB.Opacity = 0;

            _bgEnabled = t.Bool("Background", "Slideshow", false) && frame != null;
            if (!_bgEnabled) { BgSlideshow.Visibility = Visibility.Collapsed; ConfigureVideo(null); return; }

            // explicit Images= list wins; otherwise every image in Folder= (relative to the theme)
            foreach (string p in ResolveSlideshowImages(t)) _bgPaths.Add(p);

            string videoPath = t.FilePath("Background", "Video", "");
            ConfigureVideo(videoPath);

            if (_bgPaths.Count == 0 && videoPath == null)
            {
                // nothing to show -> fall back to the static frame (unchanged behaviour)
                BgSlideshow.Visibility = Visibility.Collapsed;
                return;
            }

            _bgFadeMs = Math.Max(0, (int)t.Num("Background", "FadeMs", 1500));
            _bgRandom = string.Equals(t.Str("Background", "Order", "sequential").Trim(), "random", StringComparison.OrdinalIgnoreCase);
            double interval = Math.Max(3, t.Num("Background", "IntervalSeconds", 10));

            BgSlideshow.CacheMode = new BitmapCache { RenderAtScale = 1.0 };
            BgSlideshow.Visibility = Visibility.Visible;

            if (_bgPaths.Count > 0)
            {
                int first = _bgRandom ? _rnd.Next(_bgPaths.Count) : 0;
                BitmapImage img = Theme.LoadImage(_bgPaths[first], BgDecodeWidth);
                BgSlideA.Source = img;
                BgSlideA.Opacity = 1; BgSlideB.Opacity = 0;
                _bgShowingA = true; _bgIndex = first;
                if (_bgPaths.Count > 1)
                {
                    PreloadNext();
                    _bgTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(interval) };
                    _bgTimer.Tick += delegate { AdvanceSlideshow(); };
                    if ((IsActive || App.EmbersAlways) && WindowState != WindowState.Minimized) _bgTimer.Start();
                }
            }
        }

        private IEnumerable<string> ResolveSlideshowImages(Theme t)
        {
            List<string> result = new List<string>();
            string list = t.Str("Background", "Images", "");
            if (!string.IsNullOrWhiteSpace(list))
            {
                foreach (string raw in list.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string rel = raw.Trim();
                    if (rel.Length == 0) continue;
                    string full = System.IO.Path.IsPathRooted(rel) ? rel : System.IO.Path.GetFullPath(System.IO.Path.Combine(t.Dir, rel));
                    if (File.Exists(full)) result.Add(full);
                }
                return result;
            }
            string folderRel = t.Str("Background", "Folder", "backgrounds").Trim();
            if (folderRel.Length == 0) return result;
            string folder = System.IO.Path.IsPathRooted(folderRel) ? folderRel : System.IO.Path.GetFullPath(System.IO.Path.Combine(t.Dir, folderRel));
            if (!Directory.Exists(folder)) return result;
            try
            {
                List<string> files = new List<string>(Directory.GetFiles(folder));
                files.Sort(StringComparer.OrdinalIgnoreCase);
                foreach (string f in files)
                {
                    string ext = System.IO.Path.GetExtension(f).ToLowerInvariant();
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp") result.Add(f);
                }
            }
            catch { }
            return result;
        }

        /// <summary>Decodes the following image off the UI thread so the transition never decodes on it.</summary>
        private void PreloadNext()
        {
            if (_bgPaths.Count < 2) return;
            int next = _bgRandom ? NextRandom() : (_bgIndex + 1) % _bgPaths.Count;
            _bgNextIndex = next;
            string path = _bgPaths[next];
            Task.Run(delegate
            {
                BitmapImage img = Theme.LoadImage(path, BgDecodeWidth); // returns a frozen BitmapImage
                Dispatcher.BeginInvoke(new Action(delegate { if (_bgNextIndex == next) _bgNext = img; }));
            });
        }

        private int NextRandom()
        {
            if (_bgPaths.Count <= 1) return 0;
            int n;
            do { n = _rnd.Next(_bgPaths.Count); } while (n == _bgIndex);
            return n;
        }

        private void AdvanceSlideshow()
        {
            if (!_bgEnabled || _bgPaths.Count < 2) return;
            if (!IsActive && !App.EmbersAlways) { StopSlideshowTimer(); return; }
            if (WindowState == WindowState.Minimized) { StopSlideshowTimer(); return; }

            int next = _bgNextIndex >= 0 ? _bgNextIndex : (_bgRandom ? NextRandom() : (_bgIndex + 1) % _bgPaths.Count);
            BitmapImage img = _bgNext ?? Theme.LoadImage(_bgPaths[next], BgDecodeWidth);
            if (img == null) { PreloadNext(); return; }

            Image incoming = _bgShowingA ? BgSlideB : BgSlideA;
            Image outgoing = _bgShowingA ? BgSlideA : BgSlideB;
            incoming.Source = img;
            incoming.Opacity = 0;

            Duration d = new Duration(TimeSpan.FromMilliseconds(_bgFadeMs));
            IEasingFunction ease = new SineEase { EasingMode = EasingMode.EaseInOut };
            DoubleAnimation fin = new DoubleAnimation(0, 1, d) { EasingFunction = ease };
            DoubleAnimation fout = new DoubleAnimation(1, 0, d) { EasingFunction = ease };
            incoming.BeginAnimation(OpacityProperty, fin);
            outgoing.BeginAnimation(OpacityProperty, fout);

            _bgShowingA = !_bgShowingA;
            _bgIndex = next;
            _bgNext = null; _bgNextIndex = -1;
            PreloadNext();
        }

        private void StopSlideshowTimer()
        {
            if (_bgTimer != null) _bgTimer.Stop();
        }

        private void PauseSlideshow()
        {
            if (App.EmbersAlways && IsVisible && WindowState != WindowState.Minimized) return;
            StopSlideshowTimer();
        }

        private void ResumeSlideshow()
        {
            if (_bgEnabled && _bgTimer != null && _bgPaths.Count > 1 && WindowState != WindowState.Minimized)
                _bgTimer.Start();
        }

        /// <summary>
        /// Optional, OFF by default: MediaElement over AllowsTransparency is heavy, so video only plays
        /// when a [Background] Video path is set AND the GPU supports hardware acceleration (RenderCapability
        /// Tier >= 2). On low-end / software rendering it is skipped. Looping, muted; sits under the images.
        /// </summary>
        private void ConfigureVideo(string path)
        {
            try { BgVideo.Stop(); } catch { }
            BgVideo.Source = null;
            BgVideo.Visibility = Visibility.Collapsed;
            if (string.IsNullOrEmpty(path)) return;
            bool tierOk = (System.Windows.Media.RenderCapability.Tier >> 16) >= 2;
            if (!tierOk) return; // low-end auto-off
            try
            {
                BgVideo.MediaEnded += delegate { try { BgVideo.Position = TimeSpan.Zero; BgVideo.Play(); } catch { } };
                BgVideo.Source = new Uri(path);
                BgVideo.Visibility = Visibility.Visible;
                BgVideo.Play();
            }
            catch { BgVideo.Visibility = Visibility.Collapsed; }
        }

        // ============================================================ background music
        // Audio only (no MediaElement in the visual tree), so it is cheap over the layered window.
        // Default volume is low; the mute state + volume persist in launcher.ini [Music].

        private void InitMusic()
        {
            Theme t = _theme;
            _musicEnabled = t.Bool("Music", "Enabled", false);
            _musicPauseOnMin = t.Bool("Music", "PauseWhenMinimized", false);
            string file = t.FilePath("Music", "File", "");
            // effective volume/mute: the player's own saved choice (launcher.ini) wins over the theme default
            int themeVol = (int)Math.Round(t.Num("Music", "Volume", 30));
            _musicVolume = Clamp01((_cfg.MusicUserVolume >= 0 ? _cfg.MusicUserVolume : themeVol) / 100.0);
            bool themeMuted = t.Bool("Music", "StartMuted", false);
            _musicMuted = _cfg.MusicUserMuted >= 0 ? _cfg.MusicUserMuted == 1 : themeMuted;

            bool playable = _musicEnabled && file != null;
            MuteBtn.Visibility = playable ? Visibility.Visible : Visibility.Collapsed;
            MusicRow.IsEnabled = playable;
            MusicLabel.Opacity = playable ? 1 : 0.45;
            if (!playable) { UpdateMuteButton(); return; }

            try
            {
                _music = new System.Windows.Media.MediaPlayer();
                _music.MediaEnded += delegate { try { _music.Position = TimeSpan.Zero; _music.Play(); } catch { } };
                _music.Open(new Uri(file));
                _music.Volume = _musicMuted ? 0 : _musicVolume;
                if (!_musicMuted) _music.Play();
            }
            catch { _music = null; MuteBtn.Visibility = Visibility.Collapsed; }
            UpdateMuteButton();
        }

        private static double Clamp01(double v) { return v < 0 ? 0 : (v > 1 ? 1 : v); }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            SetMuted(!_musicMuted);
            _cfg.SaveMusicMuted(_musicMuted);
            if (MusicMuteBox != null) { _musicLoading = true; MusicMuteBox.IsChecked = _musicMuted; _musicLoading = false; }
        }

        private void SetMuted(bool muted)
        {
            _musicMuted = muted;
            if (_music != null)
            {
                _music.Volume = muted ? 0 : _musicVolume;
                try { if (muted) _music.Pause(); else _music.Play(); } catch { }
            }
            UpdateMuteButton();
        }

        private void UpdateMuteButton()
        {
            if (MuteBtn == null) return;
            MuteBtn.Content = _musicMuted ? "" : ""; // Segoe MDL2: Mute / Volume
            MuteBtn.ToolTip = _musicMuted ? Lang.T("Unmute") : Lang.T("Mute");
        }

        private void MusicPause() { try { if (_music != null) _music.Pause(); } catch { } }

        private void MusicResumeIfOn() { try { if (_music != null && _musicEnabled && !_musicMuted) _music.Play(); } catch { } }

        /// <summary>Settings volume slider moved: preview live and remember the value.</summary>
        private void MusicVolume_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MusicValue != null) MusicValue.Text = ((int)Math.Round(MusicSlider.Value)) + "%";
            if (_musicLoading) return;
            _musicVolume = Clamp01(MusicSlider.Value / 100.0);
            if (_music != null && !_musicMuted) { _music.Volume = _musicVolume; try { _music.Play(); } catch { } }
        }

        private void MusicMuteBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_musicLoading) return;
            SetMuted(MusicMuteBox.IsChecked == true);
            _cfg.SaveMusicMuted(_musicMuted);
        }

        // ============================================================ embers
        // At most 15 soft dots (shared frozen radial brushes, no blur), moved by one 20 fps timer that only
        // runs while the window is active and not minimized. They live inside the frame, below the card.

        private class Ember
        {
            public Ellipse Shape;
            public double X, Y, Vx, Vy, Life, MaxLife, Size;
        }

        private int _emberTarget = 12;
        private Color _emberColor = Color.FromRgb(0xFF, 0xA0, 0x40);
        private readonly List<Ember> _embers = new List<Ember>();
        private Brush[] _emberBrushes;
        private DispatcherTimer _emberTimer;

        private void BuildEmberBrushes()
        {
            _emberBrushes = new Brush[3];
            for (int i = 0; i < 3; i++)
            {
                Color core = Color.FromArgb(255, _emberColor.R, (byte)Math.Max(0, _emberColor.G - i * 30), _emberColor.B);
                RadialGradientBrush b = new RadialGradientBrush();
                b.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 240, 200), 0));
                b.GradientStops.Add(new GradientStop(Color.FromArgb(220, core.R, core.G, core.B), 0.25));
                b.GradientStops.Add(new GradientStop(Color.FromArgb(0, core.R, core.G, core.B), 1));
                b.Freeze();
                _emberBrushes[i] = b;
            }
        }

        private void StartEmbers()
        {
            EmberCanvas.Children.Clear();
            _embers.Clear();
            for (int i = 0; i < _emberTarget; i++)
            {
                Ember em = new Ember();
                em.Shape = new Ellipse { IsHitTestVisible = false };
                Respawn(em, true);
                _embers.Add(em);
                EmberCanvas.Children.Add(em.Shape);
            }
            if (_emberTimer == null)
            {
                _emberTimer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(1000.0 / 20) };
                _emberTimer.Tick += delegate { StepEmbers(1.0 / 20); };
            }
            if (_emberTarget > 0 && (IsActive || App.EmbersAlways) && WindowState != WindowState.Minimized) _emberTimer.Start();
            else _emberTimer.Stop();
        }

        private void PauseEmbers()
        {
            if (App.EmbersAlways && IsVisible && WindowState != WindowState.Minimized) return;
            if (_emberTimer != null) _emberTimer.Stop();
        }

        private void ResumeEmbers()
        {
            if (_emberTimer != null && _emberTarget > 0 && WindowState != WindowState.Minimized) _emberTimer.Start();
        }

        private void Respawn(Ember em, bool anywhere)
        {
            em.Size = 4 + _rnd.NextDouble() * 6;
            em.X = _rnd.NextDouble() * 880;
            em.Y = anywhere ? 260 + _rnd.NextDouble() * 230 : 488 + _rnd.NextDouble() * 10;
            em.Vx = _rnd.NextDouble() * 16 - 8;
            em.Vy = -(12 + _rnd.NextDouble() * 20);
            em.MaxLife = 5 + _rnd.NextDouble() * 6;
            em.Life = anywhere ? _rnd.NextDouble() * em.MaxLife : 0;
            em.Shape.Width = em.Size;
            em.Shape.Height = em.Size;
            em.Shape.Fill = _emberBrushes[_rnd.Next(_emberBrushes.Length)];
            Place(em);
        }

        private void StepEmbers(double dt)
        {
            foreach (Ember em in _embers)
            {
                em.Life += dt;
                em.X += em.Vx * dt + Math.Sin(em.Life * 1.7 + em.Size) * 0.25;
                em.Y += em.Vy * dt;
                if (em.Life >= em.MaxLife || em.Y < 150) { Respawn(em, false); continue; }
                Place(em);
            }
        }

        private static void Place(Ember em)
        {
            Canvas.SetLeft(em.Shape, em.X);
            Canvas.SetTop(em.Shape, em.Y);
            double k = em.Life / em.MaxLife;
            em.Shape.Opacity = k < 0.15 ? k / 0.15 : (1 - k) / 0.85 * 0.9 + 0.1 * (1 - k);
        }
    }
}
