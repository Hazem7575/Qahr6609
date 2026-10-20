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

        public static string RechargeOptionInfo(Client.GameClient user, Role.Player.RechargeType type, uint Cost)
        {
            if (user.Player.ContainRechargeType(type))
                return "(claimed).";

            return "(" + Cost + "Points).";
        }


        [NpcAttribute(NpcID.VotePointsReward)]
        public static void VotePointsReward(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Welcome to VoteManger seems like you are eligible for vote please do remember that you can vote only once at 6 hours.\n")
                         .AddOption("Vote.", 41)
                         .AddOption("Exchange Points.", 60)
                         .AddOption("I`ll think about it.", 255)
                         .AddAvatar(3)
                         .FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (Database.VoteSystem.CanVote(client))
                        {
                            client.SendSysMesage("-----" + client.Player.UID + "", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                            client.Player.StartVote = true;
                            client.Player.StartVoteStamp = Extensions.Time32.Now.AddSeconds(30);
                            client.SendSysMesage("Please wait for system to check your vote");
                        }
                        else
                        {
                            data.AddText("You have already claimed your reward for voting. You may only vote once every 12 hour per IP address.")
                             .AddOption("I~see.", 255)
                             .AddAvatar(63).FinalizeDialog();
                        }
                        break;

                    }
                #region vote reward
                case 60:
                    {
                        data.AddText("Dear " + client.Player.Name + " You currently have [" + client.Player.VotePoints + "] Vote Point. Would you like to trade them for some random prizes.")
                        .AddOption("DragonBall (4 VPs)", 11)
                        .AddOption("VIP(1-day) (4 VPs)", 12)
                        .AddOption("PowerExpBall (12 VPs)", 13)
                            //.AddOption("SilverPrize (60 VPs)", 14)
                        .AddOption("Leave", 255)
                        .AddAvatar(10).FinalizeDialog();
                        break;
                    }
                #region DragonBall
                case 11:
                    {
                        if (client.Player.VotePoints >= 4)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.VotePoints -= 4;
                                client.Inventory.Add(stream, 1088000);//DragonBall
                            }
                            else client.SendSysMesage("You don`t have enough space in your inventory.");
                        }
                        else client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region VIP(1-day)
                case 12:
                    {
                        if (client.Player.VotePoints > 4)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.VotePoints -= 4;
                                client.Inventory.Add(stream, 780001);
                            }
                            else client.SendSysMesage("You don`t have enough space in your inventory.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough VotePoints.")
                                 .AddOption("Ah ok.", 255)
                                 .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region PowerExpBall
                case 13:
                    {
                        if (client.Player.VotePoints >= 12)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.VotePoints -= 12;
                                client.Inventory.Add(stream, 722057);//PowerExpBall
                            }
                            else client.SendSysMesage("You don`t have enough space in your inventory.");
                        }
                        else client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region SilverPrize
                case 14:
                    {
                        if (client.Player.VotePoints >= 60)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.VotePoints -= 60;
                                client.Inventory.Add(stream, 2100065);//SilverPrize
                            }
                            else client.SendSysMesage("You don`t have enough space in your inventory.");
                        }
                        else client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion





                #endregion
            }
        }

     


        [NpcAttribute(NpcID.AccesoryPacket2)]
        public static void AccesoryPacket2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        data.AddText("Select the Accesory you need.");
                        byte page = (byte)(Option + 1);
                        int tocount = (int)Math.Min(Database.ItemType.TopAccesorys.Count, (page * 10));
                        for (int x = page - 1; x < tocount; x++)
                        {
                            var garment = Database.ItemType.TopAccesorys[x];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Accessorys.TryGetValue(garment, out DBInfo))
                            {
                                data.AddOption("" + DBInfo.Name + "", (byte)(100 + x));
                            }

                        }
                        data.AddOption("Next Page", page)
                    .AddAvatar(7)
                    .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option < 100)
                        {
                            data.AddText("Select the Accessory you need.");
                            byte page = (byte)(Option + 1);
                            int tocount = (int)Math.Min(Database.ItemType.TopAccesorys.Count, (page * 10));
                            for (int x = (page - 1) * 10; x < tocount; x++)
                            {
                                var garment = Database.ItemType.TopAccesorys[x];
                                Database.ItemType.DBItem DBInfo;
                                if (Database.ItemType.Accessorys.TryGetValue(garment, out DBInfo))
                                {
                                    data.AddOption("" + DBInfo.Name + "", (byte)(100 + x));
                                }
                            }
                            if (tocount % 10 == 0)
                                data.AddOption("Next Page", page);
                            data.AddAvatar(7)
                            .FinalizeDialog();
                        }
                        else //get garment.
                        {
                            client.Player.BuyItemS = Database.ItemType.TopAccesorys[Option - 100];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Accessorys.TryGetValue(client.Player.BuyItemS, out DBInfo))
                            {
                                data.AddText("Are you sure you wanna buy this Accessory (" + DBInfo.Name + ")?!")
                          .AddOption("Sure.", 250)
                          .AddOption("Show me the Accessory.", 251)
                          .AddOption("Let me check.", 255).AddAvatar(7)
                          .FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 250:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        if (client.Player.BuyItemS != 0 && Database.ItemType.TopAccesorys.Contains(client.Player.BuyItemS) && client.Player.ContainRechargeType(Role.Player.RechargeType.OneRareAccesory_2) == false)
                        {
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.OneRareAccesory_2);

                            client.Inventory.Add(stream, client.Player.BuyItemS);
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Accessorys.TryGetValue(client.Player.BuyItemS, out DBInfo))
                                client.CreateBoxDialog("You received 1 " + DBInfo.Name + "");
                            client.Player.BuyItemS = 0;

                        }
                        break;
                    }
                case 251:
                    {

                        Database.ItemType.DBItem DBInfo;
                        if (Database.ItemType.Accessorys.TryGetValue(client.Player.BuyItemS, out DBInfo))
                        {
                            ShowChatItems.ShowFakeItem(client, stream, client.Player.BuyItemS, DBInfo.Durability);

                            data.AddText("Are you sure you wanna buy this Accessory (" + DBInfo.Name + ")?!")
                                .AddOption("Sure.", 250)
                                .AddOption("Show me the Accessory.", 251)
                                .AddOption("Let me check.", 255).AddAvatar(7)
                                .FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MountPacket3)]
        public static void MountPacket3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        data.AddText("Select the Mount you need.");
                        byte page = (byte)(Option + 1);
                        int tocount = (int)Math.Min(Database.CoatStorage.MountsBig.Count, (page * 10));
                        for (int x = page - 1; x < tocount; x++)
                        {
                            var garment = Database.CoatStorage.MountsBig[x];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.SteedMounts.TryGetValue(garment.ID, out DBInfo))
                            {
                                data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                            }

                        }
                        data.AddOption("Next Page", page)
                    .AddAvatar(7)
                    .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option < 100)
                        {
                            data.AddText("Select the Mount you need.");
                            byte page = (byte)(Option + 1);
                            int tocount = (int)Math.Min(Database.CoatStorage.MountsBig.Count, (page * 10));
                            for (int x = (page - 1) * 10; x < tocount; x++)
                            {
                                var garment = Database.CoatStorage.MountsBig[x];
                                Database.ItemType.DBItem DBInfo;
                                if (Database.ItemType.SteedMounts.TryGetValue(garment.ID, out DBInfo))
                                {
                                    data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                                }
                            }
                            if (tocount % 10 == 0)
                                data.AddOption("Next Page", page);
                            data.AddAvatar(7)
                            .FinalizeDialog();
                        }
                        else //get garment.
                        {
                            client.Player.BuyItemS = Database.CoatStorage.MountsBig[Option - 100].ID;
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.SteedMounts.TryGetValue(client.Player.BuyItemS, out DBInfo))
                            {
                                data.AddText("Are you sure you wanna buy this Mount (" + DBInfo.Name + ")?!")
                          .AddOption("Sure.", 250)
                          .AddOption("Show me the Mount.", 251)
                          .AddOption("Let me check.", 255).AddAvatar(7)
                          .FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 250:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        if (client.Player.BuyItemS != 0 && Database.CoatStorage.MountsBig.Any(p => p.ID == client.Player.BuyItemS) && client.Player.ContainRechargeType(Role.Player.RechargeType.OneMount3) == false)
                        {
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.OneMount3);

                            client.Inventory.Add(stream, client.Player.BuyItemS);
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.SteedMounts.TryGetValue(client.Player.BuyItemS, out DBInfo))
                                client.CreateBoxDialog("You received 1 " + DBInfo.Name + "");
                            client.Player.BuyItemS = 0;

                        }
                        break;
                    }
                case 251:
                    {

                        Database.ItemType.DBItem DBInfo;
                        if (Database.ItemType.SteedMounts.TryGetValue(client.Player.BuyItemS, out DBInfo))
                        {
                            ShowChatItems.ShowFakeItem(client, stream, client.Player.BuyItemS, DBInfo.Durability);

                            data.AddText("Are you sure you wanna buy this Mount (" + DBInfo.Name + ")?!")
                                .AddOption("Sure.", 250)
                                .AddOption("Show me the Mount.", 251)
                                .AddOption("Let me check.", 255).AddAvatar(7)
                                .FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.AccesoryPacket)]
        public static void AccesoryPacket(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        data.AddText("Select the Accesory you need.");
                        byte page = (byte)(Option + 1);
                        int tocount = (int)Math.Min(Database.ItemType.TopAccesorys.Count, (page * 10));
                        for (int x = page - 1; x < tocount; x++)
                        {
                            var garment = Database.ItemType.TopAccesorys[x];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Accessorys.TryGetValue(garment, out DBInfo))
                            {
                                data.AddOption("" + DBInfo.Name + "", (byte)(100 + x));
                            }

                        }
                        data.AddOption("Next Page", page)
                    .AddAvatar(7)
                    .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option < 100)
                        {
                            data.AddText("Select the Accessory you need.");
                            byte page = (byte)(Option + 1);
                            int tocount = (int)Math.Min(Database.ItemType.TopAccesorys.Count, (page * 10));
                            for (int x = (page - 1) * 10; x < tocount; x++)
                            {
                                var garment = Database.ItemType.TopAccesorys[x];
                                Database.ItemType.DBItem DBInfo;
                                if (Database.ItemType.Accessorys.TryGetValue(garment, out DBInfo))
                                {
                                    data.AddOption("" + DBInfo.Name + "", (byte)(100 + x));
                                }
                            }
                            if (tocount % 10 == 0)
                                data.AddOption("Next Page", page);
                            data.AddAvatar(7)
                            .FinalizeDialog();
                        }
                        else //get garment.
                        {
                            client.Player.BuyItemS = Database.ItemType.TopAccesorys[Option - 100];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Accessorys.TryGetValue(client.Player.BuyItemS, out DBInfo))
                            {
                                data.AddText("Are you sure you wanna buy this Accessory (" + DBInfo.Name + ")?!")
                          .AddOption("Sure.", 250)
                          .AddOption("Show me the Accessory.", 251)
                          .AddOption("Let me check.", 255).AddAvatar(7)
                          .FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 250:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        if (client.Player.BuyItemS != 0 && Database.ItemType.TopAccesorys.Contains(client.Player.BuyItemS) && client.Player.ContainRechargeType(Role.Player.RechargeType.OneRareAccesory) == false)
                        {
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.OneRareAccesory);

                            client.Inventory.Add(stream, client.Player.BuyItemS);
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Accessorys.TryGetValue(client.Player.BuyItemS, out DBInfo))
                                client.CreateBoxDialog("You received 1 " + DBInfo.Name + "");
                            client.Player.BuyItemS = 0;

                        }
                        break;
                    }
                case 251:
                    {

                        Database.ItemType.DBItem DBInfo;
                        if (Database.ItemType.Accessorys.TryGetValue(client.Player.BuyItemS, out DBInfo))
                        {
                            ShowChatItems.ShowFakeItem(client, stream, client.Player.BuyItemS, DBInfo.Durability);

                            data.AddText("Are you sure you wanna buy this Accessory (" + DBInfo.Name + ")?!")
                                .AddOption("Sure.", 250)
                                .AddOption("Show me the Accessory.", 251)
                                .AddOption("Let me check.", 255).AddAvatar(7)
                                .FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MountPacket2)]
        public static void MountPacket2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        data.AddText("Select the Mount you need.");
                        byte page = (byte)(Option + 1);
                        int tocount = (int)Math.Min(Database.CoatStorage.MountsBig.Count, (page * 10));
                        for (int x = page - 1; x < tocount; x++)
                        {
                            var garment = Database.CoatStorage.MountsBig[x];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.SteedMounts.TryGetValue(garment.ID, out DBInfo))
                            {
                                data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                            }

                        }
                        data.AddOption("Next Page", page)
                    .AddAvatar(7)
                    .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option < 100)
                        {
                            data.AddText("Select the Mount you need.");
                            byte page = (byte)(Option + 1);
                            int tocount = (int)Math.Min(Database.CoatStorage.MountsBig.Count, (page * 10));
                            for (int x = (page - 1) * 10; x < tocount; x++)
                            {
                                var garment = Database.CoatStorage.MountsBig[x];
                                Database.ItemType.DBItem DBInfo;
                                if (Database.ItemType.SteedMounts.TryGetValue(garment.ID, out DBInfo))
                                {
                                    data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                                }
                            }
                            if (tocount % 10 == 0)
                                data.AddOption("Next Page", page);
                            data.AddAvatar(7)
                            .FinalizeDialog();
                        }
                        else //get garment.
                        {
                            client.Player.BuyItemS = Database.CoatStorage.MountsBig[Option - 100].ID;
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.SteedMounts.TryGetValue(client.Player.BuyItemS, out DBInfo))
                            {
                                data.AddText("Are you sure you wanna buy this Mount (" + DBInfo.Name + ")?!")
                          .AddOption("Sure.", 250)
                          .AddOption("Show me the Mount.", 251)
                          .AddOption("Let me check.", 255).AddAvatar(7)
                          .FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 250:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        if (client.Player.BuyItemS != 0 && Database.CoatStorage.MountsBig.Any(p => p.ID == client.Player.BuyItemS) && client.Player.ContainRechargeType(Role.Player.RechargeType.OneMount2) == false)
                        {
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.OneMount2);

                            client.Inventory.Add(stream, client.Player.BuyItemS);
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.SteedMounts.TryGetValue(client.Player.BuyItemS, out DBInfo))
                                client.CreateBoxDialog("You received 1 " + DBInfo.Name + "");
                            client.Player.BuyItemS = 0;

                        }
                        break;
                    }
                case 251:
                    {

                        Database.ItemType.DBItem DBInfo;
                        if (Database.ItemType.SteedMounts.TryGetValue(client.Player.BuyItemS, out DBInfo))
                        {
                            ShowChatItems.ShowFakeItem(client, stream, client.Player.BuyItemS, DBInfo.Durability);

                            data.AddText("Are you sure you wanna buy this Mount (" + DBInfo.Name + ")?!")
                                .AddOption("Sure.", 250)
                                .AddOption("Show me the Mount.", 251)
                                .AddOption("Let me check.", 255).AddAvatar(7)
                                .FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MountPacket)]
        public static void MountPacket(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        data.AddText("Select the Mount you need.");
                        byte page = (byte)(Option + 1);
                        int tocount = (int)Math.Min(Database.CoatStorage.MountsBig.Count, (page * 10));
                        for (int x = page - 1; x < tocount; x++)
                        {
                            var garment = Database.CoatStorage.MountsBig[x];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.Server.ItemsBase.TryGetValue(garment.ID, out DBInfo))
                            {
                                data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                            }

                        }
                        data.AddOption("Next Page", page)
                    .AddAvatar(7)
                    .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option < 100)
                        {
                            data.AddText("Select the Mount you need.");
                            byte page = (byte)(Option + 1);
                            int tocount = (int)Math.Min(Database.CoatStorage.MountsBig.Count, (page * 10));
                            for (int x = (page - 1) * 10; x < tocount; x++)
                            {
                                var garment = Database.CoatStorage.MountsBig[x];
                                Database.ItemType.DBItem DBInfo;
                                if (Database.Server.ItemsBase.TryGetValue(garment.ID, out DBInfo))
                                {
                                    data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                                }

                            }
                            if (tocount % 10 == 0)
                                data.AddOption("Next Page", page);
                            data.AddAvatar(7)
                            .FinalizeDialog();
                        }
                        else //get garment.
                        {
                            client.Player.BuyItemS = Database.CoatStorage.MountsBig[Option - 100].ID;
                            Database.ItemType.DBItem DBInfo;
                            if (Database.Server.ItemsBase.TryGetValue(client.Player.BuyItemS, out DBInfo))
                            {
                                data.AddText("Are you sure you wanna buy this Mount (" + DBInfo.Name + ")?!")
                          .AddOption("Sure.", 250)
                          .AddOption("Show me the Mount.", 251)
                          .AddOption("Let me check.", 255).AddAvatar(7)
                          .FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 250:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        if (client.Player.BuyItemS != 0 && Database.CoatStorage.MountsBig.Any(p => p.ID == client.Player.BuyItemS) && client.Player.ContainRechargeType(Role.Player.RechargeType.OneMount) == false)
                        {
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.OneMount);

                            client.Inventory.Add(stream, client.Player.BuyItemS);
                            Database.ItemType.DBItem DBInfo;
                            if (Database.Server.ItemsBase.TryGetValue(client.Player.BuyItemS, out DBInfo))
                                client.CreateBoxDialog("You received 1 " + DBInfo.Name + "");
                            client.Player.BuyItemS = 0;

                        }
                        break;
                    }
                case 251:
                    {

                        Database.ItemType.DBItem DBInfo;
                        if (Database.ItemType.SteedMounts.TryGetValue(client.Player.BuyItemS, out DBInfo))
                        {
                            ShowChatItems.ShowFakeItem(client, stream, client.Player.BuyItemS, DBInfo.Durability);

                            data.AddText("Are you sure you wanna buy this Mount (" + DBInfo.Name + ")?!")
                                .AddOption("Sure.", 250)
                                .AddOption("Show me the Mount.", 251)
                                .AddOption("Let me check.", 255).AddAvatar(7)
                                .FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GarmentPacket2)]
        public static void GarmentPacket2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        data.AddText("Select the Garment you need.");
                        byte page = (byte)(Option + 1);
                        int tocount = (int)Math.Min(Database.CoatStorage.GarmentsBig.Count, (page * 10));
                        for (int x = page - 1; x < tocount; x++)
                        {
                            var garment = Database.CoatStorage.GarmentsBig[x];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Garments.TryGetValue(garment.ID, out DBInfo))
                            {
                                data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                            }

                        }
                        data.AddOption("Next Page", page)
                    .AddAvatar(7)
                    .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option < 100)
                        {
                            data.AddText("Select the Garment you need.");
                            byte page = (byte)(Option + 1);
                            int tocount = (int)Math.Min(Database.CoatStorage.GarmentsBig.Count, (page * 10));
                            for (int x = (page - 1) * 10; x < tocount; x++)
                            {
                                var garment = Database.CoatStorage.GarmentsBig[x];
                                Database.ItemType.DBItem DBInfo;
                                if (Database.ItemType.Garments.TryGetValue(garment.ID, out DBInfo))
                                {
                                    data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                                }

                            }
                            if (tocount % 10 == 0)
                                data.AddOption("Next Page", page);
                            data.AddAvatar(7)
                            .FinalizeDialog();
                        }
                        else //get garment.
                        {
                            client.Player.BuyItemS = Database.CoatStorage.GarmentsBig[Option - 100].ID;
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Garments.TryGetValue(client.Player.BuyItemS, out DBInfo))
                            {
                                data.AddText("Are you sure you wanna buy this garment (" + DBInfo.Name + ")?!")
                          .AddOption("Sure.", 250)
                          .AddOption("Show me the garment.", 251)
                          .AddOption("Let me check.", 255).AddAvatar(7)
                          .FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 250:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        if (client.Player.BuyItemS != 0 && Database.CoatStorage.GarmentsBig.Any(p => p.ID == client.Player.BuyItemS) && client.Player.ContainRechargeType(Role.Player.RechargeType.OneGerment2) == false)
                        {
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.OneGerment2);

                            client.Inventory.Add(stream, client.Player.BuyItemS);
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Garments.TryGetValue(client.Player.BuyItemS, out DBInfo))
                                client.CreateBoxDialog("You received 1 " + DBInfo.Name + "");
                            client.Player.BuyItemS = 0;

                        }
                        break;
                    }
                case 251:
                    {

                        Database.ItemType.DBItem DBInfo;
                        if (Database.ItemType.Garments.TryGetValue(client.Player.BuyItemS, out DBInfo))
                        {
                            ShowChatItems.ShowFakeItem(client, stream, client.Player.BuyItemS, DBInfo.Durability);

                            data.AddText("Are you sure you wanna buy this garment (" + DBInfo.Name + ")?!")
                                .AddOption("Sure.", 250)
                                .AddOption("Show me the garment.", 251)
                                .AddOption("Let me check.", 255).AddAvatar(7)
                                .FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SelectSacredRefineryPack)]
        public static void SelectSacredMaterial(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            /* {
                 if (!client.Inventory.Contain(3004249, 1))
                 {
                     client.CreateBoxDialog("Sorry, you don`t have a SacredRefineryPack.");
                     return;
                 }
             }*/


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        {
                            data.AddText("Choose what Refineries with level 6 you need .")
                             .AddOption("Detoxication[Bag]. ", 32) //32
                             .AddOption("Detoxication[Necklace]. ", 33)
                             .AddOption("Breakthrough[Bracelet]. ", 34)
                             .AddOption("Breakthrough[Ring]. ", 36)
                             .AddOption("Penetration[Bracelet]. ", 37)
                             .AddOption("Detoxication[Armor]. ", 38)
                             .AddOption("Next Page.", 39)
                                   .AddAvatar(200)
                                  .FinalizeDialog();
                        }


                        break;
                    }




                #region null2
                case 31:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("Detoxication[Bag]. ", 32)
                        .AddOption("Detoxication[Necklace]. ", 33)
                        .AddOption("Breakthrough[Bracelet]. ", 34)
                        .AddOption("Breakthrough[Ring]. ", 36)
                        .AddOption("Penetration[Bracelet]. ", 37)
                        .AddOption("Detoxication[Armor]. ", 38)
                        .AddOption("Next Page.", 39)

                          .AddAvatar(200)
                        .FinalizeDialog();
                        break;
                    }
                #endregion
                #region Next Page 1
                case 39:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("Counteraction[Necklace]. ", 15)
                        .AddOption("Breakthrough[2-Handed]. ", 16)
                        .AddOption("CriticalStrike[Ring]. ", 17)
                        .AddOption("M-Defense[Bracelet]. ", 18)
                        .AddOption("CriticalStrike[2-Handed]. ", 19)
                        .AddOption("M-Defense[Ring]. ", 20)
                        .AddOption("Next Page.", 21)
                          .AddAvatar(200)
                          .FinalizeDialog();
                        break;
                    }
                #endregion
                #region Next Page 2
                case 21:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("Penetration[Bag]. ", 22)
                        .AddOption("Block[Shield]. ", 23)
                        .AddOption("Detoxication[Boots]. ", 24)
                        .AddOption("Counteraction[Bag]. ", 25)
                        .AddOption("Breakthrough[Bow]. ", 26)
                        .AddOption("Immunity[Boots]. ", 27)
                        .AddOption("Next Page.", 28)
                          .AddAvatar(200)
                          .FinalizeDialog();
                        break;
                    }
                #endregion
                #region Next Page 3
                case 28:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("SkillC.Strike[Bracelet]. ", 29)
                        .AddOption("CriticalStrike[Bow]. ", 30)
                        .AddOption("M-Defense[Bag]. ", 41)
                        .AddOption("Penetration[HeadGear]. ", 42)
                        .AddOption("Block[HeadGear]. ", 43)
                        .AddOption("Detoxication[Headgear]. ", 44)
                        .AddOption("Next Page.", 45)
                          .AddAvatar(200)
                          .FinalizeDialog();
                        break;
                    }
                #endregion
                #region Next Page 4
                case 45:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("SkillC.Strike[Bracelet]. ", 29)
                        .AddOption("CriticalStrike[Bow]. ", 30)
                        .AddOption("M-Defense[Bag]. ", 41)
                        .AddOption("Penetration[HeadGear]. ", 42)
                        .AddOption("Block[HeadGear]. ", 43)
                        .AddOption("Detoxication[Headgear]. ", 44)
                        .AddOption("Next Page.", 46)
                          .AddAvatar(200)
                          .FinalizeDialog();
                        break;
                    }
                #endregion
                #region Next Page 5
                case 46:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("Counteraction[Armor]. ", 47)
                        .AddOption("Breakthrough[1-Handed]. ", 48)
                        .AddOption("Intensification[Headgear]. ", 49)
                        .AddOption("Immunity[Armor]. ", 83)
                        .AddOption("SkillC.Strike[Backsword]. ", 84)
                        .AddOption("CriticalStrike[1-Handed]. ", 85)
                        .AddOption("Next Page.", 86)
                          .AddAvatar(200)
                          .FinalizeDialog();
                        break;
                    }
                #endregion
                #region Next Page 6
                case 86:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("M-Defense[Necklace]. ", 87)
                        .AddOption("SkillC.Strike[Backsword]. ", 88)
                        .AddOption("Penetration[Bracelet]. ", 89)
                        .AddOption("Detoxication[Bag]. ", 90)
                        .AddOption("Penetration[Headgear]. ", 91)
                        .AddOption("Detoxication[Necklace]. ", 92)
                        .AddOption("Next Page.", 93)
                          .AddAvatar(200)
                          .FinalizeDialog();
                        break;
                    }
                #endregion
                #region Next Page 7
                case 93:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("Detoxication[Boots]. ", 94)
                        .AddOption("Detoxication[Armor]. ", 95)
                        .AddOption("Detoxication[Headgear]. ", 96)
                        .AddOption("Block[Shield]. ", 97)
                        .AddOption("Breakthrough[Bow]. ", 101)
                        .AddOption("Breakthrough[2-Handed]. ", 102)
                        .AddOption("Next Page.", 103)
                          .AddAvatar(200)
                          .FinalizeDialog();
                        break;
                    }
                #endregion
                #region Next Page 7
                case 103:
                    {
                        data.AddText("Choose what Refineries with level 6 you need .")
                        .AddOption("Breakthrough[1-Handed]. ", 104)
                        .AddOption("CriticalStrike[2-Handed]. ", 105)
                        .AddOption("CriticalStrike[1-Handed]. ", 106)
                        .AddOption("CriticalStrike[Bow]. ", 107)
                        .AddOption("First page.", 31) //aici
                          .AddAvatar(200)
                         .FinalizeDialog();
                        break;
                    }
                #endregion

                #region Level 6 Refainaries.

                case 107: //CriticalStrike[Bow]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
                        client.Inventory.Add(stream, 3004147, 1);



                        break;
                    }

                case 106: //CriticalStrike[1-Handed]
                    {



                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
                        client.Inventory.Add(stream, 3004137, 1);


                        break;
                    }
                case 105: //CriticalStrike[2-Handed]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004156, 1);


                        break;
                    }

                case 104: //Breakthrough[1-Handed]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004141, 1);


                        break;
                    }

                case 102: //Breakthrough[2-Handed]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004159, 1);


                        break;
                    }
                case 101: //Breakthrough[Bow].
                    {

                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");



                        client.Inventory.Add(stream, 3004150, 1);


                        break;
                    }

                case 97: //Block[Shield].
                    {

                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");



                        client.Inventory.Add(stream, 3004153, 1);


                        break;
                    }

                case 96: //Detoxication[Headgear]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004143, 1);


                        break;
                    }

                case 95: //Detoxication[Armor]
                    {



                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004161, 1);


                        break;
                    }

                case 94: //Detoxication[Boots]
                    {



                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004152, 1);


                        break;
                    }
                case 92: //Detoxication[Necklace]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004165, 1);


                        break;
                    }

                case 91: //Penetration[Headgear]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004145, 1);


                        break;
                    }


                case 90: //Detoxication[Bag]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004154, 1);

                        break;
                    }


                case 89: //Penetration[Bracelet]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004162, 1);


                        break;
                    }

                case 88: //SkillC.Strike[Backsword]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004138, 1);


                        break;
                    }

                case 87: //M-Defense[Necklace]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004136, 1);


                        break;
                    }


                case 85: //CriticalStrike[1-Handed]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004137, 1);


                        break;
                    }


                case 84: //SkillC.Strike[Backsword].
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004138, 1);


                        break;
                    }

                case 83: //Immunity[Armor].
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004139, 1);


                        break;
                    }


                case 49: //Intensification[Headgear]
                    {

                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004140, 1);


                        break;
                    }


                case 48: //Breakthrough[1-Handed]
                    {



                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004141, 1);


                        break;
                    }


                case 47: //Counteraction[Armor]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004142, 1);


                        break;
                    }


                case 44: //Detoxication[HeadGear]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004143, 1);


                        break;
                    }


                case 43: //Block[HeadGear]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004144, 1);


                        break;
                    }


                case 42: //Penetration[HeadGear]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004145, 1);


                        break;
                    }

                case 41: //M-Defense[Bag]
                    {



                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004146, 1);


                        break;
                    }

                case 30: //CriticalStrike[Bow]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004147, 1);


                        break;
                    }

                case 29: //SkillC.Strike[Bracelet]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004148, 1);


                        break;
                    }

                case 27: //Immunity[Boots]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004149, 1);


                        break;
                    }

                case 26: //Breakthrough[Bow]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004150, 1);


                        break;
                    }

                case 25: //Counteraction[Bag]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004151, 1);

                        break;
                    }

                case 24: //Detoxication[Boots]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004152, 1);


                        break;
                    }

                case 23: //Block[Shield]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004153, 1);


                        break;
                    }


                case 22: //Penetration[Bag]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004154, 1);


                        break;
                    }

                case 20: //M-Defense[Ring]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004155, 1);


                        break;
                    }

                case 19: //CriticalStrike[2-Handed]
                    {





                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.Inventory.Add(stream, 3004156, 1);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
                        break;
                    }

                case 18: //M-Defense[Bracelet]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004157, 1);


                        break;
                    }

                case 17: //CriticalStrike[Ring]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004158, 1);

                        break;
                    }

                case 16: //Breakthrough[2-Handed]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004159, 1);


                        break;
                    }

                case 15: //Counteraction[Necklace]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004160, 1);


                        break;
                    }
                case 37: //Penetration[Bracelet]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004162, 1);


                        break;
                    }
                case 36: //Breakthrough[Ring]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004163, 1);



                        break;
                    }

                case 34: //Breakthrough[Bracelet]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004164, 1);


                        break;
                    }
                case 33: //Detoxication[Necklace]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004165, 1);


                        break;
                    }
                case 32: //Detoxication[Bag]
                    {


                        if (!client.Inventory.Remove(3004249, 1, stream)) break;
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004166, 1);


                        break;
                    }
                #endregion
            }
        }

        [NpcAttribute(NpcID.GarmentPacket)]
        public static void GarmentPacket(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        data.AddText("Select the Garment you need.");
                        byte page = (byte)(Option + 1);
                        int tocount = (int)Math.Min(Database.CoatStorage.GarmentsBig.Count, (page * 10));
                        for (int x = page - 1; x < tocount; x++)
                        {
                            var garment = Database.CoatStorage.GarmentsBig[x];
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Garments.TryGetValue(garment.ID, out DBInfo))
                            {
                                data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                            }

                        }
                        data.AddOption("Next Page", page)
                    .AddAvatar(7)
                    .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option < 100)
                        {
                            data.AddText("Select the Garment you need.");
                            byte page = (byte)(Option + 1);
                            int tocount = (int)Math.Min(Database.CoatStorage.GarmentsBig.Count, (page * 10));
                            for (int x = (page - 1) * 10; x < tocount; x++)
                            {
                                var garment = Database.CoatStorage.GarmentsBig[x];
                                Database.ItemType.DBItem DBInfo;
                                if (Database.ItemType.Garments.TryGetValue(garment.ID, out DBInfo))
                                {
                                    data.AddOption("" + DBInfo.Name + "(" + garment.Stars + "Stars)", (byte)(100 + x));
                                }

                            }
                            if (tocount % 10 == 0)
                                data.AddOption("Next Page", page);
                            data.AddAvatar(7)
                            .FinalizeDialog();
                        }
                        else //get garment.
                        {
                            client.Player.BuyItemS = Database.CoatStorage.GarmentsBig[Option - 100].ID;
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Garments.TryGetValue(client.Player.BuyItemS, out DBInfo))
                            {
                                data.AddText("Are you sure you wanna buy this garment (" + DBInfo.Name + ")?!")
                          .AddOption("Sure.", 250)
                          .AddOption("Show me the garment.", 251)
                          .AddOption("Let me check.", 255).AddAvatar(7)
                          .FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 250:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255).AddAvatar(7)
                      .FinalizeDialog();
                            break;
                        }
                        if (client.Player.BuyItemS != 0 && Database.CoatStorage.GarmentsBig.Any(p => p.ID == client.Player.BuyItemS) && client.Player.ContainRechargeType(Role.Player.RechargeType.OneGarment) == false)
                        {
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.OneGarment);

                            client.Inventory.Add(stream, client.Player.BuyItemS);
                            Database.ItemType.DBItem DBInfo;
                            if (Database.ItemType.Garments.TryGetValue(client.Player.BuyItemS, out DBInfo))
                                client.CreateBoxDialog("You received 1 " + DBInfo.Name + "");
                            client.Player.BuyItemS = 0;

                        }
                        break;
                    }
                case 251:
                    {

                        Database.ItemType.DBItem DBInfo;
                        if (Database.ItemType.Garments.TryGetValue(client.Player.BuyItemS, out DBInfo))
                        {
                            ShowChatItems.ShowFakeItem(client, stream, client.Player.BuyItemS, DBInfo.Durability);

                            data.AddText("Are you sure you wanna buy this garment (" + DBInfo.Name + ")?!")
                                .AddOption("Sure.", 250)
                                .AddOption("Show me the garment.", 251)
                                .AddOption("Let me check.", 255).AddAvatar(7)
                                .FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        public static void KingDoomCrystals(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            //To claim a bounty, you should kill 50 enemies in the Realm.
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Crystal, client.Player.Class, 35028);

            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You`ve already claimed a bounty today. No more to claim.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255)
                          .FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 50))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.CreateBoxDialog("You`ve restored peace to the Realm by killing to enemy soldiers, and received a Realm Crystal Fragment.");

                            }
                            else
                                client.CreateBoxDialog("To claim a bounty, you should kill 50 enemies in the Realm.");
                        }
                        else
                            client.CreateBoxDialog("To claim a bounty, you should kill 50 enemies in the Realm.");

                        break;
                    }
            }

        }


        [NpcAttribute(NpcID.GoldPrizeToken)]
        public static void GoldPrizeToken(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (!client.Inventory.Contain(723467, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a LordToken.");
                return;
            }



            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(723467, Database.Server.LordTokenCount))
                        {
                            data.AddText("Hey " + client.Player.Name + ", are you sure if your account is ready to equip the legendary GoldPrize ?!")
                              .AddOption("Yes , i am ready.", 1)
                              .AddOption("Nevermind.", 255)
                              .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hey " + client.Player.Name + ", sorry you need " + Database.Server.LordTokenCount + " Tokens ?!")
                              .AddOption("Nevermind.", 255)
                              .FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            // The full token count is re-checked and consumed before the GoldPrize is granted.
                            if (Database.Server.LordTokenCount == 0 || !HasItems(client, 723467, Database.Server.LordTokenCount))
                                break;
                            if (!client.Inventory.Remove(723467, Database.Server.LordTokenCount, stream)) break;
                            client.Inventory.Add(stream, 2100075, 1);
                            string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to open the legendary LordToken and received 1 GoldPrize.";
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                        }
                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.SashFragment_Realm)]
        public static void SashFragment_Realm(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (!client.Inventory.Contain(3600024, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a SashFragment(Realm).");
                return;
            }



            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("You`ll have a 20% chance to get a Sash (S) if using this fragment alone, or lose it if failed.")
                            .AddText("~While if you have 5 fragments, you can surely combine them into a Sash (S). How many fragments would you like to use?")
                          .AddOption("1~fragment.~(20%~chance)", 1)
                          .AddOption("Combine~5~fragments.", 10)
                          .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        // 5 fragments are consumed exactly once, after the inventory space check, before the Sash is granted.
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(3600024, 5, stream))
                        {
                            data.AddText("You should have 5 Sash Fragments to combine them into a Sash (S). Please retry when you have enough fragments.")
                   .AddOption("Alright.", 255)
                   .FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 1100003);
                        data.AddText("You successfully made a Sash (S)!")
                  .AddOption("Great!", 255)
                  .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Remove(3600024, 1, stream)) break;
                        if (Role.Core.Rate(20))
                        {
                            client.Inventory.Add(stream, 1100003);
                            data.AddText("Wow, it must be your lucky day! You successfully made a Sash (S)!")
                   .AddOption("Great!", 255)
                   .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The fragment vanished since you failed to make a Sash (S) with it.")
                    .AddOption("Great!", 255)
                    .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ExplosiveDevide1)]
        public static void ExplosiveDevide1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                client.CreateBoxDialog("The Deserted Village has been burnt down into ashes. Report back to the Kingdom Mission Envoy and claim your reward.");

                            }
                            else
                                client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExplosiveDevide2)]
        public static void ExplosiveDevide2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                client.CreateBoxDialog("The Deserted Village has been burnt down into ashes. Report back to the Kingdom Mission Envoy and claim your reward.");
                            }
                            else
                                client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExplosiveDevide3)]
        public static void ExplosiveDevide3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                client.CreateBoxDialog("The Deserted Village has been burnt down into ashes. Report back to the Kingdom Mission Envoy and claim your reward.");
                            }
                            else
                                client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExplosiveDevide4)]
        public static void ExplosiveDevide4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                client.CreateBoxDialog("The Deserted Village has been burnt down into ashes. Report back to the Kingdom Mission Envoy and claim your reward.");
                            }
                            else
                                client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
    }
}
