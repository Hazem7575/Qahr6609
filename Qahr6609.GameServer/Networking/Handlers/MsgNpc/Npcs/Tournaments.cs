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
        [NpcAttribute(NpcID.TcBoxer)]
        public static void TcBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BirdBoxer)]
        public static void BirdBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PheonixBoxer)]
        public static void PheonixBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ApeBoxer)]
        public static void ApeBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }



                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.DesertBoxer)]
        public static void DesertBoxer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you are level 20 or above, you may level up at the training grounds.it does ")
                            .AddText(" not consume any durability,life and mana. Do you want me to teleport you there?")
                            .AddText("It is free for my guild members, and 1000 silver for others.")
                    .AddOption("Please teleport me there.", 1)
                    .AddOption("Just passing by.", 255)
                    .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money > 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Teleport(226, 218, 1039);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("When you gain experience at the training ground, you can only attack the dummies or stakes which ")
                            .AddText(" level is lower than your.")
                    .AddOption("I see.", 255)
                    .AddAvatar(8).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You do not have 1000 silvers with you.")
                                           .AddOption("oh, sorry", 255)
                                           .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.DicCityMap4)]
        public static void DicCityMap4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                   .AddOption("TwinCity.", 2)
                   //.AddOption("I`m have the DarkHorn.", 20)
                   .AddOption("Let~me~think...", 255)
                   .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(790001, 1))
                        {
                            if (client.Inventory.HaveSpace(8))
                            {
                                MsgTournaments.MsgSchedules.DisCity.RewardDarkHorn(client, stream);
                            }
                            else
                            {
                                data.AddText("Please make 8 more spaces in your inventory")
                                              .AddOption("Let me check.", 255)
                                              .AddAvatar(8).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry you not have DarkHorn.")
                      .AddOption("Ah. ok", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DisCityMap2)]
        public static void DisCityMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello, can you help me? If you kill the required monsters I desire you can win some EXP. You must kill " + MsgTournaments.MsgSchedules.DisCity.KillsMap2Records(client.Player.Class) + " monsters.")
                                .AddText("You have killed " + client.Player.KillersDisCity + " monsters.")
                                .AddOption("I finished", 1)
                           .AddOption("Sorry, I'll be back", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.KillersDisCity >= MsgTournaments.MsgSchedules.DisCity.KillsMap2Records(client.Player.Class))
                        {
                            if (client.Inventory.HaveSpace(2))
                            {
                                client.Inventory.Add(stream, 723087);
                                if (Role.Core.Rate(2, 10))
                                {
                                    if (Role.Core.Rate(1, 2))
                                    {
                                        client.Inventory.Add(stream, 724155, 1);
                                        client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Veneer Pack.");
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 724161, 1);
                                        client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Cutter Pack.");
                                    }
                                }
                                else
                                {
                                    switch (Program.GetRandom.Next(0, 8))
                                    {
                                        case 0:
                                            {
                                                client.Inventory.Add(stream, 724158, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Red Tassel Pack.");
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Inventory.Add(stream, 724137, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Plate Pack.");
                                                break;
                                            }
                                        case 2:
                                            {
                                                client.Inventory.Add(stream, 724140, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Visor Pack.");
                                                break;
                                            }
                                        case 3:
                                            {
                                                client.Inventory.Add(stream, 724164, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Cauterant Pack.");
                                                break;
                                            }
                                        case 4:
                                            {
                                                client.Inventory.Add(stream, 724143, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Breastplate Pack.");
                                                break;
                                            }
                                        case 5:
                                            {
                                                client.Inventory.Add(stream, 724173, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Tri Plume Pack.");
                                                break;
                                            }
                                        case 6:
                                            {
                                                client.Inventory.Add(stream, 724176, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Belt Pack.");
                                                break;
                                            }
                                        case 7:
                                            {
                                                client.Inventory.Add(stream, 724146, 1);
                                                client.SendSysMesage("You passed the 2nd level in the Dis City quest and got the Medium Hat Rope Pack.");
                                                break;
                                            }
                                    }
                                }
                                MsgTournaments.MsgSchedules.DisCity.TeleportToMap3(client);

                            }
                            else
                            {
                                data.AddText("Please make 2 empty space in your inventory to receive the reward for passing the 2nd level.");
                                data.AddOption("OK.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You`ll~pass~if~you~kill~" + MsgTournaments.MsgSchedules.DisCity.KillsMap2Records(client.Player.Class) + "~monsters~in~HellHall.~Hurry~up!~Only~30~persons~can~get~to~HellCloister~with~me.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DisCityMap1)]
        public static void DisCityMap1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(723085, 5))
                        {
                            data.AddText("Well, now that you`ve got 5 Soul Stones, I`ll give you 2 Exp Ball(B), a Cordiality Book(B), an Exp Pill(B) and a Dis City Bless Stone(12H)(B). Then, send you to the next level. Are you ready?");
                            data.AddOption("I`m~ready.", 2);
                            data.AddOption("Leave~Dis~City.", 1);
                            data.AddOption("Wait~a~moment.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I~need~five~SoulStones~in~order~to~help~you.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(723085, 5))
                        {
                            if (!client.Inventory.Remove(723085, 5, stream)) break;

                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 2, stream, true);
                            client.Inventory.Add(stream, 728883, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 720393, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 728888, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            MsgTournaments.MsgSchedules.DisCity.TeleportToMap2(client);
                        }
                        else
                        {
                            data.AddText("I~need~five~SoulStones~in~order~to~help~you.");
                            data.AddOption("I~see.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        client.Teleport(300, 280, 1002);



                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.TwinDisCityMain)]
        public static void TwinDisCityMain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Our ancestors exerted their utmost efforts and defeathed the demons. Since")
                              .AddText(" then the world has been kept in peace for hundreads of years. Now the demons have come back and the world is getting into turbulence again.")
                              .AddText("The Dis City will take place every Monday, Wednesday and Friday at 19:00 (sign up 19:00-19:05, until 20:30), ")
                              .AddOption("Could you tell me more?", 1)
                              .AddOption("I am here to drive away the evils!", 10)
                              //.AddOption("I`m have the DarkHorn.", 20)
                         .AddOption("I'd better leave.", 255).AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("The decisive battle bewtween human and the demon broke out here. The ferocious battle lasted for seven days and nights. Countless heroes lost their")
                           .AddText(" lives. And the justice won. Afterwards we can live in peace for hundreads of years.")
                           .AddOption("And then?", 2).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("UltimatePluto, leader of the demons sworn to take revenge in one thousand  years befote he managed to run away. To prevent the demons coming back, I")
                            .AddText(" have been scouting their land cautiously. Yesterday I found something  unwanted. It seems that UltimatePluto had come round to endanger humans again.")
                            .AddOption("What should we do?", 3).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Fortunately UltimatePluto is still unfledged. He must resort to a Battle Formation for the moments. So i am about to organize an army too infiltrate his")
                            .AddText(" land and destroy his formation before he becomes stronger.")
                            .AddOption("I'd like to know more.", 4).AddOption("Sigh, I'm helpless.", 255).AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("You must kill UltimatePluto rapisly to get Darkhorn so that i can use it to diable the formation. Before you can do that, you must break into HellGate,")
                           .AddText(" enter the HellHall, and fight through the HellCloister. Countless ferocious  demons are watching those strongholds. I'll give you some strategies on")
                           .AddText(" breaking through those fortresses if you like.")
                           .AddOption("I want to know HellGate.", 6)
                           .AddOption("I want to know HellHall.", 7)
                           .AddOption("I want to know HellCloister.", 8)
                           .AddOption("I want to know BattleFormation.", 9)
                           .AddOption("Thanks. I know what to do.", 255)
                           .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("HellGate is shielded from poisonous fog, so you can't approach it. But demons do not fear the gas. They may turn into SoulStones after they die. If")
                            .AddText(" you get 5 stones for me, i'll help you break through the gate. To protect  the unrealted persons, i'll send the others back as soon as the first 60")
                            .AddText(" persons pass through the gate.")
                            .AddOption("Thank you.", 255)
                            .AddOption("I'd like to know more.", 4).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        data.AddText("HellHall is the very spot where the demons swear their oaths of allegiance to UltimatePluto. Everybody must do his best to make a way out. Due to limited")
                            .AddText(" time, i can lead only 30 persons to hellCloister!")
                            .AddOption("Thank you.", 255)
                            .AddOption("I'd like to know more.", 4).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        data.AddText("You will be divided into 2 groups to attack from the left and the right flank of HellCloister. Kill the Wraithas many as you can, because you can't reach")
                            .AddText(" BattleFormation untill the amount of wraith is decreased to a certain level.")
                            .AddOption("Thank you.", 255)
                            .AddOption("I'd like to know more.", 4).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        data.AddText("BattleFormation is protected by Syrens. After they are killed out, UltimatePluto will appear. Make the best effort to kill him, get his Darkhorn")
                            .AddText(" and give it to me. I can disable the BattleFormation with it. Then i'll send you back. But if you fail to do it, we have to retreat and wait for another opportunity")
                            .AddOption("Thank you.", 255)
                            .AddOption("I'd like to know more.", 4).AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(790001, 1))
                        {
                            if (client.Inventory.HaveSpace(8))
                            {
                                MsgTournaments.MsgSchedules.DisCity.RewardDarkHorn(client, stream);
                            }
                            else
                            {
                                data.AddText("Please make 8 more spaces in your inventory")
                                              .AddOption("Let me check.", 255)
                                              .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry you not have DarkHorn.")
                      .AddOption("Ah. ok", 255).AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        client.SendSysMesage("As~long~as~you~reach~level110,~you~can~enter~the~Dis~City.");
                        break;
                    }
                case 10:
                    {
                        if (MsgTournaments.MsgSchedules.DisCity.AllowJoin())
                        {
                            if (client.Player.Level >= 110)
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Kill~the~demons~to~gather~five~SoulStones.~Then~go~northeast~through~HellBridge,~and~look~for~me~at~HellGate.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(67);
                                    data.FinalizeDialog();



                                    MsgTournaments.MsgSchedules.DisCity.TeleportMap1(stream, client);
                                }
                                else
                                    client.CreateBoxDialog("Please make 1 more space in your inventory.");

                            }
                            else
                            {
                                data.AddText("You`re~not~strong~enough~for~the~moment.~Don`t~lose~your~life~in~vain.");
                                data.AddOption("I~see.", 11);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please sign up between 19:00 and 19:05 every Monday, Wednesday and Friday.")
 .AddOption("I see. ", 255)
 .AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.DisCityMain)]
        public static void DisCityMain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneApe, client.Player.Class, 1312);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DisCityMain, client.Player.Class, 1313);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I have been questing the meaning of life for decades.")
                              .AddOption("Quest:~The~Stone~Ape(1).", 40)
                              .AddOption("Have~you~found~anything?", 41)
                              .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I have been questing the meaning of life for decades.")
                            .AddOption("Have~you~found~anything?", 41)
                            .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("I have been questing the meaning of life for decades.");
                        data.AddOption("Have~you~found~anything?");
                        data.AddAvatar(67);
                        data.FinalizeDialog();




                        break;
                    }
                case 40:
                    {
                        data.AddText("The Stone Ape was born from the rock used to restore cracks in the sky. He never know about it. He gets potentials. If guided properly, he can be a great immortal!")
                               .AddOption("Thank~you~for~the~information!", 42)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 42:
                    {
                        data.AddText("I have a Divine Calix. Get some water from the fountain in the Maple Forest. It will help the Stone Ape to unleash his potential!")
                              .AddOption("Okay!", 43)
                              .AddOption("See~you~then!", 255)
                              .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 43:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 711354);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.SendSysMesage("You received 238,000 points of EXP, 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                client.IncreaseExperience(stream, 238000, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.Money += 500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            }
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                          .AddOption("Let me check.", 255)
                                          .AddAvatar(67).FinalizeDialog();
                        }


                        break;
                    }
                case 41:
                    {
                        data.AddText("Our ancestors exerted their utmost efforts and defeathed the demons. Since")
                         .AddText(" then the world has been kept in peace for hundreads of years. Now the demons have come back and the world is getting into turbulence again.")
                         .AddText("The Dis City will take place every Monday, Wednesday and Friday at 19:00 (sign up 19:00-19:05, until 20:30).")
                         .AddOption("Could you tell me more?", 1)
                         .AddOption("I am here to drive away the evils!", 10)
                         .AddOption("I`m have the DarkHorn.", 20)
                    .AddOption("I'd better leave.", 255).AddAvatar(67).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PkWarQuit)]
        public static void PkWarQuit(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello , you're the last player alive?")
                            .AddOption("Yes", 1)
                            .AddOption("Teleport~to~Twin~City.", 2)
                       .AddOption("Ah. wait", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.PkWar.IsFinished())
                        {
                            if (MsgTournaments.MsgSchedules.PkWar.TheLastPlayer())
                            {
                                if (client.Inventory.HaveSpace(2))
                                {
                                    MsgTournaments.MsgSchedules.PkWar.GiveReward(client, stream);
                                }
                                else
                                {
                                    data.AddText("Please make 2 more spaces in your inventory")
                                               .AddOption("Let me check.", 255)
                                               .AddAvatar(8).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry, the war is not finished, or other players are still alive.")
               .AddOption("Ah ok", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, the war is not finished")
                .AddOption("Ah ok", 255).AddAvatar(154).FinalizeDialog();
                        }




                        break;

                    }
                case 2:
                    {
                        data.AddText("You wana to teleport back to TwinCity? You are sure?")
           .AddOption("Yes", 3).AddOption("Ah no.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(302, 169, 1002, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PkWar)]
        public static void PkWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there! The Weekly PK War takes place every Sunday at 20:00 and the Monthly PK War on the 1st of every month at 20:00,")
                                                   .AddText("and you will be admitted into tournament Come! You will be impressed.")
                                              .AddOption("Sign~me~up", 1)
                                              .AddOption("I will come later. ", 255)
                                              .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.PkWar.AllowJoin())
                        {
                            client.Teleport(87, 88, 1508);
                        }
                        else
                        {
                            data.AddText("Please sign up between 20:00 and 20:19 (Weekly PK War: Sundays, Monthly PK War: the 1st of every month).")
            .AddOption("I see. ", 255)
            .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.SteedRace)]
        public static void SteedRace(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey there " + client.Player.Name + "! The race is held from Monday to Friday at 15:00 (sign up 14:55-15:25). Are you interested in joining the race? You might receive amazing gifts upon reaching the finish line!")

                   .AddOption("Sign~me~up", 1)
                   .AddOption("Open the Race Point shop.", 2)// Horse Racing Points shop (dialog 464, shop 6001)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Steed))
                        {
                            data.AddText("You need a horse first.")
             .AddOption("I see. ", 255)
             .AddAvatar(63).FinalizeDialog();
                            break;
                        } if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Riding))
                        {
                            data.AddText("You need to learn the ride spell first!")
             .AddOption("I see. ", 255)
             .AddAvatar(63).FinalizeDialog();
                            break;
                        }
                        if (Game.MsgTournaments.MsgSchedules.SteedRace.CanJoin)
                        {
                            Game.MsgTournaments.MsgSchedules.SteedRace.Join(client, stream);
                        }
                        else
                        {
                            data.AddText("Please sign up between 14:55 and 15:25, Monday to Friday.")
             .AddOption("I see. ", 255)
             .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        //464 
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = 464,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.TeamDeathMatch)]
        public static void TeamDeathMatch(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the TeamDeathMatch tournament? please allow me to introduce this exciting event for you.")
                       .AddText("It`s a new Tournament that takes place every day at 8:30 AM, 16:00 and 20:30. The tournament contain four team's , the team who earn the most kills won the tournament. Prepare !")
                  .AddOption("Sign~me~up", 1)
                  .AddOption("I will come later. ", 255)
                  .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.TeamDeathMatch)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
         .AddOption("I see. ", 255)
         .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
      .AddOption("I see. ", 255)
      .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DragonWar)]
        public static void DragonWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the DragonWar tournament? please allow me to introduce this exciting event for you.")
                       .AddText("It`s a new Tournament that takes place every day! \n")
                       .AddText("In dragon war you may only use FB/SS/ViperFang. One Player will become the ")
                       .AddText("DragonKing causing them to gain points per secound. 1 shot enemies and gain ")
                       .AddText("points per kill. The dragon king will die in 3hits and the person who slays ")
                       .AddText("him will become the new DragonKing! The event ends when a player reaches 300 points.")
                  .AddOption("Sign~me~up", 1)
                  .AddOption("I will come later. ", 255)
                  .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
.AddOption("I see. ", 255)
.AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
.AddOption("I see. ", 255)
.AddAvatar(63).FinalizeDialog();
                        }


                    }
                    break;

            }



        }


        [NpcAttribute(NpcID.KillerOfElite)]
        public static void KillerOfElite(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the KillerOfElite tournament? please allow me to introduce this exciting event for you.")
                        .AddText("It`s a Pk Tournament that takes place every day . The player who make the best score,he has won the tournament.")
                        .AddText("When you kill others players, you receive points.")
                   .AddOption("Sign~me~up", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.KillerOfElite)
                        {

                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
          .AddOption("I see. ", 255)
          .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
      .AddOption("I see. ", 255)
      .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExtremePk)]
        public static void ExtremePk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the ExtremePk tournament? please allow me to introduce this exciting event for you.")
                        .AddText("It`s a Pk Tournament that takes place every day. The player who make the best score,he has won the tournament.")
                        .AddText("When you kill others players, you receive points.")
                   .AddOption("Sign~me~up", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.ExtremePk)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
         .AddOption("I see. ", 255)
         .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
    .AddOption("I see. ", 255)
    .AddAvatar(63).FinalizeDialog();
                        }




                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LastManStand)]
        public static void LastManStand(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the LastManStand tournament? please allow me to introduce this exciting event for you.")
                        .AddText("It`s a Pk Tournament that takes place every day !")
                   .AddOption("Sign~me~up", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.LastManStand)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
            .AddOption("I see. ", 255)
            .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
 .AddOption("I see. ", 255)
 .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureThief)]
        public static void ThreasureThief(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I have lost all my treasure boxes inside the Frozen Groto map when I was doing the quest! now you'll need to find them for me ever day.")
                   .AddOption("Sign~me~up", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.TreasureThief)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
            .AddOption("I see. ", 255)
            .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
           .AddOption("I see. ", 255)
           .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DBShower)]
        public static void DBShower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the DBShower tournament? please allow me to introduce this exciting event for you.")
                        .AddText("It`s a new Tournament that takes place every day at 8:00 AM and 13:30. You can not use pk mode there, You play just to collect the dragonballs.")
                   .AddOption("Sign~me~up", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DBShower)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
        .AddOption("I see. ", 255)
        .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
        .AddOption("I see. ", 255)
        .AddAvatar(63).FinalizeDialog();
                        }



                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GodlyArtisan)]
        public static void GodlyArtisan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, I`m proficient in repairing max Durability. What items do you wana me to repair?")
                   .AddOption("Repair the Talisman", 20)
                   .AddOption("Hat,Earring", 1)
                   .AddOption("Necklace", 2)
                   .AddOption("Coat", 3)
                   .AddOption("Weapon", 4)
                   .AddOption("Ring/Heavy Ring/Bracelet", 6)
                   .AddOption("Boots", 8)
                   .AddOption("Riding Crop", 18)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 20:
                    {
                        data.AddText("What Talisman do you want me to repair?")
                  .AddOption("Attack Talisman", 11)
                  .AddOption("Defense Talisman", 12)
                  .AddOption("I will come later. ", 255)
                  .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        MsgServer.MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)Option, out GameItem))
                        {
                            if (GameItem.Durability < GameItem.MaximDurability)
                            {
                                data.AddText("Sorry, please ask a shopkeeper to repair this item first. I only repair the max durability.")
         .AddOption("I see. ", 255)
         .AddAvatar(63).FinalizeDialog();
                                break;
                            }

                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(GameItem.ITEM_ID, out DBItem))
                            {
                                if (GameItem.MaximDurability == DBItem.Durability)
                                {
                                    data.AddText("The max durability of this item is in good condition")
           .AddOption("I see. ", 255)
           .AddAvatar(63).FinalizeDialog();
                                    break;
                                }

                                GameItem.Durability = GameItem.MaximDurability = DBItem.Durability;
                                GameItem.Mode = Role.Flags.ItemMode.Update;
                                GameItem.Send(client, stream);

                                data.AddText("You durability was repaired successfully.")
       .AddOption("Thank you.", 255)
       .AddAvatar(63).FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Sorry,you do not wear this item. Please equip first.")
            .AddOption("I see. ", 255)
            .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.EquipmentBlacksmith)]
        public static void EquipmentBlacksmith(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Greetings! If you want to enchance you equipment, you came to the right place!")
                           .AddText("I`m the expert at this type of work. I can help you upgrade you equipment level,")
                           .AddText("Improve the quality, or make repairs! Now, tell me what you wana me to do.")

                   .AddOption("Lower Equipment Level.", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = MsgServer.DialogCommands.DegradeEquip,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BattleFieldMain)]
        public static void BattleFieldMain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The battlefield takes place every day. If you wana to prove youself in the battlefield, come to see me at that time.")
                   .AddOption("Top Hero.", 1)
                   .AddOption("Sign up.", 2)
                   .AddOption("About this. ", 3)
                   .AddOption("Let~me~think. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("The First 3 players its: \n");
                        var tournament = (Game.MsgTournaments.MsgSchedules.Tournaments[MsgTournaments.TournamentType.BattleField] as Game.MsgTournaments.MsgBattleField);
                        int count = 1;
                        foreach (var user in tournament.Rank3)
                        {
                            data.AddText(" Rank " + count.ToString() + " is " + user.Name + " with " + user.BattlePoints.ToString() + " Points. \n");
                            count++;
                        }
                        data.AddOption("Okay.", 255)
                .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.BattleField)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
         .AddOption("Okay.", 255)
         .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
 .AddOption("Okay.", 255)
 .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("The battlefield contain 3 maps, get 10 points in the first folder on Monster, 20 points in 2nd map and 30 on last map. Your points change in cps, when event is finished, or you closed event.")
                    .AddOption("Thank you.. ", 255)
                    .AddAvatar(63).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BattleFieldMap1)]
        public static void BattleFieldMap1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You have " + client.Player.BattleFieldPoints.ToString() + " BattleFieldPoints. You need 2.000 Points for join in the map 2.")
                    .AddOption("Send me there.", 1)
                    .AddOption("Send me in Twin City.", 2)
                    .AddOption("Let~me~think. ", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.BattleFieldPoints >= 2000)
                        {
                            client.Player.BattleFieldPoints -= 2000;
                            client.Teleport(195, 208, 1080);
                        }
                        else
                        {
                            data.AddText("You not have 2.000 Points.")
                    .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("You received " + (client.Player.BattleFieldPoints / 5).ToString() + " ConquerPoints.");
                        client.Player.ConquerPoints += (uint)(client.Player.BattleFieldPoints / 5);
                        client.Player.BattleFieldPoints = 0;

                        client.Teleport(300, 278, 1002);


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BattleFieldMap2)]
        public static void BattleFieldMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You have " + client.Player.BattleFieldPoints.ToString() + " BattleFieldPoints. You need 3.500 Points for join in the last map.")
                    .AddOption("Send me there.", 1)
                    .AddOption("Send me in Twin City.", 2)
                    .AddOption("Let~me~think. ", 255)
                    .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.BattleFieldPoints >= 3500)
                        {
                            client.Player.BattleFieldPoints -= 3500;
                            client.Teleport(183, 156, 2060);//1081
                        }
                        else
                        {
                            data.AddText("You not have 3.500 Points.")
                    .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("You received " + (client.Player.BattleFieldPoints / 5).ToString() + " ConquerPoints.");
                        client.Player.ConquerPoints += (uint)(client.Player.BattleFieldPoints / 5);
                        client.Player.BattleFieldPoints = 0;

                        client.Teleport(300, 278, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BattleFieldMap3)]
        public static void BattleFieldMap3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You have " + client.Player.BattleFieldPoints.ToString() + " BattleFieldPoints.")
                    .AddOption("Send me in Twin City.", 2)
                    .AddOption("Let~me~think. ", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("You received " + (client.Player.BattleFieldPoints / 5).ToString() + " ConquerPoints.");
                        client.Player.ConquerPoints += (uint)(client.Player.BattleFieldPoints / 5);
                        client.Player.BattleFieldPoints = 0;

                        client.Teleport(300, 278, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TeratoTwin)]
        public static void TeratoTwin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Want to open TeratoDragon? You need DragonPill and five meteors.")
                    .AddOption("Yes i wana open that.", 1)
                    .AddOption("Let~me~think. ", 255)
                    .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 5))
                        {
                            if (client.Inventory.Contain(720598, 1))
                            {
                                if (!client.Map.ContainMobID(20060))
                                {

                                    if (!client.Inventory.Remove(720598, 1, stream)) break;
                                    if (!client.Inventory.Remove(Database.ItemType.Meteor, 5, stream)) break;

                                    Database.Server.AddMapMonster(stream, client.Map, 20060, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");
                            }
                            else
                            {
                                data.AddText("Sorry you not have 1 dragonpill.")
                 .AddOption("Ah ok. ", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry you not have 5 meteors.")
                   .AddOption("Ah ok. ", 255)
                   .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GWJail)]
        public static void GWJail(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Welcome~in~GuildWar,~GuildArea~jail.~I~am~giving~jail~pardons~each~five~minutes.")
                    .AddOption("huuuh??", 2)
                    .AddOption("Lets~rock! ", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (DateTime.Now.Minute >= 20 && DateTime.Now.Minute <= 25 || DateTime.Now.Minute >= 0 && DateTime.Now.Minute <= 5
                            || DateTime.Now.Minute >= 40 && DateTime.Now.Minute <= 45)
                        {
                            data.AddText("It~is~time~for~GuildWar~pardon~I~will~let~you~get~out~of~here.~Enjoy.")
                                .AddOption("Cool~thx.", 3)
                                .AddOption("I~wanna~stay.", 255)
                                .AddAvatar(63).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Rest~for~a~while~get~a~cup~of~tea~and~sit.You exit to 00-05 , 20-25 or  40-45.")
                                                       .AddOption("Okeyz", 255)
                                                       .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        client.Teleport(300, 278, 1002, 0, true, true);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PrizeOfficer)]
        public static void PrizeOfficer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Please wait , until the system will check the donation !")
                    .AddOption("Ok, i`m waiting.", 1)
                    .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                 .AddOption("Let me check.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            break;
                        }


                        break;
                    }

            }
        }
    }
}
