// ============================================================================
//  DataHotReload - live reload of portals.ini, shops, itemtype.txt, Monsters\*.ini and a map's DMap cells.
//  See HotReload.cs for the rules. Each reload parses EVERYTHING first (with the start-up parser) and only
//  then publishes it, so a bad file changes nothing.
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Conquer6609.HotReload
{
    public static class DataHotReload
    {
        // ================================================================== portals.ini
        /// <summary>Re-reads portals.ini (Server.ParsePortals) and replaces the Portals list of every map whose
        /// portals changed (a reference swap: a player stepping into a portal right now uses the old or the new
        /// list, never half of one).</summary>
        public static void ReloadPortals(ReloadResult result)
        {
            byte[] data = HotReloadManager.ReadStable(HotReloadManager.DbFile("portals.ini"));
            var parsed = new List<Role.Portal>();
            try
            {
                using (var reader = HotReloadManager.TextOf(data))
                    Database.Server.ParsePortals(reader, (section, portal) => parsed.Add(portal));
            }
            catch (Exception e)
            {
                throw new HotReloadError("portals.ini does not parse (" + e.Message + ") after " + parsed.Count + " portal(s). Nothing was changed.");
            }
            if (parsed.Count == 0)
                throw new HotReloadError("portals.ini has no portals - not applied.");

            var maps = HotReloadManager.LoadedMapIds();
            var lists = new Dictionary<uint, List<Role.Portal>>();
            foreach (uint id in maps) lists[id] = new List<Role.Portal>();
            foreach (var p in parsed)
                if (lists.ContainsKey(p.MapID))// LoadPortals adds a portal to its own map when that map exists
                    lists[p.MapID].Add(p);

            int changedMaps = 0, added = 0, removed = 0, total = 0;
            foreach (var kv in lists)
            {
                var map = HotReloadManager.LoadedMap(kv.Key);
                if (map == null) continue;
                List<Role.Portal> old = map.Portals ?? new List<Role.Portal>();
                var oldKeys = new HashSet<string>(old.ToArray().Select(Key));
                var newKeys = new HashSet<string>(kv.Value.Select(Key));
                int a = newKeys.Count(k => !oldKeys.Contains(k)), r = oldKeys.Count(k => !newKeys.Contains(k));
                total += kv.Value.Count;
                if (a == 0 && r == 0 && old.Count == kv.Value.Count) continue;
                map.Portals = kv.Value;
                changedMaps++;
                added += a;
                removed += r;
                foreach (var p in kv.Value.Where(p => !oldKeys.Contains(Key(p))))
                    result.Note("map " + kv.Key + ": portal (" + p.X + "," + p.Y + ") -> " + p.Destiantion_MapID + " (" + p.Destiantion_X + "," + p.Destiantion_Y + ")");
                foreach (var p in old.ToArray().Where(p => !newKeys.Contains(Key(p))))
                    result.Note("map " + kv.Key + ": removed portal (" + p.X + "," + p.Y + ") -> " + p.Destiantion_MapID + " (" + p.Destiantion_X + "," + p.Destiantion_Y + ")");
            }
            result.Counts["maps"] = changedMaps;
            result.Counts["added"] = added;
            result.Counts["removed"] = removed;
            result.Counts["portals"] = total;
            result.Message = changedMaps == 0
                ? "No portal changes on the " + maps.Count + " map(s)."
                : "Applied live: portals of " + changedMaps + " map(s) updated (" + added + " new/changed, " + removed + " removed/old).";
            result.Note("Portal destinations are server-side: players use the new ones on their next step into the portal.");
        }

        private static string Key(Role.Portal p)
        {
            return p.MapID + ":" + p.X + "," + p.Y + ">" + p.Destiantion_MapID + ":" + p.Destiantion_X + "," + p.Destiantion_Y;
        }

        // ================================================================== shops
        /// <summary>Re-runs the shop loaders. Each one builds its table aside and publishes it at the end, so a
        /// purchase in progress sees the old or the new table. emoneyshop.ini + emoneyshopV2.ini are published
        /// together only when both parsed.</summary>
        public static void ReloadShops(ReloadResult result)
        {
            string shops = HotReloadManager.DbFile("shops\\");
            foreach (string f in new[] { "Shop.dat", "emoneyshop.ini", "emoneyshopV2.ini", "HonorShop.ini", "RacePointShop.ini", "GoldenLeagueShop.ini" })
                if (File.Exists(shops + f))
                    HotReloadManager.WaitStable(shops + f, true);

            int oldShop = Count(Database.Shops.ShopFile.Shops), oldE = Count(Database.Shops.EShopFile.Shops);

            Dictionary<uint, Database.Shops.ShopFile.Shop> eshops;
            try
            {
                eshops = Database.Shops.EShopFile.Build();
                if (File.Exists(shops + "emoneyshopV2.ini"))
                    Database.Shops.EShopV2File.Merge(eshops);
            }
            catch (Exception e) { throw new HotReloadError("emoneyshop.ini / emoneyshopV2.ini do not parse (" + e.Message + "). Nothing was changed."); }

            // Shop.dat: its loader publishes only at the end, so a failure keeps the old table
            var before = Database.Shops.ShopFile.Shops;
            try { Database.Shops.ShopFile.Load(); }
            catch (Exception e) { Database.Shops.ShopFile.Shops = before; throw new HotReloadError("Shop.dat does not parse (" + e.Message + "). Nothing was changed."); }
            if (Count(Database.Shops.ShopFile.Shops) == 0 && oldShop > 0)
            {
                Database.Shops.ShopFile.Shops = before;
                throw new HotReloadError("Shop.dat has no shops (Header/Amount missing?) - not applied.");
            }
            Database.Shops.EShopFile.Shops = eshops;
            result.Counts["shops"] = Count(Database.Shops.ShopFile.Shops);
            result.Counts["emoneyShops"] = Count(eshops);
            result.Note("Shop.dat: " + oldShop + " -> " + Count(Database.Shops.ShopFile.Shops) + " shops; emoneyshop: " + oldE + " -> " + Count(eshops) + " shops.");
            int items = Database.Shops.ShopFile.Shops.Values.Sum(s => s.Items == null ? 0 : s.Items.Count) + eshops.Values.Sum(s => (s.Items == null ? 0 : s.Items.Count) + (s.BoundItems == null ? 0 : s.BoundItems.Count));
            result.Counts["items"] = items;

            var failed = new List<string>();
            TryShop("HonorShop.ini", shops, () => Database.Shops.HonorShop.Load(), failed);
            TryShop("RacePointShop.ini", shops, () => Database.Shops.RacePointShop.Load(), failed);
            TryShop("GoldenLeagueShop.ini", shops, () => Database.Shops.ChampionShop.Load(), failed);
            foreach (string f in failed) result.Note("NOT reloaded (kept the old table): " + f);

            result.Message = "Applied live: " + result.Counts["shops"] + " shops (Shop.dat) + " + result.Counts["emoneyShops"] + " emoney shops, " + items + " items" +
                             (failed.Count > 0 ? "; " + failed.Count + " special shop file(s) failed" : "") + ".";
            result.Note("The server now sells/validates the new lists. The shop WINDOW is drawn by the client from its own shop files: an item missing from the client's list can't be clicked until the client is patched.");
        }

        private static void TryShop(string file, string folder, Action load, List<string> failed)
        {
            if (!File.Exists(folder + file)) return;
            try { load(); }
            catch (Exception e) { failed.Add(file + " (" + e.Message + ")"); }
        }

        private static int Count<T>(Dictionary<uint, T> d) { return d == null ? 0 : d.Count; }

        // ================================================================== itemtype.txt
        // Values that come from ItemAdd.ini / the loader, not from the itemtype line: never overwritten.
        private static readonly HashSet<string> ItemKeep = new HashSet<string> { "Plus", "AllowUpgradePlus", "ID", "ParsedOk" };

        /// <summary>Re-parses itemtype.txt with DBItem.Parse and copies the new values into the EXISTING item TYPE objects
        /// (in place: every table that points at a type sees the new values). Item instances (players' items) are never
        /// touched. New item ids are reported but not added (needs a restart).</summary>
        public static void ReloadItems(ReloadResult result)
        {
            var items = Database.Server.ItemsBase;
            if (items == null || items.Count == 0)
                throw new HotReloadError("item types are not loaded yet.");
            byte[] data = HotReloadManager.ReadStable(HotReloadManager.DbFile("itemtype.txt"));
            var parsed = new List<Database.ItemType.DBItem>();
            var seen = new HashSet<uint>();
            int bad = 0;
            bool prev = Database.ItemType.DBItem.SkipStaticTables;
            Database.ItemType.DBItem.SkipStaticTables = true;// [ThreadStatic]: only this thread
            try
            {
                using (var reader = HotReloadManager.TextOf(data))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // same line filter as ItemType.Loading
                        string _item_ = line.Trim();
                        if (_item_.Length <= 11 || _item_.IndexOf("//", 0, 2) == 0) continue;
                        var item = new Database.ItemType.DBItem();
                        try { item.Parse(line); } catch { bad++; continue; }
                        if (!item.ParsedOk) { bad++; continue; }
                        if (!seen.Add(item.ID)) continue;// the loader keeps the first line of an id
                        parsed.Add(item);
                    }
                }
            }
            finally { Database.ItemType.DBItem.SkipStaticTables = prev; }

            if (parsed.Count < items.Count / 2)
                throw new HotReloadError("itemtype.txt gave only " + parsed.Count + " valid lines but the server has " + items.Count +
                                         " item types - refused (truncated or wrong file?). Nothing was changed.");

            FieldInfo[] fields = typeof(Database.ItemType.DBItem).GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !ItemKeep.Contains(f.Name) && !f.IsInitOnly).ToArray();
            int changed = 0, newIds = 0, fieldChanges = 0;
            var newList = new List<uint>();
            foreach (var item in parsed)
            {
                Database.ItemType.DBItem live;
                if (!items.TryGetValue(item.ID, out live) || live == null)
                {
                    newIds++;
                    if (newList.Count < 10) newList.Add(item.ID);
                    continue;
                }
                var diffs = new List<string>();
                foreach (FieldInfo f in fields)
                {
                    object a = f.GetValue(live), b = f.GetValue(item);
                    if (Equals(a, b)) continue;
                    f.SetValue(live, b);
                    if (diffs.Count < 4) diffs.Add(f.Name + " " + a + "->" + b);
                    fieldChanges++;
                }
                if (diffs.Count > 0)
                {
                    changed++;
                    result.Note(item.ID + " " + live.Name + ": " + string.Join(", ", diffs));
                }
            }
            result.Counts["changed"] = changed;
            result.Counts["fields"] = fieldChanges;
            result.Counts["newIds"] = newIds;
            result.Counts["badLines"] = bad;
            result.Message = changed == 0
                ? "No item type changes (" + parsed.Count + " types checked)."
                : "Applied live: " + changed + " item type(s) updated (" + fieldChanges + " values).";
            if (newIds > 0)
                result.Note(newIds + " new item id(s) in itemtype.txt (" + string.Join(", ", newList) + (newIds > newList.Count ? ", ..." : "") + ") - NEW item types need a server restart.");
            if (bad > 0)
                result.Note(bad + " line(s) could not be parsed and were ignored (the start-up loader skips them too).");
            result.Note("Only item TYPE values changed (prices, stats, names on the server). Players' items are untouched; the client shows its own itemtype data. A changed purification level / garment / mount / accessory position needs a restart.");
        }

        // ================================================================== Monsters\*.ini
        private static readonly string[] MonsterStats =
        {
            "Name", "Level", "MaxAttack", "MinAttack", "Defense", "Defense2", "Mesh", "MaxHealth", "AttackRange", "Dodge",
            "Boss", "AttackSpeed", "MoveSpeed", "SpellId", "ExtraCritical", "ExtraBreack", "extra_battlelev", "extra_exp", "extra_damage",
            "ViewRange", "DropBoots", "DropArmor", "DropShield", "DropWeapon", "DropArmet", "DropRing", "DropNecklace", "DropMoney",
            "DropHPItem", "DropMPItem", "DropSpecials"
        };

        /// <summary>Re-parses Monsters\*.ini (Server.ParseMonsterFamily) and updates the monster TYPES in place.
        /// A spawned monster that holds its own copy of the type gets each value that still equals the old type value
        /// (i.e. not tuned by an event); name/look/level show on its next respawn. New monster ids are added for future
        /// spawns. MonsterSpawns (where/how many) are NOT reloaded.</summary>
        public static void ReloadMonsters(ReloadResult result)
        {
            string folder = HotReloadManager.DbFile("Monsters\\");
            if (!Directory.Exists(folder))
                throw new HotReloadError("Monsters folder not found.");
            WaitFolderStable(folder);

            var parsed = new List<Game.MsgMonster.MonsterFamily>();
            var ini = new WindowsAPI.IniFile("");
            int invalid = 0;
            foreach (string fname in Directory.GetFiles(folder))
            {
                Game.MsgMonster.MonsterFamily fam;
                try { fam = Database.Server.ParseMonsterFamily(ini, fname); }
                catch (Exception e) { throw new HotReloadError(Path.GetFileName(fname) + " does not parse (" + e.Message + "). Nothing was changed."); }
                if (fam == null) { invalid++; continue; }
                parsed.Add(fam);
            }
            var current = Database.Server.MonsterFamilies;
            if (parsed.Count < current.Count / 2)
                throw new HotReloadError("only " + parsed.Count + " valid monster files but the server has " + current.Count + " types - refused. Nothing was changed.");

            var fields = MonsterStats.Select(n => typeof(Game.MsgMonster.MonsterFamily).GetField(n, BindingFlags.Public | BindingFlags.Instance)).Where(f => f != null).ToArray();
            var changedOld = new Dictionary<uint, object[]>();// id -> values before the update
            var added = new List<Game.MsgMonster.MonsterFamily>();
            var seen = new HashSet<uint>();
            foreach (var fam in parsed)
            {
                if (!seen.Add(fam.ID)) continue;// the loader keeps the first file of an id
                Game.MsgMonster.MonsterFamily live;
                if (!current.TryGetValue(fam.ID, out live) || live == null)
                {
                    added.Add(fam);
                    continue;
                }
                object[] old = fields.Select(f => f.GetValue(live)).ToArray();
                var diffs = new List<string>();
                for (int i = 0; i < fields.Length; i++)
                {
                    object b = fields[i].GetValue(fam);
                    if (Same(old[i], b)) continue;
                    fields[i].SetValue(live, b);
                    if (diffs.Count < 4) diffs.Add(fields[i].Name + " " + Show(old[i]) + "->" + Show(b));
                }
                if (diffs.Count == 0) continue;
                live.CreateMonsterSettings();
                changedOld[fam.ID] = old;
                result.Note(fam.ID + " " + live.Name + ": " + string.Join(", ", diffs));
            }
            if (added.Count > 0)
            {
                // publish a new dictionary (readers keep using the old one until they look again)
                var next = new Dictionary<uint, Game.MsgMonster.MonsterFamily>(current);
                foreach (var fam in added)
                {
                    fam.CreateItemGenerator();
                    fam.CreateMonsterSettings();
                    next[fam.ID] = fam;
                    result.Note("new monster type " + fam.ID + " " + fam.Name + " (usable by new spawns/commands)");
                }
                Database.Server.MonsterFamilies = next;
            }

            // spawned monsters
            int liveUpdated = 0;
            if (changedOld.Count > 0)
            {
                int iName = Array.IndexOf(MonsterStats, "Name"), iMesh = Array.IndexOf(MonsterStats, "Mesh"), iLevel = Array.IndexOf(MonsterStats, "Level");
                foreach (uint mapId in HotReloadManager.LoadedMapIds())
                {
                    var map = HotReloadManager.LoadedMap(mapId);
                    if (map == null || map.View == null) continue;
                    foreach (var obj in map.View.GetAllMapRoles(Role.MapObjectType.Monster).ToArray())
                    {
                        var mob = obj as Game.MsgMonster.MonsterRole;
                        if (mob == null || mob.Family == null) continue;
                        object[] old;
                        Game.MsgMonster.MonsterFamily master;
                        if (!changedOld.TryGetValue(mob.Family.ID, out old) || !Database.Server.MonsterFamilies.TryGetValue(mob.Family.ID, out master)) continue;
                        bool any = false;
                        if (!ReferenceEquals(mob.Family, master))
                        {
                            for (int i = 0; i < fields.Length; i++)
                            {
                                object mine = fields[i].GetValue(mob.Family);
                                object neu = fields[i].GetValue(master);
                                if (Same(mine, neu) || !Same(mine, old[i])) continue;// already new, or tuned by an event
                                fields[i].SetValue(mob.Family, neu);
                                any = true;
                            }
                        }
                        else
                            any = true;
                        // the role keeps these from spawn time; they show on the next respawn / to players entering view
                        if (mob.Name == (string)old[iName]) mob.Name = master.Name;
                        if (mob.Mesh == (ushort)old[iMesh]) mob.Mesh = master.Mesh;
                        if (mob.Level == (byte)(ushort)old[iLevel]) mob.Level = (byte)master.Level;
                        if (any) liveUpdated++;
                    }
                }
            }
            result.Counts["changedTypes"] = changedOld.Count;
            result.Counts["newTypes"] = added.Count;
            result.Counts["liveMonsters"] = liveUpdated;
            result.Counts["invalidFiles"] = invalid;
            result.Message = changedOld.Count == 0 && added.Count == 0
                ? "No monster type changes (" + parsed.Count + " files checked)."
                : "Applied live: " + changedOld.Count + " monster type(s) changed, " + added.Count + " new; " + liveUpdated + " spawned monster(s) updated (looks/HP show on their next respawn).";
            result.Note("MonsterSpawns (where / how many / respawn time) are NOT reloaded live: spawns are created at start-up and live spawns players may be fighting can't be removed safely. Spawn edits apply after a restart.");
        }

        private static bool Same(object a, object b)
        {
            var aa = a as Array;
            var bb = b as Array;
            if (aa != null && bb != null)
            {
                if (aa.Length != bb.Length) return false;
                for (int i = 0; i < aa.Length; i++)
                    if (!Equals(aa.GetValue(i), bb.GetValue(i)) && !SameDrop(aa.GetValue(i), bb.GetValue(i))) return false;
                return true;
            }
            return Equals(a, b);
        }

        private static bool SameDrop(object a, object b)
        {
            // SpecialItemWatcher = item id + MobRateWatcher(tick); the watcher's running count is state, not config
            if (!(a is Game.MsgMonster.SpecialItemWatcher) || !(b is Game.MsgMonster.SpecialItemWatcher)) return false;
            var x = (Game.MsgMonster.SpecialItemWatcher)a;
            var y = (Game.MsgMonster.SpecialItemWatcher)b;
            if (x.ID != y.ID) return false;
            if (x.Rate == null || y.Rate == null) return x.Rate == y.Rate;
            var tick = typeof(Game.MsgMonster.MobRateWatcher).GetField("tick", BindingFlags.NonPublic | BindingFlags.Instance);
            return tick == null || Equals(tick.GetValue(x.Rate), tick.GetValue(y.Rate));
        }

        private static string Show(object v)
        {
            var arr = v as Array;
            return arr != null ? "[" + arr.Length + "]" : (v == null ? "null" : v.ToString());
        }

        private static void WaitFolderStable(string folder)
        {
            for (int attempt = 0; attempt < 6; attempt++)
            {
                string a = FolderStamp(folder);
                System.Threading.Thread.Sleep(300);
                if (a == FolderStamp(folder)) return;
            }
            throw new HotReloadError("the Monsters folder is still being written - nothing was changed. Try again.");
        }

        private static string FolderStamp(string folder)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var f in new DirectoryInfo(folder).GetFiles())
                sb.Append(f.Name).Append(':').Append(f.Length).Append(':').Append(f.LastWriteTimeUtc.Ticks).Append(';');
            return sb.ToString();
        }

        // ================================================================== DMap cells
        /// <summary>Re-reads one map's DMap (with its map\walk file, like the start-up) and replaces its server-side
        /// walkability. Cells under NPCs / SobNpcs / statues keep their current state; item/monster/player bits are kept.
        /// The map size must not change. Clients still walk on their own client map file.</summary>
        public static void ReloadMapCells(ReloadResult result, uint mapId)
        {
            var map = HotReloadManager.LoadedMap(mapId);
            if (map == null)
                throw new HotReloadError("map " + mapId + " does not exist on the server.");
            uint fileId = map.BaseID != 0 ? map.BaseID : map.ID;
            string mapFile;
            if (!Role.GameMap.MapContents.TryGetValue((int)fileId, out mapFile))
                throw new HotReloadError("map " + mapId + " has no DMap file in gamemap.dat.");
            var grid = Role.GameMap.ReadGrid(mapFile, (int)fileId, map.FloorType != null);
            if (grid.Error != null)
                throw new HotReloadError("can't read " + mapFile + " (" + grid.Error.Message + ").");
            if (grid.Width != map.bounds.Width || grid.Height != map.bounds.Height)
                throw new HotReloadError("map " + mapId + " changed size (" + map.bounds.Width + "x" + map.bounds.Height + " -> " + grid.Width + "x" + grid.Height + ") - needs a restart.");

            var old = map.cells;
            int w = grid.Width, h = grid.Height;
            var cells = new Role.MapFlagType[w, h];
            const Role.MapFlagType keep = Role.MapFlagType.Item | Role.MapFlagType.Monster | Role.MapFlagType.Player;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    cells[x, y] = (grid.Cells[x, y] & Role.MapFlagType.Valid) | (old[x, y] & keep);
            // keep the current state of the 5x5 footprint of everything that blocks cells
            int kept = 0;
            foreach (var typ in new[] { Role.MapObjectType.Npc, Role.MapObjectType.SobNpc, Role.MapObjectType.StaticRole })
                foreach (var obj in map.View.GetAllMapRoles(typ).ToArray())
                {
                    for (int y = Math.Max(0, obj.Y - 2); y <= Math.Min(h - 1, obj.Y + 2); y++)
                        for (int x = Math.Max(0, obj.X - 2); x <= Math.Min(w - 1, obj.X + 2); x++)
                            cells[x, y] = old[x, y];
                    kept++;
                }
            int changed = 0;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if ((cells[x, y] & Role.MapFlagType.Valid) != (old[x, y] & Role.MapFlagType.Valid)) changed++;
            if (map.FloorType != null && grid.FloorType != null)
            {
                map.Altitude = grid.Altitude;
                map.FloorType = grid.FloorType;
            }
            map.TerrainWalkable = grid.Walkable;
            map.cells = cells;// one reference swap
            result.Counts["changedCells"] = changed;
            result.Counts["keptFootprints"] = kept;
            result.Message = "Applied live: map " + mapId + " walkability reloaded (" + changed + " cell(s) changed" + (grid.WalkOverride ? ", map\\walk data applied" : "") + ").";
            result.Note("Server-side only: the game client still uses its own map file - ship the patched client map so players can walk/see the change.");
        }
    }
}
