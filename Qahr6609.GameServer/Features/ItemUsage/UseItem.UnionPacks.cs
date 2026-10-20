using System;
using System.Collections.Generic;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {
        // UseItem handlers: Kingdom stipend / allowance items (client item descriptions):
        //  (Rank)StipendPack 3006994-3007007 and OfficialStipendPack 3007435: Chi Points, Horse Racing Points, Study Points;
        //  Supply packs 3006989-3006993: Protection Pills, Favored Training Pills, Chi Points;
        //  (Privilege)...Allowance 3007259-3007263: the matching Supply Pack or Chi Points (the player chooses).
        static readonly int[] StipendChi = { 100, 150, 180, 210, 250, 280, 310, 350, 390, 430, 470, 500, 550, 600 };
        static readonly int[] StipendRace = { 300, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 1000, 1100, 1200 };
        static readonly int[] StipendStudy = { 40, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 180, 200 };

        private static void UseItem_UnionPacks(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            uint id = item.ITEM_ID;
            if (id >= 3006994 && id <= 3007007)
            {
                int i = (int)(id - 3006994);
                OpenUnionPointsPack(item, client, stream, StipendChi[i], StipendRace[i], StipendStudy[i]);
                return;
            }
            switch (id)
            {
                case 3007435: OpenUnionPointsPack(item, client, stream, 200, 600, 140); return;// OfficialStipendPack
                case 3006989: OpenSupplyPack(item, client, stream, 10, 10, 1500); return;// EmperorSupplyPack
                case 3006990: OpenSupplyPack(item, client, stream, 8, 8, 1000); return;// RoyalSupplyPack
                case 3006991: OpenSupplyPack(item, client, stream, 5, 5, 700); return;// NoblesSupplyPack
                case 3006992: OpenSupplyPack(item, client, stream, 2, 3, 500); return;// GuardSupplyPack
                case 3006993: OpenSupplyPack(item, client, stream, 6, 6, 800); return;// SeniorConcubineSupplyPack
                case 3007259: OpenAllowance(item, client, 3006989, 2000); return;// EmperorAllowance
                case 3007260: OpenAllowance(item, client, 3006990, 1600); return;// RoyalAllowance
                case 3007261: OpenAllowance(item, client, 3006991, 1000); return;// NoblesAllowance
                case 3007262: OpenAllowance(item, client, 3006992, 600); return;// GuardAllowance
                case 3007263: OpenAllowance(item, client, 3006993, 1200); return;// SeniorConcubineAllowance
            }
            handled = false;
        }

        static string UnionItemName(uint id)
        {
            Database.ItemType.DBItem db;
            return Database.Server.ItemsBase.TryGetValue(id, out db) ? db.Name : "pack";
        }

        static void AddChiPoints(Client.GameClient client, int amount)
        {
            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + amount;
            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
        }

        static void OpenUnionPointsPack(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, int chi, int race, int study)
        {
            if (client.Player.MyChi == null)
            {
                client.CreateBoxDialog("You have to unlock the Chi system before you can open this pack.");
                return;
            }
            if (client.Player.SubClass == null || client.Player.SubClass.StudyPoints + study > ushort.MaxValue)
            {
                client.CreateBoxDialog("You can`t hold that many Study Points. Use some first.");
                return;
            }
            string name = UnionItemName(item.ITEM_ID);
            if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                return;
            AddChiPoints(client, chi);
            client.Player.RacePoints += (uint)race;
            client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
            client.Player.SubClass.AddStudyPoints(client, (ushort)study, stream);
            client.CreateBoxDialog("You opened the " + name + " and received " + chi + " Chi Points, " + race + " Horse Racing Points and " + study + " Study Points.");
        }

        static void OpenSupplyPack(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, int protectionPills, int favoredPills, int chi)
        {
            if (client.Player.MyChi == null)
            {
                client.CreateBoxDialog("You have to unlock the Chi system before you can open this pack.");
                return;
            }
            const uint ProtectionPill = 3002029, FavoredTrainingPill = 3003124;
            Database.ItemType.DBItem pillDb, favoredDb;
            if (!Database.Server.ItemsBase.TryGetValue(ProtectionPill, out pillDb) || !Database.Server.ItemsBase.TryGetValue(FavoredTrainingPill, out favoredDb))
            {
                client.CreateBoxDialog("This pack can`t be opened right now.");
                return;
            }
            int slots = (pillDb.StackSize > 1 ? (protectionPills + pillDb.StackSize - 1) / pillDb.StackSize : protectionPills)
                + (favoredDb.StackSize > 1 ? (favoredPills + favoredDb.StackSize - 1) / favoredDb.StackSize : favoredPills);
            int need = Math.Max(0, slots - 1);// the pack's own slot frees up
            if (!client.Inventory.HaveSpace((byte)need))
            {
                client.CreateBoxDialog("Please make " + need + " more spaces in your inventory.");
                return;
            }
            bool bound = item.Bound != 0;
            string name = UnionItemName(item.ITEM_ID);
            if (!client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                return;
            if (pillDb.StackSize > 1)
                client.Inventory.AddItemWitchStack(ProtectionPill, 0, (ushort)protectionPills, stream, bound);
            else
                client.Inventory.Add(stream, ProtectionPill, (byte)protectionPills, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound);
            if (favoredDb.StackSize > 1)
                client.Inventory.AddItemWitchStack(FavoredTrainingPill, 0, (ushort)favoredPills, stream, bound);
            else
                client.Inventory.Add(stream, FavoredTrainingPill, (byte)favoredPills, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound);
            AddChiPoints(client, chi);
            client.CreateBoxDialog("You opened the " + name + " and received " + protectionPills + " Protection Pills, " + favoredPills
                + " Favored Training Pills and " + chi + " Chi Points.");
        }

        /// <summary>"Open to claim a ... Supply Pack or N Chi Points": OK takes the Supply Pack, Cancel the Chi Points.</summary>
        static void OpenAllowance(Game.MsgServer.MsgGameItem item, Client.GameClient client, uint supplyPack, int chi)
        {
            uint uid = item.UID, allowanceId = item.ITEM_ID;
            string packName = UnionItemName(supplyPack);
            client.Player.MessageBox("Press OK to claim the " + packName + ", or Cancel to claim " + chi + " Chi Points.", user =>
            {
                Game.MsgServer.MsgGameItem again;
                if (!user.Inventory.TryGetItem(uid, out again) || again.ITEM_ID != allowanceId)
                    return;
                bool bound = again.Bound != 0;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var s = rec.GetStream();
                    if (!user.Inventory.Update(again, Role.Instance.AddMode.REMOVE, s))
                        return;
                    user.Inventory.Add(s, supplyPack, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, bound);
                }
                user.SendSysMesage("You received a " + packName + ".", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
            }, user =>
            {
                Game.MsgServer.MsgGameItem again;
                if (!user.Inventory.TryGetItem(uid, out again) || again.ITEM_ID != allowanceId)
                    return;
                if (user.Player.MyChi == null)
                {
                    user.CreateBoxDialog("You have to unlock the Chi system before you can take Chi Points.");
                    return;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    if (!user.Inventory.Update(again, Role.Instance.AddMode.REMOVE, rec.GetStream()))
                        return;
                }
                AddChiPoints(user, chi);
                user.SendSysMesage("You received " + chi + " Chi Points.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
            }, 60);
        }
    }
}
