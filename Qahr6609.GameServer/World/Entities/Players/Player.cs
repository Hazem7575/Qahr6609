using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;
using Conquer6609.Database;

namespace Conquer6609.Role
{
    public unsafe partial class Player : IMapObj
    {

        public uint PokerTableID { get; set; }
        public Extensions.Time32 Changetamp = new Extensions.Time32();
        public byte PokerSeat { get; set; }
        public string Mac = "";
        public byte CPBoundPack = 0;
        public bool ClaimedBCPToday = false;
        public bool Mining = false;
        public Extensions.Time32 NextMine;
        public uint MiningAttempts = 140;
        public DateTime LeftDoorStamp, RightDoorStamp;
        public byte AukKilled = 0;
        public byte FateFruitUsed = 0;
        public uint ExchangeNormalAvaliability = 0, ExchangehighAvaliability = 0, AnswerdToday = 0;
        public int[] WrongAnswers = new int[5] { 0, 0, 0, 0, 0 };
        public byte nextquizop = 0, currentquizop = 0;
        public bool JustCreated = false;
        public uint DragonIslandDamage = 0;
        public uint GiantGhasomKill = 0;
        public bool GiantGhasomKillClaimed = false;
        public bool Teampk = false;
        public bool CanPlaySSFB = false;
        public enum RechargeType : ulong
        {
            None = 0,
            OneGarment = 1u << 0,
            TwoVipTokens = 1u << 1,
            OneMount = 1u << 2,
            InnerPower1000 = 1u << 3,
            FourVipTokens = 1u << 4,
            ConquerPoints225 = 1u << 5,
            InnerPower1500 = 1u << 6,
            InnerPower5000 = 1u << 7,
            OneMount2 = 1u << 8,
            OneRareAccesory = 1u << 9,
            OneGerment2 = 1u << 10,
            InnerPower5000_2 = 1u << 11,
            ConquerPoints500 = 1u << 12,
            OneSoulP7 = 1u << 13,
            OneRareMaterial = 1u << 14,
            TenVip6Tokens = 1u << 15,
            OneRareAccesory_2 = 1u << 16,
            InnerPower5000_3 = 1u << 17,
            OneMount3 = 1u << 18,
            GoldPrize = 1u << 19,
            InnerPower10000 = 1u << 20
        }

        public byte ChargeChi = 0;
        public byte ChargeJaing = 0;
        private uint chargec;
        public Client.GameClient AttackerScarofEarthl;
        public Database.MagicType.Magic ScarofEarthl;

        public Extensions.Time32 UseLayTrap = new Extensions.Time32();

        public Extensions.Time32 EarthStamp = new Extensions.Time32();
        public DateTime CanChangeWindWalkerFree = DateTime.Now;
        public Extensions.Time32 FanRecoverStamin = new Extensions.Time32();

        public Extensions.Time32 HuntingBounsStamp = new Extensions.Time32();

        public int RevengeTailChange = 0;

        public byte UseChiToken = 0;
        public uint HuntingBounsLayer;
        public DateTime MerchantApplicationEnd;
        private uint _merchant = 0;

        public bool OnRemoveLukyAmulet = false;
        public bool OnBluedBird = false;
        public DateTime BlueBirdPlumeStamp = new DateTime();

        public bool OnFerentPill { get { return ContainFlag(MsgUpdate.Flags.Poisoned); } }
        public DateTime FerventPill = new DateTime();

        public byte LuiseQuestions = 0;
        public DateTime RealPortraitStamp = new DateTime();
        public DateTime RevealVialStamp = new DateTime();
        public int GiveFlowersToPerformer = 0;

        public DateTime GallbladerrStamp = new DateTime();

        public byte MonkMiseryTransforming = 0;

        public uint[] EpicTrojanItemChange = new uint[2];
        public byte StageEpicTrojanQuest = 0;
        public uint ChangeEpicTrojan = 0;
        public byte CanChangeEpicMaterial = 1;

