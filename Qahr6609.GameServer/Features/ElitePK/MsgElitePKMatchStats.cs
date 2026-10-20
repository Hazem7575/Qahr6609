using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ElitePKMatchStatsCreate(this ServerSockets.Packet stream, MsgTournaments.MsgEliteGroup.Match match)
        {
            stream.InitWriter();

            if (match.PlayersFighting.Length > 0)
            {
              
                stream.Write(match.PlayersFighting[0].Player.UID);
                stream.Write(match.PlayersFighting[0].Player.Name, 16);
                stream.Write((uint)0); //server id
                stream.Write(match.PlayersFighting[0].ElitePKStats.Points);
             
            }
            else
                stream.ZeroFill(28);

            if (match.PlayersFighting.Length > 1)
            {
                stream.Write(match.PlayersFighting[1].Player.UID);
                stream.Write(match.PlayersFighting[1].Player.Name, 16);
                stream.Write((uint)0);  //server id
                stream.Write(match.PlayersFighting[1].ElitePKStats.Points);
            }
            else
                stream.ZeroFill(28);


            stream.Write((uint)0);//unknow

            stream.Finalize(GamePackets.MsgElitePKMatchStats);

            return stream;
        }

    }
}
