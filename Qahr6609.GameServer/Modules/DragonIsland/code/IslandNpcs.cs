using System;
using System.Linq;

namespace Conquer6609.Modules.DragonIsland
{
    /// <summary>
    /// The island's NPCs, through the module NPC hook (options 200-249 / 255):
    ///   Iron Tong (20499) in Twin City, where the conductress' "Dragon Island" ends: takes the player to the island.
    ///   Cobb Wind (20500) at the island's safe area: the next beast, the beast table, a ride to a living beast.
    /// </summary>
    public static class IslandNpcs
    {
        public const uint IronTong = 20499, CobbWind = 20500;
        const byte OptEnter = 200, OptBeasts = 201, OptGoTo = 210;// OptGoTo + beast index (max 30)

        static string T(string s) { return s.Replace(' ', '~'); }

        static string Times(Beast b)
        {
            var mins = b.Minutes.OrderBy(m => m).Select(m => Ordinal(m)).ToList();
            string list = mins.Count == 1 ? mins[0] : string.Join(", ", mins.Take(mins.Count - 1)) + " and " + mins.Last();
            return "the " + list + " minute of each hour, every day";
        }

        static string Ordinal(int m)
        {
            string s = m.ToString("00");
            int d = m % 10, t = m % 100;
            return s + (t >= 11 && t <= 13 ? "th" : d == 1 ? "st" : d == 2 ? "nd" : d == 3 ? "rd" : "th");
        }

        public static int Talk(IslandConfig c, Client.GameClient client, ServerSockets.Packet stream, byte option, uint npcId)
        {
            if (npcId == IronTong) { IronTongTalk(c, client, stream, option); return 1; }
            if (npcId == CobbWind) { CobbWindTalk(c, client, stream, option); return 1; }
            return 0;
        }

        static void IronTongTalk(IslandConfig c, Client.GameClient client, ServerSockets.Packet stream, byte option)
        {
            var dlg = new Game.MsgNpc.Dialog(client, stream);
            if (option == OptEnter)
            {
                if (!IslandBeasts.MayEnter(c, client))
                {
                    dlg.AddText(T("The Dragon Island is too dangerous for you. Come back when you are level " + c.MinLevel
                        + (c.MinReborn > 0 ? " and reborn " + (c.MinReborn > 1 ? "twice" : "once") : "") + "."));
                    dlg.AddOption("I see.", 255).AddAvatar(0).FinalizeDialog();
                    return;
                }
                client.Teleport((ushort)(c.EntryX - Program.GetRandom.Next(0, 5)), (ushort)(c.EntryY - Program.GetRandom.Next(0, 5)), c.Map);
                return;
            }
            dlg.AddText(T("The devils have occupied the Twisting Hell Array and are trying to summon their beasts to the Dragon Island. "
                + "Commander Shan needs every brave warrior there. Hunt the beasts and he will reward you!"));
            dlg.AddOption("Take me to the Dragon Island.", OptEnter);
            dlg.AddOption("Not now.", 255).AddAvatar(0).FinalizeDialog();
        }

        static void CobbWindTalk(IslandConfig c, Client.GameClient client, ServerSockets.Packet stream, byte option)
        {
            var dlg = new Game.MsgNpc.Dialog(client, stream);
            var beasts = c.Beasts.Where(b => b.Enabled).ToList();
            if (option == OptBeasts)
            {
                var text = "[Beast]  [Location]  [Appear Time]\n";
                foreach (var b in beasts.Where(b => b.Official).Concat(beasts.Where(b => !b.Official)).Take(8))
                    text += b.Name + " (DragonIsland " + b.X + "," + b.Y + ") " + Times(b) + "\n";
                text += "All the beasts above have left their lair for the Dragon Island.";
                dlg.AddText(T(text));
                dlg.AddOption("I see.", 255).AddAvatar(0).FinalizeDialog();
                return;
            }
            if (option >= OptGoTo && option < OptGoTo + 30)
            {
                int i = option - OptGoTo;
                if (i < beasts.Count && IslandBeasts.Alive(c, beasts[i]))
                    client.Teleport((ushort)(beasts[i].X + Program.GetRandom.Next(-3, 4)), (ushort)(beasts[i].Y + Program.GetRandom.Next(-3, 4)), c.Map);
                else
                    client.SendSysMesage("That beast is not on the island now.");
                return;
            }
            DateTime now = DateTime.Now;
            var next = beasts.Select(b => new { b, t = b.Next(now) }).Where(x => x.t != DateTime.MinValue).OrderBy(x => x.t).FirstOrDefault();
            string official = string.Join(", ", beasts.Where(b => b.Official).Select(b => b.Name));
            dlg.AddText(T("You came at the right time! The devils have occupied the Twisting Hell Array, and are trying to summon "
                + (official.Length > 0 ? official : "their beasts") + " to the Dragon Island. Commander Shan has given order to kill them at any cost. "
                + "Dragon Island is not allowed to sink!"
                + (next != null ? "\n* [" + next.b.Name + "] will appear at " + next.t.ToString("HH:mm") + " today around (" + next.b.X + "," + next.b.Y + ") on the Dragon Island.*" : "")));
            dlg.AddOption("Learn more about these beasts.", OptBeasts);
            for (int i = 0; i < beasts.Count && i < 30; i++)
                if (IslandBeasts.Alive(c, beasts[i]))
                    dlg.AddOption("Take me to the " + beasts[i].Name + ".", (byte)(OptGoTo + i));
            dlg.AddOption("I'll keep an eye on them.", 255).AddAvatar(0).FinalizeDialog();
        }

        // ------------------------------------------------------------------ Iron Tong in Twin City
        static Game.MsgNpc.Npc ironTong;

        public static void Spawn(IslandConfig c)
        {
            Remove();
            Role.GameMap map;
            if (!Database.Server.ServerMaps.TryGetValue(c.IronTongMap, out map) || map.View.MapContain(Role.MapObjectType.Npc, IronTong))
                return;
            var np = Game.MsgNpc.Npc.Create();
            np.UID = IronTong;
            np.NpcType = Role.Flags.NpcType.Talker;
            np.Mesh = (ushort)(c.IronTongLook * 10 + 2);
            np.Map = c.IronTongMap;
            np.X = c.IronTongX;
            np.Y = c.IronTongY;
            map.AddNpc(np);
            ironTong = np;
        }

        public static void Remove()
        {
            var np = ironTong;
            if (np == null) return;
            ironTong = null;
            Role.GameMap map;
            using (var rec = new ServerSockets.RecycledPacket())
                if (Database.Server.ServerMaps.TryGetValue(np.Map, out map))
                    try { map.RemoveNpc(np, rec.GetStream()); } catch (Exception e) { MyConsole.WriteException(e); }
        }
    }
}
