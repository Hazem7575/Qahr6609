// The Legend of the Last Crescent - GM commands (@ramadan ...). Reached only from the GM branch of ChatCommands.
using System;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.Ramadan
{
    public static unsafe partial class RamadanEvent
    {
        const string GmHelp = "@ramadan status | on | off | reload | caravan start|stop | decor add mosque|lantern|set|date [dir] | decor list | decor del <n> | me | lantern [step] | fx <name> | night <rrggbb> | stage <0-7> | tokens <n>";

        public static void GmCommand(Client.GameClient client, string[] data)
        {
            try
            {
                if (client == null || !(client.GameMaster || client.ProjectManager)) return;
                string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "status";
                string arg = data.Length > 2 ? data[2].ToLowerInvariant() : "";
                Store.Log(client.Player.UID, client.Player.Name, "gm", string.Join(" ", data));
                switch (sub)
                {
                    case "status":
                        {
                            Gm(client, "Ramadan: Loaded=" + Loaded + " Enabled=" + Config.Enabled + " Active=" + Active + " NPCs=" + npcsSpawned
                                + " Window " + Config.Start.ToString("yyyy-MM-dd HH:mm") + " -> " + Config.End.ToString("yyyy-MM-dd HH:mm")
                                + " Day=" + (RamadanRules.EventDay(Config, DateTime.Now) + 1));
                            Gm(client, "Online states=" + Online.Count + " EventMobs=" + EventMobs.Count + " Decor=" + Decor.Count + " Warnings=" + Config.Errors.Count);
                            Gm(client, CaravanStatusText(client.Player.UID));
                            Gm(client, RunsText());
                            foreach (var e in Config.Errors.Take(5)) Gm(client, "warn: " + e);
                            break;
                        }
                    case "on":
                    case "off":
                        {
                            bool on = sub == "on";
                            Config.Enabled = on;
                            PersistEnabled(on);
                            if (!on)
                            {
                                StopCaravan("The Caravan of Light was called off.", true);
                                EndAllRuns("The event was paused by a GM. You were sent back.");
                                DespawnNpcs();
                                RemoveAllMobs(m => true);
                                Save();
                            }
                            Gm(client, "Ramadan event " + (on ? "ON" : "OFF") + " (saved in event.ini). Active=" + Active + (on && !Active ? " - outside the Start/End window!" : ""));
                            break;
                        }
                    case "reload":
                        {
                            Save();
                            StopCaravan("The Caravan of Light was called off.", true);
                            EndAllRuns("The event is being reloaded. You were sent back.");
                            RemoveAllMobs(m => true);
                            DespawnNpcs();
                            LoadConfig();
                            if (NpcsUp) SpawnNpcs(); else DespawnNpcs();
                            Gm(client, "event.ini and decor.txt reloaded. Enabled=" + Config.Enabled + " Active=" + Active + " Warnings=" + Config.Errors.Count);
                            foreach (var e in Config.Errors.Take(5)) Gm(client, "warn: " + e);
                            break;
                        }
                    case "caravan":
                        {
                            if (arg == "start")
                            {
                                if (!Active) { Gm(client, "The event is not active."); break; }
                                var err = StartCaravan("gm" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                                Gm(client, err ?? "Caravan started.");
                            }
                            else if (arg == "stop") { StopCaravan("A GM ended the Caravan of Light. Defenders received partial rewards.", true); Gm(client, "Caravan stopped (partial rewards)."); }
                            else Gm(client, CaravanStatusText(client.Player.UID));
                            break;
                        }
                    case "decor":
                        {
                            if (arg == "add")
                            {
                                string type = data.Length > 3 ? data[3].ToLowerInvariant() : "";
                                byte dir = 0;
                                if (!DecorTypes.Contains(type)) { Gm(client, "type must be mosque, lantern, set or date"); break; }
                                if (data.Length > 4) byte.TryParse(data[4], out dir);
                                lock (Decor)
                                {
                                    if (Decor.Count >= 100) { Gm(client, "too many decor objects (100)."); break; }
                                    Decor.Add(new DecorItem { Type = type, Map = client.Player.Map, X = client.Player.X, Y = client.Player.Y, Dir = (byte)(dir % 8) });
                                }
                                SaveDecor();
                                if (npcsSpawned) SpawnNpcs();
                                Gm(client, "Decor '" + type + "' placed at " + client.Player.Map + "," + client.Player.X + "," + client.Player.Y + " and saved.");
                            }
                            else if (arg == "list")
                            {
                                lock (Decor)
                                    for (int i = 0; i < Decor.Count; i++)
                                        Gm(client, i + ": " + Decor[i].Type + " " + Decor[i].Map + "," + Decor[i].X + "," + Decor[i].Y + " dir " + Decor[i].Dir);
                                if (Decor.Count == 0) Gm(client, "no decor.");
                            }
                            else if (arg == "del")
                            {
                                int n;
                                if (data.Length < 4 || !int.TryParse(data[3], out n)) { Gm(client, "@ramadan decor del <n>"); break; }
                                lock (Decor)
                                {
                                    if (n < 0 || n >= Decor.Count) { Gm(client, "no decor #" + n); break; }
                                    Decor.RemoveAt(n);
                                }
                                SaveDecor();
                                if (npcsSpawned) SpawnNpcs();
                                Gm(client, "Decor #" + n + " removed.");
                            }
                            else Gm(client, "@ramadan decor add <type> [dir] | list | del <n>");
                            break;
                        }
                    case "me":
                        {
                            var s = GetState(client);
                            if (s == null) break;
                            lock (s.Sync)
                            {
                                Gm(client, "Stage=" + s.Stage + " Clues=" + s.Clues + " Seals=" + s.Seals + " Tokens=" + s.Tokens + " Points=" + s.Points + " Stamps=" + s.StampCount + "/" + Stamps.Total);
                                Gm(client, "Daily day " + (s.DailyDay + 1) + ": " + string.Join(", ", Enumerable.Range(0, 3).Select(i => (DailyTaskType)s.DailyTasks[i] + " " + s.DailyProgress[i] + ((s.DailyDoneMask & (1 << i)) != 0 ? " done" : ""))) + " Caravans=" + s.CaravanRuns + " BossClears=" + s.BossClears + " Claims=" + s.Claims.Count);
                            }
                            break;
                        }
                    case "fx": // test helper: @ramadan fx <EffectName>  (plays a client 3D effect on you; names are case sensitive)
                        {
                            if (data.Length < 3) { Gm(client, "@ramadan fx <EffectName>"); break; }
                            PlayEffect(client, data[2]);
                            Gm(client, "effect " + data[2]);
                            break;
                        }
                    case "spot": // test helper: @ramadan spot <EffectName>  (plays an effect on the ground 2 cells away, for you only)
                        {
                            if (data.Length < 3) { Gm(client, "@ramadan spot <EffectName>"); break; }
                            PlayCellEffect(client, (ushort)(client.Player.X + 2), (ushort)(client.Player.Y + 2), data[2]);
                            Gm(client, "ground effect " + data[2] + " at " + (client.Player.X + 2) + "," + (client.Player.Y + 2));
                            break;
                        }
                    case "night": // test helper: @ramadan night <rrggbb>  (screen tint for you only; ffffff = normal)
                        {
                            uint col;
                            if (data.Length < 3 || !uint.TryParse(data[2], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out col)) { Gm(client, "@ramadan night <rrggbb>"); break; }
                            SetMapColor(client, col);
                            Gm(client, "tint " + col.ToString("X6"));
                            break;
                        }
                    case "lantern": // test helper: @ramadan lantern [step 0-9]
                        {
                            var s = GetState(client);
                            if (s == null) break;
                            int n;
                            if (int.TryParse(arg, out n) && n >= 0 && n <= Lantern.Done)
                            {
                                lock (s.Sync)
                                {
                                    s.LanternStep = n;
                                    if (n == 0) s.LanternTargets = "";
                                    else if (Lantern.ParseTargets(s.LanternTargets).Count != Lantern.Pieces) s.LanternTargets = Lantern.FormatTargets(NewTargets());
                                    s.Dirty = true;
                                }
                                SaveState(s);
                                Store.Log(s.UID, s.Name, "gm-lantern", "step " + n);
                            }
                            lock (s.Sync)
                            {
                                var t = CurrentTarget(s);
                                Gm(client, "Lantern step=" + s.LanternStep + (t == null ? "" : " piece at " + t.Map + "," + t.X + "," + t.Y
                                    + (client.Player.Map == t.Map ? " distance " + Math.Max(Math.Abs(client.Player.X - t.X), Math.Abs(client.Player.Y - t.Y)) : " (other map)")));
                            }
                            break;
                        }
                    case "stage":
                        {
                            int n;
                            if (!int.TryParse(arg, out n) || n < 0 || n > 7) { Gm(client, "@ramadan stage <0-7>"); break; }
                            var s = GetState(client);
                            if (s == null) break;
                            lock (s.Sync)
                            {
                                s.Stage = (QuestStage)n;
                                if (n >= (int)QuestStage.SealChallenges) s.Clues = ClueFlags.All;
                                if (n >= (int)QuestStage.CommunityEvent) s.Seals = SealFlags.All;
                                if (n >= (int)QuestStage.FinalGateUnlocked && s.CaravanRuns == 0) s.CaravanRuns = 1;
                                if (n == 0) { s.Clues = ClueFlags.None; s.Seals = SealFlags.None; }
                                s.Dirty = true;
                            }
                            SaveState(s);
                            Gm(client, "Your Ramadan stage is now " + (QuestStage)n + " (test command, logged).");
                            break;
                        }
                    case "tokens":
                        {
                            int n;
                            if (!int.TryParse(arg, out n) || n == 0 || Math.Abs(n) > 100000) { Gm(client, "@ramadan tokens <n>  (n may be negative)"); break; }
                            var s = GetState(client);
                            if (s == null) break;
                            lock (s.Sync)
                            {
                                if (n > 0) RamadanRules.AddTokens(s, n);
                                else { s.Tokens = Math.Max(0, s.Tokens + n); s.Dirty = true; }
                            }
                            SaveState(s);
                            Gm(client, "Crescent Tokens: " + s.Tokens);
                            break;
                        }
                    default:
                        Gm(client, GmHelp);
                        break;
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); Gm(client, "error: " + e.Message); }
        }

        static void Gm(Client.GameClient c, string text)
        {
            if (text != null && text.Length > 240) text = text.Substring(0, 237) + "...";
            try { c.SendSysMesage(text, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { }
        }
    }
}
