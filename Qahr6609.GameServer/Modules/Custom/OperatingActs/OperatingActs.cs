using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.OperatingActs
{
    /// <summary>
    /// Welfare window "Events" tab (client CDlgOperateActivity, ini\OperateActivity.ini, ini\operating_prize.dat).
    /// Layouts read from the 6609 client code (see docs in the module log / COORDINATION):
    ///   1315 C->S/S->C: u16 action @4, u16 0 @6, u16 count @8, u32 0 @10, u32 ids[count] @14
    ///        C->S action 0 = query progress of ids (sent after every list), action 2 = claim (1 id)
    ///        S->C action 1 = mark ids "new", 3/4 = make the client query again
    ///   1316 S->C: u16 action @4, u16 count @6, entries @8 (packet max 1024 bytes)
    ///        action 0 list:     u32 id, u16 status (2 = off, 1 = on), u32 begin, u32 end   (yymmddhhmm)
    ///        action 1 progress: u32 id, u16 claimed tiers, u16 0, u32 points            (always the full set)
    /// The 8-slot prize grid is drawn by the client from its own operating_prize.dat; the server only sends
    /// "claimed" and "points" and hands out the items of ServerDataBase\operating_prize.txt on a claim.
    /// </summary>
    public static class OperatingActs
    {
        public const ushort MsgOperatingAct = 1315, MsgOperatingActInfo = 1316;

        /// <summary>Activities shown (official ids that have prize tiers in the client data).</summary>
        public static readonly uint[] Active = { 273, 274, 275, 276, 277, 278, 279, 280, 281 };
        public const uint CpsCredited = 273, CpsSpentMall = 274, ChiSpent = 275, JiangHuCps = 276,
            Garments = 277, MountArmors = 278, Prestige = 279, LevelReborn = 280, Achievements = 281;
        // activities whose points are counted by the server during the event (the others are computed live)
        static readonly uint[] Counted = { CpsCredited, CpsSpentMall, ChiSpent, JiangHuCps };

        sealed class Tier { public int Index; public uint Need; public uint Item; public uint Amount; }
        static Dictionary<uint, List<Tier>> tiers = new Dictionary<uint, List<Tier>>();

        sealed class State
        {
            public string Window = "";
            public Dictionary<uint, ushort> Claimed = new Dictionary<uint, ushort>();
            public Dictionary<uint, uint> Points = new Dictionary<uint, uint>();
        }
        static readonly ConcurrentDictionary<uint, State> states = new ConcurrentDictionary<uint, State>();
        static string playerDir;
        public static string LoadInfo = "";

        /// <summary>Event window = the current calendar month (counters and claims start again every month).</summary>
        static DateTime WindowBegin { get { var n = DateTime.Now; return new DateTime(n.Year, n.Month, 1); } }
        static DateTime WindowEnd { get { return WindowBegin.AddMonths(1).AddMinutes(-1); } }
        static string WindowKey { get { return WindowBegin.ToString("yyyyMM"); } }

        static uint Stamp(DateTime d)
        {
            return (uint)((d.Year % 100) * 100000000u + d.Month * 1000000u + d.Day * 10000u + d.Hour * 100u + d.Minute);
        }

        public static void Load(string dataPath)
        {
            playerDir = Path.Combine(dataPath, "players");
            try { Directory.CreateDirectory(playerDir); } catch { }
            var t = new Dictionary<uint, List<Tier>>();
            string file = Path.Combine(Program.ServerConfig.DbLocation, "operating_prize.txt");
            if (File.Exists(file))
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    // uid@actType@tier@needPoints@prizeItem@prizeValue@
                    var c = line.Split('@');
                    uint act, tier, need, item, amount;
                    if (c.Length < 6 || !uint.TryParse(c[1], out act) || !uint.TryParse(c[2], out tier) || !uint.TryParse(c[3], out need)
                        || !uint.TryParse(c[4], out item) || !uint.TryParse(c[5], out amount))
                        continue;
                    if (Array.IndexOf(Active, act) < 0)
                        continue;
                    List<Tier> list;
                    if (!t.TryGetValue(act, out list))
                        t[act] = list = new List<Tier>();
                    list.Add(new Tier { Index = (int)tier, Need = need, Item = item, Amount = Math.Max(1, amount) });
                }
            }
            foreach (var l in t.Values)
                l.Sort((a, b) => a.Index.CompareTo(b.Index));
            tiers = t;
            LoadInfo = Active.Length + " activities, " + t.Values.Sum(x => x.Count) + " prize tiers, window " + WindowBegin.ToString("yyyy-MM-dd") + " - " + WindowEnd.ToString("yyyy-MM-dd");
        }

        #region player state
        static State Get(uint uid)
        {
            State s = states.GetOrAdd(uid, ReadState);
            if (s.Window != WindowKey)
            {
                lock (s)
                {
                    if (s.Window != WindowKey)
                    {
                        s.Window = WindowKey;
                        s.Claimed.Clear();
                        s.Points.Clear();
                    }
                }
            }
            return s;
        }

        static State ReadState(uint uid)
        {
            var s = new State();
            try
            {
                string f = Path.Combine(playerDir, uid + ".txt");
                if (File.Exists(f))
                    foreach (var line in File.ReadAllLines(f))
                    {
                        int eq = line.IndexOf('=');
                        if (eq <= 0) continue;
                        string k = line.Substring(0, eq), v = line.Substring(eq + 1);
                        uint id, n;
                        if (k == "window") s.Window = v;
                        else if (k.StartsWith("c") && uint.TryParse(k.Substring(1), out id) && uint.TryParse(v, out n)) s.Claimed[id] = (ushort)Math.Min(n, ushort.MaxValue);
                        else if (k.StartsWith("p") && uint.TryParse(k.Substring(1), out id) && uint.TryParse(v, out n)) s.Points[id] = n;
                    }
            }
            catch (Exception e) { Log("read state " + uid + ": " + e.Message); }
            return s;
        }

        static void SaveState(uint uid, State s)
        {
            try
            {
                var sb = new StringBuilder();
                lock (s)
                {
                    sb.Append("window=").Append(s.Window).AppendLine();
                    foreach (var kv in s.Claimed) sb.Append('c').Append(kv.Key).Append('=').Append(kv.Value).AppendLine();
                    foreach (var kv in s.Points) sb.Append('p').Append(kv.Key).Append('=').Append(kv.Value).AppendLine();
                }
                string f = Path.Combine(playerDir, uid + ".txt");
                File.WriteAllText(f + ".tmp", sb.ToString());
                if (File.Exists(f)) File.Replace(f + ".tmp", f, null);
                else File.Move(f + ".tmp", f);
            }
            catch (Exception e) { Log("save state " + uid + ": " + e.Message); }
        }

        /// <summary>Adds to a counted activity (CPs credited/spent, Chi points used, JiangHu CPs) and refreshes the tab.</summary>
        public static void AddPoints(Client.GameClient client, uint activity, uint amount)
        {
            if (client == null || client.Player == null || amount == 0 || Array.IndexOf(Counted, activity) < 0)
                return;
            var s = Get(client.Player.UID);
            lock (s)
            {
                uint cur;
                s.Points.TryGetValue(activity, out cur);
                s.Points[activity] = (uint)Math.Min((ulong)cur + amount, uint.MaxValue);
            }
            SaveState(client.Player.UID, s);
            SendProgress(client);
        }
        #endregion

        /// <summary>Points of an activity: counted ones from the saved state, the others from the character now.</summary>
        static uint Points(Client.GameClient client, State s, uint id)
        {
            uint v;
            switch (id)
            {
                case Garments:
                    return Count(client, Role.Instance.Wardrobe.ItemsType.Garment);
                case MountArmors:
                    return Count(client, Role.Instance.Wardrobe.ItemsType.Mount);
                case LevelReborn:
                    return (uint)(client.Player.Reborn * 140 + client.Player.Level);
                case Prestige:
                    {
                        var info = Database.PrestigeRanking.GetInfo(client.Player.UID);
                        return info != null ? info.TotalPoints : 0;
                    }
                case Achievements:
                    return AchievementCount(client);
                default:
                    lock (s)
                        return s.Points.TryGetValue(id, out v) ? v : 0;
            }
        }

        static uint Count(Client.GameClient client, Role.Instance.Wardrobe.ItemsType type)
        {
            try
            {
                if (client.MyWardrobe != null && client.MyWardrobe.Items != null)
                    return (uint)client.MyWardrobe.Items[(uint)type].Count;
            }
            catch { }
            return 0;
        }

        static uint AchievementCount(Client.GameClient client)
        {
            try
            {
                if (client.Achievement == null || client.Achievement.Value == null)
                    return 0;
                uint n = 0;
                foreach (uint word in client.Achievement.Value)
                {
                    uint w = word;
                    while (w != 0) { n += w & 1; w >>= 1; }
                }
                return n;
            }
            catch { return 0; }
        }

        #region packets
        public static void SendList(Client.GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                uint begin = Stamp(WindowBegin), end = Stamp(WindowEnd);
                stream.InitWriter();
                stream.Write((ushort)0);
                stream.Write((ushort)Active.Length);
                foreach (var id in Active)
                {
                    stream.Write(id);
                    stream.Write((ushort)1);
                    stream.Write(begin);
                    stream.Write(end);
                }
                stream.Finalize(MsgOperatingActInfo);
                client.Send(stream);
            }
        }

        public static void SendProgress(Client.GameClient client)
        {
            var s = Get(client.Player.UID);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.Write((ushort)1);
                stream.Write((ushort)Active.Length);
                foreach (var id in Active)
                {
                    ushort claimed;
                    lock (s)
                        s.Claimed.TryGetValue(id, out claimed);
                    stream.Write(id);
                    stream.Write(claimed);
                    stream.Write((ushort)0);
                    stream.Write(Points(client, s, id));
                }
                stream.Finalize(MsgOperatingActInfo);
                client.Send(stream);
            }
        }

        [Conquer6609.PacketAttribute(MsgOperatingAct)]
        static unsafe void OnClientPacket(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user == null || user.Player == null || !user.FullLoading)
                return;
            stream.Seek(4);
            ushort action = stream.ReadUInt16();
            stream.Seek(8);
            int count = stream.ReadUInt16();
            if (count > 64)
                count = 64;
            var ids = new uint[count];
            stream.Seek(14);
            for (int i = 0; i < count; i++)
                ids[i] = stream.ReadUInt32();
            switch (action)
            {
                case 0:// query: answer with progress only (a list here would make the client ask again, forever)
                    SendProgress(user);
                    break;
                case 2:// claim the next tier of one activity
                    if (ids.Length == 1)
                        Claim(user, ids[0]);
                    SendProgress(user);
                    break;
            }
        }

        static void Claim(Client.GameClient user, uint id)
        {
            List<Tier> list;
            if (Array.IndexOf(Active, id) < 0 || !tiers.TryGetValue(id, out list))
                return;
            var s = Get(user.Player.UID);
            Tier tier;
            lock (s)
            {
                ushort claimed;
                s.Claimed.TryGetValue(id, out claimed);
                if (claimed >= list.Count)
                    return;
                tier = list[claimed];
                if (Points(user, s, id) < tier.Need)
                    return;
                if (!user.Inventory.HaveSpace((byte)Math.Min(tier.Amount, 40)))
                {
                    user.SendSysMesage("Please make " + Math.Min(tier.Amount, 40) + " more space(s) in your inventory.");
                    return;
                }
                s.Claimed[id] = (ushort)(claimed + 1);
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                user.Inventory.Add(stream, tier.Item, (byte)Math.Min(tier.Amount, 40));
            }
            SaveState(user.Player.UID, s);
            Log(user.Player.Name + " claimed activity " + id + " tier " + tier.Index + " item " + tier.Item + " x" + tier.Amount);
        }
        #endregion

        public static void OnLogin(Client.GameClient client)
        {
            try { SendList(client); }
            catch (Exception e) { Log("login: " + e.Message); }
        }

        public static void OnLogout(Client.GameClient client)
        {
            State s;
            if (client != null && client.Player != null)
                states.TryRemove(client.Player.UID, out s);
        }

        static void Log(string line)
        {
            var m = OperatingActsModule.Instance;
            if (m != null)
                m.Log(line);
        }

        public static void Gm(Client.GameClient client, string[] args)
        {
            string cmd = args.Length > 1 ? args[1].ToLowerInvariant() : "";
            uint id, n;
            if (cmd == "points" && args.Length > 3 && uint.TryParse(args[2], out id) && uint.TryParse(args[3], out n))
            {
                AddPoints(client, id, n);
                client.SendSysMesage("activity " + id + " +" + n + " points (counted activities only: 273-276)");
            }
            else if (cmd == "reset")
            {
                var s = Get(client.Player.UID);
                lock (s) { s.Claimed.Clear(); s.Points.Clear(); }
                SaveState(client.Player.UID, s);
                SendProgress(client);
                client.SendSysMesage("Events tab progress reset for this month.");
            }
            else
            {
                SendList(client);
                client.SendSysMesage(LoadInfo + ". @opact points <273-276> <n> | reset");
            }
        }
    }
}
