using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
   public static class MsgHandBrickInfo
   {
       // Client layout (CMsgHandBrickInfo): u16 type, i32 own treasury bricks, i32 other treasury bricks, string list.
       //  type 0 = kingdom treasury ("Our Treasury" + optional "<str0>`s Treasury"),
       //  type 1 = plunder war info (2 strings: kingdom, union), type 2 = union ("My Union`s Gold Bricks", 2 strings).
       // The confirm button of the dialog sends MsgAction 436 (SubmitGoldBrick).
       public enum BrickInfo : ushort
       {
           KingdomTreasury = 0,
           PlunderWarInfo = 1,
           ShowSubmitDialog = 2,
       }
       public static unsafe void GetHandBrickInfo(this ServerSockets.Packet stream, out BrickInfo type, out uint OwnBricks, out uint OtherBricks)
        {
            type = (BrickInfo) stream.ReadUInt16();
            OwnBricks = stream.ReadUInt32();
            OtherBricks = stream.ReadUInt32();
        }

       public static unsafe ServerSockets.Packet HandBrickInfoCreate(this ServerSockets.Packet stream, BrickInfo type, uint OwnBricks, uint OtherBricks, string Kingdom, string Union)
        {
            stream.InitWriter();

            stream.Write((ushort)type);
            stream.Write(OwnBricks);
            stream.Write(OtherBricks);
            stream.Write(new string[] { Kingdom ?? "", Union ?? "" });

            stream.Finalize(GamePackets.MsgHandBrickInfo);
            return stream;
        }
       /// <summary>Kept for the existing callers: shows the union treasury with no plunder target.</summary>
       public static unsafe ServerSockets.Packet HandBrickInfoCreate(this ServerSockets.Packet stream, BrickInfo type, ulong DwParam, ushort DwParam2)
        {
            return stream.HandBrickInfoCreate(type, (uint)Math.Min(DwParam, uint.MaxValue), 0, "", "");
        }
       [PacketAttribute(GamePackets.MsgHandBrickInfo)]
       private static void Process(Client.GameClient user, ServerSockets.Packet stream)
       {
           // the client asks for its treasury: answer with the own union`s Gold Bricks (bricks are handed in with MsgAction 436)
           if (user.Player.InUnion)
               user.Player.MyUnion.SendTreasury(stream, user);
           else
               user.SendSysMesage("You should join a Union before you can submit Gold Bricks.");
       }
    }
}
