using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ItemViewCreate(this ServerSockets.Packet stream, uint PlayerUID, long Cost, MsgGameItem DateItem, MsgItemView.ActionMode Mode)
        {
            stream.InitWriter();

            stream.Write(DateItem.UID);
            stream.Write(PlayerUID);
            stream.Write(Cost);
            stream.Write(DateItem.ITEM_ID);
            stream.Write(DateItem.Durability);
            stream.Write(DateItem.MaximDurability);
            stream.Write((ushort)Mode);
           
            stream.Write((byte)DateItem.Position);
            stream.Write(DateItem.SocketProgress);
            stream.Write((byte)DateItem.SocketOne);
            stream.Write((byte)DateItem.SocketTwo);
            stream.Write((uint)DateItem.Effect);
            
            stream.Write((byte)0);
            stream.Write(DateItem.Plus);
            stream.Write(DateItem.Bless);
            stream.Write(DateItem.Bound);
            stream.Write(DateItem.Enchant);
            stream.Write((uint)DateItem.ProgresGreen);//46
            stream.Write(DateItem.Suspicious);//50
            stream.Write((byte)0);
            stream.Write((byte)DateItem.Locked);
            stream.Write((byte)0);
            stream.Write((ushort)DateItem.Color);
            stream.Write(DateItem.PlusProgress);
            stream.Write(DateItem.Inscribed);
            //stream.Write(DateItem.Activate);
            if (DateItem.RemainingTime == 0)
            {
                stream.Write(DateItem.Activate);
            }
            stream.Write(DateItem.RemainingTime);
            stream.Write(DateItem.StackSize);
            stream.Write(DateItem.Purification.PurificationItemID);
            stream.Write(DateItem.Purification.PurificationItemID);
            stream.Write(DateItem.PerfectionLevel);//level max = 59
            stream.Write(DateItem.PerfectionProgress);//progres
            stream.Write(DateItem.OwnerUID);//uid

            stream.Write(DateItem.OwnerName, 16);
            stream.Write(DateItem.Signature, 32);//110
      

            stream.ZeroFill(4);//unknow..

            
            stream.Finalize(GamePackets.ItemView);

            return stream;
        }

    }

    public class MsgItemView
    {
        public enum ActionMode : ushort
        {
            Gold = 1,
            CPs = 3,
            ViewEquip = 4
        }
    }
}
