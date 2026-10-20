using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{

    public class ChiRank
    {
        public const int File_Size = 50;

        public Dictionary<uint, Chi.ChiPower> Dragon;
        public Dictionary<uint, Chi.ChiPower> Phoenix;
        public Dictionary<uint, Chi.ChiPower> Turtle;
        public Dictionary<uint, Chi.ChiPower> Tiger;

        public ChiRank()
        {
            Dragon = new Dictionary<uint, Chi.ChiPower>(File_Size);
            Phoenix = new Dictionary<uint, Chi.ChiPower>(File_Size);
            Turtle = new Dictionary<uint, Chi.ChiPower>(File_Size);
            Tiger = new Dictionary<uint, Chi.ChiPower>(File_Size);
        }
        public void Upadte(Dictionary<uint, Chi.ChiPower> power, Chi.ChiPower MyPower)
        {
            if (power.Count < File_Size)
            {
                Calculate(power, MyPower);
            }
            else
            {
                var array = power.Values.ToArray();
                if (array[49].Score <= MyPower.Score)
                {
                    Calculate(power, MyPower);
                }
                else
                {
                    if (power.ContainsKey(MyPower.UID))
                        Calculate(power, MyPower);
                }
            }
        }
        public void Calculate(Dictionary<uint, Chi.ChiPower> power, Chi.ChiPower MyPower)
        {
            lock (power)
            {
                if (power.ContainsKey(MyPower.UID))
                    power.Remove(MyPower.UID);
                power.Add(MyPower.UID, MyPower);
                var clients = power.Values.ToArray();
                var array = clients.OrderByDescending(p => p.Score).ToArray();
                int Rank = 1;
                power.Clear();
                foreach (var user_power in array)
                {
                    int OldRank = user_power.Rank;
                    user_power.Rank = Rank;
                    if (user_power.UID == MyPower.UID)
                        MyPower.Rank = Rank;
                    if (Rank <= File_Size)
                    {
                        if (!power.ContainsKey(user_power.UID))
                            power.Add(user_power.UID, user_power);
                    }
                    if (OldRank != user_power.Rank)
                    {
                        SendUpdate(user_power.UID, user_power);
                    }
                    Rank++;
                }
            }
        }
        public unsafe void SendUpdate(uint UID, Chi.ChiPower MyPower)
        {
            Client.GameClient user;
            if (Database.Server.GamePoll.TryGetValue(UID, out user))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.GenericRankingCreate(MsgGenericRanking.Action.QueryCount, (Game.MsgServer.MsgGenericRanking.RankType)(60000000 + (byte)MyPower.Type), 0, 0, 1);
                    stream.AddItemGenericRankingCreate((int)MyPower.Rank, (uint)MyPower.Score, MyPower.UID, MyPower.Name);
                    user.Send(stream.GenericRankingFinalize());
                }
                user.Equipment.QueryEquipment(user.Equipment.Alternante, false);
            }
        }
    }
    public class Chi : IEnumerable<Chi.ChiPower>
    {
        [Flags]
        public enum ChiAttributeFlags
        {
            None = 0,

            CriticalStrike = 1 << (ChiAttributeType.CriticalStrike - 1),
            SkillCriticalStrike = 1 << (ChiAttributeType.SkillCriticalStrike - 1),
            Immunity = 1 << (ChiAttributeType.Immunity - 1),
            Breakthrough = 1 << (ChiAttributeType.Breakthrough - 1),
            Counteraction = 1 << (ChiAttributeType.Counteraction - 1),
            MaxLife = 1 << (ChiAttributeType.MaxLife - 1),
            AddAttack = 1 << (ChiAttributeType.AddAttack - 1),
            AddMagicAttack = 1 << (ChiAttributeType.AddMagicAttack - 1),
            AddMagicDefense = 1 << (ChiAttributeType.AddMagicDefense - 1),
            FinalAttack = 1 << (ChiAttributeType.FinalAttack - 1),
            FinalMagicAttack = 1 << (ChiAttributeType.FinalMagicAttack - 1),
            FinalDefense = 1 << (ChiAttributeType.FinalDefense - 1),
            FinalMagicDefense = 1 << (ChiAttributeType.FinalMagicDefense - 1)
        }
      
        public enum ChiAttributeType
        {
            None = 0,
            CriticalStrike,
            SkillCriticalStrike,
            Immunity,
            Breakthrough,
            Counteraction,
            MaxLife,
            AddAttack,
            AddMagicAttack,
            AddMagicDefense,
            FinalAttack,
            FinalMagicAttack,
            FinalDefense,
            FinalMagicDefense
        }
        public class ChiPower
        {
            public static readonly Extensions.RandomLite Rand = new Extensions.RandomLite();
            public Game.MsgServer.MsgChiInfo.ChiPowerType Type;
            public Tuple<ChiAttributeType, int>[] Fields { get; set; }
            public uint Closed;
            public int Score { get { return UnLocked ? this.Fields.Sum((x) => GetPonits(x.Item1, x.Item2)) : 0; } }
            public int[] GetFieldsArray()
            {
                if (UnLocked)
                {
                    int[] array = new int[this.Fields.Length];
                    for (int x = 0; x < this.Fields.Length; x++)
                        array[x] = this.Fields[x].Item2;
                    return array;
                }

                return new int[] { 0, 0, 0, 0 };
            }
            private ChiAttributeFlags attributeFlags;
            public bool UnLocked { get; set; }
            public string Name = "";
            public uint UID;
            public int Rank = 0;
            public ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType _Type)
            {
                this.Fields = new Tuple<ChiAttributeType, int>[4];
                this.Type = _Type;
            }

            public void RerollGM(int Gate, ChiAttributeType PowerType, ushort Value)
            {
                lock (this.Fields)
                {
                    if (Gate < 1)
                    { return; }
                    ChiAttributeFlags flag = AttributeToFlag(PowerType);
                    if ((this.attributeFlags & flag) == 0)
                    {
                        Gate -= 1;
                        this.attributeFlags |= flag;
                        if ((byte)PowerType < 6)
                        {
                            Value *= 2;
                        }
                        this.Fields[Gate] = Tuple.Create(PowerType, (int)((uint)PowerType * 10000 + Value));
                    }
                }
            }
            public void Reroll(Game.MsgServer.MsgChiInfo.LockedFlags Locked,bool Epic = false, bool GMCommand = false)
            {
                try
                {
                    for (int x = 0; x < Fields.Length; x++)
                    {

                        if ((Locked & (Game.MsgServer.MsgChiInfo.LockedFlags)(1 << x)) != 0)
                            continue;

                        if (Fields[x] != null)
                        {
                            this.attributeFlags &= ~AttributeToFlag(Fields[x].Item1);
                        }
                        int test = 1000;
                        ChiAttributeType attribute;
                        ChiAttributeFlags flag;
                        do
                        {
                            test--;
                            if (test < 0)
                            {
                                Console.WriteLine("Problem chi");

                            }
                            string Text = "";
                            string Text1 = "";
                            string Text2 = "";
                            string Text3 = "";
                            if (Fields[x] != null)
                            {
                                Text = Fields[0].Item1.ToString();
                                Text1 = Fields[1].Item1.ToString();
                                Text2 = Fields[2].Item1.ToString();
                                Text3 = Fields[3].Item1.ToString();
                            }
                        Try: attribute = RollRand();
                            flag = AttributeToFlag(attribute);
                            if (Text == flag.ToString() || Text1 == flag.ToString() || Text2 == flag.ToString() || Text3 == flag.ToString())
                            {
                                goto Try;
                            }
                        }
                        while ((this.attributeFlags & flag) != 0);
                        this.attributeFlags |= flag;

                        Fields[x] = Tuple.Create(attribute, StatsRand(attribute, Type, Name, Epic));
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
            private static ChiAttributeFlags AttributeToFlag(ChiAttributeType type)
            {
                return (ChiAttributeFlags)(1 << (int)(type - 1));
            }
            public static ChiAttributeType RollRand()
            {
                byte val = (byte)Rand.Next(1, (byte)Bounds.Length);
                return (ChiAttributeType)Rand.Next(1, val);
            }
            public static Extensions.Counter EpicAtribut = new Extensions.Counter(0);
            public static int StatsRand(ChiAttributeType typ, Game.MsgServer.MsgChiInfo.ChiPowerType StageType, string Name, bool epic = false)
            {
                uint next = EpicAtribut.Next;

                if ((byte)typ < 6)
                {
                    var data = Bounds.First((x) => x.Item1 == typ);
                    if (epic)
                    {
                        return (int)((uint)data.Item1 * 10000 + 200);
                    }
                    ushort val = (ushort)Rand.Next(1, 200);

                    val += 30;
                    if (val > 200)
                        val -= 30;
                    ushort val1 = (ushort)Rand.Next(1, val);
                    uint ID = (uint)data.Item1;
                    if (Core.Rate(3))//نسبة السوبر والاليت
                    {
                        val = (ushort)Rand.Next(185, 195);
                    }
                    if (EpicAtribut.Count == 10000)
                    {
                        val1 = 200;
                        EpicAtribut.Set(0);
                    }
                    int role = (int)(ID * 10000 + val1);
                    if (val1 > 190)//192
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            stream.ChiMessageCreate(StageType, role, Name);
                            Program.SendGlobalPackets.Enqueue(stream);
                        }
                    }
                    return role;
                }
                else
                {
                    var data = Bounds.First((x) => x.Item1 == typ);
                    if (epic)
                    {
                        return (int)((uint)data.Item1 * 10000 + RollToValue(data.Item1, 100));
                    }
                    ushort val = (ushort)Rand.Next(1, 100);

                    ushort val1 = 0;
                    if (val > 60)
                        val1 = (ushort)Rand.Next(1, val);



                    if (EpicAtribut.Count == 10000)
                    {
                        val1 = 100;
                        EpicAtribut.Set(0);
                    }
                    uint val3 = (uint)RollToValue(data.Item1, val1);
                    uint ID = (uint)data.Item1;

                    int role = (int)(ID * 10000 + val3);

                    if (val1 > 90)//93
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            stream.ChiMessageCreate(StageType, role, Name);
                            Program.SendGlobalPackets.Enqueue(stream);
                        }
                        //create message

                    }
                    return role;
                }
            }
            public static double RollToValue(ChiAttributeType type, int roll)
            {
                // value = (roll/100)*(max-min) + min
                var data = Bounds.First((x) => x.Item1 == type);
                return ((roll / 100.0) * (data.Item3 - data.Item2) + data.Item2);
            }
            public static int GetPonits(ChiAttributeType type, double roll)
            {
                var data = Bounds.First((x) => x.Item1 == type);
                uint value = (uint)(roll - (uint)((uint)data.Item1 * 10000));
                return ValueToRoll(data.Item1, value);
            }
            public static int ValueToRoll(ChiAttributeType type, double value)
            {
                // 100*(value-min)/(max-min)
                var data = Bounds.First((x) => x.Item1 == type);
                if ((byte)type < 6)
                {
                    double nextva = (double)value / 10;
                    value = nextva;
                    if (value < 1)
                        return 0;
                }
                return (int)((100 * (value - data.Item2)) / (data.Item3 - data.Item2));
            }
            public static ChiAttributeType GetType(int roll)
            {
                int rollLevel = roll % 200;
                roll -= rollLevel;
                return (ChiAttributeType)(roll / 10000);
            }
            public static uint GetDamage(ChiAttributeType type, int roll)
            {
                var data = Bounds.First((x) => x.Item1 == type);
                uint tt = (uint)data.Item1;
                uint ava = (uint)(roll - (uint)(tt * 10000));
                return ava;
            }
            public override string ToString()
            {
                return new Database.DBActions.WriteLine('/').Add(UnLocked ? 1 : 0)
                    .Add((byte)Type).Add(GetFieldsArray()).Add(Closed).Close();
            }
            public void Load(string line, uint _UID, string _Name)
            {
                this.UID = _UID;
                this.Name = _Name;
                Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
                this.UnLocked = reader.Read((byte)0) == 1;
                if (this.UnLocked)
                {
                    this.Type = (Game.MsgServer.MsgChiInfo.ChiPowerType)reader.Read((byte)0);
                    for (int x = 0; x < Fields.Length; x++)
                    {
                        int role = reader.Read((int)0);
                        ChiAttributeType atr = GetType(role);
                        this.Fields[x] = Tuple.Create(atr, role);
                        this.attributeFlags |= AttributeToFlag(atr);
                    }
                }
                this.Closed = reader.Read((uint)0);
            }
        }


        public static ConcurrentDictionary<uint, Chi> ChiPool = new ConcurrentDictionary<uint, Chi>();

        public int ChiPoints = 0;

        public string Name = "";
        public uint UID;
        public ChiPower GetChiPowerByMode(Conquer6609.Game.MsgServer.MsgChiInfo.ChiPowerType Mode)
        {
            if (Mode == MsgChiInfo.ChiPowerType.Dragon)
                return Dragon;
            if (Mode == MsgChiInfo.ChiPowerType.Phoenix)
                return Phoenix;
            if (Mode == MsgChiInfo.ChiPowerType.Turtle)
                return Turtle;
            if (Mode == MsgChiInfo.ChiPowerType.Tiger)
                return Tiger;
            return null;
        }

        public ChiPower Dragon;
        public ChiPower Phoenix;
        public ChiPower Turtle;
        public ChiPower Tiger;
       
        public long DragonTime = 0;
        public long PhoenixTime = 0;
        public long TurtleTime = 0;
        public long TigerTime = 0;
        public Tuple<ChiAttributeType, int>[] DragonPowers = null;
        public Tuple<ChiAttributeType, int>[] PhoenixPowers = null;
        public Tuple<ChiAttributeType, int>[] TurtlePowers = null;
        public Tuple<ChiAttributeType, int>[] TigerPowers = null;
       
        public uint Locks
        {
            get
            {
                uint Value = 0;
                if (Dragon.UnLocked)
                    Value |= Dragon.Closed;
                if (Phoenix.UnLocked)
                    Value |= Phoenix.Closed;
                if (Turtle.UnLocked)
                    Value |= Turtle.Closed;
                if (Tiger.UnLocked)
                    Value |= Tiger.Closed;
                return Value;
            }
        }
        public Chi(uint _UID)
        {
            this.UID = _UID;
            this.ChiPoints = 4000;
            this.Dragon = new ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType.Dragon);
            this.Phoenix = new ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType.Phoenix);
            this.Turtle = new ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType.Turtle);
            this.Tiger = new ChiPower(Game.MsgServer.MsgChiInfo.ChiPowerType.Tiger);
        }
        public uint TotalLock
        {
            get
            {
                uint interval = 0;
                if (Dragon != null)
                    interval &= Dragon.Closed;
                if (Phoenix != null)
                    interval &= Phoenix.Closed;
                if (Tiger != null)
                    interval &= Tiger.Closed;
                if (Turtle != null)
                    interval &= Turtle.Closed;
                return interval;
            }
        }
        public uint AllScore()
        {
            uint score = 0;
            foreach (var power in this)
            {
                score += (uint)power.Score;
            }
            return score;
        }
        public static int MedPower(ChiAttributeType power)
        {
            if (power == ChiAttributeType.CriticalStrike)
                return 185;
            else if (power == ChiAttributeType.SkillCriticalStrike)
                return 185;
            else if (power == ChiAttributeType.Immunity)
                return 185;
            else if (power == ChiAttributeType.Breakthrough)
                return 185;
            else if (power == ChiAttributeType.Counteraction)
                return 185;
            else if (power == ChiAttributeType.AddAttack)
                return 1900;
            else if (power == ChiAttributeType.MaxLife)
                return 3250;
            else if (power == ChiAttributeType.AddMagicAttack)
                return 2200;
            else if (power == ChiAttributeType.AddMagicDefense)
                return 200;
            else if (power == ChiAttributeType.FinalAttack)
                return 450;
            else if (power == ChiAttributeType.FinalDefense)
                return 450;
         

            else
                return 100;
        }
        public static int MaxPower(ChiAttributeType power)
        {
            if (power == ChiAttributeType.CriticalStrike)
                return 200;
            else if (power == ChiAttributeType.SkillCriticalStrike)
                return 200;
            else if (power == ChiAttributeType.Immunity)
                return 200;
            else if (power == ChiAttributeType.Breakthrough)
                return 200;
            else if (power == ChiAttributeType.Counteraction)
                return 200;
            else if (power == ChiAttributeType.AddAttack)
                return 2000;
            else if (power == ChiAttributeType.MaxLife)
                return 3500;
            else if (power == ChiAttributeType.AddMagicAttack)
                return 2500;
            else if (power == ChiAttributeType.AddMagicDefense)
                return 250;
            else if (power == ChiAttributeType.FinalAttack)
                return 500;
            //else if (power == ChiAttributeType.FinalMagicAttack)
            //    return 300;
            //else if (power == ChiAttributeType.FinalDefense)
            //    return 500;
            else
                return 300;
        }
        public static Tuple<ChiAttributeType, int, int>[] Bounds =
        {
            Tuple.Create(ChiAttributeType.CriticalStrike, 1, 20),
            Tuple.Create(ChiAttributeType.SkillCriticalStrike, 1, 20),
            Tuple.Create(ChiAttributeType.Immunity, 1, 20),
            Tuple.Create(ChiAttributeType.Breakthrough, 1, 20),
            Tuple.Create(ChiAttributeType.Counteraction, 1, 20),
            Tuple.Create(ChiAttributeType.MaxLife, 1000, 3500),
            Tuple.Create(ChiAttributeType.AddAttack, 500, 2000),
            Tuple.Create(ChiAttributeType.AddMagicAttack, 500, 2500),
            Tuple.Create(ChiAttributeType.AddMagicDefense, 50, 250),
            Tuple.Create(ChiAttributeType.FinalAttack, 50, 500),
            Tuple.Create(ChiAttributeType.FinalMagicAttack, 50, 300),            
            Tuple.Create(ChiAttributeType.FinalDefense, 50, 500),
            Tuple.Create(ChiAttributeType.FinalMagicDefense, 50, 300)
        };
        public IEnumerator<ChiPower> GetEnumerator()
        {
            yield return Dragon;
            yield return Phoenix;
            yield return Tiger;
            yield return Turtle;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public unsafe void SendQueryUpdate(Client.GameClient user, Chi.ChiPower MyPower, ServerSockets.Packet stream)
        {
            stream.GenericRankingCreate(MsgGenericRanking.Action.QueryCount, (Game.MsgServer.MsgGenericRanking.RankType)(60000000 + (byte)MyPower.Type), 0, 0, 1);
            stream.AddItemGenericRankingCreate(MyPower.Rank, (uint)MyPower.Score, MyPower.UID, MyPower.Name);
            user.Send(stream.GenericRankingFinalize());
        }
        uint test = 0;

        public uint CriticalStrike
        {
            get
            {
                return test;
            }
            set
            {
                test = value;
            }
        }
        public uint SkillCriticalStrike { get; set; }
        public uint Immunity { get; set; }
        public uint Breakthrough { get; set; }
        public uint Counteraction { get; set; }
        public uint MaxLife { get; set; }
        public uint AddAttack { get; set; }
        public uint AddMagicAttack { get; set; }
        public uint AddMagicDefense { get; set; }
        public uint FinalAttack { get; set; }
        public uint FinalMagicAttack { get; set; }
        public uint FinalDefense { get; set; }
        public uint FinalMagicDefense { get; set; }

        public static object SynRoot = new object();
        public static void ComputeStatus(Chi ClientChi)
        {
            lock (SynRoot)
            {
                ClientChi.CriticalStrike = 0;
                ClientChi.SkillCriticalStrike = 0;
                ClientChi.Immunity = 0;
                ClientChi.Breakthrough = 0;
                ClientChi.Counteraction = 0;
                ClientChi.MaxLife = 0;
                ClientChi.AddAttack = 0;
                ClientChi.AddMagicAttack = 0;
                ClientChi.AddMagicDefense = 0;
                ClientChi.FinalAttack = 0;
                ClientChi.FinalDefense = 0;
                ClientChi.FinalMagicAttack = 0;
                ClientChi.FinalMagicDefense = 0;

                foreach (var power in ClientChi)
                {
                    if (!power.UnLocked)
                        continue;

                    foreach (var field in power.Fields)
                    {
                        int value = field.Item2;
                        switch (field.Item1)
                        {
                            case ChiAttributeType.AddAttack: ClientChi.AddAttack += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.AddMagicAttack: ClientChi.AddMagicAttack += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.AddMagicDefense: ClientChi.AddMagicDefense += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.Breakthrough: ClientChi.Breakthrough += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.Counteraction: ClientChi.Counteraction += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.CriticalStrike: ClientChi.CriticalStrike += ChiPower.GetDamage(field.Item1, field.Item2) * 10; break;
                            case ChiAttributeType.FinalAttack: ClientChi.FinalAttack += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.FinalDefense: ClientChi.FinalDefense += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.FinalMagicAttack: ClientChi.FinalMagicAttack += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.FinalMagicDefense: ClientChi.FinalMagicDefense += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.Immunity: ClientChi.Immunity += ChiPower.GetDamage(field.Item1, field.Item2) * 10; break;
                            case ChiAttributeType.MaxLife: ClientChi.MaxLife += ChiPower.GetDamage(field.Item1, field.Item2); break;
                            case ChiAttributeType.SkillCriticalStrike: ClientChi.SkillCriticalStrike += ChiPower.GetDamage(field.Item1, field.Item2) * 10; break;
                        }
                    }
                    if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Dragon)
                    {
                        if (power.Rank >= 1 && power.Rank <= 3 || power.Score == 400)
                        {
                            ClientChi.MaxLife += 5000;
                            ClientChi.FinalAttack += 1000;
                            ClientChi.FinalMagicAttack += 300;
                            ClientChi.FinalMagicDefense += 300;
                        }
                        else if (power.Rank >= 4 && power.Rank <= 20)
                        {
                            ClientChi.MaxLife += (uint)(3000 - (uint)((power.Rank - 4) * 62.5));
                            ClientChi.FinalAttack += (uint)(600 - (uint)((power.Rank - 4) * 12.5));
                            ClientChi.FinalMagicAttack += (uint)(200 - (uint)((power.Rank - 4) * 3));
                            ClientChi.FinalMagicDefense += (uint)(200 - (uint)((power.Rank - 4) * 3));
                        }
                        else if (power.Rank >= 21 && power.Rank <= 50)
                        {
                            ClientChi.MaxLife += 1500;
                            ClientChi.FinalAttack += 300;
                            ClientChi.FinalMagicAttack += 100;
                            ClientChi.FinalMagicDefense += 100;
                        }
                    }
                    else if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Phoenix)
                    {
                        if (power.Rank >= 1 && power.Rank <= 3 || power.Score == 400)
                        {
                            ClientChi.AddAttack += 3000;
                            ClientChi.AddMagicAttack += 3000;
                            ClientChi.FinalAttack += 1000;
                            ClientChi.FinalMagicAttack += 300;
                        }
                        else if (power.Rank >= 4 && power.Rank <= 20)
                        {
                            ClientChi.AddAttack += (uint)(2000 - (uint)((power.Rank - 4) * 31.5));
                            ClientChi.AddMagicAttack += (uint)(2000 - (uint)((power.Rank - 4) * 31.5));
                            ClientChi.FinalAttack += (uint)(600 - (uint)((power.Rank - 4) * 12.5));
                            ClientChi.FinalMagicAttack += (uint)(200 - (uint)((power.Rank - 4) * 3));
                        }
                        else if (power.Rank >= 21 && power.Rank <= 50)
                        {
                            ClientChi.AddAttack += 1000;
                            ClientChi.AddMagicAttack += 2000;
                            ClientChi.FinalAttack += 1000;
                            ClientChi.FinalMagicAttack += 300;
                        }
                    }
                    else if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Tiger)
                    {
                        if (power.Rank >= 1 && power.Rank <= 3 || power.Score == 400)
                        {
                            ClientChi.CriticalStrike += 15 * 100;
                            ClientChi.SkillCriticalStrike += 15 * 100;
                            ClientChi.Immunity += 8 * 100;
                        }
                        else if (power.Rank >= 4 && power.Rank <= 20)
                        {
                            ClientChi.CriticalStrike += (uint)(11 * 100 - (uint)((power.Rank - 4) * 17));
                            ClientChi.SkillCriticalStrike += (uint)(11 * 100 - (uint)((power.Rank - 4) * 17));
                            ClientChi.Immunity += (uint)(5 * 100 - (uint)((power.Rank - 4) * 13));
                        }
                        else if (power.Rank >= 21 && power.Rank <= 50)
                        {
                            ClientChi.CriticalStrike += 5 * 100;
                            ClientChi.SkillCriticalStrike += 5 * 100;
                            ClientChi.Immunity += 2 * 100;
                        }
                    }
                    else if (power.Type == Game.MsgServer.MsgChiInfo.ChiPowerType.Turtle)
                    {
                        if (power.Rank >= 1 && power.Rank <= 3 || power.Score == 400)
                        {
                            ClientChi.Breakthrough += 15 * 10;
                            ClientChi.Counteraction += 15 * 10;
                            ClientChi.Immunity += 8 * 100;
                        }
                        else if (power.Rank >= 4 && power.Rank <= 20)
                        {
                            ClientChi.Breakthrough += (uint)((11 * 100 - (uint)((power.Rank - 4) * 17)) / 10);
                            ClientChi.Counteraction += (uint)((11 * 100 - (uint)((power.Rank - 4) * 17)) / 10);
                            ClientChi.Immunity += (uint)(5 * 100 - (uint)((power.Rank - 4) * 13));
                        }
                        else if (power.Rank >= 21 && power.Rank <= 50)
                        {
                            ClientChi.Breakthrough += 5 * 10;
                            ClientChi.Counteraction += 5 * 10;
                            ClientChi.Immunity += 2 * 100;
                        }
                    }
                }

            }
        }

    }
}
