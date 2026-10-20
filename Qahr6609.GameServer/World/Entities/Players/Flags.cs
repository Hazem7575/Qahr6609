using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Role
{
    public class Flags
    {
      
      
        public enum ExploitsRank : uint
        {
            Corporal = 1,
            Decurion = 2,
            Centurion = 3,
            Sergeant = 4,
            StaffSergeant = 5,
            MasterSergeant = 6,
            DeputyGeneral = 7,
            AssistantGeneral = 8,
            General = 9,
            ChiefofStaff = 10,
            ChariotsandCavalryGeneral = 11,
            FlyingCavalryGeneral = 12,
            GeneralinChief = 13
        }
        public enum GuildMemberRank : ushort
        {
            GuildLeader = 1000,
            DeputyLeader = 990,
            HDeputyLeader = 980,
            LeaderSpouse = 920,
            Manager = 890,
            HonoraryManager = 880,

            TSupervisor = 859,//tulips
            OSupervisor = 858,//orchides
            CPSupervisor = 857,//cps
            ASupervisor = 856,//cred ca e donation super visor
            SSupervisor = 855,////silver
            GSupervisor = 854,//guide
            PKSupervisor = 853,//pk supervisor
            RoseSupervisor = 852,//rose
            LilySupervisor = 851,///lili
            Supervisor = 850,//toata donatia la guild sa fie mare
            HonorarySuperv = 840,

            Steward = 690,
            HonorarySteward = 680,
            DeputySteward = 650,
            DLeaderSpouse = 620,
            DLeaderAide = 611,
            LSpouseAide = 610,
            Aide = 602,

            TulipAgent = 599,
            OrchidAgent = 598,
            CPAgent = 597,
            ArsenalAgent = 596,
            SilverAgent = 595,
            GuideAgent = 594,
            PKAgent = 593,
            RoseAgent = 592,
            LilyAgent = 591,
            Agent = 590,

            SupervSpouse = 521,
            ManagerSpouse = 520,
            SupervisorAide = 511,
            ManagerAide = 510,

            TulipFollower = 499,
            OrchidFollower = 498,
            CPFollower = 497,
            ArsFollower = 496,
            SilverFollower = 495,
            GuideFollower = 494,
            PKFollower = 493,
            RoseFollower = 492,
            LilyFollower = 491,
            Follower = 490,

            StewardSpouse = 420,
            SeniorMember = 210,
            Member = 200,
            None = 0
        }
        public enum NpcType : ushort
        {
            Stun = 0,
            Shop = 1, // this is look shop mail! type 1 not 2 or 32 !
            Talker = 2, // this is type talker if you wanna to can see speaker! info 2 not more!
            Beautician = 5, // what is!? i can see! it's -> Photo's OF you Account, 
            Upgrader = 6,
            Socketer = 7,
            Pole = 10,
            Booth = 14, //it work now i will going to fix items #folder -> shops  
            Gambling = 19,
            Stake = 21,//
            Scarecrow = 22,
            Furniture = 25,
            Gate = 26,
            ClanInfo = 31,
            DialogAndGui = 32,
             Flag = 47
        }
        public enum ConquerAngle : byte
        {
            SouthWest = 0,
            West = 1,
            NorthWest = 2,
            North = 3,
            NorthEast = 4,
            East = 5,
            SouthEast = 6,
            South = 7
        }

        public enum ConquerAction : uint
        {
            None = 0x00,
            Cool = 0xE6,
            Kneel = 0xD2,
            Sad = 0xAA,
            Happy = 0x96,
            Angry = 0xA0,
            Lie = 0x0E,
            Dance = 0x01,
            Wave = 0xBE,
            Bow = 0xC8,
            Sit = 0xFA,
            Jump = 0x64,
            MagicDefender = 273344,

            InteractionKiss = 34466,
            InteractionHold = 34468,
            InteractionHug = 34469,
            CoupleDances = 34474
        }
        public enum SoulTyp
        {
            None = 0,
            Headgear = 1,
            Necklace = 2,
            Armor = 3,
            OneHandWeapon = 4,
            TwoHandWeapon = 5,
            Ring = 6,
            Boots = 8,
        }
        public enum ConquerItem : ushort
        {
            Inventory = 0,
            Head = 1,
            Necklace = 2,
            Armor = 3,
            RightWeapon = 4,
            LeftWeapon = 5,
            Ring = 6,
            Bottle = 7,
            Boots = 8,
            Garment = 9,
            Fan = 10,
            Tower = 11,
            Steed = 12,
            RightWeaponAccessory = 15,
            LeftWeaponAccessory = 16,
            SteedMount = 17,
            RidingCrop = 18,
            Wing = 19,


            AleternanteHead = 21,
            AleternanteNecklace = 22,
            AleternanteArmor = 23,
            AleternanteRightWeapon = 24,
            AleternanteLeftWeapon = 25,
            AleternanteRing = 26,
            AleternanteBottle = 27,
            AleternanteBoots = 28,
            AleternanteGarment = 29
        }
        [Flags]
        public enum ItemMode : ushort
        {
            None = 0,
            AddItem = 1,
            Trade = 2,
            Update = 3,
            View = 4,
            Active = 5,
            AddItemReturned = 8,
            ChatItem = 9,
            Inbox = 11,
            Auction = 12,
            PerfectionView = 13

        }
        public enum PKMode : byte
        {
            PK = 0,
            Peace = 1,
            Team = 2,
            Capture = 3,
            Revange = 4,
            Guild = 5,
            Jiang = 6,
            CS = 7,
            Union = 11
        }
        public enum ItemEffect : uint
        {
            None = 0,
            Poison = 0xC8,
            Stigma = 0xC9,
            MP = 0xCA,
            Shield = 0xCB,
            Horse = 0x64
        }
        public enum Color : uint
        {
            Black = 2,
            Orange = 3,
            LightBlue = 4,
            Red = 5,
            Blue = 6,
            Yellow = 7,
            Purple = 8,
            White = 9
        }
        public enum Gem : byte
        {
            NormalPhoenixGem = 1,
            RefinedPhoenixGem = 2,
            SuperPhoenixGem = 3,

            NormalDragonGem = 11,
            RefinedDragonGem = 12,
            SuperDragonGem = 13,

            NormalFuryGem = 21,
            RefinedFuryGem = 22,
            SuperFuryGem = 23,

            NormalRainbowGem = 31,
            RefinedRainbowGem = 32,
            SuperRainbowGem = 33,

            NormalKylinGem = 41,
            RefinedKylinGem = 42,
            SuperKylinGem = 43,

            NormalVioletGem = 51,
            RefinedVioletGem = 52,
            SuperVioletGem = 53,

            NormalMoonGem = 61,
            RefinedMoonGem = 62,
            SuperMoonGem = 63,

            NormalTortoiseGem = 71,
            RefinedTortoiseGem = 72,
            SuperTortoiseGem = 73,

            NormalThunderGem = 101,
            RefinedThunderGem = 102,
            SuperThunderGem = 103,

            NormalGloryGem = 121,
            RefinedGloryGem = 122,
            SuperGloryGem = 123,

            NoSocket = 0,
            EmptySocket = 255
        }
        public enum ExperienceEffect : byte
        {
            None = 0,
            angelwing = 1,
            bless = 2
        }
        public enum SpellID : ushort
        {
            //item Effect Spells
            Poison = 3306,
            EffectMP = 1175,
            EffectHP = 1190,
            Pervade = 3090,
            Thunder = 1000,
            Fire = 1001,
            Tornado = 1002,
            Cure = 1005,
            Lightning = 1010,
            Accuracy = 1015,
            Shield = 1020,
            Superman = 1025,
            FastBlader = 1045,
            ScrenSword = 1046,
            Roar = 1040,//need to coding that !
            Revive = 1050,
            Dash = 1051,
            HealingRain = 1055,
            Invisibility = 1075,
            StarofAccuracy = 1085,
            MagicShield = 1090,
            Stigma = 1095,
            Pray = 1100,
            Cyclone = 1110,
            Hercules = 1115,
            FireCircle = 1120,
            Vulcano = 1125,
            FireRing = 1150,
            Bomb = 1160,
            FireofHell = 1165,
            Nectar = 1170,
            AdvancedCure = 1175,
            FireMeteor = 1180,
            SpiritHealing = 1190,
            Meditation = 1195,
            WideStrike = 1250,//to check
            SpeedGun = 1260,
            Golem = 1270,
            WaterElf = 1280,
            Penetration = 1290,
            Halt = 1300,
            FlyingMoon = 1320,
            DivineHare = 1350,
            NightDevil = 1360,
            CruelShade = 3050,
            Dodge = 3080,
            FreezingArrow = 5000,//need to code!!!!!!!!!!!!!!!!!!! Type 16
            SpeedLightning = 5001,
            PoisonousArrows = 5002,
            Snow = 5010,
            StrandedMonster = 5020,// need to code !!!!!!!!!  type 4
            Phoenix = 5030,
            Boom = 5040,
            Boreas = 5050,
            TwofoldBlades = 6000,
            ToxicFog = 6001,
            PoisonStar = 6002,
            CounterKill = 6003,
            ArcherBane = 6004,
            ShurikenEffect = 6009,
            ShurikenVortex = 6010,
            FatalStrike = 6011,
            Seizer = 7000,
            Riding = 7001,
            Spook = 7002,
            WarCry = 7003,
            Earthquake = 7010,
            Rage = 7020,
            Celestial = 7030,
            Roamer = 7040,
            RapidFire = 8000,
            ScatterFire = 8001,
            XpFly = 8002,
            Fly = 8003,
            ArrowRain = 8030,
            Intensify = 9000,
            Bless = 9876,
            AzureShield = 30000,
            ChainBolt = 10309,
            HeavenBlade = 10310,
            StarArrow = 10313,
            DragonWhirl = 10315,
            Perseverance = 10311,
            RadiantPalm = 10381,
            Oblivion = 10390,
            TyrantAura = 10395,
            Serenity = 10400,
            SoulShackle = 10405,
            FendAura = 10410,
            WhirlwindKick = 10415,
            MetalAura = 10420,
            WoodAura = 10421,
            WatherAura = 10422,
            FireAura = 10423,
            EarthAura = 10424,
            Tranquility = 10425,
            Compassion = 10430,
            ShieldBlock = 10470,
            TripleAttack = 10490,
            DragonTail = 11000,
            ViperFang = 11005,
            EagleEye = 11030,
            ScurvyBomb = 11040,
            CannonBarrage = 11050,
            BlackbeardsRage = 11060,
            GaleBomb = 11070,
            KrakensRevenge = 11100,
            BladeTempest = 11110,
            AdrenalineRush = 11130,
            Windstorm = 11140,
            DefensiveStance = 11160,
            BloodyScythe = 11170,
            MortalDrag = 11180,
            ChargingVortex = 11190,
            MagicDefender = 11200,
            GapingWounds = 11230,
            KineticSpark = 11590,
            DaggerStorm = 11600,
            BladeFlurry = 11610,
            PathOfShadow = 11620,
            BlisteringWave = 11650,
            MortalWound = 11660,
            SpiritFocus = 11670,
            SuperCyclone = 11970,
            BreathFocus = 11960,
            FatalCross = 11980,
            MortalStrike = 11990,

            //guards
            SummonFox = 12020,
            SummonCrane = 12030,
            SummonPanda = 12040,
            SummonVoltaicWarg = 12050,

            TwilightDance = 12070,
            SuperTwofoldBlade = 12080,
            ShadowClone = 12090,
            FatalSpin = 12110,

            CrackingSwipe = 12160,
            SplittingSwipe = 12170,

            SpeedKick = 12120,
            ViolentKick = 12130,
            StormKick = 12140,

            AirStrike = 12210,
            EarthSweep = 12220,
            Kick = 12230,

            DragonSwing = 12200,
            DragonPunch = 12240,

            DragonFlow = 12270,
            DragonRoar = 12280,
            DragonCyclone = 12290,
            DragonFury = 12300,

            AirKick = 12320,
            AirSweep = 12330,
            AirRaid = 12340,
            DragonSlash = 12350,

            //epic fire/water
            AuroraLotus = 12370,//water
            FlameLotus = 12380,
            BlessingTouch = 12390,//water
            SearingTouch = 12400,

            //epic monk spells
            InfernalEcho = 12550,
            GraceofHeaven = 12560,
            WrathoftheEmperor = 12570,
            UpSweep = 12580,
            DownSweep = 12590,
            Strike = 12600,
            //--------------

            //epic warrior
            TwistofWar = 12660,
            ScarofEarth = 12670,
            Backfire = 12680,
            WaveofBlood = 12690,
            Pounce = 12770,
            ManiacDance = 12700,

            LeftHook = 12740,
            RightHook = 12750,
            StraightFist = 12760,

            //windwalker
            FrostGazeI = 12830,
            Thundercloud  = 12840,
            TripleBlasts = 12850,
            Omnipotence = 12860,
            SwirlingStorm = 12890,
            JusticeChant = 12870,
            RageofWar = 12930,
            BurntFrost = 12940,
            HealingSnow = 12950,
            ChillingSnow = 12960,
            Thunderbolt = 12970,//<-- de facut!
            AngerofStomper = 12980,
            HorrorofStomper = 12990,
            PeaceofStomper = 13000,
            FreezingPelter = 13020,
            RevengeTail = 13030,
            Sector = 13040,
            Circle = 13050,
            Rectangle = 13060,
            FrostGazeII = 13070,
            FrostGazeIII = 13080,
            ShadowofChaser = 13090,
            ThundercloudAttack = 13190
        }
    }
}
