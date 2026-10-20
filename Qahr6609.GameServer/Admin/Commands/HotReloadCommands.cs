// Live reload of the server data files (Infrastructure\HotReload): @reloadnpcs, @reloadportals, @reloadshops,
// @reloaditems, @reloadmonsters, @reloadmapcells <map>, @reloadquests, @reloadall. Project managers only.
// The reload runs off the player's socket thread; the summary comes back as system messages.
// "force" as an argument lets reloadnpcs/reloadall remove more than half of the NPCs of npcs.txt.
using System;
using System.Linq;
using Conquer6609.Framework.Commands;

namespace Conquer6609.Commands
{
    internal static class HotReloadCommands
    {
        static void Run(Client.GameClient client, string[] data)
        {
            string arg = data.Length > 1 ? data[1] : null;
            bool force = data.Skip(1).Any(a => string.Equals(a, "force", StringComparison.OrdinalIgnoreCase));
            HotReload.HotReloadManager.RunForPlayer(client, data[0], arg, force);
        }

        [GmCommand("reloadnpcs", Role = CommandRole.ProjectManager)]
        static void Cmd_reloadnpcs(Client.GameClient client, string[] data) { Run(client, data); }

        [GmCommand("reloadportals", Role = CommandRole.ProjectManager)]
        static void Cmd_reloadportals(Client.GameClient client, string[] data) { Run(client, data); }

        [GmCommand("reloadshops", Role = CommandRole.ProjectManager)]
        static void Cmd_reloadshops(Client.GameClient client, string[] data) { Run(client, data); }

        [GmCommand("reloaditems", Role = CommandRole.ProjectManager)]
        static void Cmd_reloaditems(Client.GameClient client, string[] data) { Run(client, data); }

        [GmCommand("reloadmonsters", Role = CommandRole.ProjectManager)]
        static void Cmd_reloadmonsters(Client.GameClient client, string[] data) { Run(client, data); }

        [GmCommand("reloadmapcells", Role = CommandRole.ProjectManager)]
        static void Cmd_reloadmapcells(Client.GameClient client, string[] data) { Run(client, data); }

        [GmCommand("reloadquests", Role = CommandRole.ProjectManager)]
        static void Cmd_reloadquests(Client.GameClient client, string[] data) { Run(client, data); }

        [GmCommand("reloadall", Role = CommandRole.ProjectManager)]
        static void Cmd_reloadall(Client.GameClient client, string[] data) { Run(client, data); }
    }
}
