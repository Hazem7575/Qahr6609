// PlayerStudio ops of the AdminApi (player administration), ported from Qahr's Base\AdminApi\AdminApi.cs so the
// released PlayerStudio (Qahr6609.Tools\Releases\PlayerStudio) works with this server unchanged: same op names,
// same request fields, same reply shapes and error texts.
//
//   read : ping (+server/version/online/maxLevel), online, player {uid|name}, search {q,limit}, bans, packages
//   edit : setlevel {level} | maxlevel | addexp {amount} | money/cps/boundcps {amount,mode:set|add|sub} | vip {level,days}
//          reborn {count} | giveitem {id,amount,plus,bless,enchant,socket1,socket2,bound} | removeitem {itemUid}
//          clearinventory | givepackage {package} | teleport {map,x,y,force} | bring {toUid} | revive | kick {reason}
//          ban {hours,reason} | unban | chatban {minutes} | unchatban | freeze {seconds} | unfreeze
//          message {text,popup} | learnskill {skill,level} | removeskill {skill} | save {uid | all:true}
//   all  : broadcast {text,mode:center|broadcast|system|talk} | massmessage {text,popup} | massgive {item.. | package, minLevel}
//
// The target is {"uid":N} or {"name":"..."}. Online players are changed with the same game calls the GM commands use
// (Admin\Commands\*.cs: UpdateLevel, Inventory.Add/AddItemWitchStack, MySpells.Add/Remove, Teleport, Revive,
// SystemBannedAccount, ...) and saved right after with ServerDatabase.SaveClient. The ops never run in parallel with
// each other (one lock), like Qahr's single admin thread.
// Offline players (only level / money / CPs / bound CPs / position, and only with "allowOffline":true) are written to
// Users\<uid>.ini through IniFile (its Written hook keeps the MySQL copy in step), after FlushPendingSaves, a
// timestamped backup in ServerDataBase\BackUp\AdminApi\ and only when the character is not in GamePoll.
// packages.json lives next to adminapi.txt (ServerDataBase) and is edited by PlayerStudio; it is read on every give.
//
// 6609 differences handled here:
//  * Player.Money / ConquerPoints / BoundConquerPoints setters auto-BAN a non-GM whose balance moves by more than
//    10,000,000,000 / 1,000,000,000 / 100,000,000 in one set: big changes are applied in steps below those limits.
//  * Money above 50,000,000,000 is moved to the warehouse by the setter, so that is the money cap here.
//  * Inventory.Clear only takes one from a stack: clearinventory removes every item whole.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Conquer6609
{
    public static partial class AdminApi
    {
        const string PlayersApiVersion = "6609-1.0";
        static readonly object playerOpLock = new object();

        sealed class AdminError : Exception
        {
            public AdminError(string message) : base(message) { }
        }

        // ------------------------------------------------------------------ dispatch
        static JObject PlayerOp(string op, JObject req)
        {
            Func<A, JObject> f;
            switch (op)
            {
                case "player": f = PlayerInfo; break;
                case "search": f = Search; break;
                case "setlevel": f = a => SetLevel(a, false); break;
                case "maxlevel": f = a => SetLevel(a, true); break;
                case "addexp": f = AddExp; break;
                case "money": f = MoneyOp; break;
                case "cps": f = a => CpsOp(a, false); break;
                case "boundcps": f = a => CpsOp(a, true); break;
                case "vip": f = VipOp; break;
                case "reborn": f = RebornOp; break;
                case "giveitem": f = GiveItem; break;
                case "removeitem": f = RemoveItem; break;
                case "clearinventory": f = ClearInventory; break;
                case "packages": f = a => PackagesOp(); break;
                case "givepackage": f = GivePackage; break;
                case "teleport": f = TeleportOp; break;
                case "bring": f = Bring; break;
                case "revive": f = Revive; break;
                case "kick": f = Kick; break;
                case "ban": f = Ban; break;
                case "unban": f = Unban; break;
                case "bans": f = a => Bans(); break;
                case "chatban": f = a => ChatBan(a, true); break;
                case "unchatban": f = a => ChatBan(a, false); break;
                case "freeze": f = a => Freeze(a, true); break;
                case "unfreeze": f = a => Freeze(a, false); break;
                case "message": f = MessageOp; break;
                case "broadcast": f = Broadcast; break;
                case "massmessage": f = MassMessage; break;
                case "learnskill": f = LearnSkill; break;
                case "removeskill": f = RemoveSkill; break;
                case "save": f = SaveOp; break;
                case "massgive": f = MassGive; break;
                default: return null;
            }
            var args = new A(req);
            try
            {
                lock (playerOpLock)
                    return f(args) ?? Ok(null);
            }
            catch (AdminError e)
            {
                if (op != "player" && op != "search" && op != "bans" && op != "packages")
                    Log(op, TargetOf(args) + " FAILED: " + e.Message, false);
                return Fail(e.Message);
            }
        }

        // request arguments (Qahr's AdminArgs over a Newtonsoft JObject)
        sealed class A
        {
            readonly JObject o;
            public A(JObject o) { this.o = o ?? new JObject(); }
            public bool Has(string k) { var t = o[k]; return t != null && t.Type != JTokenType.Null; }
            public string Str(string k, string def = null) { return Has(k) ? o[k].ToString() : def; }
            public long Long(string k, long def = 0)
            {
                if (!Has(k)) return def;
                var t = o[k];
                try
                {
                    if (t.Type == JTokenType.Integer || t.Type == JTokenType.Float) return (long)(decimal)t;
                    if (t.Type == JTokenType.Boolean) return (bool)t ? 1 : 0;
                }
                catch { throw new AdminError("'" + k + "' is out of range"); }
                long v;
                if (long.TryParse(t.ToString().Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v)) return v;
                throw new AdminError("'" + k + "' must be a number");
            }
            public ulong ULong(string k)
            {
                if (!Has(k)) return 0;
                var t = o[k];
                ulong v;
                try
                {
                    if (t.Type == JTokenType.Integer || t.Type == JTokenType.Float)
                    {
                        decimal d = (decimal)t;
                        if (d < 0) throw new AdminError("'" + k + "' must be >= 0");
                        return (ulong)d;
                    }
                }
                catch (AdminError) { throw; }
                catch { throw new AdminError("'" + k + "' is out of range"); }
                if (ulong.TryParse(t.ToString().Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v)) return v;
                throw new AdminError("'" + k + "' must be a number >= 0");
            }
            public bool Bool(string k, bool def = false)
            {
                if (!Has(k)) return def;
                var t = o[k];
                if (t.Type == JTokenType.Boolean) return (bool)t;
                string s = t.ToString().Trim();
                return s == "1" || s.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            public long Need(string k, long min, long max)
            {
                if (!Has(k)) throw new AdminError("missing '" + k + "'");
                long v = Long(k);
                if (v < min || v > max) throw new AdminError("'" + k + "' must be " + min + "-" + max);
                return v;
            }
            public JArray List(string k) { return Has(k) ? o[k] as JArray : null; }
        }

        static string TargetOf(A a)
        {
            if (a.Has("uid")) return "uid=" + a.Str("uid");
            if (a.Has("name")) return "name=" + a.Str("name");
            return "-";
        }

        static void PLog(string op, string target, string details, bool ok)
        {
            Log(op, target + " | " + (details ?? "").Replace("\r", " ").Replace("\n", " "), ok);
        }

        static string Dt(DateTime d) { return d.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); }

        // ------------------------------------------------------------------ helpers
        static int MaxLevel
        {
            get
            {
                try
                {
                    Dictionary<byte, Database.DBLevExp> t;
                    if (Database.Server.LevelInfo.TryGetValue(Database.DBLevExp.Sort.User, out t) && t.Count > 0)
                        return Math.Min(140, (int)t.Keys.Max());
                }
                catch { }
                return 140;
            }
        }

        static IEnumerable<Client.GameClient> OnlineClients()
        {
            foreach (var c in Database.Server.GamePoll.Values.ToArray())
                if (c != null && c.Player != null && c.FullLoading && c.Socket != null && c.Socket.Alive)
                    yield return c;
        }

        static Client.GameClient FindOnline(A a)
        {
            if (a.Has("uid"))
            {
                uint uid = (uint)a.Need("uid", 1, uint.MaxValue);
                Client.GameClient c;
                if (Database.Server.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && c.FullLoading)
                    return c;
                return null;
            }
            string name = a.Str("name");
            if (string.IsNullOrEmpty(name)) throw new AdminError("missing 'uid' or 'name'");
            foreach (var c in OnlineClients())
                if (string.Equals(c.Player.Name, name, StringComparison.OrdinalIgnoreCase))
                    return c;
            return null;
        }

        static Client.GameClient NeedOnline(A a)
        {
            var c = FindOnline(a);
            if (c == null) throw new AdminError("player is not online");
            return c;
        }

        static string Who(Client.GameClient c) { return c.Player.Name + " (" + c.Player.UID + ")"; }

        static bool IsGm(Client.GameClient c) { return c.GameMaster || c.ProjectManager; }

        /// <summary>Saves the character now (the same SaveClient the save queue runs; serialized per character).</summary>
        static void SafeSave(Client.GameClient c)
        {
            try
            {
                if (Program.OnMainternance) return;
                if (c.OnInterServer || c.Socket == null || !c.Socket.Alive || !c.FullLoading || c.Fake) return;
                Database.ServerDatabase.SaveClient(c);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        static int SafeInt(Func<int> f) { try { return f(); } catch { return 0; } }
        static string SafeStr(Func<string> f) { try { return f() ?? ""; } catch { return ""; } }
        static bool SafeBool(Func<bool> f) { try { return f(); } catch { return false; } }

        static string ItemName(uint id)
        {
            Database.ItemType.DBItem db;
            if (Database.Server.ItemsBase != null && Database.Server.ItemsBase.TryGetValue(id, out db) && db != null && db.Name != null)
                return db.Name.Replace('~', ' ');
            return "Item " + id;
        }

        static string SpellName(ushort id)
        {
            try
            {
                Dictionary<ushort, Database.MagicType.Magic> m;
                if (Database.Server.Magic != null && Database.Server.Magic.TryGetValue(id, out m) && m.Count > 0)
                {
                    var first = m.Values.First();
                    if (!string.IsNullOrEmpty(first.Name)) return first.Name.Replace('~', ' ');
                }
            }
            catch { }
            return "Skill " + id;
        }

        static JObject Row(Client.GameClient c)
        {
            var p = c.Player;
            return new JObject
            {
                ["uid"] = p.UID,
                ["name"] = p.Name,
                ["level"] = p.Level,
                ["cls"] = p.Class,
                ["reborn"] = p.Reborn,
                ["map"] = p.Map,
                ["x"] = p.X,
                ["y"] = p.Y,
                ["money"] = p.Money,
                ["cps"] = p.ConquerPoints,
                ["boundCps"] = p.BoundConquerPoints,
                ["vip"] = SafeInt(() => p.VipLevel),
                ["bp"] = SafeInt(() => p.BattlePower),
                ["hp"] = SafeInt(() => p.HitPoints),
                ["maxHp"] = c.Status != null ? c.Status.MaxHitpoints : 0u,
                ["alive"] = SafeBool(() => p.Alive),
                ["gm"] = IsGm(c),
                ["bot"] = c.Fake,
            };
        }

        static JObject ItemObj(Game.MsgServer.MsgGameItem it)
        {
            return new JObject
            {
                ["uid"] = it.UID,
                ["id"] = it.ITEM_ID,
                ["name"] = ItemName(it.ITEM_ID),
                ["pos"] = it.Position,
                ["plus"] = it.Plus,
                ["bless"] = it.Bless,
                ["enchant"] = it.Enchant,
                ["s1"] = (byte)it.SocketOne,
                ["s2"] = (byte)it.SocketTwo,
                ["bound"] = it.Bound != 0,
                ["stack"] = it.StackSize,
                ["locked"] = it.Locked != 0,
            };
        }

        // ------------------------------------------------------------------ read ops
        static JObject PlayersPing(JObject r)
        {
            r["server"] = Program.ServerConfig.ServerName;
            r["version"] = PlayersApiVersion;
            r["online"] = OnlineClients().Count();
            r["maxLevel"] = MaxLevel;
            r["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            return r;
        }

        static JObject PlayersOnline()
        {
            var list = new JArray();
            foreach (var c in Database.Server.GamePoll.Values.ToArray().Where(x => x != null && x.Player != null && x.FullLoading).OrderByDescending(x => x.Player.Level))
            {
                try { list.Add(Row(c)); } catch { }
            }
            var r = Ok(list.Count + " online");
            r["players"] = list;
            r["maxLevel"] = MaxLevel;
            return r;
        }

        static JObject PlayerInfo(A a)
        {
            var c = FindOnline(a);
            if (c == null)
            {
                uint uid = ResolveOfflineUid(a);
                var off = ReadOfflineIni(uid);
                if (off == null) throw new AdminError("player not found");
                var r = Ok(null);
                r["online"] = false;
                r["player"] = off;
                r["banned"] = BanInfo(uid);
                return r;
            }
            var p = c.Player;
            var row = Row(c);
            row["exp"] = p.Experience;
            row["str"] = p.Strength;
            row["agi"] = p.Agility;
            row["vit"] = p.Vitality;
            row["spi"] = p.Spirit;
            row["atr"] = p.Atributes;
            row["pk"] = p.PKPoints;
            row["mana"] = p.Mana;
            row["maxMana"] = c.Status != null ? c.Status.MaxMana : 0u;
            row["vipExpire"] = SafeStr(() => Dt(p.ExpireVip));
            row["firstClass"] = p.FirstClass;
            row["secondClass"] = p.SecoundeClass;
            row["guild"] = SafeStr(() => p.MyGuild != null ? p.MyGuild.GuildName : "");
            row["ip"] = SafeStr(() => c.Socket.RemoteIp);
            row["chatBanned"] = p.IsBannedChat;
            row["frozen"] = SafeBool(() => p.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Freeze));
            var inv = new JArray();
            foreach (var it in c.Inventory.ClientItems.Values.ToArray().OrderBy(i => i.Position).ThenBy(i => i.UID))
                inv.Add(ItemObj(it));
            var eq = new JArray();
            foreach (var it in c.Equipment.ClientItems.Values.ToArray().OrderBy(i => i.Position))
                eq.Add(ItemObj(it));
            var spells = new JArray();
            foreach (var s in c.MySpells.ClientSpells.Values.ToArray().OrderBy(s => s.ID))
                spells.Add(new JObject { ["id"] = s.ID, ["level"] = s.Level, ["name"] = SpellName(s.ID) });
            row["inventory"] = inv;
            row["inventoryMax"] = 40;
            row["equipment"] = eq;
            row["skills"] = spells;
            var d = Ok(null);
            d["online"] = true;
            d["player"] = row;
            d["banned"] = BanInfo(p.UID);
            return d;
        }

        static JToken BanInfo(uint uid)
        {
            try
            {
                if (!Database.SystemBannedAccount.BannedPoll.ContainsKey(uid)) return JValue.CreateNull();
                var b = Database.SystemBannedAccount.BannedPoll[uid];
                if (b == null) return JValue.CreateNull();
                var start = DateTime.FromBinary(b.StartBan);
                return new JObject
                {
                    ["hours"] = b.Hours,
                    ["since"] = Dt(start),
                    ["until"] = Dt(start.AddHours(b.Hours)),
                    ["reason"] = b.Reason ?? "",
                };
            }
            catch { return JValue.CreateNull(); }
        }

        static JObject Search(A a)
        {
            string q = (a.Str("q") ?? "").Trim();
            int limit = (int)Math.Max(1, Math.Min(500, a.Long("limit", 200)));
            var list = new JArray();
            var seen = new HashSet<uint>();
            foreach (var c in OnlineClients())
            {
                if (q.Length > 0 && c.Player.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0 && c.Player.UID.ToString() != q) continue;
                var r = Row(c);
                r["online"] = true;
                list.Add(r);
                seen.Add(c.Player.UID);
            }
            string dir = UsersDir;
            if (Directory.Exists(dir))
            {
                int scanned = 0;
                foreach (string f in Directory.EnumerateFiles(dir, "*.ini"))
                {
                    if (list.Count >= limit || ++scanned > 50000) break;
                    uint uid;
                    if (!uint.TryParse(Path.GetFileNameWithoutExtension(f), out uid) || seen.Contains(uid)) continue;
                    var kv = ReadIniCharacter(f);
                    if (kv == null) continue;
                    string name = Get(kv, "Name", "");
                    if (q.Length > 0 && name.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0 && uid.ToString() != q) continue;
                    list.Add(OfflineRow(uid, kv));
                }
            }
            var d = Ok(null);
            d["players"] = list;
            return d;
        }

        // ------------------------------------------------------------------ offline (Users\<uid>.ini)
        static readonly Dictionary<uint, DateTime> offlineWrites = new Dictionary<uint, DateTime>();

        static string UsersDir { get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "Users"); } }
        static string UserIni(uint uid) { return Path.Combine(UsersDir, uid + ".ini"); }

        // the [Character] section, decoded like GetPrivateProfileString (UTF-16 with BOM, else the ANSI code page)
        static Dictionary<string, string> ReadIniCharacter(string path)
        {
            try
            {
                byte[] bytes;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                string text = bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE
                    ? Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2)
                    : AnsiText.Value.GetString(bytes);
                var kv = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                bool inChar = false;
                foreach (string raw in text.Split('\n'))
                {
                    string line = raw.Trim();
                    if (line.StartsWith("["))
                    {
                        inChar = line.Equals("[Character]", StringComparison.OrdinalIgnoreCase);
                        continue;
                    }
                    if (!inChar) continue;
                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = line.Substring(0, eq).Trim();
                    if (!kv.ContainsKey(k)) kv[k] = line.Substring(eq + 1).Trim();
                }
                return kv;
            }
            catch { return null; }
        }

        static string Get(Dictionary<string, string> kv, string k, string def)
        {
            string v;
            return kv.TryGetValue(k, out v) ? v : def;
        }

        static long GetL(Dictionary<string, string> kv, string k)
        {
            long v;
            return long.TryParse(Get(kv, k, "0"), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0;
        }

        static JObject OfflineRow(uint uid, Dictionary<string, string> kv)
        {
            return new JObject
            {
                ["uid"] = uid,
                ["name"] = Get(kv, "Name", "?"),
                ["level"] = GetL(kv, "Level"),
                ["cls"] = GetL(kv, "Class"),
                ["reborn"] = GetL(kv, "Reborn"),
                ["map"] = GetL(kv, "Map"),
                ["x"] = GetL(kv, "X"),
                ["y"] = GetL(kv, "Y"),
                ["money"] = Math.Max(0, GetL(kv, "Money")),
                ["cps"] = GetL(kv, "ConquerPoints"),
                ["boundCps"] = GetL(kv, "BoundConquerPoints"),
                ["vip"] = GetL(kv, "VipLevel"),
                ["online"] = false,
            };
        }

        static JObject ReadOfflineIni(uint uid)
        {
            string f = UserIni(uid);
            if (!File.Exists(f)) return null;
            var kv = ReadIniCharacter(f);
            if (kv == null) return null;
            var d = OfflineRow(uid, kv);
            d["exp"] = Get(kv, "Experience", "0");
            d["str"] = GetL(kv, "Strength");
            d["agi"] = GetL(kv, "Agility");
            d["vit"] = GetL(kv, "Vitaliti");
            d["spi"] = GetL(kv, "Spirit");
            d["atr"] = GetL(kv, "Atributes");
            d["pk"] = GetL(kv, "PkPoints");
            long vt = GetL(kv, "VipTime");
            try { d["vipExpire"] = Dt(DateTime.FromBinary(vt)); } catch { }
            d["chatBanned"] = Get(kv, "IsBannedChat", "False").Equals("True", StringComparison.OrdinalIgnoreCase);
            d["lastSaved"] = Dt(File.GetLastWriteTime(f));
            return d;
        }

        static uint ResolveOfflineUid(A a)
        {
            if (a.Has("uid")) return (uint)a.Need("uid", 1, uint.MaxValue);
            string name = a.Str("name");
            if (string.IsNullOrEmpty(name)) throw new AdminError("missing 'uid' or 'name'");
            string dir = UsersDir;
            if (Directory.Exists(dir))
                foreach (string f in Directory.EnumerateFiles(dir, "*.ini"))
                {
                    var kv = ReadIniCharacter(f);
                    uint uid;
                    if (kv != null && string.Equals(Get(kv, "Name", ""), name, StringComparison.OrdinalIgnoreCase)
                        && uint.TryParse(Path.GetFileNameWithoutExtension(f), out uid))
                        return uid;
                }
            throw new AdminError("player '" + name + "' not found");
        }

        /// <summary>
        /// Writes [Character] keys of an OFFLINE player: not in GamePoll, its queued saves flushed first, file not saved
        /// by the server in the last 10 s, after a timestamped backup. Same IniFile path as the server's other outside
        /// writes (ServerDatabase guild/clan/spouse updates): InvalidatePlayerIni + IniFile.WriteString (MySQL hook).
        /// </summary>
        static JObject OfflineWrite(A a, string op, Dictionary<string, string> values)
        {
            if (!a.Bool("allowOffline")) throw new AdminError("player is not online (offline edit not confirmed)");
            uint uid = ResolveOfflineUid(a);
            string f = UserIni(uid);
            if (!File.Exists(f)) throw new AdminError("player file not found");
            if (Program.OnMainternance) throw new AdminError("server is shutting down");
            if (Database.Server.GamePoll.ContainsKey(uid))
                throw new AdminError("player is online or still being saved - try again in a few seconds");
            Database.ServerDatabase.FlushPendingSaves(uid, null);
            if (Database.Server.GamePoll.ContainsKey(uid))
                throw new AdminError("player is online or still being saved - try again in a few seconds");
            DateTime mtime = File.GetLastWriteTimeUtc(f), ours;
            bool lastWriteWasOurs = offlineWrites.TryGetValue(uid, out ours) && ours == mtime;
            if (!lastWriteWasOurs && (DateTime.UtcNow - mtime).TotalSeconds < 10)
                throw new AdminError("player file was saved a moment ago - try again in a few seconds");
            string bakDir = Path.Combine(Program.ServerConfig.DbLocation ?? "", "BackUp", "AdminApi");
            Directory.CreateDirectory(bakDir);
            string bak = Path.Combine(bakDir, uid + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture) + ".ini");
            File.Copy(f, bak, false);
            Database.ServerDatabase.InvalidatePlayerIni(uid);
            var ini = new WindowsAPI.IniFile("\\Users\\" + uid + ".ini");
            foreach (var kv in values)
                ini.WriteString("Character", kv.Key, kv.Value);
            offlineWrites[uid] = File.GetLastWriteTimeUtc(f);
            string details = string.Join(", ", values.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + " (offline, backup " + Path.GetFileName(bak) + ")";
            PLog(op, "uid=" + uid, details, true);
            return Ok("offline player " + uid + ": " + details);
        }

        // ------------------------------------------------------------------ level / exp
        static JObject SetLevel(A a, bool max)
        {
            int maxLevel = MaxLevel;
            int level = max ? maxLevel : (int)a.Need("level", 1, maxLevel);
            var c = FindOnline(a);
            if (c == null)
                return OfflineWrite(a, max ? "maxlevel" : "setlevel", new Dictionary<string, string> { { "Level", level.ToString(CultureInfo.InvariantCulture) }, { "Experience", "0" } });
            int old = c.Player.Level;
            using (var rec = new ServerSockets.RecycledPacket())
                c.UpdateLevel(rec.GetStream(), (ushort)level, true);// @level
            SafeSave(c);
            PLog(max ? "maxlevel" : "setlevel", Who(c), "level " + old + " -> " + level, true);
            return Ok(Who(c) + ": level " + old + " -> " + level);
        }

        static JObject AddExp(A a)
        {
            var c = NeedOnline(a);
            ulong amount = a.ULong("amount");
            if (amount == 0) throw new AdminError("'amount' must be > 0");
            int maxLevel = MaxLevel;
            int old = c.Player.Level;
            Dictionary<byte, Database.DBLevExp> table;
            if (!Database.Server.LevelInfo.TryGetValue(Database.DBLevExp.Sort.User, out table)) throw new AdminError("level table not loaded");
            var p = c.Player;
            if (p.Level >= maxLevel) throw new AdminError("player is already at max level " + maxLevel);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                // the level-up loop of GameClient.IncreaseExperience, without exp rates / gems / map multipliers
                p.Experience += amount;
                Database.DBLevExp row;
                while (p.Level < maxLevel && table.TryGetValue((byte)p.Level, out row) && row.Experience > 0 && p.Experience >= row.Experience)
                {
                    p.Experience -= row.Experience;
                    c.UpdateLevel(stream, (ushort)(p.Level + 1));
                }
                if (p.Level >= maxLevel) p.Experience = 0;
                c.UpdateRebornLastLevel(stream);
                p.SendUpdate(stream, (long)p.Experience, Game.MsgServer.MsgUpdate.DataType.Experience, false);
            }
            SafeSave(c);
            PLog("addexp", Who(c), "+" + amount + " exp, level " + old + " -> " + c.Player.Level, true);
            return Ok(Who(c) + ": +" + amount.ToString("N0", CultureInfo.InvariantCulture) + " exp, level " + old + " -> " + c.Player.Level);
        }

        // ------------------------------------------------------------------ money / cps / vip / reborn
        const long MoneyCap = 50000000000L;        // Player.Money moves anything above this to the warehouse
        const long CpsCap = 2147000000L;           // ConquerPoints above int.MaxValue = auto-ban for non-GMs
        const long BoundCpsCap = int.MaxValue;
        // Player.Money/ConquerPoints/BoundConquerPoints auto-ban a non-GM whose balance jumps by MORE than these in one set
        const long MoneyStep = 10000000000L, CpsStep = 1000000000L, BoundCpsStep = 100000000L;

        static ulong Apply(string mode, ulong current, ulong amount, ulong cap)
        {
            switch ((mode ?? "set").ToLowerInvariant())
            {
                case "add": return Math.Min(cap, current + Math.Min(amount, cap));
                case "sub": return amount >= current ? 0 : current - amount;
                case "set": return Math.Min(cap, amount);
                default: throw new AdminError("'mode' must be set, add or sub");
            }
        }

        /// <summary>Moves a balance to target through its setter in steps the setter's anti-hack check accepts.</summary>
        static void StepTo(long current, long target, long step, Action<long> set)
        {
            int guard = 0;
            while (current != target && guard++ < 64)
            {
                long next = target > current ? Math.Min(target, current + step) : Math.Max(target, current - step);
                set(next);
                current = next;
            }
        }

        static void SetMoney(Client.GameClient c, long nv) { StepTo(c.Player.Money, nv, MoneyStep, v => c.Player.Money = v); }
        static void SetCps(Client.GameClient c, long nv) { StepTo(c.Player.ConquerPoints, nv, CpsStep, v => c.Player.ConquerPoints = (uint)v); }
        static void SetBoundCps(Client.GameClient c, long nv) { StepTo(c.Player.BoundConquerPoints, nv, BoundCpsStep, v => c.Player.BoundConquerPoints = (int)v); }

        static JObject MoneyOp(A a)
        {
            ulong amount = a.ULong("amount");
            string mode = a.Str("mode", "set");
            var c = FindOnline(a);
            if (c == null)
            {
                if (mode != "set") throw new AdminError("offline players: only 'set' is supported");
                return OfflineWrite(a, "money", new Dictionary<string, string> { { "Money", Math.Min(amount, (ulong)MoneyCap).ToString(CultureInfo.InvariantCulture) } });
            }
            ulong old = (ulong)Math.Max(0, c.Player.Money);
            ulong nv = Apply(mode, old, amount, (ulong)MoneyCap);
            SetMoney(c, (long)nv);// the setter sends the update packet
            SafeSave(c);
            PLog("money", Who(c), mode + " " + amount + ": " + old + " -> " + c.Player.Money, true);
            return Ok(Who(c) + ": money " + old.ToString("N0", CultureInfo.InvariantCulture) + " -> " + c.Player.Money.ToString("N0", CultureInfo.InvariantCulture));
        }

        static JObject CpsOp(A a, bool bound)
        {
            ulong amount = a.ULong("amount");
            string mode = a.Str("mode", "set");
            string op = bound ? "boundcps" : "cps";
            var c = FindOnline(a);
            if (c == null)
            {
                if (mode != "set") throw new AdminError("offline players: only 'set' is supported");
                return OfflineWrite(a, op, new Dictionary<string, string> { { bound ? "BoundConquerPoints" : "ConquerPoints", Math.Min(amount, (ulong)(bound ? BoundCpsCap : CpsCap)).ToString(CultureInfo.InvariantCulture) } });
            }
            if (bound)
            {
                ulong old = (ulong)Math.Max(0, c.Player.BoundConquerPoints);
                ulong nv = Apply(mode, old, amount, (ulong)BoundCpsCap);
                SetBoundCps(c, (long)nv);
                SafeSave(c);
                PLog(op, Who(c), mode + " " + amount + ": " + old + " -> " + nv, true);
                return Ok(Who(c) + ": bound CPs " + old.ToString("N0", CultureInfo.InvariantCulture) + " -> " + nv.ToString("N0", CultureInfo.InvariantCulture));
            }
            else
            {
                ulong old = c.Player.ConquerPoints;
                ulong nv = Apply(mode, old, amount, (ulong)CpsCap);
                SetCps(c, (long)nv);
                SafeSave(c);
                PLog(op, Who(c), mode + " " + amount + ": " + old + " -> " + nv, true);
                return Ok(Who(c) + ": CPs " + old.ToString("N0", CultureInfo.InvariantCulture) + " -> " + nv.ToString("N0", CultureInfo.InvariantCulture));
            }
        }

        static JObject VipOp(A a)
        {
            var c = NeedOnline(a);
            int level = (int)a.Need("level", 0, 7);
            int days = (int)a.Long("days", 30);
            if (level > 0 && (days < 1 || days > 3650)) throw new AdminError("'days' must be 1-3650");
            var p = c.Player;
            if (level == 0)
            {
                p.VipLevel = 0;
                p.ExpireVip = DateTime.Now;
            }
            else
            {
                // as @vip: extend from now, or from the current expiry while it is still active
                p.ExpireVip = DateTime.Now > p.ExpireVip ? DateTime.Now.AddDays(days) : p.ExpireVip.AddDays(days);
                p.VipLevel = (byte)level;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                p.SendUpdate(stream, p.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                p.UpdateVip(stream);
            }
            if (level > 0)
                c.SendSysMesage("You received VIP " + level + " (" + days + " days).", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            SafeSave(c);
            string msg = level == 0 ? "VIP removed" : "VIP " + level + " until " + Dt(p.ExpireVip);
            PLog("vip", Who(c), msg, true);
            return Ok(Who(c) + ": " + msg);
        }

        static JObject RebornOp(A a)
        {
            var c = NeedOnline(a);
            int count = (int)a.Need("count", 0, 2);
            int old = c.Player.Reborn;
            c.Player.Reborn = (byte)count;// @reborn
            using (var rec = new ServerSockets.RecycledPacket())
                c.Player.SendUpdate(rec.GetStream(), c.Player.Reborn, Game.MsgServer.MsgUpdate.DataType.Reborn);
            SafeSave(c);
            PLog("reborn", Who(c), "reborn " + old + " -> " + count, true);
            return Ok(Who(c) + ": reborn " + old + " -> " + count + " (the player should relog to see everything updated)");
        }

        // ------------------------------------------------------------------ items
        sealed class ItemSpec
        {
            public uint Id;
            public int Amount = 1;
            public byte Plus, Bless, Enchant;
            public Role.Flags.Gem S1 = Role.Flags.Gem.NoSocket, S2 = Role.Flags.Gem.NoSocket;
            public bool Bound;
            public Database.ItemType.DBItem Db;
            public bool Stackable { get { return Db.StackSize > 1; } }
            public int Slots { get { return Stackable ? (Amount + Db.StackSize - 1) / Db.StackSize : Amount; } }
            public string Describe()
            {
                return Amount + " x " + ItemName(Id) + " [" + Id + "]" + (Plus > 0 ? " +" + Plus : "") + (Bless > 0 ? " -" + Bless : "") +
                       (Enchant > 0 ? " e" + Enchant : "") + (S1 != Role.Flags.Gem.NoSocket ? " s1=" + (byte)S1 : "") +
                       (S2 != Role.Flags.Gem.NoSocket ? " s2=" + (byte)S2 : "") + (Bound ? " (bound)" : "");
            }
        }

        static Role.Flags.Gem Gem(long v, string key)
        {
            if (v < 0 || v > 255 || !Enum.IsDefined(typeof(Role.Flags.Gem), (byte)v)) throw new AdminError("'" + key + "' is not a valid gem/socket value");
            return (Role.Flags.Gem)(byte)v;
        }

        static ItemSpec ParseItem(A a)
        {
            var s = new ItemSpec();
            s.Id = (uint)a.Need("id", 1, uint.MaxValue);
            if (Database.Server.ItemsBase == null || !Database.Server.ItemsBase.TryGetValue(s.Id, out s.Db) || s.Db == null)
                throw new AdminError("item id " + s.Id + " does not exist in the server's item table");
            long plus = a.Long("plus", 0), bless = a.Long("bless", 0);
            if (plus < 0 || plus > 12) throw new AdminError("'plus' must be 0-12");
            if (bless < 0 || bless > 7) throw new AdminError("'bless' must be 0-7");
            s.Plus = (byte)plus;
            s.Bless = (byte)bless;
            s.Enchant = (byte)Math.Max(0, Math.Min(255, a.Long("enchant", 0)));
            s.S1 = Gem(a.Long("socket1", 0), "socket1");
            s.S2 = Gem(a.Long("socket2", 0), "socket2");
            s.Bound = a.Bool("bound");
            int maxAmount = s.Stackable ? Math.Min(ushort.MaxValue, 40 * s.Db.StackSize) : 40;
            long amount = a.Long("amount", 1);
            if (amount < 1 || amount > maxAmount) throw new AdminError("'amount' must be 1-" + maxAmount + " for this item");
            s.Amount = (int)amount;
            if (s.Stackable && (s.Plus > 0 || s.Bless > 0 || s.Enchant > 0 || s.S1 != Role.Flags.Gem.NoSocket || s.S2 != Role.Flags.Gem.NoSocket))
                throw new AdminError("stackable items cannot have plus/bless/enchant/sockets");
            return s;
        }

        /// <summary>Gives one item spec (as @item). Space must have been checked by the caller.</summary>
        static bool GiveSpec(Client.GameClient c, ItemSpec s, ServerSockets.Packet stream)
        {
            if (s.Stackable)
                return c.Inventory.AddItemWitchStack(s.Id, 0, (ushort)s.Amount, stream, s.Bound);
            return c.Inventory.Add(stream, s.Id, (byte)s.Amount, s.Plus, s.Bless, s.Enchant, s.S1, s.S2, s.Bound);
        }

        static string GiveItemTo(Client.GameClient c, ItemSpec s, bool notify)
        {
            if (!c.Inventory.HaveSpace((byte)Math.Min(255, s.Slots)))
                throw new AdminError(Who(c) + ": not enough inventory space (" + c.Inventory.ClientItems.Count + "/40, needs " + s.Slots + ")");
            bool ok;
            using (var rec = new ServerSockets.RecycledPacket())
                ok = GiveSpec(c, s, rec.GetStream());
            if (!ok) throw new AdminError(Who(c) + ": the game refused to add the item");
            if (notify)
                c.SendSysMesage("You received " + s.Amount + " " + ItemName(s.Id) + (s.Bound ? " (bound)" : "") + " from the staff.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            return s.Describe();
        }

        static JObject GiveItem(A a)
        {
            var c = NeedOnline(a);
            var s = ParseItem(a);
            string what = GiveItemTo(c, s, a.Bool("notify", true));
            SafeSave(c);
            PLog("giveitem", Who(c), what, true);
            return Ok(Who(c) + ": gave " + what);
        }

        static JObject RemoveItem(A a)
        {
            var c = NeedOnline(a);
            uint itemUid = (uint)a.Need("itemUid", 1, uint.MaxValue);
            Game.MsgServer.MsgGameItem it;
            if (!c.Inventory.TryGetItem(itemUid, out it)) throw new AdminError("item uid " + itemUid + " is not in the inventory (equipped items cannot be removed here)");
            string what = ItemName(it.ITEM_ID) + " [" + it.ITEM_ID + "] uid " + it.UID + (it.StackSize > 1 ? " x" + it.StackSize : "") + (it.Plus > 0 ? " +" + it.Plus : "");
            using (var rec = new ServerSockets.RecycledPacket())
                c.Inventory.Update(it, Role.Instance.AddMode.REMOVE, rec.GetStream(), true);// whole stack
            SafeSave(c);
            PLog("removeitem", Who(c), what, true);
            return Ok(Who(c) + ": removed " + what);
        }

        static JObject ClearInventory(A a)
        {
            var c = NeedOnline(a);
            int n = 0;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var it in c.Inventory.ClientItems.Values.ToArray())
                    if (c.Inventory.Update(it, Role.Instance.AddMode.REMOVE, stream, true)) n++;
            }
            SafeSave(c);
            PLog("clearinventory", Who(c), n + " items removed", true);
            return Ok(Who(c) + ": inventory cleared (" + n + " items)");
        }

        // ------------------------------------------------------------------ packages (ServerDataBase\packages.json)
        sealed class PackageDef
        {
            public string Name, Description;
            public ulong Money, Cps, BoundCps;
            public List<ItemSpec> Items = new List<ItemSpec>();
        }

        static List<PackageDef> LoadPackages(bool strict, List<string> problems)
        {
            string f = Path.Combine(Program.ServerConfig.DbLocation ?? "", "packages.json");
            var result = new List<PackageDef>();
            if (!File.Exists(f)) return result;
            JObject root;
            try
            {
                root = JObject.Parse(File.ReadAllText(f, Encoding.UTF8), new JsonLoadSettings { CommentHandling = CommentHandling.Ignore });
            }
            catch (Exception e) { throw new AdminError("packages.json: " + e.Message); }
            var list = root["packages"] as JArray;
            if (list == null) return result;
            foreach (var po in list)
            {
                var pd = po as JObject;
                if (pd == null) continue;
                var pa = new A(pd);
                var p = new PackageDef { Name = pa.Str("name", ""), Description = pa.Str("description", "") };
                if (p.Name.Length == 0) continue;
                try
                {
                    p.Money = pa.ULong("money");
                    p.Cps = pa.ULong("cps");
                    p.BoundCps = pa.ULong("boundCps");
                    var items = pa.List("items");
                    if (items != null)
                        foreach (var io in items)
                        {
                            var id = io as JObject;
                            if (id == null) continue;
                            p.Items.Add(ParseItem(new A(id)));
                        }
                }
                catch (AdminError e)
                {
                    if (strict) throw new AdminError("package '" + p.Name + "': " + e.Message);
                    if (problems != null) problems.Add(p.Name + ": " + e.Message);
                    continue;
                }
                result.Add(p);
            }
            return result;
        }

        static JObject PackagesOp()
        {
            var problems = new List<string>();
            var list = new JArray();
            foreach (var p in LoadPackages(false, problems))
                list.Add(new JObject
                {
                    ["name"] = p.Name,
                    ["description"] = p.Description,
                    ["money"] = p.Money,
                    ["cps"] = p.Cps,
                    ["boundCps"] = p.BoundCps,
                    ["slots"] = p.Items.Sum(i => i.Slots),
                    ["items"] = new JArray(p.Items.Select(i => i.Describe())),
                });
            var r = Ok(null);
            r["packages"] = list;
            r["problems"] = new JArray(problems);
            return r;
        }

        static string GivePackageTo(Client.GameClient c, PackageDef pk)
        {
            int slots = pk.Items.Sum(i => i.Slots);
            if (slots > 0 && !c.Inventory.HaveSpace((byte)Math.Min(255, slots)))
                throw new AdminError(Who(c) + ": not enough inventory space for '" + pk.Name + "' (" + c.Inventory.ClientItems.Count + "/40, needs " + slots + ")");
            var given = new List<string>();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var s in pk.Items)
                    given.Add((GiveSpec(c, s, stream) ? "" : "FAILED ") + s.Describe());
            }
            if (pk.Money > 0)
            {
                SetMoney(c, (long)Math.Min((ulong)MoneyCap, (ulong)Math.Max(0, c.Player.Money) + Math.Min(pk.Money, (ulong)MoneyCap)));
                given.Add(pk.Money + " money");
            }
            if (pk.Cps > 0)
            {
                SetCps(c, (long)Math.Min((ulong)CpsCap, (ulong)c.Player.ConquerPoints + Math.Min(pk.Cps, (ulong)CpsCap)));
                given.Add(pk.Cps + " CPs");
            }
            if (pk.BoundCps > 0)
            {
                SetBoundCps(c, (long)Math.Min((ulong)BoundCpsCap, (ulong)Math.Max(0, c.Player.BoundConquerPoints) + Math.Min(pk.BoundCps, (ulong)BoundCpsCap)));
                given.Add(pk.BoundCps + " bound CPs");
            }
            c.SendSysMesage("You received the package \"" + pk.Name + "\" from the staff.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            return "'" + pk.Name + "': " + string.Join("; ", given.ToArray());
        }

        static PackageDef FindPackage(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new AdminError("missing 'package'");
            var pk = LoadPackages(true, null).FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            if (pk == null) throw new AdminError("package '" + name + "' not found in packages.json");
            return pk;
        }

        static JObject GivePackage(A a)
        {
            var c = NeedOnline(a);
            var pk = FindPackage(a.Str("package"));
            string what = GivePackageTo(c, pk);
            SafeSave(c);
            PLog("givepackage", Who(c), what, true);
            return Ok(Who(c) + ": gave package " + what);
        }

        // ------------------------------------------------------------------ teleport
        static JObject TeleportOp(A a)
        {
            uint map = (uint)a.Need("map", 1, uint.MaxValue);
            ushort x = (ushort)a.Need("x", 0, ushort.MaxValue);
            ushort y = (ushort)a.Need("y", 0, ushort.MaxValue);
            if (Database.Server.ServerMaps == null || !Database.Server.ServerMaps.ContainsKey(map)) throw new AdminError("map " + map + " is not loaded on the server");
            bool walkable = true;
            try { walkable = Database.Server.ServerMaps[map].ValidLocation(x, y); } catch { }
            if (!walkable && !a.Bool("force"))
                throw new AdminError("(" + x + "," + y + ") is not walkable on map " + map + " - pick another spot or use force");
            var c = FindOnline(a);
            if (c == null)
                return OfflineWrite(a, "teleport", new Dictionary<string, string> { { "Map", map.ToString() }, { "X", x.ToString() }, { "Y", y.ToString() } });
            string from = c.Player.Map + " (" + c.Player.X + "," + c.Player.Y + ")";
            c.Teleport(x, y, map);// @tele
            bool moved = c.Player.Map == map;
            PLog("teleport", Who(c), from + " -> " + map + " (" + x + "," + y + ")" + (moved ? "" : " [the game blocked it]"), moved);
            if (!moved) throw new AdminError(Who(c) + ": the game blocked the teleport (jail/event map or cross-server)");
            return Ok(Who(c) + ": teleported to map " + map + " (" + x + "," + y + ")");
        }

        static JObject Bring(A a)
        {
            var c = NeedOnline(a);
            uint toUid = (uint)a.Need("toUid", 1, uint.MaxValue);
            Client.GameClient t;
            if (!Database.Server.GamePoll.TryGetValue(toUid, out t) || t == null || t.Player == null || !t.FullLoading) throw new AdminError("target player is not online");
            if (t == c) throw new AdminError("choose a different target player");
            c.Teleport(t.Player.X, t.Player.Y, t.Player.Map, t.Player.DynamicID);
            PLog("bring", Who(c), "-> " + Who(t) + " at " + t.Player.Map + " (" + t.Player.X + "," + t.Player.Y + ")", true);
            return Ok(Who(c) + " moved to " + Who(t));
        }

        // ------------------------------------------------------------------ life / discipline
        static JObject Revive(A a)
        {
            var c = NeedOnline(a);
            bool wasDead = !c.Player.Alive;
            if (wasDead)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    c.Player.Revive(rec.GetStream());// @revive
            }
            else
            {
                c.Player.HitPoints = (int)c.Status.MaxHitpoints;
                c.Player.Mana = (ushort)Math.Min(ushort.MaxValue, c.Status.MaxMana);
            }
            PLog("revive", Who(c), wasDead ? "revived" : "healed", true);
            return Ok(Who(c) + (wasDead ? ": revived" : ": HP/MP restored"));
        }

        static JObject Kick(A a)
        {
            var c = NeedOnline(a);
            string why = a.Str("reason", "").Trim();
            if (why.Length > 0)
                c.SendSysMesage("You were disconnected by the staff: " + why, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
            c.Socket.Disconnect();// @kick
            PLog("kick", Who(c), why, true);
            return Ok(Who(c) + " kicked");
        }

        // BanUID.txt is '/'-separated: keep the separator out of names and reasons
        static string BanText(string s) { return (s ?? "").Replace('/', '|').Replace("\r", " ").Replace("\n", " ").Trim(); }

        static void SaveBans()
        {
            try { Database.SystemBannedAccount.Save(); } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        static JObject Ban(A a)
        {
            long hours = a.Need("hours", 1, 876000);
            string reason = BanText(a.Str("reason", ""));
            if (reason.Length > 120) reason = reason.Substring(0, 120);
            var c = FindOnline(a);
            uint uid;
            string name;
            if (c != null) { uid = c.Player.UID; name = c.Player.Name; }
            else
            {
                uid = ResolveOfflineUid(a);
                var off = ReadOfflineIni(uid);
                name = off != null ? (string)off["name"] : "?";
            }
            Database.SystemBannedAccount.RemoveBan(uid);
            Database.SystemBannedAccount.AddBan(uid, BanText(name), (uint)hours, reason);// @ban
            SaveBans();
            if (c != null)
            {
                c.SendSysMesage("Your account was banned by the staff.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.white);
                c.Socket.Disconnect();
            }
            PLog("ban", name + " (" + uid + ")", hours + " hours, reason: " + reason, true);
            return Ok(name + " (" + uid + ") banned for " + hours + " hours" + (c != null ? " and kicked" : ""));
        }

        static JObject Unban(A a)
        {
            uint uid = a.Has("uid") ? (uint)a.Need("uid", 1, uint.MaxValue) : ResolveOfflineUid(a);
            if (!Database.SystemBannedAccount.RemoveBan(uid)) throw new AdminError("uid " + uid + " is not banned");// @unbanuid
            SaveBans();
            PLog("unban", "uid=" + uid, "ban removed", true);
            return Ok("uid " + uid + " unbanned");
        }

        static JObject Bans()
        {
            var list = new JArray();
            var all = new List<Database.SystemBannedAccount.Client>();
            foreach (var b in Database.SystemBannedAccount.BannedPoll.Values) all.Add(b);
            foreach (var b in all)
            {
                if (b == null) continue;
                DateTime start;
                try { start = DateTime.FromBinary(b.StartBan); } catch { start = DateTime.MinValue; }
                list.Add(new JObject
                {
                    ["uid"] = b.UID,
                    ["name"] = b.Name ?? "",
                    ["hours"] = b.Hours,
                    ["since"] = Dt(start),
                    ["until"] = Dt(start.AddHours(b.Hours)),
                    ["reason"] = b.Reason ?? "",
                });
            }
            var r = Ok(null);
            r["bans"] = list;
            return r;
        }

        static JObject ChatBan(A a, bool ban)
        {
            var c = NeedOnline(a);
            var p = c.Player;
            if (ban)
            {
                long minutes = a.Long("minutes", 60);
                if (minutes <= 0)
                    p.PermenantBannedChat = true;
                else
                    p.BannedChatStamp = DateTime.Now.AddMinutes(Math.Min(minutes, 5256000));
                p.IsBannedChat = true;
            }
            else
            {
                p.BannedChatStamp = DateTime.Now;
                p.IsBannedChat = false;
                p.PermenantBannedChat = false;
            }
            SafeSave(c);
            string what = ban ? (p.PermenantBannedChat ? "chat banned permanently" : "chat banned until " + Dt(p.BannedChatStamp)) : "chat ban removed";
            PLog(ban ? "chatban" : "unchatban", Who(c), what, true);
            return Ok(Who(c) + ": " + what);
        }

        static JObject Freeze(A a, bool freeze)
        {
            var c = NeedOnline(a);
            if (freeze)
            {
                int seconds = (int)Math.Max(10, Math.Min(86400 * 7, a.Long("seconds", 86400)));
                c.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.Freeze, seconds, true);
                PLog("freeze", Who(c), seconds + " s", true);
                return Ok(Who(c) + " frozen for " + seconds + " s");
            }
            c.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Freeze);
            PLog("unfreeze", Who(c), "", true);
            return Ok(Who(c) + " unfrozen");
        }

        // ------------------------------------------------------------------ messages
        static string CleanText(A a)
        {
            string text = (a.Str("text") ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
            if (text.Length == 0) throw new AdminError("missing 'text'");
            if (text.Length > 250) text = text.Substring(0, 250);
            return text;
        }

        static JObject MessageOp(A a)
        {
            var c = NeedOnline(a);
            string text = CleanText(a);
            if (a.Bool("popup"))
                c.CreateBoxDialog(text);
            else
                c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
            PLog("message", Who(c), text, true);
            return Ok("message sent to " + Who(c));
        }

        static Game.MsgServer.MsgMessage.ChatMode BroadcastMode(string mode)
        {
            switch ((mode ?? "center").ToLowerInvariant())
            {
                case "center": return Game.MsgServer.MsgMessage.ChatMode.Center;
                case "broadcast": return Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage;
                case "system": return Game.MsgServer.MsgMessage.ChatMode.TopLeftSystem;
                case "talk": return Game.MsgServer.MsgMessage.ChatMode.System;
                default: throw new AdminError("'mode' must be center, broadcast, system or talk");
            }
        }

        static JObject Broadcast(A a)
        {
            string text = CleanText(a);
            string mode = a.Str("mode", "center");
            var cm = BroadcastMode(mode);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(text, "ALLUSERS", Game.MsgServer.MsgMessage.MsgColor.red, cm).GetArray(stream));
            }
            PLog("broadcast", "all", mode + ": " + text, true);
            return Ok("broadcast sent (" + mode + ")");
        }

        static JObject MassMessage(A a)
        {
            string text = CleanText(a);
            bool popup = a.Bool("popup");
            int n = 0;
            foreach (var c in OnlineClients())
            {
                if (c.Fake) continue;
                try
                {
                    if (popup) c.CreateBoxDialog(text);
                    else c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                    n++;
                }
                catch { }
            }
            PLog("massmessage", "all", n + " players: " + text, true);
            return Ok("message sent to " + n + " players");
        }

        // ------------------------------------------------------------------ skills
        static JObject LearnSkill(A a)
        {
            var c = NeedOnline(a);
            ushort id = (ushort)a.Need("skill", 1, ushort.MaxValue);
            ushort level = (ushort)a.Need("level", 0, 20);
            if (Database.Server.Magic == null || !Database.Server.Magic.ContainsKey(id)) throw new AdminError("skill " + id + " does not exist in MagicType");
            if (id == 1105 || (id >= 4000 && id <= 4070)) throw new AdminError("skill " + id + " cannot be learned this way (the game refuses it)");
            using (var rec = new ServerSockets.RecycledPacket())
                c.MySpells.Add(rec.GetStream(), id, level, 0, 0, 0);// @spell
            SafeSave(c);
            PLog("learnskill", Who(c), SpellName(id) + " [" + id + "] level " + level, true);
            return Ok(Who(c) + ": learned " + SpellName(id) + " level " + level);
        }

        static JObject RemoveSkill(A a)
        {
            var c = NeedOnline(a);
            ushort id = (ushort)a.Need("skill", 1, ushort.MaxValue);
            if (!c.MySpells.ClientSpells.ContainsKey(id)) throw new AdminError("the player does not have skill " + id);
            using (var rec = new ServerSockets.RecycledPacket())
                c.MySpells.Remove(id, rec.GetStream());// @repell
            SafeSave(c);
            PLog("removeskill", Who(c), SpellName(id) + " [" + id + "]", true);
            return Ok(Who(c) + ": removed " + SpellName(id));
        }

        static JObject SaveOp(A a)
        {
            if (a.Bool("all"))
            {
                int n = 0;
                foreach (var c in OnlineClients()) { if (!c.Fake) { SafeSave(c); n++; } }
                PLog("save", "all", n + " players saved", true);
                return Ok(n + " players saved");
            }
            var one = NeedOnline(a);
            SafeSave(one);
            PLog("save", Who(one), "", true);
            return Ok(Who(one) + " saved");
        }

        // ------------------------------------------------------------------ mass
        static JObject MassGive(A a)
        {
            ItemSpec item = null;
            PackageDef pk = null;
            if (a.Has("package")) pk = FindPackage(a.Str("package"));
            else item = ParseItem(a);
            int minLevel = (int)a.Long("minLevel", 0);
            int okCount = 0;
            var failed = new JArray();
            foreach (var c in OnlineClients())
            {
                if (c.Fake || c.Player.Level < minLevel) continue;
                try
                {
                    if (pk != null) GivePackageTo(c, pk);
                    else GiveItemTo(c, item, true);
                    SafeSave(c);
                    okCount++;
                }
                catch (AdminError e) { failed.Add(e.Message); }
                catch (Exception e) { failed.Add(Who(c) + ": " + e.Message); }
            }
            string what = pk != null ? "package '" + pk.Name + "'" : item.Describe();
            PLog("massgive", "all (minLevel " + minLevel + ")", what + " -> " + okCount + " ok, " + failed.Count + " failed", true);
            var r = Ok("gave " + what + " to " + okCount + " players" + (failed.Count > 0 ? ", " + failed.Count + " failed" : ""));
            r["failed"] = failed;
            return r;
        }
    }
}
