using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conquer6609.Game.MsgTournaments
{
    public class GuildWarScore
    {
        public SafeDictionary<uint, Winner> GuildWarScor = new SafeDictionary<uint, Winner>();
        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures;
        public static ushort MapID = 8800;//8800
        public ProcesType Proces;
        public class Winner
        {
            public uint GuildID;
            public ulong Score;
            public string GuildName;
        }
        public GuildWarScore()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
        }

        public void Load()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 8447));
        }
        public void CheckUP()
        {
            DateTime Now = DateTime.Now;
            // 1 s tick: push the pending scoreboard (UpdateScore only marks it dirty).
            FlushScores();

            if (Proces == ProcesType.Dead && Now.Hour == 18 && Now.Minute == 00)
                this.Start();

            if (Proces == ProcesType.Alive)
            {

                if (Now.Hour == 18 && Now.Minute == 30)
                    this.End();
            }

        }


        public void Start()
        {
            Proces = ProcesType.Alive;
            GuildWarScor = new SafeDictionary<uint, Winner>();
            MsgSchedules.SendInvitation("GuildWarScore has begun! Would you like to join?", "En", 230, 233, 1002, 0, 60);

        }
        public void Reset(ServerSockets.Packet stream)
        {

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
            var users = Database.Server.GamePoll.Values.Where(u => u != null && u.Player.Map == MapID && Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
            if (users != null)
            {
                foreach (var user in users)
                {
                    MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Pole.UID, 1);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Pole.Mesh);
                    stream = upd.GetArray(stream);
                    user.Send(stream);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Pole.HitPoints);
                    stream = upd.GetArray(stream);
                    user.Send(stream);
                    if ((Role.SobNpc.StaticMesh)Pole.Mesh == Role.SobNpc.StaticMesh.Pole)
                        user.Send(Pole.GetArray(stream, false));
                }
            }
        }
        public void FinishRound(ServerSockets.Packet stream)
        {
            ScoresDirty = false;
            SortScores();
            var RoundOwner = MsgSchedules.GuildWarScore.GuildWarScor.Values.OrderByDescending(i => i.Score).FirstOrDefault();
            if (RoundOwner != null)
            {
                Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.GuildName;
                MsgSchedules.SendSysMesage("The Round Ownered by guild " + RoundOwner.GuildName + ".", MsgServer.MsgMessage.ChatMode.Center);
            }
            Reset(stream);
        }
        public void End()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;
                var RoundOwner = MsgSchedules.GuildWarScore.GuildWarScor.Values.OrderByDescending(i => i.Score).FirstOrDefault();
                MsgSchedules.SendSysMesage(RoundOwner == null ? "GuildWarScore has ended and there is no winner" : "Guild " + RoundOwner.GuildName + ", He is the winner of GuildWarScore.", MsgServer.MsgMessage.ChatMode.Center);

            }
        }
        public void UpdateScore(ServerSockets.Packet stream, uint Score, Role.Instance.Guild Guild)
        {
            // Guildless attackers score nothing (previously a NullReferenceException on every hit).
            if (Guild != null)
            {
                if (!GuildWarScor.ContainsKey(Guild.Info.GuildID))
                    GuildWarScor.Add(Guild.Info.GuildID, new Winner() { GuildName = Guild.GuildName, GuildID = Guild.Info.GuildID, Score = Score });
                else
                    GuildWarScor[Guild.Info.GuildID].Score += Score;

                if (MsgSchedules.ScoreboardThrottle) ScoresDirty = true; else SortScores();// [Performance] ScoreboardThrottle (default 0 = original: every hit)
            }

            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 1)
            {
                FinishRound(stream);
                return;
            }
        }
        // Scoreboard changes are broadcast at most once per second (from CheckUP) instead of on every hit.
        private volatile bool ScoresDirty;
        public void FlushScores()
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
        private void SortScores()
        {
            if (Proces != ProcesType.Dead)
            {
                var Array = GuildWarScor.Values.ToArray();
                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.GuildName + " (" + element.Score + ")"
                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

                        SendMapPacket(msg.GetArray(rec.GetStream()));
                    }
                    if (x == 4)
                        break;
                }
            }
        }
        public void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.ServerMaps[MapID].Values)
                client.Send(packet);
        }
    }
}
