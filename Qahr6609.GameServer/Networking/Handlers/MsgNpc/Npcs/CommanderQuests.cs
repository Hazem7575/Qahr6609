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
        [NpcAttribute(NpcID.ArenaGuard)]
        public static void ArenaGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("A suited weapon is a thing that you can`t do without nowadays. I keep faith in it.");
                        data.AddOption("Attractive.");
                        data.AddAvatar(94);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.IronsmithChou)]
        public static void IronsmithChou(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m actually a dead man. Hey, don`t be afraid. Before I died, I had made numerous helmets");
                        data.AddText("~for the emperor and his generals. But one day, I made a big mistake and were killed. After that, I");
                        data.AddText("~decided to make helmets for anyone who is honest and diligent.");
                        data.AddOption("That`s~good.");
                        data.AddAvatar(85);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PrizeOfficer2)]
        public static void PrizeOfficer2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("All your rewards have been sent to your mail. Don`t forget to check and see whether you have any reward not claimed.");
                        data.AddOption("Okay.");
                        data.AddAvatar(10);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BirdExorcist)]
        public static void BirdExorcist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Sigh, I should have felt the danger earlier. The Ancient Devil sealed away a thousand years ago is gonna wake up! I`m afraid a war is inevitable.");
                        data.AddOption("What~can~I~do?", 1);
                        data.AddOption("Goodness!~I`d~better~flee!");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Bring your people together, and collect amulets of 7 classes by completing the given tests in the Ancient Island. If you have Ninjas, Monks, Saints or Pirates in your team,");
                        data.AddText("~they`ll get the amulets after finishing the test of any classes.");
                        data.AddOption("Okay,~what~to~do~next?", 2);
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("If your team holds a Monk Amulet, use that amulet to combine all the other amulets. If not, ask any one in your team to take all the 7 amulets and turn on a shining yellow switch");
                        data.AddText("~to bring out the Ancient Devil. Kill the devil before its power fully recovered! So, what do you say?");
                        data.AddOption("I~love~challenge!", 3);
                        data.AddOption("I~need~a~break!");
                        data.AddAvatar(87);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level < 50)
                        {
                            client.CreateBoxDialog("You~should~be~Level~50~or~above~to~take~this~quest.");
                        }
                        else
                        {
                            client.Teleport(191, 232, 1052);
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.CommanderKerry)]
        public static void CommanderKerry(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1825);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuideZhang, client.Player.Class, 1826);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("My friend. Can you tell Felix that I miss him so much?");
                            data.AddOption("Yep.~I~will.", 1);
                            data.AddOption("I`m~just~passing~by.");
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                    && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("My friend. Can you tell Felix that I miss him so much?");
                                data.AddOption("Yep.~I~will.", 20);
                                data.AddOption("I`m~just~passing~by.");
                                data.AddAvatar(43);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("My friend. Can you tell Felix that I miss him so much?");
                                data.AddOption("Yep.~I~will.", 30);
                                data.AddOption("I`m~just~passing~by.");
                                data.AddAvatar(43);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("My friend. Can you tell Felix that I miss him so much?");
                            data.AddOption("Yep.~I~will.", 1);//1273300
                            data.AddOption("I`m~just~passing~by.");
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("My days are numbered...will you tell him that I am really sorry, and I miss him very much? Will you?");
                        data.AddOption("OK.", 12);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("My days are numbered...will you tell him that I am really sorry, and I miss him very much? Will you?");
                        data.AddOption("OK.", 12);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 5))
                        {
                            data.AddText("I don`t even know who you are. Why do you want to kill me?");
                            data.AddOption("For~Felix!", 4);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 4))
                        {
                            data.AddText("Be careful and be quick...Step into 1 of the eyes of the Maze Puzzle in 10 seconds and I will send you out to Felix...");
                            data.AddOption("I`ll~try.", 10);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 3))
                        {
                            data.AddText("Be careful and be quick...Step into 2 of the eyes of the Maze Puzzle in 10 seconds and I will send you out to Felix...");
                            data.AddOption("I`ll~try.", 10);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 2))
                        {
                            data.AddText("Be careful and be quick...Step into 3 of the eyes of the Maze Puzzle in 10 seconds and I will send you out to Felix...");
                            data.AddOption("I`ll~try.", 10);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1))
                        {
                            data.AddText("Be careful and be quick...Step into 4 of the eyes of the Maze Puzzle in 10 seconds and I will send you out to Felix...");
                            data.AddOption("I`ll~try.", 10);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What did you come for, young hero?");
                            data.AddOption("For~your~life.", 2);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        client.CreateBoxDialog("Get ready for the Maze Puzzle.");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 5);
                        break;
                    }
                case 4:
                    {
                        data.AddText("Oh, Felix! My son! He is still alive. I miss him so much!");
                        data.AddOption("What?", 5);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("It is a long story. Many years ago, I mistakenly killed Felix`s birth parents, that is why I took to raise him up. Felix finally got to know that it was me,");
                        data.AddText("~and he will never forgive me. I know that.");
                        data.AddOption("I~see.", 6);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("My days are numbered...will you tell him that I am really sorry, and I miss him very much? Will you?");
                        data.AddOption("OK.", 11);
                        data.AddAvatar(43);
                        data.FinalizeDialog();//finish quest here!!
                        break;
                    }
                case 11:
                    {
                        if (HasItems(client, 710567, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(710567, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            client.CreateBoxDialog("You~received~100~Silver.");

                            data.AddText("My days are numbered...will you tell him that I am really sorry, and I miss him very much? Will you?");
                            data.AddOption("OK.", 12);
                            data.AddAvatar(43);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        client.Teleport(671, 782, 1015);
                        client.CreateBoxDialog("You~have~been~teleported~out~of~the~Bandit~Chamber.~Now~tell~Felix~the~whole~story.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                        client.CreateBoxDialog("Step into 5 of the eyes of the Maze Puzzle within 10 seconds and break the Maze Puzzle.");
                        break;
                    }
                case 2:
                    {
                        data.AddText("For my life? What makes you so cocky...Hey, see what, if you could step into 5 of the eyes of the Maze Puzzle within 10 seconds");
                        data.AddText("~I will give you a chance.");
                        data.AddOption("Interesting.", 3);
                        data.AddAvatar(43);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.GuideZhang)]
        public static void GuideZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1825);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuideZhang, client.Player.Class, 1826);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                            data.AddOption("Hey,~it`s~an~important~thing.", 1);
                            data.AddOption("OK.");
                            data.AddAvatar(13);
                            data.FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                      && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                            data.AddOption("OK.");
                            data.AddAvatar(13);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                            data.AddOption("OK.");
                            data.AddAvatar(13);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            if (client.Inventory.Contain(710567, 1))
                            {
                                data.AddText("What? You have the Golden Token. I will teleport you to Commander Kerry, now.");
                                data.AddOption("OK.", 2);
                                data.AddAvatar(13);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                                data.AddOption("OK.");
                                data.AddAvatar(13);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            if (client.Inventory.Contain(710567, 1))
                            {
                                data.AddText("Though you have the Golden Token, you are not a bandit. I can`t teleport you to Commander Kerry.");
                                data.AddOption("I~see.");
                                data.AddAvatar(13);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                                data.AddOption("OK.");
                                data.AddAvatar(13);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710567, 1))
                        {
                            client.Teleport(78, 57, 1818);
                            client.CreateBoxDialog("You were teleported to the Bandit Chamber.");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, Commander Kerry is busy, and you don`t have the Golden Token. You`d better leave.");
                            data.AddOption("OK.");
                            data.AddAvatar(13);
                            data.FinalizeDialog();
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Felix)]
        public static void Felix(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1819);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1820);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1821);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1822);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1823);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1824);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1825);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuideZhang, client.Player.Class, 1826);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1827);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                            data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                            data.AddOption("Sure.~I~can~do~it.", 1);
                            data.AddOption("It`s~none~of~my~business.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 4);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 5);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                   && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 8);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 9);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
             && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 12);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 13);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
         && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 16);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 17);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
   && client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 20);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }

                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 21);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
