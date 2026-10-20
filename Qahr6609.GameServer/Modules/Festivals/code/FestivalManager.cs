// The running side: which festivals run now, start / end (NPCs, monsters, announcements), and the hooks the module
// forwards (login, item use, monster kills, NPC talk).
//   START: [Npc.*] put on their maps (players in view see them at once), [Spawn.*] monsters placed, [Announce] Start to
//          everyone, [Login] Give to the players online, script OnStart.
//   END:   script OnStop, monsters and NPCs removed, [Announce] End. A module reload (OnShutdown) removes everything
//          quietly and the new code puts it back.
// The schedule is checked every 10 s; a GM override (@festival on/off) wins over the dates.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.Festivals
{
    /// <summary>The code part of a festival (optional): a class in this module with the festival's folder name as Key.</summary>
    public abstract class FestivalScript
    {
        public abstract string Key { get; }
        public virtual void OnLoad(FestivalDef f) { }
        public virtual void OnStart(FestivalDef f) { }
        public virtual void OnStop(FestivalDef f) { }
        /// <summary>[Item.*] Script=name. Return true when the use went through (counts the item's Limit).</summary>
        public virtual bool OnItemUse(FestivalDef f, Client.GameClient c, MsgGameItem item, string name, ServerSockets.Packet stream) { return false; }
        /// <summary>A dialog option "script:name" (the NPC is next to the player).</summary>
        public virtual void OnAction(FestivalDef f, Client.GameClient c, string name, ServerSockets.Packet stream) { }
    }

    public static class FestivalManager
    {
        sealed class Run
        {
            public FestivalDef Def;
            public DateTime Started;
            public DateTime NextAnnounce;
            public readonly List<Game.MsgNpc.Npc> Npcs = new List<Game.MsgNpc.Npc>();
            public readonly List<Game.MsgMonster.MonsterRole> Mobs = new List<Game.MsgMonster.MonsterRole>();
        }

        static readonly object sync = new object();
        static Dictionary<string, FestivalDef> defs = new Dictionary<string, FestivalDef>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, Run> running = new Dictionary<string, Run>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, FestivalScript> scripts = new Dictionary<string, FestivalScript>(StringComparer.OrdinalIgnoreCase);
        static Action<string> log = s => { };

        public static IEnumerable<FestivalDef> Defs { get { lock (sync) return defs.Values.ToList(); } }
        public static FestivalDef Find(string key) { lock (sync) { FestivalDef d; return key != null && defs.TryGetValue(key, out d) ? d : null; } }
        public static bool IsRunning(string key) { lock (sync) return running.ContainsKey(key); }
        public static FestivalScript Script(string key) { lock (sync) { FestivalScript s; return scripts.TryGetValue(key, out s) ? s : null; } }

        /// <summary>A line in the module log (logs\yyyyMMdd.log).</summary>
        public static void Log(string line) { log(line); }

        static void Print(string line)
        {
            log(line);
            MyConsole.WriteLine("[Festivals] " + line, ConsoleColor.Cyan);
        }

        // ------------------------------------------------------------------ load
        /// <summary>Reads every festival folder (a folder of the data path with a festival.ini; "_name" = switched off).</summary>
        public static List<string> Load(string root, Action<string> logger)
        {
            log = logger ?? log;
            var lines = new List<string>();
            var fresh = new Dictionary<string, FestivalDef>(StringComparer.OrdinalIgnoreCase);
            foreach (string dir in Directory.GetDirectories(root).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                string name = Path.GetFileName(dir);
                if (name.StartsWith("_") || name.StartsWith(".") || !File.Exists(Path.Combine(dir, "festival.ini"))) continue;
                var f = FestivalDef.Load(dir);
                fresh[f.Key] = f;
                lines.Add(f.Key + ": " + (f.Valid ? "ok" : "INVALID") + ", " + f.Schedule?.Text + ", " + f.Npcs.Count + " NPC(s), " + f.Items.Count + " item rule(s), "
                    + f.Exchanges.Count + " exchange(s), " + f.Drops.Count + " drop(s), " + f.Spawns.Count + " spawn(s)");
                foreach (string e in f.Errors) lines.Add("  error: " + e);
                foreach (string w in f.Warnings) lines.Add("  warning: " + w);
            }
            lock (sync)
            {
                defs = fresh;
                scripts.Clear();
                foreach (var t in typeof(FestivalManager).Assembly.GetTypes())
                    if (t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(FestivalScript)))
                    {
                        var s = (FestivalScript)Activator.CreateInstance(t);
                        scripts[s.Key] = s;
                    }
            }
            foreach (var f in fresh.Values.Where(d => d.Valid))
            {
                var s = Script(f.Key);
                if (s == null) continue;
                try { s.OnLoad(f); }
                catch (Exception e) { f.Errors.Add("script OnLoad: " + e.Message); lines.Add(f.Key + ": script OnLoad failed: " + e.Message); }
            }
            foreach (var l in lines) Print(l);
            return lines;
        }

        /// <summary>Should it run now: a valid festival, the GM override, else Enabled + its dates.</summary>
        public static bool Wanted(FestivalDef f, DateTime now)
        {
            if (!f.Valid) return false;
            string o = FestivalStore.Override(f.Key);
            if (o == "on") return true;
            if (o == "off") return false;
            return f.Enabled && f.Schedule.IsDue(now);
        }

        /// <summary>Starts / stops what has to. quiet = no announcements (module load); quietKeys = the festivals that
        /// were running before a @festival reload (started again without a second announcement).</summary>
        public static void Evaluate(bool quiet, ICollection<string> quietKeys = null)
        {
            DateTime now = DateTime.Now;
            foreach (var f in Defs)
            {
                bool want = Wanted(f, now), on = IsRunning(f.Key);
                if (want && !on) Start(f, quiet || (quietKeys != null && quietKeys.Contains(f.Key)));
                else if (!want && on) Stop(f.Key, quiet);
            }
            List<Run> runs;
            lock (sync)
            {
                foreach (var k in running.Keys.Where(k => !defs.ContainsKey(k)).ToList()) Stop(k, quiet);
                runs = running.Values.ToList();
            }
            foreach (var r in runs)
                if (r.Def.EveryMinutes > 0 && r.Def.EveryText.Length > 0 && now >= r.NextAnnounce)
                {
                    r.NextAnnounce = now.AddMinutes(r.Def.EveryMinutes);
                    Broadcast(r.Def.EveryText, MsgMessage.ChatMode.TopLeft);
                }
        }

        public static void Broadcast(string text, MsgMessage.ChatMode mode)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            using (var rec = new ServerSockets.RecycledPacket())
                Program.SendGlobalPackets.Enqueue(new MsgMessage(text, MsgMessage.MsgColor.yellow, mode).GetArray(rec.GetStream()));
        }

        // ------------------------------------------------------------------ START / END
        static void Start(FestivalDef f, bool quiet)
        {
            var r = new Run { Def = f, Started = DateTime.Now, NextAnnounce = DateTime.Now.AddMinutes(Math.Max(1, f.EveryMinutes)) };
            lock (sync) { if (running.ContainsKey(f.Key)) return; running[f.Key] = r; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var n in f.Npcs)
                {
                    Role.GameMap map;
                    if (!Database.Server.ServerMaps.TryGetValue(n.Map, out map)) continue;
                    if (map.View.MapContain(Role.MapObjectType.Npc, n.Uid)) { Print(f.Key + ": NPC " + n.Uid + " is already on map " + n.Map + ", not placed"); continue; }
                    var np = Game.MsgNpc.Npc.Create();
                    np.UID = n.Uid;
                    np.NpcType = Role.Flags.NpcType.Talker;
                    np.Mesh = (ushort)(n.Look * 10 + n.Dir);
                    np.Map = n.Map;
                    np.X = n.X;
                    np.Y = n.Y;
                    map.AddNpc(np);
                    r.Npcs.Add(np);
                }
                foreach (var s in f.Spawns) SpawnMobs(r, s);
            }
            var script = Script(f.Key);
            if (script != null) try { script.OnStart(f); } catch (Exception e) { Print(f.Key + ": script OnStart: " + e.Message); }
            Print(f.Key + " started" + (quiet ? " (reload)" : "") + ": " + r.Npcs.Count + " NPC(s), " + r.Mobs.Count + " monster(s)");
            if (!quiet)
            {
                Broadcast(f.StartText, MsgMessage.ChatMode.Center);
                Broadcast(f.StartText, MsgMessage.ChatMode.System);
            }
            foreach (var c in Database.Server.GamePoll.Values.ToArray())
                if (c != null && c.Player != null && c.FullLoading) LoginGift(f, c);
        }

        static unsafe void SpawnMobs(Run r, FSpawn s)
        {
            Role.GameMap map;
            Game.MsgMonster.MonsterFamily fam;
            if (!Database.Server.ServerMaps.TryGetValue(s.Map, out map) || !Database.Server.MonsterFamilies.TryGetValue(s.Monster, out fam)) return;
            if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap()) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                for (int i = 0; i < s.Count; i++)
                {
                    var m = fam.Copy();
                    m.SpawnX = s.X; m.SpawnY = s.Y;
                    m.MaxSpawnX = (ushort)(s.X + s.W); m.MaxSpawnY = (ushort)(s.Y + s.H);
                    m.MapID = map.ID;
                    m.SpawnCount = 1;
                    var mob = map.MonstersColletion.Add(m, false, 0, true);// respawns normally while the festival runs
                    if (mob == null) continue;
                    r.Mobs.Add(mob);
                    mob.Send(mob.GetArray(stream, false));
                }
            }
        }

        static void Stop(string key, bool quiet)
        {
            Run r;
            lock (sync) { if (!running.TryGetValue(key, out r)) return; running.Remove(key); }
            var script = Script(key);
            if (script != null) try { script.OnStop(r.Def); } catch (Exception e) { Print(key + ": script OnStop: " + e.Message); }
            RemoveAll(r);
            Print(key + " stopped" + (quiet ? " (reload)" : ""));
            if (!quiet)
            {
                Broadcast(r.Def.EndText, MsgMessage.ChatMode.Center);
                Broadcast(r.Def.EndText, MsgMessage.ChatMode.System);
            }
        }

        static unsafe void RemoveAll(Run r)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var mob in r.Mobs)
                {
                    try
                    {
                        mob.RemoveOnDead = true;
                        var map = mob.GMap;
                        if (map == null) continue;
                        if (mob.Alive) map.SetMonsterOnTile(mob.X, mob.Y, false);
                        mob.HitPoints = 0;
                        var action = new ActionQuery { ObjId = mob.UID, Type = ActionType.RemoveEntity };
                        mob.Send(stream.ActionCreate(&action));
                        map.View.LeaveMap<Role.IMapObj>(mob);
                    }
                    catch (Exception e) { MyConsole.WriteException(e); }
                }
                r.Mobs.Clear();
                foreach (var np in r.Npcs)
                {
                    Role.GameMap map;
                    if (Database.Server.ServerMaps.TryGetValue(np.Map, out map))
                        try { map.RemoveNpc(np, stream); } catch (Exception e) { MyConsole.WriteException(e); }
                }
                r.Npcs.Clear();
            }
        }

        /// <summary>Module shutdown / reload: everything out of the world, no announcement. Returns what was running.</summary>
        public static List<string> StopAll()
        {
            List<string> keys;
            lock (sync) keys = running.Keys.ToList();
            foreach (var k in keys) Stop(k, true);
            return keys;
        }

        public static string Status(FestivalDef f)
        {
            Run r;
            lock (sync) running.TryGetValue(f.Key, out r);
            string o = FestivalStore.Override(f.Key);
            return f.Key + " (" + f.Name + "): " + (!f.Valid ? "INVALID" : r != null ? "RUNNING since " + r.Started.ToString("MM-dd HH:mm") + ", " + r.Npcs.Count + " NPC(s), " + r.Mobs.Count + " monster(s)" : "stopped")
                + (o != null ? " [GM " + o + "]" : !f.Enabled ? " [Enabled=0]" : "") + ", dates: " + f.Schedule?.Text;
        }

        // ------------------------------------------------------------------ hooks
        static List<FestivalDef> Running() { lock (sync) return running.Values.Select(r => r.Def).ToList(); }

        static bool LevelOk(Client.GameClient c, int min) { return c.Player.Level >= min || c.GameMaster || c.ProjectManager; }

        static void LoginGift(FestivalDef f, Client.GameClient c)
        {
            if (f.LoginGive.Count == 0 || c.Player.Level < f.LoginMinLevel) return;
            if (FestivalStore.Get(f, "login", c.Player.UID, "festival") > 0) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var list = FestivalRewards.Resolve(f, c, f.LoginGive, null);
                if (!FestivalRewards.HasRoom(c, list)) { c.SendSysMesage(f.Name + ": make room in your inventory and log in again for your festival gift."); return; }
                FestivalStore.Add(f, "login", c.Player.UID, "festival", 1);
                string got = FestivalRewards.Give(c, list, stream);
                c.SendSysMesage(string.IsNullOrEmpty(f.LoginGiveText) ? f.Name + ": you received " + got + "." : f.LoginGiveText, MsgMessage.ChatMode.System);
            }
        }

        public static void OnLogin(Client.GameClient c)
        {
            foreach (var f in Running())
            {
                if (!string.IsNullOrEmpty(f.LoginText) && c.Player.Level >= f.MinLevel) c.SendSysMesage(f.LoginText, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                LoginGift(f, c);
            }
        }

        /// <summary>The rule for this item: a running festival's, else an Always rule of any valid festival.</summary>
        static FItem FindItem(uint id, out FestivalDef owner)
        {
            foreach (var f in Running()) { FItem it; if (f.Items.TryGetValue(id, out it)) { owner = f; return it; } }
            foreach (var f in Defs) { FItem it; if (f.Valid && f.Items.TryGetValue(id, out it) && it.Always) { owner = f; return it; } }
            owner = null;
            return null;
        }

        /// <summary>True = the item belongs to a festival (the server's own item code does not run).</summary>
        public static bool OnItemUse(Client.GameClient c, MsgGameItem item)
        {
            if (c == null || c.Player == null || item == null) return false;
            FestivalDef f;
            var rule = FindItem(item.ITEM_ID, out f);
            if (rule == null)
            {
                // an item of a festival that is not running
                if (Defs.Any(d => d.Valid && d.Items.ContainsKey(item.ITEM_ID))) { c.SendSysMesage("This item can be used only during its festival."); return true; }
                return false;
            }
            int min = Math.Max(rule.MinLevel, f.MinLevel);
            if (!LevelOk(c, min)) { c.SendSysMesage("You need to be level " + min + " to use it."); return true; }
            string counter = rule.Counter;
            if (FestivalStore.Reached(f, counter, c.Player.UID, rule.Limit)) { c.SendSysMesage("You can use it " + rule.Limit + ". Come back " + (rule.Limit.Scope == "daily" ? "tomorrow" : "next time") + "."); return true; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!string.IsNullOrEmpty(rule.Script))
                {
                    var s = Script(f.Key);
                    if (s != null && s.OnItemUse(f, c, item, rule.Script, stream)) FestivalStore.Count(f, counter, c.Player.UID, rule.Limit);
                    return true;
                }
                var list = FestivalRewards.Resolve(f, c, rule.Give, rule.Give140);
                if (!FestivalRewards.HasRoom(c, list)) { c.SendSysMesage("Please make " + FestivalRewards.Slots(list) + " more space(s) in your inventory."); return true; }
                if (rule.Consume && !c.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream)) return true;
                FestivalStore.Count(f, counter, c.Player.UID, rule.Limit);
                string got = FestivalRewards.Give(c, list, stream);
                if (!string.IsNullOrEmpty(rule.Text)) c.SendSysMesage(rule.Text, MsgMessage.ChatMode.System);
                if (got.Length > 0) c.SendSysMesage("You received " + got + ".", MsgMessage.ChatMode.System);
                if (!string.IsNullOrEmpty(rule.Box)) c.CreateBoxDialog(rule.Box);
                log(c.Player.Name + " used " + Reward.ItemName(rule.Id) + ": " + got);
            }
            return true;
        }

        public static void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole mob)
        {
            if (killer == null || killer.Player == null || mob == null || mob.Family == null) return;
            foreach (var f in Running())
            {
                if (f.Drops.Count == 0 || killer.Player.Level < f.MinLevel) continue;
                foreach (var d in f.Drops)
                {
                    if ((d.Map != 0 && d.Map != mob.Map) || (d.Monster != 0 && d.Monster != mob.Family.ID) || !FestivalRewards.Chance(d.Chance)) continue;
                    string counter = "d:" + d.Name;
                    if (FestivalStore.Reached(f, counter, killer.Player.UID, d.Limit)) continue;
                    var list = FestivalRewards.Resolve(f, killer, d.Give, null);
                    if (!FestivalRewards.HasRoom(killer, list)) { killer.SendSysMesage("Your inventory is full: a festival prize was lost."); continue; }
                    FestivalStore.Count(f, counter, killer.Player.UID, d.Limit);
                    using (var rec = new ServerSockets.RecycledPacket())
                        killer.SendSysMesage("You found " + FestivalRewards.Give(killer, list, rec.GetStream()) + " (" + f.Name + ").", MsgMessage.ChatMode.System);
                }
            }
        }

        // ------------------------------------------------------------------ NPC dialogs
        sealed class Pending { public string Festival; public uint Npc; public Dictionary<byte, string> Actions = new Dictionary<byte, string>(); }
        static readonly Dictionary<uint, Pending> pending = new Dictionary<uint, Pending>();
        const byte FirstOption = 200, LastOption = 249;

        static FestivalDef OwnerOf(uint npcId, out FNpc npc)
        {
            foreach (var f in Running())
                foreach (var n in f.Npcs)
                    if (n.Uid == npcId) { npc = n; return f; }
            npc = null;
            return null;
        }

        public static string T(string s) { return (s ?? "").Replace(' ', '~'); }

        /// <summary>A dialog: text + options (text, action). Close is added when none of the options is "close".</summary>
        public static void Show(FestivalDef f, Client.GameClient c, ServerSockets.Packet stream, uint npcId, string text, IList<FOption> options)
        {
            var p = new Pending { Festival = f.Key, Npc = npcId };
            var dlg = new Game.MsgNpc.Dialog(c, stream);
            dlg.AddText(T(text));
            byte id = FirstOption;
            foreach (var o in options)
            {
                if (o.Action == "close") { dlg.AddOption(o.Text, 255); continue; }
                if (id > LastOption) break;
                p.Actions[id] = o.Action;
                dlg.AddOption(o.Text, id++);
            }
            if (!options.Any(o => o.Action == "close")) dlg.AddOption("I see.", 255);
            dlg.AddAvatar(0).FinalizeDialog();
            lock (pending) pending[c.Player.UID] = p;
        }

        public static void ShowDialog(FestivalDef f, Client.GameClient c, ServerSockets.Packet stream, uint npcId, string name)
        {
            FDialog d;
            if (f.Dialogs.TryGetValue(name, out d)) Show(f, c, stream, npcId, d.Text, d.Options);
        }

        /// <summary>Module NPC hook: 0 = not a festival NPC, 1 = answered.</summary>
        public static int OnNpcTalk(Client.GameClient c, ServerSockets.Packet stream, byte option, uint npcId)
        {
            FNpc npc;
            var f = OwnerOf(npcId, out npc);
            if (f == null) return 0;
            if (option == 255) return 1;
            if (!LevelOk(c, f.MinLevel))
            {
                Show(f, c, stream, npcId, "The " + f.Name + " is for heroes of level " + f.MinLevel + " and above. Come back when you are stronger.", new List<FOption>());
                return 1;
            }
            string action = null;
            if (option != 0)
                lock (pending)
                {
                    Pending p;
                    if (pending.TryGetValue(c.Player.UID, out p) && p.Npc == npcId && p.Festival == f.Key) p.Actions.TryGetValue(option, out action);
                }
            if (action == null) { ShowDialog(f, c, stream, npcId, npc.Dialog); return 1; }
            Do(f, c, stream, npcId, action);
            return 1;
        }

        static void Do(FestivalDef f, Client.GameClient c, ServerSockets.Packet stream, uint npcId, string action)
        {
            int colon = action.IndexOf(':');
            string kind = colon > 0 ? action.Substring(0, colon) : action, arg = colon > 0 ? action.Substring(colon + 1) : "";
            switch (kind)
            {
                case "dialog": ShowDialog(f, c, stream, npcId, arg); break;
                case "exchange": Exchange(f, c, stream, npcId, arg); break;
                case "teleport":
                    {
                        string[] p = arg.Split(',');
                        c.Teleport(ushort.Parse(p[1]), ushort.Parse(p[2]), uint.Parse(p[0]));
                        break;
                    }
                case "script":
                    {
                        var s = Script(f.Key);
                        if (s != null) s.OnAction(f, c, arg, stream);
                        break;
                    }
            }
        }

        /// <summary>[Exchange.X]: level, limit, cooldown, cost, bag space; cost taken, chance rolled, prize given; the
        /// answer offers the same exchange again.</summary>
        static void Exchange(FestivalDef f, Client.GameClient c, ServerSockets.Packet stream, uint npcId, string name)
        {
            FExchange x;
            if (!f.Exchanges.TryGetValue(name, out x)) return;
            var again = new List<FOption> { new FOption { Text = "Again.", Action = "exchange:" + name }, new FOption { Text = "That's all.", Action = "close" } };
            var ok = new List<FOption> { new FOption { Text = "I see.", Action = "close" } };
            string counter = "x:" + name;
            if (!LevelOk(c, x.MinLevel)) { Show(f, c, stream, npcId, "You need to be level " + x.MinLevel + ".", ok); return; }
            if (FestivalStore.Reached(f, counter, c.Player.UID, x.Limit)) { Show(f, c, stream, npcId, "You can do it only " + x.Limit + ". Come back " + (x.Limit.Scope == "daily" ? "tomorrow" : "next time") + ".", ok); return; }
            int wait = FestivalStore.CooldownLeft(f, counter, c.Player.UID);
            if (wait > 0) { Show(f, c, stream, npcId, "Not so fast! Wait " + wait + " more second(s).", again); return; }
            if (!FestivalRewards.CanPay(c, x.Cost)) { Show(f, c, stream, npcId, "You need " + FestivalRewards.Describe(x.Cost) + ".", ok); return; }
            bool win = FestivalRewards.Chance(x.Chance);
            var list = FestivalRewards.Resolve(f, c, win ? x.Give : x.FailGive, win ? x.Give140 : null);
            if (!FestivalRewards.HasRoom(c, list)) { Show(f, c, stream, npcId, "Please make " + Math.Max(1, FestivalRewards.Slots(list)) + " more space(s) in your inventory.", ok); return; }
            FestivalRewards.Take(c, x.Cost, stream);
            FestivalStore.Count(f, counter, c.Player.UID, x.Limit);
            FestivalStore.StartCooldown(f, counter, c.Player.UID, x.Cooldown);
            string got = FestivalRewards.Give(c, list, stream);
            string text = win ? (string.IsNullOrEmpty(x.Text) ? "Done!" : x.Text) : (string.IsNullOrEmpty(x.FailText) ? "No luck this time." : x.FailText);
            if (got.Length > 0) { text += " You received " + got + "."; c.SendSysMesage("You received " + got + ".", MsgMessage.ChatMode.System); }
            if (!x.Limit.None) text += " (" + FestivalStore.Get(f, counter, c.Player.UID, x.Limit.Scope) + "/" + x.Limit.Max + (x.Limit.Scope == "daily" ? " today" : "") + ")";
            log(c.Player.Name + " exchange " + f.Key + "." + name + (win ? "" : " (failed)") + ": " + got);
            Show(f, c, stream, npcId, text, FestivalStore.Reached(f, counter, c.Player.UID, x.Limit) ? ok : again);
        }
    }
}
