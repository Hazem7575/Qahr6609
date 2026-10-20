using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgReincarnation
    {

        public static void GetReincarnation(this ServerSockets.Packet msg, out uint ToClass, out uint ToBody)
        {
            int timerstamp = msg.ReadInt32();

            ToClass = msg.ReadUInt32();
            ToBody = msg.ReadUInt32();
        }
        [PacketAttribute(Game.GamePackets.Reincarnation)]
        public unsafe static void Proces(Client.GameClient user, ServerSockets.Packet stream)
        {

            uint ToClass;
            uint ToBody;

            stream.GetReincarnation(out ToClass, out ToBody);


            if (ToClass == 11 || ToClass == 21 || ToClass == 41 || ToClass == 51 || ToClass == 61 || ToClass == 71 || ToClass == 81 || ToClass == 132 || ToClass == 142 || ToClass == 161)
            {
                if (user.Inventory.Contain(711083, 1))
                {
                    if (user.Player.Level >= 110 && user.Player.Reborn == 2)
                    {
                        if (Role.Core.IsBoy(user.Player.Body))
                        {
                            switch (ToBody)
                            {
                                case 3:
                                    user.Player.Body = 1003;
                                    break;
                                case 4:
                                    user.Player.Body = 1004;
                                    break;
                            }
                        }
                        else
                        {
                            switch (ToBody)
                            {
                                case 1:
                                    user.Player.Body = 2001;
                                    break;
                                case 2:
                                    user.Player.Body = 2002;
                                    break;
                            }
                        }
                        user.Inventory.Remove(711083, 1, stream);
                        Database.Server.RebornInfo.Reborn(user.Player, (byte)ToClass, stream);
                    }
                    else
                        user.CreateBoxDialog("You have not been Reborn twice or you are not level 110 ++");
                }
            }

        }
    }
}