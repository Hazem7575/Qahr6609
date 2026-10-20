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
        [NpcAttribute(NpcID.MarketCpAdmin)]
        public static void MarketCpAdmin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Do~you~know~how~I~desire~the~DragonBall?~I~would~like~to~pay~all~I~have~for~DragonBalls.~Would~you~like~to~exchange")
                            .AddText("DragonBalls~for~Chi~Points?~I~will~pay~you~800~Chi~Points~for~a~DragonBall,~and~8000~Chi~Points~for~a~DBScroll.~What~about~the~deal?")
                            .AddOption("Exchange~a~DBScroll.", 1)
                       .AddOption("Exchange~a~DragonBall.", 2)
                       .AddOption("I~have~nothing.", 255).AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBallScroll, 1))
                        {
                            if (!client.Inventory.Remove(Database.ItemType.DragonBallScroll, 1, stream)) break;
                            if (client.Player.MyChi != null)
                            {
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 8000;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            }
                            client.CreateBoxDialog("Congratulations!You~have~got~8000~Chi~points.");
                        }
                        else
                        {
                            data.AddText("Where~is~your~DBScroll?")
                          .AddOption("Er,~sorry.~I'm not have DBScroll.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove2(Database.ItemType.DragonBall, 1,stream,0))
                        {
                           
                            if (client.Player.MyChi != null)
                            {
                                client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 800;
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                            }
                            client.CreateBoxDialog("Congratulations!You~have~got~800~Chi~points.");
                        }
                        else
                        {
                            data.AddText("Where~is~your~DragonBall?")
                          .AddOption("Er,~sorry.~I'm not have Dragon Ball.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("You wana to change 10.000.000 Money for 100 ConquerPoints? or 100.000.000 Money for 1000 ConquerPoints?")
              .AddOption("Change 10kk for 100CPs", 4)
              .AddOption("Change 100kk for 1000CPs", 5)
              .AddOption("No Thanks", 255).AddAvatar(7).FinalizeDialog();

                        break;
                    }
              
             
            }
        }
        [NpcAttribute(NpcID.Steed1)]
        public static void Steed1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (!client.Inventory.Contain(728525, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1.");
                return;
            }



            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: black, brown or white.")
                            .AddOption("Black horse", 1)
                       .AddOption("Brown horse", 2)
                       .AddOption("White horse", 3)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Remove(728525, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 255, 0, 150);

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(728525, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 0, 150, 255);

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Remove(728525, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 150, 255, 0);

                        break;
                    }
            }
        }
        //DazzlingDiamondBox
        [NpcAttribute(NpcID.DazzlingDiamondBox)]
        public static void DazzlingDiamondBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3008732, 1) && !client.Inventory.Contain(3008732, 1, 1))
            {
                //Sun~Diamond,~Moon~Diamond,~Star~Diamond~and~Cloud~Diamond
                client.CreateBoxDialog("Sorry, you don`t have a DazzlingDiamondBox.");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a 30 pieces Diamond: Sun~Diamond, Moon~Diamond, Star~Diamond or Cloud~Diamond.")
                           .AddOption("Sun~Diamond", 1)
                      .AddOption("Moon~Diamond", 2)
                      .AddOption("Star~Diamond", 3)
                      .AddOption("Cloud~Diamond", 4)
                      .AddOption("Nevermind", 255)
                      .FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Remove(3008732, 1, stream)) break;
                            client.Inventory.AddItemWitchStack(721533, 0, 30, stream);

                            client.CreateBoxDialog("You receive 30 pieces of the Sun~Diamond.");
                        }
                        else
                        {
                            data.AddText("Please make 30 spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .FinalizeDialog();
                        }



                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Remove(3008732, 1, stream)) break;
                            client.Inventory.AddItemWitchStack(721534, 0, 30, stream);

                            client.CreateBoxDialog("You receive 30 pieces of the Moon~Diamond.");
                        }
                        else
                        {
                            data.AddText("Please make 30 spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .FinalizeDialog();
                        }



                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Remove(3008732, 1, stream)) break;
                            client.Inventory.AddItemWitchStack(721535, 0, 30, stream);

                            client.CreateBoxDialog("You receive 30 pieces of the Star~Diamond.");
                        }
                        else
                        {
                            data.AddText("Please make 30 spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Remove(3008732, 1, stream)) break;
                            client.Inventory.AddItemWitchStack(721536, 0, 30, stream);

                            client.CreateBoxDialog("You receive 30 pieces of the Cloud~Diamond.");
                        }
                        else
                        {
                            data.AddText("Please make 30 spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .FinalizeDialog();
                        }



                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.Steed3)]
        public static void Steed3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(728526, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1.");
                return;
            }


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: black, brown or white.")
                           .AddOption("Black horse", 1)
                      .AddOption("Brown horse", 2)
                      .AddOption("White horse", 3)
                      .AddOption("Nevermind", 255)
                      .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Remove(728526, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 255, 0, 150);

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(728526, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 0, 150, 255);

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Remove(728526, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 150, 255, 0);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Steed6)]
        public static void Steed6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(728527, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1.");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: black, brown or white.")
                            .AddOption("Black horse", 1)
                       .AddOption("Brown horse", 2)
                       .AddOption("White horse", 3)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Remove(728527, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 255, 0, 150);

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(728527, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 0, 150, 255);

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Remove(728527, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 150, 255, 0);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RareSteedPack6)]
        public static void RareSteedPack6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3001406, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a NobleSteedPack");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: SpitfireSteed, FrostbiteSteed, BlazehoofSteed, SpottedSteed or Zebra.")
                            .AddOption("+6~Spitfire~Steed", 1)
                       .AddOption("+6~Frostbite~Steed", 2)
                       .AddOption("+6~Blazehoof~Steed", 3)
                       .AddOption("+6~Spotted~Steed", 4)
                       .AddOption("+6~Zebra", 5)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Remove(3001406, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 201, 135, 187);

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(3001406, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 184, 63, 62);

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Remove(3001406, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 137, 156, 148);

                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.Remove(3001406, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 122, 134, 147);

                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.Remove(3001406, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 46, 39, 142);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KitMerchant)]
        public static void KitMerchant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        {
                            data.AddText("Well,hello there! I'm here selling special kits for all the different classes.See anything you like? We've got Elite kits, cheap and sturdy, or Super kits for the more afluent adventurer.Which kind of kit were you interested in? The Super or Elite kits?")
                        .AddOption("Super~kits.", 1)
                        .AddOption("Elite~kits.", 2)
                        .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 105:
                    {

                        {
                            data.AddText("Well,hello there! I'm here selling special kits for all the different classes.See anything you like? We've got Elite kits, cheap and sturdy, or Super kits for the more afluent adventurer.Which kind of kit were you interested in? The Super or Elite kits?")
                        .AddOption("Super~kits.", 1)
                        .AddOption("Elite~kits.", 2)
                        .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                #region Elite Kits
                case 2:
                    {
                        data.AddText("There are some options.");
                        if (client.Player.Class >= 60 && client.Player.Class <= 65)
                            data.AddOption("Elite~Monk~Outfit.", 60); //done
                        else if (client.Player.Class >= 50 && client.Player.Class <= 55)
                            data.AddOption("Elite~Ninja~Outfit.", 61); //done
                        else if (client.Player.Class >= 10 && client.Player.Class <= 15)
                            data.AddOption("Elite~Trojan~Outfit.", 62); //done
                        else if (client.Player.Class >= 20 && client.Player.Class <= 25)
                            data.AddOption("Elite~Warrior~Outfit.", 63); //done
                        else if (client.Player.Class >= 40 && client.Player.Class <= 45)
                            data.AddOption("Elite~Archer~Outfit.", 64); //done
                        else if (client.Player.Class >= 70 && client.Player.Class <= 75)
                            data.AddOption("Elite~Pirate~Outfit.", 65); //done
                        else if (client.Player.Class >= 80 && client.Player.Class <= 85)
                            data.AddOption("Elite~Long~Lee~Outfit.", 66); //DONE
                        else if (client.Player.Class >= 100 && client.Player.Class <= 145)
                            data.AddOption("Elite~Water-Fire~Outfit.", 67);
                        if (client.Player.Class >= 100 && client.Player.Class <= 145)
                            data.AddOption("Common~Elite~outfit~B.", 49);
                        else
                            data.AddOption("Common~Elite~outfit~A.", 68);

                        data.AddOption("Anything~else?", 105)
                        .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                #endregion
                #region Water
                case 67:
                    {
                        data.AddText("This pack contains an~Elite~L70~Great~Backsword,~an~Elite~L70~Crane~Vestment,~an~Elite~L67~Shark~Cap~and~3~+1~Stones.")
                     .AddOption("Yes.", 40)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 40:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720710, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteTaoistKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Long-lee
                case 66:
                    {
                        data.AddText("This pack contains 2 (Elite)HeartNunchaku , 1 (Elite) Level 70 Combat Suit,1 (Elite) level 70 WindHood and~1~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 41)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 41:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 617138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeatNunchaku
                                client.Inventory.Add(stream, 617138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeatNunchaku
                                client.Inventory.Add(stream, 138068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //CombatSuit
                                client.Inventory.Add(stream, 148068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //WindHood
                                client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                                client.SendSysMesage("You received  2 (Elite)HeartNunchaku , 1 (Elite) Level 70 Combat Suit,1 (Elite) level 70 WindHood and~1~+1~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Pirate
                case 65:
                    {
                        data.AddText("This pack contains 1 (Elite)MisteryRapier ,1 (Elite)GlowerPistol, 1 (Elite) Level 70 Tide Coat,1 (Elite) level 70 VikingHood and~1~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 42)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 42:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 611138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //MisteryRapier
                                client.Inventory.Add(stream, 612138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //GlowPistol
                                client.Inventory.Add(stream, 139068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //TideCoat
                                client.Inventory.Add(stream, 145068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //VikingHood
                                client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                                client.SendSysMesage("You received 1 (Elite)MisteryRapier ,1 (Elite)GlowerPistol, 1 (Elite) Level 70 Tide Coat,1 (Elite) level 70 VikingHood and~1~+1~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Archer
                case 64:
                    {
                        data.AddText("This pack contains an~Elite~L70~Goose~Bow,~an~Elite~L67~Ape~Coat,~an~Elite~L72~Ape~Hat~and~3~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 43)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 43:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720708, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteArcherKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Warrior
                case 63:
                    {
                        data.AddText("This pack contains ~L70~Iron~Wand,~an~Elite~L70~Light~Armor,~an~Elite~L67~Heart~of~Ocean~and~3~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 44)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 44:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720706, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteWarriorKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Trojan
                case 62:
                    {
                        data.AddText("This pack contains an~Elite~L70~Shark~Sword,~an~Elite~L70~Rage~Armor,~an~Elite~L67~War~Coronet~and~3~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 45)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 45:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720707, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteTrojanKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Ninja
                case 61:
                    {
                        data.AddText("This pack contains an~Elite~L70~Rain~Katana,~an~Elite~L70~TigerVest,~an~Elite~L67~BloodVeil~and~3~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 46)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 46:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720709, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteNinjaKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Monk
                case 60:
                    {
                        data.AddText("This pack contains  2 Elite level 90 Beads of Eternity, Elite Level 87 Tough Frock,Elite Level 82 Martyr's Headband and~1~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 47)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 47:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 610178, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BeadsofEternity
                                client.Inventory.Add(stream, 610178, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BeadsofEternity
                                client.Inventory.Add(stream, 136078, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //ToughFrock
                                client.Inventory.Add(stream, 143078, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //Martyr`sHeadband
                                client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                                client.SendSysMesage("You received 2 (Elite)Beads of Eternity, 1 (Elite) Level 87 Tough Frock,1 (Elite) level 82 Martyr`sHeadband and~1~+1~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region UniversalKit
                case 68: //sexi
                    {

                        data.AddText("There are some options.");
                        if (client.Player.Class >= 100 && client.Player.Class <= 145)
                            data.AddText("The Elite Universal! Kit A contains an~Elite~L65~Bone~Bracelet,~an~Elite~L67~Ambergris~Bag,~an~Elite~L70~Snakeskin~Boots~and~3~+1~Stones.It costs 749 CPs, and is an incredible bargain! Would you like to buy one?");

                        else if (client.Player.Class >= 100 && client.Player.Class <= 145)
                            data.AddOption("Yeah!", 49);
                        else
                            data.AddText("The Elite Universal! Kit A contains an~Elite~L67~Platina~Necklace,~an~Elite~L70~Pearl~Ring,~an~Elite~L70~Snakeskin~Boots~and~3~+1~Stones.It costs 749 CPs, and is an incredible bargain! Would you like to buy one?");
                        data.AddOption("Yeah!", 50);
                        data.AddOption("Let~me~think.", 1)
                                   .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 49: //wateri
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 749)
                            {
                                client.Player.ConquerPoints -= 749;

                                client.Inventory.AddItemWitchStack(720712, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteUniversalKitB.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 749 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }

                case 50:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 749)
                            {
                                client.Player.ConquerPoints -= 749;

                                client.Inventory.AddItemWitchStack(720711, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteUniversalKitA.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 749 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                ////
                #region Super Kits
                case 1:
                    {
                        data.AddText("There are some options.");
                        if (client.Player.Class >= 60 && client.Player.Class <= 65)
                            data.AddOption("Super~Monk~Outfit.", 10); //done
                        else if (client.Player.Class >= 50 && client.Player.Class <= 55)
                            data.AddOption("Super~Ninja~Outfit.", 11); //done
                        else if (client.Player.Class >= 10 && client.Player.Class <= 15)
                            data.AddOption("Super~Trojan~Outfit.", 12); //done
                        else if (client.Player.Class >= 20 && client.Player.Class <= 25)
                            data.AddOption("Super~Warrior~Outfit.", 13); //done
                        else if (client.Player.Class >= 40 && client.Player.Class <= 45)
                            data.AddOption("Super~Archer~Outfit.", 14); //done
                        else if (client.Player.Class >= 70 && client.Player.Class <= 75)
                            data.AddOption("Super~Pirate~Outfit.", 15); //done
                        else if (client.Player.Class >= 80 && client.Player.Class <= 85)
                            data.AddOption("Super~Long~Lee~Outfit.", 16); //DONE
                        else if (client.Player.Class >= 100 && client.Player.Class <= 145)
                            data.AddOption("Super~Water-Fire~Outfit.", 17);
                        if (client.Player.Class >= 100 && client.Player.Class <= 145)
                            data.AddOption("Common~Super~outfit~B.", 86);
                        else
                            data.AddOption("Common~Super~outfit~A.", 30);
                        data.AddOption("Anything~else?", 105)
                             .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                #endregion
                #region Water
                case 17:
                    {
                        data.AddText("This pack contains a~Super~L90~War~Backsword,~a~Super~L87~Full~Frock,~a~Super~L82~Dragon~Cap~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 21)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 21:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720703, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperTaoistKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Long-lee
                case 16:
                    {
                        data.AddText("This pack contains 2 (Super)HeartNunchaku , 1 (Super) Level 70 Combat Suit,1 (Super) level 70 WindHood and~3~+2~Stones ! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 9)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 9:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 617139, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeatNunchaku
                                client.Inventory.Add(stream, 617139, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeatNunchaku
                                client.Inventory.Add(stream, 138069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //CombatSuit
                                client.Inventory.Add(stream, 148069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //WindHood
                                client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                                client.SendSysMesage("You received  2 (Super)HeartNunchaku , 1 (Super) Level 70 Combat Suit,1 (Super) level 70 WindHood and~3~+2~Stones .", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 4 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Pirate
                case 15:
                    {
                        data.AddText("This pack contains 1 (Super)MisteryRapier ,1 (Super)GlowerPistol, 1 (Super) Level 70 Tide Coat,1 (Super) level 70 VikingHood and~3~+2~Stones ! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 8)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 8:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 611139, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //MisteryRapier
                                client.Inventory.Add(stream, 612139, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //GlowPistol
                                client.Inventory.Add(stream, 139069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //TideCoat
                                client.Inventory.Add(stream, 145069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //VikingHood
                                client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                                client.SendSysMesage("You received 1 (Super)MisteryRapier ,1 (Super)GlowerPistol, 1 (Super) Level 70 Tide Coat,1 (Super) level 70 VikingHood and~3~+2~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Archer
                case 14:
                    {
                        data.AddText("This pack contains a~Super~L90~Star~Bow,~a~Super~L87~Shark~Coat,~a~Super~L82~Marten~Hat~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 7)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;
                                client.Inventory.AddItemWitchStack(720701, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperArcherKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Warrior
                case 13:
                    {
                        data.AddText("This pack contains a~Super~L90~Exorcising~Wand,~a~Super~L87~Lion~Armor,~a~Super~L82~Dragon~Helmet~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 6)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720699, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperWarriorKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Trojan
                case 12:
                    {
                        data.AddText("This pack contains a~Super~L90~Exorcising~Wand,~a~Super~L87~Lion~Armor,~a~Super~L82~Dragon~Helmet~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 5)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;
                                client.Inventory.AddItemWitchStack(720699, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperWarriorKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Ninja
                case 11:
                    {
                        data.AddText("This pack contains a~Super~L90~Ise~Katana,~a~Super~L87~Bear~Vest,~a~Super~L82~Stealth~Veil~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 4)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;


                                client.Inventory.AddItemWitchStack(720702, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperNinjaKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Monk
                case 10:
                    {
                        data.AddText("This pack contains  2 Super level 90 Beads of Eternity, Super Level 87 Tough Frock,Super Level 82 Martyr's Headband and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 3)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 3:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 610179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BeadsofEternity
                                client.Inventory.Add(stream, 610179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BeadsofEternity
                                client.Inventory.Add(stream, 136079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //ToughFrock
                                client.Inventory.Add(stream, 143079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //Martyr`sHeadband
                                client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                                client.SendSysMesage("You received 2 (Super)Beads of Eternity, 1 (Super) Level 87 Tough Frock,1 (Super) level 82 Martyr`sHeadband and~3~+2~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion

                #region Univeral Kit
                case 30:
                    {

                        data.AddText("There are some options.");
                        if (client.Player.Class >= 100 && client.Player.Class <= 145)
                            data.AddText("The Super Universal! Kit A contains a~Super~L85~Jade~Bracelet,~a~Super~L82~Jade~Bag,~a~Super~L90~Leopard~Boots~and~3~+2~Stones.It costs 749 CPs, and is an incredible bargain! Would you like to buy one?");

                        else if (client.Player.Class >= 100 && client.Player.Class <= 145)
                            data.AddOption("Yeah!", 86);
                        else
                            data.AddText("The Super Universal! Kit A contains a~Super~L90~Crystal~Ring,~a~Super~L82~Basalt~Necklace,~a~Super~L90~Leopard~Boots~and~3~+2~Stones.It costs 749 CPs, and is an incredible bargain! Would you like to buy one?");
                        data.AddOption("Yeah!", 99);
                        data.AddOption("Let~me~think.", 1)
                                   .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 86:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 749)
                            {
                                client.Player.ConquerPoints -= 749;

                                client.Inventory.AddItemWitchStack(720705, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperUniversalKitB.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 749 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }

                case 99: //normal
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 749)
                            {
                                client.Player.ConquerPoints -= 749;
                                client.Inventory.AddItemWitchStack(720704, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperUniversalKitA.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                            }
                            else
                            {
                                data.AddText("This pack costs 749 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
            }
        }
        
       [NpcAttribute(NpcID.SoulPacks)]
        public static void SoulPacks(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello " + client.Player.Name + " I'm Sell All Of SoulPackP7 If you Want SoulP7, Fast..")
                       .AddOption("Yes~Let~P7WeaponSoulPack", 1)
                       .AddOption("Yes~Let~P7EquipmentSoulPack", 2)
                       .AddOption("Yes~Let~SacredRefineryPack", 3)             
                       .AddOption("No~Later.", 255)                                      
                       .AddAvatar(63)
                       .FinalizeDialog();
                        break;
                    }

                case 1:
                    {//it's ID Items -> 3006589 -> (1Billion)
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + " Please Fee up some space 1 Slots.");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 1500000)
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + " You don't have Enough Cps I'm Need 1.5M Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 1500000;
                            client.Inventory.Add(stream, 3004247);
                            client.SendSysMesage("Congratz you got a SoulPackP7 in your inventory.");
                        }
                        break;
                    }
                case 2:
                    {//it's ID Items -> 3006589 -> (1Billion)
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + " Please Fee up some space 1 Slots.");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 2000000)
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + "  You don't have Enough Cps I'm Need 2M Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 2000000;
                            client.Inventory.Add(stream, 3004248);//3004249
                            client.SendSysMesage("Congratz you got a SoulPackP7 in your inventory.");
                        }
                        break;
                    }
                case 3:
                    {//it's ID Items -> 3006589 -> (1Billion)
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + " Please Fee up some space 1 Slots.");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 1000000)
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + "  You don't have Enough Cps I'm Need 1M Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 1000000;
                            client.Inventory.Add(stream, 3004249);//3004249
                            client.SendSysMesage("Congratz you got a SacredRefineryPack in your inventory.");
                        }
                         break;
                    }
            }
        }
        [NpcAttribute(NpcID.MoneyPacks)]
        public static void MoneyPacks(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello " + client.Player.Name + " I'm Sell All Of Pack Money If you Want Money, Fast..")
                       .AddOption("Yes~Let~Me~1BillionPack", 1)
                       .AddOption("Yes~Let~Me~500MillionPack", 2)                  
                       .AddOption("Yes~Let~Me~250MillionPack", 3)                        
                       .AddOption("No~Later.", 255)                                         
                       .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 1:
                    {//it's ID Items -> 3006589 -> (1Billion)
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + "<tip color=#E9D0FC> Please Fee up some space 1 Slots.");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 500000)
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + "<tip color=#B763F6>  You don't have Enough Cps I'm Need 500k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 500000;
                            client.Inventory.Add(stream, 3006589);
                            client.SendSysMesage("Congratz you got a Pack~Money in your inventory.");
                        }
                        break;
                    }
                case 2:
                    {//it's ID Items -> 3006588 -> (500Million)
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + "<tip color=#E9D0FC> Please Fee up some space 1 Slots.");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 250000)
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + "<tip color=#B763F6>  You don't have Enough Cps I'm Need 250k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 250000;
                            client.Inventory.Add(stream, 3006588);
                            client.SendSysMesage("Congratz you got a Pack~Money in your inventory.");
                        }
                        break;
                    }
                case 3:
                    {//it's ID Items -> 3006608 -> (250Million)
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + "<tip color=#E9D0FC> Please Fee up some space 1 Slots.");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 150000)
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + "<tip color=#B763F6>  You don't have Enough Cps I'm Need 150k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 150000;
                            client.Inventory.Add(stream, 3006608);
                            client.SendSysMesage("Congratz you got a Pack~Money in your inventory.");
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SellItemsEpic)]
        public static void SellItemsEpic(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        { //852102,Epic's,2,54176,1002,317,283,0,0,1,1
            
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey " + client.Player.Name + " I'm Sell All Of [Weapons] One Of 10K CP[S] .")
                       //.AddOption("Epic Trojan", 6)//614439
                       //.AddOption("Epic Ninja", 5)
                       //.AddOption("Epic Taoist", 4)
                       //.AddOption("Epic TaoistHossu", 3)
                       //.AddOption("Epic Monk", 2)
                       //.AddOption("Epic Pirate", 1) //Epic New 3D -> 671439
                       .AddOption("Let~me~think.", 255)
                       .AddAvatar(63).FinalizeDialog();
                        //}
                        break;
                    }

                case 6:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            // System.Windows.Forms.MessageBox.Show("Character must have 36 free slots into inventory");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 614439);
                            client.SendSysMesage("Congratz you got a Epic in your inventory.");
                            client.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy, stream);
                            client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Goldbrick, Role.StatusFlagsBigVector32.PermanentFlag, false);
                        }
                        break;
                    }


                case 5:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            // System.Windows.Forms.MessageBox.Show("Character must have 36 free slots into inventory");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 616439);
                            client.SendSysMesage("Congratz you got a Epic in your inventory.");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "npcjdgh");
                            client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Goldbrick, Role.StatusFlagsBigVector32.PermanentFlag, false);

                        }
                        break;
                    }

                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            // System.Windows.Forms.MessageBox.Show("Character must have 36 free slots into inventory");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 620439);
                            client.SendSysMesage("Congratz you got a Epic in your inventory.");
                            client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Goldbrick, Role.StatusFlagsBigVector32.PermanentFlag, false);
                        }
                        break;
                    }

                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            // System.Windows.Forms.MessageBox.Show("Character must have 36 free slots into inventory");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 619439);
                            client.SendSysMesage("Congratz you got a Epic in your inventory.");
                            client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Goldbrick, Role.StatusFlagsBigVector32.PermanentFlag, false);
                        }
                        break;
                    }

                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            // System.Windows.Forms.MessageBox.Show("Character must have 36 free slots into inventory");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 10000)
                        {
                            client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Inventory.Add(stream, 622439);
                            client.SendSysMesage("Congratz you got a Epic in your inventory.");
                            client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Goldbrick, Role.StatusFlagsBigVector32.PermanentFlag, false);
                        }
                        break;
                    }

                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please Fee up some space 1 Slots.");


                            // System.Windows.Forms.MessageBox.Show("Character must have 36 free slots into inventory");
                            return;
                        }
                        else if (client.Player.ConquerPoints < 1099988000)
                        {
                            client.SendSysMesage("Hey. " + client.Player.Name + " Not Now For You Get it's Epic.. I'm Sorry, This Is Coming Soon In The Server If Your Ready .");
                            //client.SendSysMesage("You don't have Enough Cps I'm Need 10k Cps.");
                            return;
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 100099980;
                            client.Inventory.Add(stream, 671439);
                            client.SendSysMesage("Congratz you got a Epic in your inventory.");
                            client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Goldbrick, Role.StatusFlagsBigVector32.PermanentFlag, false);
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SellItems)]
        public static void SellItems(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey how I can help you ? I`m Sell Promotion Items.")
                    .AddOption("MoonBox", 1)
                    .AddOption("Emerald", 2)
                    .AddOption("EuxeniteOre", 3)
                    .AddOption("[10]Saddle", 4)
                     .AddOption("YinYangFruit", 5)
                      .AddOption("Bomb", 6)
                       .AddOption("LuckyAmulet", 7)
                         .AddOption("PerformerCard", 8)
                           .AddOption("ModestyBook", 9)
                    .AddOption("Let~me~think.", 255)
                    .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 100)
                            {
                                client.Player.ConquerPoints -= 100;

                                client.Inventory.Add(stream, 723342);
                            }
                            else
                                data.AddText("Please make sure u have 100 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 50)
                            {
                                client.Player.ConquerPoints -= 50;

                                client.Inventory.Add(stream, 711679);
                            }
                            else
                                data.AddText("Please make sure u have 50 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 50)
                            {
                                client.Player.ConquerPoints -= 50;

                                client.Inventory.Add(stream, 723087);
                            }
                            else
                                data.AddText("Please make sure u have 50 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 50)
                            {
                                client.Player.ConquerPoints -= 50;

                                client.Inventory.Add(stream, 711188);
                            }
                            else
                                data.AddText("Please make sure u have 50 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 10)
                            {
                                client.Player.ConquerPoints -= 10;

                                client.Inventory.Add(stream, 721261);
                            }
                            else
                                data.AddText("Please make sure u have 10 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 200)
                            {
                                client.Player.ConquerPoints -= 200;

                                client.Inventory.Add(stream, Database.ItemType.MoonBox);
                            }
                            else
                                data.AddText("Please make sure u have 200 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(10))
                        {
                            if (client.Player.ConquerPoints >= 250)
                            {
                                client.Player.ConquerPoints -= 250;

                                client.Inventory.Add(stream, 723903, 10);
                            }
                            else
                                data.AddText("Please make sure u have 250 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 10 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 100)
                            {
                                client.Player.ConquerPoints -= 100;

                                client.Inventory.Add(stream, 1080001);
                            }
                            else
                                data.AddText("Please make sure u have 100 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 20)
                            {
                                client.Player.ConquerPoints -= 20;

                                client.Inventory.Add(stream, 1072031);
                            }
                            else
                                data.AddText("Please make sure u have 20 Cps to get this item.")
                       .AddOption("Let~me~think.", 255)
                      .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.NobleSteedPack)]
        public static void NobleSteedPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(729990, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a NobleSteedPack");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: SpitfireSteed, FrostbiteSteed, BlazehoofSteed, SpottedSteed or Zebra.")
                            .AddOption("+6~Spitfire~Steed", 1)
                       .AddOption("+6~Frostbite~Steed", 2)
                       .AddOption("+6~Blazehoof~Steed", 3)
                       .AddOption("+6~Spotted~Steed", 4)
                       .AddOption("+6~Zebra", 5)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Remove(729990, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 201, 135, 187);

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(729990, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 184, 63, 62);

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Remove(729990, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 137, 156, 148);

                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.Remove(729990, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 122, 134, 147);

                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.Remove(729990, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 46, 39, 142);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StartNpc)]
        public static void StartNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This~is~the~way~to~TwinCity.~Many~people~are~gathering~there.")
                   .AddOption("Teleport~me~to~TwinCity", 3)
                   .AddOption("Let~me~think.", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("Welcome~in~" + Program.ServerConfig.ServerName + "~you~have~some~free~items!~and~2kk~money.")
                            .FinalizeDialog();


                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                        }
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Superman);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                        }
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.XpFly);
                        }
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalStrike))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalStrike);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                        }
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlwindKick))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WhirlwindKick);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleAttack);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Oblivion))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Oblivion);
                        }
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CannonBarrage))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CannonBarrage);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeTempest))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeTempest);
                        }
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPunch))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonPunch);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonCyclone))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonCyclone);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFlow))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFlow);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirRaid))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirRaid);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirSweep))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirSweep);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirKick))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirKick);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirStrike))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirStrike);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthSweep))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EarthSweep);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Kick))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Kick);
                        }
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChainBolt);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                        }

                        client.Player.Money += 2000000;
                        //   client.Player.ConquerPoints += 500;

                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Inventory.Add(stream, 727266, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 727267, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(stream, 3000550, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None);
                        client.Inventory.AddItemWitchStack(1000000, 0, 15, stream);
                        client.Inventory.AddItemWitchStack(1001000, 0, 15, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 10, stream);
                        client.Teleport(300, 278, 1002);


                        break;
                    }
            }
        }
    }
}
