using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database.DBActions
{
    public enum Mode : byte
    {
        Open = 1,
        Create = 2
    }
    public class Write : IDisposable
    {
        private string location = "";
        private List<string> Items;

        private StreamWriter SW;



        public Write(string loc)
        {
            Items = new List<string>();
            location = Program.ServerConfig.DbLocation + loc;

        }
        public void ChangeLocation(string loc)
        {
            location = Program.ServerConfig.DbLocation + loc;

        }
        public void Dispose()
        {
            Items = null;
            location = string.Empty;
            SW = null;
        }
        public Write Add(string data)
        {
            Items.Add(data);
            return this;
        }

        /// <summary>
        /// Opt-in (Mode.Open only): skip the write when the file still exists and the content is exactly what
        /// this process last wrote to it. Only for files that are written by nothing else while the server runs.
        /// </summary>
        public bool SkipIfUnchanged;

        // last content written per file (only for writers with SkipIfUnchanged)
        static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> lastWritten =
            new System.Collections.Concurrent.ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Writes "Count=N" followed by the added lines. The file is written to "&lt;file&gt;.tmp" and then
        /// swapped in (previous version kept as "&lt;file&gt;.bak"), so a crash never leaves a half written file.
        /// Mode.Open replaces the content, Mode.Create appends a block to the existing content (as before).
        /// The content is built in memory and written with a single write call (same bytes as before:
        /// UTF-8 without BOM, Environment.NewLine after every line).
        /// </summary>
        public Write Execute(Mode mod)
        {
            try
            {
                string previous = null;
                if (mod != Mode.Open && File.Exists(location))
                    previous = File.ReadAllText(location);
                StringBuilder sb = new StringBuilder((previous == null ? 0 : previous.Length) + 256 + Items.Count * 64);
                if (previous != null)
                    sb.Append(previous);
                string nl = Environment.NewLine;
                sb.Append("Count=").Append(Items.Count).Append(nl);
                for (int x = 0; x < Items.Count; x++)
                    sb.Append(Items[x]).Append(nl);
                string content = sb.ToString();

                bool track = SkipIfUnchanged && mod == Mode.Open;
                if (track)
                {
                    string last;
                    if (lastWritten.TryGetValue(location, out last) && string.Equals(last, content, StringComparison.Ordinal) && File.Exists(location))
                        return this;
                }
                byte[] bytes = new UTF8Encoding(false).GetBytes(content);
                try
                {
                    WindowsAPI.BinaryFile.WriteAllBytesAtomic(location, bytes, bytes.Length);
                }
                catch
                {
                    string ignored;
                    lastWritten.TryRemove(location, out ignored);
                    throw;
                }
                if (track)
                    lastWritten[location] = content;
            }
            catch (Exception e)
            {
                Security.Guard.Audit("save-error", 0, "", "write " + location + ": " + e.Message);
                MyConsole.WriteLine("Save failed for " + location + ": " + e.Message);
                throw;
            }
            return this;
        }
    }
}
