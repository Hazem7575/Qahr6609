// Helpers shared by the GM commands in Qahr6609.GameServer/Commands (migrated from MsgMessage.ChatCommands).
using System;
using System.Collections.Generic;
using Conquer6609.Framework.Commands;

namespace Conquer6609.Commands
{
    internal static class CommandSupport
    {
        const string Namespace = "Conquer6609.Commands.";

        /// <summary>
        /// Same messages as the old try/catch around `switch (data[0])` in MsgMessage.ChatCommands.
        /// Returns true (= exception handled) for malformed / missing arguments; anything else goes on to the
        /// registry's own error handling.
        /// </summary>
        internal static bool ArgumentError(Client.GameClient client, Exception e)
        {
            if (e is IndexOutOfRangeException)
            {
                client.SendSysMesage("Missing command arguments.");
                return true;
            }
            if (e is FormatException || e is OverflowException || e is ArgumentException || e is KeyNotFoundException)
            {
                client.SendSysMesage("Invalid command arguments.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// The old switch matched command names case-sensitively. CommandRegistry.Find also falls back to a
        /// case-insensitive match; for the commands migrated here only the exact spelling counts, so "@Item" stays
        /// an unknown command exactly like before. Commands defined elsewhere (modules, built-ins) keep the
        /// registry's normal lookup.
        /// </summary>
        internal static bool Matches(CommandInfo info, string word)
        {
            if (info == null) return false;
            if (info.Source == null || !info.Source.StartsWith(Namespace, StringComparison.Ordinal)) return true;
            if (string.Equals(info.Name, word, StringComparison.Ordinal)) return true;
            foreach (var a in info.Aliases)
                if (string.Equals(a, word, StringComparison.Ordinal)) return true;
            return false;
        }
    }
}
