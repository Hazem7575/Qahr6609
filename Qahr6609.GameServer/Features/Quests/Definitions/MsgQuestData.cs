using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetQuestData(this ServerSockets.Packet stream, out uint UnKnow, out uint UID, out uint[] intents)
        {
            UnKnow = stream.ReadUInt32();
            UID = stream.ReadUInt32();
            intents = new uint[1];
            intents[0] = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet MsgQuestDataCreate(this ServerSockets.Packet stream, uint UnKnow, uint UID, params uint[] intents)
        {
            stream.InitWriter();
            stream.Write((ushort)0);
            stream.Write((ushort)intents.Length);
            stream.Write(UID);
            for (int x = 0; x < intents.Length; x++)
                stream.Write(intents[x]);
            if(UID== 35024)
                for (int x = 0; x < intents.Length; x++)
                    stream.Write(intents[x]);
            stream.Write(0);
            if (UID == 35007)
            {
                stream.ZeroFill(12);
                for (int x = 0; x < intents.Length; x++)
                    stream.Write(intents[x]);
            }

            stream.Finalize(GamePackets.QuestData);
            return stream;
        }
        public static unsafe ServerSockets.Packet MsgQuestDataCreate(this ServerSockets.Packet stream, uint UnKnow, uint UID , uint OwnerUID, params uint[] intents)
        {
            stream.InitWriter();

            stream.Write(UnKnow);
            stream.Write(UID);
            for (int x = 0; x < intents.Length; x++)
                stream.Write(intents[x]);
            stream.ZeroFill(sizeof(uint) * Math.Min(6, (6 - intents.Length)));
            stream.Write(OwnerUID);
            stream.ZeroFill(sizeof(uint));
            stream.Finalize(GamePackets.QuestData);
            return stream;
        }
    }
   public class MsgQuestData
    {
       [PacketAttribute(GamePackets.QuestData)]
       private static unsafe void Process(Client.GameClient user, ServerSockets.Packet stream)
       {
           uint UnKnow; uint UID;
           uint[] intents;
           stream.GetQuestData(out UnKnow, out UID, out intents);
           //if (user.OnInterServer)
           {
             /*  if (Database.QuestInfo.IsKingDomMission(UID))
               {
                   user.Player.QuestGUI.SetKingDomQuestObjectives(stream, UID, intents);
               }*/
           }
          /* uint UnKnow; uint UID;
           stream.GetQuestData(out UnKnow, out UID);
           if (Database.QuestInfo.IsKingDomMission(UID))
           {
               user.Send(stream.MsgQuestDataCreate(UnKnow, UID, user.Player.UID));
           }
           */

          // MsgQuestData* Quest = (MsgQuestData*)stream.Pointer;
        //   MyConsole.PrintPacketAdvanced(stream.Memory, stream.Size);

       }
    }
}
