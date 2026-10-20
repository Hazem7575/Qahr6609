using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static void GetUpdatePacket(this ServerSockets.Packet stream, out MsgUpdate.DataType ID, out ulong Value)
        {
            stream.SeekForward(sizeof(uint));
            uint uid = stream.ReadUInt32();
            uint count = stream.ReadUInt32();
            ID = (MsgUpdate.DataType)stream.ReadUInt32();
            Value = stream.ReadUInt64();

        }
    }


    public unsafe class MsgUpdate
    {
        public class OnlineTraining
        {
            public const byte
            Show = 0,
            InTraining = 1,
            Review = 2,
            IncreasePoints = 3,
            ReceiveExperience = 4,
            Remove = 5;
        }
        public class CreditGifts
        {
            public const byte
                Show = 0,
                CanClaim = 1,
                Claim = 5,
                ShowSpecialItems = 6;
        }
        [Flags]
        public enum Flags : int
        {
            Normal = 3,//0x0,
            FlashingName = 0,
            Poisoned = 1,
            
            Invisible = 2,
            XPList = 4,
            Dead = 5,
            TeamLeader = 6,
            StarOfAccuracy = 7,
            MagicShield = 8,
            Shield = 8,
            Stigma = 9,
            Ghost = 10,
            FadeAway = 11,
            RedName = 14,
            BlackName = 15,
            ReflectMelee = 17,
            Superman = 18,
            Ball = 19,
            Ball2 = 20,
            Invisibility = 22,
            Cyclone = 23,
            Dodge = 26,
            Fly = 27,
            Intensify = 28,
            CastPray = 30,
            Praying = 31,
            Cursed = 32,
            HeavenBlessing = 33,
            TopGuildLeader = 34,
            TopDeputyLeader = 35,
            MonthlyPKChampion = 36,
            WeeklyPKChampion = 37,
            TopWarrior = 38,
            TopTrojan = 39,
            TopArcher = 40,
            TopWaterTaoist = 41,
            TopFireTaoist = 42,
            TopNinja = 43,
            ShurikenVortex = 46,
            FatalStrike = 47,
            Flashy = 48,
            Ride = 50,
            TopSpouse = 51,
            Accelerated = 52,
            Deceleration = 53,
            Frightened = 54,
            HeavenSparkle = 55,
            IncMoveSpeed = 56,
            GodlyShield = 57,
            Dizzy = 58,
            Freeze = 59,
            Confused = 60,
            Top8Weekly = 63,
            Top4Weekly = 64,
            Top2Weekly = 65,
            ChaintBolt = 92,
            AzureShield = 93,
            ScurvyBomb = 96,//that is use for abuse.
            TyrantAura = 98,
            FeandAura = 100,
            MetalAura = 102,
            WoodAura = 104,
            WaterAura = 106,
            FireAura = 108,
            EartAura = 110,
            SoulShackle = 111,
            Oblivion = 112,
            ShieldBlock = 113,
            TopMonk = 114,
            TopPirate = 122,
            CTF_Flag = 118,
            PoisonStar = 119,
            CannonBarrage = 120,
            BlackbeardsRage = 121,
            DefensiveStance = 126,
            MagicDefender = 128,
            RemoveName = 129,
            PurpleBall = 131,
            BlueBall = 132,
            PathOfShadow = 145,
            BladeFlurry = 146,
            KineticSpark = 147,
            AutoHunt = 148,
            SuperCyclone = 150,

            TopSuperGuildWar = 153,

            //Effect = 300, // will be test!

            DragonFlow = 148,//20

            TopDragonLee = 154,////26
            DragonFury = 158,//30
            DragonCyclone = 159,//31
            DragonSwing = 160,//32

            Goldbrick = 165,//
            lianhuaran01 = 168,
            lianhuaran02 = 169,
            lianhuaran03 = 170,
            lianhuaran04 = 171,
            FullPowerWater = 172,
            FullPowerFire = 173,
            ShieldBreak = 176, // 20% at change
            DivineGuard = 177,
            Backfire = 179,
            ScarofEarth = 180,
            ManiacDance = 181,
            Pounce = 182,

            Omnipotence = 192,
            WindWalkerFan = 194,

            IncreseColdTime = 198,
            HealingSnow = 196,
            ChillingSnow = 197,
            Thunderbolt = 199,
            FreezingPelter = 200,
            RevengeTail = 202,
            ShadowofChaser = 204,
            TopWindWalker = 203,
            MRConquer = 38,
            MSConquer = 39,

            ConuqerSuperYellow = 151, // Memeber Flag  
            ConuqerSuperBlue = 152, //DL flag 
            ConuqerSuperUnderBlue = 153, //GL flag 
            GoldBrickNormal = 161,
            GoldBrickRefined = 162,
            GoldBrickUnique = 163,
            GoldBrickElite = 164,
            MRConquerHost = 166,
            MSConquerHostess = 167,
            rygh_hglx = 174,//top
            rygh_syzs = 175,//top
            bdeltoid_cyc = 205,
            _p_6_targst = 206,
            King = 205,//king
            Prince = 206,
            Duck = 207,
           
        }
        [Flags]
        public enum DataType : uint
        {
            Hitpoints = 0,
            MaxHitpoints = 1,
            Mana = 2,
            MaxMana = 3,
            Money = 4,
            Experience = 5,
            PKPoints = 6,
            Class = 7,
            Stamina = 8,
            WHMoney = 9,
            Atributes = 10,
            Mesh = 11,
            Level = 12,
            Spirit = 13,
            Vitality = 14,
            Strength = 15,
            Agility = 16,
            HeavensBlessing = 17,
            DoubleExpTimer = 18,
            CursedTimer = 20,
            Reborn = 22,
            VirtutePoints = 23,
            StatusFlag = 25,
            HairStyle = 26,
            XPCircle = 27,
            LuckyTimeTimer = 28,
            ConquerPoints = 29,
            OnlineTraining = 31,
            ExtraBattlePower = 36,
            ArsenalBp = 37,
            Merchant = 38,
            VIPLevel = 39,
            QuizPoints = 40,
            EnlightPoints = 41,
            ClanShareBp = 42,
            GuildBattlePower = 44,
            BoundConquerPoints = 45,
            RaceShopPoints = 47,
            Contestant = 48,
            AzureShield = 49,
            FirsRebornClass = 51,
            SecondRebornClass = 50,
            Team = 52,
            SoulShackle = 54,
            Fatigue = 55,
            DefensiveStance = 56,

            IncreaseMStrike = 60,
            IncreasePStrike = 59,
            IncreaseImunity = 61,
            IncreaseBreack = 62,
            IncreaseAntiBreack = 63,
            IncreaseMaxHp = 64,
            IncreasePAttack = 65,
            IncreaseMAttack = 66,
            IncreaseFinalPDamage = 67,
            IncreaseFinalMDamage = 68,
            IncreaseFinalPAttack = 69,
            IncreaseFinalMAttack = 70,
            MainFlag = 71,
            ExpProtection = 73,

            DragonSwing = 75,
            DragonFury = 74,
            InnerPowerPotency = 77,
            AppendIcon = 78,
            InventorySash = 79,
            InventorySashMax = 80,
            ExploitsRank = 82,
            UnionRank = 83,
            Anger = 90,
            HuntingBouns = 100


        }
        public unsafe MsgUpdate(ServerSockets.Packet stream, uint UID, uint count = 1)
        {
            stream.InitWriter();
            stream.Write(Extensions.Time32.Now.Value);
            stream.Write(UID);
            stream.Write(count);
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, long Value)
        {
            stream.Write((uint)ID);
            stream.Write(Value);
            stream.Write(0ul);
            stream.Write(0ul);
            stream.Write(0);

            return stream;
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, uint Flag, uint Time, uint Dmg, uint Level)
        {
            stream.Write((uint)ID);
            stream.Write(Flag);
            stream.Write(Time);
            stream.Write(Dmg);
            stream.Write(Level);
            stream.Write(0ul);
            stream.Write(0);

            return stream;
        }
        public ServerSockets.Packet Append(ServerSockets.Packet stream, DataType ID, uint[] Value)
        {
            stream.Write((uint)ID);

            if (Value != null)
            {
                for (int x = 0; x < 7; x++)
                {
                    if (Value.Length > x)
                    {
                        stream.Write(Value[x]);
                    }
                    else
                        stream.Write(0u);
                }
            }
            return stream;
        }
        public ServerSockets.Packet GetArray(ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.Update);
            return stream;
        }
    }
}
