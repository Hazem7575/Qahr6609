using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace PatchServer
{
    /// <summary>
    /// Serves GET /events.json for the launcher countdown. The source is the
    /// hand-edited file <c>www\data\events.json</c>. Two kinds of entry are
    /// supported and may be mixed:
    ///
    ///   one-time : {"name_ar":"","name_en":"","time":"2026-10-12T20:00:00Z"}
    ///   weekly   : {"name_ar":"","name_en":"","day":"Sat","time":"20:00"}
    ///              day = Sun..Sat | 0..6 (0 = Sunday) | "daily"/"everyday"
    ///              time = HH:mm in the players' local time
    ///
    /// Contract emitted to the launcher (which computes the next occurrence and
    /// ticks it down itself):
    ///   {"events":[{"name_ar","name_en","time"}],
    ///    "weekly":[{"name_ar","name_en","day","time"}]}
    ///
    /// A missing or broken source yields a valid empty feed
    /// ({"events":[],"weekly":[]}) so the launcher simply hides the countdown.
    /// </summary>
    internal sealed class EventsStore
    {
        private static readonly byte[] EmptyFeed = Encoding.UTF8.GetBytes("{\"events\":[],\"weekly\":[]}");

        private readonly string sourcePath;
        private readonly object sync = new object();

        private DateTime lastCheckUtc;
        private DateTime loadedStamp;
        private long loadedSize = -1;
        private byte[] json;

        public EventsStore(string sourcePath)
        {
            this.sourcePath = sourcePath;
        }

        /// <summary>Full path of the source file (www\data\events.json).</summary>
        public string SourcePath
        {
            get { return sourcePath; }
        }

        /// <summary>Number of entries in the current feed (one-time + weekly) for the console summary.</summary>
        public int Count { get; private set; }

        /// <summary>The current /events.json body (never null). Re-reads the source when it changed.</summary>
        public byte[] Current
        {
            get
            {
                lock (sync)
                {
                    ReloadIfChanged();
                    return json ?? EmptyFeed;
                }
            }
        }

        /// <summary>Builds the feed once from disk and logs the outcome; used at start-up.</summary>
        public void Prime()
        {
            lock (sync)
            {
                loadedSize = -1; // force a rebuild
                ReloadIfChanged();
            }

            if (!File.Exists(sourcePath))
            {
                Log.Info("events.json: no source at " + sourcePath + " - /events.json serves an empty feed (countdown hidden)");
            }
            else
            {
                Log.Info("events.json: " + Count.ToString(CultureInfo.InvariantCulture) + " entries from " + sourcePath);
            }
        }

        private void ReloadIfChanged()
        {
            if (json != null && (DateTime.UtcNow - lastCheckUtc).TotalSeconds < 1)
            {
                return;
            }
            lastCheckUtc = DateTime.UtcNow;

            DateTime stamp = DateTime.MinValue;
            long size = -1;
            if (File.Exists(sourcePath))
            {
                FileInfo fi = new FileInfo(sourcePath);
                stamp = fi.LastWriteTimeUtc;
                size = fi.Length;
            }

            if (json != null && stamp == loadedStamp && size == loadedSize)
            {
                return;
            }

            loadedStamp = stamp;
            loadedSize = size;

            if (size < 0)
            {
                json = EmptyFeed;
                Count = 0;
                return;
            }

            try
            {
                string text = ReadShared(sourcePath);
                json = Build(text, out int count);
                Count = count;
            }
            catch (Exception ex)
            {
                Log.Warn("events.json could not be built (" + ex.Message + "), serving an empty feed");
                json = EmptyFeed;
                Count = 0;
                lastCheckUtc = DateTime.UtcNow.AddSeconds(-1);
                loadedSize = -1;
            }
        }

        /// <summary>Parses the source JSON and re-emits it in the contract shape.</summary>
        internal static byte[] Build(string text, out int count)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Dictionary<string, object> parsed = serializer.DeserializeObject(text) as Dictionary<string, object>;

            List<object> oneTime = new List<object>();
            List<object> weekly = new List<object>();

            foreach (object raw in Array(parsed, "events"))
            {
                Dictionary<string, object> src = raw as Dictionary<string, object>;
                if (src == null) continue;
                string time = Str(src, "time", "nextTime", "next");
                if (string.IsNullOrEmpty(time)) continue;
                Dictionary<string, object> o = new Dictionary<string, object>();
                o["name_ar"] = Str(src, "name_ar", "name");
                o["name_en"] = Str(src, "name_en");
                o["time"] = time;
                oneTime.Add(o);
            }

            foreach (object raw in Array(parsed, "weekly"))
            {
                Dictionary<string, object> src = raw as Dictionary<string, object>;
                if (src == null) continue;
                string time = Str(src, "time");
                if (string.IsNullOrEmpty(time)) continue;
                Dictionary<string, object> o = new Dictionary<string, object>();
                o["name_ar"] = Str(src, "name_ar", "name");
                o["name_en"] = Str(src, "name_en");
                o["day"] = Days(src);
                o["time"] = time;
                weekly.Add(o);
            }

            count = oneTime.Count + weekly.Count;
            Dictionary<string, object> root = new Dictionary<string, object>();
            root["events"] = oneTime;
            root["weekly"] = weekly;
            return Encoding.UTF8.GetBytes(serializer.Serialize(root));
        }

        /// <summary>Reads "day" as a single value or an array, returned as a comma-joined string ("Mon,Wed,Fri").</summary>
        private static string Days(Dictionary<string, object> src)
        {
            object value;
            if (!src.TryGetValue("day", out value) || value == null)
            {
                return string.Empty;
            }
            object[] array = value as object[];
            if (array != null)
            {
                List<string> parts = new List<string>();
                foreach (object o in array)
                {
                    string s = Convert.ToString(o, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(s)) parts.Add(s.Trim());
                }
                return string.Join(",", parts.ToArray());
            }
            return Convert.ToString(value, CultureInfo.InvariantCulture).Trim();
        }

        private static IEnumerable<object> Array(Dictionary<string, object> root, string key)
        {
            if (root != null)
            {
                object value;
                if (root.TryGetValue(key, out value))
                {
                    object[] array = value as object[];
                    if (array != null) return array;
                }
            }
            return new object[0];
        }

        private static string Str(Dictionary<string, object> src, params string[] keys)
        {
            foreach (string key in keys)
            {
                object value;
                if (src.TryGetValue(key, out value) && value != null)
                {
                    string s = Convert.ToString(value, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(s)) return s;
                }
            }
            return string.Empty;
        }

        private static string ReadShared(string path)
        {
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read,
                        FileShare.ReadWrite | FileShare.Delete))
                    using (StreamReader reader = new StreamReader(fs, Encoding.UTF8, true))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (IOException)
                {
                    if (attempt >= 3) throw;
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }
}
