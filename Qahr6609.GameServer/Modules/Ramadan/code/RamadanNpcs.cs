// The Legend of the Last Crescent - NPCs: spawn/despawn, decor and every dialog.
// NPC packets reach these handlers through the normal NpcAttribute dispatch (Game/MsgNpc/Procesor.cs),
// which already checks that the NPC is on the player's screen; every handler re-checks map, distance and instance.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.Ramadan
{
    /// <summary>Thin NpcAttribute entry points (one per NPC UID).</summary>
    public static class RamadanNpcs
    {
        [NpcAttribute(RamadanNpcIds.RamadanGuardian)]
        public static void Guardian(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Guardian", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanLamplighter)]
        public static void Lamplighter(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Lamplighter", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanScorchedLantern)]
        public static void ScorchedLantern(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("ScorchedLantern", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanCaptainRashid)]
        public static void Rashid(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Rashid", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanSageNoor)]
        public static void Noor(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Noor", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanMasterKareem)]
        public static void Kareem(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Kareem", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanWidowSalma)]
        public static void Salma(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Salma", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanNightWatchman)]
        public static void Watchman(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Watchman", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanCaravanMaster)]
        public static void CaravanMaster(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("CaravanMaster", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanGateKeeper)]
        public static void GateKeeper(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("GateKeeper", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanCrescentMerchant)]
        public static void Merchant(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Merchant", c, s, o, i); }
        [NpcAttribute(RamadanNpcIds.RamadanCrescentHerald)]
        public static void Herald(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { RamadanEvent.HandleNpc("Herald", c, s, o, i); }
    }

    public static unsafe partial class RamadanEvent
    {
        // ------------------------------------------------------------------ NPC table
        static NpcSpot Def(string key, uint uid, ushort look, uint map, ushort x, ushort y)
        {
            return new NpcSpot { Key = key, UID = uid, LookType = look, Dir = 0, At = new MapPoint(map, x, y) };
        }

        /// <summary>Default places. LookType = NpcType section added to the client's ini\npc.ini (6001..6012).</summary>
        public static readonly NpcSpot[] NpcDefaults =
        {
            Def("Guardian",        (uint)RamadanNpcIds.RamadanGuardian,         6001, 1002, 239, 246),
            Def("Lamplighter",     (uint)RamadanNpcIds.RamadanLamplighter,      6002, 1036, 213, 197),
            Def("ScorchedLantern", (uint)RamadanNpcIds.RamadanScorchedLantern,  4990, 1011, 234, 259),
            Def("Rashid",          (uint)RamadanNpcIds.RamadanCaptainRashid,    6004, 1002, 381, 254),
            Def("Noor",            (uint)RamadanNpcIds.RamadanSageNoor,         6005, 1011, 215, 202),
            Def("Kareem",          (uint)RamadanNpcIds.RamadanMasterKareem,     6006, 1020, 567, 566),
            Def("Salma",           (uint)RamadanNpcIds.RamadanWidowSalma,       6007, 1000, 478, 636),
            Def("Watchman",        (uint)RamadanNpcIds.RamadanNightWatchman,    6008, 1015, 719, 575),
            Def("CaravanMaster",   (uint)RamadanNpcIds.RamadanCaravanMaster,    6009, 1002, 424, 374),
            Def("GateKeeper",      (uint)RamadanNpcIds.RamadanGateKeeper,       6010, 1002, 241, 243),
            Def("Merchant",        (uint)RamadanNpcIds.RamadanCrescentMerchant, 6011, 1002, 232, 252),
            Def("Herald",          (uint)RamadanNpcIds.RamadanCrescentHerald,   6012, 1002, 238, 246),
        };

        static readonly Dictionary<string, ushort> Avatars = new Dictionary<string, ushort>(StringComparer.OrdinalIgnoreCase)
        {
            { "Guardian", 63 }, { "Lamplighter", 7 }, { "ScorchedLantern", 0 }, { "Rashid", 10 }, { "Noor", 31 }, { "Kareem", 23 },
            { "Salma", 14 }, { "Watchman", 5 }, { "CaravanMaster", 110 }, { "GateKeeper", 237 }, { "Merchant", 67 }, { "Herald", 6 },
        };

        public static NpcSpot Spot(string key)
        {
            NpcSpot s;
            if (Config.Npcs.TryGetValue(key, out s)) return s;
            return NpcDefaults.First(n => n.Key == key);
        }

        // ------------------------------------------------------------------ spawn / despawn
        static readonly object NpcSync = new object();
        static readonly List<Game.MsgNpc.Npc> spawnedNpcs = new List<Game.MsgNpc.Npc>();
        static bool npcsSpawned;

        static void SyncNpcs()
        {
            bool want = NpcsUp;
            if (want == npcsSpawned) return;
            if (want) SpawnNpcs(); else DespawnNpcs();
        }

        public static void SpawnNpcs()
        {
            lock (NpcSync)
            {
                DespawnNpcsLocked();
                foreach (var spot in Config.Npcs.Values)
                    PlaceNpc(spot.UID, spot.LookType, spot.Dir, spot.At.Map, spot.At.X, spot.At.Y);
                List<DecorItem> decor;
                lock (Decor) decor = Decor.ToList();
                for (int i = 0; i < decor.Count; i++)
                {
                    var d = decor[i];
                    PlaceNpc(DecorUidBase + (uint)i, DecorLook(d.Type), d.Dir, d.Map, d.X, d.Y);
                }
                RebuildBlockedCells(decor);
                npcsSpawned = true;
            }
        }

        public static void DespawnNpcs()
        {
            lock (NpcSync) { DespawnNpcsLocked(); npcsSpawned = false; blockedCells = new HashSet<ulong>(); }
        }

        static void DespawnNpcsLocked()
        {
            if (spawnedNpcs.Count == 0) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var np in spawnedNpcs)
                {
                    Role.GameMap map;
                    if (Database.Server.ServerMaps.TryGetValue(np.Map, out map))
                        try { map.RemoveNpc(np, stream); } catch (Exception e) { MyConsole.WriteException(e); }
                }
            }
            spawnedNpcs.Clear();
        }

        // ------------------------------------------------------------------ solid mosque
        // The mosque is a building: players can not walk or jump into it. Cells with |dx| <= MosqueBlockRadius and
        // |dy| <= MosqueBlockRadius around each mosque decor are blocked (the model is aligned with the map grid).
        static volatile HashSet<ulong> blockedCells = new HashSet<ulong>();
        static ulong CellKey(uint map, int x, int y) { return ((ulong)map << 32) | ((ulong)(ushort)x << 16) | (ushort)y; }

        static void RebuildBlockedCells(List<DecorItem> decor)
        {
            var set = new HashSet<ulong>();
            int r = Config.MosqueBlockRadius;
            if (r > 0)
                foreach (var d in decor)
                {
                    if (DecorLook(d.Type) != Config.DecorMosqueType) continue;
                    for (int dx = -r; dx <= r; dx++)
                        for (int dy = -r; dy <= r; dy++)
                            set.Add(CellKey(d.Map, d.X + dx, d.Y + dy));
                }
            blockedCells = set;
        }

        /// <summary>Hook (walk/jump): true = the target cell is inside a mosque, the player was pulled back.
        /// Also drives the Crescent Lantern (warm / cold) on every step.</summary>
        public static bool BlockMove(Client.GameClient client, int x, int y)
        {
            try
            {
                if (client == null || client.Player == null) return false;
                var set = blockedCells;
                if (set.Count > 0 && client.Player.DynamicID == 0 && set.Contains(CellKey(client.Player.Map, x, y)))
                {
                    client.Pullback();
                    return true;
                }
                LanternOnMove(client, x, y);
                return false;
            }
            catch (Exception e) { MyConsole.WriteException(e); return false; }
        }

        static void PlaceNpc(uint uid, ushort look, byte dir, uint mapId, ushort x, ushort y)
        {
            Role.GameMap map;
            if (!Database.Server.ServerMaps.TryGetValue(mapId, out map)) return;
            if (map.View.MapContain(Role.MapObjectType.Npc, uid))
            {
                Store.Log(0, "", "npc-uid-in-use", uid + " already on map " + mapId);
                return;
            }
            var np = Game.MsgNpc.Npc.Create();
            np.UID = uid;
            np.NpcType = Role.Flags.NpcType.Talker;
            np.Mesh = (ushort)(look * 10 + (dir % 8));
            np.Map = mapId;
            np.X = x;
            np.Y = y;
            map.AddNpc(np);
            spawnedNpcs.Add(np);
        }

        // ------------------------------------------------------------------ decor (decor.txt: type,map,x,y,dir)
        public class DecorItem { public string Type; public uint Map; public ushort X, Y; public byte Dir; }
        public static readonly List<DecorItem> Decor = new List<DecorItem>();
        const uint DecorUidBase = 8155200;
        static string DecorPath { get { return Path.Combine(Root, "decor.txt"); } }
        public static readonly string[] DecorTypes = { "mosque", "lantern", "set", "date" };

        static ushort DecorLook(string type)
        {
            ushort raw;
            if (ushort.TryParse(type, out raw) && raw >= 1000) return raw; // numeric client npc type (testing)
            switch (type)
            {
                case "mosque": return Config.DecorMosqueType;
                case "lantern": return Config.DecorLanternType;
                case "set": return Config.DecorSetType;
                default: return Config.DecorDateType;
            }
        }

        static void LoadDecor()
        {
            lock (Decor)
            {
                Decor.Clear();
                if (!File.Exists(DecorPath))
                {
                    File.WriteAllText(DecorPath, "# Ramadan decor placed on the map as client effect NPCs.\r\n# type,map,x,y,dir   type = mosque | lantern | set | date\r\n# Add in game with: @ramadan decor add lantern   (at your position)\r\n# The mosque is ~900 units wide - place it on open ground only.\r\nmosque,1002,229,243,0\r\nlantern,1036,211,199,0\r\nlantern,1036,215,195,0\r\nset,1000,476,638,0\r\ndate,1002,229,251,0\r\n", new UTF8Encoding(false));
                }
                foreach (var raw in File.ReadAllLines(DecorPath, Encoding.UTF8))
                {
                    var line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("#")) continue;
                    var p = line.Split(',');
                    uint map; ushort x, y; byte dir = 0;
                    ushort rawType;
                    if (p.Length >= 4 && (DecorTypes.Contains(p[0].Trim().ToLowerInvariant()) || (ushort.TryParse(p[0].Trim(), out rawType) && rawType >= 1000)) && uint.TryParse(p[1], out map) && ushort.TryParse(p[2], out x) && ushort.TryParse(p[3], out y)
                        && (p.Length < 5 || byte.TryParse(p[4], out dir)) && Decor.Count < 100)
                        Decor.Add(new DecorItem { Type = p[0].Trim().ToLowerInvariant(), Map = map, X = x, Y = y, Dir = (byte)(dir % 8) });
                    else Config.Errors.Add("bad decor line: " + line);
                }
            }
        }

        static void SaveDecor()
        {
            var sb = new StringBuilder();
            sb.Append("# type,map,x,y,dir   type = mosque | lantern | set | date\r\n");
            lock (Decor) foreach (var d in Decor) sb.Append(d.Type + "," + d.Map + "," + d.X + "," + d.Y + "," + d.Dir + "\r\n");
            RamadanStore.AtomicWrite(DecorPath, sb.ToString());
        }

        // ------------------------------------------------------------------ dialog helpers
        public struct Opt
        {
            public string Text; public byte Id;
            public Opt(string text, byte id) { Text = text; Id = id; }
        }

        static IEnumerable<string> Chunks(string text, int max)
        {
            foreach (var para in text.Split('\n'))
            {
                var rest = para.Trim();
                while (rest.Length > max)
                {
                    int cut = rest.LastIndexOf(' ', max);
                    if (cut < max / 2) cut = max;
                    yield return rest.Substring(0, cut);
                    rest = rest.Substring(cut).TrimStart();
                }
                if (rest.Length > 0) yield return rest;
            }
        }

        static void Say(Client.GameClient c, ServerSockets.Packet stream, string npc, string text, params Opt[] opts)
        {
            var d = new Dialog(c, stream);
            foreach (var chunk in Chunks(text, 200)) d.AddText(chunk + " ");
            foreach (var o in opts) d.AddOption(o.Text, o.Id);
            if (!opts.Any(o => o.Id == 255)) d.AddOption("Farewell.", 255);
            ushort av; Avatars.TryGetValue(npc, out av);
            d.AddAvatar(av).FinalizeDialog();
        }

        static bool NearNpc(Client.GameClient c, string key)
        {
            var spot = Spot(key);
            return c.Player.Map == spot.At.Map && c.Player.DynamicID == 0
                && Role.Core.GetDistance(c.Player.X, c.Player.Y, spot.At.X, spot.At.Y) <= 18;
        }

        /// <summary>Entry point of every Ramadan NPC.</summary>
        public static void HandleNpc(string key, Client.GameClient c, ServerSockets.Packet stream, byte option, string input)
        {
            try
            {
                if (!NpcsUp || c == null || c.Player == null || c.Fake) return;
                if (!NearNpc(c, key)) return;
                if (TryTravel(c, option)) return;
                if (key == "Merchant") { MerchantNpc(c, stream, option); return; }
                if (key == "Herald") { HeraldNpc(c, stream, option); return; }
                if (!Active)
                {
                    Say(c, stream, key, "The last crescent has set. The story rests until next Ramadan, but the Crescent Bazaar stays open for a few more nights.");
                    return;
                }
                var s = GetState(c);
                if (s == null) return;
                lock (s.Sync) RamadanRules.EnsureDaily(Config, s, DateTime.Now);
                // The Crescent Lantern quest is the story now. Guardian options 20+ are the nightly tasks.
                if (key == "Guardian" && option < 20) { RetryEarnedRewards(c, s); LanternGuardian(c, s, stream, option); return; }
                if (LanternWitness(key, c, s, stream, option)) return;
                switch (key)
                {
                    case "Guardian": GuardianNpc(c, s, stream, option); break;
                    case "Lamplighter": LamplighterNpc(c, s, stream, option); break;
                    case "ScorchedLantern": LanternNpc(c, s, stream, option); break;
                    case "Rashid": RashidNpc(c, s, stream, option); break;
                    case "Noor": NoorNpc(c, s, stream, option); break;
                    case "Kareem": KareemNpc(c, s, stream, option); break;
                    case "Salma": SalmaNpc(c, s, stream, option); break;
                    case "Watchman": WatchmanNpc(c, s, stream, option); break;
                    case "CaravanMaster": CaravanMasterNpc(c, s, stream, option); break;
                    case "GateKeeper": GateKeeperNpc(c, s, stream, option); break;
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        // ------------------------------------------------------------------ "take me there" (no coordinates for players to follow)
        static readonly string[] TravelKeys = { "Guardian", "Lamplighter", "ScorchedLantern", "Scouts", "Rashid", "Noor", "Kareem", "Salma", "Watchman", "CaravanMaster", "GateKeeper", "Merchant", "Caravan" };
        const byte TravelBase = 100;

        static Opt Go(string key, string label)
        {
            return new Opt(label, (byte)(TravelBase + Array.IndexOf(TravelKeys, key)));
        }

        /// <summary>Options 100+ walk the player to the NPC (or the scouts' camp): the client runs and jumps there by itself.</summary>
        static bool TryTravel(Client.GameClient c, byte option)
        {
            if (option < TravelBase || option >= TravelBase + TravelKeys.Length) return false;
            string key = TravelKeys[option - TravelBase];
            MapPoint p; uint npcUid = 0; string label;
            if (key == "Scouts") { if (Config.ScoutSpawns.Count == 0) return true; p = Config.ScoutSpawns[0]; label = "the Eclipse Scouts"; }
            else if (key == "Caravan") { p = Config.CaravanAt; label = "the Caravan of Light"; }
            else { var spot = Spot(key); p = spot.At; npcUid = spot.UID; label = NpcLabel(key); }
            if (Config.WalkToNpc) { StartGuide(c, p, npcUid, label); return true; }
            if (!c.Player.Alive || c.Player.DynamicID != 0) return true;
            Role.GameMap map;
            if (!Database.Server.ServerMaps.TryGetValue(p.Map, out map)) return true;
            int[][] offs = { new[] { 2, 2 }, new[] { 1, 2 }, new[] { 2, 1 }, new[] { -2, 2 }, new[] { 2, -2 }, new[] { 0, 3 }, new[] { 3, 0 }, new[] { -2, -2 }, new[] { 1, 1 }, new[] { 0, 0 } };
            foreach (var o in offs)
            {
                int x = p.X + o[0], y = p.Y + o[1];
                if (x <= 0 || y <= 0 || !map.ValidLocation((ushort)x, (ushort)y)) continue;
                c.Teleport((ushort)x, (ushort)y, p.Map);
                return true;
            }
            c.Teleport(p.X, p.Y, p.Map);
            return true;
        }

        // ------------------------------------------------------------------ Guardian of the Crescent
        static void GuardianNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            QuestStage stage;
            lock (s.Sync) stage = s.Stage;
            switch (option)
            {
                case 1: // accept the quest
                    {
                        bool ok;
                        lock (s.Sync) ok = RamadanRules.Advance(s, QuestStage.Introduction);
                        if (!ok) break;
                        Store.Log(s.UID, s.Name, "stage", "Introduction");
                        Stamp(c, s, Stamps.Intro);
                        Grant(c, s, "intro", "Intro", "The Guardian's blessing");
                        Say(c, stream, "Guardian", "Then the first lantern is lit. Every night of this month you may also help me with small tasks; ask me about tonight's tasks whenever you pass by. When you are ready, ask me where the light was lost.",
                            new Opt("Where was the light lost?", 2));
                        return;
                    }
                case 2: // begin the clue hunt
                    {
                        bool ok;
                        lock (s.Sync) ok = RamadanRules.Advance(s, QuestStage.ClueHunt);
                        if (!ok) break;
                        Store.Log(s.UID, s.Name, "stage", "ClueHunt");
                        Say(c, stream, "Guardian", "On the night the Last Crescent fell dark, the Old Lamplighter of the Market was the last to see it burn. Start with him. Follow what he tells you, and you will find three clues: his story, a scorched lantern, and a letter carried by the shadows.", Go("Lamplighter", "Take me to the Old Lamplighter."));
                        return;
                    }
                case 3: // hand in the clues
                    {
                        bool ok;
                        lock (s.Sync) ok = RamadanRules.Advance(s, QuestStage.SealChallenges);
                        if (!ok) break;
                        Store.Log(s.UID, s.Name, "stage", "SealChallenges");
                        Grant(c, s, "clues", "Clues", "The three clues");
                        Say(c, stream, "Guardian", "The letter is signed with a black sun: the Eclipse Sovereign. Only a heart sealed with four virtues can open the Forgotten Gate. Earn the Seal of Courage from Captain Rashid, the Seal of Wisdom from Sage Noor, the Seal of Unity from Master Kareem and the Seal of Trust from Widow Salma.");
                        return;
                    }
                case 4: // four seals
                    {
                        bool ok;
                        lock (s.Sync) ok = RamadanRules.Advance(s, QuestStage.CommunityEvent);
                        if (!ok) break;
                        Store.Log(s.UID, s.Name, "stage", "CommunityEvent");
                        Say(c, stream, "Guardian", "Four seals shine on your hands. One thing is missing: the light belongs to everyone. Defend a Caravan of Light with the people of this land, then come back to me. The Caravan Master in Twin City knows when the next one leaves.");
                        return;
                    }
                case 5: // caravan defended -> gate
                    {
                        bool ok;
                        lock (s.Sync) ok = RamadanRules.Advance(s, QuestStage.FinalGateUnlocked);
                        if (!ok) break;
                        Store.Log(s.UID, s.Name, "stage", "FinalGateUnlocked");
                        Stamp(c, s, Stamps.GateOpened);
                        Grant(c, s, "gate", "GateUnlocked", "The Forgotten Gate opens");
                        Say(c, stream, "Guardian", "The Forgotten Gate answers you. The Gate Keeper will open it. Go alone or bring up to " + Config.BossMaxTeam + " friends: the Eclipse Sovereign waits inside.");
                        return;
                    }
                case 6: // story complete
                    {
                        bool ok;
                        lock (s.Sync) ok = RamadanRules.Advance(s, QuestStage.Completed);
                        if (!ok) break;
                        Store.Log(s.UID, s.Name, "stage", "Completed");
                        Stamp(c, s, Stamps.StoryComplete);
                        Grant(c, s, "story", "StoryComplete", "The Last Crescent shines again");
                        Say(c, stream, "Guardian", "Look up. The Last Crescent burns again, and its light is yours to share. Keep lighting lanterns every night of this month; the sky remembers every one.");
                        return;
                    }
                case 20: DailyBoard(c, s, stream); return;
                case 21: // take the iftar basket
                    {
                        bool ok;
                        lock (s.Sync)
                        {
                            int i = Array.IndexOf(s.DailyTasks, (int)DailyTaskType.IftarErrand);
                            ok = i >= 0 && (s.DailyDoneMask & (1 << i)) == 0 && s.ErrandStep == 0;
                            if (ok) { s.ErrandStep = 1; s.Dirty = true; }
                        }
                        if (ok) Say(c, stream, "Guardian", "Here, the iftar basket: dates, bread and warm soup. The Old Lamplighter in the Market has been working since dawn. Bring it to him before the night is over.", Go("Lamplighter", "Take me to the Old Lamplighter."));
                        else DailyBoard(c, s, stream);
                        return;
                    }
                case 22: AskRiddle(c, s, stream, "Guardian", "daily", 41); return;
                case 41: case 42: case 43: case 44: AnswerRiddle(c, s, stream, "Guardian", "daily", option - 41); return;
                case 23: // retry daily claims (e.g. after freeing bag space)
                    {
                        for (int i = 0; i < 3; i++) CompleteDaily(c, s, i);
                        DailyBoard(c, s, stream);
                        return;
                    }
                case 30: StampBook(c, s, stream); return;
                case 31: CheckMilestones(c, s); StampBook(c, s, stream); return;
            }

            // main page, by stage - the story text changes with every stage
            if (option == 0) RetryEarnedRewards(c, s);
            string caravanText = NextCaravanText();   // takes CaravanSync: never under the player lock
            var opts = new List<Opt>();
            string text;
            lock (s.Sync)
            {
                stage = s.Stage;
                switch (stage)
                {
                    case QuestStage.NotStarted:
                        text = "Peace be upon you, traveller. Every Ramadan the Last Crescent lights the sky over these lands. This year it rose dark: someone has stolen its light. Will you help me bring it back?";
                        opts.Add(new Opt("I will help you, Guardian.", 1));
                        break;
                    case QuestStage.Introduction:
                        text = "You carry the first lantern now. The light was not lost by accident; it was taken.";
                        opts.Add(new Opt("Where was the light lost?", 2));
                        break;
                    case QuestStage.ClueHunt:
                        text = "Clues found: " + Mark(s.Clues, ClueFlags.Lamplighter) + " the Lamplighter's story, " + Mark(s.Clues, ClueFlags.ScorchedLantern) + " the scorched lantern, " + Mark(s.Clues, ClueFlags.ShadowLetter) + " the Shadow Letter. ";
                        if ((s.Clues & ClueFlags.All) == ClueFlags.All) { text += "You found all three. Show me."; opts.Add(new Opt("Here are the three clues.", 3)); }
                        else if ((s.Clues & ClueFlags.Lamplighter) == 0) { text += "Speak with the Old Lamplighter in the Market."; opts.Add(Go("Lamplighter", "Take me to the Old Lamplighter.")); }
                        else if ((s.Clues & ClueFlags.ScorchedLantern) == 0) { text += "Search for the scorched lantern near Phoenix Castle."; opts.Add(Go("ScorchedLantern", "Take me to the scorched lantern.")); }
                        else { text += "Eclipse Scouts lurk on the plains east of Twin City. One of them carries the Shadow Letter."; opts.Add(Go("Scouts", "Take me to the Eclipse Scouts.")); }
                        break;
                    case QuestStage.SealChallenges:
                        text = "Seals: " + Mark(s.Seals, SealFlags.Courage) + " Courage (Captain Rashid, Twin City), " + Mark(s.Seals, SealFlags.Wisdom) + " Wisdom (Sage Noor, Phoenix Castle), "
                            + Mark(s.Seals, SealFlags.Unity) + " Unity (Master Kareem, Ape City), " + Mark(s.Seals, SealFlags.Trust) + " Trust (Widow Salma, Desert City).";
                        if ((s.Seals & SealFlags.All) == SealFlags.All) { text += " All four seals are yours!"; opts.Add(new Opt("I carry all four seals.", 4)); }
                        else
                        {
                            if ((s.Seals & SealFlags.Courage) == 0) opts.Add(Go("Rashid", "Take me to Captain Rashid (Courage)."));
                            if ((s.Seals & SealFlags.Wisdom) == 0) opts.Add(Go("Noor", "Take me to Sage Noor (Wisdom)."));
                            if ((s.Seals & SealFlags.Unity) == 0) opts.Add(Go("Kareem", "Take me to Master Kareem (Unity)."));
                            if ((s.Seals & SealFlags.Trust) == 0) opts.Add(s.TrustStep == 1 ? Go("Watchman", "Take me to the Night Watchman (Trust).") : Go("Salma", "Take me to Widow Salma (Trust)."));
                        }
                        break;
                    case QuestStage.CommunityEvent:
                        text = s.CaravanRuns > 0 ? "The caravan arrived safely, and the people speak of your courage. The Gate is ready." : "Defend a Caravan of Light, then return to me. " + caravanText;
                        if (s.CaravanRuns > 0) opts.Add(new Opt("The caravan is safe.", 5));
                        else opts.Add(Go("CaravanMaster", "Take me to the Caravan Master."));
                        break;
                    case QuestStage.FinalGateUnlocked:
                        text = "The Gate Keeper will open the Forgotten Gate for you. Defeat the Eclipse Sovereign and the crescent will be free.";
                        opts.Add(Go("GateKeeper", "Take me to the Gate Keeper."));
                        break;
                    case QuestStage.BossDefeated:
                        text = "I felt it - the Eclipse is broken! Come closer, the crescent wants to thank you.";
                        opts.Add(new Opt("Let the crescent shine.", 6));
                        break;
                    default:
                        text = "The Last Crescent shines because of you, " + c.Player.Name + ". You hold " + s.StampCount + " of " + Stamps.Total + " lantern stamps and " + s.Tokens + " Crescent Tokens.";
                        break;
                }
                if (stage >= QuestStage.Introduction)
                {
                    opts.Add(new Opt("Tonight's tasks.", 20));
                    opts.Add(new Opt("My lantern stamps.", 30));
                }
            }
            Say(c, stream, "Guardian", text, opts.ToArray());
        }

        static string Mark<T>(T value, T flag) where T : struct
        {
            return (Convert.ToInt32(value) & Convert.ToInt32(flag)) != 0 ? "[x]" : "[ ]";
        }

        static void DailyBoard(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream)
        {
            var opts = new List<Opt>();
            var sb = new StringBuilder();
            lock (s.Sync)
            {
                RamadanRules.EnsureDaily(Config, s, DateTime.Now);
                sb.Append("Night " + (s.DailyDay + 1) + " of the event. ");
                bool unclaimed = false;
                for (int i = 0; i < 3; i++)
                {
                    var t = (DailyTaskType)s.DailyTasks[i];
                    int target = RamadanRules.TaskTarget(Config, t);
                    bool done = (s.DailyDoneMask & (1 << i)) != 0;
                    if (!done && s.DailyProgress[i] >= target) unclaimed = true;
                    sb.Append("\n" + (done ? "[x] " : "[ ] ") + TaskName(t) + (target > 1 && !done ? " (" + s.DailyProgress[i] + "/" + target + ")" : "") + ".");
                    if (!done && t == DailyTaskType.IftarErrand && s.ErrandStep == 0) opts.Add(new Opt("Give me the iftar basket.", 21));
                    if (!done && t == DailyTaskType.Riddle) opts.Add(new Opt("Ask me the Riddle of the Night.", 22));
                }
                if (unclaimed) opts.Add(new Opt("Claim my finished tasks.", 23));
                if (s.ErrandStep == 1) opts.Add(Go("Lamplighter", "Take me to the Old Lamplighter (iftar basket)."));
                if (RamadanRules.MakeupAvailable(Config, s, DateTime.Now)) opts.Add(Go("Watchman", "Take me to the Night Watchman."));
                sb.Append(s.DailyStampGiven ? "\nTonight's lantern stamp is already yours." : "\nFinish all three to earn tonight's lantern stamp.");
                if (RamadanRules.MakeupAvailable(Config, s, DateTime.Now))
                    sb.Append(" You missed some nights: the Night Watchman on Bird Island offers a Night Watch to catch up.");
            }
            Say(c, stream, "Guardian", sb.ToString(), opts.ToArray());
        }

        static void StampBook(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream)
        {
            var opts = new List<Opt>();
            string text;
            lock (s.Sync)
            {
                text = "Lantern stamps: " + s.StampCount + "/" + Stamps.Total + " (story " + Enumerable.Range(1, 12).Count(n => s.HasStamp(n)) + "/12, nightly " + s.DailyStamps + "/" + Stamps.DailyCount + "). "
                    + "Crescent Tokens: " + s.Tokens + ". Contribution: " + s.Points + ". Milestone rewards at 7, 15 and 30 stamps.";
                foreach (var m in Milestones)
                    if (s.StampCount >= m && !s.Claims.Contains("milestone:" + m)) { opts.Add(new Opt("Claim my milestone rewards.", 31)); break; }
            }
            Say(c, stream, "Guardian", text, opts.ToArray());
        }

        // ------------------------------------------------------------------ riddles (Sage Noor = seal, Guardian = riddle of the night)
        static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, string> RiddleCtx = new System.Collections.Concurrent.ConcurrentDictionary<uint, string>();

        static void AskRiddle(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, string npc, string ctx, byte firstOption)
        {
            string text; var opts = new List<Opt>();
            lock (s.Sync)
            {
                if (s.WisdomLockUntil > Now())
                {
                    Say(c, stream, npc, "Your mind is still clouded by the last answer. Rest for " + Wait(s.WisdomLockUntil) + ", then ask again.");
                    return;
                }
                string cur;
                if (s.WisdomRiddle < 0 || s.WisdomRiddle >= RamadanRules.Riddles.Length || !RiddleCtx.TryGetValue(s.UID, out cur) || cur != ctx)
                    RamadanRules.NewRiddle(s, NewRandom(), s.WisdomRiddle);
                RiddleCtx[s.UID] = ctx;
                var r = RamadanRules.Riddles[s.WisdomRiddle];
                text = r.Q;
                for (int i = 0; i < 4; i++) opts.Add(new Opt(r.Options[s.WisdomOrder[i] - '0'], (byte)(firstOption + i)));
            }
            Say(c, stream, npc, text, opts.ToArray());
        }

        static Random NewRandom() { return new Random(Roll(int.MaxValue)); }

        static void AnswerRiddle(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, string npc, string ctx, int choice)
        {
            string cur;
            bool correct, sealNow = false, dailyNow = false;
            lock (s.Sync)
            {
                if (!RiddleCtx.TryGetValue(s.UID, out cur) || cur != ctx) { Say(c, stream, npc, "That answer belongs to another question. Listen again."); return; }
                if (s.WisdomLockUntil > Now()) { Say(c, stream, npc, "Patience. Rest for " + Wait(s.WisdomLockUntil) + "."); return; }
                correct = RamadanRules.CheckRiddle(s, choice);
                int last = s.WisdomRiddle;
                if (correct)
                {
                    if (ctx == "wisdom")
                    {
                        s.WisdomCorrect++;
                        if (s.WisdomCorrect >= Config.WisdomRequired) sealNow = RamadanRules.GiveSeal(s, SealFlags.Wisdom);
                    }
                    else if (ctx == "daily")
                    {
                        dailyNow = true;
                        s.RiddleOfDayDone = 1;
                    }
                }
                else s.WisdomLockUntil = Now() + Config.WisdomWrongCooldownSec;
                RamadanRules.NewRiddle(s, NewRandom(), last);   // a fresh riddle next time; old option ids become stale
                RiddleCtx.TryRemove(s.UID, out cur);
                s.Dirty = true;
            }
            Store.Log(s.UID, s.Name, "riddle", ctx + (correct ? " correct" : " wrong"));
            if (!correct) { Say(c, stream, npc, "No... that is not it. Think on it for " + Config.WisdomWrongCooldownSec + " seconds, then come back."); return; }
            if (sealNow)
            {
                Stamp(c, s, Stamps.Wisdom);
                Grant(c, s, "seal:wisdom", "SealWisdom", "Seal of Wisdom");
                Say(c, stream, npc, "Three truths, three lights. Take the Seal of Wisdom - a mind that seeks the light cannot be eclipsed.", Go("Guardian", "Take me back to the Guardian."));
                return;
            }
            if (dailyNow)
            {
                List<int> done;
                lock (s.Sync) done = RamadanRules.AddDailyProgress(Config, s, DateTime.Now, DailyTaskType.Riddle, 1);
                foreach (var i in done) CompleteDaily(c, s, i);
                Say(c, stream, npc, "Well answered! The night is a little brighter.");
                return;
            }
            int left;
            lock (s.Sync) left = Math.Max(0, Config.WisdomRequired - s.WisdomCorrect);
            Say(c, stream, npc, "Correct! " + left + " more and the seal is yours.", new Opt("Ask me another riddle.", 10));
        }

        // ------------------------------------------------------------------ clue NPCs
        static void LamplighterNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            if (option == 1)
            {
                bool got;
                lock (s.Sync) got = RamadanRules.GiveClue(s, ClueFlags.Lamplighter);
                if (got)
                {
                    Store.Log(s.UID, s.Name, "clue", "Lamplighter");
                    Stamp(c, s, Stamps.ClueA);
                    Say(c, stream, "Lamplighter", "That night a cold wind came from the east and every lantern in the Market went out at once. Only one kept burning: the great lantern of Phoenix Castle, and I saw a shadow carry it into the forest. Look for what is left of it near the castle.", Go("ScorchedLantern", "Take me to the scorched lantern."));
                    return;
                }
            }
            if (option == 2)
            {
                bool ok; int idx;
                lock (s.Sync)
                {
                    idx = Array.IndexOf(s.DailyTasks, (int)DailyTaskType.IftarErrand);
                    ok = s.ErrandStep == 1 && idx >= 0;
                    if (ok) { s.ErrandStep = 2; s.Dirty = true; }
                }
                if (ok)
                {
                    List<int> done;
                    lock (s.Sync) done = RamadanRules.AddDailyProgress(Config, s, DateTime.Now, DailyTaskType.IftarErrand, 1);
                    foreach (var i in done) CompleteDaily(c, s, i);
                    Say(c, stream, "Lamplighter", "Dates and warm soup! May your fast be accepted and your lantern never dim. Thank the Guardian for me.", Go("Guardian", "Take me back to the Guardian."));
                    return;
                }
            }
            var opts = new List<Opt>();
            string text = "Every evening I light the lanterns of the Market, one by one, as my father did.";
            lock (s.Sync)
            {
                if (RamadanRules.ClueAvailable(s, ClueFlags.Lamplighter)) { text = "You come from the Guardian? Then you want to know about the night the crescent went dark..."; opts.Add(new Opt("Tell me what you saw.", 1)); }
                if (s.ErrandStep == 1) opts.Add(new Opt("The Guardian sends you an iftar basket.", 2));
            }
            Say(c, stream, "Lamplighter", text, opts.ToArray());
        }

        static void LanternNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            if (option == 1)
            {
                bool got;
                lock (s.Sync) got = RamadanRules.GiveClue(s, ClueFlags.ScorchedLantern);
                if (got)
                {
                    Store.Log(s.UID, s.Name, "clue", "ScorchedLantern");
                    Stamp(c, s, Stamps.ClueB);
                    Say(c, stream, "ScorchedLantern", "Under the ashes you find claw marks and a black sun burned into the brass. Whoever did this left in a hurry, east toward the plains outside Twin City. Eclipse Scouts were seen there.", Go("Scouts", "Take me to the Eclipse Scouts."));
                    return;
                }
            }
            bool avail;
            lock (s.Sync) avail = RamadanRules.ClueAvailable(s, ClueFlags.ScorchedLantern);
            if (avail) Say(c, stream, "ScorchedLantern", "A great brass lantern lies in the grass, cold and scorched black.", new Opt("Search the ashes.", 1));
            else Say(c, stream, "ScorchedLantern", "An old lantern, burned black. It smells of smoke and something colder.");
        }

        // ------------------------------------------------------------------ seal NPCs
        static void RashidNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            bool eligible; long lockUntil;
            lock (s.Sync) { eligible = s.Stage == QuestStage.SealChallenges && (s.Seals & SealFlags.Courage) == 0; lockUntil = s.CourageLockUntil; }
            if (!eligible) { Say(c, stream, "Rashid", "Courage is not the absence of fear. It is lighting a lantern in the dark anyway.", Go("Guardian", "Take me to the Guardian of the Crescent.")); return; }
            if (lockUntil > Now()) { Say(c, stream, "Rashid", "Catch your breath, soldier. The trial opens again for you in " + Wait(lockUntil) + "."); return; }
            if (option == 1)
            {
                string err = StartCourage(c);
                if (err != null) Say(c, stream, "Rashid", err);
                return;
            }
            Say(c, stream, "Rashid", "An Eclipse Warden guards the old battlefield, and its shadows have grown bold. Face it alone. You have " + (Config.CourageTimeLimitSec / 60) + " minutes once you enter.",
                new Opt("Send me into the Trial of Courage.", 1));
        }

        static void NoorNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            bool eligible; int correct;
            lock (s.Sync) { eligible = s.Stage == QuestStage.SealChallenges && (s.Seals & SealFlags.Wisdom) == 0; correct = s.WisdomCorrect; }
            if (!eligible) { Say(c, stream, "Noor", "Light that is shared does not grow less. Remember that, child.", Go("Guardian", "Take me to the Guardian of the Crescent.")); return; }
            if (option >= 11 && option <= 14) { AnswerRiddle(c, s, stream, "Noor", "wisdom", option - 11); return; }
            if (option == 10) { AskRiddle(c, s, stream, "Noor", "wisdom", 11); return; }
            Say(c, stream, "Noor", "Wisdom is a lantern you carry inside. Answer " + Config.WisdomRequired + " of my riddles (" + correct + " so far). A wrong answer costs you " + Config.WisdomWrongCooldownSec + " seconds of thought.",
                new Opt("I am ready for a riddle.", 10));
        }

        static void KareemNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            bool eligible; int runs;
            lock (s.Sync) { eligible = s.Stage == QuestStage.SealChallenges && (s.Seals & SealFlags.Unity) == 0; runs = s.CaravanRuns; }
            if (!eligible) { Say(c, stream, "Kareem", "A single lantern lights a room. A thousand light a city.", Go("Guardian", "Take me to the Guardian of the Crescent.")); return; }
            if (option == 1)
            {
                string err = StartUnity(c);
                if (err != null) Say(c, stream, "Kareem", err);
                return;
            }
            if (option == 2 && Config.UnityViaCaravan && runs > 0)
            {
                bool ok;
                lock (s.Sync) ok = RamadanRules.GiveSeal(s, SealFlags.Unity);
                if (ok)
                {
                    Stamp(c, s, Stamps.Unity);
                    Grant(c, s, "seal:unity", "SealUnity", "Seal of Unity");
                    Say(c, stream, "Kareem", "You stood with strangers to protect the caravan. That is unity. Take the seal.", Go("Guardian", "Take me back to the Guardian."));
                }
                return;
            }
            var opts = new List<Opt> { new Opt("My team will face the Shadow Totems.", 1) };
            if (Config.UnityViaCaravan && runs > 0) opts.Add(new Opt("I defended a Caravan of Light.", 2));
            Say(c, stream, "Kareem", "Shadow Totems feed on loneliness. Bring a team of at least " + Config.UnityMinTeam + ": everyone who strikes down a totem earns the seal. "
                + (Config.UnityViaCaravan ? "Or defend a Caravan of Light with the people of the land and tell me about it." : ""), opts.ToArray());
        }

        static void SalmaNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            bool eligible; int step; long lockUntil;
            lock (s.Sync) { eligible = s.Stage == QuestStage.SealChallenges && (s.Seals & SealFlags.Trust) == 0; step = s.TrustStep; lockUntil = s.TrustLockUntil; }
            if (!eligible) { Say(c, stream, "Salma", "My husband's lantern hangs by my door again. Thank you.", Go("Guardian", "Take me to the Guardian of the Crescent.")); return; }
            if (lockUntil > Now()) { Say(c, stream, "Salma", "You... you came back without it. Give me time, " + Wait(lockUntil) + ", before I ask anything of you again."); return; }
            if (option == 1 && step == 0)
            {
                lock (s.Sync) { s.TrustStep = 1; s.Dirty = true; }
                Say(c, stream, "Salma", "Thank you. The Night Watchman on Bird Island has kept it since my husband passed. Please bring it home.", Go("Watchman", "Take me to the Night Watchman."));
                return;
            }
            if (option == 2 && step == 2)
            {
                bool ok;
                lock (s.Sync) { ok = RamadanRules.GiveSeal(s, SealFlags.Trust); if (ok) { s.TrustStep = 3; s.Dirty = true; } }
                if (ok)
                {
                    Stamp(c, s, Stamps.Trust);
                    Grant(c, s, "seal:trust", "SealTrust", "Seal of Trust");
                    Say(c, stream, "Salma", "His lantern... and still warm. You could have kept it and no one would know. That is why you deserve the Seal of Trust.", Go("Guardian", "Take me back to the Guardian."));
                }
                return;
            }
            if (step == 0) Say(c, stream, "Salma", "My husband lit the lanterns of this city for thirty years. His own lantern was given to the Night Watchman of Bird Island to guard. Will you bring it back to me?", new Opt("I will bring it back.", 1));
            else if (step == 1) Say(c, stream, "Salma", "The Night Watchman on Bird Island keeps my husband's lantern.", Go("Watchman", "Take me to the Night Watchman."));
            else if (step == 2) Say(c, stream, "Salma", "Is that... his lantern?", new Opt("Here is your husband's lantern.", 2));
            else Say(c, stream, "Salma", "Thank you.");
        }

        static void WatchmanNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            // Night Watch makeup: a missed nightly stamp for players who are behind
            if (option == 5)
            {
                bool ok; int day;
                lock (s.Sync)
                {
                    ok = RamadanRules.MakeupAvailable(Config, s, DateTime.Now) && s.MakeupKills >= Config.MakeupKillCount;
                    day = s.DailyDay;
                }
                if (ok)
                {
                    var r = Grant(c, s, "makeup:" + day, "Makeup", "Night Watch");
                    if (r == GrantResult.Ok || r == GrantResult.Already)
                    {
                        int n = 0;
                        lock (s.Sync)
                        {
                            if (s.MakeupDone) return;
                            s.MakeupDone = true; s.Dirty = true; n = RamadanRules.NextDailyStamp(s);
                        }
                        if (n != 0) Stamp(c, s, n);
                        Say(c, stream, "Watchman", "You kept watch through the night. Take this lantern stamp - it fills one of the nights you missed.");
                    }
                    return;
                }
            }
            int step; long lockUntil; bool seal;
            lock (s.Sync) { step = s.TrustStep; lockUntil = s.TrustLockUntil; seal = s.Stage == QuestStage.SealChallenges && (s.Seals & SealFlags.Trust) == 0; }
            if (seal && step == 1)
            {
                if (option == 1)
                {
                    lock (s.Sync) { s.TrustStep = 2; s.Dirty = true; }
                    Store.Log(s.UID, s.Name, "trust", "return");
                    Say(c, stream, "Watchman", "Then go with my blessing. Widow Salma waits in Desert City.", Go("Salma", "Take me to Widow Salma."));
                    return;
                }
                if (option == 2)
                {
                    lock (s.Sync) { s.TrustStep = 0; s.TrustLockUntil = Now() + Config.TrustRetryCooldownSec; s.Dirty = true; }
                    Store.Log(s.UID, s.Name, "trust", "keep");
                    Say(c, stream, "Watchman", "The flame dies the moment you close your hand around it. A light kept for oneself is no light at all. Go back to Salma when you are ready to try again.");
                    return;
                }
                var trustOpts = new List<Opt> { new Opt("I will return it to Salma.", 1), new Opt("I will keep the golden light.", 2) };
                lock (s.Sync) if (RamadanRules.MakeupAvailable(Config, s, DateTime.Now) && s.MakeupKills >= Config.MakeupKillCount) trustOpts.Add(new Opt("My Night Watch is done.", 5));
                Say(c, stream, "Watchman", "Salma's lantern? Here it is. Look at it closely: its flame is pure gold, and a merchant would pay a fortune for it. Nobody would ever know if you kept it.", trustOpts.ToArray());
                return;
            }
            var opts = new List<Opt>();
            string text = "I watch the sea all night so the lanterns of the island never go out.";
            lock (s.Sync)
            {
                if (RamadanRules.MakeupAvailable(Config, s, DateTime.Now))
                {
                    text = "You missed some nights of the month. Keep watch with me: defeat " + Config.MakeupKillCount + " monsters after finishing tonight's tasks (" + Math.Min(s.MakeupKills, Config.MakeupKillCount) + "/" + Config.MakeupKillCount + ") and I will give you a lantern stamp for a missed night.";
                    if (s.MakeupKills >= Config.MakeupKillCount) opts.Add(new Opt("My watch is done.", 5));
                }
            }
            Say(c, stream, "Watchman", text, opts.ToArray());
        }

        // ------------------------------------------------------------------ caravan master / gate keeper
        static void CaravanMasterNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            int runs, total;
            lock (s.Sync) { runs = s.CaravanRuns; total = s.CaravanPointsTotal; }
            bool running;
            lock (CaravanSync) running = Caravan != null;
            if (running) Say(c, stream, "CaravanMaster", CaravanStatusText(c.Player.UID) + " You have defended " + runs + " caravan(s).", Go("Caravan", "Take me to the caravan!"));
            else Say(c, stream, "CaravanMaster", CaravanStatusText(c.Player.UID) + " You have defended " + runs + " caravan(s) (" + total + " points in total).", Go("Guardian", "Take me to the Guardian of the Crescent."));
        }

        static void GateKeeperNpc(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            QuestStage stage; long lockUntil;
            lock (s.Sync) { stage = s.Stage; lockUntil = s.BossLockUntil; }
            if (stage < QuestStage.FinalGateUnlocked) { Say(c, stream, "GateKeeper", "The Forgotten Gate opens only for one who brought the 7 Crescent Pieces back to the Guardian.", Go("Guardian", "Take me to the Guardian.")); return; }
            if (lockUntil > Now()) { Say(c, stream, "GateKeeper", "The Gate is still sealed for you. Come back in " + Wait(lockUntil) + "."); return; }
            if (option == 1 || option == 2)
            {
                string err = StartBoss(c, option == 2);
                if (err != null) Say(c, stream, "GateKeeper", err);
                return;
            }
            Say(c, stream, "GateKeeper", "Beyond this gate the Eclipse Sovereign feeds on the stolen light. It grows stronger for every hero that enters, so choose your company well. You have " + (Config.BossTimeLimitSec / 60) + " minutes."
                + (stage >= QuestStage.BossDefeated ? " You have beaten it before; the sky still rewards you once per night." : ""),
                new Opt("Open the gate - I go alone.", 1), new Opt("Open the gate for my team.", 2));
        }

        // ------------------------------------------------------------------ Crescent Merchant (shop)
        static void MerchantNpc(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            if (!Config.ShopOpen(DateTime.Now)) { Say(c, stream, "Merchant", "The Crescent Bazaar is closed."); return; }
            var s = GetState(c);
            if (s == null) return;
            if (option >= 1 && option <= Config.Shop.Count)
            {
                Buy(c, s, Config.Shop[option - 1]);
            }
            var opts = new List<Opt>();
            int tokens;
            lock (s.Sync)
            {
                tokens = s.Tokens;
                for (int i = 0; i < Config.Shop.Count && i < 9; i++)
                {
                    var e = Config.Shop[i];
                    int b; s.Bought.TryGetValue(e.Key, out b);
                    opts.Add(new Opt((e.Label.Length > 40 ? e.Label.Substring(0, 40) : e.Label) + " x" + e.Count + " - " + e.Cost + " tokens" + (e.Limit > 0 ? " (" + b + "/" + e.Limit + ")" : ""), (byte)(i + 1)));
                }
            }
            Say(c, stream, "Merchant", "Welcome to the Crescent Bazaar! Crescent Tokens are earned through the Legend of the Last Crescent and cannot be traded. You have " + tokens + " tokens.", opts.ToArray());
        }

        /// <summary>Space is checked before tokens are taken; state is saved before the item is handed out; any failure rolls back.</summary>
        static void Buy(Client.GameClient c, RamadanPlayerState s, ShopEntry e)
        {
            lock (s.Sync)
            {
                var err = RamadanRules.CheckPurchase(Config, s, e, DateTime.Now);
                if (err != null) { Tell(c, err, MsgMessage.MsgColor.red); return; }
                if (!c.Inventory.HaveSpace((byte)e.Count)) { Tell(c, "Make " + e.Count + " free inventory slot(s) first.", MsgMessage.MsgColor.red); return; }
                RamadanRules.RecordPurchase(s, e);
                if (!SaveState(s))
                {
                    RamadanRules.RollbackPurchase(s, e);
                    Tell(c, "The bazaar ledger is busy, try again.", MsgMessage.MsgColor.red);
                    return;
                }
                bool ok = false;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    try { ok = c.Inventory.Add(rec.GetStream(), e.ItemId, (byte)e.Count); } catch (Exception ex) { MyConsole.WriteException(ex); }
                }
                if (!ok)
                {
                    RamadanRules.RollbackPurchase(s, e);
                    SaveState(s);
                    Store.Log(s.UID, s.Name, "shop-rollback", e.Key);
                    Tell(c, "The item could not be added; your tokens were returned.", MsgMessage.MsgColor.red);
                    return;
                }
                Store.Log(s.UID, s.Name, "shop", e.Key + " " + e.ItemId + "x" + e.Count + " for " + e.Cost + " (left " + s.Tokens + ")");
                Tell(c, "You bought " + e.Label + " x" + e.Count + ". Tokens left: " + s.Tokens + ".");
            }
        }

        // ------------------------------------------------------------------ Crescent Herald (leaderboard)
        static List<KeyValuePair<string, int>> boardCache = new List<KeyValuePair<string, int>>();
        static DateTime boardAt = DateTime.MinValue;
        static readonly object BoardSync = new object();

        /// <summary>Top list from the in-memory index (built at start-up, updated by every save) - no disk access.</summary>
        static void RefreshBoard()
        {
            lock (BoardSync)
            {
                if ((DateTime.Now - boardAt).TotalSeconds < 60) return;
                foreach (var st in Online.Values.ToList())
                    lock (st.Sync) if (st.Stage != QuestStage.NotStarted || st.Points > 0) BoardUpdate(st.UID, st.Name, st.Points);
                List<KeyValuePair<string, int>> all;
                lock (BoardIndex) all = BoardIndex.Values.Where(v => !string.IsNullOrEmpty(v.Key)).ToList();
                boardCache = RamadanRules.Top(all, int.MaxValue);
                boardAt = DateTime.Now;
            }
        }

        static void HeraldNpc(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            RefreshBoard();
            var sb = new StringBuilder("Heroes of the Last Crescent:");
            List<KeyValuePair<string, int>> board;
            lock (BoardSync) board = boardCache;
            for (int i = 0; i < board.Count && i < 10; i++)
                sb.Append("\n" + (i + 1) + ". " + board[i].Key + " - " + board[i].Value);
            if (board.Count == 0) sb.Append("\nNo heroes yet. Be the first!");
            int rank = board.FindIndex(e => e.Key == c.Player.Name);
            var s = GetState(c);
            int mine = 0;
            if (s != null) lock (s.Sync) mine = s.Points;
            sb.Append("\nYou: " + mine + " points" + (rank >= 0 ? ", rank " + (rank + 1) : "") + ". (Updated every minute.)");
            Say(c, stream, "Herald", sb.ToString());
        }
    }
}
