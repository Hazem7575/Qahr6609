using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgFreezeWar : ITournament
    {
        public const uint MapID = 1505;

        public class Team
        {
            public enum TeamType : byte
            {
                None = 0,
                Blue = 1,
                Red = 2
            }

            public TeamType Type;
            public MsgFreezeWar Freeze;
            public uint Score = 0;
            public Team(TeamType _type, MsgFreezeWar _freeze)
            {
                Freeze = _freeze;
                Type = _type;
            }
            public Client.GameClient[] Players()
            {
                return Freeze.Map.Values.Where(p => p.Player.DynamicID == Freeze.DinamicID
                    && p.Player.Map == Freeze.Map.ID
                    && p.Player.FreezeTeamType == Type && p.Socket.Alive).ToArray();
            }
            public int Count
            {
                get { return Players().Length; }
            }
            public bool FullFreeze()
            {
                var freezePlayers = Players().Where(p => p.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze)).Count();
                return freezePlayers == Count;
            }
            public void TeleportPlayers(Client.GameClient user)
            {
                switch (Type)
                {
                    case TeamType.Blue:
                        {
                            user.Teleport(153, 105, 1505, Freeze.DinamicID);
                            break;
                        }
                    case TeamType.Red:
                        {
                            user.Teleport(165, 245, 1505, Freeze.DinamicID);
                            break;
                        }
                }
            }
            public void AddPlayer(ServerSockets.Packet stream, Client.GameClient user)
            {
                user.Player.FreezeTeamType = Type;
                switch (Type)
                {

                    case TeamType.Blue:
                        {
                         
                            user.Player.AddSpecialGarment(stream, 185825);
                            break;
                        }
                    case TeamType.Red:
                        {
                            user.Player.AddSpecialGarment(stream, 185625);
                            break;
                        }
                }
                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, Role.StatusFlagsBigVector32.PermanentFlag, true);
            }

        }
        public ProcesType Process { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime ScoreStamp = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public Role.GameMap Map;
        public uint DinamicID, Secounds = 0;
        public KillerSystem KillSystem;
        public TournamentType Type { get; set; }
        public DateTime StampTeleport = new DateTime();
        public bool onTeleport = false;
        public MsgFreezeWar(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }


        public Team Team1;
        public Team Team2;

        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                onTeleport = false;
                Team1 = new Team(Team.TeamType.Blue, this);
                Team2 = new Team(Team.TeamType.Red, this);

                KillSystem = new KillerSystem();
                Map = Database.Server.ServerMaps[MapID];
                DinamicID = Map.GenerateDynamicID();
                MsgSchedules.SendInvitation("FreezeWar", "ConquerPoints", 314, 196, 1002, 0, 60);
 
                StartTimer = DateTime.Now;
                InfoTimer = DateTime.Now;
                Secounds = 60;
                Process = ProcesType.Idle;
            }
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
                if (Team1.Count < Team2.Count)
                {
                    Team1.AddPlayer(stream, user);
                    Team1.TeleportPlayers(user);
                }
                else
                {
                    Team2.AddPlayer(stream, user);
                    Team2.TeleportPlayers(user);
                }
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
                    MsgSchedules.SendSysMesage("FreezeWar has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                 
                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;

                  
                    foreach (var user in Team1.Players())
                        user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                    foreach (var user in Team2.Players())
                        user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
                    MsgSchedules.SendSysMesage("[FreezeWar] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
                    InfoTimer = DateTime.Now;
                  
                }
            }
            if (Process == ProcesType.Alive)
            {
                if (onTeleport)
                {
                    if (DateTime.Now > StampTeleport)
                    {
                        foreach (var user in Team1.Players())
                        {
                            user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                            Team1.TeleportPlayers(user);

                        }
                        foreach (var user in Team2.Players())
                        {
                            user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                            Team2.TeleportPlayers(user);
                        }
                        onTeleport = false;
                    }
                }
                else
                {
                    DateTime Now = DateTime.Now;
                    if (Team1.FullFreeze())
                        FinishRound(Team2);
                    else if (Team2.FullFreeze())
                        FinishRound(Team1);

                    if (Team1.Score == 5)
                    {
                        FinishTournament(Team1);
                        return;
                    }
                    else if (Team2.Score == 5)
                    {
                        FinishTournament(Team2);
                        return;
                    }
                    if (Team1.Count == 0)
                        FinishTournament(Team2);
                    else if (Team2.Count == 0)
                        FinishTournament(Team1);

                    if (Now > StartTimer.AddMinutes(5))
                    {
                        if (Team1.Score > Team2.Score)
                        {
                            MsgSchedules.SendSysMesage("FreezeWar has ended." + Team1.Type.ToString() + " has won ! All Players of FreezeWar has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                         
                            FinishTournament(Team1);
                        }
                        else if (Team2.Score >= Team1.Score)
                        {
                            MsgSchedules.SendSysMesage("FreezeWar has ended." + Team2.Type.ToString() + " All Players of FreezeWar has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                     
                           FinishTournament(Team2);
                        }



                        Process = ProcesType.Dead;
                    }

                    if (Now > ScoreStamp)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("FreezeWar: Time Left: " + FinishTimer() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                            SendMapPacket(msg.GetArray(stream));
                            if (Team1.Score >= Team2.Score)
                            {
                                msg = new MsgServer.MsgMessage("BlueTeam Score: " + Team1.Score + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                                SendMapPacket(msg.GetArray(stream));
                                msg = new MsgServer.MsgMessage("RedTeam Score: " + Team2.Score + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                                SendMapPacket(msg.GetArray(stream));
                            }
                            else
                            {
                                msg = new MsgServer.MsgMessage("RedTeam Score: " + Team2.Score + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                                SendMapPacket(msg.GetArray(stream));
                                msg = new MsgServer.MsgMessage("BlueTeam Score: " + Team1.Score + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                                SendMapPacket(msg.GetArray(stream));
                            }

                         
                        }
                        ScoreStamp = Now.AddSeconds(4);
                    }
                }
            }
        }
        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in MapPlayers())
                user.Send(stream);
        }
        public string FinishTimer()
        {
            TimeSpan Start = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan Finish = new TimeSpan(StartTimer.AddMinutes(10).Ticks);
            string message = "" + (Finish.Minutes - Start.Minutes).ToString() + " : " + (Finish.Seconds - Start.Seconds).ToString() + "";
            return message;
        }
        public void FinishTournament(Team Winner)
        {
            MsgSchedules.SendSysMesage("FreezeWar has ended." + Winner.Type.ToString() + " All Players of FreezeWar has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
           
           foreach (var user in Winner.Players())
            {
                user.Teleport(260, 135, 1002);
                user.Player.ConquerPoints += ServerRate.FreezWar;
                //user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Prince, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);

                user.CreateBoxDialog("Your team won and received 25000 ConquerPoints.");
            }
            Team losser;
            if (Team1 == Winner)
                losser = Team2;
            else
                losser = Team1;
            foreach (var user in losser.Players())
            {
                user.Teleport(260, 135, 1002);
                user.CreateBoxDialog("Your team lose : ( .Better luck next time.");
               
            }
            Process = ProcesType.Dead;
        }
        public void FinishRound(Team winner)
        {
            onTeleport = true;
            StampTeleport = DateTime.Now.AddSeconds(5);
            winner.Score += 1;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("" + winner.Type.ToString() + " has won this round. The next round has started.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System);
                SendMapPacket(msg.GetArray(stream));
                msg = new MsgServer.MsgMessage("The next round will start in 5 seconds.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System);
                SendMapPacket(msg.GetArray(stream));
               
            }
            foreach (var user in Team1.Players())
                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 10,false);
            foreach (var user in Team2.Players())
                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 10, false);
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => p.Player.DynamicID == DinamicID && p.Player.Map == MapID).ToArray();
        }
    }
}
