using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanAttackMonster
    {
        public static bool Verified(Client.GameClient client, MsgMonster.MonsterRole attacked
            , Database.MagicType.Magic DBSpell)
        {
            if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Reviver) == MsgMonster.MonsterSettings.Reviver)
                return false;

            if (!attacked.Alive)
            {
                client.OnAutoAttack = false;
                return false;
            }
            if (!MsgTournaments.MsgSchedules.DragonIsland.Attackable(attacked.Map, attacked.X, attacked.Y))
                return false;
            if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
            {
                if (client.Player.PkMode != Role.Flags.PKMode.PK || client.Player.Map == 1002)
                    return false;
                else
                {
                    client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 30, true);
                }
            }
            return true;
        }
    }
}
