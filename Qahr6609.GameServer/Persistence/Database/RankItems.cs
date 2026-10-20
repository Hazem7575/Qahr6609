using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Conquer6609.Game.MsgServer;
using ProtoBuf;

namespace Conquer6609.Database
{
    public class RankItems
    {
        public static Dictionary<uint, Rank> RankPoll = new Dictionary<uint, Rank>();

        public class Rank
        {
            public const int MaxItems = 50;
            public Extensions.SafeDictionary<uint, MsgGameItem> Items = new Extensions.SafeDictionary<uint, MsgGameItem>();
            public readonly object SynRoot = new object();
            // Replaced (never mutated in place) under SynRoot; readers take the same lock.
            private MsgGameItem[] Top50 = new MsgGameItem[0];

            public uint GetItemRank(uint UID)
            {
                MsgGameItem item;
                if (Items.TryGetValue(UID, out item))
                    return item.PerfectionRank;
                return 100;
            }

            public MsgGameItem[] GetRank50Items()
            {
                lock (SynRoot)
                    return Top50;
            }


            public void AddItem(MsgGameItem item)
            {
                // The whole check-then-update must be atomic (it reads Top50 and Items).
                lock (SynRoot)
                    AddItemLocked(item);
            }
            private void AddItemLocked(MsgGameItem item)
            {
                if (Items.Count < MaxItems)
                {
                    if (!Items.ContainsKey(item.UID))
                    {
                        Items.Add(item.UID, item);
                        lock (SynRoot)
                        {
                            Top50 = Items.Values.OrderByDescending(p => p.ItemPoints).ToArray();
                            for (int x = 0; x < Top50.Length; x++)
                            {
                                Top50[x].PerfectionRank = (ushort)(x + 1);
                                
                            }
                        }
                    }
                    else if (Items.ContainsKey(item.UID))
                    {
                        uint points = Items[item.UID].Position;
                        ushort rank = Items[item.UID].PerfectionRank;
                        Items[item.UID] = item;
                        item.PerfectionRank = rank;
                        if (points != item.ItemPoints)
                        {
                            lock (SynRoot)
                            {
                                Top50 = Items.Values.OrderByDescending(p => p.ItemPoints).ToArray();
                                for (int x = 0; x < Top50.Length; x++)
                                    Top50[x].PerfectionRank = (ushort)(x + 1);
                            }
                        }
                    }
                }
                else
                {
                    if (Items.ContainsKey(item.UID))
                    {
                        ushort rank = Items[item.UID].PerfectionRank;
                        Items[item.UID] = item;
                        item.PerfectionRank = rank;
                    }
                    else
                    {
                        var last = Top50[Top50.Length - 1];
                        if (item.ItemPoints > last.ItemPoints)
                        {
                            Items.Remove(last.UID);
                            Items.Add(item.UID, item);

                            lock (SynRoot)
                            {
                                Top50 = Items.Values.OrderByDescending(p => p.ItemPoints).ToArray();
                                for (int x = 0; x < Top50.Length; x++)
                                    Top50[x].PerfectionRank = (ushort)(x + 1);
                            }
                        }
                    }
                }
            }
        }

        public static void CreateRanks()
        {
            for (int x = 0; x < 11; x++)
                RankPoll.Add((uint)(x + 1), new Rank());
        }


