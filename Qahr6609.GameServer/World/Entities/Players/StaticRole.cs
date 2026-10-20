using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role
{
    public class StaticRole : IMapObj
    {
        public Role.StatusFlagsBigVector32 BitVector;
        public StaticRole(ushort x, ushort y, string _Namee = "Flag", uint FlagMesh = 513)
        {
            AllowDynamic = false;
            Name = _Namee;
            X = x;
            Y = y;
            UID = (uint)(x * 1000 + y);
            Mesh = FlagMesh;
            Level = 1;
            ObjType = MapObjectType.StaticRole;
            BitVector = new Role.StatusFlagsBigVector32(32 * 7);
        }
        public Role.GameMap GMap
        {

            get { return Database.Server.ServerMaps[Map]; }
        }
        public bool AllowDynamic { get; set; }
        public uint IndexInScreen { get; set; }
        public bool IsTrap() { return false; }
        public unsafe string Name { get; set; }
        public unsafe uint Mesh { get; set; }
        public unsafe uint UID { get; set; }
        public unsafe byte Level { get; set; }
        public unsafe uint HitPoints { get; set; }
        public uint Map { get; set; }
        public uint DynamicID { get; set; }
        public Role.MapObjectType ObjType { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public unsafe Role.Flags.ConquerAction Action { get; set; }
        public unsafe Role.Flags.ConquerAngle Facing { get; set; }

        public uint SetBy = 0;
        public bool Viable = false;

        public bool QuestionMark { get { return Mesh == 767; } }
        public Game.MsgServer.MsgRacePotion.RaceItemType Type
        {
            get
            {
                switch (Mesh)
                {
                    case 760: return Game.MsgServer.MsgRacePotion.RaceItemType.GuardPotion;
                    case 761: return Game.MsgServer.MsgRacePotion.RaceItemType.FrozenTrap;
                    case 762: return Game.MsgServer.MsgRacePotion.RaceItemType.SluggishPotion;
                    case 763: return Game.MsgServer.MsgRacePotion.RaceItemType.DizzyHammer;
                    case 764: return Game.MsgServer.MsgRacePotion.RaceItemType.RestorePotion;
                    case 765: return Game.MsgServer.MsgRacePotion.RaceItemType.ScreamBomb;
                    case 766: return Game.MsgServer.MsgRacePotion.RaceItemType.TransformItem;
                    case 768: return Game.MsgServer.MsgRacePotion.RaceItemType.SpiritPotion;
                    case 769: return Game.MsgServer.MsgRacePotion.RaceItemType.ExcitementPotion;
                    case 770: return Game.MsgServer.MsgRacePotion.RaceItemType.ChaosBomb;
                    default:
                        {

                            int val = (int)Game.MsgServer.MsgRacePotion.RaceItemType.TransformItem;
                            while (val == (int)Game.MsgServer.MsgRacePotion.RaceItemType.TransformItem || val == (int)Game.MsgServer.MsgRacePotion.RaceItemType.FrozenTrap)
                                val = Program.GetRandom.Next((int)Game.MsgServer.MsgRacePotion.RaceItemType.ChaosBomb, (int)Game.MsgServer.MsgRacePotion.RaceItemType.SuperExcitementPotion);
                            return (Game.MsgServer.MsgRacePotion.RaceItemType)val;
                        }
                }
            }
        }

        private static Tuple<uint, string, ushort>[] raceItems = new[]
            {

           new  Tuple<uint, string, ushort>(760,"GuardPotion",1),
//                new  Tuple<uint, string, ushort>(761,"FrozenTrap",1),
                new  Tuple<uint, string, ushort>(762,"SluggishPotion",3),
                new  Tuple<uint, string, ushort>(763,"DizzyHammer",1),
                new  Tuple<uint, string, ushort>(764,"RestorePotion",3 ),
                new  Tuple<uint, string, ushort>(765,"ScreamBomb",1 ),
                new  Tuple<uint, string, ushort>(766,"SuperExclamationMark",1),
                new  Tuple<uint, string, ushort>(767,"SuperQuestionMark",1),
                new  Tuple<uint, string, ushort>(768,"SpiritPotion",1),
                new  Tuple<uint, string, ushort>(769,"ExcitementPotion",1),
                new  Tuple<uint, string, ushort>(770,"ChaosBomb",1)

              
            };


        public void Pick()
        {
            Viable = true;
            var itemData = raceItems[Program.GetRandom.Next(raceItems.Length)];
            Mesh = itemData.Item1;
            Name = itemData.Item2;
            Level = (byte)itemData.Item3;

        }
        public void DoFrozenTrap(uint setter)
        {
            SetBy = setter;
            Viable = true;

            Mesh = 761;
            Name = "Frozen Trap";

            Level = 1;

        }

        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag, Role.GameMap map)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        private unsafe void UpdateFlagOffset(bool SendScreem = true)
        {
            if (SendScreem)
                SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag);
        }
        public unsafe void SendUpdate(uint[] Value, Game.MsgServer.MsgUpdate.DataType datatype)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Send(stream);
            }
        }
        public bool UpdateFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Secounds, SetNewTimer, MaxTime);
        }

        public unsafe bool MoveTo(ushort _x, ushort _y)
        {
            if (_x == X && _y == Y)
            {
                return false;
            }
            Role.Flags.ConquerAngle dir = Role.Core.GetAngle(X, Y, _x, _y);
            ushort WalkX = X; ushort WalkY = Y;
            Role.Core.IncXY(dir, ref WalkX, ref WalkY);

            // ushort WalkX = (ushort)(X + Game.MsgServer.MsgMovement.DeltaMountX[(byte)dir]);
            // ushort WalkY = (ushort)(Y + Game.MsgServer.MsgMovement.DeltaMountY[(byte)dir]);


            GMap.View.MoveTo<Role.IMapObj>(this, WalkX, WalkY);
            X = WalkX;
            Y = WalkY;



            /* var action = new ActionQuery()
             {
                 ObjId = UID,
                 wParam5 =5,
                 dwParam3 = Map,
                 dwParam_Lo = WalkX,
                 dwParam_Hi = WalkY,
                 Type =  ActionType.Jump,
                 Fascing = (ushort)dir
             };*/
            WalkQuery walk = new WalkQuery()
            {
                Direction = (byte)dir,
                Running = 2,
                UID = UID
            };
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                //  Send(stream.ActionCreate(&action));
                Send(stream.MovementCreate(walk));
            }
            return true;
        }


        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;

            packet.Strings = args;
            Send(stream.StringPacketCreate(packet));
        }
      
         public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool view)
        {


            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);

            stream.Write(Mesh);//(uint)(TransformationID * 10000000 + Face * 10000 + Body));
            stream.Write(UID);
            stream.Write(0);
            stream.ZeroFill(2);
            stream.Write((uint)0);//unknow
            for (int x = 0; x < BitVector.bits.Length; x++)
                stream.Write(BitVector.bits[x]);

            stream.Write((ushort)0);//apparence type
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write((byte)0);//talisman plus
            stream.Write(0);

            stream.Write((uint)0);//?? unknow

            stream.ZeroFill(6);//unknow
            stream.Write(HitPoints);
            stream.Write((ushort)0);//unknow
            stream.Write((ushort)Level);//monster level

            stream.Write(X);
            stream.Write(Y);
            stream.Write(0);
            stream.Write((byte)Facing);
            stream.Write((byte)Action);
            stream.Write((ushort)0);//unknow
            stream.Write((byte)0);//padding?
            stream.Write((byte)0);
            stream.Write((byte)0);
            stream.Write((byte)0);
            stream.Write((byte)0);//away
            stream.Write(0);
            stream.Write((uint)0);//unknow position = 125
            stream.Write((uint)0);//unknow position = 129
            stream.Write((uint)0);//unknow p = 133;
            stream.Write((uint)(0));
            stream.Write((uint)0);

            stream.Write((ushort)0);
            stream.Write((ushort)0);
            stream.Write((ushort)0);
            stream.Write((uint)0);//quiz points
            stream.Write(0);
            stream.Write((ushort)0);//unknow
            stream.Write(0);
            stream.Write((ushort)0);
            stream.Write((ushort)0);//merit points
            stream.Write((uint)0);//unknow
            stream.Write((uint)0);//unknow
            stream.Write(0);
            stream.Write((uint)0);
            stream.Write((uint)0);//unknow
            stream.Write((ushort)0);
            stream.Write((uint)0);
            stream.Write((uint)0);
            stream.Write((byte)0);
            stream.Write((byte)0);//1 == invisible player watcher
            stream.Write((byte)0);
            stream.Write((byte)0);
            stream.Write((byte)0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write((byte)0);
            stream.Write(0);
            stream.Write((uint)0);//unknow
            stream.Write((ushort)0);
            stream.Write((ushort)0);
            stream.Write((ushort)0);
            stream.Write((ushort)0);//country
            stream.Write(0);
            stream.Write(0);
            stream.Write((byte)0);
            stream.Write((byte)0);
            stream.Write((byte)0);
            stream.ZeroFill(2);
            stream.Write((uint)0);
            stream.Write((byte)2);//clone count 
            stream.Write((ushort)0); // clone ID
            stream.Write(0); //clone owner
            stream.ZeroFill(4);
            stream.Write((uint)0);
            stream.ZeroFill(6);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);

            stream.Write((uint)0);
            stream.Write(0);
            stream.Write((byte)0);
            stream.Write((uint)0);
            stream.Write(Name, string.Empty);
            stream.Finalize(Game.GamePackets.SpawnPlayer);

            return stream;
        }

        public bool Alive { get { return HitPoints > 0; } }

        public void RemoveRole(IMapObj obj)
        {

        }

        public unsafe void Send(ServerSockets.Packet msg)
        {
            foreach (var obj in GMap.View.Roles(MapObjectType.Player, X, Y, p => CanSee(p)))
                obj.Send(msg);
        }
        public bool CanSee(Role.IMapObj obj)
        {
            if (obj.Map != Map)
                return false;
            if (obj.DynamicID != DynamicID)
                return false;
            if (obj.UID == UID)
                return false;
            return Role.Core.GetDistance(obj.X, obj.Y, X, Y) <= 18;
        }
    }
}
