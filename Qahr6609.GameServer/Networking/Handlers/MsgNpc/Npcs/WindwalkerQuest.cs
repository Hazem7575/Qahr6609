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
        [NpcAttribute(NpcID.DukeofHell)]
        public static void DukeofHell(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)20031, client.Player.Class, 3801);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip color=0xEEFFFF00 desc=Duke#of#Hell#was#born#with#the#ability#to#control#fire.#His#birth#was#a#secret,#but#said#a#son#of#the#Flame#Devastator#who#brought#disasters#to#the#land#300#years#ago.");
                        data.AddText("~He#was#formerly#the#right-hand#man#of#the#King#of#Evil,#then#hid#in#the#central#plain#when#the#King#was#defeated.>Duke of Hell</tip>: ");
                        data.AddText("A Windwalker? Just a joke. Your `wise` Elder Power has been a loser.");
                        data.AddText("~So, you think you`re better than the loser? Well, prove yourself to me.");
                        data.AddOption("Go~to~hell!", 2);
                        data.AddAvatar(62).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.Map == 1002)
                        {
                            client.CreateBoxDialog("You have been teleported to the duke of hell city be aware man !");
                            client.Teleport(client.Player.X, client.Player.Y, 44461, Database.Server.ServerMaps[44461].GenerateDynamicID());
                            return;
                        }
                        else
                        {
                            client.CreateBoxDialog("Duke of Hell has appeared! Defeat him to avenge the deaths of your fellows!");
                            client.Teleport(client.Player.X, client.Player.Y, 44461, Database.Server.ServerMaps[44461].GenerateDynamicID());
                            Database.Server.AddMapMonster(stream, client.Map, 452373, 336, 193, 2, 2, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                            //452373
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ElderPower2)]
        public static void ElderPower2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3800);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)20031, client.Player.Class, 3801);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("(The Elder Power sitting there looks like settling to meditation, but you know he will never come back to life.");
                            data.AddText("~Remember, you must kill the Duke of Hell and protect the imperial mausoleum from the crisis. Keep your promise!)");
                            data.AddOption("I~will.", 6);
                            data.AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            goto case 3;
                        }
                        else
                        {
                            data.AddText("<tip color=0xEEFFFF00 desc=The#old#manager#is#valued#for#his#wisdom#and#holds#an#oversight#role#in#training#young#Windwalkers.#He#may#look#serious,#but#actually#cares#for#the#young.");
                            data.AddText(">Elder Power</tip>: ");
                            data.AddText("Humph! If I were not injured earlier, the head of devil would not have a chance to hurt me. gattata1! You came at the right time.");
                            data.AddText("~The head of devil has run into the deep tunnel after being hit by me. (Coughing)... Don`t let the devil escape!");
                            data.AddOption("What~about~you?", 1);
                            data.AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("(Coughing)... Don`t worry about me. Look, I`ll show you how to switch the combat style and help you...");
                        data.AddText("~help you unblock the inner barriers. Promise... You should promise to defend the imperial mausoleum against the devils.");
                        data.AddOption("I~promise.", 2);
                        data.AddAvatar(67).FinalizeDialog();
                        var action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.DrawStory,
                            dwParam = 1010

                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        //client.CreateBoxDialog("You`ve learned how to switch the combat style. Talk to the Elder Power for further instruction.");
                        client.Player.MessageBox("You`ve learned how to switch the combat style. Talk to the Elder Power for further instruction.",
                       new Action<Client.GameClient>(p =>
                       {
                           ElderPower2(client, stream, 3, "", id);
                       }), null);
                        break;
                    }
                case 3:
                    {
                        data.AddText("<tip color=0xEEFFFF00 desc=The#old#manager#is#valued#for#his#wisdom#and#holds#an#oversight#role#in#training#young#Windwalkers.#He#may#look#serious,#but#actually#cares#for#the#young.");
                        data.AddText(">Elder Power</tip>: ");
                        data.AddText("Good. It`s now your duty to defend... defend the honor of... Windwalker school. It may be too heavy for you, but...");
                        data.AddText("~but I`m old, and I can no longer... no longer...  (The Elder spoke more and more quietly until he closed his eyes.)");
                        data.AddOption("No,~no,~my~Elder!", 4);
                        data.AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(client.Player.X, client.Player.Y, 44460);
                        data.AddText("(The Elder Power sitting there looks like settling to meditation, but you know he will never come back to life.");
                        data.AddText("~Remember, you must kill the Duke of Hell and protect the imperial mausoleum from the crisis. Keep your promise!)");
                        data.AddOption("I~will.", 6);
                        data.AddAvatar(67).FinalizeDialog();//show npc 
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(10090, 334, 197, (uint)NpcID.DukeofHell);
                        break;

                    }
            }
        }
        [NpcAttribute(NpcID.SisterYung2)]
        public static void SisterYung2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3799);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3800);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            {
                                data.AddText("Someone must be in danger. Hurry and check whether you can help!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(155).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                data.AddText("<tip color=0xEEFFFF00 desc=Yung#is#your#lovely#fellow#sister#who#is#not#interested#in#kungfu#but#has#a#gift#for#cooking.");
                                data.AddText("#Many#of#your#fellow#brothers#have#great#affection#for#her.>Sister Yung</tip>: ");
                                data.AddText("Wow, my fellow brother! You killed these devils really fast. I know you won`t let me down.");
                                data.AddOption("Thanks.~Did~you~see~the~Elder?", 1);
                                data.AddAvatar(155).FinalizeDialog();
                            }
                        }
                        else
                            goto case 1;
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Teleport(client.Player.X, client.Player.Y, 44457, Database.Server.ServerMaps[10088].GenerateDynamicID());
                        data.AddText("<tip color=0xEEFFFF00 desc=Yung#is#your#lovely#fellow#sister#who#is#not#interested#in#kungfu#but#has#a#gift#for#cooking.");
                        data.AddText("#Many#of#your#fellow#brothers#have#great#affection#for#her.>Sister Yung</tip>: ");
                        data.AddText("Sorry, I don`t know that. It was me who told the Elder that some devils were running toward the imperial mausoleum. What can we do now?");
                        data.AddOption("I`ll~go~find~the~Elder.", 2);
                        data.AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(10088, 128, 36, 0);
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.SisterYung)]
        public static void SisterYung(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderPower, client.Player.Class, 3798);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3799);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Help! Help, my brother! These devils are quite furious. I`m not a match for them. Ow!");
                            data.AddOption("Don`t~worry.~I`m~here.", 1);
                            data.AddAvatar(155).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("My fellow brother, you should be very careful of them.");
                            data.AddOption("I`ll~take~care~of~them.", 2);
                            data.AddAvatar(155).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        data.AddText("My fellow brother, you should be very careful of them.");
                        data.AddOption("I`ll~take~care~of~them.", 2);
                        data.AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(client.Player.X, client.Player.Y, 44456, Database.Server.ServerMaps[10088].GenerateDynamicID());
                        Database.Server.AddMapMonster(stream, client.Map, 657218, 148, 142, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Database.Server.AddMapMonster(stream, client.Map, 657218, 154, 146, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Database.Server.AddMapMonster(stream, client.Map, 657218, 162, 139, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Database.Server.AddMapMonster(stream, client.Map, 657218, 157, 133, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Database.Server.AddMapMonster(stream, client.Map, 657218, 146, 135, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ElderPower)]
        public static void ElderPower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindwalkerGuard, client.Player.Class, 3797);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderPower, client.Player.Class, 3798);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("<tip color=0xEEFFFF00 desc=The#old#manager#is#valued#for#his#wisdom#and#holds#an#oversight#role#in#training#young#Windwalkers.#He#may#look#serious,#but#actually#cares#for#the#young.");
                            data.AddText(">Elder Power</tip>: ");
                            data.AddText("" + client.Player.Name + "? So good to have you here. (Coughing)... The devils disgracefully attacked us when the Chief was not in the camp.");
                            data.AddText("~Humph, we`ll never let them go that easily.");
                            data.AddOption("Are~you~hurt?", 1);
                            data.AddAvatar(67).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("<tip color=0xEEFFFF00 desc=The#old#manager#is#valued#for#his#wisdom#and#holds#an#oversight#role#in#training#young#Windwalkers.#He#may#look#serious,#but#actually#cares#for#the#young.");
                            data.AddText(">Elder Power</tip>: ");
                            data.AddText("TheBest01? So good to have you here. (Coughing)... The devils disgracefully attacked us when the Chief was not in the camp.");
                            data.AddText("~Humph, we`ll never let them go that easily.");
                            data.AddOption("Are~you~hurt?", 1);
                            data.AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {

                        var action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.DrawStory,
                            dwParam = 1009

                        };
                        client.Send(stream.ActionCreate(&action));
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        data.AddText("Yeah, I could have dodged the attacks, but I failed. It doesn`t matter. Listen, our school was endowed with the honor to guard");
                        data.AddText("~the imperial mausoleum for a hundred years. It`s almost finished, and we can`t jinx it. Wait, did you hear that?");
                        data.AddOption("Someone~is~calling~for~help.", 2);
                        data.AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(client.Player.X, client.Player.Y, 44455, Database.Server.ServerMaps[10088].GenerateDynamicID());
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WindwalkerGuard)]
        public static void WindwalkerGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindwalkerGuard, client.Player.Class, 3797);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderPower, client.Player.Class, 3798);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3799);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3800);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)20031, client.Player.Class, 3801);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)20032, client.Player.Class, 3802);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My fellow brother, you`re finally back! The devils suddenly attacked our camp. Many of our fellows");
                        data.AddText("~were... were killed... (sobbing)... Don`t worry about me. Hurry and give others some help!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddOption("I~need~to~check~around~first.", 255);
                            data.AddAvatar(13).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("I`ll~go~now.", 5);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("I`ll~go~now.", 4);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                                {
                                    data.AddOption("I`ll~go~now.", 3);
                                }
                                else
                                {
                                    data.AddOption("I`ll~go~now.", 2);
                                }
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("I`ll~go~now.", 2);
                            else
                                data.AddOption("I`ll~go~now.", 1);
                        }
                        data.AddOption("I~need~to~check~around~first.", 255);
                        data.AddAvatar(13).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.Teleport(359, 312, 44460);
                        client.SendSysMesage("You found the Elder Power sitting in the tomb tunnel. It seems he has been seriously injured.");
                        client.Player.QuestGUI.SendAutoPatcher("You found the Elder Power sitting in the tomb tunnel. It seems he has been seriously injured.", 10090, 350, 285, (uint)MsgNpc.NpcID.ElderPower2);
                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher("There are too many devil claws here. Try your best to break through and find the Elder Power.", 10089, 28, 89, 0);
                        client.SendSysMesage("There are too many devil claws here. Try your best to break through and find the Elder Power.");
                        client.Teleport(134, 93, 10089);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(191, 156, 44456, Database.Server.ServerMaps[10088].GenerateDynamicID());
                        break;
                    }
                case 2:
                    {
                        client.Teleport(191, 156, 44455, Database.Server.ServerMaps[10088].GenerateDynamicID());
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.QuestGUI.SendAutoPatcher("Many of Windwalker fellows were killed, and many of devils who attacked the camp were killed. Head to see the Elder Power.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                            client.Teleport(191, 156, 10088, Database.Server.ServerMaps[10088].GenerateDynamicID());
                        }
                        break;
                    }
            }
        }
    }
}
