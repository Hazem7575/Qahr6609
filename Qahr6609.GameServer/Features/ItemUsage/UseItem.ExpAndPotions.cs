using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {
        // UseItem handlers: EXP balls, EXP/attack/defense potions, pills, blessings.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_ExpAndPotions(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 723912:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Player.Level < 140)
                            client.GainExpBall(200, true, Role.Flags.ExperienceEffect.angelwing);
                        break;
                    }

                case 3004230://SuperSeaPotion
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseBreack, new Role.Instance.RoleStatus(150 * 10, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseBreack, 80, 150, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreasePStrike, new Role.Instance.RoleStatus(150 * 100, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreasePStrike, 80, 150, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseMStrike, new Role.Instance.RoleStatus(150 * 100, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseMStrike, 80, 150, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseAntiBreack, new Role.Instance.RoleStatus(150 * 10, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseAntiBreack, 80, 150, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseImunity, new Role.Instance.RoleStatus(150 * 100, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseImunity, 80, 150, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalPAttack, new Role.Instance.RoleStatus(3000, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseFinalPAttack, 80, 3000, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalMAttack, new Role.Instance.RoleStatus(3000, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseFinalMAttack, 80, 3000, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalPDamage, new Role.Instance.RoleStatus(3000, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseMAttack, 80, 3000, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalMDamage, new Role.Instance.RoleStatus(3000, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseFinalMDamage, 80, 3000, 0, false);

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }


                case 3004231://BreakSeaPotion
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseBreack, new Role.Instance.RoleStatus(150 * 10, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseBreack, 80, 150, 0, false);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3004232://StrikeSeaPotion
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreasePStrike, new Role.Instance.RoleStatus(150 * 100, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreasePStrike, 80, 150, 0, false);

                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseMStrike, new Role.Instance.RoleStatus(150 * 100, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseMStrike, 80, 150, 0, false);

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3004233://Anti-breakSeaPotion
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseAntiBreack, new Role.Instance.RoleStatus(150 * 10, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseAntiBreack, 80, 150, 0, false);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3004234://Anti-ImmunitySeaPotion 
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseImunity, new Role.Instance.RoleStatus(150 * 100, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseImunity, 80, 150, 0, false);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3004235://P-AttackSeaPotion
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalPAttack, new Role.Instance.RoleStatus(3000, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseFinalPAttack, 80, 3000, 0, false);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3004236://M-AttackSeaPotion
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalMAttack, new Role.Instance.RoleStatus(3000, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseFinalMAttack, 80, 3000, 0, false);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3004237://P-DamageSeaPotion
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalPDamage, new Role.Instance.RoleStatus(3000, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseMAttack, 80, 3000, 0, false);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3004238://M-DamageSeaPotion
                    {
                        if (client.Player.Map != 3055 && client.Player.Map != 10137)
                        {
                            client.SendSysMesage("sorry, can't use this item here.");
                            return;
                        }
                        client.ExtraStatus.TryAdd(Role.Instance.RoleStatus.StatuTyp.IncreaseFinalMDamage, new Role.Instance.RoleStatus(3000, 80));
                        client.Player.SendUpdate(stream, MsgUpdate.DataType.IncreaseFinalMDamage, 80, 3000, 0, false);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 723726://LifeFruit
                    {
                        if (DateTime.Now > client.Player.MedicineStamp.AddMilliseconds(300))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                            client.Player.Mana = (ushort)client.Status.MaxMana;
                            client.Player.MedicineStamp = DateTime.Now;
                        }
                        break;
                    }
                case 723725://LifeFruitBasket
                    {
                        if (client.Inventory.HaveSpace(9))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 723726, 10, 0);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 9 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727464:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            if (item.Bound == 1)
                            {
                                client.Inventory.Add(stream, Database.ItemType.ExpBall, 5, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                                client.Inventory.Add(stream, Database.ItemType.ExpBall, 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 720128:
                case 723727:
                    {
                        if (client.Player.PKPoints > 30)
                        {
                            client.Player.PKPoints -= 30;
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.SendSysMesage("You've removed 30 Pk Points.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        }
                        else
                        {
                            client.SendSysMesage("You can not use this item for now since your Pk points are less than 30.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        }
                        break;
                    }
                case 720393:
                    {

                        client.SendSysMesage("From now on, you can get triple experience for the next two hours.");
                        client.Player.RateExp = 3;
                        client.Player.DExpTime = 3600 * 2;
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.CreateExtraExpPacket(stream);
                        break;
                    }
                case Database.ItemType.DoubleExp:
                    {
                        client.HeroRewards.AddGoal(206);

                        client.Player.RateExp = 2;
                        client.Player.DExpTime = 3600;
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.CreateExtraExpPacket(stream);
                        client.SendSysMesage("You get double exp. time of one hour.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        break;
                    }
                case 721630:
                    {
                        if (client.Player.ExpFruit == 0)
                        {
                            client.Player.ExpFruit++;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        else
                        {
                            client.SendSysMesage("You can eat only one fruit a day. Try tomorrow.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                        }
                        break;
                    }
                case Database.ItemType.ExpBall:
                    {
                        if (client.Player.Level >= 140)
                        {
                            client.SendSysMesage("You are high level ! ", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                            break;
                        }
                        if (client.Player.ExpBallUsed < 10)
                        {
                            client.Player.ExpBallUsed++;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        else
                        {
                            client.SendSysMesage("You can use only ten exp balls a day. Try tomorrow.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);

                        }
                        break;
                    }
                case 722136:
                case Database.ItemType.ExpBall2:
                    {
                        if (client.Player.Level >= 140)
                        {
                            client.SendSysMesage("As your character has reached the highest level, you cannot use this.", MsgMessage.ChatMode.System);
                            break;
                        }

                        client.SendSysMesage("Congratulations! You have obtained the experience worth 1 Exp Ball.", MsgMessage.ChatMode.System);
                        client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3003235:
                    {
                        if (client.Inventory.ContainItemWithStack(3003235, 15))
                        {
                            client.Inventory.RemoveStackItem(3003235, 15, stream);
                            client.Inventory.Add(stream, Database.ItemType.PowerExpBall, 1);
                            break;
                        }
                        else if (client.Inventory.Contain(3003235, 15))
                        {
                            client.Inventory.Remove(3003235, 15, stream);
                            client.Inventory.Add(stream, Database.ItemType.PowerExpBall, 1);
                            break;
                        }
                        else
                        {
                            client.SendSysMesage("Need 15 scraps to combine.");
                            break;
                        }
                    }
                case 723744:
                case Database.ItemType.PowerExpBall:
                    {
                        //break;
                        if (client.Player.Level < 140)
                        {
                            var nextlevel = Database.Server.LevelInfo[Database.DBLevExp.Sort.User][(byte)(client.Player.Level)];
                            ulong exp = (ulong)(nextlevel.Experience * 10 / 100);
                            client.Player.Experience += exp + 1;
                            client.Player.SendUpdate(stream, (long)client.Player.Experience, MsgUpdate.DataType.Experience);

                            if (client.Player.Experience >= nextlevel.Experience)
                            {
                                ushort level = (ushort)(client.Player.Level + 1);
                                client.UpdateLevel(stream, level, true);
                            }
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        else
                        {
                            client.SendSysMesage("As your character has reached the highest level, you cannot use this.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3005064:// SolarActivePack
                    {
                        if (client.Inventory.HaveSpace(11))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 729023, 4);
                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 2);
                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream);
                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 5, stream);
                            //client.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2);
                            client.Player.ConquerPoints += 20000;

                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 11 space in your inventory.");

                        }
                        break;
                    }
                case 3005065:// DivineActivePack
                    {
                        if (client.Inventory.HaveSpace(14))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 729023, 4);
                            client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 2);
                            client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 5, stream);
                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 5, stream);
                            //  client.Inventory.Add(stream,Database.ItemType.PowerExpBall, 4);
                            client.Player.ConquerPoints += 30000;

                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 14 space in your inventory.");

                        }
                        break;
                    }
                case 728896:
                    {
                        client.Player.AddHeavenBlessing(stream, (int)(5 * 60 * 60));
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                        client.Map.SendSysMesage("Congratulations!~" + client.Player.Name + "~got~5~hours~of~Heaven~Blessing.", MsgMessage.ChatMode.TopLeft);

                        break;
                    }
                case 720173:
                    {
                        client.Player.AddHeavenBlessing(stream, (int)(5 * 60 * 60));
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.SendSysMesage("Congratulation! " + client.Player.Name + " got 5 hours of Heaven Blessing.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        break;
                    }
                case 720172:
                    {
                        if (client.Player.Level == 140)
                        {
                            client.SendSysMesage("As your character has reached the highest level, you cannot use this.", MsgMessage.ChatMode.System);
                            break;
                        }
                        client.GainExpBall(300, false, Role.Flags.ExperienceEffect.angelwing);
                        client.SendSysMesage("Congratulation! You have obtained the experience worth 1 EXPBallScrap.", MsgMessage.ChatMode.System);

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 728897:
                    {
                        if (client.Inventory.Contain(728897, 15))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                client.CreateBoxDialog("Please make 1 more space in your inventory");
                                break;
                            }
                            client.Inventory.Remove(728897, 15, stream);
                            client.Inventory.Add(stream, Database.ItemType.ExpBall2);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e104");
                            client.CreateBoxDialog("You~received~an~EXP~Ball~after~you~combined~15~Special~EXP~Ball~Scraps!");
                        }
                        else
                        {
                            client.CreateBoxDialog("You~need~to~have~15~Special~EXP~Ball~Scraps~to~combine~and~get~a~EXP~Ball.");
                        }

                        break;
                    }
                case 3001027:
                    {
                        if (!client.Inventory.HaveSpace(10))
                        {
                            client.CreateBoxDialog("Please make 10 more spaces in your inventory.");

                            break;

                        }
                        client.Inventory.Add(stream, Database.ItemType.ExpBall, 10, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.CreateBoxDialog("You've received 10 Exp Balls(B).");

                        break;
                    }
                case 721625://attackpotin 30
                    {
                        if (client.Player.Level > 100)
                        {
                            client.SendSysMesage("sorry, the max level is 100, for active this.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ActiveAttackPotion(30);
                        break;
                    }
                case 721626://defensepotin 30
                    {
                        if (client.Player.Level > 100)
                        {
                            client.SendSysMesage("sorry, the max level is 100, for active this.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.ActiveDefensePotion(30);
                        break;
                    }
                case 3000781:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 10, stream, true);
                        break;
                    }
                case 720846:
                    {





                        if (client.Player.AtiveQuestApe == 0)
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 340, 270) < 18)
                            {
                                client.Player.AtiveQuestApe = 1;
                                client.SendSysMesage("You drop the Enervation Pill!", MsgMessage.ChatMode.System);

                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1357, 2);
                            }
                            else
                            {
                                client.SendSysMesage("Go to territory of the Apes at(340,270) to use this item!", MsgMessage.ChatMode.System);

                            }
                        }
                        else if (client.Player.AtiveQuestApe == 1)
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 230, 340) < 18)
                            {
                                client.Player.AtiveQuestApe = 2;
                                client.SendSysMesage("You drop the Enervation Pill!", MsgMessage.ChatMode.System);

                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1357, 1);
                            }
                            else
                            {
                                client.SendSysMesage("Go to territory of the Apes at(230,340) to use this item!", MsgMessage.ChatMode.System);

                            }
                        }
                        else if (client.Player.AtiveQuestApe == 2)
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 130, 270) < 18)
                            {
                                client.Player.AtiveQuestApe = 3;
                                client.SendSysMesage("You drop the Enervation Pill!", MsgMessage.ChatMode.System);

                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1357, 1);
                            }
                            else
                            {
                                client.SendSysMesage("Go to territory of the Apes at(130,270) to use this item!", MsgMessage.ChatMode.System);

                            }
                        }
                        else if (client.Player.AtiveQuestApe == 3)
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 250, 230) < 18)
                            {
                                client.Player.AtiveQuestApe = 4;
                                client.SendSysMesage("You drop the Enervation Pill!", MsgMessage.ChatMode.System);

                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1357, 1);
                            }
                            else
                            {
                                client.SendSysMesage("Go to territory of the Apes at(250,230) to use this item!", MsgMessage.ChatMode.System);

                            }
                        }
                        else if (client.Player.AtiveQuestApe == 4)
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 300, 100) < 18)
                            {
                                client.Player.AtiveQuestApe = 5;
                                client.SendSysMesage("You drop the Enervation Pill!", MsgMessage.ChatMode.System);

                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1357, 1);
                            }
                            else
                            {
                                client.SendSysMesage("Go to territory of the Apes at(300,100) to use this item!", MsgMessage.ChatMode.System);

                            }
                        }



                        break;
                    }
                case 722454:
                    {
                        if (!client.Inventory.HaveSpace(5))
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 5, stream);
                        client.Inventory.AddItemWitchStack(723903, 0, 6, stream);
                        client.Inventory.Add(stream, 730002, 3, 2);
                        client.CreateBoxDialog("You received 5 Exp Ball, 6 Saddles and 3 (+2)Stone!");
                        break;
                    }
                case 722459:
                    {
                        if (!client.Inventory.HaveSpace(5))
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 3, stream);//1
                        client.Inventory.AddItemWitchStack(723903, 0, 5, stream);//2
                        client.Inventory.Add(stream, 730002, 2, 2);
                        client.CreateBoxDialog("You received 3 Exp Ball, 5 Saddles and 2 (+2)Stone!");
                        break;
                    }
                case 722464:
                    {
                        if (!client.Inventory.HaveSpace(5))
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 3, stream);//1
                        client.Inventory.AddItemWitchStack(723903, 0, 4, stream);//2
                        client.Inventory.Add(stream, 730001, 3, 1);
                        client.CreateBoxDialog("You received 3 Exp Ball, 4 Saddles and 3 (+1)Stone!");
                        break;
                    }
                case 722469:
                    {
                        if (!client.Inventory.HaveSpace(5))
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 2, stream);//1
                        client.Inventory.AddItemWitchStack(723903, 0, 3, stream);//2
                        client.Inventory.Add(stream, 730001, 2, 1);
                        client.CreateBoxDialog("You received 2 Exp Ball, 3 Saddles and 2 (+1)Stone!");
                        break;
                    }
                case 3002559:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.CreateExpProtection(stream, 3600);
                        client.CreateBoxDialog("You received 60 EXP protection for 1 hour.");
                        break;
                    }
                case 722474:
                    {
                        if (!client.Inventory.HaveSpace(5))
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 1, stream);//1
                        client.Inventory.AddItemWitchStack(723903, 0, 3, stream);//2
                        client.Inventory.Add(stream, 730001, 1, 1);
                        client.CreateBoxDialog("You received 1 Exp Ball, 3 Saddles and 1 (+1)Stone!");
                        break;
                    }

                case 720680:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.GainExpBall(600, false);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.SendSysMesage("You used the Heaven Pill and received EXP worth 2 and 1/2 EXP Balls!", MsgMessage.ChatMode.System);
                        break;
                    }

                case 720698:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.GainExpBall(3200);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Moon Pill and received the EXP worth 8 and 1/3 EXP Balls!");
                        break;
                    }



                case 720692:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Wind Pill and received the EXP worth 5 EXP Balls!");


                        break;
                    }





                case 720686:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.GainExpBall(300);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Mystery Pill and received EXP worth 3 and 1/3 EXP Balls!");

                        break;
                    }
                case 3006284:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3006284, 10))
                        {
                            client.Inventory.Add(stream, 3001045);
                            client.CreateBoxDialog("You successfully combined 10*MysteryDewScrap into 1 MysteryDew.");
                            client.Inventory.RemoveStackItem(3006284, 10, stream);
                        }
                        else
                            client.CreateBoxDialog("Failed to combine. You should have at least 10 Mystery Dew Scraps to combine.");
                        break;
                    }
                case 721316:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 3, stream); ;
                        client.CreateBoxDialog("You received 3 Exp Balls.");
                        break;
                    }



                case 721317:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall2, 0, 3, stream);
                        client.CreateBoxDialog("You received 3 Exp Balls.");
                        break;
                    }





                case 3003132:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3003124, 1);

                        client.CreateBoxDialog("You received 1 Favored~Training~Pill");
                        break;
                    }


                case 3003133:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3003124, 2);

                        client.CreateBoxDialog("You received 2 Favored~Training~Pill");
                        break;
                    }

                case 3003134:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(3003124, 0, 5, stream);

                        client.CreateBoxDialog("You received 5 Favored~Training~Pill");
                        break;
                    }

                case 3003135:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(3003124, 0, 10, stream);

                        client.CreateBoxDialog("You received 10 Favored~Training~Pill");
                        break;
                    }



                case 3003136:
                    {
                        if (!client.Inventory.HaveSpace(4))
                        {
                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(3003124, 0, 20, stream);

                        client.CreateBoxDialog("You received 20 Favored~Training~Pill");
                        break;
                    }



                case 3003137:
                    {
                        if (!client.Inventory.HaveSpace(5))
                        {
                            client.CreateBoxDialog("Please make 5 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(3003124, 0, 50, stream);

                        client.CreateBoxDialog("You received 50 Favored~Training~Pill");
                        break;
                    }

                case 720668:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.GainExpBall(100);
                        client.CreateBoxDialog("You used the Magic Ball and received EXP worth 1/6 of an EXP Ball from Demon Box.");
                        break;
                    }


                case 720669:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.GainExpBall(500);
                        client.CreateBoxDialog("You used the Super Ball and received EXP worth 5/6 of an EXP Ball.");
                        break;
                    }


                case 720670:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.GainExpBall(150);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Ultra Ball and received EXP worth 1 and 2/3 EXP Balls!");
                        break;
                    }
                case 3001044:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (client.Inventory.Contain(3001045, 1))
                        {
                            client.Inventory.Remove(3001045, 1, stream);
                            client.CreateBoxDialog("Success ! You successfully combined (MysteryFruit and MysteryDew) and received 3 Attribute Point`s.");
                            client.Player.AddExtraAtributes(stream, 3);
                        }
                        else
                        {
                            byte count = (byte)Program.GetRandom.Next(1, 15);
                            switch (count)
                            {
                                case 1:
                                    {
                                        client.CreateBoxDialog("Success ! You successfully combined all fragments and received 3 Attribute Point`s.");
                                        client.Player.AddExtraAtributes(stream, 3);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.CreateBoxDialog("Success ! You successfully combined all fragments and received 2 Attribute Point`s.");
                                        client.Player.AddExtraAtributes(stream, 2);
                                        break;
                                    }
                                default:
                                    {
                                        client.CreateBoxDialog("Success ! You successfully combined all fragments and received 1 Attribute Point`s.");
                                        client.Player.AddExtraAtributes(stream, 1);
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case 3004895:
                case 3004896:
                case 3004897:
                case 3004898:
                    {
                        if (client.Inventory.Contain(3004895, 1)
                            && client.Inventory.Contain(3004896, 1) && client.Inventory.Contain(3004897, 1)
                            && client.Inventory.Contain(3004898, 1))
                        {
                            client.Inventory.Remove(3004895, 1, stream);
                            client.Inventory.Remove(3004896, 1, stream);
                            client.Inventory.Remove(3004897, 1, stream);
                            client.Inventory.Remove(3004898, 1, stream);

                            client.Inventory.Add(stream, 3001044);
                            client.SendSysMesage("You~successfully~combined~the~Strength~Token,~Spirit~Token,~Vitality~Token,~and~Agility~Token~into~a~Mystery~Fruit!", MsgMessage.ChatMode.System);


                        }
                        else
                            client.CreateBoxDialog("Failed o combine. Make sure you have the Strength Token, Spirit Token, Vitality Token, and Agility Token!");

                        break;
                    }
                case 721920:
                    {
                        if (client.Player.Map == 1001)
                        {
                            if (DateTime.Now > client.Player.FerventPill.AddSeconds(300))
                            {
                                client.Player.AddFlag(MsgUpdate.Flags.Poisoned, 300, true, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "900x100");
                                client.SendSysMesage("Fervent Pill is in effect now. But you will lose 10 percent of current HP every 10 seconds.");
                            }
                            else
                            {
                                client.SendSysMesage("Fervent Pill will work for 1 minute every time you use it. It is still effective now. You don`t need to use it again.");
                            }
                        }
                        else
                        {
                            client.SendSysMesage("You need to use it in Mystic Castle.");
                        }
                        break;
                    }
                case 3007285:
                    {
                        if (!client.Inventory.HaveSpace(15))
                        {
                            client.CreateBoxDialog("Please make 15 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3002029, 15);
                        client.SendSysMesage("You received 15 ProtectionPills!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 3007269:
                    {
                        if (!client.Inventory.ContainItemWithStack(item.UID, 8))
                        {
                            client.SendSysMesage("You~need~8~scraps~to~merge.");
                            return;
                        }
                        client.Inventory.RemoveStackItem(3007269, 8, stream);
                        client.Inventory.AddRefinaryItem(3005360, false, stream);
                        client.SendSysMesage("You gained FrozenChiPill.", MsgMessage.ChatMode.System);
                        break;
                    }
                case 3007286:
                    {
                        if (!client.Inventory.HaveSpace(4))
                        {
                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(1003010, 0, 20, stream);
                        client.SendSysMesage("You received 20 7Star Ointments (B)!", MsgMessage.ChatMode.System);

                        break;
                    }
                case 3007434:
                    {
                        if (!client.Inventory.HaveSpace(4))
                        {
                            client.CreateBoxDialog("Please make 4 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(1004010, 0, 20, stream);
                        client.SendSysMesage("You received 20 Serenity Pills(B)!", MsgMessage.ChatMode.System);
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
