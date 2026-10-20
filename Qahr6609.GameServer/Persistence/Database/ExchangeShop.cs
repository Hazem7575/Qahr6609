using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
   public static class ExchangeShop
    {
       /// <summary>One price component of a good: the currency item and how many of it one good costs.</summary>
       public struct Cost
       {
           public uint ItemID;
           public uint Amount;
       }
       public class Item
       {
           public int Index;
           public uint ItemID;
           /// <summary>Shop page the good belongs to (MsgExchangeShop DwParam2: 1/2 = Elite PK shops, 7 = Hau Kan, 9 = ...).</summary>
           public byte RankType;
           /// <summary>Per-day purchase limit; 0 = unlimited.</summary>
           public uint CountItems;
           /// <summary>Monopoly flag of the good handed out; non-zero = bound.</summary>
           public uint UnKnow;
           /// <summary>First price component (kept for old callers).</summary>
           public uint ReuquestItem;
           public uint RequestItemCount;
           /// <summary>All price components. A good listing two currencies costs both of them (e.g. 88 Fortune Coins AND 88 Venus` Blessings).</summary>
           public Cost[] Costs = new Cost[0];

           public bool Unlimited { get { return CountItems == 0; } }
           public bool Bound { get { return UnKnow != 0; } }
       }
       public static Dictionary<int, Item> Items = new Dictionary<int, Item>();

       /// <summary>
       /// exchange_shop_goods.txt, one good per line, the same columns as the client's ini/exchange_shop_goods.dat:
       /// Index ItemID ShopType DailyLimit Monopoly Cost1Item Cost1Amount Cost2Item Cost2Amount Cost3Item Cost3Amount Cost4Item Cost4Amount
       /// Columns may be separated by spaces, tabs or "@@". A missing file leaves the shop empty (logged) instead of stopping the server.
       /// </summary>
       public static void LoadDBInfo()
       {
           Items.Clear();
           string path = Program.ServerConfig.DbLocation + "exchange_shop_goods.txt";
           if (!System.IO.File.Exists(path))
           {
               MyConsole.WriteLine("[ExchangeShop] " + path + " not found. The exchange shops are empty.");
               return;
           }
           string[] baseText = System.IO.File.ReadAllLines(path);
           int lineNo = 0;
           foreach (var bas_line in baseText)
           {
               lineNo++;
               string text = bas_line.Trim();
               if (text.Length == 0 || text.StartsWith("//") || text.StartsWith("#"))
                   continue;
               var line = text.Split(new string[] { " ", "\t", "@@" }, StringSplitOptions.RemoveEmptyEntries);
               try
               {
                   if (line.Length < 7)
                       throw new FormatException("expected at least 7 columns");
                   Item obj = new Item()
                   {
                       Index = int.Parse(line[0]),
                       ItemID = uint.Parse(line[1]),
                       RankType = byte.Parse(line[2]),
                       CountItems = uint.Parse(line[3]),
                       UnKnow = uint.Parse(line[4]),
                   };
                   var costs = new List<Cost>();
                   for (int c = 5; c + 1 < line.Length && c <= 11; c += 2)
                   {
                       uint costItem = uint.Parse(line[c]);
                       uint costAmount = uint.Parse(line[c + 1]);
                       if (costItem == 0 || costAmount == 0)
                           continue;
                       bool merged = false;
                       for (int k = 0; k < costs.Count; k++)
                       {
                           if (costs[k].ItemID == costItem)
                           {
                               costs[k] = new Cost() { ItemID = costItem, Amount = costs[k].Amount + costAmount };
                               merged = true;
                           }
                       }
                       if (!merged)
                           costs.Add(new Cost() { ItemID = costItem, Amount = costAmount });
                   }
                   if (costs.Count == 0)
                       throw new FormatException("no price");
                   obj.Costs = costs.ToArray();
                   obj.ReuquestItem = obj.Costs[0].ItemID;
                   obj.RequestItemCount = obj.Costs[0].Amount;
                   if (Items.ContainsKey(obj.Index))
                       throw new FormatException("duplicate index " + obj.Index);
                   Items.Add(obj.Index, obj);
               }
               catch (Exception e)
               {
                   MyConsole.WriteLine("[ExchangeShop] exchange_shop_goods.txt line " + lineNo + " skipped: " + e.Message);
               }
           }
           MyConsole.WriteLine("Loading: " + Items.Count + " Exchange Shop goods");
       }
    }
}
