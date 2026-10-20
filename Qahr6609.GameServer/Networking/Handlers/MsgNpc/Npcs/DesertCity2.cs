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
        [NpcAttribute(NpcID.SpringSoldierZhao)]
        public static void SpringSoldierZhao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringSoldierZhao, client.Player.Class, 1504);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringSoldierZhao, client.Player.Class, 1505);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1506);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringSoldierZhao, client.Player.Class, 1507);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Let`s wait and see. The backup will arrive, soon. We are going to remove all the monsters!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~1.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~3.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~4.", 30);
                        data.AddOption("Bye.", 255);
                        data.AddAvatar(92).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                data.AddText("Well done! Please accept these rewards.")
             .AddOption("Thanks.", 32).AddAvatar(92)
             .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Why are you still here? I need you to put the Antidote into the source of the Moon Spring, right away!")
                .AddOption("I`m~on~my~way.", 255).AddAvatar(92)
                .FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("I need you to put the Antidote into the source of the Moon Spring, right away! Beware of the Level 83 Blade Ghosts, on the way.")
                    .AddOption("I~will.", 31)
                  .AddOption("I`m~busy.", 255).AddAvatar(92)
                  .FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(92)
                           .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Add(stream, 730002, 1, 2);
                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 0, stream);
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've~received~some~EXP~3,000~Silver,~and~EXP~Postion~and~a~+2~Stone~(B)!", MsgMessage.ChatMode.System);
                            client.CreateBoxDialog("You~received~a~+2~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.Contain(720968, 1) == false)
                            client.Inventory.Add(stream, 720968);

                        client.CreateBoxDialog("You`ve~received~an~Antidote.~Hurry~to~put~it~into~the~Moon~Spring.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(720968, 1))
                        {
                            data.AddText("Thanks for the antidote! Here`s your reward!")
                          .AddOption("Okay.", 21).AddAvatar(92)
                          .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come back when you will have Antidote.")
                        .AddOption("Okay.", 255).AddAvatar(92)
                        .FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                        }
                        data.AddText("I need you to put the Antidote into the source of the Moon Spring, right away! Beware of the Level 83 Blade Ghosts, on the way.")
                            .AddOption("I~will.", 31)
                          .AddOption("I`m~busy.", 255).AddAvatar(92)
                          .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Does General ZhuGe have a solution?")
                          .AddOption("I`m~on~my~way.", 255).AddAvatar(92)
                          .FinalizeDialog();
                            break;
                        }
                        data.AddText("The Moon Spring was poisoned! I need you to pay a visit to General ZhuGe and ask him how to solve this problem!")
                            .AddOption("I`ll~be~back~soon.", 11)
                            .AddOption("Later.", 255).AddAvatar(92)
                            .FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~head~to~see~General~ZhuGe.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            data.AddText("So the Moon Spring is poisoned! Thank you very much for your help.")
                                .AddOption("My~pleasure.", 3).AddAvatar(92)
                                .FinalizeDialog();

                            break;//finish
                        }
                        if (client.Inventory.Contain(720967, 1))
                        {
                            data.AddText("Did you take the Moon Spring I gave to you?")
                .AddOption("Nope.", 255).AddAvatar(92)
                .FinalizeDialog();
                            break;
                        }
                        data.AddText("Recently, many villagers were poisoned by drinking the water from the Moon Spring. Could you test if the water is toxic?")
                .AddOption("Yes,~I`d~love~to.", 2)
                .AddOption("Of~course~not.", 255).AddAvatar(92)
                .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            //  client.Inventory.Add(stream, 120159, 1,0,0,0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket,true);
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball~and~2,000~Silver!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(92)
                           .FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720967);
                        client.CreateBoxDialog("You`ve~received~the~Moon~Spring.~Hurry~to~test~if~it~is~poisonous.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TaoistSpring)]
        public static void TaoistSpring(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistSpring, client.Player.Class, 1501);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistSpring, client.Player.Class, 1502);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistSpring, client.Player.Class, 1503);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("People believe the Moon Spring is blessed by the god of water. We should be grateful for every drop of water he provides!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Worship~the~Water~God~1.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Worship~the~Water~God~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Worship~the~Water~God~3.", 20);
                        data.AddOption("Leave.", 255);
                        data.AddAvatar(43).FinalizeDialog();
                        break;



                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                        {
                            data.AddText("Thank you very much. The god will bless the Moon Spring and the whole village!")
            .AddOption("Great!", 22).AddAvatar(43)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(720966, 1))
                        {
                            data.AddText("What`s up? You`re going to find the grids where 1 and 6 are located, and then sacrifice the offering to god.")
                  .AddOption("I~see.", 255).AddAvatar(43)
                  .FinalizeDialog();
                            break;
                        }

                        data.AddText("Do you still remember where the number 1 and 6 are? Take Blade Ghost`s Ear and worship the god of water there.")
              .AddOption("Okay.", 21)
              .AddOption("Later.", 255).AddAvatar(43)
              .FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(43)
                           .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Add(stream, 120159, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver,~a~Super~Necklace!");
                        }
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(43)
                           .FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720966);
                        client.CreateBoxDialog("You`ve~received~an~offering.~Hurry~go~to~the~worship~ceremony.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711410, 1) && client.Inventory.Contain(711411, 1)
                                && client.Inventory.Contain(711412, 1) && client.Inventory.Contain(711413, 1))
                            {
                                data.AddText("Let me see. Sand Pill, Hill Monster`s Heart, Rock Monster`s Tail and Blade Ghost`s Ear. Yup! They`re what I`m looking for.")
               .AddOption("Good.", 4).AddAvatar(43)
               .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need Sand Pill, Hill Monster`s Heart, Rock Monster`s Tail and Blade Ghost`s Ear. Have you collected all of them?")
               .AddOption("Not~yet.", 255).AddAvatar(43)
               .FinalizeDialog();
                            }

                            break;
                        }

                        data.AddText("We`re going to hold a worship ceremony. I need you to collect some offerings for me.")
                  .AddOption("Go~ahead.", 2)
                  .AddOption("Sorry,~I~have~to~leave.", 255).AddAvatar(43)
                  .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Water: 1 (north) and 6 (northwest). Fire: 7 (west) and 2 (southwest). Gold: 9 (south) and 4 (southeast). Wood: 3 (east) and 8 (northeast). Earth: 5 (center).")
          .AddOption("I`m~ready.", 13).AddAvatar(43)
          .FinalizeDialog();
                            break;
                        }
                        data.AddText("There`re 9 squares marked on the platform. You must learn the rules before you can make a sacrifice.")
           .AddOption("What`re~the~rules?", 11)
           .AddOption("Later.", 255).AddAvatar(43)
           .FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("Where is number 5?")
                          .AddOption("A.~In~the~center.", 14)
                          .AddOption("B.~In~the~southeast.", 255).AddAvatar(43)
                          .FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.AddText("Bingo! You gave the correct answer! Good job!")
                            .AddOption("Great.", 15).AddAvatar(43)
                            .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(43)
                           .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                        }
                        break;
                    }
                case 11:
                    {
                        data.AddText("Listen. 2, 9 and 4 (from left to right) are in the first line. 3, 5 and 7 are in the middle line. 8, 1 and 6 are in the bottom line.")
                            .AddOption("I~got~it.", 12).AddAvatar(43)
                            .FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Talk~to~Taoist~Spring~to~answer~his~questions.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 4:
                    {

                        if (HasItems(client, 711410, 1) && HasItems(client, 711411, 1) && HasItems(client, 711412, 1) && HasItems(client, 711413, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711410, 1, stream)) break;
                            if (!client.Inventory.Remove(711411, 1, stream)) break;
                            if (!client.Inventory.Remove(711412, 1, stream)) break;
                            if (!client.Inventory.Remove(711413, 1, stream)) break;
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                        }
                        data.AddText("There`re 9 squares marked on the platform. You must learn the rules before you can make a sacrifice.")
              .AddOption("What`re~the~rules?", 11)
              .AddOption("Later.", 255).AddAvatar(43)
              .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Go kill the monsters and gather a Sand Pill, Hill Monster`s Heart, Rock Monster`s Tail and Blade Ghost`s Ear.")
                 .AddOption("I~see.", 3).AddAvatar(43)
                 .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Hurry~to~help~Taoist~Spring~to~collect~the~offerings.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ElderJiang)]
        public static void ElderJiang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1508);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1509);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringGeneralXu, client.Player.Class, 1510);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1511);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Stop asking me about the Straight Hook. You will get it, someday.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Elder~Jiang~3.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Elder~Jiang~1.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Elder~Jiang~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Elder~Jiang~4.", 30);
                        data.AddOption("I`ll~leave~you~alone.", 255);
                        data.AddAvatar(63).FinalizeDialog();
                        break;



                    }
                case 30:
                    {
                        data.AddText("I`m waiting for someone who is in a higher position.")
                   .AddOption("Roger.", 31)
                   .AddOption("Bye.", 255).AddAvatar(63)
                   .FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("Spring General Xu? I`m waiting for someone who is in a higher position.")
                  .AddOption("Oh.", 21).AddAvatar(63)
                  .FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/6~EXP~Ball~and~1,000~Silver!");
                        }
                        data.AddText("I`m waiting for someone who is in a higher position.")
                        .AddOption("Roger.", 31)
                        .AddOption("Bye.", 255).AddAvatar(63)
                        .FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("You~should~report~it~to~the~DC~Vice~Captain.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Why a straight hook? Well, I`m not interested in the fish. I`m waiting for the nobles.")
                     .AddOption("Visit~Spring~General~Xu.", 255).AddAvatar(63)
                     .FinalizeDialog();
                            break;
                        }
                        data.AddText("You want to know why a straight hook? Well, I`m not interested in the fish. I`m waiting for the nobles.")
                       .AddOption("Aha.", 11)
                       .AddOption("See~ya.", 255).AddAvatar(63)
                       .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~report~it~to~Spring~General~Xu.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711414, 1))
                            {
                                data.AddText("Great! That`s the Straight Hook! Why a straight hook? Haha, you`ll know that soon enough!")
                     .AddOption("Can`t~wait.", 3).AddAvatar(63)
                     .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you got a Straight Hook? Go kill the Blade Ghosts to grab one!")
                         .AddOption("Got~it.", 255).AddAvatar(63)
                         .FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Look at the pool. It`s full of fish! Can you go get a Straight Hook from the Blade Ghost?")
                            .AddOption("Yes,~I~can.", 2)
                            .AddOption("Sorry,~I`m~busy.", 255).AddAvatar(63)
                            .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (HasItems(client, 711414, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711414, 1, stream)) break;
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball~and~2,000~Silver!");
                        }
                        data.AddText("You want to know why a straight hook? Well, I`m not interested in the fish. I`m waiting for the nobles.")
                        .AddOption("Aha.", 11)
                        .AddOption("See~ya.", 255).AddAvatar(63)
                        .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Blade~Ghosts~to~get~a~Straight~Hook.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ConvoyViceLeader)]
        public static void ConvoyViceLeader(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1485);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1486);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1487);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1488);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1489);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1490);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Leave me alone. I`m busy.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Prove~Myself.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~1.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~2.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~3.", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~4.", 40);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~5.", 50);

                        data.AddOption("Alright.", 255);
                        data.AddAvatar(95).FinalizeDialog();
                        break;

                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("We have planted cactus on the way to the Moon Spring. It`ll prevent the monsters from getting close. Please inform Vice General Dong of the good news.")
