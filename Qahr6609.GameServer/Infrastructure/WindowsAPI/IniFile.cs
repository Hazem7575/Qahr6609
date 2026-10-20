using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.WindowsAPI
{
    public unsafe class IniFile
    {
        public string FileName;
        public IniFile(string _FileName)
        {
            FileName = Program.ServerConfig.DbLocation + _FileName;
        }
        public IniFile()
        {
            FileName = null;
        }

        public const int
            Int32_Size = 15,
            Int16_Size = 9,
            Int8_Size = 6,
            Bool_Size = 6,
            Double_Size = 20,
            Int64_Size = 22,
            Float_Size = 10;

        public static Func<string, int> ToInt32 = new Func<string, int>(int.Parse);
        public static Func<string, uint> ToUInt32 = new Func<string, uint>(uint.Parse);
        public static Func<string, short> ToInt16 = new Func<string, short>(short.Parse);
        public static Func<string, ushort> ToUInt16 = new Func<string, ushort>(ushort.Parse);
        public static Func<string, sbyte> ToInt8 = new Func<string, sbyte>(sbyte.Parse);
        public static Func<string, byte> ToUInt8 = new Func<string, byte>(byte.Parse);
        public static Func<string, bool> ToBool = new Func<string, bool>(bool.Parse);
        public static Func<string, double> ToDouble = new Func<string, double>(double.Parse);
        public static Func<string, long> ToInt64 = new Func<string, long>(long.Parse);
        public static Func<string, ulong> ToUInt64 = new Func<string, ulong>(ulong.Parse);
        public static Func<string, float> ToFloat = new Func<string, float>(float.Parse);

        // ===== [Storage] read overlay (from Qahr) =====
        // Another source for the reads of one file (the players row from MySQL while LoadCharacter runs). Without an
        // overlay for the file (the normal case) reads take exactly the old path. VerifyOnly = the value still comes
        // from the file and the overlay value is compared with it (after parsing to the type); differences are reported.
        public interface IReadOverlay
        {
            bool TryGet(string section, string key, out string value);
            void OnMismatch(string section, string key, string fileValue, string overlayValue);
            // the file was written for this key while the overlay is active -> stop serving it (the file is newer)
            void OnWrite(string section, string key);
        }
        private sealed class OverlayEntry { public IReadOverlay Src; public bool VerifyOnly; }
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, OverlayEntry> overlays =
            new System.Collections.Concurrent.ConcurrentDictionary<string, OverlayEntry>(StringComparer.OrdinalIgnoreCase);
        private static int overlayCount;

        private static string OverlayKey(string fileName)
        {
            try { return System.IO.Path.GetFullPath(fileName); } catch { return fileName; }
        }
        public static void SetOverlay(string fileName, IReadOverlay src, bool verifyOnly)
        {
            if (fileName == null || src == null) return;
            if (overlays.TryAdd(OverlayKey(fileName), new OverlayEntry { Src = src, VerifyOnly = verifyOnly }))
                System.Threading.Interlocked.Increment(ref overlayCount);
            else
                overlays[OverlayKey(fileName)] = new OverlayEntry { Src = src, VerifyOnly = verifyOnly };
        }
        public static void ClearOverlay(string fileName)
        {
            OverlayEntry e;
            if (fileName != null && overlays.TryRemove(OverlayKey(fileName), out e))
                System.Threading.Interlocked.Decrement(ref overlayCount);
        }
        private OverlayEntry GetOverlay()
        {
            if (System.Threading.Volatile.Read(ref overlayCount) == 0 || FileName == null) return null;
            OverlayEntry e;
            return overlays.TryGetValue(OverlayKey(FileName), out e) ? e : null;
        }
        private void NotifyWrite(string section, string key)
        {
            OverlayEntry ov = GetOverlay();
            if (ov != null) ov.Src.OnWrite(section, key);
        }
        /// <summary>[Storage] Called with the file name after every write through IniFile (PlayerStorage copies
        /// Users\uid.ini changes made outside SaveClient into MySQL). null = off.</summary>
        public static Action<string> Written;
        private void AfterWrite()
        {
            IniCache.Invalidate(FileName);
            var hook = Written;
            if (hook != null && FileName != null)
            {
                try { hook(FileName); }
                catch { }
            }
        }
        // GetPrivateProfileString returns at most Size-1 characters
        private static string Clip(string v, int size)
        {
            if (v == null) return "";
            return size > 0 && v.Length > size - 1 ? v.Substring(0, Math.Max(0, size - 1)) : v;
        }
        private static T ParseOverlay<T>(string v, T Default, Func<string, T> callback)
        {
            try
            {
                // the DB keeps bools as 1/0, the file as True/False
                if ((object)callback == (object)ToBool)
                {
                    if (v == "1") return (T)(object)true;
                    if (v == "0") return (T)(object)false;
                }
                return callback.Invoke(v);
            }
            catch { return Default; }
        }

        public string ReadString(string Section, string Key, string Default, int Size)
        {
            OverlayEntry ov = GetOverlay();
            string dbv;
            if (ov != null && ov.Src.TryGet(Section, Key, out dbv))
            {
                dbv = Clip(dbv, Size);
                if (!ov.VerifyOnly) return dbv;
                string fv = RawRead(Section, Key, Default, Size);
                if (!string.Equals(fv, dbv, StringComparison.Ordinal))
                    ov.Src.OnMismatch(Section, Key, fv, dbv);
                return fv;
            }
            return RawRead(Section, Key, Default, Size);
        }
        private string RawRead(string Section, string Key, string Default, int Size)
        {
            string cached;
            if (IniCache.TryRead(FileName, Section, Key, Default, Size, out cached))
                return cached;
            char* lpBuffer = stackalloc char[Size];
            Kernel32.GetPrivateProfileStringW(Section, Key, Default, lpBuffer, Size, FileName);
            return new string(lpBuffer).Trim('\0');
        }
        public string ReadString(string Section, string Key, string Default)
        {
            return ReadString(Section, Key, Default, 255);
        }
        public string ReadBigString(string Section, string Key, string Default)
        {
            return ReadString(Section, Key, Default, 255 * 4);
        }
        public void ReadString(string Section, string Key, void* Default, void* Buffer, int Size)
        {
            Kernel32.GetPrivateProfileStringA(Section, Key, Default, (sbyte*)Buffer, Size, FileName);
        }
       
        public T ReadValue<T>(string Section, string Key, T Default, Func<string, T> callback)
        {
            return ReadValue<T>(Section, Key, Default, callback, 255);
        }
        public T ReadValue<T>(string Section, string Key, T Default, Func<string, T> callback, int BufferSize)
        {
            OverlayEntry ov = GetOverlay();
            string dbv;
            if (ov != null && ov.Src.TryGet(Section, Key, out dbv))
            {
                T dbT = ParseOverlay(Clip(dbv, BufferSize), Default, callback);
                if (!ov.VerifyOnly) return dbT;
                T fileT;
                try { fileT = callback.Invoke(RawRead(Section, Key, Default.ToString(), BufferSize)); }
                catch { fileT = Default; }
                if (!EqualityComparer<T>.Default.Equals(fileT, dbT))
                    ov.Src.OnMismatch(Section, Key, Convert.ToString(fileT, System.Globalization.CultureInfo.InvariantCulture),
                        Convert.ToString(dbT, System.Globalization.CultureInfo.InvariantCulture) + " (db raw '" + dbv + "')");
                return fileT;
            }
            try
            {
                return callback.Invoke(ReadString(Section, Key, Default.ToString(), BufferSize));
            }
            catch
            {
                return Default;
            }
        }

        public int ReadInt32(string Section, string Key, int Default)
        {
            return ReadValue<int>(Section, Key, Default, ToInt32, Int32_Size);
        }
        public ulong ReadUInt64(string Section, string Key, ulong Default)
        {
            return ReadValue<ulong>(Section, Key, Default, ToUInt64, Int64_Size);
        }
        public long ReadInt64(string Section, string Key, long Default)
        {
            return ReadValue<long>(Section, Key, Default, ToInt64, Int64_Size);
        }
        public double ReadDouble(string Section, string Key, double Default)
        {
            return ReadValue<double>(Section, Key, Default, ToDouble, Double_Size);
        }
        public uint ReadUInt32(string Section, string Key, uint Default)
        {
            return ReadValue<uint>(Section, Key, Default, ToUInt32, Int32_Size);
        }
        public short ReadInt16(string Section, string Key, short Default)
        {
            return ReadValue<short>(Section, Key, Default, ToInt16, Int16_Size);
        }
        public ushort ReadUInt16(string Section, string Key, ushort Default)
        {
            return ReadValue<ushort>(Section, Key, Default, ToUInt16, Int16_Size);
        }
        public sbyte ReadSByte(string Section, string Key, sbyte Default)
        {
            return ReadValue<sbyte>(Section, Key, Default, ToInt8, Int8_Size);
        }
        public byte ReadByte(string Section, string Key, byte Default)
        {
            return ReadValue<byte>(Section, Key, Default, ToUInt8, Int8_Size);
        }
        public bool ReadBool(string Section, string Key, bool Default)
        {
            return ReadValue<bool>(Section, Key, Default, ToBool, Bool_Size);
        }
        public float ReadFloat(string Section, string Key, float Default)
        {
            return ReadValue<float>(Section, Key, Default, ToFloat, Float_Size);
        }

        public void WriteString(string Section, string Key, string Value)
        {
            if (Value == null)
            {
                Value = "0";
            }
            NotifyWrite(Section, Key);
            Kernel32.WritePrivateProfileString(Section, Key, Value, FileName);
            AfterWrite();
        }
        public void Write<T>(string Section, string Key, T Value)
        {
            NotifyWrite(Section, Key);
            Kernel32.WritePrivateProfileString(Section, Key, Value.ToString(), FileName);
            AfterWrite();
        }
        public void WriteStruct(string Section, string Key, void* lpStruct, int Size)
        {
            NotifyWrite(Section, Key);
            Kernel32.WritePrivateProfileStructW(Section, Key, lpStruct, Size, FileName);
            AfterWrite();
        }

        public string[] GetSectionNames(int BufferSize)
        {
            char* lpBuffer = stackalloc char[BufferSize];
            int Size = Kernel32.GetPrivateProfileSectionNamesW(lpBuffer, BufferSize, FileName);
            if (Size == 0)
                return new string[0];
            return new string(lpBuffer, 0, Size - 1).Split('\0');
        }
        public string[] GetSection(string Section, int BufferSize)
        {
            char* lpBuffer = stackalloc char[BufferSize];
            int Size = Kernel32.GetPrivateProfileSectionW(Section, lpBuffer, BufferSize, FileName);
            if (Size == 0)
                return new string[0];
            return new string(lpBuffer, 0, Size - 1).Split('\0');
        }
        public string[] GetSectionNames()
        {
            return GetSectionNames(4096);
        }
        public string[] GetSection(string Section)
        {
            return GetSection(Section, 4096);
        }
        public bool SectionExists(string Section)
        {
            char* temp = stackalloc char[Section.Length + 1];
            int r = Kernel32.GetPrivateProfileSectionW(Section, temp, Section.Length + 1, FileName);
            return (temp[0] != 0);
        }
        public bool KeyExists(string Section, string Key)
        {
            const char not_used = (char)0x007F;
            const string not_used_s = "\007F";

            char* temp = stackalloc char[2];
            uint r = Kernel32.GetPrivateProfileStringW(Section, Key, not_used_s, temp, 2, FileName);
            return (r == 0 && temp[0] == 0) || (r > 0 && temp[0] != not_used);
        }

      
    }
    /// <summary>
    /// In-memory copy of .ini files for IniFile.ReadString, used only while the server loads
    /// (Begin() ... End() around Database.Server.Initialize/LoadDatabase). GetPrivateProfileString
    /// opens and parses the whole file again for every key, so the ~35k reads of Monsters\*.ini,
    /// MonsterSpawns\*.ini and maps\*.ini took most of the start-up time. Outside Begin/End every
    /// read goes to Win32 exactly as before.
    /// Lookup rules copy GetPrivateProfileStringW: no BOM = ANSI code page, FF FE = UTF-16;
    /// section and key names compared case-insensitively and trimmed; the first section and the
    /// first key with that name win; the value is trimmed and one pair of matching quotes removed;
    /// a missing key returns the default without its trailing blanks; the result is cut to Size-1.
    /// A file is re-parsed when its size or last-write time changes, and every IniFile write
    /// drops it, so a file written during loading is never read stale.
    /// </summary>
    public static class IniCache
    {
        sealed class Entry
        {
            public long Length;
            public DateTime Time;
            public Dictionary<string, Dictionary<string, string>> Sections;
            public long CheckedAt;// [FastBoot] trustClock ms of the last size/time check (0 = never)
        }

        // [FastBoot, from Qahr] Boot only (Server.Initialize, before any socket is open): a file whose size + write time
        // were checked less than TrustMs ago is not stat'ed again (the boot reads ~72k keys, each one did a FileInfo
        // stat). IniFile writers still Invalidate() the entry, so the server's own writes are seen at once; only an
        // outside edit made in the same 100 ms is picked up on the next check instead. Off = the old check on every read.
        static volatile bool trustRecent;
        const long TrustMs = 100;
        static readonly System.Diagnostics.Stopwatch trustClock = System.Diagnostics.Stopwatch.StartNew();
        public static bool TrustRecent { get { return trustRecent; } set { trustRecent = value; } }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Entry> files =
            new System.Collections.Concurrent.ConcurrentDictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        static volatile bool active;
        static readonly string windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static int Hits, Parsed, Fallbacks;

        public static bool Active { get { return active; } }

        public static void Begin()
        {
            files.Clear();
            Hits = Parsed = Fallbacks = 0;
            active = true;
        }

        public static void End()
        {
            trustRecent = false;// [FastBoot]
            active = false;
            files.Clear();
        }

        /// <summary>[FastBoot, Qahr idea] Parses every *.ini under the folder into the cache on pool threads, so the boot
        /// loaders that walk it (monsters, spawns, maps) find the files already parsed. Only fills the cache: every read
        /// still goes through TryRead (size/time check, Invalidate on writes), so values are the same as without it.</summary>
        public static System.Threading.Tasks.Task Prefetch(string folder, bool subFolders)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                if (!active || !System.IO.Directory.Exists(folder))
                    return;
                var list = System.IO.Directory.GetFiles(folder, "*.ini", subFolders ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
                var options = new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Math.Min(8, Environment.ProcessorCount / 2)) };
                System.Threading.Tasks.Parallel.ForEach(list, options, file =>
                {
                    if (!active)
                        return;
                    try
                    {
                        string path = FullPath(file);
                        if (files.ContainsKey(path))
                            return;
                        var info = new System.IO.FileInfo(path);
                        // CheckedAt = -TrustMs: never trusted as "recently checked", so the first real read still compares
                        // size + write time once (a write + Invalidate racing this parse cannot leave a stale entry in use)
                        var entry = new Entry { Length = info.Length, Time = info.LastWriteTimeUtc, Sections = Parse(System.IO.File.ReadAllBytes(path)), CheckedAt = -TrustMs };
                        files.TryAdd(path, entry);// a reader that got there first keeps its own entry
                        if (!active)
                            files.Clear();// End() ran meanwhile: leave nothing behind
                    }
                    catch { }// the reader parses it itself
                });
            });
        }

        public static void Invalidate(string fileName)
        {
            if (!active || fileName == null)
                return;
            Entry removed;
            try { files.TryRemove(FullPath(fileName), out removed); }
            catch { files.Clear(); }
        }

        static string FullPath(string fileName)
        {
            // Like the profile API: a bare file name lives in the Windows folder, anything with a
            // folder part is resolved from the current directory.
            if (fileName.IndexOf('\\') < 0 && fileName.IndexOf('/') < 0)
                return System.IO.Path.Combine(windowsDir, fileName);
            return System.IO.Path.GetFullPath(fileName);
        }

        /// <summary>False = not handled (the caller asks Win32).</summary>
        public static bool TryRead(string fileName, string section, string key, string def, int size, out string result)
        {
            result = null;
            if (!active || fileName == null || section == null || key == null || size <= 0)
                return false;
            try
            {
                string path = FullPath(fileName);
                Dictionary<string, Dictionary<string, string>> sections = null;
                Entry recent;// [FastBoot] boot: reuse an entry checked < TrustMs ago without a new stat
                long now = trustRecent ? trustClock.ElapsedMilliseconds : 0;
                if (trustRecent && files.TryGetValue(path, out recent) && now - System.Threading.Volatile.Read(ref recent.CheckedAt) < TrustMs)
                    sections = recent.Sections;
                else
                {
                    var info = new System.IO.FileInfo(path);
                    if (info.Exists)
                    {
                        Entry entry;
                        if (!files.TryGetValue(path, out entry) || entry.Length != info.Length || entry.Time != info.LastWriteTimeUtc)
                        {
                            entry = new Entry { Length = info.Length, Time = info.LastWriteTimeUtc, Sections = Parse(System.IO.File.ReadAllBytes(path)) };
                            if (trustRecent) entry.CheckedAt = now;
                            files[path] = entry;
                            System.Threading.Interlocked.Increment(ref Parsed);
                        }
                        else if (trustRecent)
                            System.Threading.Volatile.Write(ref entry.CheckedAt, now);
                        sections = entry.Sections;
                    }
                }
                string value = null;
                Dictionary<string, string> keys;
                if (sections != null && sections.TryGetValue(section.Trim(), out keys))
                    keys.TryGetValue(key.Trim(), out value);
                if (value == null)
                    value = def == null ? "" : def.TrimEnd(' ');
                if (value.Length > size - 1)
                    value = value.Substring(0, size - 1);
                int nul = value.IndexOf('\0');
                if (nul >= 0)
                    value = value.Substring(0, nul);
                result = value.Trim('\0');
                System.Threading.Interlocked.Increment(ref Hits);
                return true;
            }
            catch
            {
                System.Threading.Interlocked.Increment(ref Fallbacks);
                return false;
            }
        }

        static Dictionary<string, Dictionary<string, string>> Parse(byte[] bytes)
        {
            string text;
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                text = Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
            else
                text = global::Conquer6609.AnsiText.Value.GetString(bytes);
            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> current = null;
            int pos = 0;
            while (pos < text.Length)
            {
                int end = text.IndexOf('\n', pos);
                if (end < 0)
                    end = text.Length;
                string line = text.Substring(pos, end - pos).Trim();
                pos = end + 1;
                if (line.Length == 0)
                    continue;
                if (line[0] == '[')
                {
                    int close = line.IndexOf(']');
                    string name = (close < 0 ? line.Substring(1) : line.Substring(1, close - 1)).Trim();
                    if (result.ContainsKey(name))
                        current = null;// a repeated section is never reached by the API
                    else
                    {
                        current = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        result.Add(name, current);
                    }
                    continue;
                }
                if (current == null)
                    continue;
                int eq = line.IndexOf('=');
                string k = eq < 0 ? line : line.Substring(0, eq).Trim();
                if (current.ContainsKey(k))
                    continue;
                string v = eq < 0 ? "" : line.Substring(eq + 1).Trim();
                if (v.Length >= 2 && (v[0] == '"' || v[0] == '\'') && v[v.Length - 1] == v[0])
                    v = v.Substring(1, v.Length - 2);
                current.Add(k, v);
            }
            return result;
        }
    }
}
