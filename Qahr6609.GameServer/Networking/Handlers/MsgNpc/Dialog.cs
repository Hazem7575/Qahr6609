using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;

namespace Conquer6609.Game.MsgNpc
{
    public class Dialog
    {
        private Client.GameClient client;
        public ServerSockets.Packet stream;

        public Dialog(Client.GameClient Client,ServerSockets.Packet _stream)
        {
            stream = _stream;
            client = Client;
        }

        private bool begun;

        private uint PlayerUID
        {
            get { return (client != null && client.Player != null) ? client.Player.UID : 0; }
        }

        // The options offered by this dialog are recorded server-side for the NPC the reply is routed to
        // (client.ActiveNpc). The previous option set is replaced as soon as this dialog shows content;
        // message boxes leave the NPC option set untouched.
        private void BeginState()
        {
            if (begun || client == null) return;
            begun = true;
            global::Conquer6609.Security.NpcDialogState.Begin(PlayerUID, client.ActiveNpc);
        }

        private void RecordOption(byte id)
        {
            if (client == null) return;
            BeginState();
            global::Conquer6609.Security.NpcDialogState.RecordOption(PlayerUID, client.ActiveNpc, id);
        }

        public Dialog CreateMessageBox(string Text)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.MessageBox, Text, ushort.MaxValue, 0, true));
            return this;
        }

        public Dialog AddText(string text)
        {
            BeginState();
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Dialog, text, 0, 0));
            return this;
        }

        public Dialog AddAvatar(ushort id)
        {
            BeginState();
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Avatar, "", id, 0));
            return this;
        }
        public Dialog AddOption(string text, byte id)
        {
            RecordOption(id);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, id));
            return this;
        }
        public Dialog AddOption(string text)
        {
            RecordOption(255);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, 255));
            return this;
        }
        public Dialog AddInput(string text, byte id)
        {
            RecordOption(id);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Input, text, 16, id));
            return this;
        }
        public unsafe void FinalizeDialog(bool messagebox = false)
        {
            if (!messagebox)
                client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Finish, "", 0, 0, false));
        }

    }
}
