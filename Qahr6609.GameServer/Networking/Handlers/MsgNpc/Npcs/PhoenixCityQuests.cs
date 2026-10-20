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
        [NpcAttribute(NpcID.GoodManLiu)]
        public static void GoodManLiu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Thanks for coming to the Ancestral Hall. They`re all the greatest Guild Leaders of all time!")
                       .AddOption("How~to~worship?", 2).AddOption("Back~to~Twin~City.", 3).AddOption("Indeed!", 255).AddAvatar(47)
                      .FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("You just need to click on the incense burners in front of statues to worship Robin~, iEatBabies and ~*~PureHeart~*~. Each day, you only have one chance to worship one of them.")
                        .AddOption("Worship~xHolic.", 4).AddOption("Worship~Robin~.", 5).AddOption("Worship~*~PureHeart~*~.", 6).AddOption("Got~it.", 255)
                        .AddAvatar(47)
                            .FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(256, 238, 1002);
                        break;
                    }
                case 4:
                    {
                        data.AddText("xHolic is the leader of the 2nd Supreme Guild MakeMeFamous. Worshipping xHolic will bring you 60 minutes` EXP, 50 Study Points and 1 Chi Token.")
                     .AddOption("I~see.", 255)
                      .AddAvatar(47)
                          .FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Robin~ is the leader of the 1st Supreme Guild SkillzDatKillz. Worshipping Robin~ will bring you 5 hours` Blessing, 60 minutes` EXP and 1 Chi Token.")
                     .AddOption("I~see.", 255)
                      .AddAvatar(47)
                          .FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("~*~PureHeart~*~ is the leader of the 3rd Supreme Guild Seven?hH??aven. Worshipping ~*~PureHeart~*~ will briing you 5 hours` Blessing, 50 Study Points and 1 Chi Token.")
                     .AddOption("I~see.", 255)
                      .AddAvatar(47)
                          .FinalizeDialog();

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Nuwa)]
        public static void Nuwa(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.CreateBoxDialog("Failed~to~worship~Nuwa~.~Please~go~to~the~front~of~the~Nuwa~statue~to~worship.");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HuangDi)]
        public static void HuangDi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.CreateBoxDialog("Failed~to~worship~HuangDi.~Please~go~to~the~front~of~the~HuangDi~statue~to~worship.");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Cense3)]
        public static void Cense3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.CreateBoxDialog("Failed~to~worship~HuangDi.~Please~go~to~the~front~of~the~HuangDi~statue~to~worship.");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Censer2)]
        public static void Censer2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.CreateBoxDialog("Failed~to~worship~Nuwa~.~Please~go~to~the~front~of~the~Nuwa~statue~to~worship.");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Censer1)]
        public static void Censer1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6329);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Worshipping ~YanDi~ will briing you 5 hours` Blessing, 50 Study Points and 1 Chi Token. Continue?");
                        data.AddOption("Yes.", 1);
                        data.AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.AddPick(stream, "Sure!", 2);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                            client.CreateBoxDialog("You~received~5~hours~Blessing,~50~Study~Points~and~1~Chi~Token~from~YanDi~.");
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Player.AddHeavenBlessing(stream, (int)(5 * 60 * 60));
                        }
                        else
                        {
                            data.AddText("You`ve worshipped today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.YanDi)]
        public static void YanDi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6329);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Worshipping ~YanDi~ will briing you 5 hours` Blessing, 50 Study Points and 1 Chi Token. Continue?");
                        data.AddOption("Yes.", 1);
                        data.AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                            client.CreateBoxDialog("You~received~5~hours~Blessing,~50~Study~Points~and~1~Chi~Token~from~YanDi~.");
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Player.SubClass.AddStudyPoints(client, 50, stream);
                            client.Player.AddHeavenBlessing(stream, (int)(5 * 60 * 60));
                        }
                        else
                        {
                            data.AddText("You`ve worshipped today! Please come again tomorrow!");
                            data.AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.BanditBoss)]
        public static void BanditBoss(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1116);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PhoenixCastellan, client.Player.Class, 1118);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("What are you looking for?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) && client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0))
                            data.AddOption("Bandits`~Plot~II.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) && client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1) == false)
                            data.AddOption("Bandits` Plot IV.", 6);
                        data.AddOption("Nothing.", 255)
     .AddAvatar(100).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.TransformationID != 0)
                        {
                            data.AddText("Well, we are going to take Phoenix Castle, tomorrow. Pass the message to the other remembers and don`t tell anyone else!");
                            data.AddOption("Sure~I~won`t.", 2)
                                 .AddAvatar(100).FinalizeDialog();
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~put~on~the~coat~you~stole~from~level~33~Bandit.");
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("You~are~heading~to~tell~the~Intelligence~Agent~the~news.");
                        data.AddOption("Yes.", 3).AddOption("No.", 255)
                             .AddAvatar(100).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.SendSysMesage("You've put on the Bandit`s Coat. Hurry to find the Bandit Boss to find out about the confidential letter!", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

                        break;
                    }
                case 6:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 0, 1);
                            client.Player.QuestGUI.SendAutoPatcher("You~are~heading~to~report~to~the~Phoenix~Castellan.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                            break;
                        }
                        data.AddText("You spy! I`ll kill you!");
                        data.AddOption("Keep~dreaming!", 7)
                             .AddAvatar(100).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (!client.Map.ContainMobID(11008))
                        {
                            Database.Server.AddMapMonster(stream, client.Map, 11008, client.Player.X, client.Player.Y, 18, 18, 1);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WarrantOfArrest)]
        public static void WarrantOfArrest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WarrantOfArrest, client.Player.Class, 1144);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("		Most Wanted").AddText("  	Eidolon! Dead or alive!").AddText("                                 -----Militia Captain.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            data.AddOption("Slay~the~Eidolon~Envoy.", 1);
                        data.AddOption("Quit.", 255);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Objective: 15 Fire Spirits\n").AddText("Fire Spirit Zone\n").AddText("Risk: Low\n").AddText("Issuer: Militia Captain\n")
                         .AddOption("Accept~Quest.", 3).AddOption("Quit.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Kill~the~Fire~Spirits~and~then~report~back~to~the~Militia~Captain.", 1011, 613, 710, 0);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.MilitiaCaptain)]
        public static void MilitiaCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaLeopard, client.Player.Class, 1143);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BoldBoy, client.Player.Class, 1131);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WarrantOfArrest, client.Player.Class, 1144);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaCaptain, client.Player.Class, 1300);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I love my family and my village! It is my duty to stay here protect them. It will be great if you can offer me a hand. I will give you some jobs to do. Which one of the following do you want to try?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~am~ordered~to~report~to~you!", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~am~ordered~to~report~to~you!", 3);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Solve~the~Fire~Spirit.", 8);
                        data.AddOption("Quest:~The~Ape~City.", 9);
                        data.AddOption("They~miss~you~too!", 255);
                        data.AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 2 more spaces in your inventory")
                          .AddOption("Let me check.", 255)
                          .AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.IncreaseExperience(stream, 161000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Inventory.AddItemWitchStack(720172, 0, 5, stream);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've receive some EXP 1,000 Silver, 5 EXP Ball Scraps and Level 40 Super Weapon(B)! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                client.Inventory.Add(stream, 480079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                client.Inventory.Add(stream, 561079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                client.Inventory.Add(stream, 500189, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                client.Inventory.Add(stream, 601079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                client.Inventory.Add(stream, 610079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                client.Inventory.Add(stream, 611079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                client.Inventory.Add(stream, 617079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 620079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                client.Inventory.Add(stream, 626079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            break;
                        }
                        data.AddText("Well done! Maple Forest is a little bit safer, now! Take your reward. You did good, today.")
                            .AddOption("Thanks.", 9)
                            .AddAvatar(102).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        data.AddText("The AC-Lieutenant in Ape City need our help! I hope you can offer him a hand!")
                          .AddOption("No~problem!", 10)
                          .AddOption("I~don`t~want~to~go~there!", 255)
                          .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        data.AddText("Speak~with~the~AC-Lieutenant~in~Ape~City!")
                           .AddOption("Yes.", 11)
                           .AddOption(" No.", 255)
                           .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Teleport(573, 625, 1020);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 161000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You receive 161,000 Experience and 500 Gold.. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("This is not a safe place! You have to be careful, if you`re going to wander around, here!")
                           .AddOption("Yes~sir!", 2)
                           .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 656, 698, 8278);
                        break;
                    }
                case 3:
                    {
                        data.AddText("What? He is gone? Oh, my god! He is really gone? Not Leopard!  LEOPARD! NOOOOOOOOOOOOOOOOOOOOOOOOOO!!!!!!!!!")
                             .AddOption("Control~yourself!", 4)
                             .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check", 255)
                           .AddAvatar(102).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            client.IncreaseExperience(stream, 100000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received some EXP, 500 Silver and 5 Amritas!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("What? He is gone? Oh, my god! He is really gone? Not Leopard!  LEOPARD! NOOOOOOOOOOOOOOOOOOOOOOOOOO!!!!!!!!!")
                             .AddOption("Control~yourself!", 5)
                             .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Would you like to some Fire Spirits! I guarantee you would be well paid!")
                               .AddOption("Great!", 6)
                               .AddAvatar(102).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 791, 478, 8281);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MilitiaLeopard)]
        public static void MilitiaLeopard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaDragon, client.Player.Class, 1141);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaLeopard, client.Player.Class, 1142);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaLeopard, client.Player.Class, 1143);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I am a soldier! Whether I`m alive or dead, I will fight for my people! But I have many unfinished wishes. I want to return my identity token, and I want my revenge. Will you help me?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Are~you~Leopard?", 1);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("I~will~revenge~for~you!", 3);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("What identity token?", 5);
                        data.AddOption("Great Soldier", 255);
                        data.AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 194000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 600;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You receive 200,000 Experience and 600 Gold. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Are you looking for me? I am a dead man now!")
                                     .AddOption("Rest~in~peace,~soldier!", 3)
                                     .AddAvatar(236).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.IncreaseExperience(stream, 194000, Role.Flags.ExperienceEffect.angelwing);
                                    client.Player.Money += 1000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.Inventory.AddItemWitchStack(720172, 0, 1, stream);
                                    client.SendSysMesage("You've received some EXP, 1,000 Silver and 5 EXP Ball Scraps! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    data.AddText("Die! Die! You bastard!")
                                          .AddOption("I~know~you~are~angry!", 5)
                                          .AddAvatar(236).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please, collect 5 Fury Cores for me! My soul will be appeased.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(236).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I went too deep into this dangerous area and was trapped by the Fire Spirits. I`m a dead man, now.. But I didn`t lost the heart of a soldier! I will keep fighting to save my village! I know these beasts carry Fury Cores to boost their powers. Will you")
                            .AddText("~collect 5 cores for me? I need them to put my enraged soul at peace. Only then will I be able to move on to the next world...")
                            .AddOption("I~will~go!~Now!", 4)
                                 .AddAvatar(236).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Collect~5~Fury~Cores.", 1011, 683, 492, 0);

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This is my identity token... I want you to take it to my Captain. He will know what happen to me when he sees it!")
             .AddOption("I~will.", 255)
             .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        data.AddText("This is a token used to recognize my identity. I want you to take it to my Captain. He will know what happened to me, when he sees it. I can`t forget Tiger and Dragon. They will always be my friends, even in the afterlife! I will miss them all...")
                                    .AddOption("I~will~do~that~for~you!", 6)
                                    .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check", 255)
                                .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711333);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher("Send~the~Token~to~Militia~Captain.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MilitiaDragon)]
        public static void MilitiaDragon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaDragon, client.Player.Class, 1133);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaDragon, client.Player.Class, 1141);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("It is tough living out here in the wild! I just got a little water left, and unsavoury food to chow. And the worst thing is that we lost one of my team mate.");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("I~can~get~some~water~for~you!.", 1);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("I~can~get~some~water~for~you!.", 3);
                        data.AddOption("Homesick!~Right?", 255);
                        data.AddAvatar(48).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            if (HasItems(client, 711331, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (!client.Inventory.Remove(711331, 1, stream)) break;
                                client.IncreaseExperience(stream, 194000, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.Money += 600;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You receive 194,000 Experience and 600 Gold. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("Ahhhhhh! Fresh water! You saved my life, man. Nothing like cool water, if you`re standing in this hell! Thank you!")
                                     .AddOption("You`re~welcome!", 3)
                                     .AddAvatar(48).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("It`s hard staying near the territory of the Fire Spirits. It`s so hot and I need some water, but I can`t leave my post.")
                                 .AddText("~Hey! Will you go and get some water for me?")
                                 .AddOption("My~pleasure.", 2)
                                 .AddAvatar(48).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me to check", 255)
                                .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711331);
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Use~the~Empty~Bucket~to~fetch~a~pail~of~water~by~the~river~near~the~bridge.", 1011, 643, 622, 0);

                        break;
                    }
                case 3:
                    {
                        data.AddText("I wonder where another of our team, Militia Leopard, might have gone? He has been missing for quite a while. I want to make sure he`s okay. Do you think you could take a look around, and see if you can find him?")
                                .AddOption("Show~me~the~direction!", 4)
                                .AddAvatar(48).FinalizeDialog();

                        break;
                    }

                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.MilitiaTiger)]
        public static void MilitiaTiger(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BoldBoy, client.Player.Class, 1131);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaTiger, client.Player.Class, 1135);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I miss my wife and my son back home. My son`s birthday is also approaching. But I will not come home until those blasted Fire Spirits have all been wiped out!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~got~a~letter~for~you!", 2);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("You~get~anything~for~your~son?.", 3);
                        data.AddOption("You~are~a~good~father.", 255);
                        data.AddAvatar(57).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.IncreaseExperience(stream, 161000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You receive 161,000 Experience and 500 Gold.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("Oh, I miss them, too! But I have job to do here! We have to save the village! I can`t just walk away from my duty.")
                                  .AddOption("Great~man!", 3)
                                  .AddAvatar(57).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                        {
                            if (HasItems(client, 711332, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(711332, 10, stream)) break;
                                client.Player.Money += 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.RateExp = 2;
                                client.Player.DExpTime = 60 * 60;
                                client.Player.CreateExtraExpPacket(stream);
                                client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                                client.SendSysMesage("You received 60 minutes EXP, 1,000 Silver and 5 Amritas!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            data.AddText("Wooh! What beautiful teeth you have! I can feel the heat coming off them, from here! My son will love it!")
                         .AddOption("You`re~a~good~father.", 5)
                         .AddAvatar(57).FinalizeDialog();
                            break;
                        }
                        data.AddText("My son holds great enthusiasm, joy, and pleasure for this world! I want to make him a necklace for his birthday, but I need 10 Fire Spirit Teeth to finish making it. Will you help me to collect some?")
                             .AddOption("Okay!", 4)
                             .AddAvatar(57).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Collect~10~Spirit~Teeth~from~the~Fire~Spirits.", 1011, 704, 673, 0);

                        break;
                    }
                case 5:
                    {
                        data.AddText("Wait a second, please. My brother, Militia Dragon, needs some water badly. Go help him, if you can!")
                        .AddOption("OK,~I~will.", 6)
                        .AddAvatar(57).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 661, 704, 8279);
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.BoldBoy)]
        public static void BoldBoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BoldBoy, client.Player.Class, 1121);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BoldBoy, client.Player.Class, 1131);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I used to spend a night in the hill, alone! Don`t you think I`m brave? If you are also a brave man, I will have some job for you to do. Which of the following quests do you want to try?");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Kill~10~Ratlings.", 2);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Letter~From~Home.", 10);
                        data.AddOption("It`s~great!", 255);
                        data.AddAvatar(36).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("I bet you wouldn`t  even dare to kill a Ratling! If you think you can, kill 10 for me, and I`ll pay you!")
                                     .AddOption("It`s~a~cinch.", 3)
                                     .AddAvatar(36).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                        {

                            data.AddText("You are great! Thank you! You have definitely earned the right to join my gang! How cool is that?")
                                  .AddOption("Really?~That`s~great!", 5)
                                  .AddAvatar(36).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        data.AddText("You~will~find~some~Ratlings~running~outside~the~village.~go~slay~them.")
                                    .AddOption("Yes.", 4).AddOption("No.", 255)
                                    .AddAvatar(36).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~killed~10~Ratlings!", 1011, 638, 456, 0);

                        break;
                    }
                case 5:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.RateExp = 2;
                            client.Player.DExpTime = 60 * 60;
                            client.Player.CreateExtraExpPacket(stream);
                            client.Inventory.Add(stream, 721710);
                            client.SendSysMesage("You received 60 minutes EXP, 800 Silver and Castle Gate", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText(" I hear that the Fur Dealer is badly in need of some Ratling Fur. I`m sure you`ll be well paid, if you help him!")
                                    .AddOption("Thanks~for~the~info..", 6)
                                    .AddAvatar(36).FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 796, 475, 8275);
                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you going to find the Militia in the forest? Can you take me with you? I miss my father, he has been away from home for over 2 months! No? Can you take this letter to my father? My mother and I miss him, very much! Oh, my father is the Militia Tiger.")
                                .AddOption("Sure,~I~can.", 11)
                                .AddAvatar(36).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Send~the~letter~to~the~Militia~Tiger.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FurDealer)]
        public static void FurDealer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.FurDealer, client.Player.Class, 1123);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Ratlings have beautiful fur! I can make good money from selling their fur to the other cities.");
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Are~you~buying~the~Fur?", 2);
                        data.AddOption("What~a~good~business!", 255);
                        data.AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                        {
                            //client.Inventory.Remove(711325, 10, stream);
                            data.AddText("Oh! Beautiful, shining fur! I can make good money by selling these. Here is your reward! Thank you, again!")
                                   .AddOption("See~you!", 4)
                                   .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("Let me introduce myself, I am the Fur Dealer! A far travelling businessman, with quite the eye for animals! Would you like to make some money? I need to get 10 Ratling Furs, quick! If you can help, you will be paid handsomely!")
                                     .AddOption("Okay!", 3)
                                     .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Collect~10~Ratling~Furs.", 1011, 638, 456, 0);

                        break;
                    }
                case 4:
                    {
                        if (HasItems(client, 711325, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711325, 10, stream)) break;
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.IncreaseExperience(stream, 500, Role.Flags.ExperienceEffect.angelwing);
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                client.Inventory.Add(stream, 130038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                client.Inventory.Add(stream, 131038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                client.Inventory.Add(stream, 133028, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                client.Inventory.Add(stream, 135038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                client.Inventory.Add(stream, 136038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                client.Inventory.Add(stream, 139038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                client.Inventory.Add(stream, 138038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 134038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                client.Inventory.Add(stream, 170038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.SendSysMesage("You received some EXP, 500 Silver and a level 40 Elite Armor(B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText("Hey, wait! The Village Head is looking for you, about the Ratling, I guess.")
                                 .AddOption("Got~it.", 5)
                                 .AddAvatar(53).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 772, 475, 8273);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.VillageHead)]
        public static void VillageHead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PhoenixCastellan, client.Player.Class, 1120);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1125);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1126);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1127);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1128);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1129);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1130);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Phoenix Castle is always our friend! My village is in trouble now, I hope our friends in the Castle can help us out! If you are here to help me, I have a bunch of jobs for you to do! Which of the following quests do you want to try?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~was~sent~to~help~you!", 2);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~1.", 5);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~2.", 7);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~3.", 10);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~4.", 12);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ratling~King~5.", 14);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Help~Militia~Captain.", 20);
                        data.AddOption("See~you.", 255);
                        data.AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.Money += 300;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.SendSysMesage("You've received 300 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        data.AddText("Phoenix Castle will send help to us? Thank you! Thank you, so much!")
                                       .AddOption("You`re~welcome.", 3)
                                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You`re welcome! By the way, there`s a Bold Boy who`s going to  kill a Ratling!")
                                        .AddOption("~That is dangerous! Go help him, please.", 4)
                                        .AddOption("Okay!", 255)
                                        .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 761, 473, 8274);
                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                         .AddOption("Let me check.", 255)
                                         .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                        {
                            if (!client.Inventory.Remove(711327, 1, stream)) break;
                            client.Inventory.AddItemWitchStack(720172, 0, 3, stream);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                client.Inventory.Add(stream, 118038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                client.Inventory.Add(stream, 111038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                client.Inventory.Add(stream, 113038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                client.Inventory.Add(stream, 112018, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                client.Inventory.Add(stream, 143038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                client.Inventory.Add(stream, 144038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                client.Inventory.Add(stream, 148038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 114038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                client.Inventory.Add(stream, 101038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.IncreaseExperience(stream, 138000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some EXP, 800 Silver, 3 EXP Ball Scraps and a Level 37 Elite Helment! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);


                            data.AddText("I am quite impressed by your performance! You really are great! We could use more people like you in the village.")
                           .AddOption(" Thank~you!", 7)
                                   .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Ratlings are out of control! We found that there is an elusive Ratling King, hiding out there. If you killed it, I think the Ratlings would get confused and stop attacking our village. Snake Meat is the favorite food of the Ratling King,")
                            .AddText("~so if you get some from the Winged Snakes, we should be able to use it to charm the Ratling King out of hiding!")
                            .AddOption("Winged~Snake~Meat?~Okay!", 6)
                                    .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Head~to~Hunter~Wang~for~more.", 1011, 790, 447, 8268);


                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                        {
                            if (!client.Inventory.Remove(711328, 1, stream)) break;
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.IncreaseExperience(stream, 151000, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.Money += 500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 151000 Experience and 500 Gold. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("Good Wine! I could smell it as soon as you were walking up the path! Well, let`s get busy making that lure.")
                  .AddOption("Great!~Let`s~get~busy!", 10)
                          .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("You must be thinking what I am doing. I will make a lure using the Snake Meat! To make the lure more attractive! I need some Wine! Could you buy a bottle of wine from Deeler Shen?")
                              .AddOption("Okay!", 8)
                                      .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Buy~a~bottle~of~Wine~from~the~Dealer~Shen(779,447).", 1011, 182, 280, 4596);

                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more spaces in your inventory.")
                                     .AddOption("Let me check.", 255)
                                     .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 10))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Inventory.AddItemWitchStack(720173, 0, 3, stream);
                                client.IncreaseExperience(stream, 151000, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.Money += 800;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You've received some EXP, 800 Silver and 3 Small Joy Stones. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("I was quite impressed by you performance! Good job!")
                          .AddOption("Thank~you.", 12)
                       .AddAvatar(67).FinalizeDialog();

                            }


                            break;
                        }
                        data.AddText("I will soak the Snake Meet in the Wine, so it will take a bit. Why not use this time to slay some Ratlings! This will help make you smell like the Ratlings. That way, it will make it harder for the Ratling King to discover you!")
                            .AddOption("Okay!", 11)
                         .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Slay~10~Ratlings~outside~the~village.", 1011, 638, 456, 0);

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1))
                        {
                            data.AddText("Good! You have it! Now take a little break. I want study the Frost Token first, and will instruct you on what to do, later.")
                            .AddOption("Okay!", 14)
                         .AddAvatar(67).FinalizeDialog();

                            client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId);
                            if (!client.Inventory.Remove(711329, 1, stream)) break;
                            client.IncreaseExperience(stream, 242000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 242,000 Experience and 800 Gold. ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            break;
                        }
                        data.AddText("You still need a Frost Token. It will provide you with extra defense, especially your resistance against heat! I`ll show you where to find it!")
                        .AddOption("Okay!", 13)
                     .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 532, 434, 0);
                        break;
                    }
                case 14:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more spaces in your inventory.")
                                         .AddOption("Let me check.", 255)
                                         .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId);
                            client.IncreaseExperience(stream, 242000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received some EXP, 2,000 Silver and a Bound EXP Ball! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.Add(stream, Database.ItemType.ExpBall, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            data.AddText("I can`t believe my ears! You killed it and saved our village! See? The Ratlings are scattering! Your name will be remembered for as long as our village remains! Thank you, hero!")
                      .AddOption("Thank~you!", 20)
                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Listen! The Ratling King excels at fire attacks. This Frost Token can provide with you extra resistance against heat and fire, for 5 minutes. Use the Wine Soaked Snake to charm it out, then slay it while you`re protected. If you don`t make it in time,")
                    .AddText("~you will be sent back to village, before anything bad happens to you! Are you ready to challenge the Ratling King?")
                            .AddOption("I~am~ready!", 15)
                      .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more spaces in your inventory.")
                                     .AddOption("Let me check.", 255)
                                     .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720840);
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Use~the~Wine~Soaked~Snake~to~charm~the~Ratling~King~out,~and~then~slay~it!", 1011, 648, 441, 0);

                        break;
                    }
                case 20:
                    {
                        data.AddText("Fire Spirits are another big threat to the village! In fact, they`re a threat to Phoenix Castle, too! There are some militiaman fighting desperately against these evil spirits. Maybe you can offer them some help? You can speak with the Militia Captain")
                          .AddText("~for more information!")
                            .AddOption("Find~the~Militia~Captain.", 21)
                            .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 761, 473, 8274);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WineKiddo)]
        public static void WineKiddo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillageHead, client.Player.Class, 1126);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 401);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("My wine enjoys a great reputation among people! Would you like a bottle of wine? It`s free!");
                            data.AddOption("Do~you~have~wine~to~sell?", 5);
                            data.AddOption("Wonderful!");
                            data.AddAvatar(138);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("My wine enjoys a great reputation among people! Would you like a bottle of wine? It`s free!");
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("Quest:~Ratling~King~2.", 2);
                            data.AddOption("Wonderful!", 255);
                            data.AddAvatar(138).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Hey, my friend! Welcome to my store. You see, the Bamboo Wine is our best seller. Expensive? Nah, hah. It`s only 10,000");
                        data.AddText("~Silver for each bottle! Don`t get drunk, ok? Oh, I`ll have a branch in Twin City, soon. You can come and have a drink.");
                        data.AddOption("Buy~one.", 6);
                        data.AddOption("Let~me~check~my~wage,~first.");
                        data.AddAvatar(138);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(721800, 1))
                        {
                            data.AddText("I tell you, this Bamboo Wine is pretty strong. I`ve never seen one guy buy 2 bottles before. Take care.");
                            data.AddOption("Don`t~worry~about~me.");
                            data.AddAvatar(138);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money < 10000)
                                {
                                    data.AddText("You need 10,000 silvers for this. It`s already the floor price.");
                                    data.AddOption("Alright.");
                                    data.AddAvatar(138);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    client.Player.Money -= 10000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                    client.Inventory.Add(stream, 721800, 1);
                                    client.SendSysMesage("You brought a bottle of Bamboo Wine with 10,000 silvers.");
                                }
                            }
                            else
                            {
                                data.AddText("Oh, look at your inventory. I have to remind you that the bottle is very fragile. Make some room for it, first.");
                                data.AddOption("One~second.");
                                data.AddAvatar(138);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText(" Come! Best wine! Cheap! 1000 gold a bottle! Come! Come! Best wine for sale!")
                                .AddOption(" I~need~a~bottle~of~wine!", 3)
                                .AddAvatar(138).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(711328, 1))
                        {
                            data.AddText("You have bought a bottle of wine from me! Don`t drink too much!")
                                .AddOption("Thanks.", 255)
                                .AddAvatar(138).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("let me check.", 255)
                              .AddAvatar(138).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Money >= 1000)
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.Money -= 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 711328);

                            data.AddText("You have bought a bottle of wine from me! Don`t drink too much!")
                              .AddOption("Thanks.", 4)
                              .AddAvatar(138).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                              .AddOption("I~see.", 255)
                              .AddAvatar(138).FinalizeDialog();

                        }

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HunterWong)]
        public static void HunterWong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HunterWong, client.Player.Class, 1107);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("He who fights with monsters should take care, lest he wind up becoming a monster, himself! Would you like to help me with any quest?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Snake~Scale.", 2);
                        data.AddOption("Sooner~or~later.", 255);
                        data.AddAvatar(45).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                        {
                            data.AddText("I can`t thank you enough. That`s perfect!")
                                .AddOption("Fantastic!.", 5)
                                .AddAvatar(45).FinalizeDialog();

                            break;
                        }
                        data.AddText("To do a good job, you must first sharpen your tools. I heard that poisonous Snake Scales can make animals dizzy.")
                                    .AddOption("~Can you get 10 Snake Scales, for me?", 3)
                                    .AddOption("I`d~love~to.", 255)
                                    .AddAvatar(45).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You~are~going~to~kill~Winged~Snakes~and~collect~10~Snake~Scales.")
                                  .AddOption("Yes.", 4)
                                  .AddOption("No.", 255)
                                  .AddAvatar(45).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 342, 132, 0);
                        break;
                    }
                case 5:
                    {
                        if (HasItems(client, 711321, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711321, 10, stream)) break;
                            client.IncreaseExperience(stream, 138000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 600;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 138,000 EXP and 600 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText("Have you got some Snake Meat, by the way? Go speak with the Village Head. He`s worried about it.")
                                  .AddOption("I`m~on~my~way.", 6)
                                  .AddAvatar(45).FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 772, 475, 8273);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PhoenixCastellan)]
        public static void PhoenixCastellan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1117);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PhoenixCastellan, client.Player.Class, 1118);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PhoenixCastellan, client.Player.Class, 1120);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I will always protect the people, here.")
                                     .AddOption("Bandits`~Plot~III.", 2)
                                     .AddOption("Greetings,~Castellan!", 255)
                                     .AddAvatar(71).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I will always protect the people, here.");
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Bandits`~Plot~IV.", 4);
                            else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Help~the~Village~Head..", 10);
                            data.AddOption("Greetings,~Castellan!", 255)
                               .AddAvatar(71).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("This letter is very important. Thank you very much!")
                                   .AddOption("It`s~OK.", 3)
                                   .AddAvatar(71).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (HasItems(client, 711323, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711323, 1, stream)) break;
                            client.IncreaseExperience(stream, 10000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some exp and 500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("Kill the leader and the gang will collapse. Please, help us kill the Bandit Boss!")
                                   .AddOption("Yes,~sir!", 5)
                                   .AddAvatar(71).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.IncreaseExperience(stream, 10000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1200;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some exp and 1200 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText("You must be tired. Take a break, buddy.")
                              .AddOption("Okay.", 10)
                              .AddAvatar(71).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What`s the matter? You need to go kill the Bandit Boss!")
                                .AddOption("I~will.", 255)
                                .AddAvatar(71).FinalizeDialog();
                            break;
                        }
                        data.AddText("Kill the leader and the gang will collapse. Please, help us kill the Bandit Boss!")
                                 .AddOption("Yes,~sir!", 5)
                                 .AddAvatar(71).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.SendSysMesage("You are heading to kill the Bandit Boss.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 227, 404, 8272);

                        break;
                    }
                case 10:
                    {
                        data.AddText("Yes, there is a small village outside the Phoenix Castle in the forest. We maintain a good relationship with the village. Recently, the village was badly damaged by rats. As a neighbor, we need to do something! Will you go see the Village Head first,")
                            .AddText("~and let him know about our plan to help out?")
                            .AddOption("Of~course!", 11)
                               .AddAvatar(71).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Tell~the~Village~Head~that~the~Castellan~will~help~him~deal~with~the~ratalings.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.IntelligenceAgent)]
        public static void IntelligenceAgent(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1115);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1116);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IntelligenceAgent, client.Player.Class, 1117);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, there! I`m Intelligence Agent. Want to take the quests and destroy Bandits` plots?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Bandits`~Plot~I.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Bandits`~Plot~II.", 4);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Bandits`~Plot~III.", 10);
                        data.AddOption("Nice~to~meet~you..", 255)
                        .AddAvatar(236).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.IncreaseExperience(stream, 15000, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.Money += 800;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You've received some exp and 800 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("You did a good job!")
                                        .AddOption("No~problem.", 4)
                                        .AddAvatar(236).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You need to get a Bandit`s Coat from the Level 33 Bandits. We will be able to blend in with the rest of the gang, once we have it.")
                                    .AddOption("Roger.", 255)
                                    .AddAvatar(236).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("We have been here trying to collect information for weeks, but haven`t found anything, yet. We are planning to steal a Bandit`s Coat from")
                        .AddText("~the Level 33 Bandits, so we will be able to blend into the gang. Would you like to get one for me?")
                        .AddOption("Sure.", 2)
                               .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("You~are~going~to~get~a~Bandit`s~Coat~from~the~Level~33~Bandits.")
                       .AddOption("Yes.", 3).AddOption("No.", 255)
                              .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(711323, 1))
                                client.Inventory.Add(stream, 711323);
                            data.AddText("OK. You`ve got the letter. Deliver it to the Phoenix Castellan.")
                          .AddOption("Take~care.", 255)
                          .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        data.AddText("I didn`t expect you would be back so soon! Did you read the letter yet?")
                            .AddOption("Yes.~I~remember~everything.", 11)
                            .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("That`s great. I`m writing a letter. Please deliver it to the Phoenix Castellan.")
                          .AddOption("I`ll~wait.", 12)
                          .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check", 255)
                                .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711323);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You~are~going~to~deliver~the~letter~to~the~Phoenix~Castellan.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check", 255)
                                    .AddAvatar(236).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                                client.IncreaseExperience(stream, 16000, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.Money += 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                                client.SendSysMesage("You've received some Exp, 1,000 Silver and 5 Amritas!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                data.AddText("I didn`t expect you would be back so soon! Did you read the letter yet?")
    .AddOption("Yes.~I~remember~everything.", 11)
    .AddAvatar(236).FinalizeDialog();
                                break;
                            }
                            if (client.Inventory.Contain(720837, 1) == false)
                                client.Inventory.Add(stream, 720837);
                            data.AddText("If you happen to be recognized, just try to get another coat from the Level 33 Bandits and try again, later.")
     .AddOption("Copy~that.", 255)
     .AddAvatar(236).FinalizeDialog();
                            break;
                        }
                        data.AddText("Put on the Bandit`s Coat, and you won`t be recognized. If you happen to be recognized, just try to get another coat from the Level 33 Bandits.")
                     .AddOption("What~shall~I~do~then?", 5)
                            .AddAvatar(236).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("You need to talk to the Bandit Boss and read the confidential letter. If he asks you what the code is, you should answer `Success is around the corner`.")
                            .AddOption("Got~it.", 6)
                            .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("You~put~on~the~coat~and~are~going~to~see~the~Bandit~Boss.")
                            .AddOption("Yes", 7).AddOption("No.", 255)
                            .AddAvatar(236).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Contain(720837, 1))
                                client.Inventory.Add(stream, 720837);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.QuestGUI.SendAutoPatcher(1011, 227, 404, (uint)NpcID.BanditBoss);
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("let me check.", 255)
                           .AddAvatar(236).FinalizeDialog();
                            break;
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ForestGuvernor)]
        public static void ForestGuvernor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ForestGuvernor, client.Player.Class, 1114);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey! Who are you and why are you here? Did you take any quest, or what?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Regain~Supplies.", 3);
                        data.AddOption("Good.", 255)
                        .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 3:
                    {

                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                        {
                            if (!client.Inventory.HaveSpace(2))
                            {
                                data.AddText("Please make 2 more spaces in your inventory.")
                         .AddOption("Let me check.", 255)
                                .AddAvatar(95).FinalizeDialog();
                                break;
                            }
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            if (!client.Inventory.Remove(711322, 10, stream)) break;
                            client.Inventory.Add(stream, 723017, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 150057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some exp, 500 Silver, a Bound EXP Potion and a Gold Ring(Bound)!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            data.AddText("Thank god! Please accept these gifts, as thanks. You`ve really managed to get me out of this mess! You`re a lifesaver!")
                           .AddOption("Thanks.", 6)
                                  .AddAvatar(95).FinalizeDialog();
                            break;
                        }

                        data.AddText("We got big trouble! Well, we were carrying military supplies to Phoenix Castle, when all of a sudden the Bandits rushed out")
                            .AddOption("Relax.~I`ll~be~back,~soon.", 4)
                                   .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("You~are~going~to~get~10~bags~of~grain~from~the~Bandits.")
                          .AddOption("Yes.", 5).AddOption("No.", 255)
                                 .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 235, 373, 0);
                        break;
                    }
                case 6:
                    {
                        data.AddText("Listen! Since you`re smart, I`m wondering if you can go help the Intelligence Agent. He`ll give you a challenging mission!")
                            .AddOption("I`m~on~my~way.", 7)
                            .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 215, 360, 8270);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YuJing)]
        public static void YuJing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuLin, client.Player.Class, 1102);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuJing, client.Player.Class, 1103);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuJing, client.Player.Class, 1104);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                               .AddOption("Father~and~Daughter~I.", 2)
                               .AddOption("Leave.", 255)
                               .AddAvatar(116).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                                  .AddOption("Father~and~Daughter~II.", 1)
                                  .AddOption("Leave.", 255)
                                  .AddAvatar(116).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                                  .AddOption("Father~and~Daughter~III.", 5)
                                  .AddOption("Leave.", 255)
                                  .AddAvatar(116).FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                                 .AddOption("Leave.", 255)
                                 .AddAvatar(116).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                             .AddOption("Leave.", 255)
                             .AddAvatar(116).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                        {
                            data.AddText("Great! You are my hero!")
                       .AddOption("No~problem.", 5)
                       .AddAvatar(116).FinalizeDialog();
                            break;
                        }
                        data.AddText("Haha, hi! Are you looking for me? Do you want to play a game? Let`s play hide-and-seek!")
                        .AddOption("No,~you~should~go~home.", 3)
                        .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 300;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some EXP and 300 Silver", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("But I`m still waiting for my dad. You see those Bandits, over there? What if my dad walks home at night and they see him?")
                            .AddText("~Hey, do you think you can scare them off or something? If you can get rid of 10, I bet they`ll all run off.")
                      .AddOption("Alright.", 1)
                      .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You~are~going~to~kill~10~Bandits.")
                                .AddOption("Yes.", 4)
                                .AddOption("No.", 255)

                                .AddAvatar(116).FinalizeDialog();
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1011, 48, 381, 0);
                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some EXP and 500 Silver", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        data.AddText("Give me a break! I need to focus, if I`m going to spot Dad, in time. Can you go first, and tell my mum that I will be home soon?")
                                .AddOption("OK.~Take~care.", 6).AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        data.AddText("You~are~on~the~way~to~see~Yu~Lin.")
                                .AddOption("Yes.", 7).AddOption("No.", 255).AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.YuLin)]
        public static void YuLin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterHao, client.Player.Class, 1023);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuLin, client.Player.Class, 1102);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YuJing, client.Player.Class, 1104);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ever has it been, that love knows not its own depth until the hour of separation... Oh, sorry, I miss my husband... Well, which quest are you coming for?")
                            .AddOption("Father~and~Daughter~III.", 6)
                            .AddAvatar(116).FinalizeDialog();

                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ever has it been, that love knows not its own depth until the hour of separation... Oh, sorry, I miss my husband... Well, which quest are you coming for?");
                            data.AddOption("Here`s~a~Letter~Home~for~you.", 1);
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Father~and~Daughter~I.", 2);
                            data.AddOption("See~ya.", 255)
                       .AddAvatar(116).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Ever has it been, that love knows not its own depth until the hour of separation... Oh, sorry, I miss my husband... Well, which quest are you coming for?")
                         .AddOption("Leave.", 255)
                         .AddAvatar(116).FinalizeDialog();
                            break;
                        }
                        data.AddText("Ever has it been, that love knows not its own depth until the hour of separation... Oh, sorry, I miss my husband... Well, which quest are you coming for?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Here`s~a~Letter~Home~for~you.", 1);
                        else
                            data.AddOption("Father~and~Daughter~I.", 2);
                        data.AddOption("See~ya.", 255)
                         .AddAvatar(116).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (HasItems(client, 720777, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (!client.Inventory.Remove(720777, 1, stream)) break;
                                //Some EXP, 300 Silver, Bound EXP Potion
                                client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing);

                                client.Player.Money += 300;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Add(stream, Database.ItemType.DoubleExp, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                data.AddText("What? My husband is dead? You can`t be serious!")
                                .AddText("~Please don`t tell my little girl. She couldn`t take it...")
                           .AddOption("I~won`t.", 2)
                           .AddAvatar(116).FinalizeDialog();
                            }
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("My daughter, Yu Jing, goes to the northwest everyday, in hopes that she can see her father returning home.")
                               .AddText("~It has been many hours since she went out, today. I`m worried about her. Can you go there and bring her home, please?")
                          .AddOption("You~can~count~on~me.", 3)
                          .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You~are~on~the~way~to~look~for~Yu~Jing.")
                            .AddOption("Yes.", 4).AddOption("No.", 255)
                            .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("I can`t think of any need in childhood as strong as the need for a father`s protection. Well, are you coming for any quest?")
                                  .AddOption("Yes.", 5).AddOption("No.", 255)
                                  .AddAvatar(116).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 6:
                    {
                        data.AddText("Why are you alone? Where`s my daughter?")
                                .AddOption("She~will~be~back,~soon.", 7)
                                .AddAvatar(116).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                             .AddOption("let me check.", 255)
                             .AddAvatar(116).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received some EXP and 500 Silver, and Uniquer Soft Boots(B).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            client.Inventory.Add(stream, 160057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 721673);
                            if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 121047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else

                                client.Inventory.Add(stream, 120047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.Player.QuestGUI.SendAutoPatcher(1011, 211, 312, 8266);
                        }


                        //8266,1011,0211,0312
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MasterHao)]
        public static void MasterHao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterHao, client.Player.Class, 1023);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Fortuneteller, client.Player.Class, 1017);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.CreateBoxDialog("The~quest~`Capture~Spirit~I`~is~completed.~You`ve~received~500~Silver~and~some~EXP!");
                                client.Player.Money += 500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.GainExpBall(10, true, Role.Flags.ExperienceEffect.angelwing);
                            }
                        }
                        data.AddText("A holy habit cannot cleanse a foul soul. What can I do for you, my child?")
               .AddOption("Letter~Home.", 1)
               .AddOption("See~ya.", 255)
               .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("I just found this letter. It was a poem written by a villager, specially for his wife.")
                            .AddText("~Unfortunately, it seems he was killed by the Grisly Specter, while he was on his way home...")
                        .AddOption("I`m~sorry~to~hear~that.", 2)
                        .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        data.AddText("I have a bad feeling that awful things are going to happen in Phoenix Castle.")
                            .AddText("~Can you pay a visit to the Forest Governor and pass along this message, for me?")
                        .AddOption("Sure.", 3)
                        .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Teleport(212, 258, 1011);
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                client.Inventory.Add(stream, 720777);
                                client.Player.QuestGUI.Accept(ActiveQuest, 0);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            }
                            data.AddText("You~were~teleported~to~Phoenix~Castle.~Hurry~to~visit~the~Forest~Governor!").FinalizeDialog();
                            client.Player.QuestGUI.SendAutoPatcher("This~is~extremely~important.~Take~this~letter~to~the~widow,~Yu~Lin.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "archer-s");


                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check.", 255)
                        .AddAvatar(67).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Fortuneteller)]
        public static void Fortuneteller(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Fortuneteller, client.Player.Class, 1016);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Fortuneteller, client.Player.Class, 1017);
            switch (Option)
            {
                //Hello, there! Would you like to challenge the Eight-Diagram Dimensions, or any other quests?
                case 0:
                    {
                        data.AddText("Hello, there! Would you like to challenge the Eight-Diagram Dimensions, or any other quests?")
                          .AddOption("Capture~Spirit.", 1)
                          .AddOption("Dimensions?", 5) //not add.
                          .AddOption("Just~passing~by.", 255)
                          .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("It`s a secret, and I`ll only tell people who are really interested. Understand?")
                            .AddOption("Yeah,~please~go~ahead.", 6)
                            .AddOption("Sorry,~I~am~very~busy.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("I stumbled upon a mystic dimension a few days ago. I tried to work it out, but I failed and almost died from it.")
                            .AddOption("I`d~like~to~have~a~try.", 7)
                            .AddOption("Too~dangerous.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("Are you sure? That`s a horrible space filled with dangers. I`ll introduce you to Maggie if you have made up your mind. She is a know-it-all.")
                            .AddOption("Yeah.", 8)
                            .AddOption("I~changed~my~mind.", 255)
                            .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        client.Teleport(28, 33, 1042);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("It is strange that the spirits of Apparitions have been sealed by a mysterious power. I tried every possible way but none have worked!")
                                                              .AddText("~Can you pay a visit to Master Hao and ask him how to break the seal?")
                                                              .AddOption("Sure.", 3)
                                                              .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            data.AddText("I have been studying about spirits for years. Recently I invented this Spirit Container, but I`m not sure if it works.")
                                .AddText("~Would you like to capture the spirits of 10 Apparitions for me? I need to test if the aura is strong enough.")
                    .AddOption("I`m~on~my~way.", 255)
                    .AddAvatar(7).FinalizeDialog();
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.QuestGUI.Accept(ActiveQuest, 0);
                                client.Player.QuestGUI.SendAutoPatcher("You~are~going~to~capture~the~spirits~of~10~Apparitions.", 1002, 610, 510, 0);
                                client.Inventory.Add(stream, 711243);
                            }
                            else
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                       .AddOption("Let me check.", 255)
                                       .AddAvatar(7).FinalizeDialog();
                            }
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                            {
                                data.AddText("The Spirit Container is working! Thanks a million!")
                           .AddOption("You`re~welcome..", 2)//2
                           .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please take this Spirit Container and head out to capture the spirits of 10 Apparitions.")
                                    .AddOption("I`m~on~my~way.", 255)
                                    .AddAvatar(7).FinalizeDialog();
                            }
                            break;
                        }


                        break;
                    }
                case 2:
                    {
                        if (HasItems(client, 711243, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(711243, 1, stream)) break;
                            client.CreateBoxDialog("The~quest~`Capture~Spirit~I`~is~completed.~You`ve~received~500~Silver~and~some~EXP!");
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(10, true, Role.Flags.ExperienceEffect.angelwing);
                            data.AddText("It is strange that the spirits of Apparitions have been sealed by a mysterious power. I tried every possible way but none have worked!")
                                   .AddText("~Can you pay a visit to Master Hao and ask him how to break the seal?")
                                   .AddOption("Sure.", 3)
                                   .AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please visit Master Hao and ask him how to break the seal that is binding the spirits.")
                                   .AddOption("I`ll~go~now.", 255)
                                   .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.SendAutoPatcher("You~are~on~the~way~to~see~Master~Hao.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.RuHua)]
        public static void RuHua(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1012);//1013
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.RuHua, client.Player.Class, 1013);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.RuHua, client.Player.Class, 1014);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1015);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("To the world you may be one person, but to one person you may be the world! I hope I can find my Mr. Right, soon.")
                                .AddOption("Good~luck!", 255)
                                .AddAvatar(129).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What are you doing back here, so soon? Did my love enjoy the handkerchief? I do hope so!")
                       .AddOption("Send~you~a~message.", 10)
                       .AddOption("Just~passying~by.", 255)
                       .AddAvatar(129).FinalizeDialog();
                            break;
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("To the world you may be one person, but to one person you may be the world! I hope I can find my Mr. Right, soon.")
                                .AddText("~Oh, what? You have something for me?")
                        .AddOption("A~surprise~for~you.", 1)
                        .AddOption("Good~luck!", 255)
                        .AddAvatar(129).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(711241, 1))
                            {
                                data.AddText("Well, I have always had a bit of a crush on him... I wanted to make a Handkerchief and give it to him, but it was stolen by the Apparitions!")
                      .AddText("~Could you please help me to retrieve it?")
                      .AddOption("I`ll~go~now.", 255)
                      .AddAvatar(129).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("To the world you may be one person, but to one person you may be the world. I hope I can find my Mr. Right soon. What are you doing here?")
                    .AddOption("Do~you~love~Xu~Liang?", 5).AddOption("Good~luck!", 255)
                    .AddAvatar(129).FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`ve sewn some patterns on the handkerchief. Could you please help me to send this Mandarin Duck Hanky to Xu Liang?")
                .AddOption("Okay.", 255)
                .AddAvatar(129).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("To the world you may be one person, but to one person you may be the world. I hope I can find my Mr. Right soon. What are you doing here?")
                      .AddOption("Do~you~love~Xu~Liang?", 6)
                      .AddOption("Good~luck!", 255)
                      .AddAvatar(129).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("To the world you may be one person, but to one person you may be the world. I hope I can find my Mr. Right soon. What are you doing here?")
                       .AddOption("Do~you~love~Xu~Liang?", 3)
                       .AddOption("Good~luck!", 255)
                       .AddAvatar(129).FinalizeDialog();
                        }



                        break;
                    }
                case 10:
                    {
                        data.AddText("You mean Xu Liang wants to take me on a date beside the river, tonight? Okay, I see. Thanks for telling me! I`d love to!")
                          .AddOption("Best~wishes~for~you.", 11)
                          .AddAvatar(129).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId);
                            client.CreateBoxDialog("You`ve~completed~`Love~Confession~5`~and~received~200~Silver~and~some~EXP!");
                            client.Player.Money += 200;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(10, true, Role.Flags.ExperienceEffect.angelwing);

                            data.AddText("It`s said that the Fortuneteller nearby has been studying spirits for years and recently invented some new things.")
                                .AddText("~Go pay him a visit, if you wish.")
                       .AddOption("I`m~on~my~way.", 12)
                       .AddOption("Not~interested.", 255)
                       .AddAvatar(129).FinalizeDialog();

                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Yes! This is the Handkerchief I made it, myself. Thanks for your help!")
                         .AddOption("No~problem.", 6)
                         .AddAvatar(129).FinalizeDialog();
                        if (!client.Inventory.Remove(711241, 1, stream)) break;
                        client.CreateBoxDialog("You`ve~completed~`Love~Confession~3`~and~received~500~Silver~and~EXP~worth~1/12~of~an~EXP~Ball.");
                        client.Player.Money += 200;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.GainExpBall(30, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);


                        break;
                    }
                case 6:
                    {
                        data.AddText("I`ve sewn some beautiful patterns on this handkerchief. Could you please help me send this Mandarin Duck Hanky to Xu Liang?")
                        .AddOption("Okay.", 7)
                        .AddAvatar(129).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.Player.QuestGUI.SendAutoPatcher("Go~send~the~Mandarin~Duck~Hanky~to~Xu~Liang.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                            client.Inventory.Add(stream, 711242);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check", 255)
                        .AddAvatar(129).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Wow, a Bouquet of Peach Blossoms! It`s from Xu Liang? Well, why don`t he send this by himself?")
                        .AddOption("Do~you~know~he~loves~you?", 2)
                        .AddAvatar(129).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Remove(711240, 1, stream)) break;
                        client.CreateBoxDialog("You`ve~completed~Love~Confession~2~and~received~200~silver~and~EXP~worth~1/30~of~an~EXP~Ball");
                        client.Player.Money += 200;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.GainExpBall(30, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);


                        goto case 3;
                    }
                case 3:
                    {
                        data.AddText("Well, I have always had a bit of a crush on him... I wanted to make a Handkerchief and give it to him, but it was stolen by the Apparitions!")
                      .AddText("~Could you please help me to retrieve it?")
                            .AddOption("No~problem.", 4)
                          .AddAvatar(129).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~kill~the~Apparitions~and~bring~back~the~Handkerchief~to~Ru~Hua.", 1002, 627, 526, 0);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.XuLiang)]
        public static void XuLiang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1008);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1009);//2365
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 2365);//1011
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1011);//1012
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1012);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.RuHua, client.Player.Class, 1014);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuLiang, client.Player.Class, 1015);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("My mission is to ensure the security and peace of Twin City. What are you doing here?")
                   .AddOption("Nothing.", 255)
                   .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Splendid! I`m madly in love with Ru Hua! I really appreciate your help! Hmm, I want to take her on a date beside the river at night.")
                                .AddText("~Please do me one last favor and send this message to her, okay?")
                     .AddOption("I`ll~go~now.", 255)
                     .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("My mission is to ensure the security and peace of Twin City. Hey, how is it going?")
                     .AddOption("I~have~something~for~you.", 30)
                     .AddOption("Nothing.", 255)
                     .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("My mission is to ensure the security and peace of Twin City. What are you doing here?")
                        .AddOption("Here~is~a~letter~for~you.", 1)
                        .AddOption("Nothing.", 255)
                        .AddAvatar(30).FinalizeDialog();
                            break;
                        }

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                            {
                                if (client.Inventory.HaveSpace(2) == false)
                                {
                                    data.AddText("Please make 2 more spaces in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(30).FinalizeDialog();
                                    break;
                                }
                                client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                                client.CreateBoxDialog("You~completed~`Robin~Elimination`~and~received~500~Silver,~some~EXP~and~3~Small~Joy~Stones!");
                                client.Player.Money += 500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.GainExpBall(200, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.AddItemWitchStack(720173, 0, 3, stream);
                                client.Inventory.Add(stream, 729508);
                                data.AddText("Good job! Thanks for your help! Here is a reward for you. Take it!")
                                  .AddOption("Okay,~see~you.", 8)
                                  .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("My mission is to ensure the security and peace of Twin City. You still want to take the Robins Elimination quest?")
                                    .AddOption("Absolutely.", 7)
                                    .AddOption("Nothing.", 255)
                                    .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                            goto case 4;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 100))
                            {
                                client.CreateBoxDialog("You~received~1,300~silver~and~13~minutes~of~EXP,~and~you`ll~receive~Double~EXP~for~the~next~25~minutes!");
                                client.Player.Money += 1200;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.RateExp = 2;
                                client.Player.DExpTime = 25 * 60;
                                client.Player.CreateExtraExpPacket(stream);
                                client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId);
                                goto case 20;
                            }

                            data.AddText("My~mission~is~to~ensure~the~security~and~peace~of~Twin~City.~What~are~you~doing~here?")
                                .AddOption("Experience~XP~skill.", 12)
                                .AddOption("Nothing.", 255)
                                .AddAvatar(30).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 5))
                            {
                                data.AddText("My mission is to ensure the security and peace of Twin City. Would you please do me a favor? ")
                      .AddOption("Sure~thing.", 24)
                      .AddOption(" Leave.", 255)
                      .AddAvatar(30).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("My mission is to ensure the security and peace of Twin City. Would you please do me a favor? ")
                        .AddOption("Sure~thing.", 22)
                        .AddOption(" Leave.", 255)
                        .AddAvatar(30).FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Well, I`ve made a Bouquet of Peach Blossoms , but I`m afraid that she may refuse me when she saw the flowers...")
                                .AddText("Nothing.")
                                .AddAvatar(30).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 31;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("My mission is to ensure the security and peace of Twin City. What are you doing here?")
                                 .AddText(" ~Can you help me to send this Bouquet of Peach Blossoms to Ru Hua?")
                         .AddOption("No problem.", 255)
                         .AddAvatar(30).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 27;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 20;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 8;
                        }


                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.Remove(711242, 1, stream)) break;
                        data.AddText("Wow, Ru Hua asked you to give me this Mandarin Duck Hanky? How terrific! She has accepted my love! I`m so happy! ")
                         .AddOption("Best~wishes~for~you!", 31)
                         .AddAvatar(30).FinalizeDialog();
                        client.CreateBoxDialog("You`ve~completed~`Love~Confession~4`~and~received~300~Silver~and~EXP~worth~1/20~of~an~EXP~Ball.");
                        client.Player.Money += 300;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.GainExpBall(15, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId);


                        break;
                    }
                case 31:
                    {
                        data.AddText("Splendid! I`m madly in love with Ru Hua! I really appreciate your help! Hmm, I want to take her on a date beside the river at night. ")
                                .AddText("~Please do me one last favor and send this message to her, okay?")
                            .AddOption("No~problem.", 32)
                            .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        data.AddText("Splendid! I`m madly in love with Ru Hua! I really appreciate your help! Hmm, I want to take her on a date beside the river at night.")
                              .AddText("~Please do me one last favor and send this message to her, okay?")
                          .AddOption("I`ll~go~now.", 255)
                          .AddAvatar(30).FinalizeDialog();
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Send~a~message~to~Ru~Hua.", ActiveQuest7.FinishNpcId.Map, ActiveQuest7.FinishNpcId.X, ActiveQuest7.FinishNpcId.Y, ActiveQuest7.FinishNpcId.ID);


                        break;
                    }
                case 26:
                    {
                        data.AddText("Well, I`ve made a bouquet of peach blossoms, but I`m afraid that she may refuse me when she sees the flowers... ")
                             .AddText("~Can you help me to send this Bouquet of Peach Blossoms to Ru Hua?")
                         .AddOption("My~pleasure.", 28)
                         .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 27:
                    {
                        data.AddText("My mission is to ensure the security and peace of Twin City. Well, I do need your help again. You won`t say no, right? ")
                               .AddText("~Can you help me to send this Bouquet of Peach Blossoms to Ru Hua?")
                           .AddOption("Just~tell~me.", 26)
                          .AddOption("Leave.", 255)
                           .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 711240);
                            client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                            client.Player.QuestGUI.SendAutoPatcher("Please~give~the~Bouquet~of~Peach~Blossoms~to~Ru~Hua.", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest5.MissionId, 1);
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in you inventory. ")
                         .AddOption("Let me check", 255)
                          .AddAvatar(30).FinalizeDialog();

                        }


                        break;
                    }
                case 24:
                    {
                        if (!client.Inventory.Remove(711239, 5, stream)) break;
                        data.AddText(" Wow, look at the beautiful peach blossoms! Thanks a lot! I`ll make a bouquet of peach blossoms and give them to Ru Hua!")
                         .AddOption("Good~luck!", 26)
                         .AddAvatar(30).FinalizeDialog();
                        client.CreateBoxDialog("You`ve~completed~`Love~Confession~1`~and~received~400~Silver~and~EXP~worth~1/15~of~an~EXP~Ball!");
                        client.Player.Money += 400;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        client.GainExpBall(15, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId);


                        break;
                    }
                case 22:
                    {
                        data.AddText("Hmm, I have been secretly in love with Ru Hua for a long time... But I`m still not courageous enough to tell her of my love. ")
                           .AddText("~Could you please help me pick 5 Peach Blossoms from the Peach Trees, so I can make a bouquet of Peach Blossoms to give to her?")
                       .AddOption("I`ll~go~now.", 255)
                       .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 20:
                    {
                        data.AddText(" Hmm, I have been secretly in love with Ru Hua for a long time... But I`m still not courageous enough to tell her of my love.")
                            .AddText("~Could you please help me pick 5 Peach Blossoms from the Peach Trees, so I can make a bouquet of Peach Blossoms to give to her?")

                        .AddOption("No~problem.", 21)
                        .AddOption(" Leave.", 255)
                        .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~pick~5~Peach~Blossoms~from~the~Peach~Trees,~nearby.", 1002, 480, 701, 7997);


                        break;
                    }
                case 1:
                    {
                        data.AddText("Oh, it`s a Plume Letter from the TC Vice Captain! Something must be urgent. Thanks for your help!")
                        .AddOption("You`re~welcome.", 2)
                        .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(711237, 1))
                        {
                            if (!client.Inventory.Remove(711237, 1, stream)) break;
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            client.CreateBoxDialog("You~completed~`Letter~Delivery~3`,~and~received~500~Silver~and~5~minutes~of~EXP!");
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.RateExp = 2;
                            client.Player.DExpTime = 5 * 60;
                            client.Player.CreateExtraExpPacket(stream);
                            data.AddText("Recently, lots of robins invaded the Wind Plain. Could you go kill 5 Robins and restore peace to Wind Plain?")
                     .AddOption("Piece~of~cake!", 4)
                     .AddAvatar(30).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Oh, where it`s a Plume Letter from the TC Vice Captain?")
              .AddOption("I lost that.", 255)
              .AddAvatar(30).FinalizeDialog();
                        }


                        break;
                    }

                case 4:
                    {
                        data.AddText("My mission is to ensure the security and peace of Twin City. Hey, I have 2 quests for you: Robins Elimination and Love Confession. Which one are you interested in?")
                            .AddOption("Robin~Elimination.", 5)
                            .AddOption("Nothing.", 255)
                            .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("By the way, Artisan Luo is looking for someone to help him gather some Robin Feather. Maybe you can give him a hand?")
                             .AddOption("Sure.", 6)
                             .AddAvatar(30).FinalizeDialog();
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~kill~10~Robins~and~then~return~to~Xu~Liang.", 1002, 468, 638, 0);


                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 442, 431, 4467);//4467,2,5510,1002,442,431
                        break;
                    }

                case 7:
                    {
                        data.AddText("Good. Then go kill 10 Robins and restore peace to Wind Plain!")
                              .AddOption("Piece~of~cake.", 255)
                              .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        Role.Flags.SpellID spell = (Role.Flags.SpellID)0;
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            spell = Role.Flags.SpellID.Cyclone;
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            spell = Role.Flags.SpellID.Superman;
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            spell = Role.Flags.SpellID.XpFly;
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            spell = Role.Flags.SpellID.FatalStrike;
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            spell = Role.Flags.SpellID.Oblivion;
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            spell = Role.Flags.SpellID.CannonBarrage;
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                            spell = Role.Flags.SpellID.DragonCyclone;
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            spell = Role.Flags.SpellID.Lightning;
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            spell = Role.Flags.SpellID.Omnipotence;

                        if (spell != 0)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)spell))
                                client.MySpells.Add(stream, (ushort)spell);
                        }
                        data.AddText("In this dangerous world, a powerful XP skill can make you stronger. Now I`ll teach you a unique XP skill, " + spell.ToString() + "")
                                .AddOption("What~is~it?", 10)
                                .AddOption("I`m~just~passing~by.", 255)
                                .AddAvatar(30).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            data.AddText("The Trojans have unique XP skill which disguises them as a robot and wreaks havoc amongst enemies.")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            data.AddText("The 'Superman' XP Skill is unique, possessed only by the Warrior. When activated it causes a death blow, decupled damage!")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            data.AddText("The ability to 'Fly' is the unique ability of the archer. Archers who fly are ")
                                .AddText(" immune to physical attacks and can shoot without fear of being attacked.")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            data.AddText("The ninja's skills can cause massive damage in a split second, or disable enemies, or prevent them from healing themselves.\n\n")
                                 .AddText("Fatal Strike Boosts Ninja's speed and attack, allowing the Ninja to blink between ")
                                 .AddText("targets and kill them instantly. This can only be used against monsters (not players).")
                               .AddOption("Kill~Robins.", 11)
                               .AddOption("I~see.", 255)
                               .AddAvatar(30).FinalizeDialog();

                        }
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            data.AddText("Move speed will increase. Inflict 200% damage on targets. For each monster killed within ")
                                .AddText(" 30 seconds after the XP skill is activated, the EXP awarded will be increased by 1/2.")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            data.AddText("(Cannon Barrage)  Inflict 400% damage on all monsters (100% damage on bosses), 10% of normal damage ")
                                .AddText(" on players within 5 paces every 2 seconds. ")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            data.AddText("The XP skill needs a certain period of time to accumulate its power. Once cast, you can kill a lot of monsters in a very short time. ")
                              .AddText("You`ve already learned DragonCyclone, and have 5 minutes to experience it. Use it to kill 100 Robins in ")
                              .AddText("~5 minutes, and you`ll be handsomely rewarded. What`s more, if you can kill double the amount, your reward will be even better!")
                            .AddOption("Kill~Robins.", 11)
                            .AddOption("I~see.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            data.AddText("Taoists with no magic is a sitting duck. They cast skills or magic from range and their  ")
                                .AddText("XP skills will cause even greater damage.\n")
                                .AddText("The XP skill needs a certain period of time to accumulate its power. Once cast, you can kill a lot of monsters in a very short time.")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                        {
                            data.AddText("Windwalker XP skill. Equip Windwalker fan in both hands to cast. Cast it to enter Omnipotence status which deals 10% of ")
                                .AddText("normal damage on enemies within 12 paces (350% damage on monsters), increases normal attack`s damage on ")
                                .AddText("monsters by 500%, and enhances the effects of Swirling Storm and Burnt Frost")
                              .AddOption("Kill~Robins.", 11)
                              .AddOption("I~see.", 255)
                              .AddAvatar(30).FinalizeDialog();
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Use your XP skill to kill 100 Robins!", 1002, 468, 638, 0);

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 100))
                        {
                            //  a
                        }
                        else
                        {
                            data.AddText("Use your XP skill to kill 100 Robins!")
                               .AddOption("Okay.", 255)
                               .AddAvatar(30).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
    }
}
