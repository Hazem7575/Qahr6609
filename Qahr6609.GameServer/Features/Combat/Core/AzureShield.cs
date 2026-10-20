using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.Calculate
{
   public class AzureShield
    {
       public unsafe static void CreateDmg(Role.Player player, Role.Player target, uint DMG)
       {
           using (var rec = new ServerSockets.RecycledPacket())
           {
               var stream = rec.GetStream();

               InteractQuery action = new InteractQuery()
               {
                   AtkType = MsgAttackPacket.AttackID.BlueDamage,
                   UID = player.UID,
                   OpponentUID = target.UID,
                   X = target.X,
                   Y = target.Y,
                   Damage = (int)DMG
               };

               target.View.SendView(stream.InteractionCreate(&action), true);
           }
       }
    }
}
