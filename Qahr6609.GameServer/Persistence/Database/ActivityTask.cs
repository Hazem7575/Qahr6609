using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class ActivityTask : Extensions.SafeDictionary<uint, ActivityTask.Task>
    {

        public class Task
        {
            public uint ID;
            public byte NeedProgress = 0;
            public byte ActiveValue = 0;
            public uint OpenLev;
            public uint CloseLev;

            // OpenLev / CloseLev use the client encoding "reborn * 1000 + level" (e.g. 1001 = 1st reborn level 1,
            // 2141 = 2nd reborn level 141). Exactly like the client (CDlgActivityTask) a task is open when
            // OpenLev <= value < CloseLev, so a character only has the task tier of its own reborn count.
            public static uint LevelValue(ushort level, byte reborn)
            {
                return (uint)reborn * 1000 + level;
            }
            public bool CheckOpen(ushort level, byte reborn)
            {
                uint value = LevelValue(level, reborn);
                if (value < OpenLev)
                    return false;
                if (CloseLev != 0 && value >= CloseLev)
                    return false;
                return true;
            }

        }


        public void Load()
        {
            WindowsAPI.IniFile Reader = new WindowsAPI.IniFile("ActivityTask.ini");

            int TaskCount = Reader.ReadByte("ActivityTask", "Num", 0);

            for (uint x = 1; x <= TaskCount; x++)
            {
                string section = x.ToString();
                // The official (client) ActivityTask.ini numbers its sections 1..Num but the task id is the "id" key
                // (e.g. section [36] is task id 8). Old server copies have section == id, so fall back to it.
                uint id = Reader.ReadUInt32(section, "id", x);
                if (id == 0 || ContainsKey(id))
                    continue;
                Add(id, new Task()
                {
                    ID = id,
                    NeedProgress = Reader.ReadByte(section, "Progress", 0),
                    ActiveValue = Reader.ReadByte(section, "ActiveValue", 0),
                    OpenLev = Reader.ReadUInt32(section, "OpenLev", 0),
                    CloseLev = Reader.ReadUInt32(section, "CloseLev", 0)
                });
            }

        }
    }
}
