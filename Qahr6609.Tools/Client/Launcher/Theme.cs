using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScriptHazemLauncher
{
    /// <summary>
    /// Everything that can be customised without touching code lives in
    /// themes\&lt;name&gt;\theme.ini (+ images\ and fonts\ next to it).
    /// See themes\README_THEME.txt for every key.
    /// </summary>
    public class Theme
    {
        public string Dir { get; private set; }
        private IniText _ini;

        public static Theme Load(string baseDir, string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName)) themeName = "default";
            string dir = Path.Combine(baseDir, "themes", themeName);
            if (!Directory.Exists(dir)) dir = Path.Combine(baseDir, "themes", "default");
            Theme t = new Theme();
            t.Dir = dir;
            t._ini = new IniText(Path.Combine(dir, "theme.ini"));
            return t;
        }

        // ------------------------------------------------------------ raw getters

        public string Str(string section, string key, string def)
        {
            return _ini.Get(section, key, def);
        }

        /// <summary>
        /// Language-aware text: English uses "Key_en", Arabic uses "Key_ar" or plain "Key".
        /// Falls back to the built-in defaults when the theme does not define the key.
        /// </summary>
        public string Text(string section, string key, string arDefault, string enDefault)
        {
            if (Lang.IsArabic)
            {
                string v = _ini.Get(section, key + "_ar", null) ?? _ini.Get(section, key, null);
                return v ?? arDefault;
            }
            return _ini.Get(section, key + "_en", null) ?? enDefault;
        }

        public double Num(string section, string key, double def)
        {
            double v;
            string s = _ini.Get(section, key, "");
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : def;
        }

        public bool Bool(string section, string key, bool def)
        {
            string s = _ini.Get(section, key, def ? "1" : "0").Trim().ToLowerInvariant();
            return s == "1" || s == "true" || s == "yes" || s == "on";
        }

        public Color Col(string section, string key, string def)
        {
            return ParseColor(_ini.Get(section, key, def), ParseColor(def, Colors.White));
        }

        public static Color ParseColor(string s, Color fallback)
        {
            try
            {
                object o = ColorConverter.ConvertFromString(s.Trim());
                return o is Color ? (Color)o : fallback;
            }
            catch { return fallback; }
        }

        /// <summary>Full path of a theme file (relative to the theme folder), or null when missing/empty.</summary>
        public string FilePath(string section, string key, string def)
        {
            string rel = _ini.Get(section, key, def);
            if (string.IsNullOrWhiteSpace(rel)) return null;
            string full = Path.IsPathRooted(rel) ? rel : Path.GetFullPath(Path.Combine(Dir, rel));
            return File.Exists(full) ? full : null;
        }

        /// <summary>Loads an image without locking the file (so it can be replaced while the launcher runs).</summary>
        public ImageSource Image(string section, string key, string def)
        {
            string path = FilePath(section, key, def);
            return path == null ? null : LoadImage(path, 0);
        }

        public static BitmapImage LoadImage(string path, int decodeWidth)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(data);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                if (decodeWidth > 0) bmp.DecodePixelWidth = decodeWidth;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch { return null; }
        }

        /// <summary>Font from the theme fonts folder: "fonts/File.ttf#Family Name", or an installed font name.</summary>
        public FontFamily Font(string section, string key, string def)
        {
            string spec = _ini.Get(section, key, def);
            if (string.IsNullOrWhiteSpace(spec)) spec = def;
            int hash = spec.IndexOf('#');
            if (hash > 0)
            {
                string file = spec.Substring(0, hash).Trim();
                string family = spec.Substring(hash + 1).Trim();
                string full = Path.GetFullPath(Path.Combine(Dir, file));
                if (File.Exists(full))
                {
                    string folder = Path.GetDirectoryName(full).Replace('\\', '/') + "/";
                    return new FontFamily(new Uri("file:///" + folder), "./#" + family + ", Segoe UI");
                }
                return new FontFamily(family + ", Segoe UI");
            }
            return new FontFamily(spec + ", Segoe UI");
        }

        // ------------------------------------------------------------ apply colours / fonts

        /// <summary>Pushes colours and fonts into Application resources (used through DynamicResource).</summary>
        public void ApplyResources(ResourceDictionary res)
        {
            Color goldLight = Col("Colors", "GoldLight", "#FFFFE7A8");
            Color gold = Col("Colors", "Gold", "#FFE8B64E");
            Color goldDark = Col("Colors", "GoldDark", "#FF8A5512");

            LinearGradientBrush goldBrush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            goldBrush.GradientStops.Add(new GradientStop(goldLight, 0));
            goldBrush.GradientStops.Add(new GradientStop(gold, 0.45));
            goldBrush.GradientStops.Add(new GradientStop(goldDark, 0.75));
            goldBrush.GradientStops.Add(new GradientStop(goldLight, 1));
            goldBrush.Freeze();
            res["GoldBrush"] = goldBrush;

            LinearGradientBrush frame = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
            frame.GradientStops.Add(new GradientStop(goldLight, 0));
            frame.GradientStops.Add(new GradientStop(goldDark, 0.35));
            frame.GradientStops.Add(new GradientStop(gold, 0.65));
            frame.GradientStops.Add(new GradientStop(goldDark, 1));
            frame.Freeze();
            res["GoldFrameBrush"] = frame;

            res["GoldText"] = Solid(gold);
            res["ThinGold"] = Solid(Color.FromArgb(0x70, gold.R, gold.G, gold.B));
            res["SoftText"] = Solid(Col("Colors", "Text", "#FFE9DCC8"));
            res["DimText"] = Solid(Col("Colors", "DimText", "#FF9C8A78"));
            res["AccentBrush"] = Solid(Col("Colors", "Accent", "#FFF0A040"));
            res["AuthorBrush"] = Solid(Col("Colors", "Author", "#FFE0503A"));
            res["PanelBrush"] = Solid(Col("Colors", "Panel", "#D0120605"));
            res["BarTrackBrush"] = Solid(Col("Colors", "BarTrack", "#FF0C0403"));
            res["FileBarBrush"] = Bar(Col("Colors", "FileBar", "#FFD02A1E"));
            res["TotalBarBrush"] = Bar(Col("Colors", "TotalBar", "#FFF0A030"));
            res["PlayTextBrush"] = Solid(Col("Colors", "PlayText", "#FF3A1504"));

            res["OrnamentGold"] = Solid(Col("Colors", "Ornament", "#C0C9A25A"));

            // light news card (v2 layout)
            res["CardTitle"] = Solid(Col("Card", "Title", "#FF2E2420"));
            res["CardText"] = Solid(Col("Card", "Text", "#FF4A403A"));
            res["CardDim"] = Solid(Col("Card", "Dim", "#FF8C827A"));
            res["CardTag"] = Solid(Col("Card", "Tag", "#FFE0874A"));
            res["CardLink"] = Solid(Col("Card", "Link", "#FF3A302A"));
            res["CardAuthor"] = Solid(Col("Card", "Author", "#FFD3352B"));

            res["TitleFont"] = Font("Fonts", "Title", "fonts/CinzelDecorative-Bold.ttf#Cinzel Decorative");
            res["LogoFont"] = Font("Fonts", "Logo", "fonts/Cinzel-Bold.ttf#Cinzel");
            res["BodyFont"] = Font("Fonts", "Body", "fonts/Cairo-Regular.ttf#Cairo");
        }

        private static SolidColorBrush Solid(Color c)
        {
            SolidColorBrush b = new SolidColorBrush(c);
            b.Freeze();
            return b;
        }

        /// <summary>Fill for a progress bar: a horizontal deep-&gt;bright gradient (dark at the start, bright at the fill edge).</summary>
        private static LinearGradientBrush Bar(Color c)
        {
            LinearGradientBrush b = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
            b.GradientStops.Add(new GradientStop(Darken(c, 0.45), 0));
            b.GradientStops.Add(new GradientStop(Darken(c, 0.15), 0.5));
            b.GradientStops.Add(new GradientStop(Lighten(c, 0.28), 1));
            b.Freeze();
            return b;
        }

        public static Color Lighten(Color c, double k)
        {
            return Color.FromArgb(c.A, (byte)(c.R + (255 - c.R) * k), (byte)(c.G + (255 - c.G) * k), (byte)(c.B + (255 - c.B) * k));
        }

        public static Color Darken(Color c, double k)
        {
            return Color.FromArgb(c.A, (byte)(c.R * (1 - k)), (byte)(c.G * (1 - k)), (byte)(c.B * (1 - k)));
        }
    }
}
