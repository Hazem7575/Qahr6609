using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
   public static class MsgLeaguePalaceGuardsList
    {


        public static unsafe ServerSockets.Packet LeaguePalaceGuardsListCreate(this ServerSockets.Packet stream, byte Page, byte count)
        {
            stream.InitWriter();
            stream.Write(count);
            stream.Write((byte)Page);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemLeaguePalaceGuardsList(this ServerSockets.Packet stream, uint BattlePower
            , uint Mesh, uint Exploits, uint NobilityRank, uint UID, ushort Level, ushort Class, byte Online, string Name)
        {
            stream.Write(BattlePower);
            stream.Write(Mesh);
            stream.Write(Exploits);
            stream.Write(NobilityRank);
            stream.Write(UID);
            stream.Write(Level);
            stream.Write(Class);
            stream.Write(Online);
            stream.Write(Name, 16);

            return stream;
        }
        public static unsafe ServerSockets.Packet LeaguePalaceGuardsListFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.LeaguePalaceGuardsList);
            return stream;
        }
    }
}
