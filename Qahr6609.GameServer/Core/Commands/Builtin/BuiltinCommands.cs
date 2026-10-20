// Built-in chat commands of the module framework.
using System;
using System.Text;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Framework.Commands.Builtin
{
    internal static class BuiltinCommands
    {
        [GmCommand("module", Role = CommandRole.GM, Help = "hot reload of a runtime module (ServerDataBase\\Modules\\<Name>)", Usage = "@module reload <Name>")]
        static void ModuleCommand(Client.GameClient client, string[] args)
        {
            if (args.Length < 3 || !args[1].Equals("reload", StringComparison.OrdinalIgnoreCase))
            {
                client.SendSysMesage("@module reload <Name>  (a module in ServerDataBase\\Modules; in development mode saving a .cs file reloads it by itself)");
                return;
            }
            int n = 0;
            foreach (var line in ModuleManager.ReloadRuntime(args[2]))
                if (n++ < 10) client.SendSysMesage(Trim(line, 200));
        }

        [GmCommand("modules", Role = CommandRole.GM, Help = "list loaded modules and their status", Usage = "@modules")]
        static void ModulesCommand(Client.GameClient client, string[] args)
        {
            var mods = ModuleManager.Loaded;
            if (mods.Count == 0)
            {
                client.SendSysMesage("[Modules] no modules loaded.");
                return;
            }
            client.SendSysMesage("[Modules] " + mods.Count + " module(s):");
            foreach (var m in mods)
            {
                var sb = new StringBuilder();
                sb.Append(m.Name).Append(" v").Append(m.Version).Append(" [").Append(m.Status).Append("]");
                if (m.ErrorCount > 0) sb.Append(" errors=").Append(m.ErrorCount);
                if (!string.IsNullOrEmpty(m.Hooks)) sb.Append(" hooks=").Append(m.Hooks);
                client.SendSysMesage(sb.ToString());
                if (m.Faulted && !string.IsNullOrEmpty(m.LastError))
                    client.SendSysMesage("   last error: " + Trim(m.LastError, 150));
            }
        }

        [GmCommand("help", "commands", Role = CommandRole.GM, Help = "list the registered commands you can use", Usage = "@help [command]")]
        static void HelpCommand(Client.GameClient client, string[] args)
        {
            if (args.Length > 1 && args[1].Length > 0)
            {
                var c = CommandRegistry.Find(args[1].TrimStart('@'));
                if (c == null || !CommandRegistry.CanUse(client, c.Role))
                {
                    client.SendSysMesage("No registered command @" + args[1].TrimStart('@') + ".");
                    return;
                }
                client.SendSysMesage(Describe(c));
                if (c.Usage.Length > 0) client.SendSysMesage("   usage: " + c.Usage);
                client.SendSysMesage("   role: " + c.Role);
                return;
            }
            int n = 0;
            foreach (var c in CommandRegistry.Commands)
            {
                if (!CommandRegistry.CanUse(client, c.Role)) continue;
                client.SendSysMesage(Describe(c));
                n++;
            }
            client.SendSysMesage(n + " registered command(s).");
        }

        static string Describe(CommandInfo c)
        {
            var sb = new StringBuilder("@").Append(c.Name);
            if (c.Aliases.Length > 0) sb.Append(" (@").Append(string.Join(", @", c.Aliases)).Append(")");
            if (c.Help.Length > 0) sb.Append(" - ").Append(c.Help);
            return sb.ToString();
        }

        static string Trim(string s, int max)
        {
            return s.Length > max ? s.Substring(0, max) + "..." : s;
        }
    }
}
