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
    public unsafe partial class MonsterRole
    {

        public short GetMyDistance(ushort X2, ushort Y2)
        {
            return Role.Core.GetDistance(X, Y, X2, Y2);
        }
        public short OldGetDistance(ushort X2, ushort Y2)
        {
            return Role.Core.GetDistance(PX, PY, X2, Y2);
        }
        public bool InView(ushort X2, ushort Y2, byte distance)
        {
            return (!(OldGetDistance(X2, Y2) < distance) && GetMyDistance(X2, Y2) < distance);
        }


        public unsafe void Send(ServerSockets.Packet msg)
        {
            View.SendScreen(msg, GMap);
        }
        public void UpdateMonsterView(Role.RoleView Target, ServerSockets.Packet stream)
        {
            foreach (var player in View.Roles(GMap, Role.MapObjectType.Player))
            {
                if (InView(player.X, player.Y, MonsterView.ViewThreshold))
                    player.Send(GetArray(stream, false));
            }
        }
        public bool UpdateMapCoords(ushort New_X, ushort New_Y, Role.GameMap _map)
        {
            if (!_map.IsFlagPresent(New_X, New_Y, Role.MapFlagType.Monster))
            {
                _map.SetMonsterOnTile(X, Y, false);
                _map.SetMonsterOnTile(New_X, New_Y, true);
                _map.View.MoveTo<MonsterRole>(this, New_X, New_Y);
                X = New_X;
                Y = New_Y;
                return true;
            }
            return false;
        }
        public void RemoveRole(Role.IMapObj obj)
        {

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
            if (IsFloor && Mesh != 980)
            {
                return stream.ItemPacketCreate(this.FloorPacket);

            }
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);
            stream.Write(Mesh);
            stream.Write(UID);
            stream.ZeroFill(10);

            for (int x = 0; x < BitVector.bits.Length; x++)
                stream.Write(BitVector.bits[x]);



            stream.ZeroFill(57);



            if (Boss > 0)
            {
                if (IsFloor)
                {
                    stream.Write(StampFloorSecounds);
                }
                else
                {
                    uint key = (uint)(Family.MaxHealth / 10000);
                    if (key != 0)
                        stream.Write((uint)(HitPoints / key));
                    else
                        stream.Write((uint)(HitPoints * Family.MaxHealth));
                }
            }
            else
            {
                if (IsFloor)
                {
                    stream.Write(StampFloorSecounds);
                }
                else
                    stream.Write(HitPoints);
            }
            stream.Write((ushort)0);
            stream.Write((ushort)Level);


            stream.Write(X);
            stream.Write(Y);
            stream.Write((ushort)0);
            stream.Write((byte)Facing);
            stream.Write((byte)Action);
            stream.ZeroFill(93);

            stream.Write((byte)Boss);
            stream.ZeroFill(50);




            if (IsFloor)
            {
                stream.Write((ushort)FloorPacket.m_ID);
                stream.Write((byte)0);
                stream.Write((uint)(OwnerFloor.Player.UID));
                stream.Write((ushort)9);
            }
            else
            {
                stream.ZeroFill(7);
                stream.Write((ushort)0);
            }


            stream.Write(0);
            stream.Write(0);

            stream.Write(0);
            //

            stream.Write(0);
            stream.Write(0);
            stream.Write(0);

            stream.Write(0);
            if (IsFloor)
            {
                stream.Write(PetFlag);//3?
            }
            else
                stream.Write(0);
            stream.Write((byte)0);
            stream.Write((uint)0);

            stream.Write(Name, string.Empty, string.Empty, string.Empty);
            stream.Finalize(Game.GamePackets.SpawnPlayer);
            //    MyConsole.PrintPacketAdvanced(stream.Memory, stream.Size);

            return stream;
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
    }
}
