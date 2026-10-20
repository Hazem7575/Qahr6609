using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CheckFloors
    {
        public static bool CheckGuildWar(Client.GameClient client, int newx, int newy)
        {
            if (client.Player.Map == 1038)
            {

                if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidJump(client.TerainMask, out client.TerainMask, (ushort)newx, (ushort)newy))
                {
                    client.SendSysMesage("Illegal jumping over the gates detected.");
                    client.Pullback();
                    return false;
                }
            }
            return true;
        }
        public static bool CheckSuperGuildWar(Client.GameClient client, int newx, int newy)
        {
            return true;
        }
    }
}
