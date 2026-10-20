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
        [NpcAttribute(NpcID.PromotionArcher)]
        public static void PromotionArcher(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 40 && client.Player.Class <= 45))
            {
                data.AddText("Sorry, but you are not a Archer.")
                                                           .AddOption("Alright.", 255)
                                                           .AddAvatar(10).FinalizeDialog();
                return;
            }


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("Born with sharp insight and swift skill, the Archers are expert in shooting arrows and wielding throwing knives to deadly purpose.")
                                    .AddOption("Learn~about~Promotion.", 110);
                            if (client.Player.Class == 40)
                                data.AddOption("I~want~to~get~promoted", 1);
                            else
                                data.AddOption("I~want~to~get~promoted", 33);
                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 40)
                        {
                            data.AddText("Born with sharp insight and swift skill, the Archers are expert in shooting arrows and wielding throwing knives to deadly purpose.")
                            .AddOption("I~want~to~get~promoted", 1)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Born with sharp insight and swift skill, the Archers are expert in shooting arrows and wielding throwing knives to deadly purpose.")
                               .AddOption("I~want~to~get~promoted", 33)
                               .AddOption("Learn~the~Class~Skill.", 5)
                               .AddOption("Learn~the~Pure~Skill.", 30)
                               .AddOption("I`ll~think~about~it.", 255)
                               .AddAvatar(10).FinalizeDialog();
                        }


                        break;
                    }
                case 110:
                    {

                        data.AddText("Ah, you look promising. I bet `Archer` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 111:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 3005160);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                        }
                        data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                                    .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                        if (client.Player.Class == 40)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else
                            data.AddOption("I~want~to~get~promoted", 33);
                        data.AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 33:
                    {
                        if (client.Player.Level >= 15 && client.Player.Level < 40 && client.Player.Class == 41)
                        {
                            data.AddText("Sorry,~you~cannot~get~promoted~before~you~reach~level~40.~Please~train~harder.")
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(234).FinalizeDialog();
                            return;
                        }


                        switch (client.Player.Class)
                        {
                            case 41:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(10).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1072031, 5))
                                        {
                                            data.AddText("I'm sorry but you need to have 5 EuxeniteOre in your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {

                                            if (!client.Inventory.Remove(1072031, 5, stream)) break;

                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(5).FinalizeDialog();
                                                    break;
                                                }
                                                //500069 ScarletBow
                                                client.Inventory.Add(stream, 500069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)ScarletBow.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Archer. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(10).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 42:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(10).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(2))
                                                {
                                                    data.AddText("Please make 2 more spaces in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(10).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//150139 PearlRing
                                                client.Inventory.Add(stream, 133049, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//133049 ApeCoat
                                                client.SendSysMesage("You received 1 (Super)ApeCoat and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Archer. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(10).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 43:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(10).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(10).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 500189, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2RosewoodBow 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Archer. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(10).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 44://133089 EagleCoat
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(10).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(2))
                                            {
                                                data.AddText("Please make 2 more spaces in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(10).FinalizeDialog();
                                                break;
                                            }
                                            if (client.Player.Reborn == 1)
                                            {
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received DragonBall and 3 DragonBall(B).", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (client.Player.Reborn == 2)
                                            {
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                                                client.SendSysMesage("You received 3 DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 1);
                                            client.SendSysMesage("You received 1 DragonBallScroll.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            if (!client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Archer. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(10).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                        .AddOption("Thank~you,~Master.", 255)
                                                                                                        .AddAvatar(10).FinalizeDialog();


                                    break;
                                }
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Flight, ranged attack and maneuverability are powers all possessed by the archer.")
                           .AddOption("I~want~to~get~promoted", 2)
                           .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Class == 40)
                        {
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.XpFly);
                            client.Player.Class++;
                            data.AddText("Good, you have trained hard, and completed my lesson this day. You have gained power and skills.")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(10).FinalizeDialog();
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }


                        break;
                    }
                case 30:
                    {
                        data.AddText("Only Pure Archers can learn the Pure Skill, Star Arrow. If you`ve been Reborn twice, but are not a Pure Archer, you can take some Oblivion Dew to the Reincarnation Seer in the Market and be")
                     .AddText("~Reincarnated. That way, you can obtain a new class combination.")
                            .AddOption("I~want~to~learn~it~now.", 31)
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(10).FinalizeDialog();



                        break;
                    }
                case 31:
                    {
                        if (client.Player.Reborn == 2 && Database.AtributesStatus.IsArcher(client.Player.FirstClass) && Database.AtributesStatus.IsArcher(client.Player.SecoundeClass))
                        {
                            var SpellID = Role.Flags.SpellID.StarArrow;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Only pure Archers, whose classes for their past two lives and their current life are all Archers, can learn the pure skill Star Arrow.")
                      .AddOption("Okay.~I~see.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("The powerful Archers are expert in shooting clustered demons to death. When transforming into Oriental Assassins, they are able to wield the Throwing Knives to deadly purpose.")
                             .AddOption("Learn~Archer~skills.", 10)
                             .AddOption("Learn~Assassin~skills.", 11)
                            //.AddOption("Learn~Superior~skills.", 12)
                             .AddOption("Leave.", 255)
                             .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        data.AddText("After you get promoted as an Eagle Archer at Level 40, you can equip a Throwing Knife and use skill `Path Of Shadow` to disguise as an Assassin! The skills will cause fatal damage to the opponent!\n")
                           .AddText("Tip: Use Mortal Wound under Stigma state, to inflict high damage!")
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PathOfShadow) ? "(Learnt)" : "L40:") + "~Path~Of~Shadow~(Disguise)", 20)
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeFlurry) ? "(Learnt)" : "L40:") + "~Blade~Flurry~(XP)", 21)
                        .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalWound) ? "(Learnt)" : "L40:") + "~Mortal~Wound~(Offensive)", 22)
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KineticSpark) ? "(Learnt)" : "L50:") + "~Kinetic~Spark~(Passive)", 23)
                       .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlisteringWave) ? "(Learnt)" : "L70:") + "~Blistering~Wave~(Mass)", 24)
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritFocus) ? "(Learnt)" : "L90:") + "~Spirit~Focus~(Boost)", 25)
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DaggerStorm) ? "(Learnt)" : "L100:") + "~Dagger~Storm~(Mass)", 26)
                      .AddOption("Leave.", 255)
                      .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 26:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 44)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DaggerStorm;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 25:
                    {
                        if (client.Player.Level < 90 || client.Player.Class < 43)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 90 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SpiritFocus;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 24:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 43)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BlisteringWave;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 23:
                    {
                        if (client.Player.Level < 50 || client.Player.Class < 42)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 50 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.KineticSpark;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 42)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MortalWound;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 42)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BladeFlurry;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();



                        break;
                    }
                case 20:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 42)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.PathOfShadow;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        data.AddText("After equipping bow and arrows, Archers can fly and attack enemies in the air!")
                      .AddOption("XP~Fly~[Level~1]", 70)
                      .AddOption("Rapid~Fire" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RapidFire) ? "(Learnt)" : "[Level~46+]") + "", 13)
                      .AddOption("Improved~Fly" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fly) ? "(Learnt)" : "[Level~70+]") + "", 14)
                      .AddOption("Arrow~Rain" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowRain) ? "(Learnt)" : "[Level~70+]") + "", 15)
                      .AddOption("Intensify" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Intensify) ? "(Learnt)" : "[Level~71+]") + "", 16)
                      .AddOption("Scatter~Fire" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScatterFire) ? "(Learnt)" : "[Level~23+]") + "", 17)
                      .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 70:
                    {
                        var SpellID = Role.Flags.SpellID.XpFly;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 17:
                    {
                        if (client.Player.Level < 23 || client.Player.Class < 41)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 23 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ScatterFire;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        if (client.Player.Level < 71 || client.Player.Class < 42)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 71 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Intensify;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 42)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ArrowRain;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 42)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Fly;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 46 || client.Player.Class < 41)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 46 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.RapidFire;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PromotionLeeLong)]
        public static void PromotionLeeLong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 80 && client.Player.Class <= 85))
            {
                data.AddText("Sorry, but you are not a Dragon-Warrior.")
                                                           .AddOption("Alright.", 255)
                                                           .AddAvatar(259).FinalizeDialog();
                return;
            }


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 6426);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("Dragon-Warrior is a kung fu star who fights fast, accurately and ruthlessly, as well as a group training martial arts on the basis")
                      .AddText("~of practicality. The Dragon Warrior masters martial arts of various schools, and also excels in wielding many kinds of weapons, such as nunchaku.")
                    .AddOption("Learn~about~Promotion.", 110)
                      .AddOption("I~want~to~get~promoted", 3)
                     .AddOption("I`ll~think~about~it.", 255)
                     .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 80)
                        {

                            data.AddText("Dragon-Warrior is a kung fu star who fights fast, accurately and ruthlessly, as well as a group training martial arts on the basis")
                             .AddText("~of practicality. The Dragon Warrior masters martial arts of various schools, and also excels in wielding many kinds of weapons, such as nunchaku.")
                            .AddOption("I~want~to~get~promoted", 3)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Dragon-Warrior is a kung fu star who fights fast, accurately and ruthlessly, as well as a group training martial arts on the basis")
                             .AddText("~of practicality. The Dragon Warrior masters martial arts of various schools, and also excels in wielding many kinds of weapons, such as nunchaku.")
                                .AddOption("I~want~to~get~promoted", 4)
                               .AddOption("Learn~the~Class~Skill.", 5)
                               .AddOption("Learn~the~Pure~Skill.", 30)
                               .AddOption("I`ll~think~about~it.", 255)
                               .AddAvatar(259).FinalizeDialog();
                        }

                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Dragon-Warrior` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(259).FinalizeDialog();
                        break;
                    }
                case 111:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 3005160);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                        }
                        data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                                    .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                          .AddOption("Job~Promotion~2.", 3)
                          .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Player.Level >= 15 && client.Player.Level < 40 && client.Player.Class == 81)
                        {
                            data.AddText("`Dragon-Warrior` can get promoted to be `Expert Dragon-Warrior` at Level 40.")
                                .AddText("You should have reached Level 40 to get promoted. Keep practicing!.")
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(259).FinalizeDialog();
                        }


                        switch (client.Player.Class)
                        {
                            case 81:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(259).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(2))
                                                {
                                                    data.AddText("Please make 2 more spaces in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(259).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 617079, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//617079 SpiralNunchaku
                                                client.SendSysMesage("You received 2 (Super)SpiralNunchaku.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Dragon-Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(259).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 82:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(259).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(259).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(2))
                                                {
                                                    data.AddText("Please make 2 more spaces in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(259).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//150139 PearlRing
                                                client.Inventory.Add(stream, 138069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//138069 CombatSuit(Lv.70) 
                                                client.SendSysMesage("You received 1 (Super)CombatSuit and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Dragon-Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(259).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 83:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(259).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(259).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(259).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 617199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2SpiritNunchaku 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Dragon-Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(259).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 84:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(259).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(259).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(2))
                                            {
                                                data.AddText("Please make 2 more spaces in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(259).FinalizeDialog();
                                                break;
                                            }
                                            if (client.Player.Reborn == 1)
                                            {
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received DragonBall and 3 DragonBall(B).", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (client.Player.Reborn == 2)
                                            {
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                                                client.SendSysMesage("You received 3 DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }

                                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 1);
                                            client.SendSysMesage("You received 1 DragonBallScroll.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            if (!client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Dragon-Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(259).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(259).FinalizeDialog();

                                    break;
                                }
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("The pure skill, [Dragon Fury], is exclusive for Dragon-Warrior who were also Dragon-Warrior in previous two lives.")
                            .AddText("~If you`ve been 2nd-reborn but not a pure Dragon-Warrior, go find the Rebirth Master for reincarnation to get a new class combination.")
                         .AddOption("Learn~[Dragon~Fury].", 31)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (client.Player.Reborn >= 2 && Database.AtributesStatus.IsLee(client.Player.FirstClass) && Database.AtributesStatus.IsLee(client.Player.SecoundeClass))
                        {
                            var SpellID = Role.Flags.SpellID.DragonFury;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(259).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("To learn [Dragon Fury], you should be Dragon-Warrior for three lives in a row and reach at least Level 70.")
                    .AddOption("OK", 255)
                    .AddAvatar(259).FinalizeDialog();
                        }


                        break;
                    }

                case 5:
                    {
                        data.AddText("Dragon-Warrior enjoys unique and brilliant combo skills. Typically, the striking [Dragon Kicks], destructive [Dragon Swipe]")
                            .AddText("~and overwhelming [Dragon Strides] enable the caster to get close to enemies at a lightning speed")
                            .AddText("~and kill the target in a second")
                          .AddOption("Dragon~Punch. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPunch) ? "(Learnt)" : "(Lv.15)") + "", 6)
                          .AddOption("Dragon~Cyclone. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonCyclone) ? "(Learnt)" : "(Lv.15)") + "", 7)
                          .AddOption("AirKick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirKick) ? "(Learnt)" : "(Lv.15)") + "", 8)
                          .AddOption("AirRaid. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirRaid) ? "(Learnt)" : "(Lv.15)") + "", 9)
                          .AddOption("AirSweep. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirSweep) ? "(Learnt)" : "(Lv.15)") + "", 10)
                          .AddOption("Dragon~Flow. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFlow) ? "(Learnt)" : "(Lv.15)") + "", 11)
                          .AddOption("Cracking~Swipe. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackingSwipe) ? "(Learnt)" : "(Lv.70)") + "", 12)
                          .AddOption("Next.", 15)
                          .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 15:
                    {
                        data.AddText("Dragon-Warrior enjoys unique and brilliant combo skills. Typically, the striking [Dragon Kicks], destructive [Dragon Swipe]")
                            .AddText("~and overwhelming [Dragon Strides] enable the caster to get close to enemies at a lightning speed")
                            .AddText("~and kill the target in a second")
                        .AddOption("Splitting~Swipe. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SplittingSwipe) ? "(Learnt)" : "(Lv.100)") + "", 13)
                        .AddOption("Dragon~Slash. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSlash) ? "(Learnt)" : "(Lv.100)") + "", 14)
                        .AddOption("Dragon~Roar. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRoar) ? "(Learnt)" : "(Lv.70)") + "", 16)
                        .AddOption("Dragon~Swing. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSwing) ? "(Learnt)" : "(Lv.70)") + "", 17)
                        .AddOption("SpeedKick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedKick) ? "(Learnt)" : "(Lv.40)") + "", 18)
                        .AddOption("ViolentKick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ViolentKick) ? "(Learnt)" : "(Lv.40)") + "", 19)

                        .AddOption("Previous~page.", 5).AddOption("Next.", 120).AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 120:
                    {
                        data.AddText("Dragon-Warrior enjoys unique and brilliant combo skills. Typically, the striking [Dragon Kicks], destructive [Dragon Swipe]")
                            .AddText("~and overwhelming [Dragon Strides] enable the caster to get close to enemies at a lightning speed")
                            .AddText("~and kill the target in a second")
                        .AddOption("AirStrike. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirStrike) ? "(Learnt)" : "(Lv.1)") + "", 121)
                        .AddOption("EarthSweep. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthSweep) ? "(Learnt)" : "(Lv.1)") + "", 122)
                        .AddOption("Kick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Kick) ? "(Learnt)" : "(Lv.1)") + "", 123)
                        .AddOption("StormKick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StormKick) ? "(Learnt)" : "(Lv.40)") + "", 20)
                        .AddOption("Previous~page.", 5).AddOption("Next.", 120).AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 121:
                    {
                        var SpellID = Role.Flags.SpellID.AirStrike;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 122:
                    {
                        var SpellID = Role.Flags.SpellID.EarthSweep;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;

                    }
                case 123:
                    {

                        var SpellID = Role.Flags.SpellID.Kick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;

                    }
                case 20:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 82)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.StormKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 19:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 82)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ViolentKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 18:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 82)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SpeedKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 17:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 83)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DragonSwing;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 16:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 83)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DragonRoar;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 84)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DragonSlash;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 84)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SplittingSwipe;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 80)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.CrackingSwipe;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        var SpellID = Role.Flags.SpellID.DragonFlow;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        var SpellID = Role.Flags.SpellID.AirSweep;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        var SpellID = Role.Flags.SpellID.AirRaid;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        var SpellID = Role.Flags.SpellID.AirKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        var SpellID = Role.Flags.SpellID.DragonCyclone;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        var SpellID = Role.Flags.SpellID.DragonPunch;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level >= 15 && client.Player.Class == 80)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPunch))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonPunch);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonCyclone))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonCyclone);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFlow))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFlow);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirRaid))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirRaid);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirSweep))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirSweep);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirKick))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirKick);
                            }
                            client.Player.Class++;
                            data.AddText("You`ve been promoted!")
                            .AddOption("Great,~thanks!", 255)
                            .AddAvatar(259).FinalizeDialog();//7991 384 251
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }
                        else
                        {
                            data.AddText("Sorry, you need level 15.")
                      .AddOption("Ah ok.", 255)
                      .AddAvatar(259).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PromotionPirate)]
        public static void PromotionPirate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 70 && client.Player.Class <= 75))
            {
                data.AddText("Sorry, but you are not a Pirate.")
                                                           .AddOption("Alright.", 255)
                                                           .AddAvatar(234).FinalizeDialog();
                return;
            }

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("We Pirates are always in pursuit of freedom and strength. The rapier and pistol stand for cruelty and mercilessness.")
                               .AddOption("Learn~about~Promotion.", 110);
                            if (client.Player.Class == 70)
                                data.AddOption("I~want~to~get~promoted", 1);
                            else
                                data.AddOption("I~want~to~get~promoted", 4);
                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(234).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 70)
                        {
                            data.AddText("We Pirates are always in pursuit of freedom and strength. The rapier and pistol stand for cruelty and mercilessness.")
                            .AddOption("I~want~to~get~promoted", 1)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(234).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("We Pirates are always in pursuit of freedom and strength. The rapier and pistol stand for cruelty and mercilessness.")
                               .AddOption("I~want~to~get~promoted", 4)
                               .AddOption("Learn~the~Class~Skill.", 5)
                               .AddOption("Learn~the~Pure~Skill.", 20)
                               .AddOption("I`ll~think~about~it.", 255)
                               .AddAvatar(234).FinalizeDialog();
                        }


                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Pirate` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(234).FinalizeDialog();


                        break;
                    }
                case 111:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 3005160);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                        }
                        data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                                    .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                        if (client.Player.Class == 70)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else
                            data.AddOption("I~want~to~get~promoted", 4);
                        data.AddAvatar(234).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.Level >= 15 && client.Player.Level < 40 && client.Player.Class == 71)
                        {
                            data.AddText("As a Pirate, you`ll need to reach Level 40 to get promoted to a Pirate Gunner. You will learn Adrenaline Rush, Eagle Eye, Blackbeard`s Rage and Black Spot after promotion.")
                                .AddText("~If you never change your class as a Pirate after the 2nd Rebirth, you can learn Scurvy Bomb.")
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(234).FinalizeDialog();


                        }
                        switch (client.Player.Class)
                        {
                            case 71:
                                {

                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(5).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(2))
                                                {
                                                    data.AddText("Please make 2 more spaces in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(5).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 611079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//611079 SirenRapier
                                                client.Inventory.Add(stream, 612079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//612079 SteelPistol 70
                                                client.SendSysMesage("You received 1 (Super)SirenRapier and 1 (Super)SteelPistol.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Pirate. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 72:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(5).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(2))
                                                {
                                                    data.AddText("Please make 2 more spaces in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(5).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//150139 PearlRing
                                                client.Inventory.Add(stream, 139069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//139069 TideCoat
                                                client.SendSysMesage("You received 1 (Super)TideCoat and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Pirate. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 73:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(5).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(5).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 611199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2BloodRapier 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Pirate. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 74:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(5).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(2))
                                            {
                                                data.AddText("Please make 2 more spaces in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(5).FinalizeDialog();
                                                break;
                                            }
                                            if (client.Player.Reborn == 1)
                                            {
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received DragonBall and 3 DragonBall(B).", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (client.Player.Reborn == 2)
                                            {
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                                                client.SendSysMesage("You received 3 DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }

                                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 1);
                                            client.SendSysMesage("You received 1 DragonBallScroll.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            if (!client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Pirate. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(5).FinalizeDialog();


                                    break;
                                }
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("If you want to learn the Scurvy Bomb, you`ll need to be a pure Pirate, who`ve been a Pirate for 3 straight lives. You can talk to the Rebirth Master in the Market to have the Reincarnation.")
                         .AddOption("Learn~the~skill.", 21)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(234).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        if (client.Player.Reborn >= 2 && Database.AtributesStatus.IsPirate(client.Player.FirstClass) && Database.AtributesStatus.IsPirate(client.Player.SecoundeClass))
                        {
                            var SpellID = Role.Flags.SpellID.ScurvyBomb;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("If you want to learn the Scurvy Bomb, you`ll need to be a pure Pirate, who`ve been a Pirate for 3 straight lives.")
                    .AddOption("OK", 255)
                    .AddAvatar(234).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        data.AddText("As an Intern Pirate, you can get promoted to a Pirate and learn Blade Tempest after you reach Level 15. When you reach Level 20, you can learn Gale Bomb.")
                        .AddOption("Get~promoted.", 3)
                        .AddOption("I`ll~think~about~it.", 255)
                        .AddAvatar(234).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level >= 15 && client.Player.Class == 70)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CannonBarrage))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CannonBarrage);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeTempest))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeTempest);
                            }
                            client.Player.Class++;
                            data.AddText("You`ve been promoted!")
                            .AddOption("Great,~thanks!", 255)
                            .AddAvatar(234).FinalizeDialog();
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }
                        else
                        {
                            data.AddText("Sorry, you need level 15.")
                      .AddOption("Ah ok.", 255)
                      .AddAvatar(234).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("Which Pirate Skill do you want to learn?")
                          .AddOption("Blade~Tempest" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeTempest) ? "(Learnt)" : "(Lv.15)") + "", 6)
                          .AddOption("Gale~Bomb" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GaleBomb) ? "(Learnt)" : "(Lv.20)") + "", 7)
                              .AddOption("Adrenaline~Rush" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdrenalineRush) ? "(Learnt)" : "(Lv.40)") + "", 8)
                          .AddOption("Eagle~Eye" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EagleEye) ? "(Learnt)" : "(Lv.40)") + "", 9)
                          .AddOption("Blackbeard`s~Rage" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlackbeardsRage) ? "(Learnt)" : "(Lv.40)") + "", 10)
                          .AddOption("Kraken`s~Revenge" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KrakensRevenge) ? "(Learnt)" : "(Lv.70)") + "", 11)
                          .AddOption("Next.", 15)
                          .AddAvatar(234).FinalizeDialog();


                        break;
                    }
                case 15:
                    {
                        data.AddText("Which Pirate Skill do you want to learn?")
                            //.AddOption("Black~Spot" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.) ? "(Learnt)" : "(Lv.15)") + "", 12)
                       .AddOption("Windstorm" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Windstorm) ? "(Learnt)" : "(Lv.15)") + "", 13)
                       .AddOption("Learn~the~Golem" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem) ? "(Learnt)" : "(Lv.15)") + "", 14)
                            .AddOption("CannonBarrage" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CannonBarrage) ? "(Learnt)" : "(Lv.15)") + "", 60)
                       .AddOption("Previous~page.", 5).AddAvatar(234).FinalizeDialog();

                        break;
                    }
                case 60:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 70)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 15 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.CannonBarrage;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 71)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Golem;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 71)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Windstorm;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 71)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BladeTempest;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < 20 || client.Player.Class < 71)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.GaleBomb;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 72)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.AdrenalineRush;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 72)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.EagleEye;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 72)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BlackbeardsRage;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn > 1 && client.Player.FirstClass == 75)
                        {
                            if (client.Player.Level < 70 || client.Player.Class < 73)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.KrakensRevenge;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry you are not reborn, or not is pirat-pirat ")
                                                                    .AddOption("Ok , thank you.", 255)
                                                                    .AddAvatar(5).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}
