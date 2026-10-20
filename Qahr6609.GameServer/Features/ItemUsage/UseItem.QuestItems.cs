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
        // UseItem handlers: quest items.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_QuestItems(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 3000550:
                    {
                        switch (item.Bound)
                        {

                            case 1://level 5
                                {
                                    if (!client.Inventory.HaveSpace(2))
                                    {
                                        client.CreateBoxDialog("Please make 3 more spaces in your inventory.");

                                        break;
                                    }
                                    if (client.Player.Level >= 5)
                                    {
                                        item.Bound = 2;
                                        client.Inventory.Add(stream, Database.ItemType.ExpBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.DoubleExp, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (!client.Inventory.HaveSpace(2))
                                    {
                                        client.CreateBoxDialog("Please make 2 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 10)
                                    {

                                        item.Bound = 3;
                                        client.Inventory.Add(stream, 1200000, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.AddItemWitchStack(1000000, 0, 3, stream);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 3:
                                {
                                    if (!client.Inventory.HaveSpace(2))
                                    {
                                        client.CreateBoxDialog("Please make 2 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 15)
                                    {

                                        item.Bound = 4;
                                        //    client.Player.ConquerPoints += 200;
                                        ///      client.Player.SendUpdate(client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                        //  client.SendSysMesage("You have received 200 ConquerPoints.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                        client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.AddItemWitchStack(1002000, 0, 3, stream);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 4:
                                {
                                    if (!client.Inventory.HaveSpace(2))
                                    {
                                        client.CreateBoxDialog("Please make 2 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 20)//give steed.
                                    {

                                        item.Bound = 5;
                                        //    client.Player.ConquerPoints += 200;
                                        //   client.Player.SendUpdate(client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                        //    client.SendSysMesage("You have received 200 ConquerPoints.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 150, 255, 0);
                                        client.Inventory.AddItemWitchStack(1060020, 0, 3, stream);
                                        //300000

                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 5:
                                {
                                    if (!client.Inventory.HaveSpace(3))
                                    {
                                        client.CreateBoxDialog("Please make 3 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 25)
                                    {

                                        item.Bound = 6;
                                        client.Inventory.Add(stream, Database.ItemType.ExpBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.DoubleExp, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 6:
                                {
                                    if (!client.Inventory.HaveSpace(3))
                                    {
                                        client.CreateBoxDialog("Please make 3 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 30)
                                    {

                                        item.Bound = 7;
                                        client.Inventory.Add(stream, 203008, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.DoubleExp, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, 3000587, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }//client.Inventory.Add(730001, 5, 1);
                            case 7:
                                {
                                    if (!client.Inventory.HaveSpace(5))
                                    {
                                        client.CreateBoxDialog("Please make 5 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 35)
                                    {

                                        item.Bound = 8;
                                        client.Inventory.Add(stream, 730001, 5, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 8:
                                {
                                    if (!client.Inventory.HaveSpace(3))
                                    {
                                        client.CreateBoxDialog("Please make 3 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 40)
                                    {

                                        item.Bound = 9;
                                        //  client.Player.ConquerPoints += 300;
                                        //  client.Player.SendUpdate(client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                        //  client.SendSysMesage("You have received 300 ConquerPoints.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                        client.Inventory.Add(stream, Database.ItemType.ExpBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 9:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        client.CreateBoxDialog("Please make 1 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 45)
                                    {

                                        item.Bound = 10;
                                        client.Inventory.Add(stream, 700013, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 10:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        client.CreateBoxDialog("Please make 1 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 50)
                                    {

                                        item.Bound = 11;
                                        //reward Weapon
                                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                            client.Inventory.Add(stream, 410019, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//leafBlade
                                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                            client.Inventory.Add(stream, 480099, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//club
                                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                            client.Inventory.Add(stream, 500009, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BambooBow
                                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                            client.Inventory.Add(stream, 601019, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//iron katana
                                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                            client.Inventory.Add(stream, 610019, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BambooPrayerBeads
                                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                            client.Inventory.Add(stream, 611019, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//recruitRapier
                                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                            client.Inventory.Add(stream, 421019, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//SpellBacksword 
                                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                            client.Inventory.Add(stream, 617019, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//WhiteWaxNunchaku 
                                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                            client.Inventory.Add(stream, 626099, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);// 
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 11:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        client.CreateBoxDialog("Please make 1 more space in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 55)
                                    {

                                        item.Bound = 12;

                                        //reward armor
                                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                            client.Inventory.Add(stream, 130049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                            client.Inventory.Add(stream, 131049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                            client.Inventory.Add(stream, 133049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                            client.Inventory.Add(stream, 135049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                            client.Inventory.Add(stream, 136049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                            client.Inventory.Add(stream, 139049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                            client.Inventory.Add(stream, 134049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                            client.Inventory.Add(stream, 138049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                            client.Inventory.Add(stream, 101059, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//138049 CombatSuit(Lv.47
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 12:
                                {
                                    if (!client.Inventory.HaveSpace(4))
                                    {
                                        client.CreateBoxDialog("Please make 4 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 60)
                                    {

                                        item.Bound = 13;
                                        client.Inventory.Add(stream, 3000588, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.ExpBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.DoubleExp, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 13:
                                {
                                    if (!client.Inventory.HaveSpace(3))
                                    {
                                        client.CreateBoxDialog("Please make 3 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 65)
                                    {

                                        item.Bound = 14;
                                        client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 14:
                                {
                                    if (!client.Inventory.HaveSpace(2))
                                    {
                                        client.CreateBoxDialog("Please make 2 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 70)
                                    {
                                        item.Bound = 15;
                                        client.Inventory.Add(stream, Database.ItemType.MeteorTearPacket, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, 1080001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 15:
                                {
                                    if (!client.Inventory.HaveSpace(2))
                                    {
                                        client.CreateBoxDialog("Please make 2 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 75)
                                    {
                                        item.Bound = 16;
                                        client.Inventory.Add(stream, 730002, 2, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 16:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        client.CreateBoxDialog("Please make 1 more space in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 80)
                                    {
                                        item.Bound = 17;
                                        client.Inventory.Add(stream, 160159, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//160159 TigerBoots 
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 17:
                                {
                                    if (!client.Inventory.HaveSpace(10))
                                    {
                                        client.CreateBoxDialog("Please make 10 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 85)
                                    {
                                        item.Bound = 18;
                                        client.Inventory.Add(stream, Database.ItemType.ExpBall, 5, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.DoubleExp, 5, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 18:
                                {
                                    if (!client.Inventory.HaveSpace(10))
                                    {
                                        client.CreateBoxDialog("Please make 10 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 90)
                                    {
                                        item.Bound = 19;
                                        client.Inventory.Add(stream, Database.ItemType.ExpBall, 5, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.DoubleExp, 5, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 19:
                                {
                                    if (!client.Inventory.HaveSpace(4))
                                    {
                                        client.CreateBoxDialog("Please make 4 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 95)
                                    {
                                        item.Bound = 20;
                                        client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.DoubleExp, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 20:
                                {
                                    if (!client.Inventory.HaveSpace(2))
                                    {
                                        client.CreateBoxDialog("Please make 2 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 100)
                                    {
                                        item.Bound = 21;
                                        client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                                        // client.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2,0,0,0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        //     client.Player.ConquerPoints += 600;
                                        //     client.Player.SendUpdate(client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                        //     client.SendSysMesage("You have received 600 ConquerPoints.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 21:
                                {
                                    if (!client.Inventory.HaveSpace(3))
                                    {
                                        client.CreateBoxDialog("Please make 3 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 105)
                                    {
                                        item.Bound = 22;
                                        //client.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 22:
                                {
                                    if (!client.Inventory.HaveSpace(3))
                                    {
                                        client.CreateBoxDialog("Please make 3 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 110)
                                    {
                                        item.Bound = 23;
                                        //  client.Inventory.Add(stream, Database.ItemType.PowerExpBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        //   client.Player.ConquerPoints += 1000;
                                        //   client.Player.SendUpdate(client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                        //   client.SendSysMesage("You have received 1000 ConquerPoints.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 23:
                                {
                                    if (!client.Inventory.HaveSpace(4))
                                    {
                                        client.CreateBoxDialog("Please make 4 more spaces in your inventory");
                                        break;
                                    }
                                    if (client.Player.Level >= 115)
                                    {
                                        item.Bound = 24;
                                        // client.Inventory.Add(Database.ItemType.PowerExpBall, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 4, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        //   client.Player.ConquerPoints += 2000;
                                        //   client.Player.SendUpdate(client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                        //   client.SendSysMesage("You have received 2000 ConquerPoints.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }
                            case 24:
                                {
                                    if (!client.Inventory.HaveSpace(4))
                                    {
                                        client.CreateBoxDialog("Please make 4 more spaces in your inventory");

                                        break;
                                    }
                                    if (client.Player.Level >= 120)
                                    {
                                        item.Bound = 25;
                                        //client.Inventory.Add(Database.ItemType.PowerExpBall, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 4, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, 721259);

                                        //   client.Player.ConquerPoints += 2000;
                                        //   client.Player.SendUpdate(client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                        //   client.SendSysMesage("You have received 2000 ConquerPoints.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    }
                                    else
                                    {
                                        client.SendSysMesage("Sorry, You can open this package of 5 in 5 levels", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                                    }

                                    break;
                                }

                        }
                        if (item.Bound < 25)
                        {
                            item.Mode = Role.Flags.ItemMode.Update;
                            item.Send(client, stream);
                        }
                        else
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        break;
                    }
                case 3004464:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem))
                        {
                            if (Database.ItemType.IsKatana(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 616000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);

                                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Ninja Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {
                                client.SendSysMesage("Soryy, You're not equipped with the Katana's(right weapon).");

                            }
                        }
                        else
                        {

                            client.SendSysMesage("Soryy, You're not equipped with the Katana's(right weapon).");
                        }
                        break;
                    }
                case 3006016:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem))
                        {
                            if (Database.ItemType.IsBacksword(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 620000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Taoist Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {
                                client.SendSysMesage("Soryy, You're not equipped with the backsword's(right weapon).");

                            }
                        }
                        else
                        {
                            client.SendSysMesage("Soryy, You're not equipped with the backsword's(right weapon).");

                        } break;
                    }
                case 3007564:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem))
                        {
                            if (Database.ItemType.IsMonkWeapon(GameItem.ITEM_ID))
                            {
                                uint UpdateToEpic = (GameItem.ITEM_ID % 1000) + 622000;
                                GameItem.ITEM_ID = UpdateToEpic;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Monk Epic Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));

                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                            {
                                client.SendSysMesage("Soryy, You're not equipped with the PrayerBeads's(right weapon).");

                            }
                        }
                        else
                        {
                            client.SendSysMesage("Soryy, You're not equipped with the PrayerBeads's(right weapon).");

                        }
                        break;
                    }
                case 720598://dragonpill
                    {
                        if (client.Player.TeratoDragon > 0)
                        {
                            client.SendSysMesage("sorry, but you really spawned Dragon today.");
                            return;
                        }
                        if (client.Player.Map == 2056)
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 532, 394) <= 18)
                            {
                                if (!client.Map.ContainMobID(20060))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    Database.Server.AddMapMonster(stream, client.Map, 20060, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                                    client.Player.TeratoDragon++;
                                }
                                else
                                {
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");

                                }
                            }
                            else
                            {
                                client.CreateDialog(stream, "The Dragon Pill is used to summon the Terato Dragon. You can use it in the Frozen Grotto Floor 6 (532,394) or the Wind Plain (517,742).", "I~see.");

                            }
                        }
                        else if (client.Player.Map == 1002)
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 517, 742) <= 18)
                            {
                                if (!client.Map.ContainMobID(20060))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    Database.Server.AddMapMonster(stream, client.Map, 20060, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                                }
                                else
                                {
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");

                                }
                            }
                            else
                            {
                                client.CreateDialog(stream, "The Dragon Pill is used to summon the Terato Dragon. You can use it in the Frozen Grotto Floor 6 (532,394) or the Wind Plain (517,742).", "I~see.");


                            }
                        }
                        break;
                    }
                case 720599://shadow jade  fg5
                    {
                        if (client.Player.Map == 2055 || client.Player.Map == 2056)
                        {

                            client.Player.MessageBox("Do you want to speak with the F5 Grotto Miner now?", new Action<Client.GameClient>(p =>
                            {
                                p.Teleport(540, 573, 2055);
                                p.CreateBoxDialog("You were send to the F5 Grotto Miner.");
                            }
                               ), null, 0);

                        }
                        else
                        {
                            client.CreateBoxDialog("Sorry,  use that in F5 AND F6.");

                        }
                        break;
                    }
                case 729983:
                case 729984:
                case 729985:
                case 729986:
                case 729987:
                case 729988:
                case 729989:
                    {
                        if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                         && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                         && client.Inventory.Contain(729989, 1))
                        {
                            client.Inventory.Remove(729983, 1, stream);
                            client.Inventory.Remove(729984, 1, stream);
                            client.Inventory.Remove(729985, 1, stream);
                            client.Inventory.Remove(729986, 1, stream);
                            client.Inventory.Remove(729987, 1, stream);
                            client.Inventory.Remove(729988, 1, stream);
                            client.Inventory.Remove(729989, 1, stream);
                            client.Inventory.Add(stream, 729982);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1785, 0, 0, 0, 0, 0, 0, 0, 1);
                            client.CreateBoxDialog("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.");

                            client.Teleport(554, 600, 1020);
                            break;
                        }
                        else
                        {
                            client.CreateBoxDialog("Well, you are short of powders.");

                        }
                        break;
                    }
                case 720968:
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 280, 442) <= 4 && client.Player.Map == 1000)
                        {

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1507, 1);
                            client.CreateBoxDialog("You~put~the~Antidote~in~the~Moon~Spring~and~the~water~became~clear!");
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        else
                        {
                            client.CreateBoxDialog("Get~closer~to~put~the~Antidote.");
                        }

                        break;
                    }
                case 720938:
                    {

                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 293, 450) <= 12 && client.Player.Map == 1000)
                        {

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1457, 1);
                            client.CreateBoxDialog("You`ve~filled~the~Emerald~Bottle~with~water.~Now~go~report~to~Taoist~Yu.");
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ll~need~to~collect~water~on~the~platform~in~the~Moon~Spring.");
                        }
                        break;
                    }
                case 720966:
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 292, 453) <= 3 && client.Player.Map == 1000)
                        {
                            client.Player.AddFlag(MsgUpdate.Flags.Poisoned, 5, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1503, 1);
                            client.CreateBoxDialog("You~worshipped~the~god~of~water.~Go~report~to~the~Taoist.");
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        else
                        {
                            client.CreateBoxDialog("Get~closer~to~use~the~offering.");
                        }
                        break;
                    }
                case 720967:
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 292, 453) <= 15 && client.Player.Map == 1000)
                        {

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1504, 1);
                            client.CreateBoxDialog("You~drank~the~Moon~Spring~and~instantly~felt~dizzy.~Go~report~to~Spring~Soldier~Zhao.");
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        else
                        {
                            client.CreateBoxDialog("Get~closer~to~use~the~offering.");
                        }
                        break;
                    }
                case 720965:
                    {
                        if (client.Player.Map == 1000)
                        {
                            if (Role.Core.GetDistance(559, 313, client.Player.X, client.Player.Y) <= 8)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1489, 1))
                                {

                                    client.CreateBoxDialog("You`ve~already~planted~a~cactus~here.~Go~try~other~places.");
                                }
                                else
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1489, 1);
                                    client.CreateBoxDialog("You`ve~planted~a~cactus~see.");
                                }
                                if (client.Player.QuestGUI.CheckObjectives(1489, 1, 1, 1))
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else if (Role.Core.GetDistance(360, 460, client.Player.X, client.Player.Y) <= 8)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1489, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~already~planted~a~cactus~here.~Go~try~other~places.");
                                }
                                else
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1489, 0, 1);
                                    client.CreateBoxDialog("You`ve~planted~a~cactus~see.");
                                }
                                if (client.Player.QuestGUI.CheckObjectives(1489, 1, 1, 1))
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else if (Role.Core.GetDistance(416, 355, client.Player.X, client.Player.Y) <= 8)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1489, 0, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~already~planted~a~cactus~here.~Go~try~other~places.");
                                }
                                else
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1489, 0, 0, 1);
                                    client.CreateBoxDialog("You`ve~planted~a~cactus~see.");
                                }
                                if (client.Player.QuestGUI.CheckObjectives(1489, 1, 1, 1))
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            }
                            else
                                client.CreateBoxDialog("You`ve~planted~a~cactus~see.");
                        }
                        break;
                    }

                case 720961:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (Role.Core.GetDistance(408, 349, client.Player.X, client.Player.Y) <= 10
                            && client.Player.QuestGUI.CheckQuest(1486, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (DateTime.Now > client.Player.SickleStamp2.AddSeconds(10))
                            {
                                client.Player.SickleStamp2 = DateTime.Now;
                                if (Role.Core.Rate(30))
                                {
                                    client.Inventory.Add(stream, 711403);
                                    client.CreateBoxDialog("You`ve~got~a~Cactus!");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "leedragonblood");
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    client.CreateBoxDialog("You~were~hurt~by~the~cactus!");
                                }
                            }
                            else
                                client.CreateBoxDialog("Please~wait~10~seconds~to~use~the~Sickle,~again.");
                        }
                        else
                        {
                            client.CreateBoxDialog("Use~the~item~in~(408,349).");
                        }

                        break;
                    }
                case 722700:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GreatMerchant, client.Player.Class, 706);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.SendSysMesage("You've~used~the~Exp~Potion.~Report~to~the~Great~Merchant~as~soon~as~possible.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                            client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        }
                        client.Player.RateExp = 2;
                        client.Player.DExpTime = 10 * 60;
                        client.Player.CreateExtraExpPacket(stream);
                        break;
                    }
                case 721779:
                    {
                        if (client.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(313, 380, client.Player.X, client.Player.Y) <= 5 && client.Player.Map == 1015)
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                                Database.Server.AddMapMonster(stream, client.Map, 2156, client.Player.X, client.Player.Y, 1, 1, 3, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                                client.CreateBoxDialog("The~pond~is~bursting~with~Trout~Elves.~Hurry~up~and~catch~them!");
                            }
                            else
                            {
                                client.CreateBoxDialog("You~should~go~to~Bird~Island~(313,380)~and~use~the~Fishing~Net~there.");
                            }
                        }
                        break;
                    }
                case 721931:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.OnBluedBird)
                            {
                                if (DateTime.Now < client.Player.BlueBirdPlumeStamp)
                                {
                                    client.Player.OnBluedBird = false;
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1850, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1850, 10))
                                    {
                                        client.Player.Money += 100;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                        client.SendSysMesage("You received 100 Silver!");
                                        client.Player.QuestGUI.FinishQuest(1850);

                                        client.CreateBoxDialog("You`ve~finished~the~10th~step.~Now~commence~the~11th~step.");
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");

                                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1851);
                                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                                    }
                                    else
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "fire4");
                                }
                                else
                                    client.CreateBoxDialog("3~seconds~have~passed.~You~cannot~ignite~this~corpse~any~more.~Try~others.");
                            }
                            else
                                client.CreateBoxDialog("You~can~only~use~it~near~the~corpses~of~Minotaurs~L120.");
                        }
                        break;
                    }
                case 721929:
                    {
                        if (client.Player.QuestGUI.FinishQuest(1848))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");

                            client.CreateBoxDialog("You`ve~finished~the~8th~step.~Now~commence~the~9th~step.");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1849);
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        }
                        break;
                    }
                case 721928:
                    {
                        if (client.Inventory.Contain(721928, 1))
                        {
                            if (client.Inventory.Contain(721929, 1))
                            {
                                client.CreateBoxDialog("It~is~a~beautiful~fire,~just~the~one~you~can~use~to~roast~the~Fiend~Bat~Wing.");
                            }
                            else
                            {
                                if (client.Player.QuestGUI.CheckQuest(1848, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {

                                    client.Inventory.Remove(721928, 1, stream);
                                    if (Role.Core.Rate(1, 3))
                                    {
                                        if (client.Inventory.HaveSpace(1))
                                        {
                                            client.Inventory.Add(stream, 721929, 1);
                                            client.CreateBoxDialog("The~Fiend~Bat~Wing~was~well~roasted.");
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "bigfire01");
                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("Your~inventory~is~full.");
                                        }
                                    }
                                    else
                                    {
                                        client.CreateBoxDialog("The~Fiend~Bat~Wing~was~overdone.~Find~another~wing~and~roast~it~carefully.");
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "Disapear3");
                                    }
                                }
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("It~is~a~beautiful~fire,~just~the~one~you~can~use~to~roast~the~Fiend~Bat~Wing.");
                        }
                        break;
                    }
                case 721926:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1845, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Inventory.Remove(721926, 1, stream);
                            if (Role.Core.Rate(1, 3))
                            {
                                client.Player.QuestGUI.FinishQuest(1845);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1846);
                                client.Player.QuestGUI.Accept(ActiveQuest, 0);

                                client.CreateBoxDialog("You~left~teeth~marks~and~finished~the~5th~step.~Commence~the~6th~step~now.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                            }
                            else
                            {
                                if (client.Player.HitPoints < 1000)
                                {
                                    client.CreateBoxDialog("Well,~instead~of~leaving~teeth~marks~on~it,~you~destroyed~it.");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "RedThunder");
                                }
                                else
                                {
                                    client.Player.HitPoints -= 500;
                                    client.Player.SendUpdateHP();
                                    client.CreateBoxDialog("Well,~instead~of~leaving~teeth~marks~on~it,~you~destroyed~it.");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "RedThunder");
                                }
                            }
                        }
                        break;
                    }
                case 729976:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1306, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 553, 607) < 25)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1306, 1);
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "M_Fire1");
                                client.SendSysMesage("You brun! the Vulture Feather.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                            }
                            else
                            {
                                client.CreateBoxDialog("Climb~up~to~the~City~Wall~at~(554,606)~to~burn~it!");

                            }
                        }
                        break;
                    }
                case 3007870:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 2375);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            client.CreateBoxDialog("You already finished the quest Spirit Beads!");
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId);
                        client.SendSysMesage("You've completed the quest. Spirit Beads` and received 360 minutes of EXP, 48-hour~Heaven~Blessing, 100Study Points and a Chi Token!", MsgMessage.ChatMode.System);

                        client.GainExpBall(600 * 6);
                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                        client.Inventory.Add(stream, 729304, 1);
                        client.Player.AddHeavenBlessing(stream, 60 * 60 * 48);

                        break;
                    }
                case 3007311:
                    {
                        var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6366);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            client.CreateBoxDialog("You already finished the quest Rare Materials`!");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId);

                        client.SendSysMesage("You've completed the quest. Rare Materials` and received 240 minutes of EXP, 70Study Points and a Chi Token!", MsgMessage.ChatMode.System);

                        client.GainExpBall(600 * 4);
                        client.Player.SubClass.AddStudyPoints(client, 70, stream);
                        client.Inventory.Add(stream, 729304, 1);
                        break;
                    }

                case 3008729:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DesertGuardian, client.Player.Class, 6049);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            client.CreateBoxDialog("You already finished the quest Release the Souls!");
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId);
                        client.SendSysMesage("You've completed the quest. Release the Souls` and received 120 minutes of EXP, 50 Study Points and a Chi Token!", MsgMessage.ChatMode.System);

                        client.GainExpBall(600 * 2);
                        client.Player.SubClass.AddStudyPoints(client, 50, stream);
                        client.Inventory.Add(stream, 729304, 1);
                        break;
                    }
                case 728246:
                    {



                        if (client.Player.Class <= 15)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Remove(728246, 1, stream);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 130078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Sacred~Armor!");
                            }
                            else
                            {
                                client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                            }
                        }
                        else
                        {
                            if (client.Player.Class <= 25)
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    client.Inventory.Remove(728246, 1, stream);
                                    client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.Inventory.Add(stream, 131078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Lion~Armor!");
                                }
                                else
                                {
                                    client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                                }
                            }
                            else
                            {
                                if (client.Player.Class <= 45)
                                {
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.Remove(728246, 1, stream);
                                        client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.Inventory.Add(stream, 133068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Shark~Coat!");
                                    }
                                    else
                                    {
                                        client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                                    }
                                }
                                else
                                {
                                    if (client.Player.Class <= 55)
                                    {
                                        if (client.Inventory.HaveSpace(1))
                                        {
                                            client.Inventory.Remove(728246, 1, stream);
                                            client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                            client.Inventory.Add(stream, 135078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Bear~Vest!");
                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                                        }
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 65)
                                        {
                                            if (client.Inventory.HaveSpace(1))
                                            {
                                                client.Inventory.Remove(728246, 1, stream);
                                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 136078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Tough~Frock!");
                                            }
                                            else
                                            {
                                                client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                                            }
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 75)
                                            {
                                                if (client.Inventory.HaveSpace(1))
                                                {
                                                    client.Inventory.Remove(728246, 1, stream);
                                                    client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                    client.Inventory.Add(stream, 139078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                    client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Pirate~clothing~(L87)!");
                                                }
                                                else
                                                {
                                                    client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                                                }
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 85)
                                                {
                                                    if (client.Inventory.HaveSpace(1))
                                                    {
                                                        client.Inventory.Remove(728246, 1, stream);
                                                        client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                        client.Inventory.Add(stream, 138078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                        client.CreateBoxDialog("You~received~an~Level~87~Elite~Yellow~Combat~Suit~and~an~EXP~Ball!");
                                                    }
                                                    else
                                                    {
                                                        client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                                                    }
                                                }
                                                else if (client.Player.Class <= 145)
                                                {
                                                    if (client.Inventory.HaveSpace(1))
                                                    {
                                                        client.Inventory.Remove(728246, 1, stream);
                                                        client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                        client.Inventory.Add(stream, 134078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                        client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Full~Frock!");
                                                    }
                                                    else
                                                    {
                                                        client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                                                    }
                                                }
                                                else
                                                {
                                                    if (client.Inventory.HaveSpace(1))
                                                    {
                                                        client.Inventory.Remove(728246, 1, stream);
                                                        client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                        client.Inventory.Add(stream, 170078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                        client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~WindWalker~Armor!");
                                                    }
                                                    else
                                                    {
                                                        client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
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
                case 720939:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1455);
                        if (!client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 465, 676) <= 5)
                            {
                                client.CreateBoxDialog("You`ve~filled~the~Jade~Flask~with~water.");

                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                            }
                            else
                            {
                                client.CreateBoxDialog("Go~fetch~water~near~(465,676)");

                            }
                        }
                        else if (!client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 503, 684) <= 5)
                            {
                                client.CreateBoxDialog("You`ve~filled~the~Jade~Flask~with~water.");

                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                            }
                            else
                            {
                                client.CreateBoxDialog("Go~fetch~water~near~(503,684)");

                            }
                        }
                        else if (!client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 534, 654) <= 5)
                            {
                                client.CreateBoxDialog("You`ve~filled~the~Jade~Flask~with~water.");

                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                            }
                            else
                            {
                                client.CreateBoxDialog("Go~fetch~water~near~(534,654)");

                            }
                        }
                        else if (!client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 1))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 533, 626) <= 5)
                            {
                                client.CreateBoxDialog("You`ve~filled~the~Jade~Flask~with~water.");

                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
                            }
                            else
                            {
                                client.CreateBoxDialog("Go~fetch~water~near~(533,626)");

                            }

                        }
                        if (client.Player.QuestGUI.CheckObjectives(0, 1, 1, 1, 1))
                            client.CreateBoxDialog("Now~go~back~to~report~to~the~DC~Vice~General.");

                        break;
                    }
                case 720845:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1354, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1354, 1);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "M_Fire1");
                            client.CreateBoxDialog("You~feel~like~your~body~is~burning!");

                            break;
                        }
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.ClikerON,
                            Fascing = 7,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            dwParam = 0x0c,
                            PacketStamp = 0

                        };
                        client.Send(stream.ActionCreate(&action));

                        break;
                    }
                case 720970:
                    {
                        if (client.Player.Map == 1000)
                        {
                            if (Role.Core.GetDistance(470, 331, client.Player.X, client.Player.Y) <= 2)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1478, 1))
                                {
                                    client.CreateBoxDialog("You`ve~already~repaired~the~damage~here.~Go~try~the~other~places.");
                                }
                                else
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1478, 1);
                                    client.CreateBoxDialog("You`ve~set~up~a~slabstone~on~the~west~to~block~sand~from~swallowing~the~water~pool.");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                }
                            }


                            if (Role.Core.GetDistance(478, 341, client.Player.X, client.Player.Y) <= 2)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1478, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~already~repaired~the~damage~here.~Go~try~the~other~places.");
                                }
                                else
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1478, 0, 1);
                                    client.CreateBoxDialog("You`ve~set~up~a~slabstone~on~the~west~to~block~sand~from~swallowing~the~water~pool.");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                }
                            }
                            if (Role.Core.GetDistance(490, 332, client.Player.X, client.Player.Y) <= 2)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1478, 0, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~already~repaired~the~damage~here.~Go~try~the~other~places.");
                                }
                                else
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1478, 0, 0, 1);
                                    client.CreateBoxDialog("You`ve~set~up~a~slabstone~on~the~west~to~block~sand~from~swallowing~the~water~pool.");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                }
                            }
                            if (Role.Core.GetDistance(476, 318, client.Player.X, client.Player.Y) <= 2)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1478, 0, 0, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~already~repaired~the~damage~here.~Go~try~the~other~places.");
                                }
                                else
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1478, 0, 0, 0, 1);
                                    client.CreateBoxDialog("You`ve~set~up~a~slabstone~on~the~west~to~block~sand~from~swallowing~the~water~pool.");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                }
                            }
                        }
                        break;
                    }
                case 720972:
                    {
                        if (client.Player.Map == 1000)
                        {
                            if (Role.Core.GetDistance(513, 431, client.Player.X, client.Player.Y) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1470, 1))
                                {
                                    client.CreateBoxDialog("You`ve~planted~a~seed~here.~Go~try~the~other~spots.");
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1470, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1470, 1, 1, 1))
                                    {
                                        client.CreateBoxDialog("You`ve~planted~3~trees.~Hurry~to~report~to~Wan~Ying!");
                                    }
                                    else
                                        client.CreateBoxDialog("You`ve~planted~a~tree!");

                                }
                            }
                            if (Role.Core.GetDistance(550, 431, client.Player.X, client.Player.Y) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1470, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~planted~a~seed~here.~Go~try~the~other~spots.");
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1470, 0, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1470, 1, 1, 1))
                                    {
                                        client.CreateBoxDialog("You`ve~planted~3~trees.~Hurry~to~report~to~Wan~Ying!");
                                    }
                                    else
                                        client.CreateBoxDialog("You`ve~planted~a~tree!");

                                }
                            }
                            if (Role.Core.GetDistance(511, 407, client.Player.X, client.Player.Y) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1470, 0, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~planted~a~seed~here.~Go~try~the~other~spots.");
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1470, 0, 0, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1470, 1, 1, 1))
                                    {
                                        client.CreateBoxDialog("You`ve~planted~3~trees.~Hurry~to~report~to~Wan~Ying!");
                                    }
                                    else
                                        client.CreateBoxDialog("You`ve~planted~a~tree!");

                                }
                            }
                        }
                        break;
                    }
                case 720993:
                case 711458:
                    {
                        if (client.Inventory.Contain(720993, 1) && client.Inventory.Contain(711458, 1))
                        {
                            client.Inventory.Remove(720993, 1, stream);
                            client.Inventory.Remove(711458, 1, stream);
                            client.Inventory.Add(stream, 711459);
                            client.SendSysMesage("The Hawk Blood revealed the Blank Paper. It`s a treasure map!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 720986:
                    {
                        client.SendSysMesage("The feathers have been burned to ash.", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1597, 1);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "M_Fire1");
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 720991:
                    {
                        client.SendSysMesage("You've buried the Portait under the tree. Now you can report back to Soldier Fei.", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1617, 1);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "M_Fire1");
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 720992:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1619, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 590, 745) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1619, 1))
                                {
                                    return;
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "M_Fire1");
                                    client.SendSysMesage("You bumt the pill, and it`s giving off some strange smell..", MsgMessage.ChatMode.System);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1618, 1);
                                }
                            }
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 333, 464) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1619, 0, 1))
                                {
                                    return;
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "M_Fire1");
                                    client.SendSysMesage("You bumt the pill, and it`s giving off some strange smell..", MsgMessage.ChatMode.System);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1619, 0, 1);
                                }
                            }
                        }
                        break;
                    }
                case 720990:
                    {

                        if (client.Player.QuestGUI.CheckQuest(1612, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 330, 488) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1612, 1))
                                {
                                    client.SendSysMesage("The place has been reinforced. Go to the next place!", MsgMessage.ChatMode.System);
                                    break;
                                }
                                else
                                {
                                    client.SendSysMesage("You've successfully reinforced the magical circle.", MsgMessage.ChatMode.System);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1612, 1);
                                }
                            }
                            else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 372, 597) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1612, 0, 1))
                                {
                                    client.SendSysMesage("The place has been reinforced. Go to the next place!", MsgMessage.ChatMode.System);
                                    break;
                                }
                                else
                                {
                                    client.SendSysMesage("You've successfully reinforced the magical circle.", MsgMessage.ChatMode.System);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1612, 0, 1);
                                }
                            }
                            else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 507, 734) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1612, 0, 0, 1))
                                {
                                    client.SendSysMesage("The place has been reinforced. Go to the next place!", MsgMessage.ChatMode.System);
                                    break;
                                }
                                else
                                {
                                    client.SendSysMesage("You've successfully reinforced the magical circle.", MsgMessage.ChatMode.System);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1612, 0, 0, 1);
                                }
                            }
                            else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 425, 477) <= 3)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1612, 0, 0, 0, 1))
                                {
                                    client.SendSysMesage("The place has been reinforced. Go to the next place!", MsgMessage.ChatMode.System);
                                    break;
                                }
                                else
                                {
                                    client.SendSysMesage("You've successfully reinforced the magical circle.", MsgMessage.ChatMode.System);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1612, 0, 0, 0, 1);
                                }
                            }
                            else
                                client.SendSysMesage("Go to the 4 comers of Bird Island: (424,477), (507,734), (372,597) and (330,488), then use the token.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 721713:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1792, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(269, 215, client.Player.X, client.Player.Y) <= 2)
                            {
                                client.CreateBoxDialog("This~is~Ghost~City.~You~should~be~able~to~find~Mo~Mo~near~(145,134).");
                                client.Teleport(151, 102, 1780);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "soul-1");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e236");
                            }
                            else if (Role.Core.GetDistance(298, 205, client.Player.X, client.Player.Y) <= 2)
                            {
                                client.CreateBoxDialog("This~is~Ghost~City.~You~should~be~able~to~find~Mo~Mo~near~(145,134).");
                                client.Teleport(151, 102, 1780);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "soul-1");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e236");

                            }
                            else if (Role.Core.GetDistance(357, 304, client.Player.X, client.Player.Y) <= 2)
                            {
                                client.CreateBoxDialog("This~is~Ghost~City.~You~should~be~able~to~find~Mo~Mo~near~(145,134).");
                                client.Teleport(151, 102, 1780);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "soul-1");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e236");
                            }
                            else
                            {
                                client.CreateDialog(stream, "This only works in designated place.", "");
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(1791, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(269, 215, client.Player.X, client.Player.Y) <= 2)
                            {
                                client.CreateBoxDialog("This~is~Ghost~City.~You~should~be~able~to~find~Mo~Mo~near~(145,134).");
                                client.Teleport(151, 102, 1779);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "soul-1");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e236");
                            }
                            else if (Role.Core.GetDistance(298, 205, client.Player.X, client.Player.Y) <= 2)
                            {
                                client.CreateBoxDialog("This~is~Ghost~City.~You~should~be~able~to~find~Mo~Mo~near~(145,134).");
                                client.Teleport(151, 102, 1779);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "soul-1");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e236");

                            }
                            else if (Role.Core.GetDistance(357, 304, client.Player.X, client.Player.Y) <= 2)
                            {
                                client.CreateBoxDialog("This~is~Ghost~City.~You~should~be~able~to~find~Mo~Mo~near~(145,134).");
                                client.Teleport(151, 102, 1779);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "soul-1");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e236");
                            }
                            else
                            {
                                client.CreateDialog(stream, "This only works in designated place.", "");
                            }
                        }
                        break;
                    }
                case 721717:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1789, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(269, 215, client.Player.X, client.Player.Y) <= 2)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1789, 1))
                                    client.CreateBoxDialog("You~must~set~off~the~fireworks~in~the~specified~place.");
                                else
                                {
                                    client.CreateBoxDialog("You~set~off~the~fireworks~but~didn`t~find~Mo~Mo.~Hurry~to~go~to~the~next~spot.");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1789, 1);
                                }
                            }
                            else if (Role.Core.GetDistance(298, 205, client.Player.X, client.Player.Y) <= 2)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1789, 0, 1))
                                    client.CreateBoxDialog("You~must~set~off~the~fireworks~in~the~specified~place.");
                                else
                                {
                                    client.CreateBoxDialog("You~set~off~the~fireworks~but~didn`t~find~Mo~Mo.~Hurry~to~go~to~the~next~spot.");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1789, 0, 1);
                                }
                            }
                            else if (Role.Core.GetDistance(357, 304, client.Player.X, client.Player.Y) <= 2)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1789, 0, 0, 1))
                                    client.CreateBoxDialog("You~must~set~off~the~fireworks~in~the~specified~place.");
                                else
                                {
                                    client.CreateBoxDialog("You~set~off~the~fireworks~but~didn`t~find~Mo~Mo.~Hurry~to~go~to~the~next~spot.");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1789, 0, 0, 1);
                                }
                            }
                            else
                            {
                                client.CreateBoxDialog("You~must~set~off~the~fireworks~in~the~specified~place.");
                            }
                        }
                        break;
                    }
                case 721909:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (DateTime.Now > client.Player.GallbladerrStamp.AddSeconds(30))
                            {

                                if (client.Player.Map == 1015)
                                {
                                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 450, 720) <= 3)
                                    {
                                        client.CreateDialog(stream, "Hey, move on and get the Bitter Ginseng in the trap within 30 seconds!", "");
                                        MsgServer.MsgGameItem _item = new MsgServer.MsgGameItem();
                                        _item.Color = (Role.Flags.Color)2;
                                        _item.ITEM_ID = 11;
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(_item, 450, 720, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                               , 0, false, client.Map, 30);
                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);
                                    }
                                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 296, 290) <= 3)
                                    {
                                        client.CreateDialog(stream, "Hey, move on and get the Bitter Ginseng in the trap within 30 seconds!", "");
                                        MsgServer.MsgGameItem _item = new MsgServer.MsgGameItem();
                                        _item.Color = (Role.Flags.Color)2;
                                        _item.ITEM_ID = 11;
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(_item, 296, 290, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                               , 0, false, client.Map, 30);
                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);
                                    }
                                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 255, 228) <= 3)
                                    {
                                        client.CreateDialog(stream, "Hey, move on and get the Bitter Ginseng in the trap within 30 seconds!", "");
                                        MsgServer.MsgGameItem _item = new MsgServer.MsgGameItem();
                                        _item.Color = (Role.Flags.Color)2;
                                        _item.ITEM_ID = 11;
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(_item, 255, 228, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                               , 0, false, client.Map, 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);

                                    }
                                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 243, 193) <= 3)
                                    {
                                        client.CreateDialog(stream, "Hey, move on and get the Bitter Ginseng in the trap within 30 seconds!", "");
                                        MsgServer.MsgGameItem _item = new MsgServer.MsgGameItem();
                                        _item.Color = (Role.Flags.Color)2;
                                        _item.ITEM_ID = 11;
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(_item, 243, 193, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                               , 0, false, client.Map, 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);
                                    }
                                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 229, 142) <= 3)
                                    {
                                        client.CreateDialog(stream, "Hey, move on and get the Bitter Ginseng in the trap within 30 seconds!", "");
                                        MsgServer.MsgGameItem _item = new MsgServer.MsgGameItem();
                                        _item.Color = (Role.Flags.Color)2;
                                        _item.ITEM_ID = 11;
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(_item, 229, 142, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                               , 0, false, client.Map, 30);

                                        if (client.Map.EnqueueItem(DropItem))
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);
                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "It is not the right place. You can`t use it. Try somewhere else.", "");
                                    }
                                }

                                client.Player.GallbladerrStamp = DateTime.Now;
                            }
                            else
                                client.CreateDialog(stream, "Use the Gallbladder 30 seconds later.", "");

                        }
                        else
                            client.CreateDialog(stream, "It`s been used. You can`t use it again.", "");
                        break;
                    }
                case 721874:
                    {
                        if (client.Inventory.Contain(711518, 1))
                        {
                            client.Inventory.Remove(711518, 1, stream);
                            client.Inventory.Remove(721874, 1, stream);
                            client.Inventory.Add(stream, 721872);
                            client.CreateBoxDialog("You`ve~obtained~the~Buddha~Relic.~Hurry~up~and~give~it~to~Louis.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You~don`t~have~a~Jinx~Tomb~Bat~Heart!");
                        }
                        break;
                    }
                case 721921:
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 102, 292) <= 7 && client.Player.Map == 1001)
                        {
                            client.Player.AddMapEffect(stream, 102, 292, "zf2-e027");
                            client.SendSysMesage("You used the Ghost Mirror and a trap was set by it. Now herd the Mad Bull here and kill it to capture its ghost.");

                        }
                        else
                        {
                            client.SendSysMesage("You need to use the Ghost Mirror at Heaven Well (102,292) in Mystic Castle.");
                        }
                        break;
                    }
                case 721871:
                    {
                        if (client.Inventory.Contain(721877, 5))
                        {

                            if (client.Inventory.Remove(721877, 5, stream))
                            {
                                client.CreateBoxDialog("This~portrait~is~about~Emperor~Taizong~giving~audience~to~an~ambassador~of~Tibet.");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1832, 0, 1);

                                client.Player.RealPortraitStamp = DateTime.Now;
                                ActionQuery action = new ActionQuery();
                                action.ObjId = client.Player.UID;
                                action.dwParam = 60;
                                action.Type = ActionType.CountDown;
                                client.Send(stream.ActionCreate(&action));
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("You~need~to~collect~5~bottles~of~Bat~Essence~from~the~Bloody~Bats.");
                        }
                        break;
                    }
                case 721803:
                    {
                        if (client.Player.QuestGUI.CheckQuest(404, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721802, 1))
                            {
                                if (client.Inventory.Contain(721803, 1))
                                {
                                    client.Inventory.Remove(721802, 1, stream);
                                    client.Inventory.Remove(721803, 1, stream);
                                    client.Inventory.Add(stream, 721804);
                                    client.SendSysMesage("The Poison Fang and Kunlun Liquid have been combined into the Sacred Water.");
                                }
                                else
                                {
                                    client.SendSysMesage("Sorry, you don`t have a Poison Fang. Maybe the Kunlun Wanderer can help you.");
                                }
                            }
                            else
                            {
                                client.SendSysMesage("Sorry, you don`t have any Kunlun Liquid. Maybe the Kunlun Wanderer can help you.");
                            }
                        }
                        break;
                    }
                case 721802:
                    {
                        if (client.Player.QuestGUI.CheckQuest(404, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721802, 1))
                            {
                                if (client.Inventory.Contain(721803, 1))
                                {
                                    client.Inventory.Remove(721802, 1, stream);
                                    client.Inventory.Remove(721803, 1, stream);
                                    client.Inventory.Add(stream, 721804);
                                    client.SendSysMesage("The Poison Fang and Kunlun Liquid have been combined into the Sacred Water.");
                                }
                                else
                                {
                                    client.SendSysMesage("Sorry, you don`t have a Poison Fang. Maybe the Kunlun Wanderer can help you.");
                                }
                            }
                            else
                            {
                                client.SendSysMesage("Sorry, you don`t have any Kunlun Liquid. Maybe the Kunlun Wanderer can help you.");
                            }
                        }
                        break;
                    }
                case 721806:
                    {
                        if (client.Player.Map == 1794)
                        {
                            client.Teleport(557, 649, 1000);
                            client.Inventory.Remove(721806, 1, stream);
                            client.SendSysMesage("You`ve come back to Desert City. Now take the Posion Fang to the Kunlun Wanderer (490,618).");
                        }
                        else
                            client.SendSysMesage("It can be used in the Viper Cave only.");
                        break;
                    }
                case 721804:
                    {
                        if (client.Player.QuestGUI.CheckQuest(404, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721804, 1))
                            {
                                if (client.Inventory.Contain(721798, 1))
                                {
                                    client.Inventory.Remove(721804, 1, stream);
                                    client.Inventory.Remove(721798, 1, stream);
                                    client.Inventory.Add(stream, 721801, 1);
                                    Dialog data = new Dialog(client, stream);
                                    data.AddText("The Tang Silk has been dyed with Sacred Water. A complicated poem appears on the silk. Now take this Poem Silk to Sugar Tang in");
                                    data.AddText("~Bird Island (685,599). This poem was written by her grandma and a man called Mr. Loneliness. She may know something.");
                                    data.AddOption("Gotcha.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.SendSysMesage("Sorry, you don`t have the Tang Silk. Maybe Villager Chou can help you.");
                                }
                            }
                            else
                            {
                                client.SendSysMesage("Sorry, you don`t have any Sacred Water. Maybe the Kunlun Wanderer can help you.");
                            }
                        }
                        break;
                    }
                case 721801:
                    {
                        Dialog data = new Dialog(client, stream);
                        data.AddText("The Tang Silk has been dyed with Sacred Water. A complicated poem appears on the silk. Now take this Poem Silk to Sugar Tang in");
                        data.AddText("~Bird Island (685,599). This poem was written by her grandma and a man called Mr. Loneliness. She may know something.");
                        data.AddOption("Gotcha.");
                        data.FinalizeDialog();
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
