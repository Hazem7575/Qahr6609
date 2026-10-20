using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
   public static class MsgArenaCrossServer
    {
       [PacketAttribute(GamePackets.ArenaCrossServer)]
       private unsafe static void Poroces(Client.GameClient user, ServerSockets.Packet stream)
       {
   
           MyConsole.PrintPacketAdvanced(stream.Memory, stream.Size);
           if (Program.ServerConfig.IsInterServer)
           {
               if (MsgTournaments.MsgSchedules.ElitePkTournament.Proces != MsgTournaments.ProcesType.Dead)
               {
                   stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, 3, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, user.Player.UID);
                   stream.ElitePkRankingFinalize();
                   user.Send(stream);
                   return;
               }
           }
       }
    }
}
