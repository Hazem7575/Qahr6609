using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class PrestigeRanking
    {
        public enum Type : uint
        {
            World,
            Trojan,
            Warrior,
            Archer,
            Ninja,
            Monk,
            Pirate,
            DragonWarrior,
            WaterTao,
            FireTao,
            WindWalker,
            Thunderstriker,
            Count
        }

        public static Type GetIndex(byte Class)
        {
            if (Database.AtributesStatus.IsTrojan(Class))
                return Type.Trojan;
            if (Database.AtributesStatus.IsWarrior(Class))
                return Type.Warrior;
            if (Database.AtributesStatus.IsArcher(Class))
                return Type.Archer;
            if (Database.AtributesStatus.IsNinja(Class))
                return Type.Ninja;
            if (Database.AtributesStatus.IsMonk(Class))
                return Type.Monk;
            if (Database.AtributesStatus.IsPirate(Class))
                return Type.Pirate;
            if (Database.AtributesStatus.IsLee(Class))
                return Type.DragonWarrior;
            if (Database.AtributesStatus.IsWater(Class))
                return Type.WaterTao;
            if (Database.AtributesStatus.IsFire(Class))
                return Type.FireTao;
            if (Database.AtributesStatus.IsWindWalker(Class))
                return Type.WindWalker;
            return Type.WaterTao;
        }
        public class Entry
        {
            public uint Rank;
            public uint UID;
            public uint[] Points;
            public uint TotalPoints = 0;
            public uint Class;
            public byte Level;
            public Type type;
            public uint Mesh;
            public string Name = "";

            public uint HairStyle;
            public uint Head;
            public uint Garment;
            public uint LeftWeapon;
            public uint LefttWeaponAccessory;
            public uint RightWeapon;
            public uint RightWeaponAccessory;
            public uint MountArmor;
            public uint Armor;//??
            public uint Wing;
            public uint WingPlus;
            public uint Title;
            public uint Flag;//??
            public string GuildName = "";

            public void AddInfo(Client.GameClient user)
            {
                HairStyle = user.Player.Hair;
                Head = user.Equipment.HeadID;
                Garment = user.Player.GarmentId;
                LeftWeapon = user.Equipment.LeftWeapon;
                RightWeapon = user.Equipment.RightWeapon;
                RightWeaponAccessory = user.Player.RightWeaponAccessoryId;
                LefttWeaponAccessory = user.Player.LeftWeaponAccessoryId;
                MountArmor = user.Player.MountArmorId;
                Wing = user.Player.WingId;
                if (user.Player.SpecialWingID != 0)
                    Wing = user.Player.SpecialWingID;
                WingPlus = user.Player.WingPlus;
                Title = user.Player.SpecialTitleID;
                Armor = user.Player.ArmorId;
                if (user.Player.MyGuild != null)
                    GuildName = user.Player.MyGuild.GuildName;

                if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.WeeklyPKChampion))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.WeeklyPKChampion;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopTrojan))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopTrojan;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWarrior))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWarrior;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopArcher))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopArcher;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopNinja))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopNinja;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopMonk))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopMonk;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopPirate))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopPirate;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopDragonLee))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopDragonLee;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopFireTaoist))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopFireTaoist;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWaterTaoist))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWaterTaoist;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopDeputyLeader))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopDeputyLeader;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopGuildLeader))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopGuildLeader;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWindWalker))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWindWalker;

            }
            public void UpdateInfo(Client.GameClient user)
            {
                HairStyle = user.Player.Hair;
                Head = user.Equipment.HeadID;
                Garment = user.Player.GarmentId;
                LeftWeapon = user.Equipment.LeftWeapon;
                RightWeapon = user.Equipment.RightWeapon;
                RightWeaponAccessory = user.Player.RightWeaponAccessoryId;
                LefttWeaponAccessory = user.Player.LeftWeaponAccessoryId;
                MountArmor = user.Player.MountArmorId;
                Wing = user.Player.WingId;
                if (user.Player.SpecialWingID != 0)
                    Wing = user.Player.SpecialWingID;
                WingPlus = user.Player.WingPlus;
                Title = user.Player.SpecialTitleID;
                Armor = user.Player.ArmorId;
                if (user.Player.MyGuild != null)
                    GuildName = user.Player.MyGuild.GuildName;
            }
        }
        public static void Remove(uint UID)
        {
            foreach (var rank in PrestigeRanking.Ranks.Values)
            {
                rank.Remove(UID);
            }
        }
        public static Entry BestOfTheWorld
        {
            get
            {
                try
                {
                    return Ranks[Type.World].BestOfClass;
                }
                catch
                {
                    MyConsole.SaveException(new Exception("BestOfWorld Error Loading."));
                }
                return null;
            }
        }
        public static SafeDictionary<Type, Rank> Ranks = new SafeDictionary<Type, Rank>();
        public static object SynRoot = new object();


        public static void Create()
        {
            for (int x = 0; x < (byte)Type.Count; x++)
            {
                Ranks.Add((Type)x, new Rank((Type)x));
                if (x == (int)Type.World)
                {
                    Ranks[(Type)x].MaxItems = 100;
                }
                else Ranks[(Type)x].MaxItems = 30;
            }
        }

        public class Rank
        {
            public Entry BestOfClass;
            public Type _Type;
            public SafeDictionary<uint, Entry> Items = new SafeDictionary<uint, Entry>();

            public Rank(Type typ)
            {
                _Type = typ;
            }
            public ushort MaxItems = 0;
            // Guards Items/Members/BestOfClass updates of this rank (Members is replaced, never mutated).
            private readonly object SyncRoot = new object();
            public void Remove(uint UID)
            {
                lock (SyncRoot)
                {
                    var best = BestOfClass;
                    if (best != null && best.UID == UID)
                        BestOfClass = null;
                    if (Items.ContainsKey(UID))
                        Items.Remove(UID);
                    Members = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                    for (int x = 0; x < Members.Length; x++)
                        Members[x].Rank = (ushort)(x + 1);
                    BestOfClass = Members.FirstOrDefault();
                }
                Client.GameClient client;
                if (Server.GamePoll.TryGetValue(UID, out client) && client != null && client.Player != null && client.Player.UID == UID)
                    client.CreatePrestigePoints();
            }
            private Entry[] Members = new Entry[0];
            public Entry[] GetMember()
            {
                lock (SyncRoot)
                    return Members;
            }
            public void AddItem(Entry item)
            {
                if (item.TotalPoints < 20000) return;
                lock (SyncRoot)
                    AddItemLocked(item);
            }
            private void AddItemLocked(Entry item)
            {
                try
                {
                    if (Items.Count < MaxItems)
                    {
                        if (!Items.ContainsKey(item.UID))
                        {
                            Items.Add(item.UID, item);
                            lock (SynRoot)
                            {
                                Members = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                                for (int x = 0; x < Members.Length; x++)
                                    Members[x].Rank = (ushort)(x + 1);
                            }
                        }
                        else if (Items.ContainsKey(item.UID))
                        {
                            var points = Items[item.UID].TotalPoints;
                            Items[item.UID] = item;
                            if (points != item.TotalPoints)
                            {
                                lock (SynRoot)
                                {
                                    Members = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                                    for (int x = 0; x < Members.Length; x++)
                                        Members[x].Rank = (ushort)(x + 1);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Items.ContainsKey(item.UID))
                        {
                            uint points = Items[item.UID].TotalPoints;
                            uint rank = Items[item.UID].Rank;
                            Items[item.UID] = item;
                            item.Rank = rank;

                            if (points != item.TotalPoints)
                            {
                                lock (SynRoot)
                                {
                                    Members = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                                    for (int x = 0; x < Members.Length; x++)
                                        Members[x].Rank = (ushort)(x + 1);
                                }
                            }
                        }
                        else
                        {
                            var last = Members[Members.Length - 1];
                            if (item.TotalPoints > last.TotalPoints)
                            {
                                Items.Remove(last.UID);
                                Items.Add(item.UID, item);

                                lock (SynRoot)
                                {
                                    Members = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                                    for (int x = 0; x < Members.Length; x++)
                                        Members[x].Rank = (ushort)(x + 1);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    try
                    {
                        BestOfClass = Members.FirstOrDefault();
                    }
                    catch
                    {
                        MyConsole.SaveException(new Exception("Prestige Rank Type " + _Type + "is empty."));
                    }
                }
            }


        }

        public static void CheckReborn(Client.GameClient user)
        {
            if (Ranks[GetIndex(user.Player.SecoundeClass)].Items.ContainsKey(user.Player.UID)
                && !IsCurrentRank(user, user.Player.SecoundeClass))
            {
                Ranks[GetIndex(user.Player.SecoundeClass)].Remove(user.Player.UID);
            }
            if (user.Player.FirstClass != user.Player.Class)
            {
                if (Ranks[GetIndex(user.Player.FirstClass)].Items.ContainsKey(user.Player.UID)
                    && !IsCurrentRank(user, user.Player.FirstClass))
                    Ranks[GetIndex(user.Player.FirstClass)].Remove(user.Player.UID);
            }
        }
        /// <summary>
        /// True when a previous class maps to the rank the player is listed in right now. Removing the
        /// entry there would only be undone by the immediate re-add in CreatePrestigePoints (and cost two
        /// full prestige recomputes + re-sorts on every gear change), so it is kept and updated instead.
        /// </summary>
        private static bool IsCurrentRank(Client.GameClient user, byte oldClass)
        {
            var cls = user.Player.Class;
            if (cls >= 100 && cls <= 102)//simple taoist: never re-added, keep the original removal
                return false;
            return GetIndex(oldClass) == GetIndex(cls);
        }

        public static uint GetMyRank(Type typ, uint UID)
        {
            Entry item;
            if (Ranks[typ].Items.TryGetValue(UID, out item))
            {
                return item.Rank;
            }
            return 0;
        }

        public static Entry GetInfo(uint UID)
        {
            foreach (var rank in Ranks.Values)
            {
                foreach (var item in rank.Items.GetValues())
                    if (item.UID == UID)
                        return item;
            }
            return null;
        }



        public static void Load()
        {
            Create();
            using (DBActions.Read reader = new DBActions.Read("PrestigeRanking.txt", false))
            {
                if (reader.Reader())
                {
                    int count = reader.Count;
                    for (int x = 0; x < count; x++)
                    {
                        DBActions.ReadLine line = new DBActions.ReadLine(reader.ReadString("/"), '/');
                        Entry item = new Entry();
                        item.UID = line.Read((uint)0);
                        item.Name = line.Read("");
                        item.type = (Type)line.Read((byte)0);

                        item.Class = line.Read((uint)0);
                        item.Level = line.Read((byte)0);
                        item.Mesh = line.Read((uint)0);
                        item.TotalPoints = line.Read((uint)0);
                        item.HairStyle = line.Read((uint)0);
                        item.Head = line.Read((uint)0);
                        item.Garment = line.Read((uint)0);
                        item.LeftWeapon = line.Read((uint)0);
                        item.LefttWeaponAccessory = line.Read((uint)0);
                        item.RightWeapon = line.Read((uint)0);
                        item.RightWeaponAccessory = line.Read((uint)0);
                        item.MountArmor = line.Read((uint)0);
                        item.Armor = line.Read((uint)0);
                        item.Wing = line.Read((uint)0);
                        item.WingPlus = line.Read((uint)0);
                        item.Title = line.Read((uint)0);
                        item.Flag = line.Read((uint)0);
                        item.GuildName = line.Read("");
                        int pointscount = line.Read((int)0);
                        item.Points = new uint[pointscount];
                        for (int i = 0; i < item.Points.Length; i++)
                        {
                            item.Points[i] = line.Read((uint)0);

                        }
                         //if (!item.Name.Contains("[PM]") && !item.Name.Contains("[GM]"))
                        {
                            Ranks[item.type].AddItem(item);
                            Ranks[Type.World].AddItem(item);
                        }
                    }
                }
            }
        }
        public static void Save()
        {
            using (DBActions.Write writer = new DBActions.Write("PrestigeRanking.txt"))
            {
                foreach (var rank in Ranks.GetValues())
                {
                    if (rank._Type == Type.World)
                        continue;
                    foreach (var obj in rank.Items.GetValues())
                    {
                        Database.DBActions.WriteLine line = new DBActions.WriteLine('/');
                        line.Add(obj.UID).Add(obj.Name).Add((byte)obj.type).Add(obj.Class)
                            .Add(obj.Level).Add(obj.Mesh).Add(obj.TotalPoints)
                            .Add(obj.HairStyle)
                            .Add(obj.Head)
                            .Add(obj.Garment)
                            .Add(obj.LeftWeapon)
                            .Add(obj.LefttWeaponAccessory)
                            .Add(obj.RightWeapon)
                            .Add(obj.RightWeaponAccessory)
                            .Add(obj.MountArmor)
                            .Add(obj.Armor)
                            .Add(obj.Wing)
                            .Add(obj.WingPlus)
                            .Add(obj.Title)
                            .Add(obj.Flag)
                        .Add(obj.GuildName);

                        if (obj.Points == null)
                            line.Add(0);
                        else
                        {
                            line.Add(obj.Points.Length);
                            for (int x = 0; x < obj.Points.Length; x++)
                                line.Add(obj.Points[x]);
                        }
                        writer.Add(line.Close());
                    }
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }
    }
}
