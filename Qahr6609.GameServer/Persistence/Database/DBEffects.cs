using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.Database
{
   public static class DBEffects
    {

       public static Extensions.Counter Counter = new Extensions.Counter(1);

       public static Dictionary<uint, string> Effecte = new Dictionary<uint, string>();


       public static void Load()
       {
       }
    }
}
