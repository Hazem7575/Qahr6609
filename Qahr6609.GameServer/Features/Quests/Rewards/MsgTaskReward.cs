using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static class MsgTaskReward
    {
        public static unsafe void GetMsgTaskReward(this ServerSockets.Packet stream, out ActionID Action, out uint dwPram)
        {
            Action = (ActionID)stream.ReadUInt8();
            dwPram = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet MsgTaskRewardCreate(this ServerSockets.Packet stream, ActionID Action, uint dwParam)
        {
            stream.InitWriter();

            stream.Write((byte)Action);
            stream.Write(dwParam);
            stream.Finalize(GamePackets.MsgTaskReward);
            return stream;
        }
        public enum ActionID : byte
        {
            Open = 0,
            SetReward = 1,
            Draw = 2,
            Claim = 3,
            Redraw = 4,
            Continue = 5,
            Times10Spinning = 7
        }
        // Reward table recorded at Draw time; Redraw always uses this table (client table id is ignored).
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, uint> DrawTable = new System.Collections.Concurrent.ConcurrentDictionary<uint, uint>();
        // Conquer points charged for one redraw of a pending reward (original price).
        public const uint RedrawCps = 1;
        // Conquer points charged per spin when paying with the Fortune Wheel (item 3301169).
        // Official: the 10x spin costs exactly 10 single spins (10 x 27 = 270 CPs).
        public const uint SpinCps = 27;

        // Wheel opened by a ticket (UseItem -> OpenWheel): the ticket that may be spent and its reward table.
        // The client table id is accepted only for the FortuneWheel ticket, and never a table of another ticket.
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, KeyValuePair<uint, uint>> OpenedWheel = new System.Collections.Concurrent.ConcurrentDictionary<uint, KeyValuePair<uint, uint>>();
        public const uint FortuneWheelTicket = 3301169;
        static readonly uint[] TicketTables = { 3668, 3651 };// EminentExploitPack, Frozen[Spin]

        public static void OpenWheel(Client.GameClient user, ServerSockets.Packet stream, uint ticket, uint table)
        {
            OpenedWheel[user.Player.UID] = new KeyValuePair<uint, uint>(ticket, table);
            user.Send(stream.MsgTaskRewardCreate(ActionID.Open, table));
        }

        static bool ResolveWheel(Client.GameClient user, uint clientTable, out uint ticket, out uint table)
        {
            ticket = table = 0;
            KeyValuePair<uint, uint> opened;
            if (!OpenedWheel.TryGetValue(user.Player.UID, out opened))
                return false;
            ticket = opened.Key;
            table = opened.Value;
            if (ticket == FortuneWheelTicket)
            {
                if (Array.IndexOf(TicketTables, clientTable) >= 0 || !Database.TaskRewards.Rewards.ContainsKey(clientTable))
                    return false;
                table = clientTable;
            }
            return true;
        }

        [PacketAttribute(GamePackets.MsgTaskReward)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
          //  MyConsole.PrintPacketAdvanced(stream.Memory, stream.Size);

            ActionID Action;
            uint dwParam;
            stream.GetMsgTaskReward(out Action, out dwParam);
            switch (Action)
            {
                case ActionID.Times10Spinning:
                    {
                        uint ticket, table;
                        if (!ResolveWheel(user, dwParam, out ticket, out table))
                            break;
                        if (!user.Inventory.HaveSpace(10))
                        {
                            user.SendSysMesage("Please make 10 more spaces in your inventory.");
                          
                            break;
                        }
                        bool spun = false;
                        for (int x = 0; x < 10; x++)
                        {
                            List<uint> Rewards = new List<uint>();
                            if (Database.TaskRewards.Rewards.TryGetValue(table, out Rewards) && Rewards.Count > 0)
                            {
                                if (ticket == 3600023 && (user.Inventory.Contain(3600023, 1, 0) || user.Inventory.Contain(3600023, 1, 1)))
                                {
                                    user.Player.TaskRewardIndex = Database.TaskRewards.PickIndex(table, Rewards.Count);
                                    user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                    bool boundTicket = !user.Inventory.Contain(3600023, 1, 0);
                                    user.Inventory.Remove(3600023, 1, stream);
                                    user.Inventory.Add(stream, user.Player.TaskReward, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, boundTicket);
                                    spun = true;
                                    user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex));
                                }
                                else if (ticket == 3008733 && (user.Inventory.Contain(3008733, 1, 0) || user.Inventory.Contain(3008733, 1, 1)))
                                {
                                    user.Player.TaskRewardIndex = Database.TaskRewards.PickIndex(table, Rewards.Count);
                                    user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                    bool boundTicket = !user.Inventory.Contain(3008733, 1, 0);
                                    user.Inventory.Remove(3008733, 1, stream);
                                    user.Inventory.Add(stream, user.Player.TaskReward, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, boundTicket);
                                    spun = true;
                                    user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex));

                                }
                                else if (ticket == 3301169 && (user.Inventory.Contain(3301169, 1, 0) || user.Inventory.Contain(3301169, 1, 1)))
                                {
                                    if (user.Player.ConquerPoints >= SpinCps)
                                    {
                                        user.Player.ConquerPoints -= SpinCps;
                                        user.Player.TaskRewardIndex = Database.TaskRewards.PickIndex(table, Rewards.Count);
                                        user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                        bool boundTicket = !user.Inventory.Contain(3301169, 1, 0);
                                        user.Inventory.Remove(3301169, 1, stream);
                                        user.Inventory.Add(stream, user.Player.TaskReward, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, boundTicket);
                                        spun = true;
                                        user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex));
                                    }
                                    else
                                    {
                                        user.SendSysMesage("Failed to use the fortune Wheel since you don't have " + SpinCps + " cps.");
                                        break;
                                    }
                                }
                            }
                        }
                        // Every spin of the 10x wheel is delivered directly; nothing stays pending for Claim.
                        // A pending single draw is only dropped when a 10x spin actually happened.
                        if (spun)
                        {
                            user.Player.TaskReward = 0;
                            user.Player.TaskRewardIndex = 0;
                            uint ignoredTable;
                            DrawTable.TryRemove(user.Player.UID, out ignoredTable);
                        }
                        break;
                    }
                case ActionID.Continue:
                case ActionID.Draw:
                    {
                        uint ticket, table;
                        if (!ResolveWheel(user, dwParam, out ticket, out table))
                            break;
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.SendSysMesage("Please make 1 more space in your inventory.");

                            break;
                        }
                        List<uint> Rewards = new List<uint>();
                        if (Database.TaskRewards.Rewards.TryGetValue(table, out Rewards) && Rewards.Count > 0)
                        {
                            if (ticket == 3600023 && (user.Inventory.Contain(3600023, 1, 0) || user.Inventory.Contain(3600023, 1, 1)))
                            {
                                user.Player.TaskRewardIndex = Database.TaskRewards.PickIndex(table, Rewards.Count);
                                user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                user.Inventory.Remove(3600023, 1, stream);
                                DrawTable[user.Player.UID] = table;
                                user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex));
                            }
                            else if (ticket == 3008733 && (user.Inventory.Contain(3008733, 1, 0) || user.Inventory.Contain(3008733, 1, 1)))
                            {
                                user.Player.TaskRewardIndex = Database.TaskRewards.PickIndex(table, Rewards.Count);
                                user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                user.Inventory.Remove(3008733, 1, stream);
                                DrawTable[user.Player.UID] = table;
                                    user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex));

                            }
                            else if (ticket == 3301169 && (user.Inventory.Contain(3301169, 1, 0) || user.Inventory.Contain(3301169, 1, 1)))
                            {
                                if (user.Player.ConquerPoints >= SpinCps)
                                {
                                    user.Player.ConquerPoints -= SpinCps;
                                    user.Player.TaskRewardIndex = Database.TaskRewards.PickIndex(table, Rewards.Count);
                                    user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                    user.Inventory.Remove(3301169, 1, stream);
                                    DrawTable[user.Player.UID] = table;
                                    user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex));
                                }
                                else
                                {
                                    user.SendSysMesage("Failed to use the fortune Wheel since you don't have " + SpinCps + " cps.");
                                }
                            }
                        }
                        break;
                    }
                case ActionID.Claim:
                    {
                        if (user.Player.TaskReward != 0)
                        {
                            user.SendSysMesage("You received a new item from the roulette prize.", MsgMessage.ChatMode.System);
                          
                            user.Inventory.Add(stream, user.Player.TaskReward, 1, 0,0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            user.Player.TaskReward = 0;
                            uint ignoredTable;
                            DrawTable.TryRemove(user.Player.UID, out ignoredTable);
                        }
                        break;
                    }
                case ActionID.Redraw:
                    {
                        // Redraw requires a pending draw and re-rolls from the table recorded at Draw time.
                        uint drawnTable;
                        if (user.Player.TaskReward == 0 || !DrawTable.TryGetValue(user.Player.UID, out drawnTable))
                            break;
                        if (user.Player.ConquerPoints >= RedrawCps)
                        {

                            if (!user.Inventory.HaveSpace(1))
                            {
                                user.SendSysMesage("Please make 1 more space in your inventory.");
                            
                                break;
                            }
                            List<uint> Rewards = new List<uint>();
                            if (Database.TaskRewards.Rewards.TryGetValue(drawnTable, out Rewards) && Rewards.Count > 0)
                            {
                                user.Player.ConquerPoints -= RedrawCps;
                                user.Player.TaskRewardIndex = Database.TaskRewards.PickIndex(drawnTable, Rewards.Count);
                                user.Player.TaskReward = Rewards.ToArray()[user.Player.TaskRewardIndex];
                                user.Send(stream.MsgTaskRewardCreate(ActionID.SetReward, user.Player.TaskRewardIndex));
                            }
                        }
                        break;
                    }
            }

        }
    }
}
