// The Legend of the Last Crescent - "take me there" without teleporting.
// The game client walks / jumps the player by itself (client path finding, MsgAction 162).
// The client path finder only works inside the current map, so the guide goes one map at a time:
// to the portal that leads towards the target map, then (after the map change) on to the next portal,
// and finally to the target. The Market has no portal: the guide leads to the Twin City Conductress.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.Ramadan
{
    public static unsafe partial class RamadanEvent
    {
        class GuideJob
        {
            public uint Map; public ushort X, Y; public uint NpcUid; public string Label;
            public uint LastMap; public ushort LastX, LastY; public DateTime LastSent; public DateTime Started; public int Resends;
        }
        static readonly ConcurrentDictionary<uint, GuideJob> guides = new ConcurrentDictionary<uint, GuideJob>();
        const uint MarketMap = 1036, TwinCityMap = 1002;
        static readonly MapPoint TwinCityConductress = new MapPoint(1002, 307, 340);
        const uint TwinCityConductressUid = 10050;
        static readonly MapPoint MarketController = new MapPoint(1036, 215, 220);
        const uint MarketControllerUid = 45;

        /// <summary>Starts walking the player to the spot. Returns false only when nothing can be done.</summary>
        static bool StartGuide(Client.GameClient c, MapPoint to, uint npcUid, string label)
        {
            if (c == null || c.Player == null || to == null) return false;
            if (!c.Player.Alive) { Tell(c, "You can not travel while you are dead.", MsgMessage.MsgColor.red); return true; }
            if (c.Player.DynamicID != 0) { Tell(c, "Leave this place first.", MsgMessage.MsgColor.red); return true; }
            var job = new GuideJob { Map = to.Map, X = to.X, Y = to.Y, NpcUid = npcUid, Label = label, Started = DateTime.Now };
            guides[c.Player.UID] = job;
            GuideStep(c, job, true);
            return true;
        }

        /// <summary>Next map to walk to from 'from' on the way to 'to' (breadth first over the portals).</summary>
        static Role.Portal NextPortal(uint from, uint to)
        {
            var prev = new Dictionary<uint, KeyValuePair<uint, Role.Portal>>();
            var queue = new Queue<uint>();
            prev[from] = new KeyValuePair<uint, Role.Portal>(0, null);
            queue.Enqueue(from);
            while (queue.Count > 0)
            {
                uint m = queue.Dequeue();
                if (m == to) break;
                Role.GameMap map;
                if (!Database.Server.ServerMaps.TryGetValue(m, out map) || map.Portals == null) continue;
                foreach (var p in map.Portals.ToArray())
                {
                    uint d = p.Destiantion_MapID;
                    if (prev.ContainsKey(d)) continue;
                    prev[d] = new KeyValuePair<uint, Role.Portal>(m, p);
                    queue.Enqueue(d);
                }
            }
            if (!prev.ContainsKey(to) || to == from) return null;
            uint cur = to;
            Role.Portal first = null;
            while (cur != from) { var e = prev[cur]; first = e.Value; cur = e.Key; }
            return first;
        }

        static string NpcLabel(string key)
        {
            switch (key)
            {
                case "Guardian": return "the Guardian of the Crescent";
                case "Merchant": return "the Crescent Merchant";
                case "Herald": return "the Crescent Herald";
                case "GateKeeper": return "the Gate Keeper";
                case "CaravanMaster": return "the Caravan Master";
                case "ScorchedLantern": return "the scorched lantern";
                default: return WitnessName(key) == "the Guardian" ? key : WitnessName(key);
            }
        }

        /// <summary>MsgAction 162 (client path finding) - same layout as the server's PathFinding(), but with the
        /// target map in dwParam (the stock builder leaves it 0 and the client then asks for map 0 = "Invalid Portal").</summary>
        static void SendPath(Client.GameClient c, ushort x, ushort y, uint npcUid)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                stream.Write(Extensions.Time32.Now.GetHashCode()); // 4
                stream.Write(c.Player.UID);                         // 8
                stream.Write((uint)c.Player.Map);                   // 12 dwParam = map
                stream.Seek(20);
                stream.Write(npcUid);                               // 20
                stream.Write((ushort)162);                          // 24 type
                stream.Seek(28);
                stream.Write(x);                                    // 28
                stream.Write(y);                                    // 30
                stream.Finalize(GamePackets.DataMap);
                c.Send(stream);
            }
        }

        static void GuideStep(Client.GameClient c, GuideJob job, bool first)
        {
            uint map = c.Player.Map;
            job.LastMap = map; job.LastX = c.Player.X; job.LastY = c.Player.Y; job.LastSent = DateTime.Now;
            if (map == job.Map)
            {
                if (Role.Core.GetDistance(c.Player.X, c.Player.Y, job.X, job.Y) <= 3) { ArriveGuide(c); return; }
                SendPath(c, job.X, job.Y, job.NpcUid);
                if (first) Tell(c, "Follow the path: you are on your way to " + job.Label + ".", MsgMessage.MsgColor.white);
                return;
            }
            var portal = NextPortal(map, job.Map);
            if (portal == null && job.Map == MarketMap)
            {
                // the Market has no portal: the Twin City Conductress takes players there
                if (map == TwinCityMap)
                {
                    SendPath(c, TwinCityConductress.X, TwinCityConductress.Y, TwinCityConductressUid);
                    Tell(c, "Walking to the Twin City Conductress. Ask her to take you to the Market.", MsgMessage.MsgColor.white);
                    return;
                }
                portal = NextPortal(map, TwinCityMap);
            }
            if (portal == null && map == MarketMap)
            {
                // leaving the Market: its controller sends players back to Twin City
                SendPath(c, MarketController.X, MarketController.Y, MarketControllerUid);
                Tell(c, "Walking to the Market Controller. Ask him to take you back to Twin City.", MsgMessage.MsgColor.white);
                return;
            }
            if (portal == null)
            {
                guides.TryRemove(c.Player.UID, out job);
                Tell(c, "There is no road from here. Go back to Twin City first.", MsgMessage.MsgColor.red);
                return;
            }
            SendPath(c, portal.X, portal.Y, 0);
            Tell(c, "Follow the path to the gate of this land. You are on your way to " + job.Label + ".", MsgMessage.MsgColor.white);
        }

        static void ArriveGuide(Client.GameClient c)
        {
            GuideJob job;
            if (guides.TryRemove(c.Player.UID, out job)) Tell(c, "You have arrived at " + job.Label + ".", MsgMessage.MsgColor.yellow);
        }

        /// <summary>Every second: after a map change (portal) continue to the next gate; resend if the player stopped.</summary>
        static void TickGuides(DateTime now)
        {
            foreach (var kv in guides.ToArray())
            {
                var c = FindOnline(kv.Key);
                GuideJob job = kv.Value, dummy;
                if (c == null || !c.Player.Alive || c.Player.DynamicID != 0 || (now - job.Started).TotalMinutes > 15) { guides.TryRemove(kv.Key, out dummy); continue; }
                if (c.Player.Map == job.Map && Role.Core.GetDistance(c.Player.X, c.Player.Y, job.X, job.Y) <= 3) { ArriveGuide(c); continue; }
                if (c.Player.Map != job.LastMap) { GuideStep(c, job, false); continue; }
                bool moved = c.Player.X != job.LastX || c.Player.Y != job.LastY;
                if (moved) { job.LastX = c.Player.X; job.LastY = c.Player.Y; job.LastSent = now; continue; }
                // waiting at the Conductress / Market Controller: nothing to resend
                if (job.Map == MarketMap && c.Player.Map == TwinCityMap
                    && Role.Core.GetDistance(c.Player.X, c.Player.Y, TwinCityConductress.X, TwinCityConductress.Y) <= 5) continue;
                if (c.Player.Map == MarketMap && job.Map != MarketMap
                    && Role.Core.GetDistance(c.Player.X, c.Player.Y, MarketController.X, MarketController.Y) <= 5) continue;
                // stood still for 4 s on the way (path finder gave up): try again, at most 3 times
                if ((now - job.LastSent).TotalSeconds >= 4)
                {
                    if (++job.Resends > 3) { guides.TryRemove(kv.Key, out dummy); Tell(c, "The path is blocked. Walk a little and ask again.", MsgMessage.MsgColor.red); continue; }
                    GuideStep(c, job, false);
                }
            }
        }
    }
}
