using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database.Shops
{
    public static class EShopFile
    {
        public static Dictionary<uint, ShopFile.Shop> Shops;

        public static void Load()
        {
            Shops = Build();
        }

        /// <summary>emoneyshop.ini as a new table (published by the caller: Load, or the live reload after EShopV2File.Merge).</summary>
        public static Dictionary<uint, ShopFile.Shop> Build()
        {
            string[] text = File.ReadAllLines(Program.ServerConfig.DbLocation + "\\shops\\emoneyshop.ini");
            ShopFile.Shop shop = new ShopFile.Shop();
            shop.Items = new List<uint>();
            var Shops = new Dictionary<uint, ShopFile.Shop>();
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x].Replace("_", " ");
                string[] split = line.Split('=');
                if (split[0] == "ID")
                {
                    if (shop.UID == 0)
                        shop.UID = uint.Parse(split[1]);
                    else
                    {
                        if (!Shops.ContainsKey(shop.UID))
                        {
                            Shops.Add(shop.UID, shop);
                            shop = new ShopFile.Shop();
                            shop.Items = new List<uint>();
                            shop.UID = uint.Parse(split[1]);
                        }
                    }
                }
                else if (split[0] == "MoneyType")
                {
                    shop.MoneyType = (ShopFile.MoneyType)byte.Parse(split[1]);
                }
                else if (split[0].Contains("Item") && split[0] != "ItemAmount")
                {
                    uint ID = uint.Parse(split[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (!shop.Items.Contains(ID))
                        shop.Items.Add(ID);
                }
            }
            if (!Shops.ContainsKey(shop.UID))
                Shops.Add(shop.UID, shop);
            return Shops;
        }
    }
}
