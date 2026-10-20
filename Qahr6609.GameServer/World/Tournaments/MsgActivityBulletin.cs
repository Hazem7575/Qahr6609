using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    /// <summary>
    /// "Today's Activities" (Bulletin) panel: the client sends MsgAction 166 (ActionType.Bulletin) with the activity
    /// id from its ini/Activity.ini in dwParam when the player presses "Go". Like official, Go takes the player to
    /// the NPC that runs the activity (the NPC is looked up on its map; the fixed spot is used if it is not spawned).
    /// </summary>
    public static class ActivityBulletin
    {
        private class Target
        {
            public string Name;
            public uint NpcUID;
            public ushort Map, X, Y;
            public Target(string name, uint npc, ushort map, ushort x, ushort y)
            {
                Name = name; NpcUID = npc; Map = map; X = x; Y = y;
            }
        }

        private static readonly Dictionary<uint, Target> Targets = new Dictionary<uint, Target>();
        private static readonly Dictionary<uint, string> NotAvailable = new Dictionary<uint, string>();

        private static void Add(string name, uint npc, ushort map, ushort x, ushort y, params uint[] ids)
        {
            foreach (var id in ids)
                Targets[id] = new Target(name, npc, map, x, y);
        }

        static ActivityBulletin()
        {
            // Activity.ini id -> NPC (fixed spot = the event's invitation teleport, next to the NPC)
            Add("Horse Racing", (uint)MsgNpc.NpcID.SteedRace, 1002, 286, 147, 1);
            Add("Team PK Tournament", (uint)MsgNpc.NpcID.TeamPkManager, 1002, 292, 146, 2, 3, 4, 5);
            Add("Dis City", (uint)MsgNpc.NpcID.TwinDisCityMain, 1002, 297, 150, 8);
            Add("Elite PK Tournament", (uint)MsgNpc.NpcID.ElitePk, 1002, 302, 151, 9, 10, 11, 12);
            Add("Weekly PK War", (uint)MsgNpc.NpcID.PkWar, 1002, 324, 194, 14, 37);
            Add("Monthly PK War", (uint)MsgNpc.NpcID.PkWar, 1002, 324, 194, 54);
            Add("Capture the Flag", (uint)MsgNpc.NpcID.GuildConductor, 1002, 226, 238, 15);// CTF is entered at the Guild Controller (Questinfo 136); NPC 8713 is not spawned
            Add("Flame Lit", (uint)MsgNpc.NpcID.FlameTaoist, 1002, 251, 216, 16);
            Add("Guild War", (uint)MsgNpc.NpcID.GuildConductor, 1002, 228, 240, 17, 35);
            Add("Skill Team PK Tournament", (uint)MsgNpc.NpcID.SkillTeamPkManager, 1002, 317, 142, 18, 19, 20, 21);
            Add("Treasure in the Blue", (uint)MsgNpc.NpcID.SquidwardOctopus, 1002, 297, 208, 22);
            Add("Power Arena", (uint)MsgNpc.NpcID.ArenaManagerWang, 1002, 338, 138, 81);
            Add("Class PK War", (uint)MsgNpc.NpcID.ClassPkEnvoy, 1002, 308, 144, 82);
            Add("Kingdom War", (uint)MsgNpc.NpcID.KingdomMissionEnvoy, 1002, 283, 293, 91);// envoy at its official TC spot 281,291 (Questinfo)
            Add("Labyrinth", (uint)MsgNpc.NpcID.Simon, 1002, 265, 138, 98);
            Add("Tower of Mystery", (uint)MsgNpc.NpcID.TowerKeeper, 4020, 85, 75, 104);
            Add("Class 6 House", (uint)MsgNpc.NpcID.HouseAdmin, 1036, 200, 95, 105);
            Add("CS Elite PK Tournament", (uint)MsgNpc.NpcID.CSElitePkManager, 5000, 0, 0, 146);// only if spawned
            Add("Dragon Island", 0, 10137, 90, 409, 68);// older clients
            Add("Chasm of Chaos", (uint)MsgNpc.NpcID.YuKoon, 10137, 452, 482, 267);// "Head to see Yu Koon" (Activity.ini); Yu Koon at the Giant Chasm entrance 10137 450,480 (MapDestination.dat)

            // in Activity.ini but not on this server / dated events whose dates are over
            // 38: official = quest 2376 of Taoist Pure (Market 187,220): kill invaders in 5 battlefields on Tue/Thu/Sat.
            // The invaders/battlefields are not on this server (MsgBattleField is a different, custom CP event).
            NotAvailable[38] = "Battlefield Heroes";
            // dated 2017 events (Activity.ini type 2): no NPC script / event logic on this server
            NotAvailable[90] = "Peach Banquet";
            NotAvailable[99] = "Rainbow Fashion Deals";
            NotAvailable[204] = "Trials of Power";
            NotAvailable[222] = "Myth of Celestial Fox";
            NotAvailable[30] = "Father`s Day";
            NotAvailable[40] = "Swap Rush";
            NotAvailable[52] = "Anniversary Shopping Rush";
            NotAvailable[85] = "Mr. & Ms. Conquer";
            NotAvailable[102] = "Mr. & Ms. Conquer";
            NotAvailable[84] = "The Eve of the Kingdom War";
            NotAvailable[93] = "Frozen Fantasy";
            NotAvailable[103] = "Celestial Chest";
            NotAvailable[87] = "Underworld Devils";
            NotAvailable[116] = "Euro Championship";
            NotAvailable[203] = "Secret of the Elements";
            NotAvailable[28] = "Chicken with Brains";
        }

        public static void Go(Client.GameClient client, ServerSockets.Packet stream, uint activityId)
        {
            Target target;
            if (Targets.TryGetValue(activityId, out target))
            {
                ushort map, x, y;
                if (Locate(target, out map, out x, out y))
                {
                    client.Teleport(x, y, map);
                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                    return;
                }
                client.SendSysMesage(target.Name + " is not available on this server at the moment.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.yellow);
                return;
            }
            string name;
            if (NotAvailable.TryGetValue(activityId, out name))
                client.SendSysMesage("Sorry, " + name + " is not running at the moment.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.yellow);
            else
                client.SendSysMesage("Sorry, this activity is not available at the moment.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.yellow);
            if (client.ProjectManager)
                client.SendSysMesage("[PM] Bulletin activity id " + activityId + " has no target.", MsgServer.MsgMessage.ChatMode.Talk, MsgServer.MsgMessage.MsgColor.red);
        }

        // NPC spawned -> a free cell next to it; otherwise the fixed spot (if the map exists).
        private static readonly int[][] Offsets = { new[] { 2, 2 }, new[] { 0, 3 }, new[] { 3, 0 }, new[] { -2, 2 }, new[] { 2, -2 }, new[] { 0, -3 }, new[] { -3, 0 }, new[] { -2, -2 }, new[] { 1, 1 } };
        private static bool Locate(Target target, out ushort map, out ushort x, out ushort y)
        {
            map = x = y = 0;
            // 5000 (NPCJail) / 6000 (PKerJail) hold parked NPCs: never a Go destination.
            if (target.Map == 5000 || target.Map == 6000)
                return false;
            Role.GameMap gameMap;
            if (!Database.Server.ServerMaps.TryGetValue(target.Map, out gameMap))
                return false;
            if (target.NpcUID != 0)
            {
                var npc = gameMap.View.GetMapObject<MsgNpc.Npc>(Role.MapObjectType.Npc, target.NpcUID);
                if (npc != null)
                {
                    foreach (var o in Offsets)
                    {
                        int nx = npc.X + o[0], ny = npc.Y + o[1];
                        if (nx <= 0 || ny <= 0 || nx > ushort.MaxValue || ny > ushort.MaxValue)
                            continue;
                        if (gameMap.ValidLocation((ushort)nx, (ushort)ny))
                        {
                            map = target.Map; x = (ushort)nx; y = (ushort)ny;
                            return true;
                        }
                    }
                }
            }
            if (target.X == 0)
                return false;// no fixed spot: only next to the NPC
            if (!gameMap.ValidLocation(target.X, target.Y))
                return false;
            map = target.Map; x = target.X; y = target.Y;
            return true;
        }
    }
}
