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
        [NpcAttribute(NpcID.PakYan)]
        public static void PakYan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuidofFieryDragon, 0, 3631);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PakYan, 0, 3632);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3649);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Good to see you back! Is everything all right?")
                     .AddOption("I~have~good~news~for~you.", 4).AddAvatar(67)
              .FinalizeDialog();
                            break;
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Are you here to give our Bright Tribe a hand?")
                       .AddOption("Yes, exactly", 1).AddAvatar(67)
                .FinalizeDialog();
                            break;
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                data.AddText("Do you have any news about the Elder and others? I`m really worried about them.")
                   .AddOption("I`ll go and check.", 3).AddAvatar(67)
            .FinalizeDialog();
                                break;
                            }
                            data.AddText("Two days ago, Chong Yan Elder led a group to find the Flame Temple which is not only our ")
                         .AddText("tribe`s holy zone, but also they key to complety activate the Mystery Pearl and stop the ")
                         .AddText("Fiery Dragon Volcano from eruption, However, there is still no message from them.")
                   .AddOption("I'll go find them.", 3)
                   .AddOption("Don`t worry, they`ll be fine.", 255).AddAvatar(67)
            .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("We`ve finally arrived home, and we`ll spare no effort to restore our")
                                .AddText("~homeland to its former glory.")
                .AddOption("Bright~Fortune.~(Daily)", 10).AddAvatar(67)
         .FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.ClaimTowerAmulets == 0)
                        {
                            data.AddText("For our tribe`s helpers, I`ll never be stingy with rewards for them. Under the Fiery Dragon region, there are not only massive magma chambers,")
                              .AddText("~but also mythical treasures. Are you interested? You can claim Hunting Amulets from me every day. Wish you a harvest!")
                              .AddText("    Reward: <tip color=0xfff05050 desc=Bloody#Jade,#Bright#Star#Stone,#Blazing#CP#Fragment,#Champion#Points,#etc.>Treasure of Dragon</tip>")
              .AddOption("Claim~5~Hunting~Amulets.", 11).AddAvatar(67)
       .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("How is your treasure hunting today? It must be a harvest! If you`re still interested, come to me tomorrow.")
                  .AddOption("Great!", 255).AddAvatar(67)
           .FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more spaces in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(67)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ClaimTowerAmulets == 0)
                        {
                            client.Player.ClaimTowerAmulets = 1;
                            client.Inventory.AddItemWitchStack(3008993, 0, 5, stream);
                            client.Teleport(40, 347, 3998);
                            client.CreateBoxDialog("The Chief of Bright Tribe gave you 5 Hunting Amulets which will bring you surprises.");
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("Really? We`ve been waiting hundreds of years for this moment! We can finally worship")
                            .AddText("~our ancestors in the Flame Temple. From now on, you`re the most honored friend to")
                            .AddText("~the Bright Tribe. I don`t know how to thank you enough.")
                    .AddOption("It`s~my~pleasure.", 5).AddAvatar(67)
             .FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("The Elder and others have been rescued when you were on the journey to find the Flame Temple.")
                              .AddText("~They just can`t wait to see the Flame Altar in front of the temple. If the Mystery Pearl can be activated")
                              .AddText("~at the altar, we`ll be able to prevent the Fiery Dragon Volcano from eruption. Would you like to join us?")
                            .AddOption("Sure.", 6).AddAvatar(67)
                            .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 3200344, 3);
                        }
                        var action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.DrawStory,
                            dwParam = 1004

                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
                case 1:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        var action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.DrawStory,
                            dwParam = 1001

                        };
                        client.Send(stream.ActionCreate(&action));
                        data.AddText("For many times, I dreamed of returning home, but forgot almost everything upon waking.It`s saied ")
                            .AddText("the Fiery Dragon was a prosperous land, and there Bright ancestors lived in a golden age. Look at")
                            .AddText(" it now, our homeland has become utterly desolate. It`s really depressing.")
                      .AddOption("We still have hope.", 2).AddAvatar(67)
               .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(67)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.SendSysMesage("You received 3 Twilight Star Stones!", MsgMessage.ChatMode.System);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 3009000, 3);
                        }
                        data.AddText("Two days ago, Chong Yan Elder led a group to find the Flame Temple which is not only our ")
                           .AddText("tribe`s holy zone, but also they key to complety activate the Mystery Pearl and stop the ")
                           .AddText("Fiery Dragon Volcano from eruption, However, there is still no message from them.")
                     .AddOption("I'll go find them.", 3)
                     .AddOption("Don`t worry, they`ll be fine.", 255).AddAvatar(67)
              .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.FlameAltar)]
        public static void FlameAltar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3643);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3644);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3645);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3646);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3647);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3648);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3649);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You`ve finally arrived at relics of the Flame Temple. At the center of the temple square,")
                                .AddText("~there is a ruined pentagonal altar carved with complex patterns at each side.")
              .AddOption("It`s~amazing!", 1)
       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("(Hurry and go collect Building Stones to repair the ruined altar.)")
                   .AddOption("Use~stones~to~repair~altar.", 3)
            .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("(You found two sides of the altar were seriously damaged, and Metal and Fire icons were absent from the elemental patterns.")
                              .AddText("~You should restore the Flame Altar, first. Look around and collect appropriate Building Stones.")
            .AddOption("Go~find~Building~Stones.", 2)
     .FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("(The altar is dead without the Wheel of Nature.")
                                    .AddText("~Hurry and retrieve it from the Clawed Rock Devils.)")
                   .AddOption("Place~the~wheel~on~the~altar.", 10)
            .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("(To further restore the altar, you`ll need to switch on the five elemental forces resting in the Wheel of Nature.")
                          .AddText("~But where is the Wheel of Nature? It must be stolen by the Clawed Rock Devils nearby.")
                          .AddText("~Go retrieve the wheel from the Clawed Rock Devils.)")
                          .AddOption("Go~retrieve~the~wheel.", 6)
                          .FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("(The Metal Force is essential to motivate the Mystery Pearl. With the Metal Force,")
                                    .AddText("~you can light the Metal icon on the altar.)")
                   .AddOption("Spin~the~Wheel~of~Nature.", 13)
            .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("(It`s said the Mystery Pearl contains terrific power which can only be motivated by the purest Metal Force.")
                         .AddText("~Is it true? If you have the Earth Force, please transform it into Metal Force on the Wheel of Nature.)")
                         .AddOption("Go~make~the~Metal~Force.", 12)
                         .FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("(Put the Lava Flower on the Wheel of Nature, and spin the wheel to extract Fire Force.")
                                    .AddText("~When you light the Fire icon with the Fire Force, the Flame Altar will be fully restored and activated.)")
                   .AddOption("Extract~the~Fire~Force.", 20)
            .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("(It`s said the Mystery Pearl contains terrific power which can only be motivated by the purest Metal Force.")
                         .AddText("~Is it true? If you have the Earth Force, please transform it into Metal Force on the Wheel of Nature.)")
                         .AddOption("Go~make~the~Metal~Force.", 15)
                         .FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("(Insert the Essence of Star into the eyes of the altar, and you`ll achieve the final success against the Fiery Dragon Volcano.)")
                   .AddOption("Extract~the~Essence~of~Star.", 23)
            .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("(It requires a large number of Star Ores to extract the Essence of Star.")
                                 .AddText("~The Hellfire Beasts are known to carry such ores.")
                .AddOption("Go~collect~Star~Ores.", 22).FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("(You`ve finished your job here. Hurry and report back to Chief Pak Yan.)")
                .AddOption("Okay.", 25)
         .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("(Everything is ready. Hurry and report back to Chief Pak Yan!)")
                .AddOption("Okay.", 25).FinalizeDialog();
                            }

                        }
                        else
                        {

                            data.AddText("(The Flame Altar is a special place for the Bright tribe to hold grand sacrifice events.)")
                    .AddOption("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 25:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest7.FinishNpcId.Map, ActiveQuest7.FinishNpcId.X, ActiveQuest7.FinishNpcId.Y, ActiveQuest7.FinishNpcId.ID);
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Contain(3008748, 100))
                        {
                            data.AddText("(With the aid of the Essence of Star, the energy of Flame Altar becomes steady.")
            .AddText("~You`ve finished your job. Hurry and report back to Chief Pak Yan. He`ll take care of the rest")
            .AddText("~to suppress the Fiery Dragon Volcano with the Mystery Pearl.)")
                            .AddOption("I`ve~finally~made~it!", 24)
      .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("(The airflow around the altar is getting unstable. You need to insert the Essence of Star into the eyes of the altar quickly.")
                .AddText("~Don`t jinx it. Hurry and go collect Star Ores from the Hellfire Beasts.)")
                                .AddOption("I`m~on~the~way.", 22)
          .FinalizeDialog();
                        }
                        break;
                    }
                case 24:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 3008748, 100) && client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {

                            client.SendSysMesage("You received a Radiant Star Stone!", MsgMessage.ChatMode.System);
                            client.Inventory.RemoveStackItem(3008748, 100, stream);
                            client.Inventory.Add(stream, 3200346);
                            client.Inventory.Add(stream, 3009002);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(You`ve finished your job here. Hurry and report back to Chief Pak Yan.)")
                 .AddOption("Okay.", 25)
          .FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3998, 522, 347, 0);
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 3008747, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            if (!client.Inventory.Remove(3008747, 10, stream)) break;
                            client.Inventory.Add(stream, 3200345);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(It requires a large number of Star Ores to extract the Essence of Star.")
                                .AddText("~The Hellfire Beasts are known to carry such ores.")
               .AddOption("Go~collect~Star~Ores.", 22).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(3008747, 10))
                        {
                            data.AddText("(With the recovery of the Fire icon, the five elemental icons shine more brightly. You`re now very close to")
                             .AddText("~the ultimate success: place the Essence of Star at the eye of the altar to comfort the restless airflow,")
                             .AddText("~and then burn the power of Mystery Pearl to completely freeze the Fiery Dragon Volcano.)")
                             .AddOption("I`ll~try~my~best.", 21)
                             .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("(To fully activated the Flame Altar, you still need the Fire Force which can be extracted")
                                .AddText("~from the flowers blooming by the Lava Pool.)")
                                .AddOption("Head~to~the~Lava~Pool.", 15)
                                .FinalizeDialog();

                        }

                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(3008745, 1))
                        {
                            data.AddText("(Rolling slowly, the Wheel of Nature successfully transformed the Earth Force into Metal Force,")
                                 .AddText("~with the Metal icon shining brightly. Now, you can turn to switch on the Fire Force.)")
                .AddOption("Everything~will~go~well.", 14)
         .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have the Earth Force which can be transformed into the Metal Force through the Wheel of Nature.")
                                   .AddText("~When enough Essences of Earth are collected, you can extract the Earth Force from them.")
                  .AddOption("Go~collect~Essence~of~Earth.", 12)
           .FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3998, 559, 685, 0);
                        break;
                    }
                case 14:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 3008745, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(3008745, 1, stream)) break;
                            client.Inventory.Add(stream, 3200346);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(The Metal element has been recovered, and it`s time to explore the Fire Force.")
                                .AddText("~As the totem of the Bright tribe, Fire is essential to their altar.")
                                .AddText("~Not far away from the Flame Temple, there is a Lava Pool where special flowers are blooming.")
                                .AddText("~You can extract the purest Fire Force from the Lava Flowers.)")
               .AddOption("Head~to~the~Lava~Pool.", 15).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3008743, 1))
                        {
                            data.AddText("(You placed the Wheel of Nature at the center of the altar, and saw the icons of five elements")
                                 .AddText("~giving out faint light. What a good beginning! Before burning the power of Mystery Pearl, you")
                                 .AddText("~need to switch on the five elemental forces through the Wheel of Nature.")
                .AddOption("Good.", 11)
         .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("(The Wheel of Nature is the key to fully restore the altar,")
                                   .AddText("~which is also a must to energize the Mystery Pearl. Hurry and retrieve the wheel from the Clawed Rock Devils.)")
                  .AddOption("I`m~on~the~way.", 6)
           .FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3998, 482, 216, 0);
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 3008743, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(3008743, 1, stream)) break;
                            client.Inventory.Add(stream, 3200346);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(It`s said the Mystery Pearl contains terrific power which can only be motivated by the purest Metal Force.")
                            .AddText("~Is it true? If you have the Earth Force, please transform it into Metal Force on the Wheel of Nature.)")
                            .AddOption("Go~make~the~Metal~Force.", 12)
                            .FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3998, 307, 102, 0);
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(3008742, 50))
                        {
                            data.AddText("(The altar has been roughly restored by using some unique Building Stones.)")
                  .AddOption("I`m~so~tired.", 5)
           .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("(To restore the altar, you need to collect more Building Stones.)")
                     .AddOption("Go~collect~more~stones.", 4)
              .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 3008742, 50) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.RemoveStackItem(3008742, 50, stream);
                            client.Inventory.Add(stream, 3200346);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(To further restore the altar, you`ll need to switch on the five elemental forces resting in the Wheel of Nature.")
                            .AddText("~But where is the Wheel of Nature? It must be stolen by the Clawed Rock Devils nearby.")
                            .AddText("~Go retrieve the wheel from the Clawed Rock Devils.)")
                            .AddOption("Go~retrieve~the~wheel.", 6)
                            .FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3998, 596, 447, 0);
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            var action = new ActionQuery()
                            {
                                ObjId = client.Player.UID,
                                Type = ActionType.DrawStory,
                                dwParam = 1003

                            };
                            client.Send(stream.ActionCreate(&action));

                            client.Inventory.Add(stream, 3200344);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(You found two sides of the altar were seriously damaged, and Metal and Fire icons were absent from the elemental patterns.")
                               .AddText("~You should restore the Flame Altar, first. Look around and collect appropriate Building Stones.")
             .AddOption("Go~find~Building~Stones.", 2)
      .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3998, 596, 447, 0);

                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.LavaFlower1)]
        public static void LavaFlower1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008747, 10))
                {
                    client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.LavaFlower2)]
        public static void LavaFlower2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008747, 10))
                {
                    client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.LavaFlower3)]
        public static void LavaFlower3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008747, 10))
                {
                    client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.LavaFlower4)]
        public static void LavaFlower4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008747, 10))
                {
                    client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.LavaFlower5)]
        public static void LavaFlower5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008747, 10))
                {
                    client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.LavaFlower6)]
        public static void LavaFlower6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008747, 10))
                {
                    client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.LavaFlower7)]
        public static void LavaFlower7(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008747, 10))
                {
                    client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.ChingYan)]
        public static void ChingYan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3640);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3641);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3642);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3643);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Cough... Cough... Who is there?! I`m not afraid of you! I`ll avenge for the sacrificed people!")
              .AddOption("The~Elder~sent~me~here.", 1).AddAvatar(90)
       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Have you eli... eliminate the monsters?")
       .AddOption("Yes!", 4);
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 15, 1) == false)
                                    data.AddOption("Give me one more drink.", 3);
                                data.AddAvatar(90)
                     .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Um... I`ve never seen you before. The Elder... The Elder sent you here? You need to prove it.")
                          .AddText("~Kill the Lava Scorpions, and... and offer this bottle of Strong Drink to the sacrificed Bright heroes.")
                          .AddText("~Go, go kill the monsters, and I`ll believe you.")
          .AddOption("Okay.", 2).AddAvatar(90)
   .FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Why are you still here? I`m incurable. Don`t waste your time.")
                                    .AddOption("Accept~the~treatment.", 7).AddAvatar(90)
             .FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Thanks for fulfilling my last wish! Now... Now I can die without anything to regret.")
                          .AddText("~In my next life, I may have a chance to thank you.")
                    .AddOption("Stop~it.~I`ll~save~you.", 6).AddAvatar(90)
             .FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Really? Oh, sorry... I was too nervous and angry. Monsters... Monsters killed my friends. It`s blood for blood!")
                      .AddOption("They`ll~pay~the~price.", 11).AddAvatar(90)
               .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You`re looking for the Flame Temple? If not I can`t even walk, I`ll go with you.")
                              .AddText("~Don`t worry. I know how to contact my tribe. Just go and do your job.")
                       .AddOption("Take~care!", 11).AddAvatar(90)
                .FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Really? Oh, sorry... I was too nervous and angry. Monsters... Monsters killed my friends. It`s blood for blood!")
                      .AddOption("They`ll~pay~the~price.", 255).AddAvatar(90)
               .FinalizeDialog();
                        }


                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(90)
            .FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 3008741, 10) && client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            if (!client.Inventory.Remove(3008741, 10, stream)) break;
                            client.Inventory.Add(stream, 3200345);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        data.AddText("You`re looking for the Flame Temple? If not I can`t even walk, I`ll go with you.")
                            .AddText("~Don`t worry. I know how to contact my tribe. Just go and do your job.")
                     .AddOption("Take~care!", 11).AddAvatar(90)
              .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3008741, 10))
                        {


                            data.AddText("How did you collect so many White Herbs? You`re too kind. I felt much better now.")
                                .AddOption("Take~a~good~rest.", 10).AddAvatar(90)
                 .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("(Ching Yan was seriously injred. To save him, you need to collect enough White Herbs. ")
                              .AddText("There is nothing more important then saving a life. Hurry and pick the White Herbs!)")
                              .AddOption("I need more White Herbs.", 6).AddAvatar(90)
               .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 15, 1))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in you inventory.")
                       .AddOption("Let me check.", 255).AddAvatar(90)
                .FinalizeDialog();
                                break;
                            }
                            if (HasItems(client, 3008740, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(3008740, 1, stream)) break;
                                client.Inventory.Add(stream, 3200346);
                                client.Player.Money += 5000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            }
                        }
                        data.AddText("Thanks for fulfilling my last wish! Now... Now I can die without anything to regret.")
                            .AddText("~In my next life, I may have a chance to thank you.")
                      .AddOption("Stop~it.~I`ll~save~you.", 6).AddAvatar(90)
               .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3998, 273, 273, 0);
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 15, 1))
                        {
                            data.AddText("Alas... My friends... You can rest in peace now.")
                                .AddOption("I`m~sorry.", 5)
                                .AddAvatar(90).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Don`t try to cheat... cheat me! See, so many monsters still... still get away whit their crimesc! Hurry and kill them all!")
                                   .AddOption("Got it!", 2)
                                   .AddAvatar(90).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(3200344, 1))
                            {
                                data.AddText("The drink is... is still in your inventory. Why did you ask more?")
                                    .AddOption("Sorry, I forgot it.", 255)
                                    .AddAvatar(90).FinalizeDialog();
                            }
                            else
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in you inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(90)
                    .FinalizeDialog();
                                    break;
                                }
                                client.Inventory.Add(stream, 3200344);
                                data.AddText("Cough... You lost the drink? Alright, alright... I'll give you another one, but keep it well.")
                        .AddOption("Okay.", 255).AddAvatar(90)
                 .FinalizeDialog();
                            }

                        }
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(90)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 3200344);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("Um... I`ve never seen you before. The Elder... The Elder sent you here? You need to prove it.")
                            .AddText("~Kill the Lava Scorpions, and... and offer this bottle of Strong Drink to the sacrificed Bright heroes.")
                            .AddText("~Go, go kill the monsters, and I`ll believe you.")
            .AddOption("Okay.", 2).AddAvatar(90)
     .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        if (!client.Inventory.Contain(3008740, 1))
                            client.Inventory.Add(stream, 3008740);
                        client.Player.QuestGUI.SendAutoPatcher(3998, 230, 273, 0);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.WhiteHerb1)]
        public static void WhiteHerb1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008741, 10))
                {
                    client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.WhiteHerb2)]
        public static void WhiteHerb2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008741, 10))
                {
                    client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.WhiteHerb3)]
        public static void WhiteHerb3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008741, 10))
                {
                    client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.WhiteHerb4)]
        public static void WhiteHerb4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008741, 10))
                {
                    client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.WhiteHerb5)]
        public static void WhiteHerb5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008741, 10))
                {
                    client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }
        [NpcAttribute(NpcID.WhiteHerb6)]
        public static void WhiteHerb6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Inventory.Contain(3008741, 10))
                {
                    client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
                    return;
                }
                client.Player.AddPick(stream, "Collecting...", 2);
            }
        }



        [NpcAttribute(NpcID.WoundedBrightTribesman)]
        public static void WoundedBrightTribesman(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Ow, stupid, stupid! I promised to the Chief to keep the Elder safe, but I can`t even protect myself now.")
                            .AddText("~I won`t forgive myself if the Elder gets into any trouble.")
          .AddOption("Let~me~help~you~out.", 255).AddAvatar(32)
   .FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.BrightGuard)]
        public static void BrightGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`ve tried my best to protect the Elder. Hope the Elder is okay.")
                            .AddText("~Disgusting monsters, I`ll kill you all someday.")
          .AddOption("You~will.", 255).AddAvatar(30)
   .FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BrokenForgeFurnace)]
        public static void BrokenForgeFurnace(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3635);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3636);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3637);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3638);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3639);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("In the deep corner, you discovered a Forge Furnace which was broken but still majestic,")
                                .AddText("~A huge number of monsters emerged from the furnace. It must be the root of the monster crisis.")

                        .AddOption("Check~the~furnace~carefully.", 1)
                           .FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("(You need to collect the Rune Fragments to complete the runes on the Forge Furnace first.)")
                 .AddOption("Use the Rune Fragment.", 2)
          .FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("(You read the runes on the Forge Furnace carefully, and understood the most words.)")
         .AddOption("Read~the~rune.", 6)
  .FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("(When you`re approaching the Forge Furnace, the Holy Hammer started shaking suddenly.")
                         .AddText("~You watched around and found a possible cause of the shaking - a hammering block.")
                         .AddText("~Why not hit the block with your Holy Hammer?)")
     .AddOption("Hit~the~block.", 10)
