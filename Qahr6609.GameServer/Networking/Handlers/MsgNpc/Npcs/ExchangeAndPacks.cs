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


        /// <summary>CP exchange limit, counted in CPs (B) received over all exchange rates (Player.ExchangeNormalAvaliability).</summary>
        const uint CPExchangeLimit = 34000;
        /// <summary>One CP exchange: the limit is checked against the single exchange counter, the CPs are checked,
        /// and for coupon rates one coupon is consumed before anything is granted.</summary>
        static void CPExchange(Client.GameClient client, ServerSockets.Packet stream, Dialog data, uint cost, uint bound, uint coupon)
        {
            if ((ulong)client.Player.ExchangeNormalAvaliability + bound > CPExchangeLimit)
            {
                client.Player.MessageBox("You have reached your limit.", null, null);
                return;
            }
            if (client.Player.ConquerPoints < cost)
            {
                data.AddText("It seems you don`t have enough CPs.")
                    .AddOption("Alright.", 255).AddAvatar(1).FinalizeDialog();
                return;
            }
            if (coupon != 0 && !client.Inventory.Remove(coupon, 1, stream))
                return;
            client.Player.ConquerPoints -= cost;
            client.Player.BoundConquerPoints += (int)bound;
            client.Player.ExchangeNormalAvaliability += bound;
            data.AddText("Exchange done have a nice day.")
                .AddOption("Alright.", 255).AddAvatar(1).FinalizeDialog();
        }
        [NpcAttribute(NpcID.CPExchangeEnvoy)]
        public static void CPExchangeEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        /*if (DateTime.Now.Day < 31 && (DateTime.Now.Month > 10 || DateTime.Now.Month < 10))
                        {
                            data.AddText("Ended sorry.").AddOption("Go~grab~the~exchange~coupon.", 3).AddAvatar(1).FinalizeDialog();
                            break;
                        }
                        else*/
                        {
                            data.AddText("Happy CP Carnival! You can now multiply the value of your CPs~by exchanging them for CPs (B) here. Don`t forget to grab the\n")
                                .AddText("~CP Exchange Coupon on the event page for a higher exchange rate!\n")
                                .AddText("Time: September 14 to October 31, 2017\n")
                                .AddOption("Exchange~CPs~at~normal~rate.", 1)
                                .AddOption("Exchange~CPs~at~higher~rate.", 2)
                                .AddOption("Go~grab~the~exchange~coupon.", 3)
                                .AddAvatar(1).FinalizeDialog();
                            break;
                        }

                    }
                case 1:
                    {
                        if (client.Player.ExchangeNormalAvaliability >= CPExchangeLimit)
                        {
                            client.Player.MessageBox("You have reached your limit.", null, null);
                            break;
                        }
                        uint value = CPExchangeLimit - client.Player.ExchangeNormalAvaliability;
                        data.AddText("During the event, every hero is given an exchange limit of 34,000 CPs. Please choose the amount of CPs (B) to exchange for.\n")
                            .AddText("Available Exchange: " + value + " CPs\n")
                            .AddOption("500~CPs~for~1,500~CPs~(B).", 10).AddOption("5,000~CPs~for~15,000~CPs~(B).", 11).AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 10: CPExchange(client, stream, data, 500, 1500, 0); break;
                case 11: CPExchange(client, stream, data, 5000, 15000, 0); break;
                case 2:
                    {
                        if (client.Player.ExchangeNormalAvaliability >= CPExchangeLimit)
                        {
                            client.Player.MessageBox("You have reached your limit.", null, null);
                            break;
                        }
                        uint value = CPExchangeLimit - client.Player.ExchangeNormalAvaliability;
                        data.AddText("If you have a CP Exchange Coupon, you`ll be able to exchange CPs for CPs (B) at the corresponding rate.\n");
                        if (HasItems(client, 3304768, 1))//1:6
                        {
                            data.AddText("Your Coupon: CPExchangeCoupon(1:6)\n").AddOption("500~CPs~for~3000~CPs~(B).", 20).AddOption("5000~CPs~for~30000~CPs~(B).", 22);
                        }
                        if (HasItems(client, 3304767, 1))//1:5
                        {
                            data.AddText("Your Coupon: CPExchangeCoupon(1:5)\n").AddOption("500~CPs~for~2500~CPs~(B).", 30).AddOption("5000~CPs~for~25000~CPs~(B).", 32);
                        }
                        if (HasItems(client, 3304766, 1))//1:4
                        {
                            data.AddText("Your Coupon: CPExchangeCoupon(1:4)\n").AddOption("500~CPs~for~2000~CPs~(B).", 40).AddOption("5000~CPs~for~20000~CPs~(B).", 42);
                        }
                        data.AddText("Available Exchange: " + value + " CPs\n").AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 20: CPExchange(client, stream, data, 500, 3000, 3304768); break;
                case 22: CPExchange(client, stream, data, 5000, 30000, 3304768); break;
                case 30: CPExchange(client, stream, data, 500, 2500, 3304767); break;
                case 32: CPExchange(client, stream, data, 5000, 25000, 3304767); break;
                case 40: CPExchange(client, stream, data, 500, 2000, 3304766); break;
                case 42: CPExchange(client, stream, data, 5000, 20000, 3304766); break;
                case 3:
                    {
                        client.Send(stream.NpcReplyWebCreate(Program.ServerConfig.OfficialWebSite));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ScholarWise)]
        public static void ScholarWise(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (DateTime.Now.Day < 31 && (DateTime.Now.Month > 10 || DateTime.Now.Month < 10))
                            break;

                        data.AddText("New Conquer, New World! There is a quiz on the new expansion to make sure you`re ready.\n")
                            .AddText("~Come and answer 5 questions correctly every day to win nice rewards. Are you ready?\n")
                            .AddText("Duration: September 21 to October 31, 2017\n")
                            .AddText("Rewards: 10 Favored Training Pills (B), 500 Chi Points, 1 Bright Star Stone\n");
                        if (client.Player.AnswerdToday == 7)
                        {
                            data.AddOption("Start~the~quiz~(Failed~Today)", 255);
                            data.AddOption("Claim~quiz~rewards.~(Failed~Today)", 255);
                        }
                        else if (client.Player.AnswerdToday == 6)
                        {
                            data.AddOption("Start~the~quiz~(Completed)", 220);
                            data.AddOption("Claim~quiz~rewards.~(Claimed)", 211);
                        }
                        else if (client.Player.AnswerdToday == 5)
                        {
                            data.AddOption("Start~the~quiz~(Completed)", 1);
                            data.AddOption("Claim~quiz~rewards.", 210);
                        }
                        else
                        {
                            data.AddOption("Start~the~quiz!", 1).AddOption("Not~yet.", 2);
                        }
                        data.AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("1.What events will come together with the new expansion pack?\n")
                            .AddText("A.New Expansion Quiz\n")
                            .AddText("B.Pre-Expansion Quests\n")
                            .AddText("C.New CO Daily Pack\n")
                            .AddText("D.All the Above\n")
                            .AddOption("A", 204)
                            .AddOption("B", 204)
                            .AddOption("C", 204)
                            .AddOption("D", 203)
                            .AddAvatar(247).FinalizeDialog();
                        client.Player.nextquizop = 2;
                        client.Player.currentquizop = Option;
                        break;
                    }
                case 2:
                    {
                        data.AddText("2.After the new expansion pack, what class will gain new skill?\n")
                            .AddText("A.Monk\n")
                            .AddText("B.Pirate\n")
                            .AddText("C.Ninja\n")
                            .AddText("D.None\n")
                            .AddOption("A", 204)
                            .AddOption("B", 204)
                            .AddOption("C", 204)
                            .AddOption("D", 203)
                            .AddAvatar(247).FinalizeDialog();
                        client.Player.nextquizop = 3;
                        client.Player.currentquizop = Option;
                        break;
                    }
                case 3:
                    {
                        data.AddText("3.What's the difference between Life Transformation and Rebirth?\n")
                            .AddText("A.Life Transformation will transform gears into corresponding gears of the target class. Rebirth can't do that.\n")
                            .AddText("B.The level of the character will not be decreased in Life Transformation. Rebirth will decrease the character's level.\n")
                            .AddText("C.Both A and B are right.\n")
                            .AddText("D.There's no difference between these two, they are the same.\n")
                            .AddOption("A", 204)
                            .AddOption("B", 204)
                            .AddOption("C", 203)
                            .AddOption("D", 204)
                            .AddAvatar(247).FinalizeDialog();
                        client.Player.nextquizop = 4;
                        client.Player.currentquizop = Option;
                        break;
                    }
                case 4:
                    {
                        data.AddText("4.The visual effect of which skill remains the same in the new expansion pack?\n")
                            .AddText("A.Cannon Barrage\n")
                            .AddText("B.Archer Bane\n")
                            .AddText("C.Rage of War\n")
                            .AddText("D.Fatal Strike\n")
                            .AddOption("A", 204)
                            .AddOption("B", 204)
                            .AddOption("C", 203)
                            .AddOption("D", 204)
                            .AddAvatar(247).FinalizeDialog();
                        client.Player.nextquizop = 5;
                        client.Player.currentquizop = Option;
                        break;
                    }
                case 5:
                    {
                        data.AddText("5.The appearance of which class remains the same in the new expansion?\n")

                            .AddText("A.Dragon Warrior\n")
                            .AddText("B.Monk\n")
                            .AddText("C.Ninja\n")
                            .AddText("D.Warrior\n")
                            .AddOption("A", 203)
                            .AddOption("B", 204)
                            .AddOption("C", 204)
                            .AddOption("D", 204)
                            .AddAvatar(247).FinalizeDialog();
                        client.Player.nextquizop = 6;
                        client.Player.currentquizop = Option;
                        break;
                    }
                case 203:
                    {
                        // A correct answer counts once per question shown (currentquizop) and only while the quiz is open.
                        if (client.Player.currentquizop < 1 || client.Player.currentquizop > 5 || client.Player.AnswerdToday >= 5)
                            break;
                        client.Player.currentquizop = 0;
                        client.Player.AnswerdToday++;
                        if (client.Player.AnswerdToday == 5)
                        {
                            data.AddText("You`ve answered 5 questions correctly. Here are 10 Favored Training Pills (B), 500 Chi Points and 1 Bright Star Stone for you! Do you want to claim these rewards now?")
                                .AddOption("Yes.", 210).AddOption("Not~now.", 255).AddAvatar(247).FinalizeDialog();
                        }
                        else if (client.Player.AnswerdToday < 5)
                        {
                            data.AddText("Correct answer! Do you want to continue?")
                                .AddOption("Yes.", client.Player.nextquizop).AddOption("No.", 255).AddAvatar(247).FinalizeDialog();
                        }
                        break;
                    }
                case 204:
                    {
                        if (client.Player.currentquizop < 1 || client.Player.currentquizop > 5 || client.Player.AnswerdToday >= 5
                            || client.Player.WrongAnswers == null || client.Player.WrongAnswers.Length < 5)
                            break;
                        client.Player.WrongAnswers[client.Player.currentquizop - 1] -= 1;
                        data.AddText("Wrong answer. You need to try again if you still want the rewards.(" + client.Player.WrongAnswers[client.Player.currentquizop - 1] + " chance(s) left for this question)");
                        if (client.Player.WrongAnswers[client.Player.currentquizop - 1] > 0)
                        {
                            data.AddOption("Try~again.", client.Player.currentquizop);
                        }
                        else
                        { client.Player.AnswerdToday = 7; }
                        data.AddOption("I~need~a~break.", 255).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 210:
                    {
                        // Rewards only after 5 correct answers and only once (state 5 -> 6).
                        if (client.Player.AnswerdToday != 5)
                            break;
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                                .AddOption("Okay.", 255).AddAvatar(247).FinalizeDialog();
                            break;
                        }
                        client.Player.AnswerdToday = 6;
                        client.Inventory.AddItemWitchStack(3003124, 0, 10, stream);
                        client.Inventory.Add(stream, 3009001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 2);
                        client.Inventory.Add(stream, 3001413, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        break;
                    }
                case 211:
                    {
                        data.AddText("You`ve already claimed the rewards today. Please come and challenge tomorrow.")
                            .AddOption("Okay.", 255).AddAvatar(247).FinalizeDialog();
                        break;
                    }
                case 220:
                    {
                        data.AddText("You`ve already taken the quiz today. Please try again tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(247).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FortuneWheelPromoter)]
        public static void FortuneWheelPromoter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        /*if (DateTime.Now.Day < 31 && (DateTime.Now.Month > 10 || DateTime.Now.Month < 10))
                            break;*/
                        data.AddText("Wanna to find out what great fortune you have? Each hero can claim a Fortune Wheel from me during the event.\n")
                            .AddText("~Precious prizes including +Stones, Dragon Balls and Permanent Stones are waiting to be claimed, by you!\n")
                            .AddText("Duration: September 14 to October 31, 2017\n")
                            .AddOption("Claim~a~Fortune~Wheel.", 1)
                            .AddOption("I`ll~talk~to~you~later.", 255)
                            .AddAvatar(48).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.Contain(3301169, 1))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 3301169, 1);
                            }
                            else
                            {
                                client.Inventory.AddReturnedItem(stream, 3301169, 1);
                            }
                        }
                        else
                        {
                            data.AddText("The Fortune Wheel is already in your inventory. You don`t need to claim one more.")
                                .AddOption("Okay.", 255).AddAvatar(48).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BoundItemPromotion)]
        public static void BoundItemPromotion(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        /*if (DateTime.Now.Day < 31 && (DateTime.Now.Month > 10 || DateTime.Now.Month < 10))
                            break;*/
                        data.AddText("The Bound Item Sales has begun! Go and check what you like!\n")
                            .AddText("~If you don`t have enough CPs (B), you can find the CP Exchange Envoy. So, go and find what you like!\n")
                            .AddText("Time: September 14 to October 31, 2017\n")
                            .AddOption("Dragon~Ball~(B)~(" + Database.Server.DB + "~left)", 1)
                            .AddOption("+3~Stone~(B)~(" + Database.Server.Plus3Stone + "~left)", 10)
                            .AddOption("Permanent~Stone~(B)~(" + Database.Server.Parement + "~left)", 20)
                            .AddOption("Chi~Pack~(1000~Pts)~(B)~(" + Database.Server.ChiPack + "~left)", 30)
                            .AddOption("Tough~Drill~(B)~(" + Database.Server.ToughDrill + "~left)", 40)
                            .AddOption("Star~Drill~(B)~(" + Database.Server.StarDrill + "~left)", 50)
                            .AddOption("Talk~to~the~CP~Exchange~Envoy.", 60)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(3).FinalizeDialog();
                        break;
                    }
                #region DB
                case 1:
                    {
                        data.AddText("Each DragonBall(B) is at a price of 100 CPs (B). You can enter the order quantity in the box below.")
                            .AddText("~Don`t forget to check your inventory space before making an order.")
                            .AddText("Enter the order quantity (1-10): ")
                            .AddInput("(Enter a number)", 2)
                            .AddAvatar(3).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        byte Quantity = 0;
                        if (byte.TryParse(Input, out Quantity))
                        {
                            if (Database.Server.DB < Quantity)
                            {
                                data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            if (Quantity > 10 || Quantity < 1)
                            {
                                data.AddText("You gave wrong quantity. Please enter a number between 1 and 10.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                if (!client.Inventory.HaveSpace(Quantity))
                                {
                                    client.Player.MessageBox("Please clear at least 1 empty space(s) before making an order.", null, null);
                                    break;
                                }
                                int price = Quantity * 100;
                                if (client.Player.BoundConquerPoints >= price)
                                {
                                    client.Player.BoundConquerPoints -= price;
                                    client.Inventory.Add(stream, Database.ItemType.DragonBall, Quantity, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.SendSysMesage("You claimed " + Quantity + " DBs (B).");
                                    Database.Server.DB -= Quantity;
                                }
                                else
                                {
                                    data.AddText("You don`t have enough CPs (B) to buy DragonBall(B).")
                                   .AddOption("Alright.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region +3Stone
                case 10:
                    {
                        data.AddText("Each +3Stone(B) is at a price of 80 CPs (B). You can enter the order quantity in the box below.")
                            .AddText("~Don`t forget to check your inventory space before making an order.")
                            .AddText("Enter the order quantity (1-10): ")
                            .AddInput("(Enter a number)", 12)
                            .AddAvatar(3).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        byte Quantity = 0;
                        if (byte.TryParse(Input, out Quantity))
                        {
                            if (Database.Server.Plus3Stone < Quantity)
                            {
                                data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            if (Quantity > 10 || Quantity < 1)
                            {
                                data.AddText("You gave wrong quantity. Please enter a number between 1 and 10.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                if (!client.Inventory.HaveSpace(Quantity))
                                {
                                    client.Player.MessageBox("Please clear at least 1 empty space(s) before making an order.", null, null);
                                    break;
                                }
                                int price = Quantity * 80;
                                if (client.Player.BoundConquerPoints >= price)
                                {
                                    client.Player.BoundConquerPoints -= price;
                                    client.Inventory.Add(stream, 730003, Quantity, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.SendSysMesage("You claimed " + Quantity + " Stone[+3] (B).");
                                    Database.Server.Plus3Stone -= Quantity;
                                    break;
                                }
                                else
                                {
                                    data.AddText("You don`t have enough CPs (B) to buy +3Stone(B).")
                                   .AddOption("Alright.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Parament
                case 20:
                    {
                        data.AddText("Each PermanentStone(B) is at a price of 375 CPs (B). You can enter the order quantity in the box below.")
                            .AddText("~Don`t forget to check your inventory space before making an order.")
                            .AddText("Enter the order quantity (1-10): ")
                            .AddInput("(Enter a number)", 22)
                            .AddAvatar(3).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        byte Quantity = 0;
                        if (byte.TryParse(Input, out Quantity))
                        {
                            if (Database.Server.Parement < Quantity)
                            {
                                data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            if (Quantity > 10 || Quantity < 1)
                            {
                                data.AddText("You gave wrong quantity. Please enter a number between 1 and 10.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                if (!client.Inventory.HaveSpace(Quantity))
                                {
                                    client.Player.MessageBox("Please clear at least 1 empty space(s) before making an order.", null, null);
                                    break;
                                }
                                int price = Quantity * 375;
                                if (client.Player.BoundConquerPoints >= price)
                                {
                                    client.Player.BoundConquerPoints -= price;
                                    client.Inventory.Add(stream, 723694, Quantity, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.SendSysMesage("You claimed " + Quantity + " PermanentStone (B).");
                                    Database.Server.Parement -= Quantity;
                                    break;
                                }
                                else
                                {
                                    data.AddText("You don`t have enough CPs (B) to buy PermanentStone(B).")
                                   .AddOption("Alright.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region ChiPack
                case 30:
                    {
                        data.AddText("Each Chi~Pack~(1000~Pts)(B) is at a price of 150 CPs (B). You can enter the order quantity in the box below.")
                            .AddText("~Don`t forget to check your inventory space before making an order.")
                            .AddText("Enter the order quantity (1-10): ")
                            .AddInput("(Enter a number)", 32)
                            .AddAvatar(3).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        byte Quantity = 0;
                        if (byte.TryParse(Input, out Quantity))
                        {
                            if (Database.Server.ChiPack < Quantity)
                            {
                                data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            if (Quantity > 10 || Quantity < 1)
                            {
                                data.AddText("You gave wrong quantity. Please enter a number between 1 and 10.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                if (!client.Inventory.HaveSpace(Quantity))
                                {
                                    client.Player.MessageBox("Please clear at least 1 empty space(s) before making an order.", null, null);
                                    break;
                                }
                                int price = Quantity * 150;
                                if (client.Player.BoundConquerPoints >= price)
                                {
                                    client.Player.BoundConquerPoints -= price;
                                    client.Inventory.Add(stream, 3008193, Quantity, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.SendSysMesage("You claimed " + Quantity + " Chi~Pack~(1000~Pts) (B).");
                                    Database.Server.ChiPack -= Quantity;
                                    break;
                                }
                                else
                                {
                                    data.AddText("You don`t have enough CPs (B) to buy Chi~Pack~(1000~Pts)(B).")
                                   .AddOption("Alright.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region ToughDrill
                case 40:
                    {
                        data.AddText("Each ToughDrill(B) is at a price of 1700 CPs (B). You can enter the order quantity in the box below.")
                            .AddText("~Don`t forget to check your inventory space before making an order.")
                            .AddText("Enter the order quantity (1-10): ")
                            .AddInput("(Enter a number)", 42)
                            .AddAvatar(3).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        byte Quantity = 0;
                        if (byte.TryParse(Input, out Quantity))
                        {
                            if (Database.Server.ToughDrill < Quantity)
                            {
                                data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            if (Quantity > 10 || Quantity < 1)
                            {
                                data.AddText("You gave wrong quantity. Please enter a number between 1 and 10.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(3).FinalizeDialog();
                            }
                            else
                            {
                                if (!client.Inventory.HaveSpace(Quantity))
                                {
                                    client.Player.MessageBox("Please clear at least 1 empty space(s) before making an order.", null, null);
                                    break;
                                }
                                int price = Quantity * 1700;
                                if (client.Player.BoundConquerPoints >= price)
                                {
                                    client.Player.BoundConquerPoints -= price;
                                    client.Inventory.Add(stream, 1200005, Quantity, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.SendSysMesage("You claimed " + Quantity + " ToughDrill (B).");
                                    Database.Server.ToughDrill -= Quantity;
                                    break;
                                }
                                else
                                {
                                    data.AddText("You don`t have enough CPs (B) to buy ToughDrill(B).")
                                   .AddOption("Alright.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region StarDrill
                case 50:
                    {
                        data.AddText("Each StarDrill(B) is at a price of 999 CPs (B). You can enter the order quantity in the box below.")
                            .AddText("~Don`t forget to check your inventory space before making an order.")
                            .AddText("Enter the order quantity (1-10): ")
                            .AddInput("(Enter a number)", 52)
                            .AddAvatar(3).FinalizeDialog();
                        break;
                    }
                case 52:
                    {
                        byte Quantity = 0;
                        if (byte.TryParse(Input, out Quantity))
                        {
                            if (Database.Server.StarDrill < Quantity)
                            {
                                data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            if (Quantity > 10 || Quantity < 1)
                            {
                                data.AddText("You gave wrong quantity. Please enter a number between 1 and 10.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(3).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                if (!client.Inventory.HaveSpace(Quantity))
                                {
                                    client.Player.MessageBox("Please clear at least 1 empty space(s) before making an order.", null, null);
                                    break;
                                }
                                int price = Quantity * 999;
                                if (client.Player.BoundConquerPoints >= price)
                                {
                                    client.Player.BoundConquerPoints -= price;
                                    client.Inventory.Add(stream, 1200005, Quantity, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.SendSysMesage("You claimed " + Quantity + " StarDrill (B).");
                                    Database.Server.StarDrill -= Quantity;
                                    break;
                                }
                                else
                                {
                                    data.AddText("You don`t have enough CPs (B) to buy StarDrill(B).")
                                   .AddOption("Alright.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You gave wrong quantity. Please enter avaliable number.")
                                   .AddOption("Okay.", 255)
                                   .AddAvatar(3).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                case 60:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 336, 248, 19349);
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.Joyful1StonePack)]
        public static void Joyful1StonePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Do you want to directly claim a +1 Stone (B) from the pack, or pay 15 CPs to upgrade it to an unbound +2 Stone?")
                            .AddOption("Directly~claim~it.", 1).AddOption("15~CPs~to~upgrade.", 2).AddAvatar(73).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory");
                            break;
                        }
                        if (client.Inventory.Remove(3301223, 1, stream))
                        {
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+1~Bound~Stone!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 15 && client.Inventory.Remove(3301223, 1, stream))
                        {
                            client.Player.ConquerPoints -= 15;
                            client.Inventory.Add(stream, 730002, 1, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+2~UnBound~Stone!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Joyful2StonePack)]
        public static void Joyful2StonePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Do you want to directly claim a +2 Stone (B) from the pack, or pay 50 CPs to upgrade it to an unbound +3 Stone?")
                            .AddOption("Directly~claim~it.", 1).AddOption("50~CPs~to~upgrade.", 2).AddAvatar(73).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory");
                            break;
                        }
                        if (client.Inventory.Remove(3301224, 1, stream))
                        {
                            client.Inventory.Add(stream, 730002, 1, 2, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+2~Bound~Stone!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 50 && client.Inventory.Remove(3301224, 1, stream))
                        {
                            client.Player.ConquerPoints -= 50;
                            client.Inventory.Add(stream, 730003, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+3~UnBound~Stone!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Joyful3StonePack)]
        public static void Joyful3StonePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Do you want to directly claim a +3 Stone (B) from the pack, or pay 150 CPs to upgrade it to an unbound +4 Stone?")
                            .AddOption("Directly~claim~it.", 1).AddOption("150~CPs~to~upgrade.", 2).AddAvatar(73).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory");
                            break;
                        }
                        if (client.Inventory.Remove(3301225, 1, stream))
                        {
                            client.Inventory.Add(stream, 730003, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+3~Bound~Stone!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 150 && client.Inventory.Remove(3301225, 1, stream))
                        {
                            client.Player.ConquerPoints -= 150;
                            client.Inventory.Add(stream, 730004, 1, 4, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+4~UnBound~Stone!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Joyful4StonePack)]
        public static void Joyful4StonePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Do you want to directly claim a +4 Stone (B) from the pack, or pay 450 CPs to upgrade it to an unbound +5 Stone?")
                            .AddOption("Directly~claim~it.", 1).AddOption("450~CPs~to~upgrade.", 2).AddAvatar(73).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory");
                            break;
                        }
                        if (client.Inventory.Remove(3301226, 1, stream))
                        {
                            client.Inventory.Add(stream, 730004, 1, 4, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+4~Bound~Stone!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 450 && client.Inventory.Remove(3301226, 1, stream))
                        {
                            client.Player.ConquerPoints -= 450;
                            client.Inventory.Add(stream, 730005, 1, 5, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+5~UnBound~Stone!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Joyful5StonePack)]
        public static void Joyful5StonePack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Do you want to directly claim a +5 Stone (B) from the pack, or pay 1350 CPs to upgrade it to an unbound +6 Stone?")
                            .AddOption("Directly~claim~it.", 1).AddOption("450~CPs~to~upgrade.", 2).AddAvatar(73).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory");
                            break;
                        }
                        if (client.Inventory.Remove(3301227, 1, stream))
                        {
                            client.Inventory.Add(stream, 730005, 1, 5, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+5~Bound~Stone!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 1350 && client.Inventory.Remove(3301227, 1, stream))
                        {
                            client.Player.ConquerPoints -= 1350;
                            client.Inventory.Add(stream, 730006, 1, 6, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 2);
                            client.SendSysMesage("You~received~a~+6~UnBound~Stone!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExchangeShop)]
        public static void ExchangeShop(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        MsgExchangeShop.ExchangeShop obj = new MsgExchangeShop.ExchangeShop();
                        obj.DwParam1 = 19424;
                        obj.DwParam2 = 7;
                        obj.DwParam3 = client.Player.UID;

                        DateTime now64 = DateTime.Now;
                        int hours = now64.Hour;
                        int minutes = now64.Minute;
                        int secounds = now64.Second;

                        int lefthours = 23 - hours;
                        int leftminutes = 60 - minutes;
                        int leftsecounds = 60 - secounds;

                        TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
                        TimeSpan nextday = new TimeSpan(DateTime.Now.AddHours(lefthours).AddMinutes(leftminutes).AddSeconds(leftsecounds).Ticks);

                        obj.DwParam4 = (uint)(nextday.TotalSeconds - now.TotalSeconds);
                        client.Send(stream.CreateExchangeShop(obj));

                        //data.AddText("<tip desc=Hau Kan has served in the Legion of Heroes for dozens of years. Even though he is very old now,he still wants to make contribution to the hunting evil, and acts as the Logistics Officer>Hau Kan</tip>:")
                        //    .AddText("I`m old, but I`m vigorous. If you need any supplies, find me.The devils in the Giant Chasm are digging for the <tip color=0xfff05050 desc=Collect#Chaos#Beads#by#killing#devils#in#the#Giant#Chasm.>Chaos Bead</tip>~which can enhance their power. If you`ve collected some Chaos Beads, just exchange them for supplies with me.")
                        //    .AddOption("Exchange~for~supplies.", 1).AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3303483, 1) || client.Inventory.Contain(3303485, 1) || client.Inventory.Contain(3303486, 1))
                        {
                            MsgExchangeShop.ExchangeShop obj = new MsgExchangeShop.ExchangeShop();
                            obj.DwParam1 = 21875;
                            obj.DwParam2 = 23;
                            obj.DwParam3 = client.Player.UID;

                            DateTime now64 = DateTime.Now;
                            int hours = now64.Hour;
                            int minutes = now64.Minute;
                            int secounds = now64.Second;

                            int lefthours = 23 - hours;
                            int leftminutes = 60 - minutes;
                            int leftsecounds = 60 - secounds;

                            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
                            TimeSpan nextday = new TimeSpan(DateTime.Now.AddHours(lefthours).AddMinutes(leftminutes).AddSeconds(leftsecounds).Ticks);

                            obj.DwParam4 = (uint)(nextday.TotalSeconds - now.TotalSeconds);
                            client.Send(stream.CreateExchangeShop(obj));

                            //client.MyExchangeShop.ShowBoughtItems(stream);
                        }
                        break;
                    }
            }
        }
        const uint SeniorAssistantMaxExchange = 6000000;
        [NpcAttribute(NpcID.SeniorAssistant)]
        public static void SeniorAssistant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            data.AddAvatar(148);
            switch (Option)
            {
                case 0:// quest item 711533
                    {



                        // data.AddText("you can give me your cps and I will automatically do the hard work for you. The exchange ratio is 1:50000. That means you give me 60 CPs, I donate for you 50000 gold. The Maxmum amount is 6,000,000 CPs.");
                        data.AddText("I'm CPS Changer i'm new here in this city i found here a lot of Bound Cps ! \n");

                        data.AddText("you can give me <tip color=0xfff05050> ");
                        data.AddText(" BoundCPS </tip> \n , you will take the half of the amount example : [ 1k CPs for 500 (B)Cps ] \n ");
                        data.AddText("So what about your opinion ?!");
                        data.AddInput("Insert amount:", 1);
                        data.AddOption("No thank you.", 255);
                        data.AddAvatar(329).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        uint input;
                        //  if (int.TryParse(Input, out Value))
                        if (uint.TryParse(Input, out input))
                        {
                            if (input < 60)
                            {
                                data.AddText("You need to input more or atleast 60 CPs..");
                                data.AddInput("Insert amount:", 1);
                                data.AddOption("No thank you.", 255);
                                data.AddAvatar(48).FinalizeDialog();
                                return;
                            }
                            if (input > SeniorAssistantMaxExchange)
                            {
                                data.AddText("The maximum amount per exchange is " + SeniorAssistantMaxExchange + " (B) CPs.");
                                data.AddInput("Insert amount:", 1);
                                data.AddOption("No thank you.", 255);
                                data.AddAvatar(48).FinalizeDialog();
                                return;
                            }
                            if (client.Player.BoundConquerPoints >= input)
                            {
                                client.NpcCpsInput = input;
                                data.AddText("Are you sure you want to Change " + input + " Bound CPs? That means I will change it for you " + (((ulong)input) / 2) + " CPS.");
                                data.AddOption("Yes", 2);
                                data.AddOption("No thank you.", 255);
                                data.AddAvatar(48).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You don't have that much CPs. Try again. \nThe exchange ratio is 1:50000. That means you give me 100 (B) CPs, I  change it for you 50 CPS. The Maxmum amount is 6,000,000 CPs.");
                                data.AddInput("Insert amount:", 1);
                                data.AddOption("No thank you.", 255);
                                data.AddAvatar(48).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Try again. \nThe exchange ratio is 1:50000. That means you give me 100 (B) CPs, I  change it for you 50 CPS. The minimum amount is 60 CPs.");
                            data.AddInput("Insert amount:", 1);
                            data.AddOption("No thank you.", 255);
                            data.AddAvatar(48).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        // The amount confirmed here is re-validated: 60..SeniorAssistantMaxExchange and covered by Bound CPs.
                        if (client.NpcCpsInput >= 60 && client.NpcCpsInput <= SeniorAssistantMaxExchange
                            && client.Player.BoundConquerPoints >= client.NpcCpsInput)
                        {
                            client.Player.BoundConquerPoints -= (int)client.NpcCpsInput;
                            client.Player.ConquerPoints += client.NpcCpsInput / 2;
                            //data.AddText("");
                            client.Player.MessageBox("You Changed " + client.NpcCpsInput + " Bound CPS for " + client.NpcCpsInput / 2 + " Normal Cps", null, null);
                            client.NpcCpsInput = 0;
                            client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgServer.MsgUpdate.DataType.ConquerPoints);
                            client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgServer.MsgUpdate.DataType.BoundConquerPoints);

                        }
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.GiftPackRetailer)]
        public static void GiftPackRetailer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (DateTime.Now >= Database.Server.RetailDuration)
                        {
                            data.AddText("Sorry, but this event has been ended.")
                           .AddOption("I`m~just~passing~by.", 255).AddAvatar(53).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hellow " + client.Player.Name + " if you are new in this world you have to check the redeem gift site, \n or you can claim your prize if you have entered code in website. ")
                        .AddOption("Check~My~Prize.", 1)
                        .AddOption("Go~To~Redeem~Page.", 2)
                       .AddOption("I`m~just~passing~by.", 255).AddAvatar(53).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.JustCreated)
                        {
                            if (Database.Server.RedeemActivated.Contains(client.Player.UID))
                            {
                                client.Inventory.Add(stream, 3003219);
                                client.Player.JustCreated = false;
                                Database.Server.RedeemActivated.Remove(client.Player.UID);
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but this event for beginners.")
                          .AddOption("Okay.", 255).AddAvatar(53).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.JustCreated)
                        {
                            client.SendSysMesage(Program.ServerConfig.XtremeTopLink, MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.white);
                        }
                        else
                        {
                            data.AddText("Sorry, but this event for beginners.")
                           .AddOption("Okay.", 255).AddAvatar(1).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}
