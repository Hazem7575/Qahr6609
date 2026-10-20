// Festivals module: the festival engine (dated events made of data) and the festivals written on it. LIVE module (.live):
// the running server compiles this folder and reloads it whenever a .cs file here is saved.
//
//   FestivalsModule.cs        this file: start-up, save, hooks, @festival
//   code\FestivalData.cs      festival.ini format (dates, NPCs, dialogs, exchanges, item rules, drops, spawns, pools)
//   code\FestivalManager.cs   which festivals run, start / end (NPCs, monsters, announcements), dialogs, item use, drops
//   code\FestivalRewards.cs   reward lists: pools, bag space, cost, give (items, EXP minutes, Chi, Study, CPs, silver)
//   code\FestivalStore.cs     per player counters (daily / per festival / ever) in counters.txt, GM overrides in state.ini
//   code\CelestialChest.cs    the Celestial Chest festival's code part (jades, chest growth and class, daily opening)
//   data\<Festival>\          the default festivals, copied to ServerDataBase\Modules\Festivals\<Festival>\ the first time
//                             (edit that copy; "@festival reload" reads it again). A folder "_Name" is switched off.
//   client\README_AR.md       what the client needs for the festivals (reference only)
//
// A new festival = a new folder with a festival.ini (see FestivalData.cs); code only when the data can not say it
// (a FestivalScript class whose Key is the folder name). New monster types / maps need content + a server restart.
// GM: @festival list | status <key> | on <key> | off <key> | auto <key> | reload
//     dev tests: @festival use <item id> (as if you used it) | npc <uid> [option] (as if you clicked the NPC)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.Festivals
{
    public sealed class FestivalsModule : ModuleBase
    {
        public override string Name { get { return "Festivals"; } }

        static FestivalsModule instance;
        int ticks;

        public override void OnLoad()
        {
            instance = this;
            CopyDefaults();
            FestivalStore.Load(DataPath);
            FestivalManager.Load(DataPath, Log);
            FestivalManager.Evaluate(true);
        }

        /// <summary>data\<Festival>\ -> ServerDataBase\Modules\Festivals\<Festival>\ when that folder is not there yet.</summary>
        void CopyDefaults()
        {
            string src = Path.Combine(RuntimeModules.FolderOf(Name), "data");
            if (!Directory.Exists(src)) return;
            foreach (string dir in Directory.GetDirectories(src))
            {
                string dest = Path.Combine(DataPath, Path.GetFileName(dir));
                if (Directory.Exists(dest) || Directory.Exists(Path.Combine(DataPath, "_" + Path.GetFileName(dir)))) continue;
                foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    string to = Path.Combine(dest, file.Substring(dir.Length).TrimStart('\\', '/'));
                    Directory.CreateDirectory(Path.GetDirectoryName(to));
                    File.Copy(file, to);
                }
                Log("default festival copied: " + dest);
            }
        }

        public override void OnSave() { FestivalStore.Save(); }

        public override void OnShutdown()
        {
            FestivalManager.StopAll();
            try { FestivalStore.Save(); } catch (Exception e) { Log("counters not saved: " + e.Message); }
        }

        public override void OnTick(Extensions.Time32 clock)
        {
            if (++ticks % 10 == 0) FestivalManager.Evaluate(false);
        }

        public override void OnLogin(Client.GameClient client) { FestivalManager.OnLogin(client); }
        public override bool OnItemUse(Client.GameClient client, Game.MsgServer.MsgGameItem item) { return FestivalManager.OnItemUse(client, item); }
        public override void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster) { FestivalManager.OnMonsterKilled(killer, monster); }
        public override int OnNpcTalk(Client.GameClient client, ServerSockets.Packet stream, byte option, uint npcId) { return FestivalManager.OnNpcTalk(client, stream, option, npcId); }

        [GmCommand("festival", "fest", Role = CommandRole.GM, Help = "Festivals: list | status <key> | on <key> | off <key> | auto <key> | reload | use <item> | npc <uid> [option]",
            Usage = "@festival on CelestialChest")]
        static void Command(Client.GameClient client, string[] data)
        {
            var m = instance;
            if (m == null) return;
            string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "list";
            string key = data.Length > 2 ? data[2] : null;
            Action<string> say = s => { client.SendSysMesage(s); m.Log("@festival " + sub + ": " + s); };
            FestivalDef f = key != null ? FestivalManager.Find(key) : null;
            switch (sub)
            {
                case "list":
                    foreach (var d in FestivalManager.Defs) say(FestivalManager.Status(d));
                    if (!FestivalManager.Defs.Any()) say("No festival folders in " + m.DataPath);
                    break;
                case "status":
                    if (f == null) { say("@festival status <key>  (keys: @festival list)"); return; }
                    say(FestivalManager.Status(f));
                    foreach (var e in f.Errors) say("error: " + e);
                    foreach (var w in f.Warnings) say("warning: " + w);
                    foreach (var line in FestivalStore.Describe(f, client.Player.UID)) say("you: " + line);
                    break;
                case "on":
                case "off":
                case "auto":
                    if (f == null) { say("@festival " + sub + " <key>  (keys: @festival list)"); return; }
                    FestivalStore.SetOverride(f.Key, sub == "auto" ? null : sub);
                    m.Log(client.Player.Name + " @festival " + sub + " " + f.Key);
                    FestivalManager.Evaluate(false);
                    say(FestivalManager.Status(f));
                    break;
                case "reload":
                    {
                        var was = new HashSet<string>(FestivalManager.StopAll(), StringComparer.OrdinalIgnoreCase);
                        FestivalStore.Save();
                        foreach (var line in FestivalManager.Load(m.DataPath, m.Log).Where(l => l.Contains("error") || l.Contains("INVALID") || l.Contains(": ok")).Take(20)) say(line);
                        FestivalManager.Evaluate(false, was);
                        break;
                    }
                case "use":
                    {
                        uint id;
                        if (!ServerMode.IsDevelopment || key == null || !uint.TryParse(key, out id)) { say("@festival use <item id> (development mode)"); return; }
                        var item = client.Inventory.ClientItems.Values.FirstOrDefault(i => i.ITEM_ID == id);
                        if (item == null) { say("no item " + id + " in your bag"); return; }
                        say("use " + id + ": " + (FestivalManager.OnItemUse(client, item) ? "handled by a festival" : "not a festival item"));
                        break;
                    }
                case "npc":
                    {
                        uint uid;
                        byte opt = 0;
                        if (!ServerMode.IsDevelopment || key == null || !uint.TryParse(key, out uid) || (data.Length > 3 && !byte.TryParse(data[3], out opt))) { say("@festival npc <uid> [option] (development mode)"); return; }
                        using (var rec = new ServerSockets.RecycledPacket())
                            say("npc " + uid + " option " + opt + ": " + (FestivalManager.OnNpcTalk(client, rec.GetStream(), opt, uid) == 1 ? "answered" : "not a running festival NPC"));
                        break;
                    }
                default:
                    say("@festival list | status <key> | on <key> | off <key> | auto <key> | reload | use <item> | npc <uid> [option]");
                    break;
            }
        }
    }
}
