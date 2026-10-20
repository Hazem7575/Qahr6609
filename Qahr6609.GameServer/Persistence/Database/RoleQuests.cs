using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database
{
    public class RoleQuests
    {
        public unsafe static void Save(Client.GameClient user)
        {
            // Count and records come from the same snapshot.
            var quests = user.Player.QuestGUI.src.Values.ToArray();
            using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
            {
                if (binary.Open(Program.ServerConfig.DbLocation + "\\Quests\\" + user.Player.UID + ".bin", FileMode.Create))
                {
                    int AmountQuests = quests.Length;
                    binary.Write(&AmountQuests, sizeof(int));
                    foreach (var quest in quests)
                    {
                        uint UID = quest.UID;
                        uint Status = (uint)quest.Status;
                        uint Time = quest.Time;
                        uint[] intentions = quest.Intentions ?? new uint[0];

                        int size = intentions.Length;
                        binary.Write(&UID, sizeof(uint));
                        binary.Write(&Status, sizeof(uint));
                        binary.Write(&Time, sizeof(uint));
                        binary.Write(&size, sizeof(int));
                        for (int x = 0; x < size; x++)
                        {
                            uint Intention = intentions[x];
                            binary.Write(&Intention, sizeof(uint));
                        }

                    }
                    binary.Close();
                }
            }
        }

        /// <summary>File layout: int count, then per quest uint UID, uint Status, uint Time, int n, n x uint.</summary>
        public static bool ValidateFile(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            long len = data.Length;
            if (len < 4)
                return false;
            int count = BitConverter.ToInt32(data, 0);
            long pos = 4;
            if (count < 0 || count > (len - pos) / 16)
                return false;
            for (int x = 0; x < count; x++)
            {
                if (pos + 16 > len)
                    return false;
                int size = BitConverter.ToInt32(data, (int)pos + 12);
                pos += 16;
                if (size < 0 || size > (len - pos) / 4)
                    return false;
                pos += (long)size * 4;
            }
            return pos == len;
        }

        public unsafe static void Load(Client.GameClient user)
        {
            Load(user, ServerDatabase.PickLoadPath(Program.ServerConfig.DbLocation + "\\Quests\\" + user.Player.UID + ".bin", ValidateFile, user.Player.UID));
        }
        public unsafe static void Load(Client.GameClient user, string path)
        {
            if (path == null)
                return;
            using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
            {
                if (!binary.Open(path, FileMode.Open))
                    throw new InvalidDataException("Cannot open " + path);
                int AmountQuests = 0;
                binary.Read(&AmountQuests, sizeof(int));
                for (int x = 0; x < AmountQuests; x++)
                {

                    uint _UID;
                    uint _Status;
                    uint _Time;
                    int Intentions;
                    uint[] QIntentions;
                    binary.Read(&_UID, sizeof(uint));
                    binary.Read(&_Status, sizeof(uint));
                    binary.Read(&_Time, sizeof(uint));
                    binary.Read(&Intentions, sizeof(int));
                    QIntentions = new uint[Intentions];
                    for (int i = 0; i < Intentions; i++)
                    {
                        uint QIntention;
                        binary.Read(&QIntention, sizeof(uint));
                        QIntentions[i] = QIntention;
                    }

                    var quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                    {
                        UID = _UID,
                        Status = (Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus)_Status,
                        Time = _Time,
                        Intentions = QIntentions
                    };

                    if (!user.Player.QuestGUI.src.ContainsKey(quest.UID))
                        user.Player.QuestGUI.src.Add(quest.UID, quest);
                    if (quest.Status == Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Accepted)
                    {
                        if (!user.Player.QuestGUI.AcceptedQuests.ContainsKey(quest.UID))
                            user.Player.QuestGUI.AcceptedQuests.Add(quest.UID, quest);
                    }
                }
                binary.Close();
            }
        }
    }
}
