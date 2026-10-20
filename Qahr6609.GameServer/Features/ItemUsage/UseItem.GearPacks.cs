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
        // UseItem handlers: equipment packs (elite / super / unique gear).
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_GearPacks(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 728237:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            client.Inventory.Add(stream, 130027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            client.Inventory.Add(stream, 131027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            client.Inventory.Add(stream, 133027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            client.Inventory.Add(stream, 135027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            client.Inventory.Add(stream, 136027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            client.Inventory.Add(stream, 139027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                            client.Inventory.Add(stream, 138027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            client.Inventory.Add(stream, 134027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            client.Inventory.Add(stream, 170027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.CreateBoxDialog("You~received~a~Level~32~Unique~Armor!");

                        break;
                    }
                case 728244:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 160118);
                        client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Crocodile~Boots!");

                        break;
                    }
                case 729971:
                    {
                        Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                        dialog.AddText("We sincerely welcome you to join us and admire the Spun Gold Armor recently found by Cloud the Lustful.")
                           .FinalizeDialog();

                        break;
                    }
                case 3000587:
                    {
                        if (!client.Inventory.HaveSpace(10))
                        {
                            client.SendSysMesage("Please make 10 more spaces in your inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 117026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//earring
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 130026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 410056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//blade
                            client.Inventory.Add(stream, 480056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 150056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                            client.Inventory.Add(stream, 120046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                        }
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 131026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 561056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//wand
                            client.Inventory.Add(stream, 150056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                            client.Inventory.Add(stream, 120046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                        }
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 133016, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//armor
                            client.Inventory.Add(stream, 500046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bow
                            client.Inventory.Add(stream, 150056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                            client.Inventory.Add(stream, 120046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                        }
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 135026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 601056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 601056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 150056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                            client.Inventory.Add(stream, 120046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                        }
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 136026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 610056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 610056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 150056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                            client.Inventory.Add(stream, 120046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                        }
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 139026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 611056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//rapier
                            client.Inventory.Add(stream, 612056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//pistol
                            client.Inventory.Add(stream, 150056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                            client.Inventory.Add(stream, 120046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots

                        }
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 134026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 421046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                            client.Inventory.Add(stream, 121046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                            client.Inventory.Add(stream, 152066, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                            //152066
                        }
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 138026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 617056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 617056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 150056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                            client.Inventory.Add(stream, 120046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                        }
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                        {
                            client.Inventory.Add(stream, 101026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 111026, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 626056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 626056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 150056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                            client.Inventory.Add(stream, 120046, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                            client.Inventory.Add(stream, 160056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                        }
                        client.CreateBoxDialog("You've received a suit of Level 30 Refined equipment.");

                        break;
                    }
                case 3100097:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);


                            if (client.Player.Class >= 10 && client.Player.Class <= 15)
                            {
                                client.Inventory.Add(stream, 118008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 GuardCoronet(Elite).");
                            }

                            if (client.Player.Class >= 20 && client.Player.Class <= 25)
                            {
                                client.Inventory.Add(stream, 111008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 IronHelmet(Elite).");
                            }


                            if (client.Player.Class >= 40 && client.Player.Class <= 45)
                            {
                                client.Inventory.Add(stream, 113008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 BadgerHat(Elite).");
                            }


                            if (client.Player.Class >= 50 && client.Player.Class <= 55)
                            {
                                client.Inventory.Add(stream, 123008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 CottonHood(Elite).");
                            }

                            if (client.Player.Class >= 60 && client.Player.Class <= 65)
                            {
                                client.Inventory.Add(stream, 143008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 BronzeHeadband(Elite).");
                            }


                            if (client.Player.Class >= 70 && client.Player.Class <= 75)
                            {
                                client.Inventory.Add(stream, 145008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 TidalHood(Elite).");
                            }



                            if (client.Player.Class >= 80 && client.Player.Class <= 85)
                            {
                                client.Inventory.Add(stream, 148008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 LinenHood(Elite).");
                            }

                            if (client.Player.Class >= 101 && client.Player.Class <= 145)
                            {
                                client.Inventory.Add(stream, 114008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 DestinyCap(Elite).");
                            }



                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }


                case 3100098:
                    {
                        if (client.Inventory.HaveSpace(2))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);


                            if (client.Player.Class >= 10 && client.Player.Class <= 15)
                            {
                                client.Inventory.Add(stream, 420018, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); //sword
                                client.Inventory.Add(stream, 410028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); //blade
                                client.CreateBoxDialog("You received 1 SpringSword(Elite) and 1 DemonBlade(Elite).");
                            }

                            if (client.Player.Class >= 20 && client.Player.Class <= 25)
                            {
                                client.Inventory.Add(stream, 561028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 IronwoodWand(Elite).");
                            }


                            if (client.Player.Class >= 40 && client.Player.Class <= 45)
                            {
                                client.Inventory.Add(stream, 500018, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 HuntingBow(Elite).");
                            }


                            if (client.Player.Class >= 50 && client.Player.Class <= 55)
                            {
                                client.Inventory.Add(stream, 601028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 601028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 2 SteelKatana(Elite).");
                            }

                            if (client.Player.Class >= 60 && client.Player.Class <= 65)
                            {
                                client.Inventory.Add(stream, 610028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 610028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 2 SandalwoodPrayerBeads(Elite).");
                            }


                            if (client.Player.Class >= 70 && client.Player.Class <= 75)
                            {
                                client.Inventory.Add(stream, 612028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); //TidePistol
                                client.Inventory.Add(stream, 611028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 TidePistol(Elite) and 1 ValorRapier(Elite).");
                            }



                            if (client.Player.Class >= 80 && client.Player.Class <= 85)
                            {
                                client.Inventory.Add(stream, 617028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 617028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 2 JujubeWoodNunchaku(Elite).");
                            }

                            if (client.Player.Class >= 101 && client.Player.Class <= 145)
                            {
                                client.Inventory.Add(stream, 421028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 561028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 IronwoodWand(Elite) and 1 JustBacksword(Elite).");
                            }



                        }
                        else
                        {
                            client.SendSysMesage("Please make 2 more spaces in your inventory.");

                        }
                        break;
                    }




                case 3100100:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);


                            if (client.Player.Class >= 10 && client.Player.Class <= 15)
                            {
                                client.Inventory.Add(stream, 130008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 LeatherArmor(Elite).");
                            }

                            if (client.Player.Class >= 20 && client.Player.Class <= 25)
                            {
                                client.Inventory.Add(stream, 131008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 OxhideArmor(Elite).");
                            }


                            if (client.Player.Class >= 40 && client.Player.Class <= 45)
                            {
                                client.Inventory.Add(stream, 133008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 DeerskinCoat(Elite).");
                            }


                            if (client.Player.Class >= 50 && client.Player.Class <= 55)
                            {
                                client.Inventory.Add(stream, 135008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 LowerNinjaVest(Elite).");
                            }

                            if (client.Player.Class >= 60 && client.Player.Class <= 65)
                            {
                                client.Inventory.Add(stream, 136008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 BurlapFrock(Elite).");
                            }


                            if (client.Player.Class >= 70 && client.Player.Class <= 75)
                            {
                                client.Inventory.Add(stream, 139008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 RecruitCoat(Elite).");
                            }



                            if (client.Player.Class >= 80 && client.Player.Class <= 85)
                            {
                                client.Inventory.Add(stream, 138008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 CombatSuit(Elite).");
                            }

                            if (client.Player.Class >= 101 && client.Player.Class <= 145)
                            {
                                client.Inventory.Add(stream, 134008, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received 1 TaoRobe(Elite).");
                            }



                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }





                case 3100102:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 120028, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You received 1 HeartNecklace(Elite).");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }


                case 3100099:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 152018, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You received 1 PeachBracelet(Elite).");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }







                case 3100101:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 160018, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You received 1 OxhideBoots(Elite).");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }

                //
                case 720706:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 561138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //ironwand
                            client.Inventory.Add(stream, 131068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //lightarmour
                            client.Inventory.Add(stream, 117068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeartofOcean
                            client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                            client.CreateBoxDialog("You received an~Elite~L70~Iron~Wand,~an~Elite~L70~Light~Armor,~an~Elite~L67~Heart~of~Ocean~and~3~+1~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720707:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 420138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //SharkSword
                            client.Inventory.Add(stream, 130068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //RageArmor
                            client.Inventory.Add(stream, 118068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //WarCoronet
                            client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                            client.CreateBoxDialog("You received an~Elite~L70~Shark~Sword,~an~Elite~L70~Rage~Armor,~an~Elite~L67~War~Coronet~and~3~+1~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }



                case 720708:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 500128, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //GooseBow
                            client.Inventory.Add(stream, 133048, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //ApeCoat
                            client.Inventory.Add(stream, 113048, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //ApeHat
                            client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                            client.CreateBoxDialog("You received an~Elite~L70~Goose~Bow,~an~Elite~L67~Ape~Coat,~an~Elite~L72~Ape~Hat~and~3~+1~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }

                case 720709:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 601138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //RainKatana
                            client.Inventory.Add(stream, 135068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //TigerVest
                            client.Inventory.Add(stream, 112068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BloodEvil
                            client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                            client.CreateBoxDialog("You received an~Elite~L70~Rain~Katana,~an~Elite~L70~TigerVest,~an~Elite~L67~BloodVeil~and~3~+1~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720710:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 421138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //GreatBacksword
                            client.Inventory.Add(stream, 134068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //CraneVestment
                            client.Inventory.Add(stream, 114068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //Sharkcap
                            client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                            client.CreateBoxDialog("You received an~Elite~L70~Great~Backsword,~an~Elite~L70~Crane~Vestment,~an~Elite~L67~Shark~Cap~and~3~+1~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }

                case 720711:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 120128, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //PlatinaNecklace
                            client.Inventory.Add(stream, 150138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //PearlRing
                            client.Inventory.Add(stream, 160138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //SnakeskinBoots
                            client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                            client.CreateBoxDialog("You received an~Elite~L67~Platina~Necklace,~an~Elite~L70~Pearl~Ring,~an~Elite~L70~Snakeskin~Boots~and~3~+1~Stones");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }



                case 720712:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 152128, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BoneBracelet
                            client.Inventory.Add(stream, 121128, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //AmbergrisBag
                            client.Inventory.Add(stream, 160138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //SnakeskinBoots
                            client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                            client.CreateBoxDialog("You received an~Elite~L65~Bone~Bracelet,~an~Elite~L67~Ambergris~Bag,~an~Elite~L70~Snakeskin~Boots~and~3~+1~Stones");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720704:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 150179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //CrystalRing
                            client.Inventory.Add(stream, 120159, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BasaltNecklace
                            client.Inventory.Add(stream, 160179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //LeopardBoots
                            client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                            client.CreateBoxDialog("You received a~Super~L90~Crystal~Ring,~a~Super~L82~Basalt~Necklace,~a~Super~L90~Leopard~Boots~and~3~+2~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720705:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 152169, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //JadeBracelet
                            client.Inventory.Add(stream, 121159, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //JadeBag
                            client.Inventory.Add(stream, 160179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //LeopardBoots
                            client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                            client.CreateBoxDialog("You received a~Super~L85~Jade~Bracelet,~a~Super~L82~Jade~Bag,~a~Super~L90~Leopard~Boots~and~3~+2~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }




                case 720699:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 561179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //ExorcisingWand
                            client.Inventory.Add(stream, 131079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //LionArmor
                            client.Inventory.Add(stream, 111079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //DragonHelmet
                            client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                            client.CreateBoxDialog("You received a~Super~L90~Exorcising~Wand,~a~Super~L87~Lion~Armor,~a~Super~L82~Dragon~Helmet~and~3~+2~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720700:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 420179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //LongSword
                            client.Inventory.Add(stream, 130079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //SacredArmor
                            client.Inventory.Add(stream, 118079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HerculesCoronet
                            client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                            client.CreateBoxDialog("You received a~Super~L90~Long~Sword,~a~Super~L87~Sacred~Armor,~a~Super~L82~Hercules~Coronet~and~3~+2~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }




                case 720701:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 500169, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //StarBow
                            client.Inventory.Add(stream, 133069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //SharkCoat
                            client.Inventory.Add(stream, 113059, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //MartenHat
                            client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                            client.CreateBoxDialog("You received a~Super~L90~Star~Bow,~a~Super~L87~Shark~Coat,~a~Super~L82~Marten~Hat~and~3~+2~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720702:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 601179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //IseKatana
                            client.Inventory.Add(stream, 135079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BearVest
                            client.Inventory.Add(stream, 112079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //StealthVeil
                            client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                            client.CreateBoxDialog("You received a~Super~L90~Ise~Katana,~a~Super~L87~Bear~Vest,~a~Super~L82~Stealth~Veil~and~3~+2~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }


                case 720703:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                            client.Inventory.Add(stream, 421179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //WarBacksword
                            client.Inventory.Add(stream, 134079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //FullFrock
                            client.Inventory.Add(stream, 114079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //DragonCap
                            client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                            client.CreateBoxDialog("You received a~Super~L90~War~Backsword,~a~Super~L87~Full~Frock,~a~Super~L82~Dragon~Cap~and~3~+2~Stones.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 5 more spaces in your inventory.");

                        }
                        break;
                    }
                case 728242:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, Database.ItemType.ExpBall2);
                            if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Jade~Bracelet,~a~ExpBall(Event)!");
                                client.Inventory.Add(stream, 152168, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 150178, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Crystal~Ring,~a~ExpBall(Event)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 728241:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, Database.ItemType.ExpBall2);
                            if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Jade~Bag,~a~ExpBall(Event)!");
                                client.Inventory.Add(stream, 121158, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 120158, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Basalt~Necklace,~a~ExpBall(Event)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }

                        break;
                    }
                case 728248:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 722136);
                            if (client.Player.Class <= 85)
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, 120189, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Dragon~Necklace!");
                            }
                            else
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.Add(stream, 121189, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Gold~Bag!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }

                        break;
                    }
                case 728249:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.Inventory.Add(stream, 160199, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Kylin~Boots!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 3300049:// elite pk patch
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        byte rate = (byte)Program.GetRandom.Next(1, 8);
                        switch (rate)
                        {
                            case 1:
                                {
                                    client.CreateBoxDialog("You received AstralPhoenixFragment*1!");
                                    client.Inventory.AddItemWitchStack(3300036, 0, 1, stream);
                                    break;
                                }
                            case 2:
                                {
                                    client.CreateBoxDialog("You've received a LegendarySpiritPack!");
                                    client.Inventory.AddItemWitchStack(3300179, 0, 1, stream);
                                    break;
                                }
                            default:
                                {
                                    client.CreateBoxDialog("You've received a SpiritofLegend!");
                                    client.Inventory.AddItemWitchStack(3300056, 0, 1, stream);
                                    break;
                                }

                        }


                        break;
                    }
                case 724132:
                    {
                        client.Inventory.Remove(724132, 1, stream);
                        if (Role.Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724392);
                            client.SendSysMesage("You opened the Superior Bell Pack and got the Unique Bell.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            if (Role.Core.Rate(30, 40))
                            {
                                client.Inventory.Add(stream, 724393);
                                client.SendSysMesage("You opened the Superior Bell Pack and got the Elite Bell.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                client.Inventory.Add(stream, 724394);
                                client.SendSysMesage("You opened the Superior Bell Pack and got the Super Bell.", MsgMessage.ChatMode.System);
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
