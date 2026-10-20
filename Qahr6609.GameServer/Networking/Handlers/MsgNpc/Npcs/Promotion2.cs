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
        [NpcAttribute(NpcID.PromotionWarrior)]
        public static void PromotionWarrior(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 20 && client.Player.Class <= 25))
            {
                data.AddText("Sorry, but you are not a Warrior.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                return;
            }
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("To be a good Warrior, you must know your weapon, your armor be a 2nd skin, and use your shield to ward off your enemy! Remember, fortune favors the brave, and you must keep your head, even in the darkest hours. So, what can I do for you?.")
                    .AddOption("Learn~about~Promotion.", 110);
                            if (client.Player.Class == 20)
                                data.AddOption("I~want~to~get~promoted", 1);
                            else
                                data.AddOption("I~want~to~get~promoted", 3);
                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Class == 20)
                        {
                            data.AddText("To be a good Warrior, you must know your weapon, your armor be a 2nd skin, and use your shield to ward off your enemy! Remember, fortune favors the brave, and you must keep your head, even in the darkest hours. So, what can I do for you?.")
                            .AddOption("I~want~to~get~promoted", 1)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("To be a good Warrior, you must know your weapon, your armor be a 2nd skin, and use your shield to ward off your enemy! Remember, fortune favors the brave, and you must keep your head, even in the darkest hours. So, what can I do for you?")
                               .AddOption("I~want~to~get~promoted.", 3)
                                /*.AddOption("Learn~class~skills.", 5)
                                 * Avatar= 0  There are 3 superior skills. Which skill do you want to learn?Avatar= 0  Summon~Guard~[Lvl~15] Avatar= 0  Fire~Evil~[Lvl~40]Avatar= 0  Let~me~think.
                                   
                                 * */
                                  .AddOption("Learn~skills~for~Fist.", 130)
                               .AddOption("Learn~shield~skills.", 4)

                               .AddOption("Learn~XP~skills.", 10)
                                   .AddOption("Learn~weapon~skills.", 20)
                                       .AddOption("Learn~Pure~Skills.", 21)
                                            .AddOption("You~bet!", 255)
                               .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }
                case 130:
                    {
                        data.AddText("If you`re interested in the skills for Fist, I`m glad to give some guidance.")
                                     .AddText("[Wave of Blood]: Active skill for Fist. Attack targets and restore XP.\n[Backfire]: Active skill for Fist. Reflect the damage received and restore Stamina.\n[Scar of Earth]: Passive skill for Fist.")
                                     .AddText("~Attack targets and inflicts bleeding.\n [Twist of War]: Passive skill for Fist. Retrieve XP status after revival.\n[Maniac Dance]: XP skill for Fist. Immune to physical attack and inflict damage.")
                           .AddOption("Scar~of~Earth.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScarofEarth) ? "(Acquired)" : "(Lv.40)") + "", 131)
                            .AddOption("Wave~of~Blood.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaveofBlood) ? "(Acquired)" : "(Lv.40)") + "", 132)
                            .AddOption("Maniac~Dance.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ManiacDance) ? "(Acquired)" : "(Lv.40)") + "", 133)
                             .AddOption("Twist~of~War.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar) ? "(Acquired)" : "(Lv.70)") + "", 134)
                           .AddOption("Backfire.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar) ? "(Acquired)" : "(Lv.100)") + "", 135)
                             .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 135:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Backfire;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 134:
                    {
                        if (client.Player.Level < 70)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.TwistofWar;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 133:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ManiacDance;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 132:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.WaveofBlood;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 131:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ScarofEarth;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Warrior` will be a big name in a short time. Listen, you can learn class skills and")
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
                        if (client.Player.Class == 20)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else
                            data.AddOption("I~want~to~get~promoted", 3);
                        data.AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        switch (client.Player.Class)
                        {
                            case 21:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(8).FinalizeDialog();
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
                                                                   .AddAvatar(8).FinalizeDialog();
                                                    break;
                                                }//900009 SoftShield
                                                client.Inventory.Add(stream, 900009, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 480079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)SoftShield and 1 (Super)IronClub.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(8).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(8).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 22:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(8).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(8).FinalizeDialog();
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
                                                                   .AddAvatar(8).FinalizeDialog();
                                                    break;
                                                }//131069 LightArmor
                                                client.Inventory.Add(stream, 131069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)LightArmor and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(8).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(8).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 23:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(8).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(8).FinalizeDialog();
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
                                                                   .AddAvatar(8).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 561199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2CopperWand 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(8).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(8).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 24:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(8).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(8).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(1))
                                            {
                                                data.AddText("Please make 1 more space in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(8).FinalizeDialog();
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
                                            data.AddText("Excellent! You`ve learned ways of the Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(8).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(8).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(8).FinalizeDialog();

                                    break;
                                }
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("Only Warriors who`ve trained for all three lives can learn the Pure Skill, Perseverance. If you`ve been Reborn twice, but are not a Pure Warrior, you take some Oblivion Dew to the Reincarnation Seer in the Market and be")
                            .AddText("~Reincarnated! That way, you can obtain a new class combination.")
                                                       .AddOption("I~want~to~learn~it~now.", 22).AddOption("I`ll~think~about~it.", 255)
                                                       .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 22:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("Only pure Warriors, who have spent all 3 lives as a Warrior, can learn the Pure Skill, Perseverance.")
                                                            .AddOption("Ok~I~see.", 255)
                                                            .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        if (!Database.AtributesStatus.IsWarrior(client.Player.FirstClass) || !Database.AtributesStatus.IsWarrior(client.Player.SecoundeClass))
                        {
                            data.AddText("Only pure Warriors, who have spent all 3 lives as a Warrior, can learn the Pure Skill, Perseverance.")
                                                                                   .AddOption("Ok~I~see.", 255)
                                                                                   .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level < 40 || client.Player.Class < 22)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Perseverance;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                        {
                            data.AddText("You`ve learned Fast Blade and Scented Sword!")
                                                             .AddOption("Okay.", 255)
                                                             .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            string line = "You just have learned the skill ";
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                                line += Role.Flags.SpellID.FastBlader.ToString();
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                line += "/" + Role.Flags.SpellID.ScrenSword.ToString();
                            }
                            data.AddText(line)
                                            .AddOption("Thank you..", 255)
                                            .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }
                case 10:
                    {
                        data.AddText("Which skill do you want to learn?")
                            .AddOption("Superman" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman) ? "(Learnt)" : "(Lv.3)") + "", 11)
                            .AddOption("Accuracy" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy) ? "(Learnt)" : "(Lv.3)") + "", 12)
                            .AddOption("Roar" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar) ? "(Learnt)" : "(Lv.3)") + "", 13)
                            .AddOption("Shield" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield) ? "(Learnt)" : "(Lv.3)") + "", 14)
                              .AddOption("FlyingMoon" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlyingMoon) ? "(Learnt)" : "(Lv.40)") + "", 15)
                            .AddOption("Let~me~think~it~over.", 255)
                            .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        var SpellID = Role.Flags.SpellID.FlyingMoon;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 14:
                    {
                        var SpellID = Role.Flags.SpellID.Shield;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        var SpellID = Role.Flags.SpellID.Roar;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        var SpellID = Role.Flags.SpellID.Accuracy;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        var SpellID = Role.Flags.SpellID.Superman;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("Which skill do you want to learn?")
                            .AddOption("Dash" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Dash) ? "(Learnt)" : "(Lv.40)") + "", 5)//40
                            .AddOption("Shield~Block" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShieldBlock) ? "(Learnt)" : "(Lv.40)") + "", 6)//40
                            .AddOption("Magic~Defender" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender) ? "(Learnt)" : "(Lv.70)") + "", 7)//level 70
                            .AddOption("Defensive~Stance" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DefensiveStance) ? "(Learnt)" : "(Lv.70)") + "", 8)//level 70
                            .AddOption("Let~me~think~it~over.", 255)
                            .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 22)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Dash;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 22)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ShieldBlock;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 23)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MagicDefender;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 23)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DefensiveStance;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("I`m proud that you made a mature decision! The warriors are adept in the use of weapons and shields for additional defense. Do you want to get promoted, now?")
                           .AddOption("Yeah,~please!", 2)
                           .AddOption("Let~me~think~it~over.", 255)
                           .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.Class == 20)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Superman);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                            }
                            client.Player.Class++;
                            data.AddText("You`ve learned ways of the Warrior. Now, more weapons and armors are available for you, while new skills are at your disposal. Go test your power!")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(8).FinalizeDialog();
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }


                        break;
                    }
            }
        }
        /// <summary>
        /// level 40 receive 1 blade and one club super
        /// level 70 receive 1 ring and one armor super
        /// level 100 receive 1 blade super +2 with 2 socket -1
        /// level 110 receive 1 club super +3 with 2 socket -1 and one DBScroll.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stream"></param>
        /// 


        [NpcAttribute(NpcID.PromotionWindWalker)]
        public static void PromotionWindWalker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 160 && client.Player.Class <= 165))
            {
                switch (Option)
                {
                    case 0:
                        {

                            data.AddText("You don`t look like a Windwalker. What`s the matter? If you were ever a Windwalker in either of the previous two lives, you can learn [Frost Gaze I] from me.");
                            if (client.Player.Reborn == 1)
                            {
                                if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass))
                                    data.AddOption("Frost~Gaze~II.", 41);
                                else if (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                    data.AddOption("Frost~Gaze~I.", 40);
                                else
                                    data.AddOption("Frost~Gaze~I.", 40);
                            }
                            else if (client.Player.Reborn == 2)
                            {
                                if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                    data.AddOption("Frost~Gaze~III.", 42);
                                else if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                                    data.AddOption("Frost~Gaze~II.", 41);
                                else if (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                    data.AddOption("Frost~Gaze~II.", 41);
                                else
                                    data.AddOption("Frost~Gaze~I.", 40);

                            }
                            else
                                data.AddOption("Frost~Gaze~I.", 40);

                            data.AddOption("I`ll~talk~to~you~later.", 255);
                            data.AddAvatar(333).FinalizeDialog();

                            break;
                        }
                    case 42:
                        {
                            if (client.Player.Reborn >= 2
                                && Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                            {
                                var SpellID = Role.Flags.SpellID.FrostGazeIII;

                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                    data.AddOption("Okay.", 8);
                                    data.AddAvatar(333).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(333).FinalizeDialog();
                            }
                            else
                                client.CreateBoxDialog("You're not qualified to learn this skill.");
                            break;
                        }
                    case 41:
                        {
                            if (client.Player.Reborn >= 1
                                && (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass)
                                || Database.AtributesStatus.IsWindWalker(client.Player.Class) && (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                || Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                            {
                                var SpellID = Role.Flags.SpellID.FrostGazeII;

                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                    data.AddOption("Okay.", 8);
                                    data.AddAvatar(333).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(333).FinalizeDialog();
                            }
                            else
                                client.CreateBoxDialog("You're not qualified to learn this skill.");
                            break;
                        }
                    case 40:
                        {
                            if (client.Player.Reborn > 0 && (Database.AtributesStatus.IsWindWalker(client.Player.Class) || Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                            {
                                var SpellID = Role.Flags.SpellID.FrostGazeI;

                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                    data.AddOption("Okay.", 8);
                                    data.AddAvatar(333).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(333).FinalizeDialog();
                            }
                            else
                                client.CreateBoxDialog("You're not qualified to learn this skill.");
                            break;
                        }
                }
                return;
            }

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 3795);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)20032, client.Player.Class, 3802);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Hey, FellowBrother! You`re back. Are you okay? No hurry, the Chief has sent me a letter and told me the things.");
                            data.AddOption("So~what~can~we~do~now?", 60);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        data.AddText("Our Windwalker school was founded by Saint Wan on the principle of KINDNESS and JUSTICE. Once you decide to");
                        data.AddText("~join us as a Windwalker, you take the responsibility for the security of the country against alien invaders and devils.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Learn~about~Promotion.", 52);
                        data.AddOption("I~want~to~get~promoted.", 50);
                        data.AddOption("Learn~skills.", 8);
                        data.AddOption("Switch~to~another~branch.", 1);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 60:
                    {
                        data.AddText("Our school and the devil force have been at daggers drawn for centuries, but I don`t really get their purposes for");
                        data.AddText("~attacking us and intruding the imperial mausoleum this time. Now that we`ve completed the service in the mausoleum,");
                        data.AddText("~we should return to society. FellowBrother, set off to investigate and avenge the deaths of our fellows!");
                        data.AddOption("I~see.", 61);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (client.Inventory.HaveSpace(2))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.SendSysMesage("You`ve completed the class quest of Windwalker and received 2 Lv.40 Elite Pride Fans (B)!");
                                client.Inventory.Add(stream, 626078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You`ve completed the class quest of Windwalker and received 2 Lv.40 Elite Pride Fans (B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");
                        }
                        break;
                    }
                case 52:
                    {
                        data.AddText("You`re such a talent and so promising. The Windwalker shool will amaze the world soon.");
                        data.AddText("~Remember, you can learn class skills and get promoted here to start your life. Would you like");
                        data.AddText("~to learn something about your class promotion and quests?");
                        data.AddOption("Thanks.", 53);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 53:
                    {
                        data.AddText("Congratulations! You`ve graduated from the training quest. From now on, you are no longer a simple recruit, but a trained");
                        data.AddText("~soldier. You can sign up for your class quests with me when you`re Level 12, and get promoted at Level 15.");
                        data.AddText("~The world is filled with dangers, so be sure and take care of yourself!");
                        data.AddOption("Let~the~legend~begin!", 54);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 54:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 3005160);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                        }
                        data.AddText("The TC Vice Captain (TwinCity 384,251) is now damned busy. Hey, if you can give him a hand,");
                        data.AddText("~he will remember you for the rest of his life. Hah, just kidding. Anyway, would you like to help him?");
                        data.AddOption("Send~me~there.", 56);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 56:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        break;
                    }
                case 50:
                    {

                        switch (client.Player.Class)
                        {
                            case 160:
                                {
                                    if (client.Player.Level < 15)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 15 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.15.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.Class++;
                                        data.AddText("Excellent! You`ve learned ways of the Windwalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                          .AddOption("Thank~you,~Master.", 255)
                                                                          .AddAvatar(333).FinalizeDialog();
                                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);

                                    }
                                    break;
                                }
                            case 161:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 40 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.40.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
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
                                                                   .AddAvatar(333).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 626079, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received (Super)Pride Fan*2.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Windwalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(333).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 162:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 70 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.70.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
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
                                                client.Inventory.Add(stream, 101069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)MysticWindrobe and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the WindWalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(333).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 163:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 100 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.100.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
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
                                                                   .AddAvatar(333).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 626199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2PrideFan 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the WindWalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(333).FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText(Database.DataCore.AtributeStatus.InfoStr(client.Player.Class, (byte)client.Player.Level)).AddOption("Thank you..", 255).AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 164:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 110 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.110.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(1))
                                            {
                                                data.AddText("Please make 1 more space in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(333).FinalizeDialog();
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
                                            data.AddText("Excellent! You`ve learned ways of the WindWalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(333).FinalizeDialog();
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
                                                                                                         .AddAvatar(333).FinalizeDialog();

                                    break;
                                }
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("Our Windwalker school is divided into two branches: Chaser and Stomper. Chaser focuses on");
                        data.AddText("~ranged combat, while Stomper excels in melee combat. I`m here representing the Chief to");
                        data.AddText("~impart distinctive skills to the Windwalkers.");
                        data.AddOption("Learn~skills~of~Stomper.", 9);
                        data.AddOption("Learn~skills~of~Chaser.", 12);
                        data.AddOption("Learn~rebirth~skill.", 13);
                        data.AddOption("Learn~universal~skills.", 14);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("Frost Gaze is an exclusive rebirth skill for Windwalker. The heroes who got reborn to be Windwalker can learn Frost Gaze I. If both of your precious andd");
                        data.AddText("~current Class are Windwalker, you can learn Frost Gaze II. While for Windwalkers who were also a Windwalker in previous two lives, they can learn Frost Gaze III.");
                        if (client.Player.Reborn == 1)
                        {
                            if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass))
                                data.AddOption("Frost~Gaze~II.", 41);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                data.AddOption("Frost~Gaze~I.", 40);
                            else
                                data.AddOption("Frost~Gaze~I.", 40);
                        }
                        else if (client.Player.Reborn == 2)
                        {
                            if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                data.AddOption("Frost~Gaze~III.", 42);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                                data.AddOption("Frost~Gaze~II.", 41);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                data.AddOption("Frost~Gaze~II.", 41);
                            else
                                data.AddOption("Frost~Gaze~I.", 40);

                        }
                        else
                            data.AddOption("Frost~Gaze~I.", 40);

                        data.AddOption("I~want~to~learn~other~skills.", 8);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        if (client.Player.Reborn >= 2
                            && Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                        {
                            var SpellID = Role.Flags.SpellID.FrostGazeIII;

                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                data.AddOption("Okay.", 8);
                                data.AddAvatar(333).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                        }
                        else
                            client.CreateBoxDialog("You're not qualified to learn this skill.");
                        break;
                    }
                case 41:
                    {
                        if (client.Player.Reborn >= 1
                            && (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass)
                            || Database.AtributesStatus.IsWindWalker(client.Player.Class) && (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                            || Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                        {
                            var SpellID = Role.Flags.SpellID.FrostGazeII;

                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                data.AddOption("Okay.", 8);
                                data.AddAvatar(333).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                        }
                        else
                            client.CreateBoxDialog("You're not qualified to learn this skill.");
                        break;
                    }
                case 40:
                    {
                        if (client.Player.Reborn > 0 && (Database.AtributesStatus.IsWindWalker(client.Player.Class) || Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                        {
                            var SpellID = Role.Flags.SpellID.FrostGazeI;

                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                data.AddOption("Okay.", 8);
                                data.AddAvatar(333).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                        }
                        else
                            client.CreateBoxDialog("You're not qualified to learn this skill.");
                        break;
                    }
                case 14:
                    {
                        data.AddText("Omnipotence and Justice Chant are basic skills for Windwalker, which are available to both Chaser and Stomper learners. I`ll teach you if you want.");
                        data.AddOption("Omnipotence~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Omnipotence) ? "(Learned)" : "(Lv.3)") + "", 35);
                        data.AddOption("JusticeChant~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.JusticeChant) ? "(Learned)" : "(Lv.3)") + "", 36);
                        data.AddOption("I~want~to~learn~other~skills.", 8);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        var SpellID = Role.Flags.SpellID.JusticeChant;
                        if (client.Player.Level < 3)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 3.");
                            data.AddOption("I`ll~see~you~at~Lv.3!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 35:
                    {
                        var SpellID = Role.Flags.SpellID.Omnipotence;
                        if (client.Player.Level < 3)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 3.");
                            data.AddOption("I`ll~see~you~at~Lv.3!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("The Chaser branch is famous for its ability to control wind and outstanding ranged attacks.");
                        data.AddText("~FellowBrother, which skill would you like to learn?");
                        data.AddOption("SwirlingStorm~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SwirlingStorm) ? "(Learned)" : "(Lv.15)") + "", 30);
                        data.AddOption("ShadowofChaser~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShadowofChaser) ? "(Learned)" : "(Lv.15)") + "", 31);
                        data.AddOption("TripleBlasts~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleBlasts) ? "(Learned)" : "(Lv.40)") + "", 32);
                        data.AddOption("Thundercloud~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thundercloud) ? "(Learned)" : "(Lv.70)") + "", 33);
                        data.AddOption("Thunderbolt~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunderbolt) ? "(Learned)" : "(Lv.100)") + "", 34);
                        data.AddOption("I~want~to~learn~other~skills.", 8);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        var SpellID = Role.Flags.SpellID.Thunderbolt;
                        if (client.Player.Level < 100)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 100.");
                            data.AddOption("I`ll~see~you~at~Lv.100!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 33:
                    {
                        var SpellID = Role.Flags.SpellID.Thundercloud;
                        if (client.Player.Level < 70)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 70.");
                            data.AddOption("I`ll~see~you~at~Lv.70!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        var SpellID = Role.Flags.SpellID.TripleBlasts;
                        if (client.Player.Level < 40)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 40.");
                            data.AddOption("I`ll~see~you~at~Lv.40!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        var SpellID = Role.Flags.SpellID.ShadowofChaser;
                        if (client.Player.Level < 15)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 15.");
                            data.AddOption("I`ll~see~you~at~Lv.15!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 30:
                    {
                        var SpellID = Role.Flags.SpellID.SwirlingStorm;
                        if (client.Player.Level < 15)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 15.");
                            data.AddOption("I`ll~see~you~at~Lv.15!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        data.AddText("The Stomper branch has developed overwhelming melee skills which enable the caster to disable an enemy with swift and flexible fighting moves.");
                        data.AddText("~FellowBrother, which skill would you like to learn?");
                        data.AddOption("AngerofStomper~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngerofStomper) ? "(Learned)" : "(Lv.1)") + "", 20);
                        data.AddOption("BurntFrost~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BurntFrost) ? "(Learned)" : "(Lv.15)") + "", 21);
                        data.AddOption("HealingSnow~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingSnow) ? "(Learned)" : "(Lv.15)") + "", 22);
                        data.AddOption("RageofWar~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RageofWar) ? "(Learned)" : "(Lv.40)") + "", 23);
                        data.AddOption("HorrorofStomper~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HorrorofStomper) ? "(Learned)" : "(Lv.40)") + "", 24);
                        data.AddOption("ChillingSnow~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChillingSnow) ? "(Learned)" : "(Lv.70)") + "", 25);
                        data.AddOption("PeaceofStomper~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PeaceofStomper) ? "(Learned)" : "(Lv.70)") + "", 26);
                        data.AddOption("Next.", 10);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("The Stomper branch has developed overwhelming melee skills which enable the caster to disable an enemy with swift and flexible fighting moves.");
                        data.AddText("~FellowBrother, which skill would you like to learn?");
                        data.AddOption("RevengeTail~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeTail) ? "(Learned)" : "(Lv.100)") + "", 27);
                        data.AddOption("FreezingPelter~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FreezingPelter) ? "(Learned)" : "(Lv.100)") + "", 28);
                        data.AddOption("I~want~to~learn~other~skills.", 8);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddOption("Previous.", 9);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        var SpellID = Role.Flags.SpellID.FreezingPelter;
                        if (client.Player.Level < 100)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 100.");
                            data.AddOption("I`ll~see~you~at~Lv.100!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        var SpellID = Role.Flags.SpellID.RevengeTail;
                        if (client.Player.Level < 100)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 100.");
                            data.AddOption("I`ll~see~you~at~Lv.100!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        var SpellID = Role.Flags.SpellID.PeaceofStomper;
                        if (client.Player.Level < 70)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 70.");
                            data.AddOption("I`ll~see~you~at~Lv.70!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        var SpellID = Role.Flags.SpellID.ChillingSnow;
                        if (client.Player.Level < 70)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 70.");
                            data.AddOption("I`ll~see~you~at~Lv.70!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        var SpellID = Role.Flags.SpellID.HorrorofStomper;
                        if (client.Player.Level < 40)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 40.");
                            data.AddOption("I`ll~see~you~at~Lv.40!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        var SpellID = Role.Flags.SpellID.RageofWar;
                        if (client.Player.Level < 40)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 40.");
                            data.AddOption("I`ll~see~you~at~Lv.40!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        var SpellID = Role.Flags.SpellID.HealingSnow;
                        if (client.Player.Level < 15)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 15.");
                            data.AddOption("I`ll~see~you~at~Lv.15!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        var SpellID = Role.Flags.SpellID.BurntFrost;
                        if (client.Player.Level < 15)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 15.");
                            data.AddOption("I`ll~see~you~at~Lv.15!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        var SpellID = Role.Flags.SpellID.AngerofStomper;

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.CanChangeWindWalkerFree)
                        {
                            data.AddText("										*Branches of Windwalker School*");
                            data.AddText("¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª");
                            data.AddText("Stomper: Excels in melee combat with excellent attack range and remarkable recovery ability. Stomper`s exclusive skill");
                            data.AddText("		   can freeze enemies` blood to extend their skill cooldown time and response time.");
                            data.AddText("Chaser: Excels in ranged combat. With a unique focusing skill, Chaser is able to control the power of wind to attack");
                            data.AddText("		  enemies far away.");
                            data.AddText("¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª");
                            data.AddText("(You`re currently on the [RangedChaser] branch. Each Windwalker can switch his/her branch once every 30 days, free of charge.)");
                            if ((client.Player.MainFlag & Role.Player.MainFlagType.OnMeleeAttack) != Role.Player.MainFlagType.OnMeleeAttack)
                                data.AddOption("Switch~to~[MeleeStomper]~branch.", 2);
                            else
                                data.AddOption("Switch~to~[RangedStomper]~branch.", 3);
                            data.AddOption("I`ll~think~about~it.", 255);
                            data.AddAvatar(333).FinalizeDialog();

                        }
                        else
                        {
                            string myStomper = "MeleeStomper";
                            if ((client.Player.MainFlag & Role.Player.MainFlagType.OnMeleeAttack) != Role.Player.MainFlagType.OnMeleeAttack)
                                myStomper = "RangedStomper";

                            data.AddText("										*Branches of Windwalker School*");
                            data.AddText("¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª");
                            data.AddText("Stomper: Excels in melee combat with excellent attack range and remarkable recovery ability. Stomper`s exclusive skill");
                            data.AddText("		   can freeze enemies` blood to extend their skill cooldown time and response time.");
                            data.AddText("Chaser: Excels in ranged combat. With a unique focusing skill, Chaser is able to control the power of wind to attack");
                            data.AddText("		  enemies far away.");
                            data.AddText("¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª¡ª");
                            data.AddText("(You`re currently on the [" + myStomper + "] branch.)");
                            if ((client.Player.MainFlag & Role.Player.MainFlagType.OnMeleeAttack) != Role.Player.MainFlagType.OnMeleeAttack)
                            {
                                data.AddOption("Switch~to~[MeleeStomper]~branch.~(1WindCore)", 4);
                                data.AddOption("Switch~to~[MeleeStomper]~branch.~(27~CPs)", 5);
                            }
                            else
                            {
                                data.AddOption("Switch~to~[RangedChaser]~branch.~(1WindCore)", 6);
                                data.AddOption("Switch~to~[RangedChaser]~branch.~(27~CPs)", 7);
                            }
                            data.AddOption("I`ll~think~about~it.", 255);
                            data.AddAvatar(333).FinalizeDialog();

                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3301705, 1))
                        {
                            client.Player.SwitchWingWalkerAttack(stream);
                            if (!client.Inventory.Remove(3301705, 1, stream)) break;
                            data.AddText("Okay, I`ve changed your branch to [RangedChaser].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I can`t help you switch your Windwalker branch if you don`t have a Wind Core. You can exchange for one in the Horse Racing Store.");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.SwitchWingWalkerAttack(stream);
                            client.Player.ConquerPoints -= 27;
                            data.AddText("Okay, I`ve changed your branch to [RangedChaser].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I`m afraid I can`t help you switch the branch if you don`t have enough CPs.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3301705, 1))
                        {
                            client.Player.SwitchWingWalkerAttack(stream);
                            if (!client.Inventory.Remove(3301705, 1, stream)) break;
                            data.AddText("Okay, I`ve changed your branch to [MeleeStomper].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I can`t help you switch your Windwalker branch if you don`t have a Wind Core. You can exchange for one in the Horse Racing Store.");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            client.Player.SwitchWingWalkerAttack(stream);
                            client.Player.ConquerPoints -= 27;
                            data.AddText("Okay, I`ve changed your branch to [MeleeStomper].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I`m afraid I can`t help you switch the branch if you don`t have enough CPs.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (DateTime.Now > client.Player.CanChangeWindWalkerFree)
                        {
                            client.Player.CanChangeWindWalkerFree = DateTime.Now.AddDays(30);
                            client.Player.SwitchWingWalkerAttack(stream);
                            data.AddText("Okay, I`ve changed your branch to [RangedStomper].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (DateTime.Now > client.Player.CanChangeWindWalkerFree)
                        {
                            client.Player.CanChangeWindWalkerFree = DateTime.Now.AddDays(30);
                            client.Player.SwitchWingWalkerAttack(stream);
                            data.AddText("Okay, I`ve changed your branch to [MeleeStomper].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
            }
        }



        [NpcAttribute(NpcID.PromotionTrojan)]
        public static void PromotionTrojan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 10 && client.Player.Class <= 15))
            {
                data.AddText("Trojans do not share their secrets of battle with others. I shall not teach you.").AddOption("Alright.", 255).AddAvatar(5).FinalizeDialog();
                return;
            }

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("Wielding dual weapons, the Trojans always charge fearlessly into combat and they believe courage is the secret of victory. So, what can I do for you?")
                                 .AddOption("Learn~about~Promotion.", 110);
                            if (client.Player.Class == 10)
                                data.AddOption("I~want~to~get~promoted", 1);
                            else
                                data.AddOption("I~want~to~get~promoted", 3);
                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(259).FinalizeDialog();
                            break;
                        }

                        if (client.Player.Class == 10)
                        {
                            data.AddText("Wielding dual weapons, the Trojans always charge fearlessly into combat and they believe courage is the secret of victory. So, what can I do for you?")
                            .AddOption("I~want~to~get~promoted.", 1)
                            .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Wielding dual weapons, the Trojans always charge fearlessly into combat and they believe courage is the secret of victory. So, what can I do for you?")
                               .AddOption("I~want~to~get~promoted.", 3)
                               .AddOption("Learn~class~and~weapon~skills.", 4)
                               .AddOption("Pure~skills~for~Trojans.", 30)
                               .AddOption("Learn~about~the~Epic~quest.", 5)
                               .AddOption("Okay.~I~see.", 255)
                               .AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Trojan` will be a big name in a short time. Listen, you can learn class skills and")
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
                        if (client.Player.Class == 10)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else
                            data.AddOption("I~want~to~get~promoted", 3);
                        data.AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 30:
                    {
                        data.AddText("The Dragon Whirl is a Pure skill, which means that only players who`ve been a Trojan on their two past lives and their current life as well can learn it. Though you`ve been Reborn twice, You are not a Pure Trojan.")
                                                 .AddText("~Why not take an Oblivion Dew and speak with the Reincarnation Seer? Through the power of Reincarnation, you can obtain new class combinations and make yourself a Pure Trojan!")
                                                 .AddOption("Learn~Dragon~Whirl~(Lv~40).", 31)
                                                     .AddOption("I`ll~think~about~it.", 255)
                                                     .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("After your 2nd Rebirth, you still have to be Reincarnated and become a Pure Trojan, to be able to learn the Dragon Whirl skill.")
                                                            .AddOption("Ok~I~see.", 255)
                                                            .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (!Database.AtributesStatus.IsTrojan(client.Player.FirstClass) || !Database.AtributesStatus.IsTrojan(client.Player.SecoundeClass))
                        {
                            data.AddText("You don'thave the combo ( Tro - Tro - Tro ) in your status.")
                                                 .AddOption("Okay.~I~see..", 255)
                                                 .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level < 40 || client.Player.Class < 12)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DragonWhirl;
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
                case 5:
                    {
                        data.AddText("300 years ago, General Pak was ordered to fight against the Flame Devastator. However, a traitor in his army led to Pak`s unexpected death.")
                                                    .AddText("~and the annihilation of his army. Pak`s Ghost (Twin City 267,194) refused to rest in peace, waiting for a chosen Trojan to change the history.")
                                                    .AddText("~The Trojan who successfully changes the history will earn a fabulous Epic Weapon.")
                                                    .AddText("\n\n    (Level 120+ or reborn Trojans will receive a Trojan Epic Weapon for free by completing a specified Trojan Epic Quest every day, and a series of Epic Quests in 90 days.)")
                                                    .AddOption("Head~to~see~General~Pak.", 101)
                                                    .AddOption("I`ll~think~about~it.", 255)
                                                        .AddAvatar(5).FinalizeDialog();



                        break;
                    }
                case 101:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 267, 194, (uint)NpcID.PaksGhost);

                        break;
                    }
                case 3:
                    {
                        switch (client.Player.Class)
                        {
                            case 11:
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
                                                client.Inventory.Add(stream, 410079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 480079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)Cutlass and 1 (Super)IronClub.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
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
                            case 12:
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
                                                client.Inventory.Add(stream, 130069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)RageArmor and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(1080001, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
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
                            case 13:
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
                                                client.Inventory.Add(stream, 410199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2RainbowBlade 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            if (!client.Inventory.Remove(Database.ItemType.Meteor, 1, stream)) break;
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
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
                            case 14:
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
                                            if (!client.Inventory.HaveSpace(1))
                                            {
                                                data.AddText("Please make 1 more space in your inventory")
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
                                            if (!client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream)) break;
                                            client.SendSysMesage("You received DragonBallScroll.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
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
                case 4:
                    {
                        data.AddText("The XP skills for Trojans are Accuracy, Cyclone and Golem. Accuracy can triple your hit rate; Cyclone can increase your speed; By using the Golem skill, you can transform")
                                                        .AddText("~into a fearful Golem! Trojans are always fearless. Hercules makes them powerful and swift, while the mysterious Epic skills enable Trojans to be much fiercer than ever.")
                                                        .AddText("~So, which one did you want to learn?")
                                                        .AddOption("Trojan`s~skills.", 6)
                                                            .AddOption("Trojan`s~Epic~skills.", 7)
                                                            .AddOption("Let~me~see.", 255)
                                                            .AddAvatar(5).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        data.AddText("Fierce as hurricane, Fatal Cross will crash all blocking the way. While the Mortal Strike holds a devastated power that can cause terrifying damage.")
                                                     .AddText("\n[Fatal Cross]: Epic active skill; Ranged attack.")
                                                     .AddText("\n[Mortal Strike]: Epic passive skill; Inflicts heavy damage within a fan-shape range.")
                                                     .AddOption("Fatal~Cross.~", 16)
                                                         .AddOption("Mortal~Strike.~", 17)
                                                         .AddOption("I`ll~think~about~it.", 255)
                                                         .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 17:
                    {
                        //if (client.Equipment.UseEpicTrojanWeapon())
                        {
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalStrike))
                            {
                                data.AddText("You`ve learned MortalStrike!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            if (client.Player.Money >= 1)
                            {
                                client.Player.Money -= 1;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalStrike);
                                data.AddText("You~successfully~acquired~the~skill,~[Mortal~Strike].~Equip~2~Trojan`s~Epic~Weapons~to~cast.~While~attacking,~the~caster~has~a~chance~to~cause~severe~damage~on~human~targets,");
                                data.AddText("~and~more~violent~to~monsters,~within~a~certain~range.");
                                data.AddOption("Thanks!").AddAvatar(5).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Come~on,~you~don`t~have~5,000,000~Silver~with~you.");
                                data.AddOption("Sorry...");
                                data.AddAvatar(5).FinalizeDialog();
                            }
                        }
                        //else
                        //{
                        //    data.AddText("You need to equip a Trojan`s Epic Weapon to learn [MortalStrike].")
                        //                 .AddOption("Ah ok.", 255)
                        //                 .AddAvatar(5).FinalizeDialog();
                        //}

                        break;
                    }
                case 16:
                    {
                        //if (client.Equipment.UseEpicTrojanWeapon())
                        //{
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalCross))
                        {
                            data.AddText("You`ve~already~acquired~this~skill.").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Money >= 1)
                        {
                            client.Player.Money -= 1;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalCross);
                            data.AddText("You~successfully~acquired~the~skill,~[Fatal~Cross].~This~active~skill~is~for~Trojan`s~Epic~Weapon~only.");
                            data.AddText("~Equip~2~Epic~Weapons~and~throw~the~Cross~Saber~in~target~direction,~inflicting~damage~on~all~the~targets");
                            data.AddText("~within~2~paces~along~its~attack~path.~Cost:~25~Stamina.");
                            data.AddOption("Thanks!").AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~5,000,000~Silver~with~you.");
                            data.AddOption("Sorry...");
                            data.AddAvatar(5).FinalizeDialog();
                        }
                        //}
                        //else
                        //{
                        //    data.AddText("You need to equip a Trojan`s Epic Weapon to learn [Fatal Cross].")
                        //                 .AddOption("Ah ok.", 255)
                        //                 .AddAvatar(5).FinalizeDialog();
                        //}


                        break;
                    }
                case 6:
                    {
                        data.AddText("The XP skills for Trojans are Accuracy, Cyclone and Golem. Accuracy can triple your hit rate, Cyclone can increase your attack speed, and by using the Golem skill, you can transform")
                                                   .AddText("~into a fearful Golem! Trojans are always fearless. Hercules makes them powerful and swift, and the Breath Focus recovers the Stamina in time.")
                                                   .AddText("When the Super Cyclone sweeps, no one can stand. Which one would you like to learn?")
                                                   .AddOption("New:~Super~Cyclone~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone) ? "(Learnt)" : "(Lv.40)") + "", 8)
                                                     .AddOption("Accuracy~and~Cyclone~" + ((client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy)) ? "(Learnt)" : "(Lv.40)") + ".", 9)
                                                       .AddOption("Golem~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem) ? "(Learnt)" : "(Lv.40)") + ".", 10)
                                                         .AddOption("Hercules~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hercules) ? "(Learnt)" : "(Lv.40)") + ".", 11)
                                                             .AddOption("Spirit~Healing~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritHealing) ? "(Learnt)" : "(Lv.40)") + ".", 12)
                                                                 .AddOption("New:~Breath~Focus~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus) ? "(Learnt)" : "(Lv.40)") + ".", 13)
                                                       .AddOption("Superior~skills.", 14)
                                                       .AddOption("Learn~weapon~skills.", 15)
                                                               .AddOption("I`ll~think~about~it.", 255)
                                                       .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        if (client.Player.Reborn > 1)
                        {
                            data.AddText("Please choose the spell you want to learn.")
                                  .AddOption("CruelShade(L40).", 31).AddOption("I`ll~think~about~it.", 255)
                                  .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, only reborn players can learn this skill.")
                                                 .AddOption("Okay.~I~see.", 255)
                                                 .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 32:
                    {
                        if (client.Player.Reborn > 1)
                        {
                            if (client.Player.Level < 40 || client.Player.Class < 12)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.CruelShade;
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

                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 12)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SuperCyclone;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("You~successfully~acquired~the~skill,~[Super~Cyclone].~This~XP~skill~exclusive~for~Trojans~boosts~the~caster`s~movement~and~attack~speed,");
                        data.AddText("~and~also~reduces~the~cost~of~Stamina~required~by~other~skills~by~half,~however,~the~caster~cannot~recover~XP~while~in~use~or~hunting~monsters.");
                        data.AddText("~Last~for~40~seconds.~");
                        data.AddOption("Thanks!");
                        data.AddAvatar(5);
                        data.FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 12)
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
                case 11:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 12)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Hercules;
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
                case 12:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 12)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SpiritHealing;
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
                        if (client.Player.Level < 90 || client.Player.Class < 13)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 90 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BreathFocus;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("You~successfully~acquired~the~skill,~[Breath~Focus]!~This~passive~skill~exclusive~for~Trojans");
                        data.AddText("~gives~a~100%~chance~to~recover~a~certain~amount~of~Stamina~if~fails~to~hit~a~target~with~a~random~active~skill");
                        data.AddText("~(excludes~Reborn~skills).");
                        data.AddOption("Thanks!");
                        data.AddAvatar(5);
                        data.FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 12)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                        {
                            data.AddText("You`ve learned Accuracy and Cyclone!")
                                                             .AddOption("Okay.", 255)
                                                             .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            string line = "You just have learned the skill ";
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                                line += Role.Flags.SpellID.Accuracy.ToString();
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                                line += "/" + Role.Flags.SpellID.Cyclone.ToString();
                            }
                            data.AddText(line)
                                            .AddOption("Thank you..", 255)
                                            .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                        {
                            data.AddText("You`ve learned Fast Blade and Scented Sword!")
                                                             .AddOption("Okay.", 255)
                                                             .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            string line = "You just have learned the skill ";
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                                line += Role.Flags.SpellID.FastBlader.ToString();
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                line += "/" + Role.Flags.SpellID.ScrenSword.ToString();
                            }
                            data.AddText(line)
                                            .AddOption("Thank you..", 255)
                                            .AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Level >= 15 && client.Player.Class == 10)
                        {
                            data.AddText("The Trojans are all-out offensive melee combat fighters. Do you want to be a qualified Trojan, now?.")
                                                             .AddOption("Yeah,~please!", 2)
                                                             .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Only the powerful deserve to be trained. You are still a mere fledgeling.")
                                                              .AddOption("Oh,~I~see..", 255)
                                                              .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Player.Level >= 15 && client.Player.Class == 10)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                            }
                            client.Player.Class++;
                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                //.AddText("~For a limited time, you can buy special Super and Elite Kits that are just right for you! Only at the Kit Merchant! One more thing, the Vice Captain in Twin City (511,360) has a quest to test you. You won`t say NO, right?")
                                                              .AddOption("Thank~you,~Master.", 255)
                                                              .AddAvatar(5).FinalizeDialog();

                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }
                        else
                        {
                            data.AddText("Only the powerful deserve to be trained. You are still a mere fledgeling.")
                                                                 .AddOption("Oh,~I~see..", 255)
                                                                 .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }

            }

        }
    }
}
