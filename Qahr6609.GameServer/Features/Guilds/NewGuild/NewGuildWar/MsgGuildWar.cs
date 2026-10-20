using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer.AttackHandler.Algoritms;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgGuildWar
    {
        public bool SendInvitation = false;
        internal ushort[][] StatueCoords = 
        {
          new ushort[] {140 ,134 }
         ,new ushort[] {144 ,124 }
         ,new ushort[] {130 ,138 }
         ,new ushort[] {153 ,124 }
         ,new ushort[] {161 ,124 }
         ,new ushort[] {130 ,147 }
         ,new ushort[] {130 ,155 }
        };

        public class GuildConductor
        {
            public static List<uint> BlockMaps = new List<uint>()
            {
                1038
            };
            public Game.MsgNpc.Npc Npc;
            public uint ToMap;
            public ushort ToX, ToY;

            public override string ToString()
            {
                Database.DBActions.WriteLine Line = new Database.DBActions.WriteLine(',');
                Line.Add(Npc.UID).Add(Npc.X).Add(Npc.Y).Add(Npc.Map).Add(Npc.Mesh).Add((ushort)Npc.NpcType)
                    .Add(ToX).Add(ToY).Add(ToMap);
                return Line.Close();
            }
            internal void Load(string Line, MsgNpc.NpcID UID)
            {
                Npc = Game.MsgNpc.Npc.Create();
                if (Line == "")
                {
                    Npc.UID = (uint)UID;
                    return;
                }
                Database.DBActions.ReadLine Reader = new Database.DBActions.ReadLine(Line, ',');
                Npc.UID = Reader.Read((uint)0);
                Npc.X = Reader.Read((ushort)0);
                Npc.Y = Reader.Read((ushort)0);
                Npc.Map = Reader.Read((ushort)0);
                Npc.Mesh = Reader.Read((ushort)0);
                Npc.NpcType = (Role.Flags.NpcType)Reader.Read((ushort)0);

                ToX = Reader.Read((ushort)0);
                ToY = Reader.Read((ushort)0);
                ToMap = Reader.Read((ushort)0);
            }
            internal static bool ChangeNpcLocation(Role.GameMap map, ref ushort X, ref ushort Y, ref uint Map)
            {
                
                
                return false;
            }

            internal void GetCoords(out ushort x, out ushort y, out uint map)
            {
                if (ToMap != 0 && ToX != 0 && ToY != 0)
                {
                    x = ToX;
                    y = ToY;
                    map = ToMap;
                    return;
                }
                x = 300;
                y = 278;
                map = 1002;
            }
        }
     
        public class DataFlameQuest
        {
            public List<uint> Registred;
            public bool ActiveFlame10;

            public DataFlameQuest()
            {
                Registred = new List<uint>();
                ActiveFlame10 = false;
            }
        }
        public class GuildWarScrore
        {
            public const int ConquerPointsReward = 1000000;

            public uint GuildID;
            public string Name;
            public uint Score;

            //for reward
            public int LeaderReward = 1;
            public int DeputiLeaderReward = 7;
        }

        public List<uint> RewardLeader = new List<uint>();
        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound = new DateTime();
        public DateTime StampShuffleScore = new DateTime();
        private Role.GameMap GuildWarMap;

        public ProcesType Proces { get; set; }

        public DataFlameQuest FlamesQuest;
        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures { get; set; }
        public ConcurrentDictionary<uint, GuildWarScrore> ScoreList;
        public GuildWarScrore Winner;
        public Dictionary<MsgNpc.NpcID, GuildConductor> GuildConductors;

        public bool LeftGateOpen { get { return Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh == Role.SobNpc.StaticMesh.OpenLeftGate; } }
        public bool RightGateOpen { get { return Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh == Role.SobNpc.StaticMesh.OpenRightGate; } }
        public MsgGuildWar()
        {
           FlamesQuest = new DataFlameQuest();
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
            GuildConductors = new Dictionary<MsgNpc.NpcID, GuildConductor>();
            ScoreList = new ConcurrentDictionary<uint, GuildWarScrore>();
            Winner = new GuildWarScrore() { Name = "None", Score = 100, GuildID = 0 };
        }

        public DateTime ClosableLeftGateStamp , ClosableRightGateStamp;

        public unsafe void CreateFurnitures()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.LeftGate, Database.Server.ServerMaps[1038].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc,516074));
            Furnitures.Add(Role.SobNpc.StaticMesh.RightGate, Database.Server.ServerMaps[1038].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc,516075));
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[1038].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc,810));
            this.ClosableLeftGateStamp = DateTime.Now;
            this.ClosableRightGateStamp = DateTime.Now;
        }
        public unsafe bool Bomb(ServerSockets.Packet stream, Client.GameClient client, Role.SobNpc.StaticMesh gate)
        {
            // same rules as attacking a gate: only during the war, never by the guild that holds the area
            if (Proces != ProcesType.Alive || client.Player.MyGuild == null || client.Player.GuildID == Winner.GuildID)
            {
                client.SendSysMesage("The gates can only be blown up by an attacking guild during the Guild War.");
                return false;
            }
            if (Furnitures[gate].HitPoints > 3000000)
            {
                Furnitures[gate].HitPoints -= 2000000;

                MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Furnitures[gate].UID,1);
                stream = upd.Append(stream,MsgServer.MsgUpdate.DataType.Hitpoints, Furnitures[gate].HitPoints);
                Furnitures[gate].SendScrennPacket(upd.GetArray(stream));

                client.Player.Dead(null, client.Player.X, client.Player.Y, client.Player.UID);
                Furnitures[gate].SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, "firemagic");
                Furnitures[gate].SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, "bombarrow");

                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("" + client.Player.Name + " from " + (client.Player.MyGuild != null ? client.Player.MyGuild.GuildName.ToString() : "None".ToString()).ToString() + " detonated the Bomb and killed herself/himself. But the " + gate.ToString() + " was blown up!"
                    , MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                return true;
            }
            return false;
        }
        internal unsafe void ResetFurnitures(ServerSockets.Packet stream)
        {
            Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh = Role.SobNpc.StaticMesh.LeftGate;
            Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh = Role.SobNpc.StaticMesh.RightGate;

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == 1038)
                {
                    foreach (var npc in Furnitures.Values)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, npc.X, npc.Y) <= Role.SobNpc.SeedDistrance)
                        {
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream,npc.UID,2);
                            stream = upd.Append(stream,MsgServer.MsgUpdate.DataType.Mesh, (long)npc.Mesh);
                            stream = upd.Append(stream,MsgServer.MsgUpdate.DataType.Hitpoints, npc.HitPoints);
                            stream = upd.GetArray(stream);
                            client.Send(stream);
                            if ((Role.SobNpc.StaticMesh)npc.Mesh == Role.SobNpc.StaticMesh.Pole)
                                client.Send(npc.GetArray(stream, false));
                        }
                    }
                }
            }
        }
        internal unsafe void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == 1038 || client.Player.Map == 6001)
                {
                    client.Send(packet);
                }
            }
        }
        /// <summary>Day the war last ended (GM end included): the Saturday schedule does not start it again that day.</summary>
        public DateTime EndedOn = DateTime.MinValue;
        public DateTime StartedOn = DateTime.MinValue;
        /// <summary>Started by a GM (@gw start): the Saturday clock does not end it, Flame Lit is open at once.</summary>
        public bool GmStarted = false;

        /// <summary>A disbanded winner guild leaves the Guild Area without owner.</summary>
        internal void ValidateWinner()
        {
            if (Winner == null)
                Winner = new GuildWarScrore() { Name = "None", Score = 0, GuildID = 0 };
            if (Winner.GuildID != 0 && !Role.Instance.Guild.GuildPoll.ContainsKey(Winner.GuildID))
            {
                Winner = new GuildWarScrore() { Name = "None", Score = 0, GuildID = 0, LeaderReward = 0, DeputiLeaderReward = 0 };
                if (Furnitures.ContainsKey(Role.SobNpc.StaticMesh.Pole))
                    Furnitures[Role.SobNpc.StaticMesh.Pole].Name = "None";
            }
        }

        /// <summary>The winner halo (client statuseffect 34 gamemain / 35 gameassistant) is kept only while the player is still
        /// leader / deputy of the current winner guild and has claimed this week's reward.</summary>
        internal void RefreshHalo(Client.GameClient client)
        {
            if (client == null || client.Player == null)
                return;
            bool inWinner = Winner != null && Winner.GuildID != 0 && client.Player.GuildID == Winner.GuildID && client.Player.MyGuild != null;
            bool leader = inWinner && client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader && RewardLeader.Contains(client.Player.UID);
            bool deputy = inWinner && (client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader)
                && RewardDeputiLeader.Contains(client.Player.UID);
            if (leader)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopGuildLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);
            else if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.TopGuildLeader))
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.TopGuildLeader);
            if (deputy)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopDeputyLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);
            else if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.TopDeputyLeader))
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.TopDeputyLeader);
        }

        /// <summary>Client Cn_Res.ini STR_RAND_REBORN_IN_FACTION_BATTLE: "Random revival is available during Guild War."
        /// Same spots the Guild Controller uses to send players in.</summary>
        internal static readonly ushort[][] RevivePoints =
        {
            new ushort[] {326,374}, new ushort[] {299,371}, new ushort[] {293,352}, new ushort[] {262,337}, new ushort[] {285,324},
            new ushort[] {315,289}, new ushort[] {323,268}, new ushort[] {351,282}, new ushort[] {371,305}, new ushort[] {370,326},
            new ushort[] {357,335}, new ushort[] {333,330}, new ushort[] {322,321}
        };
        internal static void RandomPoint(out ushort x, out ushort y)
        {
            var p = RevivePoints[Program.GetRandom.Next(0, RevivePoints.Length)];
            x = p[0];
            y = p[1];
        }

        public string Status()
        {
            Role.SobNpc pole, left, right;
            Furnitures.TryGetValue(Role.SobNpc.StaticMesh.Pole, out pole);
            Furnitures.TryGetValue(Role.SobNpc.StaticMesh.LeftGate, out left);
            Furnitures.TryGetValue(Role.SobNpc.StaticMesh.RightGate, out right);
            var top = ScoreList.Values.OrderByDescending(p => p.Score).Take(5).Select(p => p.Name + "(" + p.Score + ")").ToArray();
            return "GuildWar " + Proces + " holder=" + Winner.Name + "[" + Winner.GuildID + "]"
                + " pole=" + (pole != null ? pole.HitPoints + "/" + pole.MaxHitPoints + " '" + pole.Name + "'" : "?")
                + " left=" + (left != null ? left.HitPoints + (LeftGateOpen ? " open" : " closed") : "?")
                + " right=" + (right != null ? right.HitPoints + (RightGateOpen ? " open" : " closed") : "?")
                + " claims L" + Winner.LeaderReward + "/D" + Winner.DeputiLeaderReward
                + " top=" + string.Join(", ", top);
        }

        internal unsafe void CompleteEndGuildWar()
        {
            SendInvitation = false;
            Proces = ProcesType.Dead;
            GmStarted = false;
            EndedOn = DateTime.Now.Date;
            ScoreList.Clear();
            ValidateWinner();
            // ChatTips0062: "The guild that knocked down guild pole will dominate the guild area when the guild war is over."
            // -> the winner is the guild holding the pole now (set in FinishRound); new week = new claims.
            RewardDeputiLeader.Clear();
            RewardLeader.Clear();
            Winner.DeputiLeaderReward = Winner.GuildID != 0 ? 7 : 0;
            Winner.LeaderReward = Winner.GuildID != 0 ? 1 : 0;
            FlamesQuest.ActiveFlame10 = false;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ResetFurnitures(stream);
                string msg = Winner.GuildID != 0
                    ? "The Guild War is over. Congratulations to " + Winner.Name + ", they have won the Guild War and dominate the Guild Area!"
                    : "The Guild War is over. No guild knocked down the pole.";
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                // clear the right-corner score board of the players still on the map
                SendMapPacket(new MsgServer.MsgMessage("", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner).GetArray(stream));
            }
            foreach (var user in Database.Server.GamePoll.Values)
            {
                try { RefreshHalo(user); }
                catch (Exception e) { MyConsole.SaveException(e); }
            }
            Log("end, winner " + Winner.Name + " [" + Winner.GuildID + "]");
            Database.Server.Save(new Action(Save));
        }

        internal unsafe void Start()
        {
            // callers check Proces == Dead (the old console "startgw" sets Alive before calling Start)
            // a restart during the war resumes it: Flame Lit sign-ups of the same day are kept (one rune per player per week)
            if (StartedOn != DateTime.Now.Date)
                FlamesQuest = new DataFlameQuest();
            FlamesQuest.ActiveFlame10 = false;
            StartedOn = DateTime.Now.Date;
            ValidateWinner();
            Proces = ProcesType.Alive;
            StampShuffleScore = DateTime.Now;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                // client StrRes 10515 "Guild War is about to begin! Will you join it?"; Questinfo [132]: enter through the Guild Controller (TC 222,237)
                MsgSchedules.SendInvitation("GuildWar has begun! Would you like to join?", "En", 228, 240, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.GuildWar);
                ResetFurnitures(stream);
                ScoreList.Clear();

                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("The Guild War has started! Speak with the Guild Controller in Twin City to enter the Guild War map.", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
            }
            Log("start, holder " + Winner.Name + " [" + Winner.GuildID + "]" + (GmStarted ? " (GM)" : ""));
            Database.Server.Save(new Action(Save));
        }

        internal unsafe void FinishRound()
        {
            lock (ScoreList)
            {
                if (Proces != ProcesType.Alive || ScoreList.Count == 0)
                    return;
                ShuffleGuildScores(true);
                ScoreList.Clear();
            }
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = Winner.Name;
            Winner.LeaderReward = 1;
            Winner.DeputiLeaderReward = 7;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(Winner.Name + " knocked down the pole and holds the Guild Area now!"
                   , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(Winner.Name + " knocked down the pole and holds the Guild Area now!"
                    , MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                ResetFurnitures(stream);
            }
            StampRound = DateTime.Now.AddSeconds(3);
            Log("pole knocked down, holder " + Winner.Name + " [" + Winner.GuildID + "] score " + Winner.Score);
            Database.Server.Save(new Action(Save));
        }
        internal void UpdateScore(Role.Player client, uint Damage)
        {
            if (client == null || client.MyGuild == null || Proces != ProcesType.Alive)
                return;
            // only from inside the Guild Area, never for the guild that holds the pole
            if (client.Map != 1038 || client.GuildID == Winner.GuildID)
                return;
            if (!client.Owner.HeroRewards.ContainGoal(607))
                client.Owner.HeroRewards.AddGoal(607);
            uint guildId = client.MyGuild.Info.GuildID;
            lock (ScoreList)
            {
                GuildWarScrore score;
                if (!ScoreList.TryGetValue(guildId, out score))
                    ScoreList.TryAdd(guildId, new GuildWarScrore() { GuildID = guildId, Name = client.MyGuild.GuildName, Score = Damage });
                else
                    score.Score += Damage;
            }
            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                FinishRound();
        }

        internal unsafe void ShuffleGuildScores(bool createWinned = false)
        {
            if (Proces != ProcesType.Dead)
            {
                StampShuffleScore = DateTime.Now.AddSeconds(8);
                var Array = ScoreList.Values.ToArray();
                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];
                    if (x == 0 && createWinned)
                        Winner = element;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score.ToString() + ")"
                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                       
                        SendMapPacket(msg.GetArray(stream));

                    }
                    if (x == 4)
                        break;
                }
            }
        }
        internal bool ValidJump(int Current, out int New, ushort X, ushort Y, Line MovingLine = null)
        {
            if (Role.Core.GetDistance(217, 177, X, Y) <= 3)
            {
                New = 0;
                return true;
            }
            New = 0;
          //  return true;
            if (Role.Core.GetDistance(Furnitures[Role.SobNpc.StaticMesh.LeftGate].X, Furnitures[Role.SobNpc.StaticMesh.LeftGate].Y, X, Y) <= 5)
            {
                if (LeftGateOpen)
                {
                    New = 0;
                    return true;
                }
            }
            if (Role.Core.GetDistance(Furnitures[Role.SobNpc.StaticMesh.RightGate].X, Furnitures[Role.SobNpc.StaticMesh.RightGate].Y, X, Y) <= 5)
            {
                if (RightGateOpen)
                {
                    New = 0;
                    return true;
                }
            }
            New = Current;
            int new_mask = GuildWarMap.FloorType[X, Y];
            if (Current == 3)
            {
                if (new_mask == 0 || new_mask == 9 || new_mask == 13)
                {
                    if (Y == 209 || Y == 208)
                    {
                        if (Role.Core.GetDistance(X, Y, 163, 209) <= 3)
                        {
                            if (LeftGateOpen)
                            {
                                New = new_mask;
                                return true;
                            }
                        }
                    }
                    else if (X == 216)
                    {
                        if (Role.Core.GetDistance(X, Y, 216, 177) <= 4)
                        {
                            if (RightGateOpen)
                            {
                                New = new_mask;
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }
            New = new_mask;
            return true;
        }
        internal bool ValidWalk(int Current, out int New, ushort X, ushort Y, Line MovingLine = null)
        {
            if (Role.Core.GetDistance(217, 177, X, Y) <= 3)
            {
                New = 0;
                return true;
            }
            New = 0;
          //  return true;
            if (Role.Core.GetDistance(Furnitures[Role.SobNpc.StaticMesh.LeftGate].X, Furnitures[Role.SobNpc.StaticMesh.LeftGate].Y, X, Y) <= 5)
            {
                if(LeftGateOpen)
                {
                    New = 0;
                    return true;
                }
            }
            if (Role.Core.GetDistance(Furnitures[Role.SobNpc.StaticMesh.RightGate].X, Furnitures[Role.SobNpc.StaticMesh.RightGate].Y, X, Y) <= 5)
            {
                if (RightGateOpen)
                {
                    New = 0;
                    return true;
                }
            }
            New = Current;
            int new_mask = GuildWarMap.FloorType[X, Y];
            if (Current == 3)
            {
                if (new_mask == 0 || new_mask == 9 || new_mask == 13)
                {
                    if (Y == 209 || Y == 208)
                    {
                        if (Role.Core.GetDistance(X, Y, 163, 209) <= 3)
                        {
                            if (LeftGateOpen)
                            {
                                New = new_mask;
                                return true;
                            }
                        }
                    }
                    else if (X == 216)
                    {
                        if (Role.Core.GetDistance(X, Y, 216, 177) <= 4)
                        {
                            if (RightGateOpen)
                            {
                                New = new_mask;
                                return true;
                            }
                        }
                    }
                 
                    return false;
                }
            }
            New = new_mask;
            return true;
        }
        internal void Save()
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\GuildWarInfo.ini");
            // saved in every state (the holder changes during the war); fixed slots, because a slot that is not
            // rewritten keeps an old UID (an old leader got the halo back after a restart)
            write.Write<uint>("Info", "ID", Winner.GuildID);
            write.WriteString("Info", "Name", Winner.Name);
            write.Write<int>("Info", "LeaderReward", Winner.LeaderReward);
            write.Write<int>("Info", "DeputiLeaderReward", Winner.DeputiLeaderReward);
            var leaders = RewardLeader.ToArray();
            var deputies = RewardDeputiLeader.ToArray();
            for (int x = 0; x < 4; x++)
                write.Write<uint>("Info", "LeaderTop" + x.ToString(), x < leaders.Length ? leaders[x] : 0);
            for (int x = 0; x < 7; x++)
                write.Write<uint>("Info", "DeputiTop" + x.ToString(), x < deputies.Length ? deputies[x] : 0);
            write.WriteString("Info", "EndedOn", EndedOn == DateTime.MinValue ? "" : EndedOn.ToString("yyyy-MM-dd"));
            write.WriteString("Info", "StartedOn", StartedOn == DateTime.MinValue ? "" : StartedOn.ToString("yyyy-MM-dd"));
            write.WriteString("Info", "FlameRegistered", Proces == ProcesType.Dead ? "" : string.Join("|", FlamesQuest.Registred.ToArray()));
            write.WriteString("Pole", "Name", Winner.Name);
            write.Write<int>("Pole", "HitPoints", Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints);

            // same order as Load (GuildConductorN = teleporter 101614 + (N-1)*2); the old TeleGuild1..4 order
            // (101620..101614) swapped the four teleporters on every save/restart
            for (int x = 0; x < 4; x++)
            {
                GuildConductor conductor;
                if (GuildConductors.TryGetValue((MsgNpc.NpcID)(101614 + x * 2), out conductor))
                    write.WriteString("Condutors", "GuildConductor" + (x + 1).ToString(), conductor.ToString());
            }
        }
        internal static void Log(string text)
        {
            // ServerDataBase\GuildWar.log: start/end/round winners and GM actions (small, a few lines a week)
            try
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(Program.ServerConfig.DbLocation, "GuildWar.log"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + text + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }
        internal void Load()
        {
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\GuildWarInfo.ini");
            Winner.GuildID = reader.ReadUInt32("Info", "ID", 0);
            Winner.Name = reader.ReadString("Info", "Name", "None");
            Winner.LeaderReward = reader.ReadInt32("Info", "LeaderReward", 0);
            Winner.DeputiLeaderReward = reader.ReadInt32("Info", "DeputiLeaderReward", 0);

            for (int x = 0; x < 4; x++)
            {
                uint uid = reader.ReadUInt32("Info", "LeaderTop" + x.ToString(), 0);
                if (uid != 0 && !RewardLeader.Contains(uid))
                    RewardLeader.Add(uid);
            }
            for (int x = 0; x < 7; x++)
            {
                uint uid = reader.ReadUInt32("Info", "DeputiTop" + x.ToString(), 0);
                if (uid != 0 && !RewardDeputiLeader.Contains(uid))
                    RewardDeputiLeader.Add(uid);
            }
            DateTime ended;
            if (DateTime.TryParse(reader.ReadString("Info", "EndedOn", ""), out ended))
                EndedOn = ended.Date;
            if (DateTime.TryParse(reader.ReadString("Info", "StartedOn", ""), out ended))
                StartedOn = ended.Date;
            foreach (var part in reader.ReadString("Info", "FlameRegistered", "").Split('|'))
            {
                uint uid;
                if (uint.TryParse(part, out uid) && uid != 0 && !FlamesQuest.Registred.Contains(uid))
                    FlamesQuest.Registred.Add(uid);
            }

            // the pole carries the holder's name; HP is full outside a round (a saved 0 left it dead after a restart)
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = Winner.Name;
            int poleHp = reader.ReadInt32("Pole", "HitPoints", 0);
            var pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
            pole.HitPoints = poleHp > 0 && poleHp <= pole.MaxHitPoints ? poleHp : pole.MaxHitPoints;

            for (int x = 0; x < 4; x++)
            {
                GuildConductor conductor = new GuildConductor();
                conductor.Load(reader.ReadString("Condutors", "GuildConductor" + (x + 1).ToString() + "", ""), (MsgNpc.NpcID)(101614 + x * 2));
                // key = the teleporter UID stored in the line (files written by the old swapped Save), else the slot
                var key = (MsgNpc.NpcID)(101614 + x * 2);
                uint saved = conductor.Npc.UID;
                if (saved >= 101614 && saved <= 101620 && (saved - 101614) % 2 == 0 && !GuildConductors.ContainsKey((MsgNpc.NpcID)saved))
                    key = (MsgNpc.NpcID)saved;
                if (GuildConductors.ContainsKey(key))
                    key = (MsgNpc.NpcID)Enumerable.Range(0, 4).Select(i => (uint)(101614 + i * 2)).First(u => !GuildConductors.ContainsKey((MsgNpc.NpcID)u));
                conductor.Npc.UID = (uint)key;
                GuildConductors.Add(key, conductor);
                if (conductor.Npc.Map != 0)
                {
                    if (Database.Server.ServerMaps.ContainsKey(conductor.Npc.Map))
                        Database.Server.ServerMaps[conductor.Npc.Map].AddNpc(conductor.Npc);
                }
            }
            GuildWarMap = Database.Server.ServerMaps[1038];
        
        }

    }
}