        public uint EpicTrojanEvilArrayPoints = 0;
        public uint EpicTrojanAbysalStage = 0;

        public uint ChangeArrayEpicTrojan = 0;
        public byte CanChangeArrayEpicMaterial = 1;
        public uint[] EpicTrojanArrayItemChange = new uint[2];

        public byte EpicTrijanKillGhostReaver = 0;
        public uint[] EpicTrojanMr_MirrorItemChange = new uint[2];
        public uint ChangeMr_MirrorEpicTrojan = 0;
        public byte CanChangeMr_MirrorEpicMaterial = 1;


        public uint[] EpicTrojanGeneralPakItemChange = new uint[2];
        public uint ChangeGeneralPakEpicTrojan = 0;
        public byte CanChangGeneralPakMaterial = 1;

        public byte EpicTrojanMrMirrorPrograss = 0;

        public int DefeatedArenaGuardians = 0;

        public DateTime JoinPowerArenaStamp = new DateTime();


        public DateTime JoinPrizeNpcOctopus = new DateTime();

        public bool TOM_StartChallenge = false;
        public bool TOM_FinishChallenge = false;

        public byte ClaimTowerAmulets = 0;//reset
        public DateTime TowerAmuletStamp = new DateTime();


        public byte TOMClaimTeamReward = 0;//reset
        public DateTime TowerOfMysteryFrezeeStamp = new DateTime();
        public byte JoinTowerOfMysteryLayer = 0;
        public byte MyTowerOfMysteryLayer = 0;//save
        public byte MyTowerOfMysteryLayerElite = 0;//save
        public byte TowerOfMysterychallenge = 3;//reset to 3
        public bool CanSweapTowerOfMystery = false;

        //public uint TowerOfMysteryChallengeFlag = 9;//reset
        public uint TowerOfMysteryChallengeFlag = 0;//reset -> working 
        public byte TOMSelectChallengeToday = 0;//reset
        public byte TOMChallengeToday = 0;//reset
        public uint TOMRefreshReward = 0;//reset
        public Game.MsgTournaments.MsgTowerOfMystery.RewardTypes TOM_Reward = Game.MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal;

        public byte TOM_SellectOptionC = 0;

        public byte OpenHousePack = 0;
        public string TransferToServer = "";

        public uint InitTransfer = 0;
        public bool CheckTransfer = false;
        public bool OnTransfer = false;

        public ushort ExtraAtributes = 0;

        public uint BuyItemS = 0;

        public RechargeType RechargeProgress = RechargeType.None;
        public uint RechargePoints = 0;

        public byte BuyKingdomDeeds = 0;
        public uint KingDomDeeds = 0;

        public ushort ServerID = 0;

        public ushort SetLocationType = 0;
        public Role.Instance.Union MyUnion = null;
        public Role.Instance.Union.Member UnionMemeber = null;

        public DateTime SickleStamp2 = new DateTime();

        public DateTime StampJump = new DateTime();
        public int StampJumpMilisecounds = 0;
        public Extensions.Time32 ManiacDanceStamp = new Extensions.Time32();

        public DateTime KingOfTheHillStamp = new DateTime();
        public uint KingOfTheHillScore = 0;
        public uint SkillTournamentLifes = 0;

        public const ushort MaxInventorySashCount = 300;
        public ushort InventorySashCount = 0;
        public bool Invisible = false;
        public DateTime StampSecorSpells = new DateTime();
        public DateTime StampBloodyScytle = new DateTime();
        public DateTime MedicineStamp = new DateTime();
        public Game.MsgTournaments.MsgFreezeWar.Team.TeamType FreezeTeamType;
        public uint ReceiveTest = 0;
        public DateTime ReceivePing = new DateTime();



        public uint AtiveQuestApe = 0;

