// Server rates (shell.ini [Rates]), read while the server runs: edit shell.ini and the new rate applies within 5 s.
//   Exp=2     experience from monsters and quests = official x Exp (the damage-based official formula, AdjustExp)
//   Spell=2   skill experience per damage point
//   Prof=2    weapon proficiency per damage point
// Was hard-coded in Program.ServerConfig (UserExpRate 50000 = a fast "private server" rate).
using System;
using System.IO;

namespace Conquer6609
{
    public static class Rates
    {
        static long checkedAt;
        static DateTime fileTime;

        static uint Get(string key, uint def, uint max)
        {
            long now = Environment.TickCount64;
            if (now - checkedAt > 5000)
            {
                checkedAt = now;
                try
                {
                    DateTime t = File.Exists(Framework.Settings.FilePath) ? File.GetLastWriteTimeUtc(Framework.Settings.FilePath) : DateTime.MinValue;
                    if (t != fileTime) { fileTime = t; Framework.Settings.Reload(); }
                }
                catch { }
            }
            int v = Framework.Settings.Int("Rates", key, (int)def);
            return (uint)Math.Max(1, Math.Min((int)max, v));
        }

        public static uint Exp { get { return Get("Exp", 2, 1000000); } }
        public static uint Spell { get { return Get("Spell", 2, 1000); } }
        public static uint Prof { get { return Get("Prof", 2, 1000); } }
    }
}
