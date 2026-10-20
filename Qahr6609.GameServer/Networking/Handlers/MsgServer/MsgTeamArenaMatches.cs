using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{

    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetTeamArenaMatches(this ServerSockets.Packet stream, out uint Page)
        {
            Page = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet TeamArenaMatchesCreate(this ServerSockets.Packet stream, uint Page
            , MsgTournaments.MsgArena.ArenaIDs Typ, uint MarchesCount, uint PlayersCount, uint PageCount)
        {
            stream.InitWriter();
            stream.Write(Page);
            stream.Write((uint)Typ);
            stream.Write(MarchesCount);
            stream.Write(PlayersCount);
            stream.Write((uint)0);
            stream.Write(PageCount);

            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemTeamArenaMatches(this ServerSockets.Packet stream, Role.Instance.Team team1, Role.Instance.Team team2)
        {
            stream.Write(team1.UID);
            stream.Write(team1.TeamName, 16);
            stream.Write((uint)team1.Members.Count);

            stream.Write(team2.UID);
            stream.Write(team2.TeamName, 16);
            stream.Write((uint)team2.Members.Count);

            return stream;
        }
        public static unsafe ServerSockets.Packet TeamArenaMatchesFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgTeamArenaMatches);
            return stream;
        }
    }

    public unsafe struct MsgTeamArenaMatches
    {
        [PacketAttribute(GamePackets.MsgTeamArenaMatches)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                uint aPage;
                stream.GetTeamArenaMatches(out aPage);

                var info = MsgTournaments.MsgSchedules.TeamArena.MatchesRegistered.Values.Where(p => p.dinamicID != 0).ToArray();
                int Page = (int)(aPage / 6);

                const int max = 6;
                int offset = Page * max;
                int count = Math.Min(max, Math.Max(0, info.Length - offset));

                stream.TeamArenaMatchesCreate(aPage, MsgTournaments.MsgArena.ArenaIDs.QualifierList, (uint)info.Length
                    , (uint)MsgTournaments.MsgSchedules.Arena.Registered.Count(), (uint)(info.Length - (aPage - 1)));

                for (int x = 0; x < count; x++)
                {
                    if (info.Length > offset + x)
                    {
                        var element = info[offset + x];

                        stream.AddItemTeamArenaMatches(element.Teams[0], element.Teams[1]);
                    }
                }
                stream.TeamArenaMatchesFinalize();
                user.Send(stream);
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
