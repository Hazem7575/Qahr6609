using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game
{
    public class GamePackets 
    {
        public const ushort
            MsgGlobalLottery = 3282,
        MsgGlobalLotteryRankList = 3283,

            MsgTexasExInteractive = 3400,
         MsgTexasExChampionshipList = 3402,
        MsgTexasPersonalInfo = 3403,
        MsgTexasExChampionshipRank = 3404,
        MsgTexasExMyChampionshipRank = 3405,
        MsgTexasChampionshipTableChip = 3406,
        MsgPCNum = 1100,
            MsgSuperFlag = 2110,
         Update = 10017,
         SpawnPlayer = 10014,
         DataMap = 10010,
         Movement = 10005,
         MsgMagicColdTime = 1153,
         MsgTick = 1012,
          Weather = 1016,
        CMsgPCServerConfig = 1049,

         BuyFromExchangeShop = 2443,
      
         ExchangeShop = 2441,
         ArenaCrossServer = 2507,

         MsgItemRefineOpt = 3251,
         MsgUserTotalRefineLev= 3252,
         MsgUserAbilityScore = 3253,
         MsgBestOfTheWorld = 3257,
         MsgItemRefineRecord = 3255,
         MsgRefineEffect = 3254,


    
         MsgFamilyOccupy = 1313,
MsgSignIn = 3200,
MsgCoatStorage = 3300,
MsgTitleStorage = 3301,
CMsgMagicEffectTime = 1152,
MsgEquipRefineRank = 3256,

MsgOsShop = 3002,

         MsgHeroRewardsInfo = 2831,
          MsgHeroRewardsInfoDone = 2832,
         MsgHeroReward = 2830,
        
         MsgActivityTasks = 2820,
         MsgActivityRewardsInfo = 2823,
         MsgActivityClaims = 2822,

         MsgPetAttack = 2812,
         MsgTaskReward = 2811,

         MsgRouletteShareBetting = 2810,
         MsgRoulettedAddNewPlayer = 2809,
         MsgRouletteOpenGui = 2808,
         MsgRouletteTable = 2807,
         MsgRouletteSignUp = 2805,
         MsgRouletteAction = 2804,
         MsgRouletteScreen = 2803,
         MsgRouletteRecord = 2802,
         MsgRouletteNoWinner = 2801,
         MsgRouletteCheck = 2800,

         MsgJiangMessage = 2710,
         UnKnowwPP = 2711,
         JiangHuPkMode = 2704,
         JiangHuRank = 2703,
         JiangHuUpdates = 2702,
         JiangHuStatus = 2701,
         JiangHu = 2700,



         MsgSameGroupServerList = 2500,
     
        
        
         // Union/Kingdom packets use the official 6609 ids (the client sends 2622 for MsgLeagueOpt). This source
         // came with ids from a modified client (26222, 26232 ... 30012 = official id * 10 + 2) that the 6609
         // client does not know. MainRank: 2631 is MsgOverheadLeagueInfo, 2632 is the free id of the range.
         LeagueOpt = 2622,
         LeagueInfo = 2623,
         LeagueAllegianceList = 2624,
         LeagueSynList = 2625,
         LeagueMemList = 2626,
         LeagueRank = 2627,
         LeaguePalaceGuardsList = 2628,
         LeagueImperialCourtList = 2629,
         LeagueGroupTokens = 2630,
         LeagueMainRank = 2632,
         LeagueToken = 2633,
         LeagueConcubines = 2634,
         LeagueRobOpt = 2642,
         LeagueRobList = 2640,// Loot / Reinforce token target list (protobuf)
         LeagueBeRob = 2641,// Plunder War enemy/ally union lists (protobuf)
         TokenUpdate = 2643,// union token issue count / timer (protobuf)
         MsgHandBrickInfo = 3001,
  
        
         MsgOverheadLeagueInfo = 2631,


         InnerPowerStageInfo = 2611,
         InnerPowerGui = 2612,
         InnerPowerHandler = 2610,

         MsgGoldLeaguePoint = 2600,

         MsgTrainingVitalityProtect = 2536,
         MsgTrainingVitalityProtectInfo = 2537,
         MsgTrainingVitalityExpiryNotify = 2538,

         ChiInfo = 2534,
         ChiMessage = 2535,
         HandleChi = 2533,
         MsgInterServerIdentifier = 2501,
         CountryFlag = 2430,
         FlagIcon = 2410,
         SubClass = 2320,

         SecondaryPassword = 2261,

         SkillEliteSetTeamName = 2260,
         SkillElitePkTop = 2253,
         SkillElitePkBrackets = 2252,
         SkillElitePKMatchStats = 2251,
         SkillElitePKMatchUI = 2250,

         MsgTeamArenaInfoPlayers = 2247,
         MsgTeamArenaMatchScore = 2246,
         MsgTeamArenaInfo = 2245,
         MsgTeamArenaRank10 = 2244,
         MsgTeamArenaRanking = 2243,
         MsgTeamArenaMatches = 2242,
         MsgTeamArenaSignup = 2241,

         TeamEliteSetTeamName = 2240,
         TeamElitePkTop = 2233,
         TeamElitePkBrackets = 2232,
         TeamElitePKMatchStats = 2231,
         TeamElitePKMatchUI = 2230,

         ReceiveRecruit = 2227,
         Advertise = 2226,
         AdvertiseGui = 2225,
         Recruit = 2225,
         MsgCaptureTheFlagUpdate = 2224,
         EliteRanks = 2223,
         PkExploit = 2220,
        
         MsgElitePKMatchStats = 2222,
         MsgElitePk = 2219,
         ElitePKMatchUI = 2218,
         MsgElitePkWatch = 2211,

         MsgArenaWatchers = 2211,
         MsgArenaMatchScore = 2210,
            MsgArenaInfo = 2209,
         MsgArenaRank10 = 2208,
         MsgArenaRanking = 2207,
         MsgArenaMatches = 2206,
         MsgArenaSignup = 2205,
         

         FastArsenal = 2204,

         GetArsenal = 2203,
         PageArsenal = 2202,
         ArsenalInfo = 2201,

         
         MsgShowHandKick = 2088,
         PokerDrawCards = 2091,
         PokerPlayerTurn = 2092,
         PokerHand = 2093,     
         PokerShowAllCards = 2094,
         PokerRoundResult = 2095,
         PokerLeaveTable = 2096,
         PokerPlayerInfo = 2090,
         MsgShowHandLostInfo = 2098,
         MsgShowHandOnlineStatus = 2097,
         MsgShowHandTrusteeship = 2099,
         MsgShowHandRaceInteractive = 2089,
         PokerUpdateTableLocation = 2171,
         PokerTable = 2172,

         
         GuildMembers = 2102,
         GuildRanks = 2101,
         Blackspot = 2081,
         NameChange = 2080,
         ExtraItem = 2077,
         AddExtra = 2076,
         GameUpdate = 2075,
         RacePotion = 2072,
         PopupInfo = 2071,
         Fairy = 2070,
         MentorPrize = 2067,
         QuizShow = 2068,
         MentorInfomation = 2066,
         MentorAndApprentice = 2065,
         Nobility = 2064,

         MsgPetInfo = 2035,
         MsgBroadcast = 2050,
         MsgBroadcastliest = 2051,
         ItemLock = 2048,

         TratePartnerInfo = 2047,
         TradePartner = 2046,
         LeaderSchip = 2045,
         OfflineTG = 2044,
         OfflineTGStats = 2043,
         MapTraps = 2400,
         Compose = 2036,
         KnowPersInfo = 2033,
         NpcServerRequest = 2032,
         NpcServerReplay = 2031,
         NpcSpawn = 2030,

         MsgMachineResponse = 1352,
         MsgMachine = 1351,
         MsgLottery = 1314,
         MsgOperatingAct = 1315,
         MsgOperatingActInfo = 1316,
         Clan = 1312,
         FlowerPacket = 1150,
         GenericRanking = 1151,
         Achievement = 1136,
         QuestData = 1135,
         QuestList = 1134,
         Title = 1130,
         MsgVip = 1129,
         MsgVipHandler = 1128,

         Enlight = 1127,
         MsgStaticMessage = 1126,
         Authentificaton = 1124,
         InterAction = 1114,
         MapStaus =1110,
         SobNpcs = 1109,
         ItemView = 1108,
         ProcesGuild = 1107,
         Guild = 1106,
         SpellUse = 1105,
             UpgradeSpellExperience = 1104,
         Spell = 1103,
         Warehause = 1102,
         FloorMap = 1101,
         AutoHunt = 1070,
         RaceRecord = 1071,
         Reincarnation =1066,
         ElitePKWagersList = 1065,
         ElitePKWager = 1064,
         CaptureTheFlagRankings = 1063,
         GuildMinDonations = 1061,
         GuildInfo = 1058,
         Trade = 1056,
         LoginGame = 1052,
         MapLoading = 1044,
         MsgStauts = 1040,
         Stabilization = 1038,
         ClainInfo = 1036,
         DetainedItem =1034,
         ServerInfo = 1033,
         EmbedSocket =1027,
         TeamMemberInfo = 1026,
         Proficiency = 1025,
         AtributeSet = 1024,
         Team = 1023,
         Attack = 1022,
         KnowPersons = 1019,
         String_ = 1015,
         Usage = 1009,
         Item = 1008,
         HeroInfo = 1006,
         Chat = 1004,

         MsgMailOperation = 1045,
         MsgMailList = 1046,
         MsgMailNotify = 1047,
         MsgMailContent = 1048,

         NewClient = 1001;
    }
}
