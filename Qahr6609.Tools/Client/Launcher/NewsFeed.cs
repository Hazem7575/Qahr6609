using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;

namespace ScriptHazemLauncher
{
    // Shape served by PatchServer GET /news.json (newest first):
    //   {"items":[{"id","tag","title_ar","title_en","body_ar","body_en","date","author","image","link"}]}
    // The older shape {"items":[{"tag","title","body","tag_en","title_en","body_en",...}],"images":[...]}
    // is still accepted (theme fallback / cached files written by older launchers).
    public class NewsFeed
    {
        public List<NewsItem> items { get; set; }
        public List<NewsImage> images { get; set; }
    }

    public class NewsItem
    {
        public int id { get; set; }
        public string tag { get; set; }
        // per-item image (PatchServer contract); relative to the patch base URL or a full http(s) URL
        public string image { get; set; }
        public string date { get; set; }
        public string author { get; set; }
        public string link { get; set; }
        // Arabic / English text. The PatchServer uses *_ar/*_en; the older shape used title/body + *_en.
        public string title_ar { get; set; }
        public string title { get; set; }
        public string title_en { get; set; }
        public string body_ar { get; set; }
        public string body { get; set; }
        public string body_en { get; set; }
        public string tag_en { get; set; }

        [JsonIgnore]
        public string Tag { get { return TagLabel(Pick(tag, tag_en)); } }
        [JsonIgnore]
        public string Title { get { return Pick(title_ar ?? title, title_en); } }
        [JsonIgnore]
        public string Body { get { return Pick(body_ar ?? body, body_en); } }

        internal static string Pick(string ar, string en)
        {
            if (!Lang.IsArabic && !string.IsNullOrEmpty(en)) return en;
            return string.IsNullOrEmpty(ar) ? en : ar;
        }

        /// <summary>Maps the canonical tags the PatchServer emits (update|event|news) to a localized label.</summary>
        private static string TagLabel(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return Lang.T("TagNews");
            switch (tag.Trim().ToLowerInvariant())
            {
                case "update": return Lang.T("TagUpdate");
                case "event": return Lang.T("TagEvent");
                case "news": return Lang.T("TagNews");
                default: return tag; // already a custom/localized label - show as-is
            }
        }
    }

    public class NewsImage
    {
        public string url { get; set; }
        public string link { get; set; }
        public string caption { get; set; }
        public string caption_en { get; set; }
        [JsonIgnore]
        public string Caption { get { return NewsItem.Pick(caption, caption_en); } }
    }

    /// <summary>Row in the "older news" list.</summary>
    public class NewsRow
    {
        public int Index { get; set; }
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public FlowDirection Flow { get; set; }
    }

    /// <summary>Thumbnail in the gallery (click jumps to that news post).</summary>
    public class GalleryItem
    {
        public ImageSource Image { get; set; }
        public string Link { get; set; }
        public string Caption { get; set; }
        /// <summary>News item index this thumbnail belongs to (for jump-on-click), -1 = none.</summary>
        public int Index { get; set; } = -1;
        public FlowDirection CaptionFlow { get { return TextDir.Of(Caption); } }
    }

    /// <summary>One dot in the news pager (click jumps to that post).</summary>
    public class NewsDot
    {
        public int Index { get; set; }
        public double Size { get; set; }
        public Brush Fill { get; set; }
    }

    public static class TextDir
    {
        /// <summary>True when the text contains Arabic letters (so it should be shown right-to-left).</summary>
        public static bool IsArabic(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char ch in s)
                if (ch >= '؀' && ch <= 'ۿ') return true;
            return false;
        }

        public static FlowDirection Of(string s)
        {
            return IsArabic(s) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }
}
