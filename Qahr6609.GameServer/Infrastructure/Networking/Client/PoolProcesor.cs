using Conquer6609.Database;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgLoader;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.AttackHandler;
using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Client
{
    public partial class PoolProcesses
    {
        public static unsafe void CheckJiangHu(Client.GameClient client)
        {
            try
            {
               

                if (client.Player.MyJiangHu != null)
                {
                    client.Player.MyJiangHu.CheckStatus(client);
                }

            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
    }
}