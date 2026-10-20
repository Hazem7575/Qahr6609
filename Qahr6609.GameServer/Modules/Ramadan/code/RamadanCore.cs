// The Legend of the Last Crescent - Ramadan event
// Core layer: no dependency on game types, so it is unit-testable on its own
// (see tests/RamadanEvent.Tests). Everything that touches GameClient lives in RamadanEvent*.cs.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.Ramadan
{
    public enum QuestStage : int
    {
        NotStarted = 0,
        Introduction = 1,
        ClueHunt = 2,
        SealChallenges = 3,
        CommunityEvent = 4,
        FinalGateUnlocked = 5,
        BossDefeated = 6,
        Completed = 7
    }

    [Flags]
    public enum ClueFlags : int
    {
        None = 0,
        Lamplighter = 1,     // talk to Old Lamplighter (Market)
        ScorchedLantern = 2, // inspect the scorched lantern (Phoenix Castle)
        ShadowLetter = 4,    // taken from an Eclipse Scout (Twin City outskirts)
        All = 7
    }

    [Flags]
    public enum SealFlags : int
    {
        None = 0,
        Courage = 1,
        Wisdom = 2,
        Unity = 4,
        Trust = 8,
        All = 15
    }

    public enum DailyTaskType : int
    {
        None = 0,
        Hunt = 1,        // kill N monsters
        CrescentDust = 2,// collect N crescent dust (server-side drop chance on kills)
        Elite = 3,       // defeat N elite monsters
        Caravan = 4,     // contribute to a Caravan of Light
        IftarErrand = 5, // carry the iftar basket Guardian -> Lamplighter
        Riddle = 6       // answer the riddle of the day
    }

    /// <summary>All 30 Ramadan stamps. 1..12 are story stamps, 13..30 are daily lantern stamps.</summary>
    public static class Stamps
    {
        public const int Intro = 1, ClueA = 2, ClueB = 3, ClueC = 4, Courage = 5, Wisdom = 6, Unity = 7, Trust = 8,
            FirstCaravan = 9, GateOpened = 10, BossDefeated = 11, StoryComplete = 12, FirstDaily = 13, Total = 30;
        public static readonly int DailyCount = Total - FirstDaily + 1; // 18

        public static string Name(int n)
        {
            switch (n)
            {
                case Intro: return "Stamp of the First Night";
                case ClueA: return "Stamp of the Lamplighter";
                case ClueB: return "Stamp of the Scorched Lantern";
                case ClueC: return "Stamp of the Shadow Letter";
                case Courage: return "Stamp of Courage";
                case Wisdom: return "Stamp of Wisdom";
                case Unity: return "Stamp of Unity";
                case Trust: return "Stamp of Trust";
                case FirstCaravan: return "Stamp of the Caravan";
                case GateOpened: return "Stamp of the Forgotten Gate";
                case BossDefeated: return "Stamp of the Broken Eclipse";
                case StoryComplete: return "Stamp of the Last Crescent";
                default:
                    if (n >= FirstDaily && n <= Total) return "Lantern Stamp #" + (n - FirstDaily + 1);
                    return "Unknown stamp";
            }
        }
    }

    // ------------------------------------------------------------------ reward spec
    /// <summary>"tokens:10;item:1088001x2;expballs:1;points:5;silver:1000" - parsed once at config load.</summary>
    public class RewardSpec
    {
        public int Tokens;
        public int Points;
        public int ExpBalls;
        public long Silver;
        public List<KeyValuePair<uint, int>> Items = new List<KeyValuePair<uint, int>>();

        public bool IsEmpty { get { return Tokens == 0 && Points == 0 && ExpBalls == 0 && Silver == 0 && Items.Count == 0; } }
        public int SlotsNeeded { get { return Items.Sum(i => i.Value); } }

        public static RewardSpec Parse(string text, List<string> errors)
        {
            var r = new RewardSpec();
            if (string.IsNullOrWhiteSpace(text)) return r;
            foreach (var raw in text.Split(';'))
            {
                var part = raw.Trim();
                if (part.Length == 0) continue;
                int c = part.IndexOf(':');
                if (c <= 0) { if (errors != null) errors.Add("bad reward part '" + part + "'"); continue; }
                string k = part.Substring(0, c).Trim().ToLowerInvariant(), v = part.Substring(c + 1).Trim();
                int iv; long lv;
                switch (k)
                {
                    case "tokens": if (int.TryParse(v, out iv) && iv >= 0) r.Tokens += iv; else if (errors != null) errors.Add("bad tokens " + v); break;
                    case "points": if (int.TryParse(v, out iv) && iv >= 0) r.Points += iv; else if (errors != null) errors.Add("bad points " + v); break;
                    case "expballs": if (int.TryParse(v, out iv) && iv >= 0 && iv <= 20) r.ExpBalls += iv; else if (errors != null) errors.Add("bad expballs " + v); break;
                    case "silver": if (long.TryParse(v, out lv) && lv >= 0 && lv <= 100000000) r.Silver += lv; else if (errors != null) errors.Add("bad silver " + v); break;
                    case "item":
                        {
                            var p = v.ToLowerInvariant().Split('x');
                            uint id; int cnt = 1;
                            if (p.Length >= 1 && uint.TryParse(p[0], out id) && (p.Length == 1 || (int.TryParse(p[1], out cnt) && cnt > 0 && cnt <= 10)))
                                r.Items.Add(new KeyValuePair<uint, int>(id, cnt));
                            else if (errors != null) errors.Add("bad item " + v);
                            break;
                        }
                    default: if (errors != null) errors.Add("unknown reward key " + k); break;
                }
            }
            return r;
        }

        public override string ToString()
        {
            var sb = new List<string>();
            if (Tokens > 0) sb.Add(Tokens + " Crescent Tokens");
            if (ExpBalls > 0) sb.Add(ExpBalls + " EXP ball(s)");
            if (Silver > 0) sb.Add(Silver + " silver");
            foreach (var i in Items) sb.Add(i.Value + "x item " + i.Key);
            if (Points > 0) sb.Add(Points + " contribution points");
            return sb.Count == 0 ? "nothing" : string.Join(", ", sb);
        }
    }

    public class ShopEntry
    {
        public string Key;      // stable key used for purchase limits
        public uint ItemId;
        public int Count;
        public int Cost;
        public int Limit;       // per character for the whole event, 0 = unlimited
        public string Label;
    }

    // ------------------------------------------------------------------ config
    public class SimpleIni
    {
        public readonly Dictionary<string, Dictionary<string, string>> Sections =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        public static SimpleIni Parse(string text)
        {
            var ini = new SimpleIni();
            Dictionary<string, string> cur = null;
            foreach (var raw in text.Replace("\r", "").Split('\n'))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("#") || line.StartsWith("//")) continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    var name = line.Substring(1, line.Length - 2).Trim();
                    if (!ini.Sections.TryGetValue(name, out cur))
                    {
                        cur = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        ini.Sections[name] = cur;
                    }
                    continue;
                }
                int eq = line.IndexOf('=');
                if (eq <= 0 || cur == null) continue;
                cur[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
            return ini;
        }
        public string Get(string s, string k, string def)
        {
            Dictionary<string, string> sec; string v;
            if (Sections.TryGetValue(s, out sec) && sec.TryGetValue(k, out v)) return v;
            return def;
        }
        public int GetInt(string s, string k, int def, int min = int.MinValue, int max = int.MaxValue)
        {
            int v;
            if (!int.TryParse(Get(s, k, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out v)) v = def;
            return Math.Max(min, Math.Min(max, v));
        }
        public double GetDouble(string s, string k, double def)
        {
            double v;
            return double.TryParse(Get(s, k, ""), NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : def;
        }
        public bool GetBool(string s, string k, bool def)
        {
            var v = Get(s, k, "").ToLowerInvariant();
            if (v == "1" || v == "true" || v == "yes" || v == "on") return true;
            if (v == "0" || v == "false" || v == "no" || v == "off") return false;
            return def;
        }
        public IEnumerable<KeyValuePair<string, string>> Section(string s)
        {
            Dictionary<string, string> sec;
            if (Sections.TryGetValue(s, out sec)) return sec;
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }
    }

    public class MapPoint
    {
        public uint Map; public ushort X; public ushort Y;
        public MapPoint(uint map, ushort x, ushort y) { Map = map; X = x; Y = y; }
        public static MapPoint Parse(string s, MapPoint def)
        {
            // "1002,306,268"
            var p = (s ?? "").Split(',');
            uint m; ushort x, y;
            if (p.Length >= 3 && uint.TryParse(p[0].Trim(), out m) && ushort.TryParse(p[1].Trim(), out x) && ushort.TryParse(p[2].Trim(), out y))
                return new MapPoint(m, x, y);
            return def;
        }
        public override string ToString() { return Map + "," + X + "," + Y; }
    }

    public class NpcSpot
    {
        public string Key; public uint UID; public ushort LookType; public byte Dir; public MapPoint At;
    }

    public class RamadanConfig
    {
        // [Event]
        public bool Enabled = true;
        public DateTime Start = new DateTime(2026, 2, 18, 0, 0, 0);
        public DateTime End = new DateTime(2026, 3, 20, 23, 59, 59);
        public int ShopGraceDays = 7;
        public int DailyResetHour = 4;
        public bool LoginAnnouncement = true;
        public bool WalkToNpc = true;            // "take me there" walks the player (client path finding) instead of teleporting
        // Crescent Lantern look and feel: effect names from the client's 3D effect list (empty = none)
        public string[] LanternFx = { "", "Health", "", "", "", "" }; // [band] 1 burning .. 5 cold (small effect on the player)
        // the night sky shows how close the piece is: deep blue far away, warm lantern gold when close
        public uint[] LanternColors = { 0, 0xFFD278, 0xD2A064, 0x96785F, 0x5A6496, 0x46508C };
        public string LanternFoundFx = "LevelUp", LanternStartFx = "", LanternCompleteFx = "firework-2love";
        public string LanternSpotFx = "stipple_bdh"; // marker on the ground where the piece lies (empty = no marker)
        public int LanternSpotRange = 20;       // the marker shows when the player is this close (cells)
        public int LanternFoundRange = 2;       // standing this close to the spot finds the piece
        public uint NightColor = 0x46508C;      // screen tint while the quest is running (0 = off)
        public uint DayColor = 0xFFFFFF;        // tint sent back when the quest ends

        // NPCs
        public Dictionary<string, NpcSpot> Npcs = new Dictionary<string, NpcSpot>(StringComparer.OrdinalIgnoreCase);
        public ushort DecorMosqueType = 6013, DecorLanternType = 6014, DecorSetType = 6015, DecorDateType = 6016;
        public int MosqueBlockRadius = 4;

        // monsters
        public uint ScoutId = 8155101, RaiderId = 8155102, WardenId = 8155103, TotemId = 8155104, SentinelId = 8155105, BossId = 8155106, ShadeId = 8155107;

        // clues
        public List<MapPoint> ScoutSpawns = new List<MapPoint>();
        public int ScoutRespawnSec = 60;

        // courage
        public MapPoint CourageArena = new MapPoint(1700, 612, 634);
        public int CourageMinions = 4, CourageTimeLimitSec = 600, CourageCooldownSec = 120;

        // wisdom
        public int WisdomRequired = 3, WisdomWrongCooldownSec = 60;

        // unity
        public MapPoint UnityArena = new MapPoint(1700, 612, 634);
        public int UnityMinTeam = 2, UnityTotems = 6, UnityTimeLimitSec = 600;
        public bool UnityViaCaravan = true;

        // trust
        public int TrustRetryCooldownSec = 600;

        // caravan
        public List<TimeSpan> CaravanTimes = new List<TimeSpan>();
        public MapPoint CaravanAt = new MapPoint(1002, 428, 378);
        public int CaravanWaves = 3, CaravanRaidersPerWave = 10, CaravanDurationMin = 15, CaravanSpawnRange = 8;
        public int CaravanMinContribution = 3, CaravanPointsCap = 30;
        public int CaravanTokensBase = 5, CaravanTokensPerPoint = 1, CaravanTokensCap = 35;
        public int CaravanMaxRewardsPerMachine = 2;

        // boss
        public MapPoint BossArena = new MapPoint(1801, 52, 57);
        public int BossBaseHP = 3000000;
        public double BossHPPerExtraMember = 0.6;
        public int BossPhase2Pct = 70, BossPhase3Pct = 35, BossSentinels = 4;
        public int BossBurstIntervalSec = 12, BossBurstWarnSec = 3, BossBurstRadius = 3, BossBurstDamagePct = 30, BossBurstTargets = 3;
        public int BossTimeLimitSec = 900, BossEntryCooldownSec = 300, BossMaxTeam = 5;
        public int BossMinDamagePctForReward = 1;

        // daily
        public int DailyHuntCount = 120, DailyDustNeeded = 15, DailyDustChancePct = 10, DailyEliteCount = 3, DailyEliteMinHP = 50000;
        public int MakeupKillCount = 250;

        // rewards
        public Dictionary<string, RewardSpec> Rewards = new Dictionary<string, RewardSpec>(StringComparer.OrdinalIgnoreCase);

        // leaderboard points
        public int PointsPerCaravanPoint = 1;

        // shop
        public List<ShopEntry> Shop = new List<ShopEntry>();

        public List<string> Errors = new List<string>();

        public RewardSpec Reward(string key)
        {
            RewardSpec r;
            return Rewards.TryGetValue(key, out r) ? r : new RewardSpec();
        }

        public bool InWindow(DateTime now) { return now >= Start && now <= End; }
        public bool ShopOpen(DateTime now) { return now >= Start && now <= End.AddDays(ShopGraceDays); }

        public static readonly string[] RewardKeys = {
            "Intro", "Clues", "SealCourage", "SealWisdom", "SealUnity", "SealTrust", "GateUnlocked",
            "BossFirstClear", "BossDaily", "StoryComplete", "DailyTask", "DailyAllDone", "Makeup",
            "Milestone7", "Milestone15", "Milestone30", "LanternPiece", "LanternComplete" };

        public static RamadanConfig Load(string text)
        {
            var c = new RamadanConfig();
            var ini = SimpleIni.Parse(text ?? "");
            c.Enabled = ini.GetBool("Event", "Enabled", c.Enabled);
            DateTime d;
            if (DateTime.TryParseExact(ini.Get("Event", "Start", ""), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) c.Start = d;
            else if (ini.Get("Event", "Start", "") != "") c.Errors.Add("Event.Start must be yyyy-MM-dd HH:mm");
            if (DateTime.TryParseExact(ini.Get("Event", "End", ""), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) c.End = d;
            else if (ini.Get("Event", "End", "") != "") c.Errors.Add("Event.End must be yyyy-MM-dd HH:mm");
            if (c.End <= c.Start) { c.Errors.Add("Event.End is before Event.Start - event disabled"); c.Enabled = false; }
            c.ShopGraceDays = ini.GetInt("Event", "ShopGraceDays", c.ShopGraceDays, 0, 60);
            c.DailyResetHour = ini.GetInt("Event", "DailyResetHour", c.DailyResetHour, 0, 23);
            c.LoginAnnouncement = ini.GetBool("Event", "LoginAnnouncement", c.LoginAnnouncement);
            c.WalkToNpc = ini.GetBool("Event", "WalkToNpc", c.WalkToNpc);
            string[] bandKeys = { "", "FxBurning", "FxHot", "FxWarm", "FxCool", "FxCold" };
            for (int b = 1; b <= 5; b++) c.LanternFx[b] = ini.Get("Lantern", bandKeys[b], c.LanternFx[b]).Trim();
            string[] colorKeys = { "", "ColorBurning", "ColorHot", "ColorWarm", "ColorCool", "ColorCold" };
            for (int b = 1; b <= 5; b++)
            {
                uint bc;
                if (uint.TryParse(ini.Get("Lantern", colorKeys[b], "").Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out bc)) c.LanternColors[b] = bc;
            }
            c.LanternFoundFx = ini.Get("Lantern", "FxFound", c.LanternFoundFx).Trim();
            c.LanternStartFx = ini.Get("Lantern", "FxStart", c.LanternStartFx).Trim();
            c.LanternCompleteFx = ini.Get("Lantern", "FxComplete", c.LanternCompleteFx).Trim();
            c.LanternSpotFx = ini.Get("Lantern", "FxSpot", c.LanternSpotFx).Trim();
            c.LanternSpotRange = ini.GetInt("Lantern", "SpotRange", c.LanternSpotRange, 0, 60);
            c.LanternFoundRange = ini.GetInt("Lantern", "FoundRange", c.LanternFoundRange, 1, 5);
            uint col;
            if (uint.TryParse(ini.Get("Lantern", "NightColor", "").Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out col)) c.NightColor = col;
            if (uint.TryParse(ini.Get("Lantern", "DayColor", "").Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out col)) c.DayColor = col;

            // NPCs: Key = uid,lookType,dir,map,x,y
            foreach (var kv in ini.Section("Npcs"))
            {
                var p = kv.Value.Split(',');
                uint uid; ushort look; byte dir; uint map; ushort x, y;
                if (p.Length == 6 && uint.TryParse(p[0], out uid) && ushort.TryParse(p[1], out look) && byte.TryParse(p[2], out dir)
                    && uint.TryParse(p[3], out map) && ushort.TryParse(p[4], out x) && ushort.TryParse(p[5], out y) && dir < 8 && look < 6553)
                    c.Npcs[kv.Key] = new NpcSpot { Key = kv.Key, UID = uid, LookType = look, Dir = dir, At = new MapPoint(map, x, y) };
                else c.Errors.Add("bad NPC line " + kv.Key + "=" + kv.Value);
            }
            c.DecorMosqueType = (ushort)ini.GetInt("Decor", "MosqueType", c.DecorMosqueType, 1, 6552);
            c.DecorLanternType = (ushort)ini.GetInt("Decor", "LanternType", c.DecorLanternType, 1, 6552);
            c.DecorSetType = (ushort)ini.GetInt("Decor", "SetType", c.DecorSetType, 1, 6552);
            c.DecorDateType = (ushort)ini.GetInt("Decor", "DateType", c.DecorDateType, 1, 6552);
            c.MosqueBlockRadius = ini.GetInt("Decor", "MosqueBlockRadius", c.MosqueBlockRadius, 0, 20);

            c.ScoutId = (uint)ini.GetInt("Monsters", "Scout", (int)c.ScoutId, 1);
            c.RaiderId = (uint)ini.GetInt("Monsters", "Raider", (int)c.RaiderId, 1);
            c.WardenId = (uint)ini.GetInt("Monsters", "Warden", (int)c.WardenId, 1);
            c.TotemId = (uint)ini.GetInt("Monsters", "Totem", (int)c.TotemId, 1);
            c.SentinelId = (uint)ini.GetInt("Monsters", "Sentinel", (int)c.SentinelId, 1);
            c.BossId = (uint)ini.GetInt("Monsters", "Boss", (int)c.BossId, 1);
            c.ShadeId = (uint)ini.GetInt("Monsters", "Shade", (int)c.ShadeId, 1);

            foreach (var s in ini.Get("Clues", "ScoutSpawns", "1002,526,332;1002,547,370;1002,617,334").Split(';'))
            {
                var mp = MapPoint.Parse(s, null);
                if (mp != null) c.ScoutSpawns.Add(mp); else if (s.Trim() != "") c.Errors.Add("bad scout spawn " + s);
            }
            c.ScoutRespawnSec = ini.GetInt("Clues", "ScoutRespawnSec", c.ScoutRespawnSec, 10, 3600);

            c.CourageArena = MapPoint.Parse(ini.Get("Courage", "Arena", ""), c.CourageArena);
            c.CourageMinions = ini.GetInt("Courage", "Minions", c.CourageMinions, 0, 12);
            c.CourageTimeLimitSec = ini.GetInt("Courage", "TimeLimitSec", c.CourageTimeLimitSec, 60, 3600);
            c.CourageCooldownSec = ini.GetInt("Courage", "CooldownSec", c.CourageCooldownSec, 0, 3600);

            c.WisdomRequired = ini.GetInt("Wisdom", "RequiredCorrect", c.WisdomRequired, 1, 6);
            c.WisdomWrongCooldownSec = ini.GetInt("Wisdom", "WrongCooldownSec", c.WisdomWrongCooldownSec, 0, 3600);

            c.UnityArena = MapPoint.Parse(ini.Get("Unity", "Arena", ""), c.UnityArena);
            c.UnityMinTeam = ini.GetInt("Unity", "MinTeam", c.UnityMinTeam, 1, 5);
            c.UnityTotems = ini.GetInt("Unity", "Totems", c.UnityTotems, 1, 20);
            c.UnityTimeLimitSec = ini.GetInt("Unity", "TimeLimitSec", c.UnityTimeLimitSec, 60, 3600);
            c.UnityViaCaravan = ini.GetBool("Unity", "AlsoViaCaravan", c.UnityViaCaravan);

            c.TrustRetryCooldownSec = ini.GetInt("Trust", "RetryCooldownSec", c.TrustRetryCooldownSec, 0, 86400);

            foreach (var t in ini.Get("Caravan", "Times", "13:00;19:00;22:30").Split(';'))
            {
                TimeSpan ts;
                if (TimeSpan.TryParseExact(t.Trim(), "hh\\:mm", CultureInfo.InvariantCulture, out ts)) c.CaravanTimes.Add(ts);
                else if (t.Trim() != "") c.Errors.Add("bad caravan time " + t);
            }
            c.CaravanAt = MapPoint.Parse(ini.Get("Caravan", "At", ""), c.CaravanAt);
            c.CaravanWaves = ini.GetInt("Caravan", "Waves", c.CaravanWaves, 1, 10);
            c.CaravanRaidersPerWave = ini.GetInt("Caravan", "RaidersPerWave", c.CaravanRaidersPerWave, 1, 40);
            c.CaravanDurationMin = ini.GetInt("Caravan", "DurationMin", c.CaravanDurationMin, 3, 120);
            c.CaravanSpawnRange = ini.GetInt("Caravan", "SpawnRange", c.CaravanSpawnRange, 1, 20);
            c.CaravanMinContribution = ini.GetInt("Caravan", "MinContribution", c.CaravanMinContribution, 1, 1000);
            c.CaravanPointsCap = ini.GetInt("Caravan", "PointsCap", c.CaravanPointsCap, 1, 1000);
            c.CaravanTokensBase = ini.GetInt("Caravan", "TokensBase", c.CaravanTokensBase, 0, 1000);
            c.CaravanTokensPerPoint = ini.GetInt("Caravan", "TokensPerPoint", c.CaravanTokensPerPoint, 0, 100);
            c.CaravanTokensCap = ini.GetInt("Caravan", "TokensCap", c.CaravanTokensCap, 0, 10000);
            c.CaravanMaxRewardsPerMachine = ini.GetInt("Caravan", "MaxRewardsPerMachine", c.CaravanMaxRewardsPerMachine, 0, 100);

            c.BossArena = MapPoint.Parse(ini.Get("Boss", "Arena", ""), c.BossArena);
            c.BossBaseHP = ini.GetInt("Boss", "BaseHP", c.BossBaseHP, 10000, 1000000000);
            c.BossHPPerExtraMember = Math.Max(0, Math.Min(5, ini.GetDouble("Boss", "HPPerExtraMember", c.BossHPPerExtraMember)));
            c.BossPhase2Pct = ini.GetInt("Boss", "Phase2AtPct", c.BossPhase2Pct, 2, 99);
            c.BossPhase3Pct = ini.GetInt("Boss", "Phase3AtPct", c.BossPhase3Pct, 1, 98);
            if (c.BossPhase3Pct >= c.BossPhase2Pct) { c.Errors.Add("Boss.Phase3AtPct must be < Phase2AtPct"); c.BossPhase3Pct = Math.Max(1, c.BossPhase2Pct - 1); }
            c.BossSentinels = ini.GetInt("Boss", "Sentinels", c.BossSentinels, 0, 10);
            c.BossBurstIntervalSec = ini.GetInt("Boss", "BurstIntervalSec", c.BossBurstIntervalSec, 4, 120);
            c.BossBurstWarnSec = ini.GetInt("Boss", "BurstWarnSec", c.BossBurstWarnSec, 1, 10);
            c.BossBurstRadius = ini.GetInt("Boss", "BurstRadius", c.BossBurstRadius, 1, 10);
            c.BossBurstDamagePct = ini.GetInt("Boss", "BurstDamagePct", c.BossBurstDamagePct, 0, 90);
            c.BossBurstTargets = ini.GetInt("Boss", "BurstTargets", c.BossBurstTargets, 1, 5);
            c.BossTimeLimitSec = ini.GetInt("Boss", "TimeLimitSec", c.BossTimeLimitSec, 120, 7200);
            c.BossEntryCooldownSec = ini.GetInt("Boss", "EntryCooldownSec", c.BossEntryCooldownSec, 0, 86400);
            c.BossMaxTeam = ini.GetInt("Boss", "MaxTeam", c.BossMaxTeam, 1, 5);
            c.BossMinDamagePctForReward = ini.GetInt("Boss", "MinDamagePctForReward", c.BossMinDamagePctForReward, 0, 50);

            c.DailyHuntCount = ini.GetInt("Daily", "HuntCount", c.DailyHuntCount, 1, 5000);
            c.DailyDustNeeded = ini.GetInt("Daily", "DustNeeded", c.DailyDustNeeded, 1, 500);
            c.DailyDustChancePct = ini.GetInt("Daily", "DustChancePct", c.DailyDustChancePct, 1, 100);
            c.DailyEliteCount = ini.GetInt("Daily", "EliteCount", c.DailyEliteCount, 1, 100);
            c.DailyEliteMinHP = ini.GetInt("Daily", "EliteMinHP", c.DailyEliteMinHP, 1000, int.MaxValue);
            c.MakeupKillCount = ini.GetInt("Daily", "MakeupKillCount", c.MakeupKillCount, 1, 10000);

            c.PointsPerCaravanPoint = ini.GetInt("Leaderboard", "PointsPerCaravanPoint", c.PointsPerCaravanPoint, 0, 100);

            foreach (var key in RewardKeys)
                c.Rewards[key] = RewardSpec.Parse(ini.Get("Rewards", key, DefaultReward(key)), c.Errors);

            int n = 0;
            foreach (var kv in ini.Section("Shop"))
            {
                // Key = itemId,count,cost,limit,Label
                var p = kv.Value.Split(',');
                uint id; int cnt, cost, lim;
                if (p.Length >= 4 && uint.TryParse(p[0], out id) && int.TryParse(p[1], out cnt) && int.TryParse(p[2], out cost) && int.TryParse(p[3], out lim)
                    && cnt > 0 && cnt <= 10 && cost > 0 && lim >= 0 && n < 9)
                {
                    c.Shop.Add(new ShopEntry { Key = kv.Key, ItemId = id, Count = cnt, Cost = cost, Limit = lim, Label = p.Length > 4 ? p[4].Trim() : ("Item " + id) });
                    n++;
                }
                else c.Errors.Add("bad shop line " + kv.Key + "=" + kv.Value);
            }
            return c;
        }

        public static string DefaultReward(string key)
        {
            switch (key)
            {
                case "Intro": return "tokens:5;points:5";
                case "Clues": return "tokens:15;expballs:1;points:15";
                case "SealCourage": return "tokens:15;item:1088001x1;points:20";
                case "SealWisdom": return "tokens:15;item:1088001x1;points:20";
                case "SealUnity": return "tokens:15;item:1088001x1;points:20";
                case "SealTrust": return "tokens:15;item:1088001x1;points:20";
                case "GateUnlocked": return "tokens:10;points:10";
                case "BossFirstClear": return "tokens:60;item:1088000x1;points:60";
                case "BossDaily": return "tokens:15;expballs:1;points:15";
                case "StoryComplete": return "tokens:40;item:730002x1;points:40";
                case "DailyTask": return "tokens:4;points:4";
                case "DailyAllDone": return "tokens:8;expballs:1;points:8";
                case "Makeup": return "tokens:4;points:4";
                case "Milestone7": return "tokens:20;item:730001x1";
                case "Milestone15": return "tokens:40;item:1088000x1";
                case "Milestone30": return "tokens:80;item:730003x1";
                case "LanternPiece": return "tokens:5;points:5";
                case "LanternComplete": return "tokens:60;item:1088000x1;points:60";
                default: return "";
            }
        }
    }

    // ------------------------------------------------------------------ player state
    public class RamadanPlayerState
    {
        public readonly object Sync = new object();
        public uint UID;
        public string Name = "";
        public QuestStage Stage = QuestStage.NotStarted;
        public ClueFlags Clues;
        public SealFlags Seals;
        public int Tokens;
        public long TokensEarned;
        public int Points;                // leaderboard contribution points
        public uint StampMask;            // bit (n-1) = stamp n
        public int WisdomCorrect;
        public int WisdomRiddle = -1;     // current riddle id
        public string WisdomOrder = "";   // shuffled option order, e.g. "2031"
        public long WisdomLockUntil;      // unix seconds
        public int TrustStep;             // 0 none, 1 asked by Salma, 2 met watchman (has lantern), 3 returned
        public long TrustLockUntil;
        public long CourageLockUntil;
        public long BossLockUntil;
        public int BossClears;
        public int BossLastClearDay = -1;
        public int CaravanRuns;
        public int CaravanPointsTotal;
        public int CaravanDay = -1;       // last event-day the player contributed >= min to a caravan
        // daily block
        public int DailyDay = -1;
        public int[] DailyTasks = new int[3];
        public int[] DailyProgress = new int[3];
        public int DailyDoneMask;         // bit i => task i rewarded
        public bool DailyStampGiven;
        public int ErrandStep;            // iftar errand: 1 carrying basket
        public int RiddleOfDayDone;
        public int MakeupKills;
        public bool MakeupDone;
        public int DailyStamps;           // number of daily lantern stamps already earned
        // The Crescent Lantern quest (warm / cold search for the 7 Crescent Pieces)
        public int LanternStep;           // 0 not started, 1..7 searching piece n, 8 all found, 9 done
        public string LanternTargets = "";// "map:x:y;map:x:y;..." one hidden spot per piece, chosen for this player
        public HashSet<string> Claims = new HashSet<string>();
        public Dictionary<string, int> Bought = new Dictionary<string, int>();
        public bool Dirty;

        public bool HasStamp(int n) { return n >= 1 && n <= 32 && (StampMask & (1u << (n - 1))) != 0; }
        public int StampCount { get { int c = 0; for (int i = 0; i < 32; i++) if ((StampMask & (1u << i)) != 0) c++; return c; } }

        public string Serialize()
        {
            var sb = new StringBuilder();
            Action<string, object> w = (k, v) => sb.Append(k).Append('=').Append(Convert.ToString(v, CultureInfo.InvariantCulture)).Append("\r\n");
            w("Version", 1);
            w("UID", UID);
            w("Name", (Name ?? "").Replace("\r", "").Replace("\n", ""));
            w("Stage", (int)Stage);
            w("Clues", (int)Clues);
            w("Seals", (int)Seals);
            w("Tokens", Tokens);
            w("TokensEarned", TokensEarned);
            w("Points", Points);
            w("StampMask", StampMask);
            w("WisdomCorrect", WisdomCorrect);
            w("WisdomRiddle", WisdomRiddle);
            w("WisdomOrder", WisdomOrder);
            w("WisdomLockUntil", WisdomLockUntil);
            w("TrustStep", TrustStep);
            w("TrustLockUntil", TrustLockUntil);
            w("CourageLockUntil", CourageLockUntil);
            w("BossLockUntil", BossLockUntil);
            w("BossClears", BossClears);
            w("BossLastClearDay", BossLastClearDay);
            w("CaravanRuns", CaravanRuns);
            w("CaravanPointsTotal", CaravanPointsTotal);
            w("CaravanDay", CaravanDay);
            w("DailyDay", DailyDay);
            w("DailyTasks", string.Join(",", DailyTasks));
            w("DailyProgress", string.Join(",", DailyProgress));
            w("DailyDoneMask", DailyDoneMask);
            w("DailyStampGiven", DailyStampGiven ? 1 : 0);
            w("ErrandStep", ErrandStep);
            w("RiddleOfDayDone", RiddleOfDayDone);
            w("MakeupKills", MakeupKills);
            w("MakeupDone", MakeupDone ? 1 : 0);
            w("DailyStamps", DailyStamps);
            w("LanternStep", LanternStep);
            w("LanternTargets", LanternTargets ?? "");
            w("Claims", string.Join("|", Claims.OrderBy(x => x, StringComparer.Ordinal)));
            w("Bought", string.Join("|", Bought.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => x.Key + ":" + x.Value)));
            w("End", "OK"); // truncated files are detected by the missing End marker
            return sb.ToString();
        }

        public static RamadanPlayerState Deserialize(string text, out string error)
        {
            error = null;
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                int eq = raw.IndexOf('=');
                if (eq > 0) d[raw.Substring(0, eq)] = raw.Substring(eq + 1);
            }
            string end;
            if (!d.TryGetValue("End", out end) || end != "OK") { error = "file is truncated or corrupt"; return null; }
            var s = new RamadanPlayerState();
            Func<string, long> L = k => { string v; long r; return d.TryGetValue(k, out v) && long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out r) ? r : 0; };
            Func<string, string> S = k => { string v; return d.TryGetValue(k, out v) ? v : ""; };
            s.UID = (uint)L("UID");
            s.Name = S("Name");
            s.Stage = (QuestStage)Math.Max(0, Math.Min(7, (int)L("Stage")));
            s.Clues = (ClueFlags)((int)L("Clues") & (int)ClueFlags.All);
            s.Seals = (SealFlags)((int)L("Seals") & (int)SealFlags.All);
            s.Tokens = (int)Math.Max(0, L("Tokens"));
            s.TokensEarned = Math.Max(0, L("TokensEarned"));
            s.Points = (int)Math.Max(0, L("Points"));
            s.StampMask = (uint)L("StampMask");
            s.WisdomCorrect = (int)L("WisdomCorrect");
            s.WisdomRiddle = d.ContainsKey("WisdomRiddle") ? (int)L("WisdomRiddle") : -1;
            s.WisdomOrder = S("WisdomOrder");
            s.WisdomLockUntil = L("WisdomLockUntil");
            s.TrustStep = (int)L("TrustStep");
            s.TrustLockUntil = L("TrustLockUntil");
            s.CourageLockUntil = L("CourageLockUntil");
            s.BossLockUntil = L("BossLockUntil");
            s.BossClears = (int)L("BossClears");
            s.BossLastClearDay = d.ContainsKey("BossLastClearDay") ? (int)L("BossLastClearDay") : -1;
            s.CaravanRuns = (int)L("CaravanRuns");
            s.CaravanPointsTotal = (int)L("CaravanPointsTotal");
            s.CaravanDay = d.ContainsKey("CaravanDay") ? (int)L("CaravanDay") : -1;
            s.DailyDay = d.ContainsKey("DailyDay") ? (int)L("DailyDay") : -1;
            s.DailyTasks = ParseInts(S("DailyTasks"), 3);
            s.DailyProgress = ParseInts(S("DailyProgress"), 3);
            s.DailyDoneMask = (int)L("DailyDoneMask");
            s.DailyStampGiven = L("DailyStampGiven") == 1;
            s.ErrandStep = (int)L("ErrandStep");
            s.RiddleOfDayDone = (int)L("RiddleOfDayDone");
            s.MakeupKills = (int)L("MakeupKills");
            s.MakeupDone = L("MakeupDone") == 1;
            s.DailyStamps = (int)L("DailyStamps");
            s.LanternStep = (int)Math.Max(0, Math.Min(Lantern.Done, L("LanternStep")));
            s.LanternTargets = S("LanternTargets");
            foreach (var c in S("Claims").Split('|')) if (c.Length > 0) s.Claims.Add(c);
            foreach (var b in S("Bought").Split('|'))
            {
                int c = b.LastIndexOf(':'); int n;
                if (c > 0 && int.TryParse(b.Substring(c + 1), out n)) s.Bought[b.Substring(0, c)] = n;
            }
            return s;
        }

        static int[] ParseInts(string s, int n)
        {
            var r = new int[n];
            var p = (s ?? "").Split(',');
            for (int i = 0; i < n && i < p.Length; i++) int.TryParse(p[i], out r[i]);
            return r;
        }
    }

    // ------------------------------------------------------------------ Crescent Lantern quest (pure helpers)
    public static class Lantern
    {
        public const int Pieces = 7, AllFound = 8, Done = 9, TotalSteps = 9;

        /// <summary>Warmth band for a distance in cells: 0 found, 1 burning, 2 hot, 3 warm, 4 cool, 5 cold.</summary>
        public static int Band(int d)
        {
            if (d <= 1) return 0;
            if (d <= 4) return 1;
            if (d <= 10) return 2;
            if (d <= 20) return 3;
            if (d <= 35) return 4;
            return 5;
        }

        public static string BandText(int band)
        {
            switch (band)
            {
                case 1: return "BURNING! The piece is right here.";
                case 2: return "Hot! You are very close.";
                case 3: return "Warm. You are getting closer.";
                case 4: return "Cool. The piece is not near.";
                default: return "Cold. The piece is far away.";
            }
        }

        /// <summary>Direction as the player sees it on screen (map x goes down-right, map y goes down-left).</summary>
        public static string ScreenDirection(int dx, int dy)
        {
            int sx = dx - dy;           // + right, - left
            int sy = dx + dy;           // + down,  - up
            if (sx == 0 && sy == 0) return "right here";
            double ang = Math.Atan2(sy, sx) * 180.0 / Math.PI; // 0 = right, 90 = down
            if (ang < 0) ang += 360;
            string[] names = { "to the right", "down and to the right", "down", "down and to the left", "to the left", "up and to the left", "up", "up and to the right" };
            return names[(int)Math.Round(ang / 45.0) % 8];
        }

        public static List<MapPoint> ParseTargets(string s)
        {
            var r = new List<MapPoint>();
            foreach (var part in (s ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var p = part.Split(':');
                uint m; ushort x, y;
                if (p.Length == 3 && uint.TryParse(p[0], out m) && ushort.TryParse(p[1], out x) && ushort.TryParse(p[2], out y)) r.Add(new MapPoint(m, x, y));
            }
            return r;
        }

        public static string FormatTargets(IEnumerable<MapPoint> pts)
        {
            return string.Join(";", pts.Select(p => p.Map + ":" + p.X + ":" + p.Y));
        }
    }

    // ------------------------------------------------------------------ pure rules
    public static class RamadanRules
    {
        public static long UnixNow(DateTime now) { return (long)(now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; }

        /// <summary>Server-side event day. Day boundaries are at DailyResetHour (server local time).
        /// Client time is never used, so reconnecting or changing the PC clock cannot trigger a reset.</summary>
        public static int EventDay(RamadanConfig cfg, DateTime now)
        {
            var shifted = now.AddHours(-cfg.DailyResetHour).Date;
            var start = cfg.Start.AddHours(-cfg.DailyResetHour).Date;
            return (int)Math.Floor((shifted - start).TotalDays);
        }

        static readonly int[][] DailyRotation = {
            new[] { (int)DailyTaskType.Hunt, (int)DailyTaskType.IftarErrand, (int)DailyTaskType.Riddle },
            new[] { (int)DailyTaskType.CrescentDust, (int)DailyTaskType.Elite, (int)DailyTaskType.IftarErrand },
            new[] { (int)DailyTaskType.Hunt, (int)DailyTaskType.Caravan, (int)DailyTaskType.Riddle },
            new[] { (int)DailyTaskType.CrescentDust, (int)DailyTaskType.IftarErrand, (int)DailyTaskType.Riddle },
            new[] { (int)DailyTaskType.Elite, (int)DailyTaskType.Caravan, (int)DailyTaskType.IftarErrand },
            new[] { (int)DailyTaskType.Hunt, (int)DailyTaskType.CrescentDust, (int)DailyTaskType.Riddle },
            new[] { (int)DailyTaskType.Elite, (int)DailyTaskType.Caravan, (int)DailyTaskType.Riddle },
        };

        public static int[] TasksForDay(int day)
        {
            var r = DailyRotation[((day % DailyRotation.Length) + DailyRotation.Length) % DailyRotation.Length];
            return new[] { r[0], r[1], r[2] };
        }

        public static int TaskTarget(RamadanConfig cfg, DailyTaskType t)
        {
            switch (t)
            {
                case DailyTaskType.Hunt: return cfg.DailyHuntCount;
                case DailyTaskType.CrescentDust: return cfg.DailyDustNeeded;
                case DailyTaskType.Elite: return cfg.DailyEliteCount;
                default: return 1;
            }
        }

        /// <summary>Rolls the daily block over when the server day changed. Returns true when a reset happened.</summary>
        public static bool EnsureDaily(RamadanConfig cfg, RamadanPlayerState s, DateTime now)
        {
            int day = EventDay(cfg, now);
            if (s.DailyDay == day) return false;
            s.DailyDay = day;
            s.DailyTasks = TasksForDay(day);
            s.DailyProgress = new int[3];
            s.DailyDoneMask = 0;
            s.DailyStampGiven = false;
            s.ErrandStep = 0;
            s.RiddleOfDayDone = 0;
            s.MakeupKills = 0;
            s.MakeupDone = false;
            s.Dirty = true;
            return true;
        }

        /// <summary>Adds progress to every task of the given type. Returns the indices of tasks that just completed.</summary>
        public static List<int> AddDailyProgress(RamadanConfig cfg, RamadanPlayerState s, DateTime now, DailyTaskType type, int amount)
        {
            var done = new List<int>();
            if (amount <= 0) return done;
            EnsureDaily(cfg, s, now);
            for (int i = 0; i < 3; i++)
            {
                if (s.DailyTasks[i] != (int)type) continue;
                if ((s.DailyDoneMask & (1 << i)) != 0) continue;
                int target = TaskTarget(cfg, type);
                int before = s.DailyProgress[i];
                s.DailyProgress[i] = Math.Min(target, before + amount);
                if (s.DailyProgress[i] != before) s.Dirty = true;
                if (s.DailyProgress[i] >= target) done.Add(i);
            }
            return done;
        }

        public static bool AllDailyDone(RamadanPlayerState s) { return (s.DailyDoneMask & 7) == 7; }

        /// <summary>How many daily stamps the player could have by now (one per started event day, capped).</summary>
        public static int DailyStampsAvailable(RamadanConfig cfg, DateTime now)
        {
            return Math.Max(0, Math.Min(Stamps.DailyCount, EventDay(cfg, now) + 1));
        }

        /// <summary>Makeup is offered only to players who are behind and already finished today's set.</summary>
        public static bool MakeupAvailable(RamadanConfig cfg, RamadanPlayerState s, DateTime now)
        {
            EnsureDaily(cfg, s, now);
            return s.DailyStampGiven && !s.MakeupDone && s.DailyStamps < Math.Min(Stamps.DailyCount, DailyStampsAvailable(cfg, now));
        }

        public static int NextDailyStamp(RamadanPlayerState s)
        {
            for (int n = Stamps.FirstDaily; n <= Stamps.Total; n++) if (!s.HasStamp(n)) return n;
            return 0;
        }

        public static bool GrantStamp(RamadanPlayerState s, int n)
        {
            if (n < 1 || n > Stamps.Total || s.HasStamp(n)) return false;
            s.StampMask |= 1u << (n - 1);
            if (n >= Stamps.FirstDaily) s.DailyStamps++;
            s.Dirty = true;
            return true;
        }

        /// <summary>Single entry point for idempotent rewards: the key is recorded exactly once.</summary>
        public static bool TryClaim(RamadanPlayerState s, string key)
        {
            if (string.IsNullOrEmpty(key) || s.Claims.Contains(key)) return false;
            s.Claims.Add(key);
            s.Dirty = true;
            return true;
        }

        public static void UndoClaim(RamadanPlayerState s, string key)
        {
            if (s.Claims.Remove(key)) s.Dirty = true;
        }

        public static void AddTokens(RamadanPlayerState s, int amount)
        {
            if (amount <= 0) return;
            long nv = (long)s.Tokens + amount;
            s.Tokens = (int)Math.Min(1000000, nv);
            s.TokensEarned += amount;
            s.Dirty = true;
        }

        public static bool SpendTokens(RamadanPlayerState s, int amount)
        {
            if (amount <= 0 || s.Tokens < amount) return false;
            s.Tokens -= amount;
            s.Dirty = true;
            return true;
        }

        // ---- quest state machine
        public static bool CanAdvance(RamadanPlayerState s, QuestStage to)
        {
            switch (to)
            {
                case QuestStage.Introduction: return s.Stage == QuestStage.NotStarted;
                case QuestStage.ClueHunt: return s.Stage == QuestStage.Introduction;
                case QuestStage.SealChallenges: return s.Stage == QuestStage.ClueHunt && (s.Clues & ClueFlags.All) == ClueFlags.All;
                case QuestStage.CommunityEvent: return s.Stage == QuestStage.SealChallenges && (s.Seals & SealFlags.All) == SealFlags.All;
                case QuestStage.FinalGateUnlocked: return s.Stage == QuestStage.CommunityEvent && s.CaravanRuns > 0;
                case QuestStage.BossDefeated: return s.Stage == QuestStage.FinalGateUnlocked;
                case QuestStage.Completed: return s.Stage == QuestStage.BossDefeated;
                default: return false;
            }
        }

        public static bool Advance(RamadanPlayerState s, QuestStage to)
        {
            if (!CanAdvance(s, to)) return false;
            s.Stage = to;
            s.Dirty = true;
            return true;
        }

        /// <summary>Clues must be found in order; returns false if the clue is not available yet.</summary>
        public static bool ClueAvailable(RamadanPlayerState s, ClueFlags clue)
        {
            if (s.Stage != QuestStage.ClueHunt || (s.Clues & clue) != 0) return false;
            if (clue == ClueFlags.Lamplighter) return true;
            if (clue == ClueFlags.ScorchedLantern) return (s.Clues & ClueFlags.Lamplighter) != 0;
            if (clue == ClueFlags.ShadowLetter) return (s.Clues & ClueFlags.ScorchedLantern) != 0;
            return false;
        }

        public static bool GiveClue(RamadanPlayerState s, ClueFlags clue)
        {
            if (!ClueAvailable(s, clue)) return false;
            s.Clues |= clue;
            s.Dirty = true;
            return true;
        }

        public static bool GiveSeal(RamadanPlayerState s, SealFlags seal)
        {
            if (s.Stage != QuestStage.SealChallenges || (s.Seals & seal) != 0) return false;
            s.Seals |= seal;
            s.Dirty = true;
            return true;
        }

        // ---- caravan
        public static int CaravanTokens(RamadanConfig cfg, int points, bool success)
        {
            if (points < cfg.CaravanMinContribution) return 0;
            int p = Math.Min(points, cfg.CaravanPointsCap);
            int t = Math.Min(cfg.CaravanTokensCap, cfg.CaravanTokensBase + p * cfg.CaravanTokensPerPoint);
            return success ? t : t / 2;
        }

        // ---- shop
        public static string CheckPurchase(RamadanConfig cfg, RamadanPlayerState s, ShopEntry e, DateTime now)
        {
            if (!cfg.ShopOpen(now)) return "The Crescent Bazaar is closed.";
            if (s.Tokens < e.Cost) return "You need " + e.Cost + " Crescent Tokens.";
            int b; s.Bought.TryGetValue(e.Key, out b);
            if (e.Limit > 0 && b >= e.Limit) return "You already bought this the maximum number of times (" + e.Limit + ").";
            return null;
        }

        public static void RecordPurchase(RamadanPlayerState s, ShopEntry e)
        {
            int b; s.Bought.TryGetValue(e.Key, out b);
            s.Bought[e.Key] = b + 1;
            s.Tokens -= e.Cost;
            s.Dirty = true;
        }

        public static void RollbackPurchase(RamadanPlayerState s, ShopEntry e)
        {
            int b; s.Bought.TryGetValue(e.Key, out b);
            if (b > 0) s.Bought[e.Key] = b - 1;
            s.Tokens += e.Cost;
            s.Dirty = true;
        }

        // ---- wisdom riddles
        public class Riddle { public string Q; public string[] Options; public int Answer; }

        public static readonly Riddle[] Riddles = {
            new Riddle { Q = "I am lit at dusk and guard the night; a cage of brass that holds a light. What am I?", Options = new[]{ "A lantern", "A sword", "A shield", "A bell" }, Answer = 0 },
            new Riddle { Q = "Thin as a blade I rise at night; grow me fourteen nights and I am round and bright. What am I?", Options = new[]{ "The crescent moon", "A comet", "The sun", "A star" }, Answer = 0 },
            new Riddle { Q = "Sweet fruit of the palm, shared first when the sun goes down. What is it?", Options = new[]{ "Dates", "Apples", "Grapes", "Figs" }, Answer = 0 },
            new Riddle { Q = "The Guardian's four virtues are Courage, Wisdom and Unity. Which one completes them?", Options = new[]{ "Trust", "Fury", "Greed", "Fear" }, Answer = 0 },
            new Riddle { Q = "What grows larger the more of it you give away?", Options = new[]{ "Kindness", "Gold", "A loaf of bread", "A candle" }, Answer = 0 },
            new Riddle { Q = "The light is first kindled, then guarded, then shared. Which order restores the crescent?", Options = new[]{ "Kindle, Guard, Share", "Share, Kindle, Guard", "Guard, Share, Kindle", "Kindle, Share, Guard" }, Answer = 0 },
            new Riddle { Q = "A caravan crosses the desert at night. What keeps it together?", Options = new[]{ "Travelling side by side", "The fastest camel", "The heaviest chest", "The loudest drum" }, Answer = 0 },
            new Riddle { Q = "The shadows fear only one thing they cannot steal. What is it?", Options = new[]{ "Light freely shared", "A locked chest", "A sharp blade", "A tall wall" }, Answer = 0 },
        };

        /// <summary>Picks a riddle and a shuffled option order. The order is stored server-side and the answer is checked against it.</summary>
        public static void NewRiddle(RamadanPlayerState s, Random rnd, int avoid = -1)
        {
            int id;
            do { id = rnd.Next(Riddles.Length); } while (id == avoid && Riddles.Length > 1);
            var order = Enumerable.Range(0, 4).OrderBy(x => rnd.Next()).ToArray();
            s.WisdomRiddle = id;
            s.WisdomOrder = string.Concat(order.Select(x => x.ToString(CultureInfo.InvariantCulture)));
            s.Dirty = true;
        }

        /// <summary>option is the dialog option index (0..3). Returns true when correct. Invalid state returns false.</summary>
        public static bool CheckRiddle(RamadanPlayerState s, int option)
        {
            if (s.WisdomRiddle < 0 || s.WisdomRiddle >= Riddles.Length || s.WisdomOrder == null || s.WisdomOrder.Length != 4) return false;
            if (option < 0 || option > 3) return false;
            int original = s.WisdomOrder[option] - '0';
            return original == Riddles[s.WisdomRiddle].Answer;
        }

        // ---- leaderboard
        public static List<KeyValuePair<string, int>> Top(IEnumerable<KeyValuePair<string, int>> entries, int n)
        {
            return entries.Where(e => e.Value > 0).OrderByDescending(e => e.Value).ThenBy(e => e.Key, StringComparer.Ordinal).Take(n).ToList();
        }
    }

    // ------------------------------------------------------------------ persistence
    public class RamadanStore
    {
        public readonly string Root;
        readonly object fileLock = new object();
        readonly object logLock = new object();

        public RamadanStore(string root)
        {
            Root = root;
            Directory.CreateDirectory(Path.Combine(Root, "Players"));
            Directory.CreateDirectory(Path.Combine(Root, "Logs"));
        }

        public string PlayerPath(uint uid) { return Path.Combine(Root, "Players", uid + ".ini"); }

        /// <summary>Atomic write: write a temp file then replace. A crash mid-write never leaves a half file in place.</summary>
        public static void AtomicWrite(string path, string content)
        {
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, content, new UTF8Encoding(false));
            if (File.Exists(path))
            {
                try { File.Replace(tmp, path, path + ".bak", true); return; }
                catch (PlatformNotSupportedException) { }
                catch (IOException) { }
                File.Copy(path, path + ".bak", true);
                File.Delete(path);
            }
            File.Move(tmp, path);
        }

        public RamadanPlayerState LoadPlayer(uint uid)
        {
            lock (fileLock)
            {
                var path = PlayerPath(uid);
                foreach (var p in new[] { path, path + ".bak" })
                {
                    if (!File.Exists(p)) continue;
                    string err;
                    var s = RamadanPlayerState.Deserialize(File.ReadAllText(p, Encoding.UTF8), out err);
                    if (s != null) { s.UID = uid; return s; }
                    Log(uid, "", "load-error", p + ": " + err);
                }
                return new RamadanPlayerState { UID = uid };
            }
        }

        public bool SavePlayer(RamadanPlayerState s)
        {
            string text;
            lock (s.Sync) { text = s.Serialize(); s.Dirty = false; }
            try
            {
                lock (fileLock) AtomicWrite(PlayerPath(s.UID), text);
                return true;
            }
            catch (Exception e)
            {
                lock (s.Sync) s.Dirty = true;
                Log(s.UID, s.Name, "save-error", e.Message);
                return false;
            }
        }

        public string ReadGlobal(string name)
        {
            var p = Path.Combine(Root, name);
            lock (fileLock) return File.Exists(p) ? File.ReadAllText(p, Encoding.UTF8) : null;
        }

        public void WriteGlobal(string name, string content)
        {
            lock (fileLock) AtomicWrite(Path.Combine(Root, name), content);
        }

        public IEnumerable<uint> AllPlayerUids()
        {
            foreach (var f in Directory.GetFiles(Path.Combine(Root, "Players"), "*.ini"))
            {
                uint u;
                if (uint.TryParse(Path.GetFileNameWithoutExtension(f), out u)) yield return u;
            }
        }

        public Func<DateTime> Clock = () => DateTime.Now;

        /// <summary>Audit log. Never logs passwords or connection data: only UID, character name and event action.</summary>
        public void Log(uint uid, string name, string action, string details)
        {
            try
            {
                var now = Clock();
                var line = now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "\t" + uid + "\t" + (name ?? "") + "\t" + action + "\t" + (details ?? "").Replace('\n', ' ').Replace('\r', ' ') + "\r\n";
                lock (logLock) File.AppendAllText(Path.Combine(Root, "Logs", "ramadan-" + now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".log"), line, new UTF8Encoding(false));
            }
            catch { }
        }
    }
}
