using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conquer6609.Game.MsgServer;
using Conquer6609.MsgInterServer.Packets;

namespace Conquer6609.MsgInterServer
{
   public class StaticConnexion
    {
       public class Entity
       {
           public ServerSockets.SecuritySocket SecuritySocket;

           public Entity(ServerSockets.SecuritySocket _socket)
           {
               _socket.Client = this;
               SecuritySocket = _socket;
           }
           public void Send(ServerSockets.Packet msg)
           {
               SecuritySocket.Send(msg);
           }
           public void Disconnect()
           {
               SecuritySocket.Disconnect();
           }

       }
       public static ServerSockets.ServerSocket ServerConnecxion;
       public static Extensions.ThreadGroup.ThreadItem Thread = null;
       public static ServerSockets.SecuritySocket ClientConnecxion;
       public static DateTime SendInfo = new DateTime();



       public static void Create()
       {
           if (Program.ServerConfig.IsInterServer == false)
           {

               ServerConnecxion = new ServerSockets.ServerSocket(
               new Action<ServerSockets.SecuritySocket>(ProcessConnect)

               , new Action<ServerSockets.SecuritySocket, ServerSockets.Packet>((p, data) =>
               {
                   ProcesReceive(p, data);
               })
               , new Action<ServerSockets.SecuritySocket>(p => (p.Client as Entity).Disconnect()));
               Connect();
             

               if (Thread == null)
               {
                   Thread = new Extensions.ThreadGroup.ThreadItem(1000, "InterServer", CheckConnection);
                   Thread.Open();
               }
           }
       }
       public static void Send(ServerSockets.Packet stream)
       {
           if (ClientConnecxion == null)
               return;
           if (ClientConnecxion.Alive == false)
               return;
           ClientConnecxion.Send(stream);
       }
       public static void Connect()
       {
           if (Program.ServerConfig.IsInterServer == false)
           {
               ServerConnecxion.Connect(Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port, "InterServer");
           }
       }
       public static void ProcessConnect(ServerSockets.SecuritySocket Socket)
       {
           if (Program.ServerConfig.IsInterServer == false)
           {
               var obj = new Entity(Socket);
               Socket.OnInterServer = true;
               // pipe v2 control link: the Realm sends PipeHello, we answer with the HMAC proof; ServerInfo (registration)
               // goes out only after the Realm proved the secret too (ProcesReceive, PipeAuthOk)
               Socket.Pipe = new PipeCodec(false, PipeLinkType.Control);
               ClientConnecxion = Socket;
               PipeHandshake.ArmTimeout(Socket, "control link");
               Socket.ConnectFull = true;
           }
       }
       public static void ProcesReceive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)
       {
           var Game = (obj.Client as Entity);
           ushort PacketID = stream.ReadUInt16();
           try
           {
               var codec = obj.Pipe;
               if (codec == null || !codec.Ready)
               {
                   bool ok = false;
                   if (codec != null && PacketID == PacketTypes.PipeHello)
                       ok = PipeHandshake.AnswerHello(obj, stream, Database.GroupServerList.MyServerInfo.ID);
                   else if (codec != null && PacketID == PacketTypes.PipeAuthOk && PipeHandshake.AcceptAuthOk(obj, stream))
                   {
                       ok = true;
                       var DBServer = Database.GroupServerList.MyServerInfo;
                       obj.Send(stream.ServerInfoCreate(1, DBServer.ID, DBServer.Name, DBServer.MapID, DBServer.X, DBServer.Y, DBServer.Group));
                   }
                   if (!ok)
                   {
                       Conquer6609.Security.Guard.Audit("interserver", 0, "", "control link handshake with " + obj.RemoteIp + " failed at packet " + PacketID);
                       obj.Disconnect();
                   }
                   return;
               }
               switch (PacketID)
               {
                   case PacketTypes.InterServer_EliteRank:
                       {
                           MsgElitePkRanking.RankType rank;
                           uint Group;
                           MsgElitePKBrackets.GuiTyp GroupStatus;
                           uint Count;
                           uint UID;

                           List<Game.MsgTournaments.MsgEliteGroup.FighterStats> players = new List<Game.MsgTournaments.MsgEliteGroup.FighterStats>();
                           stream.GetElitePkRanking(out rank, out Group, out GroupStatus, out Count, out UID);
                           for (int x = 0; x < Count; x++)
                           {
                               Game.MsgTournaments.MsgEliteGroup.FighterStats status;
                               stream.GetItemElitePkRanking(out status);
                               players.Add(status);
                           }
                           Instance.CrossElitePKTournament.AddRanks(Group, players);
                           break;
                       }
                   case Conquer6609.Game.GamePackets.Chat:
                       {

                           var mes = new MsgMessage();
                           mes.Deserialize(stream);

                           if (mes.ChatType == MsgMessage.ChatMode.BroadcastMessage)
                           {
                               if (mes.__Message == "[Cross Elite PK Tournament] begins at 20:00. Get yourself prepared for it!")
                               {
                                   Core.IsCrossPkOpen = true;
                                   Core.JoinCrossEliteStamp = DateTime.Now.AddMinutes(60 * 5);
                                   Conquer6609.Game.MsgTournaments.MsgSchedules.SendInvitation("Cross Elite PK Tournament", "[Special Accesory/Boots,Cps and more rewards]", 293, 160, 1002, 0, 60);
                                   break;
                               }
                           }
                           stream.Seek(stream.Size);
                           foreach (var user in Database.Server.GamePoll.Values)
                           {
                               user.Send(stream);
                           }

                           break;
                       }
                   case PacketTypes.InterServer_UnionRanks:
                       {
                           Game.MsgServer.MsgLeagueRank.ActionID type; ushort count;
                           ushort Page; byte dwparam; ushort PageCount;
                           stream.GetLeagueRank(out type, out count, out Page, out dwparam, out PageCount);
                           if (count == 1)
                           {
                               uint ServerID; uint GoldBricks; string Name; string LeaderName;
                               stream.GetItemLeagueRank(out ServerID, out GoldBricks, out Name, out LeaderName);
                               Instance.Union.AddUnion(ServerID, GoldBricks, Name, LeaderName);

                           }
                           break;
                       }
               }
           }
           catch (Exception e)
           {
               MyConsole.SaveException(e);
           }
           finally
           {
               ServerSockets.PacketRecycle.Reuse(stream);
           }
       }
       public static void CheckConnection()
       {
           if (ClientConnecxion == null)
               return;

           if (!ClientConnecxion.Alive || ClientConnecxion.Connection.Connected == false)
           {
               ClientConnecxion = null;
               Create();
               return;
           }
           if (DateTime.Now > SendInfo.AddSeconds(10))
           {


               //send union info.

               using (var rec = new ServerSockets.RecycledPacket())
               {
                   var stream = rec.GetStream();

                   var array = Role.Instance.Union.UnionPoll.Values.Where(p => p.IsKingdom == 1 && p.GoldBrick > 0).ToArray();

                   foreach (var obj in array)
                   {
                       
                       stream.LeagueRankCreate(MsgLeagueRank.ActionID.ShowAllUnions, (ushort)1, (ushort)0, (byte)0, (ushort)0);
                       stream.AddItemLeagueRank(Database.GroupServerList.MyServerInfo.ID, obj.GoldBrick, obj.Name, obj.Emperor);
                       ClientConnecxion.Send(stream.InterServerLeagueRankFinalize());
                   }
               }
               
               SendInfo = DateTime.Now;
           }
          

       }

    }
}
