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
        [NpcAttribute(NpcID.JiangHuNpc)]
        public static void JiangHuNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Well, the Jiang Hu is quite chaotic. I`ll test your understanding with several questions. If you pass, you`re able to name a skill of your own and start your adventure as a Jiang Hu warrior!")
                                                       .AddOption("Okay.~I`m~ready!", 1)
                                                       .AddOption("Learn~about~the~Jiang~Hu.", 2)

                                                       .AddOption("I`ll~talk~to~you~later.", 255)
                                                       .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Reborn == 2 && client.Player.MyJiangHu == null)
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.JiangHuSetName,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(&action));
                        }
                        else
                        {
                            data.AddText("Sorry, you not have 2nd Reborn, or you are learning Jiang Hu,")
                             .AddOption("Ah,~I`ll~talk~to~you~later.", 255).AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }

                case 2:
                    {
                        data.AddText("Being a fighter of the Jiang Hu, you can train your Inner-strength after activating `Jiang` button at the character window. Each time you study will cost you 10 Study Points")
                                                     .AddText("~and 1 Free Course, and will also decrease your Talent Status by 1. Remember, you can only perform up to 10 free studies in a day. If you`re at Basic Status, you can only perform extra study with CPs.")
                                                        .AddOption("How~to~raise~my~Talent~Status?", 3)
                                                     .AddOption("How~to~collect~Free~Courses?", 4)
                                                           .AddOption("What~if~I~want~to~quit?", 5)
                                                                  .AddOption("I`ll~talk~to~you~later.", 255)
                                                     .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("You need to activate the `Jiang` mode, and defeat a player whose Talent Status is not lower than yours to raise your Status Level by 1,")
                                                   .AddText("~up to Level 4. PK in the `Jiang` mode, win or lose, will only affect your Talent Status. Different levels of your Status makes your name glow different colors.")
                                                           .AddOption("Learn~about~other~things.", 2)
                                                          .AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Jiang Hu fighters will gain up to 100 Free Courses by accumulating the Talent Points over time, whether online or not. The higher the Talent Status,")
                                                   .AddText("~the more the Talent Points. While stay online in Twin City, you`ll receive Talent Points, up to x30 more than received in other areas.")
                                                           .AddOption("Learn~about~other~things.", 2)
                                                          .AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("If you want to leave the Jiang Hu for a while, deactivate the `Jiang` mode and wait for 10 minutes to quit.")
                                                .AddText("~After that, you won`t receive threats from the Jiang Hu fighters, however, you can`t accumulate Talent Points. Remember, you`re welcome to return at anytime you want!")
                                                        .AddOption("Learn~about~other~things.", 2)
                                                       .AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(6).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ClanNpc)]
        public static void ClanNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.MyClan == null)
                        {

                            data.AddText("The Clan is not only a group, but also a bond between its members! You can have a maximum of 12 members in a clan at one time,")
                           .AddText("~this means 6 members and their Spouses. After you create and upgrade the Clan, you can gain the BP Bonus Level, which can be")
                           .AddText("~shared by Clan members.")
                               .AddOption("How~to~create~a~Clan?", 1)
                               .AddOption("I~have~no~interest.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Your current Clan Level is Level " + (byte)(client.Player.MyClan.Level + 1) + "");
                            if (Role.Instance.Clan.UpdateLevelAmount.Length > client.Player.MyClan.Level)
                                data.AddText("~You need " + Role.Instance.Clan.UpdateLevelAmount[client.Player.MyClan.Level] + " Silver`s in the Clan Fund to upgrade the Clan Level.");

                            data.AddText("~You can gain BP Bonus Level " + client.Player.MyClan.BP + " now: The members can share " + client.Player.MyClan.ProcentClanBp(client.Player.MyClan.BP) + " percent of the Battle Power margin between themselves and");
                            data.AddText("~the highest Battle Power team member in the same map at BP Bonus Level " + client.Player.MyClan.BP + ".");


                            data.AddOption("Upgrade~the~Clan~Level", 5)
                            .AddOption("Upgrade~the~BP~Bonus~Level", 7)
                            .AddOption("Disband~my~Clan", 10)
                            .AddOption("Change~my~Clan`s~name.", 30)
                            .AddOption("Leave.", 255)
                            .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 30:
                    {
                        data.AddText("Only~the~Clan~Leader~can~change~the~Guild`s~name,~which~will~take~effect~after~the~server~maintenance~on~the~second~day.~It`ll~cost~you~1075~CPs.~Deal?");
                        data.AddOption("Deal!", 31);
                        data.AddOption("Wait~a~minute.");
                        data.AddAvatar(39);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Please~enter~a~new~name~for~your~Clan.");
                        data.AddInput("Name:", 32);
                        data.AddOption("Cancel.");
                        data.AddAvatar(39);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.Player.MyClan != null)
                        {
                            if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                            {
                                if (client.Player.ConquerPoints < 1075)
                                {
                                    data.AddText("Sorry,~you~don`t~have~enough~CPs.");
                                    data.AddOption("My~bad.");
                                    data.AddAvatar(39);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    string clanname = Input;
                                    if (Role.Instance.Clan.AllowCreateClan(clanname))
                                    {
                                        client.Player.ConquerPoints -= 1075;
                                        Role.Instance.Clan.RegisterChangeName(client.Player.MyClan.ID, clanname);
                                        data.AddText("Done!~You~can~check~the~name~after~the~server~maintenance~tomorrow.");
                                        data.AddOption("Great!");
                                        data.AddAvatar(39);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.CreateBoxDialog("Failed~to~change~name~Clan.~The~name~has~been~used~or~you~use~invalid~characters.");
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Sorry,~you`re~not~the~Clan~Leader.");
                                data.AddOption("My~bad.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.MyClan != null)
                        {
                            if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                            {
                                data.AddText("Only the Clan Leader has the right to disband the Clan. Do you really want to disband your Clan?")
                                    .AddOption("Yes.", 11)
                       .AddOption("Of~course~not.", 255)
                       .AddAvatar(6).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You aren`t the Clan Leader. Only the Clan Leader can upgrade or disband the Clan and create or upgrade the BP Bonus Level.");
                                data.AddOption("I~see.").AddAvatar(6).FinalizeDialog();
                            }


                        }


                        break;
                    }
                case 11:
                    {
                        if (client.Player.ClanRank != (ushort)Role.Instance.Clan.Ranks.Leader)
                        {
                            data.AddText("You aren`t the Clan Leader. Only the Clan Leader can upgrade or disband the Clan and create or upgrade the BP Bonus Level.");
                            data.AddOption("I~see.").AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.MyClan != null)
                        {
                            client.Player.MyClan.DisbandClan(stream);
                            client.CreateBoxDialog("Your~Clan~has~been~disbanded.");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                        {
                            if (Role.Instance.Clan.UpdateBPAmount.Length > client.Player.MyClan.BP)
                            {
                                data.AddText("You have reached BP Bonus Level " + client.Player.MyClan.BP + ", and the Clan members can share " + client.Player.MyClan.ProcentClanBp(client.Player.MyClan.BP) + " percent of the Battle Power margin in the same map. Do you");
                                data.AddText("~want to upgrade the BP Bonus Level using " + Role.Instance.Clan.UpdateBPAmount[client.Player.MyClan.BP] + " in Clan Fund?");
                                data.AddOption("Yes.", 8);
                                data.AddOption("Not~right~now.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You have reahced the highest BP Bonus Level and the members can share 70 percent of the Battle Power margin in the same map.");
                                data.AddOption("Sounds~good!");
                                data.AddAvatar(39);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You can`t create or upgrade the BP Bonus Level. Only the Clan Leader can upgrade or disband the Clan and create or upgrade the");
                            data.AddText("~BP Bonus Level.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 8:
                    {
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                        {
                            if (client.Player.MyClan == null)
                                break;
                            uint Money = Role.Instance.Clan.UpdateBPAmount[client.Player.MyClan.BP];
                            if (client.Player.MyClan.Donation > Money)
                            {
                                client.Player.MyClan.Donation -= Money;
                                client.Player.MyClan.BP++;
                                if (Role.Instance.Clan.UpdateBPAmount.Length > client.Player.MyClan.BP)
                                {
                                    data.AddText("You have reached BP Bonus Level " + client.Player.MyClan.BP + ", and the Clan members can share " + client.Player.MyClan.ProcentClanBp(client.Player.MyClan.BP) + " percent of the Battle Power margin in the same map. Do you");
                                    data.AddText("~want to upgrade the BP Bonus Level using " + Role.Instance.Clan.UpdateBPAmount[client.Player.MyClan.BP] + " in Clan Fund?");
                                    data.AddOption("Yes.", 8);
                                    data.AddOption("Not~right~now.");
                                    data.FinalizeDialog();

                                }
                                else
                                {
                                    data.AddText("You have reahced the highest BP Bonus Level and the members can share 70 percent of the Battle Power margin in the same map.");
                                    data.AddOption("Sounds~good!");
                                    data.AddAvatar(39);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Oh, you don`t have " + Money + " in Clan Fund to gain the BP Bonus Level... Maybe I will help you next time when you have the");
                                data.AddText("~money.");
                                data.AddOption("Okay.~See~you.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You can`t create or upgrade the BP Bonus Level. Only the Clan Leader can upgrade or disband the Clan and create or upgrade the");
                            data.AddText("~BP Bonus Level.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                        {
                            if (Role.Instance.Clan.UpdateLevelAmount.Length > client.Player.MyClan.Level)
                            {
                                data.AddText("Your current Clan Level is Level " + (byte)(client.Player.MyClan.Level + 1) + " . Do you want to spend " + Role.Instance.Clan.UpdateLevelAmount[client.Player.MyClan.Level] + " in Clan Fund to upgrade the Clan?")
                                   .AddOption("Yes.", 6)
                                   .AddOption("Not~right~now.", 255)
                                   .AddAvatar(6).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You have reahced the highest BP Bonus Level and the members can share 70 percent of the Battle Power margin in the same map.");
                                data.AddOption("Sounds~good!");
                                data.AddAvatar(39);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You can`t upgrade the Clan. Only the Clan Leader can upgrade or disband the Clan, and create or upgrade the BP Bonus Level.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                        {
                            uint Money = Role.Instance.Clan.UpdateLevelAmount[client.Player.MyClan.Level];
                            if (client.Player.MyClan.Donation > Money)
                            {
                                client.Player.MyClan.Donation -= Money;
                                client.Player.MyClan.Level++;
                                if (Role.Instance.Clan.UpdateLevelAmount.Length > client.Player.MyClan.Level)
                                {
                                    data.AddText("Your current Clan Level is Level " + (byte)(client.Player.MyClan.Level + 1) + " . Do you want to spend " + Role.Instance.Clan.UpdateLevelAmount[client.Player.MyClan.Level] + " in Clan Fund to upgrade the Clan?")
                                       .AddOption("Yes.", 6)
                                       .AddOption("Not~right~now.", 255)
                                       .AddAvatar(6).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You have reahced the highest BP Bonus Level and the members can share 70 percent of the Battle Power margin in the same map.");
                                    data.AddOption("Sounds~good!");
                                    data.AddAvatar(39);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Your do not have " + Money + " silvers in you Clan.")
                                     .AddOption("Ah Ok", 255)
                                     .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You can`t upgrade the Clan. Only the Clan Leader can upgrade or disband the Clan, and create or upgrade the BP Bonus Level.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("After you reach Level 50, you can create a Clan with 500,000 silver, half of which will be the Clan Fund. You can recruit your")
                      .AddText("~members and build your elite group! Of course, you can only be in one Clan at a time. What do you think? Are you ready to")
                      .AddText("~start your new group?")
                          .AddOption("Create~a~Clan~now.", 2)
                          .AddOption("I`m~busy.", 255)
                          .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.MyClan == null)
                        {
                            if (client.Player.Money >= 500000)
                            {
                                if (client.Player.Level >= 50)
                                {
                                    data.AddText("To create a Clan, you need to think of a fabulous name for it! Type in the name you like, but remember the name should be");
                                    data.AddText("~less than 35 characters. A name is very important to your reputation. You should make it good enough so that people will");
                                    data.AddText("~tremble upon the very mention of it! So what`ll it be?");
                                    data.AddInput("My~Clan~Name...", 3).AddOption("Let~me~think...").AddAvatar(6).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You haven`t reached Level 50 and are unable to create a Clan. You should be strong enough to lead your Clan!");
                                    data.AddOption("Got~it.");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You don`t have enough money to create a Clan. Creating a Clan needs 500,000 silver. Half of which will be the Clan Fund.")
                         .AddText("~A Clan has to have money if you want to survive, you know. Can`t have a war without money!")
                             .AddOption("I~see.", 255)
                             .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You already created a Clan and are unable to create another one!");
                            data.AddOption("Okay.");
                            data.AddAvatar(39);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.Player.MyClan == null && client.Player.Money >= 500000)
                        {
                            string clanname = Input;
                            if (Role.Instance.Clan.AllowCreateClan(clanname))
                            {
                                data.AddText("Congratulations! You have created a Clan successfully! You should protect your Clan and fight with your Clan members for glory");
                                data.AddText("~and honor!");
                                data.AddOption("I~will.", 20);
                                data.AddAvatar(39);
                                data.FinalizeDialog();

                                client.Player.Money -= 500000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                new Role.Instance.Clan().Create(client, clanname, stream);
                            }
                            else
                            {
                                client.CreateBoxDialog("Failed~to~create~a~Clan.~The~name~has~been~used~or~you~haven`t~quit~a~Clan~for~at~least~a~day.");
                            }
                        }
                        else
                        {
                            data.AddText("You already created a Clan and are unable to create another one!");
                            data.AddOption("Okay.");
                            data.AddAvatar(39);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Now you can sign up at the Clan Guide in the name of your Clan to join in the Clan War once a day. You will obtain a Normal");
                        data.AddText("~Clan Pack as a reward if you win!");
                        data.AddOption("Nice!", 21);
                        data.AddAvatar(39);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        data.AddText("If you can occupy the map continuously, you will get better prizes! Besides the reward you get after you win the Clan War,");
                        data.AddText("~you will get a Refined Clan Pack on the 7th day, Unique Clan Pack on the 14th day, Elite Clan Pack on the 21st day, and a");
                        data.AddText("~Super Clan Pack on the 28th day! The stronger you are, the more you can win!");
                        data.AddOption("I`ll~try~my~best!");
                        data.AddAvatar(39);
                        data.FinalizeDialog();
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.GuildCreator)]
        public static void GuildCreator(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Greetings! I am the guild director, in charge of administrating and managing guilds. What business do you have with me?")
                       .AddText("Guild leaders can post up a guild recruitment ad to recruit new blood for their guilds! Soldiers without a guild can get the latest guild recruitment information from me, and then find a good guild to join!")
                           .AddOption("Create~a~guild", 1)
                           .AddOption("Disband~my~guild.", 2)
                          .AddOption("Union Operation.", 10)
                            .AddOption("Change Guild Name.", 100)

                           .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 100:
                    {
                        data.AddText("Write the name, you wana use for guild")
                     .AddInput("Name", 101).AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 101:
                    {
                        string GuildName = Input;
                        if (Role.Instance.Guild.AllowToCreate(GuildName))
                        {
                            if (client.Player.Level >= 90)
                            {
                                if (client.Player.Money >= 10000)
                                {

                                    if (client.Player.MyGuild != null && client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                                    {
                                        client.Player.Money -= 10000;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                        Role.Instance.Guild.Change(client, GuildName);
                                        client.Socket.Disconnect();
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, you have Guild!.")
                     .AddOption("Ah, Ok", 255)
                     .AddAvatar(6).FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You do not have 1000000 silvers with you.")
                      .AddOption("Ah, Ok", 255)
                      .AddAvatar(6).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry! you need level 90 to use my options")
                        .AddOption("Ah, Ok", 255)
                        .AddAvatar(6).FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Soory,the name is used!")
                          .AddOption("Ah, Ok", 255)
                          .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 10:
                    {
                        data.AddText("A great war wich involves millions of heroes is about the fire. If you have a Level 9+")
                            .AddText(" Guild ranking top in Capture the Flag of your server, you can pay me 3,000 Guild CPs to")
                            .AddText(" create a Union. It`s to gather the elites and picture a new world in your hands.");

                        if (client.Player.MyUnion == null)
                            data.AddOption("Create Union.", 11);
                        else
                        {

                            if (client.Player.InUnion && client.Player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor
                                && client.Player.MyUnion.IsKingdom == 1)
                                data.AddOption("Set Kingdom Title.", 13);
                            data.AddOption("Change Union`s Name.", 12);
                        }
                        data.AddOption("It`s time to strike!", 255)
                        .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        ActionQuery query = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = MsgServer.DialogCommands.SetKingdomTitle,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(&query));
                        break;
                    }
                case 12:
                    {
                        if (client.Player.UnionMemeber != null && client.Player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                        {

                            ActionQuery query = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.ChangeNameUnion,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(&query));
                        }
                        else
                            client.SendSysMesage("Failed to change the Union`s name. You're not the Union Leader.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        break;
                    }
                case 11:
                    {
                        if (client.Player.MyGuild != null && client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader
                            && client.Player.MyGuild.Info.ConquerPointFund >= 3000 && client.Player.MyGuild.Info.Level >= 9)
                        {
                            if (client.Player.MyGuild.CTF_Rank <= 9)
                            {
                                ActionQuery query = new ActionQuery()
                                {
                                    Type = ActionType.OpenDialog,
                                    ObjId = client.Player.UID,
                                    dwParam = MsgServer.DialogCommands.CreateUnion,
                                    wParam1 = client.Player.X,
                                    wParam2 = client.Player.Y
                                };
                                client.Send(stream.ActionCreate(&query));
                            }
                            else
                            {
                                data.AddText("I'm sorry but you don't meet the requirements to create a Union only guild leaders are able to do that.")
                     .AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I'm sorry but you don't meet the requirements to create a Union only guild leaders are able to do that.")
                      .AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Write the name, you wana use for guild")
                     .AddInput("Name", 3).AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.GuildRank != Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            data.AddText("Sorry you are not the Guild Leader.")
                     .AddOption("Ah, Ok", 255)
                     .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.InUnion)
                        {
                            data.AddText("Please quit union first time.")
        .AddOption("Ah, Okay.", 255)
        .AddAvatar(6).FinalizeDialog();
                            return;
                        }
                        if (client.Player.MyGuild != null && client.Player.MyGuildMember != null)
                        {
                            client.Player.MyGuild.Dismis(client, stream);
                        }
                        else
                        {
                            data.AddText("Sorry you not have guild.")
                      .AddOption("Ah, Ok", 255)
                      .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        string GuildName = Input;
                        if (Role.Instance.Guild.AllowToCreate(GuildName))
                        {
                            if (client.Player.Level >= 90)
                            {
                                if (client.Player.Money >= 1000000)
                                {
                                    if (client.Player.InUnion)
                                    {
                                        data.AddText("Please quit union first time.")
                    .AddOption("Ah, Okay.", 255)
                    .AddAvatar(6).FinalizeDialog();
                                        return;
                                    }
                                    if (client.Player.MyGuild == null)
                                    {

                                        client.Player.Money -= 1000000;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                        new Role.Instance.Guild(client, GuildName, stream);


                                    }
                                    else
                                    {
                                        data.AddText("Sorry, you have Guild!.")
                     .AddOption("Ah, Ok", 255)
                     .AddAvatar(6).FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You do not have 1000000 silvers with you.")
                      .AddOption("Ah, Ok", 255)
                      .AddAvatar(6).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry! you need level 90 to use my options")
                        .AddOption("Ah, Ok", 255)
                        .AddAvatar(6).FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Soory,the name is used!")
                          .AddOption("Ah, Ok", 255)
                          .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LoveStone)]
        public static void LoveStone(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Fate can bring lovers together. I hope you all can get married and live happily with your lovers. What can I do for you?")
                            .AddOption("Claim~double~EXP.", 1)
                            .AddOption("I~want~to~get~married.", 2)
                            .AddOption("Can~you~help~me~divorce?", 3)
                            .AddOption("Nothing~special.", 4)
                            .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.HeavenBlessing > 0)
                        {
                            if (client.Player.BDExp == 0)
                            {
                                client.Player.BDExp = 1;

                                client.Player.RateExp = 2;
                                client.Player.DExpTime = 3600;
                                client.Player.CreateExtraExpPacket(stream);
                                client.SendSysMesage("You got double exp. time of one hour.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            }
                            else
                            {
                                data.AddText("Sorry , but you're claimed today double Xp!").AddOption("Ah yes.", 255)
                        .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry , you not have Blessing!").AddOption("Ah yes.", 255)
                       .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("if you marriage left you nothing but regret and pain, then bring 3 Meteors,i can help you out.")
                         .AddOption("I have made up my ming", 8).AddOption("Let me reconsider it", 255)
                         .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 3))
                        {
                            if (client.Player.Spouse != "None")
                            {
                                if (!client.Inventory.Remove(Database.ItemType.Meteor, 3, stream)) break;
                                Client.GameClient Spouse;
                                if (Database.Server.GamePoll.TryGetValue(client.Player.SpouseUID, out Spouse))
                                {
                                    client.Player.Spouse = "None";
                                    client.Player.SpouseUID = 0;

                                    Spouse.Player.SpouseUID = 0;
                                    Spouse.Player.Spouse = "None";

                                    client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Spouse, false, new string[1] { "None" });
                                    Spouse.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Spouse, false, new string[1] { "None" });
                                    if (Spouse.Player.GuildRank == Role.Flags.GuildMemberRank.LeaderSpouse)
                                    {
                                        Spouse.Player.GuildRank = Role.Flags.GuildMemberRank.Member;
                                        Spouse.Player.View.SendView(Spouse.Player.GetArray(stream, false), false);
                                        Spouse.Player.MyGuild.Info.MyRank = (ushort)Role.Flags.GuildMemberRank.Member;
                                        Spouse.Player.MyGuild.SendThat(Spouse.Player);
                                    }
                                    return;
                                }
                                else
                                {
                                    client.ClientFlag |= Client.ServerFlag.RemoveSpouse;
                                    Database.ServerDatabase.LoginQueue.TryEnqueue(client);
                                }
                            }
                            else
                            {
                                data.AddText("You don`t have a wife").AddOption("Ah, you right", 255)
                             .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have 3 Meteors").AddOption("Ah, you right", 255)
                         .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }

                case 2:
                    {
                        data.AddText("Fate can bring lovers together. And everybody can live happily ever after... Ahhhh... What can I do for you?")
                           .AddOption("I~want~to~get~married.", 5)
                           .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Marriage~is~the~promise~forever.~Are~you~sure~that~you~want~to~spend~the~rest~of~your~life~with~your~lover?")
                          .AddOption("I~want~to~get~married.", 6)
                           .AddOption("I~prefer~being~single.", 255)
                          .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        data.AddText("Are~you~ready~to~propose?")
                         .AddOption("Yeah.~I~am~ready..", 7)
                          .AddOption("Let~me~think~it~over.", 255)
                         .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        data.AddText("Please click on your lover to propose.")
                          .AddOption("Okay.", 255)
                          .AddAvatar(6).FinalizeDialog();


                        ActionQuery action = new ActionQuery()
                        {
                            dwParam = MsgServer.CustomCommands.FlowerPointer,
                            Type = ActionType.OpenCustom,
                            ObjId = client.Player.UID
                        };
                        client.Send(stream.ActionCreate(&action));


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TwinCityConductress)]
        public static void TwinCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Where~are~you~heading~for?~I~can~teleport~you~for~a~price~of~100~silver.")
                        .AddOption("Phoenix Castle.", 1)
                        .AddOption("Ape City.", 2)
                        .AddOption("Desert City.", 3)
                        .AddOption("Bird Island.", 4)
                        .AddOption("Mine Cave.", 5)
                        .AddOption("Market.", 6)
                        .AddOption("Dragon Island.", 7)
                       .AddOption("Just~passing~by.", 255).AddAvatar(10).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Player.PKPoints < 29)
                        {
                            if (client.Player.Money >= 100)
                            {
                                client.Player.Money -= 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                switch (Option)
                                {
                                    case 1: client.Teleport(807, 523, 1002); break; //phoenix castle
                                    case 2: client.Teleport(524, 804, 1002); break; //ape mountain
                                    case 3: client.Teleport(69, 345, 1002); break; //desert city
                                    case 4: client.Teleport(526, 332, 1002); break; //birdisland
                                    case 5: client.Teleport(100, 358, 1002); break; // Mine Cave
                                    case 6: client.Teleport(217, 198, 1036); break; //market
                                    case 7: client.Teleport(547, 370, 1002); break; //dragon island

                                }
                            }
                            else
                            {
                                data.AddText("Hey! You don't have 100 gold do you?\nDon't you dare try to cheat me or I'll call the guards!")
                             .AddOption("Crazy!", 255)
                             .AddAvatar(10)
                             .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I will not help MURDERS!")
                           .AddOption("Okay...", 255)
                            .AddAvatar(10)
                           .FinalizeDialog();

                        }



                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.BlackSmith)]
        public static void BlackSmith(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WHTwin, client.Player.Class);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class, 823);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("Ahh, Warehouseman asked you here, huh? Yeah, you are in the right place! You know, weapons are the most important things in battle. Even the toughest warriors can`t survive without a weapon in their hands!")
                            .AddOption("Teach~me~more~about~weapons.", 2)
                            .AddOption("Buy~weapons.", 1)
                            .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("What a pity. I cannot own a good weapon! But that doesn`t mean you have to be the same! Please take this weapon as a gift. Well")
                   .AddOption("Okay!", 4).AddOption("Buy~weapons.", 1)
                   .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("What a pity. I cannot own a good weapon! But that doesn`t mean you have to be the same! Please take this weapon as a gift. Well")
                .AddOption("Okay!", 4).AddOption("Buy~weapons.", 1)
                .AddAvatar(10).FinalizeDialog();
                            break;
                        }


                        goto case 1;
                    }
                case 2:
                    {
                        data.AddText("Most weapons and equipments are upgrade, you can buy them here.")
            .AddOption("Okay", 3)
            .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.GainExpBall(500);

                                if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                    client.Inventory.Add(stream, 410016, 1);//leafBlade
                                else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                    client.Inventory.Add(stream, 410016, 1);//leafBlade
                                else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                    client.Inventory.Add(stream, 500006, 1);//BambooBow
                                else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                    client.Inventory.Add(stream, 601016, 1);//iron katana
                                else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                    client.Inventory.Add(stream, 610016, 1);//BambooPrayerBeads
                                else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                    client.Inventory.Add(stream, 611016, 1);//recruitRapier
                                else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                    client.Inventory.Add(stream, 421016, 1);//SpellBacksword 
                                else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                    client.Inventory.Add(stream, 626016, 1);

                                data.AddText("I`m selling the Headgear and armors. You should not miss the equipment in my shop! Hey, you`re still here! What else can I do for you? ")
                               .AddOption("I~want~to~learn~more~from~you.", 5)
                               .AddOption("Buy~weapons.", 1)
                               .AddAvatar(10).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make space in you inventory")
                      .AddOption("Ah~thank~you!.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("What a pity. I cannot own a good weapon! But that doesn`t mean you have to be the same! Please take this weapon as a gift. Well")
                .AddOption("Okay!", 4)
                .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("TBlacksmith~asked~you~to~speak~with~the~TC~Captain~to~see~if~there~is~anything~you~can~do.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = (uint)Role.Flags.NpcType.DialogAndGui,
                            Type = ActionType.OpenGuiNpc,
                            Timestamp = (int)client.ActiveNpc,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            dwParam3 = client.Player.Map
                        };
                        client.Send(stream.ActionCreate(&action));

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Pharmacist)]
        public static void Pharmacist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TheStorekeeper, client.Player.Class);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class, 815);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;

                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("We have all kinds of potions to restore your HP and MP, what do you need? ")
                         .AddOption("Tell~me~about~your~potions.", 2)
                         .AddOption("Buy~potions.", 1)
                         .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("We have all kinds of potions to restore your HP and MP, what do you need? ")
                          .AddOption("Tell~me~about~your~potions.", 2)
                          .AddOption("Buy~potions.", 1)
                          .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("What? Hey, don`t look at me in that way. What do you want? Tell me! Don`t just stand there.")
                .AddOption("I~want~to~learn~more~from~you.", 5)
                  .AddOption("Buy~potions.", 1)
                .AddAvatar(10).FinalizeDialog();
                            break;
                        }


                        goto case 1;
                    }
                case 7:
                    {
                        data.AddText("The Barber! She is a talented hairdresser. She is worth paying a visit to! Go! ~Now, you should pay a visit to the Barber. I`m sure you will get something from her. ")
             .AddOption("Okay!", 255)
             .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("Oops, I have told you all I know. You should pay a visit to the Barber. I`m sure you will get something from her.")
               .AddOption("Okay!", 6)
                 .AddOption("See~you~later.", 255)
               .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(Database.QuestInfo.AllQuests[815], 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~Pharmacist~asked~you~to~speak~with~the~Barber~in~Twin~City.", 1002, 284, 276, (uint)NpcID.Barber);

                        // client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        // client.Player.QuestGUI.SendAutoPatcher("The~Pharmacist~asked~you~to~speak~with~the~Barber~in~Twin~City.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
                case 2:
                    {
                        data.AddText("Besides MP and HP potions, I also have a special item called Scroll, which will take you to whatever city it is made for! ~You are lucky! I have just got something new for you!")
               .AddOption("Thanks~a~lot!", 3)
               .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.IncreaseExperience(stream, 300, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.AddItemWitchStack(1000000, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(1001000, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(1060020, 0, 2, stream);
                                client.Inventory.Add(stream, 721626, 1);
                                client.Inventory.Add(stream, 721625, 1);
                                client.SendSysMesage("You received 2 Twin City Gates, 3 Stanchers, 3 Agrypnotics, 1 Attack Pot(30 min) 1 DefencePot (30 min) and some experience", MsgMessage.ChatMode.System);
                                data.AddText("What? Hey, don`t look at me in that way. What do you want? Tell me! Don`t just stand there.")
              .AddOption("I~want~to~learn~more~from~you.", 5)
                .AddOption("Buy~potions.", 1)
              .AddAvatar(10).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 5 more spaces in your inventory")
                      .AddOption("let me to check.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = (uint)Role.Flags.NpcType.DialogAndGui,
                            Type = ActionType.OpenGuiNpc,
                            Timestamp = (int)client.ActiveNpc,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            dwParam3 = client.Player.Map
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TheStorekeeper)]
        public static void TheStorekeeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Armorer, client.Player.Class);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("I am here selling all kinds of Accessories and Boots. You should not miss the equipment in my shop. ")
                             .AddOption("Tell~me~about~your~equipment.", 2)
                             .AddOption("Buy~equipment.", 1)
                             .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("Ah ha? Why don`t you just leave here? Let me guess...You want to buy something? ")
                          .AddOption("Nah.~What~should~I~do~next?", 8)
                          .AddOption("Buy~equipment.", 1)
                          .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Oh, by the way, I am also selling Pickax for mining! Come to me if you need it!")
                .AddOption(" I~will!", 5)
                  .AddOption("Buy~equipment.", 1)
                .AddAvatar(10).FinalizeDialog();
                            break;
                        }


                        goto case 1;
                    }
                case 8:
                    {
                        data.AddText("Have you spoken with the Pharmacist? She has all kinds of potions that will be helpful to you in your journey.")
           .AddOption("Okay!", 255)
           .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 1:
                    {

                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = (uint)Role.Flags.NpcType.DialogAndGui,
                            Type = ActionType.OpenGuiNpc,
                            Timestamp = (int)client.ActiveNpc,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            wParam5 = (ushort)client.Player.Map
                        };
                        client.Send(stream.ActionCreate(&action));

                        break;
                    }
                case 2:
                    {
                        data.AddText("I have boots, necklaces and rings, which are quite necessary to improve your skills and prowess ")
                        .AddOption("I~learnt~something~new!", 3)
                        .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You are lucky, kid! I have just got something new for you!")
                      .AddOption("Thanks~a~lot!", 4)
                      .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.IncreaseExperience(stream, 1000, Role.Flags.ExperienceEffect.angelwing);

                                foreach (var item in ActiveQuest.Prize)
                                    client.Inventory.Add(stream, item);

                                data.AddText("Oh, by the way, I am also selling Pickax for mining! Come to me if you need it!")
                    .AddOption(" I~will!", 5)
                    .AddAvatar(10).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 3 more spaces in your inventory")
                      .AddOption("Ah~thank~you!.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }



                        break;
                    }
                case 5:
                    {
                        data.AddText("I have told you all I know. I suggest you to find the Pharmacist. She should have something for you there.")
                 .AddOption("Thanks!", 6)
                 .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~Storekeeper~asked~you~to~speak~with~the~Pharmacist~in~Twin~City.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Armorer)]
        public static void Armorer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            // var next_quest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 6425);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KungfuMaster, client.Player.Class);
            if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)2001, client.Player.Class, 3750);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
            var AtiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 6425);
            if (client.Player.QuestGUI.IsActiveQuest(AtiveQuest3.MissionId))
                ActiveQuest = AtiveQuest3;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("I`m selling Headgears and Armors. They are so wonderful. You should not miss them. ")
                            .AddOption("Tell~me~about~your~equipment.", 2)
                            .AddOption("Buy~equipment.", 1)
                            .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("Why are you still here? Pay a visit to that lady!")
                           .AddOption("Who?~Sorry,~I~forgot~the~name..", 10)
                           .AddOption("Buy~equipment.", 1)
                           .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m selling the Headgear and armors. You should not miss the equipment in my shop! Hey, you`re still here! What else can I do for you? ")
                             .AddOption("I~want~to~learn~more~from~you.", 5)
                             .AddOption("Buy~equipment.", 1)
                             .AddAvatar(10).FinalizeDialog();
                            break;
                        }

                        goto case 1;
                    }
                case 10:
                    {
                        data.AddText("The Storekeeper. She has many useful items. She is worth paying a visit to!")
                       .AddOption("I~won`t~forget~this~time.", 255)
                       .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        data.AddText("I have Armors and Headgears of all classes! Different classes use different kinds of equipment. Right click the equipment in my shop to buy it! ")
                           .AddOption("Thank~you~for~the~knowledge.", 3)
                           .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("You are lucky, kid! I have just got something new for you!")
                        .AddOption("Thank~you!.", 4)
                        .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.IncreaseExperience(stream, 1000, Role.Flags.ExperienceEffect.angelwing);
                                foreach (var item in ActiveQuest.Prize)
                                    client.Inventory.Add(stream, item);

                                data.AddText("I`m selling the Headgear and armors. You should not miss the equipment in my shop! Hey, you`re still here! What else can I do for you? ")
                               .AddOption("I~want~to~learn~more~from~you.", 5)
                               .AddOption("Buy~equipment.", 1)
                               .AddAvatar(10).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make space in you inventory")
                      .AddOption("Ah~thank~you!.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        data.AddText("You should know only one piece of armor is not enough to survive. Now pay a visit to the Storekeeper, will you? ")
                         .AddOption("Thanks~for~your~advice.", 6)
                         .AddOption("See~you~later!", 255)
                         .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        data.AddText("You should know only one piece of armor is not enough to survive. Now pay a visit to the Storekeeper, will you? ")
                            .AddOption("Thanks~for~your~advice.", 7)
                            .AddOption("See~you~later!", 255)
                            .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 7://812
                    {
                        var next_quest = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
                        client.Player.QuestGUI.Accept(next_quest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~Armorer~asked~you~to~speak~with~the~Storekeeper~in~Twin~City.", next_quest.FinishNpcId.Map, next_quest.FinishNpcId.X, next_quest.FinishNpcId.Y, next_quest.FinishNpcId.ID);



                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = 32,
                            Type = ActionType.OpenGuiNpc,
                            Timestamp = (int)client.ActiveNpc,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            wParam5 = (ushort)client.Player.Map
                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KungFuKing)]
        public static void KungFuKing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        var next_quest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 6425);
                        if (client.Player.QuestGUI.IsActiveQuest(6435))
                        {
                            data.AddText("I dare not say I am the best martial artist in the world, but I will never admit I rank the second. ")
                                .AddText("~Do you wonder where my power comes? You`ll know it after you try it.")
                                .AddOption("I~want~to~try!", 1)
                                .AddOption("I~doubt~it.", 255)
                                .AddAvatar(259).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(next_quest.MissionId, MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(6435, MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("As the descendant of Dragon, I recommend nunchaku as your best weapon. When weapon is ready, it`s time to fight. ")
                           .AddOption("I~can`t~wait~to~fight! ", 4)
                           .AddOption("I`m~not~ready~yet.", 255)
                           .AddAvatar(259).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I dare not say I am the best martial artist in the world, but I will never admit I rank the second.")
                                .AddOption("I~doubt~it.", 255)
                                .AddAvatar(259).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("My hero, you`re chosen by God of Dragon as the new generation for the mythical power.")
                            .AddText(" ~In the name of Dragon, I`ll grant you the spirit of fight,")
                            .AddText(" ~as well as the duty to sweep evil and darkness off the land. Are you ready?")
                                .AddOption(" I`m~always~ready!", 2)
                                .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(6435))
                            client.IncreaseExperience(stream, 100, Role.Flags.ExperienceEffect.angelwing);
                        data.AddText("Good! Now, I`m transferring my Power of Dragon to you. Feel it, and remember that Dragon`s ")
                            .AddText(" ~descendants equip nunchaku, and fight in the way of Jeet Kune Do. The saying `Using no")
                            .AddText(" ~way as way & Having no limitation as limitation` reveals the true meaning of Jeet Kune Do.")
                            .AddText(" ~Empty your mind, and fight freely. Your talent for martial arts will shine through severe trials.")
                             .AddOption("Thanks!", 3)
                             .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("As the descendant of Dragon, I recommend nunchaku as your best weapon. When weapon is ready, it`s time to fight. ")
                         .AddOption("I~can`t~wait~to~fight! ", 4)
                         .AddOption("I`m~not~ready~yet.", 255)
                         .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("There are many things for you to learn in this world! Um, I guess you want to learn more, huh? ")
                            .AddText("Let me see. What about equipment and armors? That will be useful! Now speak with the Armor in Twin City, ")
                            .AddText("he will tell you something about the armors.")

                             .AddOption("Wonderful!", 5)
                             .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        var next_quest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 6425);
                        client.Player.QuestGUI.Accept(next_quest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Kungfu~King~asked~you~to~speak~with~the~Armorer~in~Twin~City.", next_quest.FinishNpcId.Map, next_quest.FinishNpcId.X, next_quest.FinishNpcId.Y, next_quest.FinishNpcId.ID);


                        break;
                    }

            }
        }


        [NpcAttribute(NpcID.KungfuBoy)]
        public static void KungfuBoyNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            data.AddText("I started learning Kungfu when I was very young. Now, I have learned to master my Chi very well! Wanna have a try? .")
                                 .AddOption("Nah", 255)
                                 .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(804))
                        {
                            data.AddText("I started learning KungFu where i was very young.Now, i have learn to master my chi very well! Wanna have a try? .")
                                .AddOption("I got a message for you.", 1)
                                .AddOption("Nah", 255)
                                .AddAvatar(10).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(802, MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Okay, you have a mastered an important skill, But you still have a lot to learn before venturing out! ")
                           .AddText("You know, that will bring you many benefits if you have a tearn. That is the next thing you must learn.")
                           .AddOption("Who should i speak with? ", 4)
                           .AddOption("See you then ", 255)
                           .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I started learning Kungfu when I was very young. Now, I have learned to master my Chi very well! Wanna have a try? .")
                               .AddOption("Nah", 255)
                               .AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Oh, a message from the Recruit Master ? I see. He wana me to teach you how to master you Chi.")
                            .AddText(" Now, listen closely. Press the Ctrl button , move the mouse arrow to the place you wana to jump to,")
                            .AddText(" and left click. You should know , you can jump somewhere that you can`t just normally walk to.")
                            .AddOption("Thanks!", 2)
                           .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Good job! That is what i want you to do! Ok! Hope this kill will help you in the future.")
                          .AddOption("Thanks!", 3)
                         .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(804))
                            client.IncreaseExperience(stream, 300, Role.Flags.ExperienceEffect.angelwing);
                        data.AddText("Okay, you have a mastered an important skill, But you still have a lot to learn before venturing out! ")
 .AddText("You know, that will bring you many benefits if you have a tearn. That is the next thing you must learn.")
 .AddOption("Who should i speak with? ", 4)
 .AddOption("See you then ", 255)
 .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("General Zhao in Twin City will tel you more about the team system! Go speak with him !")
                         .AddOption("Great!", 5).AddOption("Not now.", 255)
                        .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(Database.QuestInfo.AllQuests[802], 0);
                        client.Player.QuestGUI.SendAutoPatcher("Kungfu~Master~asked~you~to~speak~with~General~Zhao~in~Twin~City!", 1002, 262, 247, 4488);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PheonixCityConductress)]
        public static void PheonixCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Where~are~you~heading~for?~I~can~teleport~you~for~a~price~of~100~silver.")
                            .AddOption("Twin~City", 1)
                            .AddOption("Market", 2)

                            .AddOption("Just~passing~by.", 255).AddAvatar(10).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            switch (Option)
                            {
                                case 1: client.Teleport(013, 376, 1011); break;
                                case 2: client.Teleport(217, 198, 1036); break;

                            }
                        }
                        else
                        {
                            data.AddText("I~can~teleport~you~for~a~price~of~100~silver.")
                         .AddOption("Ah yes.", 255).AddAvatar(10).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DesertCityConductress)]
        public static void DesertCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Where~are~you~heading~for?~I~can~teleport~you~for~a~price~of~100~silver.")
                            .AddOption("Twin~City", 1)
                            .AddOption("Mystic~Castle", 2)
                            .AddOption("Market", 3)
                            .AddOption("Just~passing~by.", 255).AddAvatar(10).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            switch (Option)
                            {

                                case 1: client.Teleport(973, 668, 1000); break;
                                case 2: client.Teleport(82, 321, 1000); break;
                                case 3: client.Teleport(217, 198, 1036); break;
                            }
                        }
                        else
                        {
                            data.AddText("I~can~teleport~you~for~a~price~of~100~silver.")
                         .AddOption("Ah yes.", 255).AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ApeCityConductress)]
        public static void ApeCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Where~are~you~heading~for?~I~can~teleport~you~for~a~price~of~100~silver.")
                            .AddOption("Twin~City", 1)
                            .AddOption("Market", 2)
                            .AddOption("Just~passing~by.", 255).AddAvatar(10).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            switch (Option)
                            {

                                case 1: client.Teleport(378, 13, 1020); break;

                                case 2: client.Teleport(217, 198, 1036); break;

                            }
                        }
                        else
                        {
                            data.AddText("You do not have 100 silvers.")
                         .AddOption("Aww!", 255).AddAvatar(10).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.BirdCityConductress)]
        public static void BirdCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Where~are~you~heading~for?~I~can~teleport~you~for~a~price~of~100~silver.")
                    .AddOption("Twin~City.", 1)
                    .AddOption("Market", 2)
                    .AddOption("Just~passing~by.", 255).AddAvatar(10).FinalizeDialog();

                        data.AddText("Where~are~you~heading~for?~I~can~teleport~you~for~a~price~of~100~silver.")
                           .AddOption("Twin~City.", 1)
                           .AddOption("Market", 2)
                           .AddOption("Just~passing~by.", 255).AddAvatar(10).FinalizeDialog();

                        break;
                    }
                default:
                    {

                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            switch (Option)
                            {
                                case 1: client.Teleport(1015, 710, 1015); break;

                                case 2: client.Teleport(217, 198, 1036); break;

                            }
                        }
                        else
                        {
                            data.AddText("I~can~teleport~you~for~a~price~of~100~silver.")
                         .AddOption("Ah yes.", 255).AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.BoxerConductor)]
        public static void BoxerConductor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("If you want to leave just tell me when you are ready.It's free.You will be teleported in the city where in before coming here.")
                                                 .AddOption("Yeah.Thanks.", 1)
                                                 .AddOption("No,I shall stay here.", 255)
                                                  .AddOption("Just~passing~by.", 255)
                                                  .AddAvatar(15)
                                                  .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        {
                            uint PrevMap = client.Player.PMap;
                            switch (PrevMap)
                            {
                                default:
                                    {

                                        client.Teleport(301, 278, 1002);
                                        break;
                                    }
                                case 1000:
                                    {

                                        client.Teleport(500, 650, 1000);
                                        break;
                                    }
                                case 1020:
                                    {

                                        client.Teleport(565, 562, 1020);
                                        break;
                                    }
                                case 1011:
                                    {

                                        client.Teleport(188, 264, 1011);
                                        break;
                                    }
                                case 1015:
                                    {
                                        client.Teleport(717, 571, 1015);
                                        break;
                                    }
                            }

                        }
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.MarketConductress)]
        public static void MarketConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Do you want to leave the market? I can teleport you for free.")
                          .AddOption("Yeah.Thanks.", 1)
                          .AddOption("No,I shall stay here.", 255)
                           .AddOption("Just~passing~by.", 255)
                           .AddAvatar(105)
                           .FinalizeDialog();



                        break;
                    }
                case 1:
                    {
                        {
                            uint PrevMap = client.Player.PMap;
                            switch (PrevMap)
                            {
                                default:
                                    {

                                        client.Teleport(313, 331, 1002);
                                        break;
                                    }
                                case 1000:
                                    {

                                        client.Teleport(500, 650, 1000);
                                        break;
                                    }
                                case 1020:
                                    {

                                        client.Teleport(565, 562, 1020);
                                        break;
                                    }
                                case 1011:
                                    {

                                        client.Teleport(188, 264, 1011);
                                        break;
                                    }
                                case 1015:
                                    {
                                        client.Teleport(717, 571, 1015);
                                        break;
                                    }
                                case 10137:
                                    {
                                        client.Teleport((ushort)Program.GetRandom.Next(93 - 3, 93 + 3), (ushort)Program.GetRandom.Next(410 - 3, 410 + 3), 10137);
                                        break;
                                    }
                            }

                        }
                        break;
                    }
            }
        }
        
        [NpcAttribute(NpcID.OflineTGNpc)]
        public static void OflineTGNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.Level == 140)
                        {
                            client.Send(stream.OfflineTGStatsCreate(0, 0, client.Player.Level, 0));
                            break;
                        }
                        var T1 = new TimeSpan(DateTime.Now.Ticks);
                        var T2 = new TimeSpan(client.Player.JoinOnflineTG.Ticks);
                        ushort minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                        minutes = (ushort)Math.Min((ushort)900, minutes);

                        ushort nextlevel = 0;
                        ulong nextExperience = client.CalcExpBall(minutes, out nextlevel);

                        var LevelDBExp = Database.Server.LevelInfo[Database.DBLevExp.Sort.User][(byte)client.Player.Level];

                        decimal give_exp = (decimal)((decimal)nextExperience / (decimal)LevelDBExp.Experience);

                        string exp = give_exp.ToString().Replace(".", "");

                        string aaexpp = "";
                        for (int x = 0; x < exp.Length; x++)
                        {
                            if (x == 8)
                                break;
                            aaexpp += exp[x];

                        }
                        client.Send(stream.OfflineTGStatsCreate(minutes, (ushort)(900 - minutes), nextlevel, (long)(int.Parse(aaexpp))));

                        break;
                    }

            }
        }
    }
}
