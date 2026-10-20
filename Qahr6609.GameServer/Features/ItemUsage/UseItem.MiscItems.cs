using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {
        // UseItem handlers: everything else.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_MiscItems(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {

                case 725067:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(1003010, 0, 5, stream);
                        break;
                    }
                case 725068:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(1004010, 0, 5, stream);
                        break;
                    }
                case 1060020:
                    {
                        if (client.Player.Map == 10137)
                        {
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(301, 279, 1002);

                        break;
                    }
                case 1060021:
                    {
                        if (client.Player.Map == 10137)
                        {
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(500, 650, 1000);

                        break;
                    }
                case 1060022:
                    {
                        if (client.Player.Map == 10137)
                        {
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(565, 562, 1020);

                        break;
                    }
                case 1060023:
                    {
                        if (client.Player.Map == 10137)
                        {
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(188, 264, 1011);
                        break;
                    }
                case 1060024:
                    {
                        if (client.Player.Map == 10137)
                        {
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(717, 571, 1015);
                        break;
                    }
                case 1060039:
                    {
                        if (client.Player.Map == 10137)
                        {
                            return;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(535, 558, 1217);
                        break;
                    }
                case 1060025:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(301, 279, 1002);

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(405, 655, 1002);
                        break;
                    }
                case 1060026:
                    {

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(65, 252, 1002);
                        break;
                    }
                case 1060027:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(443, 372, 1002);
                        break;
                    }
                case 1060028:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(537, 767, 1011);
                        break;
                    }
                case 1060029:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(734, 448, 1011);
                        break;
                    }
                case 1060038:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(64, 426, 1011);
                        break;
                    }
                case 1060031:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(819, 602, 1020);
                        break;
                    }
                case 1060032:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(492, 726, 1020);
                        break;
                    }
                case 1060033:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(105, 396, 1020);
                        break;
                    }
                case 1060034:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(226, 201, 1000);
                        break;
                    }
                case 1060035:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(797, 547, 1000);
                        break;
                    }
                case 1060037:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (!Program.BlockTeleportMap.Contains(client.Player.Map))
                            client.Teleport(476, 365, 1001);
                        break;
                    }

                case 721540://AncestorBox
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            switch (Program.GetRandom.Next(0, 8))
                            {
                                case 0:
                                    {
                                        client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Inventory.Add(stream, 1088000, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Inventory.Add(stream, 700031, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Inventory.Add(stream, 700001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Inventory.Add(stream, 700011, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        break;
                                    }
                                case 6:
                                    {
                                        client.Inventory.Add(stream, 700062, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        break;
                                    }
                                case 7:
                                    {
                                        client.Inventory.Add(stream, 700022, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        break;
                                    }
                            }
                        }
                        else
                            client.SendSysMesage("Please prepare at least four slots in your inventory.");

                        break;
                    }
                case 720021:
                    {
                        client.UseItem = item.ITEM_ID;



                        NpcServerQuery Furniture = new NpcServerQuery();
                        Furniture.NpcType = Role.Flags.NpcType.Talker;
                        Furniture.Action = MsgNpc.NpcServerReplay.Mode.Cursor;

                        var conductor = MsgTournaments.MsgSchedules.GuildWar.GuildConductors[MsgNpc.NpcID.TeleGuild1];
                        Furniture.Mesh = (ushort)(conductor.Npc.Mesh != 0 ? conductor.Npc.Mesh : 1457);
                        Furniture.ID = MsgNpc.NpcID.TeleGuild1;

                        client.MoveNpcMesh = Furniture.Mesh;
                        client.MoveNpcUID = (uint)Furniture.ID;
                        client.Send(stream.NpcServerCreate(Furniture));
                        break;
                    }
                case 720022:
                    {
                        client.UseItem = item.ITEM_ID;
                        NpcServerQuery Furniture = new NpcServerQuery();
                        Furniture.NpcType = Role.Flags.NpcType.Talker;
                        Furniture.Action = MsgNpc.NpcServerReplay.Mode.Cursor;

                        var conductor = MsgTournaments.MsgSchedules.GuildWar.GuildConductors[MsgNpc.NpcID.TeleGuild2];
                        Furniture.Mesh = (ushort)(conductor.Npc.Mesh != 0 ? conductor.Npc.Mesh : 1467);
                        Furniture.ID = MsgNpc.NpcID.TeleGuild2;

                        client.MoveNpcMesh = Furniture.Mesh;
                        client.MoveNpcUID = (uint)Furniture.ID;
                        client.Send(stream.NpcServerCreate(Furniture));
                        break;
                    }
                case 720023:
                    {
                        client.UseItem = item.ITEM_ID;
                        NpcServerQuery Furniture = new NpcServerQuery();
                        Furniture.NpcType = Role.Flags.NpcType.Talker;
                        Furniture.Action = MsgNpc.NpcServerReplay.Mode.Cursor;

                        var conductor = MsgTournaments.MsgSchedules.GuildWar.GuildConductors[MsgNpc.NpcID.TeleGuild3];
                        Furniture.Mesh = (ushort)(conductor.Npc.Mesh != 0 ? conductor.Npc.Mesh : 1477);
                        Furniture.ID = MsgNpc.NpcID.TeleGuild3;

                        client.MoveNpcMesh = Furniture.Mesh;
                        client.MoveNpcUID = (uint)Furniture.ID;
                        client.Send(stream.NpcServerCreate(Furniture));
                        break;
                    }
                case 720024:
                    {
                        client.UseItem = item.ITEM_ID;
                        NpcServerQuery Furniture = new NpcServerQuery();
                        Furniture.NpcType = Role.Flags.NpcType.Talker;
                        Furniture.Action = MsgNpc.NpcServerReplay.Mode.Cursor;

                        var conductor = MsgTournaments.MsgSchedules.GuildWar.GuildConductors[MsgNpc.NpcID.TeleGuild4];
                        Furniture.Mesh = (ushort)(conductor.Npc.Mesh != 0 ? conductor.Npc.Mesh : 1487);
                        Furniture.ID = MsgNpc.NpcID.TeleGuild4;

                        client.MoveNpcMesh = Furniture.Mesh;
                        client.MoveNpcUID = (uint)Furniture.ID;
                        client.Send(stream.NpcServerCreate(Furniture));
                        break;
                    }

                case 3003125:
                    {
                        client.Activeness.IncreaseTask(39);

//                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
//                                client.Player.JingTryngUltra = 8;
//#if Arabic
//                                   client.SendSysMesage("You have just 8 trying to make Ultra Star.");
//#else
//                                client.SendSysMesage("You have just 8 trying to make Ultra Star.");
//#endif

                        break;
                    }
                case 727060:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalGloryGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727061:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalThunderGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727062:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalKylinGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727063:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalRainbowGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727064:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalFuryGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727065:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalDragonGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727066:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalPhoenixGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727067:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalVioletGem), 5);
                        }
                        else
                            client.SendSysMesage("Your Inventory Is Full , you need 4 spaces!");
                        break;
                    }
                case 727068:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalMoonGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 727069:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, (uint)(700000 + (uint)Role.Flags.Gem.NormalTortoiseGem), 5);
                        }
                        else
                        {
                            client.SendSysMesage("Please make 4 more spaces in your inventory.");

                        }
                        break;
                    }
                case 3000254:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200407, 1, 0, 1);
                        break;
                    }
                case 3000266:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200446, 1, 0, 1);
                        break;
                    }
                case 3000257:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200464, 1, 0, 1);
                        break;
                    }
                case 3000263:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200418, 1, 0, 1);
                        break;
                    }
                case 3000265:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200420, 1, 0, 1);
                        break;
                    }
                case 3000267:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200427, 1, 0, 1);
                        break;
                    }
                case 3000268:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200431, 1, 0, 1);
                        break;
                    }
                case 3000256:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200411, 1, 0, 1);
                        break;
                    }
                case 3000258:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200413, 1, 0, 1);
                        break;
                    }
                case 3000259:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200414, 1, 0, 1);
                        break;
                    }
                case 3000260:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200415, 1, 0, 1);
                        break;
                    }
                case 3000261:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200416, 1, 0, 1);
                        break;
                    }
                case 3000262:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200417, 1, 0, 1);
                        break;
                    }
                case 3000264:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200419, 1, 0, 1);
                        break;
                    }
                case 3000269:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200438, 1, 0, 1);
                        break;
                    }
                case 3000270:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200444, 1, 0, 1);
                        break;
                    }
                case 3000271:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 200445, 1, 0, 1);
                        break;
                    }
                case 729979:
                case 729980:
                case 729981:
                    {
                        client.Inventory.Remove(729981, 1, stream);
                        client.Inventory.Remove(729980, 1, stream);
                        client.Inventory.Remove(729979, 1, stream);
                        client.Teleport(535, 477, 1768);
                        ActionQuery action = new ActionQuery();
                        action.ObjId = client.Player.UID;
                        action.Type = ActionType.CountDown;
                        action.dwParam = (uint)300;
                        client.Send(stream.ActionCreate(&action));
                        client.Player.TaskQuestTimer = DateTime.Now.AddSeconds(300);
                        break;
                    }
                case 720963:
                    {

                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.ClikerON,
                            Fascing = 7,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            dwParam = 0x0c,
                            PacketStamp = 0

                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }
                case 720985:
                case 723724:
                    {
                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                        client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, Database.Tranformation.GetRandomTransform(), 60, 1360);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 728899:
                    {
                        if (client.Inventory.Contain(728899, 6))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                client.CreateBoxDialog("Please make 1 more space in your inventory");
                                break;
                            }
                            client.Inventory.Remove(728899, 6, stream);
                            client.Inventory.Add(stream, 720880);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e104");
                            client.CreateBoxDialog("You~received~a~Horse~Racing~Points~Pack(3K)~after~you~combined~6~Tiny~Horse~Racing~Points~Bags!");

                        }
                        else
                            client.CreateBoxDialog("You~need~to~have~6~Tiny~Horse~Racing~Points~Bags~to~combine~and~get~a~Horse~Racing~Points~Pack(3K).");
                        break;
                    }
                case 729508:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                        client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { "xp" });
                        break;
                    }
                case 720837:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                        client.Player.TransformInfo.CreateTransform(stream, 817, 201, (int)60);
                        break;
                    }
                case 3200748:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3009001, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                        break;
                    }
                case 3200747:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3009000, 2, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                        break;
                    }
                case 720840:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        Database.Server.AddMapMonster(stream, client.Map, 8303, client.Player.X, client.Player.Y, 5, 5, 1);

                        break;
                    }
                case 711331:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaDragon, client.Player.Class, 1133);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1133, 1);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        break;
                    }
                case 721700:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        var map = Database.Server.ServerMaps[1013];
                        var dinamic = map.GenerateDynamicID();
                        Database.Server.AddMapMonster(stream, map, 14333, 30, 30, 18, 18, 13, dinamic, false);
                        client.Teleport(50, 50, 1013, dinamic);
                        client.CreateBoxDialog("You`ve~taken~the~toxin~and~found~yourself~in~a~strange~place.");

                        break;
                    }
                case 721701:
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 795, 471) < 18)
                        {
                            if (client.Player.View.ContainMobInScreen("VenomousApe"))
                            {
                                Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                                dialog.AddText("Please kill all VenomousApe`s.")
                                    .FinalizeDialog();
                                break;
                            }

                            client.Player.QuestMultiple = 1;
                            Database.Server.AddMapMonster(stream, client.Map, 14334, client.Player.X, client.Player.Y, 7, 7, 7, 0, true);
                        }
                        else
                        {
                            Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                            dialog.AddText("Wrong place. You may only use the Metamorphic Vial in Love Canyon (795,471).")
                                .FinalizeDialog();

                        }
                        break;
                    }
                case 721707:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1743, 0, 1);
                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                        client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, 247, 60, 1360);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 729972:
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 100, 430) < 18)
                        {
                            Database.Server.AddMapMonster(stream, client.Map, 1403, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        }
                        else
                        {
                            Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                            dialog.AddText("Visit Zhao Jian (200,325), disguise yourself as a young girl, then use the offering at the Altar (100,430).")
                               .FinalizeDialog();


                        }
                        break;
                    }
                case 729973:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 729974);
                        break;
                    }
                case 3002307:
                    {

                        if (client.Inventory.Contain(3002307, 3))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                client.CreateBoxDialog("Please make 1 more space in your inventory.");

                                break;
                            }
                            client.Inventory.Remove(3002307, 3, stream);
                            client.Inventory.Add(stream, 3002308);
                            client.SendSysMesage("You've received HeavenKey.", MsgMessage.ChatMode.System);

                        }
                        else
                        {

                            client.CreateBoxDialog("Failed to combine. You don`t have 3 Heaven Ket Scraps.");
                        }
                        break;
                    }
                case 729307:
                case 729308:
                case 729309:
                case 729310:
                case 729306:
                    {
                        client.Player.AddPick(stream, "Pick", 2);

                        break;
                    }
                case 3005160:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 1200000);
                        break;
                    }
                case 3005161:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        Database.ItemType.DBItem aitem;
                        while (true)
                        {
                            var array = Database.ItemType.Garments.Values.ToArray();
                            aitem = array[Program.GetRandom.Next(0, array.Length)];
                            Database.CoatStorage.StorageItem DbGarment;
                            if (Database.CoatStorage.Garments.TryGetValue(aitem.ID, out DbGarment))
                            {
                                if (DbGarment.Stars > 3)
                                {
                                    client.Inventory.Add(stream, aitem.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, ".", 7);
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case 3005164:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage("Please make 1 more space in your inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        Database.ItemType.DBItem aitem;
                        while (true)
                        {
                            var array = Database.ItemType.Garments.Values.ToArray();
                            aitem = array[Program.GetRandom.Next(0, array.Length)];
                            Database.CoatStorage.StorageItem DbGarment;
                            if (Database.CoatStorage.Garments.TryGetValue(aitem.ID, out DbGarment))
                            {
                                if (DbGarment.Stars <= 4)
                                {
                                    client.Inventory.Add(stream, aitem.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    break;
                                }
                            }
                        }
                        client.Inventory.Add(stream, 1200000);
                        break;
                    }
                case 3005167:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.SendSysMesage("Please make 2 more spaces in your inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        Database.ItemType.DBItem aitem;
                        while (true)
                        {
                            var array = Database.ItemType.Garments.Values.ToArray();
                            aitem = array[Program.GetRandom.Next(0, array.Length)];
                            Database.CoatStorage.StorageItem DbGarment;
                            if (Database.CoatStorage.Garments.TryGetValue(aitem.ID, out DbGarment))
                            {
                                if (DbGarment.Stars <= 3)
                                {
                                    client.Inventory.Add(stream, aitem.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    break;
                                }
                            }
                        }
                        client.Inventory.Add(stream, 3006393);
                        uint[] Items = new uint[]
                        {
                            (uint)(700000 + (uint)Role.Flags.Gem.RefinedDragonGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedFuryGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedGloryGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedKylinGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedMoonGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedPhoenixGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedRainbowGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedThunderGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedTortoiseGem)
                            ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedVioletGem)
                        };
                        uint ItemID = Items[(uint)Program.GetRandom.Next(0, Items.Length)];
                        client.Inventory.Add(stream, ItemID, 1);
                        break;
                    }
                case 3001289:
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, Database.ItemType.MoonBox, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 1080001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 721259, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                        }
                        else
                        {
                            client.SendSysMesage("Please make 3 more spaces in your inventory.");

                        }
                        break;
                    }
                case 3000782:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(723727, 0, 10, stream, true);
                        break;
                    }
                case 720844:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        Database.Server.AddMapMonster(stream, client.Map, 1, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                        break;
                    }
                case 3600023:
                    {
                        MsgTaskReward.OpenWheel(client, stream, 3600023, 3668);
                        break;
                    }
                case 3100013:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.SendSysMesage("Please make 2 more space in inventory.");

                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3100012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        byte rand = (byte)Program.GetRandom.Next(1, 100);
                        if (rand < 40)
                        {
                            Database.ItemType.DBItem _item;
                            while (true)
                            {
                                var array = Database.ItemType.Garments.Values.ToArray();
                                _item = array[Program.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DbGarment;
                                if (Database.CoatStorage.Garments.TryGetValue(_item.ID, out DbGarment))
                                {
                                    if (DbGarment.Stars <= 4)
                                    {
                                        client.Inventory.Add(stream, _item.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.CreateBoxDialog("You received " + _item.Name + ".");

                                        break;
                                    }
                                }
                            }

                        }
                        else
                        {
                            Database.ItemType.DBItem _item;
                            while (true)
                            {
                                var array = Database.ItemType.SteedMounts.Values.ToArray();
                                _item = array[Program.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DBMount;
                                if (Database.CoatStorage.Mounts.TryGetValue(_item.ID, out DBMount))
                                {
                                    if (DBMount.Stars <= 4)
                                    {
                                        client.Inventory.Add(stream, _item.ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                        client.CreateBoxDialog("You received " + _item.Name + ".");

                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 720865:
                    {
                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                        client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, 356, 60, 1360);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1364, 1);
                        break;
                    }
                case 720870:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.HitPoints = 10;
                        client.Player.SendUpdateHP();
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1350, 1);
                        break;
                    }

                case 720861:
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 231, 431) < 18 && client.Player.Map == 1020)
                        {
                            if (client.Player.View.ContainMobInScreen("SnakemanElder") == false)
                                Database.Server.AddMapMonster(stream, client.Map, 14337, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        }
                        else
                        {
                            client.SendSysMesage("Go to territory of the Apes at(231, 431) to use this item!", MsgMessage.ChatMode.System);

                        }
                        break;
                    }
                case 720862:
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 360, 696) < 18 && client.Player.Map == 1020)
                        {
                            if (client.Player.View.ContainMobInScreen("HeresyElder") == false)
                                Database.Server.AddMapMonster(stream, client.Map, 14338, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);


                        }
                        else
                        {
                            client.SendSysMesage("Go to territory of the Apes at(360,696) to use this item!", MsgMessage.ChatMode.System);

                        }

                        break;
                    }
                case 720852:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.ClikerON,
                            Fascing = 7,
                            wParam1 = client.Player.X,
                            wParam2 = client.Player.Y,
                            dwParam = 0x0c,
                            PacketStamp = 0

                        };
                        client.Send(stream.ActionCreate(&action));
                        break;
                    }


                case 3001405:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, Database.ItemType.MoonBox, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You received 1 MoonBox.");

                        }
                        else
                        {
                            client.SendSysMesage("Please make 1 more spaces in your inventory.");

                        }
                        break;
                    }



                case 720869:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");

                            break;
                        }
                        if (client.Player.Map == 1020 && Role.Core.GetDistance(client.Player.X, client.Player.Y, 353, 252) < 18)
                        {
                            if (DateTime.Now > client.Player.SickleStamp.AddSeconds(10))
                            {
                                client.Inventory.AddItemWitchStack(720868, 0, 1, stream);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1349, 1);
                                client.SendSysMesage("You received 1 ThunderGinseng.", MsgMessage.ChatMode.System);

                                client.Player.SickleStamp = DateTime.Now;
                            }
                            else
                            {
                                client.SendSysMesage("Please wait for 10 secounds to use the Sickle again.", MsgMessage.ChatMode.System);

                            }
                        }
                        else
                        {
                            client.SendSysMesage("Go to (353,252) to use the Sickle to collect Thunder Ginseng!", MsgMessage.ChatMode.System);


                        }
                        break;
                    }
                case 720995:
                    {
                        client.Player.AddPick(stream, "Poison", 2);
                        break;
                    }
                case 720994:
                    {
                        if (client.Inventory.Contain(711479, 1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Remove(711479, 1, stream);
                            client.Inventory.Add(stream, 720995);
                            client.SendSysMesage("You used the Poison Liquid on the Salted Fish. When the liquid diffuses into the Salted Fish completely, the Poison Fish small good still.", MsgMessage.ChatMode.System);
                        }

                        break;
                    }
                case 720987:
                case 720988:
                case 720989:
                    {
                        if (client.Inventory.Contain(720987, 1) && client.Inventory.Contain(720988, 1) && client.Inventory.Contain(720989, 1))
                        {
                            client.Inventory.Remove(720987, 1, stream);
                            client.Inventory.Remove(720988, 1, stream);
                            client.Inventory.Remove(720989, 1, stream);
                            client.Inventory.Add(stream, 711452);
                        }
                        break;
                    }
                case 720962:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(711401, 1))
                        {
                            client.CreateBoxDialog("You`ve~collected~enough~Desert~Herbs.");
                            break;
                        }
                        client.Inventory.Add(stream, 711401);
                        client.CreateBoxDialog("You`ve~received~a~Desert~Herb!");
                        break;
                    }
                case 720974:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 711348);
                        client.CreateBoxDialog("You`ve~collect~a~bucket~of~water.~Hurry~to~deliver~it~to~Han~Cheng.");
                        break;
                    }
                case 720980:
                    {
                        if (client.Player.Map == 1000)
                        {
                            if (Role.Core.GetDistance(588, 418, client.Player.X, client.Player.Y) <= 18)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1468, 1))
                                {
                                    client.CreateBoxDialog("You`ve~set~a~fire~here.~Now~go~to~the~next~spot.");
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "fire4");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1468, 1);
                                    client.CreateBoxDialog("The~Hill~Monsters~got~scared~and~ran~away!");

                                }
                            }
                            if (Role.Core.GetDistance(549, 398, client.Player.X, client.Player.Y) <= 18)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1468, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~set~a~fire~here.~Now~go~to~the~next~spot.");
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "fire4");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1468, 0, 1);
                                    client.CreateBoxDialog("The~Hill~Monsters~got~scared~and~ran~away!");

                                }
                            }
                            if (Role.Core.GetDistance(501, 383, client.Player.X, client.Player.Y) <= 18)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(1468, 0, 0, 1))
                                {
                                    client.CreateBoxDialog("You`ve~set~a~fire~here.~Now~go~to~the~next~spot.");
                                }
                                else
                                {
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "fire4");
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1468, 0, 0, 1);
                                    client.CreateBoxDialog("The~Hill~Monsters~got~scared~and~ran~away!");

                                }
                            }
                        }

                        break;
                    }

                case 3008993:
                    {
                        if (DateTime.Now > client.Player.TowerAmuletStamp.AddSeconds(5))
                        {

                            client.Player.TowerAmuletStamp = DateTime.Now;
                            byte rand = (byte)Program.GetRandom.Next(0, 5);
                            switch (rand)
                            {
                                case 0:
                                    client.Teleport(43, 340, 3998);
                                    break;
                                case 1:
                                    client.Teleport(57, 347, 3998);
                                    break;
                                case 2:
                                    client.Teleport(54, 365, 3998);
                                    break;
                                case 3:
                                    client.Teleport(25, 353, 3998);
                                    break;
                                default:
                                    client.Teleport(33, 364, 3998);
                                    break;
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        }
                        else
                        {
                            client.SendSysMesage("After you use a Hunting Amulet, you need to wait 5 secounds to use another one.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3008733:
                    {
                        MsgTaskReward.OpenWheel(client, stream, 3008733, 3651);
                        break;
                    }
                case 3008741:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3642);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        break;
                    }




                case 721203:
                    {
                        if (client.Player.Level > 50)
                        {
                            if (client.Inventory.HaveSpace(4))
                            {
                                client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);



                                client.Inventory.Add(stream, 143009, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 136009, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 610019, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 610019, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received some gifts specially for your class.");

                            }
                            else
                            {

                                client.CreateBoxDialog("Please make 4 more spaces in your inventory.");

                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("You must be atleast level 50 to let you know my secrets.");

                        }
                        break;
                    }
                case 721712:
                    {

                        break;
                    }
                case 3200000:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            client.CreateBoxDialog("Please make 3 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3007108);
                        client.Inventory.Add(stream, 3200334);
                        client.Inventory.Add(stream, 729549);
                        client.Inventory.AddItemWitchStack(3002926, 0, 3, stream);
                        break;
                    }
                case 729549:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            client.CreateBoxDialog("Please make 3 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(1003010, 0, 20, stream);
                        break;
                    }
                case 3200004:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");
                            break;
                        }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.KingDomDeeds += 20;
                        client.Player.UpdateKingdomTreasury(20);

                        client.Inventory.Add(stream, 3600025);
                        client.Inventory.Add(stream, 3600024);
                        client.Inventory.Add(stream, 3600023);

                        break;
                    }
                case 3004818:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 2100025, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.CreateBoxDialog("You~opened~the~pack~and~received~1~Miraculous~Gourd~(B).");
                        break;
                    }
                case 728674:
                    {
                        client.Inventory.Remove(728674, 1, stream);
                        if (Role.Core.Rate(1, 200))
                        {
                            var PurificationArray = Database.ItemType.PurificationItems[6];
                            var Position = Program.GetRandom.Next(0, PurificationArray.Values.Count);
                            var ReceiveItem = PurificationArray.Values.ToArray()[Position];
                            client.Inventory.Add(stream, ReceiveItem.ID);
                            client.CreateBoxDialog("You received a " + ReceiveItem.Name + ".");
                        }
                        else if (Role.Core.Rate(2, 199))
                        {
                            var PurificationArray = Database.ItemType.PurificationItems[5];
                            var Position = Program.GetRandom.Next(0, PurificationArray.Values.Count);
                            var ReceiveItem = PurificationArray.Values.ToArray()[Position];
                            client.Inventory.Add(stream, ReceiveItem.ID);
                            client.CreateBoxDialog("You received a " + ReceiveItem.Name + ".");
                        }
                        else if (Role.Core.Rate(1, 30))
                        {
                            var PurificationArray = Database.ItemType.PurificationItems[4];
                            var Position = Program.GetRandom.Next(0, PurificationArray.Values.Count);
                            var ReceiveItem = PurificationArray.Values.ToArray()[Position];
                            client.Inventory.Add(stream, ReceiveItem.ID);
                            client.CreateBoxDialog("You received a " + ReceiveItem.Name + ".");
                        }
                        else
                        {
                            var PurificationArray = Database.ItemType.PurificationItems[3];
                            var Position = Program.GetRandom.Next(0, PurificationArray.Values.Count);
                            var ReceiveItem = PurificationArray.Values.ToArray()[Position];
                            client.Inventory.Add(stream, ReceiveItem.ID);
                            client.CreateBoxDialog("You received a " + ReceiveItem.Name + ".");
                        }
                        break;
                    }
                case 3300179:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        client.CreateBoxDialog("You've received 10 SpiritofLegend!");
                        client.Inventory.AddItemWitchStack(3300056, 0, 10, stream);
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        break;
                    }
                case 3300056:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 289, 163, (uint)MsgNpc.NpcID.IdealDesgner);
                        break;
                    }
                case 3007106:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3007106, 99))
                        {
                            client.Inventory.Remove(3007106, 99, stream);

                            var Accessorys = Database.ItemType.Accessorys.Values.ToArray();
                            int position = Program.GetRandom.Next(0, Accessorys.Length);
                            var accesory = Accessorys[position];
                            client.Inventory.Add(stream, accesory.ID, 1, 0, 1);
                            client.CreateBoxDialog("You received a 1%Blessed " + accesory.Name + ".");

                        }
                        else
                        {
                            client.SendSysMesage("You should have 99 Accesory Fragments to combine them into a Accesory.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3007385:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more spaces in your inventory.");
                            break;
                        }

                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                        byte rrand = (byte)Program.GetRandom.Next(1, 4);
                        uint ItemId = 0;
                        switch (rrand)
                        {
                            case 1:
                                {
                                    ItemId = 3007104;
                                    break;
                                }
                            case 2:
                                {
                                    ItemId = 3007105;
                                    break;
                                }
                            default:
                                {
                                    ItemId = 3007106;
                                    break;
                                }

                        }


                        string Item2Name = "";
                        byte rand = (byte)Program.GetRandom.Next(1, 100);
                        if (rand < 40)
                        {
                            Database.ItemType.DBItem _item;
                            while (true)
                            {
                                var array = Database.ItemType.Garments.Values.ToArray();
                                _item = array[Program.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DbGarment;
                                if (Database.CoatStorage.Garments.TryGetValue(_item.ID, out DbGarment))
                                {
                                    if (DbGarment.Stars < 4)
                                    {
                                        client.Inventory.Add(stream, _item.ID, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, ".", 3);
                                        Item2Name = _item.Name;
                                        break;
                                    }
                                }
                            }

                        }
                        else
                        {
                            Database.ItemType.DBItem _item;
                            while (true)
                            {
                                var array = Database.ItemType.SteedMounts.Values.ToArray();
                                _item = array[Program.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DBMount;
                                if (Database.CoatStorage.Mounts.TryGetValue(_item.ID, out DBMount))
                                {
                                    if (DBMount.Stars < 4)
                                    {
                                        client.Inventory.Add(stream, _item.ID, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, ".", 3);
                                        Item2Name = _item.Name;
                                        break;
                                    }
                                }
                            }
                        }
                        Database.ItemType.DBItem _DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemId, out _DBItem))
                        {
                            client.Inventory.Add(stream, ItemId);
                            client.SendSysMesage("You received a " + _DBItem.Name + " and 1%Blessed " + Item2Name + "(B).", MsgMessage.ChatMode.System);
                        }

                        break;
                    }
                case 3008101:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Player.OpenHousePack < 3)
                        {
                            client.Player.OpenHousePack += 1;
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.AddItemWitchStack(3008100, 0, 1, stream, false);
                            client.SendSysMesage("You received a Class 6 House Pack!", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already opened such a pack for 3 times today. Please retry tomorrow.", MsgMessage.ChatMode.System);

                        break;
                    }
                case 721799:
                    {
                        if (Role.Core.GetDistance(55, 109, client.Player.X, client.Player.Y) <= 10 && client.Player.Map == 1792)
                        {
                            client.Inventory.Remove(721799, 1, stream);
                            Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                            np.UID = 4584;
                            np.NpcType = Role.Flags.NpcType.Talker;
                            np.Mesh = 3920;
                            np.Map = 1792;
                            np.X = 55;
                            np.Y = 109;
                            Database.Server.ServerMaps[np.Map].AddNpc(np);
                            client.Player.View.Role(false);
                            client.Player.AddMapEffect(stream, 55, 109, "eddy");
                        }
                        else
                            client.SendSysMesage("Please burn the Incense in Swan Lake (55,109).");
                        break;
                    }
                case 3300054:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3300054, 20))
                        {
                            client.Inventory.Remove(3300054, 20, stream);
                            client.Inventory.Add(stream, 2100205, 1);
                            client.CreateBoxDialog("You've received a GoldenPhoenixCup!");
                        }
                        else
                        {
                            client.SendSysMesage("You should have 20 Gold Phoenix Cup Fragments before you can make a complete cup.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3300055:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3300055, 30))
                        {
                            client.Inventory.Remove(3300055, 30, stream);
                            client.Inventory.Add(stream, 2100245, 1);
                            client.CreateBoxDialog("You've received a HolyPhoenixCup!");
                        }
                        else
                        {
                            client.SendSysMesage("You should have 30 Holy Phoenix Cup Fragments before you can make a complete cup.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3600027:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        byte Level = 6;
                        if (Role.Core.Rate(8))
                            Level = 7;
                        if (Program.GetRandom.Next() % 100 < 50)
                        {
                            var PurificationArray = Database.ItemType.PurificationItems[Level];
                            var Position = Program.GetRandom.Next(0, PurificationArray.Values.Count);
                            var ReceiveItem = PurificationArray.Values.ToArray()[Position];
                            client.Inventory.Add(stream, ReceiveItem.ID);
                            client.CreateBoxDialog("You received a " + ReceiveItem.Name + ".");
                        }
                        else
                        {
                            var RefinaryArray = Database.ItemType.Refinary[(byte)(Level - 1)];
                            var Position = Program.GetRandom.Next(0, RefinaryArray.Values.Count);
                            var ReceiveItem = RefinaryArray.Values.ToArray()[Position];
                            client.Inventory.Add(stream, ReceiveItem.ItemID);
                            client.CreateBoxDialog("You received a " + ReceiveItem.Name + ".");
                        }
                        break;
                    }
                case 3008058:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        var RefinaryArray = Database.ItemType.Refinary[6];
                        var Position = Program.GetRandom.Next(0, RefinaryArray.Values.Count);
                        var ReceiveItem = RefinaryArray.Values.ToArray()[Position];
                        client.Inventory.Add(stream, ReceiveItem.ItemID);
                        client.CreateBoxDialog("You've received a " + ReceiveItem.Name + ".");
                        break;
                    }
                case 3005368:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Inventory.Add(stream, 188915, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.Inventory.Add(stream, 200517, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                            client.SendSysMesage("Congratulations!~You~received~a~Gold~Cloth~(Saint)~and~a~King~of~Scorpions~(Saint)!");
                            Role.Core.SendGlobalMessage(stream, "Congratulations!~" + client.Player.Name + "~received~a~Gold~Cloth~(Saint)~and~a~King~of~Scorpions~(Saint)!", MsgMessage.ChatMode.System);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                        }
                        else
                        {
                            client.SendSysMesage("Your~inventory~is~full.");
                        }
                        break;
                    }
                // ---- First Credit Pack contents (descriptions from the client itemtype) ----
                case 3301830: // FancyMeteorScrollPack: 3 Meteor Scrolls (B)
                    {
                        if (!client.Inventory.HaveSpace(3)) { client.CreateBoxDialog("Please make 3 more spaces in your inventory."); break; }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 720027, 3, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.CreateBoxDialog("You received 3 Meteor Scrolls (B).");
                        break;
                    }
                case 3300403: // BigEXPBallPack(10): 10 EXP Balls (B)
                    {
                        if (!client.Inventory.HaveSpace(10)) { client.CreateBoxDialog("Please make 10 more spaces in your inventory."); break; }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, Database.ItemType.ExpBall, 10, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.CreateBoxDialog("You received 10 EXP Balls (B).");
                        break;
                    }
                case 3300241: // RollingCashPack: 200,000 Silver
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.Money += 200000;
                        client.CreateBoxDialog("You received 200,000 Silver.");
                        break;
                    }
                case 3301828: // DelicateBrightStarPack: 5 2-day Bright Star Stones
                    {
                        if (!client.Inventory.HaveSpace(5)) { client.CreateBoxDialog("Please make 5 more spaces in your inventory."); break; }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 3009001, 5, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 2);
                        client.CreateBoxDialog("You received 5 Bright Star Stones (2 days).");
                        break;
                    }
                case 3300242: // GeniusStudyPack: 1,000 Study Points
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.SubClass.AddStudyPoints(client, 1000, stream);
                        break;
                    }
                case 3300366: // 1000CPs(B)Pack: 1,000 CPs (B)
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.BoundConquerPoints += 1000;
                        client.CreateBoxDialog("You received 1,000 CPs (B).");
                        break;
                    }
                case 3301829: // IvoryRobePack: 15-day 1% blessed Ivory Robe (B)
                    {
                        if (!client.Inventory.HaveSpace(1)) { client.CreateBoxDialog("Please make 1 more space in your inventory."); break; }
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 188175, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, false, "", 15);
                        client.CreateBoxDialog("You received an Ivory Robe (B) for 15 days.");
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
