// Mail command: sends a mail (with optional CPs and item attachment) to a player's mailbox, online or offline.
using System;
using System.Collections.Generic;
using Conquer6609.Framework.Commands;

namespace Conquer6609.Commands
{
    internal static class MailCommands
    {
        // @mail name subject|text [cps] [itemId]
        // Subject and text are separated by '|'; spaces are allowed in both (a trailing number is read as
        // cps / itemId, so end the text with something else). '~' is shown as a space too.
        [GmCommand("mail", Role = CommandRole.GM, Help = "send a mail to a player's mailbox", Usage = "@mail name subject|text [cps] [itemId]")]
        static void Cmd_mail(Client.GameClient client, string[] data)
        {
            if (data.Length < 3)
            {
                client.SendSysMesage("@mail name subject|text [cps] [itemId]");
                return;
            }
            List<string> words = new List<string>();
            for (int x = 2; x < data.Length; x++)
                if (data[x].Length > 0)
                    words.Add(data[x]);
            // up to two trailing numbers: cps then itemId (only after the subject|text part)
            List<uint> numbers = new List<uint>();
            while (numbers.Count < 2 && words.Count > 1)
            {
                uint value;
                if (!uint.TryParse(words[words.Count - 1], out value))
                    break;
                numbers.Insert(0, value);
                words.RemoveAt(words.Count - 1);
            }
            uint cps = numbers.Count > 0 ? numbers[0] : 0;
            uint itemId = numbers.Count > 1 ? numbers[1] : 0;
            string body = string.Join(" ", words).Replace('~', ' ');
            string subject, text;
            int bar = body.IndexOf('|');
            if (bar >= 0)
            {
                subject = body.Substring(0, bar).Trim();
                text = body.Substring(bar + 1).Trim();
            }
            else
            {
                subject = body.Trim();
                text = "";
            }
            if (subject.Length == 0)
            {
                client.SendSysMesage("@mail name subject|text [cps] [itemId]");
                return;
            }
            uint uid;
            string name;
            if (!Database.MailService.FindCharacter(data[1], out uid, out name))
            {
                client.SendSysMesage("Player " + data[1] + " was not found.");
                return;
            }
            string error;
            if (!Database.MailService.Send(uid, client.Player.Name, subject, text, 0, cps, itemId, null, out error))
            {
                client.SendSysMesage("Mail not sent: " + error);
                return;
            }
            client.SendSysMesage("Mail sent to " + name + " (" + cps + " CPs" + (itemId != 0 ? ", item " + itemId : "") + ").");
        }
    }
}
