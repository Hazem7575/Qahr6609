using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ShowEquipmentCreate(this ServerSockets.Packet stream, MsgShowEquipment item)
        {
            /*5B 00 F1 03 27 63 99 06 00 00 00 00 00 00 00 00      ;[ ñ'c        
00 00 00 00 2E 00 00 00 00 00 00 00 00 00 00 00      ;    .           
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 E7      ;               ç
10 38 00 E6 10 38 00 00 00 00 00 00 00 00 00 00      ;8 æ8          
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 00 00 00 00 00 00 00 54 51 53 65 72      ;           TQSer
76 65 72                                             ;ver*/
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);

            stream.Write(item.wParam);//8

            stream.Write(item.Alternante);

            stream.Write(0);

       stream.Write(item.UID);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
           
            stream.Write((byte)0);

            stream.Write(item.Head);
            stream.Write(item.Necklace);
            stream.Write(item.Armor);
            stream.Write(item.RightWeapon);
            stream.Write(item.LeftWeapon);
            stream.Write(item.Ring);
            stream.Write(item.Bottle);
            stream.Write(item.Boots);
          
            stream.Write(item.Garment);
            stream.Write(item.RightWeaponAccessory);
            stream.Write(item.LeftWeaponAccessory);
            stream.Write(item.SteedMount);
            stream.Write(item.RidingCrop);
            stream.Write(item.Wing);
        

            stream.Finalize(GamePackets.Usage);
         
            return stream;
        }

    }
    public class MsgShowEquipment
    {
        public const ushort AlternanteAllow = 44,
    Show = 46;

        public int Stamp;
        public uint wParam;
        public uint Alternante;
        public ushort UID;
        public uint Head;
        public uint Necklace;
        public uint Armor;
        public uint RightWeapon;
        public uint LeftWeapon;
        public uint Ring;
        public uint Bottle;
        public uint Boots;
        public uint Garment;
        public uint RightWeaponAccessory;
        public uint LeftWeaponAccessory;
        public uint SteedMount;
        public uint RidingCrop;
        public uint Wing;
    }
}
