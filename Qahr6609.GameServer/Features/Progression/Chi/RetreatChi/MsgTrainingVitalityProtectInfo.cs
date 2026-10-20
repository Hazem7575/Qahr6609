using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ReatreatChiInfoCreate(this ServerSockets.Packet stream, uint Count)
        {
            stream.InitWriter();
            stream.Write(Count);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddRetreatedChiItem(this ServerSockets.Packet stream,Game.MsgServer.MsgChiInfo.ChiPowerType Type, Tuple<Role.Instance.Chi.ChiAttributeType, int>[] Gate, long Time)
        {
            int[] Powers = new int[Gate.Length];
            for (int x = 0; x < Gate.Length; x++)
                Powers[x] = Gate[x].Item2;
            stream.Write((byte)Type);
            var now = DateTime.FromBinary(Time);
            uint secs = (uint)(now.Year % 100 * 100000000 + (now.Month) * 1000000 + now.Day * 10000 + now.Hour * 100 + now.Minute);
            stream.Write(secs);
            if (Powers != null)
            {
                for (int x = 0; x < Powers.Length; x++)
                {
                    stream.Write(Powers[x]);
                }
            }
            else
                stream.ZeroFill(4 * sizeof(int));
            return stream;
        }
        public static unsafe ServerSockets.Packet CreateRetreatedChiItems(this ServerSockets.Packet stream, int Count)
        {
            stream.InitWriter();
            stream.Write((uint)Count);
            return stream;
        }
        public static unsafe ServerSockets.Packet ChiRetreatInfoFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTrainingVitalityProtectInfo);
            return stream;
        }
    }
    public unsafe struct MsgTrainingVitalityProtectInfo
    {
        public static void SendInfo(ServerSockets.Packet stream, Client.GameClient client)
        {
            // (a Thread.Sleep(500) here stalled every client on the socket thread for half a second)
            int Count = 0;
            if (client.Player.MyChi.DragonTime != 0)
                Count++;
            if (client.Player.MyChi.PhoenixTime != 0)
                Count++;
            if (client.Player.MyChi.TigerTime != 0)
                Count++;
            if (client.Player.MyChi.TurtleTime != 0)
                Count++;
            stream.CreateRetreatedChiItems(Count);
            if (client.Player.MyChi.DragonTime != 0)
            {
                stream.AddRetreatedChiItem(Game.MsgServer.MsgChiInfo.ChiPowerType.Dragon, client.Player.MyChi.DragonPowers, client.Player.MyChi.DragonTime);
            }
            if (client.Player.MyChi.PhoenixTime != 0)
            {
                stream.AddRetreatedChiItem(Game.MsgServer.MsgChiInfo.ChiPowerType.Phoenix, client.Player.MyChi.PhoenixPowers, client.Player.MyChi.PhoenixTime);
            }
            if (client.Player.MyChi.TigerTime != 0)
            {
                stream.AddRetreatedChiItem(Game.MsgServer.MsgChiInfo.ChiPowerType.Tiger, client.Player.MyChi.TigerPowers, client.Player.MyChi.TigerTime);
            }
            if (client.Player.MyChi.TurtleTime != 0)
            {
                stream.AddRetreatedChiItem(Game.MsgServer.MsgChiInfo.ChiPowerType.Turtle, client.Player.MyChi.TurtlePowers, client.Player.MyChi.TurtleTime);
            }
            client.Send(stream.ChiRetreatInfoFinalize());
        }
    }
}
