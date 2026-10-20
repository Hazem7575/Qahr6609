using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScriptHazemLauncher
{
    /// <summary>
    /// Round logo emblem drawn with WPF vector shapes only (no image files): spiked gold ring, dark disc,
    /// the server name in a gold outlined font and a small ribbon line below it. Built once per theme apply
    /// and cached as a bitmap (it never changes while the launcher runs).
    /// </summary>
    public static class Emblem
    {
        /// <param name="host">300 x 160 grid; the disc centre is at (150, 80).</param>
        public static void Build(Grid host, string name, string sub, ImageSource logoImage, FontFamily logoFont, FontFamily subFont,
                                 Color goldLight, Color gold, Color goldDark, Color discColor)
        {
            host.Children.Clear();
            Canvas c = new Canvas { Width = 300, Height = 160 };
            const double cx = 150, cy = 78;

            // soft warm glow behind (radial brush, no blur effect)
            Ellipse glow = new Ellipse { Width = 280, Height = 170, Opacity = 0.55 };
            RadialGradientBrush gb = new RadialGradientBrush();
            gb.GradientStops.Add(new GradientStop(Color.FromArgb(0xB0, 0xFF, 0x80, 0x30), 0));
            gb.GradientStops.Add(new GradientStop(Color.FromArgb(0x40, 0xC0, 0x30, 0x10), 0.5));
            gb.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, 0x80, 0x10, 0x00), 1));
            glow.Fill = gb;
            Place(c, glow, cx - 140, cy - 85);

            LinearGradientBrush goldV = Gold(goldLight, gold, goldDark, true);
            LinearGradientBrush goldD = Gold(goldLight, gold, goldDark, false);
            SolidColorBrush ink = new SolidColorBrush(Color.FromRgb(0x2A, 0x10, 0x04));

            // spiked sun ring
            Path spikes = new Path { Data = Star(cx, cy, 24, 65, 73, 0.5), Fill = goldD, Stroke = ink, StrokeThickness = 0.8 };
            c.Children.Add(spikes);
            Path spikes2 = new Path { Data = Star(cx, cy, 24, 62, 68, 0), Fill = new SolidColorBrush(Darker(goldDark, 0.25)), Stroke = ink, StrokeThickness = 0.6 };
            c.Children.Add(spikes2);

            // disc
            Ellipse disc = new Ellipse { Width = 126, Height = 126, StrokeThickness = 3.5, Stroke = goldV };
            RadialGradientBrush db = new RadialGradientBrush { GradientOrigin = new Point(0.5, 0.35) };
            db.GradientStops.Add(new GradientStop(Lighter(discColor, 0.18), 0));
            db.GradientStops.Add(new GradientStop(discColor, 0.6));
            db.GradientStops.Add(new GradientStop(Darker(discColor, 0.7), 1));
            disc.Fill = db;
            Place(c, disc, cx - 63, cy - 63);
            Ellipse rim = new Ellipse { Width = 126, Height = 126, StrokeThickness = 1, Stroke = ink };
            Place(c, rim, cx - 63, cy - 63);
            // tick ring + inner rings
            Ellipse ticks = new Ellipse { Width = 112, Height = 112, StrokeThickness = 3, Stroke = goldD, StrokeDashArray = new DoubleCollection { 0.35, 1.6 }, Opacity = 0.85 };
            Place(c, ticks, cx - 56, cy - 56);
            Ellipse inner = new Ellipse { Width = 100, Height = 100, StrokeThickness = 0.9, Stroke = new SolidColorBrush(Color.FromArgb(0xC0, gold.R, gold.G, gold.B)) };
            Place(c, inner, cx - 50, cy - 50);
            Ellipse inner2 = new Ellipse { Width = 86, Height = 86, StrokeThickness = 0.6, Stroke = new SolidColorBrush(Color.FromArgb(0x70, gold.R, gold.G, gold.B)) };
            Place(c, inner2, cx - 43, cy - 43);
            // four diamonds on the rim
            for (int i = 0; i < 4; i++)
            {
                double a = i * Math.PI / 2;
                double x = cx + Math.Sin(a) * 63, y = cy - Math.Cos(a) * 63;
                Path d = new Path { Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M{0},{1} L{2},{3} L{0},{4} L{5},{3} Z", x, y - 6, x + 5, y, y + 6, x - 5)), Fill = goldV, Stroke = ink, StrokeThickness = 0.8 };
                c.Children.Add(d);
            }
            // flame / lotus mark in the top half of the disc
            Path lotus = new Path
            {
                Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture,
                    "M{0},{1} C{2},{3} {4},{5} {0},{6} C{7},{5} {8},{3} {0},{1} Z M{9},{10} C{11},{12} {13},{14} {0},{6} C{15},{16} {17},{18} {9},{10} Z M{19},{10} C{20},{12} {21},{14} {0},{6} C{22},{16} {23},{18} {19},{10} Z",
                    cx, cy - 46, cx + 9, cy - 36, cx + 7, cy - 24, cy - 18, cx - 7, cx - 9,
                    cx - 22, cy - 34, cx - 20, cy - 26, cx - 12, cy - 20, cx - 9, cy - 24, cx - 16, cy - 30,
                    cx + 22, cx + 20, cx + 12, cx + 9, cx + 16)),
                Fill = goldD, Opacity = 0.9
            };
            c.Children.Add(lotus);

            // ribbon plate across the disc
            double rw = 236, rh = 40, rx = cx - rw / 2, ry = cy - 12;
            Path ribbon = new Path
            {
                Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture,
                    "M{0},{1} L{2},{1} L{3},{4} L{2},{5} L{0},{5} L{6},{4} Z", rx + 12, ry, rx + rw - 12, rx + rw, ry + rh / 2, ry + rh, rx)),
                Stroke = goldV, StrokeThickness = 1.6
            };
            LinearGradientBrush rb = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            rb.GradientStops.Add(new GradientStop(Lighter(discColor, 0.1), 0));
            rb.GradientStops.Add(new GradientStop(Darker(discColor, 0.75), 1));
            ribbon.Fill = rb;
            c.Children.Add(ribbon);
            Path ribbonLine = new Path
            {
                Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture,
                    "M{0},{1} L{2},{1} L{3},{4} L{2},{5} L{0},{5} L{6},{4} Z", rx + 15, ry + 3, rx + rw - 15, rx + rw - 5, ry + rh / 2, ry + rh - 3, rx + 5)),
                Stroke = new SolidColorBrush(Color.FromArgb(0x90, gold.R, gold.G, gold.B)), StrokeThickness = 0.7
            };
            c.Children.Add(ribbonLine);

            // name: image from the theme, or outlined gold text
            if (logoImage != null)
            {
                Image img = new Image { Source = logoImage, Height = 46, Width = 220, Stretch = Stretch.Uniform };
                Place(c, img, cx - 110, ry - 8);
            }
            else
            {
                string text = string.IsNullOrWhiteSpace(name) ? "HAZEM" : name.Trim();
                double size = 40;
                FormattedText ft = Fmt(text, logoFont, size, FontWeights.Bold);
                while (ft.Width > 250 && size > 18) { size -= 2; ft = Fmt(text, logoFont, size, FontWeights.Bold); }
                double tx = cx - ft.Width / 2, ty = ry + rh / 2 - ft.Height / 2 - 2;
                Geometry g = ft.BuildGeometry(new Point(tx, ty));
                Path shadow = new Path { Data = g, Fill = new SolidColorBrush(Color.FromArgb(0xC0, 0, 0, 0)), RenderTransform = new TranslateTransform(0, 2.2) };
                c.Children.Add(shadow);
                Path outline = new Path { Data = g, Stroke = ink, StrokeThickness = 3.2, StrokeLineJoin = PenLineJoin.Round };
                c.Children.Add(outline);
                LinearGradientBrush tb = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
                tb.GradientStops.Add(new GradientStop(Lighter(goldLight, 0.5), 0));
                tb.GradientStops.Add(new GradientStop(goldLight, 0.3));
                tb.GradientStops.Add(new GradientStop(gold, 0.52));
                tb.GradientStops.Add(new GradientStop(goldDark, 0.56));
                tb.GradientStops.Add(new GradientStop(gold, 0.85));
                tb.GradientStops.Add(new GradientStop(goldLight, 1));
                Path fill = new Path { Data = g, Fill = tb, Stroke = new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0xF4, 0xD0)), StrokeThickness = 0.4 };
                c.Children.Add(fill);
            }

            // sub line on a small dark tab under the ribbon
            if (!string.IsNullOrWhiteSpace(sub))
            {
                string spaced = Spaced(sub.Trim().ToUpperInvariant());
                FormattedText st = Fmt(spaced, subFont, 9, FontWeights.Bold);
                double sw = st.Width + 26, sx = cx - sw / 2, sy = ry + rh + 2;
                Path tab = new Path
                {
                    Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M{0},{1} L{2},{1} L{3},{4} L{5},{4} Z", sx, sy, sx + sw, sx + sw - 8, sy + 15, sx + 8)),
                    Fill = new SolidColorBrush(Darker(discColor, 0.6)), Stroke = goldV, StrokeThickness = 1
                };
                c.Children.Add(tab);
                Path stp = new Path { Data = st.BuildGeometry(new Point(cx - st.Width / 2, sy + 7.5 - st.Height / 2)), Fill = goldV };
                c.Children.Add(stp);
            }

            host.Children.Add(c);
            host.CacheMode = new BitmapCache { RenderAtScale = 1.5, SnapsToDevicePixels = false };
        }

        /// <summary>
        /// Image logo (theme [Logo] Image, e.g. the official Conquer logo with its glow baked in) floating above the
        /// frame, with a small gold ribbon carrying the server name under it. Host is 360 x 200, centre x = 180.
        /// </summary>
        public static void BuildImage(Grid host, ImageSource logo, double width, double offsetY, string ribbon, FontFamily font,
                                      Color goldLight, Color gold, Color goldDark, Color plateColor)
        {
            host.Children.Clear();
            Canvas c = new Canvas { Width = 360, Height = 200 };
            const double cx = 180;
            double w = Math.Max(80, Math.Min(360, width));
            double h = logo.Width > 0 ? w * logo.Height / logo.Width : w * 0.7;
            Image img = new Image { Source = logo, Width = w, Height = h, Stretch = Stretch.Uniform };
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
            double top = -10 + offsetY;
            Place(c, img, cx - w / 2, top);

            if (!string.IsNullOrWhiteSpace(ribbon))
            {
                LinearGradientBrush goldV = Gold(goldLight, gold, goldDark, true);
                string spaced = Spaced(ribbon.Trim().ToUpperInvariant());
                FormattedText ft = Fmt(spaced, font, 10.5, FontWeights.Bold);
                double rw = ft.Width + 46, rh = 19;
                double rx = cx - rw / 2, ry = top + h * 0.80 - rh / 2;
                if (ry > 162) ry = 162;
                // swallow-tail ribbon
                Path tail = new Path
                {
                    Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture,
                        "M{0},{1} L{2},{1} L{3},{4} L{2},{5} L{0},{5} L{6},{4} Z", rx - 12, ry + 4, rx + rw + 12, rx + rw + 5, ry + rh / 2 + 4, ry + rh + 4, rx - 5)),
                    Fill = new SolidColorBrush(Theme.Darken(plateColor, 0.55)), Stroke = goldV, StrokeThickness = 0.9
                };
                c.Children.Add(tail);
                Path plate = new Path
                {
                    Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture,
                        "M{0},{1} L{2},{1} L{3},{4} L{2},{5} L{0},{5} L{6},{4} Z", rx + 6, ry, rx + rw - 6, rx + rw, ry + rh / 2, ry + rh, rx)),
                    Stroke = goldV, StrokeThickness = 1.3
                };
                LinearGradientBrush pb = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
                pb.GradientStops.Add(new GradientStop(Theme.Lighten(plateColor, 0.12), 0));
                pb.GradientStops.Add(new GradientStop(Theme.Darken(plateColor, 0.6), 1));
                plate.Fill = pb;
                c.Children.Add(plate);
                Path text = new Path { Data = ft.BuildGeometry(new Point(cx - ft.Width / 2, ry + rh / 2 - ft.Height / 2)), Fill = goldV };
                c.Children.Add(text);
            }
            host.Children.Add(c);
            host.CacheMode = new BitmapCache { RenderAtScale = 1.5, SnapsToDevicePixels = false };
        }

        private static string Spaced(string s)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i]);
                if (i < s.Length - 1) sb.Append(s[i] == ' ' ? "  " : " ");
            }
            return sb.ToString();
        }

        private static FormattedText Fmt(string text, FontFamily font, double size, FontWeight w)
        {
            return new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                new Typeface(font, FontStyles.Normal, w, FontStretches.Normal), size, Brushes.Black, 1.0);
        }

        private static void Place(Canvas c, UIElement e, double x, double y)
        {
            Canvas.SetLeft(e, x);
            Canvas.SetTop(e, y);
            c.Children.Add(e);
        }

        /// <summary>Star polygon with n points between radius r1 (inner) and r2 (outer).</summary>
        private static Geometry Star(double cx, double cy, int n, double r1, double r2, double phase)
        {
            StreamGeometry g = new StreamGeometry();
            using (StreamGeometryContext ctx = g.Open())
            {
                for (int i = 0; i < n * 2; i++)
                {
                    double a = (i + phase) * Math.PI / n;
                    double r = i % 2 == 0 ? r2 : r1;
                    Point p = new Point(cx + Math.Sin(a) * r, cy - Math.Cos(a) * r);
                    if (i == 0) ctx.BeginFigure(p, true, true); else ctx.LineTo(p, true, false);
                }
            }
            g.Freeze();
            return g;
        }

        private static LinearGradientBrush Gold(Color light, Color mid, Color dark, bool vertical)
        {
            LinearGradientBrush b = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = vertical ? new Point(0, 1) : new Point(1, 1) };
            b.GradientStops.Add(new GradientStop(light, 0));
            b.GradientStops.Add(new GradientStop(mid, 0.4));
            b.GradientStops.Add(new GradientStop(dark, 0.7));
            b.GradientStops.Add(new GradientStop(light, 1));
            b.Freeze();
            return b;
        }

        private static Color Lighter(Color c, double k) { return Theme.Lighten(c, k); }
        private static Color Darker(Color c, double k) { return Theme.Darken(c, k); }
    }
}
