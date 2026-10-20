using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgExtremePk : ITournament
    {
       public const uint RewardConquerPoints = ServerRate.Hourly;
       public ProcesType Process { get; set; }
       public DateTime StartTimer = new DateTime();
       public DateTime ScoreStamp = new DateTime();
       public DateTime InfoTimer = new DateTime();
       public Role.GameMap Map;
       public uint DinamicID, Secounds = 0;
       public KillerSystem KillSystem;
       public TournamentType Type { get; set; }
       // Points every participant starts with.
       public const uint StartPoints = 1000;
       // Only participants with at least one kill or a score above this threshold receive a reward.
       public static uint RewardScoreThreshold = StartPoints;
       // Minimum number of players inside when the fight starts for any reward to be paid.
       public static int MinParticipants = 2;
       public int StartParticipants = 0;
       private System.Collections.Concurrent.ConcurrentDictionary<uint, int> Kills = new System.Collections.Concurrent.ConcurrentDictionary<uint, int>();

       private bool EligibleForReward(Client.GameClient user)
       {
           int kills;
           if (Kills.TryGetValue(user.Player.UID, out kills) && kills > 0)
               return true;
           return user.Player.XtremePkPoints > RewardScoreThreshold;
       }
       public MsgExtremePk(TournamentType _type)
       {
           Type = _type;
           Process = ProcesType.Dead;
       }
       public void Open()
       {
           if (Process == ProcesType.Dead)
           {
               KillSystem = new KillerSystem();
               Kills = new System.Collections.Concurrent.ConcurrentDictionary<uint, int>();
               StartParticipants = 0;
               Map = Database.Server.ServerMaps[1505];
               DinamicID = Map.GenerateDynamicID();
               MsgSchedules.SendInvitation("ExtremePk", "ConquerPoints", 311, 251, 1002, 0, 60);
             
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
                       ushort x = 0;
                       ushort y = 0;
                       Map.GetRandCoord(ref x, ref y);
                       user.Teleport(x, y, Map.ID, DinamicID);
                   }
               }
           }
       }
       public bool Join(Client.GameClient user,ServerSockets.Packet stream)
       {
           if (user.Player.Level < 120)
               return false;
           if (Process == ProcesType.Idle)
           {
               user.Player.XtremePkPoints = StartPoints;
               ushort x = 0;
               ushort y = 0;

               Map.GetRandCoord(ref x, ref y);
               user.Teleport(x, y, Map.ID, DinamicID);
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
                   MsgSchedules.SendSysMesage("ExtremePk has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
                   Process = ProcesType.Alive;
                   StartTimer = DateTime.Now;
                   StartParticipants = MapPlayers().Length;
               }
               else if (DateTime.Now > InfoTimer.AddSeconds(10))
               {
                   Secounds -= 10;
                   MsgSchedules.SendSysMesage("[ExtremePk] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
                   InfoTimer = DateTime.Now;
               }
           }
           if (Process == ProcesType.Alive)
           {
               DateTime Now = DateTime.Now;

               if (Now > StartTimer.AddMinutes(5))
               {
                   MsgSchedules.SendSysMesage("ExtremePk has ended. All Players of ExtremePk has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
                  var array = MapPlayers().OrderByDescending(p => p.Player.XtremePkPoints).ToArray();
                   if (StartParticipants < MinParticipants)
                   {
                       foreach (var user in array)
                           user.Teleport(320, 252, 1002);
                       array = new Client.GameClient[0];
                   }
                   if (array.Length > 0 && !EligibleForReward(array.First()))
                   {
                       // Nobody scored: no winner and no rewards.
                       foreach (var user in array)
                           user.Teleport(320, 252, 1002);
                   }
                   else if (array.Length > 0)
                   {
                       var Winner = array.First();
                       MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  ExtremePk. ", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);


                       Winner.Player.AddFlag(MsgServer.MsgUpdate.Flags.Top8Weekly, Role.StatusFlagsBigVector32.DailyFlag, false, 0, 0, 0, true);
                       Winner.Player.ConquerPoints += RewardConquerPoints;
                       //Winner.Player.AddFlag(MsgServer.MsgUpdate.Flags.MonthlyPKChampion, Role.StatusFlagsBigVector32.PermanentFlag, false);
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           Winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                    
                       }

                       int x = 1;
                       foreach (var user in array)
                       {
                           if (x > 1 && EligibleForReward(user))
                           {
                               user.Player.ConquerPoints += (uint)(RewardConquerPoints / x);
                               user.SendSysMesage("You received " + (RewardConquerPoints / x).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                     
                           }
                           x++;
                           user.Teleport(320, 252, 1002);
                       }
                   }
                   Process = ProcesType.Dead;
               }

               if (Now > ScoreStamp)
               {
                     using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var array = MapPlayers().OrderByDescending(p => p.Player.XtremePkPoints).ToArray();

                        foreach (var user in MapPlayers())
                        {
                            Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("---Your Score:" + user.Player.XtremePkPoints + "---", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                            user.Send(msg.GetArray(stream));
                            msg = new MsgServer.MsgMessage("My tournament Kills: " + user.Player.TournamentKills.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
                      
                        }

                        int x = 0;
                        foreach (var obj in array)
                        {
                            if (x == 4)
                                break;
                            Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.XtremePkPoints.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                           
                            SendMapPacket(amsg.GetArray(stream));

                            x++;
                        }
                      
                    }
              

                   ScoreStamp = Now.AddSeconds(4);
               }

             
           }
       }

       public void SharePoints(Client.GameClient user, Client.GameClient Killer)
       {
           if (Killer != null && Killer.Player != null && user != Killer)
               Kills.AddOrUpdate(Killer.Player.UID, 1, (k, v) => v + 1);
           uint xtremepoints = (uint)(user.Player.XtremePkPoints / 5);
           Killer.Player.XtremePkPoints += xtremepoints;
           user.Player.XtremePkPoints -= xtremepoints;

           Killer.SendSysMesage("You gained " + xtremepoints.ToString() + " points for killing " + user.Player.Name + "", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
           user.SendSysMesage("You lost " + xtremepoints.ToString() + " points when " + Killer.Player.Name.ToString() + " has killed you.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
     
          }

       public void SendMapPacket(ServerSockets.Packet stream)
       {
           foreach (var user in MapPlayers())
               user.Send(stream);
       }

       public bool InTournament(Client.GameClient user)
       {
           if (Map == null)
               return false;
           return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
       }

       public Client.GameClient[] MapPlayers()
       {
           return Map.Values.Where(p => InTournament(p)).ToArray();
       }
    }
}
