using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace PatchServer
{
    /// <summary>
    /// Serves GET /news.json for the launcher. The source is the hand-edited
    /// file <c>www\data\news.json</c>; its images live under <c>www\news\img\</c>
    /// and are served as ordinary static files (news/img/1.jpg ...).
    ///
    /// The launcher consumes exactly:
    ///   {"items":[{"id":1,"tag":"update|event|news","title_ar":"","title_en":"",
    ///              "body_ar":"","body_en":"","date":"2026-10-12T12:00:00Z",
    ///              "author":"Hazem","image":"news/img/1.jpg","link":""}]}  (newest first)
    ///
    /// This store reads the source, normalises every item to that exact shape
    /// (assigning a sequential id when one is missing), sorts the items newest
    /// first by their ISO-8601 date and caches the result. The file is re-read
    /// when its time stamp or size changes. A missing or broken source yields a
    /// valid empty feed ({"items":[]}) so the launcher never breaks.
    /// </summary>
    internal sealed class NewsStore
    {
        private static readonly byte[] EmptyFeed = Encoding.UTF8.GetBytes("{\"items\":[]}");

        private readonly string sourcePath;
        private readonly object sync = new object();

        private DateTime lastCheckUtc;
        private DateTime loadedStamp;
        private long loadedSize = -1;
        private byte[] json;

        public NewsStore(string sourcePath)
        {
            this.sourcePath = sourcePath;
        }

        /// <summary>Full path of the source file (www\data\news.json).</summary>
        public string SourcePath
        {
            get { return sourcePath; }
        }

        /// <summary>The current /news.json body (never null). Re-reads the source when it changed.</summary>
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

        /// <summary>Number of items in the current feed (for the console summary).</summary>
        public int Count { get; private set; }

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
                Log.Warn("news source not found at " + sourcePath + " - /news.json serves an empty feed");
            }
            else
            {
                Log.Info("news.json: " + Count.ToString(CultureInfo.InvariantCulture) + " items from " + sourcePath);
            }
        }

        private void ReloadIfChanged()
        {
            // at most one disk check per second
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
                Log.Warn("news.json could not be built (" + ex.Message + "), serving an empty feed");
                json = EmptyFeed;
                Count = 0;
                // try again sooner than the normal cadence
                lastCheckUtc = DateTime.UtcNow.AddSeconds(-1);
                loadedSize = -1;
            }
        }

        /// <summary>Parses the source JSON and re-emits it in the contract shape, newest first.</summary>
        internal static byte[] Build(string text, out int count)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            object parsed = serializer.DeserializeObject(text);
            IEnumerable<object> rawItems = ExtractItems(parsed);

            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            int autoId = 0;
            foreach (object raw in rawItems)
            {
                autoId++;
                Dictionary<string, object> src = raw as Dictionary<string, object>;
                if (src == null)
                {
                    continue;
                }

                Dictionary<string, object> item = new Dictionary<string, object>();
                item["id"] = ReadId(src, autoId);
                item["tag"] = NormaliseTag(Str(src, "tag"));
                item["title_ar"] = Str(src, "title_ar", "title");
                item["title_en"] = Str(src, "title_en");
                item["body_ar"] = Str(src, "body_ar", "body");
                item["body_en"] = Str(src, "body_en");
                item["date"] = Str(src, "date");
                item["author"] = Str(src, "author");
                item["image"] = Str(src, "image");
                item["link"] = Str(src, "link");
                items.Add(item);
            }

            // newest first; items with an unparseable date sort last but keep their order
            List<Dictionary<string, object>> sorted = items
                .OrderByDescending(i => ParseDate((string)i["date"]))
                .ToList();

            count = sorted.Count;
            Dictionary<string, object> root = new Dictionary<string, object>();
            root["items"] = sorted;
            return Encoding.UTF8.GetBytes(serializer.Serialize(root));
        }

        private static IEnumerable<object> ExtractItems(object parsed)
        {
            Dictionary<string, object> root = parsed as Dictionary<string, object>;
            if (root != null)
            {
                object value;
                if (root.TryGetValue("items", out value))
                {
                    object[] array = value as object[];
                    if (array != null)
                    {
                        return array;
                    }
                }
                return new object[0];
            }

            // allow a bare array of items too
            object[] top = parsed as object[];
            return top ?? new object[0];
        }

        private static object ReadId(Dictionary<string, object> src, int fallback)
        {
            object value;
            if (src.TryGetValue("id", out value) && value != null)
            {
                if (value is int) return value;
                int n;
                if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out n))
                {
                    return n;
                }
            }
            return fallback;
        }

        private static string NormaliseTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return "news";
            }
            switch (tag.Trim().ToLowerInvariant())
            {
                case "update":
                case "تحديث":
                    return "update";
                case "event":
                case "إيفنت":
                case "ايفنت":
                case "رمضان":
                    return "event";
                case "news":
                case "خبر":
                case "أخبار":
                    return "news";
                default:
                    return "news";
            }
        }

        /// <summary>Reads a string field, trying alternative legacy key names in order.</summary>
        private static string Str(Dictionary<string, object> src, params string[] keys)
        {
            foreach (string key in keys)
            {
                object value;
                if (src.TryGetValue(key, out value) && value != null)
                {
                    string s = Convert.ToString(value, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(s))
                    {
                        return s;
                    }
                }
            }
            return string.Empty;
        }

        private static DateTime ParseDate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DateTime.MinValue;
            }
            DateTime dt;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dt))
            {
                return dt;
            }
            return DateTime.MinValue;
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
