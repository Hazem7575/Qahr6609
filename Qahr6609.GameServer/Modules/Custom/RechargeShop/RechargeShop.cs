using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609.Database
{
   public static class RechargeShop
    {


       public static ConcurrentDictionary<uint, Role.Player.RechargeType> RechargeAccounts = new ConcurrentDictionary<uint, Role.Player.RechargeType>();


       public static void UpdateRecharge(Client.GameClient user)
       {
           if (RechargeAccounts.ContainsKey(user.Player.UID))
           {
               RechargeAccounts[user.Player.UID] = user.Player.RechargeProgress;
           }
           else
               RechargeAccounts.TryAdd(user.Player.UID, user.Player.RechargeProgress);
       }

       public static void Save()
       {
           using (Database.DBActions.Write _wr = new Database.DBActions.Write("RechargeShop.txt"))
           {

               var dictionary = RechargeAccounts.ToArray();
               foreach (var item in dictionary)
                   _wr.Add(item.Key + "/" + item.Value);
               _wr.Execute(DBActions.Mode.Open);
           }


       }

       public static void Load()
       {
           using (Database.DBActions.Read r = new Database.DBActions.Read("RechargeShop.txt"))
           {
               if (r.Reader())
               {
                   int count = r.Count;
                   for (uint x = 0; x < count; x++)
                   {
                       Database.DBActions.ReadLine readerline = new DBActions.ReadLine(r.ReadString(""), '/');
                       uint key = readerline.Read((uint)0);
                       Role.Player.RechargeType val = (Role.Player.RechargeType)readerline.Read((ulong)0);
                       if (!RechargeAccounts.ContainsKey(key))
                           RechargeAccounts.TryAdd(key, val);
                   }
               }
           }
       }

    }
}
