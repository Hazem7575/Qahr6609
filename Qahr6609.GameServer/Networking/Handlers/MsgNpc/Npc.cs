using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgNpc
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet NpcCreate(this ServerSockets.Packet stream, Npc npc, ushort newMesh =0)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);
            stream.Write(npc.UID);
            stream.Write(npc.UnKnow);
            stream.Write(npc.X);
            stream.Write(npc.Y);
            if (newMesh != 0)
                stream.Write(newMesh);
            else
            stream.Write(npc.Mesh);
            stream.Write((ushort)npc.NpcType);
            stream.Write(npc.Sort);


            stream.Finalize(GamePackets.NpcSpawn);
            return stream;
        }
        public static unsafe void GetNpc(this ServerSockets.Packet stream, out Npc npc)
        {
            uint stamp = stream.ReadUInt32();

            npc = new Npc();
            npc.UID = stream.ReadUInt32();
            npc.UnKnow = stream.ReadUInt32();
            npc.X = stream.ReadUInt16();
            npc.Y = stream.ReadUInt16();
            npc.Mesh = stream.ReadUInt16();
            npc.NpcType = (Role.Flags.NpcType)stream.ReadUInt16();
            npc.Sort = stream.ReadUInt16();
        }
    }
    public unsafe class Npc : Role.IMapObj
    {
        public const byte SeedDistance = 19;//16 old , 18

        public Extensions.Time32 Respawn = new Extensions.Time32();
        public uint IndexInScreen { get; set; }
        public bool IsTrap() { return false; }

        public ushort Leng;
        public ushort PacketID;
        public int Stamp;
        public uint UID { get; set; }
        public uint UnKnow;
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Mesh;
        public Role.Flags.NpcType NpcType;
        public Role.MapObjectType ObjType { get; set; }
        public ushort Sort;
        public bool Alive { get; set; }
        
        public uint DynamicID { get; set; }
        public uint Map { get; set; }
        public bool AllowDynamic { get; set; }
        Role.GameMap _gmap;
        public Role.GameMap GMap
        {
            get {
                if (_gmap == null)
                    _gmap = Database.Server.ServerMaps[Map];
                return _gmap;
            }
        }

        public static Npc Create()
        {
            
            Npc _npc = new Npc();
            _npc.Leng = 26;
            _npc.AllowDynamic = false;
            _npc.PacketID = GamePackets.NpcSpawn;
            _npc.Stamp = Extensions.Time32.Now.GetHashCode();
            _npc.ObjType = Role.MapObjectType.Npc;
            _npc.Alive = true;
            return _npc;
        }
        public bool CanSee(Role.IMapObj obj)
        {
            try
            {
                if (obj == null)
                    return false;
                if (obj.Map != Map)
                    return false;
                if (!AllowDynamic)
                {
                    if (obj.DynamicID != DynamicID)
                        return false;
                }
                if (obj.UID == UID)
                    return false;
                return Role.Core.GetDistance(obj.X, obj.Y, X, Y) <= SeedDistance;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                return false;
            }
        }
        public void Send(ServerSockets.Packet stream)
        {
            foreach (var user in GMap.View.Roles(Role.MapObjectType.Player, X, Y))
            {
                if (CanSee(user))
                {
                    UnKnow = UID;
                    (user as Role.Player).Send(stream.NpcCreate(this));
                }
            }
        }
        public ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool View)
        {
            UnKnow = UID;
            return stream = stream.NpcCreate(this);
        }
        [PacketAttribute(GamePackets.NpcSpawn)]
        public unsafe static void Procesor(Client.GameClient client, ServerSockets.Packet packet)
        {
            Npc stream;

            packet.GetNpc(out stream);


            if (client.Player.Map == 1038 && client.UseItem == 720020)//GuildWar Statue
            {
                if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces != MsgTournaments.ProcesType.Dead)
                {
                    if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                    {
                        if (client.Inventory.Contain(client.UseItem, 1))
                        {

                            switch (client.Player.GuildRank)
                            {
                                case Role.Flags.GuildMemberRank.GuildLeader:
                                    {
                                        ushort x = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[0][0];
                                        ushort y = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[0][1];
                                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, x, y) <= 3)
                                        {
                                            if (!Role.Statue.ContainStatue(client.Map, x, y))
                                            {
                                                client.Inventory.Remove(client.UseItem, 1, packet);
                                                Role.Statue.CreateStatue(client, x, y, stream.Sort, (int)stream.NpcType);
                                            }
                                        }
                                        break;
                                    }
                                case Role.Flags.GuildMemberRank.DeputyLeader:
                                case Role.Flags.GuildMemberRank.HDeputyLeader:
                                    {
                                        for (int i = 1; i < 3; i++)
                                        {
                                            ushort x = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[i][0];
                                            ushort y = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[i][1];
                                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, x, y) <= 3)
                                            {
                                                if (!Role.Statue.ContainStatue(client.Map, x, y))
                                                {
                                                    client.Inventory.Remove(client.UseItem, 1, packet);
                                                    Role.Statue.CreateStatue(client, x, y, stream.Sort, (int)stream.NpcType);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                // client StrRes 100129: "Only the Guild Leader, Deputy Guild Leader and Branch Manager can place the statue."
                                // (plain members could place one before)
                                case Role.Flags.GuildMemberRank.Manager:
                                case Role.Flags.GuildMemberRank.HonoraryManager:
                                    {
                                        for (int i = 3; i < MsgTournaments.MsgSchedules.GuildWar.StatueCoords.Length; i++)
                                        {
                                            ushort x = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[i][0];
                                            ushort y = MsgTournaments.MsgSchedules.GuildWar.StatueCoords[i][1];
                                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, x, y) <= 3)
                                            {
                                                if (!Role.Statue.ContainStatue(client.Map, x, y))
                                                {
                                                    client.Inventory.Remove(client.UseItem, 1, packet);
                                                    Role.Statue.CreateStatue(client, x, y, stream.Sort, (int)stream.NpcType);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    client.SendSysMesage("Only the Guild Leader, Deputy Guild Leader and Branch Manager can place the statue.");
                                    break;
                            }
                        }
                    }
                    else
                        client.SendSysMesage("Your guild not dominated the pole");
                }
                return;
            }

            if (stream.Mesh - (stream.Mesh % 10) == client.MoveNpcMesh - (client.MoveNpcMesh % 10))
            {
                if (client.UseItem >= 720021 && client.UseItem <= 720024)
                {
                    if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID && client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                    {
                        ushort X = stream.X;
                        ushort Y = stream.Y;
                        if (client.Map.IsValidFlagNpc(X, Y) && !MsgTournaments.MsgGuildWar.GuildConductor.BlockMaps.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                        {
                            uint ToMap = client.Player.Map;
                            ushort ToX = X;
                            ushort ToY = Y;
                            if (client.Map.AddGuildTeleporterItem(ref ToX, ref ToY))
                            {
                                var conductor = MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID];
                                //remove oldconductor--------------------
                                Role.GameMap map;
                                if (Database.Server.ServerMaps.TryGetValue(conductor.Npc.Map, out map))
                                    map.RemoveNpc(conductor.Npc, packet);
                                //---------------------------------------

                                var npc = Npc.Create();
                                npc.UID = client.MoveNpcUID;
                                npc.Mesh = stream.Mesh;
                                npc.Map = client.Player.Map;
                                npc.X = X;
                                npc.Y = Y;
                                npc.NpcType = Role.Flags.NpcType.Talker;

                                client.Map.AddNpc(npc);
                                client.Player.View.SendView(packet.NpcCreate(npc), true);

                                MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID].Npc = npc;
                                MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID].ToX = ToX;
                                MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID].ToY = ToY;
                                MsgTournaments.MsgSchedules.GuildWar.GuildConductors[(NpcID)client.MoveNpcUID].ToMap = ToMap;

                                client.Inventory.Remove(client.UseItem, 1, packet);

                            }
                            else
                                client.SendSysMesage("Invalid map Location.");
                        }
                        else
                            client.SendSysMesage("Invalid map Location.");
                    }
                    else
                    {
                        client.SendSysMesage("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~change~my~location.");
                    }
                    return;
                }
                //if (stream->UID == client.MoveNpcUID && client.MoveNpcUID != 0)
                {
                    // MoveNpcUID/MoveNpcMesh are set when a furniture item is used and cleared once it is placed:
                    // resending the same spawn packet must not place it again (it used to, without an item).
                    if (client.MoveNpcUID == 0)
                        return;
                    if (client.MyHouse == null)
                    {
                        client.SendSysMesage("Sorry, you not have house.");
                        return;
                    }
                    if (client.MyHouse.Furnitures.Count >= Database.HouseTable.CountFurnitures(client.MyHouse.Level))
                    {
                        client.SendSysMesage("Sorry, you can have only" + Database.HouseTable.CountFurnitures(client.MyHouse.Level) + " Furnitures in you house.");
                        return;
                    }
                    if (client.MyHouse.Furnitures.ContainsKey(client.MoveNpcUID))
                    {
                        client.SendSysMesage("Sorry, but you have that npc");
                        return;
                    }
                    if (client.Player.DynamicID != client.Player.UID)
                    {
                        client.SendSysMesage("You have to be in your own house to be able to display it.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        return;
                    }
                    Npc np = Npc.Create();
                    np.UID = client.MoveNpcUID;
                    np.X = stream.X;
                    np.Y = stream.Y;
                    np.Mesh = (ushort)(client.MoveNpcMesh - client.MoveNpcMesh % 10 + stream.Mesh % 10);// the item's look, the client only picks the facing
                    np.Map = client.Player.Map;
                    np.DynamicID = client.Player.UID;
                    np.NpcType = Role.Flags.NpcType.Furniture;
                    if (np.Mesh == 8200)
                        np.NpcType = Role.Flags.NpcType.Talker;
                    if (!client.MyHouse.Furnitures.TryAdd(np.UID, np))
                        return;
                    client.MoveNpcUID = 0;
                    client.MoveNpcMesh = 0;
                    client.Player.View.SendView(packet.NpcCreate(np), true);
                }
            }
        }
    }
}
