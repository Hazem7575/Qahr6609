using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static void GetItemPacketPacket(this ServerSockets.Packet stream, out MsgGameItem item)
        {
            item = new MsgGameItem();
            item.UID = stream.ReadUInt32();
            item.ITEM_ID = stream.ReadUInt32();
            item.Durability = stream.ReadUInt16();
            item.MaximDurability = stream.ReadUInt16();
            item.Mode = (Role.Flags.ItemMode)stream.ReadUInt16();
            item.Position = stream.ReadUInt16();
            item.SocketProgress = stream.ReadUInt32();
            item.SocketOne = (Role.Flags.Gem)stream.ReadUInt8();
            item.SocketTwo = (Role.Flags.Gem)stream.ReadUInt8();
            stream.SeekForward(sizeof(ushort));
            item.Effect = (Role.Flags.ItemEffect)stream.ReadUInt32();
            stream.SeekForward(sizeof(byte));
            item.Plus = stream.ReadUInt8();
            item.Bless = stream.ReadUInt8();
            item.Bound = stream.ReadUInt8();
            item.Enchant = stream.ReadUInt8();
            stream.SeekForward(9);
            item.Locked = stream.ReadUInt8();
            stream.SeekForward(sizeof(byte));
            item.Color = (Role.Flags.Color)stream.ReadUInt32();
            item.PlusProgress = stream.ReadUInt32();
            item.Inscribed = stream.ReadUInt32();
            stream.ReadUInt32();
            stream.ReadUInt32();
            item.StackSize = stream.ReadUInt16();
            item.PerfectionRank = stream.ReadUInt16();
            item.PerfectionLevel = stream.ReadUInt32();
            item.PerfectionProgress = stream.ReadUInt32();
            item.OwnerUID = stream.ReadUInt32();
            item.OwnerName = stream.ReadCString(16);
            item.Signature = stream.ReadCString(32);
        }
    }
    public class MsgGameItem
    {
        public MsgItemExtra.Purification Purification;
        public MsgItemExtra.Refinery Refinary;

        public ConcurrentDictionary<uint, MsgGameItem> Deposite = new ConcurrentDictionary<uint, MsgGameItem>();


        public void Send(MsgInterServer.PipeClient user, ServerSockets.Packet stream)
        {
            user.Send(ItemCreate(stream, this));
            if (Purification.ItemUID != 0 || Refinary.ItemUID != 0)
            {
                MsgItemExtra extra = new MsgItemExtra();
                if (Purification.InLife)
                {
                    if (Purification.SecoundsLeft == 0)
                        Purification.Typ = MsgItemExtra.Typing.Stabilization;
                    else
                        Purification.Typ = MsgItemExtra.Typing.PurificationAdding;
                    extra.Purifications.Add(Purification);
                }
                if (Refinary.InLife)
                {
                    Refinary.Typ = MsgItemExtra.Typing.RefinaryAdding;
                    if (Refinary.EffectDuration == 0)
                        Refinary.Typ = MsgItemExtra.Typing.PermanentRefinery;
                    extra.Refinerys.Add(Refinary);
                }
                user.Send(extra.CreateArray(stream, true));
            }
        }
        public unsafe Role.Instance.Inventory Send(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (Mode == Role.Flags.ItemMode.Update)
            {

                string logs = "[Item]" + client.Player.Name + " update [" + UID + "]" + ITEM_ID + " plus [" + Plus + "] s1[" + SocketOne + "]s2[" + SocketTwo + "]";
                Logs.OnDequeue(logs);
            }
            Database.ItemType.DBItem DBItem;
            if (MaximDurability == 0)
            {
                if (Database.Server.ItemsBase.TryGetValue(ITEM_ID, out DBItem))
                    MaximDurability = DBItem.Durability;
            }
            ushort position = Database.ItemType.ItemPosition(ITEM_ID);
            /*if (position == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory || position == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory)
            {
                if (Activate == 0)
                {
                    Activate = 1;
                    EndDate = DateTime.Now.AddMinutes(30);
                }
            }*/
            if (position == (ushort)Role.Flags.ConquerItem.Garment || position == (ushort)Role.Flags.ConquerItem.SteedMount)
            {
                Database.CoatStorage.StorageItem DbGarment;
                if (Database.CoatStorage.Garments.TryGetValue(ITEM_ID, out DbGarment) || Database.CoatStorage.Mounts.TryGetValue(ITEM_ID, out DbGarment))
                {
                    if (DbGarment.Stars > 2)
                    {
                        if (Bound == 1 && Activate == 0)
                        {
                            Activate = 1;
                            EndDate = DateTime.Now.AddDays(3);
                        }
                    }
                }
            }
            if (Database.Server.ItemsBase.TryGetValue(ITEM_ID, out DBItem))
            {
                if (DBItem.Time != 0 && Activate == 0)
                {
                    Activate = 1;
                    EndDate = DateTime.Now.AddMinutes(DBItem.Time);

                }
               
            }
         
            if (ITEM_ID == Database.ItemType.MemoryAgate)
            {
                if (Activate == 0)
                {
                    Activate = 1;
                    EndDate = DateTime.Now.AddHours(12);
                }
            }
            if (Plus > 0)
            {
                if (position == 0)
                    Plus = 0;
            }
            if (ITEM_ID >= 730001 && ITEM_ID <= 730008)
                Plus = (byte)(ITEM_ID % 10);


            client.Send(ItemCreate(stream, this));
            SendItemExtra(client, stream);
            SendItemLocked(client, stream);

            return client.Inventory;
        }
        public void SendItemExtra(Client.GameClient client, ServerSockets.Packet stream)
        {
            //client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.SetEquipPosition, UID,Position,0,0,0,0));
            if (Purification.ItemUID != 0 || Refinary.ItemUID != 0)
            {
                MsgItemExtra extra = new MsgItemExtra();
                if (Purification.InLife)
                {
                    if (Purification.SecoundsLeft == 0)
                        Purification.Typ = MsgItemExtra.Typing.Stabilization;
                    else
                        Purification.Typ = MsgItemExtra.Typing.PurificationAdding;
                    extra.Purifications.Add(Purification);
                }
                if (Refinary.InLife)
                {
                    Refinary.Typ = MsgItemExtra.Typing.RefinaryAdding;
                    if (Refinary.EffectDuration == 0)
                        Refinary.Typ = MsgItemExtra.Typing.PermanentRefinery;
                    extra.Refinerys.Add(Refinary);
                }
                client.Send(extra.CreateArray(stream));
            }

        }
        public void SendItemLocked(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (Locked == 2)
            {
                if (client.Player.OnMyOwnServer)
                {
                    if (UnLockTimer == 0)
                    {
                        Locked = 0;
                        Mode = Role.Flags.ItemMode.Update;
                        client.Send(ItemCreate(stream, this));
                    }
                    else
                    {
                        if (DateTime.Now > Role.Core.GetTimer(UnLockTimer))
                        {
                            Locked = 0;
                            Mode = Role.Flags.ItemMode.Update;
                            client.Send(ItemCreate(stream, this));
                        }
                        else
                        {
                            client.Send(stream.ItemLockCreate(UID, MsgItemLock.TypeLock.UnlockDate, 0, (uint)UnLockTimer));
                        }
                    }
                }
            }
        }
        public ServerSockets.Packet ItemCreate(ServerSockets.Packet stream, MsgGameItem item)
        {
            stream.InitWriter();

            stream.Write(item.UID);
            stream.Write(item.ITEM_ID);
            stream.Write(item.Durability);
            stream.Write(item.MaximDurability);
            stream.Write((ushort)item.Mode);
            stream.Write(item.Position);//18
            stream.Write(item.SocketProgress);//20
            stream.Write((byte)item.SocketOne);//24
            stream.Write((byte)item.SocketTwo);//25

            stream.Write((ushort)0);//26
            stream.Write((uint)item.Effect);//28

            stream.Write((byte)0);//32
            stream.Write(item.Plus);//33
            stream.Write(item.Bless);//34
            stream.Write(item.Bound);//35
            stream.Write(item.Enchant);//36
            stream.ZeroFill(3);//37
            stream.Write(item.ProgresGreen);//40
            stream.Write(item.Suspicious);//44
            stream.Write((byte)0);//45

            stream.Write(item.Locked);//46
            stream.Write((byte)0);//47
            stream.Write((uint)item.Color);//48
            stream.Write(item.PlusProgress);//49
            stream.Write(item.Inscribed);//53

            stream.Write(RemainingTime);//uint.MaxValue);//active 
            stream.Write(0);

            stream.Write(item.StackSize);
            stream.Write((ushort)PerfectionRank);

            stream.Write(item.PerfectionLevel);//level max = 59
            stream.Write(item.PerfectionProgress);//progres
            stream.Write(item.OwnerUID);//uid
            stream.Write(item.OwnerName, 16);
            stream.Write(item.Signature, 32);

            stream.Finalize(Game.GamePackets.Item);

            return stream;
        }

        public bool IsWeapon
        {
            get
            {
                return (Database.ItemType.ItemPosition(ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeapon
                    || Database.ItemType.ItemPosition(ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeapon) && !Database.ItemType.IsArrow(ITEM_ID);
            }
        }
        public bool IsEquip
        {
            get
            {
                return Database.ItemType.ItemPosition(ITEM_ID) != 0;
            }
        }
        public uint ItemPoints;
        public int GetPerfectionPosition
        {
            get
            {
                ushort DBPosotion = Database.ItemType.ItemPosition(ITEM_ID);
                if (DBPosotion < 5)
                    return DBPosotion;
                if (Database.ItemType.IsShield(ITEM_ID) || Database.ItemType.IsHossu(ITEM_ID))
                    return 4;
                switch (DBPosotion)
                {
                    case (ushort)Role.Flags.ConquerItem.Wing:
                        return 5;
                    case (ushort)Role.Flags.ConquerItem.Ring:
                        return 6;
                    case (ushort)Role.Flags.ConquerItem.RidingCrop:
                        return 7;
                    case (ushort)Role.Flags.ConquerItem.Boots:
                        return 8;
                    case (ushort)Role.Flags.ConquerItem.Steed:
                        return 9;
                    case (ushort)Role.Flags.ConquerItem.Fan:
                        return 10;
                    case (ushort)Role.Flags.ConquerItem.Tower:
                        return 11;
                }
                return -1;

            }
        }
        public uint PerfectionStage
        {
            get
            {
                return PerfectionLevel / 9;
            }
        }
        public uint PerfectionStageStars
        {
            get
            {
                return PerfectionLevel % 9;
            }
        }
        public uint GetPrestigeScore
        {
            get
            {
                return ItemPoints;
            }
        }

        public ushort Leng;
        public ushort PacketID;
        public uint UID;
        public uint ITEM_ID;
        public ushort Durability;
        public ushort MaximDurability;
        public Role.Flags.ItemMode Mode;
        public ushort Position;
        public uint SocketProgress;
        public uint RemainingTime2;
        public uint RemainingTime
        {
            get
            {
                if (Activate == 1)
                {
                    TimeSpan Span = EndDate - DateTime.Now;
                    return (uint)Span.TotalSeconds;
                }
                return 0;
            }
        }
        public Role.Flags.Gem SocketOne;
        public Role.Flags.Gem SocketTwo;
        public ushort padding;
        public Role.Flags.ItemEffect Effect;
        public byte Plus;
        public byte Bless;
        public byte Bound;
        public byte Enchant;//36 // Steed  -> ProgresBlue 
        public uint ProgresGreen;//39 // for steed
        public byte Suspicious;
        public byte Locked;
        public Role.Flags.Color Color;
        public uint PlusProgress;//52
        public uint Inscribed;
        public uint Activate;
        public uint TimeLeftInMinutes;//60
        public ushort StackSize;//68
        public ushort UnKnow;
        public uint WH_ID;

        public uint OwnerUID = 0;
        public uint PerfectionLevel = 0;
        public ushort PerfectionRank = 0;
        public uint PerfectionProgress = 0;
        public string OwnerName = "";
        public string Signature = "";
        public DateTime EndDate = DateTime.FromBinary(0);
        public int UnLockTimer;
    }
}
