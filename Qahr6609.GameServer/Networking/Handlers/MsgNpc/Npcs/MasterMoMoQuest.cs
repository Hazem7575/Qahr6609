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
        [NpcAttribute(NpcID.Celestine)]
        public static void Celestine(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(700001, 1) && client.Inventory.Contain(700011, 1) && client.Inventory.Contain(700021, 1)
                            && client.Inventory.Contain(700031, 1) && client.Inventory.Contain(700041, 1) && client.Inventory.Contain(700051, 1)
                            && client.Inventory.Contain(700061, 1))
                        {
                            if (client.Inventory.Contain(721258, 1))
                            {
                                client.Inventory.Add(stream, 721259);
                                data.AddText("Aha! You must come for Celestial Stone! Check your inventory! It`s there! Now, take it to Eternity (298,217), if you wish to get 1st rebirth.");
                                data.AddOption("OK.~Thank~you.");
                                data.AddAvatar(15);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Although~you~get~seven~normal~gems,~I~cannot~refine~the~CelestialStone~for~you~without~the~CleanWater.~");
                                data.AddOption("I~will~get~it~ready.");
                                data.AddAvatar(15);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("People~are~pursuing~greater~achievement~during~their~lives,~but~none~can~make~it~due~to~the~limit~of~human~constitution.");
                            data.AddOption("What~does~it~mean?", 1);
                            data.AddOption("I~don`t~believe~it.");
                            data.AddAvatar(15);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Mortals~are~mundane.~Only~getting~rid~of~it~can~help~them~accomplish~their~aims.");
                        data.AddText("If~you~are~high~level~enough,~you~can~get~reborn~to~learn~more~and~stronger~skills.");
                        data.AddOption("I~am~satisfied.");
                        data.AddOption("How~to~get~reborn?", 2);
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("It~is~difficult.~First,~you~should~reach~certain~level.~Second,~you~need~a~Celestial~Stone.");
                        data.AddOption("How~to~get~CelestialStone?", 3);
                        data.AddOption("Forget~it.");
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("CelestialStone~is~made~of~7~gems:~VioletGem,~KylinGem,~RainbowGem,~MoonGem,~PhoenixGem,~FuryGem,~DragonGem,~and~CleanWater.");
                        data.AddOption("What~is~Clean~Water?", 4);
                        data.AddOption("It~is~difficult.");
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("It~is~used~to~get~rid~of~your~mundaneness,~and~then~you~won`t~be~affected~by~the~environment.");
                        data.AddText("By~the~way,~Clean~Water~comes~from~celestial~rinsing.");
                        data.AddOption("What~are~gems~used~for?", 5);
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Only~the~seven~gems~can~protect~you~during~the~rebirth.");
                        data.AddOption("I~will~collect~them~now.", 6);
                        data.AddOption("I~changed~my~mind.");
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("It~is~easy~to~get~the~gems.~But~CleanWater¡­");
                        data.AddOption("But~what?", 7);
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("The~Adventure~island~is~the~headstream~of~CleanWater.~But~it~is~occupied~by~WaterEvil~and~he~uses~spell~to~hide~the~stream.");
                        data.AddOption("What~can~I~do?", 8);
                        data.AddOption("I~will~give~up.");
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("WaterEvilElder~will~go~to~get~the~water~every~certain~time.~If~you~defeat~him,~you~may~get~the~water.");
                        data.AddText("But~he~is~very~hard~to~deal~with.");
                        data.AddOption("I~see.~Thank~you.");
                        data.AddOption("Anything~else?", 9);
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(700001, 1) && client.Inventory.Contain(700011, 1) && client.Inventory.Contain(700021, 1)
                           && client.Inventory.Contain(700031, 1) && client.Inventory.Contain(700041, 1) && client.Inventory.Contain(700051, 1)
                           && client.Inventory.Contain(700061, 1))
                        {
                            if (client.Inventory.Contain(721258, 1))
                            {
                                client.Inventory.Add(stream, 721259);
                                data.AddText("Aha! You must come for Celestial Stone! Check your inventory! It`s there! Now, take it to Eternity (298,217), if you wish to get 1st rebirth.");
                                data.AddOption("OK.~Thank~you.");
                                data.AddAvatar(15);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Although~you~get~seven~normal~gems,~I~cannot~refine~the~CelestialStone~for~you~without~the~CleanWater.~");
                                data.AddOption("I~will~get~it~ready.");
                                data.AddAvatar(15);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("People~are~pursuing~greater~achievement~during~their~lives,~but~none~can~make~it~due~to~the~limit~of~human~constitution.");
                            data.AddOption("What~does~it~mean?", 1);
                            data.AddOption("I~don`t~believe~it.");
                            data.AddAvatar(15);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PowerBook)]
        public static void PowerBook(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1851, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Oh...the last page is completely blank...What`s wrong? Quick,");
                            data.AddText("~go find Old Beggar to find out why!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Step 10: From Better to Best!");
                                data.AddText("~Get a Blue Bird Plume in Labyrinth 3F. Then go to Labyrinth 4F, and right click it near the body");
                                data.AddText("~within 3 seconds after you kill a Minotaur L120. You need ignite 10 bodies.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.QuestGUI.CheckQuest(1849, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    data.AddText("Step 9: From Good to Better.");
                                    data.AddText("~Kill 5 Blue Birds and 5 Fiend Bats in Labyrinth 3F.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckQuest(1848, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                    {
                                        data.AddText("Step 8: You Are What You Eat!");
                                        data.AddText("~Kill some Fiend Bats in Labyrinth 3F and take the Fiend Bat Wing to the fire");
                                        data.AddText("~there (100,66) and roast it. Eat the roasted Fiend Bat Wing.");
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.QuestGUI.CheckQuest(1847, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                        {
                                            data.AddText("Step 7: Halve it!");
                                            data.AddText("~Now, you can enter the 3rd floor to kill 2 Blue Birds within 20 seconds.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.QuestGUI.CheckQuest(1846, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                            {
                                                data.AddText("Step 6: Double practice!");
                                                data.AddText("~Now, go kill 10 Bladelings and 10 Agile Rats in Labyrinth 2F.");
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.QuestGUI.CheckQuest(1845, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                {
                                                    data.AddText("Step 5: Want a Bite?");
                                                    data.AddText("~They always say, teeth for teeth...");
                                                    data.AddText("~Go kill 1 Agile Rat in Labyrinth 2F and");
                                                    data.AddText("~collect 1 Agile Rat Fang. Bite it and see if you could leave");
                                                    data.AddText("~a teeth mark on it. But be careful. Don`t destroy it.");
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.QuestGUI.CheckQuest(1844, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                    {
                                                        data.AddText("Step 4: Speedy Speed!");
                                                        data.AddText("~Now, you can enter the 2nd floor to kill 2 Bladelings within 10 seconds.");
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.QuestGUI.CheckQuest(1843, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                        {
                                                            data.AddText("Step 3: Practice Makes Perfect!");
                                                            data.AddText("~Well, the only way to test whether you`ve mastered the previous two is to");
                                                            data.AddText("~do it again. Now, go kill 15 Slingers and 15 Gold Ghosts. Do it quick!");
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            if (client.Player.QuestGUI.CheckQuest(1842, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                            {
                                                                data.AddText("Step 2: Ghost Goer!");
                                                                data.AddText("~How do you feel after the warm-up exercises?");
                                                                data.AddText("~Now kill 10 Gold Ghosts after herding them to");
                                                                data.AddText("~Trial Cave 1F (77,23), (102,136) or (146,104).");
                                                                data.FinalizeDialog();
                                                            }
                                                            else
                                                            {

                                                                data.AddText("Step1: Warm Up!");
                                                                data.AddText("~A good beginning. To warm yourself up, go kill 20 Slingers in Labyrinth 1F.");
                                                                data.AddText("~Don`t take them for granted.");
                                                                data.FinalizeDialog();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OldBeggar)]
        public static void OldBeggar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1840);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1841);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1851);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1852);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                            data.AddText("~My power excites fear from all who dare to challenge me.");
                            data.AddOption("Please~help~me~get~stronger.", 1);
                            data.AddOption("Who~are~you?");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 10);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 9);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 15);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 10);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                         && !client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Who~are~you?", 21);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 20);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();

                            }
                        }
                        else
                        {
                            data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                            data.AddText("~My power excites fear from all who dare to challenge me.");
                            data.AddOption("Who~are~you?", 8);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("What? You don`t know me? I`m Old Beggar, and I`m the greatest Martial Arts Master in the world!");
                        data.AddOption("Crazy.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("Well, it`s just a little mistake, and I`ve admitted my fault. Now you can claim your rewards from Taskmaster Chang in Twin City.");
                        data.AddOption("Sounds~good.", 19);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Contain(721924, 1))
                        {
                            data.AddText("Mmmmmm...you have finished the first 10 steps in the book? I am impressed.");
                            data.AddText("~Trust me, you will gain some more power after the second round.");
                            data.AddText("~Just carry on and repeat all ten steps!");
                            data.AddOption("Nonsense!", 16);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("All right, all right. I`ll give you another one. Please, don`t lose it again.");
                            data.AddOption("I~won`t.", 12);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 16:
                    {
                        data.AddText("Well. You are so smart. To tell you the truth, it`s fake. The whole book, is just a story.");
                        data.AddOption("You~make~me~sick!", 17);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        data.AddText("Please forgive me. I was just too hungry. Now you can claim your rewards from Taskmaster Chang.");
                        data.AddOption("I~forgive~you.", 18);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        if (HasItems(client, 721924, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(721924, 1, stream)) break;
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.Player.Money += 6000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(300);
                            client.SendSysMesage("You received 1 hour EXp and 6,000 Silver!");
                            data.AddText("Well, it`s just a little mistake, and I`ve admitted my fault. Now you can claim your rewards from Taskmaster Chang in Twin City.");
                            data.AddOption("Sounds~good.", 19);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 19:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.CreateBoxDialog("You~can~claim~an~Armor~from~Taskmaster~Chang.");
                        break;
                    }

                case 10:
                    {
                        data.AddText("Hey, you have already got the Power Book! Why not go to the Trial Cave and start your practice with that book now?");
                        data.AddText("~I can teleport you to the Trial Cave, if you want. So?");
                        data.AddOption("I~lost~the~book.", 11);
                        data.AddOption("I~see.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(721924, 1))
                        {
                            data.AddText("What?! You LOST it? Why?!");
                            data.AddOption("Calm~down.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("All right, all right. I`ll give you another one. Please, don`t lose it again.");
                            data.AddOption("I~won`t.", 12);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Contain(721924, 1))
                                client.Inventory.Add(stream, 721924);
                            client.CreateBoxDialog("You~received~the~Power~Book~from~the~Old~Beggar!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        data.AddText("Yeah, that is the Power Book in your hands! Hurry to start your practice with that book in the Trial Cave.");
                        data.AddText("~If you want, I can teleport you to the Trial Cave. So?");
                        data.AddOption("I~see.", 7);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("What? You don`t know me? I`m Old Beggar, and I`m the greatest Martial Arts Master in the world!");
                        data.AddOption("Crazy.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Wait! You don`t need to know me...But I see a genius for Martial Arts in you! Come come! I have a good book for you.");
                        data.AddOption("Impress~me.", 2);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("I spent a life time learning this. According to my experience, it won`t take a genius like you long to master it.");
                        data.AddText("~I`m sure you will be a legend.");
                        data.AddOption("Give~it~to~me.", 5);
                        data.AddOption("What~is~it~about?", 3);
                        data.AddOption("You~are~crazy.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Well...All things have a price, right? Pay me one Meteor first.");
                        data.AddOption("OK.", 6);
                        data.AddOption("Nope.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        if (client.Inventory.Contain(1088001, 1) || client.Inventory.Contain(1088001, 1, 1))
                        {
                            if (!client.Inventory.Remove(1088001, 1, stream)) break;
                            if (!client.Inventory.Contain(721924, 1))
                                client.Inventory.Add(stream, 721924);
                            client.SendSysMesage("You gave the Old Beggar a Meteor and received the Power Book!");

                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 100 Silver!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                data.AddText("Yeah, that is the Power Book in your hands! Hurry to start your practice with that book in the Trial Cave.");
                                data.AddText("~If you want, I can teleport you to the Trial Cave. So?");
                                data.AddOption("I~see.", 7);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("My hero, it`s the Power Book, understand? Millions of people will kill for it.");
                            data.AddOption("Wait~for~me~to~get~a~meteor.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("You`ll~need~to~go~to~Labyrinth~and~practice~with~the~Power~Book.");
                        break;
                    }
                case 3:
                    {
                        data.AddText("Many people had tried to steal the book, but nobody ever discovered its secret");
                        data.AddOption("Why?", 4);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        data.AddText("Power Book! It is the essence of all the secrets behind Chinese Martial Arts.");
                        data.AddText("~Only I can understand it.");
                        data.AddOption("Whatever.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.MasterMoMo)]
        public static void MasterMoMo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1835);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1836);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1837);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1838);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1839);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                            data.AddText("~the hallowed heights here...");
                            data.AddOption("Can~I~help~you?", 1);
                            data.AddOption("Sorry,~but~I`m~busy.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 10);
                                data.AddOption("I`ll~come~back~later.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 2);
                                data.AddOption("Sorry,~but~I`m~busy.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                       && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 21);
                                data.AddOption("Too~dangerous~for~me.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 20);
                                data.AddOption("I`ll~come~back~later."); data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                 && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 30);
                                data.AddOption("I`ll~come~back~later.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 27);
                                data.AddOption("Too~dangerous~for~me.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
         && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("What a wild desert!");
                                data.AddOption("See~you.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 35);
                                data.AddOption("I`ll~come~back~later.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                            data.AddText("~the hallowed heights here...");
                            data.AddOption("Sounds~familiar.", 9);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("I need the heart of the Bull Master. I need to smash it in public to let others know that");
                        data.AddText("~our Mu family will avenge whoever dares to shame us!");
                        data.AddOption("Hand~in~the~heart.", 31);
                        data.AddOption("OK.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        data.AddText("Thanks a lot! You are my hero. Now you should go back to Taskmaster Chang.");
                        data.AddOption("OK.", 33);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.Contain(710260, 1))
                        {
                            data.AddText("Great! Bull Master`s heart! Thanks a lot!");
                            data.AddOption("You`re~welcome.", 32);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Don`t bother to ask if you didn`t retrieve the Bull Heart.");
                            data.AddOption("Oh.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.Contain(710260, 1))
                        {
                            if (!client.Inventory.Remove(710260, 1, stream)) break;

                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Player.Money += 6000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.GainExpBall(300);
                                client.SendSysMesage("You received 1 hour EXP and 6,000 Silver!");
                                client.CreateBoxDialog("Thanks~a~lot.~~Now~you~should~go~back~to~Taskmaster~Chang.");
                                data.AddText("Thanks a lot! You are my hero. Now you should go back to Taskmaster Chang.");
                                data.AddOption("OK.", 33);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Don`t bother to ask if you didn`t retrieve the Bull Heart.");
                            data.AddOption("Oh.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 33:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 21:
                    {
                        data.AddText("I am amazed by your courage, my hero. Now listen carefully. Mad Bulls are now at Heaven Well (102,292) in Mystic Castle.");
                        data.AddText("~Take this Ghost Mirror. Use it near the Heaven Well and herd Mad Bulls into it and kill them. The Ghost Mirror will");
                        data.AddText("~then absorb its ghost and turn it into a ghost crystal. You`ll just need to take back 3 Bull Ghosts.");
                        data.AddOption("I`ve~taken~them~here.", 24);
                        data.AddOption("Gimme~another~Ghost~Mirror.", 22);
                        data.AddOption("I`ll~be~right~on~it.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Contain(710261, 3))
                        {
                            if (client.Inventory.Remove(710261, 3, stream))
                            {
                                if (HasItems(client, 721921, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    if (!client.Inventory.Remove(721921, 1, stream)) break;
                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.SendSysMesage("You completed the quest and received 100 Silver!");
                                    data.AddText("You`ve got 3 ghosts! Now I will tell you a secret.");
                                    data.AddOption("OK.", 25);
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You`ll need to collect 3 ghosts for me, and then I will tell you a secret.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 27:
                    {
                        data.AddText("You`ve got 3 ghosts! Now I will tell you a secret.");
                        data.AddOption("OK.", 25);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        data.AddText("It`s about my family. They all call me Master Mo Mo because I have mastered all the Kung Fu skills accumulated by Mo`s family.");
                        data.AddText("~But we were scattered by some weird spell of Bull Master at (186,359) in Mystic Castle. Will you avenge them");
                        data.AddText("~for me? Kill the Bull Master and retrieve the Bull Heart. It`s really a big favor, and I will reward you handsomely after that.");
                        data.AddOption("I`m~in.", 26);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        client.CreateBoxDialog("Go~to~kill~the~Bull~Master~in~Mystic~Castle~(186,359)~and~retrieve~the~Bull~Heart.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain(721921, 1))
                        {
                            data.AddText("Sorry, but you`ve already had the mirror in your inventory.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I have a spare one...But only this time? Don`t lost it again, please.");
                            data.AddOption("I~won`t.", 23);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721921, 1);
                            client.CreateBoxDialog("You~received~the~Ghost~Mirror~from~Master~Mo~Mo.~Don`t~let~him~down.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Great! You`ve got 10 horns. But I still have a quest for you...");
                        data.AddOption("What~is~it?", 15);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        data.AddText("Great responsibility comes from great power.");
                        data.AddOption("Indeed.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("What took you so long? And you didn`t even bring back 10 Burnt Bull Horns? Please, go and get them there...I will pay!");
                        data.AddOption("I`ve~got~them~here.", 13);
                        data.AddOption("I~lost~the~Fervent~Pill.", 11);
                        data.AddOption("I~forgot.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        // Both quest items are checked before either one is taken.
                        if (client.Inventory.Contain(710259, 10) && client.Inventory.Contain(721920, 1))
                        {
                            if (client.Inventory.Remove(710259, 10, stream))
                            {
                                if (!client.Inventory.Remove(721920, 1, stream)) break;
                                client.SendSysMesage("You completed the quest and received 100 Silver!");
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Great! You`ve got 10 horns. But I still have a quest for you...");
                                    data.AddOption("What~is~it?", 15);
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }

                            }

                        }
                        else
                        {
                            data.AddText("I was not kidding with you! I need 10 of them! Ten! T-E-N!");
                            data.AddOption("Easy~man!~Don`t~yell.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        data.AddText("I`ve found out that it is much easier to capture the bull`s ghost than its body. Here is a Ghost Mirror. You`ll need to absorb 3 ghosts of the Mad Bull in the Mystic Castle.");
                        data.AddOption("I`ll~make~it.", 16);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        data.AddText("I am amazed by your courage, my hero. Now listen carefully. Mad Bulls are now at Heaven Well (102,292) in Mystic Castle.");
                        data.AddText("~Take this Ghost Mirror. Use it near the Heaven Well and herd Mad Bulls into it and kill them. The Ghost Mirror will");
                        data.AddText("~then absorb its ghost and turn it into a ghost crystal. You`ll just need to take back 3 Bull Ghosts.");
                        data.AddOption("I`ll~be~right~on~it.", 17);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721921);
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.CreateBoxDialog("You~received~the~Ghost~Mirror~from~Master~Mo~Mo!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(721920, 1))
                        {
                            data.AddText("Sorry, but you`ve had the Fervent Pill in your inventory.");
                            data.AddOption("Sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What? You lost my Fervent Pill? What a waste...Never mind, I have a spare one. But don`t lose it again, will you?");
                            data.AddOption("I~won`t.", 12);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721920, 1);
                            client.CreateBoxDialog("You~received~a~Fervent~Pill~from~Master~Mo~Mo!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver and a Fervent Pill! Go and collect Burnt Bull Horns in Mystic Castle.");
                            data.AddText("I`m master Mo Mo. I like making inventions.");
                            data.AddOption("Can~I~help~you?", 2);
                            data.AddOption("An~inventor.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("My Fervent Pill is quite dangerous and powerful. If you want, I can give it to you.");
                        data.AddOption("Claim~a~Fervent~Pill.", 3);
                        data.AddOption("What~is~Fervent~Pill?", 4);
                        data.AddOption("Too~dangerous~for~me.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Contain(721920, 1) == false)
                            {
                                client.Inventory.Add(stream, 721920, 1);
                                client.SendSysMesage("You received a Fervent Pill! Go and collect Burnt Bull Horns in Mystic Castle.");
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Sounds~familiar.", 6);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("People always say, those Bull Monsters have the hardest Horns in the world which can`t be hurt by any human device.");
                        data.AddText("~I want to test the power of the Fervent Pill with them...If you could bring back 10 Burnt Bull Horns, I`ll do it.");
                        data.AddOption("How~to~get~them?", 5);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Of course with my Fervent Pill! Its effect will last for 1 minute. You will be engulfed in flame. They will");
                        data.AddText("~enhance your power without scorching you. By the way, you can use it repeatedly. Now, go and get me 10 Burnt Bull Horns.");
                        data.AddOption("Deal.", 6);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Great responsibility comes from great power.");
                        data.AddOption("Indeed.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RingPack)]
        public static void RingPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can choose a ring or a bracelet.");
                        data.AddOption("Super~Diamond~Bracelet.", 1);
                        data.AddOption("Super~Dragon~Ring.", 2);
                        data.AddOption("I`ll~think~about~it.");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728250, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 152229, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Diamond~Bracelet!");
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
                            if (client.Inventory.Remove(728250, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 150219, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Dragon~Ring!");
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
        [NpcAttribute(NpcID.Shark)]
        public static void Shark(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1830);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Shark, client.Player.Class, 1831);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Shark, client.Player.Class, 1832);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                            data.AddOption("I~love~treasures.", 1);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                                data.AddOption("I~love~treasures.", 12);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(53);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                                data.AddOption("I~love~treasures.", 10);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(53);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                    && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                            data.AddOption("I~love~treasures.", 20);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                            data.AddOption("Money~isn`t~everything.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }


                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(721871, 1))
                        {
                            data.AddText("Calm down and listen to me. It is not fake. The portrait was specially made and can only be shown with the Bat");
                            data.AddText("~Essence. Use it and give it to Louis before the Essence runs out, got it?");
                            data.AddOption("Gotcha.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("A little bird picked it up and returned it to me...Here you are. Don`t lose the portrait again.");
                            data.AddOption("Thanks.", 14);
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }

                        break;
                    }

                case 12:
                    {
                        data.AddText("What`s the matter? I`ve already given you the portrait.");
                        data.AddOption("It`s~a~fake~one.", 13);
                        data.AddOption("Nothing~wrong.");
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(721878, 1))
                        {
                            data.AddText("Calm down and listen to me. It is not fake. The portrait was specially made and can only be shown with the Bat");
                            data.AddText("~Essence. Use it and give it to Louis before the Essence runs out, got it?");
                            data.AddOption("Gotcha.", 6);
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("A little bird picked it up and returned it to me...Here you are. Don`t lose the portrait again.");
                            data.AddOption("Thanks.", 14);
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721878, 1);
                            client.CreateBoxDialog("You~received~the~Victory~Portrait~from~Shark!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("What`s the matter? I`ve already given you the portrait.");
                        data.AddOption("It`s~a~fake~one.", 11);
                        data.AddOption("Nothing~wrong.");
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        data.AddText("Calm down and listen to me. It is not fake. The portrait was specially made and can only be shown with the Bat");
                        data.AddText("~Essence. Use it and give it to Louis before the Essence runs out, got it?");
                        data.AddOption("Gotcha.", 6);
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        data.AddText("Calm down and listen to me. It is not fake. The portrait was specially made and can only be shown with the Bat");
                        data.AddText("~Essence. Use it and give it to Louis before the Essence runs out, got it?");
                        data.AddOption("Gotcha.", 6);
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (HasItems(client, 721878, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (!client.Inventory.Remove(721878, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            client.Inventory.Add(stream, 721871);
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("What`s the matter? I`ve already given you the portrait.");
                        data.AddOption("It`s~a~fake~one.", 5);
                        data.AddOption("Nothing~wrong.");
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            data.AddText("You`ll just need to give a price within 1-99999 Silver and you can have it. If your price is too high or low, you will lose your silver offered and won`t receive the portrait.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                            client.Player.AddPick(stream, "Bidding...", 3);

                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Shhh...Come and take a look at this...I just obtained it from the black market...This portrait is valuable...");
                        data.AddOption("Is~it~a~Victory~Portrait?", 2);
                        data.AddAvatar(53);
                        data.FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Louis)]
        public static void Louis(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1828);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1829);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1830);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Shark, client.Player.Class, 1832);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1833);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1834);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("Wow,~I~love~treasures!", 2);
                            data.AddOption("I~see.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("I~love~all~treasures!", 30);
                            data.AddOption("I~see.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("That`s~why~I`m~here!", 8);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                      && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                                data.AddOption("I~love~treasures~more.", 15);
                                data.AddOption("I~see.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                                data.AddOption("Tell~me~about~the~second~one.", 14);
                                data.AddOption("I~see.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
            && !client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("Tell~me~about~the~last~one!", 36);
                            data.AddOption("I~see.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
      && !client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                                {
                                    data.AddText("Congratulations, you have answered all of my questions.");
                                    data.AddOption("Hurray!");
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Thank you for all this. If you still have time, I have some riddles for you. Are you interested?");
                                    data.AddOption("Tell~me~more.", 40);
                                    data.AddOption("Sorry,~but~I`m~busy.");
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Thanks! You`ve brought me all the three treasures. Now you can find Taskmaster Chang to claim your rewards.");
                                data.AddOption("I~see.", 60);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("I~see.", 1);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 60:
                    {
                        client.CreateBoxDialog("Find~Taskmaster~Chang~to~claim~the~rewards.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        data.AddText("Incredible! You are just so brave and kind. Now you can find Taskmaster Chang to claim your rewards.");
                        data.AddOption("OK.", 40);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            data.AddText("Congratulations, you have answered all of my questions.");
                            data.AddOption("Hurray!");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Mystic Castle! What a fascinating scenery! Millions of people travelled here to seek exotic treasures.");
                            data.AddOption("I~see.", 41);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 43:
                    {
                        data.AddText("Hey, you can do better than that, right?");
                        data.AddOption("Try~again.", 41);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        client.Player.LuiseQuestions += 1;
                        data.AddText("Very good, very good. Want more?");
                        data.AddOption("Yes.", 41);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 44:
                    {
                        client.Player.LuiseQuestions += 1;
                        data.AddText("Brilliant.");
                        data.AddOption("Yes.", 41);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 45:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1) == false)
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest6.MissionId, 1);
                            client.Player.LuiseQuestions += 1;
                            client.GainExpBall(300);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.SendSysMesage("You received 30 minutes EXP!");
                            data.AddText("Very good. Your intelligence matches your valor!");
                            data.AddOption("Cannot~agree~more.", 50);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 41:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            data.AddText("Congratulations, you have answered all of my questions.");
                            data.AddOption("Hurray!");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.LuiseQuestions == 0)
                        {
                            data.AddText("What comes down but never goes up?");
                            data.AddOption("Rain.", 42);
                            data.AddOption("Sun.", 43);
                            data.AddOption("Moon.", 43);
                            data.AddOption("Earth.", 43);
                            data.AddOption("Quit.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.LuiseQuestions == 1)
                        {
                            data.AddText("What is black when it is clean and white when it is dirty?");
                            data.AddOption("Desk.", 43);
                            data.AddOption("Blackboard.", 44);
                            data.AddOption("Chair.", 43);
                            data.AddOption("Chalk.", 43);
                            data.AddOption("Quit.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.LuiseQuestions == 2)
                        {
                            data.AddText("What has ears, but can`t hear?");
                            data.AddOption("Barley.", 43);
                            data.AddOption("Rice.", 43);
                            data.AddOption("Corn.", 45);
                            data.AddOption("Oat.", 43);
                            data.AddOption("Quit.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.LuiseQuestions >= 3)
                        {
                            data.AddText("Congratulations, you have answered all of my questions.");
                            data.AddOption("Hurray!");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 40://
                    {
                        data.AddText("Ok, now listen carefully.");
                        data.AddOption("Can`t~wait.", 41);
                        data.AddOption("Sorry,~but~I`m~busy.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        if (client.Inventory.Contain(721872, 1))
                        {
                            if (HasItems(client, 721872, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                if (!client.Inventory.Remove(721872, 1, stream)) break;
                                client.GainExpBall(300);
                                client.Player.Money += 6000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You received 1 hour EXP and 6,000 Silver! Find Taskmaster Chang to claim your rewards.");
                                client.CreateBoxDialog("You~gave~the~Buddha~Relic~to~Louis.");
                                client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                                data.AddText("Incredible! You are just so brave and kind. Now you can find Taskmaster Chang to claim your rewards.");
                                data.AddOption("OK.", 40);
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("My hero, don`t forget to take this Pure Bottle with you. Kill the Jinx Tomb Bat (728,676) first, then use the Bottle before");
                            data.AddText("~its body disappears to get the Buddha Relic out of his body. Here you are, take this Pure Bottle.");
                            data.AddOption("Claim~the~Pure~Bottle.", 38);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 38:
                    {
                        if (client.Inventory.Contain(721874, 1))
                        {
                            data.AddText("Take good care of this Pure Bottle. It will come in handy when you get there.");
                            data.AddOption("I~see.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Never mind. I have a backup one. But don`t lose it again, will you?");
                            data.AddOption("Sorry.", 39);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 39:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721874);
                            client.CreateBoxDialog("You~received~a~Pure~Bottle~from~Louis!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 36:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Did you get the Buddha Relic?");
                            data.AddOption("Yes.", 37);
                            data.AddOption("I`m~on~my~way.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The last one is the Buddha Relic. It was stolen by a Jinx Tomb Bat. But this time I won`t sell it.");
                            data.AddText("~I will return it to those faithful monks. Join me, you won`t regret it!");
                            data.AddOption("Good.~I`m~in.", 32);
                            data.AddOption("Not~this~time.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 34:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721874);
                            client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                            client.CreateBoxDialog("You~received~a~Pure~Bottle~from~Louis!");

                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 33:
                    {
                        data.AddText("But that is not all. Take this Pure Bottle with you. Kill the Jinx Tomb Bat (728,676) first, then use the Bottle before");
                        data.AddText("~its body disappears to get the Buddha Relic out of its body. Hear you are, take this Pure Bottle.");
                        data.AddOption("Claim~the~Pure~Bottle.", 34);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        data.AddText("Good! I knew you would do this! Jinx Tomb Bat is now in Mystic Castle (728,676). The Buddha Relic is right inside its belly.");
                        data.AddOption("I~see.", 33);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (HasItems(client, 721871, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId) && client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 5, 1))
                        {
                            if (!client.Inventory.Remove(721871, 1, stream)) break;
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.SendSysMesage("You received 100 Silver!");
                            client.CreateBoxDialog("You~gave~the~Victory~Portrait~to~Louis.");
                            data.AddText("The last one is the Buddha Relic. It was stolen by a Jinx Tomb Bat. But this time I won`t sell it.");
                            data.AddText("~I will return it to those faithful monks. Join me, you won`t regret it!");
                            data.AddOption("Good.~I`m~in.", 32);
                            data.AddOption("Not~this~time.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Contain(721871, 1))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 5, 1))
                            {
                                if (DateTime.Now > client.Player.RealPortraitStamp.AddSeconds(60))
                                {
                                    client.Player.QuestGUI.SetQuestObjectives(stream, ActiveQuest4.MissionId, 0, 0);
                                    data.AddText("Hey man! Nothing here? What are you up to?");
                                    data.AddOption("Easy~there!");
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Wonderful! This is the Victory Portrait. Maybe I can draw one like this and sell at a high price too.");
                                    data.AddOption("Are~you~kidding?", 31);
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hey man! Nothing here? What are you up to?");
                                data.AddOption("Easy~there!");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            if (client.Inventory.Contain(721878, 1))
                            {
                                data.AddText("Hey man! Nothing here? What are you up to?");
                                data.AddOption("Easy~there!");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Go find Shark in Mystic Castle (310,631) and see what he knows about the Victory Portrait.");
                                data.AddOption("Got~it.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        data.AddText("Go find Shark in Mystic Castle (310,631) and see what he knows about the Victory Portrait.");
                        data.AddOption("Got~it.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.AddText("Well, the second treasure is Victory Portrait. It is worth more than the Zen Scripture. Wanna join me?");
                        data.AddOption("Let`s~go.", 13);
                        data.AddOption("Sorry,~but~I`m~busy.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The Zen Scripture is kept in the Scripture Vault in Mystic Castle (91,283). You`ll need to use the Reveal Vial to reveal the compartment.");
                            data.AddText("~Then use the key you will get from the Bats to open it.");
                            data.AddOption("I~see.");
                            data.AddOption("I~lost~the~Reveal~Vial.", 9);
                            data.AddOption("Hand~in~the~scripture.", 11);//1273620
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Here it is! Mystic Castle! They said you could find all the treasures here! This time I came for three of them in particular.");
                            data.AddText("~My hero, will you help me? I will reward you handsomely if you find them all!");
                            data.AddOption("Sounds~interesting.", 4);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(721870, 1))
                        {
                            data.AddText("Thank you, my hero! Now I just need to wait for those greedy buyers to come and set a price for this.");
                            data.AddOption("There~you~are.", 12);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Are you kidding me? You don`t have the scripture. Keep working!");
                            data.AddOption("Just~a~joke.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        data.AddText("The desert around sucks, but how can I leave without the Victory Portrait! My hero, go find Shark there (310,631).");
                        data.AddText("~He should be able to help.");
                        data.AddOption("Got~it.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(721870, 1))
                        {
                            if (HasItems(client, 721870, 1) && HasItems(client, 721873, 1) && HasItems(client, 721876, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(721870, 1, stream)) break;
                                if (!client.Inventory.Remove(721873, 1, stream)) break;
                                if (!client.Inventory.Remove(721876, 1, stream)) break;

                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You received 100 Silver!");

                                client.CreateBoxDialog("You~gave~the~Zen~Scripture~to~Louis.");
                                data.AddText("Well, the second treasure is Victory Portrait. It is worth more than the Zen Scripture. Wanna join me?");
                                data.AddOption("Let`s~go.", 13);
                                data.AddOption("Sorry,~but~I`m~busy.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Are you kidding me? You don`t have the scripture. Keep working!");
                            data.AddOption("Just~a~joke.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(721873, 1))
                        {
                            data.AddText("Sorry, but you`ve had the Reveal Vial in your inventory.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Neat! Zen Scripture, kept in a Scripture Box, is in a compartment in Scripture Vault in Mystic Castle (91,283). Take this");
                            data.AddText("~Reveal Vial and use it there to reveal the compartment. Then use the key you will get from the Bats to open it. Clear?");
                            data.AddOption("Claim~the~Reveal~Vial.", 10);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        client.Inventory.Add(stream, 721873, 1);
                        client.SendSysMesage("You received a Reveal Vial from Louis!");
                        client.CreateBoxDialog("Now~go~to~Scripture~Vault~in~Mystic~Castle~(91,283).");
                        break;
                    }
                case 2:
                    {
                        data.AddText("It`s hard to make money nowadays. I have been noticing this area for a long time, and I`m sure that there are plenty of treasures here.");
                        data.AddOption("Wow.", 3);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721873, 1);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.SendSysMesage("You received a Reveal Vial from Louis!");
                            client.CreateBoxDialog("Now~go~to~Scripture~Vault~in~Mystic~Castle~(91,283).");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        data.AddText("Neat! Zen Scripture, kept in a Scripture Box, is in a compartment in Scripture Vault in Mystic Castle (91,283). Take this");
                        data.AddText("~Reveal Vial and use it there to reveal the compartment. Then use the key you will get from the Bats to open it. Clear?");
                        data.AddOption("Claim~the~Reveal~Vial.", 7);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("I heard through the grapevine that it is now kept in the Scripture Vault in Mystic Castle. But those Tomb Bats are guarding it.");
                        data.AddText("~I can`t get any closer to it...Will you help me with this?");
                        data.AddOption("I`m~in.", 6);
                        data.AddOption("Sorry,~but~I`m~busy.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Well, the first one is Zen Scripture. It was taken from Japan by some unknown eminent monk...I don`t get it...But it could sell");
                        data.AddText("~a high price in the open market.");
                        data.AddOption("Where~is~it~now?", 5);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You received 100 Silver!");
                                data.AddText("Here it is! Mystic Castle! They said you could find all the treasures here! This time I came for three of them in particular.");
                                data.AddText("~My hero, will you help me? I will reward you handsomely if you find them all!");
                                data.AddOption("Sounds~interesting.", 4);
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Well, some of the Scriptures are even more valuable than boxes of gold...");
                        data.AddOption("Gorgeous.");
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MrFree)]
        public static void MrFree(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(723087, 1))
                        {
                            client.Player.OnRemoveLukyAmulet = true;
                            data.AddText("I~love~living~a~seclusive~life.~Don`t~bother~me!");
                            data.AddOption("I~heard~that~you~need~LuckyAmulets.", 1);
                            data.AddAvatar(74);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I~love~living~a~seclusive~life.~Don`t~bother~me!");
                            data.AddOption("I`ll~leave~now.");
                            data.AddAvatar(74);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes.~My~only~hobby~is~to~collect~LuckyAmulets.");
                        data.AddOption("I~happen~to~have~one.", 2);
                        data.AddOption("I`d~better~keep~it.");
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Great!~If~you~present~LuckyAmulet~to~me,~I`ll~make~a~unique~hairstyle~for~you.");
                        data.AddOption("Thank~you.", 3);
                        data.AddOption("I`d~better~keep~it.");
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.IsBoy())
                        {
                            data.AddText("Thank~you~very~much.~Which~hairstyle~would~you~like~to~select~from?");
                            data.AddOption("Dignified~one.", 4);
                            data.AddOption("Unruly~one.", 5);
                            data.AddOption("Staid~one.", 6);
                            data.AddOption("Cool~one.", 7);
                            data.AddOption("Handsome~one.", 8);
                            data.AddAvatar(74);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Thank~you~very~much.~Which~hairstyle~would~you~like~to~select~from?");
                            data.AddOption("Lovely~one.", 4);
                            data.AddOption("Comely~one.", 5);
                            data.AddOption("Pretty~one.", 6);
                            data.AddOption("Elegant~one.", 7);
                            data.AddOption("Nifty~one.", 8);
                            data.AddAvatar(74);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            if (!client.Inventory.Remove(723087, 1, stream)) break;
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 21);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            if (!client.Inventory.Remove(723087, 1, stream)) break;
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 22);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            if (!client.Inventory.Remove(723087, 1, stream)) break;
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 23);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            if (!client.Inventory.Remove(723087, 1, stream)) break;
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 24);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (client.Player.OnRemoveLukyAmulet == true)
                        {
                            if (!client.Inventory.Remove(723087, 1, stream)) break;
                            client.Player.OnRemoveLukyAmulet = false;
                        }
                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + 25);
                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                        data.AddText("Are~you~satisfied~with~your~new~hairsyle?");
                        data.AddOption("Great!~Thank~you.");
                        data.AddOption("I`d~like~to~try~another~one.", 3);
                        data.AddAvatar(74);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
    }
}
