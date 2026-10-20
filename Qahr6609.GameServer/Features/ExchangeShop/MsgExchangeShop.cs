using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Conquer6609.Game.MsgServer
{
   public static class MsgExchangeShop
    {
        [ProtoContract]
       public class ExchangeShop
        {
            [ProtoMember(1, IsRequired = true)]
            public uint DwParam1;
            [ProtoMember(2, IsRequired = true)]
            public uint DwParam2;
            [ProtoMember(3, IsRequired = true)]
            public uint DwParam3;
            [ProtoMember(4, IsRequired = true)]
            public uint DwParam4;

            [ProtoMember(5, IsRequired = true)]
            public Item[] Items;

            [ProtoContract]
            public class Item
            {
                [ProtoMember(1, IsRequired = true)]
                public uint ID;
                [ProtoMember(2, IsRequired = true)]
                public uint Cost;
                
            }
        }
        public static unsafe ServerSockets.Packet CreateExchangeShop(this ServerSockets.Packet stream, ExchangeShop obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.ExchangeShop);//2441

            return stream;
        }
        public static unsafe void GetExchangeShop(this ServerSockets.Packet stream, out ExchangeShop pQuery)
        {
            pQuery = new ExchangeShop();
            pQuery = stream.ProtoBufferDeserialize<ExchangeShop>(pQuery);
        }


        
    }
}
