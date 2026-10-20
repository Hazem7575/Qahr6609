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
        [NpcAttribute(NpcID.TruthTaoist)]
        public static void TruthTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 1612);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 1618);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 1619);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1627);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I was appointed to stay here and prevent the HawKings from escaping. I`d be really happy if you could lend me a hand.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1) == false)
                            data.AddOption("Elemental~Circle~(2).", 40);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~can~I~do?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Blockade", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Blockade~2", 20);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Patrol~the~Islands.", 30);




                        data.AddOption("Sorry,~but~I`m~busy.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        data.AddText("Don`t worry, I know how to stop the HawKings, here. I hope the elemental circle will soon be summoned!")
                            .AddOption("We`ll~win!", 41)
                            .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        client.CreateBoxDialog("Now~you~can~report~back~to~the~Mystery~Taoist.");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest5.MissionId, 1);

                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1, 1, 1, 1, 1, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~20~minutes~of~EXP~and~2,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("You`ve checked all the stones, and I think you`re becoming more familiar with the islands.")
.AddOption("Definitely.", 255)
.AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please go to each of the islands and check on those stones!")
                .AddOption("Ok.", 255)
                .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("To prevent the monsters from landing, I placed 6 Guarding Stones on the islands.")
                            .AddText("~Could you please go to each island and see if these stones are safe?")
                 .AddOption("At~your~service.", 31)
                 .AddOption("Sorry,~but~I`m~busy.", 255)
                 .AddAvatar(35).FinalizeDialog();
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
                            if (!client.Inventory.Contain(720992, 1))
                                client.Inventory.Add(stream, 720992);

                            data.AddText("Go to the two portals and use the pill to stop the HawKings. Hurry up!")
                   .AddOption("Ok.", 255)
                   .AddAvatar(35).FinalizeDialog();



                            break;
                        }
                        data.AddText("I`ve made the special pill! Now, I need you to burn it on the southern and northern bridges.")
                            .AddText("~And then, the bridges will be blocked and the Haw Kings won`t be able to escape!")
                            .AddText("~After that, you`re going to inform the Mystery Taoist near the northern bridge and ask him to be on alert.")
                          .AddOption("Alright.", 21)
                          .AddOption("Sorry,~but~I`m~busy.", 255)
                          .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.Contain(720992, 1))
                            client.Inventory.Add(stream, 720992);
                        client.CreateBoxDialog("Go~burn~the~pill~on~the~southern~and~northern~bridges!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711454, 5))
                            {
                                if (HasItems(client, 711454, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711454, 5, stream)) break;
                                    client.Inventory.AddItemWitchStack(720172, 0, 1, stream);
                                    client.CreateBoxDialog("~You~received~30~minutes~of~Exp,~3,000~silver~and~1~EXP~Ball~Scrap!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Such wonderful feathers! Now I can start to make the special pill. Thanks for your help!")
.AddOption("You`re~welcome.", 255)
.AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Go collect 5 feathers from the Haw Kings before they run out of the island! Hurry up!")
                        .AddOption("Got~it.", 255)
                        .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We`ve built some wooden bridges for heroes like you, as easy accesses to the islands. However, Those Haw Kings")
                           .AddText("~also use the bridges and get out to make troubles! I need you to stop them!")
                         .AddOption("At~your~service.", 11)
                         .AddOption("Sorry,~but~I`m~busy.", 255)
                         .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("I need you to kill Haw Kings and collect 5 of their feathers. I`ll use them to make a special pill.")
                        .AddText("~And then, you`re going to burn the pill on both bridges, to prevent them from escaping.")
                      .AddOption("I~see.", 12)
                      .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Collect~5~feathers~from~the~HawKings.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {

                                data.AddText("Please make 1 more space in your inventory.")
                                      .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(720990, 1))
                                client.Inventory.Add(stream, 720990);

                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1, 1))
                            {
                                if (HasItems(client, 720990, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    if (!client.Inventory.Remove(720990, 5, stream)) break;
                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~5~Panacea!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Well done! The HawKings can never leave the magic circle, though will be destroyed sooner or later!")
.AddOption("Good~luck.", 255)
.AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please use the Sealing Tokens at the 4 corners of Bird Island to reinforce the magic circle.")
                               .AddOption("Ok.", 255)
                               .AddAvatar(35).FinalizeDialog();

                            }
                            break;

                        }
                        data.AddText("I`ve been using a magic circle to seal the HawKings all this time, but the monsters have destroyed many of the")
                            .AddText("~Sealing Tokens. I hope you can use these new tokens to reinforce the magic circle. They must be placed in the 4 corners of Bird Island.")
                          .AddOption("Got~it..", 2)
                          .AddOption("Sorry,~but~I`m~busy.", 255)
                          .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720990);
                        client.CreateBoxDialog("Place~the~Sealing~Tokens~at~the~4~corners~of~Bird~Island~to~reinforce~the~magic~circle.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Lily)]
        public static void Lily(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (!client.Inventory.HaveSpace(1))
                {
                    Dialog data = new Dialog(client, stream);
                    data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255).FinalizeDialog();
                    return;
                }
                if (client.Inventory.Contain(711440, 1))
                {
                    client.SendSysMesage("You've already got a Lily!", MsgMessage.ChatMode.System);
                    return;
                }
                else
                {
                    client.Player.AddPick(stream, "Lily", 3);
                }
            }
        }

        [NpcAttribute(NpcID.WillowLeaf)]
        public static void WillowLeaf(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (!client.Inventory.HaveSpace(1))
                {
                    Dialog data = new Dialog(client, stream);
                    data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255).FinalizeDialog();
                    return;
                }
                if (client.Inventory.Contain(711443, 1))
                {
                    client.SendSysMesage("You've already got a Willow Leaf!", MsgMessage.ChatMode.System);
                    return;
                }
                else
                {
                    client.Player.AddPick(stream, "Willow Leaf", 3);
                }
            }
        }

        [NpcAttribute(NpcID.Jasmine)]
        public static void Jasmine(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (!client.Inventory.HaveSpace(1))
                {
                    Dialog data = new Dialog(client, stream);
                    data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255).FinalizeDialog();
                    return;
                }
                if (client.Inventory.Contain(711442, 1))
                {
                    client.SendSysMesage("You've already got a Jasmine!", MsgMessage.ChatMode.System);
                    return;
                }
                else
                {
                    client.Player.AddPick(stream, "Jasmine", 3);
                }
            }
        }

        [NpcAttribute(NpcID.WhiteChrysanthemum)]
        public static void WhiteChrysanthemum(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (!client.Inventory.HaveSpace(1))
                {
                    Dialog data = new Dialog(client, stream);
                    data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255).FinalizeDialog();
                    return;
                }
                if (client.Inventory.Contain(711441, 1))
                {
                    client.SendSysMesage("You've already got a White Chrysanthemum!", MsgMessage.ChatMode.System);
                    return;
                }
                else
                {
                    client.Player.AddPick(stream, "WhiteChrysanthemum", 3);
                }
            }
        }
        [NpcAttribute(NpcID.XimenQing)]
        public static void XimenQing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StrangeBox, client.Player.Class, 1600);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1601);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StrangeBox, client.Player.Class, 1602);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1603);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1599);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1626);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I like flowers. They have their own language. For example, red roses are given as a symbol of love, beauty and passion...")
                            .AddText("~Hmm, are you coming for a quest or what?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && !client.Inventory.Contain(711461, 1))
                            data.AddOption("Elemental~Circle.", 60);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Strange~Box~1.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Strange~Box~2.", 10);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("Strange~Box~3.", 20);

                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                    data.AddOption("Strange~Box~4.", 30);
                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Flower~Arrangement.", 40);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Bird~Beaks.", 50);

                        data.AddOption("Nothing.", 255);
                        data.AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 60:
                    {
                        data.AddText("My wood sword can help defeat the HawKings? Great! Here you are. I hope you`ll succeed, soon!")
                                    .AddOption("Thanks.", 61)
                                    .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~received~the~Wood~Sword~from~the~Playboy~(wood~element)!");
                        client.Inventory.Add(stream, 711461);
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711445, 5))
                            {
                                if (HasItems(client, 711445, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                                {
                                    if (!client.Inventory.Remove(711445, 5, stream)) break;
                                    client.Inventory.AddItemWitchStack(720173, 0, 1, stream);
                                    client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~1~Small~Joy~Stone!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Well done! You deserve the reward!")
.AddOption("Thank~you!", 255)
.AddAvatar(74).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("I need you to kill L88 Birdmen and bring me 5 of their Bird Beaks.")
  .AddOption("Deal", 255)
  .AddAvatar(74).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Damn it! The L88 Birdmen ruined my garden! Please, I need you to kill them and bring back 5 of their Bird Beaks.~I`ll pay you!")
                                .AddOption("Deal!", 51)
                                .AddOption("Not~interested.", 255)
                                .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Please~kill~L88~Birdmen~to~collect~5~Bird~Beaks.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711440, 1) && client.Inventory.Contain(711443, 1)
                                && client.Inventory.Contain(711441, 1) && client.Inventory.Contain(711442, 1))
                            {
                                if (HasItems(client, 711440, 1) && HasItems(client, 711443, 1) && HasItems(client, 711442, 1) && HasItems(client, 711441, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                                {
                                    if (!client.Inventory.Remove(711440, 1, stream)) break;
                                    if (!client.Inventory.Remove(711443, 1, stream)) break;
                                    if (!client.Inventory.Remove(711442, 1, stream)) break;
                                    if (!client.Inventory.Remove(711441, 1, stream)) break;
                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);

                                    client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Great! I didn`t expect you would come back so sonn! Thank you!")
                                  .AddOption("You`re~welcome.", 255)
                                  .AddAvatar(74).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need you to collect a Lily, White Chrysanthemum, Jasmine and a Willow Leaf for me.")
                                    .AddOption("I`m~on~my~way.", 255)
                                    .AddAvatar(74).FinalizeDialog();

                            }

                            break;
                        }

                        data.AddText("Yep, I`m studying flower arranging, these days. Can you help me collect a Lily, White Chrysanthemum, Jasmine and a Willow Leaf?")
                             .AddOption("Okay.", 41)
                             .AddOption("Leave.", 255)
                             .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        client.CreateBoxDialog("Now~go~collect~a~Lily~for~Ximen~Qing!");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Contain(711448, 1))
                        {
                            data.AddText("This is a Phoenix Jade. Please put it inside the Strange Box. I want to give it to the fairy, as a gift.")
                                .AddOption("I`m~on~my~way.", 255)
                                .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        data.AddText("You can`t imagine how excited I am! This is a Phoenix Jade. Put it inside the Strange Box, okay?")
                            .AddText("~I want to give it to the fairy, as a gift.")
                                                        .AddOption("Okay.", 31)
                                                        .AddOption("Later.", 255)
                                                        .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711448);
                        client.CreateBoxDialog("Please~put~the~Phoenix~Jade~in~the~Strange~Box!");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (HasItems(client, 711447, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(711447, 1, stream)) break;
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        data.AddText("That is amazing! This letter is written by a fairy! She wants to thank me for taking care of the flowers in the island.")
                                                      .AddOption("Oh,~I~see.", 255)
                                                      .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("In my opinion, maybe we can use a L88 Birdman`s Shap Claw to open this strange box.")
                                                 .AddOption("I`m~on~the~way.", 255)
                                                 .AddAvatar(74).FinalizeDialog();
                            break;
                        }

                        data.AddText("You mean the red box? I`m curious, too. Maybe you can open it yourself.")
                                                   .AddOption("How?", 11)
                                                   .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("I don`t know what`s inside, either. It just somehow appeared here, the other day.")
                       .AddText("~I did try to open the box but failed. In my opinion, maybe we can use L88 Birdman`s Shap Claw to open this strange box.")
                                                .AddOption("Sounds~reasonable.", 12)
                                                .AddOption("I~don`t~think~so.", 255)
                                                .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Please~kill~L88~Birdman~to~get~their~Sharp~Claw.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {


                            client.CreateBoxDialog("You`ve~received~some~EXP~and~2,000~Silver!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        data.AddText("You mean the red box? I`m curious, too. Maybe you can open it yourself.")
                                                      .AddOption("How?", 11)
                                                      .AddAvatar(74).FinalizeDialog();

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.StrangeBox)]
        public static void StrangeBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StrangeBox, client.Player.Class, 1600);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1601);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StrangeBox, client.Player.Class, 1602);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1603);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("This is a strange box.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Interesting.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Strange~Box~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Strange~Box~3.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Strange~Box~4.", 30);
                        data.AddOption("Leave.", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (HasItems(client, 711448, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(711448, 1, stream)) break;
                            client.CreateBoxDialog("You~saw~a~mysterious~halo~and~found~yourself~energized!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Are you sure you want to use the claw to open this strange box?")
                                                  .AddOption("Yes.", 21)
                                                  .AddOption("No.", 255)
                                                  .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~opened~the~box~and~found~a~letter~inside.~Deliver~it~to~Ximen~Qing!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Inventory.Add(stream, 711447);
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(711446, 1))
                        {
                            if (HasItems(client, 711446, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(711446, 1, stream)) break;

                                client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                                client.IncreaseExperience(stream, 3719000);
                                client.Player.Money += 3000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            }
                            data.AddText("Are you sure you want to use the claw to open this strange box?")
                                                   .AddOption("Yes.", 21)
                                                   .AddOption("No.", 255)
                                                   .AddAvatar(67).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("As Ximen Qing said, you`d better go get a L88 Birdman`s Sharp Claw to open the box.")

                                                     .AddOption("Alright.cre", 255)
                                                     .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("You seem curious about the box. Would you like to open it?")
                                                      .AddOption("Yeah,~yeah.", 2)
                                                      .AddOption("No~need.", 255)
                                                      .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        data.AddText("The box is locked and you can`t open it. Why not ask Ximen Qing? Maybe he knows something.")
                                                      .AddOption("Okay.", 255)
                                                      .AddAvatar(67).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.CityGeneral)]
        public static void CityGeneral(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CityGeneral, client.Player.Class, 1597);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Do not make trouble, here. If you find anything abnormal or need my help with a quest, come talk to me.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Birdman~Feathers.", 1);


                        data.AddOption("I~see.", 255);
                        data.AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {

                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                                    client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Thank you for what you`ve done!")
                                                          .AddOption("Never~mind.", 255)//10
                                                          .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("We don`t have enough soldiers to guard the West Gate, and the Birdmen just keep robbing the travelers outside the gate.")
                          .AddText("~I need you to teach them a lesson! Go collect their feathers and burn them at the West Gate!")
                          .AddText("~That will keep them from being so bold.")
                                 .AddOption("You~can~count~on~me.", 255)
                                 .AddAvatar(30).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("We don`t have enough soldiers to guard the West Gate, and the Birdmen just keep robbing the travelers outside the gate.")
                            .AddText("~I need you to teach them a lesson! Go collect their feathers and burn them at the West Gate!")
                            .AddText("~That will keep them from being so bold.")
                                   .AddOption("You~can~count~on~me.", 2)
                                   .AddOption("None~of~my~business.", 255)
                                   .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Go~collect~some~Birdman~Feathers~and~burn~them~at~the~West~Gate.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WindTaoist)]
        public static void WindTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindTaoist, client.Player.Class, 1595);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindTaoist, client.Player.Class, 1596);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Bird Island is really a wonderful place. I believe that some hero will appear here, soon.");
                        data.AddText("~Will you be that hero?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Of~course.", 1);
                        //   else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        //     data.AddOption("What`s~next?", 10);

                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(720985, 1))
                                client.Inventory.Add(stream, 720985);
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5))
                            {

                            }
                            else
                            {
                                data.AddText("Use the Transformation Amulet to transform and kill 5 Birdmen. Good luck!")
           .AddOption("Ok.", 255)
           .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("The amulet is finished! Haha. Find 5 Birdmen, and use the amulet to transform them into the Pheasants.")
                            .AddText("~Then you can kill them easily. Remember, the amulet can used once per minute. What`s more, you`ll")
                            .AddText("~have the chance to transform yourself into a Pheasant for 10 seconds. Good luck!")
                                            .AddOption("Ok.", 11)
                                            .AddOption("Sorry,~but~I`m~busy.", 255)
                                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720985, 1))
                            client.Inventory.Add(stream, 720985);
                        client.CreateBoxDialog("Use~the~Transformation~Amulet~to~kill~5~Birdmen.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(1001020, 1)
                                && client.Inventory.Contain(1002000, 1)
                                && client.Inventory.Contain(711444, 1))
                            {
                                if (HasItems(client, 711444, 1) && HasItems(client, 1002000, 1) && HasItems(client, 1001020, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    if (!client.Inventory.Remove(711444, 1, stream)) break;
                                    if (!client.Inventory.Remove(1002000, 1, stream)) break;
                                    if (!client.Inventory.Remove(1001020, 1, stream)) break;

                                    client.CreateBoxDialog("Congratulations!~You~received~3,270,000~Exp~and~2,000~silver!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Great! With all these materials, I can make the amulet! Thank you, brave hero!")
                                                          .AddOption("Good~luck.", 255)//10
                                                          .AddAvatar(67).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Hurry up! We should teach a lesson to the Birdmen as soon as possible!")
                                           .AddOption("On~my~way!", 255)
                                           .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Have you heard about the evil Birdmen? They`re gathering outside of the city and often attack travelers.")
                            .AddText("~I know a kind of amulet that can transform them into the weaker Pheasants. Could you please buy a Pearl Ointment")
                            .AddText("~and Panacea from the Pharmacist, and a piece of Rice Paper from the Storekeeper?")
                                                 .AddOption("Ok.", 2)
                                                 .AddOption("Sorry,~but~I`m~busy.", 255)
                                                 .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Help~the~Wind~Taoist~collect~the~materials~he~needs.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WealthyWan)]
        public static void WealthyWan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWansWife, client.Player.Class, 1604);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWan, client.Player.Class, 1605);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWan, client.Player.Class, 1606);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWan, client.Player.Class, 1607);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1626);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("It has been my dream to be an aviator since I was a little boy! I believe that one day, I can fly high in the blue sky!");
                        data.AddText("~Higher than anyone else has ever gone! YEAH!!!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && !client.Inventory.Contain(711463, 1))
                            data.AddOption("Elemental~Circle.", 40);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Just~be~safe.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Aviator~1.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Aviator~2.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Aviator~3.", 30);

                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        data.AddText("You need some gunpowder? I`ve prepared a lot for my aircraft. Here you are! I hope we can eradicate")
                            .AddText("~the HawKings as soon as possible!")
                                  .AddOption("Thanks.", 41)
                                  .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~received~some~Powder~from~Wealthy~Wan~(fire~element)!");
                        client.Inventory.Add(stream, 711463);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {

                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~a~Level~90~Unique~weapon!");
                                    client.IncreaseExperience(stream, 4719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                        client.Inventory.Add(stream, 410177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//leafBlade
                                    else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                        client.Inventory.Add(stream, 480177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//club
                                    else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                        client.Inventory.Add(stream, 500167, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BambooBow
                                    else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                        client.Inventory.Add(stream, 601177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//iron katana
                                    else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                        client.Inventory.Add(stream, 610177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BambooPrayerBeads
                                    else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                        client.Inventory.Add(stream, 611177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//recruitRapier
                                    else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                        client.Inventory.Add(stream, 421177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//SpellBacksword 
                                    else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                        client.Inventory.Add(stream, 617177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//WhiteWaxNunchaku 
                                    else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                        client.Inventory.Add(stream, 626177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                }
                                data.AddText("It has been my dream to be an aviator since I was a little boy! I believe that one day, I can fly high in the blue sky!")
                                    .AddText("~Higher than anyone else has ever gone! YEAH!!!")
                                                          .AddOption("I`m~just~passing~by.", 255)
                                                          .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("We`ll start to fly, now. Are you ready?")
                                             .AddOption("Yes.", 33)
                                             .AddOption("I`ll~think~about~it.", 255)
                                             .AddAvatar(35).FinalizeDialog();
                            }

                            break;
                        }

                        data.AddText("Armorer Yu is so kind! Now we can go out and test my craft. Thanks for your help, and I hope you can complete the first flight.")
                                                .AddOption("I`m~looking~forward~to~it.", 31)
                                                .AddOption("Not~interested.", 255)
                                                .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        client.Teleport(764, 550, 1015);
                        client.CreateBoxDialog("The~kite~broke~right~after~rising~into~the~air.~You~fell~from~the~sky~and~were~hurt!");
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "M_Fire1");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest4.MissionId, 1);
                        break;
                    }
                case 31:
                    {
                        data.AddText("I knew you wouldn`t be afraid of it. Take a break first, and I`ll complete the preparation work.")
                                                .AddOption("Ok.", 32)
                                                .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711450, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp~and~3,000~silver!");
                                    client.IncreaseExperience(stream, 4719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Armorer Yu is so kind! Now we can go out and test my craft. Thanks for your help, and I hope you can complete the first flight.")
                                                          .AddOption("I`m~looking~forward~to~it.", 30)
                                                          .AddOption("Not~interested.", 255)
                                                          .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please bring back some powder from Armorer Yu.")
                                                          .AddOption("Ok.", 255)
                                                          .AddAvatar(35).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("The last material I need is some gunpowder. Could you please find Armorer Yu and get some for me?")
                                                       .AddOption("Ok.", 21)
                                                       .AddOption("Sorry,~but~I`m~busy.", 255)
                                                       .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Speak~with~Armorer~Yu~and~get~some~Gunpowder.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711449, 5))
                            {

                                if (HasItems(client, 711449, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711449, 5, stream)) break;
                                    client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 4719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Thanks for your help! When everything is done, please come back and join me in a trip to the sky! Haha!")
                                                          .AddOption("Good~luck!", 20)
                                                          .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Could you please kill the BirdmanL88 and collect 5 feathers?")
                                        .AddOption("Ok.", 255)
                                        .AddAvatar(35).FinalizeDialog();

                            }

                            break;
                        }

                        data.AddText("Haha, I think I`ll realize my dream, very soon. Let me tell you, I want to make a huge kite and use a large rocket to launch it.")
                            .AddText("~The first thing I need to do is finish the kite. Could you please collect 5 feathers from the BirdmenL88 that are outside?")
                                        .AddOption("Ok.", 11)
                                        .AddOption("I`ll~think~about.", 255)
                                        .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Kill~the~BirdmanL88~to~collect~5~feathers.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;

                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 20))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                      .AddOption("Let me check.", 255)
                                      .AddAvatar(35).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 1, stream);
                                client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~an~Exp~Potion!");
                                client.IncreaseExperience(stream, 4719000);
                                client.Player.Money += 3000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            }
                            data.AddText("My wife is always being too careful... And I thought my mother was overprotective! Anyway, thanks for your help!")
                                                      .AddOption("You`re~welcome.", 255)
                                                      .AddAvatar(35).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Didn`t I ask you to go outside of the city and kill 20 Birdmen L88?")
                                                               .AddOption("I~see.", 255)
                                                               .AddAvatar(35).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.WealthyWansWife)]
        public static void WealthyWansWife(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWansWife, client.Player.Class, 1604);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Sigh, my husband wanted to become an aviator for a long time. Maybe having a dream is good for a man... Well, at least I want to make sure he is safe.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I~can~help.", 1);


                        data.AddOption("You`re~a~good~wife.", 255);
                        data.AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Didn`t I ask you to go outside of the city and kill 20 Birdmen L88?")
                                    .AddOption("I~see.", 255)
                                    .AddAvatar(110).FinalizeDialog();
                            break;
                        }
                        data.AddText("Thanks! He is always working on his machine outside, but there are so many BirdmenL88 outside of the city. I`m really")
                            .AddText("~worried about him. Could you please go and kill 20 BirdmenL88? Show them no mercy!")
                                     .AddOption("Alright.", 2)
                                     .AddOption("Sorry,~but~I`m~busy.", 255)
                                     .AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Go~outside~of~the~city~and~kill~20~BirdmenL88.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.LittleBen)]
        public static void LittleBen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArmorerYu, client.Player.Class, 6129);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Nobody likes me, even my father only cares about his boring job. Maybe I should just disappear...");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Your~dad`s~worried~about~you!", 1);


                        data.AddOption("You~naughty~boy!", 255);
                        data.AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Really? So, my father sent you here? He may remember me at times, so what? Nothing will change, and he will still fix his sights on his work again, tomorrow.")
                            .AddText("~Look, how about we play some Rock-Paper-Scissors? If you win, I`ll go home. If not... Haha, I`ll push you off the roof! The rules are easy. `Rock` beats `Scissors`,")
                            .AddText("~`Scissors` beats `Paper`, and `Paper` beats `Rock`. You got it?")
                                      .AddOption("Yeah,~let`s~start!", 2)
                                      .AddOption("I~need~more~time~to~remember.", 255)
                                      .AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Rock-Paper-Scissors...Shoot!")
                                      .AddOption("Rock!", 3)
                                      .AddOption("Scissors!", 4)
                                      .AddOption("Paper!", 3)
                                      .AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Wh...A tie? I want to win! Let`s play again. Rock-Paper-Scissors...Shoot!")
                                      .AddOption("Rock!", 3)
                                      .AddOption("Scissors!", 4)
                                      .AddOption("Paper!", 3)
                                      .AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        data.AddText("Alright, it looks like I lose. Whew, I guess I`ll go home, then. See you around!")
                                         .AddOption("You`re~a~good~boy.", 5)
                                         .AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You~received~20~minutes~of~EXP~and~2,000~Silver!");
                            client.IncreaseExperience(stream, 4719000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        ActionQuery action;

                        action = new ActionQuery()
                        {
                            ObjId = (uint)NpcID.LittleBen,
                            Type = ActionType.RemoveEntity
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ArmorerYu)]
        public static void ArmorerYu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArmorerYu, client.Player.Class, 1592);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArmorerYu, client.Player.Class, 6129);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWan, client.Player.Class, 1606);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("My father used to be a famous armorer, and I want to work as well as him. The most important thing")
                            .AddText("~is to forge an ultimate weapon.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && !client.Inventory.Contain(711450, 1))
                            data.AddOption("The~Aviator~2.", 30);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Can~I~help~you?", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Lonely~Ben.", 20);

                        data.AddOption("You`ll~make~it.", 255);
                        data.AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        /*sa..
alchemist..
black
circus
china !
*/
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        data.AddText("Wealthy Wan is still insisting on his foolish dream... Well, if you want to get some gunpowder from me, you`ll need")
                            .AddText("~need to answer 5 questions correctly!")
                                       .AddOption("Go~ahead.", 31)
                                       .AddOption("Sorry,~but~I`m~busy.", 255)
                                       .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("1. Gunpowder is actually a mixture of sulfur, charcoal and what?")
                                .AddOption("Saltpeter.", 32)
                               .AddOption("Saltpeter.", 39)
                                .AddOption("Saltpeter.", 39)
                                .AddOption("Saltpeter.", 39)
                                .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        data.AddText("2. The powder was first made by who?")
                              .AddOption("Doctor.", 39)
                             .AddOption("Alchemist.", 33)
                              .AddOption("Craftsman.", 39)
                              .AddOption("Hunter.", 39)
                              .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        data.AddText("3. In the early days, gunpowder was called?")
                            .AddOption("Yellow~powder.", 39)
                           .AddOption("Green~powder.", 39)
                            .AddOption("Black~powder.", 34)
                            .AddOption("Red~powder.", 39)
                            .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        data.AddText("4. At first, gunpowder was widely used in which area?")
                           .AddOption("War.", 39)
                          .AddOption("Circus.", 35)
                           .AddOption("Medicine.", 39)
                           .AddOption("Cultivation.", 39)
                           .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        data.AddText("5. Gunpowder was first made in which country?")
                             .AddOption("America.", 39)
                            .AddOption("Britain.", 39)
                             .AddOption("India.", 39)
                             .AddOption("China.", 36)
                             .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 39:
                    {
                        data.AddText("Sorry, but you`ll need to go study up a little more.")
                               .AddOption("Ok.", 255)
                               .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~can~report~back~to~Wealthy~Wan,~now.");
                        client.Inventory.Add(stream, 711450);
                        data.AddText("It seems these questions are easy for you. Great! Open your inventory, and I`ll give you some gunpowder!")
                                 .AddOption("Thanks.", 255)
                                 .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Alas... It`s all my fault. I should have paid more attention to Little Ben. If anything happpens to him, let me know. I`m so worried about him.")
                                        .AddOption("He~will~be~fine.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        data.AddText("Hey! Did you see my son, Ben? I was so absorbed in my work that I didn`t notice when he walked away. I hope he isn`t on someone`s roof, again. If you see Little Ben")
                            .AddText("~somewhere, please bring him home.")
                                       .AddOption("Okay.~I~will.", 21)
                                       .AddOption("Maybe~he~has~returned~home.", 255)
                                       .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1015, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711438, 5))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
                                          .AddOption("Let me check.", 255)
                                          .AddAvatar(47).FinalizeDialog();
                                    break;
                                }
                                if (HasItems(client, 711438, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    if (!client.Inventory.Remove(711438, 5, stream)) break;
                                    client.Inventory.AddItemWitchStack(720172, 0, 1, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~4,850,000~Exp,~3,000~silver~and~an~EXP~Ball~Scrap!");
                                    client.IncreaseExperience(stream, 4719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Just as I expected, these arms are really solid. I`ll forge them into a powerful weapon! Thanks!")
                                        .AddOption("You`re~welcome.", 255)
                                        .AddAvatar(30).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Go and kill some Birdmen to collect 5 arms for me. Of course, you`ll be handsomely rewarded!")
                                            .AddOption("Alright.", 255)
                                            .AddAvatar(47).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("Actually, I know how to make the weapon. The Birdman Arm is the key material. Could you please kill")
                            .AddText("~them and collect 5 arms for me?")
                                             .AddOption("Ok.", 11)
                                             .AddOption("Sorry,~but~I`m~busy.", 255)
                                             .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Kill~the~Birdmen~and~collect~their~arms.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ViceGeneral)]
        public static void ViceGeneral(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IslandGeneral, client.Player.Class, 1620);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ViceGeneral, client.Player.Class, 1621);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`m guarding the island. Was there a quest can I help you with?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Time~to~Unite.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hunting~L93~Hawks.", 10);

                        data.AddOption("Nope.", 255);
                        data.AddAvatar(66).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 2719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("It`s a good start. Thank you very much!")
                           .AddOption("Not~at~all.", 255)
                           .AddAvatar(66).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Have you killed 30 L93 Hawks?")
                            .AddOption("Not~yet!", 255)
                            .AddAvatar(66).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The first plan is to kill 30 L93 Hawks. I hope you can help me with that.")
                            .AddOption("No~problem.", 11)
                            .AddOption("It`s~dangerous.", 255)
                            .AddAvatar(66).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Go~kill~30~L93~Hawks!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (HasItems(client, 711455, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711455, 1, stream)) break;
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        data.AddText("The general asked you to deliver a letter? Great! Let me read it...")
                              .AddOption("Good~luck~for~you.", 255)
                              .AddAvatar(66).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.IslandGeneral)]
        public static void IslandGeneral(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BIViceCaptain, client.Player.Class, 1608);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IslandGeneral, client.Player.Class, 1611);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IslandGeneral, client.Player.Class, 1620);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`m guarding the island. If you find anything special, please report to me.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Military~Order.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hunting~HawKings.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Time~to~Unite.", 20);

                        data.AddOption("Bye.", 255);
                        data.AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(711455, 1))
                        {
                            data.AddText("It`s really urgent. Please inform the Vice General as soon as possible!")
                                .AddOption("Copy~that.", 255)
                                .AddAvatar(30).FinalizeDialog();
                            break;
                        }

                        data.AddText("I`ve sent the Vice General to guard the northeastern island. Now, it`s the time to unite to eliminate the Hawk Bosses.")
                            .AddText("~He`ll know the detailed plans when he reads this letter.")
                                      .AddOption("You~can~count~on~me.", 21)
                                      .AddOption("Sorry,~I`m~busy.", 255)
                                      .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(711455, 1))
                            client.Inventory.Add(stream, 711455);
                        client.CreateBoxDialog("Please~give~this~letter~to~the~Vice~General.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
                                          .AddOption("Let me check.", 255)
                                          .AddAvatar(30).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.AddItemWitchStack(1060024, 0, 1, stream);
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~2/3~EXP~Ball,~4,000~Silver~and~1~Bird~Island~Gate!");
                                    client.IncreaseExperience(stream, 2719000);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Thank you for your help! I bet they won`t be that bold from now on!")
                                        .AddOption("I~hope~so.", 255)
                                        .AddAvatar(30).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need your help. Go kill 50 HawKings! I will see the ground run red with their blood!")
                                .AddOption("Got~it.", 255)
                                .AddAvatar(30).FinalizeDialog();
                                break;
                            }

                            break;
                        }
                        data.AddText("We`re going to slay the HawKings and need your help.")
                            .AddText("~Please go kill 50 of them!")
                                .AddOption("I`m~in.", 11)
                                .AddOption("No,~I~won`t.", 255)
                                .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Go~kill~50~HawKings!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {
                        if (HasItems(client, 711451, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711451, 1, stream)) break;
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        data.AddText("Okay, I got it. Thanks for your information.")
                                .AddOption("You`re~welcome.", 255)
                                .AddAvatar(30).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BIViceCaptain)]
        public static void BIViceCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BIViceCaptain, client.Player.Class, 1591);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BIViceCaptain, client.Player.Class, 1598);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BIViceCaptain, client.Player.Class, 1608);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Welcome to the Bird Island. Are you coming for a quest?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~Birdman.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hunting~L88~Birdmen.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Military~Order.", 20);

                        data.AddOption("Nothing.", 255);
                        data.AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(711451, 1))
                        {
                            data.AddText("I just received the report that the HawKings were gathering and planning to attack the city.")
                        .AddText("~Please deliver this General Token to the island General and ask him to be on alert.")
                                     .AddOption("Okay.", 255)
                                     .AddAvatar(47).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I just received the report that the HawKings were gathering and planning to attack the city.")
                          .AddText("~Please deliver this General Token to the island General and ask him to be on alert.")
                                       .AddOption("I`m~on~my~way.", 21)
                                       .AddOption("Why~me?!", 255)
                                       .AddAvatar(47).FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(711451, 1) == false)
                            client.Inventory.Add(stream, 711451);
                        client.CreateBoxDialog("Hurry~to~take~this~General~Token~to~the~Island~General.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                      .AddOption("Let me check.", 255)
                                      .AddAvatar(47).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.AddItemWitchStack(1060024, 0, 1, stream);
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~1~Bird~Island~Gate!");
                                    client.IncreaseExperience(stream, 2719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Thank you very much! We`ve removed the nearby dangers!")
                                        .AddOption("My~pleasure.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Many villagers were killed by the L88 Birdmen! We have to do something. Please go kill 30 of them. We must protect the island!")
                                        .AddOption("I~won`t~disappoint~you.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Things have become worse... Many villagers were killed by the L88 Birdmen!")
                         .AddText("~We must stop them! Please go kill 30 of them. We must protect the island!")
                                      .AddOption("I~won`t~disappoint~you.", 11)
                                      .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Please~kill~30~L88~Birdmen.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 50))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 2719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("That`s very kind of you! Thank you very much!")
                                        .AddOption("No~problem.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Birdmen keep trying to attack the city to grab the treasures we have stored. Please go kill 50 Birdmen!")
                                        .AddOption("I`m~setting~off.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("You must have noticed that many Birdmen are gathering outside the city. I need you to kill 50 of them, to protect the city!")
                                        .AddOption("I~won`t~let~you~down.", 2)
                                        .AddOption("Why~me?!", 255)
                                        .AddAvatar(47).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Go~kill~50~Birdmen!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.XuFan)]
        public static void XuFan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1590);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuFan, client.Player.Class, 1593);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey! What can I do for you? Are you coming to repair items or maybe for a job?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Inform~the~Artisan.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Retrieve~Tool~Bag.", 10);


                        data.AddOption("Nothing.", 255);
                        data.AddAvatar(53).FinalizeDialog();
                        break;
                    }

                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711439, 1))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
                                        .AddOption("Let me check.", 255)
                                        .AddAvatar(53).FinalizeDialog();
                                    break;
                                }
                                if (HasItems(client, 711439, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711439, 1, stream)) break;
                                    client.Inventory.Add(stream, 720173);
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~a~Small~Joy~Stone!");
                                    client.IncreaseExperience(stream, 2719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Yes, that`s it! Thank you very much!")
                                      .AddOption("Never~mind.", 13)
                                      .AddAvatar(53).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("My tool bag was stolen by a Birdman, the other day. Can you please retrieve it for me?")
                          .AddOption("No~problem.", 255)
                          .AddAvatar(53).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("My tool bag was stolen by a Birdman, the other day. Can you please retrieve it for me?")
                           .AddOption("My~pleasure!", 11)
                           .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("Hey! What can I do for you? Are you coming to repair items or maybe for a job?")
                             .AddOption("Nothing.", 255)
                             .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 720173);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~1~Defense~Pot~(20m)!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Really?! Thank you for what you`ve done for the city!")
                              .AddOption("My~pleasure!", 255)
                              .AddAvatar(53).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Xi_er)]
        public static void Xi_er(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1609);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Xi_er, client.Player.Class, 1610);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I was begging my father for a long time before he agreed to let me play outside. I had such a good time these days!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("~Everyone should be so free!", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Old~Book.", 10);


                        data.AddOption("Nice.", 255);
                        data.AddAvatar(135).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711452, 1))
                            {
                                if (HasItems(client, 711452, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(711452, 1, stream)) break;
                                    client.Inventory.Add(stream, 720172);
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~2/3~EXP~Ball,~4,000~Silver~and~1~EXP~Ball~Scrap!");
                                    client.IncreaseExperience(stream, 2719000);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Great! That`s the Old Book! Thank you very much!")
                                        .AddText("~I`ll bring the book home soon!")
                                      .AddOption("Okay.", 255)
                                      .AddAvatar(135).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Thank you! The book has been shred into separate parts. Please kill the HawKings and try your best to retrieve all of them!")
                                  .AddOption("I~will.", 255)
                                  .AddAvatar(135).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("I`m wondering if you can do me a favor. I took an old book with me when I left home, unfortunately, it was shred into parts and taken by the HawKings!")
                           .AddText("~Please, you have to help me. It`s my father`s favorite book...")
                            .AddOption("I`ll~try~my~best.", 11)
                            .AddOption("That`s~your~fault.", 255)
                               .AddAvatar(135).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Kill~HawKings~to~retrieve~the~Old~Book.");
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(135).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 723340);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~5~Study~Points!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Yes, I`m Castellan`s daughter, Xi`er. My father is worried about me? I guess it`s my fault... I stayed gone a long time.")
                              .AddOption("Thank~god~you`re~fine.", 255)
                              .AddAvatar(135).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BICastellan)]
        public static void BICastellan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1660);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1661);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1590);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1609);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the castellan of Bird Island. Are you coming for any jobs we have available?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Head~to~Bird~Island.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Patrol~the~City.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Inform~the~Artisan.", 20);
                        else if (client.Player.Level > 90)
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Look~for~Daughter.", 30);

                        data.AddOption("Hail~to~you!", 255);
                        data.AddAvatar(18).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            data.AddText("I`m worried about my daughter, Xi`er. She went out 3 days ago, but hasn`t come back, yet!")
                           .AddText("~Can you please search the island for her?")
                                   .AddOption("I~will.", 255)
                                   .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        data.AddText("I`m worried about my daughter, Xi`er. She went out 3 days ago, but hasn`t come back, yet!")
                            .AddText("~Can you please search the island for her?")
                                    .AddOption("Sure.", 31)
                                    .AddOption("I`m~busy.", 255)
                                    .AddAvatar(18).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Please~serch~the~Island~for~Xi`er.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please ask Xu Fan to repair the bridge without any delays!")
                                     .AddOption("Got~it!", 255)
                                     .AddAvatar(18).FinalizeDialog();

                            break;
                        }
                        data.AddText("The bridge is badly damaged? That`s a big problem! We must do something, before any disasters happen.")
                            .AddText("~Can you inform the artisan, Xu Fan, about this? Please ask him to repair the bridge as soon as possible.")
                                      .AddOption("Sure!", 21)
                                      .AddOption("Later.", 255)
                                      .AddAvatar(18).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Please~head~to~visit~the~artisan,~Xu~Fan!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~20~minutes~of~EXP~and~2,000~Silver!");
                                    client.IncreaseExperience(stream, 2719000);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Really?! Thank you for what you`ve done for the city!")
                                      .AddOption("My~pleasure!", 20)
                                      .AddAvatar(18).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please patrol the city to check if there`s any potential danger!")
                                  .AddOption("I`m~leaving.", 255)
                                  .AddAvatar(18).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("We are always shorthanded. I`m wondering if you can patrol the city for me, to find out if there`s any potential danger.")
                               .AddOption("Glad~to~help.", 11)
                               .AddOption("None~of~my~business.", 255)
                               .AddAvatar(18).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Please~go~patrol~the~east~of~the~island.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 711437, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711437, 1, stream)) break;
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Desert City Vice Captain asked you to deliver this Official Letter? He must be very busy. Thanks for your help!")
                              .AddOption("Welcome.", 255)
                              .AddAvatar(18).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
    }
}
