using System;
using System.Collections.Generic;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {
        // UseItem handler: data-driven packs from ServerDataBase\PackContents.ini (Database/PackContents.cs).
        // It runs after every other UseItem handler, so a pack with its own code is never touched here.
        // Opening checks the bag space for everything first, takes the pack (or one open of a multi-open pack),
        // then gives every reward and lists them in a message box. A bound pack gives bound rewards.
        private static void UseItem_Packs(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            Database.PackContents.Pack pack;
            if (!Database.PackContents.TryGet(item.ITEM_ID, out pack))
            {
                handled = false;
                return;
            }
            handled = true;

            int openIndex = pack.Fees.Length > 0 ? (int)item.PlusProgress : 0;
            uint fee = pack.FeeFor(openIndex);
            if (fee == 0)
            {
                OpenDataPack(client, item, pack, openIndex, stream);
                return;
            }

            string packName = DataPackName(item.ITEM_ID);
            if (client.Player.ConquerPoints < fee)
            {
                client.CreateBoxDialog("Opening the " + packName + " costs " + fee + " CPs. You don`t have enough CPs.");
                return;
            }
            string times = pack.Opens > 0 ? " (" + (openIndex + 1) + "/" + pack.Opens + ")" : "";
            uint uid = item.UID;
            uint packId = item.ITEM_ID;
            client.Player.MessageBox("Pay " + fee + " CPs to open the " + packName + times + "?", user =>
            {
                Game.MsgServer.MsgGameItem again;
                if (!user.Inventory.TryGetItem(uid, out again) || again.ITEM_ID != packId)
                    return;
                Database.PackContents.Pack p;
                if (!Database.PackContents.TryGet(packId, out p))
                    return;
                int index = p.Fees.Length > 0 ? (int)again.PlusProgress : 0;
                if (index != openIndex)
                    return;// opened in the meantime: ask again with the new price
                using (var rec = new ServerSockets.RecycledPacket())
                    OpenDataPack(user, again, p, index, rec.GetStream());
            }, null, 60);
        }

        private static string DataPackName(uint id)
        {
            Database.ItemType.DBItem db;
            if (Database.Server.ItemsBase.TryGetValue(id, out db))
                return db.Name;
            return "pack";
        }

        private static void OpenDataPack(Client.GameClient client, Game.MsgServer.MsgGameItem item, Database.PackContents.Pack pack, int openIndex, ServerSockets.Packet stream)
        {
            bool packBound = item.Bound != 0;
            bool lastOpen = pack.Opens > 0 && openIndex + 1 >= pack.Opens;
            uint fee = pack.FeeFor(openIndex);
            // a random pack gives one of its rewards; it is picked now so the space check covers exactly it
            var rewards = pack.Roll();

            // 1. everything must fit before anything is taken
            int slots = 0;
            foreach (var r in rewards)
            {
                if (r.Kind == Database.PackContents.RewardKind.Item)
                {
                    Database.ItemType.DBItem db;
                    if (!Database.Server.ItemsBase.TryGetValue(r.ItemID, out db))
                    {
                        MyConsole.WriteLine("[PackContents] pack " + pack.ID + " gives unknown item " + r.ItemID);
                        client.CreateBoxDialog("This pack can`t be opened right now.");
                        return;
                    }
                    if (db.StackSize > 1)
                        slots += (r.Count + db.StackSize - 1) / db.StackSize;
                    else
                        slots += r.Count;
                }
                else if (r.Kind == Database.PackContents.RewardKind.Steed)
                    slots += r.Count;
                else if (r.Kind == Database.PackContents.RewardKind.StudyPoints)
                {
                    if (client.Player.SubClass == null || client.Player.SubClass.StudyPoints + r.Amount > ushort.MaxValue)
                    {
                        client.CreateBoxDialog("You can`t hold that many Study Points. Use some first.");
                        return;
                    }
                }
                else if (r.Kind == Database.PackContents.RewardKind.ChiPoints)
                {
                    if (client.Player.MyChi == null)
                    {
                        client.CreateBoxDialog("You have to unlock the Chi system before you can open this pack.");
                        return;
                    }
                }
                else if (r.Kind == Database.PackContents.RewardKind.PotencyPoints)
                {
                    if (client.Player.InnerPower == null)
                    {
                        client.CreateBoxDialog("You have to unlock the Inner Power system before you can open this pack.");
                        return;
                    }
                }
            }
            // the pack's own slot frees up when it is used up and not part of a stack
            int freed = (lastOpen && item.StackSize <= 1) ? 1 : 0;
            int need = Math.Max(0, slots - freed);
            if (need > 40 || !client.Inventory.HaveSpace((byte)need))
            {
                client.CreateBoxDialog("Please make " + need + " more spaces in your inventory.");
                return;
            }
            if (fee > 0 && client.Player.ConquerPoints < fee)
            {
                client.CreateBoxDialog("You don`t have enough CPs.");
                return;
            }

            // 2. take the pack (or one of its opens) and the fee
            string packName = DataPackName(item.ITEM_ID);
            if (lastOpen)
            {
                if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                    return;
            }
            else if (pack.Fees.Length > 0)
            {
                item.PlusProgress = (uint)(openIndex + 1);
                item.Mode = Role.Flags.ItemMode.Update;
                item.Send(client, stream);
            }
            if (fee > 0)
                client.Player.ConquerPoints -= fee;

            // 3. give everything
            var got = new List<string>();
            foreach (var r in rewards)
            {
                bool bound = packBound || r.Bound;
                switch (r.Kind)
                {
                    case Database.PackContents.RewardKind.Item:
                        {
                            Database.ItemType.DBItem db;
                            if (!Database.Server.ItemsBase.TryGetValue(r.ItemID, out db))
                                break;
                            if (db.StackSize > 1)
                                client.Inventory.AddItemWitchStack(r.ItemID, r.Plus, (ushort)r.Count, stream, bound);
                            else
                            {
                                Role.Flags.Gem s1 = r.Sockets >= 1 ? Role.Flags.Gem.EmptySocket : Role.Flags.Gem.NoSocket;
                                Role.Flags.Gem s2 = r.Sockets >= 2 ? Role.Flags.Gem.EmptySocket : Role.Flags.Gem.NoSocket;
                                client.Inventory.Add(stream, r.ItemID, (byte)r.Count, r.Plus, r.Bless, 0, s1, s2, bound, Role.Flags.ItemEffect.None, false, "", r.Days);
                            }
                            got.Add(DescribeItem(r, db.Name, bound));
                            break;
                        }
                    case Database.PackContents.RewardKind.Steed:
                        client.Inventory.AddSteed(stream, r.ItemID, (byte)r.Count, r.Plus, bound, r.Green, r.Blue, r.Red);
                        got.Add((r.Count > 1 ? r.Count + " " : "") + "+" + r.Plus + " Steed" + (bound ? " (B)" : ""));
                        break;
                    case Database.PackContents.RewardKind.Money:
                        client.Player.Money += r.Amount;
                        got.Add(r.Amount.ToString("N0") + " Silver");
                        break;
                    case Database.PackContents.RewardKind.ConquerPoints:
                        client.Player.ConquerPoints += (uint)r.Amount;
                        got.Add(r.Amount.ToString("N0") + " CPs");
                        break;
                    case Database.PackContents.RewardKind.BoundConquerPoints:
                        client.Player.BoundConquerPoints += (int)r.Amount;
                        got.Add(r.Amount.ToString("N0") + " CPs (B)");
                        break;
                    case Database.PackContents.RewardKind.StudyPoints:
                        client.Player.SubClass.AddStudyPoints(client, (ushort)r.Amount, stream);
                        got.Add(r.Amount.ToString("N0") + " Study Points");
                        break;
                    case Database.PackContents.RewardKind.ChiPoints:
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + (int)r.Amount;
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                        got.Add(r.Amount.ToString("N0") + " Chi Points");
                        break;
                    case Database.PackContents.RewardKind.PotencyPoints:
                        client.Player.InnerPower.AddPotency(stream, client, (int)r.Amount);
                        got.Add(r.Amount.ToString("N0") + " Potency Points");
                        break;
                }
            }

            var text = new StringBuilder();
            text.Append("You opened the ").Append(packName).Append(" and received: ").Append(string.Join(", ", got)).Append(".");
            if (pack.Fees.Length > 0 && !lastOpen)
            {
                uint next = pack.FeeFor(openIndex + 1);
                if (pack.Opens > 0)
                    text.Append(" Opens left: ").Append(pack.Opens - openIndex - 1).Append(".");
                if (next > 0)
                    text.Append(" Next open costs ").Append(next).Append(" CPs.");
            }
            client.CreateBoxDialog(text.ToString());
        }

        private static string DescribeItem(Database.PackContents.Reward r, string name, bool bound)
        {
            var sb = new StringBuilder();
            if (r.Count > 1)
                sb.Append(r.Count).Append(" ");
            if (r.Plus > 0 && !(r.ItemID >= 730001 && r.ItemID <= 730009))
                sb.Append("+").Append(r.Plus).Append(" ");
            sb.Append(name);
            if (r.Sockets > 0)
                sb.Append(" ").Append(r.Sockets).Append("-Soc");
            if (r.Bless > 0)
                sb.Append(" -").Append(r.Bless).Append("%");
            if (r.Days > 0)
                sb.Append(" (").Append(r.Days).Append(" days)");
            if (bound)
                sb.Append(" (B)");
            return sb.ToString();
        }
    }
}
