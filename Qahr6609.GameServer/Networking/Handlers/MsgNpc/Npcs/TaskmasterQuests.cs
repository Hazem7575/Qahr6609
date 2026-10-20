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
        [NpcAttribute(NpcID.JerkWang)]
        public static void JerkWang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntZhang, client.Player.Class, 1788);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.JerkWang, client.Player.Class, 1789);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.JerkWang, client.Player.Class, 1790);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Impossible is nothing. Nothing is impossible.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Where`s~Mo~Mo?", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`m~trying~find~Mo~Mo.", 3);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Do~you~have~any~solution?", 10);
                        data.AddOption("Just~do~it.", 255)
                        .AddAvatar(74).FinalizeDialog();


                        break;
                    }
                case 1:
                    {

                        data.AddText("It was all my fault. I should have kept an eye on him. I was reading. Mo Mo was out playing alone.");
                        data.AddOption("Then~what?", 2)
                           .AddAvatar(74).FinalizeDialog();

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
                        data.AddText("Some one saw him disappear in the east of the city all of a sudden. Will you find him for me?");
                        data.AddOption("Sure.", 3)
                            .AddOption("I`m~afraid.", 255)
                           .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1, 1))
                        {
                            if (HasItems(client, 721717, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(721717, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                                data.AddText("It`s all my fault... I should have been more careful...");
                                data.AddOption("What~shall~we~do?", 10)
                                   .AddAvatar(74).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Mo Mo was seen in these three spots: (269,215), (298,205) and (357,304). Please search there.");
                        data.AddOption("I`m~on~my~way.", 4)
                           .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721717, 1))
                            client.Inventory.Add(stream, 721717);
                        client.CreateBoxDialog("You`ve~obtained~Magic~Fireworks~from~the~Jerk~Wang.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 10:
                    {
                        data.AddText("Ah... now I remember, maybe you should visit Taoist Shine (245,258). He lives in Twin City")
                            .AddText("~and knows all the secrets behind peculiarity.");
                        data.AddOption("I`ll~visit~him.", 11).AddOption("I`m~not~going~anywhere.", 255)
                           .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        data.AddText("Taoist Shine (245,258) must know why. Please pay him a visit.");
                        data.AddOption("Good~idea!", 255)
                           .AddAvatar(74).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AuntZhang)]
        public static void AuntZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1787);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntZhang, client.Player.Class, 1788);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Dark_MoMo, client.Player.Class, 1793);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntZhang, client.Player.Class, 1794);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Childern are happiest when celebrating festivals...They are running around everywhere. But sometimes they might get lost...");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("You~should~be~careful.", 2);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                         && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Your~son~is~back!", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
               && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~found~your~son!", 11);
                        data.AddOption("I~agree.", 255)
                        .AddAvatar(155).FinalizeDialog();


                        break;
                    }
                case 10:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 6000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("If you forgot some skills or you qualifed for rebirth, you can learn skills in Job Center.", MsgMessage.ChatMode.System);
                            client.SendSysMesage("You've received EXP worth 1 EXP Ball and 6,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you for saving my son. Please take this as my token of my gratitude.");
                        data.AddOption("My~pleasure.", 11)
                           .AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("I really appreciate your help! Please go claim your reward from Taskmaster Chang!");
                        data.AddOption("I~will.", 12)
                           .AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Go~claim~your~reward~from~Taskmaster~Chang!");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
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
                        data.AddText("Mo Mo is lost! Please, you have to help me!");
                        data.AddOption("How~did~it~happen?", 2)
                           .AddAvatar(155).FinalizeDialog();


                        break;
                    }
                case 2:
                    {

                        data.AddText("Have you seen my son on your way here? His name is Mo Mo. He is a cute little boy, about this height... I couldn`t find him.")
                            .AddText("~He was with Jerk Wang (347,232), setting off Magic Fireworks, but he didn`t come back last night.");
                        data.AddOption("I`ll~give~him~a~visit.", 3)
                            .AddOption("Sorry~to~hear~that.", 255)
                           .AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Hurry~and~talk~to~Jerk~Wang~(TwinCity~347,232)!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
            }
        }
        //SuperHeadgearPack
        [NpcAttribute(NpcID.SuperHeadgearPack)]
        public static void SuperHeadgearPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 15)
                        {
                            data.AddOption("Super~Monk~Coronet.", 1);
                            data.AddOption("Super~Jade~Earring.", 9);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 25)
                            {
                                data.AddOption("Super~Violent~Helmet.", 2);
                                data.AddOption("Super~Jade~Earring.", 9);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 45)
                                {
                                    data.AddOption("Super~Antler~Hat.", 3);
                                    data.AddOption("Super~Jade~Earring.", 9);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 55)
                                    {
                                        data.AddOption("Super~Evil~Hood.", 4);
                                        data.AddOption("Super~Jade~Earring.", 9);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 65)
                                        {
                                            data.AddOption("Super~Headband~Of~Holy~Light.", 5);
                                            data.AddOption("Super~Jade~Earring.", 9);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 75)
                                            {
                                                data.AddOption("Super~Cruelty~Hat.", 6);
                                                data.AddOption("Super~Jade~Earring.", 9);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 85)
                                                {
                                                    data.AddOption("Super~Shadow~Hood.", 7);
                                                    data.AddOption("Super~Jade~Earring.", 9);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 145)
                                                {
                                                    data.AddOption("Super~Crane~Cap.", 8);
                                                    data.AddOption("Super~Jade~Earring.", 9);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Super~WindWalker~Cap.", 10);
                                                    data.AddOption("Super~Jade~Earring.", 9);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 101089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Crane~Cap!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 114089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Crane~Cap!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 148089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~received~an~Super~Shadow~Hood~and~an~EXP~Ball!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 144089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Cruelty~Hat.");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 143089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Headband~Of~Holy~Light!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 123089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Evil~Hood!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 113069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Antler~Hat!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 111089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Violent~Helmet!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 117089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Jade~Earring!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 118089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Monk~Coronet!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DoctorKnowitAll)]
        public static void DoctorKnowitAll(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1809);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1810);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1811);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1812);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1813);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1814);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1815);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1816);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1817);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("The~pill~isn`t~working!", 40);
                            data.AddOption("Are~you~nuts?");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("Help~Poor~Xiao~then!", 1);
                            data.AddOption("Whatever.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("What~do~you~need?", 10);
                            data.AddOption("Leave.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("How~will~you~help~Xiao?", 5);
                                data.AddOption("See~ya.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You must pay me a meteor and then I will make the Peel Pill for Xiao. The pill is made by Bird Blood, Ginseng Grease and Hawk Claw. But they are toxic and there are");
                                data.AddText("~side-effects when people take this...I can`t guarantee its effects either...you know...Anything is possible...");
                                data.AddOption("Let~me~ask~Xiao,~first.", 3);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                       && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("You~need~Bird~Blood,~right?", 14);
                                data.AddOption("See~ya~later.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("Tell~me~what~shall~I~do.", 12);
                                data.AddOption("No...");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
               && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("You~need~Bitter~Ginseng?", 21);
                                data.AddOption("Yes...");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("What~else~do~you~need?", 20);
                                data.AddOption("I`ll~catch~ya~later.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
         && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("You~need~Hawk~Claws?", 27);
                                data.AddOption("Yup.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("You~need~anything~else?", 26);
                                data.AddOption("Nope.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
&& client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("May~I~have~a~Peel~Pill?", 33);
                                data.AddOption("I~don`t~need~to~understand.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("How`s~going~with~the~pill?", 30);
                                data.AddOption("...");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
&& client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("What`s~the~problem~with~Xiao?", 44);
                            data.AddOption("Crazy.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();

                        }
                        else
                        {
                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("Whatever.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 44:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("According to my records, only Golden Rocs may suffer from this kind of syndrome...");
                            data.AddText("~General Yue is said to be the incarnation of a Golden Roc. So I have the theory, Xiao is his descendent.");
                            data.AddOption("I`ll~tell~him.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Calm down, let me check first ... According to my records, only Golden Rocs may suffer from this kind of syndrome...");
                            data.AddText("~General Yue is said to be the incarnation of a Golden Roc. So I have the theory, Xiao is his descendent.");
                            data.AddOption("I`ll~tell~him.", 43);
                            data.AddOption("You~can`t~be~serious.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 40:
                    {
                        data.AddText("I heard that General Yue, the great hero, was the incarnation of a Golden Roc. Is it possible that Poor Xiao is his descendent?");
                        data.AddOption("What?!", 41);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve received 100 Silver!");
                            data.AddText("Calm down, let me check first ... According to my records, only Golden Rocs may suffer from this kind of syndrome...");
                            data.AddText("~General Yue is said to be the incarnation of a Golden Roc. So I have the theory, Xiao is his descendent.");
                            data.AddOption("I`ll~tell~him.", 43);
                            data.AddOption("You~can`t~be~serious.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 43:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        break;
                    }
                case 33:
                    {
                        if (client.Inventory.Contain(721911, 1))
                        {
                            data.AddText("You`ve already got a Peel Pill!");
                            data.AddOption("Oh,~yes.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You lost it? You should have been more careful! Well, I will give you a new one.");
                            data.AddOption("Thank~you.", 34);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 34:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721911);
                            client.CreateBoxDialog("You`ve~received~the~Peel~Pill.~Hurry~to~take~it~to~Poor~Xiao!");
                        }
                        else
                        {
                            data.AddText("Hey, hey! Your inventory is full! Prepare some room, please.");
                            data.AddOption("Okay.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("Come come! I`ve made it, the Peel Pill! Now hurry up and take it to Poor Xiao.");
                        data.AddOption("Yes,~sir!", 31);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721911);
                            client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                            client.CreateBoxDialog("You`ve~received~the~Peel~Pill.~Hurry~to~take~it~to~Poor~Xiao!");
                            data.AddText("Come come! I`ve made it, the Peel Pill! Now hurry up and take it to Poor Xiao.");
                            data.AddOption("Yes,~sir!", 255);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hey, hey! Your inventory is full! Prepare some room, please.");
                            data.AddOption("Okay.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 27:
                    {
                        data.AddText("Only when you kill 2 of them within 1 minute, will you get a Hawk Claw. So have you got 5 Hawk Claws?");
                        data.AddOption("Yes.~Here~you~are.", 28);
                        data.AddOption("Not~yet.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();

                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Contain(721910, 5))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId);
                            if (!client.Inventory.Remove(721910, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve received 100 Silver!");
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721911);
                                client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                                client.CreateBoxDialog("You`ve~received~the~Peel~Pill.~Hurry~to~take~it~to~Poor~Xiao!");
                                data.AddText("Come come! I`ve made it, the Peel Pill! Now hurry up and take it to Poor Xiao.");
                                data.AddOption("Yes,~sir!", 255);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hey, hey! Your inventory is full! Prepare some room, please.");
                                data.AddOption("Okay.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("I need 5 Hawk Claws. Otherwise I`m not able to make the Peel Pill.");
                            data.AddOption("No~problem.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 26:
                    {
                        data.AddText("Good, now I need 5 Hawk Claws. You`re going to hunt the Level 93 Hawks. Only when you kill 2 of them within 1 minute, will you get a Hawk Claw.");
                        data.AddOption("I~see.", 25);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.CreateBoxDialog("Hurry~to~collect~5~Hawk~Claws!");
                        break;
                    }
                case 21:
                    {
                        data.AddText("Bitter Ginseng is made by Haw King`s Gallbladderr. You`ve got it?");
                        data.AddOption("Yes,~here~it~is.", 22);
                        data.AddOption("Not~yet.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain(721908, 1))
                        {
                            if (!client.Inventory.Remove(721908, 1, stream)) break;
                            client.Inventory.Remove(721912, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve received 100 Silver!");
                            client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId);
                            data.AddText("Good, now I need 5 Hawk Claws. You`re going to hunt the Level 93 Hawks. Only when you kill 2 of them within 1 minute, will you get a Hawk Claw.");
                            data.AddOption("I~see.", 25);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.Contain(721912, 1))
                            {
                                data.AddText("I need 1 Bitter Ginseng to make Ginseng Grease. Please take the Gallbladder drops from Haw King on Bird Island (451,676)");
                                data.AddOption("I~see.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("You lost the map? It`s okay. I`ll give you another.");
                                data.AddOption("Thank~you.", 23);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721912);
                            client.CreateBoxDialog("You`ve~got~a~Ginseng~Map.~Right~click~to~read~it.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Hurry to make some room!");
                            data.AddOption("I`ll~do~right~away.");
                            data.AddAvatar(67);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("It`s made by Haw King`s Gallbladderr. Here`s a map, telling you where the Bitter Ginseng could be found. Use the Gallbladderr at the spots");
                        data.AddText("~and you may find a Bitter Ginseng.");
                        data.AddOption("Gimme~the~map.", 17);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        data.AddText("I need 10 samples of Bird Blood. Have you got them?");
                        data.AddOption("Yes.~Here~you~are.", 15);
                        data.AddOption("Not~yet.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Contain(721906, 10))
                        {
                            data.AddText("Good job. Now, you are going to find a Bitter Ginseng.");
                            data.AddOption("How~to~get~one?", 16);//1272310
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hey, there! You don`t have enough Bird Blood. I need 10. You got it?");
                            data.AddOption("Yes.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 16:
                    {

                        if (client.Inventory.Contain(721906, 10))
                        {
                            if (!client.Inventory.Remove(721906, 10, stream)) break;
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You`ve received 100 Silver!");
                                data.AddText("It`s made by Haw King`s Gallbladderr. Here`s a map, telling you where the Bitter Ginseng could be found. Use the Gallbladderr at the spots");
                                data.AddText("~and you may find a Bitter Ginseng.");
                                data.AddOption("Gimme~the~map.", 17);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 17:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        if (client.Inventory.Contain(721912, 1))
                        {
                            data.AddText("You`ve got a map, already!");
                            data.AddOption("Alright.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721912);
                                client.CreateBoxDialog("You`ve~got~a~Ginseng~Map.~Right~click~to~read~it.");
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Hurry to make some room!");
                                data.AddOption("I`ll~do~right~away.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("I need a Meteor. Do you have?");
                        data.AddOption("Yes.~Here~it~is.", 11);
                        data.AddOption("Not~yet.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(1088001, 1))
                        {
                            if (HasItems(client, 1088001, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                if (!client.Inventory.Remove(1088001, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You`ve received 100 Silver!");
                                client.CreateBoxDialog("You~gave~the~doctor~a~Meteor.");
                                data.AddText("You`re kindhearted, huh?");
                                data.AddOption("Tell~me~the~recipe!", 12);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I need a Meteor. Do not bother me again unless you have got one.");
                            data.AddOption("Sorry.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        data.AddText("Well, first of all, you`re going to gather 10 samples of Bird Blood. You can get them from the Birdmen and Level 88 Birdmen.");
                        data.AddOption("I`ll~be~back~soon.", 13);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("I know the antidote for Poor Xiao`s weird disease!");
                        data.AddOption("What~is~it?", 2);
                        data.AddOption("Impress~me.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.SendSysMesage("You`ve received 100 Silver!");
                            data.AddText("You must pay me a meteor and then I will make the Peel Pill for Xiao. The pill is made by Bird Blood, Ginseng Grease and Hawk Claw. But they are toxic and there are");
                            data.AddText("~side-effects when people take this...I can`t guarantee its effects either...you know...Anything is possible...");
                            data.AddOption("Let~me~ask~Xiao,~first.", 3);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("You must pay me a meteor and then I will make the Peel Pill for Xiao. The pill is made by Bird Blood, Ginseng Grease and Hawk Claws. But they are toxic and there are");
                        data.AddText("~side-effects when people take this...I can`t guarantee its effects either...");
                        data.AddOption("Let~me~ask~Xiao,~first.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                        data.AddOption("How~will~you~help~Xiao?", 4);
                        data.AddOption("See~ya.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PoorXiao)]
        public static void PoorXiao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1808);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1809);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1810);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1811);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1815);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1816);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1817);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1818);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("You~knew~General~Yue?", 30);
                            data.AddOption("Bye.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("I`ve~got~the~pill.", 20);
                            data.AddOption("Just~passing~by.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1);
                            data.AddOption("Enjoy~yourself.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("Let~me~tell~you~something.", 10);
                            data.AddOption("Okay.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("I`ll~help~you.", 4);
                            data.AddOption("I`ll~leave~you~alone.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("The pill is toxic? And the Meteor, I don`t have any...");
                                data.AddOption("What`s~your~opinion?", 11);
                                data.AddAvatar(56);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                                data.AddOption("Have~you~made~your~choice?", 13);
                                data.AddOption("I`ll~be~back~soon.");
                                data.AddAvatar(56);
                                data.FinalizeDialog();
                            }
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                   && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("Here`s~the~antidote.", 22);
                            data.AddOption("I`m~busy.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("I`m~leaving.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
               && client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Thank you, for bringing me the pill and helping me know my identity! Please go claim your reward from Taskmaster Chang.");
                            data.AddOption("I~will.", 33);
                            data.AddOption("Later.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("I`m~leaving.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 33:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest8, 0);
                        client.CreateBoxDialog("Go~claim~your~reward~from~Taskmaster~Chang!");
                        break;
                    }
                case 30:
                    {
                        data.AddText("My parents passed away when I was young, leaving only a Jade Pendant. The words inscribed on it say, `Serve thy country`.");
                        data.AddText("~Now I know I am one of the descendents of General Yue, the great hero.");
                        data.AddOption("That`s~true.", 31);
                        data.AddAvatar(7);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Since I`ve known my identity, I`ll work hard and never disappoint my ancestors. Thank you! Please go claim your reward from Taskmaster Chang.");
                        data.AddOption("My~pleasure.", 32);
                        data.AddAvatar(7);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.Player.Money += 6000;
                            client.GainExpBall(600);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.CreateBoxDialog("You`ve~gained~EXP~worth~1~EXP~Ball~and~6,000~Silver.~Hurry~to~claim~your~reward~from~Taskmaster~Chang!");
                            client.Player.QuestGUI.Accept(ActiveQuest8, 0);
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("You`ve got the pill? Great!");
                        data.AddOption("Hurry~to~take~it!", 21);
                        data.AddAvatar(56);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {

                        if (client.Inventory.Contain(721911, 1))
                        {
                            if (HasItems(client, 721911, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                if (!client.Inventory.Remove(721911, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You`ve received 100 Silver!", MsgMessage.ChatMode.System);
                                data.AddText("Oh my god! Are you sure it is the antidote? The wings do not disappear! No, look at my face! It turns golden!");
                                data.AddOption("I`ll~ask~the~doctor!", 23);
                                data.AddAvatar(56);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Are you kidding me? Please, go find Doctor Know-it-All right away.");
                            data.AddOption("Okay.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Oh my god! Are you sure it is the antidote? The wings do not disappear! No, look at my face! It turns golden!");
                            data.AddOption("I`ll~ask~the~doctor!");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Oh my god! Are you sure it is the antidote? The wings do not disappear! No, look at my face! It turns golden!");
                            data.AddOption("I`ll~ask~the~doctor!", 23);
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 23:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 13:
                    {
                        data.AddText("I will try it, no matter what might happen. But I still need a Meteor to complete the prescription of the Peel Pill.");
                        data.AddOption("Don`t~worry.");
                        data.AddAvatar(56);

                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("The pill is toxic? And the Meteor, I don`t have any...");
                        data.AddOption("What`s~your~opinion?", 11);
                        data.AddAvatar(56);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve received 100 Silver!", MsgMessage.ChatMode.System);
                            data.AddText("Alright, I`m willing to have a try. But you see, I`m poor and it`s impossible for me to afford a Meteor...");
                            data.AddOption("You~can~count~on~me.", 12);
                            data.AddOption("Sorry~about~it.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1809, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Thank you. Please find Doctor Know-it-All on Bird Island (701,595) as soon as possible.");
                            data.AddOption("I`m~on~my~way.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Thank you. Please find Doctor Know-it-All on Bird Island (701,595) as soon as possible.");
                            data.AddOption("I`m~on~my~way.", 3);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Hello! Please don`t get scared by me...These ugly feathers grew all over my body out of a blue. I learnt that");
                        data.AddText("~Doctor Know-it-All on Bird Island (701,595) has a special prescription for this. Will you give me a hand?");
                        data.AddOption("Of~course~I~will.", 2);
                        data.AddOption("Sorry,~not~now.");
                        data.AddAvatar(56);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.SendSysMesage("You`ve received 100 Silver!", MsgMessage.ChatMode.System);
                            data.AddText("Thank you. Please find Doctor Know-it-All on Bird Island (701,595) as soon as possible.");
                            data.AddOption("I`m~on~my~way.", 3);
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
            }
        }


        //ClothingPack
        [NpcAttribute(NpcID.ClothingPack)]
        public static void ClothingPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 15)
                        {
                            data.AddOption("Super~Conquest~Armor.", 1);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 25)
                            {
                                data.AddOption("Super~Phoenix~Armor.", 2);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 45)
                                {
                                    data.AddOption("Super~Phoenix~Jerkin.", 3);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 55)
                                    {
                                        data.AddOption("Super~Owl~Vest.", 4);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 65)
                                        {
                                            data.AddOption("Super~Robe~Of~The~Buddha.", 5);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 75)
                                            {
                                                data.AddOption("Super~Whale~Coat.", 6);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 85)
                                                {
                                                    data.AddOption("L120~Super~Yellow~Combat~Suit.", 7);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 145)
                                                {
                                                    data.AddOption("Super~Pine~Robe.", 8);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Super~WindWalker~Armor.", 9);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 170109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Conquest~Armor(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 130109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Conquest~Armor(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 131109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Phoenix~Armor(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 133109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Phoenix~Jerkin(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 135109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Owl~Vest(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 136109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Robe~Of~The~Buddha(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 139109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~WhaleCoat.");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 138109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~received~an~Level~120~Super~Yellow~Combat~Suit~and~an~EXP~Ball!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 134109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Pine~Robe(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TaskmasterChang)]
        public static void TaskmasterChang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1718);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefWong, client.Player.Class, 1722);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1723);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1726);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1727);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ScholarWu, client.Player.Class, 1732);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1733);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1737);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1738);
            var ActiveQuest10 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Minstrel, client.Player.Class, 1744);
            var ActiveQuest11 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1745);
            var ActiveQuest12 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1750);
            var ActiveQuest13 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1751);
            var ActiveQuest14 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KungfuMaster, client.Player.Class, 1757);
            var ActiveQuest15 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1781);
            var ActiveQuest16 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DuSan, client.Player.Class, 1786);

            var ActiveQuest17 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1787);
            var ActiveQuest18 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntZhang, client.Player.Class, 1794);

            var ActiveQuest19 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1795);
            var ActiveQuest20 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Carolyn, client.Player.Class, 1799);



            var ActiveQuest21 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1800);
            var ActiveQuest22 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntPeach, client.Player.Class, 1807);

            var ActiveQuest23 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1808);
            var ActiveQuest24 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1818);

            var ActiveQuest25 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1819);
            var ActiveQuest26 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1827);


            var ActiveQuest27 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1828);
            var ActiveQuest28 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1834);

            var ActiveQuest29 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1835);
            var ActiveQuest30 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1839);

            var ActiveQuest31 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1840);
            var ActiveQuest32 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1852);

            var ActiveQuest33 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 200);
            switch (Option)
            {
                case 33:
                case 0:
                    {

                        data.AddText("Everyone wants to be a hero, and so do I, but Rome wasn`t built in a day.")
                          .AddText("~If you are dreaming of becoming a celebrity around here, I can help you get prepared. Please be noted that Reincarnated players can`t accept quests from me.");
                        data.AddOption("The~Connector~(" + GetStrTask(client, ActiveQuest2) + ").", 1);
                        data.AddOption("Spun~Gold~Armor~(" + GetStrTask(client, ActiveQuest4) + ").", 4)
                            .AddOption("Azure~Pill~(" + GetStrTask(client, ActiveQuest6) + ").", 7)
                            .AddOption("Artisan`s~Feather~(" + GetStrTask(client, ActiveQuest8) + ").", 10)
                            .AddOption("Mystic~Ring~(" + GetStrTask(client, ActiveQuest10) + ").", 13)
                            .AddOption("Next~page.", 16)
                            .AddOption("Wait,~I`m~not~ready~yet!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }

                case 16:
                    {

                        data.AddText("Everyone wants to be a hero, and so do I, but Rome wasn`t built in a day.")
                         .AddText("~If you are dreaming of becoming a celebrity around here, I can help you get prepared. Please be noted that Reincarnated players can`t accept quests from me.");
                        data.AddOption("Frantic~Monkeys~(" + GetStrTask(client, ActiveQuest12) + ").", 17)
                             .AddOption("Soul~of~General~(" + GetStrTask(client, ActiveQuest14) + ").", 20)
                                .AddOption("Mad~Man~(" + GetStrTask(client, ActiveQuest16) + ").", 24)
                                .AddOption("Ghost~City~(" + GetStrTask(client, ActiveQuest18) + ").", 30)
                                .AddOption("Tomboy`s~Marriage~(" + GetStrTask(client, ActiveQuest20) + ").", 40)
                                .AddOption("Desert~Blood~(" + GetStrTask(client, ActiveQuest22) + ").", 50)
                                .AddOption("Previous.", 33)
                            .AddOption("Next~page.", 55)
                           .AddAvatar(72).FinalizeDialog();
                        break;
                    }
                case 55:
                    {
                        data.AddText("Everyone wants to be a hero, and so do I, but Rome wasn`t built in a day.")
                    .AddText("~If you are dreaming of becoming a celebrity around here, I can help you get prepared. Please be noted that Reincarnated players can`t accept quests from me.");
                        data.AddOption("Hero`s~Wings~(" + GetStrTask(client, ActiveQuest24) + ").", 60)
                            .AddOption("Family~Feud~(" + GetStrTask(client, ActiveQuest26) + ").", 70)
                            .AddOption("Desert~Treasure~(" + GetStrTask(client, ActiveQuest28) + ").", 80)
                             .AddOption("Desert~Expedition~(" + GetStrTask(client, ActiveQuest30) + ").", 90)
                             .AddOption("Power~Book~(" + GetStrTask(client, ActiveQuest32) + ").", 100)
                                .AddOption("TempestWings.~(" + GetStrTask(client, ActiveQuest33) + ").", 110)
                              .AddOption("Previous.", 16)
                            //Desert~Expedition~
                           .AddAvatar(72).FinalizeDialog();
                        break;
                    }
                case 110:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 120 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest33.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The~Tempest~Wings~have~appeared!~It`s~an~intelligent~talisman~to~boost~your~strength,~as~well~as~a~dazzling~symbol~of~nobility,");
                            data.AddText("~making~you~stand~out~in~a~second.~Wanna~be~the~Champion~of~champions?~Go~and~find~Mr.~Wonder~(212,196)~in~Phoenix~Castle");
                            data.AddText("~to~forge~a~pair~of~Tempest~Wings.");
                            data.AddOption("Take~me~to~see~Mr.~Wonder.", 111);
                            data.AddOption("Close.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest33.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("You`ve~already~completed~the~quest,~haven`t~you?");
                                data.AddOption("Yeah,~you`re~right.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The~Tempest~Wings~have~appeared!~It`s~an~intelligent~talisman~to~boost~your~strength,~as~well~as~a~dazzling~symbol~of~nobility,");
                                data.AddText("~making~you~stand~out~in~a~second.~Wanna~be~the~Champion~of~champions?~Go~and~find~Mr.~Wonder~(212,196)~in~Phoenix~Castle");
                                data.AddText("~to~forge~a~pair~of~Tempest~Wings.");
                                data.AddOption("Take~me~to~see~Mr.~Wonder.", 111);
                                data.AddOption("Close.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }

                case 111:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest33, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 212, 196, 10850);
                        break;
                    }
                case 100:
                    {
                        if (client.Player.Level < 115)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 115 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest32.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;//1262670
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest32.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("There`s an Old Beggar at the entrance of the Labyrinth (263,135). He has a very special book about the essence of all the secrets behind Chinese Martial Arts.");
                            data.AddText("If you are interested in it, pay a visit to him.");
                            data.AddOption("Claim~rewards.", 102);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest31.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("There`s an Old Beggar at the entrance of the Labyrinth (391,235). He has a very special book about the essence of all the secrets behind Chinese Martial Arts.");
                                data.AddText("If you are interested in it, pay a visit to him.");
                                data.AddOption("Accept~the~quest.", 101);
                                data.AddOption("I~see.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("Sorry, but you didn`t complete the quest.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 102:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest32.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728251, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~completed~the~quest~and~received~a~L120~Super~Clothing~Pack!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                        }
                        break;
                    }
                case 101:
                    {
                        client.CreateBoxDialog("Hurry~to~pay~a~visit~to~the~Old~Beggar.");
                        client.Player.QuestGUI.Accept(ActiveQuest31, 0);
                        break;
                    }
                case 90:
                    {
                        if (client.Player.Level < 105)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 105 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest30.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;//1262670
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest30.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Master Mo Mo in Desert City (520,589) is a nobleman who is fond of making gunpowder and researching new weapons. But");
                            data.AddText("~he seems to be in trouble. Help him. When it is done, you can claim rewards from me.");
                            data.AddOption("Claim~rewards.", 92);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest29.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("Master Mo Mo in Desert City (520,589) is a nobleman who is fond of making gunpowder and researching new weapons. But");
                                data.AddText("~he seems to be in trouble. Help him. When it is done, you can claim rewards from me.");
                                data.AddOption("Accept~the~quest.", 91);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("Sorry, but you didn`t complete the quest.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 92:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest30.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728250, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~completed~the~quest~and~received~a~L110~Super~Ring~Pack!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                        }
                        break;
                    }
                case 91:
                    {
                        client.CreateBoxDialog("Hurry~up~to~help~Master~Mo~Mo~in~Desert~City!");
                        client.Player.QuestGUI.Accept(ActiveQuest29, 0);
                        break;
                    }
                case 80://1264450
                    {
                        if (client.Player.Level < 98)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 98 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest28.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest28.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Louis in Desert City (466,649) is looking for the Zen Scripture, the Victory Portrait and the Buddha Relic. He needs your help.");
                            data.AddOption("Claim~rewards.", 82);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            //1264340
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest27.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("Louis in Desert City (466,649) is looking for the Zen Scripture, the Victory Portrait and the Buddha Relic. He needs your help.");
                                data.AddOption("Accept~the~quest.", 81);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("Sorry, but you didn`t complete the quest.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 82:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest28.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728249, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~completed~the~quest~and~received~a~L100~Super~Boots~Pack!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                        }

                        break;
                    }

                case 81:
                    {
                        client.CreateBoxDialog("Hurry~up~to~help~Louis~in~Desert~City.");
                        client.Player.QuestGUI.Accept(ActiveQuest27, 0);
                        break;
                    }
                case 70:
                    {
                        if (client.Player.Level < 95)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 95 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest26.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest26.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Felix on Bird Island (686,547) is plagued by the fact that his parents were killed by his foster father, who is a Bandit");
                            data.AddText("~Leader. Help him seek revenge in honor of his parents. When it is done, you can claim rewards from me.");
                            data.AddOption("Claim~rewards.", 72);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();

                            //1264340
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest25.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("Felix on Bird Island (686,547) is plagued by the fact that his parents were killed by his foster father, who is a Bandit");
                                data.AddText("~Leader. Help him seek revenge in honor of his parents. When it is done, you can claim rewards from me.");
                                data.AddOption("Accept~the~quest.", 71);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("Sorry, but you didn`t complete the quest.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 72:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest26.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728248);
                                client.CreateBoxDialog("You~completed~the~quest~and~received~a~L97~Super~Necklace~Pack!");
                            }
                        }
                        else
                            client.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                        break;
                    }
                case 71:
                    {
                        client.CreateBoxDialog("Hurry~up~to~help~Felix~on~Bird~Island!");
                        client.Player.QuestGUI.Accept(ActiveQuest25, 0);
                        // 1264320
                        break;
                    }
                case 60:
                    {
                        if (client.Player.Level < ActiveQuest24.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest24.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Hero`s Wings`, haven`t you?");
                            data.AddOption("Oh,~yeah.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest24.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Poor Xiao on Bird Island (725,541) caught a strange disease which made his body covered by plumes. You need to visit");
                            data.AddText("~Doctor Know-it-All for him.");
                            data.AddOption("Done!~Claim~my~reward!", 61);
                            data.AddOption("Leave.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest23.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("Poor Xiao on Bird Island (725,541) caught a strange disease which made his body covered by plumes. You need to visit");
                                data.AddText("~Doctor Know-it-All for him.");
                                data.AddOption("You~can~count~on~me.", 62);
                                data.AddOption("None~of~my~business.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Hero`s Wings`!");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 62:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest23.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.CreateBoxDialog("Hurry~up~and~set~off~right~away!");
                        }
                        else
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest23, 0);
                            client.CreateBoxDialog("Hurry~up~and~set~off~right~away!");
                        }
                        break;
                    }
                case 61:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest24.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728247);
                                client.CreateBoxDialog("You`ve~completed~the~quest~`Hero`s~Wings`~and~received~a~Super~Headgear~Pack!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~Make~some~room~first,~and~then~claim~the~reward,~later!");
                        }

                        break;
                    }
                case 50:
                    {
                        if (client.Player.Level < ActiveQuest22.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest22.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Desert Blood`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest22.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Harvey, in Desert City (495,579) is searching for a Divine Sword once possessed by an Obscure Warrior.")
                    .AddText("~Go try to help him.");
                            data.AddOption("Done!~Claim~my~reward!", 52)
                                .AddOption("Leave.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest21.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest21.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Desert Blood`!");
                            data.AddOption("Alright.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest21.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            data.AddText("Harvey, in Desert City (495,579) is searching for a Divine Sword once possessed by an Obscure Warrior.")
                  .AddText("~Go try to help him.");
                            data.AddOption("You~can~count~on~me.", 51)
                                .AddOption("None~of~my~business.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Hurry~up~and~set~off~right~away!");
                        client.Player.QuestGUI.Accept(ActiveQuest21, 0);
                        break;
                    }
                case 52:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest22.MissionId))
                        {
                            client.Inventory.Add(stream, 728242);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Desert~Blood`~and~received~a~Level~90~Elite~Ring~Pack!");
                        }

                        break;
                    }
                case 40:
                    {
                        if (client.Player.Level < ActiveQuest20.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest20.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the `Tomboy`s Marriage` quest, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest20.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Farmer Lynn in Desert City (461,661) feels great anxiety about his daughter, Carolyn, who still isn`t married at the age of 30.")
                    .AddText("~Will you go talk to him? I`ve prepared a whopping reward for a hero kind enough to help him out.");
                            data.AddOption("Claim~rewards.", 42)
                                .AddOption("Leave.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest19.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                      || client.Player.QuestGUI.CheckQuest(ActiveQuest19.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest: `Tomboy`s Marriage`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest19.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            data.AddText("Farmer Lynn in Desert City (461,661) feels great anxiety about his daughter, Carolyn, who still isn`t married at the age of 30.")
                  .AddText("~Will you go talk to him? I`ve prepared a whopping reward for a hero kind enough to help him out.");
                            data.AddOption("You~can~count~on~me.", 41)
                                .AddOption("None~of~my~business.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        break;
                    }
                case 42:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest20.MissionId))
                        {
                            client.Inventory.Add(stream, 728241);
                            client.CreateBoxDialog("You`ve~completed~the~`Tomboy`s~Marriage`~quest~and~received~a~Level~82~Elite~Necklace~Pack!");
                        }
                        break;
                    }
                case 41:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest19, 0);
                        client.CreateBoxDialog("Hurry~up~and~set~off,~right~away!");
                        break;
                    }

                case 30:
                    {
                        if (client.Player.Level < ActiveQuest18.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest18.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Ghost City`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest18.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Aunt Zhang (361,459) is looking everywhere for her son Mo Mo. He went missing in Twin City,")
                    .AddText("~the Taoist Shine should know the reason beneath this mystery.");
                            data.AddOption("Done!~Claim~my~reward!", 32)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest17.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                         || client.Player.QuestGUI.CheckQuest(ActiveQuest17.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Ghost City`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest17.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            data.AddText("Aunt Zhang (240,326) is looking everywhere for her son Mo Mo. He went missing in Twin City, and his uncle believed that")
                  .AddText("~the Taoist Shine should know the reason beneath this mystery.");
                            data.AddOption("You~can~count~on~me.", 31)
                                .AddOption("None~of~my~business.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Hurry~up~and~set~off~right~away!");
                        client.Player.QuestGUI.Accept(ActiveQuest17, 0);
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest18.MissionId))
                        {
                            client.Inventory.Add(stream, 728246);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Ghost~City`~and~received~a~Level~87~Elite~Armor~Pack!");
                        }
                        break;
                    }
                case 24:
                    {
                        if (client.Player.Level < ActiveQuest16.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest16.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Mad~Man`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest16.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Du San in Ape City (552,601) was the nephew of General Du (396,233). Du San turned lunatic out of the blue, with snakebites all")
                    .AddText("~over his body. General needs somebody to kill those Snake Men for revenge.");
                            data.AddOption("Done!~Claim~my~reward!", 26)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest15.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                          || client.Player.QuestGUI.CheckQuest(ActiveQuest15.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Mad~Man`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Du San in Ape City (552,601) was the nephew of General Du (396,233). Du San turned lunatic out of the blue, with snakebites all")
                   .AddText("~over his body. General needs somebody to kill those Snake Men for revenge.");
                        data.AddOption("You~can~count~on~me.", 25)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 26:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest16.MissionId))
                        {
                            client.Inventory.Add(stream, 728245);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Mad~Man`~and~received~a~Level~67~Elite~Headgear~Pack!");
                        }

                        break;
                    }
                case 25:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest15, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~up~and~set~off~right~away!", ActiveQuest15.FinishNpcId.Map, ActiveQuest15.FinishNpcId.X, ActiveQuest15.FinishNpcId.Y, ActiveQuest15.FinishNpcId.ID);

                        break;
                    }
                case 20:
                    {
                        if (client.Player.Level < ActiveQuest14.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest14.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Soul~of~General`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest14.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("General Amber (879,562) in Ape City lived a heroic life. But his spirit couldn`t rest in peace because of unfinished business.")
                                .AddText("~He needs your help.");
                            data.AddOption("Done!~Claim~my~reward!", 22)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest13.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                              || client.Player.QuestGUI.CheckQuest(ActiveQuest13.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Soul of General`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("General Amber (879,562) in Ape City lived a heroic life. But his spirit couldn`t rest in peace because of unfinished business.")
                   .AddText("~He needs your help.");
                        data.AddOption("You~can~count~on~me.", 21)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 22:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest14.MissionId))
                        {
                            client.Inventory.Add(stream, 728244);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Soul~of~General`~and~received~a~Level~60~Elite~Boot~Pack!");
                        }
                        break;
                    }
                case 21:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest13, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~up~and~set~off~right~away!", ActiveQuest13.FinishNpcId.Map, ActiveQuest13.FinishNpcId.X, ActiveQuest13.FinishNpcId.Y, ActiveQuest13.FinishNpcId.ID);
                        break;

                    }
                case 17:
                    {
                        if (client.Player.Level < ActiveQuest12.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest12.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Frantic Monkeys`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest12.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ape City used to be at peace. But the monkeys outside the city strangely went frantic and began to attack villagers. Officer")
                                           .AddText("~Bao (553,584) needs your help to find out why.");
                            data.AddOption("Done!~Claim~my~reward!", 19)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest11.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                             || client.Player.QuestGUI.CheckQuest(ActiveQuest11.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Franti Monkeys`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Ape City used to be at peace. But the monkeys outside the city strangely went frantic and began to attack villagers. Officer")
                   .AddText("~Bao (553,584) needs your help to find out why.");
                        data.AddOption("You~can~count~on~me.", 18)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 19:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest12.MissionId))
                        {
                            client.Inventory.Add(stream, 728243);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Frantic~Monkeys`~and~received~a~Level~55~Elite~Weapon~Pack!");
                        }

                        break;
                    }
                case 18:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest11, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~up~and~set~off~right~away!", ActiveQuest11.FinishNpcId.Map, ActiveQuest11.FinishNpcId.X, ActiveQuest11.FinishNpcId.Y, ActiveQuest11.FinishNpcId.ID);

                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < ActiveQuest10.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest10.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the `Antique Ring` quest, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest10.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("A Minstrel (184,282) came to Phoenix Castle looking for the Mystic Ring. He heard that Antique Dealer Shen (779,477) has an Antique Ring")
                .AddText("~that can be converted into the Mystic Ring, when combined with a Meteor. He needs you to visit Dealer Shen and learn more.");
                            data.AddOption("Done!~Claim~my~reward!", 15)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                              || client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest: `Mystic Ring`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("A Minstrel (184,282) came to Phoenix Castle looking for the Mystic Ring. He heard that Antique Dealer Shen (779,477) has an Antique Ring")
                   .AddText("~that can be converted into the Mystic Ring, when combined with a Meteor. He needs you to visit Dealer Shen and learn more.");
                        data.AddOption("You~can~count~on~me.", 14)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 15:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest10.MissionId))
                        {
                            client.Inventory.Add(stream, 728240);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Mystic~Ring`~and~received~a~Level~52~Unique~Headgear~Pack!");
                        }
                        break;
                    }
                case 14:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~up~and~set~off,~right~away!", ActiveQuest9.FinishNpcId.Map, ActiveQuest9.FinishNpcId.X, ActiveQuest9.FinishNpcId.Y, ActiveQuest9.FinishNpcId.ID);


                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < ActiveQuest8.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Azure Pill`!");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Apprentice Luo (TwinCity 447,331) is waiting for his father, Artisan Luo. He has been out collecting feathers near the river (442,431) for")
                                .AddText("~days, and his son is growing more and more worried that he hasn`t returned. He needs your help to find his father.");
                            data.AddOption("Done!~Claim~my~reward!", 12)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                           || client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, unless you complete the quest: `Artisan`s Feather`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Apprentice Luo (TwinCity 447,331) is waiting for his father, Artisan Luo. He has been out collecting feathers near the river (442,431) for")
                      .AddText("~days, and his son is growing more and more worried that he hasn`t returned. He needs your help to find his father.");
                        data.AddOption("You~can~count~on~me!", 11)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.Inventory.Add(stream, 728239);
                            client.CreateBoxDialog("You`ve~completed~the~`Artisan`s~Feather`~quest~and~received~a~Level~50~Unique~Weapon~Pack!");
                        }
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~find~Artisan~Luo!", ActiveQuest7.FinishNpcId.Map, ActiveQuest7.FinishNpcId.X, ActiveQuest7.FinishNpcId.Y, ActiveQuest7.FinishNpcId.ID);

                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < ActiveQuest6.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You`ve already completed the quest `Azure Pill`!");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Scholar Wu (Phoenix Castle,214 231) has been suffering from a weird asthma for years. He heard Pharmacist Mu Mu (182,241) could")
                     .AddText("~cure his disease if some rare ingredients are at hand. He now needs you to give him a hand and collect the ingredients.");
                            data.AddOption("Done!~Claim~my~reward!", 9)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Azure Pill`!");
                            data.AddOption("Got~it.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Scholar Wu (Phoenix Castle,214 231) has been suffering from a weird asthma for years. He heard Pharmacist Mu Mu (182,241) could")
                      .AddText("~cure his disease if some rare ingredients are at hand. He now needs you to give him a hand and collect the ingredients.");
                        data.AddOption("I~won`t~let~you~down.", 8)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Inventory.Add(stream, 728238);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Azure~Pill`~and~received~a~Level~43~Unique~Ring~Pack!");
                        }
                        break;
                    }
                case 8:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`re~going~to~help~Scholar~Wu~collect~the~ingredients.", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
                case 4:
                    {
                        if (client.Player.Level < ActiveQuest4.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Hey! You`ve already completed the quest: `Spun Gold Armor`!");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Thief Wong (347,228) has stolen some jewelry from the notorious landlords. He wants to give these treasures to the poor in Maple")
                          .AddText("~the Lustful and his gangsters have something to do with it. You will need to disguise yourself and set a trap to capture him.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddOption("Done!~Claim~my~reward.", 6)
                           .AddOption("Later.", 255)
                          .AddAvatar(72).FinalizeDialog();
                        }
                        else
                        {
                            data.AddOption("You~can~count~on~me!", 5)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 729974, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(729974, 1, stream)) break;
                            client.Inventory.Add(stream, 728237);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Spun~Gold~Armor`~and~received~a~Level~32~Unique~Armor~Pack!");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.Level < ActiveQuest3.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.Player.QuestGUI.SendAutoPatcher("You~have~accepted~the~quest:~`Spun~Gold~Armor`.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I won`t give you anything, you know, until you complete the quest: `Spun Gold Armor`!");
                            data.AddOption("Got~it.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.Level < ActiveQuest2.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve~already~completed~the~quest:~`The~Connector`!");
                            data.AddOption("Yep.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Thief Wong (347,228) has stolen some jewelry from the notorious landlords. He wants to give these treasures to the poor in Maple")
                       .AddText("~Forest, but Thief Chen, his connector, is being cut off by Bandits. I need you to go and help them. It`s very urgent!");
                            data.AddOption("Done!~Claim~my~reward.", 3)
                                .AddOption("I`ll~be~back~soon.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I won`t give you anything, you know, because you haven`t finished the quest `The Connector`, yet!");
                            data.AddOption("Got~it.", 255)
                               .AddAvatar(72).FinalizeDialog();

                            break;
                        }
                        data.AddText("Thief Wong (347,228) has stolen some jewelry from the notorious landlords. He wants to give these treasures to the poor in Maple")
                         .AddText("~Forest, but Thief Chen, his connector, is being cut off by Bandits. I need you to go and help them. It`s very urgent!");
                        data.AddOption("You~can~count~on~me!", 2)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~accepted~the~quest:~`The~Connector`!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, 120046);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`The~Connector`~and~received~a~Refined~Necklace~Pack!");
                        }
                        break;
                    }
            }
        }
    }
}
