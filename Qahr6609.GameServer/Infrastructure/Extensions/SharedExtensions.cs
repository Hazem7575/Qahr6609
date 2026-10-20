using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609
{
    public static class SharedExtensions
    {
        public static void Iterate<T>(this T[] collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }
    }
}
