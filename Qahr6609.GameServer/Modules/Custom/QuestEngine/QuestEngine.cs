// Generic quest engine - runs the official Conquer Online quests (client Quest List) that have no scripted
// NPC code on this server. Data: <DbLocation>\Modules\QuestEngine\quests.ini + every quests_*.ini in that folder
// (generated / written from the official 6609 client Questinfo.ini). The exact data format is documented in
// docs\QUEST_FORMAT.md - keep that file in sync with Parse() below.
//
// Wiring: QuestEngineModule (hooks OnLoad / OnTick / OnLogout / OnMonsterKilled / OnItemUse / OnNpcTalk,
// @quest command). NPC dialogs: OnNpc runs before every NPC's own script (core hook ModuleManager.NpcTalk,
// docs\COORDINATION.md R1) and adds "Quest: ..." / "Complete: ..." options (ids 200-249) to the first dialog.
// NPCs that have no script of their own get a small greeting handler in QuestNpcs.cs.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.QuestEngine
{
    public static class QuestEngine
    {
        public enum ObjKind { Talk, Npc, Kill, Spot, Item, Level, Visit, Collect, Have, Deliver, Use, Reborn, Pk }

        public class Objective
        {
            public ObjKind Kind;
            public uint NpcId, ItemId;
            public int Count = 1, Slot = -1;     // Slot = index in QuestListItem.Intentions (-1 = none)
            public uint Map; public ushort X, Y;
            public int Radius, Chance;
            public bool OnMap, Near;             // kill flags
            public bool DropOnly;                // item: no hand-over at accept / pick-up at the spot
            public string Monster = "", Label = "";
        }

        public class Prize
        {
            public ulong Exp; public int ExpBall; public long Silver; public int Cps, Study, Chi;
            public string ItemMode = "all";      // all | choose | class | random
            public List<uint[]> Items = new List<uint[]>();   // id, count, plus, bound
        }

        public class Quest
        {
            public uint Id; public string Name = "", File = ""; public int Type, LvMin, LvMax, RebornMin, RebornMax = 255, Sex;
            public byte[] Prof = new byte[0]; public byte[] ProfGroup = new byte[0];
            public uint[] Pre = new uint[0], PreAll = new uint[0], Exclude = new uint[0];
            public uint BeginNpc, FinishNpc; public uint BeginMap, FinishMap; public ushort BeginX, BeginY, FinishX, FinishY;
            public string BeginName = "", FinishName = "", Content = "", Desc = "", PrizeText = "";
            public bool Repeat, Daily, Weekly, NoWindowAccept, AutoAccept;
            public uint StartItem;
            public int SeasonFrom, SeasonTo;     // month*100+day, 0 = no season (yearly window, may wrap the year end)
            public uint[] TeleportAccept, TeleportFinish;   // map, x, y
            public bool AutoFinish { get { return FinishNpc == 0; } }
            public int[] Days = new int[0];      // 1 = Monday .. 7 = Sunday
            public List<int[]> Hours = new List<int[]>();     // minute-of-day ranges [from, to)
            public DateTime Start = DateTime.MinValue, End = DateTime.MaxValue;
            public List<uint[]> GiveItems = new List<uint[]>(); // id, count - handed over on accept
            public Prize Prize = new Prize();
            public List<Objective> Objs = new List<Objective>();
            public int Slots;                    // size of the Intentions array this quest needs
            public int SetupSlot;                // last slot: 1 once the quest was set up (items handed over, path shown)
        }

        class Db
        {
            public readonly Dictionary<uint, Quest> Quests = new Dictionary<uint, Quest>();
            public readonly Dictionary<uint, List<Quest>> ByBegin = new Dictionary<uint, List<Quest>>();
            public readonly Dictionary<uint, List<Quest>> ByPre = new Dictionary<uint, List<Quest>>();
            public readonly HashSet<uint> UseItems = new HashSet<uint>();
            public readonly List<Quest> Auto = new List<Quest>();      // AutoAccept / StartItem quests (checked on the tick)
            public bool HasPk, HasAutoFinish;
            public readonly Dictionary<uint, uint> Alias = new Dictionary<uint, uint>();
        }

        static volatile Db db = new Db();
        public static IDictionary<uint, Quest> Quests { get { return db.Quests; } }
        static string loadInfo = "not loaded";
        public static string DataDir = "";
        public static string LoadInfo { get { return loadInfo; } }
        public static Action<string> LogLine = s => { };

        const byte OptNotNow = 255, OptSomethingElse = 200, OptShowBase = 201, OptCompleteBase = 221, OptChooseBase = 230, OptAcceptBase = 241;
        const int MaxListed = 9;

        class Pending { public uint Npc; public Dictionary<byte, Action<Client.GameClient, ServerSockets.Packet>> Actions = new Dictionary<byte, Action<Client.GameClient, ServerSockets.Packet>>(); }
        static readonly ConcurrentDictionary<uint, Pending> pending = new ConcurrentDictionary<uint, Pending>();
        static readonly ConcurrentDictionary<uint, long> pickupAt = new ConcurrentDictionary<uint, long>();
        [ThreadStatic] static Random rndTs;
        static Random Rnd { get { return rndTs ?? (rndTs = new Random(Guid.NewGuid().GetHashCode())); } }
        static long lastTick;

        // ================================================================== loading
        public static void Init()
        {
            try { Load(); }
            catch (Exception e) { loadInfo = "load failed: " + e.Message; MyConsole.WriteException(e); }
            MyConsole.WriteLine("[QuestEngine] " + loadInfo);
        }

        /// <summary>quests.ini first, then every quests_*.ini (alphabetical). A later file may not redefine a quest id.</summary>
        static void Load()
        {
            var files = new List<string>();
            if (Directory.Exists(DataDir))
            {
                string main = Path.Combine(DataDir, "quests.ini");
                if (File.Exists(main)) files.Add(main);
                files.AddRange(Directory.GetFiles(DataDir, "quests_*.ini").OrderBy(f => f, StringComparer.OrdinalIgnoreCase));
            }
            var ndb = new Db();
            var report = new List<string>();
            var perFile = new List<string>();
            int bad = 0, dup = 0;
            var all = new List<KeyValuePair<string, List<Dictionary<string, string>>>>();
            foreach (var file in files) all.Add(new KeyValuePair<string, List<Dictionary<string, string>>>(Path.GetFileName(file), ReadSections(file)));
            // [NpcAlias] sections (any file): officialId=serverUid
            foreach (var f in all)
                foreach (var s in f.Value)
                    if (string.Equals(Get(s, SectionKey), "NpcAlias", StringComparison.OrdinalIgnoreCase))
                        foreach (var kv in s)
                        {
                            uint a, b;
                            if (kv.Key != SectionKey && uint.TryParse(kv.Key, out a) && uint.TryParse(kv.Value, out b)) ndb.Alias[a] = b;
                        }
            foreach (var f in all)
            {
                string fname = f.Key;
                int ok = 0;
                foreach (var s in f.Value)
                {
                    string v;
                    if (!s.TryGetValue("Id", out v)) continue;
                    try
                    {
                        var q = Parse(s, ndb.Alias);
                        q.File = fname;
                        Quest old;
                        if (ndb.Quests.TryGetValue(q.Id, out old))
                        {
                            dup++;
                            report.Add("DUPLICATE " + q.Id + " (" + q.Name + ") in " + fname + " - already defined in " + old.File + ", ignored");
                            continue;
                        }
                        ndb.Quests[q.Id] = q;
                        Index(ndb.ByBegin, q.BeginNpc, q);
                        foreach (var p in q.Pre.Concat(q.PreAll)) Index(ndb.ByPre, p, q);
                        foreach (var o in q.Objs) if (o.Kind == ObjKind.Use) ndb.UseItems.Add(o.ItemId);
                        if (q.Objs.Any(o => o.Kind == ObjKind.Pk)) ndb.HasPk = true;
                        if (q.AutoAccept || q.StartItem != 0) ndb.Auto.Add(q);
                        if (q.AutoFinish) ndb.HasAutoFinish = true;
                        ok++;
                    }
                    catch (Exception e) { bad++; report.Add("BAD " + fname + " quest " + v + ": " + e.Message); }
                }
                perFile.Add(fname + "=" + ok);
            }
            Validate(ndb, report);
            db = ndb;
            loadInfo = ndb.Quests.Count + " generic quests loaded (" + string.Join(", ", perFile) + ")"
                + (bad > 0 ? ", " + bad + " bad" : "") + (dup > 0 ? ", " + dup + " duplicate ids ignored" : "")
                + (report.Count > 0 ? " - see load_report.txt" : "");
            foreach (var r in report) if (r.StartsWith("BAD") || r.StartsWith("DUPLICATE")) MyConsole.WriteLine("[QuestEngine] " + r);
            try
            {
                report.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + loadInfo);
                File.WriteAllLines(Path.Combine(DataDir, "load_report.txt"), report.ToArray());
            }
            catch { }
            LogLine(loadInfo);
        }

        static void Index(Dictionary<uint, List<Quest>> d, uint key, Quest q)
        {
            List<Quest> l;
            if (!d.TryGetValue(key, out l)) d[key] = l = new List<Quest>();
            l.Add(q);
        }

        static List<Dictionary<string, string>> ReadSections(string path)
        {
            var sections = new List<Dictionary<string, string>>();
            Dictionary<string, string> cur = null;
            foreach (var raw in File.ReadAllLines(path, Encoding.GetEncoding(1252)))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("#") || line.StartsWith("//")) continue;
                if (line.StartsWith("["))
                {
                    cur = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    cur[SectionKey] = line.Trim('[', ']', ' ');
                    sections.Add(cur); continue;
                }
                int eq = line.IndexOf('=');
                if (cur != null && eq > 0) cur[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
            return sections;
        }

        /// <summary>Warnings for the data authors: NPCs not standing where the data says, unknown items / monsters.</summary>
        static void Validate(Db d, List<string> report)
        {
            var monsters = new HashSet<string>();
            try { foreach (var f in Database.Server.MonsterFamilies.Values) if (f != null) monsters.Add(Norm(f.Name)); } catch { }
            foreach (var q in d.Quests.Values.OrderBy(x => x.Id))
            {
                string pfx = "WARN " + q.File + " " + q.Id + " (" + q.Name + "): ";
                if (q.BeginNpc != 0) CheckNpc(report, pfx + "begin", q.BeginNpc, q.BeginMap, q.BeginX, q.BeginY);
                else if (!q.AutoAccept && q.StartItem == 0) report.Add(pfx + "Begin npc is 0 but neither AutoAccept=1 nor StartItem= is set: only the Quest List window can start it");
                if (q.FinishNpc != 0) CheckNpc(report, pfx + "finish", q.FinishNpc, q.FinishMap, q.FinishX, q.FinishY);
                if (q.StartItem != 0 && !Database.Server.ItemsBase.ContainsKey(q.StartItem)) report.Add(pfx + "StartItem " + q.StartItem + " is not in itemtype.txt");
                foreach (var it in q.Prize.Items) if (!Database.Server.ItemsBase.ContainsKey(it[0])) report.Add(pfx + "reward item " + it[0] + " is not in itemtype.txt (not given)");
                foreach (var it in q.GiveItems) if (!Database.Server.ItemsBase.ContainsKey(it[0])) report.Add(pfx + "GiveItems " + it[0] + " is not in itemtype.txt");
                foreach (var p in q.Pre.Concat(q.PreAll)) if (!d.Quests.ContainsKey(p) && !Database.QuestInfo.AllQuests.ContainsKey(p)) report.Add(pfx + "prerequisite " + p + " is unknown (no engine quest, not in Questinfo.ini)");
                if (!Database.QuestInfo.AllQuests.ContainsKey(q.Id)) report.Add(pfx + "id is not in the server Questinfo.ini: the client Quest List window will not show it");
                foreach (var o in q.Objs)
                {
                    if (o.ItemId != 0 && !Database.Server.ItemsBase.ContainsKey(o.ItemId)) report.Add(pfx + o.Kind + " item " + o.ItemId + " is not in itemtype.txt");
                    if ((o.Kind == ObjKind.Kill || o.Kind == ObjKind.Collect || (o.Kind == ObjKind.Item && o.Monster.Length > 0)) && o.Monster.Length > 0 && o.Monster != "*" && monsters.Count > 0 && !monsters.Contains(o.Monster))
                        report.Add(pfx + o.Kind + " monster '" + o.Monster + "' matches no monster name in the server monster table");
                    if (o.Kind == ObjKind.Talk || o.Kind == ObjKind.Npc || o.Kind == ObjKind.Deliver) CheckNpc(report, pfx + o.Kind.ToString().ToLowerInvariant(), o.NpcId, o.Map, o.X, o.Y);
                }
            }
        }

        static void CheckNpc(List<string> report, string what, uint npc, uint map, ushort x, ushort y)
        {
            try
            {
                Role.GameMap m;
                if (!Database.Server.ServerMaps.TryGetValue(map, out m)) { report.Add(what + " npc " + npc + ": map " + map + " is not loaded"); return; }
                Game.MsgNpc.Npc obj;
                if (!m.SearchNpcInScreen(npc, x, y, out obj)) report.Add(what + " npc " + npc + " not found near " + map + "," + x + "," + y + " (add it to Npcs.txt or fix the position)");
            }
            catch { }
        }

        const string SectionKey = "\u0001section";

        static string Get(Dictionary<string, string> s, string k, string def = "") { string v; return s.TryGetValue(k, out v) && v != null ? v : def; }
        static int GetInt(Dictionary<string, string> s, string k, int def = 0) { int v; return int.TryParse(Get(s, k), out v) ? v : def; }
        static bool GetBool(Dictionary<string, string> s, string k) { string v = Get(s, k).ToLowerInvariant(); return v == "1" || v == "true" || v == "yes" || v == "on"; }
        static uint[] Ids(string v) { return v.Split(new[] { ',', '|', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => uint.Parse(x.Trim())).Where(x => x != 0).ToArray(); }

        static void ParseNpc(string v, string key, out uint npc, out uint map, out ushort x, out ushort y, out string name)
        {
            var b = v.Split(',');
            if (b.Length < 4) throw new Exception(key + " needs npcId,map,x,y,Name");
            npc = uint.Parse(b[0].Trim()); map = uint.Parse(b[1].Trim()); x = ushort.Parse(b[2].Trim()); y = ushort.Parse(b[3].Trim());
            name = b.Length > 4 ? b[4].Trim() : "";
        }

        static List<uint[]> ParseItems(string v, int fields)
        {
            var r = new List<uint[]>();
            foreach (var it in v.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var p = it.Split(':');
                var a = new uint[fields];
                a[0] = uint.Parse(p[0].Trim());
                if (fields > 1) a[1] = 1;
                for (int i = 1; i < fields && i < p.Length; i++) a[i] = uint.Parse(p[i].Trim());
                r.Add(a);
            }
            return r;
        }

        static Quest Parse(Dictionary<string, string> s, Dictionary<uint, uint> globalAlias)
        {
            var q = new Quest();
            var alias = new Dictionary<uint, uint>(globalAlias);
            foreach (var a in Get(s, "NpcAlias").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var p = a.Split(':', '>', '=');
                if (p.Length != 2) throw new Exception("NpcAlias must be officialId:serverUid[,...]");
                alias[uint.Parse(p[0].Trim())] = uint.Parse(p[1].Trim());
            }
            q.Id = uint.Parse(Get(s, "Id"));
            q.Name = Get(s, "Name", "Quest " + q.Id); q.Type = GetInt(s, "Type");
            q.LvMin = GetInt(s, "LvMin"); q.LvMax = GetInt(s, "LvMax");
            q.RebornMin = GetInt(s, "RebornMin"); q.RebornMax = GetInt(s, "RebornMax", 255); q.Sex = GetInt(s, "Sex");
            q.Prof = Get(s, "Prof", "0").Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => byte.Parse(x.Trim())).Where(x => x != 0).ToArray();
            q.ProfGroup = Get(s, "ProfGroup", "0").Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => byte.Parse(x.Trim())).Where(x => x != 0).ToArray();
            q.Pre = Ids(Get(s, "Pre", "0")); q.PreAll = Ids(Get(s, "PreAll", "0")); q.Exclude = Ids(Get(s, "Exclude", "0"));
            ParseNpc(Get(s, "Begin"), "Begin", out q.BeginNpc, out q.BeginMap, out q.BeginX, out q.BeginY, out q.BeginName);
            string fin = Get(s, "Finish");
            if (fin.Length == 0) { q.FinishNpc = q.BeginNpc; q.FinishMap = q.BeginMap; q.FinishX = q.BeginX; q.FinishY = q.BeginY; q.FinishName = q.BeginName; }
            else ParseNpc(fin, "Finish", out q.FinishNpc, out q.FinishMap, out q.FinishX, out q.FinishY, out q.FinishName);
            q.Content = Get(s, "Content"); q.Desc = Get(s, "Desc"); q.PrizeText = Get(s, "Prize");
            q.Repeat = GetBool(s, "Repeat"); q.Daily = GetBool(s, "Daily"); q.Weekly = GetBool(s, "Weekly"); q.NoWindowAccept = GetBool(s, "NpcOnly");
            switch (Get(s, "Reset").Trim().ToLowerInvariant())
            {
                case "": case "none": case "0": break;
                case "daily": case "day": q.Daily = true; break;
                case "weekly": case "week": q.Weekly = true; break;
                case "repeat": case "always": q.Repeat = true; break;
                default: throw new Exception("Reset must be daily, weekly or repeat");
            }
            q.AutoAccept = GetBool(s, "AutoAccept");
            string season = Get(s, "Season");
            if (season.Length > 0)
            {
                var sp = season.Split(new[] { ".." }, StringSplitOptions.None);
                if (sp.Length != 2) throw new Exception("Season must be mm-dd..mm-dd");
                q.SeasonFrom = MonthDay(sp[0]); q.SeasonTo = MonthDay(sp[1]);
            }
            q.TeleportAccept = Place(Get(s, "TeleportOnAccept"), "TeleportOnAccept");
            q.TeleportFinish = Place(Get(s, "TeleportOnFinish"), "TeleportOnFinish");
            q.StartItem = (uint)GetInt(s, "StartItem");
            q.Days = Get(s, "Days").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x.Trim())).ToArray();
            foreach (var h in Get(s, "Hours").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var p = h.Split('-');
                q.Hours.Add(new[] { Minute(p[0]), Minute(p[1]) });
            }
            if (Get(s, "Start").Length > 0) q.Start = DateTime.ParseExact(Get(s, "Start"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (Get(s, "End").Length > 0) q.End = DateTime.ParseExact(Get(s, "End"), "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1);
            q.GiveItems = ParseItems(Get(s, "GiveItems"), 2);
            ulong ul; long ll;
            if (ulong.TryParse(Get(s, "PExp"), out ul)) q.Prize.Exp = ul;
            q.Prize.ExpBall = GetInt(s, "PExpBall");
            if (long.TryParse(Get(s, "PSilver"), out ll)) q.Prize.Silver = ll;
            q.Prize.Cps = GetInt(s, "PCps"); q.Prize.Study = GetInt(s, "PStudy"); q.Prize.Chi = GetInt(s, "PChi");
            q.Prize.Items = ParseItems(Get(s, "PItems"), 4);
            q.Prize.ItemMode = Get(s, "PItemMode", GetBool(s, "PRandom") ? "random" : "all").ToLowerInvariant();
            if (q.Prize.ItemMode != "all" && q.Prize.ItemMode != "choose" && q.Prize.ItemMode != "class" && q.Prize.ItemMode != "random")
                throw new Exception("PItemMode must be all, choose, class or random");

            int n = GetInt(s, "Obj"); int nextSlot = 0;
            var objs = new List<Objective>();
            for (int i = 1; i <= n; i++)
            {
                string line = Get(s, "Obj" + i);
                if (line.Length == 0) throw new Exception("Obj" + i + " missing (Obj=" + n + ")");
                var p = line.Split('|');
                var o = new Objective();
                try
                {
                    switch (p[0].Trim().ToLowerInvariant())
                    {
                        case "talk": o.Kind = ObjKind.Talk; o.NpcId = U(p[1]); o.Map = U(p[2]); o.X = W(p[3]); o.Y = W(p[4]); o.Label = L(p, 5); break;
                        case "npc": o.Kind = ObjKind.Npc; o.NpcId = U(p[1]); o.Count = I(p[2]); o.Slot = I(p[3]) - 1; o.Map = U(p[4]); o.X = W(p[5]); o.Y = W(p[6]); o.Label = L(p, 7); break;
                        case "kill":
                            o.Kind = ObjKind.Kill; o.Count = I(p[1]); o.Slot = I(p[2]) - 1; o.Monster = Norm(p[3]); o.Map = U(p[4]); o.X = W(p[5]); o.Y = W(p[6]); o.Label = L(p, 7);
                            if (p.Length > 8) { string f = p[8].Trim().ToLowerInvariant(); o.OnMap = f == "onmap"; o.Near = f == "near"; }
                            break;
                        case "spot": o.Kind = ObjKind.Spot; o.Count = I(p[1]); o.Slot = I(p[2]) - 1; o.Map = U(p[3]); o.X = W(p[4]); o.Y = W(p[5]); o.Label = L(p, 6); break;
                        case "visit": o.Kind = ObjKind.Visit; o.Slot = I(p[1]) - 1; o.Map = U(p[2]); o.X = W(p[3]); o.Y = W(p[4]); o.Radius = Math.Max(1, I(p[5])); o.Label = L(p, 6); break;
                        case "item":
                            o.Kind = ObjKind.Item; o.ItemId = U(p[1]); o.Count = I(p[2]); o.Map = U(p[3]); o.X = W(p[4]); o.Y = W(p[5]); o.Monster = Norm(p[6]); o.Label = L(p, 7);
                            o.DropOnly = o.Monster.Length > 0;   // a named monster = drop only; the spot is for the path guide
                            if (p.Length > 8) { string f = p[8].Trim().ToLowerInvariant(); if (f == "drop") o.DropOnly = true; else if (f == "pickup") o.DropOnly = false; else if (f.Length > 0) throw new Exception("item flag must be drop or pickup"); }
                            break;
                        case "pk": o.Kind = ObjKind.Pk; o.Count = I(p[1]); o.Slot = I(p[2]) - 1; o.Map = U(p[3]); o.X = p.Length > 4 ? W(p[4]) : (ushort)0; o.Y = p.Length > 5 ? W(p[5]) : (ushort)0; o.Label = L(p, 6); break;
                        case "collect": o.Kind = ObjKind.Collect; o.ItemId = U(p[1]); o.Count = I(p[2]); o.Monster = Norm(p[3]); o.Map = U(p[4]); o.X = W(p[5]); o.Y = W(p[6]); o.Chance = Math.Max(1, Math.Min(100, I(p[7]))); o.Label = L(p, 8); break;
                        case "have": o.Kind = ObjKind.Have; o.ItemId = U(p[1]); o.Count = I(p[2]); o.Map = U(p[3]); o.X = W(p[4]); o.Y = W(p[5]); o.Label = L(p, 6); break;
                        case "deliver": o.Kind = ObjKind.Deliver; o.NpcId = U(p[1]); o.ItemId = U(p[2]); o.Count = I(p[3]); o.Map = U(p[4]); o.X = W(p[5]); o.Y = W(p[6]); o.Label = L(p, 7); break;
                        case "use": o.Kind = ObjKind.Use; o.ItemId = U(p[1]); o.Count = I(p[2]); o.Slot = I(p[3]) - 1; o.Map = U(p[4]); o.X = W(p[5]); o.Y = W(p[6]); o.Radius = I(p[7]); o.Label = L(p, 8); break;
                        case "level": o.Kind = ObjKind.Level; o.Count = I(p[1]); o.Slot = (p.Length > 2 ? I(p[2]) : 0) - 1; break;
                        case "reborn": o.Kind = ObjKind.Reborn; o.Count = I(p[1]); o.Slot = (p.Length > 2 ? I(p[2]) : 0) - 1; break;
                        default: throw new Exception("unknown objective kind '" + p[0] + "'");
                    }
                }
                catch (IndexOutOfRangeException) { throw new Exception("Obj" + i + " has too few fields: " + line); }
                catch (FormatException) { throw new Exception("Obj" + i + " has a bad number: " + line); }
                if (o.Count < 1) o.Count = 1;
                if (o.Slot >= 0) nextSlot = Math.Max(nextSlot, o.Slot + 1);
                objs.Add(o);
            }
            // objectives that need a counter but got none (slot 0 / talk / deliver) get their own slot after the data slots
            foreach (var o in objs)
                if (o.Slot < 0 && NeedsSlot(o.Kind)) o.Slot = nextSlot++;
            // two counted objectives sharing one slot (same [chk_quest] index for different targets): give them their own slots
            var used = new HashSet<int>();
            foreach (var o in objs)
            {
                if (o.Slot < 0) continue;
                if (!used.Add(o.Slot)) { o.Slot = nextSlot++; used.Add(o.Slot); }
            }
            q.Objs = objs; q.SetupSlot = nextSlot; q.Slots = nextSlot + 1;
            uint t;
            if (alias.TryGetValue(q.BeginNpc, out t)) q.BeginNpc = t;
            if (alias.TryGetValue(q.FinishNpc, out t)) q.FinishNpc = t;
            foreach (var o in objs) if (o.NpcId != 0 && alias.TryGetValue(o.NpcId, out t)) o.NpcId = t;
            return q;
        }

        static bool NeedsSlot(ObjKind k)
        {
            return k == ObjKind.Talk || k == ObjKind.Npc || k == ObjKind.Kill || k == ObjKind.Spot || k == ObjKind.Visit || k == ObjKind.Deliver
                || k == ObjKind.Use || k == ObjKind.Level || k == ObjKind.Reborn || k == ObjKind.Pk;
        }

        static uint U(string s) { s = s.Trim(); return s.Length == 0 ? 0 : uint.Parse(s); }
        static ushort W(string s) { s = s.Trim(); return s.Length == 0 ? (ushort)0 : ushort.Parse(s); }
        static int I(string s) { s = s.Trim(); return s.Length == 0 ? 0 : int.Parse(s); }
        static string L(string[] p, int i) { return p.Length > i ? p[i].Trim() : ""; }
        static int MonthDay(string md) { var p = md.Trim().Split('-'); int m = int.Parse(p[0]), d = int.Parse(p[1]); if (m < 1 || m > 12 || d < 1 || d > 31) throw new Exception("bad month-day " + md); return m * 100 + d; }
        static uint[] Place(string v, string key)
        {
            if (v.Trim().Length == 0) return null;
            var p = v.Split(',');
            if (p.Length != 3) throw new Exception(key + " must be map,x,y");
            return new[] { uint.Parse(p[0].Trim()), uint.Parse(p[1].Trim()), uint.Parse(p[2].Trim()) };
        }
        static int Minute(string hm) { var p = hm.Trim().Split(':'); return int.Parse(p[0]) * 60 + (p.Length > 1 ? int.Parse(p[1]) : 0); }

        static string Norm(string s) { s = (s ?? "").Trim(); if (s == "*") return "*"; return new string(s.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray()); }

        // ================================================================== state helpers
        static readonly DateTime Epoch = new DateTime(2000, 1, 3); // a Monday
        static uint Today { get { return (uint)(DateTime.Now.Date - Epoch).TotalDays; } }

        static MsgQuestList.QuestListItem Entry(Client.GameClient c, uint id)
        {
            MsgQuestList.QuestListItem it;
            return c.Player.QuestGUI.src.TryGetValue(id, out it) ? it : null;
        }

        static MsgQuestList.QuestListItem Active(Client.GameClient c, uint id)
        {
            var it = Entry(c, id);
            return it != null && it.Status == MsgQuestList.QuestListItem.QuestStatus.Accepted ? it : null;
        }

        static bool Finished(Client.GameClient c, uint id)
        {
            var it = Entry(c, id);
            return it != null && it.Status == MsgQuestList.QuestListItem.QuestStatus.Finished;
        }

        /// <summary>Not taken yet, or finished and allowed again (Repeat / Daily on a new day / Weekly in a new week).</summary>
        static bool Takeable(Client.GameClient c, Quest q)
        {
            var it = Entry(c, q.Id);
            if (it == null) return true;
            if (it.Status != MsgQuestList.QuestListItem.QuestStatus.Finished) return false;
            if (q.Repeat) return true;
            if (q.Daily) return it.Time != Today;
            if (q.Weekly) return it.Time / 7 != Today / 7;
            return false;
        }

        static void EnsureSlots(MsgQuestList.QuestListItem it, int n)
        {
            if (it.Intentions != null && it.Intentions.Length >= n) return;
            var a = new uint[n];
            if (it.Intentions != null) Array.Copy(it.Intentions, a, Math.Min(n, it.Intentions.Length));
            it.Intentions = a;
        }

        static void SendData(Client.GameClient c, MsgQuestList.QuestListItem it)
        {
            using (var rec = new ServerSockets.RecycledPacket())
                c.Send(rec.GetStream().MsgQuestDataCreate(0, it.UID, it.Intentions));
        }

        static bool HasItem(Client.GameClient c, uint id, int count)
        {
            return c.Inventory.Contain(id, (uint)count) || c.Inventory.Contain(id, (uint)count, 1);
        }

        static int ItemCount(Client.GameClient c, uint id, int max)
        {
            for (int n = max; n > 0; n--) if (HasItem(c, id, n)) return n;
            return 0;
        }

        static bool SlotDone(Objective o, MsgQuestList.QuestListItem it) { return o.Slot >= 0 && o.Slot < it.Intentions.Length && it.Intentions[o.Slot] >= o.Count; }

        static bool ObjDone(Client.GameClient c, Quest q, Objective o, MsgQuestList.QuestListItem it, uint atNpc)
        {
            switch (o.Kind)
            {
                case ObjKind.Item: case ObjKind.Collect: case ObjKind.Have: return HasItem(c, o.ItemId, o.Count);
                case ObjKind.Level: return c.Player.Level >= o.Count;
                case ObjKind.Reborn: return c.Player.Reborn >= o.Count;
                case ObjKind.Talk:
                case ObjKind.Npc:
                    if (atNpc != 0 && o.NpcId == atNpc && o.Count <= 1) return true;
                    return SlotDone(o, it);
                default: return SlotDone(o, it);
            }
        }

        static bool AllDone(Client.GameClient c, Quest q, MsgQuestList.QuestListItem it, uint atNpc)
        {
            EnsureSlots(it, q.Slots);
            foreach (var o in q.Objs) if (!ObjDone(c, q, o, it, atNpc)) return false;
            return true;
        }

        static readonly string[] DayNames = { "", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        /// <summary>null = the player may take the quest now, else the reason.</summary>
        static string Eligible(Client.GameClient c, Quest q)
        {
            var p = c.Player;
            bool levelGate = q.StartItem == 0;   // StartItem quests ignore LvMin/LvMax (official hides them with Lv 200-201)
            if (levelGate && p.Level < q.LvMin) return "You need level " + q.LvMin + ".";
            if (levelGate && q.LvMax > 0 && p.Level > q.LvMax) return "This quest is for level " + q.LvMin + "-" + q.LvMax + ".";
            if (p.Reborn < q.RebornMin) return q.RebornMin == 1 ? "You must be reborn first." : "You need " + q.RebornMin + " rebirths.";
            if (p.Reborn > q.RebornMax) return "This quest is not for reborn heroes.";
            if (q.Prof.Length > 0 && !q.Prof.Contains(p.Class)) return "This quest is not for your class.";
            if (q.ProfGroup.Length > 0 && !q.ProfGroup.Contains((byte)(p.Class / 10))) return "This quest is not for your class.";
            if (q.Sex != 0 && q.Sex != (p.Body % 10000 < 1005 ? 1 : 2)) return q.Sex == 1 ? "Only men can take this quest." : "Only ladies can take this quest.";
            if (q.Pre.Length > 0 && !q.Pre.Any(x => Finished(c, x))) return "Finish the earlier quest first.";
            if (q.PreAll.Length > 0 && !q.PreAll.All(x => Finished(c, x))) return "Finish the earlier quests first.";
            if (q.Exclude.Length > 0 && q.Exclude.Any(x => c.Player.QuestGUI.src.ContainsKey(x))) return "You chose another path already.";
            var now = DateTime.Now;
            if (now < q.Start || now >= q.End) return "This quest is not available now.";
            if (q.SeasonFrom != 0)
            {
                int md = now.Month * 100 + now.Day;
                bool inside = q.SeasonFrom <= q.SeasonTo ? md >= q.SeasonFrom && md <= q.SeasonTo : md >= q.SeasonFrom || md <= q.SeasonTo;
                if (!inside) return "This quest is only available from " + (q.SeasonFrom / 100) + "/" + (q.SeasonFrom % 100) + " to " + (q.SeasonTo / 100) + "/" + (q.SeasonTo % 100) + ".";
            }
            if (q.Days.Length > 0)
            {
                int dow = now.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)now.DayOfWeek;
                if (!q.Days.Contains(dow)) return "Come back on " + string.Join(", ", q.Days.Where(d => d >= 1 && d <= 7).Select(d => DayNames[d])) + ".";
            }
            if (q.Hours.Count > 0)
            {
                int m = now.Hour * 60 + now.Minute;
                if (!q.Hours.Any(h => m >= h[0] && m < h[1]))
                    return "Come back at " + string.Join(", ", q.Hours.Select(h => (h[0] / 60).ToString("00") + ":" + (h[0] % 60).ToString("00") + "-" + (h[1] / 60).ToString("00") + ":" + (h[1] % 60).ToString("00"))) + ".";
            }
            return null;
        }

        static string ItemName(uint id)
        {
            Database.ItemType.DBItem it;
            return Database.Server.ItemsBase.TryGetValue(id, out it) ? it.Name : id.ToString();
        }

        static void Tell(Client.GameClient c, string text, MsgMessage.MsgColor color = MsgMessage.MsgColor.yellow)
        {
            try { c.SendSysMesage(text, MsgMessage.ChatMode.System, color); } catch { }
        }

        static void DoAccept(Client.GameClient c, Quest q)
        {
            lock (c.Player.QuestGUI)
            {
                var old = Entry(c, q.Id);
                if (old != null)
                {
                    if (old.Status != MsgQuestList.QuestListItem.QuestStatus.Finished) return;
                    c.Player.QuestGUI.RemoveQuest(q.Id);   // repeatable / daily / weekly: start over
                }
                Database.QuestInfo.DBQuest dbq;
                if (!Database.QuestInfo.AllQuests.TryGetValue(q.Id, out dbq)) dbq = new Database.QuestInfo.DBQuest { MissionId = (ushort)q.Id, Intentions = (uint)q.Slots };
                c.Player.QuestGUI.Accept(dbq, 0);
                var it = Active(c, q.Id);
                if (it != null) { it.Intentions = new uint[q.Slots]; }
            }
        }

        // ================================================================== NPC dialog
        /// <summary>0 = not handled (run the NPC's own script with the same option), 1 = handled,
        /// 2 = run the NPC's own script with option 0 (the player chose "something else").</summary>
        public static int OnNpc(Client.GameClient c, ServerSockets.Packet stream, byte option, uint npcId)
        {
            if (c == null || c.Player == null || db.Quests.Count == 0) return 0;
            if (option != 0)
            {
                Pending p;
                if (pending.TryRemove(c.Player.UID, out p) && p.Npc == npcId)
                {
                    Action<Client.GameClient, ServerSockets.Packet> act;
                    if (option == OptSomethingElse) return 2;
                    if (p.Actions.TryGetValue(option, out act)) { act(c, stream); return 1; }
                }
                return 0;
            }
            Pending old; pending.TryRemove(c.Player.UID, out old);
            return Greet(c, stream, npcId) ? 1 : 0;
        }

        static bool Greet(Client.GameClient c, ServerSockets.Packet stream, uint npcId)
        {
            var d0 = db;
            var finish = new List<Quest>(); var offer = new List<Quest>(); var updated = new List<string>();
            foreach (var q in ActiveQuests(c))
            {
                var it = Active(c, q.Id);
                if (it == null) continue;
                bool changed = false;
                lock (c.Player.QuestGUI)
                {
                    EnsureSlots(it, q.Slots);
                    foreach (var o in q.Objs)
                    {
                        if (o.NpcId != npcId || o.Slot < 0 || it.Intentions[o.Slot] >= o.Count) continue;
                        if (o.Kind == ObjKind.Talk) { it.Intentions[o.Slot] = (uint)o.Count; changed = true; }
                        else if (o.Kind == ObjKind.Npc) { it.Intentions[o.Slot]++; changed = true; }
                        else if (o.Kind == ObjKind.Deliver && HasItem(c, o.ItemId, o.Count))
                        {
                            c.Inventory.Remove(o.ItemId, (uint)o.Count, stream);
                            it.Intentions[o.Slot] = (uint)o.Count; changed = true;
                            Tell(c, "You handed over " + (o.Count > 1 ? o.Count + " " : "") + ItemName(o.ItemId) + ".", MsgMessage.MsgColor.cyan);
                        }
                    }
                }
                if (changed) { SendData(c, it); updated.Add(q.Name); }
                if (q.FinishNpc == npcId && AllDone(c, q, it, npcId)) finish.Add(q);
            }
            List<Quest> l;
            if (d0.ByBegin.TryGetValue(npcId, out l))
                foreach (var q in l)
                    if (Takeable(c, q) && Eligible(c, q) == null) offer.Add(q);
            foreach (var n in updated) Tell(c, "Quest updated: " + n + ".");
            if (finish.Count == 0 && offer.Count == 0)
            {
                // an objective NPC with no script of its own (beacon, statue...): answer the click with a short dialog
                if (updated.Count == 0 || HasScript(npcId) || ownNpcs.Contains(npcId)) return false;
                new Game.MsgNpc.Dialog(c, stream).AddText("Quest updated: " + string.Join(", ", updated) + ".").AddOption("OK.", OptNotNow).AddAvatar(10).FinalizeDialog();
                return true;
            }

            var p = new Pending { Npc = npcId };
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText(finish.Count > 0 ? "You have done what I asked. Well done!" : "I have a task for you, if you are willing.");
            byte n2 = 0;
            foreach (var q in finish.Take(MaxListed))
            {
                var qq = q; byte opt = (byte)(OptCompleteBase + n2++);
                d.AddOption("Complete: " + Short(q.Name), opt);
                p.Actions[opt] = (cl, st) => Complete(cl, st, qq, npcId, true, -1);
            }
            n2 = 0;
            foreach (var q in offer.Take(MaxListed))
            {
                var qq = q; byte opt = (byte)(OptShowBase + n2++);
                d.AddOption("Quest: " + Short(q.Name), opt);
                p.Actions[opt] = (cl, st) => ShowQuest(cl, st, qq, npcId);
            }
            if (HasScript(npcId)) d.AddOption("Something~else.", OptSomethingElse);
            d.AddOption("Not~now.", OptNotNow);
            d.AddAvatar(10).FinalizeDialog();
            pending[c.Player.UID] = p;
            return true;
        }

        static readonly HashSet<uint> ownNpcs = new HashSet<uint>(QuestNpcs.Ids);
        static readonly HashSet<uint> handlerNpcs = new HashSet<uint> { 8, 44, 10012, 10011, 10028, 10027, 7762 }; // warehouses (NpcHandler.Warehause)
        static bool HasScript(uint npcId)
        {
            if (ownNpcs.Contains(npcId)) return false;
            if (handlerNpcs.Contains(npcId)) return true;
            Tuple<Game.MsgNpc.NpcAttribute, Game.MsgNpc.ProcessAction> t;
            return Game.MsgNpc.Procesor.invoker.TryGetInvoker((Game.MsgNpc.NpcID)npcId, out t);
        }

        static string Short(string s) { s = (s ?? "").Trim().Replace(' ', '~'); return s.Length > 32 ? s.Substring(0, 32) : s; }

        static void AddLongText(Game.MsgNpc.Dialog d, string text)
        {
            text = (text ?? "").Replace("\\n", " ").Trim();
            while (text.Length > 0)
            {
                int n = Math.Min(180, text.Length);
                if (n < text.Length) { int sp = text.LastIndexOf(' ', n); if (sp > 60) n = sp; }
                d.AddText(text.Substring(0, n) + (n < text.Length ? " " : ""));
                text = text.Substring(n).TrimStart();
            }
        }

        static void ShowQuest(Client.GameClient c, ServerSockets.Packet stream, Quest q, uint npcId)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            AddLongText(d, q.Content.Length > 0 ? q.Content : q.Desc);
            string prize = PrizeLine(q);
            if (prize.Length > 0) AddLongText(d, " Reward: " + prize);
            var p = new Pending { Npc = npcId };
            p.Actions[OptAcceptBase] = (cl, st) => AcceptByNpc(cl, st, q, npcId);
            d.AddOption("I~accept.", OptAcceptBase).AddOption("Not~now.", OptNotNow).AddAvatar(10).FinalizeDialog();
            pending[c.Player.UID] = p;
        }

        static string PrizeLine(Quest q)
        {
            if (q.PrizeText.Length > 0)
            {
                // official Prize text carries [item id,...] tags the NPC dialog cannot show: replace them with names
                var sb = new StringBuilder(); string t = q.PrizeText.Replace("\\n", " ");
                int i = 0;
                while (i < t.Length)
                {
                    int a = t.IndexOf("[item", i, StringComparison.OrdinalIgnoreCase);
                    if (a < 0) { sb.Append(t.Substring(i)); break; }
                    sb.Append(t.Substring(i, a - i));
                    int b = t.IndexOf(']', a);
                    if (b < 0) break;
                    var parts = t.Substring(a + 5, b - a - 5).Trim().Split(',');
                    uint id; if (parts.Length > 0 && uint.TryParse(parts[0].Trim(), out id)) sb.Append(" ").Append(ItemName(id)).Append(" ");
                    i = b + 1;
                }
                string r = string.Join(" ", sb.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                if (r.Length > 0) return r;
            }
            var l = new List<string>();
            var pr = q.Prize;
            if (pr.Exp > 0) l.Add(pr.Exp.ToString("N0") + " EXP");
            if (pr.ExpBall > 0) l.Add(pr.ExpBall % 600 == 0 ? (pr.ExpBall / 600) + " EXP Ball(s)" : (pr.ExpBall / 6.0).ToString("0.#") + "% of an EXP Ball");
            if (pr.Silver > 0) l.Add(pr.Silver.ToString("N0") + " silver");
            if (pr.Cps > 0) l.Add(pr.Cps + " CPs (bound)");
            if (pr.Study > 0) l.Add(pr.Study + " Study Points");
            if (pr.Chi > 0) l.Add(pr.Chi + " Chi Points");
            if (pr.Items.Count > 0) l.Add((pr.ItemMode == "all" ? "" : "one of: ") + string.Join(", ", pr.Items.Select(x => (x[1] > 1 ? x[1] + "x " : "") + ItemName(x[0]))));
            return string.Join(", ", l);
        }

        static void AcceptByNpc(Client.GameClient c, ServerSockets.Packet stream, Quest q, uint npcId)
        {
            string why = Eligible(c, q) ?? (Takeable(c, q) ? null : "You have already taken this quest.");
            if (why != null) { new Game.MsgNpc.Dialog(c, stream).AddText(why).AddOption("I~see.", OptNotNow).AddAvatar(10).FinalizeDialog(); return; }
            if (!c.Player.QuestGUI.AllowAccept())
            {
                new Game.MsgNpc.Dialog(c, stream).AddText("You already have 5 quests running. Finish or quit one in your Quest List first.").AddOption("I~see.", OptNotNow).AddAvatar(10).FinalizeDialog();
                return;
            }
            DoAccept(c, q);
            OnAccepted(c, q.Id, npcId);
            // a quest that is already complete right away (e.g. "talk to me") finishes in the same talk
            if (!Greet(c, stream, npcId))
                new Game.MsgNpc.Dialog(c, stream).AddText("Good luck! " + (q.Desc.Length > 0 ? q.Desc : "")).AddOption("I~will.", OptNotNow).AddAvatar(10).FinalizeDialog();
        }

        /// <summary>After a quest was accepted (from an NPC or from the Quest List window).</summary>
        public static void OnAccepted(Client.GameClient c, uint questId, uint npcId = 0)
        {
            Quest q;
            if (c == null || !db.Quests.TryGetValue(questId, out q)) return;
            MsgQuestList.QuestListItem it;
            lock (c.Player.QuestGUI)
            {
                it = Active(c, q.Id);
                if (it == null) return;
                EnsureSlots(it, q.Slots);
                foreach (var o in q.Objs)
                {
                    if (o.Kind == ObjKind.Level) it.Intentions[o.Slot] = Math.Min((uint)o.Count, c.Player.Level);
                    if (o.Kind == ObjKind.Reborn) it.Intentions[o.Slot] = Math.Min((uint)o.Count, c.Player.Reborn);
                }
                it.Intentions[q.SetupSlot] = 1;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var g in q.GiveItems)
                {
                    int have = ItemCount(c, g[0], (int)g[1]);
                    if (have < g[1]) GiveItem(c, stream, g[0], (int)g[1] - have, true);
                }
                // items handed over by the quest giver himself (the item spot is where the giver stands)
                foreach (var o in q.Objs)
                    if (o.Kind == ObjKind.Item && !o.DropOnly && o.Map == q.BeginMap && Role.Core.GetDistance(o.X, o.Y, q.BeginX, q.BeginY) <= 5 && !HasItem(c, o.ItemId, o.Count))
                        GiveQuestItem(c, stream, o, o.Count - ItemCount(c, o.ItemId, o.Count));
            }
            SendData(c, it);
            Tell(c, "Quest accepted: " + q.Name + ". " + q.Desc);
            if (q.TeleportAccept != null) { c.Teleport((ushort)q.TeleportAccept[1], (ushort)q.TeleportAccept[2], q.TeleportAccept[0]); return; }
            if (npcId != 0 && npcId == q.FinishNpc && AllDone(c, q, it, npcId)) return; // done on the spot: the Complete option follows
            Guide(c, q, it);
        }

        /// <summary>Shows the path to the next thing to do (the client asks the player and walks there).</summary>
        static void Guide(Client.GameClient c, Quest q, MsgQuestList.QuestListItem it)
        {
            foreach (var o in q.Objs)
            {
                if (ObjDone(c, q, o, it, 0) || o.Map == 0) continue;
                c.Player.QuestGUI.SendAutoPatcher(("Next: " + Describe(o)).Replace(' ', '~'), o.Map, o.X, o.Y, o.NpcId);
                return;
            }
            if (q.FinishMap != 0)
                c.Player.QuestGUI.SendAutoPatcher(("Report to " + q.FinishName + " to finish " + q.Name).Replace(' ', '~'), q.FinishMap, q.FinishX, q.FinishY, q.FinishNpc);
        }

        static string Describe(Objective o)
        {
            string lbl = o.Label.Length > 0 ? o.Label : null;
            switch (o.Kind)
            {
                case ObjKind.Talk: case ObjKind.Npc: return "talk to " + (lbl ?? "NPC " + o.NpcId) + (o.Count > 1 ? " x" + o.Count : "");
                case ObjKind.Kill: return "kill " + o.Count + " " + (lbl ?? o.Monster);
                case ObjKind.Spot: case ObjKind.Visit: return lbl ?? "go to the marked place";
                case ObjKind.Item: case ObjKind.Collect: case ObjKind.Have: return "get " + o.Count + " " + (lbl ?? ItemName(o.ItemId));
                case ObjKind.Deliver: return "bring " + o.Count + " " + ItemName(o.ItemId) + " to " + (lbl ?? "NPC " + o.NpcId);
                case ObjKind.Use: return lbl ?? "use " + ItemName(o.ItemId) + " x" + o.Count;
                case ObjKind.Pk: return "defeat " + o.Count + " player(s)" + (lbl != null ? " (" + lbl + ")" : "");
                case ObjKind.Reborn: return "be reborn" + (o.Count > 1 ? " " + o.Count + " times" : "");
                default: return "reach level " + o.Count;
            }
        }

        static void Complete(Client.GameClient c, ServerSockets.Packet stream, Quest q, uint npcId, bool showDialog, int choice)
        {
            var pr = q.Prize;
            if (showDialog && choice < 0 && pr.ItemMode == "choose" && pr.Items.Count > 1)
            {
                var d = new Game.MsgNpc.Dialog(c, stream);
                d.AddText("Choose your reward.");
                var p = new Pending { Npc = npcId };
                for (int i = 0; i < pr.Items.Count && i < 10; i++)
                {
                    int ii = i; byte opt = (byte)(OptChooseBase + i);
                    d.AddOption(Short((pr.Items[i][1] > 1 ? pr.Items[i][1] + "x " : "") + ItemName(pr.Items[i][0])), opt);
                    p.Actions[opt] = (cl, st) => Complete(cl, st, q, npcId, true, ii);
                }
                d.AddOption("Not~now.", OptNotNow).AddAvatar(10).FinalizeDialog();
                pending[c.Player.UID] = p;
                return;
            }
            var items = PickItems(c, q, choice);
            lock (c.Player.QuestGUI)
            {
                var it = Active(c, q.Id);
                if (it == null || q.FinishNpc != npcId || !AllDone(c, q, it, npcId)) return;
                int need = items.Sum(x => (int)Math.Max(1, Math.Min(40, x[1])));
                if (need > 0 && !c.Inventory.HaveSpace((byte)Math.Min(40, need)))
                {
                    if (!showDialog) { Tell(c, "Make " + need + " free space(s) in your inventory to finish " + q.Name + ".", MsgMessage.MsgColor.red); return; }
                    new Game.MsgNpc.Dialog(c, stream).AddText("Make " + need + " free space(s) in your inventory first.").AddOption("I~see.", OptNotNow).AddAvatar(10).FinalizeDialog();
                    return;
                }
                foreach (var o in q.Objs) if (o.Kind == ObjKind.Item || o.Kind == ObjKind.Collect) c.Inventory.Remove(o.ItemId, (uint)o.Count, stream);
                if (q.Daily || q.Weekly || q.Repeat) it.Time = Today;
                if (!c.Player.QuestGUI.FinishQuest(q.Id)) return;
            }
            GivePrize(c, stream, q, items);
            Tell(c, "Quest complete: " + q.Name + "!", MsgMessage.MsgColor.yellow);
            if (q.TeleportFinish != null) { c.Teleport((ushort)q.TeleportFinish[1], (ushort)q.TeleportFinish[2], q.TeleportFinish[0]); showDialog = false; }
            LogLine("finish " + q.Id + " " + c.Player.Name + " (" + c.Player.UID + ")");
            // next quests in the chain
            List<Quest> next;
            Quest here = null;
            if (db.ByPre.TryGetValue(q.Id, out next))
                foreach (var n in next)
                {
                    if (!Takeable(c, n) || Eligible(c, n) != null) continue;
                    if (n.BeginNpc == npcId) { here = here ?? n; continue; }
                    Tell(c, "New quest: " + n.Name + " - talk to " + n.BeginName + ".", MsgMessage.MsgColor.cyan);
                    c.Player.QuestGUI.SendAutoPatcher(("New quest: " + n.Name + ". Talk to " + n.BeginName).Replace(' ', '~'), n.BeginMap, n.BeginX, n.BeginY, n.BeginNpc);
                    break;
                }
            if (!showDialog) return;
            if (here != null) { ShowQuest(c, stream, here, npcId); return; }
            if (!Greet(c, stream, npcId))
                new Game.MsgNpc.Dialog(c, stream).AddText("Well done! You finished " + q.Name + ".").AddOption("Thank~you.", OptNotNow).AddAvatar(10).FinalizeDialog();
        }

        static List<uint[]> PickItems(Client.GameClient c, Quest q, int choice)
        {
            var all = q.Prize.Items.Where(x => Database.Server.ItemsBase.ContainsKey(x[0])).ToList();
            if (all.Count <= 1 || q.Prize.ItemMode == "all") return all;
            switch (q.Prize.ItemMode)
            {
                case "choose":
                    if (choice >= 0 && choice < q.Prize.Items.Count && Database.Server.ItemsBase.ContainsKey(q.Prize.Items[choice][0])) return new List<uint[]> { q.Prize.Items[choice] };
                    goto case "class";
                case "class":
                    foreach (var x in all)
                    {
                        Database.ItemType.DBItem d;
                        if (Database.Server.ItemsBase.TryGetValue(x[0], out d) && d.Class != 0 && Database.ItemType.EquipPassJobReq(d, c)) return new List<uint[]> { x };
                    }
                    foreach (var x in all)
                    {
                        Database.ItemType.DBItem d;
                        if (Database.Server.ItemsBase.TryGetValue(x[0], out d) && Database.ItemType.EquipPassJobReq(d, c)) return new List<uint[]> { x };
                    }
                    return new List<uint[]> { all[0] };
                default:
                    return new List<uint[]> { all[Rnd.Next(all.Count)] };
            }
        }

        static void GivePrize(Client.GameClient c, ServerSockets.Packet stream, Quest q, List<uint[]> items)
        {
            var pr = q.Prize;
            if (pr.Exp > 1) c.IncreaseExperience(stream, pr.Exp);
            if (pr.ExpBall > 0) c.GainExpBall(pr.ExpBall, true);
            if (pr.Silver > 0)
            {
                c.Player.Money += pr.Silver;
                c.Player.SendUpdate(stream, c.Player.Money, MsgUpdate.DataType.Money);
                Tell(c, "You received " + pr.Silver.ToString("N0") + " silver.");
            }
            if (pr.Cps > 0)
            {
                c.Player.BoundConquerPoints += pr.Cps;
                Tell(c, "You received " + pr.Cps + " CPs (bound).");
            }
            if (pr.Study > 0) c.Player.SubClass.AddStudyPoints(c, (ushort)Math.Min(ushort.MaxValue, pr.Study), stream);
            if (pr.Chi > 0 && c.Player.MyChi != null)
            {
                c.Player.MyChi.ChiPoints = c.Player.MyChi.ChiPoints + pr.Chi;
                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(c, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                Tell(c, "You received " + pr.Chi + " Chi Points.");
            }
            foreach (var it in items)
            {
                if (!Database.Server.ItemsBase.ContainsKey(it[0])) continue;
                if (c.Inventory.Add(stream, it[0], (byte)Math.Min(40u, Math.Max(1u, it[1])), (byte)Math.Min(12u, it[2]), 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, it[3] != 0))
                    Tell(c, "You received " + (it[1] > 1 ? it[1] + "x " : "") + ItemName(it[0]) + ".");
            }
        }

        static void GiveItem(Client.GameClient c, ServerSockets.Packet stream, uint id, int n, bool bound)
        {
            if (n <= 0 || !Database.Server.ItemsBase.ContainsKey(id) || !c.Inventory.HaveSpace((byte)Math.Min(40, n))) { if (n > 0) Tell(c, "Your inventory is full.", MsgMessage.MsgColor.red); return; }
            if (c.Inventory.Add(stream, id, (byte)Math.Min(40, n), 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound))
                Tell(c, "You got " + (n > 1 ? n + "x " : "") + ItemName(id) + ".", MsgMessage.MsgColor.cyan);
        }

        static void GiveQuestItem(Client.GameClient c, ServerSockets.Packet stream, Objective o, int n)
        {
            if (n <= 0 || !Database.Server.ItemsBase.ContainsKey(o.ItemId) || !c.Inventory.HaveSpace((byte)n)) return;
            if (c.Inventory.Add(stream, o.ItemId, (byte)n, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true))
                Tell(c, "You got " + ItemName(o.ItemId) + " (" + ItemCount(c, o.ItemId, o.Count) + "/" + o.Count + ").", MsgMessage.MsgColor.cyan);
        }

        // ================================================================== kills
        public static void OnKill(Client.GameClient c, Game.MsgMonster.MonsterRole m)
        {
            if (c == null || m == null || c.Fake || c.Player == null || db.Quests.Count == 0 || c.Player.QuestGUI.AcceptedQuests.Count == 0) return;
            string name = Norm(m.Family != null ? m.Family.Name : "");
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var q in ActiveQuests(c))
                {
                    var it = Active(c, q.Id);
                    if (it == null) continue;
                    bool changed = false;
                    lock (c.Player.QuestGUI)
                    {
                        EnsureSlots(it, q.Slots);
                        foreach (var o in q.Objs)
                        {
                            bool near = m.Map == o.Map && Role.Core.GetDistance(m.X, m.Y, o.X, o.Y) <= 40;
                            bool nameOk = o.Monster == "*" || o.Monster.Length == 0 || name == o.Monster;
                            if (o.Kind == ObjKind.Kill && nameOk && (o.Monster.Length > 0 || near) && it.Intentions[o.Slot] < o.Count
                                && (!o.OnMap || m.Map == o.Map) && (!o.Near || near))
                            { it.Intentions[o.Slot]++; changed = true; Tell(c, q.Name + ": " + (o.Label.Length > 0 ? o.Label : o.Monster) + " " + it.Intentions[o.Slot] + "/" + o.Count, MsgMessage.MsgColor.cyan); }
                            else if (o.Kind == ObjKind.Spot && near && it.Intentions[o.Slot] < o.Count)
                            { it.Intentions[o.Slot]++; changed = true; Tell(c, q.Name + ": " + o.Label + " " + it.Intentions[o.Slot] + "/" + o.Count, MsgMessage.MsgColor.cyan); }
                            else if (o.Kind == ObjKind.Item && (near || (o.Monster.Length > 0 && name == o.Monster)) && !HasItem(c, o.ItemId, o.Count) && Rnd.Next(100) < 50)
                                GiveQuestItem(c, stream, o, 1);
                            else if (o.Kind == ObjKind.Collect && nameOk && (o.Map == 0 || m.Map == o.Map) && !HasItem(c, o.ItemId, o.Count) && Rnd.Next(100) < o.Chance)
                                GiveQuestItem(c, stream, o, 1);
                        }
                    }
                    if (changed)
                    {
                        SendData(c, it);
                        if (AllDone(c, q, it, 0)) Tell(c, q.Name + " is done. Report to " + q.FinishName + ".", MsgMessage.MsgColor.yellow);
                    }
                }
            }
        }

        // ================================================================== item use
        /// <summary>true = the use was a quest action (the item is consumed here, default processing skipped).</summary>
        public static bool OnItemUse(Client.GameClient c, MsgGameItem item)
        {
            if (c == null || item == null || c.Player == null || !db.UseItems.Contains(item.ITEM_ID)) return false;
            foreach (var q in ActiveQuests(c))
            {
                var it = Active(c, q.Id);
                if (it == null) continue;
                foreach (var o in q.Objs)
                {
                    if (o.Kind != ObjKind.Use || o.ItemId != item.ITEM_ID) continue;
                    lock (c.Player.QuestGUI)
                    {
                        EnsureSlots(it, q.Slots);
                        if (it.Intentions[o.Slot] >= o.Count) continue;
                        if (o.Map != 0 && o.Radius > 0 && (c.Player.Map != o.Map || Role.Core.GetDistance(c.Player.X, c.Player.Y, o.X, o.Y) > o.Radius))
                        {
                            Tell(c, "Use it at the right place: " + Describe(o) + ".", MsgMessage.MsgColor.red);
                            c.Player.QuestGUI.SendAutoPatcher(o.Map, o.X, o.Y, 0);
                            return true;
                        }
                        it.Intentions[o.Slot]++;
                    }
                    using (var rec = new ServerSockets.RecycledPacket()) c.Inventory.Remove(item.ITEM_ID, 1, rec.GetStream());
                    SendData(c, it);
                    Tell(c, q.Name + ": " + Describe(o) + " " + it.Intentions[o.Slot] + "/" + o.Count, MsgMessage.MsgColor.cyan);
                    if (AllDone(c, q, it, 0)) Tell(c, q.Name + " is done. Report to " + q.FinishName + ".", MsgMessage.MsgColor.yellow);
                    return true;
                }
            }
            return false;
        }

        static List<Quest> ActiveQuests(Client.GameClient c)
        {
            var r = new List<Quest>();
            var d = db;
            foreach (var it in c.Player.QuestGUI.AcceptedQuests.Values)
            {
                Quest q;
                if (it.Status == MsgQuestList.QuestListItem.QuestStatus.Accepted && d.Quests.TryGetValue(it.UID, out q)) r.Add(q);
            }
            return r;
        }

        // ================================================================== tick: places, pick-ups, levels
        public static void Tick()
        {
            long now = Environment.TickCount & int.MaxValue;
            if (now - lastTick < 2000 && now >= lastTick) return;
            lastTick = now;
            if (db.Quests.Count == 0) return;
            foreach (var c in Database.Server.GamePoll.Values.ToArray())
            {
                try
                {
                    if (c == null || c.Fake || c.Player == null || !c.Player.CompleteLogin || !c.Player.Alive) continue;
                    TickProximity(c);
                    TickAuto(c);
                    if (c.Player.QuestGUI.AcceptedQuests.Count == 0) continue;
                    foreach (var q in ActiveQuests(c)) TickQuest(c, q, now);
                }
                catch (Exception e) { MyConsole.WriteException(e); }
            }
        }

        // NPCs the client does not open a dialog for (the warehouse opens the storage window): standing next to them counts as talking
        static readonly HashSet<uint> ProximityNpcs = new HashSet<uint> { 8 };

        static void TickProximity(Client.GameClient c)
        {
            foreach (var q in ActiveQuests(c))
            {
                if (!ProximityNpcs.Contains(q.FinishNpc) || c.Player.Map != q.FinishMap || Role.Core.GetDistance(c.Player.X, c.Player.Y, q.FinishX, q.FinishY) > 4) continue;
                var it = Active(c, q.Id);
                if (it != null && AllDone(c, q, it, q.FinishNpc))
                    using (var rec = new ServerSockets.RecycledPacket()) Complete(c, rec.GetStream(), q, q.FinishNpc, false, -1);
            }
            foreach (var npc in ProximityNpcs)
            {
                List<Quest> l;
                if (!db.ByBegin.TryGetValue(npc, out l)) continue;
                foreach (var q in l)
                {
                    if (c.Player.Map != q.BeginMap || Role.Core.GetDistance(c.Player.X, c.Player.Y, q.BeginX, q.BeginY) > 4) continue;
                    if (!Takeable(c, q) || Eligible(c, q) != null || !c.Player.QuestGUI.AllowAccept()) continue;
                    DoAccept(c, q);
                    OnAccepted(c, q.Id, npc);
                }
            }
        }

        /// <summary>AutoAccept (Pre done + eligible), StartItem (the item is in the bag), and quests without a finish NPC.</summary>
        static void TickAuto(Client.GameClient c)
        {
            var d = db;
            foreach (var q in d.Auto)
            {
                if (q.StartItem != 0 && !HasItem(c, q.StartItem, 1)) continue;
                if (!Takeable(c, q) || Eligible(c, q) != null || !c.Player.QuestGUI.AllowAccept()) continue;
                DoAccept(c, q);
                OnAccepted(c, q.Id);
            }
            if (!d.HasAutoFinish || c.Player.QuestGUI.AcceptedQuests.Count == 0) return;
            foreach (var q in ActiveQuests(c))
            {
                if (!q.AutoFinish) continue;
                var it = Active(c, q.Id);
                if (it != null && it.Intentions != null && it.Intentions.Length > q.SetupSlot && it.Intentions[q.SetupSlot] != 0 && AllDone(c, q, it, 0))
                    using (var rec = new ServerSockets.RecycledPacket()) Complete(c, rec.GetStream(), q, 0, false, -1);
            }
        }

        // ================================================================== PvP
        public static void OnPlayerKilled(Role.Player killer, Role.Player victim)
        {
            if (killer == null || victim == null || killer == victim || !db.HasPk) return;
            var c = killer.Owner;
            if (c == null || c.Fake || c.Player == null || c.Player.QuestGUI.AcceptedQuests.Count == 0) return;
            foreach (var q in ActiveQuests(c))
            {
                var it = Active(c, q.Id);
                if (it == null) continue;
                bool changed = false;
                lock (c.Player.QuestGUI)
                {
                    EnsureSlots(it, q.Slots);
                    foreach (var o in q.Objs)
                        if (o.Kind == ObjKind.Pk && (o.Map == 0 || victim.Map == o.Map) && it.Intentions[o.Slot] < o.Count)
                        { it.Intentions[o.Slot]++; changed = true; Tell(c, q.Name + ": " + (o.Label.Length > 0 ? o.Label : "defeat players") + " " + it.Intentions[o.Slot] + "/" + o.Count, MsgMessage.MsgColor.cyan); }
                }
                if (changed)
                {
                    SendData(c, it);
                    if (AllDone(c, q, it, 0) && !q.AutoFinish) Tell(c, q.Name + " is done. Report to " + q.FinishName + ".", MsgMessage.MsgColor.yellow);
                }
            }
        }

        static void TickQuest(Client.GameClient c, Quest q, long now)
        {
            var it = Active(c, q.Id);
            if (it == null) return;
            if (it.Intentions == null || it.Intentions.Length <= q.SetupSlot || it.Intentions[q.SetupSlot] == 0)
            {
                // accepted from the Quest List window (no NPC involved): check the gates now
                string why = q.NoWindowAccept ? "Talk to " + q.BeginName + " to take this quest." : Eligible(c, q);
                if (why != null)
                {
                    lock (c.Player.QuestGUI) c.Player.QuestGUI.RemoveQuest(q.Id);
                    Tell(c, q.Name + ": " + why, MsgMessage.MsgColor.red);
                    return;
                }
                lock (c.Player.QuestGUI) { EnsureSlots(it, q.Slots); Array.Clear(it.Intentions, 0, it.Intentions.Length); }
                OnAccepted(c, q.Id);
                return;
            }
            bool changed = false;
            lock (c.Player.QuestGUI)
            {
                EnsureSlots(it, q.Slots);
                foreach (var o in q.Objs)
                {
                    if (o.Kind == ObjKind.Level || o.Kind == ObjKind.Reborn)
                    {
                        uint v = Math.Min((uint)o.Count, o.Kind == ObjKind.Level ? c.Player.Level : (uint)c.Player.Reborn);
                        if (it.Intentions[o.Slot] != v) { it.Intentions[o.Slot] = v; changed = true; }
                        continue;
                    }
                    if (c.Player.Map != o.Map || c.Player.DynamicID != 0) continue;
                    int d = Role.Core.GetDistance(c.Player.X, c.Player.Y, o.X, o.Y);
                    if (((o.Kind == ObjKind.Spot && d <= 4) || (o.Kind == ObjKind.Visit && d <= o.Radius)) && it.Intentions[o.Slot] < o.Count)
                    {
                        it.Intentions[o.Slot] = (uint)o.Count; changed = true;
                        Tell(c, q.Name + ": " + (o.Label.Length > 0 ? o.Label : "place reached") + " - done.", MsgMessage.MsgColor.cyan);
                    }
                    else if (o.Kind == ObjKind.Item && !o.DropOnly && d <= 3 && !HasItem(c, o.ItemId, o.Count))
                    {
                        long last;
                        uint key = c.Player.UID ^ (o.ItemId << 8);
                        if (pickupAt.TryGetValue(key, out last) && now - last < 5000 && now >= last) continue;
                        pickupAt[key] = now;
                        using (var rec = new ServerSockets.RecycledPacket()) GiveQuestItem(c, rec.GetStream(), o, 1);
                    }
                }
            }
            if (changed)
            {
                SendData(c, it);
                if (AllDone(c, q, it, 0)) Tell(c, q.Name + " is done. Report to " + q.FinishName + ".", MsgMessage.MsgColor.yellow);
            }
        }

        public static void OnLogout(Client.GameClient c)
        {
            if (c == null || c.Player == null) return;
            Pending p; pending.TryRemove(c.Player.UID, out p);
        }

        // ================================================================== GM: @quest list | accept <id> | done <id> | reset <id> | reload | info <id> | check
        public static void GmCommand(Client.GameClient c, string[] data)
        {
            try
            {
                string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "list";
                uint id = 0; if (data.Length > 2) uint.TryParse(data[2], out id);
                Quest q = null; if (id != 0) db.Quests.TryGetValue(id, out q);
                switch (sub)
                {
                    case "reload": Init(); Tell(c, "[quest] " + loadInfo); break;
                    case "check":
                        try
                        {
                            var lines = File.ReadAllLines(Path.Combine(DataDir, "load_report.txt"));
                            Tell(c, "[quest] " + lines.Length + " report lines (load_report.txt):");
                            foreach (var l in lines.Take(15)) Tell(c, "  " + l);
                        }
                        catch { Tell(c, "[quest] no load_report.txt"); }
                        break;
                    case "info":
                        if (q == null) { Tell(c, "[quest] " + id + " is not a generic quest"); break; }
                        Tell(c, "[quest] " + q.Id + " " + q.Name + " (" + q.File + ") lv" + q.LvMin + "-" + q.LvMax + " from " + q.BeginName + " to " + q.FinishName + " | " + (Eligible(c, q) ?? "eligible") + (Takeable(c, q) ? "" : " | taken"));
                        foreach (var o in q.Objs) Tell(c, "  - " + Describe(o) + " @" + o.Map + "," + o.X + "," + o.Y + " slot " + (o.Slot + 1));
                        Tell(c, "  reward: " + PrizeLine(q));
                        break;
                    case "npc":
                        {
                            List<Quest> l;
                            if (!db.ByBegin.TryGetValue(id, out l)) { Tell(c, "[quest] no quest begins at npc " + id); break; }
                            foreach (var x in l) Tell(c, "  " + x.Id + " " + x.Name + " | " + (Eligible(c, x) ?? "eligible") + (Takeable(c, x) ? "" : " | taken"));
                        }
                        break;
                    case "accept":
                        if (q == null) { Tell(c, "[quest] " + id + " is not a generic quest"); break; }
                        lock (c.Player.QuestGUI) c.Player.QuestGUI.RemoveQuest(q.Id);
                        DoAccept(c, q);
                        OnAccepted(c, q.Id);
                        break;
                    case "done":
                        if (q == null) { Tell(c, "[quest] " + id + " is not a generic quest"); break; }
                        {
                            var it = Active(c, q.Id);
                            if (it == null) { Tell(c, "[quest] not active"); break; }
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var st = rec.GetStream();
                                lock (c.Player.QuestGUI)
                                {
                                    EnsureSlots(it, q.Slots);
                                    foreach (var o in q.Objs)
                                    {
                                        if (o.Slot >= 0 && o.Slot < it.Intentions.Length) it.Intentions[o.Slot] = (uint)o.Count;
                                        if (o.Kind == ObjKind.Item || o.Kind == ObjKind.Collect || o.Kind == ObjKind.Have) GiveQuestItem(c, st, o, o.Count - ItemCount(c, o.ItemId, o.Count));
                                    }
                                }
                            }
                            SendData(c, it); Tell(c, "[quest] objectives filled - talk to " + q.FinishName);
                        }
                        break;
                    case "finish":
                        if (q == null) { Tell(c, "[quest] " + id + " is not a generic quest"); break; }
                        lock (c.Player.QuestGUI)
                        {
                            if (Entry(c, q.Id) == null)
                            {
                                Database.QuestInfo.DBQuest dbq;
                                if (!Database.QuestInfo.AllQuests.TryGetValue(q.Id, out dbq)) dbq = new Database.QuestInfo.DBQuest { MissionId = (ushort)q.Id, Intentions = (uint)q.Slots };
                                c.Player.QuestGUI.Accept(dbq, 0);
                            }
                            c.Player.QuestGUI.FinishQuest(q.Id);
                        }
                        Tell(c, "[quest] marked finished (no reward): " + q.Id);
                        break;
                    case "reset":
                        lock (c.Player.QuestGUI) c.Player.QuestGUI.RemoveQuest(id);
                        Tell(c, "[quest] removed " + id);
                        break;
                    default:
                        Tell(c, "[quest] " + loadInfo + ". Active generic quests:");
                        foreach (var aq in ActiveQuests(c))
                        {
                            var it = Active(c, aq.Id);
                            if (it == null) continue;
                            Tell(c, "  " + aq.Id + " " + aq.Name + " [" + string.Join(",", it.Intentions) + "] " + (AllDone(c, aq, it, 0) ? "DONE -> " + aq.FinishName : ""));
                        }
                        Tell(c, "@quest info|accept|done|finish|reset <id>, @quest npc <npcId>, @quest reload, @quest check");
                        break;
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); Tell(c, "[quest] error " + e.Message); }
        }
    }
}