        public static unsafe void SaveRanks()
        {
            if (!File.Exists(Program.ServerConfig.DbLocation + "\\PerfectionRanking.bin"))
            {
                File.Create(Program.ServerConfig.DbLocation + "\\PerfectionRanking.bin").Dispose();
            }
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PerfectionRanking.bin", FileMode.Open))
            {
                ClientItems.DBItem DBItem = new ClientItems.DBItem();
                int RanksCount = 11;
                binary.Write(&RanksCount, sizeof(int));
                foreach (var rank in RankPoll.Values)
                {
                    int ItemsCounts = rank.Items.Count;
                    binary.Write(&ItemsCounts, sizeof(int));
                    foreach (var item in rank.Items.GetValues())
                    {
                        DBItem.GetDBItem(item);
                        if (!binary.Write(&DBItem, sizeof(ClientItems.DBItem)))
                            Console.WriteLine("test");
                        var info = DBItem.GetPerfectionInfo(item);
                        if (!binary.Write(&info, sizeof(ClientItems.Perfection)))
                            Console.WriteLine("test");
                    }
                }
                binary.Close();
            } 
        }
        public unsafe static void LoadAllItems()
        {
             CreateRanks();
             if (File.Exists(Program.ServerConfig.DbLocation + "\\PerfectionRanking.bin"))
             {
                 WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                 if (binary.Open(Program.ServerConfig.DbLocation + "\\PerfectionRanking.bin", FileMode.Open))
                 {
                     ClientItems.DBItem Item;
                     int RanksCount;
                     binary.Read(&RanksCount, sizeof(int));
                     for (int x = 0; x < RanksCount; x++)
                     {
                         int ItemsCount;
                         binary.Read(&ItemsCount, sizeof(int));
                         for (int i = 0; i < ItemsCount; i++)
                         {
                             binary.Read(&Item, sizeof(ClientItems.DBItem));
                             Game.MsgServer.MsgGameItem ClienItem = Item.GetDataItem();
                             ClientItems.Perfection info = new ClientItems.Perfection();
                             binary.Read(&info, sizeof(ClientItems.Perfection));
                             ClienItem.PerfectionLevel = info.Level;
                             ClienItem.OwnerUID = info.OwnerUID;
                             ClienItem.OwnerName = info.OwnerName;
                             ClienItem.PerfectionProgress = info.Progres;
                             ClienItem.Signature = info.SpecialText;

                             RankPoll[(uint)(x + 1)].AddItem(ClienItem);
                         }
                     }
                     binary.Close();
                 }
             }
             // CreateRanks();
             else
             {
                 foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
                 {
                     // Only live item files: skip N.bin.bak / N.bin.tmp / N.bin.corrupt-* left by atomic saves.
                     if (!string.Equals(Path.GetExtension(fname), ".bin", StringComparison.OrdinalIgnoreCase))
                         continue;
                     var str = fname.Split(new string[1] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                     uint ClientUID;
                     if (!uint.TryParse(Path.GetFileNameWithoutExtension(str), out ClientUID))
                         continue;
                     WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Users\\" + ClientUID + ".ini");

                     string Name = reader.ReadString("Character", "Name", "None");


                     Dictionary<uint, MsgGameItem> MyItems = new Dictionary<uint, MsgGameItem>();
                     WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                     if (binary.Open(fname, FileMode.Open))
                     {
                         ClientItems.DBItem Item;
                         int ItemCount;
                         binary.Read(&ItemCount, sizeof(int));

                         for (int x = 0; x < ItemCount; x++)
                         {
                             binary.Read(&Item, sizeof(ClientItems.DBItem));
                             if (Item.ITEM_ID == 750000)//demonExterminator jar
                                 continue;

                             Game.MsgServer.MsgGameItem ClienItem = Item.GetDataItem();

                             if (Item.DepositeCount != 0)
                             {
                                 uint DepositeCount = Item.DepositeCount;
                                 for (int i = 0; i < DepositeCount; i++)
                                 {
                                     binary.Read(&Item, sizeof(ClientItems.DBItem));
                                 }
                             }
                             if (Item.Position > 0 && Item.Position < 40)
                             {
                                 if (MyItems.ContainsKey(ClienItem.UID) == false)
                                 {

                                     MyItems.Add(ClienItem.UID, ClienItem);
                                 }
                             }
                         }
                         binary.Read(&ItemCount, sizeof(int));
                         for (int x = 0; x < ItemCount; x++)
                         {
                             ClientItems.Perfection info = new ClientItems.Perfection();
                             binary.Read(&info, sizeof(ClientItems.Perfection));

                             if (MyItems.ContainsKey(info.ItemUID))
                             {
                                 var item = MyItems[info.ItemUID];
                                 item.PerfectionLevel = info.Level;
                                 item.OwnerUID = info.OwnerUID;
                                 item.OwnerName = info.OwnerName;
                                 item.PerfectionProgress = info.Progres;
                                 item.Signature = info.SpecialText;

                             }
                         }
                         binary.Close();
                     }
                     foreach (var _item in MyItems.Values)
                     {
                         try
                         {
                             if (_item.GetPerfectionPosition == -1 || _item.OwnerUID == 0)
                                 continue;
                             if (Database.Server.ItemsBase.ContainsKey(_item.ITEM_ID))
                             {
                                 _item.ItemPoints = Database.ItemType.GetItemPoints(Database.Server.ItemsBase[_item.ITEM_ID], _item);
                                 RankPoll[(uint)_item.GetPerfectionPosition].AddItem(_item);
                             }
                         }
                         catch (Exception)
                         {

                         }
                     }
                 }
             }
        }
    }
}
