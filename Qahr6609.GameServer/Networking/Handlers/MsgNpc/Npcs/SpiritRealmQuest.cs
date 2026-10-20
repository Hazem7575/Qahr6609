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
        [NpcAttribute(NpcID.AuntPeach)]
        public static void AuntPeach(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1806);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntPeach, client.Player.Class, 1807);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("When we two parted, in silence and tears, my dear. The dew of the morning, stuck chilled. It felt like the")
                            .AddText("~warning of what I feel now. How should I greet thee? With silence and tears.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Do~you~know~the~bag?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                            client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~got~the~Blunt~Sword.", 4);


                        data.AddOption("Beautiful.", 255).AddAvatar(186).FinalizeDialog();


                        break;
                    }
                case 1:
                    {

                        data.AddText("Oh, this is the Peach Bag I made for my husband. Where did you get it? He left twenty years ago... No, he will come")
                            .AddText("~back...He promised...no...");
                        data.AddOption("Take~this~Bag.", 2)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("As you may have guessed by now, Obscure Warrior is my husband. Twenty years ago, he left for a duel, but he never")
                       .AddText("~came back ever since then. I made this Peach bag. Look, there is a note inside it.");
                        data.AddOption("Let~me~see.", 3)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Aunt~Peach~found~a~Note~in~the~Peach~Bag~and~gave~it~to~you.");
                        if (HasItems(client, 721842, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(721842, 1, stream)) break;
                            client.Player.Money += 6000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received EXP worth 1 EXP Ball and 6,000 Silver!", MsgMessage.ChatMode.System);

                        }
                        data.AddText("I will do what my husband told me to. As for the Divine Sword, oh, there is never such thing.")
                     .AddText("~My husband excels in using sword, and that Divine Sword they were talking about was only a normal sword.");
                        data.AddOption("Brilliant.", 4)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Thank you for bringing the message. Please take this as your reward.");

                        data.AddOption("My~pleasure.", 5)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("You know my feelings, right? Leave me alone, please... Oh, by the way, you can claim your reward from Taskmaster Chang.");

                        data.AddOption("OK,~I~will.", 6)
                            .AddOption("I`m~leaving.", 255)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Hurry~to~claim~your~reward~from~Taskmaster~Chang!");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ObscureWarrior)]
        public static void ObscureWarrior(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1801);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1802);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1803);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1804);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1805);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1806);

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m like a candle in the wind. But you are so young, so vigorous...I have engraved the essence of my blade skills on the four")
                            .AddText("~columns around you. You may click them to get some experience.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("How~long~have~you~been~here?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                            client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~got~the~Blunt~Sword.", 3);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                              client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~can~I~do~for~you?", 5);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                            client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~can~I~do~for~you?", 7);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                           client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~got~something~for~you.", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                         client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~got~something~for~you.", 11);


                        data.AddOption("Okay.", 255).AddAvatar(57).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (HasItems(client, 721841, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            if (!client.Inventory.Remove(721841, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("I thought Blade Han would keep his word...");
                        data.AddOption("Harvey`s~Letter,~for~you.", 11)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("So I have been waiting for nothing for 20 years? Sigh... This is my last wish. Will you take this Peach Bag to Aunt Peach on Bird Island (765,602)? She will tell you more about")
                            .AddText("~the Divine Sword.");
                        data.AddOption("Gotcha.", 12)
                            .AddOption("Later.", 255)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721842, 1) == false)
                            client.Inventory.Add(stream, 721842);

                        client.Teleport(503, 652, 1000);
                        client.CreateBoxDialog("Obscure~Warrior~took~out~a~Peach~Bag,~gave~it~to~you,~and~teleported~you~to~Desert~City.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 1:
                    {
                        if (HasItems(client, 721843, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(721843, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Haha, I have been here since 20 years ago. Someone finally came...");
                        data.AddOption("I`m~looking~for~you.", 2)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("One`s success relies on thousands of deaths. I have been looking forward for that duel for years... But it just never occurred...");
                        data.AddOption("Tell~me~about~Divine~Sword.", 3)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 200)
                                && client.Inventory.Contain(721845, 1))
                            {

                                if (HasItems(client, 721845, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    if (!client.Inventory.Remove(721845, 1, stream)) break;
                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                                }
                                data.AddText("My hero, you are as might as your fame suggests!");
                                data.AddOption("Thank~you.", 5)
                                   .AddAvatar(57).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hey you! Why don`t you kill 200 Blade Ghosts L83 and take back the Blunt Sword first?!");
                                data.AddOption("Okay.", 4)
                                    .AddOption("I~am~kinda~busy~right~now.", 255)
                                   .AddAvatar(57).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Divine Sword? Of course, what else can you want from me... Well, it`s in the hands of the Blade Ghosts in Desert (165,276),")
                            .AddText("~but turned into Blunt Sword. Kill 200 Blade Ghosts and take Blunt Sword back to me, maybe I will do something about it.");

                        data.AddOption("I`m~in!", 4)
                            .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(165, 271, 1000);
                        client.CreateBoxDialog("After~a~dazzling~flash,~you~arrived~at~Desert~(165,276).~It`s~about~time~to~conquer~those~stupid~Blade~Ghosts!");

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721846, 5))
                            {
                                data.AddText("Good job! I knew I could count on you!");
                                data.AddOption("No~problem.", 7)
                                   .AddAvatar(57).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Well, I need 5 Blade Ghost Fangs. It seems that you haven`t brought enough of them.");
                                data.AddOption("Send~me~back~to~Desert.", 6)
                                    .AddOption("I~am~kinda~busy~right~now.", 255)
                                   .AddAvatar(57).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("Now, I need 5 Blade Ghost Fangs to sharpen the Sword and restore")
                            .AddText("~its power. You will do me the favor?");
                        data.AddOption("Of~course.", 6)
                            .AddOption("Sorry,~I~can`t~help.", 255)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Teleport(165, 271, 1000);
                        client.CreateBoxDialog("You~have~been~teleported~to~the~Level~83~Blade~Ghosts.");
                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721846, 5))
                            {

                                if (HasItems(client, 721846, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    if (!client.Inventory.Remove(721846, 5, stream)) break;
                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                                }
                            }
                        }
                        data.AddText("I see a hero in you, my child, just like me when I was young... so ambitious, so bold... Oh, if only I could defeat Blade Han!");
                        data.AddOption("Who~is~it?", 8)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Blade Han is famous for his `Fervent Blade`. We had a rendezvous here, to set a duel. I have been waiting for years...")
                            .AddText("~Will you find him for me?");
                        data.AddOption("I~should~visit~Harvey.", 9)
                            .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        client.CreateBoxDialog("Weird,~Blade~Han~looked~just~like~Harvey~in~Desert~City~(495,579).~Find~him~to~learn~more.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Teleport(503, 652, 1000);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn1)]
        public static void StoneColumn1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { Intentions = 4, MissionId = 11 }, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(11, 1) || client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You have already learnt these inscriptions. You can`t learn them again.");
                            data.AddOption("All~right~then.", 255).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 11, 1);
                            data.AddText("You obtained experience worth 1/12 of an EXP Ball after reading those inscriptions on the Stone Columns.");
                            data.FinalizeDialog();
                            client.GainExpBall(100, false);
                            if (client.Player.QuestGUI.CheckObjectives(11, 1, 1, 1, 1))
                                client.Player.QuestGUI.FinishQuest(11);

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn2)]
        public static void StoneColumn2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { Intentions = 4, MissionId = 11 }, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(11, 0, 1) || client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You have already learnt these inscriptions. You can`t learn them again.");
                            data.AddOption("All~right~then.", 255).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 11, 0, 1);
                            data.AddText("You obtained experience worth 1/12 of an EXP Ball after reading those inscriptions on the Stone Columns.");
                            data.FinalizeDialog();
                            client.GainExpBall(100, false);
                            if (client.Player.QuestGUI.CheckObjectives(11, 1, 1, 1, 1))
                                client.Player.QuestGUI.FinishQuest(11);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn3)]
        public static void StoneColumn3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { Intentions = 4, MissionId = 11 }, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(11, 0, 0, 1) || client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You have already learnt these inscriptions. You can`t learn them again.");
                            data.AddOption("All~right~then.", 255).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 11, 0, 0, 1);
                            data.AddText("You obtained experience worth 1/12 of an EXP Ball after reading those inscriptions on the Stone Columns.");
                            data.FinalizeDialog();
                            client.GainExpBall(100, false);
                            if (client.Player.QuestGUI.CheckObjectives(11, 1, 1, 1, 1))
                                client.Player.QuestGUI.FinishQuest(11);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn4)]
        public static void StoneColumn4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { Intentions = 4, MissionId = 11 }, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(11, 0, 0, 0, 1) || client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You have already learnt these inscriptions. You can`t learn them again.");
                            data.AddOption("All~right~then.", 255).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 11, 0, 0, 0, 1);
                            data.AddText("You obtained experience worth 1/12 of an EXP Ball after reading those inscriptions on the Stone Columns.");
                            data.FinalizeDialog();
                            client.GainExpBall(100, false);
                            if (client.Player.QuestGUI.CheckObjectives(11, 1, 1, 1, 1))
                                client.Player.QuestGUI.FinishQuest(11);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn)]
        public static void StoneColumn(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1801);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntPeach, client.Player.Class, 1807);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You need to post the Stone Spell onto the Stone Column to see what will happen.");

                        if ((client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Place~the~spell.", 1);



                        data.AddOption("Not~now.", 255).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.CreateBoxDialog("You~opened~your~eyes~and~find~yourself~in~the~Crypt.");
                        client.Teleport(52, 57, 1801);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Harvey)]
        public static void Harvey(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1800);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1801);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1804);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1805);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Have you ever seen an isis oasis? That is a sea of green, interspersed with towering, rock mountains and surrounded")
                           .AddText("~by the formidable Desert. Maybe only the legendary Obscure Warrior can survive the daunting Desert.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Who`s~Obscure~Warrior?", 2);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("You~know~Blade~Han?", 6);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Who`s~Obscure~Warrior?", 7);


                        data.AddOption("You~bet!", 255)
                        .AddAvatar(64).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Well, yes, that is my father. He was famous for wielding blades. Twenty years ago")
                            .AddText("~he went to the Desert for a duel. But he didn`t find the Warrior as there was a horrific sand storm. Pity.");
                        data.AddOption("Sorry~to~hear~that.", 7)
                           .AddAvatar(64).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Have you ever seen an isis oasis? That is a sea of green, interspersed with towering, rock mountains and surrounded")
                     .AddText("~by the formidable Desert. Maybe only the legendary Obscure Warrior can survive the daunting Desert.");
                            data.AddOption("You~bet!", 255)
                               .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        data.AddText("Because of this unfinished dream, my father shelved his blade and took to drinking... So I began to search for the Divine Sword")
                          .AddText("~to realize his dream. Please take this letter to the Ghost of Obscure Warrior and let him rest in peace.");
                        data.AddOption("Okay.~Gimme.", 8)
                            .AddOption("Later.", 255)
                           .AddAvatar(64).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721841, 1) == false)
                            client.Inventory.Add(stream, 721841);
                        client.CreateBoxDialog("Harvey~gave~the~letter~to~you.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("I have kept this secret for years...");
                        data.AddOption("Can~you~tell~me?", 2)
                           .AddAvatar(64).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            data.AddText("Twenty years ago, a brave warrior appeared, helping the poor while punishing the bad. No one knows where he was from. So they")
                      .AddText("~called him Obscure Warrior. Go find Blade Ghost L83 (165,276) to find out more about him, and get his Stone Spell from them.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(64).FinalizeDialog();

                            break;
                        }
                        data.AddText("People won`t forget those people who have helped them. A pure heart shouldn`t be forgotten. You heard of the Obscure Warrior before,")
                            .AddText("~right? Do you want to know more?");
                        data.AddOption("All~ears.", 3)
                           .AddAvatar(64).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Rumor is spread that Obscure Warrior went for a duel in the Desert but never came back. He had an enormously powerful weapon, the")
                          .AddText("~Divine Sword. I learnt that anyone who could find him would get the Sword. But the Desert... Hey, are you interested?");
                        data.AddOption("Risk~nothing,~gain~nothing.", 4)
                            .AddOption("Not~really.", 255)
                           .AddAvatar(64).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Indeed! I heard the Level 83 Blade Ghosts have a Stone Spell that was once possessed by Obscure")
                         .AddText("~Warrior. Find them at (165,276) and get the spell first. Goodbye for now.");
                        data.AddOption("Okay.", 5)
                           .AddAvatar(64).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.CreateBoxDialog("Hunt~Level~83~Blade~Ghosts~(165,276)~to~get~the~Stone~Spell!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Carolyn)]
        public static void Carolyn(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.FarmerLynn, client.Player.Class, 1798);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Carolyn, client.Player.Class, 1799);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, there! My father asked you to persuade me to marry, right?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Yes.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Yup!", 4);

                        data.AddOption("I`ll~leave~you~alone.", 255)
                        .AddAvatar(109).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("Well, answer my question, first!");
                        data.AddOption("Okay!", 2)
                            .AddOption("Not~interested.", 255)
                           .AddAvatar(109).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Listen carefully. Which kind of Jade can a Leaf Stone and a Maple Stone be fused into?");
                        data.AddOption("Rune~Jade.", 3)
                           .AddOption("Moon~Jade.", 10)
                           .AddOption("Sun~Jade.", 10)
                           .AddOption("Ocean~Jade.", 10)
                           .AddAvatar(109).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Sorry, but you`ll need to go study up a little more.")
                        .AddOption("Ok.", 255)
                        .AddAvatar(109).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Bingo! Wow, you are so brilliant! Um, I`m starting to like you... Could you tell me...would you...marry me?")
                            .AddOption("I~like~you,~too.~But~sorry...", 4)
                            .AddAvatar(109).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~managed~to~persuade~Carolyn!~You`ve~received~3,000~Silver~and~EXP~worth~1/2~EXP~Ball!");
                        }



                        data.AddText("I`m so happy to become your friend. Now go claim your reward from Taskmaster Chang!")
                          .AddOption("See~ya.", 5)
                          .AddAvatar(109).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.CreateBoxDialog("You~managed~to~persuade~Carolyn!~Go~claim~your~reward~from~Taskmaster~Chang.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.FarmerLynn)]
        public static void FarmerLynn(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1795);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FarmerLynn, client.Player.Class, 1796);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FarmerLynn, client.Player.Class, 1797);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FarmerLynn, client.Player.Class, 1798);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Ah, a hero! Please stop and have a talk with me! You see, my days are slowly ending... I`ve been working on my")
                            .AddText("~farm for my whole life, and to be frank, I`m still quite poor. Wait, let me ask you something first. Have you married, yet?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~about~the~house?", 2);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~about~the~house?", 5);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~about~the~house?", 7);

                        data.AddOption("Um.~What`s~up?", 255)
                        .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("This problem is killing me. I have to look for someone to help me make sure she is taken care of.");
                        data.AddOption("What`s~the~problem?", 2)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(710581, 10))
                            {
                                data.AddText("Wow, you made it! This house will be done in no time! Thank you!");
                                data.AddOption("You`re~welcome.", 5)
                                   .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need 10 hunks of Marble to build a house for my daughter. You can get them from the Rock Monsters. It won`t be hard for you, right?");
                                data.AddOption("Yup.", 255)
                                   .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("You`ll help me?! That`s the best news I`ve heard today! First, she needs a house. It won`t be difficult for you, right?");
                        data.AddOption("Er?~Buy~one~for~you?", 3)
                            .AddOption("No~way.", 255)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("No, no. I`m not a greedy man. You just help me get some marble. Let me see...1, 2, 3...um, I need 10 slabs of marble. You can get them")
                         .AddText("~from the Rock Monsters. Just take your time and make sure they`re good quality!");
                        data.AddOption("Wait~a~second.", 4)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.CreateBoxDialog("Hurry~to~hunt~the~Rock~Monsters~to~get~10~hunks~of~Marble.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710581, 10))
                        {
                            if (HasItems(client, 710581, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(710581, 10, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You`ve~received~100~Silver!", MsgMessage.ChatMode.System);
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(710582, 1) && client.Inventory.Contain(710583, 1)
                                && client.Inventory.Contain(710584, 1))
                            {
                                data.AddText("Wow, unbelievable! You got them? Thank you so much, hero!");
                                data.AddOption("You`re~welcome.", 7)
                                   .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Yeah, jade! My daughter likes the Rune Jade, Moon Jade and Sun Jade very much. Each of the jades needs to be fused by 2 special stones.")
                                    .AddText("~A Leaf Stone and a Maple Stone can be fused into a Rune Jade. An Ocean Stone and a Leaf Stone can be fused into a Sun Jade.")
                                    .AddText("~The Moon Jade needs to be fused by a Maple Stone and an Ocean Stone. You can get these special stones from Level 78 Rock Monsters. Understand?");
                                data.AddOption("Yes.", 255)
                                   .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I still need some decorations for the house... Just imagine, if you lived in a big, empty")
                            .AddText("~stone house... Oh, it`s far too dull for a girl of her tastes.");
                        data.AddOption("I`d~like~to~help.", 6)
                            .AddOption("Decorations?", 255)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.CreateBoxDialog("Hurry~and~hunt~the~Level~78~Rock~Monsters~for~the~stones~to~make~the~3~jades!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(710582, 1) && client.Inventory.Contain(710583, 1)
                                  && client.Inventory.Contain(710584, 1))
                        {
                            if (HasItems(client, 710582, 1) && HasItems(client, 710583, 1) && HasItems(client, 710584, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                if (!client.Inventory.Remove(710582, 1, stream)) break;
                                if (!client.Inventory.Remove(710583, 1, stream)) break;
                                if (!client.Inventory.Remove(710584, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You`ve~received~100~Silver!", MsgMessage.ChatMode.System);
                            }
                        }
                        data.AddText("My daughter is 30 years old and wants to be single forever! Please try to persuade her to change her mind!")
                            .AddText("~But since she`s really childish, unless you can correctly answer her questions she won`t listen to you.");
                        data.AddOption("I`ll~try~my~best.", 9)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        client.CreateBoxDialog("You~promised~to~try~and~persuade~Farmer~Lynn`s~daughter~to~marry.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.TaoistShine)]
        public static void TaoistShine(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.JerkWang, client.Player.Class, 1790);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistShine, client.Player.Class, 1791);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistShine, client.Player.Class, 1792);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Reverence for nature and ancestor spirits is important. Never fool around with celestials.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("You~knew~Mo~Mo?", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Trust~me.~I`ll~find~Mo~Mo.", 2);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("How~to~get~there?", 10);

                        data.AddOption("Any~ideas?", 255)
                        .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("You came for Mo Mo? I`ve heard the story. It`s weird...");
                        data.AddOption("Any~ideas?", 2)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721712, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {

                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                                }
                                data.AddText("This Powder is used to bewilder little kids and kidnap them to Ghost City.");
                                data.AddOption("Okay.", 10)
                                   .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hey you! You already have one!");
                                data.AddOption("Oh,~yeah.", 255)
                                   .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I`m afraid he has entered Ghost City, by mistake. Here I have a Charm, with it, you can enter there, too.");
                        data.AddOption("Gimme.", 3)
                            .AddOption("I`m~not~going~there.", 255)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Take this Powder and use it where Mo Mo was seen missing, and you will be able to enter the city.");
                        data.AddOption("Okay.", 11)
                            .AddOption("I`ll~go~there~later.", 255)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721713, 1))
                            client.Inventory.Add(stream, 721713);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Try~your~best~to~rescue~Mo~Mo.");
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721713, 1))
                            client.Inventory.Add(stream, 721713);

                        client.CreateBoxDialog("You~obtained~a~Charm~from~Taoist~Shine.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Ghost_SweetyPuddy)]
        public static void Ghost_SweetyPuddy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("This place is great!");
                        data.AddOption("You~bet.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyPuddy)]
        public static void Dark_SweetyPuddy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("This place is great!");
                        data.AddOption("You~bet.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyLily)]
        public static void Dark_SweetyLily(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Come and play with me!");
                        data.AddOption("Nope.", 255)
                           .AddAvatar(143).FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Ghost_SweetyLily)]
        public static void Ghost_SweetyLily(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Come and play with me!");
                        data.AddOption("Nope.", 255)
                           .AddAvatar(143).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyMindy)]
        public static void Dark_SweetyMindy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I like here.");
                        data.AddOption("Whatever.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_SweetyMindy)]
        public static void Ghost_SweetyMindy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I like here.");
                        data.AddOption("Whatever.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyDay)]
        public static void Dark_SweetyDay(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("It rocks here!");
                        data.AddOption("It~sucks.", 255)
                           .AddAvatar(143).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_SweetyDay)]
        public static void Ghost_SweetyDay(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("It rocks here!");
                        data.AddOption("It~sucks.", 255)
                           .AddAvatar(143).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_SweetyCuty)]
        public static void Ghost_SweetyCuty(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Best playground ever!");
                        data.AddOption("It`s~dangerous.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyCuty)]
        public static void Dark_SweetyCuty(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Best playground ever!");
                        data.AddOption("It`s~dangerous.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Ghost_FiendAltar1)]
        public static void Ghost_FiendAltar1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721712, 1))
                        {
                            client.CreateBoxDialog("You`ve~already~got~the~Powder~in~inventory.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ve~found~some~powder.~Hurry~to~deliver~it~to~Taoist~Shine.");
                            client.Inventory.Add(stream, 721712);
                        }
                        client.Teleport(274, 299, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_FiendAltar2)]
        public static void Ghost_FiendAltar2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721712, 1))
                        {
                            client.CreateBoxDialog("You`ve~already~got~the~Powder~in~inventory.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ve~found~some~powder.~Hurry~to~deliver~it~to~Taoist~Shine.");
                            client.Inventory.Add(stream, 721712);
                        }
                        client.Teleport(274, 299, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_FiendAltar3)]
        public static void Ghost_FiendAltar3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721712, 1))
                        {
                            client.CreateBoxDialog("You`ve~already~got~the~Powder~in~inventory.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ve~found~some~powder.~Hurry~to~deliver~it~to~Taoist~Shine.");
                            client.Inventory.Add(stream, 721712);
                        }
                        client.Teleport(274, 299, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_FiendAltar4)]
        public static void Ghost_FiendAltar4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721712, 1))
                        {
                            client.CreateBoxDialog("You`ve~already~got~the~Powder~in~inventory.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ve~found~some~powder.~Hurry~to~deliver~it~to~Taoist~Shine.");
                            client.Inventory.Add(stream, 721712);
                        }
                        client.Teleport(274, 299, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_MoMo)]
        public static void Ghost_MoMo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Here is way better than home. I could play all day here. Mom and Dad would only scold me for anything I do.");
                        data.AddOption("You~should~go~home.", 1)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("You will find some clues near Fete Pillar (162,217).");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Dark_MoMo)]
        public static void Dark_MoMo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistShine, client.Player.Class, 1792);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Dark_MoMo, client.Player.Class, 1793);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I love here! It`s fun! I`m not going home!");
                        data.AddOption("Your~mom~is~worried~about~you!", 1)
                            .AddOption("If~you~like.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721714, 1))
                        {
                            data.AddText("It`s dark. I`m scared.");
                            data.AddOption("Take~the~Bezoar.", 2)
                               .AddAvatar(36).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You still don`t get the Bezoar. Go get it.");
                            data.AddOption("I`m~on~my~way.", 255)
                               .AddAvatar(36).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("I love here! It`s fun! I`m not going home!");
                        data.AddOption("Go~with~me.", 3)
                            .AddOption("If~you~like.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        data.AddText("Thanks for bringing me home! My mother must prepare a reward for you! Go talk to her!");
                        data.AddOption("I~will.", 4)
                            .AddOption("If~you~like.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Teleport(302, 280, 1002);
                        client.CreateBoxDialog("Mo~Mo~took~the~Bezoar~and~returned~home.~Hurry~to~find~Aunt~Zhang~to~claim~your~reward!");
                        client.Inventory.Remove(721714, 1, stream);
                        client.Inventory.Remove(721712, 1, stream);
                        client.Inventory.Remove(721713, 1, stream);
                        break;
                    }
            }
        }
    }
}
