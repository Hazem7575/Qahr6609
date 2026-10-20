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
        [NpcAttribute(NpcID.Lotus)]
        public static void Lotus(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Lotus, client.Player.Class, 1456);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This place used to be a thick forest. Years ago, there came a group of monsters that destroyed the forest and turned it into a barren desert.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Withered~Trees~1.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("The~Withered~Trees~4.", 10);

                        data.AddOption("Leave.", 255);
                        data.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Really? The Withered Trees are alive, now? Thank you a lot!")
                           .AddOption("My~pleasure.", 11).AddAvatar(159)
             .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(159).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                client.Inventory.Add(stream, 118069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                client.Inventory.Add(stream, 111069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                client.Inventory.Add(stream, 113029, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                client.Inventory.Add(stream, 112069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                client.Inventory.Add(stream, 143069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                client.Inventory.Add(stream, 144069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                client.Inventory.Add(stream, 148069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 114069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                client.Inventory.Add(stream, 101069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Super~Hat!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Taoist Yu is said to be wise man. Perhaps he has a way to save the dying trees.")
                            .AddOption("Fine.", 255).AddAvatar(159)
              .FinalizeDialog();

                            break;
                        }

                        data.AddText("The sand is about to swallow the whole area. People left here one after the other... Before I leave, I wish I could save the Withered Trees which that I planted last year.")
        .AddText("~Taoist Yu is said to be wise man. Perhaps he has a way to save the dying trees?")
                            .AddOption("I~won`t~let~you~down.", 2)
                .AddOption("Leave.", 255).AddAvatar(159)
                .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Head~to~ask~Taoist~Yu~for~solution.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DCViceCaptain)]
        public static void DCViceCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1450);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1451);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1460);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1465);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest(8447u/*SculptorHe in Questinfo.ini (spawned as 84477)*/, client.Player.Class, 1496);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1511);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1660);
            switch (Option)
            {

                case 0:
                    {

                        data.AddText("I have been guarding here for years. I miss my wife and kids...");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Inscription~1.", 40);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Elder~Jiang~4.", 50);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Head~to~Desert~City.", 1);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~Sand~Monsters.", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~L68~Sand~Monsters.", 20);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Speedy~Rescue.", 30);
                        if (client.Player.Level >= 87)
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Head~to~Bird~Island.", 60);

                        data.AddOption("See~you~then!", 255);
                        data.AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please deliver this Official Letter to the Bird Island Castellan!")
                                   .AddOption("I`m~on~my~way.", 255)
                                   .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("Hello, there! I need you to do me a favor. You see, I`m really busy these days...")
                            .AddText("~Can you head to the Bird Island and deliver this letter to the Castellan?")
                          .AddOption("You~can~count~on~me.", 61)
                        .AddOption("I`m~busy,~too!", 255)
                          .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(53).FinalizeDialog();

                            break;
                        }



                        client.CreateBoxDialog("Please~visit~the~Castellan~in~the~Bird~Island.");

                        client.Inventory.Add(stream, 711437);
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        break;
                    }
                case 50:
                    {
                        data.AddText("What? A straight hook and above the water? That`s interesting. I`ll report that to the Captain and I`m sure he will visit the strange old man.")
                           .AddOption("Bye.", 51)
                           .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 51:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {

                            data.AddText("Please make 3 more spaces in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(53).FinalizeDialog();

                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
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

                            client.Inventory.Add(stream, ItemID);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.SendSysMesage("You`ve~received~some~EXP,~2,000~Silver,~a~random~Gem~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 40:
                    {
                        data.AddText("Inscription? Hurry to deliver it to Sculptor He!")
                           .AddOption("I`m~on~my~way.", 41)
                           .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(53).FinalizeDialog();

                            break;
                        }
                        if (!client.Inventory.Contain(711408, 1))
                        {
                            client.Inventory.Add(stream, 711408);

                            client.CreateBoxDialog("Hurry~to~deliver~the~Inscription~to~Sculptor~He.");

                        }
                        break;
                    }

                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You came at the right time! A refugee camp in Desert was surrounded by a group of Hill Monsters.")
                      .AddText("~Go kill 20 of them, and report back to General ZhuGe!")
                       .AddOption("Okay.", 255)
                       .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("Gulp! Those villagers in the east of Desert haven`t recovered from the sandstorm, and another threat from the Hill Monsters came.")
                       .AddText("~Go kill 20 of the Hill Monsters, and report back to General ZhuGe!")
                            .AddOption("I~won`t~let~you~down.", 31)
                        .AddOption("I`m~busy.", 255)
                        .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~20~Hill~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 30))
                            {
                                data.AddText("Good job! Here`s your reward!")
                      .AddOption("Thanks.", 22)
                      .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you killed the Sand Monsters, yet? Go patrol outside the city gates. You`ll find plenty of them roaming around.")
                         .AddOption("Roger.", 255)
                         .AddAvatar(53).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Misfortune never comes alone. This time, I need you to kill 30 Level 68 Sand Monsters!")
                          .AddOption("Got~it.", 21)
                          .AddOption("Not~interested.", 255)
                          .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~30~Level~68~Sand~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("Thanks for what you`ve done!")
                      .AddOption("My~pleasure.", 12)
                      .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The cruel Sand Monsters often attack the travellers who pass through here. I need you to kill 50 of them!")
                         .AddOption("I`ll~set~out~right~away.", 255)
                         .AddAvatar(53).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The cruel Sand Monsters often attack the travellers. I need you to kill 50 of them!")
                          .AddOption("I~will.", 11)
                          .AddOption("Why~me?", 255)
                          .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~2,519,000~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 2519000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~50~Sand~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {
                        data.AddText("Captor Cooke sent you here? Thanks for delivering the letter.")
                            .AddOption("You`re~welcome.", 2)
                            .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
         .AddOption("Let me check.", 255)
         .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~1,500~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 1739000);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
            }

        }

        [NpcAttribute(NpcID.WitheredTree1)]
        public static void WitheredTree1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This is a withered tree.")
                           .AddOption("Use~Healing~Water.", 1).AddOption("Leave.", 255)
             .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {
                                client.CreateBoxDialog("You`ve~watered~this~tree~already.");
                            }
                            else
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    client.CreateBoxDialog("You`ve~watered~all~the~Withered~Trees.~Go~report~to~Lotus.");
                                else
                                    client.CreateBoxDialog("You`ve~watered~the~tree~with~the~Healing~Water.~Instantly,~it~becomes~green~and~full~of~life!");
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WitheredTree2)]
        public static void WitheredTree2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This is a withered tree.")
                           .AddOption("Use~Healing~Water.", 1).AddOption("Leave.", 255)
             .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                            {
                                client.CreateBoxDialog("You`ve~watered~this~tree~already.");
                            }
                            else
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    client.CreateBoxDialog("You`ve~watered~all~the~Withered~Trees.~Go~report~to~Lotus.");
                                else
                                    client.CreateBoxDialog("You`ve~watered~the~tree~with~the~Healing~Water.~Instantly,~it~becomes~green~and~full~of~life!");
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WitheredTree3)]
        public static void WitheredTree3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This is a withered tree.")
                           .AddOption("Use~Healing~Water.", 1).AddOption("Leave.", 255)
             .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                            {
                                client.CreateBoxDialog("You`ve~watered~this~tree~already.");
                            }
                            else
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    client.CreateBoxDialog("You`ve~watered~all~the~Withered~Trees.~Go~report~to~Lotus.");
                                else
                                    client.CreateBoxDialog("You`ve~watered~the~tree~with~the~Healing~Water.~Instantly,~it~becomes~green~and~full~of~life!");
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TaoistYu)]
        public static void TaoistYu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1461);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceGenera, client.Player.Class, 1454);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1455);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Lotus, client.Player.Class, 1456);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1457);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1458);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);
            switch (Option)
            {
                case 0:
                    {


                        data.AddText("The desert seems to be dead, without anything moving around. But I think treasures are always hidden under a barren land. That`s why I`m here! I`m looking for a talisman!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("The~Withered~Trees~1.", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Withered~Trees~2.", 40);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Withered~Trees~3.", 50);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Withered~Trees~4.", 60);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Collecting~Sand~Essence.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Hidden~Danger~3.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Hidden~Danger~4.", 20);
                        }
                        data.AddOption("Leave.", 255);
                        data.AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("OK. The Healing Water is done! You`ll need to water the 5 Withered Trees. After that, go tell Lotus about it.")
                         .AddOption("I~will.", 255)
                         .AddAvatar(67).FinalizeDialog();

                            break;
                        }
                        data.AddText("Here`s an Emerald Bottle. You`re going to head to the Moon Spring and collect some water.")
                          .AddOption("You~can~count~on~me.", 61)
                          .AddOption("Is~it~dangerous?", 255)
                          .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711340);
                        client.CreateBoxDialog("You`ve~received~the~Healing~Water!");
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711341, 1))
                            {
                                data.AddText("I`ll use the Moon Spring and Sand Blocker to make the Healing Water. Please wait a moment.")
                          .AddOption("Take~your~time.", 53)
                          .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("To save the withered trees, we also need to collect the Sand Blocker. It`s carried by the Sand Monsters. Would you like to do me the favor?")
                                 .AddOption("Sure.", 255)
                                 .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("To save the withered trees, we also need to collect the Sand Blocker. It`s carried by the Sand Monsters. Would you like to do me this favor?")
                               .AddOption("Sure.", 52)
                               .AddOption("I`ll~think~about~it.", 255)
                               .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 53:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {

                            client.Inventory.Add(stream, Database.ItemType.Meteor, 3);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.SendSysMesage("You've~received~some~EXP~3,000~Silver,~5~Panaceas,~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1))
                            {
                                data.AddText("Good job! I didn`t expect you would come back so soon.")
                              .AddOption("It`s~a~piece~of~cake.", 42)
                              .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("Here`s an Emerald Bottle. You`re going to head to the Moon Spring and collect some water.")
                       .AddText("~Beware of the Sand Monsters, as they`re drawn to the magic of the bottle and may try to steal it!")
                               .AddOption("OK.~I~will.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Here`s an Emerald Bottle. You`re going to need to head to the Moon Spring and collect some water.")
                            .AddText("~Beware of the Sand Monsters, as they`re drawn to the magic of the bottle and may try to steal it!")
                                    .AddOption("OK.~I~will.", 41)
                                    .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720938);
                        client.CreateBoxDialog("You`ve~received~an~Emerald~Bottle.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        data.AddText("Save the withered trees? Let me see...")
                                   .AddOption("Take~your~time.", 31)
                                   .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("To save the withered trees, we also need to collect the Sand Blocker. It`s carried by the Sand Monsters. Would you like to do me this favor?")
                                  .AddOption("Sure.", 52)
                                  .AddOption("I`ll~think~about~it.", 255)
                                  .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 52:
                    {
                        client.CreateBoxDialog("Go~fetch~a~Sand~Blocker~from~the~Sand~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(720173, 0, 2, stream);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~1,000~Silver~and~2~SmallJoyStone!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("Here`s an Emerald Bottle. You`re going to need to head to the Moon Spring and collect some water.")
                          .AddText("~Beware of the Sand Monsters, as they`re drawn to the magic of the bottle and may try to steal it!")
                                  .AddOption("OK.~I~will.", 41)
                                  .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("Here`s a Jade Flask. You`re going to collect some water from the source in the city and spread the water over the city gates.")
                            .AddText("~After that, the Sand Monsters will be unable to go through the gates or enter the city. Don`t forget to inform the DC Vice General of the news.")

                           .AddOption("I`ll~do~right~away.", 22)
                            .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720939, 1))
                            client.Inventory.Add(stream, 720939);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("You`ve~received~a~Jade~Flask.~Hurry~to~collect~some~water!");

                        break;
                    }
                case 10:
                    {
                        data.AddText("You were sent by the DC Vice General? Anything I can help you with?")
                              .AddOption("Check~this~Sandbag.", 11)
                              .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~1,000~Silver!");
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711343, 10))
                            {
                                data.AddText("You really impressed me! Thank you very much!")
                                  .AddOption("You`re~welcome.", 3)
                                  .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you ever heard of the Sand Essence? It`s the perfect material to forge a talisman. As far as I know, some Level 68 Sand Monsters are carrying the Sand Essence.")
                                    .AddText("~Can you go collect 10 for me?")
                                    .AddOption("Sure.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Have you ever heard of the Sand Essence? It`s a perfect material to forge a talisman. As far as I know, some Level 68 Sand Monsters are carrying the Sand Essence.")
                      .AddText("~Can you go collect 10 for me?")
                           .AddOption("Glad~to~help.", 2)
                       .AddOption("None~of~my~business.", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Hurry~to~go~collect~10~Sand~Essences!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);

                        break;

                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 711343, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711343, 10, stream)) break;
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.SendSysMesage("You've~received~some~EXP,~3,000~Silver,~an~EXP~Ball~Scrap~and~a~+1~Stone~(B)!", MsgMessage.ChatMode.System);

                            client.Inventory.Add(stream, 720172);
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }


                        break;
                    }
            }
        }
    }
}
