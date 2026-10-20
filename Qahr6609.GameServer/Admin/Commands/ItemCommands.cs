// Item commands: give / create / remove items.
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

    internal static unsafe class ItemCommands
    {
        [GmCommand("test", Role = CommandRole.GM)]
        static void Cmd_test(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Inventory.Add(stream, 1088000, 1,0,0,0,0,0,false,0,false,"",0,0,1);
                }

                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("itemds", Role = CommandRole.GM)]
        static void Cmd_itemds(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    uint itemId;
                    if (data.Length < 2 || !uint.TryParse(data[1], out itemId))
                    {
                        client.SendSysMesage("Invlid item ID !");
                        return;
                    }
                    client.Inventory.Add(stream, itemId);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("clear", Role = CommandRole.GM)]
        static void Cmd_clear(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    client.Inventory.Clear(rec.GetStream());
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("ret", Role = CommandRole.GM)]
        static void Cmd_ret(Client.GameClient client, string[] data)
        {
            try
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var apacket = rec.GetStream();
                    client.Inventory.AddReturnedItem(apacket, uint.Parse(data[1]), byte.Parse(data[2]));
                }
               
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("itemeffect", Role = CommandRole.GM)]
        static void Cmd_itemeffect(Client.GameClient client, string[] data)
        {
            try
            {
                MsgGameItem item;
                if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out item))
                {
                    item.Effect = (Role.Flags.ItemEffect)ushort.Parse(data[1]);
                    item.Mode = Role.Flags.ItemMode.Update;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        item.Send(client, stream);
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("give", Role = CommandRole.GM, Usage = "@give name type [values]")]
        static void Cmd_give(Client.GameClient client, string[] data)
        {
            try
            {
                if (data.Length < 3)
                {
                    client.SendSysMesage("@give name type [values]");
                    return;
                }
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Name.ToLower() == data[1].ToLower())
                    {

                        switch (data[2])
                        {
                           
                            case "item":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        uint giveId;
                                        byte giveCount = 1;
                                        if (data.Length < 4 || !uint.TryParse(data[3], out giveId) || (data.Length > 4 && !byte.TryParse(data[4], out giveCount)))
                                        {
                                            client.SendSysMesage("Invlid item ID !");
                                            break;
                                        }
                                        user.Inventory.Add(stream, giveId, giveCount);
                                    }
                                    break;
                                }
                            case "claimcp":
                                {
                                    uint claim;
                                    if (data.Length < 4 || !uint.TryParse(data[3], out claim) || !Conquer6609.Security.Guard.ValidAmount(claim, Conquer6609.Security.Guard.MaxCpsAmount))
                                    {
                                        client.SendSysMesage("Invalid amount !");
                                        break;
                                    }
                                    user.Player.ChargCps = claim;
                                    user.Player.RechargePoints = ClampAddUInt(user.Player.RechargePoints, claim);
                                    break;
                                }
                            case "vip":
                                {
                                    //byte level = byte.Parse(data[3]);
                                    //int time = int.Parse(data[4]);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        if (DateTime.Now > user.Player.ExpireVip)
                                            user.Player.ExpireVip = DateTime.Now.AddMonths(1);
                                        else
                                            user.Player.ExpireVip = user.Player.ExpireVip.AddMonths(1);

                                        user.Player.VipLevel = 6;

                                        user.Player.SendUpdate(stream, user.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                                        user.Player.UpdateVip(stream);

                                        user.CreateBoxDialog("You`ve received vip" + 6 + " (" + 1 + " months).");
                                        client.Player.MessageBox(data[1] + " Has added vip.", null, null);

                                    }

                                    break;
                                }
                            case "innerpotency":
                                {
                                    int amount = 0;
                                    if (data.Length > 3 && int.TryParse(data[3], out amount) && amount > 0)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            user.Player.InnerPower.AddPotency(stream, user, amount);
                                            user.CreateBoxDialog("You receive " + amount + " InnerPower Potency.");
                                        }
                                    }
                                    break;
                                }
                            case "level":
                                {
                                    byte amount = 0;
                                    if (data.Length > 3 && byte.TryParse(data[3], out amount))
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            user.UpdateLevel(stream, amount, true);
                                        }
                                    }
                                    break;
                                }
                            case "money":
                                {
                                    long addMoney;
                                    if (data.Length < 4 || !long.TryParse(data[3], out addMoney) || !Conquer6609.Security.Guard.ValidAmount(addMoney, Conquer6609.Security.Guard.MaxMoneyAmount))
                                    {
                                        client.SendSysMesage("Invalid amount !");
                                        break;
                                    }
                                    long curMoney = user.Player.Money;
                                    user.Player.Money = curMoney > long.MaxValue - addMoney ? long.MaxValue : curMoney + addMoney;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                    }
                                    break;
                                }
                            case "cps":
                                {
                                    uint addCps;
                                    if (data.Length < 4 || !uint.TryParse(data[3], out addCps) || !Conquer6609.Security.Guard.ValidAmount(addCps, Conquer6609.Security.Guard.MaxCpsAmount))
                                    {
                                        client.SendSysMesage("Invalid amount !");
                                        break;
                                    }
                                    // CP balance is kept within int range
                                    long newCps = (long)user.Player.ConquerPoints + addCps;
                                    if (newCps > int.MaxValue)
                                        newCps = int.MaxValue;
                                    user.Player.ConquerPoints = (uint)newCps;

                                    break;
                                }
                            case "le":
                                {
                                    uint race;
                                    if (data.Length < 4 || !uint.TryParse(data[3], out race))
                                    {
                                        client.SendSysMesage("Invalid amount !");
                                        break;
                                    }
                                    user.Player.RacePoints = race;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        user.Player.SendUpdate(stream, user.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
                                    }
                                    break;
                                }
                            case "recharge":
                                {
                                    uint recharge;
                                    if (data.Length < 4 || !uint.TryParse(data[3], out recharge))
                                    {
                                        client.SendSysMesage("Invalid amount !");
                                        break;
                                    }
                                    user.Player.RechargePoints = recharge;
                                    break;
                                }
                            case "item2":
                                {
                                    if (data.Length < 9)
                                    {
                                        client.SendSysMesage("@give name item2 id plus bless enchant gem1 gem2 [count] [effect]");
                                        break;
                                    }
                                    uint ID = 0;
                                    if (!uint.TryParse(data[3], out ID))
                                    {
                                        client.SendSysMesage("Invlid item ID !");
                                        break;
                                    }
                                    byte plus = 0;
                                    if (!byte.TryParse(data[4], out plus))
                                    {
                                        client.SendSysMesage("Invlid item plus !");
                                        break;
                                    }
                                    byte bless = 0;
                                    if (!byte.TryParse(data[5], out bless))
                                    {
                                        client.SendSysMesage("Invlid item Enchant !");
                                        break;
                                    }
                                    byte enchant = 0;
                                    if (!byte.TryParse(data[6], out enchant))
                                    {
                                        client.SendSysMesage("Invlid item Enchant !");
                                        break;
                                    }
                                    byte sockone = 0;
                                    if (!byte.TryParse(data[7], out sockone))
                                    {
                                        client.SendSysMesage("Invlid item Socket One !");
                                        break;
                                    }
                                    byte socktwo = 0;
                                    if (!byte.TryParse(data[8], out socktwo))
                                    {
                                        client.SendSysMesage("Invlid item Socket Two !");
                                        break;
                                    }
                                    byte count = 1;
                                    if (data.Length > 9)
                                    {
                                        if (!byte.TryParse(data[9], out count))
                                        {
                                            client.SendSysMesage("Invlid item count !");
                                            break;
                                        }
                                    }
                                    byte Effect = 0;
                                    if (data.Length > 10)
                                    {
                                        if (!byte.TryParse(data[10], out Effect))
                                        {
                                            client.SendSysMesage("Invlid Effect Type !");
                                            break;
                                        }
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        user.Inventory.Add(rec.GetStream(), ID, count, plus, bless, enchant, (Role.Flags.Gem)sockone, (Role.Flags.Gem)socktwo, false, (Role.Flags.ItemEffect)Effect);

                                    break;
                                }

                        }
                        break;
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("addsouls", Role = CommandRole.GM)]
        static void Cmd_addsouls(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    List<string> Items = new List<string>();

                    var array = Database.ItemType.PurificationItems[ushort.Parse(data[1])].Values.ToArray();
                    for (int x = 0; x < array.Length; x++)
                    {
                        Items.Add(array[x].Name + " " + array[x].ID);
                        client.Inventory.Add(stream, array[x].ID);
                    }
                    Database.DBActions.Write writer = new Database.DBActions.Write("Souls" + ushort.Parse(data[1]) + ".ini");
                    foreach (var it in Items)
                        writer.Add(it);
                    writer.Execute(Database.DBActions.Mode.Open);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("addrefinary", Role = CommandRole.GM)]
        static void Cmd_addrefinary(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    List<string> Items = new List<string>();

                    var array = Database.ItemType.Refinary[ushort.Parse(data[1])].Values.ToArray();
                    for (int x = 0; x < array.Length; x++)
                    {
                        Items.Add(array[x].Name + " " + array[x].ItemID);
                        client.Inventory.Add(stream, array[x].ItemID);
                    }
                    Database.DBActions.Write writer = new Database.DBActions.Write("Souls" + ushort.Parse(data[1]) + ".ini");
                    foreach (var it in Items)
                        writer.Add(it);
                    writer.Execute(Database.DBActions.Mode.Open);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("itemm", Role = CommandRole.GM)]
        static void Cmd_itemm(Client.GameClient client, string[] data)
        {
            try
            {
                uint ID = 0;
                if (!uint.TryParse(data[1], out ID))
                {
                    client.SendSysMesage("Invlid item ID !");
                    return;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                    client.Inventory.AddItemWitchStack(ID, 0, 1, rec.GetStream(), false);

                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("item", Role = CommandRole.GM, Usage = "@item id plus bless enchant gem1 gem2 [count] [effect]")]
        static void Cmd_item(Client.GameClient client, string[] data)
        {
            try
            {
                if (data.Length < 7)
                {
                    client.SendSysMesage("@item id plus bless enchant gem1 gem2 [count] [effect]");
                    return;
                }
                uint ID = 0;
                if (!uint.TryParse(data[1], out ID))
                {
                    client.SendSysMesage("Invlid item ID !");
                    return;
                }
                byte plus = 0;
                if (!byte.TryParse(data[2], out plus))
                {
                    client.SendSysMesage("Invlid item plus !");
                    return;
                }
                byte bless = 0;
                if (!byte.TryParse(data[3], out bless))
                {
                    client.SendSysMesage("Invlid item Enchant !");
                    return;
                }
                byte enchant = 0;
                if (!byte.TryParse(data[4], out enchant))
                {
                    client.SendSysMesage("Invlid item Enchant !");
                    return;
                }
                byte sockone = 0;
                if (!byte.TryParse(data[5], out sockone))
                {
                    client.SendSysMesage("Invlid item Socket One !");
                    return;
                }
                byte socktwo = 0;
                if (!byte.TryParse(data[6], out socktwo))
                {
                    client.SendSysMesage("Invlid item Socket Two !");
                    return;
                }
                byte count = 1;
                if (data.Length > 7)
                {
                    if (!byte.TryParse(data[7], out count))
                    {
                        client.SendSysMesage("Invlid item count !");
                        return;
                    }
                }
                byte Effect = 0;
                if (data.Length > 8)
                {
                    if (!byte.TryParse(data[8], out Effect))
                    {
                        client.SendSysMesage("Invlid Effect Type !");
                        return;
                    }
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    client.Inventory.Add(rec.GetStream(), ID, count, plus, bless, enchant, (Role.Flags.Gem)sockone, (Role.Flags.Gem)socktwo, false, (Role.Flags.ItemEffect)Effect);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("updatestarsitem", Role = CommandRole.GM)]
        static void Cmd_updatestarsitem(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    foreach (var item in client.Inventory.ClientItems.Values)
                    {
                        if (item.ITEM_ID == uint.Parse(data[1]))
                        {
                            item.PerfectionLevel = uint.Parse(data[2]);
                            item.Mode = Role.Flags.ItemMode.Update;
                            item.Send(client, rec.GetStream());
                            break;
                        }

                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("additemstack", Role = CommandRole.GM)]
        static void Cmd_additemstack(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Inventory.AddItemWitchStack(uint.Parse(data[1]), 0, ushort.Parse(data[2]), stream);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("remitemstack", Role = CommandRole.GM)]
        static void Cmd_remitemstack(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Inventory.RemoveStackItem(uint.Parse(data[1]), ushort.Parse(data[2]), stream);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("inneritems", Role = CommandRole.GM)]
        static void Cmd_inneritems(Client.GameClient client, string[] data)
        {
            try
            {
                foreach (var stage in Database.InnerPowerTable.Stages)
                {
                    foreach (var gong in stage.NeiGongAtributes)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var packet = rec.GetStream();
                            client.Inventory.Add(packet, gong.ItemID);
                        }
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }
    }
}
