using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgNinjaClass
    {
        public static SafeDictionary<uint, Info> NobilityPoleList = new SafeDictionary<uint, Info>();

        public static Info RoundOwner;

        public static ProcesType Proces = ProcesType.Dead;

        public static Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
 
        public static ushort MapID = 2071;

        public class Info
        {
            public uint UID;
            public uint Score;
            public string Name;
        }

        public static void Load()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 22805));
        }

        public static void Start()
        {
            RoundOwner = null;
            NobilityPoleList = new SafeDictionary<uint, Info>();
            Proces = ProcesType.Alive;
        }

        public static void End()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;
                NobilityPoleList.Clear();
            }
        }

        public static void CheckUP()
        {
            DateTime Now = DateTime.Now;

            if (Proces == ProcesType.Dead && (Now.Hour == 19 || Now.Hour == 09) && Now.Minute == 00 && Now.Second == 10)
                Start();

            if (Proces == ProcesType.Alive)
            {

                if (Now.Hour == 19 || Now.Hour == 09 && Now.Minute == 30 && Now.Second == 00)
                    End();
            }
        }

        public static void Reset(ServerSockets.Packet stream)
        {
            NobilityPoleList = new SafeDictionary<uint, Info>();

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
            var users = Database.Server.GamePoll.Values.Where(u => Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
            if (users != null)
            {
                foreach (var user in users)
                {
                    MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Pole.UID, 2);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Pole.Mesh);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Pole.HitPoints);
                    stream = upd.GetArray(stream);
                    user.Send(stream);
                    if ((Role.SobNpc.StaticMesh)Pole.Mesh == Role.SobNpc.StaticMesh.Pole)
                        user.Send(Pole.GetArray(stream, false));
                }
            }
        }
        public static void FinishRound(ServerSockets.Packet stream)
        {
            ScoresDirty = false;
            SortScores(true);
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.Name;
            MsgSchedules.SendSysMesage("The [ Ninja ] Round Ownered by " + RoundOwner.Name + ".", MsgServer.MsgMessage.ChatMode.Center);
            Reset(stream);
        }
        public static void UpdateScore(ServerSockets.Packet stream, uint Score, Role.Player Player)
        {
            if (!NobilityPoleList.ContainsKey(Player.UID))
                NobilityPoleList.Add(Player.UID, new Info() { Name = Player.Name, UID = Player.UID, Score = Score });
            else
                NobilityPoleList[Player.UID].Score += Score;

            if (MsgSchedules.ScoreboardThrottle) ScoresDirty = true; else SortScores();// [Performance] ScoreboardThrottle (default 0 = original: every hit)

            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 1)
            {
                FinishRound(stream);
                return;
            }
        }
        // Scoreboard changes are broadcast at most once per second (from the 1 s schedule tick via FlushScores)
        // instead of on every hit. The final board of a round is still sent immediately by FinishRound.
        private static volatile bool ScoresDirty;
        public static void FlushScores()
        {
            if (!ScoresDirty)
                return;
            ScoresDirty = false;
            try
            {
                SortScores();
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        private static void SortScores(bool getwinner = false)
        {
            if (Proces != ProcesType.Dead)
            {
                var Array = NobilityPoleList.Values.ToArray();
                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];
                    if (x == 0 && getwinner)
                    {
                        RoundOwner = element;
                    }
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score + ")"
                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

                        SendMapPacket(msg.GetArray(rec.GetStream()));
                    }
                    if (x == 4)
                        break;
                }
            }
        }
        public static void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.ServerMaps[MapID].Values)
                client.Send(packet);
        }
    }
}
