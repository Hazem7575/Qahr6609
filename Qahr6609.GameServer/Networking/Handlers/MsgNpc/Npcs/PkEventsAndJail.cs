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
        [NpcAttribute(NpcID.ExitElitePk)]
        public static void ExitElitePk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                    .AddOption("TwinCity.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.TransformationID != 0)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.OnMyOwnServer == false)
                        {
                            client.Player.InElitePk = false;
                            client.Player.InTeamPk = false;
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Player.InElitePk = false;
                            if (client.Player.InTeamPk)
                            {
                                Game.MsgTournaments.MsgSchedules.TeamPkTournament.Remove(client);
                                client.Player.InTeamPk = false;
                            }
                            if (client.InSkillTeamPk())
                            {
                                Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Remove(client);
                            }
                            client.Teleport(300, 278, 1002);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureLeave)]
        public static void TreasureLeave(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                    .AddOption("TwinCity.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                           .AddOption("Yes.", 3)
                           .AddOption("Let~me~think...", 255)
                           .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 3:
                    {

                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExitSuperGuildWar)]
        public static void ExitSuperGuildWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                    .AddOption("TwinCity.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ItemShops)]
        public static void ItemShops(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello " + client.Player.Name + " There. Just Let Me Know If You Want To Go to Map Shop.")
                    .AddOption("Map Shops.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Teleport(089, 054, 1010);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ItemShopsTC)]
        public static void ItemShopsTC(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            //{
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello " + client.Player.Name + " there. Just let me know if You Want to go to TwinCity.")
                        .AddOption("TwinCity.", 2)
                        .AddOption("No~Wait...", 255)
                        .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Hey " + client.Player.Name + "Sure you want to leave in TwinCity?")
                                .AddOption("Yes.", 3)
                                .AddOption("Let~me~think...", 255)
                                .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(301, 279, 1002);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ExitCaptureTheFlag)]
        public static void ExitCaptureTheFlag(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                    .AddOption("TwinCity.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(300, 278, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SkillTeamPkManager)]
        public static void SkillTeamPkManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The new Skill Team Pk Tournament is open! It is held at 20:00,every")
                            .AddText(" Wednesday. The contestants will receive EXP worth 1 EXP Ball and 1000")
                            .AddText(" Honor Points for each round! The top 8 teams will receive extra prize!")
                            .AddOption("Tell~me~more~about~the~rules.", 1)
                            .AddOption("Sign~me~up!", 3)
                            .AddOption("I~want~to~claim~my~prize!", 5)
                       .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("There are limits on the Equipmen Level and Rank. Moreover, there are")
                           .AddText(" restrictions on Equipment Quality, Bonus Level, Sockets, Gems and ")
                           .AddText(" Blessing effects. Besides, the effects of the Sub-classes are ajusted, too.")
                           .AddText(" (Except for Apothecary.) By the way , the Battle Power shared from you Clan, ")
                           .AddText("  Mentor, and Guild won`t count; ")
                           .AddOption("Go ahead.", 10)
                      .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        data.AddText("here Skill Team Pk Tournament is held at 20:00 , every Wednesday. You need")
                          .AddText(" to team up with others to take part in it. Only the team leaders are capable of ")
                          .AddText(" claiming the rewards. You can up with friends in advance, or while")
                          .AddText("  (in the waiting area. People who don`t team up will be kicked out, when the")
                          .AddText(" contest starts. There are 4-groups: Level 130+, level 120-129, level 100-199  and level 1-99. ")
                          .AddOption("What else?.", 11)
                     .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        data.AddText("You can talk to me to enter the Waiting area from 19:45. No teams will be allowed to enter ")
                          .AddText(" after the contest start. The contest for level 130+ starts at 20:00, level 120-129 20:02,")
                          .AddText(" level 100-119 20:04 and level 1-99 20:06. Disbanding/creating a team during the contest will")
                          .AddText(" to disqualification. In knockout stage, each round lasts for 10 minutes")
                          .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Team == null)
                        {
                            data.AddText("You need to team up with others to take part in it. Please create team.")
                  .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            break;
                        }
                        if (!MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(client))
                        {
                            data.AddText("Please sign up between 19:45 and 20:06 every Wednesday night for the Skill PK Tournament.")
                            .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!MsgTournaments.MsgSchedules.SkillTeamPkTournament.GetReward(client, stream))
                            {
                                data.AddText("Sorry, you are not in top 8 in Skill Pk Tournament or you claimed the reward.")
                        .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make one space in your bag.")
                          .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TeamPkManager)]
        public static void TeamPkManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Attention! The Team Pk Tournament is held at 19:00, each Saturday. The Top 8")
                            .AddText("  teams will be handsomely rewarded, and each player in the team can")
                            .AddText(" receive EXP and 1000 HonorPoints affter each match.")
                            .AddOption("Tell~me~more~about~the~rules.", 1)
                            .AddOption("Sign~me~up!", 3)
                            .AddOption("I~want~to~claim~my~prize!", 5)
                       .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("The Team Pk Tournament start at 19:00 every Saturday, and teams will be divided")
                           .AddText("  into four grups: Level 1-99, Level 100-119, level 120-129 and level 130+ ")
                           .AddText(" Only teams can participate in this Tournament . You can either create a team")
                           .AddText(" befor signin up, or enter the waiting area and create you team. At the start of the")
                           .AddText(" Tournament, any player who`s not in team will be removed from the tournament map.")
                           .AddText("  Where the Tournament ends, only the team leaders of the winning teams can claim the reward.")
                           .AddOption("Ok Thank you!", 255)
                      .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }

                case 3:
                    {
                        if (client.Team == null)
                        {
                            data.AddText("You need to team up with others to take part in it. Please create team.")
                  .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            break;
                        }
                        if (!MsgTournaments.MsgSchedules.TeamPkTournament.SignUp(client))
                        {
                            data.AddText("Please sign up between 18:45 and 19:06 every Saturday for the Team PK Tournament.")
                            .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }



                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!MsgTournaments.MsgSchedules.TeamPkTournament.GetReward(client, stream))
                            {
                                data.AddText("Sorry, you are not in top 8 in Team Pk Tournament or you claimed the reward.")
                        .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make one space in your bag.")
                          .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ElitePk)]
        public static void ElitePk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the Elite PK Tournament, have you? No? Aha, please allow me to")
                            .AddText(" introduce this exciting event for you! It`s a new PK Tournament that takes place every Friday. You will be divided into four groups, based")
                            .AddText("~on your level. Not only will the top 3 winners of each group receive nice prizes, you will also receive EXP worth 1 EXP Ball and 50k Honor points for each match you take part in at the end of that round. Do you want to have a go?")
                            .AddOption("Tell~me~more~about~the~rules.", 1)
                            .AddOption("Sign~me~up!", 3)
                            .AddOption("I~want~to~claim~my~prize!", 5)
                       .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("The Elite PK Tournament begins at 20:00 every Friday night, and players will be divided into four groups, which are the Under 100 group, the 100-119 group, the 120-129 group, and the 130+ group. You can sign up for it")
                                                 .AddText("~between 19:55 and 19:59. No admittance after the tournament begins. Players should first wait in the Waiting Area, and if you leave the area before your round, you will forfeit your chance. There will be a Knockout round before the Final.")
                                                 .AddText("~But if there are less than 8 players at the start of the Elite PK Tournament, the Final will be replaced by the Knockout. Each round in the Knockout lasts 3 minutes. At the end of 3 minutes, if no one is killed, the player with a higher score wins.")
                                                 .AddText("~After you finish your round, you`ll have to wait for all the other players to finish this round to enter the next round.")
                                                 .AddOption("There~is~a~Final?", 2).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure. When there are less than 8 players left in the field, the Final starts. Each round of the Final lasts for 5 minutes. Though all of the Top 8 players will receive nice prizes, the top 3 will earn special prizes! Claim your reward before the next")
                                .AddText("~week`s Elite PK Tournament begins, or you won`t get them. We hope everyone will join this tournament, and so not only will the top 3 winners of each group receive nice prizes, you will also receive EXP worth 1 EXP Ball and 1000 Honor")
                                .AddText("~points for each match you take part in at the end of that match!")
                                .AddOption("Sign~me~up!", 3).AddOption("Not~interested", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (!MsgTournaments.MsgSchedules.ElitePkTournament.SignUp(client))
                        {
                            data.AddText("Please sign up between 19:55 and 19:59 every Friday night for the Elite PK Tournament.")
                            .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!MsgTournaments.MsgSchedules.ElitePkTournament.GetReward(client, stream))
                            {
                                data.AddText("Sorry, you are not in top 8 in elite pk or you claimed the reward.")
                        .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your bag.")
                          .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ClassPkWar)]
        public static void ClassPkWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello , you're the last player alive?")
                            .AddOption("Yes", 1)
                            .AddOption("Teleport~to~Twin~City.", 2)
                       .AddOption("Ah. wait", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        MsgTournaments.MsgClassPKWar.War war;
                        if (Game.MsgTournaments.MsgSchedules.ClassPkWar.GetWar(client, out war) != MsgTournaments.ProcesType.Dead)
                        {
                            if (!war.CheckJoin())
                            {
                                if (war.IsFinish(client))
                                {
                                    if (!client.Inventory.HaveSpace(4))
                                    {
                                        data.AddText("Please make 4 more spaces in your inventory.")
                                            .AddOption("Let me check.", 255).AddAvatar(154).FinalizeDialog();

                                    }
                                    war.GetMyReward(client, stream);

                                }
                                else
                                {
                                    data.AddText("Sorry, the war is not finished, or other players are still alive.")
                   .AddOption("Ah ok", 255).AddAvatar(154).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry, the war is not finished")
                    .AddOption("Ah ok", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, the war is finished, or other players are still alive.")
           .AddOption("Ah ok", 255).AddAvatar(154).FinalizeDialog();
                        }

                        break;

                    }
                case 2:
                    {
                        data.AddText("You wana to teleport back to TwinCity? You are sure?")
           .AddOption("Yes", 3).AddOption("Ah no.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(302, 169, 1002, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ClassPkEnvoy)]
        public static void ClassPkEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Everyone settle down! I need to announce that there are new rules for the Class PK War! It is held every Monday: sign up between 19:15 and 19:29, the war starts at 19:30.")
                            .AddOption("Tell~me~more.", 1)
                       .AddOption("Sign~up.", 2)
                       .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 1:
                    {






                        break;
                    }
                case 2:
                    {
                        MsgTournaments.MsgClassPKWar.War war;
                        if (Game.MsgTournaments.MsgSchedules.ClassPkWar.GetWar(client, out war) != MsgTournaments.ProcesType.Dead)
                        {
                            if (war.CheckJoin())
                            {
                                client.Teleport(81, 62, Game.MsgTournaments.MsgClassPKWar.MapID, war.DinamicID);
                            }
                            else
                            {
                                data.AddText("I`m sorry, but the Class PK War is only held on Mondays.")
                  .AddOption("Alright.", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I`m sorry, but the Class PK War is only held on Mondays.")
                     .AddOption("Alright.", 255).AddAvatar(154).FinalizeDialog();
                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.NemesysConductort)]
        public static void NemesysConductort(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("With only 100 silver, I can teleport you to somewhere far away. Please choose your destination.")
                           .AddOption("Desert~City.", 1)
                      .AddOption("Market.", 2)
                      .AddOption("Just~passing~by.", 255).AddAvatar(1).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Teleport(502, 646, 1000);
                        }
                        else
                        {
                            data.AddText("You do not have 100 Silvers with you.")
   .AddOption("I~see", 255).AddAvatar(1).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Teleport(200, 204, 1036);
                        }
                        else
                        {
                            data.AddText("You do not have 100 Silvers with you.")
   .AddOption("I~see", 255).AddAvatar(1).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        public static void SpaceMarks(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {

                            data.AddText("I`m sell WindSpell for 1.000 Silver. Use that to teleport back to me")
                         .AddOption("Ok give me.", 1).AddOption("No thanks.", 255).AddAvatar(3).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Only Water class can buy this teleport scroll.")
                           .AddOption("I see.", 255).AddAvatar(3).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            if (client.Player.Money >= 1000)
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    client.Player.Money -= 1000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    switch (id)
                                    {
                                        case 10031: client.Inventory.Add(stream, 1060025); break;
                                        case 10032: client.Inventory.Add(stream, 1060026); break;
                                        case 10033: client.Inventory.Add(stream, 1060027); break;
                                        case 10034: client.Inventory.Add(stream, 1060028); break;
                                        case 10035: client.Inventory.Add(stream, 1060029); break;
                                        case 10036: client.Inventory.Add(stream, 1060038); break;
                                        case 10037: client.Inventory.Add(stream, 1060031); break;
                                        case 10038: client.Inventory.Add(stream, 1060032); break;
                                        case 10039: client.Inventory.Add(stream, 1060033); break;
                                        case 10040: client.Inventory.Add(stream, 1060034); break;
                                        case 10041: client.Inventory.Add(stream, 1060035); break;
                                        case 10043: client.Inventory.Add(stream, 1060037); break;

                                    }
                                }
                                else
                                {
                                    data.AddText("Please make space in you inventory")
          .AddOption("Ah~thank~you!.", 255)
          .AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You do not have 1000 Silvers with you.")
            .AddOption("I~see", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JailCPAdmin)]
        public static void JailCPAdmin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Please wait , until the system will check the donation !")
                    .AddOption("Ok, i`m waiting.", 1)
                    .AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                 .AddOption("Let me check.", 255)
                 .AddAvatar(7).FinalizeDialog();
                            break;
                        }


                        break;
                    }

            }
           
        }
        [NpcAttribute(NpcID.WardenZhang)]
        public static void WardenZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Were always in the need of fresh bounty hunters.You see... murderers and thieves can evade out guards.")
                            .AddText("We need heroes to slay he killers and restore peace. If you can PK those whose names are black (known criminals),")
                            .AddText("you might get rewarded. If they pay")
                            .AddOption("I~want~to~claim~my~reward.", 1)
                       .AddOption("None~of~my~business.", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        Database.TheCrimeTable MyCrimes;
                        if (Database.Server.TheCrimePoll.TryGetValue(client.Player.UID, out MyCrimes))
                        {
                            string KillerName = "";
                            int ConquerPoints = MyCrimes.Claim(out KillerName);
                            if (ConquerPoints > 0)
                            {
                                data.AddText("For arrest " + KillerName + ", you will be awarded " + ConquerPoints + " ConquerPoints.")
                   .AddOption("Ok, I see.", 255).AddAvatar(37).FinalizeDialog();
                                client.Player.ConquerPoints += (uint)ConquerPoints;

                                break;
                            }
                        }

                        data.AddText("Good, good you have slain a murderer. Yes.. but only if he pays bail can you claim the bounty.")
                      .AddText("Its the way things are done! Heres a bit of advice, hehehe... Kill the rich, forget the poor. Ha ha Harrr!")
                      .AddText("Thats called makeing  a living!")
                 .AddOption("Despicable!~But~realistic!", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JailWarden)]
        public static void JailWarden(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Lucky~for~you,~the~player~who~killed~you~is~willing~to~drop~the~murder~charges~if~you~pay~him~according~to~what~you~did.")
                            .AddText("~So~if~you~pay~him~properly~he~is~willing~to~drop~the~murder~charges.")
                            .AddOption("Ok!~Let~me~get~out~from~here.", 1)
                            .AddOption("Any~chance~for~discount?", 2)
                       .AddOption("I~would~rather~rot~in~hell!", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("The~red~name,~whose~PK~points~are~30-99,~can~leave~here~without~any~price.~The~black~name,~whose~PK~points~are~100~")
                            .AddText("or~above,~should~pay~gold~as~recognizance~to~leave~here.~Or~pay~in~CPs~if~they~really~killed~a~lot.")
                            .AddOption("Ok,~I~will~pay.", 1)
                       .AddOption("I~would~rather~stay~here.", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.PKPoints < 100)
                        {
                            data.AddText("You~don\'t~need~to~stay~here~any~more.~Let~me~help~you~out")
                       .AddOption("Ok,~thanks.", 3)
                  .AddOption("I~wanna~stay~here.", 255).AddAvatar(37).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            int cost = (int)(5000 + client.Player.PKPoints * 10);
                            data.AddText("You~really~murdered~a~lot.~Well~in~this~case~if~you~pay~for~yours~sins~" + cost.ToString() + "~CPs~I~will~let~you~go.")
         .AddOption("Here~is~my~payment.", 4)
    .AddOption("No,~thanks.", 255).AddAvatar(37).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        uint cost = (uint)(5000 + client.Player.PKPoints * 10);
                        if (client.Player.ConquerPoints >= cost)
                        {
                            client.Player.ConquerPoints -= cost;


                            if (client.Player.MyKillerUID > 1000000)
                            {
                                Database.TheCrimeTable crime;
                                if (Database.Server.TheCrimePoll.TryGetValue(client.Player.MyKillerUID, out crime))
                                {
                                    crime.AddCrime(client.Player.Name, client.Player.UID, (int)cost);
                                }
                                else
                                {
                                    crime = new Database.TheCrimeTable();
                                    crime.AddCrime(client.Player.Name, client.Player.UID, (int)cost);
                                    Database.Server.TheCrimePoll.TryAdd(client.Player.MyKillerUID, crime);
                                }
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulation " + client.Player.Name + " rewarded " + client.Player.MyKillerName + " with " + cost.ToString() + " ConquerPoints compensation for his crimes!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            ushort[] Maps = new ushort[] { 1002, 1000, 1011, 1015 };

                            byte Rand = (byte)Program.GetRandom.Next(0, Maps.Length);
                            switch (Rand)
                            {
                                case 0: client.Teleport(386, 662, 1011); break;
                                case 1: client.Teleport(294, 505, 1000); break;
                                case 2: client.Teleport(617, 334, 1002); break;
                                default: client.Teleport(303, 379, 1015); break;
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                             .AddOption("I~see", 255).AddAvatar(37).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        client.Teleport(385, 255, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JailJoin)]
        public static void JailJoin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("What~can~I~do~for~you?")
                            .AddOption("Visit~the~jail.", 1)
                       .AddOption("Just~passing~by.", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Give~me~1000~silver,~I~will~teleport~you~there.")
                                 .AddOption("Here~are~1000~silver.", 2)
                            .AddOption("If~so,~I~will~stay~here.", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.PKPoints < 30)
                        {
                            if (client.Player.Money > 1000)
                            {
                                client.Player.Money -= 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Teleport(29, 72, 6000);

                            }
                            else
                            {
                                data.AddText("You do not have 1000 silvers with you.")
                               .AddOption("I~see", 255).AddAvatar(37).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I'm sorry you have more Pkpoints")
                             .AddOption("I~see", 255).AddAvatar(37).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
    }
}
