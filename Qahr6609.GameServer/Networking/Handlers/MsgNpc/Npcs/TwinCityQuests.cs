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
        //ThiefChen
        [NpcAttribute(NpcID.ThiefChen)]
        public static void ThiefChen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefWong, client.Player.Class, 1719);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefChen, client.Player.Class, 1720);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefChen, client.Player.Class, 1721);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Beware of the bandits. They`re dangerous.");
                            data.AddOption("Are~you~the~Connector?", 1)
                                .AddOption("Thanks~for~reminding.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5))
                            {
                                data.AddText("Beware of the bandits. They`re dangerous.");
                                data.AddOption("Hi!~Connector!", 4)
                                    .AddOption("Thanks~for~reminding.", 255)
                                   .AddAvatar(72).FinalizeDialog();
                                break;
                            }
                            data.AddText("Beware of the bandits. They`re dangerous.");
                            data.AddOption("I~came~for~Thief~Wong.", 2)
                                .AddOption("Thanks~for~reminding.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Yes! Here`s a letter that should help. Take it to Wong. I bet he will pay you when he sees what I have to say.");
                            data.AddOption("I~won`t~let~you~down.", 5)
                                .AddOption("I`m~busy.", 255)
                               .AddAvatar(72).FinalizeDialog();

                        }
                        else
                        {
                            data.AddText("Beware of the bandits. They`re dangerous.");
                            data.AddOption("Thanks~for~reminding.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("You said you came from Twin City and were looking for Thief Chen? What`s the problem?");
                        data.AddOption("Thief~Wong~sent~me~here.", 2)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver.", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Well, I`m still going to need you to prove yourself. Go kill 5 Bandits, first!");
                        data.AddOption("Piece~of~cake.", 3).AddOption("I`m~leaving.", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Thief~Chen~ran~away~and~hid.~Now~you`re~going~to~find~and~kill~5~Bandits.");

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.IncreaseExperience(stream, 1600, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver and 1600 EXP.", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Fine. I guess I can trust you, after all.");
                        data.AddOption("Good.", 6)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("Yes! Here`s a letter that should help. Take it to Wong. I bet he will pay you when he sees what I have to say.");
                        data.AddOption("I~won`t~let~you~down.", 5)
                            .AddOption("I`m~busy.", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Why are you still here? Deliver the letter to Wong, right away!");
                            data.AddOption("I~lost~the~letter.", 7)
                                .AddOption("I`m~on~my~way.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 729942);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~received~the~Connector~Letter.~Hurry~and~deliver~it~to~Thief~Wong~(347,228).", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(729942, 1))
                        {
                            data.AddText("Hey, friend! Check your bag. The letter is right there!");
                            data.AddOption("Oh.", 255)
                               .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                client.Inventory.Add(stream, 729942);
                                client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                                client.Player.QuestGUI.SendAutoPatcher("You`ve~received~the~Connector~Letter.~Hurry~and~deliver~it~to~Thief~Wong~(347,228).", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ThiefWong)]
        public static void ThiefWong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1718);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefWong, client.Player.Class, 1719);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefChen, client.Player.Class, 1721);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefWong, client.Player.Class, 1722);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Here`s~a~letter~for~you!", 10)
                                .AddOption("Good.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Yeah!~Count~me~in!", 1)
                                .AddOption("Leave.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) &&
                            !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Let~me~help~you!", 3)
                                .AddOption("Leave.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) &&
                            client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Need~my~help?", 12)
                                .AddOption("Good.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Good.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("To tell you the truth, I stole some jewelry from those slum lords, and want to give the proceeds back to the poor in Maple Forest.");
                        data.AddOption("That~would~be~great!", 2)
                            .AddOption("Leave.", 255)
                           .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            data.AddText("To tell you the truth, I stole some jewelry from those slum lords, and want to give the proceeds back to the poor in Maple Forest.");
                            data.AddOption("That~would~be~great!", 3)
                                .AddOption("Leave.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("The problem is, I was wounded by the guards as I escaped. Can you take a message to my connector, Thief Chen?")
                            .AddText("~He should be somewhere on the road between Phoenix Castle and Twin City.");
                        data.AddOption("Sure.", 4)
                            .AddOption("Sorry,~I~can`t~help.", 255)
                           .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~find~Thief~Chen~on~the~road~between~Phoenix~Castle~and~Twin~City.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.Remove(729942, 1, stream)) break;
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, Role.Flags.ExperienceEffect.bless.ToString());
                            client.SendSysMesage("You've received EXP worth 1/6 EXP Ball and 1,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you for the letter!");
                        data.AddOption("You`re~welcome.", 11)
                           .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("Here`s your reward. Thank you again!");
                        data.AddOption("You`re~welcome.", 12)
                           .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~claim~your~reward~from~Taskmaster~Chang!~It`s~a~necklace!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ZhaoJian)]
        public static void ZhaoJian(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1725);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1))
                            {
                                data.AddText("I am the best at disguise work. As long as you pay, I can disguise you as almost anything! Young, old, pretty, ugly...you name it.")
                                 .AddOption("Whatever.", 255)
                                .AddAvatar(33).FinalizeDialog();
                                break;
                            }
                            data.AddText("I am the best at disguise work. As long as you pay, I can disguise you as almost anything! Young, old, pretty, ugly...you name it.");
                            data.AddOption("I~need~your~help.", 1)
                                .AddOption("Whatever.", 255)
                               .AddAvatar(33).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I am the best at disguise work. As long as you pay, I can disguise you as almost anything! Young, old, pretty, ugly...you name it.")
                                .AddOption("Whatever.", 255)
                               .AddAvatar(33).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                        client.Player.TransformInfo.CreateTransform(stream, 817, 224, (int)60, 8213);
                        client.Player.QuestGUI.SendAutoPatcher("You~are~disguised~as~a~ravishing~beauty!", 1011, 100, 430, 0);
                        data.AddText("I usually charge a fortune for this work, but as you`re going to rid us of Cloud the Lustful... I suppose I can make an exception.")
                              .AddOption("Thank~you.", 255)
                             .AddAvatar(33).FinalizeDialog();

                        break;
                    }

            }
        }


        [NpcAttribute(NpcID.VeteranHong)]
        public static void VeteranHong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1723);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1724);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1725);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1726);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                                .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1)
                                .AddOption("You~bet!", 255)
                               .AddAvatar(91).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                               .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                                data.AddOption("Anything~I~can~help~you~with?", 4)
                                    .AddOption("You~bet!", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                         .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                                data.AddOption("Talk~about~the~lost~armor.", 10)
                                    .AddOption("You~bet!", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                         .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                                data.AddOption("I`ve~got~something~for~you..", 13)
                                    .AddOption("You~bet!", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                         .AddText("~Now that I am old, I can still fight as well as I could when I was young.")
                                    .AddOption("You~bet!", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                     .AddText("~Now that I am old, I can still fight as well as I could when I was young.")
                                .AddOption("You~bet!", 255)
                               .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes! I`m in trouble now, and I could use your help. Someone has stolen my Spun Gold Armor. Were this found out by the Emperor,")
                               .AddText("~I would be sentenced to death! Please, you must help me! You should find some clues from those Bandits who roam outside the city.");
                        data.AddOption("Got~it.", 2)
                            .AddOption("Spun~Gold~Armor?", 255)
                           .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver.", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Yes! I guess it was those bandits who stole my armor. You should find them and try to recover any clues you can. Feel free to use force.");
                        data.AddOption("You~can~count~on~me!", 3)
                            //   .AddOption("Spun~Gold~Armor?", 10)
                            .AddOption("I`m~busy.", 255)
                           .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                        {
                            data.AddText("Rumor has it that those bandits who roam outside the city have something to do with this. Go find them and see what they know!");
                            data.AddOption("Dirty~thief!", 6)
                                .AddOption("Who`s~Cloud~the~Lustful?", 7)
                               .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("This is an invitation to the Treasure Appraisal Meeting. Now I`m positive the Spun Gold Armor was stolen by Cloud the Lustful!");
                        data.AddOption("Okay.", 3)
                           .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (HasItems(client, 729971, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(729971, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver!", MsgMessage.ChatMode.System);

                            data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                            .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                            data.AddOption("Talk~about~the~lost~armor.", 10)
                                .AddOption("You~bet!", 255)
                               .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        data.AddText("Cloud the Lustful is the head of those bandits. They are a motley crew... Notorious and capable of committing")
                           .AddText("~every kind of crime conceivable. Now they even have the gall to steal my Spun Gold Armor! Those nasty rogues!");
                        data.AddOption("Indeed.", 255)
                           .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Go~find~some~clues~from~the~Bandits.");

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.Player.Money += 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, Role.Flags.ExperienceEffect.bless.ToString());
                                client.SendSysMesage("You've received EXP worth 1/6 EXP Ball and 1,000 Silver!", MsgMessage.ChatMode.System);
                            }
                            data.AddText("Thank you! I thought I would never regain my precious armor. Please, let me repay you for your heroic feat.");
                            data.AddOption("My~pleasure.", 13)
                               .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("He usually shows up around a small altar in the southwest of Phoenix Castle. He is a randy old man, and actually quite disgusting.")
                            .AddText("~The only way to attract the attention of Cloud the Lustful is beautiful girls. Please take these offerings and find Zhao Jian,")
                            .AddText("~the Disguiser (200,325). He will disguise you as an astounding beauty.");
                        //data.AddOption("I`m~setting~off.", 11)
                        data.AddOption("Gimme~some~offerings.", 12)
                        .AddOption("It`s~crazy.", 255)
                       .AddAvatar(91).FinalizeDialog();

                        break;
                    }

                case 12:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(91).FinalizeDialog();
                            break;
                        } if (client.Inventory.Contain(729972, 2))
                        {
                            data.AddText("Are you kidding me? You`ve already got the offering!");
                            data.AddOption("Let~me~see.", 255).AddAvatar(91).FinalizeDialog();
                            //Are you kidding me? You`ve already got the offering!
                            break;
                        }
                        client.CreateBoxDialog("You`ve~got~the~offering.~Now~go~visit~Disguiser~Zhao~(200,325)~for~help.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Inventory.Add(stream, 729972);

                        break;
                    }
                case 13:
                    {
                        data.AddText("Thank you for retrieving my Spun Gold Armor. Now you can speak with Taskmaster Chang, to claim your reward.");
                        data.AddOption("Great!", 255)
                           .AddAvatar(91).FinalizeDialog();
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~claim~your~reward~from~Taskmaster~Chang!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ScholarWu)]
        public static void ScholarWu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1727);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ScholarWu, client.Player.Class, 1728);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1731);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ScholarWu, client.Player.Class, 1732);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                            data.AddOption("Did~you~go~to~see~a~doctor?", 1)
                                .AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                    .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                                data.AddOption("Maybe~I~can~help?", 3)
                                    .AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                    .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                                data.AddOption("I`ll~try~my~best~to~help~you!", 7)
                                    .AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                    .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                                data.AddOption("I`ll~try~my~best~to~help~you!", 8)
                                    .AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                   .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                                data.AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                               .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                            data.AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        data.AddText("Hats off to your valor! You saved my life! Please accept this, with my thanks.");
                        data.AddOption("Thank~you.", 8).AddAvatar(40).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(200, true, Role.Flags.ExperienceEffect.angelwing);
                            client.SendSysMesage("You've received EXP worth 1/3 EXP Ball and 1,500 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you! You saved me! Now go claim your reward from Taskmaster Chang!");
                        data.AddOption("I`m~on~the~way.", 9).AddOption("Later.", 255).AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~claim~your~reward~from~Taskmaster~Chang!~It`s~a~ring!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes. But sigh... I still haven`t gotten over it...");
                        data.AddOption("Sorry~to~hear~that.", 2).AddAvatar(40).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver!", MsgMessage.ChatMode.System);
                        }

                        goto case 3;
                    }
                case 3:
                    {
                        data.AddText("Thank you. I heard there is a rat called the Fire Rat L38 outside the castle... Its eye and meat could be used to make a powerful")
                            .AddText("~pill, called the Azure Pill. But those rats are ferocious... Will you be so kind as to help me with this?");
                        data.AddOption("Sure!", 4).AddOption("I`m~afraid~I~can`t~help.", 255).AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 4:
                    {

                        data.AddText("Well, you are so generous... thank you. The recipe for the Azure Pill belongs to Pharmacist Mu Mu (182,241). Please go there")
                            .AddText("~immediately... Oh, my throat...");
                        data.AddOption("I`m~off~right~now.", 5).AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Head~to~visit~Pharmacist~Mu~Mu~(182,241).", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BianQing)]
        public static void BianQing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1729);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 2) && client.Inventory.Contain(721697, 2))
                            {
                                data.AddText("Oh... look who`s coming? I have anything you want. Just name it.");
                                data.AddOption("You~bet!", 255).AddAvatar(55).FinalizeDialog();
                                break;
                            }
                            data.AddText("Oh... look who`s coming? I have anything you want. Just name it.");
                            data.AddOption("You~knew~the~Azure~Pill?", 1)
                                .AddOption("You~bet!", 255).AddAvatar(55).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Oh... look who`s coming? I have anything you want. Just name it.");
                            data.AddOption("You~bet!", 255).AddAvatar(55).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes, sure. So my old friend Mu Mu sent you here? Let me see... you need to collect 1 Ratling Eye and 3 Ratling Meat, yourself,")
                            .AddText("~but I will give you the 2 Herb Packs for free. He must need them to make some medicine. Here you are.");
                        data.AddOption("Thanks.", 2).AddAvatar(55).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.");
                            data.AddOption("Let me check.", 255).AddAvatar(55).FinalizeDialog();
                            break;
                        }
                        client.Inventory.AddItemWitchStack(721697, 0, 2, stream);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 2);
                        client.CreateBoxDialog("You~receive~2~Herb~Packs.~You~still~need~1~Ratling~Eye~and~3~Ratling~Meat!");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PharmacistMuMu)]
        public static void PharmacistMuMu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ScholarWu, client.Player.Class, 1728);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1729);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1730);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1731);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                            data.AddOption("You~knew~Scholar~Wu?", 1)
                                .AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                            data.AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                                data.AddOption("What~about~the~Azure~Pill?", 4)
                                    .AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                                break;
                            }
                            data.AddText("Scholar Wu is suffering from a rare asthma. Only the Azure Pill can cure it. I need 1 Ratling Eye, 3 Ratling Meat and")
                                .AddText("~2 Herb Packs. Kill Ratlings and you`ll have a chance to get what I need. You can ask Bian Qing (Twin City 342,232) for the packs.");
                            data.AddOption("I`m~on~my~way.", 3)
                                .AddOption("None~of~my~business.", 255).AddAvatar(62).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("It will take a while to make the Azure Pill. Be patient.");
                                data.AddOption("Alright.", 6).AddAvatar(62).FinalizeDialog();
                            }
                            else
                            {
                                if (DateTime.Now > client.Player.AzurePillStamp)
                                {
                                    data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                                    data.AddOption("How`s~going~with~the~pill?", 9).AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                                    data.AddOption("How`s~going~with~the~pill?", 8).AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                                }

                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                            data.AddOption("Any~news~about~the~Azure~Pill?", 14).AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                            data.AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                        }

                        break;
                    }
                case 14:
                    {
                        data.AddText("Finally, the Azure Pill is done. Hurry up, bring it to Scholar Wu.");
                        data.AddOption("I`ll~set~off~right~away.", 15).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        if (!client.Inventory.Contain(721698, 1))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.");
                                data.AddOption("Let me check.", 255).AddAvatar(62).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 721698);
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~got~the~Azure~Pill!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest4.MissionId, 1);

                        break;
                    }
                case 9:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).AddAvatar(62).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId);
                        data.AddText("Haha, the Azure Pill is done!");
                        data.AddOption("Perfect!", 10).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        data.AddText("Finally, the Azure Pill is done. Hurry up, bring it to Scholar Wu");
                        data.AddOption("I`ll~set~off~right~away.", 11).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Inventory.Add(stream, 721698);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~got~the~Azure~Pill!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest4.MissionId, 1);

                        break;
                    }
                case 8:
                    {
                        data.AddText("Don`t bother me, right now. Get some manners and be patient, OK?");
                        data.AddOption("I`ll~wait.", 255).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Oh, Scholar Wu`s disease... It`s difficult to cure. I`ll try my best, but I`ll need your help gathering things I need.");
                        data.AddOption("Go~ahead.", 2).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Scholar Wu is suffering from a rare asthma. Only the Azure Pill can cure it. I need 1 Ratling Eye, 3 Ratling Meat and")
                                .AddText("~2 Herb Packs. Kill Ratlings and you`ll have a chance to get what I need. You can ask Bian Qing (Twin City 342,232) for the packs.");
                        data.AddOption("I`m~on~my~way.", 3)
                            .AddOption("None~of~my~business.", 255).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Hurry~to~collect~1~Ratling~Eye,~3~Ratling~Meat,~and~buy~2~Herb~Packs~from~Bian~Qing~(Twin~City~342,232)!");

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 3, 2))
                        {
                            data.AddText("Excellent! All of the ingredients are here. Now leave me alone for a while so I can combine them into an Azure Pill.");
                            data.AddOption("Cool.", 5).AddAvatar(62).FinalizeDialog();


                            break;
                        }
                        data.AddText("Now, bring me 1 Ratling Eye and 3 Ratling Meat. Make sure to get the 2 Herb Packs from Bian Qing (342,232).");
                        data.AddOption("I~see", 255).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (HasItems(client, 721695, 1) && HasItems(client, 721696, 3) && HasItems(client, 721697, 2) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(721695, 1, stream)) break;
                            if (!client.Inventory.Remove(721696, 3, stream)) break;
                            if (!client.Inventory.Remove(721697, 2, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("It will take a while to make the Azure Pill. Be patient.");
                        data.AddOption("Alright.", 6).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.AzurePillStamp = DateTime.Now.AddSeconds(30);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Just~give~me~30~seconds!");

                        ActionQuery action = new ActionQuery();
                        action.ObjId = client.Player.UID;
                        action.Type = ActionType.CountDown;
                        action.dwParam = (uint)30;
                        client.Send(stream.ActionCreate(&action));

                        break;
                    }
            }
        }
        //Level43UniqueRingPack
        [NpcAttribute(NpcID.Level43UniqueRingPack)]
        public static void Level43UniqueRingPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            switch (Option)
            {
                case 1:
                    {
                        if (!client.Inventory.Remove(728238, 1, stream)) break;
                        client.Inventory.Add(stream, 151077);
                        client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Ivory~Heavy~Ring!");

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(728238, 1, stream)) break;
                        client.Inventory.Add(stream, 150077);
                        client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Amethyst~Ring!");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ApprenticeLuo)]
        public static void ApprenticeLuo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1733);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ApprenticeLuo, client.Player.Class, 1734);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ever wonder who makes the best arrows in the world? It`s my dad!")
                                .AddText("~Those arrows... So delicate, but they can fly so steadily...");
                            data.AddOption("Your~dad?", 1).AddOption("You~are~adorable.", 255).AddAvatar(68).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Have you met my father on the Wind Plain (442,431)? I`m sure he`ll")
                                    .AddText("~bring back lots of beautiful feathers.");
                                data.AddOption("Yeah.", 255).AddAvatar(68).FinalizeDialog();
                                break;
                            }
                            data.AddText("Have you ever met my father, Artisan Luo? He makes arrows, and has been off at the Wind Plain Bridge")
                            .AddText("~(442,431) collecting feathers. It usually only takes him one or two days, but it`s been almost a week")
                            .AddText("~since he left, this time. My mother and I are so very worried. I hope he`s alright...");
                            data.AddOption("River?", 2).AddOption("Sorry,~I~can`t~help.", 255).AddAvatar(68).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Have you met my father on the Wind Plain (442,431)? I`m sure he`ll")
                                   .AddText("~bring back lots of beautiful feathers.");
                            data.AddOption("Yeah.", 255).AddAvatar(68).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Have you ever met my father, Artisan Luo? He makes arrows, and has been off at the Wind Plain Bridge")
                              .AddText("~(442,431) collecting feathers. It usually only takes him one or two days, but it`s been almost a week")
                              .AddText("~since he left, this time. My mother and I are so very worried. I hope he`s alright...");
                        data.AddOption("River?", 2).AddOption("Sorry,~I~can`t~help.", 255).AddAvatar(68).FinalizeDialog();

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
                        data.AddText("He should be near the river between the Peach Blossom Garden and Wind Plain. There are many birds roaming around.")
                             .AddText("~My dad used to take me there to collect feathers, so I`m sure that`s where he`ll be.");
                        data.AddOption("Got~it.", 3).AddOption("Sorry,~I~can`t~help.", 255).AddAvatar(68).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~the~Riverside~(442,431)!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ArtisanLuo)]
        public static void ArtisanLuo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1010);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ApprenticeLuo, client.Player.Class, 1734);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1735);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1736);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1737);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                  .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                                .AddOption("I~came~for~your~son.", 10)
                                .AddOption("Oh~no,~thanks.", 255)
                                .AddAvatar(51).FinalizeDialog();

                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                              .AddOption("I`ve~got~the~feathers.", 11)
                              .AddOption("Oh~no,~thanks.", 255)
                              .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                               .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                             .AddOption("I`ve~got~the~Vulture~Feather.", 13)
                             .AddOption("Oh~no,~thanks.", 255)
                             .AddAvatar(51).FinalizeDialog();


                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                               .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                             .AddOption("Let~me~know~if~you~need~help.", 16)
                             .AddOption("Oh~no,~thanks.", 255)
                             .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")

                              .AddOption("Oh~no,~thanks.", 255)
                              .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                            {
                                data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                    .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                              .AddOption("Feather~Collection.", 4)
                              .AddOption(" Oh~no,~thanks.", 255)
                              .AddAvatar(51).FinalizeDialog();
                            }
                            else
                            {

                                data.AddText(" I ran out of the materials I use to make arrows. Can you hunt the Robins and collect 10 Robin Feathers for me?")
                               .AddOption("Here~you~are!", 3)
                               .AddOption("I`ll~go~now.", 255)
                               .AddAvatar(51).FinalizeDialog();
                            }
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                .AddText("~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                                .AddOption("Feather~Collection.", 1)
                                .AddOption("Oh~no,~thanks.", 255)
                                .AddAvatar(51).FinalizeDialog();
                            break;
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Oh, my son is looking for me? Alright, I`ll be able to head home as soon as I can gather enough feathers.")
                          .AddOption("You~crazy.", 11)
                          .AddOption("Fine.", 255)
                          .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 100))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                            }
                            data.AddText("That`s very kind of you! Thank you!")
                                .AddOption("No~problem.", 13)
                                .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        data.AddText("Ouch...my foot! I accidentally sprained my ankle while I was trapping a gigantic bird, so it`s hard for")
                            .AddText("~me to keep collecting feathers. Would you be so kind as to help me collect 100 feathers?")
                            .AddText("~I would really appreciate your help.")
                         .AddOption("My~pleasure.", 12)
                         .AddOption("Sorry,~I~can`t~help.", 255)
                         .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            //You`ve~found~a~Vulture~Feather.
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                data.AddText("Thank you for helping me! Take this as your reward. You deserve it.")
                                    .AddOption("My~pleasure.", 16)
                            .AddAvatar(51).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                data.AddText("The Vultures are nesting in the Love Canyon, but be careful! They`re very dangerous.").AddOption("I~see.", 255)
                              .AddAvatar(51).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("One last thing... I`ve been dreaming of getting my hands on some of the native vulture`s feathers.")
                            .AddText("~Perhaps you could help me out one more time?")
                                  .AddOption("I`ll~try~my~best.", 14).AddOption("I`m~busy,~you~know.", 255)
                                  .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        if (HasItems(client, 729976, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(729976, 1, stream)) break;
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                            client.SendSysMesage("You've received EXP worth 1/6 EXP Ball and 1,500 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Thank you for what you`ve done for me! Please pay a visit to Taskmaster Chang and claim your reward.")
                               .AddOption("I~will.", 255)
                       .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        data.AddText("Thank you for helping me. Now you can go pick a reward from Taskmaster Chang.")
                                .AddOption("Really?~I`m~on~my~way.", 17)
                                .AddOption("Got~it.", 255)
                        .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 17:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~claim~your~reward~from~Taskmaster~Chang.~It`s~a~weapon!", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
                case 14:
                    {
                        data.AddText("You certainly deserve your good reputation! I heard the strange birds from the Love Canyon can cast magic spells")
                         .AddText("~and turn human beings into monkeys! I could never risk going, but maybe you could? Do you think you could collect")
                         .AddText("~a feather from one of the vultures for me?")
                               .AddOption("You~can~count~on~me.", 15).AddOption("I`m~busy,~you~know.", 255)
                               .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~the~Love~Canyon~to~hunt~the~Vultures.", 1020, 586, 771, 0);

                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Hurry~to~collect~100~feathers!");

                        break;
                    }
                case 4:
                    {
                        data.AddText("I ran out of the materials I use to make arrows. Can you hunt the Robins and collect 10 Robin Feathers for me?")
                                 .AddOption("Here~you~are!", 5)
                                 .AddOption("I`ll~go~now.", 255)
                                 .AddAvatar(51).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You~finished~the~quest~Feather~Collection~and~received~400~silver~and~EXP~worth~1/15~of~an~EXP~Ball.");
                            client.Player.Money += 400;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(10, true, Role.Flags.ExperienceEffect.angelwing);
                            data.AddText("Thank you very much for collecting the Robin Feathers for me! I can make good arrows with it!")
                               .AddOption("You~are~welcome.", 255)
                               .AddAvatar(51).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Hey, I don`t see any feathers on you. Don`t cheat me!")
                               .AddOption("I`ll~go~now.", 255)
                               .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText(" Hi! I`m the fletcher of Twin City. It`s my job to make exquisite arrows. Now, I`m out of the material I need to make arrows. Can you")
                              .AddText("~hunt some Robins and collect 10 Robin Feathers for me?")
                              .AddText("~I need those feathers, or I can`t make any arrows!")
                              .AddOption("No~problem.", 2)
                              .AddOption("I~have~other~things~to~do.", 255)
                              .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~hunt~Robins~and~collect~10~Robin~Feathers.", 1002, 577, 718, 0);


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ShenJunior)]
        public static void ShenJunior(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1742);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("So my father asked you to visit me? It is very kind of you. I really miss him.")
                               .AddOption("You~need~a~Gate~scroll?", 1)
                               .AddOption("Never~mind.", 255)
                               .AddAvatar(27).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("So my father asked you to visit me? It is very kind of you. I really miss him.")
                             .AddOption("Never~mind.", 255)
                             .AddAvatar(27).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Thank you for your help. This means a lot to me, since I`ll finally be able to go home and get a break!")
                             .AddOption("Take~care.", 2)
                             .AddAvatar(27).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        if (!client.Inventory.Remove(721710, 1, stream)) break;
                        client.Player.QuestGUI.SendAutoPatcher(1011, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DealerShen)]
        public static void DealerShen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Minstrel, client.Player.Class, 1739);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1740);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1741);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1742);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1743);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                               .AddOption("Are~you~Dealer~Shen?", 1)
                               .AddOption("That`s~interesting.", 255)
                               .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                             .AddOption("I`ve~got~what~you~need.", 3)
                             .AddOption("That`s~interesting.", 255)
                             .AddAvatar(85).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                      && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                             .AddOption("I~finished~the~Ratlings.", 5)
                             .AddOption("That`s~interesting.", 255)
                             .AddAvatar(85).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                     && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                             .AddOption("I`m~always~at~your~service.", 7)
                             .AddOption("That`s~interesting.", 255)
                             .AddAvatar(85).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                  && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                             .AddOption("Your~son~is~back.", 10)
                             .AddOption("That`s~interesting.", 255)
                             .AddAvatar(85).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                          .AddOption("That`s~interesting.", 255)
                          .AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes! Nice to see you! I love collecting antiques. They`ve been my biggest passion in life.")
                              .AddOption("I~want~your~Antique~Ring.", 2)
                              .AddAvatar(85).FinalizeDialog();

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
                        data.AddText("Oh, I could have helped you... But... you know... the Antique Ring... that`s valuable...")
                         .AddOption("What~do~you~want?", 3)
                         .AddOption("Never~mind.", 255)
                         .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 12, 12))
                        {
                            data.AddText("Thank you. I`ll get them to take their medicine, now.")
                        .AddOption("Take~your~time.", 5)
                        .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        data.AddText("You`re smart. To be honest with you, I could use some help. I need 12 Stanchers and 12 Agrypnotics to help with my family`s illness.")
                            .AddText("~You can buy them from the Pharmacist, but it`s too far away. I need to stay close to my family, right now.")
                      .AddOption("I`ll~be~right~back.", 4)
                      .AddOption("Later.", 255)
                      .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            foreach (var item in client.Inventory.ClientItems.Values)
                            {
                                if (item.ITEM_ID == 1000000)
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 1);
                                else if (item.ITEM_ID == 1001000)
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 0, 1);
                            }
                        }
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~buy~12~Stanchers~and~12~Agrypnotics!", 1011, 189, 252, 10014);

                        break;
                    }
                case 5:
                    {
                        if (HasItems(client, 1000000, 12) && HasItems(client, 1001000, 12) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(1000000, 12, stream)) break;
                            if (!client.Inventory.Remove(1001000, 12, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 20))
                            {
                                data.AddText("Thank you for what you`ve done for the village. I won`t soon forget your heroism!")
                                   .AddOption("You`re~welcome.", 7)
                                   .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("How come you didn`t kill enough Ratlings?")
                                    .AddOption("Sorry,~I`ll~try~my~best.", 255)
                                    .AddAvatar(85).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("Oh, thank you ,thank you... but... Will you please kill 20 Ratlings, too? They are really quite annoying, nibbling our food all the time.")
                       .AddOption("Okay.", 6)
                       .AddOption("It`s~boring.", 255)
                       .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~kill~20~Ratlings!", 1011, 610, 429, 0);

                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                data.AddText("My son is back! How can I thank you enough!")
                   .AddOption("You`re~welcome.", 10)
                   .AddAvatar(85).FinalizeDialog();
                                break;
                            }
                            data.AddText("Have you heard the news? Recently, many people were robbed by the Bandits who hide in Maple Forest.")
                           .AddText("~I`m worried about my son. He`s working inside the Mine Cave (116,164), so he doesn`t get much news from outside.")
                   .AddOption("I`ll~take~him~home.", 255)
                   .AddOption("I~lost~the~scroll.", 9)
                   .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        data.AddText("Can you please head into Maple Forest and deliver this Castle Gate Scroll to my son?")
                            .AddText("~His name is Shen Junior, and he`s doing excavation work inside the Mine Cave (116,164).")
                    .AddOption("Sure.", 8)
                    .AddOption("Not~now.", 255)
                    .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721710, 1))
                            client.Inventory.Add(stream, 721710);

                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Deliver~the~Castle~Gate~Scroll~to~Shen~Junior!", 1011, 920, 552, 4502);

                        break;
                    }
                case 9:
                    {
                        if (!client.Inventory.Contain(721710, 1))
                        {
                            if (client.Inventory.HaveSpace(1) == false)
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(85).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 721710);
                            client.CreateBoxDialog("Deliver~the~Castle~Gate~Scroll~to~Shen~Junior!");
                        }
                        else
                        {
                            data.AddText("You have the Scroll. Hurry up and deliver it to my son.");
                            data.AddOption("Okay.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("With my family finally together and safe, the ring is not important to me, anymore. Please, accept it as your reward.");
                        data.AddOption("Thank~you.", 11)
                            .AddOption("I`ll~take~it~later.", 255).AddAvatar(85).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721708, 1))
                            client.Inventory.Add(stream, 721708);
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~got~the~Antique~Ring.~Hurry~and~bring~it~to~the~Minstrel.", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Minstrel)]
        public static void Minstrel(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1738);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Minstrel, client.Player.Class, 1739);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1743);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Minstrel, client.Player.Class, 1744);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                               .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                               .AddOption("Taskmaster~Chang~sent~me~here.", 1)
                               .AddOption("Uh...~Are~you~okay?", 255)
                               .AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                               .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                               .AddOption("Anything~I~can~help~with?", 2)
                               .AddOption("Uh...~Are~you~okay?", 255)
                               .AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                        .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                        .AddOption("What~about~the~Mystic~Ring?", 4)
                        .AddOption("Interesting.", 255)
                        .AddAvatar(40).FinalizeDialog();

                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                       && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                               .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                               .AddOption("Anything~I~can~help~with?", 7)
                               .AddOption("Uh...~Are~you~okay?", 255)
                               .AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                            .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                            .AddOption("Uh...~Are~you~okay?", 255)
                            .AddAvatar(40).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        data.AddText("I heard Antique Dealer Shen has an Antique Ring which could be converted into Mystic Ring. But seems that I can`t even see it...")
                              .AddOption("What~a~pity!", 2)
                              .AddAvatar(40).FinalizeDialog();

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
                        data.AddText("A little bird told me an antique dealer named Shen (779,477) is living in a village near Phoenix Castle, and has an Antique Ring. His")
                            .AddText("~ring matches the description of the Mystic Ring! I must satisfy my curiosity and find out if it`s real! Will you check it out for me?")
                             .AddOption("No~problem.", 3)
                             .AddOption("Why~me?", 255)
                             .AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Ask~Dealer~Shen~(Phoenix~Castle~779,477)~about~the~Antique~Ring.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
                case 4:
                    {

                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 1))
                            {
                                data.AddText("Oh gosh, the Mystic Ring... it`s no different than an ordinary one. But you truly did a great thing.")
                                    .AddText("~So, my hero, will you wear this ring and show me its power?");
                                data.AddOption("Of~course.", 7).AddAvatar(40).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please use the AntiqueRing.");
                                data.AddOption("Let me check.", 255).AddOption("I~lost~the~Broken~Ring.", 8).AddAvatar(40).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Finally, the Mystic Ring! Hats off to your gallantry and valor! Now, will you")
                        .AddText("~do me another favor and try it on? I want to see its legendary power unleashed!")
                        .AddOption("Sure.", 5)
                        .AddOption("Sorry,~I~have~to~go.", 255)
                        .AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Finally, the Mystic Ring! I can`t wait to see its magic! Please, wear it and show me its power! Right now!")
                       .AddOption("Fine.", 6)
                       .AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        if (!client.Inventory.Remove(721708, 1, stream)) break;
                        client.Inventory.Add(stream, 721707);
                        client.CreateBoxDialog("You~found~the~ring~is~broken!");

                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                            client.SendSysMesage("You've received EXP worth 1/6 EXP Ball and 1,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you for what you`ve done for me! Now you can report to Taskmaster Chang and claim your reward.")
                            .AddOption("I`ll~go~right~away.", 9)
                            .AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.Contain(721707, 1))
                        {
                            if (client.Inventory.HaveSpace(1) == false)
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(85).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 721707);
                            client.CreateBoxDialog("You~found~the~ring~is~broken!");
                        }
                        else
                        {
                            data.AddText("You have the ring.");
                            data.AddOption("Okay.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~claim~your~reward~from~Taskmaster~Chang.~It`s~some~headgear!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.DoctorLi)]
        public static void DoctorLi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1747);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorLi, client.Player.Class, 1748);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorLi, client.Player.Class, 1749);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What are you doing here? Don`t you ever mess around with me... I`m old, but not foolish...")
                               .AddOption("You~know~the~frantic~monkeys?", 1)
                               .AddOption("I`ll~leave~you~alone.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("What are you doing here? Don`t you ever mess around with me... I`m old, but not foolish...")
                             .AddOption("What~shall~I~do~now?", 4)
                             .AddOption("I`ll~leave~you~alone.", 255)
                             .AddAvatar(67).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("What are you doing here? Don`t you ever mess around with me... I`m old, but not foolish...")
                             .AddOption("I`m~back.", 7)
                             .AddOption("I`ll~leave~you~alone.", 255)
                             .AddAvatar(67).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What are you doing here? Don`t you ever mess around with me... I`m old, but not foolish...")
                          .AddOption("I`ll~leave~you~alone.", 255)
                          .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("What? Monkeys are attacking people! I`ve never heard that before!")
                             .AddOption("But~it`s~real.", 2)
                             .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure it`s monkey`s heart? As far as I know, it`s posioned!")
                              .AddOption("Any~solution?", 3)
                              .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (!HasItems(client, 721699, 1))
                            break;
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (!client.Inventory.Remove(721699, 1, stream)) break;
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721700, 1))
                            client.Inventory.Add(stream, 721700);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Doctor~Li~gave~you~a~bottle~of~poison.");

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5))
                        {
                            data.AddText("You did go shockingly frantic! Well, your syndrome reminded me of the bites by Venomous Apes.")
                            .AddOption("I~see.", 7)
                            .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Look at this poison...I need someone to test it...")
                            .AddText("~Will you do me this favor?")
                                .AddOption("Sure.", 5)
                                .AddOption("I~lost~the~posion.", 6)
                                .AddOption("Not~me...", 255)
                                .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.OnTransform == false)
                        {
                            client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                            client.Player.TransformInfo.CreateTransform(stream, 817, 356, (int)60, 8213);
                        }
                        else
                        {
                            client.CreateBoxDialog("You~turned~yourself~into~a~gigantic~monkey.~You~can~take~the~poison~now.");
                        }

                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.Contain(721700, 1))
                        {
                            if (client.Inventory.HaveSpace(1) == false)
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 721700);
                            client.CreateBoxDialog("You~turned~yourself~into~a~gigantic~monkey.~You~can~take~the~poison~now.");
                        }
                        else
                        {
                            data.AddText("Hey! You still have the poison!");
                            data.AddOption("Okay.", 255).AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("The Venomous Apes are harassing people in Love Canyon (795,471) on the outskirts of Ape City.")
                            .AddText("~Please take this Metamorphic Vial along. It`s made of a secret recipe. Use it when you arrive there.")
                           .AddOption("I`m~on~my~way.", 8)
                           .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721701, 1))
                            client.Inventory.Add(stream, 721701);
                        data.AddText("Venomous Apes live in Love Canyon (795,471). People will turn frantic after being bitten by them. Search there for clues.")
                        .AddOption("I~will.", 9)
                        .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 795, 471, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.OfficerBao)]
        public static void OfficerBao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1745);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1746);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1747);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorLi, client.Player.Class, 1749);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1750);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                               .AddText("~What a peaceful view... Or is it?...")
                               .AddOption("Taskmaster~Chang~sent~me~here.", 1)
                               .AddOption("You~really~did~a~good~job.", 255)
                               .AddAvatar(93).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                           .AddText("~What a peaceful view... Or is it?...")
                           .AddOption("About~the~Decayed~Heart.", 3)
                           .AddOption("You~really~did~a~good~job.", 255)
                           .AddAvatar(93).FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                         && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                           .AddText("~What a peaceful view... Or is it?...")
                           .AddOption("How~to~deal~with~the~heart?", 4)
                           .AddOption("You~really~did~a~good~job.", 255)
                           .AddAvatar(93).FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                         .AddText("~What a peaceful view... Or is it?...")
                         .AddOption("I~found~this~sword.", 7)
                         .AddOption("You~really~did~a~good~job.", 255)
                         .AddAvatar(93).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                       && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                           .AddText("~What a peaceful view... Or is it?...")
                           .AddOption("Was~there~anything~else?", 8)
                           .AddOption("You~really~did~a~good~job.", 255)
                           .AddAvatar(93).FinalizeDialog();

                        }
                        else
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                         .AddText("~What a peaceful view... Or is it?...")
                         .AddOption("You~really~did~a~good~job.", 255)
                         .AddAvatar(93).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("It`s weird that the monkeys outside the city strangely went frantic and began to attack villagers...")
                              .AddOption("Really?", 2)
                              .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Yes. You may have noticed those monkeys outside our city. They used to be benign, though sometimes a little bit naughty.")
                            .AddText("~But now they are attacking people... every one of them... Will you give me a hand and see what`s going on there?")
                           .AddOption("My~pleasure.", 3)
                           .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                data.AddText("Thank you for what you`ve done for the people!")
                      .AddOption("Never~mind.", 4)
                      .AddAvatar(93).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                data.AddText("Only a genuine brave heart can conquer all. Please go and find out what`s driving the monkeys berserk. Quick!")
                       .AddOption("I`ll~be~right~back.", 255)
                       .AddAvatar(93).FinalizeDialog();
                            }
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Nothing~ventured,~nothing~gained.~Go~find~reasons~why~the~monkeys~turned~frantic~so~suddenly.", 1020, 668, 673, 0);


                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Dear me! What a horrible heart! It`s decayed. I`ve never seen this kind of thing before. Will you go visit Royal Doctor Li")
                            .AddText("~in Twin City (296,243)? He`s an accomplished doctor. He must be of some help.")
                    .AddOption(" Good~idea.", 5)
                    .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hand~the~Decayed~Heart~to~Royal~Doctor~Li~in~Twin~City~(296,243).", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 7:
                    {
                        data.AddText("You got the Devilish Sword! I`m really impressed. This is your reward.")
                            .AddOption("Thank~you.", 8)
                            .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've received EXP worth 1/2 EXP Ball and 3,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you for restoring peace to our city. Now you can go claim your reward from Taskmaster Chang.")
                           .AddOption("I`m~on~the~way.", 9)
                           .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`re~heading~to~see~Taskmaster~Chang.", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralAmber)]
        public static void GeneralAmber(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1751);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1752);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1753);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1754);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1755);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuardLi, client.Player.Class, 1756);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                            .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                            .AddOption("Was~there~anything~else?", 20)
                            .AddOption("Marvellous!", 255)
                            .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                               .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                               .AddOption("Taskmaster~Chang~sent~me~here.", 1)
                               .AddOption("Marvellous!", 255)
                               .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                              .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                              .AddOption("Let~me~help~you.", 3)
                              .AddOption("Marvellous!", 255)
                              .AddAvatar(91).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                            .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                            .AddOption("I~can~help~you.", 5)
                            .AddOption("Marvellous!", 255)
                            .AddAvatar(91).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                            .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                            .AddOption("You~said~the~portrait?", 8)
                            .AddOption("Marvellous!", 255)
                            .AddAvatar(91).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                            .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                            .AddOption("Here`s~the~portrait.", 10)
                            .AddOption("Marvellous!", 255)
                            .AddAvatar(91).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                         .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                         .AddOption("Marvellous!", 255)
                         .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("Let me check.", 255)
                              .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            data.AddText("You`ve got the Paper Figure. Hurry to find General Zhao!")
                              .AddOption("I`m~on~my~way.", 255)
                              .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("Haha! Thank you for allowing me to control you like a pawn! I could not have taken my revenge without you. General Zhao")
                            .AddText("~is now captivated by my enchantment... He is now under my spell! My revenge is complete, all thanks to you!")
                            .AddOption("Stop~it!~Release~him!", 21)
                            .AddOption("Awful.", 255)
                            .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        ActionQuery action;

                        action = new ActionQuery()
                        {
                            ObjId = id,
                            Type = ActionType.RemoveEntity
                        };
                        client.Send(stream.ActionCreate(&action));
                        if (client.Player.View.ContainMobInScreen("JadePhantom") == false)
                            Database.Server.AddMapMonster(stream, client.Map, 33212, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        client.CreateBoxDialog("General~Amber~vanished~into~thin~air.~A~vicious~Jade~Phantom~appeared...");

                        break;
                    }
                case 1:
                    {
                        data.AddText("My soul could not rest in peace. I still have some unfinished wishes. Oh, how they are torturing me...")
                            .AddText("~I do need your help.")
                            .AddOption("What~is~it?", 2)
                            .AddOption("Why~me?", 255)
                            .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Find General Zhao (262,247) in Twin City. We used to be best friends. But we had a big fight over some rift")
                         .AddText("~and swore never to meet again. But now... I miss him. Will you take a tuft of Hair of him for me as a token of peace?")
                         .AddOption("Okay.", 3)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                data.AddText("Oh, my, General Zhao`s Hair... This really reminds me of those days... when we were fighting side by side...")
                                    .AddText("~Will you help me complete my other wishes, too?")
                                  .AddOption("Not~right~now.", 5)
                                   .AddAvatar(91).FinalizeDialog();
                                break;
                            }
                            data.AddText("Pity! I can`t go visit General Zhao (262,247) myself. I do hope everything is fine with him. Will you please bring me")
                                .AddText("~a tuft of his Hair?")
                               .AddOption("Okay.", 255)
                                .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("My soul could not rest in peace. I still have some unfinished wishes. Oh, how they are torturing me...")
                    .AddText("~I do need your help.")
                     .AddOption("I`d~like~to.", 4)
                     .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (HasItems(client, 721703, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(721703, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I need 1 Snake Man, 10 Rock Monsters (Level 42) and 10 Giant Apes (Level 53) Smells. Urr... Don`t ask me why. It`s like a")
             .AddText("~BIG surprise! But I will sure repay you handsomely... So, is it a deal?")
              .AddOption("I~need~a~Silk~Pack~for~this.", 7)
              .AddOption("You~crazy.", 255)
              .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("I need 1 Snake Man, 10 Rock Monsters (Level 42) and 10 Giant Apes (Level 53) Smells. Urr... Don`t ask me why. It`s like a")
                .AddText("~BIG surprise! But I will sure repay you handsomely... So, is it a deal?")
                 .AddOption("Deal.", 6)
                 .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 10, 10))
                        {
                            data.AddText("I didn`t expect you would return so soon. This is your reward. Please accept it.")
                               .AddOption("Okay.", 8)
                               .AddAvatar(91).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hey you, one Silk Pack is enough.")
                                .AddOption("Got~it.", 255)
                                .AddAvatar(91).FinalizeDialog();

                        }

                        break;
                    }
                case 8:
                    {
                        if (HasItems(client, 721704, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(721704, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                //721705
                                data.AddText("Fantastic! So true to life! But he doesn`t look so well does he? General Zhao looks unhappy... Would you pay him a")
                                    .AddText("~visit for me? I am so worried...")
                                  .AddOption("Okay.", 10)
                                  .AddOption("Later.", 255)
                                  .AddAvatar(91).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Only the Painter Feng Kang (556,571) in Ape City can paint the best portrait. No more waste of time, please.")
                                   .AddOption("All~right.", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Ahhh, excellent. You have done well, and I commend you. But, if it`s not too much too ask, I still have one more wish. Will")
                            .AddText("~you find Painter Feng Kang (556,571) in Ape City and ask him to paint a portrait of General Zhao?")
                            .AddText("~I`d love to have such a painting...")
                               .AddOption("Well...~okay~then.", 9)
                               .AddOption("No~way.", 255)
                               .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (HasItems(client, 721705, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(721705, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                            goto case 11;
                        }
                        else
                        {
                            data.AddText("Fantastic! So true to life! But he doesn`t look so well does he? General Zhao looks unhappy... Would you pay him a")
                                    .AddText("~visit for me? I am so worried...")
                                  .AddOption("Okay.", 11)
                                  .AddOption("Later.", 255)
                                  .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You~have~agreeed~to~pay~a~visit~to~General~Zhao~for~General~Amber.", 1002, 262, 247, 4488);

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Find~Painter~Feng~Kang~(556,571)~in~Ape~City~and~paint~a~full~length~Portrait.", 1020, 556, 571, 4489);

                        //Find~Painter~Feng~Kang~(556,571)~in~Ape~City~and~paint~a~full~length~Portrait.
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("Let me check.", 255)
                              .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 721704);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Kill 1 Snake Man, 10 Rock Monsters (Level 42) and 10 Giant Apes (Level 53).");


                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Find~General~Zhao~(262,247)~in~Twin~City~and~take~back~a~tuft~of~his~hair~to~General~Amber.", 1002, 262, 247, 4488);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GuardLi)]
        public static void GuardLi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1755);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuardLi, client.Player.Class, 1756);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I will ward off any danger away from General Zhao.")
                               .AddOption("About~General~Zhao.", 1)
                               .AddOption("Good~for~you.", 255)
                               .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                  && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I will ward off any danger away from General Zhao.")
                       .AddOption("What`s~wrong~with~Zhao?", 1)
                       .AddOption("Good~for~you.", 255)
                       .AddAvatar(30).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I will ward off any danger away from General Zhao.")
                               .AddOption("Good~for~you.", 255)
                               .AddAvatar(30).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }

                        data.AddText("Wake up! Don`t you realize now? The hair, the silk pack, and the portait...they are all for an enchantment! General Zhao has been")
                            .AddText("~bewitched! How could you take them for granted?! The man you helped is not real General Amber! You must return and kill that horrid phantom!")
                             .AddOption("I`ll~try~my~best.", 2)
                             .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~back~to~kill~the~phantom!", 1020, 865, 540, 4487);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PainterFengKang)]
        public static void PainterFengKang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1754);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Art is not only for art`s sake.")
                               .AddOption("I~came~for~a~portrait.", 1)
                               .AddOption("True.", 255)
                               .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Art is not only for art`s sake.")
                               .AddOption("True.", 255)
                               .AddAvatar(74).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721705, 1))
                        {
                            data.AddText("Why not send the Portrait to General Amber right now? What`s been dragging you?")
                                     .AddOption("I`ll~hurry~up.", 255)
                                     .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        data.AddText("The revenant of General Amber has sent you here for a Portrait of General Zhao? Ok, I will try my best... But this sure")
                            .AddText("~does sound suspicious!")
                                  .AddOption("Thank~you.", 2)
                                  .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Add(stream, 721705);
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher("You~obtain~a~Portrait.~Hurry~up~to~give~it~to~General~Amber!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KungfuMaster)]
        public static void KungfuMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1752);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1755);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuardLi, client.Player.Class, 1756);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KungfuMaster, client.Player.Class, 1757);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Oh, yes. God, my head hurts. Ahhhh, my head!")
                               .AddOption("What`s~wrong~with~you?.", 15)
                               .AddOption("C`est~la~vie.", 255)
                               .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Oh, yes. God, my head hurts. Ahhhh, my head!")
                               .AddOption("You~must~be~crazy.", 13)
                               .AddOption("C`est~la~vie.", 255)
                               .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                               .AddOption("I~have~a~request.", 10)
                               .AddOption("C`est~la~vie.", 255)
                               .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Oh, yes. God, my head hurts. Ahhhh, my head!")
                              .AddOption("Are~you~OK~now?.", 16)
                              .AddOption("C`est~la~vie.", 255)
                              .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(802))
                        {
                            data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                                .AddOption("Tell~me~about~the~team~system.", 1)
                                .AddOption("See~you~then!", 255)
                                .AddAvatar(104).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(807, MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                          .AddOption("Tell~me~about~the~team~system.", 2)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(104).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                                .AddOption("C`est~la~vie.", 255)
                                .AddAvatar(104).FinalizeDialog();
                        }

                        break;
                    }
                case 15:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                        {
                            data.AddText("What`s wrong with me? It seemed I`ve had a lot of nightmares these days... I see, it`s a spell... an enchantment... You")
                          .AddText("~have defeated Jade Phantom and it turned into a Paper Figure? I really appreciate your help. Thank you, and please take this.")
                              .AddOption("My~pleasure.", 16)
                              .AddOption("Hmm...", 255)
                              .AddAvatar(104).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.QuestGUI.SendAutoPatcher("Go~back~to~kill~the~phantom!", 1020, 865, 540, 4487);
                        }

                        break;
                    }
                case 16:
                    {
                        if (HasItems(client, 721706, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(721706, 1, stream)) break;
                            client.Player.Money += 4000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've received EXP worth 2/3 EXP Ball and 4,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you, for saving my life! You can go claim your reward from Taskmaster Chang!")
                         .AddText("~have defeated Jade Phantom and it turned into a Paper Figure? I really appreciate your help. Thank you, and please take this.")
                             .AddOption("I~will", 17)
                             .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 17:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~completed~the~quest!~Hurry~to~claim~your~reward~from~Taskmaster~Chang!", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);


                        break;
                    }
                case 13:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher("General~Zhao~is~delirious.~Maybe~Guard~Li~(261,243)~will~know~why.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(721703, 1))
                        {
                            data.AddText("Didn`t I just give you my Hair?")
                               .AddOption("Oh,~yeah.", 255)
                               .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        data.AddText("Oh... General Amber missed me... I`m surprised... He always hated me so much. What would make him change his mind?")
                            .AddText("~Take this tuft of Hair back to him, as a token of trust.")
                                .AddOption("Thank~you.", 11)
                                .AddOption("Hmm...", 255)
                                .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("Let me check.", 255)
                              .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 721703);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher("You~got~General~Zhao`s~Hair.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(802))
                            client.GainExpBall(200);
                        data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                                .AddOption("Tell~me~about~the~team~system.", 2)
                                .AddOption("See~you~then!", 255)
                                .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("You can`t just venture out alone in this cruel world, the team system will help you to find some companions to help you! There are two methods to help you to find companions. The first one is to click team button to create a team by yourself and invite")
                            .AddText("~other people to join.")
                            .AddOption("What~about~the~other~one?", 3)
                            .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("The second method is to click team button and click join to apply to join a team. You can join the team if there are less than 5 members in it, after the team leader agrees.")
                           .AddOption("What~are~the~benefits?", 4)
                           .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("You will receive bonus experience when slaying monsters in a team. If there is a new player in the team, the team leader will receive Virtue Points when that player levels up. Furthermore, some special quests require players to join as a team.")
                             .AddOption("Great!", 5)
                             .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("There are many things for you to learn in this world! Um, I guess you want to learn more, huh? Let me see. What about equipment and armors? That will be useful! Now speak with the Armor in Twin City, he will tell you something about the armors.")
                       .AddOption("Wonderful!", 6)
                       .AddOption("Other~things!", 255)
                       .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        var next_quest = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
                        if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            next_quest = Database.QuestInfo.GetFinishQuest((uint)2001, client.Player.Class, 3750);
                        client.Player.QuestGUI.Accept(next_quest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("General~Zhao~asked~you~to~speak~with~the~Armorer~in~Twin~City!", next_quest.FinishNpcId.Map, next_quest.FinishNpcId.X, next_quest.FinishNpcId.Y, next_quest.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PurplePowder)]
        public static void PurplePowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729989, 1))
                            {
                                client.Inventory.Add(stream, 729989);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Indigo~Purple~from~this~purple~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an purple halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an purple halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.IndigoPowder)]
        public static void IndigoPowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729988, 1))
                            {
                                client.Inventory.Add(stream, 729988);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Indigo~Powder~from~this~indigo~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an indigo halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an indigo halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BluePowder)]
        public static void BluePowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729987, 1))
                            {
                                client.Inventory.Add(stream, 729987);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Blue~Powder~from~this~blue~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an blue halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an blue halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GreenPowder)]
        public static void GreenPowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729986, 1))
                            {
                                client.Inventory.Add(stream, 729986);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Green~Powder~from~this~green~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an yellow halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an green halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YellowPowder)]
        public static void YellowPowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729985, 1))
                            {
                                client.Inventory.Add(stream, 729985);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Yellow~Powder~from~this~yellow~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an yellow halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an red yellow around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OrangePowder)]
        public static void OrangePowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729984, 1))
                            {
                                client.Inventory.Add(stream, 729984);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Orange~Powder~from~this~orange~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an orange halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an red orange around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PharmacistDong)]
        public static void PharmacistDong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralJudd, client.Player.Class, 1784);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I need someone to kill those monsters. They must pay for their sins.")
                               .AddOption("Snake~Men!", 1)
                               .AddOption("Not~me.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I need someone to kill those monsters. They must pay for their sins.")
                                .AddOption("About~the~Rainbow~Powder.", 2)
                                .AddOption("Not~interested.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("I need someone to kill those monsters. They must pay for their sins.")
                                .AddOption("Not~interested.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Snake Men? Yes, I`ve heard of it. People get lunatic after being bitten by them. It may have been because of their")
                            .AddText("~Venomous Fang. But a true doctor never makes wild guesses. Wait a moment. Let me extract the toxin from the")
                            .AddText("~fang and see what that is.")
                              .AddOption("Good.~A~real~diagnosis.", 2)
                              .AddOption("About~these~Snake~Men...", 255)
                              .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(3))
                            {
                                data.AddText("Please make 3 more spaces in your inventory.")
                                    .AddOption("Let me chieck.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729979, 1))
                                client.Inventory.Add(stream, 729979);
                            if (!client.Inventory.Contain(729980, 1))
                                client.Inventory.Add(stream, 729980);
                            if (!client.Inventory.Contain(729981, 1))
                                client.Inventory.Add(stream, 729981);

                            data.AddText("Herbs have a variety of uses, including culinary, medicinal, or in some cases even spiritual meditation.")
                              .AddText("~Nobody can truly master them, even after years of cultivation. Nature`s secrets are even more obscure than I could possibly imagine.")
                                .AddOption("You~bet.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Now we need Rainbow Powder to relieve Du San`s syndrome. It`s made of powder of seven different colors.")
                            .AddText("~I believe these can be found in this Kun Lun place... I`ll give you 3 toxins, and you can use them to reach Kun Lun.")
                            .AddText("~So, what`s your opinion?")
                              .AddOption("I`ll~do~it!", 3)
                              .AddOption("It`s~dangerous.", 255)
                              .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                                .AddOption("Let me chieck.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("No more delays. Take the toxins now. Pick up two of them and right click to use. If your choice is right, you will arrive in Kun Lun and")
                               .AddText("~find those rock powders... They must be some type of sedimentary rock... Go collect the powders and be careful!")
                                 .AddOption("Alright.", 255)
                                 .AddAvatar(67).FinalizeDialog();

                        client.Inventory.Add(stream, 729979);
                        client.Inventory.Add(stream, 729980);
                        client.Inventory.Add(stream, 729981);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Pharmacist~Dong~gave~you~3~toxins.");

                        //Pharmacist~Dong~gave~you~3~toxins.
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.RedPowder)]
        public static void RedPowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729983, 1))
                            {
                                client.Inventory.Add(stream, 729983);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }

                                client.CreateBoxDialog("You`ve~collected~Red~Powder~from~this~red~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an red halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an red halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralJudd)]
        public static void GeneralJudd(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DuSan, client.Player.Class, 1782);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralJudd, client.Player.Class, 1783);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralJudd, client.Player.Class, 1784);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralJudd, client.Player.Class, 710);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Don`t forget your country and people when you have established your name. This is what a responsible person should do. Your")
                                .AddText("~country needs you.")
                               .AddOption("I~have~no~time~to~play.", 2)
                               .AddOption("Brilliant.", 255)
                               .AddAvatar(95).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Don`t forget your country and people when you have established your name. This is what a responsible person should do. Your")
                              .AddText("~country needs you.")
                             .AddOption("News~about~Du~San.", 2)
                             .AddOption("Brilliant.", 255)
                             .AddAvatar(95).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
             && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Don`t forget your country and people when you have established your name. This is what a responsible person should do. Your")
                              .AddText("~country needs you.")
                             .AddOption("I`ve~got~the~fang.", 4)
                             .AddOption("Brilliant.", 255)
                             .AddAvatar(95).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Don`t forget your country and people when you have established your name. This is what a responsible person should do. Your")
                             .AddText("~country needs you.");
                            if (client.Player.Level >= 70 && !client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("How~to~devote~myself?", 20);
                            data.AddOption("Brilliant.", 255)
                            .AddAvatar(95).FinalizeDialog();
                        }

                        break;
                    }
                case 20:
                    {
                        data.AddText("The Emperor keeps track of all those who make donations to his empire. When you have reached level 70, you are able to donate");
                        data.AddText("~money to the Emperor for your nobility rank, which can increase your battle power. Your title is determined by the amount of");
                        data.AddText("~money you have donated.");
                        data.AddOption("How~to~donate~money?", 21);
                        data.AddAvatar(95);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        data.AddText("Click the Nobility Button, input the amount of money you want to donate in the box, and confirm your donation. If you don`t");
                        data.AddText("~have enough silver, you can exchange your CPs for silver to use.");
                        data.AddOption("I~see.", 22);
                        data.AddAvatar(95);
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        data.AddText("Here is a question for you. How much Battle Power can be added to King/ Queen ranked players?");
                        data.AddInput("(Answer:~12)", 23);
                        data.AddAvatar(95);
                        data.FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        if (Input == "12")
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.CreateBoxDialog("You`ve~learned~something~about~the~Nobility~Rankings~and~received~EXP~worth~1/6~of~an~EXP~Ball!");
                                client.GainExpBall(100);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Sorry,~but~you`ve~given~the~incorrect~answer.");
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("You mean you saw a potpourri bag just like this before? It must be my nephew, Du San. That poor child. He lost his parents")
                              .AddText("~when he was young, so I raised him up. But when he was in his twenties, he suddenly lost his mind...")
                              .AddText("~I guess I just couldn`t be there for him all the time...")
                             .AddOption("What~happened?", 2)
                             .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                data.AddText("Really? You`ve got some clues?")
                                .AddOption("Look~at~this~Venomous~Fang.", 4)
                                .AddAvatar(95).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need some one to kill those Snake Men. They must pay for their sins.")
                                 .AddOption("I`m~in.", 255)
                                 .AddAvatar(95).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("He was in Ape City, learning Kungfu. Then our servants reported that he was missing. Several days later,")
                               .AddText("~someone caught him on the street of Ape City, totally unkempt, babbling strange words... They found snake bites all")
                               .AddText("~over his body... Snake Men got to him. Damn those snake vermin! But nobody has made them pay for it.")
                              .AddOption("I`ll~avenge~him.", 3)
                              .AddOption("I`ll~think~about~it.", 255)
                              .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Snake~Men~are~in~the~west~of~Ape~City.~Kill~some~of~them~and~find~out~why~Du~San~went~lunatic.", 1020, 255, 504, 0);

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        data.AddText("Really! It`s about time! It must be those snake bites that made Du San go crazy! Please send this Venomous Fang to")
                            .AddText("~Pharmacist Dong (258,277) for diagnosis. We`ll find out for sure, then take revenge!")
                               .AddOption("Got~it.", 5)
                               .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Speack~with~Pharmacist~Dong~(258,277)~for~diagnosis.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DuSan)]
        public static void DuSan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1781);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DuSan, client.Player.Class, 1782);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DuSan, client.Player.Class, 1786);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                             .AddOption("I~have~no~time~to~play.", 10)
                             .AddOption("What~are~you~talking~about?", 255)
                             .AddAvatar(77).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
               && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                              .AddOption("I~have~no~time~to~play.", 13)
                              .AddOption("What~are~you~talking~about?", 255)
                              .AddAvatar(77).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                               .AddOption("I~have~no~time~to~play.", 1)
                               .AddOption("What~are~you~talking~about?", 255)
                               .AddAvatar(77).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                               .AddOption("I~have~no~time~to~play.", 1)
                               .AddOption("What~are~you~talking~about?", 255)
                               .AddAvatar(77).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                             .AddOption("What~are~you~talking~about?", 255)
                             .AddAvatar(77).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("History has forgotten those glorious celestials... So aloof and dreamy...")
                             .AddText("~Oh, Kun Lun, the Land of the Immortality... Why do I only have this potpourri to remember you by?")
                             .AddOption("Let~me~help~you.", 11)
                            .AddOption("Brilliant.", 255)
                            .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 0, 0, 0, 0, 0, 0, 0, 1))
                        {
                            data.AddText("Snakes! Help!")
                          .AddOption("Take~this~Rainbow~Powder.", 13)
                         .AddAvatar(95).FinalizeDialog();

                            break;
                        }
                        data.AddText("What`d happened to me?")
                           .AddOption("~I forget everthing...", 12)
                          .AddOption("The~nightmare~is~over.", 255)
                          .AddAvatar(95).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("You took the Red Toxin and Blue Toxin in turn, and felt very dizzy.");

                        break;
                    }
                case 13:
                    {
                        if (HasItems(client, 729982, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(729982, 1, stream)) break;
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(500, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You`ve~received~EXP~worth~5/6~EXP~Ball~and~5,000~Silver!~Claim~your~reward~from~Taskmaster~Chang!!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Yes! Thank you! My uncle has given your reward to Taskmaster Chang. Go claim it there!")
                                .AddOption("OK.~Thank~you.", 14)
                                .AddOption("I`ll~go~later.", 255)
                                .AddAvatar(77).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        data.AddText("Where am I? I remembered I was in Kun Lun... then I was in a dismal abyss... Oh, my head...")
                            .AddText("~Oh, it was you! You saved me! Thank you! I thought I`d never escape the darkness.... Please, take this...")
                             .AddOption("Welcome.", 255)
                             .AddAvatar(77).FinalizeDialog();

                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~claim~your~reward~from~Taskmaster~Chang!", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
                case 1:
                    {
                        data.AddText("History has forgotten those glorious celestials... So aloof and dreamy...")
                            .AddText("~Oh, Kun Lun, the Land of the Immortality... Why do I only have this potpourri to remember you by?")
                                 .AddOption("Taskmaster~Chang~sent~me~here.", 2)
                                 .AddOption("Bye.", 255)
                                 .AddAvatar(77).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        data.AddText("For what? The story of Kun Lun?")
                                   .AddText("~I don`t remember anything except the potpourri bag...")
                                        .AddOption("How~did~you~get~it?", 3)
                                        .AddAvatar(77).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText(" Oh, this? Well, it was given by a celestial when I visited Kun Lun. Isn`t it elegant?")
                                         .AddOption("Sounds~interesting.", 4)
                                         .AddAvatar(77).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~potpourri~bag~looks~similar~to~General~Judd`s~(397,235)~in~Twin~City.~Visit~him~to~investigate.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
            }
        }
    }
}
