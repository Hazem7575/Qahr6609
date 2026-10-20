using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgCpsArenaSingle
    {
        public TournamentType Type = TournamentType.MsgCpsArenaSingle;
        public bool Working = false, Joined = false, Teleported = false;
        public const uint MapID = 1760;
        public const uint MinCurrentBet = 500;
        public CSAPlayer PlayerOne = null, PlayerTwo = null;
        public DateTime MatchStamp = new DateTime();
        public ProcesType Process { get; set; }
        public DateTime ScoreStamp = new DateTime();

        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in MapPlayers())
                user.Send(stream);
        }
        public Client.GameClient[] MapPlayers()
        {
            return Database.Server.ServerMaps[MapID].Values.Where(p => InTournament(p)).ToArray();
        }
        public bool InTournament(Client.GameClient user)
        {
            return user.Player.Map == MapID;
        }
        private void AnnounceScores(ServerSockets.Packet stream)
        {
            ScoreStamp = DateTime.Now.AddSeconds(2);
        }
        public MsgCpsArenaSingle(ProcesType _process = ProcesType.Dead)
        {
            Process = _process;
        }

        public bool SignUp(Client.GameClient _user, uint _bet)
        {
            if (PlayerOne != null && PlayerTwo != null)
            {
                _user.SendSysMesage("Tournment Completed.");
                return false;
            }
            if (_bet < MinCurrentBet)
                return false;
            if (Joined)
                return false;
            if ((_bet < MinCurrentBet ? true : false) == true)
            {
                _bet = MinCurrentBet;
            }
            if (_user.Player.ConquerPoints < _bet)
            {
                return false;
            }
            if(PlayerOne == null)
            {
                PlayerOne = null;
                PlayerOne = new CSAPlayer() { UID = _user.Player.UID, Bet = _bet, Teleported = false };
                _user.Player.ConquerPoints -= _bet;
                return true;
            }
            else//if player one selected already but player two is still null
            {
                if (!PlayerOne.Client.Socket.Alive)
                {
                    PlayerOne = null;
                    PlayerOne = new CSAPlayer() { UID = _user.Player.UID, Bet = _bet, Teleported = false };
                    _user.Player.ConquerPoints -= _bet;
                    return true;
                }
                if (PlayerTwo == null)
                {
                    PlayerTwo = null;
                    PlayerTwo = new CSAPlayer() { UID = _user.Player.UID, Bet = _bet, Teleported = false };
                    _user.Player.ConquerPoints -= _bet;
                    Process = ProcesType.Idle;
                    return true;
                }
                else
                {
                    _user.Player.MessageBox("Sorry Only 2 Members Avaliable Count.", null, null);
                }
            }
            return false;
        }

        public void CheckUp()
        {
            if (Process == ProcesType.Dead)
            {
                foreach (var p in MapPlayers())
                {
                    p.Teleport(262, 255, 1002);
                }
                return;
            }
            if (Process == ProcesType.Idle)
            {
                if (PlayerOne != null && PlayerTwo != null)
                {
                    if (PlayerOne.Client.Socket.Alive && PlayerTwo.Client.Socket.Alive)
                    {
                        SendPVPInvitation(30, PlayerOne.Client);
                        SendPVPInvitation(30, PlayerTwo.Client);
                    }
                }
                else
                    return;
            }
            if (Process == ProcesType.Alive)
            {
                if (PlayerOne != null && PlayerTwo != null)
                {
                    if (!PlayerOne.Client.Socket.Alive && PlayerTwo.Client.Socket.Alive)
                    {
                        End(PlayerTwo.UID, PlayerOne.UID);
                    }
                    else if (PlayerOne.Client.Socket.Alive && !PlayerTwo.Client.Socket.Alive)
                    {
                        End(PlayerOne.UID, PlayerTwo.UID);
                    }
                    else if (!PlayerOne.Client.Socket.Alive && !PlayerTwo.Client.Socket.Alive)
                    {
                        Process = ProcesType.Dead;
                        PlayerOne = null;
                        PlayerTwo = null;
                    }
                    else
                    {
                    }
                }
                if (DateTime.Now >= ScoreStamp)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        AnnounceScores(stream);
                    }
                }
                if (DateTime.Now >= MatchStamp)
                {
                    if (PlayerOne.Score > PlayerTwo.Score)
                    {
                        End(PlayerOne.UID, PlayerTwo.UID);
                    }
                    else if (PlayerOne.Score < PlayerTwo.Score)
                    {
                        End(PlayerTwo.UID, PlayerOne.UID);
                    }
                    else
                    {
                        MatchStamp.AddSeconds(30);
                        PlayerOne.Client.Player.MessageBox("Fight Still For 30 Seconds More.", null, null, 30);
                        PlayerTwo.Client.Player.MessageBox("Fight Still For 30 Seconds More.", null, null, 30);
                    }
                }
            }
        }
        public void End(uint Winner, uint Loser)//ending match void  With Recall back
        {
            if (PlayerOne.UID == Winner && PlayerTwo.UID == Loser)
            {
                if (PlayerOne.Client != null && PlayerTwo.Client != null)
                {
                    PlayerOne.Client.Player.ConquerPoints += PlayerOne.Bet;
                    //PlayerOne.Client.Player.ConquerPoints += PlayerTwo.Bet;
                    //PlayerOne.Client.SendSysMesage("You won Elite PVP Cps Arena.");
                    PlayerTwo.Client.SendSysMesage("You lost Elite PVP Cps Arena.");
                    //PlayerOne.Client.Teleport(262, 255, 1002);
                    PlayerTwo.Client.Teleport(262, 255, 1002);
                    PlayerOne = null;
                    PlayerTwo = null;
                    Process = ProcesType.Dead;
                }
            }
            if (PlayerTwo.UID == Winner && PlayerOne.UID == Loser)
            {
                if (PlayerOne.Client != null && PlayerTwo.Client != null)
                {
                    PlayerTwo.Client.Player.ConquerPoints += PlayerOne.Bet;
                    //PlayerTwo.Client.Player.ConquerPoints += PlayerTwo.Bet;
                    PlayerTwo.Client.SendSysMesage("You won Elite PVP Cps Arena.");
                    //PlayerOne.Client.SendSysMesage("You lost Elite PVP Cps Arena.");
                    //PlayerOne.Client.Teleport(262, 255, 1002);
                    PlayerTwo.Client.Teleport(262, 255, 1002);
                    PlayerOne = null;
                    PlayerTwo = null;
                    Process = ProcesType.Dead;
                }
            }
            //giving Winner Cps
            //teleporting both outside
            //Ending war and stop invokation
            //turning Process to Dead
        }
        public void Teleport(uint UID)
        {
            if (PlayerOne == null || PlayerTwo == null)
                return;
            ushort x = 0;
            ushort y = 0;
            Role.GameMap Map = Database.Server.ServerMaps[MapID];
            Map.GetRandCoord(ref x, ref y);
            if (PlayerOne.UID == UID && PlayerOne.Client.Socket.Alive)
            {
                PlayerOne.Client.Teleport(x, y, MapID);
                PlayerOne.Client.Player.MessageBox("StartFighting Still Working For 100 Seconds.", null, null, 100);
                PlayerOne.Client.Player.SetPkMode(Role.Flags.PKMode.PK);
                if (MapPlayers().Length == 2)
                {
                    Process = ProcesType.Alive;
                    MatchStamp = DateTime.Now.AddSeconds(90);
                }
            }
            else if (PlayerTwo.UID == UID && PlayerTwo.Client.Socket.Alive)
            {
                PlayerTwo.Client.Teleport(x, y, MapID);
                PlayerOne.Client.Player.MessageBox("StartFighting Still Working For 100 Seconds.", null, null, 100);
                PlayerTwo.Client.Player.SetPkMode(Role.Flags.PKMode.PK);
                if (MapPlayers().Length == 2)
                {
                    Process = ProcesType.Alive;
                    MatchStamp = DateTime.Now.AddSeconds(100);
                }
            }
            else
            {
            }
        }
        public void GiveUp(uint UID)
        {
            if (PlayerOne.UID == UID && PlayerOne.Client.Socket.Alive)
            {
                End(PlayerTwo.UID, UID);
            }
            if (PlayerTwo.UID == UID && PlayerTwo.Client.Socket.Alive)
            {
                End(PlayerOne.UID, UID);
            }
        }
        public void ResondInteract(uint UID, uint Damage)
        {
            if (PlayerTwo != null && PlayerOne != null)
            {
                if (PlayerOne.Client.Socket.Alive && PlayerTwo.Client.Socket.Alive)
                {
                    if (UID == PlayerOne.UID)
                    {
                        PlayerOne.Score += Damage;
                    }
                    if (UID == PlayerTwo.UID)
                    {
                        PlayerTwo.Score += Damage;
                    }
                }
            }
        }
        internal static bool SendPVPInvitation(int Secounds, Client.GameClient client, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
#if Arabic
             string Message = "Cps Arena Betting Battle Match Started Want To Accept Or Give UP?";
#else
            string Message = "Cps Arena Betting Battle Match Started Want To Accept Or Give UP?";
#endif

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                var packet = new Game.MsgServer.MsgMessage(Message, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream);
                    if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer())
                        return false;
                    client.Send(packet);
                    client.Player.MessageBox(Message, new Action<Client.GameClient>(user => MsgTournaments.MsgSchedules.ArenaCpsBattle.Teleport(user.Player.UID)), user => MsgTournaments.MsgSchedules.ArenaCpsBattle.GiveUp(user.Player.UID), Secounds, messaj);
                    return true;
            }
        }
    }
    public class CSAPlayer
    {
        public uint UID;
        public uint Bet;
        public uint Score;
        public bool Teleported;
        public string Name { get { return Client.Player.Name; } }
        public Client.GameClient Client
        {
            get
            {
                if (this.UID == 0)
                    return null;
                if (Database.Server.GamePoll.ContainsKey(this.UID))
                {
                    return Database.Server.GamePoll[this.UID];
                }
                return null;
            }
        }
    }
}