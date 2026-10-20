using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetAdvertiseOpenGui(this ServerSockets.Packet stream, out MsgAdvertiseOpenGui.Action Mode, out uint GuildId)
        {
            Mode = (MsgAdvertiseOpenGui.Action) stream.ReadUInt32();
            GuildId = stream.ReadUInt32();
        }

    }
    public struct MsgAdvertiseOpenGui
    {
        public enum Action : uint
        {
            Join = 1,
            Open = 2,
        }

        [PacketAttribute(GamePackets.ReceiveRecruit)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
              Action Mode;
             uint GuildID;
             stream.GetAdvertiseOpenGui(out Mode, out GuildID);

            switch (Mode)
            {
                case Action.Open:
                    {

                        user.Send(stream.AdvertiseGuiCreate());
                        break;
                    }
                case Action.Join:
                    {
                        if (user.Player.GuildID == 0 && user.Player.MyGuild == null)
                        {
                            Role.Instance.Guild guild;
                            if (Role.Instance.Guild.GuildPoll.TryGetValue(GuildID, out guild))
                            {
                                // Auto-join only into guilds that are currently advertising with auto-join enabled.
                                if (guild.UseAdvertise && Role.Instance.Guild.Advertise.AGuilds.ContainsKey(guild.Info.GuildID)
                                    && guild.AdvertiseRecruit.AutoJoin && !guild.Members.ContainsKey(user.Player.UID))
                                {
                                    if (guild.AdvertiseRecruit.Compare(user.Player, Role.Instance.Guild.Recruitment.Mode.Recruit))
                                        guild.AddPlayer(user.Player,stream);
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
