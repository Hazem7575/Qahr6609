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
        [NpcAttribute(NpcID.Steed3Pack)]
        public static void Steed3Pack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3007283, 1, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1Pack.");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can claim one from the three +3 Steeds (B) in this pack. Which one do you like?")
                            .AddOption("+3 White Steed(B)", 1)
                       .AddOption("+3 Brown Steed(B)", 5)
                       .AddOption("+3 Black Steed(B)", 10)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you sure you want to claim a +3 Black Steed (B) from this pack?")
                           .AddOption("Yes.", 11)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.SendSysMesage("You received +3 Black Steed (B)!", MsgMessage.ChatMode.System);
                        if (!client.Inventory.Remove(3007283, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 255, 0, 150);
                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to claim a +3 Brown Steed (B) from this pack?")
                           .AddOption("Yes.", 6)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.SendSysMesage("You received +3 Brown Steed (B)!", MsgMessage.ChatMode.System);
                        if (!client.Inventory.Remove(3007283, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 0, 150, 255);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to claim a +3 White Steed (B) from this pack?")
                           .AddOption("Yes.", 2)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You received +3 White Steed (B)!", MsgMessage.ChatMode.System);
                        if (!client.Inventory.Remove(3007283, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 150, 255, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Steed1Pack)]
        public static void Steed1Pack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3007287, 1, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1Pack.");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can claim one from the three +1 Steeds (B) in this pack. Which one do you like?")
                            .AddOption("+1 White Steed(B)", 1)
                       .AddOption("+1 Brown Steed(B)", 5)
                       .AddOption("+1 Black Steed(B)", 10)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you sure you want to claim a +1 Black Steed (B) from this pack?")
                           .AddOption("Yes.", 11)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.SendSysMesage("You received +1 Black Steed (B)!", MsgMessage.ChatMode.System);
                        if (!client.Inventory.Remove(3007287, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 255, 0, 150);
                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to claim a +1 Brown Steed (B) from this pack?")
                           .AddOption("Yes.", 6)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.SendSysMesage("You received +1 Brown Steed (B)!", MsgMessage.ChatMode.System);
                        if (!client.Inventory.Remove(3007287, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 0, 150, 255);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to claim a +1 White Steed (B) from this pack?")
                           .AddOption("Yes.", 2)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You received +1 White Steed (B)!", MsgMessage.ChatMode.System);
                        if (!client.Inventory.Remove(3007287, 1, stream)) break;
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 150, 255, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ServerTransfer)]
        public static void ServerTransfer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Guys, I`m responsible for transferring servers. If you don`t wanna stay here anymore, ")
                            .AddText("I can help anytime except 07:00 - 09:30 and 15:00 - 17:30. Anything?")
                  .AddOption("Yeah,~tell~me~more~about~it.", 1)
                  .AddOption("I~wanna~transfer~now.", 10)
                  .AddOption("Nothing.", 255).AddAvatar(186)
                  .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("I like curious people, seriously. There`re something you need to do before transferring:")
                       .AddText("1. Cancel all your relationships with this server, including guild, clan, mentor, trade partner and marriage./n")
                       .AddText("2. Make sure you aren`t detaining other players` equipment, or your equipment is being detained by others./n")
                       .AddText("3. Retrieve all items from your itemboxes in your house./n")
                        .AddText("4. Claim all your TQ Point Cards./n")
                         .AddText("5. View all your unread messages./n")
                          .AddText("6. Claim your mentor exp./n")
                          .AddText("After that, give me 200,000 CPs, done!")
             .AddOption("I~got~it.", 255).AddAvatar(186)
             .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (Program.OnMainternance)
                        {
                            client.CreateBoxDialog("You are not eligible to get transferred right now , please wait for maintenance .");
                            return;
                        }

                        data.AddText("Select a server to transfer. ");
                        foreach (var server in Database.GroupServerList.CanTransferServers())
                            data.AddOption("" + server.Name + "", (byte)(100 + server.ID));
                        data.AddOption("Nothing.", 255)
                            .AddAvatar(186)
                        .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option > 100)
                        {
                            if (client.Player.ConquerPoints < 200000)
                            {
                                data.AddText("You need to have 200k CPs for transfer ! ")
                .AddOption("I~see.", 255).AddAvatar(186)
                .FinalizeDialog();
                                break;
                            }
                            if (client.Inventory.HaveItemsInSash())
                            {
                                data.AddText("You can't transfer with sashes ,Your sashes will dissapear after transfering , be carefull! ")
                  .AddOption("I~see.", 255).AddAvatar(186)
                  .FinalizeDialog();
                                break;
                            }
                            if (client.Warehouse.HaveItemsInBanks())
                            {
                                data.AddText("You need to clean your all werehouses before you transfer!")
                 .AddOption("I~see.", 255).AddAvatar(186)
                 .FinalizeDialog();

                                return;
                            }
                            if (client.MyWardrobe.HaveItems())
                            {
                                data.AddText("You need to clean your Wardrobe(garment/mount) before you transfer!")
             .AddOption("I~see.", 255).AddAvatar(186)
             .FinalizeDialog();
                                return;
                            }

                            if (client.Player.Associate.HaveAsociats())
                            {
                                data.AddText("Remove your (trade parteners/friends/mentor/apprentice) before transfer!")
               .AddOption("I~see.", 255).AddAvatar(186)
               .FinalizeDialog();
                                return;
                            }
                            if (client.Player.SpouseUID != 0)
                            {
                                data.AddText("You need to be divorced before transfer!")
                                    .AddOption("I~see.", 255).AddAvatar(186)
                                    .FinalizeDialog();
                                return;
                            }
                            if (client.Player.MyGuild != null || client.Player.InUnion || client.Player.MyClan != null)
                            {
                                data.AddText("Please remove your character from Guild , Clan and Union before transfer!")
                                .AddOption("I~see.", 255).AddAvatar(186)
                                .FinalizeDialog();
                                return;
                            }

                            if (client.Player.IsCheckedPass == false)
                            {
                                client.CreateBoxDialog("Make~sure~you~have~entered~a~correct~secondary~password~in~the~warehouse!");
                                return;
                            }
                            var server = Database.GroupServerList.GetServer((uint)(Option - 100));
                            if (server != null)
                            {

                                client.Player.MessageBox("Are~you~sure~you~want~to~transfer~to~" + server.Name + "~server?",
                                    p => p.Player.InitializeTransfer(server.ID), null, 0, MsgStaticMessage.Messages.None);

                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BlackFridayAccesory)]
        public static void BlackFridayAccesory(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3100053, 1))
                return;
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
                        if (client.Player.BuyItemS != 0 && Database.ItemType.TopAccesorys.Contains(client.Player.BuyItemS))
                        {
                            if (!client.Inventory.Remove(3100053, 1, stream)) break;

                            client.Inventory.Add(stream, client.Player.BuyItemS, 1, 0, 1);
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
        [NpcAttribute(NpcID.BlackFridayMountPack)]
        public static void BlackFridayMountPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3100054, 1))
                return;

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
                        if (client.Player.BuyItemS != 0 && Database.CoatStorage.MountsBig.Any(p => p.ID == client.Player.BuyItemS))
                        {
                            if (!client.Inventory.Remove(3100054, 1, stream)) break;

                            client.Inventory.Add(stream, client.Player.BuyItemS, 1, 0, 1);
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

        [NpcAttribute(NpcID.BlackFridayGarmentPack)]
        public static void BlackFridayGarmentPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3100055, 1))
                return;

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
                        if (client.Player.BuyItemS != 0 && Database.CoatStorage.GarmentsBig.Any(p => p.ID == client.Player.BuyItemS))
                        {
                            if (!client.Inventory.Remove(3100055, 1, stream)) break;
                            client.Inventory.Add(stream, client.Player.BuyItemS, 1, 0, 1);
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
    }
}
