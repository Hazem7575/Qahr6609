using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgLastManStand : ITournament
    {

        public const uint RewardConquerPoints = ServerRate.Hourly;
        // Minimum number of players that must be inside when the fight starts for a reward to be paid.
        public static int MinParticipants = 2;
        public int StartParticipants = 0;

        public ProcesType Process { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public uint Secounds = 60;
        public Role.GameMap Map;
        public uint DinamicMap = 0;
        public KillerSystem KillSystem;
        public TournamentType Type { get; set; }
        public MsgLastManStand(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                KillSystem = new KillerSystem();
                StartTimer = DateTime.Now;
                MsgSchedules.SendInvitation("LastManStand", "ConquerPoints,YinYangFruit", 329, 251, 1002, 0, 60);
                

                if (Map == null)
                {
                    Map = Database.Server.ServerMaps[700];
                    DinamicMap = Map.GenerateDynamicID();
                }
                InfoTimer = DateTime.Now;
                Secounds = 60;
                Process = ProcesType.Idle;
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
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, Map.ID, DinamicMap);
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
                    MsgSchedules.SendSysMesage("LastManStand has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    
                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                    StartParticipants = MapPlayers().Length;
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
                    MsgSchedules.SendSysMesage("[LastManStand] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
                    InfoTimer = DateTime.Now;
                }
            }
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > StartTimer.AddMinutes(5))
                {
                    foreach (var user in MapPlayers())
                    {
                        user.Teleport(295, 145, 1002);
                    }
                    MsgSchedules.SendSysMesage("LastManStand has ended. All Players of LastManStand has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
                    
                    Process = ProcesType.Dead;
                    return;
                }
                if (StartParticipants < MinParticipants)
                {
                    // Not enough fighters took part: the event ends without a reward.
                    foreach (var user in MapPlayers())
                        user.Teleport(334, 250, 1002);
                    MsgSchedules.SendSysMesage("LastManStand has ended without a winner: not enough participants.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    Process = ProcesType.Dead;
                    return;
                }
                if (MapPlayers().Length == 1)
                {
                    var winner = MapPlayers().First();


                    MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  LastManStand , he received " + RewardConquerPoints.ToString() + " ConquerPoints and 1 YinYangFruit.", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);
                
                    winner.Player.ConquerPoints += RewardConquerPoints;
                    //winner.Player.AddFlag(MsgServer.MsgUpdate.Flags.King, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (winner.Inventory.HaveSpace(1))
                            winner.Inventory.Add(stream, 711188);
                        else
                            winner.Inventory.AddReturnedItem(stream, 711188);
                    }
                    winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                 
                    winner.Teleport(334, 250, 1002, 0);

                    Process = ProcesType.Dead;
                }

                Extensions.Time32 Timer =   Extensions.Time32.Now;
                foreach (var user in MapPlayers())
                {
                    if (user.Player.Alive == false)
                    {
                        if (user.Player.DeadStamp.AddSeconds(4) < Timer)
                            user.Teleport(334, 250, 1002);
                    }
                }
            }


        }

        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => p.Player.DynamicID == DinamicMap && p.Player.Map == Map.ID).ToArray();
        }

        public bool InTournament(Client.GameClient user)
        {
            if (Map == null) return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicMap;
        }
    }
}
