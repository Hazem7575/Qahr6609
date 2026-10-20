// Boot screen of the console (same look as Qahr's BootScreen): instead of ~100 lines scrolling by while the server
// loads, a loading bar is redrawn in place until the data is loaded, then a short summary. No line is lost: every
// line written during the boot goes to boot_log.txt, and warnings/errors are printed above the bar right away.
// While it is active the ConsoleQueue hands it every console line (nothing else draws), so colours never mix.
// Console redirected to a file, or shell.ini [Console] BootScreen=0 => the old scrolling start-up, unchanged.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conquer6609
{
    public static class BootScreen
    {
        private static readonly object sync = new object();
        private static volatile bool active;
        private static TextWriter realOut;
        private static readonly StringBuilder bootLog = new StringBuilder();
        private static readonly StringBuilder plainLine = new StringBuilder();
        private static readonly List<string> highlights = new List<string>();
        private static readonly System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
        private static int done, total = 85;
        private static string current = "";
        private static long lastDraw;
        private static int warnings;
        private static long stepStartMs;
        private const string StepsFile = "boot_steps.txt";

        public static bool Active { get { return active; } }
        public static string Slowest = "";

        public static void Begin(string title, string subtitle)
        {
            try
            {
                if (Console.IsOutputRedirected || !Framework.Settings.Bool("Console", "BootScreen", true))
                    return;
                // number of steps of the last boot, so the percentage is right
                try
                {
                    int n;
                    if (File.Exists(StepsFile) && int.TryParse(File.ReadAllText(StepsFile).Trim(), out n) && n > 10)
                        total = n;
                }
                catch { }
                ConsoleQueue.Exclusive(o =>
                {
                    realOut = o;
                    clock.Start();
                    try { Console.CursorVisible = false; } catch { }
                    DrawHeader(title, subtitle);
                    active = true;
                    Draw(true);
                });
            }
            catch
            {
                active = false;
            }
        }

        public static void StepStart(string name)
        {
            if (!active) return;
            lock (sync) { current = name; stepStartMs = clock.ElapsedMilliseconds; }
            Draw(false);
        }

        public static void StepDone()
        {
            if (!active) return;
            lock (sync)
            {
                if (done < total - 1) done++;
                long ms = clock.ElapsedMilliseconds - stepStartMs;
                if (ms >= 250) bootLog.Append(MyConsole.TimeStamp()).Append(" [Boot] slow step: ").Append(current).Append(' ').Append(ms).AppendLine(" ms");
            }
            Draw(false);
        }

        /// <summary>A MyConsole line during the boot (from ConsoleQueue).</summary>
        public static void OnLine(string text, ConsoleColor color)
        {
            text = (text ?? "").TrimEnd('\r', '\n');
            lock (sync)
            {
                bootLog.Append(MyConsole.TimeStamp()).Append(' ').AppendLine(text);
                if (IsSummaryLine(text)) highlights.Add(text);
                // red = always shown; other colours only when the text reads like a problem (modules print normal lines in yellow)
                bool important = color == ConsoleColor.Red || color == ConsoleColor.DarkRed || LooksLikeProblem(text);
                if (important)
                    PrintAbove(text, color == ConsoleColor.Red || color == ConsoleColor.DarkRed || LooksLikeError(text) ? ConsoleColor.Red : ConsoleColor.Yellow);
            }
        }

        /// <summary>Plain Console.Write text during the boot (from ConsoleQueue), collected line by line.</summary>
        public static void OnPlain(string text)
        {
            lock (sync)
            {
                foreach (char c in text)
                {
                    if (c == '\n')
                    {
                        string t = plainLine.ToString().TrimEnd('\r');
                        plainLine.Clear();
                        if (t.Trim().Length != 0) OnLine(t, ConsoleColor.Gray);
                    }
                    else plainLine.Append(c);
                }
            }
        }

        /// <summary>End of the boot: the summary, then the console goes back to the normal tagged lines.</summary>
        public static void Finish(string readyText, IEnumerable<KeyValuePair<string, string>> rows)
        {
            if (!active) return;
            ConsoleQueue.Exclusive(o =>
            {
                lock (sync)
                {
                    active = false;
                    try { File.WriteAllText(StepsFile, Math.Max(done + 1, 11).ToString()); } catch { }
                    done = total;
                    current = "";
                    ClearLine();
                    Bar(o, 1.0);
                    Console.ForegroundColor = ConsoleColor.Green;
                    o.Write("  " + (clock.ElapsedMilliseconds / 1000.0).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "s");
                    o.WriteLine();
                    o.WriteLine();
                    if (rows != null)
                        foreach (var r in rows)
                            Check(o, (r.Key + ":").PadRight(10) + r.Value);
                    foreach (var h in highlights)
                        Check(o, StripTag(h).Replace(" Slowest:", ""));
                    if (Slowest.Length > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        string slow = "     Slowest: " + Slowest;
                        int max = SafeWidth() - 2;
                        if (slow.Length > max) slow = slow.Substring(0, Math.Max(10, max - 3)) + "...";
                        o.WriteLine(slow);
                    }
                    if (warnings > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        o.WriteLine("   ! " + warnings + " warning(s)/error(s) while loading (shown above, full list in boot_log.txt)");
                    }
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    o.WriteLine("     Full boot log: boot_log.txt");
                    o.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    o.WriteLine("  ══════════════  " + readyText + "  ══════════════");
                    Console.ResetColor();
                    o.WriteLine();
                    try { File.WriteAllText("boot_log.txt", bootLog.ToString(), Encoding.UTF8); } catch { }
                    try { Console.CursorVisible = true; } catch { }
                }
            });
        }

        /// <summary>The boot failed with an exception: give the console back and print everything that was written.</summary>
        public static void Abort()
        {
            if (!active) return;
            ConsoleQueue.Exclusive(o =>
            {
                lock (sync)
                {
                    active = false;
                    ClearLine();
                    try { Console.CursorVisible = true; } catch { }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    o.Write(bootLog.ToString());
                    Console.ResetColor();
                    try { File.WriteAllText("boot_log.txt", bootLog.ToString(), Encoding.UTF8); } catch { }
                }
            });
        }

        private static void Check(TextWriter o, string text)
        {
            Console.ForegroundColor = ConsoleColor.Green; o.Write("   √ ");
            Console.ForegroundColor = ConsoleColor.Gray; o.WriteLine(text.Trim());
        }

        private static string StripTag(string t)
        {
            t = t.Trim();
            if (t.StartsWith("["))
            {
                int end = t.IndexOf(']');
                if (end > 1 && end < 20) t = t.Substring(end + 1).Trim();
            }
            return t;
        }

        private static bool IsSummaryLine(string t)
        {
            return t.StartsWith("[Storage] Player backend") || t.StartsWith("Database loaded in") || t.StartsWith("Loaded ") && t.Contains(" maps")
                   || t.StartsWith("[Modules]") && t.Contains("Loaded ") || t.StartsWith("Listening on port");
        }

        private static bool LooksLikeError(string t)
        {
            return t.IndexOf("exception", StringComparison.OrdinalIgnoreCase) >= 0 || t.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0
                   || t.IndexOf("failed", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool LooksLikeProblem(string t)
        {
            string n = t.Replace("Warnings=0", "").Replace(" 0 warning", "");
            return LooksLikeError(t) || n.IndexOf("warn", StringComparison.OrdinalIgnoreCase) >= 0
                   || t.IndexOf("missing", StringComparison.OrdinalIgnoreCase) >= 0 || t.IndexOf("not found", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void DrawHeader(string title, string subtitle)
        {
            var o = realOut;
            int w = 58;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            o.WriteLine();
            o.WriteLine("  ╔" + new string('═', w) + "╗");
            o.Write("  ║");
            Console.ForegroundColor = ConsoleColor.Cyan; o.Write(Center(title, w));
            Console.ForegroundColor = ConsoleColor.DarkCyan; o.WriteLine("║");
            o.Write("  ║");
            Console.ForegroundColor = ConsoleColor.Gray; o.Write(Center(subtitle, w));
            Console.ForegroundColor = ConsoleColor.DarkCyan; o.WriteLine("║");
            o.WriteLine("  ╚" + new string('═', w) + "╝");
            o.WriteLine();
            Console.ResetColor();
        }

        private static string Center(string s, int w)
        {
            if (s.Length >= w) return s.Substring(0, w);
            int left = (w - s.Length) / 2;
            return new string(' ', left) + s + new string(' ', w - s.Length - left);
        }

        private static void Draw(bool force)
        {
            if (!active) return;
            lock (sync)
            {
                long now = clock.ElapsedMilliseconds;
                if (!force && now - lastDraw < 30) return;// at most ~30 redraws a second
                lastDraw = now;
                try
                {
                    var o = realOut;
                    o.Write('\r');
                    double p = total == 0 ? 0 : (double)done / total;
                    Bar(o, p);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    string name = current;
                    int room = Math.Max(10, SafeWidth() - 50);
                    if (name.Length > room) name = name.Substring(0, room - 1) + "…";
                    o.Write("  " + name.PadRight(room));
                    Console.ResetColor();
                }
                catch { }
            }
        }

        private static void Bar(TextWriter o, double p)
        {
            const int width = 30;
            int fill = (int)Math.Round(Math.Min(1.0, p) * width);
            Console.ForegroundColor = ConsoleColor.White; o.Write("  Loading ");
            Console.ForegroundColor = ConsoleColor.Green; o.Write(new string('█', fill));
            Console.ForegroundColor = ConsoleColor.DarkGray; o.Write(new string('░', width - fill));
            Console.ForegroundColor = ConsoleColor.White; o.Write(" " + ((int)(Math.Min(1.0, p) * 100)).ToString().PadLeft(3) + "%");
        }

        private static void PrintAbove(string text, ConsoleColor color)
        {
            try
            {
                warnings++;
                ClearLine();
                Console.ForegroundColor = color;
                realOut.WriteLine("  " + (color == ConsoleColor.Red ? "x " : "! ") + StripTag(text));
                Console.ResetColor();
                lastDraw = 0;
                Draw(true);
            }
            catch { }
        }

        private static void ClearLine()
        {
            try { realOut.Write("\r" + new string(' ', SafeWidth() - 1) + "\r"); } catch { }
        }

        private static int SafeWidth()
        {
            try { int w = Console.WindowWidth; return w > 20 ? w : 80; } catch { return 80; }
        }
    }
}
