using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.WindowsAPI;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PoP.Calculations;

namespace Conquer6609
{
    public struct ErrorCodes
    {
        public const int INVALID_HANDLE_VALUE = -1;
    }
    public class MyConsole
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        public static void DissableButton()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            if (Program.ServerConfig.IPAddres != "10.0.9.252")
            {
              //  ProcessConsoleEvent(0);

                Environment.Exit(0);
            }

        }

        private static IntPtr _InputHandle;
        private static IntPtr _OutputHandle;


        private static Extensions.Counter ExceptionsCounter = new Extensions.Counter(1);

        /// <summary> The handle Input handle is not recommended </summary>
        public static IntPtr InputHandle
        {
            get { return _InputHandle; }
            set
            {
                if (!Kernel32.SetStdHandle(STD_INPUT_HANDLE, value))
                    throw new Exception("Unable to set the new Input handle");
                _InputHandle = value;
            }
        }

        /// <summary> handle Output handle is not recommended </summary>
        public static IntPtr OutputHandle
        {
            get { return _OutputHandle; }
            set
            {
                if (!Kernel32.SetStdHandle(STD_OUTPUT_HANDLE, value))
                    throw new Exception("Unable to set the new Output handle");
                _OutputHandle = value;
            }
        }

        public static string Title
        {
            get
            {
                StringBuilder sb = new StringBuilder(32767);
                uint size = (uint)sb.Capacity;
                Kernel32.GetConsoleTitle(sb, size);
                sb.Capacity = sb.Length;
                return sb.ToString();
            }
            set { Kernel32.SetConsoleTitle(value); }
        }

        public const int STD_INPUT_HANDLE = -10;
        public const int STD_OUTPUT_HANDLE = -11;
        public const int STD_ERROR_HANDLE = -12; //Not being used yet

        static MyConsole()
        {
            InputHandle = Kernel32.GetStdHandle(STD_INPUT_HANDLE);
            OutputHandle = Kernel32.GetStdHandle(STD_OUTPUT_HANDLE);

            if (InputHandle.ToInt32() == ErrorCodes.INVALID_HANDLE_VALUE ||
                OutputHandle.ToInt32() == ErrorCodes.INVALID_HANDLE_VALUE)
            {
                throw new Exception("Unable to get the Console Handle");
            }
        }

        public static void Write(string value)
        {
            // Goes through Console.Out, which is the queued writer once ConsoleQueue is installed.
            if (value != null)
                System.Console.Write(value);
        }
        private static DateTime NOW = DateTime.Now;
        private static Time32 NOW32 = Time32.Now;
        public static string TimeStamp()
        {
            return "[" + NOW.AddMilliseconds((Time32.Now - NOW32).GetHashCode()).ToString("hh:mm:ss") + "]";
        }
        public static void WriteLine(string value, ConsoleColor color = ConsoleColor.White)
        {
            if (value == null)
                value = "";
            if (BootScreen.Active)
            {
                BootScreen.OnLine(value, color);// the boot screen logs it (and shows it when it is a warning)
                return;
            }
            if (ConsoleQueue.Installed)
            {
                // Only enqueues: the console write happens on the ConsoleQueue thread.
                ConsoleQueue.EnqueueColored(DateTime.Now.ToString("HH:mm:ss"), value + Environment.NewLine, color);
                return;
            }
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write(TimeStamp() + " ");
            System.Console.ForegroundColor = color;
            // string overload: user text containing '{' must not be treated as a format string
            System.Console.Write(value + Environment.NewLine);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true)]
        private static extern IntPtr GetStdHandleRaw(int nStdHandle);
        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040, ENABLE_EXTENDED_FLAGS = 0x0080;

        /// <summary>
        /// Turns off console QuickEdit: a mouse click in the console window would otherwise pause every
        /// thread that writes to the console until a key is pressed. Does nothing without a console.
        /// </summary>
        public static void DisableQuickEdit()
        {
            try
            {
                IntPtr h = GetStdHandleRaw(STD_INPUT_HANDLE);
                uint mode;
                if (h == IntPtr.Zero || h == new IntPtr(-1) || !GetConsoleMode(h, out mode))
                    return;
                mode &= ~ENABLE_QUICK_EDIT_MODE;
                mode |= ENABLE_EXTENDED_FLAGS;
                SetConsoleMode(h, mode);
            }
            catch { }
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
        ///<summary> Attach a console to a specified process, CAUTION: If the console will be closed the process will close! </summary>
        public static void AttachConsole(uint PID)
        {
            if (!Kernel32.AttachConsole(PID))
                throw new Exception("Unable to attach the console.");
        }

        /// <summary>
        /// Create a console for the current process, Caution: Closing the console will also close the process!
        /// </summary>
        public static void AllocConsole()
        {
            if (!Kernel32.AllocConsole())
                throw new Exception("Unable to allocate a console.");
            //System.Console.read
        }
        public static void WriteException(Exception e)
        {
            LogException(e);
        }
        public static void SaveException(Exception e)
        {
            LogException(e);
        }

        private sealed class ExceptionSite
        {
            public long LastLogged = long.MinValue;
            public int Suppressed;
        }
        private const int ExceptionThrottleMs = 10000;
        private static int exceptionCalls;
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, ExceptionSite> ExceptionSites =
            new System.Collections.Concurrent.ConcurrentDictionary<string, ExceptionSite>();
        private static readonly System.Diagnostics.Stopwatch ExceptionClock = System.Diagnostics.Stopwatch.StartNew();

        /// <summary>
        /// Prints and stores an exception without doing any I/O on the calling thread. The same exception
        /// (type + throwing method) is logged in full the first time, then at most once per 10 seconds
        /// with the number of suppressed repeats. File: Exceptions\yyyy-MM-dd.log (written by ConsoleQueue).
        /// </summary>
        private static void LogException(Exception e)
        {
            if (e == null)
                return;
            try
            {
                string key = ExceptionKey(e);
                // ConcurrentDictionary.Count locks the whole table: only look at it now and then
                if ((System.Threading.Interlocked.Increment(ref exceptionCalls) & 1023) == 0 && ExceptionSites.Count > 5000)
                    ExceptionSites.Clear();
                var site = ExceptionSites.GetOrAdd(key, k => new ExceptionSite());
                long now = ExceptionClock.ElapsedMilliseconds;
                int suppressed;
                lock (site)
                {
                    if (site.LastLogged != long.MinValue && now - site.LastLogged < ExceptionThrottleMs)
                    {
                        site.Suppressed++;
                        return;
                    }
                    suppressed = site.Suppressed;
                    site.Suppressed = 0;
                    site.LastLogged = now;
                }
                string text = e.ToString();
                string extra = suppressed > 0 ? " (+" + suppressed + " suppressed)" : "";
                WriteLine(text + extra);
                ConsoleQueue.EnqueueFile("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + extra + Environment.NewLine + text + Environment.NewLine + Environment.NewLine);
            }
            catch { }
        }

        private static string ExceptionKey(Exception e)
        {
            string where = null;
            try
            {
                var target = e.TargetSite;
                if (target != null)
                    where = (target.DeclaringType != null ? target.DeclaringType.FullName : "?") + "." + target.Name;
            }
            catch { }
            if (where == null)
            {
                string trace = null;
                try { trace = e.StackTrace; }
                catch { }
                if (!string.IsNullOrEmpty(trace))
                {
                    int nl = trace.IndexOf('\n');
                    where = (nl > 0 ? trace.Substring(0, nl) : trace).Trim();
                }
                else
                    where = "?";
            }
            return e.GetType().FullName + "@" + where;
        }
        public static string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            uint read = 0;

            Kernel32.ReadConsole(InputHandle, sb, 100, out read, IntPtr.Zero);
            if (read == 0)
                return "";
            return sb.ToString(0, (int)read - 2);
        }
       
        public unsafe static string  log1(string name,byte* Data, int leng)
        {
            byte[] packet = new byte[leng];
            fixed (byte* recv_ptr = packet)
            {
                Kernel32.memcpy(recv_ptr, Data, leng);
            }
           return log2(name, packet);
        }
        public unsafe static string log2(string name , byte[] Data)
        {
            try
            {

                if (Data == null) return "";
                ushort PacketLength = (ushort)Data.Length;
                if (ASCIIEncoding.ASCII.GetString(Data).Contains("TQServer") || ASCIIEncoding.ASCII.GetString(Data).Contains("TQClient"))
                    PacketLength += 8;
                if (PacketLength > Data.Length) PacketLength = (ushort)Data.Length;

                string DataStr = "";
                DataStr += name + " " +"Packet Length : " + PacketLength + ", PacketType: " + BitConverter.ToInt16(Data, 2) + Environment.NewLine;


                for (int i = 0; i < Math.Ceiling((double)PacketLength / 16); i++)
                {
                    int t = 16;
                    if (((i + 1) * 16) > PacketLength)
                        t = PacketLength - (i * 16);
                    for (int a = 0; a < t; a++)
                    {
                        DataStr += Data[i * 16 + a].ToString("X2") + " ";

                    }
                    if (t < 16)
                        for (int a = t; a < 16; a++)
                            DataStr += "   ";
                    DataStr += "     ;";

                    for (int a = 0; a < t; a++)//unde e clasa de sockete
                    {
                        DataStr += Convert.ToChar(Data[i * 16 + a]);
                    }
                    DataStr += Environment.NewLine;
                }
                DataStr.Replace(Convert.ToChar(0), '.');
                DataStr += Environment.NewLine;
                return DataStr;
            }
            catch (Exception Exc)
            {
                MyConsole.WriteException(Exc);
            }
            return "";
        }
        public unsafe static void PrintPacketAdvanced(byte* Data , int leng)
        {
        }
        public unsafe static void PrintPacketAdvanced(byte* Data)
        {
        }
        public static void SavePacket(byte[] Data)
        {
        }
        public static void PrintPacketAdvanced(byte[] Data)
        {
        }
    }

    /// <summary>
    /// Queued console output. Console.Out is replaced by a writer that only enqueues; one background thread
    /// writes to the real console (and appends exception text to Exceptions\yyyy-MM-dd.log). Game threads
    /// therefore never block on the console (slow console, QuickEdit selection, scrolling...).
    /// Bounded: when more than [Performance] ConsoleQueueMax lines are waiting, new lines are dropped and counted.
    /// </summary>
    /// <summary>Look of the server console: tag column per line, start banner, "server online" box and the
    /// live window title. Everything goes through ConsoleQueue, so the game threads never wait for the console.</summary>
    public static class ConsoleUi
    {
        public static readonly DateTime Started = DateTime.Now;

        /// <summary>UTF-8 output (box drawing) and a wide window. Call before ConsoleQueue.Install.</summary>
        public static void Prepare()
        {
            try { Console.OutputEncoding = Encoding.UTF8; } catch { }
            try
            {
                int w = Math.Min(Console.LargestWindowWidth, 132), h = Math.Min(Console.LargestWindowHeight, 42);
                if (w >= 100 && Console.BufferWidth < w)
                    Console.BufferWidth = w;
                if (w >= 100 && h >= 30)
                    Console.SetWindowSize(w, h);
            }
            catch { }
            try { Console.BackgroundColor = ConsoleColor.Black; Console.Clear(); } catch { }
        }

        static void Raw(string text, ConsoleColor color)
        {
            if (ConsoleQueue.Installed)
                ConsoleQueue.EnqueueColored("", text, color);
            else
            {
                Console.ForegroundColor = color;
                Console.Write(text);
                Console.ResetColor();
            }
        }

        public static void Banner(string serverName)
        {
            string[] logo =
            {
                @"   __  __        ____          __    __    ___    ___  ",
                @"  |  \/  | ___  / ___|  __ _  / /_  / /_  / _ \  / _ \ ",
                @"  | |\/| |/ _ \| |  _  / _` || '_ \| '_ \| | | || (_) |",
                @"  | |  | |  __/| |_| || (_| || (_) | (_) | |_| | \__, |",
                @"  |_|  |_|\___| \____| \__,_| \___/ \___/ \___/    /_/ ",
            };
            Raw(Environment.NewLine, ConsoleColor.Gray);
            ConsoleColor[] shades = { ConsoleColor.Yellow, ConsoleColor.Yellow, ConsoleColor.DarkYellow, ConsoleColor.DarkYellow, ConsoleColor.DarkRed };
            for (int i = 0; i < logo.Length; i++)
                Raw(logo[i] + Environment.NewLine, shades[i]);
            string build = "?";
            try { build = File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd HH:mm"); } catch { }
            Raw(Environment.NewLine + "  Conquer Online 6609 game server", ConsoleColor.White);
            Raw("   \u00b7   " + (string.IsNullOrEmpty(serverName) ? "Hazem" : serverName), ConsoleColor.Cyan);
            Raw("   \u00b7   build " + build + "   \u00b7   .NET " + Environment.Version + (IntPtr.Size == 4 ? " x86" : " x64") + Environment.NewLine, ConsoleColor.DarkGray);
            Raw("  " + new string('\u2500', 76) + Environment.NewLine + Environment.NewLine, ConsoleColor.DarkGray);
        }

        /// <summary>The framed "server online" summary printed when the start-up is complete.</summary>
        public static void OnlineBox(params KeyValuePair<string, string>[] rows)
        {
            const int inner = 74;
            string title = " \u2714 SERVER ONLINE  \u00b7  Server Done Full Loaded ";
            Raw(Environment.NewLine + "  \u250c\u2500" + title + new string('\u2500', Math.Max(0, inner - title.Length - 1)) + "\u2510" + Environment.NewLine, ConsoleColor.Green);
            for (int i = 0; i < rows.Length; i += 2)
            {
                string left = Cell(rows[i]), right = i + 1 < rows.Length ? Cell(rows[i + 1]) : "";
                string line = (" " + left.PadRight(36) + right).PadRight(inner);
                Raw("  \u2502", ConsoleColor.Green);
                Raw(line.Substring(0, Math.Min(line.Length, inner)), ConsoleColor.White);
                Raw("\u2502" + Environment.NewLine, ConsoleColor.Green);
            }
            Raw("  \u2514" + new string('\u2500', inner) + "\u2518" + Environment.NewLine + Environment.NewLine, ConsoleColor.Green);
        }

        static string Cell(KeyValuePair<string, string> kv)
        {
            return (kv.Key + ":").PadRight(10) + kv.Value;
        }

        public static KeyValuePair<string, string> Row(string key, object value)
        {
            return new KeyValuePair<string, string>(key, value == null ? "" : value.ToString());
        }

        /// <summary>Window title, refreshed by KernelThread.</summary>
        public static string Title(string serverName, int online, int maxOnline)
        {
            TimeSpan up = DateTime.Now - Started;
            long mb = 0;
            try { mb = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024); } catch { }
            return (string.IsNullOrEmpty(serverName) ? "Hazem" : serverName) + " 6609  \u2502  Online " + online + " (max " + maxOnline + ")  \u2502  Up "
                + (int)up.TotalHours + ":" + up.Minutes.ToString("00") + ":" + up.Seconds.ToString("00") + "  \u2502  RAM " + mb + " MB";
        }

        /// <summary>Tag column of a console line: a leading "[Name] " becomes the tag, otherwise it is guessed
        /// from the text. Returns the 8-char tag; the text keeps everything after a leading "[Name] ".</summary>
        public static string Classify(ref string text, ConsoleColor requested, out ConsoleColor tagColor, out ConsoleColor textColor)
        {
            string tag = null;
            string t = text ?? "";
            string trimmed = t.TrimStart();
            if (trimmed.StartsWith("[") )
            {
                int end = trimmed.IndexOf(']');
                if (end > 1 && end < 20)
                {
                    tag = trimmed.Substring(1, end - 1).Trim().ToUpperInvariant();
                    text = trimmed.Substring(end + 1).TrimStart();
                    t = text;
                }
            }
            string lower = t.ToLowerInvariant();
            if (tag == null)
            {
                if (lower.Contains("exception") || lower.Contains("error") || lower.Contains("failed") || lower.Contains("not found"))
                    tag = "ERROR";
                else if (lower.StartsWith("loading") || lower.StartsWith("loaded") || lower.StartsWith("loaging") || lower.Contains(" was loading") || lower.StartsWith("database loaded"))
                    tag = "LOAD";
                else if (lower.StartsWith("listening"))
                    tag = "NET";
                else if (lower.Contains("warning") || lower.Contains("skipped") || lower.Contains("mismatch"))
                    tag = "WARN";
                else if (requested == ConsoleColor.Red)
                    tag = "ALERT";
                else
                    tag = "INFO";
            }
            switch (tag)
            {
                case "ERROR": tagColor = ConsoleColor.Red; textColor = ConsoleColor.Red; break;
                case "ALERT": tagColor = ConsoleColor.Red; textColor = ConsoleColor.White; break;
                case "WARN": tagColor = ConsoleColor.Yellow; textColor = ConsoleColor.Yellow; break;
                case "LOAD": tagColor = ConsoleColor.Cyan; textColor = ConsoleColor.Gray; break;
                case "NET": tagColor = ConsoleColor.Blue; textColor = ConsoleColor.White; break;
                case "SECURITY": tagColor = ConsoleColor.Magenta; textColor = ConsoleColor.White; break;
                case "MODULES": tagColor = ConsoleColor.DarkCyan; textColor = ConsoleColor.White; break;
                case "POKER": tagColor = ConsoleColor.Green; textColor = ConsoleColor.Gray; break;
                case "RAMADAN": tagColor = ConsoleColor.DarkYellow; textColor = ConsoleColor.White; break;
                case "QUESTENGINE": tagColor = ConsoleColor.DarkGreen; textColor = ConsoleColor.Gray; break;
                case "INFO": tagColor = ConsoleColor.DarkGray; textColor = requested == ConsoleColor.Red ? ConsoleColor.White : requested; break;
                default: tagColor = ConsoleColor.DarkCyan; textColor = ConsoleColor.Gray; break;
            }
            if (tag.Length > 8)
                tag = tag.Substring(0, 8);
            return tag.PadRight(8);
        }
    }

    public static class ConsoleQueue
    {
        private sealed class Item
        {
            public int Kind;            // 0 = plain text, 1 = colored MyConsole line, 2 = exception log file text
            public ConsoleColor Color;
            public string Prefix;
            public string Text;
        }

        private sealed class QueueWriter : TextWriter
        {
            private readonly Encoding encoding;
            public QueueWriter(Encoding encoding) { this.encoding = encoding ?? global::Conquer6609.AnsiText.Value; }
            public override Encoding Encoding { get { return encoding; } }
            public override void Write(char value) { EnqueuePlain(value.ToString()); }
            public override void Write(string value) { if (!string.IsNullOrEmpty(value)) EnqueuePlain(value); }
            public override void Write(char[] buffer, int index, int count) { if (buffer != null && count > 0) EnqueuePlain(new string(buffer, index, count)); }
            public override void WriteLine() { EnqueuePlain(CoreNewLineStr); }
            public override void WriteLine(string value) { EnqueuePlain(value == null ? CoreNewLineStr : value + CoreNewLineStr); }
            public override void Flush() { }
            private string CoreNewLineStr { get { return new string(CoreNewLine); } }
        }

        private static readonly System.Collections.Concurrent.ConcurrentQueue<Item> queue = new System.Collections.Concurrent.ConcurrentQueue<Item>();
        private static readonly System.Threading.AutoResetEvent signal = new System.Threading.AutoResetEvent(false);
        private static readonly object startLock = new object();
        private static readonly object drainLock = new object();
        private static System.Threading.Thread writerThread;
        private static TextWriter realOut;
        private static int count, dropped, waiting;
        private static int maxQueued = 20000;
        private static volatile bool installed;

        public static bool Installed { get { return installed; } }

        /// <summary>Replaces Console.Out with the queued writer ([Performance] AsyncConsole, default 1).</summary>
        public static void Install()
        {
            try
            {
                maxQueued = Math.Max(1000, Conquer6609.Framework.Settings.Int("Performance", "ConsoleQueueMax", 20000));
                if (!Conquer6609.Framework.Settings.Bool("Performance", "AsyncConsole", true))
                    return;
                lock (startLock)
                {
                    if (installed)
                        return;
                    realOut = Console.Out;
                    Console.SetOut(new QueueWriter(realOut.Encoding));
                    installed = true;
                    StartThread();
                }
                AppDomain.CurrentDomain.ProcessExit += (s, e) => FlushNow(2000);
                AppDomain.CurrentDomain.UnhandledException += (s, e) => FlushNow(2000);
            }
            catch { }
        }

        public static void EnqueueColored(string prefix, string text, ConsoleColor color)
        {
            Enqueue(new Item { Kind = 1, Prefix = prefix, Text = text, Color = color });
        }

        public static void EnqueueFile(string text)
        {
            Enqueue(new Item { Kind = 2, Text = text });
        }

        private static void EnqueuePlain(string text)
        {
            Enqueue(new Item { Kind = 0, Text = text });
        }

        private static void Enqueue(Item item)
        {
            if (BootScreen.Active && item.Kind != 2)
            {
                // while the boot screen draws, every console line goes to it (nothing else writes to the console)
                if (item.Kind == 1) BootScreen.OnLine(item.Text, item.Color);
                else BootScreen.OnPlain(item.Text);
                return;
            }
            if (System.Threading.Interlocked.Increment(ref count) > maxQueued)
            {
                System.Threading.Interlocked.Decrement(ref count);
                System.Threading.Interlocked.Increment(ref dropped);
                return;
            }
            queue.Enqueue(item);
            if (writerThread == null)
                StartThread();
            if (System.Threading.Interlocked.CompareExchange(ref waiting, 0, 1) == 1)
                signal.Set();
        }

        private static void StartThread()
        {
            lock (startLock)
            {
                if (writerThread != null)
                    return;
                var t = new System.Threading.Thread(Run) { IsBackground = true, Name = "ConsoleQueue", Priority = System.Threading.ThreadPriority.BelowNormal };
                writerThread = t;
                t.Start();
            }
        }

        private static void Run()
        {
            while (true)
            {
                try
                {
                    System.Threading.Volatile.Write(ref waiting, 1);
                    if (queue.IsEmpty)
                        signal.WaitOne(1000);
                    System.Threading.Volatile.Write(ref waiting, 0);
                    lock (drainLock)
                        Drain();
                }
                catch { System.Threading.Thread.Sleep(100); }
            }
        }

        /// <summary>Writes what is queued, then runs draw with the real console writer while nothing else can write to
        /// the console (BootScreen header / summary).</summary>
        public static void Exclusive(Action<TextWriter> draw)
        {
            bool taken = false;
            try
            {
                System.Threading.Monitor.TryEnter(drainLock, 2000, ref taken);
                if (installed)
                    Drain();
                draw(realOut ?? Console.Out);
            }
            finally
            {
                if (taken)
                    System.Threading.Monitor.Exit(drainLock);
            }
        }

        /// <summary>Writes what is still queued (process exit / crash); waits at most timeoutMs for the writer.</summary>
        public static void FlushNow(int timeoutMs)
        {
            bool taken = false;
            try
            {
                System.Threading.Monitor.TryEnter(drainLock, timeoutMs, ref taken);
                if (taken)
                    Drain();
            }
            catch { }
            finally
            {
                if (taken)
                    System.Threading.Monitor.Exit(drainLock);
            }
        }

        private static void Drain()
        {
            StringBuilder plain = null;
            StringBuilder file = null;
            Item item;
            while (queue.TryDequeue(out item))
            {
                System.Threading.Interlocked.Decrement(ref count);
                switch (item.Kind)
                {
                    case 0:
                        if (realOut == null)
                            break;
                        if (plain == null)
                            plain = new StringBuilder(1024);
                        plain.Append(item.Text);
                        if (plain.Length >= 16384)
                            FlushPlain(plain);
                        break;
                    case 1:
                        FlushPlain(plain);
                        WriteColored(item);
                        break;
                    case 2:
                        if (file == null)
                            file = new StringBuilder(2048);
                        file.Append(item.Text);
                        break;
                }
            }
            FlushPlain(plain);
            int lost = System.Threading.Interlocked.Exchange(ref dropped, 0);
            if (lost > 0)
                WriteColored(new Item { Kind = 1, Prefix = "", Text = "[console] " + lost + " lines dropped (output queue full)" + Environment.NewLine, Color = ConsoleColor.Yellow });
            if (file != null)
                AppendExceptionFile(file.ToString());
        }

        private static void FlushPlain(StringBuilder plain)
        {
            if (plain == null || plain.Length == 0)
                return;
            try { realOut.Write(plain.ToString()); }
            catch { }
            plain.Length = 0;
        }

        private static void WriteColored(Item item)
        {
            TextWriter output = realOut ?? Console.Out;
            try
            {
                if (string.IsNullOrEmpty(item.Prefix))
                {
                    // raw colored text (banner, boxes): no time stamp
                    Console.ForegroundColor = item.Color;
                    output.Write(item.Text);
                    Console.ResetColor();
                    return;
                }
                string text = item.Text;
                ConsoleColor tagColor, textColor;
                string tag = ConsoleUi.Classify(ref text, item.Color, out tagColor, out textColor);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                output.Write(item.Prefix);
                output.Write(" \u2502 ");
                Console.ForegroundColor = tagColor;
                output.Write(tag);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                output.Write(" \u2502 ");
                Console.ForegroundColor = textColor;
                output.Write(text);
                Console.ResetColor();
            }
            catch { }
        }

        private static void AppendExceptionFile(string text)
        {
            try
            {
                string dir = Path.Combine(Application.StartupPath, "Exceptions");
                Directory.CreateDirectory(dir);
                File.AppendAllText(Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".log"), text, Encoding.UTF8);
            }
            catch { }
        }
    }
}
