using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;

namespace Conquer6609.Role.Instance
{
    public class House
    {
        public struct DBNpc
        {
            public uint UID;
            public uint UnKnow;
            public ushort X;
            public ushort Y;
            public ushort Mesh;
            public Role.Flags.NpcType NpcType;
            public Role.MapObjectType ObjType;
            public ushort Sort;
            public uint DynamicID;
            public uint Map;

            public static DBNpc Create(Game.MsgNpc.Npc npc)
            {
                DBNpc Dbnpc = new DBNpc();
                Dbnpc.UID = npc.UID;
                Dbnpc.UnKnow = npc.UnKnow;
                Dbnpc.X = npc.X;
                Dbnpc.Y = npc.Y;
                Dbnpc.Mesh = npc.Mesh;
                Dbnpc.NpcType = npc.NpcType;
                Dbnpc.ObjType = npc.ObjType;
                Dbnpc.Sort = npc.Sort;
                Dbnpc.DynamicID = npc.DynamicID;
                Dbnpc.Map = npc.Map;
                return Dbnpc;
            }
            public static Game.MsgNpc.Npc GetServerNpc(DBNpc Dbnpc)
            {
                Game.MsgNpc.Npc npc = new Game.MsgNpc.Npc();
                npc.UID = Dbnpc.UID;
                npc.UnKnow = Dbnpc.UnKnow;
                npc.X = Dbnpc.X;
                npc.Y = Dbnpc.Y;
                npc.Mesh = Dbnpc.Mesh;
                npc.NpcType = Dbnpc.NpcType;
                npc.ObjType = Dbnpc.ObjType;
                npc.Sort = Dbnpc.Sort;
                npc.DynamicID = Dbnpc.DynamicID;
                npc.Map = Dbnpc.Map;
                return npc;
            }
        }
        public House(uint UID)
        {
            if (!HousePoll.ContainsKey(UID))
                HousePoll.TryAdd(UID, this);

            if (!Program.BlockAttackMap.Contains(UID))
                Program.BlockAttackMap.Add(UID);
        }
        public static ConcurrentDictionary<uint, House> HousePoll = new ConcurrentDictionary<uint, House>();

        public byte Level = 1;
        public ConcurrentDictionary<uint, Game.MsgNpc.Npc> Furnitures = new ConcurrentDictionary<uint, Game.MsgNpc.Npc>();

        /// <summary>File layout: byte level, int count, count x DBNpc.</summary>
        public unsafe static bool ValidateFile(string path)
        {
            long len = new FileInfo(path).Length;
            if (len < 1 + sizeof(int))
                return false;
            int count;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var br = new BinaryReader(fs))
            {
                br.ReadByte();
                count = br.ReadInt32();
            }
            return count >= 0 && 1 + sizeof(int) + (long)count * sizeof(DBNpc) == len;
        }
        internal unsafe static void Load(Client.GameClient client)
        {
            string path = null;
            if (!HousePoll.ContainsKey(client.Player.UID))
                path = Database.ServerDatabase.PickLoadPath(Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin", ValidateFile, client.Player.UID);
            Load(client, path);
        }
        internal unsafe static void Load(Client.GameClient client, string path)
        {
            if (HousePoll.ContainsKey(client.Player.UID))
            {
                client.MyHouse = HousePoll[client.Player.UID];
            }
            else if (path != null)
            {
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (!binary.Open(path, FileMode.Open))
                    throw new InvalidDataException("Cannot open " + path);
                {
                    int ItemCount = 0;
                    byte Level = 0;
                    binary.Read(&Level, sizeof(byte));

                    client.MyHouse = new House(client.Player.UID);
                    client.MyHouse.Level = Level;

                    binary.Read(&ItemCount, sizeof(int));

                    Game.MsgNpc.Npc Furnitures;
                    for (int x = 0; x < ItemCount; x++)
                    {
                        DBNpc DBNpcc = new DBNpc();
                        binary.Read(&DBNpcc, sizeof(DBNpc));
                        Furnitures = DBNpc.GetServerNpc(DBNpcc);
                        if (!client.MyHouse.Furnitures.ContainsKey(Furnitures.UID))
                            client.MyHouse.Furnitures.TryAdd(Furnitures.UID, Furnitures);
                    }
                    binary.Close();
                }
            }
        }
        internal unsafe static void Save(Client.GameClient client)
        {
            if (client.MyHouse != null)
            {
                var furnitures = client.MyHouse.Furnitures.Values.ToArray();
                using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
                {
                    if (binary.Open(Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin", FileMode.Create))
                    {
                        byte level = client.MyHouse.Level;
                        int ItemCount = furnitures.Length;
                        binary.Write(&level, sizeof(byte));
                        binary.Write(&ItemCount, sizeof(int));
                        foreach (var furniture in furnitures)
                        {
                            Game.MsgNpc.Npc Furnitures = furniture;
                            DBNpc Db_npc = DBNpc.Create(Furnitures);
                            binary.Write(&Db_npc, sizeof(DBNpc));
                        }
                        binary.Close();
                    }
                }
            }
        }
    }
}
