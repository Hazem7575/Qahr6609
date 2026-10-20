// Debug / packet test commands.
// Migrated 1:1 from the old `switch (data[0])` in Game/MsgServer/MsgMessage.cs (ChatCommands).
// Every command keeps its old name (case-sensitive), its `data[]` indices and its body; `break` that left the
// switch became `return`. Role = GM is the old gate (client.GameMaster || client.ProjectManager).
// Auditing (gm-command / gm-denied) is done by CommandRegistry; argument errors give the old messages
// through CommandSupport.ArgumentError.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Game.Conquer6609_Work;
using System.IO;
using Conquer6609.Role;
using Conquer6609.Framework.Commands;

namespace Conquer6609.Commands
{
    // same name lookup as inside namespace Conquer6609.Game.MsgServer / class MsgMessage
    using Conquer6609.Game;
    using Conquer6609.Game.MsgServer;
    using static Conquer6609.Game.MsgServer.MsgMessage;
    using MsgServer = Conquer6609.Game.MsgServer;
    using MsgTournaments = Conquer6609.Game.MsgTournaments;

    internal static unsafe class DebugCommands
    {
        [GmCommand("gemeff", Role = CommandRole.GM)]
        static void Cmd_gemeff(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[1] { data[1] });
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("addgui", Role = CommandRole.GM)]
        static void Cmd_addgui(Client.GameClient client, string[] data)
        {
            try
            {
                ActionQuery action = new ActionQuery()
                {
                    Type = ActionType.OpenDialog,
                    dwParam2 = 100,
                    dwParam3 = 10000,
                    dwParam4 = 1000,
                    ObjId = client.Player.UID,
                    dwParam = uint.Parse(data[1]),//MsgServer.DialogCommands.JiangHuSetName,
                    wParam1 = client.Player.X,
                    wParam2 = client.Player.Y
                };
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var apacket = rec.GetStream();
                    client.Send(apacket.ActionCreate(&action));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("teffect", Role = CommandRole.GM)]
        static void Cmd_teffect(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Send(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                    {
                        Effect = (MsgRefineEffect.RefineEffects)uint.Parse(data[1]),
                        Id = client.Player.UID,
                        dwParam = (uint)client.Player.UID

                    }));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("pick", Role = CommandRole.GM)]
        static void Cmd_pick(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.AddPick(stream, "Pick", 5);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("str", Role = CommandRole.GM)]
        static void Cmd_str(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "bagua-5");

                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("facke", Role = CommandRole.GM, Usage = "@facke count")]
        static void Cmd_facke(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    ushort fakeRequested;
                    if (data.Length < 2 || !ushort.TryParse(data[1], out fakeRequested))
                    {
                        client.SendSysMesage("@facke count");
                        return;
                    }
                    // fake client count is capped
                    int fakeCount = Conquer6609.Security.Guard.ClampCount(fakeRequested, 50);
                    for (int i = 0; i < fakeCount; i++)
                    {
                        Client.GameClient pclient = new Client.GameClient(null);
                        pclient.Fake = true;

                        pclient.Player = new Role.Player(pclient);
                        pclient.Inventory = new Role.Instance.Inventory(pclient);
                        pclient.Equipment = new Role.Instance.Equip(pclient);
                        pclient.Warehouse = new Role.Instance.Warehouse(pclient);
                        pclient.MyProfs = new Role.Instance.Proficiency(pclient);
                        pclient.MySpells = new Role.Instance.Spell(pclient);
                        pclient.Achievement = new Database.AchievementCollection();
                        pclient.Status = new MsgStatus();

                        pclient.Player.Name = "Basta [" + i.ToString() + "]";
                        pclient.Player.Body = client.Player.Body;
                        pclient.Player.UID = Database.Server.ClientCounter.Next;
                        pclient.Player.HitPoints = client.Player.HitPoints;
                        pclient.Status.MaxHitpoints = client.Status.MaxHitpoints;

                        ushort x = client.Player.X;
                        ushort y = client.Player.Y;
                        pclient.Player.X = (ushort)Program.GetRandom.Next((int)x - 7, x + 7);
                        pclient.Player.Y = (ushort)Program.GetRandom.Next((int)y - 7, y + 7);
                        pclient.Player.Map = client.Player.Map;
                        pclient.Player.Level = client.Player.Level;
                        pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                        client.Send(pclient.Player.GetArray(stream, false));

                        pclient.Map = client.Map;
                        pclient.Map.Enquer(pclient);
                        Database.Server.GamePoll.TryAdd(pclient.Player.UID, pclient);
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("mapstat", Role = CommandRole.GM)]
        static void Cmd_mapstat(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Send(stream.MapStatusCreate(client.Map.ID, client.Map.ID, (ulong)(1U << int.Parse(data[1]))));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("sound", Role = CommandRole.GM)]
        static void Cmd_sound(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendString(stream, MsgStringPacket.StringID.Sound, false, "sound/wind.wav", "1");

                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("string_effect", Role = CommandRole.GM)]
        static void Cmd_string_effect(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, data[1]);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("lotus", Role = CommandRole.GM)]
        static void Cmd_lotus(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Database.Server.AddFloor(stream, client.Map, Game.MsgFloorItem.MsgItemPacket.FlameLotus, client.Player.X, client.Player.Y, 1, Database.Server.Magic[12370][1], client, client.Player.GuildID, client.Player.UID, 0, "AuroraLotus", true);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("exchange2", Role = CommandRole.GM)]
        static void Cmd_exchange2(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    MsgExchangeShop.ExchangeShop.Item[] items = new MsgExchangeShop.ExchangeShop.Item[1];
                    items[0] = new MsgExchangeShop.ExchangeShop.Item();
                    items[0].ID = client.Player.UID;

                    client.Send(stream.CreateExchangeShop(new MsgExchangeShop.ExchangeShop()
                    {
                        DwParam1 = 19424,
                        DwParam2 = uint.Parse(data[1]),
                        DwParam3 = uint.Parse(data[2]),
                        DwParam4 = uint.Parse(data[3])//timer
                    }));


                }

                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("teelite", Role = CommandRole.GM)]
        static void Cmd_teelite(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Send(stream.MsgInterServerIdentifier(0, client.Player.UID, uint.Parse(data[1]), client.GetAllMainItems().ToArray()));
                    client.Player.SendString(stream, MsgStringPacket.StringID.ServerName, false, data[2]);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("dc", Role = CommandRole.GM)]
        static void Cmd_dc(Client.GameClient client, string[] data)
        {
            try
            {

                client.Socket.Disconnect();
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("testactwar", Role = CommandRole.GM)]
        static void Cmd_testactwar(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var item = client.MyWardrobe.GetAllItems();
                    var stream = rec.GetStream();
                    Game.MsgServer.MsgCoatStorage.CoatStorage store = new MsgCoatStorage.CoatStorage();
                    store.ActionID = (MsgCoatStorage.Action)uint.Parse(data[1]);
                    store.dwparam1 = item.FirstOrDefault().UID;
                    store.dwpram2 = item.FirstOrDefault().ITEM_ID;
                    client.Send(stream.CreateCoatStorage(store));
                    client.Send(stream.CreateCoatStorage(store));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("title", Role = CommandRole.GM)]
        static void Cmd_title(Client.GameClient client, string[] data)
        {
            try
            {
                foreach (var atitle in Database.TitleStorage.Titles.Values)
                {
                    if (atitle.ID == 2004)
                    {
                        MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                        title.ActionID = (MsgTitleStorage.Action)uint.Parse(data[1]);
                        title.dwparam1 = 100;
                        title.dwparam2 = atitle.ID;
                        title.dwparam3 = atitle.SubID;
                        title.Title = new MsgTitleStorage.Title();
                        title.Title.ID = atitle.ID;
                        title.Title.SubId = atitle.SubID;
                        title.Title.dwparam1 = 100;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var packet = rec.GetStream();
                            packet.CreateTitleStorage(title);
                            client.Send(packet);

                        }
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("wea", Role = CommandRole.GM)]
        static void Cmd_wea(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var packet = rec.GetStream();
                    packet.WeatherCreate((MsgWeather.WeatherType)ushort.Parse(data[1]), uint.Parse(data[2]), uint.Parse(data[3]), (uint)uint.Parse(data[4]), uint.Parse(data[5]));
                    client.Send(packet);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("activefairi", Role = CommandRole.GM)]
        static void Cmd_activefairi(Client.GameClient client, string[] data)
        {
            try
            {
                unsafe
                {
                    if (client.Player.testtttttttttt != 0)
                    {
                        MsgTransformFairy afair = MsgTransformFairy.Create();
                        afair.Mode = MsgTransformFairy.Action.Dezactive;
                        afair.FairyType = client.Player.testtttttttttt;
                        afair.UID = client.Player.UID;


                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var packet = rec.GetStream();
                            packet.TransformFairyCreate(MsgTransformFairy.Action.Dezactive, client.Player.testtttttttttt, client.Player.UID);
                            client.Send(packet);
                        }
                    }


                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var packet = rec.GetStream();
                        packet.TransformFairyCreate(MsgTransformFairy.Action.Active, uint.Parse(data[1]), client.Player.UID);
                        client.Send(packet);
                    }



                    client.Player.testtttttttttt = uint.Parse(data[1]);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("switch", Role = CommandRole.GM)]
        static void Cmd_switch(Client.GameClient client, string[] data)
        {
            try
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SwitchWingWalkerAttack(stream);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("bossrank", Role = CommandRole.GM)]
        static void Cmd_bossrank(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    MsgServer.MsgBossHarmRanking Rank = new MsgServer.MsgBossHarmRanking();
                    Rank.Type = (MsgServer.MsgBossHarmRanking.RankAction)int.Parse(data[1]);
                    Rank.MonsterID = uint.Parse(data[2]);
                    Rank.Hunters = new MsgServer.MsgBossHarmRankingEntry[1];
                    Rank.Hunters[0].HunterName = client.Player.Name;
                    Rank.Hunters[0].HunterUID = client.Player.UID;
                    Rank.Hunters[0].Rank = uint.Parse(data[3]);
                    Rank.Hunters[0].ServerID = Database.GroupServerList.MyServerInfo.ID;
                    Rank.Hunters[0].HunterScore = uint.Parse(data[4]);
                    client.Send(stream.CreateBossHarmRankList(Rank));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("group", Role = CommandRole.GM)]
        static void Cmd_group(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    MsgServer.MsgSameGroupServerList.GroupServer group = new MsgSameGroupServerList.GroupServer();

                    group.Servers = new MsgSameGroupServerList.Server[1];
                    for (int x = 0; x < 1; x++)
                    {

                        group.Servers[x] = new MsgSameGroupServerList.Server();
                        group.Servers[x].GroupID = uint.Parse(data[1]);
                        group.Servers[x].MapID = uint.Parse(data[2]);
                        group.Servers[x].Name = data[3];
                        group.Servers[x].X = uint.Parse(data[4]);
                        group.Servers[x].Y = uint.Parse(data[5]);
                        group.Servers[x].ServerID = uint.Parse(data[6]);
                    }
                    client.Send(stream.CreateGroupServerList(group));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("exp", Role = CommandRole.GM)]
        static void Cmd_exp(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    client.Player.SendUpdate(stream, uint.Parse(data[1]), MsgUpdate.DataType.Experience, false);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

    }
}
