using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetMailOperation(this ServerSockets.Packet stream, out ushort Act, out uint ID)
        {
            Act = stream.ReadUInt16();
            ID = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet CreateMailContent(this ServerSockets.Packet stream, uint ID, string Message)
        {
            stream.InitWriter();
            stream.Write(ID);
            stream.Write(Message, 64);
            stream.Finalize(1048);
            return stream;
        }
        public static unsafe ServerSockets.Packet CreatMailNotify(this ServerSockets.Packet stream, byte Operation)
        {
            stream.InitWriter();
            stream.Write(Operation);
            stream.ZeroFill(15);
            stream.Finalize(1047);
            return stream;
        }
    }
    public unsafe struct MsgMailOperation
    {
        public enum Action : uint
        {
            Show = 1,
            Delete = 2,
            ClaimMoney = 3,
            ClaimCP = 4,
            ClaimItem = 5,
            ClaimAttach = 6,
            DeleteOpt = 7,
            ClaimOpt = 8
        }
        /// <summary>Claims the requested parts of a mail. Inventory space and counter limits are checked before
        /// anything is taken; the mail is removed only once its contents were delivered, and whatever could not
        /// be delivered stays in the mailbox.</summary>
        private static void Claim(Client.GameClient user, ServerSockets.Packet stream, uint ID, bool money, bool cps, bool attach, bool item)
        {
            PrizeInfo prize;
            if (!user.MailBox.TryGetValue(ID, out prize))
                return;

            bool wantAttach = attach && prize.Attackment != 0;
            bool wantItem = item && prize.Item != null;
            Database.ItemType.DBItem DbItem = null;
            if (wantItem && !Database.Server.ItemsBase.TryGetValue(prize.Item.ITEM_ID, out DbItem))
                wantItem = false;

            byte slots = (byte)((wantAttach ? 1 : 0) + (wantItem ? 1 : 0));
            if (slots > 0 && !user.Inventory.HaveSpace(slots))
            {
                // A full bag only holds back the item parts; gold, CPs and beans are still credited and
                // the items stay in the mail for a later claim.
                if (slots == 2 && user.Inventory.HaveSpace(1))
                    wantItem = false;
                else
                {
                    wantAttach = false;
                    wantItem = false;
                }
                user.SendSysMesage("Clear some stuff of your bag to claim item from inbox.");
            }
            ulong gold = money ? prize.goldprize : 0;
            uint cp = cps ? prize.cpsprize : 0;
            if (gold > 0 && (gold > (ulong)long.MaxValue || user.Player.Money > long.MaxValue - (long)gold))
                return;
            if (cp > 0 && (ulong)user.Player.ConquerPoints + cp > uint.MaxValue)
                return;
            bool beans = money && prize.JoyBeans > 0;
            if (!wantAttach && !wantItem && gold == 0 && cp == 0 && !beans)
                return;

            // Take the mail out atomically so it can only be claimed once.
            if (!user.MailBox.TryRemove(ID, out prize))
                return;
            PrizeInfo rest = prize;

            if (wantAttach)
            {
                if (user.Inventory.Add(stream, prize.Attackment))
                {
                    rest.Attackment = 0;
                    user.SendSysMesage("You recieved item from Inbox.");
                }
            }
            if (wantItem)
            {
                prize.Item.Mode = Role.Flags.ItemMode.AddItem;
                if (user.Inventory.Add(prize.Item, DbItem, stream))
                {
                    prize.Item.SendItemExtra(user, stream);
                    rest.Item = null;
                    user.SendSysMesage("You recieved Attachment from Inbox.");
                }
            }
            if (gold > 0)
            {
                user.Player.Money += (long)gold;
                user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                rest.goldprize = 0;
                user.SendSysMesage("You recieved " + gold + " Silver from Inbox.");
            }
            if (cp > 0)
            {
                user.Player.ConquerPoints += cp;
                rest.cpsprize = 0;
                user.SendSysMesage("You recieved " + cp + " CPS from Inbox.");
            }
            if (beans)
            {
                user.SendSysMesage("You recieved " + prize.JoyBeans + " Beans from Inbox.");
                rest.JoyBeans = 0;
            }

            bool remaining = rest.Attackment != 0 || rest.Item != null || rest.goldprize > 0 || rest.cpsprize > 0 || rest.JoyBeans > 0;
            if (remaining)
                user.MailBox.TryAdd(ID, rest);
            // the mailbox file is rewritten right after a claim; save the character too so what was
            // credited reaches disk with it (a crash in between must not lose the mail and its rewards)
            Database.ServerDatabase.QueueUrgentSave(user);
            if (user.MailBox.Count == 0)
                user.Send(stream.CreatMailNotify(2));
        }

        [PacketAttribute(GamePackets.MsgMailOperation)]
        public static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            ushort Type;
            uint ID;
            stream.GetMailOperation(out Type,out ID);
            PrizeInfo prize;
            switch ((Action)Type)
            {
                case Action.Show:
                    {
                        if (user.MailBox.ContainsKey(ID))
                        {
                            user.Send(stream.CreateMailContent(ID, user.MailBox[ID].Message));
                        }
                        break;
                    }
                case Action.Delete:
                    {
                        if (user.MailBox.ContainsKey(ID))
                        {
                            user.MailBox.TryRemove(ID, out prize);
                            Database.MailService.Save(user);
                            if (user.MailBox.Count == 0)
                            {
                                user.Send(stream.CreatMailNotify(2));
                            }
                        }
                        break;
                    }
                case Action.ClaimMoney:
                    {
                        Claim(user, stream, ID, true, true, true, true);
                        Database.MailService.Save(user);
                        break;
                    }
                case Action.ClaimCP:
                    {
                        Claim(user, stream, ID, false, true, false, false);
                        Database.MailService.Save(user);
                        break;
                    }
                case Action.ClaimItem:
                    {
                        Claim(user, stream, ID, false, false, false, true);
                        Database.MailService.Save(user);
                        break;
                    }
                case Action.ClaimAttach:
                    {
                        Claim(user, stream, ID, false, false, true, false);
                        Database.MailService.Save(user);
                        break;
                    }
                case Action.DeleteOpt:
                case Action.ClaimOpt:
                    {
                        // Multi-select from the mailbox page (client): ushort action, uint 1, uint count,
                        // then count x (uint mailId, uint 0). The client lists at most one page (7 mails).
                        uint count = stream.ReadUInt32();
                        if (count > 7)
                            count = 7;
                        List<uint> ids = new List<uint>();
                        for (uint x = 0; x < count; x++)
                        {
                            uint mailId = stream.ReadUInt32();
                            stream.ReadUInt32();
                            if (mailId != 0 && !ids.Contains(mailId))
                                ids.Add(mailId);
                        }
                        foreach (var mailId in ids)
                        {
                            if ((Action)Type == Action.ClaimOpt)
                                Claim(user, stream, mailId, true, true, true, true);
                            else if (user.MailBox.TryGetValue(mailId, out prize))
                            {
                                // like the client: mails that still hold something to claim are not deleted
                                if (prize.Attackment != 0 || prize.Item != null || prize.goldprize > 0 || prize.cpsprize > 0 || prize.JoyBeans > 0)
                                    continue;
                                user.MailBox.TryRemove(mailId, out prize);
                            }
                        }
                        Database.MailService.Save(user);
                        if ((Action)Type == Action.DeleteOpt)
                        {
                            // reply: the client clears the selection and refreshes the page
                            stream.InitWriter();
                            stream.Write((ushort)Action.DeleteOpt);
                            stream.ZeroFill(16);
                            stream.Finalize(GamePackets.MsgMailOperation);
                            user.Send(stream);
                        }
                        if (user.MailBox.Count == 0)
                            user.Send(stream.CreatMailNotify(2));
                        break;
                    }
                default: MyConsole.WriteLine("[Mailbox] Unknown Action:" + Type); break;
            }
        }
    }
}
