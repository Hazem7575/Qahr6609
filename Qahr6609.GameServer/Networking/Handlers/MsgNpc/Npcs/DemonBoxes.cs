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

        [NpcAttribute(NpcID.FloodDemonBox)]
        public static void FloodDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720652, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Flood Demon, you will have a chance to get a Fantasy CP Pack (13800 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            if (client.Player.View.ContainMobInScreen("FloodDemon"))
                            {
                                client.SendSysMesage("The Flood Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                                break;
                            }

                            if (!client.Inventory.Remove(720652, 1, stream)) break;
                            Database.Server.AddMapMonster(stream, client.Map, 2422, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Flood Demon Box and a Flood Demon was released! Hurry and kill it to get your reward!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is unavailable for use in the cities and Market.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AncientDemonBox)]
        public static void AncientDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720651, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Ancient Demon, you will have a chance to get a Mystic CP Pack (6900~CPs). Are you sure you want to release it now?");
                        data.AddText("~now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            if (client.Player.View.ContainMobInScreen("AncientDemon"))
                            {
                                client.SendSysMesage("The Ancient Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                                break;
                            }
                            if (!client.Inventory.Remove(720651, 1, stream)) break;
                            Database.Server.AddMapMonster(stream, client.Map, 2421, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Ancient Demon Box and an Ancient Demon was released! Hurry and kill it to get your reward!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is unavailable for use in the cities and Market.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.DemonBox)]
        public static void DemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720650, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Demon, you will have a chance to get a Joy CP Pack (1380 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            if (client.Player.View.ContainMobInScreen("Demon"))
                            {
                                client.SendSysMesage("The Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                                break;
                            }
                            if (!client.Inventory.Remove(720650, 1, stream)) break;
                            Database.Server.AddMapMonster(stream, client.Map, 2420, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Demon Box and a demon was released! Hurry and kill it to get the reward!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is unavailable for use in the cities and Market.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AuroraDemonBox)]
        public static void AuroraDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720674, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you open the Aurora Demon Box, a demon will be released! After you kill it, you`ll have a chance to get a Pilgrim CP Pack (690000 CPs)! I");
                        data.AddText("~suggest you open it in a secret place to keep from being robbed. Are you sure you want to open it and release the demon now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            if (client.Player.View.ContainMobInScreen("AuroraDemon"))
                            {
                                client.SendSysMesage("The Aurora Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                                break;
                            }
                            if (!client.Inventory.Remove(720674, 1, stream)) break;
                            Database.Server.AddMapMonster(stream, client.Map, 2438, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Aurora Demon Box and a demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is unavailable for use in the cities and Market.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SacredDemonBox)]
        public static void SacredDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720673, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you open the Sacred Demon Box, a demon will be released! After you kill it, you`ll have a chance to get a Kylin CP Pack (276000 CPs)! I");
                        data.AddText("~suggest you open it somewhere safe from prying eyes. Are you sure you want to open it and release the demon now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            if (client.Player.View.ContainMobInScreen("SacredDemon"))
                            {
                                client.SendSysMesage("The Sacred Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                                break;
                            }
                            if (!client.Inventory.Remove(720673, 1, stream)) break;
                            Database.Server.AddMapMonster(stream, client.Map, 2437, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Sacred Demon Box and a Sacred Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is unavailable for use in the cities and Market.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ChaosDemonBox)]
        public static void ChaosDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720672, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Chaos Demon, you will have a chance to get a Nimbus CP Pack (138000 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {

                            if (client.Player.View.ContainMobInScreen("ChaosDemon"))
                            {
                                client.SendSysMesage("The Chaos Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                                break;
                            }
                            if (!client.Inventory.Remove(720672, 1, stream)) break;
                            Database.Server.AddMapMonster(stream, client.Map, 2436, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Chaos Demon Box and a Chaos Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is unavailable for use in the cities and Market.", MsgMessage.ChatMode.System);
                        }
                        break;

                    }
            }
        }
        [NpcAttribute(NpcID.HeavenDemonBox)]
        public static void HeavenDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720671, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Heaven Demon, you will have a chance to get a Frost CP Pack (69000 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            if (client.Player.View.ContainMobInScreen("HeavenDemon"))
                            {
                                client.SendSysMesage("The Heaven Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                                break;
                            }

                            if (!client.Inventory.Remove(720671, 1, stream)) break;
                            Database.Server.AddMapMonster(stream, client.Map, 2435, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Heaven Demon Box and a Heaven Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is unavailable for use in the cities and Market.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
    }
}
