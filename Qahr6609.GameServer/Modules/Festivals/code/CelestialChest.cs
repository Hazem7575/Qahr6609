// Celestial Chest (official 2016-03-17 -> 04-01, level 80+, Questinfo 631 "Celestial Elder, Twin City 322,248"): the code
// part of data\CelestialChest\festival.ini. The rest (Elder, Celestial Mine and its ores, jade combining, the scroll,
// the CP bag, the prize pools) is data.
//   Jade (Earth / Sky / Fairy 1 Growth Point, Divine 3): needs a chest in the bag; adds Growth Points, gives a prize
//     from the jade's pool; enough points raise the chest a class (the item is swapped, ClassUp pool prize).
//   Chest Class 1-8: open once a day for class x 100-min EXP (level 140: class x 50 Chi Points); Class 9: a Divine Jade.
//   Elder "script:getchest": a chest of the player's class when the bag has none; "script:status": growth and class.
// Growth Points are per character and per festival edition (counters.txt "growth").
using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.Festivals
{
    public sealed class CelestialChestScript : FestivalScript
    {
        public override string Key { get { return "CelestialChest"; } }

        uint firstChest = 3008710, divineJade = 3008709;
        int[] needed = { 0, 10, 25, 45, 70, 100, 135, 175, 220 };// growth for class 1..9
        readonly Dictionary<uint, int> jadeGrowth = new Dictionary<uint, int>();
        readonly Dictionary<uint, string> jadePool = new Dictionary<uint, string>();
        int openExp = 100, openChi = 50;

        const string S = "CelestialChest";

        public override void OnLoad(FestivalDef f)
        {
            firstChest = (uint)f.Num(S, "FirstChest", 3008710);
            divineJade = (uint)f.Num(S, "DivineJade", 3008709);
            openExp = (int)f.Num(S, "OpenExpPerClass", 100);
            openChi = (int)f.Num(S, "OpenChiPerClass", 50);
            var n = f.Get(S, "ClassGrowth").Split(',').Select(s => { int v; return int.TryParse(s.Trim(), out v) ? v : -1; }).ToArray();
            if (n.Length != 9 || n.Any(v => v < 0)) throw new Exception("[CelestialChest] ClassGrowth needs 9 numbers");
            needed = n;
            jadeGrowth.Clear();
            jadePool.Clear();
            foreach (string j in f.Ini.All(S, "Jade"))// Jade=<item id>|<growth points>|<pool>
            {
                string[] p = j.Split('|');
                uint id; int g;
                if (p.Length != 3 || !uint.TryParse(p[0], out id) || !int.TryParse(p[1], out g)) throw new Exception("[CelestialChest] Jade=<id>|<growth>|<pool>");
                if (!f.Pools.ContainsKey(p[2].Trim())) throw new Exception("[CelestialChest] no [Pool." + p[2].Trim() + "]");
                jadeGrowth[id] = g;
                jadePool[id] = p[2].Trim();
            }
        }

        int ClassOf(int growth)
        {
            int c = 1;
            for (int i = 0; i < 9; i++) if (growth >= needed[i]) c = i + 1;
            return c;
        }

        MsgGameItem ChestInBag(Client.GameClient c)
        {
            return c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID >= firstChest && i.ITEM_ID < firstChest + 9).OrderByDescending(i => i.ITEM_ID).FirstOrDefault();
        }

        public override bool OnItemUse(FestivalDef f, Client.GameClient c, MsgGameItem item, string name, ServerSockets.Packet stream)
        {
            if (name == "jade") return UseJade(f, c, item, stream);
            if (name == "chest") return OpenChest(f, c, item, stream);
            return false;
        }

        bool UseJade(FestivalDef f, Client.GameClient c, MsgGameItem jade, ServerSockets.Packet stream)
        {
            int g;
            if (!jadeGrowth.TryGetValue(jade.ITEM_ID, out g)) return false;
            var chest = ChestInBag(c);
            if (chest == null) { c.SendSysMesage("You have no Celestial Chest to nourish. The Celestial Elder in Twin City (322,248) will give you one."); return false; }
            var prize = FestivalRewards.Resolve(f, c, new List<Reward> { new Reward { Kind = "pool", Pool = jadePool[jade.ITEM_ID] } }, null);
            var classUp = FestivalRewards.Resolve(f, c, new List<Reward> { new Reward { Kind = "pool", Pool = "ClassUp" } }, null);
            if (!FestivalRewards.HasRoom(c, prize.Concat(classUp).ToList())) { c.SendSysMesage("Please make " + Math.Max(1, FestivalRewards.Slots(prize.Concat(classUp).ToList())) + " more space(s) in your inventory."); return false; }
            if (!c.Inventory.Update(jade, Role.Instance.AddMode.REMOVE, stream)) return false;
            uint uid = c.Player.UID;
            int now = (int)FestivalStore.Add(f, "growth", uid, "festival", g);
            int had = (int)(chest.ITEM_ID - firstChest) + 1, cls = Math.Max(had, ClassOf(now));
            string got = FestivalRewards.Give(c, prize, stream);
            FestivalManager.Log(c.Player.Name + " used " + Reward.ItemName(jade.ITEM_ID) + ": growth " + now + ", class " + cls + (cls > had ? " (up from " + had + ")" : "") + ", " + got);
            c.SendSysMesage("Your Celestial Chest absorbed the jade: +" + g + " Growth Point" + (g > 1 ? "s" : "") + " (" + now + ")." + (got.Length > 0 ? " You received " + got + "." : ""), MsgMessage.ChatMode.System);
            if (cls > had)
            {
                c.Inventory.Update(chest, Role.Instance.AddMode.REMOVE, stream, true);
                c.Inventory.Add(stream, firstChest + (uint)cls - 1, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, chest.Bound != 0);
                string extra = FestivalRewards.Give(c, classUp, stream);
                c.SendSysMesage("Your Celestial Chest grew to Class " + cls + "!" + (extra.Length > 0 ? " You received " + extra + "." : ""), MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                if (cls == 9) FestivalManager.Broadcast(c.Player.Name + " nourished a Class 9 Celestial Chest! The Celestial Elder is proud of such devotion.", MsgMessage.ChatMode.TopLeft);
            }
            else if (cls < 9)
                c.SendSysMesage("Growth Points to Class " + (cls + 1) + ": " + now + "/" + needed[cls] + ".", MsgMessage.ChatMode.System);
            return true;
        }

        bool OpenChest(FestivalDef f, Client.GameClient c, MsgGameItem chest, ServerSockets.Packet stream)
        {
            int cls = (int)(chest.ITEM_ID - firstChest) + 1;
            var list = new List<Reward>();
            if (cls >= 9) list.Add(new Reward { Kind = "item", Id = divineJade, Count = 1 });
            else if (c.Player.Level >= 140) list.Add(new Reward { Kind = "chi", Count = (uint)(cls * openChi) });
            else list.Add(new Reward { Kind = "exp", Count = (uint)(cls * openExp) });
            if (!FestivalRewards.HasRoom(c, list)) { c.SendSysMesage("Please make 1 more space in your inventory."); return false; }
            string got = FestivalRewards.Give(c, list, stream);
            FestivalManager.Log(c.Player.Name + " opened a Class " + cls + " Celestial Chest: " + got);
            c.SendSysMesage("You opened your Class " + cls + " Celestial Chest and received " + got + ". It can be opened again tomorrow.", MsgMessage.ChatMode.System);
            return true;
        }

        public override void OnAction(FestivalDef f, Client.GameClient c, string name, ServerSockets.Packet stream)
        {
            uint npc = c.ActiveNpc;
            var close = new List<FOption> { new FOption { Text = "I see.", Action = "close" } };
            int growth = (int)FestivalStore.Get(f, "growth", c.Player.UID, "festival");
            int cls = ClassOf(growth);
            var chest = ChestInBag(c);
            if (name == "getchest")
            {
                if (chest != null) { FestivalManager.Show(f, c, stream, npc, "You already carry a Celestial Chest. Nourish it with jades from the Celestial Mine!", close); return; }
                if (!c.Inventory.HaveSpace(1)) { FestivalManager.Show(f, c, stream, npc, "Please make 1 more space in your inventory.", close); return; }
                c.Inventory.Add(stream, firstChest + (uint)cls - 1, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                FestivalManager.Show(f, c, stream, npc, "Here is a Class " + cls + " Celestial Chest. Nourish it with the jades of the Celestial Mine, and open it once a day for your reward.", close);
                return;
            }
            if (name == "status")
            {
                string text = "Growth Points: " + growth + ". " + (chest != null ? "Your chest: Class " + (chest.ITEM_ID - firstChest + 1) + ". " : "You carry no chest. ")
                    + (cls < 9 ? "Class " + (cls + 1) + " needs " + needed[cls] + " Growth Points." : "Your chest has reached the highest class!")
                    + " Opened today: " + (FestivalStore.Get(f, "chest", c.Player.UID, "daily") > 0 ? "yes" : "no") + ".";
                FestivalManager.Show(f, c, stream, npc, text, close);
            }
        }

    }
}
