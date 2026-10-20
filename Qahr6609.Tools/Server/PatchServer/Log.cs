using System;
using System.Globalization;

namespace PatchServer
{
    /// <summary>
    /// Thread-safe console logger. All output goes through one lock so lines
    /// written from many connection threads never interleave.
    /// </summary>
    internal static class Log
    {
        private static readonly object Sync = new object();

        /// <summary>Writes a timestamped informational line.</summary>
        public static void Info(string message)
        {
            Write(Stamp() + message, null);
        }

        /// <summary>Writes a timestamped warning line (yellow).</summary>
        public static void Warn(string message)
        {
            Write(Stamp() + "WARNING: " + message, ConsoleColor.Yellow);
        }

        /// <summary>Writes a timestamped error line (red).</summary>
        public static void Error(string message)
        {
            Write(Stamp() + "ERROR: " + message, ConsoleColor.Red);
        }

        /// <summary>Writes a line without timestamp (banner, command output).</summary>
        public static void Raw(string message)
        {
            Write(message, null);
        }

        /// <summary>Writes a line without timestamp in the given color.</summary>
        public static void Raw(string message, ConsoleColor color)
        {
            Write(message, color);
        }

        /// <summary>
        /// Logs one HTTP request in the format:
        /// [HH:mm:ss] ip METHOD /path -> status bytes
        /// </summary>
        public static void Request(string ip, string method, string path, int status, long bytes, string note)
        {
            // [Security] the path is client-controlled (decoded %0d%0a): control characters would forge log lines
            path = path == null ? "" : new string(Array.ConvertAll(path.ToCharArray(), c => char.IsControl(c) ? '?' : c));
            string line = Stamp() + ip + " " + method + " " + path + " -> "
                + status.ToString(CultureInfo.InvariantCulture) + " "
                + bytes.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(note))
            {
                line += " (" + note + ")";
            }

            ConsoleColor? color = null;
            if (status >= 400)
            {
                color = ConsoleColor.DarkYellow;
            }
            Write(line, color);
        }

        /// <summary>Formats a byte count as a human readable string.</summary>
        public static string FormatSize(long bytes)
        {
            const double Kb = 1024.0;
            const double Mb = Kb * 1024.0;
            const double Gb = Mb * 1024.0;

            if (bytes >= Gb)
            {
                return (bytes / Gb).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
            }
            if (bytes >= Mb)
            {
                return (bytes / Mb).ToString("0.00", CultureInfo.InvariantCulture) + " MB";
            }
            if (bytes >= Kb)
            {
                return (bytes / Kb).ToString("0.0", CultureInfo.InvariantCulture) + " KB";
            }
            return bytes.ToString(CultureInfo.InvariantCulture) + " B";
        }

        private static string Stamp()
        {
            return "[" + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + "] ";
        }

        private static void Write(string line, ConsoleColor? color)
        {
            lock (Sync)
            {
                bool colored = false;
                if (color.HasValue)
                {
                    try
                    {
                        Console.ForegroundColor = color.Value;
                        colored = true;
                    }
                    catch (Exception)
                    {
                        // Console without color support (redirected output): ignore.
                    }
                }

                try
                {
                    Console.WriteLine(line);
                }
                catch (Exception)
                {
                    // Never let logging failures take the server down.
                }

                if (colored)
                {
                    try
                    {
                        Console.ResetColor();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
