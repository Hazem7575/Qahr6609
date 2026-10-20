using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
    public class Equip
    {
        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> ClientItems = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();
        public uint SoulsPotency = 0;

        public int WeaponsMinAttack = 0;
        public uint ArmorID;
        public bool CreateSpawn = true;
        public bool SuperArmor = false;
        public bool FullSuper
        {
            get
            {
                if (!SuperArmor)
                    return false;
                foreach (var item in CurentEquip)
                {
                    if (item.Position != (ushort)Role.Flags.ConquerItem.Steed
                        && item.Position != (ushort)Role.Flags.ConquerItem.Garment
                        && item.Position != (ushort)Role.Flags.ConquerItem.Bottle
                        && item.Position != (ushort)Role.Flags.ConquerItem.AleternanteBottle
                        && item.Position != (ushort)Role.Flags.ConquerItem.AleternanteGarment
                        && item.Position != (ushort)Role.Flags.ConquerItem.SteedMount
                        && item.Position != (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                        && item.Position != (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory)
                    {
                        if (item.ITEM_ID % 10 != 9)
                            return false;
                    }
                }
                return true;
            }
        }

        public bool CanUpdatePerfectionItem(MsgGameItem item)
        {
            int count = CurentEquip.Where(p => p.PerfectionLevel == item.PerfectionLevel).Count();
            uint ItemsC = (uint)(Database.ItemType.IsTwoHand(RightWeapon) ? 0 : 1);
            if (Database.ItemType.IsShield(LeftWeapon) || Database.ItemType.IsHossu(LeftWeapon))
                ItemsC = 1;
            if (count == 11 + ItemsC)
                return true;
            uint bigLevel = 0;
            foreach (var _item in CurentEquip)
                if (_item.PerfectionLevel > bigLevel)
                    bigLevel = _item.PerfectionLevel;
            return bigLevel >= item.PerfectionLevel;
        }

        public Role.Flags.ItemEffect RightWeaponEffect = Flags.ItemEffect.None;
        public Role.Flags.ItemEffect RingEffect = Flags.ItemEffect.None;
        public Role.Flags.ItemEffect NecklaceEffect = Flags.ItemEffect.None;

        public bool UseMonkEpicWeapon = false;
        public uint ShieldID = 0;
        public uint RidingCrop = 0;
        public uint HeadID;
        public uint RightWeapon = 0;
        public uint LeftWeapon = 0;
        public byte SteedPlus { get { return (byte)Owner.Player.SteedPlus; } }
        public uint SteedPlusPorgres = 0;

        public int rangeR = 0;
        public int rangeL = 0;
        public int SizeAdd = 0;

        public int SpeedR = 0;
        public int SpeedL = 0;
        public int SpeedRing = 0;

        public bool SuperDragonGem = false;
        public bool SuperPheonixGem = false;
        public bool SuperVioletGem = false;
        public bool SuperRaibowGem = false;
        public bool SuperMoonGem = false;
        public bool SuprtTortoiseGem = false;
        public bool SuperKylinGem = false;
        public bool SuperFuryGem = false;
        public bool HaveBless = false;

        public int AttackSpeed(int MS_Delay )
        {
            MS_Delay = Math.Max(300, MS_Delay - 100);

            MS_Delay = Math.Max(300, MS_Delay - SpeedR);
            MS_Delay = Math.Max(300, MS_Delay - SpeedL);
            MS_Delay = Math.Max(300, MS_Delay - SpeedRing);
            MS_Delay = Math.Max(300, MS_Delay - Owner.Player.Agility / 2);

            if(Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                MS_Delay = Math.Max(300, MS_Delay - 150);

            return MS_Delay;
        }
        public int AttackSpeed(bool physical)
        {
            /*int speed = 800;
            speed = Math.Max(300, speed - SpeedR);
            speed = Math.Max(300, speed - SpeedL);
            speed = Math.Max(300, speed - SpeedRing);
            speed = Math.Max(300, speed - Owner.Player.Agility / 2);

            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                speed = Math.Max(300, speed - 150);

            return speed;*/
            return AttackSpeed(800);
        }
        public int GetAttackRange(int targetSizeAdd)
        {
            var range = 1;
          
            if (rangeR != 0 && rangeL != 0)
                range = (rangeR + rangeL) / 2;
            else if (rangeR != 0)
                range = rangeR;
            else if (rangeL != 0)
                range = rangeL;

            range += (SizeAdd + targetSizeAdd + 1) / 2;

            return range;
        }

        public bool Alternante = false;
        private Client.GameClient Owner;
        public Equip(Client.GameClient client)
        {
            Owner = client;
        }
        public Game.MsgServer.MsgGameItem[] CurentEquip = new Game.MsgServer.MsgGameItem[0];
        public bool UseEpicTrojanWeapon()
        {
            foreach (var item in CurentEquip)
            {
                if (item != null)
                {
                    if (Database.ItemType.IsTrojanEpicWeapon(item.ITEM_ID))
                        return true;
                }
            }
            return false;
        }
        public bool UseEpicNinjaWeapon()
        {
            foreach (var item in CurentEquip)
            {
                if (item != null)
                {
                    if (Database.ItemType.IsNinjaEpicWeapon(item.ITEM_ID))
                        return true;
                }
            }
            return false;
        }

        public unsafe bool Add(ServerSockets.Packet stream, uint ID, Role.Flags.ConquerItem position, byte plus = 0, byte bless = 0, byte Enchant = 0
           , Role.Flags.Gem sockone = Flags.Gem.NoSocket
            , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None)
        {
            if (FreeEquip(position))
            {
                Database.ItemType.DBItem DbItem;
                if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                {
                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.Effect = Effect;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    CheakUp(ItemDat);
                    ItemDat.Position = (ushort)position;
                    ItemDat.Mode = Flags.ItemMode.AddItem;

                    ItemDat.Send(Owner,stream);


                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, ItemDat.UID, ItemDat.Position, 0, 0, 0, 0));

                    return true;

                }
            }
            return false;
        }
        private void CheakUp(Game.MsgServer.MsgGameItem ItemDat)
        {
            if (ItemDat.UID == 0)
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
            if (!ClientItems.TryAdd(ItemDat.UID, ItemDat))
            {
                do
                {
                   // Console.WriteLine("Modifica uidul");
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                }
                while
                  (ClientItems.TryAdd(ItemDat.UID, ItemDat) == false);
            }
        }

        public bool Exist(Func<Game.MsgServer.MsgGameItem, bool> predicate)
        {
            bool Exist = false;
            foreach (var item in CurentEquip)
                if (predicate(item))
                {
                    Exist = true;
                    break;
                }
            return Exist;
        }
        public void Have(Func<Game.MsgServer.MsgGameItem, bool> predicate, out int count)
        {
            count = 0;
            foreach (var item in CurentEquip)
                if (predicate(item))
                {
                    count++;
                }
            
        }
        public bool Exist(Func<Game.MsgServer.MsgGameItem, bool> predicate, int count)
        {
            int counter = 0;
            foreach (var item in CurentEquip)
                if (predicate(item))
                {
                    counter++;
                }
            return counter >= count;
        }
        public ICollection<Game.MsgServer.MsgGameItem> AllItems
        {
            get { return ClientItems.Values; }
        }
        public bool TryGetValue(uint UID, out Game.MsgServer.MsgGameItem itemdata)
        {
            return ClientItems.TryGetValue(UID, out itemdata);
        }
        public bool TryGetCurentValue(uint UID, out Game.MsgServer.MsgGameItem itemdata)
        {
            itemdata = null;
            foreach (var item in CurentEquip)
            {
                if (item.UID == UID)
                    itemdata = item;
            }
            return itemdata != null;
        }
        public bool Contains(uint UID)
        {
            return ClientItems.ContainsKey(UID);
        }
        public Game.MsgServer.MsgGameItem GetItem(uint UID)
        {
            Game.MsgServer.MsgGameItem Item = null;
            ClientItems.TryGetValue(UID, out Item);
            return Item;
        }
        public bool FreeEquip(Role.Flags.ConquerItem position)
        {
            var item = ClientItems.Values.Where(p => p.Position == (ushort)position)
                .FirstOrDefault();
            return item == null;
        }
        public bool TryGetEquip(Role.Flags.ConquerItem position, out Game.MsgServer.MsgGameItem itemdata)
        {

            itemdata = ClientItems.Values.Where(p => p.Position == (ushort)position).FirstOrDefault();
            return itemdata != null;
        }
        public bool Remove(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            if (!FreeEquip(position))
            {
                if ((byte)position > 20)
                {
                    return RemoveAlternante(position, stream);
                }
                if (position == Flags.ConquerItem.Steed || position == Flags.ConquerItem.SteedMount)
                {
                    if (Owner.Player.ContainFlag(MsgUpdate.Flags.Ride))
                    {
                        Owner.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                    }
                }
                // a garment / mount goes back into the wardrobe, not into the bag: no free bag slot needed.
                bool Accept = position == Flags.ConquerItem.Garment || position == Flags.ConquerItem.SteedMount || Owner.Inventory.HaveSpace(1);
                if (Accept)
                {
                    Game.MsgServer.MsgGameItem itemdata;
                    if (TryGetEquip(position, out itemdata))
                    {
                        if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                        {
                            if (position == Flags.ConquerItem.Garment || position == Flags.ConquerItem.SteedMount)
                            {
                                itemdata.Position = 0;
                                // an old worn garment that was never stored would otherwise be in no container at all (lost on save).
                                if (!Owner.MyWardrobe.ContainUID(itemdata.UID) && !Owner.MyWardrobe.AddItem(itemdata))
                                {
                                    itemdata.Mode = Flags.ItemMode.AddItem;
                                    Owner.Inventory.Update(itemdata, AddMode.MOVE, stream);
                                }
                            }
                            else
                            {
                                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0));
                                itemdata.Position = 0;
                                itemdata.Mode = Flags.ItemMode.AddItem;
                                Owner.Inventory.Update(itemdata, AddMode.MOVE, stream);
                            }
                        }
                    }
                }
                else
                {
                    Owner.SendSysMesage("Your Inventory Is Full.");
                   
                }
                return Accept;
            }
            else
                return false;
        }
        public bool Delete(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            if (!FreeEquip(position))
            {
                if ((byte)position > 20)
                {
                    return DeleteAlternante(position, stream);
                }
                bool Accept = Owner.Inventory.HaveSpace(1);
                if (Accept)
                {
                    Game.MsgServer.MsgGameItem itemdata;
                    if (TryGetEquip(position, out itemdata))
                    {
                        if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                        {
                            if (position == Flags.ConquerItem.Garment || position == Flags.ConquerItem.SteedMount)
                            {
                                itemdata.Position = 0;
                            }
                            else
                            {
                                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0));
                                itemdata.Position = 0;
                                itemdata.Mode = Flags.ItemMode.AddItem;
                                Owner.Inventory.Update(itemdata, AddMode.MOVE, stream);// into the bag on both sides first, or REMOVE sends nothing and the client keeps a ghost
                                Owner.Inventory.Update(itemdata, AddMode.REMOVE, stream);
                            }
                        }
                    }
                }
                else
                {
                    Owner.SendSysMesage("Your Inventory Is Full.");

                }
                return Accept;
            }
            else
                return false;
        }
        public bool DeleteAlternante(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            bool Accept = Owner.Inventory.HaveSpace(1);
            if (Accept)
            {
                Game.MsgServer.MsgGameItem itemdata;
                if (TryGetEquip(position, out itemdata))
                {
                    if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                    {
                        Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0));
                        itemdata.Position = 0;
                        itemdata.Mode = Flags.ItemMode.AddItem;
                        Owner.Inventory.Update(itemdata, AddMode.REMOVE, stream);
                    }
                }
            }
            else
            {
                Owner.SendSysMesage("Your Inventory Is Full.");

            }
            return Accept;
        }
        public bool RemoveAlternante(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            bool Accept = Owner.Inventory.HaveSpace(1);
            if (Accept)
            {
                Game.MsgServer.MsgGameItem itemdata;
                if (TryGetEquip(position, out itemdata))
                {
                    if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                    {
                        Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0));
                        itemdata.Position = 0;
                        itemdata.Mode = Flags.ItemMode.AddItem;
                        Owner.Inventory.Update(itemdata, AddMode.MOVE, stream);
                    }
                }
            }
            else
            {
                Owner.SendSysMesage("Your Inventory Is Full.");

            }
            return Accept;
        }

        public void Add(Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            CheakUp(item);

            if (item.Position > 20)
            {
                AddAlternante(item, stream);
                return;
            }
            ClientItems.TryAdd(item.UID, item);
            item.Mode = Flags.ItemMode.AddItem;
            item.Send(Owner, stream);
            if (item.Position == (ushort)Role.Flags.ConquerItem.SteedMount || item.Position == (ushort)Role.Flags.ConquerItem.Garment)
            {
                Owner.MyWardrobe.AddItem(item);
            }
            
        }
        public void AddAlternante(Game.MsgServer.MsgGameItem itemdata, ServerSockets.Packet stream)
        {
            ClientItems.TryAdd(itemdata.UID, itemdata);
            itemdata.Mode = Flags.ItemMode.AddItem;

            Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0));

            itemdata.Send(Owner, stream);
        }
        public void Show(ServerSockets.Packet stream)
        {
            foreach (var item in ClientItems.Values)
            {
              
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner,stream);
            }
            QueryEquipment(Alternante);
        }
        public unsafe void ClearItemSpawn()
        {
            Owner.Player.ClearItemsSpawn();
        }
        public unsafe void AddSpawn(Game.MsgServer.MsgGameItem DataItem)
        {

            switch ((Role.Flags.ConquerItem)DataItem.Position)
            {
                case Role.Flags.ConquerItem.AleternanteArmor:
                case Role.Flags.ConquerItem.Armor:
                    {
                        Owner.Player.ArmorId = DataItem.ITEM_ID;
                        Owner.Player.ColorArmor = (ushort)DataItem.Color;
                        Owner.Player.ArmorSoul = DataItem.Purification.PurificationItemID;

                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteHead:
                case Role.Flags.ConquerItem.Head:
                    {

                        Owner.Player.HeadId = DataItem.ITEM_ID;
                        Owner.Player.ColorHelment = (ushort)DataItem.Color;
                        Owner.Player.HeadSoul = DataItem.Purification.PurificationItemID;
                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteLeftWeapon:
                case Role.Flags.ConquerItem.LeftWeapon:
                    {
                        Owner.Player.LeftWeaponId = DataItem.ITEM_ID;
                        Owner.Player.LeftWeapsonSoul = DataItem.Purification.PurificationItemID;
                        break;
                    }
                case Role.Flags.ConquerItem.LeftWeaponAccessory:
                    {
                        Owner.Player.LeftWeaponAccessoryId = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteRightWeapon:
                case Role.Flags.ConquerItem.RightWeapon:
                    {
                        Owner.Player.RightWeaponId = DataItem.ITEM_ID;
                        Owner.Player.ColorShield = (ushort)DataItem.Color;
                        Owner.Player.RightWeapsonSoul = DataItem.Purification.PurificationItemID;
                        break;
                    }
                case Role.Flags.ConquerItem.RightWeaponAccessory:
                    {
                        Owner.Player.RightWeaponAccessoryId = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.Steed:
                    {
                        Owner.Player.SteedId = DataItem.ITEM_ID;
                        Owner.Player.SteedColor = DataItem.SocketProgress;
                        Owner.Player.SteedPlus = DataItem.Plus;
                        SteedPlusPorgres = DataItem.PlusProgress;
                        break;
                    }
                case Role.Flags.ConquerItem.SteedMount:
                    {
                        Owner.Player.MountArmorId = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.AleternanteGarment:
                case Role.Flags.ConquerItem.Garment:
                    {

                        Owner.Player.GarmentId = DataItem.ITEM_ID;
                        break;
                    }
                case Role.Flags.ConquerItem.Wing:
                    {
                        if (DataItem.Plus == 12)
                            Owner.Player.WingProgress = DataItem.PlusProgress;
                        Owner.Player.WingId = DataItem.ITEM_ID;
                        Owner.Player.WingPlus = DataItem.Plus;
                        break;
                    }

            }
            if (Owner.Player.SpecialGarment != 0)
                Owner.Player.GarmentId = Owner.Player.SpecialGarment;
        }
        /// <summary>
        /// Everything Player.GetArray (the spawn packet) writes except the per-tick values the client
        /// gets elsewhere (timestamp, HP, X/Y, angle, action, away). Used to skip the full re-spawn to
        /// the screen when an equipment recompute changed nothing other players can see.
        /// </summary>
        private sealed class SpawnFingerprint
        {
            private const int Slots = 72;
            private readonly long[] V = new long[Slots];
            private readonly string[] S = new string[4];
            private int n;

            private void Add(long v)
            {
                if (n < Slots)
                    V[n] = v;
                n++;
            }
            public static SpawnFingerprint Capture(Client.GameClient owner)
            {
                try
                {
                    var p = owner.Player;
                    var f = new SpawnFingerprint();
                    f.Add(p.Mesh); f.Add(p.GuildID); f.Add((long)p.GuildRank);
                    long bitsHash = unchecked((long)14695981039346656037UL);
                    var bits = p.BitVector.bits;
                    for (int x = 0; x < bits.Length; x++)
                        bitsHash = unchecked((bitsHash ^ (long)bits[x]) * 1099511628211L);
                    f.Add(bitsHash); f.Add(bits.Length);
                    f.Add(p.AparenceType);
                    f.Add(p.HeadId); f.Add(p.GarmentId); f.Add(p.ArmorId);
                    f.Add(p.LeftWeaponId); f.Add(p.RightWeaponId);
                    f.Add(p.LeftWeaponAccessoryId); f.Add(p.RightWeaponAccessoryId);
                    f.Add(p.SteedId); f.Add(p.MountArmorId); f.Add(p.WingId);
                    f.Add(p.WingPlus); f.Add(p.WingProgress);
                    f.Add(p.Hair); f.Add(p.Reborn); f.Add(p.Level);
                    f.Add(p.ExtraBattlePower); f.Add(p.FlowerRank); f.Add((long)p.NobilityRank);
                    f.Add(p.ColorArmor); f.Add(p.ColorShield); f.Add(p.ColorHelment);
                    f.Add(p.SteedPlus); f.Add(p.SteedColor); f.Add(p.Enilghten);
                    f.Add(p.ClanUID); f.Add((long)p.ClanRank); f.Add((long)p.MyTitle);
                    f.Add(p.PokerTableID); f.Add(p.PokerSeat); f.Add(owner.IsWatching() ? 1 : 0);
                    f.Add(p.HeadSoul); f.Add(p.ArmorSoul); f.Add(p.LeftWeapsonSoul); f.Add(p.RightWeapsonSoul);
                    f.Add((long)p.ActiveSublass); f.Add(p.SubClassHasPoints);
                    f.Add(p.FirstClass); f.Add(p.SecoundeClass); f.Add(p.Class);
                    f.Add(p.CountryID); f.Add(owner.Team != null ? (long)owner.Team.UID : 0);
                    f.Add(p.BattlePower); f.Add(p.JiangHuTalent); f.Add(p.JiangHuActive);
                    f.Add(p.OnMyOwnServer ? 1 : 0); f.Add(p.ServerID); f.Add(p.RealUID);
                    bool inUnion = p.InUnion;
                    f.Add(inUnion ? 1 : 0); f.Add((long)p.ExploitsRank);
                    if (inUnion)
                    {
                        f.Add(p.MyUnion.UID); f.Add((long)p.UnionMemeber.Rank); f.Add((long)p.MyUnion.IsKingdom);
                        f.S[3] = p.MyUnion.Name;
                    }
                    f.Add(p.SpecialTitleID); f.Add(p.SpecialTitleScore); f.Add(p.SpecialWingID);
                    f.Add((long)p.MainFlag);
                    f.S[0] = p.Name;
                    f.S[1] = p.ClanName;
                    f.S[2] = p.MyGuild != null ? p.MyGuild.GuildName : null;
                    if (f.n > Slots)
                        return null; // never silently truncate: treat as "changed"
                    return f;
                }
                catch
                {
                    return null;
                }
            }
            public static bool Same(SpawnFingerprint a, SpawnFingerprint b)
            {
                if (a == null || b == null || a.n != b.n)
                    return false;
                for (int x = 0; x < a.n; x++)
                    if (a.V[x] != b.V[x])
                        return false;
                for (int x = 0; x < a.S.Length; x++)
                    if (!string.Equals(a.S[x], b.S[x]))
                        return false;
                return true;
            }
        }
        // What this recompute last broadcast to the screen (null = nothing yet / unknown).
        private SpawnFingerprint LastSpawnSent;

        public unsafe void UpdateStats(Game.MsgServer.MsgGameItem[] MyGear, ServerSockets.Packet stream)
        {

            try
            {
                // Spawn state before this recompute (only needed when we may re-spawn to the screen).
                SpawnFingerprint spawnBefore = CreateSpawn ? SpawnFingerprint.Capture(Owner) : null;
                Owner.PrestigeLevel = 0;
                SoulsPotency = 0;
                rangeR = rangeL = SizeAdd = 0;
                SpeedR = SpeedL = SpeedRing = 0;
                RightWeapon = 0;
                LeftWeapon = 0;
                UseMonkEpicWeapon = false;
                SuperArmor = false;
                HeadID = 0;
                WeaponsMinAttack = 0;
                HaveBless = false;
                RingEffect = Flags.ItemEffect.None;
                RightWeaponEffect = Flags.ItemEffect.None;
                SteedPlusPorgres = 0;
                Owner.Status.MaxVigor = 0;
                RidingCrop = 0;
                if (CreateSpawn)
                {
                    lock (CurentEquip)
                        CurentEquip = MyGear;
                    ClearItemSpawn();
                }
                Owner.Status = new MsgStatus();
                Owner.Status.UID = Owner.Player.UID;

                Owner.Status.MaxAttack = (ushort)(Owner.Player.Strength  + 1);
                Owner.Status.MinAttack = (ushort)(Owner.Player.Strength);
                Owner.Status.MagicAttack = Owner.Player.Spirit;// (ushort)(Owner.Player.Spirit * 10);

                Owner.Gems = new ushort[13];

                foreach (var item in MyGear)
                {

                    if (Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Head
                        || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.AleternanteHead)
                        HeadID = item.ITEM_ID;
                    //aici sa fac schimbare de position !!!!!!!!!!!
                    Owner.PrestigeLevel += item.PerfectionLevel;


                    if (Database.ItemType.IsHossu(item.ITEM_ID))
                    {
                        if (item.Bless > 1)
                            item.Bless = 1;
                    }
                    try
                    {

                        if (CreateSpawn)
                            AddSpawn(item);

                        if (item.Durability == 0)
                            continue;

                        ushort ItemPostion = (ushort)(item.Position % 20);


                        if (item.Bless >= 1)
                            HaveBless = true;
                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.Armor)
                        {
                            SuperArmor = (item.ITEM_ID % 10) == 9;

                            ArmorID = item.ITEM_ID;
                        }
                        if (item.SocketOne != Role.Flags.Gem.NoSocket && item.SocketOne != Role.Flags.Gem.EmptySocket)
                        {
                            if (item.SocketOne == Role.Flags.Gem.SuperTortoiseGem)
                                SuprtTortoiseGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperDragonGem)
                                SuperDragonGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperPhoenixGem)
                                SuperPheonixGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperVioletGem)
                                SuperVioletGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperRainbowGem)
                                SuperRaibowGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperMoonGem)
                                SuperMoonGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperKylinGem)
                                SuperKylinGem = true;
                            if (item.SocketOne == Role.Flags.Gem.SuperFuryGem)
                                SuperFuryGem = true;
                        }

                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.RidingCrop)
                        {
                            RidingCrop = item.ITEM_ID;
                            Owner.Status.MaxVigor += 1000;
                        }
                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                        {
                            if (Database.ItemType.IsMonkEpicWeapon(item.ITEM_ID))
                                UseMonkEpicWeapon = true;
                            LeftWeapon = item.ITEM_ID;
                            if (Database.ItemType.IsShield(item.ITEM_ID))
                                ShieldID = item.ITEM_ID;
                        }
                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
                        {
                            if (Database.ItemType.IsMonkEpicWeapon(item.ITEM_ID))
                                UseMonkEpicWeapon = true;
                            RightWeaponEffect = item.Effect;
                            RightWeapon = item.ITEM_ID;
                        }

                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.Ring)
                            RingEffect = item.Effect;

                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.Necklace)
                            NecklaceEffect = item.Effect;
                        AddGem(item.SocketOne);
                        AddGem(item.SocketTwo);

                        var DBItem = Database.Server.ItemsBase[item.ITEM_ID];
                        item.ItemPoints = Database.ItemType.GetItemPoints(DBItem, item);
                        if (item.GetPerfectionPosition != -1)
                        {
                            if (item.OwnerUID != 0)
                            {
                                if (Database.GroupServerList.MyServerInfo.ID == Owner.Player.ServerID)
                                    Database.RankItems.RankPoll[(uint)item.GetPerfectionPosition].AddItem(item);
                            }
                        }
                        // Console.WriteLine(item.Position + " " + item.ItemPoints);
                        if (ItemPostion == (byte)Role.Flags.ConquerItem.Fan || ItemPostion == (byte)Role.Flags.ConquerItem.Wing)
                        {
                            Owner.Status.PhysicalDamageIncrease += DBItem.MaxAttack;
                            Owner.Status.MagicDamageIncrease += DBItem.MagicAttack;
                        }
                        else
                        {
                            if (ItemPostion == (ushort)Role.Flags.ConquerItem.Ring)
                            {
                                SpeedRing = DBItem.Frequency;
                            }
                            if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon)
                            {
                                rangeL = DBItem.AttackRange;
                                SpeedL = DBItem.Frequency;

                                WeaponsMinAttack += (int)(DBItem.MaxAttack / 2);
                                Owner.Status.MaxAttack += (uint)(DBItem.MaxAttack / 2);
                                Owner.Status.MinAttack += (uint)(DBItem.MinAttack / 2);
                                Owner.Status.MagicAttack += (uint)(DBItem.MagicAttack / 2);
                            }
                            else
                            {
                                if (ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
                                {
                                    WeaponsMinAttack += DBItem.MinAttack;
                                    rangeR = DBItem.AttackRange;
                                    SpeedR = DBItem.Frequency;
                                }
                            
                                Owner.Status.MaxAttack += DBItem.MaxAttack;
                                Owner.Status.MinAttack += DBItem.MinAttack;
                                Owner.Status.MagicAttack += DBItem.MagicAttack;
                            }
                        }
                        if (ItemPostion == (byte)Role.Flags.ConquerItem.Tower || ItemPostion == (byte)Role.Flags.ConquerItem.Wing)
                        {
                            Owner.Status.MagicDamageDecrease += DBItem.MagicDefence;
                            Owner.Status.PhysicalDamageDecrease += DBItem.PhysicalDefence;
                        }
                        else
                        {
                            Owner.Status.Immunity += DBItem.Imunity;
                            Owner.Status.CriticalStrike += DBItem.Crytical;
                            Owner.Status.SkillCStrike += DBItem.SCrytical;
                            Owner.Status.Breakthrough += DBItem.BreackTrough;
                            Owner.Status.Counteraction += DBItem.ConterAction;
                            Owner.Status.MDefence += (byte)DBItem.MagicDefence;
                            Owner.Status.Defence += DBItem.PhysicalDefence;
                        }

                        if (ItemPostion != (byte)Role.Flags.ConquerItem.Steed)
                        {
                            Owner.Status.Dodge += DBItem.Dodge;
                            Owner.Status.AgilityAtack += DBItem.Frequency;//.Agility;
                            Owner.Status.ItemBless += item.Bless;
                            Owner.Status.MaxHitpoints += item.Enchant;
                        }
                        Owner.Status.MaxHitpoints += DBItem.ItemHP;
                        Owner.Status.MaxMana += DBItem.ItemMP;

                        Owner.Status.Block += DBItem.Block;
                        Owner.Status.Penetration += DBItem.Penetration;
                     
                        if (item.Purification.InLife)
                        {
                            var purificare = Database.Server.ItemsBase[item.Purification.PurificationItemID];
                            Owner.Status.MaxAttack += purificare.MaxAttack;
                            Owner.Status.MinAttack += purificare.MinAttack;
                            Owner.Status.MagicAttack += purificare.MagicAttack;
                            Owner.Status.MDefence += (byte)purificare.MagicDefence;
                            Owner.Status.Defence += purificare.PhysicalDefence;

                            Owner.Status.CriticalStrike += purificare.Crytical;
                            Owner.Status.SkillCStrike += purificare.SCrytical;
                            Owner.Status.Immunity += purificare.Imunity;
                            Owner.Status.Penetration += purificare.Penetration;
                            Owner.Status.Block += purificare.Block;
                            Owner.Status.Breakthrough += purificare.BreackTrough;
                            Owner.Status.Counteraction += purificare.ConterAction;
                            Owner.Status.Detoxication += purificare.Detoxication;

                            Owner.Status.MetalResistance += purificare.MetalResistance;
                            Owner.Status.WoodResistance += purificare.WoodResistance;
                            Owner.Status.FireResistance += purificare.FireResistance;
                            Owner.Status.EarthResistance += purificare.EarthResistance;
                            Owner.Status.WaterResistance += purificare.WaterResistance;

                            Owner.Status.MaxHitpoints += purificare.ItemHP;
                            Owner.Status.MaxMana += purificare.ItemMP;
                            SoulsPotency += item.Purification.PurificationLevel;
                        }

                        if (item.Refinary.InLife)
                        {
                            IncreaseRifainaryStatus(item.Refinary.EffectID);
                        }
                        if (item.SocketOne != Flags.Gem.NoSocket && item.SocketOne != Flags.Gem.EmptySocket)
                        {
                            if (item.SocketOne == Flags.Gem.NormalTortoiseGem)
                            {
                                Owner.Status.Damage += 2;
                            }
                            if (item.SocketOne == Flags.Gem.RefinedTortoiseGem)
                            {
                                Owner.Status.Damage += 4;
                            }
                            if (item.SocketOne == Flags.Gem.SuperTortoiseGem)
                            {
                                Owner.Status.Damage += 6;
                            }
                        }
                        if (item.SocketTwo != Flags.Gem.NoSocket && item.SocketTwo != Flags.Gem.EmptySocket)
                        {
                            if (item.SocketTwo == Flags.Gem.NormalTortoiseGem)
                            {
                                Owner.Status.Damage += 2;
                            }
                            if (item.SocketTwo == Flags.Gem.RefinedTortoiseGem)
                            {
                                Owner.Status.Damage += 4;
                            }
                            if (item.SocketTwo == Flags.Gem.SuperTortoiseGem)
                            {
                                Owner.Status.Damage += 6;
                            }
                        }
                        if (item.Plus > 0 && item.Plus < 13)
                        {
                            if (item.Plus > 12) item.Plus = 0;//7kha
                            var extraitematributes = DBItem.Plus[item.Plus];
                            if (extraitematributes != null)
                            {
                                if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon || ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
                                {
                                    Owner.Status.Accuracy += extraitematributes.Agility;
                                }

                                if (ItemPostion == (byte)Role.Flags.ConquerItem.Steed)
                                    Owner.Status.MaxVigor = extraitematributes.Agility;

                                if (ItemPostion == (byte)Role.Flags.ConquerItem.Fan || ItemPostion == (byte)Role.Flags.ConquerItem.Wing)
                                {
                                    Owner.Status.PhysicalDamageIncrease += extraitematributes.MaxAttack;
                                    Owner.Status.MagicDamageIncrease += extraitematributes.MagicAttack;
                                }
                                else
                                {
                                  /*  if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon)// just for main attack
                                    {
                                        Owner.Status.MaxAttack += extraitematributes.MaxAttack / 2;
                                        Owner.Status.MinAttack += extraitematributes.MinAttack / 2;
                                        Owner.Status.MagicAttack += extraitematributes.MagicAttack;
                                    }
                                    else*/
                                    {
                                        Owner.Status.MaxAttack += extraitematributes.MaxAttack;
                                        Owner.Status.MinAttack += extraitematributes.MinAttack;
                                        Owner.Status.MagicAttack += extraitematributes.MagicAttack;
                                    }
                                }
                                if (ItemPostion == (byte)Role.Flags.ConquerItem.Tower || ItemPostion == (byte)Role.Flags.ConquerItem.Wing)
                                {
                                    Owner.Status.MagicDamageDecrease += extraitematributes.MagicDefence;
                                    Owner.Status.PhysicalDamageDecrease += extraitematributes.PhysicalDefence;
                                }
                                else
                                {
                                    Owner.Status.MagicDefence += extraitematributes.MagicDefence;
                                    Owner.Status.Defence += extraitematributes.PhysicalDefence;
                                }


                                if (ItemPostion != (byte)Role.Flags.ConquerItem.Steed)
                                {
                                    Owner.Status.Dodge += extraitematributes.Dodge;
                                    //   Owner.Status.AgilityAtack += extraitematributes.Agility;
                                }
                                Owner.Status.MaxHitpoints += extraitematributes.ItemHP;
                            }
                            else
                                MyConsole.WriteLine("Invalid Plus -> item " + item.ITEM_ID.ToString() + " ->  plus " + item.Plus.ToString() + "");
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }
                }
              
                //add gem stats
               // Owner.Status.MinAttack = (uint)(Owner.Status.MinAttack * Owner.GemValues(Role.Flags.Gem.NormalDragonGem)) / 100;
               //  Owner.Status.MaxAttack = (uint)(Owner.Status.MaxAttack * Owner.GemValues(Role.Flags.Gem.NormalDragonGem)) / 100;
                
               // Console.WriteLine(Owner.Status.MaxAttack);
                Owner.Status.MagicAttack += 1;
                Owner.Status.MagicDefence += Owner.GemValues(Role.Flags.Gem.NormalGloryGem);
                Owner.Status.PhysicalDamageDecrease += Owner.GemValues(Role.Flags.Gem.NormalGloryGem);

                Owner.Status.PhysicalDamageIncrease += Owner.GemValues(Role.Flags.Gem.NormalThunderGem);
                Owner.Status.MagicDamageIncrease += Owner.GemValues(Role.Flags.Gem.NormalThunderGem);

                //  Owner.Status.MagicAttack += (uint)(Owner.Status.MagicAttack * Owner.GemValues(Role.Flags.Gem.NormalPhoenixGem)) / 100;
                // Owner.Status.ItemBless += Owner.GemValues(Role.Flags.Gem.NormalTortoiseGem);

                AddChiAtribute(Owner.Player.MyChi);
                AddInnerPowerAtributes(Owner.Player.InnerPower);

                Owner.Status.MaxHitpoints += Owner.CalculateHitPoint();
                Owner.Status.MaxMana += Owner.CalculateMana();

                Owner.Vigor = (ushort)Math.Min((int)Owner.Vigor, (int)Owner.Status.MaxVigor);
                if (CreateSpawn)
                    Owner.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, Owner.Vigor));
                CalculateBattlePower();

                if (CreateSpawn)
                {
                    // Re-spawn to the screen only when something other players see changed, either in
                    // this recompute (before/after) or since the last spawn this path sent (covers
                    // callers that change a visible field and rely on this call to broadcast it).
                    var spawnAfter = SpawnFingerprint.Capture(Owner);
                    if (!SpawnFingerprint.Same(spawnBefore, spawnAfter) || !SpawnFingerprint.Same(LastSpawnSent, spawnAfter))
                    {
                        Owner.Player.View.SendView(Owner.Player.GetArray(stream, false), false);
                        LastSpawnSent = spawnAfter;
                    }
                }

                Owner.Player.CheckAura();

                Owner.Player.SubClass.UpdateStatus(Owner);

                if (Owner.Player.MyJiangHu != null)
                    Owner.Player.MyJiangHu.CreateStatusAtributes(Owner);


                Owner.Status.MaxAttack += (uint)Owner.PerfectionStatus.PhysicalAttack;
                Owner.Status.MinAttack += (uint)Owner.PerfectionStatus.PhysicalAttack;
                Owner.Status.MagicAttack += (uint)Owner.PerfectionStatus.MagicAttack;
                Owner.Status.MagicDefence += (uint)Owner.PerfectionStatus.MagicDefense;
                Owner.Status.Defence += (uint)Owner.PerfectionStatus.PhysicalDefence;

                Owner.Status.PrestigeLevel = Owner.PrestigeLevel;
                Owner.CreatePrestigePoints();
                Owner.Send(stream.StatusCreate(Owner.Status));
                SendMentorShare(stream);
                //Database.TitleStorage.CheckUpUser(Owner, stream);
            
                if (Database.ItemType.IsMonkEpicWeapon(LeftWeapon) || Database.ItemType.IsMonkEpicWeapon(RightWeapon))
                {
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UpSweep))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.UpSweep, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DownSweep))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DownSweep, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Strike))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Strike, 0, 0);
                }
                if (Database.ItemType.IsWarriorEpicWeapons(RightWeapon))
                {
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LeftHook))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.LeftHook, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RightHook))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.RightHook, 0, 0);
                    if (!Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StraightFist))
                        Owner.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StraightFist, 0, 0);
                }
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.FreezingPelter))
                {
                    var DBSpells = Database.Server.Magic[(ushort)Role.Flags.SpellID.FreezingPelter];
                    MsgSpell clientspell;
                    if (Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FreezingPelter, out clientspell))
                    {
                        var DBSpell = DBSpells[(ushort)Math.Min(clientspell.Level, DBSpells.Count - 1)];
                        Owner.Status.MinAttack += (uint)(WeaponsMinAttack * DBSpell.Damage2 / 100);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
       
        public void AddChiAtribute(Role.Instance.Chi chi, uint percent = 100)
        {
           
            Owner.Status.CriticalStrike += chi.CriticalStrike * percent / 100;
            Owner.Status.SkillCStrike += chi.SkillCriticalStrike * percent / 100;
            Owner.Status.Immunity += chi.Immunity * percent / 100;
            Owner.Status.Counteraction += chi.Counteraction * percent / 100;
            Owner.Status.Breakthrough += chi.Breakthrough * percent / 100;
            Owner.Status.MaxHitpoints += chi.MaxLife * percent / 100;
            Owner.Status.MaxAttack += chi.AddAttack * percent / 100;
            Owner.Status.MinAttack += chi.AddAttack * percent / 100;
            Owner.Status.MagicAttack += chi.AddMagicAttack * percent / 100;
            Owner.Status.MagicDefence += chi.AddMagicDefense * percent / 100;

            Owner.Status.PhysicalDamageIncrease += chi.FinalAttack * percent / 100;
            Owner.Status.PhysicalDamageDecrease += chi.FinalDefense * percent / 100;
            Owner.Status.MagicDamageIncrease += chi.FinalMagicAttack * percent / 100;
            Owner.Status.MagicDamageDecrease += chi.FinalMagicDefense * percent / 100;

            
        }
        public void AddInnerPowerAtributes(Role.Instance.InnerPower Inner, uint percent = 100)
        {

            Owner.Status.CriticalStrike += Inner.CriticalStrike * percent / 100;
            Owner.Status.SkillCStrike += Inner.SkillCriticalStrike * percent / 100;
            Owner.Status.Immunity += Inner.Immunity * percent / 100;
            Owner.Status.Counteraction += Inner.Counteraction * percent / 100;
            Owner.Status.Breakthrough += Inner.Breakthrough * percent / 100;
            Owner.Status.MaxHitpoints += Inner.MaxLife * percent / 100;
            Owner.Status.MaxAttack += Inner.AddAttack * percent / 100;
            Owner.Status.MinAttack += Inner.AddAttack * percent / 100;
            Owner.Status.MagicAttack += Inner.AddMagicAttack * percent / 100;
            Owner.Status.MagicDefence += Inner.AddMagicDefense * percent / 100;

            Owner.Status.PhysicalDamageIncrease += Inner.FinalAttack * percent / 100;
            Owner.Status.PhysicalDamageDecrease += Inner.FinalDefense * percent / 100;
            Owner.Status.MagicDamageIncrease += Inner.FinalMagicAttack * percent / 100;
            Owner.Status.MagicDamageDecrease += Inner.FinalMagicDefense * percent / 100;
          
            

            Owner.Status.Defence += Inner.Defence * percent / 100;
        }
        public unsafe void SendMentorShare(ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgApprenticeInformation Information = Game.MsgServer.MsgApprenticeInformation.Create();
            Information.Mode = Game.MsgServer.MsgApprenticeInformation.Action.Mentor;
            if (Owner.Player.MyMentor != null)
            {
                if (Owner.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Mentor))
                {
                    if (Owner.Player.MyMentor.MyClient != null)
                    {
                        if (Owner.Player.Associate.Associat[Role.Instance.Associate.Mentor].ContainsKey(Owner.Player.MyMentor.MyUID))
                        {
                            Role.Player mentor = Owner.Player.MyMentor.MyClient.Player;
                            Owner.Player.SetMentorBattlePowers(mentor.GetShareBattlePowers((uint)Owner.Player.RealBattlePower), (uint)mentor.RealBattlePower);

                            Information.Mentor_ID = mentor.UID;
                            Information.Apprentice_ID = Owner.Player.UID;
                            Information.Enrole_date = (uint)Owner.Player.Associate.Associat[Role.Instance.Associate.Mentor][mentor.UID].Timer;
                            Information.Fill(mentor.Owner);
                            Information.Shared_Battle_Power = mentor.GetShareBattlePowers((uint)Owner.Player.RealBattlePower);
                            Information.WriteString(mentor.Name, Owner.Player.Spouse, Owner.Player.Name);
                            Owner.Send(Information.GetArray(stream));
                        }
                    }

                }
            }
            if (!Owner.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                return;

            foreach (var Apprentice in Owner.Player.Associate.OnlineApprentice.Values)
            {
                if (!Owner.Player.Associate.Associat[Role.Instance.Associate.Apprentice].ContainsKey(Apprentice.Player.UID))
                    continue;
                Role.Player target = Apprentice.Player;

                target.SetMentorBattlePowers(Owner.Player.GetShareBattlePowers((uint)target.RealBattlePower), (uint)Owner.Player.RealBattlePower);

                Information.Apprentice_ID = target.UID;
                Information.Enrole_date = (uint)Owner.Player.Associate.Associat[Role.Instance.Associate.Apprentice][target.UID].Timer;
                Information.Level = (byte)Owner.Player.Level;
                Information.Class = Owner.Player.Class;
                Information.PkPoints = Owner.Player.PKPoints;
                Information.Mesh = Owner.Player.Mesh;
                Information.Online = 1;
                Information.Shared_Battle_Power = Owner.Player.GetShareBattlePowers((uint)target.RealBattlePower);
                Information.WriteString(Owner.Player.Name, Owner.Player.Spouse, target.Name);
                target.Owner.Send(Information.GetArray(stream));
            }

        }
        public void IncreaseRifainaryStatus(uint ID)
        {
            Database.Rifinery.Item refinery;
            if (Database.Server.RifineryItems.TryGetValue(ID, out refinery))
            {
                // var refinery = Database.Server.RifineryItems[ID];
                switch (refinery.Type)
                {
                    case Database.Rifinery.RefineryType.CriticalStrike:
                        {
                            Owner.Status.CriticalStrike += refinery.Procent * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.SkillCriticalStrike:
                        {
                            Owner.Status.SkillCStrike += refinery.Procent * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Break:
                        {
                            Owner.Status.Breakthrough += refinery.Procent * 10;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Detoxication:
                        {
                            Owner.Status.Detoxication += refinery.Procent;
                            break;
                        }
                    case Database.Rifinery.RefineryType.MDefence:
                        {
                            Owner.Status.PhysicalDamageDecrease += refinery.Procent;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Block:
                        {
                            Owner.Status.Block += refinery.Procent * 100;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Immunity:
                        {
                            Owner.Status.Immunity += refinery.Procent * 100;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Penetration:
                        {
                            Owner.Status.Penetration += refinery.Procent * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Intensification:
                        {
                            Owner.Status.MaxHitpoints += refinery.Procent;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Counteraction:
                        {
                            Owner.Status.Counteraction += refinery.Procent * 10;
                            break;
                        }
                    case Database.Rifinery.RefineryType.FinalMDamage:
                        {

                            Owner.Status.PhysicalDamageIncrease += refinery.Procent;

                            break;
                        }
                    case Database.Rifinery.RefineryType.FinalMAttack:
                        {
                            Owner.Status.MagicDamageIncrease += refinery.Procent;
                            break;
                        }
                }
                switch (refinery.Type2)
                {
                    case Database.Rifinery.RefineryType.CriticalStrike:
                        {
                            Owner.Status.CriticalStrike += refinery.Procent2 * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.SkillCriticalStrike:
                        {
                            Owner.Status.SkillCStrike += refinery.Procent2 * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Break:
                        {
                            Owner.Status.Breakthrough += refinery.Procent2 * 10;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Detoxication:
                        {
                            Owner.Status.Detoxication += refinery.Procent2;
                            break;
                        }
                    case Database.Rifinery.RefineryType.MDefence:
                        {
                            Owner.Status.PhysicalDamageDecrease += refinery.Procent2;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Block:
                        {
                            Owner.Status.Block += refinery.Procent2;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Immunity:
                        {
                            Owner.Status.Immunity += refinery.Procent2 * 100;
                            break;
                        }

                    case Database.Rifinery.RefineryType.Penetration:
                        {
                            Owner.Status.Penetration += refinery.Procent2 * 100;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Intensification:
                        {
                            Owner.Status.MaxHitpoints += refinery.Procent2;
                            break;
                        }
                    case Database.Rifinery.RefineryType.Counteraction:
                        {
                            Owner.Status.Counteraction += refinery.Procent2;
                            break;
                        }
                    case Database.Rifinery.RefineryType.FinalMDamage:
                        {

                            Owner.Status.PhysicalDamageIncrease += refinery.Procent2;

                            break;
                        }
                    case Database.Rifinery.RefineryType.FinalMAttack:
                        {
                            Owner.Status.MagicDamageIncrease += refinery.Procent2;
                            break;
                        }
                }
            }
            else
            {
                Console.WriteLine("error refinery id " + ID);
            }
        }
        public void AddGem(Role.Flags.Gem gem)
        {
            switch (gem)
            {
                case Role.Flags.Gem.SuperThunderGem:
                case Role.Flags.Gem.SuperGloryGem: Owner.AddGem(gem, 500); break;
                case Role.Flags.Gem.RefinedGloryGem:
                case Role.Flags.Gem.RefinedThunderGem: Owner.AddGem(gem, 300); break;
                case Role.Flags.Gem.NormalGloryGem:
                case Role.Flags.Gem.NormalThunderGem: Owner.AddGem(gem, 100); break;
                case Role.Flags.Gem.NormalPhoenixGem:
                case Role.Flags.Gem.NormalDragonGem: Owner.AddGem(gem, 5); break;
                case Role.Flags.Gem.RefinedPhoenixGem:
                case Role.Flags.Gem.RefinedDragonGem: Owner.AddGem(gem, 10); break;
                case Role.Flags.Gem.SuperPhoenixGem:
                case Role.Flags.Gem.SuperDragonGem: Owner.AddGem(gem, 15); break;
                case Role.Flags.Gem.NormalTortoiseGem: Owner.AddGem(gem, 2); break;//1
                case Role.Flags.Gem.RefinedTortoiseGem: Owner.AddGem(gem, 4); break;//2
                case Role.Flags.Gem.SuperTortoiseGem: Owner.AddGem(gem, 6); break;//3


                case Role.Flags.Gem.SuperRainbowGem: Owner.AddGem(gem, 25); break;
                case Flags.Gem.RefinedRainbowGem: Owner.AddGem(gem, 15); break;
                case Flags.Gem.NormalRainbowGem: Owner.AddGem(gem, 10); break;
            }
        }
        public int BattlePower = 0;
        public void CalculateBattlePower()
        {
            BattlePower = 0;
            int val = 0;
            int val_item = 0;
         
            foreach (var item in CurentEquip)
            {
                if (Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Bottle
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.SteedMount)
                    continue;
                if (Database.ItemType.IsHossu(item.ITEM_ID))
                    continue;
                val_item = 0;
                byte Quality = (byte)(item.ITEM_ID % 10);
                switch (Quality)
                {
                    case 9: val_item += 4; break;
                    case 8: val_item += 3; break;
                    case 7: val_item += 2; break;
                    case 6: val_item += 1; break;
                }
                if (item.Plus > 12) item.Plus = 0;//7kha
               val_item += item.Plus;

               if (item.SocketOne != Role.Flags.Gem.NoSocket)
                    val_item += 1;
               if ((byte)(((byte)item.SocketOne % 10) - 3) == 0)
                    val_item += 1;
               if (item.SocketTwo != Role.Flags.Gem.NoSocket)
                    val_item += 1;
               if ((byte)(((byte)item.SocketTwo % 10) - 3) == 0)
                    val_item += 1;

               if (Database.ItemType.IsBacksword(item.ITEM_ID) || Database.ItemType.IsTaoistEpicWeapon(item.ITEM_ID))
               {
                   val_item *= 2;
               }
               else if (Database.ItemType.IsTwoHand(item.ITEM_ID) && FreeEquip(Flags.ConquerItem.LeftWeapon) && FreeEquip(Flags.ConquerItem.AleternanteLeftWeapon))
               {
                   val_item += val_item;
               }
               
                val += val_item;
            }
            BattlePower = val;
        }
        public void OnDequeue()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                try
                {
                  
                    Dictionary<uint, Game.MsgServer.MsgGameItem> statusitens = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
                    foreach (var it in AllItems)
                        if (it.Position < 20)
                            if (!statusitens.ContainsKey(it.Position))
                                statusitens.Add(it.Position, it);
                    if (Alternante)
                    {
                        if (!FreeEquip(Flags.ConquerItem.RightWeapon) && !FreeEquip(Flags.ConquerItem.LeftWeapon))
                        {
                           MsgGameItem _left;
                           TryGetEquip(Flags.ConquerItem.LeftWeapon, out _left);
                           MsgGameItem _right;
                           TryGetEquip(Flags.ConquerItem.LeftWeapon, out _right);
                           if (Database.ItemType.IsKnife(_left.ITEM_ID) && Database.ItemType.IsKnife(_right.ITEM_ID))
                           {
                               MsgGameItem _bow;
                               if (TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon, out _bow) && FreeEquip(Flags.ConquerItem.AleternanteLeftWeapon))
                               {
                                     foreach (var it in AllItems)
                                         if (it.Position > 20)
                                         {
                                             if (statusitens.ContainsKey((ushort)(it.Position - 20)))
                                                 statusitens.Remove((ushort)(it.Position - 20));
                                             statusitens.Add((ushort)(it.Position - 20), it);
                                         }
                                     if (statusitens.ContainsKey((ushort)Flags.ConquerItem.LeftWeapon))
                                     {
                                         statusitens.Remove((ushort)Flags.ConquerItem.LeftWeapon);
                                     }
                                     goto jmp;
                               }
                           }
                        }
                        foreach (var it in AllItems)
                            if (it.Position > 20)
                            {
                                if (it.Position == (byte)Role.Flags.ConquerItem.AleternanteRightWeapon)
                                {
                                    if (Database.ItemType.IsTwoHand(it.ITEM_ID))
                                    {
                                        if (Database.ItemType.IsBow(it.ITEM_ID) == false)
                                        {
                                            if (statusitens.ContainsKey((ushort)(it.Position - 19)))
                                            {
                                                statusitens.Remove((ushort)(it.Position - 19));
                                                Remove((Role.Flags.ConquerItem)((it.Position - 19)), stream);
                                            }
                                        }
                                    }
                                }
                                if (statusitens.ContainsKey((ushort)(it.Position - 20)))
                                    statusitens.Remove((ushort)(it.Position - 20));
                                statusitens.Add((ushort)(it.Position - 20), it);
                            }
                    }
                    jmp:
                    AppendItems(CreateSpawn, statusitens.Values.ToArray(), stream);
                    UpdateStats(statusitens.Values.ToArray(), stream);

                    Owner.Player.HitPoints = Math.Min((int)Owner.Player.HitPoints, (int)Owner.Status.MaxHitpoints);
                    if (Owner.Player.OnTransform && Owner.Player.TransformInfo != null)
                        Owner.Player.TransformInfo.UpdateStatus();
                    else
                        Owner.Player.SendUpdateHP();

                    Owner.ClanShareBP();
                
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }

 
            }
        }
        public void AppendItems(bool CreateSpawn, Game.MsgServer.MsgGameItem[] Items, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgShowEquipment ShowEquip = new MsgShowEquipment();
            ShowEquip.UID = Game.MsgServer.MsgShowEquipment.Show;
            ShowEquip.Alternante = (byte)(Alternante ? 1 : 0);

            if (CreateSpawn)
            {
                foreach (var item in Items)
                {
                    if (item != null)
                    {
                        switch ((Role.Flags.ConquerItem)item.Position)
                        {
                            case Flags.ConquerItem.Ring:
                            case Flags.ConquerItem.AleternanteRing: ShowEquip.Ring = item.UID; break;
                            case Flags.ConquerItem.AleternanteHead:
                            case Flags.ConquerItem.Head: ShowEquip.Head = item.UID; break;
                            case Flags.ConquerItem.AleternanteNecklace:
                            case Flags.ConquerItem.Necklace: ShowEquip.Necklace = item.UID; break;
                            case Flags.ConquerItem.AleternanteRightWeapon:
                            case Flags.ConquerItem.RightWeapon: ShowEquip.RightWeapon = item.UID; break;
                            case Flags.ConquerItem.AleternanteLeftWeapon:
                            case Flags.ConquerItem.LeftWeapon: ShowEquip.LeftWeapon = item.UID; break;
                            case Flags.ConquerItem.AleternanteArmor:
                            case Flags.ConquerItem.Armor:
                                {
                                    ShowEquip.Armor = item.UID;
                                    break;
                                }
                            case Flags.ConquerItem.AleternanteBoots:
                            case Flags.ConquerItem.Boots: ShowEquip.Boots = item.UID; break;
                            case Flags.ConquerItem.AleternanteBottle:
                            case Flags.ConquerItem.Bottle: ShowEquip.Bottle = item.UID; break;
                            case Flags.ConquerItem.SteedMount: ShowEquip.SteedMount = item.UID; break;
                            case Flags.ConquerItem.AleternanteGarment:
                            case Flags.ConquerItem.Garment:
                                {

                                    ShowEquip.Garment = item.UID;
                                    break;
                                }
                            case Flags.ConquerItem.RidingCrop: ShowEquip.RidingCrop = item.UID; break;
                            case Flags.ConquerItem.LeftWeaponAccessory: ShowEquip.LeftWeaponAccessory = item.UID; break;
                            case Flags.ConquerItem.RightWeaponAccessory: ShowEquip.RightWeaponAccessory = item.UID; break;
                            case Flags.ConquerItem.Wing: ShowEquip.Wing = item.UID; break;
                        }
                    }
                }
                if (Owner.Player.SpecialGarment != 0)
                    ShowEquip.Garment = uint.MaxValue - 1;
                Owner.Send(stream.ShowEquipmentCreate(ShowEquip));

              
            }
        }
        public bool UseWindWalkerWeapons()
        {
            return Database.ItemType.IsWindWalkerWeapon(LeftWeapon) && Database.ItemType.IsWindWalkerWeapon(RightWeapon);
        }
        public unsafe void SendAlowAlternante(ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgShowEquipment ShowEquip = new MsgShowEquipment();
            ShowEquip.UID = Game.MsgServer.MsgShowEquipment.AlternanteAllow;
            ShowEquip.Alternante = (byte)(Alternante ? 1 : 0);
            Owner.Send(stream.ShowEquipmentCreate(ShowEquip));
        }
        public unsafe void QueryEquipment(bool Alternantes, bool CallItems = true)
        {
            this.Alternante = Alternantes;
            CreateSpawn = CallItems;
            OnDequeue();

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Owner.UpdatePerfectionLevel(stream);
            }

            if (UseWindWalkerWeapons())
            {
                Owner.Player.AddFlag(MsgUpdate.Flags.WindWalkerFan, Role.StatusFlagsBigVector32.PermanentFlag, false);
            }
            else
                Owner.Player.RemoveFlag(MsgUpdate.Flags.WindWalkerFan);
        }
    }

}
