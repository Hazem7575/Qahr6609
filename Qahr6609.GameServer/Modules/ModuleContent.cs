// Module content: the game data a module brings in its own folder, so a festival copied as one folder
// (ServerDataBase\Modules\<Name>\) is complete - code (RuntimeModules) + data (this file). Read at start-up after the
// world is loaded and before the modules' OnLoad, in this order:
//
//   content\itemtype.txt              new item types (itemtype.txt lines; an id the server already has is refused)
//   content\maps\gamemap.txt          new maps: "<map id> <dmap file>" per line (the file is relative to content\maps\,
//                                     or to the client folder CO2FOLDER); "<map id> <dmap file> <base map id>" for a copy
//   content\maps\<map id>.ini         map info ([info] type, portal0_x/y, reborn_map, color), like ServerDataBase\maps
//   content\npcs.txt                  npcs.txt lines: uid,type,mesh,map,x,y[,dynamic]
//   content\Monsters\*.ini            monster types (cq_monstertype), like ServerDataBase\Monsters
//   content\MonsterSpawns\<map>\<folder>\*.ini   spawns (cq_generator), like ServerDataBase\MonsterSpawns
//   content\portals.ini               portals, like ServerDataBase\portals.ini
//
// An id that is already taken (by the server or by another module) is not added and is printed with the module name.
// A folder name starting with '_' is ignored (a switched-off module), like RuntimeModules.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conquer6609.Framework.Modules
{
    public static class ModuleContent
    {
        public sealed class Report
        {
            public string Module;
            public int Items, Maps, Npcs, Monsters, Spawns, Portals;
            public readonly List<string> Problems = new List<string>();

            public override string ToString()
            {
                var parts = new List<string>();
                if (Items > 0) parts.Add(Items + " item type(s)");
                if (Maps > 0) parts.Add(Maps + " map(s)");
                if (Npcs > 0) parts.Add(Npcs + " NPC(s)");
                if (Monsters > 0) parts.Add(Monsters + " monster type(s)");
                if (Spawns > 0) parts.Add(Spawns + " spawn(s)");
                if (Portals > 0) parts.Add(Portals + " portal(s)");
                return parts.Count == 0 ? "nothing" : string.Join(", ", parts);
            }
        }

        static readonly List<Report> reports = new List<Report>();
        // id -> module that added it (to name the owner in a conflict between two modules)
        static readonly Dictionary<string, string> owners = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static IReadOnlyList<Report> Reports { get { lock (reports) return reports.ToArray(); } }

        /// <summary>ServerDataBase\Modules\<module>\content (it may not exist).</summary>
        public static string Folder(string module)
        {
            return Path.Combine(RuntimeModules.Root, module, "content");
        }

        /// <summary>Loads the content\ folder of every module. Call once, after the world data and before module OnLoad.</summary>
        public static void LoadAll()
        {
            if (!Settings.Bool("Modules", "Content", true) || !Directory.Exists(RuntimeModules.Root))
                return;
            foreach (string dir in Directory.GetDirectories(RuntimeModules.Root).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                string name = Path.GetFileName(dir);
                if (name.StartsWith("_") || name.StartsWith("."))
                    continue;
                string content = Path.Combine(dir, "content");
                if (!Directory.Exists(content))
                    continue;
                var clock = System.Diagnostics.Stopwatch.StartNew();
                var r = new Report { Module = name };
                Step(r, "itemtype.txt", () => LoadItems(r, content));
                Step(r, "maps", () => LoadMaps(r, content));
                Step(r, "npcs.txt", () => LoadNpcs(r, content));
                Step(r, "Monsters", () => LoadMonsters(r, content));
                Step(r, "MonsterSpawns", () => LoadSpawns(r, content));
                Step(r, "portals.ini", () => LoadPortals(r, content));
                lock (reports) reports.Add(r);
                Print("[Modules] " + name + " content: " + r + " in " + clock.ElapsedMilliseconds + " ms", r.Problems.Count == 0 ? ConsoleColor.Green : ConsoleColor.Yellow);
                foreach (string p in r.Problems.Take(20))
                    Print("    " + p, ConsoleColor.Yellow);
                if (r.Problems.Count > 20)
                    Print("    ... " + (r.Problems.Count - 20) + " more", ConsoleColor.Yellow);
            }
        }

        static void Step(Report r, string what, Action load)
        {
            try { load(); }
            catch (Exception e) { r.Problems.Add(what + ": " + e.GetType().Name + ": " + e.Message); }
        }

        // true = the id is free and now belongs to this module; false = refused (the reason is in the report)
        static bool Claim(Report r, string kind, string id, bool takenByServer)
        {
            string key = kind + ":" + id, other;
            if (owners.TryGetValue(key, out other))
            {
                r.Problems.Add(kind + " " + id + " skipped: already added by module " + other);
                return false;
            }
            if (takenByServer)
            {
                r.Problems.Add(kind + " " + id + " skipped: the server already has it");
                return false;
            }
            owners[key] = r.Module;
            return true;
        }

        static void LoadItems(Report r, string content)
        {
            string file = Path.Combine(content, "itemtype.txt");
            if (!File.Exists(file))
                return;
            var items = Database.Server.ItemsBase;
            int n = 0;
            foreach (string raw in File.ReadAllLines(file))
            {
                n++;
                string line = raw.Trim();
                if (line.Length <= 11 || line.StartsWith("//"))
                    continue;
                var item = new Database.ItemType.DBItem();
                try { item.Parse(line); }
                catch (Exception e) { r.Problems.Add("itemtype.txt line " + n + ": " + e.Message); continue; }
                if (!Claim(r, "item", item.ID.ToString(), items.ContainsKey(item.ID)))
                    continue;
                items.Add(item.ID, item);
                r.Items++;
            }
        }

        static void LoadMaps(Report r, string content)
        {
            string folder = Path.Combine(content, "maps");
            string list = Path.Combine(folder, "gamemap.txt");
            if (!File.Exists(list))
                return;
            int n = 0;
            foreach (string raw in File.ReadAllLines(list))
            {
                n++;
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("//") || line.StartsWith("#"))
                    continue;
                string[] f = line.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                int id;
                uint baseId = 0;
                if (f.Length < 2 || !int.TryParse(f[0], out id) || id <= 0 || id > ushort.MaxValue || (f.Length > 2 && !uint.TryParse(f[2], out baseId)))
                {
                    r.Problems.Add("maps\\gamemap.txt line " + n + ": expected \"<map id> <dmap file> [base map id]\"");
                    continue;
                }
                string dmap = Path.Combine(folder, f[1]);
                if (!File.Exists(dmap))
                    dmap = f[1];// relative to the client folder (CO2FOLDER), like gamemap.dat
                if (!Claim(r, "map", id.ToString(), Database.Server.ServerMaps.ContainsKey((uint)id)))
                    continue;
                Role.GameMap.LoadMap(id, dmap, baseId);
                Role.GameMap map;
                if (!Database.Server.ServerMaps.TryGetValue((uint)id, out map))
                {
                    r.Problems.Add("map " + id + ": " + f[1] + " could not be loaded");
                    continue;
                }
                Role.GameMap.MapContents[id] = dmap;
                string info = Path.Combine(folder, (baseId != 0 ? baseId : (uint)id) + ".ini");
                if (File.Exists(info))
                {
                    var reader = new WindowsAPI.IniFile { FileName = info };
                    map.TypeStatus = (Role.MapTypeFlags)reader.ReadUInt32("info", "type", 0);
                    map.Reborn_X = reader.ReadUInt16("info", "portal0_x", 0);
                    map.Reborn_Y = reader.ReadUInt16("info", "portal0_y", 0);
                    map.Reborn_Map = reader.ReadUInt16("info", "reborn_map", 0);
                    map.RecordSteedRace = reader.ReadUInt32("info", "race_record", 0);// ms, written as uint by UpdateMapRace
                    map.MapColor = reader.ReadUInt32("info", "color", 0);
                }
                r.Maps++;
            }
        }

        static void LoadNpcs(Report r, string content)
        {
            string file = Path.Combine(content, "npcs.txt");
            if (!File.Exists(file))
                return;
            int n = 0;
            foreach (string raw in File.ReadAllLines(file))
            {
                n++;
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("//") || line.StartsWith("#"))
                    continue;
                Game.MsgNpc.Npc np;
                try { np = Database.NpcServer.ParseNpcLine(line.Split(',')); }
                catch (Exception e) { r.Problems.Add("npcs.txt line " + n + ": " + e.Message); continue; }
                if (np == null)
                    continue;
                Role.GameMap map;
                if (!Database.Server.ServerMaps.TryGetValue(np.Map, out map))
                {
                    r.Problems.Add("NPC " + np.UID + " skipped: map " + np.Map + " does not exist");
                    continue;
                }
                if (!Claim(r, "NPC", np.UID.ToString(), map.View.MapContain(Role.MapObjectType.Npc, np.UID)))
                    continue;
                map.AddNpc(np);
                r.Npcs++;
            }
        }

        static void LoadMonsters(Report r, string content)
        {
            string folder = Path.Combine(content, "Monsters");
            if (!Directory.Exists(folder))
                return;
            var ini = new WindowsAPI.IniFile("");
            foreach (string fname in Directory.GetFiles(folder, "*.ini").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                var family = Database.Server.ParseMonsterFamily(ini, fname);
                if (family == null)
                {
                    r.Problems.Add("Monsters\\" + Path.GetFileName(fname) + ": corrupt (needs cq_monstertype id, name, level, attack_min <= attack_max)");
                    continue;
                }
                if (!Claim(r, "monster type", family.ID.ToString(), Database.Server.MonsterFamilies.ContainsKey(family.ID)))
                    continue;
                family.CreateItemGenerator();
                family.CreateMonsterSettings();
                Database.Server.MonsterFamilies.Add(family.ID, family);
                r.Monsters++;
            }
        }

        static void LoadSpawns(Report r, string content)
        {
            string folder = Path.Combine(content, "MonsterSpawns");
            if (!Directory.Exists(folder))
                return;
            foreach (string mapDir in Directory.GetDirectories(folder))
            {
                uint mapId;
                if (uint.TryParse(Path.GetFileName(mapDir), out mapId) && !Database.Server.ServerMaps.ContainsKey(mapId))
                    r.Problems.Add("MonsterSpawns\\" + mapId + " skipped: map " + mapId + " does not exist");
            }
            r.Spawns += Database.Server.LoadSpawnFolder(folder, false);
        }

        static void LoadPortals(Report r, string content)
        {
            string file = Path.Combine(content, "portals.ini");
            if (!File.Exists(file))
                return;
            using (var read = File.OpenText(file))
            {
                Database.Server.ParsePortals(read, (section, portal) =>
                {
                    Role.GameMap map;
                    if (!Database.Server.ServerMaps.TryGetValue(portal.MapID, out map))
                    {
                        r.Problems.Add("portal " + portal.MapID + " (" + portal.X + "," + portal.Y + ") skipped: map " + portal.MapID + " does not exist");
                        return;
                    }
                    if (!Database.Server.ServerMaps.ContainsKey(portal.Destiantion_MapID))
                        r.Problems.Add("portal " + portal.MapID + " (" + portal.X + "," + portal.Y + ") leads to map " + portal.Destiantion_MapID + ", which does not exist");
                    map.Portals.Add(portal);
                    r.Portals++;
                });
            }
        }

        static void Print(string line, ConsoleColor color)
        {
            try { MyConsole.WriteLine(line, color); }
            catch { try { Console.WriteLine(line); } catch { } }
        }
    }
}
