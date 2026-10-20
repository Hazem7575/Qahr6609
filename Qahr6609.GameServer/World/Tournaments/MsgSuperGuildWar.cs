using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgSuperGuildWar
    {
        public const ushort MapID = 3868;

        public bool SendInvitation = false;
        public enum FurnituresType : byte
        {
            CastleLeftGate = 1,
            CastleRightGate = 2,
            LeftGate = 3,
            CenterGate = 4,
            RightGate = 5,
            Pole = 6
        }


        public class GuildWarScrore
        {
            public const int ConquerPointsReward = 25000;

            public uint GuildID;
            public string Name;
            public uint Score;

            //for reward
            public int LeaderReward = 0;
            public int DeputiLeaderReward = 0;
        }

        public DateTime StampRound = new DateTime();
        public DateTime StampShuffleScore = new DateTime();
        public ProcesType Proces { get; set; }
        public Dictionary<FurnituresType, Role.SobNpc> Furnitures { get; set; }
        public ConcurrentDictionary<uint, GuildWarScrore> ScoreList;
        public GuildWarScrore Winner;

        public DateTime StampProtect = new DateTime();



        public List<uint> RewardLeader = new List<uint>();
        public List<uint> RewardDeputiLeader = new List<uint>();

        public MsgSuperGuildWar()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<FurnituresType, Role.SobNpc>();
            ScoreList = new ConcurrentDictionary<uint, GuildWarScrore>();
            Winner = new GuildWarScrore() { Name = "None", Score = 100, GuildID = 0 };
        }

        public unsafe void CreateFurnitures()
        {
            try
            {
                Furnitures.Add(FurnituresType.CastleLeftGate, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516076));
                Furnitures.Add(FurnituresType.CastleRightGate, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516077));
                Furnitures.Add(FurnituresType.LeftGate, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516078));
                Furnitures.Add(FurnituresType.CenterGate, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516079));
                Furnitures.Add(FurnituresType.RightGate, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516080));
                Furnitures.Add(FurnituresType.Pole, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 820));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        internal unsafe void ResetFurnitures(ServerSockets.Packet stream)
        {
            Furnitures[FurnituresType.CastleLeftGate].Mesh = Role.SobNpc.StaticMesh.RightGate;
            Furnitures[FurnituresType.CastleRightGate].Mesh = Role.SobNpc.StaticMesh.RightGate;


            Furnitures[FurnituresType.LeftGate].Mesh = Role.SobNpc.StaticMesh.LeftGate;
            Furnitures[FurnituresType.CenterGate].Mesh = Role.SobNpc.StaticMesh.LeftGate;
            Furnitures[FurnituresType.RightGate].Mesh = Role.SobNpc.StaticMesh.LeftGate;

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == MapID)
                {
                    foreach (var npc in Furnitures.Values)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, npc.X, npc.Y) <= Role.SobNpc.SeedDistrance)
                        {
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, npc.UID, 2);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)npc.Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, npc.HitPoints);
                            stream = upd.GetArray(stream);
                            client.Send(stream);
                            if ((Role.SobNpc.StaticMesh)npc.Mesh == Role.SobNpc.StaticMesh.SuperGuildWarPole)
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
                if (client.Player.Map == MapID)
                {
                    client.Send(packet);
                }
            }
        }
        internal unsafe void CompleteEndGuildWar()
        {
            SendInvitation = true;
            ShuffleGuildScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the Super Guild War "
                 , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the Super Guild War "
    , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
              

            }
            RewardDeputiLeader.Clear();
            RewardLeader.Clear();
            Winner.DeputiLeaderReward = 7;
            Winner.LeaderReward = 1;
        }

        internal unsafe void Start()
        {
            Proces = ProcesType.Alive;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ResetFurnitures(stream);
                ScoreList.Clear();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Super Guild War has began!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                MsgSchedules.SendSysMesage("Super Guild War has began!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
           }
        }
        internal unsafe void CheckFlags()
        {
            if (DateTime.Now > StampProtect)
            {
                Furnitures[FurnituresType.Pole].AddFlag(MsgServer.MsgUpdate.Flags.AzureShield, 30, true, 0, 0, 0);

                StampProtect = DateTime.Now.AddMinutes(Program.GetRandom.Next(2, 5));
            }
            else
            {
                var pole = Furnitures[FurnituresType.Pole];
                foreach (var flag in pole.BitVector.GetFlags())
                {
                    if (flag.Expire(Extensions.Time32.Now))
                    {
                        pole.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                    }
                }
            }
        }

        internal unsafe void FinishRound()
        {
            ShuffleGuildScores(true);
            Furnitures[FurnituresType.Pole].Name = Winner.Name;
            //Proces = ProcesType.Idle;
            ScoreList.Clear();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the Super Guild War "
                   , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the Super Guild War "
                    , MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
               

                ResetFurnitures(stream);
            }
            StampRound = DateTime.Now.AddSeconds(3);
        }
        internal void UpdateScore(Role.Player client, uint Damage)
        {
            if (client.MyGuild == null)
                return;
            if (Proces == ProcesType.Alive)
            {
                if (!ScoreList.ContainsKey(client.GuildID))
                {
                    ScoreList.TryAdd(client.GuildID, new GuildWarScrore() { GuildID = client.MyGuild.Info.GuildID, Name = client.MyGuild.GuildName, Score = Damage });
                }
                else
                {
                    ScoreList[client.MyGuild.Info.GuildID].Score += Damage;
                }

                if (Furnitures[FurnituresType.Pole].HitPoints == 0)
                    FinishRound();
            }
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
        internal void Save()
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\SuperGuildWarInfo.ini");
            if (Proces == ProcesType.Dead)
            {
                write.Write<uint>("Info", "ID", Winner.GuildID);
                write.WriteString("Info", "Name", Winner.Name);
                write.Write<int>("Info", "LeaderReward", Winner.LeaderReward);
                write.Write<int>("Info", "DeputiLeaderReward", Winner.DeputiLeaderReward);

                for (int x = 0; x < RewardLeader.Count; x++)
                    write.Write<uint>("Info", "LeaderTop" + x.ToString() + "", RewardLeader[x]);
                for (int x = 0; x < RewardDeputiLeader.Count; x++)
                {
                    if (x >= RewardDeputiLeader.Count)
                        break;
                    write.Write<uint>("Info", "DeputiTop" + x.ToString() + "", RewardDeputiLeader[x]);
                }
                write.WriteString("Pole", "Name", Winner.Name);
                write.Write<int>("Pole", "HitPoints", Furnitures[FurnituresType.Pole].HitPoints);
            }

        }
       
        internal void Load()
        {
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\SuperGuildWarInfo.ini");
            Winner.GuildID = reader.ReadUInt32("Info", "ID", 0);
            Winner.Name = reader.ReadString("Info", "Name", "None");
            Winner.LeaderReward = reader.ReadInt32("Info", "LeaderReward", 0);
            Winner.DeputiLeaderReward = reader.ReadInt32("Info", "DeputiLeaderReward", 0);

            RewardLeader.Add(reader.ReadUInt32("Info", "LeaderTop0", 0));
            for (int x = 0; x < 7; x++)
            {
                RewardDeputiLeader.Add(reader.ReadUInt32("Info", "DeputiTop" + x.ToString() + "", 0));
            }

            Furnitures[FurnituresType.Pole].Name = reader.ReadString("Pole", "Name", "None");
            Furnitures[FurnituresType.Pole].HitPoints = reader.ReadInt32("Pole", "HitPoints", 0);
        }
    }
}