.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("(Since the Rune Fragments were collected from the alien monsters,")
                          .AddText("~you can probably find the Holy Hammer from their boss.)")
      .AddOption("Head~to~see~their~boss.", 9)
.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("(This Forge furnace is broken, but still majestic. The ancient rune engraved on its surface tells ")
                                    .AddText("the history of the Bringht Tribes.)")
                                    .AddOption("Pick up the Metal Block", 14).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("(You looked inside the furnace, and found an ugly Black Metal Block which made ")
                           .AddText("~you felt nervous. Perhaps, Chong Yan Elder knows more about this item.)")
       .AddOption("Give~the~block~to~the~Elder.", 13)
  .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("(This Forge Furnace is broken, but still majestic. The ancient rune engraved on its surface")
                         .AddText("~tells the history of the Bright Tribe.)")
     .AddOption("What~a~huge~furnace.", 255)
.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.Inventory.Add(stream, 3200344);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(You tried to read the rune on the furnace, but found too many parts missing.)")
                  .AddOption("Collect~rune~fragments~nearby.", 2)
           .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(3008752, 8))
                            {
                                data.AddText("(You`ve finally collected all the missing parts, and complete the runes on the furnace.)")
              .AddOption("What~does~it~say?", 4)
       .FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("(You should have 8 Rune Fragments to complete the runes.)")
                .AddOption("Go collect more.", 3)
         .FinalizeDialog();
                            }
                        }
                        else
                        {
                            client.Player.QuestGUI.SendAutoPatcher(3998, 226, 406, 0);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(3008752, 8))
                        {
                            if (HasItems(client, 3008752, 8) && client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                if (!client.Inventory.Remove(3008752, 8, stream)) break;
                                client.Inventory.Add(stream, 3200346);
                                client.Player.Money += 5000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            }
                        }

                        data.AddText("(You read the runes on the Forge Furnace carefully, and understood the most words.)")
           .AddOption("Read~the~rune.", 6)
    .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        var action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.DrawStory,
                            dwParam = 1002

                        };
                        client.Send(stream.ActionCreate(&action));

                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        data.AddText("(It was the biggest forge furnace in ancient Bright, which had produced countless famous weapons.")
                            .AddText("~According to the runes, the Master Blacksmith was the most brilliant craftsman for metal working,")
                            .AddText("~and ranked only next to the Chief of Bright Tribe.)")
        .AddOption("I~see.", 7)
 .FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("You noted that a hammer icon always appeared when the Master Blacksmith was mentioned in the runes.")
                            .AddText("~Is this Holy Hammer a symbol of the Master Blacksmith? If you can find it, something amazing")
                            .AddText("~may happen. Do you want to have a try?")
        .AddOption("Go~and~look~for~the~hammer.", 8)
 .FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Add(stream, 3200344);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(Since the Rune Fragments were collected from the alien monsters,")
                         .AddText("~you can probably find the Holy Hammer from their boss.)")
     .AddOption("Head~to~see~their~boss.", 9)
.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3998, 392, 706, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3008750, 1))
                        {
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "Attack35r");
                            data.AddText("(When you hit the block with the Holy Hammer, an invisible energy wave bursted out, ")
                       .AddText("~killing the alien monsters in silence. No more monsters creep out from the furnace.")
                       .AddText("~It seems the hammer power has fully suppressed the evil force in the furnace.)")
   .AddOption("Great!", 12)
