using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.WindowsAPI
{
    public unsafe static class MyString
    {
        /// <summary>
        /// Get a pointer to the Windows-1252 encoded string.
        /// It will create a null-terminating string...
        /// </summary>
        public static Byte* ToPointer(this String Str)
        {
            Byte[] Buffer = Encoding.GetEncoding("Windows-1252").GetBytes(Str + "\0");
            Byte* ptr = (Byte*)Kernel32.malloc(Buffer.Length);

            fixed (Byte* pBuffer = Buffer)
                Kernel32.memcpy(ptr, pBuffer, Buffer.Length);
            return ptr;
        }

        /// <summary>
        /// Get a pointer to the Windows-1252 encoded string.
        /// It will create a null-terminating string...
        /// </summary>
        public static Byte* ToPointer(this String Str, Byte* ptr)
        {
            Byte[] Buffer = Encoding.GetEncoding("Windows-1252").GetBytes(Str + "\0");
            fixed (Byte* pBuffer = Buffer)
                Kernel32.memcpy(ptr, pBuffer, Buffer.Length);
            return ptr;
        }
    }
}
