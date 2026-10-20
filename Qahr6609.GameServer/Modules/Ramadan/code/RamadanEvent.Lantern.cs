// The Legend of the Last Crescent - the Crescent Lantern quest.
// The Last Crescent broke into 7 pieces. The Guardian gives the player a lantern that gets warmer
// the closer the player walks to the hidden piece (cold / cool / warm / hot / burning).
// Every player gets his own hiding spots, so nobody can just copy coordinates.
// One piece per land; the NPC of that land ("witness") saw where it fell and tells the direction.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.Ramadan
{
    public static unsafe partial class RamadanEvent
    {
        /// <summary>Where each piece fell: the land, the NPC who saw it, and how far from that NPC it may lie.</summary>
        public class LanternLand
        {
            public string Witness; public string Land; public int MinDist; public int MaxDist;
            public LanternLand(string witness, string land, int min, int max) { Witness = witness; Land = land; MinDist = min; MaxDist = max; }
        }

        public static readonly LanternLand[] LanternLands =
        {
            new LanternLand("Lamplighter", "the Market",      4, 16),
            new LanternLand("Rashid",      "Twin City",      10, 30),
            new LanternLand("Noor",        "Phoenix Castle", 10, 30),
            new LanternLand("Kareem",      "Ape City",       10, 30),
            new LanternLand("Salma",       "Desert City",    10, 30),
            new LanternLand("Watchman",    "Bird Island",    10, 30),
            new LanternLand("Guardian",    "Twin City, near the mosque", 8, 22),
        };

        class LanternHint { public int Band = -1; public uint Map; public long At, FxAt, SpotAt; public bool SpotTold; }
        static readonly ConcurrentDictionary<uint, LanternHint> lanternHints = new ConcurrentDictionary<uint, LanternHint>();

        static long NowMs() { return Environment.TickCount & int.MaxValue; }

        static string WitnessName(string key)
        {
            switch (key)
            {
                case "Lamplighter": return "the Old Lamplighter";
                case "Rashid": return "Captain Rashid";
                case "Noor": return "Sage Noor";
                case "Kareem": return "Master Kareem";
                case "Salma": return "Widow Salma";
                case "Watchman": return "the Night Watchman";
                default: return "the Guardian";
            }
        }

        // ------------------------------------------------------------------ hiding spots
        /// <summary>Random reachable cell between MinDist and MaxDist steps from the witness (8-way walk over valid cells).</summary>
        static MapPoint PickSpot(LanternLand land)
        {
            var at = Spot(land.Witness).At;
            Role.GameMap map;
            if (!Database.Server.ServerMaps.TryGetValue(at.Map, out map)) return null;
            var blocked = blockedCells;
            var dist = new Dictionary<int, int>();
            var queue = new Queue<int>();
            Func<int, int, int> key = (x, y) => (x << 16) | y;
            // start from the free cells around the NPC (the NPC cell itself is not walkable)
            for (int dx = -2; dx <= 2; dx++)
                for (int dy = -2; dy <= 2; dy++)
                {
                    int x = at.X + dx, y = at.Y + dy;
                    if (x <= 0 || y <= 0 || !map.ValidLocation((ushort)x, (ushort)y)) continue;
                    int k = key(x, y);
                    if (dist.ContainsKey(k)) continue;
                    dist[k] = 0; queue.Enqueue(k);
                }
            var candidates = new List<int>();
            while (queue.Count > 0)
            {
                int k = queue.Dequeue();
                int cx = k >> 16, cy = k & 0xFFFF, d = dist[k];
                int cheb = Math.Max(Math.Abs(cx - at.X), Math.Abs(cy - at.Y));
                if (cheb >= land.MinDist && cheb <= land.MaxDist && !blocked.Contains(CellKey(at.Map, cx, cy))) candidates.Add(k);
                if (d >= land.MaxDist * 2) continue;
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        int x = cx + dx, y = cy + dy;
                        if (x <= 0 || y <= 0 || Math.Max(Math.Abs(x - at.X), Math.Abs(y - at.Y)) > land.MaxDist) continue;
                        int nk = key(x, y);
                        if (dist.ContainsKey(nk) || !map.ValidLocation((ushort)x, (ushort)y) || blocked.Contains(CellKey(at.Map, x, y))) continue;
                        dist[nk] = d + 1; queue.Enqueue(nk);
                    }
            }
            if (candidates.Count == 0) return null;
            int pick = candidates[Roll(candidates.Count)];
            return new MapPoint(at.Map, (ushort)(pick >> 16), (ushort)(pick & 0xFFFF));
        }

        static List<MapPoint> NewTargets()
        {
            var r = new List<MapPoint>();
            foreach (var land in LanternLands)
            {
                var p = PickSpot(land);
                if (p == null) { Store.Log(0, "", "lantern-no-spot", land.Witness); var at = Spot(land.Witness).At; p = new MapPoint(at.Map, (ushort)(at.X + 3), (ushort)(at.Y + 3)); }
                r.Add(p);
            }
            return r;
        }

        static MapPoint CurrentTarget(RamadanPlayerState s)
        {
            if (s.LanternStep < 1 || s.LanternStep > Lantern.Pieces) return null;
            var t = Lantern.ParseTargets(s.LanternTargets);
            return t.Count == Lantern.Pieces ? t[s.LanternStep - 1] : null;
        }

        // ------------------------------------------------------------------ effects
        static void PlayEffect(Client.GameClient c, string effect)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    c.Player.SendString(rec.GetStream(), MsgStringPacket.StringID.Effect, true, effect);
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        static void PlayEffectOn(Client.GameClient c, uint uid, string effect)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    c.Player.SendString(rec.GetStream(), MsgStringPacket.StringID.Effect, uid, true, effect);
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        /// <summary>Effect on a map cell, sent to this player only (the ground marker of his own hiding spot).</summary>
        static void PlayCellEffect(Client.GameClient c, ushort x, ushort y, string effect)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var p = new MsgStringPacket { ID = MsgStringPacket.StringID.LocationEffect, X = x, Y = y, Strings = new[] { effect } };
                    c.Send(rec.GetStream().StringPacketCreate(p));
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        /// <summary>Near the piece a light marks the exact cell on the ground (like the quest spots of Conquer Online).
        /// Called on every step and once a second, so the marker stays while the player stands still.</summary>
        static void SpotPulse(Client.GameClient c, MapPoint t, LanternHint h, long now)
        {
            if (Config.LanternSpotFx.Length == 0 || Config.LanternSpotRange <= 0) return;
            if (c.Player.Map != t.Map || c.Player.DynamicID != 0) return;
            int d = Math.Max(Math.Abs(c.Player.X - t.X), Math.Abs(c.Player.Y - t.Y));
            if (d > Config.LanternSpotRange) { h.SpotTold = false; return; }
            if (now - h.SpotAt < 2500) return;
            h.SpotAt = now;
            PlayCellEffect(c, t.X, t.Y, Config.LanternSpotFx);
            if (!h.SpotTold)
            {
                h.SpotTold = true;
                Tell(c, "Your lantern lights up a spot on the ground nearby. Stand on the light to pick up the piece!", MsgMessage.MsgColor.yellow);
            }
        }

        /// <summary>Every second: keep the ground marker alive for players standing near their spot.</summary>
        static void TickLanternSpots()
        {
            if (Config.LanternSpotFx.Length == 0) return;
            long now = NowMs();
            foreach (var kv in Online.ToArray())
            {
                MapPoint t;
                lock (kv.Value.Sync) t = CurrentTarget(kv.Value);
                if (t == null) continue;
                var c = FindOnline(kv.Key);
                if (c == null || c.Fake) continue;
                SpotPulse(c, t, lanternHints.GetOrAdd(kv.Key, _ => new LanternHint()), now);
            }
        }

        static string BandEffect(int band) { return band >= 1 && band <= 5 ? Config.LanternFx[band] : ""; }

        // ------------------------------------------------------------------ Ramadan night (screen tint while the quest runs)
        static readonly ConcurrentDictionary<uint, uint> nightMap = new ConcurrentDictionary<uint, uint>(); // uid -> map that was tinted

        public static void SetMapColor(Client.GameClient c, uint rgb)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    ActionQuery a = new ActionQuery { ObjId = c.Player.UID, Type = ActionType.SetMapColor, dwParam = rgb, wParam1 = c.Player.X, wParam2 = c.Player.Y };
                    c.Send(stream.ActionCreate(&a));
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }

        static bool QuestRunning(RamadanPlayerState s) { return s.LanternStep >= 1 && s.LanternStep <= Lantern.AllFound; }

        /// <summary>Every second: players on the quest see the night; it is sent again after every map change (the client resets it).</summary>
        static void TickNight()
        {
            if (Config.NightColor == 0) return;
            foreach (var kv in Online.ToArray())
            {
                bool running;
                lock (kv.Value.Sync) running = QuestRunning(kv.Value);
                uint last;
                bool tinted = nightMap.TryGetValue(kv.Key, out last);
                if (!running) { if (tinted) { nightMap.TryRemove(kv.Key, out last); var d = FindOnline(kv.Key); if (d != null) SetMapColor(d, Config.DayColor); } continue; }
                var c = FindOnline(kv.Key);
                if (c == null) continue;
                if (tinted && last == c.Player.Map) continue;
                nightMap[kv.Key] = c.Player.Map;
                SetMapColor(c, Config.NightColor);
            }
        }

        // ------------------------------------------------------------------ the lantern (called on every walk / jump step)
        static void LanternOnMove(Client.GameClient c, int x, int y)
        {
            if (!Active || c == null || c.Player == null || c.Fake) return;
            RamadanPlayerState s;
            if (!Online.TryGetValue(c.Player.UID, out s)) return;
            MapPoint t; int step;
            lock (s.Sync) { step = s.LanternStep; t = CurrentTarget(s); }
            if (t == null) return;
            var h = lanternHints.GetOrAdd(c.Player.UID, _ => new LanternHint());
            long now = NowMs();
            if (c.Player.Map != t.Map || c.Player.DynamicID != 0)
            {
                if (h.Map != c.Player.Map) // say it once per land
                {
                    h.Map = c.Player.Map; h.Band = -1; h.At = now;
                    Tell(c, "Your lantern is dark here. Crescent Piece " + step + " is in " + LanternLands[step - 1].Land + ".", MsgMessage.MsgColor.white);
                }
                return;
            }
            int d = Math.Max(Math.Abs(x - t.X), Math.Abs(y - t.Y));
            if (d <= Config.LanternFoundRange) { FoundPiece(c, s, step); return; }
            int band = Math.Max(1, Lantern.Band(d));
            SpotPulse(c, t, h, now);
            bool changed = band != h.Band || h.Map != c.Player.Map;
            if (!changed && now - h.FxAt >= 3000 && BandEffect(band).Length > 0) { h.FxAt = now; PlayEffect(c, BandEffect(band)); }
            if (!changed && now - h.At < 8000) return;
            h.Band = band; h.Map = c.Player.Map; h.At = now;
            string text = "Lantern: " + Lantern.BandText(band);
            if (band == 1) text += " Take a step " + Lantern.ScreenDirection(t.X - x, t.Y - y) + ".";
            Tell(c, text, band <= 2 ? MsgMessage.MsgColor.red : band == 3 ? MsgMessage.MsgColor.orange : MsgMessage.MsgColor.cyan);
            if (changed)
            {
                if (Config.NightColor != 0 && Config.LanternColors[band] != 0) SetMapColor(c, Config.LanternColors[band]); // the sky warms up
                if (BandEffect(band).Length > 0) { h.FxAt = now; PlayEffect(c, BandEffect(band)); }
            }
        }

        static void FoundPiece(Client.GameClient c, RamadanPlayerState s, int step)
        {
            lock (s.Sync)
            {
                if (s.LanternStep != step) return; // already counted (two steps in a row)
                s.LanternStep = step + 1;
                s.Dirty = true;
            }
            LanternHint h;
            if (lanternHints.TryGetValue(c.Player.UID, out h)) { h.Band = -1; h.Map = 0; h.SpotTold = false; h.SpotAt = 0; }
            Store.Log(s.UID, s.Name, "lantern", "piece " + step);
            if (Config.LanternFoundFx.Length > 0) PlayEffect(c, Config.LanternFoundFx);
            if (Config.NightColor != 0) SetMapColor(c, Config.NightColor);
            Center(c, "You found Crescent Piece " + step + " of " + Lantern.Pieces + "!");
            Grant(c, s, "lantern:piece:" + step, "LanternPiece", "Crescent Piece " + step + " of " + Lantern.Pieces);
            if (step < Lantern.Pieces)
            {
                var next = LanternLands[step];
                Tell(c, "Next: Crescent Piece " + (step + 1) + " fell in " + next.Land + ". Your lantern pulls you to " + WitnessName(next.Witness) + ", who saw it fall.", MsgMessage.MsgColor.yellow);
                if (Config.WalkToNpc && next.Witness != "Guardian") StartGuide(c, Spot(next.Witness).At, Spot(next.Witness).UID, WitnessName(next.Witness));
                else if (next.Witness == "Guardian") Tell(c, "The last piece fell close to the mosque in Twin City. Walk around the mosque and watch your lantern.", MsgMessage.MsgColor.yellow);
            }
            else
            {
                Tell(c, "You have all 7 pieces! Your lantern pulls you back to the Guardian of the Crescent.", MsgMessage.MsgColor.yellow);
                if (Config.WalkToNpc) StartGuide(c, Spot("Guardian").At, Spot("Guardian").UID, "the Guardian of the Crescent");
            }
            SaveState(s);
        }

        // ------------------------------------------------------------------ Guardian (the quest giver)
        static string StepTitle(int step) { return "Step " + Math.Min(Lantern.TotalSteps, step + 1) + " of " + Lantern.TotalSteps + ": "; }

        static void LanternGuardian(Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            int step;
            lock (s.Sync) step = s.LanternStep;

            if (option == 1 && step == 0)
            {
                var targets = NewTargets();
                lock (s.Sync)
                {
                    if (s.LanternStep != 0) return;
                    s.LanternTargets = Lantern.FormatTargets(targets);
                    s.LanternStep = 1;
                    if (s.Stage == QuestStage.NotStarted)
                    {
                        s.Stage = QuestStage.Introduction; // nightly tasks open with the quest
                        s.Claims.Add("intro");             // the old story's intro reward is replaced by the piece rewards
                    }
                    s.Dirty = true;
                }
                SaveState(s);
                Store.Log(s.UID, s.Name, "lantern", "start " + Lantern.FormatTargets(targets));
                if (Config.LanternStartFx.Length > 0) PlayEffect(c, Config.LanternStartFx);
                Tell(c, "The night of the Last Crescent has fallen. Find the 7 pieces before the sky stays dark.", MsgMessage.MsgColor.white);
                step = 1;
            }
            else if (option == 2)
            {
                Say(c, stream, "Guardian", "While you search, the night sky follows your lantern. Deep blue = far away. Warm = closer. Bright gold = the piece is right next to you, and the lantern tells you which way to step. Walk onto the spot and the piece is yours.",
                    new Opt("I understand.", 0));
                return;
            }
            else if (option == 3 && step == Lantern.AllFound)
            {
                lock (s.Sync)
                {
                    if (s.LanternStep != Lantern.AllFound) return;
                    s.LanternStep = Lantern.Done;
                    if (s.Stage < QuestStage.FinalGateUnlocked)
                    {
                        s.Stage = QuestStage.FinalGateUnlocked; // the Forgotten Gate (boss) opens for him
                        s.Claims.Add("intro"); s.Claims.Add("clues"); s.Claims.Add("gate"); // old story rewards are replaced, not paid twice
                    }
                    s.Dirty = true;
                }
                Store.Log(s.UID, s.Name, "lantern", "complete");
                Grant(c, s, "lantern:complete", "LanternComplete", "The Last Crescent shines again");
                SaveState(s);
                if (Config.LanternCompleteFx.Length > 0) PlayEffect(c, Config.LanternCompleteFx);
                uint mosqueUid = MosqueUid();
                if (mosqueUid != 0) PlayEffectOn(c, mosqueUid, "firework-like");
                Broadcast(c.Player.Name + " brought back all 7 Crescent Pieces. The mosque in Twin City shines again!", MsgMessage.ChatMode.Center, MsgMessage.MsgColor.yellow);
                step = Lantern.Done;
            }

            if (step == 0)
            {
                Say(c, stream, "Guardian", StepTitle(0) + "Ramadan Kareem, traveler. Last night the Crescent Moon broke and fell to the ground in 7 pieces. Take my lantern. It gets WARMER when you walk close to a piece and COLDER when you walk away. Find the 7 pieces and bring them back to this mosque.",
                    new Opt("Give me the lantern. I will find them.", 1));
                return;
            }
            if (step >= 1 && step <= Lantern.Pieces)
            {
                var land = LanternLands[step - 1];
                string who = land.Witness == "Guardian" ? "I saw it fall close to this mosque." : WitnessName(land.Witness) + " saw where it fell. Ask him first.";
                if (land.Witness == "Salma" || land.Witness == "Noor") who = WitnessName(land.Witness) + " saw where it fell. Ask her first.";
                var opts = new List<Opt>();
                if (land.Witness != "Guardian") opts.Add(Go(land.Witness, "Take me to " + WitnessName(land.Witness) + "."));
                opts.Add(new Opt("How does the lantern work?", 2));
                opts.Add(new Opt("Tonight's small tasks.", 20));
                Say(c, stream, "Guardian", StepTitle(step) + "Find Crescent Piece " + step + " of " + Lantern.Pieces + ". It fell in " + land.Land + ". " + who + " Then follow your lantern.", opts.ToArray());
                return;
            }
            if (step == Lantern.AllFound)
            {
                Say(c, stream, "Guardian", StepTitle(Lantern.AllFound) + "You carry all 7 pieces! Give them to me and the crescent will shine over this mosque again.",
                    new Opt("Here are the 7 Crescent Pieces.", 3));
                return;
            }
            Say(c, stream, "Guardian", "Quest complete! The Last Crescent shines again, thanks to you. The Gate Keeper will now open the Forgotten Gate for you, where the Eclipse Sovereign hides. Come back every night for small tasks.",
                Go("GateKeeper", "Take me to the Gate Keeper."), new Opt("Tonight's small tasks.", 20), Go("Merchant", "Take me to the Crescent Merchant."));
        }

        static uint MosqueUid()
        {
            lock (Decor)
                for (int i = 0; i < Decor.Count; i++)
                    if (Decor[i].Type == "mosque") return DecorUidBase + (uint)i;
            return 0;
        }

        // ------------------------------------------------------------------ witnesses (one per land)
        static bool LanternWitness(string key, Client.GameClient c, RamadanPlayerState s, ServerSockets.Packet stream, byte option)
        {
            int idx = Array.FindIndex(LanternLands, l => l.Witness == key);
            if (idx < 0 || key == "Guardian") return false;
            if (key == "Lamplighter" && option == 2) return false; // nightly iftar basket: old handler delivers it
            int step, errand; MapPoint t;
            lock (s.Sync) { step = s.LanternStep; t = CurrentTarget(s); errand = s.ErrandStep; }
            var extra = new List<Opt>();
            if (key == "Lamplighter" && errand == 1) extra.Add(new Opt("The Guardian sends you an iftar basket.", 2));
            if (step == idx + 1 && t != null)
            {
                var at = Spot(key).At;
                int d = Math.Max(Math.Abs(t.X - at.X), Math.Abs(t.Y - at.Y));
                string how = d <= 12 ? "not far" : "a little far";
                extra.Add(new Opt("Thank you.", 255));
                Say(c, stream, key, "Yes! I saw a light fall from the sky, " + how + " from here. Walk " + Lantern.ScreenDirection(t.X - at.X, t.Y - at.Y) + " from where I stand. Watch the sky: it gets warmer and brighter as you get closer.",
                    extra.ToArray());
                return true;
            }
            if (step >= 1 && step <= Lantern.Pieces && step != idx + 1)
            {
                var land = LanternLands[step - 1];
                extra.Add(land.Witness == "Guardian" ? Go("Guardian", "Take me to the Guardian.") : Go(land.Witness, "Take me to " + WitnessName(land.Witness) + "."));
                Say(c, stream, key, "No light fell here. Your next piece fell in " + land.Land + ".", extra.ToArray());
                return true;
            }
            if (step == 0)
            {
                extra.Add(Go("Guardian", "Take me to the Guardian."));
                Say(c, stream, key, "Ramadan Kareem! The Guardian of the Crescent, at the mosque in Twin City, is looking for help.", extra.ToArray());
                return true;
            }
            if (step == Lantern.AllFound)
            {
                extra.Add(Go("Guardian", "Take me to the Guardian."));
                Say(c, stream, key, "You carry all 7 pieces! Hurry to the Guardian at the mosque.", extra.ToArray());
                return true;
            }
            Say(c, stream, key, "Thank you for bringing the crescent back. May your fast be accepted.", extra.ToArray());
            return true;
        }
    }
}
