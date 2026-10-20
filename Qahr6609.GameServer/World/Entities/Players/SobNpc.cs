using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role
{
  
    public unsafe class SobNpc : IMapObj
    {
        public enum StaticMesh : ushort
        {
            Vendor = 406,
            LeftGate = 241,
            OpenLeftGate = 251,
            RightGate = 277,
            OpenRightGate = 287,
            Pole = 1137,
            SuperGuildWarPole = 31220
        }

        public Role.Statue statue = null;
        public bool AllowDynamic { get; set; }
        public Role.StatusFlagsBigVector32 BitVector;
        public uint IndexInScreen { get; set; }
        public bool IsStatue
        {
            get { return statue != null; }
        }
        public SobNpc(Role.Statue _statue)
        {
            statue = _statue;
            BitVector = new StatusFlagsBigVector32(32 * 6);
        }
        

        public SobNpc()
        {
            AllowDynamic = false;
            BitVector = new StatusFlagsBigVector32(32 * 6);
        }
        public const byte SeedDistrance = 19;//17
        public bool IsTrap() { return false; }
        public uint UID { get; set; }
        public int MaxHitPoints { get; set; }
        int Hit;
        public int HitPoints
        {
            get { return Hit; }
            set {
                Hit = value;
            }

        }

        public ushort X { get; set; }
        public ushort Y { get; set; }
        public StaticMesh Mesh;
        public Flags.NpcType Type;
        public ushort Sort;
        public string Name;

        public Game.Booth Booth;
        public uint Map { get; set; }
        public uint DynamicID { get; set; }

        public bool Alive { get { return HitPoints > 0; } }
        public MapObjectType ObjType { get; set; }

        public Client.GameClient OwnerVendor;

        public void RemoveRole(IMapObj obj)
        {

        }
        public unsafe void Send(byte[] packet)
        {

        }
        public unsafe void Send(ServerSockets.Packet msg)
        {

        }
        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0, uint showamount = 0, uint amount = 0)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagScreen();
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagScreen();
              
                return true;
            }
            return false;
        }
        public bool ContainFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }
        public void UpdateFlagScreen()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, MsgUpdate.DataType.StatusFlag, BitVector.bits);
                stream = upd.GetArray(stream);

                foreach (var user in Database.Server.GamePoll.Values)
                {
                    if (user.Player.Map == Map)
                        user.Send(stream);
                }
            }
        }

        public unsafe void Die(ServerSockets.Packet stream, Client.GameClient killer)
        {
            if (HitPoints == 0)
                return;
            if (killer.Map.ID != 1002) 
                killer.OnAutoAttack = false;
            if (Map == 1039)
            {
                HitPoints = MaxHitPoints;
                InteractQuery action = new InteractQuery()
                {
                    UID = killer.Player.UID,
                    X = X,
                    Y = Y,
                    AtkType = MsgAttackPacket.AttackID.Death,
                    KillCounter = killer.Player.KillCounter,
                    SpellID = (ushort)(Database.ItemType.IsBow(killer.Equipment.RightWeapon) ? 5 : 1),
                    OpponentUID = UID,
                };
                killer.Player.View.SendView(stream.InteractionCreate(&action), true);


                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, (long)HitPoints);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, (long)MaxHitPoints);
                stream = upd.GetArray(stream);
                killer.Player.View.SendView(stream, true);
                return;
            }
            if (IsStatue)
            {
                HitPoints = 0;
                Role.Statue.RemoveStatue(stream, killer, UID, this);
                return;
            }
            if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures, Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleLeftGate)
                || UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures, Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleRightGate))
            {
                Mesh = StaticMesh.OpenRightGate;
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (client.Player.Map == Map)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.SobNpc.SeedDistrance)
                        {
                            client.Send(stream);
                        }

                    }
                }
            }
            if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures, Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.CenterGate)
               || UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures, Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.LeftGate)
                || UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures, Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.RightGate))
            {
                Mesh = StaticMesh.OpenLeftGate;
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (client.Player.Map == Map)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.SobNpc.SeedDistrance)
                        {
                            client.Send(stream);
                        }

                    }
                }
            }

            if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures, StaticMesh.RightGate))
            {
                if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null)
                {
                    Instance.Guild guild;
                    if (Instance.Guild.GuildPoll.TryGetValue(Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID, out guild))
                    {
                        guild.SendMessajGuild("[GuildWar] The right gate has been breached!");

                    }
                }
                Mesh = StaticMesh.OpenRightGate;
                Game.MsgTournaments.MsgSchedules.GuildWar.ClosableRightGateStamp = DateTime.Now.AddSeconds(5);
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (client.Player.Map == Map)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.SobNpc.SeedDistrance)
                        {
                            client.Send(stream);
                        }

                    }
                }
            }
            if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures, StaticMesh.LeftGate))
            {
                if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null)
                {
                    Instance.Guild guild;
                    if (Instance.Guild.GuildPoll.TryGetValue(Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID, out guild))
                    {
                        guild.SendMessajGuild("[GuildWar] The left gate has been breached!");
                    
                    }
                }
                Mesh = StaticMesh.OpenLeftGate;
                Game.MsgTournaments.MsgSchedules.GuildWar.ClosableLeftGateStamp = DateTime.Now.AddSeconds(5);
                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (client.Player.Map == Map)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= Role.SobNpc.SeedDistrance)
                        {
                            client.Send(stream);
                        }

                    }
                }
            }

            if (UID == 890)//what is ID?! Of ClanWar 890
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                var clanWar = Game.MsgTournaments.MsgSchedules.ClanWar.GetWar(killer);// all clan war maps run at once
                if (clanWar != null)
                    clanWar.UpdateScore(killer.Player, Damage);
            }

            else  if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.GuildWar.UpdateScore(killer.Player, Damage);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures, Game.MsgTournaments.MsgSuperGuildWar.FurnituresType.Pole))
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.SuperGuildWar.UpdateScore(killer.Player, Damage);
            }
            #region ClassPoleWar
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgWindWalker.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgWindWalker.UpdateScore(stream, Damage, killer.Player);
            }

            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgKungfuKing.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgKungfuKing.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgArcherClass.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgArcherClass.UpdateScore(stream, Damage, killer.Player);
            }

            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgFireClass.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgFireClass.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgMonkClass.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgMonkClass.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgNinjaClass.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNinjaClass.UpdateScore(stream, Damage, killer.Player);
            }

            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgTrojanClass.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgTrojanClass.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgWarriorClass.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgWarriorClass.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgWaterClass.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgWaterClass.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgPariteClass.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgPariteClass.UpdateScore(stream, Damage, killer.Player);
            }
            #endregion
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.GuildWarScore.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgSchedules.GuildWarScore.UpdateScore(stream, Damage, killer.Player.MyGuild);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.EliteGuildWar.UpdateScore(killer.Player, Damage);
            }
            //else if (UID == 22348)
            //{
            //    uint Damage = (uint)HitPoints;
            //    if (HitPoints > 0)
            //    {
            //        HitPoints = 0;
            //    }
            //    Game.MsgTournaments.MsgSchedules.UnionWar.UpdateScore(killer.Player, Damage);
            //}
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgNobilityPole.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNobilityPole.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgNobilityPole1.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNobilityPole1.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgNobilityPole2.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNobilityPole2.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == Conquer6609.Role.FurnitureUid.Of(Game.MsgTournaments.MsgNobilityPole3.Furnitures, StaticMesh.Pole))
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNobilityPole3.UpdateScore(stream, Damage, killer.Player);
            }
            else if (Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.ContainsKey(UID))
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.CaptureTheFlag.UpdateFlagScore(killer.Player, this, 0, stream);

            }
            else if (HitPoints > 0)
            {
                HitPoints = 0;
            }

        }
        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;
            packet.Strings = args;

            SendScrennPacket(stream.StringPacketCreate(packet));
        }
       
        public unsafe void SendScrennPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == Map)
                {
                    if (client.Player.GetMyDistance(X, Y) < SeedDistrance)
                    {
                        client.Send(packet);
                    }
                }
            }
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool view)
        {
            if (statue != null)
            {
                if (statue.StatuePacket != null && statue.Static)
                {
                    stream.Seek(0);
                    fixed (byte* ptr = statue.StatuePacket)
                    {
                        stream.memcpy(stream.Memory, ptr, statue.StatuePacket.Length);
                    }
                    stream.Size = statue.StatuePacket.Length;
                    return stream;
                }
                stream.InitWriter();

                stream.Write(Extensions.Time32.Now.Value);
                stream.Write((uint)(statue.user.Player.TransformationID * 10000000 + statue.user.Player.Face * 10000 + statue.user.Player.Body));
                stream.Write(UID);
                stream.Write(statue.user.Player.GuildID);
                stream.Write((ushort)statue.user.Player.GuildRank);
                stream.Write((uint)0);//unknow

                for (int x = 0; x < statue.user.Player.BitVector.bits.Length; x++)
                    stream.Write(0);//out flags

                stream.Write((ushort)0);//apparence type
                stream.Write(statue.user.Player.HeadId);
                stream.Write(statue.user.Player.GarmentId);
                stream.Write(statue.user.Player.ArmorId);
                stream.Write(statue.user.Player.LeftWeaponId);
                stream.Write(statue.user.Player.RightWeaponId);
                stream.Write(statue.user.Player.LeftWeaponAccessoryId);
                stream.Write(statue.user.Player.RightWeaponAccessoryId);
                stream.Write(statue.user.Player.SteedId);
                stream.Write(statue.user.Player.MountArmorId);
                stream.Write(statue.user.Player.WingId);
                stream.Write((byte)statue.user.Player.WingPlus);//talisman plus
                stream.Write(statue.user.Player.WingProgress);
                stream.ZeroFill(6);//unknow

                stream.Write(HitPoints / 500);
                stream.Write((ushort)0);//unknow
                stream.Write((ushort)0);//monster level

                stream.Write(X);
                stream.Write(Y);
                stream.Write(statue.user.Player.Hair);
                if (statue.Static)
                    stream.Write((byte)0);
                else
                    stream.Write((byte)statue.user.Player.Angle);
                if (statue.Static)
                    stream.Write((uint)Role.Flags.ConquerAction.Sit);
                else
                    stream.Write((uint)statue.Action);
                stream.Write((ushort)statue.Action2);//unknow
                stream.Write((byte)0);//padding?
                stream.Write(statue.user.Player.Reborn);
                stream.Write(statue.user.Player.Level);
                stream.Write((byte)0);
                stream.Write((byte)0);//away
                stream.Write(statue.user.Player.ExtraBattlePower);
                stream.Write((uint)0);//unknow position = 125
                stream.Write((uint)0);//unknow position = 129
                stream.Write((uint)0);//unknow p = 133;
                stream.Write((uint)(statue.user.Player.FlowerRank + 10000));
                stream.Write((uint)statue.user.Player.NobilityRank);

                stream.Write(statue.user.Player.ColorArmor);
                stream.Write(statue.user.Player.ColorShield);
                stream.Write(statue.user.Player.ColorHelment);
                stream.Write((uint)0);//quiz points
                stream.Write(statue.user.Player.SteedPlus);
                stream.Write((ushort)0);//unknow
                stream.Write(statue.user.Player.SteedColor);
                stream.Write((ushort)statue.user.Player.Enilghten);
                stream.Write((ushort)0);//merit points
                stream.Write((uint)0);//unknow
                stream.Write((uint)0);//unknow
                stream.Write(statue.user.Player.ClanUID);
                stream.Write(statue.user.Player.ClanRank);
                stream.Write(0);//p = 187
                stream.Write((ushort)0);//unknow
                stream.Write(statue.user.Player.MyTitle);

                stream.ZeroFill(14);
                stream.Write(statue.user.Player.HeadSoul);
                stream.Write(statue.user.Player.ArmorSoul);
                stream.Write(statue.user.Player.LeftWeapsonSoul);
                stream.Write(statue.user.Player.RightWeapsonSoul);
                stream.Write((byte)statue.user.Player.ActiveSublass);
                stream.Write(statue.user.Player.SubClassHasPoints);
                stream.Write((uint)0);//unknow
                stream.Write((ushort)statue.user.Player.FirstClass);
                stream.Write((ushort)statue.user.Player.SecoundeClass);
                stream.Write((ushort)statue.user.Player.Class);
                stream.Write((ushort)statue.user.Player.CountryID);//unknow
                stream.Write((uint)0);
                stream.Write(statue.user.Player.BattlePower);
                stream.Write(statue.user.Player.JiangHuTalent);
                stream.Write(statue.user.Player.JiangHuActive);
                /*  stream.ZeroFill(10);
                  stream.Write(0);
                  stream.Write(0);
                  stream.Write(0);*/
                stream.Write((ushort)0);

                stream.Write((byte)0);
                stream.Write((uint)0);
                stream.Write((byte)2);//clone count 
                stream.Write((ushort)0); // clone ID
                stream.Write(0); //clone owner
                stream.Write((ushort)0);//3

                stream.Write(10);//union id
                stream.Write((ushort)0);//1
                stream.Write((uint)0);//union type 
                stream.Write(ushort.MaxValue);//1 = union leader;

                stream.Write(statue.user.Player.SpecialTitleID);
                stream.Write(statue.user.Player.SpecialTitleScore);
                stream.Write(statue.user.Player.SpecialWingID);

                stream.Write((uint)statue.user.Player.MainFlag);
                stream.Write(0);
                stream.Write(Name, string.Empty, statue.user.Player.ClanName);

                stream.Finalize(Game.GamePackets.SpawnPlayer);
                if (statue.StatuePacket == null && statue.Static)
                {
                    statue.StatuePacket = new byte[stream.Size];
                    int size = stream.Size;
                    fixed (byte* ptr = statue.StatuePacket)
                    {
                        stream.memcpy(ptr, stream.Memory, size);
                    }
                }
                return stream;
            }
            stream.InitWriter();

            stream.Write(UID);
            stream.Write(0);
            stream.Write(MaxHitPoints);
            stream.Write(HitPoints);
            stream.Write(X);
            stream.Write(Y);
            stream.Write((ushort)Mesh);
            stream.Write((ushort)Type);
            stream.Write((uint)Sort);
            stream.Write((uint)0);
            stream.Write((uint)0);
            if (Name != null)
            {
                if (Name.Length > 16)
                    Name = Name.Substring(0, 16);

                stream.Write(Name);
            }
            stream.Finalize(Game.GamePackets.SobNpcs);

            return stream;


        }
     }
}
