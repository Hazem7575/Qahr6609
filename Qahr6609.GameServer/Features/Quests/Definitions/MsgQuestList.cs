using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetQuestList(this ServerSockets.Packet stream, out  MsgQuestList.QuestMode Mode, out ushort Count, out uint QuestID, out MsgQuestList.QuestListItem.QuestStatus QuestMode
            , out uint QuestTimer)
        {
            Mode = (MsgQuestList.QuestMode)stream.ReadUInt16();
            Count = stream.ReadUInt16();
            QuestID = stream.ReadUInt32();
            QuestMode = (MsgQuestList.QuestListItem.QuestStatus)stream.ReadUInt32();
            QuestTimer = stream.ReadUInt32();

        }
        public static unsafe ServerSockets.Packet QuestListCreate(this ServerSockets.Packet stream, MsgQuestList.QuestMode Mode, ushort Count)
        {
            stream.InitWriter();

            stream.Write((ushort)Mode);
            stream.Write(Count);


            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemQuestList(this ServerSockets.Packet stream, MsgQuestList.QuestListItem item)
        {

            stream.Write(item.UID);
            stream.Write((uint)item.Status);
            stream.Write(item.Time);


            return stream;
        }
        public static unsafe ServerSockets.Packet QuestListFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.QuestList);

            return stream;
        }
    }


    public unsafe struct MsgQuestList
    {
        public class QuestListItem
        {
            public enum QuestStatus : uint
            {
                Accepted = 0,
                Finished = 1,
                Available = 2
            }
            public uint UID;
            public QuestStatus Status;
            public uint Time;
            public uint[] Intentions = new uint[1];
        }
        public enum QuestMode : ushort
        {
            AcceptQuest = 1,
            QuitQuest = 2,
            Review = 3,
            FinishQuest = 4
        }

        // Quests that may only be accepted through their NPC dialog (QuestGUI.Accept in NpcHandler),
        // never through the quest-list packet. Their objectives can be completed by ClikerEntry.
        public static readonly HashSet<uint> NpcOnlyQuests = new HashSet<uint>() { 6131, 1355, 1330, 1487 };

        [PacketAttribute(GamePackets.QuestList)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {

            MsgQuestList.QuestMode Mode;
            ushort Count;
            uint QuestID; 
            MsgQuestList.QuestListItem.QuestStatus QuestaMode;
             uint QuestTimer;

             stream.GetQuestList(out Mode, out Count, out QuestID, out QuestaMode, out QuestTimer);

            switch (Mode)
            {
                case QuestMode.AcceptQuest:
                    {
                        if (NpcOnlyQuests.Contains(QuestID))
                        {
                            Conquer6609.Security.Guard.Audit("quest-accept-reject", user.Player.UID, user.Player.Name, "npc-only quest " + QuestID);
                            break;
                        }
                        if (user.Player.QuestGUI.AllowAccept())
                        {
                            Database.QuestInfo.DBQuest quest;
                            if (Database.QuestInfo.AllQuests.TryGetValue(QuestID, out quest))
                            {
                                if (Database.QuestInfo.IsKingDomMission(quest.MissionId))
                                {
                                    QuestListItem _quest;
                                    user.Player.QuestGUI.AcceptKingDomMission(quest, 0, out _quest);
                                }
                                else
                                    user.Player.QuestGUI.Accept(quest, QuestTimer);
                            }
                        }
                        break;
                    }
                case QuestMode.FinishQuest:
                    {


                        break;
                    }
                case QuestMode.QuitQuest:
                    {
                        QuestListItem n_quest;
                        if (user.Player.QuestGUI.src.TryGetValue(QuestID, out n_quest))
                        {
                            user.Player.QuestGUI.SendSinglePacket(n_quest, Game.MsgServer.MsgQuestList.QuestMode.QuitQuest);
                            user.Player.QuestGUI.RemoveQuest(n_quest.UID);
                        }
                        
                        break;
                    }
                case QuestMode.Review:
                    {
                        if (Count < 80)
                            user.Player.QuestGUI.SendFullGUI(stream);
                        break;
                    }
            }
        }
    }
}