.AddOption("Alright.", 255).AddAvatar(95)
.FinalizeDialog();
                            break;
                        }
                        data.AddText("We have planted cactus on the way to the Moon Spring. It`ll prevent the monsters from getting close. Please inform Vice General Dong of the good news.")
.AddOption("OK.", 51)
.AddOption("Later.", 255).AddAvatar(95)
.FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Hurry~to~head~to~see~Vice~General~Dong.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);

                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1, 1, 1))
                        {
                            data.AddText("You`d been a great help! Here`s your reward!")
.AddOption("Thanks.", 42)
.AddOption("I`m~busy.", 255).AddAvatar(95)
.FinalizeDialog();
                            break;
                        }
                        data.AddText("I hope you can head to the Moon Spring and plant 3 cactus seeds there.")
.AddOption("You~can~count~on~me.", 41)
.AddOption("I`m~busy.", 255).AddAvatar(95)
.FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(95)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                        }
                        data.AddText("We have planted cactus on the way to the Moon Spring. It`ll prevent the monsters from getting close. Please inform Vice General Dong of the good news.")
.AddOption("OK.", 51)
.AddOption("Later.", 255).AddAvatar(95)
.FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.Contain(720965, 1))
                            client.Inventory.Add(stream, 720965);
                        client.CreateBoxDialog("You`ve~received~some~cactus~seeds.~Hurry~to~go~plant~them.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720965, 1))
                            {
                                data.AddText("Good job! So we must hurry to plant them.")
 .AddOption("I~agree.", 32).AddAvatar(95)
 .FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("I have an idea. If we plant this cactus outside the village, the monsters won`t dare get close! I hear Wan Ying has the cactus seeds. Would you like to borrow some?")
  .AddOption("Sure.", 255).AddAvatar(95)
  .FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I have an idea. If we plant this cactus outside the village, the monsters won`t dare get close! I hear Wan Ying has the cactus seeds.")
   .AddOption("I`ll~go~borrow~some.", 31)
   .AddOption("None~of~my~business.", 255).AddAvatar(95)
   .FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {


                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~2,000~Silver!");
                        }
                        data.AddText("I hope you can head to the Moon Spring and plant 3 cactus seeds there.")
.AddOption("You~can~count~on~me.", 41)
.AddOption("I`m~busy.", 255).AddAvatar(95)
.FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Hurry~to~ask~Wan~Ying~for~some~cactus~seeds.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(720963, 1))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                            {
                                data.AddText("They are afraid of cactus? Great! Now, the real question is why....")
        .AddOption("Yup.", 22).AddAvatar(95)
        .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Go test if the Level 78 Rock Monsters are afraid of the cactus.")
            .AddOption("Okay!", 255).AddAvatar(95)
            .FinalizeDialog();
                                client.CreateBoxDialog("Use~the~Cactus~on~the~Level~78~Rock~Monsters.");
                            }
                            break;
                        }
                        data.AddText("I need you to do me a favor. Wave the cactus in front of the Level 78 Rock Monsters and watch their reaction.")
                         .AddOption("Interesting.", 21)
                        .AddOption("I`m~busy.", 255)
                        .AddAvatar(95)
                        .FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (HasItems(client, 711403, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(711403, 1, stream)) break;
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~2,000~Silver!");
                        }
                        data.AddText("I have an idea. If we plant this cactus outside the village, the monsters won`t dare get close! I hear Wan Ying has the cactus seeds.")
      .AddOption("I`ll~go~borrow~some.", 31)
      .AddOption("None~of~my~business.", 255).AddAvatar(95)
      .FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(95)
            .FinalizeDialog();
                            return;
                        }
                        client.CreateBoxDialog("You`ve~received~a~cactus.~Go~test~if~the~Level~78~Rock~Monsters~are~afraid~of~it.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Inventory.Add(stream, 720963);
                        break;
                    }

                case 10:
                    {
                        if (client.Inventory.Contain(720961, 1))
                        {
                            if (client.Inventory.Contain(711403, 1))
                            {
                                data.AddText("Well done! I`ll work out the secret!")
           .AddOption("Good~luck.", 14).AddAvatar(95)
           .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Level 78 Rock Monsters never get close to the cactus. That`s interesting. Would you like to fetch me a cactus?")
            .AddOption("Sure.", 255).AddAvatar(95)
            .FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Listen, I noticed an interesting thing. The Level 78 Rock Monsters never get close to the cactus. Would you like to fetch me one of those cactus?")
                        .AddText("~I`d like to give it a good once over and see if we can use this to our advantage...")
                            .AddOption("Of~course.", 11)
                           .AddOption("Nope.", 255)
                           .AddAvatar(95)
                           .FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        if (HasItems(client, 720961, 1) && HasItems(client, 711403, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(720961, 1, stream)) break;
                            if (!client.Inventory.Remove(711403, 1, stream)) break;
                            client.IncreaseExperience(stream, 2952000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~2,952,000~EXP~and~2,000~Silver!");
                        }

                        data.AddText("I need you to do me a favor. Wave the cactus in front of the Level 78 Rock Monsters and watch their reaction.")
                        .AddOption("Interesting.", 21)
                       .AddOption("I`m~busy.", 255)
                       .AddAvatar(95)
                       .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("Here`s a Sickle. You need to wait 10 seconds each time you use it, or else it could break.")
                            .AddOption("Roger.", 12)
                           .AddAvatar(95)
                           .FinalizeDialog();
                        break;
                    }

                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(95)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720961);
                        client.CreateBoxDialog("You`ve~received~a~Sickle.~Go~fetch~a~Cactus!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711402, 1))
                            {
                                data.AddText("Good job! You`re welcome to join us and help protect the village!")
                    .AddOption("Great.", 4)
                    .AddAvatar(95)
                    .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Are you scared? Go get a Monster Tail, then.")
                      .AddOption("I`m~on~my~way.", 255)
                      .AddAvatar(95)
                      .FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("You want to join us? Well, you`ll have to prove yourself, first.")
                            .AddOption("How?", 2)
                            .AddOption("Forget~it.", 255)
                            .AddAvatar(95)
                            .FinalizeDialog();

                        break;
                    }
                case 4:
                    {

                        if (!client.Inventory.HaveSpace(3))
                        {

                            data.AddText("Please make 3 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(95)
            .FinalizeDialog();
                            return;
                        }
                        if (HasItems(client, 711402, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711402, 1, stream)) break;
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.Inventory.Add(stream, 720173);

                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve~received~some~EXP~3,000~Silver,~a~Small~Joy~Stone~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Tell you what. Why don`t you go fetch a Monster Tail from the Level 78 Rock Monsters, then.")
                        .AddOption("I`ll~come~back~soon.", 3)
                        .AddAvatar(95)
                        .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Go~fetch~a~Monster~Tail~from~the~Level~78~Rock~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SpringViceGeneralOu)]
        public static void SpringViceGeneralOu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringViceGeneralOu, client.Player.Class, 1494);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringViceGeneralOu, client.Player.Class, 1500);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringViceGeneralOu, client.Player.Class, 6128);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I enjoy studying about smithing and forging powerful weapons for our soldiers!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Sharp~Blade.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Avenger.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Deliver~the~Water~(Daily).", 20);

                        data.AddOption("Bye.", 255);
                        data.AddAvatar(107).FinalizeDialog();
                        break;

                    }

                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.QuestGUI.SendAutoPatcher(1000, 490, 326, (ushort)NpcID.SoldierMu);
                            data.AddText("Hurry to deliver the water to the villagers!")
                      .AddOption("I`ll~go~right~away.", 255)
                      .AddAvatar(107).FinalizeDialog();
                            break;
                        }
                        data.AddText("These days, villagers are too scared because of the Blade Ghosts. I need some volunteers to disguise as Blade Ghosts and go fetch water for the villagers.")
                        .AddOption("Count~me~in.", 21)
                        .AddOption("Not~interested.", 255)
                        .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(107)
            .FinalizeDialog();
                            return;
                        }
                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                        client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, 240, 150, 1360);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("You`ve~got~the~water.~Hurry~to~deliver~it~to~Soldier~Mu.");
                        client.Inventory.Add(stream, 711417);
                        client.Player.QuestGUI.SendAutoPatcher(1000, 490, 326, (ushort)NpcID.SoldierMu);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711407, 5))
                            {
                                data.AddText("Thank you! You deserve the reward.")
            .AddOption("My~pleasure.", 13)
            .AddAvatar(107).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need you to kill the Level 83 Blade Ghosts and cut off their ears. Bring me back 5 ears, and I`ll have more work for you.")
                 .AddOption("Okay.", 255)
                 .AddAvatar(107).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I was injured by the Level 83 Blade Ghosts! I need you to give them a lesson!")
                         .AddOption("Go~ahead.", 11)
                         .AddOption("Sorry~to~hear~that.", 255)
                         .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(107)
            .FinalizeDialog();
                            return;
                        }
                        if (HasItems(client, 711407, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(711407, 5, stream)) break;
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.AddItemWitchStack(720172, 0, 3, stream);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~3~EXPBallScrap!.");
                        }
                        break;
                    }
                case 11:
                    {
                        data.AddText("I need you kill them and cut their ears. What about bringing me 5 ears?")
                        .AddOption("No~problem.", 12)
                        .AddAvatar(107).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Hurry~to~collect~5~ears~from~the~Level~89~Blade~Ghosts.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }

                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711406, 5))
                            {
                                data.AddText("That`s perfect. Thank you! Please, take your reward!")
            .AddOption("Thanks.", 4)
            .AddAvatar(107).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need you to gather 5 Sharp Blades from the Blade Ghosts.")
                 .AddOption("I~won`t~disappoint~you.", 255)
                 .AddAvatar(95).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I`m quite interested in the Blade Ghost`s weapon, the Sharp Blade.")
                         .AddOption("I`ll~go~get~some~for~you.", 2)
                         .AddOption("Leave.", 255)
                         .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(107)
            .FinalizeDialog();
                            return;
                        }
                        if (HasItems(client, 711406, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711406, 5, stream)) break;
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Meteor!");


                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("The Blade Ghosts out there are carrying the Sharp Blades. I need 5, if you think you can get them.")
                        .AddOption("I`ll~be~back~soon.", 3)
                        .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Go~kill~the~Blade~Ghosts~to~collect~5~Sharp~Blades.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.SculptorHe)]
        public static void SculptorHe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest(8447u/*SculptorHe in Questinfo.ini (spawned as 84477)*/, client.Player.Class, 1496);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest(8447u/*SculptorHe in Questinfo.ini (spawned as 84477)*/, client.Player.Class, 1497);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest(8447u/*SculptorHe in Questinfo.ini (spawned as 84477)*/, client.Player.Class, 1498);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, I`m Sculptor He. Here, I have many of my different sculptures and busts. Any interest in purchasing, perhaps?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Inscription~1.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Inscription~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Inscription~3.", 20);
                        data.AddOption("Good.", 255);
                        data.AddAvatar(63).FinalizeDialog();
                        break;

                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 3))
                            {
                                data.AddText("I really appreciate your help. Here`s your reward!")
                         .AddOption("Thanks.", 22)
                         .AddAvatar(63).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The noise I make when I hit the stone has enraged the Blade Ghosts. I hope you can protect me while I`m working, and hold back all the Angry Blade Ghosts.")
                                    .AddOption("Glad~to~help.", 21)
                                    .AddOption("I`ll~think~about~it.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                            }
                            break;
                        }

                        data.AddText("The noise I make when I hit the stone has enraged the Blade Ghosts. I hope you can protect me while I`m working, and hold back all the Angry Blade Ghosts.")
                           .AddOption("Glad~to~help.", 21)
                           .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(63)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.Add(stream, 150159, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~a~Super~Ring!");


                        }
                        break;
                    }
                case 21:
                    {
                        bool accept = true;
                        foreach (var mob in client.Player.View.Roles(Role.MapObjectType.Monster))
                        {
                            if ((mob as Game.MsgMonster.MonsterRole).Family.ID == 55434417)
                            {
                                accept = false;
                                break;
                            }
                        }
                        if (accept)
                        {
                            Database.Server.AddMapMonster(stream, client.Map, 55434417, client.Player.X, client.Player.Y, 7, 7, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                            Database.Server.AddMapMonster(stream, client.Map, 55434417, client.Player.X, client.Player.Y, 7, 7, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                            Database.Server.AddMapMonster(stream, client.Map, 55434417, client.Player.X, client.Player.Y, 7, 7, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                            client.CreateBoxDialog("The~noise~brought~3~Angry~Blade~Ghosts.~Kill~them,~all!");
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711409, 1))
                            {
                                data.AddText("Great! I`m working on the inscription, right now.")
                         .AddOption("Take~your~time.", 12)
                         .AddAvatar(63).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What are you waiting for? I need you to get a Sharp Axe from the Blade Ghosts!")
                           .AddOption("OK.", 255)
                           .AddAvatar(63).FinalizeDialog();
                            }
                            break;
                        }

                        data.AddText("Oh, my! The chisel is broken! Listen, you must help me. I need you to get a Sharp Axe from the Blade Ghosts!")
                           .AddOption("I~won`t~disappoint~you.", 11)
                           .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (HasItems(client, 711409, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(711409, 1, stream)) break;
                            client.IncreaseExperience(stream, 711348);

                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball~and~3,000~Silver!");


                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Blade~Ghosts~to~gain~a~Sharp~Axe.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711408, 1))
                            {
                                data.AddText("Yup, that`s the Inscription. Thank you very much.")
                         .AddOption("You`re~welcome.", 3)
                         .AddAvatar(63).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What happened? I need you to go fetch the Inscription from the DC Vice Captain.")
                           .AddOption("I`ll~go~right~away.", 255)
                           .AddAvatar(63).FinalizeDialog();
                            }
                            break;
                        }

                        data.AddText("I need you to do me a favor. Please go visit the DC Vice Captain and fetch the Inscription. I`ll make some changes.")
                           .AddOption("You~can~count~on~me.", 2)
                           .AddOption("Sorry,~I`m~busy.", 255)
                           .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Head~to~see~the~DC~Vice~Captain~to~fetch~the~Inscription.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(63)
            .FinalizeDialog();
                            return;
                        }
                        if (HasItems(client, 711408, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711408, 1, stream)) break;
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.AddSteed(stream, 300000, 1, 1, true, 255, 0, 150);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~a~+1~Steed(B)!");


                        }
                        break;
                    }

            }

        }
        [NpcAttribute(NpcID.SpringGeneralXu)]
        public static void SpringGeneralXu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1492);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringGeneralXu, client.Player.Class, 1493);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringGeneralXu, client.Player.Class, 1499);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1509);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringGeneralXu, client.Player.Class, 1510);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My job is to guard the Moon Spring.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Elder~Jiang~2.", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Elder~Jiang~3.", 40);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Visit~Spring~General.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Protect~the~Moon~Spring.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~Blade~Ghosts~(L83).", 20);

                        data.AddOption("Good.", 255);
                        data.AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        data.AddText("I really want to pay a visit to the elder. Please go ask if he is free to talk.")
                       .AddOption("I`d~love~to..", 41)
                       .AddOption("Why~not~go~there~yourself?", 255)
                       .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        client.CreateBoxDialog("Hurry~to~visit~Elder~Jiang.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        data.AddText("What? Catch fish on a straight hook and above the water? That`s interesting.")
                      .AddOption("Yeah..", 31)
                      .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(107)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.IncreaseExperience(stream, 2711348);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/6~EXP~Ball,~1,000~Silver~and~5~Panaceas!");
                        }
                        data.AddText("I really want to pay a visit to the elder. Please go ask if he is free to talk.")
                        .AddOption("I`d~love~to..", 41)
                        .AddOption("Why~not~go~there~yourself?", 255)
                        .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 30))
                            {
                                data.AddText("Thanks for what you`ve done for us.")
                              .AddOption("You`re~welcome.", 22)
                              .AddAvatar(107).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you killed 30 Level 83 Blade Ghosts, already?")
                                .AddOption("Not~yet.", 255)
                                .AddAvatar(107).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Have you ever seen the Level 83 Blade Ghosts? They`re smaller than Blade Ghosts, but much crueler. Well, I need you to go kill 30 of them!")
                                  .AddOption("I~won`t~let~you~down.", 21)
                                  .AddOption("Bye.", 255)
                                  .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(104)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348);

                            client.Inventory.Add(stream, 730002, 1, 1);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~a~+2~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.SendSysMesage("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver,~and~a~+2~Stone~(B)!", MsgMessage.ChatMode.System);

                        }
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~30~Level~83~Blade~Ghosts.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("Keep up the good job. Here`s your reward!")
                                   .AddOption("Thanks.", 12)
                                   .AddAvatar(107).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What are you waiting for? Go kill 50 Blade Ghosts!")
                                    .AddOption("I`m~on~my~way.", 255)
                                    .AddAvatar(107).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I don`t know why, but more and more Blade Ghosts are gathering outside the village. I need you to kill 50 of them!")
                                   .AddOption("You~can~count~on~me.", 11)
                                   .AddOption("Just~passing~by.", 255)
                                   .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {

                            data.AddText("Please make 3 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(104)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.SendSysMesage("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver,~5~Panaceas~and~2~Meteors!", MsgMessage.ChatMode.System);

                        }
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Hurry~to~kill~50~Blade~Ghosts.");
                        break;
                    }
                case 1:
                    {
                        data.AddText("It`s a good idea! I`ll ask the villagers to plant some cactus!")
                                     .AddOption("That~would~be~great!", 2)
                                     .AddAvatar(107).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.Add(stream, 720172);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~an~EXP~Ball~Scrap!");


                        }
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.ConvoyLeaderGu)]
        public static void ConvoyLeaderGu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierMu, client.Player.Class, 1483);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1484);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1491);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1492);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Nice to meet you. I`m new here.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Visit~the~Convoy~Leader.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Leader`s~Trouble.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Thunder~Stone.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Visit~Spring~General.", 30);

                        data.AddOption("Nice~to~meet~you,~too.", 255);
                        data.AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        data.AddText("Spring General Xu is guarding the Moon Spring. He needs your help.")
                                      .AddOption("I`ll~go~visit~him.", 31)
                                      .AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711405, 1))
                            {
                                client.Player.QuestGUI.SendAutoPatcher(1000, 482, 339, (ushort)NpcID.IronsmithLi);
                            }
                            else
                            {
                                data.AddText("Do you know anything about the Thunder Stone? I saw the Level 78 Rock Monsters carrying them. Please go grab one and deliver it to Ironsmith Li.")
                                          .AddOption("I`ll~do~right~away.", 255)
                                          .AddAvatar(104).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Do you know anything about the Thunder Stone? I saw the Level 78 Rock Monsters carrying them. Please go grab one and deliver it to Ironsmith Li.")
                                       .AddOption("You~can~count~on~me.", 21)
                                       .AddOption("I`m~afraid~I~can`t~help.", 255)
                                       .AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Level~78~Rock~Monsters~to~grab~a~Thunder~Stone.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30))
                            {
                                data.AddText("Thanks for your help! Please accept this reward!")
                                          .AddOption("Okay.", 12)
                                          .AddAvatar(104).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Level 78 Rock Monsters keep making troubles. They either attack villagers or steal the water. I need you to kill 30 of them!")
                                            .AddOption("I`m~on~my~way.", 255)
                                            .AddAvatar(104).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("The Level 78 Rock Monsters keep making trouble. They either attack villagers or steal any water they can find. I need you to kill 30 of them!")
                           .AddOption("You~can~count~on~me.", 11)
                           .AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");


                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Go~kill~30~Level~78~Rock~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Thanks. I`ll turn to you if I need your help.")
                            .AddOption("Yup.", 2)
                            .AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(104)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(720011, 0, 5, stream);
                            client.IncreaseExperience(stream, 711348);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~2,000~Silver~and~5~Panaceas!");


                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierMu)]
        public static void SoldierMu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierMu, client.Player.Class, 1480);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierMu, client.Player.Class, 1483);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringViceGeneralOu, client.Player.Class, 6128);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`ll give my life to guard this village. It`s my sworn duty!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Deliver~the~Water~(Daily).", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Rebuild~the~Village.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Visit~the~Convoy~Leader.", 10);

                        data.AddOption("Bye.", 255);
                        data.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("You brought the spring! Thank you!")
.AddOption("My~pleasure.", 21)
.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (HasItems(client, 711417, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(711417, 1, stream)) break;
                            client.IncreaseExperience(stream, 711348);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball~and~2,000~Silver!");
                            client.Player.QuestGUI.SendAutoPatcher(1000, 339, 244, (ushort)NpcID.ConvoyViceLeader);

                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            data.AddText("A team of soldiers were sent to protect the way to Moon Spring. Go visit Convoy Leader Gu and see if he needs any help.")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(101).FinalizeDialog();
                            break;
                        }
                        data.AddText("A team of soldiers were sent to protect the way to Moon Spring. Go visit Convoy Leader Gu and see if he needs any help.")
.AddOption("No~problem.", 11)
.AddOption("Later.", 255)
.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~visit~Convoy~Leader~Gu.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711349, 5))
                            {
                                data.AddText("Ah, so many Colorful Stones! Thank you! You`d been a great help!")
.AddOption("Welcome~to~Conquer~Online.", 3)
.AddAvatar(101).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I`m gathering some Colorful Stones to rebuild the village. Please kill the Rock Monsters to collect 5 Colorful Stones for me.")
.AddOption("I`m~on~my~way.", 2)
.AddAvatar(101).FinalizeDialog();
                            }
                            break;
                        }


                        data.AddText("The Rock Monsters are carrying these Colorful Stones. Can you collect 5 for me?")
