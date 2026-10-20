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
        [NpcAttribute(NpcID.TCViceCaptain)]
        public static void TCViceCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1006);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1007);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1008);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Hi! I`m the TC Vice Captain, and I`m here to guard Twin City. Hey, do you think you can help me deliver a letter?")
                        .AddOption("Tell~me~more!", 1)
                        .AddOption("Just~passing~by.", 255)
                        .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 4;
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 7;
                        }
                        data.AddText("Hi! I`m the TC Vice Captain, and I`m here to guard Twin City. Don`t make trouble, or we`re going to have problems!")
                         .AddOption("I see.", 255)
                         .AddAvatar(18).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(711235, 1))
                        {
                            data.AddText("Well done! You`ve got the letter. Thanks a lot!")
                             .AddOption("No~problem.", 3)
                             .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        data.AddText("I want you to deliver a letter to Xu Liang, but the letter is being held by the Warehouseman. Could you please get it back for me?")
                            .AddOption("No~problem.", 2)
                            .AddOption(" I~have~no~time.", 255)
                            .AddAvatar(18).FinalizeDialog();



                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~retrieve~the~Military~Letter~from~the~Warehouseman.", 1002, 281, 251, 8);


                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        if (!client.Inventory.Remove(711235, 1, stream)) break;
                        client.Player.Money += 100;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.CreateBoxDialog("You~completed~Letter~Delivery~1~and~received~100~silver.");


                        goto case 4;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(711236, 1))
                        {
                            data.AddText("Hi! I`m the TC Vice Captain, and I`m here to guard Twin City. Hmm, do you want to take the quest `Letter Delivery 2`?")
    .AddOption("Yeah,~I`d~like~to.", 6)
    .AddOption(" I~have~no~time.", 255)
    .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Military Letter is quite important, so I need a Turtledove Plume to mark the letter. Could you please hunt some Turtledoves and bring back a Turtledove Plume for me?")
     .AddOption("No~problem.", 5)
     .AddOption(" I~have~no~time.", 255)
     .AddAvatar(18).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hunt~the~Turtledoves~and~collect~a~Turtledove~Plume!", 1002, 340, 484, 0);


                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(2) == false)
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(711236, 1))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            if (!client.Inventory.Remove(711236, 1, stream)) break;
                            client.CreateBoxDialog("You~completed~`Letter~Delivery~2`~and~received~300~Silver,~some~EXP,~5~Resolutives~and~a~Twin~City~Gate!");
                            client.GainExpBall(200, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 300;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.AddItemWitchStack(1000010, 0, 5, stream);
                            client.Inventory.Add(stream, 1060020);
                            goto case 7;
                        }



                        break;
                    }
                case 7:
                    {
                        data.AddText("Now, please hurry and deliver the Military Letter marked with the Turtledove Plume to Xu Liang in Wind Plain! It`s urgent!")
                              .AddOption("Got~it!", 8)
                              .AddOption("Leave.", 255)
                              .AddAvatar(18).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(711237, 1))
                        {
                            data.AddText("Now, please hurry and deliver the Plume Letter to Xu Liang in Wind Plain! It`s urgent!")
                                .AddOption("Got~it!", 255)
                                .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~give~the~Plume~Letter~to~Xu~Liang~in~Wind~Plain.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                        client.Inventory.Add(stream, 711237);


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Shelby)]
        public static void Shelby(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("We~hope~all~can~help~each~other.~If~you~power~level~the~newbies,~we~may~reward~you~Experience~or~meteors.~Are~you~interested?")
                    .AddOption("Tell~me~more~details.", 1)
                    .AddOption("Check~my~virtue~points.", 5)
                    .AddOption("Claim~Prize.", 6)
                    .AddOption("Just~passing~by.", 255)
                    .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("If~you~are~above~level~70~and~try~to~power~level~the~newbies~(at~least~20~levels~lower~than~you),~you~may~gain~virtue~points.")
                            .AddOption("What~are~the~virtue~points?", 2)
                            .AddAvatar(7).FinalizeDialog();



                        break;
                    }
                case 2:
                    {
                        data.AddText("The~more~newbies~you~power~level,~the~more~virtue~points~you~gain.~I~shall~give~you~a~good~reward~for~a~certain~virtue~points.")
                           .AddOption("How~can~I~gain~virtue~points?", 3)
                           .AddOption("What~prize~can~I~expect?", 4)
                           .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("Once~the~newbies~are~one~level~up,~the~team~captain~can~gain~virtue~points~accordingly.")
                             .AddOption("I~see.", 255)
                             .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("I~shall~reward~you~exp.~of~equivalent~to~an~ExpBall~for~15,000~virtue~points~or~a~meteor~for~5000~virtue~points or DragonBall for 20,000 virtue~points.")
                         .AddOption("I~see.", 255)
                         .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("Your~current~virtue~points~are~" + client.Player.VirtutePoints + ",~please~try~to~gain~more.")
                           .AddOption("Thanks.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("What~prize~do~you~prefer?")
                            .AddOption("Meteor.", 10)
                            .AddOption("Exp.", 11)
                            .AddOption("DragonBall", 12)
                            .AddOption("Let~me~think~it~over", 255)
                            .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.VirtutePoints >= 5000)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.VirtutePoints -= 5000;
                                client.Player.SendUpdate(stream, client.Player.VirtutePoints, MsgUpdate.DataType.VirtutePoints);
                                client.Inventory.Add(stream, Database.ItemType.Meteor);
                            }
                            else
                            {
                                data.AddText("Please make 1 more space in your inventory")
                                                         .AddOption("Let me check.", 255).AddAvatar(7)
                                                        .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~the~required~virtue~points.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 11:
                    {
                        if (client.Player.Level >= 130)
                        {
                            data.AddText("Only the characters below level 130 can exchange the virtue points for exp. You may find Simon")
                                .AddText(" (Twin City 393,235) and spend 2,000 virtue points to hunt the treasure in the Labyrinth.")
                              .AddOption("Wow.~So~cool.", 255)
                              .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.VirtutePoints >= 15000)
                        {
                            client.Player.VirtutePoints -= 15000;
                            client.Player.SendUpdate(stream, client.Player.VirtutePoints, MsgUpdate.DataType.VirtutePoints);
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);

                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~the~required~virtue~points.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        if (client.Player.VirtutePoints >= 20000)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.VirtutePoints -= 20000;
                                client.Player.SendUpdate(stream, client.Player.VirtutePoints, MsgUpdate.DataType.VirtutePoints);
                                client.Inventory.Add(stream, Database.ItemType.DragonBall);
                            }
                            else
                            {
                                data.AddText("Please make 1 more space in your inventory")
                                                         .AddOption("Let me check.", 255).AddAvatar(7)
                                                        .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry,~you~do~not~have~the~required~virtue~points.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.ServerRewards)]
        public static void ServerRewards(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                case 30:
                    {
                        data.AddText("Welcome to " + Program.ServerConfig.ServerName + ", i can give you some information or rewards about the game if you'd like.")
                    //.AddOption("Rates.", 1)
                    //.AddOption("Vip Information", 5)
                       .AddOption("Shops", 7)
                     .AddOption("Charge Syestem", 150)
                    .AddOption("OnlineRewards", 10)
                    //.AddOption("TournamentRewards", 20)
                    //.AddOption("My Dropping Layer", 100)
                    //.AddOption("Arena~Cps~Single~Mode.", 200)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();


                        break;
                    }

                case 200:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("sorry, you need to be at keast level 100.")
                        .AddOption("Holy~Shit.", 255).AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Welcome, please insert the amount of cps u want to bet on.")
                       .AddInput("minimum 500 CPS", 201).AddOption("Passing~by.", 255).AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }

                case 150:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = 464,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
                case 201:
                    {
                        // CP-bet announcement: level 100+, a valid bet between 500 and MaxArenaCpsBet that the announcer can pay,
                        // at most one announcement per player per minute and one server-wide every 15 seconds.
                        if (client.Player.Level < 100)
                            break;
                        uint bet;
                        if (!uint.TryParse(Input, out bet) || bet < 500)
                        {
                            client.Player.MessageBox("Minimum 500 CPs.", null, null);
                            break;
                        }
                        if (bet > MaxArenaCpsBet)
                        {
                            client.Player.MessageBox("Maximum " + MaxArenaCpsBet + " CPs.", null, null);
                            break;
                        }
                        if (client.Player.ConquerPoints < bet)
                        {
                            client.Player.MessageBox("You don't have enough CPs.", null, null);
                            break;
                        }
                        if (Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.Working)
                        {
                            client.Player.MessageBox("Minimum 500 CPs.", null, null);
                            break;
                        }
                        // The player's 60 s slot is only consumed when the global 15 s slot is free too.
                        if (!global::Conquer6609.Security.Guard.ThrottleBoth("cpsbet:" + client.Player.UID, 60000, "cpsbet:all", 15000))
                        {
                            client.Player.MessageBox("Please wait before announcing another bet.", null, null);
                            break;
                        }
                        foreach (var user in Database.Server.GamePoll.Values)
                        {
                            user.Player.MessageBox("Cps Tournment War Started Bet " + bet + " Cps", p =>
                            {
                                if (Game.MsgTournaments.MsgSchedules.ArenaCpsBattle.SignUp(p, bet) == false)
                                {
                                    p.SendSysMesage("don't have requierments.", MsgMessage.ChatMode.Service, MsgMessage.MsgColor.yellow);
                                }
                            }, null, 15);
                        }
                        break;
                    }
                case 100:
                    {
                        uint limitavaliable = Database.ServerDatabase.CalculateDropLimitation(client.Player) - client.Player.ConquerPointsLimitaion;
                        if (limitavaliable >= 20)
                        { data.AddText("You have " + limitavaliable.ToString() + " still avaliable to drop from monsters, "); }
                        data.AddText("Your drop layer is [" + client.Player.ConquerPointDropLimitLayer.ToString() + "].")
                          .AddOption("How~to~increase~my~daily~limit?", 101)
                          .AddOption("thank~you~for~informations!", 255)
                          .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 101:
                    {
                        data.AddText("In case of increasing daily limit you have 3 choices \n")
                            .AddText("Choice 1: Level up your character or even have reborn. \n")
                            .AddText("    Note: more level and reborn times means increasing limit. \n")
                            .AddText("Choice 2: Try another character. \n")
                            .AddText("    Note: changing character could make it better. \n")
                            .AddText("Choice 3: Charge drop layer. \n")
                            .AddText("    Note: you can select the drop layer you want. \n")
                            .AddText("[Warning] any drop layer only last for 1 week which means 7 days. \n")
                            .AddText("[Warning] maximum drop layer is 255 layer level you can charge as much as you wish.")
                              .AddOption("I~Would~Like~To~Buy", 102)
    .AddOption("Nuh~don't~like~this~way.", 255)
    .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 102:
                    {
                        client.SendSysMesage(Program.ServerConfig.FBStoreLink, MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.white);
                        break;
                    }
                case 20:
                    {
                        data.AddText("You have " + client.Player.TournamentKills.ToString() + " Tournament Points. Below you can find something rewards")
                          .AddOption("10,000CPs(1000Pts)", 21)
 .AddOption("1,000CPs(100Pts)", 22)

.AddOption("StigmaRing(2000Pts)", 23)
 .AddOption("ShieldNeck(1500Pts)", 24)
 .AddOption("MagicBoot(2500Pts)", 31)
 .AddOption("PoisondRidingCrop(3000Pts)", 32)
.AddOption("[Bottle]Pandora`sShield(2300Pts)", 25)
 .AddOption("Main Menu.", 30)
 .AddOption("Thank you.", 255)
 .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 25:
                    {

                        if (client.Player.TournamentKills >= 2300)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 2100125, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.Shield);
                            client.Player.TournamentKills -= 2300;
                            client.CreateBoxDialog("You received a Pandora`sShield.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Tournament Points.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();

                        }


                        break;
                    }
                case 32:
                    {

                        if (client.Player.TournamentKills >= 3000)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 203009, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.Poison, false, "", 3);
                            client.Player.TournamentKills -= 3000;
                            client.CreateBoxDialog("You received a Super [Poison]RidingCrop.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Tournament Points.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();

                        }

                        break;
                    }
                case 31:
                    {

                        if (client.Player.TournamentKills >= 2500)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 160039, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.MP, false, "", 3);
                            client.Player.TournamentKills -= 2500;
                            client.CreateBoxDialog("You received a Super [Magic]DeerskinBoots.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Tournament Points.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();

                        }

                        break;
                    }
                case 24:
                    {

                        if (client.Player.TournamentKills >= 1500)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 120049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.Shield, false, "", 3);
                            client.Player.TournamentKills -= 1500;
                            client.CreateBoxDialog("You received a Super [Shield]ThreadNecklace.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Tournament Points.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();

                        }

                        break;
                    }
                case 23:
                    {
                        if (client.Player.TournamentKills >= 2000)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 150059, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.Stigma, false, "", 3);
                            client.Player.TournamentKills -= 2000;
                            client.CreateBoxDialog("You received a Super [Stigma]GoldRing.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Tournament Points.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();

                        }


                        break;
                    }
                case 21:
                    {
                        if (client.Player.TournamentKills >= 1000)
                        {
                            client.Player.TournamentKills -= 1000;
                            client.Player.ConquerPoints += 10000;
                            client.CreateBoxDialog("You received 10,000ConquerPoints.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Tournament Points.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();

                        }


                        break;
                    }
                case 22:
                    {
                        if (client.Player.TournamentKills >= 100)
                        {
                            client.Player.TournamentKills -= 100;
                            client.Player.ConquerPoints += 1000;
                            client.CreateBoxDialog("You received 1,000 ConquerPoints.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Tournament Points.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();

                        }


                        break;
                    }
                case 10:
                    {
                        data.AddText("You have " + (client.Player.OnlineMinutes / 60).ToString() + " hours online. Below you can find something rewards")
                          .AddOption("Experience(5 Hours)", 11)
.AddOption("1HourVIP6(120 Minutes)", 12)
.AddOption("Stars~Garment(100 Minutes)", 13)
.AddOption("Stars~Mount(100 Minutes)", 14)
.AddOption("1Hour~4xExperience(60 Minutes)", 15)
.AddOption("Main Menu.", 30)
.AddOption("Thank you.", 255)
.AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 15:
                    {
                        if (client.Player.OnlineMinutes > 60)
                        {
                            client.Player.OnlineMinutes -= 60;
                            client.Player.RateExp = 4;
                            client.Player.DExpTime = 3600;
                            client.Player.CreateExtraExpPacket(stream);
                            client.SendSysMesage("You get 4xExperience. time of one hour.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough OnlineHours.")
 .AddOption("Ah ok.", 255)
 .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 14:
                    {
                        if (client.Player.OnlineMinutes > 100)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                                break;
                            }
                            // Random pick among eligible mounts only (no unbounded retry loop); nothing is charged if none exist.
                            var mounts = Database.ItemType.SteedMounts.Values.Where(it =>
                            {
                                Database.CoatStorage.StorageItem DBMount;
                                return Database.CoatStorage.Mounts.TryGetValue(it.ID, out DBMount) && DBMount.Stars < 3;
                            }).ToArray();
                            if (mounts.Length == 0)
                            {
                                client.CreateBoxDialog("This reward is not available right now.");
                                break;
                            }
                            client.Player.OnlineMinutes -= 100;
                            var item = mounts[Program.GetRandom.Next(0, mounts.Length)];
                            client.Inventory.Add(stream, item.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough OnlineHours.")
 .AddOption("Ah ok.", 255)
 .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 13:
                    {
                        if (client.Player.OnlineMinutes > 100)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                                break;
                            }
                            // Random pick among eligible garments only (no unbounded retry loop); nothing is charged if none exist.
                            var garments = Database.ItemType.Garments.Values.Where(it =>
                            {
                                Database.CoatStorage.StorageItem DbGarment;
                                return Database.CoatStorage.Garments.TryGetValue(it.ID, out DbGarment) && DbGarment.Stars < 3;
                            }).ToArray();
                            if (garments.Length == 0)
                            {
                                client.CreateBoxDialog("This reward is not available right now.");
                                break;
                            }
                            client.Player.OnlineMinutes -= 100;
                            var aitem = garments[Program.GetRandom.Next(0, garments.Length)];
                            client.Inventory.Add(stream, aitem.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough OnlineHours.")
 .AddOption("Ah ok.", 255)
 .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        if (client.Player.VipLevel == 6)
                        {
                            client.CreateBoxDialog("You already have VIP 6 status.");
                            break;
                        }
                        if (client.Player.OnlineMinutes > 120)
                        {
                            client.Player.OnlineMinutes -= 120 ;
                            if (DateTime.Now > client.Player.ExpireVip)
                                client.Player.ExpireVip = DateTime.Now.AddMinutes(60);
                            else
                                client.Player.ExpireVip = client.Player.ExpireVip.AddMinutes(60);

                            client.Player.VipLevel = 6;

                            client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                            client.Player.UpdateVip(stream);

                            client.CreateBoxDialog("You`ve received vip6 (1 hour).");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough OnlineHours.")
 .AddOption("Ah ok.", 255)
 .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 11:
                    {
                        if (client.Player.OnlineMinutes > 50)
                        {
                            client.Player.OnlineMinutes -= 50 ;
                            client.GainExpBall(900, true, Role.Flags.ExperienceEffect.angelwing);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough OnlineHours.")
 .AddOption("Ah ok.", 255)
 .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
 //                      
                        client.Teleport(51, 68, 7023);
                        break;
                    }
                case 8:
                    {
                       
                        if (Database.VoteSystem.CanVote(client))
                        {
                            client.SendSysMesage(Program.ServerConfig.XtremeTopLink, MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.white);
                            client.Player.StartVote = true;
                            client.Player.StartVoteStamp = Extensions.Time32.Now.AddSeconds(30);
                            client.SendSysMesage("Please wait for system to check your vote");
                        }
                        else
                        {
                            data.AddText("You have already claimed your reward for voting. You may only vote once every 12 hour per IP address.")
 .AddOption("I~see.", 255)
 .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("Player who have VIP(level 6) will get the ConquerPoints added into inventory.")
                            .AddText("By staying 12 hours online you will get 1 Hour of Free VIP(Level 6)")
                   .AddOption("Main Menu.", 30)
                   .AddOption("Thank you.", 255)
                 .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Our server is are designed to be a mediu rate experience and drops. Below you can find further information on our current rates.")
                   .AddOption("Drops.", 2)
                   .AddOption("Experience", 3)
                   .AddOption("Main Menu.", 30)
                   .AddOption("Thank you.", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        data.AddText("The curent drop rate is: \n")
                           .AddText("DragonBall: 1/500 \n PlusOne: 1/100 \n PlusTwo: 1/200 \n CPs(50-100, Vip4(100-150), Vip6(150-200)): 1/10 \n Elite Items: 1/100 \n Super Items: 1/150")
                  .AddOption("Main Menu.", 30)
                  .AddOption("Thank you.", 255)
              .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("The curent Experience rate is: \n")
                           .AddText("LevelExp: " + Program.ServerConfig.UserExpRate + " \n Skill Exp: " + Program.ServerConfig.ExpRateSpell + " \n ProfExp " + Program.ServerConfig.ExpRateProf + "")
                  .AddOption("Main Menu.", 30)
                  .AddOption("Thank you.", 255)
              .AddAvatar(63).FinalizeDialog();


                        break;
                    }

            }
        }
      [NpcAttribute(NpcID.WhiteDragon)]
        public static void WhiteDragon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("When you burn a [RosewoodIncrense], you`ll feel refreshed and clear-minded. ")
                   .AddText("I`d like to give you Buddha`s gift for this incrense. The gift are ")
                           .AddText("Monk`s Skill Bock, Chi Points. If you have those items, you can also swap ")
                           .AddText("them for the incense with me. So, what you say?")
                    .AddOption("Swap the incense for gifts.", 1)
                            //.AddOption("Swap gifts for the incense.", 6)
                    .AddOption("I`ll think about it.", 255)
                    .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("Diffrent gift requires diffrent amount of RoseWood incense. Here's an exchange ")
                  .AddText("list: [Grace of Heaven Scroll] - 10 incenses; [Wrath of the Emperor Scroll ] - 1 ")
                          .AddText("incense; [Infernal Echo Scroll] - 20 incenses; [2000 Chi Points] - 10 incenses;  Wich one do you like? ")
                   .AddOption("Infernal Echo Scroll (20).", 2)
                   .AddOption("Grace of Heaven Scroll (10).", 3)
                   .AddOption("Wrath of the Emperor Scroll (1).", 4)
                   .AddOption("2000 Chi Points (10).", 5)
                   .AddOption("I`ll think about it.", 255)
                   .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3007565, 20))
                        {
                            if (!client.Inventory.Remove(3007565, 20, stream)) break;
                            client.Inventory.Add(stream, 3007568);
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 20 RosewoodIncense in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }



                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(3007565, 10))
                        {
                            if (!client.Inventory.Remove(3007565, 10, stream)) break;
                            client.Inventory.Add(stream, 3007567);
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 10 RosewoodIncense in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3007565, 1))
                        {
                            if (!client.Inventory.Remove(3007565, 1, stream)) break;
                            client.Inventory.Add(stream, 3007566);
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 1 RosewoodIncense in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3007565, 10))
                        {
                            if (!client.Inventory.Remove(3007565, 10, stream)) break;
                            client.Inventory.Add(stream, 3005128);
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 5 RosewoodIncense in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.VicotryBuddha)]
        public static void VicotryBuddha(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The mortal world is stil the best place for me . I received anlghtement here thousands years ago . ")
                   .AddText(" I hope you achieve the essence of Buddhism , too. With the [Buddha's Necklace]")
                           .AddText(", you'll have a chance to achieve that during meditation.")
                    .AddOption("Remove distractions (1 Necklace)", 1)
                    .AddOption("I wana to leave here", 3)
                    .AddOption("I prefer to keep them.", 255)
                    .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of Buddha`sNecklace?")
                            .AddOption("Yes.", 2)
                            .AddOption("No.", 255)
                            .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3007563, 1))
                        {
                            if (!client.Inventory.Remove(3007563, 1, stream)) break;
                            if (client.Player.Rate(30))
                            {
                                client.CreateBoxDialog("You received BoundlessHeart.");
                                client.Inventory.AddItemWitchStack(3007564, 0, 1, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("You received RosewoodIncense.");
                                client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 1 Buddha`sNecklace in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        client.Teleport(37, 36, 1004);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AltarCleanser)]
        public static void AltarCleanser(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Why did you keep me waiting so long ? Well i'm not wordy. To many Emotions and desires will ")
                   .AddText(" affect your study of Buddhism. You have 1/10 chance to remove a distracting")
                           .AddText(" thought with a [Spiritual Gem] each time .")
                           .AddText(" If you succeed , Ill make you a [Buddha`s Necklace] for you")
                    .AddOption("Remove distractions (1 gem)", 1)
                    .AddOption("Try 5 times. (5 gems)", 3)
                    .AddOption("I prefer to keep them.", 255)
                    .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of SpiritualGem?")
            .AddOption("Yes.", 2)
            .AddOption("No.", 255)
            .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3007562, 1))
                        {
                            if (!client.Inventory.Remove(3007562, 1, stream)) break;
                            if (client.Player.Rate(10))
                            {
                                client.CreateBoxDialog("You received Buddha`sNecklace.");
                                client.Inventory.AddItemWitchStack(3007563, 0, 1, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("You received RosewoodIncense.");
                                client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 1 SpiritualGem in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();


                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of SpiritualGem?")
                 .AddOption("Yes.", 4)
                 .AddOption("No.", 255)
                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3007562, 5))
                        {
                            if (!client.Inventory.Remove(3007562, 5, stream)) break;
                            if (client.Player.Rate(10))
                            {
                                client.CreateBoxDialog("You received Buddha`sNecklace.");
                                client.Inventory.AddItemWitchStack(3007563, 0, 1, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("You received RosewoodIncense.");
                                client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 5 SpiritualGem in your inventory.")
       .AddOption("Oh.", 255)
       .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.SuperPureOver)]
        public static void SuperPureOver(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The oven contain great cosmological principle of everything. If you're ready,")
                                .AddText(" put the Star Crystal into the oven, and you'll have a 30% change to receive a  Divine Panacea. \n\n")
                                .AddText("This mythical pill is said to contain endless power to reshape your backsword on the main hand into an Epic Weapon.")

                     .AddOption("Make a Divine Panacea.", 1)
                     .AddOption("I see.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(2))
                        {
                            if (client.Inventory.Contain(3006014, 1))
                            {
                                if (!client.Inventory.Remove(3006014, 1, stream)) break;
                                client.Player.EpicQuestThree_rdChance += 1;
                                if (Role.Core.Rate(3, 10))
                                {
                                    client.CreateBoxDialog("You've acquired the essence of Taoism, and received a Divine Panacea.");

                                    data.AddText("You've completely accquired the essence of Taoism, and received a Divine Panacea.")
                                    .AddText("With this mythycal pill, you can reshape the backsword on your main hand into an Epic Weapon.")
                                      .AddOption("I finally made it.", 255).FinalizeDialog();
                                    client.Inventory.Add(stream, 3006016, 1);
                                }
                                else
                                {
                                    client.Inventory.AddItemWitchStack(3006015, 0, 5, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");

                                    data.AddText("You've ran the oven " + client.Player.EpicQuestThree_rdChance + " time(s), but failed to make Divine Panacea. Don't ")
                                        .AddText("lose hear.You still got 5 pieces of Yin Yang Fish from the oven. Every time")
                                        .AddText(" when you run the oven, you`ll make a 10% change to make a Divine Panacea.")
                                        .AddText(" Do you want to try it again with a Star Crystal?")
                                        .AddOption("Make a Divine Panacea.", 1)
                                       .AddOption("I see.", 255).FinalizeDialog();

                                }
                            }
                            else
                            {
                                data.AddText("You~don`t~have~the~Star~Crystal~with~you.~You~need~to~make~one~with~the~Elite~Pure~Oven,~first.")
                                       .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 2 more spaces in your inventory")
                                                      .AddOption("Let me check.", 255)
                                                      .FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PureTaoist)]
        public static void PureTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Yin Yang Fish is a special token to balance the energy of Yin and Yand. If you happen to ")
                                .AddText(" collect some, you can excgange them for Taoist Epic-skill books, Elite Hossu for left hand, Chi Points.")
                     .AddOption("Swap the Fish for rewards.", 1)
                     .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Look, i have so many things for you to choose from. Each Item require different amount of ")
                               .AddText(" Yin Yang Fish. So, what would you like to exchange for?");

                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            data.AddOption("AuroraLotusScroll (20 Fish)", 2);
                            data.AddOption("BlessingTouchScroll (10 Fish)", 4);
                        }
                        else if (Database.AtributesStatus.IsFire(client.Player.Class))
                        {
                            data.AddOption("FlameLotusScroll (20 Fish)", 6);
                            data.AddOption("BreakingTouchScroll (10 Fish)", 8);
                        }
                        data.AddOption("2000 Chi Points (10 Fish)", 10)
                            .AddOption("DesireHossu (15 Fish)", 12)
                            .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("You are sure you want to swap 15 pieces of Yin Yang Fish for an DesireHossu?")
                    .AddOption("Yes.", 13)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 15))
                            {
                                if (!client.Inventory.Remove(3006015, 15, stream)) break;
                                client.Inventory.Add(stream, 619028);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an DesireHossu!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 15 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("You are sure you want to swap 10 pieces of Yin Yang Fish for 2000 Chi Points?")
                    .AddOption("Yes.", 11)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 10))
                            {
                                if (!client.Inventory.Remove(3006015, 10, stream)) break;
                                client.Inventory.Add(stream, 3005128);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for 2000 Chi Points!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 10 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("You are sure you want to swap 10 pieces of Yin Yang Fish for an Breaking Touch Scroll?")
                    .AddOption("Yes.", 9)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 10))
                            {
                                if (!client.Inventory.Remove(3006015, 10, stream)) break;
                                client.Inventory.Add(stream, 3006019);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an Breaking Touch Scroll!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 10 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        data.AddText("You are sure you want to swap 20 pieces of Yin Yang Fish for an Flame Lotus Scroll?")
                    .AddOption("Yes.", 7)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 20))
                            {
                                if (!client.Inventory.Remove(3006015, 20, stream)) break;
                                client.Inventory.Add(stream, 3006017);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an Flame Lotus Scroll!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 20 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        data.AddText("You are sure you want to swap 10 pieces of Yin Yang Fish for an BlessingTouchScroll?")
                    .AddOption("Yes.", 5)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 10))
                            {
                                if (!client.Inventory.Remove(3006015, 10, stream)) break;
                                client.Inventory.Add(stream, 3006020);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an BlessingTouchScroll!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 10 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("You are sure you want to swap 20 pieces of Yin Yang Fish for an Aurora Lotus Scroll?")
                    .AddOption("Yes.", 3)
                    .AddOption("I'll think about it.", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {

                            if (client.Inventory.Contain(3006015, 20))
                            {
                                if (!client.Inventory.Remove(3006015, 20, stream)) break;
                                client.Inventory.Add(stream, 3006018);
                                client.CreateBoxDialog("You successfully exchange the Yin Yang Fish for an Aurora Lotus Scroll!");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 20 Yin Yang Fish in your inventory.")
                                     .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                                        .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ElitePureOver)]
        public static void ElitePureOver(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This oven can balance the energy of Yin and Yang. When you put the ")
                                .AddText("Treasure Leaf, you'll have a 45% chance to get Star Crystal from the oven.")
                     .AddOption("Make a Star Crystal.", 2)
                     .AddOption("Make a Star Crystal(5 times)", 3)
                     .AddOption("I see.", 255).FinalizeDialog();


                        break;
                    }

                case 3:
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            if (client.Inventory.Contain(3006013, 5))
                            {
                                if (!client.Inventory.Remove(3006013, 5, stream)) break;
                                bool ReceivedStar = false;
                                if (client.Player.Rate(30))
                                {
                                    client.Player.EpicQuestTwo_ndChance += 1;
                                    ReceivedStar = true;
                                }


                                for (int x = 0; x < (int)(ReceivedStar == true ? 4 : 5); x++)
                                {
                                    client.Player.EpicQuestTwo_ndChance += 1;

                                    {
                                        data.AddText("You`re ran the oven " + client.Player.EpicQuestTwo_ndChance + " time(s). Received YinYangFish. \n");
                                        client.Inventory.AddItemWitchStack(3006015, 0, 1, stream);
                                    }

                                }


                                if (ReceivedStar)
                                {
                                    client.CreateBoxDialog("You Successfully made a Star Crystal Now, you're qualified to take furthe alchemy at the Super Pure Oven.");
                                }
                                else
                                {
                                    data.AddOption("Run it 5 times more.", 3)
                                   .AddOption("I see.", 255).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 5 TreasureLeaf in your inventory.")
                                       .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 4 more spaces in your inventory")
                                                       .AddOption("Let me check.", 255)
                                                      .FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3933, 48, 48, 25012);

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Contain(3006013, 1))
                            {
                                if (!client.Inventory.Remove(3006013, 1, stream)) break;
                                if (client.Player.Rate(10))
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                    data.AddText("You've got a deep understanding on the truth of Taoism and received a Star ")
                                      .AddText("Crystal. Now you can put the Star Crystal into the Super Pure Oven and burn it ")
                                      .AddText(" with the seven true fires to make a Divine Panacea. However it's not for 100% success rate.")
                                        .AddOption("Head to the Super Pure Oven.", 5)
                                        .AddOption("Keep making the Star Crystal.", 0)
                                        .FinalizeDialog();
                                    client.CreateBoxDialog("You Successfully made a Star Crystal Now, you're qualified to take furthe alchemy at the Super Pure Oven.");
                                    client.Inventory.Add(stream, 3006014);
                                }
                                else
                                {
                                    client.Inventory.AddItemWitchStack(3006015, 0, 1, stream);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                    data.AddText("You`ve ran the oven " + client.Player.EpicQuestTwo_ndChance + " time(s). but failed to make a Star Crystald. Don't lose heart. ")
                                .AddText("You still got Ying Yang Fish from the oven. Every time when you run the oven, you'll have")
                                .AddText(" a 10% chance to make a Star Crystal. Do you want to try it again with a Treasure Leaf?")
                           .AddOption("Make a Star Crystal.", 2)
                           .AddOption("Make a Star Crystal(5 times)", 3)
                           .AddOption("I see.", 255).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 1 TreasureLeaf in your inventory.")
                                       .AddOption("I see.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more spaces in your inventory")
                                                       .AddOption("Let me check.", 255)
                                                      .FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RefinedPureOven)]
        public static void RefinedPureOven(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.EpicQuestChance == 0)
                        {
                            data.AddText("This oven is empty. To make it work, you need to put in 3 Red Flowers and 3 White Flowers, everyday.")
                        .AddOption("Put in the Flowers.", 1)
                        .AddOption("Active it with 27 Cps.", 2)
                        .AddOption("Enter~in~the~Evil~Abyss.", 3)
                        .AddOption("I see.", 255).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The Red Flower and the White Flowers are composed in the oven. By collecting elements of the East,North, ")
                                .AddText("  West and South, this oven give a 10% change of making a Treasure Leaf.")
                     .AddOption("Run the process with 27 Cps.", 10)
                     .AddOption("Run 10 times with 270 Cps.", 12)
                     .AddOption("I see.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        data.AddText("You are sure you want to pay 270 Cps to ran the oven 10 times? ")
                      .AddOption("Yes.", 13)
                      .AddOption("I'm not ready, yet.", 255).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            if (client.Player.ConquerPoints >= 270)
                            {
                                client.Player.ConquerPoints -= 270;
                                for (int x = 0; x < 10; x++)
                                {
                                    uint[] array_Items = new uint[]
                           {
                               0,0, 360084, 360085, 0, 3006013,0 , 0, 0,0,360084,0,0, 0,360085,0,0,0,0,0,360084,360084,0,0,0,0,3006013 , 0, 0, 0, 0 ,360085,360085,360084,0,0,0,360085,360085
                           };
                                    int index = (int)Program.GetRandom.Next(0, array_Items.Length);
                                    uint ItemId = array_Items[index];
                                    if (ItemId != 0)
                                    {
                                        var item = Database.Server.ItemsBase[ItemId];
                                        client.Inventory.AddItemWitchStack((uint)ItemId, 0, 1, stream);
                                        data.AddText("You`re ran the oven " + client.Player.EpicQuestChance + " time(s). Received " + item.Name + ". \n");
                                    }
                                    else
                                        data.AddText("You`re ran the oven " + client.Player.EpicQuestChance + " time(s). Received nothing. \n");
                                    client.Player.EpicQuestChance += 1;
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                }
                                data.AddOption("Head to The Unique Pure Oven.", 15).AddOption("Run 10 times more (270 CPs)", 12).AddOption("I see.", 255).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 270 ConquerPoints")
     .AddOption("I see", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 4 more spaces in your inventory")
                                                       .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3933, 64, 64, 25011);
                        break;
                    }
                case 10:
                    {
                        data.AddText("You are sure you want to pay 27 Cps to activate this oven? ")
                        .AddOption("Yes.", 11)
                        .AddOption("I'm not ready, yet.", 255).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 27)
                            {
                                client.Player.ConquerPoints -= 27;
                                uint[] array_Items = new uint[]
                           {
                               0,0, 360084, 360085, 0, 3006013,0 , 0, 0,0,360084,0,0, 3006013,360085,0,360084,360084,0 , 0, 0, 0, 0 ,360085,360085,360084,0,0,0,360085,360085
                           };
                                int index = (int)Program.GetRandom.Next(0, array_Items.Length);
                                uint ItemId = array_Items[index];
                                if (ItemId != 0)
                                {
                                    var item = Database.Server.ItemsBase[ItemId];
                                    client.Inventory.AddItemWitchStack((uint)ItemId, 0, 1, stream);
                                    client.CreateBoxDialog("You received " + item.Name + ".");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                }
                                else
                                    client.CreateBoxDialog("You received nothing.");
                            }
                            else
                            {
                                data.AddText("Please come back when you will have 27 ConquerPoints")
     .AddOption("Oh.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more spaces in your inventory")
                                                       .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        client.Teleport(612, 634, 1700, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(711506, 3) && client.Inventory.Contain(711508, 3))
                        {
                            if (!client.Inventory.Remove(711506, 3, stream)) break;
                            if (!client.Inventory.Remove(711508, 3, stream)) break;
                            client.Player.EpicQuestChance = 1;
                            goto case 0;

                        }
                        else
                        {
                            data.AddText("Please come back when you will have 3 Red Flowers and 3 White Flowers in your inventory.")
         .AddOption("Oh.", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.ConquerPoints -= 27;
                            client.Player.EpicQuestChance = 1;
                            goto case 0;
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 27 ConquerPoints")
       .AddOption("Oh.", 255).FinalizeDialog();
                        }


                        break;
                    }
            }
        }



        [NpcAttribute(NpcID.SunKing)]
        public static void SunKing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Once you acquire the essence of Taoism, you'll be able to make a Divine")
                   .AddText(" Panacea, and win a peerless Epic Weapon of Taoist. All heroes who have")
                           .AddText(" reached Level 70 of 2nd rebirn are welcome to take the trial with the three Pure Ovens.")
                    .AddOption("Show me the Way", 1)
                    .AddOption("I wana to leave.", 2)
                    .AddOption("I'm not ready, yet.", 255)
                    .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("Stage 1: Activate the Refined Pure Oven (Taoist Temple 80,80) with 3 Red Flowers and 3 White Flowers")
                            .AddText(" to make a Treasure Leaf. You can probably obtain such flowers in the Evil Abyss, where you can do the 2nd Rebirth Quest. \n")
                              .AddText("Stage 2: When you obtain a Treasure Leaf, put it into the Elite Pure Oven (Taoist Temple 64,64), and you'll have a 10% chance to obtain a Star Crystal. \n")
                          .AddText("Stage 3: When you successfully get a Star Crystal from the Elite Pure Oven, use it to make the mystical Divine Panacea in the Super Pure Oven (Taoist Temple 48,48) at a 10% success rate. \n\n")
                          .AddText("Once you obtained a Divine Panacea, use it to reshape your right-handed Backsword into an Epic Weapon of Taoist. \n")
                          .AddText("If you failed to obtain the key items in the Elite Pure Oven and Super Pure Oven, instead you'll receive Yin Yang Fish. With Yin Yang Fish, you can exchange ")
                          .AddText(" with the Pure Taoist (Taoist Temple 76,83) for fabulous rewards, such as an Elite Bliss Hossu, which can")
                          .AddText(" be equipped as a secondary weapon at the left hand. The eye-catching Hossu is exclusive for Taoists which perfectly reveals the purity and grace of Taoists.")
                              .AddOption("I see.", 255)
                            .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        client.Teleport(53, 36, 1004);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FortuneArhat)]
        public static void FortuneArhat(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hell is so close to Heavem, if you commit a sin, you could easily on your way to Hell, but ")
                   .AddText("you can always be saved. Give me 3 Red Flowers and 3 White Flowers, and get ready for a trial")
                           .AddText("to clear your sin.")
                    .AddOption("Here`re the flowers.", 1)
                    .AddOption("Pay 197 CPs to start the trial.", 2)
                            //.AddOption("Pay 1970 CPs to start the trial(10 Times).", 4)
                    .AddOption("Human beling are complicated", 255)
                    .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of 1970 ConquerPoints?")
                   .AddOption("Yes.", 5)
                   .AddOption("No.", 255)
                   .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 1970)
                        {
                            for (int x = 0; x < 10; x++)
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    client.Player.ConquerPoints -= 197;
                                    if (Role.Core.Rate(20))
                                    {
                                        client.CreateBoxDialog("You received SpiritualGem.");
                                        client.Inventory.AddItemWitchStack(3007562, 0, 1, stream);
                                    }
                                    else
                                    {
                                        client.CreateBoxDialog("You received RosewoodIncense.");
                                        client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                                    }
                                }
                                else
                                {
                                    data.AddText("Please make 1 more spaces in your inventory")
                                                             .AddOption("Let me check.", 255)
                                                             .AddAvatar(226).FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You do not have 1970 ConquerPoints with you.")
              .AddOption("Oh.", 255)
              .AddAvatar(226).FinalizeDialog();
                        }



                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to start this trial with the power of 197 ConquerPoints?")
                   .AddOption("Yes.", 3)
                   .AddOption("No.", 255)
                   .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 197)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.ConquerPoints -= 197;
                                if (Role.Core.Rate(20))
                                {
                                    client.CreateBoxDialog("You received SpiritualGem.");
                                    client.Inventory.AddItemWitchStack(3007562, 0, 1, stream);
                                }
                                else
                                {
                                    client.CreateBoxDialog("You received RosewoodIncense.");
                                    client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                                }
                            }
                            else
                            {
                                data.AddText("Please make 1 more spaces in your inventory")
                                                         .AddOption("Let me check.", 255)
                                                         .AddAvatar(226).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You do not have 197 ConquerPoints with you.")
              .AddOption("Oh.", 255)
              .AddAvatar(226).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(722732, 3) && client.Inventory.Contain(722736, 3))
                        {
                            if (!client.Inventory.Remove(722736, 3, stream)) break;
                            if (!client.Inventory.Remove(722732, 3, stream)) break;
                            if (Role.Core.Rate(30))
                            {
                                client.CreateBoxDialog("You received SpiritualGem.");
                                client.Inventory.AddItemWitchStack(3007562, 0, 1, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("You received RosewoodIncense.");
                                client.Inventory.AddItemWitchStack(3007565, 0, 1, stream);
                            }
                        }
                        else
                        {
                            data.AddText("Please come back when you will have 3 Red Flowers and 3 White Flowers in your inventory.")
         .AddOption("Oh.", 255)
         .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GoldenCicada)]
        public static void GoldenCicada(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Devils are rampant again and people are living in misery. That is the reason why i came ")
                  .AddText("here with my disciples. We need Monks and Saints who have reached Level 70 of 2nd-reborn ")
                          .AddText("to repeat the journey to the Westrn Fairyland. Perhaps, the power of sacred sutras in the")
                          .AddText(" Fairland can alleviate the miseries.")
                   .AddOption("What can i do? ", 1)
                   .AddOption("I wana to leave here.", 2)
                   .AddOption("Enter~in~the~Evil~Abyss.", 5)
                   .AddOption("Nothing is real.", 255)
                   .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Trial 1: Submit 3 Red Flowers and 3 White Flowers to Fortune Arhat, and complete this trial to get a [Spiritual Gem]. \n")
                   .AddText("Trial 2: Submit [Spiritual Gem] to the Altar Cleanser, and remove distracting thoughts to get [Buddha’s Necklace]. \n")
                           .AddText("Trial 3: Submit [Buddha’s Necklace] to the Victory Buddha, and complete this trial to get a [Boundless Heart]. \n\n")
                           .AddText("Use The[Boundless Heart] to reshape your Prayer Beads into an Epic Weapon. \n")
                           .AddText("Equipping 2 Epic Weapons on your hands will inflict ranged damage while attacking.")
                    .AddOption("Head To see Fortune Arhat. ", 3)
                    .AddOption("I see.", 255)
                    .AddAvatar(226).FinalizeDialog();



                        break;
                    }
                case 5:
                    {
                        client.Teleport(612, 634, 1700, 0);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(79, 82, 3983);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(37, 36, 1004);
                        break;
                    }

            }
        }
    }
}
