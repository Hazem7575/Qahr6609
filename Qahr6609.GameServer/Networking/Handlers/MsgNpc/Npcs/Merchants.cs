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
        [NpcAttribute(NpcID.SkillEraserSpecialist)]
        public static void SkillEraserSpecialist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You have to give up something in order to get something. I can help you \n")
                            .AddText("~forget the skills you don`t need anymore.")
                            .AddOption("Forget~skills~in~this~life.", 1)
                            .AddOption("I`ll~think~about~it.", 255).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("If you don`t need some skills anymore, I can help you forget them. \n")
                            .AddText("~So, what kind of skill would you like to forget?")
                            .AddOption("Summon~skills.", 50)
                            .AddOption("Transform~skills.", 100)
                            .AddOption("General~weapon~skills.", 150)
                            .AddOption("Class-based~skills.", 200)
                            .AddOption("I~want~to~ask~something~else.", 0).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                #region Class-based~skills
                case 200:
                    {
                        data.AddText("After you forget the skills you don`t need, you can concentrate on the study of new skills. Which skill would you like to forget?")
                            .AddOption("Spirit~Healing.", 201)
                            .AddOption("Cruel~Shade.", 203)
                            .AddOption("Serenity.", 205)
                            .AddOption("Compassion.", 207)
                            .AddOption("Flying~Moon.", 209)
                            .AddOption("Thunder.", 211)
                            .AddOption("Next.", 213)
                            .AddOption("I~want~to~ask~something~else..", 1).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 213:
                    {
                        data.AddText("After you forget the skills you don`t need, you can concentrate on the study of new skills. Which skill would you like to forget?")
                            .AddOption("Invisibility.", 215)
                            .AddOption("Pervade.", 217)
                            .AddOption("Fire.", 219)
                            .AddOption("Kraken`s~Revenge.", 221)
                            .AddOption("Previous.", 200)
                            .AddOption("I~want~to~ask~something~else..", 1).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                #endregion
                #region General~weapon~skills
                case 150:
                    {
                        data.AddText("After you forget the skills you don`t need, you can concentrate on the study of new skills. Which skill would you like to forget?")
                            .AddOption("Fast~Blade.", 151)
                            .AddOption("Scent~Sword.", 153)
                            .AddOption("Phoenix.", 155)
                            .AddOption("Speed~Gun.", 157)
                            .AddOption("Wide~Strike.", 159)
                            .AddOption("Penetration.", 161)
                            .AddOption("Next.", 163)
                            .AddOption("I~want~to~ask~something~else..", 1).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 163:
                    {
                        data.AddText("After you forget the skills you don`t need, you can concentrate on the study of new skills. Which skill would you like to forget?")
                            .AddOption("Boreas.", 165)
                            .AddOption("Stranded~Monster.", 167)
                            .AddOption("Snow.", 169)
                            .AddOption("Halt.", 171)
                            .AddOption("Boom.", 173)
                            .AddOption("Previous.", 150)
                            .AddOption("Next.", 175)
                            .AddOption("I~want~to~ask~something~else..", 1).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 175:
                    {
                        data.AddText("After you forget the skills you don`t need, you can concentrate on the study of new skills. Which skill would you like to forget?")
                            .AddOption("Seizer.", 177)
                            .AddOption("Earthquake.", 179)
                            .AddOption("Rage.", 181)
                            .AddOption("Celestial.", 183)
                            .AddOption("Roamer.", 185)
                            .AddOption("Previous.", 163)
                            .AddOption("Next.", 187)
                            .AddOption("I~want~to~ask~something~else..", 1).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 187:
                    {
                        data.AddText("After you forget the skills you don`t need, you can concentrate on the study of new skills. Which skill would you like to forget?")
                            .AddOption("DragonTail.", 189)
                            .AddOption("ViperFang.", 191)
                            .AddOption("Previous.", 175)
                            .AddOption("I~want~to~ask~something~else..", 1).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                #endregion
                #region Transform~skills
                case 100:
                    {
                        data.AddText("After you forget the skills you don`t need, you can concentrate on the study of new skills. Which skill would you like to forget?")
                            .AddOption("Night~Devil.", 101)
                            .AddOption("Golem.", 103)
                            .AddOption("Water~Elf.", 105)
                            .AddOption("Divine~Hare.", 107)
                            .AddOption("I~want~to~ask~something~else..", 1).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                #endregion
                #region Summon~skills
                case 50:
                    {
                        data.AddText("After you forget the skills you don`t need, you can concentrate on the study of new skills. Which skill would you like to forget?")
                            .AddOption("Bloody~Bat.", 51)
                            .AddOption("Summon~Guard.", 53)
                            .AddOption("Fire~Evil.", 55)
                            .AddOption("Skeleton.", 57)
                            .AddOption("Summon~Bat~Boss.", 59)
                            .AddOption("Summon~Bat.", 61)
                            .AddOption("I~want~to~ask~something~else..", 1).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                #endregion
                case 252:
                    {
                        data.AddText("Are you sure you want to forget the skill [Fast Blade]?")
                            .AddOption("Yes.", 253)
                            .AddOption("No.", 255)
                            .AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 253:
                    {
                        data.AddText("You`ve successfully forgotten this skill. More powerful skills are waiting for you to learn.")
                            .AddOption("Thanks!", 1)
                            .AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 254:
                    {
                        data.AddText("Sorry, you haven`t learned the skill [Fire Circle]. How can you forget it?")
                            .AddOption("Sorry,~my~mistake.", 1)
                            .AddAvatar(247).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MerchantClerk)]
        public static void MerchantClerk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Merchant == 255)
                        {
                            data.AddText("You have been a merchant now. All of your equipment and items are not protected by the Forbidden Sale system")
                                .AddText("~in the game, and TQ Digital is not responsible for any loss related.")
                                .AddText("Click the Cancel button to reset your merchant account to be normal. A normal account will be protected by all the")
                                .AddText("~trading protection measures in case of equipment loss resulted from accounts being hacked.")
                                .AddOption("I~want~to~cancel~my~merchant~account.", 3)
                                .AddOption("Let~me~think~it~over.", 255)
                                .AddAvatar(94).FinalizeDialog();
                            //client.SendSysMesage("You have canceled merchant account.");
                        }
                        else if (client.Player.Merchant == 1)
                        {
                            data.AddText("Your merchant application is being processed. Please wait for 5 days. All of your equipment and items")
                                .AddText("~are not protected by the Forbidden Sale system in the game, and TQ Digital is not responsible for any loss related.")
                                .AddText("Click the Cancel button to reset your merchant account to be normal. A normal account will be protected by all the")
                                .AddText("~trading protection measures in case of equipment loss resulted from accounts being hacked.")
                                .AddOption("Cancel~my~application,~please.", 2)
                                .AddOption("Let~me~think~it~over.", 255)
                                .AddAvatar(94).FinalizeDialog();
                            //client.SendSysMesage("You have canceled the application.");
                        }
                        else
                        {
                            data.AddText("A merchant account is for any who frequently engages in commercial activities. A merchant account is not protected")
                                .AddText("~by the virtual property protection measures. So TQ Digital will be not responsible for any loss related.")
                                .AddText("After you apply to be a merchant account, tt will take you 5 days to censor your application. You can reset it to be a normal account at any time.")
                                .AddText("Are you ready to be a merchant?")
                                .AddOption("I~want~to~be~a~merchant.", 1)
                                .AddOption("Let~me~think~it~over.", 255)
                                .AddAvatar(94).FinalizeDialog();
                            // client.SendSysMesage("You have applied for a merchant.");
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.Merchant = 0;
                        client.SendSysMesage("You have canceled merchant account.");
                        break;
                    }
                case 2:
                    {
                        client.Player.Merchant = 0;
                        client.SendSysMesage("You have canceled the application.");
                        break;
                    }
                case 1:
                    {
                        client.Player.Merchant = 1;
                        client.Player.MerchantApplicationEnd = DateTime.Now.AddDays(5);
                        client.SendSysMesage("You have applied for a merchant.");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CrystalMerchant)]
        public static void CrystalMerchant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
           
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("18 ferocious devils are locked in the Fiend،¯s Lair. Kill them and you will have chance to gain the stolen treasures. \n")
                            .AddText("~Besides, some devils from Floor 11 to Floor 18 carry the crystals which concentrate their feeling of resentment. \n")
                            .AddText("~Once you get one, you can swap the crystal for incredibly good reward here. \n")
                            .AddText("~You can also buy Fiend،¯s Lair Permits from Shopping Mall. With the help of these permits, you can reach the corresponding floors directly (Floor 11 to Floor 18).")
                            //.AddOption("Fiend~Lair~Permit?", 1)
                            //.AddOption("I`ve~got~a~crystal.", 49)
                            .AddOption("I~see.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 49:
                    {
                        data.AddText("You can directly use the crystals to obtain huge EXP or swap them for incredibly good reward here.")
                            .AddText("~So, which kind of crystal did you get?")
                            .AddOption("Frost~Crystal.", 50)//720282
                            .AddOption("Fire~Crystal.", 51)//721003
                            .AddOption("Soul~Crystal.", 52)//721004
                            .AddOption("Dread~Crystal.", 53)//721005
                            .AddOption("Next~page", 54)
                            .AddOption("Not~yet.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 53:
                    {
                        if (!client.Inventory.Contain(721005, 1))
                        {
                            data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
                                .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
                                .AddOption("I~won`t~let~you~down.", 255)
                                .AddOption("I~wanna~sell~one.", 49)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721005, 1, stream)) break;
                        client.Player.ConquerPoints += 27600;
                        client.SendSysMesage("Dread~Crystal Exchanged.");
                        break;
                    }
                case 52:
                    {
                        if (!client.Inventory.Contain(721004, 1))
                        {
                            data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
                                .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
                                .AddOption("I~won`t~let~you~down.", 255)
                                .AddOption("I~wanna~sell~one.", 49)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721004, 1, stream)) break;
                        client.Player.ConquerPoints += 13800;
                        client.SendSysMesage("Soul~Crystal Exchanged.");
                        break;
                    }
                case 51:
                    {
                        if (!client.Inventory.Contain(721003, 1))
                        {
                            data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
                                .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
                                .AddOption("I~won`t~let~you~down.", 255)
                                .AddOption("I~wanna~sell~one.", 49)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721003, 1, stream)) break;
                        client.Player.ConquerPoints += 6900;
                        client.SendSysMesage("Fire~Crystal Exchanged.");
                        break;
                    }
                case 50:
                    {
                        if (!client.Inventory.Contain(720282, 1))
                        {
                            data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
                                .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
                                .AddOption("I~won`t~let~you~down.", 255)
                                .AddOption("I~wanna~sell~one.", 49)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(720282, 1, stream)) break;
                        client.Player.ConquerPoints += 1380;
                        client.SendSysMesage("Frost~Crystal Exchanged.");
                        break;
                    }
                case 54:
                    {
                        data.AddText("You can directly use the crystals to obtain huge EXP or swap them for incredibly good reward here.")
                            .AddText("~So, which kind of crystal did you get?")
                            .AddOption("Bloody~Crystal.", 55)//721006
                            .AddOption("Dark~Crystal.", 56)//721007
                            .AddOption("Chaos~Crystal.", 57)//721008
                            .AddOption("Moon~Crystal.", 58)//721009
                            .AddOption("Previous.", 49)
                            .AddOption("Not~yet.", 255).AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 58:
                    {
                        if (!client.Inventory.Contain(721009, 1))
                        {
                            data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
                                .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
                                .AddOption("I~won`t~let~you~down.", 255)
                                .AddOption("I~wanna~sell~one.", 49)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721009, 1, stream)) break;
                        client.Player.ConquerPoints += 690000;
                        client.SendSysMesage("Moon~Crystal Exchanged.");
                        break;
                    }
                case 57:
                    {
                        if (!client.Inventory.Contain(721008, 1))
                        {
                            data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
                                .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
                                .AddOption("I~won`t~let~you~down.", 255)
                                .AddOption("I~wanna~sell~one.", 49)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721008, 1, stream)) break;
                        client.Player.ConquerPoints += 276000;
                        client.SendSysMesage("Chaos~Crystal Exchanged.");
                        break;
                    }
                case 56:
                    {
                        if (!client.Inventory.Contain(721007, 1))
                        {
                            data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
                                .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
                                .AddOption("I~won`t~let~you~down.", 255)
                                .AddOption("I~wanna~sell~one.", 49)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721007, 1, stream)) break;
                        client.Player.ConquerPoints += 138000;
                        client.SendSysMesage("Dark~Crystal Exchanged.");
                        break;
                    }
                case 55:
                    {
                        if (!client.Inventory.Contain(721006, 1))
                        {
                            data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
                                .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
                                .AddOption("I~won`t~let~you~down.", 255)
                                .AddOption("I~wanna~sell~one.", 49)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721006, 1, stream)) break;
                        client.Player.ConquerPoints += 69000;
                        client.SendSysMesage("Bloody~Crystal Exchanged.");
                        break;
                    }
                case 1:
                    {
                        data.AddText("The 8 Fiend Lair Permits (Floor 11 to 18) are made by rare and precious materials. If you use")
                            .AddText("~this Fiend،¯s Lair Permit, you will be sent to that level of the Lair directly, by using them. Do ")
                            .AddText("~you want to buy one from the Shopping Mall? Or sell me your permit?")
                            .AddOption("I~won`t~let~you~down.", 255)
                            .AddOption("I~wanna~sell~one.", 2)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("I know what you`re thinking. Don`t worry, my friend. I will buy at the original price.")
                            .AddText("~So which Fiend`s Lair Permit do you want to sell?")
                            .AddOption("Floor~11:~9~CPs.", 3)
                            .AddOption("Floor~12:~49~CPs.", 4)
                            .AddOption("Floor~13:~99~CPs.", 5)
                            .AddOption("Floor~14:~199~CPs.", 6)
                            .AddOption("Next.", 7)
                            .AddOption("Let~me~think~about~it.", 255)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.Contain(721094, 1))
                        {
                            data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
                                .AddOption("I~am~just~looking.", 255)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721094, 1, stream)) break;
                        client.Player.ConquerPoints += 199;
                        client.SendSysMesage("FiendLairPermit Exchanged.");
                        break;
                    }
                case 5:
                    {
                        if (!client.Inventory.Contain(721093, 1))
                        {
                            data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
                                .AddOption("I~am~just~looking.", 255)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721093, 1, stream)) break;
                        client.Player.ConquerPoints += 99;
                        client.SendSysMesage("FiendLairPermit Exchanged.");
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.Contain(721092, 1))
                        {
                            data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
                                .AddOption("I~am~just~looking.", 255)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721092, 1, stream)) break;
                        client.Player.ConquerPoints += 49;
                        client.SendSysMesage("FiendLairPermit Exchanged.");
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.Contain(721091, 1))
                        {
                            data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
                                .AddOption("I~am~just~looking.", 255)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721091, 1, stream)) break;
                        client.Player.ConquerPoints += 9;
                        client.SendSysMesage("FiendLairPermit Exchanged.");
                        break;
                    }
                case 7:
                    {
                        data.AddText("I know what you`re thinking. Don`t worry, my friend. I will buy at the original price.")
                            .AddText("~So which Fiend`s Lair Permit do you want to sell?")
                            .AddOption("Floor~15:~499~CPs", 8)
                            .AddOption("Floor~16:~999~CPs.", 9)
                            .AddOption("Floor~17:~1,999~CPs.", 10)
                            .AddOption("Floor~18:~4,999~CPs.", 11)
                            .AddOption("Previous.", 2)
                            .AddOption("Let~me~think~about~it.", 255)
                            .AddAvatar(237)
                            .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.Contain(721098, 1))
                        {
                            data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
                                .AddOption("I~am~just~looking.", 255)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721098, 1, stream)) break;
                        client.Player.ConquerPoints += 4999;
                        client.SendSysMesage("FiendLairPermit Exchanged.");
                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.Contain(721097, 1))
                        {
                            data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
                                .AddOption("I~am~just~looking.", 255)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721097, 1, stream)) break;
                        client.Player.ConquerPoints += 1999;
                        client.SendSysMesage("FiendLairPermit Exchanged.");
                        break;
                    }
                case 9:
                    {
                        if (!client.Inventory.Contain(721096, 1))
                        {
                            data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
                                .AddOption("I~am~just~looking.", 255)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721096, 1, stream)) break;
                        client.Player.ConquerPoints += 999;
                        client.SendSysMesage("FiendLairPermit Exchanged.");
                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.Contain(721095, 1))
                        {
                            data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
                                .AddOption("I~am~just~looking.", 255)
                                .AddAvatar(237)
                            .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(721095, 1, stream)) break;
                        client.Player.ConquerPoints += 499;
                        client.SendSysMesage("FiendLairPermit Exchanged.");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FiendLairHerald)]
        public static void FiendLairHerald(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hush, hush! Did you ever hear of the Fiend`s Lair? It has been sealed off for years, and 18 ferocious devils are locked inside. \n")
                            .AddText("~Some devils carry these special crystals, which concentrate their feelings of resentment and hate for mortals. \n")
                            .AddText("~Pick up their crystals when they die, and you can cash in big! Besides, you also have a chance to find stolen treasures, there. \n")
                            .AddText("~The deeper you go into the cave, the better treasures are hidden. But you have to go to the each level, one by one. \n")
                            .AddText("~Now, I can send you to Fiend`s Lair Floor 1 for the low, low price of 10,000 silver. What do you think? Want to go in?")
                            .AddOption("Send~me~there.", 1)
                            .AddOption("What`s~a~Fiend~Lair~Permit?", 2)
                            .AddOption("Tell~me~more.", 3)
                            .AddOption("Not~interested.", 255).AddAvatar(52).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money < 10000)
                        {
                            client.Player.MessageBox("Sorry,~but~you~don`t~have~enough~money~to~enter~the~Fiend`s~Lair.", null, null);
                            break;
                        }
                        client.Player.Money -= 10000;
                        client.Teleport(192, 168, 8822);
                        client.Player.MessageBox("You~entered~the~Fiend`s~Lair~Floor~1.", null, null);
                        break;
                    }
                case 2:
                    {
                        data.AddText("The 8 Fiend،¯s Lair Permits (Floor 11 to 18) are temporary gates that we،¯ve managed to open. ")
                            .AddText("~You will be sent to whatever floor directly, rather than having to walk there. The problem, is ")
                            .AddText("~that they،¯re unstable and can only be used once. If you want to purchase some, you can find them in the Shopping Mall.")
                            .AddOption("Got~it.", 255).AddAvatar(52).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("There are 18 floors in the Fiend`s Lair, and each of those devils are locked on seperate floors.")
                            .AddText("~Their feelings of hate and resentment have been stored up over the years. Now they begin to emerge, and threaten our world!")
                            .AddText("~Please, shoulder this holy task and fight for peace! They must be put down, if we`re to go on with our lives.")
                            .AddOption("What`s~the~rewards?", 4).AddAvatar(52).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("~Kill the devils in the Fiend`s Lair, and you will have a chance to find their stolen treasures.")
                            .AddText("~Besides, some devils on Floors 11 - 18 carry the crystals which are of great interest to me and my studies.")
                            .AddText("~You can gain huge EXP from the crystals, or you can sell them to our Crystal Merchant for CPs!")
                            .AddOption("Excellent.", 255).AddAvatar(52).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralSam)]
        public static void GeneralSam(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("No matter how plain a weapon may be, if reinforcement can be done on it, it will be powerful. You need to bring me a Craft Scroll if you want me to open up the 1st")
                            .AddText("~Socket in your main weapon. While after that, a Holy Craft Scroll is needed to make the 2nd Socket in that main weapon. Want help?")
                            .AddOption("Make~the~1st~Socket.", 1)
                            .AddOption("Make~the~2nd~Socket.", 10)
                            .AddOption("I~was~just~leaving.", 255).AddAvatar(48).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("I`ll charge a Craft Scroll to make the 1st Socket in your main weapon. Are you ready?")
                            .AddOption("Yes!", 2)
                            .AddOption("Wait,~I`m~not~ready,~yet!", 255)
                            .AddAvatar(48).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        MsgGameItem Item = null;
                        if (!client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out Item))
                        {
                            data.AddText("Wait, you must be wearing an main weapon first.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        if (Item.SocketOne != Role.Flags.Gem.NoSocket)
                        {
                            data.AddText("Wait, you`ve already opened up the 1st Socket in your main weapon. No need to do it again!")
                            .AddOption("I~see.", 255)
                            .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(3001303, 1, Item.Bound))
                        {
                            data.AddText("Wait, you need Craft Scroll same bound state as your weapon.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(3001303, 1, stream)) break;
                        Item.SocketOne = Role.Flags.Gem.EmptySocket;
                        Item.Mode = Role.Flags.ItemMode.Update;
                        Item.Send(client, stream);
                        client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you sure you want to pay a Holy Craft Scroll to open up the 2nd Socket in your main weapon?")
                            .AddOption("Yes!", 12)
                            .AddOption("Wait,~I`m~not~ready,~yet!", 255)
                            .AddAvatar(48).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        MsgGameItem Item = null;
                        if (!client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out Item))
                        {
                            data.AddText("Wait, you must be wearing an main weapon first.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        if (Item.SocketOne == Role.Flags.Gem.NoSocket)
                        {
                            data.AddText("Wait, you need to have 1 sock main weapon first.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        if (Item.SocketTwo != Role.Flags.Gem.NoSocket)
                        {
                            data.AddText("Wait, you`ve already had the 2nd Socket of your main weapon opened up. No need to do it again!")
                            .AddOption("I~see.", 255)
                            .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(3001304, 1, Item.Bound))
                        {
                            data.AddText("Wait, you need Holy Craft Scroll same bound state as your weapon.")
                            .AddOption("I~see.", 255)
                            .AddAvatar(48).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(3001304, 1, stream)) break;
                        Item.SocketTwo = Role.Flags.Gem.EmptySocket;
                        Item.Mode = Role.Flags.ItemMode.Update;
                        Item.Send(client, stream);
                        client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AstralMaster)]
        public static void AstralMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip color=0xffffff00 desc=When#Astral#Master#was#younger,#he#practiced#in#closed#retreats#for#years#and#finally#achieved#the#unique#ability#to#change#destiny#after#penetrating#the#mystery#of#life.#Astral#Master#has#few#but#great#friends#like#Heavenly#Master.>Astral Master: </tip> \n")
                            .AddText("I`ve spent countless days and nights in developing items to add Attribute Points. Fortunately, I made it and I`m willing to help more people. \n")
                            .AddText("~Heroes can find me to make attribute items with necessary materials, once a week.")
                            .AddOption("Make~attribute~item.", 1)
                            .AddOption("How~does~it~work?", 250)
                            .AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(73).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("                      *Nature of Life* \n")
                            .AddText("،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ\n")
                            .AddText("Required Material: 10 Emeralds \n")
                            .AddText("Gains: 1 Mystery Fruit, 3 Spirit Tokens \n")
                            .AddText("،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ،ھ \n")
                            .AddText("(Collect 4 kinds of Attribute Tokens to make a Mystery Fruit which adds Attribute Points to your character.)")
                            .AddOption("I~brought~you~the~materials!", 255)
                            .AddOption("Change~the~material.~(3~CPs)", 255)
                            .AddOption("Directly~claim~gains.~(9~CPs)", 255)
                            .AddOption("I~see.", 255).AddAvatar(73).FinalizeDialog();
                        break;
                    }
                case 250:
                    {
                        data.AddText("As long as you bring me the required materials, I can help you make a Mystery Fruit and 3 same-type Attribute Tokens,")
                            .AddText("~once a week. While for VIPs, I give them an extra chance.")
                            .AddOption("I~see.", 255).AddAvatar(73).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SashTrader)]
        public static void SashTrader(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m glad to see you here. Is it about the exchange of Sash and Sash Certificate?")
                            .AddOption("Yes.", 1)
                            .AddOption("I~need~Attribute~Points.", 20)
                            .AddOption("I`m~just~passing~by.", 255)
                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("Yeah, I know a way to produce Attribute Points, but it`s a little complicated. You must be a reborn hero,")
                            .AddText("~then you may use Sashes (L) and Fruit of Fate to make Attribute Points, up to 3 times. The more Sashes (L)")
                            .AddText("~you use, the bigger the chance of getting more points. If you`re not satisfied with the Attribute produced,")
                            .AddText("~you can choose Meteor instead, which will not consume your chances.");
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 1))
                        {
                            data.AddOption("UseMeteors.", 22);
                        }
                        data.AddOption("I~have~both~items.", 21)
                            .AddOption("I~see.", 255)
                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            data.AddText("Sorry, you aren't reborn hero.")
                                .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720323, 1))//FruitofFate
                        {
                            data.AddText("Sorry, you don`t have a Fruit of Fate.")
                                .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        uint count = client.Inventory.GetCount(Database.ItemType.Meteor) == 0 ? 1 : client.Inventory.GetCount(Database.ItemType.Meteor);
                        if (!client.Inventory.Contain(Database.ItemType.Meteor, count))
                        {
                            data.AddText("Sorry, you don`t have a Meteors the more you use the more your chance.")
                                .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.FateFruitUsed >= 3)
                        {
                            data.AddText("Sorry, you have used all your tries.")
                                .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        double chance = count / 1000;
                        if (MyMath.Success(chance))
                        {
                            client.CreateBoxDialog("Success ! You successfully combined (FruitofFate and (" + count + ") Meteor(s)) and received 3 Attribute Point`s.");
                            client.Player.AddExtraAtributes(stream, 3);
                        }
                        else
                        {
                            client.CreateBoxDialog("Failed to combine try again later.");
                        }
                        if (!client.Inventory.Remove(720323, 1, stream)) break;
                        if (!client.Inventory.Remove(Database.ItemType.Meteor, count, stream)) break;
                        client.Player.FateFruitUsed++;
                        break;
                    }
                case 21:
                    {
                        if (client.Player.Reborn < 1)
                        {
                            data.AddText("Sorry, you aren't reborn hero.")
                                .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720323, 1))//FruitofFate
                        {
                            data.AddText("Sorry, you don`t have a Fruit of Fate.")
                                .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        uint count = client.Inventory.GetCount(1100009) == 0 ? 1 : client.Inventory.GetCount(1100009);
                        if (!client.Inventory.Contain(1100009, count))
                        {
                            data.AddText("Sorry, you don`t have a (L) Sash the more you use the more your chance.")
                                .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.FateFruitUsed >= 3)
                        {
                            data.AddText("Sorry, you have used all your tries.")
                                .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        double chance = count / 1000;
                        if (MyMath.Success(chance))
                        {
                            client.CreateBoxDialog("Success ! You successfully combined (FruitofFate and (" + count + ") Sash(L)) and received 3 Attribute Point`s.");
                            client.Player.AddExtraAtributes(stream, 3);
                        }
                        else
                        {
                            client.CreateBoxDialog("Failed to combine try again later.");
                        }
                        if (!client.Inventory.Remove(720323, 1, stream)) break;
                        if (!client.Inventory.Remove(1100009, count, stream)) break;
                        client.Player.FateFruitUsed++;
                        break;
                    }
                case 1:
                    {
                        data.AddText("Do you want to exchange Sash for Sash Certificate or exchange in reverse?")
                            .AddOption("Certificate~(M)~for~2~Certificates~(S).", 2)
                            .AddOption("Sash~(L)~for~Certificate~(L).", 5)
                            .AddOption("Certificate~(S)~for~Sash~(S).", 8)
                            .AddOption("Certificate~(L)~for~Sash~(L).", 11)
                            .AddOption("I`ll~talk~to~you~later.", 255)
                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                #region SashCertifications
                #region CL>>L
                case 11:
                    {
                        data.AddText("It`ll also cost you a Dragon Ball to exchange a Sash (L) for a Sash Certificate (L). Are you ready?")
                            .AddOption("Yes.", 12)
                            .AddOption("What?~I`ll~talk~to~you~later.", 255)
                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            data.AddText("The exchange will consume an unbound Dragon Ball. Please bring one for the exchange.")
                               .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            return;
                        }
                        if (client.Inventory.Contain(722842, 1))
                        {
                            if (!client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream)) break;
                            if (!client.Inventory.Remove(722842, 1, stream)) break;
                            client.Inventory.Add(stream, 1100009, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        }
                        else
                        {
                            data.AddText("Wait, you don`t have the Sash Certificate you mentioned.")
                                .AddOption("I`ll~go~and~check.", 255).AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region CS>>S
                case 8:
                    {
                        data.AddText("It`ll also cost you a Dragon Ball to exchange a Sash Certificate (S) for a Sash (S). Are you ready?")
                            .AddOption("Yes.", 9)
                            .AddOption("What?~I`ll~talk~to~you~later.", 255)
                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            data.AddText("The exchange will consume an unbound Dragon Ball. Please bring one for the exchange.")
                               .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            return;
                        }
                        if (client.Inventory.Contain(722840, 1))
                        {
                            if (!client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream)) break;
                            if (!client.Inventory.Remove(722840, 1, stream)) break;
                            client.Inventory.Add(stream, 1100003, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        }
                        else
                        {
                            data.AddText("Wait, you don`t have the Sash Certificate you mentioned.")
                                .AddOption("I`ll~go~and~check.", 255).AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region L >> CL
                case 5:
                    {
                        data.AddText("It`ll also cost you a Dragon Ball to exchange a Sash (L) for a Sash Certificate (L). Are you ready?")
                            .AddOption("Yes.", 6)
                            .AddOption("What?~I`ll~talk~to~you~later.", 255)
                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            data.AddText("The exchange will consume an unbound Dragon Ball. Please bring one for the exchange.")
                               .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            return;
                        }
                        if (client.Inventory.Contain(1100009, 1))
                        {
                            if (!client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream)) break;
                            if (!client.Inventory.Remove(1100009, 1, stream)) break;
                            client.Inventory.Add(stream, 722842, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                        }
                        else
                        {
                            data.AddText("Wait, you don`t have the Sash you mentioned.")
                                .AddOption("I`ll~go~and~check.", 255).AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region CM >> 2CS
                case 2:
                    {
                        data.AddText("It`ll also cost you a Dragon Ball to exchange a Sash Certificate (M) for (2) Sash Certificate (S). Are you ready?")
                            .AddOption("Yes.", 3)
                            .AddOption("What?~I`ll~talk~to~you~later.", 255)
                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            data.AddText("The exchange will consume an unbound Dragon Ball. Please bring one for the exchange.")
                               .AddOption("Alright.", 255).AddAvatar(67).FinalizeDialog();
                            return;
                        }
                        if (client.Inventory.Contain(722841, 1))
                        {
                            if (!client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream)) break;
                            if (!client.Inventory.Remove(722841, 1, stream)) break;
                            client.Inventory.Add(stream, 722840, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true, "(2)");
                        }
                        else
                        {
                            data.AddText("Wait, you don`t have the Sash Certificate you mentioned.")
                                .AddOption("I`ll~go~and~check.", 255).AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #endregion
            }
        }


        [NpcAttribute(NpcID.BlueRock)]
        public static void BlueRock(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Give me your code to enter.")
                            .AddInput("Please Enter", 1).AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        int Value = 0;
                        if (int.TryParse(Input, out Value))
                        {
                            if (Value == 5000)
                            {
                                //  client.Player.Money += 5000;
                                client.SendSysMesage("You recieved 5,000 silvers come back again and guess.");
                            }
                           
                        }
                        break;
                    }
            }
        }
    }
}
