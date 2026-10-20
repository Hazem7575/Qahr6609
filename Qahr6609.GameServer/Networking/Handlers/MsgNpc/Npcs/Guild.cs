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
        //FrozenGrottoGuardian5
        [NpcAttribute(NpcID.FrozenGrottoGuardian5)]
        public static void FrozenGrottoGuardian5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText(" Who`s there?.. Oh, you are from Twin City. aren`t you? Finally, i thoyght no one would come here to rescue us..")
                            .AddText("We`ve still got some business to carry out on the 6th floor. You`re here to go there and clear out the monsters,")
                            .AddText("right? I can also help you get the other floors, if you like.")
                    .AddOption("How can i go FG6? ", 1)
                    .AddOption("Gimme a Shadow Jave", 2)
                    .AddOption("Send me to FG4 then.", 3)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(31).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("There is a portal on this floor (150,178). You can get to F6 from here. Be careful.")
                            .AddOption("Thanks. I will.", 255)
                            .AddAvatar(31).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Contain(720599, 1))
                            {
                                data.AddText("Sorry, but your have ShadowJade.")
                                 .AddOption("Ah~yes.", 255)
                               .AddAvatar(0).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 720599);
                        }
                        else
                        {
                            data.AddText("Please make 1 space in your inventory.")
                                  .AddOption("Ah~yes.", 255)
                                .AddAvatar(0).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        client.Teleport(379, 470, 2054);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FrozenFroto4Teleporter)]
        public static void FrozenFroto4Teleporter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText(" ~I guess you`ve heard about the 5th floor of Frozen Grotto before. The place is infested with so many ferocious monsters that no one dares go up there. Well, until recently,")
                            .AddText(" that is... Yeah, you heard me... I found out that the monsters on the 3rd and 4th")
                            .AddText("~are carrying a Grotto Key with them. With this key, it would be easier to kill the monsters on the 5th floor. If you are interested, go kill those monsters")
                            .AddText(" on the 3rd and 4th floor and get a key, then I`ll send you to the 5th floor to get rid of")
                            .AddText("~those nasty beasts. Hmmm,~I`m~on~it.")
                            .AddText("If your VIP level has reached 4, you can enter the 5th floor of Frozen Grotto everyday, for free!")
                    .AddOption("Hey,~I`ve~got~a~key~here.", 1)
                    .AddOption("Frozen~Grotto~F5.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(31).FinalizeDialog();


                        break;
                    }
                case 1:
                    {//721347 GrottoKey
                        if (client.Inventory.Contain(721347, 1))
                        {
                            if (!client.Inventory.Remove(721347, 1, stream)) break;
                            client.Teleport(526, 562, 2055);
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the Grotto Key in your inventory. Go to kill monsters to obtain that key..")
                    .AddOption("Okay.", 255)
                    .AddAvatar(31).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Player.VipLevel >= 4)
                        {
                            client.Teleport(526, 562, 2055);
                        }
                        else
                        {
                            data.AddText("Sorry, but your VIP Level has to reach 4 to enter the 5th floor for free. If you want to go there, you must collect the Grotto Key.")
                 .AddOption("I~see.", 255)
                 .AddAvatar(31).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DesertFrozenGroto)]
        public static void DesertFrozenGroto(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The Frozen Grotto is a dangerous and ancient place. Many heroes have died there while searching for its hidden treasures.")
                            .AddText("~Six layers of the Frozen Grotto have been excavated so far. If you have made up your mind to test your ability there, I can")
                            .AddText("~teleport you to the first 2 floors. If you want to go to the 5th floor, speak to the Guardian on Frozen Grotto F4 (387,460).")
                    .AddOption("Frozen~Grotto~1.", 1)
                    .AddOption("Frozen~Grotto~2.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(183).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.Teleport(338, 354, 1762);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(392, 625, 1927);
                        break;
                    }
            }
        }
        public static void Furnitures(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Database.NpcServer.Furniture Furniture;
            if (Database.NpcServer.FurnitureInformations.TryGetValue(id, out Furniture))
            {
                Dialog data = new Dialog(client, stream);
                switch (Option)
                {
                    case 0:
                        {
                            data.AddText("Do you want this " + Furniture.Name + " ? It costs only " + Furniture.MoneyCost + " silvers.")
                                                     .AddOption("Yes.", 1)
                                                       .AddOption("No.", 255)
                                                     .AddAvatar(0).FinalizeDialog();


                            break;
                        }
                    case 1:
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= Furniture.MoneyCost)
                                {
                                    client.Player.Money -= Furniture.MoneyCost;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, Furniture.ItemID, 1);

                                    Database.ItemType.DBItem item;
                                    if (Database.Server.ItemsBase.TryGetValue(Furniture.ItemID, out item))
                                    {
                                        client.SendSysMesage("You got a " + item.Name + "!", MsgMessage.ChatMode.System);
                                    }
                                }
                                else
                                {

                                    data.AddText("You do not have " + Furniture.MoneyCost + " silvers with you.")
                                     .AddOption("I~see.", 255)
                                   .AddAvatar(0).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("There is not enough room in your inventory.")
                                    .AddOption("Ah~yes.", 255)
                                  .AddAvatar(0).FinalizeDialog();
                            }


                            break;
                        }
                }
            }
        }
        public static void LotteryBoxes(Client.GameClient user, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(user, stream);
            switch (Option)
            {

                case 0:
                    {
                        goto case 1;

                    }
                case 1:
                    {

                        // each box has its own prizes: the client names the box by the npc look (npc.ini type = mesh / 10)
                        Game.MsgNpc.Npc box;
                        uint lookType = user.Map.SearchNpcInScreen(id, user.Player.X, user.Player.Y, out box) && box != null ? (uint)(box.Mesh / 10) : 0;
                        string error;
                        if (MsgServer.MsgLottery.TryDraw(user, stream, Database.LotteryTable.BoxForNpc(lookType, id), out error))
                        {
                            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                            packet.ID = MsgStringPacket.StringID.Effect;
                            packet.UID = id;
                            packet.Strings = new string[1] { "lottery" };
                            user.Player.View.SendView(stream.StringPacketCreate(packet), true);
                        }
                        else
                        {
                            data.AddText(error)
                               .AddOption("Oops.", 255)
                               .AddAvatar(0).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        public static void Warehause(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.ActiveNpc == (uint)NpcID.WHTwin)
            {
                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Barber, client.Player.Class);
                var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);

                var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1006);
                Dialog data = new Dialog(client, stream);
                switch (Option)
                {

                    case 0:
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You can deposit or withdraw your items and money, here. Wait, are you here to claim the Military Letter for the TC Vice Captain?")
                                 .AddOption("Yeah!", 10)
                                 .AddOption("Open~my~Warehouse.", 1)
                                 .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                            {
                                data.AddText("I am the most reliable man in Twin City! Your valuable things will be safe in my warehouse!")
                               .AddOption("You~look~reliable.", 2)
                               .AddOption("Open~my~Warehouse.", 1)
                               .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                            {
                                data.AddText("You really need a good weapon during your long journey! The Blacksmith has many good stuffs! He should have something for you.")
                .AddOption("I~will~speak~with~him~now!", 3)
                  .AddOption("Open~my~Warehouse.", 1)
                .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                goto case 1;
                            }
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("You really need a good weapon during your long journey! The Blacksmith has many good stuffs! He should have something for you.")
                  .AddOption("I~will~speak~with~him~now!", 3)
                    .AddOption("Open~my~Warehouse.", 1)
                  .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            goto case 1;


                        }
                    case 10:
                        {
                            if (client.Inventory.HaveSpace(1) == false)
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                               .AddOption("let me to check.", 255)
                               .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            if (client.Inventory.Contain(711235, 1))
                            {
                                data.AddText("You`ve got the Military Letter. Go give it to the TC Vice Captain!")
                                   .AddOption("I`ll~go~now.", 11)
                                   .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 711235);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1, 1);

                            data.AddText("This is the letter you want. Take it. By the way, don`t forget about our wonderful storage services!")
                                .AddOption("Okay,~thanks.", 11)
                                .AddAvatar(10).FinalizeDialog();



                            break;
                        }
                    case 11:
                        {
                            client.Player.QuestGUI.SendAutoPatcher(1002, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                            break;
                        }
                    case 1:
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.Warehouse,
                                wParam1 = client.Player.X,
                                wParam2 = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(&action));

                            break;
                        }
                    case 2:
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.IncreaseExperience(stream, 1400, Role.Flags.ExperienceEffect.angelwing);
                                data.AddText("You really need a good weapon during your long journey! The Blacksmith has many good stuffs! He should have something for you.")
                  .AddOption("I~will~speak~with~him~now!", 3)
                    .AddOption("Open~my~Warehouse.", 1)
                  .AddAvatar(10).FinalizeDialog();
                            }


                            break;
                        }
                    case 3:
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.QuestGUI.SendAutoPatcher("The~Warehouseman~asked~you~to~pay~a~visit~to~the~Blacksmith.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);



                            //client.Player.QuestGUI.Accept(818, 0);
                            //  client.Player.QuestGUI.SendAutoPatcher("The~Warehouseman~asked~you~to~pay~a~visit~to~the~Blacksmith.", 1002, 324, 230, (uint)NpcID.BlackSmith);

                            break;
                        }
                }
            }
            else
            {
                ActionQuery action = new ActionQuery()
                {
                    ObjId = client.Player.UID,
                    Type = ActionType.OpenDialog,
                    dwParam = MsgServer.DialogCommands.Warehouse,
                    wParam1 = client.Player.X,
                    wParam2 = client.Player.Y
                };
                client.Send(stream.ActionCreate(&action));


            }
        }
        public static void GuildCondTeleBack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("In exchange of a fee of 500gold, I will teleport you back to the guild war map. Don't bother me if you are poor.")
                        .AddOption("Teleport me.", 1)

                        .AddOption("I'm too poor.", 255).AddAvatar(3).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money >= 500)
                        {
                            client.Player.Money -= 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Teleport(348, 339, 1038);
                        }
                        else
                        {
                            data.AddText("You do not have 500 silvers with you.")
                                 .AddOption("I~see.", 255)
                               .AddAvatar(3).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        public static void GuildConductorsProces(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Are~you~heading~for~the~next~teleporter?~It~is~free~for~our~members,~and~1,000~silver~for~others.")
                        .AddOption("Please~teleport~me~there.", 1)
                        .AddOption("Buy~Guild~Teleporter.", 2)
                        .AddOption("Just~passing~by.", 255).AddAvatar(3).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        // "It is free for our members, and 1,000 silver for others." (members = the guild that holds the Guild Area)
                        bool member = client.Player.MyGuild != null && client.Player.GuildID != 0
                            && client.Player.GuildID == Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID;
                        if (member || client.Player.Money >= 1000)
                        {
                            if (!member)
                            {
                                client.Player.Money -= 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            }

                            ushort x, y;
                            uint map;
                            Game.MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.ActiveNpc - 1].GetCoords(out x, out y, out map);

                            client.Teleport(x, y, map);
                        }
                        else
                        {
                            data.AddText("Please~take~1000~silvers~to~use~guild~teleporter.")
                        .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("A~Teleporter~costs~100,000~silver.~After~you~buy~one~and~place~it~at~a~good~location,~you~can~be~teleported~there~directly.")
                     .AddOption("Buy~Teleporter~1.", 3)
                     .AddOption("Buy~Teleporter~2.", 4)
                              .AddOption("Buy~Teleporter~3.", 5)
                     .AddOption("Buy~Teleporter~4.", 6)
                     .AddOption("I~changed~my~mind.", 255).AddAvatar(3).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID
                                                   && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= 100000)
                                {
                                    client.Player.Money -= 100000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720021);
                                }
                                else
                                {

                                    data.AddText("You do not have 100.000 silvers with you.")
                                  .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Please make one more space in your inventory.")
                              .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~buy~a~Guild~Teleporter.")
                          .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }



                        break;
                    }
                case 6:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID
                            && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= 100000)
                                {
                                    client.Player.Money -= 100000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720024);
                                }
                                else
                                {

                                    data.AddText("You do not have 100.000 silvers with you.")
                                  .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Please make one more space in your inventory.")
                              .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~buy~a~Guild~Teleporter.")
                          .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }



                        break;
                    }
                case 5:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID
                           && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= 100000)
                                {
                                    client.Player.Money -= 100000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720023);
                                }
                                else
                                {

                                    data.AddText("You do not have 100.000 silvers with you.")
                                  .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Please make one more space in your inventory.")
                              .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~buy~a~Guild~Teleporter.")
                          .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }



                        break;
                    }
                case 4:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID
                            && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= 100000)
                                {
                                    client.Player.Money -= 100000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720022);
                                }
                                else
                                {

                                    data.AddText("You do not have 100.000 silvers with you.")
                                  .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Please make one more space in your inventory.")
                              .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~buy~a~Guild~Teleporter.")
                          .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }



                        break;
                    }
            }
        }
        //GuildWars
        [NpcAttribute(NpcID.GuildOfficer)]
        public static void GuildOfficer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            /*if (DateTime.Now.DayOfWeek != DayOfWeek.Friday) //working
                return;*/
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Alive)
                        {
                            data.AddText("Guild~War~is~being~held~now.~When~it~is~over,~the~leader~and~the~deputy~leaders~of~the~guild~that~knocked~down")
                                   .AddText("~the~pole~can~claim~the~Guild~War~prize~here,~once~per~week.")
                               .AddOption("Ok,~I~see.", 255).AddAvatar(110).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The~guild~that~won~the~Guild~War~gets~a~prize~every~week:~" + ServerRate.Guildwar.ToString() + "~CPs~for~the~guild~leader,~a~prize~for~each~deputy~leader.")
                                  .AddOption("I~would~like~to~claim~it", 1)
                             .AddOption("Just~passing~by.", 255).AddAvatar(110).FinalizeDialog();
                        }
                      

                        break;
                    }
                case 1:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID && client.Player.MyGuild != null && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {

                            if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader && MsgTournaments.MsgSchedules.GuildWar.Winner.LeaderReward > 0
                                 && !MsgTournaments.MsgSchedules.GuildWar.RewardLeader.Contains(client.Player.UID))
                            {
                                if (!client.Inventory.HaveSpace(2))
                                {
                                    data.AddText("Please make 2 more spaces in your inventory.")
                  .AddOption("let me check.", 255).AddAvatar(110).FinalizeDialog();
                                    break;
                                }

                                // only the leader himself (the spouse UID was added too and gave the spouse the leader halo at login)
                                MsgTournaments.MsgSchedules.GuildWar.RewardLeader.Add(client.Player.UID);
                                MsgTournaments.MsgSchedules.GuildWar.Winner.LeaderReward -= 1;
                                client.Player.ConquerPoints += (uint)ServerRate.Guildwar;

                                client.Player.AddSpecialTitle(MsgTitleStorage.TitleType.Conqueror, stream);
                                client.SendSysMesage("You received a title 'Conqueror'. Check your WardRobe.");

                                client.Inventory.Add(stream, 723467, 1);

                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopGuildLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                Database.Server.Save(new Action(MsgTournaments.MsgSchedules.GuildWar.Save));
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("" + client.Player.Name + ", leader of " + client.Player.MyGuild.GuildName + ", claimed the Guild War prize: " + ServerRate.Guildwar.ToString() + " CPs and a LordToken.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                            }
                            else if (client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader && MsgTournaments.MsgSchedules.GuildWar.Winner.DeputiLeaderReward > 0
                                && !MsgTournaments.MsgSchedules.GuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                            {
                                if (!client.Inventory.HaveSpace(4))
                                {
                                    data.AddText("Please make 4 more spaces in your inventory.")
               .AddOption("Let me check.", 255).AddAvatar(110).FinalizeDialog();
                                    break;
                                }
                                client.Inventory.Add(stream, 3003235, 2);

                                MsgTournaments.MsgSchedules.GuildWar.Winner.DeputiLeaderReward -= 1;
                                MsgTournaments.MsgSchedules.GuildWar.RewardDeputiLeader.Add(client.Player.UID);
                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopDeputyLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                Database.Server.Save(new Action(MsgTournaments.MsgSchedules.GuildWar.Save));
                            }
                            else
                            {
                                data.AddText("Only~the~guild~leader~and~the~deputy~leaders~can~claim~the~prize,~once~per~week.")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your~guild~has~not~dominated~guild~area.~Guild~War~is~held~every~Saturday~from~20:00~to~23:00.")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LeftGate)]
        public static void LeftGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                       || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID) == false)
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.Player.LeftDoorStamp = DateTime.Now.AddMilliseconds(3000);
                        data.AddText("Select the option you want to pursue.");
                        if (DateTime.Now > Game.MsgTournaments.MsgSchedules.GuildWar.ClosableLeftGateStamp)
                        {
                            data.AddOption("Open~the~gate", 1)
                            .AddOption("Close gate.", 2);
                        }
                        data.AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID)
                        {
                            client.TerainMask = 0;
                            client.Teleport(162, 192, 1038);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if (DateTime.Now <= Game.MsgTournaments.MsgSchedules.GuildWar.ClosableLeftGateStamp)
                        {
                            break;
                        }
                        if (DateTime.Now > client.Player.LeftDoorStamp)
                        {
                            break;
                        }
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                   || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh == Role.SobNpc.StaticMesh.LeftGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].HitPoints == 0)
                                MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].HitPoints = MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].MaxHitPoints;

                            MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh = Role.SobNpc.StaticMesh.LeftGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].UID, 2);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh == Role.SobNpc.StaticMesh.OpenLeftGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh = Role.SobNpc.StaticMesh.OpenLeftGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RightGate)]
        public static void RightGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                       || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID) == false)
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.Player.RightDoorStamp = DateTime.Now.AddMilliseconds(3000);
                        data.AddText("Select the option you want to pursue.");
                        if (DateTime.Now > Game.MsgTournaments.MsgSchedules.GuildWar.ClosableRightGateStamp)
                        {
                            data.AddOption("Open~the~gate", 1)
                            .AddOption("Close gate.", 2);
                        }
                        data.AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID)
                        {
                            client.TerainMask = 0;
                            client.Teleport(203, 176, 1038);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if (DateTime.Now <= Game.MsgTournaments.MsgSchedules.GuildWar.ClosableRightGateStamp)
                        {
                            break;
                        }
                        if (DateTime.Now > client.Player.RightDoorStamp)
                        {
                            break;
                        }
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                           || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh == Role.SobNpc.StaticMesh.RightGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].HitPoints == 0)
                                MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].HitPoints = MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].MaxHitPoints;

                            MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh = Role.SobNpc.StaticMesh.RightGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].UID, 2);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh == Role.SobNpc.StaticMesh.OpenRightGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh = Role.SobNpc.StaticMesh.OpenRightGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.SuperGuildWar_CasteleLeftGate)]
        public static void SuperGuildWar_CasteleLeftGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                        .AddOption("Open~the~gate", 1)
                        .AddOption("Close gate.", 2)
                        .AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(191, 142, 3868);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                   || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleLeftGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.RightGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.RightGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 2);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                           || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleLeftGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenRightGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenRightGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SuperGuildWar_CasteleRightGate)]
        public static void SuperGuildWar_CasteleRightGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                                                .AddOption("Open~the~gate", 1)
                                                .AddOption("Close gate.", 2)
                                                .AddOption("Get inside.", 3)
                                                .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(264, 141, 3868);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                  || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleRightGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.RightGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.RightGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 2);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleRightGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenRightGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenRightGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SuperGuildWar_LeftGate)]
        public static void SuperGuildWar_LeftGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                        .AddOption("Open~the~gate", 1)
                        .AddOption("Close gate.", 2)
                        .AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(166, 224, 3868);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                           || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.LeftGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.LeftGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.LeftGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 2);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.LeftGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenLeftGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenLeftGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SuperGuildWar_MidleGate)]
        public static void SuperGuildWar_MidleGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                       .AddOption("Open~the~gate", 1)
                       .AddOption("Close gate.", 2)
                       .AddOption("Get inside.", 3)
                       .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(221, 202, 3868);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                   || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CenterGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.LeftGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.LeftGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 2);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CenterGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenLeftGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenLeftGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SuperGuildWar_RightGate)]
        public static void SuperGuildWar_RightGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                        .AddOption("Open~the~gate", 1)
                        .AddOption("Close gate.", 2)
                        .AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(287, 223, 3868);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                   || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.RightGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.LeftGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.LeftGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 2);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank == Role.Flags.GuildMemberRank.HDeputyLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.RightGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenLeftGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenLeftGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Database.Server.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
    }
}