&& client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.", 24);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 25);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
&& client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 30);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
&& client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                                data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                                data.AddOption("Sure.~I~can~do~it.", 33);
                                data.AddOption("It`s~none~of~my~business.");
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.CreateBoxDialog("You~are~heading~for~Taskmaster~Chang.");
                            }
                        }
                        else
                        {
                            data.AddText("Hero, I need yor help! Years ago, my parents were killed by Commander Kerry. I have to avenge my parents,");
                            data.AddText("~but Commander Kerry is so powerful that I can`t defeat him by myself. Can you help me?");
                            data.AddOption("It`s~none~of~my~business.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 33:
                    {
                        data.AddText("I don`t know. I need some time to think about it. It just happened so suddenly that I`m totally confused now.");
                        data.AddText("~Thanks, you really helped me a lot. Now you can claim your rewards from Taskmaster Chang.");
                        data.AddOption("OK.", 32);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        data.AddText("Have I done all the things wrong? Commander Kerry really treated me well. Maybe there is no right or wrong in the world.");
                        data.AddOption("Do~you~want~to~go~back?", 31);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.Player.Money += 6000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(100);
                            client.SendSysMesage("You received 1 hour EXP and 6,000 Silver!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");

                            data.AddText("I don`t know. I need some time to think about it. It just happened so suddenly that I`m totally confused now.");
                            data.AddText("~Thanks, you really helped me a lot. Now you can claim your rewards from Taskmaster Chang.");
                            data.AddOption("OK.", 32);
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 32:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        client.CreateBoxDialog("You~are~heading~for~Taskmaster~Chang.");
                        break;
                    }
                case 25:
                    {
                        data.AddText("Great! You`ve collected all 5 tokens! Now I will compose them into 1 token. It is called the Token. All the Bandits will respect you once them see the token.");
                        data.AddText("~I will dress you just like a Bandit. Then you can meet with Guide Zhang, and he can teleport you to the Bandit Chamber. But you will");
                        data.AddText("~turn back after 2 minutes.");
                        data.AddOption("I~see.", 26);
                        data.AddOption("I~lost~the~token.", 27);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Contain(710567, 1))
                        {
                            data.AddText("Sorry, but you`ve had the token in your inventory.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The token is very important. You should take good care of it.");
                            data.AddOption("Sorry.", 28);
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710567);
                            client.CreateBoxDialog("You received a Golden Token!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Your inventory is full. You`ll need to make some room, first.");
                        }
                        break;
                    }
                case 26://trans
                    {
                        if (client.Player.OnTransform)
                        {
                            data.AddText("Guide Zhang (Bird Island, 636, 656) can teleport you to Commander Kerry in the Bandit Chamber.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                            client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, 201, (int)90, 8213);
                            data.AddText("Now you look just like a bandit. You can find Guide Zhang (Bird Island, 636, 656) to teleport you to the Bandit Chamber. But the disguise will only last for 2 minutes.");
                            data.AddOption("OK.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 24:
                    {

                        data.AddText("I will dress you just like a bandit. Then you can meet with Guide Zhang, and he can teleport you to the Bandit Chamber. But you will");
                        data.AddText("~turn back after 90 seonds.");
                        data.AddOption("I~see.", 23);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Contain(710567, 1))
                            {
                                client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                                client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, 201, (int)90, 8213);
                                client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                                client.CreateBoxDialog("You~received~a~Golden~Token~and~were~disguised~as~a~bandit,~and~it~will~only~last~for~2~minutes.");
                                client.Inventory.Add(stream, 710567);
                            }
                            else
                            {
                                client.CreateBoxDialog("You~received~a~Golden~Token~and~were~disguised~as~a~bandit,~and~it~will~only~last~for~2~minutes.");
                            }
                        }
                        else
                        {
                            data.AddText("You inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(710566, 1))
                        {
                            data.AddText("Great! You`ve collected all 5 tokens! Now I will compose them into 1 Golden Token. All the Bandits will respect you once them see the token.");
                            data.AddOption("OK.", 22);//1273000
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("If you can kill the Senior Bandit, you will have the chance to get the Scarlet Token.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain(710566, 1))
                        {
                            if (HasItems(client, 710566, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                            {
                                if (!client.Inventory.Remove(710566, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                data.AddText("I will dress you just like a bandit. Then you can meet with Guide Zhang, and he can teleport you to the Bandit Chamber. But you will");
                                data.AddText("~turn back after 90 seonds.");
                                data.AddOption("I~see.", 23);
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("If you can kill the Senior Bandit, you will have the chance to get the Scarlet Token.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("The Senior Bandits (Bird Island, 570, 416) has the Scarlet Token. He is much stronger than all the former Bandits, so you`ll need to pay more attention.");
                        data.AddText("~You`ll have the chance to get the token by killing him.");
                        data.AddOption("I~see.", 19);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Contain(710565, 1))
                        {
                            data.AddText("You are much stronger than I imagined. I think my revenge will succeed, soon! There is still one thing we need to find Commander Kerry.");
                            data.AddOption("What~is~it?", 18);//1272960
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The Bandit Deliverers appear around Bird Island (431, 453). Only killing 10 of them will the Pitch Token appears.");
                            data.AddOption("OK.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Contain(710565, 1))
                        {
                            if (HasItems(client, 710565, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                if (!client.Inventory.Remove(710565, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                data.AddText("The Senior Bandits (Bird Island, 570, 416) has the Scarlet Token. He is much stronger than all the former Bandits, so you`ll need to pay more attention.");
                                data.AddText("~You`ll have the chance to get the token by killing him.");
                                data.AddOption("I~see.", 19);
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Bandit Deliverers appear around Bird Island (431, 453). Only killing 10 of them will the Pitch Token appears.");
                            data.AddOption("OK.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 19:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.CreateBoxDialog("Kill~the~Senior~Bandits~(Bird~Island,~570~416).");
                        break;
                    }
                case 16:
                    {
                        data.AddText("Actually I don`t want you to fight against the Bandit Deliverers, but we have to get the Pitch Token. You`ll need to kill 10");
                        data.AddText("~Bandit Deliverers on Bird Island (525,~345) to get the Pitch Token.");
                        data.AddOption("I~see.", 15);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(710564, 1))
                        {
                            data.AddText("Great! You`ve got the Black Token. Now, caccording to the information on it, the Pitch Token is held by the Bandit Deliverers.");
                            data.AddOption("What~should~I~do?", 14);//1272920
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I think only 10 Bandits and 1 Bandit Courier around Bird Island (316, 567) won`t be a trouble to you!");
                            data.AddText("~Kill them, and then you`ll get the Black Token.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.CreateBoxDialog("Kill~10~Bandit~Deliverers~on~Bird~Island~(525,~345).");
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Contain(710564, 1))
                        {
                            if (HasItems(client, 710564, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                if (!client.Inventory.Remove(710564, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");

                                data.AddText("Actually I don`t want you to fight against the Bandit Deliverers, but we have to get the Pitch Token. You`ll need to kill 10");
                                data.AddText("~Bandit Deliverers on Bird Island (525,~345) to get the Pitch Token.");
                                data.AddOption("I~see.", 15);
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I think only 10 Bandits and 1 Bandit Courier around Bird Island (316, 567) won`t be a trouble to you!");
                            data.AddText("~Kill them, and then you`ll get the Black Token.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        data.AddText("The Indigo Token shows that a Bandit Courier has the Black Token, who is hiding among 10 Bandits. If you kill all the Bandits,");
                        data.AddText("~the Bandit Couriers will appear.");
                        data.AddOption("I~see.", 11);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(710563, 1))
                        {
                            data.AddText("Great! You`ve found the Indigo Token! I must check it carefully. Thanks a lot.");
                            data.AddOption("You`re~welcome.", 10);//1272880
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Well, be patient! You will find the Indigo Token if you kill enough Bandits L98 (around Bird Island 455, 466).");
                            data.AddOption("Alright.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(710563, 1))
                        {
                            if (HasItems(client, 710563, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                if (!client.Inventory.Remove(710563, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");

                                data.AddText("The Indigo Token shows that a Bandit Courier has the Black Token, who is hiding among 10 Bandits. If you kill all the Bandits,");
                                data.AddText("~the Bandit Couriers will appear.");
                                data.AddOption("I~see.", 11);
                                data.AddAvatar(117); data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Well, be patient! You will find the Indigo Token if you kill enough Bandits L98 (around Bird Island 455, 466).");
                            data.AddOption("Alright.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.CreateBoxDialog("Kill~the~Bandits~around~Bird~Island~(530,349)~and~find~the~Bandit~Courier.");
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710562, 1) && client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                        {
                            data.AddText("Good! It is the Red Token. It shows that the Indigo Token is kept by a Bandit L98 .");
                            data.AddOption("How~to~find~it?", 6);//1272840
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The Red Token is held by the Bandit Leader L97  (around Bird Island 486, 481). Only if you kill 50 Bandits L97 will He appears.");
                            data.AddText("~Without this token, we won`t be able to find the other tokens.");
                            data.AddOption("I~see.");
                            data.AddAvatar(117);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(710562, 1) && client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                        {
                            if (HasItems(client, 710562, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(710562, 1, stream)) break;
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                client.CreateBoxDialog("You~received~100~Silver.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                data.AddText("The Red Token shows the Indigo Token is kept by a Bandit L98, but I don`t know which one he is exactly.");
                                data.AddText("~You`ll need to keep killing the Bandits L98 until you find the token.");
                                data.AddOption("I~see.", 7);//1272855
                                data.AddAvatar(117);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("The Red Token shows the Indigo Token is kept by a Bandit L98, but I don`t know which one he is exactly.");
                        data.AddText("~You`ll need to keep killing the Bandits L98 until you find the token.");
                        data.AddOption("I~see.", 7);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Kill~Bandits~L98~(around~Bird~Island~257,~162).");
                        break;
                    }
                case 1:
                    {
                        data.AddText("Your are much stronger than me. I think it will be no problem for you to defeat Commander Kerry, but it`s difficult to get to him.");
                        data.AddText("~You must get 4 tokens from the Bandits around Bird Island, first.");
                        data.AddOption("I`ll~help~you.", 2);
                        data.AddOption("Too~dangerous~for~me.");
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You completed the quest and received 100 Silver!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");

                            data.AddText("Great! Now let`s start from the Red Token. You`ll need to kill 50 Bandits L97 (around Bird Island 262,117) until the Bandit Leader L97 shows up.");
                            data.AddText("~When he has been killed, you will obtain a Red Token. Only with it, I can find out where is the next token.");
                            data.AddOption("I~see.", 3);
                            data.AddAvatar(117);
                            data.FinalizeDialog();

                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("Great! Now let`s start from the Red Token. You`ll need to kill 50 Bandits L97 (around Bird Island 262,117) until the Bandit Leader L97 shows up.");
                        data.AddText("~When he has been killed, you will obtain a Red Token. Only with it, I can find out where is the next token.");
                        data.AddOption("I~see.", 3);
                        data.AddAvatar(117);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Kill~Bandits~L97~(around~Bird~Island~262,117).");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.CommanderAid4)]
        public static void CommanderAid4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 1:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 2:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 3:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 4:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 5:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 6:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 7:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CommanderAid3)]
        public static void CommanderAid3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 1:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 2:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 3:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 4:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 5:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 6:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 7:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CommanderAid2)]
        public static void CommanderAid2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 1:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 2:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 3:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 4:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 5:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 6:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 7:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CommanderAid1)]
        public static void CommanderAid1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        switch (Program.GetRandom.Next(0, 8))
                        {
                            case 0:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 1:
                                {
                                    data.AddText("Long live Commander Kerry!");
                                    data.AddOption("Whatever.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 2:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 3:
                                {
                                    data.AddText("I have a rendezvous with death.");
                                    data.AddOption("Neat!");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 4:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 5:
                                {
                                    data.AddText("I will do anything for Commander Kerry.");
                                    data.AddOption("As~you~wish.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 6:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.FinalizeDialog();
                                    break;
                                }
                            case 7:
                                {
                                    data.AddText("Hey, you look just like me....why don`t you join us instead?");
                                    data.AddOption("Like~you?~No~way.");
                                    data.AddAvatar(100);
                                    data.FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
