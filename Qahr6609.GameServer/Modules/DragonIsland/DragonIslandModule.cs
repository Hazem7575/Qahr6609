// Dragon Island module (map 10137, official update 6532). The whole island is this folder.
//
//   DragonIslandModule.cs   this file: start / stop, tick, kills, NPC talk, @dragonisland
//   code\IslandConfig.cs    island.ini: entry rules, Iron Tong, the beasts (where, which minutes), Commander Shan's rewards
//   code\IslandBeasts.cs    beasts appear at their minutes (not doubled), announce + "go there?" box, damage ranking, reward mails
//   code\IslandNpcs.cs      Iron Tong (Twin City -> island) and Cobb Wind (next beast, beast table, ride to a living beast)
//   data\island.ini         the settings (copied to ServerDataBase\Modules\DragonIsland\ the first time; edit that copy)
//
// While loaded, the module runs the island's bosses: the fixed schedule of World\Tournaments\BossHandler.cs and the
// core boss rank rewards (Death.cs) leave map 10137 alone (BossHandler.ModuleOwnedMaps). Remove the folder: back to them.
// GoldenTree's festival gives Anniversary Stars for kills on this map (its own festival.ini).
// GM: @dragonisland [status] | spawn <beast number> | reload
using System;
using System.IO;
using System.Linq;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.DragonIsland
{
    public sealed class DragonIslandModule : ModuleBase
    {
        public override string Name { get { return "DragonIsland"; } }

        static DragonIslandModule instance;
        IslandConfig config;

        public override void OnLoad()
        {
            instance = this;
            LoadConfig();
        }

        void LoadConfig()
        {
            string file = Path.Combine(DataPath, "island.ini");
            if (!File.Exists(file))
            {
                string def = Path.Combine(RuntimeModules.FolderOf(Name), "data", "island.ini");
                if (File.Exists(def)) File.Copy(def, file);
            }
            if (!File.Exists(file))
                throw new FileNotFoundException("island.ini missing", file);
            var c = IslandConfig.Load(file, w => Print(w));
            var old = config;
            if (old != null && old.Map != c.Map) Game.MsgTournaments.BossHandler.ModuleOwnedMaps.TryRemove(old.Map, out _);
            IslandBeasts.DisableRanking();
            config = c;
            Game.MsgTournaments.BossHandler.ModuleOwnedMaps[c.Map] = Name;
            IslandBeasts.EnableRanking(c);
            IslandNpcs.Spawn(c);
            Print(c.Beasts.Count + " beast(s) on map " + c.Map + " (" + c.Beasts.Count(b => b.Official) + " official), entry level " + c.MinLevel + " reborn " + c.MinReborn);
        }

        void Print(string line)
        {
            Log(line);
            MyConsole.WriteLine("[DragonIsland] " + line, ConsoleColor.Cyan);
        }

        public override void OnShutdown()
        {
            var c = config;
            if (c != null) Game.MsgTournaments.BossHandler.ModuleOwnedMaps.TryRemove(c.Map, out _);
            IslandBeasts.DisableRanking();
            IslandNpcs.Remove();
        }

        public override void OnTick(Extensions.Time32 clock)
        {
            var c = config;
            if (c != null) IslandBeasts.Tick(c, Log);
        }

        public override void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            var c = config;
            if (c != null) IslandBeasts.Killed(c, killer, monster, Log);
        }

        public override int OnNpcTalk(Client.GameClient client, ServerSockets.Packet stream, byte option, uint npcId)
        {
            var c = config;
            return c == null ? 0 : IslandNpcs.Talk(c, client, stream, option, npcId);
        }

        [GmCommand("dragonisland", "di", Role = CommandRole.GM, Help = "Dragon Island: status | spawn <beast number> | reload", Usage = "@dragonisland spawn 1")]
        static void Command(Client.GameClient client, string[] data)
        {
            var m = instance;
            if (m == null || m.config == null) return;
            var c = m.config;
            string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "status";
            switch (sub)
            {
                case "reload":
                    try { m.LoadConfig(); client.SendSysMesage("Dragon Island: island.ini reloaded, " + m.config.Beasts.Count + " beast(s)."); }
                    catch (Exception e) { client.SendSysMesage("Dragon Island: " + e.Message); }
                    break;
                case "spawn":
                    {
                        int n;
                        if (data.Length < 3 || !int.TryParse(data[2], out n) || n < 1 || n > c.Beasts.Count)
                        {
                            client.SendSysMesage("@dragonisland spawn <1-" + c.Beasts.Count + ">  (numbers from @dragonisland)");
                            return;
                        }
                        IslandBeasts.Spawn(c, c.Beasts[n - 1], m.Log);
                        client.SendSysMesage(c.Beasts[n - 1].Name + " spawned.");
                        break;
                    }
                default:
                    {
                        DateTime now = DateTime.Now;
                        client.SendSysMesage("Dragon Island (map " + c.Map + "): " + c.Beasts.Count + " beast(s).");
                        for (int i = 0; i < c.Beasts.Count; i++)
                        {
                            var b = c.Beasts[i];
                            DateTime next = b.Next(now);
                            client.SendSysMesage((i + 1) + ". " + b.Name + (b.Official ? " (official)" : "") + " at :" + string.Join("/:", b.Minutes.Select(x => x.ToString("00")))
                                + (IslandBeasts.Alive(c, b) ? "  ALIVE" : next != DateTime.MinValue ? "  next " + next.ToString("HH:mm") : ""));
                        }
                        break;
                    }
            }
        }
    }
}