        public uint QuestCaptureType = 0;
        public Random MyRandom = new Random(Program.GetRandom.Next());
        public DateTime SickleStamp = new DateTime();
        public int FootballTeamID = 0;
        public bool OnAutoHunt = false;
        public ulong AutoHuntExp = 0;
        public uint FootBallMatchPoints = 0;
        public uint MyFootBallPoints = 0;

        public ulong DailySignUpDays = 0;
        public byte DailySignUpRewards = 0;
        public byte DailyMonth = 0;
        public ushort LateSignIn = 0;

        public uint TaskReward = 0;
        public uint TaskRewardIndex = 0;
        public uint CountSpeedHack = 0;

        public ushort CountryID = 0;

        public Extensions.Time32 WindWalkerEffect = new Extensions.Time32();

        public enum MainFlagType : uint
        {
            None = 0,
            CanClaim = 1 << 0,
            ShowSpecialItems = 1 << 1,
            ClaimGift = 1 << 2,
            OnMeleeAttack = 1 << 3,
        }
        public MainFlagType MainFlag = 0;
        public ushort NameEditCount = 0;
        public uint CurrentTreasureBoxes = 0;
        public DateTime TaskQuestTimer = new DateTime();
        public uint QuestMultiple = 0;

        public DateTime AzurePillStamp = new DateTime();
        public List<MsgTitleStorage.TitleType> SpecialTitles = new List<MsgTitleStorage.TitleType>();
        public uint SpecialTitleID = 0;
        public uint SpecialWingID = 0;
        public uint SpecialTitleScore = 0;




        public bool StartVote = false;
        public Extensions.Time32 StartVoteStamp = new Extensions.Time32();

        public bool OnAttackPotion = false;
        public Extensions.Time32 OnAttackPotionStamp = new Extensions.Time32();

        public bool OnDefensePotion = false;
        public Extensions.Time32 OnDefensePotionStamp = new Extensions.Time32();


        public bool WaveofBlood = false;
        public DateTime WaveofBloodStamp = new DateTime();

        public bool AllowDynamic { get; set; }
        public uint DailyMagnoliaChance = 0;
        public uint DailyMagnoliaItemId = 0;

        public uint DailySpiritBeadCount = 0;

        public uint DailySpiritBeadItem = 0;
        public uint DailyRareChance = 0;
        public uint DailyHeavenChance = 0;

        public uint TodayChampionPoints = 0;
        public uint HistoryChampionPoints = 0;

        uint _ChampionPoints;

        public Extensions.Time32 PickStamp = Extensions.Time32.Now;
        public bool ActivePick = false;

        public uint EpicQuestChance = 0;
        public uint EpicQuestTwo_ndChance = 0;
        public uint EpicQuestThree_rdChance = 0;

        public uint IndexInScreen { get; set; }
        public bool IsTrap() { return false; }

        public byte Away = 0;
        public uint OnlineMinutes = 0;
        public Extensions.Time32 OnlineStamp = Extensions.Time32.Now;

        public Role.Instance.InnerPower InnerPower;

        public Game.MsgTournaments.MsgSteedRace.UsableRacePotion[] RacePotions = null;

        public uint KillerPkPoints = 0;
        public uint XtremePkPoints = 0;

        public uint DragonWarHits = 0;
        public uint DragonWarScore = 0;



        public uint TeamDeathMacthKills = 0;
        public uint TournamentKills = 0;

        public uint SpecialGarment = 0;

        public Extensions.Time32 StampArenaScore = new Extensions.Time32();

        public uint HitShoot = 0;
        public uint MisShoot = 0;
        public uint ArenaDeads = 0;
        public uint ArenaKills = 0;

        public Extensions.Time32 StampSFScore = new Extensions.Time32();

        public uint HitSBShoot = 0;
        public uint MisSBShoot = 0;
        public uint ArenaSBDeads = 0;
        public uint ArenaSBKills = 0;
        public uint KillersDisCity = 0;


