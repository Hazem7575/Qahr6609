using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
    public class Flowers : IEnumerable<Flowers.Flower>
    {
        public class FlowersRankingToday
        {
            public const int File_Size = 30;
            public Dictionary<uint, uint> Flowers;
            // All reads/writes of Flowers go through this lock (the dictionary is rebuilt in place).
            private readonly object SyncRoot = new object();

            public FlowersRankingToday()
            {
                Flowers = new Dictionary<uint, uint>();
            }
            public void UpdateRank(uint UID, uint Amount)
            {
                lock (SyncRoot)
                {
                    if (Flowers.Count < File_Size)
                    {
                        Calculate(UID, Amount);
                    }
                    else
                    {
                        var array = Flowers.Values.ToArray();
                        if (array[File_Size - 1] <= Amount)
                        {
                            Calculate(UID, Amount);
                        }
                        else
                        {
                            if (Flowers.ContainsKey(UID))
                                Calculate(UID, Amount);
                        }
                    }
                }
            }
            /// <summary>1-based position of UID in today's ranking (insertion order = sorted), or false.</summary>
            public bool TryGetRank(uint UID, out int Rank, out uint Amount)
            {
                lock (SyncRoot)
                {
                    Rank = 1;
                    foreach (var FToday in Flowers)
                    {
                        if (FToday.Key == UID)
                        {
                            Amount = FToday.Value;
                            return true;
                        }
                        Rank++;
                    }
                }
                Rank = 0;
                Amount = 0;
                return false;
            }
            public void Calculate(uint UID, uint Amount)
            {
                lock (SyncRoot)
                {
                    if (!Flowers.ContainsKey(UID))
                        Flowers.Add(UID, Amount);
                    var clients = Flowers.Values.ToArray();
                    var array = Flowers.OrderByDescending(p => p.Value).ToArray();
                    int Rank = 1;
                    Flowers.Clear();
                    foreach (var user_power in array)
                    {
                        if (Rank <= File_Size && Flowers.Count < File_Size)
                        {
                            if (!Flowers.ContainsKey(user_power.Key))
                                Flowers.Add(user_power.Key, user_power.Value);
                        }
                        else
                            break;
                        Rank++;
                    }
                }
            }
        }
        public class FlowerRanking
        {
            public const int File_Size = 100;

            public bool Girls = true;

            // Guards the four ranking dictionaries (they are rebuilt in place on every update).
            public readonly object SynRoot;
            public Dictionary<uint, Flower> RedRoses;
            public Dictionary<uint, Flower> Lilies;
            public Dictionary<uint, Flower> Orchids;
            public Dictionary<uint, Flower> Tulips;

            public FlowerRanking(bool _Girls = true)
            {
                Girls = _Girls;
                SynRoot = new object();
                RedRoses = new Dictionary<uint, Flower>(File_Size);
                Lilies = new Dictionary<uint, Flower>(File_Size);
                Orchids = new Dictionary<uint, Flower>(File_Size);
                Tulips = new Dictionary<uint, Flower>(File_Size);

            }

            /// <summary>Thread-safe copy of one ranking list (in rank order).</summary>
            public Flower[] Snapshot(Game.MsgServer.MsgFlower.FlowersType Typ)
            {
                lock (SynRoot)
                {
                    switch (Typ)
                    {
                        case Game.MsgServer.MsgFlower.FlowersType.Rouse: return RedRoses.Values.ToArray();
                        case Game.MsgServer.MsgFlower.FlowersType.Lilies: return Lilies.Values.ToArray();
                        case Game.MsgServer.MsgFlower.FlowersType.Orchids: return Orchids.Values.ToArray();
                        case Game.MsgServer.MsgFlower.FlowersType.Tulips: return Tulips.Values.ToArray();
                    }
                }
                return null;
            }

            public void UpdateRank(Flower AllFlower, Game.MsgServer.MsgFlower.FlowersType Typ)
            {
                List<uint> changed = null;
                lock (SynRoot)
                {
                    switch (Typ)
                    {
                        case Game.MsgServer.MsgFlower.FlowersType.Rouse:
                            CreateRank(RedRoses, AllFlower, ref changed);
                            break;
                        case Game.MsgServer.MsgFlower.FlowersType.Lilies:
                            CreateRank(Lilies, AllFlower, ref changed);
                            break;
                        case Game.MsgServer.MsgFlower.FlowersType.Orchids:
                            CreateRank(Orchids, AllFlower, ref changed);
                            break;
                        case Game.MsgServer.MsgFlower.FlowersType.Tulips:
                            CreateRank(Tulips, AllFlower, ref changed);
                            break;
                    }
                }
                // Notify outside the lock: once per player whose rank changed (plus the receiver,
                // whose amount changed), instead of N times to the receiver only.
                if (changed != null && Girls)
                {
                    if (!changed.Contains(AllFlower.UID))
                        changed.Add(AllFlower.UID);
                    foreach (var uid in changed)
                        UpdateRank(uid, AllFlower);
                }
            }
            private void CreateRank(Dictionary<uint, Flower> AllRank, Flower AllFlower, ref List<uint> changed)
            {
                if (AllRank.Count < File_Size)
                {
                    Calculate(AllRank, AllFlower, ref changed);
                }
                else
                {
                    var array = AllRank.Values.ToArray();
                    if (array[99].Amount <= AllFlower.Amount)
                    {
                        Calculate(AllRank, AllFlower, ref changed);
                    }
                    else
                    {
                        if (AllRank.ContainsKey(AllFlower.UID))
                            Calculate(AllRank, AllFlower, ref changed);
                    }
                }
            }
            private void Calculate(Dictionary<uint, Flower> FlowerRank, Flower Flower, ref List<uint> changed)
            {
                // Caller holds SynRoot.
                if (!FlowerRank.ContainsKey(Flower.UID))
                    FlowerRank.Add(Flower.UID, Flower);
                var clients = FlowerRank.Values.ToArray();
                var array = clients.OrderByDescending(p => p.Amount).ToArray();
                int Rank = 1;
                FlowerRank.Clear();
                foreach (var user_power in array)
                {
                    int OldRank = user_power.Rank;
                    user_power.Rank = Rank;
                    if (user_power.UID == Flower.UID)
                        Flower.Rank = Rank;
                    if (Rank <= File_Size && FlowerRank.Count < File_Size)
                    {
                        if (!FlowerRank.ContainsKey(user_power.UID))
                            FlowerRank.Add(user_power.UID, user_power);
                    }
                    else
                    {
                        Rank = 0;
                        // Only the entry that fell off the list loses its rank (was: the receiver too).
                        if (user_power.UID == Flower.UID)
                            Flower.Rank = 0;
                        user_power.Rank = 0;
                    }
                    if (OldRank != user_power.Rank)
                    {
                        if (changed == null)
                            changed = new List<uint>(4);
                        changed.Add(user_power.UID); // each UID appears once in the list
                    }
                    Rank++;
                }
            }
            public unsafe void UpdateRank(uint UID, Flower Flower)
            {
                Client.GameClient user;
                if (Database.Server.GamePoll.TryGetValue(UID, out user) && user.Player != null && user.Player.Flowers != null)
                {
                    user.Player.Flowers.UpdateMyRank(user, true);
                }
            }
        }

        public class Flower
        {
            public Game.MsgServer.MsgFlower.FlowersType Type;
            public uint Amount;
            public uint Amount2day;

            public int Rank;
            public string Name;
            public uint UID;

            public Flower(Game.MsgServer.MsgFlower.FlowersType typ, uint _uid, string _name)
            {
                UID = _uid;
                Name = _name;
                Type = typ;
            }

            public byte GetGrade()
            {
                switch (Type)
                {
                    case Game.MsgServer.MsgFlower.FlowersType.Rouse: return 1;
                    case Game.MsgServer.MsgFlower.FlowersType.Tulips: return 2;
                    case Game.MsgServer.MsgFlower.FlowersType.Orchids: return 3;
                    case Game.MsgServer.MsgFlower.FlowersType.Lilies: return 4;
                }
                return 0;
            }
            public static implicit operator uint(Flower Data)
            {
                return Data.Amount;
            }
            public static Flower operator +(Flower Data, uint amount)
            {
                Data.Amount += amount;
                Data.Amount2day += amount;
                return Data;
            }
        }

        public uint FreeFlowers;
        public int Day;

        public Flower RedRoses;
        public Flower Lilies;
        public Flower Orchids;
        public Flower Tulips;

        public uint CreateFlowerIcon(Flower flow, bool Today = false, byte Rank = 0)
        {
            uint ID = (uint)(Today ? 30000001 : 30000002);
            if (flow == null)
                return (uint)(ID + GetRank(Rank));

            if (flow.Rank == 0 || flow.Rank > 100)
                return 0;
            // ret = (int)(30000002 + (uint)(100 * (byte)typ) + 1000 * (uint)(rak / 2));

            return (uint)(ID + (uint)(100 * (byte)flow.Type) + GetRank(flow.Rank));
        }
        public uint CreateBoyIcon(Flower flow)
        {
            return (uint)(30000402 + (uint)(100 * (byte)flow.Type) + GetRank(flow.Rank));
        }
        public ushort GetRank(int rank)
        {
            if (rank == 1)
                return 0;
            if (rank == 2)
                return 10000;
            if (rank == 3)
                return 20000;
            if (rank > 3)
                return 30000;

            return 0;
        }

        public static System.Collections.Concurrent.ConcurrentDictionary<uint, Flowers> ClientPoll = new System.Collections.Concurrent.ConcurrentDictionary<uint, Flowers>();

        public Flowers(uint UID, string Name)
        {
            FreeFlowers = 1;
            RedRoses = new Flower(Game.MsgServer.MsgFlower.FlowersType.Rouse, UID,Name);
            Lilies = new Flower(Game.MsgServer.MsgFlower.FlowersType.Lilies, UID, Name);
            Orchids = new Flower(Game.MsgServer.MsgFlower.FlowersType.Orchids, UID, Name);
            Tulips = new Flower(Game.MsgServer.MsgFlower.FlowersType.Tulips, UID, Name);
        }
        public uint AllFlowersToday()
        {
            return (uint)this.Sum((x) => x.Amount2day);
        }
        public unsafe void UpdateMyRank(Client.GameClient user, bool ReloadScreen = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {

                var stream = rec.GetStream();
                uint OldIcon = user.Player.FlowerRank;

                stream.GenericRankingCreate(MsgGenericRanking.Action.InformationRequest, MsgGenericRanking.RankType.None, 0, 0, 1);
                user.Send(stream.GenericRankingFinalize());

               


                var array = this.Where((f1) => f1.Rank >= 1 && f1.Rank <= 100).ToArray();
                Array.Sort(array, (f1, f2) =>
                {
                    int n_rank = f1.Rank.CompareTo(f2.Rank);
                    if (f2.Rank == f1.Rank)
                        return f2.GetGrade().CompareTo(f1.GetGrade());
                    return n_rank;
                });

                if (array.Length > 0)
                {
                    user.Player.FlowerRank = CreateFlowerIcon(array[0]);
                    foreach (var Flowers in array)
                    {
                        stream.GenericRankingCreate(MsgGenericRanking.Action.QueryCount, (Game.MsgServer.MsgGenericRanking.RankType)user.Player.Flowers.CreateFlowerIcon(Flowers), 0, 0, 1);

                        stream.AddItemGenericRankingCreate(Flowers.Rank, Flowers.Amount, Flowers.UID, Flowers.Name);

                        user.Send(stream.GenericRankingFinalize());
                    }
                }
                else
                {
                    int Rank;
                    uint TodayAmount;
                    if (Program.FlowersRankToday.TryGetRank(user.Player.UID, out Rank, out TodayAmount))
                    {
                        user.Player.FlowerRank = CreateFlowerIcon(null, true, (byte)Rank);

                        stream.GenericRankingCreate(MsgGenericRanking.Action.QueryCount, (Game.MsgServer.MsgGenericRanking.RankType)CreateFlowerIcon(null, true, (byte)Rank), 0, 0, 1);
                        stream.AddItemGenericRankingCreate(Rank, TodayAmount, user.Player.UID, user.Player.Name);


                        user.Send(stream.GenericRankingFinalize());
                    }
                }
                stream.GenericRankingCreate(MsgGenericRanking.Action.InformationRequest, MsgGenericRanking.RankType.None, 0, 0, 0);
                stream.AddItemGenericRankingCreate(0, 0, 0, "");
                user.Send(stream.GenericRankingFinalize());

                // Only re-spawn to the screen when the visible flower icon actually changed.
                if (ReloadScreen && user.Player.FlowerRank != OldIcon)
                    user.Player.View.SendView(user.Player.GetArray(stream,false), false);
            }
        }

        public IEnumerator<Flower> GetEnumerator()
        {
            yield return RedRoses;
            yield return Lilies;
            yield return Orchids;
            yield return Tulips;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public override string ToString()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(FreeFlowers);
            foreach (var Flow in this)
            {
                writer.Add(Flow.Amount).Add(Flow.Amount2day);
            }
            return writer.Close();
        }
    }
}