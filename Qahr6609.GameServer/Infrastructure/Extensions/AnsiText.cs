using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Conquer6609
{
    /// <summary>
    /// [net10] Explicit replacement for <c>Encoding.Default</c> (same idea as Qahr's AnsiText).
    /// On .NET Framework 4.8 <c>Encoding.Default</c> is the system ANSI code page (GetACP(), e.g. 1252/1256).
    /// On .NET 10 it is ALWAYS UTF-8, which would silently change how player names, ini files and other wire
    /// strings are encoded/decoded. This returns the same ANSI code page that the 4.8 build uses, so both builds
    /// behave the same. On .NET 10 the legacy code pages exist only after
    /// Encoding.RegisterProvider(CodePagesEncodingProvider.Instance), done first thing in Program.Main.
    /// </summary>
    internal static class AnsiText
    {
        public static readonly Encoding Value = Create();

        private static Encoding Create()
        {
            try { return Encoding.GetEncoding((int)GetACP()); }
            catch { return Encoding.Default; }
        }

        [DllImport("kernel32.dll")]
        private static extern uint GetACP();
    }
}
