using Conquer6609.Database;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgLoader;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.AttackHandler;
using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Client
{
    public partial class PoolProcesses
    {
        public static unsafe void CheckSecounds(Client.GameClient client)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || client.Player.CompleteLogin == false)
                    return;
                if (client.Player.Map == 10166 || client.Player.Map == 5051 || client.Player.Map == 1005 || client.Player.Map == 1860 || client.Player.Map == 1858 || client.Player.Map == 700 || client.Player.Map == 1004 || Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                    }
                }
                // Horse Racing: only signed-up riders on a steed may stay in a race map
                if (Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map))
                    Game.MsgTournaments.MsgSchedules.SteedRace.CheckRider(client);
                Extensions.Time32 timer = Extensions.Time32.Now;
                if (client.Player.ConquerPointDropLimitLayer == 0)
                {
                    client.Player.ConquerPointDropLimitLayer = 1;
                }
                if (client.Player.ConquerPointDropLimitLayer > 1)
                {
                    if (DateTime.Now >= client.Player.ConquerPointsDropStamp)
                     {
                        client.Player.ConquerPointDropLimitLayer = 1;
                        client.Player.ConquerPointsDropStamp = DateTime.FromBinary(0);
                    }
                }
                CheckJiangHu(client);
                //if (client.Player.Merchant == 1 && client.Player.MerchantApplicationEnd <= DateTime.Now)
                //{
                //    client.Player.Merchant = 255;
                //}
                if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                {
                    if (timer > client.Player.WindWalkerEffect.AddSeconds(7))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "TSM_SXJ_HPhf");
                            client.Player.WindWalkerEffect = Extensions.Time32.Now;
                        }
                    }
                }
                if (client.Player.Map == 601)
                {
                    if (!client.Map.ValidLocation(client.Player.X, client.Player.Y))
                    {
                        client.Teleport(64, 56, 601);
                    }
                }
                if (client.Player.Map == 44463)
                {
                    if (timer > client.Player.EarthStamp.AddSeconds(10))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                            effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeLeftRight;
                            effect.DropType = MsgDropID.Earth;
                            effect.m_X = client.Player.X;
                            effect.m_Y = client.Player.Y;
                            client.Send(stream.ItemPacketCreate(effect));
                        }
                        client.Player.EarthStamp = Extensions.Time32.Now;
                    }
                }
                if (client.Player.Map == 1700)
                {
                    if (client.Player.OnAutoHunt)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(stream.AutoHuntCreate(3, 0));
                            client.Player.OnAutoHunt = false;
                        }
                    }
                }
                if (client.Player.ExpProtection > 0)
                    client.Player.ExpProtection -= 1;
                if (client.Player.WaveofBlood)
                {
                    if (DateTime.Now > client.Player.WaveofBloodStamp.AddSeconds(8))
                    {
                        client.Player.WaveofBlood = false;
                        client.Player.XPCount += 1;//15
                    }
                }
                if (DateTime.Now > client.Player.ExpireVip)
                {
                    if (client.Player.VipLevel > 1)
                    {
                        client.Player.VipLevel = 0;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendUpdate(stream, client.Player.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);

                            client.Player.UpdateVip(stream);
                        }
                    }
                }
                if (client.Player.Map == 1768)
                {
                    if (client.Player.QuestGUI.CheckQuest(1785, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                    {
                        if (DateTime.Now > client.Player.TaskQuestTimer)
                        {
                            client.SendSysMesage("You fainted and woke up find you are in Ape City. You need to try more toxins to go Kun Lun.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Inventory.Remove(729983, 1, stream);
                                client.Inventory.Remove(729984, 1, stream);
                                client.Inventory.Remove(729985, 1, stream);
                                client.Inventory.Remove(729986, 1, stream);
                                client.Inventory.Remove(729987, 1, stream);
                                client.Inventory.Remove(729988, 1, stream);
                                client.Inventory.Remove(729989, 1, stream);
                            }
                            client.Teleport(55, 55, 1004);
                        }
                    }
                }

                if (client.Player.Map == 1011)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 471, 844) <= 5 && client.Inventory.Contain(721799, 1))
                    {
                        client.Teleport(80, 39, 1792);

                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 643, 622) < 18)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1133, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1133, 1);
                            }
                        }
                    }

                }
                var currentTournament = Game.MsgTournaments.MsgSchedules.CurrentTournament;
                if (currentTournament.Type == Game.MsgTournaments.TournamentType.FootBall)
                {
                    if (currentTournament.Process == Game.MsgTournaments.ProcesType.Alive)
                    {
                        ((Game.MsgTournaments.MsgFootball)currentTournament).CheckNaked(client);
                    }
                }
                if (client.Player.Map == 44457)
                {

                    if (Role.Core.GetDistance(128, 36, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("You followed the track of the Elder Power and found him on a road leading to the tomb tunnel. Hit the road now?",
                                  new Action<Client.GameClient>(p =>
                                  {
                                      p.Teleport(134, 93, 10089);
                                      p.Player.QuestGUI.SendAutoPatcher("There are too many devil claws here. Try your best to break through and find the Elder Power.", 10089, 28, 89, 0);
                                      p.SendSysMesage("There are too many devil claws here. Try your best to break through and find the Elder Power.");
                                  }), null);
                    }
                }
                else if (client.Player.Map == 10090)
                {
                    if (Role.Core.GetDistance(361, 314, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("The Elder Power is seriously injured. Are you sure you want to leave him alone?",
                             new Action<Client.GameClient>(p => p.Teleport(334, 625, 1002)), null);
                    }
                }
                else if (client.Player.Map == 44460)
                {
                    if (Role.Core.GetDistance(361, 314, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("The Elder Power is seriously injured. Are you sure you want to leave him alone?",
                             new Action<Client.GameClient>(p => p.Teleport(334, 625, 1002)), null);
                    }
                }
                else if (client.Player.Map == 44461)
                {
                    if (Role.Core.GetDistance(361, 314, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("The Elder Power is seriously injured. Are you sure you want to leave him alone?",
                             new Action<Client.GameClient>(p => p.Teleport(334, 625, 1002)), null);
                    }
                }
                else if (client.Player.Map == 44462)
                {
                    if (Role.Core.GetDistance(361, 314, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("The Elder Power is seriously injured. Are you sure you want to leave him alone?",
                             new Action<Client.GameClient>(p => p.Teleport(334, 625, 1002)), null);
                    }
                }
                else if (client.Player.Map == 44463)
                {
                    if (Role.Core.GetDistance(361, 314, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("The Elder Power is seriously injured. Are you sure you want to leave him alone?",
                             new Action<Client.GameClient>(p => p.Teleport(334, 625, 1002)), null);
                    }
                    if (Role.Core.GetDistance(58, 164, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("You`re very close to the tunnel exit. Exit the tunnel now?",
                             new Action<Client.GameClient>(p =>
                             {
                                 p.Teleport(253, 406, 1002);
                                 p.Player.QuestGUI.FinishQuest(3801);
                                 var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)20032, p.Player.Class, 3802);
                                 p.Player.QuestGUI.Accept(ActiveQuest, 0);
                                 p.Player.QuestGUI.SendAutoPatcher("You`ve successfully escaped before the mausoleum completely collapsed. Hurry and report back to the Windwalker Lord in Twin City.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                 unsafe
                                 {
                                     using (var rec = new ServerSockets.RecycledPacket())
                                     {
                                         var pstream = rec.GetStream();
                                         var action = new ActionQuery()
                                         {
                                             ObjId = client.Player.UID,
                                             Type = ActionType.DrawStory,
                                             dwParam = 1011

                                         };
                                         p.Send(pstream.ActionCreate(&action));
                                     }
                                 }


                             }), null);
                    }
                    foreach (var item in client.Player.View.Roles(Role.MapObjectType.Item))
                    {
                        var RoleItem = item as Game.MsgFloorItem.MsgItem;
                        if (RoleItem.MsgFloor.m_ID == 1616)
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, RoleItem.X, RoleItem.Y) <= 7)
                            {
                                if (Role.Core.Rate(50))
                                {
                                    client.CreateBoxDialog("You carelessly stepped into a sand pit and can move very slow.");
                                    client.Player.AddFlag(MsgUpdate.Flags.Deceleration, 2, true, 0, 40, 39);
                                }
                                else
                                {
                                    client.CreateBoxDialog("You can hardly move forward in the intensive quake.");
                                    client.Player.AddFlag(MsgUpdate.Flags.Deceleration, 2, true, 0, 40, 46);
                                }
                            }
                        }
                    }
                }
                else if (client.Player.Map == 10089)
                {
                    if (Role.Core.GetDistance(136, 89, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("The tomb tunnel is in front of you and the Elder Power must be there now. Enter the tunnel now?",
                             new Action<Client.GameClient>(p => p.Teleport(334, 625, 1002)), null);
                    }
                    if (Role.Core.GetDistance(28, 89, client.Player.X, client.Player.Y) <= 2)
                    {


                        client.Player.MessageBox("You haven`t found the Elder Power. Are you sure you want to return to Wind Plain now?",
                                new Action<Client.GameClient>(p =>
                                {
                                    p.Teleport(359, 312, 10090);
                                    p.Player.QuestGUI.SendAutoPatcher("You found the Elder Power sitting in the tomb tunnel. It seems he has been seriously injured.", 10090, 350, 285, (uint)Game.MsgNpc.NpcID.ElderPower2);
                                    p.SendSysMesage("You found the Elder Power sitting in the tomb tunnel. It seems he has been seriously injured.");
                                }), null);
                    }
                    foreach (var item in client.Player.View.Roles(Role.MapObjectType.Item))
                    {
                        var RoleItem = item as Game.MsgFloorItem.MsgItem;
                        if (RoleItem.MsgFloor.m_ID == 24)
                            continue;
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, RoleItem.X, RoleItem.Y) <= 7)
                        {
                            client.Player.AddFlag(MsgUpdate.Flags.Deceleration, 2, true, 0, 40, 39);
                        }
                    }
                }
                else if (client.Player.Map == 10088 || client.Player.Map == 44457
                    || client.Player.Map == 44456 || client.Player.Map == 44455)
                {
                    if (Role.Core.GetDistance(192, 151, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Player.MessageBox("Your school camp was suddenly attacked by the devil force and is in critical condition. Are you sure you want to leave now?",
                                  new Action<Client.GameClient>(p => p.Teleport(334, 625, 1002)), null);
                    }
                }
                else if (client.Player.Map == 1787)
                {
                    if (Role.Core.GetDistance(83, 75, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Teleport(126, 73, 1786);
                        client.CreateBoxDialog("You arrived at Dungeon 2F.");
                    }
                }
                else if (client.Player.Map == 1786)
                {
                    if (Role.Core.GetDistance(122, 67, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Teleport(105, 108, 1785);
                        client.CreateBoxDialog("You arrived at Dungeon 1F.");
                    }
                    else if (Role.Core.GetDistance(43, 65, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Teleport(87, 74, 1787);
                        client.CreateBoxDialog("You arrived at Dungeon 3F.");
                    }
                }
                else if (client.Player.Map == 1785)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 74, 49) <= 2)
                    {
                        if (client.Inventory.Contain(721784, 1))
                        {
                            if (client.Inventory.Contain(721788, 1))
                            {
                                client.Teleport(126, 73, 1786);
                                client.CreateBoxDialog("You arrived at Dungeon 2F.");
                            }
                            else
                            {
                                client.CreateBoxDialog("You still need to collect the Glitter Sword and the Annatto Blade. You can get them from the Mausoleum General.");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("You still need to collect the Glitter Sword and the Annatto Blade. You can get them from the Mausoleum General.");
                        }
                    }
                }
                else if (client.Player.Map == 1783)
                {
                    if (client.Player.QuestGUI.CheckQuest(526, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                    {
                        foreach (var item in client.Player.View.Roles(Role.MapObjectType.Item))
                        {
                            var RoleItem = item as Game.MsgFloorItem.MsgItem;
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, RoleItem.X, RoleItem.Y) <= 2)
                            {
                                if (RoleItem.MsgFloor.m_ID == 11)
                                {
                                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Poisoned))
                                    {
                                        client.CreateBoxDialog("Oops! You are caught in a trap and were seriously poisoned.");
                                        client.Player.AddFlag(MsgUpdate.Flags.Poisoned, 60, true, 3);
                                    }
                                    else
                                        client.CreateBoxDialog("The poison in the trap doesn`t have any impact on you, as you have already been poisoned.");
                                }
                                else if (RoleItem.MsgFloor.m_ID == 18)
                                {
                                    if (client.Player.QuestGUI.CheckObjectives(526, 20))
                                    {
                                        if (client.Inventory.HaveSpace(1))
                                        {
                                            client.Teleport(94, 333, 1001);
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var msg = rec.GetStream();
                                                client.Inventory.Add(msg, 721786, 1);
                                            }
                                            client.CreateBoxDialog("You got a Weird Invocation from nowhere. Ghoul Kong (85,313), at the entrance to the Dungeon, may be of some help.");

                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("Your inventory is full. You can`t take the Invocation.");
                                        }
                                    }
                                    else
                                        client.CreateBoxDialog("You need to kill 20 Vicious Rats.");
                                }
                            }
                        }
                    }
                }

                else if (client.Player.Map == 1001)
                {

                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 88, 279) <= 3)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1829, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721876, 1))
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var msg = rec.GetStream();
                                        client.Inventory.Remove(721876, 1, msg);
                                        if (Role.Core.Rate(30))
                                        {
                                            client.Player.QuestGUI.SetQuestObjectives(msg, 1829, 1);
                                            client.Inventory.Add(msg, 721870);
                                            client.CreateBoxDialog("You used the key and unlocked the compartment. There it is, the Scripture Box.");
                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("Oh, the compartment clicked once but the Key disappeared. You`d better go kill some more Tomb Bats and get another key.");
                                        }
                                        if (client.Inventory.Contain(721876, 1) == false)
                                        {
                                            client.Player.QuestGUI.SetQuestObjectives(msg, 1829, 0);
                                        }
                                    }
                                }
                                else
                                    client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            }
                            else
                                client.CreateBoxDialog("A key for a compartment came out of the Tomb Bat`s body. You need the key to unlock the compartment.");
                        }
                    }
                }

                if (client.Player.Map == 4000)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 40, 66) <= 2)
                        client.Player.MessageBox("Do you want to leave the Tower of Mystery?.",
                                  new Action<Client.GameClient>(p => p.Teleport(83, 74, 4020)), null);
                }
                else if (client.Player.Map == 4003)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 42, 64) <= 2)
                        client.Player.MessageBox("Do you want to leave the Tower of Mystery?.",
                                  new Action<Client.GameClient>(p => p.Teleport(83, 74, 4020)), null);
                }
                else if (client.Player.Map == 4006)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 44, 62) <= 2)
                        client.Player.MessageBox("Do you want to leave the Tower of Mystery?.",
                                  new Action<Client.GameClient>(p => p.Teleport(83, 74, 4020)), null);
                }
                else if (client.Player.Map == 4008)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 46, 68) <= 2)
                        client.Player.MessageBox("Do you want to leave the Tower of Mystery?.",
                                  new Action<Client.GameClient>(p => p.Teleport(83, 74, 4020)), null);
                }
                else if (client.Player.Map == 4009)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 46, 44) <= 2)
                        client.Player.MessageBox("Do you want to leave the Tower of Mystery?.",
                                  new Action<Client.GameClient>(p => p.Teleport(83, 74, 4020)), null);
                }

                if (client.Player.Map == 4020)
                {
                    if (Role.Core.GetDistance(73, 98, client.Player.X, client.Player.Y) <= 2)
                    {
                        client.Teleport(78, 349, 3998);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var msg = rec.GetStream();
                            client.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "movego");
                        }
                    }
                }
                if (client.Player.Map == 3998)
                {
                    if (client.Player.QuestGUI.CheckQuest(3641, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                    {

                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 220, 294) <= 3)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.ChingYan, client.Player.Class, 3641);
                                client.Inventory.Remove(3200344, 1, msg);
                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 3641, 0, 1);
                                client.Player.QuestGUI.SendAutoPatcher("You appease the sacrificed Bright people.Hurry and claim the reward!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                            }
                        }
                    }
                    if (client.Player.X <= 110)
                    {
                        foreach (var item in client.Player.View.Roles(Role.MapObjectType.Item))
                        {
                            if (Role.Core.GetDistance(item.X, item.Y, client.Player.X, client.Player.Y) <= 2)
                            {
                                client.Map.RemoveTrap(item.X, item.Y, item);
                                if (client.Inventory.Contain(3008993, 1))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var msg = rec.GetStream();
                                        client.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "accession3");
                                        if (client.Inventory.HaveSpace(1))
                                        {
                                            if (Role.Core.Rate(60))
                                            {
                                                client.Inventory.Remove(3008993, 1, msg);
                                                client.Inventory.AddItemWitchStack(3008992, 0, 1, msg);
                                                client.CreateBoxDialog("The earth was split apart, with a flash of golden light burst out, and you received a Treasure of Dragon.");
                                            }
                                            else
                                            {
                                                client.CreateBoxDialog("The earth was split apart, but you got nothing inside. Go and check another spot.");
                                            }

                                        }
                                        else
                                        {
                                            client.CreateBoxDialog("You need to make some room in your inventory before you can continue the adventure.");
                                        }
                                    }
                                }
                                else
                                {


                                    client.Player.MessageBox("You felt something strange under the ground. Maybe, the Chief`s Hunting Amulet can clear your confusion.",
                                       new Action<Client.GameClient>(p =>
                                       {
                                           p.Teleport(78, 349, 3998);
                                           using (var rec = new ServerSockets.RecycledPacket())
                                           {
                                               var pstream = rec.GetStream();
                                               client.Player.SendString(pstream, MsgStringPacket.StringID.Effect, true, "moveback");
                                           }
                                       }), null);
                                }
                                break;
                            }

                        }
                    }
                }

                if (client.Player.Map == 1015)
                {


                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 362, 401) <= 1)
                    {
                        if (client.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(522, 3))
                            {
                                client.Teleport(63, 102, 1784);
                                client.CreateBoxDialog("You arrived at the Hut.");
                                client.Player.QuestGUI.FinishQuest(522);
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.TimeDoor, client.Player.Class, 523);
                                client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            }
                            else
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var msg = rec.GetStream();
                                    client.CreateDialog(msg, "You need to break open all of the stone doors. ALL of them!", "I~heard~you~the~first~time.");
                                }
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            client.Teleport(63, 102, 1784);
                            client.CreateBoxDialog("You arrived at the Hut.");
                        }
                        else
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.CreateDialog(msg, "You need to break open all of the stone doors. ALL of them!", "I~heard~you~the~first~time.");
                            }
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 365, 393) <= 1)
                    {
                        if (client.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(522, 3))
                            {
                                client.Teleport(63, 102, 1784);
                                client.CreateBoxDialog("You arrived at the Hut.");
                                client.Player.QuestGUI.FinishQuest(522);
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.TimeDoor, client.Player.Class, 523);
                                client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            }
                            else
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var msg = rec.GetStream();
                                    client.CreateDialog(msg, "You need to break open all of the stone doors. ALL of them!", "I~heard~you~the~first~time.");
                                }
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            client.Teleport(63, 102, 1784);
                            client.CreateBoxDialog("You arrived at the Hut.");
                        }
                        else
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.CreateDialog(msg, "You need to break open all of the stone doors. ALL of them!", "I~heard~you~the~first~time.");
                            }
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 351, 407) <= 1)
                    {
                        if (client.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(522, 3))
                            {
                                client.Teleport(63, 102, 1784);
                                client.CreateBoxDialog("You arrived at the Hut.");
                                client.Player.QuestGUI.FinishQuest(522);
                                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.TimeDoor, client.Player.Class, 523);
                                client.Player.QuestGUI.Accept(ActiveQuest, 0);

                            }
                            else
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var msg = rec.GetStream();
                                    client.CreateDialog(msg, "You need to break open all of the stone doors. ALL of them!", "I~heard~you~the~first~time.");
                                }
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(522, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            client.Teleport(63, 102, 1784);
                            client.CreateBoxDialog("You arrived at the Hut.");
                        }
                        else
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.CreateDialog(msg, "You need to break open all of the stone doors. ALL of them!", "I~heard~you~the~first~time.");
                            }
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 450, 720) <= 3)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!(DateTime.Now > client.Player.GallbladerrStamp.AddSeconds(30)))
                                if (client.Inventory.Contain(721909, 1))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var msg = rec.GetStream();
                                        client.Inventory.Remove(721909, 1, msg);
                                        if (Role.Core.Rate(20))
                                        {
                                            client.Inventory.Add(msg, 721908);
                                            client.CreateBoxDialog("You've used the Gallbladder and find a Bitter Ginseng!");
                                        }
                                        else
                                            client.CreateBoxDialog("You've used the Gallbladder but didn`t find a Bitter Ginseng!");
                                    }
                                }
                                else
                                    client.CreateBoxDialog("You don`t have a Gallbladder");
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 296, 290) <= 3)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!(DateTime.Now > client.Player.GallbladerrStamp.AddSeconds(30)))
                                if (client.Inventory.Contain(721909, 1))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var msg = rec.GetStream();
                                        client.Inventory.Remove(721909, 1, msg);
                                        if (Role.Core.Rate(20))
                                        {
                                            client.Inventory.Add(msg, 721908);
                                            client.CreateBoxDialog("You've used the Gallbladder and find a Bitter Ginseng!");
                                        }
                                        else
                                            client.CreateBoxDialog("You've used the Gallbladder but didn`t find a Bitter Ginseng!");
                                    }
                                }
                                else
                                    client.CreateBoxDialog("You don`t have a Gallbladder");
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 255, 228) <= 3)
                    {
                        if (!(DateTime.Now > client.Player.GallbladerrStamp.AddSeconds(30)))
                            if (client.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (client.Inventory.Contain(721909, 1))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var msg = rec.GetStream();
                                        client.Inventory.Remove(721909, 1, msg);
                                        if (Role.Core.Rate(20))
                                        {
                                            client.Inventory.Add(msg, 721908);
                                            client.CreateBoxDialog("You've used the Gallbladder and find a Bitter Ginseng!");
                                        }
                                        else
                                            client.CreateBoxDialog("You've used the Gallbladder but didn`t find a Bitter Ginseng!");
                                    }
                                }
                                else
                                    client.CreateBoxDialog("You don`t have a Gallbladder");
                            }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 243, 193) <= 3)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!(DateTime.Now > client.Player.GallbladerrStamp.AddSeconds(30)))
                                if (client.Inventory.Contain(721909, 1))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var msg = rec.GetStream();
                                        client.Inventory.Remove(721909, 1, msg);
                                        if (Role.Core.Rate(20))
                                        {
                                            client.Inventory.Add(msg, 721908);
                                            client.CreateBoxDialog("You've used the Gallbladder and find a Bitter Ginseng!");
                                        }
                                        else
                                            client.CreateBoxDialog("You've used the Gallbladder but didn`t find a Bitter Ginseng!");
                                    }
                                }
                                else
                                    client.CreateBoxDialog("You don`t have a Gallbladder");
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 229, 142) <= 3)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1813, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!(DateTime.Now > client.Player.GallbladerrStamp.AddSeconds(30)))
                                if (client.Inventory.Contain(721909, 1))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var msg = rec.GetStream();
                                        client.Inventory.Remove(721909, 1, msg);
                                        if (Role.Core.Rate(20))
                                        {
                                            client.Inventory.Add(msg, 721908);
                                            client.CreateBoxDialog("You've used the Gallbladder and find a Bitter Ginseng!");
                                        }
                                        else
                                            client.CreateBoxDialog("You've used the Gallbladder but didn`t find a Bitter Ginseng!");
                                    }
                                }
                                else
                                    client.CreateBoxDialog("You don`t have a Gallbladder");
                        }
                    }

                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 445, 681) <= 4)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1625, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.MessageBox("Are~you~sure~you~want~to~dig~up~treasures,~here?", new Action<Client.GameClient>(user =>
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var msg = rec.GetStream();
                                    if (user.Inventory.HaveSpace(1))
                                        user.Inventory.Add(msg, 711460);
                                    else
                                        user.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }

                            })
                                , null, 99999);
                        }
                    }



                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 774, 526) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1661, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1661, 1);
                            }
                            client.SendSysMesage("There's nothing abnormal, here. You can go spy on the other places.", MsgMessage.ChatMode.System);


                            if (client.Player.QuestGUI.CheckObjectives(1661, 1, 1, 1, 1))
                                client.SendSysMesage("You found that bridge was bady damaged. Hurry to tell the Bird Island Castelian about it!", MsgMessage.ChatMode.System);


                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 774, 606) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1661, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1661, 0, 1);

                            }
                            client.SendSysMesage("There's nothing abnormal, here. You can go spy on the other places.", MsgMessage.ChatMode.System);


                            if (client.Player.QuestGUI.CheckObjectives(1661, 1, 1, 1, 1))
                                client.SendSysMesage("You found that bridge was bady damaged. Hurry to tell the Bird Island Castelian about it!", MsgMessage.ChatMode.System);


                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 693, 606) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1661, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1661, 0, 0, 1);
                            }
                            client.SendSysMesage("There's nothing abnormal, here. You can go spy on the other places.", MsgMessage.ChatMode.System);


                            if (client.Player.QuestGUI.CheckObjectives(1661, 1, 1, 1, 1))
                                client.SendSysMesage("You found that bridge was bady damaged. Hurry to tell the Bird Island Castelian about it!", MsgMessage.ChatMode.System);


                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 693, 519) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1661, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1661, 0, 0, 1);

                            }
                            client.SendSysMesage("There's nothing abnormal, here. You can go spy on the other places.", MsgMessage.ChatMode.System);


                            if (client.Player.QuestGUI.CheckObjectives(1661, 1, 1, 1, 1))
                                client.SendSysMesage("You found that bridge was bady damaged. Hurry to tell the Bird Island Castelian about it!", MsgMessage.ChatMode.System);


                        }
                    }


                }
                if (client.Player.Map == 1020)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 380, 49) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1352, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            client.Player.MessageBox("Do you want to jump off the cliff to prove your courge?", new Action<Client.GameClient>(user => user.Teleport(375, 48, 1011, 0)), null, 99999);


                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 027, 375) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1344, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            client.SendSysMesage("It`s cliff all around.");
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();

                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1344, 1);
                            }
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 451, 463) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1344, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            client.SendSysMesage("It`s cliff all around.");
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();

                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1344, 0, 1);
                            }
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 543, 885) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1344, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            client.SendSysMesage("It`s cliff all around.");
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();

                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1344, 0, 0, 1);
                            }
                        }
                    }

                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 473, 541) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1338, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();
                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1338, 1);
                            }
                            client.Teleport(566, 570, 1020);

                            client.CreateBoxDialog("You~chanted~the~spell!~You~arrived~the~Love~Canyon!");


                        }
                    }
                    //1338
                }
                else if (client.Player.Map == 1000)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 465, 676) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1452, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            client.CreateBoxDialog("There`s~no~problem~here.~Go~inspect~the~next~spot.");
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();

                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1452, 1);
                            }
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 506, 684) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1452, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            client.CreateBoxDialog("There`s~no~problem~here.~Go~inspect~the~next~spot.");
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();

                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1452, 0, 1);
                            }
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 533, 654) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1452, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            client.CreateBoxDialog("There`s~no~problem~here.~Go~inspect~the~next~spot.");
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();

                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1452, 0, 0, 1);
                            }
                        }
                    }
                    else if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 533, 626) <= 2)
                    {
                        if (client.Player.QuestGUI.CheckQuest(1452, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            client.CreateBoxDialog("There`s~no~problem~here.~Go~inspect~the~next~spot.");
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var msg = rec.GetStream();

                                client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1452, 0, 0, 0, 1);
                            }
                        }
                    }
                }






                if (client.Player.StartVote)
                {
                    Database.VoteSystem.CheckUp(client);
                }
              
                if (client.Player.OnDefensePotion)
                {
                    if (timer > client.Player.OnDefensePotionStamp)
                    {
                        client.Player.OnDefensePotion = false;
                    }
                }
                if (client.Player.OnAttackPotion)
                {
                    if (timer > client.Player.OnAttackPotionStamp)
                    {
                        client.Player.OnAttackPotion = false;
                    }
                }
                if (client.Player.ActivePick)
                {
                    if (timer > client.Player.PickStamp)
                    {
                        client.Player.ActivePick = false;

                        if (client.Player.MonkMiseryTransforming == 1)
                        {
                            client.Player.MonkMiseryTransforming = 0;
                            client.Teleport(client.Player.X, client.Player.Y, 3831);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Database.Server.AddMapMonster(stream, client.Map, 7484, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgItemPacket.EffectMonsters.None);

                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower1 || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower6
                                || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower2 || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower5
                                || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower3 || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower4
                                || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower7)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.AddItemWitchStack(3008747, 0, 1, stream);
                                        client.SendSysMesage("You received LavaFlower!", MsgMessage.ChatMode.System);
                                        if (client.Inventory.Contain(3008747, 10))
                                            client.CreateBoxDialog("You`ve collected 10 Lava Flowers. Go and try to extract the Fire Force.");

                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");

                                }
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc >= (uint)Game.MsgNpc.NpcID.WhiteHerb1 && client.ActiveNpc <= (uint)Game.MsgNpc.NpcID.WhiteHerb6)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.AddItemWitchStack(3008741, 0, 1, stream);
                                        client.SendSysMesage("You received WhiteHerb!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");

                                }
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(1653, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc >= 8551 && client.ActiveNpc <= 8555)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.AddItemWitchStack(711478, 0, 1, stream);
                                        client.SendSysMesage("You~received~a~Rainbow~Flower!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");


                                    if (client.OnRemoveNpc != null)
                                    {
                                        client.OnRemoveNpc.Respawn = Extensions.Time32.Now.AddSeconds(10);
                                        client.Map.RemoveNpc(client.OnRemoveNpc, stream);
                                        client.Map.soldierRemains.TryAdd(client.OnRemoveNpc.UID, client.OnRemoveNpc);
                                    }
                                }
                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(6131, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720995, 1))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
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
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.SaltedFish)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.Add(stream, 711479);
                                        client.SendSysMesage("You received a pack of Salted Fish!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(1640, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.SaltedFish)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.Add(stream, 711472);
                                        client.SendSysMesage("You receive the Salted Fish!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.FishingNet)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.Add(stream, 711473);
                                        client.SendSysMesage("You received a Fishing Net!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }

                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(1594, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.WhiteChrysanthemum)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 711441);
                                    client.SendSysMesage("You've got a White Chrysanthemum!", MsgMessage.ChatMode.System);
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.Jasmine)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 711442);
                                    client.SendSysMesage("You've got a Jasmine!", MsgMessage.ChatMode.System);
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.Lily)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 711440);
                                    client.SendSysMesage("You've got a Lily!", MsgMessage.ChatMode.System);
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.WillowLeaf)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 711443);
                                    client.SendSysMesage("You've got a Willow Leaf!", MsgMessage.ChatMode.System);
                                }
                            }


                        }
                        if (client.Player.QuestGUI.CheckQuest(1469, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.st1TreeSeed)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    client.Inventory.Add(stream, 720971);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1469, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1469, 1, 1, 1))
                                        client.CreateBoxDialog("You`ve~collected~enough~seeds.~Go~report~to~Wan~Ying,~right~away.");
                                    else
                                        client.CreateBoxDialog("You`ve~received~a~seed.");
                                }
                            }
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.nd2TreeSeed)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 720971);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1469, 0, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1469, 1, 1, 1))
                                        client.CreateBoxDialog("You`ve~collected~enough~seeds.~Go~report~to~Wan~Ying,~right~away.");
                                    else
                                        client.CreateBoxDialog("You`ve~received~a~seed.");
                                }
                            }
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.rd3TreeSeed)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 720971);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1469, 0, 0, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1469, 1, 1, 1))
                                        client.CreateBoxDialog("You`ve~collected~enough~seeds.~Go~report~to~Wan~Ying,~right~away.");
                                    else
                                        client.CreateBoxDialog("You`ve~received~a~seed.");
                                }
                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(1330, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "allcure5");
                                switch (client.Player.QuestCaptureType)
                                {

                                    case 1:
                                        {
                                            client.SendSysMesage("You captured a Thunder Ape.", MsgMessage.ChatMode.System);

                                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1330, 1);
                                        }
                                        break;
                                    case 2:
                                        {
                                            client.SendSysMesage("You captured a Thunder Ape L58.", MsgMessage.ChatMode.System);

                                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1330, 0, 1);
                                        }
                                        break;

                                }
                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(1317, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.CarpenterJack, client.Player.Class, 1317);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Inventory.AddItemWitchStack(711356, 0, 1, stream);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1317, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "allcure5");
                                client.SendSysMesage("You received 1 Chiff Flower.", MsgMessage.ChatMode.System);

                            }
                            if (client.Player.QuestGUI.CheckObjectives(1317, 20))
                            {
                                client.Player.QuestGUI.SendAutoPatcher("You have collected enough CliffFowers. Send it to Carpenter Jack.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

                            }
                        }

                        else if (client.Player.QuestGUI.CheckQuest(1011, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Contain(711239, 5))
                                {
                                    var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.XuLiang, client.Player.Class, 1011);
                                    client.Player.QuestGUI.SendAutoPatcher("You`ve~picked~5~Peach~Blossoms!~Now~give~them~to~Xu~Liang.", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                                }
                                else
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1011, 1);
                                        client.Inventory.AddItemWitchStack(711239, 0, 1, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "allcure5");
                                        client.SendSysMesage("You picked a Peach Blossom from the Peach Tree!", MsgMessage.ChatMode.System);

                                    }

                                }
                            }
                            else
                            {
                                client.SendSysMesage("Please make 1 more space in your inventory.", MsgMessage.ChatMode.System);

                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(6049, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 6049, 1, 1);

                                if (client.OnRemoveNpc != null)
                                {
                                    Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                    packet.ID = MsgStringPacket.StringID.Effect;
                                    packet.UID = client.OnRemoveNpc.UID;
                                    packet.Strings = new string[1] { "M_Fire1" };
                                    client.Player.View.SendView(stream.StringPacketCreate(packet), true);


                                    client.OnRemoveNpc.Respawn = Extensions.Time32.Now.AddSeconds(10);
                                    client.Map.RemoveNpc(client.OnRemoveNpc, stream);
                                    client.Map.soldierRemains.TryAdd(client.OnRemoveNpc.UID, client.OnRemoveNpc);
                                    //add effect here

                                    Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                                    dialog.AddText("What? You said the Desert Guardian sent you here to find us? Well, I had to play dead to keep the bandits from seeing me. I will avenge my comrades, one day!")
                                  .AddText("~I`ll go back and report this to Desert Guardian! Thanks for coming to find us. I thought we would never be seen again.");
                                    dialog.AddOption("No~Problem.", 255);
                                    dialog.AddAvatar(101).FinalizeDialog();

                                }

                                if (client.Player.QuestGUI.CheckObjectives(6049, 8))
                                {

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.DesertGuardian, client.Player.Class, 6049);
                                    client.Player.QuestGUI.SendAutoPatcher("You~are~too~far~away~from~the~Soldier`s~Remains!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

                                    client.Player.QuestGUI.SendAutoPatcher("You~are~too~far~away~from~the~Soldier`s~Remains!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                }
                                else
                                {
                                    client.CreateBoxDialog("This soldier has died. Release his soul!");
                                }

                                //client.CreateBoxDialog("You~are~too~far~away~from~the~Soldier`s~Remains!");
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(6014, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(client.Player.DailyMagnoliaItemId, 1))
                            {


                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    client.Map.AddMagnolia(stream, client.Player.DailyMagnoliaItemId);
                                    Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                    packet.ID = MsgStringPacket.StringID.Effect;
                                    packet.UID = client.Map.Magnolia.UID;
                                    packet.Strings = new string[1] { "accession1" };
                                    client.Player.View.SendView(stream.StringPacketCreate(packet), true);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                    client.Player.QuestGUI.FinishQuest(6014);
                                    client.Inventory.Remove(client.Player.DailyMagnoliaItemId, 1, stream);
                                    switch (client.Player.DailyMagnoliaItemId)
                                    {
                                        case 729306:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 10, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                                                client.CreateBoxDialog("Congratulations!~You~received~60 minutes of EXP, 10 Study Points and 1 Chi Token.!");

                                                break;
                                            }
                                        case 729307:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 20, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(900, true, Role.Flags.ExperienceEffect.angelwing);
                                                client.CreateBoxDialog("Congratulations!~You~received~90 minutes of EXP, 20 Study Points, 1 Chi Token.!");

                                                break;
                                            }
                                        case 729308:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
                                                client.CreateBoxDialog("Congratulations!~You~received~120 minutes of EXP, 50 Study Points, 1 Chi Token!");

                                                break;
                                            }
                                        case 729309:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
                                                client.CreateBoxDialog("Congratulations!~You~received~180 minutes of EXP, 100 Study Points, 1 Chi Token.!");

                                                break;
                                            }
                                        case 7293010:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 300, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
                                                client.CreateBoxDialog("Congratulations!~You~received~300 minutes of EXP, 300 Study Points, 1 Chi Token.!");

                                                break;
                                            }
                                    }
                                }
                            }
                            else
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.RemovePick(stream);
                                }
                            }
                        }
                    }
                }

                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == Game.MsgTournaments.ProcesType.Alive)
                {
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.KingOfTheHill)
                    {
                        var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgKingOfTheHill;
                        tournament.Revive(timer, client);
                        tournament.GetPoints(client);
                    }
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.KillerOfElite)
                    {
                        var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgTheKillerOfElite;
                        tournament.Revive(timer, client);
                    }
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.SkillTournament)
                    {
                        var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgSkillTournament;
                        tournament.Revive(timer, client);
                    }

                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.DragonWar)
                    {
                       // var tourn = (Game.MsgTournaments.MsgDragonWar)Game.MsgTournaments.MsgSchedules.CurrentTournament;
                        //tourn.DragonWarRevive(timer, client);
                    }
                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.ExtremePk)
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgExtremePk;
                            tournament.Revive(timer, client);
                        }
                    }

                }
                if (timer > client.Player.OnlineStamp.AddMinutes(1))
                {
                    client.Player.OnlineMinutes += 1;
                    client.Player.OnlineStamp = Extensions.Time32.Now;
                }
                if (client.Player.Map == 1005)//pk arena
                {
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.SoulShackle))
                    {
                        if (!client.Player.Alive)
                        {
                            if (client.Player.DeadStamp.AddSeconds(5) < timer)
                            {
                                ushort x = 0; ushort y = 0;
                                client.Map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 1005, 0);
                            }
                        }
                    }
                    if (client.Player.StampArenaScore.AddSeconds(3) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisShoot != 0)
                            Rate = (uint)(((float)client.Player.HitShoot / (float)client.Player.MisShoot) * 100f);

                        client.SendSysMesage("[Arena Stats]", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage("Shots: " + client.Player.MisShoot + " Hits: " + client.Player.HitShoot + " Rate: " + Rate.ToString() + " percent", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage("Kills: " + client.Player.ArenaKills + " Deaths: " + client.Player.ArenaDeads + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        client.Player.StampArenaScore = timer;


                    }
                }
                if (client.Player.Map == 5051)//pk arena
                {

                    if (!client.Player.Alive)
                    {
                        if (client.Player.CanPlaySSFB)
                        {
                            client.Teleport(295, 275, 1002);
                            client.Player.CanPlaySSFB = false;
                        }
                        if (client.Player.DeadStamp.AddSeconds(4) < timer)
                        {
                            ushort x = 0; ushort y = 0;
                            client.Map.GetRandCoord(ref x, ref y);
                            client.Teleport(x, y, 5051, 0);
                        }
                    }
                    if (client.Player.StampSFScore.AddSeconds(3) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisSBShoot != 0)
                            Rate = (uint)(((float)client.Player.HitSBShoot / (float)client.Player.MisSBShoot) * 100f);

                        client.SendSysMesage("[FB/SS Stats]", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage("Shots: " + client.Player.MisSBShoot + " Hits: " + client.Player.HitSBShoot + " Rate: " + Rate.ToString() + " percent", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage("Kills: " + client.Player.ArenaSBKills + " Deaths: " + client.Player.ArenaSBDeads + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);

                        client.Player.StampSFScore = timer;


                    }
                }
                client.Player.UpdateTaoistPower(timer);

                if (client.Player.X == 0 || client.Player.Y == 0)
                {
                    client.Teleport(300, 278, 1002);
                }
                if (client.Player.HeavenBlessing > 0)
                {
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing))
                    {
                        if (timer > client.Player.HeavenBlessTime)
                        {
                            client.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing);
                            client.Player.HeavenBlessing = 0;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, 0, Game.MsgServer.MsgUpdate.DataType.HeavensBlessing);
                                client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.Remove, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);

                                client.Player.Stamina = (ushort)Math.Min((int)client.Player.Stamina, 100);
                                client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                            }
                        }
                        if (client.Player.Map != 601 && client.Player.Map != 1039)
                        {
                            if (timer > client.Player.ReceivePointsOnlineTraining)
                            {
                                client.Player.ReceivePointsOnlineTraining = timer.AddMinutes(1);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.IncreasePoints, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);//+10
                                }
                            }
                            if (timer > client.Player.OnlineTrainingTime)
                            {
                                client.Player.OnlineTrainingPoints += 100000;
                                client.Player.OnlineTrainingTime = timer.AddMinutes(10);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.ReceiveExperience, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                                }
                            }
                        }
                    }
                }
                if (client.Player.EnlightenReceive > 0)
                {
                    if (DateTime.Now > client.Player.EnlightenTime.AddMinutes(20))
                    {
                        client.Player.EnlightenTime = DateTime.Now;
                        client.Player.EnlightenReceive -= 1;
                    }
                }
                if (client.Player.DExpTime > 0)
                {
                    client.Player.DExpTime -= 1;
                    if (client.Player.DExpTime == 0)
                        client.Player.RateExp = 1;
                }



            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }
    }
}