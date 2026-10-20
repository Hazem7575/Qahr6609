using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe struct TeamMemberInfo
    {
        public string Name;
        public uint UID;
        public uint Mesh;
        public ushort MaxHitpoints;
        public ushort MinMHitpoints;
    }
    public unsafe static class MsgTeamMemberInfo
    {
        

        public static unsafe ServerSockets.Packet TeamMemberInfoCreate(this ServerSockets.Packet stream, TeamMemberAction action, Role.Instance.Team.MemberInfo[] members)
        {
            stream.InitWriter();

            stream.Write((byte)action);

            if (members != null)
                stream.Write((byte)members.Length);
            else
                stream.Write((byte)0);
         

            stream.Write((ushort)1);

            for (int x = 0; x < 5; x++)
            {
                if (members != null && members.Length > x)
                {
                    var mem = members[x];

              
                    stream.Write(mem.Info.Name, 16);
                    stream.Write(mem.Info.UID);
                    stream.Write(mem.Info.Mesh);
                    
                    stream.Write((uint)mem.Info.MaxHitpoints);
                    stream.Write((uint)mem.Info.MinMHitpoints);
                }
                else
                    stream.ZeroFill(32);
            }
            stream.Write((uint)0);//unknow;
            stream.Finalize(GamePackets.TeamMemberInfo);



            return stream;
        }


        public enum TeamMemberAction : byte
        {
            AddMember = 0,
            DropAddMember = 1
        }

    }
}
