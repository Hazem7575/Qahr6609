using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Database;

namespace Conquer6609.Game.MsgMonster
{
    public unsafe partial class MonsterRole : Role.IMapObj
    {
        public const ushort
    UintStamp = 4,
    Uint_Mesh = 8,
    Uint_UID = 12,
    BitVector32 = 26,
    Uint16_HitPoints = 94,//90
    Uint16_Level = 100,
    Uint16_X = 102,
    Uint16_Y = 104,
    Byte_Fascing = 108,
    Byte_Action = 109,
    Byte_Boss = 203,

    Str_Count = 257,//246,//244,
    Str_NameLenght = 258,//247,//245,
    Str_Name = 259,//248,//246,
    Uint16_PLenght = 260;//249;//247;

        public DateTime RemoveFloor = DateTime.Now;
        public int StampFloorSecounds = 0;

        public static List<uint> SpecialMonsters = new List<uint>()
        {
            20070,
            20300,
            20160,
            20060,
            29370,
            29360,
            29300,
            29363,
            20170,
            20061,
            20063,
            20064,

        };

        public Client.GameClient AttackerScarofEarthl;
        public Database.MagicType.Magic ScarofEarthl;

        public int ExtraDamage { get { return Family.extra_damage; } }
        public int BattlePower { get { return Family.extra_battlelev; } }
        public bool AllowDynamic { get; set; }
        // MonsterAiMode >= 1: MapGroupThread.AiTick in which this monster was last handled (dedupe across viewers).
        public int LastAiTick;
        public int LastBuffTick;
        public bool IsTrap() { return false; }
        public uint IndexInScreen { get; set; }

        public Client.GameClient OwnerFloor;
        public Database.MagicType.Magic DBSpell;
        public ushort SpellLevel = 0;
        public DateTime FloorStampTimer = new DateTime();
        public bool IsFloor = false;
        public Game.MsgFloorItem.MsgItemPacket FloorPacket;


        public bool BlackSpot = false;
        public Extensions.Time32 Stamp_BlackSpot = new Extensions.Time32();


        public int SizeAdd { get { return Family.AttackRange; } }

        public byte PoisonLevel = 0;

        private Extensions.Time32 DeadStamp = new Extensions.Time32();
        private Extensions.Time32 FadeAway = new Extensions.Time32();
        public Extensions.Time32 RespawnStamp = new Extensions.Time32();
        public Extensions.Time32 MoveStamp = new Extensions.Time32();
        // Written from several threads (hits) and read on death: concurrent map, entries updated under scoreObj.
        public System.Collections.Concurrent.ConcurrentDictionary<uint, MsgBossHarmRankingEntry> Hunters = new System.Collections.Concurrent.ConcurrentDictionary<uint, MsgBossHarmRankingEntry>();
        public void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
          , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream));
            }
        }
        public bool logged = false;
        public object scoreObj = new object();
        public MonsterFamily Family;
        public MonsterView View;
        public MobStatus State;
        public Role.Player Target = null;
        public Extensions.Time32 AttackSpeed = new Extensions.Time32();

        public Role.StatusFlagsBigVector32 BitVector;

        public byte OpenBoss = 0;
        public uint Map { get; set; }
        public uint DynamicID { get; set; }
        public Role.MapObjectType ObjType { get; set; }
        public unsafe string Name = "";

        public byte Boss = 0;
        public uint Mesh = 0;
        public uint UID { get; set; }
        public byte Level = 0;
        public uint HitPoints;

        public ushort RespawnX;
        public ushort RespawnY;


        public ushort PX = 0;
        public ushort PY = 0;
        public ushort _xx;
        public ushort _yy;

        public ushort X { get { return _xx; } set { PX = _xx; _xx = value; } }
        public ushort Y { get { return _yy; } set { PY = _yy; _yy = value; } }
        public Role.Flags.ConquerAction Action = Role.Flags.ConquerAction.None;
        public Role.Flags.ConquerAngle Facing = Role.Flags.ConquerAngle.East;
        public string LocationSpawn = "";
        public Role.GameMap GMap;
        public bool RemoveOnDead = false;
        public uint PetFlag = 0;
        public MonsterRole(MonsterFamily Famil, uint _UID, string locationspawn, Role.GameMap _map)
        {
            AllowDynamic = false;
            GMap = _map;
            LocationSpawn = locationspawn;
            ObjType = Role.MapObjectType.Monster;
            Name = Famil.Name;
            Family = Famil;
            UID = _UID;
            Mesh = Famil.Mesh;
            Level = (byte)Famil.Level;
            HitPoints = (uint)Famil.MaxHealth;
            View = new MonsterView(this);
            State = MobStatus.Idle;
            BitVector = new Role.StatusFlagsBigVector32(32 * 7);//5
            Boss = Family.Boss;
            Facing = (Role.Flags.ConquerAngle)Program.GetRandom.Next(0, 8);

        }
        public bool Alive { get { return HitPoints > 0; } }
    }
}