        public byte TaoistPower = 0;
        public Extensions.Time32 TaoistStampPower = Extensions.Time32.Now;

        public uint AparenceType = 0;
        public uint TCCaptainTimes = 0;
        public bool IsCheckedPass = false;
        public uint SecurityPassword = 0;
        public uint OnReset = 0;
        
        public DateTime ResetSecurityPassowrd = new DateTime();

        public Extensions.Time32 LoginTimer = Extensions.Time32.Now;

        public bool InElitePk = false;
        public bool InTeamPk = false;

        public uint RacePoints = 0;
        public uint SteedRaceBestTime = 0;// Horse Racing: this player's fastest finish in ms (0 = none), saved as Character/SteedRaceBest
        public uint BattleFieldPoints;


        public List<byte> Titles = new List<byte>();
        public unsafe byte MyTitle;

        public Flags.PKMode PreviousPkMode = Flags.PKMode.Capture;
        public DateTime EnlightenTime = new DateTime();

        public int CursedTimer = 0;




        public uint MyKillerUID;
        public string MyKillerName;

        public bool Delete = false;


        public uint testtttttttttt = 0;


        public Extensions.MyList<Clone> MyClones = new Extensions.MyList<Clone>();

        public ConcurrentDictionary<ushort, FloorSpell.ClientFloorSpells> FloorSpells = new ConcurrentDictionary<ushort, FloorSpell.ClientFloorSpells>();

        public ushort RandomSpell = 0;
        public bool MSConquer = false;
        public bool MRConquer = false;
        public bool DbTry = false;
        public byte AddJade = 0;

        public byte LotteryEntries;
        public Database.LotteryTable.LotteryItem LotteryItem;
        /// <summary>Lucky Box of the last lottery draw (the window's "Try Again" draws from the same box).</summary>
        public string LotteryBox = Database.LotteryTable.BoxAssorted;


        public bool Reincarnation = false;

        public byte JingTryngUltra = 0;
        public Instance.JiangHu MyJiangHu;
        public unsafe byte JiangHuTalent;
        public unsafe byte JiangHuActive;

        Instance.Clan.Member clanmemb;

        public Instance.Clan MyClan;
        public unsafe uint ClanUID;
        public unsafe ushort ClanRank;



        public Role.Instance.Guild.Member MyGuildMember;
        public Role.Instance.Guild MyGuild;
        public uint TargetGuild = 0;

        uint _extbattle;

        public unsafe Flags.GuildMemberRank GuildRank = Flags.GuildMemberRank.None;
        public unsafe uint GuildID;

        uint guildBP;

        uint _clanbp;

        uint _mentorBp;

        public uint targetTrade = 0;

        public Role.Instance.Associate.MyAsociats MyMentor = null;
        public Role.Instance.Associate.MyAsociats Associate;


        public uint TradePartner = 0;
        public uint TargetFriend = 0;

        public Role.Instance.Nobility Nobility;
        Role.Instance.Nobility.NobilityRank _NobilityRank;

        public Instance.Flowers Flowers;
        public unsafe uint FlowerRank;
        public bool OnFairy = false;
        public unsafe Game.MsgServer.MsgTransformFairy FairySpawn;


        public Instance.Chi MyChi;

        public ushort ActiveDance = 0;
        public Client.GameClient ObjInteraction;

        uint _KingDomExploits;
        public Role.Flags.ExploitsRank ExploitsRank = 0;

        public unsafe Game.MsgServer.InteractQuery InteractionEffect = default(Game.MsgServer.InteractQuery);
        public bool OnInteractionEffect = false;

        public Instance.SubClass SubClass;
        public unsafe uint SubClassHasPoints;
        public unsafe Database.DBLevExp.Sort ActiveSublass;

