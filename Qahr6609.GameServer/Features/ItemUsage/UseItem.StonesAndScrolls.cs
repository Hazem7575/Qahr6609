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
        // UseItem handlers: +N stones, meteor / dragonball scrolls, gems, jades.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_StonesAndScrolls(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 720020://statue scroll
                    {
                        client.UseItem = item.ITEM_ID;


                        NpcServerQuery npc = new NpcServerQuery();
                        npc.ID = (MsgNpc.NpcID)3;
                        npc.Mesh = 1130;
                        npc.Action = MsgNpc.NpcServerReplay.Mode.Cursor;
                        npc.NpcType = (Role.Flags.NpcType)9;
                        client.Send(stream.NpcServerCreate(npc));

                        break;
                    }
                case 720027://MeteorScroll
                    {
                        if (client.Inventory.HaveSpace(9))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            if (item.Bound == 1)
                            {
                                client.Inventory.Add(stream, 1088001, 10, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                                client.Inventory.Add(stream, 1088001, 10, 0);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 10 more spaces in your inventory.");

                        }
                        break;
                    }
                case 3302769://supermeteorscroll
                    {
                        if (client.Inventory.HaveSpace(9))
                        {
                            bool bound = item.Bound == 1 ? true : false;
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 720027, 10, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound);
                        }
                        else
                        {

                            client.SendSysMesage("Please make 9 more spaces in your inventory.");
                        }
                        break;
                    }
                case 3302896://superdragonballscroll(Bound)
                    {
                        if (client.Inventory.HaveSpace(9))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 720028, 10, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        }
                        else
                        {

                            client.SendSysMesage("Please make 9 more spaces in your inventory.");
                        }
                        break;
                    }
                case 3302770://superdragonballscroll
                    {
                        if (client.Inventory.HaveSpace(9))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 720028, 10, 0);
                        }
                        else
                        {

                            client.SendSysMesage("Please make 9 more spaces in your inventory.");
                        }
                        break;
                    }
                case 720028://dragonballscroll
                    {
                        if (client.Inventory.HaveSpace(9))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 1088000, 10, 0);
                        }
                        else
                        {

                            client.SendSysMesage("Please make 9 more spaces in your inventory.");
                        }
                        break;
                    }
                case 723711://MeteorTearScroll
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 1088002, 5, 0);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 729022://+2 stone packet
                    {
                        if (client.Inventory.HaveSpace(9))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 730002, 10, 2);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 9 more spaces in your inventory.");

                        }
                        break;
                    }
                case 729023://+3 stone packet
                    {
                        if (client.Inventory.HaveSpace(9))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 730003, 10, 3);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 9 more spaces in your inventory.");

                        }
                        break;
                    }
                case 723712://+1 stone packet
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            if (item.Bound == 1)
                            {
                                client.Inventory.Add(stream, 730001, 5, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                                client.Inventory.Add(stream, 730001, 5, 1);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                #region PrayingStone(S)
                case 1200000:
                    {
                        client.Player.AddHeavenBlessing(stream, (int)(3 * 24 * 60 * 60));
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                #endregion
                case 728888:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.AddHeavenBlessing(stream, (int)(12 * 60 * 60));
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dee_cit_ble");
                        client.SendSysMesage("You used a Dis City Bless Stone (12H) and received 12 hours of Heaven`s Blessing!");
                        break;
                    }
                #region PrayingStone(M)
                case 1200001:
                    {
                        client.Player.AddHeavenBlessing(stream, (int)(7 * 24 * 60 * 60));
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                #endregion
                #region PrayingStone(L)
                case 1200002:
                    {
                        client.Player.AddHeavenBlessing(stream, (int)(30 * 24 * 60 * 60));
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                #endregion
                case 3100017:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            client.SendSysMesage("Please make 3 more space in inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3009000, 4, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 3009001, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 3100012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.CreateBoxDialog("You received 4TwilightStarStones, 2BrightStarStones, a SeniorSign-inBox.");

                        break;
                    }



                case 720892:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            client.CreateBoxDialog("Please make 3 more spaces in your inventory.");
                            break;
                        }

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 700073, 3);

                        client.CreateBoxDialog("You received 3~Super~Tortoise~Gems.");
                        break;
                    }



                case 723826:
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 160138, 0, 0, 3, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, false); //bots -3 & elite (B)
                            client.Inventory.Add(stream, 723712, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // +1 Pack - non bound
                            client.Inventory.AddItemWitchStack(3001283, 0, 3, stream);   // 3 ExpBalls
                            client.Inventory.AddItemWitchStack(1200001, 0, 1, stream);  // PrayingStone(M)
                            client.Inventory.AddItemWitchStack(723711, 0, 5, stream);  //// MeteorTearPack - 5
                            client.CreateBoxDialog("You received~1~Socket~Lv70EliteBoots(-3%),~a~+1~StonePack,~3~EXPBalls(F),~a~7~Days`~Blessing~Stone(F),~and~5~Meteor~Tear~Packs.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }




                case 723827:
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 120128, 0, 0, 3, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, false); //Necklace -3 & elite 
                            client.Inventory.Add(stream, 723712, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // +1 Pack - non bound
                            client.Inventory.AddItemWitchStack(3001283, 0, 3, stream);   // 3 ExpBalls
                            client.Inventory.AddItemWitchStack(1200001, 0, 1, stream);  // PrayingStone(M)
                            client.Inventory.AddItemWitchStack(723711, 0, 5, stream);  //// MeteorTearPack - 5
                            client.CreateBoxDialog("You received~1~Socket~Lv67~Elite~Necklace(-3%),~a~+1~StonePack,~3~EXPBalls(F),~a~7~Days`~Blessing~Stone(F),~and~5~Meteor~Tear~Packs.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }



                case 723828:
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 150138, 0, 0, 3, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, false); //Ring -3 & elite 
                            client.Inventory.Add(stream, 723712, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // +1 Pack - non bound
                            client.Inventory.AddItemWitchStack(3001283, 0, 3, stream);   // 3 ExpBalls
                            client.Inventory.AddItemWitchStack(1200001, 0, 1, stream);  // PrayingStone(M)
                            client.Inventory.AddItemWitchStack(723711, 0, 5, stream);  //// MeteorTearPack - 5
                            client.CreateBoxDialog("You received~1~Socket~Lv70~EliteRing(-3%),~a~+1~StonePack,~3~EXPBalls(F),~a~7~Days`~Blessing~Stone(F),~and~5~Meteor~Tear~Packs.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }


                case 723829:
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 121128, 0, 0, 3, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, false); //Bag -3 & elite 
                            client.Inventory.Add(stream, 723712, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // +1 Pack - non bound
                            client.Inventory.AddItemWitchStack(3001283, 0, 3, stream);   // 3 ExpBalls
                            client.Inventory.AddItemWitchStack(1200001, 0, 1, stream);  // PrayingStone(M)
                            client.Inventory.AddItemWitchStack(723711, 0, 5, stream);  //// MeteorTearPack - 5
                            client.CreateBoxDialog("You received~1~Socket~Lv67~Elite~Bag(-3%),~a~+1~StonePack,~3~EXPBalls(F),~a~7~Days`~Blessing~Stone(F),~and~5~Meteor~Tear~Packs.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }


                case 723830:
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 152148, 0, 0, 3, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, false); //Bracelet -3 & elite 
                            client.Inventory.Add(stream, 723712, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // +1 Pack - non bound
                            client.Inventory.AddItemWitchStack(3001283, 0, 3, stream);   // 3 ExpBalls
                            client.Inventory.AddItemWitchStack(1200001, 0, 1, stream);  // PrayingStone(M)
                            client.Inventory.AddItemWitchStack(723711, 0, 5, stream);  //// MeteorTearPack - 5
                            client.CreateBoxDialog("You received~1~Socket~Lv75EliteBracelet(-3%),~a~+1~StonePack,~3~EXPBalls(F),~a~7~Days`~Blessing~Stone(F),~and~5~Meteor~Tear~Packs.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }


                case 723831:
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 117068, 0, 0, 3, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, false); //Earings -3 & elite 
                            client.Inventory.Add(stream, 723712, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); // +1 Pack - non bound
                            client.Inventory.AddItemWitchStack(3001283, 0, 3, stream);   // 3 ExpBalls
                            client.Inventory.AddItemWitchStack(1200001, 0, 1, stream);  // PrayingStone(M)
                            client.Inventory.AddItemWitchStack(723711, 0, 5, stream);  //// MeteorTearPack - 5
                            client.CreateBoxDialog("You received~1~Socket~Lv67~EliteEarrings(-3%),~a~+1~StonePack,~3~EXPBalls(F),~a~7~Days`~Blessing~Stone(F),~and~5~Meteor~Tear~Packs.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 6 more spaces in your inventory.");

                        }
                        break;
                    }


                case 3005654://+2StonePack
                    {
                        if (client.Inventory.HaveSpace(2))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            if (item.Bound == 1)
                            {
                                client.Inventory.Add(stream, 730002, 2, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                                client.Inventory.Add(stream, 730002, 2, 1);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 2 more spaces in your inventory.");

                        }
                        break;
                    }
                case 721952:
                    {
                        if (client.Inventory.Contain(721953, 1))
                        {
                            if (client.Inventory.Contain(1, 710583))
                            {
                                client.SendSysMesage("You've already obtained a Moon Jade.", MsgMessage.ChatMode.System);
                                break;
                            }
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Remove(721953, 1, stream);
                            if (client.Player.Rate(40))
                            {
                                client.Inventory.Add(stream, 710583);//si itemul
                                client.SendSysMesage("A Maple Stone and Ocean Stone have been fused into a Moon Jade!", MsgMessage.ChatMode.System);
                            }
                            else
                                client.SendSysMesage("Failed to fuse! The Maple Stone and Ocean Stone disapperead!", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("If you have a Maple Stone and an Ocean Stone, right click the Leaf Stone to fuse them info a Sun Jade.", MsgMessage.ChatMode.System);
                        break;
                    }
                case 721953://ocean Stone
                    {
                        if (client.Inventory.Contain(721951, 1))
                        {
                            if (client.Inventory.Contain(1, 710584))
                            {
                                client.SendSysMesage("You've already obtained a Sun Jade.", MsgMessage.ChatMode.System);
                                break;
                            }
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Remove(721951, 1, stream);
                            if (client.Player.Rate(40))
                            {
                                client.Inventory.Add(stream, 710584);//si itemul
                                client.SendSysMesage("A Leaf Stone and Ocean Stone have been fused into a Sun Jade!", MsgMessage.ChatMode.System);
                            }
                            else
                                client.SendSysMesage("Failed to fuse! The leaf Stone and Ocean Stone disapperead!", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("If you have a Leaf Stone and an Ocean Stone, right click the Leaf Stone to fuse them info a Sun Jade.", MsgMessage.ChatMode.System);

                        break;
                    }
                case 721951://leaf Stone
                    {
                        if (client.Inventory.Contain(721952, 1))
                        {

                            if (client.Inventory.Contain(1, 710582))
                            {
                                client.SendSysMesage("You've already obtained a Rune Jade.", MsgMessage.ChatMode.System);
                                break;
                            }
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Remove(721952, 1, stream);



                            if (client.Player.Rate(40))
                            {
                                client.Inventory.Add(stream, 710582);
                                client.SendSysMesage("A Leaf Stone and Maple Stone have been fused into a Rune Jade!", MsgMessage.ChatMode.System);
                            }
                            else
                                client.SendSysMesage("Failed to fuse! The leaf Stone and Maple Stone disapperead!", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("If you have a Leaf Stone and an Maple Stone, right click the Leaf Stone to fuse them info a Sun Jade.", MsgMessage.ChatMode.System);
                        break;
                    }
                case 3008994:
                    {

                        if (client.Inventory.Contain(3008994, 10))
                        {
                            client.Inventory.Remove(3008994, 10, stream);
                            client.Inventory.Add(stream, Database.ItemType.DragonBall);
                            client.SendSysMesage("You successfully made a DragonBall.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3600031:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3009000, 3);
                        client.SendSysMesage("You received 3xTwilightStarStone.", MsgMessage.ChatMode.System);
                        break;
                    }
                case 3300051:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3009000, 1);
                        client.SendSysMesage("You've received a TwilightStarStone.", MsgMessage.ChatMode.System);
                        break;
                    }
                case 3300052:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3009001, 1);
                        client.SendSysMesage("You've received a BrightStarStone.", MsgMessage.ChatMode.System);
                        break;
                    }
                case 721878:
                    {
                        client.CreateDialog(stream, "What? It`s but a blank scroll?! I should go and teach Shark a lesson!", "");
                        break;
                    }
                case 3006030:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 730006, 1, 6);
                            client.Inventory.Add(stream, 730005, 1, 5);
                            client.CreateBoxDialog("You~received~a~+6~Stone~and~a~+5~Stone!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~Please~make~some~room,~first.");
                        }
                        break;
                    }
                case 3006029:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 730005, 1, 5);
                            client.Inventory.Add(stream, 730004, 1, 4);
                            client.CreateBoxDialog("You~received~a~+5~Stone~and~a~+4~Stone!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~Please~make~some~room,~first.");
                        }
                        break;
                    }
                case 3006028:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 730004, 1, 4);
                            client.Inventory.Add(stream, 730003, 1, 3);
                            client.CreateBoxDialog("You~received~a~+4~Stone~and~a~+3~Stone!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~Please~make~some~room,~first.");
                        }
                        break;
                    }
                case 3006027:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 730003, 1, 3);
                            client.Inventory.Add(stream, 730002, 1, 2);
                            client.CreateBoxDialog("You~received~a~+3~Stone~and~a~+2~Stone!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~Please~make~some~room,~first.");
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
