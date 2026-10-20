using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace ScriptHazemLauncher
{
    // ======================================================================= status.json
    // GET <UpdateUrl>status.json (PatchServer): the maintenance banner + live server status.
    //   {"maintenance":false,"until":"2026-10-12T18:00:00Z","message_ar":"","message_en":"",
    //    "servers":[{"id":1,"name":"Hazem","online":true,"players":-1,"maintenance":false}]}
    public class StatusDto
    {
        public bool maintenance { get; set; }
        public string until { get; set; }
        public string message_ar { get; set; }
        public string message_en { get; set; }
        public List<StatusServer> servers { get; set; }

        [JsonIgnore]
        public string Message { get { return NewsItem.Pick(message_ar, message_en); } }

        /// <summary>The player count for a server by display name, or -1 when unknown / not present.</summary>
        public int PlayersOf(string name)
        {
            if (servers == null) return -1;
            foreach (StatusServer s in servers)
                if (string.Equals(s.name, name, StringComparison.OrdinalIgnoreCase)) return s.players;
            return -1;
        }
    }

    public class StatusServer
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool online { get; set; }
        public int players { get; set; } = -1;
        public bool maintenance { get; set; }
    }

    // ======================================================================= events.json
    // GET <UpdateUrl>events.json (PatchServer): upcoming events for the countdown.
    //   {"events":[{"name_ar","name_en","time"}],"weekly":[{"name_ar","name_en","day","time"}]}
    public class EventsFeed
    {
        public List<EventTime> events { get; set; }
        public List<WeeklyEvent> weekly { get; set; }

        /// <summary>The nearest upcoming occurrence from <paramref name="now"/> (local time), or null when none.</summary>
        public EventOccurrence Next(DateTime now)
        {
            EventOccurrence best = null;
            if (events != null)
            {
                foreach (EventTime e in events)
                {
                    DateTime? t = e.AtLocal();
                    if (t == null || t.Value <= now) continue;
                    if (best == null || t.Value < best.When) best = new EventOccurrence(e.Name, t.Value);
                }
            }
            if (weekly != null)
            {
                foreach (WeeklyEvent w in weekly)
                {
                    DateTime? t = w.NextLocal(now);
                    if (t == null) continue;
                    if (best == null || t.Value < best.When) best = new EventOccurrence(w.Name, t.Value);
                }
            }
            return best;
        }

        /// <summary>The next <paramref name="max"/> upcoming occurrences from now, soonest first (one per event).</summary>
        public List<EventOccurrence> Upcoming(DateTime now, int max)
        {
            List<EventOccurrence> all = new List<EventOccurrence>();
            if (events != null)
            {
                foreach (EventTime e in events)
                {
                    DateTime? t = e.AtLocal();
                    if (t != null && t.Value > now) all.Add(new EventOccurrence(e.Name, t.Value));
                }
            }
            if (weekly != null)
            {
                foreach (WeeklyEvent w in weekly)
                {
                    DateTime? t = w.NextLocal(now);
                    if (t != null) all.Add(new EventOccurrence(w.Name, t.Value));
                }
            }
            all.Sort(delegate (EventOccurrence a, EventOccurrence b) { return a.When.CompareTo(b.When); });
            if (max > 0 && all.Count > max) all.RemoveRange(max, all.Count - max);
            return all;
        }
    }

    public class EventTime
    {
        public string name_ar { get; set; }
        public string name_en { get; set; }
        public string name { get; set; }
        public string time { get; set; }

        [JsonIgnore]
        public string Name { get { return NewsItem.Pick(name_ar ?? name, name_en); } }

        /// <summary>The one-off date in local time (ISO-8601, 'Z' = UTC), or null when unparsable.</summary>
        public DateTime? AtLocal()
        {
            if (string.IsNullOrWhiteSpace(time)) return null;
            DateTime dt;
            if (DateTime.TryParse(time, CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind, out dt))
            {
                return dt.Kind == DateTimeKind.Utc ? dt.ToLocalTime()
                     : dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Local) : dt.ToLocalTime();
            }
            return null;
        }
    }

    public class WeeklyEvent
    {
        public string name_ar { get; set; }
        public string name_en { get; set; }
        public string name { get; set; }
        public string day { get; set; }   // Sun..Sat | 0..6 | daily | comma list
        public string time { get; set; }  // HH:mm (players' local time)

        [JsonIgnore]
        public string Name { get { return NewsItem.Pick(name_ar ?? name, name_en); } }

        /// <summary>The nearest future occurrence of this weekly event, in local time, or null.</summary>
        public DateTime? NextLocal(DateTime now)
        {
            int hh, mm;
            if (!ParseTime(time, out hh, out mm)) return null;
            List<int> days = ParseDays(day); // -1 entry = any day (daily)
            if (days.Count == 0) return null;

            DateTime? best = null;
            for (int add = 0; add <= 7; add++)
            {
                DateTime d = now.Date.AddDays(add);
                bool match = days.Contains(-1) || days.Contains((int)d.DayOfWeek);
                if (!match) continue;
                DateTime at = new DateTime(d.Year, d.Month, d.Day, hh, mm, 0, DateTimeKind.Local);
                if (at <= now) continue;
                if (best == null || at < best.Value) best = at;
            }
            return best;
        }

        private static bool ParseTime(string s, out int hh, out int mm)
        {
            hh = mm = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            string[] p = s.Trim().Split(':');
            if (p.Length < 2) return false;
            return int.TryParse(p[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out hh)
                && int.TryParse(p[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out mm)
                && hh >= 0 && hh <= 23 && mm >= 0 && mm <= 59;
        }

        /// <summary>The parsed day numbers (0=Sun..6=Sat); a single element of -1 means "daily".</summary>
        public int[] DayIndices() { return ParseDays(day).ToArray(); }

        private static List<int> ParseDays(string day)
        {
            List<int> result = new List<int>();
            if (string.IsNullOrWhiteSpace(day)) { result.Add(-1); return result; } // no day = daily
            foreach (string raw in day.Split(','))
            {
                string d = raw.Trim().ToLowerInvariant();
                if (d.Length == 0) continue;
                if (d == "daily" || d == "everyday" || d == "all") { result.Clear(); result.Add(-1); return result; }
                int n;
                if (int.TryParse(d, out n) && n >= 0 && n <= 6) { result.Add(n); continue; }
                switch (d.Substring(0, Math.Min(3, d.Length)))
                {
                    case "sun": result.Add(0); break;
                    case "mon": result.Add(1); break;
                    case "tue": result.Add(2); break;
                    case "wed": result.Add(3); break;
                    case "thu": result.Add(4); break;
                    case "fri": result.Add(5); break;
                    case "sat": result.Add(6); break;
                }
            }
            return result;
        }
    }

    /// <summary>One concrete upcoming event occurrence (localized name + when, in local time).</summary>
    public class EventOccurrence
    {
        public string Name { get; private set; }
        public DateTime When { get; private set; }
        public EventOccurrence(string name, DateTime when) { Name = name; When = when; }
    }

    /// <summary>One row of the events-schedule table; Countdown updates live while the panel is open.</summary>
    public class EventScheduleRow : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public FlowDirection NameFlow { get; set; }
        public DateTime When { get; set; }

        private string _cd = "";
        public string Countdown
        {
            get { return _cd; }
            set
            {
                if (_cd == value) return;
                _cd = value;
                PropertyChangedEventHandler h = PropertyChanged;
                if (h != null) h(this, new PropertyChangedEventArgs("Countdown"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Tick(DateTime now)
        {
            TimeSpan left = When - now;
            if (left < TimeSpan.Zero) left = TimeSpan.Zero;
            Countdown = ((int)left.TotalHours).ToString("00", CultureInfo.InvariantCulture) + ":"
                + left.Minutes.ToString("00", CultureInfo.InvariantCulture) + ":"
                + left.Seconds.ToString("00", CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Last-good copy of a launcher feed cached next to the exe (launcher_cache\&lt;name&gt;).
    /// Lets the news survive a PatchServer outage. Every call is best-effort and never throws.
    /// </summary>
    public static class FeedCache
    {
        private static string Dir { get { return Path.Combine(App.ExeDir, "launcher_cache"); } }

        public static void Save(string name, string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            try
            {
                Directory.CreateDirectory(Dir);
                File.WriteAllText(Path.Combine(Dir, name), json, new UTF8Encoding(false));
            }
            catch { /* read-only folder: the cache is a convenience only */ }
        }

        public static string Load(string name)
        {
            try
            {
                string p = Path.Combine(Dir, name);
                return File.Exists(p) ? File.ReadAllText(p, Encoding.UTF8) : null;
            }
            catch { return null; }
        }
    }
}
