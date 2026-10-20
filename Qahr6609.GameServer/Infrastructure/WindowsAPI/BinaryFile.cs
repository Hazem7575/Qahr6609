using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer6609.WindowsAPI
{
    /// <summary>
    /// Raw struct file access.
    /// Opening with FileMode.Create is atomic: data goes to "&lt;file&gt;.tmp" and Close() flushes it and
    /// replaces the original, keeping the previous version as "&lt;file&gt;.bak". If a write failed, or the
    /// object is disposed without Close(), the original file is left untouched.
    /// Performance: an atomic write collects the records in memory and writes the temp file with a single
    /// write at Close() (instead of one WriteFile call per record); other modes go through a buffered
    /// FileStream (one read/write syscall per 64 KB instead of one per record). The bytes on disk are unchanged.
    /// </summary>
    public unsafe class BinaryFile : IDisposable
    {
        const int StreamBufferSize = 64 * 1024;

        /// <summary>[Storage] Called after an atomic write was committed (Close of a FileMode.Create file, WriteAllBytesAtomic)
        /// with the exact bytes now in the file. Set once at start-up by PlayerBlobStorage (MySQL copy); null = off.</summary>
        public static Action<string, byte[], int> Committed;
        /// <summary>[Storage] For FileMode.Open: the bytes to read instead of the file (the MySQL copy), or null = read the
        /// file. Set by PlayerBlobStorage only with shell.ini [Storage] PlayerReadSource=Db/Verify; null = off.</summary>
        public static Func<string, byte[]> BlobResolver;

        private Stream pFile;// a FileStream, or a read-only MemoryStream for a blob from BlobResolver
        private string targetName;   // final file name when writing atomically
        private string tempName;     // "<target>.tmp" while writing atomically
        private bool writeFailed;

        // atomic write (FileMode.Create): records are collected here and written once at Close()
        private byte[] wbuf;
        private int wlen, wpos;
        // scratch buffer for the buffered FileStream path (other modes)
        private byte[] scratch;

        public BinaryFile(string FileName, FileMode Mode)
        {
            Open(FileName, Mode);
        }
        public BinaryFile()
        {
            pFile = null;
        }

        bool Buffered { get { return tempName != null; } }

        public int Position
        {
            get
            {
                if (Success)
                {
                    if (Buffered)
                        return wpos;
                    return (int)pFile.Position;
                }
                else
                {
                    throw new IOException("File isn't open/failed to open previously");
                }
            }
            set
            {
                if (Success)
                {
                    if (Buffered)
                    {
                        if (value < 0)
                            throw new ArgumentOutOfRangeException("value");
                        EnsureCapacity(value);
                        if (value > wlen)
                        {
                            Array.Clear(wbuf, wlen, value - wlen);
                            wlen = value;
                        }
                        wpos = value;
                        return;
                    }
                    pFile.Position = value;
                }
                else
                {
                    throw new IOException("File isn't open/failed to open previously");
                }
            }
        }
        public bool Success { get { return pFile != null; } }

        /// <summary>Length of the open file in bytes.</summary>
        public long Length
        {
            get
            {
                if (!Success)
                    throw new IOException("File isn't open/failed to open previously");
                if (Buffered)
                    return wlen;
                return pFile.Length;
            }
        }

        public bool Open(string FileName, FileMode Mode)
        {
            pFile = null;
            targetName = null;
            tempName = null;
            writeFailed = false;
            wbuf = null;
            wlen = wpos = 0;
            try
            {
                if (Mode == FileMode.Create)
                {
                    string tmp = FileName + ".tmp";
                    FileStream fs = null;
                    // Another save of the same file may still hold the temp file for a moment.
                    for (int attempt = 0; ; attempt++)
                    {
                        try
                        {
                            fs = new FileStream(tmp, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096);
                            break;
                        }
                        catch (IOException ioe)
                        {
                            if (attempt >= 20 || ioe is DirectoryNotFoundException || ioe is PathTooLongException)
                                throw;
                            System.Threading.Thread.Sleep(25);
                        }
                    }
                    pFile = fs;
                    targetName = FileName;
                    tempName = tmp;
                    wbuf = new byte[4096];
                }
                else
                {
                    var resolver = BlobResolver;
                    byte[] blob = null;
                    if (Mode == FileMode.Open && resolver != null)
                    {
                        try { blob = resolver(FileName); }
                        catch { blob = null; }
                    }
                    if (blob != null)
                        pFile = new MemoryStream(blob, false);
                    else
                        pFile = new FileStream(FileName, Mode, Mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite, FileShare.Read, StreamBufferSize);
                }
            }
            catch (Exception e)
            {
                pFile = null;
                targetName = null;
                tempName = null;
                wbuf = null;
                bool missing = (e is FileNotFoundException) && Mode != FileMode.Create;
                if (!missing)
                    Security.Guard.Audit(Mode == FileMode.Create ? "save-error" : "load-error", 0, "", "open " + FileName + ": " + e.Message);
            }
            return Success;
        }

        /// <summary>Closes the file. For an atomic write this writes the collected data, flushes and replaces
        /// the target. Returns false if the data could not be committed (the previous file is kept).</summary>
        public bool Close()
        {
            if (!Success)
                throw new IOException("File isn't open/failed to open previously");
            if (tempName == null)
            {
                Stream fs = pFile;
                pFile = null;
                scratch = null;
                fs.Close();
                return !writeFailed;
            }
            return Commit();
        }

        private bool Commit()
        {
            FileStream fs = (FileStream)pFile;
            string tmp = tempName, target = targetName;
            byte[] data = wbuf;
            int length = wlen;
            pFile = null;
            tempName = null;
            targetName = null;
            wbuf = null;
            wlen = wpos = 0;
            bool ok = false;
            try
            {
                try
                {
                    if (length > 0)
                        fs.Write(data, 0, length);// one write for the whole file
                    fs.Flush(true);
                }
                finally
                {
                    fs.Close();
                }
                if (writeFailed)
                {
                    Security.Guard.Audit("save-error", 0, "", "incomplete write, kept previous " + target);
                }
                else
                {
                    CommitTemp(tmp, target);
                    ok = true;
                    OnCommitted(target, data, length);
                }
            }
            catch (Exception e)
            {
                Security.Guard.Audit("save-error", 0, "", "commit " + target + ": " + e.Message);
            }
            if (!ok)
            {
                try { if (File.Exists(tmp)) File.Delete(tmp); }
                catch { }
            }
            return ok;
        }

        /// <summary>Drops an atomic write without touching the target file.</summary>
        public void Abort()
        {
            if (!Success)
                return;
            Stream fs = pFile;
            string tmp = tempName;
            pFile = null;
            tempName = null;
            targetName = null;
            wbuf = null;
            scratch = null;
            wlen = wpos = 0;
            try { fs.Close(); }
            catch { }
            if (tmp != null)
            {
                try { if (File.Exists(tmp)) File.Delete(tmp); }
                catch { }
            }
        }

        /// <summary>Disposing without Close() aborts a pending atomic write (e.g. after an exception).</summary>
        public void Dispose()
        {
            Abort();
        }

        /// <summary>
        /// Replaces target with an already written and flushed temp file, keeping "&lt;target&gt;.bak".
        /// Used by BinaryFile and by DBActions.Write.
        /// </summary>
        public static void CommitTemp(string tmp, string target)
        {
            string bak = target + ".bak";
            if (File.Exists(target))
            {
                try
                {
                    File.Replace(tmp, target, bak, true);
                    return;
                }
                catch (Exception e)
                {
                    // Without the temp file there is nothing newer to commit.
                    if (!File.Exists(tmp))
                        throw;
                    // A failed Replace can leave the target already renamed away (to .bak): then the
                    // newest data only exists in tmp, so move it into place instead of copying a missing file.
                    if (!File.Exists(target))
                    {
                        File.Move(tmp, target);
                        return;
                    }
                    // File.Replace is not available everywhere (e.g. some network/FAT volumes): fall back
                    // to copy + overwrite, still keeping the previous version as .bak when possible.
                    try { File.Copy(target, bak, true); }
                    catch (Exception be)
                    {
                        Conquer6609.Security.Guard.Audit("save-error", 0, "", "backup " + bak + " failed (" + e.Message + " / " + be.Message + "), committing newest data anyway");
                    }
                    File.Copy(tmp, target, true);
                    File.Delete(tmp);// only after the newest data is in place
                    return;
                }
            }
            File.Move(tmp, target);
        }

        /// <summary>Writes a complete file atomically (tmp + flush + replace, previous version kept as .bak)
        /// with a single write call.</summary>
        public static void WriteAllBytesAtomic(string target, byte[] data, int count)
        {
            string tmp = target + ".tmp";
            try
            {
                using (FileStream fs = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None, 4096))
                {
                    if (count > 0)
                        fs.Write(data, 0, count);
                    fs.Flush(true);
                }
                CommitTemp(tmp, target);
            }
            catch
            {
                try { if (File.Exists(tmp)) File.Delete(tmp); }
                catch { }
                throw;
            }
            OnCommitted(target, data, count);
        }

        static void OnCommitted(string target, byte[] data, int length)
        {
            var hook = Committed;
            if (hook == null)
                return;
            try { hook(target, data ?? new byte[0], Math.Max(0, length)); }
            catch (Exception e) { MyConsole.WriteLine("[Storage] copy of " + target + " failed: " + e.Message); }
        }

        public bool Reopen(string FileName, FileMode Mode)
        {
            Close();
            return Open(FileName, Mode);
        }

        void EnsureCapacity(int size)
        {
            if (size <= wbuf.Length)
                return;
            int n = wbuf.Length;
            while (n < size)
                n = n < (1 << 29) ? n * 2 : size;
            byte[] grown = new byte[n];
            Buffer.BlockCopy(wbuf, 0, grown, 0, wlen);
            wbuf = grown;
        }

        byte[] Scratch(int size)
        {
            if (scratch == null || scratch.Length < size)
                scratch = new byte[Math.Max(size, 256)];
            return scratch;
        }

        public bool Read(void* Buffer, int Count, int Size)
        {
            return Read(Buffer, Count * Size);
        }
        public bool Read(void* Buffer, int AmountOfBytes)
        {
            if (AmountOfBytes < 0)
                throw new ArgumentException("AmountOfBytes");
            if (!Success)
                throw new IOException("File isn't open/failed to open previously");
            if (AmountOfBytes == 0)
                return true;
            if (Buffered)
            {
                // reading back an atomic write that is not committed yet
                int available = Math.Max(0, Math.Min(AmountOfBytes, wlen - wpos));
                if (available > 0)
                {
                    System.Runtime.InteropServices.Marshal.Copy(wbuf, wpos, (IntPtr)Buffer, available);
                    wpos += available;
                }
                return available == AmountOfBytes;
            }
            byte[] tmp = Scratch(AmountOfBytes);
            int read = 0;
            try
            {
                while (read < AmountOfBytes)
                {
                    int n = pFile.Read(tmp, read, AmountOfBytes - read);
                    if (n <= 0)
                        break;
                    read += n;
                }
            }
            catch (IOException)
            {
                // same as a failed ReadFile: report a short read
            }
            if (read > 0)
                System.Runtime.InteropServices.Marshal.Copy(tmp, 0, (IntPtr)Buffer, read);
            return (read == AmountOfBytes);
        }
        public bool Write(void* Buffer, int Count, int Size)
        {
            return Write(Buffer, Count * Size);
        }
        public bool Write(void* Buffer, int AmountOfBytes)
        {
            if (AmountOfBytes < 0)
                throw new ArgumentException("AmountOfBytes");
            if (!Success)
                throw new IOException("File isn't open/failed to open previously");
            if (AmountOfBytes == 0)
                return true;
            if (Buffered)
            {
                EnsureCapacity(wpos + AmountOfBytes);
                System.Runtime.InteropServices.Marshal.Copy((IntPtr)Buffer, wbuf, wpos, AmountOfBytes);
                wpos += AmountOfBytes;
                if (wpos > wlen)
                    wlen = wpos;
                return true;
            }
            try
            {
                byte[] tmp = Scratch(AmountOfBytes);
                System.Runtime.InteropServices.Marshal.Copy((IntPtr)Buffer, tmp, 0, AmountOfBytes);
                pFile.Write(tmp, 0, AmountOfBytes);
                return true;
            }
            catch (Exception e)
            {
                writeFailed = true;
                MyConsole.WriteLine("BinaryFile: write failed (" + AmountOfBytes + " bytes): " + e.Message);
                return false;
            }
        }
    }
}
