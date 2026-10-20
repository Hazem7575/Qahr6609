using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetItemExtra(this ServerSockets.Packet stream, out  uint Mode, out uint ItemUID, out   uint ItemArtfact)
        {
            Mode = stream.ReadUInt32();
            ItemUID = stream.ReadUInt32();
            ItemArtfact = stream.ReadUInt32();
        }

        public static void GetExtraItem(this ServerSockets.Packet stream, out MsgItemExtra.Purification purification, out MsgItemExtra.Refinery refinary)
        {
            purification = new MsgItemExtra.Purification();
            refinary = new MsgItemExtra.Refinery();

            uint count = stream.ReadUInt32();
            for (int x = 0; x < count; x++)
            {
                uint UID = stream.ReadUInt32();
                MsgItemExtra.Typing type = (MsgItemExtra.Typing)stream.ReadUInt32();
                if (type <= MsgItemExtra.Typing.StabilizationEffectRefined)
                {
     
                    refinary.ItemUID = UID;
                    refinary.Typ = type;
                    refinary.EffectID = stream.ReadUInt32();
                    refinary.EffectLevel = stream.ReadUInt32();
                    refinary.EffectPercent = stream.ReadUInt32();
                    refinary.EffectPercent2 = stream.ReadUInt32();
                    stream.ReadUInt32();//secounds left
                    refinary.EffectDuration = 0;
                    refinary.AddedOn = DateTime.Now;
                    refinary.Available = true;
             
                }
                else
                {
                    purification.ItemUID = UID;
                    purification.Typ = type;
                    purification.PurificationItemID = stream.ReadUInt32();
                    purification.PurificationLevel = stream.ReadUInt32();
                    purification.EffectPercent2 = stream.ReadUInt32();
                    purification.PurificationDuration = stream.ReadUInt32();
                    stream.ReadUInt32();
                    stream.ReadUInt32();
                    purification.AddedOn = DateTime.Now;
                }
            }
        }
    }

    public class MsgItemExtra
    {
        public class Artefacts
        {
            public const byte Refinary = 0,
                Purification = 1;
        }
        public enum Typing : uint
        {
            RefinaryAdding = 2,
            PermanentRefinery = 3,
            StabilizationEffectRefined = 4,
            PurificationEffect = 5,
            PurificationAdding = 6,
            ExpireTime = 7,
            Stabilization = 8,
            StabilizationEffectPurification = 9
        }

        // Explicit offsets = the .NET Framework x86 layout (it auto-arranges structs holding a DateTime). This struct is
        // stored raw inside MsgDetainedItem (ServerDataBase\ClaimContainer.bin), so the .NET 10 x64 build must use the same bytes.
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 36)]
        public struct Purification
        {
            public static Purification ShallowCopy(Purification item)
            {
                return (Purification)item.MemberwiseClone();
            }
            [System.Runtime.InteropServices.FieldOffset(0)] public uint ItemUID;
            [System.Runtime.InteropServices.FieldOffset(4)] public Typing Typ;
            [System.Runtime.InteropServices.FieldOffset(8)] public uint PurificationItemID;
            [System.Runtime.InteropServices.FieldOffset(12)] public uint PurificationLevel;
            [System.Runtime.InteropServices.FieldOffset(16)] public uint EffectPercent2;
            /// <summary>
            /// In minutes.
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(20)] public uint PurificationDuration;

            [System.Runtime.InteropServices.FieldOffset(28)] public DateTime AddedOn;
            [System.Runtime.InteropServices.FieldOffset(24)] private bool Available;
            public int SecoundsLeft
            {
                get
                {
                    if (PurificationDuration != 0)
                    {
                        TimeSpan span1 = new TimeSpan(AddedOn.AddSeconds(PurificationDuration).Ticks);
                        TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                        int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                        if (secondsleft > 0)
                        {
                            Available = true;
                            return secondsleft;
                        }
                        else
                            Available = false;
                    }
                    else if (PurificationItemID >  1000)
                        Available = true;

                    return 0;
                }
            }
            public bool InLife
            {
                get
                {

                    int secount = SecoundsLeft;//to create time
                    return Available;
                }
            }
        }
      
        // Explicit offsets = the .NET Framework x86 layout (it auto-arranges structs holding a DateTime). This struct is
        // stored raw inside MsgDetainedItem (ServerDataBase\ClaimContainer.bin), so the .NET 10 x64 build must use the same bytes.
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 40)]
        public struct Refinery
        {
            public static Refinery ShallowCopy(Refinery item)
            {
                return (Refinery)item.MemberwiseClone();
            }
            [System.Runtime.InteropServices.FieldOffset(0)] public uint ItemUID;
            
            [System.Runtime.InteropServices.FieldOffset(4)] public Typing Typ;
            [System.Runtime.InteropServices.FieldOffset(8)] public uint EffectID;
            [System.Runtime.InteropServices.FieldOffset(12)] public uint EffectLevel;
            [System.Runtime.InteropServices.FieldOffset(16)] public uint EffectPercent;
            [System.Runtime.InteropServices.FieldOffset(20)] public uint EffectPercent2;
            /// <summary>
            /// In minutes.
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(24)] public uint EffectDuration;
            [System.Runtime.InteropServices.FieldOffset(32)] public DateTime AddedOn;
            [System.Runtime.InteropServices.FieldOffset(28)] public bool Available;

            public int SecoundsLeft
            {
                get
                {
                    if (EffectDuration != 0)
                    {
                        TimeSpan span1 = new TimeSpan(AddedOn.AddSeconds(EffectDuration).Ticks);
                        TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                        int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                        if (secondsleft > 0)
                        {
                            Available = true;
                            return secondsleft;
                        }
                        else
                            Available = false;
                    }
                    else if (EffectID > 1000)
                        Available = true;

                    return 0;
                }
            }
            public bool InLife
            {
                get
                {

                    int secount = SecoundsLeft;//verified create time
                    return Available;
                }
            }
        }
        public List<Purification> Purifications;
        public List<Refinery> Refinerys;

        public MsgItemExtra()
        {
            Purifications = new List<Purification>();
            Refinerys = new List<Refinery>();

        }

        public unsafe ServerSockets.Packet CreateArray(ServerSockets.Packet stream,bool tointer =false)
        {

            stream.InitWriter();

            stream.Write((uint)(Refinerys.Count + Purifications.Count));

            foreach (var item in Purifications)
            {
                stream.Write(item.ItemUID);//0
                stream.Write((uint)item.Typ);//4
                stream.Write(item.PurificationItemID);//8
                stream.Write(item.PurificationLevel);//12
                stream.Write(item.EffectPercent2);//16
                stream.Write(item.PurificationDuration);//20
                stream.Write(item.SecoundsLeft);//24

                stream.Write((uint)0);//unknow
            }
            foreach (var item in Refinerys)
            {
                stream.Write(item.ItemUID);//0
                stream.Write((uint)item.Typ);//4
                if (tointer)
                    stream.Write(item.EffectID);
                else
                {
                    if (item.EffectID >= 724440 && item.EffectID <= 724444 || item.EffectID == 3004140 || item.EffectID >= 3006165 && item.EffectID <= 3006170)
                        stream.Write((uint)301);
                    else
                        stream.Write(item.EffectID);//8
                }
                stream.Write(item.EffectLevel);//12
                stream.Write(item.EffectPercent);//16
                stream.Write(item.EffectPercent2);//20
                stream.Write(item.SecoundsLeft);//24
            }

            stream.Finalize(GamePackets.ExtraItem);

            return stream;
        }

        /// <summary>Removes `take` stones from the inventory, all or nothing: every stone is validated first, and if a
        /// removal still fails the stones already taken are put back, so stones are never spent without the effect.</summary>
        static bool RemoveStones(Client.GameClient client, Queue<Game.MsgServer.MsgGameItem> items, int take, ServerSockets.Packet packet)
        {
            if (take <= 0 || items.Count < take)
                return false;
            var stones = new List<Game.MsgServer.MsgGameItem>(take);
            for (int x = 0; x < take; x++)
            {
                var stone = items.Dequeue();
                Game.MsgServer.MsgGameItem held;
                if (stone == null || !client.Inventory.TryGetItem(stone.UID, out held) || held != stone)
                    return false;
                stones.Add(stone);
            }
            var removed = new List<Game.MsgServer.MsgGameItem>(take);
            foreach (var stone in stones)
            {
                if (!client.Inventory.RemoveOne(stone.UID, packet))
                {
                    RestoreStones(client, removed, packet);
                    return false;
                }
                removed.Add(stone);
            }
            return true;
        }
        /// <summary>Undoes RemoveOne for each stone: one unit back on a stack still held, otherwise the item itself.</summary>
        static void RestoreStones(Client.GameClient client, List<Game.MsgServer.MsgGameItem> removed, ServerSockets.Packet packet)
        {
            foreach (var stone in removed)
            {
                if (client.Inventory.ClientItems.ContainsKey(stone.UID))
                {
                    stone.StackSize += 1;
                    stone.Mode = Role.Flags.ItemMode.Update;
                    stone.Send(client, packet);
                }
                else
                {
                    if (stone.StackSize == 0)
                        stone.StackSize = 1;
                    client.Inventory.Update(stone, Role.Instance.AddMode.ADD, packet);
                }
            }
        }

        [PacketAttribute(GamePackets.Stabilization)]
        public unsafe static void Stabilization(Client.GameClient client, ServerSockets.Packet packet)
        {
            uint Mode;
            uint ItemUID;
            uint Count;

            packet.GetItemExtra(out Mode, out ItemUID, out Count);

            if (client.InTrade || client.IsVendor)
                return;
            // Client supplied stone count is clamped to the inventory size.
            int stoneCount = Conquer6609.Security.Guard.ClampCount(Count, 40);

            switch (Mode)
            {
                case Artefacts.Refinary:
                    {
                        Game.MsgServer.MsgGameItem Item;
                        if (client.TryGetItem(ItemUID, out Item))
                        {
                            if (!Item.Refinary.InLife) return;
                            if (Item.Refinary.EffectID == 0) return;
                            ushort Position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (Position <= 0)
                            {
                                return;
                            }
                            uint procent = 0;
                            Queue<Game.MsgServer.MsgGameItem> items = new Queue<Game.MsgServer.MsgGameItem>();
                            List<uint> numberCounts = new List<uint>();
                            for (int count = 0; count < stoneCount; count++)
                            {
                                if (packet.Position + 4 > packet.Size)
                                    break;
                                uint stoneUID = packet.ReadUInt32();

                                Game.MsgServer.MsgGameItem stones;
                                if (client.Inventory.TryGetItem(stoneUID, out stones) && (stones.ITEM_ID == 723694 || stones.ITEM_ID == 723695))
                                {
                                    if (numberCounts.Contains(stones.UID))
                                    {

                                        client.Socket.Disconnect();
                                        return;
                                    }
                                    else
                                        numberCounts.Add(stones.UID);

                                    if (stones.ITEM_ID == 723694)
                                        procent += 10;
                                    if (stones.ITEM_ID == 723695)
                                        procent += 100;
                                    items.Enqueue(stones);
                                }
                            }

                            if (procent >= Database.ItemType.RefineryStabilizationPoints((byte)Item.Refinary.EffectLevel))
                            {
                                // Stones are consumed first; the effect is applied only if all were removed.
                                if (!RemoveStones(client, items, items.Count, packet))
                                    return;
                                Item.Refinary.EffectDuration = 0;
                                Item.Refinary.Typ = Typing.StabilizationEffectRefined;

                                MsgItemExtra effect = new MsgItemExtra();
                                effect.Refinerys.Add(Item.Refinary);
                                client.Send(effect.CreateArray(packet));

                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, packet);


                            }
                            // (removed: a single 10-point stone used to stabilize any level for free)
                        }
                        break;
                    }
                case Artefacts.Purification:
                    {

                        Game.MsgServer.MsgGameItem Item;
                        if (client.TryGetItem(ItemUID, out Item))
                        {
                            if (!Item.Purification.InLife) return;
                            if (Item.Purification.PurificationItemID == 0) return;

                            uint procent = 0;
                            Queue<Game.MsgServer.MsgGameItem> items = new Queue<Game.MsgServer.MsgGameItem>();
                            List<uint> numberCounts = new List<uint>();
                            for (int count = 0; count < stoneCount; count++)
                            {
                                if (packet.Position + 4 > packet.Size)
                                    break;
                                uint stoneUID = packet.ReadUInt32();

                                Game.MsgServer.MsgGameItem stones;
                                if (client.Inventory.TryGetItem(stoneUID, out stones) && (stones.ITEM_ID == 723694 || stones.ITEM_ID == 723695))
                                {
                                    if (numberCounts.Contains(stones.UID))
                                    {

                                        client.Socket.Disconnect();
                                        return;
                                    }
                                    else
                                        numberCounts.Add(stones.UID);
                                    if (stones.ITEM_ID == 723694)
                                        procent += 10;
                                    if (stones.ITEM_ID == 723695)
                                        procent += 100;
                                    items.Enqueue(stones);
                                }
                            }

                            if (procent >= Database.ItemType.PurifyStabilizationPoints((byte)Item.Purification.PurificationLevel))
                            {
                                // Stones are consumed first; the effect is applied only if all were removed.
                                if (!RemoveStones(client, items, items.Count, packet))
                                    return;
                                Item.Purification.PurificationDuration = 0;
                                Item.Purification.Typ = Typing.StabilizationEffectPurification;

                                MsgItemExtra effect = new MsgItemExtra();
                                effect.Purifications.Add(Item.Purification);
                                client.Send(effect.CreateArray(packet));

                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, packet);


                            }
                            // (removed: a single 10-point stone used to stabilize any level for free)


                        }
                        break;
                    }
            }
        }

        [PacketAttribute(GamePackets.AddExtra)]
        public unsafe static void AddExtra(Client.GameClient client, ServerSockets.Packet packet)
        {
            uint Mode = packet.ReadUInt32();
            uint ItemUID = packet.ReadUInt32();
            uint ItemArtfact = packet.ReadUInt32();
            if (ItemUID == ItemArtfact)
                return;
            if (client.InTrade || client.IsVendor)
                return;
            switch (Mode)
            {
                case Artefacts.Refinary:
                    {
                        Game.MsgServer.MsgGameItem Item, Artefact;
                        if (client.TryGetItem(ItemUID, out Item) && client.Inventory.TryGetItem(ItemArtfact, out Artefact))
                        {
                            if (Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Bottle
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Fan
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Tower
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RidingCrop
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.SteedMount
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Inventory
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                               || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                                return;
                            ushort Position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (Position <= 0)
                            {
                                return;
                            }

                            Database.Rifinery.Item BaseAddingItem;
                            if (Database.Server.RifineryItems.TryGetValue(Artefact.ITEM_ID, out BaseAddingItem))
                            {

                                if (Database.ItemType.ItemPosition(Item.ITEM_ID) != BaseAddingItem.ForItemPosition)
                                    return;
                                if (BaseAddingItem.Name == "2-Handed" || BaseAddingItem.Name == "2-handed")
                                {
                                    if (!Database.ItemType.IsTwoHand(Item.ITEM_ID))
                                        return;
                                }
                                // The refinery item is consumed before the effect is applied.
                                uint artefactId = Artefact.ITEM_ID;
                                if (!client.Inventory.RemoveOne(Artefact.UID, packet))
                                    return;

                                Item.Refinary = new Refinery();
                                Item.Refinary.AddedOn = DateTime.Now;
                                Item.Refinary.Available = true;
                                Item.Refinary.ItemUID = ItemUID;
                                Item.Refinary.EffectLevel = BaseAddingItem.Level;
                                int days = 7;
                                switch (client.Player.VipLevel)
                                {
                                    case 1: days = 8; break;
                                    case 2: days = 9; break;
                                    case 3: days = 11; break;
                                    default:
                                        days = 7;
                                        break;
                                }

                                Item.Refinary.EffectDuration = (uint)(days * 24 * 60 * 60);
                                Item.Refinary.EffectID = artefactId;
                                Item.Refinary.EffectPercent = BaseAddingItem.Procent;
                                Item.Refinary.EffectPercent2 = BaseAddingItem.Procent2;
                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, packet);

                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                        }
                        break;
                    }
                case Artefacts.Purification:
                    {
                        Game.MsgServer.MsgGameItem Item, Artefact;
                        if (client.TryGetItem(ItemUID, out Item) && client.Inventory.TryGetItem(ItemArtfact, out Artefact))
                        {
                            if (Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Bottle
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Fan
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Tower
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RidingCrop
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Inventory
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.SteedMount
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                                || Database.ItemType.ItemPosition(Item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory)
                                return;
                            ushort Position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                            if (Position <= 0)
                            {
                                return;
                            }
                            Database.ItemType.DBItem PurifyInformation, ItemInformation;
                            if (!Database.Server.ItemsBase.TryGetValue(Artefact.ITEM_ID, out PurifyInformation)
                                || !Database.Server.ItemsBase.TryGetValue(Item.ITEM_ID, out ItemInformation))
                                return;

                            if (PurifyInformation.PurificationLevel > 0)
                            {
                                {
                                    if (Database.ItemType.CompareSoul(ItemInformation.ID, PurifyInformation.ID))
                                    {
                                        if (client.Inventory.CheckMeteors((byte)PurifyInformation.PurificationMeteorNeed, false, packet))
                                        {
                                            // The soul item is consumed before the meteors and the effect.
                                            uint artefactId = Artefact.ITEM_ID;
                                            if (!client.Inventory.RemoveOne(Artefact.UID, packet))
                                                return;
                                            client.Inventory.CheckMeteors((byte)PurifyInformation.PurificationMeteorNeed, true, packet);

                                            Item.Purification = new Purification();
                                            Item.Purification.AddedOn = DateTime.Now;
                                            Item.Purification.ItemUID = ItemUID;
                                            Item.Purification.PurificationLevel = PurifyInformation.PurificationLevel;
                                            Item.Purification.PurificationDuration = 7 * 24 * 60 * 60;
                                            Item.Purification.PurificationItemID = artefactId;
                                            Item.Purification.Typ = Typing.PurificationEffect;

                                            MsgItemExtra effect = new MsgItemExtra();
                                            effect.Purifications.Add(Item.Purification);
                                            client.Send(effect.CreateArray(packet));

                                            Item.Mode = Role.Flags.ItemMode.Update;
                                            Item.Send(client, packet);

                                            if (Item.Position != 0)
                                                client.Equipment.QueryEquipment(client.Equipment.Alternante);

                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

            }
        }
    }
}
