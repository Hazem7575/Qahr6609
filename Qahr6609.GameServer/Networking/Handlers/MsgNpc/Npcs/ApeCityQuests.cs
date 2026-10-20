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
        [NpcAttribute(NpcID.HarmonyTaoist)]
        public static void HarmonyTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1335);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1343);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1336);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1337);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1338);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1339);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1340);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1341);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1342);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You are not a normal person! I can tell it from your face!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Back~Home(2).", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Back~Home(3).", 30);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Back~Home(4).", 40);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Back~Home(5).", 60);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Pick~a~Fight(1).", 70);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Pick~a~Fight(2).", 80);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Pick~a~Fight(3).", 90);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Eradicate~Snakeman.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snakeman~L63.", 10);
                        data.AddOption("Continue Quests.", 50).AddOption("See~you~then!", 255);

                        data.AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 90:
                    {
                        data.AddText("I have cast my magic on the Holy Bead and the Heresy Snake Gall! Use the Holy Bead to transform into a Snakeman and challenge the Heresy Leader. Use the Heresy Snake Gall to transform into a Heresy Snakeman to challenge the Snakeman Leader.")
                                 .AddText("~By doing this, we can arose their hatred against each other and see both sides jump on each other without getting involved in it!")
                     .AddOption("No~problem!", 91)
                  .AddOption("Sorry,~I~am~not~interested.", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 91:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 441, 451, 30105);
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        break;
                    }
                case 80:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720857, 1)
                                && client.Inventory.Contain(720858, 1))
                            {
                                data.AddText("Good. I will cast my magic spell on the Holy Bead and the Heresy Snake Gall! Then you can use it to transform into a Snakeman or a Heresy Snakeman to continue our plan.")
                      .AddOption("Great!", 82)
                      .AddOption("See~you~then!", 255)
                           .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Get a Holy Bead from the Snakeman and a Heresy Snake Gall from the Heresy Snakeman. We need them to pick a fight between the two snake races.")
                      .AddOption("Okay!", 255)
                           .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Listen! We have a plan to pick a fight between the two snake races. Get a Holy Bead from the Snakeman and a Heresy Snake Gall from the Heresy Snakeman. These two items are the key to carry out the plan!")
                     .AddOption("No~problem!", 81)
                  .AddOption("Sorry,~I~am~not~interested.", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 82:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.SendSysMesage("You received 1,205,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1205000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 81:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest8, 0);
                        break;
                    }
                case 70:
                    {
                        data.AddText("I have discussed this with Captor Cooke. This will be a big plan!")
                         .AddOption("Tell~me~what!", 71)
                      .AddOption("Sorry,~I~am~not~interested.", 255)
                           .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 71:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.SendSysMesage("You received 301,000 EXP, and 500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 301000);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Teach Captor Cooke Cloud Riding! We have to help him send the criminals back to Ape City to be punished by the courts!")
                        .AddOption("Okay!", 255)
                             .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Since you have learned the secret of Cloud Riding, you should go teach Captor Cooke how to ride the clouds. We need to help him send the criminals back to Ape City to be punished by the courts!")
                           .AddOption("No~problem!", 61)
                        .AddOption("See~you~then!", 255)
                             .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 61:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1))
                            {
                                data.AddText("You are not a normal person! I knew it at the time when I saw you! See, you mastered the Cloud Riding!")
                  .AddOption("Thank~you.", 42)
                  .AddOption("See~you~then!", 255)
                       .AddAvatar(67).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Go to the cliff closest to Ape City, and chant `Kuh Limb Mah, Kuh Limb Mah` to use the skill! Don`t forget to put away your weapons, first!")
                      .AddOption("Okay.", 255)
                           .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("According to the Scroll, you need a pure mind to fly over the cliff. Now, put down your weapon. Go to the cliff closest to Ape City, and chant `Kuh Limb Mah, Kuh Limb Mah` to use the skill!")
                          .AddOption("No~problem!", 41)
                       .AddOption("See~you~then!", 255)
                            .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 42:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                             .AddOption("Let me check.", 255)
                             .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.SendSysMesage("You received 1,187,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1870000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 41:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Contain(720856, 1))
                        {
                            data.AddText("That is it! The Cloud Riding Scroll. It`s written in the Snakemen`s language... Give it to me. I will translate it to you!")
                 .AddOption("Okay!", 32)
                 .AddOption("See~you~then!", 255)
                 .AddAvatar(67).FinalizeDialog();
                            break;
                        }


                        data.AddText("Snakeman has a secret skill of Cloud Riding. As the name suggested, you can raid the cloud flying over the cliffs. Get a Cloud Riding Scroll from the Snakeman and you will know how to fly!")
                       .AddOption("No~problem!", 31)
                       .AddOption("See~you~then!", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 32:
                    {
                        if (!client.Inventory.Remove(720856, 1, stream)) break;
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.SendSysMesage("You received 1,290,000 EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1290000);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 20:
                    {
                        data.AddText("A shortcut to Ape City? There is one. Fly over the cliff, and there you go! But you`ll need the Cloud Riding Skill to help you!")
                         .AddOption("What~is~it?", 22)
                         .AddOption("See~you~then!", 255)
                         .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You received 890,000 EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 890000);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 50:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 211, 445, 8298);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("I can`t believe it! You made it! Here are your rewards!")
                          .AddOption("Thank~you!", 12)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 50 Snakemen L63 for me! Be careful! They are very aggressive!")
                              .AddOption("Okay!", 255)
                              .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Snakeman L63 is the most aggressive snake race! I want you to slay 50 Snakemen L63 for me!")
                                 .AddOption("No~problem!", 11)
                                 .AddOption("Sorry,~not~today!", 255)
                                 .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You received 1,223,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1223000);
                            client.Player.Money += 2000;
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
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 60))
                            {
                                data.AddText("Thank you very much for slaying the Snakeman for me. Here are your rewards!")
                          .AddOption("Thank~you!", 3)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 60 Snakeman to return peace to Ape City!")
                              .AddOption("Okay!", 255)
                              .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Snakeman is a bane on Ape City! I want you to slay 60 Snakeman and return peace to Ape City!")
                               .AddOption("No~problem!", 2)
                               .AddOption("Sorry,~not~today!", 255)
                               .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 1,170,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1170000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MoonMaiden)]
        public static void MoonMaiden(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BreezePupil, client.Player.Class, 1331);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MoonMaiden, client.Player.Class, 1332);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MoonMaiden, client.Player.Class, 1333);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MoonMaiden, client.Player.Class, 1334);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Poisonous snakes are dangerous to people!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Snakeman~Leader(1).", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snakeman~Leader(2).", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snake~Gall.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snake~Fang.", 30);

                        data.AddOption("Continue Quests.", 50).AddOption("See~you~then!", 255);

                        data.AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 50:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 265, 429, 8297);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720855, 5))
                            {
                                data.AddText("Thank you very much for punishing the Snakeman. Here are your rewards!")
                                   .AddOption("Thank~you!", 32)
                                   .AddOption("See~you~then!", 255)
                                   .AddAvatar(175).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Go slay the Snakeman and collect 5 Snake Fangs for me! This is will be a warning!")
                                    .AddOption("Okay!", 255)
                                    .AddAvatar(175).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Many villagers were wounded by the Snakemen. They deserve to be punished! Go slay the Snakemen and collect 5 Snake Fangs for me! This is will be a warning to them!")
                    .AddOption("No~problem!", 31)
                    .AddOption("Sorry,~not~today!", 255)
                    .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        if (HasItems(client, 720855, 5) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(720855, 5, stream)) break;
                            client.Inventory.AddItemWitchStack(723340, 0, 1, stream);
                            client.SendSysMesage("You received 1,223,000 points of EXP, 2 Diligence Book, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1223000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

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
                            if (client.Inventory.Contain(720854, 10))
                            {
                                data.AddText("Thank you very much for the Snake Gall. Here are your rewards!")
                             .AddOption("You~are~welcome.", 22)
                             .AddOption("See~you~then!", 255)
                             .AddAvatar(175).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Snake Gall of the Snakeman is a rare medicine, but a necessary ingredient in making the drug for snakebites. Collect 10 Snake Galls for me from the Snakeman!")
                             .AddOption("Okay!", 255)
                             .AddAvatar(175).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Snake Gall of the Snakeman is a rare medicine, but a necessary ingredient in making the drug for snakebites. Collect 10 Snake Galls for me! I can use them to save lives.")
                          .AddOption("No~problem!", 21)
                          .AddOption("Sorry,~not~today!", 255)
                          .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (HasItems(client, 720854, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(720854, 10, stream)) break;
                            client.Inventory.AddItemWitchStack(720172, 0, 2, stream);
                            client.SendSysMesage("You received 1,170,000 points of EXP, 2 EXPBallScrap, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1170000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
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
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                        {
                            data.AddText("Well done! The Snakeman Leader must have learnt a lesson this time!")
                             .AddOption("Thank~you.", 12)
                             .AddOption("See~you~then!", 255)
                             .AddAvatar(175).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Snakeman Leader is an enthusiast in alchemy who likes to eat the bolus produced by the alchemists to strength his power! I have some Catholicon, it some kind of drug for snakebites which is very effective in dealing with the snakes.")
                      .AddText("~Take it to the Snakeman Leader persuade him to eat it! Then the Breeze Pupil will have his revenge!")
                            .AddOption("No~problem!", 11)
                            .AddOption("Sorry,~not~today!", 255)
                            .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("Let me check.", 255)
                              .AddAvatar(175).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.SendSysMesage("You received 702000 EXP, a Meteor and 1200 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 702000);
                            client.Player.Money += 1200;
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
                        data.AddText("You want me to punish the Snakeman Leader? that is easy!")
                                 .AddOption("Tell~me~how?", 2)
                                 .AddOption("Just~kidding!", 255)
                                 .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.SendSysMesage("You received 292,000 points of EXP, and 5,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 292000);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TempestTaoist)]
        public static void TempestTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArdenteTaoist, client.Player.Class, 1326);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TempestTaoist, client.Player.Class, 1327);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TempestTaoist, client.Player.Class, 1328);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I want to know the limits of man. That is the purpose of my practice, and my goal in life.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Medicine~Supply.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Thunder~Stone.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Rage~Stone.", 20);

                        data.AddOption("Open another Quest.", 100).AddOption("See~you~then!", 255);

                        data.AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 100:
                    {

                        client.Player.QuestGUI.SendAutoPatcher(1020, 356, 315, 8295);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720850, 3))
                            {
                                data.AddText("I will do some research on these Rage Stones. Thank you very much for your help!")
                               .AddOption("You~are~welcome!", 22)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {


                                data.AddText("Collect 3 Rage Stones for me from the Thunder Ape L58.")
                                             .AddOption("Okay!", 255)
                                             .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Rage Stones of the Thunder Apes L58 can really boost their strength! I hope you can collect 3 Rage Stones for me.")
                                     .AddOption("No~problem!", 21)
                                     .AddOption("See~you~then!", 255)
                                     .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.Remove(720850, 3, stream)) break;
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.SendSysMesage("You received 1,134,000 points of EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1134000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
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

                            if (client.Inventory.Contain(711353, 3))
                            {
                                data.AddText("Thank you very much for your help! Here are your rewards!")
                                      .AddOption("Thank~you!", 12)
                                      .AddOption("See~you~then!", 255)
                                      .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {


                                data.AddText("Please collect 3 Thunder Stones for me from the Thunder Apes. I need to do a research on them!")
                                             .AddOption("Okay!", 255)
                                             .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Thunder Apes usually carry the Thunder Stones. I guess these strange stones give them special power! Will you collect 3 Thunder Stones for me, for further research?")
                                .AddOption("No~problem!", 11)
                                           .AddOption("Sorry,~not~today!", 255)
                                           .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.Remove(711353, 3, stream)) break;
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, 720172);
                            client.SendSysMesage("You received 1,250,000 points of EXP, 1 EXP Ball Scrap, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1250000);
                            client.Player.Money += 1500;
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
                        data.AddText("Thank you very much for your medicine! We can use them to enter the lair of the apes and find out the way to deal with them.")
                             .AddOption("You~are~welcome!", 2)
                                        .AddOption("Sorry,~not~today!", 255)
                                        .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(1000030, 1) && client.Inventory.Contain(1001020, 1))
                        {
                            if (!client.Inventory.Remove(1001020, 1, stream)) break;
                            if (!client.Inventory.Remove(1000030, 1, stream)) break;
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.SendSysMesage("You received 850,000 points of EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                client.IncreaseExperience(stream, 850000);
                                client.Player.Money += 1500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            }
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ArdenteTaoist)]
        public static void ArdenteTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArdenteTaoist, client.Player.Class, 1324);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArdenteTaoist, client.Player.Class, 1325);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArdenteTaoist, client.Player.Class, 1326);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The goal of the Taoist practice is to achieve harmony in both the mental and physical planes.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Thunder~Ape.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Thunder~Ape~L58.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Medicine~Supply.", 20);
                        data.AddOption("See~you~then!", 255);

                        data.AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        data.AddText("My brother, the Tempest Taoist, and I decided to enter the lair of the Giant Apes, but we can`t afford the necessary medicine. Please, buy 2 Amrita and 2 Pearl Ointments for my brother and I. I will give you all I have in return.")
                                 .AddOption("No~problem!", 21)
                                            .AddOption("Sorry,~not~today!", 255)
                                            .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 2);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                        {
                            data.AddText("Thank you very much for wiping out the Thunder Apes L58 for me! Here are your rewards!")
                                     .AddOption("Thank~you!", 12)
                                     .AddOption("See~you~then!", 255)
                                     .AddAvatar(35).FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Slay 50 Thunder Apes L58 for me! Be quick!")
                                     .AddOption("Okay!", 255)
                                     .AddAvatar(35).FinalizeDialog();
                                break;
                            }
                            data.AddText("Why can`t they stop bothering me! Go slay 50 Thunder Apes L58 for me! I need a quiet place for my practice!")
                               .AddOption("No~problem!", 11)
                                          .AddOption("Sorry,~not~today!", 255)
                                          .AddAvatar(35).FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B).~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                            client.SendSysMesage("You received 1,134,000 points of EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 1134000);

                            client.Player.Money += 2000;
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
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 60))
                        {

                            data.AddText("Thank you very much for wiping out the Thunder Apes for me! Here are your rewards!")
                          .AddOption("Thank~you!", 3)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I hope you can slay 60 Thunder Apes for me. Maybe then I can continue with my studies and practice.")
                                 .AddOption("Sorry,~not~today!", 255)
                                 .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Thunder Apes are such a nuisance to me! They keep on interrupting my practice. Take this Soulfire Token and slay 60 Thunder Apes for me!")
                          .AddOption("No~problem!", 2)
                                     .AddOption("Okay!", 255)
                                     .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                            client.SendSysMesage("You received 1,079,000 points of EXP, 1 Small Joy Stone, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.Inventory.Add(stream, 720173);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.IncreaseExperience(stream, 1079000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BreezePupil)]
        public static void BreezePupil(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BreezePupil, client.Player.Class, 1329);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BreezePupil, client.Player.Class, 1330);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BreezePupil, client.Player.Class, 1331);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I will become a great taoist in the future!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Blaze~Pill.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Capture~the~Apes.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snakeman~Leader(1).", 20);
                        data.AddOption("See~you~then!", 255);

                        data.AddAvatar(186).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please find the Moon Maiden for me!")
                                 .AddOption("See~you~then!", 255)
                                 .AddAvatar(186).FinalizeDialog();
                            break;
                        }
                        data.AddText("Everytime I entered the habitation of the Snakemen, I was bullied by the Snakeman Leader. I heard that the Moon Maiden is an expert in dealing with the Snakemen and making drugs for snakebite. Will you find her and ask her to help me?")
                            .AddText("~I~want~my~revenge!")
                            .AddOption("No~problem!", 21)
                                       .AddOption("Sorry,~not~today!", 255)
                                       .AddAvatar(186).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1020, 272, 438, 8296);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5, 5))
                            {
                                data.AddText("Thank you very much for your help! Here are your rewards!")
                                      .AddOption("You~are~welcome!", 12)
                                      .AddOption("See~you~then!", 255)
                                      .AddAvatar(186).FinalizeDialog();
                            }
                            else
                            {
                                if (!client.Inventory.Contain(720852, 1))
                                    client.Inventory.Add(stream, 720852);
                                data.AddText("Capture 5 Thunder Apes and 5 Thunder Apes L58 for me.")
                                      .AddOption("Okay!", 255)
                                      .AddAvatar(186).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("My master gave me a Ape Gourd. He told me that I can use it to capture some Apes. Use it to Capture 5 Thunder Apes and 5 Thunder Apes L58 for me.")
                                  .AddOption("No~problem!", 11)
                                        .AddOption("Sorry,~not~today!", 255)
                                        .AddAvatar(186).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.Remove(720852, 1, stream)) break;
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more space in your inventory")
                                    .AddOption("Let me check.", 255)
                                  .AddAvatar(186).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.SendSysMesage("You received 1,116,000 points of EXP, 5 Amritas, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1116000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                        }

                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                    .AddOption("Let me check.", 255)
                                  .AddAvatar(186).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720852, 1))
                            client.Inventory.Add(stream, 720852);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720851, 1))
                            {
                                data.AddText("Thank you very much for retrieving my Blaze Pill! Here are your rewards!")
                                      .AddOption("You~are~welcome!", 3)
                                      .AddOption("See~you~then!", 255)
                                    .AddAvatar(186).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need your help. Please bring the Blaze Pill back to me!")
                                         .AddOption("Okay!", 255)
                                       .AddAvatar(186).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("My master gave me a Blaze Pill. But the Thunder Ape grabbed it away. If my master knows that I lost the pill, he will be mad at me. Please find the pill back for me.")
                                  .AddOption("No~problem!", 2)
                                        .AddOption("Sorry,~not~today!", 255)
                                        .AddAvatar(186).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Remove(720851, 1, stream)) break;
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.SendSysMesage("You received 1,079,000 points of EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1079000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PoisonMaster)]
        public static void PoisonMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1348);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1349);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1350);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1351);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1352);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1353);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1354);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1355);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1356);
            var ActiveQuest10 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1357);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The purpose of studying poison is to save people`s lives!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Deliver~Medicine.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Thunder~Ginseng.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Courage~Test(1).", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Courage~Test(2).", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Courage~Test(3).", 40);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(1).", 50);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(2).", 60);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(3).", 70);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(4).", 80);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest10.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(5).", 90);

                        data.AddOption("Open another Quest.", 100).AddOption("Good!", 255);

                        data.AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 100:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 366, 304, 8293);
                        break;
                    }
                case 90:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest10.MissionId, 5))
                        {
                            data.AddText("Thank you very much for your help! The ape problem will be solved! Here are your rewards!")
                      .AddOption("Thank~you!", 92).AddOption("See~you~then!", 255)
                      .AddAvatar(56).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I have a plan to deal with the Apes. They`re very curious creatures. I am sure they will eat the Enervation Pill when they find it. I will give you a bottle of Enervation Pill. Spread the pills in the required position. After the Apes eat the pills,")
                                .AddText("~I am sure they will become less aggressive.")
                                .AddOption("No~problem!", 91)
                                      .AddOption("Sorry,~not~today!", 255)
                                      .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 92:
                    {
                        if (!client.Inventory.Remove(720846, 1, stream)) break;
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest10.MissionId))
                        {
                            client.Inventory.Add(stream, 150118, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.SendSysMesage("You received 518,000 points of EXP,a level 60 Elite Ring and 900 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 518000);
                            client.Player.Money += 900;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 91:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720846, 1))
                            client.Inventory.Add(stream, 720846);
                        client.Player.AtiveQuestApe = 0;
                        client.Player.QuestGUI.Accept(ActiveQuest10, 0);

                        break;
                    }
                case 80:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest9.MissionId, 1))
                        {
                            data.AddText("Thank you very much for your help! Here are your rewards!")
                      .AddOption("Thank~you!", 82).AddOption("See~you~then!", 255)
                      .AddAvatar(56).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Now, I want to put the Enervation Pill in the really tough battle. There is a Raging Ape in the wild. Feed the Enervation Pill to him and kill it!")
                                       .AddOption("No~problem!", 81)
                                      .AddOption("Sorry,~not~today!", 255)
                                      .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 81:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        if (client.Player.View.ContainMobInScreen("RagingApe") == false)
                            Database.Server.AddMapMonster(stream, client.Map, 14336, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        else
                        {
                            data.AddText("There is a Raging Ape in the wild. Feed the Enervation Pill to him and kill it!")
                                     .AddOption("Okay!", 255)
                                     .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 82:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest9.MissionId))
                        {
                            client.SendSysMesage("You received 1,152,000 points of EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1152000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 70:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest8.MissionId, 1))
                        {
                            data.AddText("Thank you very much for testing the Enervation Pill for me! Here are your rewards!")
                       .AddOption("Thank~you!", 72).AddOption("See~you~then!", 255)
                       .AddAvatar(56).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            if (client.Inventory.Contain(720845, 1))
                            {
                                data.AddText("Go and find a Thunder Ape L58 and feed it the Enervation Pill and kill it!")
                                    .AddOption("Okay!", 255)
                                    .AddAvatar(56).FinalizeDialog();
                                break;
                            }
                            data.AddText("This time, I want to test the Enervation Pill on a real ape. Go find a Thunder Ape L58 and feed it the Enervation Pill and kill it! Don`t forget to tell me its response.")
                                   .AddOption("No~problem!", 71)
                                  .AddOption("Sorry,~not~today!", 255)
                                  .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 72:
                    {
                        if (HasItems(client, 720845, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            if (!client.Inventory.Remove(720845, 1, stream)) break;
                            client.SendSysMesage("You received 864,000 points of EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 864000);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }


                        break;
                    }
                case 71:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720845, 1))
                            client.Inventory.Add(stream, 720845);
                        client.Player.QuestGUI.Accept(ActiveQuest8, 0);

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest7.MissionId, 1))
                        {
                            data.AddText("Thank you very much for helping me test the poison! Here are your rewards!")
                       .AddOption("Thank~you!", 62).AddOption("See~you~then!", 255)
                       .AddAvatar(56).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            data.AddText("Okay! This is the medicine I made! I call it Enervation Pill. Take Transmute Pill to transform into an Ape and then take the Enervation Pill to see what will happen on you!")

                                   .AddOption("No~problem!", 61)
                                  .AddOption("Sorry,~not~today!", 255)
                                  .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 62:
                    {
                        if (HasItems(client, 720845, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            if (!client.Inventory.Remove(720845, 1, stream)) break;
                            client.SendSysMesage("You received 461,000 points of EXP, and 800 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 461000);
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }


                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720845, 1))
                            client.Inventory.Add(stream, 720845);
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);


                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            data.AddText("Yes, that is the Transmute Pill. Let`s go to the next step.")
                       .AddOption("Let`s~begin!", 52).AddOption("See~you~then!", 255)
                       .AddAvatar(56).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Go get the Transmute Pill from my old friend, the Apothecary!")
                         .AddOption("Okay!", 255)
                         .AddAvatar(56).FinalizeDialog();
                                break;
                            }
                            data.AddText("I have a good news to share with you! I made a new medicine! It can weaken the apes, but it does no harm to humans. So I can`t use my body to test its potency. Can you get a Transmute Pill from the Apothecary to transform into an Ape")
                             .AddText("~and then take my medicine to test it?")
                                .AddOption("No~problem!", 51)
                               .AddOption("Sorry,~not~today!", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 52:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.SendSysMesage("You received 288,000 points of EXp, and 500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 288000);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 51:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1))
                        {
                            data.AddText("I am quite impressed by your courage! Here are your rewards!")
                             .AddOption("Thank~you!", 42)
                             .AddOption("See~you~then!", 255)
                             .AddAvatar(56).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Here is the final test! Jump off the cliff.  If you are a man of dignity, righteousness and courage, you will be lifted up by the clouds! Dare you try this?")
                              .AddOption("No~problem!", 41)
                              .AddOption("Never!", 255)
                              .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 42:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.SendSysMesage("You received 1,116,000 points of EXp, 1 Meteor and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1116000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                        }

                        break;
                    }
                case 41:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                data.AddText("I am quite impressed by your courage! Here are your rewards!")
                          .AddOption("Thank~you~very~much!", 32)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(56).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Kill the Thunder Ape King to prove your courage to me!")
                            .AddOption("Okay!", 255)
                            .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }

                        data.AddText("You passed one of my Courage Test, but it is only a beginning. Now, kill the Thunder Ape King!")
                              .AddOption("No~problem!", 31)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.SendSysMesage("You received 558,000 points of EXp, and 1000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 558000);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 31:
                    {
                        client.SendSysMesage("The Thunder Ape King appeared in front of your kill it!", MsgMessage.ChatMode.System);
                        if (client.Player.View.ContainMobInScreen("ApeTyrant") == false)
                            Database.Server.AddMapMonster(stream, client.Map, 678000, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                            {
                                if (!client.Inventory.Contain(720870, 1, 0))
                                {
                                    data.AddText("What? You lost the Toxicant? Maybe I should ask someone else to take my courage test!")
                          .AddOption("Thank~you~very~much!", 22)
                          .AddAvatar(56).FinalizeDialog();
                                    break;
                                }
                                data.AddText("I am quite impressed by your courage! Here are your rewards!")
                       .AddOption("Never", 255)
                       .AddAvatar(56).FinalizeDialog();

                            }
                            else
                            {
                                if (!client.Inventory.Contain(720870, 1, 0))
                                {
                                    data.AddText("What? You lost the Toxicant? Maybe I should ask someone else to take my courage test!")
                          .AddOption("Thank~you~very~much!", 22)
                          .AddAvatar(56).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Dare you take this Toxicant?")
                        .AddOption("Never", 255)
                        .AddAvatar(56).FinalizeDialog();
                                }
                            }

                            break;
                        }
                        data.AddText("People told me that you are brave. But I don`t believe it! You need to prove it to me! I made a new Toxicant! Dare you take it to test its power like me?")
                             .AddOption("No~problem!", 21)
                             .AddOption("Never!", 255)
                             .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You received 279,000 points of EXp, 1 EXP Ball Scrap, and 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 279000);
                            client.Inventory.Add(stream, 720172);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Inventory.Add(stream, 720870);

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                       && client.Inventory.Contain(720869, 1, 0))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                            {
                                data.AddText("Thank you very much for the Thunder Ginseng you collected! Here are your rewards!")
                        .AddOption("You~are~welcome!", 12)
                        .AddOption("See~you~then!", 255)
                        .AddAvatar(56).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Use the Sickle to collect 10 Thunder Ginsengs for me!")
                           .AddOption("Okay!", 255)
                           .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }
                        else
                        {
                            data.AddText("I need 10 Thunder Ginsengs to make an antidote to a rare poison. Take this Sickle and collect 10 Thunder Ginsengs for me!")
                            .AddOption("No~problem!", 11)
                            .AddOption("Sorry,~not~today!", 255)
                            .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 720869, 1) && HasItems(client, 720868, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(720869, 1, stream)) break;
                            if (!client.Inventory.Remove(720868, 10, stream)) break;
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B).~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                            client.SendSysMesage("You received 809,000 points of EXp, 5 Amritas and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.IncreaseExperience(stream, 809000);
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Inventory.Add(stream, 720869);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        data.AddText("Thank you. I can use this Bezoar to purge the remaining poison from my body!")
                        .AddOption("You~are~welcome!", 2)
                        .AddOption("See~you~then!", 255)
                        .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (HasItems(client, 711362, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711362, 1, stream)) break;
                            client.SendSysMesage("You received 247,000 points of EXp, and 800 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 247000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Apothecary)]
        public static void Apothecary(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1363);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1364);

            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1348);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1353);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I have been doctored in Ape City for decades. Saving people is my job!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Collect~Medicine.", 50);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Collect~Medicine.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Weary~Pill.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Deliver~Medicine.", 30);
                        data.AddOption("Great~doctor!", 255);
                        data.AddAvatar(64).FinalizeDialog();

                        break;
                    }
                case 50:
                    {


                        data.AddText("Poison Master asked you to get a Transmute Pill from me? My friend must have an interesting plan! Ok, this is the Transmute Pill you need!")
                        .AddOption("Thanks.", 51)
                        .AddAvatar(64).FinalizeDialog();

                        break;
                    }
                case 51:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720871, 1))
                        {
                            client.Inventory.Add(stream, 720871);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest5.MissionId, 1);
                        }

                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && client.Inventory.Contain(711362, 1, 0))
                        {
                            data.AddText("Please send the Quality Bezoar to my friend Poison Master.")
                         .AddOption("Okay.", 255)
                         .AddAvatar(64).FinalizeDialog();
                            break;
                        }

                        data.AddText("My friend Poison Master dedicated his life to the research of poison. He even use himself to try the poison and find out the way of detoxification. I am worrying about his body! After so many years of trial, many toxin must have remained in his body!")
                           .AddText("~I have a Quality Bezoar. It can help him clear up the remained toxin in his body! Will you send the Quality Bezoar to him?")
                            .AddOption("My~pleasure!", 31)
                             .AddOption("Sorry,~not~today!", 255)
                             .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.Inventory.Add(stream, 711362, 1);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.TransformationID == 356)
                        {
                            data.AddText("Thank you very much for testing the Weary Pill! It seems that it is safe to mankind! Here are your rewards!")
                          .AddOption("You~are~welcome!", 12)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        data.AddText("After years of study! I developed a Weary Pill that can deal with the apes in Ape City! But I need someone to test it whether it`s harmful to human beings. Will you try this Weary Pill? Don`t worry, I am a doctor, I can save you if anything bad happens.")
                                 .AddOption("I~can~risk~it!", 11)
                                   .AddOption("Sorry,~not~today!", 255)
                                   .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            client.Inventory.AddItemWitchStack(1000030, 0, 4, stream);
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720865, 1, 0))
                        {
                            client.Inventory.Add(stream, 720865);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                            {
                                data.AddText("These are what I have been looking for! Thank you!")
                         .AddOption("You~are~welcome!", 3)
                         .AddOption("See~you~then!", 255)
                         .AddAvatar(64).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The wounded soldiers are waiting for medicine. Collect 1 Ape Claw, 1 Ape Bone, and 1 Ape Skin for me to make a medicine to treat them!")
                            .AddOption("Okay!", 255)
                            .AddAvatar(64).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Many soldiers were wounded by the monkeys. Collect 1 Ape Claw, 1 Ape Bone, and 1 Ape Skin for me! I can will make a medicine to treat the wounded soldiers.")
                                .AddOption("No~problem!", 2)
                                  .AddOption("Sorry,~not~today!", 255)
                                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (HasItems(client, 711357, 1) && HasItems(client, 711358, 1) && HasItems(client, 711359, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711357, 1, stream)) break;
                            if (!client.Inventory.Remove(711358, 1, stream)) break;
                            if (!client.Inventory.Remove(711359, 1, stream)) break;
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                            client.Inventory.Add(stream, 720172);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CarpenterJack)]
        public static void CarpenterJack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ACCastellan, client.Player.Class, 1308);//The Carpenter
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1315);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.JackDaniel, client.Player.Class, 1316);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1317);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I am the only carpenter in Ape City.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~The~Carpenter.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Wood~Shortage(1).", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Wood~Shortage(2).", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Cliff~Flower.", 30);
                        data.AddOption("The~city~needs~you!", 255)
                            .AddOption("Continue Quests.", 50);
                        data.AddAvatar(9).FinalizeDialog();

                        break;
                    }
                case 50:
                    {
                        var nextquest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1363);
                        client.Player.QuestGUI.SendAutoPatcher(nextquest.FinishNpcId.Map, nextquest.FinishNpcId.X, nextquest.FinishNpcId.Y, nextquest.FinishNpcId.ID);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 20))
                        {
                            data.AddText("Yes! That is the flower I have been looking for! It can help us to consolidate the bridges! Here are your rewards.")
                            .AddOption("Great!", 32)
                              .AddOption("See~you~then!", 255)
                              .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        data.AddText("My master told me that the juice of the Cliff Flower can be painted on the wood to prevent it from decaying and make it stronger.")
                            .AddText("~I hope you can collect 20 Cliff Flowers for me, so I can use them to fix the bridge outside Ape City.")
                            .AddOption("No~problem!", 31)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                            .AddOption("let me check.", 255)
                            .AddAvatar(9).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Remove(711356, 20, stream)) break;
                            client.SendSysMesage("You received 247,000 points of EXP, 1 Ape City Gage and 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 247000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 1060022);

                        }

                        break;
                    }
                case 31:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 50))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                            .AddOption("let me check.", 255)
                            .AddAvatar(9).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.SendSysMesage("You received 743,000 points of Exp, 1,500 Silver, 1 Meteor.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                client.IncreaseExperience(stream, 743000, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.Money += 1500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Add(stream, Database.ItemType.Meteor);
                                data.AddText("The wood is arriving? Good! I will have enough materials to fix the wooden bridge! Thank you very much!")
                           .AddOption("You~are~welcome!", 255)
                           .AddAvatar(9).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Go slay 50 Giant Ape and protect the wood transport column.")
                       .AddOption("Okay!", 255)
                       .AddAvatar(9).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please go visit Jack Daniel.")
                           .AddOption("I~will~visit~him~right~away!", 11)
                           .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        data.AddText("The wooden bridge outside Ape City need to be fixed, but I don`t have enough wood. Speak with Jack Daniel to get some wood for me!")
                            .AddOption("No~problem!", 11)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check.", 255)
                        .AddAvatar(9).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 711351, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711351, 1, stream)) break;
                            client.SendSysMesage("You received 247,000 points of EXP, 500 Silver, and 1 Diligence Book.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 247000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 723340);
                            data.AddText("Thank you very much! This bottle of Bamboo Liquid can be a nice, little refreshment for me!")
                       .AddOption("You~are~welcome!", 255)
                       .AddAvatar(9).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ACCastellan)]
        public static void ACCastellan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ACCastellan, client.Player.Class, 1306);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ACCastellan, client.Player.Class, 1307);//a waring
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ACCastellan, client.Player.Class, 1308);//The Carpenter
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("It`s difficult to manage a city. There are too many things you need to think about!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Vulture~Feather.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~A~Warning.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~The~Carpenter.", 20);
                        data.AddOption("You~need~a~break!", 255);
                        data.AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check.", 255)
                        .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(711351, 1))
                                client.Inventory.Add(stream, 711351);
                            data.AddText("Please go visit Carpenter Jack, and bring him the Bamboo Liquid.")
                        .AddOption("I~will~visit~him~right~away!", 255)
                        .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        data.AddText("Carpenter Jack helped us a lot by fixing the wooden bridge outside the city. I want you to visit him and bring him this bottle of Bamboo Liquid, to express our gratitude.")
                            .AddOption("My~pleasure!", 21)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Inventory.Add(stream, 711351);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check.", 255)
                        .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.SendSysMesage("You received 374,000 points of EXP, 800 Silver, and 1 Apec City Gate.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                    client.IncreaseExperience(stream, 374000, Role.Flags.ExperienceEffect.angelwing);
                                    client.Player.Money += 800;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 1060022);
                                }
                                data.AddText("You brought peace back to the city! Thank you very much for your help. Here are your rewards!")
                           .AddOption("Thanks.", 255)
                           .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                if (!client.Inventory.Contain(720844, 1))
                                    client.Inventory.Add(stream, 720844);
                                data.AddText("The Macaques are doing increasing damage to farms. We need to do something to drive them away! Have you ever heard of the story of beating the dog before the lion? I want to do the same thing this time. Here is a bag of Pheasants.")
                            .AddText("~Release the Pheasant and kill it before the Macaques. It will be a warning to the Macaques. Go do it for me!")
                            .AddOption("I~will~do~it~right~away!", 255)
                            .AddAvatar(7).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Macaques are doing increasing damage to farms. We need to do something to drive them away! Have you ever heard of the story of beating the dog before the lion? I want to do the same thing this time. Here is a bag of Pheasants.")
                           .AddText("~Release the Pheasant and kill it before the Macaques. It will be a warning to the Macaques. Go do it for me!")
                            .AddOption("My~pleasure!", 11)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.Contain(720844, 1))
                            client.Inventory.Add(stream, 720844);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1020, 648, 634, 0);
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
                                    client.SendSysMesage("You received 674,000 points of Exp, 1,500 Silver and 1 Exp Ball Scrap.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                    client.IncreaseExperience(stream, 674000, Role.Flags.ExperienceEffect.angelwing);
                                    client.Player.Money += 1500;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720172);
                                }
                                data.AddText("You brought peace to the city! Thank you very much for your help. Here are your rewards!")
                           .AddOption("Thanks.", 255)
                           .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Go get a Vulture Feather and burn it in the city. The other Vultures will never come near Ape City, after that!")
                            .AddOption("I~will~do~it~right~away!", 255)
                            .AddAvatar(7).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We are quite annoyed by the Vultures outside Ape City, but I have an effective way to drive them far away from the city! If we get a Vulture Feather and burn it in the city, the other Vultures won`t dare come back!")
                           .AddText("~Don`t ask why, I know what I`m doing! Would you like to do this job for me?")
                            .AddOption("My~pleasure!", 2)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Lydia)]
        public static void Lydia(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Lydia, client.Player.Class, 1311);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The price of gold will soar! Believe me!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Retrieve~Jewelry.", 1);
                        data.AddOption("I~don`t~believe~it!", 255);
                        data.AddAvatar(1).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 20))
                            {
                                data.AddText("These are my Jewelry Boxes. Thank you very much for helping me! Here are some rewards for you!")
                               .AddOption("Thanks.", 3)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(1).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please! Please retrieve 20 Jewelry Boxes for me from the Macaques!")
                               .AddOption("I~will~do~it~right~away!", 255)
                               .AddAvatar(1).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Macaques broke in the house and ran away with 20 of my Jewelry Boxes! They`re all I have! Will you bring them back for me?")
               .AddText("~I need to sell them to feed my family!")
               .AddOption("Don`t~worry,~I~will~help~you!", 2)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(1).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(1).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 711352, 20) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711352, 20, stream)) break;
                            client.SendSysMesage("You received 701,000 points of EXP, 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 701000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.CreateBoxDialog("You~received~a~+1~Stone.~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AC_Lieutenant)]
        public static void AC_Lieutenant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaCaptain, client.Player.Class, 1300);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1301);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1303);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1304);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1305);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("What would be the proof of a hero? A true hero never falters, even when the enemy is at the gates!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~The~Ape~City.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Clear~the~Way.", 2);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Appease~Dead~Souls.", 5);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Macaques(1).", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Macaques(2).", 15);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Macaques(3).", 20);
                        data.AddOption("You~are~a~great~hero!", 255);
                        data.AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                            {
                                data.AddText("Good job! Here are your rewards! Thank you very much for your help!")
                               .AddOption("Thanks.", 22)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 1 Monkey King to return peace to Ape City.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("This time, our target is the Monkey King. Slay the Monkey King and bring back its tail. It will help inspire the people in Ape City,")
               .AddText("~and should lift their spirits a bit.")
               .AddOption("My~pleasure.", 21)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.HaveSpace(2) == false)
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 720863, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            if (!client.Inventory.Remove(720863, 1, stream)) break;
                            client.SendSysMesage("You received 701,000 points of EXP, 1,500 Silver, 1 emerald and level50 EliteBoots.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 701000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 1080001);
                            client.Inventory.Add(stream, 160098);
                        }

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 15:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 30))
                            {
                                data.AddText("You gave the Macaques a heavy blow. I think they will think twice before coming, again. Thank you very much for your help. Here are your rewards!")
                               .AddOption("Thanks.", 17)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 30 MacaquesL48 to return peace to Ape City.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("According to my observations, the Macaques L48 are the upper level forces of the apes. I need you to slay 30 Macaques L48 for me, to weaken them.")
                  .AddOption("My~pleasure.", 16)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 17:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.SendSysMesage("You received 701,000 points of EXP, 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 701000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 50))
                            {
                                data.AddText("I am quite impressed by your performance! Here are your rewards!")
                               .AddOption("Thanks.", 12)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 50 Macaques to return peace to Ape City.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The naughty Macaques has been making trouble in the city for too long! We are tired of the constant noise, and don`t get")
                          .AddText("~me started on the smell! Go slay 50 Macaques for me!")
                  .AddOption("My~pleasure.", 11)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.SendSysMesage("You received 467,000 points of EXP, 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 467000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 1, 1, 1, 1))
                            {
                                data.AddText("Thank you very much for your help! Here are your rewards!")
                               .AddOption("Thanks.", 7)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please go appease the souls of my fallen soldiers on the battlefield.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("I lost 5 of my best men in a decisive battle. They appeared to me one night, in a vision. Their souls are still wandering")
                            .AddText("~the battlefield, tortured and lost. I hope you can go to find their bodies and appease their souls for me.")
                    .AddOption("My~pleasure.", 6)
                    .AddOption("Sorry,~not~today!", 255)
                   .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You received 674,000 points of EXP, 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 674000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            break;
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 179,000 points of EXP, 400 Silver, and 5 Amritas.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 179000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 400;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            break;
                        }
                        data.AddText("Nice to see you, hero! It is great that you came to Ape City. We could use the help. You need to keep your eyes open, while you`re here.")
                             .AddText("~This is not a safe place to just wander around.")
                     .AddOption("Okay!", 255)
                    .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("Thank you very much for your help! Here are your rewards!")
                .AddOption("Thank~you!", 4)
                .AddOption("See~you~then!", 255)
               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("For the safety of the town, you must wipe out 50 Vultures gathered outside of Ape City!")
                  .AddOption("Okay!", 255)
                 .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Many Vultures appeared at the gates of Ape City. For the safety of the town, I hope you can help us slay 50 Vultures.")
                            .AddOption("No~problem!", 3)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You received 674,000 points of EXP, 1,500 Silver, and 1 +1Stone.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 674000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B).~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GreatMerchant)]
        public static void GreatMerchant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GreatMerchant, client.Player.Class, 706);
            switch (Option)
            {
                case 0:
                    {

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("We have various kinds of stores in the market. But if you are interested in some rare items with special effect, I will be the")
                                .AddText("~perfect one to help you with your needs.")
             .AddOption("Well,~impress~me.", 1)
                        .AddOption("Purchase~Item.", 20)
                       .AddAvatar(174).FinalizeDialog();
                        }
                        else
                            goto case 20;

                        break;
                    }
                case 20:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = (uint)Role.Flags.NpcType.DialogAndGui,
                            Type = ActionType.OpenGuiNpc,
                            Timestamp = (int)client.ActiveNpc,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            dwParam3 = client.Player.Map
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(722700, 1) && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) || client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {
                                data.AddText("How do you like it? I have lots of those items. When you need them, don`t hesitate to let me know!")
                           .AddOption("Brilliant.", 5)
                           .AddAvatar(174).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Just right click this EXP Potion to activate a 10 minute double EXP bonus. Talk to me again when you`re done.")
                             .AddOption("Okay.", 255)
                             .AddAvatar(174).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("You can buy some unusual items in the Shopping Mall. To enter it, talk to the Great Merchant")
                              .AddText("~in the Market, or click the Shopping Mall Button on the lower left of your screen when you are level 70 or have some CPs on you")
           .AddOption("CPs?", 2)
                     .AddAvatar(174).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("I guess you don`t know what that is yet, right?")
                                .AddText("~CP is short for Conquer Points, which are used to purchase special items from the Shopping Mall.")
                            .AddOption("I~see.", 3)
                            .AddAvatar(174).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Please take this EXP Potion as a present, you will obtain a 10 mins double EXP bonus when using it.")
                              .AddOption("Thank~you!", 4)
                              .AddAvatar(174).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(174).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 722700);
                        client.CreateBoxDialog("You`ve~received~an~EXP~Potion.~Right~click~it~to~use~it.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.CreateBoxDialog("You`ve~learned~about~CPs~and~the~Shopping~Mall,~and~received~EXP~worth~1/12~of~an~EXP~Ball!");
                        }

                        break;
                    }

            }
        }
    }
}
