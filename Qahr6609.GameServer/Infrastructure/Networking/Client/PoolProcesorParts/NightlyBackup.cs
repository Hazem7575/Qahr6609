using Conquer6609.Database;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgLoader;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.AttackHandler;
using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Client
{
    public partial class PoolProcesses
    {
        #region daily tasks (KernelThread)
        // Same times and actions as before, but driven once per day by KernelThread (not by every online player on
        // the MapGroupThread): the backup starts at 23:55:00 (or the first tick up to 23:59:58), the restart at
        // 23:59:59 on its own thread. Both also run when nobody is online.
        static string lastBackupDay, lastRestartDay;
        static int restartStarted;

        /// <summary>Called by KernelThread every second.</summary>
        public static void DailyTasksTick()
        {
            DateTime now = DateTime.Now;
            if (now.Hour != 23 || now.Minute < 55)
                return;
            string today = now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            int secOfHour = now.Minute * 60 + now.Second;
            if (lastBackupDay != today && secOfHour >= 55 * 60 && secOfHour <= 59 * 60 + 58)
            {
                lastBackupDay = today;
                StartNightlyBackup();
            }
            // armed from 23:59:50 so a late/irregular tick can't miss the second; the restart thread waits for 23:59:59
            if (lastRestartDay != today && secOfHour >= 59 * 60 + 50)
            {
                lastRestartDay = today;
                StartDailyRestart(now.Date.AddHours(23).AddMinutes(59).AddSeconds(59));
            }
        }

        static void StartDailyRestart(DateTime at)
        {
            if (System.Threading.Interlocked.CompareExchange(ref restartStarted, 1, 0) != 0)
                return;
            var thread = new System.Threading.Thread(() =>
            {
                try
                {
                    TimeSpan wait = at - DateTime.Now;
                    if (wait > TimeSpan.Zero)
                        System.Threading.Thread.Sleep(wait);
                    RunDailyRestart();
                }
                catch (Exception e)
                {
                    MyConsole.WriteException(e);
                    System.Threading.Interlocked.Exchange(ref restartStarted, 0);
                }
            });
            thread.IsBackground = false;
            thread.Name = "DailyRestart";
            thread.Start();
        }

        static void RunDailyRestart()
        {
            // let a running nightly backup finish (bounded) before the restart
            var backupWait = System.Diagnostics.Stopwatch.StartNew();
            while (BackupRunning && backupWait.ElapsedMilliseconds < 120000)
                System.Threading.Thread.Sleep(100);
            Database.Server.SaveDatabase();
            System.Threading.Thread.Sleep(1000);
            Program.ProcessConsoleEvent(0);
            System.Diagnostics.Process hproces = new System.Diagnostics.Process();
            hproces.StartInfo.FileName = System.Reflection.Assembly.GetEntryAssembly().Location;
            hproces.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            hproces.Start();

            Environment.Exit(0);
        }
        #endregion

        #region nightly backup
        public const int BackupDaysToKeep = 14;
        static int backupRunning;
        public static bool BackupRunning { get { return System.Threading.Volatile.Read(ref backupRunning) != 0; } }

        /// <summary>Runs the nightly database backup on a background thread (once at a time).</summary>
        public static void StartNightlyBackup()
        {
            if (System.Threading.Interlocked.CompareExchange(ref backupRunning, 1, 0) != 0)
                return;
            try
            {
                var thread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        RunNightlyBackup();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("********************");
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        System.Threading.Interlocked.Exchange(ref backupRunning, 0);
                    }
                });
                thread.IsBackground = true;
                thread.Name = "NightlyBackup";
                thread.Priority = System.Threading.ThreadPriority.BelowNormal;
                thread.Start();
            }
            catch (Exception e)
            {
                System.Threading.Interlocked.Exchange(ref backupRunning, 0);
                MyConsole.WriteException(e);
            }
        }

        /// <summary>
        /// Copies the database into AABackUP\yyyy-MM-dd (built in a ".partial" folder and renamed when done),
        /// then keeps only the newest BackupDaysToKeep dated folders. A file that can't be copied is reported
        /// and skipped instead of aborting the backup. Older day-number folders (1..31) are left untouched.
        /// </summary>
        public static void RunNightlyBackup()
        {
            string root = Program.ServerConfig.DbLocation + "\\AABackUP\\";
            string name = DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            string create = root + name;
            if (Directory.Exists(create))
            {
                Console.WriteLine("" + create + " Already Backed Up !");
                return;
            }
            string work = create + ".partial";
            if (Directory.Exists(work))
                Directory.Delete(work, true);
            Directory.CreateDirectory(work);

            int failed = 0;
            string[] files = new string[] { "Arena.ini", "BanIp.txt", "BanUID.txt", "ClassPkWar.ini", "ElitePk.ini", "GuildWarInfo.ini", "Npcs.txt",
                "SkillTeamPK.ini", "SobNpcs.txt", "SuperGuildWarInfo.ini", "TeamArena.ini", "TeamElitePK.ini", "Votes.txt" };
            foreach (string file in files)
                failed += BackupCopyFile(Program.ServerConfig.DbLocation + "\\" + file, work + "\\" + file) ? 0 : 1;
            string[] folders = new string[] { "Users", "PlayersSpells", "PlayersProfs", "PlayersItems", "Houses", "Clans", "Guilds" };
            foreach (string folder in folders)
                failed += BackupCopyFolder(new DirectoryInfo(Program.ServerConfig.DbLocation + "\\" + folder), work + "\\" + folder);

            Directory.Move(work, create);
            Console.WriteLine("Done BackUp Database For Today ( " + name + " )" + (failed > 0 ? ", " + failed + " file(s) skipped" : "") + ".");
            if (failed > 0)
                Security.Guard.Audit("backup", 0, "", failed + " file(s) could not be copied into " + create);
            PruneBackups(root);
        }
        static bool BackupCopyFile(string source, string target)
        {
            try
            {
                if (!File.Exists(source))
                    return true;//optional file
                File.Copy(source, target, true);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Backup: " + source + ": " + e.Message);
                return false;
            }
        }
        static int BackupCopyFolder(DirectoryInfo source, string target)
        {
            int failed = 0;
            if (!source.Exists)
                return 0;
            Directory.CreateDirectory(target);
            FileInfo[] files;
            try
            {
                files = source.GetFiles();
            }
            catch (Exception e)
            {
                Console.WriteLine("Backup: " + source.FullName + ": " + e.Message);
                return 1;
            }
            foreach (FileInfo fi in files)
            {
                // .bak files are copied too (they are the fallback when a save file is damaged)
                if (fi.Name.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase)
                    || fi.Name.IndexOf(".corrupt-", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;//unfinished save or quarantined copy
                if (!BackupCopyFile(fi.FullName, Path.Combine(target, fi.Name)))
                    failed++;
            }
            DirectoryInfo[] dirs;
            try
            {
                dirs = source.GetDirectories();
            }
            catch (Exception e)
            {
                Console.WriteLine("Backup: " + source.FullName + ": " + e.Message);
                return failed + 1;
            }
            foreach (DirectoryInfo sub in dirs)
                failed += BackupCopyFolder(sub, Path.Combine(target, sub.Name));
            return failed;
        }
        static void PruneBackups(string root)
        {
            try
            {
                var dated = new List<KeyValuePair<DateTime, string>>();
                foreach (string dir in Directory.GetDirectories(root))
                {
                    if (dir.EndsWith(".partial", StringComparison.OrdinalIgnoreCase))
                    {
                        try { Directory.Delete(dir, true); }//left over from an interrupted backup
                        catch (Exception e) { Console.WriteLine("Backup: could not delete " + dir + ": " + e.Message); }
                        continue;
                    }
                    DateTime day;
                    if (DateTime.TryParseExact(Path.GetFileName(dir), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out day))
                        dated.Add(new KeyValuePair<DateTime, string>(day, dir));
                }
                foreach (var old in dated.OrderByDescending(p => p.Key).Skip(BackupDaysToKeep))
                {
                    try
                    {
                        Directory.Delete(old.Value, true);
                    }
                    catch (Exception e) { Console.WriteLine("Backup: could not delete " + old.Value + ": " + e.Message); }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        #endregion
    }
}