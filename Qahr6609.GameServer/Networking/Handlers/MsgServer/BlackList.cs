using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conquer6609.Game.MsgServer
{
    /// <summary>
    /// Per-player black list (Friends window, "Black List" tab). Names are kept in
    /// PlayersBlackList\&lt;UID&gt;.txt (one name per line), written on every change and loaded at login.
    /// A blacklisted player cannot whisper the owner nor send him friend, team or trade requests.
    /// </summary>
    public static class BlackList
    {
        public const int MaxNames = 50;

        static readonly ConcurrentDictionary<uint, HashSet<string>> Lists = new ConcurrentDictionary<uint, HashSet<string>>();

        static string PathOf(uint uid)
        {
            return Program.ServerConfig.DbLocation + "\\PlayersBlackList\\" + uid + ".txt";
        }

        static HashSet<string> NewSet()
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Loads the list at login (called from ServerDatabase.LoadCharacter).</summary>
        public static void Load(uint uid)
        {
            var set = NewSet();
            try
            {
                string path = PathOf(uid);
                if (File.Exists(path))
                {
                    foreach (var line in File.ReadAllLines(path))
                    {
                        string name = line.Trim();
                        if (name.Length > 0 && set.Count < MaxNames)
                            set.Add(name);
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[BlackList] cannot load " + uid + ": " + e.Message);
            }
            Lists[uid] = set;
        }

        static void Save(uint uid, HashSet<string> set)
        {
            try
            {
                string path = PathOf(uid);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                string[] lines;
                lock (set)
                    lines = set.ToArray();
                string tmp = path + ".tmp";
                File.WriteAllLines(tmp, lines);
                if (File.Exists(path))
                    File.Replace(tmp, path, null);
                else
                    File.Move(tmp, path);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[BlackList] cannot save " + uid + ": " + e.Message);
            }
        }

        public static bool Add(uint uid, string name)
        {
            HashSet<string> set = Lists.GetOrAdd(uid, u => NewSet());
            lock (set)
            {
                if (set.Contains(name))
                    return true;
                if (set.Count >= MaxNames)
                    return false;
                set.Add(name);
            }
            Save(uid, set);
            return true;
        }

        public static void Remove(uint uid, string name)
        {
            HashSet<string> set;
            if (!Lists.TryGetValue(uid, out set))
                return;
            bool removed;
            lock (set)
                removed = set.Remove(name);
            if (removed)
                Save(uid, set);
        }

        /// <summary>True when the player <paramref name="ownerUid"/> has <paramref name="senderName"/> on his black list.</summary>
        public static bool Blocks(uint ownerUid, string senderName)
        {
            if (string.IsNullOrEmpty(senderName))
                return false;
            HashSet<string> set;
            if (!Lists.TryGetValue(ownerUid, out set))
                return false;
            lock (set)
                return set.Contains(senderName);
        }
    }
}
