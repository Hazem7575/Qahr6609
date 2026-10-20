using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role
{
    public unsafe class Clone
    {
        public void RemoveThat(Client.GameClient _Owner)
        {

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ActionQuery action = new ActionQuery()
                {
                    ObjId = this.UID,
                    Type = ActionType.RemoveEntity
                };
                Owner.Player.View.SendView(stream.ActionCreate(&action), true);

            }

        }
        public Extensions.BitVector32 BitVector;
        public uint UID = 0;

        public Client.GameClient Owner;
        public static Extensions.Counter CounterUID = new Extensions.Counter(700100);

        public void AddFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (!BitVector.Contain((int)Flag))
            {
                BitVector.Add((int)Flag);
                UpdateSpawnPacket();
            }
        }
        public void RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (BitVector.Contain((int)Flag))
            {
                BitVector.Remove((int)Flag);
                UpdateSpawnPacket();
            }
        }
        public void UpdateSpawnPacket()
        {
            // fixed (byte* ptr = Packet)
            // {
            //     for (int x = 0; x < BitVector.bits.Length; x++)
            //         *(uint*)(ptr + BitVector32 + x * 4) = BitVector.bits[x];
            // }
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
                Owner.Send(stream);
            }
        }



        public static void CreateShadowClone(Role.Player client, ServerSockets.Packet stream)
        {
            client.MyClones.Add(new Clone(client, "ShadowClone", 3, stream));//3
            client.MyClones.Add(new Clone(client, "ShadowClone", 10003, stream));//10003

        }
        public static void CreateShadowClone(Role.Player client, int flag, ServerSockets.Packet stream)
        {
            client.MyClones.Add(new Clone(client, "ShadowClone", flag, stream));//3

            //client.MyClones.Add(new Clone(client, "ShadowClone", 10003));//10003
        }

        public int cloneflag = 0;
        public string clonename = "";
        public Clone(Role.Player role, string CloneName, int flag, ServerSockets.Packet stream)
        {
            BitVector = new Extensions.BitVector32(32 * 7);
            Owner = role.Owner;
            cloneflag = flag;
            clonename = CloneName;
            UID = CounterUID.Next;

            SendView(role.Owner, stream);

            AddFlag(Game.MsgServer.MsgUpdate.Flags.Invisibility);

        }

        public void SendView(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Player.View.SendView(GetArray(stream), true);
        }
        public void Send(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Send(GetArray(stream));
        }


        public ServerSockets.Packet GetArray(ServerSockets.Packet stream)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);
            stream.Write(Owner.Player.Mesh);
            stream.Write(UID);
            stream.Write(Owner.Player.GuildID);
            stream.Write((ushort)Owner.Player.GuildRank);
            stream.Write((uint)0);//unknow

            for (int x = 0; x < BitVector.bits.Length; x++)
                stream.Write(BitVector.bits[x]);


            stream.Write((ushort)Owner.Player.AparenceType);//apparence type
            stream.Write(Owner.Player.HeadId);
            stream.Write(Owner.Player.GarmentId);
            stream.Write(Owner.Player.ArmorId);
            stream.Write(Owner.Player.LeftWeaponId);
            stream.Write(Owner.Player.RightWeaponId);
            stream.Write(Owner.Player.LeftWeaponAccessoryId);
            stream.Write(Owner.Player.RightWeaponAccessoryId);
            stream.Write(Owner.Player.SteedId);
            stream.Write(Owner.Player.MountArmorId);
            stream.Write(Owner.Player.WingId);//winds
            stream.Write(Owner.Player.WingPlus);//plus
            stream.Write(Owner.Player.WingProgress);

            stream.Write((uint)0);//???
            stream.ZeroFill(6);//unknow

            stream.Write(Owner.Player.HitPoints);
            stream.Write((ushort)0);//unknow
            stream.Write((ushort)0);//monster level

            stream.Write(Owner.Player.X);
            stream.Write(Owner.Player.Y);
            stream.Write(Owner.Player.Hair);
            stream.Write((byte)Owner.Player.Angle);
            stream.Write((uint)Owner.Player.Action);
            stream.Write((ushort)0);//unknow
            stream.Write((byte)0);//padding?
            stream.Write(Owner.Player.Reborn);
            stream.Write(Owner.Player.Level);


            stream.Write((byte)(0));
            stream.Write((byte)0);//away
            stream.Write(Owner.Player.ExtraBattlePower);
            stream.Write((uint)0);//unknow position = 125
            stream.Write((uint)0);//unknow position = 129
            stream.Write((uint)0);//unknow p = 133;
            stream.Write((uint)(Owner.Player.FlowerRank + 10000));
            stream.Write((uint)Owner.Player.NobilityRank);

            stream.Write(Owner.Player.ColorArmor);
            stream.Write(Owner.Player.ColorShield);
            stream.Write(Owner.Player.ColorHelment);
            stream.Write((uint)0);//quiz points
            stream.Write(Owner.Player.SteedPlus);
            stream.Write((ushort)0);//unknow
            stream.Write(Owner.Player.SteedColor);
            stream.Write((ushort)Owner.Player.Enilghten);
            stream.Write((ushort)0);//merit points
            stream.Write((uint)0);//unknow
            stream.Write((uint)0);//unknow

            stream.Write(Owner.Player.ClanUID);
            stream.Write(Owner.Player.ClanRank);
            stream.Write(0);//p = 187
            stream.Write((ushort)0);//unknow
            stream.Write(Owner.Player.MyTitle);

            stream.ZeroFill(14);
            stream.Write(Owner.Player.HeadSoul);
            stream.Write(Owner.Player.ArmorSoul);
            stream.Write(Owner.Player.LeftWeapsonSoul);
            stream.Write(Owner.Player.RightWeapsonSoul);
            stream.Write((byte)Owner.Player.ActiveSublass);
            stream.Write(Owner.Player.SubClassHasPoints);
            stream.Write((uint)0);//unknow
            stream.Write((ushort)Owner.Player.FirstClass);
            stream.Write((ushort)Owner.Player.SecoundeClass);
            stream.Write((ushort)Owner.Player.Class);
            stream.Write((ushort)Owner.Player.CountryID);//unknow
            stream.Write((uint)0);
            stream.Write(Owner.Player.BattlePower);
            stream.Write(Owner.Player.JiangHuTalent);
            stream.Write(Owner.Player.JiangHuActive);

            stream.Write((ushort)0);

            stream.Write((byte)0);
            stream.Write((uint)0);
            stream.Write((byte)2);//clone count 263
            stream.Write((ushort)cloneflag); //264
            stream.Write(Owner.Player.UID); //366
            stream.Write((ushort)0);

            stream.Write(0);//union id?? aici e uint

            stream.Write((ushort)0);
            stream.Write((uint)0);//union type (bite Flag)
            stream.Write(ushort.MaxValue);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);

            stream.Write(0);
            stream.Write(0);
            stream.Write((uint)0);
            stream.Write((byte)0);
            stream.Write(Owner.Player.Name, string.Empty, string.Empty, "ShadowClone(" + Owner.Player.Name + ")");

         //   Owner.Player.View.SendView(GetArray(stream), true);
            
            stream.Finalize(Game.GamePackets.SpawnPlayer);

            return stream;
        }

    }
}
