using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgNpc
{
    public unsafe struct NpcServerQuery
    {
        public int Stamp;
        public NpcID ID;//8
        public ushort Mesh;//12
        public byte OptionID;//14
        public byte InteractType;//15
        public NpcServerReplay.Mode Action;//16
        public Role.Flags.NpcType NpcType;//18

    }
    public unsafe static class NpcServerReplay
    {

        public enum Mode : ushort
        {
            Statue =3,
            PlaceFurniture = 6,
            Cursor = 5
        }


        public static ServerSockets.Packet NpcDialog(this ServerSockets.Packet stream, out uint npcid, out ushort Mesh, out byte option, out byte type,out Mode Action,  out string input)
        {
            int Stamp = stream.ReadInt32();

            npcid = stream.ReadUInt32();
            Mesh = stream.ReadUInt16();

            option = stream.ReadUInt8();
            type = stream.ReadUInt8();

            Action = (Mode)stream.ReadUInt8();

            int size = stream.ReadUInt8();
            input = stream.ReadCString(size);

            
            return stream;
        }
        public static ServerSockets.Packet NpcServerCreate(this ServerSockets.Packet stream,NpcServerQuery PQuery)
        {
            stream.InitWriter();

            stream.Write((uint)Extensions.Time32.Now.Value);
            stream.Write((uint)PQuery.ID);
            stream.Write(PQuery.Mesh);
            stream.Write(PQuery.OptionID);
            stream.Write(PQuery.InteractType);
            stream.Write((ushort)PQuery.Action);
            stream.Write((ushort)PQuery.NpcType);
            stream.Finalize(GamePackets.NpcServerReplay);
            return stream;
        }
    }
}