        public uint ToxicLevel = 0;
        public bool BlackSpot = false;
        public Extensions.Time32 Stamp_BlackSpot = new Extensions.Time32();
        public DateTime LastMove;
        public int TotalHits, Hits, Chains, MaxChains;
        public byte UseStamina = 0;
        public Extensions.Time32 Protect = new Extensions.Time32();
        private Extensions.Time32 ProtectedJumpAttack = new Extensions.Time32();
        public uint ShieldBlockDamage = 0;

        public Game.MsgServer.MsgUpdate.Flags UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
        public Database.MagicType.Magic Aura;
        private int AuraTimer = 0;
        private bool AuraStatusIncreased = false;
        public uint SpouseUID = 0;

        public Extensions.Time32 AttackStamp = new Extensions.Time32();
        public Extensions.Time32 SpellAttackStamp = new Extensions.Time32();

        public bool OnTransform { get { return TransformationID != 0; } }
        public ClientTransform TransformInfo = null;
        public byte PoisonLevel = 0;
        public byte PoisonLevehHu = 0;

        public bool ActivateCounterKill = false;

        public Action<Client.GameClient> MessageOK;
        public Action<Client.GameClient> MessageCancel;
        public Extensions.Time32 StartMessageBox = new Extensions.Time32();

        public bool InUseIntensify = false;
        public Extensions.Time32 IntensifyStamp = new Extensions.Time32();
        public bool Intensify = false;
        public int IntensifyDamage = 0;
        ushort azuredef;
        public byte AzureShieldLevel;
        private ushort _stamina;
        public Extensions.Time32 XPListStamp = new Extensions.Time32();
        public StatusFlagsBigVector32 BitVector;
        public ushort Dead_X;
        public ushort Dead_Y;

        public bool GetPkPkPoints = false;
        public bool CompleteLogin = false;

        public DateTime GhostStamp = new DateTime();
        public Extensions.Time32 PkPointsStamp = new Extensions.Time32();
        public uint BlessTime = 0;
        public Extensions.Time32 CastPrayStamp = new Extensions.Time32();
        public Extensions.Time32 CastPrayActionsStamp = new Extensions.Time32();

        public Game.MsgServer.MsgUpdate.Flags UseXPSpell;
        public ushort OblivionCounter;
        private ushort _killcount;
        ushort _xpc;

        public Extensions.Time32 DeadStamp = new Extensions.Time32();
        public ushort Avatar;
        public long WHMoney;
        public bool IsAsasin = false;
        public Game.MsgServer.ClientAchievement Achievement;
        public Extensions.Time32 LastWorldMessaj = new Extensions.Time32();
        public Flags.PKMode PkMode = Flags.PKMode.Capture;
        public Instance.JiangHu.AttackFlag JiangPkFlag = Instance.JiangHu.AttackFlag.None;
        public Client.GameClient Owner;
        public MapObjectType ObjType { get; set; }
        public Instance.Quests QuestGUI;
        public Player(Client.GameClient _own)
        {
            AllowDynamic = false;
            this.Owner = _own;
            ObjType = MapObjectType.Player;
            View = new RoleView(Owner);
            BitVector = new StatusFlagsBigVector32(32 * 7);//6
            QuestGUI = new Instance.Quests(this);
        }
        public int Day = 0;

        public unsafe uint UID { get; set; }
        public unsafe string Name = "";
        public unsafe string ClanName = "";

        public string Spouse = "None";
        public ushort Agility;
        public ushort Vitality;
        public ushort Spirit;
        public ushort Strength;
        public ushort Atributes;
        public uint DragonIslandCollectedPoints = 0;
        byte _class;
        public byte FirstRebornLevel;
        public byte SecoundeRebornLevel;

        public unsafe byte FirstClass;
        public unsafe byte SecoundeClass;

        ushort _level;
        public unsafe byte Reborn;
        long _Money;
        public uint ConquerPointsLimitaion = 0;
        public byte ConquerPointDropLimitLayer = 1;
        public DateTime ConquerPointsDropStamp = DateTime.FromBinary(0);
        uint _cps;
        int _bountCps;
        public ulong Experience;
        public uint VirtutePoints;

