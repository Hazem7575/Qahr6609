using Conquer6609.ServerSockets;
using Conquer6609;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conquer6609.Bot
{
    public enum EquipmentType
    {
        Necklace,
        Ring,
        Armet,
        Armor,
        OneHander,
        TwoHander
    }
    public class Equipment
    {
        public static ushort[] NecklaceType = new ushort[] { 120, 121 };
        public static ushort[] RingType = new ushort[] { 150, 151 };
        public static ushort[] ArmetType = new ushort[] { 111, 112, 113, 114, 117, 118 };
        public static ushort[] ArmorType = new ushort[] { 130, 131, 132, 133, 134 };
        public static ushort[] OneHanderType = new ushort[] { 410, 420, 421, 430, 440, 450, 460, 480, 481, 490, 500, 601 };
        public static ushort[] TwoHanderType = new ushort[] { 510, 530, 560, 561, 580, 900, };
        public static ushort[] DevBow = new ushort[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 310, 320, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420 };
        public static uint[] Garments = new uint[]
        {
            181375, 182305, 181365, 181345, 182315,
            181335, 191305, 181305, 181405, 181505,
            181605, 181705, 181805, 181905, 181315,
            181415, 181515, 181615, 181715, 181815,
            181915, 181325, 181425, 181525, 181625,
            181725, 181825, 181925, 181355, 191405
        };
        public BotManager.ConquerAI Bot;
        public Equipment(BotManager.ConquerAI _bot)
        {
            Bot = _bot;
        }

        // Super (quality 9) items with Level >= 10, grouped by equipment position, in ItemsBase order.
        // Built once per ItemsBase instance (the per-bot filter below only checks class + level).
        private sealed class PositionCache
        {
            public object Source;
            public Database.ItemType.DBItem[] Head, Ring, Boots, Necklace, RightWeapon;
        }
        private static PositionCache cache;
        private static readonly object cacheSync = new object();

        private static PositionCache GetCache()
        {
            var items = Database.Server.ItemsBase;
            var c = cache;
            if (c != null && object.ReferenceEquals(c.Source, items))
                return c;
            lock (cacheSync)
            {
                c = cache;
                if (c != null && object.ReferenceEquals(c.Source, items))
                    return c;
                var head = new List<Database.ItemType.DBItem>();
                var ring = new List<Database.ItemType.DBItem>();
                var boots = new List<Database.ItemType.DBItem>();
                var necklace = new List<Database.ItemType.DBItem>();
                var rwep = new List<Database.ItemType.DBItem>();
                foreach (var e in items.Values)
                {
                    if ((e.ID % 10) != 9 || e.Level < 10)
                        continue;
                    ushort pos = Database.ItemType.ItemPosition(e.ID);
                    if (pos == (ushort)Role.Flags.ConquerItem.Head) head.Add(e);
                    if (pos == (ushort)Role.Flags.ConquerItem.Ring) ring.Add(e);
                    if (pos == (ushort)Role.Flags.ConquerItem.Boots) boots.Add(e);
                    if (pos == (ushort)Role.Flags.ConquerItem.Necklace) necklace.Add(e);
                    if (pos == (ushort)Role.Flags.ConquerItem.RightWeapon) rwep.Add(e);
                }
                c = new PositionCache
                {
                    Source = items,
                    Head = head.ToArray(),
                    Ring = ring.ToArray(),
                    Boots = boots.ToArray(),
                    Necklace = necklace.ToArray(),
                    RightWeapon = rwep.ToArray()
                };
                cache = c;
                return c;
            }
        }
        // Same result as the old per-bot query: items the bot's job can wear and whose Level is below the bot's level.
        private Database.ItemType.DBItem[] ForBot(Database.ItemType.DBItem[] source)
        {
            var list = new List<Database.ItemType.DBItem>();
            ushort level = Bot.Bot.Player.Level;
            foreach (var e in source)
            {
                if (e.Level < level && Database.ItemType.EquipPassJobReq(e, Bot.Bot))
                    list.Add(e);
            }
            return list.ToArray();
        }
        private static bool Pick(Database.ItemType.DBItem[] arr, out uint id)
        {
            id = 0;
            if (arr.Length == 0)
                return false;
            id = arr[Role.Core.Random.Next(0, arr.Length)].ID;
            return true;
        }
        public Equipment GetRandomEquipment(byte c)
        {
            var all = GetCache();
            uint id;
            if (Pick(ForBot(all.Head), out id)) Head(id);
            if (Pick(ForBot(all.Ring), out id)) Ring(id);
            if (Pick(ForBot(all.Boots), out id)) Boots(id);
            if (Pick(ForBot(all.Necklace), out id)) Necklace(id);
            var r_wep = ForBot(all.RightWeapon);
            if (c >= 40 && c <= 45)
            {
                var wep = r_wep.Where(e => Database.ItemType.IsBow(e.ID)).ToArray();
                if (Pick(wep, out id)) RightWeapon(id);
            }
            if (c >= 10 && c <= 15)
            {
                RightWeapon(480219);
                LeftWeapon(480219);
                Armor(130109);
                RightWeaponAccessory(360466);
                LeftWeaponAccessory(360466);
            }
            if (c >= 100)
            {
                var wep = r_wep.Where(e => Database.ItemType.IsBacksword(e.ID)).ToArray();
                if (Pick(wep, out id)) RightWeapon(id);
            }
            if (c >= 20 && c <= 25)
            {
                var wep = r_wep.Where(e => (e.ID / 1000) == 561 || (e.ID / 1000) == 560).ToArray();
                if (Pick(wep, out id)) RightWeapon(id);
            }
            if (Bot.Bot.Player.Level >= 100)
            {
                Tower(202009);
                Fan(201009);
            }
            return this;
        }
        // A fresh pooled packet per call (the old code kept a packet that was already returned to the pool).
        private void AddItem(uint ID, Role.Flags.ConquerItem position, byte plus = 0)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                Bot.Bot.Equipment.Add(rec.GetStream(), ID, position, plus);
            }
        }
        public Equipment LeftWeapon(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.LeftWeapon, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment RightWeapon(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.RightWeapon, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment RightWeaponAccessory(uint ID)
        {

            AddItem(ID, Role.Flags.ConquerItem.RightWeaponAccessory);
            return this;
        }
        public Equipment LeftWeaponAccessory(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.LeftWeaponAccessory);
            return this;
        }
        public Equipment Garment(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.Garment);
            return this;
        }
        public Equipment Armor(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.Armor, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment Boots(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.Boots, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment Ring(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.Ring, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment Head(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.Head, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment Necklace(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.Necklace, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment Tower(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.Tower, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment Fan(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.Fan, (byte)Program.GetRandom.Next(3, 5));
            return this;
        }
        public Equipment Crop(uint ID)
        {
            AddItem(ID, Role.Flags.ConquerItem.RidingCrop, (byte)Program.GetRandom.Next(1, 6));
            return this;
        }
        public Equipment Send()
        {
            Bot.Bot.Equipment.QueryEquipment(false);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Bot.Bot.Player.View.SendView(Bot.Bot.Player.GetArray(stream, false), false);
            }
            return this;
        }
        private uint GenerateBowItem()
        {
            uint BaseID = 500000;
            uint Super = 9;
            uint SecPartID = (uint)DevBow[Program.GetRandom.Next(0, DevBow.Length)] + Super;
            return BaseID + SecPartID;
        }
        private uint GenerateItem(EquipmentType equipmentType)
        {
            uint ItemId = 0;
            byte dwItemQuality = 9;
            uint dwItemSort = 0;
            uint dwItemLev = 0;
            switch (equipmentType)
            {
                case EquipmentType.Necklace:
                    {
                        dwItemSort = NecklaceType[Program.GetRandom.Next(0, NecklaceType.Length)];
                        dwItemLev = 7;
                        break;
                    }
                case EquipmentType.Ring:
                    {
                        dwItemSort = RingType[Program.GetRandom.Next(0, RingType.Length)];
                        dwItemLev = 10;
                        break;
                    }
                case EquipmentType.Armor:
                    {
                        dwItemSort = ArmorType[Program.GetRandom.Next(0, ArmorType.Length)];
                        dwItemLev = 15;
                        break;
                    }
                case EquipmentType.OneHander:
                    {
                        dwItemSort = OneHanderType[Program.GetRandom.Next(0, OneHanderType.Length)];
                        dwItemLev = 15;
                        break;
                    }
            }
            ItemId = (dwItemSort * 1000) + (dwItemLev * 4) + dwItemQuality;
            if (equipmentType == EquipmentType.Armor)
            {
                if (Database.Server.ItemsBase.ContainsKey(ItemId))
                {
                    return ItemId;
                }
                else Garment(Garments[Program.GetRandom.Next(0, Garments.Length)]);
            }
            else
            {
                if (Database.Server.ItemsBase.ContainsKey(ItemId))
                    return ItemId;
            }
            return 0;
        }
        private uint AlterItemLevel(uint dwItemLev, uint dwItemSort)
        {
            int nRand = Extensions.BaseFunc.RandGet(100, true);

            if (nRand < 50) // 50% down one level
            {
                uint dwLev = dwItemLev;
                dwItemLev = (uint)(Extensions.BaseFunc.RandGet((int)(dwLev / 2 + dwLev / 3), false));

                if (dwItemLev > 1)
                    dwItemLev--;
            }
            else if (nRand > 80) // 20% up one level
            {
                if ((dwItemSort >= 110 && dwItemSort <= 114) ||
                    (dwItemSort >= 130 && dwItemSort <= 134) ||
                    (dwItemSort >= 900 && dwItemSort <= 999))
                {
                    dwItemLev = Math.Min(dwItemLev + 1, 9);
                }
                else
                {
                    dwItemLev = Math.Min(dwItemLev + 1, 23);
                }
            }

            return dwItemLev;
        }
    }
}
