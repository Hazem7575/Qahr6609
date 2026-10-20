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
        // UseItem handlers: the "Refinery Packs" region (refinery material packs by attribute).
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_RefineryPacks(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                #region Refinery Packs

                case 724186:
                    {
                        client.Inventory.Remove(724186, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724382);
                            client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Unique Hemstitch.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724383);
                                client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Elite Hemstitch.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724384);
                                client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Super Hemstitch.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724171:
                    {
                        client.Inventory.Remove(724171, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724360);
                            client.SendSysMesage("You opened the Superior Pin Pack and got the Unique Pin.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724361);
                                client.SendSysMesage("You opened the Superior Pin Pack and got the Elite Pin.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724362);
                                client.SendSysMesage("You opened the Superior Pin Pack and got the Super Pin.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724147:
                    {
                        client.Inventory.Remove(724147, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724355);
                            client.SendSysMesage("You opened the Superior Hat Rope Pack and got the Unique Hat Rope.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724356);
                                client.SendSysMesage("You opened the Superior Hat Rope Pack and got the Elite Hat Rope.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724357);
                                client.SendSysMesage("You opened the Superior Hat Rope Pack and got the Super Hat Rope.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724177:
                    {
                        client.Inventory.Remove(724177, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724490);
                            client.SendSysMesage("You opened the Superior Belt Pack and got the Unique Belt.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724491);
                                client.SendSysMesage("You opened the Superior Belt Pack and got the Elite Belt.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724492);
                                client.SendSysMesage("You opened the Superior Belt Pack and got the Super Belt.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724174:
                    {
                        client.Inventory.Remove(724174, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724485);
                            client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Unique Tri Plume.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724486);
                                client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Elite Tri Plume.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724487);
                                client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Super Tri Plume.", MsgMessage.ChatMode.System);
                            }
                        }

                        break;
                    }
                case 724144:
                    {
                        client.Inventory.Remove(724144, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724475);
                            client.SendSysMesage("You opened the Superior Breastplate Pack and got the Unique Breastplate.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724476);
                                client.SendSysMesage("You opened the Superior Breastplate Pack and got the Elite Breastplate.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724477);
                                client.SendSysMesage("You opened the Superior Breastplate Pack and got the Super Breastplate.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724165:
                    {
                        client.Inventory.Remove(724165, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724456);
                            client.SendSysMesage("You opened the Superior Cauterant Pack and got the Unique Cauterant.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724457);
                                client.SendSysMesage("You opened the Superior Cauterant Pack and got the Elite Cauterant.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724458);
                                client.SendSysMesage("You opened the Superior Cauterant Pack and got the Super Cauterant.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724168:
                    {
                        client.Inventory.Remove(724168, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724461);
                            client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Unique Bow Limb.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724462);
                                client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Elite Bow Limb.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724463);
                                client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Super Bow Limb.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724162:
                    {
                        client.Inventory.Remove(724162, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724451);
                            client.SendSysMesage("You opened the Superior Cutter Pack and got the Unique Cutter.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724452);
                                client.SendSysMesage("You opened the Superior Cutter Pack and got the Elite Cutter.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724453);
                                client.SendSysMesage("You opened the Superior Cutter Pack and got the Super Cutter.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724141:
                    {
                        client.Inventory.Remove(724141, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724442);
                            client.SendSysMesage("You opened the Superior Visor Pack and got the Unique Visor.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724443);
                                client.SendSysMesage("You opened the Superior Visor Pack and got the Elite Visor.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724444);
                                client.SendSysMesage("You opened the Superior Visor Pack and got the Super Visor.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724138:
                    {

                        client.Inventory.Remove(724138, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724432);
                            client.SendSysMesage("You opened the Superior Plate Pack and got the Unique Plate.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724433);
                                client.SendSysMesage("You opened the Superior Plate Pack and got the Elite Plate.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724434);
                                client.SendSysMesage("You opened the Superior Plate Pack and got the Super Plate.", MsgMessage.ChatMode.System);
                            }
                        }

                        break;
                    }
                case 724135:
                    {
                        client.Inventory.Remove(724135, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724422);
                            client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Unique Long Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724423);
                                client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Elite Long Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724424);
                                client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Super Long Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724159:
                    {
                        client.Inventory.Remove(724159, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724412);
                            client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Unique Red Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724413);
                                client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Elite Red Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724414);
                                client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Super Red Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724153:
                    {
                        client.Inventory.Remove(724153, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724402);
                            client.SendSysMesage("You opened the Superior Bowstring Pack and got the Unique Bowstring.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724403);
                                client.SendSysMesage("You opened the Superior Bowstring Pack and got the Elite Bowstring.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724404);
                                client.SendSysMesage("You opened the Superior Bowstring Pack and got the Super Bowstring.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724156:
                    {
                        client.Inventory.Remove(724156, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724407);
                            client.SendSysMesage("You opened the Superior Veneer Pack and got the Unique Veneer.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724408);
                                client.SendSysMesage("You opened the Superior Veneer Pack and got the Elite Veneer.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724409);
                                client.SendSysMesage("You opened the Superior Veneer Pack and got the Super Veneer.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }


                case 724131:
                    {
                        client.Inventory.Remove(724131, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724391);
                            client.SendSysMesage("You opened the Medium Bell Pack and got the Refined Bell.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724392);
                                client.SendSysMesage("You opened the Medium Bell Pack and got the Unique Bell.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724393);
                                client.SendSysMesage("You opened the Medium Bell Pack and got the Elite Bell.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724185:
                    {
                        client.Inventory.Remove(724185, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724381);
                            client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Refined Hemstitch.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724382);
                                client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Unique Hemstitch.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724383);
                                client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Elite Hemstitch.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724170:
                    {
                        client.Inventory.Remove(724170, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724359);
                            client.SendSysMesage("You opened the Medium Pin Pack and got the Refined Pin.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724360);
                                client.SendSysMesage("You opened the Medium Pin Pack and got the Unique Pin.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724361);
                                client.SendSysMesage("You opened the Medium Pin Pack and got the Elite Pin.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724146:
                    {
                        client.Inventory.Remove(724146, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724354);
                            client.SendSysMesage("You opened the Medium Hat Rope Pack and got the Refined Hat Rope.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724355);
                                client.SendSysMesage("You opened the Medium Hat Rope Pack and got the Unique Hat Rope.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724356);
                                client.SendSysMesage("You opened the Medium Hat Rope Pack and got the Elite Hat Rope.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724176:
                    {
                        client.Inventory.Remove(724176, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724489);
                            client.SendSysMesage("You opened the Medium Belt Pack and got the Refined Belt.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724490);
                                client.SendSysMesage("You opened the Medium Belt Pack and got the Unique Belt.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724491);
                                client.SendSysMesage("You opened the Medium Belt Pack and got the Elite Belt.", MsgMessage.ChatMode.System);
                            }
                        }

                        break;
                    }
                case 724173:
                    {
                        client.Inventory.Remove(724173, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724484);
                            client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Refined Tri Plume.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724485);
                                client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Unique Tri Plume.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724486);
                                client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Elite Tri Plume.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724143:
                    {
                        client.Inventory.Remove(724143, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724474);
                            client.SendSysMesage("You opened the Medium Breastplate Pack and got the Refined Breastplate.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724475);
                                client.SendSysMesage("You opened the Medium Breastplate Pack and got the Unique Breastplate.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724476);
                                client.SendSysMesage("You opened the Medium Breastplate Pack and got the Elite Breastplate.", MsgMessage.ChatMode.System);
                            }
                        }

                        break;
                    }
                case 724164:
                    {
                        client.Inventory.Remove(724164, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724455);
                            client.SendSysMesage("You opened the Medium Cauterant Pack and got the Refined Cauterant.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724456);
                                client.SendSysMesage("You opened the Medium Cauterant Pack and got the Unique Cauterant.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724457);
                                client.SendSysMesage("You opened the Medium Cauterant Pack and got the Elite Cauterant.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724167:
                    {
                        client.Inventory.Remove(724167, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724460);
                            client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Refined Bow Limb.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724461);
                                client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Unique Bow Limb.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724462);
                                client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Elite Bow Limb.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724161:
                    {
                        client.Inventory.Remove(724161, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724450);
                            client.SendSysMesage("You opened the Medium Cutter Pack and got the Refined Cutter.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724451);
                                client.SendSysMesage("You opened the Medium Cutter Pack and got the Unique Cutter.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724452);
                                client.SendSysMesage("You opened the Medium Cutter Pack and got the Elite Cutter.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }

                case 724140:
                    {
                        client.Inventory.Remove(724140, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724441);
                            client.SendSysMesage("You opened the Medium Visor Pack and got the Refined Visor.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724442);
                                client.SendSysMesage("You opened the Medium Visor Pack and got the Unique Visor.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724443);
                                client.SendSysMesage("You opened the Medium Visor Pack and got the Elite Visor.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }

                case 724137:
                    {
                        client.Inventory.Remove(724137, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724431);
                            client.SendSysMesage("You opened the Medium Plate Pack and got the Refined Plate.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724432);
                                client.SendSysMesage("You opened the Medium Plate Pack and got the Unique Plate.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724433);
                                client.SendSysMesage("You opened the Medium Plate Pack and got the Elite Plate.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724134:
                    {
                        client.Inventory.Remove(724134, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724421);
                            client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Refined Long Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724422);
                                client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Unique Long Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724423);
                                client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Elite Long Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724158:
                    {
                        client.Inventory.Remove(724158, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724411);
                            client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Refined Red Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724412);
                                client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Unique Red Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724413);
                                client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Elite Red Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724152:
                    {
                        client.Inventory.Remove(724152, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724401);
                            client.SendSysMesage("You opened the Medium Bowstring Pack and got the Refined Bowstring.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724402);
                                client.SendSysMesage("You opened the Medium Bowstring Pack and got the Unique Bowstring.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724403);
                                client.SendSysMesage("You opened the Medium Bowstring Pack and got the Elite Bowstring.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724155:
                    {
                        client.Inventory.Remove(724155, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724406);
                            client.SendSysMesage("You opened the Medium Veneer Pack and got the Refined Veneer.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724407);
                                client.SendSysMesage("You opened the Medium Veneer Pack and got the Unique Veneer.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724408);
                                client.SendSysMesage("You opened the Medium Veneer Pack and got the Elite Veneer.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724130:
                    {
                        client.Inventory.Remove(724130, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724390);
                            client.SendSysMesage("You opened the Primary Bell Pack and got the Normal Bell.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724391);
                                client.SendSysMesage("You opened the Primary Bell Pack and got the Refined Bell.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724392);
                                client.SendSysMesage("You opened the Primary Bell Pack and got the Unique Bell.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724184:
                    {
                        client.Inventory.Remove(724184, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724380);
                            client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Normal Hemstitch.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724381);
                                client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Refined Hemstitch.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724382);
                                client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Unique Hemstitch.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724169:
                    {
                        client.Inventory.Remove(724169, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724358);
                            client.SendSysMesage("You opened the Primary Pin Pack and got the Normal Pin.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724359);
                                client.SendSysMesage("You opened the Primary Pin Pack and got the Refined Pin.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724360);
                                client.SendSysMesage("You opened the Primary Pin Pack and got the Unique Pin.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724145:
                    {
                        client.Inventory.Remove(724145, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724353);
                            client.SendSysMesage("You opened the Primary Hat Rope Pack and got the Normal Hat Rope.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724354);
                                client.SendSysMesage("You opened the Primary Hat Rope Pack and got the Refined Hat Rope.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724355);
                                client.SendSysMesage("You opened the Primary Hat Rope Pack and got the Unique Hat Rope.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724175:
                    {
                        client.Inventory.Remove(724175, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724488);
                            client.SendSysMesage("You opened the Primary Belt Pack and got the Normal Belt.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724489);
                                client.SendSysMesage("You opened the Primary Belt Pack and got the Refined Belt.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724490);
                                client.SendSysMesage("You opened the Primary Belt Pack and got the Unique Belt.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724172:
                    {
                        client.Inventory.Remove(724172, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724483);
                            client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Normal Tri Plume.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724484);
                                client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Refined Tri Plume.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724485);
                                client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Unique Tri Plume.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;

                    }
                case 724142:
                    {
                        client.Inventory.Remove(724142, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724473);
                            client.SendSysMesage("You opened the Primary Breastplate Pack and got the Normal Breastplate.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724474);
                                client.SendSysMesage("You opened the Primary Breastplate Pack and got the Refined Breastplate.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724475);
                                client.SendSysMesage("You opened the Primary Breastplate Pack and got the Unique Breastplate.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724163:
                    {
                        client.Inventory.Remove(724163, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724454);
                            client.SendSysMesage("You opened the Primary Cauterant Pack and got the Normal Cauterant.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724455);
                                client.SendSysMesage("You opened the Primary Cauterant Pack and got the Refined Cauterant.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724456);
                                client.SendSysMesage("You opened the Primary Cauterant Pack and got the Unique Cauterant.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724166:
                    {
                        client.Inventory.Remove(724166, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724459);
                            client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Normal Bow Limb.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724460);
                                client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Refined Bow Limb.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724461);
                                client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Unique Bow Limb.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724160:
                    {
                        client.Inventory.Remove(724160, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724445);
                            client.SendSysMesage("You opened the Primary Cutter Pack and got the Normal Cutter.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724450);
                                client.SendSysMesage("You opened the Primary Cutter Pack and got the Refined Cutter.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724451);
                                client.SendSysMesage("You opened the Primary Cutter Pack and got the Unique Cutter.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724139:
                    {
                        client.Inventory.Remove(724139, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724440);
                            client.SendSysMesage("You opened the Primary Visor Pack and got the Normal Visor.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724441);
                                client.SendSysMesage("You opened the Primary Visor Pack and got the Refined Visor.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724442);
                                client.SendSysMesage("You opened the Primary Visor Pack and got the Unique Visor.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724136:
                    {
                        client.Inventory.Remove(724136, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724430);
                            client.SendSysMesage("You opened the Primary Plate Pack and got the Normal Plate.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724431);
                                client.SendSysMesage("You opened the Primary Plate Pack and got the Refined Plate.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724432);
                                client.SendSysMesage("You opened the Primary Plate Pack and got the Unique Plate.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724133:
                    {
                        client.Inventory.Remove(724133, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724420);
                            client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Normal Long Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724421);
                                client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Refined Long Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724422);
                                client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Unique Long Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724157:
                    {
                        client.Inventory.Remove(724157, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724410);
                            client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Normal Red Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724411);
                                client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Refined Red Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724412);
                                client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Unique Red Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724151:
                    {
                        client.Inventory.Remove(724151, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724400);
                            client.SendSysMesage("You opened the Primary Bowstring Pack and got the Normal Bowstring.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724401);
                                client.SendSysMesage("You opened the Primary Bowstring Pack and got the Refined Bowstring.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724402);
                                client.SendSysMesage("You opened the Primary Bowstring Pack and got the Unique Bowstring.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724154:
                    {
                        client.Inventory.Remove(724154, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724405);
                            client.SendSysMesage("You opened the Primary Veneer Pack and got the Normal Veneer.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724406);
                                client.SendSysMesage("You opened the Primary Veneer Pack and got the Refined Veneer.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724407);
                                client.SendSysMesage("You opened the Primary Veneer Pack and got the Unique Veneer.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }





                case 3006316:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 3006160);
                            client.SendSysMesage("You~opened~the~Fancy~Hossu~Hair~Pack,~and~received~a~bundled~Refined~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 3006161);
                                client.SendSysMesage("You~opened~the~Fancy~Hossu~Hair~Pack,~and~received~a~bundled~Unique~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3006162);
                                client.SendSysMesage("You~opened~the~Fancy~Hossu~Hair~Pack,~and~received~a~bundled~Elite~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3006317:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 3006161);
                            client.SendSysMesage("You~opened~the~Luxury~Hossu~Hair~Pack,~and~received~a~bundled~Unique~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 3006162);
                                client.SendSysMesage("You~opened~the~Luxury~Hossu~Hair~Pack,~and~received~a~bundled~Elite~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3006163);
                                client.SendSysMesage("You~opened~the~Luxury~Hossu~Hair~Pack,~and~received~a~bundled~Super~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3006318:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 3006162);
                            client.SendSysMesage("You~opened~the~Legendary~Hossu~Hair~Pack,~and~received~a~bundled~Elite~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 3006163);
                                client.SendSysMesage("You~opened~the~Legendary~Hossu~Hair~Pack,~and~received~a~bundled~Super~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3006164);
                                client.SendSysMesage("You~opened~the~Legendary~Hossu~Hair~Pack,~and~received~a~bundled~Sacred~Hossu~Hair~(B)!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3006319:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 3006166);
                            client.SendSysMesage("You~opened~the~Fancy~Hossu~Hair~Pack,~and~received~a~bundled~Refined~Hossu~Hair!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 3006167);
                                client.SendSysMesage("You~opened~the~Fancy~Hossu~Hair~Pack,~and~received~a~bundled~Unique~Hossu~Hair!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3006168);
                                client.SendSysMesage("You~opened~the~Fancy~Hossu~Hair~Pack,~and~received~a~bundled~Elite~Hossu~Hair!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3006320:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 3006167);
                            client.SendSysMesage("You~opened~the~Luxury~Hossu~Hair~Pack,~and~received~a~bundled~Unique~Hossu~Hair!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 3006168);
                                client.SendSysMesage("You~opened~the~Luxury~Hossu~Hair~Pack,~and~received~a~bundled~Elite~Hossu~Hair!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3006169);
                                client.SendSysMesage("You~opened~the~Luxury~Hossu~Hair~Pack,~and~received~a~bundled~Super~Hossu~Hair!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 3006321:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 3006168);
                            client.SendSysMesage("You~opened~the~Legendary~Hossu~Hair~Pack,~and~received~a~bundled~Elite~Hossu~Hair!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 3006169);
                                client.SendSysMesage("You~opened~the~Legendary~Hossu~Hair~Pack,~and~received~a~bundled~Super~Hossu~Hair!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 3006170);
                                client.SendSysMesage("You~opened~the~Legendary~Hossu~Hair~Pack,~and~received~a~bundled~Sacred~Hossu~Hair!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }



                #region Penetration - Bracelet
                case 723130:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721390, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Bell Pack and got the Normal Bell (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721391, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Bell Pack and got the Refined Bell (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721392, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Bell Pack and got the Unique Bell (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723131:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721391, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Bell Pack and got the Refined Bell (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721392, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Bell Pack and got the Unique Bell (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721393, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Bell Pack and got the Elite Bell (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723132:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721392, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Bell Pack and got the Unique Bell (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721393, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Bell Pack and got the Elite Bell (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721394, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Bell Pack and got the Super Bell (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Penetration - Headgear
                case 723684:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721380, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Normal Hemstitch (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721381, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Refined Hemstitch (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721382, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Unique Hemstitch (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723685:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721381, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Refined Hemstitch (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721382, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Unique Hemstitch (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721383, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Elite Hemstitch (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723686:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721382, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Unique Hemstitch (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721383, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Elite Hemstitch (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721384, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Super Hemstitch (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Penetration - Bag
                case 724148:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724385, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Silk Rope Pack and got the Normal Silk Rope.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724386, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Silk Rope Pack and got the Refined Silk Rope.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724387, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Silk Rope Pack and got the Unique Silk Rope.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724149:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724386, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Silk Rope Pack and got the Refined Silk Rope.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724387, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Silk Rope Pack and got the Unique Silk Rope.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724388, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Silk Rope Pack and got the Elite Silk Rope.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724150:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724387, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Silk Rope Pack and got the Unique Silk Rope.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724388, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Silk Rope Pack and got the Elite Silk Rope.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724389, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Silk Rope Pack and got the Super Silk Rope.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion

                #region Skill Critical Strike - Backsword
                case 723133:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721420, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Normal Long Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721421, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Refined Long Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721422, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Unique Long Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723134:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721421, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Refined Long Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721422, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Unique Long Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721423, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Elite Long Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723135:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721422, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Unique Long Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721423, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Elite Long Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721424, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Super Long Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Skill Critical Strike - Bracelet
                case 724195:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724425, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Bead Pack and got a Normal Bead.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724426, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Bead Pack and got a Refined Bead.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724427, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Bead Pack and got an Unique Bead.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724196:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724426, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Bead Pack and got a Refined Bead.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724427, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Bead Pack and got an Unique Bead.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724428, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Bead Pack and got an Elite Bead.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724197:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724427, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Bead Pack and got an Unique Bead.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724428, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Bead Pack and got an Elite Bead.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724429, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Bead Pack and got a Super Bead.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion

                #region Block - Shield
                case 723669:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721358, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Pin Pack and got the Normal Pin (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721359, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Pin Pack and got the Refined Pin (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721360, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Pin Pack and got the Unique Pin (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723670:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721359, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Pin Pack and got the Refined Pin (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721360, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Pin Pack and got the Unique Pin (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721361, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Pin Pack and got the Elite Pin (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723671:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721360, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Pin Pack and got the Unique Pin (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721361, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Pin Pack and got the Elite Pin (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721362, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Pin Pack and got the Super Pin (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion


                #region CriticalStrike - Bow
                case 723651:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721400, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Bowstring Pack and got the Normal Bowstring (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721401, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Bowstring Pack and got the Refined Bowstring (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721402, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Bowstring Pack and got the Unique Bowstring (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723652:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721401, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Bowstring Pack and got the Refined Bowstring (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721402, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Bowstring Pack and got the Unique Bowstring (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721403, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Bowstring Pack and got the Elite Bowstring (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723653:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721402, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Bowstring Pack and got the Unique Bowstring (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721403, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Bowstring Pack and got the Elite Bowstring (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721404, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Bowstring Pack and got the Super Bowstring (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region CriticalStrike - 1 Handed weapons
                case 723654:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721405, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Veneer Pack and got the Normal Veneer (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721406, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Veneer Pack and got the Refined Veneer (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721407, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Veneer Pack and got the Unique Veneer (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723655:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721406, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Veneer Pack and got the Refined Veneer (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721407, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Veneer Pack and got the Unique Veneer (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721408, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Veneer Pack and got the Elite Veneer (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723656:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721407, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Veneer Pack and got the Unique Veneer (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721408, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Veneer Pack and got the Elite Veneer (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721409, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Veneer Pack and got the Super Veneer (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region CriticalStrike - 2 Handed weapons
                case 723657:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721410, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Normal Red Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721411, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Refined Red Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721412, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Unique Red Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723658:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721411, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Refined Red Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721412, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Unique Red Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721413, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Elite Red Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723659:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721412, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Unique Red Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721413, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Elite Red Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721414, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Super Red Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region CriticalStrike - 2 Ring
                case 724215:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724415, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Amber Pack and got a Normal Amber.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724416, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Amber Pack and got a Refined Amber.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724417, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Amber Pack and got an Unique Amber.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724193:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724416, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Amber Pack and got a Refined Amber.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724417, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Amber Pack and got an Unique Amber.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724418, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Amber Pack and got an Elite Amber.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724194:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724417, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Amber Pack and got an Unique Amber.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724418, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Amber Pack and got an Elite Amber.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724419, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Amber Pack and got a Super Amber.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion

                #region Detoxication - Necklace
                case 723681:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721380, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Normal Hemstitch (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721381, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Refined Hemstitch (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721382, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Unique Hemstitch (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;

                    }
                case 723682:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721499, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Jasper Pack and got the Refined Jasper (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721350, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Jasper Pack and got the Unique Jasper (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721351, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Jasper Pack and got the Elite Jasper (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723683:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721350, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Jasper Pack and got the Unique Jasper (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721351, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Jasper Pack and got the Elite Jasper (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721352, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Jasper Pack and got the Super Jasper (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Detoxication - Headgear
                case 723672:
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721483, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Normal Tri Plume (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721484, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Refined Tri Plume (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721485, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Unique Tri Plume (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723673:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721484, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Refined Tri Plume (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721485, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Unique Tri Plume (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721486, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Elite Tri Plume (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723674:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721485, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Unique Tri Plume (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721486, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Elite Tri Plume (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721487, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Super Tri Plume (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Detoxication - Bag
                case 723690:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721373, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Tassel Pack and got the Normal Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721374, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Tassel Pack and got the Refined Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721375, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Tassel Pack and got the Unique Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723691:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721374, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Tassel Pack and got the Refined Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721375, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Tassel Pack and got the Unique Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721376, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Tassel Pack and got the Elite Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723692:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721375, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Tassel Pack and got the Unique Tassel (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721376, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Tassel Pack and got the Elite Tassel (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721377, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Tassel Pack and got the Super Tassel (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Detoxication - Armor
                case 723675:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721488, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Belt Pack and got the Normal Belt (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721489, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Belt Pack and got the Refined Belt (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721490, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Belt Pack and got the Unique Belt (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723676:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721489, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Belt Pack and got the Refined Belt (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721490, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Belt Pack and got the Unique Belt (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721491, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Belt Pack and got the Elite Belt (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723677:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721490, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Belt Pack and got the Unique Belt (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721491, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Belt Pack and got the Elite Belt (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721492, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Belt Pack and got the Super Belt (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Detoxication - Boots
                case 723678:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721493, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Normal Cloud Silk (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721494, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Refined Cloud Silk (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721495, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Unique Cloud Silk (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723679:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721494, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Refined Cloud Silk (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721495, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Unique Cloud Silk (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721496, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Elite Cloud Silk (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723680:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721495, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Unique Cloud Silk (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721496, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Elite Cloud Silk (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721497, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Super Cloud Silk (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion

                #region Breakthrough - 1 Handed weapons
                case 723660:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721445, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Cutter Pack and got the Normal Cutter (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721450, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Cutter Pack and got the Refined Cutter (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721451, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Cutter Pack and got the Unique Cutter (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723661:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721450, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Cutter Pack and got the Refined Cutter (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721451, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Cutter Pack and got the Unique Cutter (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721452, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Cutter Pack and got the Elite Cutter (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723662:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721451, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Cutter Pack and got the Unique Cutter (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721452, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Cutter Pack and got the Elite Cutter (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721453, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Cutter Pack and got the Super Cutter (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Breakthrough - 2 Handed weapons
                case 723663:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721454, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Cauterant Pack and got the Normal Cauterant (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721455, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Cauterant Pack and got the Refined Cauterant (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721456, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Cauterant Pack and got the Unique Cauterant (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723664:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721455, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Cauterant Pack and got the Refined Cauterant (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721456, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Cauterant Pack and got the Unique Cauterant (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721457, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Cauterant Pack and got the Elite Cauterant (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723665:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721456, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Cauterant Pack and got the Unique Cauterant (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721457, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Cauterant Pack and got the Elite Cauterant (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721458, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Cauterant Pack and got the Super Cauterant (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Breakthrough - Bracelet
                case 724201:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724363, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Wristband Pack and got a Normal Wristband.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724364, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Wristband Pack and got a Refined Wristband.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724365, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Wristband Pack and got an Unique Wristband.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724202:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724364, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Wristband Pack and got a Refined Wristband.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724365, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Wristband Pack and got an Unique Wristband.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724366, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Wristband Pack and got an Elite Wristband.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724203:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724365, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Wristband Pack and got an Unique Wristband.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724366, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Wristband Pack and got an Elite Wristband.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724367, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Wristband Pack and got a Super Wristband.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Breakthrough - Bow
                case 723666:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721459, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Normal Bow Limb (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721460, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Refined Bow Limb (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721461, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Unique Bow Limb (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723667:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721460, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Refined Bow Limb (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721461, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Unique Bow Limb (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721462, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Elite Bow Limb (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 723668:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 721461, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Unique Bow Limb (B).", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 721462, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Elite Bow Limb (B).", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 721463, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Super Bow Limb (B).", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Breakthrough - Ring
                case 724198:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724464, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Jadeite Pack and got a Normal Jadeite.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724465, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Jadeite Pack and got a Refined Jadeite.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724470, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Jadeite Pack and got an Unique Jadeite.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724199:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724465, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Jadeite Pack and got a Refined Jadeite.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724470, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Jadeite Pack and got an Unique Jadeite.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724471, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Jadeite Pack and got an Elite Jadeite.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724200:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724470, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Jadeite Pack and got an Unique Jadeite.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724471, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Jadeite Pack and got an Elite Jadeite.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724472, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Jadeite Pack and got a Super Jadeite.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion


                #region Counteraction - Bag
                case 724207:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724368, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Fringe Pack and got a Normal Fringe.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724369, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Fringe Pack and got a Refined Fringe.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724370, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Fringe Pack and got an Unique Fringe.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724208:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724369, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Fringe Pack and got a Refined Fringe.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724370, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Fringe Pack and got an Unique Fringe.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724371, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Fringe Pack and got an Elite Fringe.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724209:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724370, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Fringe Pack and got an Unique Fringe.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724371, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Fringe Pack and got an Elite Fringe.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724372, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Fringe Pack and got a Super Fringe.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region Counteraction - Necklace
                case 724204:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724478, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Aglet Pack and got a Normal Aglet.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724479, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Aglet Pack and got a Refined Aglet.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724480, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Aglet Pack and got an Unique Aglet.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724205:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724479, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Aglet Pack and got a Refined Aglet.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724480, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Aglet Pack and got an Unique Aglet.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724481, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Aglet Pack and got an Elite Aglet.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724206:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724480, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Aglet Pack and got an Unique Aglet.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724481, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Aglet Pack and got an Elite Aglet.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724482, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Aglet Pack and got a Super Aglet.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion

                #region Immunity - Boots
                case 724210:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724435, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Primary Spur Pack and got a Normal Spur.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724436, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Spur Pack and got a Refined Spur.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724437, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Primary Spur Pack and got an Unique Spur.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724211:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724436, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Medium Spur Pack and got a Refined Spur.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724437, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Spur Pack and got an Unique Spur.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724438, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Medium Spur Pack and got an Elite Spur.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724212:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724437, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the Superior Spur Pack and got an Unique Spur.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724438, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Spur Pack and got an Elite Spur.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724439, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the Superior Spur Pack and got a Super Spur.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion



                #region M-Defense - Necklace
                case 725160:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725192, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Normal) Material for a Necklace.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725193, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Refined) Material for a Necklace.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725194, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Unique) Material for a Necklace.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725161:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725193, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Refined) Material for a Necklace.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725194, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Unique) Material for a Necklace.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725195, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Elite) Material for a Necklace.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725162:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725194, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Unique) Material for a Necklace.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725195, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Elite) Material for a Necklace.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725196, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Super) Material for a Necklace.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region M-Defense - Bag
                case 725163:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725197, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Normal) Material for a Bag.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725198, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Refined) Material for a Bag.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725199, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Unique) Material for a Bag.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725164:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725198, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Refined) Material for a Bag.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725199, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Unique) Material for a Bag.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725200, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Elite) Material for a Bag.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725165:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725199, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Unique) Material for a Bag.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725200, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Elite) Material for a Bag.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725201, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Super) Material for a Bag.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region M-Defense - Bracelet
                case 725166:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725202, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Normal) Material for a Bracelet.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725203, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Refined) Material for a Bracelet.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725204, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Unique) Material for a Bracelet.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725167:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725203, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Refined) Material for a Bracelet.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725204, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Unique) Material for a Bracelet.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725205, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Elite) Material for a Bracelet.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725168:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725204, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Unique) Material for a Bracelet.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725205, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Elite) Material for a Bracelet.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725206, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Super) Material for a Bracelet.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                #region M-Defense - Ring
                case 725169:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725202, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Normal) Material for a Bracelet.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725203, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Refined) Material for a Bracelet.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725204, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Unique) Material for a Bracelet.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725170:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725208, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Refined) Material for a Heavy Ring or Ring.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725209, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Unique) Material for a Heavy Ring or Ring.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725210, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Elite) Material for a Heavy Ring or Ring.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725171:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 725209, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Unique) Material for a Heavy Ring or Ring.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 725210, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Elite) Material for a Heavy Ring or Ring.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 725211, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Super) Material for a Heavy Ring or Ring.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion


                case 724178:
                    {
                        client.Inventory.Remove(724178, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724493);
                            client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Normal Cloud Silk.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724494);
                                client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Refined Cloud Silk.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724495);
                                client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Unique Cloud Silk.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724179:
                    {
                        client.Inventory.Remove(724179, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724494);
                            client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Refined Cloud Silk.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724495);
                                client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Unique Cloud Silk.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724496);
                                client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Elite Cloud Silk.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724180:
                    {
                        client.Inventory.Remove(724180, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724495);
                            client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Unique Cloud Silk.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724496);
                                client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Elite Cloud Silk.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724497);
                                client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Super Cloud Silk.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724181:
                    {
                        client.Inventory.Remove(724181, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724498);
                            client.SendSysMesage("You opened the Primary Jasper Pack and got the Normal Jasper.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724499);
                                client.SendSysMesage("You opened the Primary Jasper Pack and got the Refined Jasper.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724350);
                                client.SendSysMesage("You opened the Primary Jasper Pack and got the Unique Jasper.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724182:
                    {
                        client.Inventory.Remove(724182, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724499);
                            client.SendSysMesage("You opened the Medium Jasper Pack and got the Refined Jasper.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724350);
                                client.SendSysMesage("You opened the Medium Jasper Pack and got the Unique Jasper.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724351);
                                client.SendSysMesage("You opened the Medium Jasper Pack and got the Elite Jasper.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724183:
                    {
                        client.Inventory.Remove(724183, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724350);
                            client.SendSysMesage("You opened the Superior Jasper Pack and got the Unique Jasper.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724351);
                                client.SendSysMesage("You opened the Superior Jasper Pack and got the Elite Jasper.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724352);
                                client.SendSysMesage("You opened the Superior Jasper Pack and got the Super Jasper.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724190:
                    {
                        client.Inventory.Remove(724190, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724373);
                            client.SendSysMesage("You opened the Primary Tassel Pack and got the Normal Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724374);
                                client.SendSysMesage("You opened the Primary Tassel Pack and got the Refined Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724375);
                                client.SendSysMesage("You opened the Primary Tassel Pack and got the Unique Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724191:
                    {
                        client.Inventory.Remove(724191, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724374);
                            client.SendSysMesage("You opened the Medium Tassel Pack and got the Refined Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724375);
                                client.SendSysMesage("You opened the Medium Tassel Pack and got the Unique Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724376);
                                client.SendSysMesage("You opened the Medium Tassel Pack and got the Elite Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 724192:
                    {
                        client.Inventory.Remove(724192, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724375);
                            client.SendSysMesage("You opened the Superior Tassel Pack and got the Unique Tassel.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724376);
                                client.SendSysMesage("You opened the Superior Tassel Pack and got the Elite Tassel.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724377);
                                client.SendSysMesage("You opened the Superior Tassel Pack and got the Super Tassel.", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725055:
                    {
                        client.Inventory.Remove(725055, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724415);
                            client.SendSysMesage("You opened the Primary Critical Strike Pack and received a Bound Normal Refinery material (Ring)!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724416);
                                client.SendSysMesage("You opened the Primary Critical Strike Pack and received a Bound Refined Refinery material (Ring)!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724417);
                                client.SendSysMesage("You opened the Primary Critical Strike Pack and received a Bound Unique Refinery material (Ring)!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725056:
                    {
                        client.Inventory.Remove(725056, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724425);
                            client.SendSysMesage("You opened the Primary Skill Critical Strike Pack and received a Bound Normal Refinery material (Bracelet)!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724426);
                                client.SendSysMesage("You opened the Primary Skill Critical Strike Pack and received a Bound Refined Refinery material (Bracelet)!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724427);
                                client.SendSysMesage("You opened the Primary Skill Critical Strike Pack and received a Bound Unique Refinery material (Bracelet)!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725057:
                    {
                        client.Inventory.Remove(725057, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724464);
                            client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Normal Refinery material (Ring)!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724465);
                                client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Refined Refinery material (Ring)!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724470);
                                client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Unique Refinery material (Ring)!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                case 725058:
                    {
                        client.Inventory.Remove(725058, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724363);
                            client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Normal Refinery material (Bracelet)!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724364);
                                client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Refined Refinery material (Bracelet)!", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724365);
                                client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Unique Refinery material (Bracelet)!", MsgMessage.ChatMode.System);
                            }
                        }
                        break;
                    }
                #endregion
                default:
                    handled = false;
                    break;
            }
        }
    }
}
