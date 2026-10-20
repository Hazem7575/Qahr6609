// MonsterDrops module: extra monster drops read from <DbLocation>\Modules\MonsterDrops\drops.txt, so a drop can be
// added or changed without a rebuild (@drops reload). The hard-coded drops in MonsterRole\Death.cs are not touched:
// most of that file is quest/map scripting, and its simple drops sit inside else-if chains whose order matters.
// A rule here is always an independent extra roll, made when the monster dies (ModuleManager.MonsterKilled).
// Switch off: Qahr6609.Tools\Server\ServerCtl\module_off.bat MonsterDrops + rebuild.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.MonsterDrops
{
    public sealed class MonsterDropsModule : ModuleBase
    {
        public override string Name { get { return "MonsterDrops"; } }

        sealed class Rule
        {
            public uint Monster;// 0 = any
            public uint Map;// 0 = any
            public int ChanceValue, ChanceOf;// Role.Core.Rate(ChanceValue, ChanceOf)
            public uint Item;
            public byte Count;
            public bool ToBag;
            public string Message;
            public int Line;
        }

        static volatile Dictionary<uint, Rule[]> byMonster = new Dictionary<uint, Rule[]>();
        static volatile Rule[] anyMonster = new Rule[0];
        static string file;
        static MonsterDropsModule instance;

        public override void OnLoad()
        {
            instance = this;
            file = DataPath + "drops.txt";
            Log(Load());
        }

        public override void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            if (killer == null || monster == null || monster.Family == null)
                return;
            Rule[] rules;
            if (byMonster.TryGetValue(monster.Family.ID, out rules))
                Apply(killer, monster, rules);
            var any = anyMonster;
            if (any.Length > 0)
                Apply(killer, monster, any);
        }

        static void Apply(Client.GameClient killer, Game.MsgMonster.MonsterRole monster, Rule[] rules)
        {
            foreach (var r in rules)
            {
                if (r.Map != 0 && r.Map != monster.Map)
                    continue;
                if (!Role.Core.Rate(r.ChanceValue, r.ChanceOf))
                    continue;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (r.ToBag && killer.Inventory.HaveSpace(r.Count))
                        killer.Inventory.Add(stream, r.Item, r.Count);
                    else
                        for (int i = 0; i < r.Count; i++)
                            monster.DropItemID(killer, r.Item, stream);
                }
                if (r.Message != null)
                    killer.SendSysMesage(r.Message.Replace("{name}", killer.Player.Name).Replace("{monster}", monster.Name));
            }
        }

        /// <summary>Reads drops.txt (creates it with examples when missing). Returns a one-line summary.</summary>
        static string Load()
        {
            try
            {
                if (!File.Exists(file))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                    File.WriteAllText(file, Template);
                }
                var rules = new List<Rule>();
                var problems = new List<string>();
                string[] lines = File.ReadAllLines(file);
                for (int n = 0; n < lines.Length; n++)
                {
                    string line = lines[n].Trim();
                    if (line.Length == 0 || line[0] == '#' || line.StartsWith("//"))
                        continue;
                    string[] f = line.Split(new[] { ' ', '\t' }, 7, StringSplitOptions.RemoveEmptyEntries);
                    Rule r;
                    string error = Parse(f, out r);
                    if (error != null)
                    {
                        problems.Add("line " + (n + 1) + ": " + error);
                        continue;
                    }
                    r.Line = n + 1;
                    rules.Add(r);
                }
                var map = new Dictionary<uint, List<Rule>>();
                var any = new List<Rule>();
                foreach (var r in rules)
                {
                    if (r.Monster == 0)
                    {
                        any.Add(r);
                        continue;
                    }
                    List<Rule> list;
                    if (!map.TryGetValue(r.Monster, out list))
                        map[r.Monster] = list = new List<Rule>();
                    list.Add(r);
                }
                var frozen = new Dictionary<uint, Rule[]>();
                foreach (var kv in map)
                    frozen[kv.Key] = kv.Value.ToArray();
                byMonster = frozen;
                anyMonster = any.ToArray();
                foreach (var p in problems)
                    instance.Log("drops.txt " + p);
                return rules.Count + " drop rule(s) loaded from drops.txt" + (problems.Count > 0 ? ", " + problems.Count + " line(s) skipped (see log)" : "");
            }
            catch (Exception e)
            {
                return "drops.txt not loaded: " + e.Message;
            }
        }

        static string Parse(string[] f, out Rule r)
        {
            r = null;
            if (f.Length < 6)
                return "needs at least 6 columns: Monster Map Chance Item Count Where [Message]";
            var rule = new Rule();
            if (f[0] != "*" && !uint.TryParse(f[0], out rule.Monster))
                return "bad monster id '" + f[0] + "'";
            if (f[1] != "*" && !uint.TryParse(f[1], out rule.Map))
                return "bad map id '" + f[1] + "'";
            if (!ParseChance(f[2], out rule.ChanceValue, out rule.ChanceOf))
                return "bad chance '" + f[2] + "' (use 5% or 1/1000)";
            if (!uint.TryParse(f[3], out rule.Item) || !Database.Server.ItemsBase.ContainsKey(rule.Item))
                return "item '" + f[3] + "' is not in itemtype";
            if (!byte.TryParse(f[4], out rule.Count) || rule.Count == 0 || rule.Count > 40)
                return "count must be 1..40";
            string where = f[5].ToLowerInvariant();
            if (where != "floor" && where != "bag")
                return "where must be floor or bag";
            rule.ToBag = where == "bag";
            if (f.Length > 6)
                rule.Message = f[6].Trim();
            r = rule;
            return null;
        }

        static bool ParseChance(string s, out int value, out int of)
        {
            value = of = 0;
            if (s.EndsWith("%"))
            {
                of = 100;
                return int.TryParse(s.Substring(0, s.Length - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value > 0 && value <= 100;
            }
            int slash = s.IndexOf('/');
            return slash > 0
                && int.TryParse(s.Substring(0, slash), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                && int.TryParse(s.Substring(slash + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out of)
                && value > 0 && of > 0 && value <= of;
        }

        [GmCommand("drops", Role = CommandRole.GM, Help = "extra monster drops from Modules\\MonsterDrops\\drops.txt: @drops reload | @drops (count)", Usage = "@drops reload")]
        static void DropsCommand(Client.GameClient client, string[] args)
        {
            if (args.Length > 1 && args[1].Equals("reload", StringComparison.OrdinalIgnoreCase))
            {
                string info = Load();
                instance.Log(info + " (reload by " + client.Player.Name + ")");
                client.SendSysMesage(info);
                return;
            }
            int count = anyMonster.Length;
            foreach (var kv in byMonster)
                count += kv.Value.Length;
            client.SendSysMesage(count + " extra drop rule(s) active. @drops reload re-reads drops.txt.");
        }

        const string Template =
@"# Extra monster drops (module MonsterDrops). Edit, then type @drops reload in game (no rebuild).
# Every rule is an extra, independent roll when the monster dies; the drops written in Death.cs still happen.
#
# Columns (spaces or tabs):  Monster  Map  Chance  Item  Count  Where  [Message]
#   Monster  monster type id (Monsters\*.ini [cq_monstertype] id), or * for any monster
#   Map      map id where the monster dies, or * for any map
#   Chance   5%  (5 in 100)   or   1/1000  (1 in 1000)
#   Item     item type id (must exist in itemtype.txt)
#   Count    1..40
#   Where    floor (dropped on the ground, owned by the killer) or bag (inventory; floor when the bag is full)
#   Message  optional system message to the killer; {name} = killer, {monster} = monster name
#
# Examples (remove the # to use):
# 15      *     1/100   1088001  1  floor
# *       1015  1/5000  1088000  1  bag    {name}, the {monster} dropped a DragonBall into your bag!
";
    }
}