.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("(You don`t have the Holy Hammer to hit the block. Go challenge the Violet Bat King, and you'll ")
                         .AddText("have a change to get hammer after defeating him.)")
     .AddOption("I see.", 11)
.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(3008750, 1))
                        {
                            if (HasItems(client, 3008750, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                if (!client.Inventory.Remove(3008750, 1, stream)) break;
                                client.Inventory.Add(stream, 3009001, 2);
                                client.Inventory.Add(stream, 3200346);
                                client.Player.Money += 5000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 2 Bright Star Stones!", MsgMessage.ChatMode.System);
                            }
                        }
                        data.AddText("(You looked inside the furnace, and found an ugly Black Metal Block which made ")
                        .AddText("~you felt nervous. Perhaps, Chong Yan Elder knows more about this item.)")
    .AddOption("Give~the~block~to~the~Elder.", 13)
.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255)
            .FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        if (!client.Inventory.Contain(3008751, 1))
                            client.Inventory.Add(stream, 3008751);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, 0);
                        break;
                    }
                case 14:
                    {
                        if (!client.Inventory.Contain(3008751, 1))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in you inventory.")
                       .AddOption("Let me check.", 255)
                .FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 3008751);
                        }
                        data.AddText("(The Black Metal Block is already in your inventory. Hurry and talk to Chong Yan Elder about this block.)")
               .AddOption("I`ll go now.", 15)
        .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, 0);
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3998, 392, 706, 0);
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3998, 226, 406, 0);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ChongYan)]
        public static void ChongYan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.RemainofBrightTribesman, 0, 3633);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3634);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3635);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3639);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3640);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Why did you come back again?")
              .AddOption("I go something for you.", 10).AddAvatar(67)
       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("HELP! HELP! Could you help me out?")
                             .AddOption("What`s~wrong?", 1).AddAvatar(67)
                                .FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Have you cleared those monsters nearby?")
                .AddOption("Yes, It's safe now.", 4).AddAvatar(67)
         .FinalizeDialog();


                            }
                            else
                            {
                                data.AddText("Hero, you saved my life, and I am eternally grateful! We got attacked by a horde of")
                              .AddText("~strange monsters. It happened all of a sudden, and we suffered heavy casualties.")
                              .AddText("~We got no way to go, and retreated here.")
                .AddOption("What~kind~of~monsters?", 2).AddAvatar(67)
         .FinalizeDialog();
                            }
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                data.AddText("We're okay. Please go find the Flame Temple and remember my words. It's so urgent!")
                                             .AddOption("I`ll start off now.", 255).AddAvatar(67)
                                      .FinalizeDialog();
                                break;
                            }
                            data.AddText("The Chief asked you to find us? If so, you must have known the importance of the Flame Temple.")
                              .AddText("~To stop the Fiery Dragon Volcano from eruption, we have to completely activate the Mystery Pearl")
                              .AddText("~at the altar in front of the temple. If we failed, the flowing lava will swallow all living things,")
                              .AddText("~repeating the disaster of our ancestors. How stupid I am to get stuck here at this critical moment.")
                              .AddText("~I feel so sorry to my people.")
                .AddOption("I~can~help~you.", 6).AddAvatar(67)
         .FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false &&
                        client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Time is limited. Please hit the road now.")
                      .AddOption("Okay.", 12).AddAvatar(67)
                      .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Now the monsters have been swept off, we`ll continue to find the Flame Temple.")
                            .AddText("~You can go first, and I`ll inform our Chief. If you see our lost tribesmen, please give them a hand.")
                            .AddOption("Got~it.", 12).AddAvatar(67)
                            .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("i got stuck here, you my people are still waiting for my news about the Flame Temple. ")
                      .AddText("I don't want to disappoint them.")
  .AddOption("You'll be okay.", 255).AddAvatar(67)
.FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("You meant you got this metal block from the Forge Furnace? It`s hard to tell what it is,")
                            .AddText("~but the material looks the same as that of the top Bright craftsman`s ID token.")
                            .AddText("~Well, I`ll go ask the scholars in our tribe.")
                            .AddOption("Is~there~anything~I~can~help?", 11).AddAvatar(67)
                            .FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(67)
            .FinalizeDialog();
                            break;
                        }
                        if (HasItems(client, 3008751, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.Remove(3008751, 1, stream)) break;
                            client.Inventory.Add(stream, 3200344);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        data.AddText("Now the monsters have been swept off, we`ll continue to find the Flame Temple.")
                         .AddText("~You can go first, and I`ll inform our Chief. If you see our lost tribesmen, please give them a hand.")
                         .AddOption("Got~it.", 12).AddAvatar(67)
                         .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("It`s great! See that Fiery Dragon Volcano over there? The Flame Temple was built at the foot of the volcano.")
                            .AddText("~First, you need to break through the monster siege. When you find the Flame Temple and restore its altar,")
                            .AddText("~put this Mystery Pearl at the altar. Then, I`ll do the rest to completely dispel the crisis of Fiery Dragon.")
                                            .AddOption("I~see.~Take~care~of~yourself!", 7).AddAvatar(67)
                                     .FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in you inventory.")
                       .AddOption("Let me check.", 255).AddAvatar(67)
                .FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Add(stream, 3200346);
                                client.Player.Money += 5000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            }
                            data.AddText("The Chief asked you to find us? If so, you must have known the importance of the Flame Temple.")
                                                            .AddText("~To stop the Fiery Dragon Volcano from eruption, we have to completely activate the Mystery Pearl")
                                                            .AddText("~at the altar in front of the temple. If we failed, the flowing lava will swallow all living things,")
                                                            .AddText("~repeating the disaster of our ancestors. How stupid I am to get stuck here at this critical moment.")
                                                            .AddText("~I feel so sorry to my people.")
                                              .AddOption("I~can~help~you.", 6).AddAvatar(67)
                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                        {

                            data.AddText("Thanks for your help! We`re finally safe now.")
                                             .AddOption("You`re~welcome.", 5).AddAvatar(67)
                                      .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There are still many monsters crawling around. Could you kill more?")
                                              .AddOption("Okay.", 3).AddAvatar(67)
                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("I don`t know, but I saw flame runes imprinted on their body. Every time I tried to approach them,")
                               .AddText("~I got burnt. You know, it`s impossible for me to kill so many monsters. Could you help me?")
                               .AddText("~Please kill the monsters surrounding us, and my people can be saved.")
                 .AddOption("Count~on~me!", 3).AddAvatar(67)
          .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 145, 338, 0);
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(67)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 3200344);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("Hero, you saved my life, and I am eternally grateful! We got attacked by a horde of")
                                .AddText("~strange monsters. It happened all of a sudden, and we suffered heavy casualties.")
                                .AddText("~We got no way to go, and retreated here.")
                  .AddOption("What~kind~of~monsters?", 2).AddAvatar(67)
           .FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RemainofBrightTribesman)]
        public static void RemainofBrightTribesman(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            //3633
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PakYan, 0, 3632);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.RemainofBrightTribesman, 0, 3633);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You found a burnt body on the way to find the Elder.")
                             .AddOption("Have~a~check.", 1).AddAvatar(67)
                                .FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {

                            data.AddText("We couldn`t afford any delay. Chong Yan Elder and other must have encountered terrific enemy.")
                         .AddText(" Hurry and follow their footsteps. Hope it`s not too late.")
                .AddOption("Let`s go!", 2).AddAvatar(67)
         .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("(The Bright Tribesman`s body was left lying on the read.)")
                             .AddOption("Poor guy.", 255).AddAvatar(67)
                                .FinalizeDialog();

                        }
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(67)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(It seems the body has been burnt, and his remaining accessories belonged to the Bright Tribe.")
                            .AddText("~You found many bloodstains and footsteps along the road. The Bright tribesmen must have a")
                            .AddText("~tough fight here, and retreated to that direction.)")
                   .AddOption("Follow~the~footsteps.", 2).AddAvatar(67)
            .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
            }

        }


        [NpcAttribute(NpcID.GuidofFieryDragon)]
        public static void GuidofFieryDragon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Have you been to the Fiery Dragon region? The land was home to the Bright Tribe.")
                            .AddText("~I wonder what it looks like now.")
                     .AddOption("Send~me~to~Fiery~Dragon.", 1)
                     .AddOption("You`ll~return~home~soon.", 255).AddAvatar(7)
              .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(4020, 73, 98, 0);
                        break;
                    }
            }

        }
    }
}
