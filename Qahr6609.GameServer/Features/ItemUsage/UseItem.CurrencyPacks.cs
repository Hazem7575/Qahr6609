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
        // UseItem handlers: CPs / silver / money packs.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_CurrencyPacks(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 721017:
                    {
                        client.Player.ConquerPoints += 10;

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 721019:
                    {

                        client.Player.ConquerPoints += 25;

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 721018:
                    {

                        client.Player.ConquerPoints += 20;

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 721026:
                    {

                        client.Player.ConquerPoints += 50;

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 721027:
                    {

                        client.Player.ConquerPoints += 100;

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3001318:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.Money += 500000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.SendSysMesage("You received 500,000 silver for opening the GloryMoneyBag's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        break;
                    }
                case 3001322:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.Money += 25000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.SendSysMesage("You received 25,000,000 silver for opening the SplendidMoneyBox's.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        break;
                    }
                case 3005061:// BrightActivePack
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 729022, 1);
                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 1);
                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream);
                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 5, stream);
                            client.Player.ConquerPoints += 7000;

                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 3 space in your inventory.");

                        }

                        break;
                    }
                case 3005062:// FortuneActivePack 
                    {
                        if (client.Inventory.HaveSpace(6))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 729022, 2);
                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 2);
                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream);
                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 5, stream);
                            client.Player.ConquerPoints += 10000;

                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 6 space in your inventory.");

                        }
                        break;
                    }
                case 728898:
                    {
                        if (client.Inventory.Contain(728898, 8))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                client.CreateBoxDialog("Please make 1 more space in your inventory");
                                break;
                            }
                            client.Inventory.Remove(728898, 8, stream);

                            client.Inventory.Add(stream, 723714);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e104");
                            client.CreateBoxDialog("You~received~a~Class2~Money~Bag~after~you~combined~8~Tiny~Class2~Money~Pouches!");
                        }
                        else
                            client.CreateBoxDialog("You~need~to~have~8~Tiny~Class2~Money~Pouches~to~combine~and~get~a~Class2~Money~Bag.");
                        break;
                    }
                case 728900:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory");
                            break;
                        }
                        if (client.Inventory.Contain(728900, 8))
                        {
                            client.Inventory.Remove(728900, 8, stream);
                            client.Inventory.Add(stream, 723713);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e104");
                            client.CreateBoxDialog("You~received~a~Class1~Money~Bag~after~you~combined~8~Class1~Money~Pouches!");
                        }
                        else
                            client.CreateBoxDialog("You~need~to~have~8~Class1~Money~Pouches~to~combine~and~get~a~Class1~Money~Bag.");

                        break;
                    }
                case 3303526://GiantCPsPack
                    {
                        client.Inventory.Remove(3303526, 1, stream);
                        client.Player.ConquerPoints += 3000;
                        client.CreateBoxDialog("You~received~3,000~CPs.");
                        break;
                    }
                case 3303527://GiantCPsPack
                    {
                        client.Inventory.Remove(3303527, 1, stream);
                        client.Player.BoundConquerPoints += 3000;
                        client.CreateBoxDialog("You~received~3,000~CPs~(B).");
                        break;
                    }
                case 3006813:
                    {
                        client.Player.ConquerPoints += 10000;
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.SendSysMesage("You~can~gain~10,000~cps.");
                        break;
                    }
                case 3001266:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.Money += 100000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        break;
                    }
                case 729536:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        uint amount = (uint)Program.GetRandom.Next(20, 90);
                        client.Player.ConquerPoints += amount;
                        client.SendSysMesage("You`ve received " + amount.ToString() + " Conquer Points.");

                        break;
                    }
                case 720665:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 60;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.SendSysMesage("You used the Cute CP Pack and received 60 CPs.");
                        break;
                    }


                case 3001133: //HopeCPPack
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 5;
                        client.CreateBoxDialog("You have received 5 ConquerPoints.");
                        break;
                    }



                case 3001134: //MascotCPPack
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 10;
                        client.CreateBoxDialog("You have received 10 ConquerPoints.");
                        break;
                    }



                case 3001135: //MammonCPPack
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 20;
                        client.CreateBoxDialog("You have received 20 ConquerPoints.");
                        break;
                    }



                case 3001136: //DeityCPPack
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 500;
                        client.CreateBoxDialog("You have received 500 ConquerPoints.");
                        break;
                    }


                case 729462: //6,999CPPack(B)
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 6999;
                        client.CreateBoxDialog("You have received 6,999 (B)ConquerPoints.");
                        break;
                    }


                case 729463: //4,999CPPack(B)
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 4999;
                        client.CreateBoxDialog("You have received 4,999 (B)ConquerPoints.");
                        break;
                    }





                case 729464: //2,999CPPack(B)
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 2999;
                        client.CreateBoxDialog("You have received 2,999 (B)ConquerPoints.");
                        break;
                    }


                case 729465: //599CPPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 599;
                        client.CreateBoxDialog("You have received 599 (B)ConquerPoints.");
                        break;
                    }


                case 729466: //99CPPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 99;
                        client.CreateBoxDialog("You have received 99 (B)ConquerPoints.");
                        break;
                    }



                case 3200522: //60CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 60;
                        client.CreateBoxDialog("You have received 60 (B)ConquerPoints.");
                        break;
                    }

                case 3200523: //125CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 125;
                        client.CreateBoxDialog("You have received 125 (B)ConquerPoints.");
                        break;
                    }


                case 3200524: //320CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 320;
                        client.CreateBoxDialog("You have received 320 (B)ConquerPoints.");
                        break;
                    }


                case 3200525: //530CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 530;
                        client.CreateBoxDialog("You have received 530 (B)ConquerPoints.");
                        break;
                    }



                case 3200526: //1075CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 1075;
                        client.CreateBoxDialog("You have received 1075 (B)ConquerPoints.");
                        break;
                    }


                case 3200527: //2050CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 2050;
                        client.CreateBoxDialog("You have received 2050 (B)ConquerPoints.");
                        break;
                    }



                case 3200528: //4200 CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 4200;
                        client.CreateBoxDialog("You have received 4200 (B)ConquerPoints.");
                        break;
                    }



                case 3200700: //20 CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 20;
                        client.CreateBoxDialog("You have received 20 (B)ConquerPoints.");
                        break;
                    }


                case 3200701: //300 CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 300;
                        client.CreateBoxDialog("You have received 300 (B)ConquerPoints.");
                        break;
                    }


                case 3200702: //500 CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 500;
                        client.CreateBoxDialog("You have received 500 (B)ConquerPoints.");
                        break;
                    }



                case 3200703: //5000 CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 5000;
                        client.CreateBoxDialog("You have received 5000 (B)ConquerPoints.");
                        break;
                    }


                case 3200704: //50 CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 50;
                        client.CreateBoxDialog("You have received 50 (B)ConquerPoints.");
                        break;
                    }



                case 3200705: //600 CPsPack(B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 600;
                        client.CreateBoxDialog("You have received 600 (B)ConquerPoints.");
                        break;
                    }




                case 3005646: //200
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ConquerPoints += 200;
                        client.CreateBoxDialog("You have received 200 ConquerPoints.");
                        break;
                    }
                case 3005836:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.Money += 50000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.SendSysMesage("You received 50000000 silver for opening the MediumSilverBag.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        break;
                    }
                case 3005837:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.Money += 100000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.SendSysMesage("You received 100000000 silver for opening the BigSilverBag.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        break;
                    }
                case 3008727:
                    {

                        if (client.Inventory.Contain(3008727, 9))
                        {
                            client.Player.BoundConquerPoints += 270;
                            client.Inventory.Remove(3008727, 9, stream);
                            client.CreateBoxDialog("Success ! You successfully combined all fragments and received 270 CPs(B).");
                        }
                        else
                        {
                            client.SendSysMesage("Failed to combine since you don`t have enough number of fragments.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3300045:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        byte rand = (byte)Program.GetRandom.Next(1, 7);
                        switch (rand)
                        {
                            case 1:
                                {
                                    client.CreateBoxDialog("You've received a SilverDragonslayer(2-handed)!");
                                    client.Inventory.Add(stream, 350103);
                                    break;
                                }
                            case 2:
                                {
                                    client.CreateBoxDialog("You've received a GoldenDragonslayer(2-handed)!");
                                    client.Inventory.Add(stream, 350104);
                                    break;

                                }
                            case 3:
                                {
                                    client.CreateBoxDialog("You've received a BronzeDragonslayer(2-handed)!");
                                    client.Inventory.Add(stream, 350105);
                                    break;
                                }
                            case 4:
                                {
                                    client.CreateBoxDialog("You've received a SilverDragonslayer(1-handed)!");
                                    client.Inventory.Add(stream, 360201);
                                    break;
                                }
                            case 5:
                                {
                                    client.CreateBoxDialog("You've received a GoldenDragonslayer(1-handed)!");
                                    client.Inventory.Add(stream, 360202);
                                    break;
                                }
                            default:
                                {
                                    client.CreateBoxDialog("You've received a BronzeDragonslayer(1-handed)!");
                                    client.Inventory.Add(stream, 360203);
                                    break;
                                }
                        }

                        break;
                    }
                case 3000316:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Inventory.HaveSpace(2) == false)
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (Role.Core.Rate(5))
                        {
                            client.Player.ConquerPoints += 5000;
                            Program.SendGlobalPackets.Enqueue(new MsgMessage("" + client.Player.Name + " got 5000 ConquerPoints while opening the ChristmasPack!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                            client.CreateBoxDialog("You received 5000 ConquerPoints!");
                        }
                       else if (Role.Core.Rate(4))
                        {

                            client.Player.ConquerPoints += 10000;
                            Program.SendGlobalPackets.Enqueue(new MsgMessage("" + client.Player.Name + " got 10.000 ConquerPoints while opening the ChristmasPack!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                            client.CreateBoxDialog("You received 10.000 ConquerPoints!");
                        }
                        else if (Role.Core.Rate(4))
                        {

                            client.Player.ConquerPoints += 1000;
                            Program.SendGlobalPackets.Enqueue(new MsgMessage("" + client.Player.Name + " got 1.000 ConquerPoints while opening the ChristmasPack!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                            client.CreateBoxDialog("You received 1.000 ConquerPoints!");
                        }
                        else
                        {

                            client.Inventory.Add(stream, 3003235, 2);
                            client.CreateBoxDialog("You received a 2*PowerExpBallScrap!");
                            Program.SendGlobalPackets.Enqueue(new MsgMessage("" + client.Player.Name + " got 2*PowerExpBallScrap while opening the ChristmasPack!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                        }

                        break;
                    }
                case 3300053:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3300053, 15))
                        {
                            client.Inventory.Remove(3300053, 15, stream);
                            client.Inventory.Add(stream, 2100195, 1);
                            client.CreateBoxDialog("You've received a SilverPhoenixCup!");
                        }
                        else
                        {
                            client.SendSysMesage("You should have 15 Silver Phoenix Cup Fragments before you can make a complete cup.", MsgMessage.ChatMode.System);
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
