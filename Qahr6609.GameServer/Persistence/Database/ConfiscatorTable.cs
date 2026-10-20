using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;

namespace Conquer6609.Database
{
    public class ConfiscatorTable
    {
        public ConcurrentDictionary<uint, Role.Instance.Confiscator> PollContainers = new ConcurrentDictionary<uint, Role.Instance.Confiscator>();


        public void QueueObj(uint UID, Role.Instance.Confiscator conainter)
        {
            if (!PollContainers.ContainsKey(UID))
                PollContainers.TryAdd(UID, conainter);
        }
        private int GetRedeemItemsCount()
        {
            return PollContainers.Values.Sum(p => p.RedeemContainer.Count);
        }
        private int GetClaimItemsCount()
        {
            return PollContainers.Values.Sum(p => p.ClaimContainer.Count);
        }
        internal unsafe void Save()
        {
            // Count and records come from the same snapshot; files are replaced atomically (see BinaryFile).
            var containers = PollContainers.Values.ToArray();
            var redeem = containers.SelectMany(p => p.RedeemContainer.Values.ToArray()).ToArray();
            using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
            {
                if (binary.Open(Program.ServerConfig.DbLocation + "\\RedeemContainer.bin", FileMode.Create))
                {
                    int ItemCount = redeem.Length;
                    binary.Write(&ItemCount, sizeof(int));
                    foreach (var item in redeem)
                    {
                        Game.MsgServer.MsgDetainedItem packet = item;
                        binary.Write(&packet, sizeof(Game.MsgServer.MsgDetainedItem));
                    }
                    binary.Close();
                }
            }
            var claim = containers.SelectMany(p => p.ClaimContainer.Values.ToArray()).ToArray();
            using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
            {
                if (binary.Open(Program.ServerConfig.DbLocation + "\\ClaimContainer.bin", FileMode.Create))
                {
                    int ItemCount = claim.Length;
                    binary.Write(&ItemCount, sizeof(int));
                    foreach (var item in claim)
                    {
                        Game.MsgServer.MsgDetainedItem packet = item;
                        binary.Write(&packet, sizeof(Game.MsgServer.MsgDetainedItem));
                    }
                    binary.Close();
                }
            }
        }
        /// <summary>Picks the container file to load: the file, or its .bak when the file is damaged.</summary>
        unsafe string PickContainerFile(string path)
        {
            int size = sizeof(Game.MsgServer.MsgDetainedItem);
            try
            {
                return ServerDatabase.PickLoadPath(path, p => ServerDatabase.ValidateCountedFile(p, size), 0);
            }
            catch (InvalidDataException)
            {
                MyConsole.WriteLine("WARNING: " + path + " and its backup are damaged, detained items not loaded.");
                try
                {
                    // keep the damaged data: the next save would otherwise rotate it away
                    if (File.Exists(path))
                        File.Copy(path, path + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss"), true);
                }
                catch (Exception e) { MyConsole.WriteLine(e.Message); }
                return null;
            }
        }

        internal unsafe void Load()
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            string redeemPath = PickContainerFile(Program.ServerConfig.DbLocation + "\\RedeemContainer.bin");
            if (redeemPath != null && binary.Open(redeemPath, FileMode.Open))
            {
                Game.MsgServer.MsgDetainedItem Item;
                int ItemCount;
                binary.Read(&ItemCount, sizeof(int));
                for (int x = 0; x < ItemCount; x++)
                {
                    if (!binary.Read(&Item, sizeof(Game.MsgServer.MsgDetainedItem)))
                        break;// file shorter than its count (damaged): stop instead of using a half-read record
                    if (Item.UID >= Role.Instance.Confiscator.CounterUID.Count)
                        Role.Instance.Confiscator.CounterUID.Set(Item.UID + 1);

                    if (!PollContainers.ContainsKey(Item.GainerUID))
                        PollContainers.TryAdd(Item.GainerUID, new Role.Instance.Confiscator());
                    if (!PollContainers.ContainsKey(Item.OwnerUID))
                        PollContainers.TryAdd(Item.OwnerUID, new Role.Instance.Confiscator());

                    Item.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(Item.Date).Ticks).Days);
                    if (Item.Action == Game.MsgServer.MsgDetainedItem.ContainerType.RewardCps)
                    {
                        //Item.Action = Game.MsgServer.MsgDetainedItem.ContainerType.RewardCps;
                        PollContainers[Item.GainerUID].ClaimContainer.TryAdd(Item.UID, Item);
                        continue;
                    }
                    if (Item.DaysLeft > 7)
                    {
                       // Item.Action = Game.MsgServer.MsgDetainedItem.ContainerType.RewardCps;
                        PollContainers[Item.GainerUID].ClaimContainer.TryAdd(Item.UID, Item);
                        continue;
                    }
                    else
                    {
                        PollContainers[Item.GainerUID].ClaimContainer.TryAdd(Item.UID, Item);
                        PollContainers[Item.OwnerUID].RedeemContainer.TryAdd(Item.UID, Item);
                    }
                }
                binary.Close();
            }
            binary = new WindowsAPI.BinaryFile();
            string claimPath = PickContainerFile(Program.ServerConfig.DbLocation + "\\ClaimContainer.bin");
            if (claimPath != null && binary.Open(claimPath, FileMode.Open))
            {
                Game.MsgServer.MsgDetainedItem Item;
                int ItemCount;
                binary.Read(&ItemCount, sizeof(int));
                for (int x = 0; x < ItemCount; x++)
                {
                    if (!binary.Read(&Item, sizeof(Game.MsgServer.MsgDetainedItem)))
                        break;// file shorter than its count (damaged): stop instead of using a half-read record

                    if (Item.UID >= Role.Instance.Confiscator.CounterUID.Count)
                        Role.Instance.Confiscator.CounterUID.Set(Item.UID + 1);

                    if (!PollContainers.ContainsKey(Item.GainerUID))
                        PollContainers.TryAdd(Item.GainerUID, new Role.Instance.Confiscator());
                    if (!PollContainers.ContainsKey(Item.OwnerUID))
                        PollContainers.TryAdd(Item.OwnerUID, new Role.Instance.Confiscator());

                    Item.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(Item.Date).Ticks).Days);
                  /* if (Item.DaysLeft > 7)
                    {
                        Item.Action = Game.MsgServer.MsgDetainedItem.ContainerType.RewardCps;
                    }*/

                    PollContainers[Item.GainerUID].ClaimContainer.TryAdd(Item.UID, Item);
                }
                binary.Close();
            }
        }
    }
}
