using System;

namespace Conquer6609.Framework.Commands
{
    /// <summary>Who may run a command.</summary>
    public enum CommandRole
    {
        /// <summary>Every player.</summary>
        Player = 0,
        /// <summary>client.GameMaster or client.ProjectManager (same gate as the old MsgMessage switch).</summary>
        GM = 1,
        /// <summary>client.ProjectManager only.</summary>
        ProjectManager = 2
    }

    /// <summary>Marks a chat command handler:
    /// <code>[GmCommand("name", "alias", Role = CommandRole.GM, Help = "...")]
    /// public static void Name(Client.GameClient client, string[] args) { ... }</code>
    /// args is the chat text after '@' split on ' ' (args[0] = the command word, args[1] = first argument),
    /// exactly like `data` in MsgMessage.ChatCommands. The attribute may be repeated on the same method.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class GmCommandAttribute : Attribute
    {
        public GmCommandAttribute(params string[] names)
        {
            Names = names ?? new string[0];
            Role = CommandRole.GM;
            Help = "";
            Usage = "";
        }

        /// <summary>Command name followed by its aliases (without '@').</summary>
        public string[] Names { get; private set; }
        public CommandRole Role { get; set; }
        /// <summary>One line shown by @help.</summary>
        public string Help { get; set; }
        /// <summary>Optional usage text, e.g. "@give name itemid".</summary>
        public string Usage { get; set; }
        /// <summary>True = only the command word is written to the audit log (use for passwords / secrets).</summary>
        public bool Sensitive { get; set; }
    }
}
