using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetLottery(this ServerSockets.Packet stream, out MsgLottery.Action Mode, out Role.Flags.Gem SocketOne
            , out Role.Flags.Gem SocketTwo,out byte Plus,out byte Color,out byte JadesAddeds,out uint ItemID)
        {

            Mode = (MsgLottery.Action)stream.ReadUInt16();
            stream.ReadUInt8();//unknow
            SocketOne = (Role.Flags.Gem)stream.ReadUInt8();
            SocketTwo = (Role.Flags.Gem)stream.ReadUInt8();
            Plus = stream.ReadUInt8();
            Color = stream.ReadUInt8();
            JadesAddeds = stream.ReadUInt8();
            ItemID = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet LotteryCreate(this ServerSockets.Packet stream, MsgLottery.Action Mode, Role.Flags.Gem SocketOne
            , Role.Flags.Gem SocketTwo, byte Plus, byte Color, byte JadesAddeds, uint ItemID)
        {
            stream.InitWriter();

            /*14 00 22 05 
             * 03 02
             * 02 00 
             * 00 00 03 00 D5 D6 30 00      ; "    ÕÖ0 
00 00 00 00 54 51 53 65 72 76 65 72                  ;    TQServer*/
            stream.Write((ushort)Mode);//4
            stream.Write((byte)2);
            stream.Write((byte)SocketOne);
            stream.Write((byte)SocketTwo);
            stream.Write(Plus);
            stream.Write(Color);
            stream.Write(JadesAddeds);
            stream.Write(ItemID);

            stream.Finalize(GamePackets.MsgLottery);
            return stream;
        }


    }
    [StructLayout(LayoutKind.Explicit, Size = 28)]
    public unsafe struct MsgLottery
    {
        public enum Action : uint
        {
            Accept = 0, 
            AddJade = 1, 
            Continue = 2,
            Show =515// 263171

        }
        /// <summary>Re-draws per lottery draw (client strings STR_LOT_INTRO_RESULT_EXCHANGE_0/1/2).</summary>
        const byte MaxReDraws = 2;
        /// <summary>Small Lottery Ticket.</summary>
        public const uint SmallLotteryTicket = 711504;
        /// <summary>Lady Luck (Market) swaps 1 Lottery Ticket (710212) for this many Small Lottery Tickets
        /// (official itemtype text of 710212: "Swap it with Lady Luck (Market 212,188) for 3 Small Lottery Tickets.").</summary>
        public const byte SmallTicketsPerLotteryTicket = 3;
        /// <summary>Lottery Center.</summary>
        public const ushort LotteryMap = 700;

        /// <summary>Opens the client's lottery result window (1314 mode Show) on the pending prize. Also used at login and by
        /// Lady Luck to bring back a prize that was drawn but not accepted before logout.</summary>
        public static void SendShow(Client.GameClient user, ServerSockets.Packet stream)
        {
            var it = user.Player.LotteryItem;
            if (it == null)
                return;
            user.Send(stream.LotteryCreate(Action.Show
                , (it.Sockets > 0) ? Role.Flags.Gem.EmptySocket : Role.Flags.Gem.NoSocket
                , (it.Sockets > 1) ? Role.Flags.Gem.EmptySocket : Role.Flags.Gem.NoSocket
                , it.Plus
                , it.Color
                , user.Player.AddJade
                , it.ID));
        }

        /// <summary>
        /// A paid draw (3 Small Lottery Tickets, one daily chance) from one Lucky Box (LotteryTable.Box*). Used by the
        /// Lucky Boxes (NPC 925-930) and by the window's "Try Again" button (mode 2, same box as the last draw).
        /// Returns false with the client's own message when it cannot draw.
        /// </summary>
        public static bool TryDraw(Client.GameClient user, ServerSockets.Packet stream, string box, out string error)
        {
            error = null;
            if (user.Player.Map != LotteryMap || user.Player.DynamicID != 0)
            {
                error = "You can only draw from the Lottery in the Lottery Center.";
                return false;
            }
            if (user.Player.LotteryEntries >= Database.Server.Lottery.LotteryEntry(user.Player.VipLevel))
            {
                error = "You have used all of your Lottery chances, today!";
                return false;
            }
            var prize = Database.Server.Lottery.GenerateLotteryItem(user, box);
            if (prize == null)// no prizes loaded (lottery.ini missing/broken): don't take the tickets
            {
                error = "The Lucky Box is empty right now. Please try again later.";
                return false;
            }
            if (!user.Inventory.Contain(SmallLotteryTicket, 3) || !user.Inventory.Remove(SmallLotteryTicket, 3, stream))
            {
                error = "You don`t have 3 Small Lottery Tickets to draw from the Lottery.";
                return false;
            }
            if (user.Player.LotteryItem != null)// an un-accepted prize is given up by drawing again
                Database.LotteryTable.Log(user.Player.Name + " (" + user.Player.UID + ") discarded " + user.Player.LotteryItem.Name);

            user.Player.Achievement.CreateFlaID(10118, user, stream);
            user.HeroRewards.AddGoal(306);
            user.Activeness.IncreaseTask(4);
            user.Activeness.IncreaseTask(16);
            user.Activeness.IncreaseTask(28);

            user.Player.LotteryEntries++;
            user.Player.AddJade = 0;
            user.Player.LotteryBox = prize.Box;
            user.Player.LotteryItem = prize;
            Database.LotteryTable.Log(user.Player.Name + " (" + user.Player.UID + ") draw " + user.Player.LotteryEntries + "/" + Database.Server.Lottery.LotteryEntry(user.Player.VipLevel)
                + " " + prize.Box + "Box -> rank " + user.Player.LotteryItem.Rank + " " + user.Player.LotteryItem.Name + " [" + user.Player.LotteryItem.ID + "]");
            SendShow(user, stream);
            return true;
        }

        [PacketAttribute(GamePackets.MsgLottery)]
        private static void Procesor(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgLottery.Action Mode;
            Role.Flags.Gem SocketOne;
            Role.Flags.Gem SocketTwo;
            byte Plus;
            byte Color;
            byte JadesAddeds;
            uint ItemID;

            stream.GetLottery(out Mode, out SocketOne, out SocketTwo, out Plus, out Color, out JadesAddeds, out ItemID);

            switch (Mode)
            {
                case Action.Accept:
                    {
                        var prize = user.Player.LotteryItem;
                        if (prize != null)
                        {
                            if (!user.Inventory.HaveSpace(1))
                            {
                                user.SendSysMesage("Clear your bag and make 1 free space to receive your Lottery item.");
                                break;
                            }
                            // clear first: a second Accept (double click / replay) finds nothing to take
                            user.Player.LotteryItem = null;
                            user.Player.AddJade = 0;

                            var item = Database.Server.Lottery.CreateGameItem(prize);
                            user.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);
                            Database.LotteryTable.Log(user.Player.Name + " (" + user.Player.UID + ") accepted rank " + prize.Rank + " " + prize.Name
                                + " [" + prize.ID + "] uid " + item.UID + " +" + prize.Plus + " sockets " + prize.Sockets);
                            if (prize.Rank < 5)
                            {
                                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations! " + user.Player.Name + " Won " + prize.Name + " in Lottery.", MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));
                                user.SendSysMesage("You won a " + prize.Name + " from the lottery!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }

                            user.Send(stream.LotteryCreate(Mode, SocketOne, SocketTwo, Plus, Color, JadesAddeds, ItemID));
                        }
                        break;
                    }
                case Action.AddJade:
                    {
                        // Official: a re-draw ("add 1 Small Lottery Ticket") needs a drawn, not yet accepted item,
                        // and is allowed at most MaxReDraws times per draw ("That is your fate, accept the item you drew.").
                        if (user.Player.LotteryItem == null)
                            break;
                        if (user.Player.AddJade >= MaxReDraws)
                        {
                            user.SendSysMesage("That is your fate, accept the item you drew.");
                            break;
                        }
                        var old = user.Player.LotteryItem;
                        // the re-draw comes from the same Lucky Box as the draw
                        var redraw = Database.Server.Lottery.GenerateLotteryItem(user, old.Box);
                        if (redraw == null)
                            break;
                        if (user.Inventory.Contain(SmallLotteryTicket, 1) && user.Inventory.Remove(SmallLotteryTicket, 1, stream))
                        {
                            user.Player.AddJade++;
                            user.Player.LotteryItem = redraw;
                            Database.LotteryTable.Log(user.Player.Name + " (" + user.Player.UID + ") re-draw " + user.Player.AddJade + "/" + MaxReDraws + " " + old.Name
                                + " " + redraw.Box + "Box -> rank " + user.Player.LotteryItem.Rank + " " + user.Player.LotteryItem.Name + " [" + user.Player.LotteryItem.ID + "]");
                            SendShow(user, stream);
                        }
                        else
                            user.SendSysMesage("You don`t have 1 Small Lottery Ticket to draw from the Lottery, again.");
                        break;
                    }
                case Action.Continue:
                    {
                        string error;
                        if (!TryDraw(user, stream, user.Player.LotteryBox, out error))
                            user.SendSysMesage(error);
                        break;
                    }
                default:
                    {
                        MyConsole.WriteLine("MsgLottery not found ->" + Mode);
                        break;
                    }
            }
        }
    }
}