.AddOption("Sure!", 2)
.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Go~kill~the~Rock~Monsters~to~collect~5~Colorful~Stones!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(101)
            .FinalizeDialog();
                            return;
                        }
                        if (HasItems(client, 711349, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711349, 5, stream)) break;
                            client.Inventory.Add(stream, Database.ItemType.DoubleExp, 1, 1);
                            client.IncreaseExperience(stream, 711348);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~ExpPotion!");


                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HanCheng)]
        public static void HanCheng(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1475);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HanCheng, client.Player.Class, 1476);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Be on alert while passing through the desert.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Water~Delivery.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Water~Delivery.", 10);


                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(60).FinalizeDialog();
                        break;
                    }

                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 40, 10))
                            {
                                data.AddText("Good job! Here`s your reward!")
.AddOption("Thanks.", 12)
.AddAvatar(60).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Those Hill Monsters are really annoying. They often tear up trees in the desert.")
                                    .AddText("~Go kill 40 Hill Monsters and 10 Hill Monsters L73!")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(60).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I notice that the Hill Monsters often tear up these trees. We must stop them!")
                            .AddText(" ~I need you to kill 40 Hill Monsters and 10 Hill Monsters L73!")
.AddOption("I`ll~give~them~a~lesson.", 11)
.AddOption("Let~me~think~about~it.", 255)
.AddAvatar(60).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {

                            data.AddText("Please make 2 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.IncreaseExperience(stream, 711348);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            uint[] Items = new uint[]
                                {
                                    (uint)(700000 + (uint)Role.Flags.Gem.RefinedDragonGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedFuryGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedGloryGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedKylinGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedMoonGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedPhoenixGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedRainbowGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedThunderGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedTortoiseGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedVioletGem)
                                };
                            uint ItemID = Items[(uint)Program.GetRandom.Next(0, Items.Length)];
                            client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Hill~Monsters~and~Level~73~Hill~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("I really appreciate your help.")
.AddOption("You`re~welcome.", 2)
.AddAvatar(60).FinalizeDialog();
                        break;
                    }
                case 2:
                    {

                        if (HasItems(client, 711416, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711416, 1, stream)) break;
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.Add(stream, 723340);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Diligence~Book!");

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DCWanYing)]
        public static void DCWanYing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1469);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1470);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1473);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1475);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HanCheng, client.Player.Class, 1476);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1488);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My husband, Han Cheng, and I have lived here since we were born.")
                            .AddText("~The village was kind of beautiful, but all has been destroyed by the sands");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("The~Cactus~3.", 50);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Green~Seeds~1.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Green~Seeds~1.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hill~Monster`s~Armor.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Water~Delivery.", 30);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("The~Desert~Herb.", 40);


                        data.AddOption("I~agree.", 255);
                        data.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        if (client.Inventory.Contain(720965, 1))
                        {
                            data.AddText("You have already got the cactus seeds, haven`t you?")
