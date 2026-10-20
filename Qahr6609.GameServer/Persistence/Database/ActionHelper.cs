using Extensions.Threading;
using Conquer6609.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public static class ActionHelper
    {
#region Actions
        public static Action<ServerSockets.Packet, Client.GameClient> LvlAction = null;
#endregion
        public static void Create()
        {
            LvlAction = new Action<ServerSockets.Packet, Client.GameClient>(Uplvl);
        }
#region Voids
        private static void Uplvl(ServerSockets.Packet stream, Client.GameClient client)
        {
            while (client.Player.Experience >= Database.Server.LevelInfo[Database.DBLevExp.Sort.User][(byte)client.Player.Level].Experience)
            {
                client.Player.Experience -= Database.Server.LevelInfo[Database.DBLevExp.Sort.User][(byte)client.Player.Level].Experience;
                ushort newlev = (ushort)(client.Player.Level + 1);
                client.UpdateLevel(stream, newlev);
                //if (client.Player.Level == 130)
                //{
                //    client.Player.Experience -= Database.Server.LevelInfo[Database.DBLevExp.Sort.User][(byte)client.Player.Level].Experience;
                //    ushort newlev2 = (ushort)(client.Player.Level + 10);
                //    client.UpdateLevel(stream, newlev2);
                //}
                if (client.Player.Level >= 140)
                {
                    client.Player.Experience = 0;
                    break;
                }
            }
        }
#endregion
    }
}
