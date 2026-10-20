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
        [NpcAttribute(NpcID.SuperGuildWar)]
        public static void SuperGuildWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("The rules of Super Guild War it's same with the Guild War. It`s a Guild Tournament that takes place every Tuesday at 20:00 AM and will finish Tuesday at 21:00 AM.")
              .AddOption("Enter~the~Super~Guild~Area.", 6)
                         .AddOption("Claim The SuperGuildWar Reward.", 7)
                            .AddOption("Just~passing~by.", 255)
                        .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Alive)
                            client.Teleport(232, 385, 3868);
                        else
                        {

                            data.AddText("The tournament is offline at the moment.")
            .AddOption("Thank you.", 255).AddAvatar(110).FinalizeDialog();

                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID && client.Player.MyGuild != null && MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {

                            if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.LeaderReward > 0
                                 && !MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Contains(client.Player.UID))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make one more space in your inventory.")
                  .AddOption("let me check", 255).AddAvatar(110).FinalizeDialog();
                                    break;
                                }

                                MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Add(client.Player.UID);

                                MsgTournaments.MsgSchedules.SuperGuildWar.Winner.LeaderReward -= 1;
                                client.Player.ConquerPoints += ServerRate.SGuildwar;

                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopSuperGuildWar, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("" + client.Player.Name + " , GuildLideru from " + client.Player.MyGuild.GuildName + " was rewarded with " + ServerRate.SGuildwar.ToString() + " for the domination the SuperGuildWar.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                            }
                            else if (client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.DeputiLeaderReward > 0
                                && !MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                            {
                                if (!client.Inventory.HaveSpace(4))
                                {
                                    data.AddText("Please make 4 more spaces in your inventory.")
               .AddOption("Let me check.", 255).AddAvatar(110).FinalizeDialog();
                                    break;
                                }
                                //client.Inventory.Add(stream, Database.ItemType.PowerExpBall, 4);
                                MsgTournaments.MsgSchedules.SuperGuildWar.Winner.DeputiLeaderReward -= 1;
                                MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Add(client.Player.UID);
                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopSuperGuildWar, Role.StatusFlagsBigVector32.PermanentFlag, false);
                            }
                            else
                            {
                                data.AddText("Just the GuildLeader or DeputiLeader can claim the reward!")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your~guild~has~not~dominated~super~guild~area.~Super~Guild~war~will~be~held~on~every~Tuesday,~lasting~2~days.")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.WuxingOven)]
        public static void WuxingOven(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("A~good~set~of~equipment~can~help~you~a~lot~in~future~battles.~Is~there~something~I~can~do~for~you?")
                        .AddOption("Special~blessed~enchantment.", 1)
                        .AddOption("How~to~upgrade~bonus~level?", 14)
                        .AddOption("Enchanted~effects?", 15)
                        .AddOption("Refinery~effects?", 19)
                        .AddOption("Not~interested.", 255)
                        .FinalizeDialog();


                        break;
                    }
                case 19:
                    {
                        data.AddText("You can obtain Composition Points from minor items and make your main items reach the next bonus level on the Forging interface.")
                      .AddOption("How~to~refine~equipment?", 20)
                      .FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        data.AddText("There are two ways to refine items. \n1 .Right click the refined items and click the equipment with the same type to refine.")
                          .AddText("\n 2.Refine the equipment on the Forging interface.")
                    .AddOption("Okay.", 255)
                    .FinalizeDialog();

                        break;
                    }
                case 14:
                    {

                        data.AddText("You may promote the bonus level of items on the Forging interface, namely compose +n items. +n items have +n stats which")
                            .AddText("~can make your items much more powerful.")
                      .AddOption("I~see.", 255)
                      .FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        data.AddText("The enchanted weapon can increase up to 255 HP. The higher quality gem will enchant higher HP. The enchanted effects vary with different gems.")
                                 .AddOption("How~to~enchant?", 16)
                                 .AddOption("I~can~enchat~it~repeatedly?", 17)
                                 .AddOption("What~gems~do~I~need?", 18)
                                 .FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        data.AddText("You can promote the bonus level of weapon on the Forging interface. Put the weapon and the enchanted materials into the designated places, then begin to enchant.")
                            .AddOption("Okay.", 255)
                            .FinalizeDialog();

                        break;
                    }
                case 17:
                    {
                        data.AddText("Sure. You can re-enchant your weapon on the Forging interface. If the new attribute is higher, it will replace the old one, otherwise, it won`t replace.")
                              .AddOption("Okay.", 255)
                              .FinalizeDialog();

                        break;
                    }
                case 18:
                    {
                        data.AddText("They can be Dragon, Phoenix, Tortoise, Rainbow, Moon, Violet, Fury, or Kylin. The enchanted effects vary with the gem quality.")
                              .AddOption("Okay.", 255)
                              .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Just paying 10.000 CPs, you`re able to activate the Wuxing Oven and attach 1 blessed attribute on specific equipment. It`s different from that attributes upgraded with Tortoise Gems.")
                            .AddText("~So, which type of equipment would you like to bless?")
                       .AddOption("Garment.", 2)
                       .AddOption("Mount~Armor.", 4)
                       .AddOption("Star~Tower.", 6)
                       .AddOption("Heaven~Fan.", 8)
                       .AddOption("Riding~Crop.", 10)
                       .AddOption("Gourd/Magic~Bottle.", 12)
                         .AddOption("RightWeaponAccessory.", 100)
                       .AddOption("Lefit Acc.", 101)
                            //.AddOption("Wing.", 50)
                       .AddOption("I`ll~think~about~it.", 255)
                       .FinalizeDialog();

                        break;

                    }
                case 50:
                    {
                        data.AddText("Are you sure you want to pay 10.000 CPs to add 1 blessed attribute to the Wing?")
                        .AddOption("Yes.", 51)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("I~need~to~think~it~over.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 100:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.RightWeaponAccessory))
                        {
                            data.AddText("You need to equip RightWeaponAccessory, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 10000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeaponAccessory, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Wing.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the RightWeaponAccessory.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 10000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 101:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.LeftWeaponAccessory))
                        {
                            data.AddText("You need to equip LeftWeaponAccessory, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 10000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeaponAccessory, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in LeftWeaponAccessory.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the LeftWeaponAccessory.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 10000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        data.AddText("Are you sure you want to pay 10.000 CPs to add 1 blessed attribute to the Gourd?")
                        .AddOption("Yes.", 13)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("I~need~to~think~it~over.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Bottle))
                        {
                            data.AddText("You need to equip Gourd, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 10000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Bottle, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Gourd.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Gourd.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 10000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you sure you want to pay 10.000 CPs to add 1 blessed attribute to the Riding Crop?")
                       .AddOption("Yes.", 11)
                       .AddOption("No,~I~changed~my~mind.", 1)
                       .AddOption("I~need~to~think~it~over.", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.RidingCrop))
                        {
                            data.AddText("You need to equip Riding Crop, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 10000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RidingCrop, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Riding Crop.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Riding Crop.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 10000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 8:
                    {
                        data.AddText("Are you sure you want to pay 10.000 CPs to add 1 blessed attribute to the Heaven Fan?")
                        .AddOption("Yes.", 9)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("I~need~to~think~it~over.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Fan))
                        {
                            data.AddText("You need to equip Heaven Fan, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 10000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Fan, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Heaven Fan.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Heaven Fan.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 10000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        data.AddText("Are you sure you want to pay 10.000 CPs to add 1 blessed attribute to the Star Tower?")
                       .AddOption("Yes.", 7)
                       .AddOption("No,~I~changed~my~mind.", 1)
                       .AddOption("I~need~to~think~it~over.", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Tower))
                        {
                            data.AddText("You need to equip Star Tower, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 10000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Tower, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Star Tower.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Star Tower.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 10000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("Are you sure you want to pay 10.000 CPs to add 1 blessed attribute to the Mount Armor?")
                        .AddOption("Yes.", 5)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("I~need~to~think~it~over.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.SteedMount))
                        {
                            data.AddText("You need to equip Mount Armor, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 10000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.SteedMount, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Mount Armor.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Mount Armor.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 10000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to pay 10.000 CPs to add 1 blessed attribute to the garment?")
                        .AddOption("Yes.", 3)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("I~need~to~think~it~over.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Garment))
                        {
                            data.AddText("You need to equip Garment, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 10000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Garment, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Garment.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the garment.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 10000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PokerCpsCasino)]
        public static void PokerCpsCasino(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Greeting, I`m glad to be at your service. Win or lose is not that important. To win is not the ultimate aim of the game, happiness is what we have been pursuing. Here, I even more like to wish you a good health than a good luck!")
                       .AddOption("I~have~no~interest~in~it.", 1)
                       .AddOption("Enter~Poker~Room.", 2)
                       .AddOption("I~still~want~to~play.", 255)
                       .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        client.SendSysMesage("You are teleported to Twin City.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(298, 283, 1002);

                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You are teleported to the Poker Room.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(157, 119, 1858);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PokerCasinoHostess)]
        public static void PokerCasinoHostess(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
          
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Greeting, I`m glad to be at your service. Win or lose is not that important. To win is not the ultimate aim of the game, happiness is what we have been pursuing. Here, I even more like to wish you a good health than a good luck!")
                        .AddOption("I~have~no~interest~in~it.", 1)
                        .AddOption("Enter~VIP~Hall.", 2)
                            //.AddOption("Exchange~Silver~Into~Cps.", 3)
                        .AddOption("I~still~want~to~play.", 255)
                        .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.SendSysMesage("You are teleported to Twin City.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(298, 283, 1002);

                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You arrived on the VIP Hall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(55, 62, 1860);

                        break;
                    }
                case 3:
                    {
                        data.AddText("Greeting, I`m glad to be at your service. Exchanging silver into cps")
                           .AddOption("Exchange~1~(M)~into~10k~cps.", 4)
                           .AddOption("Exchange~10k~cps~into~1~(M).", 5)
                           .AddOption("I~still~want~to~play.", 255)
                            .AddAvatar(175).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Player.Money >= 1000000000)
                        {
                            client.Player.Money -= 1000000000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.ConquerPoints += 10000;
                        }
                        else
                        {
                            client.SendSysMesage("You don't have 1 kkk silver.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Player.Money += 1000000000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        else
                        {
                            client.SendSysMesage("You don't have 10 k cps.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.InnerPowerNpc)]
        public static void InnerPowerNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Reborn == 2)
                        {
                            if (client.Player.InnerPower.Stages[0].UnLocked == false)
                            {
                                data.AddText("To concentrate on the study of humankind`s potential, I completely")
                                    .AddText(" isolated myself from the outside world 100 years ago. Finally, I achieved the essence of Inner Power.")
                            .AddOption("How~to~acquire~Inner~Power?", 1)
                            .AddOption("Not~interested.", 255)
                            .AddAvatar(6).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Good to see you again. If you've finished the studying the Inner Power Book I gave")
                                   .AddText(" you last time, you can claim the next book.")
                           .AddOption("Claim the next book.", 10)
                           .AddOption("I'm just passing by.", 255)
                           .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Kid, you`re not strong enough to study my Inner Power Books. The unusual power may hurt you instead.")
                                   .AddText("If you`re really interested, find me when you reach at least Level 15 of 2nd-reborn.")
                           .AddOption("Alright.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        Role.Instance.InnerPower.Stage stage;
                        if (client.Player.InnerPower.GetLastStage(out stage))
                        {
                            if (stage.Complete)
                            {
                                if (stage.ID < Database.InnerPowerTable.Count)
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in your inventory")
                                           .AddOption("Let me check.", 255).AddAvatar(6)
                                          .FinalizeDialog();
                                        break;
                                    }
                                    var next_stage = Database.InnerPowerTable.Stages[stage.ID];
                                    var DbNeiGong = next_stage.NeiGongAtributes[0];
                                    var DbItem = Database.Server.ItemsBase[DbNeiGong.ItemID];
                                    if (client.Inventory.Contain(DbItem.ID, 1))
                                    {
                                        data.AddText("Kid, you need to train the power of " + DbItem.Name + " to the perfect level before")
                      .AddText(" you can claim the next book. Be patient, okay?")
                       .AddOption("Okay.", 255)
                      .AddAvatar(6).FinalizeDialog();
                                        break;
                                    }
                                    client.Inventory.Add(stream, DbItem.ID);
                                    client.SendSysMesage("You received an Inner Power Book, " + DbItem.Name + ". Hurry and go training your Inner Power with it!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    data.AddText("Meeting you was fate. I`d like present you a " + DbItem.Name + ". When you acquire its essence and reach the perfect level, you can")
                           .AddText(" find me to claim the next book.")
                            .AddOption("Thanks~a~lot!", 255)
                           .AddAvatar(6).FinalizeDialog();
                                }
                                else
                                {
                                    //You've completed the inner power concept .
                                    data.AddText("You've completed the inner power concept .")
                                         .AddOption("Thanks~a~lot.", 255).AddAvatar(6)
                                        .FinalizeDialog();
                                    //completed.
                                }
                            }
                            else
                            {
                                Role.Instance.InnerPower.Stage.NeiGong nei_gong;
                                if (stage.GetLastNeiGang(out nei_gong))
                                {
                                    if (nei_gong.Complete)
                                    {
                                        var db_stage = Database.InnerPowerTable.Stages[stage.ID - 1];
                                        foreach (var dbnei in db_stage.NeiGongAtributes)
                                        {
                                            if (dbnei.ID == (byte)(nei_gong.ID + 1))
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                       .AddOption("Let me check.", 255).AddAvatar(6)
                                                      .FinalizeDialog();
                                                    break;
                                                }
                                                var DbItem = Database.Server.ItemsBase[dbnei.ItemID];
                                                if (client.Inventory.Contain(DbItem.ID, 1))
                                                {
                                                    data.AddText("Kid, you need to train the power of " + DbItem.Name + " to the perfect level before")
                                  .AddText(" you can claim the next book. Be patient, okay?")
                                   .AddOption("Okay.", 255)
                                  .AddAvatar(6).FinalizeDialog();
                                                    break;
                                                }
                                                client.SendSysMesage("You received an Inner Power Book, " + DbItem.Name + ". Hurry and go training your Inner Power with it!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                                client.Inventory.Add(stream, DbItem.ID);
                                                data.AddText("Meeting you was fate. I`d like present you a " + DbItem.Name + ". When you acquire its essence and reach the perfect level, you can")
                                       .AddText(" find me to claim the next book.")
                                        .AddOption("Thanks~a~lot!", 255)
                                       .AddAvatar(6).FinalizeDialog();
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var db_stage = Database.InnerPowerTable.Stages[stage.ID - 1];
                                        foreach (var dbnei in db_stage.NeiGongAtributes)
                                        {
                                            if (dbnei.ID == (byte)(nei_gong.ID))
                                            {
                                                var DbItem = Database.Server.ItemsBase[dbnei.ItemID];
                                                data.AddText("Kid, you need to train the power of " + DbItem.Name + " to the perfect level before")
                                   .AddText(" you can claim the next book. Be patient, okay?")
                                    .AddOption("Okay.", 255)
                                   .AddAvatar(6).FinalizeDialog();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("During the 100 years, I created numerous concepts, but found it too complicated. Therefore, I embodies these concepts through books.")
                  .AddText("~It also requires Potency Points to make breakthrough while training the Inner Power. You can collect Potency Points from the Kingdom Missions.")
                            .AddOption("How~can~I~get~the~books?", 2)
                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 2:
                    {

                        var db_stage = Database.InnerPowerTable.Stages[0];
                        client.Inventory.Add(stream, db_stage.NeiGongAtributes[0].ItemID);

                        data.AddText("Meeting you was fate. I`d like present you a Universal Concept (A). When you acquire its essence and reach the perfect level, you can")
                            .AddText(" find me to claim the next book.")
                             .AddOption("Thanks~a~lot!", 255)
                            .AddAvatar(7).FinalizeDialog();

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PeachTree)]
        public static void PeachTree(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Player.ActivePick)
            {
                if (client.Player.QuestGUI.IsActiveQuest(1011))
                {
                    client.Player.AddPick(stream, "Pick", 5);
                }
            }
        }
        [NpcAttribute(NpcID.PeachTree2)]
        public static void PeachTree2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Player.ActivePick)
            {
                if (client.Player.QuestGUI.IsActiveQuest(1011))
                {
                    client.Player.AddPick(stream, "Pick", 5);
                }
            }
        }
        public static string GetQuestModes(Client.GameClient user, uint UID, byte Level)
        {
            string text = "Level~" + Level.ToString();
            if (user.Player.QuestGUI.CheckQuest(UID, MsgQuestList.QuestListItem.QuestStatus.Finished))
                text = "Completed";
            else if (user.Player.QuestGUI.CheckQuest(UID, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                text = "Accepted";
            return text;
        }
        [NpcAttribute(NpcID.DailyItem1)]
        public static void DailyItem1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            // Opened from the daily quest scroll: the scroll must be owned and teleports are refused on restricted maps.
            if (Program.BlockTeleportMap.Contains(client.Player.Map))
                return;
            if (!HasItems(client, 729087, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 1:
                    {
                        client.Teleport(746, 665, 1015);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(256, 238, 1002);
                        break;
                    }
            }
        }

        public static uint DailiHeavenCpsCost(uint Stage)
        {
            if (Stage == 1)
                return 5;
            if (Stage == 2)
                return 20;
            if (Stage == 3)
                return 35;
            if (Stage == 4)
                return 100;
            return 0;
        }
    }
}
