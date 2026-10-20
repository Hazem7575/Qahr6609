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
        [NpcAttribute(NpcID.GarmentSpecialist)]
        public static void GarmentSpecialist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Fresh vegtebles! Fair Guys, take a look!")

                           .AddOption("Sounds good.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                        }
            }

        }
        [NpcAttribute(NpcID.RedPaintbox)]
        public static void RedPaintbox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This is a Red Paint Box. Be carefull. Don't knock it over.")

                           .AddOption("I don't need this color.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.YellowPaintbox)]
        public static void YellowPaintbox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This is a Yellow Paint box. Be carefull. Be carefull. Don't knock it over.")

                           .AddOption("I don't need this color.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.BluePaintbox)]
        public static void BluePaintbox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("This is a Blue Paint box. Be carefull. Don't knock it over.")

                           .AddOption("I don't need this color.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.GreatPainterZoe)]
        public static void GreatPainterZoe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("The Anniversary is over. See you next year!.")

                           .AddOption("See you!.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.Bonfire)]
        public static void Bonfire(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("The Bonfire is over. See you next year!.")

                           .AddOption("See you!.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.MountArmorSpecialist)]
        public static void MountArmorSpecialist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("The Mount's Specialist is over. See you next year!.")

                           .AddOption("See you!.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.LuckyEggEnvoy)]
        public static void LuckyEggEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("The LuckyEgg's Envoy is over. See you next year!.")

                           .AddOption("See you!.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
            }

        }

        //L60UniqueGearPack
        [NpcAttribute(NpcID.L60UniqueGearPack)]
        public static void L60UniqueGearPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.SendSysMesage("Please make 2 more spaces in your inventory.");
                            break;
                        }

                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 410119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 130067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 118067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 561119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 131067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 141067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            data.AddText("What~kind~of~weapon~do~you~want?");
                            data.AddOption("Throwing~Knife.", 1);
                            data.AddOption("Bow.", 2);
                            data.AddOption("Let~me~see...");
                            data.FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            data.AddText("What~kind~of~weapon~do~you~want?");
                            data.AddOption("Katana.", 3);
                            data.AddOption("Scythe.", 4);
                            data.AddOption("Let~me~see...");
                            data.FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {

                                client.Inventory.Add(stream, 610119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 136067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 143067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            data.AddText("What~kind~of~weapon~do~you~want?");
                            data.AddOption("Rapier.", 5);
                            data.AddOption("Pistol.", 6);
                            data.AddOption("Let~me~see...");
                            data.FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 617119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 138057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 148057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 421129, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 134067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 114067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 626119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 101067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 170067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        break;
                    }
                case 5:
                    {

                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 611119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 101057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 145067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");


                        }
                        break;
                    }
                case 6:
                    {

                        if (client.Inventory.Remove(3000588, 1, stream))
                        {

                            client.Inventory.Add(stream, 612119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 139067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 145067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");

                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 601119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 135067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 123067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 511119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 135067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 123067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 613109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 133047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 142037, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 500109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 133047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 142037, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                        }

                        break;
                    }
            }

        }
    }
}
