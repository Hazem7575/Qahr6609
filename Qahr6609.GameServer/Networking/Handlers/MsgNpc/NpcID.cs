
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgNpc
{
    public enum NpcID : uint
    {
    
        ChiToken = 55121,
        BankCps = 52145,
        Huint = 51211,
        MRConquer = 77454,
        MissConquer = 77453,
        ReallotPoints = 52155,
        JiangMaker = 15532,
        VotePointsReward = 32422,
        ChiMaker = 54152,
       EliteGuildOfficer= 770000,
        UnionWar = 44443,
        CouplesPK = 81701,
        GuildWarScore = 22232,
        NobilityWar = 212215,
        ClassPoleWar = 212216,

        UnlimitedArena = 22115,
        BootManager = 21351,
        NewMonster = 21554,
    
        FSConquer = 77452,

        BlackFridayGarmentPack = 20544,
        BlackFridayMountPack = 20545,
        BlackFridayAccesory = 20546, 
        SpouseWar = 20522,
        SpouseWarQ = 20523,
        IronMineAssistant = 42930,
        LostLady = 88888,
        NPCDONATe = 88887,
        PowerFight = 88889,
        GM = 29680,
        SantaStuff = 521509,
        //FiendLair
        FiendLairGuard = 7545,
        FlamePagoda_1_ = 7635,
        FlamePagoda_2_ = 7636,
        FlamePagoda_3_ = 7637,
        FlamePagoda_4_ = 7638,
        FiendLairPortal_1 = 7565,
        FiendLairPortal_2 = 7563,
        FiendLairPortal_3 = 7564,
        FiendLairPortal_4 = 7566,
        //


        //MoonBoxQuest
        Maggie = 600003,
        Ghost_1043_ = 600005,
        Ghost_1046_ = 600011,
        Ghost_1044_ = 600007,
        Ghost_1045_ = 600009,
        Ghost_1048_ = 600015,
        Ghost_1047_ = 600013,
        VagrantGhost_1_ = 600018,
        VagrantGhost_2_ = 600019,
        VagrantGhost_3_ = 600020,
        VagrantGhost_4_ = 600021,
        VagrantGhost_5_ = 600022,
        VagrantGhost_6_ = 600023,
        //
        testtops = 445554,

        CloudBeast = 1503,
        SkillEraserSpecialist = 18713,
        MerchantClerk = 1880,
        CrystalMerchant = 7676,
        FiendLairHerald = 7675,
        GeneralSam = 10295,
        AstralMaster = 18742,
        SashTrader = 2320,
        BlueRock = 8715,
        CPExchangeEnvoy = 19349,
        ScholarWise = 21123,
        FortuneWheelPromoter = 19785,
        BoundItemPromotion = 19350,

        SeniorAssistant = 17573,
        GiftPackRetailer = 30500,
        //newquests-> ["TQCONQUER->  3D "]
        BarbecuePigsy = 8855,
        Bonfire = 8874, //done
        MrsStingy = 77887,
        MusicianDebra = 778899,
        LuckyEggEnvoy = 77115, //done
        SigninOfficerJasper = 9960,
        MrLike = 90547,
        GreatPainterZoe = 22101, //done
        RedPaintbox = 603201, // done
        YellowPaintbox = 441141, //done 
        BluePaintbox = 741470, //done
        MountArmorSpecialist = 96221, //done
        GarmentSpecialist = 99611, //done
     
        ExchangeShop = 19978,
        //Giant Ghsm
        HauKan = 21875,
        VictoryChest = 21972,
        YuKoon2 = 20824,
        CobbWind2 = 20825,
        FBSSPortal = 20860, //20860,1,36,10166,60,126
        GiantChasmPortal1 = 20862,
        GiantChasmPortal2 = 20864,
        GiantChasmPortal3 = 20865,
        GiantChasmPortal4 = 20861,
        GiantChasmPortal5 = 20863,
        RareChest = 21975,
        EpicChest = 21974,
        //

        YuKoon = 20823,
        DragonIslandPharmacist = 20509,
        CobbWind = 20500,
        IronTong = 20499,
        DragonIslandConductress = 20508,
        PotionDealer1 = 20619,
        PotionDealer2 = 20620,
        PotionDealer3 = 20621,
        PotionDealer4 = 20622,
        GlowTownPortal1 = 20571,
        GlowTownPortal2 = 20572,
        GlowTownPortal3 = 20573,


        WindwalkerGuard = 20028,
        ElderPower = 20029,
        SisterYung = 20030,
        SisterYung2 = 200058,
        ElderPower2 = 200124,
        DukeofHell = 200002,

        Matchmaker = 30000,
        PromotionWindWalker = 19634,
        SellItemsInnerPower = 665566,
        JadeCourtesan = 4586,
        DayBox = 4592,
        DarkBox = 4593,
        NightBox = 4594,
        MrLoneliness =4595,
        SparrowPillar = 4587,
        DragonPillar = 4588,
        TigerPillar = 4589,
        LeopardPillar= 4590,
        HerbalistChou = 4584,
        VillagerChou = 4582,
        KunlunWanderer = 4583,
        Platform = 4591,


        Cauldron = 4578,
        BlockStone = 4603,
        Plum = 4572,
        RuanBrother = 4568,
        RuanGoodMan = 4570,
        RuanBetterMan = 4571,
        DreamDoor = 4577,
        SeasonDoor = 4576,
        NetPeddler = 4579,
        YeSheng = 4569,
        TimeDoor = 4575,
        TreasureBox = 4580,
        TreasureBox1 = 4600,
        TreasureBox2 = 4601,
        TreasureBox3 = 4602,
        YangYun2 = 4567,
        YangYun1 = 4566,
        GhoulKong = 4573,
        SugarTang = 4565,

        MrWonder = 10850,

        NaughtyBoy = 8269,

        Celestine = 20005,

        OldBeggar = 4704,

        MasterMoMo = 4702,

        Louis = 4651,
        Shark = 4652,

        MrFree = 3216,

        LeadWrangler = 8616,
        ChiMaster = 7781,
        Sage= 7780,
        Warlock = 7779,
        MartialArtist = 7778,
        ApothecarySubClass = 7777,
        Performer = 7776,

        SubClassManager = 8591,

        ArenaGuard = 4432,
        IronsmithChou = 4434,
        PrizeOfficer2 = 10223,

        BirdExorcist =300010,
        CommanderKerry = 4707,

        CommanderAid1=  4708,
        CommanderAid2 = 4709,
        CommanderAid3 = 4710,
        CommanderAid4 = 4711,

        Felix = 4705,
       GuideZhang =  4706,


        PoorXiao = 4700,
        DoctorKnowitAll = 4701,

        GeneralPakMap3 = 10603,
        MrMirrorMap3  =10604,
            DivineJadeMap3 = 10605,


        GeneralPakMap2 = 10600,
        MrMirrorMap2 = 10601,
        DivineJadeMap2 = 10602,

        EpicTrojanDivineJade = 10606,
        MonkMisery = 10583,
        MrMirror = 10582,
        MrMirror2 = 10619,
        GeneralPak = 10581,

        MonkMisery1 = 10584,
     
        PaksGhost = 10579,

        Epic2MonkMisery = 10585,
        Epic2ArrayEye1 = 10589,

        ServerTransfer = 15702,//15702,2,24150,1002,280,290

        ArenaManagerWang = 1881,
        Agate1 = 1882,
        Agate2 = 1883,
        Agate3 = 1884,
        Agate4 = 1885,
        Agate5 = 1886,
        Agate6 = 1887,
        Agate7 = 1888,

        PrizeCenterTeleporter1 = 9619,
        PrizeCenterTeleporter2 = 9620,
        PrizeCenterTeleporter3 = 9621,
        PrizeCenterTeleporter4 = 9622,

        MammonEnvoy = 9623,

        PrizeCenterTeleporter5 = 9650,
        PrizeCenterTeleporter6 = 9651,
        PrizeCenterTeleporter7 = 9652,
        PrizeCenterTeleporter8 = 9653,

        SquidwardOctopus = 9534,

        FlameAltar = 19165,

      WoundedBrightTribesman=  19183,
        BrightGuard = 19184,

        LavaFlower1 =19239,
 LavaFlower2 =19172,
 LavaFlower3 =19173,
 LavaFlower4 =19174,
 LavaFlower5 =19176,
 LavaFlower6 =19177,
 LavaFlower7 = 19238,

        WhiteHerb1 = 19167,
        WhiteHerb2 = 19168,
        WhiteHerb3 = 19169,
        WhiteHerb4 = 19170,
        WhiteHerb5 = 19171,
        WhiteHerb6 = 19175,

        ChingYan = 19164,
        ChongYan = 19162,
        PakYan = 19160,
        RemainofBrightTribesman = 19161,
        BrokenForgeFurnace = 19163,

        TowerOfMysteryLayerChange =19139,
        TowerofMysteryChallenge1 = 200020,
        TowerofMysteryChallenge2 = 200025,
        TowerofMysteryChallenge3 = 200018,
        TowerofMysteryChallenge4 = 200024,

        CloudSweeper = 19128,
        TowerofMysteryConductor = 19231,
        TowerofMysteryConductor2 = 19194,
        TowerKeeper = 19127,
        GuidofFieryDragon = 19166,


        BronzePhoenixCup = 19504,
        SilverPheoenixCup = 19505,
        GoldenPhoenixCup = 19506,
        HolyPhoenixCup = 19507,
        CSElitePkManager = 19424,
        IdealDesgner = 19436,
        AstralPheonix = 19452,


        BronzePhoenixCup2 = 19879,
        SilverPheoenixCup2 = 78965,
        GoldenPhoenixCup2 = 32101,
        HolyPhoenixCup2 = 19514,
        CSElitePkManager2 = 19370,
        IdealDesgner2 = 20436,
        AstralPheonix2 = 17895,


 
        MoonMaiden = 8296,
        PkReset = 52003,

        Crystal = 19121,
        KingdomMissionEnvoy = 17400,
        RealmEnvoy = 18787,
        twincpadmin = 223311,

          VoteManager = 5555555,



        RechargeShop = 52300,



        ExplosiveDevide1 = 10951,
        ExplosiveDevide2 = 10950,
        ExplosiveDevide3 = 10949,
        ExplosiveDevide4 = 10948,

        Crystal1 = 19121,
        Crystal2 = 19122,
        Crystal3 = 19123,
        Crystal4 = 19124,
        Crystal5 = 19125,



        HellConquer_EU = 52100,
        HellConquer_US = 52101,
        OurCoqnuer_EU = 52102,
        OurCoqnuer_US = 52103,

        PokerMillionaireLee = 19111,

        FarmerLynn = 4718,
        Carolyn = 4719,
        Harvey = 4621,
        AuntPeach = 4633,

        StoneColumn = 4622,
        ObscureWarrior = 4623,
        StoneColumn1 = 4626,
        StoneColumn2 = 4624,
        StoneColumn3 = 4625,
        StoneColumn4 = 4627,


        Dark_MoMo = 4526,
        Dark_SweetyPuddy = 4513,
        Dark_SweetyLily = 4514,
        Dark_SweetyMindy = 4515,
        Dark_SweetyDay = 4516,
        Dark_SweetyCuty = 4517,
        Dark_FiendAltar1 = 4522,
        Dark_FiendAltar2 = 4523,
        Dark_FiendAltar3 = 4524,
        Dark_FiendAltar4 = 4525,


        Ghost_MoMo= 4506,
        Ghost_SweetyPuddy = 4507,
        Ghost_SweetyLily = 4508,
        Ghost_SweetyMindy = 4509,
        Ghost_SweetyDay = 4510,
        Ghost_SweetyCuty = 4511,
        Ghost_FiendAltar1 = 4518,
        Ghost_FiendAltar2 = 4519,
        Ghost_FiendAltar3 = 4520,
        Ghost_FiendAltar4 = 4521,


        TaoistShine = 4505,
        JerkWang = 4504,
        AuntZhang = 4503,
        GeneralArmand = 8539,

        GreenSnake =30115,
        Mulan = 8542,
        Harriet = 8541,
        Alvis =   8538,

        BiVillageHead = 8537,

        AdjutantLi = 8562,
        OldZhang = 8540,
        SaltedFish = 8545,
        FishingNet = 8544,


        SoldiersMother  = 8536,

        SoldierZhang = 8535,

        SouthwestIsland = 8559,
        WestIsland = 8558,
        NorthwestIsland = 8561,
        NorthIsland = 8557,
        NortheastIsland = 8556,
        CentralIsland = 8560,

        CookYuan = 8528,

        SoldierGuan = 8529,
        SoldierXu = 8530,
        SoldierZheng = 8531,

        SoldierFei = 8532,






        MysteryTaoist = 8533,
        TruthTaoist = 8527,

       WhiteChrysanthemum =  8516,
        Jasmine = 8517,
        Lily = 8515,
        WillowLeaf= 8518,


        ViceGeneral = 8534,
        StrangeBox = 8522,
        XimenQing = 8514,

        CityGeneral = 8521,
        WindTaoist = 8519,
        WealthyWan = 8524,
        WealthyWansWife = 8523,
        LittleBen = 8520,
        ArmorerYu = 8513,
        IslandGeneral = 8525,
        
        BIViceCaptain =8512,
        Xi_er = 8526,
 
        XuFan = 8511,
        BICastellan = 8510,

        WitheredTree1 = 8429,
        WitheredTree2 = 8430,
        WitheredTree3 = 8431,

        Lotus = 8428,
        SpringSoldierZhao = 8449,
        SoldierMu = 8442,
        TaoistSpring = 8448,

        ElderJiang = 8450,
       ConvoyViceLeader = 8444,

        SpringViceGeneralOu = 8446,

        SculptorHe = 84477,
        SpringGeneralXu = 8445,
        ConvoyLeaderGu = 8443,
        HanCheng = 8441,
        st1TreeSeed = 8451,
        nd2TreeSeed = 8452,
        rd3TreeSeed = 8453,

        DCWanYing = 8440,
       // DCGeneralZhuGe =8442,
        IronsmithLi = 8438,
        ViceGeneralDong = 8439,

        KeYulunsFollower = 8435,
       KeYulun = 8434,
   

        VipQuest = 52002,

        Arthur = 3601,
        DCViceGenera= 8426,
        TaoistYu = 8427,
        GeneralZhuGe = 8437,
       DCViceCaptain = 8425,
        HeresySnakemanLeader =30105,
        MountRetailer = 5517,
        CaptorCooke = 8298,
        SnakemanLeader = 30101,
        HarmonyTaoist = 8297,

        ArdenteTaoist = 8293,
        TempestTaoist = 8294,
        BreezePupil =8295,
        CliffFlower1 = 8288,
        CliffFlower2 = 8300,
        Apothecary = 8304,
        PoisonMaster = 8299,

        KingOfTheHill = 52006,
    SkillTournament = 52005,

        FreezeWar = 52004,
        Football = 52001,
        OldQuarrier = 422,
        Norbert = 421,

        //task manager
        ThiefWong = 4436,
        ThiefChen = 4437,
        VeteranHong = 4464,
        ZhaoJian = 4465,
        ScholarWu = 4482,
        PharmacistMuMu = 4483,
        BianQing = 4484,
        ApprenticeLuo = 4466,
        Minstrel = 4500,
        DealerShen = 4501,
        ShenJunior = 4502,
        OfficerBao = 4485,
        DoctorLi = 4486,
        GeneralAmber = 4487,
        PainterFengKang =4489,
        GuardLi =4490,
        DuSan= 4468,
        GeneralJudd = 1611,
        PharmacistDong = 4470,


      
        ACCastellan = 8590,
        Lydia = 8289,
        StoneApe = 8290,
        CarpenterJack = 8291,
        JackDaniel = 8292,

        AC_Lieutenant = 8287,

        Revenant1 = 4491,
        Revenant2 = 4492,
        Revenant3 = 4493,
        Revenant4 = 4494,
        Revenant5 = 4495,

        CrystalRiddler = 3664,

        WuxingOven = 35016,

        GreatMerchant = 432,

        RedPowder =4471,
        OrangePowder = 4472,
        YellowPowder = 4473,
        GreenPowder = 4474,
        BluePowder = 4475,
        IndigoPowder = 4476,
        PurplePowder = 4477,


        TreasureThief= 52000,
        TreasureLeave =4026320,

        SuperGuildWar= 25015,
        PokerMillinaireLee = 19111,
        PokerCasinoHostess = 6298,
        PokerWarehouseman = 7762,
        PokerCpsCasino = 6299,

        ArtisanLuo = 4467,
        XuLiang = 7992,
        KungFuKing = 17222,
        TCViceCaptain =  7991,
        PeachTree = 7997,
        PeachTree2 = 7996,
        RuHua = 7993,
        Fortuneteller = 600050,
        MasterHao = 7994,
        YuLin = 7995,
        YuJing = 8267,
        ForestGuvernor = 8266,
        IntelligenceAgent = 8270,
        BanditBoss = 8272,//,2,4406,1011,227,404
        PhoenixCastellan = 8271,
        VillageHead = 8273,
        BoldBoy = 8274,
        FurDealer = 8275,
        HunterWong = 8268,
        WineKiddo = 4596,
        MilitiaTiger = 8278,
        MilitiaDragon =8279,
        MilitiaCaptain = 8277,
        MilitiaLeopard = 8280,
        WarrantOfArrest = 8281,
        

        NameRegister = 7935,
        DrYinYang =  15805,

        //daily
        YanDi = 9894,
        Censer1 = 9954,
        Nuwa = 9892,
        Censer2 = 9953,
        HuangDi = 9893,
        Cense3 = 9952,
        GoodManLiu = 9891,
        DailyQuestEnvoy = 9998,
        //
        DesertGuardian = 301,
        MagnoliaEnvoy = 9897,
        HeavenlyMaster = 8233,

        EartMaster = 200012,
        //
        FairyCloud = 16050,
        MysterCheast1 = 16051,
        MysterCheast2 = 16052,
        MysterCheast3 = 16053,
        MysterCheast4 = 16054,
        MysterCheast5 = 16055,

        //-------------------------


        //epic quest taoist------
        RefinedPureOven = 25010,
        ElitePureOver = 25011,
        SuperPureOver = 25012,
        SunKing = 25013,
        PureTaoist = 25014,
        //------------------------
        //epic quest monk------------
        FortuneArhat = 25001,
        AltarCleanser = 25002,
        VicotryBuddha = 25003,
        WhiteDragon = 25004,
        GoldenCicada = 25005,
        

        //------------------
        InnerPowerNpc = 18786,
        Shelby = 300000,
       TcBoxer =  101611,
ApeBoxer = 101623,
DesertBoxer= 101625,
PheonixBoxer =101627,
BirdBoxer= 101629,

        TCCaptain = 2001,
        PCCaptain = 2002,
        AMCaptain = 2003,
        DCCaptain = 2004,
        BCaptain = 2005,
        MCaptain = 2006,

        SkillTeamPkManager = 8592,
        TeamPkManager = 8158,

        SteedRace = 6001,
        KillerOfElite =780,
        ExtremePk = 4699,
        TeamDeathMatch = 777,
        DragonWar = 778,
        LastManStand = 779,


        TwinDisCityMain = 3215,
        DisCityMain = 9280,
        DisCityMap1 = 4026314,
        DisCityMap2 = 4026315,
        DicCityMap4 = 4026316,
        ServerRewards = 781,
        DBShower = 666,

        PkWar = 31,
        PkWarQuit = 4026313,

        CaptureTheFlag = 8713,
        EquipmentBlacksmith = 9071,

        GodlyArtisan = 10065,

        BattleFieldMain = 10123,
        BattleFieldMap1 = 4026308,
        BattleFieldMap2 = 4026309,
        BattleFieldMap3 = 4026310,

        GWJail = 4026307,

        ExitElitePk = 168052,
        StartNpc = 4026306, //4026306,2,190,1010,88,42
        ItemShops = 88088,
        ItemShopsTC = 88778877,
        PrizeOfficer = 47,
        MarketCpAdmin = 2071,
        TaskmasterChang =8796,
        SellItems = 30015,
        SellItemsEpic = 96321,
        MarialDealer = 15987,
        JailJoin = 43,
        JailWarden = 42,
        WardenZhang = 1002,

        JailCPAdmin = 1001,

        LoveStone = 390,
        OflineTGNpc = 3836,
        ClanNpc = 5533,


        KitMerchant = 7863,
        SkillEraser = 15988,
        FrozenGrottoGeneral = 6144,


        TwinCityConductress = 10050,
        PheonixCityConductress = 10052,
        DesertCityConductress = 10051,
        ApeCityConductress = 10053,
        BirdCityConductress = 10056,
        MarketConductress = 45,
        BoxerConductor = 180,
        WHTwin = 8,
        WHMarket = 44,
        wHPheonix = 10012,
        WHDesert = 10011,
        WHApe = 10028,
        WHBird = 10027,
        WHPoker =7762,
        WHStone = 4101,

        BlackSmith = 5,//5,32,50,1002,324,230
        TheStorekeeper = 1,
        Pharmacist = 3,
        //Pharmacists = 88,
        Armorer = 4,
        KungfuBoy = 5673,
        KungfuMaster = 4488,
        Barber = 10002,
        GuildCreator = 10003,

        JiangHuNpc = 15745,
        PromotionLeeLong = 17126,
        PromotionPirate = 9391,
        PromotionTrojan = 10022,
        PromotionArcher = 400,
        PromotionWarrior = 10001,
        PromotionNinja = 4972,
        PromotionMonk = 8314,
        PromotionTaoist = 10000,
        MountTrainer = 5600,
        TeratoTwin = 7927,

        RebirthMaster = 9072,

        MarketLadyLuck = 923,
        LotteryLadyLuck = 924,
        LotteryCollectorWong = 3952,
        CollectorZhao = 2070,
        UnknowMan = 3825,
        MillionaireLee = 5004,
        GarmentShopKeeper = 6002,
        ShopKeepers = 90080,//shopmounts
        ShopType = 98880,

        Confiscator = 4450,
        JailConfiscator = 4542,
        CloudSaint = 2000,

        MasterXuan = 3085,
        SurgeonMiracle = 3381,
        Costumer = 683,
        ArenaNpc = 10021,
        Shopboy = 10063,
        Tinter = 832662,
        FurnitureStore = 30161,
        HouseAdmin = 30156,
        Class6House =18950,


        GuildConductor = 380,
        GuildGateKeeper = 7000,

        FlameTaoist = 4452,

        Flame1 = 4453,
        Flame2 = 4454,
        Flame3 = 4455,
        Flame4 = 4456,
        Flame5 = 4457,
        Flame6 = 4458,
        Flame7 = 4459,
        Flame8 = 4460,
        Flame9 = 4461,
        Flame10 = 4462,

        RightGate = 516075,
        LeftGate = 516074,


        SuperGuildWar_CasteleRightGate = 516077,
        SuperGuildWar_CasteleLeftGate = 516076,
        SuperGuildWar_LeftGate = 516078,
        SuperGuildWar_MidleGate = 516079,
        SuperGuildWar_RightGate = 516080,

        ExitSuperGuildWar= 4026311,
        ExitCaptureTheFlag = 4026312,

        GuildConductor1 = 101621,
        TeleGuild1 = 101620,
        GuildConductor2 = 101619,
        TeleGuild2 = 101618,
        GuildConductor3 = 101617,
        TeleGuild3 = 101616,
        GuildConductor4 = 101615,
        TeleGuild4 = 101614,
        GuildOfficer = 101897,
        //GuildWars = 506050, not work

        DesertFrozenGroto = 6135,
        FrozenFroto4Teleporter = 13124,

        FrozenGrottoGuardian5 = 13125,


        Simon = 1152,
        SoulPacks = 65950, 
        MoneyPacks = 654520,//654520,MoneyPacks,2,44785,5000,66,33,0,0,1,1


        Lab1 = 1153,
        Lab2 = 1154,
        Lab3 = 1155,
        Lab4 = 1156,

        CasinoHostess = 16915,
        GameManager = 6297,

        Stanley = 3623,
        SwapperStarry = 832663,
        ElementalPool = 832664,
        P7SacredTreasuresEnvoy = 16849,
        NemesysConductort = 9464,

        SarckSoldier = 16833,
        SarckSoldierEnctrance = 16834,
        SarckSoldierSuppluPoint1 = 16852,
        SarckSoldierSuppluPoint2 = 16853,
        SarckSoldierSuppluPoint3 = 16854,
        SarckSoldierSuppluPoint4 = 16855,
        SarckSoldierSuppluPoint5 = 16856,

        SuperMok = 10580,
        DivineJade = 16850,

        ClassPkEnvoy = 662,
        ClassPkWar = 16851,

        ElitePk = 7879,

        SelectP7WeaponSoulPack = 556644,//uint.MaxValue - 1
        SelectP7EquipmentSoulPack = uint.MaxValue - 2,
        SelectSacredRefineryPack = uint.MaxValue - 3,
        Steed1 = uint.MaxValue - 4,
        Steed3 = uint.MaxValue - 5,
        Steed6 = uint.MaxValue - 6,
        DailyItem1 = uint.MaxValue - 7,
        DailyNormalSpiritBead = uint.MaxValue - 8,
        DailyRefinedSpiritBead = uint.MaxValue - 9,
        DailyUniqueSpiritBead = uint.MaxValue - 10,
        DailyEliteSpiritBead = uint.MaxValue - 11,
        DailySuperSpiritBead = uint.MaxValue - 12,
        Level43UniqueRingPack = uint.MaxValue - 13,
        NobleSteedPack = uint.MaxValue - 14,
        RareSteedPack6 =  uint.MaxValue - 15,
        DazzlingDiamondBox = UInt32.MaxValue - 16,//UInt32.MaxValue - 16
        TempestSecretLetter = uint.MaxValue - 17,
        SashFragment_Realm = uint.MaxValue - 18,
        GarmentPacket = 77788777,//uint.MaxValue - 19,
        MountPacket = uint.MaxValue - 20, //uint.MaxValue - 20,
        MountPacket2 = 556455,//uint.MaxValue - 21
        AccesoryPacket = uint.MaxValue - 22,
        GarmentPacket2 = uint.MaxValue - 23,
        AccesoryPacket2 = uint.MaxValue - 24,
        MountPacket3 = uint.MaxValue - 25,
        GoldPrizeToken = uint.MaxValue - 26, //uint.MaxValue - 26,


        Steed1Pack = uint.MaxValue - 30,
        Steed3Pack = uint.MaxValue - 31,

        HeavenDemonBox = uint.MaxValue - 32, //uint.MaxValue - 32
        ChaosDemonBox = 50560,//uint.MaxValue
        SacredDemonBox = uint.MaxValue - 34,//uint.MaxValue - 34
        AuroraDemonBox = uint.MaxValue - 35,//uint.MaxValue - 35
        DemonBox = 8165,//uint.MaxValue - 36
        AncientDemonBox = uint.MaxValue - 37,//uint.MaxValue - 37
        FloodDemonBox = uint.MaxValue - 38,//uint.MaxValue - 38
        SuperHeadgearPack = uint.MaxValue - 39,//uint.MaxValue - 39
        RingPack = uint.MaxValue - 40,
        ClothingPack = uint.MaxValue - 41,
        PowerBook = uint.MaxValue - 42,


        Level50UniqueWeaponPack = uint.MaxValue - 43,
        Level52UniqueHeadgearPack = uint.MaxValue - 44,
        Level55EliteWeaponPack = uint.MaxValue - 45,
        Level67EliteHeadgearPack = uint.MaxValue - 46,
        L60UniqueGearPack = uint.MaxValue - 47,

        Joyful1StonePack = uint.MaxValue - 48,
        Joyful2StonePack = uint.MaxValue - 49,
        Joyful3StonePack = uint.MaxValue - 50,
        Joyful4StonePack = uint.MaxValue - 51,
        Joyful5StonePack = uint.MaxValue - 52,
    }
}
