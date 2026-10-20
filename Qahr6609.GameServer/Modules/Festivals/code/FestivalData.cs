// Festival data: one folder per festival, ServerDataBase\Modules\Festivals\<Key>\festival.ini, read and checked here.
// Nothing here throws: a festival with an error is listed (@festival list) but never started.
//
//   [Festival]    Name, Enabled, MinLevel, Edition (names the "once per festival" counters; default = Start)
//                 Start / End: "yyyy-MM-dd [HH:mm]" (a date alone = the whole day), or "MM-dd [HH:mm]" = every year;
//                 empty = no limit. Days=Mon,Tue,.. (empty = every day). Hours=08:00-12:00,19:00-23:00 (empty = all day)
//   [Announce]    Start, End (to everyone), Login (to the player), Every (minutes) + Text (to everyone while it runs)
//   [Login]       Give, MinLevel, Text: once per edition, at login (and at the start for the players online)
//   [Npc.X]       Uid, Look, Dir, Map, X, Y, Dialog          placed at the start, removed at the end
//   [Dialog.X]    Text, Option=<text>|<action> (repeat)      actions: dialog:<X> exchange:<X> teleport:<map>,<x>,<y> script:<name> close
//   [Exchange.X]  Cost, Give, Give140, Chance, FailGive, FailText, Limit, Cooldown, MinLevel, Text
//   [Item.<id>]   (or Item.<id>-<id>) Give, Give140, Consume, Limit (Counter= shares it between items), MinLevel, Always,
//                 Text (chat), Box (message box), Script (the festival's code handles the use)
//   [Drop.X]      Map, Monster (0 = any), Chance, Give, Limit  a killer's reward while it runs (into the bag)
//   [Spawn.X]     Monster, Map, X, Y, W, H, Count             monsters placed at the start, removed at the end
//   [Pool.X]      Add=<weight>|<reward list> (repeat): "pool:X" in a reward list gives one of them
//   other sections are free for the festival's script (FestivalDef.Get).
// Reward list: comma separated  item:<id>[x<n>][:b]  exp:<minutes>  chi:<n>  study:<n>  cps:<n>  bcps:<n>  silver:<n>  pool:<X>
// Give140 replaces Give for a level 140 player (EXP is no use there). Limit: daily:<n> | festival:<n> | ever:<n>.
// Chance: percent (0.01 - 100).
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Conquer6609.Modules.Festivals
{
    public sealed class Ini
    {
        public readonly List<string> Order = new List<string>();
        public readonly Dictionary<string, List<KeyValuePair<string, string>>> Sections =
            new Dictionary<string, List<KeyValuePair<string, string>>>(StringComparer.OrdinalIgnoreCase);

        public static Ini Load(string file)
        {
            var ini = new Ini();
            List<KeyValuePair<string, string>> cur = null;
            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;
                if (line[0] == '[')
                {
                    string name = line.Trim('[', ']').Trim();
                    if (!ini.Sections.TryGetValue(name, out cur))
                    {
                        ini.Sections[name] = cur = new List<KeyValuePair<string, string>>();
                        ini.Order.Add(name);
                    }
                    continue;
                }
                int eq = line.IndexOf('=');
                if (eq > 0 && cur != null) cur.Add(new KeyValuePair<string, string>(line.Substring(0, eq).Trim(), line.Substring(eq + 1).Trim()));
            }
            return ini;
        }

        public string Get(string section, string key, string def = "")
        {
            List<KeyValuePair<string, string>> s;
            if (!Sections.TryGetValue(section, out s)) return def;
            foreach (var kv in s) if (string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase)) return kv.Value;
            return def;
        }

        public List<string> All(string section, string key)
        {
            List<KeyValuePair<string, string>> s;
            var list = new List<string>();
            if (Sections.TryGetValue(section, out s))
                foreach (var kv in s) if (string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase)) list.Add(kv.Value);
            return list;
        }

        public long Num(string section, string key, long def)
        {
            long v;
            return long.TryParse(Get(section, key), out v) ? v : def;
        }

        public double Dbl(string section, string key, double def)
        {
            double v;
            return double.TryParse(Get(section, key), NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : def;
        }
    }

    // ------------------------------------------------------------------ rewards
    public sealed class Reward
    {
        public string Kind = "";// item exp chi study cps bcps silver pool
        public uint Id;
        public uint Count = 1;
        public bool Bound;
        public string Pool;

        public override string ToString()
        {
            switch (Kind)
            {
                case "item": return ItemName(Id) + (Count > 1 ? " x" + Count : "");
                case "exp": return Count + "-min EXP";
                case "chi": return Count + " Chi Points";
                case "study": return Count + " Study Points";
                case "cps": return Count + " CPs";
                case "bcps": return Count + " CPs (B)";
                case "silver": return Count + " Silver";
                default: return "a prize";
            }
        }

        public static string ItemName(uint id)
        {
            Database.ItemType.DBItem db;
            return Database.Server.ItemsBase.TryGetValue(id, out db) && db.Name != null ? db.Name : "item " + id;
        }

        /// <summary>"item:3008706x2:b, exp:30" -> rewards; bad tokens go to errors.</summary>
        public static List<Reward> ParseList(string text, List<string> errors, string where)
        {
            var list = new List<Reward>();
            if (string.IsNullOrWhiteSpace(text)) return list;
            foreach (string part in text.Split(','))
            {
                string t = part.Trim();
                if (t.Length == 0) continue;
                int c = t.IndexOf(':');
                if (c <= 0) { errors.Add(where + ": bad reward '" + t + "'"); continue; }
                var r = new Reward { Kind = t.Substring(0, c).ToLowerInvariant() };
                string v = t.Substring(c + 1);
                if (r.Kind == "pool") { r.Pool = v.Trim(); list.Add(r); continue; }
                if (r.Kind == "item")
                {
                    if (v.EndsWith(":b", StringComparison.OrdinalIgnoreCase)) { r.Bound = true; v = v.Substring(0, v.Length - 2); }
                    int x = v.IndexOf('x');
                    uint n = 1;
                    if (x > 0 && !uint.TryParse(v.Substring(x + 1), out n)) { errors.Add(where + ": bad count in '" + t + "'"); continue; }
                    if (x > 0) v = v.Substring(0, x);
                    if (!uint.TryParse(v, out r.Id)) { errors.Add(where + ": bad item id in '" + t + "'"); continue; }
                    r.Count = Math.Max(1, Math.Min(1000, n));
                    if (!Database.Server.ItemsBase.ContainsKey(r.Id)) { errors.Add(where + ": item " + r.Id + " is not in itemtype"); continue; }
                    list.Add(r);
                    continue;
                }
                if (r.Kind != "exp" && r.Kind != "chi" && r.Kind != "study" && r.Kind != "cps" && r.Kind != "bcps" && r.Kind != "silver")
                { errors.Add(where + ": unknown reward kind '" + r.Kind + "'"); continue; }
                if (!uint.TryParse(v, out r.Count) || r.Count == 0) { errors.Add(where + ": bad amount in '" + t + "'"); continue; }
                list.Add(r);
            }
            return list;
        }
    }

    public sealed class Limit
    {
        public string Scope = "";// daily festival ever ("" = none)
        public int Max;
        public bool None { get { return Scope.Length == 0 || Max <= 0; } }

        public static Limit Parse(string text, List<string> errors, string where)
        {
            var l = new Limit();
            if (string.IsNullOrWhiteSpace(text)) return l;
            string[] p = text.Split(':');
            int n;
            if (p.Length != 2 || !int.TryParse(p[1], out n) || n <= 0) { errors.Add(where + ": bad Limit '" + text + "'"); return l; }
            l.Scope = p[0].Trim().ToLowerInvariant();
            if (l.Scope != "daily" && l.Scope != "festival" && l.Scope != "ever") { errors.Add(where + ": Limit scope must be daily, festival or ever"); l.Scope = ""; }
            l.Max = n;
            return l;
        }

        public override string ToString() { return None ? "no limit" : Max + (Scope == "daily" ? " a day" : Scope == "festival" ? " per festival" : " in all"); }
    }

    // ------------------------------------------------------------------ schedule
    public sealed class Schedule
    {
        public DateTime Start = DateTime.MinValue, End = DateTime.MaxValue;// absolute (when not yearly)
        public bool Yearly;
        public int SMonth, SDay, EMonth, EDay;
        public TimeSpan STime, ETime = TimeSpan.FromDays(1);
        public bool[] Days;// null = every day (index = DayOfWeek)
        public List<int[]> Hours = new List<int[]>();// [start minute, end minute) of the day
        public string Text = "always";

        public bool InDates(DateTime now)
        {
            if (!Yearly) return now >= Start && now < End;
            for (int y = now.Year - 1; y <= now.Year; y++)
            {
                DateTime s = new DateTime(y, SMonth, Math.Min(SDay, DateTime.DaysInMonth(y, SMonth))) + STime;
                int ey = (EMonth < SMonth || (EMonth == SMonth && EDay < SDay)) ? y + 1 : y;
                DateTime e = new DateTime(ey, EMonth, Math.Min(EDay, DateTime.DaysInMonth(ey, EMonth))) + ETime;
                if (now >= s && now < e) return true;
            }
            return false;
        }

        public bool IsDue(DateTime now)
        {
            if (!InDates(now)) return false;
            if (Days != null && !Days[(int)now.DayOfWeek]) return false;
            if (Hours.Count == 0) return true;
            int m = now.Hour * 60 + now.Minute;
            foreach (var w in Hours)
                if (w[1] > w[0] ? m >= w[0] && m < w[1] : m >= w[0] || m < w[1]) return true;
            return false;
        }

        static readonly string[] DayNames = { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };

        public static Schedule Parse(Ini ini, List<string> errors)
        {
            var s = new Schedule();
            string start = ini.Get("Festival", "Start"), end = ini.Get("Festival", "End");
            bool ys = start.Length > 0 && start.Length <= 11 && start.IndexOf('-') == 2;
            bool ye = end.Length > 0 && end.Length <= 11 && end.IndexOf('-') == 2;
            if ((start.Length > 0 && end.Length > 0 && ys != ye) || (ys && end.Length == 0) || (ye && start.Length == 0))
                errors.Add("[Festival] Start and End must both be MM-dd (every year) or both yyyy-MM-dd");
            else if (ys)
            {
                s.Yearly = true;
                if (!ParseMonthDay(start, out s.SMonth, out s.SDay, out s.STime, false) || !ParseMonthDay(end, out s.EMonth, out s.EDay, out s.ETime, true))
                    errors.Add("[Festival] bad Start/End (MM-dd [HH:mm])");
            }
            else
            {
                if (start.Length > 0 && !ParseDate(start, false, out s.Start)) errors.Add("[Festival] bad Start '" + start + "' (yyyy-MM-dd [HH:mm])");
                if (end.Length > 0 && !ParseDate(end, true, out s.End)) errors.Add("[Festival] bad End '" + end + "' (yyyy-MM-dd [HH:mm])");
                if (s.End <= s.Start) errors.Add("[Festival] End is not after Start");
            }
            string days = ini.Get("Festival", "Days");
            if (days.Length > 0)
            {
                s.Days = new bool[7];
                foreach (string d in days.Split(','))
                {
                    int i = Array.IndexOf(DayNames, d.Trim().ToLowerInvariant().PadRight(3).Substring(0, 3));
                    if (i < 0) errors.Add("[Festival] bad day '" + d + "' in Days"); else s.Days[i] = true;
                }
            }
            string hours = ini.Get("Festival", "Hours");
            if (hours.Length > 0)
                foreach (string w in hours.Split(','))
                {
                    string[] p = w.Split('-');
                    TimeSpan a, b;
                    if (p.Length != 2 || !ParseTime(p[0], out a) || !ParseTime(p[1], out b)) { errors.Add("[Festival] bad Hours window '" + w + "' (HH:mm-HH:mm)"); continue; }
                    s.Hours.Add(new[] { (int)a.TotalMinutes, (int)b.TotalMinutes });
                }
            s.Text = (start.Length == 0 && end.Length == 0 ? "always" : (start.Length > 0 ? start : "...") + " -> " + (end.Length > 0 ? end : "...") + (s.Yearly ? " every year" : ""))
                + (days.Length > 0 ? ", " + days : "") + (hours.Length > 0 ? ", " + hours : "");
            return s;
        }

        static bool ParseTime(string t, out TimeSpan v)
        {
            t = t.Trim();
            if (t == "24:00") { v = TimeSpan.FromDays(1); return true; }
            return TimeSpan.TryParseExact(t, @"h\:mm", CultureInfo.InvariantCulture, out v);
        }

        static bool ParseDate(string t, bool end, out DateTime v)
        {
            if (DateTime.TryParseExact(t, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out v)) return true;
            if (DateTime.TryParseExact(t, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out v)) { if (end) v = v.AddDays(1); return true; }
            return false;
        }

        static bool ParseMonthDay(string t, out int month, out int day, out TimeSpan time, bool end)
        {
            month = day = 0;
            time = end ? TimeSpan.FromDays(1) : TimeSpan.Zero;
            string[] p = t.Trim().Split(' ');
            DateTime d;
            if (!DateTime.TryParseExact("2000-" + p[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) return false;
            month = d.Month; day = d.Day;
            return p.Length == 1 || ParseTime(p[1], out time);
        }
    }

    // ------------------------------------------------------------------ festival parts
    public sealed class FNpc { public string Name; public uint Uid; public ushort Look, X, Y; public byte Dir; public uint Map; public string Dialog; }
    public sealed class FOption { public string Text, Action; }
    public sealed class FDialog { public string Name, Text; public List<FOption> Options = new List<FOption>(); }
    public sealed class FExchange
    {
        public string Name, Text, FailText;
        public List<Reward> Cost = new List<Reward>(), Give = new List<Reward>(), Give140, FailGive = new List<Reward>();
        public double Chance = 100;
        public Limit Limit = new Limit();
        public int Cooldown, MinLevel;
    }
    public sealed class FItem
    {
        public uint Id;
        public List<Reward> Give = new List<Reward>(), Give140;
        public bool Consume = true, Always;
        public Limit Limit = new Limit();
        public int MinLevel;
        public string Text, Box, Script, Counter;
    }
    public sealed class FDrop { public string Name; public uint Map, Monster; public double Chance; public List<Reward> Give; public Limit Limit; }
    public sealed class FSpawn { public string Name; public uint Monster, Map; public ushort X, Y, W, H; public int Count; }
    public sealed class FPool { public List<int> Weights = new List<int>(); public List<List<Reward>> Entries = new List<List<Reward>>(); }

    /// <summary>One festival folder, parsed and checked.</summary>
    public sealed class FestivalDef
    {
        public string Key, Folder, Name;
        public bool Enabled = true;
        public int MinLevel;
        public string Edition;
        public Schedule Schedule;
        public string StartText, EndText, LoginText, EveryText;
        public int EveryMinutes;
        public List<Reward> LoginGive = new List<Reward>();
        public int LoginMinLevel;
        public string LoginGiveText;
        public readonly List<FNpc> Npcs = new List<FNpc>();
        public readonly Dictionary<string, FDialog> Dialogs = new Dictionary<string, FDialog>(StringComparer.OrdinalIgnoreCase);
        public readonly Dictionary<string, FExchange> Exchanges = new Dictionary<string, FExchange>(StringComparer.OrdinalIgnoreCase);
        public readonly Dictionary<uint, FItem> Items = new Dictionary<uint, FItem>();
        public readonly List<FDrop> Drops = new List<FDrop>();
        public readonly List<FSpawn> Spawns = new List<FSpawn>();
        public readonly Dictionary<string, FPool> Pools = new Dictionary<string, FPool>(StringComparer.OrdinalIgnoreCase);
        public readonly List<string> Errors = new List<string>(), Warnings = new List<string>();
        public Ini Ini;
        public bool Valid { get { return Errors.Count == 0; } }

        public string Get(string section, string key, string def = "") { return Ini.Get(section, key, def); }
        public long Num(string section, string key, long def) { return Ini.Num(section, key, def); }

        public static FestivalDef Load(string folder)
        {
            var f = new FestivalDef { Folder = folder, Key = Path.GetFileName(folder) };
            try { Parse(f); }
            catch (Exception e) { f.Errors.Add("festival.ini: " + e.Message); }
            return f;
        }

        static void Parse(FestivalDef f)
        {
            var ini = f.Ini = Ini.Load(Path.Combine(f.Folder, "festival.ini"));
            var err = f.Errors;
            f.Name = ini.Get("Festival", "Name", f.Key);
            f.Enabled = ini.Num("Festival", "Enabled", 1) != 0;
            f.MinLevel = (int)ini.Num("Festival", "MinLevel", 0);
            f.Schedule = Schedule.Parse(ini, err);
            f.Edition = ini.Get("Festival", "Edition", ini.Get("Festival", "Start", "1"));
            f.StartText = ini.Get("Announce", "Start");
            f.EndText = ini.Get("Announce", "End");
            f.LoginText = ini.Get("Announce", "Login");
            f.EveryText = ini.Get("Announce", "Text");
            f.EveryMinutes = (int)Math.Max(0, ini.Num("Announce", "Every", 0));
            f.LoginGive = Reward.ParseList(ini.Get("Login", "Give"), err, "[Login] Give");
            f.LoginMinLevel = (int)ini.Num("Login", "MinLevel", f.MinLevel);
            f.LoginGiveText = ini.Get("Login", "Text");

            foreach (string sec in ini.Order)
            {
                int dot = sec.IndexOf('.');
                if (dot <= 0) continue;
                string kind = sec.Substring(0, dot).ToLowerInvariant(), name = sec.Substring(dot + 1).Trim();
                string w = "[" + sec + "]";
                switch (kind)
                {
                    case "npc":
                        {
                            var n = new FNpc
                            {
                                Name = name, Uid = (uint)ini.Num(sec, "Uid", 0), Look = (ushort)ini.Num(sec, "Look", 0), Dir = (byte)(ini.Num(sec, "Dir", 2) % 8),
                                Map = (uint)ini.Num(sec, "Map", 0), X = (ushort)ini.Num(sec, "X", 0), Y = (ushort)ini.Num(sec, "Y", 0), Dialog = ini.Get(sec, "Dialog", name)
                            };
                            if (n.Uid == 0 || n.Look == 0 || n.Map == 0 || n.X == 0 || n.Y == 0) err.Add(w + ": Uid, Look, Map, X and Y are needed");
                            else if (!Database.Server.ServerMaps.ContainsKey(n.Map)) err.Add(w + ": map " + n.Map + " is not loaded on the server");
                            else f.Npcs.Add(n);
                            break;
                        }
                    case "dialog":
                        {
                            var d = new FDialog { Name = name, Text = ini.Get(sec, "Text") };
                            foreach (string o in ini.All(sec, "Option"))
                            {
                                int bar = o.LastIndexOf('|');
                                if (bar <= 0) { err.Add(w + ": Option must be <text>|<action>"); continue; }
                                d.Options.Add(new FOption { Text = o.Substring(0, bar).Trim(), Action = o.Substring(bar + 1).Trim() });
                            }
                            if (d.Options.Count > 40) err.Add(w + ": at most 40 options");
                            f.Dialogs[name] = d;
                            break;
                        }
                    case "exchange":
                        {
                            var x = new FExchange
                            {
                                Name = name, Text = ini.Get(sec, "Text"), FailText = ini.Get(sec, "FailText"),
                                Cost = Reward.ParseList(ini.Get(sec, "Cost"), err, w + " Cost"),
                                Give = Reward.ParseList(ini.Get(sec, "Give"), err, w + " Give"),
                                FailGive = Reward.ParseList(ini.Get(sec, "FailGive"), err, w + " FailGive"),
                                Chance = Math.Max(0.01, Math.Min(100, ini.Dbl(sec, "Chance", 100))),
                                Limit = Limit.Parse(ini.Get(sec, "Limit"), err, w),
                                Cooldown = (int)Math.Max(0, ini.Num(sec, "Cooldown", 0)),
                                MinLevel = (int)ini.Num(sec, "MinLevel", 0)
                            };
                            if (ini.Get(sec, "Give140").Length > 0) x.Give140 = Reward.ParseList(ini.Get(sec, "Give140"), err, w + " Give140");
                            if (x.Cost.Any(r => r.Kind == "pool" || r.Kind == "exp" || r.Kind == "study" || r.Kind == "chi")) err.Add(w + ": Cost may hold item, cps, bcps and silver only");
                            f.Exchanges[name] = x;
                            break;
                        }
                    case "item":
                        {
                            uint a, b = 0;
                            string[] r = name.Split('-');
                            bool ok = uint.TryParse(r[0], out a) && (r.Length == 1 || uint.TryParse(r[1], out b));
                            if (r.Length == 1) b = a;
                            if (!ok || r.Length > 2 || b < a || b - a > 200)
                            { err.Add(w + ": the section must be Item.<id> or Item.<id>-<id>"); break; }
                            for (uint id = a; id <= b; id++)
                            {
                                if (!Database.Server.ItemsBase.ContainsKey(id)) { err.Add(w + ": item " + id + " is not in itemtype"); continue; }
                                var it = new FItem
                                {
                                    Id = id, Give = Reward.ParseList(ini.Get(sec, "Give"), err, w + " Give"),
                                    Consume = ini.Num(sec, "Consume", 1) != 0, Always = ini.Num(sec, "Always", 0) != 0,
                                    Limit = Limit.Parse(ini.Get(sec, "Limit"), err, w), MinLevel = (int)ini.Num(sec, "MinLevel", 0),
                                    Text = ini.Get(sec, "Text"), Box = ini.Get(sec, "Box"), Script = ini.Get(sec, "Script"), Counter = ini.Get(sec, "Counter", "i:" + id)
                                };
                                if (ini.Get(sec, "Give140").Length > 0) it.Give140 = Reward.ParseList(ini.Get(sec, "Give140"), err, w + " Give140");
                                f.Items[id] = it;
                            }
                            break;
                        }
                    case "drop":
                        {
                            var d = new FDrop
                            {
                                Name = name, Map = (uint)ini.Num(sec, "Map", 0), Monster = (uint)ini.Num(sec, "Monster", 0),
                                Chance = Math.Max(0, Math.Min(100, ini.Dbl(sec, "Chance", 0))),
                                Give = Reward.ParseList(ini.Get(sec, "Give"), err, w + " Give"), Limit = Limit.Parse(ini.Get(sec, "Limit"), err, w)
                            };
                            if (d.Monster != 0 && !Database.Server.MonsterFamilies.ContainsKey(d.Monster)) f.Warnings.Add(w + ": monster type " + d.Monster + " does not exist");
                            if (d.Chance <= 0 || d.Give.Count == 0) err.Add(w + ": Chance and Give are needed");
                            else f.Drops.Add(d);
                            break;
                        }
                    case "spawn":
                        {
                            var s = new FSpawn
                            {
                                Name = name, Monster = (uint)ini.Num(sec, "Monster", 0), Map = (uint)ini.Num(sec, "Map", 0),
                                X = (ushort)ini.Num(sec, "X", 0), Y = (ushort)ini.Num(sec, "Y", 0), W = (ushort)Math.Max(1, ini.Num(sec, "W", 10)), H = (ushort)Math.Max(1, ini.Num(sec, "H", 10)),
                                Count = (int)Math.Max(1, Math.Min(100, ini.Num(sec, "Count", 5)))
                            };
                            if (!Database.Server.MonsterFamilies.ContainsKey(s.Monster)) err.Add(w + ": monster type " + s.Monster + " does not exist (new types need content\\Monsters + a restart)");
                            else if (!Database.Server.ServerMaps.ContainsKey(s.Map)) err.Add(w + ": map " + s.Map + " is not loaded on the server");
                            else f.Spawns.Add(s);
                            break;
                        }
                    case "pool":
                        {
                            var p = new FPool();
                            foreach (string line in ini.All(sec, "Add"))
                            {
                                int bar = line.IndexOf('|'), wt;
                                if (bar <= 0 || !int.TryParse(line.Substring(0, bar).Trim(), out wt) || wt <= 0) { err.Add(w + ": Add must be <weight>|<rewards>"); continue; }
                                p.Weights.Add(wt);
                                p.Entries.Add(Reward.ParseList(line.Substring(bar + 1), err, w));
                            }
                            if (p.Weights.Count == 0) err.Add(w + ": no Add= lines");
                            f.Pools[name] = p;
                            break;
                        }
                }
            }

            // cross checks
            Action<IEnumerable<Reward>, string> pools = (list, where) =>
            {
                if (list == null) return;
                foreach (var r in list) if (r.Kind == "pool" && !f.Pools.ContainsKey(r.Pool)) err.Add(where + ": no [Pool." + r.Pool + "]");
            };
            pools(f.LoginGive, "[Login]");
            foreach (var x in f.Exchanges.Values) { pools(x.Give, "[Exchange." + x.Name + "]"); pools(x.Give140, "[Exchange." + x.Name + "]"); pools(x.FailGive, "[Exchange." + x.Name + "]"); }
            foreach (var it in f.Items.Values) { pools(it.Give, "[Item." + it.Id + "]"); pools(it.Give140, "[Item." + it.Id + "]"); }
            foreach (var d in f.Drops) pools(d.Give, "[Drop." + d.Name + "]");
            foreach (var p in f.Pools) foreach (var e in p.Value.Entries) if (e.Any(r => r.Kind == "pool")) err.Add("[Pool." + p.Key + "]: a pool can not hold another pool");
            foreach (var n in f.Npcs) if (!f.Dialogs.ContainsKey(n.Dialog)) err.Add("[Npc." + n.Name + "]: no [Dialog." + n.Dialog + "]");
            foreach (var d in f.Dialogs.Values)
                foreach (var o in d.Options)
                {
                    string a = o.Action, arg = a.IndexOf(':') > 0 ? a.Substring(a.IndexOf(':') + 1) : "";
                    if (a.StartsWith("dialog:") && !f.Dialogs.ContainsKey(arg)) err.Add("[Dialog." + d.Name + "]: no [Dialog." + arg + "]");
                    else if (a.StartsWith("exchange:") && !f.Exchanges.ContainsKey(arg)) err.Add("[Dialog." + d.Name + "]: no [Exchange." + arg + "]");
                    else if (a.StartsWith("teleport:"))
                    {
                        string[] p = arg.Split(',');
                        uint m; ushort x, y;
                        if (p.Length != 3 || !uint.TryParse(p[0], out m) || !ushort.TryParse(p[1], out x) || !ushort.TryParse(p[2], out y)) err.Add("[Dialog." + d.Name + "]: teleport:<map>,<x>,<y>");
                        else if (!Database.Server.ServerMaps.ContainsKey(m)) err.Add("[Dialog." + d.Name + "]: map " + m + " is not loaded");
                    }
                    else if (!a.StartsWith("dialog:") && !a.StartsWith("exchange:") && !a.StartsWith("teleport:") && !a.StartsWith("script:") && a != "close")
                        err.Add("[Dialog." + d.Name + "]: unknown action '" + a + "'");
                }
            if (f.Npcs.Select(n => n.Uid).Distinct().Count() != f.Npcs.Count) err.Add("two [Npc.*] have the same Uid");
        }
    }
}
