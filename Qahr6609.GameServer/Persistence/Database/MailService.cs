using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Database
{
    /// <summary>
    /// Player mailbox (inbox) storage and sending. Each player's mails are kept in PlayersMail\&lt;UID&gt;.bin
    /// (int count + count ClientMail.DBPrize). The in-memory mailbox of a loaded player always mirrors the file:
    /// every change (new mail, claim, delete) is written right away, so mails survive relogs and restarts and a
    /// claimed mail can never come back. Mail can be sent to online and offline players.
    /// </summary>
    public static unsafe class MailService
    {
        /// <summary>Most mails one mailbox holds; sending to a full mailbox fails.</summary>
        public const int MaxMails = 100;
        public const int SenderMax = 31, SubjectMax = 31, MessageMax = 63;

        static readonly ConcurrentDictionary<uint, object> Locks = new ConcurrentDictionary<uint, object>();
        static readonly ConcurrentDictionary<uint, uint> LastIds = new ConcurrentDictionary<uint, uint>();
        // UID -> mailbox of the last session that loaded it (mirrors the file).
        static readonly ConcurrentDictionary<uint, ConcurrentDictionary<uint, PrizeInfo>> Boxes = new ConcurrentDictionary<uint, ConcurrentDictionary<uint, PrizeInfo>>();

        static object LockOf(uint uid)
        {
            return Locks.GetOrAdd(uid, u => new object());
        }
        static string PathOf(uint uid)
        {
            return Program.ServerConfig.DbLocation + "\\PlayersMail\\" + uid + ".bin";
        }

        /// <summary>Loads the player's mailbox at login (called from ServerDatabase.LoadCharacter).</summary>
        public static void Load(Client.GameClient client)
        {
            uint uid = client.Player.UID;
            lock (LockOf(uid))
            {
                client.MailBox.Clear();
                List<PrizeInfo> list;
                if (!TryRead(uid, out list))
                    return;//unreadable right now: keep it unloaded so nothing overwrites the file
                foreach (var prize in list)
                    client.MailBox.TryAdd(prize.ID, prize);
                Boxes[uid] = client.MailBox;
            }
        }

        /// <summary>Writes the player's mailbox after it changed (claim / delete).</summary>
        public static void Save(Client.GameClient client)
        {
            uint uid = client.Player.UID;
            lock (LockOf(uid))
            {
                ConcurrentDictionary<uint, PrizeInfo> box;
                // only the session that loaded the mailbox may write it
                if (!Boxes.TryGetValue(uid, out box) || !object.ReferenceEquals(box, client.MailBox))
                    return;
                Write(uid, box.Values.ToList());
            }
        }

        static string Clip(string text, int max)
        {
            if (text == null)
                return "";
            return text.Length > max ? text.Substring(0, max) : text;
        }

        /// <summary>
        /// Sends a mail to the player with this UID (online or offline). money = silver, cps = Conquer Points,
        /// itemId = item type id given when the attachment is claimed (0 = none), item = a full item (optional).
        /// </summary>
        public static bool Send(uint uid, string sender, string subject, string text, ulong money, uint cps, uint itemId, MsgGameItem item, out string error)
        {
            error = null;
            if (itemId != 0 && !Server.ItemsBase.ContainsKey(itemId))
            {
                error = "Unknown item " + itemId + ".";
                return false;
            }
            PrizeInfo prize = new PrizeInfo();
            prize.Sender = Clip(sender, SenderMax);
            prize.Subject = Clip(subject, SubjectMax);
            prize.Message = Clip(text, MessageMax);
            prize.goldprize = money;
            prize.cpsprize = cps;
            prize.Attackment = itemId;
            prize.Item = item;
            prize.SentOn = DateTime.Now;

            ConcurrentDictionary<uint, PrizeInfo> box;
            lock (LockOf(uid))
            {
                List<PrizeInfo> list;
                if (Boxes.TryGetValue(uid, out box))
                    list = box.Values.ToList();
                else if (!TryRead(uid, out list))
                {
                    error = "The mailbox of " + uid + " cannot be read right now.";
                    return false;
                }
                if (list.Count >= MaxMails)
                {
                    error = "The mailbox is full.";
                    return false;
                }
                // ids only go up (also past mails that are briefly out of the box during a claim)
                uint top = list.Count == 0 ? 0 : list.Max(p => p.ID);
                uint last;
                LastIds.TryGetValue(uid, out last);
                prize.ID = Math.Max(top, last) + 1;
                LastIds[uid] = prize.ID;
                list.Add(prize);
                if (!Write(uid, list))
                {
                    error = "The mail could not be saved.";
                    return false;
                }
                if (box != null)
                    box.TryAdd(prize.ID, prize);
            }
            Client.GameClient online;
            if (box != null && Server.GamePoll.TryGetValue(uid, out online) && object.ReferenceEquals(online.MailBox, box))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    online.Send(stream.CreatMailNotify(3));//new-mail icon
                }
            }
            return true;
        }

        /// <summary>Finds a character (online first, then the saved characters) by name, ignoring case.</summary>
        public static bool FindCharacter(string name, out uint uid, out string realName)
        {
            uid = 0;
            realName = null;
            if (string.IsNullOrEmpty(name))
                return false;
            foreach (var user in Server.GamePoll.Values)
            {
                if (user.Player != null && string.Equals(user.Player.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    uid = user.Player.UID;
                    realName = user.Player.Name;
                    return true;
                }
            }
            string dir = Program.ServerConfig.DbLocation + "\\Users\\";
            if (!Directory.Exists(dir))
                return false;
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in Directory.GetFiles(dir, "*.ini"))
            {
                uint fileUid;
                if (!uint.TryParse(Path.GetFileNameWithoutExtension(fname), out fileUid))
                    continue;
                ini.FileName = fname;
                string n = ini.ReadString("Character", "Name", "");
                if (string.Equals(n, name, StringComparison.OrdinalIgnoreCase))
                {
                    uid = fileUid;
                    realName = n;
                    return true;
                }
            }
            return false;
        }

        // Reads the mail file. A missing file is an empty mailbox; a damaged one is moved aside and the mailbox
        // starts empty. Returns false only when the file exists but cannot be read now.
        static bool TryRead(uint uid, out List<PrizeInfo> list)
        {
            list = new List<PrizeInfo>();
            string path = PathOf(uid);
            if (!File.Exists(path))
                return true;
            byte[] data;
            try
            {
                data = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mail] cannot read " + path + ": " + e.Message);
                return false;
            }
            int size = sizeof(ClientMail.DBPrize);
            int count = data.Length >= 4 ? BitConverter.ToInt32(data, 0) : -1;
            if (count < 0 || 4 + (long)count * size != data.Length)
            {
                try
                {
                    File.Move(path, path + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[Mail] cannot move aside " + path + ": " + e.Message);
                    return false;
                }
                MyConsole.WriteLine("[Mail] damaged mailbox " + path + " moved aside.");
                return true;
            }
            fixed (byte* ptr = data)
            {
                for (int x = 0; x < count; x++)
                {
                    ClientMail.DBPrize db = *(ClientMail.DBPrize*)(ptr + 4 + x * size);
                    var prize = db.GetClientPrize();
                    if (prize.ID != 0)
                        list.Add(prize);
                }
            }
            return true;
        }

        static bool Write(uint uid, List<PrizeInfo> prizes)
        {
            string path = PathOf(uid);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var valid = prizes.Where(p => p.ID != 0).ToList();
                int size = sizeof(ClientMail.DBPrize);
                byte[] data = new byte[4 + valid.Count * size];
                BitConverter.GetBytes(valid.Count).CopyTo(data, 0);
                fixed (byte* ptr = data)
                {
                    for (int x = 0; x < valid.Count; x++)
                    {
                        ClientMail.DBPrize db = new ClientMail.DBPrize();
                        db.GetDBPrize(valid[x]);
                        *(ClientMail.DBPrize*)(ptr + 4 + x * size) = db;
                    }
                }
                string tmp = path + ".tmp";
                File.WriteAllBytes(tmp, data);
                if (File.Exists(path))
                    File.Replace(tmp, path, null);
                else
                    File.Move(tmp, path);
                return true;
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mail] cannot save " + path + ": " + e.Message);
                return false;
            }
        }
    }
}
