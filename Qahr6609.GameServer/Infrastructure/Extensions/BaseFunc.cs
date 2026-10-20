using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer6609
{
    public static class BaseFunc
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 srand(UInt64 seed);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 rand();

        public static int RandGet(int nMax, bool bRealRand)
        {
            if (nMax <= 0)
                nMax = 1;

            if (bRealRand)
                srand(Extensions.Time32.Now.Value);
            long val = rand();
            Console.WriteLine(val);
            return (int)(val % nMax);
        }

        //////////////////////////////////////////////////////////////////////
        public static double RandomRateGet(double dRange)
        {
            double pi = 3.1415926;

            int nRandom = RandGet(999, true) + 1;
            double a = Math.Sin(nRandom * pi / 1000);
            double b;
            if (nRandom >= 90)
                b = (1.0 + dRange) - Math.Sqrt(Math.Sqrt(a)) * dRange;
            else
                b = (1.0 - dRange) + Math.Sqrt(Math.Sqrt(a)) * dRange;

            return b;
        }

    }
}
