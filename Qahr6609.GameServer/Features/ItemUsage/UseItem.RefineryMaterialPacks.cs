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
        // UseItem handlers: Prime packs and (Superior) Refinery Packs.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_RefineryMaterialPacks(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 3004280:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721423, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721424, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004295, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004279:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721393, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721394, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004294, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004278:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721376, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721377, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004293, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004277:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721383, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721384, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004292, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004276:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721351, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721352, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004291, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004275:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721496, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721497, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004290, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004274:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721491, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721492, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004289, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004273:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721486, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721487, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004288, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004272:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721361, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721362, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004287, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004271:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721462, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721463, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004286, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004270:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721457, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721458, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004285, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004269:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721452, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721453, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004284, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004268:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721413, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721414, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004283, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004267:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721408, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721409, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004282, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004266:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721403, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Elite)~Material~(B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721404, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Super)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004281, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Sacred)~Material~(B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004226:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725210, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725211, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004155, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004225:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725205, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725206, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004157, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004224:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725200, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725201, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004146, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004223:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725195, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725196, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~M-Defense~Pack~and~received~1~M-Defense~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004222:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724438, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Immunity~Pack~and~received~1~Immunity~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724439, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Immunity~Pack~and~received~1~Immunity~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004149, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Immunity~Pack~and~received~1~Immunity~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004221:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724371, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724372, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004151, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004220:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724481, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724482, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004160, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004219:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724366, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724367, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004164, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004218:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724471, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724472, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004163, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004217:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724428, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724429, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004148, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004216:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724418, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724419, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004158, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004215:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724388, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724389, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004154, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004214:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724356, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724357, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004144, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004213:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724476, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724477, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004142, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Counteraction~Pack~and~received~1~Counteraction~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004212:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724443, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Intensification~Pack~and~received~1~Intensification~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724444, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Intensification~Pack~and~received~1~Intensification~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004140, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Intensification~Pack~and~received~1~Intensification~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004211:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724433, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Immunity~Pack~and~received~1~Immunity~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724434, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Immunity~Pack~and~received~1~Immunity~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Immunity~Pack~and~received~1~Immunity~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004210:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724376, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724377, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004166, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004209:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724383, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724384, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004145, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Penetration~Pack~and~received~1~Penetration~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }

                        break;
                    }
                case 3004208:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724351, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724352, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004165, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004207:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724496, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724497, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004152, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004206:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724491, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724492, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004161, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004205:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724486, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724487, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004143, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Detoxication~Pack~and~received~1~Detoxication~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004204:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724361, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724362, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004153, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Block~Pack~and~received~1~Block~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004203:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724462, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724463, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004150, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004202:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724457, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724458, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004159, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004201:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724452, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724453, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004141, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Breakthrough~Pack~and~received~1~Breakthrough~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }

                        break;
                    }
                case 3004200:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724413, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724414, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004156, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004199:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724408, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724409, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004137, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004198:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724403, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724404, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004147, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Critical~Strike~Pack~and~received~1~Critical~Strike~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3004197:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724423, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Elite)~Material.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724424, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Super)~Material.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3004138, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Prime~Skill~C.Strike~Pack~and~received~1~Skill~C.Strike~(Sacred)~Material.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 729594://gold exorcism
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            client.Inventory.Remove(729594, 1, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e280");
                            if (Role.Core.Rate(4, 10))
                            {
                                if (Role.Core.Rate(1, 8))
                                {
                                    client.Inventory.Add(stream, 350004, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                    client.SendSysMesage("You opened the pack, and received a Dragon Bone!");
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 7))
                                    {
                                        client.Inventory.Add(stream, 350010, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.SendSysMesage("You opened the pack, and received a Hockey Stick!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 6))
                                        {
                                            client.Inventory.Add(stream, 360002, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                            client.SendSysMesage("You opened the pack, and received a Wooden Club!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 5))
                                            {
                                                client.Inventory.Add(stream, 360006, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                client.SendSysMesage("You opened the pack, and received a Spatula!");
                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 4))
                                                {
                                                    client.Inventory.Add(stream, 360010, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                    client.SendSysMesage("You opened the pack, and received a Handbag!");
                                                }
                                                else
                                                {
                                                    if (Role.Core.Rate(1, 3))
                                                    {
                                                        client.Inventory.Add(stream, 360012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                        client.SendSysMesage("You opened the pack, and received a Sports Bag!");
                                                    }
                                                    else
                                                    {
                                                        if (Role.Core.Rate(1, 2))
                                                        {
                                                            client.Inventory.Add(stream, 380001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                            client.SendSysMesage("You opened the pack, and received a Wok!");
                                                        }
                                                        else
                                                        {
                                                            client.Inventory.Add(stream, 370005, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                            client.SendSysMesage("You opened the pack, and received a Force Bow!");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                client.Inventory.Add(stream, 720957, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.Inventory.Add(stream, 728918, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You~opened~the~Golden~Exorcism~Pack,~and~received~a~precious~Dragon~Soul~pack,~and~a~Refinery~Ticket!");

                                client.Player.BoundConquerPoints += 50;
                                client.SendSysMesage("You opened the Golden Exorcism Pack, and received 50 CPs (B)!");
                                client.Inventory.Add(stream, 729304, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You received 1 Chi Token!");
                            }
                        }
                        else
                        {
                            client.SendSysMesage("Your~inventory~is~full.~You~need~to~make~at~least~3~empty~spaces~first,~and~then~open~the~pack~again.");
                        }
                        break;
                    }

                case 728918:
                    {
                        client.Inventory.Remove(728918, 1, stream);
                        if (Role.Core.Rate(2, 10))
                        {
                            client.Inventory.Add(stream, 720399);
                            client.CreateBoxDialog("You~received~a~Superior~Refinery~Pack!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e104");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 720549);
                            client.CreateBoxDialog("You~received~a~Refinery~Pack!");

                        }
                        break;
                    }
                case 720549:
                    {
                        client.Inventory.Remove(720549, 1, stream);
                        if (Role.Core.Rate(1, 10))
                        {
                            if (Role.Core.Rate(1, 27))
                            {
                                client.Inventory.Add(stream, 724403);
                                client.SendSysMesage("You opened a Refinery Pack and received an Elite Critical Strike material (Bow).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                if (Role.Core.Rate(1, 26))
                                {
                                    client.Inventory.Add(stream, 724408);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Critical Strike material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 25))
                                    {
                                        client.Inventory.Add(stream, 724413);
                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Critical Strike material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 3))
                                        {
                                            switch (Program.GetRandom.Next(0, 8))
                                            {
                                                case 0:
                                                    {
                                                        client.Inventory.Add(stream, 724418);
                                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Critical Strike material (Ring).", MsgMessage.ChatMode.System);
                                                        break;
                                                    }
                                                case 1:
                                                    {
                                                        client.Inventory.Add(stream, 724423);
                                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Skill Critical Strike material (Backsword).", MsgMessage.ChatMode.System);
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        client.Inventory.Add(stream, 724428);
                                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Skill Critical Strike material (Bracelet).", MsgMessage.ChatMode.System);
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        client.Inventory.Add(stream, 724433);
                                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Immunity material (Armor).", MsgMessage.ChatMode.System);
                                                        break;
                                                    }
                                                case 4:
                                                    {
                                                        client.Inventory.Add(stream, 724438);
                                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Immunity material (Boots).", MsgMessage.ChatMode.System);
                                                        break;
                                                    }
                                                case 5:
                                                    {
                                                        client.Inventory.Add(stream, 724443);
                                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Intensification material (Headgear).", MsgMessage.ChatMode.System);
                                                        break;
                                                    }
                                                case 6:
                                                    {
                                                        client.Inventory.Add(stream, 724452);
                                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                                        break;
                                                    }
                                                case 7:
                                                    {
                                                        client.Inventory.Add(stream, 724457);
                                                        client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                                        break;
                                                    }
                                            }

                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 2))
                                            {
                                                switch (Program.GetRandom.Next(0, 8))
                                                {
                                                    case 0:
                                                        {
                                                            client.Inventory.Add(stream, 724462);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (Bow).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 1:
                                                        {
                                                            client.Inventory.Add(stream, 724471);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (Ring).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 2:
                                                        {
                                                            client.Inventory.Add(stream, 724476);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Counteraction material (Armor).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            client.Inventory.Add(stream, 724481);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Counteraction material (Necklace).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 4:
                                                        {
                                                            client.Inventory.Add(stream, 724486);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Headgear).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 5:
                                                        {
                                                            client.Inventory.Add(stream, 724491);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Armor).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 6:
                                                        {
                                                            client.Inventory.Add(stream, 724496);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Boots).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 7:
                                                        {
                                                            client.Inventory.Add(stream, 724351);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Necklace).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                }

                                            }
                                            else
                                            {
                                                switch (Program.GetRandom.Next(0, 8))
                                                {
                                                    case 0:
                                                        {
                                                            client.Inventory.Add(stream, 724356);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Block material (Headgear).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 1:
                                                        {
                                                            client.Inventory.Add(stream, 724361);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Block material (Shield).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 2:
                                                        {
                                                            client.Inventory.Add(stream, 724366);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (Bracelet).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            client.Inventory.Add(stream, 724371);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Counteraction material (Bag).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 4:
                                                        {
                                                            client.Inventory.Add(stream, 724376);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Bag).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 5:
                                                        {
                                                            client.Inventory.Add(stream, 724383);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Penetration material (Headgear).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 6:
                                                        {
                                                            client.Inventory.Add(stream, 724388);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Penetration material (Bag).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 7:
                                                        {
                                                            client.Inventory.Add(stream, 724393);
                                                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Penetration material (Bracelet).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Role.Core.Rate(4, 9))
                            {
                                if (Role.Core.Rate(1, 27))
                                {
                                    client.Inventory.Add(stream, 724402);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Critical Strike material (Bow).", MsgMessage.ChatMode.System);
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 26))
                                    {
                                        client.Inventory.Add(stream, 724407);
                                        client.SendSysMesage("You opened a Refinery Pack and received a Unique Critical Strike material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 25))
                                        {
                                            client.Inventory.Add(stream, 724412);
                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Critical Strike material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 3))
                                            {
                                                switch (Program.GetRandom.Next(0, 8))
                                                {
                                                    case 0:
                                                        {
                                                            client.Inventory.Add(stream, 724417);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Critical Strike material (Ring).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 1:
                                                        {
                                                            client.Inventory.Add(stream, 724422);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Skill Critical Strike material (Backsword).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 2:
                                                        {
                                                            client.Inventory.Add(stream, 724427);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Skill Critical Strike material (Bracelet).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            client.Inventory.Add(stream, 724432);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Immunity material (Armor).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 4:
                                                        {
                                                            client.Inventory.Add(stream, 724437);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Immunity material (Boots).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 5:
                                                        {
                                                            client.Inventory.Add(stream, 724442);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Intensification material (Headgear).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 6:
                                                        {
                                                            client.Inventory.Add(stream, 724451);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 7:
                                                        {
                                                            client.Inventory.Add(stream, 724456);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                }

                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 2))
                                                {
                                                    switch (Program.GetRandom.Next(0, 8))
                                                    {
                                                        case 0:
                                                            {
                                                                client.Inventory.Add(stream, 724461);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (Bow).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                client.Inventory.Add(stream, 724470);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (Ring).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                client.Inventory.Add(stream, 724475);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Counteraction material (Armor).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                client.Inventory.Add(stream, 724480);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Counteraction material (Necklace).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                client.Inventory.Add(stream, 724485);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                client.Inventory.Add(stream, 724490);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Armor).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                client.Inventory.Add(stream, 724495);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Boots).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                client.Inventory.Add(stream, 724350);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Necklace).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                    }

                                                }
                                                else
                                                {
                                                    switch (Program.GetRandom.Next(0, 8))
                                                    {
                                                        case 0:
                                                            {
                                                                client.Inventory.Add(stream, 724355);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Block material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                client.Inventory.Add(stream, 724360);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Block material (Shield).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                client.Inventory.Add(stream, 724365);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (Bracelet).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                client.Inventory.Add(stream, 724370);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Counteraction material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                client.Inventory.Add(stream, 724375);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                client.Inventory.Add(stream, 724382);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Penetration material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                client.Inventory.Add(stream, 724387);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Penetration material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                client.Inventory.Add(stream, 724392);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Unique Penetration material (Bracelet).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Role.Core.Rate(1, 27))
                                {
                                    client.Inventory.Add(stream, 724401);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Refined Critical Strike material (Bow).", MsgMessage.ChatMode.System);
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 26))
                                    {
                                        client.Inventory.Add(stream, 724406);
                                        client.SendSysMesage("You opened a Refinery Pack and received a Refined Critical Strike material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 25))
                                        {
                                            client.Inventory.Add(stream, 724411);
                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Critical Strike material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 3))
                                            {
                                                switch (Program.GetRandom.Next(0, 8))
                                                {
                                                    case 0:
                                                        {
                                                            client.Inventory.Add(stream, 724416);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Critical Strike material (Ring).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 1:
                                                        {
                                                            client.Inventory.Add(stream, 724421);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Skill Critical Strike material (Backsword).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 2:
                                                        {
                                                            client.Inventory.Add(stream, 724426);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Skill Critical Strike material (Bracelet).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            client.Inventory.Add(stream, 724431);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Immunity material (Armor).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 4:
                                                        {
                                                            client.Inventory.Add(stream, 724436);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Immunity material (Boots).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 5:
                                                        {
                                                            client.Inventory.Add(stream, 724441);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Intensification material (Headgear).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 6:
                                                        {
                                                            client.Inventory.Add(stream, 724450);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 7:
                                                        {
                                                            client.Inventory.Add(stream, 724455);
                                                            client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                }

                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 2))
                                                {
                                                    switch (Program.GetRandom.Next(0, 8))
                                                    {
                                                        case 0:
                                                            {
                                                                client.Inventory.Add(stream, 724460);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (Bow).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                client.Inventory.Add(stream, 724465);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (Ring).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                client.Inventory.Add(stream, 724474);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Counteraction material (Armor).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                client.Inventory.Add(stream, 724479);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Counteraction material (Necklace).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                client.Inventory.Add(stream, 724484);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                client.Inventory.Add(stream, 724489);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Armor).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                client.Inventory.Add(stream, 724494);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Boots).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                client.Inventory.Add(stream, 724499);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Necklace).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                    }

                                                }
                                                else
                                                {
                                                    switch (Program.GetRandom.Next(0, 8))
                                                    {
                                                        case 0:
                                                            {
                                                                client.Inventory.Add(stream, 724354);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Block material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                client.Inventory.Add(stream, 724359);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Block material (Shield).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                client.Inventory.Add(stream, 724364);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (Bracelet).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                client.Inventory.Add(stream, 724369);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Counteraction material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                client.Inventory.Add(stream, 724374);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                client.Inventory.Add(stream, 724381);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Penetration material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                client.Inventory.Add(stream, 724386);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Penetration material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                client.Inventory.Add(stream, 724391);
                                                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Penetration material (Bracelet).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

                case 720399:
                    {
                        client.Inventory.Remove(720399, 1, stream);
                        if (Role.Core.Rate(3, 10))
                        {
                            if (Role.Core.Rate(1, 28))
                            {
                                client.Inventory.Add(stream, 724404);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Critical Strike material (Bow).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                if (Role.Core.Rate(1, 27))
                                {
                                    client.Inventory.Add(stream, 724409);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Critical Strike material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 26))
                                    {
                                        client.Inventory.Add(stream, 724414);
                                        client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Critical Strike material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 25))
                                        {
                                            client.Inventory.Add(stream, 3006169);
                                            client.SendSysMesage("You~received~a~Super~Hossu!", MsgMessage.ChatMode.System);
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 3))
                                            {
                                                switch (Program.GetRandom.Next(0, 8))
                                                {
                                                    case 0:
                                                        {
                                                            client.Inventory.Add(stream, 724419);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Critical Strike material (Ring).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 1:
                                                        {
                                                            client.Inventory.Add(stream, 724424);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Skill Critical Strike material (Backsword).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 2:
                                                        {
                                                            client.Inventory.Add(stream, 724429);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Skill Critical Strike material (Bracelet).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            client.Inventory.Add(stream, 724434);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Immunity material (Armor).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 4:
                                                        {
                                                            client.Inventory.Add(stream, 724439);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Immunity material (Boots).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 5:
                                                        {
                                                            client.Inventory.Add(stream, 724444);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Intensification material (Headgear).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 6:
                                                        {
                                                            client.Inventory.Add(stream, 724453);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 7:
                                                        {
                                                            client.Inventory.Add(stream, 724458);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                }

                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 2))
                                                {
                                                    switch (Program.GetRandom.Next(0, 8))
                                                    {
                                                        case 0:
                                                            {
                                                                client.Inventory.Add(stream, 724463);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (Bow).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                client.Inventory.Add(stream, 724472);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (Ring).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                client.Inventory.Add(stream, 724477);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Counteraction material (Armor).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                client.Inventory.Add(stream, 724482);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Counteraction material (Necklace).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                client.Inventory.Add(stream, 724487);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                client.Inventory.Add(stream, 724492);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Armor).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                client.Inventory.Add(stream, 724497);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Boots).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                client.Inventory.Add(stream, 724352);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Necklace).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                    }

                                                }
                                                else
                                                {
                                                    switch (Program.GetRandom.Next(0, 8))
                                                    {
                                                        case 0:
                                                            {
                                                                client.Inventory.Add(stream, 724357);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Block material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                client.Inventory.Add(stream, 724362);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Block material (Shield).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                client.Inventory.Add(stream, 724367);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (Bracelet).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                client.Inventory.Add(stream, 724372);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Counteraction material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                client.Inventory.Add(stream, 724377);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                client.Inventory.Add(stream, 724384);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Penetration material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                client.Inventory.Add(stream, 724389);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Penetration material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                client.Inventory.Add(stream, 724394);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Penetration material (Bracelet).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Role.Core.Rate(1, 28))
                            {
                                client.Inventory.Add(stream, 724403);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Critical Strike material (Bow).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                if (Role.Core.Rate(1, 27))
                                {
                                    client.Inventory.Add(stream, 724408);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Critical Strike material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 26))
                                    {
                                        client.Inventory.Add(stream, 724413);
                                        client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Critical Strike material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 25))
                                        {
                                            client.Inventory.Add(stream, 3006168);
                                            client.SendSysMesage("You~received~an~Elite~Hossu!", MsgMessage.ChatMode.System);
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 3))
                                            {
                                                switch (Program.GetRandom.Next(0, 8))
                                                {
                                                    case 0:
                                                        {
                                                            client.Inventory.Add(stream, 724418);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Critical Strike material (Ring).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 1:
                                                        {
                                                            client.Inventory.Add(stream, 724423);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Skill Critical Strike material (Backsword).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 2:
                                                        {
                                                            client.Inventory.Add(stream, 724428);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Skill Critical Strike material (Bracelet).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            client.Inventory.Add(stream, 724433);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Immunity material (Armor).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 4:
                                                        {
                                                            client.Inventory.Add(stream, 724438);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Immunity material (Boots).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 5:
                                                        {
                                                            client.Inventory.Add(stream, 724443);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Intensification material (Headgear).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 6:
                                                        {
                                                            client.Inventory.Add(stream, 724452);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (1-Handed Weapon).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                    case 7:
                                                        {
                                                            client.Inventory.Add(stream, 724457);
                                                            client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (2-Handed Weapon).", MsgMessage.ChatMode.System);
                                                            break;
                                                        }
                                                }

                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 2))
                                                {
                                                    switch (Program.GetRandom.Next(0, 8))
                                                    {
                                                        case 0:
                                                            {
                                                                client.Inventory.Add(stream, 724462);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (Bow).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                client.Inventory.Add(stream, 724471);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (Ring).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                client.Inventory.Add(stream, 724476);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Counteraction material (Armor).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                client.Inventory.Add(stream, 724481);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Counteraction material (Necklace).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                client.Inventory.Add(stream, 724486);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                client.Inventory.Add(stream, 724491);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Armor).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                client.Inventory.Add(stream, 724496);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Boots).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                client.Inventory.Add(stream, 724351);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Necklace).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                    }

                                                }
                                                else
                                                {
                                                    switch (Program.GetRandom.Next(0, 8))
                                                    {
                                                        case 0:
                                                            {
                                                                client.Inventory.Add(stream, 724356);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Block material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                client.Inventory.Add(stream, 724361);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Block material (Shield).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                client.Inventory.Add(stream, 724366);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (Bracelet).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                client.Inventory.Add(stream, 724371);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Counteraction material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                client.Inventory.Add(stream, 724376);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                client.Inventory.Add(stream, 724383);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Penetration material (Headgear).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                client.Inventory.Add(stream, 724388);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Penetration material (Bag).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                client.Inventory.Add(stream, 724393);
                                                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Penetration material (Bracelet).", MsgMessage.ChatMode.System);
                                                                break;
                                                            }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
