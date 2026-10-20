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

        [NpcAttribute(NpcID.IdealDesgner)]
        public static void IdealDesgner(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I can turn a Spirit of Legend into a random temporary Phoenix Cup or <tip color=0xfff05050 desc=A#certain#number#of#cup#fragments#can#be#combined#into#the#corresponding#Phoenix#Cup.>cup fragment</tip>.")
                            .AddText("~Equipping the Phoenix Cup will bring you attribute bonus. If you have any cup that you don`t need anymore, you can exchange it for <tip color=0xfff05050 desc=30#fragments#can#be#combined#into#a#90-day#Holy#Phoenix#Cup.>Holy Phoenix Cup Fragment</tip> or")
                            .AddText("~Chi Points.")
                   .AddOption("Use~the~Spirit~of~Legend.", 1)
                   .AddOption("Swap~for~cup~fragment~or~Chi.", 255)
                   .AddOption("Learn~more~about~Phoenix~Cups.", 30)
                   .AddOption("I`ll~talk~to~you~later.", 255)
                   .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3300056, 5))
                        {
                            data.AddText("f you really want the Phoenix Cup, you can also pay 5 Spirit of Legend to make a try.")
                  .AddOption("Deal!~Here~are~5xSpiritofLegend.", 5)
                  .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("It seems you don`t have 5x Spirit of Legend. If you really want the Phoenix Cup, you can also pay 150,000 CPs to make a try.")
                    .AddOption("Deal!~Here~are~150,000~CPs.", 2)
                    .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to try with 5x Spirit of Legend?")
            .AddOption("Yes.~I`ll~pay~the~5xSpiritofLegend.", 6)
            .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(3300056, 5))
                        {
                            if (!client.Inventory.Remove(3300056, 5, stream)) break;
                            byte rand = (byte)Program.GetRandom.Next(1, 3);
                            byte count = (byte)Program.GetRandom.Next(1, 3);
                            switch (rand)
                            {
                                case 1:
                                    {

                                        client.SendSysMesage("You received SilverPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300053, 0, count, stream);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.SendSysMesage("You received GoldenPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300054, 0, count, stream);
                                        break;
                                    }
                                default:
                                    {
                                        client.SendSysMesage("You received HolyPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300055, 0, count, stream);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You should give me a Spirit of Legend or 150,000 CPs before trying to make a phoenix cup.")
             .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to try with 150,000 CPs instead of a Spirit of Legend?")
              .AddOption("Yes.~I`ll~pay~the~money.", 3)
              .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 150000)
                        {
                            client.Player.ConquerPoints -= 150000;
                            byte rand = (byte)Program.GetRandom.Next(1, 3);
                            byte count = (byte)Program.GetRandom.Next(1, 3);
                            switch (rand)
                            {
                                case 1:
                                    {

                                        client.SendSysMesage("You received SilverPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300053, 0, count, stream);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.SendSysMesage("You received GoldenPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300054, 0, count, stream);
                                        break;
                                    }
                                default:
                                    {
                                        client.SendSysMesage("You received HolyPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300055, 0, count, stream);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You should give me a Spirit of Legend or 15,000 CPs before trying to make a phoenix cup.")
             .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("The Cross-server Elite PK Tournament will award 4 kinds of phoenix cups which grant different attributes.")
                          .AddText("~Which cup would you like to know about?")
                 .AddOption("Holy~Phoenix~Cup.", 31)
                 .AddOption("Golden~Phoenix~Cup.", 32)
                 .AddOption("Silver~Phoenix~Cup.", 33)
                 .AddOption("Bronze~Phoenix~Cup.", 34).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Holy Phoenix Cup's basic attributes: 1% Blessed, HP +1200, MP +1200, Defense +1000, \n")
                        .AddText("                  P-Strike +1%, M-Strike +1%, Break +1%,      Anti-break +1% \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.5 Draining Touch, Lv.2 Blood Spawn, \n")
                        .AddText("                   Lv.5 Free Soul, Lv.5 Killing Flash, Lv.3 Straight Life, HP +500")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        data.AddText("Golden Phoenix Cup's basic attributes: 1% Blessed, HP +1000, MP +1000, Defense +500 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.4 Draining Touch, Lv.2 Blood Spawn, \n")
                        .AddText("                  Lv.4 Free Soul, Lv.4 Killing Flash, Lv.3 Straight Life, HP +300 \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        data.AddText("Silver Phoenix Cup's basic attributes: 1% Blessed, HP +900, MP +900, Defense +300 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.3 Draining Touch, Lv.1 Blood Spawn, \n")
                        .AddText("                  Lv.3 Free Soul, Lv.3 Killing Flash, Lv.2 Straight Life \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        data.AddText("Bronze Phoenix Cup's basic attributes: 1% Blessed, HP +800, MP +800, Defense +150 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.2 Draining Touch, Lv.2 Killing Flash,  Lv.2 Free Soul \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.AstralPheonix)]
        public static void AstralPheonix(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("1.Join the Cross-Server Elite PK Tournament and you can get Elite Points. \n")
                            .AddText("2.Elite Points can be used to exchange for reward. \n")
                            .AddText("3.Top 8 players can get valuable reward such as exclusive halo and garments.")
                   .AddOption("I~see.", 255).AddAvatar(4).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CSElitePkManager)]
        public static void CSElitePkManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("The 1st Cross-server Elite PK Tournament will kick off soon. On the stage, the elite fighters will compete for the glory championship together with exclusive mount armor and title! \n")
                            .AddText("Admittance: 19:55 - 19:59 on the event date \n")
                            .AddText("Kick-off Time: 20:00 on the event date \n")
                            .AddText("Requirement: Level 100 \n")
                            .AddOption("Sign~me~up! ", 1)
                            .AddOption("Learn~about~the~rules.", 2)
                            .AddOption("Swap~points~for~rewards.", 10)
                            .AddOption("Claim~participation~reward.", 30)
                            .AddOption("View~the~ranking.", 20)
                   .AddOption("I`ll~talk~to~you~later. ", 255)
                   .AddAvatar(4).FinalizeDialog();

                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (MsgInterServer.Instance.CrossElitePKTournament.GetReward(client, stream))
                        {
                            client.CreateBoxDialog("I have added the coupons to inventory. Thanks for participating.");
                        }
                        else
                        {
                            data.AddText("You can`t claim the reward since you didn`t participate in the Cross-server Elite PK this time.")
                           .AddOption("Alright.", 255).AddAvatar(4).FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        List<Game.MsgTournaments.MsgEliteGroup.FighterStats> array = new List<MsgTournaments.MsgEliteGroup.FighterStats>();
                        if (MsgInterServer.Instance.CrossElitePKTournament.RanksElite.TryGetValue(3, out array))
                        {
                            stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, 3, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, (uint)(array.Count), client.Player.UID);

                            for (int x = 0; x < array.Count; x++)
                                stream.AddItemElitePkRanking(array[x], array[x].CrossEliteRank);
                            client.Send(stream.ElitePkRankingFinalize());
                        }
                        else
                        {
                            stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, 3, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, client.Player.UID);
                            client.Send(stream.ElitePkRankingFinalize());
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3300035, 1))
                        {
                            MsgExchangeShop.ExchangeShop obj = new MsgExchangeShop.ExchangeShop();
                            obj.DwParam1 = 19424;
                            obj.DwParam2 = 1;
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

                            client.MyExchangeShop.ShowBoughtItems(stream);
                            foreach (var item in Database.ExchangeShop.Items.Values)
                            {
                                MsgExchangeShopBuy.ExchangeShopBuy _obj = new MsgExchangeShopBuy.ExchangeShopBuy();
                                _obj.DwParam = 19424;
                                _obj.Index = (uint)item.Index;
                                _obj.CountItems = 0;//item.BoughtItems;
                                _obj.Type = item.RankType;
                                client.Send(stream.CreateBuyExchangeShop(_obj));
                            }



                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Contestants will be fighting in 3 level divisions. The higher division, the better the prize. Prizes are granted via mail.")
                              .AddText("~The server`s Elite PK Top 8 winners will be qualified for the cross-server tournament, while others need to register")
                              .AddText("~with me to get an accreditation card. Please note, leaving the match early will result in no reward for you.")
                              .AddOption("Prizes~for~L130+~division.", 3)
                              .AddOption("Prizes~for~L120-129~division.", 4)
                                .AddOption("Prizes~for~L100-119~division.", 5)
                            .AddOption("I`ll~talk~to~you~later. ", 255)
                            .AddAvatar(4).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Champion: 7-day 1% Blessed Astral Phoenix, 30-day champion title and halo,")
                              .AddText("      3 <tip color=0xfff05050 desc=It#requires#50#fragments#to#make#a#complete#Astral#Phoenix.>Astral Phoenix Fragments </tip>, and 5,000")
                              .AddText("<tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 7-day 1% Blessed Astral Phoenix, 30-day 2nd Place title and halo,")
                              .AddText("      2 Astral Phoenix Fragments, and 4,000 Elite PK Point Coupons.")
                              .AddText("3rd Place: 7-day 1% Blessed Astral Phoenix, 30-day 3rd Place title and halo,")
                              .AddText("      1 Astral Phoenix Fragment, and 3,500  Elite PK Point Coupons.")
                              .AddText("Top 8: 30-day Top 8 title and halo and 3,000 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")
                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Champion: 30-day champion halo and 3,000 <tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 30-day 2nd Place halo and 2,500 Elite PK Point Coupons.")
                              .AddText("3rd Place: 30-day 3rd Place halo and 2,000 Elite PK Point Coupons.")
                              .AddText("Top 8: 30-day Top 8 halo and 1,500 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")

                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Champion: 2,000 <tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 1,500 Elite PK Point Coupons.")
                              .AddText("3rd Place: 1,200 Elite PK Point Coupons.")
                              .AddText("Top 8: 1,000 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")
                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (MsgInterServer.Core.IsCrossPkOpen)
                        {
                            client.Player.SetLocationType = 1;
                            MsgInterServer.PipeClient.Connect(client, Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port);
                        }
                        else
                        {
                            data.AddText("Please sign up between 19:55 and 19:59 every Thursday night for the Cross Elite PK Tournament.")
                              .AddOption("Okay.", 255).AddAvatar(4).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HolyPhoenixCup)]
        public static void HolyPhoenixCup(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                  *Holy Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +1200					 Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +1200					 Lv.5 Draining Touch \n")
                            .AddText(" 					Defense +1000			 Lv.2 Blood Spawn \n")
                            .AddText(" 					P-Strike +1%				 Lv.5 Free Soul \n")
                            .AddText(" 					M-Strike +1%				 Lv.5 Killing Flash \n")
                            .AddText(" 					Break +1%					 Lv.3 Straight Life \n")
                             .AddText(" 				Anti-break +1%				 HP +500 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Holy Phoenix Cup`s Duration: 90 days")
                            //.AddOption("Change 50 legend spirit..", 255)
                   .AddOption("I~see ", 255)
                   .AddAvatar(318).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.GoldenPhoenixCup)]
        public static void GoldenPhoenixCup(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                  *Golden Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +1000					 Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +1000					 Lv.4 Draining Touch \n")
                            .AddText(" 					Defense +500			 Lv.2 Blood Spawn \n")
                            .AddText(" 					1% Blessed			    Lv.4 Free Soul \n")
                             .AddText("											Lv.4 Killing Flash \n")
                            .AddText(" 											Lv.3 Straight Life \n")
                             .AddText(" 										   HP +300 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Golden Phoenix Cup`s Duration: 90 days")
                   .AddOption("I~see ", 255)
                   .AddAvatar(317).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SilverPheoenixCup)]
        public static void SilverPheoenixCup(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);


            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                   *Silver Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +900					Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +900					Lv.3 Draining Touch \n")
                            .AddText(" 					Defense +300		  Lv.1 Blood Spawn \n")
                            .AddText(" 					1% Blessed			  Lv.3 Free Soul \n")
                             .AddText("										         Lv.3 Killing Flash \n")
                            .AddText(" 										         Lv.2 Straight Life \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Silver Phoenix Cup`s Duration: 60 days")
                   .AddOption("I~see ", 255)
                   .AddAvatar(316).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BronzePhoenixCup)]
        public static void BronzePhoenixCup(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                   *Bronze Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +800					Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +800					Lv.2 Draining Touch \n")
                            .AddText(" 					Defense +150		  Lv.2 Killing Flash \n")
                            .AddText(" 					1% Blessed			  Lv. Free Soul2 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 				        Bronze Phoenix Cup`s Duration: 30 days")

                   .AddOption("I~see ", 255)
                   .AddAvatar(315).FinalizeDialog();


                        break;
                    }

            }
        }

        //npc > market
        [NpcAttribute(NpcID.IdealDesgner2)]
        public static void IdealDesgner2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I can turn a Spirit of Legend into a random temporary Phoenix Cup or <tip color=0xfff05050 desc=A#certain#number#of#cup#fragments#can#be#combined#into#the#corresponding#Phoenix#Cup.>cup fragment</tip>.")
                            .AddText("~Equipping the Phoenix Cup will bring you attribute bonus. If you have any cup that you don`t need anymore, you can exchange it for <tip color=0xfff05050 desc=30#fragments#can#be#combined#into#a#90-day#Holy#Phoenix#Cup.>Holy Phoenix Cup Fragment</tip> or")
                            .AddText("~Chi Points.")
                   .AddOption("Use~the~Spirit~of~Legend.", 1)
                   .AddOption("Swap~for~cup~fragment~or~Chi.", 255)
                   .AddOption("Learn~more~about~Phoenix~Cups.", 30)
                   .AddOption("I`ll~talk~to~you~later.", 255)
                   .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3300056, 5))
                        {
                            data.AddText("f you really want the Phoenix Cup, you can also pay 5 Spirit of Legend to make a try.")
                  .AddOption("Deal!~Here~are~5xSpiritofLegend.", 5)
                  .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("It seems you don`t have 5x Spirit of Legend. If you really want the Phoenix Cup, you can also pay 150,000 CPs to make a try.")
                    .AddOption("Deal!~Here~are~150,000~CPs.", 2)
                    .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to try with 5x Spirit of Legend?")
            .AddOption("Yes.~I`ll~pay~the~5xSpiritofLegend.", 6)
            .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(3300056, 5))
                        {
                            if (!client.Inventory.Remove(3300056, 5, stream)) break;
                            byte rand = (byte)Program.GetRandom.Next(1, 3);
                            byte count = (byte)Program.GetRandom.Next(1, 3);
                            switch (rand)
                            {
                                case 1:
                                    {

                                        client.SendSysMesage("You received SilverPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300053, 0, count, stream);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.SendSysMesage("You received GoldenPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300054, 0, count, stream);
                                        break;
                                    }
                                default:
                                    {
                                        client.SendSysMesage("You received HolyPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300055, 0, count, stream);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You should give me a Spirit of Legend or 150,000 CPs before trying to make a phoenix cup.")
             .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to try with 150,000 CPs instead of a Spirit of Legend?")
              .AddOption("Yes.~I`ll~pay~the~money.", 3)
              .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 150000)
                        {
                            client.Player.ConquerPoints -= 150000;
                            byte rand = (byte)Program.GetRandom.Next(1, 3);
                            byte count = (byte)Program.GetRandom.Next(1, 3);
                            switch (rand)
                            {
                                case 1:
                                    {

                                        client.SendSysMesage("You received SilverPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300053, 0, count, stream);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.SendSysMesage("You received GoldenPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300054, 0, count, stream);
                                        break;
                                    }
                                default:
                                    {
                                        client.SendSysMesage("You received HolyPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300055, 0, count, stream);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You should give me a Spirit of Legend or 15,000 CPs before trying to make a phoenix cup.")
             .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("The Cross-server Elite PK Tournament will award 4 kinds of phoenix cups which grant different attributes.")
                          .AddText("~Which cup would you like to know about?")
                 .AddOption("Holy~Phoenix~Cup.", 31)
                 .AddOption("Golden~Phoenix~Cup.", 32)
                 .AddOption("Silver~Phoenix~Cup.", 33)
                 .AddOption("Bronze~Phoenix~Cup.", 34).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Holy Phoenix Cup's basic attributes: 1% Blessed, HP +1200, MP +1200, Defense +1000, \n")
                        .AddText("                  P-Strike +1%, M-Strike +1%, Break +1%,      Anti-break +1% \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.5 Draining Touch, Lv.2 Blood Spawn, \n")
                        .AddText("                   Lv.5 Free Soul, Lv.5 Killing Flash, Lv.3 Straight Life, HP +500")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        data.AddText("Golden Phoenix Cup's basic attributes: 1% Blessed, HP +1000, MP +1000, Defense +500 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.4 Draining Touch, Lv.2 Blood Spawn, \n")
                        .AddText("                  Lv.4 Free Soul, Lv.4 Killing Flash, Lv.3 Straight Life, HP +300 \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        data.AddText("Silver Phoenix Cup's basic attributes: 1% Blessed, HP +900, MP +900, Defense +300 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.3 Draining Touch, Lv.1 Blood Spawn, \n")
                        .AddText("                  Lv.3 Free Soul, Lv.3 Killing Flash, Lv.2 Straight Life \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        data.AddText("Bronze Phoenix Cup's basic attributes: 1% Blessed, HP +800, MP +800, Defense +150 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.2 Draining Touch, Lv.2 Killing Flash,  Lv.2 Free Soul \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AstralPheonix2)]
        public static void AstralPheonix2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("1.Join the Cross-Server Elite PK Tournament and you can get Elite Points. \n")
                            .AddText("2.Elite Points can be used to exchange for reward. \n")
                            .AddText("3.Top 8 players can get valuable reward such as exclusive halo and garments.")
                   .AddOption("I~see.", 255).AddAvatar(4).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CSElitePkManager2)]
        public static void CSElitePkManager2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("The 1st Cross-server Elite PK Tournament will kick off soon. On the stage, the elite fighters will compete for the glory championship together with exclusive mount armor and title! \n")
                            .AddText("Admittance: 19:55 - 19:59 on the event date \n")
                            .AddText("Kick-off Time: 20:00 on the event date \n")
                            .AddText("Requirement: Level 100 \n")
                            .AddOption("Sign~me~up! ", 1)
                            .AddOption("Learn~about~the~rules.", 2)
                            .AddOption("Swap~points~for~rewards.", 10)
                            .AddOption("Claim~participation~reward.", 30)
                            .AddOption("View~the~ranking.", 20)
                   .AddOption("I`ll~talk~to~you~later. ", 255)
                   .AddAvatar(4).FinalizeDialog();

                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (MsgInterServer.Instance.CrossElitePKTournament.GetReward(client, stream))
                        {
                            client.CreateBoxDialog("I have added the coupons to inventory. Thanks for participating.");
                        }
                        else
                        {
                            data.AddText("You can`t claim the reward since you didn`t participate in the Cross-server Elite PK this time.")
                           .AddOption("Alright.", 255).AddAvatar(4).FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        List<Game.MsgTournaments.MsgEliteGroup.FighterStats> array = new List<MsgTournaments.MsgEliteGroup.FighterStats>();
                        if (MsgInterServer.Instance.CrossElitePKTournament.RanksElite.TryGetValue(3, out array))
                        {
                            stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, 3, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, (uint)(array.Count), client.Player.UID);

                            for (int x = 0; x < array.Count; x++)
                                stream.AddItemElitePkRanking(array[x], array[x].CrossEliteRank);
                            client.Send(stream.ElitePkRankingFinalize());
                        }
                        else
                        {
                            stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, 3, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, client.Player.UID);
                            client.Send(stream.ElitePkRankingFinalize());
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3300035, 1))
                        {
                            MsgExchangeShop.ExchangeShop obj = new MsgExchangeShop.ExchangeShop();
                            obj.DwParam1 = 19424;
                            obj.DwParam2 = 1;
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

                            client.MyExchangeShop.ShowBoughtItems(stream);
                            /*foreach (var item in Database.ExchangeShop.Items.Values)
                            {
                                MsgExchangeShopBuy.ExchangeShopBuy _obj = new MsgExchangeShopBuy.ExchangeShopBuy();
                                _obj.DwParam = 19424;
                                _obj.Index = (uint)item.Index;
                                _obj.CountItems = 0;//item.BoughtItems;
                                _obj.Type = item.RankType;
                                client.Send(stream.CreateBuyExchangeShop(_obj));
                            }*/



                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Contestants will be fighting in 3 level divisions. The higher division, the better the prize. Prizes are granted via mail.")
                              .AddText("~The server`s Elite PK Top 8 winners will be qualified for the cross-server tournament, while others need to register")
                              .AddText("~with me to get an accreditation card. Please note, leaving the match early will result in no reward for you.")
                              .AddOption("Prizes~for~L130+~division.", 3)
                              .AddOption("Prizes~for~L120-129~division.", 4)
                                .AddOption("Prizes~for~L100-119~division.", 5)
                            .AddOption("I`ll~talk~to~you~later. ", 255)
                            .AddAvatar(4).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Champion: 7-day 1% Blessed Astral Phoenix, 30-day champion title and halo,")
                              .AddText("      3 <tip color=0xfff05050 desc=It#requires#50#fragments#to#make#a#complete#Astral#Phoenix.>Astral Phoenix Fragments </tip>, and 5,000")
                              .AddText("<tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 7-day 1% Blessed Astral Phoenix, 30-day 2nd Place title and halo,")
                              .AddText("      2 Astral Phoenix Fragments, and 4,000 Elite PK Point Coupons.")
                              .AddText("3rd Place: 7-day 1% Blessed Astral Phoenix, 30-day 3rd Place title and halo,")
                              .AddText("      1 Astral Phoenix Fragment, and 3,500  Elite PK Point Coupons.")
                              .AddText("Top 8: 30-day Top 8 title and halo and 3,000 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")
                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Champion: 30-day champion halo and 3,000 <tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 30-day 2nd Place halo and 2,500 Elite PK Point Coupons.")
                              .AddText("3rd Place: 30-day 3rd Place halo and 2,000 Elite PK Point Coupons.")
                              .AddText("Top 8: 30-day Top 8 halo and 1,500 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")

                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Champion: 2,000 <tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 1,500 Elite PK Point Coupons.")
                              .AddText("3rd Place: 1,200 Elite PK Point Coupons.")
                              .AddText("Top 8: 1,000 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")
                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (MsgInterServer.Core.IsCrossPkOpen)
                        {
                            client.Player.SetLocationType = 1;
                            MsgInterServer.PipeClient.Connect(client, Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port);
                        }
                        else
                        {
                            data.AddText("Please sign up between 19:55 and 19:59 every Thursday night for the Cross Elite PK Tournament.")
                              .AddOption("Okay.", 255).AddAvatar(4).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HolyPhoenixCup2)]
        public static void HolyPhoenixCup2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                  *Holy Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +1200					 Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +1200					 Lv.5 Draining Touch \n")
                            .AddText(" 					Defense +1000			 Lv.2 Blood Spawn \n")
                            .AddText(" 					P-Strike +1%				 Lv.5 Free Soul \n")
                            .AddText(" 					M-Strike +1%				 Lv.5 Killing Flash \n")
                            .AddText(" 					Break +1%					 Lv.3 Straight Life \n")
                             .AddText(" 				Anti-break +1%				 HP +500 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Holy Phoenix Cup`s Duration: 90 days")
                            //.AddOption("Change 50 legend spirit..", 255)
                   .AddOption("I~see ", 255)
                   .AddAvatar(318).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.GoldenPhoenixCup2)]
        public static void GoldenPhoenixCup2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                  *Golden Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +1000					 Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +1000					 Lv.4 Draining Touch \n")
                            .AddText(" 					Defense +500			 Lv.2 Blood Spawn \n")
                            .AddText(" 					1% Blessed			    Lv.4 Free Soul \n")
                             .AddText("											Lv.4 Killing Flash \n")
                            .AddText(" 											Lv.3 Straight Life \n")
                             .AddText(" 										   HP +300 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Golden Phoenix Cup`s Duration: 90 days")
                   .AddOption("I~see ", 255)
                   .AddAvatar(317).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SilverPheoenixCup2)]
        public static void SilverPheoenixCup2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);


            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                   *Silver Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +900					Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +900					Lv.3 Draining Touch \n")
                            .AddText(" 					Defense +300		  Lv.1 Blood Spawn \n")
                            .AddText(" 					1% Blessed			  Lv.3 Free Soul \n")
                             .AddText("										         Lv.3 Killing Flash \n")
                            .AddText(" 										         Lv.2 Straight Life \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Silver Phoenix Cup`s Duration: 60 days")
                   .AddOption("I~see ", 255)
                   .AddAvatar(316).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BronzePhoenixCup2)]
        public static void BronzePhoenixCup2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                   *Bronze Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +800					Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +800					Lv.2 Draining Touch \n")
                            .AddText(" 					Defense +150		  Lv.2 Killing Flash \n")
                            .AddText(" 					1% Blessed			  Lv. Free Soul2 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 				        Bronze Phoenix Cup`s Duration: 30 days")

                   .AddOption("I~see ", 255)
                   .AddAvatar(315).FinalizeDialog();


                        break;
                    }

            }
        }
    }
}
