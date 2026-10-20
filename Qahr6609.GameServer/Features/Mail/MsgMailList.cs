using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.Inbox
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetMailListPage(this ServerSockets.Packet stream, out uint Page)
        {
            Page = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet CreateMailList(this ServerSockets.Packet stream,uint TotalCount, uint ShowCount, uint Page)
        {
            stream.InitWriter();
            stream.Write((byte)0);
            stream.Write(TotalCount);//5
            stream.Write(Page);//9
            stream.Write(ShowCount);//13
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemInboxMail(this ServerSockets.Packet stream, PrizeInfo prize, Client.GameClient user)
        {
            stream.Write(prize.ID);
            stream.Write(prize.Sender, 32);
            stream.Write(prize.Subject, 64);
            stream.Write(prize.goldprize);
            stream.Write(prize.cpsprize);
            stream.Write(prize.JoyBeans);
            stream.Write(prize.Time);
            if (prize.Item != null)
            {
                stream.Write(prize.Item.UID);
                prize.Item.Mode = Role.Flags.ItemMode.Inbox;
                prize.Item.Send(user, stream);
            }
            else
            {
                stream.Write((uint)0);
            }
            stream.Write(prize.Attackment);
            //MyConsole.WriteLine(stream.Dump("MsgMailList"));
            return stream;
        }
        public static unsafe ServerSockets.Packet MsgMailListFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgMailList);
            return stream;
        }
    }
    public unsafe struct MsgMailList
    {
        [PacketAttribute(GamePackets.MsgMailList)]
        public static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.MailBox.Count == 0)
                return;
            try
            {
                uint Page;
                stream.GetMailListPage(out Page);
                //MyConsole.WriteLine(stream.Dump("MsgMailList"));
                // the shown count must match the mails actually written (the last page can hold fewer than 7)
                var ShowMais = user.MailBox.Values.Where(p => p.ID != 0).OrderBy(p => p.ID).Skip((int)(Page * 7)).Take(7).ToArray();
                int take = ShowMais.Length;
                stream.CreateMailList((uint)user.MailBox.Count, (uint)take, Page);
                foreach (var message in ShowMais)
                {
                    if (message.ID == 0)
                        continue;
                    stream.AddItemInboxMail(message, user);
                }
                user.Send(stream.MsgMailListFinalize());
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public static void SendUpdate(Client.GameClient user, ServerSockets.Packet stream)
        {
            /*using (var rect = new ServerSockets.RecycledPacket())
            {
                var ss = rect.GetStream();
                ss.Write((ushort)155);
                ss.Write((ushort)1046);
                ss.ZeroFill(151 - 8);
                user.Send(ss);
            }*/
            if (user.MailBox.Count == 0)
                return;
            uint Page = 0;
            int take = user.MailBox.Count > 7 ? 7 : user.MailBox.Count;
            var ShowMais = user.MailBox.Values.Skip((int)(Page * 7)).Take(take);
            stream.CreateMailList((uint)user.MailBox.Count, (uint)take, Page);
            foreach (var message in ShowMais)
            {
                stream.AddItemInboxMail(message, user);
            }
            user.Send(stream.MsgMailListFinalize());
        }
    }
}
