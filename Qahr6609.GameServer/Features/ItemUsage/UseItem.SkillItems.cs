using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {
        // UseItem handlers: items that teach a skill.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_SkillItems(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 725016:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1360, 0, 0, 0, 0);
                        break;
                    }
                case 725018:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1380, 0, 0, 0, 0);
                        break;
                    }
                case 725019:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1385, 0, 0, 0, 0);
                        break;
                    }
                case 725020:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1390, 0, 0, 0, 0);
                        break;
                    }
                case 725021:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1395, 0, 0, 0, 0);
                        break;
                    }
                case 725022:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1400, 0, 0, 0, 0);
                        break;
                    }
                case 725023:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1405, 0, 0, 0, 0);
                        break;
                    }
                case 725024:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1410, 0, 0, 0, 0);
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
