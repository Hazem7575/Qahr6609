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
        [NpcAttribute(NpcID.PromotionTaoist)]
        public static void PromotionTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 130 && client.Player.Class <= 145 || client.Player.Class == 100 || client.Player.Class == 101))
            {
                data.AddText("Sorry, but you are not a Taoist.").AddOption("Alright.", 255).AddAvatar(6).FinalizeDialog();
                return;
            }


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("Every Taoist wants holy spirit, and few gain. You`re gifted in controlling inner power. Just pay more attention to details, you may be the next achiever!")
                                .AddOption("Learn~about~Promotion.", 110);
                            if (client.Player.Class == 100)
                                data.AddOption("I~want~to~get~promoted", 1);
                            else if (client.Player.Class == 101)
                                data.AddOption("I~want~to~get~promoted.", 3);
                            else
                                data.AddOption("I~want~to~get~promoted.", 53);
                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 100)
                        {

                            data.AddText("Every Taoist wants holy spirit, and few gain. You`re gifted in controlling inner power. Just pay more attention to details, you may be the next achiever!")
                            .AddOption("I~want~to~get~promoted", 1)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Every Taoist wants holy spirit, and few gain. You`re gifted in controlling inner power. Just pay more attention to details, you may be the next achiever!");

                            if (client.Player.Class == 100 || client.Player.Class == 101)
                                data.AddOption("I~want~to~get~promoted.", 3);
                            else
                                data.AddOption("I~want~to~get~promoted.", 53);
                            data.AddOption("Learn~class~skills.", 10)
                            .AddOption("Learn~Pure~Skills.", 50)
                            .AddOption("Epic Quest(L70, 2nd-reborn)", 190)
                            .AddOption("Nothing.", 255)
                            .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet Taoist will be a big name in a short time. Listen, you can learn class skills and")
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
                        if (client.Player.Class == 100)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else if (client.Player.Class == 101)
                            data.AddOption("I~want~to~get~promoted.", 3);
                        else
                            data.AddOption("I~want~to~get~promoted.", 53);
                        data.AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 190:
                    {
                        data.AddText("The Great Sun King has acquired the essence of Taoism and mad a Divine Panacea.")
                           .AddText(" This mythical pill can be used to reshape the backsword, and bring out the")
                           .AddText(" endless power. However, it`s not that easy. You'll need guidance from Sun King, profound")
                           .AddText(" understanding on art of Taoism, and also lucky. Many Taoists have set out to find the way")
                           .AddText(" of forging an Epic Weapon.")
                          .AddOption("Take me to see Sun King", 191)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 191:
                    {
                        if (client.Player.Level >= 70 && client.Player.Reborn >= 2)
                        {
                            client.Teleport(84, 84, 3933);
                        }
                        else
                        {
                            data.AddText("Please come back when you will be 2nd Reborn and with a higher level than 70.")
                                                                        .AddOption("Okay.", 255)
                                                                        .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 53:
                    {
                        switch (client.Player.Class)
                        {
                            case 142:
                            case 132:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
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
                                                                   .AddAvatar(6).FinalizeDialog();
                                                    break;
                                                }
                                                //stream, 300000, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true
                                                client.Inventory.Add(stream, 152129, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//152129 BoneBracelet
                                                client.Inventory.Add(stream, 134069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//134069 CraneVestment
                                                client.SendSysMesage("You received 1 (Super)BoneBracelet and 1 (Super)CraneVestment.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Taoist. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(6).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(6).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 143:
                            case 133:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
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
                                                                   .AddAvatar(6).FinalizeDialog();
                                                    break;
                                                }//421199 CloudBacksword
                                                client.Inventory.Add(stream, 421199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2CloudBacksword 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Taoist. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(6).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(6).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 144:
                            case 134:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(1))
                                            {
                                                data.AddText("Please make 1 more space in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(6).FinalizeDialog();
                                                break;
                                            }
                                            if (client.Player.Reborn == 0)
                                            {//134089 RoyalGown
                                                client.Inventory.Add(stream, 134089, 1, 3, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)+3RoyalGown 1 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream)) break;
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
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Taoist. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(6).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(6).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                        .AddOption("Thank~you,~Master.", 255)
                                                                                                        .AddAvatar(6).FinalizeDialog();


                                    break;
                                }
                        }
                        break;
                    }
                case 50:
                    {
                        if (client.Player.Class > 101)
                        {
                            if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddText("Only Pure Fire Taoists, who have trained as Water Taoists for all 3 lives, can learn the Pure Skills, Azure Shield. If you`ve been Reborn twice, but are not a Pure Fire Taoist, you can take some")
                                    .AddText("~Oblivion Dew to the Reincarnation Seer in the Market and be Reincarnated! That way, you can obtain a new class combination. Oh, by the way, Chain Bolt is a XP skill, and it`ll allow you to throw")
                                    .AddText("~bolts of lightning at several targets during the XP time.")
                          .AddOption("AzureShield" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AzureShield) ? "(Learnt)" : "(Lv.40)") + "", 51)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();
                            }
                            else if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {
                                data.AddText("Only Pure Fire Taoists, who have trained as Fire Taoists for all 3 lives, can learn the Pure Skills, Heaven Blade. If you`ve been Reborn twice, but are not a Pure Fire Taoist, you can take some")
                                   .AddText("~Oblivion Dew to the Reincarnation Seer in the Market and be Reincarnated! That way, you can obtain a new class combination. Oh, by the way, Chain Bolt is a XP skill, and it`ll allow you to throw")
                                   .AddText("~bolts of lightning at several targets during the XP time.")
                         .AddOption("Learn~Heaven~Blade" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavenBlade) ? "(Learnt)" : "(Lv.40)") + "", 52)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(6).FinalizeDialog();
                            }
                        }


                        break;
                    }
                case 51:
                    {
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            if (client.Player.Reborn < 2)
                            {
                                data.AddText("Only pure Fire Taoists, who have trained as Water Taoists for all 3 of their lives, can learn the Pure Skill, AzureShield.")
                                                                .AddOption("Ok~I~see.", 255)
                                                                .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            if (!Database.AtributesStatus.IsWater(client.Player.FirstClass) || !Database.AtributesStatus.IsWater(client.Player.SecoundeClass))
                            {
                                data.AddText("Only pure water Taoists, who have trained as Water Taoists for all 3 of their lives, can learn the Pure Skill, AzureShield.")
                                                                                       .AddOption("Ok~I~see.", 255)
                                                                                       .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            if (client.Player.Level < 40)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.AzureShield;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 52:
                    {
                        if (Database.AtributesStatus.IsFire(client.Player.Class))
                        {
                            if (client.Player.Reborn < 2)
                            {
                                data.AddText("Only pure Fire Taoists, who have trained as Fire Taoists for all 3 of their lives, can learn the Pure Skill, Heaven Blade.")
                                                                .AddOption("Ok~I~see.", 255)
                                                                .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            if (!Database.AtributesStatus.IsFire(client.Player.FirstClass) || !Database.AtributesStatus.IsFire(client.Player.SecoundeClass))
                            {
                                data.AddText("Only pure Fire Taoists, who have trained as Fire Taoists for all 3 of their lives, can learn the Pure Skill, Heaven Blade.")
                                                                                       .AddOption("Ok~I~see.", 255)
                                                                                       .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            if (client.Player.Level < 40)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.HeavenBlade;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("I know many types of skills and spells for a true Taoist, like you! Which of them do you want to learn, first?")
                          .AddOption("Learn~magic.", 11)
                          .AddOption("Learn~XP~Skill.", 12)
                            //.AddOption("Universal~skills.", 13)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        data.AddText("The Taoist`s skills are varied and powerful. Which skill do you want to learn?")
                        .AddOption("ChainBolt" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt) ? "(Learnt)" : "(Lv.1)") + "", 40)
                        .AddOption("Lightning" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning) ? "(Learnt)" : "(Lv.3)") + "", 41)
                        .AddOption("Vulcano" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano) ? "(Learnt)" : "(Lv.40)") + "", 42);
                        if (client.Player.Class > 101)
                        {
                            if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddOption("Revive" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Revive) ? "(Learnt)" : "[Lv.40]") + "", 43);
                            }
                        }
                        data.AddOption("SpeedLightning" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning) ? "(Learnt)" : "[Lv.40]") + "", 44);
                        data.AddOption("I`ll~think~about~it.", 255).AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 44:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SpeedLightning;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 43:
                    {
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            if (client.Player.Level < 40)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.Revive;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 42:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Vulcano;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (client.Player.Level < 3)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 3 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Lightning;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 40:
                    {
                        var SpellID = Role.Flags.SpellID.ChainBolt;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 58:
                    {
                        if (client.Player.Level < 93)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireofHell;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("The Taoist`s skills are varied and powerful. Which skill do you want to learn?")
                           .AddOption("Thunder" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder) ? "(Learnt)" : "(Lv.1)") + "", 20)
                           .AddOption("Cure" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure) ? "(Learnt)" : "(Lv.1)") + "", 21)
                           .AddOption("Meditation" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation) ? "(Learnt)" : "(Lv.44)") + "", 22);
                        if (client.Player.Class >= 101)
                        {
                            data.AddOption("ChainBolt" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt) ? "(Learnt)" : "[Lv.1]") + "", 70)
                                .AddOption("Lightning" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning) ? "(Learnt)" : "[Lv.1]") + "", 71);
                            if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {

                                data.AddOption("Fire" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fire) ? "(Learnt)" : "[Lv.40][Thunder~Level~4]") + "", 23)
                                    .AddOption("Tornado" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tornado) ? "(Learnt)" : "[Lv.92][Fire~Level~3]") + "", 24)
                                    .AddOption("FireRing" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireRing) ? "(Learnt)" : "(Lv.55)") + "", 25)
                                    .AddOption("FireMeteor" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireMeteor) ? "(Learnt)" : "(Lv.52)") + "", 26)
                                    .AddOption("FireCircle" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireCircle) ? "(Learnt)" : "(Lv.65)") + "", 27)
                                   .AddOption("Bomb" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Bomb) ? "(Learnt)" : "(Lv.82)") + "", 28)
                               .AddOption("FireofHell" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireofHell) ? "(Learnt)" : "(Lv.93)") + "", 58);
                            }
                            else if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddOption("HealingRain" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingRain) ? "(Learnt)" : "(Lv.40)") + "", 29)
                                    .AddOption("StarofAccuracy" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarofAccuracy) ? "(Learnt)" : "(Lv.45)") + "", 30)
                                    .AddOption("MagicShield" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield) ? "(Learnt)" : "(Lv.50)") + "", 31)
                                    .AddOption("Stigma" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma) ? "(Learnt)" : "(Lv.55)") + "", 32)
                                    .AddOption("Invisibility" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Invisibility) ? "(Learnt)" : "(Lv.60)") + "", 33)
                                    .AddOption("Pray" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Pray) ? "(Learnt)" : "(Lv.70)") + "", 34)
                                    .AddOption("AdvancedCure" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdvancedCure) ? "(Learnt)" : "(Lv.81)") + "", 35)
                                    .AddOption("Nectar" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Nectar) ? "(Learnt)" : "(Lv.94)") + "", 36);
                            }
                        }
                        data.AddOption("I`ll~think~about~it.", 255).AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 70:
                    {
                        var SpellID = Role.Flags.SpellID.ChainBolt;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 71:
                    {
                        var SpellID = Role.Flags.SpellID.Lightning;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 28:
                    {
                        if (client.Player.Level < 82)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 82 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Bomb;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 27:
                    {
                        if (client.Player.Level < 65)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 65 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireCircle;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 26:
                    {
                        if (client.Player.Level < 52)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 52 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireMeteor;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 25:
                    {
                        if (client.Player.Level < 55)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 55 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireRing;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 24:
                    {
                        if (client.Player.Level < 92)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 92 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        MsgServer.MsgSpell spell;
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Fire, out spell))
                        {
                            if (spell.Level == 3)
                            {
                                var SpellID = Role.Flags.SpellID.Tornado;
                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry. but you need thunder level 4.")
                                                     .AddOption("Okay.~I~see..", 255)
                                                     .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry. but you need thunder level 4.")
                                                 .AddOption("Okay.~I~see..", 255)
                                                 .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 23:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        MsgServer.MsgSpell spell;
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Thunder, out spell))
                        {
                            if (spell.Level == 4)
                            {
                                var SpellID = Role.Flags.SpellID.Fire;
                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry. but you need thunder level 4.")
                                                     .AddOption("Okay.~I~see..", 255)
                                                     .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry. but you need thunder level 4.")
                                                 .AddOption("Okay.~I~see..", 255)
                                                 .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 36:
                    {
                        if (client.Player.Level < 94)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 94 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Nectar;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 35:
                    {
                        if (client.Player.Level < 81)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 81 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.AdvancedCure;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 34:
                    {
                        if (client.Player.Level < 70)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Pray;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 33:
                    {
                        if (client.Player.Level < 60)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 60 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Invisibility;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 32:
                    {
                        if (client.Player.Level < 55)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 55 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Stigma;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (client.Player.Level < 50)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 50 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MagicShield;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 30:
                    {
                        if (client.Player.Level < 45)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 45 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.StarofAccuracy;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 29:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.HealingRain;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.Level < 44)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 44 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Meditation;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        var SpellID = Role.Flags.SpellID.Cure;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 20:
                    {
                        var SpellID = Role.Flags.SpellID.Thunder;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("The Taoist are capable of casting devastating skills and deadly spells, and proved to be versatile. Do you want to be a Taoist, now?")
                          .AddOption("Yeah,~please!", 2)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Class == 100)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChainBolt);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);
                            client.Player.Class++;
                            data.AddText("You`ve been promoted and learned corresponding XP skill. The system will notify you when your XP is full. Keep up with the good work!")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(6).FinalizeDialog();

                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("There are two paths of the Taoist, the Fire and the Water. Which one would you choose?")
                          .AddOption("Fire~Taoist.", 4)
                          .AddOption("Water~Taoist.", 5)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                           .AddOption("Let me check.", 255)
                                           .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 101)
                        {
                            client.Inventory.Add(stream, 421079, 1);
                            client.SendSysMesage("You received 1 (Super)EndBacksword.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                            client.Player.Class = 142;
                            data.AddText("Good, you`ve finished the training as a Fire Taoist. Hope you can play the role of a saver in your groups well!")
                         .AddOption("Thank~you,~Master.", 255)
                         .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                           .AddOption("Let me check.", 255)
                                           .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 101)
                        {
                            client.Inventory.Add(stream, 421079, 1);
                            client.SendSysMesage("You received 1 (Super)EndBacksword.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                            client.Player.Class = 132;
                            data.AddText("Good, you`ve finished the training as a Water Taoist. Hope you can play the role of a saver in your groups well!")
              .AddOption("Thank~you,~Master.", 255)
              .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PromotionMonk)]
        public static void PromotionMonk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 60 && client.Player.Class <= 65))
            {
                data.AddText("Sorry, but you are not a Monk.").AddOption("Alright.", 255).AddAvatar(226).FinalizeDialog();
                return;
            }


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);


            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("Monk and Saints have a deep understanding of both the physical and spiritual world! Through intense training, Monk and Saints have gained devastating power. What is it, my child?")
                    .AddOption("Learn~about~Promotion.", 110);
                            if (client.Player.Class == 60)
                                data.AddOption("I~want~to~get~promoted", 1);
                            else
                                data.AddOption("I~want~to~get~promoted", 22);
                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 60)
                        {
                            data.AddText("Monk and Saints have a deep understanding of both the physical and spiritual world! Through intense training, Monk and Saints have gained devastating power. What is it, my child?")
                            .AddOption("I~want~to~get~promoted", 1)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Monk and Saints have a deep understanding of both the physical and spiritual world! Through intense training, Monk and Saints have gained devastating power. What is it, my child?")
                               .AddOption("I~want~to~get~promoted.", 22)
                               .AddOption("Learn~class~skills.", 3)
                               .AddOption("Learn~Pure~Skills.", 20)
                               .AddOption("Epic Quest", 90)
                               .AddOption("Nothing.", 255)
                               .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet Monk will be a big name in a short time. Listen, you can learn class skills and")
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
                        if (client.Player.Class == 60)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else
                            data.AddOption("I~want~to~get~promoted", 22);
                        data.AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 90:
                    {
                        data.AddText("The alleviate miseries in the mortal world, Golden Cicada is recruiting tenacious Monks and Saints to obtain")
                            .AddText(" sacred sutrs in the Western Fairyland. The journey is full of trials and tribolations, but the heroes who go")
                            .AddText(" through them will be rewarded with a [Boundless Heart] and obtain the core secret of forging Monk`s Epic Weapon.")
                           .AddOption("Head to see Golden Cicada", 91)
                           .AddOption("Secret is secret.", 255)
                           .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 91:
                    {
                        if (client.Player.Level >= 70 && client.Player.Reborn >= 2)
                        {
                            client.Teleport(89, 71, 3983);
                        }
                        else
                        {
                            data.AddText("Please come back when you will be 2nd Reborn and with a higher level than 70.")
                                                                        .AddOption("Okay.", 255)
                                                                        .AddAvatar(226).FinalizeDialog();
                        }


                        break;
                    }
                case 22:
                    {

                        switch (client.Player.Class)
                        {
                            case 61:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(226).FinalizeDialog();
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
                                                                   .AddAvatar(226).FinalizeDialog();
                                                    break;
                                                }//610070 BeadsOfWisdom
                                                client.Inventory.Add(stream, 610079, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 2 (Super)BeadsOfWisdom.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Monk. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(226).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(226).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 62:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(226).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(226).FinalizeDialog();
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
                                                                   .AddAvatar(226).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 136069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)CloudFrock and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Monk. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(226).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(226).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 63:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(226).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(226).FinalizeDialog();
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
                                                                   .AddAvatar(226).FinalizeDialog();
                                                    break;
                                                }//610199 BeadsOfConcentration
                                                client.Inventory.Add(stream, 610199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2BeadsOfConcentration 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Monk. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(226).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(226).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 64:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(226).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(226).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(1))
                                            {
                                                data.AddText("Please make 1 more space in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(226).FinalizeDialog();
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
                                            if (!client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream)) break;
                                            client.SendSysMesage("You received DragonBallScroll.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Monk. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(226).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(226).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(226).FinalizeDialog();


                                    break;
                                }
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Only a Pure Monk/Saint can learn the skill, Soul Shackle. If you`ve been reborn twice, but are not Pure, you can take Oblivion Dew to the Reincarnation Seer in the Market")
                            .AddText("~and ask him to help you get Reincarnated. That way, you can obtain a new class combination.")
                            .AddOption("I~want~to~learn~it~now.", 21)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("You can only learn the Pure Skill `Soul Shackle` when you`ve been reborn twice, and have become a Pure Monk or Saint.")
                                                            .AddOption("Ok~I~see.", 255)
                                                            .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        if (!Database.AtributesStatus.IsMonk(client.Player.FirstClass) || !Database.AtributesStatus.IsMonk(client.Player.SecoundeClass))
                        {
                            data.AddText("You can only learn the Pure Skill `Soul Shackle` when you`ve been reborn twice, and have become a Pure Monk or Saint.")
                                                                                   .AddOption("Ok~I~see.", 255)
                                                                                   .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level < 40 || client.Player.Class < 62)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SoulShackle;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("The Monk`s skills are varied and powerful. Which skill do you want to learn?")
                            .AddOption("Deflection~Aura" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FendAura) ? "(Learnt)" : "(Lv.20)") + "", 4)
                            .AddOption("Tyrant~Aura" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TyrantAura) ? "(Learnt)" : "(Lv.20)") + "", 5)
                            .AddOption("Serenity" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Serenity) ? "(Learnt)" : "(Lv.40)") + "", 6)
                            .AddOption("Radiant~Palm" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RadiantPalm) ? "(Learnt)" : "(Lv.40)") + "", 7)
                            .AddOption("Tranquility" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tranquility) ? "(Learnt)" : "(Lv.70)") + "", 8)
                            .AddOption("Next~page.", 9)
                            .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        data.AddText("The Monk`s skills are varied and powerful. Which skill do you want to learn?")
                            .AddOption("Compassion" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Compassion) ? "(Learnt)" : "(Lv.100)") + "", 10)
                            .AddOption("ElementalAuraMetal" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MetalAura) ? "(Learnt)" : "(Lv.100)") + "", 11)
                            .AddOption("ElementalAuraWood" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WoodAura) ? "(Learnt)" : "(Lv.100)") + "", 12)
                            .AddOption("ElementalAuraWater" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WatherAura) ? "(Learnt)" : "(Lv.100)") + "", 13)
                            .AddOption("ElementalAuraFire" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireAura) ? "(Learnt)" : "(Lv.100)") + "", 14)
                            .AddOption("ElementalAuraEarth" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthAura) ? "(Learnt)" : "(Lv.100)") + "", 15)
                            .AddOption("Next~page.", 70)
                            .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 70:
                    {
                        data.AddText("The Monk`s skills are varied and powerful. Which skill do you want to learn?")
                          .AddOption("WhirlwindKick" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlwindKick) ? "(Learnt)" : "(Lv.1)") + "", 71)
                          .AddOption("TripleAttack" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack) ? "(Learnt)" : "(Lv.1)") + "", 72)
                          .AddOption("Oblivion" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Oblivion) ? "(Learnt)" : "(Lv.1)") + "", 73)
                          .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 71:
                    {
                        var SpellID = Role.Flags.SpellID.WhirlwindKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 72:
                    {
                        var SpellID = Role.Flags.SpellID.TripleAttack;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 73:
                    {
                        var SpellID = Role.Flags.SpellID.Oblivion;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 64)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.EarthAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 64)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 64)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.WatherAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 64)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.WoodAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 64)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MetalAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 64)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Compassion;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 63)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Tranquility;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 62)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.RadiantPalm;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 62)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Serenity;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.Level < 20 || client.Player.Class < 61)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 20 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.TyrantAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.Level < 20 || client.Player.Class < 61)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 20 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FendAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("You`ve met the requirement for job promotion. You can get promoted from Intern to a Monk and learn `Whirlwind Kick` when you are Level 15 or above.")
                          .AddOption("Get~promoted.", 2)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Class == 60)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlwindKick))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WhirlwindKick);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleAttack);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Oblivion))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Oblivion);
                            client.Player.Class++;
                            data.AddText("You`ve been promoted and learned corresponding XP skill. The system will notify you when your XP is full. Keep up with the good work!")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(226).FinalizeDialog();

                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PromotionNinja)]
        public static void PromotionNinja(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 50 && client.Player.Class <= 55))
            {
                data.AddText("Sorry, but you are not a Ninja.").AddOption("Alright.", 255).AddAvatar(203).FinalizeDialog();
                return;
            }


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("Cloaked in shadow, mysterious and aloof, the Ninja is the fastest class, with the most devastating attacks! So, what can I do for you?")
                    .AddOption("Learn~about~Promotion.", 110);
                            if (client.Player.Class == 50)
                                data.AddOption("I~want~to~get~promoted", 1);
                            else
                                data.AddOption("I~want~to~get~promoted", 20);
                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 50)
                        {
                            data.AddText("Cloaked in shadow, mysterious and aloof, the Ninja is the fastest class, with the most devastating attacks! So, what can I do for you?")
                            .AddOption("I~want~to~get~promoted", 1)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Cloaked in shadow, mysterious and aloof, the Ninja is the fastest class, with the most devastating attacks! So, what can I do for you?")
                               .AddOption("I~want~to~get~promoted.", 20)
                               .AddOption("Learn~Katana~skills.", 6)
                               .AddOption("Learn~Scythe~skills.", 8)
                                   .AddOption("Learn~XP~skills.", 11)
                                   .AddOption("Epic~Quest.~(L70,~2nd~reborn)", 80)
                                  .AddOption("Learn~class~skills.", 3)
                                       .AddOption("Learn~Pure~Skills.", 15)
                                            .AddOption("You~bet!", 255)
                               .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Ninja` will be a big name in a short time. Listen, you can learn class skills and")
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
                        if (client.Player.Class == 50)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else
                            data.AddOption("I~want~to~get~promoted", 20);
                        data.AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 80:
                    {
                        data.AddText("The Divine Shadow has opened the Infinite Abyss and set 3 trials for Ninjas who have reached Level 70 of the 2nd rebirth.")
                            .AddText("~If you`re able to complete the trials, you`ll acquire the legendary ninja skills, and also obtain a Life`s Eye.")
                            .AddText("~This mythical eye can be used to transform your weapon into an Epic weapon.")
                          .AddOption("Enter~the~Infinite~Abyss.", 81)
                           .AddOption("Learn~Epic~Spells.", 82)
                          .AddOption("Sounds~crazy.", 255)
                          .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 82:
                    {
                        data.AddText("I change you ShadowScroll for ShadowClone spell, TwilightScroll for TwilightDance or FatalScroll for FatalSpin.")
                           .AddText("Your are found this in Epic~Quest, when you missing stage 2 ore 3.")
                      .AddOption("ShadowClone.", 83)
                       .AddOption("TwilightDance.", 84)
                      .AddOption("FatalSpin", 85)
                    .AddOption("SuperTwofoldBlade", 86)
                      .AddOption("Sounds~crazy.", 255)
                      .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 86:
                    {
                        //if (client.Equipment.UseEpicNinjaWeapon())
                        {
                            var SpellID = Role.Flags.SpellID.SuperTwofoldBlade;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Remove(3004461, 1, stream)) break;
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        //    else
                        //    {
                        //        data.AddText("You need to equip EpicWeapon, first.")
                        //.AddOption("Alright.", 255)
                        //.FinalizeDialog();
                        //    }


                        break;
                    }
                case 85:
                    {
                        //if (client.Inventory.Contain(3004461, 1))
                        {

                            var SpellID = Role.Flags.SpellID.FatalSpin;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Remove(3004461, 1, stream)) break;
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        //                   else
                        //                   {
                        //                       data.AddText("Sorry, you not have the FatalScroll in your inventory.")
                        //.AddOption("Okay.", 255)
                        //.AddAvatar(203).FinalizeDialog();
                        //                   }


                        break;
                    }
                case 84:
                    {
                        //if (client.Inventory.Contain(3004460, 1))
                        {

                            var SpellID = Role.Flags.SpellID.TwilightDance;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Remove(3004460, 1, stream)) break;
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        //                   else
                        //                   {
                        //                       data.AddText("Sorry, you not have the TwilightScroll in your inventory.")
                        //.AddOption("Okay.", 255)
                        //.AddAvatar(203).FinalizeDialog();
                        //                   }

                        break;
                    }
                case 83:
                    {
                        //if (client.Inventory.Contain(3004459, 1))
                        //{

                        var SpellID = Role.Flags.SpellID.ShadowClone;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(3004459, 1, stream)) break;
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();
                        //}
                        //                   else
                        //                   {
                        //                       data.AddText("Sorry, you not have the ShadowScroll in your inventory.")
                        //.AddOption("Okay.", 255)
                        //.AddAvatar(203).FinalizeDialog();
                        //                   }

                        break;
                    }
                case 81:
                    {
                        if (client.Player.Reborn >= 2 && client.Player.Level >= 70)
                        {
                            //32 35
                            client.Teleport(32, 35, 3851, 0);
                        }
                        else
                        {
                            data.AddText("Only Ninjas who have reached Level 70 of their 2nd rebirth are available for the Infinite Abyss.")
                              .AddOption("Alright.", 255)
                              .AddAvatar(203).FinalizeDialog();

                        }


                        break;
                    }
                case 20:
                    {
                        switch (client.Player.Class)
                        {
                            case 51:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(203).FinalizeDialog();
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
                                                                   .AddAvatar(203).FinalizeDialog();
                                                    break;
                                                }//601079 MoonKatana
                                                client.Inventory.Add(stream, 601079, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 2 (Super)MoonKatana.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Ninja. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(203).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(203).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 52:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(203).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
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
                                                                    .AddAvatar(203).FinalizeDialog();
                                                    break;
                                                }//135069 TigerVest
                                                client.Inventory.Add(stream, 135069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)TigerVest and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Ninja. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(203).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(203).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 53:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(203).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
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
                                                                   .AddAvatar(203).FinalizeDialog();
                                                    break;
                                                }//601199 LordKatana
                                                client.Inventory.Add(stream, 601199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2LordKatana 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Ninja. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(203).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(203).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 54:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(203).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(1))
                                            {
                                                data.AddText("Please make 1 more space in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(203).FinalizeDialog();
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
                                            if (!client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream)) break;
                                            client.SendSysMesage("You received DragonBallScroll.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Ninja. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(203).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(203).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(203).FinalizeDialog();

                                    break;
                                }
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("You are over level 15. You can now get promoted to Ninja and learn Fatal Strike (XP skill).")
                            .AddOption("Yeah,~please!", 2)
                            .AddOption("Let~me~think~it~over.", 255)
                            .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        if (client.Player.Class == 50)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalStrike))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalStrike);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                            client.Player.Class++;
                            data.AddText("You`ve been promoted and learned corresponding XP skill. The system will notify you when your XP is full. Keep up with the good work!")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        data.AddText("Only Pure Ninjas can learn the Pure Skill, Counter Kill. If you`ve been Reborn twice, but are not a Pure Ninja, you can take some Oblivion Dew to the Reincarnation Seer in the Market and be")
                            .AddText("~Reincarnated! That way, you can obtain a new class combination.")
                 .AddOption("I~want~to~learn~it~now.", 16).AddOption("I`ll~think~about~it.", 255)
                 .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("Only pure Ninja, who have spent all 3 lives as a Ninja, can learn the Pure Skill, Counter Kill.")
                                                            .AddOption("Ok~I~see.", 255)
                                                            .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        if (!Database.AtributesStatus.IsNinja(client.Player.FirstClass) || !Database.AtributesStatus.IsNinja(client.Player.SecoundeClass))
                        {
                            data.AddText("Only pure Ninja, who have spent all 3 lives as a Ninja, can learn the Pure Skill, Counter Kill.")
                                                                                   .AddOption("Ok~I~see.", 255)
                                                                                   .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level < 40 || client.Player.Class < 52)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.CounterKill;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("Which skill do you want to learn?")
              .AddOption("ShurikenVortex" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShurikenVortex) ? "(Learnt)" : "(Lv.70)") + "", 12)
                       .AddOption("FatalStrike" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalStrike) ? "(Learnt)" : "(Lv.3)") + "", 13)
              .AddOption("I`ll~think~about~it.", 255)
              .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 3)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 3 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FatalStrike;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 53)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ShurikenVortex;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        data.AddText("Which skill do you want to learn?")
                  .AddOption("MortalDrag" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalDrag) ? "(Learnt)" : "(Lv.20)") + "", 9)
                           .AddOption("BloodyScythe" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodyScythe) ? "(Learnt)" : "(Lv.40)") + "", 10)
                           .AddOption("GapingWounds" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodyScythe) ? "(Learnt)" : "(Lv.100)") + "", 87)
                  .AddOption("I`ll~think~about~it.", 255)
                  .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 87:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 52)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.GapingWounds;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 52)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BloodyScythe;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level < 20 || client.Player.Class < 51)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 20 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MortalDrag;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;

                    }

                case 6:
                    {
                        data.AddText("Which skill do you want to learn?")
                  .AddOption("TwofoldBlades" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwofoldBlades) ? "(Learnt)" : "(Lv.40)") + "", 7)
                  .AddOption("I`ll~think~about~it.", 255)
                  .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 52)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.TwofoldBlades;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("Which skill do you want to learn?")
                            // .AddOption("Poison~Star" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar) ? "(Learnt)" : "(Lv.70)") + "", 4)
                 .AddOption("Archer~Bane" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArcherBane) ? "(Learnt)" : "(Lv.100)") + "", 5)
                 .AddOption("ToxicFog" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog) ? "(Learnt)" : "(Lv.15)") + "", 14)
                 .AddOption("Okay.~I~see.", 255)
                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 51)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ToxicFog;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        if (client.Player.Level < 110 || client.Player.Class < 55)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 110 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ArcherBane;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.Reborn == 0)
                        {
                            data.AddText("Only reborn Ninja-ninja, who have spent all 2 lives as Ninja, can learn the that Skill, PoisonStar.")
                                                       .AddOption("Ok~I~see.", 255)
                                                       .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Reborn == 1 && client.Player.FirstClass == 55 || client.Player.Reborn == 2 && client.Player.SecoundeClass == 55)
                        {
                            if (client.Player.Level < 70 || client.Player.Class < 53)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.PoisonStar;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Only reborn Ninja-ninja, who have spent all 2 lives as Ninja, can learn the that Skill, PoisonStar.")
                                                     .AddOption("Ok~I~see.", 255)
                                                     .AddAvatar(203).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
    }
}
