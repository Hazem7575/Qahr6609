using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgCaptureTheFlag
    {
        public const ushort MapID = 2057,
            AliveTournamentMinutes = 59,
            X2CastleMinutes = 15,
            UpScoreBoardSecounds = 6;

        public class Basse
        {
            public Role.SobNpc Npc;
            public Extensions.SafeDictionary<uint, WarScrore> Scores = new Extensions.SafeDictionary<uint, WarScrore>();
            public uint CapturerID = 0;
            public bool IsX2 = false;

            public class WarScrore
            {
                public uint GuildID;
                public string Name;
                public uint Score;
            }

            internal void UpdateScore(Role.Player client, uint Damage)
            {
                if (client.MyGuild == null)
                    return;
                if (!Scores.ContainsKey(client.GuildID))
                {
                    Scores.Add(client.GuildID, new WarScrore()
                    {
                        GuildID = client.MyGuild.Info.GuildID,
                        Name = client.MyGuild.GuildName,
                        Score = Damage
                    });
                }
                else
                {
                    Scores[client.MyGuild.Info.GuildID].Score += Damage;
                }
            }
        }

        public Role.GameMap Map;
        public uint X2Castle = 0;


        public ProcesType Proces;
        public Extensions.SafeDictionary<uint, Basse> Bases;
        public DateTime UpdateStampScore = new DateTime();
        public DateTime SendX2LoctionStamp = new DateTime();
        public DateTime TournamentStamp = new DateTime();

        public MsgCaptureTheFlag()
        {
            Proces = ProcesType.Dead;
            Bases = new Extensions.SafeDictionary<uint, Basse>();
            Program.FreePkMap.Add(MapID);
        }

        public System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Guild> RegistredGuilds = new System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Guild>();
        public void Start()
        {
            if (Proces == ProcesType.Dead)
            {
                TournamentStamp = DateTime.Now;

                Bases.Clear();
                CreateBases();
                foreach (var guild in Role.Instance.Guild.GuildPoll.Values)
                {
                    guild.ClaimCtfReward = 0;
                    guild.CTF_Exploits = 0;
                    guild.CTF_Rank = 0;

                    foreach (var user in guild.Members.Values)
                    {
                        user.CTF_Exploits = 0;
                        user.RewardConquerPoints = 0;
                        user.RewardMoney = 0;
                        user.CTF_Claimed = 0;
                    }

                }
                RegistredGuilds = new System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Guild>();
                GenerateX2Castle();

                Proces = ProcesType.Alive;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var user in Database.Server.GamePoll.Values)
                        user.Player.MessageBox("", new Action<Client.GameClient>(p =>
                        {
                            p.Teleport(226, 238, 1002);
                        }), null
                        , 60, MsgServer.MsgStaticMessage.Messages.CapturetheFlag);
                }
            }
        }
        public bool CheckStarted()
        {
            return Proces == ProcesType.Alive;
        }
        public void CheckFinish()
        {
            if (Proces == ProcesType.Alive && DateTime.Now > TournamentStamp.AddMinutes(AliveTournamentMinutes))
            {
                Proces = ProcesType.Dead;

                var array = Role.Instance.Guild.GuildPoll.Values.Where(p => p.CTF_Exploits != 0).ToArray();
                var ranks = array.OrderByDescending(p => p.CTF_Exploits).ToArray();
                for (int x = 0; x < Math.Min(9, ranks.Length); x++)
                    ranks[x].CTF_Rank = (byte)(x + 1);
                foreach (var guild in ranks)
                {
                    if (guild.CTF_Rank >= 9)
                    {
                        continue;
                    }
                    var array_members = guild.Members.Values.Where(p => p.CTF_Exploits != 0).ToArray();
                    var Ranks_members = array_members.OrderByDescending(p => p.CTF_Exploits).ToArray();
                    Client.GameClient fighter = null;
                    for (int x = 0; x < Ranks_members.Length; x++)
                    {
                        if (Database.Server.GamePoll.TryGetValue(Ranks_members[x].UID, out fighter))
                        {
                            if (fighter != null)
                            {
                                var rank = CalculateMemberRewardCTF((uint)(x + 1), guild, fighter);
                                Ranks_members[x].RewardConquerPoints = rank[0];
                                Ranks_members[x].RewardMoney = rank[1];


                                fighter.Player.Money += rank[1];
                                fighter.Player.ConquerPoints += rank[0];
                                fighter.SendSysMesage("Congratulations, you won " + Ranks_members[x].RewardConquerPoints + " cps and " + Ranks_members[x].RewardMoney + " silver from CTF Tournment.");
                            }
                        }
                    }
                    guild.CTF_Next_ConquerPoints = 0;
                    guild.CTF_Next_Money = 0;
                }

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Capture The Flag has finished.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                }

                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Map == MapID)
                        user.Teleport(300, 278, 1002);
                }
            }
        }
        private uint[] CalculateMemberRewardCTF(uint Rank, Role.Instance.Guild guild, Client.GameClient user)
        {
            uint[] rew = new uint[2];
            //cps
            uint OnePointsCps = guild.CTF_Next_ConquerPoints / guild.CTF_Exploits;
            uint TotalCps = user.Player.MyGuild.Members[user.Player.UID].CTF_Exploits * OnePointsCps;
            //Money
            uint OnePointsMoney = guild.CTF_Next_Money / guild.CTF_Exploits;
            uint TotalMoney = user.Player.MyGuild.Members[user.Player.UID].CTF_Exploits * OnePointsMoney;
            rew[0] = TotalCps;
            rew[1] = TotalMoney;
            return rew;
        }
        public void GenerateX2Castle()
        {
            int random = Program.GetRandom.Next(0, Bases.Count);
            var basse = Bases.Values.ToArray()[X2Castle];
            basse.IsX2 = false;
            X2Castle = (uint)random;
            UpdateMapX2Location();
        }
        public void UpdateMapX2Location()
        {
            var basse = Bases.Values.ToArray()[X2Castle];
            basse.IsX2 = true;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.X2Location, X2Castle + 1, 1);
                stream.AddX2LocationCaptureTheFlagUpdate(basse.Npc.X, basse.Npc.Y);
                stream.CaptureTheFlagUpdateFinalize();
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                    {
                        user.Send(stream);
                    }
                }
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Proces == ProcesType.Alive)
            {
                if (!RegistredGuilds.ContainsKey(user.Player.GuildID))
                {
                    RegistredGuilds.TryAdd(user.Player.GuildID, user.Player.MyGuild);
                }


                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.InitializeCTF, 0, 0);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);


                var basse = Bases.Values.ToArray()[X2Castle];
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.X2Location, X2Castle + 1, 1);
                stream.AddX2LocationCaptureTheFlagUpdate(basse.Npc.X, basse.Npc.Y);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);

                user.Teleport(478, 373, MapID);
                if (!user.HeroRewards.ContainGoal(609))
                    user.HeroRewards.AddGoal(609);
                return true;
            }
            return false;
        }
        public void CreateBases()
        {
            Map = Database.Server.ServerMaps[MapID];

            foreach (var npc in Map.View.GetAllMapRoles(Role.MapObjectType.SobNpc))
                Bases.Add(npc.UID, new Basse() { Npc = npc as Role.SobNpc });

            SpawnFlags();
        }
        public void CheckUpX2()
        {
            if (Proces == ProcesType.Alive)
            {
                if (DateTime.Now > SendX2LoctionStamp.AddMinutes(X2CastleMinutes))
                {
                    GenerateX2Castle();

                    SendX2LoctionStamp = DateTime.Now;
                }
            }
        }
        public void RandomReviveCTF(Client.GameClient user)
        {
            if (user.Player.Map != Map.ID || Proces != ProcesType.Alive)
            {
                user.SendSysMesage("only avaliable during CTF contest.");
                return;
            }
            
            using (var rect = new ServerSockets.RecycledPacket())
            {
                var stream = rect.GetStream();
                if (user.Player.ContainFlag(MsgUpdate.Flags.SoulShackle))
                {
                    user.Player.RemoveFlag(MsgUpdate.Flags.SoulShackle);
                    user.Player.SendUpdate(stream, MsgUpdate.Flags.SoulShackle, 1, 0, 0, MsgUpdate.DataType.SoulShackle, false);
                }
                user.Player.ProtectAttack(5 * 1000);//5 secounds

                user.Player.HitPoints = (int)user.Status.MaxHitpoints;
                user.Player.ClearFlags();
                user.Player.TransformationID = 0;


                if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar))
                {
                    user.Player.XPCount = 0;
                }
                user.Player.SendUpdate(stream, user.Player.XPCount, MsgUpdate.DataType.XPCircle);

                user.Player.Stamina = 100;
                user.Player.SendUpdate(stream, user.Player.Stamina, MsgUpdate.DataType.Stamina);
                ushort x = 0; ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                while (!InMainCastle(x, y))
                {
                    Map.GetRandCoord(ref x, ref y);
                }
                user.Teleport(x, y, Map.ID);
                user.Player.Send(stream.MapStatusCreate(Map.ID, Map.ID, (uint)user.Map.TypeStatus));
                user.Player.View.SendView(user.Player.GetArray(stream, false), false);
            }
        }
        public void SpawnFlags()
        {
            for (int i = 40 - Map.View.GetAllMapRolesCount(Role.MapObjectType.Npc, x => x.UID == (uint)(x.X * 1000 + x.Y)); i > 0; i--)
            {
                ushort x = 0; ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                if (!InMainCastle(x, y))
                {
                    var npc = Game.MsgNpc.Npc.Create();
                    npc.UID = (uint)(x * 1000 + y);
                    npc.Mesh = 8910;
                    npc.Map = MapID;
                    npc.X = x;
                    npc.Y = y;
                    npc.NpcType = Role.Flags.NpcType.Flag;
                    Map.AddNpc(npc);
                }
            }
        }
        public void UpdateMapScore()
        {
            if (Proces == ProcesType.Alive)
            {
                if (DateTime.Now > UpdateStampScore.AddSeconds(UpScoreBoardSecounds))
                {
                    SendUpdateBoardScore();
                    UpdateStampScore = DateTime.Now;
                }
            }
        }
        public void SendUpdateBoardScore()
        {

            var array = RegistredGuilds.Values.Where(p => p.CTF_Exploits != 0).ToArray();
            var rank = array.OrderByDescending(p => p.CTF_Exploits).ToArray();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.InitializeCTF, 0, 0);
                stream.CaptureTheFlagUpdateFinalize();
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                        user.Send(stream);
                }



                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.ScoreUpdate, 2, (uint)Math.Min(rank.Length, 5));

                for (uint x = 0; x < rank.Length; x++)
                {
                    if (x == 4)
                        break;
                    var element = rank[x];

                    stream.AddItemCaptureTheFlagUpdate(x, element.CTF_Exploits, element.GuildName);
                }

                stream.CaptureTheFlagUpdateFinalize();
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                        user.Send(stream);
                }

                //send base score.
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                    {
                        Basse flag_base;
                        if (TryGetBase(user, out flag_base))
                        {
                            var array_scorebasse = flag_base.Scores.Values.OrderByDescending(p => p.Score).ToArray();
                            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.ScoreBase, 0, (uint)Math.Min(array_scorebasse.Length, 5));
                            for (uint x = 0; x < array_scorebasse.Length; x++)
                            {
                                if (x == 4)
                                    break;
                                var element = array_scorebasse[x];
                                stream.AddItemCaptureTheFlagUpdate(x, element.Score, element.Name);
                            }
                            stream.CaptureTheFlagUpdateFinalize();
                            user.Send(stream);
                        }
                    }
                }
            }

        }
        public bool TryGetBase(Client.GameClient user, out Basse bas)
        {
            if (user.Player.Map == MapID && user.Player.DynamicID == 0)
            {
                foreach (var flag_base in Bases.Values)
                {
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, flag_base.Npc.X, flag_base.Npc.Y) <= 11)
                    {
                        bas = flag_base;
                        return true;
                    }
                }
            }
            bas = null;
            return false;
        }
        public void UpdateFlagScore(Role.Player client, Role.SobNpc Attacked, uint Damage, ServerSockets.Packet stream)
        {
            if (Proces != ProcesType.Alive)
                return;
            if (client.MyGuild == null)
                return;
            Basse Bas;
            if (Bases.TryGetValue(Attacked.UID, out Bas))
            {
                Bas.UpdateScore(client, Damage);

                if (Bas.Npc.HitPoints == 0)
                {

                    var array = Bas.Scores.Values.OrderByDescending(p => p.Score).ToArray();
                    var GuildWinner = array.First();

                    Bas.CapturerID = GuildWinner.GuildID;

                    Bas.Scores.Clear();
                    Bas.Npc.HitPoints = Bas.Npc.MaxHitPoints;
                    Bas.Npc.Name = GuildWinner.Name;
                    

                    foreach (var user in Database.Server.GamePoll.Values)
                    {
                        if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                        {
                            if (Role.Core.GetDistance(user.Player.X, user.Player.Y, Bas.Npc.X, Bas.Npc.Y) <= 9)
                            {
                                MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Bas.Npc.UID, 2);
                                stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Bas.Npc.Mesh);
                                stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Bas.Npc.HitPoints);
                                stream = upd.GetArray(stream);
                                client.Send(stream);
                                client.Send(Bas.Npc.GetArray(stream, false));
                            }
                        }
                    }

                    stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.OccupiedBase, (byte)(Bas.Npc.UID % 10), 0);
                    stream.CaptureTheFlagUpdateFinalize();
                    SendMapPacket(stream);
                }
            }
        }
        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                    user.Send(stream);
            }
        }
        public bool Attackable(Role.Player user)
        {
            return !InMainCastle(user.X, user.Y);
        }
        public bool InMainCastle(ushort X, ushort Y)
        {
            return Role.Core.GetDistance(X, Y, 482, 367) < 32;
        }
        public void PlantTheFlag(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Proces != ProcesType.Alive)
                return;
            if (user.Player.Map == MapID)
            {
                if (user.Player.MyGuild == null)
                    return;

                if (user.Player.ContainFlag(MsgUpdate.Flags.CTF_Flag))
                {
                    Basse flag_base;
                    if (TryGetBase(user, out flag_base))
                    {
                        if (flag_base.CapturerID == user.Player.GuildID)
                        {
                            user.Player.RemoveFlag(MsgUpdate.Flags.CTF_Flag);

                            uint exploits = (uint)(user.Player.Level / 2);

                            if (flag_base.IsX2)
                                exploits *= 2;

                            user.Player.MyGuild.CTF_Exploits += exploits;
                            user.Player.MyGuildMember.CTF_Exploits += exploits;


                            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateTimer, 0, user.Player.UID);
                            stream.CaptureTheFlagUpdateFinalize();
                            user.Send(stream);

                            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateEffect, user.Player.UID);
                            stream.CaptureTheFlagUpdateFinalize();
                            user.Send(stream);

                            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.RemoveFlagEffect, user.Player.UID);
                            stream.CaptureTheFlagUpdateFinalize();
                            user.Send(stream);
                        }
                    }
                }
            }
        }
        public void ChechMoveFlag(Client.GameClient user)
        {
            if (Proces != ProcesType.Alive)
                return;
            if (user.Player.Map == MapID)
            {
                if (user.Player.MyGuild == null)
                    return;
                if (!user.Player.ContainFlag(MsgUpdate.Flags.CTF_Flag))
                {
                    foreach (var flag in user.Map.View.Roles(Role.MapObjectType.Npc, user.Player.X, user.Player.Y))
                    {
                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, flag.X, flag.Y) < 2)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.CTF_Flag, 60, true);

                                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateTimer, 60, user.Player.UID);
                                stream.CaptureTheFlagUpdateFinalize();
                                user.Send(stream);

                                user.Player.MyGuild.CTF_Exploits += 3;
                                user.Player.MyGuildMember.CTF_Exploits += 3;

                                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateTimer, 60, user.Player.UID); stream.CaptureTheFlagRankingsFinalize();
                                user.Send(stream);

                                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateEffect, user.Player.UID);
                                stream.CaptureTheFlagUpdateFinalize();
                                user.Send(stream);


                                user.Map.View.LeaveMap<Role.IMapObj>(flag);

                                ActionQuery action;

                                action = new ActionQuery()
                                {
                                    ObjId = flag.UID,
                                    Type = ActionType.RemoveEntity
                                };
                                unsafe
                                {
                                    user.Player.View.SendView(stream.ActionCreate(&action), true);
                                }

                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
