using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database.Shops
{
    public static class RacePointShop
    {
        public const uint UID = 6001;

        public static RacePointShopBase Shop;
        public class RacePointItem
        {
            public uint ID, Price;
        }
        public class RacePointShopBase
        {
            public int Count { get { return Items.Count; } }
            public Dictionary<uint, RacePointItem> Items;
        }
        public static void Load()
        {
            string[] text = File.ReadAllLines(Program.ServerConfig.DbLocation + "\\shops\\RacePointShop.ini");
            var shop = new RacePointShopBase();// built aside, published at the end (live reload)
            shop.Items = new Dictionary<uint, RacePointItem>();
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x].ToLower();

                if (line.StartsWith("[normal]"))
                {
                    continue;
                }
                else if (line.StartsWith("[recommend]"))
                {
                    break;
                }
                else
                {
                    if (line.Length > 3)
                    {
                        string[] Parts = line.Split(';')[0].Split(' ');
                        RacePointItem item = new RacePointItem();

                        item.ID = uint.Parse(Parts[0]);
                        item.Price = uint.Parse(Parts[1]);

                        shop.Items.Add(item.ID, item);
                    }
                }
            }
            Shop = shop;
        }
    }
}
