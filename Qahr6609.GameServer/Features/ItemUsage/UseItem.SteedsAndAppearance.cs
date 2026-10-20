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
        // UseItem handlers: steeds, garments, hair styles / colors.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_SteedsAndAppearance(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {




                case 1060030:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.HairColor = 3;
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                        break;
                    }
                case 1060040:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.HairColor = 9;
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                        break;
                    }
                case 1060050:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.HairColor = 8;
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                        break;
                    }
                case 1060060:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.HairColor = 7;
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                        break;
                    }
                case 1060070:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.HairColor = 6;
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                        break;
                    }
                case 1060080:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.HairColor = 5;
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                        break;
                    }
                case 1060090:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.HairColor = 4;
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                        break;
                    }
                case 750000:
                    {
                        if (client.DemonExterminator != null)
                        {
                            client.SendSysMesage("You currently have " + client.DemonExterminator.HuntKills + "KOs of the required " + item.Durability + ". Please keep in mind the jar will sometimes list incorrect amounts.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);

                        }
                        break;
                    }
                case 723855:// +1MaroonSteedPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 0, 150, 255);
                        client.SendSysMesage("You received (+1) MaroonSteed.", MsgMessage.ChatMode.System);

                        break;
                    }
                case 723856:// +1WhiteSteedPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 150, 255, 0);
                        client.SendSysMesage("You received (+1) WhiteSteed.", MsgMessage.ChatMode.System);

                        break;
                    }
                case 723859:// +1BlackSteedPack 
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 255, 0, 150);
                        client.SendSysMesage("You received (+1) BlackSteed.", MsgMessage.ChatMode.System);

                        break;
                    }

                case 723860:// +3MaroonSteedPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 0, 150, 255);
                        client.SendSysMesage("You received (+3) MaroonSteed.", MsgMessage.ChatMode.System);

                        break;
                    }
                case 723861:// +3WhiteSteedPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 150, 255, 0);
                        client.SendSysMesage("You received (+3) WhiteSteed.", MsgMessage.ChatMode.System);

                        break;
                    }
                case 723862:// +3BlackSteedPack 
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 255, 0, 150);
                        client.SendSysMesage("You received (+3) BlackSteed.", MsgMessage.ChatMode.System);

                        break;
                    }

                case 723863:// +6MaroonSteedPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 0, 150, 255);
                        client.SendSysMesage("You received (+6) MaroonSteed.", MsgMessage.ChatMode.System);

                        break;
                    }
                case 723864:// +6WhiteSteedPack
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 150, 255, 0);
                        client.SendSysMesage("You received (+6) WhiteSteed.", MsgMessage.ChatMode.System);

                        break;
                    }
                case 723865:// +6BlackSteedPack 
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 255, 0, 150);
                        client.SendSysMesage("You received (+6) BlackSteed.", MsgMessage.ChatMode.System);

                        break;
                    }

                case 3001065: //+3BlackSteedPack
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 255, 0, 150);
                        client.CreateBoxDialog("You have received  +3 Black Steed(Bound).");
                        break;
                    }


                case 3001060: //+1MaroonSteedPack 
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 0, 150, 255);
                        client.CreateBoxDialog("You have received  +1 Maroon Steed(Bound).");
                        break;
                    }


                case 3001061: //+1WhiteSteedPack 
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 150, 255, 0);
                        client.CreateBoxDialog("You have received  +1 White Steed(Bound).");
                        break;
                    }




                case 3001062: //+1BlackSteedPack
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 255, 0, 150);
                        client.CreateBoxDialog("You have received  +1 Black Steed(Bound).");
                        break;
                    }


                case 3001063: //+3MaroonSteedPack 
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 0, 150, 255);
                        client.CreateBoxDialog("You have received  +3 Maroon Steed(Bound).");
                        break;
                    }



                case 3001064: //+3WhiteSteedPack 
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 150, 255, 0);
                        client.CreateBoxDialog("You have received  +3 White Steed(Bound).");
                        break;
                    }
                case 3300036:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3300036, 50))
                        {
                            client.Inventory.Remove(3300036, 50, stream);
                            client.Inventory.Add(stream, 200575);
                            client.CreateBoxDialog("You've received a AstralPhoenix!");
                        }
                        else
                        {
                            client.SendSysMesage("You should have 50 AstralPhoenix Fragments before you can make a complete mount.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3006032:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 723701);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        client.CreateBoxDialog("Your~received~an~Exemption~Token!");
                        break;
                    }
                case 3007105:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3007105, 30))
                        {
                            client.Inventory.Remove(3007105, 30, stream);
                            var array = Database.ItemType.SteedMounts.Values.ToArray();
                            var _item = array[Program.GetRandom.Next(0, array.Length)];
                            Database.CoatStorage.StorageItem DBMount;
                            if (Database.CoatStorage.Mounts.TryGetValue(_item.ID, out DBMount))
                            {
                                client.Inventory.Add(stream, _item.ID, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received a 1%Blessed " + _item.Name + ".");
                            }
                        }
                        else
                        {
                            client.SendSysMesage("You should have 30 Mount Armor Fragments to combine them into a Mount Armor.", MsgMessage.ChatMode.System);
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