.AddOption("Oh,~yup.", 255)
.AddAvatar(159).FinalizeDialog();
                            break;
                        }
                        data.AddText("You want to buy cactus seeds? Yes, I have some, but each bag costs 500 Silver.")
.AddOption("I`ll~buy.", 51)
.AddOption("Forget~it.", 255)
.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720965);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711401, 1))
                            {
                                data.AddText("Thank you! You saved my husband!")
.AddOption("My~pleasure.", 43)
.AddAvatar(60).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please take the Desert Shovel and dig for a Desert Herb in the desert. Notice that you`ll need to wait 1 minute every time before you use the Desert Shovel.")
.AddOption("Roger.", 255)
.AddAvatar(60).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("My husband was wounded the other day. The doctor said only the Desert Herb could cure him. Please, I need your help to pick some.")
.AddOption("Where~to~find~it?", 41)
.AddOption("Let~me~think~about~it.", 255)
.AddAvatar(60).FinalizeDialog();
                        break;
                    }
                case 43:
                    {
                        if (HasItems(client, 720962, 1) && HasItems(client, 711401, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            if (!client.Inventory.Remove(720962, 1, stream)) break;
                            if (!client.Inventory.Remove(711401, 1, stream)) break;
                            client.Inventory.Add(stream, 730001, 1);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");

                        }
                        break;
                    }
                case 41:
                    {
                        data.AddText("I`ll give you a Desert Shovel. Please go to the desert and dig for a Desert Herb.")
.AddOption("I`ll~be~back~soon.", 42)
.AddAvatar(60).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720962);
                        client.CreateBoxDialog("You`ve~received~a~Desert~Shovel.~Hurry~to~dig~for~a~Desert~Herb.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711348, 1))
                            {
                                if (HasItems(client, 711348, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    if (!client.Inventory.Remove(711348, 1, stream)) break;
                                    client.IncreaseExperience(stream, 2919000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");

                                }
                            }
                            else
                            {
                                data.AddText("My husband, Han Cheng, must be patrolling the desert now.")
                               .AddText("~Could you fetch some water from the pool and take it to my husband?")
    .AddOption("I`m~on~my~way.", 255)
    .AddAvatar(159).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("My husband, Han Cheng, should be out patrolling the desert right now, but he forgot to take his water.")
                            .AddText("~Can you fetch some water from the pool and take it to my husband?")
.AddOption("No~problem.", 31)
.AddOption("I`m~busy.", 255)
.AddAvatar(159).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720974);
                        client.CreateBoxDialog("You`ve~received~a~bucket.~Hurry~to~fetch~some~water.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }

                case 20:
                    {
                        if (client.Inventory.Contain(711416, 1))
                        {
                            data.AddText("Is this really the Hill Monster`s Armor? Thank you! I`ll give it to my husband, right away!")
                                .AddOption("Nice.", 22)
                                .AddAvatar(159).FinalizeDialog();

                            break;
                        }
                        data.AddText("People say Level 73 Hill Monster`s armor can protect against wind and sand better than anything. I wish my husband had one!")
.AddOption("Let~me~help.", 21)
.AddOption("None~of~my~business.", 255)
.AddAvatar(159).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (HasItems(client, 711416, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(711416, 1, stream)) break;
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");

                        }
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Go~get~an~armor~from~the~Level~73~Hill~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1, 1))
                            {
                                data.AddText("Amazing! You`d been a great help!")
.AddOption("Never~mind.", 12)
.AddAvatar(159).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Now, let`s plant the tree seeds near the water sources. Looks like these places are all inside Hill Monsters territory, though. Would you like to go plant the seeds there?")
.AddOption("I`d~love~to.", 255)
.AddAvatar(159).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Now, let`s plant the tree seeds near the water sources. Looks like these places are all inside Hill Monsters territory, though. Would you like to go plant the seeds there?")
.AddOption("Sure.", 11)
.AddOption("Not~interested.", 255)
.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        if (HasItems(client, 720972, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(720972, 1, stream)) break;
                            client.Inventory.Add(stream, Database.ItemType.DoubleExp);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~3,000~Silver~and~an~EXP~Potion!");

                        }
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720972);
                        client.CreateBoxDialog("Go~plant~the~tree~seeds~near~the~water~sources.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                            {
                                data.AddText("Wow, the Tree Seeds you brought are wonderful! Thanks for your help!")
.AddOption("You`re~welcome.", 3)
.AddAvatar(159).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Few plants can live in the desert, but there are several green trees near the portal leading to the Wind Plain. Can you go there and bring me 3 Tree Seeds?")
.AddOption("Sure.", 255)
.AddAvatar(159).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Things wouldn`t have been so terrible if there were more green plants and flowers.")
                            .AddText("~Could you go to the portal leading to Wind Plain, and bring me 3 Tree Seeds?")
.AddOption("Sure.", 2)
.AddOption("I`m~afraid~not.", 255)
.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (HasItems(client, 720971, 3) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(720971, 3, stream)) break;
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                            goto case 10;
                        }
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Hurry~to~head~to~the~Green~Trees~and~fetch~3~Tree~Seeds.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.st1TreeSeed)]
        public static void st1TreeSeed(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1469);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                //client.Player.QuestGUI.IncreaseQuestObjectives(
                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                {
                    data.AddText("You`ve already collected a seed from this tree.")
.AddOption("Alright.", 255)
.FinalizeDialog();
                }
                else
                {
                    if (!client.Inventory.HaveSpace(1))
                    {

                        data.AddText("Please make 1 more space in your inventory.")
        .AddOption("Let me check.", 255)
        .FinalizeDialog();
                        return;
                    }

                    client.Player.AddPick(stream, "Collect", 4);
                }
            }
        }
        [NpcAttribute(NpcID.nd2TreeSeed)]
        public static void nd2TreeSeed(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1469);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                {
                    data.AddText("You`ve already collected a seed from this tree.")
.AddOption("Alright.", 255)
.FinalizeDialog();
                }
                else
                {
                    if (!client.Inventory.HaveSpace(1))
                    {

                        data.AddText("Please make 1 more space in your inventory.")
        .AddOption("Let me check.", 255)
        .FinalizeDialog();
                        return;
                    }
                    client.Player.AddPick(stream, "Collect", 4);
                }
            }
        }
        [NpcAttribute(NpcID.rd3TreeSeed)]
        public static void rd3TreeSeed(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1469);

            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                {
                    data.AddText("You`ve already collected a seed from this tree.")
.AddOption("Alright.", 255)
.FinalizeDialog();
                }
                else
                {
                    if (!client.Inventory.HaveSpace(1))
                    {

                        data.AddText("Please make 1 more space in your inventory.")
        .AddOption("Let me check.", 255)
        .FinalizeDialog();
                        return;
                    }
                    client.Player.AddPick(stream, "Collect", 4);
                }
            }
        }
        [NpcAttribute(NpcID.GeneralZhuGe)]
        public static void GeneralZhuGe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1465);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1466);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1471);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1479);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1477);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringSoldierZhao, client.Player.Class, 1505);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1506);

            switch (Option)
            {
                case 0:
                    {


                        data.AddText("It was a nice village in Desert, but turned into a refuge overnight after the sandstorm. Well, at least the villagers haven`t lost their hearts.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Speedy~Rescue.", 60);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Poisonous~Water~2.", 40);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~3.", 50);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hill~Monster`s~Threat.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~Hill~Monsters~(L73).", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Retrieve~the~Water~Source.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            if (client.Inventory.Contain(720975, 1))
                                data.AddOption("Thank you for the letter.", 30);

                        data.AddOption("They`ll~get~better.", 255);
                        data.AddAvatar(54).FinalizeDialog();


                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You`ve got the Antidote? Hurry to deliver it to Spring Soldier Zhao.")
                      .AddOption("Okay.", 255)
                      .AddAvatar(54).FinalizeDialog();
                            break;
                        }
                        data.AddText("All clues are indicating the Level Blade Ghosts! I need you to kill them to get the Antidote and take it to Spring Soldier Zhao.")
                        .AddOption("No~problem.", 51)
                        .AddOption("None~of~my~business.", 255)
                        .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Level~83~Blade~Ghosts!");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~2,000~Silver!");
                        }
                        data.AddText("All clues are indicating the Level Blade Ghosts! I need you to kill them to get the Antidote and take it to Spring Soldier Zhao.")
                            .AddOption("No~problem.", 51)
                            .AddOption("None~of~my~business.", 255)
                            .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 41:
                    {

                        data.AddText("Villagers are poisoned by the spring! I`ll figure that out!")
                        .AddOption("Great.", 41)
                        .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 30:
                    {

                        if (HasItems(client, 720975, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(720975, 1, stream)) break;
                            client.Inventory.Add(stream, 1060021);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Desert~City~Gate!");
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 50))
                            {
                                data.AddText("Thank you! The villagers will be able to go out and fetch water, again.")
.AddOption("Great.", 22)
.AddAvatar(54).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What are you waiting for? Go kill 50 Rock Monsters!")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(54).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("Oh-oh! The Rock Monsters on the west of the camp blocked the way to the Moon Spring! That`s the most important water source in the desert.")
                        .AddText("~I need you to kill 50 of them!")
.AddOption("You~can~count~on~me.", 21)
.AddOption("Bye.", 255)
.AddAvatar(54).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                        }

                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~50~Rock~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);


                        break;

                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30))
                            {
                                data.AddText("Great! That may be enough to keep them from being so bold.")

.AddOption("I~hope~so.", 12)
.AddAvatar(54).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("A horde of Hill Monsters L73 are going to attack the camp.")
                          .AddText("~To weaken their power, I need you to kill 30 of them!")
.AddOption("I`m~on~my~way.", 255)
 .AddAvatar(54).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("It`s reported that some Hill Monsters L73 are going to attack the camp, tonight!")
                            .AddText("~A good offense is the best defense! Go kill 30 Hill Monsters L73, right away!")
  .AddOption("I~won`t~disappoint~you.", 11)
  .AddOption("Why~me?", 255)
   .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, 720172);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~an~EXP~Ball~Scrap!");
                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~30~Level~73~Hill~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 50))
                            {
                                data.AddText("Well done! Please take these gifts, as your rewards.")
.AddOption("Thanks.", 3)
.AddAvatar(54).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("We should do something to save the camp. Go kill 50 of the Hill Monsters, and cut down their power!")
.AddOption("I~won`t~disappoint~you.", 255)
 .AddAvatar(54).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("More and more Hill Monsters are gathering outside the camp! It`s really dangerous.")
                            .AddText("~I need you to kill 50 of them!")
  .AddOption("I~won`t~disappoint~you.", 2)
  .AddOption("Why~not~do~that~yourself?", 255)
   .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.Inventory.Add(stream, 723340);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Diligence~Book!");
                        }

                        break;
                    }
                case 2:
                    {

                        client.CreateBoxDialog("Hurry~to~kill~50~Hill~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 60:
                    {

                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest7.MissionId, 20))
                            {
                                data.AddText("Really? You`ve cleared the way? Good job!")
                       .AddOption("Never~mind.", 62)
                       .AddAvatar(54).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("You came at the right time! A refugee camp in Desert was surrounded by a group of Hill Monsters.")
                         .AddText("~Go kill 20 of them, and report back to General ZhuGe!")
                          .AddOption("Okay.", 255)
                          .AddAvatar(54).FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 62:
                    {

                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 mores space in your inventory.")
         .AddOption("Let me check.", 255)
         .AddAvatar(53).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.IncreaseExperience(stream, 1739000);
                            client.SendSysMesage("You've~received~some~EXP,~2,000~Silver,~5~Panaceas~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                    }
                    break;
            }

        }

        [NpcAttribute(NpcID.IronsmithLi)]
        public static void IronsmithLi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IronsmithLi, client.Player.Class, 1467);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IronsmithLi, client.Player.Class, 1472);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IronsmithLi, client.Player.Class, 1481);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1491);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, I`m an ironsmith. What can I do for you? You need any smithing done, by chance?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Stolen~Grindstone.", 30);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Ironsmith`s~Request.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~New~Weapon.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Stolen~Grindstone.", 20);

                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Contain(711405, 1))
                        {
                            data.AddText("So is it a Thunder Stone? I`ll reveal the secret.")
                                     .AddOption("Good~luck.", 31)
                                     .AddAvatar(53).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Do you know anything about the Thunder Stone? I saw the Level 78 Rock Monsters carrying them. Please go grab one and deliver it to Ironsmith Li.")
                                      .AddOption("I`ll~do~right~away.", 255)
                                      .AddAvatar(53).FinalizeDialog();
                        }
                        break;
                    }
                case 31:
                    {
                        if (HasItems(client, 711405, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(711405, 1, stream)) break;
                            client.Inventory.Add(stream, 1060021);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~a~Desert~City~Gate!");
                        }
                        break;
                    }
                case 20:
                    {

                        if (client.Inventory.Contain(711399, 1))
                        {
                            data.AddText("Yes! That`s my grindstone! Thank you!")
.AddOption("No~problem.", 22)
.AddAvatar(53).FinalizeDialog();

                            break;
                        }


                        data.AddText("My grindstone was stolen by the Rock Monsters! It`s useless for them but to me, it`s important. Please, go retrieve it!")
      .AddOption("You~can~count~on~me.", 21)
      .AddOption("I`ll~think~about~it.", 255)
       .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 711399, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(711399, 1, stream)) break;
                            client.Inventory.Add(stream, 720172);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~an~EXP~Ball~Scrap!");
                        }
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Rock~Monsters~to~retrieve~the~grindstone.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 20))
                            {
                                data.AddText("Great! I`ll go report to the General. Here`s your reward, by the way.")

    .AddOption("Thank~you.", 12)
     .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("This is my Newly Made Hammer. I need you to test it.")
                          .AddText("~Just bring it with you and kill 20 Hill Monsters L73!")
         .AddOption("I`m~leaving.", 255)
          .AddAvatar(53).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Haha, this is my Newly Made Hammer. I need you to test it out.")
                          .AddText("~Just bring it with you and kill 20 Hill Monsters L73!")
         .AddOption("I`ll~try!", 11)
         .AddOption("I`m~going~anywhere.", 255)
          .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {

                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~+1~Stone~(B)!");
                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You`ve~received~a~Newly~Made~Hammer.~Go~kill~20~Level~73~Hill~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711344, 5, 0))
                            {
                                data.AddText("See? These hammers are great! Thank you very much!")
           .AddOption("My~pleasure.", 3)
            .AddAvatar(53).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("With the help of Hill Monster`s hammers, I`ll make better weapons.")
           .AddOption("I`ll~go~get~5~for~you.", 255)
            .AddAvatar(53).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("To be frank, I`m quite interested in the Hill Monster`s hammer. Could you collect some for me?")
            .AddOption("OK.~I`ll~go~get~5~for~you.", 2)
            .AddOption("Sorry,~I~can`t~help.", 255)
             .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 711344, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711344, 5, stream)) break;
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.Inventory.Add(stream, 720173);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silvers,~a~Small~Joy~Stone~and~2~Meteors!");
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                        }
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.CreateBoxDialog("Hurry~to~kill~the~Hill~Monsters~to~collect~5~Hammers!");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ViceGeneralDong)]
        public static void ViceGeneralDong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ViceGeneralDong, client.Player.Class, 1468);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ViceGeneralDong, client.Player.Class, 1474);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ViceGeneralDong, client.Player.Class, 1478);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1490);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the Vice General, you can call me Dong. Watching the refugee camp is my duty.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("The~Cactus~5.", 30);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Go~Deep~into~the~Lair.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Supply~Line.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Sand-proof~Wall.", 20);


                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 30:
                    {

                        data.AddText("Good idea! I`ll ask the villagers to plant more of the cactus. Thanks for your advice!")
               .AddOption("You`re~welcome.", 31)
                .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Player.QuestGUI.SendAutoPatcher(1000, 308, 456, (ushort)NpcID.ElderJiang);

                            client.Inventory.Add(stream, 160157, 1);
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.SendSysMesage("You`ve~received~some~EXP,~2,000~Silver,~Tiger~Boots~and~a~+1~Stone~(B)!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 1, 1, 1))
                            {
                                data.AddText("I`m the Vice General, you can call me Dong. Watching the refugee camp is my duty.")
               .AddOption("The~Slabstones.", 25)
               .AddOption("Nice~to~see~you.", 255)
                .AddAvatar(59).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Rock Monsters are known for carrying fine Slabstones. You can collect some, and use them to build a wall around the pool in 4 directions.")
                .AddOption("I`ll~come~back~soon.", 255)
                 .AddAvatar(59).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The sand is swallowing the water pool. We need to collect some slabstones to build a sand-proof wall around the pool.")
                       .AddOption("Ready~to~help.", 21)
                       .AddOption("I`ll~think~about~it.", 255)
                        .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        data.AddText("You`ve built the wall! You`ve been such a big help!")
               .AddOption("My~pleasure.", 26)
                .AddAvatar(59).FinalizeDialog();

                        break;
                    }
                case 26:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.SendSysMesage("You`ve~received~by~the~Vice~General~Dong~with~EXP~worth~1/2~EXP~Ball,~3,000~Silver,~and~5~Panaceas,~for~building~the~sand~prof~wall!", MsgMessage.ChatMode.System);
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("You`re going to turn to the Rock Monsters for the Slabstones. After that, go build a wall around the water pool in 4 directions.")
                     .AddOption("I`ll~come~back~soon.", 22)
                      .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(4))
                        {
                            data.AddText("Please make 4 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        client.Inventory.AddItemWitchStack(720970, 0, 4, stream);
                        client.CreateBoxDialog("Go~kill~the~Rock~Monsters,~and~gather~some~Slabstones!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(711415, 5))
                        {
                            data.AddText("Really? You`ve gathered enough Heavy Axes? Thank you!")
                      .AddOption("Welcome.", 12)
                       .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        data.AddText("Emergency! Our armory is running low on weapons and the supply lines are cut off!")
                            .AddText("~I need you to get 5 Heavy Axes from the Hill Monsters or Hill Monsters L73!")
                         .AddOption("Got~it.", 11)
                         .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Kill~the~Hill~Monsters~and~Level~73~Hill~Monsters~to~get~5~Heavy~Axes!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 711415, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(711415, 5, stream)) break;

                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                        {
                            data.AddText("You made it? Were they scared? Haha!")
                       .AddOption("Yeah,~yeah.", 3)
                        .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        data.AddText("We have a plan. We need a brave man, like you, to go deep into the Hill Monster`s lair and set a fire. That should smoke them out!")
                        .AddOption("Count~me~in.", 2)
                        .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 720980, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(720980, 1, stream)) break;
                            client.SendSysMesage("You`ve~received~some~EXP,~2,000~Silvers~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 2500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Inventory.Add(stream, 720980);
                        client.CreateBoxDialog("You`ve~got~a~torch.~Hurry~to~set~a~fire~and~threaten~the~Hill~Monsters!");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KeYulun)]
        public static void KeYulun(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KeYulun, client.Player.Class, 1462);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KeYulun, client.Player.Class, 1463);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KeYulun, client.Player.Class, 1464);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This desert is often called the throat of the Silk Road. Merchants from all over the world came here, as well as evildoers.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Clear~the~Path.", 20);
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Ginseng~Delivery.", 1);
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Save~the~Company.", 10);
                        }
                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(47).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 40, 10))
                        {
                            data.AddText("Great! We`re going home! Thanks a million!")
                          .AddOption("Have~a~good~journey.", 22)
                           .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        data.AddText("My followers and I are planning to go home right away. But the problem is, the path is blocked by the bloody monsters, again!")
                            .AddText("~I`m wondering if you can help us kill 40 Sand Monsters and 10 Level 68 Sand Monsters.")
                            .AddOption("You~can~count~on~me.", 21)
                            .AddOption("Sorry,~I`m~busy.", 255)
                             .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You`ve~received~some~EXP,~3,000~Silvers,~Unique~Snakeskin~Boots~and~a~+1~Stone~(B)!", MsgMessage.ChatMode.System);
                            client.Inventory.Add(stream, 160137, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.QuestGUI.SendAutoPatcher(1000, 486, 346, 8439);
                        }
                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 7))
                            {
                                data.AddText("Thanks for what you`ve done for us! Please accept these rewards!")
                             .AddOption("My~pleasure.", 12)
                              .AddAvatar(47).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("While going through the desert, we were hit by a sand storm and my follower was kidnapped by the Level 68 Sand Monsters.")
                            .AddText("~Please save the poor guy!")
                                .AddOption("I`ll~try~my~best.", 255)
                                 .AddAvatar(47).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("While going through the desert, we were hit by a sand storm and my follower was kidnapped by the Level 68 Sand Monsters.")
                            .AddText("~Please save the poor guy!")
                                .AddOption("I`ll~try~my~best.", 11)
                                .AddOption("Leave.", 255)
                                 .AddAvatar(47).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You've~received~some~EXP,~3,000~Silver~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.IncreaseExperience(stream, 2919000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(1060021, 1))
                        {
                            data.AddText("Tell me how to thank you enough! You saved my life!")
                              .AddOption("My~pleasure.", 3)
                               .AddAvatar(47).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Thank God you came! I ran into a sand storm on my way here, and was badly injured. Can you bring me a Ginseng, please?")
                                 .AddOption("I`ll~be~back~soon.", 2)
                                 .AddOption("Leave.", 255)
                                  .AddAvatar(47).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Hurry~to~fetch~a~Ginseng!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);

                        break;
                    }
                case 3:
                    {
                        if (HasItems(client, 1060021, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(1060021, 1, stream)) break;
                            client.Inventory.Add(stream, 1060021, 1);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~2,000~Silver~and~a~Desert~City~Gate!");
                            client.IncreaseExperience(stream, 1719000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
            }
        }
    }
}
