using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScriptHazem.ClientFiles;

namespace GuardTool
{
    /// <summary>
    /// guardtool encode Guard.txt Guard.dat   plain XML  -> client file
    /// guardtool decode Guard.dat Guard.txt   client file -> plain XML
    /// guardtool verify Guard.txt Guard.dat   exit 0 when encode(txt) == dat byte for byte
    /// guardtool list   Guard.dat             print the server rows
    /// Cipher: see README.txt (same function both ways). Exit codes: 0 ok, 1 usage, 2 error, 3 verify mismatch.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                return Usage();
            }
            string cmd = args[0].ToLowerInvariant();
            try
            {
                switch (cmd)
                {
                    case "encode":
                    case "decode":
                        if (args.Length != 3) return Usage();
                        byte[] input = File.ReadAllBytes(args[1]);
                        string outDir = Path.GetDirectoryName(Path.GetFullPath(args[2]));
                        if (!string.IsNullOrEmpty(outDir)) Directory.CreateDirectory(outDir);
                        File.WriteAllBytes(args[2], GuardFile.Crypt(input));
                        Console.WriteLine(cmd + "d " + input.Length + " bytes: " + args[1] + " -> " + args[2]);
                        return 0;

                    case "verify":
                        if (args.Length != 3) return Usage();
                        byte[] txt = File.ReadAllBytes(args[1]);
                        byte[] dat = File.ReadAllBytes(args[2]);
                        bool ok = Equal(GuardFile.Crypt(txt), dat) && Equal(GuardFile.Crypt(dat), txt);
                        Console.WriteLine(ok ? "OK: " + args[2] + " is " + args[1] + " encoded (" + txt.Length + " bytes)" : "MISMATCH");
                        return ok ? 0 : 3;

                    case "list":
                        List<GuardFile.Entry> rows = GuardFile.Read(args[1]);
                        foreach (GuardFile.Entry e in rows)
                        {
                            Console.WriteLine("ServerName=" + e.LoginName + "  FlashName=" + e.DisplayName + "  " + e.LoginHost + ":" + e.LoginPort
                                + "  FlashIcon=" + e.Icon + "  FlashHint=" + e.Hint);
                        }
                        if (rows.Count == 0) Console.WriteLine("(no server rows - wrong file or not a Guard.dat)");
                        return rows.Count > 0 ? 0 : 2;

                    default:
                        return Usage();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("error: " + ex.Message);
                return 2;
            }
        }

        private static bool Equal(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
            return true;
        }

        private static int Usage()
        {
            Console.WriteLine("guardtool encode <Guard.txt> <Guard.dat>");
            Console.WriteLine("guardtool decode <Guard.dat> <Guard.txt>");
            Console.WriteLine("guardtool verify <Guard.txt> <Guard.dat>");
            Console.WriteLine("guardtool list   <Guard.dat>");
            return 1;
        }
    }
}
