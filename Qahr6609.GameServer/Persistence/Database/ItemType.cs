using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Conquer6609.Game.MsgServer;
namespace Conquer6609.Database
{
    public class RefinaryBoxes : Dictionary<uint, RefinaryBoxes.Boxe>
    {
        public class Boxe
        {
            public UInt32 Identifier, Position;
            public Boolean Untradable;
            public Rifinery.RefineryType Type;
        }

        public RefinaryBoxes()
        {
            using (Database.DBActions.Read reader = new DBActions.Read("databaserefineryboxes.txt"))
            {
                if (reader.Reader())
                {
                    for (int x = 0; x < reader.Count; x++)
                    {
                        DBActions.ReadLine line = new DBActions.ReadLine(reader.ReadString("0,0"), ',');
                        Boxe box = new Boxe();
                        box.Identifier = line.Read((uint)0);
                        box.Position = line.Read((uint)0);
                        box.Type = (Rifinery.RefineryType)line.Read((byte)0);
                        box.Untradable = line.Read((byte)0) == 1;

                        Add(box.Identifier, box);
                    }
                }

            }

        }

        public uint GainRefineryItem(uint ID)
        {
            Boxe RefineryB = null;
            if (TryGetValue(ID, out RefineryB))
            {
                List<Rifinery.Item> Possible = new List<Rifinery.Item>();
                foreach (Rifinery.Item RefineryI in Database.Server.RifineryItems.Values)
                {
                    if (RefineryI.Type == RefineryB.Type)
                    {
                        if (RefineryI.ForItemPosition == RefineryB.Position)
                        {
                            if (RefineryB.Identifier >= 3004197 && RefineryB.Identifier <= 3004226
                                || RefineryB.Identifier >= 3004266 && RefineryB.Identifier <= 3004280)
                            {
                                if (RefineryI.Level > 3)
                                    Possible.Add(RefineryI);
                            }
                            else
                                if (RefineryI.Level < 6)
                                    Possible.Add(RefineryI);
                        }
                    }
                }
                if (Possible.Count > 0)
                {
                    Random Rand = new Random();
                    Int32 x = Rand.Next(1, Possible.Count);
                    Rifinery.Item Refinery = Possible[x];

                    if (Refinery != null)
                    {
                        return Refinery.ItemID;
                    }
                }
            }
            return 0;

        }
    }
    public class Rifinery : Dictionary<uint, Rifinery.Item>
    {
        public enum RefineryType
        {
            None = 0,
            MDefence = 1,
            CriticalStrike = 2,
            SkillCriticalStrike = 3,
            Immunity = 4,
            Break = 5,
            Counteraction = 6,
            Detoxication = 7,
            Block = 8,
            Penetration = 9,
            Intensification = 10,

            FinalMDamage = 11,
            FinalMAttack = 12
        }
        public Rifinery()
        {
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "Rifinery.txt");
            foreach (string aline in baseText)
            {
                string[] line = aline.Split(' ');
                Item ite = new Item();
                ite.ItemID = uint.Parse(line[0]);
                ite.Level = CalculateLevel(line[1]);
                ite.Procent = uint.Parse(line[2]);
                ite.Type = (RefineryType)byte.Parse(line[3]);


                ite.Type2 = (RefineryType)byte.Parse(line[4]);
                ite.Procent2 = uint.Parse(line[5]);

                string UseItemName = line[53];
                if (ite.ItemID >= 3006165 && ite.ItemID <= 3006170)
                {
                    UseItemName = UseItemName.Replace("(", "");
                    UseItemName = UseItemName.Split(')')[0];
                }
                else if (ite.ItemID >= 3004136)
                {
                    UseItemName = UseItemName.Replace("[", "");
                    UseItemName = UseItemName.Split(']')[0];
                }

                ite.Name = UseItemName;
                ite.ForItemPosition = ForItemPosition(UseItemName);
                //  if(ite.ItemID >= 3004136)
                //      MyConsole.WriteLine(ite.ItemID + " level " + ite.Level + " foritem= " + ite.ForItemPosition + " ite.Type= " + ite.Type + " procent" + ite.Procent + " " + UseItemName);
                if (!ContainsKey(ite.ItemID))
                    Add(ite.ItemID, ite);

                if (!ItemType.Refinary.ContainsKey(ite.Level))
                    ItemType.Refinary.Add(ite.Level, new Dictionary<uint, Item>());
                if (!ItemType.Refinary[ite.Level].ContainsKey(ite.ItemID))
                    ItemType.Refinary[ite.Level].Add(ite.ItemID, ite);

            }
            MyConsole.WriteLine("Loading [" + baseText.Length + "] Refinery items");
            // [FastBoot, from Qahr] was GC.Collect(): a full blocking GC of every heap on each loader; the runtime collects on its own
        }

        public uint ForItemPosition(string name)
        {
            uint pos = 0;
            if (name == "Bow" || name == "2-Handed" || name == "1-Handed" || name == "Backsword" || name == "2-handed" || name == "1-handed")
                pos = (ushort)Role.Flags.ConquerItem.RightWeapon;
            if (name == "Shield" || name == "Hossu")
                pos = (ushort)Role.Flags.ConquerItem.LeftWeapon;
            if (name == "Ring" || name == "Bracelet")
                pos = (ushort)Role.Flags.ConquerItem.Ring;
            if (name == "Armor")
                pos = (ushort)Role.Flags.ConquerItem.Armor;
            if (name == "Boots")
                pos = (ushort)Role.Flags.ConquerItem.Boots;
            if (name == "Headgear")
                pos = (ushort)Role.Flags.ConquerItem.Head;
            if (name == "Necklace" || name == "Bag")
                pos = (ushort)Role.Flags.ConquerItem.Necklace;
            return pos;
        }

