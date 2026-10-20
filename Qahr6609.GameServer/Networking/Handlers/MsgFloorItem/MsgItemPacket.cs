using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgFloorItem
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetItemPacket(this ServerSockets.Packet stream, out uint uid)
        {
            uint stamp = stream.ReadUInt32();
            uid = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet ItemPacketCreate(this ServerSockets.Packet stream, MsgItemPacket Item)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);
            stream.Write(Item.m_UID);//8
            stream.Write(Item.m_ID);//12
            stream.Write(Item.m_X);//16
            stream.Write(Item.m_Y);//18
            if (Item.MaxLife != 0)
                stream.Write(Item.MaxLife);//20
            else
                stream.Write((ushort)Item.m_Color);//Item.m_Color);
            stream.Write((byte)Item.DropType);//22
            stream.Write(Item.Life);//23
            if (Item.DropType == MsgDropID.Effect)
                stream.Write((byte)Item.m_Color);//aici = 27 plus.
            else
                stream.Write((byte)Item.Plus);
            stream.Write(Item.ItemOwnerUID);//28

            if (Item.DontShow == 1)
            {
                stream.Write(2);//??
                stream.Write(3);//??
            }
            else
                stream.ZeroFill(8);//32
            stream.Write(Item.GuildID);//40
            stream.Write(Item.FlowerType);//44
            stream.Write(Item.Timer);//45
            stream.Write(Item.UnKnow);//53 uint
            stream.Write((uint)0);//57
           

            stream.Write(Item.OwnerX);
            stream.Write(Item.OwnerY);
            //  stream.Write(30933348);

            stream.ZeroFill(28);//??
            if (Item.Name != null)
                stream.Write(Item.Name, 16);
            stream.Write((uint)0);
            stream.Write((uint)0);
            stream.Write((uint)0);
            stream.Finalize(GamePackets.FloorMap);
            return stream;
        }
    }

    public unsafe class MsgItemPacket
    {
        public enum EffectMonsters : uint
        {
            None = 0,
            EarthquakeLeftRight = 1,
            EarthquakeUpDown = 2,
            Night = 4,
            EarthquakeAndNight = 5
        }

        public const uint
            DBShowerEffect = 17,
    TwilightDance = 40,
    NormalDaggerStorm = 50,
    SoulOneDaggerStorm = 41,
    SoulTwoDaggerStorm = 42,//46
    InfernalEcho = 1001390,
    WrathoftheEmperor = 1001380,
    AuroraLotus = 930,
    FlameLotus = 940,

    RageofWar = 1500,
    ShadowofChaser = 1550,

    HorrorofStomper = 1530,
    PeaceofStomper = 1540,

    Thundercloud = 3843;


        public uint m_UID;
        public uint m_ID;
        public ushort m_X;
        public ushort m_Y;
        public ushort MaxLife;
        public MsgDropID DropType;
        public uint Life;
        public byte m_Color;
        public byte m_Color2;
        public uint ItemOwnerUID;
        public byte DontShow;
        public uint GuildID;
        public byte FlowerType;
        public ulong Timer;
        public string Name;
        public uint UnKnow;
        public byte Plus;



        public ushort OwnerX;
        public ushort OwnerY;

        public static MsgItemPacket Create()
        {
            MsgItemPacket item = new MsgItemPacket();
            return item;
        }

        [PacketAttribute(GamePackets.FloorMap)]
        public unsafe static void FloorMap(Client.GameClient client, ServerSockets.Packet packet)
        {
            if (client.InTrade)
                return;
            if (!client.Player.OnMyOwnServer)
                return;

            uint m_UID;

            packet.GetItemPacket(out m_UID);

            MsgFloorItem.MsgItem MapItem;
            if (client.Map.View.TryGetObject<MsgFloorItem.MsgItem>(m_UID, Role.MapObjectType.Item, client.Player.X, client.Player.Y, out MapItem))
            {
                if (MapItem.ToMySelf)
                {
                    if (!MapItem.ExpireMySelf)
                    {
                        if (MapItem.ItemOwner != client.Player.UID)
                        {
                            if (client.Team != null)
                            {
                                if (MapItem.Typ != MsgItem.ItemType.Money &&
                                    (!client.Team.IsTeamMember(MapItem.ItemOwner) || !client.Team.PickupItems))
                                {
                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                                   
                                    return;
                                }
                                else if (MapItem.Typ == MsgItem.ItemType.Money)
                                {
                                    if (!client.Team.PickupMoney)
                                    {
                                        client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                                    
                                        return;
                                    }
                                }
                            }
                            else if (client.Team == null)
                            {
                                if (MapItem.Typ == MsgItem.ItemType.Money)
                                {
                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                             
                                    return;
                                }
                                else
                                {
                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                                    
                                    return;
                                }
                            }
                        }
                    }
                }
                if (Role.Core.GetDistance(client.Player.X,client.Player.Y,MapItem.MsgFloor.m_X,MapItem.MsgFloor.m_Y) <= 5)
                {
                    switch (MapItem.Typ)
                    {

                        case MsgItem.ItemType.Money:
                            {
                               
                                    client.Player.Money += MapItem.Gold;
                                    client.Player.SendUpdate(packet, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    MapItem.SendAll(packet, MsgDropID.Remove);
                                    client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                    client.Map.View.LeaveMap<Role.IMapObj>(MapItem);
                                    client.SendSysMesage("You have picked up a " + MapItem.Gold + " silvers.");
                                  
                           
                                break;
                            }
                        case MsgItem.ItemType.Item:
                            {
                                Database.ItemType.DBItem DBItem;
                                if (client.Inventory.HaveSpace(1))
                                {
                                    if (Database.Server.ItemsBase.TryGetValue(MapItem.MsgFloor.m_ID, out DBItem))
                                    {

                                       
                                        client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                        if (MapItem.ItemBase.StackSize > 1)
                                        {
                                            client.Inventory.Update(MapItem.ItemBase, Role.Instance.AddMode.ADD, packet);
                                        }
                                        else
                                            client.Inventory.Add(MapItem.ItemBase, DBItem, packet);
                                        client.Map.View.LeaveMap<Role.IMapObj>(MapItem);
                                        MapItem.SendAll(packet,MsgDropID.Remove);
                                        client.SendSysMesage("You have picked up a " + DBItem.Name + ".");
                                      

                                        if (DBItem.ID == 711352)
                                        {
                                            client.Player.QuestGUI.IncreaseQuestObjectives(packet, 1311, 1);
                                        }
                                    }
                                }
                                break;
                            }
                        case MsgItem.ItemType.Cps:
                            {
                                client.Player.ConquerPoints += MapItem.ConquerPoints;
               
                                MapItem.SendAll(packet,MsgDropID.Remove);
                                client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                client.Map.View.LeaveMap<Role.IMapObj>(MapItem);
                                client.SendSysMesage("You have picked up a " + MapItem.ConquerPoints + " ConquerPoints.");
                               
                                break;
                            }
                    }
                }
            }
        }
    }
}
