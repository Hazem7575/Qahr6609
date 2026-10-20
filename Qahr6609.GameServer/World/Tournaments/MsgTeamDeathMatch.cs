using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgTeamDeathMatch : ITournament
    {

       public enum TeamTyps : byte
       {
           BlueTeam =0, RedTeam =1, BlackTeam=2, WhiteTeam =3, Count=4
       }
       public uint GetGarmentID(TeamTyps typ)
       {
           switch (typ)
           {
               case TeamTyps.BlueTeam: return BlueGarment;
               case TeamTyps.RedTeam: return RedGarment; 
               case TeamTyps.BlackTeam: return BlackGarment; 
               case TeamTyps.WhiteTeam: return WhiteGarment;
           }
           return 0;
       }

       public TeamTyps GetTeamTyp(uint GarmentID)
       {
           switch (GarmentID)
           {
               case RedGarment: return TeamTyps.RedTeam;
               case WhiteGarment: return TeamTyps.WhiteTeam;
               case BlueGarment: return TeamTyps.BlueTeam;
               case BlackGarment: return TeamTyps.BlackTeam;
           }
           return TeamTyps.Count;
       }

       public const uint RedGarment = 185625,
           WhiteGarment = 185325,
           BlackGarment = 181525,
           BlueGarment = 185825,
           MapID = 1082;


       public DateTime StampScore = new DateTime();
       public TournamentType Type { get; set; }

       public Dictionary<uint, Client.GameClient>[] Teams;



       public ProcesType Process { get; set; }
       public DateTime StartTimer = new DateTime();
       public KillerSystem KillSystem;
       public MsgTeamDeathMatch(TournamentType _type)
       {
           Type = _type;
           Process = ProcesType.Dead;
       }

       public bool InTournament(Client.GameClient user)
       {
           return user.Player.Map == MapID;
       }
       public void Open()
       {
           if (Process == ProcesType.Dead)
           {
               KillSystem = new KillerSystem();
               Teams = new Dictionary<uint, Client.GameClient>[(uint)TeamTyps.Count];
               for (uint x = 0; x < (uint)TeamTyps.Count; x++)
                   Teams[x] = new Dictionary<uint, Client.GameClient>();

               Process = ProcesType.Idle;
               MsgSchedules.SendInvitation("TeamDeathMatch", "ConquerPoints", 328, 140, 1002, 0, 60);

               StartTimer = DateTime.Now;
           }
       }
       public bool Join(Client.GameClient client, ServerSockets.Packet stream)
       {
           if(client.Player.Level < 80)
           {
               client.SendSysMesage("Need to be level 80 at least.");
               return false;
           }
           if (Process == ProcesType.Idle)
           {
               client.Player.TeamDeathMacthKills = 0;

               var Array = Teams.OrderByDescending(p => p.Count).ToArray();

               for (int x = 0; x < (uint)Teams.Length; x++)
               {
                   if (Teams[x] == Array[Array.Length - 1])
                   {
                       if (!Teams[x].ContainsKey(client.Player.UID))
                           Teams[x].Add(client.Player.UID, client);

                       client.Player.AddSpecialGarment(stream, GetGarmentID((TeamTyps)x));
                   }
               }       

               client.Teleport(276, 193, 1082);

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
                   StartTimer = DateTime.Now;
                   Process = ProcesType.Alive;
                   MsgSchedules.SendSysMesage("TeamDeathMatch has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);               
                   TeamTyps Typ = TeamTyps.BlueTeam;
                   foreach (var objects in Teams)
                   {
                       foreach (var user in objects.Values)
                           TeleportPlayer(Typ, user);
                       Typ += 1;
                   }

               }
           }
           if (Process == ProcesType.Alive)
           {
               if (DateTime.Now > StartTimer.AddMinutes(5))
               {
                   Process = ProcesType.Dead;
                   MsgSchedules.SendSysMesage("TeamDeathMatch has ended. All Players of TeamDeathMatch has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   Dictionary<TeamTyps, uint> Scores = new Dictionary<TeamTyps, uint>();
                   for (int x = 0; x < (uint)TeamTyps.Count; x++)
                       Scores.Add((TeamTyps)x, GetTeamScore(Teams[x]));
                   var ranks = Scores.OrderByDescending(p => p.Value);
                   int i = 0;
                   foreach (var objects in ranks)
                   {
                       if (i == 0)
                       {

                           MsgSchedules.SendSysMesage("" + objects.Key.ToString() + " Won  TeamDeathMatch.", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);
                         
                           foreach (var user in Teams[(uint)objects.Key].Values)
                           {
                               if (user.Player.TeamDeathMacthKills != 0 && user.Player.Map == MapID)
                               {
                                   user.Player.ConquerPoints += user.Player.TeamDeathMacthKills * ServerRate.TeamDeathMacthRank1;
                                   user.SendSysMesage("You received " + (user.Player.TeamDeathMacthKills * ServerRate.TeamDeathMacthRank1).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                              
                              }
                           }
                           Teams[(uint)objects.Key].Clear();
                       }
                       break;
                   }
                   foreach (var objects in Teams)
                   {
                       foreach (var user in objects.Values)
                       {
                           if (user.Player.TeamDeathMacthKills != 0 && user.Player.Map == MapID)
                           {
                               user.Player.ConquerPoints += user.Player.TeamDeathMacthKills * ServerRate.TeamDeathMacth;
                               user.SendSysMesage("You received " + (user.Player.TeamDeathMacthKills * ServerRate.TeamDeathMacth).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                         
                          }
                       }
                   }
                   using (var rec = new ServerSockets.RecycledPacket())
                   {
                       var stream = rec.GetStream();
                       foreach (var client in Database.Server.GamePoll.Values)
                       {
                           if (client.Player.Map == MapID)
                           {
                               client.Player.RemoveSpecialGarment(stream);
                               client.Teleport(278, 138, 1002);
                           }
                       }
                   }
                   Teams = new Dictionary<uint, Client.GameClient>[(uint)TeamTyps.Count];
                   Process = ProcesType.Dead;
               }

               if (DateTime.Now > StampScore)
               {

                   Dictionary<TeamTyps, uint> Scores = new Dictionary<TeamTyps, uint>();
                   for (int x = 0; x < (uint)TeamTyps.Count; x++)
                       Scores.Add((TeamTyps)x, GetTeamScore(Teams[x]));


                   var ranks = Scores.OrderByDescending(p => p.Value);
                   using (var rec = new ServerSockets.RecycledPacket())
                   {
                       var stream = rec.GetStream();

                       Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("Death Match Score: Time Left: " + FinishTimer() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                      
                      SendMapPacket(msg.GetArray(stream));
                       int i = 0;
                       foreach (var obj in ranks)
                       {
                           Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (i + 1).ToString() + ". " + obj.Key.ToString() + " (" + obj.Value.ToString() + ")"
                             , MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                         
                           SendMapPacket(amsg.GetArray(stream));
                           i++;
                       }

                       foreach (var client in Database.Server.GamePoll.Values)
                       {
                           if (client.Player.Map == MapID)
                           {
                               Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("Your Kills: " + client.Player.TournamentKills.ToString() + ""
              , MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                             
                               client.Send(amsg.GetArray(stream));
                           }
                       }
                    
                   }
                   StampScore = DateTime.Now.AddSeconds(5);
               }
           }
       }
       public string FinishTimer()
       {
           TimeSpan Start = new TimeSpan(DateTime.Now.Ticks);
           TimeSpan Finish = new TimeSpan(StartTimer.AddMinutes(10).Ticks);
           string message = "" + (Finish.Minutes - Start.Minutes).ToString() + " : " + (Finish.Seconds - Start.Seconds).ToString() + "";
           return message;
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
       public uint GetTeamScore(Dictionary<uint, Client.GameClient> players)
       {
           uint score = 0;
           if (players != null)
           {
               if (players.Count > 0)
               {
                   foreach (var user in players.Values)
                       score += user.Player.TeamDeathMacthKills;
               }
           }
           return score;
       }


       public void TeleportPlayer(TeamTyps Typ, Client.GameClient client)
       {
           switch (Typ)
           {
               case TeamTyps.RedTeam: client.Teleport(289, 169, (ushort)MapID); break;
               case TeamTyps.WhiteTeam: client.Teleport(170, 289, (ushort)MapID); break;
               case TeamTyps.BlackTeam: client.Teleport(166, 79, (ushort)MapID); break;
               case TeamTyps.BlueTeam: client.Teleport(77, 166, (ushort)MapID); break;
           }
       }



    }
}
