// Developer tools for testing (development mode only, GMs only).
//   @maxme       your character at its best: master class, 2nd reborn (130/130), level 140, attribute points spent, every skill
//                of the class at its top level, weapon proficiency 20, a full set of the best gear (+12, -7, 255 HP, super gems),
//                a +12 steed, money, CPs and VIP 7. The gear goes to the bag: equip it yourself.
//   @maxgear     every item you wear (or, if you wear nothing, the gear in your bag) at its best: +12, -7, 255 HP, two super gems,
//                P7 soul (permanent), the best Sacred refinery (permanent), perfection level 54 (max stars).
//                "@maxgear bag" does the gear in the bag too.
//   @maxchi      the 4 Chi powers (Dragon, Phoenix, Tiger, Turtle) unlocked, 4 epic attributes each (score 100 each).
//   @maxjianghu  Jiang Hu opened, all 9 stages, 9 stars each at level 6, one aligned attribute per stage, talent 5.
//   @maxinner    Inner Power: every stage and every Nei Gong unlocked, top level, Perfect with score 100.
//   @maxpoints   all attribute points spent: strength and agility 256 (more if the gear needs it), the rest vitality.
//   @maxextras   the 7 sub classes at level 9 / phase 9 (Martial Artist active), nobility King (top donation).
//   @maxall      all of the above (level/reborn/skills too; a gear set only if you have no gear at all).
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Modules.DevTools
{
    public sealed class DevToolsModule : ModuleBase
    {
        public override string Name { get { return "DevTools"; } }

        static bool DevOnly(Client.GameClient client, string cmd)
        {
            if (ServerMode.IsDevelopment) return true;
            client.SendSysMesage("@" + cmd + " works only in development mode.");
            return false;
        }

        static void Report(Client.GameClient client, string tag, List<string> lines)
        {
            foreach (var line in lines)
                client.SendSysMesage("[" + tag + "] " + line);
        }

        // ------------------------------------------------------------------ @maxme
        [GmCommand("maxme", Role = CommandRole.GM, Help = "development: your character at its best (level, reborn, points, skills, gear)")]
        static void MaxMe(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "maxme")) return;
            var report = new List<string>();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MaxCharacter(client, stream, report);
                GiveGearSet(client, stream, report);
                MaxProficiency(client, stream);
            }
            Report(client, "maxme", report);
            client.SendSysMesage("[maxme] done. Log out and in once so every window shows the new character.");
        }

        // class, reborns, level, points, skills, money
        static void MaxCharacter(Client.GameClient client, ServerSockets.Packet stream, List<string> report)
        {
            var p = client.Player;
            byte master = Master(p.Class);
            p.Class = master;
            p.FirstClass = master;
            p.SecoundeClass = master;
            p.FirstRebornLevel = 130;
            p.SecoundeRebornLevel = 130;
            p.Reborn = 2;
            p.SendUpdate(stream, p.Reborn, MsgUpdate.DataType.Reborn);
            if (p.Level < 140)
                client.UpdateLevel(stream, 140, true);
            report.Add("class " + master + ", 2nd reborn, level " + p.Level);

            SpendPoints(client, stream, report);

            // every skill of the class at its top level
            int skills = 0;
            foreach (ushort id in ClassSkills(p.Class))
            {
                Dictionary<ushort, Database.MagicType.Magic> levels;
                if (!Database.Server.Magic.TryGetValue(id, out levels) || levels.Count == 0) continue;
                ushort top = levels.Keys.Max();
                client.MySpells.Add(stream, id, (byte)top, 0, 0, 0);
                skills++;
            }
            // Riding: needed to mount the steed (and to join the horse race); every class learns it
            if (!client.MySpells.ClientSpells.ContainsKey(7001)) { client.MySpells.Add(stream, 7001, 0, 0, 0, 0); skills++; }
            report.Add(skills + " skill(s) at their top level");

            p.Money = Math.Max(p.Money, 1000000000);
            p.SendUpdate(stream, p.Money, MsgUpdate.DataType.Money);
            p.ConquerPoints = Math.Max(p.ConquerPoints, 10000000u);
            p.VipLevel = 7;
            p.SendUpdate(stream, p.VipLevel, MsgUpdate.DataType.VIPLevel);
            report.Add("1,000,000,000 silver, 10,000,000 CPs, VIP 7");
        }

        // gear: the best super item the class can wear in every slot, to the bag
        static void GiveGearSet(Client.GameClient client, ServerSockets.Packet stream, List<string> report)
        {
            var given = new List<string>();
            foreach (var slot in new[] { Role.Flags.ConquerItem.Head, Role.Flags.ConquerItem.Necklace, Role.Flags.ConquerItem.Armor, Role.Flags.ConquerItem.Ring,
                Role.Flags.ConquerItem.Boots, Role.Flags.ConquerItem.RightWeapon, Role.Flags.ConquerItem.LeftWeapon, Role.Flags.ConquerItem.Fan,
                Role.Flags.ConquerItem.Tower, Role.Flags.ConquerItem.RidingCrop, Role.Flags.ConquerItem.Wing })
            {
                var best = Best(client, slot);
                if (best == null) continue;
                var gem = GemFor((ushort)slot);
                if (!client.Inventory.HaveSpace(1)) { report.Add("bag full: stopped at " + slot); break; }
                client.Inventory.Add(stream, best.ID, 1, 12, 7, 255, gem, gem, false);
                given.Add(best.Name);
            }
            if (client.Inventory.HaveSpace(1))
            {
                client.Inventory.Add(stream, 300000, 1, 12, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                given.Add("+12 Steed");
            }
            report.Add("gear in the bag: " + string.Join(", ", given));
        }

        // weapon proficiency 20 for every weapon worn or in the bag
        static void MaxProficiency(Client.GameClient client, ServerSockets.Packet stream)
        {
            var done = new HashSet<ushort>();
            foreach (var it in client.Inventory.ClientItems.Values.Concat(client.Equipment.ClientItems.Values).ToArray())
            {
                ushort pos = Database.ItemType.ItemPosition(it.ITEM_ID);
                if (pos != (ushort)Role.Flags.ConquerItem.RightWeapon && pos != (ushort)Role.Flags.ConquerItem.LeftWeapon) continue;
                ushort prof = (ushort)(it.ITEM_ID / 1000);
                if (Database.ItemType.IsShield(it.ITEM_ID) || !done.Add(prof)) continue;
                client.MyProfs.Add(stream, prof, 20, 0);
            }
        }

        // ------------------------------------------------------------------ @maxpoints
        [GmCommand("maxpoints", Role = CommandRole.GM, Help = "development: all attribute points spent (str/agi 256 or what the gear needs, rest vitality)")]
        static void MaxPoints(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "maxpoints")) return;
            var report = new List<string>();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                SpendPoints(client, rec.GetStream(), report);
                client.Equipment.QueryEquipment(client.Equipment.Alternante);
            }
            Report(client, "maxpoints", report);
        }

        // what a 2nd reborn of this level has, spent for the class: strength and agility 256 (raised to what the worn/bag gear
        // asks for), taoists 50/50 and spirit; the rest vitality
        static void SpendPoints(Client.GameClient client, ServerSockets.Packet stream, List<string> report)
        {
            var p = client.Player;
            int points = Database.Server.RebornInfo.ExtraAtributePoints(p.FirstRebornLevel, p.FirstClass)
                + Database.Server.RebornInfo.ExtraAtributePoints(p.SecoundeRebornLevel, p.SecoundeClass) + 52 + 3 * (p.Level - 15) + p.ExtraAtributes;
            if (p.Reborn < 2)
                points = p.Strength + p.Agility + p.Vitality + p.Spirit + p.Atributes;
            bool taoist = Database.AtributesStatus.IsTaoist(p.Class);
            int needStr = 0, needAgi = 0;
            foreach (var it in client.Equipment.ClientItems.Values.Concat(client.Inventory.ClientItems.Values).ToArray())
            {
                Database.ItemType.DBItem db;
                if (!it.IsEquip || !Database.Server.ItemsBase.TryGetValue(it.ITEM_ID, out db)) continue;
                if (!Database.ItemType.EquipPassJobReq(db, client)) continue;
                needStr = Math.Max(needStr, db.Strength);
                needAgi = Math.Max(needAgi, db.Agility);
            }
            int str = Math.Min(points, Math.Max(taoist ? 50 : 256, needStr));
            int agi = Math.Min(points - str, Math.Max(taoist ? 50 : 256, needAgi));
            int spi = taoist ? Math.Min(points - str - agi, 600) : 0;
            p.Strength = (ushort)str;
            p.Agility = (ushort)agi;
            p.Spirit = (ushort)spi;
            p.Vitality = (ushort)(points - str - agi - spi);
            p.Atributes = 0;
            p.SendUpdate(stream, p.Strength, MsgUpdate.DataType.Strength);
            p.SendUpdate(stream, p.Agility, MsgUpdate.DataType.Agility);
            p.SendUpdate(stream, p.Spirit, MsgUpdate.DataType.Spirit);
            p.SendUpdate(stream, p.Vitality, MsgUpdate.DataType.Vitality);
            p.SendUpdate(stream, p.Atributes, MsgUpdate.DataType.Atributes);
            report.Add(points + " attribute points (str " + str + ", agi " + agi + ", spi " + spi + ", vit " + p.Vitality
                + "); the gear needs str " + needStr + ", agi " + needAgi);
        }

        // ------------------------------------------------------------------ @maxgear
        [GmCommand("maxgear", Role = CommandRole.GM, Help = "development: worn gear (or bag gear) +12, -7, 255 HP, super gems, P7 soul, Sacred refinery, perfection 54", Usage = "@maxgear [bag = also the gear in the bag]")]
        static void MaxGear(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "maxgear")) return;
            var report = new List<string>();
            bool bag = data.Length > 1 && data[1].Equals("bag", StringComparison.OrdinalIgnoreCase);
            using (var rec = new ServerSockets.RecycledPacket())
                MaxGearPart(client, rec.GetStream(), report, bag);
            Report(client, "maxgear", report);
        }

        static readonly ushort[] SoulSlots = { 1, 2, 3, 4, 5, 6, 8 };// head, necklace, armor, weapons, ring, boots

        // the slot an item is for: where it is worn (alternate slots 21-29 count as 1-9), else what itemtype says
        static ushort SlotOf(MsgGameItem item)
        {
            ushort pos = item.Position;
            if (pos > 20 && pos < 30) pos -= 20;
            return pos != 0 ? pos : Database.ItemType.ItemPosition(item.ITEM_ID);
        }

        static void MaxGearPart(Client.GameClient client, ServerSockets.Packet stream, List<string> report, bool withBag = false)
        {
            var items = client.Equipment.ClientItems.Values.Where(i => GearSlot(SlotOf(i))).OrderBy(i => i.Position).ToList();
            string where = "worn";
            if (items.Count == 0 || withBag)
            {
                where = items.Count == 0 ? "in the bag" : "worn and in the bag";
                items.AddRange(client.Inventory.ClientItems.Values.Where(i => GearSlot(SlotOf(i))));
            }
            if (items.Count == 0)
            {
                report.Add("no gear worn or in the bag: run @maxme first.");
                return;
            }
            int souls = 0, refines = 0;
            foreach (var item in items)
            {
                ushort pos = SlotOf(item);
                var parts = new List<string>();
                bool steed = pos == (ushort)Role.Flags.ConquerItem.Steed;
                item.Plus = 12;
                if (!steed)
                {
                    item.Bless = 7;
                    item.Enchant = 255;
                }
                parts.Add("+12" + (steed ? "" : " -7 255HP"));
                var gem = GemFor(pos);
                if (gem != Role.Flags.Gem.NoSocket)
                {
                    item.SocketOne = gem;
                    item.SocketTwo = gem;
                    parts.Add("2x " + gem);
                }

                // P7 soul (permanent = stabilized), as MsgItemExtra.AddExtra / Stabilization do
                if (SoulSlots.Contains(pos))
                {
                    var soul = BestSoul(pos, item.ITEM_ID);
                    if (soul != null)
                    {
                        item.Purification = new MsgItemExtra.Purification();
                        item.Purification.AddedOn = DateTime.Now;
                        item.Purification.ItemUID = item.UID;
                        item.Purification.PurificationLevel = soul.PurificationLevel;
                        item.Purification.PurificationDuration = 0;
                        item.Purification.PurificationItemID = soul.ID;
                        item.Purification.Typ = MsgItemExtra.Typing.PurificationEffect;
                        var effect = new MsgItemExtra();
                        effect.Purifications.Add(item.Purification);
                        client.Send(effect.CreateArray(stream));
                        parts.Add("P" + soul.PurificationLevel + " " + soul.Name);
                        souls++;
                    }
                    else parts.Add("no soul fits");
                }

                // refinery (permanent)
                var refine = BestRefinery(pos, item.ITEM_ID);
                if (refine != null)
                {
                    item.Refinary = new MsgItemExtra.Refinery();
                    item.Refinary.AddedOn = DateTime.Now;
                    item.Refinary.Available = true;
                    item.Refinary.ItemUID = item.UID;
                    item.Refinary.EffectLevel = refine.Level;
                    item.Refinary.EffectDuration = 0;
                    item.Refinary.EffectID = refine.ItemID;
                    item.Refinary.EffectPercent = refine.Procent;
                    item.Refinary.EffectPercent2 = refine.Procent2;
                    item.Refinary.Typ = MsgItemExtra.Typing.PermanentRefinery;
                    parts.Add("refinery " + refine.Type + " " + refine.Procent + " (L" + refine.Level + ")");
                    refines++;
                }

                // perfection: the top star level the server knows (MsgItemRefineOpt caps at 54)
                uint maxStars = (uint)Math.Min(54, Database.ItemRefineUpgrade.ProgresUpdates.Count);
                item.PerfectionLevel = maxStars;
                item.PerfectionProgress = 0;
                item.OwnerUID = client.Player.UID;
                item.OwnerName = client.Player.Name;
                parts.Add("perfection " + maxStars);

                item.Mode = Role.Flags.ItemMode.Update;
                item.Send(client, stream);
                report.Add(ItemName(item.ITEM_ID) + " (" + (Role.Flags.ConquerItem)pos + (item.Position == 0 ? ", bag" : "") + "): " + string.Join(", ", parts));
            }
            client.UpdatePerfectionLevel(stream);
            client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
            MaxProficiency(client, stream);
            report.Add(items.Count + " item(s) " + where + " done: " + souls + " soul(s), " + refines + " refinery(ies), weapon proficiency 20.");
        }

        static bool GearSlot(ushort pos)
        {
            switch ((Role.Flags.ConquerItem)pos)
            {
                case Role.Flags.ConquerItem.Head:
                case Role.Flags.ConquerItem.Necklace:
                case Role.Flags.ConquerItem.Armor:
                case Role.Flags.ConquerItem.RightWeapon:
                case Role.Flags.ConquerItem.LeftWeapon:
                case Role.Flags.ConquerItem.Ring:
                case Role.Flags.ConquerItem.Boots:
                case Role.Flags.ConquerItem.Fan:
                case Role.Flags.ConquerItem.Tower:
                case Role.Flags.ConquerItem.Steed:
                case Role.Flags.ConquerItem.RidingCrop:
                case Role.Flags.ConquerItem.Wing:
                    return true;
            }
            return false;
        }

        static Role.Flags.Gem GemFor(ushort pos)
        {
            switch ((Role.Flags.ConquerItem)pos)
            {
                case Role.Flags.ConquerItem.Fan: return Role.Flags.Gem.SuperThunderGem;
                case Role.Flags.ConquerItem.Tower: return Role.Flags.Gem.SuperGloryGem;
                case Role.Flags.ConquerItem.Steed:
                case Role.Flags.ConquerItem.RidingCrop:
                case Role.Flags.ConquerItem.Wing: return Role.Flags.Gem.NoSocket;
                default: return Role.Flags.Gem.SuperDragonGem;
            }
        }

        // the highest purification level soul for the slot (the soul kinds ItemType.CompareSoul matches, decided by the slot
        // the item is worn in: some ids, e.g. the Windrobes 1013x3-1013x9, are outside the armor ranges of ItemType)
        static Database.ItemType.DBItem BestSoul(ushort pos, uint itemId)
        {
            Role.Flags.SoulTyp want;
            switch ((Role.Flags.ConquerItem)pos)
            {
                case Role.Flags.ConquerItem.Head: want = Role.Flags.SoulTyp.Headgear; break;
                case Role.Flags.ConquerItem.Necklace: want = Role.Flags.SoulTyp.Necklace; break;
                case Role.Flags.ConquerItem.Armor: want = Role.Flags.SoulTyp.Armor; break;
                case Role.Flags.ConquerItem.Ring: want = Role.Flags.SoulTyp.Ring; break;
                case Role.Flags.ConquerItem.Boots: want = Role.Flags.SoulTyp.Boots; break;
                default:
                    // no weapon soul: the client draws a weapon soul's own model instead of the weapon (a katana showed as a fan)
                    return null;
                    want = Database.ItemType.GetItemSoulTYPE(itemId); break;
            }
            foreach (var level in Database.ItemType.PurificationItems.Keys.OrderByDescending(k => k))
            {
                var fit = Database.ItemType.PurificationItems[level].Values
                    .Where(s => Database.ItemType.GetSoulPosition(s.ID) == want).OrderByDescending(s => s.ID).FirstOrDefault();
                if (fit != null) return fit;
            }
            return null;
        }

        // refinery: the highest level for the slot, then the most useful effect for a melee class
        static readonly Database.Rifinery.RefineryType[] RefineOrder = {
            Database.Rifinery.RefineryType.CriticalStrike, Database.Rifinery.RefineryType.Intensification, Database.Rifinery.RefineryType.Immunity,
            Database.Rifinery.RefineryType.Break, Database.Rifinery.RefineryType.Counteraction, Database.Rifinery.RefineryType.Penetration,
            Database.Rifinery.RefineryType.SkillCriticalStrike, Database.Rifinery.RefineryType.Block, Database.Rifinery.RefineryType.MDefence,
            Database.Rifinery.RefineryType.Detoxication };

        static Database.Rifinery.Item BestRefinery(ushort pos, uint itemId)
        {
            bool twoHand = Database.ItemType.IsTwoHand(itemId);
            bool bow = Database.ItemType.IsBow(itemId);
            bool backsword = itemId / 1000 == 421;
            bool shield = Database.ItemType.IsShield(itemId);
            if (pos == (ushort)Role.Flags.ConquerItem.LeftWeapon && !shield) pos = (ushort)Role.Flags.ConquerItem.RightWeapon;// a 1-handed weapon in the left hand
            if (pos == (ushort)Role.Flags.ConquerItem.RightWeapon && shield) pos = (ushort)Role.Flags.ConquerItem.LeftWeapon;
            Database.Rifinery.Item best = null;
            foreach (var r in Database.Server.RifineryItems.Values)
            {
                if (r.ForItemPosition != pos || r.Procent == 0) continue;
                string n = r.Name.ToLowerInvariant();
                if (n == "2-handed" && (!twoHand || bow)) continue;
                if (n == "1-handed" && (twoHand || backsword)) continue;
                if (n == "bow" && !bow) continue;
                if (n == "backsword" && !backsword) continue;
                if (n == "hossu" || (n == "shield" && !shield)) continue;
                if (best == null || Better(r, best)) best = r;
            }
            return best;
        }

        static bool Better(Database.Rifinery.Item a, Database.Rifinery.Item b)
        {
            if (a.Level != b.Level) return a.Level > b.Level;
            int ia = Array.IndexOf(RefineOrder, a.Type), ib = Array.IndexOf(RefineOrder, b.Type);
            if (ia < 0) ia = 99;
            if (ib < 0) ib = 99;
            if (ia != ib) return ia < ib;
            return a.Procent > b.Procent;
        }

        static string ItemName(uint id)
        {
            Database.ItemType.DBItem db;
            return Database.Server.ItemsBase.TryGetValue(id, out db) ? db.Name : id.ToString();
        }

        // ------------------------------------------------------------------ @maxchi
        [GmCommand("maxchi", Role = CommandRole.GM, Help = "development: the 4 Chi powers unlocked with 4 epic attributes each")]
        static void MaxChi(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "maxchi")) return;
            var report = new List<string>();
            MaxChiPart(client, report);
            Report(client, "maxchi", report);
        }

        static readonly Dictionary<MsgChiInfo.ChiPowerType, Role.Instance.Chi.ChiAttributeType[]> ChiPlan = new Dictionary<MsgChiInfo.ChiPowerType, Role.Instance.Chi.ChiAttributeType[]>
        {
            { MsgChiInfo.ChiPowerType.Dragon, new[] { Role.Instance.Chi.ChiAttributeType.CriticalStrike, Role.Instance.Chi.ChiAttributeType.SkillCriticalStrike, Role.Instance.Chi.ChiAttributeType.FinalAttack, Role.Instance.Chi.ChiAttributeType.MaxLife } },
            { MsgChiInfo.ChiPowerType.Phoenix, new[] { Role.Instance.Chi.ChiAttributeType.CriticalStrike, Role.Instance.Chi.ChiAttributeType.SkillCriticalStrike, Role.Instance.Chi.ChiAttributeType.AddAttack, Role.Instance.Chi.ChiAttributeType.FinalAttack } },
            { MsgChiInfo.ChiPowerType.Tiger, new[] { Role.Instance.Chi.ChiAttributeType.CriticalStrike, Role.Instance.Chi.ChiAttributeType.SkillCriticalStrike, Role.Instance.Chi.ChiAttributeType.Breakthrough, Role.Instance.Chi.ChiAttributeType.Immunity } },
            { MsgChiInfo.ChiPowerType.Turtle, new[] { Role.Instance.Chi.ChiAttributeType.CriticalStrike, Role.Instance.Chi.ChiAttributeType.SkillCriticalStrike, Role.Instance.Chi.ChiAttributeType.Breakthrough, Role.Instance.Chi.ChiAttributeType.Counteraction } },
        };

        static void MaxChiPart(Client.GameClient client, List<string> report)
        {
            var p = client.Player;
            if (p.MyChi == null)
            {
                report.Add("no Chi object on this character (log out and in once).");
                return;
            }
            var flagsField = typeof(Role.Instance.Chi.ChiPower).GetField("attributeFlags", BindingFlags.NonPublic | BindingFlags.Instance);
            if (!Role.Instance.Chi.ChiPool.ContainsKey(p.UID))
                Role.Instance.Chi.ChiPool.TryAdd(p.UID, p.MyChi);
            foreach (var power in p.MyChi)
            {
                var plan = ChiPlan[power.Type];
                int flags = 0;
                for (int i = 0; i < 4; i++)
                {
                    // StatsRand with epic = the top value of the attribute (the same roll a "charged" study gives)
                    power.Fields[i] = Tuple.Create(plan[i], Role.Instance.Chi.ChiPower.StatsRand(plan[i], power.Type, p.Name, true));
                    flags |= 1 << ((int)plan[i] - 1);
                }
                if (flagsField != null)
                    flagsField.SetValue(power, Enum.ToObject(flagsField.FieldType, flags));
                power.UnLocked = true;
                power.UID = p.UID;
                power.Name = p.Name;
                if (power.Type == MsgChiInfo.ChiPowerType.Dragon) Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, power);
                else if (power.Type == MsgChiInfo.ChiPowerType.Phoenix) Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, power);
                else if (power.Type == MsgChiInfo.ChiPowerType.Tiger) Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, power);
                else if (power.Type == MsgChiInfo.ChiPowerType.Turtle) Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, power);
                report.Add(power.Type + ": " + string.Join(", ", power.Fields.Select(f => f.Item1.ToString())) + " - score " + power.Score + ", rank " + power.Rank);
            }
            p.MyChi.ChiPoints = 4000;// a full Chi bar (the most the Chi window buys up to)
            Role.Instance.Chi.ComputeStatus(p.MyChi);
            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
            MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Send);
            MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Upgrade);
            report.Add("total score " + p.MyChi.AllScore() + ", " + p.MyChi.ChiPoints + " Chi points");
        }

        // ------------------------------------------------------------------ @maxjianghu
        [GmCommand("maxjianghu", Role = CommandRole.GM, Help = "development: Jiang Hu opened, 9 stages x 9 stars at level 6, talent 5")]
        static void MaxJiangHu(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "maxjianghu")) return;
            var report = new List<string>();
            using (var rec = new ServerSockets.RecycledPacket())
                MaxJiangHuPart(client, rec.GetStream(), report);
            Report(client, "maxjianghu", report);
        }

        // the attribute each stage gets (all 9 stars the same = the 9-star alignment bonus); each must be in
        // JingHuCultivateStatus.txt for its stage, else the first allowed one of this list is used
        static readonly byte[] JiangPlan = { 11, 12, 6, 10, 1, 11, 2, 13, 6 };
        static readonly byte[] JiangFallback = { 11, 10, 6, 2, 13, 12, 1, 14, 3, 5, 7, 8, 9, 4, 15 };

        static void MaxJiangHuPart(Client.GameClient client, ServerSockets.Packet stream, List<string> report)
        {
            var p = client.Player;
            var jiang = p.MyJiangHu;
            if (jiang == null)
            {
                // as MsgJiangHuInfo CreateJiang does when the player names the Jiang Hu
                jiang = new Role.Instance.JiangHu(p.UID);
                string name = p.Name.Replace("/", "");
                jiang.CustomizedName = name.Length > 16 ? name.Substring(0, 16) : name;
                jiang.Name = p.Name;
                jiang.Level = (byte)p.Level;
                p.MyJiangHu = jiang;
                jiang.CreateTime();
                jiang.SendInfo(client, MsgJiangHuInfo.JiangMode.SetName, false, p.UID.ToString(), "1", "1");
                report.Add("Jiang Hu created (name " + jiang.CustomizedName + ")");
            }
            var types = new List<string>();
            for (int s = 0; s < jiang.ArrayStages.Length; s++)
            {
                byte stageNo = (byte)(s + 1);
                List<byte> allowed;
                Database.JianHuTable.CultivateStatus.TryGetValue(stageNo, out allowed);
                byte typ = s < JiangPlan.Length ? JiangPlan[s] : (byte)0;
                if (allowed != null && !allowed.Contains(typ))
                    typ = JiangFallback.FirstOrDefault(t => allowed.Contains(t));
                ushort uid = Database.JianHuTable.ValueToRoll(typ, 6);
                if (!Database.JianHuTable.Atributes.ContainsKey(uid))
                {
                    report.Add("stage " + stageNo + ": no level-6 value for attribute " + typ + ", skipped");
                    continue;
                }
                var stage = jiang.ArrayStages[s];
                stage.Activate = true;
                foreach (var star in stage.ArrayStars)
                {
                    star.Activate = true;
                    star.Level = 6;
                    star.Typ = (Role.Instance.JiangHu.Stage.AtributesType)typ;
                    star.UID = uid;
                }
                types.Add(stageNo + ":" + (Role.Instance.JiangHu.Stage.AtributesType)typ);
            }
            jiang.Talent = 5;
            jiang.Level = (byte)p.Level;
            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);// runs CreateStatusAtributes
            jiang.SendStatus(stream, client, client);
            jiang.SendInfo(client, MsgJiangHuInfo.JiangMode.UpdateTalent, true, p.UID.ToString(), jiang.Talent.ToString());
            jiang.SendInfo(client, MsgJiangHuInfo.JiangMode.UpdateTime, false, jiang.FreeCourse.ToString(), jiang.Time.ToString());
            report.Add("9 stages x 9 stars at level 6: " + string.Join(", ", types));
            report.Add("score level " + jiang.GetGrade() + ", inner strength " + jiang.Inner_Strength + ", talent " + jiang.Talent);
        }

        // ------------------------------------------------------------------ @maxinner
        [GmCommand("maxinner", Role = CommandRole.GM, Help = "development: Inner Power, every stage and Nei Gong at top level, Perfect score 100")]
        static void MaxInner(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "maxinner")) return;
            var report = new List<string>();
            using (var rec = new ServerSockets.RecycledPacket())
                MaxInnerPart(client, rec.GetStream(), report);
            Report(client, "maxinner", report);
        }

        static void MaxInnerPart(Client.GameClient client, ServerSockets.Packet stream, List<string> report)
        {
            var inner = client.Player.InnerPower;
            if (inner == null)
            {
                report.Add("no Inner Power object on this character (log out and in once).");
                return;
            }
            int gongs = 0;
            foreach (var stage in inner.Stages)
            {
                var dbStage = Database.InnerPowerTable.Stages[stage.ID - 1];
                stage.UnLocked = true;
                for (int x = 0; x < stage.NeiGongs.Length; x++)
                {
                    var gong = stage.NeiGongs[x];
                    gong.Unlocked = true;
                    gong.level = dbStage.NeiGongAtributes[x].MaxLevel;
                    gong.Complete = true;
                    gong.Score = 100;
                    gongs++;
                }
            }
            inner.UpdateStatus();
            Role.Instance.InnerPower.InnerPowerRank.UpdateRank(inner);
            client.Send(stream.InnerPowerGui(inner.GetNeiGongs()));
            foreach (var stage in inner.Stages)
            {
                client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateStage, client.Player.UID, stage));
                client.Send(stream.InnerPowerStageInfo(InnerPowerStage.ActionID.UpdateScore, client.Player.UID, stage));
            }
            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
            report.Add(inner.Stages.Length + " stage(s), " + gongs + " Nei Gong(s) at top level, Perfect 100; total score " + inner.TotalScore);
        }

        // ------------------------------------------------------------------ @maxextras
        [GmCommand("maxextras", Role = CommandRole.GM, Help = "development: the 7 sub classes at level 9 / phase 9, nobility King")]
        static void MaxExtras(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "maxextras")) return;
            var report = new List<string>();
            using (var rec = new ServerSockets.RecycledPacket())
                MaxExtrasPart(client, rec.GetStream(), report);
            Report(client, "maxextras", report);
        }

        static void MaxExtrasPart(Client.GameClient client, ServerSockets.Packet stream, List<string> report)
        {
            var p = client.Player;

            // sub classes: as MsgSubClass Join / Upgrade / Pro do, straight to the top
            var sorts = new[] { Database.DBLevExp.Sort.MartialArtist, Database.DBLevExp.Sort.Warlock, Database.DBLevExp.Sort.ChiMaster,
                Database.DBLevExp.Sort.Sage, Database.DBLevExp.Sort.Apothecary, Database.DBLevExp.Sort.Performer, Database.DBLevExp.Sort.Wrangler };
            foreach (var sort in sorts)
            {
                MsgSubClass.SubClases sub;
                if (!p.SubClass.src.TryGetValue(sort, out sub))
                {
                    sub = MsgSubClass.SubClases.Create(sort);
                    p.SubClass.SendLearn(client, sub, stream);
                }
                sub.Level = 9;
                sub.Phrase = 9;
                p.SubClass.src[sort] = sub;
                client.Send(stream.SubClassCreate(MsgSubClass.Action.SendUpdate, sub.ID, 0, 0));
                p.SubClass.SendPromoted(client, sub, stream);
            }
            p.SubClass.ActiveSubclass = Database.DBLevExp.Sort.MartialArtist;
            client.Send(stream.SubClassCreate(MsgSubClass.Action.ActivateSubClass, Database.DBLevExp.Sort.MartialArtist, 9, 0));
            p.SubClass.CreateSpawn(client);
            p.SubClass.StudyPoints = (ushort)Math.Max((int)p.SubClass.StudyPoints, 10000);
            p.SubClass.ShowGui(client, stream);
            report.Add("7 sub classes at level 9 / phase 9, Martial Artist active, " + p.SubClass.StudyPoints + " study points");

            // nobility: a donation above the current top one (as the nobility donate window does, free)
            if (p.Nobility == null)
                p.Nobility = new Role.Instance.Nobility(client);
            ulong top = 0;
            foreach (var n in Program.NobilityRanking.GetArray())
                if (n != null && n.UID != p.UID) top = Math.Max(top, n.Donation);
            p.Nobility.Donation = Math.Max(p.Nobility.Donation, Math.Max(top + 1000000, 5000000000UL));
            Program.NobilityRanking.UpdateRank(p.Nobility);
            client.Send(stream.NobilityIconCreate(p.Nobility));
            report.Add("nobility " + p.Nobility.Rank + " (donation " + p.Nobility.Donation.ToString("N0") + ", position " + (p.Nobility.Position + 1) + ")");
            client.Equipment.QueryEquipment(client.Equipment.Alternante);
        }

        // ------------------------------------------------------------------ @maxall
        [GmCommand("maxall", Role = CommandRole.GM, Help = "development: @maxme (no new gear if you have some) + @maxgear @maxchi @maxjianghu @maxinner @maxpoints @maxextras")]
        static void MaxAll(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "maxall")) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Run(client, "maxme", r => MaxCharacter(client, stream, r));
                bool hasGear = client.Equipment.ClientItems.Values.Any(i => GearSlot(SlotOf(i))) || client.Inventory.ClientItems.Values.Any(i => GearSlot(SlotOf(i)));
                if (!hasGear)
                    Run(client, "maxme", r => GiveGearSet(client, stream, r));
                Run(client, "maxgear", r => MaxGearPart(client, stream, r));
                Run(client, "maxchi", r => MaxChiPart(client, r));
                Run(client, "maxjianghu", r => MaxJiangHuPart(client, stream, r));
                Run(client, "maxinner", r => MaxInnerPart(client, stream, r));
                Run(client, "maxextras", r => MaxExtrasPart(client, stream, r));
                client.Equipment.QueryEquipment(client.Equipment.Alternante);
            }
            client.SendSysMesage("[maxall] done. Log out and in once so every window shows the new character.");
        }

        // one part: its report, or the error it threw (the next parts still run)
        static void Run(Client.GameClient client, string tag, Action<List<string>> part)
        {
            var report = new List<string>();
            try { part(report); }
            catch (Exception e) { report.Add("error: " + e.GetType().Name + ": " + e.Message); }
            Report(client, tag, report);
        }

        // ------------------------------------------------------------------ @potions
        [GmCommand("potions", Role = CommandRole.GM, Help = "development: stacks of the best HP potion the server heals with (7StarOintment, 10000 HP)", Usage = "@potions [stacks, default 5]")]
        static void Potions(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "potions")) return;
            int stacks = 5;
            if (data.Length > 1) int.TryParse(data[1], out stacks);
            stacks = Math.Max(1, Math.Min(20, stacks));
            // 7StarOintment: 10000 HP, the best potion UseItem.cs heals with (LifePill 1003020 has 12000 HP in itemtype but the
            // server does not use it as a potion: the old @potions gave it, so those are taken back here)
            const uint LifePill = 1003010, OldPill = 1003020;
            Database.ItemType.DBItem db;
            ushort size = (ushort)(Database.Server.ItemsBase.TryGetValue(LifePill, out db) && db.StackSize > 0 ? db.StackSize : 10);
            int given = 0;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var it in client.Inventory.ClientItems.Values.ToArray())
                    if (it.ITEM_ID == OldPill) client.Inventory.Update(it, Role.Instance.AddMode.REMOVE, stream);
                for (int i = 0; i < stacks && client.Inventory.HaveSpace(1); i++, given++)
                    client.Inventory.AddItemWitchStack(LifePill, 0, size, stream, false);
            }
            client.SendSysMesage("[potions] " + given + " stack(s) of " + size + " 7StarOintment (10000 HP each)" + (given < stacks ? ", the bag is full" : "") + ".");
        }

        // ------------------------------------------------------------------ @setclass
        static readonly Dictionary<string, byte> Classes = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase)
        {
            { "trojan", 15 }, { "warrior", 25 }, { "archer", 45 }, { "ninja", 55 }, { "monk", 65 }, { "pirate", 75 },
            { "dragonwarrior", 85 }, { "lee", 85 }, { "water", 135 }, { "fire", 145 }, { "windwalker", 165 },
        };

        [GmCommand("setclass", Role = CommandRole.GM, Help = "development: another class at its best: old class skills and gear out, the new class skills and best gear worn with full stuff", Usage = "@setclass ninja|trojan|warrior|archer|monk|pirate|dragonwarrior|water|fire|windwalker")]
        static void SetClass(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "setclass")) return;
            byte cls;
            if (data.Length < 2 || !Classes.TryGetValue(data[1], out cls))
            {
                client.SendSysMesage("@setclass " + string.Join("|", Classes.Keys));
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var p = client.Player;
                Run(client, "setclass", r =>
                {
                    // the old class: its skills and its gear (worn and in the bag) go
                    int skillsOut = 0;
                    foreach (ushort id in ClassSkills(p.Class))
                        if (client.MySpells.ClientSpells.ContainsKey(id)) { client.MySpells.Remove(id, stream); skillsOut++; }
                    int gearOut = 0;
                    foreach (var slot in new[] { Role.Flags.ConquerItem.Head, Role.Flags.ConquerItem.Necklace, Role.Flags.ConquerItem.Armor, Role.Flags.ConquerItem.Ring,
                        Role.Flags.ConquerItem.Boots, Role.Flags.ConquerItem.RightWeapon, Role.Flags.ConquerItem.LeftWeapon })
                        if (!client.Equipment.FreeEquip(slot) && DeleteWorn(client, slot, stream)) gearOut++;
                    p.Class = cls;
                    p.FirstClass = cls;
                    p.SecoundeClass = cls;
                    foreach (var it in client.Inventory.ClientItems.Values.ToArray())
                    {
                        ushort pos = SlotOf(it);
                        if (pos == 0 || !GearSlot(pos)) continue;
                        if (pos == (ushort)Role.Flags.ConquerItem.Steed || pos == (ushort)Role.Flags.ConquerItem.SteedMount) continue;
                        if (!Database.ItemType.Equipable(it.ITEM_ID, client) || Database.ItemType.IsWindWalkerWeapon(it.ITEM_ID))
                        { client.Inventory.Update(it, Role.Instance.AddMode.REMOVE, stream); gearOut++; }
                    }
                    r.Add("old class: " + skillsOut + " skill(s) and " + gearOut + " gear item(s) removed");
                });
                Run(client, "setclass", r => MaxCharacter(client, stream, r));
                Run(client, "setclass", r =>
                {
                    // the new class: the best gear, worn
                    var worn = new List<string>();
                    foreach (var slot in new[] { Role.Flags.ConquerItem.Head, Role.Flags.ConquerItem.Necklace, Role.Flags.ConquerItem.Armor, Role.Flags.ConquerItem.Ring,
                        Role.Flags.ConquerItem.Boots, Role.Flags.ConquerItem.RightWeapon, Role.Flags.ConquerItem.LeftWeapon })
                    {
                        if (!client.Equipment.FreeEquip(slot)) continue;
                        var best = Best(client, slot);
                        if (best == null) continue;
                        var gem = GemFor((ushort)slot);
                        if (client.Equipment.Add(stream, best.ID, slot, 12, 7, 255, gem, gem, false)) worn.Add(best.Name);
                    }
                    r.Add("worn: " + string.Join(", ", worn));
                });
                Run(client, "maxgear", r => MaxGearPart(client, stream, r));
                Run(client, "maxpoints", r => SpendPoints(client, stream, r));
                // the client has gear icons for colours 2-9 only: a colour 0/1 item shows an empty slot
                foreach (var it in client.Equipment.ClientItems.Values.ToArray())
                    if ((byte)it.Color < 2) { it.Color = (Role.Flags.Color)3; it.Mode = Role.Flags.ItemMode.Update; it.Send(client, stream); }
                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
            }
            client.SendSysMesage("[setclass] done. Log out and in once so the client shows the new class everywhere.");
        }

        // ------------------------------------------------------------------ @setweapon
        [GmCommand("setweapon", Role = CommandRole.GM, Help = "development: this weapon in both hands (+12, -7, 255, super dragon gems), the old ones deleted", Usage = "@setweapon <item id> [right|both]")]
        static void SetWeapon(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "setweapon")) return;
            uint id;
            if (data.Length < 2 || !uint.TryParse(data[1], out id) || !Database.Server.ItemsBase.ContainsKey(id))
            {
                client.SendSysMesage("@setweapon <item id> [right|both]");
                return;
            }
            bool both = data.Length < 3 || data[2] != "right";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var slot in both ? new[] { Role.Flags.ConquerItem.RightWeapon, Role.Flags.ConquerItem.LeftWeapon } : new[] { Role.Flags.ConquerItem.RightWeapon })
                {
                    if (!client.Equipment.FreeEquip(slot)) DeleteWorn(client, slot, stream);
                    client.Equipment.Add(stream, id, slot, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false);
                }
                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
            }
            client.SendSysMesage("[setweapon] " + ItemName(id) + " (" + id + ") " + (both ? "in both hands" : "in the right hand") + ". Run @maxgear for its soul / refinery / perfection.");
        }
        // Equipment.Delete leaves a ghost in the client's bag (the client gets "unequip" = to the bag, but the server never
        // had the item in its bag, so Inventory.Update(REMOVE) sends nothing): unequip to the bag, then delete it from there.
        static bool DeleteWorn(Client.GameClient client, Role.Flags.ConquerItem slot, ServerSockets.Packet stream)
        {
            MsgGameItem item;
            if (!client.Equipment.TryGetEquip(slot, out item)) return false;
            uint uid = item.UID;
            if (!client.Equipment.Remove(slot, stream)) return false;// to the bag, on both sides
            MsgGameItem inBag;
            if (client.Inventory.ClientItems.TryGetValue(uid, out inBag))
                client.Inventory.Update(inBag, Role.Instance.AddMode.REMOVE, stream);
            return true;
        }
        // ------------------------------------------------------------------ @steedrace
        // The horse race only starts by itself Monday-Friday 14:55 (MsgSteedRace.work). This drives the same race object
        // through its own private steps (reflection) at any time: invitations now, gate after LeadSeconds, end after RaceMinutes.
        static DateTime raceGate = DateTime.MinValue, raceEnd = DateTime.MinValue;
        static bool raceFive, raceOpen;
        static readonly System.Reflection.BindingFlags Priv = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        [GmCommand("steedrace", Role = CommandRole.GM, Help = "development: start the horse race now (invitations, gate after N s, end after M min) | end", Usage = "@steedrace start [lead seconds, default 60] [race minutes, default 30] | end | status")]
        static void SteedRace(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "steedrace")) return;
            var race = Game.MsgTournaments.MsgSchedules.SteedRace;
            string sub = data.Length > 1 ? data[1].ToLowerInvariant() : "status";
            if (sub == "start")
            {
                if (race.IsOn) { client.SendSysMesage("[steedrace] already running."); return; }
                int lead = 60, minutes = 30;
                if (data.Length > 2) int.TryParse(data[2], out lead);
                if (data.Length > 3) int.TryParse(data[3], out minutes);
                lead = Math.Max(10, Math.Min(300, lead));
                minutes = Math.Max(2, Math.Min(60, minutes));
                race.Create();
                typeof(Game.MsgTournaments.MsgSteedRace).GetMethod("SendInvitations", Priv).Invoke(race, null);
                // the race counts its own 5 minutes from InvitationsSentOut: move that back so the gate opens after `lead`
                DateTime sent = DateTime.Now.AddSeconds(lead).AddMinutes(-Game.MsgTournaments.MsgSteedRace.InvitationLeadMinutes);
                typeof(Game.MsgTournaments.MsgSteedRace).GetField("InvitationsSentOut", Priv).SetValue(race, sent);
                raceGate = DateTime.Now.AddSeconds(lead);
                raceEnd = raceGate.AddMinutes(minutes);
                raceFive = raceOpen = false;
                client.SendSysMesage("[steedrace] invitations sent; the gate opens in " + lead + " s, the race ends at " + raceEnd.ToString("HH:mm") + ".");
            }
            else if (sub == "join")
            {
                if (!race.IsOn) { client.SendSysMesage("[steedrace] not running: @steedrace start"); return; }
                // the official way: to the Steed Envoy in Twin City, who checks the steed and the Riding skill before the race
                client.Teleport(286, 147, 1002);
                client.SendSysMesage("[steedrace] talk to the Steed Envoy (needs a steed and the Riding skill).");
            }
            else if (sub == "end")
            {
                if (race.IsOn) typeof(Game.MsgTournaments.MsgSteedRace).GetMethod("End", Priv).Invoke(race, null);
                raceGate = raceEnd = DateTime.MinValue;
                client.SendSysMesage("[steedrace] ended.");
            }
            else
                client.SendSysMesage("[steedrace] " + (race.IsOn ? "running, " + (raceOpen ? "gate open, ends " + raceEnd.ToString("HH:mm") : "gate opens " + raceGate.ToString("HH:mm:ss")) : "not running") + ".");
        }

        public override void OnTick(Extensions.Time32 clock)
        {
            var race = Game.MsgTournaments.MsgSchedules.SteedRace;
            var t = typeof(Game.MsgTournaments.MsgSteedRace);
            if (raceGate == DateTime.MinValue)
            {
                // a hot reload of this module forgets the static schedule: take it back from the running race itself
                if (!race.IsOn || (bool)t.GetField("GateOpen", Priv).GetValue(race) && DateTime.Now.Hour == 15) return;
                if (!race.IsOn) return;
                DateTime sent = (DateTime)t.GetField("InvitationsSentOut", Priv).GetValue(race);
                raceGate = sent.AddMinutes(Game.MsgTournaments.MsgSteedRace.InvitationLeadMinutes);
                raceEnd = raceGate.AddMinutes(20);
                raceFive = (bool)t.GetField("FiveSecondsLeft", Priv).GetValue(race);
                raceOpen = (bool)t.GetField("GateOpen", Priv).GetValue(race);
            }
            if (!race.IsOn) { raceGate = raceEnd = DateTime.MinValue; return; }
            DateTime now = DateTime.Now;
            if (!raceFive && now >= raceGate.AddSeconds(-5))
            {
                raceFive = true;
                t.GetField("InvitationsExpired", Priv).SetValue(race, true);
                t.GetField("FiveSecondsLeft", Priv).SetValue(race, true);
                t.GetMethod("SendData", Priv).Invoke(race, new object[] { ActionType.BeginSteedRace, 0u, 1u });
            }
            else if (raceFive && !raceOpen && now >= raceGate)
            {
                raceOpen = true;
                t.GetMethod("OpenGate", Priv).Invoke(race, null);
            }
            else if (raceOpen && now >= raceEnd)
            {
                t.GetMethod("End", Priv).Invoke(race, null);
                raceGate = raceEnd = DateTime.MinValue;
            }
        }
        // ------------------------------------------------------------------ @takeoff
        [GmCommand("takeoff", Role = CommandRole.GM, Help = "development: take off what is worn in a slot (17 = mount armor, 9 = garment, 12 = steed)", Usage = "@takeoff <slot>")]
        static void TakeOff(Client.GameClient client, string[] data)
        {
            if (!DevOnly(client, "takeoff")) return;
            byte slot;
            if (data.Length < 2 || !byte.TryParse(data[1], out slot)) { client.SendSysMesage("@takeoff <slot>"); return; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                bool ok = client.Equipment.Remove((Role.Flags.ConquerItem)slot, stream);
                client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                client.SendSysMesage("[takeoff] slot " + slot + (ok ? " taken off." : ": nothing removed."));
            }
        }
        static byte Master(byte cls)
        {
            if (cls >= 132 && cls <= 135) return 135;// water taoist
            if (cls >= 142 && cls <= 145) return 145;// fire taoist
            if (cls >= 100 && cls <= 101) return 145;
            return (byte)(cls / 10 * 10 + 5);
        }

        // the skill ids of a class (magictype ranges of the 6609 classes)
        static IEnumerable<ushort> ClassSkills(byte cls)
        {
            if (Database.AtributesStatus.IsWindWalker(cls))
                return new ushort[] { 12830, 12840, 12850, 12860, 12870, 12890, 12930, 12940, 12950, 12960, 12970, 12980, 12990, 13000, 13020, 13030, 13040, 13050, 13060, 13070, 13080, 13090 };
            if (Database.AtributesStatus.IsNinja(cls))
                return new ushort[] { 6000, 6001, 6002, 6004, 6010, 6011, 3050, 11110, 11170, 11180, 11230, 11650, 11670, 12070, 12080, 12090, 12110 };
            return new ushort[0];
        }

        // the item ids the 6609 client has an icon for (ani\ItemMinIcon.Ani of the client folder, shell.ini [DevTools] Client=)
        static HashSet<uint> icons;
        static bool HasIcon(uint id)
        {
            if (icons == null)
            {
                var set = new HashSet<uint>();
                try
                {
                    string client = Framework.Settings.String("DevTools", "Client", @"D:\Conquer Online 3.0");
                    string ani = System.IO.Path.Combine(client, "ani", "ItemMinIcon.Ani");
                    if (System.IO.File.Exists(ani))
                        foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(System.IO.File.ReadAllText(ani), @"\[Item(\d+)\]"))
                            set.Add(uint.Parse(m.Groups[1].Value));
                }
                catch { }
                icons = set;
            }
            if (icons.Count == 0 || icons.Contains(id)) return true;// no client folder: do not filter
            for (uint c = 2; c <= 9; c++)// gear icons are keyed by colour too: [Item<colour><id>]
                if (icons.Contains(c * 1000000 + id)) return true;
            return false;
        }
        static Database.ItemType.DBItem Best(Client.GameClient client, Role.Flags.ConquerItem slot)
        {
            // one-hand weapons (katana, ...) count as right-hand items in ItemPosition: the left hand gets the same best one
            if (slot == Role.Flags.ConquerItem.LeftWeapon && Database.AtributesStatus.IsNinja(client.Player.Class))
                return Best(client, Role.Flags.ConquerItem.RightWeapon);
            Database.ItemType.DBItem best = null;
            bool ww = Database.AtributesStatus.IsWindWalker(client.Player.Class);
            foreach (var it in Database.Server.ItemsBase.Values)
            {
                if (it.ID % 10 != 9 || it.Level > client.Player.Level) continue;
                if (Database.ItemType.ItemPosition(it.ID) != (ushort)slot) continue;
                bool weaponSlot = slot == Role.Flags.ConquerItem.RightWeapon || slot == Role.Flags.ConquerItem.LeftWeapon;
                if (weaponSlot && ww && !Database.ItemType.IsWindWalkerWeapon(it.ID)) continue;
                if (weaponSlot && Database.AtributesStatus.IsNinja(client.Player.Class) && !Database.ItemType.IsKatana(it.ID)) continue;
                if (it.ID >= 601390 && it.ID <= 601439) continue;// HanzoKatana: this client draws it as a hand fan (checked in game), EmperorKatana is drawn right
                if (!HasIcon(it.ID)) continue;// the client has no picture for it (newer item): it would show as an empty slot
                if (!Database.ItemType.Equipable(it.ID, client)) continue;
                if (best == null || it.Level > best.Level || (it.Level == best.Level && it.ID > best.ID)) best = it;
            }
            return best;
        }
    }
}
