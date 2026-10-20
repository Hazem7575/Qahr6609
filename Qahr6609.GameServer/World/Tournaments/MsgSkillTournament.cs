using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgSkillTournament : ITournament
    {

        public const uint MapID = 1505, RewardConquerPoints = ServerRate.Hourly, MaxLifes = 3;

        public ProcesType Process { get; set; }
        public TournamentType Type { get; set; }
        public KillerSystem KillSystem;
        public DateTime StartTimer = new DateTime();
        public DateTime ScoreStamp = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public Role.GameMap Map;
        public uint DinamicID, Secounds = 0;
        // Minimum number of players inside when the fight starts for a reward to be paid.
        public static int MinParticipants = 2;
        // The fight always ends after this many minutes, with or without a winner.
        public static int HardLimitMinutes = 15;
        public int StartParticipants = 0;

        public MsgSkillTournament(TournamentType _Type)
        {
            Process = ProcesType.Dead;
            Type = _Type;
        }
        public void Open()
        {
            if (Process == ProcesType.Dead)
            {

                KillSystem = new KillerSystem();
                Map = Database.Server.ServerMaps[MapID];
                DinamicID = Map.GenerateDynamicID();
                MsgSchedules.SendInvitation("SkillTournament", "ConquerPoints", 317, 251, 1002, 0, 60);

                StartTimer = DateTime.Now;
                InfoTimer = DateTime.Now;
                Secounds = 60;
                Process = ProcesType.Idle;
            }
        }
        public void Revive(Extensions.Time32 Timer, Client.GameClient user)
        {
            if (user.Player.Alive == false && Process != ProcesType.Dead)
            {
                if (InTournament(user))
                {
                    if (user.Player.DeadStamp.AddSeconds(4) < Timer)
                    {
                        user.Teleport(315, 251, 1002);
                        user.CreateBoxDialog("You've been eliminated , good luck next time.");
                    }
                }
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.Level < 100)
            {
                user.SendSysMesage("Need to be level 100 at least.");
                return false;
            }
            if (Process == ProcesType.Idle)
            {
                ushort x = 0;
                ushort y = 0;

                user.Player.SkillTournamentLifes = MaxLifes;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, Map.ID, DinamicID);
                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 60, true);
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
                    MsgSchedules.SendSysMesage("SkillTournament has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                    foreach (var user in MapPlayers())
                    {
                        user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                    }
                    StartParticipants = MapCount();
                    if (StartParticipants < MinParticipants)
                    {
                        // Not enough fighters took part: the event ends without a reward.
                        EndWithoutWinner("SkillTournament has ended without a winner: not enough participants.");
                    }
                   
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
                    MsgSchedules.SendSysMesage("[SkillTournament] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                    InfoTimer = DateTime.Now;
                }
            }
            else if (Process == ProcesType.Alive)
            {
                DateTime Now = DateTime.Now;

                if (Now > StartTimer.AddMinutes(HardLimitMinutes))
                {
                    EndWithoutWinner("SkillTournament has ended: time is over.");
                    return;
                }
                if (Now > StartTimer.AddMinutes(4))
                {

                    if (MapCount() == 1 && StartParticipants >= MinParticipants)
                    {
                        var winner = Map.Values.Where(p => p.Player.DynamicID == DinamicID && p.Player.Map == Map.ID).FirstOrDefault();
                        if (winner != null)
                        {
                            MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  SkillTournament. ", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);

                            //winner.Player.AddFlag(MsgServer.MsgUpdate.Flags.Prince, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                            winner.Player.ConquerPoints += (int)RewardConquerPoints;
                            winner.Player.AddFlag(MsgServer.MsgUpdate.Flags.GoldBrickNormal, Role.StatusFlagsBigVector32.HorlyFlag, false, 0, 0, 0, true);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                winner.Teleport(315, 251, 1002);
                                Process = ProcesType.Dead;
                            }

                        }
                    }
                    else if (MapCount() == 0)
                    {
                        Process = ProcesType.Dead;
                    }
                }
            }

        }
        private void EndWithoutWinner(string message)
        {
            foreach (var user in MapPlayers())
            {
                user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                user.Teleport(315, 251, 1002);
            }
            MsgSchedules.SendSysMesage(message, MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
            Process = ProcesType.Dead;
        }
        public int MapCount()
        {
            return MapPlayers().Length;
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => p.Player.DynamicID == DinamicID && p.Player.Map == Map.ID).ToArray();
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
        }

    }
}
