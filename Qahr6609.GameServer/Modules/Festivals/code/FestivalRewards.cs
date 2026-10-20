// Paying and charging reward lists (FestivalData.cs: "item:<id>x<n>, exp:<min>, chi:<n>, ..."). Pools are rolled first
// (Resolve), then the bag space is checked for the whole list (HasRoom), then Give. Costs are checked (CanPay) and
// taken (Take) before anything is given.
using System;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.Festivals
{
    public static class FestivalRewards
    {
        static readonly Random rand = new Random();

        public static int Roll(int max) { lock (rand) return rand.Next(max); }
        public static bool Chance(double percent) { lock (rand) return rand.NextDouble() * 100.0 < percent; }

        /// <summary>The list to give: Give140 for a level 140 player when there is one, pools rolled.</summary>
        public static List<Reward> Resolve(FestivalDef f, Client.GameClient c, List<Reward> give, List<Reward> give140)
        {
            var src = give140 != null && c.Player.Level >= 140 ? give140 : give;
            var list = new List<Reward>();
            foreach (var r in src ?? new List<Reward>())
            {
                FPool p;
                if (r.Kind != "pool") { list.Add(r); continue; }
                if (!f.Pools.TryGetValue(r.Pool, out p) || p.Weights.Count == 0) continue;
                int roll = Roll(p.Weights.Sum());
                for (int i = 0; i < p.Weights.Count; i++)
                {
                    if (roll < p.Weights[i]) { list.AddRange(p.Entries[i]); break; }
                    roll -= p.Weights[i];
                }
            }
            return list;
        }

        /// <summary>Free bag slots the list may need (a new stack for each stackable item, a slot per other item).</summary>
        public static int Slots(List<Reward> list)
        {
            int n = 0;
            foreach (var r in list)
            {
                if (r.Kind != "item") continue;
                Database.ItemType.DBItem db;
                if (!Database.Server.ItemsBase.TryGetValue(r.Id, out db)) continue;
                n += db.StackSize > 1 ? (int)((r.Count + db.StackSize - 1) / db.StackSize) : (int)r.Count;
            }
            return n;
        }

        public static bool HasRoom(Client.GameClient c, List<Reward> list)
        {
            int n = Slots(list);
            return n == 0 || c.Inventory.HaveSpace((byte)Math.Min(40, n));
        }

        public static bool CanPay(Client.GameClient c, List<Reward> cost)
        {
            foreach (var r in cost)
                switch (r.Kind)
                {
                    case "item": if (c.Inventory.GetCountItem(r.Id) < r.Count) return false; break;
                    case "cps": if (c.Player.ConquerPoints < r.Count) return false; break;
                    case "bcps": if (c.Player.BoundConquerPoints < r.Count) return false; break;
                    case "silver": if (c.Player.Money < r.Count) return false; break;
                }
            return true;
        }

        /// <summary>Takes the cost (call CanPay first).</summary>
        public static void Take(Client.GameClient c, List<Reward> cost, ServerSockets.Packet stream)
        {
            foreach (var r in cost)
                switch (r.Kind)
                {
                    case "item": TakeItems(c, r.Id, r.Count, stream); break;
                    case "cps": c.Player.ConquerPoints -= r.Count; break;
                    case "bcps": c.Player.BoundConquerPoints -= (int)r.Count; break;
                    case "silver":
                        c.Player.Money -= r.Count;
                        c.Player.SendUpdate(stream, c.Player.Money, MsgUpdate.DataType.Money);
                        break;
                }
        }

        /// <summary>Removes count units of an item from the bag (stacks first decremented, then removed).</summary>
        public static bool TakeItems(Client.GameClient c, uint id, uint count, ServerSockets.Packet stream)
        {
            if (c.Inventory.GetCountItem(id) < count) return false;
            foreach (var item in c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == id).OrderBy(i => i.StackSize).ToList())
            {
                if (count == 0) break;
                uint have = Math.Max(1u, (uint)item.StackSize);
                if (have > count)
                {
                    item.StackSize = (ushort)(have - count);
                    item.Mode = Role.Flags.ItemMode.Update;
                    item.Send(c, stream);
                    count = 0;
                }
                else
                {
                    c.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
                    count -= have;
                }
            }
            return true;
        }

        /// <summary>Gives the (resolved) list; returns the words for the "You received ..." line.</summary>
        public static string Give(Client.GameClient c, List<Reward> list, ServerSockets.Packet stream)
        {
            var got = new List<string>();
            foreach (var r in list)
            {
                switch (r.Kind)
                {
                    case "item":
                        {
                            Database.ItemType.DBItem db;
                            if (!Database.Server.ItemsBase.TryGetValue(r.Id, out db)) continue;
                            if (db.StackSize > 1) c.Inventory.AddItemWitchStack(r.Id, 0, (ushort)r.Count, stream, r.Bound);
                            else c.Inventory.Add(stream, r.Id, (byte)Math.Min(40, r.Count), 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, r.Bound);
                            break;
                        }
                    case "exp":
                        if (c.Player.Level >= 140) continue;
                        c.GainExpBall(r.Count * 10.0, false, Role.Flags.ExperienceEffect.angelwing);
                        break;
                    case "chi":
                        if (c.Player.MyChi == null) { got.Add("(no Chi yet: " + r + " lost)"); continue; }
                        c.Player.MyChi.ChiPoints += (int)r.Count;
                        MsgChiInfo.MsgHandleChi.SendInfo(c, MsgChiInfo.Action.Upgrade);
                        break;
                    case "study":
                        if (c.Player.SubClass == null) continue;
                        c.Player.SubClass.AddStudyPoints(c, (ushort)Math.Min(ushort.MaxValue, r.Count), stream);
                        break;
                    case "cps": c.Player.ConquerPoints += r.Count; break;
                    case "bcps": c.Player.BoundConquerPoints += (int)r.Count; break;
                    case "silver":
                        c.Player.Money += r.Count;
                        c.Player.SendUpdate(stream, c.Player.Money, MsgUpdate.DataType.Money);
                        break;
                    default: continue;
                }
                got.Add(r.ToString());
            }
            return string.Join(", ", got);
        }

        public static string Describe(List<Reward> list)
        {
            return list == null || list.Count == 0 ? "nothing" : string.Join(", ", list.Select(r => r.Kind == "pool" ? "a prize" : r.ToString()));
        }
    }
}
