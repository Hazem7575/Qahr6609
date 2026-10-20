using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet GuildMembersListCreate(this ServerSockets.Packet stream, MsgGuildMembers.Action Mode, int Page
            , Role.Instance.Guild.Member[] Members)
        {
            stream.InitWriter();

            DateTime TimerNow = DateTime.Now;

            const int max = 12;
            int offset = Page / 12 * max;
            int count = Math.Min(max, Math.Max(0, Members.Length - offset));

            stream.Write((uint)Mode);
            stream.Write(Page);
            stream.Write(count);

            for (int x = 0; x < count; x++)
            {
                if (Members.Length > offset + x)
                {
                    var element = Members[offset + x];
                    stream.Write((uint)0);
                    stream.Write(element.Name, 16);
                    stream.Write((uint)(element.IsOnline ? 1 : 0));
                
                    stream.Write(element.NobilityRank);
                    stream.Write((uint)element.Graden);
                    stream.Write((uint)element.Level);
                    stream.Write((uint)element.Rank);
                    stream.Write((uint)0);
                    stream.Write(element.MoneyDonate);


                    stream.Write(element.PrestigePoints);
                    stream.Write((uint)element.Class);
                    stream.Write((uint)(((TimerNow.Ticks - element.LastLogin) / 10000000))); 
                 
                    stream.Write((uint)element.Mesh);
                }
            }
            stream.Finalize(GamePackets.GuildMembers);
           
            return stream;
        }

        public static unsafe ServerSockets.Packet GuildRankListCreate(this ServerSockets.Packet stream, MsgGuildMembers.Action Mode, Role.Instance.Guild Guild, Role.Instance.Guild.Member[] Members)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write(0);
            stream.Write(Math.Min(28, Members.Length));
            for (int x = 0; x < Math.Min(28, Members.Length); x++)
            {
                try
                {
                    if (Members.Length > x)
                    {
                        var element = Members[x];

                        stream.Write((uint)element.Level);
                        stream.Write((uint)(element.IsOnline ? 1 : 0));
                        stream.Write((uint)Guild.ShareMemberPotency(element.Rank));
                        stream.Write(0);
                        stream.Write(element.Name, 16);
                    }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }

            stream.Finalize(GamePackets.GuildMembers);

            return stream;
        }
    }

    public struct MsgGuildMembers
    {
        public enum Action : uint
        {
            MembersList = 0,
            ListRanks = 1
        }
       

        [PacketAttribute(GamePackets.GuildMembers)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.MyGuild != null)
            {
                var Members = user.Player.MyGuild.Members.Values.ToArray();

                Array.Sort(Members, (f1, f2) =>
                {
                    int n_rank = f2.IsOnline.CompareTo(f1.IsOnline);
                  //  if (f2.IsOnline == f1.IsOnline)
                  //      return f2.Rank.CompareTo(f1.Rank);
                    return n_rank;
                });
                if (Members != null)
                {
                    Action Mode = (Action)stream.ReadUInt32();
                    uint Page = stream.ReadUInt32();

                    user.Send(stream.GuildMembersListCreate(Mode, (int)Page, Members));
                }
            }
        }
    }
}
