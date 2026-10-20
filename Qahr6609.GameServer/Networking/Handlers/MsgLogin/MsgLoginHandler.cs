using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;
using Project2D.Database;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet LoginHandlerCreate(this ServerSockets.Packet stream, uint Type, uint Map)
        {
            stream.InitWriter();

            stream.Write(0);
            stream.Write(Type);
            stream.Write(Map);

            stream.Finalize(GamePackets.MapLoading);

            return stream;
        }

    }
    public unsafe struct MsgLoginHandler
    {

        [PacketAttribute(GamePackets.MapLoading)]
        public unsafe static void LoadMap(Client.GameClient client, ServerSockets.Packet packet)
        {
            if ((client.ClientFlag & Client.ServerFlag.AcceptLogin) == Client.ServerFlag.AcceptLogin)
            {
                try
                {
                    client.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;

                    client.Send(packet.HeroInfo(client.Player));


                    MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Upgrade, client, 142);


                    client.Send(packet.FlowerCreate(Role.Core.IsBoy(client.Player.Body) ? MsgFlower.FlowerAction.Flower : MsgFlower.FlowerAction.FlowerSender
                        , 0, 0, client.Player.Flowers.RedRoses, client.Player.Flowers.RedRoses.Amount2day
                        , client.Player.Flowers.Lilies, client.Player.Flowers.Lilies.Amount2day
                        , client.Player.Flowers.Orchids, client.Player.Flowers.Orchids.Amount2day
                        , client.Player.Flowers.Tulips, client.Player.Flowers.Tulips.Amount2day));


                    if (client.Player.Flowers.FreeFlowers > 0)
                    {
                        client.Send(packet.FlowerCreate(Role.Core.IsBoy(client.Player.Body)
                            ? MsgFlower.FlowerAction.FlowerSender : MsgFlower.FlowerAction.Flower
                            , 0, 0, client.Player.Flowers.FreeFlowers));
                    }
                    client.Send(packet.NobilityIconCreate(client.Player.Nobility));
                    if (client.Player.Achievement != null)
                        client.Player.Achievement.Send(client, packet);

                    if (client.Player.BlessTime > 0)
                        client.Player.SendUpdate(packet, client.Player.BlessTime, MsgUpdate.DataType.LuckyTimeTimer);

                    client.Player.ProtectAttack(1000 * 10);//10 secounds
                    client.Player.CreateHeavenBlessPacket(packet, true);


                    if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.QuizShow
                        && MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                        MsgTournaments.MsgSchedules.CurrentTournament.Join(client, packet);
                        

                    if (client.Player.DExpTime > 0)
                        client.Player.CreateExtraExpPacket(packet);
                    client.Player.UpdateExploitsRank();

                    if (client.Player.MyClan != null)
                    {
                        client.Player.MyClan.SendThat(packet, client);
                        client.Send(packet.ClanRelation(client.Player.MyClan.Ally.Values.ToList(), MsgClan.Info.LoginAlly));
                        client.Send(packet.ClanRelation(client.Player.MyClan.Enemy.Values.ToList(), MsgClan.Info.LoginEnemy));
                        /*foreach (var ally in client.Player.MyClan.Ally.Values)
                            client.Send(packet.ClanRelationCreate(client.Player.MyClan.ID, ally.Name, ally.LeaderName, MsgClan.Info.AddAlly));
                        foreach (var enemy in client.Player.MyClan.Enemy.Values)
                            client.Send(packet.ClanRelationCreate(client.Player.MyClan.ID, enemy.Name, enemy.LeaderName, MsgClan.Info.AddEnemy));*/
                    }

                    client.Equipment.Show(packet);
                    client.Inventory.ShowALL(packet);

                    if (client.Player.MyJiangHu != null)
                        client.Player.MyJiangHu.LoginClient(packet, client);
                    else if (client.Player.Reborn == 2)
                    {
                        client.Send(packet.JiangHuInfoCreate(MsgJiangHuInfo.JiangMode.IconBar, "0"));
                    }
                    //send chi------------- query
                    // Plunder War running: enemy/ally union lists for the Loot PK mode (2641)
                    Game.MsgTournaments.MsgKingdomWar.SendWarState(client);
                    // only unlocked powers: a locked one has UID 0, and the client asserts on a "my rank" entry
                    // that is not the hero (msgrank.cpp:240)
                    foreach (var chipower in client.Player.MyChi)
                    {
                        if (!chipower.UnLocked)
                            continue;
                        if (chipower.UID == 0)
                        {
                            chipower.UID = client.Player.UID;
                            chipower.Name = client.Player.Name;
                        }
                        client.Player.MyChi.SendQueryUpdate(client, chipower, packet);
                    }

                    //send confiscator items
                    foreach (var item in client.Confiscator.RedeemContainer.Values)
                    {
                        if (client.Inventory.Contain(item.ItemUID, 1))
                        {
                            client.Inventory.Remove(item.ItemUID, 1, packet);
                            Console.WriteLine("fuck player " + client.Player.Name + " try use Confiscator bug");
                            Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, 99999, "Hack  Confiscator");
                            string log = ("[Hack]" + client.Player.Name + "He Try To Use hack Confiscator Bug ");
                            Logs.OnDequeue(log);
                            client.Socket.Disconnect();

                        }
                        var Dataitem = item;
                        Dataitem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(item.Date).Ticks).Days);
                        if (Dataitem.DaysLeft > 7)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.RewardCps;
                        }
                        if (Dataitem.Action != MsgDetainedItem.ContainerType.RewardCps)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.DetainPage;
                            Dataitem.Send(client, packet);
                        }
                        if (Dataitem.Action == MsgDetainedItem.ContainerType.RewardCps)
                            client.Confiscator.RedeemContainer.TryRemove(item.UID, out Dataitem);
                    }
                    foreach (var item in client.Confiscator.ClaimContainer.Values)
                    {
                        var Dataitem = item;
                        Dataitem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(item.Date).Ticks).Days);
                        if (Dataitem.RewardConquerPoints != 0)
                        {
                            Dataitem.Action = MsgDetainedItem.ContainerType.RewardCps;
                        }
                        Dataitem.Send(client, packet);
                        client.Confiscator.ClaimContainer[item.UID] = Dataitem;
                    }
                    //-------------

                    // Guild War halo only while still leader/deputy of the current winner guild (no halo after a guild change)
                    MsgTournaments.MsgSchedules.GuildWar.RefreshHalo(client);

                    if (MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.TopSuperGuildWar, Role.StatusFlagsBigVector32.PermanentFlag, false);
                    if (MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Contains(client.Player.UID))
                        client.Player.AddFlag(MsgUpdate.Flags.TopSuperGuildWar, Role.StatusFlagsBigVector32.PermanentFlag, false);
                    if (MsgTournaments.MsgSchedules.CouplesPKWar.Winner1 == client.Player.Name ||
                       MsgTournaments.MsgSchedules.CouplesPKWar.Winner2 == client.Player.Name)
                        client.Player.AddFlag(MsgUpdate.Flags.TopSpouse, Role.StatusFlagsBigVector32.PermanentFlag, false);

                    client.Player.PKPoints = client.Player.PKPoints;
                    if (client.Player.CursedTimer > 0)
                    {
                        client.Player.AddCursed(client.Player.CursedTimer);
                    }

                    client.Send(packet.ServerTimerCreate());
                    #region pk
                    client.Player.PkPointsStamp = Extensions.Time32.Now;
                    if (client.Player.PKPoints > 29)
                    {
                        if (client.Player.ContainFlag(MsgUpdate.Flags.RedName))
                        {
                            client.Player.RemoveFlag(MsgUpdate.Flags.RedName);
                        }
                        client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.RedName, Role.StatusFlagsBigVector32.PermanentFlag, false, 6 * 60);
                        client.Player.SendUpdate(packet, client.Player.PKPoints, Game.MsgServer.MsgUpdate.DataType.PKPoints);

                    }
                    #endregion
                    MsgTournaments.MsgSchedules.ClassPkWar.LoginClient(client);
                    MsgTournaments.MsgSchedules.ElitePkTournament.GetTitle(client, packet);
                    MsgTournaments.MsgSchedules.TeamPkTournament.GetTitle(client, packet);
                    MsgTournaments.MsgSchedules.SkillTeamPkTournament.GetTitle(client, packet);

                    if (MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityID != 1)
                    {
                        client.Send(new MsgServer.MsgMessage(MsgTournaments.MsgBroadcast.CurrentBroadcast.Message
                            , "ALLUSERS"
                            , MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityName
                            , MsgServer.MsgMessage.MsgColor.red
                            , MsgServer.MsgMessage.ChatMode.BroadcastMessage
                            ).GetArray(packet));
                    }


                    if (client.Player.RacePoints > 0)
                        client.Player.SendUpdate(packet, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);

                    client.Activeness.CheckTasks(packet);

                    client.Activeness.IncreaseTask(1);
                    client.Activeness.IncreaseTask(13);
                    client.Activeness.IncreaseTask(25);

                    if (client.Player.VipLevel > 0)
                    {
                        client.Activeness.IncreaseTask(2);
                        client.Activeness.IncreaseTask(14);
                        client.Activeness.IncreaseTask(26);
                    }
                    client.Activeness.UpdateActivityPoints(packet);
                    client.Activeness.UpdateClaimRewards(packet);
                    client.Player.InnerPower.AddPotency(packet, client, 0);
                    client.Player.UpdateVip(packet);
                    //update merchant
                    client.Player.SendUpdate(packet, 255, MsgUpdate.DataType.Merchant);

                    if ((client.Player.MainFlag & Role.Player.MainFlagType.ClaimGift) != Role.Player.MainFlagType.ClaimGift)
                    {
                        client.Player.SendUpdate(packet, (uint)client.Player.MainFlag, MsgUpdate.DataType.MainFlag);
                    }
                    else
                    {
                        client.Player.SendUpdate(packet, MsgUpdate.CreditGifts.ShowSpecialItems, MsgUpdate.DataType.MainFlag);
                    }

                  //  client.Player.SendUpdate(packet, (uint)client.Player.MainFlag, MsgUpdate.DataType.CreditGifts);

                    ActionQuery action = new ActionQuery()
                    {
                        ObjId = client.Player.UID,
                        Type = (ActionType)157,
                        dwParam = 2
                    };

                    client.Send(packet.ActionCreate(&action));
                  
                        client.Send(packet.ServerConfig());

                    

                    if (client.Player.InUnion)
                    {
                        client.Player.MyUnion.SendMyInfo(packet, client);
                    }



                    if (Database.RechargeShop.RechargeAccounts.ContainsKey(client.Player.UID))
                    {
                        if (Database.RechargeShop.RechargeAccounts[client.Player.UID] >= client.Player.RechargeProgress)
                        {
                            client.Player.RechargeProgress = Database.RechargeShop.RechargeAccounts[client.Player.UID];
                        }
                        else
                        {
                            Database.RechargeShop.RechargeAccounts[client.Player.UID] = client.Player.RechargeProgress;
                        }
                    }
                    else if(client.Player.RechargeProgress > 0)
                    {
                        Database.RechargeShop.RechargeAccounts.TryAdd(client.Player.UID, client.Player.RechargeProgress);
                    }


                    // Players only receive the special titles they own (no blanket grant).
                    {
                        Database.TitleStorage.CheckUpUser(client, packet);
                        foreach (var title in client.Player.SpecialTitles)
                            client.Player.AddSpecialTitle(title, packet);
                        client.Player.SendSpecialTitle(packet);
                        client.Player.ActiveSpecialTitles(packet);
                    }

                




                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Send(stream.AutoHuntCreate(0, 341));
                    }
                    if (client.Player.SecurityPassword != 0)
                    {
                        client.Send(packet.SecondaryPasswordCreate(MsgSecondaryPassword.ActionID.PasswordCorrect, 1, 0));
                    }
                    else
                        client.Player.IsCheckedPass = true;

                    MsgTournaments.MsgSchedules.PkWar.AddTop(client);

                    client.SendSysMesage("Welcome to " + Program.ServerConfig.ServerName + ".", MsgMessage.ChatMode.Talk);
                    client.SendSysMesage("The guild war this week will start from 20:00 Sat. and end on 23:00 Sat."/* = MsgSchedules (Activity.ini Guild PK, EU slot) */, MsgMessage.ChatMode.Talk);
                    client.SendSysMesage("The drop rate for vip players is 10 to 50 conquer points in your inventory.", MsgMessage.ChatMode.Talk);
                    client.SendSysMesage("Official Site: " + Program.ServerConfig.OfficialWebSite + " ", MsgMessage.ChatMode.Talk);

                    client.SendSysMesage("Enjoy " + Program.ServerConfig.ServerName + ".", MsgMessage.ChatMode.Talk);


                    //#region Update Accuont
                    //var cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                    //cmd.Update("accounts").Set("Online", 1).Where("EntityID", client.Player.UID).Execute2();
                    //#endregion
                    client.Player.UpdateInventorySash(packet);
                    if (client.Player.ExpProtection > 0)
                        client.Player.CreateExpProtection(packet, 0, false);
                    if (client.Player.VipLevel >= 6)
                    {
                        TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                        TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                        int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                        int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                        int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);
                        if (days_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                        else if (hour_left > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                        else if (left_minutes > 0)
                            client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);



                    }

                    Halo_Flag.Login(client.Player.Owner);//Halo_Flag


                    if (Database.AtributesStatus.IsTrojan(client.Player.Class)
                        || Database.AtributesStatus.IsTrojan(client.Player.FirstClass)
                        || Database.AtributesStatus.IsTrojan(client.Player.SecoundeClass))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            client.MySpells.Add(packet, (ushort)Role.Flags.SpellID.Cyclone);
                    }


                    if (client.Inventory.HaveSpace(1))
                    {
                        foreach (var item in client.Equipment.ClientItems.Values)
                        {
                            if (item.Position >= (uint)Role.Flags.ConquerItem.Head && item.Position <= (uint)Role.Flags.ConquerItem.Wing)
                            {
                                if (client.Inventory.HaveSpace(1) && item.Position == (uint)Role.Flags.ConquerItem.RightWeapon
                                    && item.Position == (uint)Role.Flags.ConquerItem.LeftWeapon)
                                {
                                    if (!Database.ItemType.IsShield(item.ITEM_ID))
                                    {
                                        if (!Database.ItemType.Equipable(item.ITEM_ID, client))
                                        {
                                            client.Equipment.Remove((Role.Flags.ConquerItem)item.Position, packet);
                                        }
                                    }
                                }
                            }
                            else if (item.Position >= (uint)Role.Flags.ConquerItem.AleternanteHead && item.Position <= (uint)Role.Flags.ConquerItem.AleternanteGarment)
                            {
                                if (client.Inventory.HaveSpace(1) && item.Position == (uint)Role.Flags.ConquerItem.AleternanteRightWeapon
                                    && item.Position == (uint)Role.Flags.ConquerItem.AleternanteLeftWeapon)
                                {
                                    if (!Database.ItemType.IsShield(item.ITEM_ID))
                                    {
                                        if (!Database.ItemType.Equipable(item.ITEM_ID, client))
                                        {
                                            client.Equipment.RemoveAlternante((Role.Flags.ConquerItem)item.Position, packet);
                                        }
                                    }
                                }
                            }
                        }
                    }

                  

                    MsgServer.MsgSameGroupServerList.GroupServer group = new MsgSameGroupServerList.GroupServer();
                    var GroupServers = Database.GroupServerList.GroupServers.Values.ToArray();
                    group.Servers = new MsgSameGroupServerList.Server[Database.GroupServerList.GroupServers.Count];
                    for (int x = 0; x < GroupServers.Length; x++)
                    {
                        var DBServer = GroupServers[x];
                        group.Servers[x] = new MsgSameGroupServerList.Server();
                        group.Servers[x].GroupID = DBServer.Group;
                        group.Servers[x].MapID = DBServer.MapID;
                        group.Servers[x].Name = DBServer.Name;
                        group.Servers[x].dwparam = 1;
                        group.Servers[x].X = DBServer.X;
                        group.Servers[x].Y = DBServer.Y;
                        group.Servers[x].ServerID = DBServer.ID;
                    }
                    client.Send(packet.CreateGroupServerList(group));


                    client.Warehouse.SendReturnedItems(packet);
                 

                    client.ClientFlag &= ~Client.ServerFlag.AcceptLogin;
                    client.ClientFlag |= Client.ServerFlag.LoginFull;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
        }

    }
}
