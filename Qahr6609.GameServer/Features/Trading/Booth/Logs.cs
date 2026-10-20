using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conquer6609.Game.Conquer6609_Work
{
    class Logs
    {
        // tags written to Co_Loggs\<tag>\<M-D>\<M-D>.txt
        static readonly string[] FolderTags = new string[]
        {
            "[Chat]", "[Hack]", "[Item]", "[GMCommand]", "[PlayerCommand]", "[TradeConquerPoints]",
            "[TradeMoney]", "[TradeItem]", "[BoundConquerPoints]", "[Money]", "[ConquerPoints]",
            // forwarded through LoginQueue but previously dropped:
            "[Bosses]", "[ExChange]", "[EVENT]", "[Roulette-Money]", "[Roulette-ConquerPoints]"
        };
        // tags written to Co_Loggs\<Name><M-D>\<Name><M-D>.txt
        static readonly string[] FlatTags = new string[] { "[DemonBox]", "[PVEPoints]", "[Lottery]" };

        // log writes are queued and performed on a background thread, never on the packet thread
        static readonly BlockingCollection<string> queue = new BlockingCollection<string>(new ConcurrentQueue<string>(), 100000);
        static readonly object startLock = new object();
        static Thread writer;
        static readonly HashSet<string> knownDirectories = new HashSet<string>();
        // one open writer per log file (= per tag and day), only touched by the writer thread
        static readonly Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>(StringComparer.OrdinalIgnoreCase);
        static readonly List<StreamWriter> touched = new List<StreamWriter>();
        static string writersDate;
        const int MaxBatch = 512;

        public static void OnDequeue(string obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj))
                    return;
                StartWriter();
                queue.TryAdd(obj);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        static void StartWriter()
        {
            if (writer != null) return;
            lock (startLock)
            {
                if (writer != null) return;
                var t = new Thread(WriterLoop) { IsBackground = true, Name = "Co_Loggs" };
                t.Start();
                writer = t;
            }
        }

        static void WriterLoop()
        {
            foreach (var first in queue.GetConsumingEnumerable())
            {
                string text = first;
                int n = 0;
                do
                {
                    try
                    {
                        Write(text);
                    }
                    catch (Exception e)
                    {
                        try { MyConsole.SaveException(e); }
                        catch { }
                    }
                }
                while (++n < MaxBatch && queue.TryTake(out text));
                FlushBatch();
            }
        }

        static void FlushBatch()
        {
            foreach (var w in touched)
            {
                try { w.Flush(); }
                catch (Exception e) { Console.WriteLine("An error occurred: " + e.Message); }
            }
            touched.Clear();
        }

        static StreamWriter GetWriter(string date, string file)
        {
            if (writersDate != date)
            {
                // new day: close yesterday's files
                FlushBatch();
                foreach (var w in writers.Values)
                {
                    try { w.Dispose(); }
                    catch { }
                }
                writers.Clear();
                knownDirectories.Clear();
                writersDate = date;
            }
            StreamWriter writer;
            if (!writers.TryGetValue(file, out writer))
            {
                var fs = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                writer = new StreamWriter(fs, new UTF8Encoding(false));
                writers[file] = writer;
            }
            return writer;
        }

        static void Write(string text)
        {
            var dt = DateTime.Now;
            string date = dt.Month + "-" + dt.Day + "";
            string directory = null;
            string file = null;

            foreach (var tag in FolderTags)
            {
                if (text.StartsWith(tag))
                {
                    directory = Path.Combine(Application.StartupPath, "Co_Loggs\\" + tag + "\\", date);
                    file = Path.Combine(directory, date + ".txt");
                    break;
                }
            }
            if (directory == null)
            {
                foreach (var tag in FlatTags)
                {
                    if (text.StartsWith(tag))
                    {
                        string name = tag.Substring(1, tag.Length - 2) + date;
                        directory = Application.StartupPath + "\\Co_Loggs\\" + name;
                        file = directory + "\\" + name + ".txt";
                        break;
                    }
                }
            }
            if (directory == null)
                return;

            if (!knownDirectories.Contains(directory))
            {
                Directory.CreateDirectory(directory);
                knownDirectories.Add(directory);
            }
            try
            {
                var writer = GetWriter(date, file);
                writer.WriteLine(text);
                if (!touched.Contains(writer))
                    touched.Add(writer);
            }
            catch (IOException ex)
            {
                knownDirectories.Remove(directory);
                StreamWriter broken;
                if (writers.TryGetValue(file, out broken))
                {
                    writers.Remove(file);
                    touched.Remove(broken);
                    try { broken.Dispose(); }
                    catch { }
                }
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public static string GenerateDate()
        {
            DateTime now = DateTime.Now;
            return now.Year.ToString() + "and" + now.Month.ToString() + "and" + now.Day.ToString() + "and" + now.Hour.ToString() + "and" + now.Minute.ToString() + "and" + now.Second.ToString();
        }
    }
}
