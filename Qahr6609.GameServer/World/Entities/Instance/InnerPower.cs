using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Role.Instance
{
    public class InnerPower
    {

        public static class InnerPowerRank
        {
            public const int MaxPlayers = 100;
            private static System.Collections.Concurrent.ConcurrentDictionary<uint, InnerPower> InnerPowerRanks = new System.Collections.Concurrent.ConcurrentDictionary<uint, InnerPower>();
            private static InnerPower[] Rankings;
            private static object synroot = new object();

            public static void UpdateRank(InnerPower inner)
            {
                if (InnerPowerRanks.Count < MaxPlayers)
                {
                    InnerPowerRanks.TryAdd(inner.UID, inner);
                    CreateRanks();
                }
                else
                {
                    var lastplayer = Rankings.Last();
                    if (lastplayer.TotalScore < inner.TotalScore)
                    {
                        InnerPowerRanks.TryAdd(inner.UID, inner);
                        CreateRanks();
                    }
                    else if (InnerPowerRanks.ContainsKey(inner.UID))
                        CreateRanks();
                }
            }

            public static void CreateRanks()
            {
                lock (synroot)
                {
                    var array = InnerPowerRanks.Values.ToArray();
                    Rankings = array.OrderByDescending(p => p.TotalScore).ToArray();
                }
            }
            public static InnerPower[] GetRankingList()
            {
                InnerPower[] array = null;
                lock (synroot)
                    array = Rankings.ToArray();
                return array;
            }
        }

        public static System.Collections.Concurrent.ConcurrentDictionary<uint, InnerPower> InnerPowerPolle = new System.Collections.Concurrent.ConcurrentDictionary<uint, InnerPower>();

        public int Potency = 0;
        public uint TotalScore { get { return (uint)Stages.Sum(p => p.Score); } }
        public uint UID;
        public string Name = "";

        public override string ToString()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(Name).Add(UID).Add(Potency);
            var array = Stages.Where(p => p.UnLocked).ToArray();
            writer.Add(array.Length);
            foreach (var stage in array)
            {
                writer.Add(stage.ID).Add((byte)(stage.UnLocked ? 1 : 0)).Add(stage.NeiGongs.Length);
                foreach (var neigong in stage.NeiGongs)
                    writer.Add(neigong.ID).Add(neigong.Score).Add((byte)(neigong.Unlocked ? 1 : 0)).Add(neigong.level).Add((byte)(neigong.Complete ? 1 : 0));
            }
            //extra (read by ReadExtra; older loaders ignore it): last transfer time, then the best Perfect
            //score each reshaped Inner Power had before it was abandoned.
            writer.Add(LastExchange);
            List<Stage.NeiGong> reshaped = new List<Stage.NeiGong>();
            foreach (var stage in Stages)
                foreach (var neigong in stage.NeiGongs)
                    if (neigong.BestScore > 0)
                        reshaped.Add(neigong);
            writer.Add(reshaped.Count);
            foreach (var neigong in reshaped)
                writer.Add(neigong.ID).Add(neigong.BestScore);
            return writer.Close();
        }

        /// <summary>Reads the fields ToString appends after the stages (missing in old saves: defaults).</summary>
        public void ReadExtra(Database.DBActions.ReadLine reader)
        {
            LastExchange = reader.Read((long)0);
            int count = reader.Read((int)0);
            for (int x = 0; x < count && x < 1000; x++)
            {
                byte id = reader.Read((byte)0);
                byte best = reader.Read((byte)0);
                Stage stage;
                Stage.NeiGong gong;
                if (TryGetStageAndGong(id, out stage, out gong))
                    gong.BestScore = best;
            }
            //the score of an Inner Power is only known once it is Perfect ("Score: Unknown" before)
            foreach (var stage in Stages)
                foreach (var neigong in stage.NeiGongs)
                    if (!neigong.Complete)
                        neigong.Score = 0;
        }

        /// <summary>Time (DateTime ticks) of the last Inner Power transfer with a trade partner.</summary>
        public long LastExchange = 0;
        public const int ExchangeCostCPs = 270;//client: STR_NEIGONG_MIJI
        public const int ExchangeCooldownHours = 12;
        public const uint PowerEraser = 3005412;
        public const byte PerfectScoreMin = 60;

        /// <summary>Potency spent to raise this Inner Power from P0 to its current phase.</summary>
        public static int SpentPotency(Database.InnerPowerTable.Stage.NeiGong DBGong, byte level)
        {
            int sum = 0;
            if (DBGong == null || DBGong.ProgressNeiGongValue == null || DBGong.ProgressNeiGongValue.Length == 0)
                return 0;
            for (int x = 0; x < level; x++)
                sum += (int)DBGong.ProgressNeiGongValue[Math.Min(x, DBGong.ProgressNeiGongValue.Length - 1)];
            return sum;
        }

        /// <summary>
        /// Perfect (client "YuanGong" button): an Inner Power at its last phase is completed and its score is
        /// revealed. The score is random (PerfectScoreMin-100); after a reshape the next Perfect always scores
        /// higher than the abandoned one ("Perfect again brings a higher score").
        /// </summary>
        public static byte RollPerfectScore(Stage.NeiGong gong)
        {
            int min = Math.Max(PerfectScoreMin, gong.BestScore + 1);
            if (min > 100)
                min = 100;
            return (byte)Program.GetRandom.Next(min, 101);
        }

        public bool CanExchange(out int minutesLeft)
        {
            minutesLeft = 0;
            if (LastExchange == 0)
                return true;
            TimeSpan left = new DateTime(LastExchange).AddHours(ExchangeCooldownHours) - DateTime.Now;
            if (left.TotalMinutes <= 0)
                return true;
            minutesLeft = (int)Math.Ceiling(left.TotalMinutes);
            return false;
        }

        /// <summary>Swaps the Inner Power progress and Potency of two players (trade partner transfer).</summary>
        public void ExchangeWith(InnerPower other)
        {
            Stage[] stages = Stages;
            Stages = other.Stages;
            other.Stages = stages;
            int potency = Potency;
            Potency = other.Potency;
            other.Potency = potency;
            LastExchange = other.LastExchange = DateTime.Now.Ticks;
            UpdateStatus();
            other.UpdateStatus();
        }

        public uint CriticalStrike { get; private set; }
        public uint SkillCriticalStrike { get; private set; }
        public uint Immunity { get; private set; }
        public uint Breakthrough { get; private set; }
        public uint Counteraction { get; private set; }
        public uint MaxLife { get; private set; }
        public uint AddAttack { get; private set; }
        public uint AddMagicAttack { get; private set; }
        public uint AddMagicDefense { get; private set; }
        public uint FinalAttack { get; private set; }
        public uint FinalMagicAttack { get; private set; }
        public uint FinalDefense { get; private set; }
        public uint FinalMagicDefense { get; private set; }
        public uint Defence { get; private set; }

        public Stage[] Stages = new Stage[Database.InnerPowerTable.Count];

        public class Stage
        {
            public ushort ID;
            public bool UnLocked = false;
            public bool Complete
            {
                get
                {
                    return NeiGongs.Length == NeiGongs.Where(p => p.Complete).Count();
                }
            }
            public uint Score { get { return (uint)NeiGongs.Sum(p => p.Score); } }
            public NeiGong[] NeiGongs;

            public ushort GetNoumberAtributes(NeiGong[] gongs)
            {
                var DBStage = Database.InnerPowerTable.Stages[ID - 1];
                ushort value = 0;
                for (int x = 0; x < gongs.Length; x++)
                    value += (ushort)DBStage.NeiGongAtributes[x].AtributesValues.Length;

                return value;
            }
            public ushort GetAtributesCount(ushort gong_noumber)
            {
                return (ushort)Database.InnerPowerTable.Stages[ID - 1].NeiGongAtributes[gong_noumber].AtributesValues.Length;
            }
            public bool GetLastNeiGang(out NeiGong _gong)
            {
                _gong = null;
                for (int x = 0; x < NeiGongs.Length; x++)
                {
                    if (NeiGongs[x].Unlocked)
                    {
                        _gong = NeiGongs[x];
                    }
                }
                return _gong != null;
            }
            public class NeiGong
            {
                public byte ID;
                public byte Score;
                public bool Unlocked = false;
                public byte level;
                public bool Complete = false;
                public byte BestScore = 0;//Perfect score before the last reshape (Power Eraser)
            }
        }

        public InnerPower(string _Name, uint _uid)
        {
            Name = _Name;
            UID = _uid;

            for (int x = 0; x < Database.InnerPowerTable.Count; x++)
            {
                var DBStage = Database.InnerPowerTable.Stages[x];
                Stage stage = new Stage();
                stage.ID = DBStage.ID;


                stage.NeiGongs = new Stage.NeiGong[DBStage.NeiGongNum];
                for (int y = 0; y < DBStage.NeiGongNum; y++)
                {
                    stage.NeiGongs[y] = new Stage.NeiGong();
                    stage.NeiGongs[y].ID = DBStage.NeiGongAtributes[y].ID;
                }
                Stages[x] = stage;
            }
            InnerPowerPolle.TryAdd(_uid, this);
        }


        public void UpdateStatus()
        {

            Defence = 0;
            CriticalStrike = 0;
            SkillCriticalStrike = 0;
            Immunity = 0;
            Breakthrough = 0;
            Counteraction = 0;
            MaxLife = 0;
            AddAttack = 0;
            AddMagicAttack = 0;
            AddMagicDefense = 0;
            FinalAttack = 0;
            FinalDefense = 0;
            FinalMagicAttack = 0;
            FinalMagicDefense = 0;

            foreach (var stage in Stages)
            {
                if (!stage.UnLocked)
                    break;
                var DBStage = Database.InnerPowerTable.Stages[stage.ID - 1];
                for (int x = 0; x < stage.NeiGongs.Length; x++)
                {
                    var neigong = stage.NeiGongs[x];
                    if (!neigong.Unlocked)
                        break;
                    if (neigong.level == 0)
                        continue;//a reshaped (P0) Inner Power does not cancel the ones after it
                    var DBNeiGong = DBStage.NeiGongAtributes[x];
                    for (int i = 0; i < DBNeiGong.AtributesType.Length; i++)
                    {
                        var AtributType = DBNeiGong.AtributesType[i];
                        var AtributValue = DBNeiGong.AtributesValues[i];

                        AddAtributes(AtributType, (uint)((AtributValue / DBNeiGong.MaxLevel) * neigong.level));
                    }
                }
                if (stage.Complete)
                {
                    for (int y = 0; y < DBStage.SpecialAtributesType.Length; y++)
                    {
                        var SpecialAtributType = DBStage.SpecialAtributesType[y];
                        var SpecialAtributValue = DBStage.AtributesValues[y];
                        AddAtributes(SpecialAtributType, SpecialAtributValue);
                    }
                }
            }
        }
        public void AddAtributes(Database.InnerPowerTable.AtributeType AtributType, uint AtributValue)
        {
            switch (AtributType)
            {
                case Database.InnerPowerTable.AtributeType.Break:
                    Breakthrough += (uint)(AtributValue / 10);
                    break;
                case Database.InnerPowerTable.AtributeType.FinalMAttack:
                    FinalMagicAttack += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.FinalPAttack:
                    FinalAttack += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.FinalMDamage:
                    FinalMagicDefense += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.FinalPDamage:
                    FinalDefense += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.Immunity:
                    Immunity += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.MAttack:
                    AddMagicAttack += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.MaxHP:
                    MaxLife += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.MDefense:
                    AddMagicDefense += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.MStrike:
                    SkillCriticalStrike += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.PAttack:
                    AddAttack += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.PDefense:
                    Defence += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.PStrike:
                    CriticalStrike += AtributValue;
                    break;
                case Database.InnerPowerTable.AtributeType.Conteraction:
                    Counteraction += AtributValue / 10;
                    break;

            }
        }

        public bool isUnlockedNeiGong(byte stage)
        {
            if (stage > 1)
            {
                Stage mstage = null;
                Stage.NeiGong mgong = null;
                if (TryGetStageAndGong((byte)(stage - 1), out mstage, out mgong))
                    return mgong.Unlocked == true;
            }
            return stage == 1;
        }


        public void AddPotency(ServerSockets.Packet stream, Client.GameClient user, int _Potency)
        {
            this.Potency += _Potency;
            user.Player.SendUpdate(stream, Potency, Game.MsgServer.MsgUpdate.DataType.InnerPowerPotency, false);
        }
        public bool TryGetStageAndGong(byte ID, out Stage _stage, out Stage.NeiGong _gong)
        {
            foreach (var m_stage in Stages)
            {
                foreach (var m_gong in m_stage.NeiGongs)
                {
                    if (ID == m_gong.ID)
                    {
                        _stage = m_stage;
                        _gong = m_gong;
                        return true;
                    }
                }
            }
            _stage = null;
            _gong = null;
            return false;

        }
        public Stage.NeiGong[] GetNeiGongs()
        {
            List<Stage.NeiGong> arraygongs = new List<Stage.NeiGong>();
            foreach (var stage in Stages)
            {
                if (stage.UnLocked)
                {
                    foreach (var neigong in stage.NeiGongs)
                        if (neigong.Unlocked)
                            arraygongs.Add(neigong);
                }
            }
            return arraygongs.ToArray();
        }
        public bool GetLastStage(out Stage stage)
        {
            stage = null;
            for (int x = 0; x < Stages.Length; x++)
            {
                var element = Stages[x];
                if (element.UnLocked)
                {
                    stage = element;
                }
            }
            return stage != null;
        }
    }
}