        int _minhitpoints;
        ushort _mana;
        ushort _pkpoints;
        public unsafe uint QuizPoints;
        public unsafe ushort Enilghten;
        //  DateTime _ExpireVip;
        DateTime _ExpireVip;

        byte _viplevel;

        public ushort EnlightenReceive;
        ushort face;
        public DateTime BannedChatStamp;
        public bool PermenantBannedChat;
        public bool IsBannedChat;
        public ConcurrentDictionary<int, Conquer6609.Game.Conquer6609_Work.Halo_Flag.Clientt> HaloPoll = new ConcurrentDictionary<int, Conquer6609.Game.Conquer6609_Work.Halo_Flag.Clientt>();//Halo_Flag
        #region events & quests
        public bool canhelplady = false;
        public byte helpladytime = 0;
        public uint VotePoints = 0;
        public Extensions.Time32 StartHelpLady = new Extensions.Time32();
        #endregion
        public unsafe ushort Face
        {
            get { return face; }
            set
            {
                face = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh);
                    }
                }
            }
        }

        public unsafe ushort Hair;

        public uint PDinamycID { get; set; }
        public uint DynamicID { get; set; }

        uint _mmmap;
        public uint Map
        {
            get { return _mmmap; }
            set { _mmmap = value; }
        }


        ushort xx, yy;
        public unsafe ushort X
        {
            get { return xx; }
            set { Px = X; xx = value; }
        }
        public unsafe ushort Y
        {
            get { return yy; }
            set { Py = Y; yy = value; }
        }

        public void ClearPreviouseCoord()
        {
            Px = 0;
            Py = 0;
        }

        public ushort Px;
        public ushort Py;
        public ushort PMapX;
        public ushort PMapY;
        public uint PMap;

        public unsafe Flags.ConquerAngle Angle = Flags.ConquerAngle.East;
        public unsafe Flags.ConquerAction Action = Flags.ConquerAction.None;

        public byte ExpBallUsed = 0, ExpFruit = 0, TeratoDragon = 0, SwordMaster = 0, PurbleBanshee = 0;
        public byte BDExp = 0;
        public DateTime JoinOnflineTG = new DateTime();
        public Extensions.Time32 OnlineTrainingTime = new Extensions.Time32();
        public Extensions.Time32 ReceivePointsOnlineTraining = new Extensions.Time32();
        public Extensions.Time32 HeavenBlessTime = new Extensions.Time32();
        public int HeavenBlessing = 0;
        public uint OnlineTrainingPoints = 0;
        public uint HuntingBlessing = 0;

        public uint DExpTime = 0;
        public uint RateExp = 1;

        public uint ExpProtection = 0;
        ushort body;
        private ushort _transformationid;
        public bool Alive { get { return HitPoints > 0; } }

        public uint HeadId = 0;
        public uint GarmentId = 0;
        public uint ArmorId = 0;
        public uint LeftWeaponId = 0;
        public uint RightWeaponId = 0;
        public uint LeftWeaponAccessoryId = 0;
        public uint RightWeaponAccessoryId = 0;
        public uint SteedId = 0;
        public uint MountArmorId = 0;

        public ushort ColorArmor = 0;
        public ushort ColorShield = 0;
        public ushort ColorHelment = 0;

        public uint SteedPlus = 0;
        public uint SteedColor = 0;

        public uint HeadSoul = 0;
        public uint ArmorSoul = 0;
        public uint LeftWeapsonSoul = 0;
        public uint RightWeapsonSoul = 0;

        public uint WingId = 0;
        public byte WingPlus = 0;
        public uint WingProgress = 0;


        public uint RealUID = 0;

        public string UnionName = "";
        public int CountJumpHack, CountMoveHack, CountPing;
        public DateTime JumpingStamp, PreviousJump, MoveingStamp, PreviousMove;
        public string NewUser = "";
    }

}