        public static uint CalculateLevel(string name)
        {
            byte level = 0;
            if (name.Contains("Normal")) level = 1;
            if (name.Contains("Unique")) level = 3;
            if (name.Contains("Refined")) level = 2;
            if (name.Contains("Elite")) level = 4;
            if (name.Contains("Super")) level = 5;
            if (name.Contains("Sacred")) level = 6;
            return level;
        }
        public class Item
        {
            public string Name = "";
            public uint ItemID = 0;
            public uint Level = 0;
            public uint ForItemPosition = 0;
            public uint Procent = 0;
            public uint Procent2 = 0;
            public RefineryType Type = 0;
            public RefineryType Type2 = 0;

        }
    }
    public class ItemType : Dictionary<uint, ItemType.DBItem>
    {
        public static Dictionary<ushort, Dictionary<uint, ItemType.DBItem>> PurificationItems = new Dictionary<ushort, Dictionary<uint, DBItem>>();
        public static Dictionary<uint, ItemType.DBItem> Accessorys = new Dictionary<uint, DBItem>();
        public static List<uint> TopAccesorys = new List<uint>()
        {
            360071 , 
360072  , 
360073 , 
360074  , 
360150  , 
360151  , 
380028 , 
200501 , 
360102 , 
360085  , 
360084  , 
360091  , 
360092 , 
350081 , 
350082 , 
360152 , 
360153  , 
360154 , 
360101  , 
360144 , 
350054 , 
360149  , 
360150  , 
360159 , 
360186  , 
380046  , 
360176 , 
360185 , 

350103  , 
350104  , 
350105 , 
360201 , 
360202 , 
360203
        };

        public static Dictionary<uint, Dictionary<uint, Rifinery.Item>> Refinary = new Dictionary<uint, Dictionary<uint, Rifinery.Item>>();
        public static Dictionary<uint, ItemType.DBItem> Garments = new Dictionary<uint, ItemType.DBItem>();
        public static Dictionary<uint, ItemType.DBItem> SteedMounts = new Dictionary<uint, ItemType.DBItem>();
        public static List<uint> unabletradeitem = new List<uint>()
        {
            750000,
            722700,
            729304,//chitoken
            711533,//lady
            3600025,
            3200000,
            729549,
            3008994,
            3200005,
            3600029,
            3007108,
            3007109,
            3007110,
            3200004,
            3600031,
            3600027,
            3600024


        };
        public const uint
            MaxPlus = 12,
            MaxEnchant = 255,
            MaxDurability = 7099,
            MemoryAgate = 720828,
            GoldPrize = 2100075,
            ExpBall = 723700,
            MeteorTearPacket = 723711,
            MeteorTear = 1088002,
            OneStonePacket = 723712,
            ArenaExperience = 723912,
            OneStone = 730001,
            SuperDragonBallScroll = 3302770,
            SuperDragonBallScroll_B_ = 3302896,
            DragonBallScroll = 720028,
            DragonBall = 1088000,
            MeteorScroll = 720027,
            SuperMeteorScroll = 3302769,
            Meteor = 1088001,
            MoonBox = 721080,
            ExperiencePotion = 723017,
            NinjaAmulet = 723583,
            PowerExpBall = 722057,
            DoubleExp = 723017,
            SkillTeamPKPack = 720981,
            ToughDrill = 1200005,
            StarDrill = 1200006,
            SuperToroiseGem = 700073,
            /////
            P7WeaponSoulScrap = 3004243,
            P7EquipmentSoulScrap = 3004244,
            SacredRefineryPack = 3004249,
            SacredRefineryScrap = 3004245,
            P7WeaponSoulPack = 3004247,
            P7EquipmentSoulPack = 3004248,
            /////
            ExpBall2 = 723834;
      
        public static bool IsCPPack(uint ID)
        {
            return ID >= 720653 && ID <= 720667 || ID >= 720675 && ID <= 720679 || ID >= 720681 && ID <= 720685 || ID >= 720687 && ID <= 720691 || ID >= 720693 && ID <= 720697;
        }
        public static bool IsMoneyBag(uint ID)
        {
            return ID >= 723713 && ID <= 723723 || ID == 3005945 || ID == 3008452;
        }
        public static bool IsMagicSpar(uint ID)
        {
            return ID >= 3006587 && ID <= 3006616 || ID == 3006582;
        }
        public static bool CheckAddGemFan(Role.Flags.Gem gem)
        {
            return gem == Role.Flags.Gem.NormalThunderGem || gem == Role.Flags.Gem.RefinedThunderGem || gem == Role.Flags.Gem.SuperThunderGem;
        }
        public static bool CheckAddGemTower(Role.Flags.Gem gem)
        {
            return gem == Role.Flags.Gem.NormalGloryGem || gem == Role.Flags.Gem.RefinedGloryGem || gem == Role.Flags.Gem.SuperGloryGem;
        }
        public static bool CheckAddGemWing(Role.Flags.Gem gem, byte slot)
        {
            if (slot == 1)
                return CheckAddGemFan(gem);
            else if (slot == 2)
                return CheckAddGemTower(gem);
            return false;
        }

        public static uint GetItemPoints(DBItem _dbitem, Game.MsgServer.MsgGameItem item)
        {

            uint Points = (uint)(item.GetPerfectionPosition == 9 ? 0 : 50);
            if (_dbitem.Level <= 120)
                Points += (uint)((double)((double)_dbitem.Level / 10d) * 30);
            else if (_dbitem.Level > 120 && _dbitem.Level <= 130)
                Points += (uint)((double)((double)_dbitem.Level / 10d) * 50);
            else
                Points += (uint)((double)((double)_dbitem.Level / 10d) * 60);

            switch (_dbitem.ID % 10)
            {
                case 6: Points += 50; break;
                case 7: Points += 150; break;
                case 8: Points += 250; break;
                case 9: Points += 450; break;

            }
            switch (item.Bless)
            {
                case 1: Points += 100; break;
                case 3: Points += 300; break;
                case 5: Points += 500; break;
                case 7: Points += 700; break;
            }
            if (item.SocketOne != Role.Flags.Gem.NoSocket)
            {
                Points += 1000;
                switch ((ushort)item.SocketOne % 10)
                {
                    case 1: Points += 200; break;
                    case 2: Points += 500; break;
                    case 3: Points += 800; break;
                }
            }
            if (item.SocketTwo != Role.Flags.Gem.NoSocket)
            {
                Points += 2500;
                switch ((ushort)item.SocketTwo % 10)
                {
                    case 1: Points += 200; break;
                    case 2: Points += 500; break;
                    case 3: Points += 800; break;
                }
            }
            switch (item.Plus)
            {
                case 1: Points += 200; break;
                case 2: Points += 600; break;
                case 3: Points += 1200; break;
                case 4: Points += 1800; break;
                case 5: Points += 2600; break;
                case 6: Points += 3500; break;
                case 7: Points += 4800; break;
                case 8: Points += 5800; break;
                case 9: Points += 6800; break;
                case 10: Points += 7800; break;
                case 11: Points += 8800; break;
                case 12: Points += 10000; break;

            }
            switch (item.Purification.PurificationLevel)
            {
                case 7: Points += 2000; break;
                case 6: Points += 1600; break;
                case 5: Points += 1200; break;
                case 4:
                    {
                        if (ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Ring)
                            Points += 2000;
                        else
                            Points += 800;
                        break;
                    }
                case 3:
                    {
                        Points += 500; break;

                    }
            }
            switch (item.Refinary.EffectLevel)
            {

                case 6: Points += 2000; break;
                case 5: Points += 1600; break;
                case 4: Points += 1200; break;
                case 3: Points += 800; break;
                case 2: Points += 200; break;
                case 1: Points += 100; break;
            }
            if (item.GetPerfectionPosition != 9)//steed
                Points += (uint)(item.Enchant * 2);

            if (item.PerfectionLevel >= 0 && item.PerfectionLevel < 10) Points += (uint)(item.PerfectionLevel * 180);
            if (item.PerfectionLevel >= 10 && item.PerfectionLevel < 19) Points += (uint)(item.PerfectionLevel * 400);
            if (item.PerfectionLevel >= 19 && item.PerfectionLevel < 28) Points += (uint)(item.PerfectionLevel * 650);
            if (item.PerfectionLevel >= 28)
            {
                Points += (uint)((item.PerfectionLevel - 28) * 100) + 28 * 650;
                Points -= 550;
                //Points -= (uint)(item.PerfectionLevel + 1) * 10;
            }
            if (item.PerfectionLevel > 0)
            {
                if (item.IsWeapon)
                    Points += 200;
            }
            return Points;
        }
        public static uint GetGemID(Role.Flags.Gem Gem)
        {
            return (uint)(700000 + (byte)Gem);
        }

        public static uint[] TalismanExtra = new uint[13] { 0, 6, 30, 70, 240, 740, 2240, 6670, 20000, 60000, 62000, 67000, 73000 };

        public static ushort PurifyStabilizationPoints(byte plevel)
        {
            return purifyStabilizationPoints[Math.Min(plevel - 1, (byte)5)];
        }

        static ushort[] purifyStabilizationPoints = new ushort[6] { 10, 30, 60, 100, 150, 200 };

        public static ushort RefineryStabilizationPoints(byte elevel)
        {
            return refineryStabilizationPoints[Math.Min(elevel - 1, (byte)4)];
        }
        static ushort[] refineryStabilizationPoints = new ushort[5] { 10, 30, 70, 150, 270 };
      
        public string GetItemName(uint ID)
        {
            DBItem item;
            if (Server.ItemsBase.TryGetValue(ID, out item))
            {
                return item.Name;
            }
            return "";
        }

        public static uint ComposePlusPoints(byte plus)
        {
            return ComposePoints[Math.Min(plus, (byte)12)];
        }
        public static uint StonePlusPoints(byte plus)
        {
            return StonePoints[Math.Min((int)plus, 8)];
        }

        public static bool IsWarriorEpicWeapons(uint ID)
        {
            return ID / 1000 == 624;
        }
        static ushort[] StonePoints = new ushort[9] { 1, 10, 40, 120, 360, 1080, 3240, 9720, 29160 };
        static ushort[] ComposePoints = new ushort[13] { 20, 20, 80, 240, 720, 2160, 6480, 19440, 58320, 2700, 5500, 9000, 0 };

        public static Role.Flags.SoulTyp GetSoulPosition(uint ID)
        {
            if (ID >= 820001 && ID <= 820076 || ID == 822074)
                return Role.Flags.SoulTyp.Headgear;
            if (ID >= 821002 && ID <= 821034 || ID == 822076)
                return Role.Flags.SoulTyp.Necklace;
            if (ID >= 824002 && ID <= 824020 || ID == 822077)
                return Role.Flags.SoulTyp.Boots;
            if (ID >= 823000 && ID <= 823062 || ID == 822075)
                return Role.Flags.SoulTyp.Ring;
            if (ID >= 800000 && ID <= 800142 || ID >= 800701 && ID <= 800917 || ID >= 801000 && ID <= 801104 || ID >= 801200 && ID <= 801308) //800916
                return Role.Flags.SoulTyp.OneHandWeapon;
            if (ID >= 800200 && ID <= 800618 || ID == 801103)
                return Role.Flags.SoulTyp.TwoHandWeapon;
            if (ID >= 822001 && ID <= 822073)
                return Role.Flags.SoulTyp.Armor;

            return Role.Flags.SoulTyp.None;
        }
        public static bool CompareSoul(uint ITEMID, uint SoulID)
        {
            Role.Flags.SoulTyp soul = GetSoulPosition(SoulID);
            var positionit = GetItemSoulTYPE(ITEMID);
            if (positionit == soul)
                return true;
            return false;
        }
        public static Role.Flags.SoulTyp GetItemSoulTYPE(UInt32 itemid)
        {
            UInt32 iType = itemid / 1000;

            if (iType >= 111 && iType <= 118 || iType == 123 || iType >= 141 && iType <= 148 || itemid >= 170000 && itemid <= 170309)
                return Role.Flags.SoulTyp.Headgear;

            else if (iType >= 120 && iType <= 121)
                return Role.Flags.SoulTyp.Necklace;

            else if (iType >= 130 && iType <= 139 || itemid >= 101000 && itemid <= 101309)
                return Role.Flags.SoulTyp.Armor;

            else if (iType >= 150 && iType <= 152)
                return Role.Flags.SoulTyp.Ring;

            else if (iType == 160)
                return Role.Flags.SoulTyp.Boots;

            else if (IsTwoHand(itemid) || itemid >= 421003 && itemid <= 421439)
                return Role.Flags.SoulTyp.TwoHandWeapon;

            else if ((iType >= 410 && iType <= 490) || (iType >= 500 && iType <= 580) || (iType >= 601 && iType <= 613) || iType == 616 || iType == 614 || iType == 617 || iType == 622
                || iType == 624 || iType == 619 || IsWindWalkerWeapon(itemid))
                return Role.Flags.SoulTyp.OneHandWeapon;

            else
                return Role.Flags.SoulTyp.TwoHandWeapon;
        }

        public static uint MoneyItemID(uint value)
        {
            if (value < 100)
                return 1090000;
            else if (value < 399)
                return 1090010;
            else if (value < 5099)
                return 1090020;
            else if (value < 8099)
                return 1091000;
            else if (value < 12099)
                return 1091010;
            else
                return 1091020;
        }
        public static ulong CalculateExpBall(byte Level)
        {
            if (Level < 130)
                return (ulong)(680462.7536 + (3479.5308 * (Level / 2)) * Level);
            else if (Level < 135)
                return (ulong)((680462.7536 + (3479.5308 * (Level / 2)) * Level) * ((Level % 10) + 6));
            else
                return (ulong)((680462.7536 + (3479.5308 * (Level / 2)) * Level) * ((Level % 10) + 8));
        }
        public void Loading()
        {
            Dictionary<uint, ITPlus[]> itemsplus = new Dictionary<uint, ITPlus[]>();

            string[] baseplusText = File.ReadAllLines(Program.ServerConfig.DbLocation + "ItemAdd.ini");
            foreach (string line in baseplusText)
            {
                string _item_ = line.Trim();
                ITPlus pls = new ITPlus();
                pls.Parse(_item_);
                if (itemsplus.ContainsKey(pls.ID))
                {
                    itemsplus[pls.ID][pls.Plus] = pls;
                }
                else
                {
                    ITPlus[] a_pls = new ITPlus[13];
                    a_pls[pls.Plus] = pls;
                    itemsplus.Add(pls.ID, a_pls);
                }
            }
            string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "itemtype.txt");
            foreach (string line in baseText)
            {
                string _item_ = line.Trim();
                if (_item_.Length > 11)
                {
                    if (_item_.IndexOf("//", 0, 2) != 0)
                    {
                        DBItem item = new DBItem();
                        item.Parse(line);
                        // if (item.ID == PowerExpBall)
                        //     continue;
                        if (itemsplus.ContainsKey(GetBaseID(item.ID)) || itemsplus.ContainsKey(GetBaseID(item.ID) + 10) || itemsplus.ContainsKey(GetBaseID(item.ID) + 20))
                        {
                            item.AllowUpgradePlus = true;
                            if (!itemsplus.TryGetValue(GetBaseID(item.ID), out item.Plus))
                            {
                                if (!itemsplus.TryGetValue(GetBaseID(item.ID) + 10, out item.Plus))
                                {
                                    if (!itemsplus.TryGetValue(GetBaseID(item.ID) + 20, out item.Plus))
                                    {
                                        int pos = ItemPosition(item.ID);
                                        if (pos < 6)
                                            Console.WriteLine("eroror item " + item.ID + " " + item.Name);
                                    }
                                }
                            }
                        }
                        if (!ContainsKey(item.ID))
                            Add(item.ID, item);
                    }
                }
            }
            MyConsole.WriteLine("Loading [" + Count + "] items");
            itemsplus = null;
            // [FastBoot, from Qahr] was GC.Collect(): a full blocking GC of every heap on each loader; the runtime collects on its own
        }
        public uint DowngradeItem(uint ID)
        {
            try
            {
                ushort Tryng = 0;
                ushort firstposition = ItemPosition(ID);
                uint rebornid = ID;
                while (true)
                {
                    if (Tryng > 1000)
                        break;
                    Tryng++;
                    //shield !
                    if (ID >= 900000 && ID <= 900309)
                    {
                        if (this[rebornid].Level <= 40)
                            break;
                    }
                    if (this[rebornid].Level <= ItemMinLevel((Role.Flags.ConquerItem)ItemPosition(ID)))
                    {
                        break;
                    }
                    if (this.ContainsKey(rebornid - 10))
                    {
                        rebornid -= 10;
                    }
                    else if (this.ContainsKey(rebornid - 20))
                    {
                        rebornid -= 20;
                    }
                    else if (this.ContainsKey(rebornid - 30))
                    {
                        rebornid -= 30;
                    }
                    else if (this.ContainsKey(rebornid - 40))
                    {
                        rebornid -= 40;
                    }
                }
                if (firstposition == ItemPosition(rebornid) && this.ContainsKey(rebornid))
                    return rebornid;
                else
                    return ID;

            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); return ID; }
        }
        public uint UpdateItem(uint id, out bool succesed)
        {
            ushort firstposition = ItemPosition(id);
            uint nextId = 0;
            if (this[id].Level < ItemMaxLevel((Role.Flags.ConquerItem)ItemPosition(id)))
            {
                if (this.ContainsKey(id + 10))
                {
                    nextId = id + 10;
                }
                else if (this.ContainsKey(id + 20))
                {
                    nextId = id + 20;
                }
                else if (this.ContainsKey(id + 30))
                {
                    nextId = id + 30;
                }
                else if (this.ContainsKey(id + 40))
                {
                    nextId = id + 40;
                }
            }
            if (firstposition == ItemPosition(nextId) && this.ContainsKey(nextId))
            {
                succesed = true;
                return nextId;
            }
            else
            {
                succesed = false;
                return id;
            }
        }
        public uint UpdateItem(uint id, Client.GameClient client)
        {
            uint nextId = id;
            if ((this[id].Level < ItemMaxLevel((Role.Flags.ConquerItem)ItemPosition(id))) && this[id].Level < client.Player.Level)
            {
                if (this.ContainsKey(id + 10))
                {
                    nextId = id + 10;
                }
            }
            return nextId;
        }
        public static byte ItemMinLevel(Role.Flags.ConquerItem postion)
        {
            switch (postion)
            {
                case 0: return 0;
                case Role.Flags.ConquerItem.Head: return 15;
                case Role.Flags.ConquerItem.Necklace: return 7;
                case Role.Flags.ConquerItem.Armor: return 15;
                case Role.Flags.ConquerItem.LeftWeapon: return 15;
                case Role.Flags.ConquerItem.RightWeapon: return 15;
                case Role.Flags.ConquerItem.Boots: return 10;
                case Role.Flags.ConquerItem.Ring: return 10;
                case Role.Flags.ConquerItem.Tower: return 0;
                case Role.Flags.ConquerItem.Fan: return 0;
                case Role.Flags.ConquerItem.Steed: return 0;
                case Role.Flags.ConquerItem.Garment: return 0;
                case Role.Flags.ConquerItem.RidingCrop: return 0;
            }
            return 0;
        }
        public static byte GetSashCounts(uint ID)
        {
            if (ID == 1100009) return 12;
            if (ID == 1100006) return 6;
            if (ID == 1100003) return 3;
            return 0;
        }
        public static bool IsSash(uint ID)
        {
            if (ID == 1100009 || ID == 1100006 || ID == 1100003) return true;
            return false;
        }
        public static bool IsPistol(uint ID)
        {
            return ID >= 612000 && ID <= 612439;
        }
        public static bool IsPickAxe(uint ID)
        {
            return ID == 562000;
        }
        public static bool IsRapier(uint ID)
        {
            return ID >= 611000 && ID <= 611439;
        }
        public static bool IsKnife(uint ID)
        {
            return ID >= 613000 && ID <= 613429;
        }
        public static bool IsShield(uint ID)
        {
            return ID >= 900000 && ID <= 900309;
        }
        public static bool IsHeavyRing(uint ID)
        {
            return ID >= 151013 && ID <= 151269;
        }
        public static bool IsTrojanEpicWeapon(uint ID)
        {
            return ID >= 614000 && ID <= 614439;
        }
        public static bool IsArmor(uint ID)
        {
            return (ID >= 130003 && ID <= 139309 || ID >= 101000 && ID <= 101309);
        }

        public static bool IsOneHand(uint ID)
        {
            var type = ID / 100000;
            return type == 4 || type == 6;
        }
        public static bool IsRing(uint ID)
        {
            return ID >= 150013 && ID <= 150269;
        }
        public static bool IsHelmet(uint ID)
        {
            return ((ID >= 111003 && ID <= 118309) || (ID >= 123000 && ID <= 123309) || (ID >= 141003 && ID <= 145309) || (ID >= 148000 && ID <= 148309)
              || ID >= 170000 && ID <= 170309);
        }
        public static bool IsBraclet(uint ID)
        {
            return ID >= 152013 && ID <= 152279;
        }
      
        public static bool IsNinjaEpicWeapon(uint ID)
        {
            return ID >= 616000 && ID <= 616439;
        }
        public static bool IsPrayedBead(uint ID)
        {
            return ID >= 610000 && ID <= 610439;
        }
        public static bool IsMonkEpicWeapon(uint ID)
        {
            return ID >= 622000 && ID <= 622439;

        }
        public static bool IsTalisman(uint ID)
        {
            return ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Tower || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Fan
                || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Wing;
        }

        public static bool AllowToUpdate(Role.Flags.ConquerItem Position)
        {
            if (Position == Role.Flags.ConquerItem.RidingCrop
                 || Position == Role.Flags.ConquerItem.Fan
                  || Position == Role.Flags.ConquerItem.Tower
                  || Position == Role.Flags.ConquerItem.Wing
                || Position == Role.Flags.ConquerItem.Garment
                || Position == Role.Flags.ConquerItem.AleternanteGarment
                || Position == Role.Flags.ConquerItem.AleternanteBottle
                || Position == Role.Flags.ConquerItem.Bottle
                || Position == Role.Flags.ConquerItem.Inventory
                || Position == Role.Flags.ConquerItem.LeftWeaponAccessory
                || Position == Role.Flags.ConquerItem.RightWeaponAccessory
                || Position == Role.Flags.ConquerItem.SteedMount
                || Position == Role.Flags.ConquerItem.Steed)
                return false;
            return true;
        }
        public static int TypeofItem(uint ID)
        {
            int ret = -1;
            uint Type1 = ID % 1000000;
            uint Type2 = Type1 / 1000;
            switch (Type2)
            {
                case 123:
                    return 1;
                case 203:
                    return 21;
                case 204:
                    return 22;
                case 300:
                    if (ID % 10 != 0)
                        return 14;
                    break;
                case 601:
                    return 4;
                case 201:
                case 202:
                    return 12;
                case 200:
                    return 20;
                case 350:
                    return 16;
                case 360:
                    return 15;
                case 370:
                    return 17;
                case 380:
                    return 18;
            }
            uint Type3 = ID % 10000000 / 100000;
            uint Type4 = Type1 / 10000;
            switch (Type3)
            {
                case 1:
                    switch (Type4)
                    {
                        case 11:
                        case 14:
                        case 17:
                            return 1;
                        case 18:
                        case 19:
                            return 11;
                        case 12:
                            return 2;
                        case 13:
                            return 3;
                        case 15:
                            return 7;
                        case 16:
                            return 8;
                    }
                    break;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 7:
                    return 9;
                case 9:
                    return 6;
                case 10:
                    return 0;
                default:
                    if (Type3 <= 29 && Type3 > 7)
                        return 9;
                    if (Type3 != 6)
                        return ret;
                    break;
            }
            return ret;
        }
        public static bool IsWindWalkerWeapon(uint ID)
        {
            return ID >= 626003 && ID <= 626439;
        }
        public static ushort ItemPosition(uint ID)
        {
            if (ID >= 203003 && ID <= 203009)
                return (ushort)Role.Flags.ConquerItem.RidingCrop;
            if (ID >= 200000 && ID <= 200999 || ID == 200613 || ID == 200614)
                return (ushort)Role.Flags.ConquerItem.SteedMount;


            if (ID >= 350001 && ID <= 370050)
                return (ushort)Role.Flags.ConquerItem.RightWeaponAccessory;
            if (ID >= 380001 && ID <= 380046 || ID == 380046 || ID == 370087 || ID == 370088 || ID == 370089 || ID == 370090 || ID == 380080 || ID == 380081 || ID == 380082 || ID == 380083|| ID == 370087)//380046 
                return (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory;

            if (ID >= 196605 && ID <= 196665 || ID == 134155 || ID == 131155 || ID == 133155 || ID == 130155 || ID == 194865 || ID == 194875 || ID == 196305 || ID == 196295 || ID == 196565 || ID == 196595 || ID == 199415 || ID == 196195 || ID == 198015 || ID == 198025 || ID == 198035 || ID == 198045)
                return (ushort)Role.Flags.ConquerItem.Garment;

            if ((ID >= 111003 && ID <= 118309) || (ID >= 123000 && ID <= 123309) || (ID >= 141003 && ID <= 145309) || (ID >= 148000 && ID <= 148309)
              || ID >= 170000 && ID <= 170309)
                return (ushort)Role.Flags.ConquerItem.Head;

            else if (ID >= 120001 && ID <= 121269)
                return (ushort)Role.Flags.ConquerItem.Necklace;

            else if (ID >= 130003 && ID <= 139309 || ID >= 101000 && ID <= 101309)
                return (ushort)Role.Flags.ConquerItem.Armor;

            else if (ID >= 150000 && ID <= 152279)
                return (ushort)Role.Flags.ConquerItem.Ring;

            else if (ID >= 160013 && ID <= 160249)
                return (ushort)Role.Flags.ConquerItem.Boots;

            else if (ID >= 198015 && ID <= 198115)
                return (ushort)Role.Flags.ConquerItem.Garment;

            else if (ID >= 181305 && ID <= 196700 || ID >= 199450 && ID <= 199975 || ID == 199945)
                return (ushort)Role.Flags.ConquerItem.Garment;
            else if (ID >= 193015 && ID <= 194875)
                return (ushort)Role.Flags.ConquerItem.Garment;
            else if (ID >= 201003 && ID <= 201009)
                return (ushort)Role.Flags.ConquerItem.Fan;

            else if (ID >= 202003 && ID <= 202009)
                return (ushort)Role.Flags.ConquerItem.Tower;

            else if (ID == 300000)
                return (ushort)Role.Flags.ConquerItem.Steed;

            else if (ID >= 410003 && ID <= 613429 || IsTrojanEpicWeapon(ID) || IsMonkEpicWeapon(ID) || IsNinjaEpicWeapon(ID) || (ID >= 617000 && ID <= 617439) || IsTaoistEpicWeapon(ID)
                || IsWarriorEpicWeapons(ID) || IsWindWalkerWeapon(ID))
                return (ushort)Role.Flags.ConquerItem.RightWeapon;

            else if ((ID >= 900000 && ID <= 900309) || (ID >= 1050000 && ID <= 1051000) || (ID >= 612000 && ID <= 612439)
                || (ID >= 612000 && ID <= 613429) || IsWarriorEpicWeapons(ID) || IsWindWalkerWeapon(ID)
                || (ID >= 611000 && ID <= 611439) || IsTrojanEpicWeapon(ID) || IsNinjaEpicWeapon(ID) || IsMonkEpicWeapon(ID) || (ID >= 617000 && ID <= 617439) || IsHossu(ID))
                return (ushort)Role.Flags.ConquerItem.LeftWeapon;

            else if (ID >= 2100025 && ID <= 2100125 || ID == 2100165 || ID == 2100175 ||
                ID == 2100185 || ID == 2100195 || ID == 2100205 || ID == 2100245)
                return (ushort)Role.Flags.ConquerItem.Bottle;
            else if (ID >= 204003 && ID <= 204009)
                return (ushort)Role.Flags.ConquerItem.Wing;

            return 0;
        }
        public static bool IsHossu(uint ID)
        {
            return ID >= 619000 && ID <= 619439;
        }
        public static bool IsTaoistEpicWeapon(uint ID)
        {
            return ID >= 620003 && ID <= 620439;
        }
        public static bool IsArrow(uint ID)
        {
            if (ID >= 1050000 && ID <= 1051000)
                return true;
            return false;
        }
        public static bool IsTwoHand(uint ID)
        {
            return ((ID.ToString()[0] == '5' || ID >= 421003 && ID <= 421439) ? true : false);
        }
        public static bool IsBow(uint ID)
        {
            return ID >= 500003 && ID <= 500429;
        }
        public static bool IsAccessory(uint ID)
        {
            return ID >= 350001 && ID <= 380015;
        }
        public static byte ItemMaxLevel(Role.Flags.ConquerItem Position)
        {
            switch (Position)
            {
                case 0: return 0;
                case Role.Flags.ConquerItem.Head: return 140;
                case Role.Flags.ConquerItem.Necklace: return 139;
                case Role.Flags.ConquerItem.Armor: return 140;
                case Role.Flags.ConquerItem.LeftWeapon: return 140;
                case Role.Flags.ConquerItem.RightWeapon: return 140;
                case Role.Flags.ConquerItem.Boots: return 129;
                case Role.Flags.ConquerItem.Ring: return 136;
                case Role.Flags.ConquerItem.Tower: return 100;
                case Role.Flags.ConquerItem.Fan: return 100;
                case Role.Flags.ConquerItem.Steed: return 0;
                case Role.Flags.ConquerItem.SteedMount: return 30;
                case Role.Flags.ConquerItem.Wing: return 100;
            }
            return 0;
        }
        public static bool IsBacksword(uint ID)
        {
            return ID >= 421003 && ID <= 421439;
        }
        public static bool IsKatana(uint ID)
        {
            return ID >= 601000 && ID <= 601439;
        }
        public static bool IsMonkWeapon(uint ID)
        {
            return ID >= 610000 && ID <= 610439;
        }
        public static bool IsWeapon(uint ID)
        {
            return ID >= 601000 && ID <= 601439;
        }

        public static bool EquipPassStatsReq(DBItem baseInformation, Client.GameClient client)
        {
            if (client.Player.Strength >= baseInformation.Strength && client.Player.Agility >= baseInformation.Agility)
                return true;
            else
                return false;
        }
        public static bool EquipPassJobReq(DBItem baseInformation, Client.GameClient client)
        {
            switch (baseInformation.Class)
            {
                #region WindWalker
                case 160: if (client.Player.Class <= 165 && client.Player.Class >= 160) return true; break;
                case 161: if (client.Player.Class <= 165 && client.Player.Class >= 161) return true; break;
                case 162: if (client.Player.Class <= 165 && client.Player.Class >= 162) return true; break;
                case 163: if (client.Player.Class <= 165 && client.Player.Class >= 163) return true; break;
                case 164: if (client.Player.Class <= 165 && client.Player.Class >= 164) return true; break;
                case 165: if (client.Player.Class == 165) return true; break;
                #endregion
                #region Trojan
                case 10: if (client.Player.Class <= 15 && client.Player.Class >= 10) return true; break;
                case 11: if (client.Player.Class <= 15 && client.Player.Class >= 11) return true; break;
                case 12: if (client.Player.Class <= 15 && client.Player.Class >= 12) return true; break;
                case 13: if (client.Player.Class <= 15 && client.Player.Class >= 13) return true; break;
                case 14: if (client.Player.Class <= 15 && client.Player.Class >= 14) return true; break;
                case 15: if (client.Player.Class == 15) return true; break;
                #endregion
                #region Warrior
                case 20: if (client.Player.Class <= 25 && client.Player.Class >= 20) return true; break;
                case 21: if (client.Player.Class <= 25 && client.Player.Class >= 21) return true; break;
                case 22: if (client.Player.Class <= 25 && client.Player.Class >= 22) return true; break;
                case 23: if (client.Player.Class <= 25 && client.Player.Class >= 23) return true; break;
                case 24: if (client.Player.Class <= 25 && client.Player.Class >= 24) return true; break;
                case 25: if (client.Player.Class == 25) return true; break;
                #endregion
                #region Archer
                case 40: if (client.Player.Class <= 45 && client.Player.Class >= 40) return true; break;
                case 41: if (client.Player.Class <= 45 && client.Player.Class >= 41) return true; break;
                case 42: if (client.Player.Class <= 45 && client.Player.Class >= 42) return true; break;
                case 43: if (client.Player.Class <= 45 && client.Player.Class >= 43) return true; break;
                case 44: if (client.Player.Class <= 45 && client.Player.Class >= 44) return true; break;
                case 45: if (client.Player.Class == 45) return true; break;
                #endregion
                #region Ninja
                case 50: if (client.Player.Class <= 55 && client.Player.Class >= 50) return true; break;
                case 51: if (client.Player.Class <= 55 && client.Player.Class >= 51) return true; break;
                case 52: if (client.Player.Class <= 55 && client.Player.Class >= 52) return true; break;
                case 53: if (client.Player.Class <= 55 && client.Player.Class >= 53) return true; break;
                case 54: if (client.Player.Class <= 55 && client.Player.Class >= 54) return true; break;
                case 55: if (client.Player.Class == 55) return true; break;
                #endregion
                #region Monk
                case 60: if (client.Player.Class <= 65 && client.Player.Class >= 60) return true; break;
                case 61: if (client.Player.Class <= 65 && client.Player.Class >= 61) return true; break;
                case 62: if (client.Player.Class <= 65 && client.Player.Class >= 62) return true; break;
                case 63: if (client.Player.Class <= 65 && client.Player.Class >= 63) return true; break;
                case 64: if (client.Player.Class <= 65 && client.Player.Class >= 64) return true; break;
                case 65: if (client.Player.Class == 65) return true; break;
                #endregion
                #region Taoist
                case 190: if (client.Player.Class >= 100) return true; break;
                #endregion
                #region Pirate
                case 70: if (client.Player.Class <= 75 && client.Player.Class >= 70) return true; break;
                case 71: if (client.Player.Class <= 75 && client.Player.Class >= 71) return true; break;
                case 72: if (client.Player.Class <= 75 && client.Player.Class >= 72) return true; break;
                case 73: if (client.Player.Class <= 75 && client.Player.Class >= 73) return true; break;
                case 74: if (client.Player.Class <= 75 && client.Player.Class >= 74) return true; break;
                case 75: if (client.Player.Class == 75) return true; break;
                #endregion
                #region LongLee
                case 80: if (client.Player.Class <= 85 && client.Player.Class >= 80) return true; break;
                case 81: if (client.Player.Class <= 85 && client.Player.Class >= 81) return true; break;
                case 82: if (client.Player.Class <= 85 && client.Player.Class >= 82) return true; break;
                case 83: if (client.Player.Class <= 85 && client.Player.Class >= 83) return true; break;
                case 84: if (client.Player.Class <= 85 && client.Player.Class >= 84) return true; break;
                case 85: if (client.Player.Class == 85) return true; break;
                #endregion
                case 0: return true;
                default: return false;
            }
            return false;
        }
        public static byte GetNextRefineryItem()
        {
            if (Extensions.BaseFunc.RandGet(100, false) < 30)
                return 2;
            if (Extensions.BaseFunc.RandGet(100, false) < 30)
                return 1;
            return 0;
        }

        public static byte GetLevel(uint ID)
        {
            if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Armor || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Head || IsShield(ID))
                return (byte)((ID % 100) / 10);
            else
                return (byte)((ID % 1000) / 10);
        }

        public static int GetUpEpLevelInfo(uint ID)
        {
            int cost = 0;
            int nLev = GetLevel(ID);

            if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Armor || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Head || IsShield(ID))
            {
                switch (nLev)
                {
                    case 5: cost = 50; break;
                    case 6: cost = 40; break;
                    case 7: cost = 30; break;
                    case 8:
                    case 9: cost = 20; break;
                    default: cost = 500; break;
                }

                int nQuality = (int)(ID % 10);
                switch (nQuality)
                {
                    case 6: cost = cost * 90 / 100; break;
                    case 7: cost = cost * 70 / 100; break;
                    case 8: cost = cost * 30 / 100; break;
                    case 9: cost = cost * 10 / 100; break;
                    default:
                        break;
                }
            }
            else
            {
                switch (nLev)
                {
                    case 11: cost = 95; break;
                    case 12: cost = 90; break;
                    case 13: cost = 85; break;
                    case 14: cost = 80; break;
                    case 15: cost = 75; break;
                    case 16: cost = 70; break;
                    case 17: cost = 65; break;
                    case 18: cost = 60; break;
                    case 19: cost = 55; break;
                    case 20: cost = 50; break;
                    case 21: cost = 45; break;
                    case 22: cost = 40; break;
                    default:
                        cost = 500;
                        break;
                }

                int nQuality = (int)(ID % 10);
                switch (nQuality)
                {
                    case 6: cost = cost * 90 / 100; break;
                    case 7: cost = cost * 70 / 100; break;
                    case 8: cost = cost * 30 / 100; break;
                    case 9: cost = cost * 10 / 100; break;
                    default:
                        break;
                }
            }
            return (100 / cost + 1) * 12 / 10;
        }
        public static int GetUpEpQualityInfo(uint ID)
        {
            var item = Server.ItemsBase[ID];
            int change = 100;
            switch (ID % 10)
            {
                case 6: change = 50; break;
                case 7: change = 33; break;
                case 8: change = 20; break;
                default: change = 100; break;
            }
            int level = item.Level;
            if (level > 70)
                change = (int)(change * (100 - (level - 70) * 1.0) / 100);

            return Math.Max(1, 100 / change);
        }
        public static bool UpQualityDB(uint ID, uint DBs)
        {
            int cost = GetUpEpQualityInfo(ID);
            if (DBs >= cost)
                return true;
            else
            {
                double percent = 100 / cost;
                double MyCost = DBs * percent;
                return Extensions.BaseFunc.RandGet(100, true) < MyCost;
            }
        }
        public static bool UpItemMeteors(uint ID, uint Meteors)
        {
            int CompleteCost = GetUpEpLevelInfo(ID);
            if (Meteors >= CompleteCost)
                return true;
            else
            {
                double percent = 100 / CompleteCost;
                double MyCost = Meteors * percent;
                return Extensions.BaseFunc.RandGet(100, true) < MyCost;
            }
        }
        public static int RateSuccForQuality(uint wQuality)
        {
            if (wQuality <= 6) return 30;
            else if (wQuality == 7) return 12;
            else if (wQuality == 8) return 6;//6
            else if (wQuality == 9) return 5;//4

            return 0;
        }

        public static bool EquipPassSexReq(DBItem baseInformation, Client.GameClient client)
        {
            int ClientGender = client.Player.Body % 10000 < 1005 ? 1 : 2;
            if (baseInformation.Gender == 2 && ClientGender == 2)
                return true;
            if (baseInformation.Gender != 2)
                return true;
            return false;
        }
        public static bool EquipPassRbReq(DBItem baseInformation, Client.GameClient client)
        {
            if (baseInformation.Level < 71 && client.Player.Reborn > 0 && client.Player.Level >= 70)
                return true;
            else
                return false;
        }
        public static bool EquipPassLvlReq(DBItem baseInformation, Client.GameClient client)
        {
            if (client.Player.Level < baseInformation.Level)
                return false;
            else
                return true;
        }
        public static bool Equipable(Game.MsgServer.MsgGameItem item, Client.GameClient client)
        {
            DBItem BaseInformation = null;
            if (Server.ItemsBase.TryGetValue((uint)item.ITEM_ID, out BaseInformation))
            {
                bool pass = false;
                if (!EquipPassSexReq(BaseInformation, client))
                    return false;
                // a reborn player may wear low (<= 70) items without their level/stat needs, but never another class's item
                if (EquipPassRbReq(BaseInformation, client) && EquipPassJobReq(BaseInformation, client))
                    pass = true;
                else
                    if (EquipPassJobReq(BaseInformation, client) && EquipPassStatsReq(BaseInformation, client) && EquipPassLvlReq(BaseInformation, client))
                        pass = true;
                if (!pass)
                    return false;

                if (client.Player.Reborn > 0)
                {
                    if (client.Player.Level >= 70 && BaseInformation.Level <= 70)
                        return pass;
                }
                return pass;
            }
            return false;
        }
        public static bool Equipable(uint ItemID, Client.GameClient client)
        {
            DBItem BaseInformation = null;
            if (Server.ItemsBase.TryGetValue(ItemID, out BaseInformation))
            {
                bool pass = false;

                if (EquipPassRbReq(BaseInformation, client) && EquipPassJobReq(BaseInformation, client))
                    pass = true;
                else
                    if (EquipPassJobReq(BaseInformation, client) && EquipPassLvlReq(BaseInformation, client))
                        pass = true;
                if (!pass)
                    return false;

                if (client.Player.Reborn > 0)
                {
                    if (client.Player.Level >= 70 && BaseInformation.Level <= 70)
                        return pass;
                }
                return pass;
            }
            return false;
        }
        public static uint GetBaseID(uint ID)
        {
            int itemtype = (int)(ID / 1000);
            if (ID == 300000)//steed
                return ID;
            switch ((byte)(ID / 10000))
            {
                case 20://tower/ Fan / RidingCrop
                case 14://hood`s. cap`s . band`s
                case 11://helment / hat / cap...
                case 90://shields
                case 13://armors
                case 12://necklace / bag / hood
                case 15://ring
                case 16://boots
                case 42:/*BackswordID*/
                    if (itemtype == 420)//Normal Sword
                        goto default;
                    ID = (uint)(ID - (ID % 10));
                    break;
                case 60:/*NinjaSwordID*/
                case 61:/*AssasinKnifeID*/
                case 62://epic tao backsword
                    ID = (uint)(ID - (ID % 10));
                    break;
                default://someting weapon`s / someting coronet`s earing`s / bow`s
                    {
                        byte def_val = (byte)(ID / 100000);
                        ID = (uint)(((def_val * 100000) + (def_val * 10000) + (def_val * 1000)) + ((ID % 1000) - (ID % 10)));
                        break;
                    }
            }
            return ID;
        }
        public class ITPlus
        {
            public uint ID;
            public byte Plus;
            public ushort ItemHP;
            public uint MinAttack;
            public uint MaxAttack;
            public ushort PhysicalDefence;
            public ushort MagicAttack;
            public ushort MagicDefence;
            public ushort Agility;
            public ushort Vigor { get { return Agility; } }
            public byte Dodge;
            public ushort SpeedPlus { get { return Dodge; } }
            public void Parse(string Line)
            {
                string[] Info = Line.Split(' ');
                ID = uint.Parse(Info[0]);
                Plus = byte.Parse(Info[1]);
                ItemHP = ushort.Parse(Info[2]);
                MinAttack = uint.Parse(Info[3]);
                MaxAttack = uint.Parse(Info[4]);
                PhysicalDefence = ushort.Parse(Info[5]);
                MagicAttack = ushort.Parse(Info[6]);
                MagicDefence = ushort.Parse(Info[7]);
                Agility = ushort.Parse(Info[8]);
                Dodge = byte.Parse(Info[9]);

            }
        }
        public class DBItem
        {
            public uint ID;
            public ITPlus[] Plus;
            public bool AllowUpgradePlus = false;
            public string Name;
            public byte Class;
            public byte Proficiency;
            public byte Level;
            public byte Gender;
            public ushort Strength;
            public ushort Agility;
            public uint GoldWorth;
            public ushort MinAttack;
            public ushort MaxAttack;
            public ushort PhysicalDefence;
            public ushort MagicDefence;
            public ushort MagicAttack;
            public byte Dodge;
            public ushort Frequency;
            public uint ConquerPointsWorth;
            public ushort Durability;
            public ushort StackSize;
            public ushort ItemHP;
            public ushort ItemMP;
            public ushort AttackRange;
            public ItemType Type;
            public string Description;
            public string LongDescription;
            public int GradeKey;

            public uint Crytical;
            public uint SCrytical;
            public uint Imunity;
            public uint Penetration;
            public uint Block;
            public uint BreackTrough;
            public uint ConterAction;
            public uint Detoxication;

            public uint MetalResistance = 0;
            public uint WoodResistance = 0;
            public uint WaterResistance = 0;
            public uint FireResistance = 0;
            public uint EarthResistance = 0;

            public ushort PurificationLevel;
            public ushort PurificationMeteorNeed;

            public uint Time;

            /// <summary>[HotReload] Parse filled every field (the line had all columns and no value failed).</summary>
            public bool ParsedOk;
            /// <summary>[HotReload] Set on the reload thread only: Parse then does not add the item to the static
            /// PurificationItems / Accessorys / SteedMounts / Garments tables the running server reads.</summary>
            [ThreadStatic] public static bool SkipStaticTables;

            public void Parse(string Line)
            {
                Plus = new ITPlus[13];
              
                string[] data = Line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                if (data[2] == "")
                {
                    for (int x = 2; x < data.Length - 1; x++)
                        data[x] = data[x + 1];
                }
                try
                {
                    if (data.Length > 52 && data[0] != "\0")
                    {
                        ID = Convert.ToUInt32(data[0]);
                        if (ID == 200626)
                        {
 
                        }
                        Name = data[1].Trim();
                        Class = Convert.ToByte(data[2]);
                        Proficiency = Convert.ToByte(data[3]);
                        Level = Convert.ToByte(data[4]);
                        Gender = Convert.ToByte(data[5]);
                        Strength = Convert.ToUInt16(data[6]);
                        Agility = Convert.ToUInt16(data[7]);
                        //Vitality = Convert.ToUInt16(data[8]);
                        //Spirit = Convert.ToUInt16(data[9]);
                        
                        Type = Convert.ToUInt32(data[10]) == 0 ? ItemType.Dropable : ItemType.Others;
                        GoldWorth = Convert.ToUInt32(data[12]);
                        MaxAttack = Convert.ToUInt16(data[14]);
                        MinAttack = Convert.ToUInt16(data[15]);
                        PhysicalDefence = Convert.ToUInt16(data[16]);
                        Frequency = Convert.ToUInt16(data[17]);
                        Dodge = Convert.ToByte(data[18]);
                        ItemHP = Convert.ToUInt16(data[19]);
                        ItemMP = Convert.ToUInt16(data[20]);
                        Durability = Convert.ToUInt16(data[22]);
                        MagicAttack = Convert.ToUInt16(data[30]);
                        MagicDefence = Convert.ToUInt16(data[31]);
                        AttackRange = Convert.ToUInt16(data[32]);
                        ConquerPointsWorth = Convert.ToUInt32(data[37]);

                        Time = Convert.ToUInt32(data[39]);
                        /*if (Time > 0)
                            MyConsole.WriteLine(data[0]);*/
                        // if (ID == 117309)
                        //     return;

                        Crytical = Convert.ToUInt32(data[40]);
                        SCrytical = Convert.ToUInt32(data[41]);
                        Imunity = Convert.ToUInt32(data[42]);
                        Penetration = Convert.ToUInt32(data[43]);
                        Block = Convert.ToUInt32(data[44]);
                        BreackTrough = Convert.ToUInt32(data[45]);
                        ConterAction = Convert.ToUInt32(data[46]);
                        Detoxication = Convert.ToUInt32(data[47]);

                        MetalResistance = Convert.ToByte(data[48]);
                        WoodResistance = Convert.ToByte(data[49]);
                        WaterResistance = Convert.ToByte(data[50]);
                        FireResistance = Convert.ToByte(data[51]);
                        EarthResistance = Convert.ToByte(data[52]);
                        //    StackSize = Convert.ToUInt16(data[47]);
                        ushort.TryParse(data[47].ToString(), out StackSize);


                        Description = data[53].Replace("`s", "");
                        if (Description == "NinjaKatana")
                            Description = "NinjaWeapon";
                        if (Description == "Earrings")
                            Description = "Earring";
                        if (Description == "Bow")
                            Description = "ArcherBow";
                        if (Description == "Backsword")
                            Description = "TaoistBackSword";
                        Description = Description.ToLower();
                        LongDescription = data[54];
                        if (ID >= 730001 && ID <= 730009)
                        {
                            Name = "(+" + (ID % 10).ToString() + ")Stone";
                        }

                        if (data.Length >= 58)
                        {
                            ushort.TryParse(data[56].ToString(), out PurificationLevel);
                            ushort.TryParse(data[57].ToString(), out PurificationMeteorNeed);

                            if (PurificationLevel != 0 && ID != 729305 && ID != 727465 && !SkipStaticTables)
                            {
                                if (!PurificationItems.ContainsKey(PurificationLevel))
                                    PurificationItems.Add(PurificationLevel, new Dictionary<uint, DBItem>());

                                if (!PurificationItems[PurificationLevel].ContainsKey(ID))
                                    PurificationItems[PurificationLevel].Add(ID, this);
                            }
                        }
                        if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                        {
                            if (!SkipStaticTables && !Accessorys.ContainsKey(ID))
                                Accessorys.Add(ID, this);
                            StackSize = 1;
                        }
                        if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.SteedMount)
                            if (!SkipStaticTables && !SteedMounts.ContainsKey(ID))
                                SteedMounts.Add(ID, this);
                        if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Garment)
                            if (!SkipStaticTables && !Garments.ContainsKey(ID))
                                Garments.Add(ID, this);
                        // CP prices come only from itemtype.txt (official 6609 values); the hard-coded
                        // overrides for 754099/754999/753999/751999/619028/723723/3005945 were removed.
                        ParsedOk = true;
                    }
                    else
                        MyConsole.WriteLine(data[0]);
                }
                catch (Exception e)
                { 
                    MyConsole.WriteLine(e.ToString());
                }
            }
            public enum ItemType : byte
            {
                Dropable = 0,
                Others
            }
        }
    }
}
