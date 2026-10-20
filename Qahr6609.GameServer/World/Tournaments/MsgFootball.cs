using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgFootball : ITournament
    {
        public const uint MapID = 1017;
        public class Team
        {
            public enum TeamTypes : byte
            {
                Spain = 0,
                Argentina = 1
            }
            public Team(MsgFootball _Football, TeamTypes type, int number)
            {
                TeamType = type;
                TeamNumber = number;
                Football = _Football;
            }
            public MsgFootball Football;
            public int TeamNumber = 0;
            public TeamTypes TeamType = TeamTypes.Spain;

            public int Score = 0;

            public int Count
            {
                get
                {
                    return Football.Map.Values.Where(p => p.Player.FootballTeamID == TeamNumber).Count();
                }
            }
            public void SetPlayerLocation(Client.GameClient user, bool addfreze, bool teleported = true)
            {
                if (addfreze)
                    user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 8, true);
                if (teleported)
                {
                    switch (TeamNumber)
                    {
                        case 1:
                            {
                                byte rand = (byte)Program.GetRandom.Next(0, 5);
                                user.Teleport((ushort)(97 + 3 * rand), (ushort)(87 + 3 * rand), 1017);
                                break;
                            }
                        case 2:
                            {
                                byte rand = (byte)Program.GetRandom.Next(0, 5);
                                user.Teleport((ushort)(83 + 3 * rand), (ushort)(99 + 3 * rand), 1017);
                                break;
                            }
                    }
                }
            }
            public void AddPlayer(Client.GameClient user, ServerSockets.Packet stream)
            {
                SetPlayerLocation(user, false);
                user.Player.FootballTeamID = TeamNumber;
                switch (TeamType)
                {
                    case TeamTypes.Spain:
                        {

                            user.Player.AddSpecialGarment(stream, 192665);
                            break;
                        }
                    case TeamTypes.Argentina:
                        {
                            user.Player.AddSpecialGarment(stream, 192675);
                            break;
                        }

                }
                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, Role.StatusFlagsBigVector32.PermanentFlag, true);
            }
        }

        public Team Team1;
        public Team Team2;
        public Role.GameMap Map;
        public ProcesType Process { get; set; }
        public TournamentType Type { get; set; }
        public MsgFootball(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        public DateTime StampScore;
        public DateTime StartTimer;
        public DateTime InfoTimer;
        public int Secounds;


        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                Map = Database.Server.ServerMaps[MapID];
                Team1 = new Team(this, Team.TeamTypes.Spain, 1);
                Team2 = new Team(this, Team.TeamTypes.Argentina, 2);
                MsgSchedules.SendInvitation("Football[War]", "ConquerPoints, 1-PowerExpBalls", 223, 251, 1002, 0, 60);
                StartTimer = DateTime.Now;
                InfoTimer = DateTime.Now;
                Secounds = 60;
                Process = ProcesType.Idle;
                StampScore = DateTime.Now;
                AddNpc();

            }
        }
        Role.SobNpc npc = new Role.SobNpc();
        public void AddNpc()
        {
            if (Map.View.Contain(3333333, 98, 101))
                return;
            npc = new Role.SobNpc();
            npc.X = 98;
            npc.Map = Map.ID;
            npc.ObjType = Role.MapObjectType.SobNpc;
            npc.Y = 101;
            npc.UID = 3333333;
            npc.Type = Role.Flags.NpcType.Talker;
            npc.Mesh = (Role.SobNpc.StaticMesh)480;
            npc.Name = "";
            Map.View.EnterMap<Role.IMapObj>(npc);
            Map.SetFlagNpc(npc.X, npc.Y);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var user in Map.View.Roles(Role.MapObjectType.Player, npc.X, npc.Y))
                {
                    user.Send(npc.GetArray(stream, false));
                }
            }
        }
        public unsafe void RemoveNpc()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                packet.ID = MsgStringPacket.StringID.Effect;
                packet.UID = npc.UID;
                packet.Strings = new string[1] { "accession1" };
                ActionQuery action;

                action = new ActionQuery()
                {
                    ObjId = npc.UID,
                    Type = ActionType.RemoveEntity
                };
                foreach (var user in Map.View.Roles(Role.MapObjectType.Player, npc.X, npc.Y))
                {
                    user.Send(stream.StringPacketCreate(packet));
                    user.Send(stream.ActionCreate(&action));
                }
            }
            Map.View.LeaveMap<Role.IMapObj>(npc);
            Map.RemoveFlagNpc(npc.X, npc.Y);


        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.Level < 80)
            {
                user.SendSysMesage("Need to be level 80 at least.");
                return false;
            }
            if (Process == ProcesType.Idle)
            {
                if (user.Player.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                    user.Player.RemoveFlag(MsgUpdate.Flags.lianhuaran04);
                user.Player.FootBallMatchPoints = 0;
                if (Team1.Count < Team2.Count)
                    Team1.AddPlayer(user, stream);
                else
                    Team2.AddPlayer(user, stream);
                return true;
            }
            return false;
        }

        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddMinutes(1))
                {

                    MsgSchedules.SendSysMesage("Football has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    
                     Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                    foreach (var user in Players())
                        user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
                    MsgSchedules.SendSysMesage("[Football] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                 
                    InfoTimer = DateTime.Now;
                }
            }
            else if (Process == ProcesType.Alive)
            {
                if (OnFinishRound)
                {
                    if (DateTime.Now > TeleportedTime)
                    {
                        OnFinishRound = false;
                        foreach (var user in Players())
                        {
                            if (user.Player.FootballTeamID == Team1.TeamNumber)
                                Team1.SetPlayerLocation(user, true, true);
                            else
                                Team2.SetPlayerLocation(user, true, true);
                        }

                        AddNpc();
                    }
                }
                if (DateTime.Now > StartTimer.AddMinutes(8))
                {
                    if (Team1.Score > Team2.Score)
                        Reward(Team1);
                    else
                        Reward(Team2);
                    Finish();
                    return;
                }
                if (Team1.Count == 0)
                {
                    Reward(Team2);
                    Finish();
                }
                else if (Team2.Count == 0)
                {
                    Reward(Team1);
                    Finish();
                }
                //update score

                if (DateTime.Now > StampScore.AddSeconds(5))
                {
                    CheckBall();
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("Football Match Score:", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                       
                        SendMapPacket(msg.GetArray(stream));
                        msg = new MsgServer.MsgMessage("" + Team2.TeamType.ToString() + " : " + Team2.Score + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        SendMapPacket(msg.GetArray(stream));
                        msg = new MsgServer.MsgMessage("" + Team1.TeamType.ToString() + " : " + Team1.Score + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        SendMapPacket(msg.GetArray(stream));

                        foreach (var user in Players())
                        {
                            msg = new MsgServer.MsgMessage("Your Score : " + user.Player.FootBallMatchPoints + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
                        }
                    }
                    StampScore = DateTime.Now;
                }

            }
        }
        public void CheckBall()
        {
            if (Map.View.Contain(3333333, 98, 101))
                return;
            bool newround = true;
            foreach (var user in Players())
                if (user.Player.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                    newround = false;
            if (newround)
                FinishRound();
        }
        public void CheckNaked(Client.GameClient user)
        {
            if (!user.Player.ContainFlag(MsgServer.MsgUpdate.Flags.lianhuaran04))
                return;

            if (user.Player.FootballTeamID == 2)
            {
                if (RightNaked(user.Player.X, user.Player.Y))
                {
                    TeamGoal(user);
                }
            }
            else
            {
                if (LeftNaked(user.Player.X, user.Player.Y))
                {
                    TeamGoal(user);
                }
            }

        }

        public void PassTheBall(Client.GameClient user, Client.GameClient target)
        {
         
            user.Player.AddFlag(MsgServer.MsgUpdate.Flags.lianhuaran04, Role.StatusFlagsBigVector32.PermanentFlag, true);
            target.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.lianhuaran04);
        }
        public void TeamGoal(Client.GameClient user)
        {
            user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.lianhuaran04);
            Team myteam = null;
            if (Team1.TeamNumber == user.Player.FootballTeamID)
                myteam = Team1;
            else
                myteam = Team2;
            myteam.Score += 1;
            user.Player.FootBallMatchPoints += 1;
            user.Player.MyFootBallPoints += 1;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("The player " + user.Player.Name + " scored a goal for " + myteam.TeamType.ToString() + " .", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System);
               
                SendMapPacket(msg.GetArray(stream));

            }
            if (myteam.Score == 5)
                Finish();
            else
                FinishRound();

        }
        public bool OnFinishRound = false;
        public DateTime TeleportedTime = new DateTime();
        public void FinishRound()
        {
            OnFinishRound = true;
            TeleportedTime = DateTime.Now.AddSeconds(2);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("The next round will start in 8 seconds.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System);
               SendMapPacket(msg.GetArray(stream));
            }
            foreach (var user in Players())
            {
                user.Player.AddFlag(MsgUpdate.Flags.Freeze, 10, true);
            }
           
        }
        public bool LeftNaked(ushort x, ushort y)
        {
            for (ushort i = 0; i < 6; i++)
            {
                ushort _x = (ushort)(53 + 3 * i);
                ushort _y = (ushort)(138 + 3 * i);
                if (GetDistance(x, y, _x, _y) <= 3)
                    return true;
            }
            return false;
        }
        public bool RightNaked(ushort x, ushort y)
        {
            for (ushort i = 0; i < 6; i++)
            {
                ushort _x = (ushort)(137 + 3 * i);
                ushort _y = (ushort)(53 + 3 * i);
                if (GetDistance(x, y, _x, _y) <= 3)
                    return true;
            }
            return false;
        }
        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            short x = 0;
            short y = 0;
            if (X >= X2) x = (short)(X - X2);
            else if (X2 >= X) x = (short)(X2 - X);
            if (Y >= Y2) y = (short)(Y - Y2);
            else if (Y2 >= Y) y = (short)(Y2 - Y);
            if (x > y) return x;
            else return y;
        }
        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in Players())
                user.Send(stream);
        }
        public Client.GameClient[] Players()
        {
            return Map.Values;
        }
        public bool InTournament(Client.GameClient user)
        {
            return user.Player.Map == MapID;
        }
        public void Reward(Team winner)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var user in Players())
                {
                    if (user.Player.FootballTeamID == winner.TeamNumber)
                    {
                        user.Player.ConquerPoints += ServerRate.Hourly;

                        user.CreateBoxDialog("Your team won and received " + ServerRate.Hourly + " ConquerPoints.");
                      
                    }
                   
                }
            }
            

        }
        public void Finish()
        {
            Process = ProcesType.Dead;
            try
            {
                var thebest = Players().OrderByDescending(p => p.Player.FootBallMatchPoints).FirstOrDefault();
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (Team1.Count < 1 && Team2.Count < 1)
                    {
                        return;
                    }
                    if (Team1.Score > Team2.Score)
                    {



                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("" + Team1.TeamType.ToString() + " won against " + Team2.TeamType.ToString() + " with the score " + Team1.Score + " - " + Team2.Score + ". Most Valuable Player of the match : " + thebest != null ? thebest.Player.Name : "" + "", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center);
                        SendMapPacket(msg.GetArray(stream));

                      
                    }
                    else
                    {

                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("" + Team2.TeamType.ToString() + " won against " + Team1.TeamType.ToString() + " with the score " + Team2.Score + " - " + Team1.Score + ". Most Valuable Player of the match : " + thebest != null ? thebest.Player.Name : "" + "", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center);
                        SendMapPacket(msg.GetArray(stream));

                    }

                    foreach (var user in Players())
                    {
                        
                        user.Player.RemoveSpecialGarment(stream);
                        user.Teleport(260, 141, 1002);
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
    }
}
