using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgSteedRace
    {
        public class UsableRacePotion
        {
            public MsgRacePotion.RaceItemType Type;
            public int Count;
        }

        public enum Maps : uint
        {
            MarketRace = 1950, // Start: 88, 149 End: 420, 431
            IceRace = 2064, //Start: 175,250 End: 200,170
            DungeonRace = 2063, //Start: 450, 520 End: 690,450
            LavaRace = 2062, // Start: 150,350 End:320,160
            IslandRace = 2061, // Start: 60,400 End: 870,830

        }

        public bool InSteedRace(uint mapid)
        {
            return mapid == (ushort)Maps.MarketRace || mapid == (ushort)Maps.IceRace || mapid == (ushort)Maps.DungeonRace || mapid == (ushort)Maps.LavaRace || mapid == (ushort)Maps.IslandRace;
        }
        public uint[][] RaceSettings = new[]
        {
            new uint[]{ (uint)Maps.MarketRace, 88, 149, 
                420, 431, 4,  
                65, 174, 621,
                123, 243, 60, 
                214, 334, 70, 
                346, 459, 100},
            new uint[]{ (uint)Maps.IceRace, 175, 250, 
                200, 153, 6, //255, 535*
                154, 267, 621,
                146, 392, 70,
                283, 351, 100,
                295, 079, 100},
            new uint[]{ (uint)Maps.IslandRace, 60, 400, 
                899, 816, 10,
                96, 392, 621,
                220, 234, 200,
                472, 160, 200,
                777, 464, 300},
            new uint[]{ (uint)Maps.DungeonRace, 450, 520,
                682, 484 , 10,
                435, 559, 621,
                471, 759, 200,
                714, 598, 250,
                489, 679, 20},
            new uint[]{ (uint)Maps.LavaRace, 150, 350,
                330, 170, 6,
                101, 397, 623,
                327, 553, 100,
                526, 477, 200,
                283, 275, 100}
        };
        public static ushort MAPID = 1950;
        public static uint[] Settings;
        public static uint RaceRecord;

        private Role.GameMap Map;
        private bool isOn, InvitationsOut, InvitationsExpired, FiveSecondsLeft, GateOpen;
        private DateTime InvitationsSentOut, InvitationsExpireDate, Last5Seconds, GateOpened;

        // Official (client Activity.ini "Horse Racing"): Monday-Friday, enter 14:55-15:25, race 15:00-15:30.
        // Invitations go out at 14:55 and the gate opens 5 minutes later (15:00).
        public const int InvitationLeadMinutes = 5;
        public int SecondsLeftUntilStart { get { return (int)(InvitationsSentOut.AddMinutes(InvitationLeadMinutes) - DateTime.Now).TotalSeconds; } }
        // Activity.ini id 1: Enter 14:55~15:25, race 15:00~15:30 -> late riders may still join until 15:25.
        public const int LateJoinMinutes = 25;
        public bool CanJoin { get { return isOn && (!GateOpen || DateTime.Now < GateOpened.AddMinutes(LateJoinMinutes)); } }
        public bool IsOn { get { return isOn; } }
        private Role.SobNpc Gate;
        private ushort GateSetX, GateSetY;
        public ushort GateX { get { return GateSetX; } }
        public ushort GateY { get { return GateSetY; } }

        private int Records;// finishing rank counter (Interlocked)
        // riders who signed up for / already crossed the finish line of this race: a finisher can't sign up again for a second prize
        private readonly System.Collections.Concurrent.ConcurrentDictionary<uint, bool> Joined = new System.Collections.Concurrent.ConcurrentDictionary<uint, bool>();
        private readonly System.Collections.Concurrent.ConcurrentDictionary<uint, bool> Finished = new System.Collections.Concurrent.ConcurrentDictionary<uint, bool>();

        // Prizes for EVERY rider who crosses the finish line, by finishing rank.
        // Client texts: Activity.ini [t1] "Champion Points and Horse Racing Points"; Questinfo.ini "Horse Racing [Daily]":
        // "Join the race to win 100 Champion Points and Horse Racing Points"; the finish window (STR_RIDE_MATCH_RESULT)
        // shows "Prize: %d Pts" = the Horse Racing Points (RacePoints, spent in the Steed Envoy's Race Point shop).
        // Optional shell.ini [HorseRace]: RacePoints=<1st>,<2nd>,...  RacePointsOthers=<every rank after the list>
        //                                  ChampionPoints=<1st>,...    ChampionPointsOthers=<every rank after the list>
        public const string DefaultRacePoints = "6000,5000,4000,3500,3000,2500,2000,2000,2000,2000";
        public const int DefaultRacePointsOthers = 1000;
        public const string DefaultChampionPoints = "100";// official: 100 for every finisher (the daily Champion Point cap still applies)
        public const int DefaultChampionPointsOthers = 100;
        public static uint RacePointsFor(int rank) { return RankPrize("RacePoints", DefaultRacePoints, "RacePointsOthers", DefaultRacePointsOthers, rank); }
        public static uint ChampionPointsFor(int rank) { return RankPrize("ChampionPoints", DefaultChampionPoints, "ChampionPointsOthers", DefaultChampionPointsOthers, rank); }
        static uint RankPrize(string listKey, string list, string othersKey, int others, int rank)
        {
            string[] parts = Framework.Settings.String("HorseRace", listKey, list).Split(new[] { ',', ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);
            uint value;
            if (rank >= 1 && rank <= parts.Length && uint.TryParse(parts[rank - 1], out value))
                return value;
            return (uint)Math.Max(0, Framework.Settings.Int("HorseRace", othersKey, others));
        }

        public void Create()
        {
            while (true)
            {
                MyConsole.WriteLine("SteedRace Has Start!");
                int rand = Program.GetRandom.Next(RaceSettings.Length);
                if (Database.Server.ServerMaps.ContainsKey((ushort)RaceSettings[rand][0]))
                {
                    Create(RaceSettings[rand][0]);
                    break;
                }
            }
        }
        public void Create(uint mapId)
        {
            int index = -1;
            for (int i = 0; i < RaceSettings.Length; i++)
            {
                if (RaceSettings[i][0] == mapId)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1) return;
            Settings = RaceSettings[index];
            MAPID = (ushort)Settings[0];
           
            Map = Database.Server.ServerMaps[MAPID];

            var array = Map.View.GetAllMapRoles(Role.MapObjectType.StaticRole);
            foreach (var obj in array)
            {
                Map.RemoveStaticRole(obj);
            }

            Map.View.ClearMap(Role.MapObjectType.SobNpc);

            Gate = new Role.SobNpc();
            Gate.UID = 19501;
            Gate.ObjType = Role.MapObjectType.SobNpc;
            Gate.Map = MAPID;
            Gate.X = (ushort)Settings[6];
            Gate.Y = (ushort)Settings[7];
            Gate.Mesh =  (Role.SobNpc.StaticMesh)Settings[8];
            Gate.Name = " ";
            Gate.Type = Role.Flags.NpcType.Furniture;
            Gate.Sort = 1;
            GateSetX = Gate.X; GateSetY = Gate.Y;
            Map.View.EnterMap<Role.IMapObj>(Gate);
            Map.SetFlagNpc(Gate.X, Gate.Y);

            Init();
        }
        private void Init()
        {
            isOn = InvitationsOut = InvitationsExpired = GateOpen = FiveSecondsLeft = false;
            GeneratePotions();
            Records = 0;
            Joined.Clear();
            Finished.Clear();
        }

        private void GeneratePotions()
        {
            uint count = 100;
            ushort x, y;
            Tuple<ushort, ushort, int>[] limits = new[]
            {
                new  Tuple<ushort,ushort,int>( (ushort)Settings[9], (ushort)Settings[10], (int)Settings[11] ),
                new  Tuple<ushort,ushort,int>( (ushort)Settings[12], (ushort)Settings[13], (int)Settings[14] ),
                new  Tuple<ushort,ushort,int>( (ushort)Settings[15], (ushort)Settings[16], (int)Settings[17] )
            };

            while (count > 0)
            {
                x = (ushort)Program.GetRandom.Next(0, Map.bounds.Width);// Kernel.Random.Next(0, Map.Floor.Bounds.Width);
                y = (ushort)Program.GetRandom.Next(0, Map.bounds.Height);
                bool valid = false;
                foreach (var range in limits)
                    valid |= (Role.Core.GetDistance(x, y, range.Item1, range.Item2) < range.Item3);
                if (valid)
                {
                    if (Map.IsFlagPresent(x,y, Role.MapFlagType.Valid))
                    {
                        bool v = true;
                     
                        for (int i = 0; i < Role.GameMap.XDir.Length; i++)
                            if (Map.IsFlagPresent(x + Role.GameMap.XDir[i], y + Role.GameMap.YDir[i], Role.MapFlagType.Valid) == false)
                                v = false;
                        if (!v) continue;

                        Role.StaticRole role = new Role.StaticRole(x, y, "");
                        role.Map = MAPID;
                        role.Pick();
                        Map.AddStaticRole(role);
                        count--;
                    }
                }
            }
        }

        public void work(int tim)
        {
            //Horse Racing Monday-Friday: invitations 14:55, gate opens 15:00, race closes 15:30 (client Activity.ini).

            DateTime now = DateTime.Now;
            bool weekday = now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday;
            bool inWindow = weekday && ((now.Hour == 14 && now.Minute >= 55) || (now.Hour == 15 && now.Minute < 30));
            if (inWindow)
            {
                if (!InvitationsOut)
                {
                    if (now.Hour == 14 && now.Minute == 55)
                    {
                        Create();
                        SendInvitations();
                    }
                }
                else if (!InvitationsExpired)
                {
                    if (now >= InvitationsExpireDate)
                    {
                        InvitationsExpired = true;
                        FiveSecondsLeft = false;
                        Last5Seconds = InvitationsSentOut.AddMinutes(InvitationLeadMinutes).AddSeconds(-5);
                    }
                }
                else if (!FiveSecondsLeft)
                {
                    if (now > Last5Seconds)
                    {
                        FiveSecondsLeft = true;
                        SendData(ActionType.BeginSteedRace, uid: 1);
                        Last5Seconds = DateTime.Now.AddSeconds(5);
                    }
                }
                else if (!GateOpen)
                {
                    if (now > Last5Seconds)
                    {
                        OpenGate();
                    }
                }
            }
            else if (weekday && now.Hour == 15 && now.Minute >= 30)
            {
                if (isOn)
                {
                    End();
                }
            }
        }

        private void SendInvitations()
        {
            isOn = true;
            InvitationsOut = true;
            InvitationsExpired = false;
            InvitationsSentOut = DateTime.Now;
            InvitationsExpireDate = InvitationsSentOut.AddMinutes(InvitationLeadMinutes).AddSeconds(-10);
            MsgSchedules.SendInvitation("SteedRace", "RacePoints", 286, 147, 1002, 0, 60, MsgStaticMessage.Messages.HorseRace);

        }

        public unsafe void Join(Client.GameClient client, ServerSockets.Packet stream)
        {
            // No level limit (Activity.ini Level_min=1); the Steed Envoy checks for a steed and the Riding skill.
            // Checked again here: nobody gets into the race map without a steed to ride, or twice into the same race.
            if (!CanJoin || InSteedRace(client.Player.Map))
                return;
            if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Steed) || !client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Riding))
            {
                client.SendSysMesage("You need a steed and the Riding skill to join the Horse Racing.");
                return;
            }
            if (client.Player.OnTransform || client.Player.ContainFlag(MsgUpdate.Flags.Fly))
            {
                client.SendSysMesage("You can't join the Horse Racing while transformed or flying.");
                return;
            }
            if (Finished.ContainsKey(client.Player.UID))
            {
                client.SendSysMesage("You have already finished this race. Come back for the next one.");
                return;
            }
            Joined[client.Player.UID] = true;
            int seconds = SecondsLeftUntilStart;
            if (seconds > 0)
            {

                ActionQuery action = new ActionQuery();
                action.ObjId = client.Player.UID;
                action.Type = ActionType.CountDown;

                action.dwParam = (uint)seconds;

                client.Send(stream.ActionCreate(&action));

            }

            client.Player.AddFlag(MsgUpdate.Flags.Ride, Role.StatusFlagsBigVector32.PermanentFlag, true);

            client.Vigor = client.Status.MaxVigor;

            client.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));

            client.Teleport((ushort)Settings[1], (ushort)Settings[2], MAPID);

            client.Send(stream.RaceRecordCreate(MsgRaceRecord.RaceRecordTypes.BestTime, (int)Map.RecordSteedRace, 1800000, 0, 0, 0));
            client.Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() {  PotionType = MsgRacePotion.RaceItemType.Null, Amount = 1 }));
            client.Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() { PotionType = MsgRacePotion.RaceItemType.Null, Amount = 0 }));

            client.Player.RacePotions = new UsableRacePotion[5];
            if (client.Player.SteedRaceBestTime > 0)
                client.SendSysMesage("Your best Horse Racing time: " + FormatTime(client.Player.SteedRaceBestTime) + ".");
            client.Activeness.IncreaseTask(9);
            client.Activeness.IncreaseTask(21);
            client.Activeness.IncreaseTask(33);
        }
        private unsafe void OpenGate()
        {
            GateOpened = DateTime.Now;
            GateOpen = true;
            Gate.X = 0;
            Gate.Y = 0;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ActionQuery action = new ActionQuery();
                action.ObjId = Gate.UID;
                action.Type = ActionType.RemoveEntity;

                Send(stream.ActionCreate(&action));
            }
        }

        private void Send(ServerSockets.Packet stream)
        {
            foreach (var user in Map.Values)
                user.Send(stream);
        }
        private unsafe void SendData(ActionType ID, uint value = 0, uint uid = 0)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ActionQuery action = new ActionQuery();
                action.ObjId = uid;
                action.Type = ID;
                action.dwParam = value;


                foreach (var player in Map.Values)
                {
                    if (uid == 0)
                    {
                        ActionQuery aaction = new ActionQuery();
                        action.ObjId = player.Player.UID;
                        action.Type = ID;
                        action.dwParam = value;

                        player.Send(stream.ActionCreate(&aaction));
                    }
                    else
                    {
                        player.Send(stream.ActionCreate(&action));
                    }

                }
            }
        }

        private void Status(Client.GameClient client, int rank, int time, int award)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.RaceRecordCreate(MsgRaceRecord.RaceRecordTypes.AddRecord, rank, client.Player.Name, time, award);

                Send(stream.RaceRecordCreate(MsgRaceRecord.RaceRecordTypes.AddRecord, rank, client.Player.Name, time, award));
            }
        }

        private void End()
        {
            InvitationsOut = false;
            Gate.X = GateSetX;
            Gate.Y = GateSetY;
            foreach (var user in Map.Values)
                Exit(user);
            Init();
        }

        public void FinishRace(Client.GameClient client)
        {
            // only a rider who signed up, while the race runs and the gate is open
            if (!isOn || !GateOpen || client.Player.Map != MAPID || !Joined.ContainsKey(client.Player.UID))
                return;
            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, (ushort)Settings[3], (ushort)Settings[4]) > 22)
            {
                return;
            }
            else
            {
                // one prize per rider per race; every finisher is ranked and paid (not only the top 5)
                if (Finished.TryAdd(client.Player.UID, true))
                {
                    int rank = System.Threading.Interlocked.Increment(ref Records);
                    TimeSpan span = DateTime.Now - GateOpened;
                    uint time = (uint)span.TotalMilliseconds;
                    // map record ("Record" in the race window); 0 = no record yet, so the first finisher sets it
                    if (Map.RecordSteedRace == 0 || Map.RecordSteedRace > time)
                    {
                        Map.RecordSteedRace = time;
                        Database.ServerDatabase.LoginQueue.TryEnqueue(Map);// -> UpdateMapRace: maps\<id>.ini race_record
                    }
                    bool personalBest = client.Player.SteedRaceBestTime == 0 || client.Player.SteedRaceBestTime > time;
                    if (personalBest)
                        client.Player.SteedRaceBestTime = time;// saved with the character (Character/SteedRaceBest)

                    uint racePoints = RacePointsFor(rank);
                    uint championPoints = ChampionPointsFor(rank);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.RacePoints += racePoints;
                        client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
                        if (championPoints > 0)
                        {
                            client.Player.AddChampionPoints(championPoints);
                            client.Send(stream.CreateGoldLeaguePoint(new MsgGoldLeaguePoint.GoldLeaguePoint() { Points = client.Player.ChampionPoints, HistoryPoints = client.Player.HistoryChampionPoints }));
                        }

                        Status(client, rank, (int)time, (int)racePoints);
                        client.Send(stream.RaceRecordCreate(MsgRaceRecord.RaceRecordTypes.EndTime, (int)rank, (int)time, (int)racePoints, (int)time, (int)racePoints));
                    }
                    client.SendSysMesage("Horse Racing: No. " + rank + " in " + FormatTime(time) + ". You received " + racePoints + " Horse Racing Points"
                        + (championPoints > 0 ? " and " + championPoints + " Champion Points" : "") + "." + (personalBest ? " New personal best!" : ""));
                }
                Exit(client);
            }
        }
        static string FormatTime(uint ms)
        {
            return (ms / 60000) + ":" + (ms / 1000 % 60).ToString("00") + ":" + (ms % 1000).ToString("000");// as STR_RIDE_MATCH_TIME_FMT
        }
        // Every second (CheckSecounds): nobody stays in a race map unless racing on a steed in the running race.
        public void CheckRider(Client.GameClient client)
        {
            if (client.ProjectManager || !InSteedRace(client.Player.Map))
                return;
            if (!isOn || client.Player.Map != MAPID || !Joined.ContainsKey(client.Player.UID) || client.Equipment.FreeEquip(Role.Flags.ConquerItem.Steed))
            {
                Exit(client);
                return;
            }
            if (!client.Player.ContainFlag(MsgUpdate.Flags.Ride))// knocked off (e.g. Spook / War Cry): back on the steed
                client.Player.AddFlag(MsgUpdate.Flags.Ride, Role.StatusFlagsBigVector32.PermanentFlag, true);
        }
        public unsafe void CheckForRaceItems(Client.GameClient client)
        {

            if (client.Map.IsFlagPresent(client.Player.X, client.Player.Y, Role.MapFlagType.None))
            {
                foreach (var _item in client.Map.View.Roles(Role.MapObjectType.StaticRole, client.Player.X, client.Player.Y))
                {
                    var item = _item as Role.StaticRole;
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, item.X, item.Y) <= 1)
                    {
                        if (item == null) return;
                        if (!item.Viable) return;
                        var type = item.Type; // for super exclamation mark 
                        bool successful = false;
                        if (type == MsgRacePotion.RaceItemType.FrozenTrap && !item.QuestionMark)
                        {
                            if (item.SetBy != client.Player.UID)
                            {
                                client.ApplyRacePotion(type, uint.MaxValue);
                                client.Map.RemoveStaticRole(item);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    ActionQuery action;

                                    action = new ActionQuery()
                                    {
                                        ObjId = item.UID,
                                        Type = ActionType.RemoveEntity
                                    };
                                    client.Send(stream.ActionCreate(&action));
                                }
                                successful = true;
                            }
                        }
                        else
                        {
                            if (client.Player.RacePotions == null) client.Player.RacePotions = new UsableRacePotion[5];
                            for (ushort i = 0; i < client.Player.RacePotions.Length; i++)
                            {
                                var pot = client.Player.RacePotions[i];
                                if (pot == null)
                                {
                                    pot = (client.Player.RacePotions[i] = new UsableRacePotion());
                                    pot.Type = type;
                                    pot.Count = item.Level;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() { PotionType = type, Amount = (ushort)pot.Count, Location = (ushort)(i + 1) }));
                                    }

                                    successful = true;
                                    break;
                                }
                                else
                                {
                                    if (pot.Type == type)
                                    {
                                        pot.Count += item.Level;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            client.Send(stream.CreateRecePotion(new MsgRacePotion.RacePotion() { PotionType = type, Amount = (ushort)pot.Count, Location = (ushort)(i + 1) }));
                                        }
                                        successful = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (successful)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            }

                            client.Map.RemoveStaticRole(item);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                ActionQuery action;

                                action = new ActionQuery()
                                {
                                    ObjId = item.UID,
                                    Type = ActionType.RemoveEntity
                                };
                                client.Send(stream.ActionCreate(&action));
                            }
                            item.Viable = false;

                        }

                        break;
                    }
                }
            }
        }
        public void Exit(Client.GameClient client)
        {
            client.Player.RacePotions = null;// race items don't leave the race
            client.TeleportCallBack();
        }
    }
}
