using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Role.Instance
{
    public class Trade
    {
        public Client.GameClient Owner;
        public Client.GameClient Target;
        public uint ConquerPoints;
        public long Money;
        public bool WindowOpen;
        public bool Confirmed;

        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> Items;

        public Trade(Client.GameClient _owner)
        {
            Owner = _owner;
            Items = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();
        }

        public bool ItemInTrade(Game.MsgServer.MsgGameItem Dataitem)
        {
            return Items.ContainsKey(Dataitem.UID);
        }

        /// <summary>Single lock for every trade state change (offers, completion, close) so the
        /// two partners' handlers can never interleave on the same escrow.</summary>
        public static readonly object SyncRoot = new object();

        /// <summary>Both sides of the trade must point at each other with an open window.</summary>
        public bool IsMutual()
        {
            var t = Target;
            return t != null && t.MyTrade != null && t.MyTrade.Target == Owner && Owner.MyTrade == this
                && WindowOpen && t.MyTrade.WindowOpen;
        }

        public unsafe void AddConquerPoints(ulong dwParam, ServerSockets.Packet stream)
        {
            lock (SyncRoot)
            {
                if (!IsMutual()) return;
                if (Target.IsVendor || !Target.Socket.Alive || Owner.IsVendor || !Owner.Socket.Alive) return;
                // Amount is validated on the full 64-bit value and must be covered by the current balance.
                long amount;
                if (!Conquer6609.Security.Guard.ValidAmount(dwParam, Conquer6609.Security.Guard.MaxCpsAmount, out amount))
                {
                    Conquer6609.Security.Guard.Audit("trade-cps", Owner.Player.UID, Owner.Player.Name, "rejected amount " + dwParam);
                    return;
                }
                if ((long)Owner.Player.ConquerPoints < amount) return;
                // Total escrow stays within the per-operation CP limit.
                long total = (long)ConquerPoints + amount;
                if (total > Conquer6609.Security.Guard.MaxCpsAmount) return;
                Owner.Player.ConquerPoints -= (uint)amount;
                ConquerPoints = (uint)total;
                Target.Send(stream.TradeCreate((uint)ConquerPoints, MsgTrade.TradeID.DisplayConquerPoints));
            }
        }
        public unsafe void AddMoney(ulong dwParam, ServerSockets.Packet stream)
        {
            lock (SyncRoot)
            {
                if (!IsMutual()) return;
                if (Target.IsVendor || !Target.Socket.Alive || Owner.IsVendor || !Owner.Socket.Alive) return;
                // Amount is validated on the full 64-bit value and must be covered by the current balance.
                long amount;
                if (!Conquer6609.Security.Guard.ValidAmount(dwParam, Conquer6609.Security.Guard.MaxMoneyAmount, out amount))
                {
                    Conquer6609.Security.Guard.Audit("trade-money", Owner.Player.UID, Owner.Player.Name, "rejected amount " + dwParam);
                    return;
                }
                if (Owner.Player.Money < amount) return;
                // Total escrow stays within the per-operation money limit.
                long total = Money + amount;
                if (total > Conquer6609.Security.Guard.MaxMoneyAmount) return;
                Owner.Player.Money -= amount;
                Money = total;
                Target.Send(stream.TradeCreate((ulong)Money, MsgTrade.TradeID.DisplayMoney));
            }
        }
        /// <summary>Every offered item must still be in the inventory and still tradeable.</summary>
        public bool ValidItems()
        {
            foreach (var item in Items.Values)
            {
                Game.MsgServer.MsgGameItem current;
                if (!Owner.Inventory.ClientItems.TryGetValue(item.UID, out current))
                    return false;
                if (current.Bound != 0 || current.Inscribed == 1)
                    return false;
            }
            return true;
        }

        public unsafe void AddItem(ServerSockets.Packet stream, uint dwparam, Game.MsgServer.MsgGameItem DataItem)
        {
            lock (SyncRoot)
            {
            if (IsMutual())
            {
                if (DataItem.Locked != 0)
                {
                    ConcurrentDictionary<uint, Role.Instance.Associate.Member> src;
                    if (!Owner.Player.Associate.Associat.TryGetValue(Role.Instance.Associate.Partener, out src))
                    {
                        Owner.Send(stream.TradeCreate(dwparam, MsgTrade.TradeID.RemoveItem));
                        Owner.SendSysMesage("unable to trade this item.");
                        return;

                    }
                    else if (!src.ContainsKey(Target.Player.UID))
                    {
                        Owner.Send(stream.TradeCreate(dwparam, MsgTrade.TradeID.RemoveItem));
                        Owner.SendSysMesage("unable to trade this item.");
                        return;
                    }
                }
                //Database.ItemType.DBItem DBItem = null;
                //if (Database.Server.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                //{
                //    if (DBItem.Type != Database.ItemType.DBItem.ItemType.Dropable)
                //        return;
                //}
                if (DataItem.Bound >= 1 || DataItem.Inscribed == 1 || Database.ItemType.unabletradeitem.Contains(DataItem.ITEM_ID))
                {

                    Owner.Send(stream.TradeCreate(dwparam, MsgTrade.TradeID.RemoveItem));

                    Owner.SendSysMesage("unable to trade this item.");


                    return;
                }
                if (Target.Inventory.HaveSpace((byte)(Items.Count + 1)))
                {
                    DataItem.Mode = Flags.ItemMode.Trade;
                    DataItem.Send(Target, stream);
                    DataItem.Mode = Flags.ItemMode.AddItem;
                    Items.TryAdd(DataItem.UID, DataItem);
                }
                else
                {
                    Owner.Send(stream.TradeCreate(dwparam, MsgTrade.TradeID.RemoveItem));
                    Owner.SendSysMesage("There is not enough room in your partner inventory.");

                }
            }
            }
        }

        /// <summary>Closes this trade and, when the partner is still paired with us, the partner's side too.
        /// Each side always gets its own escrow (CPs, money, items) back exactly once.</summary>
        public unsafe void CloseTrade()
        {
            lock (SyncRoot)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var msg = rec.GetStream();
                    var partner = Target;
                    Trade partnerTrade = null;
                    if (partner != null && partner.MyTrade != null && partner.MyTrade.Target == Owner)
                        partnerTrade = partner.MyTrade;

                    Owner.Send(msg.TradeCreate(Owner.Player.UID, MsgTrade.TradeID.CloseTradeWindow));
                    Owner.Player.targetTrade = 0;
                    WindowOpen = false;
                    if (Owner.MyTrade == this)
                        Owner.MyTrade = null;
                    DestroyItems(msg);

                    if (partnerTrade != null)
                    {
                        partner.Send(msg.TradeCreate(Owner.Player.UID, MsgTrade.TradeID.CloseTradeWindow));
                        partner.Player.targetTrade = 0;
                        partnerTrade.WindowOpen = false;
                        partner.MyTrade = null;
                        partnerTrade.DestroyItems(msg);
                    }
                }
            }
        }

        /// <summary>Returns the escrow to the owner and empties it, so it can never be paid out twice.</summary>
        public void DestroyItems(ServerSockets.Packet stream)
        {
            lock (SyncRoot)
            {
                uint cps = ConquerPoints;
                long money = Money;
                ConquerPoints = 0;
                Money = 0;
                if (cps > 0)
                    Owner.Player.ConquerPoints += cps;
                if (money > 0)
                {
                    Owner.Player.Money += money;
                    Owner.Player.SendUpdate(stream, Owner.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                }

                foreach (var item in Items.Values)
                {
                    item.Mode = Flags.ItemMode.AddItem;
                    item.Send(Owner, stream);
                }
                Items.Clear();
            }
        }

        /// <summary>Marks the escrow as paid out after a completed trade.</summary>
        public void ClearEscrow()
        {
            lock (SyncRoot)
            {
                ConquerPoints = 0;
                Money = 0;
                Items.Clear();
                WindowOpen = false;
            }
        }

    }
}
