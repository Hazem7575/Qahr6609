using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgFloorItem;
using Project2D.Database;
using Conquer6609.Game.MsgTournaments;


namespace Conquer6609.Game.MsgNpc
{
   

    public unsafe partial class NpcHandler
    {
        /// <summary>True when the inventory holds count of the item (bound or unbound), as required by Inventory.Remove.</summary>
        static bool HasItems(Client.GameClient client, uint itemId, uint count)
        {
            return client.Inventory.Contain(itemId, count) || client.Inventory.Contain(itemId, count, 1);
        }

    }
}
