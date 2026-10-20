// Money and points commands: silver, CPs, VIP, event points.
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

    internal static unsafe class MoneyCommands
    {
        [GmCommand("claimcp", Role = CommandRole.GM)]
        static void Cmd_claimcp(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.ChargCps = uint.Parse(data[1]);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        // @creditgift name [reset]: unlock the First Credit Pack for a player who credited (reset = allow claiming again).
        [GmCommand("creditgift", Role = CommandRole.GM, Usage = "@creditgift name [reset]")]
        static void Cmd_creditgift(Client.GameClient client, string[] data)
        {
            if (data.Length < 2)
            {
                client.SendSysMesage("@creditgift name [reset]");
                return;
            }
            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (user.Player.Name.ToLower() != data[1].ToLower())
                    continue;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (data.Length > 2 && data[2].ToLower() == "reset")
                        user.Player.MainFlag &= ~(Role.Player.MainFlagType.CanClaim | Role.Player.MainFlagType.ClaimGift | Role.Player.MainFlagType.ShowSpecialItems);
                    if (user.Player.MarkFirstCredit(stream))
                        client.SendSysMesage("First Credit Pack unlocked for " + user.Player.Name + ".");
                    else
                        client.SendSysMesage(user.Player.Name + " already has the First Credit Pack (unlocked or claimed). Use: @creditgift name reset");
                }
                return;
            }
            client.SendSysMesage("Player " + data[1] + " is not online.");
        }

        [GmCommand("recharge", Role = CommandRole.GM)]
        static void Cmd_recharge(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.RechargePoints = uint.Parse(data[1]);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("battlepoints", Role = CommandRole.GM)]
        static void Cmd_battlepoints(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.BattleFieldPoints = ushort.Parse(data[1]);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("vip", Role = CommandRole.GM, Usage = "@vip name level(0-7)")]
        static void Cmd_vip(Client.GameClient client, string[] data)
        {
            try
            {
                byte vipLevel;
                if (data.Length < 3 || !byte.TryParse(data[2], out vipLevel) || vipLevel > 7)
                {
                    client.SendSysMesage("@vip name level(0-7)");
                    return;
                }
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Name.ToLower() == data[1].ToLower())
                    {

                        if (DateTime.Now > user.Player.ExpireVip)
                            user.Player.ExpireVip = DateTime.Now.AddDays(30);
                        else
                            user.Player.ExpireVip = user.Player.ExpireVip.AddDays(30);

                        user.Player.VipLevel = vipLevel;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            user.Player.SendUpdate(stream, user.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                            user.Player.UpdateVip(stream);
                        }
                        user.CreateBoxDialog("You`ve received vip6 (30 days) . Thank for you donation.");

                        break;
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("arenapoints", Role = CommandRole.GM)]
        static void Cmd_arenapoints(Client.GameClient client, string[] data)
        {
            try
            {
                client.HonorPoints = uint.Parse(data[1]);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("le", Role = CommandRole.GM)]
        static void Cmd_le(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.RacePoints = uint.Parse(data[1]);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("money", Role = CommandRole.GM)]
        static void Cmd_money(Client.GameClient client, string[] data)
        {
            try
            {
                long amount = 0;
                if (data.Length > 1 && long.TryParse(data[1], out amount) && amount >= 0)
                {
                    client.Player.Money = amount;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("cps", Role = CommandRole.GM)]
        static void Cmd_cps(Client.GameClient client, string[] data)
        {
            try
            {
                uint amount = 0;
                // CP balance is kept within int range
                if (data.Length > 1 && uint.TryParse(data[1], out amount) && amount <= int.MaxValue)
                {
                    client.Player.ConquerPoints = amount;

                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("bcps", Role = CommandRole.GM)]
        static void Cmd_bcps(Client.GameClient client, string[] data)
        {
            try
            {
                int bcps;
                if (data.Length < 2 || !int.TryParse(data[1], out bcps) || bcps < 0)
                {
                    client.SendSysMesage("Invalid amount !");
                    return;
                }
                client.Player.BoundConquerPoints = bcps;
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("innerpotency", Role = CommandRole.GM)]
        static void Cmd_innerpotency(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var packet = rec.GetStream();
                    client.Player.InnerPower.AddPotency(packet, client, int.Parse(data[1]));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("championpoints", Role = CommandRole.GM)]
        static void Cmd_championpoints(Client.GameClient client, string[] data)
        {
            try
            {
                client.Player.AddChampionPoints(uint.Parse(data[1]));
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }
    }
}
