using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database.Shops
{
    public static class ShopFile
    {
        public static Dictionary<uint, Shop> Shops;
        public class Shop
        {
         
            public uint UID;
            public MoneyType MoneyType;
            public int Count { get { return Items.Count; } }
            public List<uint> Items;

            public List<uint> BoundItems = new List<uint>();
          
        }
        public static void Load()
        {
            var shops = new Dictionary<uint, Shop>();// // built aside and published at the end: a live reload never shows a half-filled table
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\shops\\Shop.dat");
            int Count = reader.ReadInt32("Header", "Amount", 0);
            for (int x = 0; x < Count; x++)
            {
                Shop shop = new Shop();
                shop.UID = reader.ReadUInt32("Shop" + x.ToString(), "ID", 0);
                shop.MoneyType = (MoneyType)reader.ReadUInt32("Shop" + x.ToString(), "MoneyType", 0);
                int Items = reader.ReadInt32("Shop" + x.ToString(), "ItemAmount", 0);
                shop.Items = new List<uint>();
                for (int i = 0; i < Items; i++)
                {
                    shop.Items.Add(reader.ReadUInt32("Shop" + x.ToString(), "Item" + i.ToString(), 0));
                }
                if (!shops.ContainsKey(shop.UID))
                    shops.Add(shop.UID, shop);
            }
            Shops = shops;
        }
        public enum MoneyType
        {
            Gold = 0,
            ConquerPoints = 1,
            HonorPoints = 2, 
            BoundConquerPoints = 3
        }
    }
}
