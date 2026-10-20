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
        [NpcAttribute(NpcID.HellConquer_EU)]
        public static void HellConquer_EU(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ServerInfo = Database.GroupServerList.GetServer(1);
            if (ServerInfo == null)
                return;

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the keeper of [" + ServerInfo.Name + "] server. You`ll find a keeper like me in each server.")
                            .AddText("~If you want to visit another server, you need to register with me. I can also help you")
                            .AddText("~return to your original server. Please carefully learn about the rules before taking the trip.");
                        data.AddOption("Take~me~to~server~[" + ServerInfo.Name + "].", 1)
                            .AddOption("How~to~travel~across~server?", 10)
                            .AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.ServerID == ServerInfo.ID)
                        {
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Send(stream.ConnectionInfoCreate(1, ServerInfo.ID));
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("If you want to go to another server, talk to the Cross-server Keeper in your server to enter the Realm,")
                            .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm. If return,")
                            .AddText("~you`ll take the same procedures. First, go transit in the Realm. Then, find the Keeper of your original server")
                            .AddText("~to get back. Please make sure you`ve learned all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server, however,")
                             .AddText("~only chat and PK functions are available now. The chat channels include Talk, Whisper, Team, Guild,")
                             .AddText("~Clan, Friend, Service, World and Ally. If you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~are~unavailable?", 12)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions related to money, item trade, forging,")
                             .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP")
                             .AddText("~matches. Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HellConquer_US)]
        public static void HellConquer_US(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ServerInfo = Database.GroupServerList.GetServer(2);
            if (ServerInfo == null)
                return;

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the keeper of [" + ServerInfo.Name + "] server. You`ll find a keeper like me in each server.")
                            .AddText("~If you want to visit another server, you need to register with me. I can also help you")
                            .AddText("~return to your original server. Please carefully learn about the rules before taking the trip.");
                        data.AddOption("Take~me~to~server~[" + ServerInfo.Name + "].", 1)
                            .AddOption("How~to~travel~across~server?", 10)
                            .AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.ServerID == ServerInfo.ID)
                        {
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Send(stream.ConnectionInfoCreate(1, ServerInfo.ID));
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("If you want to go to another server, talk to the Cross-server Keeper in your server to enter the Realm,")
                            .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm. If return,")
                            .AddText("~you`ll take the same procedures. First, go transit in the Realm. Then, find the Keeper of your original server")
                            .AddText("~to get back. Please make sure you`ve learned all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server, however,")
                             .AddText("~only chat and PK functions are available now. The chat channels include Talk, Whisper, Team, Guild,")
                             .AddText("~Clan, Friend, Service, World and Ally. If you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~are~unavailable?", 12)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions related to money, item trade, forging,")
                             .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP")
                             .AddText("~matches. Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OurCoqnuer_EU)]
        public static void OurCoqnuer_EU(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ServerInfo = Database.GroupServerList.GetServer(3);
            if (ServerInfo == null)
                return;

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the keeper of [" + ServerInfo.Name + "] server. You`ll find a keeper like me in each server.")
                            .AddText("~If you want to visit another server, you need to register with me. I can also help you")
                            .AddText("~return to your original server. Please carefully learn about the rules before taking the trip.");
                        data.AddOption("Take~me~to~server~[" + ServerInfo.Name + "].", 1)
                            .AddOption("How~to~travel~across~server?", 10)
                            .AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.ServerID == ServerInfo.ID)
                        {
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Send(stream.ConnectionInfoCreate(1, ServerInfo.ID));
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("If you want to go to another server, talk to the Cross-server Keeper in your server to enter the Realm,")
                            .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm. If return,")
                            .AddText("~you`ll take the same procedures. First, go transit in the Realm. Then, find the Keeper of your original server")
                            .AddText("~to get back. Please make sure you`ve learned all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server, however,")
                             .AddText("~only chat and PK functions are available now. The chat channels include Talk, Whisper, Team, Guild,")
                             .AddText("~Clan, Friend, Service, World and Ally. If you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~are~unavailable?", 12)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions related to money, item trade, forging,")
                             .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP")
                             .AddText("~matches. Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OurCoqnuer_US)]
        public static void OurCoqnuer_US(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ServerInfo = Database.GroupServerList.GetServer(4);
            if (ServerInfo == null)
                return;

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the keeper of [" + ServerInfo.Name + "] server. You`ll find a keeper like me in each server.")
                            .AddText("~If you want to visit another server, you need to register with me. I can also help you")
                            .AddText("~return to your original server. Please carefully learn about the rules before taking the trip.");
                        data.AddOption("Take~me~to~server~[" + ServerInfo.Name + "].", 1)
                            .AddOption("How~to~travel~across~server?", 10)
                            .AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.ServerID == ServerInfo.ID)
                        {
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Send(stream.ConnectionInfoCreate(1, ServerInfo.ID));
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("If you want to go to another server, talk to the Cross-server Keeper in your server to enter the Realm,")
                            .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm. If return,")
                            .AddText("~you`ll take the same procedures. First, go transit in the Realm. Then, find the Keeper of your original server")
                            .AddText("~to get back. Please make sure you`ve learned all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server, however,")
                             .AddText("~only chat and PK functions are available now. The chat channels include Talk, Whisper, Team, Guild,")
                             .AddText("~Clan, Friend, Service, World and Ally. If you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~are~unavailable?", 12)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions related to money, item trade, forging,")
                             .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP")
                             .AddText("~matches. Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RealmEnvoy)]
        public static void RealmEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.ServerID == Database.GroupServerList.MyServerInfo.ID)
                        {
                            data.AddText("I`m serving the Realm. After you travel to other servers, you can find me to return to the Realm.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(43).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            if (client.Player.OnTransform)
                            {
                                client.Player.TransformInfo.FinishTransform();
                            }
                            data.AddText("I have the code to enter the Realm. If you want to return to the Realm, i can help you.");
                            data.AddOption("Return to the Realm.", 1)
                                .AddOption("I see.", 255)
                               .AddAvatar(43).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        client.Send(stream.ConnectionInfoCreate(1, Database.GroupServerList.InterServer.ID));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KingdomMissionEnvoy)]
        public static void KingdomMissionEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35007);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Crystal, client.Player.Class, 35028);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35025);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35034);


            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.OnMyOwnServer == false)
                        {
                            data.AddText("I have the code to enter the Realm. If you want to return to the Realm, i can help you.");
                            data.AddOption("Return to the Realm.", 2)
                                .AddOption("I see.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level >= 110 || client.Player.Reborn >= 1)
                        {
                            data.AddText("A real hero must harbor great ambition for his kingdom.")
                                .AddText("~In the name of conquer, I`m calling heroes who`ve reached Level 110 of the 1st rebirth.")
                                .AddText("~for a series of cross-server missions. Go to the Realm, and you`ll find a way to explore")
                                .AddText("~a new world out of this world.")
                                .AddText("\nYou`ve earned " + client.Player.KingDomDeeds.ToString() + " points of Kingdom Deeds.");
                            data.AddOption("Enter~the~Realm.", 1)
                                .AddOption("Kingdom~Mission.", 10)
                                .AddOption("Submit~Gold~Bricks.~(special)", 20)
                                .AddOption("Swap~deeds~for~rewards.", 30)
                                .AddOption("How~to~travel~across~server?", 40)
                                .AddOption("What~should~I~pay~attention~to?", 50)
                                .AddOption("I~see", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You should reach at Level 110 of the 1st rebirth before you can enter the Realm.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {

                        if (client.Player.InUnion)
                            client.Send(stream.HandBrickInfoCreate(MsgHandBrickInfo.BrickInfo.ShowSubmitDialog, client.Player.MyUnion.GoldBrick, 2));
                        else
                            client.Send(stream.HandBrickInfoCreate(MsgHandBrickInfo.BrickInfo.ShowSubmitDialog, 0, 2));
                        break;
                    }

                case 30:
                    {
                        data.AddText("You can collect up to 300 Kingdom Deeds in a day from the Kingdom Mission. If you`ve earned 50, 150 or 300 points,")
                            .AddText("~you can swap them for corresponding reward with me. So, which pack would you like to claim? \n")
                            .AddText(" Your Kingdom Deeds: " + client.Player.KingDomDeeds + ".");
                        data.AddOption("Outstanding~Exploit~Pack.~(50)", 31)
                            .AddOption("Brilliant~Exploit~Pack.~(150)", 32)
                            .AddOption("Supreme~Exploit~Pack.~(300)", 33)
                            .AddOption("Buy~Kingdom~Deeds.", 34)
                            .AddOption("What`re~in~the~packs?", 37)
                            .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        data.AddText("You can swap 50 Kingdom Deeds for an Outstanding Exploit Pack: 50 War Exploits, 400 Potency Points and 50 points for Union Fund;")
                            .AddText("~150 Kingdom Deeds for a Brilliant Exploit Pack: 50 Champion Points (excluded if maximum is reached), 100 War Exploits, 600 Potency Points and 160 points to Union Fund;")
                            .AddText("~300 Kingdom Deeds for a Supreme Exploit Pack: 100 Champion Points (excluded if maximum is reached), 150 War Exploits, 1000 Potency Points and 360 points to Union Fund.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        data.AddText("So far, you`ve earned " + client.Player.KingDomDeeds + " Kingdom Deeds. If you want it full of 300 points immediately, you need to pay CPs. The costs: \n")
                        .AddText("     0 - 49 Kingdom Deeds: 990 CPs; 50 - 149 Kingdom Deeds: 540 CPs;  150 - 299 Kingdom Deeds: 270 CPs.");
                        data.AddOption("Pay~CPs~to~fill~up~with~deeds.", 35)
                            .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        uint Cost = 990;

                        if (client.Player.KingDomDeeds >= 159)
                            Cost = 270;
                        else if (client.Player.KingDomDeeds >= 50)
                            Cost = 540;

                        if (client.Player.BuyKingdomDeeds != 0)
                        {
                            data.AddText("You already bought one today , please come back tomorrow .");
                            data.AddOption("Okay.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            return;
                        }

                        data.AddText("Are you sure you want to pay " + Cost + " CPs to fill up your Kingdom Deed?");
                        data.AddOption("Yes.", 36)
                            .AddOption("Wait,~I~haven`t~decided.", 255)
                           .AddAvatar(6).FinalizeDialog();



                        break;
                    }
                case 36:
                    {
                        uint Cost = 990;

                        if (client.Player.KingDomDeeds >= 159)
                            Cost = 270;
                        else if (client.Player.KingDomDeeds >= 50)
                            Cost = 540;
                        if (client.Player.ConquerPoints >= Cost)
                        {
                            client.Player.BuyKingdomDeeds = 1;
                            client.Player.ConquerPoints -= Cost;
                            client.Player.KingDomDeeds = 300;
                            client.CreateBoxDialog("You've received 300 KingDom Deeds.");
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have enough CPs to fill up your Kingdom Deed.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 33:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.KingDomDeeds >= 300)
                        {
                            client.Player.KingDomDeeds -= 300;
                            client.Inventory.Add(stream, 3007110);
                            client.CreateBoxDialog("You received a Supreme Exploit Pack.");
                        }
                        else
                        {
                            data.AddText("Sorry, you can`t claim this pack since you don`t have 150 Kingdom Deeds. Go complete the Kingdom Mission to earn more Kingdom Deeds.");
                            data.AddOption("Alright.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 32:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.KingDomDeeds >= 150)
                        {
                            client.Player.KingDomDeeds -= 150;
                            client.Inventory.Add(stream, 3007109);
                            client.CreateBoxDialog("You received a Brilliant Exploit Pack.");
                        }
                        else
                        {
                            data.AddText("Sorry, you can`t claim this pack since you don`t have 150 Kingdom Deeds. Go complete the Kingdom Mission to earn more Kingdom Deeds.");
                            data.AddOption("Alright.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.KingDomDeeds >= 50)
                        {
                            client.Player.KingDomDeeds -= 50;
                            client.Inventory.Add(stream, 3007108);
                            client.CreateBoxDialog("You received a Outstanding Exploit Pack.");
                        }
                        else
                        {
                            data.AddText("Sorry, you can`t claim this pack since you don`t have 50 Kingdom Deeds. Go complete the Kingdom Mission to earn more Kingdom Deeds.");
                            data.AddOption("Alright.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("A real hero must harbor great ambition for his kingdom. In the name of Conquer, I`m calling heroes who`ve reached Level 110 of the 1st rebirth.")
                            .AddText("~for a series of cross-server missions. Go abroad, and explore a new world out of this world.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Beasts~of~Legend(completed)", 90);
                        else
                            data.AddOption("[Realm]~Beasts~of~Legend.", 90);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Thunder~Strike(completed)", 60);
                        else
                            data.AddOption("[Realm]~Thunder~Strike.", 60);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ashes~of~Anger(completed)", 11);
                        else
                            data.AddOption("[Realm]~Ashes~of~Anger.", 11);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Scramble~for~Justice(completed)", 80);
                        else
                            data.AddOption("[Realm]~Scramble~for~Justice.", 80);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Crystal~Bounty(completed)", 70);
                        else
                            data.AddOption("[Realm]~Crystal~Bounty.", 70);
                        data.AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("A horde of bloodthirsty devils have occupied the Deserted Village in the Realm. They built a weird array with 4 Dark Crystals,")
                        .AddText("~guarding their leader Alluring Witch at the center. We`re calling heroes to team up and behead the horrific Alluring Witch")
                    .AddText("~after breaking through her magic array. Tons of rewards will be offered to the contributors.");
                        data.AddOption("Count~on~me!", 255)
                          .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("The Realm has received countless threats, but this time seems different. The enemies are so manic that we`re")
                         .AddText("~in the most urgent need of helpers. After killing a certain number of enemies, you can claim a starry")
                     .AddText("<tip color=0xfff05050 desc=A#chance#to#get#Dragon#Ball#Piece,#Savage#Bone,#Chi#Points,#Frozen#Chi#Pill,#etc.,#once#a#day.>bounty</tip> from the Realm Crystal.");
                        data.AddOption("Count~on~me!", 255)
                          .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 90:
                    {
                        data.AddText("Unbelievable! The four mythological creatures make a rare appearance together in the Realm.")
                           .AddText("~Right now, go defeat 3 of their defenders, and you can challenge these divine beasts who carry")
                       .AddText("<tip color=0xfff05050 desc=Eminent#Exploit#Pack,#Protection#Pill,#Fierce#Spirit,#200-min#EXP,#30#Kingdom#Deeds,#50#Champion#Points#and#500#Horse#Racing#Points.> invaluable treasures</tip>.");
                        data.AddOption("Claim~my~rewards.", 91)
                            .AddOption("I`m~not~ready~yet!", 255)
                          .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 91:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already claimed the reward for the [Beasts of Legend] mission in the Realm.");
                            data.AddOption("Sorry,~I~forgot.", 255)
                               .AddAvatar(6).FinalizeDialog();

                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            == false && client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest4.MissionId, 1))
                        {
                            client.Inventory.Add(stream, 3200005);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId);
                            data.AddText("Your loyalty to your kingdom is quite strong. Please accept this reward.");
                            data.AddOption("Thanks!", 255)
                               .AddAvatar(6).FinalizeDialog();
                            client.SendSysMesage("You've received a bounty of (Divine Conquest Pack) for conquering the divine beast! ", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("To claim the reward, you should conquer the divine beast first.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 60:
                    {
                        data.AddText("While in a battle against millions of enemies, striking first is the key to gain initiative. Hurry up and disable the enemy soldiers, officers, and their Kylin Chariots in the Realm!")
                             .AddText("~When you collect 300 Strike Points, a wonderful<tip color=0xfff05050 desc=Eminent#Exploit#Pack,#Free#Training#Pill,#or#other#rewards#like#Chi#Points#&#300-min#EXP,#90#Kingdom#Deeds,#and#50#Champion#Points.>bounty</tip>")
                         .AddText("~will be yours. You can also earn War Exploits by destroying the enemy`s Defensive Barrier, Dark Crystal, or beheading their Military Officer.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddOption("Learn~about~other~things.", 10).AddOption("I`m~not~ready~yet!", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        data.AddOption("Claim~my~bounty.", 61)
                            .AddOption("Directly~complete~it.~(500~CPs)", 62)
                            .AddOption("Learn~about~other~things.", 10)
                            .AddOption("I`m~not~ready~yet!", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 62:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            break;
                        }
                        if (client.Player.ConquerPoints >= 500)
                        {
                            client.Player.ConquerPoints -= 500;
                            MsgQuestList.QuestListItem _quest;
                            client.Player.QuestGUI.AcceptKingDomMission(ActiveQuest2, 0, out _quest);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.Inventory.Add(stream, 3200000);
                            data.AddText("Your loyalty to your kingdom is quite strong. Please accept this reward.");
                            data.AddOption("Thanks!", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("Okay.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;

                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            == false && client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest2.MissionId, 300))
                        {
                            client.Inventory.Add(stream, 3200000);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            data.AddText("Your loyalty to your kingdom is quite strong. Please accept this reward.");
                            data.AddOption("Thanks!", 255)
                               .AddAvatar(6).FinalizeDialog();
                            client.SendSysMesage("You've completed the mission [Thunder~Strike]. Hurry and check your reward in the inventory.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("To claim a reward, you should collect 300 Strike Points, first. Fight for your home, and kill enemies in the Realm!");
                            data.AddOption("Go~fight!", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        data.AddText("In the Realm, the bloodthirsty devils reside in the Deserted Village and start killing straggling soldiers.")
                               .AddText("~Their cruelty cannot be forgiven. You`re asked to ignite the Explosive Device and burn the Deserted")
                           .AddText("~Village into ashes. If succeed, you can claim a valuable reward.");
                        data.AddOption("Claim~my~rewards.", 12)
                            .AddOption("Count~on~me!", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already claimed the reward for the [Ashes of Anger] mission in the Realm.");
                            data.AddOption("Sorry,~I~forgot.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            == false && client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1))
                        {
                            client.Inventory.Add(stream, 3200004);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            client.SendSysMesage("You've completed the mission [Realm~Ashes of Anger]. Hurry and check your reward in the inventory.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("You haven`t destroyed Deserted Village. Please finish your job, first.");
                            data.AddOption("Okay.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Send(stream.ConnectionInfoCreate(1, Database.GroupServerList.InterServer.ID));
                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        client.SendSysMesage("Pay attention you won`t be able to attack the players in other servers if you travel there bevond Plumber War(Tuesday, Wednesdat, Thursday, Saturday and Sunday)!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        MsgInterServer.PipeClient.Connect(client, Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port);



                        break;
                    }
                case 40:
                    {
                        data.AddText("To travel to another server, you need to find me to enter the Realm first.")
                               .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm.")
                               .AddText("~It`s the same when you return. First, go transit in the Realm, and then find the Keeper of your server.")
                               .AddText("~Please learn all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 41)
                            .AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                                 .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server. During the Plunder War, you can travel to other servers through the Realm.")
                                 .AddText("~only chat and PK are available now. The chat channels include Talk, Whisper, Team, Guild, Clan, Friend, Service, World and Ally,")
                                 .AddText("~Please note that, if you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~will~be~unavailable?", 42)
                            .AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions concerning money, item trade, forging,")
                                   .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP matches.")
                                   .AddText("~Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server. During the Plunder War, you can travel to other servers through the Realm.")
                             .AddText("~only chat and PK are available now. The chat channels include Talk, Whisper, Team, Guild, Clan, Friend, Service, World and Ally,")
                             .AddText("~Please note that, if you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~will~be~unavailable?", 51)
                            .AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions concerning money, item trade, forging,")
                                   .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP matches.")
                                   .AddText("~Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PokerMillionaireLee)]
        public static void PokerMillionaireLee(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
           
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Good choice! You can just pay 100,000 Silver to pack 50,000,000 Silver into a Medium Silver Bag or pack")
                            .AddText("~100,000,000 Silver into a Big Silver Bag.")
                        //data.AddOption("Pack~50,000,000~Silver.", 3)
                        //    .AddOption("Pack~100,000,000~Silver.", 1)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to pay 100,000 Silver to pack 100,000,000 Silver into a Big Silver Bag?");
                        data.AddOption("Yes.", 2)
                            .AddOption("No.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Money >= 100100000)
                        {
                            client.Player.Money -= 100100000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 3005837);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 100,100,000 Silver in total.");
                            data.AddOption("My~bad.", 255)
                               .AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to pay 100,000 Silver to pack 50,000,000 Silver into a Medium Silver Bag?");
                        data.AddOption("Yes.", 4)
                            .AddOption("No.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Money >= 50100000)
                        {
                            client.Player.Money -= 50100000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 3005836);
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have 50,100,000 Silver in total.");
                            data.AddOption("My~bad.", 255)
                               .AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}
