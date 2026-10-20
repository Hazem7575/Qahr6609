using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetProficiency(this ServerSockets.Packet stream, out MsgProficiency prof)
        {
            prof = new MsgProficiency();
            prof.ID = stream.ReadUInt32();
            prof.Level = stream.ReadUInt32();
            prof.Experience = stream.ReadUInt32();
            prof.UID = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet ProficiencyCreate(this ServerSockets.Packet stream, uint ID,uint Level, uint Experience, uint UID)
        {
            stream.InitWriter();

            stream.Write(ID);
            stream.Write(Level);
            stream.Write(Experience);
            stream.Write(UID);//UID);

            stream.Finalize(GamePackets.Proficiency);
            return stream;
        }
    }
    public class MsgProficiency
    {
        public uint ID;
        public uint Level;
        public uint Experience;
        public uint UID;
        public byte PreviouseLevel;
    }
}
