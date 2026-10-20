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
        [NpcAttribute(NpcID.Matchmaker)]
        public static void Matchmaker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddOption("I~shall~try~my~best!", 1);
                        data.AddAvatar(2);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddOption("OK.~I~shall~visit~her.", 2);
                        data.AddOption("Just~passing~by.");
                        data.AddAvatar(4);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("You~must~have~a~meteor~tear~and~a~meteor,~then~you~can~divorce.");
                        data.AddOption("Here~you~are.", 3);
                        data.AddOption("I~changed~my~mind.");
                        data.AddAvatar(6);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        /*data.AddText("Some~lovers~love~each~other~but~cannot~live~together.~Are~you~sure~to~divorce~from~your~spouse~whom~you~once~loved~so~much.");
                        data.AddOption("I~decide~to~divorce.", 4);
                        data.AddOption("I~changed~my~mind.");
                        data.AddAvatar(6);
                        data.FinalizeDialog();
                        */

                        data.AddOption("Sorry,~I~cannot~tell~you.");
                        data.AddAvatar(2);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Joe!~You~have~seen~him?~Where~is~he~now?~Is~everything~going~on~well~with~him?");
                        data.AddOption("Do~not~cry.", 5);
                        data.AddOption("Do~not~ruin~the~meteor!");
                        data.AddAvatar(2);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureBox)]
        public static void TreasureBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The treasure box is empty. The bandits just took the treasure and left.");
                        data.AddOption("I~got~a~little~late,~still.");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureBox1)]
        public static void TreasureBox1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The treasure box is empty. The bandits just took the treasure and left.");
                        data.AddOption("I~got~a~little~late,~still.");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureBox2)]
        public static void TreasureBox2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The treasure box is empty. The bandits just took the treasure and left.");
                        data.AddOption("I~got~a~little~late,~still.");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureBox3)]
        public static void TreasureBox3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The treasure box is empty. The bandits just took the treasure and left.");
                        data.AddOption("I~got~a~little~late,~still.");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.YangYun2)]
        public static void YangYun2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 520);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 521);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Well, here we are! This is the Treasure House of our Yang family.");
                            data.AddOption("Show~me~the~Tower~Splinter.", 1);
                            data.AddOption("It`s~a~nice~place.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("I really want to help you and the Tang family, but the Tower Splinter I kept has been stolen.");
                                data.AddOption("What~should~I~do~next?", 6);
                                data.AddOption("Alright.");
                            }
                            else
                            {
                                data.AddText("I really want to help you and the Tang family, but the Tower Splinter I kept has been stolen.");
                                data.AddOption("What~should~I~do~next?", 6);
                                data.AddOption("Alright.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Years ago, the defensive talisman, Star Tower, was broken into three pieces. For that, our Yang family and the Tang family became enemies. I just");
                            data.AddText("~happened to have one of the Tower Splinters. Maybe it`s the key to the ending the feud between the two clans.");
                            data.AddOption("Could~be.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 6:
                    {
                        data.AddText("I`m afraid you should go find the Tower Splinters around. If you have any problem, just find me on Bird Island. Good luck!");
                        data.AddOption("Bye!", 3);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Map.View.GetAllMapRolesCount(Role.MapObjectType.Monster, p => p.Alive == true) == 0)
                        {
                            data.AddText("I can`t believe it! They`ve stolen the Tower piece!! Please, help me get it back!");
                            data.AddOption("The~thieves~will~pay~dearly.", 2);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Geez! Where had all these bandits come from? Hero, I need you to teach them a lesson! They should stay away from my collections and the Tower Splinter.");
                            data.AddOption("I`ll~take~care~of~it.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("I`m afraid you should go find the Tower Splinters around. If you have any problem, just find me on Bird Island. Good luck!");
                            data.AddOption("Bye!", 3);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(733, 518, 1015);
                        client.CreateBoxDialog("You~are~on~Bird~Island,~now.");
                        break;
                    }
            }
        }

        //
        [NpcAttribute(NpcID.NetPeddler)]
        public static void NetPeddler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The Ruan Brothers` hut is just under those stone doors. They`re crazy rich, but I`m not so jealous.");
                            data.AddText("~I don`t care if I`m poor. Selling fishing supplies is a decent trade, and safer, too!");
                            data.AddOption("Do~you~sell~Fishing~Nets?", 1);
                            data.AddOption("As~long~as~you`re~happy.");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Do you know, that Ruan Brothers have a hut under those stone doors. They`re crazy rich, but I`m not so jealous.");
                            data.AddText("~I don`t care if I`m poor. Selling fishing supplies is a decent trade, and safer, too!");
                            data.AddOption("As~long~as~you`re~happy.");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Of course! You don`t need to be a genius to fish, but you will need a good net. I might have a spare around here, somewhere.");
                        data.AddOption("One~Fishing~Net,~please.", 2);
                        data.AddOption("I`ll~be~back~soon.");
                        data.AddAvatar(118);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("Make sure you bring enough money with you! It`s just 10,000 Silver!");
                            data.AddOption("Alright.");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.Money -= 10000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Add(stream, 721779, 1);
                                client.CreateBoxDialog("You~bought~a~Fishing~Net~with~10,000~Silver.~Hurry~and~use~it~around~the~water~(303,380).");

                            }
                            else
                            {
                                data.AddText("Sorry. Your inventory is full. You need to make some room.");
                                data.AddOption("Oh.~No~problem.");
                                data.AddAvatar(118);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YeSheng)]
        public static void YeSheng(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("There are quite a number of mysteries, down here. You can tell a lot from this single Iron Poise.");
                            data.AddOption("What`s~the~price?", 1);

                            data.AddOption("That~is~fake,~maybe.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There are quite a number of mysteries, down here. You can tell a lot from this single Iron Poise.");
                            data.AddOption("That~is~fake,~maybe.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Come on, now! I`m selling this Iron Poise at a cut-throat price! Three fish for one! Hey, you know how to");
                        data.AddText("~get fish? Go buy a Fishing Net from the Net Peddler (321,371) and catch fish near the Island. Good luck!");
                        data.AddOption("I`ve~got~your~fish.", 2);
                        data.AddOption("I~hope,~too.");
                        data.AddAvatar(77);
                        data.FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(721778, 3))
                        {
                            if (client.Inventory.Remove(721778, 3, stream))
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    client.Inventory.Add(stream, 721780, 1);
                                    client.SendSysMesage("You received the Iron Poise!");
                                    data.AddText("Wow, the fish you brought is wonderful! Take this Iron Poise! You can use it to remove the spell at the stone doors (344,403),");
                                    data.AddText("~then the path to the underground chamber will be open ahead of you.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(77);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Your inventory is full! Get rid of some stuff and you can come back for the Iron Poise.");
                                    data.AddOption("I`ll~be~back~soon.");
                                    data.AddAvatar(77);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Are you kidding? You don`t have any fish with you! Go buy a Fishing Net from the Net Peddler (189,183)");
                            data.AddText("~and catch fish near the water. Remember, I want THREE fish for one Iron Poise!");
                            data.AddOption("Got~it.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        //
        [NpcAttribute(NpcID.Plum)]
        public static void Plum(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.RuanBrother, client.Player.Class, 524);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.Plum, client.Player.Class, 525);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Days before, a group of bandits in the Desert were killed, overnight. Nobody knows who did it or for what. So, what are you doing here?");
                            data.AddOption("Do~you~know~the~Star~Tower?", 1);
                            data.AddOption("I`m~just~passing~by.");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Take the Tower piece and leave. Anything can happen in these troubled times.");
                                data.AddOption("You~bet.");
                                data.AddAvatar(118);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Actually, the Star Tower has been broken into three pieces. It`s said one of the pieces was brought to the Desert. Just days before, a group of bandits");
                                data.AddText("~in Desert were killed overnight, there.");
                                data.AddOption("Where~can~I~find~it?", 5);
                                data.AddOption("I`d~better~return~home.");
                                data.AddAvatar(118);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("False friends are worse than bitter enemies.");
                            data.AddOption("You~think?");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("As I know, the third Tower Splinter is in the deep of Desert. Go to (166,226) and see what you can find out there. Be careful, though. The long-standing war has");
                        data.AddText("~ruined the landscape, leaving only homeless villagers and savage bandits in its wake...");
                        data.AddOption("I`ll~be~careful.", 3);
                        data.AddAvatar(118);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Star Tower? Yeah, but I should tell you the road ahead will be long and dangerous. Are you sure you want to continue?");
                        data.AddOption("I~can`t~be~sure~more.", 2);
                        data.AddAvatar(118);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("As I know, the third Tower Splinter is in the deep of Desert. Go to (166,226) and see what you can find out there. Be careful, though. The long-standing war has");
                            data.AddText("~ruined the landscape, leaving only homeless villagers and savage bandits in its wake...");
                            data.AddOption("I`ll~be~careful.", 3);
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Head~to~the~deep~of~Desert~at~(166,226)~to~start~an~investigation.");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RuanBrother)]
        public static void RuanBrother(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.TimeDoor, client.Player.Class, 523);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.RuanBrother, client.Player.Class, 524);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("Hey, you are still too green! Keep leveling up and come back when you reach Level 100.");
                            data.AddOption("Alright.");
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Hey, who are you?! How can you arrive here?");
                                data.AddOption("I~came~for~the~Tower~Splinter.", 1);
                                data.AddOption("Sorry,~I~got~lost.");
                                data.AddAvatar(18);
                                data.FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    data.AddText("What are you doing here? You`d better have something important to bother me.");
                                    data.AddOption("I~lost~the~Tower~Splinter.", 9);
                                    data.AddOption("Sorry,~bye!");
                                    data.AddAvatar(18);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Don`t you see I`m busy, now? You`d better have something important to bother me.");
                                    data.AddOption("I~brought~the~Shawn`s~Spear.", 5);
                                    data.AddOption("Sorry,~bye!");
                                    data.AddAvatar(18);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("What are you doing here?");
                                data.AddOption("I`m~just~passing~by.");
                                data.AddAvatar(18);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(721777, 1))
                        {
                            data.AddText("Show some manners! You`ve already received the piece.");
                            data.AddOption("My~apologies.");
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I-I-I-I! Where did you lose the Tower Splinter? Do you know how important that is? Fortunately, the one I gave you is a fake one. I don`t mean to");
                            data.AddText("~cheat you. See, this is the real one. I promise this one is true.");
                            data.AddOption("I~hope~you~said~the~true~word.", 3);
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 5:
                    {
                        data.AddText("Yeah, this is that spear I want. I`m old. You`ve already made the revenge for me. So I don`t need this Tower Splinter");
                        data.AddText("~to remind me of the past. Come, take it.");
                        data.AddOption("Great.", 3);
                        data.AddAvatar(18);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        data.AddText("Tower Splinter? Did Frost Tang send you here? She is my diva, the apple of my eye, my precious little sweety...But all this was shattered by Bloody");
                        data.AddText("~Shawn. He always tried to keep us apart. If you could snatch away his spear and give it to me, I can give you the piece I have.");
                        data.AddOption("I`ve~got~it~right~here.", 2);
                        data.AddOption("I`ll~think~it~over.");
                        data.AddAvatar(18);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(721781, 1))
                        {
                            if (!client.Inventory.Remove(721781, 1, stream)) break;
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 100 Silver!");
                                data.AddText("Yeah, this is that spear I want. I`m old. You`ve already made the revenge for me. So I don`t need this Tower Splinter");
                                data.AddText("~to remind me of the past. Come, take it.");
                                data.AddOption("Great.", 3);
                                data.AddAvatar(18);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Come on, I mean the spear that Bloody Shawn holds, not any others. Go take his spear, and I`ll consider to show you the splinter.");
                            data.AddText("~Oh, wait! Did I mention where he is? Sorry! You can probably find him in the northwest of Bird Island. Good luck!");
                            data.AddOption("Got~it.");
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721777, 1);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.CreateBoxDialog("You~received~the~Tower~Splinter!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. Why not tidy it up, first?");
                            data.AddOption("I`ll~be~back~soon.");
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RuanGoodMan)]
        public static void RuanGoodMan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My brother is STILL single, you know. He`s already in his forties! Nobody knows why. It`s weird.");
                        data.AddOption("Go~back~to~Bird~Island.", 1);
                        data.AddAvatar(48);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(723, 573, 1015);
                        client.CreateBoxDialog("You~returned~to~Bird~Island.");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.RuanBetterMan)]
        public static void RuanBetterMan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Alas... My brother had a duel with the Bloody Shawn days before, and lost again. Yeah, I mean that ugly beast in the northwest of Bird Island.");
                        data.AddOption("Weak~men~will~always~lose.");
                        data.AddAvatar(25);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DreamDoor)]
        public static void DreamDoor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take these stone doors seriously. They are cursed and can only be opened with the Iron Poise.");
                        data.AddOption("Okay.", 1);
                        data.AddOption("I`ll~pass.");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721780, 1))
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3) == false)
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                    client.SendSysMesage("The stone door has opened!");
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        if (!client.Inventory.Remove(721780, 1, stream)) break;
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        if (!client.Inventory.Remove(721780, 1, stream)) break;
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Take these stone doors seriously. They are cursed, and can only be opened with the Iron Poise owned by Ye Sheng (192,212) on Bird Island.");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Don`t be mesmerized by the mystic radiance around here. Be quick. The door has opened.");
                            data.AddOption("Indeed.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SeasonDoor)]
        public static void SeasonDoor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("These stone doors are all glowing with a strange aura. You can only break them with the Iron Poise.");
                        data.AddOption("Okay.", 1);
                        data.AddOption("I`ll~pass.");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721780, 1))
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 2) == false)
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                    client.SendSysMesage("The stone door has opened!");
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        if (!client.Inventory.Remove(721780, 1, stream)) break;
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        if (!client.Inventory.Remove(721780, 1, stream)) break;
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Take these stone doors seriously. They are cursed, and can only be opened with the Iron Poise owned by Ye Sheng (192,212) on Bird Island.");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Don`t be mesmerized by the mystic radiance around here. Be quick. The door has opened.");
                            data.AddOption("Indeed.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.TimeDoor)]
        public static void TimeDoor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("These stone doors are cursed. They can only be opened with the Iron Poise, owned by Ye Sheng (192,212) on Bird Island.");
                        data.AddOption("I`ll~use~it~now.", 1);
                        data.AddOption("Alright.");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721780, 1))
                            {

                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                    client.SendSysMesage("The stone door has opened!");
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        if (!client.Inventory.Remove(721780, 1, stream)) break;
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        if (!client.Inventory.Remove(721780, 1, stream)) break;
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Take these stone doors seriously. They are cursed, and can only be opened with the Iron Poise owned by Ye Sheng (192,212) on Bird Island.");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Don`t be mesmerized by the mystic radiance around here. Be quick. The door has opened.");
                            data.AddOption("Indeed.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YangYun1)]
        public static void YangYun1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 520);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 521);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.Plum, client.Player.Class, 525);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.YangYun1, client.Player.Class, 526);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("Hey, you are still too green! Keep leveling up and come back when you reach Level 100.");
                            data.AddOption("Alright.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("I`ve rechecked the Treasure House, but found nothing useful. How about you?");
                                data.AddOption("Something~strange~happened.", 20);
                                data.AddOption("No~good~news.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Years ago, the defensive talisman, Star Tower, was broken into three pieces. For that, our Yang family and the Tang family became enemies.");
                                data.AddText("~If I can find all the Tower Splinters, maybe I can end the feud between the two clans. What`s your opinion?");
                                data.AddOption("I~heard~you~have~one~piece.", 1);
                                data.AddOption("Could~be.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    data.AddText("How is my Treasure House? So luxurious, right? It used to have countless treasure and collections, including the Tower Splinter.");
                                    data.AddOption("Where~is~the~Tower~Splinter?", 10);
                                    data.AddOption("Not~bad.");
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                                else
                                {

                                    data.AddText("Years ago, the defensive talisman, Star Tower, was broken into three pieces. For that, our Yang family and the Tang family became enemies.");
                                    data.AddText("~If I can find all the Tower Splinters, maybe I can end the feud between the two clans. What`s your opinion?");
                                    data.AddOption("I~heard~you~have~one~piece.", 4);
                                    data.AddOption("Could~be.");
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {

                                data.AddText("Hello, buddy! Did you find my Tower Splinter? No? So, what can I do for you?");
                                data.AddOption("Where~are~other~splinters?", 13);
                                data.AddOption("I`m~just~passing~by.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();

                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    data.AddText("I`ve shown the handkerchief to my brothers. Actually, that belongs to our Yang family. I don`t know how to explain it to you.");
                                    data.AddOption("Just~tell~me~all.", 23);
                                    data.AddOption("Then~forget~it.~Bye!");
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("I`ve shown the handkerchief to my brothers. Actually, that belongs to our Yang family. I don`t know how to explain it to you.");
                                    data.AddOption("Just~tell~me~all.", 23);
                                    data.AddOption("Then~forget~it.~Bye!");
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Years ago, the defensive talisman, Star Tower, was broken into three pieces. For that, our Yang family and the Tang family became enemies. I just");
                                data.AddText("~happened to have one of the Tower Splinters. Maybe it`s the key to the ending the feud between the two clans.");
                                data.AddOption("Could~be.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }

                case 23:
                    {
                        data.AddText("But where did you get the Bud Handkerchief? You see the character `Yang` embroidered on it? This belongs to Yang Feng,");
                        data.AddText("~the old head of the Yang family. I`d be willing to take you there, but I`m keeping the handkerchief. Sound good?");
                        data.AddOption("Alright,~let`s~go.", 22);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(721782, 1))
                        {
                            data.AddText("What? You have one piece of the Star Tower, but it disappeared and became a handkerchief? It`s incredible! Could you show me that Bud Handkerchief?");
                            data.AddOption("Okay.~Here~you~are.", 21);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I have ever seen the third Tower Splinter in the deep of Desert. Why not return the place where the Desert Condors live? Maybe you can");
                            data.AddText("~find something special that you didn`t noticed last time.");
                            data.AddOption("Alright.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(721782, 1))
                        {
                            if (!client.Inventory.Remove(721782, 1, stream)) break;
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 100 Silver!");
                                data.AddText("But where did you get the Bud Handkerchief? You see the character `Yang` embroidered on it? This belongs to Yang Feng,");
                                data.AddText("~the old head of the Yang family. I`d be willing to take you there, but I`m keeping the handkerchief. Sound good?");
                                data.AddOption("Alright,~let`s~go.", 22);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I have ever seen the third Tower Splinter in the deep of Desert. Why not return the place where the Desert Condors live? Maybe you can");
                            data.AddText("~find something special that you didn`t noticed last time.");
                            data.AddOption("Alright.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 22:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Teleport(52, 50, 1783);
                        client.CreateBoxDialog("You~arrived~at~the~Treasure~House,~once~again.~Be~careful!~You~sense~trouble~in~the~air.");
                        break;
                    }
                case 13:
                    {
                        data.AddText("I`m not sure, but someone told me that the second piece is in the Hut in the north of Bird Island. That Hut belongs to the Ruan Brothers.");
                        data.AddText("~Watch out for them. They`re a little off.");
                        data.AddOption("Got~it.", 12);
                        data.AddOption("Maybe~I~should~give~up.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("You knew that, didn`t you? The Tower Splinter was stolen by those bandits! And you`ve promised to investigate that for me, right?");
                        data.AddOption("Sure.~I`ll.", 11);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("I`m not sure, but someone told me that the second piece is in the Hut in the north of Bird Island. That Hut belongs to the Ruan Brothers.");
                            data.AddText("~Watch out for them. They`re a little off.");
                            data.AddOption("Got~it.", 12);
                            data.AddOption("Maybe~I~should~give~up.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("You~were~advised~to~find~the~Ruan~Brother~in~the~Hut,~in~the~north~of~Bird~Island.");
                        break;
                    }
                case 1:
                    {
                        data.AddText("Um... Yeah, I have one of the pieces. How did you know? You must have pulled quite a few strings to hear");
                        data.AddText("~about that. Well, I guess I`d better tell you where the piece is, before you wreck the place. It`s in the Treasure House.");
                        data.AddOption("I`m~doing~this~for~Sugar~Tang.", 2);
                        data.AddOption("I`m~just~passing~by.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Sugar Tang? So you are trying to end the feud, right? In that case, I`ll take you to the House myself! Thank you for your help.");
                        data.AddOption("Just~doing~my~job.", 3);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Teleport(52, 50, 1782);
                        client.CreateBoxDialog("You~arrived~at~the~Treasure~House,~but~only~found~it`s~been~ransacked~by~bandits.~Talk~about~that~with~Yang~Yun~soon.");
                        break;
                    }
                case 4:
                    {
                        data.AddText("Um... Yeah, I have one of the pieces. How did you know? You must have pulled quite a few strings to hear");
                        data.AddText("~about that. Well, I guess I`d better tell you where the piece is, before you wreck the place. It`s in the Treasure House.");
                        data.AddOption("I`m~doing~this~for~Sugar~Tang.", 5);
                        data.AddOption("I`m~just~passing~by.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Sugar Tang? So you are trying to end the feud, right? In that case, I`ll take you to the House myself! Thank you for your help.");
                        data.AddOption("Just~doing~my~job.", 6);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Teleport(52, 50, 1782);
                        client.CreateBoxDialog("You~arrived~at~the~Treasure~House,~but~only~found~it`s~been~ransacked~by~bandits.~Talk~about~that~with~Yang~Yun~soon.");

                        break;
                    }
            }
        }



        [NpcAttribute(NpcID.GhoulKong)]
        public static void GhoulKong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.YangYun1, client.Player.Class, 526);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.GhoulKong, client.Player.Class, 527);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("If only I were 10 years younger...");
                            data.AddOption("I~have~some~questions.", 1);
                            data.AddOption("In~your~dreams.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {

                            data.AddText("If only I were 10 years younger...");
                            data.AddOption("I~want~to~ask~you~something.", 6);
                            data.AddOption("In~your~dreams.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("If only I were 10 years younger...");
                            data.AddOption("In~your~dreams.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        data.AddText("Well, according to your map, you are gonna meet quite a number of obstacles. A good beginning is half done. I shall");
                        data.AddText("~escort you there. Anyway, that`s all an old man can do...");
                        data.AddOption("Thank~you.", 5);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("You found the right person. I`m just the most knowledgeable man, but the dungeon is very complicated. Without a map, no one knows where to get");
                        data.AddText("~in or how to get out. So, what is your question?");
                        data.AddOption("I~got~a~Weird~Invocation.", 3);
                        data.AddOption("How~to~smelt~the~Star~Tower?", 2);
                        data.AddOption("Eh...~I~forgot.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("The Star Tower was left by Frost Tang. It is said that it takes 4 clinkers, smelt from 4 weapons. They are the Glitter Sword,");
                        data.AddText("~Annatto Blade, Wasp Spear, and Infernal Axe. With those and the three Tower pieces together, you can restore it in the Cauldro");
                        data.AddOption("There~is~another~thing.", 1);
                        data.AddOption("Unbelievable.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }

                case 3:
                    {
                        if (client.Inventory.Contain(721786, 1))
                        {
                            data.AddText("Oh my! This isn`t an invocation! It`s the map to the Dungeon! The Dungeon, I`m telling you, is chock full of treasure...");
                            data.AddText("~But it`s also full of hidden dangers and fierce monsters...");
                            data.AddOption("No~pain,~no~gain.", 4);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Maybe Yang Yun has the map to Dungeon. What can an old man like me do? I am useless...");
                            data.AddOption("I`m~glad~you~said~it.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            // client.Inventory.Remove(721786, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Well, according to your map, you are gonna meet quite a number of obstacles. A good beginning is half done. I shall");
                            data.AddText("~escort you there. Anyway, that`s all an old man can do...");
                            data.AddOption("Thank~you.", 5);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(31, 89, 1785);
                        client.CreateBoxDialog("The~path~has~lead~you~to~Dungeon~1F.");
                        break;
                    }
            }

        }

        [NpcAttribute(NpcID.Cauldron)]
        public static void Cauldron(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.GhoulKong, client.Player.Class, 527);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.GhoulKong, client.Player.Class, 528);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Everyone wants to be immortal, but what is the meaning, if you are alone in the world. What do you say?");
                            data.AddOption("I~want~to~smelt~some~items.", 1);
                            data.AddOption("You`re~right.");
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Everyone wants to be immortal, but what is the meaning, if you are alone in the world. What do you say?");
                            data.AddOption("How~is~the~smelting~going?", 12);
                            data.AddOption("Indeed.");
                            data.FinalizeDialog();

                        }
                        else
                        {
                            if (client.Inventory.Contain(721785, 1))
                            {
                                data.AddText("You know you`ve done this before, right? You can only do it once. Happiness lies in contentment, you know.");
                                data.AddOption("Pity.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Love built on beauty fades all too quickly. Pity, that Frost Tang was never happy... To restore the Tower, you need to collect");
                                data.AddText("~Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe, smelt them and meld 4 clinkers with the 3 pieces in the Cauldron. So, how may I help you?");
                                data.AddOption("Smelt~weapons~into~clinkers.", 10);
                                data.AddOption("Restore~the~Star~Tower.", 4);
                                data.AddOption("I`ll~come~back~later.");
                                data.AddOption("Bird~Island,~please.", 2);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        data.AddText("Now that the Star Tower has been restored, you should leave this dangerous place right away. Hurry up and return to Bird Island for Sugar Tang.");
                        data.AddOption("Thanks,~bye!", 7);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721785, 1))
                        {
                            data.AddText("You know you`ve done this before, right? You can only do it once. Happiness lies in contentment, you know.");
                            data.AddOption("Pity.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Love built on beauty fades all too quickly. Pity, that Frost Tang was never happy... To restore the Tower, you need to collect");
                            data.AddText("~Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe, smelt them and meld 4 clinkers with the 3 pieces in the Cauldron. So, how may I help you?");
                            data.AddOption("Smelt~weapons~into~clinkers.", 10);
                            data.AddOption("Restore~the~Star~Tower.", 4);
                            data.AddOption("I`ll~come~back~later.");
                            data.AddOption("Bird~Island,~please.", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(721784, 1))
                        {
                            if (client.Inventory.Contain(721788, 1))
                            {
                                if (client.Inventory.Contain(721789, 1))
                                {
                                    if (client.Inventory.Contain(721790, 1))
                                    {
                                        if (!client.Inventory.Remove(721784, 1, stream)) break;
                                        if (!client.Inventory.Remove(721788, 1, stream)) break;
                                        if (!client.Inventory.Remove(721789, 1, stream)) break;
                                        if (!client.Inventory.Remove(721790, 1, stream)) break;

                                        client.Inventory.Add(stream, 721791, 1);
                                        client.Inventory.Add(stream, 721792, 1);
                                        client.Inventory.Add(stream, 721793, 1);
                                        client.Inventory.Add(stream, 721794, 1);
                                        data.AddText("You`ve got the Fire Clinker, Earth Clinker, Earth Clinker, and Wood Clinker.");
                                        data.AddOption("Great!");
                                    }
                                    else
                                    {
                                        data.AddText("Beat the generals and bring the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe here.");
                                        data.AddOption("It~will~be~done.");
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Beat the generals and bring the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe here.");
                                    data.AddOption("It~will~be~done.");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Beat the generals and bring the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe here.");
                                data.AddOption("It~will~be~done.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Beat the generals and bring the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe here.");
                            data.AddOption("It~will~be~done.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(721785, 1))
                        {
                            data.AddText("You know you`ve done this before, right? You can only do it once. Happiness lies in contentment, you know.");
                            data.AddOption("Pity.");
                            data.FinalizeDialog();
                        }
                        else
                        {

                            if (client.Inventory.Contain(721791, 1))
                            {
                                if (client.Inventory.Contain(721792, 1))
                                {
                                    if (client.Inventory.Contain(721793, 1))
                                    {
                                        if (client.Inventory.Contain(721794, 1))
                                        {
                                            if (client.Inventory.Contain(721777, 3))
                                            {
                                                data.AddText("It takes a combination of the five elements of fire, earth, water, metal, and wood to restore the talisman.");
                                                data.AddText("~Wait a moment. The Star Tower is almost completely restored.");
                                                data.AddOption("Great!", 6);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                data.AddText("You will have all three Tower Splinters after defeating the ghost of Yang Feng. Don`t stop until he`s dead again!");
                                                data.AddOption("Got~it!");
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            data.AddText("You need to smelt the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe into clinkers.");
                                            data.AddOption("Okay.");
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("You need to smelt the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe into clinkers.");
                                        data.AddOption("Okay.");
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You need to smelt the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe into clinkers.");
                                    data.AddOption("Okay.");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You need to smelt the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe into clinkers.");
                                data.AddOption("Okay.");
                                data.FinalizeDialog();
                            }

                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(721777, 3))
                        {

                            if (HasItems(client, 721791, 1) && HasItems(client, 721792, 1) && HasItems(client, 721793, 1) && HasItems(client, 721794, 1) && HasItems(client, 721777, 3) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                if (!client.Inventory.Remove(721791, 1, stream)) break;
                                if (!client.Inventory.Remove(721792, 1, stream)) break;
                                if (!client.Inventory.Remove(721793, 1, stream)) break;
                                if (!client.Inventory.Remove(721794, 1, stream)) break;
                                if (!client.Inventory.Remove(721777, 3, stream)) break;
                                client.Inventory.Add(stream, 721785);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("The time is up, and the Cauldron opened. Fortunately, the Star Tower appeared. And you received 100 Silver!");
                                data.AddText("Now that the Star Tower has been restored, you should leave this dangerous place right away. Hurry up and return to Bird Island for Sugar Tang.");
                                data.AddOption("Thanks,~bye!", 7);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You will have all three Tower Splinters after defeating the ghost of Yang Feng. Don`t stop until he`s dead again!");
                            data.AddOption("Got~it!");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        client.CreateBoxDialog("Report~back~to~Sugar~Tang~with~the~restored~Star~Tower.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(717, 577, 1015);
                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to return to Bird Island, now?");
                        data.AddOption("Yes,~please.", 3);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("You`re~on~the~way~to~Bird~Island,~now.");
                        client.Teleport(717, 577, 1015);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BlockStone)]
        public static void BlockStone(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The entrance has been blocked! You need to kill 20 Vicious Rats, so you`ll be able to get out of here!");
                        data.AddOption("Damned!");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.KunlunWanderer)]
        public static void KunlunWanderer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 403);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KunlunWanderer, client.Player.Class, 404);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KunlunWanderer, client.Player.Class, 405);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You must have heard about the Kunlun Stone! Guess what I found out? The stone is the Poison Fang of the Viper!");
                            data.AddText("~When dissolved in the Kunlun Liquid, it can be used to dye Tang Silk. Haha, I just happened to have the liquid.");
                            data.AddOption("I~have~the~Tang~Silk.", 1);
                            data.AddOption("Sounds~fantastic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Hey, glad to see you alive! How is your adventure in the Viper Cave? You know, to dye the Tang Silk, you should have the Sacred Water!");
                                data.AddText("~That`s made from the Poison Fang and the Kunlun Liquid.");
                                data.AddOption("Now~what~should~I~do?", 5);
                                data.AddOption("Got~it.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("As I mentioned before, you should have the Sacred Water composed by the Kunlun Liquid and the Poison Fang to dye the Tang Silk. I`ll give you the");
                                data.AddText("~Kunlun Liquid, if you can get the Poison Fang from the Viper in the Viper Cave. So, you are going to the Viper Cave?");
                                data.AddOption("Yeah,~but~how~to~get~there?", 4);
                                data.AddOption("Viper?~I`ll~be~killed!");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Finally, we have discovered the secret of the Tang Silk! But it seems another mystery has emerged...");
                            data.AddText("~Things are far from over.");
                            data.AddOption("I`ve~dyed~the~Tang~Silk.", 10);
                            data.AddOption("Sounds~fantastic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Someone must have told you something about the Kunlun Stone. But you know what it is? It`s just the Posion Fang of the Viper!");
                            data.AddText("~Combined with the Kunlun Liquid, it can be used to dye Tang Silk.");
                            data.AddOption("Sounds~fantastic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("Then, what are you doing here? Shouldn`t you send the Poem Silk to Villager Chou, now?");
                        data.AddOption("I`m~on~my~way.", 9);
                        data.AddOption("I~lost~the~Poem~Silk.", 8);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(721801, 1))
                        {
                            data.AddText("Oh, Sacred Water...It can be used to dye Tang Silk... It`s said a mysterious secret will appear right away!");
                            data.AddOption("Yeah...", 7);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.Contain(721804, 1))
                            {
                                data.AddText("Oh, Sacred Water...It can be used to dye Tang Silk... It`s said a mysterious secret will appear right away!");
                                data.AddOption("Yeah...");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Inventory.Contain(721803, 1))
                                {
                                    if (client.Inventory.Contain(721802, 1))
                                    {
                                        data.AddText("Now, merge the Poison Fang with the Kunlun Liquid. That`s how Sacred Water is made. Do it, now!");
                                        data.AddOption("Don`t~talk~while~I`m~pouring.");
                                        data.AddAvatar(52);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("So, you`ve obtained the Poison Fang? Great! Here is the Kunlun Liquid. After dissolving the fang into the liquid, you`ll receive the Sacred Water.");
                                        data.AddOption("Thanks.", 6);
                                        data.AddAvatar(52);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Look, I can send you to the Viper Cave if you pay me 10,000 Silver. I`ll be glad to share the Kunlun Liquid");
                                    data.AddText("~when you obtain the Poison Fang. So, you want to explore the Viper Cave?");
                                    data.AddOption("Yep!", 3);
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Then, what are you doing here? Shouldn`t you send the Poem Silk to Villager Chou, now?");
                            data.AddOption("I`m~on~my~way.", 9);
                            data.AddOption("I~lost~the~Poem~Silk.", 8);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        client.CreateBoxDialog("You~received~the~Poem~Silk.~Now,~hurry~to~find~Villager~Chou.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Contain(721801, 1))
                        {
                            data.AddText("Stop kidding me! I just saw the Poem Silk in your inventory. Hurry to take it to Villager Chou!");
                            data.AddOption("Sorry,~bye!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721801, 1);
                                client.SendSysMesage("You received the Poem Silk.");
                            }
                            else
                            {
                                data.AddText("Your inventory is full! You need to clean some room, first.");
                                data.AddOption("I`ll~be~back~soon!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721802, 1);
                            client.SendSysMesage("You obtained the Kunlun Liquid. Right click the Poison Fang to combine them into Sacred Water.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! You need to clean some room, first.");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("Look, I can send you to the Viper Cave if you pay me 10,000 Silver. I`ll be glad to share the Kunlun Liquid");
                        data.AddText("~when you obtain the Poison Fang. So, you want to explore the Viper Cave?");
                        data.AddOption("Yep!", 3);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721798, 1))
                        {
                            data.AddText("Do you mean the Tang Silk owned by Villager Chou?");
                            data.AddOption("Yeah,~he~gave~me~the~silk.", 2);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Without Tang Silk, everything will be in vain. Find Villager Chou in Maple Forest (788,456) now. I know his grandpa quite well.");
                            data.AddOption("I`ll~visit~him,~now.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Look, I can send you to the Viper Cave if you pay me 10,000 Silver. I`ll be glad to share the Kunlun Liquid");
                            data.AddText("~when you obtain the Poison Fang. So, you want to explore the Viper Cave?");
                            data.AddOption("Yep!", 3);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("Come on! 10,000 silvers wouldn`t be so hard for you. Make sure you bring enough money with you!");
                            data.AddOption("Alright.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.Money -= 10000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Teleport(135, 103, 1794);
                            client.SendSysMesage("You arrived at Viper Cave. Now kill the Viper to get the Poison Fang.");
                            client.SendSysMesage("Watch out! Rocks are falling down!!");
                        }
                        break;

                    }
            }
        }


        [NpcAttribute(NpcID.HerbalistChou)]
        public static void HerbalistChou(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 407);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("It`s a long long story...Two of my friends loved each other so much... And they wrote a book together to show their love.");
                            data.AddOption("Is~that~the~Poem~Book?", 1);
                            data.AddOption("It`s~so~romantic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("It`s a long long story...Two of my friends loved each other so much... And they wrote a book together to show their love.");
                            data.AddOption("It`s~so~romantic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("You come for the Poem Book? Aha! Finally, there is someone who know the story. It`s a long long story...They loved each other so much... Sorry,");
                        data.AddText("~but I can`t help, but talking that. The Poem Book was written by Frost Tang and Mr. Loneliness together.");
                        data.AddOption("Where~is~the~book,~now?", 2);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("The Poem Book is in the Rose Garden, but it`s hard to get through the 4 stone pillars that Mr. Loneliness has set in Bird Island. The keys");
                        data.AddText("~are 4 special items: Purple Eye, Jade Sceptre, Jadeite, and Hairpin. You need to find them and put them into the pillars to");
                        data.AddText("~open the Teleport Platform to the Rose Garden. The first two have been stolen by the Crimson Viper and Jade Courtesan! Clear?");
                        data.AddOption("I~know~my~duty~now.", 3);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        Database.Server.AddMapMonster(stream, client.Map, 2169, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Database.Server.AddMapMonster(stream, client.Map, 2170, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");

                        client.SendSysMesage("Crimson Viper and Crimson Claws have shown up! Watch out!");

                        var npc = client.Player.View.Roles(Role.MapObjectType.Npc).Where(p => p.UID == id).First();
                        if (npc != null)
                            client.Map.RemoveNpc(npc as Npc, stream);

                        break;
                    }
            }
        }
        //
        [NpcAttribute(NpcID.JadeCourtesan)]
        public static void JadeCourtesan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ah, I really do have a gift for steal! So easy for me to get the Jade Scepter! But I still need the Purple Eye, Jadeite, and Hairpin");
                            data.AddText("~to unlock the stone pillars on Bird Island.");
                            data.AddOption("Give~me~the~Jade~Scepter!", 1);
                            data.AddOption("Steal?~Shame~on~you!");
                            data.AddAvatar(145);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Have you ever heard the story of Mr. Loneliness and Frost Tang? They loved each other so much... And they wrote a book together to show their love.");
                            data.AddOption("It`s~so~romantic.");
                            data.AddAvatar(145);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            data.AddText("Who...are...you...? I...");
                            data.AddOption("Important?", 2);
                            data.AddAvatar(145);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You came for my Jade Sceptre? You must be dreaming! Time to show me what you are made of! Let`s go!");
                            data.AddOption("I`ll~let~you~know.", 3);
                            data.AddAvatar(145);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        var action = new ActionQuery()
                        {
                            ObjId = id,
                            Type = ActionType.RemoveEntity
                        };
                        unsafe
                        {
                            client.Send(stream.ActionCreate(&action));
                        }
                        Database.Server.AddMapMonster(stream, client.Map, 2171, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Contain(710217, 1))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 710217);
                                client.SendSysMesage("You received the Jade Sceptre!");
                                if (client.Inventory.Contain(710216, 1))
                                {
                                    if (client.Inventory.Contain(710217, 1))
                                    {
                                        if (client.Inventory.Contain(710218, 1))
                                        {
                                            if (client.Inventory.Contain(710219, 1))
                                            {
                                                client.SendSysMesage("You`ve collected the Purple Eye, Jade Scepter, Jadeite, and Hairpin. Go to open the Teleport Platform on Bird Island (655,693).");
                                            }
                                            else
                                            {
                                                client.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                            }
                                        }
                                        else
                                        {
                                            client.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                        }
                                    }
                                    else
                                    {
                                        client.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                    }
                                }
                                else
                                {
                                    client.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                }
                            }
                            else
                            {
                                client.SendSysMesage("Your inventory is full!");
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LeopardPillar)]
        public static void LeopardPillar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Activate~this~pillar.", 1);
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 1))
                        {
                            data.AddText("The Leopard Pillar has been activated. Now, go activate the other pillars.");
                            data.AddOption("Alright.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There`s a hole in the pillar. It seems that you can put something in it.");
                            data.AddOption("Put~in~Purple~Eye.", 5);
                            data.AddOption("Put~in~Jade~Sceptre.", 4);
                            data.AddOption("Put~in~Jadeite.", 3);
                            data.AddOption("Put~in~Hairpin.", 2);
                            data.AddOption("Not~now.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710217, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            if (!client.Inventory.Remove(710219, 1, stream)) break;
                            client.Player.AddMapEffect(stream, 698, 731, "accession");
                            client.SendSysMesage("You used the Hairpin.");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
                            data.AddText("The Leopard Pillar is activated!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TigerPillar)]
        public static void TigerPillar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Activate~this~pillar.", 1);
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1))
                        {
                            data.AddText("The Tiger Pillar has been activated. Now, go activate the other pillars.");
                            data.AddOption("Alright.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There`s a hole in the pillar. It seems that you can put something in it.");
                            data.AddOption("Put~in~Purple~Eye.", 5);
                            data.AddOption("Put~in~Jade~Sceptre.", 4);
                            data.AddOption("Put~in~Jadeite.", 3);
                            data.AddOption("Put~in~Hairpin.", 2);
                            data.AddOption("Not~now.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710217, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            if (!client.Inventory.Remove(710218, 1, stream)) break;
                            client.SendSysMesage("You used the Jadeite.");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                            client.Player.AddMapEffect(stream, 694, 735, "accession1");
                            data.AddText("The Tiger Pillar is activated!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DragonPillar)]
        public static void DragonPillar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Activate~this~pillar.", 1);
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                        {
                            data.AddText("The Dragon Pillar has been activated. Now, go activate the other pillars.");
                            data.AddOption("Alright.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There`s a hole in the pillar. It seems that you can put something in it.");
                            data.AddOption("Put~in~Purple~Eye.", 5);
                            data.AddOption("Put~in~Jade~Sceptre.", 4);
                            data.AddOption("Put~in~Jadeite.", 3);
                            data.AddOption("Put~in~Hairpin.", 2);
                            data.AddOption("Not~now.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710217, 1))
                        {
                            if (!client.Inventory.Remove(710217, 1, stream)) break;
                            client.SendSysMesage("You used the Jade Sceptre.");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                            client.Player.AddMapEffect(stream, 694, 721, "accession1");

                            data.AddText("The Dragon Pillar is activated!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SparrowPillar)]
        public static void SparrowPillar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Activate~this~pillar.", 1);
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        break;

                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                        {
                            data.AddText("The Sparrow Pillar has been activated. Now, go activate the other pillars.");
                            data.AddOption("Alright.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There`s a hole in the pillar. It seems that you can put something in it.");
                            data.AddOption("Put~in~Purple~Eye.", 5);
                            data.AddOption("Put~in~Jade~Sceptre.", 4);
                            data.AddOption("Put~in~Jadeite.", 3);
                            data.AddOption("Put~in~Hairpin.", 2);
                            data.AddOption("Not~now.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            client.Player.AddMapEffect(stream, 683, 730, "accession1");
                            if (!client.Inventory.Remove(710216, 1, stream)) break;
                            client.SendSysMesage("You used the Purple Eye.");

                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                            data.AddText("The Sparrow Pillar is activated!");
                            data.AddOption("Great!");
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710217, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //
        [NpcAttribute(NpcID.MrLoneliness)]
        public static void MrLoneliness(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster, p => p.Alive == true).Count() == 0)
                            {
                                if (client.Inventory.Contain(710221, 1))
                                {
                                    data.AddText("Yeah, the Heaven Fan is mine. How did you find it? You came for the poem book, right? So many people have asked");
                                    data.AddText("~me about it, but you`re the only one who gave me the Heaven Fan. So, here`s the book. Take it to Sugar Tang, and be gone!");
                                    data.AddOption("With~pleasure!", 1);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You`re the 101st guy to come for the Poem Book. But, I won`t give it to you if you can`t get me the Heaven Fan.");
                                    data.AddOption("Heaven~Fan...");
                                    data.AddOption("Bird~Island,~please!", 2);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Blech! Look what evil you have brought out! Shouldn`t you sweep them out of my house?");
                                data.AddOption("Oh,~my!");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You`re the 101st guy to come for the Poem Book. But, I won`t give it to you if you can`t get me the Heaven Fan.");
                            data.AddOption("Heaven~Fan...");
                            data.AddOption("Bird~Island,~please!", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Teleport(717, 577, 1015);
                        client.SendSysMesage("You`ve been sent back to Bird Island.");
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(710221, 1))
                        {
                            if (!client.Inventory.Remove(710221, 1, stream)) break;
                            client.Inventory.Add(stream, 710222);
                            client.SendSysMesage("The Heaven Fan has been taken away. You obtained the Poem Book.");
                            client.Teleport(717, 577, 1015);
                            client.SendSysMesage("You`ve been sent back to Bird Island. Find Sugar Tang to get your reward.");
                        }
                        else
                        {
                            data.AddText("You`re the 101st guy to come for the Poem Book. But, I won`t give it to you if you can`t get me the Heaven Fan.");
                            data.AddOption("Heaven~Fan...");
                            data.AddOption("Bird~Island,~please!", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.NightBox)]
        public static void NightBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster, p => p.Alive == true).Count() == 0)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                {
                                    if (client.Inventory.Contain(710222, 1))
                                    {
                                        data.AddText("This is an empty box. Go check the other boxes.");
                                        data.AddOption("I~see.");
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Inventory.Contain(710221, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                            data.AddOption("Gimme!~Gimme!", 1);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    {
                                        data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                        data.AddOption("Gimme!~Gimme!", 1);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("There`s a poem engraved on the box. Open it now?");
                                            data.AddOption("Can`t~wait!", 2);
                                            data.AddOption("Not~now.");
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Watch your back!!");
                                data.AddOption("Oh,~my!!");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("This is an empty box. Go check the other boxes.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        Database.Server.AddMapMonster(stream, client.Map, 2172, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                        client.SendSysMesage("There`s something in the box...What?! A Poem Ghost!! Look out!");
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710221);
                            data.AddText("You obtained a Heaven Fan!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.SendSysMesage("There`s not enough space in your inventory!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DarkBox)]
        public static void DarkBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster, p => p.Alive == true).Count() == 0)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                {
                                    if (client.Inventory.Contain(710222, 1))
                                    {
                                        data.AddText("This is an empty box. Go check the other boxes.");
                                        data.AddOption("I~see.");
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Inventory.Contain(710221, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                            data.AddOption("Gimme!~Gimme!", 1);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    {
                                        data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                        data.AddOption("Gimme!~Gimme!", 1);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("There`s a poem engraved on the box. Open it now?");
                                            data.AddOption("Can`t~wait!", 2);
                                            data.AddOption("Not~now.");
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Watch your back!!");
                                data.AddOption("Oh,~my!!");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("This is an empty box. Go check the other boxes.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        Database.Server.AddMapMonster(stream, client.Map, 2172, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                        client.SendSysMesage("There`s something in the box...What?! A Poem Ghost!! Look out!");
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710221);
                            data.AddText("You obtained a Heaven Fan!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.SendSysMesage("There`s not enough space in your inventory!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DayBox)]
        public static void DayBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster, p => p.Alive == true).Count() == 0)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                {
                                    if (client.Inventory.Contain(710222, 1))
                                    {
                                        data.AddText("This is an empty box. Go check the other boxes.");
                                        data.AddOption("I~see.");
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Inventory.Contain(710221, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                            data.AddOption("Gimme!~Gimme!", 1);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    {
                                        data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                        data.AddOption("Gimme!~Gimme!", 1);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("There`s a poem engraved on the box. Open it now?");
                                            data.AddOption("Can`t~wait!", 2);
                                            data.AddOption("Not~now.");
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Watch your back!!");
                                data.AddOption("Oh,~my!!");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("This is an empty box. Go check the other boxes.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        Database.Server.AddMapMonster(stream, client.Map, 2172, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.SendSysMesage("There`s something in the box...What?! A Poem Ghost!! Look out!");
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710221);
                            data.AddText("You obtained a Heaven Fan!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.SendSysMesage("There`s not enough space in your inventory!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Platform)]
        public static void Platform(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Rules are rules. You`ve put the special items into the four stone pillars? If not, the Teleport Platform will not activate.");
                            data.AddOption("Absolutely.~Activate~it~now!", 1);
                            data.AddOption("Er,~sorry.~I~haven`t.");
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Rules are rules. You`ve put the special items into the four stone pillars? If not, the Teleport Platform will not activate.");
                                data.AddOption("Absolutely.~Activate~it~now!", 4);
                                data.AddOption("Er,~sorry.~I~haven`t.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Teleport Platform looks mysterious...");
                                data.AddOption("Send~me~to~the~Rose~Garden.", 3);
                                data.AddOption("It~looks~so~weird.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Teleport Platform looks mysterious...");
                            data.AddOption("It~looks~so~weird.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        client.Player.AddMapEffect(stream, 686, 723, "bomb");
                        data.AddText("The platform is activated! Good job! Now, you wanna be teleported to the Rose Garden or not?");
                        data.AddOption("What~do~you~think?", 3);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Teleport(69, 56, 1791);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("You~will~be~teleported~to~the~Rose~Garden.");

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1, 1, 1, 1))
                        {
                            client.Player.AddMapEffect(stream, 686, 723, "bomb");
                            data.AddText("The platform is activated! Good job! Now, you wanna be teleported to the Rose Garden or not?");
                            data.AddOption("What~do~you~think?", 2);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, but if the 4 stone pillars are not activated, the Teleport Platform will just be a decoration.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            client.Teleport(69, 56, 1791);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.CreateBoxDialog("You~will~be~teleported~to~the~Rose~Garden.");
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.VillagerChou)]
        public static void VillagerChou(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 400);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 401);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 402);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KunlunWanderer, client.Player.Class, 405);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 406);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 407);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Hey, buddy! Want to have a drink with me? Wine is a good thing that can help you forget everything.");
                            data.AddOption("Do~you~know~Herbalist~Chou?", 1);
                            data.AddOption("I~will.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Did Sugar Tang tell you anything about the Tang Silk?");
                            data.AddOption("I`ve~got~the~answer.", 20);
                            data.AddOption("I`m~just~passing~by.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You brought me the Snake Meat and wine?");
                                data.AddOption("Yeah,~here~you~are.", 6);
                                data.AddOption("Sorry,~I~haven`t...");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("He is just in this village. Is that related with Sugar Tang? Oh, I know. Herbalist Chou was Frost Tang`s best friend. So she thought the Herbalist Chou`s grandson might know something, right?");
                                data.AddOption("Yeah,~you~guessed~correctly.", 5);
                                data.AddOption("I`m~not~sure.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                 && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Why are you still here? Take the Tang Silk to Sugar Tang (685,599) on Bird Island to learn more.");
                                data.AddOption("I`m~on~my~way.", 13);
                                data.AddOption("I~quit!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Calm down, okay? The money you spent will be worthy. See, this Tang Silk is the only thing my grandpa left.");
                                data.AddOption("What~can~I~do~with~the~silk?", 11);
                                data.AddOption("It~looks~nothing~special.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("So scary. I didn`t expect those Crimson Claws would follow me here.");
                                data.AddOption("Let`s~go~into~~the~Poem~Silk.", 26);
                                data.AddOption("I`ve~dealt~with~them.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Wait, wait... I read the poems on the silk again and find something strange.");
                                data.AddOption("What~did~you~find~out?", 25);
                                data.AddOption("Stop~day~dreaming...");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
           && client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("How is my grandpa`s grave, now? I hope it hasn`t been swallowed by weeds. By the way, what did you get from the Swan Lake?");
                                data.AddOption("The~Purple~Eye.", 31);
                                data.AddOption("I~got~nothing,~till~now.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("To find out the real meaning of the Poem Silk, we should find the Swan Lake where my grandpa was buried.");
                                data.AddOption("Swan~Lake?", 30);
                                data.AddOption("A~grave?~Let~me~go!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
     && client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Look, we have never been so close to the success, now. As long as we have all the 4 things, we can unlock the way to the Platform.");
                                data.AddOption("Where~are~they?", 34);
                                data.AddOption("I`m~too~tired~to~continue.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I`ve checked the Purple Eye you brought. That`s real.");
                                data.AddOption("Then,~what`s~the~next~thing?", 40);
                                data.AddOption("It~should~be.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("It`s been long before I heard from Sugar Tang (BirdIsland 685,599) last time. She has always wanted to find out the secrets of her dearly departed grandma.");
                            data.AddOption("She~must~be~fine.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 40:
                    {
                        data.AddText("Since you have the Purple Eye, I`ll give you my Hairpin. Now, we just need to find the Jade Scepter and the Jadeite to unlock the way to the Platform.");
                        data.AddOption("I~know~what~to~do~now.", 34);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }

                case 34:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        data.AddText("Hmm, you`ve retrieved the Purple Eye from the Swan Lake. I have the Hairpin, and Sugar Tang owns the Jadeite. For the Jade Scepter, it was stolen by Jade Courtesan. Hurry and collect all of them!");
                        data.AddOption("Give~me~the~Hairpin.", 36);
                        data.AddOption("Give~me~the~Purple~Eye.", 35);
                        data.AddOption("I`ll~return~soon!");
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            data.AddText("Isn`t that the Hairpin in your inventory? You should wear glasses to check, next time.");
                            data.AddOption("Sorry,~I~forgot.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 710219, 1);
                                client.CreateBoxDialog("You~received~the~Hairpin.");
                            }
                            else
                            {
                                data.AddText("Your inventory is full! You need to clean some room, first.");
                                data.AddOption("Okay.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 35:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("I saw the Purple Eye is still in your inventory. Stop kidding me, okay?");
                            data.AddOption("Sorry,~I~forgot.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 710216, 1);
                                client.CreateBoxDialog("You~received~the~Purple~Eye.");
                            }
                            else
                            {
                                data.AddText("Your inventory is full, now. Why not tidy it up, first?");
                                data.AddOption("I`ll~be~back~soon!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 33:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Since you have the Purple Eye, I`ll give you my Hairpin. Now, we just need to find the Jade Scepter and the Jadeite to unlock the way to the Platform.");
                            data.AddOption("I~know~what~to~do~now.", 34);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("You got the Purple Eye? Wow, you are incredible powerful!");
                            data.AddOption("Thanks.", 33);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.Contain(721799, 1))
                            {
                                data.AddText("It seems you haven`t found anything special from the Swan Lake. Wait, you have a stick of Incense, right? Go use it at the Swan Lake (55,109)!");
                                data.AddOption("I`m~on~my~way.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Did you lose your way? The entrance to Swan Lake is in the Maple Forest (471,844). Before approaching, you should burn the Incense at the Swan Lake.");
                                data.AddOption("Sorry,~I~lost~the~Incense.", 32);
                                data.AddOption("Thanks!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721799, 1);
                            client.CreateBoxDialog("You~received~a~stick~of~Incense~from~Villager~Chou.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full now. Come back when you have some room. I`ll be here waiting.");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("Sugar Tang`s grandma and Mr. Loneliness often dated in Swan Lake. That`s why you need to go there and investigate. The entrance");
                        data.AddText("~to Swan Lake is in the Maple Forest (471,844). Here`s an Incense for you. Burn it at Swan Lake (55,109). Don`t forget it.");
                        data.AddOption("Burn~Incense?~What?", 28);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                            client.Inventory.Add(stream, 721799, 1);
                            client.SendSysMesage("You received a stick of Incense.");
                        }
                        else
                        {
                            data.AddText("I can`t help, but find your inventory is full. Why not tidy up, first?");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Sugar Tang`s grandma and Mr. Loneliness often dated in Swan Lake. That`s why you need to go there and investigate. The entrance");
                            data.AddText("~to Swan Lake is in the Maple Forest (471,844). Here`s an Incense for you. Burn it at Swan Lake (55,109). Don`t forget it.");
                            data.AddOption("Burn~Incense?~What?", 28);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Player.View.ContainMobInScreen("CrimsonClaw") == false)
                        {
                            data.AddText("Poem Silk? Wait, I remember something! The Heaven Fan! Mr. Loneliness had done something with it! We need to");
                            data.AddText("~find it first! Now you`d better go to a place called Swan Lake where my grandpa was buried. Damn! Why do I have to be blind!");
                            data.AddOption("Relax.~I~can~deal~with~it.", 27);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Oh, my! They found me! Crimson Claw! Help! Please! Please!");
                            data.AddOption("Sigh.~Man,~I`m~here.~Relax.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 25:
                    {
                        data.AddText("Listen to me... Oh, no!");
                        data.AddOption("What`s~wrong?", 22);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(721801, 1))
                        {
                            data.AddText("What... There are poems on the Tang Silk only? Are you sure that isn`t a treasure map? Anyway, thanks for your help.");
                            data.AddOption("I`m~doing~my~job.", 21);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I can`t find the Poem Silk you mentioned. Did you lose it somewhere?");
                            data.AddOption("It`s~very~possible.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(721801, 1))
                        {
                            if (!client.Inventory.Remove(721801, 1, stream)) break;
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 100 Silver!");
                                data.AddText("Listen to me... Oh, no!");
                                data.AddOption("What`s~wrong?", 22);
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I can`t find the Poem Silk you mentioned. Did you lose it somewhere?");
                            data.AddOption("It`s~very~possible.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 22:
                    {
                        Database.Server.AddMapMonster(stream, client.Map, 2169, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(721798, 1))
                        {
                            data.AddText("If you find out the secret of this Tang Silk, don`t forget to let me know it, please!");
                            data.AddOption("No~problem.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.Contain(721801, 1))
                            {
                                data.AddText("If you find out the secret of this Tang Silk, don`t forget to let me know it, please!");
                                data.AddOption("No~problem.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What? You lost it? How can you be so careless! My father only gave me two. This is the last one. Take it. Keep it safe for me,");
                                data.AddText("~will you?");
                                data.AddOption("I~will.", 14);
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721798, 1);
                            client.SendSysMesage("You~received~the~Tang~Silk~from~Villager~Chou.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. Please make some room first.");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 11:
                    {
                        data.AddText("Haha, it`s just a joke! Look, I have the Tang Silk my grandpa left. I want to know more about it, too. But you see,");
                        data.AddText("~I am a blind man... Well, you just take it to Sugar Tang. If you get something good, don`t forget to tell me!");
                        data.AddOption("Okay.", 10);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721798, 1);
                            client.SendSysMesage("You obtained the Tang Silk from Villager Chou. Now find Sugar Tang in Bird Island (685,599) to learn more about it.");
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        }
                        else
                        {
                            data.AddText("Your inventory is full. Please make some room first.");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(721800, 1))
                        {
                            if (client.Inventory.Contain(710220, 5))
                            {
                                if (client.Inventory.Remove(710220, 5, stream))
                                {
                                    if (!client.Inventory.Remove(721800, 1, stream)) break;
                                    if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                    {
                                        client.Player.Money += 100;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                        client.SendSysMesage("You received 100 Silver!");
                                        data.AddText("Haha, it`s just a joke! Look, I have the Tang Silk my grandpa left. I want to know more about it, too. But you see,");
                                        data.AddText("~I am a blind man... Well, you just take it to Sugar Tang. If you get something good, don`t forget to tell me!");
                                        data.AddOption("Okay.", 10);
                                        data.AddAvatar(77);
                                        data.FinalizeDialog();

                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Did you get the wine and meat I want? The answer is `NO`. My memory is bad when I am starving. Just bring me a bottle of Bamboo");
                                data.AddText("~Wine from Wine Kiddo (Phoenix Castle 182,280) and 5 pieces of Snake Meat in the northeast of Maple Forest. I await you back!");
                                data.AddOption("I`m~on~my~way.", 7);
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Did you get the wine and meat I want? The answer is `NO`. My memory is bad when I am starving. Just bring me a bottle of Bamboo");
                            data.AddText("~Wine from Wine Kiddo (Phoenix Castle 182,280) and 5 pieces of Snake Meat in the northeast of Maple Forest. I await you back!");
                            data.AddOption("I`m~on~my~way.", 7);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(721800, 1))
                        {
                            if (client.Inventory.Contain(710220, 5))
                            {
                                data.AddText("Mmm, that`s good. I do like snakes, but only after they`re in my belly. It so happens, that I am the man you`re looking for...");
                                data.AddOption("You~made~me~wait~for~that?", 9);//9564290
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Did you get the wine and meat I want? The answer is `NO`. My memory is bad when I am starving. Just bring me a bottle of Bamboo");
                                data.AddText("~Wine from Wine Kiddo (Phoenix Castle 182,280) and 5 pieces of Snake Meat in the northeast of Maple Forest. I await you back!");
                                data.AddOption("I`m~on~my~way.", 7);
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Did you get the wine and meat I want? The answer is `NO`. My memory is bad when I am starving. Just bring me a bottle of Bamboo");
                            data.AddText("~Wine from Wine Kiddo (Phoenix Castle 182,280) and 5 pieces of Snake Meat in the northeast of Maple Forest. I await you back!");
                            data.AddOption("I`m~on~my~way.", 7);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        client.SendSysMesage("Buy the Bamboo Wine from Wine Kiddo (Phoenix Castle 182,280) and collect 5 pieces of Snake Meat from northeast of Maple Forest.");
                        break;
                    }
                case 5:
                    {
                        data.AddText("Of course, I know where he is! Sorry, I feel a little hungry. Would you mind bringing me some Bamboo Wine from");
                        data.AddText("~Wine Kiddo (Phoenix Castle 182,280)? Hey! And Snake Meat in the northeast of Maple Forest is amazing. Get me five!");
                        data.AddOption("Greedy~son~of~a..", 3);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Herbalist Chou? He`s dead. But I know his grandson.");
                        data.AddOption("Where~can~I~find~him?", 2);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Of course, I know where he is! Sorry, I feel a little hungry. Would you mind bringing me some Bamboo Wine from");
                            data.AddText("~Wine Kiddo (Phoenix Castle 182,280)? Hey! And Snake Meat in the northeast of Maple Forest is amazing. Get me five!");
                            data.AddOption("Greedy~son~of~a..", 3);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Buy~the~Bamboo~Wine~from~a~Wine~Kiddo~in~Phoenix~Castle~(182,280)~and~get~5~pieces~of~Snake~Meat~from~northeast~Maple~Forest.");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SugarTang)]
        public static void SugarTang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 520);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GhoulKong, client.Player.Class, 528);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 400);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 402);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 403);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("Hey, you are still too green! Keep practicing, and come back when you reach level 100.");
                            data.AddOption("Alright.");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Have you ever heard of Frost Tang? Yeah, she was very famous in her time. Not only for her beauty, but also for her great skill. You know");
                                data.AddText("~she is my grandma and I wish I could be as good as her. That`s the reason why I should find the Star Tower she lost.");
                                data.AddOption("Star~Tower?", 10);
                                data.AddOption("I~don`t~think~so.");
                                data.AddAvatar(175);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                {
                                    if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                    {
                                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                        {
                                            data.AddText("Did you find Chou`s children? No?! Come on, Chou is my grandma`s best friend, so he must know something useful!");
                                            data.AddOption("How~can~I~find~him?", 35);
                                            data.AddOption("I`m~just~passing~by.");
                                            data.AddAvatar(175);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("Have you ever seen Yang Feng? He was the last head of the Yang family. Though he`s dead, he won`t give up his desire to destroy the Star Tower.");
                                            data.AddText("~Anyway, there is still something I can`t understand about my family. I heard the Heaven Fan has the answer I want.");
                                            data.AddText("~are still many things I cannot understand about my family. I heard that the attack talisman, the Heaven Fan, has the answers I want.");
                                            data.AddOption("Heaven~Fan?", 30);
                                            data.AddOption("I~see.");
                                            data.AddAvatar(175);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                    {
                                        data.AddText("Did you find Herbalist Chou? Or any of his descendants?");
                                        data.AddOption("He~gave~me~the~Tang~Silk.", 40);
                                        data.AddOption("Sorry,~no.");
                                        data.AddAvatar(175);
                                        data.FinalizeDialog();
                                    }
                                    else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                                    && !client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    {
                                        data.AddText("Why are you still here? You should take the Tang Silk to the Kunlun Wanderer. He may know something more about it. Was there anything else you needed?");
                                        data.AddOption("How~can~I~find~him?", 43);
                                        data.AddOption("I~see.");
                                        data.AddAvatar(175);
                                        data.FinalizeDialog();
                                    }
                                    else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                    {
                                        data.AddText("Sorry, I didn`t recognize you, just now. How is your job going? I just made an important discovery about My grandma.");
                                        data.AddOption("I~brought~you~a~book!", 50);
                                        data.AddOption("I~see.");
                                        data.AddAvatar(175);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("I read the Poem Book my grandma wrote again, and I totally understand the love between Mr. Loneliness and her, now. Thanks for all you have done for us!");
                                        data.AddOption("You`re~welcome.");
                                        data.AddAvatar(175);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Have you ever heard of Frost Tang? Yeah, she was very famous in her time. Not only for her beauty, but also for her great skill. You know");
                                    data.AddText("~she is my grandma and I wish I could be as good as her. That`s the reason why I should find the Star Tower she lost.");
                                    data.AddOption("Star~Tower?", 1);
                                    data.AddOption("I~don`t~think~so.");
                                    data.AddAvatar(175);
                                    data.FinalizeDialog();
                                }
                            }

                        }
                        break;
                    }
                case 50:
                    {
                        if (client.Inventory.Contain(710222, 1))
                        {
                            data.AddText("Oh, this is just my grandma`s Poem Book! I`ll bury this book at her grave, then she can rest in peace... Well, my friend,");
                            data.AddText("~thanks for what you have done for my family. Here`s your reward. Can you guess what it is?");
                            data.AddOption("Better~be~the~Heaven~Fan...", 51);
                            data.AddOption("Don`t~care.");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What book? I can`t find any books in your bag! By the way, my grandma did write a poem book before. If we can find that book, things may be easier to understand.");
                            data.AddOption("Alright.");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 51:
                    {
                        if (client.Inventory.Contain(710222, 1))
                        {
                            if (HasItems(client, 710222, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                            {
                                if (!client.Inventory.Remove(710222, 1, stream)) break;
                                client.Inventory.Add(stream, 201009, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.SendSysMesage("Sugar Tang took the Poem Book from you. In return, she gave you the Heaven Fan!");
                            }
                        }
                        else
                        {
                            data.AddText("What book? I can`t find any books in your bag! By the way, my grandma did write a poem book before. If we can find that book, things may be easier to understand.");
                            data.AddOption("Alright.");
                            data.AddOption("Don`t~care.");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 43:
                    {
                        data.AddText("Hmm... We need to refer this Tang Silk to Kunlun Wanderer. You can probably find him around (490,618) in Desert City. By the way, the Kunlun Wanderer will go on a trip. soon. So, be quick!");
                        data.AddOption("I`m~on~my~way.", 42);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }

                case 40:
                    {
                        if (client.Inventory.Contain(721798, 1))
                        {
                            data.AddText("Tang Silk? It looks so beautiful! There must be many secrets about this silk. Thanks for your help!");
                            data.AddOption("It`s~okay.", 41);
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You said that Villager Chou gave you a Tang Silk. But where is it now? Don`t tell me you have sold it for money.");
                            data.AddOption("Maybe~I~lost~it~somewhere?");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 41:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Hmm... We need to refer this Tang Silk to Kunlun Wanderer. You can probably find him around (490,618) in Desert City. By the way, the Kunlun Wanderer will go on a trip. soon. So, be quick!");
                            data.AddOption("I`m~on~my~way.", 42);
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 42:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.CreateBoxDialog("Pay~a~visit~to~the~Kunlun~Wanderer~around~(490,618)~in~Desert~City.");
                        break;
                    }
                case 35:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            data.AddText("You can probably find him in the southeast of Maple Forest. Well, I await your good news!");
                            data.AddOption("Okay,~bye!");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("How could you be so careless to lose that? Fortunately, I have another one. Here you are! Don`t lose it again, okay?");
                            data.AddOption("Sorry,~I~won`t.", 36);
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 36:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710218, 1);
                            client.SendSysMesage("You~received~a~Jadeite.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Why not tidy it up, first?");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("I don`t know much about the Heaven Fan, but I feel that must be related to the Poem Book written by my grandma and Mr. Loneliness, years ago.");
                        data.AddText("~You found the Star Tower, so I think it won`t be hard for you to find a book, right?");
                        data.AddOption("Well,~do~you~have~any~clue?", 31);
                        data.AddOption("I~have~more~important~things.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Herbalist Chou is my grandma`s best friend, but he died, years ago. Could you go find his children with this Jadeite? I`m sure the family is still");
                        data.AddText("~living in the small village, in the southeast of Maple Forest.");
                        data.AddOption("Okay.~Bye!", 32);
                        data.AddOption("I~have~more~important~things.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.Inventory.Add(stream, 710218, 1);
                            client.SendSysMesage("Sugar Tang gave you the Jadeite that Frost Tang left behind. You`ll need it, someday.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Why not tidy it up, first?");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("The Star Tower is a defensive talisman once possessed by my grandma. It was incredibly powerful, but she lost it and no one knows its whereabouts.");
                        data.AddText("~My hero, will you help me find it?");
                        data.AddOption("Any~clue?", 15);
                        data.AddOption("You~mean~this~talisman?", 11);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.AddText("Hm... The Star Tower has been broken into three pieces. I heard it through the grapevine.");
                        data.AddText("~Yang Yun may know something about one of the pieces. He is on Bird Island, now.");
                        data.AddOption("Then~where~is~the~second~one?", 4);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }

                case 11:
                    {
                        if (client.Inventory.Contain(721785, 1))
                        {
                            if (HasItems(client, 721785, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(721785, 1, stream)) break;
                                client.Inventory.Add(stream, 202008, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.SendSysMesage("You received an Elite Star Tower(B)!");
                                data.AddText("Yeah, that`s exactly what I`m looking for! My grandma`s Star Tower was known for its legendary defensive qualities. But it broke into");
                                data.AddText("~3 pieces when a man from the Yang family broke her heart. This started the long-standing family feud between our Tang and the Yang.");
                                data.AddOption("Love~is~never~easy.", 12);
                                data.AddAvatar(175);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I don`t find any talisman in your inventory. If you really want to help me, visit Yang Yun (732,516) on Bird Island. He will definitely be able to help you.");
                            data.AddOption("Okay.~Bye!");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {

                        data.AddText("So the ghost of Yang Feng, the late head of the family, wants to destroy the talisman, rather than let someone else have it.");
                        data.AddText("~That`s why some weird things happened ever since...");
                        data.AddOption("Sounds~complicated.", 13);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("Well, who knows? This is a crazy world. Anyway, thank you. You may keep this Star Tower as a souvenir.");
                        data.AddText("~That`s the least I can do to thank you for your help.");
                        data.AddOption("Thanks!");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("The Star Tower is a defensive talisman once possessed by my grandma. It was incredibly powerful, but she lost it and no one knows its whereabouts.");
                        data.AddText("~My hero, will you help me find it?");
                        data.AddOption("Any~clue?", 3);
                        data.AddOption("You~mean~this~talisman?", 2);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Stop kidding me! I don`t see any talismans with you. Sigh... When can I retrieve the Star Tower?");
                        data.AddOption("It~won`t~be~long.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Hm... The Star Tower has been broken into three pieces. I heard it through the grapevine.");
                        data.AddText("~Yang Yun may know something about one of the pieces. He is on Bird Island, now.");
                        data.AddOption("Then~where~is~the~second~one?", 4);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("The second piece is in a Hut in the north of Bird Island. It belongs to the three Ruan Brothers. If you can find Ruan Brother, you may know more.");
                        data.AddText("~When you get two pieces, go to Desert City. There is someone, Plum, who should know about the third piece.");
                        data.AddOption("I`ll~visit~Yang~Yuan,~first.", 7);
                        data.AddOption("Ruan~Brother?", 6);
                        data.AddOption("Plum?", 5);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Plum knows where to find the third piece. You can probably find her around (472,578) in Desert City. Watch out for the bandits when you get there.");
                        data.AddOption("No~problem.");
                        data.AddOption("I~still~have~some~questions.", 4);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Ruan Brother`s Hut is in the north of Bird Island guarded by some magical stone doors (208,216). Ye Sheng (192,212) may tell you how to break those doors.");
                        data.AddOption("Got~it.");
                        data.AddOption("I~still~have~some~questions.", 4);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                        }
                        else
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            client.CreateBoxDialog("You~were~advised~to~find~Yang~Yun~on~Bird~Island.");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MrWonder)]
        public static void MrWonder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 200);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {
                                data.AddText("It`s~a~legendary~bird~of~enormous~size~and~strength.~Though~I`ve~figured~out~the~way~of~making~the~Tempest~Wings,");
                                data.AddText("~you~need~to~bring~me~Roc`s~Feather~from~the~Rainbow~Sky.~It~could~be~much~more~difficult~than~any~challenges");
                                data.AddText("~you`ve~ever~taken.~If~you`re~ready,~I`ll~send~you~there~with~a~fee~of~5~Meteor~Scrolls.");
                                if (client.Inventory.Contain(3006392, 1))
                                {
                                    data.AddOption("Hand~in~Roc`s~Feather.", 10);//97257880
                                    data.AddOption("I`ll~talk~to~you~later.");
                                    data.AddAvatar(6);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                                    {
                                        data.AddOption("Send~me~to~the~Rainbow~Sky!", 4);
                                        data.AddOption("I`ll~talk~to~you~later.");
                                        data.AddAvatar(6);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddOption("Head~to~the~sky~(5~Meteor~Scrolls)", 2);
                                        data.AddOption("I`ll~talk~to~you~later.");
                                        data.AddAvatar(6);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                            {

                                data.AddText("So~many~people~ask~for~the~Tempest~Wings,~but~it`s~as~hard~as~climbing~up~to~the~sky.");
                                data.AddText("~Let~alone~the~loss~of~the~technique,~it~requires~Roc`s~Feather~to~make~the~Tempest~Wings.");
                                data.AddText("~You~know~Roc?");
                                data.AddOption("I`m~all~ears.", 1);
                                data.AddAvatar(6);
                                data.FinalizeDialog();

                            }
                        }
                        else
                        {
                            data.AddText("Roc~is~an~enormous~legendary~bird~transformed~from~a~gigantic~fish~of~terrific~strength.");
                            data.AddOption("Oh,~I~know~it!");
                            data.AddAvatar(6);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3006392, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            if (!client.Inventory.Remove(3006392, 1, stream)) break;
                            client.Inventory.Add(stream, 204008, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.SendSysMesage("You~received~Elite~Tempest~Wings~(B)!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        else
                        {
                            data.AddText("It`s~really~hard~to~get~Roc`s~Feather.~I~know~you`ve~tried~your~best.~It~doesn`t~matter.");
                            data.AddOption("Sorry,~leave~me~alone.");
                            data.AddAvatar(6);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        data.AddText("It`s~a~legendary~bird~of~enormous~size~and~strength.~Though~I`ve~figured~out~the~way~of~making~the~Tempest~Wings,");
                        data.AddText("~you~need~to~bring~me~Roc`s~Feather~from~the~Rainbow~Sky.~It~could~be~much~more~difficult~than~any~challenges");
                        data.AddText("~you`ve~ever~taken.~If~you`re~ready,~I`ll~send~you~there~with~a~fee~of~5~Meteor~Scrolls.");
                        if (client.Inventory.Contain(3006392, 1))
                        {
                            data.AddOption("Hand~in~Roc`s~Feather.", 10);//97257880
                            data.AddOption("I`ll~talk~to~you~later.");
                            data.AddAvatar(6);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                            {
                                data.AddOption("Send~me~to~the~Rainbow~Sky!", 4);
                                data.AddOption("I`ll~talk~to~you~later.");
                                data.AddAvatar(6);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddOption("Head~to~the~sky~(5~Meteor~Scrolls)", 2);
                                data.AddOption("I`ll~talk~to~you~later.");
                                data.AddAvatar(6);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3006392, 1))
                        {
                            client.SendSysMesage("You`ve~obtained~Roc`s~Feather.~No~need~to~challenge~the~Rainbow~Sky,~again.");
                        }
                        else
                        {
                            client.Teleport(125, 99, 1109);
                            client.SendSysMesage("You`ve~arrived~in~the~Rainbow~Sky.~Hurry~and~defeat~the~Heaven~Roc~for~its~feather!");
                            Database.Server.AddMapMonster(stream, client.Map, 7696, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~5~Meteor~Scrolls~to~enter~the~Rainbow~Sky?");
                        data.AddOption("Yes!", 3);
                        data.AddOption("No.");
                        data.AddAvatar(6);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(3006392, 1))
                        {
                            client.SendSysMesage("You`ve~obtained~Roc`s~Feather.~No~need~to~challenge~the~Rainbow~Sky,~again.");
                        }
                        else
                        {
                            if (client.Inventory.Contain(720027, 5) || client.Inventory.Contain(720027, 5, 1))
                            {
                                if (client.Inventory.Remove(720027, 5, stream))
                                {//7696
                                    client.Teleport(125, 99, 1109);
                                    Database.Server.AddMapMonster(stream, client.Map, 7696, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                                    client.SendSysMesage("You`ve~arrived~in~the~Rainbow~Sky.~Hurry~and~defeat~the~Heaven~Roc~for~its~feather!");

                                }
                            }
                            else
                            {
                                data.AddText("Come~on,~you~don`t~have~5~Meteor~Scrolls.~It~couldn`t~be~too~much~for~you,~could~it?");
                                data.AddOption("No,~but...");
                                data.AddAvatar(6);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.NaughtyBoy)]
        public static void NaughtyBoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Naughty boys sometimes make good men.");
                        data.AddOption("You~naughty~boy!");
                        data.AddAvatar(68);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
    }
}
