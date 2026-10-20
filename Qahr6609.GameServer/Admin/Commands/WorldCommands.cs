// World / server commands: broadcast, online count, shutdown, NPC and monster spawning.
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

    internal static unsafe class WorldCommands
    {
        [GmCommand("bc", Role = CommandRole.GM)]
        static void Cmd_bc(Client.GameClient client, string[] data)
        {
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(data[1], "ALLUSERS", MsgColor.red, ChatMode.Center).GetArray(stream));
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(data[1], "ALLUSERS", MsgColor.red, ChatMode.BroadcastMessage).GetArray(stream));
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(data[1], "ALLUSERS", MsgColor.red, ChatMode.System).GetArray(stream));
                }
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("addnpc", Role = CommandRole.GM)]
        static void Cmd_addnpc(Client.GameClient client, string[] data)
        {
            try
            {
                Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                np.UID = (uint)Program.GetRandom.Next(10000, 100000);
                np.NpcType = (Role.Flags.NpcType)byte.Parse(data[1]);
                np.Mesh = ushort.Parse(data[2]);
                np.Map = client.Player.Map;//ushort.Parse(data[3]);
                np.X = client.Player.X;//ushort.Parse(data[4]);
                np.Y = client.Player.Y;//ushort.Parse(data[5]);
                client.Map.AddNpc(np);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("addflagnpc", Role = CommandRole.GM)]
        static void Cmd_addflagnpc(Client.GameClient client, string[] data)
        {
            try
            {
                Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                np.UID = uint.Parse(data[1]);
                np.NpcType = (Role.Flags.NpcType)byte.Parse(data[2]);
                np.Mesh = ushort.Parse(data[3]);
                np.Map = client.Player.Map;//ushort.Parse(data[3]);
                np.X = client.Player.X;//ushort.Parse(data[4]);
                np.Y = client.Player.Y;//ushort.Parse(data[5]);
                client.Map.AddNpc(np);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("online", Role = CommandRole.GM)]
        static void Cmd_online(Client.GameClient client, string[] data)
        {
            try
            {
                client.SendSysMesage("Online Players : " + Database.Server.GamePoll.Count + " ", ChatMode.System);
                client.SendSysMesage("Online Players : " + Database.Server.GamePoll.Count + " ");
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("testmoob", Role = CommandRole.GM)]
        static void Cmd_testmoob(Client.GameClient client, string[] data)
        {
            try
            {
                if (client.Map.ContainMobID(uint.Parse(data[1])))
                    return;
                using (var rec = new ServerSockets.RecycledPacket())
                    Database.Server.AddMapMonster(rec.GetStream(), client.Map, uint.Parse(data[1]), client.Player.X, client.Player.Y, ushort.Parse(data[2]), ushort.Parse(data[3]), byte.Parse(data[4]));
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("map", Role = CommandRole.GM)]
        static void Cmd_map(Client.GameClient client, string[] data)
        {
            try
            {
                client.SendSysMesage("MapID = " + client.Player.Map, ChatMode.System);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }

        [GmCommand("exit", Role = CommandRole.GM)]
        static void Cmd_exit(Client.GameClient client, string[] data)
        {
            try
            {
                Program.ProcessConsoleEvent(0);
                return;
            }
            catch (Exception e) when (CommandSupport.ArgumentError(client, e)) { }
        }
    }
}
