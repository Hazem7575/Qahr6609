// Moderation commands: kick, ban, unban, player info.
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

    internal static unsafe class ModerationCommands
    {
        [GmCommand("info", Role = CommandRole.GM)]
        static void Cmd_info(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    foreach (var user in Database.Server.GamePoll.Values)
                    {
                        if (user.Socket == null)
                            continue;
                        if (user.Player.Name.ToLower() == data[1].ToLower())
                        {

                            client.Send(new MsgMessage("[Info" + user.Player.Name + "]", MsgColor.yellow, ChatMode.FirstRightCorner).GetArray(stream));
                            client.Send(new MsgMessage("UID = " + user.Player.UID + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                            client.Send(new MsgMessage("IP = " + user.Socket.RemoteIp + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                            client.Send(new MsgMessage("ConquerPoints = " + user.Player.ConquerPoints + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                            client.Send(new MsgMessage("Money = " + user.Player.Money + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                            client.Send(new MsgMessage("Map = " + user.Player.Map + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                            client.Send(new MsgMessage("X = " + user.Player.X + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                            client.Send(new MsgMessage("Y = " + user.Player.Y + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                            client.Send(new MsgMessage("BattlePower = " + user.Player.BattlePower + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                            break;
                        }
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("kick", Role = CommandRole.GM)]
        static void Cmd_kick(Client.GameClient client, string[] data)
        {
            try
            {
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Socket == null)
                        continue;
                    if (user.Player.Name.ToLower() == data[1].ToLower())
                    {
                        user.Socket.Disconnect();
                        break;
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("ban", Role = CommandRole.GM, Usage = "@ban name hours")]
        static void Cmd_ban(Client.GameClient client, string[] data)
        {
            try
            {
                uint banHours;
                if (data.Length < 3 || !uint.TryParse(data[2], out banHours))
                {
                    client.SendSysMesage("@ban name hours");
                    return;
                }
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Socket == null)
                        continue;
                    if (user.Player.Name.ToLower() == data[1].ToLower())
                    {

                        Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, banHours);
                        user.SendSysMesage("You Account was Banned by [PM]/[GM].", ChatMode.System, MsgColor.white);
                        user.Socket.Disconnect();
                        break;
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("unbanstr", Role = CommandRole.GM)]
        static void Cmd_unbanstr(Client.GameClient client, string[] data)
        {
            try
            {
                Database.SystemBannedAccount.RemoveBan(data[1]);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("unbanuid", Role = CommandRole.GM)]
        static void Cmd_unbanuid(Client.GameClient client, string[] data)
        {
            try
            {
                Database.SystemBannedAccount.RemoveBan(uint.Parse(data[1]));
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("banip", Role = CommandRole.GM, Usage = "@banip name hours")]
        static void Cmd_banip(Client.GameClient client, string[] data)
        {
            try
            {
                uint banHours;
                if (data.Length < 3 || !uint.TryParse(data[2], out banHours))
                {
                    client.SendSysMesage("@banip name hours");
                    return;
                }
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Socket == null)
                        continue;
                    if (user.Player.Name.ToLower() == data[1].ToLower())
                    {
                        Database.SystemBanned.AddBan(user.Socket.RemoteIp, banHours);
                        user.SendSysMesage("You Ip Address was Banned by [PM]/[GM].", ChatMode.System, MsgColor.white);
                        user.Socket.Disconnect();
                        break;
                    }
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }
    }
}
